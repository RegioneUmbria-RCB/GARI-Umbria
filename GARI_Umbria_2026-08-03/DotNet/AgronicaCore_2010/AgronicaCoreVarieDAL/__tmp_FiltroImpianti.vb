



Imports System.Data.SqlClient
Imports Newtonsoft.Json

Public Class __tmp_FiltroImpianti_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function VerificaEsistenzaCampoDataCreazione(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim NomeRoutine As String = "VerificaEsistenzaCampoDataCreazione()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim dt As DataTable

        Try



            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine(" Select 1 from sys.tables tt  ")
            Stb.AppendLine(" inner Join sys.columns cc on tt.object_id = cc.object_id  ")
            Stb.AppendLine(" where tt.name = '__tmp_FiltroImpianti'  ")
            Stb.AppendLine(" And cc.name = 'Data_Creazione' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count > 0 Then
                xRisp = True
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function ConteggioRecordDistinctPivaDaIDTestataTemp(ByVal IDTestataTemp As Integer, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "ConteggioRecordDistinctPivaDaIDTestataTemp"
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" Select distinct piva  ")
            stb.AppendLine(" From __tmp_FiltroImpianti ")
            stb.AppendLine(" Where IDTestataTemp = " & IDTestataTemp)



            '-----------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------


        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Function LeggiElencoDaIDTestatTemp(ByVal IDTestataTemp As Integer, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "LeggiElencoDaIDTestatTemp"
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" Select *  ")
            stb.AppendLine(" From __tmp_FiltroImpianti ")
            stb.AppendLine(" Where IDTestataTemp = " & IDTestataTemp)

            '-----------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function VerificaStampaMultiCentro(ByVal IDTestataTemp As Integer, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "VerificaStampaMultiCentro"
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim isStampaMultiCentro As Boolean = True
        Try

            stb.Length = 0

            stb.AppendLine(" SELECT DISTINCT Piva, Sa_Cod  ")
            stb.AppendLine(" FROM __tmp_FiltroImpianti ")
            stb.AppendLine($" WHERE IDTestataTemp =  {IDTestataTemp}")

            '-----------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------

            If DT IsNot Nothing AndAlso DT.Rows.Count = 1 Then
                isStampaMultiCentro = False
            End If

            DT = Nothing

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)

            'Non lancio eccezzione, ma ritorno isStampaMultiCentro = true, così da tenere il comportamento pre-modifiche
            'Throw New Exception(MessaggioErrore)

        End Try

        Return isStampaMultiCentro

    End Function
    Public Function LeggiElencoDaUsername(ByVal username As String, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "LeggiElencoDaUsername"
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" Select *  ")
            stb.AppendLine(" From __tmp_FiltroImpianti ")
            stb.AppendLine(" Where IDTestataTemp = ")
            stb.AppendLine("(Select MAX(idTestataTemp) from __tmp_FiltroImpianti where username_creazione = '" & username & "')")

            '-----------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT

    End Function

End Class


'#################################################################
'#################################################################
'#################################################################

Public Class __tmp_FiltroImpianti_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Sub Popola__tmp_FiltroImpianti_Da_Lista(ByVal listaImpianti As List(Of TmpFiltroImpiantiObject),
                                                    ByRef IDTestataTemp As Integer,
                                                    ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            If Not IsNothing(listaImpianti) AndAlso listaImpianti.Any Then

                If IDTestataTemp <= 0 Then
                    Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                    'Idtestata = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri_Server)
                    'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                    IDTestataTemp = xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)

                    For Each impianto As TmpFiltroImpiantiObject In listaImpianti
                        impianto.IdTestataTemp = IDTestataTemp
                    Next
                End If

                Dim jsonData = JsonConvert.SerializeObject(New With {Key .Table = listaImpianti})
                Dim ds As DataSet = JsonConvert.DeserializeObject(Of DataSet)(jsonData)

                Using sqlBulkCopy As New SqlBulkCopy(objParametri_Server.objConnessione, Nothing, objParametri_Server.objTransazione)
                    sqlBulkCopy.DestinationTableName = "__tmp_FiltroImpianti"
                    sqlBulkCopy.ColumnMappings.Add("Piva", "piva")
                    sqlBulkCopy.ColumnMappings.Add("Sa_Cod", "sa_cod")
                    sqlBulkCopy.ColumnMappings.Add("Appezza", "appezza")
                    sqlBulkCopy.ColumnMappings.Add("IdReg", "id_reg")
                    sqlBulkCopy.ColumnMappings.Add("IdTestataTemp", "idTestataTemp")
                    sqlBulkCopy.ColumnMappings.Add("Validita_Inizio", "Validita_Inizio")
                    sqlBulkCopy.ColumnMappings.Add("Validita_Fine", "Validita_Fine")
                    sqlBulkCopy.WriteToServer(ds.Tables(0))
                End Using

            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            'MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & "Popola__tmp_FiltroImpianti" & "] : " & ex.Message)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
        End Try

    End Sub


    '##############################################################################################
    Public Function CancellaVecchiRecordPerDataCreazione(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim NomeRoutine As String = "CancellaVecchiRecordPerDataCreazione()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine(" Delete  ")
            Stb.AppendLine(" From __tmp_FiltroImpianti ")
            Stb.AppendLine(" Where DateDiff(d, Data_Creazione, GETDATE()) > 7")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function


    Public Function CancellaRecordDaUsername(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "CancellaRecordDaUsername()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine(" Delete  ")
            Stb.AppendLine(" From __tmp_FiltroImpianti ")
            Stb.AppendLine(" Where Username_Creazione = '" & objParametri.UtenteUsername & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp


    End Function


    Public Function CancellaRecordDaIDTestataTemp(ByVal IDTestataTemp As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "CancellaRecordDaUsername()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine(" Delete  ")
            Stb.AppendLine(" From __tmp_FiltroImpianti ")
            Stb.AppendLine(" Where IDTestataTemp = " & IDTestataTemp)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp


    End Function

End Class


Public Class TmpFiltroImpiantiObject

    Public Piva As String
    Public Sa_Cod As String
    Public Appezza As String
    Public IdReg As String
    Public IdTestataTemp As Integer
    Public Validita_Inizio As String
    Public Validita_Fine As String
End Class

Public Class TmpFiltroImpiantiObjectComparer
    Implements IEqualityComparer(Of TmpFiltroImpiantiObject)

    Public Function Equals(x As TmpFiltroImpiantiObject, y As TmpFiltroImpiantiObject) As Boolean Implements IEqualityComparer(Of TmpFiltroImpiantiObject).Equals

        ' Check whether the compared objects reference the same data.
        If x Is y Then Return True

        'Check whether any of the compared objects is null.
        If x Is Nothing OrElse y Is Nothing Then Return False

        ' Check whether the products' properties are equal.
        Return (x.Piva = y.Piva) AndAlso (x.Sa_Cod = y.Sa_Cod) AndAlso (x.Appezza = y.Appezza) AndAlso (x.IdReg = y.IdReg)

    End Function

    Public Function GetHashCode(obj As TmpFiltroImpiantiObject) As Integer Implements IEqualityComparer(Of TmpFiltroImpiantiObject).GetHashCode

        If IsNothing(obj) Then Return 0

        Return String.Format("{0}{1}{2}{3}", obj.Piva, obj.Sa_Cod, obj.Appezza, obj.IdReg).GetHashCode

    End Function

End Class