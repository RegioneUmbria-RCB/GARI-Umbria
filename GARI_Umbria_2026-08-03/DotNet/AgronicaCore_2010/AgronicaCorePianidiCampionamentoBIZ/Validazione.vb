Imports System.Web
Imports AgronicaCoreWebService.AgroWs

Imports <xmlns="http://ws_CapitolatoCliente_VerificaAnalisi">


Public Class Validazione
    
    ''' <summary>
    ''' invio l'XML passato al WebService per la validazione
    ''' </summary>
    ''' <param name="xml"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function inviaXmlpervalidazione_al_WebService(ByVal xml As String)

        'TODO: REFACTOR = Togli uso Session!!!
        Dim objParametriUtenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim ObjDownloadWs As New WS_CapitolatoCliente.AgroWS_CapitolatoCliente
        'ObjDownloadWs.Timeout = System.Configuration.ConfigurationManager.AppSettings("ws_timeout")

        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

        Dim objAgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        objWs.NewWS(ObjDownloadWs,
                    objAgroWebConfig.GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente,
                    objParametriUtenti)

        Dim xmlDoc As Xml.XmlDocument

        Dim strCredenziali As String

        Dim objVariabiliSessione As New AgronicaCoreGestioneRichieste.VariabiliSessione

        Try

            xmlDoc = New Xml.XmlDocument
            Dim objWSUtility As New AgronicaCoreWebService.AgroWs

            objWSUtility.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                 strCredenziali,
                                                 enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi,
                                                 objWSUtility.AgroWS_DoorKey(enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi),
                                                 objVariabiliSessione.ASG_ProgressivoGIAS,
                                                 objVariabiliSessione.ASG_SuperUser_Username,
                                                 objVariabiliSessione.ASG_SuperUser_Password)

            xmlDoc.LoadXml(strCredenziali)

            Dim risultati As String

            ObjDownloadWs.Timeout = "600000"

            'aggiungo quelli Pubblici
            risultati = ObjDownloadWs.verifica_Analisi(xml)
            risultati = objWSUtility.AWS_Decodifica_R(risultati)
            risultati = risultati.Replace(">", ">" & vbCrLf)

            'prova Carico con Linq
            Return risultati

        Catch ex As Exception
            Throw New Exception("Errore nella comunicazione con il WebService al caricamento dei DPI privati e pubblici ")
        End Try

    End Function

    ''' <summary>
    ''' Genera l'XML da inviare al WebService
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getXmlPerValidazione_SingoloCodice_Limite_Legge(ByVal ID_PDC_Testata As Integer,
                                                                           ByVal PDC_Data_Istantanea As Date,
                                                                           ByVal codice_analisi As String)

        'per ogni item del List_Codici_Analisi identifico il controllo dpi da inviare
        Dim listAnalisi As New List(Of ObjAnalisi)

        'Dim objPdcSoloAnalisi As New AgronicaCorePianidiCampionamentoBiz.PDC_Testata
        'objPdcSoloAnalisi = HttpContext.Current.Session("_PDC_Testata")

        'mi ricavo tutti i dettagli delle analisi
        '   (Analisi_Testata_Cod CUL_COD     Veg_Cod     Analisi_Parametro_Cod Analisi_Dettaglio_Valore_1 Analisi_Dettaglio_Valore_2 Analisi_Dettaglio_MargineErrore_1 Disciplinare_Cod)
        Dim objAnalisiCore As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_R
        'TODO: REFACTOR = Togli uso Session!!!
        Dim dt As DataTable = objAnalisiCore.Leggi_x_controlloDP(ID_PDC_Testata,
                                                                 codice_analisi,
                                                                 PDC_Data_Istantanea,
                                                                 "", "",
                                                                 HttpContext.Current.Session("ASG_objParametri_Server"))


        'aggiungo i rispetti di legge
        Dim lCapCli As New List(Of ObjCapitolatoCliente)
        Dim app As New ObjCapitolatoCliente With {
            .Des = "Limiti di Legge",
            .ID = 0
        }
        lCapCli.Add(app)

        Dim xmlDoc As XDocument

        'intestazione con lo schema corretto
        'If System.Diagnostics.Debugger.IsAttached Then
        '    xmlDoc = <?xml version="1.0"?>
        '             <root xmlns="http://ws_CapitolatoCliente_VerificaAnalisi">
        '                 <debug/>
        '             </root>
        'Else
        '    xmlDoc = <?xml version="1.0"?>
        '             <root xmlns="http://ws_CapitolatoCliente_VerificaAnalisi">
        '             </root>

        'End If

        xmlDoc = <?xml version="1.0"?>
                 <root xmlns="http://ws_CapitolatoCliente_VerificaAnalisi">
                 </root>





        Dim dr() As DataRow
        'per ogni analisi

        dr = dt.Select("Analisi_Testata_Cod = " & codice_analisi)

        If dr.Length > 0 Then
            'creo un nuovo oggetto da aggiungere alla lista dei prodotti
            Dim objVer As New ObjVerifiche With {
                .listCapitolatiCliente = lCapCli
            }



            Dim disciplinare As String
            If dr(0).Item("Disciplinare_Cod") = "0" Then
                disciplinare = -1
            Else
                disciplinare = dr(0).Item("Disciplinare_Cod").Split("/")(1)
            End If

            'aggiungo i dati su Specie e Varietà 
            Dim objDati As New ObjDatiInput With {
                .specie = dr(0).Item("Veg_Cod"),
                .varieta = dr(0).Item("Cul_Cod"),
                .DPI_Cod_Regolamento = disciplinare,
                .listaPA = New List(Of ObjPrincipiAttivi),
                .listaFam = New List(Of ObjFamiglie)
            }

            'Aggiungo le informazioni sui principi attivi riscontrati
            For j As Integer = 0 To dr.Length - 1
                If Not IsDBNull(dr(j).Item("Analisi_Parametro_Cod")) Then
                    'controllo se è > 0
                    If dr(j).Item("Analisi_Parametro_Cod") > 0 Then
                        Dim oPA As New ObjPrincipiAttivi With {
                            .paCod = dr(j).Item("Analisi_Parametro_Cod"),
                            .qtaRilevata = If(2151 = dr(j).Item("Analisi_Dettaglio_Valore_2"),
                                              dr(j).Item("Analisi_Dettaglio_Valore_1"),
                                              dr(j).Item("Analisi_Dettaglio_Valore_1") * 1000)
                        }
                        '.qtaRilevataUdmGIAS = dr(j).Item("Analisi_Dettaglio_Valore_2")
                        objDati.listaPA.Add(oPA)
                    Else
                        Dim oFa As New ObjFamiglie With {
                            .FamCod = 0 - dr(j).Item("Analisi_Parametro_Cod"),
                            .qtaRilevata = If(2151 = dr(j).Item("Analisi_Dettaglio_Valore_2"),
                                              dr(j).Item("Analisi_Dettaglio_Valore_1"),
                                              dr(j).Item("Analisi_Dettaglio_Valore_1") * 1000)
                        }
                        '.qtaRilevataUdmGIAS = dr(j).Item("Analisi_Dettaglio_Valore_2")



                        objDati.listaFam.Add(oFa)
                    End If
                End If
            Next

            'TODO: REFACTOR = Togli uso Session!!!
            Dim oAnalisi As New ObjAnalisi With {
               .id = dr(0).Item("Analisi_Testata_Cod"),
               .dataAnalisi = dr(0).Item("Analisi_Testata_Data_Inizio"),
               .pivaSuperUser = HttpContext.Current.Session("ASG_objParametri_Server").PivaSuperUser,
               .verifiche = objVer,
               .datiInput = objDati
            }
            xmlDoc.<root>.FirstOrDefault.Add(oAnalisi.getXml())
        End If

        Return xmlDoc.ToString

    End Function
    

    ''' <summary>
    ''' Spacchetto l'XML di risposta dal WebService 
    ''' </summary>
    ''' <param name="xmlStr"></param>
    ''' <remarks></remarks>
    Public Shared Function SpacchettaRisposta(ByVal xmlStr As String)
         
        xmlStr = Replace(xmlStr, Chr(13), "")
        xmlStr = Replace(xmlStr, Chr(10), "")
 
        'lavoro l'xml che mi è arrivato
        Dim xmlDpi As XElement = XElement.Load(New IO.StringReader(xmlStr))

        Dim dpiAn = (
            From analysis In xmlDpi.<analisi>
            Select New ObjAnalisi With {
                .pivaSuperUser = analysis.@pivaSuperUser,
                .id = analysis.@id,
                .verifiche = (
                    From verify In analysis.<verifiche>
                    Select New ObjVerifiche With {
                        .listCapitolatiCliente = (
                            From listCapCli In verify.<capitolatiCliente>.<capitolatoCliente>
                            Select New ObjCapitolatoCliente With {
                                            .ID = listCapCli.<id>.Value,
                                            .esitoCultivarProibite = CBool(listCapCli.<cultivarNonAmmesse>.Value.Trim.ToLower()),
                                            .listDPI = New ObjDPI With {
                                                        .ID = listCapCli.<dpiCapitolatoImpianto>.<dpiCodRegolamento>.Value,
                                                        .Des = listCapCli.<dpiCapitolatoImpianto>.<dpiDescrizione>.Value,
                                                        .Privato = listCapCli.<dpiCapitolatoImpianto>.<dpiFlagPrivatoPubblico>.Value,
                                                        .warningDPI = listCapCli.<dpiCapitolatoImpianto>.<warning>.ToList.Count > 0,
                                                        .NoCheckDP = listCapCli.<dpiCapitolatoImpianto>.<noCheckDP>.ToList.Count > 0,
                                                        .Response = (
                                                                    From dpResp In listCapCli.<dpiCapitolatoImpianto>.<dpiResponse>
                                                                    Select New ObjResponse With {
                                                                        .esitoGlobale = CBool(dpResp.<esitoGlobale>.Value.Trim.ToLower),
                                                                        .singoliPrincipiAttiviNonConformi = (
                                                                            From d In dpResp.<principiAttiviNonConformi>.<principioAttivoNonConforme>
                                                                            Select New ObjPrincipiAttivi With {
                                                                                .esitoSoloDP = If(d.<esitoSoloDP>.ToList.Count = 0, False, True),
                                                                                .paCod = d.<id>.Value,
                                                                                .percentuale = If(d.<rma>.ToList.Count > 0, d.<rma>.Value, -2),
                                                                                .checkFormulato = CBool(d.<checkFormulati>.Value),
                                                                                .isFamiglia = False
                                                                                }).ToList(),
                                                                        .famigliePrincipiAttiviNonConformi = (
                                                                            From d In dpResp.<FamiglieDiPrincipiAttiviNonConformi>.<FamigliaDiPrincipioAttivoNonConforme>
                                                                            Select New ObjPrincipiAttivi With {
                                                                                .esitoSoloDP = If(d.<esitoSoloDP>.ToList.Count = 0, False, True),
                                                                                .paCod = d.<id>.Value,
                                                                                .percentuale = If(d.<rma>.ToList.Count > 0, d.<rma>.Value, -2),
                                                                                .checkFormulato = CBool(d.<checkFormulati>.Value),
                                                                                .isFamiglia = True
                                                                                }).ToList()
                                                            }).FirstOrDefault()
                                                        },
                                                        .capitolatoResponse = (
                                                            From capCliResponse In listCapCli.<capitolatoResponse>
                                                            Select New ObjCapitolatoResponse With {
                                                                .esitoGlobale = CBool(capCliResponse.<esitoGlobale>.Value.Trim().ToLower()),
                                                                .esitoNumeroPrincipiAttivi = CBool(If(capCliResponse.<esitoNumeroPrincipiAttivi>.Value.Trim() <> "", capCliResponse.<esitoNumeroPrincipiAttivi>.Value.Trim().ToLower(), Nothing)),
                                                                .esitoPercentualeMax = CBool(If(capCliResponse.<esitoPercentualeMax>.<percSuperata>.Value.Trim() <> "", capCliResponse.<esitoPercentualeMax>.<percSuperata>.Value.Trim().ToLower(), Nothing)),
                                                                .sumQtaRilevata = CDbl(capCliResponse.<esitoPercentualeMax>.<sumQtaRilevata>.Value.Trim()),
                                                                .listSingoliPAnonConformi = (
                                                                        From badPA In capCliResponse.<principioAttivoNonConforme>
                                                                        Select New ObjPrincipiAttivi With {
                                                                            .paCod = badPA.<id>.Value,
                                                                            .percentuale = If(badPA.<percentuale>.ToList.Count > 0, badPA.<percentuale>.Value, -1),
                                                                            .rma = badPA.<rma>.Value,
                                                                            .checkFormulato = CBool(badPA.<checkFormulati>.Value),
                                                                            .isFamiglia = False,
                                                                            .regolamento = badPA.<Regolamento>.Value
                                                                        }).ToList(),
                                                                .listFAMnonConformi = (
                                                                        From badPA In capCliResponse.<famigliaPrincipioAttivoNonConforme>
                                                                        Select New ObjPrincipiAttivi With {
                                                                            .paCod = badPA.<id>.Value,
                                                                            .percentuale = If(badPA.<percentuale>.ToList.Count > 0, badPA.<percentuale>.Value, -1),
                                                                            .rma = badPA.<rma>.Value,
                                                                            .checkFormulato = CBool(badPA.<checkFormulati>.Value),
                                                                            .isFamiglia = True,
                                                                            .regolamento = badPA.<Regolamento>.Value
                                                                        }).ToList()
                                                            }).FirstOrDefault()
                                            }).ToList()
                                    }).FirstOrDefault()
                            }).ToList()
        
        Return dpiAn

    End Function

    ''' <summary>
    ''' invio l'XML passato al WebService per la validazione
    ''' </summary>
    ''' <param name="xml"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function inviaXmlperVerificaAziendeXCapitolati_al_WebService(ByVal xml As String)

        'TODO: REFACTOR = Togli uso Session!!!
        Dim objParametriUtenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim ObjDownloadWs As New WS_CapitolatoCliente.AgroWS_CapitolatoCliente
        'ObjDownloadWs.Timeout = System.Configuration.ConfigurationManager.AppSettings("ws_timeout")

        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

        Dim objAgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        objWs.NewWS(ObjDownloadWs,
                    objAgroWebConfig.GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente,
                    objParametriUtenti)

        Dim xmlDoc As Xml.XmlDocument

        Dim strCredenziali As String

        Dim objVariabiliSessione As New AgronicaCoreGestioneRichieste.VariabiliSessione

        Try

            xmlDoc = New Xml.XmlDocument
            Dim objWSUtility As New AgronicaCoreWebService.AgroWs

            objWSUtility.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                 strCredenziali,
                                                 enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi,
                                                 objWSUtility.AgroWS_DoorKey(enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi),
                                                 objVariabiliSessione.ASG_ProgressivoGIAS,
                                                 objVariabiliSessione.ASG_SuperUser_Username,
                                                 objVariabiliSessione.ASG_SuperUser_Password)

            xmlDoc.LoadXml(strCredenziali)

            Dim risultati As String

            ObjDownloadWs.Timeout = "600000"

            'aggiungo quelli Pubblici
            risultati = ObjDownloadWs.ElaboraListaImpreseEscluseDaCapitolati(xml)
            risultati = objWSUtility.AWS_Decodifica_R(risultati)
            risultati = risultati.Replace(">", ">" & vbCrLf)

            'prova Carico con Linq
            Return risultati

        Catch ex As Exception
            Throw New Exception("Errore nella comunicazione con il WebService al caricamento dei DPI privati e pubblici ")
        End Try

    End Function

    Public Function Ottieni_LimitiMerceologica(ByVal piva As String
                                               ) As List(Of LimiteMerceologica)

        Const nomeRoutine = "Ottieni_LimitiMerceologica()"
        Dim limiti As New List(Of LimiteMerceologica)

        Try

            Select Case piva
                Case Impostazioni_PDC.PivaZESPRI

                    limiti.Add(New LimiteMerceologica With {
                                  .Parametro = LimiteMerceologica.PARAM_SOSTANZA_SECCA,
                                  .PuntoPrelievo = LimiteMerceologica.ZESPRI_PTO_ORCHARD,
                                  .Limite = 15.95D
                                })

                    limiti.Add(New LimiteMerceologica With {
                                  .Parametro = LimiteMerceologica.PARAM_SOSTANZA_SECCA,
                                  .PuntoPrelievo = LimiteMerceologica.ZESPRI_PTO_PACKHOUSE,
                                  .Limite = 16.45D
                                })

                Case Else
                    Throw New NotImplementedException("Limiti Merceologica non impostati")
            End Select

        Catch ex As Exception
            Throw new Exception("[" & nomeRoutine & "]: " & ex.Message)
        End Try

        Return limiti
    End Function

End Class


''++++++++++++++++++++++++++++++++++++++'
Public Class ObjAnalisi

    Public pivaSuperUser As String
    Public dataAnalisi As Date
    Public id As Integer
    Public datiInput As ObjDatiInput
    Public verifiche As ObjVerifiche
    Public pivaImpianto As String

    Public Function getXml() As XElement

        'vanni, new school
        Dim xEl = <analisi pivaSuperUser=<%= pivaSuperUser %> id=<%= id %> dataAnalisi=<%= dataAnalisi %> pivaImpianto=<%= pivaImpianto %>>
                      <datiInput>
                          <specie><%= datiInput.specie %></specie>
                          <varieta><%= datiInput.varieta %></varieta>
                          <dpiImpianto>
                              <dpiFlagPrivatoPubblico><%= datiInput.DPI_Flag_PrivatoPubblico %></dpiFlagPrivatoPubblico>
                              <dpiCodRegolamento><%= datiInput.DPI_Cod_Regolamento %></dpiCodRegolamento>
                          </dpiImpianto>
                      </datiInput>
                      <verifiche>
                          <capitolatiCliente></capitolatiCliente>
                      </verifiche>
                  </analisi>


        For Each c In datiInput.listaPA
            Dim coso = <principiAttivi>
                           <paCod><%= c.paCod %></paCod>
                           <qtaRilevata><%= c.qtaRilevata %></qtaRilevata>
                           <qtaRilevataUdmGIAS><%= c.qtaRilevataUdmGIAS %></qtaRilevataUdmGIAS>
                       </principiAttivi>
            xEl.<datiInput>.FirstOrDefault.Add(coso)
        Next

        For Each c In datiInput.listaFam
            Dim coso = <famigliePrincipiAttivi>
                           <famCod><%= c.FamCod %></famCod>
                           <qtaRilevata><%= c.qtaRilevata %></qtaRilevata>
                           <qtaRilevataUdmGIAS><%= c.qtaRilevataUdmGIAS %></qtaRilevataUdmGIAS>
                       </famigliePrincipiAttivi>
            xEl.<datiInput>.FirstOrDefault.Add(coso)
        Next

        For Each cc In verifiche.listCapitolatiCliente
            Dim coso = <capitolatoCliente>
                           <id><%= cc.ID %></id>
                       </capitolatoCliente>
            xEl.<verifiche>.<capitolatiCliente>.FirstOrDefault.Add(coso)
        Next

        Return xEl

    End Function

End Class


Public Class ObjDatiInput

    Public specie As Integer
    Public varieta As String
    Public DPI_Cod_Regolamento As String
    Public DPI_Flag_PrivatoPubblico As Integer = 1
    Public listaPA As List(Of ObjPrincipiAttivi)
    Public listaFam As List(Of ObjFamiglie)
    Public listaMerceologica As List(Of ObjMerceologica)

End Class

Public Class ObjPrincipiAttivi

    Public esitoSoloDP As Boolean
    Public paCod As Integer
    Public qtaRilevata As Decimal
    Public qtaRilevataUdmGIAS As Decimal
    Public regolamento As String

    Public isFamiglia As Boolean

    Public percentuale As Decimal
    Public rma As Decimal
    Public checkFormulato As Boolean
    Public percSuperata As Boolean

End Class

Public Class ObjFamiglie

    Public FamCod As Integer
    Public qtaRilevata As Decimal
    Public qtaRilevataUdmGIAS As Decimal

    Public rma As Decimal

End Class

Public Class ObjMerceologica

    Public ParametroCod As Integer
    Public qtaRilevata As Decimal
    Public qtaRilevataUdmGIAS As Decimal

    Public rma As Decimal

    Public PuntoPrelievo As Integer

    Public LimiteConsentito As Decimal

End Class

Public Class LimiteMerceologica

    Public Const PARAM_SOSTANZA_SECCA As Integer = 231
    Public Const ZESPRI_PTO_ORCHARD As Integer = 1
    Public Const ZESPRI_PTO_PACKHOUSE As Integer = 2

    Public Property Parametro As Integer
    Public Property PuntoPrelievo As Integer
    Public Property Limite As Decimal
End Class

Public Class ObjVerifiche
    Public listCapitolatiCliente As List(Of ObjCapitolatoCliente)
End Class

Public Class ObjCapitolatoCliente

    Public ID As Integer
    Public Des As String
    Public Sigla As String
    Public capitolatoResponse As ObjCapitolatoResponse
    Public esitoCultivarProibite As Boolean
    Public aziendaEsclusaDaCapitolato As Boolean
    Public elencoCultivarNonAmmesse As String
    Public listDPI As ObjDPI

    Public Function getValue() As String
        Return ID
    End Function

    Public Function getDescrizione() As String
        Return Des
    End Function

    Public Function getSigla() As String
        Return Sigla
    End Function

End Class

Public Class ObjResponse

    Public Sub New()
        singoliPrincipiAttiviNonConformi = New List(Of ObjPrincipiAttivi)
        famigliePrincipiAttiviNonConformi = New List(Of ObjPrincipiAttivi)
    End Sub

    Public esitoGlobale As Boolean
    Public singoliPrincipiAttiviNonConformi As List(Of ObjPrincipiAttivi)
    Public famigliePrincipiAttiviNonConformi As List(Of ObjPrincipiAttivi)

    Public ReadOnly Property principiAttiviNonConformi As List(Of ObjPrincipiAttivi)
        Get
            Dim lFinal As New List(Of ObjPrincipiAttivi)

            lFinal.AddRange(singoliPrincipiAttiviNonConformi)
            lFinal.AddRange(famigliePrincipiAttiviNonConformi)

            Return lFinal
        End Get

    End Property

End Class

Public Class ObjDPI

    Public ID As Integer
    Public Privato As Integer
    Public Des As String
    Public Response As ObjResponse
    Public warningDPI As Boolean
    Public NoCheckDP As Boolean = False

    Public Function getValue() As String
        Dim str As String = ""
        If Privato = True Then
            str = "Private_" & ID
        Else
            str = "Public_" & ID
        End If
        Return str
    End Function

    Public Function getDescrizione() As String
        Return Des
    End Function

End Class

Public Class ObjCapitolatoResponse

    Public Sub New()
        listSingoliPAnonConformi = New List(Of ObjPrincipiAttivi)
        listFAMnonConformi = New List(Of ObjPrincipiAttivi)
        listBioPAnonConformi = New List(Of ObjPrincipiAttivi)
        listBioFAMnonConformi = New List(Of ObjPrincipiAttivi)
        listMerceologichenonConformi = New List(Of ObjMerceologica)
    End SUb

    Public esitoGlobale As Boolean
    Public esitoNumeroPrincipiAttivi As Boolean
    Public esitoPercentualeMax As Boolean
    Public sumQtaRilevata As Decimal

    Public listSingoliPAnonConformi As List(Of ObjPrincipiAttivi)
    Public listFAMnonConformi As List(Of ObjPrincipiAttivi)

    Public listBioPAnonConformi As List(Of ObjPrincipiAttivi)
    Public listBioFAMnonConformi As List(Of ObjPrincipiAttivi)

    Public listMerceologichenonConformi As List(Of ObjMerceologica)

    Public ReadOnly Property listPaNonConformi As List(Of ObjPrincipiAttivi)
        Get
            Dim l As New List(Of ObjPrincipiAttivi)

            l.AddRange(listSingoliPAnonConformi)
            l.AddRange(listFAMnonConformi)

            Return l
        End Get
    End Property
    
End Class
