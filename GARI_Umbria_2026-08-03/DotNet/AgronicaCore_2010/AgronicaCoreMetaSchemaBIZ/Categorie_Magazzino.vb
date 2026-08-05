Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Categorie_Magazzino

    Public Function Leggi_CategorieMagazzino(filtro As String,
                                             ByVal objParametri_Server As AgronicaCoreParametri,
                                             ByVal objParametri_Utenti As AgronicaCoreParametri) As IEnumerable(Of Object)
        Dim objCatMag As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
        Dim TabellaLettura = "CategorieMagazzino"
        Dim xOrderBy = " NomeComune "

        'If filtro = "" Then
        '    xFiltroAggiuntivo = " Elem_Cod > 0 "
        'End If

        Dim DT_Categorie As DataTable = objCatMag.LeggiTabella_da_CategorieMagazzino(
            TabellaLettura, "", "NomeComune", "", 0, filtro, xOrderBy, objParametri_Server)
       
        Dim listItems = (From row In DT_Categorie.Rows
                         Select (New With {
                             .Elem_Cod = if(isdbnull(row("Elem_Cod")), 0, row("Elem_Cod")),
                             .Tabella = if(isdbnull(row("Tabella")), "", row("Tabella")),
                             .NomeComune = if(isdbnull(row("NomeComune")), "", row("NomeComune")),
                             .Tabella_Cod = if(isdbnull(row("Tabella_Cod")), 0, row("Tabella_Cod")),
                             .Tabella_Des = if(isdbnull(row("Tabella_Des")), 0, row("Tabella_Des")),
                             .Tabella_Tio = if(isdbnull(row("Tabella_Tipo")), 0, row("Tabella_Tipo")),
                             .inviato = if(isdbnull(row("inviato")), 0, row("inviato")),
                             .dataInvio = if(isdbnull(row("datainvio")), 0, row("datainvio")),
                             .Data_Creazione = if(isdbnull(row("Data_Creazione")), 0, row("Data_Creazione")),
                             .Data_Modifica = if(isdbnull(row("Data_Modifica")), 0, row("Data_Modifica")),
                             .Username_Creazione = if(isdbnull(row("Username_Creazione")), 0, row("Username_Creazione")),
                             .Username_Modifica = if(isdbnull(row("Username_Modifica")), 0, row("Username_Modifica")),
                             .Validita_Inizio = if(isdbnull(row("Validita_Inizio")), 0, row("Validita_Inizio")),
                             .Validita_Fine = if(isdbnull(row("Validita_Fine")), 0, row("Validita_Fine")),
                             .Tipo_Prodzione = if(isdbnull(row("Tipo_Produzione")), 0, row("Tipo_Produzione")),
                             .ammortizzabile = if(isdbnull(row("ammortizzabile")), 0, row("ammortizzabile"))
                         })).ToList()

        Return listItems
    End Function

    ''' <summary>
    ''' Legge i valori associati alle impostazioni 180 e 181.
    ''' Tali impostazini sono configurabili a livello di SUper User e a livello
    ''' di azienda.
    ''' </summary>
    Public Function LeggiImpostazioniCategorieMagazzinoImpreseSU(
        setting As Integer, piva As String, saCod As Integer,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri
    ) As IEnumerable(Of Object)
        Dim businessSettings As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim userSettings As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim settingValue = ""

        If Not String.IsNullOrEmpty(piva) Then
            settingValue = businessSettings.LeggiScalareMulticentroAziendaSuperUser(
                piva, {saCod}.ToList,
                setting, String.Empty,
                objParametri_Utenti, objParametri_Server
            )
        Else
            Dim suSettings = userSettings.LeggiScalare(
                objParametri_Utenti.SuperUserUsername, {setting},
                objParametri_Utenti
            ).FirstOrDefault
            If suSettings IsNot Nothing Then
                settingValue = suSettings.Valore
            End If
        End If

        Return ParseToValues(settingValue)
    End Function

    Private Function ParseToValues(storageCatSettingValue As String) As IEnumerable(Of Object)
        If String.IsNullOrWhiteSpace(storageCatSettingValue) Then
            Return New List(Of Object)
        End If
        Return storageCatSettingValue.Split("|").
            Where(Function(str) Not String.IsNullOrWhiteSpace(str) AndAlso str.Contains("_")).
            Select(Function(str) New With {
                .Elem_Cod = str.Split("_").First,
                .radioValue = str.Split("_").Last
            })
    End Function

    Public Function Leggi_CategorieMagazzino_Impostazioni_CentroAziendaSuperuser(
        Impostazione_Cod As Integer,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri
    )
        Dim objCatMag As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim user As String = objParametri_Utenti.UtenteUsername

        Dim tipoUtente As Integer = 1 '1 = utente, 2 = superuser
        Dim DT_Impostazioni As DataTable = objImpostazioni.Leggi2(
            tipoUtente, user, Impostazione_Cod,
            xFiltroAggiuntivo:="", xOrderBy:="", objParametri_Utenti
        )
        Dim result As New With {
            .Impostazione_Cod = Impostazione_Cod,
            .username = user,
            .values = New List(Of Object)
        }
        Dim listItem = DT_Impostazioni.Select.FirstOrDefault
        If listItem IsNot Nothing Then
            Dim strValue As String = listItem(2)
            result.values = ParseToValues(strValue)
        End If
        Return result
    End Function

    Public Function Leggi_Categoria_Magazzino_Da_IdAgenda(IdAgenda As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of Integer)
        Dim objCatMag As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R

        Dim elemCod = objCatMag.Leggi_ElemCod_Da_IdAgenda(IdAgenda, objParametri)

        Return elemCod
    End Function

    Private Function CreaDT_CategorieGiacenze()
        Dim Dt As New DataTable

        Dim elemColumn = New DataColumn("Elem_Cod", GetType(Integer))

        Dt.Columns.Add(elemColumn)
        Dt.Columns.Add(New DataColumn("NomeComune", GetType(String)))
        Dt.Columns.Add(New DataColumn("Val", GetType(Integer)))

        Dim DtKeys(0) As DataColumn
        DtKeys(0) = elemColumn

        Dt.PrimaryKey = DtKeys

        Return Dt
    End Function

End Class
