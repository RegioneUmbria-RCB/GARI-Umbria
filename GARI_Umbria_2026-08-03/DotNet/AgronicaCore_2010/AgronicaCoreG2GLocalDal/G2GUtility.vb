Imports System.Data.Entity.Core.Objects
Imports System.Reflection
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Data.Entity

Public Class G2GUtility

    Public Const transactionScope As Boolean = False
    Public Const saveCount As Integer = 0
    Public Const statoIdle As String = "-999999"
    Public Const SeparatoreChiave As String = "|"

    Public Shared Function GetTransactionTimeout(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As TimeSpan
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim timeout = objConfigSiti.Leggi_Valore(0, "G2GTransactionTimeout", "", "", objParametri)
        Return If(String.IsNullOrEmpty(timeout), TransactionManager.MaximumTimeout, New TimeSpan(0, CInt(timeout), 0))
    End Function

    Public Shared Sub SaveChanges(ByRef GiasContext As Gias_DeveloperServer_Entities, ByRef count As Integer)
        count += 1
        If transactionScope AndAlso saveCount > 0 AndAlso count Mod saveCount = 0 Then
            GiasContext.SaveChanges()
        End If
    End Sub

    Public Shared Sub Log(ByRef log As StringBuilder, ByRef info As String)
        log.Append(Date.Now & " - " & info & vbCrLf)
    End Sub

    Public Shared Function NuovoId_Tabella(ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByVal NomeTabella As String,
                                    ByVal Base As Integer,
                                    ByVal Fine As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
        'Dim saveChanges = G2GUtility.saveCount = 0
        Dim idSeq As Integer = 0

        If G2GUtility.transactionScope AndAlso GiasContext IsNot Nothing Then
            idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, NomeTabella, Base, Fine, objParametri)
        Else
            idSeq = objSequenze.NuovoId_Tabella(NomeTabella, Base, Fine, objParametri)
        End If

        Return idSeq

    End Function

    Public Shared Sub RecuparaListe(oConfigurazione As G2G_Configurazione_FiltriReq_Pratiche, listaVerificaContains As List(Of String), listaVerificaStatoAttuale As List(Of String))
        For Each sww In oConfigurazione.listaServiziWWorkflow

            If sww.PassaggiDiStatoEsclusi.Count = 0 Then

                listaVerificaContains.Add(sww.Servizio_Cod & SeparatoreChiave & statoIdle & SeparatoreChiave & statoIdle)
                listaVerificaStatoAttuale.Add(sww.Servizio_Cod & SeparatoreChiave & statoIdle)

            Else

                For Each st In sww.PassaggiDiStatoEsclusi

                    'per verifica passaggi di stato
                    listaVerificaContains.Add(sww.Servizio_Cod & SeparatoreChiave & st.Stato_Iniziale & SeparatoreChiave & st.Stato_Finale)

                    'per verifica dello stato attuale
                    Dim sfServ As String = sww.Servizio_Cod & SeparatoreChiave & st.Stato_Finale
                    If Not listaVerificaStatoAttuale.Contains(sfServ) Then
                        listaVerificaStatoAttuale.Add(sfServ)
                    End If
                Next
            End If


        Next
    End Sub

    Public Shared Function Nuovo_G2G_Recode() As G2G_Recode

        Dim recode As New G2G_Recode

        AgronicaCoreUtility.ClassiReflection.PropertyImpostaSeNothing(Of G2G_Recode)(recode, "LogRecode,MessaggioErrore")

        Return recode

    End Function

    Private Shared Sub Elabora_G2G_Pua(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Pua), ByVal ListaUpdate As List(Of G2G_Recode_Pua))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Pua.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Piva = elem.From_Piva AndAlso g.From_Pua_Cod = elem.From_Pua_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_ParticelleCatastalixVincoliAgronomici(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_ParticelleCatastalixVincoliAgronomici), ByVal ListaUpdate As List(Of G2G_Recode_ParticelleCatastalixVincoliAgronomici))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Id = elem.From_Id) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Programmazione_Testata(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Programmazione_Testata), ByVal ListaUpdate As List(Of G2G_Recode_Programmazione_Testata))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Programmazione_Testata.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Piva = elem.From_Piva AndAlso g.From_Programmazione_Cod = elem.From_Programmazione_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Programmazione_Entita(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Programmazione_Entita), ByVal ListaUpdate As List(Of G2G_Recode_Programmazione_Entita))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Programmazione_Entita.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Piva = elem.From_Piva AndAlso g.From_Programmazione_Entita_Cod = elem.From_Programmazione_Entita_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Allegati(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Allegati), ByVal ListaUpdate As List(Of G2G_Recode_Allegati))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Allegati.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Allegati_Documenti_Cod = elem.From_Allegati_Documenti_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Allegati_Entita(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Allegati_Entita), ByVal ListaUpdate As List(Of G2G_Recode_Allegati_Entita))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Allegati_Entita.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_ID = elem.From_ID) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_ParcoMacchine(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Parco_Macchine), ByVal ListaUpdate As List(Of G2G_Recode_Parco_Macchine))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Parco_Macchine.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Mac_Cod = elem.From_Mac_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_MateriePrime(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_MateriePrime), ByVal ListaUpdate As List(Of G2G_Recode_MateriePrime))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_MateriePrime.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Piva = elem.From_Piva AndAlso g.From_Mat_Cod = elem.From_Mat_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_MateriePrimeCampionature(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Materie_Prime_Campionature), ByVal ListaUpdate As List(Of G2G_Recode_Materie_Prime_Campionature))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Materie_Prime_Campionature.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Progressivo = elem.From_Progressivo AndAlso g.From_Tipo = elem.From_Tipo AndAlso g.From_Tipo_Cod = elem.From_Tipo_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Contatti(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Contatti), ByVal ListaUpdate As List(Of G2G_Recode_Contatti))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Contatti.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Cod_RisUm = elem.From_Cod_RisUm) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Indirizzi(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Indirizzi), ByVal ListaUpdate As List(Of G2G_Recode_Indirizzi))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Indirizzi.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Cod_Indirizzo = elem.From_Cod_Indirizzo) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Pratiche(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Pratiche), ByVal ListaUpdate As List(Of G2G_Recode_Pratiche))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Pratiche.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Pratica_Cod = elem.From_Pratica_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Pratiche_stati(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Pratiche_Stati), ByVal ListaUpdate As List(Of G2G_Recode_Pratiche_Stati))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Pratiche_Stati.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_PassaggioDiStato_Cod = elem.From_PassaggioDiStato_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_ImpreseCentri(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Imprese), ByVal ListaUpdate As List(Of G2G_Recode_Imprese))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Imprese.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.FROM_Piva = elem.FROM_Piva AndAlso g.FROM_SaCod = elem.FROM_SaCod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Campi(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Campo), ByVal ListaUpdate As List(Of G2G_Recode_Campo))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Campo.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Piva = elem.From_Piva AndAlso g.From_Sa_Cod = elem.From_Sa_Cod AndAlso g.From_Campo_cod = elem.From_Campo_cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Appezzamenti(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Appezzamenti), ByVal ListaUpdate As List(Of G2G_Recode_Appezzamenti))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Appezzamenti.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Piva = elem.From_Piva AndAlso g.From_Sa_Cod = elem.From_Sa_Cod AndAlso g.From_Appezza = elem.From_Appezza) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub
    Private Shared Sub Elabora_G2G_Impianti(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Impianti), ByVal ListaUpdate As List(Of G2G_Recode_Impianti))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Impianti.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso
                                                       g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso
                                                       g.From_Piva = elem.From_Piva AndAlso
                                                       g.From_Sa_Cod = elem.From_Sa_Cod AndAlso
                                                       g.From_Appezza = elem.From_Appezza AndAlso
                                                       g.From_Id_Reg = elem.From_Id_Reg) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub
    Private Shared Sub Elabora_G2G_Distinte(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Distinta), ByVal ListaUpdate As List(Of G2G_Recode_Distinta))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Distinta.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Piva = elem.From_Piva AndAlso g.From_Progetto_cod = elem.From_Progetto_cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Fabbricati(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Fabbricati), ByVal ListaUpdate As List(Of G2G_Recode_Fabbricati))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Fabbricati.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.FromPiva = elem.FromPiva AndAlso g.FromSa_cod = elem.FromSa_cod AndAlso g.From_FabbricatoCod = elem.From_FabbricatoCod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_PianoConcimazione_Testata(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_PianoConcimazione_Testata), ByVal ListaUpdate As List(Of G2G_Recode_PianoConcimazione_Testata))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_PianoConcimazione_Testata.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_PC_Testata_Cod = elem.From_PC_Testata_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_PianoConcimazione_Dettagli(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_PianoConcimazione_Dettagli), ByVal ListaUpdate As List(Of G2G_Recode_PianoConcimazione_Dettagli))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_PC_Testata_Cod = elem.From_PC_Testata_Cod AndAlso g.From_PC_Dettagli_Cod = elem.From_PC_Dettagli_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_PianoConcimazione_Elaborazioni(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_PianoConcimazione_Elaborazioni), ByVal ListaUpdate As List(Of G2G_Recode_PianoConcimazione_Elaborazioni))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_PC_Elaborazione_Cod = elem.From_PC_Elaborazione_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_PianoConcimazione_EntitaxTestata(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_PianoConcimazione_EntitaxTestata), ByVal ListaUpdate As List(Of G2G_Recode_PianoConcimazione_EntitaxTestata))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Piva = elem.From_Piva AndAlso g.From_PC_Testata_Cod = elem.From_PC_Testata_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Analisi_Certificato(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Analisi_Certificato), ByVal ListaUpdate As List(Of G2G_Recode_Analisi_Certificato))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Analisi_Certificato.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Analisi_Certificato_Cod = elem.From_Analisi_Certificato_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Analisi_Testata(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Analisi_Testata), ByVal ListaUpdate As List(Of G2G_Recode_Analisi_Testata))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Analisi_Testata.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Analisi_Testata_Cod = elem.From_Analisi_Testata_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Analisi_Dettagli(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Analisi_Dettagli), ByVal ListaUpdate As List(Of G2G_Recode_Analisi_Dettagli))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Analisi_Dettagli.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Analisi_Testata_Cod = elem.From_Analisi_Testata_Cod AndAlso g.From_Analisi_Dettaglio_Cod = elem.From_Analisi_Dettaglio_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Analisi_EntitaxTestata(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Analisi_EntitaxTestata), ByVal ListaUpdate As List(Of G2G_Recode_Analisi_EntitaxTestata))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Analisi_EntitaxTestata.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Analisi_Testata_Cod = elem.From_Analisi_Testata_Cod AndAlso g.From_Piva = elem.From_Piva) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Analisi_Campioni(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Analisi_Campioni), ByVal ListaUpdate As List(Of G2G_Recode_Analisi_Campioni))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Analisi_Campioni.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Analisi_Campione_Cod = elem.From_Analisi_Campione_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Ricette(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Ricette), ByVal ListaUpdate As List(Of G2G_Recode_Ricette))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Ricette.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Piva = elem.From_Piva AndAlso g.From_Ricetta_Cod = elem.From_Ricetta_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Ricette_Operazioni(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Ricette_Operazioni), ByVal ListaUpdate As List(Of G2G_Recode_Ricette_Operazioni))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Ricette_Operazioni.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Piva = elem.From_Piva AndAlso g.From_Ricetta_Cod = elem.From_Ricetta_Cod AndAlso g.From_Ricetta_Operazione_Cod = elem.From_Ricetta_Operazione_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Ricette_Dettagli(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Ricette_Dettagli), ByVal ListaUpdate As List(Of G2G_Recode_Ricette_Dettagli))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Ricette_Dettagli.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Piva = elem.From_Piva AndAlso g.From_Ricetta_Cod = elem.From_Ricetta_Cod AndAlso g.From_Ricetta_Operazione_Cod = elem.From_Ricetta_Operazione_Cod AndAlso g.From_Ricetta_Dettaglio_Cod = elem.From_Ricetta_Dettaglio_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Ricette_Dettaglio_Tecnico(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Ricette_Dettaglio_Tecnico), ByVal ListaUpdate As List(Of G2G_Recode_Ricette_Dettaglio_Tecnico))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Piva = elem.From_Piva AndAlso g.From_Ricetta_Cod = elem.From_Ricetta_Cod AndAlso g.From_Ricetta_Operazione_Cod = elem.From_Ricetta_Operazione_Cod AndAlso g.From_Ricetta_Dettaglio_Cod = elem.From_Ricetta_Dettaglio_Cod AndAlso g.From_Ricetta_Tecnico_Cod = elem.From_Ricetta_Tecnico_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Ricette_Destinazioni(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Ricette_Destinazioni), ByVal ListaUpdate As List(Of G2G_Recode_Ricette_Destinazioni))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Ricette_Destinazioni.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_Piva = elem.From_Piva AndAlso g.From_Ricetta_Cod = elem.From_Ricetta_Cod AndAlso g.From_Ricetta_Operazione_Cod = elem.From_Ricetta_Operazione_Cod AndAlso g.From_Ricetta_Dettaglio_Cod = elem.From_Ricetta_Dettaglio_Cod AndAlso g.From_Ricetta_Destinazione_Cod = elem.From_Ricetta_Destinazione_Cod) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_PUA_LetamazioniPrecedenti(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_PUA_LetamazioniPrecedenti), ByVal ListaUpdate As List(Of G2G_Recode_PUA_LetamazioniPrecedenti))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_ID = elem.From_ID) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Private Shared Sub Elabora_G2G_Anagrafe_VincoliAgronomici(ByVal GiasContext As Gias_DeveloperServer_Entities, ByRef ListaInsert As List(Of G2G_Recode_Anagrafe_VincoliAgronomici), ByVal ListaUpdate As List(Of G2G_Recode_Anagrafe_VincoliAgronomici))

        For Each elem In ListaInsert
            If GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Any(Function(g) g.From_PivaSuperUser = elem.From_PivaSuperUser AndAlso g.To_PivaSuperUser = elem.To_PivaSuperUser AndAlso g.From_ID = elem.From_ID) Then
                ListaUpdate.Add(elem)
            End If
        Next

        For Each el1 In ListaUpdate
            ListaInsert.Remove(el1)
        Next

    End Sub

    Public Shared Sub Elabora_Liste_G2G(ByVal GiasContext As Gias_DeveloperServer_Entities, g2g As G2G_Recode)

        'imposta in automatico tutte le property che non sono state inizializzate
        AgronicaCoreUtility.ClassiReflection.PropertyImpostaSeNothing(Of G2G_Recode)(g2g, "LogRecode,MessaggioErrore")

        'imprese, centri aziendali        
        Elabora_G2G_ImpreseCentri(GiasContext, g2g.G2GRecodeImpreseCentriToInsert, g2g.G2GRecodeImpreseCentriToUpdate)

        'piano colturale: campi, appezzamenti, impianti, distinte       
        Elabora_G2G_Campi(GiasContext, g2g.G2GRecodeCampiToInsert, g2g.G2GRecodeCampiToUpdate)
        Elabora_G2G_Appezzamenti(GiasContext, g2g.G2GRecodeAppezzamentiToInsert, g2g.G2GRecodeAppezzamentiToUpdate)
        Elabora_G2G_Impianti(GiasContext, g2g.G2GRecodeImpiantiToInsert, g2g.G2GRecodeImpiantiToUpdate)
        Elabora_G2G_Distinte(GiasContext, g2g.G2GRecodeDistinteToInsert, g2g.G2GRecodeDistinteToUpdate)

        'pratiche        
        Elabora_G2G_Pratiche(GiasContext, g2g.G2GRecodePraticheToInsert, g2g.G2GRecodePraticheToUpdate)

        'pratiche_stati        
        Elabora_G2G_Pratiche_stati(GiasContext, g2g.G2GRecodePraticheStatiToInsert, g2g.G2GRecodePraticheStatiToUpdate)

        'Contatti        
        Elabora_G2G_Contatti(GiasContext, g2g.G2GRecodeContattiToInsert, g2g.G2GRecodeContattiToUpdate)

        'Indirizzi        
        Elabora_G2G_Indirizzi(GiasContext, g2g.G2GRecodeIndirizziToInsert, g2g.G2GRecodeIndirizziToUpdate)

        'Parco Macchine        
        Elabora_G2G_ParcoMacchine(GiasContext, g2g.G2GRecodeParcoMacchineToInsert, g2g.G2GRecodeParcoMacchineToUpdate)

        'Materie Prime        
        Elabora_G2G_MateriePrime(GiasContext, g2g.G2GRecodeMateriePrimeToInsert, g2g.G2GRecodeMateriePrimeToUpdate)

        'Materie Prime Campionature   
        Elabora_G2G_MateriePrimeCampionature(GiasContext, g2g.G2GRecodeMateriePrimeCampionatureToInsert, g2g.G2GRecodeMateriePrimeCampionatureToUpdate)

        'Fabbricati        
        Elabora_G2G_Fabbricati(GiasContext, g2g.G2GRecodeFabbricatiToInsert, g2g.G2GRecodeFabbricatiToUpdate)

        'Programmazione
        Elabora_G2G_Programmazione_Testata(GiasContext, g2g.G2GRecodeProgrammazioneTestataToInsert, g2g.G2GRecodeProgrammazioneTestataToUpdate)

        'Programmazione, Entita
        Elabora_G2G_Programmazione_Entita(GiasContext, g2g.G2GRecodeProgrammazioneEntitaToInsert, g2g.G2GRecodeProgrammazioneEntitaToUpdate)

        'Allegati
        Elabora_G2G_Allegati(GiasContext, g2g.G2GRecodeAllegatiToInsert, g2g.G2GRecodeAllegatiToUpdate)

        'Allegati_Entita
        Elabora_G2G_Allegati_Entita(GiasContext, g2g.G2GRecodeAllegati_EntitaToInsert, g2g.G2GRecodeAllegati_EntitaToUpdate)

        'PUA
        Elabora_G2G_Pua(GiasContext, g2g.G2GRecodePuaToInsert, g2g.G2GRecodePuaToUpdate)

        'ParticelleCatastalixVincoliAgronomici
        Elabora_G2G_ParticelleCatastalixVincoliAgronomici(GiasContext, g2g.G2GRecodeParticelleCatastalixVincoliAgronomiciToInsert, g2g.G2GRecodeParticelleCatastalixVincoliAgronomiciToUpdate)

        'Piano Concimazione
        Elabora_G2G_PianoConcimazione_Testata(GiasContext, g2g.G2GRecodePianoConcimazione_TestataToInsert, g2g.G2GRecodePianoConcimazione_TestataToUpdate)
        Elabora_G2G_PianoConcimazione_Dettagli(GiasContext, g2g.G2GRecodePianoConcimazione_DettagliToInsert, g2g.G2GRecodePianoConcimazione_DettagliToUpdate)
        Elabora_G2G_PianoConcimazione_EntitaxTestata(GiasContext, g2g.G2GRecodePianoConcimazione_EntitaxTestataToInsert, g2g.G2GRecodePianoConcimazione_EntitaxTestataToUpdate)
        Elabora_G2G_PianoConcimazione_Elaborazioni(GiasContext, g2g.G2GRecodePianoConcimazione_ElaborazioniToInsert, g2g.G2GRecodePianoConcimazione_ElaborazioniToUpdate)

        'Analisi
        Elabora_G2G_Analisi_Certificato(GiasContext, g2g.G2GRecodeAnalisi_CertificatoToInsert, g2g.G2GRecodeAnalisi_CertificatoToUpdate)
        Elabora_G2G_Analisi_Testata(GiasContext, g2g.G2GRecodeAnalisi_TestataToInsert, g2g.G2GRecodeAnalisi_TestataToUpdate)
        Elabora_G2G_Analisi_Dettagli(GiasContext, g2g.G2GRecodeAnalisi_DettagliToInsert, g2g.G2GRecodeAnalisi_DettagliToUpdate)
        Elabora_G2G_Analisi_EntitaxTestata(GiasContext, g2g.G2GRecodeAnalisi_EntitaxTestataToInsert, g2g.G2GRecodeAnalisi_EntitaxTestataToUpdate)
        Elabora_G2G_Analisi_Campioni(GiasContext, g2g.G2GRecodeAnalisi_CampioniToInsert, g2g.G2GRecodeAnalisi_CampioniToUpdate)

        'Ricette
        Elabora_G2G_Ricette(GiasContext, g2g.G2GRecodeRicetteToInsert, g2g.G2GRecodeRicetteToUpdate)
        Elabora_G2G_Ricette_Operazioni(GiasContext, g2g.G2GRecodeRicette_OperazioniToInsert, g2g.G2GRecodeRicette_OperazioniToUpdate)
        Elabora_G2G_Ricette_Dettagli(GiasContext, g2g.G2GRecodeRicette_DettagliToInsert, g2g.G2GRecodeRicette_DettagliToUpdate)
        Elabora_G2G_Ricette_Dettaglio_Tecnico(GiasContext, g2g.G2GRecodeRicette_Dettaglio_TecnicoToInsert, g2g.G2GRecodeRicette_Dettaglio_TecnicoToUpdate)
        Elabora_G2G_Ricette_Destinazioni(GiasContext, g2g.G2GRecodeRicette_DestinazioniToInsert, g2g.G2GRecodeRicette_DestinazioniToUpdate)

        'PUA_LetamazioniPrecedenti
        Elabora_G2G_PUA_LetamazioniPrecedenti(GiasContext, g2g.G2GRecodePUA_LetamazioniPrecedentiToInsert, g2g.G2GRecodePUA_LetamazioniPrecedentiToUpdate)

        'Anagrafe_VincoliAgronomici
        Elabora_G2G_Anagrafe_VincoliAgronomici(GiasContext, g2g.G2GRecodeAnagrafe_VincoliAgronomiciToInsert, g2g.G2GRecodeAnagrafe_VincoliAgronomiciToUpdate)

    End Sub

    Public Shared Sub Copia_G2G_Recode(ByRef g2g As G2G_Recode, ByRef g2g2 As G2G_Recode)

        'Fabbricati
        For Each recode As G2G_Recode_Fabbricati In g2g.G2GRecodeFabbricatiToInsert
            g2g2.G2GRecodeFabbricatiToInsert.Add(recode)
        Next
        For Each recode As G2G_Recode_Fabbricati In g2g.G2GRecodeFabbricatiToUpdate
            g2g2.G2GRecodeFabbricatiToUpdate.Add(recode)
        Next
        For Each recode As G2G_Recode_Fabbricati In g2g.G2GRecodeFabbricatiToDelete
            g2g2.G2GRecodeFabbricatiToDelete.Add(recode)
        Next

        'Indirizzi
        For Each recode As G2G_Recode_Indirizzi In g2g.G2GRecodeIndirizziToInsert
            g2g2.G2GRecodeIndirizziToInsert.Add(recode)
        Next
        For Each recode As G2G_Recode_Indirizzi In g2g.G2GRecodeIndirizziToUpdate
            g2g2.G2GRecodeIndirizziToUpdate.Add(recode)
        Next
        For Each recode As G2G_Recode_Indirizzi In g2g.G2GRecodeIndirizziToDelete
            g2g2.G2GRecodeIndirizziToDelete.Add(recode)
        Next

        ' concateno log/ errori
        If Not String.IsNullOrEmpty(g2g.MessaggioErrore) Then
            g2g2.MessaggioErrore &= g2g.MessaggioErrore & vbCrLf
        ElseIf Not String.IsNullOrEmpty(g2g.LogRecode) Then
            g2g2.LogRecode &= g2g.LogRecode & vbCrLf
        End If

    End Sub

    Public Shared Function Scrivi_G2G_Recode(ByRef g2g As G2G_Recode, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional ByVal ElaboraListe As Boolean = False) As String

        Dim nomeRoutine As String = "Scrivi_G2G_Recode()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        If ElaboraListe Then
            Elabora_Liste_G2G(GiasContext, g2g)
        End If

        'Dim transactionOptions As New TransactionOptions()
        'transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        'transactionOptions.Timeout = TransactionManager.MaximumTimeout

        'Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

        Try

            'Imprese / Centri Aziendali
            If g2g.G2GRecodeImpreseCentriToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Imprese)
                For Each recode As G2G_Recode_Imprese In g2g.G2GRecodeImpreseCentriToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Imprese.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeImpreseCentriToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Imprese In g2g.G2GRecodeImpreseCentriToUpdate
                    GiasContext.G2G_Recode_Imprese.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeImpreseCentriToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Imprese In g2g.G2GRecodeImpreseCentriToDelete
                    GiasContext.G2G_Recode_Imprese.Attach(recode)
                    GiasContext.G2G_Recode_Imprese.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Piano Colturale: Centri / Appezzamenti / Impianti / Distinte
            If g2g.G2GRecodeCampiToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Campo In g2g.G2GRecodeCampiToDelete
                    GiasContext.G2G_Recode_Campo.Attach(recode)
                    GiasContext.G2G_Recode_Campo.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeCampiToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Campo)
                For Each recode As G2G_Recode_Campo In g2g.G2GRecodeCampiToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Campo.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeCampiToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Campo In g2g.G2GRecodeCampiToUpdate
                    GiasContext.G2G_Recode_Campo.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAppezzamentiToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Appezzamenti In g2g.G2GRecodeAppezzamentiToDelete
                    GiasContext.G2G_Recode_Appezzamenti.Attach(recode)
                    GiasContext.G2G_Recode_Appezzamenti.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAppezzamentiToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Appezzamenti)
                For Each recode As G2G_Recode_Appezzamenti In g2g.G2GRecodeAppezzamentiToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Appezzamenti.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAppezzamentiToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Appezzamenti In g2g.G2GRecodeAppezzamentiToUpdate
                    GiasContext.G2G_Recode_Appezzamenti.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeImpiantiToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Impianti In g2g.G2GRecodeImpiantiToDelete
                    GiasContext.G2G_Recode_Impianti.Attach(recode)
                    GiasContext.G2G_Recode_Impianti.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeImpiantiToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Impianti)
                For Each recode As G2G_Recode_Impianti In g2g.G2GRecodeImpiantiToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Impianti.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeImpiantiToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Impianti In g2g.G2GRecodeImpiantiToUpdate
                    GiasContext.G2G_Recode_Impianti.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeDistinteToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Distinta In g2g.G2GRecodeDistinteToDelete
                    GiasContext.G2G_Recode_Distinta.Attach(recode)
                    GiasContext.G2G_Recode_Distinta.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeDistinteToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Distinta)
                For Each recode As G2G_Recode_Distinta In g2g.G2GRecodeDistinteToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Distinta.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeDistinteToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Distinta In g2g.G2GRecodeDistinteToUpdate
                    GiasContext.G2G_Recode_Distinta.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'Contatti
            If g2g.G2GRecodeContattiToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Contatti)
                For Each recode As G2G_Recode_Contatti In g2g.G2GRecodeContattiToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Contatti.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeContattiToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Contatti In g2g.G2GRecodeContattiToUpdate
                    GiasContext.G2G_Recode_Contatti.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeContattiToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Contatti In g2g.G2GRecodeContattiToDelete
                    GiasContext.G2G_Recode_Contatti.Attach(recode)
                    GiasContext.G2G_Recode_Contatti.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Indirizzi
            If g2g.G2GRecodeIndirizziToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Indirizzi)
                For Each recode As G2G_Recode_Indirizzi In g2g.G2GRecodeIndirizziToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Indirizzi.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeIndirizziToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Indirizzi In g2g.G2GRecodeIndirizziToUpdate
                    GiasContext.G2G_Recode_Indirizzi.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeIndirizziToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Indirizzi In g2g.G2GRecodeIndirizziToDelete
                    GiasContext.G2G_Recode_Indirizzi.Attach(recode)
                    GiasContext.G2G_Recode_Indirizzi.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Parco Macchine
            If g2g.G2GRecodeParcoMacchineToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Parco_Macchine)
                For Each recode As G2G_Recode_Parco_Macchine In g2g.G2GRecodeParcoMacchineToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Parco_Macchine.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeParcoMacchineToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Parco_Macchine In g2g.G2GRecodeParcoMacchineToUpdate
                    GiasContext.G2G_Recode_Parco_Macchine.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeParcoMacchineToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Parco_Macchine In g2g.G2GRecodeParcoMacchineToDelete
                    GiasContext.G2G_Recode_Parco_Macchine.Attach(recode)
                    GiasContext.G2G_Recode_Parco_Macchine.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Fabbricati
            If g2g.G2GRecodeFabbricatiToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Fabbricati)
                For Each recode As G2G_Recode_Fabbricati In g2g.G2GRecodeFabbricatiToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Fabbricati.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeFabbricatiToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Fabbricati In g2g.G2GRecodeFabbricatiToUpdate
                    GiasContext.G2G_Recode_Fabbricati.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeFabbricatiToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Fabbricati In g2g.G2GRecodeFabbricatiToDelete
                    GiasContext.G2G_Recode_Fabbricati.Attach(recode)
                    GiasContext.G2G_Recode_Fabbricati.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Pratiche
            If g2g.G2GRecodePraticheToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Pratiche)
                For Each recode As G2G_Recode_Pratiche In g2g.G2GRecodePraticheToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Pratiche.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePraticheToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Pratiche In g2g.G2GRecodePraticheToUpdate
                    GiasContext.G2G_Recode_Pratiche.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePraticheToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Pratiche In g2g.G2GRecodePraticheToDelete
                    GiasContext.G2G_Recode_Pratiche.Attach(recode)
                    GiasContext.G2G_Recode_Pratiche.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If


            'Pratiche, Stati
            If g2g.G2GRecodePraticheStatiToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Pratiche_Stati)
                For Each recode As G2G_Recode_Pratiche_Stati In g2g.G2GRecodePraticheStatiToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Pratiche_Stati.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePraticheStatiToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Pratiche_Stati In g2g.G2GRecodePraticheStatiToUpdate
                    GiasContext.G2G_Recode_Pratiche_Stati.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePraticheStatiToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Pratiche_Stati In g2g.G2GRecodePraticheStatiToDelete
                    GiasContext.G2G_Recode_Pratiche_Stati.Attach(recode)
                    GiasContext.G2G_Recode_Pratiche_Stati.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Programmazione
            If g2g.G2GRecodeProgrammazioneTestataToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Programmazione_Testata)
                For Each recode As G2G_Recode_Programmazione_Testata In g2g.G2GRecodeProgrammazioneTestataToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Programmazione_Testata.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeProgrammazioneTestataToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Programmazione_Testata In g2g.G2GRecodeProgrammazioneTestataToUpdate
                    GiasContext.G2G_Recode_Programmazione_Testata.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeProgrammazioneTestataToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Programmazione_Testata In g2g.G2GRecodeProgrammazioneTestataToDelete
                    GiasContext.G2G_Recode_Programmazione_Testata.Attach(recode)
                    GiasContext.G2G_Recode_Programmazione_Testata.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Programmazione, entita
            If g2g.G2GRecodeProgrammazioneEntitaToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Programmazione_Entita)
                For Each recode As G2G_Recode_Programmazione_Entita In g2g.G2GRecodeProgrammazioneEntitaToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Programmazione_Entita.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeProgrammazioneEntitaToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Programmazione_Entita In g2g.G2GRecodeProgrammazioneEntitaToUpdate
                    GiasContext.G2G_Recode_Programmazione_Entita.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeProgrammazioneEntitaToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Programmazione_Entita In g2g.G2GRecodeProgrammazioneEntitaToDelete
                    GiasContext.G2G_Recode_Programmazione_Entita.Attach(recode)
                    GiasContext.G2G_Recode_Programmazione_Entita.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Allegati
            If g2g.G2GRecodeAllegatiToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Allegati)
                For Each recode As G2G_Recode_Allegati In g2g.G2GRecodeAllegatiToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Allegati.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAllegatiToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Allegati In g2g.G2GRecodeAllegatiToUpdate
                    GiasContext.G2G_Recode_Allegati.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAllegatiToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Allegati In g2g.G2GRecodeAllegatiToDelete
                    GiasContext.G2G_Recode_Allegati.Attach(recode)
                    GiasContext.G2G_Recode_Allegati.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Allegati_Entita
            If g2g.G2GRecodeAllegati_EntitaToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Allegati_Entita)
                For Each recode As G2G_Recode_Allegati_Entita In g2g.G2GRecodeAllegati_EntitaToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Allegati_Entita.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAllegati_EntitaToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Allegati_Entita In g2g.G2GRecodeAllegati_EntitaToUpdate
                    GiasContext.G2G_Recode_Allegati_Entita.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAllegati_EntitaToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Allegati_Entita In g2g.G2GRecodeAllegati_EntitaToDelete
                    GiasContext.G2G_Recode_Allegati_Entita.Attach(recode)
                    GiasContext.G2G_Recode_Allegati_Entita.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'PUA
            If g2g.G2GRecodePuaToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Pua)
                For Each recode As G2G_Recode_Pua In g2g.G2GRecodePuaToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Pua.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePuaToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Pua In g2g.G2GRecodePuaToUpdate
                    GiasContext.G2G_Recode_Pua.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePuaToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Pua In g2g.G2GRecodePuaToDelete
                    GiasContext.G2G_Recode_Pua.Attach(recode)
                    GiasContext.G2G_Recode_Pua.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'PUA EFFLUENTE
            If g2g.G2GRecodePua_EffluenteToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_PUA_Effluente)
                For Each recode As G2G_Recode_PUA_Effluente In g2g.G2GRecodePua_EffluenteToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_PUA_Effluente.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePua_EffluenteToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_PUA_Effluente In g2g.G2GRecodePua_EffluenteToUpdate
                    GiasContext.G2G_Recode_PUA_Effluente.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePua_EffluenteToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_PUA_Effluente In g2g.G2GRecodePua_EffluenteToDelete
                    GiasContext.G2G_Recode_PUA_Effluente.Attach(recode)
                    GiasContext.G2G_Recode_PUA_Effluente.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'ParticelleCatastalixVincoliAgronomici
            If g2g.G2GRecodeParticelleCatastalixVincoliAgronomiciToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_ParticelleCatastalixVincoliAgronomici)
                For Each recode As G2G_Recode_ParticelleCatastalixVincoliAgronomici In g2g.G2GRecodeParticelleCatastalixVincoliAgronomiciToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeParticelleCatastalixVincoliAgronomiciToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_ParticelleCatastalixVincoliAgronomici In g2g.G2GRecodeParticelleCatastalixVincoliAgronomiciToUpdate
                    GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeParticelleCatastalixVincoliAgronomiciToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_ParticelleCatastalixVincoliAgronomici In g2g.G2GRecodeParticelleCatastalixVincoliAgronomiciToDelete
                    GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Attach(recode)
                    GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Analisi Certificati
            If g2g.G2GRecodeAnalisi_CertificatoToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Analisi_Certificato)
                For Each recode As G2G_Recode_Analisi_Certificato In g2g.G2GRecodeAnalisi_CertificatoToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Analisi_Certificato.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAnalisi_CertificatoToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Analisi_Certificato In g2g.G2GRecodeAnalisi_CertificatoToUpdate
                    GiasContext.G2G_Recode_Analisi_Certificato.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAnalisi_CertificatoToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Analisi_Certificato In g2g.G2GRecodeAnalisi_CertificatoToDelete
                    GiasContext.G2G_Recode_Analisi_Certificato.Attach(recode)
                    GiasContext.G2G_Recode_Analisi_Certificato.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Analisi Testate
            If g2g.G2GRecodeAnalisi_TestataToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Analisi_Testata)
                For Each recode As G2G_Recode_Analisi_Testata In g2g.G2GRecodeAnalisi_TestataToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Analisi_Testata.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAnalisi_TestataToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Analisi_Testata In g2g.G2GRecodeAnalisi_TestataToUpdate
                    GiasContext.G2G_Recode_Analisi_Testata.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAnalisi_TestataToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Analisi_Testata In g2g.G2GRecodeAnalisi_TestataToDelete
                    GiasContext.G2G_Recode_Analisi_Testata.Attach(recode)
                    GiasContext.G2G_Recode_Analisi_Testata.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Analisi Dettagli
            If g2g.G2GRecodeAnalisi_DettagliToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Analisi_Dettagli)
                For Each recode As G2G_Recode_Analisi_Dettagli In g2g.G2GRecodeAnalisi_DettagliToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Analisi_Dettagli.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAnalisi_DettagliToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Analisi_Dettagli In g2g.G2GRecodeAnalisi_DettagliToUpdate
                    GiasContext.G2G_Recode_Analisi_Dettagli.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAnalisi_DettagliToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Analisi_Dettagli In g2g.G2GRecodeAnalisi_DettagliToDelete
                    GiasContext.G2G_Recode_Analisi_Dettagli.Attach(recode)
                    GiasContext.G2G_Recode_Analisi_Dettagli.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Analisi Entita
            If g2g.G2GRecodeAnalisi_EntitaxTestataToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Analisi_EntitaxTestata In g2g.G2GRecodeAnalisi_EntitaxTestataToDelete
                    GiasContext.G2G_Recode_Analisi_EntitaxTestata.Attach(recode)
                    GiasContext.G2G_Recode_Analisi_EntitaxTestata.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAnalisi_EntitaxTestataToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Analisi_EntitaxTestata)
                For Each recode As G2G_Recode_Analisi_EntitaxTestata In g2g.G2GRecodeAnalisi_EntitaxTestataToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Analisi_EntitaxTestata.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAnalisi_EntitaxTestataToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Analisi_EntitaxTestata In g2g.G2GRecodeAnalisi_EntitaxTestataToUpdate
                    GiasContext.G2G_Recode_Analisi_EntitaxTestata.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'Analisi Campioni
            If g2g.G2GRecodeAnalisi_CampioniToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Analisi_Campioni In g2g.G2GRecodeAnalisi_CampioniToDelete
                    GiasContext.G2G_Recode_Analisi_Campioni.Attach(recode)
                    GiasContext.G2G_Recode_Analisi_Campioni.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAnalisi_CampioniToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Analisi_Campioni)
                For Each recode As G2G_Recode_Analisi_Campioni In g2g.G2GRecodeAnalisi_CampioniToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Analisi_Campioni.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAnalisi_CampioniToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Analisi_Campioni In g2g.G2GRecodeAnalisi_CampioniToUpdate
                    GiasContext.G2G_Recode_Analisi_Campioni.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'Piani di Concimazione Testate
            If g2g.G2GRecodePianoConcimazione_TestataToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_PianoConcimazione_Testata In g2g.G2GRecodePianoConcimazione_TestataToDelete
                    GiasContext.G2G_Recode_PianoConcimazione_Testata.Attach(recode)
                    GiasContext.G2G_Recode_PianoConcimazione_Testata.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePianoConcimazione_TestataToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_PianoConcimazione_Testata)
                For Each recode As G2G_Recode_PianoConcimazione_Testata In g2g.G2GRecodePianoConcimazione_TestataToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_PianoConcimazione_Testata.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePianoConcimazione_TestataToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_PianoConcimazione_Testata In g2g.G2GRecodePianoConcimazione_TestataToUpdate
                    GiasContext.G2G_Recode_PianoConcimazione_Testata.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'Piani di Concimazione Dettagli
            If g2g.G2GRecodePianoConcimazione_DettagliToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_PianoConcimazione_Dettagli In g2g.G2GRecodePianoConcimazione_DettagliToDelete
                    GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Attach(recode)
                    GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePianoConcimazione_DettagliToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_PianoConcimazione_Dettagli)
                For Each recode As G2G_Recode_PianoConcimazione_Dettagli In g2g.G2GRecodePianoConcimazione_DettagliToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_PianoConcimazione_Dettagli.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePianoConcimazione_DettagliToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_PianoConcimazione_Dettagli In g2g.G2GRecodePianoConcimazione_DettagliToUpdate
                    GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'Piani di Concimazione Entita
            If g2g.G2GRecodePianoConcimazione_EntitaxTestataToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_PianoConcimazione_EntitaxTestata In g2g.G2GRecodePianoConcimazione_EntitaxTestataToDelete
                    GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.Attach(recode)
                    GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePianoConcimazione_EntitaxTestataToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_PianoConcimazione_EntitaxTestata)
                For Each recode As G2G_Recode_PianoConcimazione_EntitaxTestata In g2g.G2GRecodePianoConcimazione_EntitaxTestataToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePianoConcimazione_EntitaxTestataToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_PianoConcimazione_EntitaxTestata In g2g.G2GRecodePianoConcimazione_EntitaxTestataToUpdate
                    GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'Piani di Concimazione Elaborazioni
            If g2g.G2GRecodePianoConcimazione_ElaborazioniToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_PianoConcimazione_Elaborazioni In g2g.G2GRecodePianoConcimazione_ElaborazioniToDelete
                    GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Attach(recode)
                    GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePianoConcimazione_ElaborazioniToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_PianoConcimazione_Elaborazioni)
                For Each recode As G2G_Recode_PianoConcimazione_Elaborazioni In g2g.G2GRecodePianoConcimazione_ElaborazioniToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePianoConcimazione_ElaborazioniToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_PianoConcimazione_Elaborazioni In g2g.G2GRecodePianoConcimazione_ElaborazioniToUpdate
                    GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'Ricette
            If g2g.G2GRecodeRicetteToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Ricette In g2g.G2GRecodeRicetteToDelete
                    GiasContext.G2G_Recode_Ricette.Attach(recode)
                    GiasContext.G2G_Recode_Ricette.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeRicetteToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Ricette)
                For Each recode As G2G_Recode_Ricette In g2g.G2GRecodeRicetteToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Ricette.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeRicetteToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Ricette In g2g.G2GRecodeRicetteToUpdate
                    GiasContext.G2G_Recode_Ricette.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'Ricette Operazioni
            If g2g.G2GRecodeRicette_OperazioniToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Ricette_Operazioni In g2g.G2GRecodeRicette_OperazioniToDelete
                    GiasContext.G2G_Recode_Ricette_Operazioni.Attach(recode)
                    GiasContext.G2G_Recode_Ricette_Operazioni.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeRicette_OperazioniToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Ricette_Operazioni)
                For Each recode As G2G_Recode_Ricette_Operazioni In g2g.G2GRecodeRicette_OperazioniToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Ricette_Operazioni.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeRicette_OperazioniToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Ricette_Operazioni In g2g.G2GRecodeRicette_OperazioniToUpdate
                    GiasContext.G2G_Recode_Ricette_Operazioni.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'Ricette Dettagli
            If g2g.G2GRecodeRicette_DettagliToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Ricette_Dettagli In g2g.G2GRecodeRicette_DettagliToDelete
                    GiasContext.G2G_Recode_Ricette_Dettagli.Attach(recode)
                    GiasContext.G2G_Recode_Ricette_Dettagli.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeRicette_DettagliToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Ricette_Dettagli)
                For Each recode As G2G_Recode_Ricette_Dettagli In g2g.G2GRecodeRicette_DettagliToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Ricette_Dettagli.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeRicette_DettagliToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Ricette_Dettagli In g2g.G2GRecodeRicette_DettagliToUpdate
                    GiasContext.G2G_Recode_Ricette_Dettagli.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'Ricette Dettaglio Tecnico
            If g2g.G2GRecodeRicette_Dettaglio_TecnicoToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Ricette_Dettaglio_Tecnico In g2g.G2GRecodeRicette_Dettaglio_TecnicoToDelete
                    GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Attach(recode)
                    GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeRicette_Dettaglio_TecnicoToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Ricette_Dettaglio_Tecnico)
                For Each recode As G2G_Recode_Ricette_Dettaglio_Tecnico In g2g.G2GRecodeRicette_Dettaglio_TecnicoToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeRicette_Dettaglio_TecnicoToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Ricette_Dettaglio_Tecnico In g2g.G2GRecodeRicette_Dettaglio_TecnicoToUpdate
                    GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'Ricette Destinazioni
            If g2g.G2GRecodeRicette_DestinazioniToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Ricette_Destinazioni In g2g.G2GRecodeRicette_DestinazioniToDelete
                    GiasContext.G2G_Recode_Ricette_Destinazioni.Attach(recode)
                    GiasContext.G2G_Recode_Ricette_Destinazioni.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeRicette_DestinazioniToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Ricette_Destinazioni)
                For Each recode As G2G_Recode_Ricette_Destinazioni In g2g.G2GRecodeRicette_DestinazioniToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Ricette_Destinazioni.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeRicette_DestinazioniToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Ricette_Destinazioni In g2g.G2GRecodeRicette_DestinazioniToUpdate
                    GiasContext.G2G_Recode_Ricette_Destinazioni.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'Materie Prime Campionature
            If g2g.G2GRecodeMateriePrimeCampionatureToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Materie_Prime_Campionature In g2g.G2GRecodeMateriePrimeCampionatureToDelete
                    GiasContext.G2G_Recode_Materie_Prime_Campionature.Attach(recode)
                    GiasContext.G2G_Recode_Materie_Prime_Campionature.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeMateriePrimeCampionatureToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Materie_Prime_Campionature)
                For Each recode As G2G_Recode_Materie_Prime_Campionature In g2g.G2GRecodeMateriePrimeCampionatureToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Materie_Prime_Campionature.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeMateriePrimeCampionatureToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Materie_Prime_Campionature In g2g.G2GRecodeMateriePrimeCampionatureToUpdate
                    GiasContext.G2G_Recode_Materie_Prime_Campionature.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'Materie Prime
            If g2g.G2GRecodeMateriePrimeToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_MateriePrime In g2g.G2GRecodeMateriePrimeToDelete
                    'GiasContext.AttachTo("G2G_Recode_Materie_Prime", recode)
                    'GiasContext.DeleteObject(recode)
                    GiasContext.G2G_Recode_MateriePrime.Attach(recode)
                    GiasContext.G2G_Recode_MateriePrime.Remove(recode)

                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeMateriePrimeToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_MateriePrime)
                For Each recode As G2G_Recode_MateriePrime In g2g.G2GRecodeMateriePrimeToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_MateriePrime.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeMateriePrimeToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_MateriePrime In g2g.G2GRecodeMateriePrimeToUpdate
                    GiasContext.G2G_Recode_MateriePrime.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            'PUA_LetamazioniPrecedenti
            If g2g.G2GRecodePUA_LetamazioniPrecedentiToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_PUA_LetamazioniPrecedenti)
                For Each recode As G2G_Recode_PUA_LetamazioniPrecedenti In g2g.G2GRecodePUA_LetamazioniPrecedentiToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePUA_LetamazioniPrecedentiToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_PUA_LetamazioniPrecedenti In g2g.G2GRecodePUA_LetamazioniPrecedentiToUpdate
                    GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePUA_LetamazioniPrecedentiToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_PUA_LetamazioniPrecedenti In g2g.G2GRecodePUA_LetamazioniPrecedentiToDelete
                    GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Attach(recode)
                    GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'PUA_PeriodoDivieto
            If g2g.G2GRecodePUA_Effluente_PeriodoDivietoToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_PUA_PeriodoDivieto)
                For Each recode As G2G_Recode_PUA_PeriodoDivieto In g2g.G2GRecodePUA_Effluente_PeriodoDivietoToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_PUA_PeriodoDivieto.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePUA_Effluente_PeriodoDivietoToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_PUA_PeriodoDivieto In g2g.G2GRecodePUA_Effluente_PeriodoDivietoToUpdate
                    GiasContext.G2G_Recode_PUA_PeriodoDivieto.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodePUA_Effluente_PeriodoDivietoToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_PUA_PeriodoDivieto In g2g.G2GRecodePUA_Effluente_PeriodoDivietoToDelete
                    GiasContext.G2G_Recode_PUA_PeriodoDivieto.Attach(recode)
                    GiasContext.G2G_Recode_PUA_PeriodoDivieto.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            'Anagrafe_VincoliAgronomici
            If g2g.G2GRecodeAnagrafe_VincoliAgronomiciToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Anagrafe_VincoliAgronomici)
                For Each recode As G2G_Recode_Anagrafe_VincoliAgronomici In g2g.G2GRecodeAnagrafe_VincoliAgronomiciToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAnagrafe_VincoliAgronomiciToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Anagrafe_VincoliAgronomici In g2g.G2GRecodeAnagrafe_VincoliAgronomiciToUpdate
                    GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAnagrafe_VincoliAgronomiciToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Anagrafe_VincoliAgronomici In g2g.G2GRecodeAnagrafe_VincoliAgronomiciToDelete
                    GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Attach(recode)
                    GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If



            'Attivita
            If g2g.G2GRecodeAttivitaToInsert IsNot Nothing Then
                Dim list As New List(Of G2G_Recode_Attivita)
                For Each recode As G2G_Recode_Attivita In g2g.G2GRecodeAttivitaToInsert
                    list.Add(recode)
                Next
                GiasContext.G2G_Recode_Attivita.AddRange(list)
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAttivitaToUpdate IsNot Nothing Then
                For Each recode As G2G_Recode_Attivita In g2g.G2GRecodeAttivitaToUpdate
                    GiasContext.G2G_Recode_Attivita.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified
                Next
                GiasContext.SaveChanges()
            End If

            If g2g.G2GRecodeAttivitaToDelete IsNot Nothing Then
                For Each recode As G2G_Recode_Attivita In g2g.G2GRecodeAttivitaToDelete
                    GiasContext.G2G_Recode_Attivita.Attach(recode)
                    GiasContext.G2G_Recode_Attivita.Remove(recode)
                Next
                GiasContext.SaveChanges()
            End If

            ' COMMIT Effettivo
            'scope.Complete()
            'scope.Dispose()

        Catch ex As Exception

            messaggioErrore = ex.Message
            'scope.Dispose()
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        Finally

            GiasContext.Dispose()

        End Try

        'End Using

        Return messaggioErrore

    End Function

    Public Shared Function Scrivi_G2G_Recode_Agenda(ByVal CodiciRimappati As String, ByVal TipoOperazioneBD As enum_TipoOperazioneDB, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim nomeRoutine As String = "Scrivi_G2G_Recode_Agenda()"
        Dim messaggioErrore As String = ""

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try

            Dim vCodiciRimappati As String() = CodiciRimappati.Split("|")

            For Each c In vCodiciRimappati

                Dim vCod As String() = c.Split(":")
                Dim Valori As String() = vCod(1).Split("*")

                If vCod(0) <> "Agenda" AndAlso TipoOperazioneBD = enum_TipoOperazioneDB.Modifica Then
                    Return ""
                End If

                Dim vvVecchi As String() = Valori(0).Split(",")
                Dim vvNuovi As String() = Valori(1).Split(",")

                Select Case vCod(0)

                    Case "Agenda"

                        If TipoOperazioneBD = enum_TipoOperazioneDB.Scrittura Then

                            'nuovo recode agenda
                            Dim recode =
                                New G2G_Recode_Agenda With {
                                    .From_PivaSuperUser = vvVecchi(0),
                                    .To_PivaSuperUser = vvNuovi(0),
                                    .FromPiva = vvVecchi(1),
                                    .ToPiva = vvNuovi(1),
                                    .FromSa_cod = vvVecchi(2),
                                    .ToSa_cod = vvNuovi(2),
                                    .FromId_Agenda = vvVecchi(3),
                                    .ToId_Agenda = vvNuovi(3),
                                    .RiferimentiElaborati = 0,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = 0,
                                    .datainvio = Now()
                                }
                            GiasContext.G2G_Recode_Agenda.Add(recode)

                        Else

                            Cancella_G2G_Recode_Agenda(vvNuovi(1), vvNuovi(2), vvNuovi(3), objParametri)

                        End If

                    Case "Movimento"

                        If TipoOperazioneBD = enum_TipoOperazioneDB.Scrittura Then

                            'nuovo recode movimento
                            Dim recode =
                                New G2G_Recode_Movimenti With {
                                    .From_PivaSuperUser = vvVecchi(0),
                                    .To_PivaSuperUser = vvNuovi(0),
                                    .FromPiva = vvVecchi(1),
                                    .ToPiva = vvNuovi(1),
                                    .FromSa_cod = vvVecchi(2),
                                    .ToSa_cod = vvNuovi(2),
                                    .FromId_Agenda = vvVecchi(3),
                                    .ToId_Agenda = vvNuovi(3),
                                    .FromId_mov = vvVecchi(4),
                                    .ToId_mov = vvNuovi(4),
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = 0,
                                    .datainvio = Now()
                                }
                            GiasContext.G2G_Recode_Movimenti.Add(recode)

                        End If

                    Case "Movimento_Dettaglio"

                        If TipoOperazioneBD = enum_TipoOperazioneDB.Scrittura Then

                            'nuovo recode movimento dettaglio
                            Dim recode =
                                New G2G_Recode_Mov_Dettagli With {
                                    .From_PivaSuperUser = vvVecchi(0),
                                    .To_PivaSuperUser = vvNuovi(0),
                                    .FromPiva = vvVecchi(1),
                                    .ToPiva = vvNuovi(1),
                                    .FromSa_cod = vvVecchi(2),
                                    .ToSa_cod = vvNuovi(2),
                                    .FromId_Agenda = vvVecchi(3),
                                    .ToId_Agenda = vvNuovi(3),
                                    .FromId_mov = vvVecchi(4),
                                    .ToId_mov = vvNuovi(4),
                                    .FromId_mov_det = vvVecchi(5),
                                    .ToId_mov_det = vvNuovi(5),
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = 0,
                                    .datainvio = Now()
                                }
                            GiasContext.G2G_Recode_Mov_Dettagli.Add(recode)

                        End If

                    Case "Raccoglitore"

                        If TipoOperazioneBD = enum_TipoOperazioneDB.Scrittura Then

                            Dim From_PivaSuperUser As String = vvVecchi(0)
                            Dim From_Piva As String = vvVecchi(1)
                            Dim From_Raccoglitore_Cod As Integer = vvVecchi(2)
                            Dim To_PivaSuperUser As String = vvNuovi(0)
                            Dim To_Piva As String = vvNuovi(1)
                            Dim To_Raccoglitore_Cod As Integer = vvNuovi(2)

                            Dim recode = (From x In GiasContext.G2G_Recode_Raccoglitore Where x.From_PivaSuperUser = From_PivaSuperUser AndAlso x.From_Piva = From_Piva AndAlso x.From_Raccoglitore_Cod = From_Raccoglitore_Cod AndAlso
                                               x.To_PivaSuperUser = To_PivaSuperUser AndAlso x.To_Piva = To_Piva AndAlso x.To_Raccoglitore_Cod = To_Raccoglitore_Cod).FirstOrDefault()

                            If recode Is Nothing Then

                                'nuovo recode raccoglitore
                                recode =
                                    New G2G_Recode_Raccoglitore With {
                                        .From_PivaSuperUser = vvVecchi(0),
                                        .To_PivaSuperUser = vvNuovi(0),
                                        .From_Piva = vvVecchi(1),
                                        .To_Piva = vvNuovi(1),
                                        .From_Raccoglitore_Cod = vvVecchi(2),
                                        .To_Raccoglitore_Cod = vvNuovi(2),
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = 0,
                                        .datainvio = Now()
                                    }
                                GiasContext.G2G_Recode_Raccoglitore.Add(recode)

                            End If


                        End If

                End Select

            Next

            GiasContext.SaveChanges()

        Catch ex As Exception

            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        Finally

            GiasContext.Dispose()

        End Try

        Return messaggioErrore

    End Function

    Public Shared Function Scrivi_G2G_Recode_AgendaReverse(ByVal CodiciRimappati As String, ByVal TipoOperazioneBD As enum_TipoOperazioneDB, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim nomeRoutine As String = "Scrivi_G2G_Recode_AgendaReverse()"
        Dim messaggioErrore As String = ""

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try

            Dim vCodiciRimappati As String() = CodiciRimappati.Split("|")

            For Each c In vCodiciRimappati

                Dim vCod As String() = c.Split(":")
                Dim Valori As String() = vCod(1).Split("*")

                If vCod(0) <> "Agenda" AndAlso TipoOperazioneBD = enum_TipoOperazioneDB.Modifica Then
                    Return ""
                End If

                Dim vvVecchi As String() = Valori(0).Split(",")
                Dim vvNuovi As String() = Valori(1).Split(",")

                Select Case vCod(0)

                    Case "Agenda"

                        If TipoOperazioneBD = enum_TipoOperazioneDB.Scrittura Then

                            'nuovo recode agenda
                            Dim recode =
                                New G2G_Recode_Agenda With {
                                    .From_PivaSuperUser = vvNuovi(0),
                                    .To_PivaSuperUser = vvVecchi(0),
                                    .FromPiva = vvNuovi(1),
                                    .ToPiva = vvVecchi(1),
                                    .FromSa_cod = vvNuovi(2),
                                    .ToSa_cod = vvVecchi(2),
                                    .FromId_Agenda = vvNuovi(3),
                                    .ToId_Agenda = vvVecchi(3),
                                    .RiferimentiElaborati = 0,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = 0,
                                    .datainvio = Now()
                                }
                            GiasContext.G2G_Recode_Agenda.Add(recode)

                        Else

                            Cancella_G2G_Recode_Agenda(vvNuovi(1), vvNuovi(2), vvNuovi(3), objParametri)

                        End If

                    Case "Movimento"

                        If TipoOperazioneBD = enum_TipoOperazioneDB.Scrittura Then

                            'nuovo recode movimento
                            Dim recode =
                                New G2G_Recode_Movimenti With {
                                    .From_PivaSuperUser = vvNuovi(0),
                                    .To_PivaSuperUser = vvVecchi(0),
                                    .FromPiva = vvNuovi(1),
                                    .ToPiva = vvVecchi(1),
                                    .FromSa_cod = vvNuovi(2),
                                    .ToSa_cod = vvVecchi(2),
                                    .FromId_Agenda = vvNuovi(3),
                                    .ToId_Agenda = vvVecchi(3),
                                    .FromId_mov = vvNuovi(4),
                                    .ToId_mov = vvVecchi(4),
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = 0,
                                    .datainvio = Now()
                                }
                            GiasContext.G2G_Recode_Movimenti.Add(recode)

                        End If

                    Case "Movimento_Dettaglio"

                        If TipoOperazioneBD = enum_TipoOperazioneDB.Scrittura Then

                            'nuovo recode movimento dettaglio
                            Dim recode =
                                New G2G_Recode_Mov_Dettagli With {
                                    .From_PivaSuperUser = vvNuovi(0),
                                    .To_PivaSuperUser = vvVecchi(0),
                                    .FromPiva = vvNuovi(1),
                                    .ToPiva = vvVecchi(1),
                                    .FromSa_cod = vvNuovi(2),
                                    .ToSa_cod = vvVecchi(2),
                                    .FromId_Agenda = vvNuovi(3),
                                    .ToId_Agenda = vvVecchi(3),
                                    .FromId_mov = vvNuovi(4),
                                    .ToId_mov = vvVecchi(4),
                                    .FromId_mov_det = vvNuovi(5),
                                    .ToId_mov_det = vvVecchi(5),
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = 0,
                                    .datainvio = Now()
                                }
                            GiasContext.G2G_Recode_Mov_Dettagli.Add(recode)

                        End If

                    Case "Raccoglitore"

                        If TipoOperazioneBD = enum_TipoOperazioneDB.Scrittura Then

                            Dim From_PivaSuperUser As String = vvVecchi(0)
                            Dim From_Piva As String = vvVecchi(1)
                            Dim From_Raccoglitore_Cod As Integer = vvVecchi(2)
                            Dim To_PivaSuperUser As String = vvNuovi(0)
                            Dim To_Piva As String = vvNuovi(1)
                            Dim To_Raccoglitore_Cod As Integer = vvNuovi(2)

                            Dim recode = (From x In GiasContext.G2G_Recode_Raccoglitore Where x.From_PivaSuperUser = From_PivaSuperUser AndAlso x.From_Piva = From_Piva AndAlso x.From_Raccoglitore_Cod = From_Raccoglitore_Cod AndAlso
                                               x.To_PivaSuperUser = To_PivaSuperUser AndAlso x.To_Piva = To_Piva AndAlso x.To_Raccoglitore_Cod = To_Raccoglitore_Cod).FirstOrDefault()

                            If recode Is Nothing Then

                                'nuovo recode raccoglitore
                                recode =
                                    New G2G_Recode_Raccoglitore With {
                                        .From_PivaSuperUser = vvNuovi(0),
                                        .To_PivaSuperUser = vvVecchi(0),
                                        .From_Piva = vvNuovi(1),
                                        .To_Piva = vvVecchi(1),
                                        .From_Raccoglitore_Cod = vvNuovi(2),
                                        .To_Raccoglitore_Cod = vvVecchi(2),
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = 0,
                                        .datainvio = Now()
                                    }
                                GiasContext.G2G_Recode_Raccoglitore.Add(recode)

                            End If


                        End If

                End Select

            Next

            GiasContext.SaveChanges()

        Catch ex As Exception

            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        Finally

            GiasContext.Dispose()

        End Try

        Return messaggioErrore

    End Function

    Public Shared Function Cancella_G2G_Recode_Agenda(ByVal To_Piva As String, ByVal To_Sa_Cod As Integer, ByVal To_Id_Agenda As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim nomeRoutine As String = "Cancella_G2G_Recode_Agenda()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try

            Dim recodeAgenda As G2G_Recode_Agenda =
                (From g In GiasContext.G2G_Recode_Agenda
                 Where g.ToPiva = To_Piva AndAlso
                       g.ToSa_cod = To_Sa_Cod AndAlso
                       g.ToId_Agenda = To_Id_Agenda).FirstOrDefault

            If recodeAgenda IsNot Nothing Then

                GiasContext.G2G_Recode_Agenda.Remove(recodeAgenda)

                Dim recodeMovimenti As List(Of G2G_Recode_Movimenti) = (
                    From m In GiasContext.G2G_Recode_Movimenti
                    Where m.FromPiva = recodeAgenda.FromPiva AndAlso
                          m.ToPiva = recodeAgenda.ToPiva AndAlso
                          m.FromId_Agenda = recodeAgenda.FromId_Agenda AndAlso
                          m.ToId_Agenda = recodeAgenda.ToId_Agenda).ToList()

                For Each recode In recodeMovimenti
                    GiasContext.G2G_Recode_Movimenti.Remove(recode)
                Next

                Dim recodeMovimentiDettagli As List(Of G2G_Recode_Mov_Dettagli) = (
                    From m In GiasContext.G2G_Recode_Mov_Dettagli
                    Where m.FromPiva = recodeAgenda.FromPiva AndAlso
                          m.ToPiva = recodeAgenda.ToPiva AndAlso
                          m.FromId_Agenda = recodeAgenda.FromId_Agenda AndAlso
                          m.ToId_Agenda = recodeAgenda.ToId_Agenda).ToList()

                For Each recode As G2G_Recode_Mov_Dettagli In recodeMovimentiDettagli
                    GiasContext.G2G_Recode_Mov_Dettagli.Remove(recode)
                Next

                Dim recodeMovimentiDettaglioTecnico As List(Of G2G_Recode_Mov_DettaglioTecnico) = (
                    From m In GiasContext.G2G_Recode_Mov_DettaglioTecnico
                    Where m.FromPiva = recodeAgenda.FromPiva AndAlso
                          m.ToPiva = recodeAgenda.ToPiva AndAlso
                          m.FromId_Agenda = recodeAgenda.FromId_Agenda AndAlso
                          m.ToId_Agenda = recodeAgenda.ToId_Agenda).ToList()

                For Each recode As G2G_Recode_Mov_DettaglioTecnico In recodeMovimentiDettaglioTecnico
                    GiasContext.G2G_Recode_Mov_DettaglioTecnico.Remove(recode)
                Next

            End If

            GiasContext.SaveChanges()

        Catch ex As Exception

            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        Finally

            GiasContext.Dispose()

        End Try

        Return messaggioErrore

    End Function

    Public Shared Function Aggiorna_Riferimenti_Agenda(ByVal Piva_SuperUser As String, ByVal Piva As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim nomeRoutine As String = "Aggiorna_Riferimenti_Agenda()"
        Dim messaggioErrore As String = ""

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim objScrivi As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
        Dim idAgendaAggiornati As New List(Of Integer)

        Try

            Dim recodeMovimenti = (
                    From mm In GiasContext.G2G_Recode_Movimenti
                    Join ii In GiasContext.G2G_Recode_Agenda On ii.FromId_Agenda Equals mm.FromId_Agenda And ii.FromPiva Equals mm.FromPiva
                    Where ii.RiferimentiElaborati = 0 AndAlso
                          ii.FromPiva = Piva AndAlso
                          ii.From_PivaSuperUser = Piva_SuperUser
                    Select mm
            ).ToList

            Dim recodeMovDettagli = (
                    From mm In GiasContext.G2G_Recode_Mov_Dettagli
                    Join ii In GiasContext.G2G_Recode_Agenda On ii.FromId_Agenda Equals mm.FromId_Agenda And ii.FromPiva Equals mm.FromPiva
                    Where ii.RiferimentiElaborati = 0 AndAlso
                          ii.FromPiva = Piva AndAlso
                          ii.From_PivaSuperUser = Piva_SuperUser
                    Select mm
            ).ToList

            For Each recode As G2G_Recode_Movimenti In recodeMovimenti

                Dim righeAggiornate As Integer

                objScrivi.G2G_ModificaCodici(
                    recode.FromPiva,
                    0,
                    recode.FromId_Agenda,
                    recode.FromId_mov,
                    -1,
                    recode.ToPiva,
                    0,
                    recode.ToId_Agenda,
                    recode.ToId_mov,
                    -1,
                    objParametri,
                    righeAggiornate
                )

                'If righeAggiornate > 0 Then
                '    idAgendaAggiornati.Add(recode.FromId_Agenda)
                'End If

                objScrivi.G2G_ModificaCodici(
                    recode.FromPiva,
                    recode.FromSa_cod,
                    recode.FromId_Agenda,
                    recode.FromId_mov,
                    -1,
                    recode.ToPiva,
                    recode.ToSa_cod,
                    recode.ToId_Agenda,
                    recode.ToId_mov,
                    -1,
                    objParametri,
                    righeAggiornate
                )

                'If righeAggiornate > 0 Then
                '    idAgendaAggiornati.Add(recode.FromId_Agenda)
                'End If

                If Not idAgendaAggiornati.Contains(recode.FromId_Agenda) Then
                    idAgendaAggiornati.Add(recode.FromId_Agenda)
                End If

            Next

            For Each recode As G2G_Recode_Mov_Dettagli In recodeMovDettagli

                Dim righeAggiornate As Integer

                objScrivi.G2G_ModificaCodici(
                    recode.FromPiva,
                    0,
                    recode.FromId_Agenda,
                    recode.FromId_mov,
                    recode.FromId_mov_det,
                    recode.ToPiva,
                    0,
                    recode.ToId_Agenda,
                    recode.ToId_mov,
                    recode.ToId_mov_det,
                    objParametri,
                    righeAggiornate
                )

                'If righeAggiornate > 0 Then
                '    idAgendaAggiornati.Add(recode.FromId_Agenda)
                'End If

                objScrivi.G2G_ModificaCodici(
                    recode.FromPiva,
                    recode.FromSa_cod,
                    recode.FromId_Agenda,
                    recode.FromId_mov,
                    recode.FromId_mov_det,
                    recode.ToPiva,
                    recode.ToSa_cod,
                    recode.ToId_Agenda,
                    recode.ToId_mov,
                    recode.ToId_mov_det,
                    objParametri,
                    righeAggiornate
                )

                'If righeAggiornate > 0 Then
                '    idAgendaAggiornati.Add(recode.FromId_Agenda)
                'End If

                If Not idAgendaAggiornati.Contains(recode.FromId_Agenda) Then
                    idAgendaAggiornati.Add(recode.FromId_Agenda)
                End If

            Next

            Dim agendaModificata =
                From a In GiasContext.G2G_Recode_Agenda
                Where idAgendaAggiornati.Contains(a.FromId_Agenda)
                Select a

            For Each a In agendaModificata
                a.RiferimentiElaborati = 1
            Next

            GiasContext.SaveChanges()

        Catch ex As Exception

            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        Finally

            GiasContext.Dispose()

        End Try

        Return messaggioErrore

    End Function

    Public Shared Function Aggiorna_Riferimenti_AgendaReverse(ByVal Piva_SuperUser As String, ByVal Piva As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim nomeRoutine As String = "Aggiorna_Riferimenti_AgendaReverse()"
        Dim messaggioErrore As String = ""

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim objScrivi As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
        Dim idAgendaAggiornati As New List(Of Integer)

        Try

            Dim recodeMovimenti = (
                    From mm In GiasContext.G2G_Recode_Movimenti
                    Join ii In GiasContext.G2G_Recode_Agenda On ii.ToId_Agenda Equals mm.ToId_Agenda And ii.FromPiva Equals mm.FromPiva
                    Where ii.RiferimentiElaborati = 0 AndAlso
                          ii.FromPiva = Piva AndAlso
                          ii.To_PivaSuperUser = Piva_SuperUser
                    Select mm
            ).ToList

            Dim recodeMovDettagli = (
                    From mm In GiasContext.G2G_Recode_Mov_Dettagli
                    Join ii In GiasContext.G2G_Recode_Agenda On ii.ToId_Agenda Equals mm.ToId_Agenda And ii.FromPiva Equals mm.FromPiva
                    Where ii.RiferimentiElaborati = 0 AndAlso
                          ii.FromPiva = Piva AndAlso
                          ii.To_PivaSuperUser = Piva_SuperUser
                    Select mm
            ).ToList

            For Each recode As G2G_Recode_Movimenti In recodeMovimenti

                Dim righeAggiornate As Integer

                objScrivi.G2G_ModificaCodici(
                    recode.ToPiva,
                    0,
                    recode.ToId_Agenda,
                    recode.ToId_mov,
                    -1,
                    recode.FromPiva,
                    0,
                    recode.FromId_Agenda,
                    recode.FromId_mov,
                    -1,
                    objParametri,
                    righeAggiornate
                )

                'If righeAggiornate > 0 Then
                '    idAgendaAggiornati.Add(recode.FromId_Agenda)
                'End If

                objScrivi.G2G_ModificaCodici(
                    recode.ToPiva,
                    recode.ToSa_cod,
                    recode.ToId_Agenda,
                    recode.ToId_mov,
                    -1,
                    recode.FromPiva,
                    recode.FromSa_cod,
                    recode.FromId_Agenda,
                    recode.FromId_mov,
                    -1,
                    objParametri,
                    righeAggiornate
                )

                'If righeAggiornate > 0 Then
                '    idAgendaAggiornati.Add(recode.FromId_Agenda)
                'End If

                If Not idAgendaAggiornati.Contains(recode.FromId_Agenda) Then
                    idAgendaAggiornati.Add(recode.FromId_Agenda)
                End If

            Next

            For Each recode As G2G_Recode_Mov_Dettagli In recodeMovDettagli

                Dim righeAggiornate As Integer

                objScrivi.G2G_ModificaCodici(
                    recode.ToPiva,
                    0,
                    recode.ToId_Agenda,
                    recode.ToId_mov,
                    recode.ToId_mov_det,
                    recode.FromPiva,
                    0,
                    recode.FromId_Agenda,
                    recode.FromId_mov,
                    recode.FromId_mov_det,
                    objParametri,
                    righeAggiornate
                )

                'If righeAggiornate > 0 Then
                '    idAgendaAggiornati.Add(recode.FromId_Agenda)
                'End If

                objScrivi.G2G_ModificaCodici(
                    recode.ToPiva,
                    recode.ToSa_cod,
                    recode.ToId_Agenda,
                    recode.ToId_mov,
                    recode.ToId_mov_det,
                    recode.FromPiva,
                    recode.FromSa_cod,
                    recode.FromId_Agenda,
                    recode.FromId_mov,
                    recode.FromId_mov_det,
                    objParametri,
                    righeAggiornate
                )

                'If righeAggiornate > 0 Then
                '    idAgendaAggiornati.Add(recode.FromId_Agenda)
                'End If

                If Not idAgendaAggiornati.Contains(recode.FromId_Agenda) Then
                    idAgendaAggiornati.Add(recode.FromId_Agenda)
                End If

            Next

            Dim agendaModificata =
                From a In GiasContext.G2G_Recode_Agenda
                Where idAgendaAggiornati.Contains(a.FromId_Agenda)
                Select a

            For Each a In agendaModificata
                a.RiferimentiElaborati = 1
            Next

            GiasContext.SaveChanges()

        Catch ex As Exception

            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        Finally

            GiasContext.Dispose()

        End Try

        Return messaggioErrore

    End Function

End Class




