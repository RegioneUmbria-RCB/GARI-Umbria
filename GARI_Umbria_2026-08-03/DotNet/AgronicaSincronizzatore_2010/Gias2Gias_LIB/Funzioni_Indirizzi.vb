Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreModello.Anagrafe
Imports AgronicaCoreModello
Imports AgronicaCoreG2GLocalDal
Imports AgronicaCoreUtility
Imports AgronicaCoreEntityFramework_POCO

Partial Public Class Funzioni

    Public Function recode_return_Cod_Indirizzo(ByVal oldCod_Indirizzo As Integer, ByVal Piva_SuperUser_Destinazione As String) As Integer

        'viene effettuata di nuovo la verifica perchè questa funzione viene chiamata senza "controlli", quindi può arrivare "Zero"
        If oldCod_Indirizzo = 0 Then
            Return 0
        End If

        Dim Cod_Indirizzo_Destinazione As Integer = If(_efG2G Is Nothing, (
                From c In FunzioniGLOBAL.Indirizzi
                Where c.From_Cod_Indirizzo = oldCod_Indirizzo _
                    And c.To_PivaSuperUser = Piva_SuperUser_Destinazione
                Select c.To_Cod_Indirizzo).FirstOrDefault, (
                From c In _efG2G.G2G_Recode_Indirizzi
                Where c.From_Cod_Indirizzo = oldCod_Indirizzo _
                    And c.To_PivaSuperUser = Piva_SuperUser_Destinazione
                Select c.To_Cod_Indirizzo).FirstOrDefault)

        If Cod_Indirizzo_Destinazione = 0 Then
            Dim messaggio As String
            messaggio = "Cod_Indirizzo in origine = " & CStr(oldCod_Indirizzo) &
                            " Errore, non esiste una decodifica del Cod_Indirizzo, funzione recode_return_Cod_Indirizzo."
            Throw New Exception(messaggio)

        End If

        Return Cod_Indirizzo_Destinazione
    End Function

    Public Function recode_return_Cod_IndirizzoReverse(ByVal oldCod_Indirizzo As Integer, ByVal Piva_SuperUser_Destinazione As String) As Integer

        'viene effettuata di nuovo la verifica perchè questa funzione viene chiamata senza "controlli", quindi può arrivare "Zero"
        If oldCod_Indirizzo = 0 Then
            Return 0
        End If

        Dim Cod_Indirizzo_Destinazione As Integer = If(_efG2G Is Nothing, (
                From c In FunzioniGLOBAL.Indirizzi
                Where c.To_Cod_Indirizzo = oldCod_Indirizzo _
                    And c.From_PivaSuperUser = Piva_SuperUser_Destinazione
                Select c.From_Cod_Indirizzo).FirstOrDefault, (
                From c In _efG2G.G2G_Recode_Indirizzi
                Where c.To_Cod_Indirizzo = oldCod_Indirizzo _
                    And c.From_PivaSuperUser = Piva_SuperUser_Destinazione
                Select c.From_Cod_Indirizzo).FirstOrDefault)

        If Cod_Indirizzo_Destinazione = 0 Then
            Dim messaggio As String
            messaggio = "Cod_Indirizzo in origine = " & CStr(oldCod_Indirizzo) &
                            " Errore, non esiste una decodifica del Cod_Indirizzo, funzione recode_return_Cod_Indirizzo."
            Throw New Exception(messaggio)

        End If

        Return Cod_Indirizzo_Destinazione
    End Function

End Class
