Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreModelsSTD.exceptions

<CachedDataProviderAttribute("Imprese_Impostazioni_R")>
Public Class Imprese_Impostazioni_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider


    Public Function LeggiDizionario_WorkflowDocContabili(piva As String,
                                                         ByRef objParametriServer As AgronicaCoreParametri
                                                         ) As Dictionary(Of Integer, enum_Servizi)

        Const nomeRoutine = "CoreAnagrafeDAL.Imprese_Impostazioni_R.LeggiWorkflow_DocContabili()"

        Dim dtImpreseImpostazioni As DataTable

        Dim objImpreseImpostazioni As New Imprese_Impostazioni_R

        Dim dizionario_LavCod_ServizioCod As New Dictionary(Of Integer, enum_Servizi)

        Try

            Dim impostazioni_LavCod_ServizioCod = LeggiStringa_WorkflowDocContabili(piva, objParametriServer)

            If Not String.IsNullOrEmpty(impostazioni_LavCod_ServizioCod) Then

                dizionario_LavCod_ServizioCod = CaricaDizionario_LavCod_ServizioCod(impostazioni_LavCod_ServizioCod)

            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dizionario_LavCod_ServizioCod

    End Function

    Public Function LeggiStringa_WorkflowDocContabili(piva As String,
                                                      ByRef objParametriServer As AgronicaCoreParametri
                                                      ) As String

        Const nomeRoutine = "CoreAnagrafeDAL.Imprese_Impostazioni_R.LeggiWorkflow_DocContabili_Stringa()"

        Dim dtImpreseImpostazioni As DataTable

        Dim objImpreseImpostazioni As New Imprese_Impostazioni_R

        Dim stringa_LavCod_ServizioCod As String = ""

        Try

            dtImpreseImpostazioni = objImpreseImpostazioni.Leggi(piva,
                                                                 0,
                                                                 enum_Impostazioni_Utenti.DocContabili_GestioneWorkFlow,
                                                                 "",
                                                                 "",
                                                                 objParametriServer)

            If Not IsNothing(dtImpreseImpostazioni) AndAlso dtImpreseImpostazioni.Rows.Count > 0 Then

                stringa_LavCod_ServizioCod = CStr(dtImpreseImpostazioni.Rows(0).Item("Impostazione_Valore"))

            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return stringa_LavCod_ServizioCod

    End Function

    Public Function CaricaDizionario_LavCod_ServizioCod(ByVal impostazioni_LavCod_ServizioCod As String) As Dictionary(Of Integer, enum_Servizi)

        Dim dizionario_LavCod_ServizioCod As New Dictionary(Of Integer, enum_Servizi)

        If Not String.IsNullOrEmpty(impostazioni_LavCod_ServizioCod) Then

            Dim separatori() As String = {"|"}
            Dim coppie_LavCod_ServizioCod As String() = impostazioni_LavCod_ServizioCod.Split(separatori, StringSplitOptions.RemoveEmptyEntries)

            For Each coppia In coppie_LavCod_ServizioCod

                Dim elemCoppia As String() = coppia.Split("_")

                dizionario_LavCod_ServizioCod.Add(elemCoppia(0), elemCoppia(1))

            Next

        End If

        Return dizionario_LavCod_ServizioCod

    End Function

    Public Function Ottieni_ServizioCod_Da_LavCod(ByVal LavCod As Integer,
                                                  ByVal dizionario_LavCod_ServizioCod As Dictionary(Of Integer, enum_Servizi)
                                                  ) As Integer

        Dim ServizioCod = 0

        If dizionario_LavCod_ServizioCod.ContainsKey(LavCod) Then
            ServizioCod = dizionario_LavCod_ServizioCod.Item(LavCod)
        End If

        Return ServizioCod

    End Function

    Public Function LavCod_Gestisce_Workflow(ByVal LavCod As Integer,
                                             ByVal dizionario_LavCod_ServizioCod As Dictionary(Of Integer, enum_Servizi)
                                             ) As Boolean

        Dim SeWorkflow = False

        If dizionario_LavCod_ServizioCod.ContainsKey(LavCod) Then
            SeWorkflow = True
        End If

        Return SeWorkflow

    End Function

    ''' <summary>
    ''' per non filtrare il centro usare <see cref="CostantiPersonalizzate.SACOD_NOFILTRO"/>
    ''' </summary>
    ''' <param name="sa_cod">per non filtrare il centro usare <see cref="CostantiPersonalizzate.SACOD_NOFILTRO"/></param>
    Public Function Leggi(ByVal piva As String,
                          ByVal sa_cod As Integer,
                          ByVal impostazione_cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "CoreAnagrafeDAL.Imprese_Impostazioni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0
            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            stb.AppendLine(" SELECT * FROM Imprese_Impostazioni ")
            stb.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If sa_cod <> SACOD_NOFILTRO Then
                stb.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
            End If

            If impostazione_cod <> 0 Then
                stb.AppendLine(" AND Impostazione_cod = " & Agro_SQL_SaveNum(impostazione_cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    ''' <summary>
    ''' Per non filtrare il centro usare <see cref="CostantiPersonalizzate.SACOD_NOFILTRO"/>
    ''' </summary>
    ''' <param name="sa_cod">per non filtrare il centro usare <see cref="CostantiPersonalizzate.SACOD_NOFILTRO"/></param>
    ''' <returns></returns>
    Public Function Leggi(ByVal piva As String,
                          ByVal sa_cod As Integer,
                          ByVal impostazione_cod As Integer,
                          ByVal selezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "CoreAnagrafeDAL.Imprese_Impostazioni_R.Leggi()"

        Dim stb As New StringBuilder With {.Length = 0}
        Dim dt As DataTable

        Try
            Select Case selezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    stb.AppendLine(" SELECT * FROM Imprese_Impostazioni ")
                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    stb.AppendLine(" SELECT Piva_SuperUser, Piva, Sa_Cod, Impostazione_Cod, Impostazione_Valore FROM Imprese_Impostazioni ")
                Case Else
                    stb.AppendLine(" SELECT Imprese_Impostazioni.*, Imprese.rag_soc, coalesce(Centri_Aziendali.sa_nome, '') as sa_nome, ")
                    stb.AppendLine("        Guida_Impostazioni.Flag_InApp, Guida_Impostazioni.Label as Impostazione_Des, Guida_Impostazioni.Tipo_Campo, ")
                    stb.AppendLine(" CASE WHEN ISNULL(imprese.partitaIvaReale, '') = '' THEN imprese.Piva ELSE imprese.partitaIvaReale END AS partitaIvaReale ")
                    stb.AppendLine(" FROM Imprese_Impostazioni ")
                    stb.AppendLine(" LEFT JOIN imprese ON imprese.piva = imprese_impostazioni.piva ")
                    stb.AppendLine(" LEFT JOIN Centri_Aziendali ON centri_aziendali.piva = imprese.PIVA AND Centri_Aziendali.sa_cod = Imprese_Impostazioni.Sa_Cod ")
                    stb.AppendLine(" LEFT JOIN Guida_Impostazioni ON Guida_Impostazioni.Impostazione_Cod = Imprese_Impostazioni.Impostazione_Cod ")
            End Select

            stb.AppendLine(" WHERE Imprese_Impostazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If piva <> "" Then
                stb.AppendLine(" AND Imprese_Impostazioni.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If
            If sa_cod <> SACOD_NOFILTRO Then
                stb.AppendLine(" AND Imprese_Impostazioni.Sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
            End If

            If impostazione_cod <> 0 Then
                stb.AppendLine(" AND Imprese_Impostazioni.Impostazione_cod = " & Agro_SQL_SaveNum(impostazione_cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function

    Public Function LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva As String, sa_cod_lst As List(Of Integer), impostazioneCod As enum_Impostazioni_Utenti, tipoProdotto As Integer, valoreDefault As String, objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim valImpostazione As String = valoreDefault

        'DT: in caso di nessun centro indicato, si aggiunge il centro "0" per attivare la scalarità su azienda 
        If sa_cod_lst Is Nothing OrElse sa_cod_lst.Count = 0 Then
            sa_cod_lst = New List(Of Integer)({0})
        End If

        valImpostazione = LeggiScalareCentroAziendaSuperUser_ElemCod(piva, sa_cod_lst(0), impostazioneCod, tipoProdotto, valoreDefault, objParametri_Utenti, objParametri_Server)

        For Each sa_cod In sa_cod_lst.Skip(1)
            Dim valImpostazioneCorrente = LeggiScalareCentroAziendaSuperUser_ElemCod(piva, sa_cod, impostazioneCod, tipoProdotto, valoreDefault, objParametri_Utenti, objParametri_Server)
            If valImpostazioneCorrente <> valImpostazione Then
                Throw New GiasException(String.Format(Gias.ImpostazioneIncompatibileMulticentro, impostazioneCod.ToString))
            End If
        Next

        Return valImpostazione

    End Function

    Public Function LeggiScalareMulticentroAziendaSuperUser(piva As String, sa_cod_lst As List(Of Integer), impostazioneCod As enum_Impostazioni_Utenti, valoreDefault As String, objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim valImpostazione As String = valoreDefault

        'DT: in caso di nessun centro indicato, si aggiunge il centro "0" per attivare la scalarità su azienda 
        If sa_cod_lst Is Nothing OrElse sa_cod_lst.Count = 0 Then
            sa_cod_lst = New List(Of Integer)({0})
        End If

        valImpostazione = LeggiScalareCentroAziendaSuperUser(piva, sa_cod_lst(0), impostazioneCod, valoreDefault, objParametri_Utenti, objParametri_Server)

        For Each sa_cod In sa_cod_lst.Skip(1)
            Dim valImpostazioneCorrente = LeggiScalareCentroAziendaSuperUser(piva, sa_cod, impostazioneCod, valoreDefault, objParametri_Utenti, objParametri_Server)
            If valImpostazioneCorrente <> valImpostazione Then
                Throw New GiasException(String.Format(Gias.ImpostazioneIncompatibileMulticentro, impostazioneCod.ToString))
            End If
        Next

        Return valImpostazione

    End Function

#Region "Funzioni private"

    Private Function LeggiScalareCentroAziendaSuperUser_ElemCod(piva As String, sa_cod As Integer, impostazioneCod As enum_Impostazioni_Utenti, tipoProdotto As Integer, valoreDefault As String, objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim valImpostazione As String = LeggiScalareCentroAziendaSuperUser(piva, sa_cod, impostazioneCod, valoreDefault, objParametri_Utenti, objParametri_Server)

        If Not String.IsNullOrEmpty(valImpostazione) Then
            Dim impostazione = valImpostazione.Split("|")
            For Each s As String In impostazione
                Dim elem_cod_impostazione = s.Split("_")
                If elem_cod_impostazione(0) = tipoProdotto.ToString Then
                    valImpostazione = elem_cod_impostazione(1)
                    Return valImpostazione
                End If
            Next
        End If

        Return valoreDefault

    End Function

    Private Function LeggiScalareCentroAziendaSuperUser(piva As String, sa_cod As Integer, impostazioneCod As enum_Impostazioni_Utenti, valoreDefault As String, objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        'TODO_DT: da capire come modificare quando arriverà la tabella nuova delle impostazioni con default e "solo azienda"
        Dim valImpostazione As String = valoreDefault

        Dim strImpostazioni = LeggiScalare_Centro_Azienda_SuperUser(piva, sa_cod, impostazioneCod, objParametri_Utenti, objParametri_Server)

        If Not String.IsNullOrEmpty(strImpostazioni) Then
            valImpostazione = strImpostazioni
        End If

        Return valImpostazione

    End Function


    ''' <summary>
    ''' Ritorna la prima impostazione valida scalando il livello: se passato il centro ritorna l'impostazione di quel centro o in sua assenza quella dell'impresa
    ''' </summary>
    ''' <param name="sa_cod">per non filtrare il centro usare <see cref="CostantiPersonalizzate.SACOD_NOFILTRO"/></param>
    Private Function LeggiScalare_Centro_Azienda(ByVal piva As String,
                                                ByVal sa_cod As Integer,
                                                ByVal impostazione_cod As Integer,
                                                ByRef objParametri_Server As AgronicaCoreParametri
                                                ) As String

        Const nomeRoutine = "CoreAnagrafeDAL.Imprese_Impostazioni_R.LeggiScalare_Centro_Azienda()"

        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        If impostazione_cod = 0 Then
            Throw New Exception("Parametro impostazione_cod obbligatorio")
        End If

        Try

            stb.Length = 0

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            stb.AppendLine(" SELECT TOP 1 * FROM Imprese_Impostazioni ")
            stb.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "' ")
            stb.AppendLine(" AND Impostazione_cod = " & Agro_SQL_SaveNum(impostazione_cod) & " ")

            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            'DT: lo 0 deve essere un valore di filtro, solo il -999 ha valore di "nessun filtro"
            If sa_cod <> SACOD_NOFILTRO Then
                stb.AppendLine(" AND (Sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & " OR Sa_Cod = 0)")
            End If

            'DT: ordinamento prefissato per garantire la scalarità centro/azienda
            stb.AppendLine(" ORDER BY Sa_Cod DESC")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        'DT: ritornando direttamente la stringa, il chiamante non riesce a capire da quale livello di scalarità arrivi. Se necessario fare un altro metodo
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 AndAlso Not IsDBNull(dt.Rows(0).Item("Impostazione_Valore")) AndAlso Not String.IsNullOrEmpty(dt.Rows(0).Item("Impostazione_Valore")) Then
            Return dt.Rows(0).Item("Impostazione_Valore")
        Else
            Return String.Empty
        End If

    End Function

    <Cacheable(True)>
    Private Function LeggiScalare_Centro_Azienda_Cacheable(ByVal piva As String,
                                                ByVal sa_cod As Integer,
                                                ByVal impostazione_cod As Integer,
                                                ByRef objParametri_Server As AgronicaCoreParametri
                                                ) As String

        Const nomeRoutine = "CoreAnagrafeDAL.Imprese_Impostazioni_R.LeggiScalare_Centro_Azienda()"

        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        If impostazione_cod = 0 Then
            Throw New Exception("Parametro impostazione_cod obbligatorio")
        End If

        Try

            stb.Length = 0

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            stb.AppendLine(" SELECT TOP 1 * FROM Imprese_Impostazioni ")
            stb.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "' ")
            stb.AppendLine(" AND Impostazione_cod = " & Agro_SQL_SaveNum(impostazione_cod) & " ")

            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            'DT: lo 0 deve essere un valore di filtro, solo il -999 ha valore di "nessun filtro"
            If sa_cod <> SACOD_NOFILTRO Then
                stb.AppendLine(" AND (Sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & " OR Sa_Cod = 0)")
            End If

            'DT: ordinamento prefissato per garantire la scalarità centro/azienda
            stb.AppendLine(" ORDER BY Sa_Cod DESC")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        'DT: ritornando direttamente la stringa, il chiamante non riesce a capire da quale livello di scalarità arrivi. Se necessario fare un altro metodo
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 AndAlso Not IsDBNull(dt.Rows(0).Item("Impostazione_Valore")) AndAlso Not String.IsNullOrEmpty(dt.Rows(0).Item("Impostazione_Valore")) Then
            Return dt.Rows(0).Item("Impostazione_Valore")
        Else
            Return String.Empty
        End If

    End Function

    Private Function LeggiGuidaImpostazioneCod(ByRef impostazione_cod As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "CoreAnagrafeDAL.Imprese_Impostazioni_R.LeggiGuidaImpostazioneCod()"

        Dim stb As New System.Text.StringBuilder
        Dim dtGuida As New DataTable

        If impostazione_cod = 0 Then
            Throw New Exception("Parametro impostazione_cod obbligatorio")
        End If

        Try
            stb.Length = 0

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            stb.AppendLine(" SELECT * FROM Guida_Impostazioni ")
            stb.AppendLine(" WHERE Impostazione_cod = " & Agro_SQL_SaveNum(impostazione_cod) & " ")

            '--------------------------------------------------------------------------
            dtGuida = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dtGuida

    End Function

    <Cacheable(True)>
    Private Function LeggiGuidaImpostazioneCod_Cacheable(ByRef impostazione_cod As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "CoreAnagrafeDAL.Imprese_Impostazioni_R.LeggiGuidaImpostazioneCod()"

        Dim stb As New System.Text.StringBuilder
        Dim dtGuida As New DataTable

        If impostazione_cod = 0 Then
            Throw New Exception("Parametro impostazione_cod obbligatorio")
        End If

        Try
            stb.Length = 0

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            stb.AppendLine(" SELECT * FROM Guida_Impostazioni ")
            stb.AppendLine(" WHERE Impostazione_cod = " & Agro_SQL_SaveNum(impostazione_cod) & " ")

            '--------------------------------------------------------------------------
            dtGuida = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dtGuida

    End Function

    ''' <summary>
    ''' per non filtrare il centro usare <see cref="CostantiPersonalizzate.SACOD_NOFILTRO"/>
    ''' </summary>
    ''' <param name="sa_cod">per non filtrare il centro usare <see cref="CostantiPersonalizzate.SACOD_NOFILTRO"/></param>
    Private Function LeggiScalare_Centro_Azienda_SuperUser(ByVal piva As String,
                                                          ByVal sa_cod As Integer,
                                                          ByVal impostazione_cod As Integer,
                                                          ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                          ByRef objParametri_Server As AgronicaCoreParametri
                                                          ) As String

        Const nomeRoutine = "CoreAnagrafeDAL.Imprese_Impostazioni_R.LeggiScalare_Centro_Azienda_SuperUser()"
        Dim strImpostazione_Valore As String

        Try

            Dim dtGuida = LeggiGuidaImpostazioneCod_Cacheable(impostazione_cod, objParametri_Server)

            Dim defaultDaGuida As String = String.Empty
            Dim flagAzienda As Boolean = False
            Dim flagAziendaCentro As Boolean = False
            Dim flagSuperUser As Boolean = False

            If dtGuida.Rows.Count > 0 Then
                Dim rowGuida = dtGuida.Rows(0)
                flagAzienda = If(rowGuida.Field(Of Integer)("Impostazione_Azienda") = 1, True, False)
                flagAziendaCentro = If(rowGuida.Field(Of Integer)("Impostazione_Azienda_Centro") = 1, True, False)
                flagSuperUser = If(rowGuida.Field(Of Integer)("Impostazione_SuperUser") = 1, True, False)

                defaultDaGuida = rowGuida.Field(Of String)("Valore_Default")
            End If

            If Not String.IsNullOrWhiteSpace(piva) Then

                If flagAziendaCentro AndAlso Not flagAzienda AndAlso sa_cod <> 0 AndAlso sa_cod <> SACOD_NOFILTRO Then
                    Dim dtImpCentro = Leggi(piva, sa_cod, impostazione_cod, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                    If dtImpCentro.Rows.Count > 0 Then
                        strImpostazione_Valore = dtImpCentro.Rows(0).Item("Impostazione_Valore")
                    End If
                ElseIf flagAziendaCentro AndAlso flagAzienda Then
                    'In questo caso, la funzione cerca esattamente per il centro passato ed anche per l'impresa, in quanto al suo interno
                    '"scala" sull'azienda aggiungendo la clausola: OR sa_cod = 0
                    'in caso sa_cod vale SA_COD_NOFILTRO, sta restituendo il valore per il centro creato più di recente sulla specifica impresa
                    strImpostazione_Valore = LeggiScalare_Centro_Azienda_Cacheable(piva, sa_cod, impostazione_cod, objParametri_Server)
                ElseIf flagAzienda Then
                    Dim tuttiICentri = 0
                    strImpostazione_Valore = LeggiScalare_Centro_Azienda_Cacheable(piva, tuttiICentri, impostazione_cod, objParametri_Server)
                End If

            End If

            If flagSuperUser AndAlso String.IsNullOrEmpty(strImpostazione_Valore) Then
                Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim dt = objUtente.Leggi_Cacheable(impostazione_cod, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 AndAlso Not IsDBNull(dt.Rows(0).Item("Impostazione_Valore_1")) AndAlso Not String.IsNullOrEmpty(dt.Rows(0).Item("Impostazione_Valore_1")) Then
                    strImpostazione_Valore = dt.Rows(0).Item("Impostazione_Valore_1")
                Else
                    strImpostazione_Valore = If(defaultDaGuida, String.Empty)
                End If
            End If

        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        'DT: ritornando direttamente la stringa, il chiamante non riesce a capire da quale livello di scalarità arrivi. Se necessario fare un altro metodo
        Return strImpostazione_Valore
    End Function

#End Region
End Class

Public Class Imprese_Impostazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(PIVA As String, Sa_Cod As Integer,
                           Impostazione_Cod As Integer, Impostazione_Value As String,
                           Validita_Inizio As Date, Validita_Fine As Date,
                           objParametri As AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpreseImpotazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Imprese_Impostazioni ( ")
            strSql.AppendLine(" Piva_SuperUser, Piva, Sa_Cod, Impostazione_Cod, Impostazione_Valore, ")
            strSql.AppendLine(" Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ")
            strSql.AppendLine(" ) VALUES ( ")
            strSql.AppendLine(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "', ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(PIVA) & "', ")
            strSql.AppendLine(Agro_SQL_SaveNum(Sa_Cod) & ", ")
            strSql.AppendLine(Agro_SQL_SaveNum(Impostazione_Cod) & ", ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(Impostazione_Value)) & "', ")
            strSql.AppendLine(" GETDATE() , ")
            strSql.AppendLine(" GETDATE() , ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(objParametri.UsernameOperazione)) & "', ")
            strSql.AppendLine(Agro_SQL_SaveDate(Validita_Inizio) & ", ")
            strSql.AppendLine(Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Modifica(PIVA As String, Sa_Cod As Integer,
                             Impostazione_Cod As Integer, Impostazione_Value As String,
                             Validita_Inizio As Date, Validita_Fine As Date,
                             objParametri As AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpreseImpotazioni_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Imprese_Impostazioni SET ")

            strSql.AppendLine("Impostazione_Valore = '" & Agro_SQL_SaveText(Trim(Impostazione_Value)) & "', ")
            strSql.AppendLine("Validita_Inizio =" & Agro_SQL_SaveDate(Validita_Inizio) & ", ")
            strSql.AppendLine("Validita_Fine =" & Agro_SQL_SaveDate(Validita_Fine) & ", ")
            strSql.AppendLine("Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "', ")
            strSql.AppendLine("Data_Modifica = GETDATE() ")

            strSql.AppendLine(" WHERE     (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")

            If Impostazione_Cod <> 0 Then
                strSql.AppendLine(" AND     (Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
            End If

            If PIVA <> "" Then
                strSql.AppendLine(" AND     (Piva = '" & Agro_SQL_SaveText(PIVA) & "')   ")
            End If
            If Sa_Cod <> CostantiPersonalizzate.SACOD_NOFILTRO Then
                strSql.AppendLine(" AND     (Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")   ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function
    Public Function Cancella(PIVA As String, Sa_Cod As Integer,
                             Impostazione_Cod As Integer,
                             objParametri As AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpreseImpotazioni_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0

            strSql.AppendLine(" DELETE FROM Imprese_Impostazioni ")

            strSql.AppendLine(" WHERE     (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")
            strSql.AppendLine(" AND     (Piva = '" & Agro_SQL_SaveText(PIVA) & "')   ")

            If Impostazione_Cod <> 0 Then
                strSql.AppendLine(" AND     (Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
            End If

            If Sa_Cod <> CostantiPersonalizzate.SACOD_NOFILTRO Then
                strSql.AppendLine(" AND     (Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")   ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class


