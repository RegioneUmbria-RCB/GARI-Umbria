

Imports System
    Imports System.Collections.Generic
    Imports System.Runtime.CompilerServices
    Imports System.Text
    Imports AgronicaCoreDataProvider
    Imports AgronicaCoreDataProvider.CostantiPersonalizzate
    Imports AgronicaCoreDataProvider.TipiEnumerativi
    Imports AgronicaCoreEntityFramework
    Imports AgronicaCoreEntityFramework_POCO
    Imports Newtonsoft.Json
Public Class UMA_Richieste_Restituzioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String,
                          ByVal Richiesta_Cod As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Restituzioni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT RR.*,venditore.rag_soc as Distributore_Des ")
            stb.AppendLine(" FROM UMA_Richieste_Restituzioni RR ")
            stb.AppendLine("  JOIN Imprese venditore ON RR.Piva_Distributore = venditore.PIVA ")
            stb.AppendLine($" WHERE RR.Piva_SuperUser = '{Agro_SQL_SaveText(objParametri.PivaSuperUser)}' ")
            stb.AppendLine($" AND RR.Piva = '{Agro_SQL_SaveText(piva)}' ")
            stb.AppendLine($" AND RR.Richiesta_Cod = {Agro_SQL_SaveNum(Richiesta_Cod)} ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiTotali(ByVal piva As String,
                                ByVal Richiesta_Cod As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Restituzioni_R.LeggiTotali()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT ")
            stb.AppendLine("   isnull(sum(gasolio),0) Gasolio ")
            stb.AppendLine(" , isnull(sum(benzina),0) Benzina ")
            stb.AppendLine(" , isnull(sum(gasolio_serra),0) Gasolio_Serra ")
            stb.AppendLine(" , isnull(sum(confermato_gasolio),0) Confermato_Gasolio ")
            stb.AppendLine(" , isnull(sum(confermato_benzina),0) Confermato_Benzina ")
            stb.AppendLine(" , isnull(sum(confermato_gasolio_serra),0) Confermato_Gasolio_Serra ")
            stb.AppendLine(" FROM UMA_Richieste_Restituzioni ")
            stb.AppendLine($" WHERE Piva_SuperUser = '{Agro_SQL_SaveText(objParametri.PivaSuperUser)}' ")
            stb.AppendLine($" AND Piva = '{Agro_SQL_SaveText(piva)}' ")
            stb.AppendLine($" AND Richiesta_Cod = {Agro_SQL_SaveNum(Richiesta_Cod)} ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
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

Public Class UMA_Richieste_Restituzioni_W
    Inherits DataProvider

    Public Function AggiungiNuovi(listLav As List(Of RichiesteRestituzioni),
                                  objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Restituzioni.AggiungiNouvi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try
            For Each l In listLav

                StrSQL = New System.Text.StringBuilder

                StrSQL.Append("INSERT INTO UMA_Richieste_Restituzioni ( ")
                StrSQL.Append("       Piva_SuperUser                ,  ")
                StrSQL.Append("       Piva                          ,  ")
                StrSQL.Append("       Richiesta_Cod                 ,  ")
                StrSQL.Append("       Piva_Distributore             ,  ")
                StrSQL.Append("       Gasolio                       ,  ")
                StrSQL.Append("       Benzina                       ,  ")
                StrSQL.Append("       Gasolio_Serra                 ,  ")
                StrSQL.Append("       Data_Trasferimento            ,  ")
                StrSQL.Append("       Confermato_Gasolio            ,  ")
                StrSQL.Append("       Confermato_Benzina            ,  ")
                StrSQL.Append("       Confermato_Gasolio_Serra      ,  ")
                StrSQL.Append("       inviato                       ,  ")
                StrSQL.Append("       datainvio                     ,  ")
                StrSQL.Append("       Data_Creazione                ,  ")
                StrSQL.Append("       Username_Creazione            ,  ")
                StrSQL.Append("       Data_Modifica                 ,  ")
                StrSQL.Append("       Username_Modifica             ,  ")
                StrSQL.Append("       Validita_Inizio               ,  ")
                StrSQL.Append("       Validita_Fine                 )  ")

                StrSQL.Append("VALUES (")
                StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append("  ,       '" & Agro_SQL_SaveText(l.Piva) & "' ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.Richiesta_Cod) & " ")
                StrSQL.Append("  ,       '" & Agro_SQL_SaveText(l.Piva_Distributore) & "' ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.Gasolio) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.Benzina) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.Gasolio_Serra) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveDate(l.Data_Trasferimento) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.Confermato_Gasolio) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.Confermato_Benzina) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.Confermato_Gasolio_Serra) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.inviato) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.datainvio) & " ")
                StrSQL.Append("  ,       GETDATE()")
                StrSQL.Append("  ,       '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("  ,       GETDATE()")
                StrSQL.Append("  ,       '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
                StrSQL.Append(" )")

                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True

    End Function

    Public Function Aggiorna(listLav As List(Of RichiesteRestituzioni),
                                  objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Restituzioni.Aggiorna()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try
            For Each l In listLav

                StrSQL = New System.Text.StringBuilder

                StrSQL.Append("UPDATE UMA_Richieste_Restituzioni SET ")
                StrSQL.Append("        Piva_Distributore        = '" & Agro_SQL_SaveText(l.Piva_Distributore) & "' ")
                StrSQL.Append(",       Gasolio                  =  " & Agro_SQL_SaveNum(l.Gasolio) & " ")
                StrSQL.Append(",       Benzina                  =  " & Agro_SQL_SaveNum(l.Benzina) & " ")
                StrSQL.Append(",       Gasolio_Serra            =  " & Agro_SQL_SaveNum(l.Gasolio_Serra) & " ")
                StrSQL.Append(",       Data_Trasferimento       =  " & Agro_SQL_SaveDate(l.Data_Trasferimento) & " ")
                StrSQL.Append(",       Confermato_Gasolio       =  " & Agro_SQL_SaveNum(l.Confermato_Gasolio) & " ")
                StrSQL.Append(",       Confermato_Benzina       =  " & Agro_SQL_SaveNum(l.Confermato_Benzina) & " ")
                StrSQL.Append(",       Confermato_Gasolio_Serra =  " & Agro_SQL_SaveNum(l.Confermato_Gasolio_Serra) & " ")
                StrSQL.Append(",       inviato                  =  " & Agro_SQL_SaveNum(l.inviato) & " ")
                StrSQL.Append(",       datainvio                =  " & Agro_SQL_SaveNum(l.datainvio) & " ")
                StrSQL.Append(",       Data_Modifica            =  GETDATE() ")
                StrSQL.Append(",       Username_Modifica        = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(",       Validita_Inizio          =  " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
                StrSQL.Append(",       Validita_Fine            =  " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
                StrSQL.Append(" WHERE  Piva_SuperUser           = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" and    Piva                     = '" & Agro_SQL_SaveText(l.Piva) & "' ")
                StrSQL.Append(" and    Richiesta_Cod            =  " & Agro_SQL_SaveNum(l.Richiesta_Cod) & " ")
                StrSQL.Append(" and    Piva_Distributore        = '" & Agro_SQL_SaveText(l.Piva_Distributore) & "' ")

                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True

    End Function

    Public Function Azzera_Confermato(ByVal piva As String,
                                      ByVal richiestaCod As Integer,
                                      objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Restituzioni.Azzera_Confermato()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try

            StrSQL = New System.Text.StringBuilder

            StrSQL.Append("UPDATE UMA_Richieste_Restituzioni SET ")
            StrSQL.Append("  Confermato_Gasolio       = 0 ")
            StrSQL.Append(", Confermato_Benzina       = 0 ")
            StrSQL.Append(", Confermato_Gasolio_Serra = 0 ")
            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" and   Piva           = '" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.Append(" and   Richiesta_Cod  = " & Agro_SQL_SaveNum(richiestaCod) & " ")

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True

    End Function

    Public Function Rimuovi(listLav As List(Of RichiesteRestituzioni),
                                  objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Restituzioni.Rimuovi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim PivaSuperUser = objParametri.PivaSuperUser


        Try
            For Each l In listLav

                StrSQL = New System.Text.StringBuilder

                StrSQL.Append("DELETE FROM  UMA_Richieste_Restituzioni ")
                StrSQL.Append(" WHERE ")
                StrSQL.Append("           Piva_SuperUser                =        '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append("  and         Piva                          =        '" & Agro_SQL_SaveText(l.Piva) & "' ")
                StrSQL.Append("  and         Richiesta_Cod                 =        " & Agro_SQL_SaveNum(l.Richiesta_Cod) & " ")
                StrSQL.Append("  and         Piva_Distributore                          =        '" & Agro_SQL_SaveText(l.Piva_Distributore) & "' ")



                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function


    Class RichiesteRestituzioni

        Public Property Piva_SuperUser As String
        Public Property Piva As String
        Public Property Richiesta_Cod As Integer
        Public Property Piva_Distributore As String
        Public Property Gasolio As Single
        Public Property Benzina As Single
        Public Property Gasolio_Serra As Single
        Public Property Data_Trasferimento As DateTime
        Public Property Confermato_Gasolio As Single
        Public Property Confermato_Benzina As Single
        Public Property Confermato_Gasolio_Serra As Single
        Public Property inviato As Short
        Public Property datainvio As DateTime
        Public Property Data_Creazione As DateTime
        Public Property Data_Modifica As DateTime
        Public Property Username_Creazione As String
        Public Property Username_Modifica As String
        Public Property Validita_Inizio As DateTime
        Public Property Validita_Fine As DateTime

    End Class



End Class
