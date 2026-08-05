Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreModello
Public Class CapitolatoClienteXImprese_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal PivaSuperUser As String, ByVal idCapitolato As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXImprese_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine("    [Piva] ")
            StrSQL.AppendLine("	   ,[Capitolato_COD]  ")
            StrSQL.AppendLine("    ,[PivaSuperUser]  ")
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine("     CapitolatoClienteXImprese ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine("     PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.AppendLine("     And Capitolato_COD = " & Agro_SQL_SaveNum(idCapitolato) & " ")

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

End Class

Public Class CapitolatoClienteXImprese_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Private nome_tabella As String = "CapitolatoClienteXImprese"
    Public Function cancella(PivaSuperUser As String, capitolato_cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXImprese_W.Cancella()"
        Dim messaggioErrore As String = ""
        Dim strSql As String = ""
        Dim xRisp As Boolean = False
        Dim sql As String = ""
        Dim properties As New Dictionary(Of String, String)
        Dim parametriInjection As Object = Nothing

        Try
            strSql = "Delete from CapitolatoClienteXImprese "
            strSql += " Where Capitolato_COD = '" + Agro_SQL_SaveText(capitolato_cod.ToString) + "' "
            strSql += " And pivasuperuser = '" + Agro_SQL_SaveText(PivaSuperUser) + "' "
            ' -- chiede i parametri creati e li assegna all'oggetto tornato per referenza
            parametriInjection = DammiParametriCollezionati()

            ' -- setta parametri tornati nell'oggetto base 
            MyBase.SettaParametriPrecedenti(parametriInjection)

            xRisp = EseguiQuery_Scrittura(objParametri, strSql, nomeRoutine)
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" + nomeRoutine & "] : " + messaggioErrore)
        End Try

        Return xRisp
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="CapitolatoClienteXCultivarDataModel"></param>
    ''' <param name="_objParametriServer"></param>
    ''' <returns></returns>
    Public Function Scrivi(CapitolatoClienteXImpreseDataModel As List(Of modelElencoAziende), ByRef _objParametriServer As AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXImprese_W.Scrivi()"
        Dim messaggioErrore As String = ""
        Dim strSql As String = ""
        Dim xRisp As Boolean = False
        Dim sql As String = ""
        Dim properties As New Dictionary(Of String, String)
        Dim parametriInjection As Object = Nothing

        Try

            If CapitolatoClienteXImpreseDataModel IsNot Nothing AndAlso CapitolatoClienteXImpreseDataModel.Count > 0 Then
                For Each lObj As modelElencoAziende In CapitolatoClienteXImpreseDataModel
                    xRisp = False
                    ' -- Crea nuovo oggetto
                    Using dmu As New DatamodelUtils
                        ' -- crea un Dictionary delle proprietà dal modello e un Dictionary dei parametri e crea oggetto parametri 
                        xRisp = dmu.getPropertysOfDataModell(lObj, properties, parametriInjection)
                        ' -- setta parametri tornati nell'oggetto base 
                        MyBase.SettaParametriPrecedenti(parametriInjection)
                        ' -- crea sql inserimento dati 
                        strSql = dmu.getSqlInserisci(nome_tabella, properties)
                        ' -- esegue inserimento dati
                        xRisp = EseguiQuery_Scrittura(_objParametriServer, strSql, nomeRoutine)
                    End Using
                Next
            Else
                xRisp = True
            End If

            If xRisp = False Then
                Throw New Exception("Si è generato un errore.")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(_objParametriServer, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" + nomeRoutine & "] : " + messaggioErrore)
        End Try

        Return xRisp
    End Function

End Class
