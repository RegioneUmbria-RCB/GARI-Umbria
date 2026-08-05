Imports System.Data
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Xml
Imports AgronicaCoreDataProvider

Public Class DPI_Interfaccia


    Inherits AgronicaCoreDataProvider.LogProvider

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub



    'Public Function DPI_Verifica_Conformita_Intervento(ByVal DatiAgenda As String, _
    '                                                   ByVal Id_Agenda_Escluso As Int32, _
    '                                                   ByVal Piva_Riferimento As String, _
    '                                                   ByVal Sa_Cod_Riferimento As Int32, _
    '                                                   ByVal Appezza_Riferimento As Int32, _
    '                                                   ByVal Id_Reg_Riferimento As Int32, _
    '                                                   ByVal Disciplinare_Cod As Int32, _
    '                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                                  ) As String

    '    '============================================================================

    '    Dim NomeRoutine As String = "DpiBIZ.DPI_Consultazione.Verifica_Conformita_Intervento()"

    '    Dim MessaggioErrore As String = ""

    '    Dim FlagConnessioneLocale As Boolean = False
    '    Dim xConnessione As DbConnection
    '     
    '    Dim xTransazione As DbTransaction
    '    Dim i As Int32
    '    Dim j As Int32

    '    Dim RisultatoFunzione As String = String.Empty

    '    '======
    '    Dim XmlDoc As XmlDocument
    '    Dim XmlDom As XmlDocument
    '    Dim xDatiAgenda As XmlElement
    '    Dim xAgenda As XmlElement
    '    Dim xDatiAgende As XmlNodeList
    '    Dim xAgende As XmlNodeList


    '    Dim Lav_Cod As Int32

    '    Dim i_DatiAgenda As Int32
    '    Dim i_Agenda As Int32

    '    Dim DatiWS As String
    '    '------------------------------

    '    Const MetodoNome = "DPI_Verifica_Conformita_Intervento:"

    '    Try

    '        '------------------------------
    '        'Verifico se e' stata impostata una connessione
    '        If IsNothing(objParametri.objConnessione) Then
    '            'Flag
    '            FlagConnessioneLocale = True
    '            'Creo la connessione localmente
    '            xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
    '            xTransazione = Nothing
    '        Else
    '            'Utilizzo quella passata come parametro
    '            xConnessione = objParametri.objConnessione
    '            xConnectionState = objParametri.objConnessione.State
    '            xTransazione = objParametri.objTransazione
    '        End If

    '        '------------------------------

    '        XmlDoc = New Xml.XmlDocument
    '        xDatiAgenda = XmlDoc.CreateElement("DatiAgenda")

    '        i_DatiAgenda = 0

    '        Do While i_DatiAgenda < xDatiAgende.Count

    '            'Prelevo l'i-esimo blocco di DatiAgenda (in realta' ne esiste uno solo)
    '            xDatiAgenda = xDatiAgende.Item(i_DatiAgenda)

    '            '------------------------------

    '            xAgende = xDatiAgenda.GetElementsByTagName("Agenda")

    '            i_Agenda = 0

    '            Do While i_Agenda < xAgende.Count

    '                'Prelevo l' i-esima Codifica Agenda
    '                xAgenda = xAgende.Item(i_Agenda)
    '                Lav_Cod = Agro_SQL_SaveNum(xAgenda.GetAttribute("lav_cod"))


    '                Select Case Lav_Cod

    '                    Case 18, 74, 13, 155, 158 'DISERBO, TRATTAMENTO ANTIPARASSITARIO, CONCIA, GEODISINFESTAZIONE, DISSECCAMENTO

    '                        DPI_Verifica_Conformita_Intervento = DPI_Verifica_DifesaDiserbo( _
    '                                                                             DatiAgenda, _
    '                                                                             Id_Agenda_Escluso, _
    '                                                                             Disciplinare_Cod, _
    '                                                                             Piva_Riferimento, _
    '                                                                             Sa_Cod_Riferimento, _
    '                                                                             Appezza_Riferimento, _
    '                                                                             Id_Reg_Riferimento, _
    '                                                                             objParametri)



    '                    Case 106, 123, 124, 14, 26, 156 'TRATTAMENTO ANTIBUTTERATURA, CONCIMAZIONE FOGLIARE,
    '                        'DISTRIBUZIONE AMMENDANTI ORGANICI, DISTRIBUZIONE CONCIME IN PIENO CAMPO,
    '                        'FERTIRRIGAZIONE, SARCHIATURA

    '                        DPI_Verifica_Conformita_Intervento = DPI_Verifica_Concimazione(DatiAgenda, _
    '                                                                                       Id_Agenda_Escluso, _
    '                                                                                       Piva_Riferimento, _
    '                                                                                       Sa_Cod_Riferimento, _
    '                                                                                       Appezza_Riferimento, _
    '                                                                                       Id_Reg_Riferimento, _
    '                                                                                        objParametri)




    '                    Case 125 'Raccolta

    '                        DPI_Verifica_Conformita_Intervento = DPI_Verifica_Raccolta(DatiAgenda, _
    '                                                                                   Id_Agenda_Escluso, _
    '                                                                                   Piva_Riferimento, _
    '                                                                                   Sa_Cod_Riferimento, _
    '                                                                                   Appezza_Riferimento, _
    '                                                                                   Id_Reg_Riferimento, _
    '                                                                                   objParametri)


    '                    Case Else 'Lavorazione non verificabile

    '                        DPI_Verifica_Conformita_Intervento = Nothing 'Dummy

    '                End Select

    '                i_Agenda = i_Agenda + 1

    '            Loop

    '            i_DatiAgenda = i_DatiAgenda + 1

    '        Loop

    '        '------------------------------


    '    Catch ex As Exception

    '        RisultatoFunzione = ""
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '        DPI_Verifica_Conformita_Intervento = "-1"


    '    Finally

    '        If FlagConnessioneLocale = True Then
    '            If Not IsNothing(xConnessione) Then
    '                xConnessione.Close()
    '                xConnessione.Dispose()
    '            End If
    '        Else
    '            If xConnectionState = ConnectionState.Closed Then
    '                xConnessione.Close()
    '            End If
    '        End If

    '    End Try


    'End Function


    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################

    'Public Function DPI_Verifica_Conformita_Impianto(ByVal PIVA As String, _
    '                                                 ByVal Sa_Cod As Int32, _
    '                                                 ByVal Appezza As Int32, _
    '                                                 ByVal Id_Reg As Int32, _
    '                                                 ByVal TipoTestata As Int32, _
    '                                                 ByVal Disciplinare_Cod As Int32, _
    '                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                                 ) As Integer

    '    '============================================================================

    '    Dim NomeRoutine As String = "DpiBIZ.DPI_Consultazione.DPI_Consultazione_Difesa()"

    '    Dim MessaggioErrore As String = ""

    '    Dim FlagConnessioneLocale As Boolean = False
    '    Dim xConnessione As DbConnection
    '     
    '    Dim xTransazione As DbTransaction
    '    Dim i As Int32
    '    Dim j As Int32

    '    Dim RisultatoFunzione As String = String.Empty

    '    '======
    '    Dim XmlDoc As XmlDocument
    '    Dim XmlDom As XmlDocument
    '    Dim xDatiAgenda As XmlElement
    '    Dim xAgenda As XmlElement
    '    Dim xDatiAgende As XmlNodeList
    '    Dim xAgende As XmlNodeList

    '    Dim InMts As Boolean

    '    Dim DatiAgenda As String
    '    Dim Risultato As String

    '    '------------------------------

    '    Const MetodoNome = "DPI_Verifica_Conformita_Impianto:"

    '    Try

    '        '------------------------------
    '        'Verifico se e' stata impostata una connessione
    '        If IsNothing(objParametri.objConnessione) Then
    '            'Flag
    '            FlagConnessioneLocale = True
    '            'Creo la connessione localmente
    '            xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
    '            xTransazione = Nothing
    '        Else
    '            'Utilizzo quella passata come parametro
    '            xConnessione = objParametri.objConnessione
    '            xConnectionState = objParametri.objConnessione.State
    '            xTransazione = objParametri.objTransazione
    '        End If

    '        '------------------------------

    '        Dim ObjAgende As New AgronicaCoreContabDAL.Mov_Destinazioni_R
    '        Dim DtAgende As DataTable
    '        'ObjAgende = CreateObject("Mov_Destinazioni_R")

    '        Dim ObjAgenda As New AgronicaCoreContabBIZ.Agenda_R
    '        Dim DtAgenda As DataTable
    '        Dim Dr() As DataRow
    '        'ObjAgenda = CreateObject("Agro_Contab.Agenda_R")

    '        DtAgende = ObjAgende.LeggiCronologiaMovimenti(PIVA, Sa_Cod, Appezza, Id_Reg, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

    '        If DtAgende.Rows.Count <> 0 Then

    '            'Filtro gli interventi in base alle norme di intervento
    '            Select Case TipoTestata
    '                Case 0 'Difesa
    '                    Dr = DtAgende.Select("Lav_Cod = 74") 'Trattamenti Antiparassitari
    '                Case 1 'Diserbo
    '                    Dr = DtAgende.Select("Lav_Cod = 18")
    '                Case 10 'Limitazioni Generali
    '                    Dr = DtAgende.Select("Lav_Cod = 125")

    '                Case Else
    '                    'Eccezione
    '                    Dr = DtAgende.Select("Lav_Cod = -1001")   'Uscita morbida
    '            End Select


    '            Dim iAge As Integer
    '            Do While Not iAge < Dr.Length And DPI_Verifica_Conformita_Impianto = 0

    '                DatiAgenda = _
    '                    ObjAgenda.Agenda_Leggi(PIVA, _
    '                                            Sa_Cod, _
    '                                            CLng(Dr(iAge).Item("Id_Agenda")), _
    '                                             0, _
    '                                             False, _
    '                                             objParametri)

    '                Select Case TipoTestata

    '                    Case 0, 1

    '                        Risultato = DPI_Verifica_DifesaDiserbo(DatiAgenda, _
    '                                                               CLng(DtAgende.Rows(iAge).Item("Id_Agenda")), _
    '                                                               Disciplinare_Cod, _
    '                                                               PIVA, _
    '                                                               Sa_Cod, _
    '                                                               Appezza, _
    '                                                               Id_Reg, _
    '                                                                objParametri)

    '                    Case Else

    '                        Risultato = DPI_Verifica_Raccolta(DatiAgenda, _
    '                                                          CLng(DtAgende.Rows(iAge).Item("Id_Agenda")), _
    '                                                          PIVA, _
    '                                                          Sa_Cod, _
    '                                                          Appezza, _
    '                                                          Id_Reg, _
    '                                                          objParametri)
    '                End Select

    '                '========================================================================================
    '                'Verifica criterio di arresto
    '                '----------------------------------------------------------------------------------------
    '                If InStr(1, Risultato, "err_code") > 0 Then
    '                    DPI_Verifica_Conformita_Impianto = 1
    '                End If

    '                iAge += 1

    '            Loop

    '        End If

    '        ObjAgende = Nothing
    '        DtAgende = Nothing


    '    Catch ex As Exception

    '        RisultatoFunzione = ""
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '        DPI_Verifica_Conformita_Impianto = -1


    '    Finally

    '        If FlagConnessioneLocale = True Then
    '            If Not IsNothing(xConnessione) Then
    '                xConnessione.Close()
    '                xConnessione.Dispose()
    '            End If
    '        Else
    '            If xConnectionState = ConnectionState.Closed Then
    '                xConnessione.Close()
    '            End If
    '        End If

    '    End Try


    'End Function

    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################

    ''============================================================================
    'Private Function DPI_Verifica_Concimazione(ByVal DatiAgenda As String, _
    '                                           ByVal Id_Agenda_Escluso As Int32, _
    '                                           ByVal Piva_Riferimento As String, _
    '                                           ByVal Sa_Cod_Riferimento As Int32, _
    '                                           ByVal Appezza_Riferimento As Int32, _
    '                                           ByVal Id_Reg_Riferimento As Int32, _
    '                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                          ) As String

    '    '============================================================================

    '    Dim DatiVerifica As String

    '    Const MetodoNome = "DPI_Verifica_Concimazione"
    '    Dim NomeRoutine As String = "DpiBIZ.DPI_Interfaccia.DPI_Verifica_Concimazione()"
    '    Dim MessaggioErrore As String = ""

    '    Dim FlagConnessioneLocale As Boolean = False
    '    Dim xConnessione As DbConnection
    '     
    '    Dim xTransazione As DbTransaction
    '    Dim i As Int32
    '    Dim j As Int32

    '    Dim RisultatoFunzione As String = String.Empty

    '    Dim objDpi As AgronicaCoreDpiDAL.Dpi_R

    '    '======

    '    Dim ObjDpi_Verifica As New AgronicaCoreDpiBIZ.DPI_Verifica
    '    Dim DtDpi_Verifica As DataTable
    '    'ObjDpi_Verifica = CreateObject("Agro_DPI.DPI_Verifica")

    '    DatiVerifica = ObjDpi_Verifica.DPI_Verifica_Concimazione(DatiAgenda, _
    '                                                             Id_Agenda_Escluso, _
    '                                                             Piva_Riferimento, _
    '                                                             Sa_Cod_Riferimento, _
    '                                                             Appezza_Riferimento, _
    '                                                             Id_Reg_Riferimento, _
    '                                                             objParametri)



    '    '    ========================================================================================


    '    DPI_Verifica_Concimazione = ObjDpi_Verifica.XmlFINALE(DatiVerifica, _
    '                                                          Id_Agenda_Escluso, _
    '                                                          objParametri)

    '    ObjDpi_Verifica = Nothing


    'End Function

    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################

    'Private Function DPI_Verifica_DifesaDiserbo(ByVal DatiAgenda As String, _
    '                                            ByVal Id_Agenda_Escluso As Int32, _
    '                                            ByVal Disciplinare_Cod As Int32, _
    '                                            ByVal Piva_Riferimento As String, _
    '                                            ByVal Sa_Cod_Riferimento As Int32, _
    '                                            ByVal Appezza_Riferimento As Int32, _
    '                                            ByVal Id_Reg_Riferimento As Int32, _
    '                                             ByRef objparametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                           ) As String


    '    '============================================================================


    '    Dim NomeRoutine As String = "DpiBIZ.DPI_Interfaccia.DPI_Verifica_Concimazione()"
    '    Dim MessaggioErrore As String = ""

    '    Dim FlagConnessioneLocale As Boolean = False
    '    Dim xConnessione As DbConnection
    '     
    '    Dim xTransazione As DbTransaction
    '    Dim i As Int32
    '    Dim j As Int32

    '    Dim RisultatoFunzione As String = String.Empty

    '    Dim DatiWS As String
    '    Dim DatiVerifica As String

    '    Const MetodoNome = "DPI_Verifica_DifesaDiserbo"


    '    '######################################################################################################
    '    '################################ DATI WS #############################################################
    '    '######################################################################################################


    '    Dim ObjDpi_DatiWS As New AgronicaCoreDpiBIZ.DPI_VerificaWS
    '    Dim DtDpi_DatiWS As DataTable
    '    ObjDpi_DatiWS = CreateObject("AgronicaCoreDpiBIZ.DPI_VerificaWS")

    '    'ObjDpi_DatiWS = CreateObject("Agro_DPI.DPI_VerificaWS")

    '    DatiWS = ObjDpi_DatiWS.DPI_DatiWS(DatiAgenda, _
    '                                      Id_Agenda_Escluso, _
    '                                      Disciplinare_Cod, _
    '                                      Piva_Riferimento, _
    '                                      Sa_Cod_Riferimento, _
    '                                      Appezza_Riferimento, _
    '                                      Id_Reg_Riferimento, _
    '                                      objparametri)


    '    '######################################################################################################
    '    '####################### CHIAMATA WEB SERVICE #########################################################
    '    '######################################################################################################

    '    Dim strRisultato As String

    '    'MsgBox "Ip Web Service non Impostato."

    '    Dim ObjDpi_VerificaWS As New AgronicaCoreDpiBIZ.DPI_VerificaWS
    '    Dim DtDpi_VerificaWS As DataTable
    '    ObjDpi_VerificaWS = CreateObject("AgronicaCoreDpiBIZ.DPI_VerificaWS")
    '    'ObjDpi_VerificaWS = CreateObject("Agro_DPI.DPI_VerificaWS")

    '    DatiVerifica = ObjDpi_VerificaWS.DPI_Verifica_DifesaDiserboWS(DatiWS, _
    '                                                                 objparametri)


    '    '==============================================================================================

    '    Dim ObjDpi_Verifica As New AgronicaCoreDpiBIZ.DPI_Verifica
    '    Dim DtDpi_Verifica As DataTable
    '    ObjDpi_Verifica = CreateObject("AgronicaCoreDpiBIZ.DPI_Verifica")
    '    'ObjDpi_Verifica = CreateObject("Agro_DPI.DPI_Verifica")

    '    DPI_Verifica_DifesaDiserbo = ObjDpi_Verifica.XmlFINALE(DatiVerifica, _
    '                                                           Id_Agenda_Escluso, _
    '                                                           objparametri)

    '    ObjDpi_Verifica = Nothing



    'End Function


    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################

    'Private Function DPI_Verifica_Raccolta(ByVal DatiAgenda As String, _
    '                                       ByVal Id_Agenda_Escluso As Int32, _
    '                                       ByVal Piva_Riferimento As String, _
    '                                       ByVal Sa_Cod_Riferimento As Int32, _
    '                                       ByVal Appezza_Riferimento As Int32, _
    '                                       ByVal Id_Reg_Riferimento As Int32, _
    '                                       ByRef objparametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                       ) As String

    '    '============================================================================

    '    Dim DatiVerifica As String

    '    Const MetodoNome = "DPI_Verifica_Raccolta"


    '    Dim NomeRoutine As String = "DpiBIZ.DPI_Interfaccia.DPI_Verifica_Concimazione()"
    '    Dim MessaggioErrore As String = ""

    '    Dim FlagConnessioneLocale As Boolean = False
    '    Dim xConnessione As DbConnection
    '     
    '    Dim xTransazione As DbTransaction
    '    Dim i As Int32
    '    Dim j As Int32

    '    Dim RisultatoFunzione As String = String.Empty

    '    Dim ObjDpi_Verifica As New AgronicaCoreDpiBIZ.DPI_Verifica
    '    Dim DtDpi_Verifica As DataTable
    '    ObjDpi_Verifica = CreateObject("AgronicaCoreDpiBIZ.DPI_Verifica")
    '    'ObjDpi_Verifica = CreateObject("Agro_DPI.DPI_Verifica")

    '    DatiVerifica = ObjDpi_Verifica.DPI_Verifica_Raccolta(DatiAgenda, _
    '                                                         Id_Agenda_Escluso, _
    '                                                         Piva_Riferimento, _
    '                                                         Sa_Cod_Riferimento, _
    '                                                         Appezza_Riferimento, _
    '                                                         Id_Reg_Riferimento, _
    '                                                         objparametri)

    '    ''   ========================================================================================

    '    Dim ObjDpi_VerificaWS As New AgronicaCoreDpiBIZ.DPI_VerificaWS
    '    Dim DtDpi_VerificaWS As DataTable
    '    ObjDpi_VerificaWS = CreateObject("AgronicaCoreDpiBIZ.DPI_VerificaWS")
    '    DPI_Verifica_Raccolta = ObjDpi_Verifica.XmlFINALE(DatiVerifica, _
    '                                                      Id_Agenda_Escluso, _
    '                                                      objparametri)

    '    ObjDpi_Verifica = Nothing

    '    '-----------------------------------------------------------------------------------------------------------------

    '    'GestioneErrore:
    '    DPI_Verifica_Raccolta = "-1"
    '    'Agro_GestioneErrore ClasseNome, MetodoNome, Err.Number, Err.Description, Agro_ErrSaveToLog + Agro_ErrShowMessage, LogFileName

    'End Function



    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################







End Class
