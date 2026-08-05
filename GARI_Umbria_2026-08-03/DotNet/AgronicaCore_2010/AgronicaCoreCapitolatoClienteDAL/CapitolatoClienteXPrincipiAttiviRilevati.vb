Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreModello
Public Class CapitolatoClienteXPrincipiAttiviRilevati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
        ByVal codCapitolato As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXPrincipiAttiviRilevati_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Clear()

            StrSQL.AppendLine(" SELECT Capitolato_COD, PA_COD, LMR, inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, PartecipaSommatoria ")
            StrSQL.AppendLine(" FROM CapitolatoClienteXPrincipiAttiviRilevati ")
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
End Class

Public Class CapitolatoClienteXPrincipiAttiviRilevati_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Private nome_tabella As String = "CapitolatoClienteXPrincipiAttiviRilevati"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="CapitolatoClienteXCultivarDataModel"></param>
    ''' <param name="_objParametriServer"></param>
    ''' <returns></returns>
    Public Function Scrivi(CapitolatoClienteXPrincipiAttiviRilevatiDataModel As List(Of CapitolatoClienteXPrincipiAttiviRilevati), ByRef _objParametriServer As AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXPrincipiAttiviRilevati_W.Scrivi()"
        Dim messaggioErrore As String = ""
        Dim strSql As String = ""
        Dim xRisp As Boolean = False
        Dim sql As String = ""
        Dim properties As New Dictionary(Of String, String)
        Dim parametriInjection As Object = Nothing

        Try
            If CapitolatoClienteXPrincipiAttiviRilevatiDataModel IsNot Nothing AndAlso CapitolatoClienteXPrincipiAttiviRilevatiDataModel.Count > 0 Then
                For Each lObj As CapitolatoClienteXPrincipiAttiviRilevati In CapitolatoClienteXPrincipiAttiviRilevatiDataModel
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

    Public Function Cancella(capitolato_cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional partecipaSommatoria As Boolean = False,
                             Optional partecipa As Integer = -1) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXPrincipiAttiviRilevati_W.Cancella()"
        Dim messaggioErrore As String = ""
        Dim strSql As String = ""
        Dim xRisp As Boolean = False
        Dim sql As String = ""
        Dim properties As New Dictionary(Of String, String)
        Dim parametriInjection As Object = Nothing

        Dim LMR = IIf(partecipaSommatoria, "AND LMR <> 0 and partecipaSommatoria = " & partecipa, "AND LMR = 0")

        Try
            strSql = "DELETE FROM CapitolatoClienteXPrincipiAttiviRilevati "
            strSql += " WHERE Capitolato_COD = " + Agro_SQL_SaveNum(capitolato_cod.ToString) + " "
            strSql += LMR



            ' -- chiede i parametri creati e li assegna all'oggetto tornato per referenza
            parametriInjection = DammiParametriCollezionati()

            ' -- setta parametri tornati nell'oggetto base 
            MyBase.SettaParametriPrecedenti(parametriInjection)

            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" + nomeRoutine & "] : " + messaggioErrore)
        End Try

        Return xRisp
    End Function
End Class