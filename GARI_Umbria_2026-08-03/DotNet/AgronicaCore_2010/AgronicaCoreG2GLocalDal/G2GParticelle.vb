Imports System.Transactions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Data.Entity

Public Class G2GParticelle_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Nuovo_Particelle_G2G(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Sa_Cod As Integer, ByVal To_Progressivo As Integer) As G2G_Particelle

        Dim g2g As New G2G_Particelle
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Sa_Cod = To_Sa_Cod
            .To_Progressivo = To_Progressivo
            .ParticelleToInsert = New List(Of G2G_Particella)
            .ParticelleToUpdate = New List(Of G2G_Particella)
            .ParticelleToDelete = New List(Of G2G_Particella)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Nuovo_Particelle_G2GReverse(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Sa_Cod As Integer, ByVal To_Progressivo As Integer) As G2G_Particelle_Reverse

        Dim g2g As New G2G_Particelle_Reverse
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Sa_Cod = To_Sa_Cod
            .To_Progressivo = To_Progressivo
            .ParticelleToInsert = New List(Of G2G_Particella)
            .ParticelleToUpdate = New List(Of G2G_Particella)
            .ParticelleToDelete = New List(Of G2G_Particella)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Leggi_Particelle_G2G(ByVal Sa_Cod As Integer, ByRef g2g As G2G_Particelle, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParticelle_R.Leggi_Particelle_G2G()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Data = objParametri.FinestraTemporaleInizio
        Dim To_Data = objParametri.FinestraTemporaleFine

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaParticelle = (
                From ip In GiasContext.ImpreseXParticelle
                Join p In GiasContext.ParticelleCatastali On ip.PROV Equals p.PROV And ip.COM Equals p.COM And ip.SEZIONE Equals p.SEZIONE And ip.FOGLIO Equals p.FOGLIO And ip.NUMERO Equals p.NUMERO And ip.SUBALTERNO Equals p.SUBALTERNO
                Where ip.PIVA = From_Piva AndAlso ip.sa_cod = Sa_Cod AndAlso ip.Validita_Inizio <= To_Data AndAlso ip.Validita_Fine >= From_Data Select p).Distinct().ToList()

            For Each particella In listaParticelle

                Dim impreseParticella = (From ip In GiasContext.ImpreseXParticelle Where ip.PIVA = From_Piva AndAlso ip.sa_cod = Sa_Cod AndAlso
                                  ip.PROV = particella.PROV And ip.COM = particella.COM And ip.SEZIONE = particella.SEZIONE And
                                  ip.FOGLIO = particella.FOGLIO And ip.NUMERO = particella.NUMERO And ip.SUBALTERNO = particella.SUBALTERNO AndAlso
                                  ip.Validita_Inizio <= To_Data AndAlso ip.Validita_Fine >= From_Data Select ip).ToList()

                Dim zone = (From zp In GiasContext.ZonexParticelle Where zp.Piva_SuperUser = From_PivaSuperUser And
                                  zp.PROV = particella.PROV And zp.COM = particella.COM And zp.SEZIONE = particella.SEZIONE And
                                  zp.FOGLIO = particella.FOGLIO And zp.NUMERO = particella.NUMERO And zp.SUBALTERNO = particella.SUBALTERNO Select zp
                                  ).ToList()

                Dim macrousi = (From pcm In GiasContext.ParticelleCatastalixMacrousi Where pcm.PIVA = From_Piva And
                                  pcm.PROV = particella.PROV And pcm.COM = particella.COM And pcm.SEZIONE = particella.SEZIONE And
                                  pcm.FOGLIO = particella.FOGLIO And pcm.NUMERO = particella.NUMERO And pcm.SUBALTERNO = particella.SUBALTERNO Select pcm
                                  ).ToList()

                Dim utilizzi = (From pcmu In GiasContext.ParticelleCatastalixMacrousixUtilizzo Where pcmu.PIVA = From_Piva And
                                  pcmu.PROV = particella.PROV And pcmu.COM = particella.COM And pcmu.SEZIONE = particella.SEZIONE And
                                  pcmu.FOGLIO = particella.FOGLIO And pcmu.NUMERO = particella.NUMERO And pcmu.SUBALTERNO = particella.SUBALTERNO Select pcmu
                                  ).ToList()

                g2g.ParticelleToInsert.Add(New G2G_Particella With {.Particella = particella, .ImpreseParticella = impreseParticella, .Zone = zone, .Macrousi = macrousi, .Utilizzi = utilizzi})

            Next

        End Using

        Return g2g.ParticelleToInsert.Count > 0

    End Function

    Public Function Leggi_Particelle_G2G_Reverse(ByVal Sa_Cod As Integer, ByRef g2g As G2G_Particelle_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParticelle_R.Leggi_Particelle_G2G_Reverse()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Data = objParametri.FinestraTemporaleInizio
        Dim To_Data = objParametri.FinestraTemporaleFine

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaParticelle = (
                From ip In GiasContext.ImpreseXParticelle
                Join p In GiasContext.ParticelleCatastali On ip.PROV Equals p.PROV And ip.COM Equals p.COM And ip.SEZIONE Equals p.SEZIONE And ip.FOGLIO Equals p.FOGLIO And ip.NUMERO Equals p.NUMERO And ip.SUBALTERNO Equals p.SUBALTERNO
                Where ip.PIVA = From_Piva AndAlso ip.sa_cod = Sa_Cod AndAlso ip.Validita_Inizio <= To_Data AndAlso ip.Validita_Fine >= From_Data Select p).Distinct().ToList()

            For Each particella In listaParticelle

                Dim impreseParticella = (From ip In GiasContext.ImpreseXParticelle Where ip.PIVA = From_Piva AndAlso ip.sa_cod = Sa_Cod AndAlso
                                  ip.PROV = particella.PROV And ip.COM = particella.COM And ip.SEZIONE = particella.SEZIONE And
                                  ip.FOGLIO = particella.FOGLIO And ip.NUMERO = particella.NUMERO And ip.SUBALTERNO = particella.SUBALTERNO AndAlso
                                  ip.Validita_Inizio <= To_Data AndAlso ip.Validita_Fine >= From_Data Select ip).ToList()

                Dim zone = (From zp In GiasContext.ZonexParticelle Where zp.Piva_SuperUser = From_PivaSuperUser And
                                  zp.PROV = particella.PROV And zp.COM = particella.COM And zp.SEZIONE = particella.SEZIONE And
                                  zp.FOGLIO = particella.FOGLIO And zp.NUMERO = particella.NUMERO And zp.SUBALTERNO = particella.SUBALTERNO Select zp
                                  ).ToList()

                Dim macrousi = (From pcm In GiasContext.ParticelleCatastalixMacrousi Where pcm.PIVA = From_Piva And
                                  pcm.PROV = particella.PROV And pcm.COM = particella.COM And pcm.SEZIONE = particella.SEZIONE And
                                  pcm.FOGLIO = particella.FOGLIO And pcm.NUMERO = particella.NUMERO And pcm.SUBALTERNO = particella.SUBALTERNO Select pcm
                                  ).ToList()

                Dim utilizzi = (From pcmu In GiasContext.ParticelleCatastalixMacrousixUtilizzo Where pcmu.PIVA = From_Piva And
                                  pcmu.PROV = particella.PROV And pcmu.COM = particella.COM And pcmu.SEZIONE = particella.SEZIONE And
                                  pcmu.FOGLIO = particella.FOGLIO And pcmu.NUMERO = particella.NUMERO And pcmu.SUBALTERNO = particella.SUBALTERNO Select pcmu
                                  ).ToList()

                g2g.ParticelleToInsert.Add(New G2G_Particella With {.Particella = particella, .ImpreseParticella = impreseParticella, .Zone = zone, .Macrousi = macrousi, .Utilizzi = utilizzi})

            Next

        End Using

        Dim PivaSuperUser_Appoggio = ""
        PivaSuperUser_Appoggio = g2g.From_PivaSuperUser
        g2g.From_PivaSuperUser = g2g.To_PivaSuperUser
        g2g.To_PivaSuperUser = PivaSuperUser_Appoggio

        g2g.From_Sa_Cod = g2g.To_Sa_Cod
        g2g.To_Sa_Cod = Sa_Cod

        Return g2g.ParticelleToInsert.Count > 0

    End Function

End Class

Public Class G2GParticelle_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Particelle_G2G(ByRef g2g As G2G_Particelle, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParticelle_W.Scrivi_Particelle_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_Particelle(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_Particelle(g2g, objParametri)

    End Function

    Public Function Scrivi_Particelle_G2GReverse(ByRef g2g As G2G_Particelle_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParticelle_W.Scrivi_Particelle_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_ParticelleReverse(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_ParticelleReverse(g2g, objParametri)

    End Function

    Public Function Scrivi_Particelle(ByRef g2g As G2G_Particelle, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParticelle_W.Scrivi_Particelle()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim To_Sa_Cod = g2g.To_Sa_Cod

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim BaseCode As Long = 0
        Dim TopCode As Long = 2000000000
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        objSequenze.Calcola_BaseCode(g2g.To_Progressivo, TopCode, BaseCode, objParametri)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' PARTICELLE DA INSERIRE
                If g2g.ParticelleToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each p As G2G_Particella In g2g.ParticelleToInsert

                        Dim particella = (From pc In GiasContext.ParticelleCatastali
                                          Where pc.PROV = p.Particella.PROV And pc.COM = p.Particella.COM And pc.SEZIONE = p.Particella.SEZIONE And
                                                pc.FOGLIO = p.Particella.FOGLIO And pc.NUMERO = p.Particella.NUMERO And pc.SUBALTERNO = p.Particella.SUBALTERNO).FirstOrDefault()

                        If particella Is Nothing Then

                            ' nuova particella
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "ParticelleCatastali", 0, 2000000000, objParametri)
                            particella = Gias_EF_Utility.CopyEntity(GiasContext, p.Particella, Nothing, username, data)
                            particella.PART_COD = idSeq
                            GiasContext.ParticelleCatastali.Add(particella)

                        Else

                            'modifica particella
                            Dim To_PART_COD = particella.PART_COD
                            particella = Gias_EF_Utility.CopyEntity(GiasContext, p.Particella, particella, username, data)
                            particella.PART_COD = To_PART_COD
                            GiasContext.ParticelleCatastali.Attach(particella)
                            GiasContext.Entry(particella).State = EntityState.Modified

                        End If

                        Dim imprese_particella = (From ip In GiasContext.ImpreseXParticelle
                                                  Where ip.PIVA = To_Piva And ip.sa_cod = To_Sa_Cod And ip.PROV = particella.PROV And ip.COM = particella.COM And ip.SEZIONE = particella.SEZIONE And
                                                        ip.FOGLIO = particella.FOGLIO And ip.NUMERO = particella.NUMERO And ip.SUBALTERNO = particella.SUBALTERNO).ToList()

                        ' cancella imprese particelle
                        For Each ip In imprese_particella
                            GiasContext.ImpreseXParticelle.Attach(ip)
                            GiasContext.ImpreseXParticelle.Remove(ip)
                        Next

                        ' inserisce imprese particella
                        For Each ip As ImpreseXParticelle In p.ImpreseParticella
                            Dim impresa_particella = Gias_EF_Utility.CopyEntity(GiasContext, ip, Nothing, username, data)
                            impresa_particella.PIVA = To_Piva
                            impresa_particella.sa_cod = To_Sa_Cod
                            GiasContext.ImpreseXParticelle.Add(impresa_particella)
                        Next

                        ' inserisce/modifica zone particelle
                        For Each zona As ZonexParticelle In p.Zone
                            Dim zona_particella = (From zp In GiasContext.ZonexParticelle Where zp.Piva_SuperUser = To_PivaSuperUser And zp.Zona_Cod = zona.Zona_Cod And
                                                    zp.PROV = zona.PROV And zp.COM = zona.COM And zp.SEZIONE = zona.SEZIONE And
                                                    zp.FOGLIO = zona.FOGLIO And zp.NUMERO = zona.NUMERO And zp.SUBALTERNO = zona.SUBALTERNO).FirstOrDefault()
                            If zona_particella Is Nothing Then
                                zona_particella = Gias_EF_Utility.CopyEntity(GiasContext, zona, Nothing, username, data)
                                zona_particella.Piva_SuperUser = To_PivaSuperUser
                                GiasContext.ZonexParticelle.Add(zona_particella)
                            Else
                                zona_particella = Gias_EF_Utility.CopyEntity(GiasContext, zona, zona_particella, username, data)
                                zona_particella.Piva_SuperUser = To_PivaSuperUser
                                GiasContext.ZonexParticelle.Attach(zona_particella)
                                GiasContext.Entry(zona_particella).State = EntityState.Modified
                            End If
                        Next

                        Dim macrousi_particella = (From mp In GiasContext.ParticelleCatastalixMacrousi Where mp.PIVA = To_Piva And
                                                    mp.PROV = particella.PROV And mp.COM = particella.COM And mp.SEZIONE = particella.SEZIONE And
                                                    mp.FOGLIO = particella.FOGLIO And mp.NUMERO = particella.NUMERO And mp.SUBALTERNO = particella.SUBALTERNO).ToList()
                        ' cancella macrousi particella
                        For Each mp In macrousi_particella
                            GiasContext.ParticelleCatastalixMacrousi.Attach(mp)
                            GiasContext.ParticelleCatastalixMacrousi.Remove(mp)
                        Next

                        ' inserisce macrousi particelle
                        For Each macrouso As ParticelleCatastalixMacrousi In p.Macrousi
                            Dim macrouso_particella = Gias_EF_Utility.CopyEntity(GiasContext, macrouso, Nothing, username, data)
                            macrouso_particella.PIVA = To_Piva
                            GiasContext.ParticelleCatastalixMacrousi.Add(macrouso_particella)
                        Next

                        Dim utilizzi_particella = (From mup In GiasContext.ParticelleCatastalixMacrousixUtilizzo Where mup.PIVA = To_Piva And
                                                    mup.PROV = particella.PROV And mup.COM = particella.COM And mup.SEZIONE = particella.SEZIONE And
                                                    mup.FOGLIO = particella.FOGLIO And mup.NUMERO = particella.NUMERO And mup.SUBALTERNO = particella.SUBALTERNO).ToList()

                        ' cancella macrousi utilizzi particella
                        For Each up In utilizzi_particella
                            GiasContext.ParticelleCatastalixMacrousixUtilizzo.Attach(up)
                            GiasContext.ParticelleCatastalixMacrousixUtilizzo.Remove(up)
                        Next

                        ' inserisce macrouso utilizzo particella
                        For Each utilizzo As ParticelleCatastalixMacrousixUtilizzo In p.Utilizzi
                            Dim utilizzo_particella = Gias_EF_Utility.CopyEntity(GiasContext, utilizzo, Nothing, username, data)
                            utilizzo_particella.PIVA = To_Piva
                            GiasContext.ParticelleCatastalixMacrousixUtilizzo.Add(utilizzo_particella)
                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode = g2g.ParticelleToInsert.Count & " nuovi / modificati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_ParticelleReverse(ByRef g2g As G2G_Particelle_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParticelle_W.Scrivi_Particelle()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim To_Sa_Cod = g2g.From_Sa_Cod

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim BaseCode As Long = 0
        Dim TopCode As Long = 2000000000
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        objSequenze.Calcola_BaseCode(g2g.To_Progressivo, TopCode, BaseCode, objParametri)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' PARTICELLE DA INSERIRE
                If g2g.ParticelleToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each p As G2G_Particella In g2g.ParticelleToInsert

                        Dim particella = (From pc In GiasContext.ParticelleCatastali
                                          Where pc.PROV = p.Particella.PROV And pc.COM = p.Particella.COM And pc.SEZIONE = p.Particella.SEZIONE And
                                                pc.FOGLIO = p.Particella.FOGLIO And pc.NUMERO = p.Particella.NUMERO And pc.SUBALTERNO = p.Particella.SUBALTERNO).FirstOrDefault()

                        If particella Is Nothing Then

                            ' nuova particella
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "ParticelleCatastali", 0, 2000000000, objParametri)
                            particella = Gias_EF_Utility.CopyEntity(GiasContext, p.Particella, Nothing, username, data)
                            particella.PART_COD = idSeq
                            GiasContext.ParticelleCatastali.Add(particella)

                        Else

                            'modifica particella
                            Dim To_PART_COD = particella.PART_COD
                            particella = Gias_EF_Utility.CopyEntity(GiasContext, p.Particella, particella, username, data)
                            particella.PART_COD = To_PART_COD
                            GiasContext.ParticelleCatastali.Attach(particella)
                            GiasContext.Entry(particella).State = EntityState.Modified

                        End If

                        Dim imprese_particella = (From ip In GiasContext.ImpreseXParticelle
                                                  Where ip.PIVA = To_Piva And ip.sa_cod = To_Sa_Cod And ip.PROV = particella.PROV And ip.COM = particella.COM And ip.SEZIONE = particella.SEZIONE And
                                                        ip.FOGLIO = particella.FOGLIO And ip.NUMERO = particella.NUMERO And ip.SUBALTERNO = particella.SUBALTERNO).ToList()

                        ' cancella imprese particelle
                        For Each ip In imprese_particella
                            GiasContext.ImpreseXParticelle.Attach(ip)
                            GiasContext.ImpreseXParticelle.Remove(ip)
                        Next

                        ' inserisce imprese particella
                        For Each ip As ImpreseXParticelle In p.ImpreseParticella
                            Dim impresa_particella = Gias_EF_Utility.CopyEntity(GiasContext, ip, Nothing, username, data)
                            impresa_particella.PIVA = To_Piva
                            impresa_particella.sa_cod = To_Sa_Cod
                            GiasContext.ImpreseXParticelle.Add(impresa_particella)
                        Next

                        ' inserisce/modifica zone particelle
                        For Each zona As ZonexParticelle In p.Zone
                            Dim zona_particella = (From zp In GiasContext.ZonexParticelle Where zp.Piva_SuperUser = From_PivaSuperUser And zp.Zona_Cod = zona.Zona_Cod And
                                                    zp.PROV = zona.PROV And zp.COM = zona.COM And zp.SEZIONE = zona.SEZIONE And
                                                    zp.FOGLIO = zona.FOGLIO And zp.NUMERO = zona.NUMERO And zp.SUBALTERNO = zona.SUBALTERNO).FirstOrDefault()
                            If zona_particella Is Nothing Then
                                zona_particella = Gias_EF_Utility.CopyEntity(GiasContext, zona, Nothing, username, data)
                                zona_particella.Piva_SuperUser = From_PivaSuperUser
                                GiasContext.ZonexParticelle.Add(zona_particella)
                            Else
                                zona_particella = Gias_EF_Utility.CopyEntity(GiasContext, zona, zona_particella, username, data)
                                zona_particella.Piva_SuperUser = From_PivaSuperUser
                                GiasContext.ZonexParticelle.Attach(zona_particella)
                                GiasContext.Entry(zona_particella).State = EntityState.Modified
                            End If
                        Next

                        Dim macrousi_particella = (From mp In GiasContext.ParticelleCatastalixMacrousi Where mp.PIVA = To_Piva And
                                                    mp.PROV = particella.PROV And mp.COM = particella.COM And mp.SEZIONE = particella.SEZIONE And
                                                    mp.FOGLIO = particella.FOGLIO And mp.NUMERO = particella.NUMERO And mp.SUBALTERNO = particella.SUBALTERNO).ToList()
                        ' cancella macrousi particella
                        For Each mp In macrousi_particella
                            GiasContext.ParticelleCatastalixMacrousi.Attach(mp)
                            GiasContext.ParticelleCatastalixMacrousi.Remove(mp)
                        Next

                        ' inserisce macrousi particelle
                        For Each macrouso As ParticelleCatastalixMacrousi In p.Macrousi
                            Dim macrouso_particella = Gias_EF_Utility.CopyEntity(GiasContext, macrouso, Nothing, username, data)
                            macrouso_particella.PIVA = To_Piva
                            GiasContext.ParticelleCatastalixMacrousi.Add(macrouso_particella)
                        Next

                        Dim utilizzi_particella = (From mup In GiasContext.ParticelleCatastalixMacrousixUtilizzo Where mup.PIVA = To_Piva And
                                                    mup.PROV = particella.PROV And mup.COM = particella.COM And mup.SEZIONE = particella.SEZIONE And
                                                    mup.FOGLIO = particella.FOGLIO And mup.NUMERO = particella.NUMERO And mup.SUBALTERNO = particella.SUBALTERNO).ToList()

                        ' cancella macrousi utilizzi particella
                        For Each up In utilizzi_particella
                            GiasContext.ParticelleCatastalixMacrousixUtilizzo.Attach(up)
                            GiasContext.ParticelleCatastalixMacrousixUtilizzo.Remove(up)
                        Next

                        ' inserisce macrouso utilizzo particella
                        For Each utilizzo As ParticelleCatastalixMacrousixUtilizzo In p.Utilizzi
                            Dim utilizzo_particella = Gias_EF_Utility.CopyEntity(GiasContext, utilizzo, Nothing, username, data)
                            utilizzo_particella.PIVA = To_Piva
                            GiasContext.ParticelleCatastalixMacrousixUtilizzo.Add(utilizzo_particella)
                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode = g2g.ParticelleToInsert.Count & " nuovi / modificati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

End Class
