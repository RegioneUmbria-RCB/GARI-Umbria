Imports AgronicaCoreMailDAL
Imports AgronicaCoreDataProvider

Public Class Mail_Programmazione_R

    '##############################################################################################
    Public Function LeggiTutteLeMailDaInviareOra( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        'Leggi lista email con filtro pivasuperuser + [DataOraDaCuiInviare] < ora + [Spedita]=0 + [AnnullatoInvio]=0
        Dim mp_R As New AgronicaCoreMailDAL.Mail_Programmazione_R()
        Return mp_R.Leggi(Nothing, Nothing, Nothing, Nothing, DateTime.Now(), False, False, "", "", objParametri)

    End Function

    Public Function Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal ID_Mail As Integer?, _
                            ByVal TipoMail_ID As Integer?, _
                            ByVal TipoMail_Chiave As String, _
                            ByVal Spedita As Boolean?, _
                            ByVal AnnullatoInvio As Boolean?, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String _
                            ) As DataTable

        Dim mp_R As New AgronicaCoreMailDAL.Mail_Programmazione_R()
        Return mp_R.Leggi(ID_Mail, TipoMail_ID, TipoMail_Chiave, Nothing, Nothing, Spedita, AnnullatoInvio, xFiltroAggiuntivo, xOrderBy, objParametri)

    End Function

    Public Function LeggixUpdateAvviso(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByVal ID_Area As Integer,
                                       ByVal ID_Tipologia As Integer,
                                       ByVal Filtro_RapCon As String,
                                       ByVal ID_Avviso As Integer
                                       ) As DataTable

        Dim mp_R As New AgronicaCoreMailDAL.Mail_Programmazione_R()
        Return mp_R.LeggixUpdateAvviso(ID_Area, ID_Tipologia, Filtro_RapCon, ID_Avviso, "", "", objParametri)

    End Function

End Class

Public Class Mail_Programmazione_W

    '##############################################################################################
    Public Function ScriviNuovaMail(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal TipoMail_ID As Integer,
                            ByVal TipoMail_Chiave As String,
                            ByVal Mittente As String,
                            ByVal DestinatariA As String,
                            ByVal DestinatariCC As String,
                            ByVal DestinatariCCN As String,
                            ByVal Oggetto As String,
                            ByVal Body As String,
                            ByVal IsBodyHTML As Boolean,
                            ByVal Allegati As String,
                            ByVal DataOraDaCuiInviare As DateTime,
                            Optional ByVal Data_creazione As Date = #2/1/1900#,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_creazione As String = "",
                            Optional ByVal username_modifica As String = "",
                            Optional ByVal Filtro_RapConMail As String = ""
                            ) As Boolean

        '  Prendo l'indice dalle agrosequenze
        Dim seq As New Agro_Sequenze()
        Dim ID_Mail As Integer = seq.NuovoId_Tabella("Mail_ID_Mail", 0, 2000000000, objParametri)

        Dim mp_W As New AgronicaCoreMailDAL.Mail_Programmazione_W()
        Return mp_W.Scrivi(objParametri, ID_Mail, TipoMail_ID, TipoMail_Chiave, Mittente, DestinatariA,
                      DestinatariCC, DestinatariCCN, Oggetto, Body, IsBodyHTML, Allegati,
                      DataOraDaCuiInviare, False, Nothing, "", False, Nothing, "", Filtro_RapConMail)
    End Function

    Public Function CancellaProgrammazioniFromIDMail(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                            ByVal Old_ID_Mail As Integer, _
                                            Optional ByVal Data_modifica As Date = #2/1/1900#, _
                                            Optional ByVal username_modifica As String = "" _
                                            ) As Boolean

        Dim mp_W As New AgronicaCoreMailDAL.Mail_Programmazione_W()
        Return mp_W.Modifica_AnnullaInvioFromIDMail(objParametri, Old_ID_Mail, True, DateTime.Now, objParametri.UsernameOperazione)

    End Function

    Public Function CancellaProgrammazioniFromChiave(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal Old_TipoMail_ID As Integer,
                                        ByVal Old_TipoMail_Chiave As String,
                                        Optional ByVal Data_modifica As Date = #2/1/1900#,
                                        Optional ByVal username_modifica As String = ""
                                        ) As Boolean

        Dim mp_W As New AgronicaCoreMailDAL.Mail_Programmazione_W()
        Return mp_W.Modifica_AnnullaInvioFromChiave(objParametri, Old_TipoMail_ID, Old_TipoMail_Chiave, True, DateTime.Now, objParametri.UsernameOperazione)

    End Function

    Public Function CancellaProgrammazioniFromChiaveTestataFaseNC(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal Old_TipoMail_ID As Integer,
                                        ByVal Old_TipoMail_Chiave As String,
                                        Optional ByVal Data_modifica As Date = #2/1/1900#,
                                        Optional ByVal username_modifica As String = ""
                                        ) As Boolean

        Return CancellaProgrammazioniFromChiave(objParametri, Old_TipoMail_ID, Old_TipoMail_Chiave, DateTime.Now, objParametri.UsernameOperazione)

    End Function

    Public Function CancellaProgrammazioniFromChiaveTestataNC(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByVal Old_TipoMail_ID As Integer,
                                    ByVal Old_TipoMail_ChiaveLike As String,
                                    Optional ByVal Data_modifica As Date = #2/1/1900#,
                                    Optional ByVal username_modifica As String = ""
                                    ) As Boolean

        Dim mp_W As New AgronicaCoreMailDAL.Mail_Programmazione_W()
        Return mp_W.Modifica_AnnullaInvioFromChiaveLike(objParametri, Old_TipoMail_ID, Old_TipoMail_ChiaveLike, True, DateTime.Now, objParametri.UsernameOperazione)

    End Function

    Public Function MarcaSpedizioneMail(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                    ByVal ID_Mail As Integer, _
                                    ByVal Spedita As Boolean, _
                                    ByVal Spedizione_DataOra As DateTime, _
                                    ByVal Spedizione_Risultato As String, _
                                    Optional ByVal Data_modifica As Date = #2/1/1900#, _
                                    Optional ByVal username_modifica As String = "" _
                                ) As Boolean

        Dim mp_W As New AgronicaCoreMailDAL.Mail_Programmazione_W()
        Return mp_W.MarcaSpedizioneMail(objParametri, ID_Mail, Spedita, Spedizione_DataOra, Spedizione_Risultato, Data_modifica, username_modifica)

    End Function


    Public Function AggiornaDataInvio(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      ByVal TipoMail_Chiave As Integer,
                                      ByVal New_MailA As String,
                                      ByVal DataOraDaCuiInviare As DateTime
                                      ) As Boolean

        Dim mp_W As New AgronicaCoreMailDAL.Mail_Programmazione_W()
        Return mp_W.AggiornaDataInvio(objParametri, TipoMail_Chiave, New_MailA, DataOraDaCuiInviare)

    End Function

End Class