Imports System.Data.Entity
Imports System.Linq
Imports System.Text
Imports System.Transactions
Imports System.Web
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.exceptions

Public Class Replica_GIAS
    Inherits AgronicaCoreDataProvider.LogProvider

    ' leggo algoritmo codifica automatica progressivi
    Public Shared Function LeggiAlgoritmoCodifica(ByVal piva As String, ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim objImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Return objImpreseCodici.Leggi_Codice_from_Imprese_Codici(piva, enum_CodiciAnagrafe.Algoritmo_Codifica, objParametri_Server)
    End Function

    ' restituisce codice progressivo in base alla tipologia
    Public Shared Function LeggiCodiceProgressivo(ByVal piva As String, ByVal tipo As Integer, ByVal anno As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim algoritmo = LeggiAlgoritmoCodifica(piva, objParametri_Server)
        Return LeggiCodiceProgressivo(piva, algoritmo, tipo, anno, objParametri_Server)
    End Function

    ' restituisce codice progressivo in base all'algoritmo di codifica
    Public Shared Function LeggiCodiceProgressivo(ByVal piva As String, ByVal algoritmo As String, ByVal tipo As Integer, ByVal anno As Integer, ByRef objParametri_Server As AgronicaCoreParametri, Optional ByVal replica As Boolean = False) As String
        Dim progressivo As String = ""
        Select Case algoritmo
            Case "1", "2" ' Algoritmo per codifica Aboca - SAM
                Dim dal As New Sequenza_Progressivi_R
                If tipo = enum_SequenzaProgressiviTipi.CodiciOPAgriZoo Then
                    Dim prg = dal.Nuovo_Progressivo_UpdateImmediato(piva, anno, tipo, "", "", 0, objParametri_Server)
                    progressivo = anno.ToString.Substring(2) & prg.ToString("D6")
                ElseIf algoritmo = "1" OrElse Not replica Then
                    Dim prefisso As String = If(tipo = enum_SequenzaProgressiviTipi.CodiciProgettoZootecnici, "ZO", "PG")
                    Dim dt = dal.Leggi2(piva, 0, tipo, "", "", objParametri_Server)
                    If dt.Rows.Count > 0 Then
                        prefisso = dt.Rows(0).Item("Doc_Numero_Sin")
                    End If
                    Dim prg = dal.Nuovo_Progressivo_UpdateImmediato(piva, 0, tipo, prefisso, "", 0, objParametri_Server)
                    progressivo = prefisso & prg
                End If
        End Select
        Return progressivo
    End Function

    Public Shared Function LeggiConfigReplicaGIAS(ByRef Piva As String, ByRef Sa_Cod As Integer, ByRef Prefix As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim dtConfigSiti As DataTable = objConfigSiti.Leggi(0, "Replica_GIAS", "", "", objParametri)

        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 Then
            Dim configurazione = dtConfigSiti.Rows(0)("valore").ToString
            Dim config = configurazione.Split("|")
            If config.Length Mod 5 = 0 Then
                Dim objCentro As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Dim numRows = config.Length \ 5
                For i As Integer = 1 To numRows
                    Dim index As Integer = (i - 1) * 5
                    Dim Piva_From = config(0 + index)
                    Dim Sa_Cod_From = config(1 + index)
                    If Piva = Piva_From AndAlso (Sa_Cod = Sa_Cod_From OrElse Sa_Cod_From = 0) Then
                        Piva = config(2 + index)
                        Sa_Cod = config(3 + index)
                        Prefix = config(4 + index)
                        Return True
                    End If
                Next
            End If
        End If

        Return False
    End Function

    Public Shared Function IsSimpleType(ByVal type As Type) As Boolean
        Return type.IsValueType OrElse type.IsPrimitive OrElse New Type() {GetType(String), GetType(Decimal), GetType(DateTime), GetType(DateTimeOffset), GetType(TimeSpan), GetType(Guid)}.Contains(type) OrElse Convert.GetTypeCode(type) <> TypeCode.Object
    End Function

    Public Shared Function CopyEntity(Of T As Class)(ByRef ctx As Gias_DeveloperServer_Entities, ByRef entity As T, ByVal username As String, ByVal data As Date) As T
        '11/02/2021: questa riga andava bene per l'ObjectContext, ma non per il DbContext
        'Dim clone As T = ctx.CreateObject(Of T)()
        Dim clone As T = ctx.Set(Of T)().Create()
        Dim propInfo = entity.GetType().GetProperties() '.Where(Function(p) p.CanRead AndAlso p.CanWrite)
        For Each item In propInfo
            If item.Name = "Username_Creazione" OrElse item.Name = "Username_Modifica" Then
                clone.GetType().GetProperty(item.Name).SetValue(clone, username, Nothing)
            ElseIf item.Name = "Data_Creazione" OrElse item.Name = "Data_Modifica" Then
                clone.GetType().GetProperty(item.Name).SetValue(clone, data, Nothing)
            ElseIf IsSimpleType(item.PropertyType) Then
                clone.GetType().GetProperty(item.Name).SetValue(clone, item.GetValue(entity, Nothing), Nothing)
            End If
        Next
        Return clone
    End Function

    Public Shared Function LeggiCodiceCentro(ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal codice As Integer, ByRef ctx As Gias_DeveloperServer_Entities) As Centri_Aziendali_Codici
        Return (From a In ctx.Centri_Aziendali_Codici Where a.PIVA = Piva AndAlso a.sa_cod = Sa_Cod AndAlso a.id_cod = codice).FirstOrDefault
    End Function

    Public Shared Function ScriviCodiceCentro(ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal codice As Integer, ByVal valore As String, ByVal username As String, ByRef ctx As Gias_DeveloperServer_Entities) As Centri_Aziendali_Codici
        Dim centroCod As New Centri_Aziendali_Codici With {
            .PIVA = Piva,
            .sa_cod = Sa_Cod,
            .id_cod = codice,
            .val_cod = valore,
            .inviato = 0,
            .Data_Creazione = DateTime.Now,
            .Data_Modifica = DateTime.Now,
            .Validita_Inizio = AGRODATAINIZIO,
            .Validita_Fine = AGRODATAFINE,
            .Username_Creazione = username,
            .Username_Modifica = username,
            .Validazione = 0,
            .Data_Validazione = DateTime.Now,
            .UserName_Validazione = ""
        }
        ctx.Centri_Aziendali_Codici.Add(centroCod)
        ctx.SaveChanges()
        Return centroCod
    End Function

    Public Shared Function LeggiCodiceImpianto(ByVal Piva As String, ByVal sa_cod As Integer, ByVal appezza As Integer, ByVal id_reg As Integer, ByVal codice As Integer, ByRef ctx As Gias_DeveloperServer_Entities) As Reg_Impianti_Codici
        Return (From a In ctx.Reg_Impianti_Codici Where a.PIVA = Piva AndAlso a.sa_cod = sa_cod AndAlso a.appezza = appezza AndAlso a.Id_Reg = id_reg AndAlso a.id_cod = codice).FirstOrDefault
    End Function

    Public Shared Function LeggiCodiceProgetto(ByVal Piva As String, ByVal Progetto_Cod As Integer, ByVal codice As Integer, ByRef ctx As Gias_DeveloperServer_Entities) As Reg_Impianti_Codici
        Return (From a In ctx.Reg_Impianti_Codici Where a.PIVA = Piva AndAlso a.Progetto_Cod = Progetto_Cod AndAlso a.id_cod = codice).FirstOrDefault
    End Function

    Public Shared Function ScriviCodiceProgetto(ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal appezza As Integer, ByVal id_reg As Integer, ByVal progetto_cod As Integer, ByVal codice As Integer, ByVal valore As String, ByVal username As String, ByRef ctx As Gias_DeveloperServer_Entities) As Reg_Impianti_Codici
        Dim impiantoCod As New AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici With {
            .PIVA = Piva,
            .sa_cod = Sa_Cod,
            .appezza = appezza,
            .Id_Reg = id_reg,
            .Progetto_Cod = progetto_cod,
            .id_cod = codice,
            .val_cod = valore,
            .inviato = 0,
            .Data_Creazione = DateTime.Now,
            .Data_Modifica = DateTime.Now,
            .Validita_Inizio = AGRODATAINIZIO,
            .Validita_Fine = AGRODATAFINE,
            .Username_Creazione = username,
            .Username_Modifica = username
        }
        ctx.Reg_Impianti_Codici.Add(impiantoCod)
        ctx.SaveChanges()
        Return impiantoCod
    End Function

    Public Function LeggiCentroAziendaleDestinazione(ByRef Piva As String, ByRef Sa_Cod As Integer, ByRef Piva_To As String, ByRef Sa_Cod_To As Integer, ByRef ctx As Gias_DeveloperServer_Entities) As Boolean
        If Sa_Cod_To = 0 Then
            Dim Codice_Centro = LeggiCodiceCentro(Piva, Sa_Cod, enum_CodiciAnagrafe.Codice_Centro, ctx)
            Dim Valore_Centro As String = If(Codice_Centro Is Nothing, "", Codice_Centro.val_cod)
            If Not String.IsNullOrEmpty(Valore_Centro) Then
                Dim Valori = Valore_Centro.Split("|")
                If Valori.Length = 2 Then
                    Piva_To = Valori(0)
                    Sa_Cod_To = CInt(Valori(1))
                End If
            End If
        End If
        Return Sa_Cod_To = 0
    End Function

    Public Function LeggiImpiantoDestinazione(ByVal Piva As String, ByVal sa_cod As Integer, ByRef appezza As Integer, ByRef id_reg As Integer, ByRef ctx As Gias_DeveloperServer_Entities) As Boolean
        Dim Codice_Impianto = LeggiCodiceImpianto(Piva, sa_cod, appezza, id_reg, enum_CodiciAnagrafe.Codice_Impianto_Ribaltato, ctx)
        Dim Valore_Impianto As String = If(Codice_Impianto Is Nothing, "", Codice_Impianto.val_cod)
        If Not String.IsNullOrEmpty(Valore_Impianto) Then
            Dim Valori = Valore_Impianto.Split("|")
            If Valori.Length >= 4 Then
                appezza = CInt(Valori(2))
                id_reg = CInt(Valori(3))
                Return False
            End If
        End If
        Return True
    End Function

    Public Function LeggiDistintaDestinazione(ByVal Piva As String, ByVal Progetto_Cod As Integer, ByRef ctx As Gias_DeveloperServer_Entities) As Boolean
        Dim Distinta = (From a In ctx.Imprese_Progetti Where a.Piva = Piva AndAlso a.Progetto_Cod = Progetto_Cod).FirstOrDefault
        If Distinta IsNot Nothing Then
            Dim Distinta_Chiusa = LeggiCodiceProgetto(Piva, Progetto_Cod, enum_CodiciAnagrafe.Distinta_Chiusa, ctx)
            Dim Distinta_Ribaltata = LeggiCodiceProgetto(Piva, Progetto_Cod, enum_CodiciAnagrafe.Codice_Impianto_Ribaltato, ctx)
            Return Distinta_Chiusa IsNot Nothing AndAlso Distinta_Chiusa.val_cod = "1" AndAlso Distinta_Ribaltata Is Nothing
        End If
        Return False
    End Function

    ' verifica se distinta chiusa e non ribaltata
    Public Function VerificaDistinta(ByVal Piva As String, ByVal Progetto_Cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim objRegImpiantiCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim Distinta_Chiusa = objRegImpiantiCodici.LeggiValCod_2(Piva, 0, 0, 0, Progetto_Cod, enum_CodiciAnagrafe.Distinta_Chiusa, False, "", "", objParametri)
        Dim Distinta_Ribaltata = objRegImpiantiCodici.LeggiValCod_2(Piva, 0, 0, 0, Progetto_Cod, enum_CodiciAnagrafe.Codice_Impianto_Ribaltato, False, "", "", objParametri)
        Return Distinta_Ribaltata = "" ' AndAlso Distinta_Chiusa = "1" 
    End Function

    ' verifica configurazione ribaltamento, restituisce la piva dell'azienda destinazione
    Public Shared Function VerificaConfigurazione(ByVal Piva As String, ByVal Sa_Cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim Piva_To As String = Piva
        Dim Sa_Cod_To As Integer = Sa_Cod
        Dim Prefix As String = ""
        If LeggiConfigReplicaGIAS(Piva_To, Sa_Cod_To, Prefix, objParametri) Then
            Return Piva_To
        End If
        Return ""
    End Function

    Public Function Replica_Impianto(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal appezza As Integer,
                                     ByVal id_reg As Integer,
                                     ByVal campo_cod As Integer,
                                     ByVal progetto_cod As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByRef objParametriUtente As AgronicaCoreParametri = Nothing) As String

        Dim nomeRoutine As String = "Replica_GIAS.Replica_Impianto"
        Dim messaggioErrore As String = ""
        Dim log = New StringBuilder

        ' Controllo nella configurazione siti (chiave “Replica_GIAS”) se è attiva la replica dei dati impianto
        Dim Piva_To As String = Piva
        Dim Sa_Cod_To As Integer = Sa_Cod
        Dim Appezza_To As Integer = appezza
        Dim Id_Reg_To As Integer = id_reg
        Dim Prefix As String = ""
        Dim Replica_GIAS = LeggiConfigReplicaGIAS(Piva_To, Sa_Cod_To, Prefix, objParametri)

        ' Verifico se attivo ribaltamento e se la distinta impianto è chiusa e non è già stata ribaltata
        If Replica_GIAS AndAlso VerificaDistinta(Piva, progetto_cod, objParametri) Then

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim PivaSuperUser = objParametri.PivaSuperUser
            Dim username = objParametri.UsernameOperazione
            Dim data = DateTime.Now

            Using scope As New TransactionScope()

                Try

                    Dim BaseCode As Long = 0
                    Dim TopCode As Long = 200000000
                    Dim Progressivo As Long = 0

                    If IsNothing(HttpContext.Current.Session) Then
                        Progressivo = 0
                        Dim DT As DataTable
                        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read

                        DT = objUtenti.Leggi_Superuser_e_ProgressivoGIAS(
                            objParametriUtente.SuperUserUsername,
                            "",
                            Date.Now,
                            CType(Now.Hour, Short),
                            5,
                            objParametriUtente
                            )

                        If DT.Rows.Count > 0 Then
                            Progressivo = DT.Rows(0).Item("ProgressivoGIAS")
                        Else
                            Throw New Exception("ProgressivoGias non trovato")
                        End If
                    Else
                        Progressivo = HttpContext.Current.Session("ASG_ProgressivoGIAS")
                    End If

                    agroDP.Calcola_BaseCode(Progressivo, TopCode, BaseCode, objParametri)

                    ' Verifico se esiste già il centro aziendale destinazione
                    If LeggiCentroAziendaleDestinazione(Piva, Sa_Cod, Piva_To, Sa_Cod_To, GiasContext) Then

                        ' Centro Aziendale origine
                        Dim CentroAziendale_From = (From a In GiasContext.Centri_Aziendali Where a.PIVA = Piva AndAlso a.sa_cod = Sa_Cod).FirstOrDefault

                        ' Ricavo l'id del centro aziendale destinazione
                        Sa_Cod_To = agroDP.NuovoId_CentriAziendali(Piva_To, BaseCode, TopCode, objParametri)

                        ' Copio il centro aziendale per l'azienda destinazione aggiungendo alla descrizione il prefisso
                        Dim CentroAziendale_To = CopyEntity(GiasContext, CentroAziendale_From, username, data)
                        CentroAziendale_To.PIVA = Piva_To
                        CentroAziendale_To.sa_cod = Sa_Cod_To
                        CentroAziendale_To.sa_nome = Prefix & CentroAziendale_From.sa_nome
                        GiasContext.Centri_Aziendali.Add(CentroAziendale_To)
                        GiasContext.SaveChanges()

                        ' Copio utente x strutture
                        Dim UtenteCentro_From = (From u In GiasContext.UtentiXStrutture Where u.USER = PivaSuperUser AndAlso u.PIVA = Piva AndAlso u.SA_COD = Sa_Cod Select u).FirstOrDefault()
                        If UtenteCentro_From IsNot Nothing Then
                            Dim utente = CopyEntity(GiasContext, UtenteCentro_From, username, data)
                            utente.USER = PivaSuperUser
                            utente.PIVA = Piva_To
                            utente.SA_COD = Sa_Cod_To
                            GiasContext.UtentiXStrutture.Add(utente)
                            GiasContext.SaveChanges()
                        End If

                        ' Copio gli indirizzi del centro aziendale
                        Dim CentroAziendaleIndirizzi_From = (From a In GiasContext.CentrixIndirizzi Where a.PIVA = Piva AndAlso a.sa_cod = Sa_Cod)
                        For Each CentroAziendaleIndirizzo_From In CentroAziendaleIndirizzi_From
                            Dim Indirizzo_From = (From a In GiasContext.Indirizzi Where a.cod_indirizzo = CentroAziendaleIndirizzo_From.cod_indirizzo).FirstOrDefault
                            Dim Cod_Indirizzo_To = agroDP.NuovoId_Tabella_EF(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                            Dim Indirizzo_To = CopyEntity(GiasContext, Indirizzo_From, username, data)
                            Indirizzo_To.cod_indirizzo = Cod_Indirizzo_To
                            GiasContext.Indirizzi.Add(Indirizzo_To)
                            GiasContext.SaveChanges()

                            Dim CentroAziendaleIndirizzo_To = CopyEntity(GiasContext, CentroAziendaleIndirizzo_From, username, data)
                            CentroAziendaleIndirizzo_To.PIVA = Piva_To
                            CentroAziendaleIndirizzo_To.sa_cod = Sa_Cod_To
                            CentroAziendaleIndirizzo_To.cod_indirizzo = Cod_Indirizzo_To
                            GiasContext.CentrixIndirizzi.Add(CentroAziendaleIndirizzo_To)
                            GiasContext.SaveChanges()
                        Next

                        ' Copio i codici del centro aziendale
                        Dim CentroAziendaleCodici_From = (From a In GiasContext.Centri_Aziendali_Codici Where a.PIVA = Piva AndAlso a.sa_cod = Sa_Cod)
                        For Each CentroAziendaleCodice_From In CentroAziendaleCodici_From
                            Dim CentroAziendaleCodice_To = CopyEntity(GiasContext, CentroAziendaleCodice_From, username, data)
                            CentroAziendaleCodice_To.PIVA = Piva_To
                            CentroAziendaleCodice_To.sa_cod = Sa_Cod_To
                            GiasContext.Centri_Aziendali_Codici.Add(CentroAziendaleCodice_To)
                            GiasContext.SaveChanges()
                        Next

                        ' Creo il record nella reg impianti codici con la chiave dell'impianto ribaltato
                        Dim codice_centro = Piva_To & "|" & Sa_Cod_To
                        ScriviCodiceCentro(Piva, Sa_Cod, enum_CodiciAnagrafe.Codice_Centro, codice_centro, username, GiasContext)

                    End If

                    ' algoritmo per codifica impianti / esercizi
                    Dim algoritmo = LeggiAlgoritmoCodifica(Piva_To, objParametri)

                    ' Verifico se esiste già l'impianto destinazione
                    If algoritmo = "1" OrElse LeggiImpiantoDestinazione(Piva, Sa_Cod, Appezza_To, Id_Reg_To, GiasContext) Then

                        If campo_cod <> 0 Then

                            ' Campo origine
                            Dim Campo_From = (From a In GiasContext.Campi Where a.Piva = Piva AndAlso a.Sa_Cod = Sa_Cod AndAlso a.Campo_Cod = campo_cod).FirstOrDefault

                            ' Copio il campo relativo all'impianto da ribaltare
                            Dim Campo_Cod_To = agroDP.NuovoId_Campi(Piva_To, Sa_Cod_To, BaseCode, TopCode, objParametri)
                            Dim Campo_To = CopyEntity(GiasContext, Campo_From, username, data)
                            Campo_To.Piva = Piva_To
                            Campo_To.Sa_Cod = Sa_Cod_To
                            Campo_To.Campo_Cod = Campo_Cod_To
                            Campo_To.Campo_Des = data.Year & " " & Campo_Cod_To
                            GiasContext.Campi.Add(Campo_To)
                            GiasContext.SaveChanges()

                            ' Copio utente x campi
                            Dim UtenteCampo_From = (From u In GiasContext.UtentiXCampi Where u.USER = PivaSuperUser AndAlso u.PIVA = Piva AndAlso u.SA_COD = Sa_Cod AndAlso u.Campo_Cod = campo_cod Select u).FirstOrDefault()
                            If UtenteCampo_From IsNot Nothing Then
                                Dim utente = CopyEntity(GiasContext, UtenteCampo_From, username, data)
                                utente.USER = PivaSuperUser
                                utente.PIVA = Piva_To
                                utente.SA_COD = Sa_Cod_To
                                utente.Campo_Cod = Campo_Cod_To
                                GiasContext.UtentiXCampi.Add(utente)
                                GiasContext.SaveChanges()
                            End If

                            ' Copio i codici campo
                            Dim CampoCodici_From = (From a In GiasContext.Campi_Codici Where a.PIVA = Piva AndAlso a.sa_cod = Sa_Cod AndAlso a.campo_cod = Campo_From.Campo_Cod)
                            For Each CampoCodice_From In CampoCodici_From
                                Dim CampoCodice_To = CopyEntity(GiasContext, CampoCodice_From, username, data)
                                CampoCodice_To.PIVA = Piva_To
                                CampoCodice_To.sa_cod = Sa_Cod_To
                                CampoCodice_To.campo_cod = Campo_Cod_To
                                GiasContext.Campi_Codici.Add(CampoCodice_To)
                                GiasContext.SaveChanges()
                            Next

                        End If

                        ' Appezzamento origine
                        Dim Appezzamento_From = (From a In GiasContext.Appezzamento Where a.PIVA = Piva AndAlso a.SA_COD = Sa_Cod AndAlso a.APPEZZA = appezza).FirstOrDefault

                        ' Ricavo l'id dell'appezzamento destinazione
                        Appezza_To = agroDP.NuovoId_Appezzamento(Piva_To, Sa_Cod_To, BaseCode, TopCode, objParametri)

                        ' Copio l'appezzamento relativo all'impianto da ribaltare
                        Dim Appezzamento_To = CopyEntity(GiasContext, Appezzamento_From, username, data)
                        Appezzamento_To.PIVA = Piva_To
                        Appezzamento_To.SA_COD = Sa_Cod_To
                        Appezzamento_To.APPEZZA = Appezza_To
                        GiasContext.Appezzamento.Add(Appezzamento_To)
                        GiasContext.SaveChanges()

                        ' Copio utente x appezzamenti
                        Dim UtenteAppezzamento_From = (From u In GiasContext.UtentiXAppezzamenti Where u.USER = PivaSuperUser AndAlso u.PIVA = Piva AndAlso u.SA_COD = Sa_Cod AndAlso u.Appezza = appezza Select u).FirstOrDefault()
                        If UtenteAppezzamento_From IsNot Nothing Then
                            Dim utente = CopyEntity(GiasContext, UtenteAppezzamento_From, username, data)
                            utente.USER = PivaSuperUser
                            utente.PIVA = Piva_To
                            utente.SA_COD = Sa_Cod_To
                            utente.Appezza = Appezza_To
                            GiasContext.UtentiXAppezzamenti.Add(utente)
                            GiasContext.SaveChanges()
                        End If

                        ' Copio i codici appezzamento
                        Dim AppezzamentoCodici_From = (From a In GiasContext.Appezzamento_Codici Where a.PIVA = Piva AndAlso a.sa_cod = Sa_Cod AndAlso a.appezza = Appezzamento_From.APPEZZA)
                        For Each AppezzamentoCodice_From In AppezzamentoCodici_From
                            Dim AppezzamentoCodice_To = CopyEntity(GiasContext, AppezzamentoCodice_From, username, data)
                            AppezzamentoCodice_To.PIVA = Piva_To
                            AppezzamentoCodice_To.sa_cod = Sa_Cod_To
                            AppezzamentoCodice_To.appezza = Appezza_To
                            GiasContext.Appezzamento_Codici.Add(AppezzamentoCodice_To)
                            GiasContext.SaveChanges()
                        Next

                        ' Impianto origine
                        Dim Impianto_From = (From a In GiasContext.Reg_Impianti Where a.PIVA = Piva AndAlso a.SA_COD = Sa_Cod AndAlso a.APPEZZA = appezza AndAlso a.ID_REG = id_reg).FirstOrDefault
                        Dim Impianto_Anno_To As Integer = If(Impianto_From.Validita_Inizio Is Nothing, Date.Now.Year, CDate(Impianto_From.Validita_Inizio).Year)

                        ' Ricavo l'id dell'impianto destinazione
                        Id_Reg_To = agroDP.NuovoId_Reg_Impianti(Piva_To, Sa_Cod_To, Appezza_To, BaseCode, TopCode, objParametri)

                        ' Copio l'impianto da ribaltare
                        Dim Impianto_To = CopyEntity(GiasContext, Impianto_From, username, data)
                        Impianto_To.PIVA = Piva_To
                        Impianto_To.SA_COD = Sa_Cod_To
                        Impianto_To.APPEZZA = Appezza_To
                        Impianto_To.ID_REG = Id_Reg_To
                        GiasContext.Reg_Impianti.Add(Impianto_To)
                        GiasContext.SaveChanges()

                        ' Copio i codici impianto
                        Dim ImpiantoCodici_From = (From a In GiasContext.Reg_Impianti_Codici Where a.PIVA = Piva AndAlso a.sa_cod = Sa_Cod AndAlso a.appezza = Appezzamento_From.APPEZZA AndAlso a.Id_Reg = Impianto_From.ID_REG AndAlso a.Progetto_Cod = 0)
                        For Each ImpiantoCodice_From In ImpiantoCodici_From
                            Dim ImpiantoCodice_To = CopyEntity(GiasContext, ImpiantoCodice_From, username, data)
                            ImpiantoCodice_To.PIVA = Piva_To
                            ImpiantoCodice_To.sa_cod = Sa_Cod_To
                            ImpiantoCodice_To.appezza = Appezza_To
                            ImpiantoCodice_To.Id_Reg = Id_Reg_To
                            ' Creazione automatica codice impianto
                            If ImpiantoCodice_From.id_cod = enum_CodiciAnagrafe.Codice_Impianto Then
                                Dim Codice_Progetto = LeggiCodiceProgressivo(Piva_To, algoritmo, enum_SequenzaProgressiviTipi.CodiciProgettoAgricoli, Impianto_Anno_To, objParametri, True)
                                If Codice_Progetto <> "" Then
                                    ImpiantoCodice_To.val_cod = Codice_Progetto
                                End If
                            End If
                            GiasContext.Reg_Impianti_Codici.Add(ImpiantoCodice_To)
                            GiasContext.SaveChanges()
                        Next

                    End If

                    ' Esercizio di origine
                    Dim Progetto_From = (From a In GiasContext.Imprese_Progetti Where a.Piva = Piva AndAlso a.Progetto_Cod = progetto_cod).FirstOrDefault

                    ' Controllo sovrapposizione date
                    Dim Progetti_To = (From a In GiasContext.Imprese_Progetti Where a.Piva = Piva_To AndAlso a.Sa_Cod = Sa_Cod_To AndAlso a.Appezza = Appezza_To And a.Id_Reg = Id_Reg_To)
                    For Each progetto In Progetti_To
                        If (Progetto_From.Validita_Inizio >= progetto.Validita_Inizio AndAlso Progetto_From.Validita_Inizio <= progetto.Validita_Fine) OrElse
                            (Progetto_From.Validita_Fine >= progetto.Validita_Inizio AndAlso Progetto_From.Validita_Fine <= progetto.Validita_Fine) Then
                            messaggioErrore = "Le date validità si sovrappongono con quelle degli esercizi già presenti per l'impianto ribaltato"
                            Throw New Exception(messaggioErrore)
                        End If
                    Next

                    ' Creazione automatica codice esercizio
                    Dim Progetto_Cod_To As Integer = agroDP.NuovoId_Tabella_EF(GiasContext, "IMPRESA_PROGETTO", 0, 200000000, objParametri)
                    Dim Progetto_Anno_To As Integer = If(Progetto_From.Validita_Inizio Is Nothing, Date.Now.Year, CDate(Progetto_From.Validita_Inizio).Year)
                    Dim Progetto_Nome_To = LeggiCodiceProgressivo(Piva_To, algoritmo, enum_SequenzaProgressiviTipi.CodiciOPAgriZoo, Progetto_Anno_To, objParametri, True)

                    ' Creo l'esercizio per l'impianto ribaltato
                    Dim Progetto_To = CopyEntity(GiasContext, Progetto_From, username, data)
                    Progetto_To.Piva = Piva_To
                    Progetto_To.Sa_Cod = Sa_Cod_To
                    Progetto_To.Appezza = Appezza_To
                    Progetto_To.Id_Reg = Id_Reg_To
                    Progetto_To.Progetto_Cod = Progetto_Cod_To
                    If algoritmo <> "" Then
                        Progetto_To.Progetto_Nome = Progetto_Nome_To
                    End If
                    GiasContext.Imprese_Progetti.Add(Progetto_To)
                    GiasContext.SaveChanges()

                    ' Creo il record nella reg impianti codici con la chiave dell'impianto ribaltato
                    Dim codice_impianto = Piva_To & "|" & Sa_Cod_To & "|" & Appezza_To & "|" & Id_Reg_To & "|" & Progetto_Cod_To
                    ScriviCodiceProgetto(Piva, Sa_Cod, appezza, id_reg, progetto_cod, enum_CodiciAnagrafe.Codice_Impianto_Ribaltato, codice_impianto, username, GiasContext)

                    scope.Complete()
                    scope.Dispose()

                Catch ex As Exception

                    messaggioErrore = ex.Message
                    Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

                    log.Append("[" & nomeRoutine & "] : " & messaggioErrore)
                    scope.Dispose()

                Finally

                    GiasContext.Dispose()

                End Try

            End Using

        End If

        Return messaggioErrore

    End Function

    Public Function Replica_Esercizio(ByVal Piva As String, ByVal progetto_cod As Integer,
                                      ByVal annualita As Integer, ByVal data_chiusura As Date,
                                      ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional NoteLog As String = "") As String

        Dim nomeRoutine As String = "Replica_GIAS.Replica_Esercizio"
        Dim messaggioErrore As String = ""
        Dim log = New StringBuilder

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim PivaSuperUser = objParametri_Server.PivaSuperUser
        Dim username = objParametri_Server.UsernameOperazione
        Dim data = DateTime.Now

        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
        Dim logAnagrafe As Agronica_Log_Anagrafe

        Dim campoModificato As String = "Chiusura esercizio al " & data_chiusura.ToShortDateString()
        If annualita > 0 Then
            campoModificato += " + Apertura " & annualita.ToString() & " nuove annualità"
        End If

        Dim objEreditatore As New AgronicaCoreAnagrafeBIZ.Ereditatore
        Using scope As New TransactionScope()

            Try

                ' Esercizio di origine
                Dim ProgettoCodici_From = (From a In GiasContext.Reg_Impianti_Codici Where a.PIVA = Piva AndAlso a.Progetto_Cod = progetto_cod).ToList()
                Dim Progetto_From = (From a In GiasContext.Imprese_Progetti Where a.Piva = Piva AndAlso a.Progetto_Cod = progetto_cod).FirstOrDefault()

                Dim Impianto_From = (
                    From a In GiasContext.Reg_Impianti Where a.PIVA = Progetto_From.Piva AndAlso a.SA_COD = Progetto_From.Sa_Cod AndAlso
                    a.APPEZZA = Progetto_From.Appezza AndAlso a.ID_REG = Progetto_From.Id_Reg).FirstOrDefault()

                Dim Appezzamento_From = (
                    From a In GiasContext.Appezzamento Where a.PIVA = Progetto_From.Piva AndAlso a.SA_COD = Progetto_From.Sa_Cod AndAlso
                    a.APPEZZA = Progetto_From.Appezza).FirstOrDefault()

                Dim data_chiusura_impianto As Date = If(annualita > 0, data_chiusura.AddYears(annualita), data_chiusura)

                If annualita = -1 OrElse Impianto_From.Validita_Fine < data_chiusura_impianto Then

                    ' Chiusura impianto
                    Impianto_From.Validita_Fine = data_chiusura_impianto
                    Impianto_From.Data_Modifica = data
                    Impianto_From.Username_Modifica = username
                    GiasContext.Reg_Impianti.Attach(Impianto_From)
                    GiasContext.Entry(Impianto_From).State = EntityState.Modified

                    'Scrittura tabella Agronica_Log_Anagrafe
                    logAnagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Impianti,
                                                                CStr(Impianto_From.PIVA), CStr(Impianto_From.SA_COD),
                                                                CStr(Impianto_From.APPEZZA), CStr(Impianto_From.ID_REG),
                                                                Nothing, Nothing,
                                                                enum_TipoOperazioneDB.Modifica,
                                                                objParametri_Server,
                                                                enum_Id_Servizio.GiasOnline,
                                                                NoteLog & " (" & campoModificato & ")",
                                                                "")
                    GiasContext.Agronica_Log_Anagrafe.Add(logAnagrafe)
                    GiasContext.SaveChanges()

                    If Appezzamento_From.Validita_Fine < data_chiusura_impianto Then
                        objEreditatore.verifica_ValiditaFineAppezzamento(Appezzamento_From, data_chiusura_impianto, objParametri_Server, GiasContext)

                        ' Chiusura appezzamento
                        Appezzamento_From.Validita_Fine = data_chiusura_impianto
                        Appezzamento_From.Data_Modifica = data
                        Appezzamento_From.Username_Modifica = username
                        GiasContext.Appezzamento.Attach(Appezzamento_From)
                        GiasContext.Entry(Appezzamento_From).State = EntityState.Modified

                        'Scrittura tabella Agronica_Log_Anagrafe
                        logAnagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Appezza,
                                                                        CStr(Appezzamento_From.PIVA), CStr(Appezzamento_From.SA_COD),
                                                                        CStr(Appezzamento_From.APPEZZA), Nothing,
                                                                        Nothing, Nothing,
                                                                        enum_TipoOperazioneDB.Modifica,
                                                                        objParametri_Server,
                                                                        enum_Id_Servizio.GiasOnline,
                                                                        NoteLog & " (" & campoModificato & ")",
                                                                        "")
                        GiasContext.Agronica_Log_Anagrafe.Add(logAnagrafe)
                        GiasContext.SaveChanges()
                    End If

                End If

                ' Chiusura esercizio
                Progetto_From.Validita_Fine = data_chiusura
                Progetto_From.Data_Modifica = data
                Progetto_From.Username_Modifica = username
                GiasContext.Imprese_Progetti.Attach(Progetto_From)
                GiasContext.Entry(Progetto_From).State = EntityState.Modified

                'Scrittura tabella Agronica_Log_Anagrafe
                logAnagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                                CStr(Progetto_From.Piva), CStr(Progetto_From.Progetto_Cod),
                                                                CStr(Progetto_From.Sa_Cod), CStr(Progetto_From.Appezza),
                                                                CStr(Progetto_From.Id_Reg), Nothing,
                                                                enum_TipoOperazioneDB.Modifica,
                                                                objParametri_Server,
                                                                enum_Id_Servizio.GiasOnline,
                                                                NoteLog & " (" & campoModificato & ")",
                                                                "")
                GiasContext.Agronica_Log_Anagrafe.Add(logAnagrafe)
                GiasContext.SaveChanges()

                Dim dataInizio As Date
                Dim dataFine As Date = data_chiusura

                ' algoritmo per codifica impianti / esercizi
                Dim algoritmo = LeggiAlgoritmoCodifica(Piva, objParametri_Server)
                Dim listaCodici As New List(Of Integer) From {
                    enum_CodiciAnagrafe.Organismo_Referente,
                    enum_CodiciAnagrafe.Capitolato_Privato,
                    enum_CodiciAnagrafe.Magazzino_Conferimento,
                    enum_CodiciAnagrafe.Zespri_Codice_kPIN,
                    enum_CodiciAnagrafe.Zespri_Block_Name,
                    enum_CodiciAnagrafe.Zepri_Grower_Number,
                    enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati
                }

                For index As Integer = 1 To annualita

                    dataInizio = dataFine.AddDays(1)
                    dataFine = dataFine.AddYears(1)

                    ' Duplica esercizio
                    Dim Progetto_Cod_To As Integer = agroDP.NuovoId_Tabella_EF(GiasContext, "IMPRESA_PROGETTO", 0, 200000000, objParametri_Server)
                    Dim Progetto_Nome_To = LeggiCodiceProgressivo(Piva, algoritmo, enum_SequenzaProgressiviTipi.CodiciOPAgriZoo, dataInizio.Year, objParametri_Server)
                    Dim Progetto_To = CopyEntity(GiasContext, Progetto_From, username, data)
                    Progetto_To.Progetto_Cod = Progetto_Cod_To
                    If algoritmo <> "" Then
                        Progetto_To.Progetto_Nome = Progetto_Nome_To
                    End If
                    Progetto_To.Progetto_Des = Progetto_To.Progetto_Nome & " - Lotto " & dataInizio.Year
                    If Progetto_To.Data_Inizio_Prevista <> AGRODATAINIZIO Then
                        Dim dataSeminaPrevista As Date = Progetto_To.Data_Inizio_Prevista
                        Progetto_To.Data_Inizio_Prevista = dataSeminaPrevista.AddYears(1)
                    End If
                    If Progetto_To.Data_Fine_Prevista <> AGRODATAFINE Then
                        Dim dataRaccoltaPrevista As Date = Progetto_To.Data_Fine_Prevista
                        Progetto_To.Data_Fine_Prevista = dataRaccoltaPrevista.AddYears(1)
                    End If
                    If Progetto_To.Data_Fioritura_Prevista <> AGRODATAINIZIO Then
                        Dim dataFiorituraPrevista As Date = Progetto_To.Data_Fioritura_Prevista
                        Progetto_To.Data_Fioritura_Prevista = dataFiorituraPrevista.AddYears(1)
                    End If
                    Progetto_To.Validita_Inizio = dataInizio
                    Progetto_To.Validita_Fine = dataFine
                    Progetto_To.Disciplinare_Cod = 0
                    Progetto_To.Disciplinare_PubblicoPrivato = 0
                    Progetto_To.Regolamento_Concimazioni_Cod = 0
                    GiasContext.Imprese_Progetti.Add(Progetto_To)

                    'Scrittura tabella Agronica_Log_Anagrafe
                    logAnagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                                    CStr(Progetto_To.Piva), CStr(Progetto_To.Progetto_Cod),
                                                                    CStr(Progetto_To.Sa_Cod), CStr(Progetto_To.Appezza),
                                                                    CStr(Progetto_To.Id_Reg), Nothing,
                                                                    enum_TipoOperazioneDB.Scrittura,
                                                                    objParametri_Server,
                                                                    enum_Id_Servizio.GiasOnline,
                                                                    NoteLog & " (" & campoModificato & ")",
                                                                    "")
                    GiasContext.Agronica_Log_Anagrafe.Add(logAnagrafe)
                    GiasContext.SaveChanges()

                    ' Duplico i codici esercizio
                    For Each ProgettoCodice_From In ProgettoCodici_From
                        If listaCodici.Contains(ProgettoCodice_From.id_cod) Then

                            Dim ProgettoCodice_To = CopyEntity(GiasContext, ProgettoCodice_From, username, data)
                            ProgettoCodice_To.Progetto_Cod = Progetto_Cod_To
                            GiasContext.Reg_Impianti_Codici.Add(ProgettoCodice_To)
                            If ProgettoCodice_To.Validita_Inizio <> AGRODATAINIZIO Then
                                ProgettoCodice_To.Validita_Inizio = dataInizio
                            End If
                            If ProgettoCodice_To.Validita_Fine <> AGRODATAFINE Then
                                ProgettoCodice_To.Validita_Fine = dataFine
                            End If
                            GiasContext.SaveChanges()

                            ' elimino il codice nell'esercizio di origine
                            If ProgettoCodice_From.id_cod = enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati Then
                                GiasContext.Reg_Impianti_Codici.Attach(ProgettoCodice_From)
                                GiasContext.Reg_Impianti_Codici.Remove(ProgettoCodice_From)
                                GiasContext.SaveChanges()
                            End If

                        End If
                    Next
                Next

                GiasContext.SaveChanges()

                scope.Complete()
                scope.Dispose()

            Catch ex As GiasException
                Throw ex
            Catch ex As Exception

                messaggioErrore = ex.Message
                Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)

                log.Append("[" & nomeRoutine & "] : " & messaggioErrore)
                scope.Dispose()

            Finally

                GiasContext.Dispose()

            End Try

        End Using

        Return messaggioErrore

    End Function

    Public Sub AggiornaValiditaCampo(ByVal piva As String, ByVal sa_cod As Integer, ByVal campo_cod As Integer,
                                     ByVal validita_inizio As Date, ByVal validita_fine As Date,
                                     ByRef objParametri As AgronicaCoreParametri)

        Dim objAppezzamenti_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objAppezzamenti_W As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
        Dim objUtentixAppezzamenti As New AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W
        Dim objAppezzaxParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W
        Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write
        Dim objGrafica_AD As New AgronicaCoreGraficaDAL.Grafica_Write

        '-------------------------------------------------
        'Modifica delle finestre temporali dei figli
        Dim dtAppezza = objAppezzamenti_R.LeggiconCampo(piva,
                                               sa_cod,
                                               campo_cod,
                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                               "", "", objParametri)

        Dim objAgenda_R As New AgronicaCoreContabDAL.Mov_Destinazioni_R

        Dim Validita_Inizio_Orig = objParametri.FinestraTemporaleInizio
        Dim Validita_Fine_Orig = objParametri.FinestraTemporaleFine

        For Each RowAppezza In dtAppezza.Rows

            Dim validita_inizio_appezza As Date = RowAppezza("Validita_Inizio")
            Dim validita_fine_appezza As Date = RowAppezza("Validita_Fine")

            If validita_inizio > validita_inizio_appezza Then
                objParametri.FinestraTemporaleInizio = validita_inizio_appezza
                objParametri.FinestraTemporaleFine = validita_inizio
                Dim dt = objAgenda_R.Leggi_conImpianti(piva, sa_cod, 0, 0, 0, RowAppezza("Appezza"), 0, "", "", objParametri)
                If dt.Rows.Count > 0 Then
                    objParametri.FinestraTemporaleInizio = Validita_Inizio_Orig
                    objParametri.FinestraTemporaleFine = Validita_Fine_Orig
                    Throw New Exception("Sono presenti operazioni di agenda precedenti alla chiusura del campo per l'appezzamento " & CStr(RowAppezza("App_Nome")))
                End If
            End If

            If validita_fine < validita_fine_appezza Then
                objParametri.FinestraTemporaleInizio = validita_fine
                objParametri.FinestraTemporaleFine = validita_fine_appezza
                Dim dt = objAgenda_R.Leggi_conImpianti(piva, sa_cod, 0, 0, 0, RowAppezza("Appezza"), 0, "", "", objParametri)
                If dt.Rows.Count > 0 Then
                    objParametri.FinestraTemporaleInizio = Validita_Inizio_Orig
                    objParametri.FinestraTemporaleFine = Validita_Fine_Orig
                    Throw New Exception("Sono presenti operazioni di agenda successive alla chiusura del campo per l'appezzamento " & CStr(RowAppezza("App_Nome")))
                End If
            End If

        Next

        objAppezzamenti_W.AggiornaValiditaInizio(piva,
                                               sa_cod,
                                               campo_cod,
                                               0,
                                               validita_inizio,
                                               "",
                                               objParametri)

        objAppezzamenti_W.AggiornaValiditaFine(piva,
                                             sa_cod,
                                             campo_cod,
                                             0,
                                             validita_fine,
                                             "",
                                             objParametri)

        objUtentixAppezzamenti.AggiornaValiditaInizio(piva,
                                                      sa_cod,
                                                      campo_cod,
                                                      0,
                                                      validita_inizio,
                                                      "",
                                                      objParametri)
        objUtentixAppezzamenti.AggiornaValiditaFine(piva,
                                                    sa_cod,
                                                    campo_cod,
                                                    0,
                                                    validita_fine,
                                                    "",
                                                    objParametri)

        objAppezzaxParticelle.AggiornaValiditaInizio(piva,
                                                     sa_cod,
                                                     campo_cod,
                                                     0,
                                                     "",
                                                     "",
                                                     "",
                                                     0, 0, "", validita_inizio,
                                                     "",
                                                     objParametri)

        objAppezzaxParticelle.AggiornaValiditaFine(piva,
                                                   sa_cod,
                                                   campo_cod,
                                                   0,
                                                   "", "", "", 0, 0, "", validita_fine,
                                                   "",
                                                   objParametri)

        objReg_Impianti.AggiornaValiditaInizio(piva,
                                               sa_cod,
                                               0,
                                               campo_cod,
                                               0,
                                               validita_inizio,
                                               objParametri)

        objReg_Impianti.AggiornaValiditaFine(piva,
                                             sa_cod,
                                             0,
                                             campo_cod,
                                             0,
                                             validita_fine,
                                             objParametri)

        'Aggiorno le date della grafica
        Dim GraphicKey As String = "M" & Right(New String("0", 8) & Hex(campo_cod), 8)

        objGrafica_AD.AggiornaValiditaInizio(piva,
                                             sa_cod,
                                             GraphicKey,
                                             validita_inizio,
                                             "",
                                             objParametri)
        objGrafica_AD.AggiornaValiditaFine(piva,
                                           sa_cod,
                                           GraphicKey,
                                           validita_fine,
                                           "",
                                           objParametri)
    End Sub

End Class
