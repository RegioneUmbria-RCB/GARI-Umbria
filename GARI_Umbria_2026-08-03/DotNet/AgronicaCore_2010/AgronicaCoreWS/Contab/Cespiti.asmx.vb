Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<System.Web.Script.Services.ScriptService()> _
Public Class Cespiti
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function



    <WebMethod()> _
    Public Function WS_Cespiti_Leggi(ByVal int_TipoLettura As Integer, _
                                     ByVal objP_server As String,
                                     ByVal ForDelete As Boolean, _
                                     ByVal Piva_SuperUser As String, _
                                     ByVal Piva As String, _
                                     ByVal Sa_Cod As Long, _
                                     ByVal IdCodCespite As Long, _
                                     ByVal BeneTipo As Integer, _
                                   ByVal Elem_Cod As Long, _
                                   ByVal NomeTabella As String, _
                                   ByVal Tabella_Cod As String, _
                                   ByVal Pro_Cod As Long, _
                                     ByVal xFiltroAggiuntivo As String, _
                                     ByVal xOrderBy As String, _
                                     ByVal ChiamataDaGiasLan As Boolean
                                     ) As String


        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..
            Dim strRisposta As String = ""
            Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_R
            Dim dt As DataTable

            Select Case int_TipoLettura
                Case 1

                    dt = objCoreDAL.Leggi(objParametri_Server, _
                                          Piva_SuperUser, Piva, _
                                          Sa_Cod, IdCodCespite, BeneTipo, _
                                          xFiltroAggiuntivo, xOrderBy, _
                                          ChiamataDaGiasLan, strRisposta)

                Case 2
                    dt = objCoreDAL.LeggiAnagraficheCollegateACespiti(objParametri_Server, _
                                                                  Piva_SuperUser, Piva, _
                                                                  Sa_Cod, IdCodCespite, BeneTipo, _
                                                                  Elem_Cod, NomeTabella, Tabella_Cod, Pro_Cod, _
                                                                  xFiltroAggiuntivo, xOrderBy, _
                                                                  ChiamataDaGiasLan, strRisposta)

                Case Else
            End Select
            'AL MOMENTO NON UTILIZZATA
            'Chiamata al BIZ per restituzione XML 
            'Dim objCoreBIZ As New AgronicaCoreContabBIZ.Cespiti_R
            'Dim strXMLCespiti As String


            'strXMLCespiti = objCoreBIZ.Leggi(objParametri_Server, ForDelete, _
            '                                 Piva_SuperUser, Piva, _
            '                                 Sa_Cod, IdCodCespite, TipoCod, _
            '                                 xFiltroAggiuntivo, xOrderBy, ChiamataDaGiasLan)
            ' strRisposta = strXMLCespiti


            r.RispostaOK = True
            r.RispostaStringa = strRisposta

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function



    <WebMethod()> _
    Public Function WS_ListaCategorieBeniAmmortizzabili( _
                                     ByVal objP_server As String,
                                     ByVal BeneCod As Integer, _
                                     ByVal BeneTipo As Integer, _
                                     ByVal xFiltroAggiuntivo As String, _
                                     ByVal xOrderBy As String, _
                                     ByVal ChiamataDaGiasLan As Boolean
                                     ) As String


        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..
            Dim strRisposta As String = ""
            Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_R
            Dim dt As DataTable


            dt = objCoreDAL.ListaCategorieBeniAmmortizzabili(objParametri_Server, _
                                    BeneCod, BeneTipo, _
                                    xFiltroAggiuntivo, xOrderBy, _
                                    ChiamataDaGiasLan, strRisposta)


            r.RispostaOK = True
            r.RispostaStringa = strRisposta

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function





    <WebMethod()> _
    Public Function WS_Cespiti_Scrivi( _
                                     ByVal objP_server As String,
                                     ByVal Piva_SuperUser As String, _
                                     ByVal Piva As String, _
                                     ByVal Sa_Cod As Long, _
                                     ByVal IdCodCespite As Long, _
                                     ByVal DesCespite As String, _
                                     ByVal Bene_Cod As Long, _
                                     ByVal BeneTipo As Integer, _
                                     ByVal Cesp_Cod As Long, _
                                     ByVal IdCodCategoria As Long, _
                                     ByVal PrcAmmort As Double, _
                                     ByVal TipoBene As Integer, _
                                     ByVal NaturaBene As Integer, _
                                     ByVal DimezzaPrimoAnno As Integer, _
                                     ByVal DatIniUtilizzo As Date, _
                                     ByVal DatChiusura As Date, _
                                     ByVal NumAnniDurata As Integer, _
                                     ByVal TipoUbicazione As Integer, _
                                     ByVal CodUbicazione As Long, _
                                     ByVal IdCodCespitePadre As Long, _
                                    ByVal Fis_PrcAmmort As Double, _
                                    ByVal Fis_DimezzaPrimoAnno As Integer, _
                                    ByVal Fis_LimMaxDed As Double, _
                                    ByVal Fis_PrcDed As Double
                                    ) As Boolean


        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..

            Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_W


            r.RispostaOK = objCoreDAL.Scrivi(objParametri_Server, _
                                            Piva_SuperUser, _
                                            Piva, _
                                            Sa_Cod, _
                                            IdCodCespite, _
                                            DesCespite, _
                                            Bene_Cod, _
                                            BeneTipo, _
                                            Cesp_Cod, _
                                            IdCodCategoria, _
                                            PrcAmmort, _
                                            TipoBene, _
                                            NaturaBene, _
                                            DimezzaPrimoAnno, _
                                            DatIniUtilizzo, _
                                            DatChiusura, _
                                            NumAnniDurata, _
                                            TipoUbicazione, _
                                            CodUbicazione, _
                                            IdCodCespitePadre, _
                                           Fis_PrcAmmort, _
                                           Fis_DimezzaPrimoAnno, _
                                           Fis_LimMaxDed, _
                                           Fis_PrcDed)


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaOK
    End Function



    <WebMethod()> _
    Public Function WS_Cespiti_Modifica( _
                                     ByVal objP_server As String,
                                     ByVal Sa_Cod As Long, _
                                     ByVal IdCodCespite As Long, _
                                     ByVal DesCespite As String, _
                                     ByVal TipoCod As Integer, _
                                     ByVal IdCodCategoria As Long, _
                                     ByVal PrcAmmort As Integer, _
                                     ByVal BeneTipo As Integer, _
                                     ByVal NaturaBene As Integer, _
                                     ByVal DimezzaPrimoAnno As Integer, _
                                     ByVal DatIniUtilizzo As Date, _
                                     ByVal DatChiusura As Date, _
                                     ByVal NumAnniDurata As Integer, _
                                     ByVal TipoUbicazione As Integer, _
                                     ByVal CodUbicazione As Long, _
                                     ByVal IdCodCespitePadre As Long, _
                                    ByVal Fis_PrcAmmort As Double, _
                                    ByVal Fis_DimezzaPrimoAnno As Integer, _
                                    ByVal Fis_LimMaxDed As Double,
                                    ByVal Fis_PrcDed As Double
                                     ) As Boolean


        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..

            Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_W


            r.RispostaOK = objCoreDAL.Modifica(objParametri_Server, _
                                                Sa_Cod, _
                                                IdCodCespite, _
                                                DesCespite, _
                                                TipoCod, _
                                                IdCodCategoria, _
                                                PrcAmmort, _
                                                BeneTipo, _
                                                NaturaBene, _
                                                DimezzaPrimoAnno, _
                                                DatIniUtilizzo, _
                                                DatChiusura, _
                                                NumAnniDurata, _
                                                TipoUbicazione, _
                                                CodUbicazione, _
                                                IdCodCespitePadre, _
                                                Fis_PrcAmmort, _
                                                Fis_DimezzaPrimoAnno, _
                                                Fis_LimMaxDed, _
                                                Fis_PrcDed)


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaOK
    End Function


    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Function WS_Cespiti_Categorie_Leggi(ByVal objP_server As String, _
                                      ByVal IdCodCategoria As Long, _
                                      ByVal xFiltroAggiuntivo As String, _
                                      ByVal xOrderBy As String, _
                                      ByVal ChiamataDaGiasLan As Boolean) As String

        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..
            Dim strRisposta As String = ""
            Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_Categorie_R
            Dim dt As DataTable


            dt = objCoreDAL.Leggi(objParametri_Server, _
                                          IdCodCategoria, _
                                          xFiltroAggiuntivo, xOrderBy, _
                                          ChiamataDaGiasLan, strRisposta)


            r.RispostaOK = True
            r.RispostaStringa = strRisposta

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function


    <WebMethod()> _
    Public Function WS_Cespiti_Categorie_Modifica( _
                                     ByVal objP_server As String,
                                     ByVal IdCodCategoria As Long, _
                                     ByVal DesCategoria As String, _
                                     ByVal PrcAmmort As Double, _
                                     ByVal TipoBene As Integer, _
                                     ByVal DimezzaPrimoAnno As Integer, _
                                     ByVal FisPrcAmmor As Double, _
                                     ByVal FisDimezzaPrimoAnno As Integer, _
                                     ByVal FisLimMaxDed As Double, _
                                     ByVal FisPercDed As Double) As Boolean


        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..

            Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_Categorie_W


            r.RispostaOK = objCoreDAL.Modifica(objParametri_Server, _
                                                IdCodCategoria, _
                                                DesCategoria, _
                                                PrcAmmort, _
                                                TipoBene, _
                                                DimezzaPrimoAnno, _
                                               FisPrcAmmor, _
                                               FisDimezzaPrimoAnno, _
                                               FisLimMaxDed,
                                               FisPercDed)


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaOK
    End Function


    <WebMethod()> _
    Public Function WS_Cespiti_Categorie_Cancella( _
                             ByVal objP_server As String, _
                             ByVal id_cod_categoria As Integer) As Boolean

        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..

            Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_Categorie_W


            r.RispostaOK = objCoreDAL.Cancella(objParametri_Server, _
                              id_cod_categoria, _
                               "")


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaOK
    End Function



    <WebMethod()> _
<Script.Services.ScriptMethod()> _
    Public Function WS_Cespiti_Movimenti_Leggi(ByVal objP_server As String, _
                                      ByVal IdCodCespite As Long, _
                                      ByVal xFiltroAggiuntivo As String, _
                                      ByVal xOrderBy As String, _
                                      ByVal ChiamataDaGiasLan As Boolean) As String

        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..
            Dim strRisposta As String = ""
            Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_Movimenti_R
            Dim dt As DataTable


            dt = objCoreDAL.Leggi(objParametri_Server, _
                                          IdCodCespite, _
                                          xFiltroAggiuntivo, xOrderBy, _
                                          ChiamataDaGiasLan, strRisposta)


            r.RispostaOK = True
            r.RispostaStringa = strRisposta

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function


    <WebMethod()> _
    Public Function WS_Cespiti_Movimenti_Scrivi( _
                             ByVal objP_server As String,
                             ByVal id_cod_cespite As Integer, _
                             ByVal dat_mov As Date, _
                             ByVal cau_mov As Integer, _
                             ByVal esercizio As Integer, _
                             ByVal qta_N_mov As Integer, _
                             ByVal val_qtaN_costo_acq As Double, _
                             ByVal val_qta1_costo_acq As Double, _
                             ByVal val_qta1_mov As Double, _
                             ByVal val_qtaN_mov As Double, _
                             ByVal val_qta1_amm As Double, _
                             ByVal val_qtaN_amm As Double, _
                             ByVal prc_amm As Double, _
                             ByVal val_qta1_fondo_amm As Double, _
                             ByVal val_qtaN_fondo_amm As Double, _
                             ByVal val_qta1_residuo_amm As Double, _
                             ByVal val_qtaN_residuo_amm As Double, _
                             ByVal val_qta1_minus As Double, _
                             ByVal val_qtaN_minus As Double, _
                             ByVal val_qta1_plus As Double, _
                             ByVal val_qtaN_plus As Double, _
                            ByVal fis_val_qta1_ammortizzabile As Double, _
                            ByVal fis_val_qtaN_ammortizzabile As Double, _
                             ByVal fis_val_qta1_mov As Double, _
                             ByVal fis_val_qtaN_mov As Double, _
                             ByVal fis_val_qta1_amm As Double, _
                             ByVal fis_val_qtaN_amm As Double, _
                             ByVal fis_prc_amm As Double, _
                             ByVal fis_val_qta1_fondo_amm As Double, _
                             ByVal fis_val_qtaN_fondo_amm As Double, _
                             ByVal fis_val_qta1_residuo_amm As Double, _
                             ByVal fis_val_qtaN_residuo_amm As Double, _
                             ByVal fis_val_qta1_minus As Double, _
                             ByVal fis_val_qtaN_minus As Double, _
                             ByVal fis_val_qta1_plus As Double, _
                             ByVal fis_val_qtaN_plus As Double _
                            ) As Boolean



        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..

            Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_Movimenti_W


            r.RispostaOK = objCoreDAL.Scrivi(objParametri_Server, _
                              id_cod_cespite, _
                              dat_mov, _
                              cau_mov, _
                              esercizio, _
                              qta_N_mov, _
                              val_qtaN_costo_acq, _
                              val_qta1_costo_acq, _
                              val_qta1_mov, _
                              val_qtaN_mov, _
                              val_qta1_amm, _
                              val_qtaN_amm, _
                              prc_amm, _
                              val_qta1_fondo_amm, _
                              val_qtaN_fondo_amm, _
                              val_qta1_residuo_amm, _
                              val_qtaN_residuo_amm, _
                              val_qta1_minus, _
                              val_qtaN_minus, _
                              val_qta1_plus, _
                              val_qtaN_plus, _
                              fis_val_qta1_ammortizzabile, _
                              fis_val_qtaN_ammortizzabile, _
                              fis_val_qta1_mov, _
                              fis_val_qtaN_mov, _
                              fis_val_qta1_amm, _
                              fis_val_qtaN_amm, _
                              fis_prc_amm, _
                              fis_val_qta1_fondo_amm, _
                              fis_val_qtaN_fondo_amm, _
                              fis_val_qta1_residuo_amm, _
                              fis_val_qtaN_residuo_amm, _
                              fis_val_qta1_minus, _
                              fis_val_qtaN_minus, _
                              fis_val_qta1_plus, _
                              fis_val_qtaN_plus
                              )



        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaOK
    End Function


    <WebMethod()> _
    Public Function WS_Cespiti_Movimenti_Cancella( _
                             ByVal objP_server As String, _
                             ByVal id_cod_cespite As Integer, _
                             ByVal cau_mov As Integer, _
                             ByVal esercizio As Integer
                            ) As Boolean



        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..

            Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_Movimenti_W


            r.RispostaOK = objCoreDAL.Cancella(objParametri_Server, _
                              id_cod_cespite, _
                              cau_mov, _
                              esercizio, "")



        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaOK
    End Function



    <WebMethod()> _
    Public Function WS_Cespiti_Cancella( _
                             ByVal objP_server As String, _
                             ByVal id_cod_cespite As Integer) As Boolean

        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..

            Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_W


            r.RispostaOK = objCoreDAL.Cancella(objParametri_Server, _
                              id_cod_cespite, _
                               "")


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaOK
    End Function

    <WebMethod()> _
    Public Function WS_Cespiti_Categorie_Scrivi( _
                                     ByVal objP_server As String,
                                     ByVal IdCodCategoria As Long, _
                                     ByVal DesCategoria As String, _
                                     ByVal PrcAmmort As Double, _
                                     ByVal TipoBene As Integer, _
                                     ByVal DimezzaPrimoAnno As Integer, _
                                  ByVal FisPrcAmmor As Double, _
                                  ByVal FisDimezzaPrimoAnno As Integer, _
                                  ByVal FisLimMaxDed As Double, _
                                  ByVal FisPercDed As Double
                                     ) As Boolean


        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..

            Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_Categorie_W

            r.RispostaOK = objCoreDAL.Scrivi(objParametri_Server, _
                                             IdCodCategoria, _
                                             DesCategoria, _
                                             PrcAmmort, _
                                             TipoBene, _
                                             DimezzaPrimoAnno, _
                                            FisPrcAmmor, _
                                            FisDimezzaPrimoAnno, _
                                            FisLimMaxDed,
                                            FisPercDed)


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaOK
    End Function


End Class