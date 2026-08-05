Imports System.Text
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class XML_Log_Invio_Chiamate_R
    Inherits AgronicaCoreDataProvider.DataProvider


End Class


Public Class XML_Log_Invio_Chiamate_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           ID As Integer,
                           Tipo_Esportazione As Integer,
                           Dati_Inviati As String,
                           Data_Invio As DateTime,
                           Esito As String,
                           Dati_Ricevuti As String,
                           Controllata As Integer,
                           Tipo_Operazione As String
                           ) As Boolean


        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Log_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO [dbo].[XML_Log_Invio_Chiamate]	  ")
            stb.AppendLine("            ([PivaSuperUser]	  ")
            stb.AppendLine("            ,[ID]	  ")
            stb.AppendLine("            ,[Tipo_Esportazione]	  ")
            stb.AppendLine("            ,[Dati_Inviati]	  ")
            stb.AppendLine("            ,[Data_Invio]	  ")
            stb.AppendLine("            ,[Esito]	  ")
            stb.AppendLine("            ,[Dati_Ricevuti]	  ")
            stb.AppendLine("            ,[Controllata]	  ")
            stb.AppendLine("            ,[Tipo_Operazione]	  ")
            stb.AppendLine("            ,[inviato]	  ")
            stb.AppendLine("            ,[datainvio]	  ")
            stb.AppendLine("            ,[Data_Creazione]	  ")
            stb.AppendLine("            ,[Data_Modifica]	  ")
            stb.AppendLine("            ,[Username_Creazione]	  ")
            stb.AppendLine("            ,[Username_Modifica]	  ")
            stb.AppendLine("            ,[Validita_Inizio]	  ")
            stb.AppendLine("            ,[Validita_Fine])	  ")
            stb.AppendLine("      VALUES	  ")
            stb.AppendLine("            ('" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'	  ")
            stb.AppendLine("            ," & Agro_SQL_SaveNum(ID) & "	  ")
            stb.AppendLine("            ," & Agro_SQL_SaveNum(Tipo_Esportazione) & "	  ")
            stb.AppendLine("            ,'" & Agro_SQL_SaveText(Dati_Inviati) & "'	  ")
            stb.AppendLine("            ," & Agro_SQL_SaveDateTime(Data_Invio) & "	  ")
            stb.AppendLine("            ,'" & Agro_SQL_SaveText(Esito) & "'	  ")
            stb.AppendLine("            ,'" & Agro_SQL_SaveText(Dati_Ricevuti) & "'	  ")
            stb.AppendLine("            ," & Agro_SQL_SaveNum(Controllata) & "	  ")
            stb.AppendLine("            ,'" & Agro_SQL_SaveText(Tipo_Operazione) & "'	  ")
            stb.AppendLine("            ,0	  ")
            stb.AppendLine("            ,NULL	  ")
            stb.AppendLine("            ," & Agro_SQL_SaveDateTime(DateTime.Now) & "	  ")
            stb.AppendLine("            ," & Agro_SQL_SaveDateTime(DateTime.Now) & "	  ")
            stb.AppendLine("            ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'	  ")
            stb.AppendLine("            ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'	  ")
            stb.AppendLine("            ," & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & "	  ")
            stb.AppendLine("            ," & Agro_SQL_SaveDateTime(AGRODATAFINE) & ")	  ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

End Class
