Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class MVVStampaDAL


    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtente As AgronicaCoreParametri

    Private ReadOnly _mvv_R As MVV_R
    Private ReadOnly _teleRegistriDettagli_R As TeleRegistri_Dettagli_R
    Private ReadOnly _Stati_R As Stati_Membri_R
    Private ReadOnly _codificaVino_R As Codifica_Vino_R
    Private ReadOnly _moduloGen_R As ModuloGenerazione_R
    Private ReadOnly _MovimentiDettagli_R As MovimentiDettagli_R
    Private ReadOnly _descrizioneBeniServizi_R As DescrizioneBeniServizi_R

    Public Sub New(
                  ByVal objParametriServer As AgronicaCoreParametri,
                  ByVal objParametriUtente As AgronicaCoreParametri
        )

        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
        Dim giasContext As New Gias_DeveloperServer_Entities(efConnString)

        _mvv_R = New MVV_R(_objParametriServer)
        _teleRegistriDettagli_R = New TeleRegistri_Dettagli_R(giasContext)
        _Stati_R = New Stati_Membri_R(giasContext)
        _codificaVino_R = New Codifica_Vino_R(giasContext)
        _moduloGen_R = New ModuloGenerazione_R(_objParametriServer)
        _MovimentiDettagli_R = New MovimentiDettagli_R(_objParametriServer)
        _descrizioneBeniServizi_R = New DescrizioneBeniServizi_R(_objParametriServer, _objParametriUtente)

    End Sub


    Public Function PreparaDatiStampa(ByVal Piva As String, ByVal id_Agenda As Integer) As Object

        Return New With
        {
            .DatiMVV = _mvv_R.LeggiMVV(Piva, id_Agenda),
            .DettagliTeleregistri = _teleRegistriDettagli_R.Leggi(),
            .StatiMembri = _Stati_R.Leggi(),
            .CodificheVino = _codificaVino_R.Leggi(),
            .ModuloGenerazione = _moduloGen_R.Leggi(),
            .MovimentiDettagli = _MovimentiDettagli_R.Leggi(Piva, id_Agenda)
        }

    End Function

    Public Function LeggiDescrizioneBeneServizio(ByVal movimento As Movimenti_dettagli,
                                                ByRef flag_extra As Boolean,
                                                ByVal modulo_generazione As enum_Omni_Modulo_Generazione) As String

        Return _descrizioneBeniServizi_R.Leggi(movimento, flag_extra, modulo_generazione)

    End Function

End Class

Public Class Materie_PrimexLotto_Proprieta_R : Inherits DALBase

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi(ByVal piva As String, ByVal id_Agenda As Integer) As DataTable

        Dim Stb As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim nomeProcedura As String = "Materie_PrimexLotto_Proprieta_R.Leggi"

        Try

            Stb.AppendLine(" Select md.Mat_Cod, md.Lotto, lp.Id_Proprieta, lp.Proprieta_Val ")
            Stb.AppendLine(" From Movimenti_dettagli md ")
            Stb.AppendLine(" Left Join Materie_PrimexLotto_Proprieta lp ")
            Stb.AppendLine(" On md.PIVA = lp.Piva ")
            Stb.AppendLine(" And md.Mat_Cod = lp.Mat_Cod ")
            Stb.AppendLine(" WHERE md.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            Stb.AppendLine(" AND md.Id_Agenda = " & Agro_SQL_SaveNum(id_Agenda) & " ")

            dt = EseguiQuery_Lettura(_objParametriServer, Stb.ToString, nomeProcedura)

            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

    End Function

End Class


Public Class MovimentiDettagli_R : Inherits DALBase

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi(ByVal piva As String, ByVal id_Agenda As Integer) As List(Of Movimenti_dettagli)

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Return (From md In dal.Movimenti_dettagli Where md.PIVA.Equals(piva) AndAlso md.Id_Agenda.Equals(id_Agenda)).ToList()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "MovimentiDettagli_R.Leggi")
        End Try

    End Function

End Class

Public Class ModuloGenerazione_R : Inherits DALBase

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi() As enum_Omni_Modulo_Generazione

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim modulo = (From mg In dal.OGenerazioni_Anagrafe_Moduli_Log).FirstOrDefault()

            If Not modulo Is Nothing Then
                Return modulo.Modulo_Generazione
            Else
                Return enum_Omni_Modulo_Generazione.Nessuno
            End If
        Catch ex As Exception
            Throw RaiseDAlException(ex, "ModuloGenerazione_R.Leggi")
        End Try

    End Function

End Class

Public Class Codifica_Vino_R : Inherits DALBase

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities)
        _giasContext = giasContext
    End Sub

    Public Function Leggi() As List(Of AGEACodificaVini)

        Dim nomeProcedura As String = "Codifica_Vino_R.Leggi"

        Try

            Return (From cv In _giasContext.AGEACodificaVini).ToList()

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

    End Function

End Class

Public Class MVV_R : Inherits DALBase

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities)
        _giasContext = giasContext
    End Sub

    Public Function LeggiMVV(ByVal Piva As String, ByVal id_Agenda As Integer) As DataTable

        Dim Stb As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim nomeProcedura As String = "MVV_R.LeggiMVV"

        Try

            Stb.Append("Select * from GiasMVV where Id_agenda = " + id_Agenda.ToString() + " And Piva = '" & Agro_SQL_SaveText(Piva) & "' ")


            dt = EseguiQuery_Lettura(_objParametriServer, Stb.ToString, nomeProcedura)

            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

    End Function

End Class

Public Class Stati_Membri_R : Inherits DALBase

    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities

    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities)
        _giasContext = giasContext
    End Sub

    Public Function Leggi() As List(Of ACCDAA_ANAG_T004_TabellaCodiciStatiMembri)

        Try

            Dim result = From tr In _giasContext.ACCDAA_ANAG_T004_TabellaCodiciStatiMembri
            Return result.ToList()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "Stati_Membri_R.Leggi")
        End Try

    End Function

End Class

Public Class TeleRegistri_Dettagli_R : Inherits DALBase

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities)
        _giasContext = giasContext
    End Sub
    Public Function Leggi() As List(Of TeleRegistri_Dettagli)

        Try

            'todo validita date
            Dim result = From tr In _giasContext.TeleRegistri_Dettagli

            Return result.ToList()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "TeleRegistri_Dettagli_R.Leggi")
        End Try

    End Function

End Class