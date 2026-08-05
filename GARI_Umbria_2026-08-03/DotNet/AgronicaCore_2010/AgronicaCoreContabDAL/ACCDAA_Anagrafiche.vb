Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework_POCO

Public Class CodiciOriginiSpedione_R : Inherits AgronicaCoreDataProvider.DataProvider

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi(Optional ByVal codice As Integer = 0) As List(Of ACCDAA_ANAG_T017_TabellaCodiciOrigineDellaSpedizione)

        Dim nomeProcedura = "CodiciOriginiSpedione_R.Leggi"

        Try

            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim qry = From t In dal.ACCDAA_ANAG_T017_TabellaCodiciOrigineDellaSpedizione
            If codice <> 0 Then
                qry = qry.Where(Function(q) q.Codice.Equals(codice))
            End If

            Return qry.ToList()

        Catch ex As Exception

            Scrivi_LOG(_objParametriServer, nomeProcedura, ex.Message)

            Dim messaggio = ex.Message.ToString & If(Not IsNothing(ex.InnerException), " (" & ex.InnerException.Message.ToString & ")", "")
            Throw New Exception(String.Format("{0} : {1}{2}", nomeProcedura, messaggio, "</br>"))

        End Try

    End Function

End Class

Public Class TipologiaDestinazione_R : Inherits AgronicaCoreDataProvider.DataProvider

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi(Optional ByVal codice As Integer = 0) As List(Of ACCDAA_ANAG_T015_TabellaCodiciTipoDestinazione)

        Dim nomeProcedura = "TipologiaDestinazione_R.Leggi"

        Try

            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim qry = From t In dal.ACCDAA_ANAG_T015_TabellaCodiciTipoDestinazione
            If codice <> 0 Then
                qry = qry.Where(Function(q) q.Codice.Equals(codice))
            End If

            Return qry.ToList()

        Catch ex As Exception

            Scrivi_LOG(_objParametriServer, nomeProcedura, ex.Message)

            Dim messaggio = ex.Message.ToString & If(Not IsNothing(ex.InnerException), " (" & ex.InnerException.Message.ToString & ")", "")
            Throw New Exception(String.Format("{0} : {1}{2}", nomeProcedura, messaggio, "</br>"))

        End Try

    End Function

End Class

Public Class UfficiDA_R : Inherits AgronicaCoreDataProvider.DataProvider

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi(Optional ByVal codice As Integer = 0) As List(Of ACCDAA_ANAG_TA03_UfficiDA)

        Dim nomeProcedura = "UfficiDA_R.Leggi"

        Try

            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim qry = From t In dal.ACCDAA_ANAG_TA03_UfficiDA
            If codice <> 0 Then
                qry = qry.Where(Function(q) q.Codice.Equals(codice))
            End If

            Return qry.ToList()

        Catch ex As Exception

            Scrivi_LOG(_objParametriServer, nomeProcedura, ex.Message)

            Dim messaggio = ex.Message.ToString & If(Not IsNothing(ex.InnerException), " (" & ex.InnerException.Message.ToString & ")", "")
            Throw New Exception(String.Format("{0} : {1}{2}", nomeProcedura, messaggio, "</br>"))

        End Try

    End Function


End Class



Public Class SigleAE_R : Inherits AgronicaCoreDataProvider.DataProvider

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub


    '###############################################################################
    Public Function LeggiSigleAE(
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.UfficiDA_R.LeggiSigleAE()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------


        Try

            StrSQL.Length = 0
            StrSQL.Append("Select *, (SiglaAE + ' ' + Descrizione) as xDescrizione  " + vbCrLf)
            StrSQL.Append(" From TipologieDocumento " + vbCrLf)
            StrSQL.Append(" Where 1 = 1 " + vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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





