Imports System.Runtime.CompilerServices
Imports AgronicaCoreContabDAL.ws_Canopy_Anagrafica_W
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class ws_Canopy_Anagrafica_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Legge i valori da db ws_Canopy_Anagrafica
    ''' </summary>
    ''' <param name="ID"></param> BlockID
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiCanopy(ByVal ID As Integer,
                               ByRef objParametri As AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.ws_Canopy_Anagrafica.LeggiCanopy()"

        ' ------- Variabili
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append("SELECT * FROM ws_Canopy_Anagrafica")

            If ID <> 0 Then
                StrSQL.AppendLine(" WHERE BlockID = " + Agro_SQL_SaveNum(ID))
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

End Class


Public Class ws_Canopy_Anagrafica_W
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Inserisce le righe in ws_Canopy_Anagrafica con ID progressivo (NuovoId_Tabella_EF())
    ''' </summary>
    Public Function Inserisci(righeInseriteArr As List(Of ws_Canopy_Anagrafica_Dto),
                             context As Gias_DeveloperServer_Entities,
                             ByRef objParametri As AgronicaCoreParametri) As Boolean

        Try
            Dim nuovoID = New AgronicaCoreDataProvider.Agro_Sequenze

            For Each l In righeInseriteArr
                l.ID = nuovoID.NuovoId_Tabella_EF(context, "ws_Canopy_Anagrafica", 0, 2000000000, objParametri)
                context.ws_Canopy_Anagrafica.Add(To_ws_Canopy_Anagrafica_Poco(l))
            Next

            context.SaveChanges()

        Catch ex As Exception
            Dim NomeRoutine As String = "AgronicaCoreContabDAL.ws_Canopy_Anagrafica_W.Inserisci()"
            Dim MessaggioErrore As String

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function


    ' TO DO
    Public Function Modifica(righeModificate As List(Of ws_Canopy_Anagrafica_Dto),
                             context As Gias_DeveloperServer_Entities,
                             ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim DT As DataTable

        Try

            For Each elem In righeModificate

                'per prima cosa cerco in base al block id che è chiave univoca, poi per kpin, block name.
                Dim result As ws_Canopy_Anagrafica = Nothing
                If elem.BlockID > 0 Then
                    result = (From a In context.ws_Canopy_Anagrafica Where a.BlockID = elem.BlockID).FirstOrDefault()
                End If

                If result Is Nothing Then
                    result = (From a In context.ws_Canopy_Anagrafica Where a.Kpin = elem.Kpin And a.BlockName = elem.BlockName).FirstOrDefault()
                End If

                If Not result Is Nothing Then
                    result.BlockID = elem.BlockID

                    If String.IsNullOrEmpty(elem.Username_Creazione) Then

                        If String.IsNullOrEmpty(elem.MessaggiSincroCanopy) Then
                            result.MessaggiSincroCanopy = ""
                        Else
                            result.MessaggiSincroCanopy = elem.MessaggiSincroCanopy
                        End If
                    End If

                    result.Data_Modifica = elem.Data_Modifica
                    result.Username_Modifica = elem.Username_Modifica
                    result.Data_Modifica_Canopy = elem.Data_Modifica_Canopy
                End If
            Next

            context.SaveChanges()

        Catch ex As Exception
            Dim NomeRoutine As String = "AgronicaCoreContabDAL.ws_Canopy_Anagrafica_W.Modifica()"
            Dim MessaggioErrore As String = ""

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True

    End Function
    ' 

    ''' <summary>
    ''' Oggetto BLOCK
    ''' </summary>
    Class ws_Canopy_Anagrafica_Dto

        ' *************************** Collone chiavi primarie ***************************
        Public ID As Integer

        ' ************************* Collone della tabella ws_Canopy_Anagrafica_Dto *************************
        Public BlockID As Integer
        Public Kpin As String
        Public BlockName As String
        Public DescrizioneSalvataggio As String
        Public MessaggiSincroCanopy As String

        Public StatoSincronizzazione As Integer

        Public Validita_Inizio As Date?
        Public Validita_Fine As Date?
        Public Inviato As Short
        Public DataInvio As Date?
        Public Data_Creazione As Date?
        Public Data_Modifica As Date?
        Public Data_Modifica_Canopy As Date?
        Public Username_Creazione As String
        Public Username_Modifica As String

    End Class

    ''' <summary>
    ''' Corrispondenza fra i nomi delle colonne nel db e l'oggetto creato in VB
    ''' </summary>
    ''' <param name="l"></param>
    ''' <returns></returns>
    Function To_ws_Canopy_Anagrafica_Poco(ByVal l As ws_Canopy_Anagrafica_W.ws_Canopy_Anagrafica_Dto) As AgronicaCoreEntityFramework_POCO.ws_Canopy_Anagrafica

        Dim r As New AgronicaCoreEntityFramework_POCO.ws_Canopy_Anagrafica With {
            .ID = l.ID,
            .datainvio = l.DataInvio,
            .Data_Creazione = l.Data_Creazione,
            .Data_Modifica = l.Data_Modifica,
            .Data_Modifica_Canopy = l.Data_Modifica_Canopy,
             .inviato = l.Inviato,
            .BlockID = l.BlockID,
            .Kpin = l.Kpin,
            .BlockName = l.BlockName,
            .DescrizioneSalvataggio = l.DescrizioneSalvataggio,
            .MessaggiSincroCanopy = l.MessaggiSincroCanopy,
            .StatoSincronizzazione = l.StatoSincronizzazione,
            .Username_Creazione = l.Username_Creazione,
            .Username_Modifica = l.Username_Modifica,
            .Validita_Fine = l.Validita_Fine,
            .Validita_Inizio = l.Validita_Inizio
        }

        Return r

    End Function
End Class
