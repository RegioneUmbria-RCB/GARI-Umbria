Imports System.Text
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class UMA_Vendite_R
    Inherits DataProvider

    ''' <summary>
    ''' Ottiene la somma dei totali di carburante venduti raggruppati per cliente, anno e tipo
    ''' </summary>
    ''' <param name="pivaCliente"></param>
    ''' <param name="annoVendita"></param>
    ''' <param name="contoProprioTerzi"></param>
    ''' <param name="tipoCarb"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri">Vengono aggiunti filtri su piva_superuser e visibilità</param>
    ''' <returns></returns>
    Public Function LeggiCarburanteVenduto(
            ByVal pivaCliente As String,
            ByVal annoVendita As Integer,
            ByVal contoProprioTerzi As Integer,
            ByVal tipoCarb As Integer,
            ByVal xFiltroAggiuntivo As String,
            ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Vendite_R.LeggiCarburanteVenduto()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.AppendLine("SELECT Piva_Cliente, Anno, Tipo_Carburante, SUM(lt) Totale_Carb")
            strSql.AppendLine("FROM UMA_Vendite")
            strSql.AppendLine("WHERE 1=1 ")

            If pivaCliente <> "" Then
                strSql.AppendLine("AND UMA_Vendite.Piva_Cliente = '" & Agro_SQL_SaveText(pivaCliente) & "'")
            End If

            If annoVendita <> 0 Then
                strSql.AppendLine("AND UMA_Vendite.Anno = " & annoVendita)
            End If

            If contoProprioTerzi <> -999 Then
                strSql.AppendLine("AND UMA_Vendite.Conto_Proprio_Terzi = " & contoProprioTerzi)
            End If

            If tipoCarb <> 0 Then
                strSql.AppendLine("AND UMA_Vendite.Tipo_Carburante = " & tipoCarb)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine("AND UMA_Vendite.PivaSuperUser = '" & objParametri.PivaSuperUser & "'")

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine("AND   UMA_Vendite.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine("AND   UMA_Vendite.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------
            'Group by
            strSql.AppendLine("GROUP BY piva_cliente, anno, tipo_carburante")
            '------------

            'If xOrderBy <> "" Then
            '    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function


    ''' <summary>
    ''' Ottiene la somma dei totali di carburante venduti raggruppati per cliente, e anno
    ''' </summary>
    ''' <param name="pivaCliente"></param>
    ''' <param name="annoVendita"></param>
    ''' <param name="contoProprioTerzi"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri">Vengono aggiunti filtri su piva_superuser e visibilità</param>
    ''' <returns></returns>
    Public Function LeggiCarburanteVenduto(
            ByVal pivaCliente As String,
            ByVal annoVendita As Integer,
            ByVal contoProprioTerzi As Integer,
            ByVal xFiltroAggiuntivo As String,
            ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Vendite_R.LeggiCarburanteVenduto()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.AppendLine("SELECT Piva_Cliente, Anno, SUM(lt) Totale_Carb")
            strSql.AppendLine("FROM UMA_Vendite")
            strSql.AppendLine("WHERE 1=1 ")

            If pivaCliente <> "" Then
                strSql.AppendLine("AND UMA_Vendite.Piva_Cliente = '" & Agro_SQL_SaveText(pivaCliente) & "'")
            End If

            If annoVendita <> 0 Then
                strSql.AppendLine("AND UMA_Vendite.Anno = " & annoVendita)
            End If

            If contoProprioTerzi <> -999 Then
                strSql.AppendLine("AND UMA_Vendite.Conto_Proprio_Terzi = " & contoProprioTerzi)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine("AND UMA_Vendite.PivaSuperUser = '" & objParametri.PivaSuperUser & "'")

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine("AND   UMA_Vendite.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine("AND   UMA_Vendite.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------
            'Group by
            strSql.AppendLine("GROUP BY piva_cliente, anno")
            '------------

            'If xOrderBy <> "" Then
            '    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function

    Public Function OttieniDataPrimoAcquisto(ByVal pivaCliente As String,
                                             ByVal annoVendita As Integer,
                                             ByVal contoProprioTerzi As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Vendite_R.ottieniDataPrimoAcquisto()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT top (1) * ")
            strSql.AppendLine(" FROM UMA_Vendite ")
            strSql.AppendLine(" WHERE piva_cliente = '" + Agro_SQL_SaveText(pivaCliente) + "' and anno = " + annoVendita.ToString + " and conto_proprio_terzi = " + contoProprioTerzi.ToString + " ")
            strSql.AppendLine(" order by Data_Documento ASC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function

    Public Function LeggiCarburante(ByVal pivaVenditore As String,
                                    ByVal pivaCliente As String,
                                    ByVal annoVendita As Integer,
                                    ByVal contoProprioTerzi As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Vendite_R.LeggiCarburante()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.AppendLine("SELECT *")
            strSql.AppendLine("FROM UMA_Vendite")
            strSql.AppendLine("WHERE 1=1")

            If pivaVenditore <> "" Then
                strSql.AppendLine("AND UMA_Vendite.Piva_Venditore = '" & Agro_SQL_SaveText(pivaVenditore) & "'")
            End If

            If pivaCliente <> "" Then
                strSql.AppendLine("AND UMA_Vendite.Piva_Cliente = '" & Agro_SQL_SaveText(pivaCliente) & "'")
            End If

            If annoVendita <> 0 Then
                strSql.AppendLine("AND UMA_Vendite.Anno = " & annoVendita)
            End If

            If contoProprioTerzi <> -999 Then
                strSql.AppendLine("AND UMA_Vendite.Conto_Proprio_Terzi = " & contoProprioTerzi)
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine("AND UMA_Vendite.PivaSuperUser = '" & objParametri.PivaSuperUser & "'")

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine("AND UMA_Vendite.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine("AND UMA_Vendite.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt

    End Function

End Class

Public Class UMA_Vendite_W
    Inherits DataProvider

    ''' <summary>
    ''' Effettua l'insert o l'update degli oggetti specificati.
    ''' Se una proprietà nullable è passata nothing viene scritto un valore di default
    ''' </summary>
    ''' <param name="listaVendite"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function Scrivi(ByRef listaVendite As List(Of UMA_Vendite), ByRef objParametri As AgronicaCoreParametri) As String
        Dim messaggioErrore As String = ""
        Dim umaVenditeLog As New UMA_Vendite

        Dim transactionOptions = New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As Gias_DeveloperServer_Entities = Nothing
        Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
            Try
                GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
                GiasContext.Database.Connection.Open()

                Dim handleSequenze = New Agro_Sequenze()

                For Each objUmaVendite As UMA_Vendite In listaVendite

                    Dim venditaSuDatabase =
                        (From vendita In GiasContext.UMA_Vendite
                         Where vendita.PivaSuperUser = objUmaVendite.PivaSuperUser AndAlso
                            vendita.PIVA_Venditore = objUmaVendite.PIVA_Venditore AndAlso
                            vendita.PIVA_Cliente = objUmaVendite.PIVA_Cliente AndAlso
                            vendita.Anno = objUmaVendite.Anno AndAlso
                            vendita.Conto_Proprio_Terzi = objUmaVendite.Conto_Proprio_Terzi AndAlso
                            vendita.Data_Documento = objUmaVendite.Data_Documento AndAlso
                            vendita.Nr_Documento = objUmaVendite.Nr_Documento).FirstOrDefault()

                    If venditaSuDatabase Is Nothing Then
                        'Sto inserendo
                        objUmaVendite.Id_Vendite = handleSequenze.NuovoId_Tabella_EF(GiasContext,
                                            "UMA_Vendite", 0, 2000000000, objParametri)

                        If Not objUmaVendite.Data_Creazione.HasValue Then
                            objUmaVendite.Data_Creazione = DateTime.Now
                            objUmaVendite.Data_Modifica = DateTime.Now
                        End If
                        If Not objUmaVendite.Validita_Inizio.HasValue Then
                            objUmaVendite.Validita_Inizio = AGRODATAINIZIO
                        End If
                        If Not objUmaVendite.Validita_Fine.HasValue Then
                            objUmaVendite.Validita_Fine = AGRODATAFINE
                        End If
                        If objUmaVendite.Username_Creazione.Length = 0 Then
                            objUmaVendite.Username_Creazione = objParametri.UtenteUsername
                            objUmaVendite.Username_Modifica = objParametri.UtenteUsername
                        End If
                        If Not objUmaVendite.inviato.HasValue Then
                            objUmaVendite.inviato = 0
                        End If
                        'Non controllo il campo "datainvio" perché può essere inserito a NULL

                        umaVenditeLog = objUmaVendite
                        GiasContext.UMA_Vendite.Add(objUmaVendite)
                    Else
                        'Sono in modifica
                        Dim dataModifica = IIf(objUmaVendite.Data_Modifica.HasValue, objUmaVendite.Data_Modifica.Value, DateTime.Now)
                        Dim usernameModifica = IIf(objUmaVendite.Username_Modifica.Length <> 0, objUmaVendite.Username_Modifica, objParametri.UtenteUsername)
                        venditaSuDatabase.Data_Modifica = dataModifica
                        venditaSuDatabase.Username_Modifica = usernameModifica

                        If objUmaVendite.Validita_Inizio.HasValue Then
                            venditaSuDatabase.Validita_Inizio = objUmaVendite.Validita_Inizio
                        End If
                        If objUmaVendite.Validita_Fine.HasValue Then
                            venditaSuDatabase.Validita_Fine = objUmaVendite.Validita_Fine
                        End If

                        If objUmaVendite.inviato.HasValue Then
                            venditaSuDatabase.inviato = objUmaVendite.inviato
                        End If
                        If objUmaVendite.datainvio.HasValue Then
                            venditaSuDatabase.datainvio = objUmaVendite.datainvio
                        End If

                        venditaSuDatabase.Tipo_Carburante = objUmaVendite.Tipo_Carburante
                        venditaSuDatabase.Lt = objUmaVendite.Lt
                        venditaSuDatabase.Tipo_Documento = objUmaVendite.Tipo_Documento
                        venditaSuDatabase.Note_Rivenditore = objUmaVendite.Note_Rivenditore

                        umaVenditeLog = venditaSuDatabase
                        GiasContext.Entry(venditaSuDatabase).State = Entity.EntityState.Modified
                    End If

                    GiasContext.SaveChanges()
                Next

                ' COMMIT Effettivo
                scope.Complete()

            Catch ex As Exception
                ' Rollback
                scope.Dispose()

                messaggioErrore = String.Format(
                    "Errore inserimento vendita Piva cliente {0}, Dati ddt {1} - {2}: {3}",
                    umaVenditeLog.PIVA_Cliente,
                    umaVenditeLog.Nr_Documento,
                    umaVenditeLog.Data_Documento.ToString("d"),
                    ex.Message)
            Finally
                If GiasContext.Database.Connection.State = ConnectionState.Open Then
                    GiasContext.Database.Connection.Close()
                End If
            End Try

        End Using

        Return messaggioErrore
    End Function

    Public Function Scrivi_vendite(ByRef listaVendite As List(Of UMA_VenditeToRow), ByRef objParametri As AgronicaCoreParametri) As String
        Dim NomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Vendite_W.AggiungiNouvi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim PivaSuperUser = objParametri.PivaSuperUser
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As Gias_DeveloperServer_Entities = Nothing
        Dim transactionOptions = New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted

        Using scope As New TransactionScope(TransactionScopeOption.Required, TransactionOptions)
            Try
                GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
                GiasContext.Database.Connection.Open()

                Dim handleSequenze = New Agro_Sequenze()

                For Each l In listaVendite

                    l.Id_Vendite = handleSequenze.NuovoId_Tabella_EF(GiasContext,
                                                                     "UMA_Vendite",
                                                                     0,
                                                                     2000000000,
                                                                     objParametri)

                    StrSQL = New System.Text.StringBuilder

                    StrSQL.Append("INSERT INTO UMA_Vendite ( ")
                    StrSQL.Append(" Id_Vendite, ")
                    StrSQL.Append(" PivaSuperUser, ")
                    StrSQL.Append(" PIVA_Venditore, ")
                    StrSQL.Append(" PIVA_Cliente, ")
                    StrSQL.Append(" Anno, ")
                    StrSQL.Append(" Conto_Proprio_Terzi, ")
                    StrSQL.Append(" Tipo_Carburante, ")
                    StrSQL.Append(" Lt, ")
                    StrSQL.Append(" Tipo_Documento, ")
                    StrSQL.Append(" Data_Documento, ")
                    StrSQL.Append(" Nr_Documento, ")
                    StrSQL.Append(" inviato, ")
                    StrSQL.Append(" datainvio, ")
                    StrSQL.Append(" Data_Creazione, ")
                    StrSQL.Append(" Username_Creazione, ")
                    StrSQL.Append(" Data_Modifica, ")
                    StrSQL.Append(" Username_Modifica, ")
                    StrSQL.Append(" Validita_Inizio, ")
                    StrSQL.Append(" Validita_Fine, ")
                    StrSQL.Append(" Note_Rivenditore, ")
                    StrSQL.Append(" Richiesta_Cod, ")
                    StrSQL.Append(" Origine_Movimento) ")

                    StrSQL.Append("VALUES (")
                    StrSQL.Append("     " & Agro_SQL_SaveNum(l.Id_Vendite) & " ")
                    StrSQL.Append("  , '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.Append("  , '" & Agro_SQL_SaveText(l.PIVA_Venditore) & "' ")
                    StrSQL.Append("  , '" & Agro_SQL_SaveText(l.PIVA_Cliente) & "' ")
                    StrSQL.Append("  ,  " & Agro_SQL_SaveNum(l.Anno) & " ")
                    StrSQL.Append("  , '" & Agro_SQL_SaveText(l.Conto_Proprio_Terzi) & "' ")
                    StrSQL.Append("  ,  " & Agro_SQL_SaveNum(l.Tipo_Carburante) & " ")
                    StrSQL.Append("  ,  " & Agro_SQL_SaveNum(l.Lt) & " ")
                    StrSQL.Append("  ,  " & Agro_SQL_SaveNum(l.Tipo_Documento) & " ")
                    StrSQL.Append("  ,  " & Agro_SQL_SaveDate(l.Data_Documento) & " ")
                    StrSQL.Append("  , '" & Agro_SQL_SaveText(l.Nr_Documento) & "' ")
                    StrSQL.Append("  , 0 ")
                    StrSQL.Append("  ,  GETDATE() ")
                    StrSQL.Append("  ,  GETDATE() ")
                    StrSQL.Append("  , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                    StrSQL.Append("  ,  GETDATE() ")
                    StrSQL.Append("  , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                    StrSQL.Append("  ,  " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
                    StrSQL.Append("  ,  " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
                    StrSQL.Append("  , '" & Agro_SQL_SaveText(l.Note_Rivenditore) & "' ")
                    StrSQL.Append("  ,  " & Agro_SQL_SaveNum(l.Richiesta_Cod) & " ")
                    StrSQL.Append("  ,  " & Agro_SQL_SaveNum(l.Origine_Movimento) & " ")
                    StrSQL.Append(" )")

                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

                Next

                ' COMMIT Effettivo
                scope.Complete()

            Catch ex As Exception
                ' Rollback
                scope.Dispose()
                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            End Try

        End Using

        Return MessaggioErrore

    End Function

    Public Function Delete_Vendite_UMA(ByRef Richiesta_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Vendite_W.Delete_Vendite_UMA()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim PivaSuperUser = objParametri.PivaSuperUser
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As Gias_DeveloperServer_Entities = Nothing
        Dim transactionOptions = New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
            Try
                GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
                GiasContext.Database.Connection.Open()

                Dim handleSequenze = New Agro_Sequenze()



                StrSQL = New System.Text.StringBuilder

                StrSQL.Append("Delete from UMA_Vendite ")
                StrSQL.Append(" where ")
                StrSQL.Append(" PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" and Richiesta_Cod = " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")

                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

                ' COMMIT Effettivo
                scope.Complete()

            Catch ex As Exception
                ' Rollback
                scope.Dispose()
                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            End Try
        End Using

        Return MessaggioErrore
    End Function
End Class


Partial Public Class UMA_VenditeToRow

    Public Property Id_Vendite As Integer
    Public Property PivaSuperUser As String
    Public Property PIVA_Venditore As String
    Public Property PIVA_Cliente As String
    Public Property Anno As Integer
    Public Property Conto_Proprio_Terzi As Integer
    Public Property Tipo_Carburante As Integer
    Public Property Lt As Double

    Public Property Tipo_Documento As Integer

    Public Property Data_Documento As Date

    Public Property Nr_Documento As String

    Public Property inviato As Nullable(Of Short)

    Public Property datainvio As Nullable(Of Date)

    Public Property Data_Creazione As Nullable(Of Date)

    Public Property Data_Modifica As Nullable(Of Date)

    Public Property Username_Creazione As String

    Public Property Username_Modifica As String

    Public Property Validita_Inizio As Nullable(Of Date)

    Public Property Validita_Fine As Nullable(Of Date)

    Public Property Note_Rivenditore As String

    Public Property Richiesta_Cod As Integer
    Public Property Origine_Movimento As Integer


End Class