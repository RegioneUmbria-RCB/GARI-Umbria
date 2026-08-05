Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Provisioning.Retail

Public Class Utenti_InfoTransazioneComm_R

End Class
Public Class Utenti_InfoTransazioneComm_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function ScriviDaObj(ByVal datiTransazioneCommerciale As Dati_Transazione_Commerciale,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_InfoTransazioneComm_W.ScriviDaObj()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO Utenti_InfoTransazioneComm ( ")
            StrSQL.AppendLine("	codiceTransazioneComm ")
            StrSQL.AppendLine("	, Username ")
            StrSQL.AppendLine("	, Servizio_Cod ")
            StrSQL.AppendLine("	, codiceSdi ")
            StrSQL.AppendLine("	, nLicenze ")
            StrSQL.AppendLine("	, codiceCoupon ")
            StrSQL.AppendLine("	, modalitaPagamento ")
            StrSQL.AppendLine("	, statoPagamento ")
            StrSQL.AppendLine("	, PEC ")
            StrSQL.AppendLine("	, Prezzo ")
            StrSQL.AppendLine("	, Codice_Prodotto ")
            StrSQL.AppendLine("	, Username_Creazione ")
            StrSQL.AppendLine("	, Username_Modifica ")
            StrSQL.AppendLine("	) VALUES ( ")

            StrSQL.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(datiTransazioneCommerciale.Codice_Transazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(datiTransazioneCommerciale.Codice_Servizio)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(datiTransazioneCommerciale.Codice_SDI)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(datiTransazioneCommerciale.Numero_Licenze)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(datiTransazioneCommerciale.Codice_Coupon)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(datiTransazioneCommerciale.Modalita_Pagamento)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(datiTransazioneCommerciale.Stato_Pagamento)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(datiTransazioneCommerciale.PEC)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(datiTransazioneCommerciale.Prezzo)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(datiTransazioneCommerciale.Codice_Prodotto)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri_Utenti.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri_Utenti.UsernameOperazione)))

            StrSQL.AppendLine("	) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Utenti, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

End Class
