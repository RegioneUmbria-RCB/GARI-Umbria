
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports System.Text
Imports System.Data.Entity

Public Class Gruppi_Merce_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                            ByVal Sa_Cod As Integer?,
                            ByVal Id_Gruppo_Merce As Integer?,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Gruppi_Merce_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Gruppi_Merce  ")
            StrSQL.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Not IsNothing(Id_Gruppo_Merce) Then
                StrSQL.AppendLine(" AND   Id_Gruppo_Merce = " & Agro_SQL_SaveNum(Id_Gruppo_Merce) & "   ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND   Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Not IsNothing(Sa_Cod) Then
                StrSQL.AppendLine(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    ''' <summary>
    ''' Restituisce tutti i Gruppi_Merce della Piva che gli viene passata ed anche i Gruppi_Merce che sono pubblici Sa_Cod = -1
    ''' </summary>
    Public Function Leggi_con_Visibilita(ByVal Piva As String,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Gruppi_Merce_R.Leggi_con_Visibilita()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Gruppi_Merce  ")
            StrSQL.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND  ( Piva = '" & Agro_SQL_SaveText(Piva) & "'  OR Sa_Cod = -1 )")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiConRagioneSociale(ByVal piva As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable
        
        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Gruppi_Merce_R.LeggiConRagioneSociale()"

        Dim messaggioErrore = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT imprese.rag_soc RagioneSociale, gruppi_merce.* ")
            StrSQL.AppendLine(" FROM  Gruppi_Merce  ")
            StrSQL.AppendLine(" left join imprese on Gruppi_Merce.piva = imprese.piva")
            StrSQL.AppendLine(" WHERE   Gruppi_Merce.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            ' TODO 
            If piva <> "" Then
                StrSQL.AppendLine(" AND  ( Gruppi_Merce.Piva = '" & Agro_SQL_SaveText(piva) & "'  OR Gruppi_Merce.Sa_Cod = -1 )")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function LeggiGruppiMerciAssociatiProdotti(ByVal Piva As String,
                                                      ByVal Elem_Cod As Integer,
                                                      ByVal Mat_Cod As Integer,
                                                      ByVal Pro_Cod As Integer,
                                                      ByVal Id_Gruppo_Merce As Integer,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByVal xOrderBy As String,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Gruppi_Merce_R.LeggiGruppiMerciAssociatiProdotti()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Gruppi_Merce.Id_Gruppo_Merce, Gruppi_Merce.Codice, Gruppi_Merce.Descrizione ")
            StrSQL.AppendLine("        , Prodotti_Extra_Privata.* ")
            StrSQL.AppendLine(" FROM Prodotti_Extra_Privata ")
            StrSQL.AppendLine(" INNER JOIN Gruppi_Merce ON Prodotti_Extra_Privata.Piva_SuperUser = Gruppi_Merce.Piva_SuperUser ")
            StrSQL.AppendLine("         AND Prodotti_Extra_Privata.Id_Gruppo_Merce = Gruppi_Merce.Id_Gruppo_Merce ")
            StrSQL.AppendLine(" LEFT JOIN Materie_Prime ON Prodotti_Extra_Privata.Piva = Materie_Prime.Piva ")
            StrSQL.AppendLine("         AND Prodotti_Extra_Privata.Mat_Cod = Materie_Prime.Mat_Cod ")
            StrSQL.AppendLine(" WHERE Gruppi_Merce.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND ((Prodotti_Extra_Privata.Pro_Cod <> 0 AND Prodotti_Extra_Privata.Piva = '" & Agro_SQL_SaveText(Piva) & "') --prodotti di banca dati privati miei")
                StrSQL.AppendLine(" OR (Prodotti_Extra_Privata.Mat_Cod <> 0 AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Materie_Prime.Sa_Cod = -1 ))) -- prodotti aziendali privati miei o pubblici ")
            End If

            If Elem_Cod <> 0 Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            End If

            If Mat_Cod <> 0 Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            End If

            If Pro_Cod <> 0 Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " ")
            End If

            If Id_Gruppo_Merce <> 0 Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Id_Gruppo_Merce = " & Agro_SQL_SaveNum(Id_Gruppo_Merce) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Gruppi_Merce.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Gruppi_Merce.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

#Region "Gruppi Merce Default per categoria"

    Private Function ElaboraGruppoMerceDefault(ByVal defaultCategorie As String, ByVal piva As String, ByRef objParametri As AgronicaCoreParametri,
                                               Optional caricaSoltantoIdGruppoMerce As Boolean = False) As List(Of ImpostazioneDefault_GruppiMerce)

        Dim listImpGruppoMerceDefault As New List(Of ImpostazioneDefault_GruppiMerce)

        Dim tempSplit As String() = defaultCategorie.Split({"|"c}, StringSplitOptions.RemoveEmptyEntries)

        Dim listGruppiMerc As New List(Of String)
        For Each item In tempSplit
            Dim elemGruppo As String() = item.Split({"_"c})
            If elemGruppo.Length <> 2 Then
                Throw New Exception("Impostazione Gruppi Merce Default con formato invalido.")
            End If

            Dim objGruppoMerce As New ImpostazioneDefault_GruppiMerce With {
                .Piva = piva,
                .Elem_Cod = CInt(elemGruppo(0)),
                .Id_Gruppo_Merce = CInt(elemGruppo(1))
            }
            listGruppiMerc.Add(CStr(objGruppoMerce.Id_Gruppo_Merce))
            listImpGruppoMerceDefault.Add(objGruppoMerce)
        Next

        If caricaSoltantoIdGruppoMerce Then
            Return listImpGruppoMerceDefault
        End If

        Dim strGruppi As String = ""
        If listGruppiMerc.Count > 0 Then
            strGruppi = String.Join(",", listGruppiMerc.ToArray())
        End If

        If Not String.IsNullOrEmpty(strGruppi) Then

            Dim dtAnaGruppiMerci As DataTable = Leggi("", Nothing, Nothing,
                                                      "Id_Gruppo_Merce IN (" & strGruppi & ")",
                                                      "", objParametri)

            If dtAnaGruppiMerci IsNot Nothing AndAlso dtAnaGruppiMerci.Rows.Count > 0 Then

                For Each itemDefault In listImpGruppoMerceDefault
                    Dim rowAnag As DataRow = dtAnaGruppiMerci.Select("Id_Gruppo_Merce = " & itemDefault.Id_Gruppo_Merce).FirstOrDefault()
                    If rowAnag IsNot Nothing Then
                        itemDefault.Codice = CStr(rowAnag.Item("Codice"))
                        itemDefault.Descrizione = CStr(rowAnag.Item("Descrizione"))
                    End If
                Next

            End If

        End If

        Return listImpGruppoMerceDefault

    End Function

    Public Function GetListGruppiMerceDefault(ByVal piva As String,
                                              ByVal saCod As Integer,
                                              ByRef objParametri_Utenti As AgronicaCoreParametri,
                                              ByRef objParametri_Server As AgronicaCoreParametri
                                              ) As List(Of ImpostazioneDefault_GruppiMerce)

        Const nomeRoutine = "GetListGruppiMerceDefault"
        Dim messaggioErrore As String = ""

        Dim listImpGruppoMerceDefault As New List(Of ImpostazioneDefault_GruppiMerce)

        Try

            Dim impreseImpostazioniR As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
            Dim defaultCategorie As String = impreseImpostazioniR.LeggiScalareMulticentroAziendaSuperUser(piva, New List(Of Integer)({saCod}),
                                                                                              enum_Impostazioni_Utenti.Default_GruppoMerce_CategoriaProdotto,
                                                                                              "",
                                                                                              objParametri_Utenti,
                                                                                              objParametri_Server)

            If Not String.IsNullOrEmpty(defaultCategorie) Then

                listImpGruppoMerceDefault = ElaboraGruppoMerceDefault(defaultCategorie, piva, objParametri_Server)

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return listImpGruppoMerceDefault

    End Function

    Public Function GetListGruppiMerceDefault_MultiAzienda(ByVal listaPive As List(Of String),
                                              ByRef objParametri As AgronicaCoreParametri,
                                              Optional caricaSoltantoGruppoMerce As Boolean = False
                                              ) As List(Of ImpostazioneDefault_GruppiMerce)

        Const nomeRoutine = "GetListGruppiMerceDefault"
        Dim messaggioErrore As String = ""

        Dim listImpGruppoMerceDefault As New List(Of ImpostazioneDefault_GruppiMerce)

        Try

            Dim impreseImpostazioniR As New Imprese_Impostazioni_R

            Dim filtroAggImprese As String = If(listaPive Is Nothing OrElse listaPive.Count = 0, "",
                "Imprese_Impostazioni.Piva IN (" & Agro_SQL_Save_Clausola_IN(" '" & String.Join("', '", listaPive) & "' ") & ")")

            Dim dtDefaultCategorie = impreseImpostazioniR.Leggi("", CostantiPersonalizzate.SACOD_NOFILTRO,
                                                                enum_Impostazioni_Utenti.Default_GruppoMerce_CategoriaProdotto,
                                                                filtroAggImprese, "", objParametri)

            If dtDefaultCategorie.Rows.Count > 0 Then

                For Each dr As DataRow In dtDefaultCategorie.Rows

                    Dim defaultCategorie = dr.Field(Of String)("Impostazione_Valore")

                    listImpGruppoMerceDefault.AddRange(ElaboraGruppoMerceDefault(defaultCategorie, dr.Field(Of String)("Piva"), objParametri, caricaSoltantoIdGruppoMerce:=caricaSoltantoGruppoMerce))
                Next

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return listImpGruppoMerceDefault

    End Function

    Public Function ComponiSql_CreaTempDefaultGruppiMerce(ByVal listaPive As List(Of String),
                                                          ByRef objParametri As AgronicaCoreParametri,
                                                          Optional ByRef listImpGruppoMerceDefault As List(Of ImpostazioneDefault_GruppiMerce) = Nothing) As StringBuilder

        If listImpGruppoMerceDefault Is Nothing Then
            listImpGruppoMerceDefault = GetListGruppiMerceDefault_MultiAzienda(listaPive, objParametri)
        End If

        Dim StrSQL As New StringBuilder

        If Not IsNothing(listImpGruppoMerceDefault) AndAlso listImpGruppoMerceDefault.Count > 0 Then

            StrSQL.AppendLine(" IF OBJECT_ID('tempdb.dbo.#DefaultGruppiMerce') IS NULL BEGIN ")
            StrSQL.AppendLine("    CREATE TABLE #DefaultGruppiMerce ( ")
            StrSQL.AppendLine("        Piva nvarchar(max) null,")
            StrSQL.AppendLine("        Elem_Cod int null,")
            StrSQL.AppendLine("        Id_Gruppo_Merce int null,")
            StrSQL.AppendLine("        Codice nvarchar(max) null,")
            StrSQL.AppendLine("        Descrizione nvarchar(max) null")
            StrSQL.AppendLine("    )")
            StrSQL.AppendLine()

            For Each defaultGrpMerceCategoria In listImpGruppoMerceDefault

                StrSQL.AppendLine(String.Format("    INSERT INTO #DefaultGruppiMerce(Piva, Elem_Cod, Id_Gruppo_Merce, Codice, Descrizione) VALUES('{0}', {1}, {2}, '{3}', '{4}') ",
                                                defaultGrpMerceCategoria.Piva, defaultGrpMerceCategoria.Elem_Cod, defaultGrpMerceCategoria.Id_Gruppo_Merce,
                                                defaultGrpMerceCategoria.Codice, defaultGrpMerceCategoria.Descrizione))
            Next

            StrSQL.AppendLine(" END ")

        End If

        Return StrSQL

    End Function

    Public Function ComponiSql_CancellaTempDefaultGruppiMerce() As StringBuilder

        Dim StrSQL As New StringBuilder

        StrSQL.AppendLine(" IF OBJECT_ID('tempdb.dbo.#DefaultGruppiMerce') IS NOT NULL BEGIN ")
        StrSQL.AppendLine("   DROP TABLE #DefaultGruppiMerce ")
        StrSQL.AppendLine(" END ")

        Return StrSQL

    End Function

#End Region


    Public Function GetProdottiExtraPrivata(objParametriServer As AgronicaCoreParametri) As List(Of AgronicaCoreEntityFramework_POCO.Prodotti_Extra_Privata)
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim prodottiExtra = GiasContext.Prodotti_Extra_Privata.ToList()
        Return prodottiExtra
    End Function


    Public Function CaricaPermessiCollegatiAlGruppoMerce(IdGruppoMerce As Integer, objPServer As AgronicaCoreParametri) As Gruppi_UtenteXGruppi_Merce()
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objPServer.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim prodottiExtra = GiasContext.Gruppi_UtenteXGruppi_Merce.Where(Function(gruppoDB) gruppoDB.Id_Gruppo_Merce = IdGruppoMerce).ToArray()
        Return prodottiExtra
    End Function

End Class

Public Class ImpostazioneDefault_GruppiMerce
    Public Property Piva As String
    Public Property Elem_Cod As Integer
    Public Property Id_Gruppo_Merce As Integer
    Public Property Codice As String
    Public Property Descrizione As String

    Sub New()
        Piva = ""
        Codice = ""
        Descrizione = ""
    End Sub
End Class


Public Class Gruppi_Merce_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public objPServer As AgronicaCoreParametri
    Public objPUtenti As AgronicaCoreParametri
    Public idGenerator As New Agro_Sequenze

    Sub New(objPServer As AgronicaCoreParametri, objPUtenti As AgronicaCoreParametri)
        Me.objPServer = objPServer
        Me.objPUtenti = objPUtenti
    End Sub

    Public Sub CancellaPermeeso(permesso As Gruppi_UtenteXGruppi_Merce)
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objPServer.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)


        GiasContext.Gruppi_UtenteXGruppi_Merce.Attach(permesso)
        GiasContext.Gruppi_UtenteXGruppi_Merce.Remove(permesso)
        GiasContext.SaveChanges()
    End Sub

    Public Sub CancellaGruppoMerce(IdGruppoMerce As Integer)
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objPServer.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)


        Dim gruppoMerce As New Gruppi_Merce() With {.Id_Gruppo_Merce = IdGruppoMerce}
        GiasContext.Gruppi_Merce.Attach(gruppoMerce)
        GiasContext.Gruppi_Merce.Remove(gruppoMerce)
        GiasContext.SaveChanges()
    End Sub

    Public Sub AggiungiGruppoMerce(grMerce As Gruppi_Merce)
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objPServer.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim now = Date.Now
        Dim username = objPServer.UtenteUsername


        grMerce.Piva_SuperUser = objPServer.PivaSuperUser
        grMerce.Id_Gruppo_Merce = idGenerator.NuovoId_Tabella_EF(GiasContext, "Gruppi_Merce", 0, 2000000000, objPServer)
        grMerce.inviato = 0
        grMerce.Data_Creazione = now
        grMerce.Data_Modifica = now
        grMerce.Username_Creazione = username
        grMerce.Username_Modifica = username
        grMerce.Validita_Inizio = CostantiPersonalizzate.AGRODATAINIZIO
        grMerce.Validita_Fine = CostantiPersonalizzate.AGRODATAFINE


        GiasContext.Gruppi_Merce.Add(grMerce)
        GiasContext.SaveChanges()
    End Sub

    Public Sub ModificaGruppoMerce(grMerce As Gruppi_Merce)
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objPServer.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim now = Date.Now
        Dim username = objPServer.UtenteUsername

        Dim dbElem As Gruppi_Merce = GiasContext.Gruppi_Merce.FirstOrDefault(Function(elem) elem.Id_Gruppo_Merce = grMerce.Id_Gruppo_Merce)

        dbElem.Sa_Cod = grMerce.Sa_Cod
        dbElem.Codice = grMerce.Codice
        dbElem.Descrizione = grMerce.Descrizione
        dbElem.Data_Modifica = now
        dbElem.Username_Modifica = username

        GiasContext.SaveChanges()
    End Sub
End Class