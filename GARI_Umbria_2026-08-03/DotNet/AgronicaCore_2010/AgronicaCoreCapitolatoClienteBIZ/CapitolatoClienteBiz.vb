Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider
Imports <xmlns="http://ws_CapitolatoCliente_ListaImpreseCapitolati">
Imports AgronicaCoreCapitolatoClienteDAL
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility

Public Class CapitolatoClienteBiz
    Private _objParametriServer As AgronicaCoreParametri

    Public Function AvviaControlli()

    End Function
    Public Sub New()
        Me.New(Nothing)
    End Sub

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Function leggiNuovoIdTabellaCapitolatoCliente() As Integer
        Dim aSequenze As Agro_Sequenze = New Agro_Sequenze
        Return aSequenze.NuovoId_Tabella("CapitolatoCliente", 0, 0, _objParametriServer)
    End Function
    ''' <summary>
    ''' Crea una copia di tutto l'aggregato dati del capitolato cliente 
    ''' </summary>
    ''' <param name="pivasuperuser"></param>
    ''' <param name="codCApitolato"></param>
    ''' <returns>Torna il nuovo codice capitolato creato</returns>
    Public Function DuplicaCapitolatoCliente(pivasuperuser As String, codCApitolato As Integer
                                    ) As modelCapitolatoCliente
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim nuovoCODcapitolato As Integer = 0
        Dim CapitolatoCliente As CapitolatoCliente = Nothing
        Dim ListCapitolatoClienteXCultivar As New List(Of CapitolatoClienteXCultivar)
        Dim ListCapitolatoClienteXFamigliePrincipiAttiviRilevati As New List(Of CapitolatoClienteXFamigliePrincipiAttiviRilevati)
        Dim ListCapitolatoClienteXImprese As New List(Of modelElencoAziende)
        Dim ListCapitolatoClienteXPrincipiAttiviRilevati As New List(Of CapitolatoClienteXPrincipiAttiviRilevati)

        Dim objCapitolato As New modelCapitolatoCliente()

        Try
            ' -- estraggo nuovo codice capitolato (id) da assegnare a copia capitolati 
            nuovoCODcapitolato = leggiNuovoIdTabellaCapitolatoCliente()
            ' -- estare il modello testata del capitolato
            CapitolatoCliente = LeggiCapitolatoCliente(pivasuperuser, codCApitolato)
            ' -- se trovo la testata (dovrebbe essere sempre presente !!) e riesco ad ottenre un nuovo
            '    codice capitolato procedo con la lettura anche degli altri dati dell'aggregato 
            If Not (IsNothing(CapitolatoCliente)) And nuovoCODcapitolato > 0 Then
                ListCapitolatoClienteXCultivar = LeggiElencoCapitolatoClienteXCultivar(codCApitolato)
                ListCapitolatoClienteXFamigliePrincipiAttiviRilevati = LeggiElencoCapitolatoClienteXFamigliePrincipiAttiviRilevati(codCApitolato)
                ListCapitolatoClienteXImprese = LeggiElencoCapitolatoClienteXImprese(pivasuperuser, codCApitolato)
                ListCapitolatoClienteXPrincipiAttiviRilevati = LeggiElencoCapitolatoClienteXPrincipiAttiviRilevati(codCApitolato)
                ' -- cambio i valori per la duplica 
                If DuplicaCapitolatoCliente_cambia_valori(nuovoCODcapitolato,
                                                            CapitolatoCliente,
                                                            ListCapitolatoClienteXCultivar,
                                                            ListCapitolatoClienteXFamigliePrincipiAttiviRilevati,
                                                            ListCapitolatoClienteXImprese,
                                                            ListCapitolatoClienteXPrincipiAttiviRilevati) Then

                    ' -- se arrivo qui persisto i dati duplicati sul DB
                    Dim xRisp As Boolean = False
                    '
                    ' -- istanzio i vari dal necessari per la persistenza dati 
                    Dim CapitolatoW As New Capitolato_W
                    Dim CapitolatoClienteXCultivarW As New CapitolatoClienteXCultivar_W
                    Dim CapitolatoClienteXFamigliePrincipiAttiviRilevatiW As New CapitolatoClienteXFamigliePrincipiAttiviRilevati_W
                    Dim CapitolatoClienteXImpreseW As New CapitolatoClienteXImprese_W
                    Dim CapitolatoClienteXPrincipiAttiviRilevatiW As New CapitolatoClienteXPrincipiAttiviRilevati_W
                    '
                    ' -- fase persistenza DB 
                    Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

                    xRisp = CapitolatoW.Scrivi(CapitolatoCliente, _objParametriServer)
                    xRisp = CapitolatoClienteXCultivarW.Scrivi(ListCapitolatoClienteXCultivar, _objParametriServer)
                    xRisp = CapitolatoClienteXFamigliePrincipiAttiviRilevatiW.Scrivi(ListCapitolatoClienteXFamigliePrincipiAttiviRilevati, _objParametriServer)
                    xRisp = CapitolatoClienteXImpreseW.Scrivi(ListCapitolatoClienteXImprese, _objParametriServer)
                    xRisp = CapitolatoClienteXPrincipiAttiviRilevatiW.Scrivi(ListCapitolatoClienteXPrincipiAttiviRilevati, _objParametriServer)

                    Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
                    '
                    ' -- serializza oggetto per il ritorno  riga nuova.
                    '    usa oggetto 'modelCapitolatoCliente' valorizzando solo la 
                    '    property 'Capitolato_COD'
                    objCapitolato.Capitolato_COD = CapitolatoCliente.Capitolato_COD

                End If
            End If
        Catch ex As Exception
            ' -- in caso di errore se avevo aperto la transaction provvede a chiuderla
            If flagTransazione Then
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If
            Throw New Exception("[ CapitolatoClienteBiz.DuplicaCapitolatoCliente() ] : " + ex.Message)
        End Try

        ' -- torno codice nuovo capitolato 
        Return objCapitolato
    End Function

    Private Function DuplicaCapitolatoCliente_cambia_valori(nuovoCODcapitolato As Integer,
                                                            ByRef CapitolatoCliente As CapitolatoCliente,
                                                            ByRef ListCapitolatoClienteXCultivar As List(Of CapitolatoClienteXCultivar),
                                                            ByRef ListCapitolatoClienteXFamigliePrincipiAttiviRilevati As List(Of CapitolatoClienteXFamigliePrincipiAttiviRilevati),
                                                            ByRef ListCapitolatoClienteXImprese As List(Of modelElencoAziende),
                                                            ByRef ListCapitolatoClienteXPrincipiAttiviRilevati As List(Of CapitolatoClienteXPrincipiAttiviRilevati)
                                                            ) As Boolean
        Dim prefissoCopiaDescrizione As String = "***COPIA "

        Try
            ' *** inizio trattamento dati prima della scrittura dei dati 
            CapitolatoCliente.Capitolato_DES = prefissoCopiaDescrizione + CapitolatoCliente.Capitolato_DES
            CapitolatoCliente.Capitolato_COD = nuovoCODcapitolato
            ' *** aggiorno Capitolato_COD con il nuovo cod capitolato nelle tabelle aggregate 
            For Each e As CapitolatoClienteXCultivar In ListCapitolatoClienteXCultivar
                e.Capitolato_COD = nuovoCODcapitolato
            Next
            For Each e As CapitolatoClienteXFamigliePrincipiAttiviRilevati In ListCapitolatoClienteXFamigliePrincipiAttiviRilevati
                e.Capitolato_COD = nuovoCODcapitolato
            Next
            For Each e As modelElencoAziende In ListCapitolatoClienteXImprese
                e.Capitolato_COD = nuovoCODcapitolato
            Next
            For Each e As CapitolatoClienteXPrincipiAttiviRilevati In ListCapitolatoClienteXPrincipiAttiviRilevati
                e.Capitolato_COD = nuovoCODcapitolato
            Next
        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.DuplicaCapitolatoCliente_cambia_valori() ] : " & ex.Message)
            Return False
        End Try

        Return True
    End Function

    Public Function LeggiCapitolatoCliente(ByVal pivasuperuser As String,
                                    ByVal idCapitolato As Integer
                                    ) As CapitolatoCliente

        Dim flagConnessione As Boolean = False

        Dim obj As New CapitolatoCliente

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim objCapR As New AgronicaCoreCapitolatoClienteDAL.Capitolato_R
            Dim dt = objCapR.Leggi(pivasuperuser, idCapitolato, _objParametriServer)

            Using dmu As New DatamodelUtils
                Dim obj1 As List(Of Object) = dmu.loadDTonDataModel(dt, New CapitolatoCliente)
                If obj1.Count > 0 Then
                    obj = obj1.Cast(Of CapitolatoCliente).ToList(0)
                Else
                    obj = New CapitolatoCliente
                End If
            End Using

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiCapitolatoCliente() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return obj

    End Function

    Public Function LeggiElencoCapitolatoClienteXCultivar(ByVal codCApitolato As Integer
                                    ) As List(Of CapitolatoClienteXCultivar)

        Dim flagConnessione As Boolean = False

        Dim objList As New List(Of CapitolatoClienteXCultivar)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim objDAL As New AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXCultivar_R
            Dim dt = objDAL.Leggi(codCApitolato, _objParametriServer)

            Using dmu As New DatamodelUtils
                Dim obj1 As List(Of Object) = dmu.loadDTonDataModel(dt, New CapitolatoClienteXCultivar)
                objList = obj1.Cast(Of CapitolatoClienteXCultivar).ToList()
            End Using

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiElencoCapitolatoClienteXCultivar() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objList

    End Function

    Public Function LeggiElencoCapitolatoClienteXFamigliePrincipiAttiviRilevati(ByVal codCApitolato As Integer
                                    ) As List(Of CapitolatoClienteXFamigliePrincipiAttiviRilevati)

        Dim flagConnessione As Boolean = False

        Dim objList As New List(Of CapitolatoClienteXFamigliePrincipiAttiviRilevati)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim objDAL As New AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXFamigliePrincipiAttiviRilevati_R
            Dim dt = objDAL.Leggi(codCApitolato, _objParametriServer)

            Using dmu As New DatamodelUtils
                Dim obj1 As List(Of Object) = dmu.loadDTonDataModel(dt, New CapitolatoClienteXFamigliePrincipiAttiviRilevati)
                objList = obj1.Cast(Of CapitolatoClienteXFamigliePrincipiAttiviRilevati).ToList()
            End Using
        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiElencoCapitolatoClienteXFamigliePrincipiAttiviRilevati() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objList

    End Function

    Public Function LeggiElencoCapitolatoClienteXImprese(pivasuperuser As String, ByVal codCApitolato As Integer
                                    ) As List(Of modelElencoAziende)

        Dim flagConnessione As Boolean = False

        Dim objList As New List(Of modelElencoAziende)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim objDAL As New AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXImprese_R
            Dim dt = objDAL.Leggi(pivasuperuser, codCApitolato, _objParametriServer)

            Using dmu As New DatamodelUtils
                Dim obj1 As List(Of Object) = dmu.loadDTonDataModel(dt, New modelElencoAziende)
                objList = obj1.Cast(Of modelElencoAziende).ToList()
            End Using

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiElencoCapitolatoClienteXImprese() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objList

    End Function

    Public Function LeggiElencoCapitolatoClienteXPrincipiAttiviRilevati(ByVal codCApitolato As Integer
                                    ) As List(Of CapitolatoClienteXPrincipiAttiviRilevati)

        Dim flagConnessione As Boolean = False

        Dim objList As New List(Of CapitolatoClienteXPrincipiAttiviRilevati)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim objDAL As New AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXPrincipiAttiviRilevati_R
            Dim dt = objDAL.Leggi(codCApitolato, _objParametriServer)

            Using dmu As New DatamodelUtils
                Dim obj1 As List(Of Object) = dmu.loadDTonDataModel(dt, New CapitolatoClienteXPrincipiAttiviRilevati)
                objList = obj1.Cast(Of CapitolatoClienteXPrincipiAttiviRilevati).ToList()
            End Using

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiElencoCapitolatoClienteXPrincipiAttiviRilevati() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objList

    End Function

    Public Function LeggiCapitolato(ByVal pivasuperuser As String,
                                    ByVal idCapitolato As Integer
                                    ) As modelCapitolatoCliente

        Dim flagConnessione As Boolean = False

        Dim objCapitolato As modelCapitolatoCliente = Nothing
        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim objCapR As New AgronicaCoreCapitolatoClienteDAL.Capitolato_R
            Dim dt = objCapR.LeggiXvideo(pivasuperuser, idCapitolato, _objParametriServer)

            For Each row As DataRow In dt.Rows

                objCapitolato = New modelCapitolatoCliente() With {
                    .Capitolato_COD = row.ToIntero("Capitolato_COD"),
                    .Capitolato_Codice = row.ToStringa("Capitolato_Codice"),
                    .Nome = row.ToStringa("Nome"),
                    .NomeBreve = row.ToStringa("NomeBreve"),
                    .DPCampagnaPubPriv = row.ToStringa("DPCampagnaPubPriv"),
                    .DPCampagnaDES = row.ToStringa("DPCampagnaDES"),
                    .N_Max_PA = row.ToIntero("N_Max_PA"),
                    .N_Max_Tracce = row.ToIntero("N_Max_Tracce"),
                    .Perc_Max_RMA = row.ToDecimale("Perc_Max_RMA"),
                    .Sum_Perc_Max_RMA = row.ToDecimale("Sum_Perc_Max_RMA"),
                    .Attivo = row.ToBooleano("Attivo"), '= False
                    .DPI_Impianto = row.ToBooleano("DPI_Impianto"), '= False
                    .GestioneVarieta = row.ToBooleano("GestioneVarieta"), '= False
                    .Validita_Inizio = row.ToDataInizio("Validita_Inizio"), '= AGRODATAINIZIO
                    .Validita_Fine = row.ToDataFine("Validita_Fine"),  '= AGRODATAINIZIO
                    .Piva_SuperUser = row.ToStringa("Piva_SuperUser"), '= ""
                    .Flag_Privato_Pubblico_DPI = If(row.ToIntero("Flag_Privato_Pubblico_DPI") Is Nothing, 0, row.ToIntero("Flag_Privato_Pubblico_DPI")), '= 0
                    .DPI_COD_REGOLAMENTO = If(row.ToIntero("DPI_COD_REGOLAMENTO") Is Nothing, 0, row.ToIntero("DPI_COD_REGOLAMENTO")),'= 0
                    .veg_cod = row.ToIntero("veg_cod"), '= 0
                    .TabellaRMA_COD = row.ToIntero("TabellaRMA_COD"), '= 0
                    .FlagBIO = row.ToBooleano("FlagBIO"), '= False
                    .GestionePartecipazioneAziende = row.ToBooleano("GestionePartecipazioneAziende"), '= False
                    .AttivaVerificaFormulatoValido = row.ToBooleano("AttivaVerificaFormulatoValido") '= False
                }
            Next

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiCapitolato() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objCapitolato

    End Function


    Public Function LeggiElencoCapitolati(ByVal pivasuperuser As String
                                    ) As List(Of modelCapitolatoCliente)

        Dim flagConnessione As Boolean = False
        Dim objCapitolatoList As New List(Of modelCapitolatoCliente)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim objCapR As New AgronicaCoreCapitolatoClienteDAL.Capitolato_R
            Dim dt = objCapR.Leggi_ElencoCapitolati(pivasuperuser, _objParametriServer)

            For Each row As DataRow In dt.Rows
                objCapitolatoList.Add(New modelCapitolatoCliente() With {
                    .Capitolato_COD = row("Capitolato_COD"),
                    .Nome = row("Nome"),
                    .NomeBreve = If(IsDBNull(row("NomeBreve")), "", row("NomeBreve")),
                    .DPCampagnaPubPriv = If(IsDBNull(row("DPCampagnaPubPriv")), "", row("DPCampagnaPubPriv")),
                    .DPCampagnaDES = If(IsDBNull(row("DPCampagnaDES")), "", row("DPCampagnaDES")),
                    .N_Max_PA = row.ToIntero("N_Max_PA"),
                    .N_Max_Tracce = row.ToIntero("N_Max_Tracce"),
                    .Perc_Max_RMA = row.ToDecimale("Perc_Max_RMA"),
                    .Sum_Perc_Max_RMA = row.ToDecimale("Sum_Perc_Max_RMA"),
                    .Attivo = CBool(row("Attivo")), '= False
                    .DPI_Impianto = CBool(row("DPI_Impianto")), '= False
                    .GestioneVarieta = CBool(row("GestioneVarieta")), '= False
                    .Piva_SuperUser = row("Piva_SuperUser"),
                    .Data_Modifica = row("Data_Modifica"),
                    .Data_Creazione = row("Data_Creazione"),
                    .veg_des = row("veg_des")
                })
            Next

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiElencoCapitolati() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objCapitolatoList

    End Function

    Public Function LeggiElencoTabellaRMA(ByVal pivasuperuser As String) As List(Of TabellaRMAModel)

        Dim flagConnessione As Boolean = False

        Dim objList As New List(Of TabellaRMAModel)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim objDAL As New AgronicaCoreCapitolatoClienteDAL.Capitolato_R
            Dim dt = objDAL.Leggi_ElencoTabellaRMA(pivasuperuser, _objParametriServer)

            Using dmu As New DatamodelUtils
                Dim obj1 As List(Of Object) = dmu.loadDTonDataModel(dt, New TabellaRMAModel)
                objList = obj1.Cast(Of TabellaRMAModel).ToList()
            End Using

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiElencoCapitolatoClienteXCultivar() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objList
    End Function

    Public Function LeggiElencoPiveIncluseEscluse(ByVal pivasuperuser As String,
                                                  ByVal idCapitolato As Integer) As List(Of modelElencoAziende)

        Dim flagConnessione As Boolean = False

        Dim objPivaList As New List(Of modelElencoAziende)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)


            Dim rCapClixImp = New AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXImprese_R
            Dim dt = rCapClixImp.Leggi(pivasuperuser, idCapitolato, _objParametriServer)

            For Each row As DataRow In dt.Rows
                objPivaList.Add(New modelElencoAziende() With {
                    .Capitolato_COD = row.ToIntero("Capitolato_COD"),
                    .Piva = row.ToStringa("Piva"),
                    .PivaSuperUser = row.ToStringa("PivaSuperUser")
                })
            Next

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiElencoCapitolati() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objPivaList
    End Function

    Public Function LeggiDPIPubblici(ByVal pivasuperuser As String,
                                     ByVal veg_cod As Int32,
                                     ByVal testoFiltro As String) As List(Of modelDPIRegolamento)
        Dim flagConnessione As Boolean = False

        Dim objDPIList As New List(Of modelDPIRegolamento)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim DLL_AD As New AgronicaCoreDpiDAL.Dpi_R

            Dim dt = DLL_AD.Leggi_DPI_DataPivaSuperUSer_PrivatoPubblico_DammiDPI(
                    pivasuperuser,
                    1,
                    _objParametriServer
                )

            Dim strFiltroDpiPubblici As String = "Regolamenti.Cod_Regolamento IN ("

            For Each row In dt.Rows
                strFiltroDpiPubblici &= row("DPI_COD_Regolamento").ToString() + ","
            Next

            strFiltroDpiPubblici = strFiltroDpiPubblici.Substring(0, strFiltroDpiPubblici.Length - 1)

            strFiltroDpiPubblici &= ")"

            Dim DT_Pubblici As DataTable

            DT_Pubblici = DLL_AD.Leggi_Disciplinari(0,
                                                    0,
                                                    veg_cod,
                                                    0,
                                                    0,
                                                    AGRODATAINIZIO,
                                                    AGRODATAFINE,
                                                    strFiltroDpiPubblici,
                                                    " Regolamenti.validoAl desc, NomeEsteso ASC  ",
                                                    _objParametriServer)


            objDPIList.Add(New modelDPIRegolamento() With {
                    .DPI_COD_Regolamento = 0,
                    .NomeEsteso = ""
                })

            For Each row As DataRow In DT_Pubblici.Rows
                If row("NomeEsteso").ToString().Contains(testoFiltro) Or testoFiltro = "" Then
                    objDPIList.Add(New modelDPIRegolamento() With {
                        .DPI_COD_Regolamento = row.ToIntero("Cod_Regolamento"),
                        .NomeEsteso = row.ToStringa("NomeEsteso")
                    })
                End If
            Next

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiDPIPubblici() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objDPIList
    End Function

    Public Function LeggiDPIPrivati(ByVal pivasuperuser As String,
                                    ByVal veg_cod As Int32,
                                    ByVal testoFiltro As String) As List(Of modelDPIRegolamento)
        Dim flagConnessione As Boolean = False

        Dim objDPIList As New List(Of modelDPIRegolamento)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim DLL_AD As New AgronicaCoreDpiDAL.Dpi_R

            Dim dt = DLL_AD.Leggi_DPI_DataPivaSuperUSer_PrivatoPubblico_DammiDPI(
                    pivasuperuser,
                    2,
                    _objParametriServer
                )

            Dim strFiltroDpiPrivati As String = "Regolamenti.Cod_Regolamento IN ("

            For Each row In dt.Rows
                strFiltroDpiPrivati &= row("DPI_COD_Regolamento") + ","
            Next

            strFiltroDpiPrivati &= ")"

            Dim DT_Pubblici As DataTable

            DT_Pubblici = DLL_AD.Leggi_Disciplinari(0,
                                                    0,
                                                    veg_cod,
                                                    0,
                                                    0,
                                                    AGRODATAINIZIO,
                                                    AGRODATAFINE,
                                                    strFiltroDpiPrivati,
                                                    " Regolamenti.validoAl desc, NomeEsteso ASC  ",
                                                    _objParametriServer)


            objDPIList.Add(New modelDPIRegolamento() With {
                    .DPI_COD_Regolamento = 0,
                    .NomeEsteso = ""
                })

            For Each row As DataRow In DT_Pubblici.Rows
                If row("NomeEsteso").ToString().Contains(testoFiltro) Or testoFiltro = "" Then
                    objDPIList.Add(New modelDPIRegolamento() With {
                        .DPI_COD_Regolamento = row.ToIntero("Cod_Regolamento"),
                        .NomeEsteso = row.ToStringa("NomeEsteso")
                    })
                End If
            Next

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiDPIPubblici() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objDPIList
    End Function

    Public Function GetFlagDPIPubblicoPrivato() As List(Of modelFlagDPIPrivatoPubblico)
        Dim flagConnessione As Boolean = False

        Dim objDPIList As New List(Of modelFlagDPIPrivatoPubblico)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim rFlag As New AgronicaCoreCapitolatoClienteDAL.FlagDPIPrivatoPubblico_R

            Dim dt = rFlag.LeggiElencoFlag(_objParametriServer)

            Dim n = dt.NewRow()
            n("Flag_DPI_COD") = 0
            n("Descrizione") = ""
            dt.Rows.Add(n)

            dt.DefaultView.Sort = "Flag_DPI_COD ASC"
            dt = dt.DefaultView.ToTable

            For Each row As DataRow In dt.Rows
                objDPIList.Add(New modelFlagDPIPrivatoPubblico() With {
                    .Flag_DPI_COD = row.ToIntero("Flag_DPI_COD"),
                    .Descrizione = row.ToStringa("Descrizione")
                })
            Next

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiDPIPubblici() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objDPIList
    End Function


    Public Function LeggiElencoDisciplinari(ByVal pivasuperuser As String,
                                            ByVal flag_privato_pubblico As Boolean,
                                            ByVal valoreFiltro As String) As List(Of modelElencoDisciplinari)

        Dim flagConnessione As Boolean = False

        Dim objDPIList As New List(Of modelElencoDisciplinari)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim filtroAggiuntivo As String = If(String.IsNullOrEmpty(valoreFiltro), "", $" (dpi.NomeEsteso Like '%{valoreFiltro}%' ) ")

            Dim rCapCli = New AgronicaCoreCapitolatoClienteDAL.Capitolato_R
            Dim dt = rCapCli.LeggiRegolamentoDPIPrivPubb(pivasuperuser, flag_privato_pubblico, filtroAggiuntivo, _objParametriServer)

            For Each row As DataRow In dt.Rows
                objDPIList.Add(New modelElencoDisciplinari() With {
                    .DPI_COD_REGOLAMENTO = row.ToIntero("DPI_COD_REGOLAMENTO"),
                    .NomeEsteso = row.ToStringa("NomeEsteso")
                })
            Next

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiElencoDisciplinari() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objDPIList
    End Function

    Public Function GetElencoSpecieVegtali(ByVal valoreFiltro As String) As List(Of modelElencoSpecieVegetali)
        Dim flagConnessione As Boolean = False

        Dim objSpecieList As New List(Of modelElencoSpecieVegetali)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim filtroAggiuntivo As String = If(String.IsNullOrEmpty(valoreFiltro), "",
                String.Format(" (SpecieVegetali.Veg_Des Like '%{0}%' ) ", valoreFiltro))

            Dim rCapCli = New AgronicaCoreCapitolatoClienteDAL.Capitolato_R
            Dim dt = rCapCli.LeggiElencoSpecieVegetali(filtroAggiuntivo, _objParametriServer)

            For Each row As DataRow In dt.Rows
                objSpecieList.Add(New modelElencoSpecieVegetali() With {
                    .CodiceSpecie = row.ToIntero("Veg_Cod"),
                    .Descrizione = row.ToStringa("Veg_Des")
                })
            Next

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.GetElencoSpecieVegtali() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objSpecieList

    End Function

    Public Function LeggiElencoCultivar(ByVal Capitolato_COD As Integer, veg_cod As Integer) As List(Of modelElencoVarieta)
        Dim flagConnessione As Boolean = False

        Dim objVarietaList As New List(Of modelElencoVarieta)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim rCapCli = New AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXCultivar_R
            Dim dt = rCapCli.LeggiElencoCultivar(Capitolato_COD, veg_cod, _objParametriServer)

            For Each row As DataRow In dt.Rows
                objVarietaList.Add(New modelElencoVarieta() With {
                    .CodiceVarieta = row.ToIntero("Cul_COD"),
                    .Descrizione = row.ToStringa("Cul_Des")
                })
            Next

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiElencoCultivar() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objVarietaList
    End Function
    Public Function LeggiElencoCapitolatoClienteCultivar(ByVal Capitolato_COD As Integer) As List(Of modelElencoVarieta)
        Dim flagConnessione As Boolean = False

        Dim objVarietaList As New List(Of modelElencoVarieta)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)


            Dim rCapCli = New AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXCultivar_R
            Dim dt = rCapCli.LeggiElencoCapitolatoClienteCultivar(Capitolato_COD, _objParametriServer)

            For Each row As DataRow In dt.Rows
                objVarietaList.Add(New modelElencoVarieta() With {
                    .CodiceVarieta = row.ToIntero("Cul_Cod"),
                    .Descrizione = row.ToStringa("Cul_Des")
                })
            Next

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.GetElencoVarietaXSpecieVegtali() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objVarietaList

    End Function


    Public Function CreaCapitolato(ByVal pivasuperuser As String,
                                   ByVal inData As modelCapitolatoCliente) As modelCapitolatoCliente

        Dim flagConnessione As Boolean = False

        Dim objCapitolato As modelCapitolatoCliente = Nothing

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.LeggiCapitolato() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objCapitolato

    End Function

    Public Function ElaboraListaImpreseEscluseDaCapitolati(
        ByVal listaImprese As String,
        ByVal objParametri As AgronicaCoreParametri) As String

        listaImprese = listaImprese.Replace("<imprese", "<imprese xmlns=""http://ws_CapitolatoCliente_ListaImpreseCapitolati"" ")

        Dim xD As XDocument = XDocument.Parse(listaImprese)
        Dim xVerifica As New AgronicaCoreCapitolatoClienteDAL.Capitolato_R

        Dim lista = (
            From aa In xD.<imprese>.<impresa>
        ).ToList

        Dim pivaDaVerificare As String

        Dim pivaSuperUserDaVerificare As String
        pivaSuperUserDaVerificare = xD.<imprese>.@pivasuperuser

        Dim dtEsitoVerifica As DataTable

        For Each ll In lista

            pivaDaVerificare = ll.@piva

            Dim listaCapitolatiDaVerificare = (From lCap In ll.<capitolato>).ToList

            Dim listaNodiCapitolatiCod = String.Join(",", (From el1 In listaCapitolatiDaVerificare Select el1.@id).ToList)

            dtEsitoVerifica = xVerifica.LeggiElencoCapitolatiEsclusiDataPiva(pivaSuperUserDaVerificare, pivaDaVerificare, listaNodiCapitolatiCod, objParametri)

            For Each curCap As XElement In listaCapitolatiDaVerificare
                curCap.Value = ((From tdd In dtEsitoVerifica.AsEnumerable Where tdd("Capitolato_Cod") = curCap.@id).ToList.Count = 0).ToString.ToLower
            Next

        Next

        Return xD.ToString()

    End Function


    Public Function SalvaTutto(piva As String, idCapitolato As String, dati As CapitolatoCliente_DatiGlobale, tipoOperazione As String, username As String) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim ritornaGlobale As Boolean = False
        Dim ritornaTemp As Boolean = True

        Dim result_DatiTestata
        Dim result_DatiAziende
        Dim result_DatiVarieta
        Dim result_DatiSAVietate
        Dim result_DatiLMREccezioni
        Dim result_DatiLMRUlteriori

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            result_DatiTestata = SalvaCapitolatoCliente(piva, idCapitolato, dati.DatiTestata, tipoOperazione, _objParametriServer, username)

            'Passo al salvataggio seguente solo se ritornaTemp è TRUE
            ritornaTemp = ritornaTemp AndAlso result_DatiTestata
            If ritornaTemp = True Then
                result_DatiAziende = SalvaClienteXImprese(piva, idCapitolato, dati.DatiAziende, _objParametriServer)
                ritornaTemp = ritornaTemp AndAlso result_DatiAziende
            End If

            If ritornaTemp = True Then
                result_DatiVarieta = SalvaElencoVarieta(piva, idCapitolato, dati.DatiVarieta, _objParametriServer, username)
                ritornaTemp = ritornaTemp AndAlso result_DatiVarieta
            End If

            If ritornaTemp = True Then
                result_DatiSAVietate = SalvaElencoSostanzeAttiveVietate(idCapitolato, dati.DatiSAVietate, _objParametriServer, username)
                ritornaTemp = ritornaTemp AndAlso result_DatiSAVietate
            End If

            If ritornaTemp = True Then
                result_DatiLMREccezioni = SalvaElencoLMR(idCapitolato, dati.DatiLMREccezioni, _objParametriServer, partecipaSommatoria:=1, username)
                ritornaTemp = ritornaTemp AndAlso result_DatiLMREccezioni
            End If

            If ritornaTemp = True Then
                result_DatiLMRUlteriori = SalvaElencoLMR(idCapitolato, dati.DatiLMRUlteriori, _objParametriServer, partecipaSommatoria:=0, username)
                ritornaTemp = ritornaTemp AndAlso result_DatiLMRUlteriori
            End If

            ritornaGlobale = ritornaTemp

            Utility.VerificaChiudiTransazione(_objParametriServer, flagConnessione)

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)

            Throw New Exception("[ CapitolatoClienteBiz.SalvaTutto() ] : " & ex.Message)

        Finally

            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)

        End Try

        Return ritornaGlobale
    End Function

    Public Function SalvaClienteXImprese(piva As String, idCapitolato As String, DatiAziende As List(Of modelElencoAziende), objParamConn As AgronicaCoreParametri) As Boolean
        Dim ritorna As Boolean = False
        Dim CCXC As New List(Of modelElencoAziende)
        Dim result As Boolean = False
        Dim objParamConnUsaLocale As Boolean = False
        Dim objParamConnLocale As AgronicaCoreParametri = Nothing

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            ' - -se non passata una crea una connesione locale 
            If IsNothing(objParamConn) Then
                objParamConnUsaLocale = True
                objParamConnLocale = _objParametriServer
            Else
                objParamConnUsaLocale = False
                objParamConnLocale = objParamConn
            End If

            Dim capCliXCulW As New AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXImprese_W
            ' -- cancelal tutte le righe 
            result = capCliXCulW.cancella(piva, idCapitolato, objParamConnLocale)
            If DatiAziende IsNot Nothing Then
                If result Then
                    For Each azienda As modelElencoAziende In DatiAziende
                        If Not IsNothing(azienda.Piva) And Not (azienda.Piva.Equals("")) Then
                            Dim CCXCrec As New modelElencoAziende

                            CCXCrec.Piva = azienda.Piva
                            CCXCrec.Capitolato_COD = azienda.Capitolato_COD
                            CCXCrec.PivaSuperUser = azienda.PivaSuperUser

                            CCXC.Add(CCXCrec)
                        End If
                    Next
                    ' -- ora esegue la scrittura di tutte le righe 
                    ritorna = capCliXCulW.Scrivi(CCXC, objParamConnLocale)
                End If

            Else
                ritorna = True
            End If

            If ritorna = False Then
                Throw New Exception("Si è verificato un errore.")
            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagConnessione)

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)

            Throw New Exception("[ CapitolatoClienteBiz.SalvaClienteXImprese() ] : " & ex.Message)

        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return ritorna
    End Function
    Public Function SalvaElencoVarieta(piva As String, idCapitolato As String, DatiVarieta As List(Of modelElencoVarieta), objParamConn As AgronicaCoreParametri, username As String) As Boolean
        Dim ritorna As Boolean = False
        Dim CCXC As New List(Of CapitolatoClienteXCultivar)
        Dim result As Boolean = False

        Dim objParamConnUsaLocale As Boolean = False
        Dim objParamConnLocale As AgronicaCoreParametri = Nothing


        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            ' - -se non passata una crea una connesione locale 
            If IsNothing(objParamConn) Then
                objParamConnUsaLocale = True
                objParamConnLocale = _objParametriServer
            Else
                objParamConnUsaLocale = True
                objParamConnLocale = objParamConn
            End If


            Dim capCliXCulW As New AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXCultivar_W()
            '-- cancella tutte le righe 
            result = capCliXCulW.cancella(idCapitolato, "", objParamConnLocale)
            If DatiVarieta IsNot Nothing AndAlso DatiVarieta.Count > 0 Then
                If result Then
                    For Each varieta As modelElencoVarieta In DatiVarieta
                        ' -- le righe vuote vengono saltate
                        If Not IsNothing(varieta.CodiceVarieta) Then
                            Dim CCXCrec As New CapitolatoClienteXCultivar

                            CCXCrec.Capitolato_COD = idCapitolato
                            CCXCrec.Cul_COD = varieta.CodiceVarieta
                            CCXCrec.datainvio = Date.Today
                            CCXCrec.Data_Creazione = Date.Today
                            CCXCrec.Data_Modifica = Date.Today
                            CCXCrec.inviato = 0
                            CCXCrec.Username_Creazione = username
                            CCXCrec.Username_Modifica = username
                            CCXCrec.Validita_Inizio = CostantiPersonalizzate.AGRODATAINIZIO
                            CCXCrec.Validita_Fine = CostantiPersonalizzate.AGRODATAFINE

                            CCXC.Add(CCXCrec)
                        End If
                    Next
                    ' -- ora esegue la scrittura di tutte le righe 
                    ritorna = capCliXCulW.Scrivi(CCXC, objParamConnLocale)
                End If
            Else
                ritorna = True
            End If

            If ritorna = False Then
                Throw New Exception("Si è verificato un errore.")
            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagConnessione)

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)

            Throw New Exception("[ CapitolatoClienteBiz.SalvaElencoVarieta() ] : " & ex.Message)

        Finally

            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)

        End Try

        Return ritorna
    End Function

    ''' <summary>
    ''' Salva dati tabella capitolato cliente e tutte le tabelle legate
    ''' in base a 'TipoOperazione' viene fatto un aggiornamento o un inserimento
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="idCapitolato"></param>
    ''' <param name="jsonDati"></param>
    ''' <param name="TipoOperazione">1=inserimento 2=aggiornamento</param>
    ''' <param name="objParamConn"></param>
    ''' <returns></returns>
    Private Function SalvaCapitolatoCliente(piva As String, idCapitolato As String, DatiTestata As SalvaCapitolatoClienteModel, TipoOperazione As String, objParamConn As AgronicaCoreParametri, username As String) As Boolean
        Dim ritorna As Boolean = False
        'Dim CCXC As New modelCapitolatoCliente
        Dim result As Boolean = True
        Dim flagConnessione As Boolean = False
        Dim objParamConnUsaLocale As Boolean = False
        Dim objParamConnLocale As AgronicaCoreParametri = Nothing

        Try
            ' - -se non passata una crea una connesione locale 
            If IsNothing(objParamConn) Then
                objParamConnUsaLocale = True
                objParamConnLocale = _objParametriServer
            Else
                objParamConnUsaLocale = True
                objParamConnLocale = objParamConn
            End If

            Dim CCXC As New CapitolatoCliente
            ' -- legge riga originale e sovrascrive i valori provenienti dal frontend
            CCXC = LeggiCapitolatoCliente(piva, idCapitolato)

            CCXC.Capitolato_COD = DatiTestata.Capitolato_COD
            CCXC.Capitolato_DES = DatiTestata.Capitolato_DES

            CCXC.Flag_Privato_Pubblico_DPI = If((DatiTestata.Flag_Privato_Pubblico_DPI = 0), Nothing, (DatiTestata.Flag_Privato_Pubblico_DPI))

            If (CCXC.Flag_Privato_Pubblico_DPI Is Nothing) Then
                CCXC.DPI_COD_REGOLAMENTO = Nothing
            Else
                CCXC.DPI_COD_REGOLAMENTO = CInt(DatiTestata.DPI_COD_REGOLAMENTO)
            End If

            CCXC.N_Max_PA = DatiTestata.N_Max_PA ' If(DatiTestata.N_Max_PA Is Nothing, Nothing, CInt(DatiTestata.N_Max_PA))
            CCXC.Perc_Max_RMA = DatiTestata.Perc_Max_RMA 'If(DatiTestata.Perc_Max_RMA Is Nothing, Nothing, CDec(DatiTestata.Perc_Max_RMA))
            CCXC.Sum_Perc_Max_RMA = DatiTestata.Sum_Perc_Max_RMA ' If(DatiTestata.Sum_Perc_Max_RMA Is Nothing, Nothing, CDec(DatiTestata.Sum_Perc_Max_RMA))

            CCXC.Piva_SuperUser = piva
            CCXC.Attivo = Convert.ToInt32(DatiTestata.Attivo)
            CCXC.DPI_Impianto = Convert.ToInt32(DatiTestata.DPI_Impianto)
            'RecordModi.inviato = Convert.ToInt32(RecordOriginale.inviato)
            'RecordModi.datainvio = Convert.ToDateTime(RecordOriginale.datainvio)
            If TipoOperazione = "1" Then
                CCXC.Data_Creazione = Date.Now
                CCXC.Username_Creazione = username
            End If
            CCXC.Data_Modifica = Date.Now
            CCXC.Username_Modifica = username

            CCXC.Validita_Inizio = If(DatiTestata.Validita_Inizio Is Nothing, AGRODATAINIZIO, Convert.ToDateTime(DatiTestata.Validita_Inizio))
            CCXC.Validita_Fine = If(DatiTestata.Validita_Fine Is Nothing, AGRODATAFINE, Convert.ToDateTime(DatiTestata.Validita_Fine))

            CCXC.GestioneVarieta = Convert.ToInt32(DatiTestata.GestioneVarieta)
            CCXC.veg_cod = Convert.ToInt32(DatiTestata.veg_cod)

            CCXC.TabellaRMA_COD = If(DatiTestata.TabellaRMA_COD.Equals(""), 0, Convert.ToInt32(DatiTestata.TabellaRMA_COD))

            'CCXC.TabellaRMA_COD = Convert.ToInt32(DatiTestata.TabellaRMA_COD)
            CCXC.Capitolato_DES_Breve = DatiTestata.Capitolato_DES_Breve
            CCXC.FlagBIO = If((DatiTestata.FlagBIO Is Nothing), 0, (DatiTestata.FlagBIO))
            CCXC.GestionePartecipazioneAziende = If((DatiTestata.GestionePartecipazioneAziende Is Nothing), 0, CInt(DatiTestata.GestionePartecipazioneAziende))
            CCXC.AttivaVerificaFormulatoValido = If((DatiTestata.AttivaVerificaFormulatoValido Is Nothing), 1, CInt(DatiTestata.AttivaVerificaFormulatoValido))
            CCXC.Capitolato_Codice = DatiTestata.Capitolato_Codice




            Dim capTestate As New AgronicaCoreCapitolatoClienteDAL.Capitolato_W
            Select Case TipoOperazione
                Case "1"    ' Nuovo inserimento
                    ' -- ora esegue la scrittura di tutte le righe 
                    ritorna = capTestate.Scrivi(CCXC, objParamConnLocale)

                Case "2"    ' Modifica
                    ' -- ora esegue la scrittura di tutte le righe 
                    ritorna = capTestate.Aggiorna(idCapitolato, CCXC, objParamConnLocale)
            End Select


        Catch ex As Exception

            Throw New Exception("[ CapitolatoClienteBiz.SalvaCapitolatoCliente() ] : " & ex.Message)

        End Try

        Return ritorna
    End Function





    Public Function GetElencoSostanzeAttive(ByVal capitolato_cod As Integer,
                                            ByVal leggiSingoloElemento As Boolean) As List(Of modelElencoSostanzeAttive)
        Dim flagConnessione As Boolean = False

        Dim objSostanzeAttiveList As New List(Of modelElencoSostanzeAttive)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim rCapCli = New AgronicaCoreCapitolatoClienteDAL.Capitolato_R
            Dim dt = rCapCli.LeggiElencoSostanzeAttive(capitolato_cod, leggiSingoloElemento, _objParametriServer)

            For Each row As DataRow In dt.Rows
                objSostanzeAttiveList.Add(New modelElencoSostanzeAttive() With {
                    .CodiceSostanzaAttiva = row.ToIntero("CodiceSostanzaAttiva"),
                    .Descrizione = row.ToStringa("Descrizione"),
                    .Tipo = row.ToIntero("Tipo")
                })
            Next

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.GetElencoSostanzeAttive() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objSostanzeAttiveList

    End Function

    Public Function SalvaElencoSostanzeAttiveVietate(idCapitolato As String,
                                                     DatiSAVietate As List(Of modelElencoSostanzeAttive),
                                                     objParamConn As AgronicaCoreParametri,
                                                     username As String) As Boolean
        Dim ritorna As Boolean = False
        Dim CCXPAR As New List(Of CapitolatoClienteXPrincipiAttiviRilevati)
        Dim CCXFPAR As New List(Of CapitolatoClienteXFamigliePrincipiAttiviRilevati)
        Dim result As Boolean = False
        Dim objParamConnUsaLocale As Boolean = False
        Dim objParamConnLocale As AgronicaCoreParametri = Nothing

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)


            ' - -se non passata una crea una connesione locale 
            If IsNothing(objParamConn) Then
                objParamConnUsaLocale = True
                objParamConnLocale = _objParametriServer
            Else
                objParamConnUsaLocale = True
                objParamConnLocale = objParamConn
            End If


            Dim CCXPAR_W As New AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXPrincipiAttiviRilevati_W()
            Dim CCXFPAR_W As New AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXFamigliePrincipiAttiviRilevati_W()

            ' -- cancella tutte le righe 
            result = CCXPAR_W.Cancella(idCapitolato, objParamConnLocale)
            result = CCXFPAR_W.Cancella(idCapitolato, objParamConnLocale)
            If DatiSAVietate IsNot Nothing Then
                If result Then
                    For Each SAVietata As modelElencoSostanzeAttive In DatiSAVietate
                        ' -- le righe vuote vengono saltate
                        If Not IsNothing(SAVietata.CodiceSostanzaAttiva) Then
                            If SAVietata.Tipo = 1 Then 'PRINCIPI ATTIVI
                                Dim CCXPAR_EF As New CapitolatoClienteXPrincipiAttiviRilevati

                                CCXPAR_EF.Capitolato_COD = idCapitolato
                                CCXPAR_EF.PA_COD = SAVietata.CodiceSostanzaAttiva
                                CCXPAR_EF.LMR = 0 'perchè vietato

                                CCXPAR_EF.inviato = 0
                                CCXPAR_EF.datainvio = Date.Today

                                CCXPAR_EF.Data_Creazione = Date.Today
                                CCXPAR_EF.Data_Modifica = Date.Today
                                CCXPAR_EF.Username_Creazione = username
                                CCXPAR_EF.Username_Modifica = username
                                CCXPAR_EF.Validita_Inizio = CostantiPersonalizzate.AGRODATAINIZIO
                                CCXPAR_EF.Validita_Fine = CostantiPersonalizzate.AGRODATAFINE

                                CCXPAR_EF.PartecipaSommatoria = Nothing
                                CCXPAR.Add(CCXPAR_EF)

                            ElseIf SAVietata.Tipo = 0 Then 'FAMIGLIE PRINCIPI ATTIVI
                                Dim CCXFPAR_EF As New CapitolatoClienteXFamigliePrincipiAttiviRilevati

                                CCXFPAR_EF.Capitolato_COD = idCapitolato
                                CCXFPAR_EF.FAM_COD = SAVietata.CodiceSostanzaAttiva
                                CCXFPAR_EF.LMR = 0 'perchè vietato

                                CCXFPAR_EF.inviato = 0
                                CCXFPAR_EF.datainvio = Date.Today

                                CCXFPAR_EF.Data_Creazione = Date.Today
                                CCXFPAR_EF.Data_Modifica = Date.Today
                                CCXFPAR_EF.Username_Creazione = username
                                CCXFPAR_EF.Username_Modifica = username
                                CCXFPAR_EF.Validita_Inizio = CostantiPersonalizzate.AGRODATAINIZIO
                                CCXFPAR_EF.Validita_Fine = CostantiPersonalizzate.AGRODATAFINE

                                CCXFPAR_EF.PartecipaSommatoria = Nothing
                                CCXFPAR.Add(CCXFPAR_EF)
                            End If
                        End If
                    Next
                    ' -- ora esegue la scrittura di tutte le righe 
                    ritorna = CCXPAR_W.Scrivi(CCXPAR, objParamConnLocale)
                    ritorna = CCXFPAR_W.Scrivi(CCXFPAR, objParamConnLocale)

                End If
            Else
                ritorna = True
            End If

            If ritorna = False Then
                Throw New Exception("Si è verificato un errore.")
            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagConnessione)

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)

            Throw New Exception("[ CapitolatoClienteBiz.SalvaElencoSostanzeAttiveVietate() ] : " & ex.Message)

        Finally

            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)

        End Try

        Return ritorna
    End Function

    Public Function GetElencoLMR(ByVal capitolato_cod As Integer,
                                 ByVal leggiSingoloElemento As Boolean,
                                 ByVal partecipa As Integer) As List(Of modelElencoLMR)
        Dim flagConnessione As Boolean = False

        Dim objLMRList As New List(Of modelElencoLMR)

        Try
            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            Dim rCapCli = New AgronicaCoreCapitolatoClienteDAL.Capitolato_R
            Dim dt = rCapCli.LeggiElencoSostanzeAttive(capitolato_cod, leggiSingoloElemento, _objParametriServer, partecipa)

            For Each row As DataRow In dt.Rows

                If partecipa <> -1 Then
                    objLMRList.Add(New modelElencoLMR() With {
                    .CodiceSostanzaAttiva = row.ToIntero("CodiceSostanzaAttiva"),
                    .Descrizione = row.ToStringa("Descrizione"),
                    .Tipo = row.ToIntero("Tipo"),
                    .partecipaSommatoria = row.ToIntero("partecipaSommatoria"),
                    .maxLMR = row.ToIntero("maxLMR")
                                   })
                Else
                    objLMRList.Add(New modelElencoLMR() With {
                    .CodiceSostanzaAttiva = row.ToIntero("CodiceSostanzaAttiva"),
                    .Descrizione = row.ToStringa("Descrizione"),
                    .Tipo = row.ToIntero("Tipo")
                                   })
                End If
            Next

        Catch ex As Exception
            Throw New Exception("[ CapitolatoClienteBiz.GetElencoSostanzeAttive() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objLMRList

    End Function

    Public Function SalvaElencoLMR(idCapitolato As String,
                                   DatiLMR As List(Of modelElencoLMR),
                                   objParamConn As AgronicaCoreParametri,
                                   partecipaSommatoria As Integer,
                                   username As String) As Boolean
        Dim ritorna As Boolean = False
        Dim CCXPAR As New List(Of CapitolatoClienteXPrincipiAttiviRilevati)
        Dim CCXFPAR As New List(Of CapitolatoClienteXFamigliePrincipiAttiviRilevati)
        Dim result As Boolean = False
        Dim objParamConnUsaLocale As Boolean = False
        Dim objParamConnLocale As AgronicaCoreParametri = Nothing

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            ' - -se non passata una crea una connesione locale 
            If IsNothing(objParamConn) Then
                objParamConnUsaLocale = True
                objParamConnLocale = _objParametriServer
            Else
                objParamConnUsaLocale = True
                objParamConnLocale = objParamConn
            End If


            Dim CCXPAR_W As New AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXPrincipiAttiviRilevati_W()
            Dim CCXFPAR_W As New AgronicaCoreCapitolatoClienteDAL.CapitolatoClienteXFamigliePrincipiAttiviRilevati_W()
            ' -- cancella tutte le righe 
            result = CCXPAR_W.Cancella(idCapitolato, objParamConnLocale, partecipaSommatoria:=True, partecipaSommatoria)
            result = CCXFPAR_W.Cancella(idCapitolato, objParamConnLocale, partecipaSommatoria:=True, partecipaSommatoria)

            If DatiLMR IsNot Nothing Then
                If result Then
                    For Each LMR As modelElencoLMR In DatiLMR
                        ' -- le righe vuote vengono saltate
                        If Not IsNothing(LMR.CodiceSostanzaAttiva) Then
                            If LMR.Tipo = 1 Then 'PRINCIPI ATTIVI
                                Dim CCXPAR_EF As New CapitolatoClienteXPrincipiAttiviRilevati

                                CCXPAR_EF.Capitolato_COD = idCapitolato
                                CCXPAR_EF.PA_COD = LMR.CodiceSostanzaAttiva
                                CCXPAR_EF.LMR = LMR.maxLMR

                                CCXPAR_EF.inviato = 0
                                CCXPAR_EF.datainvio = Date.Today

                                CCXPAR_EF.Data_Creazione = Date.Today
                                CCXPAR_EF.Data_Modifica = Date.Today
                                CCXPAR_EF.Username_Creazione = username
                                CCXPAR_EF.Username_Modifica = username
                                CCXPAR_EF.Validita_Inizio = CostantiPersonalizzate.AGRODATAINIZIO
                                CCXPAR_EF.Validita_Fine = CostantiPersonalizzate.AGRODATAFINE

                                CCXPAR_EF.PartecipaSommatoria = partecipaSommatoria
                                CCXPAR.Add(CCXPAR_EF)
                            ElseIf LMR.Tipo = 0 Then 'FAMIGLIE PRINCIPI ATTIVI
                                Dim CCXFPAR_EF As New CapitolatoClienteXFamigliePrincipiAttiviRilevati

                                CCXFPAR_EF.Capitolato_COD = idCapitolato
                                CCXFPAR_EF.FAM_COD = LMR.CodiceSostanzaAttiva
                                CCXFPAR_EF.LMR = LMR.maxLMR

                                CCXFPAR_EF.inviato = 0
                                CCXFPAR_EF.datainvio = Date.Today

                                CCXFPAR_EF.Data_Creazione = Date.Today
                                CCXFPAR_EF.Data_Modifica = Date.Today
                                CCXFPAR_EF.Username_Creazione = username
                                CCXFPAR_EF.Username_Modifica = username
                                CCXFPAR_EF.Validita_Inizio = CostantiPersonalizzate.AGRODATAINIZIO
                                CCXFPAR_EF.Validita_Fine = CostantiPersonalizzate.AGRODATAFINE

                                CCXFPAR_EF.PartecipaSommatoria = partecipaSommatoria
                                CCXFPAR.Add(CCXFPAR_EF)
                            End If
                        End If
                    Next
                    ' -- ora esegue la scrittura di tutte le righe 
                    ritorna = CCXPAR_W.Scrivi(CCXPAR, objParamConnLocale)
                    ritorna = CCXFPAR_W.Scrivi(CCXFPAR, objParamConnLocale)

                End If
            Else
                ritorna = True
            End If

            If ritorna = False Then
                Throw New Exception("Si è verificato un errore.")
            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagConnessione)

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)

            Throw New Exception("[ CapitolatoClienteBiz.SalvaElencoSostanzeAttiveVietate() ] : " & ex.Message)

        Finally

            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)

        End Try

        Return ritorna
    End Function

End Class
