Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class AccessoVeloceDB
    Inherits AgronicaCoreDataProvider.DataProvider



    ' A T T E N Z I O N E !!!!!!!!!!!!!!!!!!!!!!!!!!!

    'USARE AgronicaCoreMetaSchemaDAL.ISTAT

    '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
