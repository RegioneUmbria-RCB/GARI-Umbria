Imports System.Linq
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class Rubrica_R
    Inherits AgronicaCoreDataProvider.LogProvider

    ''' <summary>
    ''' LETTURA OGGETTO RubricaVoci IN BASE ALLA CHIAMATA DI PROVENIENZA
    ''' </summary>
    ''' <param name="Elemento_Anagrafico"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Private Function Leggi_RubricaVoci(Piva As String,
                                       Sa_Cod As Integer,
                                       Cod_Contatto As String,
                                       appezza As Integer,
                                       Elemento_Anagrafico As enum_EntitaAlberoImprese,
                                       ElementiDaEscludere As List(Of enum_CodiciAnagrafe),
                                       ByRef objParametri_Server As AgronicaCoreParametri
                                       ) As List(Of AgronicaCoreModelsSTD.anagrafiche.RubricaVoci)

        Dim cod_list As New List(Of AgronicaCoreModelsSTD.anagrafiche.RubricaVoci)
        Dim dt As DataTable = Nothing
        Select Case Elemento_Anagrafico
            Case enum_EntitaAlberoImprese.Centro
                Dim CentrixRubrica_Read As New AgronicaCoreAnagrafeDAL.CentrixRubrica_Read
                dt = CentrixRubrica_Read.Leggi(Piva, Sa_Cod, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Contatto
                Dim ContattixRubrica_R As New AgronicaCoreAnagrafeDAL.ContattixRubrica_R
                'dt = ContattixRubrica_R.Leggi(Piva, Cod_Contatto, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Appezzamento
                Dim AppezzamentixRubrica_Read As New AgronicaCoreAnagrafeDAL.AppezzamentixRubrica_Read
                'dt = AppezzamentixRubrica_Read.(Piva, Sa_Cod, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case Else
                Throw New Exception("Lettura Indirizzo Associato su elemento anagrafico " & Elemento_Anagrafico & " non gestito. ")
        End Select

        If dt IsNot Nothing Then
            If dt.Rows.Count = 0 Then
                Return Nothing
            End If
            For Each row In dt.Rows
                Dim rubricaVoci As New AgronicaCoreModelsSTD.anagrafiche.RubricaVoci
                rubricaVoci.valore = row("numero")
                rubricaVoci.rubrica = New AgronicaCoreModelsSTD.anagrafiche.Rubrica(row("cod_rubrica")) With {.tipologia = row("descr")}
                cod_list.Add(rubricaVoci)
            Next
        End If

        Return cod_list
    End Function

    Public Function Leggi_Rubrica_Centro(Piva As String,
                                         Sa_Cod As Integer,
                                         ByRef objParametri_Server As AgronicaCoreParametri
                                         ) As List(Of AgronicaCoreModelsSTD.anagrafiche.RubricaVoci)
        Return Leggi_RubricaVoci(Piva, Sa_Cod, "", 0, enum_EntitaAlberoImprese.Centro, New List(Of enum_CodiciAnagrafe), objParametri_Server)
    End Function

    Public Function Leggi_Rubrica_Contatto(Piva As String,
                                           Cod_Contatto As String,
                                           ByRef objParametri_Server As AgronicaCoreParametri
                                           ) As List(Of AgronicaCoreModelsSTD.anagrafiche.RubricaVoci)
        Return Leggi_RubricaVoci(Piva, 0, Cod_Contatto, 0, enum_EntitaAlberoImprese.Contatto, New List(Of enum_CodiciAnagrafe), objParametri_Server)
    End Function

    Public Function Leggi_Rubrica_Appezzamento(Piva As String,
                                               appezza As Integer,
                                               ByRef objParametri_Server As AgronicaCoreParametri
                                               ) As List(Of AgronicaCoreModelsSTD.anagrafiche.RubricaVoci)
        Return Leggi_RubricaVoci(Piva, 0, "", appezza, enum_EntitaAlberoImprese.Appezzamento, New List(Of enum_CodiciAnagrafe), objParametri_Server)
    End Function
End Class

Public Class Rubrica_W
    Inherits AgronicaCoreDataProvider.LogProvider

    Private Function Scrivi_Rubrica_Voci(RubricaVoci As AgronicaCoreModelsSTD.anagrafiche.RubricaVoci,
                                         ByRef GiasContext As Gias_DeveloperServer_Entities,
                                         ByRef Impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                         ByRef Centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                         ByRef Contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                         ByRef Appezzamento As AgronicaCoreEntityFramework_POCO.Appezzamento,
                                         Elemento_Anagrafico As enum_EntitaAlberoImprese,
                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim Cod_Rubrica As Integer
        Dim RubricaEF As AgronicaCoreEntityFramework_POCO.Rubrica

        If RubricaVoci.rubrica.codice = 0 Then

            Select Case Elemento_Anagrafico
                Case enum_EntitaAlberoImprese.Centro
                    RubricaEF = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateRubricaCentro(GiasContext, objParametri_Server, Centro, RubricaVoci.valore, RubricaVoci.rubrica.tipologia, objParametri_Utenti.UsernameOperazione)
                Case enum_EntitaAlberoImprese.Contatto
                    RubricaEF = AgronicaCoreAnagrafeDAL.EFContatti.CreateRubricaContatto(GiasContext, objParametri_Server, Contatto.Piva, Contatto.Sa_Cod, Contatto, RubricaVoci.valore, RubricaVoci.rubrica.tipologia, objParametri_Utenti.UsernameOperazione)
                Case enum_EntitaAlberoImprese.Appezzamento
                    RubricaEF = AgronicaCoreAnagrafeDAL.EFAppezzamento.CreateRubricaAppezzamento(GiasContext, objParametri_Server, Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento, RubricaVoci.valore, RubricaVoci.rubrica.tipologia, objParametri_Utenti.UsernameOperazione)
                Case Else
                    Throw New Exception("Scrittura Rubrica Voci su elemento anagrafico " & Elemento_Anagrafico & " non riuscita. ")
            End Select

            Cod_Rubrica = RubricaEF.cod_rubrica

        Else

            Cod_Rubrica = RubricaVoci.rubrica.codice
            RubricaEF = (From rub In GiasContext.Rubrica
                        Where rub.cod_rubrica = Cod_Rubrica).FirstOrDefault()

            '########### elemxRubrica ###########
            Select Case Elemento_Anagrafico
                Case enum_EntitaAlberoImprese.Centro

                    Dim CentrixIndirizziEF As AgronicaCoreEntityFramework_POCO.CentrixRubrica
                    Dim PIVA = Centro.PIVA
                    Dim SA_COD = Centro.sa_cod

                    CentrixIndirizziEF = (From cenxi In GiasContext.CentrixRubrica
                                            Where cenxi.PIVA = PIVA AndAlso
                                                  cenxi.sa_cod = SA_COD AndAlso
                                                  cenxi.cod_rubrica = Cod_Rubrica)

                    'If CentrixIndirizziEF.Tipo_Indirizzo = indirizzoAssociato.tipo_Indirizzo Then
                    '    'se il tipo_indirizzo è uguale, non faccio nulla
                    'Else
                    '    ' se il tipo indirizzo è diverso, cancello il record e lo riscrivo
                    '    GiasContext.CentrixIndirizzi.Attach(CentrixIndirizziEF)
                    '    GiasContext.CentrixIndirizzi.Remove(CentrixIndirizziEF)

                    '    CentrixIndirizziEF.PIVA = PIVA
                    '    CentrixIndirizziEF.sa_cod = SA_COD

                    '    CentrixIndirizziEF.cod_indirizzo = Cod_Indirizzo
                    '    CentrixIndirizziEF.Tipo_Indirizzo = indirizzoAssociato.tipo_Indirizzo

                    '    CentrixIndirizziEF.inviato = 0
                    '    CentrixIndirizziEF.datainvio = DateTime.Now

                    '    CentrixIndirizziEF.Data_Creazione = DateTime.Now
                    '    CentrixIndirizziEF.Data_Modifica = DateTime.Now

                    '    CentrixIndirizziEF.Username_Creazione = objParametri_Utenti.UsernameOperazione
                    '    CentrixIndirizziEF.Username_Modifica = objParametri_Utenti.UsernameOperazione

                    '    CentrixIndirizziEF.Validita_Inizio = AGRODATAINIZIO
                    '    CentrixIndirizziEF.Validita_Fine = AGRODATAFINE

                    '    CentrixIndirizziEF.Validazione = 0
                    '    CentrixIndirizziEF.Data_Validazione = DateTime.Now
                    '    CentrixIndirizziEF.UserName_Validazione = ""

                    'End If

                Case enum_EntitaAlberoImprese.Contatto

                    Dim ContattiXIndirizziEF As AgronicaCoreEntityFramework_POCO.ContattiXRubrica
                    Dim PIVA = Contatto.Piva
                    Dim Cod_Contatto = Contatto.Cod_Contatto
                    Dim SA_COD = Contatto.Sa_Cod

                    ContattiXIndirizziEF = (From cxi In GiasContext.ContattiXRubrica
                                            Where cxi.Piva = PIVA AndAlso
                                                  cxi.Cod_Contatto = Cod_Contatto AndAlso
                                                  cxi.Cod_Rubrica = Cod_Rubrica)

                    'If ContattiXIndirizziEF.Tipo_Indirizzo = indirizzoAssociato.tipo_Indirizzo Then
                    '    'se il tipo_indirizzo è uguale, non faccio nulla
                    'Else
                    '    ' se il tipo indirizzo è diverso, cancello il record e lo riscrivo
                    '    GiasContext.ContattiXIndirizzi.Attach(ContattiXIndirizziEF)
                    '    GiasContext.ContattiXIndirizzi.Remove(ContattiXIndirizziEF)

                    '    ContattiXIndirizziEF.Piva = PIVA
                    '    ContattiXIndirizziEF.Sa_Cod = SA_COD
                    '    ContattiXIndirizziEF.Cod_Contatto = Cod_Contatto

                    '    ContattiXIndirizziEF.Cod_Indirizzo = Cod_Indirizzo
                    '    ContattiXIndirizziEF.Tipo_Indirizzo = indirizzoAssociato.tipo_Indirizzo

                    '    ContattiXIndirizziEF.Inviato = 0
                    '    ContattiXIndirizziEF.DataInvio = DateTime.Now

                    '    ContattiXIndirizziEF.Data_Creazione = DateTime.Now
                    '    ContattiXIndirizziEF.Data_Modifica = DateTime.Now

                    '    ContattiXIndirizziEF.Username_Creazione = objParametri_Utenti.UsernameOperazione
                    '    ContattiXIndirizziEF.Username_Modifica = objParametri_Utenti.UsernameOperazione

                    '    ContattiXIndirizziEF.Validita_Inizio = AGRODATAINIZIO
                    '    ContattiXIndirizziEF.Validita_Fine = AGRODATAFINE

                    'End If

                Case enum_EntitaAlberoImprese.Appezzamento

                    Dim AppezzamentixIndirizziEF As AgronicaCoreEntityFramework_POCO.AppezzamentixRubrica
                    Dim PIVA = Appezzamento.PIVA
                    Dim sa_cod = Appezzamento.SA_COD
                    Dim appezza = Appezzamento.APPEZZA

                    AppezzamentixIndirizziEF = (From axi In GiasContext.AppezzamentixRubrica
                                                Where axi.PIVA = PIVA AndAlso
                                                      axi.sa_cod = sa_cod AndAlso
                                                      axi.sa_cod = appezza AndAlso
                                                      axi.cod_rubrica = Cod_Rubrica)

                    'If AppezzamentixIndirizziEF.Tipo_Indirizzo = indirizzoAssociato.tipo_Indirizzo Then
                    '    'se il tipo_indirizzo è uguale, non faccio nulla
                    'Else
                    '    ' se il tipo indirizzo è diverso, cancello il record e lo riscrivo
                    '    GiasContext.AppezzamentixIndirizzi.Attach(AppezzamentixIndirizziEF)
                    '    GiasContext.AppezzamentixIndirizzi.Remove(AppezzamentixIndirizziEF)

                    '    AppezzamentixIndirizziEF.PIVA = PIVA
                    '    AppezzamentixIndirizziEF.sa_cod = sa_cod
                    '    AppezzamentixIndirizziEF.appezza = appezza

                    '    AppezzamentixIndirizziEF.cod_indirizzo = Cod_Indirizzo
                    '    AppezzamentixIndirizziEF.Tipo_Indirizzo = indirizzoAssociato.tipo_Indirizzo

                    '    AppezzamentixIndirizziEF.inviato = 0
                    '    AppezzamentixIndirizziEF.datainvio = DateTime.Now

                    '    AppezzamentixIndirizziEF.Data_Creazione = DateTime.Now
                    '    AppezzamentixIndirizziEF.Data_Modifica = DateTime.Now

                    '    AppezzamentixIndirizziEF.Username_Creazione = objParametri_Utenti.UsernameOperazione
                    '    AppezzamentixIndirizziEF.Username_Modifica = objParametri_Utenti.UsernameOperazione

                    '    AppezzamentixIndirizziEF.Validita_Inizio = AGRODATAINIZIO
                    '    AppezzamentixIndirizziEF.Validita_Fine = AGRODATAFINE

                    '    AppezzamentixIndirizziEF.Validazione = 0
                    '    AppezzamentixIndirizziEF.Data_Validazione = DateTime.Now
                    '    AppezzamentixIndirizziEF.UserName_Validazione = ""

                    'End If

                Case Else
                    Throw New Exception("Modifica Rubrica Voci su elemento anagrafico " & Elemento_Anagrafico & " non riuscita. ")
            End Select

        End If

        RubricaEF.numero = RubricaVoci.valore

        GiasContext.SaveChanges()

        Return Cod_Rubrica
    End Function

    'Public Function Scrivi_Indirizzi_Associati_Impresa(indirizzoAssociato As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato,
    '                                                   ByRef GiasContext As Gias_DeveloperServer_Entities,
    '                                                   ByRef Impresa As AgronicaCoreEntityFramework_POCO.Imprese,
    '                                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
    '                                                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

    '    Return Scrivi_Rubrica_Voci(indirizzoAssociato, GiasContext, Impresa, Nothing, Nothing, Nothing, enum_EntitaAlberoImprese.Impresa, objParametri_Server, objParametri_Utenti)

    'End Function

    'Public Function Scrivi_Indirizzi_Associati_Centro(indirizzoAssociato As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato,
    '                                                   ByRef GiasContext As Gias_DeveloperServer_Entities,
    '                                                   ByRef Centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
    '                                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
    '                                                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

    '    Return Scrivi_Rubrica_Voci(indirizzoAssociato, GiasContext, Nothing, Centro, Nothing, Nothing, enum_EntitaAlberoImprese.Centro, objParametri_Server, objParametri_Utenti)

    'End Function
    'Public Function Scrivi_Indirizzi_Associati_Contatto(indirizzoAssociato As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato,
    '                                                   ByRef GiasContext As Gias_DeveloperServer_Entities,
    '                                                   ByRef Contatto As AgronicaCoreEntityFramework_POCO.Contatti,
    '                                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
    '                                                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

    '    Return Scrivi_Rubrica_Voci(indirizzoAssociato, GiasContext, Nothing, Nothing, Contatto, Nothing, enum_EntitaAlberoImprese.Contatto, objParametri_Server, objParametri_Utenti)

    'End Function
    'Public Function Scrivi_Indirizzi_Associati_Appezzamento(indirizzoAssociato As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato,
    '                                                   ByRef GiasContext As Gias_DeveloperServer_Entities,
    '                                                   ByRef Appezzamento As AgronicaCoreEntityFramework_POCO.Appezzamento,
    '                                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
    '                                                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

    '    Return Scrivi_Rubrica_Voci(indirizzoAssociato, GiasContext, Nothing, Nothing, Nothing, Appezzamento, enum_EntitaAlberoImprese.Appezzamento, objParametri_Server, objParametri_Utenti)

    'End Function


End Class
