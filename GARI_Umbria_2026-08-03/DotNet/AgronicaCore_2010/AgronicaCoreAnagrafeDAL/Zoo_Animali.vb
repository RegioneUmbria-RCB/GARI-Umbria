Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.ListExtensions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModelsSTD
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreDataProvider.My.Resources
Imports Newtonsoft.Json.Linq

Public Class Zoo_Animali
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub Scrivi(ByRef zooAnimale As AgronicaCoreEntityFramework_POCO.Zoo_Animali,
                      ByRef GiasContext As Gias_DeveloperServer_Entities,
                      ByRef objParametriServer As AgronicaCoreParametri,
                      Optional ByVal servizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_Animali.Scrivi()"
        Dim messaggioErrore As String = ""

        Try

            Valorizza(zooAnimale, objParametriServer)

            zooAnimale.datainvio = DateTime.Now
            zooAnimale.Data_Creazione = DateTime.Now
            zooAnimale.Data_Modifica = DateTime.Now

            zooAnimale.Username_Creazione = objParametriServer.UsernameOperazione
            zooAnimale.Username_Modifica = objParametriServer.UsernameOperazione

            If zooAnimale.Razza_Madre Is Nothing Then
                zooAnimale.Razza_Madre = 0
            End If
            If zooAnimale.Razza_Padre Is Nothing Then
                zooAnimale.Razza_Padre = 0
            End If
            If zooAnimale.Nome Is Nothing Then
                zooAnimale.Nome = ""
            End If
            If zooAnimale.Collare Is Nothing Then
                zooAnimale.Collare = ""
            End If
            If zooAnimale.NOME_AIA Is Nothing Then
                zooAnimale.NOME_AIA = ""
            End If
            If zooAnimale.PROV_NASCITA Is Nothing Then
                zooAnimale.PROV_NASCITA = ""
            End If
            If zooAnimale.STATO_NASCITA Is Nothing Then
                zooAnimale.STATO_NASCITA = ""
            End If
            If zooAnimale.MAT_PADRE Is Nothing Then
                zooAnimale.MAT_PADRE = ""
            End If
            If zooAnimale.MATRICOLA_AIA Is Nothing Then
                zooAnimale.MATRICOLA_AIA = ""
            End If
            If zooAnimale.AUA_AZI_NASCITA Is Nothing Then
                zooAnimale.AUA_AZI_NASCITA = ""
            End If
            If zooAnimale.AUSL_AZI_NASCITA Is Nothing Then
                zooAnimale.AUSL_AZI_NASCITA = ""
            End If
            If zooAnimale.CF_PROPRIETARIO Is Nothing Then
                zooAnimale.CF_PROPRIETARIO = ""
            End If
            If zooAnimale.CF_DETENTORE Is Nothing Then
                zooAnimale.CF_DETENTORE = ""
            End If
            If zooAnimale.CF_Fornitore Is Nothing Then
                zooAnimale.CF_Fornitore = ""
            End If
            If zooAnimale.Lotto_Fornitore Is Nothing Then
                zooAnimale.Lotto_Fornitore = ""
            End If
            If zooAnimale.Stalla_Svezzamento Is Nothing Then
                zooAnimale.Stalla_Svezzamento = ""
            End If
            If zooAnimale.inviato Is Nothing Then
                zooAnimale.inviato = 0
            End If

            If zooAnimale.Id_Capo_BDN Is Nothing Then
                zooAnimale.Id_Capo_BDN = 0
            End If

            If zooAnimale.Validato Is Nothing Then
                zooAnimale.Validato = 0
            End If


            GiasContext.Zoo_Animali.Add(zooAnimale)
            GiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.ZooAnimali,
                                                        zooAnimale.PIVA, CStr(zooAnimale.Cod_Progetto),
                                                        Nothing, Nothing,
                                                        Nothing, Nothing,
                                                        enum_TipoOperazioneDB.Scrittura,
                                                        objParametriServer, servizio)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

        Catch ex As Exception
            messaggioErrore = ex.Message
            If ex.InnerException IsNot Nothing AndAlso ex.InnerException.Message IsNot Nothing Then
                messaggioErrore &= "Inner Exception: " & ex.InnerException.Message
            End If
            Dim exStrJSON As String = ""
            Try
                exStrJSON = JsonConvert.SerializeObject(ex)
            Catch ex1 As Exception
                Scrivi_LOG(objParametriServer, nomeRoutine, "Errore in JSON Serialize Object")
            End Try
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Scrivi_LOG(objParametriServer, nomeRoutine, "Exception Serializzata: " & exStrJSON)
            messaggioErrore &= " Exception Serializzata: " & exStrJSON
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Modifica(ByRef zooAnimale As AgronicaCoreEntityFramework_POCO.Zoo_Animali,
                        ByRef GiasContext As Gias_DeveloperServer_Entities,
                        ByRef objParametriServer As AgronicaCoreParametri,
                        Optional ByVal servizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_Animali.Modifica()"
        Dim messaggioErrore As String = ""

        Try

            Valorizza(zooAnimale, objParametriServer)

            zooAnimale.Data_Modifica = DateTime.Now
            zooAnimale.Username_Modifica = objParametriServer.UsernameOperazione

            If zooAnimale.Razza_Madre Is Nothing Then
                zooAnimale.Razza_Madre = 0
            End If
            If zooAnimale.Razza_Padre Is Nothing Then
                zooAnimale.Razza_Padre = 0
            End If
            If zooAnimale.Nome Is Nothing Then
                zooAnimale.Nome = ""
            End If
            If zooAnimale.Collare Is Nothing Then
                zooAnimale.Collare = ""
            End If
            If zooAnimale.NOME_AIA Is Nothing Then
                zooAnimale.NOME_AIA = ""
            End If
            If zooAnimale.PROV_NASCITA Is Nothing Then
                zooAnimale.PROV_NASCITA = ""
            End If
            If zooAnimale.STATO_NASCITA Is Nothing Then
                zooAnimale.STATO_NASCITA = ""
            End If
            If zooAnimale.MAT_PADRE Is Nothing Then
                zooAnimale.MAT_PADRE = ""
            End If
            If zooAnimale.MATRICOLA_AIA Is Nothing Then
                zooAnimale.MATRICOLA_AIA = ""
            End If
            If zooAnimale.AUA_AZI_NASCITA Is Nothing Then
                zooAnimale.AUA_AZI_NASCITA = ""
            End If
            If zooAnimale.AUSL_AZI_NASCITA Is Nothing Then
                zooAnimale.AUSL_AZI_NASCITA = ""
            End If
            If zooAnimale.CF_PROPRIETARIO Is Nothing Then
                zooAnimale.CF_PROPRIETARIO = ""
            End If
            If zooAnimale.CF_DETENTORE Is Nothing Then
                zooAnimale.CF_DETENTORE = ""
            End If
            If zooAnimale.CF_Fornitore Is Nothing Then
                zooAnimale.CF_Fornitore = ""
            End If
            If zooAnimale.Lotto_Fornitore Is Nothing Then
                zooAnimale.Lotto_Fornitore = ""
            End If
            If zooAnimale.Stalla_Svezzamento Is Nothing Then
                zooAnimale.Stalla_Svezzamento = ""
            End If
            If zooAnimale.Id_Capo_BDN Is Nothing Then
                zooAnimale.Id_Capo_BDN = 0
            End If
            If zooAnimale.inviato Is Nothing Then
                zooAnimale.inviato = 0
            End If


            GiasContext.Entry(zooAnimale).State = EntityState.Modified
            GiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.ZooAnimali,
                                                        zooAnimale.PIVA, CStr(zooAnimale.Cod_Progetto),
                                                        Nothing, Nothing,
                                                        Nothing, Nothing,
                                                        enum_TipoOperazioneDB.Modifica,
                                                        objParametriServer, servizio)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub


    Public Sub ModificaGriglia(ByVal zooAnimale As AgronicaCoreEntityFramework_POCO.Zoo_Animali,
                               ByRef objParametri As AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL_Zoo_Aninali.ModificaGriglia()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Try


            'Cancellazione Dettagli
            StrSQL.Length = 0
            StrSQL.AppendLine(" Update Zoo_Animali ")
            StrSQL.AppendLine(" Set Raz_Cod =  " & Agro_SQL_SaveNum(zooAnimale.RAZ_COD) & "  ")
            StrSQL.AppendLine("   , MAT_Madre =  '" & Agro_SQL_SaveText(zooAnimale.MAT_MADRE) & "'  ")
            StrSQL.AppendLine("   , Sesso =  '" & Agro_SQL_SaveText(zooAnimale.Sesso) & "'  ")
            StrSQL.AppendLine("   , Certificato =  '" & Agro_SQL_SaveText(zooAnimale.Certificato) & "'  ")
            StrSQL.AppendLine("   , DAT_NASCITA =  " & Agro_SQL_SaveDate(zooAnimale.DAT_NASCITA) & "  ")
            StrSQL.AppendLine("   , Tipo_Cod =  " & Agro_SQL_SaveNum(zooAnimale.TIPO_COD) & "  ")
            StrSQL.AppendLine("   , Cat_Cod =  " & Agro_SQL_SaveNum(zooAnimale.CAT_COD) & "  ")
            StrSQL.AppendLine("   , Username_Modifica = " & Agro_SQL_SaveText_NULL(objParametri.UsernameOperazione))
            StrSQL.AppendLine("   , Data_Modifica = " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine("   , CF_Fornitore = '" & Agro_SQL_SaveText(zooAnimale.CF_Fornitore) & "'  ")
            StrSQL.AppendLine("   , Fornitore_Provenienza = '" & Agro_SQL_SaveText(zooAnimale.Fornitore_Provenienza) & "'  ")
            StrSQL.AppendLine("   , Lotto_Fornitore = '" & Agro_SQL_SaveText(zooAnimale.Lotto_Fornitore) & "'  ")
            StrSQL.AppendLine("   , Modello4_Ingresso = '" & Agro_SQL_SaveText(zooAnimale.Modello4_Ingresso) & "'  ")
            StrSQL.AppendLine("   , Modello4_Ingresso_Numero = '" & Agro_SQL_SaveText(zooAnimale.Modello4_Ingresso_Numero) & "'  ")
            StrSQL.AppendLine("   , N_Bolla_Fornitore = '" & Agro_SQL_SaveText(zooAnimale.N_Bolla_Fornitore) & "'  ")
            StrSQL.AppendLine("   , Data_Documento_Ingresso = " & Agro_SQL_SaveDate(zooAnimale.Data_Documento_Ingresso))
            StrSQL.AppendLine("   , Data_DDT_Ingresso = " & Agro_SQL_SaveDate(zooAnimale.Data_DDT_Ingresso))
            StrSQL.AppendLine("   , Incremento_Teorico = " & Agro_SQL_SaveNum(zooAnimale.Incremento_Teorico))
            StrSQL.AppendLine("   , Tag = '" & Agro_SQL_SaveText(zooAnimale.Tag) & "'  ")
            StrSQL.AppendLine(" Where Piva = '" & Agro_SQL_SaveText(zooAnimale.PIVA) & "' ")
            StrSQL.AppendLine(" And Cod_Progetto  = " & Agro_SQL_SaveNum(zooAnimale.Cod_Progetto) & " ")

            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub






    Public Sub Valorizza(ByRef zooAnimale As AgronicaCoreEntityFramework_POCO.Zoo_Animali,
                         ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_Animali.Valorizza()"
        Dim messaggioErrore As String = ""

        Try

            If zooAnimale.CAT_COD Is Nothing Then
                zooAnimale.CAT_COD = 0
            End If

            If zooAnimale.PESO Is Nothing Then
                zooAnimale.PESO = 0
            End If

            If zooAnimale.DATA_PESA Is Nothing Then
                zooAnimale.DATA_PESA = AGRODATAINIZIO
            End If

            If zooAnimale.Metodo_Produzione Is Nothing Then
                zooAnimale.Metodo_Produzione = 1
            End If

            If zooAnimale.Regolamento_Cod Is Nothing Then
                zooAnimale.Regolamento_Cod = 0
            End If

            If zooAnimale.Conversione_Inizio Is Nothing Then
                zooAnimale.Conversione_Inizio = AGRODATAINIZIO
            End If

            If zooAnimale.Conversione_Fine Is Nothing Then
                zooAnimale.Conversione_Fine = AGRODATAFINE
            End If

            If zooAnimale.Chk_Batteria Is Nothing Then
                zooAnimale.Chk_Batteria = 0
            End If

            If zooAnimale.codZootecnica Is Nothing Then
                zooAnimale.codZootecnica = ""
            End If

            If zooAnimale.descrZootecnica Is Nothing Then
                zooAnimale.descrZootecnica = ""
            End If

            If zooAnimale.Fonte Is Nothing Then
                zooAnimale.Fonte = ""
            End If

            If zooAnimale.ID_Utente Is Nothing Then
                zooAnimale.ID_Utente = ""
            End If

            If zooAnimale.DT_Variazione Is Nothing Then
                zooAnimale.DT_Variazione = AGRODATAINIZIO
            End If

            If zooAnimale.TIPO_COD Is Nothing Then
                zooAnimale.TIPO_COD = 0
            End If

            If zooAnimale.Data_Documento_Ingresso < AGRODATAINIZIO Then
                zooAnimale.Data_Documento_Ingresso = AGRODATAINIZIO
            End If

            If zooAnimale.Data_Documento_Uscita < AGRODATAINIZIO Then
                zooAnimale.Data_Documento_Uscita = AGRODATAFINE
            End If

            If zooAnimale.Stalla_Svezzamento Is Nothing Then
                zooAnimale.Stalla_Svezzamento = ""
            End If

            If zooAnimale.Incremento_Teorico Is Nothing Then
                zooAnimale.Incremento_Teorico = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Elimina(ByRef zooAnimale As AgronicaCoreEntityFramework_POCO.Zoo_Animali,
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreParametri,
                       Optional ByVal servizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_Animali.Elimina()"
        Dim messaggioErrore As String = ""

        Try

            GiasContext.Entry(zooAnimale).State = EntityState.Deleted
            GiasContext.Zoo_Animali.Remove(zooAnimale)

            GiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.ZooAnimali,
                                                        zooAnimale.PIVA, CStr(zooAnimale.Cod_Progetto),
                                                        Nothing, Nothing,
                                                        Nothing, Nothing,
                                                        enum_TipoOperazioneDB.Cancellazione,
                                                        objParametriServer, servizio)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Function GetGenitori(ByVal piva As String,
                                ByVal codProgetto As Integer,
                                ByVal sesso As String,
                                ByRef objParametriServer As AgronicaCoreParametri,
                                Optional ByVal genere As Integer = 0,
                                Optional ByVal specie As Integer = 0
                                ) As Object

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_Animali.GetGenitori()"
        Dim messaggioErrore As String = ""
        Dim objGenitoriList As Object

        Try

            Dim gEfUtils As New Gias_EF_Utility
            Dim efConnString As String = gEfUtils.GetEntityConnectionString(objParametriServer.StringaConnessione)

            Using dal As New Gias_DeveloperServer_Entities(efConnString)

                Dim query = (From a In dal.Zoo_Animali
                             Join d In dal.Zoo_Animali_Distinte
                                 On a.PIVA Equals d.PIVA And
                                    a.sa_cod Equals d.sa_cod And
                                    a.Cod_Progetto Equals d.Cod_Animale
                             Where a.PIVA = piva AndAlso
                                   a.Sesso = sesso AndAlso
                                   a.Cod_Progetto <> codProgetto AndAlso
                                   (genere = If(genere <> 0, a.GEN_COD, genere) AndAlso specie = If(specie <> 0, a.SPE_COD, specie))
                             Select a.Progetto, a.Matricola, d.Cod_Animale, d.Cod_Progetto, d.Codice_Distinta)

                objGenitoriList = query.ToList()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return objGenitoriList

    End Function

    Public Function Leggi_Giacenze_old(Piva As String,
                                   Sa_Cod As Integer,
                                   STA_NUM As Integer,
                                   Raggruppamento_Cod As Integer,
                                   Cod_Animale As Integer,
                                   Data As DateTime,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal bAll As Boolean = False
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Giacenze()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0
            stb.AppendLine("SELECT * FROM ")
            stb.AppendLine("            (SELECT  ")
            stb.AppendLine("    Imprese.Piva, Imprese.Rag_Soc As Impresa, ")
            stb.AppendLine("    Movimenti_dettagli.Cod_Progetto AS Cod_Animale, ")
            stb.AppendLine("    Zoo_Animali_Distinte.Cod_Progetto, ")
            stb.AppendLine("    Zoo_Animali_Distinte.Progetto_Des, ")
            stb.AppendLine("    Zoo_Animali_Distinte.Codice_Distinta, ")
            stb.AppendLine("    Zoo_AnimalixStati_Accrescimento.Stato_Cod, ")
            stb.AppendLine("    Zoo_Animali_Lista_Stati_Accrescimento.Stato_Des, ")

            stb.AppendLine("    Lista_Specie_Animali.SPE_DES, ")
            stb.AppendLine("    Lista_Razze_Animali.RAZ_DES, ")
            stb.AppendLine("    Zoo_Animali_Lista_Tipi.Tipo_Des, ")
            stb.AppendLine("    Zoo_Animali.Mat_Madre, ")
            stb.AppendLine("    Zoo_Animali.Mat_Padre, ")
            stb.AppendLine("    CASE Zoo_Animali.Metodo_Produzione WHEN 1 THEN 'Integrato' WHEN 2 THEN 'In Conversione' WHEN 3 THEN 'Biologico' END as Metodo_Produzione, ")
            stb.AppendLine("    Lista_IndirizziProd_Animali.IPRO_DES, ")
            stb.AppendLine("    Zoo_Animali_Distinte.Progetto_Nome, ")
            stb.AppendLine("    Zoo_Animali_Distinte.Codice_Distinta, ")
            stb.AppendLine("    Zoo_Animali.Lotto_Fornitore, ")
            stb.AppendLine("    Contatti.Rag_Soc, ")
            stb.AppendLine("    Contatti.Cod_Contatto,")

            stb.AppendLine("    Movimenti_dettagli.Lotto, ")
            stb.AppendLine("    Movimenti_dettagli.Udm_Cod, ")
            stb.AppendLine("    Mov_Destinazioni.Sa_Cod, ")
            stb.AppendLine("    Centri_Aziendali.sa_nome, ")
            stb.AppendLine("    Mov_Destinazioni.Id_Destinazione, ")
            stb.AppendLine("    Mov_Destinazioni.Tipo_Destinazione, ")
            stb.AppendLine("    UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim, ")
            stb.AppendLine("    CASE  ")
            stb.AppendLine("    WHEN Mov_Destinazioni.Tipo_Destinazione = 15 ")
            stb.AppendLine("    THEN (SELECT Fabbricato_Des FROM Fabbricati WHERE Fabbricati.piva = Imprese.Piva And Fabbricati.SA_COD=Mov_Destinazioni.Sa_Cod And Fabbricati.Fabbricato_Cod=Mov_Destinazioni.Id_Destinazione) ")
            stb.AppendLine("    Else ( ")
            stb.AppendLine("      Select Fabbricato_Des FROM Stalla_Raggruppamenti ")
            stb.AppendLine("           INNER Join Fabbricati ON Fabbricati.Fabbricato_Cod = Stalla_Raggruppamenti.STA_NUM And Fabbricati.piva = Stalla_Raggruppamenti.piva And Fabbricati.SA_COD= Stalla_Raggruppamenti.sa_cod ")
            stb.AppendLine("           WHERE Stalla_Raggruppamenti.Raggruppamento_Cod = Mov_Destinazioni.Id_Destinazione ")
            stb.AppendLine("      And Stalla_Raggruppamenti.sa_cod = Mov_Destinazioni.Sa_Cod ")
            stb.AppendLine("      And Stalla_Raggruppamenti.piva = Imprese.Piva ")
            stb.AppendLine("        ) ")
            stb.AppendLine("  End As STA_DES, ")
            stb.AppendLine("   Case  ")
            stb.AppendLine("  WHEN Mov_Destinazioni.Tipo_Destinazione = 15 ")
            stb.AppendLine("  THEN (SELECT Fabbricato_Cod FROM Fabbricati WHERE Fabbricati.piva = Imprese.Piva And Fabbricati.SA_COD=Mov_Destinazioni.Sa_Cod And Fabbricati.Fabbricato_Cod=Mov_Destinazioni.Id_Destinazione) ")
            stb.AppendLine("  Else ( ")
            stb.AppendLine("        Select Fabbricato_Cod FROM Stalla_Raggruppamenti ")
            stb.AppendLine("           INNER Join Fabbricati ON Fabbricati.Fabbricato_Cod = Stalla_Raggruppamenti.STA_NUM And Fabbricati.piva = Stalla_Raggruppamenti.piva And Fabbricati.SA_COD= Stalla_Raggruppamenti.sa_cod ")
            stb.AppendLine("           WHERE Stalla_Raggruppamenti.Raggruppamento_Cod = Mov_Destinazioni.Id_Destinazione ")
            stb.AppendLine("        And Stalla_Raggruppamenti.sa_cod = Mov_Destinazioni.Sa_Cod ")
            stb.AppendLine("        And Stalla_Raggruppamenti.piva = Imprese.Piva ")
            stb.AppendLine("        ) ")
            stb.AppendLine("  End As STA_NUM, ")
            stb.AppendLine("   COALESCE(Stalla_Raggruppamenti.Raggruppamento_Des, '') as Raggruppamento_Des, ")
            stb.AppendLine("   COALESCE(Stalla_Raggruppamenti.Raggruppamento_Cod, 0) as Raggruppamento_Cod, ")
            stb.AppendLine("   Zoo_Animali.Matricola, ")
            stb.AppendLine("   Zoo_Animali.GEN_COD, ")
            stb.AppendLine("   Zoo_Animali.SPE_COD, ")
            stb.AppendLine("   Zoo_Animali.TIPO_COD, ")
            stb.AppendLine("   Zoo_Animali.Progetto, ")
            stb.AppendLine("   Zoo_Animali.Validita_Inizio, ")
            stb.AppendLine("   Zoo_Animali.Validita_Fine, ")
            stb.AppendLine("   Zoo_Animali.Sesso, ")
            stb.AppendLine("   Zoo_Animali.Dat_Nascita, ")
            stb.AppendLine("   Convert(INTEGER, SUM( Case When Movimenti.CAU_MOV In ('7350','3750')             ")
            stb.AppendLine("          THEN -(Mov_Destinazioni.qta)            ")
            stb.AppendLine("          Else Mov_Destinazioni.qta            ")
            stb.AppendLine("          End)) As Giacenza  ")
            stb.AppendLine(" From Agenda ")
            stb.AppendLine(" INNER Join Imprese ON Imprese.Piva = Agenda.Piva  ")
            stb.AppendLine(" INNER Join Movimenti ON Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            stb.AppendLine(" INNER Join Movimenti_Dettagli ON Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda  ")
            stb.AppendLine("                               AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")
            stb.AppendLine(" INNER Join Mov_Destinazioni ON Mov_Destinazioni.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            stb.AppendLine("                             AND Mov_Destinazioni.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            stb.AppendLine("                             AND Mov_Destinazioni.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")
            stb.AppendLine(" INNER Join Zoo_Animali ON Zoo_Animali.Cod_Progetto = Movimenti_dettagli.Cod_Progetto ")
            stb.AppendLine(" Left Join Stalla_Raggruppamenti ON Mov_Destinazioni.Sa_Cod = Stalla_Raggruppamenti.sa_cod ")
            stb.AppendLine("                                AND Mov_Destinazioni.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod ")
            stb.AppendLine("                                AND Mov_Destinazioni.Tipo_Destinazione = 21 ")
            stb.AppendLine(" INNER Join UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod  ")
            stb.AppendLine(" LEFT JOIN Centri_Aziendali ON Mov_Destinazioni.Piva = Centri_Aziendali.Piva and Mov_Destinazioni.Sa_Cod=Centri_Aziendali.sa_cod ")
            stb.AppendLine(" LEFT JOIN Zoo_Animali_Distinte ON Zoo_Animali.PIVA = Zoo_Animali_Distinte.PIVA AND Zoo_animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale And CAST(Zoo_Animali_Distinte.Validita_Inizio as date) <=  " & Agro_SQL_SaveDate(Data) & "  And CAST(Zoo_Animali_Distinte.Validita_Fine as date) >=  " & Agro_SQL_SaveDate(Data) & "     ")
            stb.AppendLine(" LEFT JOIN Zoo_AnimalixStati_Accrescimento ON Zoo_Animali.PIVA = Zoo_AnimalixStati_Accrescimento.PIVA AND Zoo_animali.Cod_Progetto = Zoo_AnimalixStati_Accrescimento.Cod_Progetto And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as date) <=  " & Agro_SQL_SaveDate(Data) & "  And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as date) >=  " & Agro_SQL_SaveDate(Data) & "     ")
            stb.AppendLine(" LEFT JOIN Zoo_Animali_Lista_Stati_Accrescimento ON Zoo_AnimalixStati_Accrescimento.GEN_COD = Zoo_Animali_Lista_Stati_Accrescimento.GEN_COD AND Zoo_AnimalixStati_Accrescimento.SPE_COD = Zoo_Animali_Lista_Stati_Accrescimento.SPE_COD AND Zoo_AnimalixStati_Accrescimento.STATO_COD = Zoo_Animali_Lista_Stati_Accrescimento.STATO_COD AND Zoo_AnimalixStati_Accrescimento.TIPO_COD = Zoo_Animali_Lista_Stati_Accrescimento.TIPO_COD ")

            stb.AppendLine(" INNER JOIN Lista_Specie_Animali ON Zoo_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Specie_Animali.SPE_COD ")
            stb.AppendLine(" INNER JOIN Lista_Razze_Animali ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD And Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD ")
            stb.AppendLine(" INNER JOIN Zoo_Animali_Lista_Tipi ON Zoo_Animali.GEN_COD = Zoo_Animali_Lista_Tipi.GEN_COD And Zoo_Animali.SPE_COD = Zoo_Animali_Lista_Tipi.SPE_COD And Zoo_Animali.TIPO_COD = Zoo_Animali_Lista_Tipi.TIPO_COD ")
            stb.AppendLine(" INNER JOIN Lista_IndirizziProd_Animali ON Zoo_Animali.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD And Zoo_Animali.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD ")
            stb.AppendLine(" LEFT  JOIN Contatti ON Zoo_Animali.CF_Fornitore = Contatti.Cod_Contatto")


            stb.AppendLine(" WHERE Agenda.lav_Cod > 0 ")
            stb.AppendLine(" And Movimenti_Dettagli.Elem_Cod = 300 ")
            stb.AppendLine(" And Movimenti.Data_Movimento >=  CONVERT(DateTime,'1900/01/01',120)    ")
            stb.AppendLine(" And Movimenti.Cau_Mov In ('7300', '7350', '3700', '3750')    ")
            stb.AppendLine(" And Movimenti_Dettagli.Jolly_Int = 0 ")
            stb.AppendLine(" And Mov_Destinazioni.Tipo_Destinazione IN (15, 21) ")

            Select Case bAll
                Case False

                    stb.AppendLine(" And Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDateTime(Data) & "   ")
                    stb.AppendLine(" And Zoo_Animali.Validita_Inizio <= " & Agro_SQL_SaveDateTime(Data) & "   ")
                    stb.AppendLine(" And Zoo_Animali.Validita_Fine >= " & Agro_SQL_SaveDateTime(Data) & "   ")

                Case True

                    stb.AppendLine(" And CAST(Movimenti.Data_Movimento As Date) <=  " & Agro_SQL_SaveDate(Data) & "   ")
                    stb.AppendLine(" And CAST(Zoo_Animali.Validita_Inizio as Date) <= " & Agro_SQL_SaveDate(Data) & "   ")
                    stb.AppendLine(" And CAST(Zoo_Animali.Validita_Fine as Date) >= " & Agro_SQL_SaveDate(Data) & "   ")

            End Select



            stb.AppendLine(" GROUP BY Imprese.Piva, Imprese.Rag_Soc, ")
            stb.AppendLine(" Movimenti_dettagli.Cod_Progetto, ")
            stb.AppendLine(" Movimenti_dettagli.Lotto, ")
            stb.AppendLine(" Movimenti_dettagli.Udm_Cod, ")
            stb.AppendLine(" Mov_Destinazioni.Sa_Cod, ")
            stb.AppendLine(" Centri_Aziendali.sa_nome, ")
            stb.AppendLine(" Mov_Destinazioni.Id_Destinazione, ")
            stb.AppendLine(" Mov_Destinazioni.Tipo_Destinazione, ")
            stb.AppendLine(" UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim, ")
            stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Des, ")
            stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Cod, ")
            stb.AppendLine(" Zoo_Animali.Matricola, ")
            stb.AppendLine(" Zoo_Animali.GEN_COD, ")
            stb.AppendLine(" Zoo_Animali.SPE_COD, ")
            stb.AppendLine(" Zoo_Animali.TIPO_COD, ")
            stb.AppendLine(" Zoo_Animali.Progetto, ")
            stb.AppendLine(" Zoo_Animali.Validita_Inizio, ")
            stb.AppendLine(" Zoo_Animali.Validita_Fine, ")
            stb.AppendLine(" Zoo_Animali.Sesso, ")
            stb.AppendLine(" Zoo_Animali.Dat_Nascita, ")

            stb.AppendLine(" Lista_Specie_Animali.SPE_DES, ")
            stb.AppendLine(" Lista_Razze_Animali.RAZ_DES, ")
            stb.AppendLine(" Zoo_Animali_Lista_Tipi.Tipo_Des, ")
            stb.AppendLine(" Zoo_Animali.Mat_Madre, ")
            stb.AppendLine(" Zoo_Animali.Mat_Padre, ")
            stb.AppendLine(" Zoo_Animali.Metodo_Produzione, ")
            stb.AppendLine(" Lista_IndirizziProd_Animali.IPRO_DES, ")
            stb.AppendLine(" Zoo_Animali_Distinte.Progetto_Nome, ")
            stb.AppendLine(" Zoo_Animali_Distinte.Codice_Distinta, ")
            stb.AppendLine(" Zoo_Animali.Lotto_Fornitore, ")
            stb.AppendLine(" Contatti.Rag_Soc, ")
            stb.AppendLine(" Contatti.Cod_Contatto,")

            stb.AppendLine(" Zoo_Animali_Distinte.Cod_Progetto, ")
            stb.AppendLine(" Zoo_Animali_Distinte.Progetto_Des, ")
            stb.AppendLine("    Zoo_Animali_Distinte.Codice_Distinta, ")
            stb.AppendLine(" Zoo_AnimalixStati_Accrescimento.Stato_Cod, ")
            stb.AppendLine(" Zoo_Animali_Lista_Stati_Accrescimento.Stato_Des ")

            stb.AppendLine(" HAVING Convert(INTEGER, SUM(Case When Movimenti.Cau_Mov In ('7350','3750') THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END)) <> 0  ")



            stb.AppendLine(" ) a ")
            stb.AppendLine(" WHERE 1=1 ")

            If Piva <> "" Then
                stb.AppendLine(" And a.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And a.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND a.Sta_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If

            If Raggruppamento_Cod <> 0 Then
                stb.AppendLine(" AND a.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "  ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND a.Cod_Animale = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If

            stb.AppendLine("  ORDER BY Impresa")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Giacenze_CTE(Piva As String,
                                   Sa_Cod As Integer,
                                   STA_NUM As Integer,
                                   Raggruppamento_Cod As Integer,
                                   Cod_Animale As Integer,
                                   Data As DateTime,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal bAll As Boolean = False,
                                   Optional ByVal Filtro_Visibilita_Utente As Boolean = False,
                                   Optional ByVal listCod_Animali As List(Of Integer) = Nothing,
                                   Optional ByVal filtraGiacenze1 As Boolean = True,
                                   Optional ByVal filtraFornitori As Boolean = False,
                                   Optional ByVal mostraPesate As Boolean = False,
                                   Optional ByVal mostraAnomalie As Boolean = False,
                                   Optional xFiltroAggiuntivo As String = "",
                                   Optional ByVal Matricola As String = "",
                                   Optional ByVal MostraGGPrimoCaricamento As Boolean = False,
                                   Optional ByVal listMatricola_Animali As List(Of String) = Nothing) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Giacenze2()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            Dim IDTestataTemp As Integer = 0
            Dim IDTestataTempMatricola As Integer = 0
            'CancellaRecordDaIDTestataTemp
            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then

                Dim listTmp_Agenda = (From el In listCod_Animali Select New __Tmp_Agenda_Model() With {
                                                                 .id_agenda = el,
                                                                 .piva = "",
                                                                 .lav_cod = 0,
                                                                 .raccoglitore_cod = 0
                                                              }).ToList()
                objTmp_AgendaTemp.ScriviMassivo(IDTestataTemp, listTmp_Agenda, objParametri)
            End If
            If listMatricola_Animali IsNot Nothing AndAlso listMatricola_Animali.Count > 0 Then

                Dim listTmp_Agenda = (From el In listMatricola_Animali Select New __Tmp_Agenda_Model() With {
                                                                 .id_agenda = 0,
                                                                 .piva = el,
                                                                 .lav_cod = 0,
                                                                 .raccoglitore_cod = 0
                                                              }).ToList()
                objTmp_AgendaTemp.ScriviMassivo(IDTestataTemp, listTmp_Agenda, objParametri)

                'For Each mat In listMatricola_Animali
                '    objTmp_AgendaTemp.Scrivi(IDTestataTempMatricola, mat, 0, 0, objParametri)
                'Next
            End If

            stb.Length = 0
            stb.AppendLine(" with  ")
            'If Filtro_Visibilita_Utente Then
            '    stb.AppendLine(" piva_visibili as ")
            '    stb.AppendLine(" ( ")
            '    stb.AppendLine(" select Piva  ")
            '    stb.AppendLine(" FROM  utenti_Visibilita_Appoggio ")
            '    stb.AppendLine(" WHERE Sa_Cod = 0 AND Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            '    stb.AppendLine(" ), ")
            'End If
            stb.AppendLine(" agn_cte as ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" select a.Id_Agenda,  ")
            stb.AppendLine("	a.PIVA,  ")
            stb.AppendLine("	md.Cod_Progetto, ")
            stb.AppendLine("	md.Lotto, ")
            stb.AppendLine("	md.Udm_Cod, ")
            stb.AppendLine("	UnitaMisura.Udm_Des, ")
            stb.AppendLine("	UnitaMisura.Udm_Sim, ")
            stb.AppendLine("	mdes.Sa_Cod, ")
            stb.AppendLine("	mdes.Id_Destinazione, ")
            stb.AppendLine("	mdes.Tipo_Destinazione, ")
            stb.AppendLine("	mdes.Qta, ")
            stb.AppendLine("	md.Id_Mov, ")
            stb.AppendLine("	md.Id_Mov_Det, ")
            stb.AppendLine("	m.Cau_Mov, ")
            stb.AppendLine("	i.rag_soc, ")
            stb.AppendLine("	ic.val_cod as cuaa, ")
            stb.AppendLine("	Centri_Aziendali.sa_nome ")
            stb.AppendLine("	from agenda a (NOLOCK)  ")
            stb.AppendLine("	inner join imprese i (NOLOCK) on i.PIVA = a.PIVA ")
            stb.AppendLine("	inner join imprese_codici ic (NOLOCK) on i.piva = ic.piva and ic.id_cod = 1010 ")
            stb.AppendLine("	inner join Movimenti m (NOLOCK) on	a.Id_Agenda = m.Id_Agenda  ")
            stb.AppendLine("	inner join Movimenti_dettagli md (NOLOCK) on md.Id_Agenda = m.Id_Agenda  AND md.Id_Mov = m.Id_Mov  ")
            stb.AppendLine("	inner join mov_destinazioni mdes (NOLOCK) on mdes.Id_Agenda = md.Id_Agenda and mdes.Id_Mov = md.Id_Mov and mdes.Id_Mov_Det = md.Id_Mov_Det ")
            stb.AppendLine("	inner join Centri_Aziendali (NOLOCK) ON mdes.Piva = Centri_Aziendali.Piva and mdes.Sa_Cod=Centri_Aziendali.sa_cod  ")
            stb.AppendLine("	INNER Join UnitaMisura (NOLOCK) ON UnitaMisura.Udm_Cod = md.Udm_Cod ")
            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                stb.AppendLine("	INNER Join __Tmp_Agenda (NOLOCK) ON md.Cod_Progetto = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
            End If

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON a.Piva = p.piva AND p.Sa_Cod = mdes.Sa_Cod  AND p.entita_Cod = 2 AND p.Appezza = 0 AND p.Id_Reg = 0 AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            End If
            'If Filtro_Visibilita_Utente And Piva = "" Then
            '    stb.AppendLine("	inner join piva_visibili p (NOLOCK) ON a.Piva = p.piva  ")
            'End If
            stb.AppendLine("	where  1 = 1 ")
            If Piva <> "" Then
                stb.AppendLine(" And a.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And mdes.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND md.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If

            stb.AppendLine("	and a.lav_Cod >= 3000 And a.lav_cod < 4000 ")
            stb.AppendLine("	And m.Cau_Mov In ('7300', '7350', '3700', '3750')     ")
            stb.AppendLine("	And m.Data_Movimento >=  CONVERT(DateTime,'1900/01/01',120)     ")

            If filtraGiacenze1 Then
                Select Case bAll
                    Case False
                        stb.AppendLine("	And m.Data_Movimento <=   " & Agro_SQL_SaveDateTime(Data) & "     ")
                    Case True
                        stb.AppendLine("	And m.Data_Movimento <=   " & Agro_SQL_SaveDate(Data) & "     ")
                End Select
            End If

            stb.AppendLine("	And md.Elem_Cod = 300  ")
            stb.AppendLine("	And md.Jolly_Int = 0  ")
            stb.AppendLine("	And mdes.Tipo_Destinazione IN (15, 21)  ")
            stb.AppendLine("), ")
            stb.AppendLine("giac_cte as ( ")
            stb.AppendLine("	SELECT   ")
            stb.AppendLine("  agn_cte.Piva, agn_cte.Rag_Soc As Impresa,  ")
            stb.AppendLine("  agn_cte.cuaa, Stalla.BDN_Codice_Azienda,  ")
            stb.AppendLine("  agn_cte.Cod_Progetto AS Cod_Animale,  ")
            stb.AppendLine("  agn_cte.Lotto,  ")
            stb.AppendLine("  agn_cte.Udm_Cod,  ")
            stb.AppendLine("  agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
            stb.AppendLine("  agn_cte.Sa_Cod,  ")
            stb.AppendLine("  agn_cte.sa_nome,  ")
            stb.AppendLine("  agn_cte.Id_Destinazione,  ")
            stb.AppendLine("  agn_cte.Tipo_Destinazione,  ")
            stb.AppendLine("  COALESCE(Fabbricati.Fabbricato_Des, '') As STA_DES,  ")
            stb.AppendLine("	COALESCE(Fabbricati.Fabbricato_Cod, 0) As STA_NUM,  ")
            stb.AppendLine("  COALESCE(Stalla_Raggruppamenti.Raggruppamento_Des, '') as Raggruppamento_Des,  ")
            stb.AppendLine("  COALESCE(Stalla_Raggruppamenti.Raggruppamento_Cod, 0) as Raggruppamento_Cod,  ")
            stb.AppendLine("  Convert(INTEGER, SUM( Case When agn_cte.CAU_MOV In ('7350','3750')              ")
            stb.AppendLine("          THEN -(agn_cte.qta)             ")
            stb.AppendLine("          Else agn_cte.qta             ")
            stb.AppendLine("          End)) As Giacenza   ")
            stb.AppendLine(" From agn_cte  ")
            stb.AppendLine(" INNER Join Stalla_Raggruppamenti (NOLOCK) ON agn_cte.Sa_Cod = Stalla_Raggruppamenti.sa_cod  ")
            stb.AppendLine("                                AND agn_cte.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod  ")
            stb.AppendLine("                                AND agn_cte.Tipo_Destinazione = 21  ")
            stb.AppendLine(" INNER JOIN Fabbricati (NOLOCK) ON Stalla_Raggruppamenti.PIVA = Fabbricati.Piva AND Stalla_Raggruppamenti.sa_cod = Fabbricati.SA_COD AND Stalla_Raggruppamenti.STA_NUM = Fabbricati.Fabbricato_Cod ")
            stb.AppendLine(" INNER JOIN Stalla (NOLOCK) ON Stalla.PIVA = Fabbricati.Piva AND Stalla.sa_cod = Fabbricati.SA_COD AND Stalla.STA_NUM = Fabbricati.Fabbricato_Cod  ")
            If mostraAnomalie Then
                stb.AppendLine(" LEFT Join Zoo_AnimalixAnomalie (NOLOCK) ON Zoo_AnimalixAnomalie.Cod_Animale = agn_cte.Cod_Progetto  ")
            End If
            stb.AppendLine("  where 1=1  ")
            If Sa_Cod <> 0 Then
                stb.AppendLine(" And agn_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If
            stb.AppendLine(" GROUP BY agn_cte.Piva, agn_cte.Rag_Soc, agn_cte.cuaa, Stalla.BDN_Codice_Azienda, ")
            stb.AppendLine(" agn_cte.Cod_Progetto,  ")
            stb.AppendLine(" agn_cte.Lotto,  ")
            stb.AppendLine(" agn_cte.Udm_Cod, ")
            stb.AppendLine(" Fabbricati.Fabbricato_Des, ")
            stb.AppendLine(" Fabbricati.Fabbricato_Cod, ")
            stb.AppendLine(" agn_cte.Sa_Cod,  ")
            stb.AppendLine(" agn_cte.sa_nome, ")
            stb.AppendLine(" agn_cte.Id_Destinazione,  ")
            stb.AppendLine(" agn_cte.Tipo_Destinazione,  ")
            stb.AppendLine(" agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
            stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Des,  ")
            stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Cod  ")
            If filtraGiacenze1 Then
                stb.AppendLine(" HAVING Convert(INTEGER, SUM(Case When agn_cte.Cau_Mov In ('7350','3750') THEN -(agn_cte.qta) ELSE agn_cte.qta END)) <> 0   ")
            End If
            stb.AppendLine(") ")
            If MostraGGPrimoCaricamento Then
                stb.AppendLine(" ,  ")
                stb.AppendLine("  ZooMatricoleList as (  ")
                stb.AppendLine("  	SELECT Zoo_Animali.Matricola  ")
                stb.AppendLine("  	FROM giac_cte   ")
                stb.AppendLine("  	INNER Join Zoo_Animali (NOLOCK) ON Zoo_Animali.Cod_Progetto = giac_cte.Cod_Animale    ")
                stb.AppendLine("  	WHERE 1=1   ")
                stb.AppendLine("  ),   ")
                stb.AppendLine("  DataPrimoCaricamento as (  ")
                stb.AppendLine("  SELECT ZooMatricoleList.Matricola, MIN(Movimenti.Data_Movimento) as Data_Movimento  ")
                stb.AppendLine("  FROM Agenda (NOLOCK)  ")
                stb.AppendLine("  JOIN Movimenti (NOLOCK) ON Agenda.ID_Agenda = Movimenti.ID_Agenda  ")
                stb.AppendLine("  JOIN Movimenti_Dettagli (NOLOCK) ON Movimenti.ID_Agenda = Movimenti_Dettagli.ID_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov AND Movimenti_dettagli.Elem_Cod = 300  ")
                stb.AppendLine("  JOIN Zoo_Animali (NOLOCK) ON Movimenti_Dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto  ")
                stb.AppendLine("  JOIN ZooMatricoleList  ON Zoo_Animali.Matricola = ZooMatricoleList.Matricola  ")
                stb.AppendLine("  WHERE 1 = 1  ")
                If Piva <> "" Then
                    stb.AppendLine(" And Agenda.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
                End If
                stb.AppendLine("  GROUP BY ZooMatricoleList.Matricola  ")
                stb.AppendLine("  )  ")
            End If
            If mostraPesate Then
                stb.AppendLine(" , pesate_cte as ( ")
                stb.AppendLine(" 	SELECT Movimenti_dettagli.Cod_Progetto, MAX(Data_Movimento) as Data_Peso_Max, MIN(Data_Movimento) as Data_Peso_Min  ")
                'stb.AppendLine(" 	FROM Agenda (NOLOCK)  ")
                'stb.AppendLine(" 	JOIN Movimenti (NOLOCK)  ON Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                stb.AppendLine(" 	FROM Movimenti (NOLOCK)  ")
                stb.AppendLine(" 	JOIN Movimenti_dettagli (NOLOCK)  ON Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
                stb.AppendLine(" 	JOIN giac_cte ON Movimenti_dettagli.Cod_Progetto = giac_cte.Cod_Animale ")
                stb.AppendLine(" 	WHERE Movimenti.Cau_Mov = '" & CAU_PESATURA_ANIMALI & "' ") '" AND Agenda.Lav_Cod = " & LAVCOD_PESATURA_ANIMALI & " ")
                Select Case bAll
                    Case False
                        stb.AppendLine("	And Data_Movimento <=   " & Agro_SQL_SaveDateTime(Data.AddMinutes(1)) & "     ")
                    Case True
                        stb.AppendLine("	And Data_Movimento <=   " & Agro_SQL_SaveDate(Data) & "     ")
                End Select
                stb.AppendLine("    GROUP BY Movimenti_dettagli.Cod_Progetto ")
                stb.AppendLine(" ) , ")
                stb.AppendLine(" max_pesate_ct as ( ")
                stb.AppendLine(" 	SELECT Movimenti_dettagli.Qta, Movimenti_dettagli.Cod_Progetto, Movimenti.Data_Movimento as Data_Peso ")
                'stb.AppendLine(" 	FROM Agenda  (NOLOCK)  ")
                'stb.AppendLine(" 	JOIN Movimenti (NOLOCK)  ON Agenda.Id_Agenda = Movimenti.Id_Agenda   ")
                stb.AppendLine(" 	FROM Movimenti (NOLOCK)  ")
                stb.AppendLine(" 	JOIN Movimenti_dettagli (NOLOCK)  ON Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov   ")
                stb.AppendLine(" 	JOIN pesate_cte ON pesate_cte.Cod_Progetto = Movimenti_dettagli.Cod_Progetto AND Movimenti.Data_Movimento = pesate_cte.Data_Peso_Max   ")
                stb.AppendLine(" 	WHERE Movimenti.Cau_Mov = '" & CAU_PESATURA_ANIMALI & "' ")
                stb.AppendLine(" ), ")
                stb.AppendLine(" min_pesate_ct as ( ")
                stb.AppendLine(" 	SELECT Movimenti_dettagli.Qta, Movimenti_dettagli.Cod_Progetto, Movimenti.Data_Movimento as Data_Peso ")
                'stb.AppendLine(" 	FROM Agenda (NOLOCK)   ")
                'stb.AppendLine(" 	JOIN Movimenti (NOLOCK)  ON Agenda.Id_Agenda = Movimenti.Id_Agenda   ")
                stb.AppendLine(" 	FROM Movimenti (NOLOCK)  ")
                stb.AppendLine(" 	JOIN Movimenti_dettagli (NOLOCK)  ON Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov   ")
                stb.AppendLine(" 	JOIN pesate_cte ON pesate_cte.Cod_Progetto = Movimenti_dettagli.Cod_Progetto AND Movimenti.Data_Movimento = pesate_cte.Data_Peso_Min   ")
                stb.AppendLine(" 	WHERE Movimenti.Cau_Mov = '" & CAU_PESATURA_ANIMALI & "' ")
                stb.AppendLine(" ) ")
            End If
            If mostraAnomalie Then
                stb.AppendLine(" , anomalie_ct as (  ")
                stb.AppendLine(" 	SELECT giac_cte.cod_animale, COALESCE(STRING_AGG(Zoo_AnimalixAnomalie.Codice, ','), '') as Anomalie, COALESCE(STRING_AGG(Zoo_Animali_Anomalie.descrizione, ','), '') as Anomalie_Str   ")
                stb.AppendLine(" 	FROM Zoo_AnimalixAnomalie (NOLOCK)  ")
                stb.AppendLine(" 	JOIN giac_cte ON Zoo_AnimalixAnomalie.Cod_Animale = giac_cte.Cod_Animale   ")
                stb.AppendLine(" 	JOIN Zoo_Animali_Anomalie (NOLOCK)  ON Zoo_AnimalixAnomalie.Codice = Zoo_Animali_Anomalie.Codice   ")
                stb.AppendLine(" 	GROUP BY giac_cte.cod_animale   ")
                stb.AppendLine(" ) ")
            End If
            stb.AppendLine("SELECT giac_cte.Piva + '_' + CAST(giac_cte.Sa_Cod as varchar(100)) + '_' + CAST(giac_cte.Cod_Animale as varchar(100)) as chiave,  ")
            stb.AppendLine("giac_cte.*,  ")

            stb.AppendLine("Lista_Specie_Animali.SPE_DES,  ")
            stb.AppendLine("Lista_Razze_Animali.RAZ_DES,  ")
            stb.AppendLine("Zoo_Animali_Lista_Tipi.Tipo_Des,  ")
            stb.AppendLine("Lista_IndirizziProd_Animali.IPRO_DES,  ")
            If filtraGiacenze1 Then
                stb.AppendLine("Zoo_Animali_Distinte.Cod_Progetto,  ")
                stb.AppendLine("Zoo_Animali_Distinte.Progetto_Des,  ")
                stb.AppendLine("Zoo_Animali_Distinte.Progetto_Nome,  ")
                stb.AppendLine("Zoo_Animali_Distinte.Codice_Distinta,  ")
                stb.AppendLine("Zoo_AnimalixStati_Accrescimento.Stato_Cod,  ")
                stb.AppendLine("Zoo_Animali_Lista_Stati_Accrescimento.Stato_Des,  ")
            End If

            If filtraFornitori Then
                stb.AppendLine("COALESCE(Contatti_FornFatt.Cod_Contatto, '') AS CF_FornFatt,  ")
                stb.AppendLine("COALESCE(Contatti_FornFatt.Rag_Soc + Contatti_FornFatt.cognome + ' ' + Contatti_FornFatt.Nome, '') AS RagSoc_FornFatt,  ")

                stb.AppendLine("COALESCE(Contatti_FornProv.Cod_Contatto, '') AS CF_FornProv,  ")
                stb.AppendLine("COALESCE(Contatti_FornProv.Rag_Soc + Contatti_FornProv.cognome + ' ' + Contatti_FornProv.Nome, '') AS RagSoc_FornProv,  ")
            End If

            stb.AppendLine(" COALESCE(Contatti.Rag_Soc, '') as Rag_Soc,  ")
            stb.AppendLine(" COALESCE(Contatti.Cod_Contatto, '') as Cod_Contatto, ")
            stb.AppendLine(" Zoo_Animali.RAZ_COD,  ")
            stb.AppendLine(" 	Zoo_Animali.IPRO_COD,  ")
            stb.AppendLine(" 	COALESCE(Zoo_Animali.CF_Fornitore, '') as CF_Fornitore,  ")
            stb.AppendLine("   Zoo_Animali.Mat_Madre,   ")
            stb.AppendLine("   Zoo_Animali.Mat_Padre,   ")
            stb.AppendLine("   CASE Zoo_Animali.Metodo_Produzione WHEN 1 THEN 'Integrato' WHEN 2 THEN 'In Conversione' WHEN 3 THEN 'Biologico' END as Metodo_Produzione,   ")
            stb.AppendLine("   Zoo_Animali.Razza_Padre AS RazCod_Padre,   ")
            stb.AppendLine("   razza_madre.RAZ_DES as RazDes_Madre,   ")
            stb.AppendLine("   Zoo_Animali.Razza_Madre AS RazCod_Madre,   ")
            stb.AppendLine("   razza_padre.RAZ_DES as RazDes_Padre,   ")
            stb.AppendLine("   Zoo_Animali.Lotto_Fornitore, ")
            stb.AppendLine("   Zoo_Animali.Matricola,   ")
            stb.AppendLine("   RIGHT(Zoo_Animali.Matricola, 5) AS Matricola_Breve,   ")
            stb.AppendLine("   RIGHT(Zoo_Animali.Matricola, 4) AS Matricola_Breve4,   ")
            stb.AppendLine("   Zoo_Animali.GEN_COD,   ")
            stb.AppendLine("   Zoo_Animali.SPE_COD,   ")
            stb.AppendLine("   Zoo_Animali.TIPO_COD,   ")
            stb.AppendLine("   Zoo_Animali.Progetto,   ")
            stb.AppendLine("   CAST(Zoo_Animali.Validita_Inizio as date) as Validita_Inizio,   ")
            stb.AppendLine("   CAST(Zoo_Animali.Validita_Fine as date) as Validita_Fine,   ")
            stb.AppendLine("   Zoo_Animali.Nome,   ")
            stb.AppendLine("   Zoo_Animali.Sesso,   ")
            stb.AppendLine("   CASE WHEN Zoo_Animali.Validato = 1 THEN 'Si' ELSE 'No' END AS Validato,   ")
            stb.AppendLine("   Zoo_Animali.Modello4_Ingresso,   ")
            stb.AppendLine("   Zoo_Animali.Modello4_Uscita,   ")
            stb.AppendLine("   ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ZOO_Animali.Username_Creazione), ZOO_Animali.Username_Creazione) AS Utente_Creazione,   ")
            stb.AppendLine("   ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ZOO_Animali.Username_Modifica), ZOO_Animali.Username_Modifica) AS Utente_Modifica,   ")
            stb.AppendLine("   Zoo_Animali.Data_Creazione,   ")
            stb.AppendLine("   Zoo_Animali.Data_Modifica,   ")
            stb.AppendLine("   Zoo_Animali.Dat_Nascita,   ")
            stb.AppendLine("   Zoo_Animali.Id_Capo_BDN,   ")
            stb.AppendLine("   Zoo_Animali.CF_PROPRIETARIO,   ")
            stb.AppendLine("   Zoo_Animali.CF_DETENTORE,   ")
            stb.AppendLine("   COALESCE(Zoo_Animali.AUSL_AZI_NASCITA, '') as AUSL_AZI_NASCITA,   ")
            stb.AppendLine("   CASE WHEN Zoo_Animali.Id_Capo_BDN = 0 THEN 'No' ELSE 'Si' END AS FlagBDN,   ")
            stb.AppendLine("   Zoo_Animali.Certificato  ")

            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Ingresso_Numero, '') as Modello4_Ingresso_Numero  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Uscita_Numero, '') as Modello4_Uscita_Numero ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Ingresso_Prenotazione, '') as Modello4_Ingresso_Prenotazione  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Uscita_Prenotazione, '') as Modello4_Uscita_Prenotazione  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Codice_Azienda_Uscita, '') as Codice_Azienda_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_Documento_Ingresso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_Documento_Ingresso  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_Documento_Uscita, CONVERT(datetime, '2100-12-31 00:00:00.000', 120)) as Data_Documento_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Fornitore_Provenienza, '') as Fornitore_Provenienza  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.N_Bolla_Fornitore, '') as N_Bolla_Fornitore  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.N_Bolla_Uscita, '') as N_Bolla_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_DDT_Ingresso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_DDT_Ingresso  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_DDT_Uscita, CONVERT(datetime, '2100-12-31 00:00:00.000', 120)) as Data_DDT_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Codice_Azienda_Fornitore, '') as Codice_Azienda_Fornitore  ")
            stb.AppendLine("   , COALESCE(Contatti_StallaSvezz.Cod_Contatto, '') AS CF_StallaSvezz  ")
            stb.AppendLine("   , COALESCE(Contatti_StallaSvezz.Rag_Soc + Contatti_StallaSvezz.cognome + ' ' + Contatti_StallaSvezz.Nome, '') AS RagSoc_StallaSvezz  ")
            stb.AppendLine("   , Zoo_Animali.Stalla_Svezzamento  ")
            stb.AppendLine("   , Zoo_Animali.Note ")
            stb.AppendLine("   , Zoo_Animali.Anomalie_Note ")
            stb.AppendLine("   , Zoo_Animali.Id_Patologia AS Patologia_Cod ")
            stb.AppendLine("   , COALESCE(Lista_Patologie.Patologia_Des, '') AS Patologia_Des ")
            stb.AppendLine("   , Zoo_Animali.Incremento_Teorico AS Incremento_Teorico ")
            If MostraGGPrimoCaricamento Then
                stb.AppendLine("   , DATEDIFF(day, DataPrimoCaricamento.Data_Movimento,   " & Agro_SQL_SaveDate(Data, False) & "  ) as giorni_stalla_primo_caricamento ")
                stb.AppendLine("   , DataPrimoCaricamento.Data_Movimento as data_primo_caricamento ")
            End If
            If filtraGiacenze1 Then
                stb.AppendLine("  ,  DATEDIFF(day, Zoo_Animali.Validita_Inizio, " & Agro_SQL_SaveDate(Data, False) & ") as giorni_in_stalla ")
                stb.AppendLine("   , DATEDIFF(day, Zoo_Animali.DAT_NASCITA, " & Agro_SQL_SaveDate(Data, False) & ") As Eta_Giorni_TOTALI ")
                stb.AppendLine("   , IIF(DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") , Zoo_Animali.Dat_Nascita), " & Agro_SQL_SaveDate(Data, False) & ") >= 0,  ")
                stb.AppendLine("      DATEDIFF(month, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") ")
                stb.AppendLine("   , DATEDIFF(month, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") -1)  as eta_mesi ")
                stb.AppendLine("   , IIF(DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") , Zoo_Animali.Dat_Nascita), " & Agro_SQL_SaveDate(Data, False) & ") >= 0 ")
                stb.AppendLine("   , DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") , Zoo_Animali.Dat_Nascita), " & Agro_SQL_SaveDate(Data, False) & ") ")
                stb.AppendLine("   , DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") - 1, Zoo_Animali.Dat_Nascita), " & Agro_SQL_SaveDate(Data, False) & ")) as eta_giorni ")
            End If
            If mostraPesate Then
                stb.AppendLine("  , COALESCE(min_pesate_ct.Data_Peso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_Prima_Pesata ")
                stb.AppendLine("  , min_pesate_ct.Qta as Qta_Prima_Pesata ")
                stb.AppendLine("  , COALESCE(max_pesate_ct.Data_Peso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_Ultima_Pesata ")
                stb.AppendLine("  , max_pesate_ct.Qta as Qta_Ultima_Pesata ")
                stb.AppendLine("  , ((DATEDIFF(day, COALESCE(min_pesate_ct.Data_Peso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)), " & Agro_SQL_SaveDate(Data, False) & ")) * Zoo_Animali.Incremento_Teorico) + min_pesate_ct.Qta AS Incremento_Teorico_Calcolato ")
            End If
            If mostraAnomalie Then
                stb.AppendLine("   , COALESCE(anomalie_ct.Anomalie, '') as Anomalie ")
                stb.AppendLine("   , COALESCE(anomalie_ct.Anomalie_Str, '') as Anomalie_Str ")
                '
            End If
            stb.AppendLine("   , COALESCE(Zoo_Animali.Anomalie_Note, '') as Anomalie_Note ")
            stb.AppendLine("FROM giac_cte ")
            stb.AppendLine(" INNER Join Zoo_Animali (NOLOCK) ON Zoo_Animali.Cod_Progetto = giac_cte.Cod_Animale  ")
            If listMatricola_Animali IsNot Nothing AndAlso listMatricola_Animali.Count > 0 Then
                stb.AppendLine("	INNER Join __Tmp_Agenda tmp_agenda (NOLOCK) ON Zoo_Animali.Matricola = tmp_agenda.Piva AND tmp_agenda.id_agenda = 0 AND tmp_agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTempMatricola) & " ")
            End If

            If MostraGGPrimoCaricamento Then
                stb.AppendLine(" INNER JOIN DataPrimoCaricamento ON Zoo_Animali.Matricola = DataPrimoCaricamento.Matricola ")
            End If
            If filtraGiacenze1 Then
                stb.AppendLine(" INNER JOIN Zoo_Animali_Distinte (NOLOCK) ON Zoo_Animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale  ")
                Select Case bAll
                    Case False
                        stb.AppendLine("                                         AND CAST(Zoo_Animali_Distinte.Validita_Inizio as datetime) <=    " & Agro_SQL_SaveDateTime(Data) & " ")
                        stb.AppendLine("                                         And CAST(Zoo_Animali_Distinte.Validita_Fine as datetime) >=    " & Agro_SQL_SaveDateTime(Data) & " ")
                    Case True
                        stb.AppendLine("                                         AND CAST(Zoo_Animali_Distinte.Validita_Inizio as date) <=    " & Agro_SQL_SaveDate(Data) & " ")
                        stb.AppendLine("                                         And CAST(Zoo_Animali_Distinte.Validita_Fine as date) >=    " & Agro_SQL_SaveDate(Data) & " ")
                End Select
            End If
            If filtraGiacenze1 Then
                stb.AppendLine(" INNER JOIN Zoo_AnimalixStati_Accrescimento (NOLOCK)  ON Zoo_AnimalixStati_Accrescimento.Cod_Progetto  = Zoo_Animali.Cod_Progetto  ")
                'Select Case bAll
                '    Case False
                '        stb.AppendLine("                                           AND CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as datetime) <=    " & Agro_SQL_SaveDateTime(Data) & " ")
                '        stb.AppendLine("                                           And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as datetime) >=   " & Agro_SQL_SaveDateTime(Data) & " ")
                '    Case True
                '        stb.AppendLine("                                           AND CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as date) <=    " & Agro_SQL_SaveDate(Data) & " ")
                '        stb.AppendLine("                                           And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as date) >=   " & Agro_SQL_SaveDate(Data) & " ")
                'End Select
                stb.AppendLine("                                           AND CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as date) <=    " & Agro_SQL_SaveDate(Data) & " ")
                stb.AppendLine("                                           And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as date) >=   " & Agro_SQL_SaveDate(Data) & " ")

            End If
            If filtraGiacenze1 Then
                stb.AppendLine("INNER JOIN Zoo_Animali_Lista_Stati_Accrescimento (NOLOCK) ON Zoo_AnimalixStati_Accrescimento.GEN_COD = Zoo_Animali_Lista_Stati_Accrescimento.GEN_COD  ")
                stb.AppendLine("												AND Zoo_AnimalixStati_Accrescimento.SPE_COD = Zoo_Animali_Lista_Stati_Accrescimento.SPE_COD  ")
                stb.AppendLine("												AND Zoo_AnimalixStati_Accrescimento.STATO_COD = Zoo_Animali_Lista_Stati_Accrescimento.STATO_COD  ")
                stb.AppendLine("												AND Zoo_AnimalixStati_Accrescimento.TIPO_COD = Zoo_Animali_Lista_Stati_Accrescimento.TIPO_COD  ")
            End If
            stb.AppendLine("INNER JOIN Lista_Specie_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Specie_Animali.SPE_COD  ")
            stb.AppendLine("INNER JOIN Lista_Razze_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD And Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD  ")
            stb.AppendLine(" INNER JOIN Lista_Razze_Animali razza_madre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_madre.GEN_COD And Zoo_Animali.SPE_COD = razza_madre.SPE_COD And Zoo_Animali.Razza_Madre = razza_madre.RAZ_COD   ")
            stb.AppendLine(" INNER JOIN Lista_Razze_Animali razza_padre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_padre.GEN_COD And Zoo_Animali.SPE_COD = razza_padre.SPE_COD And Zoo_Animali.Razza_Padre = razza_padre.RAZ_COD   ")
            stb.AppendLine("INNER JOIN Zoo_Animali_Lista_Tipi (NOLOCK) ON Zoo_Animali.GEN_COD = Zoo_Animali_Lista_Tipi.GEN_COD And Zoo_Animali.SPE_COD = Zoo_Animali_Lista_Tipi.SPE_COD And Zoo_Animali.TIPO_COD = Zoo_Animali_Lista_Tipi.TIPO_COD  ")
            stb.AppendLine("INNER JOIN Lista_IndirizziProd_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD And Zoo_Animali.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD  ")
            If mostraPesate Then
                stb.AppendLine(" LEFT JOIN min_pesate_ct ON Zoo_Animali.Cod_Progetto = min_pesate_ct.Cod_Progetto ")
                stb.AppendLine(" LEFT JOIN max_pesate_ct ON Zoo_Animali.Cod_Progetto = max_pesate_ct.Cod_Progetto ")
            End If
            If mostraAnomalie Then
                stb.AppendLine(" LEFT JOIN anomalie_ct ON anomalie_ct.Cod_Animale = giac_cte.Cod_Animale ")
            End If
            stb.AppendLine("LEFT  JOIN Contatti (NOLOCK) ON Zoo_Animali.CF_Fornitore = Contatti.Cod_Contatto AND Zoo_Animali.Piva = Contatti.Piva ")
            stb.AppendLine("  LEFT JOIN Lista_Patologie (NOLOCK)  ON Lista_Patologie.Patologia_Cod = Zoo_Animali.Id_Patologia  ")
            If filtraFornitori Then
                stb.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Contatti  (NOLOCK) ")
                stb.AppendLine("             WHERE Contatti.Cod_Contatto = Zoo_Animali.CF_Fornitore AND ")
                stb.AppendLine("                (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1)) Contatti_FornFatt")

                stb.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Contatti  (NOLOCK) ")
                stb.AppendLine("             WHERE Contatti.Cod_Contatto = Zoo_Animali.Fornitore_Provenienza AND ")
                stb.AppendLine("                (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1)) Contatti_FornProv")
            End If

            'Stalla svezzamento
            stb.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Contatti  (NOLOCK) ")
            stb.AppendLine("             WHERE Contatti.Cod_Contatto = Zoo_Animali.Stalla_Svezzamento AND ")
            stb.AppendLine("                (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1)) Contatti_StallaSvezz")

            stb.AppendLine(" WHERE 1=1 ")
            If filtraGiacenze1 Then
                Select Case bAll
                    Case False
                    'stb.AppendLine(" And Zoo_Animali.Validita_Inizio <= " & Agro_SQL_SaveDateTime(Data) & "   ")
                    'stb.AppendLine(" And Zoo_Animali.Validita_Fine >= " & Agro_SQL_SaveDateTime(Data) & "   ")
                    Case True
                        stb.AppendLine(" And CAST(Zoo_Animali.Validita_Inizio as Date) <= " & Agro_SQL_SaveDate(Data) & "   ")
                        stb.AppendLine(" And CAST(Zoo_Animali.Validita_Fine as Date) >= " & Agro_SQL_SaveDate(Data) & "   ")
                End Select
            End If

            If Piva <> "" Then
                stb.AppendLine(" And giac_cte.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And giac_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND giac_cte.Sta_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If

            If Raggruppamento_Cod <> 0 Then
                stb.AppendLine(" AND giac_cte.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "  ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND giac_cte.Cod_Animale = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If

            If Matricola <> "" Then
                stb.AppendLine(" AND Zoo_Animali.Matricola = '" & Agro_SQL_SaveText(Matricola) & "'  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            'stb.AppendLine("ORDER BY giac_cte.Impresa ")
            If LivelloCompatibilita(objParametri) >= 150 Then
                stb.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
            End If

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

            If listMatricola_Animali IsNot Nothing AndAlso listMatricola_Animali.Count > 0 AndAlso IDTestataTempMatricola <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTempMatricola, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Private Function CreaAgn_TT(Piva As String,
                                   Sa_Cod As Integer,
                                   STA_NUM As Integer,
                                   Cod_Animale As Integer,
                                   Data As DateTime,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal bAll As Boolean = False,
                                   Optional ByVal Filtro_Visibilita_Utente As Boolean = False,
                                   Optional ByVal listCod_Animali As List(Of Integer) = Nothing,
                                   Optional ByVal filtraGiacenze1 As Boolean = True,
                                   Optional ByVal IDTestataTemp As Integer = 0) As String

        Dim stb As New StringBuilder
        stb.AppendLine(" select a.Id_Agenda,  ")
        stb.AppendLine("	a.PIVA,  ")
        stb.AppendLine("	ISNULL(i.partitaIvaReale, a.PIVA) As partitaIvaReale,")
        stb.AppendLine("	md.Cod_Progetto, ")
        stb.AppendLine("	md.Lotto, ")
        stb.AppendLine("	md.Udm_Cod, ")
        stb.AppendLine("	UnitaMisura.Udm_Des, ")
        stb.AppendLine("	UnitaMisura.Udm_Sim, ")
        stb.AppendLine("	mdes.Sa_Cod, ")
        stb.AppendLine("	mdes.Id_Destinazione, ")
        stb.AppendLine("	mdes.Tipo_Destinazione, ")
        stb.AppendLine("	mdes.Qta, ")
        stb.AppendLine("	md.Id_Mov, ")
        stb.AppendLine("	md.Id_Mov_Det, ")
        stb.AppendLine("	m.Cau_Mov, ")
        stb.AppendLine("	i.rag_soc, ")
        stb.AppendLine("	ic.val_cod as cuaa, ")
        stb.AppendLine("	Centri_Aziendali.sa_nome ")
        stb.AppendLine("	INTO #agn_cte ")
        stb.AppendLine("	from agenda a (NOLOCK)  ")
        stb.AppendLine("	inner join imprese i (NOLOCK) on i.PIVA = a.PIVA ")
        stb.AppendLine("	inner join imprese_codici ic (NOLOCK) on i.piva = ic.piva and ic.id_cod = 1010 ")
        stb.AppendLine("	inner join Movimenti m (NOLOCK) on	a.Id_Agenda = m.Id_Agenda  ")
        stb.AppendLine("	inner join Movimenti_dettagli md (NOLOCK) on md.Id_Agenda = m.Id_Agenda  AND md.Id_Mov = m.Id_Mov  ")
        stb.AppendLine("	inner join mov_destinazioni mdes (NOLOCK) on mdes.Id_Agenda = md.Id_Agenda and mdes.Id_Mov = md.Id_Mov and mdes.Id_Mov_Det = md.Id_Mov_Det ")
        stb.AppendLine("	inner join Centri_Aziendali (NOLOCK) ON mdes.Piva = Centri_Aziendali.Piva and mdes.Sa_Cod=Centri_Aziendali.sa_cod  ")
        stb.AppendLine("	INNER Join UnitaMisura (NOLOCK) ON UnitaMisura.Udm_Cod = md.Udm_Cod ")
        If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
            stb.AppendLine("	INNER Join __Tmp_Agenda (NOLOCK) ON md.Cod_Progetto = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
        End If

        If Filtro_Visibilita_Utente Then
            stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON a.Piva = p.piva AND p.Sa_Cod = mdes.Sa_Cod  AND p.entita_Cod = 2 AND p.Appezza = 0 AND p.Id_Reg = 0 AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
        End If
        'If Filtro_Visibilita_Utente And Piva = "" Then
        '    stb.AppendLine("	inner join piva_visibili p (NOLOCK) ON a.Piva = p.piva  ")
        'End If
        stb.AppendLine("	where  1 = 1 ")
        If Piva <> "" Then
            stb.AppendLine(" And a.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
        End If

        If Sa_Cod <> 0 Then
            stb.AppendLine(" And mdes.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
        End If

        If Cod_Animale <> 0 Then
            stb.AppendLine(" AND md.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
        End If

        stb.AppendLine("	and a.lav_Cod >= 3000 And a.lav_cod < 4000 ")
        stb.AppendLine("	And m.Cau_Mov In ('7300', '7350', '3700', '3750')     ")
        stb.AppendLine("	And m.Data_Movimento >=  CONVERT(DateTime,'1900/01/01',120)     ")

        If filtraGiacenze1 Then
            Select Case bAll
                Case False
                    stb.AppendLine("	And m.Data_Movimento <=   " & Agro_SQL_SaveDateTime(Data) & "     ")
                Case True
                    stb.AppendLine("	And m.Data_Movimento <=   " & Agro_SQL_SaveDate(Data) & "     ")
            End Select
        End If

        stb.AppendLine("	And md.Elem_Cod = 300  ")
        stb.AppendLine("	And md.Jolly_Int = 0  ")
        stb.AppendLine("	And mdes.Tipo_Destinazione IN (15, 21)  ")
        Return stb.ToString
    End Function

    Private Function CreaAgn_TT_New(Piva As String,
                                   Sa_Cod As Integer,
                                   STA_NUM As Integer,
                                   Cod_Animale As Integer,
                                   Data As DateTime,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal bAll As Boolean = False,
                                   Optional ByVal Filtro_Visibilita_Utente As Boolean = False,
                                   Optional ByVal listCod_Animali As List(Of Integer) = Nothing,
                                   Optional ByVal filtraGiacenze1 As Boolean = True,
                                   Optional ByVal IDTestataTemp As Integer = 0) As String

        Dim stb As New StringBuilder
        stb.AppendLine(" select a.Id_Agenda,  ")
        stb.AppendLine("	a.PIVA,  ")
        stb.AppendLine("	ISNULL(i.partitaIvaReale, a.PIVA) as partitaIvaReale,  ")
        stb.AppendLine("	md.Cod_Progetto, ")
        stb.AppendLine("	Stalla.BDN_Codice_Azienda, ")
        stb.AppendLine("	Stalla.STA_DES, ")
        stb.AppendLine("	Stalla.STA_NUM, ")
        stb.AppendLine("	Stalla_Raggruppamenti.Raggruppamento_Des, ")
        stb.AppendLine("	Stalla_Raggruppamenti.Raggruppamento_Cod, ")
        stb.AppendLine("	md.Lotto, ")
        stb.AppendLine("	md.Udm_Cod, ")
        stb.AppendLine("	UnitaMisura.Udm_Des, ")
        stb.AppendLine("	UnitaMisura.Udm_Sim, ")
        stb.AppendLine("	mdes.Sa_Cod, ")
        stb.AppendLine("	mdes.Id_Destinazione, ")
        stb.AppendLine("	mdes.Tipo_Destinazione, ")
        stb.AppendLine("	mdes.Qta, ")
        stb.AppendLine("	md.Id_Mov, ")
        stb.AppendLine("	md.Id_Mov_Det, ")
        stb.AppendLine("	m.Cau_Mov, ")
        stb.AppendLine("	i.rag_soc, ")
        stb.AppendLine("	ic.val_cod as cuaa, ")
        stb.AppendLine("	Centri_Aziendali.sa_nome ")
        stb.AppendLine("	INTO #agn_cte ")
        stb.AppendLine("	from agenda a (NOLOCK)  ")
        stb.AppendLine("	inner join imprese i (NOLOCK) on i.PIVA = a.PIVA ")
        stb.AppendLine("	inner join imprese_codici ic (NOLOCK) on i.piva = ic.piva and ic.id_cod = 1010 ")
        stb.AppendLine("	inner join Movimenti m (NOLOCK) on	a.Id_Agenda = m.Id_Agenda  ")
        stb.AppendLine("	inner join Movimenti_dettagli md (NOLOCK) on md.Id_Agenda = m.Id_Agenda  AND md.Id_Mov = m.Id_Mov  ")
        stb.AppendLine("	inner join mov_destinazioni mdes (NOLOCK) on mdes.Id_Agenda = md.Id_Agenda and mdes.Id_Mov = md.Id_Mov and mdes.Id_Mov_Det = md.Id_Mov_Det ")
        stb.AppendLine(" INNER Join Stalla_Raggruppamenti (NOLOCK) ON mdes.Sa_Cod = Stalla_Raggruppamenti.sa_cod  ")
        stb.AppendLine("                                AND mdes.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod  ")
        stb.AppendLine("                                AND mdes.Tipo_Destinazione = 21  ")
        stb.AppendLine(" INNER JOIN Centri_Aziendali (NOLOCK) ON a.Piva = Centri_Aziendali.Piva and a.Sa_Cod=Centri_Aziendali.sa_cod  ")
        stb.AppendLine(" INNER JOIN Stalla (NOLOCK) ON Stalla.PIVA = a.Piva AND Stalla.sa_cod = a.SA_COD AND Stalla.STA_NUM = a.STA_NUM  ")
        stb.AppendLine("	INNER Join UnitaMisura (NOLOCK) ON UnitaMisura.Udm_Cod = md.Udm_Cod ")
        If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
            stb.AppendLine("	INNER Join __Tmp_Agenda (NOLOCK) ON md.Cod_Progetto = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
        End If

        If Filtro_Visibilita_Utente Then
            stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON a.Piva = p.piva AND p.Sa_Cod = mdes.Sa_Cod  AND p.entita_Cod = 2 AND p.Appezza = 0 AND p.Id_Reg = 0 AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
        End If
        'If Filtro_Visibilita_Utente And Piva = "" Then
        '    stb.AppendLine("	inner join piva_visibili p (NOLOCK) ON a.Piva = p.piva  ")
        'End If
        stb.AppendLine("	where  1 = 1 ")
        If Piva <> "" Then
            stb.AppendLine(" And a.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
        End If

        If Sa_Cod <> 0 Then
            stb.AppendLine(" And a.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
        End If

        If STA_NUM <> 0 Then
            stb.AppendLine(" And a.STA_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "      ")
        End If

        If Cod_Animale <> 0 Then
            stb.AppendLine(" AND md.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
        End If

        stb.AppendLine("	and a.lav_Cod >= 3000 And a.lav_cod < 4000 ")
        stb.AppendLine("	And m.Cau_Mov In ('3700', '3750')     ")

        If filtraGiacenze1 Then
            Select Case bAll
                Case False
                    stb.AppendLine("	And m.Data_Movimento <=   " & Agro_SQL_SaveDateTime(Data) & "     ")
                Case True
                    stb.AppendLine("	And m.Data_Movimento <=   " & Agro_SQL_SaveDate(Data) & "     ")
            End Select
        End If

        stb.AppendLine("	And md.Elem_Cod = 300  ")
        stb.AppendLine("	And md.Jolly_Int = 0  ")
        stb.AppendLine("	And mdes.Tipo_Destinazione IN (15, 21)  ")
        Return stb.ToString
    End Function

    Private Function CreaGiac_TT(Piva As String,
                                   Sa_Cod As Integer,
                                   STA_NUM As Integer,
                                   Optional ByVal filtraGiacenze1 As Boolean = True,
                                   Optional ByVal mostraAnomalie As Boolean = False) As String

        Dim stb As New StringBuilder

        stb.AppendLine("	SELECT   ")
        stb.AppendLine("  agn_cte.Piva,  ")
        stb.AppendLine("  agn_cte.partitaIvaReale,  ")
        stb.AppendLine("  agn_cte.Rag_Soc As Impresa,  ")
        stb.AppendLine("  agn_cte.cuaa, Stalla.BDN_Codice_Azienda,  ")
        stb.AppendLine("  agn_cte.Cod_Progetto AS Cod_Animale,  ")
        stb.AppendLine("  agn_cte.Lotto,  ")
        stb.AppendLine("  agn_cte.Udm_Cod,  ")
        stb.AppendLine("  agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
        stb.AppendLine("  agn_cte.Sa_Cod,  ")
        stb.AppendLine("  agn_cte.sa_nome,  ")
        stb.AppendLine("  agn_cte.Id_Destinazione,  ")
        stb.AppendLine("  agn_cte.Tipo_Destinazione,  ")
        stb.AppendLine("  COALESCE(Fabbricati.Fabbricato_Des, '') As STA_DES,  ")
        stb.AppendLine("	COALESCE(Fabbricati.Fabbricato_Cod, 0) As STA_NUM,  ")
        stb.AppendLine("  COALESCE(Stalla_Raggruppamenti.Raggruppamento_Des, '') as Raggruppamento_Des,  ")
        stb.AppendLine("  COALESCE(Stalla_Raggruppamenti.Raggruppamento_Cod, 0) as Raggruppamento_Cod,  ")
        stb.AppendLine("  Convert(INTEGER, SUM( Case When agn_cte.CAU_MOV In ('7350','3750')              ")
        stb.AppendLine("          THEN -(agn_cte.qta)             ")
        stb.AppendLine("          Else agn_cte.qta             ")
        stb.AppendLine("          End)) As Giacenza   ")
        stb.AppendLine(" INTO #giac_cte ")
        stb.AppendLine(" From #agn_cte agn_cte  ")
        stb.AppendLine(" INNER Join Stalla_Raggruppamenti (NOLOCK) ON agn_cte.Sa_Cod = Stalla_Raggruppamenti.sa_cod  ")
        stb.AppendLine("                                AND agn_cte.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod  ")
        stb.AppendLine("                                AND agn_cte.Tipo_Destinazione = 21  ")
        stb.AppendLine(" INNER JOIN Fabbricati (NOLOCK) ON Stalla_Raggruppamenti.PIVA = Fabbricati.Piva AND Stalla_Raggruppamenti.sa_cod = Fabbricati.SA_COD AND Stalla_Raggruppamenti.STA_NUM = Fabbricati.Fabbricato_Cod ")
        stb.AppendLine(" INNER JOIN Stalla (NOLOCK) ON Stalla.PIVA = Fabbricati.Piva AND Stalla.sa_cod = Fabbricati.SA_COD AND Stalla.STA_NUM = Fabbricati.Fabbricato_Cod  ")
        If mostraAnomalie Then
            'stb.AppendLine(" LEFT Join Zoo_AnimalixAnomalie (NOLOCK) ON Zoo_AnimalixAnomalie.Cod_Animale = agn_cte.Cod_Progetto  ")
        End If
        stb.AppendLine("  where 1=1  ")
        If Sa_Cod <> 0 Then
            stb.AppendLine(" And agn_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
        End If

        If STA_NUM <> 0 Then
            stb.AppendLine(" AND Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
        End If
        stb.AppendLine(" GROUP BY agn_cte.Piva, agn_cte.partitaIvaReale, agn_cte.Rag_Soc, agn_cte.cuaa, Stalla.BDN_Codice_Azienda, ")
        stb.AppendLine(" agn_cte.Cod_Progetto,  ")
        stb.AppendLine(" agn_cte.Lotto,  ")
        stb.AppendLine(" agn_cte.Udm_Cod, ")
        stb.AppendLine(" Fabbricati.Fabbricato_Des, ")
        stb.AppendLine(" Fabbricati.Fabbricato_Cod, ")
        stb.AppendLine(" agn_cte.Sa_Cod,  ")
        stb.AppendLine(" agn_cte.sa_nome, ")
        stb.AppendLine(" agn_cte.Id_Destinazione,  ")
        stb.AppendLine(" agn_cte.Tipo_Destinazione,  ")
        stb.AppendLine(" agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
        stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Des,  ")
        stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Cod  ")
        If filtraGiacenze1 Then
            stb.AppendLine(" HAVING Convert(INTEGER, SUM(Case When agn_cte.Cau_Mov In ('7350','3750') THEN -(agn_cte.qta) ELSE agn_cte.qta END)) <> 0   ")
        End If

        Return stb.ToString
    End Function

    Private Function CreaGiac_TT_New(Piva As String,
                                   Sa_Cod As Integer,
                                   STA_NUM As Integer,
                                   Optional ByVal filtraGiacenze1 As Boolean = True,
                                   Optional ByVal mostraAnomalie As Boolean = False) As String

        Dim stb As New StringBuilder

        stb.AppendLine("	SELECT   ")
        stb.AppendLine("  agn_cte.Piva,   ")
        stb.AppendLine("  agn_cte.partitaIvaReale,  ")
        stb.AppendLine("  agn_cte.Rag_Soc As Impresa,  ")
        stb.AppendLine("  agn_cte.cuaa, agn_cte.BDN_Codice_Azienda,  ")
        stb.AppendLine("  agn_cte.Cod_Progetto AS Cod_Animale,  ")
        stb.AppendLine("  agn_cte.Lotto,  ")
        stb.AppendLine("  agn_cte.Udm_Cod,  ")
        stb.AppendLine("  agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
        stb.AppendLine("  agn_cte.Sa_Cod,  ")
        stb.AppendLine("  agn_cte.sa_nome,  ")
        stb.AppendLine("  agn_cte.Id_Destinazione,  ")
        stb.AppendLine("  agn_cte.Tipo_Destinazione,  ")
        stb.AppendLine("  COALESCE(agn_cte.STA_DES, '') As STA_DES,  ")
        stb.AppendLine("  COALESCE(agn_cte.STA_NUM, 0) As STA_NUM,  ")
        stb.AppendLine("  COALESCE(agn_cte.Raggruppamento_Des, '') as Raggruppamento_Des,  ")
        stb.AppendLine("  COALESCE(agn_cte.Raggruppamento_Cod, 0) as Raggruppamento_Cod,  ")
        stb.AppendLine("  Convert(INTEGER, SUM( Case When agn_cte.CAU_MOV In ('7350','3750')              ")
        stb.AppendLine("          THEN -(agn_cte.qta)             ")
        stb.AppendLine("          Else agn_cte.qta             ")
        stb.AppendLine("          End)) As Giacenza   ")
        stb.AppendLine(" INTO #giac_cte ")
        stb.AppendLine(" From #agn_cte agn_cte  ")
        If mostraAnomalie Then
            'stb.AppendLine(" LEFT Join Zoo_AnimalixAnomalie (NOLOCK) ON Zoo_AnimalixAnomalie.Cod_Animale = agn_cte.Cod_Progetto  ")
        End If
        stb.AppendLine("  where 1=1  ")
        stb.AppendLine(" GROUP BY agn_cte.Piva, agn_cte.partitaIvaReale, agn_cte.Rag_Soc, agn_cte.cuaa, agn_cte.BDN_Codice_Azienda, ")
        stb.AppendLine(" agn_cte.Cod_Progetto,  ")
        stb.AppendLine(" agn_cte.Lotto,  ")
        stb.AppendLine(" agn_cte.Udm_Cod, ")
        stb.AppendLine(" agn_cte.STA_DES, ")
        stb.AppendLine(" agn_cte.STA_NUM, ")
        stb.AppendLine(" agn_cte.Sa_Cod,  ")
        stb.AppendLine(" agn_cte.sa_nome, ")
        stb.AppendLine(" agn_cte.Id_Destinazione,  ")
        stb.AppendLine(" agn_cte.Tipo_Destinazione,  ")
        stb.AppendLine(" agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
        stb.AppendLine(" agn_cte.Raggruppamento_Des,  ")
        stb.AppendLine(" agn_cte.Raggruppamento_Cod  ")
        If filtraGiacenze1 Then
            stb.AppendLine(" HAVING Convert(INTEGER, SUM(Case When agn_cte.Cau_Mov In ('7350','3750') THEN -(agn_cte.qta) ELSE agn_cte.qta END)) <> 0   ")
        End If

        Return stb.ToString
    End Function

    Private Function CreaZooMatricoleList_TT() As String

        Dim stb As New StringBuilder
        stb.AppendLine("  	SELECT Zoo_Animali.Matricola  ")
        stb.AppendLine("  	INTO #ZooMatricoleList  ")
        stb.AppendLine("  	FROM #giac_cte giac_cte   ")
        stb.AppendLine("  	INNER Join Zoo_Animali (NOLOCK) ON Zoo_Animali.Cod_Progetto = giac_cte.Cod_Animale    ")
        stb.AppendLine("  	WHERE 1=1   ")
        Return stb.ToString
    End Function

    Private Function CreaDataPrimoCaricamento_TT(Piva As String) As String
        'DataPrimoCaricamento
        Dim stb As New StringBuilder
        stb.AppendLine("  SELECT ZooMatricoleList.Matricola, MIN(Movimenti.Data_Movimento) as Data_Movimento  ")
        stb.AppendLine("  INTO #DataPrimoCaricamento  ")
        stb.AppendLine("  FROM Agenda (NOLOCK)  ")
        stb.AppendLine("  JOIN Movimenti (NOLOCK) ON Agenda.ID_Agenda = Movimenti.ID_Agenda  ")
        stb.AppendLine("  JOIN Movimenti_Dettagli (NOLOCK) ON Movimenti.ID_Agenda = Movimenti_Dettagli.ID_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov AND Movimenti_dettagli.Elem_Cod = 300  ")
        stb.AppendLine("  JOIN Zoo_Animali (NOLOCK) ON Movimenti_Dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto  ")
        stb.AppendLine("  JOIN #ZooMatricoleList ZooMatricoleList  ON Zoo_Animali.Matricola = ZooMatricoleList.Matricola  ")
        stb.AppendLine("  WHERE 1 = 1  ")
        If Piva <> "" Then
            stb.AppendLine(" And Agenda.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
        End If
        stb.AppendLine("  GROUP BY ZooMatricoleList.Matricola  ")
        Return stb.ToString
    End Function

    Private Function CreaPesate_TT(Data As DateTime, Optional ByVal bAll As Boolean = False, Optional ByVal leggiUltimaPesata As Boolean = False) As String
        Dim stb As New StringBuilder
        stb.AppendLine(" 	SELECT Movimenti_dettagli.Cod_Progetto, MAX(Data_Movimento) as Data_Peso_Max, MIN(Data_Movimento) as Data_Peso_Min  ")
        stb.AppendLine("    INTO #pesate_cte")
        stb.AppendLine(" 	FROM Movimenti (NOLOCK)  ")
        stb.AppendLine(" 	JOIN Movimenti_dettagli (NOLOCK)  ON Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
        stb.AppendLine(" 	JOIN #giac_cte giac_cte ON Movimenti_dettagli.Cod_Progetto = giac_cte.Cod_Animale ")
        stb.AppendLine(" 	WHERE Movimenti.Cau_Mov = '" & CAU_PESATURA_ANIMALI & "' ") '" AND Agenda.Lav_Cod = " & LAVCOD_PESATURA_ANIMALI & " ")
        If Not leggiUltimaPesata Then
            stb.AppendLine("	AND CAST(Data_Movimento as Date) <=   " & Agro_SQL_SaveDate(Data) & " ")
        End If
        stb.AppendLine("    GROUP BY Movimenti_dettagli.Cod_Progetto ")
        Return stb.ToString
    End Function

    Private Function CreaPesate_CaricoScarico_TT()
        Dim stb As New StringBuilder

        stb.AppendLine("    SELECT Movimenti.Id_Agenda, Movimenti_dettagli.Cod_Progetto, Movimenti_dettagli.Qta AS Peso_Pagato ").
            AppendLine("    INTO #pesate_cs_cte ").
            AppendLine("    FROM Movimenti (NOLOCK) ").
            AppendLine("    INNER JOIN Movimenti_dettagli (NOLOCK) ON Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ").
            AppendLine($"    WHERE Movimenti.Cau_Mov = '{CAU_PESATURA_ANIMALI}' ")

        Return stb.ToString
    End Function

    Private Function CreaMaxPesate_TT() As String
        Dim stb As New StringBuilder
        stb.AppendLine(" 	SELECT IIF(Movimenti_dettagli.Qta_Dettaglio1 = 0, Qta, Qta_Dettaglio1) AS Qta,  Movimenti_dettagli.Cod_Progetto, Movimenti.Data_Movimento as Data_Peso ")
        stb.AppendLine(" 	INTO #max_pesate_ct  ")
        stb.AppendLine(" 	FROM Movimenti (NOLOCK)  ")
        stb.AppendLine(" 	JOIN Movimenti_dettagli (NOLOCK)  ON Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov   ")
        stb.AppendLine(" 	JOIN #pesate_cte pesate_cte ON pesate_cte.Cod_Progetto = Movimenti_dettagli.Cod_Progetto AND Movimenti.Data_Movimento = pesate_cte.Data_Peso_Max   ")
        stb.AppendLine(" 	WHERE Movimenti.Cau_Mov = '" & CAU_PESATURA_ANIMALI & "' ")
        Return stb.ToString
    End Function

    Private Function CreaMinPesate_TT() As String
        Dim stb As New StringBuilder
        stb.AppendLine(" 	SELECT IIF(Movimenti_dettagli.Qta_Dettaglio1 = 0, Qta, Qta_Dettaglio1) AS Qta, Movimenti_dettagli.Cod_Progetto, Movimenti.Data_Movimento as Data_Peso ")
        stb.AppendLine(" 	INTO #min_pesate_ct  ")
        stb.AppendLine(" 	FROM Movimenti (NOLOCK)  ")
        stb.AppendLine(" 	JOIN Movimenti_dettagli (NOLOCK)  ON Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov   ")
        stb.AppendLine(" 	JOIN #pesate_cte pesate_cte ON pesate_cte.Cod_Progetto = Movimenti_dettagli.Cod_Progetto AND Movimenti.Data_Movimento = pesate_cte.Data_Peso_Min   ")
        stb.AppendLine(" 	WHERE Movimenti.Cau_Mov = '" & CAU_PESATURA_ANIMALI & "' ")
        Return stb.ToString
    End Function

    Private Function CreaAnomalie_TT() As String
        Dim stb As New StringBuilder
        stb.AppendLine(" 	SELECT giac_cte.cod_animale, COALESCE(STRING_AGG(Zoo_AnimalixAnomalie.Codice, ','), '') as Anomalie, COALESCE(STRING_AGG(Zoo_Animali_Anomalie.descrizione, ','), '') as Anomalie_Str   ")
        stb.AppendLine(" 	INTO #anomalie_ct  ")
        stb.AppendLine(" 	FROM Zoo_AnimalixAnomalie (NOLOCK)  ")
        stb.AppendLine(" 	JOIN #giac_cte giac_cte ON Zoo_AnimalixAnomalie.Cod_Animale = giac_cte.Cod_Animale   ")
        stb.AppendLine(" 	JOIN Zoo_Animali_Anomalie (NOLOCK)  ON Zoo_AnimalixAnomalie.Codice = Zoo_Animali_Anomalie.Codice   ")
        stb.AppendLine(" 	GROUP BY giac_cte.cod_animale   ")
        Return stb.ToString
    End Function


    Public Function Leggi_Giacenze(Piva As String,
                                   Sa_Cod As Integer,
                                   STA_NUM As Integer,
                                   Raggruppamento_Cod As Integer,
                                   Cod_Animale As Integer,
                                   Data As DateTime,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal bAll As Boolean = False,
                                   Optional ByVal Filtro_Visibilita_Utente As Boolean = False,
                                   Optional ByVal listCod_Animali As List(Of Integer) = Nothing,
                                   Optional ByVal filtraGiacenze1 As Boolean = True,
                                   Optional ByVal filtraFornitori As Boolean = False,
                                   Optional ByVal mostraPesate As Boolean = False,
                                   Optional ByVal mostraAnomalie As Boolean = False,
                                   Optional xFiltroAggiuntivo As String = "",
                                   Optional ByVal Matricola As String = "",
                                   Optional ByVal MostraGGPrimoCaricamento As Boolean = False,
                                   Optional ByVal listMatricola_Animali As List(Of String) = Nothing,
                                   Optional ByVal leggiUltimaPesata As Boolean = False,
                                   Optional ByVal CFproprietario As String = "") As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_Animali.Leggi_Giacenze()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            Dim IDTestataTemp As Integer = 0
            Dim IDTestataTempMatricola As Integer = 0
            'CancellaRecordDaIDTestataTemp

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                For Each codAnimale In listCod_Animali
                    objTmp_AgendaTemp.Scrivi(IDTestataTemp, "", codAnimale, 0, objParametri)
                Next
            End If
            If listMatricola_Animali IsNot Nothing AndAlso listMatricola_Animali.Count > 0 Then
                For Each mat In listMatricola_Animali
                    objTmp_AgendaTemp.Scrivi(IDTestataTempMatricola, mat, 0, 0, objParametri)
                Next
            End If

            stb.Length = 0
            Dim newQuery As Boolean = False
            Dim agn_TT = ""
            If newQuery Then
                agn_TT = CreaAgn_TT_New(Piva, Sa_Cod, STA_NUM, Cod_Animale, Data, objParametri, bAll, Filtro_Visibilita_Utente, listCod_Animali, filtraGiacenze1, IDTestataTemp)
            Else
                agn_TT = CreaAgn_TT(Piva, Sa_Cod, STA_NUM, Cod_Animale, Data, objParametri, bAll, Filtro_Visibilita_Utente, listCod_Animali, filtraGiacenze1, IDTestataTemp)
            End If
            stb.AppendLine(agn_TT)

            Dim giac_TT = ""
            If newQuery Then
                giac_TT = CreaGiac_TT_New(Piva, Sa_Cod, STA_NUM, filtraGiacenze1, mostraAnomalie)
            Else
                giac_TT = CreaGiac_TT(Piva, Sa_Cod, STA_NUM, filtraGiacenze1, mostraAnomalie)
            End If
            stb.AppendLine(giac_TT)

            If MostraGGPrimoCaricamento Then
                Dim ZooMatricoleList_TT = CreaZooMatricoleList_TT()
                stb.AppendLine(ZooMatricoleList_TT)
                Dim DataPirmoCaricamento_TT = CreaDataPrimoCaricamento_TT(Piva)
                stb.AppendLine(DataPirmoCaricamento_TT)
            End If

            If mostraPesate Then
                Dim pesate_TT As String = CreaPesate_TT(Data, bAll, leggiUltimaPesata)
                stb.AppendLine(pesate_TT)
                Dim maxPesate_TT As String = CreaMaxPesate_TT()
                stb.AppendLine(maxPesate_TT)
                Dim minPesate_TT As String = CreaMinPesate_TT()
                stb.AppendLine(minPesate_TT)
            End If

            If mostraAnomalie Then
                Dim anomalie_TT As String = CreaAnomalie_TT()
                stb.AppendLine(anomalie_TT)
            End If

            stb.AppendLine("SELECT giac_cte.Piva + '_' + CAST(giac_cte.Sa_Cod as varchar(100)) + '_' + CAST(giac_cte.Cod_Animale as varchar(100)) as chiave,  ")
            stb.AppendLine("giac_cte.*,  ")

            stb.AppendLine("Lista_Specie_Animali.SPE_DES,  ")
            stb.AppendLine("Lista_Razze_Animali.RAZ_DES,  ")
            stb.AppendLine("Zoo_Animali_Lista_Tipi.Tipo_Des,  ")
            stb.AppendLine("Lista_IndirizziProd_Animali.IPRO_DES,  ")
            If filtraGiacenze1 Then
                stb.AppendLine("Zoo_Animali_Distinte.Cod_Progetto,  ")
                stb.AppendLine("Zoo_Animali_Distinte.Progetto_Des,  ")
                stb.AppendLine("Zoo_Animali_Distinte.Progetto_Nome,  ")
                stb.AppendLine("Zoo_Animali_Distinte.Codice_Distinta,  ")
                stb.AppendLine("Zoo_AnimalixStati_Accrescimento.Stato_Cod,  ")
                stb.AppendLine("Zoo_Animali_Lista_Stati_Accrescimento.Stato_Des,  ")
            End If

            If filtraFornitori Then
                stb.AppendLine("COALESCE(Contatti_FornFatt.Cod_Contatto, '') AS CF_FornFatt,  ")
                stb.AppendLine("COALESCE(Contatti_FornFatt.Rag_Soc + Contatti_FornFatt.cognome + ' ' + Contatti_FornFatt.Nome, '') AS RagSoc_FornFatt,  ")

                stb.AppendLine("COALESCE(Contatti_FornProv.Cod_Contatto, '') AS CF_FornProv,  ")
                stb.AppendLine("COALESCE(Contatti_FornProv.Rag_Soc + Contatti_FornProv.cognome + ' ' + Contatti_FornProv.Nome, '') AS RagSoc_FornProv,  ")
            End If

            stb.AppendLine(" COALESCE(Contatti.Rag_Soc, '') as Rag_Soc,  ")
            stb.AppendLine(" COALESCE(Contatti.Cod_Contatto, '') as Cod_Contatto, ")
            stb.AppendLine(" Zoo_Animali.RAZ_COD,  ")
            stb.AppendLine(" 	Zoo_Animali.IPRO_COD,  ")
            stb.AppendLine(" 	COALESCE(Zoo_Animali.CF_Fornitore, '') as CF_Fornitore,  ")
            stb.AppendLine("   Zoo_Animali.Mat_Madre,   ")
            stb.AppendLine("   Zoo_Animali.Mat_Padre,   ")
            stb.AppendLine("   CASE Zoo_Animali.Metodo_Produzione WHEN 1 THEN '" &
                           Agro_SQL_SaveText(Gias.Integrato) & "' WHEN 2 THEN '" &
                           Agro_SQL_SaveText(Gias.InConversione) & "' WHEN 3 THEN '" &
                           Agro_SQL_SaveText(Gias.Biologico) & "' END as Metodo_Produzione,   ")
            stb.AppendLine("   Zoo_Animali.Razza_Padre AS RazCod_Padre,   ")
            stb.AppendLine("   razza_madre.RAZ_DES as RazDes_Madre,   ")
            stb.AppendLine("   Zoo_Animali.Razza_Madre AS RazCod_Madre,   ")
            stb.AppendLine("   razza_padre.RAZ_DES as RazDes_Padre,   ")
            stb.AppendLine("   Zoo_Animali.Lotto_Fornitore, ")
            stb.AppendLine("   Zoo_Animali.Matricola,   ")
            stb.AppendLine("   Zoo_Animali.Tag,   ")
            stb.AppendLine("   RIGHT(Zoo_Animali.Matricola, 5) AS Matricola_Breve,   ")
            stb.AppendLine("   RIGHT(Zoo_Animali.Matricola, 4) AS Matricola_Breve4,   ")
            stb.AppendLine("   Zoo_Animali.GEN_COD,   ")
            stb.AppendLine("   Zoo_Animali.SPE_COD,   ")
            stb.AppendLine("   Zoo_Animali.TIPO_COD,   ")
            stb.AppendLine("   Zoo_Animali.Progetto,   ")
            stb.AppendLine("   CAST(Zoo_Animali.Validita_Inizio as date) as Validita_Inizio,   ")
            stb.AppendLine("   CAST(Zoo_Animali.Validita_Fine as date) as Validita_Fine,   ")
            stb.AppendLine("   Zoo_Animali.Nome,   ")
            stb.AppendLine("   Zoo_Animali.Sesso,   ")
            stb.AppendLine("   CASE WHEN Zoo_Animali.Validato = 1 THEN '" & Gias.Si & "' ELSE '" & Gias.No & "' END AS Validato,   ")
            stb.AppendLine("   Zoo_Animali.Modello4_Ingresso,   ")
            stb.AppendLine("   Zoo_Animali.Modello4_Uscita,   ")
            stb.AppendLine("   ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ZOO_Animali.Username_Creazione), ZOO_Animali.Username_Creazione) AS Utente_Creazione,   ")
            stb.AppendLine("   ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ZOO_Animali.Username_Modifica), ZOO_Animali.Username_Modifica) AS Utente_Modifica,   ")
            stb.AppendLine("   Zoo_Animali.Data_Creazione,   ")
            stb.AppendLine("   Zoo_Animali.Data_Modifica,   ")
            stb.AppendLine("   Zoo_Animali.Dat_Nascita,   ")
            stb.AppendLine("   Zoo_Animali.Id_Capo_BDN,   ")
            stb.AppendLine("   Zoo_Animali.CF_PROPRIETARIO,   ")
            stb.AppendLine("   Zoo_Animali.CF_DETENTORE,   ")
            stb.AppendLine("   COALESCE(Zoo_Animali.AUSL_AZI_NASCITA, '') as AUSL_AZI_NASCITA,   ")
            stb.AppendLine("   CASE WHEN Zoo_Animali.Id_Capo_BDN = 0 THEN '" & Gias.No & "' ELSE '" & Gias.Si & "' END AS FlagBDN,   ")
            stb.AppendLine("   Zoo_Animali.Certificato  ")

            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Ingresso_Numero, '') as Modello4_Ingresso_Numero  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Uscita_Numero, '') as Modello4_Uscita_Numero ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Ingresso_Prenotazione, '') as Modello4_Ingresso_Prenotazione  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Uscita_Prenotazione, '') as Modello4_Uscita_Prenotazione  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Codice_Azienda_Uscita, '') as Codice_Azienda_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_Documento_Ingresso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_Documento_Ingresso  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_Documento_Uscita, CONVERT(datetime, '2100-12-31 00:00:00.000', 120)) as Data_Documento_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Fornitore_Provenienza, '') as Fornitore_Provenienza  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.N_Bolla_Fornitore, '') as N_Bolla_Fornitore  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.N_Bolla_Uscita, '') as N_Bolla_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_DDT_Ingresso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_DDT_Ingresso  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_DDT_Uscita, CONVERT(datetime, '2100-12-31 00:00:00.000', 120)) as Data_DDT_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Codice_Azienda_Fornitore, '') as Codice_Azienda_Fornitore  ")
            stb.AppendLine("   , COALESCE(Contatti_StallaSvezz.Cod_Contatto, '') AS CF_StallaSvezz  ")
            stb.AppendLine("   , COALESCE(Contatti_StallaSvezz.Rag_Soc + Contatti_StallaSvezz.cognome + ' ' + Contatti_StallaSvezz.Nome, '') AS RagSoc_StallaSvezz  ")
            stb.AppendLine("   , Zoo_Animali.Stalla_Svezzamento  ")
            stb.AppendLine("   , Zoo_Animali.Note ")
            stb.AppendLine("   , Zoo_Animali.Anomalie_Note ")
            stb.AppendLine("   , Zoo_Animali.Id_Patologia AS Patologia_Cod ")
            stb.AppendLine("   , COALESCE(Lista_Patologie.Patologia_Des, '') AS Patologia_Des ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Incremento_Teorico, 0) AS Incremento_Teorico ")
            If MostraGGPrimoCaricamento Then
                stb.AppendLine("   , DATEDIFF(day, DataPrimoCaricamento.Data_Movimento,   " & Agro_SQL_SaveDate(Data, False) & "  ) as giorni_stalla_primo_caricamento ")
                stb.AppendLine("   , DataPrimoCaricamento.Data_Movimento as data_primo_caricamento ")
            End If
            If filtraGiacenze1 Then
                stb.AppendLine("  ,  DATEDIFF(day, Zoo_Animali.Validita_Inizio, " & Agro_SQL_SaveDate(Data, False) & ") + 1 as giorni_in_stalla ")
                stb.AppendLine("   , DATEDIFF(day, Zoo_Animali.DAT_NASCITA, " & Agro_SQL_SaveDate(Data, False) & ") As Eta_Giorni_TOTALI ")
                stb.AppendLine("   , IIF(DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") , Zoo_Animali.Dat_Nascita), " & Agro_SQL_SaveDate(Data, False) & ") >= 0,  ")
                stb.AppendLine("      DATEDIFF(month, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") ")
                stb.AppendLine("   , DATEDIFF(month, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") -1)  as eta_mesi ")
                stb.AppendLine("   , IIF(DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") , Zoo_Animali.Dat_Nascita), " & Agro_SQL_SaveDate(Data, False) & ") >= 0 ")
                stb.AppendLine("   , DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") , Zoo_Animali.Dat_Nascita), " & Agro_SQL_SaveDate(Data, False) & ") ")
                stb.AppendLine("   , DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") - 1, Zoo_Animali.Dat_Nascita), " & Agro_SQL_SaveDate(Data, False) & ")) as eta_giorni ")
            End If
            If mostraPesate Then
                stb.AppendLine("  , COALESCE(min_pesate_ct.Data_Peso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_Prima_Pesata ")
                stb.AppendLine("  , ROUND(min_pesate_ct.Qta, 2) as Qta_Prima_Pesata ")
                stb.AppendLine("  , COALESCE(max_pesate_ct.Data_Peso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_Ultima_Pesata ")
                stb.AppendLine("  , ROUND(max_pesate_ct.Qta, 2) as Qta_Ultima_Pesata ")
                stb.AppendLine("  , COALESCE(ROUND(((DATEDIFF(day, COALESCE(min_pesate_ct.Data_Peso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)), " & Agro_SQL_SaveDate(Data, False) & ")) * Zoo_Animali.Incremento_Teorico) + min_pesate_ct.Qta, 2), 0) AS Incremento_Teorico_Calcolato ")
            End If
            If mostraAnomalie Then
                stb.AppendLine("   , COALESCE(anomalie_ct.Anomalie, '') as Anomalie ")
                stb.AppendLine("   , COALESCE(anomalie_ct.Anomalie_Str, '') as Anomalie_Str ")
                '
            End If
            stb.AppendLine("   , COALESCE(Zoo_Animali.Anomalie_Note, '') as Anomalie_Note ")
            stb.AppendLine("FROM #giac_cte giac_cte ")
            stb.AppendLine(" INNER Join Zoo_Animali (NOLOCK) ON Zoo_Animali.Cod_Progetto = giac_cte.Cod_Animale  ")
            If listMatricola_Animali IsNot Nothing AndAlso listMatricola_Animali.Count > 0 Then
                stb.AppendLine("	INNER Join __Tmp_Agenda tmp_agenda (NOLOCK) ON Zoo_Animali.Matricola = tmp_agenda.Piva AND tmp_agenda.id_agenda = 0 AND tmp_agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTempMatricola) & " ")
            End If

            If MostraGGPrimoCaricamento Then
                stb.AppendLine(" INNER JOIN #DataPrimoCaricamento DataPrimoCaricamento ON Zoo_Animali.Matricola = DataPrimoCaricamento.Matricola ")
            End If
            If filtraGiacenze1 Then
                stb.AppendLine(" INNER JOIN Zoo_Animali_Distinte (NOLOCK) ON Zoo_Animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale  ")
                Select Case bAll
                    Case False
                        stb.AppendLine("                                         AND CAST(Zoo_Animali_Distinte.Validita_Inizio as date) <=    " & Agro_SQL_SaveDateTime(Data) & " ")
                        stb.AppendLine("                                         And CAST(Zoo_Animali_Distinte.Validita_Fine as datetime) >=    " & Agro_SQL_SaveDateTime(Data) & " ")
                    Case True
                        stb.AppendLine("                                         AND CAST(Zoo_Animali_Distinte.Validita_Inizio as date) <=    " & Agro_SQL_SaveDate(Data) & " ")
                        stb.AppendLine("                                         And CAST(Zoo_Animali_Distinte.Validita_Fine as date) >=    " & Agro_SQL_SaveDate(Data) & " ")
                End Select
            End If
            If filtraGiacenze1 Then
                stb.AppendLine(" INNER JOIN Zoo_AnimalixStati_Accrescimento (NOLOCK)  ON Zoo_AnimalixStati_Accrescimento.Cod_Progetto  = Zoo_Animali.Cod_Progetto  ")
                'Select Case bAll
                '    Case False
                '        stb.AppendLine("                                           AND CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as datetime) <=    " & Agro_SQL_SaveDateTime(Data) & " ")
                '        stb.AppendLine("                                           And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as datetime) >=   " & Agro_SQL_SaveDateTime(Data) & " ")
                '    Case True
                '        stb.AppendLine("                                           AND CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as date) <=    " & Agro_SQL_SaveDate(Data) & " ")
                '        stb.AppendLine("                                           And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as date) >=   " & Agro_SQL_SaveDate(Data) & " ")
                'End Select
                stb.AppendLine("                                           AND Zoo_AnimalixStati_Accrescimento.PIVA  = Zoo_Animali.PIVA ")
                stb.AppendLine("                                           AND Zoo_AnimalixStati_Accrescimento.sa_cod  = Zoo_Animali.sa_cod ")
                stb.AppendLine("                                           AND CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as date) <=    " & Agro_SQL_SaveDate(Data) & " ")
                stb.AppendLine("                                           And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as date) >=   " & Agro_SQL_SaveDate(Data) & " ")

            End If
            If filtraGiacenze1 Then
                stb.AppendLine("INNER JOIN Zoo_Animali_Lista_Stati_Accrescimento (NOLOCK) ON Zoo_AnimalixStati_Accrescimento.GEN_COD = Zoo_Animali_Lista_Stati_Accrescimento.GEN_COD  ")
                stb.AppendLine("												AND Zoo_AnimalixStati_Accrescimento.SPE_COD = Zoo_Animali_Lista_Stati_Accrescimento.SPE_COD  ")
                stb.AppendLine("												AND Zoo_AnimalixStati_Accrescimento.STATO_COD = Zoo_Animali_Lista_Stati_Accrescimento.STATO_COD  ")
                stb.AppendLine("												AND Zoo_AnimalixStati_Accrescimento.TIPO_COD = Zoo_Animali_Lista_Stati_Accrescimento.TIPO_COD  ")
            End If
            stb.AppendLine("INNER JOIN Lista_Specie_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Specie_Animali.SPE_COD  ")
            stb.AppendLine("INNER JOIN Lista_Razze_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD And Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD  ")
            stb.AppendLine(" INNER JOIN Lista_Razze_Animali razza_madre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_madre.GEN_COD And Zoo_Animali.SPE_COD = razza_madre.SPE_COD And Zoo_Animali.Razza_Madre = razza_madre.RAZ_COD   ")
            stb.AppendLine(" INNER JOIN Lista_Razze_Animali razza_padre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_padre.GEN_COD And Zoo_Animali.SPE_COD = razza_padre.SPE_COD And Zoo_Animali.Razza_Padre = razza_padre.RAZ_COD   ")
            stb.AppendLine("INNER JOIN Zoo_Animali_Lista_Tipi (NOLOCK) ON Zoo_Animali.GEN_COD = Zoo_Animali_Lista_Tipi.GEN_COD And Zoo_Animali.SPE_COD = Zoo_Animali_Lista_Tipi.SPE_COD And Zoo_Animali.TIPO_COD = Zoo_Animali_Lista_Tipi.TIPO_COD  ")
            stb.AppendLine("INNER JOIN Lista_IndirizziProd_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD And Zoo_Animali.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD  ")
            If mostraPesate Then
                stb.AppendLine(" LEFT JOIN #min_pesate_ct min_pesate_ct ON Zoo_Animali.Cod_Progetto = min_pesate_ct.Cod_Progetto ")
                stb.AppendLine(" LEFT JOIN #max_pesate_ct max_pesate_ct ON Zoo_Animali.Cod_Progetto = max_pesate_ct.Cod_Progetto ")
            End If
            If mostraAnomalie Then
                stb.AppendLine(" LEFT JOIN #anomalie_ct anomalie_ct ON anomalie_ct.Cod_Animale = giac_cte.Cod_Animale ")
            End If
            stb.AppendLine("LEFT  JOIN Contatti (NOLOCK) ON Zoo_Animali.CF_Fornitore = Contatti.Cod_Contatto AND Zoo_Animali.Piva = Contatti.Piva ")
            stb.AppendLine("  LEFT JOIN Lista_Patologie (NOLOCK)  ON Lista_Patologie.Patologia_Cod = Zoo_Animali.Id_Patologia  ")
            If filtraFornitori Then
                stb.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Contatti  (NOLOCK) ")
                stb.AppendLine("             WHERE Contatti.Cod_Contatto = Zoo_Animali.CF_Fornitore AND ")
                stb.AppendLine("                (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1)) Contatti_FornFatt")

                stb.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Contatti  (NOLOCK) ")
                stb.AppendLine("             WHERE Contatti.Cod_Contatto = Zoo_Animali.Fornitore_Provenienza AND ")
                stb.AppendLine("                (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1)) Contatti_FornProv")
            End If

            'Stalla svezzamento
            stb.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Contatti  (NOLOCK) ")
            stb.AppendLine("             WHERE Contatti.Cod_Contatto = Zoo_Animali.Stalla_Svezzamento AND ")
            stb.AppendLine("                (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1)) Contatti_StallaSvezz")

            stb.AppendLine(" WHERE 1=1 ")
            If filtraGiacenze1 Then
                Select Case bAll
                    Case False
                    'stb.AppendLine(" And Zoo_Animali.Validita_Inizio <= " & Agro_SQL_SaveDateTime(Data) & "   ")
                    'stb.AppendLine(" And Zoo_Animali.Validita_Fine >= " & Agro_SQL_SaveDateTime(Data) & "   ")
                    Case True
                        stb.AppendLine(" And CAST(Zoo_Animali.Validita_Inizio as Date) <= " & Agro_SQL_SaveDate(Data) & "   ")
                        stb.AppendLine(" And CAST(Zoo_Animali.Validita_Fine as Date) >= " & Agro_SQL_SaveDate(Data) & "   ")
                End Select
            End If

            If Piva <> "" Then
                stb.AppendLine(" And giac_cte.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And giac_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND giac_cte.Sta_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If

            If Raggruppamento_Cod <> 0 Then
                stb.AppendLine(" AND giac_cte.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "  ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND giac_cte.Cod_Animale = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If

            If Matricola <> "" Then
                stb.AppendLine(" AND Zoo_Animali.Matricola = '" & Agro_SQL_SaveText(Matricola) & "'  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If CFproprietario <> "" Then
                stb.AppendLine(" AND Zoo_Animali.CF_PROPRIETARIO = '" & Agro_SQL_SaveText(CFproprietario) & "'  ")
            End If
            'stb.AppendLine("ORDER BY giac_cte.Impresa ")

            stb.AppendLine(" DROP TABLE #agn_cte ")
            stb.AppendLine(" DROP TABLE #giac_cte ")

            If MostraGGPrimoCaricamento Then
                stb.AppendLine(" DROP TABLE #ZooMatricoleList ")
                stb.AppendLine(" DROP TABLE #DataPrimoCaricamento ")
            End If

            If mostraPesate Then
                stb.AppendLine(" DROP TABLE #pesate_cte ")
                stb.AppendLine(" DROP TABLE #max_pesate_ct ")
                stb.AppendLine(" DROP TABLE #min_pesate_ct ")
            End If

            If mostraAnomalie Then
                stb.AppendLine(" DROP TABLE #anomalie_ct ")
            End If


            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

            If listMatricola_Animali IsNot Nothing AndAlso listMatricola_Animali.Count > 0 AndAlso IDTestataTempMatricola <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTempMatricola, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiInviiNogmo(Piva As String,
                                    chiavi As List(Of String),
                                    objParametri_Server As AgronicaCoreParametri,
                                    objParametri_Utenti As AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.LeggiInviiNogmo()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            Dim IDTestataTemp As Integer = 0
            'CancellaRecordDaIDTestataTemp

            If chiavi Is Nothing OrElse chiavi.Count = 0 Then
                Return Nothing
            End If

            stb.Length = 0
            stb.AppendLine(" select Chiave, datainvio, stato, note, IIF(ud.Cognome != '', ud.Cognome + ' ' + ud.Nome, ud.Rag_Soc) as Utente ")
            stb.AppendLine("From Agronica_Log_Invio_Anagrafe")
            stb.AppendLine("LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Dettagli ud ON Agronica_Log_Invio_Anagrafe.Username_Modifica = ud.CodFisc")
            stb.AppendLine("Where Tipo = 'NOGMO'")
            stb.AppendLine("And Chiave IN " & chiavi.ToQueryInExpression)

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function


    Public Function LeggiCapoAnimale(Cod_Animale As String, ByVal objParametri_Server As AgronicaCoreParametri) As anagrafiche.CapoAnimale
        Dim objCapo As anagrafiche.CapoAnimale
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim zoo_animali = (From a In GiasContext.Zoo_Animali Where a.Cod_Progetto = Cod_Animale).FirstOrDefault
        If zoo_animali IsNot Nothing Then
            objCapo = New anagrafiche.CapoAnimale

            objCapo.categoria = New metaschema.Categoria(zoo_animali.CAT_COD)
            objCapo.codice = zoo_animali.Cod_Progetto
            objCapo.codiceAziendaNascita = zoo_animali.AUSL_AZI_NASCITA
            objCapo.codiceFiscaleDetentore = zoo_animali.CF_DETENTORE
            objCapo.codiceFiscaleProprietario = zoo_animali.CF_PROPRIETARIO
            objCapo.collare = zoo_animali.Collare
            objCapo.dataNascita = zoo_animali.DAT_NASCITA
            objCapo.flagCancellazione = False
            objCapo.fornitore = New anagrafiche.Contatto() With {.primaryKey = New anagrafiche.Contatto.PK("", zoo_animali.CF_Fornitore)}
            objCapo.genere = New metaschema.Genere(zoo_animali.GEN_COD)
            objCapo.idCapo_BDN = zoo_animali.Id_Capo_BDN
            objCapo.indirizzoProd = New metaschema.IndirizzoProduttivo(zoo_animali.IPRO_COD)
            objCapo.ingresso_mm_id = zoo_animali.Modello4_Ingresso
            objCapo.lottoFornitore = zoo_animali.Lotto_Fornitore
            objCapo.madre = New anagrafiche.CapoAnimale("", zoo_animali.Cod_Progetto_Madre, zoo_animali.MAT_MADRE) With {.razza = New metaschema.Razza(zoo_animali.Razza_Madre)}
            objCapo.padre = New anagrafiche.CapoAnimale("", zoo_animali.Cod_Progetto_Padre, zoo_animali.MAT_PADRE) With {.razza = New metaschema.Razza(zoo_animali.Razza_Padre)}
            objCapo.matricola = zoo_animali.Matricola
            objCapo.metodoProduzione = New metaschema.MetodoProduzione(zoo_animali.Metodo_Produzione)
            objCapo.nome = zoo_animali.Nome
            objCapo.numCertificato = zoo_animali.Certificato
            objCapo.partitaIva = zoo_animali.PIVA
            objCapo.razza = New metaschema.Razza(zoo_animali.RAZ_COD)
            objCapo.sesso = zoo_animali.Sesso
            objCapo.specie = New metaschema.utilizzi.Specie(zoo_animali.SPE_COD)
            objCapo.tipologia = New metaschema.TipologiaCapoAnimale(zoo_animali.TIPO_COD)
            objCapo.uscita_mm_id = zoo_animali.Modello4_Uscita
            objCapo.validita = New anagrafiche.IntervalloTemporale(zoo_animali.Validita_Inizio, zoo_animali.Validita_Fine)
            objCapo.validitaConversione = New anagrafiche.IntervalloTemporale(zoo_animali.Conversione_Inizio, zoo_animali.Conversione_Fine)

            objCapo.esercizi = New List(Of anagrafiche.EsercizioCapoAnimale)

            Dim zoo_animali_distinte = (From a In GiasContext.Zoo_Animali_Distinte Where a.Cod_Animale = Cod_Animale).ToList
            For Each distinta In zoo_animali_distinte
                Dim objDistinta = New anagrafiche.EsercizioCapoAnimale()
                objDistinta.codice = distinta.Cod_Progetto
                objDistinta.codice_capo_animale = distinta.Cod_Animale
                objDistinta.descrizione = distinta.Progetto_Des
                objDistinta.progettoNome = distinta.Progetto_Nome
                objDistinta.validita = New anagrafiche.IntervalloTemporale(distinta.Validita_Inizio, distinta.Validita_Fine)
                objCapo.esercizi.Add(objDistinta)
            Next

            objCapo.statiAccrescimento = New List(Of anagrafiche.StatoAccrescimento)

            Dim zoo_animali_stati_accrescimento = (From a In GiasContext.Zoo_AnimalixStati_Accrescimento Where a.Cod_Progetto = Cod_Animale).ToList
            For Each statoacrr In zoo_animali_stati_accrescimento
                Dim objStato = New anagrafiche.StatoAccrescimento()
                objStato.codice = statoacrr.Cod_Progetto
                objStato.validita = New anagrafiche.IntervalloTemporale(statoacrr.Validita_Inizio, statoacrr.Validita_Fine)
                objCapo.statiAccrescimento.Add(objStato)
            Next

        End If

        Return objCapo
    End Function

    Public Function Leggi(Piva As String, Cod_Animale As String, Matricola As String, Id_Capo_Bdn As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Giacenze2()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0
            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Zoo_Animali   ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If Piva <> "" Then
                stb.AppendLine(" And Zoo_Animali.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Id_Capo_Bdn <> 0 Then
                stb.AppendLine(" And Zoo_Animali.Id_Capo_Bdn = " & Agro_SQL_SaveNum(Id_Capo_Bdn) & "      ")
            End If

            If Cod_Animale <> "" AndAlso IsNumeric(Cod_Animale) AndAlso Cod_Animale <> 0 Then
                stb.AppendLine(" AND Zoo_Animali.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If

            If Matricola <> "" Then
                stb.AppendLine(" AND Zoo_Animali.Matricola = '" & Agro_SQL_SaveText(Matricola) & "'  ")
            End If

            'stb.AppendLine("ORDER BY giac_cte.Impresa ")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

        Catch ex As Exception

        End Try

        Return dt

    End Function

    Public Function LeggiLikeMatricola(Piva As String, Cod_Animale As Integer, Matricola As String, Id_Capo_Bdn As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Giacenze2()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0
            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Zoo_Animali   ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If Piva <> "" Then
                stb.AppendLine(" And Zoo_Animali.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Id_Capo_Bdn <> 0 Then
                stb.AppendLine(" And Zoo_Animali.Id_Capo_Bdn = " & Agro_SQL_SaveNum(Id_Capo_Bdn) & "      ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND Zoo_Animali.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If

            If Matricola <> "" Then
                stb.AppendLine(" AND Zoo_Animali.Matricola like '%" & Agro_SQL_SaveText(Matricola) & "'  ")
            End If

            'stb.AppendLine("ORDER BY giac_cte.Impresa ")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

        Catch ex As Exception

        End Try

        Return dt

    End Function

    Public Function ListCodProgettoFromMatricola(Matricola As String, matricolaInLike As Boolean, objParametri_Server As AgronicaCoreParametri) As List(Of Integer)
        Dim result As New List(Of Integer)
        Try
            Dim dt As New DataTable
            If Not matricolaInLike Then
                dt = Leggi("", "", Matricola, 0, objParametri_Server)
            Else
                dt = LeggiLikeMatricola("", 0, Matricola, 0, objParametri_Server)
            End If

            For Each rows In dt.Rows
                result.Add(rows("Cod_Progetto"))
            Next
        Catch ex As Exception
            result.Add(0)
        End Try
        Return result
    End Function


    Public Function Modifica_Parametrizzata(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Cod_Progetto As Integer,
                                            ByVal Campo As String,
                                            ByVal Valore As Object,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_Animali.Modifica_Parametrizzata()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False
        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim Stringa As Type = GetType(System.String)
        Dim Data As Type = GetType(System.DateTime)
        Dim Intero32 As Type = GetType(System.Int32)

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Cod_Progetto = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Progetto obbligatorio)")
            End If

            '---------------------------------------------

            Dim TypeVal As Type = Valore.GetType()

            If TypeVal.Equals(Stringa) Then
                strAssegnamento = Campo & "= '" & Agro_SQL_SaveText(Valore.ToString) & "' "
            ElseIf TypeVal.Equals(Data) Then
                strAssegnamento = Campo & "= " & Agro_SQL_SaveDate(Valore.ToString) & " "
            Else
                strAssegnamento = Campo & "= " & Agro_SQL_SaveNum(Valore.ToString) & " "
            End If


            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE Zoo_Animali SET ")

            StrSQL.AppendLine(strAssegnamento)

            StrSQL.AppendLine("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            'StrSQL.AppendLine(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ") Il sa_cod di default è 0 quindi non dovrebbe servire
            StrSQL.AppendLine(" AND   Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & " ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Leggi_Trattamenti(Piva As String,
                                      Sa_Cod As Integer,
                                      STA_NUM As Integer,
                                      Raggruppamento_Cod As Integer,
                                      Cod_Animale As Integer,
                                      Matricola As String,
                                      Data_Inizio As DateTime,
                                      Data_Fine As DateTime,
                                      ByVal Filtro_Visibilita_Utente As Boolean,
                                      ByVal listCod_Animali As List(Of Integer),
                                      ByVal Farm_Cat_List As Integer(),
                                      ByVal Farm_Cat_Sempl_List As Integer(),
                                      ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Trattamenti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            Dim IDTestataTemp As Integer = 0
            'CancellaRecordDaIDTestataTemp

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                For Each codAnimale In listCod_Animali
                    objTmp_AgendaTemp.Scrivi(IDTestataTemp, "", codAnimale, 0, objParametri)
                Next
            End If

            stb.Length = 0
            stb.AppendLine("  with  ")
            stb.AppendLine(" agn_cte as ")
            stb.AppendLine("  ( ")
            stb.AppendLine("  select a.Id_Agenda,  ")
            stb.AppendLine(" 	a.PIVA,  ")
            stb.AppendLine(" 	md.Cod_Progetto, ")
            stb.AppendLine(" 	md.Lotto, ")
            stb.AppendLine(" 	md.Udm_Cod, ")
            stb.AppendLine(" 	UnitaMisura.Udm_Des, ")
            stb.AppendLine(" 	UnitaMisura.Udm_Sim, ")
            stb.AppendLine(" 	mdes.Sa_Cod, ")
            stb.AppendLine(" 	mdes.Id_Destinazione, ")
            stb.AppendLine(" 	mdes.Tipo_Destinazione, ")
            stb.AppendLine(" 	mdes.Qta, ")
            stb.AppendLine(" 	md.Id_Mov, ")
            stb.AppendLine(" 	md.Id_Mov_Det, ")
            stb.AppendLine(" 	m.Cau_Mov, ")
            stb.AppendLine(" 	i.rag_soc, ")
            stb.AppendLine(" 	ic.val_cod as cuaa, ")
            stb.AppendLine(" 	Centri_Aziendali.sa_nome,")
            stb.AppendLine(" 	CAST(m.Data_Movimento as date) as Data_Movimento")
            stb.AppendLine(" 	from agenda a (NOLOCK)  ")
            stb.AppendLine(" 	inner join imprese i (NOLOCK) on i.PIVA = a.PIVA ")
            stb.AppendLine(" 	inner join imprese_codici ic (NOLOCK) on i.piva = ic.piva and ic.id_cod = 1010 ")
            stb.AppendLine(" 	inner join Movimenti m (NOLOCK) on	a.Id_Agenda = m.Id_Agenda  ")
            stb.AppendLine(" 	inner join Movimenti_dettagli md (NOLOCK) on md.Id_Agenda = m.Id_Agenda  AND md.Id_Mov = m.Id_Mov  ")
            stb.AppendLine(" 	inner join mov_destinazioni mdes (NOLOCK) on mdes.Id_Agenda = md.Id_Agenda and mdes.Id_Mov = md.Id_Mov and mdes.Id_Mov_Det = md.Id_Mov_Det ")
            stb.AppendLine(" 	inner join Centri_Aziendali (NOLOCK) ON mdes.Piva = Centri_Aziendali.Piva and mdes.Sa_Cod=Centri_Aziendali.sa_cod  ")
            stb.AppendLine(" 	INNER Join UnitaMisura (NOLOCK) ON UnitaMisura.Udm_Cod = md.Udm_Cod ")
            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                stb.AppendLine("	INNER Join __Tmp_Agenda (NOLOCK) ON md.Cod_Progetto = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
            End If
            stb.AppendLine(" 	where  1 = 1 ")
            If Piva <> "" Then
                stb.AppendLine(" And a.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And mdes.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND md.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If
            stb.AppendLine(" 	and a.lav_Cod >= 3000 And a.lav_cod < 4000 ")
            stb.AppendLine(" 	And m.Cau_Mov In ('7300', '7350', '3700', '3750')     ")
            stb.AppendLine(" 	And m.Data_Movimento >=  CONVERT(DateTime,'1900/01/01',120)     ")
            'stb.AppendLine(" 	And m.Data_Movimento <=    CONVERT(DateTime,'2023/02/02 16:23:02:111',120)      ")
            stb.AppendLine(" 	And md.Elem_Cod = 300  ")
            stb.AppendLine(" 	And md.Jolly_Int = 0  ")
            stb.AppendLine(" 	And mdes.Tipo_Destinazione IN (15, 21)  ")
            stb.AppendLine("	And a.Lav_Cod in (3034, 3001, 3000) ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" giac_cte as ( ")
            stb.AppendLine(" 	SELECT   ")
            stb.AppendLine("   agn_cte.Piva, agn_cte.Rag_Soc As Impresa,  ")
            stb.AppendLine("   agn_cte.cuaa, Stalla.BDN_Codice_Azienda,  ")
            stb.AppendLine("   agn_cte.Cod_Progetto AS Cod_Animale,  ")
            stb.AppendLine("   agn_cte.Lotto,  ")
            stb.AppendLine("   agn_cte.Udm_Cod,  ")
            stb.AppendLine("   agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
            stb.AppendLine("   agn_cte.Sa_Cod,  ")
            stb.AppendLine("   agn_cte.sa_nome,  ")
            stb.AppendLine("   agn_cte.Id_Destinazione,  ")
            stb.AppendLine("   agn_cte.Tipo_Destinazione,  ")
            stb.AppendLine("   Lista_AUSL.denominazione AS AUSL_DES,  ")
            stb.AppendLine("   MIN(agn_cte.Data_Movimento) as mindata,")
            stb.AppendLine("   IIF (MAX(agn_cte.Data_Movimento) = MIN(agn_cte.Data_Movimento), CONVERT(datetime, '2100-12-31 00:00:00.000', 120), MAX(agn_cte.Data_Movimento)) as maxdata,")
            stb.AppendLine("   COALESCE(Fabbricati.Fabbricato_Des, '') As STA_DES,  ")
            stb.AppendLine("   COALESCE(Fabbricati.Fabbricato_Cod, 0) As STA_NUM,  ")
            stb.AppendLine("   Convert(INTEGER, SUM( Case When agn_cte.CAU_MOV In ('7350','3750')              ")
            stb.AppendLine("           THEN -(agn_cte.qta)             ")
            stb.AppendLine("           Else agn_cte.qta             ")
            stb.AppendLine("           End)) As Giacenza   ")
            stb.AppendLine("  From agn_cte  ")
            stb.AppendLine("  INNER Join Stalla_Raggruppamenti (NOLOCK) ON agn_cte.Sa_Cod = Stalla_Raggruppamenti.sa_cod  ")
            stb.AppendLine("                                 AND agn_cte.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod  ")
            stb.AppendLine("                                 AND agn_cte.Tipo_Destinazione = 21  ")
            stb.AppendLine("  INNER JOIN Fabbricati (NOLOCK) ON Stalla_Raggruppamenti.PIVA = Fabbricati.Piva AND Stalla_Raggruppamenti.sa_cod = Fabbricati.SA_COD AND Stalla_Raggruppamenti.STA_NUM = Fabbricati.Fabbricato_Cod ")
            stb.AppendLine("  INNER JOIN Stalla (NOLOCK) ON Stalla.PIVA = Fabbricati.Piva AND Stalla.sa_cod = Fabbricati.SA_COD AND Stalla.STA_NUM = Fabbricati.Fabbricato_Cod  ")
            stb.AppendLine("  INNER JOIN Indirizzi (NOLOCK) ON Fabbricati.Indirizzo_Cod = Indirizzi.cod_indirizzo ")
            stb.AppendLine("  LEFT JOIN IstatxDistretti (NOLOCK) ON IstatxDistretti.pro_cod = Indirizzi.pro_cod_istat AND IstatxDistretti.com_cod = Indirizzi.com_cod_istat ")
            stb.AppendLine("  LEFT JOIN Lista_Distretti (NOLOCK) ON Lista_Distretti.distretto_id = IstatxDistretti.distretto_id ")
            stb.AppendLine("  LEFT JOIN Lista_AUSL (NOLOCK) ON Lista_Distretti.asl_id = Lista_AUSL.asl_id ")
            stb.AppendLine("   where 1=1  ")
            If Piva <> "" Then
                stb.AppendLine(" And agn_cte.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "     ")
            End If
            If Sa_Cod <> 0 Then
                stb.AppendLine(" And agn_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If
            stb.AppendLine("  GROUP BY agn_cte.Piva, agn_cte.Rag_Soc, agn_cte.cuaa, Stalla.BDN_Codice_Azienda, ")
            stb.AppendLine("  agn_cte.Cod_Progetto,  ")
            stb.AppendLine("  agn_cte.Lotto,  ")
            stb.AppendLine("  agn_cte.Udm_Cod, ")
            stb.AppendLine("  Fabbricati.Fabbricato_Des, ")
            stb.AppendLine("  Fabbricati.Fabbricato_Cod, ")
            stb.AppendLine("  agn_cte.Sa_Cod,  ")
            stb.AppendLine("  agn_cte.sa_nome, ")
            stb.AppendLine("  agn_cte.Id_Destinazione,  ")
            stb.AppendLine("  agn_cte.Tipo_Destinazione,  ")
            stb.AppendLine("  agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
            stb.AppendLine("  Lista_AUSL.denominazione  ")
            stb.AppendLine("  --HAVING Convert(INTEGER, SUM(Case When agn_cte.Cau_Mov In ('7350','3750') THEN -(agn_cte.qta) ELSE agn_cte.qta END)) <> 0   ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Categorie_Farmaci_Con_Semplificati as  ( ")
            stb.AppendLine(" SELECT ")
            stb.AppendLine(" 	Farmaci_Categorie.ID,  ")
            stb.AppendLine(" 	Farmaci_Categorie.Categoria_Codice,  ")
            stb.AppendLine(" 	Farmaci_Categorie.Categoria_Descrizione, ")
            stb.AppendLine(" 	CONCAT('|' ,STRING_AGG(Farmaci_Categorie_Semplificate.ID, '|') , '|') as Categorie_Semplificate_Cod, ")
            stb.AppendLine(" 	COALESCE(STRING_AGG(Farmaci_Categorie_Semplificate.Descrizione, ','), '') as Categorie_Semplificate_Des ")
            stb.AppendLine(" FROM Farmaci_Categorie ")
            stb.AppendLine(" LEFT JOIN Farmaci_CategoriexFarmaci_Categorie_Semplificate ON Farmaci_Categorie.ID = Farmaci_CategoriexFarmaci_Categorie_Semplificate.ID_Categoria ")
            stb.AppendLine(" LEFT JOIN Farmaci_Categorie_Semplificate ON Farmaci_CategoriexFarmaci_Categorie_Semplificate.ID_Categoria_Semplificata = Farmaci_Categorie_Semplificate.ID ")
            stb.AppendLine(" GROUP BY Farmaci_Categorie.ID, Farmaci_Categorie.Categoria_Codice, Farmaci_Categorie.Categoria_Descrizione ")
            stb.AppendLine(" ) ")
            stb.AppendLine(" SELECT ")
            stb.AppendLine(" 	CAST(Mov_Destinazioni.Id_Agenda as varchar(250)) + '_' + CAST(Mov_Destinazioni.Id_Destinazione as varchar(250)) as ID, ")
            stb.AppendLine(" 	Imprese.Piva,  ")
            stb.AppendLine(" 	Imprese.rag_soc,  ")
            stb.AppendLine(" 	Imprese_Codici.val_cod as CF_Impresa, ")
            stb.AppendLine(" 	giac_cte.BDN_Codice_Azienda,  ")
            stb.AppendLine(" 	giac_cte.Sa_Nome, ")
            stb.AppendLine(" 	giac_cte.Sa_Cod, ")
            stb.AppendLine(" 	giac_cte.STA_DES, ")
            stb.AppendLine(" 	giac_cte.STA_NUM, ")
            stb.AppendLine(" 	giac_cte.AUSL_DES, ")
            'stb.AppendLine(" 	Movimenti.Data_Movimento as Data_Somministrazione, ")
            stb.AppendLine(" 	Movimenti_dettagli.Extra_Date as Data_Prescrizione, ")
            stb.AppendLine(" 	Movimenti_Dettagli.Rif_Esterno as Num_Trattamento, ")
            stb.AppendLine(" 	Movimenti_Dettagli.Qta_Extra_Totale as QtaTotale, ")
            stb.AppendLine(" 	Movimenti_Dettagli.extra_int as Udm_Cod, ")
            stb.AppendLine(" 	UnitaMisura.UDM_SIM as Unita_Misura, ")
            stb.AppendLine(" 	COALESCE(rza.Note, '') AS Note_Somministrazione, ")
            stb.AppendLine(" 	Agenda.Validita_Inizio as Data_Inizio_Trattamento, ")
            stb.AppendLine(" 	Agenda.Validita_Fine as Data_Fine_Trattamento, ")
            stb.AppendLine(" 	Zoo_Animali.Matricola as Matricola, ")
            stb.AppendLine("    RIGHT(Zoo_Animali.Matricola, 5) AS Matricola_Breve,   ")
            stb.AppendLine("    RIGHT(Zoo_Animali.Matricola, 4) AS Matricola_Breve4,   ")
            stb.AppendLine("  	Zoo_Animali.Modello4_Uscita_Numero,  ")
            stb.AppendLine("  	Zoo_Animali.Modello4_Uscita_Prenotazione,  ")

            stb.AppendLine("  	Zoo_Animali.RAZ_COD,  ")
            stb.AppendLine("  	Lista_Razze_Animali.RAZ_DES,  ")
            stb.AppendLine("  	Zoo_Animali.Sesso,  ")
            stb.AppendLine("  	Zoo_Animali.Validita_Inizio,  ")
            stb.AppendLine("  	Zoo_Animali.Validita_Fine,  ")

            stb.AppendLine(" 	giac_cte.Cod_Animale, ")
            stb.AppendLine(" 	Zoo_Animali_Distinte.Cod_Progetto, ")
            stb.AppendLine(" 	Zoo_Animali_Distinte.Codice_Distinta, ")
            stb.AppendLine(" 	Zoo_Animali_Distinte.Progetto_Des, ")
            stb.AppendLine("    Zoo_Animali_Distinte.Codice_Distinta, ")
            stb.AppendLine(" 	CAST(Movimenti_Dettagli.Qta_Extra_Totale * Mov_Destinazioni.QuotaDistribuzione as decimal(10, 4)) as Qta_Capo, ")
            stb.AppendLine(" 	SospensioneCarne.Extra_Int as giorniSospensioneCarne, ")
            stb.AppendLine(" 	DATEADD(day, SospensioneCarne.Extra_Int, Agenda.Validita_Fine) AS DataSospensioneCarne, ")
            stb.AppendLine(" 	SospensioneLatte.Extra_Int as giorniSospensioneLatte, ")
            stb.AppendLine(" 	DATEADD(day, SospensioneLatte.Extra_Int, Agenda.Validita_Fine) AS DataSospensioneLatte, ")
            stb.AppendLine(" 	Farmaci.AIC, ")
            stb.AppendLine(" 	Farmaci.Denominazione, ")
            stb.AppendLine(" 	Farmaci.Confezione, ")
            stb.AppendLine(" 	Categorie_Farmaci_Con_Semplificati.Categoria_Codice, ")
            stb.AppendLine(" 	Categorie_Farmaci_Con_Semplificati.Categoria_Descrizione, ")
            stb.AppendLine(" 	Categorie_Farmaci_Con_Semplificati.Categorie_Semplificate_Cod, ")
            stb.AppendLine(" 	Categorie_Farmaci_Con_Semplificati.Categorie_Semplificate_Des, ")
            stb.AppendLine(" 	COALESCE(STRING_AGG(PrincipiAttivi.Pa_Des, ', '), '') as PrincipiAttivi ")
            stb.AppendLine(" FROM Agenda")
            stb.AppendLine(" JOIN Movimenti ON Agenda.Id_Agenda = Movimenti.Id_Agenda AND Movimenti.Cau_Mov = '3860'")
            stb.AppendLine(" JOIN Movimenti_Dettagli ON Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov")
            stb.AppendLine(" LEFT JOIN Mov_Dettaglio_Tecnico as SospensioneCarne ON Movimenti_Dettagli.Piva = SospensioneCarne.Piva ")
            stb.AppendLine(" 												    AND Movimenti_Dettagli.Id_Agenda = SospensioneCarne.Id_Agenda ")
            stb.AppendLine(" 													AND Movimenti_Dettagli.Id_Mov = SospensioneCarne.Id_Mov ")
            stb.AppendLine(" 													AND Movimenti_Dettagli.Id_Mov_Det = SospensioneCarne.Id_Mov_Det ")
            stb.AppendLine(" 													AND SospensioneCarne.Dett_Cod = 1 ")
            stb.AppendLine("  LEFT JOIN Mov_Dettaglio_Tecnico as SospensioneLatte ON Movimenti_Dettagli.Id_Agenda = SospensioneLatte.Id_Agenda ")
            stb.AppendLine(" 													AND Movimenti_Dettagli.Id_Mov = SospensioneLatte.Id_Mov ")
            stb.AppendLine(" 													AND Movimenti_Dettagli.Id_Mov_Det = SospensioneLatte.Id_Mov_Det ")
            stb.AppendLine(" 													AND SospensioneLatte.Dett_Cod = 2 ")
            stb.AppendLine(" JOIN Mov_Destinazioni ON Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det")
            stb.AppendLine(" INNER JOIN giac_cte ON Mov_Destinazioni.Id_Destinazione = giac_cte.Cod_Animale ")
            stb.AppendLine(" 		AND giac_cte.maxdata >= Movimenti.Data_Movimento")
            stb.AppendLine(" 		AND giac_cte.mindata <= Movimenti.Data_Movimento")
            stb.AppendLine("  INNER Join Zoo_Animali (NOLOCK) ON Zoo_Animali.Cod_Progetto = giac_cte.Cod_Animale  ")
            stb.AppendLine("  INNER JOIN Zoo_Animali_Distinte (NOLOCK) ON Zoo_Animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale  ")
            stb.AppendLine("                                          AND CAST(Zoo_Animali_Distinte.Validita_Inizio as datetime) <= Movimenti.Data_Movimento ")
            stb.AppendLine("                                          And CAST(Zoo_Animali_Distinte.Validita_Fine as datetime) >=   Movimenti.Data_Movimento ")
            stb.AppendLine("  INNER JOIN Zoo_AnimalixStati_Accrescimento ON Zoo_AnimalixStati_Accrescimento.Cod_Progetto  = Zoo_Animali.Cod_Progetto  ")
            stb.AppendLine("                                            AND Zoo_AnimalixStati_Accrescimento.PIVA  = Zoo_Animali.PIVA  ")
            stb.AppendLine("                                            AND Zoo_AnimalixStati_Accrescimento.sa_cod  = Zoo_Animali.sa_cod  ")
            stb.AppendLine("                                            AND CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as datetime) <=  Movimenti.Data_Movimento ")
            stb.AppendLine("                                            And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as datetime) >=  Movimenti.Data_Movimento ")
            stb.AppendLine(" INNER JOIN Zoo_Animali_Lista_Stati_Accrescimento (NOLOCK) ON Zoo_AnimalixStati_Accrescimento.GEN_COD = Zoo_Animali_Lista_Stati_Accrescimento.GEN_COD  ")
            stb.AppendLine(" 												AND Zoo_AnimalixStati_Accrescimento.SPE_COD = Zoo_Animali_Lista_Stati_Accrescimento.SPE_COD  ")
            stb.AppendLine(" 												AND Zoo_AnimalixStati_Accrescimento.STATO_COD = Zoo_Animali_Lista_Stati_Accrescimento.STATO_COD  ")
            stb.AppendLine(" 												AND Zoo_AnimalixStati_Accrescimento.TIPO_COD = Zoo_Animali_Lista_Stati_Accrescimento.TIPO_COD  ")
            stb.AppendLine(" INNER JOIN Lista_Specie_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Specie_Animali.SPE_COD  ")
            stb.AppendLine(" INNER JOIN Lista_Razze_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD And Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD  ")
            stb.AppendLine("  INNER JOIN Lista_Razze_Animali razza_madre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_madre.GEN_COD And Zoo_Animali.SPE_COD = razza_madre.SPE_COD And Zoo_Animali.Razza_Madre = razza_madre.RAZ_COD   ")
            stb.AppendLine("  INNER JOIN Lista_Razze_Animali razza_padre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_padre.GEN_COD And Zoo_Animali.SPE_COD = razza_padre.SPE_COD And Zoo_Animali.Razza_Padre = razza_padre.RAZ_COD   ")
            stb.AppendLine(" INNER JOIN Zoo_Animali_Lista_Tipi (NOLOCK) ON Zoo_Animali.GEN_COD = Zoo_Animali_Lista_Tipi.GEN_COD And Zoo_Animali.SPE_COD = Zoo_Animali_Lista_Tipi.SPE_COD And Zoo_Animali.TIPO_COD = Zoo_Animali_Lista_Tipi.TIPO_COD  ")
            stb.AppendLine(" INNER JOIN Lista_IndirizziProd_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD And Zoo_Animali.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD  ")
            stb.AppendLine(" LEFT  JOIN Contatti (NOLOCK) ON Zoo_Animali.CF_Fornitore = Contatti.Cod_Contatto AND Zoo_Animali.Piva = Contatti.Piva ")
            stb.AppendLine(" JOIN Imprese ON Agenda.PIVA = Imprese.PIVA")
            stb.AppendLine(" JOIN Imprese_Codici ON Imprese.Piva = Imprese_Codici.Piva AND Imprese_Codici.id_cod = 1010")
            stb.AppendLine(" JOIN Farmaci ON Movimenti_Dettagli.Pro_Cod = Farmaci.Farm_Cod")
            stb.AppendLine(" JOIN FarmacixCategorie ON Farmaci.Farm_Cod = FarmacixCategorie.Farm_Cod")
            stb.AppendLine(" JOIN Categorie_Farmaci_Con_Semplificati ON FarmacixCategorie.Cat_Cod = Categorie_Farmaci_Con_Semplificati.ID ")
            stb.AppendLine(" LEFT JOIN FarmacixPrincipiAttivi ON Farmaci.Farm_Cod = FarmacixPrincipiAttivi.Farm_Cod ")
            stb.AppendLine(" LEFT JOIN PrincipiAttivi ON FarmacixPrincipiAttivi.PA_Cod = PrincipiAttivi.Pa_Cod ")
            stb.AppendLine(" JOIN UnitaMisura ON UnitaMisura.UDM_COD = Movimenti_Dettagli.Extra_Int")

            ' JOIN Ricette_Zoo per ricavare numero somministrazione
            stb.AppendLine("LEFT JOIN Ricette_ZooxAgenda rzxa ON Agenda.Id_Agenda = rzxa.Id_Agenda ").
                AppendLine("LEFT JOIN Ricette_Zoo_Agenda rza ON rzxa.Id_Ricetta = rza.IdRicetta AND rzxa.Id_RigaRicetta = rza.IdAgenda ")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON Agenda.Piva = p.piva AND p.Sa_Cod = giac_cte.Sa_Cod AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            End If
            stb.AppendLine(" WHERE Agenda.Lav_Cod = 3028 ")

            If Piva <> "" Then
                stb.AppendLine(" And Agenda.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And giac_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND giac_cte.Sta_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If

            If Raggruppamento_Cod <> 0 Then
                stb.AppendLine(" AND giac_cte.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "  ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND giac_cte.Cod_Animale = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If

            If Matricola <> "" Then
                stb.AppendLine(" AND Zoo_Animali.Matricola LIKE '%" & Agro_SQL_SaveText(Matricola) & "%' ")
            End If

            If Data_Inizio > AGRODATAINIZIO Then
                stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDateTime(Data_Inizio) & " ")
            End If

            If Farm_Cat_List IsNot Nothing AndAlso Farm_Cat_List.Length > 0 Then
                stb.AppendLine(" AND Categorie_Farmaci_Con_Semplificati.ID IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", Farm_Cat_List)) & ") ")
            End If

            If Farm_Cat_Sempl_List IsNot Nothing AndAlso Farm_Cat_Sempl_List.Length > 0 Then
                stb.AppendLine(" AND ( ")
                Dim i = 0
                For Each cat_sempl In Farm_Cat_Sempl_List
                    If i <> 0 Then
                        stb.Append(" OR ")
                    End If
                    stb.Append(" Categorie_Farmaci_Con_Semplificati.Categorie_Semplificate_Cod like '%|" & cat_sempl & "|%' ")
                    i = i + 1
                Next
                stb.Append(" ) ")
            End If

            If Data_Fine < AGRODATAFINE Then
                Data_Fine = Data_Fine.AddHours(24)
                Data_Fine = Data_Fine.AddSeconds(-1)
                stb.AppendLine(" AND	Movimenti.Data_Movimento <= " & Agro_SQL_SaveDateTime(Data_Fine) & " ")
            End If

            stb.AppendLine(" GROUP BY  ")
            stb.AppendLine(" 	Mov_Destinazioni.Id_Agenda, ")
            stb.AppendLine(" 	Mov_Destinazioni.Id_Destinazione, ")
            stb.AppendLine(" 	Mov_Destinazioni.QuotaDistribuzione, ")
            stb.AppendLine("  	Imprese.Piva,   ")
            stb.AppendLine("  	Imprese.rag_soc,   ")
            stb.AppendLine("  	Imprese_Codici.val_cod,  ")
            stb.AppendLine("  	giac_cte.BDN_Codice_Azienda,   ")
            stb.AppendLine("  	giac_cte.Sa_Nome,  ")
            stb.AppendLine("  	giac_cte.Sa_Cod,  ")
            stb.AppendLine("  	giac_cte.STA_DES,  ")
            stb.AppendLine("  	giac_cte.STA_NUM,  ")
            stb.AppendLine("  	giac_cte.AUSL_DES,  ")
            stb.AppendLine("  	rza.Note,  ")
            stb.AppendLine("  	Movimenti.Data_Movimento,  ")
            stb.AppendLine("  	Movimenti_dettagli.Extra_Date,  ")
            stb.AppendLine("  	Movimenti_Dettagli.Rif_Esterno,  ")
            stb.AppendLine("  	Movimenti_Dettagli.Qta_Extra_Totale,  ")
            stb.AppendLine("  	Movimenti_Dettagli.extra_int,  ")
            stb.AppendLine("  	UnitaMisura.UDM_SIM,  ")
            stb.AppendLine("  	Agenda.Validita_Inizio,  ")
            stb.AppendLine("  	Agenda.Validita_Fine,  ")
            stb.AppendLine("  	Zoo_Animali.Matricola,  ")
            stb.AppendLine("  	Zoo_Animali.Modello4_Uscita_Numero,  ")
            stb.AppendLine("  	Zoo_Animali.Modello4_Uscita_Prenotazione,  ")
            stb.AppendLine("  	Zoo_Animali.RAZ_COD,  ")
            stb.AppendLine("  	Lista_Razze_Animali.RAZ_DES,  ")
            stb.AppendLine("  	Zoo_Animali.Sesso,  ")
            stb.AppendLine("  	Zoo_Animali.Validita_Inizio,  ")
            stb.AppendLine("  	Zoo_Animali.Validita_Fine,  ")
            stb.AppendLine("  	giac_cte.Cod_Animale,  ")
            stb.AppendLine("  	Zoo_Animali_Distinte.Cod_Progetto,  ")
            stb.AppendLine("  	Zoo_Animali_Distinte.Progetto_Des, ")
            stb.AppendLine("  Zoo_Animali_Distinte.Codice_Distinta, ")
            stb.AppendLine("  	SospensioneCarne.Extra_Int,  ")
            stb.AppendLine("  	SospensioneLatte.Extra_Int,  ")
            stb.AppendLine("  	Farmaci.AIC,  ")
            stb.AppendLine("  	Farmaci.Denominazione,  ")
            stb.AppendLine("  	Farmaci.Confezione,  ")
            stb.AppendLine("  	Categorie_Farmaci_Con_Semplificati.Categoria_Codice,  ")
            stb.AppendLine("  	Categorie_Farmaci_Con_Semplificati.Categoria_Descrizione, ")
            stb.AppendLine("  	Categorie_Farmaci_Con_Semplificati.Categorie_Semplificate_Cod, ")
            stb.AppendLine("  	Categorie_Farmaci_Con_Semplificati.Categorie_Semplificate_Des ")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Leggi_Testata_Trattamenti(Piva As String,
                                      Sa_Cod As Integer,
                                      STA_NUM As Integer,
                                      Data_Inizio As DateTime,
                                      Data_Fine As DateTime,
                                      ByVal Filtro_Visibilita_Utente As Boolean,
                                      ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Trattamenti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0
            stb.AppendLine("  with  ")
            stb.AppendLine(" agn_cte as ")
            stb.AppendLine("  ( ")
            stb.AppendLine("  select a.Id_Agenda,  ")
            stb.AppendLine(" 	a.PIVA,  ")
            stb.AppendLine(" 	md.Cod_Progetto, ")
            stb.AppendLine(" 	md.Lotto, ")
            stb.AppendLine(" 	md.Udm_Cod, ")
            stb.AppendLine(" 	UnitaMisura.Udm_Des, ")
            stb.AppendLine(" 	UnitaMisura.Udm_Sim, ")
            stb.AppendLine(" 	mdes.Sa_Cod, ")
            stb.AppendLine(" 	mdes.Id_Destinazione, ")
            stb.AppendLine(" 	mdes.Tipo_Destinazione, ")
            stb.AppendLine(" 	mdes.Qta, ")
            stb.AppendLine(" 	md.Id_Mov, ")
            stb.AppendLine(" 	md.Id_Mov_Det, ")
            stb.AppendLine(" 	m.Cau_Mov, ")
            stb.AppendLine(" 	i.rag_soc, ")
            stb.AppendLine(" 	ic.val_cod as cuaa, ")
            stb.AppendLine(" 	Centri_Aziendali.sa_nome,")
            stb.AppendLine(" 	m.Data_Movimento")
            stb.AppendLine(" 	from agenda a (NOLOCK)  ")
            stb.AppendLine(" 	inner join imprese i (NOLOCK) on i.PIVA = a.PIVA ")
            stb.AppendLine(" 	inner join imprese_codici ic (NOLOCK) on i.piva = ic.piva and ic.id_cod = 1010 ")
            stb.AppendLine(" 	inner join Movimenti m (NOLOCK) on	a.Id_Agenda = m.Id_Agenda  ")
            stb.AppendLine(" 	inner join Movimenti_dettagli md (NOLOCK) on md.Id_Agenda = m.Id_Agenda  AND md.Id_Mov = m.Id_Mov  ")
            stb.AppendLine(" 	inner join mov_destinazioni mdes (NOLOCK) on mdes.Id_Agenda = md.Id_Agenda and mdes.Id_Mov = md.Id_Mov and mdes.Id_Mov_Det = md.Id_Mov_Det ")
            stb.AppendLine(" 	inner join Centri_Aziendali (NOLOCK) ON mdes.Piva = Centri_Aziendali.Piva and mdes.Sa_Cod=Centri_Aziendali.sa_cod  ")
            stb.AppendLine(" 	INNER Join UnitaMisura (NOLOCK) ON UnitaMisura.Udm_Cod = md.Udm_Cod ")
            stb.AppendLine(" 	where  1 = 1 ")
            If Piva <> "" Then
                stb.AppendLine(" And a.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And mdes.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            stb.AppendLine(" 	and a.lav_Cod >= 3000 And a.lav_cod < 4000 ")
            stb.AppendLine(" 	And m.Cau_Mov In ('7300', '7350', '3700', '3750')     ")
            stb.AppendLine(" 	And m.Data_Movimento >=  CONVERT(DateTime,'1900/01/01',120)     ")
            'stb.AppendLine(" 	And m.Data_Movimento <=    CONVERT(DateTime,'2023/02/02 16:23:02:111',120)      ")
            stb.AppendLine(" 	And md.Elem_Cod = 300  ")
            stb.AppendLine(" 	And md.Jolly_Int = 0  ")
            stb.AppendLine(" 	And mdes.Tipo_Destinazione IN (15, 21)  ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" giac_cte as ( ")
            stb.AppendLine(" 	SELECT   ")
            stb.AppendLine("   agn_cte.Piva, agn_cte.Rag_Soc As Impresa,  ")
            stb.AppendLine("   agn_cte.cuaa, Stalla.BDN_Codice_Azienda,  ")
            stb.AppendLine("   agn_cte.Cod_Progetto AS Cod_Animale,  ")
            stb.AppendLine("   agn_cte.Lotto,  ")
            stb.AppendLine("   agn_cte.Udm_Cod,  ")
            stb.AppendLine("   agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
            stb.AppendLine("   agn_cte.Sa_Cod,  ")
            stb.AppendLine("   agn_cte.sa_nome,  ")
            stb.AppendLine("   agn_cte.Id_Destinazione,  ")
            stb.AppendLine("   agn_cte.Tipo_Destinazione,  ")
            stb.AppendLine("   Lista_AUSL.denominazione AS AUSL_DES,  ")
            stb.AppendLine("   MIN(agn_cte.Data_Movimento) as mindata,")
            stb.AppendLine("   IIF (MAX(agn_cte.Data_Movimento) = MIN(agn_cte.Data_Movimento), CONVERT(datetime, '2100-12-31 00:00:00.000', 120), MAX(agn_cte.Data_Movimento)) as maxdata,")
            stb.AppendLine("   COALESCE(Fabbricati.Fabbricato_Des, '') As STA_DES,  ")
            stb.AppendLine(" 	COALESCE(Fabbricati.Fabbricato_Cod, 0) As STA_NUM,  ")
            stb.AppendLine("   COALESCE(Stalla_Raggruppamenti.Raggruppamento_Des, '') as Raggruppamento_Des,  ")
            stb.AppendLine("   COALESCE(Stalla_Raggruppamenti.Raggruppamento_Cod, 0) as Raggruppamento_Cod,  ")
            stb.AppendLine("   Convert(INTEGER, SUM( Case When agn_cte.CAU_MOV In ('7350','3750')              ")
            stb.AppendLine("           THEN -(agn_cte.qta)             ")
            stb.AppendLine("           Else agn_cte.qta             ")
            stb.AppendLine("           End)) As Giacenza   ")
            stb.AppendLine("  From agn_cte  ")
            stb.AppendLine("  INNER Join Stalla_Raggruppamenti (NOLOCK) ON agn_cte.Sa_Cod = Stalla_Raggruppamenti.sa_cod  ")
            stb.AppendLine("                                 AND agn_cte.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod  ")
            stb.AppendLine("                                 AND agn_cte.Tipo_Destinazione = 21  ")
            stb.AppendLine("  INNER JOIN Fabbricati (NOLOCK) ON Stalla_Raggruppamenti.PIVA = Fabbricati.Piva AND Stalla_Raggruppamenti.sa_cod = Fabbricati.SA_COD AND Stalla_Raggruppamenti.STA_NUM = Fabbricati.Fabbricato_Cod ")
            stb.AppendLine("  INNER JOIN Stalla (NOLOCK) ON Stalla.PIVA = Fabbricati.Piva AND Stalla.sa_cod = Fabbricati.SA_COD AND Stalla.STA_NUM = Fabbricati.Fabbricato_Cod  ")
            stb.AppendLine("  INNER JOIN Indirizzi (NOLOCK) ON Fabbricati.Indirizzo_Cod = Indirizzi.cod_indirizzo ")
            stb.AppendLine("  LEFT JOIN IstatxDistretti (NOLOCK) ON IstatxDistretti.pro_cod = Indirizzi.pro_cod_istat AND IstatxDistretti.com_cod = Indirizzi.com_cod_istat ")
            stb.AppendLine("  LEFT JOIN Lista_Distretti (NOLOCK) ON Lista_Distretti.distretto_id = IstatxDistretti.distretto_id ")
            stb.AppendLine("  LEFT JOIN Lista_AUSL (NOLOCK) ON Lista_Distretti.asl_id = Lista_AUSL.asl_id ")
            stb.AppendLine("   where 1=1  ")
            If Piva <> "" Then
                stb.AppendLine(" And agn_cte.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "     ")
            End If
            If Sa_Cod <> 0 Then
                stb.AppendLine(" And agn_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If
            stb.AppendLine("  GROUP BY agn_cte.Piva, agn_cte.Rag_Soc, agn_cte.cuaa, Stalla.BDN_Codice_Azienda, ")
            stb.AppendLine("  agn_cte.Cod_Progetto,  ")
            stb.AppendLine("  agn_cte.Lotto,  ")
            stb.AppendLine("  agn_cte.Udm_Cod, ")
            stb.AppendLine("  Fabbricati.Fabbricato_Des, ")
            stb.AppendLine("  Fabbricati.Fabbricato_Cod, ")
            stb.AppendLine("  agn_cte.Sa_Cod,  ")
            stb.AppendLine("  agn_cte.sa_nome, ")
            stb.AppendLine("  agn_cte.Id_Destinazione,  ")
            stb.AppendLine("  agn_cte.Tipo_Destinazione,  ")
            stb.AppendLine("  agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
            stb.AppendLine("  Lista_AUSL.denominazione,  ")
            stb.AppendLine("  Stalla_Raggruppamenti.Raggruppamento_Des,  ")
            stb.AppendLine("  Stalla_Raggruppamenti.Raggruppamento_Cod  ")
            stb.AppendLine("  --HAVING Convert(INTEGER, SUM(Case When agn_cte.Cau_Mov In ('7350','3750') THEN -(agn_cte.qta) ELSE agn_cte.qta END)) <> 0   ")
            stb.AppendLine(" ) ")
            stb.AppendLine(" SELECT  ")
            stb.AppendLine(" 	DISTINCT  ")
            stb.AppendLine(" 	giac_cte.Piva, ")
            stb.AppendLine(" 	giac_cte.BDN_Codice_Azienda,   ")
            stb.AppendLine("  	giac_cte.Sa_Cod,  ")
            stb.AppendLine("  	giac_cte.STA_DES,  ")
            stb.AppendLine("  	giac_cte.STA_NUM,  ")
            stb.AppendLine("  	giac_cte.AUSL_DES,  ")
            stb.AppendLine("  	agenda.Id_Agenda,  ")
            stb.AppendLine("  	Movimenti_dettagli.Extra_Date as Data_Prescrizione,  ")
            stb.AppendLine("  	Movimenti_Dettagli.Rif_Esterno as Num_Trattamento,  ")
            stb.AppendLine(" 	Agenda.Validita_Inizio as Data_Inizio_Trattamento,  ")
            stb.AppendLine("  	Agenda.Validita_Fine as Data_Fine_Trattamento  ")
            stb.AppendLine(" FROM Agenda")
            stb.AppendLine(" JOIN Movimenti ON Agenda.Id_Agenda = Movimenti.Id_Agenda AND Movimenti.Cau_Mov = '3860'")
            stb.AppendLine(" JOIN Movimenti_Dettagli ON Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov")
            stb.AppendLine(" LEFT JOIN Mov_Dettaglio_Tecnico as SospensioneCarne ON Movimenti_Dettagli.Piva = SospensioneCarne.Piva ")
            stb.AppendLine(" 												    AND Movimenti_Dettagli.Id_Agenda = SospensioneCarne.Id_Agenda ")
            stb.AppendLine(" 													AND Movimenti_Dettagli.Id_Mov = SospensioneCarne.Id_Mov ")
            stb.AppendLine(" 													AND Movimenti_Dettagli.Id_Mov_Det = SospensioneCarne.Id_Mov_Det ")
            stb.AppendLine(" 													AND SospensioneCarne.Dett_Cod = 1 ")
            stb.AppendLine("  LEFT JOIN Mov_Dettaglio_Tecnico as SospensioneLatte ON Movimenti_Dettagli.Id_Agenda = SospensioneLatte.Id_Agenda ")
            stb.AppendLine(" 													AND Movimenti_Dettagli.Id_Mov = SospensioneLatte.Id_Mov ")
            stb.AppendLine(" 													AND Movimenti_Dettagli.Id_Mov_Det = SospensioneLatte.Id_Mov_Det ")
            stb.AppendLine(" 													AND SospensioneLatte.Dett_Cod = 2 ")
            stb.AppendLine(" JOIN Mov_Destinazioni ON Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det")
            stb.AppendLine(" INNER JOIN giac_cte ON Mov_Destinazioni.Id_Destinazione = giac_cte.Cod_Animale ")
            stb.AppendLine(" 		AND giac_cte.maxdata >= Movimenti.Data_Movimento")
            stb.AppendLine(" 		AND giac_cte.mindata <= Movimenti.Data_Movimento")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON Agenda.Piva = p.piva AND p.Sa_Cod = giac_cte.Sa_Cod AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            End If
            stb.AppendLine(" WHERE Agenda.Lav_Cod = 3028 ")

            If Piva <> "" Then
                stb.AppendLine(" And Agenda.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And giac_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND giac_cte.Sta_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If

            If Data_Inizio > AGRODATAINIZIO Then
                stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDateTime(Data_Inizio) & " ")
            End If

            If Data_Fine < AGRODATAFINE Then
                Data_Fine = Data_Fine.AddHours(24)
                Data_Fine = Data_Fine.AddSeconds(-1)
                stb.AppendLine(" AND	Movimenti.Data_Movimento <= " & Agro_SQL_SaveDateTime(Data_Fine) & " ")
            End If

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Leggi_Capi_Senza_Trattamenti(Piva As String,
                                      Sa_Cod As Integer,
                                      STA_NUM As Integer,
                                      Raggruppamento_Cod As Integer,
                                      Cod_Animale As Integer,
                                      Data As DateTime,
                                      giorni_senza_trattamenti As Integer,
                                      ByVal Filtro_Visibilita_Utente As Boolean,
                                      ByVal listCod_Animali As List(Of Integer),
                                      ByVal Farm_Cat_List As Integer(),
                                      ByVal Farm_Cat_Sempl_List As Integer(),
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal MostraGGPrimoCaricamento As Boolean = False,
                                      Optional ByVal MostraAnomalie As Boolean = False) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Capi_Senza_Trattamenti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            Dim IDTestataTemp As Integer = 0
            'CancellaRecordDaIDTestataTemp

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                For Each codAnimale In listCod_Animali
                    objTmp_AgendaTemp.Scrivi(IDTestataTemp, "", codAnimale, 0, objParametri)
                Next
            End If

            stb.Length = 0
            stb.AppendLine("  --Creazione agn_cte  ")
            Dim agenda_cte = crea_agn_cte(Piva, Sa_Cod, STA_NUM, Raggruppamento_Cod, Cod_Animale, Data, giorni_senza_trattamenti, Filtro_Visibilita_Utente, listCod_Animali, IDTestataTemp, objParametri)
            stb.AppendLine(agenda_cte)

            stb.AppendLine("  --Creazione Categorie_Farmaci_Con_Semplificati  ")
            Dim categorie_farmaci_con_Semplificati_cte = crea_Categorie_Farmaci_Con_Semplificati()
            stb.AppendLine(categorie_farmaci_con_Semplificati_cte)

            stb.AppendLine("  --Creazione sospensioneCarne_cte")
            Dim sospensioneCarne_cte = crea_sospensioneCarne_cte(Piva, Filtro_Visibilita_Utente, objParametri)
            stb.AppendLine(sospensioneCarne_cte)

            stb.AppendLine("  --Creazione sospensioneLatte_cte")
            Dim sospensioneLatte_cte = crea_sospensioneLatte_cte(Piva, Filtro_Visibilita_Utente, objParametri)
            stb.AppendLine(sospensioneLatte_cte)

            stb.AppendLine("  --Creazione Trattamenti_cte")
            Dim trattamenti_cte = crea_trattamenti_cte(Piva, Sa_Cod, STA_NUM, Raggruppamento_Cod, Cod_Animale, Data, giorni_senza_trattamenti, listCod_Animali, Farm_Cat_List, Farm_Cat_Sempl_List, IDTestataTemp, Filtro_Visibilita_Utente, objParametri)
            stb.AppendLine(trattamenti_cte)

            stb.AppendLine("  --Creazione giac_cte")
            Dim giac_cte = crea_giac_cte(Piva, Sa_Cod, STA_NUM, Raggruppamento_Cod, Cod_Animale, Data, giorni_senza_trattamenti, Filtro_Visibilita_Utente, listCod_Animali, IDTestataTemp)
            stb.AppendLine(giac_cte)

            stb.AppendLine("  --Creazione dataUltimoProtocolloInCorso")
            Dim cte_UltimoProtocollo = crea_ultimoProtocolloInCorso_cte()
            stb.AppendLine(cte_UltimoProtocollo)

            If MostraGGPrimoCaricamento Then
                stb.AppendLine(" --Creazione ZooMatricoleList")
                Dim zooMatricoleList_cte = crea_zooMatricoleList()
                stb.AppendLine(zooMatricoleList_cte)

                stb.AppendLine(" --Creazione DataPrimoCaricamento")
                Dim DataPrimoCaricamento_cte = crea_DataPrimoCaricamento(Piva)
                stb.AppendLine(DataPrimoCaricamento_cte)
            End If

            If MostraAnomalie Then
                Dim anomalie_TT As String = CreaAnomalie_TT()
                stb.AppendLine(anomalie_TT)
            End If

            stb.AppendLine(" SELECT giac_cte.Piva + '_' + CAST(giac_cte.Sa_Cod as varchar(100)) + '_' + CAST(giac_cte.Cod_Animale as varchar(100)) as chiave,  ")
            stb.AppendLine(" giac_cte.*,  ")
            stb.AppendLine(" Lista_Specie_Animali.SPE_DES,  ")
            stb.AppendLine(" Lista_Razze_Animali.RAZ_DES,  ")
            stb.AppendLine(" Zoo_Animali_Lista_Tipi.Tipo_Des,  ")
            stb.AppendLine(" Lista_IndirizziProd_Animali.IPRO_DES,  ")
            stb.AppendLine(" Zoo_Animali_Distinte.Cod_Progetto,  ")
            stb.AppendLine(" Zoo_Animali_Distinte.Progetto_Des,  ")
            stb.AppendLine(" Zoo_Animali_Distinte.Progetto_Nome,  ")
            stb.AppendLine("   RIGHT(Zoo_Animali.Matricola, 4) AS Matricola_Breve4,   ")
            stb.AppendLine("   RIGHT(Zoo_Animali.Matricola, 5) AS Matricola_Breve,   ")
            stb.AppendLine(" Zoo_Animali_Distinte.Codice_Distinta,  ")
            stb.AppendLine(" Zoo_AnimalixStati_Accrescimento.Stato_Cod,  ")
            stb.AppendLine(" Zoo_Animali_Lista_Stati_Accrescimento.Stato_Des,  ")
            stb.AppendLine(" Contatti.Rag_Soc,  ")
            stb.AppendLine(" Contatti.Cod_Contatto, ")
            stb.AppendLine("  Zoo_Animali.RAZ_COD,  ")
            stb.AppendLine("  	Zoo_Animali.IPRO_COD,  ")
            stb.AppendLine("  	Zoo_Animali.CF_Fornitore,  ")
            stb.AppendLine("    Zoo_Animali.Mat_Madre,   ")
            stb.AppendLine("    Zoo_Animali.Mat_Padre,   ")
            stb.AppendLine("    CASE Zoo_Animali.Metodo_Produzione WHEN 1 THEN 'Integrato' WHEN 2 THEN 'In Conversione' WHEN 3 THEN 'Biologico' END as Metodo_Produzione,   ")
            stb.AppendLine("    Zoo_Animali.Razza_Padre AS RazCod_Padre,   ")
            stb.AppendLine("    razza_madre.RAZ_DES as RazDes_Madre,   ")
            stb.AppendLine("    Zoo_Animali.Razza_Madre AS RazCod_Madre,   ")
            stb.AppendLine("    razza_padre.RAZ_DES as RazDes_Padre,   ")
            stb.AppendLine("    Zoo_Animali.Lotto_Fornitore, ")
            stb.AppendLine("    Zoo_Animali.Matricola,   ")
            stb.AppendLine("    Zoo_Animali.GEN_COD,   ")
            stb.AppendLine("    Zoo_Animali.SPE_COD,   ")
            stb.AppendLine("    Zoo_Animali.TIPO_COD,   ")
            stb.AppendLine("    Zoo_Animali.Progetto,   ")
            stb.AppendLine("    Zoo_Animali.Validita_Inizio,   ")
            stb.AppendLine("    Zoo_Animali.Validita_Fine,   ")
            stb.AppendLine("    Zoo_Animali.Nome,   ")
            stb.AppendLine("    Zoo_Animali.Sesso,   ")
            stb.AppendLine("    Zoo_Animali.Modello4_Ingresso,   ")
            stb.AppendLine("    Zoo_Animali.Modello4_Uscita,   ")
            stb.AppendLine("    ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ZOO_Animali.Username_Creazione), ZOO_Animali.Username_Creazione) AS Utente_Creazione,   ")
            stb.AppendLine("    ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ZOO_Animali.Username_Modifica), ZOO_Animali.Username_Modifica) AS Utente_Modifica,   ")
            stb.AppendLine("    Zoo_Animali.Data_Creazione,   ")
            stb.AppendLine("    Zoo_Animali.Data_Modifica,   ")
            stb.AppendLine("    Zoo_Animali.Dat_Nascita,   ")
            stb.AppendLine("    DATEDIFF(day, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") as eta_giorni_totali,")
            'stb.AppendLine("    IIF(DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, GETDATE()) , Zoo_Animali.DAT_NASCITA), GETDATE()) >= 0,")
            'stb.AppendLine(" 	   DATEDIFF(month, Zoo_Animali.Dat_Nascita, GETDATE()),")
            'stb.AppendLine(" 	   DATEDIFF(month, Zoo_Animali.Dat_Nascita, GETDATE()) -1) as eta_mesi,")
            stb.AppendLine(Calcolo_Mesi("Zoo_Animali.Dat_Nascita", Agro_SQL_SaveDate(Data, False)) & " as eta_mesi,")
            'stb.AppendLine("    IIF(DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, GETDATE()) , Zoo_Animali.DAT_NASCITA), GETDATE()) >= 0,")
            'stb.AppendLine(" 	   DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, GETDATE()) , Zoo_Animali.DAT_NASCITA), GETDATE()),")
            'stb.AppendLine(" 	   DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, GETDATE()) - 1, Zoo_Animali.DAT_NASCITA), GETDATE())) as eta_giorni,")
            stb.AppendLine(Calcolo_Giorni("Zoo_Animali.Dat_Nascita", Agro_SQL_SaveDate(Data, False)) & " as eta_giorni,")


            stb.AppendLine("    DATEDIFF(day, Zoo_Animali.Validita_Inizio, " & Agro_SQL_SaveDate(Data, False) & ") as giorni_totali_stalla,")
            stb.AppendLine(Calcolo_Mesi("Zoo_Animali.Validita_Inizio", Agro_SQL_SaveDate(Data, False)) & " as mesi_stalla,")
            stb.AppendLine(Calcolo_Giorni("Zoo_Animali.Validita_Inizio", Agro_SQL_SaveDate(Data, False)) & " as giorni_stalla,")

            stb.AppendLine("    Zoo_Animali.Id_Capo_BDN,   ")
            stb.AppendLine("    Zoo_Animali.CF_PROPRIETARIO,   ")
            stb.AppendLine("    Zoo_Animali.CF_DETENTORE,   ")
            stb.AppendLine("    Zoo_Animali.AUSL_AZI_NASCITA,   ")
            stb.AppendLine("    CASE WHEN Zoo_Animali.Id_Capo_BDN = 0 THEN 'No' ELSE 'Si' END AS FlagBDN,   ")
            stb.AppendLine("    Zoo_Animali.Certificato  ")
            stb.AppendLine("    , Zoo_Animali.Modello4_Ingresso_Numero  ")
            stb.AppendLine("    , Zoo_Animali.Modello4_Uscita_Numero  ")
            stb.AppendLine("    , Zoo_Animali.Modello4_Ingresso_Prenotazione  ")
            stb.AppendLine("    , Zoo_Animali.Modello4_Uscita_Prenotazione  ")
            stb.AppendLine("    , Zoo_Animali.Codice_Azienda_Uscita  ")
            stb.AppendLine("    , Zoo_Animali.Data_Documento_Ingresso  ")
            stb.AppendLine("    , Zoo_Animali.Data_Documento_Uscita ")
            stb.AppendLine("    , Zoo_Animali.Note ")
            stb.AppendLine("    , COALESCE(Zoo_Animali.Anomalie_Note, '') as Anomalie_Note ")
            stb.AppendLine("    , Lista_Patologie.Patologia_Des ")
            stb.AppendLine("    , trattamenti_cte.Data_Ultimo_Trattamento")
            stb.AppendLine("    , DATEADD(DAY, " & Agro_SQL_SaveNum(giorni_senza_trattamenti) & " , trattamenti_cte.Data_Ultimo_Trattamento) as Data_Disponibilita ")
            stb.AppendLine("    , trattamenti_cte.Data_Sospensione_Carne")
            stb.AppendLine("    , trattamenti_cte.Data_Sospensione_Latte")
            stb.AppendLine("    , CAST(tp.DataInizioTrattamento AS DATE) AS Data_Ultimo_Trattamento_Final ")

            If MostraGGPrimoCaricamento Then
                stb.AppendLine("   , DATEDIFF(day, DataPrimoCaricamento.Data_Movimento,   " & Agro_SQL_SaveDate(Data, False) & "  ) as giorni_stalla_primo_caricamento ")
                stb.AppendLine("   , DataPrimoCaricamento.Data_Movimento as data_primo_caricamento ")
            End If

            If MostraAnomalie Then
                stb.AppendLine("   , COALESCE(anomalie_ct.Anomalie, '') as Anomalie ")
                stb.AppendLine("   , COALESCE(anomalie_ct.Anomalie_Str, '') as Anomalie_Str ")
            End If

            stb.AppendLine(" FROM #giac_cte giac_cte ")
            stb.AppendLine("  INNER Join Zoo_Animali (NOLOCK) ON Zoo_Animali.Cod_Progetto = giac_cte.Cod_Animale  ")
            stb.AppendLine("  INNER JOIN Zoo_Animali_Distinte (NOLOCK) ON Zoo_Animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale  ")
            stb.AppendLine("                                          AND CAST(Zoo_Animali_Distinte.Validita_Inizio as datetime) <=     " & Agro_SQL_SaveDate(Data, False) & "  ")
            stb.AppendLine("                                          And CAST(Zoo_Animali_Distinte.Validita_Fine as datetime) >=     " & Agro_SQL_SaveDate(Data, False) & " ")
            stb.AppendLine("  INNER JOIN Zoo_AnimalixStati_Accrescimento ON Zoo_AnimalixStati_Accrescimento.Cod_Progetto  = Zoo_Animali.Cod_Progetto  ")
            stb.AppendLine("                                            AND Zoo_AnimalixStati_Accrescimento.PIVA  = Zoo_Animali.PIVA ")
            stb.AppendLine("                                            AND Zoo_AnimalixStati_Accrescimento.sa_cod  = Zoo_Animali.sa_cod ")
            stb.AppendLine("                                            AND CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as datetime) <=     " & Agro_SQL_SaveDate(Data, False) & "  ")
            stb.AppendLine("                                            And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as datetime) >=    " & Agro_SQL_SaveDate(Data, False) & "  ")
            stb.AppendLine(" INNER JOIN Zoo_Animali_Lista_Stati_Accrescimento (NOLOCK) ON Zoo_AnimalixStati_Accrescimento.GEN_COD = Zoo_Animali_Lista_Stati_Accrescimento.GEN_COD  ")
            stb.AppendLine(" 												AND Zoo_AnimalixStati_Accrescimento.SPE_COD = Zoo_Animali_Lista_Stati_Accrescimento.SPE_COD  ")
            stb.AppendLine(" 												AND Zoo_AnimalixStati_Accrescimento.STATO_COD = Zoo_Animali_Lista_Stati_Accrescimento.STATO_COD  ")
            stb.AppendLine(" 												AND Zoo_AnimalixStati_Accrescimento.TIPO_COD = Zoo_Animali_Lista_Stati_Accrescimento.TIPO_COD  ")
            stb.AppendLine(" INNER JOIN Lista_Specie_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Specie_Animali.SPE_COD  ")
            stb.AppendLine(" INNER JOIN Lista_Razze_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD And Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD  ")
            stb.AppendLine("  INNER JOIN Lista_Razze_Animali razza_madre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_madre.GEN_COD And Zoo_Animali.SPE_COD = razza_madre.SPE_COD And Zoo_Animali.Razza_Madre = razza_madre.RAZ_COD   ")
            stb.AppendLine("  INNER JOIN Lista_Razze_Animali razza_padre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_padre.GEN_COD And Zoo_Animali.SPE_COD = razza_padre.SPE_COD And Zoo_Animali.Razza_Padre = razza_padre.RAZ_COD   ")
            stb.AppendLine(" INNER JOIN Zoo_Animali_Lista_Tipi (NOLOCK) ON Zoo_Animali.GEN_COD = Zoo_Animali_Lista_Tipi.GEN_COD And Zoo_Animali.SPE_COD = Zoo_Animali_Lista_Tipi.SPE_COD And Zoo_Animali.TIPO_COD = Zoo_Animali_Lista_Tipi.TIPO_COD  ")
            stb.AppendLine(" INNER JOIN Lista_IndirizziProd_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD And Zoo_Animali.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD  ")
            stb.AppendLine(" LEFT  JOIN Contatti (NOLOCK) ON Zoo_Animali.CF_Fornitore = Contatti.Cod_Contatto AND Zoo_Animali.Piva = Contatti.Piva ")
            stb.AppendLine(" LEFT JOIN #trattamenti_cte trattamenti_cte ON giac_cte.Cod_Animale = trattamenti_cte.Cod_Animale")
            stb.AppendLine("  LEFT JOIN Lista_Patologie ON Lista_Patologie.Patologia_Cod = Zoo_Animali.Id_Patologia  ")

            ' OUTER APPLY per Data_Ultimo_Trattamento
            stb.AppendLine(" LEFT JOIN #TrattamentiProgrammati tp ON giac_cte.Cod_Animale = tp.Cod_Animale")

            If MostraGGPrimoCaricamento Then
                stb.AppendLine(" INNER JOIN #DataPrimoCaricamento DataPrimoCaricamento ON Zoo_Animali.Matricola = DataPrimoCaricamento.Matricola ")
            End If
            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON giac_cte.Piva = p.piva AND p.Sa_Cod = giac_cte.Sa_Cod AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            End If
            If MostraAnomalie Then
                stb.AppendLine(" LEFT JOIN #anomalie_ct anomalie_ct ON anomalie_ct.Cod_Animale = giac_cte.Cod_Animale ")
            End If
            stb.AppendLine("  WHERE 1=1 ")
            If Piva <> "" Then
                stb.AppendLine("  And giac_cte.Piva = '" & Agro_SQL_SaveText(Piva) & "'      ")
            End If
            If Sa_Cod <> 0 Then
                stb.AppendLine("  And giac_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If Raggruppamento_Cod <> 0 Then
                stb.AppendLine("  And giac_cte.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "      ")
            End If

            stb.AppendLine(" 	AND Zoo_Animali.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")
            If LivelloCompatibilita(objParametri) >= 150 Then
                stb.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
            End If

            stb.AppendLine("DROP TABLE #agn_cte ")
            stb.AppendLine("DROP TABLE #giac_cte ")
            stb.AppendLine("DROP TABLE #Categorie_Farmaci_Con_Semplificati ")
            stb.AppendLine("DROP TABLE #sospensioneCarne_cte ")
            stb.AppendLine("DROP TABLE #sospensioneLatte_cte ")
            stb.AppendLine("DROP TABLE #trattamenti_cte ")
            stb.AppendLine("DROP TABLE #TrattamentiProgrammati ")
            If MostraGGPrimoCaricamento Then
                stb.AppendLine("DROP TABLE #DataPrimoCaricamento ")
                stb.AppendLine("DROP TABLE #ZooMatricoleList ")
            End If
            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiTrattamentiCorrenti(Piva As String,
                                      Sa_Cod As Integer,
                                      STA_NUM As Integer,
                                      Data As DateTime,
                                      ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.LeggiTrattamentiCorrenti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("-- Creazione farmCategorieS_cte --")
            stb.AppendLine(crea_farmCategorieS_cte(True))
            stb.AppendLine()

            stb.AppendLine("-- Check sui protocolli in corso ")
            stb.AppendLine("SELECT DISTINCT ")
            stb.AppendLine("    --rzDst.IdRicetta, rzDst.IdAgenda, rzDst.IdMov, rzDst.IdDettaglio, rzDst.IdDestinazione, ")
            stb.AppendLine("    --rzDtt.Pro_Cod, rzDtt.ProdottoAic, ")
            stb.AppendLine("    --rzAg.DataInizioTrattamento AS Somm_Inizio, rzAg.DataFineTrattamento AS Somm_Fine, rzAg.Intervallo_Somm, rzAg.Note, ")
            stb.AppendLine("    IIF(farmCatSim_cte.FarmCatS_Id = 2, 1, 0) AS Antibiotico, ")
            stb.AppendLine("    IIF(farmCatSim_cte.FarmCatS_Id = 6, 1, 0) AS Antinfiammatorio, ")
            stb.AppendLine("    zooA.Cod_Progetto AS Cod_Animale, zooA.Matricola ")
            stb.AppendLine("FROM Ricette_Zoo_Destinazioni rzDst ")
            stb.AppendLine("INNER JOIN Ricette_Zoo_Dettagli rzDtt ON rzDst.IdDettaglio = rzDtt.IdDettaglio ")
            stb.AppendLine("INNER JOIN Ricette_Zoo_Agenda rzAg ON rzDtt.IdAgenda = rzAg.IdAgenda ")
            stb.AppendLine("INNER JOIN Ricette_Zoo rz ON rzAg.IdRicetta = rz.IdRicetta ")
            stb.AppendLine("LEFT JOIN Ricette_ZooxAgenda rzxAg ON rzAg.IdAgenda = rzxAg.Id_RigaRicetta ")
            stb.AppendLine("INNER JOIN Zoo_Animali zooA ON rzDst.CodAnimale = zooA.Cod_Progetto ")
            stb.AppendLine("LEFT JOIN #farmCategorieS_cte farmCatSim_cte ON rzDtt.ProdottoAic = farmCatSim_cte.FamigliaAIC ")
            stb.AppendLine("WHERE 1=1 ")
            stb.AppendLine("    AND rzxAg.Id_Agenda IS NULL ")
            stb.AppendLine("    AND rzAg.DataInizioTrattamento >= " + Agro_SQL_SaveDateTime(Data.Date) + " ")

            If Piva <> "" Then
                stb.AppendLine("    AND rz.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine("    AND rz.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine("    AND rz.Sta_Num = " & Agro_SQL_SaveNum(STA_NUM) & " ")
            End If

            stb.AppendLine(" UNION ")

            stb.AppendLine("-- Check sui trattamenti eseguiti ")
            stb.AppendLine("SELECT DISTINCT ")
            stb.AppendLine("    IIF(farmCatSim_cte.FarmCatS_Id = 2, 1, 0) AS Antibiotico, ")
            stb.AppendLine("    IIF(farmCatSim_cte.FarmCatS_Id = 6, 1, 0) AS Antinfiammatorio, ")
            stb.AppendLine("    zooA.Cod_Progetto AS Cod_Animale, zooA.Matricola ")
            stb.AppendLine("FROM Agenda ag ")
            stb.AppendLine("INNER JOIN Movimenti mov ON ag.Id_Agenda = mov.Id_Agenda ")
            stb.AppendLine("INNER JOIN Movimenti_dettagli mdett ON ag.Id_Agenda = mdett.Id_Agenda AND mov.Id_Mov = mdett.Id_Mov ")
            stb.AppendLine("INNER JOIN Mov_Destinazioni mdest ON ag.Id_Agenda = mdest.Id_Agenda AND mov.Id_Mov = mdest.Id_Mov AND mdett.Id_Mov_Det = mdest.Id_Mov_Det ")
            stb.AppendLine("INNER JOIN Zoo_Animali zooA ON mdest.Id_Destinazione = zooA.Cod_Progetto ")
            stb.AppendLine("INNER JOIN Farmaci farm ON mdett.Pro_Cod = farm.Farm_Cod ")
            stb.AppendLine("LEFT JOIN #farmCategorieS_cte farmCatSim_cte ON LEFT(farm.AIC, 6) = farmCatSim_cte.FamigliaAIC ")
            stb.AppendLine("WHERE 1=1 ")
            stb.AppendLine("    AND ag.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_CUREMEDICAMENTI_ANIMALI) & " ")
            stb.AppendLine("    AND mov.Cau_Mov = '" & Agro_SQL_SaveText(CAU_TRATTAMENTO_ZOO) & "' ")
            stb.AppendLine("    AND ag.Validita_Fine >= " + Agro_SQL_SaveDateTime(Data.Date) + " ")

            If Piva <> "" Then
                stb.AppendLine("    AND ag.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine("    AND ag.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine("    AND ag.Sta_Num = " & Agro_SQL_SaveNum(STA_NUM) & " ")
            End If

            stb.AppendLine("-- DROP CTE --")
            stb.AppendLine("DROP TABLE #farmCategorieS_cte ")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Private Function crea_farmCategorieS_cte(ByVal useFamigliaAIC As Boolean) As String
        Dim stb As New StringBuilder()

        stb.AppendLine("SELECT ")

        If useFamigliaAIC Then
            stb.AppendLine("    DISTINCT LEFT(farm.AIC, 6) AS FamigliaAIC, ")
        Else
            stb.AppendLine("    farm.AIC, farm.Farm_Cod,  farm.Confezione, farm.Codice_GTIN, ")
        End If

        stb.AppendLine("    farm.Denominazione, farm.ModalitaPrescrizione, ")
        stb.AppendLine("    farmCat.Id AS FarmCat_Id, farmCat.Categoria_Codice AS FarmCat_Cod, farmCat.Categoria_Descrizione AS FarmCat_Des, ")
        stb.AppendLine("    farmCatS.ID AS FarmCatS_Id, farmCatS.Descrizione AS FarmCatS_Des ")
        stb.AppendLine("INTO #farmCategorieS_cte ")
        stb.AppendLine("FROM Farmaci farm ")
        stb.AppendLine("INNER JOIN FarmacixCategorie fxc ON farm.Farm_Cod = fxc.Farm_Cod ")
        stb.AppendLine("INNER JOIN Farmaci_Categorie farmCat ON fxc.Cat_Cod = farmCat.ID ")
        stb.AppendLine("INNER JOIN Farmaci_CategoriexFarmaci_Categorie_Semplificate fcxfcs ON farmCat.ID = fcxfcs.ID_Categoria ")
        stb.AppendLine("INNER JOIN Farmaci_Categorie_Semplificate farmCatS ON fcxfcs.ID_Categoria_Semplificata = farmCatS.ID  ")


        Return stb.ToString()

    End Function


    Private Function crea_agn_cte(Piva As String,
                                      Sa_Cod As Integer,
                                      STA_NUM As Integer,
                                      Raggruppamento_Cod As Integer,
                                      Cod_Animale As Integer,
                                      Data As DateTime,
                                      giorni_senza_trattamenti As Integer,
                                      ByVal Filtro_Visibilita_Utente As Boolean,
                                      ByVal listCod_Animali As List(Of Integer),
                                      ByVal IDTestataTemp As Integer,
                                      ByRef objParametri As AgronicaCoreParametri) As String
        Dim stb As New StringBuilder

        stb.AppendLine("  select a.Id_Agenda,  ")
        stb.AppendLine(" 	a.PIVA,  ")
        stb.AppendLine(" 	md.Cod_Progetto, ")
        stb.AppendLine(" 	md.Lotto, ")
        stb.AppendLine(" 	md.Udm_Cod, ")
        stb.AppendLine(" 	UnitaMisura.Udm_Des, ")
        stb.AppendLine(" 	UnitaMisura.Udm_Sim, ")
        stb.AppendLine(" 	mdes.Sa_Cod, ")
        stb.AppendLine(" 	mdes.Id_Destinazione, ")
        stb.AppendLine(" 	mdes.Tipo_Destinazione, ")
        stb.AppendLine(" 	mdes.Qta, ")
        stb.AppendLine(" 	md.Id_Mov, ")
        stb.AppendLine(" 	md.Id_Mov_Det, ")
        stb.AppendLine(" 	m.Cau_Mov, ")
        stb.AppendLine(" 	i.rag_soc, ")
        stb.AppendLine(" 	ic.val_cod as cuaa, ")
        stb.AppendLine(" 	Centri_Aziendali.sa_nome ")
        stb.AppendLine(" 	INTO #agn_cte  ")
        stb.AppendLine(" 	from agenda a (NOLOCK)  ")
        stb.AppendLine(" 	inner join imprese i (NOLOCK) on i.PIVA = a.PIVA ")
        stb.AppendLine(" 	inner join imprese_codici ic (NOLOCK) on i.piva = ic.piva and ic.id_cod = 1010 ")
        stb.AppendLine(" 	inner join Movimenti m (NOLOCK) on	a.Id_Agenda = m.Id_Agenda  ")
        stb.AppendLine(" 	inner join Movimenti_dettagli md (NOLOCK) on md.Id_Agenda = m.Id_Agenda  AND md.Id_Mov = m.Id_Mov  ")
        stb.AppendLine(" 	inner join mov_destinazioni mdes (NOLOCK) on mdes.Id_Agenda = md.Id_Agenda and mdes.Id_Mov = md.Id_Mov and mdes.Id_Mov_Det = md.Id_Mov_Det ")
        stb.AppendLine(" 	inner join Centri_Aziendali (NOLOCK) ON mdes.Piva = Centri_Aziendali.Piva and mdes.Sa_Cod=Centri_Aziendali.sa_cod  ")
        stb.AppendLine(" 	INNER Join UnitaMisura (NOLOCK) ON UnitaMisura.Udm_Cod = md.Udm_Cod ")
        If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
            stb.AppendLine("	INNER Join __Tmp_Agenda (NOLOCK) ON md.Cod_Progetto = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
        End If
        If Piva = "" And Filtro_Visibilita_Utente Then
            stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON a.Piva = p.piva AND p.Entita_Cod = 1 AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
        End If
        stb.AppendLine(" 	where  1 = 1 ")
        If Piva <> "" Then
            stb.AppendLine(" And a.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
        End If

        If Sa_Cod <> 0 Then
            stb.AppendLine(" And mdes.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
        End If

        If Cod_Animale <> 0 Then
            stb.AppendLine(" AND md.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
        End If
        stb.AppendLine(" 	and a.lav_Cod >= 3000 And a.lav_cod < 4000 ")
        stb.AppendLine(" 	And m.Cau_Mov In ('7300', '7350', '3700', '3750')     ")
        stb.AppendLine(" 	And m.Data_Movimento >=  " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & "     ")
        stb.AppendLine(" 	And m.Data_Movimento <=  " & Agro_SQL_SaveDateTime(Data) & "  ")
        stb.AppendLine(" 	And md.Elem_Cod = 300  ")
        stb.AppendLine(" 	And md.Jolly_Int = 0  ")
        stb.AppendLine(" 	And mdes.Tipo_Destinazione IN (15, 21)  ")

        Return stb.ToString
    End Function


    Private Function crea_Categorie_Farmaci_Con_Semplificati() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" SELECT ")
        stb.AppendLine(" 	Farmaci_Categorie.ID,  ")
        stb.AppendLine(" 	Farmaci_Categorie.Categoria_Codice,  ")
        stb.AppendLine(" 	Farmaci_Categorie.Categoria_Descrizione, ")
        stb.AppendLine(" 	CONCAT('|' ,STRING_AGG(Farmaci_Categorie_Semplificate.ID, '|') , '|') as Categorie_Semplificate_Cod, ")
        stb.AppendLine(" 	COALESCE(STRING_AGG(Farmaci_Categorie_Semplificate.Descrizione, ','), '') as Categorie_Semplificate_Des ")
        stb.AppendLine(" INTO #Categorie_Farmaci_Con_Semplificati ")
        stb.AppendLine(" FROM Farmaci_Categorie ")
        stb.AppendLine(" LEFT JOIN Farmaci_CategoriexFarmaci_Categorie_Semplificate ON Farmaci_Categorie.ID = Farmaci_CategoriexFarmaci_Categorie_Semplificate.ID_Categoria ")
        stb.AppendLine(" LEFT JOIN Farmaci_Categorie_Semplificate ON Farmaci_CategoriexFarmaci_Categorie_Semplificate.ID_Categoria_Semplificata = Farmaci_Categorie_Semplificate.ID ")
        stb.AppendLine(" GROUP BY Farmaci_Categorie.ID, Farmaci_Categorie.Categoria_Codice, Farmaci_Categorie.Categoria_Descrizione ")

        Return stb.ToString
    End Function

    Private Function crea_sospensioneCarne_cte(Piva As String, Filtro_Visibilita_Utente As Boolean, ByRef objParametri As AgronicaCoreParametri) As String
        Dim stb As New StringBuilder

        stb.AppendLine("    SELECT Mov_Destinazioni.Id_Destinazione as Cod_Animale,")
        stb.AppendLine("        MAX(IIF(mdt_Carne.Extra_Int > 0, DATEADD(DAY, mdt_Carne.Extra_Int, Agenda.Validita_Fine), '')) AS Data_Sospensione_Carne ")
        stb.AppendLine("    INTO #sospensioneCarne_cte ")
        stb.AppendLine("    FROM Agenda  (NOLOCK)  ")
        stb.AppendLine("    JOIN Movimenti  (NOLOCK) ON Agenda.Id_Agenda = Movimenti.Id_Agenda AND Movimenti.Cau_Mov = '" & CAU_TRATTAMENTO_ZOO & "'")
        stb.AppendLine("    JOIN Movimenti_Dettagli  (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
        stb.AppendLine("        AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")
        stb.AppendLine("    JOIN Mov_Destinazioni  (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
        stb.AppendLine("        AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
        stb.AppendLine("        AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
        stb.AppendLine("        AND Mov_Destinazioni.Tipo_Destinazione = " & TIPO_DESTINAZIONE_ANIMALE & " ")
        stb.AppendLine("    JOIN Mov_Dettaglio_Tecnico AS mdt_Carne  (NOLOCK) ON mdt_Carne.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
        stb.AppendLine("        AND mdt_Carne.Id_Mov = Movimenti_Dettagli.Id_Mov ")
        stb.AppendLine("        AND mdt_Carne.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")
        If Piva = "" And Filtro_Visibilita_Utente Then
            stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON Agenda.Piva = p.piva AND p.Entita_Cod = 1 AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
        End If
        stb.AppendLine("    WHERE mdt_Carne.Dett_Cod = 1 ")

        If Piva <> "" Then
            stb.AppendLine(" And Agenda.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
        End If

        stb.AppendLine("    GROUP BY Mov_Destinazioni.Id_Destinazione ")


        Return stb.ToString
    End Function

    Private Function crea_sospensioneLatte_cte(Piva As String, Filtro_Visibilita_Utente As Boolean, ByRef objParametri As AgronicaCoreParametri) As String
        Dim stb As New StringBuilder

        stb.AppendLine("    SELECT Mov_Destinazioni.Id_Destinazione as Cod_Animale,")
        stb.AppendLine("        MAX(IIF(mdt_Latte.Extra_Int > 0, DATEADD(DAY, mdt_Latte.Extra_Int, Agenda.Validita_Fine), '')) AS Data_Sospensione_Latte ")
        stb.AppendLine("    INTO #sospensioneLatte_cte ")
        stb.AppendLine("    FROM Agenda  (NOLOCK) ")
        stb.AppendLine("    JOIN Movimenti (NOLOCK)  ON Agenda.Id_Agenda = Movimenti.Id_Agenda AND Movimenti.Cau_Mov = '" & CAU_TRATTAMENTO_ZOO & "'")
        stb.AppendLine("    JOIN Movimenti_Dettagli  (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
        stb.AppendLine("        AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")
        stb.AppendLine("    JOIN Mov_Destinazioni  (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
        stb.AppendLine("        AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
        stb.AppendLine("        AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
        stb.AppendLine("        AND Mov_Destinazioni.Tipo_Destinazione = " & TIPO_DESTINAZIONE_ANIMALE & " ")
        stb.AppendLine("    JOIN Mov_Dettaglio_Tecnico AS mdt_Latte  (NOLOCK) ON mdt_Latte.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
        stb.AppendLine("        AND mdt_Latte.Id_Mov = Movimenti_Dettagli.Id_Mov ")
        stb.AppendLine("        AND mdt_Latte.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")
        If Piva = "" And Filtro_Visibilita_Utente Then
            stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON Agenda.Piva = p.piva AND p.Entita_Cod = 1 AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
        End If
        stb.AppendLine("    WHERE mdt_Latte.Dett_Cod = 2 ")

        If Piva <> "" Then
            stb.AppendLine(" And Agenda.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
        End If

        stb.AppendLine("    GROUP BY Mov_Destinazioni.Id_Destinazione  ")

        Return stb.ToString
    End Function

    Private Function crea_trattamenti_cte(Piva As String,
                                          Sa_Cod As Integer,
                                          STA_NUM As Integer,
                                          Raggruppamento_Cod As Integer,
                                          Cod_Animale As Integer,
                                          Data As DateTime,
                                          giorni_senza_trattamenti As Integer,
                                          ByVal listCod_Animali As List(Of Integer),
                                          ByVal Farm_Cat_List As Integer(),
                                          ByVal Farm_Cat_Sempl_List As Integer(),
                                          ByVal IDTestataTemp As Integer,
                                          Filtro_Visibilita_Utente As Boolean,
                                          ByRef objParametri As AgronicaCoreParametri) As String

        Dim stb As New StringBuilder

        stb.AppendLine(" 	SELECT Mov_Destinazioni.Id_Destinazione as Cod_Animale, ")
        stb.AppendLine(" 	MAX(Agenda.Validita_Fine) as Data_Ultimo_Trattamento, ")
        stb.AppendLine(" 	sospensioneCarne_cte.Data_Sospensione_Carne AS Data_Sospensione_Carne, ")
        stb.AppendLine(" 	sospensioneLatte_cte.Data_Sospensione_Latte AS Data_Sospensione_Latte ")
        stb.AppendLine("    INTO #trattamenti_cte ")
        stb.AppendLine(" 	FROM Agenda (NOLOCK) ")
        stb.AppendLine(" 	JOIN Movimenti  (NOLOCK) ON Agenda.Id_Agenda = Movimenti.Id_Agenda AND Movimenti.Cau_Mov = '" & CAU_TRATTAMENTO_ZOO & "'")
        stb.AppendLine(" 	JOIN Movimenti_Dettagli  (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
        stb.AppendLine(" 	    AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")
        stb.AppendLine(" 	JOIN Mov_Destinazioni  (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
        stb.AppendLine(" 	    AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
        stb.AppendLine(" 	    AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
        stb.AppendLine(" 	    AND Mov_Destinazioni.Tipo_Destinazione = " & TIPO_DESTINAZIONE_ANIMALE & " ")
        stb.AppendLine(" 	LEFT JOIN #sospensioneCarne_cte sospensioneCarne_cte ON Mov_Destinazioni.Id_Destinazione = sospensioneCarne_cte.Cod_Animale")
        stb.AppendLine(" 	LEFT JOIN #sospensioneLatte_cte sospensioneLatte_cte ON Mov_Destinazioni.Id_Destinazione = sospensioneLatte_cte.Cod_Animale")
        stb.AppendLine(" 	JOIN Farmaci  (NOLOCK) ON Movimenti_Dettagli.Pro_Cod = Farmaci.Farm_Cod")
        stb.AppendLine(" 	JOIN FarmacixCategorie  (NOLOCK) ON FarmacixCategorie.Farm_Cod = Farmaci.Farm_Cod")
        stb.AppendLine(" 	JOIN #Categorie_Farmaci_Con_Semplificati Categorie_Farmaci_Con_Semplificati ON FarmacixCategorie.Cat_Cod = Categorie_Farmaci_Con_Semplificati.ID ")
        If Piva = "" And Filtro_Visibilita_Utente Then
            stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON Agenda.Piva = p.piva AND p.Entita_Cod = 1 AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
        End If
        If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
            stb.AppendLine("	INNER Join __Tmp_Agenda (NOLOCK) ON Mov_Destinazioni.id_Destinazione = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
        End If
        stb.AppendLine(" 	WHERE 1 = 1")

        If Piva <> "" Then
            stb.AppendLine(" And Agenda.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
        End If

        If Farm_Cat_List IsNot Nothing AndAlso Farm_Cat_List.Length > 0 Then
            stb.AppendLine(" And Categorie_Farmaci_Con_Semplificati.ID IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", Farm_Cat_List)) & ") ")
        End If

        If Farm_Cat_Sempl_List IsNot Nothing AndAlso Farm_Cat_Sempl_List.Length > 0 Then
            stb.AppendLine(" AND ( ")
            Dim i = 0
            For Each cat_sempl In Farm_Cat_Sempl_List
                If i <> 0 Then
                    stb.Append(" OR ")
                End If
                stb.Append(" Categorie_Farmaci_Con_Semplificati.Categorie_Semplificate_Cod like '%|" & cat_sempl & "|%' ")
                i = i + 1
            Next
            stb.Append(" ) ")
        End If

        stb.AppendLine(" 	AND CAST(Agenda.Validita_Inizio as Date) <= " & Agro_SQL_SaveDate(Data) & " ")
        stb.AppendLine(" 	AND CAST(Agenda.Validita_Fine as Date) >= DATEADD(day, -" & giorni_senza_trattamenti & ", " & Agro_SQL_SaveDate(Data) & ")")
        stb.AppendLine(" 	GROUP BY Mov_Destinazioni.Id_Destinazione, sospensioneCarne_cte.Data_Sospensione_Carne, sospensioneLatte_cte.Data_Sospensione_Latte ")

        Return stb.ToString
    End Function

    Private Function crea_ultimoProtocolloInCorso_cte() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" 	SELECT Cod_Animale, MAX(Ricette_Zoo_Agenda.DataInizioTrattamento) as DataInizioTrattamento   ")
        stb.AppendLine(" 	INTO #TrattamentiProgrammati   ")
        stb.AppendLine(" 	FROM #giac_cte   ")
        stb.AppendLine(" 	JOIN Ricette_Zoo_Destinazioni ON #giac_cte.cod_animale = Ricette_Zoo_Destinazioni.CodAnimale   ")
        stb.AppendLine(" 	JOIN Ricette_Zoo_Agenda ON Ricette_Zoo_Destinazioni.IdAgenda = Ricette_Zoo_Agenda.IdAgenda   ")
        stb.AppendLine(" 	JOIN Ricette_Zoo ON Ricette_Zoo_Agenda.IdRicetta = Ricette_Zoo.IdRicetta   ")
        stb.AppendLine(" 	LEFT JOIN Ricette_ZooxAgenda ON Ricette_Zoo_Agenda.IdAgenda = Ricette_ZooxAgenda.Id_RigaRicetta   ")
        stb.AppendLine($" 	WHERE Ricette_ZooxAgenda.Id_RigaRicetta IS NULL   ")
        stb.AppendLine($"     AND Ricette_Zoo.TipoCodice = {CInt(enum_TipoPrescrizione.Da_Protocollo_GIAS)}   ")
        stb.AppendLine(" 	GROUP BY Cod_Animale   ")

        Return stb.ToString()
    End Function

    Private Function crea_giac_cte(Piva As String,
                                   Sa_Cod As Integer,
                                   STA_NUM As Integer,
                                   Raggruppamento_Cod As Integer,
                                   Cod_Animale As Integer,
                                   Data As DateTime,
                                   giorni_senza_trattamenti As Integer,
                                   ByVal Filtro_Visibilita_Utente As Boolean,
                                   ByVal listCod_Animali As List(Of Integer),
                                   ByVal IDTestataTemp As Integer) As String
        Dim stb As New StringBuilder

        stb.AppendLine(" 	SELECT   ")
        stb.AppendLine("   agn_cte.Piva, agn_cte.Rag_Soc As Impresa,  ")
        stb.AppendLine("   agn_cte.cuaa, Stalla.BDN_Codice_Azienda,  ")
        stb.AppendLine("   agn_cte.Cod_Progetto AS Cod_Animale,  ")
        stb.AppendLine("   agn_cte.Lotto,  ")
        stb.AppendLine("   agn_cte.Udm_Cod,  ")
        stb.AppendLine("   agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
        stb.AppendLine("   agn_cte.Sa_Cod,  ")
        stb.AppendLine("   agn_cte.sa_nome,  ")
        stb.AppendLine("   agn_cte.Id_Destinazione,  ")
        stb.AppendLine("   agn_cte.Tipo_Destinazione,  ")
        stb.AppendLine("   Lista_AUSL.denominazione AS AUSL_DES,  ")
        stb.AppendLine("   COALESCE(Fabbricati.Fabbricato_Des, '') As STA_DES,  ")
        stb.AppendLine("   COALESCE(Fabbricati.Fabbricato_Cod, 0) As STA_NUM,  ")
        stb.AppendLine("   COALESCE(Stalla_Raggruppamenti.Raggruppamento_Des, '') as Raggruppamento_Des,  ")
        stb.AppendLine("   COALESCE(Stalla_Raggruppamenti.Raggruppamento_Cod, 0) as Raggruppamento_Cod,  ")
        stb.AppendLine("   Convert(INTEGER, SUM( Case When agn_cte.CAU_MOV In ('7350','3750')              ")
        stb.AppendLine("           THEN -(agn_cte.qta)             ")
        stb.AppendLine("           Else agn_cte.qta             ")
        stb.AppendLine("           End)) As Giacenza   ")
        stb.AppendLine("  INTO #giac_cte")
        stb.AppendLine("  From #agn_cte agn_cte  ")
        stb.AppendLine("  INNER Join Stalla_Raggruppamenti (NOLOCK) ON agn_cte.Sa_Cod = Stalla_Raggruppamenti.sa_cod  ")
        stb.AppendLine("                                 AND agn_cte.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod  ")
        stb.AppendLine("                                 AND agn_cte.Tipo_Destinazione = 21  ")
        stb.AppendLine("  INNER JOIN Fabbricati (NOLOCK) ON Stalla_Raggruppamenti.PIVA = Fabbricati.Piva AND Stalla_Raggruppamenti.sa_cod = Fabbricati.SA_COD AND Stalla_Raggruppamenti.STA_NUM = Fabbricati.Fabbricato_Cod ")
        stb.AppendLine("  INNER JOIN Stalla (NOLOCK) ON Stalla.PIVA = Fabbricati.Piva AND Stalla.sa_cod = Fabbricati.SA_COD AND Stalla.STA_NUM = Fabbricati.Fabbricato_Cod  ")
        stb.AppendLine("  INNER JOIN Indirizzi (NOLOCK) ON Fabbricati.Indirizzo_Cod = Indirizzi.cod_indirizzo ")
        stb.AppendLine("  LEFT JOIN IstatxDistretti (NOLOCK) ON IstatxDistretti.pro_cod = Indirizzi.pro_cod_istat AND IstatxDistretti.com_cod = Indirizzi.com_cod_istat ")
        stb.AppendLine("  LEFT JOIN Lista_Distretti (NOLOCK) ON Lista_Distretti.distretto_id = IstatxDistretti.distretto_id ")
        stb.AppendLine("  LEFT JOIN Lista_AUSL (NOLOCK) ON Lista_Distretti.asl_id = Lista_AUSL.asl_id ")
        stb.AppendLine("   where 1=1  ")
        If Piva <> "" Then
            stb.AppendLine(" And agn_cte.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "     ")
        End If
        If Sa_Cod <> 0 Then
            stb.AppendLine(" And agn_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
        End If
        If STA_NUM <> 0 Then
            stb.AppendLine(" AND Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
        End If
        stb.AppendLine("  GROUP BY agn_cte.Piva, agn_cte.Rag_Soc, agn_cte.cuaa, Stalla.BDN_Codice_Azienda, ")
        stb.AppendLine("  agn_cte.Cod_Progetto,  ")
        stb.AppendLine("  agn_cte.Lotto,  ")
        stb.AppendLine("  agn_cte.Udm_Cod, ")
        stb.AppendLine("  Fabbricati.Fabbricato_Des, ")
        stb.AppendLine("  Fabbricati.Fabbricato_Cod, ")
        stb.AppendLine("  agn_cte.Sa_Cod,  ")
        stb.AppendLine("  agn_cte.sa_nome, ")
        stb.AppendLine("  agn_cte.Id_Destinazione,  ")
        stb.AppendLine("  agn_cte.Tipo_Destinazione,  ")
        stb.AppendLine("  agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
        stb.AppendLine("  Lista_AUSL.denominazione,  ")
        stb.AppendLine("  Stalla_Raggruppamenti.Raggruppamento_Des,  ")
        stb.AppendLine("  Stalla_Raggruppamenti.Raggruppamento_Cod  ")
        stb.AppendLine("  HAVING Convert(INTEGER, SUM(Case When agn_cte.Cau_Mov In ('7350','3750') THEN -(agn_cte.qta) ELSE agn_cte.qta END)) <> 0   ")

        Return stb.ToString
    End Function

    Private Function crea_zooMatricoleList() As String
        Dim stb As New StringBuilder

        stb.AppendLine("  	SELECT Zoo_Animali.Matricola  ")
        stb.AppendLine("  	INTO #ZooMatricoleList  ")
        stb.AppendLine("  	FROM #giac_cte giac_cte   ")
        stb.AppendLine("  	INNER Join Zoo_Animali (NOLOCK) ON Zoo_Animali.Cod_Progetto = giac_cte.Cod_Animale    ")
        stb.AppendLine("  	WHERE 1=1   ")

        Return stb.ToString
    End Function

    Private Function crea_DataPrimoCaricamento(Piva As String) As String
        Dim stb As New StringBuilder

        stb.AppendLine("  SELECT ZooMatricoleList.Matricola, MIN(Movimenti.Data_Movimento) as Data_Movimento  ")
        stb.AppendLine("  INTO #DataPrimoCaricamento  ")
        stb.AppendLine("  FROM Agenda (NOLOCK)  ")
        stb.AppendLine("  JOIN Movimenti (NOLOCK) ON Agenda.ID_Agenda = Movimenti.ID_Agenda  ")
        stb.AppendLine("  JOIN Movimenti_Dettagli (NOLOCK) ON Movimenti.ID_Agenda = Movimenti_Dettagli.ID_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov AND Movimenti_dettagli.Elem_Cod = 300  ")
        stb.AppendLine("  JOIN Zoo_Animali (NOLOCK) ON Movimenti_Dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto  ")
        stb.AppendLine("  JOIN #ZooMatricoleList ZooMatricoleList  ON Zoo_Animali.Matricola = ZooMatricoleList.Matricola  ")
        stb.AppendLine("  WHERE 1 = 1  ")
        If Piva <> "" Then
            stb.AppendLine(" And Agenda.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
        End If
        stb.AppendLine("  GROUP BY ZooMatricoleList.Matricola  ")

        Return stb.ToString
    End Function


    Public Function Leggi_Capi_Senza_Trattamenti_CTE(Piva As String,
                                      Sa_Cod As Integer,
                                      STA_NUM As Integer,
                                      Raggruppamento_Cod As Integer,
                                      Cod_Animale As Integer,
                                      Data As DateTime,
                                      giorni_senza_trattamenti As Integer,
                                      ByVal Filtro_Visibilita_Utente As Boolean,
                                      ByVal listCod_Animali As List(Of Integer),
                                      ByVal Farm_Cat_List As Integer(),
                                      ByVal Farm_Cat_Sempl_List As Integer(),
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal MostraGGPrimoCaricamento As Boolean = False) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Capi_Senza_Trattamenti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            Dim IDTestataTemp As Integer = 0
            'CancellaRecordDaIDTestataTemp

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                For Each codAnimale In listCod_Animali
                    objTmp_AgendaTemp.Scrivi(IDTestataTemp, "", codAnimale, 0, objParametri)
                Next
            End If

            stb.Length = 0
            stb.AppendLine("  with  ")
            stb.AppendLine("  agn_cte as ")
            stb.AppendLine("  ( ")
            stb.AppendLine("  select a.Id_Agenda,  ")
            stb.AppendLine(" 	a.PIVA,  ")
            stb.AppendLine(" 	md.Cod_Progetto, ")
            stb.AppendLine(" 	md.Lotto, ")
            stb.AppendLine(" 	md.Udm_Cod, ")
            stb.AppendLine(" 	UnitaMisura.Udm_Des, ")
            stb.AppendLine(" 	UnitaMisura.Udm_Sim, ")
            stb.AppendLine(" 	mdes.Sa_Cod, ")
            stb.AppendLine(" 	mdes.Id_Destinazione, ")
            stb.AppendLine(" 	mdes.Tipo_Destinazione, ")
            stb.AppendLine(" 	mdes.Qta, ")
            stb.AppendLine(" 	md.Id_Mov, ")
            stb.AppendLine(" 	md.Id_Mov_Det, ")
            stb.AppendLine(" 	m.Cau_Mov, ")
            stb.AppendLine(" 	i.rag_soc, ")
            stb.AppendLine(" 	ic.val_cod as cuaa, ")
            stb.AppendLine(" 	Centri_Aziendali.sa_nome ")
            stb.AppendLine(" 	from agenda a (NOLOCK)  ")
            stb.AppendLine(" 	inner join imprese i (NOLOCK) on i.PIVA = a.PIVA ")
            stb.AppendLine(" 	inner join imprese_codici ic (NOLOCK) on i.piva = ic.piva and ic.id_cod = 1010 ")
            stb.AppendLine(" 	inner join Movimenti m (NOLOCK) on	a.Id_Agenda = m.Id_Agenda  ")
            stb.AppendLine(" 	inner join Movimenti_dettagli md (NOLOCK) on md.Id_Agenda = m.Id_Agenda  AND md.Id_Mov = m.Id_Mov  ")
            stb.AppendLine(" 	inner join mov_destinazioni mdes (NOLOCK) on mdes.Id_Agenda = md.Id_Agenda and mdes.Id_Mov = md.Id_Mov and mdes.Id_Mov_Det = md.Id_Mov_Det ")
            stb.AppendLine(" 	inner join Centri_Aziendali (NOLOCK) ON mdes.Piva = Centri_Aziendali.Piva and mdes.Sa_Cod=Centri_Aziendali.sa_cod  ")
            stb.AppendLine(" 	INNER Join UnitaMisura (NOLOCK) ON UnitaMisura.Udm_Cod = md.Udm_Cod ")
            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                stb.AppendLine("	INNER Join __Tmp_Agenda (NOLOCK) ON md.Cod_Progetto = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
            End If
            stb.AppendLine(" 	where  1 = 1 ")
            If Piva <> "" Then
                stb.AppendLine(" And a.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And mdes.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND md.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If
            stb.AppendLine(" 	and a.lav_Cod >= 3000 And a.lav_cod < 4000 ")
            stb.AppendLine(" 	And m.Cau_Mov In ('7300', '7350', '3700', '3750')     ")
            stb.AppendLine(" 	And m.Data_Movimento >=  " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & "     ")
            stb.AppendLine(" 	And m.Data_Movimento <=  " & Agro_SQL_SaveDateTime(Data) & "  ")
            stb.AppendLine(" 	And md.Elem_Cod = 300  ")
            stb.AppendLine(" 	And md.Jolly_Int = 0  ")
            stb.AppendLine(" 	And mdes.Tipo_Destinazione IN (15, 21)  ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Categorie_Farmaci_Con_Semplificati as (SELECT ")
            stb.AppendLine(" 	Farmaci_Categorie.ID,  ")
            stb.AppendLine(" 	Farmaci_Categorie.Categoria_Codice,  ")
            stb.AppendLine(" 	Farmaci_Categorie.Categoria_Descrizione, ")
            stb.AppendLine(" 	CONCAT('|' ,STRING_AGG(Farmaci_Categorie_Semplificate.ID, '|') , '|') as Categorie_Semplificate_Cod, ")
            stb.AppendLine(" 	COALESCE(STRING_AGG(Farmaci_Categorie_Semplificate.Descrizione, ','), '') as Categorie_Semplificate_Des ")
            stb.AppendLine(" FROM Farmaci_Categorie ")
            stb.AppendLine(" LEFT JOIN Farmaci_CategoriexFarmaci_Categorie_Semplificate ON Farmaci_Categorie.ID = Farmaci_CategoriexFarmaci_Categorie_Semplificate.ID_Categoria ")
            stb.AppendLine(" LEFT JOIN Farmaci_Categorie_Semplificate ON Farmaci_CategoriexFarmaci_Categorie_Semplificate.ID_Categoria_Semplificata = Farmaci_Categorie_Semplificate.ID ")
            stb.AppendLine(" GROUP BY Farmaci_Categorie.ID, Farmaci_Categorie.Categoria_Codice, Farmaci_Categorie.Categoria_Descrizione ")
            stb.AppendLine(" ) ")


            stb.AppendLine(", sospensioneCarne_cte AS (")
            stb.AppendLine("    SELECT Mov_Destinazioni.Id_Destinazione as Cod_Animale,")
            stb.AppendLine("        MAX(IIF(mdt_Carne.Extra_Int > 0, DATEADD(DAY, mdt_Carne.Extra_Int, Agenda.Validita_Fine), '')) AS Data_Sospensione_Carne ")
            'stb.AppendLine("        MAX(IIF(mdt_Carne.Extra_Int > 0, DATEADD(DAY, mdt_Carne.Extra_Int, Movimenti.Data_Movimento), '')) AS Data_Sospensione_Carne ")
            stb.AppendLine("    FROM Agenda  (NOLOCK)  ")
            stb.AppendLine("    JOIN Movimenti  (NOLOCK) ON Agenda.Id_Agenda = Movimenti.Id_Agenda AND Movimenti.Cau_Mov = '" & CAU_TRATTAMENTO_ZOO & "'")
            stb.AppendLine("    JOIN Movimenti_Dettagli  (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            stb.AppendLine("        AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")
            stb.AppendLine("    JOIN Mov_Destinazioni  (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            stb.AppendLine("        AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            stb.AppendLine("        AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            stb.AppendLine("        AND Mov_Destinazioni.Tipo_Destinazione = " & TIPO_DESTINAZIONE_ANIMALE & " ")
            stb.AppendLine("    JOIN Mov_Dettaglio_Tecnico AS mdt_Carne  (NOLOCK) ON mdt_Carne.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            stb.AppendLine("        AND mdt_Carne.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            stb.AppendLine("        AND mdt_Carne.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")
            stb.AppendLine("    WHERE mdt_Carne.Dett_Cod = 1 ")
            stb.AppendLine("    GROUP BY Mov_Destinazioni.Id_Destinazione), ")

            stb.AppendLine("sospensioneLatte_cte AS (")
            stb.AppendLine("    SELECT Mov_Destinazioni.Id_Destinazione as Cod_Animale,")
            stb.AppendLine("        MAX(IIF(mdt_Latte.Extra_Int > 0, DATEADD(DAY, mdt_Latte.Extra_Int, Agenda.Validita_Fine), '')) AS Data_Sospensione_Latte ")
            'stb.AppendLine("        MAX(IIF(mdt_Latte.Extra_Int > 0, DATEADD(DAY, mdt_Latte.Extra_Int, Movimenti.Data_Movimento), '')) AS Data_Sospensione_Latte ")
            stb.AppendLine("    FROM Agenda  (NOLOCK) ")
            stb.AppendLine("    JOIN Movimenti (NOLOCK)  ON Agenda.Id_Agenda = Movimenti.Id_Agenda AND Movimenti.Cau_Mov = '" & CAU_TRATTAMENTO_ZOO & "'")
            stb.AppendLine("    JOIN Movimenti_Dettagli  (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            stb.AppendLine("        AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")
            stb.AppendLine("    JOIN Mov_Destinazioni  (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            stb.AppendLine("        AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            stb.AppendLine("        AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            stb.AppendLine("        AND Mov_Destinazioni.Tipo_Destinazione = " & TIPO_DESTINAZIONE_ANIMALE & " ")
            stb.AppendLine("    JOIN Mov_Dettaglio_Tecnico AS mdt_Latte  (NOLOCK) ON mdt_Latte.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            stb.AppendLine("        AND mdt_Latte.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            stb.AppendLine("        AND mdt_Latte.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")
            stb.AppendLine("    WHERE mdt_Latte.Dett_Cod = 2 ")
            stb.AppendLine("    GROUP BY Mov_Destinazioni.Id_Destinazione),  ")

            stb.AppendLine(" trattamenti_cte AS (")
            stb.AppendLine(" 	SELECT Mov_Destinazioni.Id_Destinazione as Cod_Animale, ")
            stb.AppendLine(" 	MAX(Agenda.Validita_Fine) as Data_Ultimo_Trattamento, ")
            stb.AppendLine(" 	sospensioneCarne_cte.Data_Sospensione_Carne AS Data_Sospensione_Carne, ")
            stb.AppendLine(" 	sospensioneLatte_cte.Data_Sospensione_Latte AS Data_Sospensione_Latte ")
            stb.AppendLine(" 	FROM Agenda (NOLOCK) ")
            stb.AppendLine(" 	JOIN Movimenti  (NOLOCK) ON Agenda.Id_Agenda = Movimenti.Id_Agenda AND Movimenti.Cau_Mov = '" & CAU_TRATTAMENTO_ZOO & "'")
            stb.AppendLine(" 	JOIN Movimenti_Dettagli  (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            stb.AppendLine(" 	    AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")
            stb.AppendLine(" 	JOIN Mov_Destinazioni  (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            stb.AppendLine(" 	    AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            stb.AppendLine(" 	    AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            stb.AppendLine(" 	    AND Mov_Destinazioni.Tipo_Destinazione = " & TIPO_DESTINAZIONE_ANIMALE & " ")
            stb.AppendLine(" 	LEFT JOIN sospensioneCarne_cte ON Mov_Destinazioni.Id_Destinazione = sospensioneCarne_cte.Cod_Animale")
            stb.AppendLine(" 	LEFT JOIN sospensioneLatte_cte ON Mov_Destinazioni.Id_Destinazione = sospensioneLatte_cte.Cod_Animale")
            stb.AppendLine(" 	JOIN Farmaci  (NOLOCK) ON Movimenti_Dettagli.Pro_Cod = Farmaci.Farm_Cod")
            stb.AppendLine(" 	JOIN FarmacixCategorie  (NOLOCK) ON FarmacixCategorie.Farm_Cod = Farmaci.Farm_Cod")
            stb.AppendLine(" 	JOIN Categorie_Farmaci_Con_Semplificati ON FarmacixCategorie.Cat_Cod = Categorie_Farmaci_Con_Semplificati.ID ")
            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                stb.AppendLine("	INNER Join __Tmp_Agenda (NOLOCK) ON Mov_Destinazioni.id_Destinazione = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
            End If
            stb.AppendLine(" 	WHERE 1 = 1")
            If Piva <> "" Then
                stb.AppendLine(" And Agenda.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If
            If Farm_Cat_List IsNot Nothing AndAlso Farm_Cat_List.Length > 0 Then
                stb.AppendLine(" And Categorie_Farmaci_Con_Semplificati.ID IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", Farm_Cat_List)) & ") ")
            End If

            If Farm_Cat_Sempl_List IsNot Nothing AndAlso Farm_Cat_Sempl_List.Length > 0 Then
                stb.AppendLine(" AND ( ")
                Dim i = 0
                For Each cat_sempl In Farm_Cat_Sempl_List
                    If i <> 0 Then
                        stb.Append(" OR ")
                    End If
                    stb.Append(" Categorie_Farmaci_Con_Semplificati.Categorie_Semplificate_Cod like '%|" & cat_sempl & "|%' ")
                    i = i + 1
                Next
                stb.Append(" ) ")
            End If

            stb.AppendLine(" 	AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDateTime(Data) & " ")
            stb.AppendLine(" 	AND Movimenti.Data_Movimento >= DATEADD(day, -" & giorni_senza_trattamenti & ", " & Agro_SQL_SaveDateTime(Data) & ")")
            stb.AppendLine(" 	GROUP BY Mov_Destinazioni.Id_Destinazione, sospensioneCarne_cte.Data_Sospensione_Carne, sospensioneLatte_cte.Data_Sospensione_Latte ")
            stb.AppendLine(" ),")
            stb.AppendLine(" giac_cte as ( ")
            stb.AppendLine(" 	SELECT   ")
            stb.AppendLine("   agn_cte.Piva, agn_cte.Rag_Soc As Impresa,  ")
            stb.AppendLine("   agn_cte.cuaa, Stalla.BDN_Codice_Azienda,  ")
            stb.AppendLine("   agn_cte.Cod_Progetto AS Cod_Animale,  ")
            stb.AppendLine("   agn_cte.Lotto,  ")
            stb.AppendLine("   agn_cte.Udm_Cod,  ")
            stb.AppendLine("   agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
            stb.AppendLine("   agn_cte.Sa_Cod,  ")
            stb.AppendLine("   agn_cte.sa_nome,  ")
            stb.AppendLine("   agn_cte.Id_Destinazione,  ")
            stb.AppendLine("   agn_cte.Tipo_Destinazione,  ")
            stb.AppendLine("   Lista_AUSL.denominazione AS AUSL_DES,  ")
            stb.AppendLine("   COALESCE(Fabbricati.Fabbricato_Des, '') As STA_DES,  ")
            stb.AppendLine("   COALESCE(Fabbricati.Fabbricato_Cod, 0) As STA_NUM,  ")
            stb.AppendLine("   COALESCE(Stalla_Raggruppamenti.Raggruppamento_Des, '') as Raggruppamento_Des,  ")
            stb.AppendLine("   COALESCE(Stalla_Raggruppamenti.Raggruppamento_Cod, 0) as Raggruppamento_Cod,  ")
            stb.AppendLine("   Convert(INTEGER, SUM( Case When agn_cte.CAU_MOV In ('7350','3750')              ")
            stb.AppendLine("           THEN -(agn_cte.qta)             ")
            stb.AppendLine("           Else agn_cte.qta             ")
            stb.AppendLine("           End)) As Giacenza   ")
            stb.AppendLine("  From agn_cte  ")
            stb.AppendLine("  INNER Join Stalla_Raggruppamenti (NOLOCK) ON agn_cte.Sa_Cod = Stalla_Raggruppamenti.sa_cod  ")
            stb.AppendLine("                                 AND agn_cte.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod  ")
            stb.AppendLine("                                 AND agn_cte.Tipo_Destinazione = 21  ")
            stb.AppendLine("  INNER JOIN Fabbricati (NOLOCK) ON Stalla_Raggruppamenti.PIVA = Fabbricati.Piva AND Stalla_Raggruppamenti.sa_cod = Fabbricati.SA_COD AND Stalla_Raggruppamenti.STA_NUM = Fabbricati.Fabbricato_Cod ")
            stb.AppendLine("  INNER JOIN Stalla (NOLOCK) ON Stalla.PIVA = Fabbricati.Piva AND Stalla.sa_cod = Fabbricati.SA_COD AND Stalla.STA_NUM = Fabbricati.Fabbricato_Cod  ")
            stb.AppendLine("  INNER JOIN Indirizzi (NOLOCK) ON Fabbricati.Indirizzo_Cod = Indirizzi.cod_indirizzo ")
            stb.AppendLine("  LEFT JOIN IstatxDistretti (NOLOCK) ON IstatxDistretti.pro_cod = Indirizzi.pro_cod_istat AND IstatxDistretti.com_cod = Indirizzi.com_cod_istat ")
            stb.AppendLine("  LEFT JOIN Lista_Distretti (NOLOCK) ON Lista_Distretti.distretto_id = IstatxDistretti.distretto_id ")
            stb.AppendLine("  LEFT JOIN Lista_AUSL (NOLOCK) ON Lista_Distretti.asl_id = Lista_AUSL.asl_id ")
            stb.AppendLine("   where 1=1  ")
            If Piva <> "" Then
                stb.AppendLine(" And agn_cte.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "     ")
            End If
            If Sa_Cod <> 0 Then
                stb.AppendLine(" And agn_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If
            If STA_NUM <> 0 Then
                stb.AppendLine(" AND Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If
            stb.AppendLine("  GROUP BY agn_cte.Piva, agn_cte.Rag_Soc, agn_cte.cuaa, Stalla.BDN_Codice_Azienda, ")
            stb.AppendLine("  agn_cte.Cod_Progetto,  ")
            stb.AppendLine("  agn_cte.Lotto,  ")
            stb.AppendLine("  agn_cte.Udm_Cod, ")
            stb.AppendLine("  Fabbricati.Fabbricato_Des, ")
            stb.AppendLine("  Fabbricati.Fabbricato_Cod, ")
            stb.AppendLine("  agn_cte.Sa_Cod,  ")
            stb.AppendLine("  agn_cte.sa_nome, ")
            stb.AppendLine("  agn_cte.Id_Destinazione,  ")
            stb.AppendLine("  agn_cte.Tipo_Destinazione,  ")
            stb.AppendLine("  agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
            stb.AppendLine("  Lista_AUSL.denominazione,  ")
            stb.AppendLine("  Stalla_Raggruppamenti.Raggruppamento_Des,  ")
            stb.AppendLine("  Stalla_Raggruppamenti.Raggruppamento_Cod  ")
            stb.AppendLine("  HAVING Convert(INTEGER, SUM(Case When agn_cte.Cau_Mov In ('7350','3750') THEN -(agn_cte.qta) ELSE agn_cte.qta END)) <> 0   ")
            stb.AppendLine(" ) ")
            If MostraGGPrimoCaricamento Then
                stb.AppendLine(" ,  ")
                stb.AppendLine("  ZooMatricoleList as (  ")
                stb.AppendLine("  	SELECT Zoo_Animali.Matricola  ")
                stb.AppendLine("  	FROM giac_cte   ")
                stb.AppendLine("  	INNER Join Zoo_Animali (NOLOCK) ON Zoo_Animali.Cod_Progetto = giac_cte.Cod_Animale    ")
                stb.AppendLine("  	WHERE 1=1   ")
                stb.AppendLine("  ),   ")
                stb.AppendLine("  DataPrimoCaricamento as (  ")
                stb.AppendLine("  SELECT ZooMatricoleList.Matricola, MIN(Movimenti.Data_Movimento) as Data_Movimento  ")
                stb.AppendLine("  FROM Agenda (NOLOCK)  ")
                stb.AppendLine("  JOIN Movimenti (NOLOCK) ON Agenda.ID_Agenda = Movimenti.ID_Agenda  ")
                stb.AppendLine("  JOIN Movimenti_Dettagli (NOLOCK) ON Movimenti.ID_Agenda = Movimenti_Dettagli.ID_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov AND Movimenti_dettagli.Elem_Cod = 300  ")
                stb.AppendLine("  JOIN Zoo_Animali (NOLOCK) ON Movimenti_Dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto  ")
                stb.AppendLine("  JOIN ZooMatricoleList  ON Zoo_Animali.Matricola = ZooMatricoleList.Matricola  ")
                stb.AppendLine("  WHERE 1 = 1  ")
                If Piva <> "" Then
                    stb.AppendLine(" And Agenda.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
                End If
                stb.AppendLine("  GROUP BY ZooMatricoleList.Matricola  ")
                stb.AppendLine("  )  ")
            End If
            stb.AppendLine(" SELECT giac_cte.Piva + '_' + CAST(giac_cte.Sa_Cod as varchar(100)) + '_' + CAST(giac_cte.Cod_Animale as varchar(100)) as chiave,  ")
            stb.AppendLine(" giac_cte.*,  ")
            stb.AppendLine(" Lista_Specie_Animali.SPE_DES,  ")
            stb.AppendLine(" Lista_Razze_Animali.RAZ_DES,  ")
            stb.AppendLine(" Zoo_Animali_Lista_Tipi.Tipo_Des,  ")
            stb.AppendLine(" Lista_IndirizziProd_Animali.IPRO_DES,  ")
            stb.AppendLine(" Zoo_Animali_Distinte.Cod_Progetto,  ")
            stb.AppendLine(" Zoo_Animali_Distinte.Progetto_Des,  ")
            stb.AppendLine(" Zoo_Animali_Distinte.Progetto_Nome,  ")
            stb.AppendLine(" Zoo_Animali_Distinte.Codice_Distinta,  ")
            stb.AppendLine(" Zoo_AnimalixStati_Accrescimento.Stato_Cod,  ")
            stb.AppendLine(" Zoo_Animali_Lista_Stati_Accrescimento.Stato_Des,  ")
            stb.AppendLine(" Contatti.Rag_Soc,  ")
            stb.AppendLine(" Contatti.Cod_Contatto, ")
            stb.AppendLine("  Zoo_Animali.RAZ_COD,  ")
            stb.AppendLine("  	Zoo_Animali.IPRO_COD,  ")
            stb.AppendLine("  	Zoo_Animali.CF_Fornitore,  ")
            stb.AppendLine("    Zoo_Animali.Mat_Madre,   ")
            stb.AppendLine("    Zoo_Animali.Mat_Padre,   ")
            stb.AppendLine("    CASE Zoo_Animali.Metodo_Produzione WHEN 1 THEN 'Integrato' WHEN 2 THEN 'In Conversione' WHEN 3 THEN 'Biologico' END as Metodo_Produzione,   ")
            stb.AppendLine("    Zoo_Animali.Razza_Padre AS RazCod_Padre,   ")
            stb.AppendLine("    razza_madre.RAZ_DES as RazDes_Madre,   ")
            stb.AppendLine("    Zoo_Animali.Razza_Madre AS RazCod_Madre,   ")
            stb.AppendLine("    razza_padre.RAZ_DES as RazDes_Padre,   ")
            stb.AppendLine("    Zoo_Animali.Lotto_Fornitore, ")
            stb.AppendLine("    Zoo_Animali.Matricola,   ")
            stb.AppendLine("    Zoo_Animali.GEN_COD,   ")
            stb.AppendLine("    Zoo_Animali.SPE_COD,   ")
            stb.AppendLine("    Zoo_Animali.TIPO_COD,   ")
            stb.AppendLine("    Zoo_Animali.Progetto,   ")
            stb.AppendLine("    Zoo_Animali.Validita_Inizio,   ")
            stb.AppendLine("    Zoo_Animali.Validita_Fine,   ")
            stb.AppendLine("    Zoo_Animali.Nome,   ")
            stb.AppendLine("    Zoo_Animali.Sesso,   ")
            stb.AppendLine("    Zoo_Animali.Modello4_Ingresso,   ")
            stb.AppendLine("    Zoo_Animali.Modello4_Uscita,   ")
            stb.AppendLine("    ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ZOO_Animali.Username_Creazione), ZOO_Animali.Username_Creazione) AS Utente_Creazione,   ")
            stb.AppendLine("    ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ZOO_Animali.Username_Modifica), ZOO_Animali.Username_Modifica) AS Utente_Modifica,   ")
            stb.AppendLine("    Zoo_Animali.Data_Creazione,   ")
            stb.AppendLine("    Zoo_Animali.Data_Modifica,   ")
            stb.AppendLine("    Zoo_Animali.Dat_Nascita,   ")
            stb.AppendLine("    DATEDIFF(day, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") as eta_giorni_totali,")
            'stb.AppendLine("    IIF(DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, GETDATE()) , Zoo_Animali.DAT_NASCITA), GETDATE()) >= 0,")
            'stb.AppendLine(" 	   DATEDIFF(month, Zoo_Animali.Dat_Nascita, GETDATE()),")
            'stb.AppendLine(" 	   DATEDIFF(month, Zoo_Animali.Dat_Nascita, GETDATE()) -1) as eta_mesi,")
            stb.AppendLine(Calcolo_Mesi("Zoo_Animali.Dat_Nascita", Agro_SQL_SaveDate(Data, False)) & " as eta_mesi,")
            'stb.AppendLine("    IIF(DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, GETDATE()) , Zoo_Animali.DAT_NASCITA), GETDATE()) >= 0,")
            'stb.AppendLine(" 	   DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, GETDATE()) , Zoo_Animali.DAT_NASCITA), GETDATE()),")
            'stb.AppendLine(" 	   DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, Zoo_Animali.Dat_Nascita, GETDATE()) - 1, Zoo_Animali.DAT_NASCITA), GETDATE())) as eta_giorni,")
            stb.AppendLine(Calcolo_Giorni("Zoo_Animali.Dat_Nascita", Agro_SQL_SaveDate(Data, False)) & " as eta_giorni,")


            stb.AppendLine("    DATEDIFF(day, Zoo_Animali.Validita_Inizio, " & Agro_SQL_SaveDate(Data, False) & ") as giorni_totali_stalla,")
            stb.AppendLine(Calcolo_Mesi("Zoo_Animali.Validita_Inizio", Agro_SQL_SaveDate(Data, False)) & " as mesi_stalla,")
            stb.AppendLine(Calcolo_Giorni("Zoo_Animali.Validita_Inizio", Agro_SQL_SaveDate(Data, False)) & " as giorni_stalla,")

            stb.AppendLine("    Zoo_Animali.Id_Capo_BDN,   ")
            stb.AppendLine("    Zoo_Animali.CF_PROPRIETARIO,   ")
            stb.AppendLine("    Zoo_Animali.CF_DETENTORE,   ")
            stb.AppendLine("    Zoo_Animali.AUSL_AZI_NASCITA,   ")
            stb.AppendLine("    CASE WHEN Zoo_Animali.Id_Capo_BDN = 0 THEN 'No' ELSE 'Si' END AS FlagBDN,   ")
            stb.AppendLine("    Zoo_Animali.Certificato  ")
            stb.AppendLine("    , Zoo_Animali.Modello4_Ingresso_Numero  ")
            stb.AppendLine("    , Zoo_Animali.Modello4_Uscita_Numero  ")
            stb.AppendLine("    , Zoo_Animali.Modello4_Ingresso_Prenotazione  ")
            stb.AppendLine("    , Zoo_Animali.Modello4_Uscita_Prenotazione  ")
            stb.AppendLine("    , Zoo_Animali.Codice_Azienda_Uscita  ")
            stb.AppendLine("    , Zoo_Animali.Data_Documento_Ingresso  ")
            stb.AppendLine("    , Zoo_Animali.Data_Documento_Uscita ")
            stb.AppendLine("    , Zoo_Animali.Note ")
            stb.AppendLine("    , Lista_Patologie.Patologia_Des ")
            stb.AppendLine("    , trattamenti_cte.Data_Ultimo_Trattamento")
            stb.AppendLine("    , trattamenti_cte.Data_Sospensione_Carne")
            stb.AppendLine("    , trattamenti_cte.Data_Sospensione_Latte")
            If MostraGGPrimoCaricamento Then
                stb.AppendLine("   , DATEDIFF(day, DataPrimoCaricamento.Data_Movimento,   " & Agro_SQL_SaveDate(Data, False) & "  ) as giorni_stalla_primo_caricamento ")
                stb.AppendLine("   , DataPrimoCaricamento.Data_Movimento as data_primo_caricamento ")
            End If
            stb.AppendLine(" FROM giac_cte ")
            stb.AppendLine("  INNER Join Zoo_Animali (NOLOCK) ON Zoo_Animali.Cod_Progetto = giac_cte.Cod_Animale  ")
            stb.AppendLine("  INNER JOIN Zoo_Animali_Distinte (NOLOCK) ON Zoo_Animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale  ")
            stb.AppendLine("                                          AND CAST(Zoo_Animali_Distinte.Validita_Inizio as datetime) <=     " & Agro_SQL_SaveDate(Data, False) & "  ")
            stb.AppendLine("                                          And CAST(Zoo_Animali_Distinte.Validita_Fine as datetime) >=     " & Agro_SQL_SaveDate(Data, False) & " ")
            stb.AppendLine("  INNER JOIN Zoo_AnimalixStati_Accrescimento ON Zoo_AnimalixStati_Accrescimento.Cod_Progetto  = Zoo_Animali.Cod_Progetto  ")
            stb.AppendLine("                                            AND CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as datetime) <=     " & Agro_SQL_SaveDate(Data, False) & "  ")
            stb.AppendLine("                                            And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as datetime) >=    " & Agro_SQL_SaveDate(Data, False) & "  ")
            stb.AppendLine(" INNER JOIN Zoo_Animali_Lista_Stati_Accrescimento (NOLOCK) ON Zoo_AnimalixStati_Accrescimento.GEN_COD = Zoo_Animali_Lista_Stati_Accrescimento.GEN_COD  ")
            stb.AppendLine(" 												AND Zoo_AnimalixStati_Accrescimento.SPE_COD = Zoo_Animali_Lista_Stati_Accrescimento.SPE_COD  ")
            stb.AppendLine(" 												AND Zoo_AnimalixStati_Accrescimento.STATO_COD = Zoo_Animali_Lista_Stati_Accrescimento.STATO_COD  ")
            stb.AppendLine(" 												AND Zoo_AnimalixStati_Accrescimento.TIPO_COD = Zoo_Animali_Lista_Stati_Accrescimento.TIPO_COD  ")
            stb.AppendLine(" INNER JOIN Lista_Specie_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Specie_Animali.SPE_COD  ")
            stb.AppendLine(" INNER JOIN Lista_Razze_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD And Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD  ")
            stb.AppendLine("  INNER JOIN Lista_Razze_Animali razza_madre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_madre.GEN_COD And Zoo_Animali.SPE_COD = razza_madre.SPE_COD And Zoo_Animali.Razza_Madre = razza_madre.RAZ_COD   ")
            stb.AppendLine("  INNER JOIN Lista_Razze_Animali razza_padre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_padre.GEN_COD And Zoo_Animali.SPE_COD = razza_padre.SPE_COD And Zoo_Animali.Razza_Padre = razza_padre.RAZ_COD   ")
            stb.AppendLine(" INNER JOIN Zoo_Animali_Lista_Tipi (NOLOCK) ON Zoo_Animali.GEN_COD = Zoo_Animali_Lista_Tipi.GEN_COD And Zoo_Animali.SPE_COD = Zoo_Animali_Lista_Tipi.SPE_COD And Zoo_Animali.TIPO_COD = Zoo_Animali_Lista_Tipi.TIPO_COD  ")
            stb.AppendLine(" INNER JOIN Lista_IndirizziProd_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD And Zoo_Animali.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD  ")
            stb.AppendLine(" LEFT  JOIN Contatti (NOLOCK) ON Zoo_Animali.CF_Fornitore = Contatti.Cod_Contatto AND Zoo_Animali.Piva = Contatti.Piva ")
            stb.AppendLine(" LEFT JOIN trattamenti_cte ON giac_cte.Cod_Animale = trattamenti_cte.Cod_Animale")
            stb.AppendLine("  LEFT JOIN Lista_Patologie ON Lista_Patologie.Patologia_Cod = Zoo_Animali.Id_Patologia  ")
            If MostraGGPrimoCaricamento Then
                stb.AppendLine(" INNER JOIN DataPrimoCaricamento ON Zoo_Animali.Matricola = DataPrimoCaricamento.Matricola ")
            End If
            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON giac_cte.Piva = p.piva AND p.Sa_Cod = giac_cte.Sa_Cod AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            End If
            stb.AppendLine("  WHERE 1=1 ")
            If Piva <> "" Then
                stb.AppendLine("  And giac_cte.Piva = '" & Agro_SQL_SaveText(Piva) & "'      ")
            End If
            If Sa_Cod <> 0 Then
                stb.AppendLine("  And giac_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If Raggruppamento_Cod <> 0 Then
                stb.AppendLine("  And giac_cte.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "      ")
            End If

            stb.AppendLine(" 	AND Zoo_Animali.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")
            If LivelloCompatibilita(objParametri) >= 150 Then
                stb.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
            End If

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Leggi_Stazionamento_Capi(Piva As String,
                                      Sa_Cod As Integer,
                                      STA_NUM As Integer,
                                      Raggruppamento_Cod As Integer,
                                      Cod_Animale As Integer,
                                      Data As DateTime,
                                      giorni_stazionamento As Integer,
                                      ByVal Filtro_Visibilita_Utente As Boolean,
                                      ByVal listCod_Animali As List(Of Integer),
                                      ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Stazionamento_Capi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            Dim IDTestataTemp As Integer = 0
            'CancellaRecordDaIDTestataTemp
            Dim filtraGiacenze1 = True
            Dim bAll = False

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                For Each codAnimale In listCod_Animali
                    objTmp_AgendaTemp.Scrivi(IDTestataTemp, "", codAnimale, 0, objParametri)
                Next
            End If

            stb.Length = 0
            stb.AppendLine(" with  ")
            'If Filtro_Visibilita_Utente Then
            '    stb.AppendLine(" piva_visibili as ")
            '    stb.AppendLine(" ( ")
            '    stb.AppendLine(" select Piva  ")
            '    stb.AppendLine(" FROM  utenti_Visibilita_Appoggio ")
            '    stb.AppendLine(" WHERE Sa_Cod = 0 AND Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            '    stb.AppendLine(" ), ")
            'End If
            stb.AppendLine(" agn_cte as ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" select a.Id_Agenda,  ")
            stb.AppendLine("	a.PIVA,  ")
            stb.AppendLine("	md.Cod_Progetto, ")
            stb.AppendLine("	md.Lotto, ")
            stb.AppendLine("	md.Udm_Cod, ")
            stb.AppendLine("	UnitaMisura.Udm_Des, ")
            stb.AppendLine("	UnitaMisura.Udm_Sim, ")
            stb.AppendLine("	mdes.Sa_Cod, ")
            stb.AppendLine("	mdes.Id_Destinazione, ")
            stb.AppendLine("	mdes.Tipo_Destinazione, ")
            stb.AppendLine("	mdes.Qta, ")
            stb.AppendLine("	md.Id_Mov, ")
            stb.AppendLine("	md.Id_Mov_Det, ")
            stb.AppendLine("	m.Cau_Mov, ")
            stb.AppendLine("	i.rag_soc, ")
            stb.AppendLine("	ic.val_cod as cuaa, ")
            stb.AppendLine("	Centri_Aziendali.sa_nome ")
            stb.AppendLine("	from agenda a (NOLOCK)  ")
            stb.AppendLine("	inner join imprese i (NOLOCK) on i.PIVA = a.PIVA ")
            stb.AppendLine("	inner join imprese_codici ic (NOLOCK) on i.piva = ic.piva and ic.id_cod = 1010 ")
            stb.AppendLine("	inner join Movimenti m (NOLOCK) on	a.Id_Agenda = m.Id_Agenda  ")
            stb.AppendLine("	inner join Movimenti_dettagli md (NOLOCK) on md.Id_Agenda = m.Id_Agenda  AND md.Id_Mov = m.Id_Mov  ")
            stb.AppendLine("	inner join mov_destinazioni mdes (NOLOCK) on mdes.Id_Agenda = md.Id_Agenda and mdes.Id_Mov = md.Id_Mov and mdes.Id_Mov_Det = md.Id_Mov_Det ")
            stb.AppendLine("	inner join Centri_Aziendali (NOLOCK) ON mdes.Piva = Centri_Aziendali.Piva and mdes.Sa_Cod=Centri_Aziendali.sa_cod  ")
            stb.AppendLine("	INNER Join UnitaMisura (NOLOCK) ON UnitaMisura.Udm_Cod = md.Udm_Cod ")
            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                stb.AppendLine("	INNER Join __Tmp_Agenda (NOLOCK) ON md.Cod_Progetto = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
            End If
            'If Filtro_Visibilita_Utente And Piva = "" Then
            '    stb.AppendLine("	inner join piva_visibili p (NOLOCK) ON a.Piva = p.piva  ")
            'End If
            stb.AppendLine("	where  1 = 1 ")
            If Piva <> "" Then
                stb.AppendLine(" And a.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And mdes.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND md.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If

            stb.AppendLine("	and a.lav_Cod >= 3000 And a.lav_cod < 4000 ")
            stb.AppendLine("	And m.Cau_Mov In ('7300', '7350', '3700', '3750')     ")
            stb.AppendLine("	And m.Data_Movimento >=  CONVERT(DateTime,'1900/01/01',120)     ")

            If filtraGiacenze1 Then
                Select Case bAll
                    Case False
                        stb.AppendLine("	And m.Data_Movimento <=   " & Agro_SQL_SaveDateTime(Data) & "     ")
                    Case True
                        stb.AppendLine("	And m.Data_Movimento <=   " & Agro_SQL_SaveDate(Data) & "     ")
                End Select
            End If

            stb.AppendLine("	And md.Elem_Cod = 300  ")
            stb.AppendLine("	And md.Jolly_Int = 0  ")
            stb.AppendLine("	And mdes.Tipo_Destinazione IN (15, 21)  ")
            stb.AppendLine("), ")
            stb.AppendLine("giac_cte as ( ")
            stb.AppendLine("	SELECT   ")
            stb.AppendLine("  agn_cte.Piva, agn_cte.Rag_Soc As Impresa,  ")
            stb.AppendLine("  agn_cte.cuaa, Stalla.BDN_Codice_Azienda,  ")
            stb.AppendLine("  agn_cte.Cod_Progetto AS Cod_Animale,  ")
            stb.AppendLine("  agn_cte.Lotto,  ")
            stb.AppendLine("  agn_cte.Udm_Cod,  ")
            stb.AppendLine("  agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
            stb.AppendLine("  agn_cte.Sa_Cod,  ")
            stb.AppendLine("  agn_cte.sa_nome,  ")
            stb.AppendLine("  agn_cte.Id_Destinazione,  ")
            stb.AppendLine("  agn_cte.Tipo_Destinazione,  ")
            stb.AppendLine("  COALESCE(Fabbricati.Fabbricato_Des, '') As STA_DES,  ")
            stb.AppendLine("	COALESCE(Fabbricati.Fabbricato_Cod, 0) As STA_NUM,  ")
            stb.AppendLine("  COALESCE(Stalla_Raggruppamenti.Raggruppamento_Des, '') as Raggruppamento_Des,  ")
            stb.AppendLine("  COALESCE(Stalla_Raggruppamenti.Raggruppamento_Cod, 0) as Raggruppamento_Cod,  ")
            stb.AppendLine("  Convert(INTEGER, SUM( Case When agn_cte.CAU_MOV In ('7350','3750')              ")
            stb.AppendLine("          THEN -(agn_cte.qta)             ")
            stb.AppendLine("          Else agn_cte.qta             ")
            stb.AppendLine("          End)) As Giacenza   ")
            stb.AppendLine(" From agn_cte  ")
            stb.AppendLine(" INNER Join Stalla_Raggruppamenti (NOLOCK) ON agn_cte.Sa_Cod = Stalla_Raggruppamenti.sa_cod  ")
            stb.AppendLine("                                AND agn_cte.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod  ")
            stb.AppendLine("                                AND agn_cte.Tipo_Destinazione = 21  ")
            stb.AppendLine(" INNER JOIN Fabbricati (NOLOCK) ON Stalla_Raggruppamenti.PIVA = Fabbricati.Piva AND Stalla_Raggruppamenti.sa_cod = Fabbricati.SA_COD AND Stalla_Raggruppamenti.STA_NUM = Fabbricati.Fabbricato_Cod ")
            stb.AppendLine(" INNER JOIN Stalla (NOLOCK) ON Stalla.PIVA = Fabbricati.Piva AND Stalla.sa_cod = Fabbricati.SA_COD AND Stalla.STA_NUM = Fabbricati.Fabbricato_Cod  ")
            stb.AppendLine("  where 1=1  ")
            If Sa_Cod <> 0 Then
                stb.AppendLine(" And agn_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If
            stb.AppendLine(" GROUP BY agn_cte.Piva, agn_cte.Rag_Soc, agn_cte.cuaa, Stalla.BDN_Codice_Azienda, ")
            stb.AppendLine(" agn_cte.Cod_Progetto,  ")
            stb.AppendLine(" agn_cte.Lotto,  ")
            stb.AppendLine(" agn_cte.Udm_Cod, ")
            stb.AppendLine(" Fabbricati.Fabbricato_Des, ")
            stb.AppendLine(" Fabbricati.Fabbricato_Cod, ")
            stb.AppendLine(" agn_cte.Sa_Cod,  ")
            stb.AppendLine(" agn_cte.sa_nome, ")
            stb.AppendLine(" agn_cte.Id_Destinazione,  ")
            stb.AppendLine(" agn_cte.Tipo_Destinazione,  ")
            stb.AppendLine(" agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
            stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Des,  ")
            stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Cod  ")
            If filtraGiacenze1 Then
                stb.AppendLine(" HAVING Convert(INTEGER, SUM(Case When agn_cte.Cau_Mov In ('7350','3750') THEN -(agn_cte.qta) ELSE agn_cte.qta END)) <> 0   ")
            End If
            stb.AppendLine("), ")
            stb.AppendLine(" ZooMatricoleList as ( ")
            stb.AppendLine(" 	SELECT Zoo_Animali.Matricola ")
            stb.AppendLine(" 	FROM giac_cte  ")
            stb.AppendLine(" 	 INNER Join Zoo_Animali (NOLOCK) ON Zoo_Animali.Cod_Progetto = giac_cte.Cod_Animale   ")
            stb.AppendLine(" 	 WHERE 1=1  ")
            stb.AppendLine(" ),  ")
            stb.AppendLine(" DataPrimoCaricamento as ( ")
            stb.AppendLine(" SELECT ZooMatricoleList.Matricola, MIN(Movimenti.Data_Movimento) as Data_Movimento ")
            stb.AppendLine(" FROM Agenda (NOLOCK) ")
            stb.AppendLine(" JOIN Movimenti (NOLOCK) ON Agenda.ID_Agenda = Movimenti.ID_Agenda ")
            stb.AppendLine(" JOIN Movimenti_Dettagli (NOLOCK) ON Movimenti.ID_Agenda = Movimenti_Dettagli.ID_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov AND Movimenti_dettagli.Elem_Cod = 300 ")
            stb.AppendLine(" JOIN Zoo_Animali (NOLOCK) ON Movimenti_Dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
            stb.AppendLine(" JOIN ZooMatricoleList  ON Zoo_Animali.Matricola = ZooMatricoleList.Matricola ")
            stb.AppendLine(" WHERE 1 = 1 ")
            If Piva <> "" Then
                stb.AppendLine(" And Agenda.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If
            stb.AppendLine(" GROUP BY ZooMatricoleList.Matricola ")
            stb.AppendLine(" ) ")
            stb.AppendLine("SELECT giac_cte.Piva + '_' + CAST(giac_cte.Sa_Cod as varchar(100)) + '_' + CAST(giac_cte.Cod_Animale as varchar(100)) as chiave,  ")
            stb.AppendLine("giac_cte.*,  ")

            stb.AppendLine("Lista_Specie_Animali.SPE_DES,  ")
            stb.AppendLine("Lista_Razze_Animali.RAZ_DES,  ")
            stb.AppendLine("Zoo_Animali_Lista_Tipi.Tipo_Des,  ")
            stb.AppendLine("Lista_IndirizziProd_Animali.IPRO_DES,  ")
            If filtraGiacenze1 Then
                stb.AppendLine("Zoo_Animali_Distinte.Cod_Progetto,  ")
                stb.AppendLine("Zoo_Animali_Distinte.Progetto_Des,  ")
                stb.AppendLine("Zoo_Animali_Distinte.Progetto_Nome,  ")
                stb.AppendLine("Zoo_Animali_Distinte.Codice_Distinta,  ")
                stb.AppendLine("Zoo_AnimalixStati_Accrescimento.Stato_Cod,  ")
                stb.AppendLine("Zoo_Animali_Lista_Stati_Accrescimento.Stato_Des,  ")
            End If

            stb.AppendLine("Contatti.Rag_Soc,  ")
            stb.AppendLine("Contatti.Cod_Contatto, ")
            stb.AppendLine(" Zoo_Animali.RAZ_COD,  ")
            stb.AppendLine("   RIGHT(Zoo_Animali.Matricola, 4) AS Matricola_Breve4,   ")
            stb.AppendLine("   RIGHT(Zoo_Animali.Matricola, 5) AS Matricola_Breve,   ")
            stb.AppendLine(" 	Zoo_Animali.IPRO_COD,  ")
            stb.AppendLine(" 	Zoo_Animali.CF_Fornitore,  ")
            stb.AppendLine("   Zoo_Animali.Mat_Madre,   ")
            stb.AppendLine("   Zoo_Animali.Mat_Padre,   ")
            stb.AppendLine("   CASE Zoo_Animali.Metodo_Produzione WHEN 1 THEN 'Integrato' WHEN 2 THEN 'In Conversione' WHEN 3 THEN 'Biologico' END as Metodo_Produzione,   ")
            stb.AppendLine("   Zoo_Animali.Razza_Padre AS RazCod_Padre,   ")
            stb.AppendLine("   razza_madre.RAZ_DES as RazDes_Madre,   ")
            stb.AppendLine("   Zoo_Animali.Razza_Madre AS RazCod_Madre,   ")
            stb.AppendLine("   razza_padre.RAZ_DES as RazDes_Padre,   ")
            stb.AppendLine("   Zoo_Animali.Lotto_Fornitore, ")
            stb.AppendLine("   Zoo_Animali.Matricola,   ")
            stb.AppendLine("   Zoo_Animali.GEN_COD,   ")
            stb.AppendLine("   Zoo_Animali.SPE_COD,   ")
            stb.AppendLine("   Zoo_Animali.TIPO_COD,   ")
            stb.AppendLine("   Zoo_Animali.Progetto,   ")
            stb.AppendLine("   CAST(Zoo_Animali.Validita_Inizio as date) as Validita_Inizio,   ")
            stb.AppendLine("   CAST(Zoo_Animali.Validita_Fine as date) as Validita_Fine,   ")
            stb.AppendLine("   Zoo_Animali.Nome,   ")
            stb.AppendLine("   Zoo_Animali.Sesso,   ")
            stb.AppendLine("   Zoo_Animali.Modello4_Ingresso,   ")
            stb.AppendLine("   Zoo_Animali.Modello4_Uscita,   ")
            stb.AppendLine("   ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ZOO_Animali.Username_Creazione), ZOO_Animali.Username_Creazione) AS Utente_Creazione,   ")
            stb.AppendLine("   ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ZOO_Animali.Username_Modifica), ZOO_Animali.Username_Modifica) AS Utente_Modifica,   ")
            stb.AppendLine("   Zoo_Animali.Data_Creazione,   ")
            stb.AppendLine("   Zoo_Animali.Data_Modifica,   ")
            stb.AppendLine("   Zoo_Animali.Dat_Nascita,   ")
            stb.AppendLine("   Zoo_Animali.Id_Capo_BDN,   ")
            stb.AppendLine("   Zoo_Animali.CF_PROPRIETARIO,   ")
            stb.AppendLine("   Zoo_Animali.CF_DETENTORE,   ")
            stb.AppendLine("   Zoo_Animali.AUSL_AZI_NASCITA,   ")
            stb.AppendLine("   CASE WHEN Zoo_Animali.Id_Capo_BDN = 0 THEN 'No' ELSE 'Si' END AS FlagBDN,   ")
            stb.AppendLine("   Zoo_Animali.Certificato  ")

            stb.AppendLine("   , Zoo_Animali.Modello4_Ingresso_Numero  ")
            stb.AppendLine("   , Zoo_Animali.Modello4_Uscita_Numero  ")
            stb.AppendLine("   , Zoo_Animali.Modello4_Ingresso_Prenotazione  ")
            stb.AppendLine("   , Zoo_Animali.Modello4_Uscita_Prenotazione  ")
            stb.AppendLine("   , Zoo_Animali.Codice_Azienda_Uscita  ")
            stb.AppendLine("   , Zoo_Animali.Data_Documento_Ingresso  ")
            stb.AppendLine("   , Zoo_Animali.Data_Documento_Uscita  ")

            stb.AppendLine("   , DATEDIFF(day, Zoo_Animali.Validita_Inizio, " & Agro_SQL_SaveDate(Data, False) & ") + 1 as giorni_stalla ")

            stb.AppendLine("   , DATEDIFF(day, DataPrimoCaricamento.Data_Movimento,  " & Agro_SQL_SaveDate(Data, False) & " ) + 1 as giorni_stalla_primo_caricamento ")
            stb.AppendLine("   , DataPrimoCaricamento.Data_Movimento as data_primo_caricamento ")
            stb.AppendLine("   , DATEADD(day, " & giorni_stazionamento & " , DataPrimoCaricamento.Data_Movimento) as data_giorni_da_primo_caricamento, ")
            stb.AppendLine(Calcolo_Mesi("Zoo_Animali.Dat_Nascita", "DATEADD(day, " & giorni_stazionamento & " , DataPrimoCaricamento.Data_Movimento)") & " as eta_mesi_da_primo_caricamento, ")
            stb.AppendLine(Calcolo_Giorni("Zoo_Animali.Dat_Nascita", "DATEADD(day, " & giorni_stazionamento & " , DataPrimoCaricamento.Data_Movimento)") & " as eta_giorni_da_primo_caricamento ")

            stb.AppendLine("   , DATEADD(day, " & giorni_stazionamento & " , Zoo_Animali.Validita_Inizio) as Uscita_Data,  ")
            stb.AppendLine("    DATEDIFF(day, Zoo_Animali.Dat_Nascita, " & Agro_SQL_SaveDate(Data, False) & ") as eta_giorni_totali,")
            stb.AppendLine(Calcolo_Mesi("Zoo_Animali.Dat_Nascita", Agro_SQL_SaveDate(Data, False)) & " as eta_mesi,")
            stb.AppendLine(Calcolo_Giorni("Zoo_Animali.Dat_Nascita", Agro_SQL_SaveDate(Data, False)) & " as eta_giorni,")

            stb.AppendLine("    DATEDIFF(day, Zoo_Animali.Dat_Nascita, DATEADD(day, " & giorni_stazionamento & " , Zoo_Animali.Validita_Inizio)) as eta_giorni_totali_uscita,")
            stb.AppendLine(Calcolo_Mesi("Zoo_Animali.Dat_Nascita", "DATEADD(day, " & giorni_stazionamento & " , Zoo_Animali.Validita_Inizio)") & " as eta_mesi_uscita,")
            stb.AppendLine(Calcolo_Giorni("Zoo_Animali.Dat_Nascita", "DATEADD(day, " & giorni_stazionamento & " , Zoo_Animali.Validita_Inizio)") & " as eta_giorni_uscita")

            stb.AppendLine("FROM giac_cte ")
            stb.AppendLine(" INNER Join Zoo_Animali (NOLOCK) ON Zoo_Animali.Cod_Progetto = giac_cte.Cod_Animale  ")
            stb.AppendLine(" INNER JOIN DataPrimoCaricamento ON Zoo_Animali.Matricola = DataPrimoCaricamento.Matricola ")
            If filtraGiacenze1 Then
                stb.AppendLine(" INNER JOIN Zoo_Animali_Distinte (NOLOCK) ON Zoo_Animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale  ")
                Select Case bAll
                    Case False
                        stb.AppendLine("                                         AND CAST(Zoo_Animali_Distinte.Validita_Inizio as datetime) <=    " & Agro_SQL_SaveDateTime(Data) & " ")
                        stb.AppendLine("                                         And CAST(Zoo_Animali_Distinte.Validita_Fine as datetime) >=    " & Agro_SQL_SaveDateTime(Data) & " ")
                    Case True
                        stb.AppendLine("                                         AND CAST(Zoo_Animali_Distinte.Validita_Inizio as date) <=    " & Agro_SQL_SaveDate(Data) & " ")
                        stb.AppendLine("                                         And CAST(Zoo_Animali_Distinte.Validita_Fine as date) >=    " & Agro_SQL_SaveDate(Data) & " ")
                End Select
            End If
            If filtraGiacenze1 Then
                stb.AppendLine(" INNER JOIN Zoo_AnimalixStati_Accrescimento ON Zoo_AnimalixStati_Accrescimento.Cod_Progetto  = Zoo_Animali.Cod_Progetto  ")
                Select Case bAll
                    Case False
                        stb.AppendLine("                                           AND CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as datetime) <=    " & Agro_SQL_SaveDateTime(Data) & " ")
                        stb.AppendLine("                                           And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as datetime) >=   " & Agro_SQL_SaveDateTime(Data) & " ")
                    Case True
                        stb.AppendLine("                                           AND CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as date) <=    " & Agro_SQL_SaveDate(Data) & " ")
                        stb.AppendLine("                                           And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as date) >=   " & Agro_SQL_SaveDate(Data) & " ")
                End Select

            End If
            If filtraGiacenze1 Then
                stb.AppendLine("INNER JOIN Zoo_Animali_Lista_Stati_Accrescimento (NOLOCK) ON Zoo_AnimalixStati_Accrescimento.GEN_COD = Zoo_Animali_Lista_Stati_Accrescimento.GEN_COD  ")
                stb.AppendLine("												AND Zoo_AnimalixStati_Accrescimento.SPE_COD = Zoo_Animali_Lista_Stati_Accrescimento.SPE_COD  ")
                stb.AppendLine("												AND Zoo_AnimalixStati_Accrescimento.STATO_COD = Zoo_Animali_Lista_Stati_Accrescimento.STATO_COD  ")
                stb.AppendLine("												AND Zoo_AnimalixStati_Accrescimento.TIPO_COD = Zoo_Animali_Lista_Stati_Accrescimento.TIPO_COD  ")
            End If
            stb.AppendLine("INNER JOIN Lista_Specie_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Specie_Animali.SPE_COD  ")
            stb.AppendLine("INNER JOIN Lista_Razze_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD And Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD  ")
            stb.AppendLine(" INNER JOIN Lista_Razze_Animali razza_madre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_madre.GEN_COD And Zoo_Animali.SPE_COD = razza_madre.SPE_COD And Zoo_Animali.Razza_Madre = razza_madre.RAZ_COD   ")
            stb.AppendLine(" INNER JOIN Lista_Razze_Animali razza_padre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_padre.GEN_COD And Zoo_Animali.SPE_COD = razza_padre.SPE_COD And Zoo_Animali.Razza_Padre = razza_padre.RAZ_COD   ")
            stb.AppendLine("INNER JOIN Zoo_Animali_Lista_Tipi (NOLOCK) ON Zoo_Animali.GEN_COD = Zoo_Animali_Lista_Tipi.GEN_COD And Zoo_Animali.SPE_COD = Zoo_Animali_Lista_Tipi.SPE_COD And Zoo_Animali.TIPO_COD = Zoo_Animali_Lista_Tipi.TIPO_COD  ")
            stb.AppendLine("INNER JOIN Lista_IndirizziProd_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD And Zoo_Animali.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD  ")
            stb.AppendLine("LEFT  JOIN Contatti (NOLOCK) ON Zoo_Animali.CF_Fornitore = Contatti.Cod_Contatto AND Zoo_Animali.Piva = Contatti.Piva ")
            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON Zoo_Animali.Piva = p.piva AND p.Sa_Cod = giac_cte.Sa_Cod AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            End If
            stb.AppendLine(" WHERE 1=1 ")
            If filtraGiacenze1 Then
                Select Case bAll
                    Case False
                    'stb.AppendLine(" And Zoo_Animali.Validita_Inizio <= " & Agro_SQL_SaveDateTime(Data) & "   ")
                    'stb.AppendLine(" And Zoo_Animali.Validita_Fine >= " & Agro_SQL_SaveDateTime(Data) & "   ")
                    Case True
                        stb.AppendLine(" And CAST(Zoo_Animali.Validita_Inizio as Date) <= " & Agro_SQL_SaveDate(Data) & "   ")
                        stb.AppendLine(" And CAST(Zoo_Animali.Validita_Fine as Date) >= " & Agro_SQL_SaveDate(Data) & "   ")
                End Select
            End If

            If Piva <> "" Then
                stb.AppendLine(" And giac_cte.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And giac_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND giac_cte.Sta_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If

            If Raggruppamento_Cod <> 0 Then
                stb.AppendLine(" AND giac_cte.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "  ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND giac_cte.Cod_Animale = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If


            If LivelloCompatibilita(objParametri) >= 150 Then
                stb.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
            End If
            'stb.AppendLine("ORDER BY giac_cte.Impresa ")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Leggi_Scarico_Capi(Piva As String,
                                       Sa_Cod As Integer,
                                       STA_NUM As Integer,
                                       Raggruppamento_Cod As Integer,
                                       Cod_Animale As Integer,
                                       Data_Inizio As Date,
                                       Data_Fine As Date,
                                       Matricola As String,
                                       ByVal Filtro_Visibilita_Utente As Boolean,
                                       ByVal listCod_Animali As List(Of Integer),
                                       ByVal Lav_Cod As Integer,
                                       ByVal flagFornitore As Boolean,
                                       ByVal flagAziendaUscita As Boolean,
                                       ByRef objParametri As AgronicaCoreParametri,
                                       Optional ByVal mostraPesate As Boolean = False) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Scarico_Capi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            Dim IDTestataTemp As Integer = 0
            'CancellaRecordDaIDTestataTemp

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                For Each codAnimale In listCod_Animali
                    objTmp_AgendaTemp.Scrivi(IDTestataTemp, "", codAnimale, 0, objParametri)
                Next
            End If

            stb.Length = 0


            If mostraPesate Then stb.AppendLine(CreaPesate_CaricoScarico_TT)

            stb.AppendLine("SELECT Imprese.Piva + '_' + CAST(Centri_Aziendali.Sa_Cod as varchar(100)) + '_' + CAST(Zoo_Animali.Cod_Progetto as varchar(100)) as chiave,  ")
            stb.AppendLine(" 	Imprese.Piva,  ")
            stb.AppendLine(" 	Imprese.rag_soc,  ")
            stb.AppendLine(" 	Imprese.rag_soc as Impresa,  ")
            stb.AppendLine(" 	Imprese_codici.val_cod as CUAA, ")
            stb.AppendLine(" 	Stalla.BDN_Codice_Azienda, ")
            stb.AppendLine(" 	Centri_Aziendali.sa_nome, ")
            stb.AppendLine(" 	Centri_Aziendali.sa_cod, ")
            stb.AppendLine(" 	Stalla.STA_DES, ")
            stb.AppendLine(" 	Stalla.STA_NUM, ")
            stb.AppendLine(" 	Stalla_Raggruppamenti.Raggruppamento_Cod, ")
            stb.AppendLine(" 	Stalla_Raggruppamenti.Raggruppamento_Des, ")
            stb.AppendLine(" 	Operazioni.LAV_COD, ")
            stb.AppendLine(" 	Operazioni.LAV_DES, ")
            stb.AppendLine(" 	Zoo_Animali.Cod_Progetto, ")
            stb.AppendLine("   RIGHT(Zoo_Animali.Matricola, 4) AS Matricola_Breve4,   ")
            stb.AppendLine("   RIGHT(Zoo_Animali.Matricola, 5) AS Matricola_Breve,   ")
            stb.AppendLine(" 	Zoo_Animali.Matricola, ")
            stb.AppendLine(" 	Zoo_Animali_Distinte.Codice_Distinta as Lotto, ")
            stb.AppendLine("    Zoo_Animali_Distinte.Cod_Progetto,  ")
            stb.AppendLine("    Zoo_Animali_Distinte.Progetto_Des,  ")
            stb.AppendLine("    Zoo_Animali_Distinte.Progetto_Nome,  ")
            stb.AppendLine("    Zoo_Animali_Distinte.Codice_Distinta,  ")
            stb.AppendLine("    Zoo_AnimalixStati_Accrescimento.Stato_Cod,  ")
            stb.AppendLine("    Zoo_Animali_Lista_Stati_Accrescimento.Stato_Des,  ")
            stb.AppendLine(" 	Zoo_Animali.DAT_NASCITA, ")
            stb.AppendLine(" 	Movimenti.Data_Movimento as Validita_Fine, ")
            stb.AppendLine(" 	Zoo_Animali.SPE_COD, ")
            stb.AppendLine(" 	Lista_Specie_Animali.SPE_DES, ")
            stb.AppendLine(" 	Zoo_Animali.RAZ_COD, ")
            stb.AppendLine(" 	Lista_Razze_Animali.RAZ_DES, ")
            stb.AppendLine(" 	Lista_IndirizziProd_Animali.IPRO_COD, ")
            stb.AppendLine(" 	Lista_IndirizziProd_Animali.IPRO_DES, ")
            stb.AppendLine(" 	CAST(Zoo_Animali.Validita_Inizio as date) as Validita_Inizio, ")
            stb.AppendLine(" 	Zoo_Animali.Sesso, ")
            stb.AppendLine(" 	Zoo_Animali_Lista_Tipi.Tipo_Cod, ")
            stb.AppendLine(" 	Zoo_Animali_Lista_Tipi.Tipo_Des, ")
            stb.AppendLine(" 	COALESCE(Zoo_Animali.Modello4_Uscita_Numero, '') as Modello4_Uscita_Numero, ")
            stb.AppendLine(" 	COALESCE(Zoo_Animali.Modello4_Uscita_Prenotazione, '') as Modello4_Uscita_Prenotazione, ")
            stb.AppendLine("  Zoo_Animali.Note, ")
            stb.AppendLine("  Lista_Patologie.Patologia_Des, ")
            stb.AppendLine("   Zoo_Animali.Mat_Madre,   ")
            stb.AppendLine("   Zoo_Animali.Mat_Padre,   ")
            stb.AppendLine("   CASE Zoo_Animali.Metodo_Produzione WHEN 1 THEN '" &
                           Agro_SQL_SaveText(Gias.Integrato) & "' WHEN 2 THEN '" &
                           Agro_SQL_SaveText(Gias.InConversione) & "' WHEN 3 THEN '" &
                           Agro_SQL_SaveText(Gias.Biologico) & "' END as Metodo_Produzione,   ")
            stb.AppendLine("   Zoo_Animali.Razza_Padre AS RazCod_Padre,   ")
            stb.AppendLine("   razza_madre.RAZ_DES as RazDes_Madre,   ")
            stb.AppendLine("   Zoo_Animali.Razza_Madre AS RazCod_Madre,   ")
            stb.AppendLine("   razza_padre.RAZ_DES as RazDes_Padre,   ")
            stb.AppendLine("   Zoo_Animali.Progetto,   ")
            stb.AppendLine("   Zoo_Animali.CF_PROPRIETARIO,   ")
            stb.AppendLine("   Zoo_Animali.CF_DETENTORE,   ")
            stb.AppendLine("   COALESCE(Zoo_Animali.AUSL_AZI_NASCITA, '') as AUSL_AZI_NASCITA,   ")
            stb.AppendLine("   CASE WHEN Zoo_Animali.Id_Capo_BDN = 0 THEN '" & Gias.No & "' ELSE '" & Gias.Si & "' END AS FlagBDN,   ")
            stb.AppendLine("   CASE WHEN Zoo_Animali.Validato = 1 THEN '" & Gias.Si & "' ELSE '" & Gias.No & "' END AS Validato,   ")
            stb.AppendLine("   Zoo_Animali.Certificato,  ")
            stb.AppendLine("   CAST(Zoo_Animali.Validita_Fine as date) as Validita_Fine   ")
            stb.AppendLine("   , CAST(Ultimo_Trattamento.Data_Movimento AS DATE) AS Data_Ultimo_Trattamento ")
            stb.AppendLine("   , Zoo_Animali.Incremento_Teorico AS Incremento_Teorico ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Ingresso_Numero, '') as Modello4_Ingresso_Numero  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Uscita_Numero, '') as Modello4_Uscita_Numero ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Ingresso_Prenotazione, '') as Modello4_Ingresso_Prenotazione  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Uscita_Prenotazione, '') as Modello4_Uscita_Prenotazione  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Codice_Azienda_Uscita, '') as Codice_Azienda_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_Documento_Ingresso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_Documento_Ingresso  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_Documento_Uscita, CONVERT(datetime, '2100-12-31 00:00:00.000', 120)) as Data_Documento_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Fornitore_Provenienza, '') as Fornitore_Provenienza  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.N_Bolla_Fornitore, '') as N_Bolla_Fornitore  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.N_Bolla_Uscita, '') as N_Bolla_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_DDT_Ingresso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_DDT_Ingresso  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_DDT_Uscita, CONVERT(datetime, '2100-12-31 00:00:00.000', 120)) as Data_DDT_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Codice_Azienda_Fornitore, '') as Codice_Azienda_Fornitore  ")
            stb.AppendLine("   , Zoo_Animali.Stalla_Svezzamento  ")
            stb.AppendLine("   , Zoo_Animali.Note ")
            stb.AppendLine("   , Zoo_Animali.Anomalie_Note, ")
            If flagFornitore Then
                stb.AppendLine("   COALESCE(Contatti_StallaSvezz.Cod_Contatto, '') AS CF_StallaSvezz,  ")
                stb.AppendLine("   COALESCE(Contatti_StallaSvezz.Rag_Soc + Contatti_StallaSvezz.cognome + ' ' + Contatti_StallaSvezz.Nome, '') AS RagSoc_StallaSvezz,  ")
                stb.AppendLine("   COALESCE(FornFatt_Contatti.Rag_Soc, '') AS Fornitore_Fatt, ")
                stb.AppendLine("   COALESCE(FornProv_Contatti.Rag_Soc, '') AS Fornitore_Prov, ")
            End If
            If flagAziendaUscita Then
                stb.AppendLine(" 	COALESCE(AU_Contatti.Rag_Soc, '') AS Azienda_Uscita, ")
            End If
            stb.AppendLine("  Agenda.Id_Agenda AS Id_Operazione, ")
            stb.AppendLine("  CAST(Agenda.Data_Creazione as date) as Data_Creazione, ")
            stb.AppendLine("  CAST(Agenda.Data_Modifica as date) as Data_Modifica, ")
            stb.AppendLine(" 	COALESCE(Zoo_Animali.Codice_Azienda_Uscita, '') as Codice_Azienda_Uscita, ")
            stb.AppendLine(" 	COALESCE(Zoo_Animali.N_Bolla_Uscita, '') as N_Bolla_Uscita, ")
            stb.AppendLine(" 	COALESCE(Zoo_Animali.Data_DDT_UScita, CONVERT(datetime, '2100-12-31 00:00:00.000', 120)) as Data_DDT_UScita, ")
            stb.AppendLine(" 	COALESCE(Lista_Causali_Morte.Des, '') AS Causale_Morte, ")
            stb.AppendLine(" 	DATEDIFF(day, Zoo_Animali.DAT_NASCITA, Movimenti.Data_Movimento) As Eta_Giorni_TOTALI, ")
            stb.AppendLine("    " & Calcolo_Mesi("Zoo_Animali.Dat_Nascita", "Movimenti.Data_Movimento") & " as eta_mesi,")
            stb.AppendLine("    " & Calcolo_Giorni("Zoo_Animali.Dat_Nascita", "Movimenti.Data_Movimento") & " as eta_giorni, ")
            stb.AppendLine(" 	DATEDIFF(day, Zoo_Animali.Validita_Inizio, Movimenti.Data_Movimento) As Giorni_Stalla ")

            If mostraPesate Then stb.AppendLine(",    ROUND(pesate_cs_cte.Peso_Pagato, 2) Peso_Pagato ")

            stb.AppendLine(" FROM Agenda ")
            stb.AppendLine(" JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")
            stb.AppendLine(" JOIN Movimenti (NOLOCK) ON Agenda.iD_Agenda = Movimenti.ID_Agenda AND Movimenti.Cau_Mov = '3750' ")
            stb.AppendLine(" JOIN Movimenti_Dettagli (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")
            stb.AppendLine(" JOIN Mov_Destinazioni (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            stb.AppendLine(" INNER Join Zoo_Animali (NOLOCK) ON Zoo_Animali.Cod_Progetto = Movimenti_Dettagli.Cod_Progetto ")
            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                stb.AppendLine("	INNER Join __Tmp_Agenda (NOLOCK) ON Zoo_Animali.Cod_Progetto = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
            End If
            stb.AppendLine(" INNER JOIN Zoo_Animali_Distinte (NOLOCK) ON Zoo_Animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale  ")
            stb.AppendLine(" 				AND CAST(Zoo_Animali_Distinte.Validita_Inizio as date) <= CAST(Movimenti.Data_Movimento as date) ")
            stb.AppendLine(" 				And CAST(Zoo_Animali_Distinte.Validita_Fine as date) >=  CAST(Movimenti.Data_Movimento as date) ")
            stb.AppendLine(" INNER JOIN Zoo_AnimalixStati_Accrescimento (NOLOCK) ON Zoo_AnimalixStati_Accrescimento.Cod_Progetto  = Zoo_Animali.Cod_Progetto ")
            stb.AppendLine(" 				AND CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as datetime) <=  CAST(Movimenti.Data_Movimento as date) ")
            stb.AppendLine(" 				And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as datetime) >=  CAST(Movimenti.Data_Movimento as date) ")
            stb.AppendLine(" INNER JOIN Zoo_Animali_Lista_Stati_Accrescimento (NOLOCK) ON Zoo_AnimalixStati_Accrescimento.GEN_COD = Zoo_Animali_Lista_Stati_Accrescimento.GEN_COD ")
            stb.AppendLine(" 				AND Zoo_AnimalixStati_Accrescimento.SPE_COD = Zoo_Animali_Lista_Stati_Accrescimento.SPE_COD ")
            stb.AppendLine(" 				AND Zoo_AnimalixStati_Accrescimento.STATO_COD = Zoo_Animali_Lista_Stati_Accrescimento.STATO_COD ")
            stb.AppendLine(" 				AND Zoo_AnimalixStati_Accrescimento.TIPO_COD = Zoo_Animali_Lista_Stati_Accrescimento.TIPO_COD ")
            stb.AppendLine(" INNER JOIN Lista_Specie_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Specie_Animali.SPE_COD  ")
            stb.AppendLine(" INNER JOIN Lista_Razze_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD And Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD  ")
            stb.AppendLine(" INNER JOIN Lista_Razze_Animali razza_madre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_madre.GEN_COD And Zoo_Animali.SPE_COD = razza_madre.SPE_COD And Zoo_Animali.Razza_Madre = razza_madre.RAZ_COD   ")
            stb.AppendLine(" INNER JOIN Lista_Razze_Animali razza_padre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_padre.GEN_COD And Zoo_Animali.SPE_COD = razza_padre.SPE_COD And Zoo_Animali.Razza_Padre = razza_padre.RAZ_COD  ")
            stb.AppendLine(" INNER JOIN Zoo_Animali_Lista_Tipi (NOLOCK) ON Zoo_Animali.GEN_COD = Zoo_Animali_Lista_Tipi.GEN_COD And Zoo_Animali.SPE_COD = Zoo_Animali_Lista_Tipi.SPE_COD And Zoo_Animali.TIPO_COD = Zoo_Animali_Lista_Tipi.TIPO_COD  ")
            stb.AppendLine(" INNER JOIN Lista_IndirizziProd_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD And Zoo_Animali.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD  ")
            stb.AppendLine(" LEFT JOIN Lista_Causali_Morte (NOLOCK) ON Zoo_Animali.Causale_Morte = Lista_Causali_Morte.Cod ")
            stb.AppendLine(" LEFT JOIN Lista_Patologie ON Lista_Patologie.Patologia_Cod = Zoo_Animali.Id_Patologia  ")

            If mostraPesate Then stb.AppendLine("LEFT JOIN #pesate_cs_cte pesate_cs_cte (NOLOCK) ON Agenda.Id_Agenda = pesate_cs_cte.Id_Agenda AND Movimenti_dettagli.Cod_Progetto = pesate_cs_cte.Cod_Progetto ")

            ' OUTER APPLY per Data_Ultimo_Trattamento
            stb.AppendLine("OUTER APPLY ( ").
                AppendLine("    SELECT TOP 1 m_tratt.Data_Movimento ").
                AppendLine("    FROM Agenda a_tratt ").
                AppendLine("    INNER JOIN Movimenti m_tratt (NOLOCK) ON m_tratt.Id_Agenda = a_tratt.Id_Agenda ").
                AppendLine("    INNER JOIN Movimenti_dettagli mdett_tratt (NOLOCK) ON mdett_tratt.Id_Agenda = m_tratt.Id_Agenda ").
                AppendLine("        AND mdett_tratt.Id_Mov = m_tratt.Id_Mov ").
                AppendLine("    INNER JOIN Mov_Destinazioni mdest_tratt (NOLOCK) ON mdest_tratt.Id_Agenda = mdett_tratt.Id_Agenda ").
                AppendLine("        AND mdest_tratt.Id_Mov = mdett_tratt.Id_Mov ").
                AppendLine("        AND mdest_tratt.Id_Mov_Det = mdett_tratt.Id_Mov_Det ").
                AppendLine($"    WHERE a_tratt.Lav_Cod = {LAVCOD_CUREMEDICAMENTI_ANIMALI} ").
                AppendLine($"        AND m_tratt.Cau_Mov = '{CAU_TRATTAMENTO_ZOO}' ").
                AppendLine($"        AND mdest_tratt.Id_Destinazione = Zoo_Animali.Cod_Progetto ").
                AppendLine($"    ORDER BY m_tratt.Data_Movimento DESC ").
                AppendLine($") AS Ultimo_Trattamento ")

            If flagFornitore Then
                stb.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Contatti ")
                stb.AppendLine("             WHERE Contatti.Cod_Contatto = Zoo_Animali.CF_Fornitore AND ")
                stb.AppendLine("                (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1)) FornFatt_Contatti")

                stb.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Contatti ")
                stb.AppendLine("             WHERE Contatti.Cod_Contatto = Zoo_Animali.Fornitore_Provenienza AND ")
                stb.AppendLine("                (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1)) FornProv_Contatti")

                stb.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti  (NOLOCK)  ")
                stb.AppendLine("             WHERE Contatti.Cod_Contatto = Zoo_Animali.Stalla_Svezzamento AND  ")
                stb.AppendLine("                (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1)) Contatti_StallaSvezz ")
                'stb.AppendLine("LEFT JOIN Contatti AS FornFatt_Contatti (NOLOCK) ON Zoo_Animali.CF_Fornitore = FornFatt_Contatti.Cod_Contatto")
                'stb.AppendLine("    AND Zoo_Animali.Piva = FornFatt_Contatti.Piva")
                'stb.AppendLine("LEFT JOIN Contatti AS FornProv_Contatti (NOLOCK) ON Zoo_Animali.Fornitore_Provenienza = FornProv_Contatti.Cod_Contatto")
                'stb.AppendLine("    AND Zoo_Animali.Piva = FornProv_Contatti.Piva")
            Else
                stb.AppendLine(" LEFT  JOIN Contatti (NOLOCK) ON Zoo_Animali.CF_Fornitore = Contatti.Cod_Contatto AND Zoo_Animali.Piva = Contatti.Piva ")
            End If

            If flagAziendaUscita Then
                stb.AppendLine("LEFT JOIN Risorse_Umane AS AU_RisorseUmane (NOLOCK) ON Zoo_Animali.Codice_Azienda_Uscita = AU_RisorseUmane.Attivita_Des AND Zoo_Animali.Codice_Azienda_Uscita <> '' ")
                stb.AppendLine("LEFT JOIN Contatti AS AU_Contatti (NOLOCK) ON AU_RisorseUmane.Cod_Contatto = AU_Contatti.Cod_Contatto")
                stb.AppendLine("    AND AU_RisorseUmane.Piva = AU_Contatti.Piva")
            End If

            stb.AppendLine(" JOIN Stalla_Raggruppamenti (NOLOCK) ON Mov_Destinazioni.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod ")
            stb.AppendLine(" JOIN Stalla (NOLOCK) ON Stalla_Raggruppamenti.STA_NUM = Stalla.STA_NUM AND Stalla_Raggruppamenti.PIVA = Stalla.Piva AND Stalla_Raggruppamenti.sa_cod = Stalla.sa_cod ")
            stb.AppendLine(" JOIN Centri_Aziendali (NOLOCK) ON Centri_Aziendali.PIVA = Stalla.Piva AND Centri_Aziendali.sa_cod = Stalla.sa_cod ")
            stb.AppendLine(" JOIN Imprese (NOLOCK) ON Agenda.PIVA = Imprese.PIVA ")
            stb.AppendLine(" JOIN Imprese_Codici (NOLOCK) ON Imprese.Piva = Imprese_Codici.Piva AND Imprese_Codici.id_cod = 1010 ")
            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON Imprese.Piva = p.piva AND p.Sa_Cod = Centri_Aziendali.sa_cod AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            End If
            stb.AppendLine(" WHERE 1 = 1 ")

            If Piva <> "" Then
                stb.AppendLine(" And Imprese.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And Centri_Aziendali.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND Stalla.Sta_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If

            If Raggruppamento_Cod <> 0 Then
                stb.AppendLine(" AND Stalla_Raggruppamenti.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "  ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND Zoo_Animali.Cod_Animale = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If

            If Data_Inizio <> AGRODATAINIZIO Then
                'Movimenti.Data_Movimento
                stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Data_Inizio) & "  ")
            End If

            If Data_Fine <> AGRODATAFINE Then
                stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Data_Fine) & "  ")
            End If

            If Matricola <> "" Then
                stb.AppendLine(" AND Zoo_Animali.Matricola = '" & Agro_SQL_SaveText(Matricola) & "'  ")
            End If

            If Lav_Cod <> 0 Then
                stb.AppendLine(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
            Else
                stb.AppendLine(" AND Agenda.Lav_Cod IN (" & LAVCOD_VENDITA_ANIMALI & ", " & LAVCOD_MORTE_ANIMALI & ", " & LAVCOD_MACELLAZIONE_ANIMALI & ", " & LAVCOD_DECREMENTO_CONSISTENZE_ZOO & "," & LAVCOD_TRASFERIMENTO_ANIMALI & ")  ")
            End If

            If LivelloCompatibilita(objParametri) >= 150 Then
                stb.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
            End If

            If mostraPesate Then stb.AppendLine("DROP TABLE #pesate_cs_cte ")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Carico_Capi(Piva As String,
                                      Sa_Cod As Integer,
                                      STA_NUM As Integer,
                                      Raggruppamento_Cod As Integer,
                                      Cod_Animale As Integer,
                                      Data_Inizio As Date,
                                      Data_Fine As Date,
                                      Matricola As String,
                                      ByVal Filtro_Visibilita_Utente As Boolean,
                                      ByVal listCod_Animali As List(Of Integer),
                                      ByVal Lav_Cod As Integer,
                                      ByVal flagFornitore As Boolean,
                                      ByVal flagAziendaUscita As Boolean,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal mostraPesate As Boolean = False) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Carico_Capi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            Dim IDTestataTemp As Integer = 0
            'CancellaRecordDaIDTestataTemp

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                For Each codAnimale In listCod_Animali
                    objTmp_AgendaTemp.Scrivi(IDTestataTemp, "", codAnimale, 0, objParametri)
                Next
            End If

            stb.Length = 0

            If mostraPesate Then stb.AppendLine(CreaPesate_CaricoScarico_TT)

            stb.AppendLine("SELECT Imprese.Piva + '_' + CAST(Centri_Aziendali.Sa_Cod as varchar(100)) + '_' + CAST(Zoo_Animali.Cod_Progetto as varchar(100)) as chiave,  ")

            stb.AppendLine(" 	Imprese.Piva,  ")
            stb.AppendLine(" 	Imprese.rag_soc,  ")
            stb.AppendLine(" 	Imprese.rag_soc as Impresa,  ")
            stb.AppendLine(" 	Imprese_codici.val_cod as CUAA, ")
            stb.AppendLine(" 	Stalla.BDN_Codice_Azienda, ")
            stb.AppendLine(" 	Centri_Aziendali.sa_nome, ")
            stb.AppendLine(" 	Centri_Aziendali.sa_cod, ")
            stb.AppendLine(" 	Stalla.STA_DES, ")
            stb.AppendLine(" 	Stalla.STA_NUM, ")
            stb.AppendLine(" 	Stalla_Raggruppamenti.Raggruppamento_Cod, ")
            stb.AppendLine(" 	Stalla_Raggruppamenti.Raggruppamento_Des, ")
            stb.AppendLine(" 	Operazioni.LAV_COD, ")
            stb.AppendLine(" 	Operazioni.LAV_DES, ")
            stb.AppendLine(" 	Zoo_Animali.Cod_Progetto as Cod_Animale, ")
            stb.AppendLine(" 	Zoo_Animali.Cod_Progetto, ")
            stb.AppendLine("    RIGHT(Zoo_Animali.Matricola, 4) AS Matricola_Breve4,   ")
            stb.AppendLine("    RIGHT(Zoo_Animali.Matricola, 5) AS Matricola_Breve,   ")
            stb.AppendLine(" 	Zoo_Animali.Matricola, ")
            stb.AppendLine(" 	Zoo_Animali_Distinte.Codice_Distinta as Lotto, ")
            stb.AppendLine("    Zoo_Animali_Distinte.Cod_Progetto,  ")
            stb.AppendLine("    Zoo_Animali_Distinte.Progetto_Des,  ")
            stb.AppendLine("    Zoo_Animali_Distinte.Progetto_Nome,  ")
            stb.AppendLine("    Zoo_Animali_Distinte.Codice_Distinta,  ")
            stb.AppendLine("    Zoo_AnimalixStati_Accrescimento.Stato_Cod,  ")
            stb.AppendLine("    Zoo_Animali_Lista_Stati_Accrescimento.Stato_Des,  ")
            stb.AppendLine(" 	Zoo_Animali.DAT_NASCITA, ")
            stb.AppendLine(" 	CAST(Zoo_Animali.Validita_Fine as date) as Validita_Fine, ")
            stb.AppendLine(" 	Zoo_Animali.SPE_COD, ")
            stb.AppendLine(" 	Lista_Specie_Animali.SPE_DES, ")
            stb.AppendLine(" 	Zoo_Animali.RAZ_COD, ")
            stb.AppendLine(" 	Lista_Razze_Animali.RAZ_DES, ")
            stb.AppendLine(" 	Lista_IndirizziProd_Animali.IPRO_COD, ")
            stb.AppendLine(" 	Lista_IndirizziProd_Animali.IPRO_DES, ")
            stb.AppendLine(" 	Movimenti.Data_Movimento as Validita_Inizio, ")
            stb.AppendLine(" 	Zoo_Animali.Sesso, ")
            stb.AppendLine(" 	Zoo_Animali_Lista_Tipi.Tipo_Cod, ")
            stb.AppendLine(" 	Zoo_Animali_Lista_Tipi.Tipo_Des, ")
            stb.AppendLine("    Zoo_Animali.Lotto_Fornitore, ")
            stb.AppendLine(" 	COALESCE(Zoo_Animali.Modello4_Ingresso_Numero, '') as Modello4_Ingresso_Numero, ")
            stb.AppendLine(" 	COALESCE(Zoo_Animali.Modello4_Ingresso_Prenotazione, '') as Modello4_Ingresso_Prenotazione, ")
            stb.AppendLine(" 	COALESCE(Zoo_Animali.Certificato, '') as Certificato, ")
            stb.AppendLine("   Zoo_Animali.Mat_Madre,   ")
            stb.AppendLine("   Zoo_Animali.Mat_Padre,   ")
            stb.AppendLine("   CASE Zoo_Animali.Metodo_Produzione WHEN 1 THEN '" &
                           Agro_SQL_SaveText(Gias.Integrato) & "' WHEN 2 THEN '" &
                           Agro_SQL_SaveText(Gias.InConversione) & "' WHEN 3 THEN '" &
                           Agro_SQL_SaveText(Gias.Biologico) & "' END as Metodo_Produzione,   ")
            stb.AppendLine("   Zoo_Animali.Razza_Padre AS RazCod_Padre,   ")
            stb.AppendLine("   razza_madre.RAZ_DES as RazDes_Madre,   ")
            stb.AppendLine("   Zoo_Animali.Razza_Madre AS RazCod_Madre,   ")
            stb.AppendLine("   razza_padre.RAZ_DES as RazDes_Padre,   ")
            stb.AppendLine("   Zoo_Animali.Progetto,   ")
            stb.AppendLine("   Zoo_Animali.CF_PROPRIETARIO,   ")
            stb.AppendLine("   Zoo_Animali.CF_DETENTORE,   ")
            stb.AppendLine("   COALESCE(Zoo_Animali.AUSL_AZI_NASCITA, '') as AUSL_AZI_NASCITA,   ")
            stb.AppendLine("   CASE WHEN Zoo_Animali.Id_Capo_BDN = 0 THEN '" & Gias.No & "' ELSE '" & Gias.Si & "' END AS FlagBDN,   ")
            stb.AppendLine("   CASE WHEN Zoo_Animali.Validato = 1 THEN '" & Gias.Si & "' ELSE '" & Gias.No & "' END AS Validato,   ")
            stb.AppendLine("   Zoo_Animali.Certificato,  ")
            stb.AppendLine("   CAST(Zoo_Animali.Validita_Fine as date) as Validita_Fine   ")
            stb.AppendLine("   , Zoo_Animali.Incremento_Teorico AS Incremento_Teorico ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Ingresso_Numero, '') as Modello4_Ingresso_Numero  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Uscita_Numero, '') as Modello4_Uscita_Numero ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Ingresso_Prenotazione, '') as Modello4_Ingresso_Prenotazione  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Modello4_Uscita_Prenotazione, '') as Modello4_Uscita_Prenotazione  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Codice_Azienda_Uscita, '') as Codice_Azienda_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_Documento_Ingresso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_Documento_Ingresso  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_Documento_Uscita, CONVERT(datetime, '2100-12-31 00:00:00.000', 120)) as Data_Documento_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Fornitore_Provenienza, '') as Fornitore_Provenienza  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.N_Bolla_Fornitore, '') as N_Bolla_Fornitore  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.N_Bolla_Uscita, '') as N_Bolla_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_DDT_Ingresso, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_DDT_Ingresso  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Data_DDT_Uscita, CONVERT(datetime, '2100-12-31 00:00:00.000', 120)) as Data_DDT_Uscita  ")
            stb.AppendLine("   , COALESCE(Zoo_Animali.Codice_Azienda_Fornitore, '') as Codice_Azienda_Fornitore  ")
            stb.AppendLine("   , Zoo_Animali.Stalla_Svezzamento  ")
            stb.AppendLine("   , Zoo_Animali.Note ")
            stb.AppendLine("   , Zoo_Animali.Anomalie_Note, ")
            If flagFornitore Then
                stb.AppendLine("   COALESCE(Contatti_StallaSvezz.Cod_Contatto, '') AS CF_StallaSvezz,  ")
                stb.AppendLine("   COALESCE(Contatti_StallaSvezz.Rag_Soc + Contatti_StallaSvezz.cognome + ' ' + Contatti_StallaSvezz.Nome, '') AS RagSoc_StallaSvezz,  ")
                stb.AppendLine("   COALESCE(FornFatt_Contatti.Rag_Soc + FornFatt_Contatti.cognome + ' ' + FornFatt_Contatti.Nome, '') AS Fornitore_Fatt, ")
                stb.AppendLine("   COALESCE(FornProv_Contatti.Rag_Soc + FornProv_Contatti.cognome + ' ' + FornProv_Contatti.Nome, '') AS Fornitore_Prov, ")
            End If
            If flagAziendaUscita Then
                stb.AppendLine(" 	COALESCE(AU_Contatti.Rag_Soc, '') AS Azienda_Fornitore, ")
            End If
            stb.AppendLine(" 	COALESCE(Zoo_Animali.Codice_Azienda_Fornitore, '') as Codice_Azienda_Fornitore, ")
            stb.AppendLine(" 	COALESCE(Zoo_Animali.N_Bolla_Fornitore, '') as N_Bolla_Fornitore, ")
            stb.AppendLine(" 	COALESCE(Zoo_Animali.Data_DDT_Ingresso, CONVERT(datetime, '2100-12-31 00:00:00.000', 120)) as Data_DDT_Ingresso, ")
            stb.AppendLine(" 	COALESCE(Zoo_Animali.Data_Documento_Ingresso, CONVERT(datetime, '2100-12-31 00:00:00.000', 120)) as Data_Documento_Ingresso, ")
            stb.AppendLine(" 	DATEDIFF(day, Zoo_Animali.DAT_NASCITA, Movimenti.Data_Movimento) As Eta_Giorni_TOTALI, ")
            stb.AppendLine("    " & Calcolo_Mesi("Zoo_Animali.Dat_Nascita", "Movimenti.Data_Movimento") & " as eta_mesi,")
            stb.AppendLine("    " & Calcolo_Giorni("Zoo_Animali.Dat_Nascita", "Movimenti.Data_Movimento") & " as eta_giorni, ")
            stb.AppendLine(" 	DATEDIFF(day, Zoo_Animali.Validita_Inizio, Movimenti.Data_Movimento) As Giorni_Stalla, ")
            stb.AppendLine("  Agenda.Id_Agenda AS Id_Operazione, ")
            stb.AppendLine("  CAST(Agenda.Data_Creazione as Date) AS Data_Creazione, ")
            stb.AppendLine("  CAST(Agenda.Data_Modifica as Date) AS Data_Modifica")

            If mostraPesate Then stb.AppendLine(",    ROUND(pesate_cs_cte.Peso_Pagato, 2) Peso_Pagato ")

            stb.AppendLine("  FROM Agenda ")
            stb.AppendLine("  JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")
            stb.AppendLine("  JOIN Movimenti (NOLOCK) ON Agenda.iD_Agenda = Movimenti.ID_Agenda AND Movimenti.Cau_Mov = '3700' ")
            stb.AppendLine("  JOIN Movimenti_Dettagli (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")
            stb.AppendLine("  JOIN Mov_Destinazioni (NOLOCK) ON Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            stb.AppendLine("  INNER Join Zoo_Animali (NOLOCK) ON Zoo_Animali.Cod_Progetto = Movimenti_Dettagli.Cod_Progetto ")
            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                stb.AppendLine("	INNER Join __Tmp_Agenda (NOLOCK) ON Zoo_Animali.Cod_Progetto = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
            End If
            stb.AppendLine("  INNER JOIN Zoo_Animali_Distinte (NOLOCK) ON Zoo_Animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale  ")
            stb.AppendLine(" 				AND CAST(Zoo_Animali_Distinte.Validita_Inizio as date) <= CAST(Movimenti.Data_Movimento as date) ")
            stb.AppendLine(" 				And CAST(Zoo_Animali_Distinte.Validita_Fine as date) >=  CAST(Movimenti.Data_Movimento as date) ")
            stb.AppendLine("  INNER JOIN Zoo_AnimalixStati_Accrescimento (NOLOCK) ON Zoo_AnimalixStati_Accrescimento.Cod_Progetto  = Zoo_Animali.Cod_Progetto ")
            stb.AppendLine(" 				AND CAST(Zoo_AnimalixStati_Accrescimento.Validita_Inizio as datetime) <=  CAST(Movimenti.Data_Movimento as date) ")
            stb.AppendLine(" 				And CAST(Zoo_AnimalixStati_Accrescimento.Validita_Fine as datetime) >=  CAST(Movimenti.Data_Movimento as date) ")
            stb.AppendLine("  INNER JOIN Zoo_Animali_Lista_Stati_Accrescimento (NOLOCK) ON Zoo_AnimalixStati_Accrescimento.GEN_COD = Zoo_Animali_Lista_Stati_Accrescimento.GEN_COD ")
            stb.AppendLine(" 				AND Zoo_AnimalixStati_Accrescimento.SPE_COD = Zoo_Animali_Lista_Stati_Accrescimento.SPE_COD ")
            stb.AppendLine(" 				AND Zoo_AnimalixStati_Accrescimento.STATO_COD = Zoo_Animali_Lista_Stati_Accrescimento.STATO_COD ")
            stb.AppendLine(" 				AND Zoo_AnimalixStati_Accrescimento.TIPO_COD = Zoo_Animali_Lista_Stati_Accrescimento.TIPO_COD ")
            stb.AppendLine("  INNER JOIN Lista_Specie_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Specie_Animali.SPE_COD  ")
            stb.AppendLine("  INNER JOIN Lista_Razze_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD And Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD  ")
            stb.AppendLine("  INNER JOIN Lista_Razze_Animali razza_madre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_madre.GEN_COD And Zoo_Animali.SPE_COD = razza_madre.SPE_COD And Zoo_Animali.Razza_Madre = razza_madre.RAZ_COD   ")
            stb.AppendLine("  INNER JOIN Lista_Razze_Animali razza_padre (NOLOCK) ON Zoo_Animali.GEN_COD = razza_padre.GEN_COD And Zoo_Animali.SPE_COD = razza_padre.SPE_COD And Zoo_Animali.Razza_Padre = razza_padre.RAZ_COD  ")
            stb.AppendLine("  INNER JOIN Zoo_Animali_Lista_Tipi (NOLOCK) ON Zoo_Animali.GEN_COD = Zoo_Animali_Lista_Tipi.GEN_COD And Zoo_Animali.SPE_COD = Zoo_Animali_Lista_Tipi.SPE_COD And Zoo_Animali.TIPO_COD = Zoo_Animali_Lista_Tipi.TIPO_COD  ")
            stb.AppendLine("  INNER JOIN Lista_IndirizziProd_Animali (NOLOCK) ON Zoo_Animali.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD And Zoo_Animali.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD  ")
            stb.AppendLine("  LEFT JOIN Lista_Causali_Morte (NOLOCK) ON Zoo_Animali.Causale_Morte = Lista_Causali_Morte.Cod ")
            stb.AppendLine("  LEFT JOIN Lista_Patologie ON Lista_Patologie.Patologia_Cod = Zoo_Animali.Id_Patologia  ")

            If mostraPesate Then stb.AppendLine("LEFT JOIN #pesate_cs_cte pesate_cs_cte (NOLOCK) ON Agenda.Id_Agenda = pesate_cs_cte.Id_Agenda AND Movimenti_dettagli.Cod_Progetto = pesate_cs_cte.Cod_Progetto ")

            If flagFornitore Then
                stb.AppendLine("  OUTER APPLY (SELECT TOP 1 * FROM Contatti ")
                stb.AppendLine("             WHERE Contatti.Cod_Contatto = Zoo_Animali.CF_Fornitore AND ")
                stb.AppendLine("                (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1)) FornFatt_Contatti")

                stb.AppendLine("  OUTER APPLY (SELECT TOP 1 * FROM Contatti ")
                stb.AppendLine("             WHERE Contatti.Cod_Contatto = Zoo_Animali.Fornitore_Provenienza AND ")
                stb.AppendLine("                (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1)) FornProv_Contatti")

                stb.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti  (NOLOCK)  ")
                stb.AppendLine("             WHERE Contatti.Cod_Contatto = Zoo_Animali.Stalla_Svezzamento AND  ")
                stb.AppendLine("                (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1)) Contatti_StallaSvezz ")

                'stb.AppendLine("LEFT JOIN Contatti AS FornFatt_Contatti (NOLOCK) ON Zoo_Animali.CF_Fornitore = FornFatt_Contatti.Cod_Contatto")
                'stb.AppendLine("    AND Zoo_Animali.Piva = FornFatt_Contatti.Piva")
                'stb.AppendLine("LEFT JOIN Contatti AS FornProv_Contatti (NOLOCK) ON Zoo_Animali.Fornitore_Provenienza = FornProv_Contatti.Cod_Contatto")
                'stb.AppendLine("    AND Zoo_Animali.Piva = FornProv_Contatti.Piva")
            Else
                stb.AppendLine(" LEFT  JOIN Contatti (NOLOCK) ON Zoo_Animali.CF_Fornitore = Contatti.Cod_Contatto AND Zoo_Animali.Piva = Contatti.Piva ")
            End If

            If flagAziendaUscita Then
                stb.AppendLine("  LEFT JOIN Risorse_Umane AS AU_RisorseUmane (NOLOCK) ON Zoo_Animali.Codice_Azienda_Fornitore = AU_RisorseUmane.Attivita_Des AND Zoo_Animali.Codice_Azienda_Fornitore <> '' ")
                stb.AppendLine("  LEFT JOIN Contatti AS AU_Contatti (NOLOCK) ON AU_RisorseUmane.Cod_Contatto = AU_Contatti.Cod_Contatto")
                stb.AppendLine("    AND AU_RisorseUmane.Piva = AU_Contatti.Piva")
            End If

            stb.AppendLine("  JOIN Stalla_Raggruppamenti (NOLOCK) ON Mov_Destinazioni.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod ")
            stb.AppendLine("  JOIN Stalla (NOLOCK) ON Stalla_Raggruppamenti.STA_NUM = Stalla.STA_NUM AND Stalla_Raggruppamenti.PIVA = Stalla.Piva AND Stalla_Raggruppamenti.sa_cod = Stalla.sa_cod ")
            stb.AppendLine("  JOIN Centri_Aziendali (NOLOCK) ON Centri_Aziendali.PIVA = Stalla.Piva AND Centri_Aziendali.sa_cod = Stalla.sa_cod ")
            stb.AppendLine("  JOIN Imprese (NOLOCK) ON Agenda.PIVA = Imprese.PIVA ")
            stb.AppendLine("  JOIN Imprese_Codici (NOLOCK) ON Imprese.Piva = Imprese_Codici.Piva AND Imprese_Codici.id_cod = 1010 ")
            If Filtro_Visibilita_Utente Then
                stb.AppendLine("  INNER JOIN utenti_Visibilita_Appoggio p (NOLOCK) ON Imprese.Piva = p.piva AND p.Sa_Cod = Centri_Aziendali.sa_cod AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            End If
            stb.AppendLine(" WHERE 1 = 1 ")

            If Piva <> "" Then
                stb.AppendLine(" And Imprese.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And Centri_Aziendali.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND Stalla.Sta_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If

            If Raggruppamento_Cod <> 0 Then
                stb.AppendLine(" AND Stalla_Raggruppamenti.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "  ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND Zoo_Animali.Cod_Animale = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If

            If Data_Inizio <> AGRODATAINIZIO Then
                'Movimenti.Data_Movimento
                stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Data_Inizio) & "  ")
            End If

            If Data_Fine <> AGRODATAFINE Then
                stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Data_Fine) & "  ")
            End If

            If Matricola <> "" Then
                stb.AppendLine(" AND Zoo_Animali.Matricola = '" & Agro_SQL_SaveText(Matricola) & "'  ")
            End If

            If Lav_Cod <> 0 Then
                stb.AppendLine(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
            Else
                stb.AppendLine(" AND Agenda.Lav_Cod IN (" & LAVCOD_ACQUISTO_ANIMALI & ", " & LAVCOD_NASCITA_ANIMALI & ", " & LAVCOD_INCREMENTO_CONSISTENZE_ZOO & ")  ")
            End If

            If LivelloCompatibilita(objParametri) >= 150 Then
                stb.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
            End If

            If mostraPesate Then stb.AppendLine("DROP TABLE #pesate_cs_cte ")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function BDN_Leggi_Movimenti_Non_Sincronizzati(Piva As String,
                                      Sa_Cod As Integer,
                                      STA_NUM As Integer,
                                      Raggruppamento_Cod As Integer,
                                      Cod_Animale As Integer,
                                      Data_Inizio As Date,
                                      Data_Fine As Date,
                                      xFiltroAggiuntivo As String,
                                      ByVal Filtro_Visibilita_Utente As Boolean,
                                      ByVal listCod_Animali As List(Of Integer),
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal ConvertiDate As Boolean = False) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.BDN_Leggi_Movimenti_Non_Sincronizzati()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            Dim IDTestataTemp As Integer = 0
            'CancellaRecordDaIDTestataTemp

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                For Each codAnimale In listCod_Animali
                    objTmp_AgendaTemp.Scrivi(IDTestataTemp, "", codAnimale, 0, objParametri)
                Next
            End If

            stb.Length = 0
            stb.AppendLine(" SELECT  ")
            stb.AppendLine(" Imprese.Piva, ")
            stb.AppendLine(" Imprese.rag_soc, ")
            stb.AppendLine(" Imprese_Codici.val_cod as CUAA, ")
            stb.AppendLine(" Centri_Aziendali.Sa_Cod, ")
            stb.AppendLine(" Centri_Aziendali.Sa_Nome, ")
            stb.AppendLine(" Stalla.STA_NUM, ")
            stb.AppendLine(" Stalla.STA_DES, ")
            stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Cod, ")
            stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Des, ")
            stb.AppendLine(" Stalla.BDN_Codice_Azienda, ")
            stb.AppendLine(" Operazioni.LAV_DES, ")
            stb.AppendLine(" Agenda.id_agenda, ")
            stb.AppendLine(" Agenda.Lav_Cod, ")
            stb.AppendLine(" Movimenti.Id_Mov, ")
            stb.AppendLine(" Movimenti_Dettagli.Id_Mov_Det, ")
            If ConvertiDate Then
                stb.AppendLine(" CAST(Movimenti.Data_Movimento as Date) as Data_Movimento, ")
            Else
                stb.AppendLine(" Movimenti.Data_Movimento, ")
            End If
            stb.AppendLine(" Zoo_Animali.Cod_Progetto, ")
            stb.AppendLine(" Zoo_Animali_Distinte.Codice_Distinta, ")
            stb.AppendLine(" Zoo_Animali.Matricola, ")
            stb.AppendLine(" Zoo_Animali.Sesso, ")
            stb.AppendLine(" Zoo_Animali.GEN_COD, ")
            stb.AppendLine(" Zoo_Animali.SPE_COD, ")
            stb.AppendLine(" Lista_Razze_Animali.RAZ_DES, ")
            stb.AppendLine(" Zoo_Animali.Causale_Morte, ")
            stb.AppendLine(" Zoo_Animali.MAT_MADRE, ")
            stb.AppendLine(" Zoo_Animali.Certificato, ")
            stb.AppendLine(" Zoo_Animali.CF_Proprietario, ")
            stb.AppendLine(" Zoo_Animali.CF_Detentore, ")
            stb.AppendLine(" Zoo_Animali.Modello4_Ingresso_Numero, ")
            stb.AppendLine(" Zoo_Animali.Modello4_Ingresso, ")
            stb.AppendLine(" Zoo_Animali.Modello4_Ingresso_Prenotazione, ")
            stb.AppendLine(" Zoo_Animali.Modello4_Uscita_Numero, ")
            stb.AppendLine(" Zoo_Animali.Modello4_Uscita, ")
            stb.AppendLine(" Zoo_Animali.Modello4_Uscita_Prenotazione, ")
            stb.AppendLine("   RIGHT(Zoo_Animali.Matricola, 4) AS Matricola_Breve4,   ")
            stb.AppendLine("   RIGHT(Zoo_Animali.Matricola, 5) AS Matricola_Breve,   ")
            stb.AppendLine(" Zoo_Animali.Lotto_Fornitore ")
            stb.AppendLine(" FROM Agenda ")
            stb.AppendLine(" JOIN Movimenti ON Agenda.Id_Agenda = Movimenti.ID_Agenda ")
            stb.AppendLine(" JOIN Movimenti_Dettagli ON Movimenti.Id_Agenda = Movimenti_Dettagli.ID_Agenda ")
            stb.AppendLine(" 					AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            stb.AppendLine(" JOIN Mov_Destinazioni ON Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.ID_Agenda ")
            stb.AppendLine(" 					AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            stb.AppendLine(" 					AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            stb.AppendLine(" JOIN Zoo_Animali ON Movimenti_dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
            stb.AppendLine(" JOIN Lista_Razze_Animali ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD ")
            stb.AppendLine(" 					AND Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD ")
            stb.AppendLine(" 					AND Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD ")
            stb.AppendLine(" JOIN Zoo_Animali_Distinte ON Zoo_Animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale ")
            stb.AppendLine(" 						AND Zoo_Animali_Distinte.Validita_Inizio <= Movimenti.Data_Movimento ")
            stb.AppendLine(" 						AND Zoo_Animali_Distinte.Validita_Fine >= Movimenti.Data_Movimento ")
            stb.AppendLine(" JOIN Stalla_Raggruppamenti ON Mov_Destinazioni.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod ")
            stb.AppendLine(" JOIN Stalla ON Stalla_Raggruppamenti.Piva = Stalla.PIVA  ")
            stb.AppendLine(" 			AND Stalla_Raggruppamenti.sa_cod = Stalla.sa_cod  ")
            stb.AppendLine(" 			AND Stalla_Raggruppamenti.STA_NUM = Stalla.STA_NUM ")
            stb.AppendLine(" JOIN Imprese ON Agenda.Piva = Imprese.PIVA ")
            stb.AppendLine(" JOIN Imprese_Codici ON Imprese.Piva = Imprese_Codici.PIVA AND Imprese_Codici.id_cod = 1010 ")
            stb.AppendLine(" JOIN Centri_Aziendali ON Imprese.Piva = Centri_Aziendali.PIVA AND Centri_Aziendali.Sa_Cod = Stalla.Sa_Cod ")
            stb.AppendLine(" JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")
            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON Imprese.Piva = p.piva AND p.Sa_Cod = Centri_Aziendali.Sa_Cod AND p.entita_Cod = 2 AND p.Appezza = 0 AND p.ID_Reg = 0 AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            End If
            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                stb.AppendLine("	INNER Join __Tmp_Agenda (NOLOCK) ON Movimenti_Dettagli.Cod_Progetto = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
            End If
            stb.AppendLine(" WHERE Agenda.Lav_Cod IN (3000,3001,3002,3003,3004,3034,3035,3037) ")
            stb.AppendLine(" AND Movimenti.Cau_Mov IN ('3700', '3750') ")
            stb.AppendLine(" AND Movimenti_dettagli.Id_Mov_Esterno = 0 ")

            If Piva <> "" Then
                stb.AppendLine(" And Imprese.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And Centri_Aziendali.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND Stalla.Sta_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If

            If Raggruppamento_Cod <> 0 Then
                stb.AppendLine(" AND Stalla_Raggruppamenti.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "  ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND Zoo_Animali.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & "  ")
            End If

            If Data_Inizio <> AGRODATAINIZIO Then
                'Movimenti.Data_Movimento
                stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Data_Inizio) & "  ")
            End If

            If Data_Fine <> AGRODATAFINE Then
                stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Data_Fine) & "  ")
            End If

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function BDN_Leggi_Movimenti_Sincronizzati(Piva As String,
                                                      Sa_Cod As Integer,
                                                      STA_NUM As Integer,
                                                      Raggruppamento_Cod As Integer,
                                                      Cod_Animale As Integer,
                                                      Data_Inizio As Date,
                                                      Data_Fine As Date,
                                                      xFiltroAggiuntivo As String,
                                                      ByVal Filtro_Visibilita_Utente As Boolean,
                                                      ByVal listCod_Animali As List(Of Integer),
                                                      ByRef objParametri As AgronicaCoreParametri,
                                                      Optional ByVal ConvertiDate As Boolean = False) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.BDN_Leggi_Movimenti_Sincronizzati()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try
            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            Dim IDTestataTemp As Integer = 0
            'CancellaRecordDaIDTestataTemp

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                For Each codAnimale In listCod_Animali
                    objTmp_AgendaTemp.Scrivi(IDTestataTemp, "", codAnimale, 0, objParametri)
                Next
            End If

            stb.Length = 0
            'SELECT
            stb.AppendLine(" SELECT  ")
            stb.AppendLine(" Imprese.Piva, ")
            stb.AppendLine(" Imprese.rag_soc, ")
            stb.AppendLine(" Imprese_Codici.val_cod as CUAA, ")
            stb.AppendLine(" Centri_Aziendali.Sa_Cod, ")
            stb.AppendLine(" Centri_Aziendali.Sa_Nome, ")
            stb.AppendLine(" Stalla.STA_NUM, ")
            stb.AppendLine(" Stalla.STA_DES, ")
            stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Cod, ")
            stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Des, ")
            stb.AppendLine(" Stalla.BDN_Codice_Azienda, ")
            stb.AppendLine(" Operazioni.LAV_DES, ")
            stb.AppendLine(" Agenda.id_agenda, ")
            stb.AppendLine(" Agenda.Lav_Cod, ")
            stb.AppendLine(" Movimenti.Id_Mov, ")
            stb.AppendLine(" Movimenti_Dettagli.Id_Mov_Det, ")
            stb.AppendLine(" Movimenti_Dettagli.Id_Mov_Esterno, ")
            If ConvertiDate Then
                stb.AppendLine(" CAST(Movimenti.Data_Movimento as Date) as Data_Movimento, ")
            Else
                stb.AppendLine(" Movimenti.Data_Movimento, ")
            End If
            stb.AppendLine(" Zoo_Animali.Cod_Progetto, ")
            stb.AppendLine(" Zoo_Animali_Distinte.Codice_Distinta, ")
            stb.AppendLine(" Zoo_Animali.Matricola, ")
            stb.AppendLine(" Zoo_Animali.Sesso, ")
            stb.AppendLine(" Zoo_Animali.GEN_COD, ")
            stb.AppendLine(" Zoo_Animali.SPE_COD, ")
            stb.AppendLine(" Lista_Razze_Animali.RAZ_DES, ")
            stb.AppendLine(" Zoo_Animali.MAT_MADRE, ")
            stb.AppendLine(" Zoo_Animali.Certificato, ")
            stb.AppendLine(" Zoo_Animali.CF_Proprietario, ")
            stb.AppendLine(" Zoo_Animali.CF_Detentore, ")
            stb.AppendLine(" Zoo_Animali.Modello4_Ingresso_Numero, ")
            stb.AppendLine(" Zoo_Animali.Modello4_Ingresso, ")
            stb.AppendLine(" Zoo_Animali.Modello4_Ingresso_Prenotazione, ")
            stb.AppendLine(" Zoo_Animali.Modello4_Uscita_Numero, ")
            stb.AppendLine(" Zoo_Animali.Modello4_Uscita, ")
            stb.AppendLine(" Zoo_Animali.Modello4_Uscita_Prenotazione ")
            stb.AppendLine(" FROM Agenda ")

            'JOIN
            stb.AppendLine(" JOIN Movimenti ON Agenda.Id_Agenda = Movimenti.ID_Agenda ")
            stb.AppendLine(" JOIN Movimenti_Dettagli ON Movimenti.Id_Agenda = Movimenti_Dettagli.ID_Agenda ")
            stb.AppendLine(" 					AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            stb.AppendLine(" JOIN Mov_Destinazioni ON Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.ID_Agenda ")
            stb.AppendLine(" 					AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            stb.AppendLine(" 					AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            stb.AppendLine(" JOIN Zoo_Animali ON Movimenti_dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
            stb.AppendLine(" JOIN Lista_Razze_Animali ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD ")
            stb.AppendLine(" 					AND Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD ")
            stb.AppendLine(" 					AND Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD ")
            stb.AppendLine(" JOIN Zoo_Animali_Distinte ON Zoo_Animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale ")
            stb.AppendLine(" 						AND Zoo_Animali_Distinte.Validita_Inizio <= Movimenti.Data_Movimento ")
            stb.AppendLine(" 						AND Zoo_Animali_Distinte.Validita_Fine >= Movimenti.Data_Movimento ")
            stb.AppendLine(" JOIN Stalla_Raggruppamenti ON Mov_Destinazioni.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod ")
            stb.AppendLine(" JOIN Stalla ON Stalla_Raggruppamenti.Piva = Stalla.PIVA  ")
            stb.AppendLine(" 			AND Stalla_Raggruppamenti.sa_cod = Stalla.sa_cod  ")
            stb.AppendLine(" 			AND Stalla_Raggruppamenti.STA_NUM = Stalla.STA_NUM ")
            stb.AppendLine(" JOIN Imprese ON Agenda.Piva = Imprese.PIVA ")
            stb.AppendLine(" JOIN Imprese_Codici ON Imprese.Piva = Imprese_Codici.PIVA AND Imprese_Codici.id_cod = 1010 ")
            stb.AppendLine(" JOIN Centri_Aziendali ON Imprese.Piva = Centri_Aziendali.PIVA AND Centri_Aziendali.Sa_Cod = Stalla.Sa_Cod ")
            stb.AppendLine(" JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")
            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" INNER JOIN utenti_Visibilita_Appoggio p (NOLOCK) ON Imprese.Piva = p.piva AND p.Sa_Cod = 0 AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            End If
            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 Then
                stb.AppendLine("	INNER JOIN __Tmp_Agenda (NOLOCK) ON Movimenti_Dettagli.Cod_Progetto = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
            End If

            'WHERE
            stb.AppendLine(" WHERE Agenda.Lav_Cod IN (3000,3001,3002,3003,3004,3034,3035,3037) ")
            stb.AppendLine(" AND Movimenti.Cau_Mov IN ('3700', '3750') ")
            stb.AppendLine(" AND Movimenti_dettagli.Id_Mov_Esterno <> 0 ")

            If Piva <> "" Then
                stb.AppendLine(" AND Imprese.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" AND Centri_Aziendali.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            End If

            If STA_NUM <> 0 Then
                stb.AppendLine(" AND Stalla.Sta_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If

            If Raggruppamento_Cod <> 0 Then
                stb.AppendLine(" AND Stalla_Raggruppamenti.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "  ")
            End If

            If Cod_Animale <> 0 Then
                stb.AppendLine(" AND Zoo_Animali.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Animale) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & "  ")
            End If

            If Data_Inizio <> AGRODATAINIZIO Then
                'Movimenti.Data_Movimento
                stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Data_Inizio) & "  ")
            End If

            If Data_Fine <> AGRODATAFINE Then
                stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Data_Fine) & "  ")
            End If

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If listCod_Animali IsNot Nothing AndAlso listCod_Animali.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function trovaMovimentoCarico()

    End Function

    Public Function trovaMovimentoScarico()

    End Function

    Public Function BDN_Leggi_Movimenti_Carico_Non_Sincronizzati(Piva As String,
                                      Sa_Cod As Integer,
                                      STA_NUM As Integer,
                                      Raggruppamento_Cod As Integer,
                                      Cod_Animale As Integer,
                                      Data_Inizio As Date,
                                      Data_Fine As Date,
                                      ByVal Filtro_Visibilita_Utente As Boolean,
                                      ByVal listCod_Animali As List(Of Integer),
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal ConvertiDate As Boolean = False) As DataTable
        Return BDN_Leggi_Movimenti_Non_Sincronizzati(Piva, Sa_Cod, STA_NUM, Raggruppamento_Cod, Cod_Animale, Data_Inizio, Data_Fine, " Movimenti.Cau_Mov = '3700' ", Filtro_Visibilita_Utente, listCod_Animali, objParametri, ConvertiDate)
    End Function

    Public Function BDN_Leggi_Movimenti_Scarico_Non_Sincronizzati(Piva As String,
                                      Sa_Cod As Integer,
                                      STA_NUM As Integer,
                                      Raggruppamento_Cod As Integer,
                                      Cod_Animale As Integer,
                                      Data_Inizio As Date,
                                      Data_Fine As Date,
                                      ByVal Filtro_Visibilita_Utente As Boolean,
                                      ByVal listCod_Animali As List(Of Integer),
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal ConvertiDate As Boolean = False) As DataTable
        Return BDN_Leggi_Movimenti_Non_Sincronizzati(Piva, Sa_Cod, STA_NUM, Raggruppamento_Cod, Cod_Animale, Data_Inizio, Data_Fine, " Movimenti.Cau_Mov = '3750' ", Filtro_Visibilita_Utente, listCod_Animali, objParametri, ConvertiDate)
    End Function

    Public Function BDN_Leggi_Movimenti_Carico_Sincronizzati(Piva As String,
                                                             Sa_Cod As Integer,
                                                             STA_NUM As Integer,
                                                             Raggruppamento_Cod As Integer,
                                                             Cod_Animale As Integer,
                                                             Data_Inizio As Date,
                                                             Data_Fine As Date,
                                                             ByVal Filtro_Visibilita_Utente As Boolean,
                                                             ByVal listCod_Animali As List(Of Integer),
                                                             ByRef objParametri As AgronicaCoreParametri,
                                                             Optional ByVal ConvertiDate As Boolean = False) As DataTable
        Return BDN_Leggi_Movimenti_Sincronizzati(Piva, Sa_Cod, STA_NUM, Raggruppamento_Cod, Cod_Animale, Data_Inizio, Data_Fine, " Movimenti.Cau_Mov = '3700' ", Filtro_Visibilita_Utente, listCod_Animali, objParametri, ConvertiDate)
    End Function

    Public Function BDN_Leggi_Movimenti_Scarico_Sincronizzati(Piva As String,
                                                              Sa_Cod As Integer,
                                                              STA_NUM As Integer,
                                                              Raggruppamento_Cod As Integer,
                                                              Cod_Animale As Integer,
                                                              Data_Inizio As Date,
                                                              Data_Fine As Date,
                                                              ByVal Filtro_Visibilita_Utente As Boolean,
                                                              ByVal listCod_Animali As List(Of Integer),
                                                              ByRef objParametri As AgronicaCoreParametri,
                                                              Optional ByVal ConvertiDate As Boolean = False) As DataTable
        Return BDN_Leggi_Movimenti_Sincronizzati(Piva, Sa_Cod, STA_NUM, Raggruppamento_Cod, Cod_Animale, Data_Inizio, Data_Fine, " Movimenti.Cau_Mov = '3750' ", Filtro_Visibilita_Utente, listCod_Animali, objParametri, ConvertiDate)
    End Function

    Private Function Calcolo_Mesi(campo_from As String, campo_to As String) As String
        Dim strResult = ""
        strResult &= " IIF(DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, " & campo_from & ", " & campo_to & ") , " & campo_from & "), " & campo_to & ") >= 0, " & vbCrLf
        strResult &= "     DATEDIFF(month, " & campo_from & ", " & campo_to & "), " & vbCrLf
        strResult &= "     DATEDIFF(month, " & campo_from & ", " & campo_to & ") -1) "
        Return strResult
    End Function

    Private Function Calcolo_Giorni(campo_from As String, campo_to As String) As String
        Dim strResult = ""
        strResult &= " IIF(DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, " & campo_from & ", " & campo_to & ") , " & campo_from & "), " & campo_to & ") >= 0," & vbCrLf
        strResult &= "     DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, " & campo_from & ", " & campo_to & ") , " & campo_from & "), " & campo_to & ")," & vbCrLf
        strResult &= "     DATEDIFF(day, DATEADD(MONTH, DATEDIFF(month, " & campo_from & ", " & campo_to & ") - 1, " & campo_from & "), " & campo_to & "))"
        Return strResult
    End Function



    Public Function ottieniRazzeNogmo(ByRef objparametri As AgronicaCoreParametri) As DataTable




        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.ottieniRazzeNogmo()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try
            'If listacod_animali.Count = 0 Then
            '    Return Nothing
            'End If


            stb.Length = 0
            stb.AppendLine("select GEN_COD, SPE_COD, RAZ_COD, CODICE ")
            stb.AppendLine("from Codifica_BDN_RazzeAnimali")
            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objparametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objparametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function OttieniDatiNogmo(ByVal listCod_Animali As List(Of String), ByRef objParametri As AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.OttieniDatiNogmo()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            'Dim IDTestataTemp As Integer = 0

            stb.Length = 0

            stb.AppendLine("WITH #StalleSvezzamento as ( ")
            stb.AppendLine("	SELECT Contatti.Cod_Contatto, Contatti.Rag_Soc, Contatti.Nome, Contatti.Cognome, ISTAT.LOCALITA, Indirizzi.frz_des, Rubrica.numero, Risorse_Umane.Settore_Des ")
            stb.AppendLine("	FROM Contatti ")
            stb.AppendLine("	JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva ")
            stb.AppendLine("	JOIN ContattiXIndirizzi ON Contatti.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto AND Contatti.PIVA = ContattiXIndirizzi.Piva ")
            stb.AppendLine("	JOIN Indirizzi ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.Cod_Indirizzo ")
            stb.AppendLine("	JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
            stb.AppendLine("	JOIN ContattiXRubrica ON Contatti.Cod_Contatto = ContattiXRubrica.Cod_Contatto AND Contatti.PIVA = ContattiXRubrica.Piva ")
            stb.AppendLine("	JOIN Rubrica ON ContattiXRubrica.Cod_Rubrica = Rubrica.cod_rubrica AND Rubrica.descr like '%telefono%' ")
            stb.AppendLine("	WHERE Risorse_Umane.Cod_Rapporto IN (-31, -3)  ")
            stb.AppendLine("	AND Risorse_Umane.Settore_Des <> ''  ")
            stb.AppendLine("	AND ((Indirizzi.pro_cod_istat <> '000' AND Indirizzi.com_cod_istat <> '000') OR (Indirizzi.frz_des <> '' AND Indirizzi.frz_des <> 'Non Definita')) ")
            stb.AppendLine(") ")
            stb.AppendLine("SELECT za.Cod_Progetto, za.Matricola, za.Sesso, za.DAT_NASCITA, za.GEN_COD, za.SPE_COD, za.Razza_Madre, za.Razza_Padre, za.RAZ_COD, za.PESO, za.AUSL_AZI_NASCITA, za.Certificato, za.Validita_Inizio, ")
            stb.AppendLine("    COALESCE(IIF(co.LOCALITA <> '', co.LOCALITA, co.frz_des), '') AS StallaSvezz_Comune, COALESCE(co.numero, '') AS StallaSvezz_Telefono,  ")
            stb.AppendLine("    COALESCE(IIF(co.Rag_Soc <> '', co.Rag_Soc, (co.Cognome + ' ' + co.Nome)), '') AS StallaSvezz_RagSoc, COALESCE(co.settore_des, '') AS StallaSvezz_Codice ")
            stb.AppendLine("FROM Zoo_Animali AS za  ")
            stb.AppendLine("LEFT JOIN #StalleSvezzamento co ON (za.Stalla_Svezzamento = co.Cod_Contatto)  ")
            stb.AppendFormat("WHERE za.Cod_Progetto IN {0}", listCod_Animali.ToQueryInExpression).AppendLine()
            stb.AppendLine("GROUP BY za.Cod_Progetto, za.Matricola, za.Sesso, za.DAT_NASCITA, za.GEN_COD, za.SPE_COD,  ")
            stb.AppendLine("    za.Razza_Madre, za.Razza_Padre, za.RAZ_COD, za.PESO, za.AUSL_AZI_NASCITA, za.Certificato, za.Validita_Inizio,  ")
            stb.AppendLine("    co.Rag_Soc, co.Nome, co.Cognome, co.numero, co.frz_des, co.LOCALITA, co.settore_des ")

            'stb.AppendLine("SELECT za.Cod_Progetto, za.Matricola, za.Sesso, za.DAT_NASCITA, za.GEN_COD, za.SPE_COD, za.Razza_Madre, za.Razza_Padre, za.RAZ_COD, za.PESO, za.AUSL_AZI_NASCITA, za.Certificato, za.Validita_Inizio,")
            'stb.AppendLine("    COALESCE(IIF(Indirizzi.com_des <> '', Indirizzi.com_des, Indirizzi.frz_des), '') AS StallaSvezz_Comune, COALESCE(Rubrica.numero, '') AS StallaSvezz_Telefono, ")
            'stb.AppendLine("    COALESCE(IIF(co.Rag_Soc <> '', co.Rag_Soc, (co.Cognome + ' ' + co.Nome)), '') AS StallaSvezz_RagSoc ")
            'stb.AppendLine("FROM Zoo_Animali AS za ")
            'stb.AppendLine("LEFT JOIN Contatti co ON (za.Stalla_Svezzamento = co.Cod_Contatto) ")
            'stb.AppendLine("LEFT JOIN ContattiXIndirizzi ON (co.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto AND za.PIVA = ContattiXIndirizzi.Piva AND za.sa_cod = ContattiXIndirizzi.Sa_Cod) ")
            'stb.AppendLine("LEFT JOIN Indirizzi ON ( ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.Cod_Indirizzo)")
            'stb.AppendLine("LEFT JOIN ContattiXRubrica ON (co.Cod_Contatto = ContattiXRubrica.Cod_Contatto AND za.PIVA = ContattiXRubrica.Piva AND za.sa_cod = ContattiXRubrica.Sa_Cod) ")
            'stb.AppendLine("LEFT JOIN Rubrica ON ( ContattiXRubrica.Cod_Rubrica = Rubrica.cod_rubrica)")
            'stb.AppendFormat("WHERE za.Cod_Progetto IN {0}", listCod_Animali.ToQueryInExpression).AppendLine()
            ''stb.AppendLine("    AND Rubrica.descr = 'Telefono' AND Rubrica.numero <> '#' ")
            'stb.AppendLine("GROUP BY za.Cod_Progetto, za.Matricola, za.Sesso, za.DAT_NASCITA, za.GEN_COD, za.SPE_COD, ")
            'stb.AppendLine("    za.Razza_Madre, za.Razza_Padre, za.RAZ_COD, za.PESO, za.AUSL_AZI_NASCITA, za.Certificato, za.Validita_Inizio, ")
            'stb.AppendLine("    co.Rag_Soc, co.Nome, co.Cognome, Indirizzi.com_des, Rubrica.numero, Indirizzi.frz_des ")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function


    Public Function Movimenti_AnimalixAgenda(Piva As String, ID_Agenda As Integer, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Movimenti_AnimalixAgenda()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            'Dim IDTestataTemp As Integer = 0

            stb.Length = 0

            stb.AppendLine(" SELECT a.PIVA, a.Cod_Progetto, z.Matricola, b.Data_Movimento ")
            stb.AppendLine(" FROM Movimenti_Dettagli a ")
            stb.AppendLine(" JOIN Movimenti b ON a.PIVA = b.PIVA AND a.Id_Agenda = b.Id_Agenda AND a.Id_Mov = b.Id_Mov ")
            stb.AppendLine(" JOIN Zoo_Animali z ON a.Cod_Progetto = z.Cod_Progetto ")
            stb.AppendLine(" WHERE a.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            stb.AppendLine(" AND a.Id_Agenda = " & Agro_SQL_SaveNum(ID_Agenda) & " ")
            stb.AppendLine(" And a.Cod_Progetto <> 0 ")
            stb.AppendLine(" AND a.Elem_Cod = 300 ")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Animale_Movimentato(Piva As String, Cod_Progetto As Integer, Data As DateTime, ByRef objParametri As AgronicaCoreParametri) As Boolean
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Animale_Movimentato()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim movimentato = False
        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            'Dim IDTestataTemp As Integer = 0

            stb.Length = 0

            stb.AppendLine(" SELECT 1  ")
            stb.AppendLine(" FROM Movimenti ")
            stb.AppendLine(" JOIN Movimenti_dettagli ON Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda ")
            stb.AppendLine(" Join Agenda On Movimenti.Id_Agenda = Agenda.Id_Agenda ")
            stb.AppendLine(" Where Movimenti.Data_Movimento > " & Agro_SQL_SaveDateTime(Data) & " ")
            stb.AppendLine("      AND Movimenti_Dettagli.Elem_Cod = " & ZOO_CONSISTENZA & " ")
            stb.AppendLine("	  AND Movimenti_Dettagli.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            stb.AppendLine("      AND Agenda.Lav_Cod NOT IN (" & LAVCOD_INCREMENTO_CONSISTENZE_ZOO & ", " & LAVCOD_ACQUISTO_ANIMALI & ", " & LAVCOD_NASCITA_ANIMALI & ") ")
            stb.AppendLine("      AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & " ")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If dt.Rows.Count > 0 Then
                Return True
            End If
            stb = New StringBuilder
            stb.Length = 0
            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" From Movimenti ")
            stb.AppendLine(" Join Movimenti_Dettagli On Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            stb.AppendLine(" Join Mov_Destinazioni On Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda And ")
            stb.AppendLine(" 						 Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov And ")
            stb.AppendLine(" 						 Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            stb.AppendLine(" Join Agenda On Movimenti.Id_Agenda = Agenda.Id_Agenda ")
            stb.AppendLine(" Where Movimenti.Data_Movimento > " & Agro_SQL_SaveDateTime(Data) & "  ")
            stb.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = " & TIPO_DESTINAZIONE_ANIMALE & " ")
            stb.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Cod_Progetto) & " ")
            stb.AppendLine(" AND Agenda.Lav_Cod NOT IN (" & LAVCOD_INCREMENTO_CONSISTENZE_ZOO & ", " & LAVCOD_ACQUISTO_ANIMALI & ", " & LAVCOD_NASCITA_ANIMALI & ") ")
            stb.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If dt.Rows.Count > 0 Then
                Return True
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return movimentato
    End Function

    Public Function Animali_Movimentati(Piva As String,
                                        progetti As List(Of Integer),
                                        Data As DateTime,
                                        agendeEscluse As List(Of Integer),
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As List(Of String)
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Animale_Movimentato()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt1 As DataTable
        Dim dt2 As DataTable
        Dim matricole As New List(Of String)

        Try

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            'Dim IDTestataTemp As Integer = 0

            stb.Length = 0

            stb.AppendLine(" SELECT Agenda.ID_AGenda, Movimenti.Data_Movimento, Zoo_Animali.Matricola, Zoo_Animali.Cod_Progetto ")
            stb.AppendLine(" FROM Movimenti ")
            stb.AppendLine(" JOIN Movimenti_dettagli ON Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda ")
            stb.AppendLine(" Join Agenda On Movimenti.Id_Agenda = Agenda.Id_Agenda ")
            stb.AppendLine(" JOIN Zoo_Animali ON Movimenti_dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
            stb.AppendLine(" Where Movimenti.Data_Movimento > " & Agro_SQL_SaveDateTime(Data) & " ")
            stb.AppendLine("      AND Movimenti_Dettagli.Elem_Cod = " & ZOO_CONSISTENZA & " ")
            stb.AppendLine("	  AND Movimenti_Dettagli.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            stb.AppendLine("      AND Agenda.Lav_Cod NOT IN (" & LAVCOD_INCREMENTO_CONSISTENZE_ZOO & ", " & LAVCOD_ACQUISTO_ANIMALI & ", " & LAVCOD_NASCITA_ANIMALI & ") ")
            stb.AppendLine("      AND Movimenti_Dettagli.Cod_Progetto IN ( " & Agro_SQL_Save_Clausola_IN(String.Join(",", progetti), False) & " ) ")
            If agendeEscluse IsNot Nothing AndAlso agendeEscluse.Count > 0 Then
                stb.AppendLine("      AND Agenda.ID_Agenda NOT IN ( " & Agro_SQL_Save_Clausola_IN(String.Join(",", agendeEscluse), False) & " ) ")
            End If
            '------------------------------------------------------------------------------------------------------
            dt1 = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            stb = New StringBuilder
            stb.Length = 0
            stb.AppendLine(" SELECT Agenda.ID_AGenda, Movimenti.Data_Movimento, Zoo_Animali.Matricola, Zoo_Animali.Cod_Progetto ")
            stb.AppendLine(" From Movimenti ")
            stb.AppendLine(" Join Movimenti_Dettagli On Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            stb.AppendLine(" Join Mov_Destinazioni On Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda And ")
            stb.AppendLine(" 						 Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov And ")
            stb.AppendLine(" 						 Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            stb.AppendLine(" Join Agenda On Movimenti.Id_Agenda = Agenda.Id_Agenda ")
            stb.AppendLine(" JOIN Zoo_Animali ON Mov_Destinazioni.Id_Destinazione = Zoo_Animali.Cod_Progetto ")
            stb.AppendLine(" Where Movimenti.Data_Movimento > " & Agro_SQL_SaveDateTime(Data) & "  ")
            stb.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = " & TIPO_DESTINAZIONE_ANIMALE & " ")
            stb.AppendLine(" AND Mov_Destinazioni.Id_Destinazione IN ( " & Agro_SQL_Save_Clausola_IN(String.Join(",", progetti), False) & " ) ")
            stb.AppendLine(" AND Agenda.Lav_Cod NOT IN (" & LAVCOD_INCREMENTO_CONSISTENZE_ZOO & ", " & LAVCOD_ACQUISTO_ANIMALI & ", " & LAVCOD_NASCITA_ANIMALI & ") ")
            stb.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            If agendeEscluse IsNot Nothing AndAlso agendeEscluse.Count > 0 Then
                stb.AppendLine("      AND Agenda.ID_Agenda NOT IN ( " & Agro_SQL_Save_Clausola_IN(String.Join(",", agendeEscluse), False) & " ) ")
            End If

            '------------------------------------------------------------------------------------------------------
            dt2 = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------



            If dt1.Rows.Count > 0 Then
                For Each r In dt1.Rows
                    Dim matricola = r("matricola")
                    If matricole.Contains(matricola) = False Then
                        matricole.Add(matricola)
                    End If
                Next
            End If

            If dt2.Rows.Count > 0 Then
                For Each r In dt2.Rows
                    Dim matricola = r("matricola")
                    If matricole.Contains(matricola) = False Then
                        matricole.Add(matricola)
                    End If
                Next
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt2 = Nothing
            dt1 = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return matricole
    End Function

    Public Function CodProgetto_Da_Matricola_E_Stalla(Piva As String, Sa_Cod As Integer, sta_num As Integer, matricola As String, ByRef objParametri As AgronicaCoreParametri) As Integer
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.CodProgetto_Da_Matricola_E_Stalla()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim cod_progetto = 0
        Try
            If Piva = "" Then
                Throw New Exception("Piva non valorizzata")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Sa_Cod non valorizzato")
            End If

            If sta_num = 0 Then
                Throw New Exception("sta_num non valorizzato")
            End If

            If matricola = "" Then
                Throw New Exception("matricola non valorizzata")
            End If


            stb.Length = 0

            stb.AppendLine(" SELECT Zoo_Animali.Cod_Progetto ")
            stb.AppendLine(" FROM Agenda ")
            stb.AppendLine(" JOIN Movimenti ON Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            stb.AppendLine(" JOIN Movimenti_dettagli ON Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            stb.AppendLine(" 						AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")
            stb.AppendLine(" JOIN Mov_Destinazioni ON Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            stb.AppendLine(" 						AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            stb.AppendLine(" 						AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            stb.AppendLine(" JOIN Zoo_Animali ON Movimenti_dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto AND Movimenti_dettagli.Elem_Cod = 300 ")
            stb.AppendLine(" JOIN Stalla_Raggruppamenti ON Mov_Destinazioni.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod ")
            stb.AppendLine(" WHERE Agenda.Lav_Cod IN (3000,3001,3034) ")
            stb.AppendLine(" AND Zoo_Animali.Matricola = '" & Agro_SQL_SaveText(matricola) & "' ")
            stb.AppendLine(" AND Stalla_Raggruppamenti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            stb.AppendLine(" AND Stalla_Raggruppamenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            stb.AppendLine(" AND Stalla_Raggruppamenti.Sta_Num = " & Agro_SQL_SaveNum(sta_num) & " ")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If dt.Rows.Count > 0 Then
                cod_progetto = dt.Rows(0)("Cod_Progetto")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return cod_progetto
    End Function

    Public Function LeggiModello4Inviati(Piva As String,
                                   numModello As String,
                                   prenotazioneId As String,
                                   ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Giacenze2()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        stb.Length = 0
        Try
            stb.AppendLine("SELECT Agenda.Piva, Agenda.Id_Agenda, mvt.Id_Mov, mvtTE.Id_Reg_Dettaglio, mvt.Extra_Str as 'numModello', mvtTE.Num_Riferimento as 'prenotazioneID'").
                AppendLine("FROM Agenda").
                AppendLine("INNER JOIN Movimenti mvt ON mvt.Id_Agenda = Agenda.Id_Agenda").
                AppendLine("INNER JOIN Mov_Dettaglio_Tecnico_Extra mvtTE ON mvtTE.Id_Agenda = mvt.Id_Agenda AND mvtTE.Id_Mov = mvt.Id_Mov").
                AppendLine("WHERE mvt.Extra_Str IS NOT NULL").
                AppendLine("  AND mvt.Extra_Str <> ''").
                AppendLine("  AND mvtTE.Num_Riferimento IS NOT NULL").
                AppendLine("  AND mvtTE.Num_Riferimento <> ''").
                AppendLine("  AND Agenda.Lav_Cod IN (3000,3001,3034) ")
            'Aggiunto questo controllo per verificare effettivamente solo i modelli 4 di Carico capi
            'caso specifico che andava in errore:
            '   Modello 4 di uscita fatto dalla stalla di quarantena verso galvana
            '   Stesso Modello 4 di entrata in galvana non andava l'importazione perché provava ad 'aggiornarlo' invece di importarlo da 0


            If Not String.IsNullOrWhiteSpace(Piva) Then
                stb.AppendFormat("  AND Agenda.Piva = '{0}'", Agro_SQL_SaveText(Piva)).AppendLine()
            End If

            If Not String.IsNullOrWhiteSpace(numModello) Then
                stb.AppendFormat("  AND mvt.Extra_Str = '{0}'", Agro_SQL_SaveText(numModello)).AppendLine()
            End If

            If Not String.IsNullOrWhiteSpace(prenotazioneId) Then
                stb.AppendFormat("  AND mvtTE.Num_Riferimento = '{0}'", Agro_SQL_SaveText(prenotazioneId)).AppendLine()
            End If


            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Somministrazioni_Sincro(ByVal Piva As String,
                                                  ByVal SaCod As Integer,
                                                  ByVal StaNum As Integer,
                                                  ByVal DataInizio As Date,
                                                  ByVal DataFine As Date,
                                                  ByRef objP_Server As AgronicaCoreParametri,
                                                  Optional CodAnimale As Integer = 0,
                                                  Optional Matricola As String = "",
                                                  Optional xFiltroAggiuntivo As String = "",
                                                  Optional Filtro_Visibilita_Utente As Boolean = False,
                                                  Optional id_agendaList As List(Of Integer) = Nothing,
                                                  Optional readingRespSync As Boolean = False) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Somministrazioni_Sincro()"

        Dim msgErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try
            stb.Length = 0

            Dim objTmp_AgendaTemp As New AgronicaCoreVarieDAL.__tmp_Agenda_W
            Dim IDTestataTemp As Integer = 0
            If id_agendaList IsNot Nothing AndAlso id_agendaList.Count > 0 Then

                Dim listTmp_Agenda = (From el In id_agendaList Select New __Tmp_Agenda_Model() With {
                                                                 .id_agenda = el,
                                                                 .piva = "",
                                                                 .lav_cod = 0,
                                                                 .raccoglitore_cod = 0
                                                              }).ToList()
                objTmp_AgendaTemp.ScriviMassivo(IDTestataTemp, listTmp_Agenda, objP_Server)
            End If

            Dim filtraAnimale As Boolean = CodAnimale <> 0 Or Matricola <> ""

            ' CTE per recuperare il numero di somministrazioni per gruppo
            stb.AppendLine("WITH gruppoRicetta AS ( ").
                AppendLine("    SELECT rz.Gruppo_Ricetta, ").
                AppendLine("        COUNT(*) AS Num_Somm_Gruppo ").
                AppendLine("    FROM Ricette_Zoo rz ").
                AppendLine("    GROUP BY rz.Gruppo_Ricetta ").
                AppendLine(") ")
            'AppendLine("), ")

            ' CTE per recuperare la famiglia AIC di ogni prodotto
            'stb.AppendLine("famiglieAIC AS ( ").
            '    AppendLine("    SELECT DISTINCT ").
            '    AppendLine("        LEFT(Farmaci.AIC, 6) AS FamigliaAIC, ").
            '    AppendLine("        Farmaci.Farm_Cod ").
            '    AppendLine("    FROM Farmaci ").
            '    AppendLine(") ")

            ' SELECT
            stb.AppendLine("SELECT rzxa.Id_Ricetta, rzxa.Id_RigaRicetta, rzxa.Id_Agenda, ").
                AppendLine("    ag.PIVA, imp.rag_soc AS Rag_Soc, ").
                AppendLine("    ag.Sa_Cod, centri.sa_nome AS Sa_Des, ").
                AppendLine("    ag.Sta_Num, sta.STA_DES AS Sta_Des, ").
                AppendLine("    movdett.Pro_Cod, ").
                AppendLine("    movdett.Mov_Det_Des AS Pro_Des, ").
                AppendLine("    movdett.Qta, ").
                AppendLine("    Farmaci.AIC, ").
                AppendLine("    LEFT(Farmaci.AIC, 6) AS FamigliaAIC, ")

            ' AGGREGATE per Prodotti (Cod, Des, Qta):
            ' Raggruppa se stesso trattamento usa stesso prodotto, ma diverse confezioni 
            ' Pro_Cod: (Pro_Cod1, Prod_Cod2, ...)
            ' Pro_Des: Farmaci.Denominazione + ' - TOT: ' + SUM(Qta) + ' (Dettagli1; Dettagli2; ...)'
            ' Qta: SUM(Qta)
            'stb.AppendLine("    STRING_AGG(CAST(movdett.Pro_Cod AS VARCHAR(20)), ', ') WITHIN GROUP (ORDER BY movdett.Pro_Cod) AS Pro_Cod, ").
            '    AppendLine("    CASE WHEN COUNT(DISTINCT ParsedDesc.BaseName) = 1 THEN ").
            '    AppendLine("        MIN(ParsedDesc.BaseName) + ").
            '    AppendLine("        ' - TOT: ' + ").
            '    AppendLine("        CAST(CAST(SUM(movdett.Qta) AS INT) AS VARCHAR(20)) + ").
            '    AppendLine("        ' (' + ").
            '    AppendLine("        STRING_AGG(ParsedDesc.Details, '; ') WITHIN GROUP (ORDER BY movdett.Pro_Cod) + ").
            '    AppendLine("        ')' ").
            '    AppendLine("    ELSE ").
            '    AppendLine("        STRING_AGG(movdett.Mov_Det_Des, '; ') WITHIN GROUP (ORDER BY movdett.Pro_Cod) ").
            '    AppendLine("    END AS Pro_Des, ").
            '    AppendLine("    SUM(movdett.Qta) AS Qta, ").
            '    AppendLine("    CASE ").
            '    AppendLine("        WHEN COUNT(DISTINCT movdett.Pro_Cod) > 1 THEN 1 ").
            '    AppendLine("        ELSE 0 ").
            '    AppendLine("     END AS MultiAIC, ")

            stb.AppendLine("    movdett.Udm_Cod, ").
                AppendLine("    COALESCE(movdett.Rif_Esterno, '') AS Tratt_Numero, ").
                AppendLine("    COALESCE(movdett.Extra_Str, '') AS Somm_Numero, ").
                AppendLine("    COALESCE(rz.Numero, '') AS Pres_Numero, ").
                AppendLine("    COALESCE(rza.Numero, '') AS PresRiga_Numero, ").
                AppendLine("    rz.DataEmissione AS Data_Prescrizione, ").
                AppendLine("    rz.TipoCodice AS Pres_Tipo, ").
                AppendLine("    COALESCE(movdett.RegSco_Numero, '') AS RegSco_Numero, ").
                AppendLine("    COALESCE(rz.Gruppo_Ricetta, 0) AS Gruppo_Ricetta, ").
                AppendLine("    rz.ProtocolloCodice AS Prot_Numero, ").
                AppendLine("    COALESCE(rz.Id_Protocollo, 0) AS Id_Protocollo, ").
                AppendLine("    rza.Numero_Somm, rza.Note AS Num_Somm_Des, movz.Note, ").
                AppendLine("    sta.GEN_COD, sta.SPE_COD, ").
                AppendLine("    sta.BDN_Codice_Azienda AS Azienda_Codice, ").
                AppendLine("    rz.ProprietarioIdFiscale AS Prop_IdFiscale, ").
                AppendLine("    ag.Validita_Inizio AS Data_Inizio, ").
                AppendLine("    ag.Validita_Fine AS Data_Fine, ").
                AppendLine("    COALESCE(movz.Tipo_Trattamento, 0) AS Somm_Tipo, ").
                AppendLine("    COALESCE(movz.Stato_Trattamento, 0) AS Somm_Stato, ").
                AppendLine("    gr.Num_Somm_Gruppo ")

            If filtraAnimale Then stb.AppendLine("    , zoo.Cod_Progetto, zoo.Matricola, zoo.Sesso, zoo.DAT_NASCITA AS Data_Nascita ")

            ' JOIN
            stb.AppendLine("FROM Agenda ag ").
                AppendLine("INNER JOIN Ricette_ZooxAgenda rzxa ON rzxa.Id_Agenda = ag.Id_Agenda ").
                AppendLine("INNER JOIN Ricette_Zoo rz ON rz.IdRicetta = rzxa.Id_Ricetta ").
                AppendLine("INNER JOIN Ricette_Zoo_Agenda rza ON rza.IdRicetta = rz.IdRicetta AND rza.IdAgenda = rzxa.Id_RigaRicetta ").
                AppendLine("INNER JOIN Movimenti mov ON mov.Id_Agenda = ag.Id_Agenda ").
                AppendLine("LEFT JOIN Movimenti_Zoo movz ON movz.Id_Agenda = mov.Id_Agenda AND movz.Id_Mov = mov.Id_Mov ").
                AppendLine("INNER JOIN Movimenti_dettagli movdett ON movdett.Id_Agenda = mov.Id_Agenda AND movdett.Id_Mov = mov.Id_Mov ").
                AppendLine("INNER JOIN Imprese imp ON imp.piva = ag.PIVA ").
                AppendLine("INNER JOIN Centri_Aziendali centri ON centri.piva = ag.PIVA AND centri.sa_cod = ag.Sa_Cod ").
                AppendLine("INNER JOIN Stalla sta ON sta.piva = ag.PIVA AND sta.sa_cod = ag.sa_cod AND sta.STA_NUM = ag.Sta_Num ").
                AppendLine("LEFT JOIN gruppoRicetta gr ON gr.Gruppo_Ricetta = rz.Gruppo_Ricetta ").
                AppendLine("INNER JOIN Farmaci farmaci ON farmaci.Farm_Cod = movdett.Pro_Cod ")
            'AppendLine("INNER JOIN famiglieAIC fAIC ON fAIC.Farm_Cod = movdett.Pro_Cod ")

            ' Gestione per raggruppamento Pro_Des in caso di stesso trattamento
            ' con stesso prodotto, ma diverse confezioni ((stessa famiglia AIC, ma AIC specifico diverso)
            'stb.AppendLine("CROSS APPLY ( ").
            '    AppendLine("    SELECT ParenPos = CHARINDEX(' (', movdett.Mov_Det_Des) ").
            '    AppendLine(") AS ParenCalc ").
            '    AppendLine("OUTER APPLY ( ").
            '    AppendLine("    SELECT ").
            '    AppendLine("        BaseName = CASE ").
            '    AppendLine("            WHEN ParenCalc.ParenPos > 0 THEN LEFT(movdett.Mov_Det_Des, ParenCalc.ParenPos - 1) ").
            '    AppendLine("            ELSE movdett.Mov_Det_Des ").
            '    AppendLine("        END, ").
            '    AppendLine("        Details = CASE ").
            '    AppendLine("            WHEN ParenCalc.ParenPos > 0 THEN ").
            '    AppendLine("                SUBSTRING( ").
            '    AppendLine("                    movdett.Mov_Det_Des, ").
            '    AppendLine("                    ParenCalc.ParenPos + 2, ").
            '    AppendLine("                    CHARINDEX(')', movdett.Mov_Det_Des, ParenCalc.ParenPos) - (ParenCalc.ParenPos + 2) ").
            '    AppendLine("                ) ").
            '    AppendLine("        ELSE '' ").
            '    AppendLine("    END ").
            '    AppendLine(") AS ParsedDesc ")

            If id_agendaList IsNot Nothing AndAlso id_agendaList.Count > 0 Then
                stb.AppendLine("	INNER Join __Tmp_Agenda (NOLOCK) ON ag.ID_Agenda = __Tmp_Agenda.id_agenda AND __Tmp_Agenda.Piva = '' AND __Tmp_Agenda.IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp) & " ")
            End If

            If filtraAnimale Then stb.AppendLine("INNER JOIN Mov_Destinazioni movdest ON movdest.Id_Agenda = movdett.Id_Agenda ").
                                      AppendLine("    AND movdest.Id_Mov = movdett.Id_Mov AND movdest.Id_Mov_Det = movdett.Id_Mov_Det ").
                                      AppendLine("INNER JOIN Zoo_Animali zoo ON zoo.Cod_Progetto = movdest.Id_Destinazione ")

            If Filtro_Visibilita_Utente Then stb.AppendLine("INNER JOIN utenti_Visibilita_Appoggio p (NOLOCK) ON Agenda.Piva = p.piva AND p.Sa_Cod = giac_cte.Sa_Cod ").
                                                 AppendLine("    AND p.Username = " & Agro_SQL_SaveText_NULL(objP_Server.UtenteUsername) & " ")

            ' WHERE
            stb.AppendLine("WHERE 1=1 ").
                AppendLine("    AND ag.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_CUREMEDICAMENTI_ANIMALI) & " ").
                AppendLine("    AND mov.Cau_Mov = '" & Agro_SQL_SaveText(CAU_TRATTAMENTO_ZOO) & "' ").
                AppendLine($"    AND rz.TipoCodice IN({CInt(enum_TipoPrescrizione.Da_Protocollo_GIAS)}, {CInt(enum_TipoPrescrizione.Veterinaria)}, {CInt(enum_TipoPrescrizione.Indicazione_Terapeutica)}) ")

            If Not readingRespSync Then
                stb.AppendLine("    AND ag.Blocco_Flag = 0 ") ' esclude trattamenti chiusi su vetinfo
                stb.AppendLine($"    AND movz.Stato_Trattamento IN ({CInt(enum_StatoTrattamento.UNDEFINED)}, {CInt(enum_StatoTrattamento.Aperto)}) ") ' esclude quelli già inviati
            End If

            If Piva <> "" Then stb.AppendLine("    And ag.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If SaCod <> 0 Then stb.AppendLine("    AND ag.Sa_Cod = " & Agro_SQL_SaveNum(SaCod) & " ")

            If StaNum <> 0 Then stb.AppendLine("    AND ag.Sta_Num = " & Agro_SQL_SaveNum(StaNum) & " ")

            If DataInizio > AGRODATAINIZIO Then stb.AppendLine("    AND ag.Validita_Inizio >= " & Agro_SQL_SaveDate(DataInizio) & " ")

            If DataFine < AGRODATAFINE Then stb.AppendLine("    AND ag.Validita_Fine <= " & Agro_SQL_SaveDate(DataFine) & " ")

            If CodAnimale <> 0 Then stb.AppendLine("    AND zoo.Cod_Animale = " & Agro_SQL_SaveNum(CodAnimale) & " ")

            If Matricola <> "" Then stb.AppendLine("    AND zoo.Matricola = '" & Agro_SQL_SaveText(Matricola) & "' ")

            If xFiltroAggiuntivo <> "" Then stb.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & "  ")

            ' GROUP BY
            'stb.AppendLine("GROUP BY ").
            '    AppendLine("    rzxa.Id_Ricetta, rzxa.Id_RigaRicetta, rzxa.Id_Agenda, ").
            '    AppendLine("    ag.PIVA, imp.rag_soc, ").
            '    AppendLine("    ag.Sa_Cod, centri.sa_nome, ").
            '    AppendLine("    ag.Sta_Num, sta.STA_DES, ").
            '    AppendLine("    fAIC.FamigliaAIC, ").
            '    AppendLine("    movdett.Udm_Cod, ").
            '    AppendLine("    COALESCE(movdett.Rif_Esterno, ''), ").
            '    AppendLine("    COALESCE(movdett.Extra_Str, ''), ").
            '    AppendLine("    COALESCE(rz.Numero, ''), ").
            '    AppendLine("    COALESCE(rza.Numero, ''), ").
            '    AppendLine("    rz.DataEmissione, ").
            '    AppendLine("    rz.TipoCodice, ").
            '    AppendLine("    COALESCE(movdett.RegSco_Numero, ''), ").
            '    AppendLine("    COALESCE(rz.Gruppo_Ricetta, 0), ").
            '    AppendLine("    rz.ProtocolloCodice, ").
            '    AppendLine("    COALESCE(rz.Id_Protocollo, 0), ").
            '    AppendLine("    rza.Numero_Somm, rza.Note, movz.Note, ").
            '    AppendLine("    sta.GEN_COD, sta.SPE_COD, ").
            '    AppendLine("    sta.BDN_Codice_Azienda, rz.ProprietarioIdFiscale, ").
            '    AppendLine("    ag.Validita_Inizio, ag.Validita_Fine, ").
            '    AppendLine("    COALESCE(movz.Tipo_Trattamento, 0), ").
            '    AppendLine("    COALESCE(movz.Stato_Trattamento, 0), ").
            '    AppendLine("    gr.Num_Somm_Gruppo")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objP_Server, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

            If id_agendaList IsNot Nothing AndAlso id_agendaList.Count > 0 AndAlso IDTestataTemp <> 0 Then
                objTmp_AgendaTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objP_Server)
            End If

        Catch ex As Exception
            msgErrore = ex.Message
            Scrivi_LOG(objP_Server, nomeRoutine, msgErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & msgErrore)
        End Try

        Return dt

    End Function

    Public Function GetCapiFromSomministrazioni(ByVal listIdAgenda As List(Of Integer),
                                                ByRef objP_Server As AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Somministrazioni_Sincro()"

        Dim msgErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try
            stb.Length = 0

            stb.AppendLine("SELECT a.Id_Agenda, ").
                AppendLine("    mdtt.Rif_Esterno, ").
                AppendLine("    mdtt.Extra_Str, ").
                AppendLine("    mdst.Qta, ").
                AppendLine("    mdst.QuotaDistribuzione, ").
                AppendLine("    zoo.Cod_Progetto, ").
                AppendLine("    zoo.Matricola, ").
                AppendLine("    zoo.DAT_NASCITA, ").
                AppendLine("    zoo.GEN_COD, ").
                AppendLine("    zoo.SPE_COD, ").
                AppendLine("    zoo.RAZ_COD, ").
                AppendLine("    zoo.Sesso ")

            stb.AppendLine("FROM Agenda a ").
                AppendLine("INNER JOIN Movimenti mov ON mov.Id_Agenda = a.Id_Agenda ").
                AppendLine("INNER JOIN Movimenti_dettagli mdtt ON mdtt.Id_Agenda = mov.Id_Agenda AND mdtt.Id_Mov = mov.Id_Mov ").
                AppendLine("INNER JOIN Mov_Destinazioni mdst ON mdst.Id_Agenda = mdtt.Id_Agenda AND mdst.Id_Mov = mdtt.Id_Mov AND mdst.Id_Mov_Det = mdtt.Id_Mov_Det ").
                AppendLine("INNER JOIN Zoo_Animali zoo ON zoo.Cod_Progetto = mdst.Id_Destinazione ")

            stb.AppendLine($"WHERE mov.Cau_Mov = {CAU_TRATTAMENTO_ZOO} ").
                AppendLine("    AND a.Id_Agenda IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", listIdAgenda), False) & ") ")

            '------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objP_Server, stb.ToString, nomeRoutine)
            '------------------------------------------------------------------------------------------------------

        Catch ex As Exception
            msgErrore = ex.Message
            Scrivi_LOG(objP_Server, nomeRoutine, msgErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & msgErrore)
        End Try

        Return dt

    End Function

End Class