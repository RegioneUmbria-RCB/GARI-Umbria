Imports System.Data.OleDb
Imports System.Xml
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Imports AgronicaGIS2012.Commons


Public Class APP_EntrateUsciteCoordinate

    Public Sub EntrateUscite_ScriviPerAPP(
        ByVal unid As String,
        EntrateUsciteList As List(Of APP_LogEventi),
        objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    )

        Dim EFArrayToInsert As New ArrayList

        Try

            For Each objAPP_LogEventi In EntrateUsciteList
                objAPP_LogEventi.ID = unid & "|" & objAPP_LogEventi.ID
                objAPP_LogEventi.Piva_Superuser = objParametri_Server.PivaSuperUser
                objAPP_LogEventi.Data_Creazione = DateTime.Now
                objAPP_LogEventi.Data_Modifica = DateTime.Now
                EFArrayToInsert.Add(objAPP_LogEventi)
            Next

            Dim scriviEntrateUscite As New AgronicaCoreGisDAL.APP_EntrateUsciteCoordinate_W
            Dim err As String = ""
            err = scriviEntrateUscite.APP_Scrivi_EntrateUsciteCoordinate(EFArrayToInsert, objParametri_Server)

            If err <> "" Then
                Throw New Exception(err)
            End If

        Catch ex As Exception

            Throw ex

        End Try

    End Sub

    Public Sub Coordinate_ScriviPerAPP(
        ByVal unid As String,
        CoordinateList As List(Of APP_GIS),
        objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    )

        Dim EFArrayToInsert As New ArrayList

        Try

            For Each objAPP_Gis In CoordinateList
                objAPP_Gis.ID = unid & "|" & objAPP_Gis.ID
                objAPP_Gis.Piva_Superuser = objParametri_Server.PivaSuperUser
                EFArrayToInsert.Add(objAPP_Gis)
            Next

            Dim scriviEntrateUscite As New AgronicaCoreGisDAL.APP_EntrateUsciteCoordinate_W
            Dim err As String = ""
            err = scriviEntrateUscite.APP_Scrivi_EntrateUsciteCoordinate(EFArrayToInsert, objParametri_Server)

            If err <> "" Then                
                Throw New Exception(err)
            End If

            Try
                Dim esitoAllinea As Boolean = 
                    scriviEntrateUscite.APP_AllineaUltimaPosizioneIMotionDaAPP(objParametri_Server)
           
            Catch ex As Exception
                'in ogni caso, la scrittura del dato nella tabella di frontiera va a buon fine, quindi l'esito è comunque positivo
            End Try
            
        Catch ex As Exception

            Throw ex

        End Try

    End Sub

End Class
