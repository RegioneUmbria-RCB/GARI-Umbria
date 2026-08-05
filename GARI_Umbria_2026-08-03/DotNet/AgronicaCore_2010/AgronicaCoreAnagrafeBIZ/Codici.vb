Imports System.Data.Entity
Imports System.Linq
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO


Public Class Codici_R
    ''' <summary>
    ''' LETTURA OGGETTO CodiceAnagrafe 
    ''' </summary>
    ''' <returns></returns>
    Public Function Leggi_CodiceAnagrafe(codice As Integer,
                                         ByRef objParametri_Server As AgronicaCoreParametri
                                         ) As AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe

        Dim cod As New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(codice)
        Dim codice_R As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

        Dim DT = codice_R.Leggi(codice, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        If (DT.Rows.Count = 0) Then
            Throw New Exception("Codice con ID " & codice & " non trovato nella tabella Codici Anagrafe")
        End If


    End Function

    ''' <summary>
    ''' LETTURA OGGETTO CodiciAnagrafeValori IN BASE ALLA CHIAMATA DI PROVENIENZA
    ''' </summary>
    ''' <param name="Elemento_Anagrafico"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Private Function Leggi_CodiciAnagrafeValori(Piva As String,
                                                Sa_Cod As Integer,
                                                Appezza As Integer,
                                                Campo_Cod As Integer,
                                                ID_Reg As Integer,
                                                Cod_Contatto As String,
                                                Elemento_Anagrafico As enum_EntitaAlberoImprese,
                                                ElementiDaEscludere As List(Of enum_CodiciAnagrafe),
                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                escludiCodiciCliente As Boolean) As List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)

        Dim cod_list As New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)
        Dim dt As DataTable
        Select Case Elemento_Anagrafico
            Case enum_EntitaAlberoImprese.Impresa
                Dim imprese_Codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                dt = imprese_Codici.Leggi(Piva, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Centro
                Dim centri_Codici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
                dt = centri_Codici.Leggi(Piva, Sa_Cod, 0, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Contatto
                Dim contatti_Codici As New AgronicaCoreAnagrafeDAL.Contatti_Codici_R
                dt = contatti_Codici.Leggi(Piva, Cod_Contatto, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Appezzamento
                Dim appezzamenti_Codici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
                dt = appezzamenti_Codici.Leggi(Piva, Sa_Cod, Appezza, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Campo
                Dim campi_Codici As New AgronicaCoreAnagrafeDAL.Campi_Codici_Read
                dt = campi_Codici.Leggi(Piva, Sa_Cod, Campo_Cod, 0, "", AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Impianto
                Dim reg_impianti_Codici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
                dt = reg_impianti_Codici.Leggi(Piva, Sa_Cod, Appezza, ID_Reg, "", 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case Else
                Throw New Exception("Lettura Indirizzo Associato su elemento anagrafico " & Elemento_Anagrafico & " non gestito ")
        End Select

        If ElementiDaEscludere Is Nothing Then
            ElementiDaEscludere = New List(Of enum_CodiciAnagrafe)
        End If

        'Dim resultsNumber

        'Dim resultsQuery = (From myRow In dt.AsEnumerable
        '                    Where Not ElementiDaEscludere.Contains(myRow.Field(Of Integer)("id_cod")))

        'If escludiCodiciCliente Then
        '    resultsQuery.Where(Function(el) el("id_cod") < 2000 AndAlso el("id_cod") >= 3000)
        'End If

        'resultsNumber = resultsQuery.Count()

        'Dim results As New DataTable
        'Dim subResults As New DataTable
        'If resultsNumber > 0 Then
        '    subResults = (From myRow In dt.AsEnumerable
        '                  Where Not ElementiDaEscludere.Contains(myRow.Field(Of Integer)("id_cod"))).CopyToDataTable
        'End If

        'If escludiCodiciCliente Then
        '    'results = (From myRow In subResults.AsEnumerable
        '    '           Where myRow.Field(Of Integer)("id_cod") < 2000 AndAlso myRow.Field(Of Integer)("id_cod") >= 3000).CopyToDataTable
        'Else
        '    results = subResults
        'End If

        Dim resultsQuery = (From myRow In dt.AsEnumerable
                            Where Not ElementiDaEscludere.Contains(myRow.Field(Of Integer)("id_cod"))).AsEnumerable()

        If escludiCodiciCliente Then
            resultsQuery = (From x In resultsQuery Where x("id_cod") < 2000 Or x("id_cod") >= 3000).AsEnumerable()
        End If

        Dim resultsNumber = resultsQuery.Count()

        Dim results As DataTable = Nothing
        If resultsNumber > 0 Then

            Dim resQ = (From myRow In dt.AsEnumerable
                        Where Not ElementiDaEscludere.Contains(myRow.Field(Of Integer)("id_cod"))).AsEnumerable()

            If escludiCodiciCliente Then
                resQ = (From x In resQ Where x("id_cod") < 2000 Or x("id_cod") >= 3000).AsEnumerable()
            End If

            results = resQ.CopyToDataTable

        End If

        If results IsNot Nothing Then
            If (results.Rows.Count = 0) Then
                Return Nothing
            End If
            For Each row In results.Rows
                Dim codiceAnagrafeValori As New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori
                codiceAnagrafeValori.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(row("Validita_Inizio"), row("Validita_Fine"))
                codiceAnagrafeValori.valore = row("val_cod")
                codiceAnagrafeValori.codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(row("id_cod")) With {
                    .descrizione = row("descrizione")
                }
                cod_list.Add(codiceAnagrafeValori)
            Next
        End If

        Return cod_list
    End Function

    Public Function Leggi_Codici_Impresa(Piva As String,
                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef ElementiDaEscludere As List(Of enum_CodiciAnagrafe),
                                         escludiCodiciCliente As Boolean) As List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)
        Return Leggi_CodiciAnagrafeValori(Piva, 0, 0, 0, 0, "", enum_EntitaAlberoImprese.Impresa, ElementiDaEscludere, objParametri_Server, escludiCodiciCliente)

    End Function

    Public Function Leggi_Codici_Centro(Piva As String,
                                        Sa_Cod As Integer,
                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                        ByRef ElementiDaEscludere As List(Of enum_CodiciAnagrafe),
                                        escludiCodiciCliente As Boolean) As List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)

        Return Leggi_CodiciAnagrafeValori(Piva, Sa_Cod, 0, 0, 0, "", enum_EntitaAlberoImprese.Centro, ElementiDaEscludere, objParametri_Server, escludiCodiciCliente)

    End Function

    Public Function Leggi_Codici_Appezzamento(Piva As String,
                                              sa_cod As Integer,
                                              appezza As Integer,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              escludiCodiciCliente As Boolean) As List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)

        Return Leggi_CodiciAnagrafeValori(Piva, sa_cod, appezza, 0, 0, "", enum_EntitaAlberoImprese.Contatto, New List(Of enum_CodiciAnagrafe), objParametri_Server, escludiCodiciCliente)

    End Function
    Public Function Leggi_Codici_Campo(Piva As String,
                                       sa_cod As Integer,
                                       campo_cod As Integer,
                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                       escludiCodiciCliente As Boolean) As List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)

        Return Leggi_CodiciAnagrafeValori(Piva, sa_cod, 0, campo_cod, 0, "", enum_EntitaAlberoImprese.Contatto, New List(Of enum_CodiciAnagrafe), objParametri_Server, escludiCodiciCliente)

    End Function

    Public Function Leggi_Codici_RegImpianti(Piva As String,
                                             sa_cod As Integer,
                                             appezza As Integer,
                                             id_reg As Integer,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             escludiCodiciCliente As Boolean) As List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)

        Return Leggi_CodiciAnagrafeValori(Piva, sa_cod, appezza, 0, id_reg, "", enum_EntitaAlberoImprese.Contatto, New List(Of enum_CodiciAnagrafe), objParametri_Server, escludiCodiciCliente)

    End Function



    Private Function Leggi_CodiceAnagrafeValori(Piva As String,
                                                Sa_Cod As Integer,
                                                Appezza As Integer,
                                                Campo_Cod As Integer,
                                                ID_Reg As Integer,
                                                Cod_Contatto As String,
                                                id_cod As Integer,
                                                progetto_cod As Integer,
                                                Elemento_Anagrafico As enum_EntitaAlberoImprese,
                                                ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori

        Dim codice As New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori
        Dim dt As DataTable

        Select Case Elemento_Anagrafico
            Case enum_EntitaAlberoImprese.Impresa
                Dim imprese_Codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                dt = imprese_Codici.Leggi(Piva, id_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Centro
                Dim centri_Codici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
                dt = centri_Codici.Leggi(Piva, Sa_Cod, id_cod, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Contatto
                Dim contatti_Codici As New AgronicaCoreAnagrafeDAL.Contatti_Codici_R
                dt = contatti_Codici.Leggi(Piva, Cod_Contatto, id_cod, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Appezzamento
                Dim appezzamenti_Codici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
                dt = appezzamenti_Codici.Leggi(Piva, Sa_Cod, Appezza, id_cod, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Campo
                Dim campi_Codici As New AgronicaCoreAnagrafeDAL.Campi_Codici_Read
                dt = campi_Codici.Leggi(Piva, Sa_Cod, Campo_Cod, id_cod, "", AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Impianto
                Dim reg_impianti_Codici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
                dt = reg_impianti_Codici.Leggi(Piva, Sa_Cod, Appezza, ID_Reg, "", id_cod, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Distinta
                Dim reg_impianti_Codici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
                dt = reg_impianti_Codici.LeggixProgetto(Piva, Sa_Cod, Appezza, ID_Reg, "", progetto_cod, id_cod, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Case Else
                Throw New Exception("Lettura Indirizzo Associato su elemento anagrafico " & Elemento_Anagrafico & " non gestito ")
        End Select

        'If ElementiDaEscludere Is Nothing Then
        '    ElementiDaEscludere = New List(Of enum_CodiciAnagrafe)
        'End If

        'Dim results = (From myRow In dt.AsEnumerable
        '               Where Not ElementiDaEscludere.Contains(myRow.Field(Of Integer)("id_cod"))).CopyToDataTable

        'If results IsNot Nothing Then
        '    If (results.Rows.Count = 0) Then
        '        Return Nothing
        '    End If
        '    For Each row In results.Rows
        '        Dim codiceAnagrafeValori As New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori
        '        codiceAnagrafeValori.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(row("Validita_Inizio"), row("Validita_Fine"))
        '        codiceAnagrafeValori.valore = row("val_cod")
        '        codiceAnagrafeValori.codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(row("id_cod")) With {
        '            .descrizione = row("descrizione")
        '        }
        '    Next
        'End If

        Return codice

    End Function


    Public Function Leggi_Codice_Impresa(Piva As String,
                                         id_cod As Integer,
                                         ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori

        Return Leggi_CodiceAnagrafeValori(Piva, 0, 0, 0, 0, "", id_cod, 0, enum_EntitaAlberoImprese.Impresa, objParametri_Server)

    End Function

    Public Function Leggi_Codice_Centro(Piva As String,
                                        Sa_Cod As Integer,
                                        id_cod As Integer,
                                            objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori

        Return Leggi_CodiceAnagrafeValori(Piva, Sa_Cod, 0, 0, 0, "", id_cod, 0, enum_EntitaAlberoImprese.Centro, objParametri_Server)

    End Function

    Public Function Leggi_Codice_Appezzamento(Piva As String,
                                         sa_cod As Integer,
                                         appezza As Integer,
                                         id_cod As Integer,
                                              ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori

        Return Leggi_CodiceAnagrafeValori(Piva, sa_cod, appezza, 0, 0, "", id_cod, 0, enum_EntitaAlberoImprese.Contatto, objParametri_Server)

    End Function
    Public Function Leggi_Codice_Campo(Piva As String,
                                       sa_cod As Integer,
                                       campo_cod As Integer,
                                       id_cod As Integer,
                                            ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori

        Return Leggi_CodiceAnagrafeValori(Piva, sa_cod, 0, campo_cod, 0, "", id_cod, 0, enum_EntitaAlberoImprese.Contatto, objParametri_Server)

    End Function

    Public Function Leggi_Codici_Contatto(Piva As String,
                                                       Cod_Contatto As String,
                                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                        escludiCodiciCliente As Boolean) As List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)
        Return Leggi_CodiciAnagrafeValori(Piva, 0, 0, 0, 0, Cod_Contatto, enum_EntitaAlberoImprese.Contatto, New List(Of enum_CodiciAnagrafe), objParametri_Server, escludiCodiciCliente)
    End Function

    Public Function Leggi_Codice_RegImpianti(Piva As String,
                                             sa_cod As Integer,
                                             appezza As Integer,
                                             id_reg As Integer,
                                             id_cod As Integer,
                                                ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori

        Return Leggi_CodiceAnagrafeValori(Piva, sa_cod, appezza, 0, id_reg, "", id_cod, 0, enum_EntitaAlberoImprese.Contatto, objParametri_Server)
    End Function
    Public Function Leggi_Codice_Distinta(Piva As String,
                                             sa_cod As Integer,
                                             appezza As Integer,
                                             id_reg As Integer,
                                             id_cod As Integer,
                                             progetto_cod As Integer,
                                                ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori
        Return Leggi_CodiceAnagrafeValori(Piva, sa_cod, appezza, 0, id_reg, "", id_cod, progetto_cod, enum_EntitaAlberoImprese.Contatto, objParametri_Server)

    End Function
End Class


'################################################################################################################################################################################
'################################################################################################################################################################################
'################################################################################################################################################################################


Public Class Codici_W

    Private Function Scrivi_Codici_Anagrafe_Valori(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                   ByRef Impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                   ByRef Centri As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                   ByRef Contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                   ByRef Appezzamento As AgronicaCoreEntityFramework_POCO.Appezzamento,
                                   ByRef Campi As AgronicaCoreEntityFramework_POCO.Campi,
                                   ByRef Reg_Impianti As AgronicaCoreEntityFramework_POCO.Reg_Impianti,
                                   ByRef Imprese_Progetti As AgronicaCoreEntityFramework_POCO.Imprese_Progetti,
                                   Elemento_Anagrafico As enum_EntitaAlberoImprese,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                                   Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                   Optional ByVal NewTransaction As Boolean = True)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Codici_W.Scrivi_Codici_Anagrafe_Valori()"
        Dim messaggioErrore As String = ""

        Dim username As String = If(objParametri_Utenti.UtenteUsername <> "", objParametri_Utenti.UtenteUsername, objParametri_Server.UsernameOperazione)

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim id_cod = CodiciAnagrafeValori.codiceAnagrafe.codice
        Dim val_cod = CodiciAnagrafeValori.valore
        Dim validita = CodiciAnagrafeValori.validita

        Try
            If val_cod <> "" AndAlso val_cod <> "0" Then

                Select Case Elemento_Anagrafico
                    Case enum_EntitaAlberoImprese.Impresa

                        Dim PIVA = Impresa.PIVA
                        Dim ImpresexCodiciEF As AgronicaCoreEntityFramework_POCO.Imprese_Codici

                        ImpresexCodiciEF = (From ixc In GiasContext.Imprese_Codici Where
                                                        ixc.id_cod = id_cod AndAlso
                                                        PIVA = ixc.PIVA).FirstOrDefault()

                        If ImpresexCodiciEF Is Nothing Then
                            ImpresexCodiciEF = AgronicaCoreAnagrafeDAL.EFImprese.CreateImprese_CodiciEF(GiasContext, objParametri_Server, Impresa, id_cod, val_cod, username)
                        End If

                        ImpresexCodiciEF.val_cod = val_cod
                        ImpresexCodiciEF.Username_Modifica = username
                        ImpresexCodiciEF.Data_Modifica = DateTime.Now

                    Case enum_EntitaAlberoImprese.Centro

                        Dim PIVA = Centri.PIVA
                        Dim SA_COD = Centri.sa_cod

                        Dim CentrixCodiciEF As AgronicaCoreEntityFramework_POCO.Centri_Aziendali_Codici

                        CentrixCodiciEF = (From cxc In GiasContext.Centri_Aziendali_Codici Where
                                                        cxc.id_cod = id_cod AndAlso
                                                        PIVA = cxc.PIVA AndAlso
                                                        SA_COD = cxc.sa_cod).FirstOrDefault()

                        If CentrixCodiciEF Is Nothing Then
                            CentrixCodiciEF = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateCentri_AziendaliCodici(GiasContext, objParametri_Server, Centri, id_cod, val_cod, username)
                        End If

                        CentrixCodiciEF.val_cod = val_cod
                        CentrixCodiciEF.Username_Modifica = username
                        CentrixCodiciEF.Data_Modifica = DateTime.Now

                    Case enum_EntitaAlberoImprese.Campo

                        Dim PIVA = Campi.Piva
                        Dim SA_COD = Campi.Sa_Cod
                        Dim campo_cod = Campi.Campo_Cod

                        Dim codici = From cxc In GiasContext.Campi_Codici
                                     Where cxc.id_cod = id_cod AndAlso
                                        PIVA = cxc.PIVA AndAlso
                                        campo_cod = cxc.campo_cod AndAlso
                                        SA_COD = cxc.sa_cod
                                     Select cxc

                        Dim codice As Campi_Codici = codici.FirstOrDefault()

                        Dim efCampi As New EFCampi()

                        If codice Is Nothing Then
                            codice = efCampi.Create_CampiCodici(GiasContext, objParametri_Server, Campi, id_cod, val_cod, username)
                        End If

                        codice.val_cod = val_cod
                        codice.Username_Modifica = username
                        codice.Data_Modifica = DateTime.Now

                        'TODO Salvo: togliere questi commenti
                        '---------------------------------------------------------

                        GiasContext.Campi_Codici.Attach(codice)
                        GiasContext.Entry(codice).State = EntityState.Added
                        GiasContext.SaveChanges()

                        ''Scrittura tabella Agronica_Log_Anagrafe
                        'Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                        'Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.CampiCodici,
                        '                                                                    CStr(codice.PIVA), CStr(SA_COD),
                        '                                                                    CStr(codice.campo_cod), Nothing,
                        '                                                                    Nothing, Nothing,
                        '                                                                    enum_TipoOperazioneDB.Scrittura,
                        '                                                                    objParametri_Server, enum_Id_Servizio.GiasOnline)

                        'GiasContext.Agronica_Log_Anagrafe.Add(log)
                        'GiasContext.SaveChanges()

                    Case enum_EntitaAlberoImprese.Appezzamento

                        Dim PIVA = Appezzamento.PIVA
                        Dim SA_COD = Appezzamento.SA_COD
                        Dim appezza = Appezzamento.APPEZZA

                        Dim AppezzamentixCodiciEF As AgronicaCoreEntityFramework_POCO.Appezzamento_Codici

                        AppezzamentixCodiciEF = (From axc In GiasContext.Appezzamento_Codici Where
                                                        axc.id_cod = id_cod AndAlso
                                                        PIVA = axc.PIVA AndAlso
                                                        appezza = axc.appezza AndAlso
                                                        SA_COD = axc.sa_cod).FirstOrDefault()

                        If AppezzamentixCodiciEF Is Nothing Then
                            AppezzamentixCodiciEF = AgronicaCoreAnagrafeDAL.EFAppezzamento.Create_AppezzamentoCodici(GiasContext, Appezzamento, id_cod, val_cod, username)
                        End If

                        AppezzamentixCodiciEF.val_cod = val_cod
                        AppezzamentixCodiciEF.Username_Modifica = username
                        AppezzamentixCodiciEF.Data_Modifica = DateTime.Now

                    Case enum_EntitaAlberoImprese.Impianto

                        Dim PIVA = Reg_Impianti.PIVA
                        Dim SA_COD = Reg_Impianti.SA_COD
                        Dim appezza = Reg_Impianti.APPEZZA
                        Dim id_Reg = Reg_Impianti.ID_REG

                        Dim ImpiantixCodiciEF As AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici

                        ImpiantixCodiciEF = (From ixc In GiasContext.Reg_Impianti_Codici Where
                                                        ixc.id_cod = id_cod AndAlso
                                                        PIVA = ixc.PIVA AndAlso
                                                        appezza = ixc.appezza AndAlso
                                                        SA_COD = ixc.sa_cod AndAlso
                                                        id_Reg = ixc.Id_Reg AndAlso
                                                        ixc.Progetto_Cod = 0).FirstOrDefault()

                        If ImpiantixCodiciEF Is Nothing Then
                            ImpiantixCodiciEF = AgronicaCoreAnagrafeDAL.EFReg_Impianti.Create_RegImpiantiCodici(GiasContext, Reg_Impianti, id_cod, val_cod, username)
                        End If

                        ImpiantixCodiciEF.val_cod = val_cod
                        ImpiantixCodiciEF.Username_Modifica = username
                        ImpiantixCodiciEF.Data_Modifica = DateTime.Now

                    Case enum_EntitaAlberoImprese.Distinta

                        Dim PIVA = Imprese_Progetti.Piva
                        Dim SA_COD = Imprese_Progetti.Sa_Cod
                        Dim appezza = Imprese_Progetti.Appezza
                        Dim id_Reg = Imprese_Progetti.Id_Reg
                        Dim progetto_cod = Imprese_Progetti.Progetto_Cod

                        Dim DistintexCodiciEF As AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici

                        DistintexCodiciEF = (From dxc In GiasContext.Reg_Impianti_Codici Where
                                                        dxc.id_cod = id_cod AndAlso
                                                        PIVA = dxc.PIVA AndAlso
                                                        appezza = dxc.appezza AndAlso
                                                        SA_COD = dxc.sa_cod AndAlso
                                                        id_Reg = dxc.Id_Reg AndAlso
                                                        dxc.Progetto_Cod = progetto_cod).FirstOrDefault()

                        If DistintexCodiciEF Is Nothing Then
                            DistintexCodiciEF = AgronicaCoreAnagrafeDAL.EFReg_Impianti.Create_RegImpiantiCodici(GiasContext, Reg_Impianti, id_cod, val_cod, username)
                        End If

                        DistintexCodiciEF.val_cod = val_cod
                        DistintexCodiciEF.Username_Modifica = username
                        DistintexCodiciEF.Data_Modifica = DateTime.Now

                    Case enum_EntitaAlberoImprese.Contatto

                        Dim PIVA = Contatto.Piva
                        Dim cod_contatto = Contatto.Cod_Contatto


                        Dim ContattoxCodiciEF As AgronicaCoreEntityFramework_POCO.Contatti_Codici

                        ContattoxCodiciEF = (From cxc In GiasContext.Contatti_Codici Where
                                                        cxc.Id_cod = id_cod AndAlso
                                                        PIVA = cxc.PIVA AndAlso
                                                        cod_contatto = cxc.Cod_Contatto).FirstOrDefault()

                        If ContattoxCodiciEF Is Nothing Then
                            ContattoxCodiciEF = AgronicaCoreAnagrafeDAL.EFContatti.Create_ContattiCodici(GiasContext, objParametri_Server, Contatto, id_cod, val_cod, username)
                        End If

                        ContattoxCodiciEF.Val_cod = val_cod
                        ContattoxCodiciEF.Username_Modifica = username
                        ContattoxCodiciEF.Data_Modifica = DateTime.Now

                    Case Else
                        Throw New Exception("Scrittura Codice Anagrafe su elemento anagrafico " & Elemento_Anagrafico & " non riuscita. ")
                End Select

            Else

                Select Case Elemento_Anagrafico
                    Case enum_EntitaAlberoImprese.Impresa

                        Dim ImpresexCodiciEF As AgronicaCoreEntityFramework_POCO.Imprese_Codici
                        Dim PIVA = Impresa.PIVA

                        ImpresexCodiciEF = (From ixc In GiasContext.Imprese_Codici Where
                                                        ixc.id_cod = id_cod AndAlso
                                                        PIVA = ixc.PIVA).FirstOrDefault()

                        If ImpresexCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Imprese_Codici.Attach(ImpresexCodiciEF)
                            GiasContext.Imprese_Codici.Remove(ImpresexCodiciEF)

                            'RW = re-write
                            Dim RW_ImpresexCodiciEF = AgronicaCoreAnagrafeDAL.EFImprese.CreateImprese_CodiciEF(GiasContext, objParametri_Server, Impresa, id_cod, val_cod, objParametri_Utenti.UsernameOperazione)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Centro

                        Dim PIVA = Centri.PIVA
                        Dim SA_COD = Centri.sa_cod

                        Dim CentrixCodiciEF As AgronicaCoreEntityFramework_POCO.Centri_Aziendali_Codici

                        CentrixCodiciEF = (From cxc In GiasContext.Centri_Aziendali_Codici Where
                                                        cxc.id_cod = id_cod AndAlso
                                                        PIVA = cxc.PIVA AndAlso
                                                        SA_COD = cxc.sa_cod).FirstOrDefault()

                        If CentrixCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Centri_Aziendali_Codici.Attach(CentrixCodiciEF)
                            GiasContext.Centri_Aziendali_Codici.Remove(CentrixCodiciEF)

                            'RW = re-write
                            Dim RW_CentrixCodiciEF = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateCentri_AziendaliCodici(GiasContext, objParametri_Server, Centri, id_cod, val_cod, objParametri_Utenti.UsernameOperazione)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Campo

                        Dim PIVA = Campi.Piva
                        Dim SA_COD = Campi.Sa_Cod
                        Dim campo_cod = Campi.Campo_Cod

                        Dim CampixCodiciEF As AgronicaCoreEntityFramework_POCO.Campi_Codici

                        CampixCodiciEF = (From cxc In GiasContext.Campi_Codici Where
                                                        cxc.id_cod = id_cod AndAlso
                                                        PIVA = cxc.PIVA AndAlso
                                                        campo_cod = cxc.campo_cod AndAlso
                                                        SA_COD = cxc.sa_cod).FirstOrDefault()

                        If CampixCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Campi_Codici.Attach(CampixCodiciEF)
                            GiasContext.Campi_Codici.Remove(CampixCodiciEF)

                            Dim efCampi As New EFCampi()

                            'RW = re-write
                            Dim RW_CampixCodiciEF = efCampi.Create_CampiCodici(GiasContext, objParametri_Server, Campi, id_cod, val_cod, objParametri_Utenti.UsernameOperazione)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Appezzamento

                        Dim PIVA = Appezzamento.PIVA
                        Dim SA_COD = Appezzamento.SA_COD
                        Dim appezza = Appezzamento.APPEZZA

                        Dim AppezzamentixCodiciEF As AgronicaCoreEntityFramework_POCO.Appezzamento_Codici

                        AppezzamentixCodiciEF = (From axc In GiasContext.Appezzamento_Codici Where
                                                        axc.id_cod = id_cod AndAlso
                                                        PIVA = axc.PIVA AndAlso
                                                        appezza = axc.appezza AndAlso
                                                        SA_COD = axc.sa_cod).FirstOrDefault()

                        If AppezzamentixCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Appezzamento_Codici.Attach(AppezzamentixCodiciEF)
                            GiasContext.Appezzamento_Codici.Remove(AppezzamentixCodiciEF)

                            'RW = re-write
                            Dim RW_AppezzamentixCodiciEF = AgronicaCoreAnagrafeDAL.EFAppezzamento.Create_AppezzamentoCodici(GiasContext, Appezzamento, id_cod, val_cod, username)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Impianto

                        Dim PIVA = Reg_Impianti.PIVA
                        Dim SA_COD = Reg_Impianti.SA_COD
                        Dim appezza = Reg_Impianti.APPEZZA
                        Dim id_Reg = Reg_Impianti.ID_REG

                        Dim ImpiantixCodiciEF As AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici

                        ImpiantixCodiciEF = (From ixc In GiasContext.Reg_Impianti_Codici Where
                                                        ixc.id_cod = id_cod AndAlso
                                                        PIVA = ixc.PIVA AndAlso
                                                        appezza = ixc.appezza AndAlso
                                                        SA_COD = ixc.sa_cod AndAlso
                                                        id_Reg = ixc.Id_Reg AndAlso
                                                        ixc.Progetto_Cod = 0).FirstOrDefault()

                        If ImpiantixCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Reg_Impianti_Codici.Attach(ImpiantixCodiciEF)
                            GiasContext.Reg_Impianti_Codici.Remove(ImpiantixCodiciEF)

                            'RW = re-write
                            Dim RW_ImpiantixCodiciEF = AgronicaCoreAnagrafeDAL.EFReg_Impianti.Create_RegImpiantiCodici(GiasContext, Reg_Impianti, id_cod, val_cod, username)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Distinta

                        Dim PIVA = Imprese_Progetti.Piva
                        Dim SA_COD = Imprese_Progetti.Sa_Cod
                        Dim appezza = Imprese_Progetti.Appezza
                        Dim id_Reg = Imprese_Progetti.Id_Reg
                        Dim progetto_cod = Imprese_Progetti.Progetto_Cod

                        Dim DistintexCodiciEF As AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici

                        DistintexCodiciEF = (From dxc In GiasContext.Reg_Impianti_Codici Where
                                                        dxc.id_cod = id_cod AndAlso
                                                        PIVA = dxc.PIVA AndAlso
                                                        appezza = dxc.appezza AndAlso
                                                        SA_COD = dxc.sa_cod AndAlso
                                                        id_Reg = dxc.Id_Reg AndAlso
                                                        dxc.Progetto_Cod = progetto_cod).FirstOrDefault()

                        If DistintexCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Reg_Impianti_Codici.Attach(DistintexCodiciEF)
                            GiasContext.Reg_Impianti_Codici.Remove(DistintexCodiciEF)

                            'RW = re-write
                            Dim RW_DistintexCodiciEF = AgronicaCoreAnagrafeDAL.EFReg_Impianti.Create_RegImpiantiCodici(GiasContext, Reg_Impianti, id_cod, val_cod, username)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Contatto

                        Dim PIVA = Contatto.Piva
                        Dim cod_contatto = Contatto.Cod_Contatto
                        Dim SA_COD = Contatto.Sa_Cod

                        Dim ContattoxCodiciEF As AgronicaCoreEntityFramework_POCO.Contatti_Codici

                        ContattoxCodiciEF = (From cxc In GiasContext.Contatti_Codici Where
                                                        cxc.Id_cod = id_cod AndAlso
                                                        PIVA = cxc.PIVA AndAlso
                                                        cod_contatto = cxc.Cod_Contatto).FirstOrDefault()

                        If ContattoxCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Contatti_Codici.Attach(ContattoxCodiciEF)
                            GiasContext.Contatti_Codici.Remove(ContattoxCodiciEF)

                            'RW = re-write
                            Dim RW_ContattoxCodiciEF = AgronicaCoreAnagrafeDAL.EFContatti.Create_ContattiCodici(GiasContext, objParametri_Server, Contatto, id_cod, val_cod, username)

                        Else
                            'non faccio nulla
                        End If

                    Case Else
                        Throw New Exception("Modifica Codice Anagrafe su elemento anagrafico " & Elemento_Anagrafico & " non riuscita. ")
                End Select

                GiasContext.SaveChanges()

                'If NewTransaction Then
                '    scope.Complete()
                '    scope.Dispose()
                'End If

            End If

        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If NewTransaction Then
            scope.Complete()
            scope.Dispose()
        End If

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return id_cod

    End Function

    Private Function Scrivi_Codici_Anagrafe_Valori_NoEF(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                   ByRef Impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                   ByRef Centri As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                   ByRef Contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                   ByRef Appezzamento As AgronicaCoreEntityFramework_POCO.Appezzamento,
                                   ByRef Campi As AgronicaCoreEntityFramework_POCO.Campi,
                                   ByRef Reg_Impianti As AgronicaCoreEntityFramework_POCO.Reg_Impianti,
                                   ByRef Imprese_Progetti As AgronicaCoreEntityFramework_POCO.Imprese_Progetti,
                                   Elemento_Anagrafico As enum_EntitaAlberoImprese,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                                   Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                   Optional ByVal NewTransaction As Boolean = True)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Codici_W.Scrivi_Codici_Anagrafe_Valori()"
        Dim messaggioErrore As String = ""

        Dim username As String = If(objParametri_Utenti.UtenteUsername <> "", objParametri_Utenti.UtenteUsername, objParametri_Server.UsernameOperazione)

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim id_cod = CodiciAnagrafeValori.codiceAnagrafe.codice
        Dim val_cod = CodiciAnagrafeValori.valore
        Dim validita = CodiciAnagrafeValori.validita

        Try
            If val_cod <> "" AndAlso val_cod <> "0" Then

                Select Case Elemento_Anagrafico
                    Case enum_EntitaAlberoImprese.Impresa

                        Dim PIVA = Impresa.PIVA
                        Dim Imprese_Codici_Write_DAL As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Write

                        Imprese_Codici_Write_DAL.Scrivi(PIVA, id_cod, val_cod, validita.inizio, validita.fine, objParametri_Server, Date.Now, Date.Now, username, username)

                    Case enum_EntitaAlberoImprese.Centro

                        Dim PIVA = Centri.PIVA
                        Dim SA_COD = Centri.sa_cod

                        Dim Centri_Codici_Write_DAL As New AgronicaCoreAnagrafeDAL.Centri_Codici_Write

                        Centri_Codici_Write_DAL.Scrivi(PIVA, SA_COD, id_cod, val_cod, validita.inizio, validita.fine, objParametri_Server, Date.Now, Date.Now, username, username)

                    Case enum_EntitaAlberoImprese.Campo

                        Dim PIVA = Campi.Piva
                        Dim SA_COD = Campi.Sa_Cod
                        Dim campo_cod = Campi.Campo_Cod

                        Dim Validita_Inizio = If(validita Is Nothing, AGRODATAINIZIO, validita.inizio)
                        Dim Validita_Fine = If(validita Is Nothing, AGRODATAFINE, validita.fine)

                        Dim Campi_Codici_Write_DAL As New AgronicaCoreAnagrafeDAL.Campi_Codici_Write
                        Campi_Codici_Write_DAL.Scrivi(PIVA, SA_COD, campo_cod, id_cod, val_cod, Validita_Inizio, Validita_Fine, objParametri_Server)

                    Case enum_EntitaAlberoImprese.Appezzamento

                        Dim PIVA = Appezzamento.PIVA
                        Dim SA_COD = Appezzamento.SA_COD
                        Dim appezza = Appezzamento.APPEZZA

                        Dim Appezzamenti_Codici_Scrivi As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W
                        Appezzamenti_Codici_Scrivi.Scrivi(PIVA, SA_COD, appezza, id_cod, val_cod, validita.inizio, validita.fine, objParametri_Server)

                    Case enum_EntitaAlberoImprese.Impianto

                        Dim PIVA = Reg_Impianti.PIVA
                        Dim SA_COD = Reg_Impianti.SA_COD
                        Dim appezza = Reg_Impianti.APPEZZA
                        Dim id_Reg = Reg_Impianti.ID_REG

                        Dim Impianti_Codici_Scrivi As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W
                        Impianti_Codici_Scrivi.Scrivi(PIVA, SA_COD, appezza, id_Reg, id_cod, val_cod, validita.inizio, validita.fine, objParametri_Server)

                    Case enum_EntitaAlberoImprese.Distinta

                        Dim PIVA = Imprese_Progetti.Piva
                        Dim SA_COD = Imprese_Progetti.Sa_Cod
                        Dim appezza = Imprese_Progetti.Appezza
                        Dim id_Reg = Imprese_Progetti.Id_Reg

                        Dim Distinte_Codici_Write As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W
                        Distinte_Codici_Write.Scrivi(PIVA, SA_COD, appezza, id_Reg, id_cod, val_cod, validita.inizio, validita.fine, objParametri_Server)

                    Case enum_EntitaAlberoImprese.Contatto

                        Dim PIVA = Contatto.Piva
                        Dim SA_COD = Contatto.Sa_Cod
                        Dim cod_contatto = Contatto.Cod_Contatto

                        Dim Contatto_Codici_Write As New AgronicaCoreAnagrafeDAL.Contatti_Codici_W
                        Contatto_Codici_Write.Scrivi(PIVA, SA_COD, cod_contatto, id_cod, val_cod, validita.inizio, validita.fine, objParametri_Server)

                    Case Else
                        Throw New Exception("Scrittura Codice Anagrafe su elemento anagrafico " & Elemento_Anagrafico & " non riuscita. ")
                End Select

            Else

                Select Case Elemento_Anagrafico
                    Case enum_EntitaAlberoImprese.Impresa

                        Dim ImpresexCodiciEF As AgronicaCoreEntityFramework_POCO.Imprese_Codici
                        Dim PIVA = Impresa.PIVA

                        ImpresexCodiciEF = (From ixc In GiasContext.Imprese_Codici Where
                                                        ixc.id_cod = id_cod AndAlso
                                                        PIVA = ixc.PIVA).FirstOrDefault()

                        If ImpresexCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Imprese_Codici.Attach(ImpresexCodiciEF)
                            GiasContext.Imprese_Codici.Remove(ImpresexCodiciEF)

                            'RW = re-write
                            Dim RW_ImpresexCodiciEF = AgronicaCoreAnagrafeDAL.EFImprese.CreateImprese_CodiciEF(GiasContext, objParametri_Server, Impresa, id_cod, val_cod, objParametri_Utenti.UsernameOperazione)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Centro

                        Dim PIVA = Centri.PIVA
                        Dim SA_COD = Centri.sa_cod

                        Dim CentrixCodiciEF As AgronicaCoreEntityFramework_POCO.Centri_Aziendali_Codici

                        CentrixCodiciEF = (From cxc In GiasContext.Centri_Aziendali_Codici Where
                                                        cxc.id_cod = id_cod AndAlso
                                                        PIVA = cxc.PIVA AndAlso
                                                        SA_COD = cxc.sa_cod).FirstOrDefault()

                        If CentrixCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Centri_Aziendali_Codici.Attach(CentrixCodiciEF)
                            GiasContext.Centri_Aziendali_Codici.Remove(CentrixCodiciEF)

                            'RW = re-write
                            Dim RW_CentrixCodiciEF = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateCentri_AziendaliCodici(GiasContext, objParametri_Server, Centri, id_cod, val_cod, objParametri_Utenti.UsernameOperazione)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Campo

                        Dim PIVA = Campi.Piva
                        Dim SA_COD = Campi.Sa_Cod
                        Dim campo_cod = Campi.Campo_Cod

                        Dim CampixCodiciEF As AgronicaCoreEntityFramework_POCO.Campi_Codici

                        CampixCodiciEF = (From cxc In GiasContext.Campi_Codici Where
                                                        cxc.id_cod = id_cod AndAlso
                                                        PIVA = cxc.PIVA AndAlso
                                                        campo_cod = cxc.campo_cod AndAlso
                                                        SA_COD = cxc.sa_cod).FirstOrDefault()

                        If CampixCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Campi_Codici.Attach(CampixCodiciEF)
                            GiasContext.Campi_Codici.Remove(CampixCodiciEF)

                            Dim efCampi As New EFCampi()

                            'RW = re-write
                            Dim RW_CampixCodiciEF = efCampi.Create_CampiCodici(GiasContext, objParametri_Server, Campi, id_cod, val_cod, objParametri_Utenti.UsernameOperazione)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Appezzamento

                        Dim PIVA = Appezzamento.PIVA
                        Dim SA_COD = Appezzamento.SA_COD
                        Dim appezza = Appezzamento.APPEZZA

                        Dim AppezzamentixCodiciEF As AgronicaCoreEntityFramework_POCO.Appezzamento_Codici

                        AppezzamentixCodiciEF = (From axc In GiasContext.Appezzamento_Codici Where
                                                        axc.id_cod = id_cod AndAlso
                                                        PIVA = axc.PIVA AndAlso
                                                        appezza = axc.appezza AndAlso
                                                        SA_COD = axc.sa_cod).FirstOrDefault()

                        If AppezzamentixCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Appezzamento_Codici.Attach(AppezzamentixCodiciEF)
                            GiasContext.Appezzamento_Codici.Remove(AppezzamentixCodiciEF)

                            'RW = re-write
                            Dim RW_AppezzamentixCodiciEF = AgronicaCoreAnagrafeDAL.EFAppezzamento.Create_AppezzamentoCodici(GiasContext, Appezzamento, id_cod, val_cod, username)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Impianto

                        Dim PIVA = Reg_Impianti.PIVA
                        Dim SA_COD = Reg_Impianti.SA_COD
                        Dim appezza = Reg_Impianti.APPEZZA
                        Dim id_Reg = Reg_Impianti.ID_REG

                        Dim ImpiantixCodiciEF As AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici

                        ImpiantixCodiciEF = (From ixc In GiasContext.Reg_Impianti_Codici Where
                                                        ixc.id_cod = id_cod AndAlso
                                                        PIVA = ixc.PIVA AndAlso
                                                        appezza = ixc.appezza AndAlso
                                                        SA_COD = ixc.sa_cod AndAlso
                                                        id_Reg = ixc.Id_Reg AndAlso
                                                        ixc.Progetto_Cod = 0).FirstOrDefault()

                        If ImpiantixCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Reg_Impianti_Codici.Attach(ImpiantixCodiciEF)
                            GiasContext.Reg_Impianti_Codici.Remove(ImpiantixCodiciEF)

                            'RW = re-write
                            Dim RW_ImpiantixCodiciEF = AgronicaCoreAnagrafeDAL.EFReg_Impianti.Create_RegImpiantiCodici(GiasContext, Reg_Impianti, id_cod, val_cod, username)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Distinta

                        Dim PIVA = Imprese_Progetti.Piva
                        Dim SA_COD = Imprese_Progetti.Sa_Cod
                        Dim appezza = Imprese_Progetti.Appezza
                        Dim id_Reg = Imprese_Progetti.Id_Reg
                        Dim progetto_cod = Imprese_Progetti.Progetto_Cod

                        Dim DistintexCodiciEF As AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici

                        DistintexCodiciEF = (From dxc In GiasContext.Reg_Impianti_Codici Where
                                                        dxc.id_cod = id_cod AndAlso
                                                        PIVA = dxc.PIVA AndAlso
                                                        appezza = dxc.appezza AndAlso
                                                        SA_COD = dxc.sa_cod AndAlso
                                                        id_Reg = dxc.Id_Reg AndAlso
                                                        dxc.Progetto_Cod = progetto_cod).FirstOrDefault()

                        If DistintexCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Reg_Impianti_Codici.Attach(DistintexCodiciEF)
                            GiasContext.Reg_Impianti_Codici.Remove(DistintexCodiciEF)

                            'RW = re-write
                            Dim RW_DistintexCodiciEF = AgronicaCoreAnagrafeDAL.EFReg_Impianti.Create_RegImpiantiCodici(GiasContext, Reg_Impianti, id_cod, val_cod, username)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Contatto

                        Dim PIVA = Contatto.Piva
                        Dim cod_contatto = Contatto.Cod_Contatto
                        Dim SA_COD = Contatto.Sa_Cod

                        Dim ContattoxCodiciEF As AgronicaCoreEntityFramework_POCO.Contatti_Codici

                        ContattoxCodiciEF = (From cxc In GiasContext.Contatti_Codici Where
                                                        cxc.Id_cod = id_cod AndAlso
                                                        PIVA = cxc.PIVA AndAlso
                                                        cod_contatto = cxc.Cod_Contatto).FirstOrDefault()

                        If ContattoxCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Contatti_Codici.Attach(ContattoxCodiciEF)
                            GiasContext.Contatti_Codici.Remove(ContattoxCodiciEF)

                            'RW = re-write
                            Dim RW_ContattoxCodiciEF = AgronicaCoreAnagrafeDAL.EFContatti.Create_ContattiCodici(GiasContext, objParametri_Server, Contatto, id_cod, val_cod, username)

                        Else
                            'non faccio nulla
                        End If

                    Case Else
                        Throw New Exception("Modifica Codice Anagrafe su elemento anagrafico " & Elemento_Anagrafico & " non riuscita. ")
                End Select

                GiasContext.SaveChanges()

                'If NewTransaction Then
                '    scope.Complete()
                '    scope.Dispose()
                'End If

            End If

        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If NewTransaction Then
            scope.Complete()
            scope.Dispose()
        End If

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return id_cod

    End Function

    Public Function Scrivi_Codici_Impresa_NoEF(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                          ByRef GiasContext As Gias_DeveloperServer_Entities,
                                          ByRef Impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional ByVal NewTransaction As Boolean = True)
        Return Scrivi_Codici_Anagrafe_Valori_NoEF(CodiciAnagrafeValori, Impresa, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, enum_EntitaAlberoImprese.Impresa, objParametri_Server, objParametri_Utenti, GiasContext, NewTransaction)
    End Function

    Public Function Scrivi_Codici_Contatto_NoEF(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                          ByRef GiasContext As Gias_DeveloperServer_Entities,
                                          ByRef Contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional ByVal NewTransaction As Boolean = True)

        Return Scrivi_Codici_Anagrafe_Valori_NoEF(CodiciAnagrafeValori, Nothing, Nothing, Contatto, Nothing, Nothing, Nothing, Nothing, enum_EntitaAlberoImprese.Contatto, objParametri_Server, objParametri_Utenti, GiasContext, NewTransaction)
    End Function
    Public Function Scrivi_Codici_Centro_NoEF(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                         ByRef GiasContext As Gias_DeveloperServer_Entities,
                                         ByRef Centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional ByVal NewTransaction As Boolean = True)

        Return Scrivi_Codici_Anagrafe_Valori_NoEF(CodiciAnagrafeValori, Nothing, Centro, Nothing, Nothing, Nothing, Nothing, Nothing, enum_EntitaAlberoImprese.Centro, objParametri_Server, objParametri_Utenti, GiasContext, NewTransaction)
    End Function

    Public Function Scrivi_Codici_Appezzamento_NoEF(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                               ByRef GiasContext As Gias_DeveloperServer_Entities,
                                               ByRef Appezzamento As AgronicaCoreEntityFramework_POCO.Appezzamento,
                                               ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional ByVal NewTransaction As Boolean = True)

        Return Scrivi_Codici_Anagrafe_Valori_NoEF(CodiciAnagrafeValori, Nothing, Nothing, Nothing, Appezzamento, Nothing, Nothing, Nothing, enum_EntitaAlberoImprese.Appezzamento, objParametri_Server, objParametri_Utenti, GiasContext, NewTransaction)
    End Function

    Public Function Scrivi_Codici_RegImpianti_NoEF(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                              ByRef GiasContext As Gias_DeveloperServer_Entities,
                                              ByRef Reg_Impianti As AgronicaCoreEntityFramework_POCO.Reg_Impianti,
                                              ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              Optional ByVal NewTransaction As Boolean = True)

        Return Scrivi_Codici_Anagrafe_Valori_NoEF(CodiciAnagrafeValori, Nothing, Nothing, Nothing, Nothing, Nothing, Reg_Impianti, Nothing, enum_EntitaAlberoImprese.Impianto, objParametri_Server, objParametri_Utenti, GiasContext, NewTransaction)
    End Function

    Public Function Scrivi_Codici_Campo_NoEF(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                        ByRef GiasContext As Gias_DeveloperServer_Entities,
                                        ByRef Campo As AgronicaCoreEntityFramework_POCO.Campi,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal NewTransaction As Boolean = True)

        Return Scrivi_Codici_Anagrafe_Valori_NoEF(CodiciAnagrafeValori, Nothing, Nothing, Nothing, Nothing, Campo, Nothing, Nothing, enum_EntitaAlberoImprese.Campo, objParametri_Server, objParametri_Utenti, GiasContext, NewTransaction)
    End Function

    Public Function Scrivi_Codici_Impresa(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                          ByRef GiasContext As Gias_DeveloperServer_Entities,
                                          ByRef Impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional ByVal NewTransaction As Boolean = True)
        Return Scrivi_Codici_Anagrafe_Valori(CodiciAnagrafeValori, Impresa, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, enum_EntitaAlberoImprese.Impresa, objParametri_Server, objParametri_Utenti, GiasContext, NewTransaction)
    End Function

    Public Function Scrivi_Codici_Contatto(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                          ByRef GiasContext As Gias_DeveloperServer_Entities,
                                          ByRef Contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional ByVal NewTransaction As Boolean = True)

        Return Scrivi_Codici_Anagrafe_Valori(CodiciAnagrafeValori, Nothing, Nothing, Contatto, Nothing, Nothing, Nothing, Nothing, enum_EntitaAlberoImprese.Contatto, objParametri_Server, objParametri_Utenti, GiasContext, NewTransaction)
    End Function
    Public Function Scrivi_Codici_Centro(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                         ByRef GiasContext As Gias_DeveloperServer_Entities,
                                         ByRef Centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional ByVal NewTransaction As Boolean = True)

        Return Scrivi_Codici_Anagrafe_Valori(CodiciAnagrafeValori, Nothing, Centro, Nothing, Nothing, Nothing, Nothing, Nothing, enum_EntitaAlberoImprese.Centro, objParametri_Server, objParametri_Utenti, GiasContext, NewTransaction)
    End Function

    Public Function Scrivi_Codici_Appezzamento(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                               ByRef GiasContext As Gias_DeveloperServer_Entities,
                                               ByRef Appezzamento As AgronicaCoreEntityFramework_POCO.Appezzamento,
                                               ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional ByVal NewTransaction As Boolean = True)

        Return Scrivi_Codici_Anagrafe_Valori(CodiciAnagrafeValori, Nothing, Nothing, Nothing, Appezzamento, Nothing, Nothing, Nothing, enum_EntitaAlberoImprese.Appezzamento, objParametri_Server, objParametri_Utenti, GiasContext, NewTransaction)
    End Function

    Public Function Scrivi_Codici_RegImpianti(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                              ByRef GiasContext As Gias_DeveloperServer_Entities,
                                              ByRef Reg_Impianti As AgronicaCoreEntityFramework_POCO.Reg_Impianti,
                                              ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              Optional ByVal NewTransaction As Boolean = True)

        Return Scrivi_Codici_Anagrafe_Valori(CodiciAnagrafeValori, Nothing, Nothing, Nothing, Nothing, Nothing, Reg_Impianti, Nothing, enum_EntitaAlberoImprese.Impianto, objParametri_Server, objParametri_Utenti, GiasContext, NewTransaction)
    End Function

    Public Function Scrivi_Codici_Campo(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                        ByRef GiasContext As Gias_DeveloperServer_Entities,
                                        ByRef Campo As AgronicaCoreEntityFramework_POCO.Campi,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal NewTransaction As Boolean = True)

        Return Scrivi_Codici_Anagrafe_Valori(CodiciAnagrafeValori, Nothing, Nothing, Nothing, Nothing, Campo, Nothing, Nothing, enum_EntitaAlberoImprese.Campo, objParametri_Server, objParametri_Utenti, GiasContext, NewTransaction)
    End Function

End Class
