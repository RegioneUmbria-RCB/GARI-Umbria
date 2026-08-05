Imports System.Linq
Imports System.Runtime.Serialization
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class AttivitaXCentri_Aziendali

    Private _objParametri_Server As AgronicaCoreParametri

    Public Sub New(ByVal objParametri_Server As AgronicaCoreParametri)
        _objParametri_Server = objParametri_Server
    End Sub

    Public Function Salva(
                         ByVal Piva As String,
                         ByVal Inclusi As Boolean,
                         ByVal tutteLeAttivita As String,
                         ByVal righeInserite As String,
                         ByVal righeModificate As String,
                         ByVal righeEliminate As String,
                         ByRef Errore As String
                         ) As Boolean

        Errore = ""
        Dim nomeProcedura As String = "AttivitaXCentri_Aziendali.Salva"

        'inizializzo reader e writer con stesso contesto
        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametri_Server.StringaConnessione)
        Dim context = New Gias_DeveloperServer_Entities(efConnString)
        Dim writer As New AttivitaXCentri_Aziendali_W(context, _objParametri_Server)
        Dim reader As New AttivitaXCentri_Aziendali_R(context)

        Dim inseritiModificati As List(Of AttivitaXCentri_AziendaliModel) = New List(Of AttivitaXCentri_AziendaliModel)()
        Dim eliminati As List(Of AttivitaXCentri_AziendaliModel) = New List(Of AttivitaXCentri_AziendaliModel)()
        Dim tutte As List(Of AttivitaXCentri_AziendaliModel) = New List(Of AttivitaXCentri_AziendaliModel)()
        Dim almenoUnoSbagliato As Boolean = False

        If Not String.IsNullOrEmpty(righeInserite) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of AttivitaXCentri_AziendaliModel))(righeInserite))
        End If
        If Not String.IsNullOrEmpty(righeModificate) Then
            inseritiModificati.AddRange(JsonConvert.DeserializeObject(Of List(Of AttivitaXCentri_AziendaliModel))(righeModificate))
        End If
        If Not String.IsNullOrEmpty(righeEliminate) Then
            eliminati.AddRange(JsonConvert.DeserializeObject(Of List(Of AttivitaXCentri_AziendaliModel))(righeEliminate))
        End If

        If Not String.IsNullOrEmpty(tutteLeAttivita) Then
            tutte.AddRange(JsonConvert.DeserializeObject(Of List(Of AttivitaXCentri_AziendaliModel))(tutteLeAttivita))
        End If

        Try

            Dim inseriti = inseritiModificati.Where(Function(s) String.IsNullOrEmpty(s.Chiave)).ToList()
            Dim modificati = inseritiModificati.Where(Function(s) Not String.IsNullOrEmpty(s.Chiave)).ToList()

            Dim gurdaIn As Short = If(Inclusi, 0, 1)
            Dim listaInGestione = If(Inclusi, "'Incluse'", "'Escluse'")
            Dim listaOpposta = If(Inclusi, "'Escluse'", "'Incluse'")

            'Controllo se tutti compilati correttamente
            For Each e As AttivitaXCentri_AziendaliModel In inseritiModificati
                If e.ID_Attivita = 0 OrElse e.Sa_Cod = 0 Then
                    almenoUnoSbagliato = True
                    Errore = "ATTENZIONE. Uno o più elementi che si sta tentando di inserire / modificare non sono compilati correttamente."
                    Exit For
                End If
            Next

            If almenoUnoSbagliato Then
                Return False
            End If

            'Controlli su inseriti se esistono già nella lista degli opposti
            almenoUnoSbagliato = ControllaSeEsisteGia(Piva, gurdaIn, tutte, inseriti)
            If almenoUnoSbagliato Then
                Errore = String.Format("ATTENZIONE. Uno o più elementi {0} che si sta tentando di inserire sono già presenti nella lista {1}.", listaInGestione, listaOpposta)
                Return False
            End If

            'Controlli su modificati se esistono già sulla lista dei modificati
            almenoUnoSbagliato = ControllaSeEsisteGia(Piva, gurdaIn, tutte, modificati)
            If almenoUnoSbagliato Then
                Errore = String.Format("ATTENZIONE. Uno o più elementi {0} che si sta tentando di modificare sono già presenti nella lista {1}.", listaInGestione, listaOpposta)
                Return False
            End If

            gurdaIn = If(Inclusi, 1, 0)
            'Controlli su inseriti se esistono già nella stessa lista
            almenoUnoSbagliato = ControllaSeEsisteGia(Piva, gurdaIn, tutte, inseriti)
            If almenoUnoSbagliato Then
                Errore = String.Format("ATTENZIONE. Uno o più elementi {0} che si sta tentando di inserire sono già presenti nella stessa lista.", listaInGestione)
                Return False
            End If

            'Controlli su modificati se esistono già nella stessa lista

            For Each e As AttivitaXCentri_AziendaliModel In modificati
                almenoUnoSbagliato = tutte.Where(Function(t) t.Piva.Equals(e.Piva) AndAlso
                t.ID_Attivita.Equals(e.ID_Attivita) AndAlso e.Sa_Cod.Equals(e.Sa_Cod) AndAlso
                t.Inclusa.Equals(gurdaIn) AndAlso t.Chiave <> e.Chiave).Any()
                If almenoUnoSbagliato Then
                    Errore = String.Format("ATTENZIONE. Uno o più elementi {0} che si sta tentando di modificare sono già presenti nella stessa lista.", listaInGestione)
                    Return False
                End If
            Next

            ' Eliminazione
            For Each e As AttivitaXCentri_AziendaliModel In eliminati
                Dim daEliminare = reader.Leggi(e.Piva, e.ID_Attivita, e.Sa_Cod, _objParametri_Server).FirstOrDefault()
                If Not daEliminare Is Nothing Then
                    writer.Elimina(daEliminare)
                End If
            Next

            'Inserimento nuovi
            For Each e As AttivitaXCentri_AziendaliModel In inseriti
                Dim entita = CentroAziendaliXSalvataggio(Inclusi, e)
                entita.Piva = Piva
                writer.Inserisci(entita)
            Next

            ' Aggiornamento modificati
            For Each e As AttivitaXCentri_AziendaliModel In modificati
                Dim chiavi = e.Chiave.Split("_")
                Dim entita = reader.Leggi(chiavi(0), Convert.ToInt32(chiavi(2)), Convert.ToInt32(chiavi(1)), _objParametri_Server).FirstOrDefault()
                If Not entita Is Nothing Then
                    writer.Elimina(entita)
                    Dim nuovaEntita = CentroAziendaliXSalvataggio(Inclusi, e)
                    nuovaEntita.Piva = Piva
                    writer.Inserisci(nuovaEntita)
                End If

            Next

            writer.Committa()

            Return True

        Catch ex As Exception
            Throw New Exception("[" & nomeProcedura & "] : " & ex.Message)
        End Try

    End Function

    Private Function ControllaSeEsisteGia(ByVal Piva As String,
                         ByVal gurdaIn As Short,
                         ByVal tutteLeAttivita As List(Of AttivitaXCentri_AziendaliModel),
                         ByVal daControllare As List(Of AttivitaXCentri_AziendaliModel)) As Boolean

        Dim almenoUnoSbagliato = False

        For Each e As AgronicaCoreEntityFramework_POCO.AttivitaXCentri_Aziendali In daControllare
            e.Piva = Piva
            Dim esisteInInsiemeOpposto = tutteLeAttivita.Where(Function(t) t.Piva.Equals(e.Piva) AndAlso
                t.ID_Attivita.Equals(e.ID_Attivita) AndAlso t.Sa_Cod.Equals(e.Sa_Cod) AndAlso
                t.Inclusa.Equals(gurdaIn)).Any()
            If esisteInInsiemeOpposto Then
                almenoUnoSbagliato = True
                Exit For
            End If

        Next

        Return almenoUnoSbagliato

    End Function

    Private Function CentroAziendaliXSalvataggio(
            ByVal Inclusa As Boolean,
            ByVal entita As AttivitaXCentri_AziendaliModel) As AgronicaCoreEntityFramework_POCO.AttivitaXCentri_Aziendali


        Return New AgronicaCoreEntityFramework_POCO.AttivitaXCentri_Aziendali With
            {
                .ID_Attivita = entita.ID_Attivita,
                .Data_Creazione = If(entita.Data_Creazione.HasValue, entita.Data_Creazione, New Nullable(Of DateTime)),
                .Data_Modifica = If(entita.Data_Modifica.HasValue, entita.Data_Modifica, New Nullable(Of DateTime)),
                .datainvio = New Nullable(Of DateTime),
                .Inclusa = If(Inclusa, 1, 0),
                .inviato = If(entita.inviato.HasValue, entita.inviato, 0),
                .Piva = entita.Piva,
                .Piva_SuperUser = If(String.IsNullOrEmpty(entita.Piva_SuperUser), "", entita.Piva_SuperUser),
                .Sa_Cod = entita.Sa_Cod,
                .Username_Creazione = If(String.IsNullOrEmpty(entita.Username_Creazione), "", entita.Username_Creazione),
                .Username_Modifica = If(String.IsNullOrEmpty(entita.Username_Modifica), "", entita.Username_Modifica),
                .Validita_Fine = AGRODATAFINE,
                .Validita_Inizio = AGRODATAINIZIO
            }


    End Function

    Public Shared Function ToModel(ByVal table As DataTable) As List(Of AttivitaModel)

        Dim models As List(Of AttivitaModel) = New List(Of AttivitaModel)()

        If table.Rows.Count > 0 Then

            For Each row As DataRow In table.AsEnumerable()
                models.Add(New AttivitaModel With
                           {
                                .ID_Attivita = If(row.IsNull("ID_Attivita"), 0, CInt(row("ID_Attivita"))),
                                .Descrizione = If(row.IsNull("Descrizione"), "", row("Descrizione")),
                                .attivita_poliannuale = If(row.IsNull("attivita_poliannuale"), False, CBool(row("attivita_poliannuale"))),
                                .Sa_Cod = If(row.IsNull("Sa_Cod"), 0, CInt(row("Sa_Cod")))
                           })
            Next

        End If

        Return models

    End Function

End Class

<DataContract(IsReference:=True)>
Public Class AttivitaXCentri_AziendaliModel : Inherits AgronicaCoreEntityFramework_POCO.AttivitaXCentri_Aziendali

    Private _chiave As String
    <DataMember()>
    Public Property Chiave() As String
        Get
            Return _chiave
        End Get
        Set(value As String)
            _chiave = value
        End Set
    End Property

    Private _utilizzo_GiasAPP As Boolean
    <DataMember()>
    Public Property Utilizzo_GiasAPP() As Boolean
        Get
            Return _utilizzo_GiasAPP
        End Get
        Set(value As Boolean)
            _utilizzo_GiasAPP = value
        End Set
    End Property

    Public Sub New()
        MyBase.New()
    End Sub

End Class

Public Class AttivitaModel

    Public ID_Attivita As Integer
    Public Descrizione As String
    Public Sa_Cod As Integer
    Public attivita_poliannuale As Boolean

End Class