
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports System.Transactions
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Magazzino_DAL_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '################################################################################

    Public Function LeggiDestinazioneAccettazioneDefault(ByVal Piva As String,
                                                         ByVal Sa_Cod As Integer,
                                                         ByVal Modulo_Generazione As Integer,
                                                         ByVal Mat_Cod As Integer,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                         ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Magazzino_DAL_R.LeggiDestinazioneAccettazioneDefault()"

        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.Append(" SELECT Top 1 Linee_Preparazioni_Dettagli.* FROM ")
            stbSql.Append(" Linee_Classi_Produzioni, Linee_Produzioni, Linee_ProduzionixPreparazioni, Linee_Preparazioni, Linee_Preparazioni_Dettagli, OGenerazioni_Anagrafe_Log ")
            stbSql.Append(" WHERE Linee_Classi_Produzioni.Piva = '" + Agro_SQL_SaveText(Piva) + "' " + vbCrLf)
            stbSql.Append(" And   Linee_Classi_Produzioni.Piva = Linee_Produzioni.Piva ")
            stbSql.Append(" And   Linee_Classi_Produzioni.Linea_Classe_Cod = Linee_Produzioni.Linea_Classe_Cod ")
            stbSql.Append(" And   Linee_Produzioni.Piva = OGenerazioni_Anagrafe_Log.Piva ")
            stbSql.Append(" And   Linee_Produzioni.Linea_Cod = OGenerazioni_Anagrafe_Log.Linea_Cod ")
            stbSql.Append(" And   Linee_Preparazioni_Dettagli.Mat_Cod = OGenerazioni_Anagrafe_Log.Mat_Cod ")
            stbSql.Append(" And   OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & "   ")




            stbSql.Append(" And Linee_Produzioni.Piva = Linee_ProduzionixPreparazioni.Piva ")
            stbSql.Append(" And Linee_Produzioni.Linea_Cod = Linee_ProduzionixPreparazioni.Linea_Cod ")

            stbSql.Append(" And Linee_Preparazioni.Piva = Linee_ProduzionixPreparazioni.Piva ")
            stbSql.Append(" And Linee_Preparazioni.Preparazione_Cod = Linee_ProduzionixPreparazioni.Preparazione_Cod ")

            stbSql.Append(" And Linee_Preparazioni.Piva = Linee_Preparazioni_Dettagli.Piva ")
            stbSql.Append(" And Linee_Preparazioni.Preparazione_Cod = Linee_Preparazioni_Dettagli.Preparazione_Cod ")

            stbSql.Append(" And Linee_Preparazioni.Tipo_Esclusione = 2 ")
            stbSql.Append(" And Linee_Preparazioni_Dettagli.Cau_Mov = '7300' ")
            stbSql.Append(" And Linee_Preparazioni_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")

            If Sa_Cod <> 0 Then
                stbSql.Append(" AND Linee_Classi_Produzioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
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
