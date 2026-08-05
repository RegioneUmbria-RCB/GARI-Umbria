Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreModello
Public Class CapitolatoClienteXCultivar_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
        ByVal codCapitolato As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXCultivar_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Clear()

            StrSQL.AppendLine(" SELECT Capitolato_COD, Cul_COD, inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ")

            StrSQL.AppendLine(" FROM CapitolatoClienteXCultivar ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine("     1=1 ")
            StrSQL.AppendLine("AND Capitolato_COD = " + Agro_SQL_SaveNum(codCapitolato.ToString) + " ")

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

    Public Function LeggiElencoCapitolatoClienteCultivar(ByVal Capitolato_COD As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXCultivar_R.LeggiElencoCapitolatoClienteCultivar()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" Select ccxc.Cul_COD, c.Cul_Des  from CapitolatoClienteXCultivar ccxc ")
            StrSQL.AppendLine(" inner join Cultivar c on c.Cul_Cod = ccxc.Cul_COD ")
            StrSQL.AppendLine(" where ccxc.Capitolato_COD  = " + Agro_SQL_SaveNum(Capitolato_COD.ToString) + " ")
            StrSQL.AppendLine(" Order by c.Cul_Des ")

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
    Public Function LeggiElencoCultivar(ByVal Capitolato_COD As Integer, veg_cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXCultivar_R.LeggiElencoCultivar()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" select Cul_COD, Cul_Des from Cultivar ")
            StrSQL.AppendLine(" where Veg_Cod  = " + Agro_SQL_SaveNum(veg_cod.ToString) + " ")
            StrSQL.AppendLine(" Order by Cul_Des ")

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

Public Class CapitolatoClienteXCultivar_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Private nome_tabella As String = "CapitolatoClienteXCultivar"
    Public Function cancella(capitolato_cod As Integer, cul_cod As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXCultivar_W.Cancella()"
        Dim messaggioErrore As String = ""
        Dim strSql As String = ""
        Dim xRisp As Boolean = False
        Dim sql As String = ""
        Dim properties As New Dictionary(Of String, String)
        Dim parametriInjection As Object = Nothing

        Try
            strSql = "Delete from CapitolatoClienteXCultivar "
            strSql += " Where Capitolato_COD = " + Agro_SQL_SaveNum(capitolato_cod.ToString) + " "
            If Not (cul_cod.Equals("")) Then
                strSql += " And cul_COD = '" + Agro_SQL_SaveText(cul_cod) + " "
            End If
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
    Public Function Scrivi(CapitolatoClienteXCultivarDataModel As List(Of CapitolatoClienteXCultivar), ByRef _objParametriServer As AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXCultivar_W.Scrivi()"
        Dim messaggioErrore As String = ""
        Dim strSql As String = ""
        Dim xRisp As Boolean = False
        Dim sql As String = ""
        Dim properties As New Dictionary(Of String, String)
        Dim parametriInjection As Object = Nothing

        Try
            If CapitolatoClienteXCultivarDataModel IsNot Nothing AndAlso CapitolatoClienteXCultivarDataModel.Count > 0 Then
                For Each lObj As CapitolatoClienteXCultivar In CapitolatoClienteXCultivarDataModel
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
