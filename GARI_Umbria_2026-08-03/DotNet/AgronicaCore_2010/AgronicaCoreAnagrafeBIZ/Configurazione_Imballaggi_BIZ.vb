Imports System.Data
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeDAL


Public Class Configurazione_Imballaggi_BIZ_W

    '##############################################################################################

    Public Function AggiornaImballiProdottoBIZ(
            ByVal piva As String,
            ByVal Modulo_Generazione As Integer,
            ByVal Tipo_Config As Integer,
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "AnagrafeBIZ.Configurazione_Imballaggi.AggiornaImballiProdotto()"

        Try
            Dim campConf_R As New Configurazione_Imballaggi_R

            Dim ConfigImballaggioProdotto As New Configurazione_Imballaggi
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray
                ConfigImballaggioProdotto = New Configurazione_Imballaggi
                ConfigImballaggioProdotto.Piva_SuperUser = Piva_SuperUser
                ConfigImballaggioProdotto.Piva = piva
                ConfigImballaggioProdotto.Tabella_Cod = obj("Tabella_Cod")
                ConfigImballaggioProdotto.Tipo_Config = Tipo_Config
                ConfigImballaggioProdotto.Modulo_Generazione = Modulo_Generazione
                ConfigImballaggioProdotto.Mat_Cod = obj("Mat_Cod")
                ConfigImballaggioProdotto.Tabella_Par_Cod = 0
                ConfigImballaggioProdotto.Veg_Cod = obj("Veg_Cod")
                ConfigImballaggioProdotto.Cul_Cod = obj("Cul_Cod")
                ConfigImballaggioProdotto.Valore = obj("Valore").ToString
                ConfigImballaggioProdotto.Validita_Inizio = AGRODATAINIZIO
                ConfigImballaggioProdotto.Validita_Fine = AGRODATAFINE

                ConfigImballaggioProdotto.Data_Creazione = Date.Now
                ConfigImballaggioProdotto.Username_Creazione = objParametri.UsernameOperazione
                ConfigImballaggioProdotto.Data_Modifica = Date.Now
                ConfigImballaggioProdotto.Username_Modifica = objParametri.UsernameOperazione
                ConfigImballaggioProdotto.inviato = 0
                ' Controllo che non ci siano periodi sovrapposti già registrati
                'MessaggioErrore += CheckTestataGriglia(ConfigImballaggioProdotto, objParametri, False, True)
                EFArrayToInsert.Add(ConfigImballaggioProdotto)
            Next
            For Each obj As JObject In righeModificateArray
                ConfigImballaggioProdotto = campConf_R.Leggi_Elem_Configurazione_Imballaggi(piva, obj("Id_Config"), objParametri)
                If ConfigImballaggioProdotto Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare " & obj("Imballo_Des").ToString & " " & obj("Veg_Des").ToString & " " & obj("Cul_Des").ToString & " non trovata"
                Else
                    ConfigImballaggioProdotto.Piva_SuperUser = Piva_SuperUser
                    ConfigImballaggioProdotto.Piva = piva
                    ConfigImballaggioProdotto.Id_Config = obj("Id_Config")
                    ConfigImballaggioProdotto.Tabella_Cod = obj("Tabella_Cod")
                    ConfigImballaggioProdotto.Tipo_Config = Tipo_Config
                    ConfigImballaggioProdotto.Modulo_Generazione = Modulo_Generazione
                    ConfigImballaggioProdotto.Mat_Cod = obj("Mat_Cod")
                    ConfigImballaggioProdotto.Tabella_Par_Cod = 0
                    ConfigImballaggioProdotto.Veg_Cod = obj("Veg_Cod")
                    ConfigImballaggioProdotto.Cul_Cod = obj("Cul_Cod")
                    ConfigImballaggioProdotto.Valore = obj("Valore").ToString

                    ConfigImballaggioProdotto.Data_Modifica = Date.Now
                    ConfigImballaggioProdotto.Username_Modifica = objParametri.UsernameOperazione
                    ' Controllo che non si possano fare modifiche se ci sono movimenti collegati e che non ci siano periodi sovrapposti già registrati
                    'MessaggioErrore += CheckTestataGriglia(ConfigImballaggioProdotto, objParametri, True, True)
                    EFArrayToUpdate.Add(ConfigImballaggioProdotto)
                End If
            Next
            For Each obj As JObject In righeCancellateArray
                ConfigImballaggioProdotto = New Configurazione_Imballaggi
                ConfigImballaggioProdotto.Piva_SuperUser = Piva_SuperUser
                ConfigImballaggioProdotto.Piva = piva
                ConfigImballaggioProdotto.Id_Config = obj("Id_Config")
                ' Controllo che non si possano fare modifiche se ci sono movimenti collegati
                'MessaggioErrore += CheckTestataGriglia(ConfigImballaggioProdotto, objParametri, True, False)
                EFArrayToDelete.Add(ConfigImballaggioProdotto)
            Next

            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New Configurazione_Imballaggi_W

                MessaggioErrore = campConf_W.Aggiorna_Configurazione_Imballaggi(piva, EFArrayToInsert, EFArrayToUpdate,
                    EFArrayToDelete, objParametri)

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function

    Private Sub Scrivi_LOG(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, NomeRoutine As String, MessaggioErrore As String)
        Throw New NotImplementedException
    End Sub

End Class
