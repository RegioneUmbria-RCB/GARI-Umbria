Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


Public Class FF_CampionamentoConferimento_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public ElencoTipoAnagraficaLiquidazione As New List(Of TipoAnagraficaLiquidazione) From {
        New TipoAnagraficaLiquidazione("A"),
        New TipoAnagraficaLiquidazione("L"),
        New TipoAnagraficaLiquidazione("")
    }

    Public Class TipoAnagraficaLiquidazione
        Public Property Sigla As String
        Public Property Descrizione As String

        Public Sub New(ByVal tipoAnagraficaSigla As String)
            Sigla = tipoAnagraficaSigla
            Descrizione = GetTipoAnagraficaLiquidazioneDes(tipoAnagraficaSigla)
        End Sub

        Public Shared Function GetTipoAnagraficaLiquidazioneDes(ByVal tipoAnagraficaSigla As String) As String
            Select Case tipoAnagraficaSigla
                Case "A"
                    Return "ACCONTO"
                Case "L"
                    Return "LIQUIDAZIONE"
                Case Else
                    Return ""
            End Select
        End Function
    End Class

    '##############################################################################################
    Public ElencoTipoAccontoLiquidazione As New List(Of TipoAccontoLiquidazione) From {
        New TipoAccontoLiquidazione("C"),
        New TipoAccontoLiquidazione("U"),
        New TipoAccontoLiquidazione(""),
        New TipoAccontoLiquidazione(Nothing)
    }

    Public Class TipoAccontoLiquidazione
        Public Property Sigla As String
        Public Property Descrizione As String

        Public Sub New(ByVal tipoAccontoSigla As String)
            Sigla = tipoAccontoSigla
            Descrizione = GetTipoAccontoLiquidazioneDes(tipoAccontoSigla)
        End Sub

        Public Shared Function GetTipoAccontoLiquidazioneDes(ByVal tipoAccontoSigla As String) As String
            Select Case tipoAccontoSigla
                Case "C"
                    Return "CALCOLATO"
                Case "U"
                    Return "UNA TANTUM"
                Case Else
                    Return ""
            End Select
        End Function
    End Class


    '##############################################################################################
    Public Function Leggi_Testata_GriglieCampionamento(ByVal piva As String,
                                                       ByVal descr As String,
                                                       ByVal DataRif As String,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As String

        Dim risposta As String = ""

        Dim DataRifDateTime As Nullable(Of DateTime)
        DataRifDateTime = Nothing
        If Not String.IsNullOrEmpty(DataRif) Then
            DataRifDateTime = Convert.ToDateTime(DataRif)
        End If

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Testata_GriglieCampionamento()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From griglia_testata In GiasContext.CampionamentoConferito_TestataGriglia
               Where
                   (griglia_testata.Piva_SuperUser.Equals(Piva_SuperUser)) _
                   AndAlso
                   (griglia_testata.PIVA.Equals(piva)) _
                   AndAlso
                   (griglia_testata.des_TestataGriglia.Contains(descr)) _
                   AndAlso
                   (DataRifDateTime Is Nothing OrElse (griglia_testata.Validita_Inizio <= DataRifDateTime AndAlso griglia_testata.Validita_Fine >= DataRifDateTime))
               Order By griglia_testata.des_TestataGriglia
               Select
                       griglia_testata.Id_TestataGriglia, griglia_testata.des_TestataGriglia, griglia_testata.Validita_Inizio, griglia_testata.Validita_Fine

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(TestataElem.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Elem_Testata_GriglieCampionamento(ByVal piva As String,
                                                            ByVal IDTestataGriglia As Integer,
                                                            ByRef objParametri As AgronicaCoreParametri
                                                            ) As CampionamentoConferito_TestataGriglia
        'As List(Of CampConferito_Testata_Griglia)

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Elem_Testata_GriglieCampionamento()"
        Dim TestataElem As CampionamentoConferito_TestataGriglia = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From griglia_testata In GiasContext.CampionamentoConferito_TestataGriglia
            Where griglia_testata.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                  griglia_testata.PIVA.Equals(piva) AndAlso
                  griglia_testata.Id_TestataGriglia = IDTestataGriglia
            Select griglia_testata).FirstOrDefault()

        End Using

        Return TestataElem

    End Function

    '##############################################################################################
    Public Function Leggi_Elem_Testata_Griglie_Prodotti(ByVal piva As String,
                                                        ByVal IDTestataGriglia As Integer,
                                                        ByVal IDProd As Integer,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As CampionamentoConferito_TestataGriglia_Prodotti

        Dim TestataElem As CampionamentoConferito_TestataGriglia_Prodotti = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Elem_Testata_Griglie_Prodotti()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From griglia_testata_prod In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
            Where
              (griglia_testata_prod.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
              (griglia_testata_prod.PIVA.Equals(piva)) AndAlso
              (IDTestataGriglia = 0 OrElse griglia_testata_prod.Id_TestataGriglia = IDTestataGriglia) AndAlso
              (IDProd = 0 OrElse griglia_testata_prod.Id_TestataGriglia_Prod = IDProd)
            Select griglia_testata_prod).FirstOrDefault()

        End Using

        Return TestataElem


    End Function

    '##############################################################################################
    Public Function Controlla_Date_Testata_GriglieCampionamento_Generica(
                ByVal campConfTestataGriglia As CampionamentoConferito_TestataGriglia,
                ByRef objParametri As AgronicaCoreParametri
                ) As Integer

        Dim countTrovati As Integer = 0

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Elem_Testata_GriglieCampionamento_FiltroAggiuntivo()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ListTestataGriglia = From griglia_testata In GiasContext.CampionamentoConferito_TestataGriglia
                                     Where
                                    (griglia_testata.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                    (griglia_testata.PIVA.Equals(campConfTestataGriglia.PIVA)) AndAlso
                                    (griglia_testata.Id_TestataGriglia <> campConfTestataGriglia.Id_TestataGriglia AndAlso
                                    griglia_testata.des_TestataGriglia = campConfTestataGriglia.des_TestataGriglia AndAlso
                                    ((campConfTestataGriglia.Validita_Inizio.HasValue AndAlso
                                        campConfTestataGriglia.Validita_Inizio <= griglia_testata.Validita_Inizio AndAlso
                                        campConfTestataGriglia.Validita_Fine >= griglia_testata.Validita_Inizio) OrElse
                                    (campConfTestataGriglia.Validita_Fine.HasValue AndAlso
                                        campConfTestataGriglia.Validita_Inizio <= griglia_testata.Validita_Fine AndALso
                                        campConfTestataGriglia.Validita_Fine >= griglia_testata.Validita_Fine) OrElse
                                         (campConfTestataGriglia.Validita_Inizio.HasValue AndAlso
                                         campConfTestataGriglia.Validita_Fine.HasValue AndAlso
                                         campConfTestataGriglia.Validita_Inizio >= griglia_testata.Validita_Inizio AndAlso
                                       campConfTestataGriglia.Validita_Fine <= griglia_testata.Validita_Fine) OrElse
                                         (campConfTestataGriglia.Validita_Inizio.HasValue AndAlso
                                         campConfTestataGriglia.Validita_Fine.HasValue AndAlso
                                         griglia_testata.Validita_Inizio >= campConfTestataGriglia.Validita_Inizio AndAlso
                                        griglia_testata.Validita_Fine <= campConfTestataGriglia.Validita_Fine)))
                                     Select griglia_testata

            countTrovati = ListTestataGriglia.Count()

        End Using

        Return countTrovati

    End Function




    '##############################################################################################
    Public Function Controlla_Sovrapposizione_Riga_Prodotti(
                ByVal campConfTestataGrigliaProdotti As CampionamentoConferito_TestataGriglia_Prodotti,
                ByRef objParametri As AgronicaCoreParametri,
                ByVal GiasContext As Gias_DeveloperServer_Entities
                ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Controlla_Sovrapposizione_Riga_Prodotti()"
        Dim messaggioErrore As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim griglia_testata_q_p = (From griglia_testata_questo_prodotto In GiasContext.CampionamentoConferito_TestataGriglia
                                   Where griglia_testata_questo_prodotto.Piva_SuperUser = campConfTestataGrigliaProdotti.Piva_SuperUser AndAlso
                                         griglia_testata_questo_prodotto.PIVA = campConfTestataGrigliaProdotti.PIVA AndAlso
                                         griglia_testata_questo_prodotto.Id_TestataGriglia = campConfTestataGrigliaProdotti.Id_TestataGriglia).FirstOrDefault

        Dim ListTestataGrigliaProd = (From griglia_testata_prod In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                                      Join griglia_testata In GiasContext.CampionamentoConferito_TestataGriglia
                                         On griglia_testata.Piva_SuperUser Equals griglia_testata_prod.Piva_SuperUser And
                                         griglia_testata.PIVA Equals griglia_testata_prod.PIVA And
                                         griglia_testata.Id_TestataGriglia Equals griglia_testata_prod.Id_TestataGriglia
                                      Join mat_prime In GiasContext.Materie_Prime
                                        On mat_prime.Mat_Cod Equals griglia_testata_prod.Mat_Cod
                                      Group Join otab_param_calibro In GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 1 AndAlso (x.Piva = campConfTestataGrigliaProdotti.PIVA OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                                        On otab_param_calibro.Tabella_Par_Cod Equals griglia_testata_prod.Tabella_Par_Cod_Calibro
                                            Into otab_param_calibro_group = Group
                                      From _otp_calibro In otab_param_calibro_group.DefaultIfEmpty()
                                      Group Join otab_param_qual In GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 3 AndAlso (x.Piva = campConfTestataGrigliaProdotti.PIVA OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                                        On otab_param_qual.Tabella_Par_Cod Equals griglia_testata_prod.Tabella_Par_Cod_Qualita
                                        Into otab_param_qual_group = Group
                                      From _otp_qual In otab_param_qual_group.DefaultIfEmpty()
                                      Where
                                            campConfTestataGrigliaProdotti.Id_TestataGriglia_Prod <> griglia_testata_prod.Id_TestataGriglia_Prod AndAlso
                                            griglia_testata_prod.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                                            griglia_testata_prod.PIVA.Equals(campConfTestataGrigliaProdotti.PIVA) AndAlso
                                            griglia_testata_prod.Mat_Cod = campConfTestataGrigliaProdotti.Mat_Cod AndAlso
                                            griglia_testata_prod.Tabella_Par_Cod_Qualita = campConfTestataGrigliaProdotti.Tabella_Par_Cod_Qualita AndAlso
                                            griglia_testata_prod.Tabella_Par_Cod_Calibro = campConfTestataGrigliaProdotti.Tabella_Par_Cod_Calibro AndAlso
                                            ((griglia_testata_q_p.Validita_Inizio.HasValue AndAlso
                                                griglia_testata_q_p.Validita_Inizio <= griglia_testata.Validita_Inizio AndAlso
                                                griglia_testata_q_p.Validita_Fine >= griglia_testata.Validita_Inizio) OrElse
                                            (griglia_testata_q_p.Validita_Fine.HasValue AndAlso
                                                griglia_testata_q_p.Validita_Inizio <= griglia_testata.Validita_Fine AndAlso
                                                griglia_testata_q_p.Validita_Fine >= griglia_testata.Validita_Fine) OrElse
                                            (griglia_testata_q_p.Validita_Inizio.HasValue AndAlso
                                            griglia_testata_q_p.Validita_Fine.HasValue AndAlso
                                            griglia_testata_q_p.Validita_Inizio >= griglia_testata.Validita_Inizio AndAlso
                                                griglia_testata_q_p.Validita_Fine <= griglia_testata.Validita_Fine) OrElse
                                            (griglia_testata_q_p.Validita_Inizio.HasValue AndAlso
                                            griglia_testata_q_p.Validita_Fine.HasValue AndAlso
                                            griglia_testata.Validita_Inizio >= griglia_testata_q_p.Validita_Inizio AndAlso
                                            griglia_testata.Validita_Fine <= griglia_testata_q_p.Validita_Fine))
                                      Select New With {
                                         .des_TestataGriglia = griglia_testata.des_TestataGriglia,
                                        .mat_des = mat_prime.Mat_Des,
                                        .qualita_des = If(_otp_qual Is Nothing, "", _otp_qual.Descrizione),
                                        .calibro_des = If(_otp_calibro Is Nothing, "", _otp_calibro.Descrizione)
                                    }).FirstOrDefault()

        If ListTestataGrigliaProd IsNot Nothing Then
            messaggioErrore = "Errore: esiste già una riga " & ListTestataGrigliaProd.mat_des & " " & ListTestataGrigliaProd.qualita_des & " " & ListTestataGrigliaProd.calibro_des & " nella griglia " & ListTestataGrigliaProd.des_TestataGriglia
        End If

        Return messaggioErrore

    End Function

    '##############################################################################################
    Public Function Leggi_TestataGriglia_Prodotti(ByVal piva As String,
                                                  ByVal IDTestataGriglia As Integer,
                                                  ByVal Id_TestataGriglia_Prod As Integer,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_TestataGriglia_Prodotti()"
        Dim risposta As String = ""

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim test_gr_prod =
               From testata_griglia_prodotti In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
               Join mat_prime In GiasContext.Materie_Prime
                   On mat_prime.Mat_Cod Equals testata_griglia_prodotti.Mat_Cod
               Group Join otab_param_calibro In GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 1 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On otab_param_calibro.Tabella_Par_Cod Equals testata_griglia_prodotti.Tabella_Par_Cod_Calibro
                    Into otab_param_calibro_group = Group
               From _otp_calibro In otab_param_calibro_group.DefaultIfEmpty()
               Group Join otab_param_qual In GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 3 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On
                     otab_param_qual.Tabella_Par_Cod Equals testata_griglia_prodotti.Tabella_Par_Cod_Qualita
                    Into otab_param_qual_group = Group
               From _otp_qual In otab_param_qual_group.DefaultIfEmpty()
               Where
                    testata_griglia_prodotti.Id_TestataGriglia = IDTestataGriglia AndAlso
                   (Id_TestataGriglia_Prod = 0 OrElse testata_griglia_prodotti.Id_TestataGriglia_Prod = Id_TestataGriglia_Prod) AndAlso
                    testata_griglia_prodotti.PIVA.Equals(piva) AndAlso
                    mat_prime.Elem_Cod = TRASFORMATI_VEGETALI
               Order By mat_prime.Mat_Des, testata_griglia_prodotti.Ordinamento, If(_otp_qual Is Nothing, "", _otp_qual.Descrizione), If(_otp_calibro Is Nothing, "", _otp_calibro.Descrizione)
               Select New With {
                   .Id_TestataGriglia_Prod = testata_griglia_prodotti.Id_TestataGriglia_Prod,
                   .Ordinamento = testata_griglia_prodotti.Ordinamento,
                   .Mat_Cod = mat_prime.Mat_Cod,
                   .Mat_Des = mat_prime.Mat_Des,
                   .qualita_cod = testata_griglia_prodotti.Tabella_Par_Cod_Qualita,
                   .qualita_des = If(_otp_qual Is Nothing, "", _otp_qual.Descrizione),
                   .calibro_cod = testata_griglia_prodotti.Tabella_Par_Cod_Calibro,
                   .calibro_des = If(_otp_calibro Is Nothing, "", _otp_calibro.Descrizione),
                   .Veg_Cod = mat_prime.Veg_Cod,
                   .Cul_Cod = mat_prime.Cul_Cod
               }

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(test_gr_prod.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function


    Public Function Leggi_TestataGriglia_Prodotti(ByVal piva As String,
                                                  ByVal IDTestataGriglia As Integer,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As List(Of CampionamentoConferito_TestataGriglia_Prodotti)

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_TestataGriglia_Prodotti()"
        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser


        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim ProdottiCampionamento As List(Of CampionamentoConferito_TestataGriglia_Prodotti)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            ProdottiCampionamento =
           (From prodotti_campionam In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
            Where prodotti_campionam.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                  prodotti_campionam.PIVA.Equals(piva) AndAlso
                  prodotti_campionam.Id_TestataGriglia = IDTestataGriglia
            Select prodotti_campionam).ToList

        End Using

        Return ProdottiCampionamento

    End Function

    Public Function Leggi_TestataGriglia_Prodotti_FiltroMat_Cod(ByVal piva As String,
                                                                  ByVal mat_cod As Integer,
                                                                  ByVal xOrderBy As String,
                                                                  ByRef objParametri As AgronicaCoreParametri
                                                                  ) As List(Of CampionamentoConferito_TestataGriglia_Prodotti)

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_TestataGriglia_Prodotti_FiltroMat_Cod()"
        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim ProdottiCampionamento As List(Of CampionamentoConferito_TestataGriglia_Prodotti)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            ProdottiCampionamento =
               (From prodotti_campionam In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                Where prodotti_campionam.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                      (piva = "" OrElse prodotti_campionam.PIVA.Equals(piva)) AndAlso
                      (mat_cod = 0 OrElse prodotti_campionam.Mat_Cod = mat_cod)
                Select prodotti_campionam).ToList

        End Using

        Return ProdottiCampionamento

    End Function

    ''' <summary>
    ''' Restituisce l'elenco dei prodotti associati almeno una volta ad una griglia nel periodo indicato
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="objParametriServer"></param>
    ''' <returns>Array Json contenente i Mat_Cod identificativi dei prodotti</returns>
    Public Function Leggi_TestataGriglia_Prodotti_InUso(ByVal piva As String,
                                                  ByVal Validita_Inizio As Date,
                                                  ByVal Validita_Fine As Date,
                                                  ByRef objParametriServer As AgronicaCoreParametri
                                                  ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_TestataGriglia_Prodotti()"
        Dim risposta As String = ""

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim test_gr_prod =
               From testata_griglia_prodotti In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
               Join griglia_testata In GiasContext.CampionamentoConferito_TestataGriglia
                    On griglia_testata.Id_TestataGriglia Equals testata_griglia_prodotti.Id_TestataGriglia
               Where
                    testata_griglia_prodotti.PIVA.Equals(piva) And Not (
                        Validita_Inizio > griglia_testata.Validita_Fine Or Validita_Fine < griglia_testata.Validita_Inizio)
               Select testata_griglia_prodotti.Mat_Cod

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(test_gr_prod.Distinct().ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    ''' <summary>
    ''' Restituisce l'attuale numero di ordinamento più alto per la griglia indicata
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="IDTestataGriglia">PK della tabella CampionamentoConferito_TestataGriglia</param>
    ''' <param name="objParametriServer"></param>
    ''' <returns></returns>
    Public Function Leggi_Ultimo_Ordinamento(ByVal piva As String, ByVal IDTestataGriglia As Integer, ByRef objParametriServer As AgronicaCoreParametri) As Integer
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Ultimo_Ordinamento()"
        Dim ultimoOrdinamento As Integer

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)

        Dim ordinamentiGriglia As List(Of Integer)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            ordinamentiGriglia =
                    (From prodotti_campionam In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                     Where prodotti_campionam.PIVA.Equals(piva) AndAlso
                              prodotti_campionam.Id_TestataGriglia = IDTestataGriglia
                     Select prodotti_campionam.Ordinamento).ToList()
        End Using

        If ordinamentiGriglia.Count > 0 Then
            ultimoOrdinamento = ordinamentiGriglia.Max()
        Else
            ultimoOrdinamento = 0
        End If

        Return ultimoOrdinamento

    End Function

    '##############################################################################################
    ' Stefano - 25/1/2017 - Non più necessario perché da ora ci si lega alle materie prime e non alle linee produzione
    'Public Function Leggi_Linee_Produzioni(ByVal piva As String,
    '                                       ByVal IDTestataGriglia As Integer,
    '                                       ByVal xOrderBy As String,
    '                                       ByRef objParametri As AgronicaCoreParametri
    '                                       ) As String

    '    Dim Piva_SuperUser = objParametri.PivaSuperUser

    '    '----- Descrizione
    '    Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Linee_Produzioni()"

    '    Dim gefutils As New Gias_EF_Utility

    '    Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

    '    Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

    '    Dim TestataGriglia = (From griglia_testata In GiasContext.CampionamentoConferito_TestataGriglia
    '                          Where
    '  (griglia_testata.Piva_SuperUser.Equals(Piva_SuperUser) And
    '   griglia_testata.PIVA.Equals(piva) And
    '   griglia_testata.Id_TestataGriglia.Equals(IDTestataGriglia))).FirstOrDefault()

    '    ' Vengono selezionati tutti i prodotti senza griglia associata +
    '    ' tutti i prodotti che sono associati alla griglia corrente + 
    '    ' tutti i prodotti associati ad una griglia fuori validità
    '    Dim LineeProduzioni =
    '       From linea_produzione In GiasContext.Linee_Produzioni
    '       Group Join testata_griglia_prodotti In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
    '           On linea_produzione.Piva Equals testata_griglia_prodotti.PIVA _
    '           And
    '           linea_produzione.Linea_Cod Equals testata_griglia_prodotti.Mat_Cod
    '        Into _testata_griglia_prodotti = Group
    '       From _tgp In _testata_griglia_prodotti.DefaultIfEmpty()
    '       Group Join testata_griglia In GiasContext.CampionamentoConferito_TestataGriglia
    '           On _tgp.Piva_SuperUser Equals testata_griglia.Piva_SuperUser _
    '           And
    '           _tgp.PIVA Equals testata_griglia.PIVA _
    '           And
    '           _tgp.Id_TestataGriglia Equals testata_griglia.Id_TestataGriglia
    '           Into _testata_griglia = Group
    '       From _tg In _testata_griglia.DefaultIfEmpty()
    '       Where linea_produzione.Piva.Equals(piva) And
    '           If(_tgp Is Nothing, True,
    '            If(_tg.Id_TestataGriglia = IDTestataGriglia,
    '                True,
    '                If(_tg.Validita_Inizio > TestataGriglia.Validita_Fine Or _tg.Validita_Fine < TestataGriglia.Validita_Inizio,
    '                True, False)))
    '       Order By linea_produzione.Linea_Des
    '       Select New With {
    '           .Linea_Cod = linea_produzione.Linea_Cod,
    '           .Linea_Des = linea_produzione.Linea_Des,
    '           .Id_TestataGriglia = If(Not _tgp Is Nothing And _tgp.Id_TestataGriglia = IDTestataGriglia, True, False)
    '       }

    '    Dim serializerSettings As New JsonSerializerSettings()
    '    serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    '    Dim risposta As String = JsonConvert.SerializeObject(LineeProduzioni.ToList(), Formatting.None, serializerSettings)

    '    Return risposta

    'End Function
    ' FINE Stefano - 25/1/2017 - Non più necessario perché da ora ci si lega alle materie prime e non alle linee produzione

    '##############################################################################################
    Public Function Leggi_Elem_Testata_GriglieCalibri(ByVal piva As String,
                                                      ByVal IDTestataGriglia As Integer,
                                                      ByVal Id_Calibro As Integer,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As CampionamentoConferito_TestataGriglia_Calibri

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Elem_Testata_GriglieCalibri()"
        Dim TestataElem As CampionamentoConferito_TestataGriglia_Calibri = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From griglia_testata In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
            Where griglia_testata.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                  griglia_testata.PIVA.Equals(piva) AndAlso
                  griglia_testata.Id_TestataGriglia = IDTestataGriglia AndAlso
                  griglia_testata.Id_Calibro = Id_Calibro
            Select griglia_testata).FirstOrDefault()
        End Using

        Return TestataElem


    End Function

    '##############################################################################################
    Public Function Leggi_TestataGriglie_Calibri(ByVal piva As String,
                                                 ByVal IDTestataGriglia As Integer,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_TestataGriglie_Calibri()"
        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim CalibriCampionamento =
           From calibri_campionam In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
           Where calibri_campionam.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                 calibri_campionam.PIVA.Equals(piva) AndAlso
                 calibri_campionam.Id_TestataGriglia = IDTestataGriglia
           Order By calibri_campionam.Ordinamento
           Select
                calibri_campionam.Id_Calibro, calibri_campionam.Descr_qualita, calibri_campionam.Descr_calibro, calibri_campionam.Peso, calibri_campionam.Ordinamento

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(CalibriCampionamento.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Righe_Conferimento(ByVal piva As String,
                                             ByVal _docNumeroSin As String, ByVal _docNumero As Integer,
                                             ByVal _docNumeroDes As String, ByVal _nrRiga As Integer,
                                             ByVal _dataMovDal As String, ByVal _dataMovAl As String,
                                             ByVal _dataInizioCampDal As String, ByVal _dataInizioCampAl As String,
                                             ByVal _bollaCampDal As String, ByVal _bollaCampAl As String,
                                             ByVal _specie As Integer, ByVal _varieta As Integer(),
                                             ByVal _operazioni As Integer(),
                                             ByVal _fornitori As String(),
                                             ByVal gruppoFatturazione As Integer(),
                                             ByVal xOrderBy As String,
                                             ByVal StatoCampionamentoIniziale As String,
                                             ByVal TipoCampionamentoIniziale As Short,
                                             ByVal Id_Mov_Det As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Righe_Conferimento()"
        Dim risposta As String = ""

        Dim _filtroSuVarieta As Boolean = False
        If _varieta IsNot Nothing AndAlso _varieta.Length > 0 Then
            _filtroSuVarieta = True
        End If

        Dim _filtroSuFornitori As Boolean = False
        If _fornitori IsNot Nothing AndAlso _fornitori.Length > 0 Then
            _filtroSuFornitori = True
        End If

        Dim _filtroSuOperazioni As Boolean = False
        If _operazioni IsNot Nothing AndAlso _operazioni.Length > 0 Then
            _filtroSuOperazioni = True
        End If

        Dim _filtroSuGruppoFatturazione As Boolean = False
        If gruppoFatturazione IsNot Nothing AndAlso gruppoFatturazione.Length > 0 Then
            _filtroSuGruppoFatturazione = True
        End If

        Dim dataMovDalDateTime As Nullable(Of DateTime)
        dataMovDalDateTime = Nothing
        If Not String.IsNullOrEmpty(_dataMovDal) Then
            dataMovDalDateTime = Convert.ToDateTime(_dataMovDal)
        End If

        Dim dataMovAlDateTime As Nullable(Of DateTime)
        dataMovAlDateTime = Nothing
        If Not String.IsNullOrEmpty(_dataMovAl) Then
            dataMovAlDateTime = Convert.ToDateTime(_dataMovAl)
        End If

        Dim inizioCampDalDateTime As Nullable(Of DateTime)
        inizioCampDalDateTime = Nothing
        If Not String.IsNullOrEmpty(_dataInizioCampDal) Then
            inizioCampDalDateTime = Convert.ToDateTime(_dataInizioCampDal)
        End If

        Dim inizioCampAlDateTime As Nullable(Of DateTime)
        inizioCampAlDateTime = Nothing
        If Not String.IsNullOrEmpty(_dataInizioCampAl) Then
            inizioCampAlDateTime = Convert.ToDateTime(_dataInizioCampAl)
        End If

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim campionamentoConferito_Movimenti As DbSet(Of CampionamentoConferito_Movimenti) = GiasContext.CampionamentoConferito_Movimenti
            Dim movimenti_dettagli As DbSet(Of Movimenti_dettagli) = GiasContext.Movimenti_dettagli
            Dim movimenti As DbSet(Of Movimenti) = GiasContext.Movimenti
            Dim agenda As DbSet(Of Agenda) = GiasContext.Agenda
            Dim contatti As DbSet(Of Contatti) = GiasContext.Contatti
            Dim risorse_umane As DbSet(Of Risorse_Umane) = GiasContext.Risorse_Umane
            Dim matPrima As DbSet(Of Materie_Prime) = GiasContext.Materie_Prime
            Dim matPrimeCampionature As DbSet(Of Materie_Prime_Campionature) = GiasContext.Materie_Prime_Campionature
            Dim tabelleParametri As DbSet(Of OTabelle_Parametri) = GiasContext.OTabelle_Parametri
            Dim matprimadett As DbSet(Of Materie_Prime_Dettagli) = GiasContext.Materie_Prime_Dettagli

            Dim lavCodConf As Integer?() = {LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE}

            Dim RigheConferimento =
                From ag In agenda
                Join mov In movimenti
                    On
                     ag.PIVA Equals mov.PIVA And
                     ag.Id_Agenda Equals mov.Id_Agenda
                Join r_u In risorse_umane
                    On
                     mov.Cod_RisUm Equals r_u.Cod_RisUm
                Join cont In contatti
                    On
                     r_u.Piva Equals cont.Piva And
                     r_u.Cod_Contatto Equals cont.Cod_Contatto
                Join mov_det In movimenti_dettagli
                    On
                     ag.PIVA Equals mov_det.PIVA And
                     mov.Id_Agenda Equals mov_det.Id_Agenda And
                     mov.Id_Mov Equals mov_det.Id_Mov
                Join mat_prima In matPrima
                    On
                     mov_det.Elem_Cod Equals mat_prima.Elem_Cod And
                     mov_det.Mat_Cod Equals mat_prima.Mat_Cod
                Group Join mat_prima_dett In matprimadett
                    On mat_prima_dett.Mat_Cod Equals mat_prima.Mat_Cod
                    Into mat_prima_dett_group = Group
                From _mat_prima_dett In mat_prima_dett_group.DefaultIfEmpty()
                Group Join otab_param_grpfatt In tabelleParametri.Where(Function(x) x.Tabella_Cod = 20 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On otab_param_grpfatt.Tabella_Par_Cod Equals _mat_prima_dett.Extra_Int1
                    Into otab_param_grpfatt_group = Group
                From _otab_param_grpfatt In otab_param_grpfatt_group.DefaultIfEmpty()
                Group Join mat_prime_camp_certificazioni In matPrimeCampionature.Where(Function(x) x.Tipo = "ocertificazioni")
                    On
                     mat_prime_camp_certificazioni.Progressivo Equals mov_det.Cal_Cod
                    Into mat_prime_camp_certificazioni_group = Group
                From _mpc_cert In mat_prime_camp_certificazioni_group.DefaultIfEmpty()
                Group Join otab_param_cert In tabelleParametri.Where(Function(x) x.Tabella_Cod = 12 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On otab_param_cert.Tabella_Par_Cod Equals _mpc_cert.Tipo_Cod
                    Into otab_param_cert_group = Group
                From _otp_cert In otab_param_cert_group.DefaultIfEmpty()
                Group Join mat_prime_camp_qualita In matPrimeCampionature.Where(Function(x) x.Tipo = "oqualità")
                    On mat_prime_camp_qualita.Progressivo Equals mov_det.Cal_Cod
                    Into mat_prime_camp_qualita_group = Group
                From _mpc_qual In mat_prime_camp_qualita_group.DefaultIfEmpty()
                Group Join otab_param_qual In tabelleParametri.Where(Function(x) x.Tabella_Cod = 3 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On
                     otab_param_qual.Tabella_Par_Cod Equals _mpc_qual.Tipo_Cod
                    Into otab_param_qual_group = Group
                From _otp_qual In otab_param_qual_group.DefaultIfEmpty()
                Group Join mat_prime_camp_calibri In matPrimeCampionature.Where(Function(x) x.Tipo = "ocalibro")
                    On
                     mat_prime_camp_calibri.Progressivo Equals mov_det.Cal_Cod
                    Into mat_prime_camp_calibri_group = Group
                From _mpc_cal In mat_prime_camp_calibri_group.DefaultIfEmpty()
                Group Join otab_param_calibro In tabelleParametri.Where(Function(x) x.Tabella_Cod = 1 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On
                     otab_param_calibro.Tabella_Par_Cod Equals _mpc_cal.Tipo_Cod
                    Into otab_param_cal_group = Group
                From _otp_cal In otab_param_cal_group.DefaultIfEmpty()
                Group Join mat_prime_camp_rugginosita In matPrimeCampionature.Where(Function(x) x.Tipo = "orugginosita")
                    On
                    mat_prime_camp_rugginosita.Progressivo Equals mov_det.Cal_Cod
                Into mat_prime_camp_rugginosita_group = Group
                From _mpc_rugg In mat_prime_camp_rugginosita_group.DefaultIfEmpty()
                Group Join otab_param_rugg In tabelleParametri.Where(Function(x) x.Tabella_Cod = 22 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On
                    otab_param_rugg.Tabella_Par_Cod Equals _mpc_rugg.Tipo_Cod
                    Into otab_param_rugg_group = Group
                From _otp_rugg In otab_param_rugg_group.DefaultIfEmpty()
                Group Join campconf_mov In campionamentoConferito_Movimenti
                    On mov_det.PIVA Equals campconf_mov.PIVA And
                       mov_det.Id_Mov_Det Equals campconf_mov.Id_Mov_Det Into campconf_movimenti = Group
                From _cm In campconf_movimenti.DefaultIfEmpty()
                Where
                    ag.PIVA.Equals(piva) AndAlso
                    lavCodConf.Contains(ag.Lav_Cod) AndAlso
                    mov.PIVA.Equals(piva) AndAlso
                    mov_det.PIVA.Equals(piva) AndAlso
                    (String.IsNullOrEmpty(_dataMovDal) OrElse mov.Data_Movimento >= dataMovDalDateTime) AndAlso
                    (String.IsNullOrEmpty(_dataMovAl) OrElse mov.Data_Movimento <= dataMovAlDateTime) AndAlso
                    (String.IsNullOrEmpty(_docNumeroDes) OrElse mov.Doc_Numero_Des.Contains(_docNumeroDes)) AndAlso
                    (_docNumero = 0 Or mov.Doc_Numero = _docNumero) AndAlso
                    (String.IsNullOrEmpty(_docNumeroSin) OrElse mov.Doc_Numero_Sin.Contains(_docNumeroSin)) AndAlso
                    (mov.Cau_Mov = CAU_CARICO OrElse mov.Cau_Mov = CAU_REGISTRAZIONI_TERZIARIA) AndAlso
                    mov_det.Elem_Cod = TRASFORMATI_VEGETALI AndAlso
                    (_nrRiga = 0 OrElse mov_det.Ordine_Det = _nrRiga OrElse mov_det.Extra_Str.Equals(CStr(_nrRiga))) AndAlso
                    (_specie = -1 Or mat_prima.Veg_Cod = _specie) AndAlso
                    ((_filtroSuVarieta = False) OrElse _varieta.Contains(mat_prima.Cul_Cod)) AndAlso
                    ((_filtroSuOperazioni = False) OrElse _operazioni.Contains(ag.Lav_Cod)) AndAlso
                    ((_filtroSuFornitori = False) OrElse _fornitori.Contains(cont.Cod_Contatto)) AndAlso
                    ((_filtroSuGruppoFatturazione = False) OrElse gruppoFatturazione.Contains(_mat_prima_dett.Extra_Int1)) AndAlso
                    ((Id_Mov_Det = 0) OrElse (mov_det.Id_Mov_Det = Id_Mov_Det)) AndAlso
                    (String.IsNullOrEmpty(_dataInizioCampDal) OrElse _cm.Dt_Inizio_Campionamento >= inizioCampDalDateTime) AndAlso
                    (String.IsNullOrEmpty(_dataInizioCampAl) OrElse _cm.Dt_Inizio_Campionamento <= inizioCampAlDateTime) AndAlso
                    (String.IsNullOrEmpty(_bollaCampDal) OrElse _cm.NrBolla_Campionamento >= _bollaCampDal) AndAlso
                    (String.IsNullOrEmpty(_bollaCampAl) OrElse _cm.NrBolla_Campionamento <= _bollaCampAl)
                Order By mov.Doc_Numero, mov_det.Ordine_Det, mov_det.Extra_Str
                Select New With
                {
                    .Id_Mov_Det = mov_det.Id_Mov_Det,
                    .Data_Movimento = mov.Data_Movimento,
                    .Cod_Contatto = cont.Cod_Contatto,
                    .Rag_Soc = cont.Rag_Soc,
                    .ConferimentoOAcquisto = If(r_u.Cod_Rapporto = -18, "Conferimento", "Acquisto"),
                    .Doc_Numero = mov.Doc_Numero_Sin & CStr(mov.Doc_Numero) & mov.Doc_Numero_Des,
                    .NrRiga = If(mov_det.Ordine_Det = 0, mov_det.Extra_Str, CStr(mov_det.Ordine_Det)),
                    .Mat_Cod = mat_prima.Mat_Cod,
                    .Mat_Des = mat_prima.Mat_Des,
                    .Calibro_Cod = If(_otp_cal Is Nothing, 0, _otp_cal.Tabella_Par_Cod),
                    .Qualita_Cod = If(_otp_qual Is Nothing, 0, _otp_qual.Tabella_Par_Cod),
                    .Certificazione_Cod = If(_otp_cert Is Nothing, 0, _otp_cert.Tabella_Par_Cod),
                    .Rugginosita_Cod = If(_otp_rugg Is Nothing, 0, _otp_rugg.Tabella_Par_Cod),
                    .Calibro = If(_otp_cal Is Nothing, "", If(_otp_cal.Sigla, "")),
                    .Qualita = If(_otp_qual Is Nothing, "", If(_otp_qual.Sigla, "")),
                    .Certificazione = If(_otp_cert Is Nothing, "", If(_otp_cert.Sigla, "")),
                    .Rugginosita = CStr(If(_otp_rugg Is Nothing, "", _otp_rugg.Descrizione)),
                    .Qta_Extra_Totale = mov_det.Qta_Extra_Totale - mov_det.Qta_Extra_Totale / 100 * mov_det.Variazione,
                    .Lotto = mov_det.Lotto,
                    .Note = If(_cm Is Nothing, "", If(_cm.Note, "")),
                    .QtaCampionata = If(_cm Is Nothing, 0, _cm.QtaCampionata),
                    .Automatico = If(_cm Is Nothing, TipoCampionamentoIniziale, _cm.Automatico),
                    .StatoCampionamento = If(_cm Is Nothing, StatoCampionamentoIniziale, _cm.StatoCampionamento),
                    .Data_InizioLavorazione = If(_cm Is Nothing, DateTime.Now(), _cm.Dt_Inizio_Campionamento),
                    .Data_FineLavorazione = If(_cm Is Nothing, DateTime.Now(), _cm.Dt_Fine_Campionamento),
                    .NrBolla = If(_cm Is Nothing, "", _cm.NrBolla_Campionamento)
                }

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}

            risposta = JsonConvert.SerializeObject(RigheConferimento.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Id_Testata_Griglia_Da_Movim_Conferimento(
                ByVal piva As String,
                ByVal ID_Mov_Det As Integer,
                ByRef Id_Testata_Griglia_Trovata As Integer,
                ByRef Id_Testata_Griglia_Prod_Trovata As Integer,
                ByVal ControllaPresenzaGrigliaCampionamento As Boolean,
                ByVal ControllaPresenzaCalibri As Boolean,
                ByRef objParametri As AgronicaCoreParametri,
                Optional ByVal Id_Mov As Integer = 0,
                Optional ByVal Elem_Cod As Integer = 0,
                Optional ByVal Mat_Cod As Integer = 0,
                Optional ByVal Cal_Cod As Integer = 0
                ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Id_Testata_Griglia_Da_Movim_Conferimento()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            If (Id_Mov = 0 OrElse Elem_Cod = 0 OrElse Mat_Cod = 0 OrElse Cal_Cod = 0) Then
                Dim MovimentoDettElem =
            (From mov_dettaglio In GiasContext.Movimenti_dettagli
             Where mov_dettaglio.PIVA.Equals(piva) AndAlso
                 mov_dettaglio.Id_Mov_Det = ID_Mov_Det).FirstOrDefault()
                If MovimentoDettElem Is Nothing Then
                    Throw New Exception("Riga Movimento con chiave " & ID_Mov_Det & " non trovata")
                    Return ""
                End If

                Id_Mov = MovimentoDettElem.Id_Mov
                Elem_Cod = MovimentoDettElem.Elem_Cod
                Mat_Cod = MovimentoDettElem.Mat_Cod
                Cal_Cod = MovimentoDettElem.Cal_Cod

            End If

            Dim MovimentoElem =
                (From mov In GiasContext.Movimenti
                 Where mov.Id_Mov = Id_Mov AndAlso
                        mov.PIVA.Equals(piva)).FirstOrDefault()
            If MovimentoElem Is Nothing Then
                Throw New Exception("Movimento con chiave " & Id_Mov.ToString() & " non trovato")
                Return ""
            End If

            Dim MatPrimaElem =
                    (From matprime In GiasContext.Materie_Prime
                     Where matprime.Elem_Cod = Elem_Cod AndAlso
                         matprime.Mat_Cod = Mat_Cod).FirstOrDefault()
            If MatPrimaElem Is Nothing Then
                Throw New Exception("Materia prima con chiave " & Elem_Cod.ToString() & " " & Mat_Cod.ToString() & " non trovata")
                Return ""
            End If

            Dim foundQualita = True
            Dim qualita =
                (From mat_prime_camp_qualita In GiasContext.Materie_Prime_Campionature
                 Where mat_prime_camp_qualita.Tipo = "oqualità" AndAlso mat_prime_camp_qualita.Progressivo = Cal_Cod).FirstOrDefault()
            If qualita Is Nothing Then
                foundQualita = False
                qualita = New Materie_Prime_Campionature
                'Throw New Exception("Qualità con chiave " & Cal_Cod.ToString() & " non trovata")
                'Return ""
            End If

            Dim foundCalibro = True
            Dim calibro =
                (From mat_prime_camp_calibro In GiasContext.Materie_Prime_Campionature
                 Where mat_prime_camp_calibro.Tipo = "ocalibro" AndAlso mat_prime_camp_calibro.Progressivo = Cal_Cod).FirstOrDefault()
            If calibro Is Nothing Then
                foundCalibro = False
                calibro = New Materie_Prime_Campionature
                'Throw New Exception("Calibro con chiave " & Cal_Cod.ToString() & " non trovato")
                'Return ""
            End If

            'Primo tentativo a chiave completa
            'If Not qualita Is Nothing AndAlso Not calibro Is Nothing Then
            Dim Testata_GrigliaElem = (From prod_griglia In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                                       Join testata_griglia In GiasContext.CampionamentoConferito_TestataGriglia
                        On prod_griglia.Id_TestataGriglia Equals testata_griglia.Id_TestataGriglia
                                       Where testata_griglia.Validita_Inizio <= MovimentoElem.Data_Movimento And
                          testata_griglia.Validita_Fine >= MovimentoElem.Data_Movimento And
                          prod_griglia.Mat_Cod = Mat_Cod And
                          (foundQualita AndAlso prod_griglia.Tabella_Par_Cod_Qualita = qualita.Tipo_Cod) And
                          (foundCalibro AndAlso prod_griglia.Tabella_Par_Cod_Calibro = calibro.Tipo_Cod) And
                                           testata_griglia.PIVA.Equals(piva)
                                       Select testata_griglia.Id_TestataGriglia, prod_griglia.Id_TestataGriglia_Prod, testata_griglia.des_TestataGriglia).FirstOrDefault()
            'End If

            'Secondo tentativo con prodotto + qualità 
            If Testata_GrigliaElem Is Nothing Then
                Testata_GrigliaElem = (From prod_griglia In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                                       Join testata_griglia In GiasContext.CampionamentoConferito_TestataGriglia
                        On prod_griglia.Id_TestataGriglia Equals testata_griglia.Id_TestataGriglia
                                       Where testata_griglia.Validita_Inizio <= MovimentoElem.Data_Movimento And
                          testata_griglia.Validita_Fine >= MovimentoElem.Data_Movimento And
                          prod_griglia.Mat_Cod = Mat_Cod And
                          (foundQualita AndAlso prod_griglia.Tabella_Par_Cod_Qualita = qualita.Tipo_Cod) And
                                           testata_griglia.PIVA.Equals(piva)
                                       Select testata_griglia.Id_TestataGriglia, prod_griglia.Id_TestataGriglia_Prod, testata_griglia.des_TestataGriglia).FirstOrDefault()
            End If

            'Terzo tentativo con prodotto + calibro 
            If Testata_GrigliaElem Is Nothing Then
                Testata_GrigliaElem = (From prod_griglia In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                                       Join testata_griglia In GiasContext.CampionamentoConferito_TestataGriglia
                        On prod_griglia.Id_TestataGriglia Equals testata_griglia.Id_TestataGriglia
                                       Where testata_griglia.Validita_Inizio <= MovimentoElem.Data_Movimento And
                          testata_griglia.Validita_Fine >= MovimentoElem.Data_Movimento And
                          prod_griglia.Mat_Cod = Mat_Cod And
                          (foundCalibro AndAlso prod_griglia.Tabella_Par_Cod_Calibro = calibro.Tipo_Cod) And
                                           testata_griglia.PIVA.Equals(piva)
                                       Select testata_griglia.Id_TestataGriglia, prod_griglia.Id_TestataGriglia_Prod, testata_griglia.des_TestataGriglia).FirstOrDefault()
            End If

            'Quarto tentativo con il solo prodotto
            If Testata_GrigliaElem Is Nothing Then
                Testata_GrigliaElem = (From prod_griglia In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                                       Join testata_griglia In GiasContext.CampionamentoConferito_TestataGriglia
                        On prod_griglia.Id_TestataGriglia Equals testata_griglia.Id_TestataGriglia
                                       Where testata_griglia.Validita_Inizio <= MovimentoElem.Data_Movimento And
                          testata_griglia.Validita_Fine >= MovimentoElem.Data_Movimento And
                          prod_griglia.Mat_Cod = Mat_Cod And
                          prod_griglia.Tabella_Par_Cod_Qualita = 0 And
                          prod_griglia.Tabella_Par_Cod_Calibro = 0 And
                                           testata_griglia.PIVA.Equals(piva)
                                       Select testata_griglia.Id_TestataGriglia, prod_griglia.Id_TestataGriglia_Prod, testata_griglia.des_TestataGriglia).FirstOrDefault()
            End If

            If Testata_GrigliaElem Is Nothing Then
                Id_Testata_Griglia_Trovata = 0
                Id_Testata_Griglia_Prod_Trovata = 0
                If ControllaPresenzaGrigliaCampionamento Then
                    Throw New Exception("Non esiste una griglia di campionamento per il prodotto " & MatPrimaElem.Mat_Cod.ToString() & " " & MatPrimaElem.Mat_Des.ToString())
                End If
            Else

                Id_Testata_Griglia_Trovata = Testata_GrigliaElem.Id_TestataGriglia
                Id_Testata_Griglia_Prod_Trovata = Testata_GrigliaElem.Id_TestataGriglia_Prod

                If ControllaPresenzaCalibri Then
                    Dim CampionaRiga = From campconfer_calibri In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
                                       Where campconfer_calibri.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                                         campconfer_calibri.PIVA.Equals(piva) AndAlso
                                         campconfer_calibri.Id_TestataGriglia = Testata_GrigliaElem.Id_TestataGriglia
                                       Select campconfer_calibri.Id_Calibro

                    If CampionaRiga.Count() = 0 Then
                        Throw New Exception("Griglia calibri non trovata per il prodotto " & MatPrimaElem.Mat_Cod.ToString() & " " & MatPrimaElem.Mat_Des.ToString())
                    End If
                End If
            End If

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Testata_GrigliaElem, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function Leggi_Id_Testata_Griglia_Da_MatCod(
                ByVal piva As String,
                ByVal ControllaPresenzaGrigliaCampionamento As Boolean,
                ByVal ControllaPresenzaCalibri As Boolean,
                ByVal Elem_Cod As Integer,
                ByVal Mat_Cod As Integer,
                ByVal Tipo_Cod_Qualita As Integer,
                ByVal Tipo_Cod_Calibro As Integer,
                ByVal Data_Movimento As Date,
                ByRef Id_Testata_Griglia_Trovata As Integer,
                ByRef Id_Testata_Griglia_Prod_Trovata As Integer,
                ByRef objParametri As AgronicaCoreParametri
                ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Id_Testata_Griglia_Da_MatCod()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)


            Dim MatPrimaElem =
                    (From matprime In GiasContext.Materie_Prime
                     Where matprime.Elem_Cod = Elem_Cod AndAlso
                         matprime.Mat_Cod = Mat_Cod).FirstOrDefault()
            If MatPrimaElem Is Nothing Then
                Throw New Exception("Materia prima con chiave " & Elem_Cod.ToString() & " " & Mat_Cod.ToString() & " non trovata")
                Return ""
            End If

            'Primo tentativo a chiave completa
            'If Not qualita Is Nothing AndAlso Not calibro Is Nothing Then
            Dim Testata_GrigliaElem = (From prod_griglia In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                                       Join testata_griglia In GiasContext.CampionamentoConferito_TestataGriglia
                        On prod_griglia.Id_TestataGriglia Equals testata_griglia.Id_TestataGriglia
                                       Where testata_griglia.Validita_Inizio <= Data_Movimento And
                          testata_griglia.Validita_Fine >= Data_Movimento And
                          prod_griglia.Mat_Cod = Mat_Cod And
                          (Tipo_Cod_Qualita = 0 OrElse prod_griglia.Tabella_Par_Cod_Qualita = Tipo_Cod_Qualita) And
                          (Tipo_Cod_Calibro = 0 OrElse prod_griglia.Tabella_Par_Cod_Calibro = Tipo_Cod_Calibro) And
                                           testata_griglia.PIVA.Equals(piva)
                                       Select testata_griglia.Id_TestataGriglia, prod_griglia.Id_TestataGriglia_Prod, testata_griglia.des_TestataGriglia).FirstOrDefault()
            'End If

            'Secondo tentativo con prodotto + qualità 
            If Testata_GrigliaElem Is Nothing Then
                Testata_GrigliaElem = (From prod_griglia In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                                       Join testata_griglia In GiasContext.CampionamentoConferito_TestataGriglia
                        On prod_griglia.Id_TestataGriglia Equals testata_griglia.Id_TestataGriglia
                                       Where testata_griglia.Validita_Inizio <= Data_Movimento And
                          testata_griglia.Validita_Fine >= Data_Movimento And
                          prod_griglia.Mat_Cod = Mat_Cod And
                          (Tipo_Cod_Qualita = 0 OrElse prod_griglia.Tabella_Par_Cod_Qualita = Tipo_Cod_Qualita) And
                                           testata_griglia.PIVA.Equals(piva)
                                       Select testata_griglia.Id_TestataGriglia, prod_griglia.Id_TestataGriglia_Prod, testata_griglia.des_TestataGriglia).FirstOrDefault()
            End If

            'Terzo tentativo con prodotto + calibro 
            If Testata_GrigliaElem Is Nothing Then
                Testata_GrigliaElem = (From prod_griglia In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                                       Join testata_griglia In GiasContext.CampionamentoConferito_TestataGriglia
                        On prod_griglia.Id_TestataGriglia Equals testata_griglia.Id_TestataGriglia
                                       Where testata_griglia.Validita_Inizio <= Data_Movimento And
                          testata_griglia.Validita_Fine >= Data_Movimento And
                          prod_griglia.Mat_Cod = Mat_Cod And
                          (Tipo_Cod_Calibro = 0 OrElse prod_griglia.Tabella_Par_Cod_Calibro = Tipo_Cod_Calibro) And
                                           testata_griglia.PIVA.Equals(piva)
                                       Select testata_griglia.Id_TestataGriglia, prod_griglia.Id_TestataGriglia_Prod, testata_griglia.des_TestataGriglia).FirstOrDefault()
            End If

            'Quarto tentativo con il solo prodotto
            If Testata_GrigliaElem Is Nothing Then
                Testata_GrigliaElem = (From prod_griglia In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                                       Join testata_griglia In GiasContext.CampionamentoConferito_TestataGriglia
                        On prod_griglia.Id_TestataGriglia Equals testata_griglia.Id_TestataGriglia
                                       Where testata_griglia.Validita_Inizio <= Data_Movimento And
                          testata_griglia.Validita_Fine >= Data_Movimento And
                          prod_griglia.Mat_Cod = Mat_Cod And
                          prod_griglia.Tabella_Par_Cod_Qualita = 0 And
                          prod_griglia.Tabella_Par_Cod_Calibro = 0 And
                                           testata_griglia.PIVA.Equals(piva)
                                       Select testata_griglia.Id_TestataGriglia, prod_griglia.Id_TestataGriglia_Prod, testata_griglia.des_TestataGriglia).FirstOrDefault()
            End If

            If Testata_GrigliaElem Is Nothing Then
                Id_Testata_Griglia_Trovata = 0
                Id_Testata_Griglia_Prod_Trovata = 0
                If ControllaPresenzaGrigliaCampionamento Then
                    Throw New Exception("Non esiste una griglia di campionamento per il prodotto " & MatPrimaElem.Mat_Cod.ToString() & " " & MatPrimaElem.Mat_Des.ToString())
                End If
            Else

                Id_Testata_Griglia_Trovata = Testata_GrigliaElem.Id_TestataGriglia
                Id_Testata_Griglia_Prod_Trovata = Testata_GrigliaElem.Id_TestataGriglia_Prod

                If ControllaPresenzaCalibri Then
                    Dim CampionaRiga = From campconfer_calibri In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
                                       Where campconfer_calibri.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                                         campconfer_calibri.PIVA.Equals(piva) AndAlso
                                         campconfer_calibri.Id_TestataGriglia = Testata_GrigliaElem.Id_TestataGriglia
                                       Select campconfer_calibri.Id_Calibro

                    If CampionaRiga.Count() = 0 Then
                        Throw New Exception("Griglia calibri non trovata per il prodotto " & MatPrimaElem.Mat_Cod.ToString() & " " & MatPrimaElem.Mat_Des.ToString())
                    End If
                End If
            End If

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Testata_GrigliaElem, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function LeggiBlocchiCampionamentoConferito(ByVal piva_superuser As String,
                                                       ByVal piva As String,
                                                       ByVal id_agenda As Integer,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.LeggiBlocchiCampionamentoConferito"
        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT Distinct CampionamentoConferito_Movimenti.* FROM CampionamentoConferito_Movimenti ")
            stbSql.AppendLine(" WHERE Piva_SuperUser = '" & piva_superuser & "' ")
            stbSql.AppendLine(" And Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stbSql.AppendLine(" And Id_Mov_Det In (Select Id_Mov_Det From Agenda, Movimenti_Dettagli  ")
            stbSql.AppendLine("                    Where Agenda.Piva = Movimenti_Dettagli.Piva AND  ")
            stbSql.AppendLine("                    Agenda.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND  ")
            stbSql.AppendLine("                    Agenda.Id_Agenda = " & Agro_SQL_SaveNum(id_agenda) & ")")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '##############################################################################################
    Public Function Leggi_CampionamentoConferito_Movimenti_Testata(
                ByVal piva As String,
                ByVal ID_Mov_Det As Integer,
                ByRef objParametri As AgronicaCoreParametri
                ) As CampionamentoConferito_Movimenti

        Dim Movimento As CampionamentoConferito_Movimenti = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_CampionamentoConferito_Movimenti_Testata()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Movimento =
            (From mov In GiasContext.CampionamentoConferito_Movimenti
             Where mov.Piva_SuperUser = Piva_SuperUser AndAlso
                   mov.Id_Mov_Det = ID_Mov_Det AndAlso
                   mov.PIVA.Equals(piva)).FirstOrDefault()

        End Using

        Return Movimento

    End Function

    '##############################################################################################
    Public Function Leggi_CampionamentoConferito_Movimenti_Righe(
                ByVal piva As String,
                ByVal ID_Mov_Det As Integer,
                ByVal Id_Testata_Griglia_Prod As Integer,
                ByVal Id_Testata_Griglia As Integer,
                ByRef objParametri As AgronicaCoreParametri
                ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_CampionamentoConferito_Movimenti_Righe()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim MovimentoDettElem =
            (From mov_dettaglio In GiasContext.Movimenti_dettagli
             Where mov_dettaglio.Id_Mov_Det = ID_Mov_Det AndAlso
                 mov_dettaglio.PIVA.Equals(piva)).FirstOrDefault()
            If MovimentoDettElem Is Nothing Then
                Throw New Exception("Riga Movimento con chiave " & ID_Mov_Det & " non trovata")
            End If

            ' Cerco se esiste già un campionamento con un altro id griglia prodotti
            '   capita se dopo al campionamento vanno sulla riga di entrata a cambiare il prodotto
            '   oppure una delle sue caratteristiche che determinano la griglia
            Dim CampionamentoMovimentoDettElem =
            (From mov_camp In GiasContext.CampionamentoConferito_Movimenti
             Where mov_camp.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                 mov_camp.PIVA.Equals(piva) AndAlso
                 mov_camp.Id_Mov_Det = ID_Mov_Det
                 ).FirstOrDefault()
            If CampionamentoMovimentoDettElem IsNot Nothing Then
                If CampionamentoMovimentoDettElem.Id_TestataGriglia_Prod <> Id_Testata_Griglia_Prod Then
                    Dim TestataGrigliaCampOld =
                        (From tgcold In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                         Where tgcold.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                                tgcold.PIVA.Equals(piva) AndAlso
                                tgcold.Id_TestataGriglia_Prod = CampionamentoMovimentoDettElem.Id_TestataGriglia_Prod
                        ).FirstOrDefault()
                    Dim TestataGrigliaCampNew =
                        (From tgcnew In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                         Where tgcnew.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                                tgcnew.PIVA.Equals(piva) AndAlso
                                tgcnew.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod
                        ).FirstOrDefault()
                    If TestataGrigliaCampOld Is Nothing OrElse
                       TestataGrigliaCampNew Is Nothing Then
                        Throw New Exception("Errore durante la lettura delle griglie di campionamento. <br/> Contattare l'assistenza")
                    Else
                        If TestataGrigliaCampOld.Id_TestataGriglia = TestataGrigliaCampNew.Id_TestataGriglia Then
                            ' La griglia di conferimento è la stessa, aggiorno sui movimenti Id_TestataGriglia_Prod
                            Dim campConf_W As New FF_CampionamentoConferimento_W
                            campConf_W.Aggiorna_ID_TestataGriglia_Prod_Su_Campioni_RigaConferimento(CampionamentoMovimentoDettElem, Id_Testata_Griglia_Prod, objParametri)
                        Else
                            'TODO: non deve bloccare ma permettere di correggere
                            Throw New Exception("Prodotto o caratteristiche modificate rispetto a quelle con cui è avvenuto il campionamento. <br/> I dati attuali riportano ad una griglia di camp. diversa: cancellare il campionamento e reinserirlo")
                        End If
                    End If

                End If
            End If

            Dim CampionaRiga =
               From campconf_calibri In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
               Group Join campconf_mov_righe In GiasContext.CampionamentoConferito_Movimenti_Righe.Where(Function(x) x.Id_Mov_Det = ID_Mov_Det AndAlso x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod)
                   On campconf_calibri.PIVA Equals campconf_mov_righe.PIVA And
                   campconf_calibri.Id_Calibro Equals campconf_mov_righe.Id_Calibro
                    Into _campconf_mov_righe = Group
               From _cmr In _campconf_mov_righe.DefaultIfEmpty()
               Group Join campconf_mov In GiasContext.CampionamentoConferito_Movimenti
                        On _cmr.PIVA Equals campconf_mov.PIVA And
                           _cmr.Id_Mov_Det Equals campconf_mov.Id_Mov_Det And
                           _cmr.Id_TestataGriglia_Prod Equals campconf_mov.Id_TestataGriglia_Prod Into _campconf_mov = Group
               From _cm In _campconf_mov.DefaultIfEmpty()
               Where campconf_calibri.PIVA.Equals(piva) AndAlso
                        campconf_calibri.Id_TestataGriglia = Id_Testata_Griglia
               Order By campconf_calibri.Ordinamento
               Select New With {
                   .Id_Calibro = campconf_calibri.Id_Calibro,
                   .Descrizione = If(campconf_calibri.Descr_qualita Is Nothing, campconf_calibri.Descr_calibro, campconf_calibri.Descr_qualita & " " & campconf_calibri.Descr_calibro),
                   .PercentualeCampionato = If(_cmr Is Nothing, 0, _cmr.PercentualeCampionato),
                   .SviluppoCampionato = If(_cmr Is Nothing AndAlso _cm Is Nothing, 0, _cm.QtaCampionata / 100 * _cmr.PercentualeCampionato),
                   .SviluppoTotale = If(_cmr Is Nothing AndAlso _cm Is Nothing, 0, (MovimentoDettElem.Qta_Extra_Totale - MovimentoDettElem.Qta_Extra_Totale / 100 * MovimentoDettElem.Variazione) / 100 * _cmr.PercentualeCampionato),
                   .Qualita = If(campconf_calibri.Descr_qualita, ""),
                   .Calibro = campconf_calibri.Descr_calibro,
                   .DescrPeso = If(campconf_calibri.Peso, "")
               }

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(CampionaRiga.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function


    '##############################################################################################
    Public Function Leggi_Righe_Conferimento_E_Calibri(
                ByVal piva As String,
                ByVal _docNumeroSin As String, ByVal _docNumero As Integer,
                ByVal _docNumeroDes As String, ByVal _nrRiga As Integer,
                ByVal _dataMovDal As String, ByVal _dataMovAl As String,
                ByVal _specie As Integer, ByVal _varieta As Integer(),
                ByVal _matcod As Integer, ByVal _certificazione As Integer,
                ByVal _qualita As Integer, ByVal _calibro As Integer,
                ByVal _operazioni As Integer(),
                ByVal _fornitori As String(),
                ByVal gruppoFatturazione As Integer(),
                ByVal xOrderBy As String,
                ByVal TipoCampionamentoIniziale As Short,
                ByVal StatoCampionamentoIniziale As String,
                ByVal soloCampionate As Boolean,
                ByVal Id_Testata_Griglia_Prod_Filtro As Integer,
                ByRef objParametri As AgronicaCoreParametri
                ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim _filtroSuVarieta As Boolean = False
        If _varieta IsNot Nothing AndAlso _varieta.Length > 0 Then
            _filtroSuVarieta = True
        End If

        Dim _filtroSuFornitori As Boolean = False
        If _fornitori IsNot Nothing AndAlso _fornitori.Length > 0 Then
            _filtroSuFornitori = True
        End If

        Dim _filtroSuOperazioni As Boolean = False
        If _operazioni IsNot Nothing AndAlso _operazioni.Length > 0 Then
            _filtroSuOperazioni = True
        End If

        Dim _filtroSuGruppoFatturazione As Boolean = False
        If gruppoFatturazione IsNot Nothing AndAlso gruppoFatturazione.Length > 0 Then
            _filtroSuGruppoFatturazione = True
        End If

        Dim dataMovDalDateTime As Nullable(Of DateTime)
        dataMovDalDateTime = Nothing
        If Not String.IsNullOrEmpty(_dataMovDal) Then
            dataMovDalDateTime = Convert.ToDateTime(_dataMovDal)
        End If

        Dim dataMovAlDateTime As Nullable(Of DateTime)
        dataMovAlDateTime = Nothing
        If Not String.IsNullOrEmpty(_dataMovAl) Then
            dataMovAlDateTime = Convert.ToDateTime(_dataMovAl)
        End If

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Righe_Conferimento_E_Calibri()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim campionamentoConferito_Movimenti As DbSet(Of CampionamentoConferito_Movimenti) = GiasContext.CampionamentoConferito_Movimenti
            Dim movimenti_dettagli As DbSet(Of Movimenti_dettagli) = GiasContext.Movimenti_dettagli
            Dim movimenti As DbSet(Of Movimenti) = GiasContext.Movimenti
            Dim agenda As DbSet(Of Agenda) = GiasContext.Agenda
            Dim contatti As DbSet(Of Contatti) = GiasContext.Contatti
            Dim risorse_umane As DbSet(Of Risorse_Umane) = GiasContext.Risorse_Umane
            Dim matPrima As DbSet(Of Materie_Prime) = GiasContext.Materie_Prime
            Dim matPrimeCampionature As DbSet(Of Materie_Prime_Campionature) = GiasContext.Materie_Prime_Campionature
            Dim tabelleParametri As DbSet(Of OTabelle_Parametri) = GiasContext.OTabelle_Parametri
            Dim matprimadett As DbSet(Of Materie_Prime_Dettagli) = GiasContext.Materie_Prime_Dettagli

            Dim lavCodConf As Integer?() = {LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE}

            Dim RigheConferimento =
                From ag In agenda
                Join mov In movimenti
                    On
                     ag.PIVA Equals mov.PIVA And
                     ag.Id_Agenda Equals mov.Id_Agenda
                Join r_u In risorse_umane
                    On
                     mov.Cod_RisUm Equals r_u.Cod_RisUm
                Join cont In contatti
                    On
                     r_u.Piva Equals cont.Piva And
                     r_u.Cod_Contatto Equals cont.Cod_Contatto
                Join mov_det In movimenti_dettagli
                    On
                     ag.PIVA Equals mov_det.PIVA And
                     mov.Id_Agenda Equals mov_det.Id_Agenda And
                     mov.Id_Mov Equals mov_det.Id_Mov
                Join mat_prima In matPrima
                    On
                     mov_det.Elem_Cod Equals mat_prima.Elem_Cod And
                     mov_det.Mat_Cod Equals mat_prima.Mat_Cod
                Group Join mat_prima_dett In matprimadett
                    On mat_prima_dett.Mat_Cod Equals mat_prima.Mat_Cod
                    Into mat_prima_dett_group = Group
                From _mat_prima_dett In mat_prima_dett_group.DefaultIfEmpty()
                Group Join otab_param_grpfatt In tabelleParametri.Where(Function(x) x.Tabella_Cod = 20 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On otab_param_grpfatt.Tabella_Par_Cod Equals _mat_prima_dett.Extra_Int1
                    Into otab_param_grpfatt_group = Group
                From _otab_param_grpfatt In otab_param_grpfatt_group.DefaultIfEmpty()
                Group Join mat_prime_camp_certificazioni In matPrimeCampionature.Where(Function(x) x.Tipo = "ocertificazioni")
                    On mat_prime_camp_certificazioni.Progressivo Equals mov_det.Cal_Cod
                    Into otab_param_certificazioni_group = Group
                From _otab_param_certificazioni In otab_param_certificazioni_group.DefaultIfEmpty()
                Group Join otab_param_cert In tabelleParametri.Where(Function(x) x.Tabella_Cod = 12 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On
                        otab_param_cert.Tabella_Par_Cod Equals _otab_param_certificazioni.Tipo_Cod
                    Into otab_param_cert_group = Group
                From _otp_cert In otab_param_cert_group.DefaultIfEmpty()
                Group Join mat_prime_camp_qualita In matPrimeCampionature.Where(Function(x) x.Tipo = "oqualità")
                    On mat_prime_camp_qualita.Progressivo Equals mov_det.Cal_Cod
                    Into otab_param_qualita_group = Group
                From _otab_param_qualita In otab_param_qualita_group.DefaultIfEmpty()
                Group Join otab_param_qual In tabelleParametri.Where(Function(x) x.Tabella_Cod = 3 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On
                     otab_param_qual.Tabella_Par_Cod Equals _otab_param_qualita.Tipo_Cod
                    Into otab_param_qual_group = Group
                From _otp_qual In otab_param_qual_group.DefaultIfEmpty()
                Group Join mat_prime_camp_calibri In matPrimeCampionature.Where(Function(x) x.Tipo = "ocalibro")
                    On
                     mat_prime_camp_calibri.Progressivo Equals mov_det.Cal_Cod
                    Into otab_param_calibri_group = Group
                From _otab_param_calibri In otab_param_calibri_group.DefaultIfEmpty()
                Group Join otab_param_calibro In tabelleParametri.Where(Function(x) x.Tabella_Cod = 1 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On
                     otab_param_calibro.Tabella_Par_Cod Equals _otab_param_calibri.Tipo_Cod
                    Into otab_param_cal_group = Group
                From _otp_cal In otab_param_cal_group.DefaultIfEmpty()
                Group Join mat_prime_camp_rugginosita In matPrimeCampionature.Where(Function(x) x.Tipo = "orugginosita")
                    On
                    mat_prime_camp_rugginosita.Progressivo Equals mov_det.Cal_Cod
                Into mat_prime_camp_rugginosita_group = Group
                From _mpc_rugg In mat_prime_camp_rugginosita_group.DefaultIfEmpty()
                Group Join otab_param_rugg In tabelleParametri.Where(Function(x) x.Tabella_Cod = 22 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On
                    otab_param_rugg.Tabella_Par_Cod Equals _mpc_rugg.Tipo_Cod
                    Into otab_param_rugg_group = Group
                From _otp_rugg In otab_param_rugg_group.DefaultIfEmpty()
                Group Join campconf_mov In campionamentoConferito_Movimenti.Where(Function(x) x.Piva_SuperUser = Piva_SuperUser)
                    On
                    mov_det.PIVA Equals campconf_mov.PIVA And
                       mov_det.Id_Mov_Det Equals campconf_mov.Id_Mov_Det Into campconf_movimenti = Group
                From _cm In campconf_movimenti.DefaultIfEmpty()
                Group Join griglia_prodotti In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                    On
                    griglia_prodotti.Piva_SuperUser Equals _cm.Piva_SuperUser And
                    griglia_prodotti.PIVA Equals _cm.PIVA And
                        griglia_prodotti.Id_TestataGriglia_Prod Equals _cm.Id_TestataGriglia_Prod Into griglia_prod = Group
                From _gprod In griglia_prod.DefaultIfEmpty()
                Group Join campconf_calibri In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
                On
                    _gprod.Piva_SuperUser Equals campconf_calibri.Piva_SuperUser And
                     _gprod.PIVA Equals campconf_calibri.PIVA And
                        _gprod.Id_TestataGriglia Equals campconf_calibri.Id_TestataGriglia Into griglia_cal = Group
                From _gcal In griglia_cal.DefaultIfEmpty()
                Group Join testata_griglia In GiasContext.CampionamentoConferito_TestataGriglia
                    On
                    testata_griglia.Id_TestataGriglia Equals _gprod.Id_TestataGriglia And
                     testata_griglia.Piva_SuperUser Equals _gprod.Piva_SuperUser And
                     testata_griglia.PIVA Equals _gprod.PIVA Into test_griglia = Group
                From _testgriglia In test_griglia.DefaultIfEmpty()
                Group Join campconf_mov_righe In GiasContext.CampionamentoConferito_Movimenti_Righe
                   On
                    _gprod.Piva_SuperUser Equals campconf_mov_righe.Piva_SuperUser And
                    _gprod.PIVA Equals campconf_mov_righe.PIVA And
                    _gprod.Id_TestataGriglia_Prod Equals campconf_mov_righe.Id_TestataGriglia_Prod And
                   _gcal.Id_Calibro Equals campconf_mov_righe.Id_Calibro And
                   mov_det.Id_Mov_Det Equals campconf_mov_righe.Id_Mov_Det
                    Into _campconf_mov_righe = Group
                From _t2 In _campconf_mov_righe.DefaultIfEmpty()
                Group Join campconf_mov In GiasContext.CampionamentoConferito_Movimenti
                        On
                    _t2.PIVA Equals campconf_mov.PIVA And
                           _t2.Id_Mov_Det Equals campconf_mov.Id_Mov_Det And
                           _t2.Id_TestataGriglia_Prod Equals campconf_mov.Id_TestataGriglia_Prod Into _campconf_mov = Group
                From _t1 In _campconf_mov.DefaultIfEmpty()
                Where
                    ag.PIVA.Equals(piva) AndAlso
                    lavCodConf.Contains(ag.Lav_Cod) AndAlso
                    mov.PIVA.Equals(piva) AndAlso
                    mov_det.PIVA.Equals(piva) AndAlso
                    (String.IsNullOrEmpty(_dataMovDal) OrElse mov.Data_Movimento >= dataMovDalDateTime) AndAlso
                    (String.IsNullOrEmpty(_dataMovAl) OrElse mov.Data_Movimento <= dataMovAlDateTime) AndAlso
                    (String.IsNullOrEmpty(_docNumeroDes) OrElse mov.Doc_Numero_Des.Contains(_docNumeroDes)) AndAlso
                    (_docNumero = 0 OrElse mov.Doc_Numero = _docNumero) AndAlso
                    (String.IsNullOrEmpty(_docNumeroSin) OrElse mov.Doc_Numero_Sin.Contains(_docNumeroSin)) AndAlso
                    (mov.Cau_Mov = CAU_CARICO OrElse mov.Cau_Mov = CAU_REGISTRAZIONI_TERZIARIA) AndAlso
                    mov_det.Elem_Cod = TRASFORMATI_VEGETALI AndAlso
                    (_nrRiga = 0 OrElse mov_det.Ordine_Det = _nrRiga OrElse mov_det.Extra_Str.Equals(CStr(_nrRiga))) AndAlso
                    ((_filtroSuGruppoFatturazione = False) OrElse gruppoFatturazione.Contains(_mat_prima_dett.Extra_Int1)) AndAlso
                    (_specie = -1 OrElse mat_prima.Veg_Cod = _specie) AndAlso
                    ((Not _filtroSuVarieta) OrElse _varieta.Contains(mat_prima.Cul_Cod.Value)) AndAlso
                    ((Not _filtroSuOperazioni) OrElse _operazioni.Contains(ag.Lav_Cod.Value)) AndAlso
                    ((Not _filtroSuFornitori) OrElse _fornitori.Contains(cont.Cod_Contatto)) AndAlso
                    (_testgriglia Is Nothing OrElse
                        (_testgriglia.Validita_Inizio <= mov.Data_Movimento AndAlso _testgriglia.Validita_Fine >= mov.Data_Movimento)
                    ) AndAlso
                    (_matcod = 0 OrElse mov_det.Mat_Cod.Value = _matcod) AndAlso
                    (_certificazione = 0 OrElse _otab_param_certificazioni.Tipo_Cod = _certificazione) AndAlso
                    (_qualita = 0 OrElse _otab_param_qualita.Tipo_Cod = _qualita) AndAlso
                    (_calibro = 0 OrElse _otab_param_calibri.Tipo_Cod = _calibro) AndAlso
                    (Id_Testata_Griglia_Prod_Filtro = 0 OrElse (Not _gprod Is Nothing AndAlso _gprod.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Filtro)) AndAlso
                    (Not soloCampionate OrElse Not _t2 Is Nothing)
                Order By mov.Doc_Numero, mov_det.Ordine_Det, mov_det.Extra_Str, If(_gcal Is Nothing, "", CStr(_gcal.Ordinamento))
                Select New With
                {
                    .Key = CStr(mov_det.Id_Mov_Det) & "-" & CStr(If(_gcal Is Nothing, 0, _gcal.Id_Calibro)),
                    .Id_Calibro = If(_gcal Is Nothing, 0, _gcal.Id_Calibro),
                    .Id_Testata_Griglia_Prod = If(_gprod Is Nothing, 0, _gprod.Id_TestataGriglia_Prod),
                    .Data_Movimento = mov.Data_Movimento,
                    .Rag_Soc = cont.Rag_Soc,
                    .ConferimentoOAcquisto = If(r_u.Cod_Rapporto = -18, "Conferimento", "Acquisto"),
                    .Numero_E_Data_Movimento = "",
                    .Doc_Numero = mov.Doc_Numero_Sin & CStr(mov.Doc_Numero) & mov.Doc_Numero_Des,
                    .NrRiga = If(mov_det.Ordine_Det = 0, mov_det.Extra_Str, CStr(mov_det.Ordine_Det)),
                    .Mat_Cod = mat_prima.Mat_Cod,
                    .Mat_Des = mat_prima.Mat_Des,
                    .Calibro = CStr(If(_otp_cal Is Nothing, "", _otp_cal.Sigla)),
                    .Qualita = CStr(If(_otp_qual Is Nothing, "", _otp_qual.Sigla)),
                    .Certificazione = CStr(If(_otp_cert Is Nothing, "", _otp_cert.Sigla)),
                    .Rugginosita = CStr(If(_otp_rugg Is Nothing, "", _otp_rugg.Descrizione)),
                    .Referenza = "",
                    .Lotto = mov_det.Lotto,
                    .Ordinamento = If(_gcal Is Nothing, "", CStr(_gcal.Ordinamento)),
                    .Campione = If(_gcal Is Nothing, "", If(_gcal.Descr_qualita Is Nothing, _gcal.Descr_calibro, _gcal.Descr_qualita & " " & _gcal.Descr_calibro)),
                    .PercentualeCampionato = If(_t2 Is Nothing, 0, _t2.PercentualeCampionato),
                    .SviluppoCampionato = If(_t2 Is Nothing AndAlso _t1 Is Nothing, 0, _t1.QtaCampionata / 100 * _t2.PercentualeCampionato),
                    .SviluppoTotale = If(_gcal Is Nothing, mov_det.Qta_Extra_Totale - mov_det.Qta_Extra_Totale / 100 * mov_det.Variazione, If(_t2 Is Nothing AndAlso _t1 Is Nothing, 0, (mov_det.Qta_Extra_Totale - mov_det.Qta_Extra_Totale / 100 * mov_det.Variazione) / 100 * _t2.PercentualeCampionato)),
                    .Automatico = If(_cm Is Nothing, TipoCampionamentoIniziale, _cm.Automatico)
                }

            'If Id_Testata_Griglia_Prod_Filtro <> 0 Then
            '    RigheConferimento = From results In RigheConferimento
            '                        Where results.Id_Testata_Griglia_Prod = Id_Testata_Griglia_Prod_Filtro
            '                        Select New With
            '    {
            '        .Key = results.Key,
            '        .Id_Calibro = results.Id_Calibro,
            '        .Id_Testata_Griglia_Prod = results.Id_Testata_Griglia_Prod,
            '        .Data_Movimento = results.Data_Movimento,
            '        .Rag_Soc = results.Rag_Soc,
            '        .Numero_E_Data_Movimento = results.Numero_E_Data_Movimento,
            '        .Doc_Numero = results.Doc_Numero,
            '        .NrRiga = results.NrRiga,
            '        .Mat_Cod = results.Mat_Cod,
            '        .Mat_Des = results.Mat_Des,
            '         .Calibro = results.Calibro,
            '         .Qualita = results.Qualita,
            '         .Certificazione = results.Certificazione,
            '         .Rugginosita = results.Rugginosita,
            '         .Referenza = results.Referenza,
            '         .Lotto = results.Lotto,
            '         .Ordinamento = results.Ordinamento,
            '         .Campione = results.Campione,
            '         .PercentualeCampionato = results.PercentualeCampionato,
            '         .SviluppoCampionato = results.SviluppoCampionato,
            '        .SviluppoTotale = results.SviluppoTotale,
            '        .Automatico = results.Automatico
            '     }
            'End If

            'If soloCampionate Then
            '    RigheConferimento = From results In RigheConferimento
            '                        Where Not results.Campione Is Nothing And
            '                       results.Campione <> ""
            '                        Select New With
            '    {
            '        .Key = results.Key,
            '        .Id_Calibro = results.Id_Calibro,
            '        .Id_Testata_Griglia_Prod = results.Id_Testata_Griglia_Prod,
            '        .Data_Movimento = results.Data_Movimento,
            '        .Rag_Soc = results.Rag_Soc,
            '        .Numero_E_Data_Movimento = results.Numero_E_Data_Movimento,
            '        .Doc_Numero = results.Doc_Numero,
            '        .NrRiga = results.NrRiga,
            '        .Mat_Cod = results.Mat_Cod,
            '        .Mat_Des = results.Mat_Des,
            '         .Calibro = results.Calibro,
            '         .Qualita = results.Qualita,
            '         .Certificazione = results.Certificazione,
            '         .Rugginosita = results.Rugginosita,
            '         .Referenza = results.Referenza,
            '         .Lotto = results.Lotto,
            '         .Ordinamento = results.Ordinamento,
            '         .Campione = results.Campione,
            '         .PercentualeCampionato = results.PercentualeCampionato,
            '         .SviluppoCampionato = results.SviluppoCampionato,
            '        .SviluppoTotale = results.SviluppoTotale,
            '        .Automatico = results.Automatico
            '     }
            'End If

            ' Necessario perché più di 4 stringhe non riesce a concatenarle e qui ce ne sono 7 (spazi intermedi compresi)
            Dim myList = RigheConferimento.ToList()
            For Each obj In myList
                If obj.Campione <> "" Then
                    obj.Campione = obj.Ordinamento.PadLeft(3, "0") & " " & obj.Campione
                End If
                obj.Referenza = obj.Mat_Des & " " & obj.Qualita & " " & obj.Calibro & " " & obj.Rugginosita
                obj.Numero_E_Data_Movimento = obj.Doc_Numero & " " & obj.Data_Movimento
            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Controllo_Esistenza_CampionamentoConferito_Movimenti(
                ByVal piva As String,
                ByVal ID_Mov_Det As Integer,
                ByVal Id_Testata_Griglia As Integer,
                ByVal Id_Testata_Griglia_Prod As Integer,
                ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim countMov As Integer = 0

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Controllo_Esistenza_CampionamentoConferito_Movimenti()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim CampionamentoConferito_Movimenti =
            (From mov In GiasContext.CampionamentoConferito_Movimenti
             Join tg_prod In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                     On mov.Piva_SuperUser Equals tg_prod.Piva_SuperUser And
                 mov.PIVA Equals tg_prod.PIVA And
                 mov.Id_TestataGriglia_Prod Equals tg_prod.Id_TestataGriglia_Prod
             Where mov.Piva_SuperUser = Piva_SuperUser AndAlso
                mov.PIVA = piva AndAlso
                 (ID_Mov_Det = 0 OrElse mov.Id_Mov_Det = ID_Mov_Det) AndAlso
                 (Id_Testata_Griglia = 0 OrElse tg_prod.Id_TestataGriglia = Id_Testata_Griglia) AndAlso
                 (Id_Testata_Griglia_Prod = 0 OrElse mov.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod))

            countMov = CampionamentoConferito_Movimenti.Count()

        End Using

        Return countMov

    End Function

    '##############################################################################################
    Public Function Controllo_Esistenza_CampionamentoConferito_Movimenti_Righe(
                ByVal piva As String,
                ByVal ID_Mov_Det As Integer,
                ByVal Id_Testata_Griglia As Integer,
                ByVal Id_Testata_Griglia_Prod As Integer,
                ByVal Id_Calibro As Integer,
                ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim movCount As Integer = 0

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Controllo_Esistenza_CampionamentoConferito_Movimenti_Righe()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim CampionamentoConferito_Movimenti_Righe =
            (From mov_righe In GiasContext.CampionamentoConferito_Movimenti_Righe
             Join tg_prod In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                     On mov_righe.Piva_SuperUser Equals tg_prod.Piva_SuperUser And
                 mov_righe.PIVA Equals tg_prod.PIVA And
                 mov_righe.Id_TestataGriglia_Prod Equals tg_prod.Id_TestataGriglia_Prod
             Where mov_righe.Piva_SuperUser = Piva_SuperUser AndAlso mov_righe.PIVA = piva AndAlso
                 (ID_Mov_Det = 0 OrElse mov_righe.Id_Mov_Det = ID_Mov_Det) AndAlso
                 (Id_Testata_Griglia = 0 OrElse tg_prod.Id_TestataGriglia = Id_Testata_Griglia) AndAlso
                 (Id_Testata_Griglia_Prod = 0 OrElse mov_righe.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod) AndAlso
                 (Id_Calibro = 0 OrElse mov_righe.Id_Calibro = Id_Calibro))

            movCount = CampionamentoConferito_Movimenti_Righe.Count()

        End Using

        Return movCount

    End Function


    '##############################################################################################
    Public Function Controllo_Esistenza_Listini_Calibri(
                ByVal piva As String,
                ByVal Id_Testata_Griglia As Integer,
                ByVal Id_Calibro As Integer,
                ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim listCount As Integer = 0

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Controllo_Esistenza_Listini_Calibri()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Listini_Calibri =
            (From cal In GiasContext.Listini_CampionamentoConferito_Dettagli
             Where cal.Piva_SuperUser = Piva_SuperUser AndAlso cal.PIVA = piva AndAlso
                 (cal.Id_TestataGriglia = Id_Testata_Griglia) AndAlso
                 (cal.Id_Calibro = Id_Calibro))

            listCount = Listini_Calibri.Count()

        End Using

        Return listCount

    End Function

    '##############################################################################################
    Public Function Controllo_Esistenza_Prodotti_Calibri_Per_TestataGriglia(
                ByVal piva As String,
                ByVal Id_Testata_Griglia As Integer,
                ByRef calibriTrovati As Integer,
                ByRef prodottiTrovati As Integer,
                ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Controllo_Esistenza_Prodotti_Calibri_Per_TestataGriglia()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim CampionamentoConferito_TestataGriglia_Calibri =
            (From righe In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
             Where righe.Piva_SuperUser = Piva_SuperUser AndAlso righe.PIVA = piva AndAlso
                 (righe.Id_TestataGriglia = Id_Testata_Griglia))
            calibriTrovati = CampionamentoConferito_TestataGriglia_Calibri.Count()

            Dim CampionamentoConferito_TestataGriglia_Prod =
                (From righe In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                 Where righe.Piva_SuperUser = Piva_SuperUser AndAlso righe.PIVA = piva AndAlso
                     (righe.Id_TestataGriglia = Id_Testata_Griglia))
            prodottiTrovati = CampionamentoConferito_TestataGriglia_Prod.Count()

        End Using

        If calibriTrovati > 0 OrElse prodottiTrovati > 0 Then
            Return True
        End If
        Return False

    End Function

    Public Function LeggiElem_CampionamentoRigaConferito(ByVal piva As String,
                                                         ByVal Id_Mov_Det As Integer,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         ) As CampionamentoConferito_Movimenti

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.LeggiElem_CampionamentoRigaConferito()"
        Dim messaggioErrore As String = ""
        Dim TestataElem As CampionamentoConferito_Movimenti = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                TestataElem =
                (From gconf_mov In GiasContext.CampionamentoConferito_Movimenti
                 Where gconf_mov.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                       gconf_mov.PIVA.Equals(piva) AndAlso
                       gconf_mov.Id_Mov_Det = Id_Mov_Det
                 Select gconf_mov).FirstOrDefault()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return TestataElem

    End Function

    '##############################################################################################
    Public Function Leggi_Listini_Prezzi(ByVal piva As String,
                                         ByVal descr As String,
                                         ByVal DataRif As String,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As String

        Dim risposta As String = ""

        Dim DataRifDateTime As Nullable(Of DateTime)
        DataRifDateTime = Nothing
        If Not String.IsNullOrEmpty(DataRif) Then
            DataRifDateTime = Convert.ToDateTime(DataRif)
        End If

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Listini_Prezzi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Listini_Prezzi =
           From list_prezzi In GiasContext.Listini_Prezzi
           Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
               On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
               list_classi_prezzi.Piva Equals list_prezzi.Piva And
               list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
           Where
               list_classi_prezzi.Tipo_Classe = 1 AndAlso
               list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
               list_prezzi.Piva.Equals(piva) AndAlso
               list_prezzi.Listino_Des.Contains(descr) AndAlso
               (DataRifDateTime Is Nothing Or (list_prezzi.Validita_Inizio <= DataRifDateTime And list_prezzi.Validita_Fine >= DataRifDateTime))
           Select
                   list_classi_prezzi.Listino_Classe_Des, list_prezzi.Listino_Cod, list_prezzi.Listino_Cod_Des, list_prezzi.Listino_Des, list_prezzi.Validita_Inizio, list_prezzi.Validita_Fine

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Listini_Prezzi.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Listini_Prezzi_Prodotti(ByVal piva As String,
                                                  ByVal Listino_Cod As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Listini_Prezzi_Prodotti()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            '17/3/2020 NON SERVE PIU' PERCHE' SI UTILIZZA LA TABELLA Listini_CampionamentoConferito_X_Testata_Griglia
            'Dim Listino_Prezzi =
            '        (From list_prezzi In GiasContext.Listini_Prezzi
            '         Where
            '        list_prezzi.Piva_SuperUser = Piva_SuperUser And
            '        list_prezzi.Piva = piva And
            '        list_prezzi.Listino_Cod = Listino_Cod
            '         Select list_prezzi).FirstOrDefault()

            'Necessario fare la where così per poter mettere una condizione sulla variabile sulla tabella secondaria
            ' (Listino_Cod in questo caso)

            'Non vengono mostrati i prodotti che sono già fra quelli equivalenti
            ' 24/2/2018 ... NON PIU'
            '' Dim ListiniPrezziProdotti =
            ''From campconf_prodotti In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
            ''Join materie_prime In GiasContext.Materie_Prime
            ''    On
            ''         materie_prime.Piva Equals campconf_prodotti.PIVA And
            ''         materie_prime.Mat_Cod Equals campconf_prodotti.Mat_Cod
            ''Join campconf_griglia In GiasContext.CampionamentoConferito_TestataGriglia
            ''    On
            ''         campconf_griglia.Piva_SuperUser Equals campconf_prodotti.Piva_SuperUser And
            ''         campconf_griglia.PIVA Equals campconf_prodotti.PIVA And
            ''         campconf_griglia.Id_TestataGriglia Equals campconf_prodotti.Id_TestataGriglia
            ''Group Join list_prodotti In GiasContext.Listini_CampionamentoConferito_Prodotti.Where(Function(x) x.Listino_Cod = Listino_Cod)
            ''    On
            ''         campconf_prodotti.Piva_SuperUser Equals list_prodotti.Piva_SuperUser And
            ''         campconf_prodotti.PIVA Equals list_prodotti.PIVA And
            ''         campconf_prodotti.Id_TestataGriglia Equals list_prodotti.Id_TestataGriglia And
            ''         campconf_prodotti.Mat_Cod Equals list_prodotti.Mat_Cod
            '' Into _list_prodotti = Group
            ''From _lp In _list_prodotti.DefaultIfEmpty()
            ''Where campconf_prodotti.Piva_SuperUser.Equals(Piva_SuperUser) And
            ''         campconf_prodotti.PIVA.Equals(piva) And
            ''          Not GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Any(Function(y) _
            ''         y.Piva_SuperUser = campconf_prodotti.Piva_SuperUser And
            ''         y.PIVA = campconf_prodotti.PIVA And
            ''         y.Listino_Cod = Listino_Cod And
            ''         y.Id_TestataGriglia = campconf_prodotti.Id_TestataGriglia And
            ''         y.Mat_Cod_Equivalente = campconf_prodotti.Mat_Cod)
            ''Order By campconf_griglia.des_TestataGriglia, materie_prime.Mat_Des
            ''Select New With {
            ''    .KeyListinoProdotto = "",
            ''    .PresentiPrezzi = If(Not _lp Is Nothing, If(_lp.prezzo_su_conferito = "0", "Conferito", "Campionato"), ""),
            ''    .Id_TestataGriglia = campconf_griglia.Id_TestataGriglia,
            ''    .Mat_Cod = materie_prime.Mat_Cod,
            ''    .Mat_Des = materie_prime.Mat_Des,
            ''    .des_TestataGriglia = campconf_griglia.des_TestataGriglia,
            ''    .validita_Inizio = campconf_griglia.Validita_Inizio,
            ''    .validita_Fine = campconf_griglia.Validita_Fine
            ''}
            ' FINE 24/2/2018 ... NON PIU'

            'Non vengono mostrati i prodotti che sono già fra quelli equivalenti
            Dim ListiniPrezziProdotti =
           From campconf_prodotti In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
           Join materie_prime In GiasContext.Materie_Prime
               On
                    materie_prime.Mat_Cod Equals campconf_prodotti.Mat_Cod
           Join campconf_griglia In GiasContext.CampionamentoConferito_TestataGriglia
               On
                    campconf_griglia.Piva_SuperUser Equals campconf_prodotti.Piva_SuperUser And
                    campconf_griglia.PIVA Equals campconf_prodotti.PIVA And
                    campconf_griglia.Id_TestataGriglia Equals campconf_prodotti.Id_TestataGriglia
           Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Listino_Cod = Listino_Cod)
               On campconf_griglia.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                campconf_griglia.PIVA Equals listiniXgriglie.PIVA And
                campconf_griglia.Id_TestataGriglia Equals listiniXgriglie.Id_TestataGriglia
           Join specievegetali In GiasContext.SpecieVegetali On materie_prime.Veg_Cod Equals specievegetali.Veg_Cod
           Join varieta In GiasContext.Cultivar On materie_prime.Cul_Cod Equals varieta.Cul_Cod And
                                                    materie_prime.Veg_Cod Equals varieta.Veg_Cod
           Group Join list_prodotti_equivalenti In GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Where(Function(x) x.Listino_Cod = Listino_Cod)
               On
                    campconf_prodotti.Piva_SuperUser Equals list_prodotti_equivalenti.Piva_SuperUser And
                    campconf_prodotti.PIVA Equals list_prodotti_equivalenti.PIVA And
                    campconf_prodotti.Id_TestataGriglia_Prod Equals list_prodotti_equivalenti.Id_TestataGriglia_Prod
            Into _list_prodotti_equivalenti = Group
           From _lpe In _list_prodotti_equivalenti.DefaultIfEmpty()
           Group Join materie_prime_equiv In GiasContext.Materie_Prime
               On
                    materie_prime_equiv.Mat_Cod Equals _lpe.CampionamentoConferito_TestataGriglia_Prodotti.Mat_Cod
                Into _materie_prime_equiv = Group
           From _mppe In _materie_prime_equiv.DefaultIfEmpty()
           Group Join otab_param_calibro In GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 1 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On otab_param_calibro.Tabella_Par_Cod Equals campconf_prodotti.Tabella_Par_Cod_Calibro
                    Into otab_param_calibro_group = Group
           From _otp_calibro In otab_param_calibro_group.DefaultIfEmpty()
           Group Join otab_param_qual In GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 3 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On
                     otab_param_qual.Tabella_Par_Cod Equals campconf_prodotti.Tabella_Par_Cod_Qualita
                    Into otab_param_qual_group = Group
           From _otp_qual In otab_param_qual_group.DefaultIfEmpty()
           Group Join otab_param_calibro_equiv In GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 1 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On otab_param_calibro_equiv.Tabella_Par_Cod Equals _lpe.CampionamentoConferito_TestataGriglia_Prodotti.Tabella_Par_Cod_Calibro
                    Into otab_param_calibro_group_equiv = Group
           From _otp_calibro_equiv In otab_param_calibro_group_equiv.DefaultIfEmpty()
           Group Join otab_param_qual_equiv In GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 3 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On
                     otab_param_qual_equiv.Tabella_Par_Cod Equals _lpe.CampionamentoConferito_TestataGriglia_Prodotti.Tabella_Par_Cod_Qualita
                    Into otab_param_qual_group_equiv = Group
           From _otp_qual_equiv In otab_param_qual_group_equiv.DefaultIfEmpty()
           Order By campconf_griglia.des_TestataGriglia, materie_prime.Mat_Des, campconf_prodotti.Ordinamento
           Select New With {
               .KeyListinoProdotto = "",
               .PrezzoAssociato = If(Not _mppe Is Nothing, 1, 0),
               .PresentiPrezzi = "",
               .ProdottoEquivalente = If(Not _mppe Is Nothing, _mppe.Mat_Des, ""),
               .qualita_des_equiv = If(_otp_qual_equiv Is Nothing, "", _otp_qual_equiv.Descrizione),
               .calibro_des_equiv = If(_otp_calibro_equiv Is Nothing, "", _otp_calibro_equiv.Descrizione),
               .Id_TestataGriglia_Prod = campconf_prodotti.Id_TestataGriglia_Prod,
               .Cod_Articolo = materie_prime.Cod_Articolo,
               .Prodotto = materie_prime.Mat_Des,
               .Veg_Des = specievegetali.Veg_Des,
               .Cul_Des = varieta.Cul_Des,
               .Reg_Cod = materie_prime.Regolamento,
               .Reg_Des = If(materie_prime.Regolamento = enum_Cod_Regolamento.Regolamento_bio, "Biologico (Reg.CE 834/07 (Ex.Reg.CE 2092/91))", "Convenzionale (Reg. Nessuno)"),
               .Codice_Esterno = materie_prime.Codice_Esterno,
               .qualita_des = If(_otp_qual Is Nothing, "", _otp_qual.Descrizione),
               .calibro_des = If(_otp_calibro Is Nothing, "", _otp_calibro.Descrizione),
               .des_TestataGriglia = campconf_griglia.des_TestataGriglia,
               .validita_Inizio = campconf_griglia.Validita_Inizio,
               .validita_Fine = campconf_griglia.Validita_Fine
           }

            ' Compongo la chiave
            Dim myList = ListiniPrezziProdotti.ToList()
            For Each obj In myList
                obj.KeyListinoProdotto = piva & "-" & Listino_Cod.ToString() & "-" & obj.Id_TestataGriglia_Prod.ToString()
                Dim Listini_Prodotti =
                    (From list_prodotti In GiasContext.Listini_CampionamentoConferito_Prodotti
                     Where
                    list_prodotti.Piva_SuperUser = Piva_SuperUser AndAlso
                    list_prodotti.PIVA = piva AndAlso
                    list_prodotti.Id_TestataGriglia_Prod = obj.Id_TestataGriglia_Prod AndAlso
                    list_prodotti.Listino_Cod = Listino_Cod
                     Select list_prodotti).FirstOrDefault()

                If Listini_Prodotti IsNot Nothing Then
                    If Listini_Prodotti.prezzo_su_conferito = 1 Then
                        obj.PresentiPrezzi = "Conferito"
                    Else
                        obj.PresentiPrezzi = "Campionato"
                    End If
                    obj.PrezzoAssociato = 1
                End If

            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function




    Public Function LeggiListini_From_Veg_Cod(ByVal PIVA As String,
                                              ByVal Id_TestataGriglia As Integer,
                                              ByVal Veg_Cod As Integer,
                                              ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.LeggiListini_From_Veg_Cod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Distinct Listini_Prezzi.Piva,  Listini_Prezzi.Listino_Cod, Listini_Prezzi.Listino_Des ")
                    StrSQL.Append(" FROM  Listini_Prezzi, Listini_CampionamentoConferito_X_Testata_Griglia, CampionamentoConferito_TestataGriglia, CampionamentoConferito_TestataGriglia_Prodotti, Materie_Prime   ")
                    StrSQL.Append(" Where CampionamentoConferito_TestataGriglia.Piva_SuperUser = CampionamentoConferito_TestataGriglia_Prodotti.Piva_SuperUser ")

                    StrSQL.Append(" AND   CampionamentoConferito_TestataGriglia.Piva = Listini_CampionamentoConferito_X_Testata_Griglia.Piva ")
                    StrSQL.Append(" AND   CampionamentoConferito_TestataGriglia.Id_TestataGriglia = Listini_CampionamentoConferito_X_Testata_Griglia.Id_TestataGriglia ")

                    StrSQL.Append(" AND   Listini_Prezzi.Piva = Listini_CampionamentoConferito_X_Testata_Griglia.Piva ")
                    StrSQL.Append(" AND   Listini_Prezzi.Listino_Cod = Listini_CampionamentoConferito_X_Testata_Griglia.Listino_Cod ")


                    StrSQL.Append(" AND   CampionamentoConferito_TestataGriglia.Piva = CampionamentoConferito_TestataGriglia_Prodotti.Piva ")
                    StrSQL.Append(" AND   CampionamentoConferito_TestataGriglia.Id_TestataGriglia = CampionamentoConferito_TestataGriglia_Prodotti.Id_TestataGriglia ")
                    StrSQL.Append(" AND   CampionamentoConferito_TestataGriglia_Prodotti.Mat_Cod = Materie_Prime.Mat_Cod ")
                    StrSQL.Append(" AND   (Materie_Prime.Piva = CampionamentoConferito_TestataGriglia_Prodotti.Piva Or Materie_Prime.Sa_Cod = -1 ) ")
                    StrSQL.Append(" AND   CampionamentoConferito_TestataGriglia.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   CampionamentoConferito_TestataGriglia.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   CampionamentoConferito_TestataGriglia.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

                    If PIVA <> "" Then
                        StrSQL.Append(" AND CampionamentoConferito_TestataGriglia.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
                    End If

                    If Id_TestataGriglia <> 0 Then
                        StrSQL.Append(" AND CampionamentoConferito_TestataGriglia.Id_TestataGriglia = " & Agro_SQL_SaveNum(Id_TestataGriglia) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Materie_Prime.Veg_Cod = " & Veg_Cod & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append("ORDER BY Piva ASC, Listino_Des ASC ")
                    End If



            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return DT

    End Function



    '##############################################################################################
    Public Function Leggi_Elenco_Listini_Prodotti(ByVal piva As String,
                                                  ByVal key_Listino_Cod As Integer?,
                                                  ByVal key_Id_TestataGriglia_Prod As Integer,
                                                  ByVal prezzo_su_conferito As Boolean,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As List(Of Listini_CampionamentoConferito_Prodotti)

        Dim Righe As List(Of Listini_CampionamentoConferito_Prodotti) = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Elenco_Listini_Prodotti()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            GiasContext.Configuration.LazyLoadingEnabled = False

            If prezzo_su_conferito Then
                Righe = (From prod In GiasContext.Listini_CampionamentoConferito_Prodotti
                         Where
                               prod.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                               prod.PIVA.Equals(piva) AndAlso
                               (key_Listino_Cod Is Nothing OrElse prod.Listino_Cod = key_Listino_Cod) AndAlso
                               prod.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod AndAlso
                               prod.prezzo_su_conferito = 1
                            ).ToList()
            Else
                Righe = (From prod In GiasContext.Listini_CampionamentoConferito_Prodotti.Include("Listini_CampionamentoConferito_Dettagli")
                         Where
                               prod.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                               prod.PIVA.Equals(piva) AndAlso
                               (key_Listino_Cod Is Nothing OrElse prod.Listino_Cod = key_Listino_Cod) AndAlso
                               prod.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod AndAlso
                               prod.prezzo_su_conferito = 0).ToList()
            End If


        End Using

        Return Righe

    End Function

    '##############################################################################################
    Public Function Leggi_Intestazione_ListiniPerProdotto(ByVal piva As String,
                                                          ByVal KeyListinoProdotto As String,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As Hashtable

        Dim ht As New Hashtable

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim keys As String() = KeyListinoProdotto.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Id_TestataGriglia_Prod As Integer = Integer.Parse(keys(2))

        Dim descrCalibro = " - Calibro: "
        Dim descrQualita = " - Qualità: "

        Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        Dim dtParamQual As DataTable = objConfigDettagli.Leggi(piva, 0, False, "Tipo = 1", "", objParametri)
        For Each paramQual In dtParamQual.Rows
            If paramQual("Tabella_Key") = "calibro" Then
                descrCalibro = " - " & paramQual("Tabella_Des") & ": "
                Exit For
            End If

        Next



        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Intestazione_ListiniPerProdotto()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            ' Lettura listino
            Dim Listini_Prezzi =
           (From list_prezzi In GiasContext.Listini_Prezzi
            Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
               On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
               list_classi_prezzi.Piva Equals list_prezzi.Piva And
               list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
            Where
               list_classi_prezzi.Tipo_Classe = 1 AndAlso
               (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
               (list_prezzi.Piva.Equals(piva)) AndAlso
               (list_prezzi.Listino_Cod = key_Listino_Cod)
            Select
                list_classi_prezzi.Listino_Classe_Des, list_prezzi.Listino_Cod_Des, list_prezzi.Listino_Des, list_prezzi.Validita_Inizio, list_prezzi.Validita_Fine).FirstOrDefault()

            Dim TestataElemProd =
               (From griglia_testata_prod In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                Join matprime In GiasContext.Materie_Prime
                     On matprime.Mat_Cod Equals griglia_testata_prod.Mat_Cod
                Group Join otab_param_calibro In
                    GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 1 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                                        On otab_param_calibro.Tabella_Par_Cod Equals griglia_testata_prod.Tabella_Par_Cod_Calibro
                                            Into otab_param_calibro_group = Group
                From _otp_calibro In otab_param_calibro_group.DefaultIfEmpty()
                Group Join otab_param_qual In
                    GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 3 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                                        On otab_param_qual.Tabella_Par_Cod Equals griglia_testata_prod.Tabella_Par_Cod_Qualita
                                        Into otab_param_qual_group = Group
                From _otp_qual In otab_param_qual_group.DefaultIfEmpty()
                Where
                  griglia_testata_prod.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                  griglia_testata_prod.PIVA.Equals(piva) AndAlso
                  griglia_testata_prod.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod
                Select New With {
                        .id_TestataGriglia = griglia_testata_prod.CampionamentoConferito_TestataGriglia.Id_TestataGriglia,
                        .des_TestataGriglia = griglia_testata_prod.CampionamentoConferito_TestataGriglia.des_TestataGriglia,
                        .mat_des = matprime.Mat_Des & If(_otp_qual Is Nothing, "", descrQualita & _otp_qual.Descrizione) & If(_otp_calibro Is Nothing, "", descrCalibro & _otp_calibro.Descrizione)
                                }
            ).FirstOrDefault()

            Dim Listini_CampConf_ProdottiElem =
                (From list_camp_conf In GiasContext.Listini_CampionamentoConferito_Prodotti
                 Where
                        list_camp_conf.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                        list_camp_conf.PIVA.Equals(piva) AndAlso
                        list_camp_conf.Listino_Cod = key_Listino_Cod AndAlso
                        list_camp_conf.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod).FirstOrDefault()

            'Se non ho trovato elementi verifico se ci sono calibri associati alla griglia per decidere 
            ' che default mettere su prezzo su conferito
            Dim TestataElemProd_Calibri As New CampionamentoConferito_TestataGriglia_Calibri
            If TestataElemProd IsNot Nothing AndAlso
                Listini_CampConf_ProdottiElem Is Nothing Then

                TestataElemProd_Calibri =
                   (From griglia_testata_cal In GiasContext.CampionamentoConferito_TestataGriglia_Calibri Where
                        griglia_testata_cal.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                        griglia_testata_cal.PIVA.Equals(piva) AndAlso
                        griglia_testata_cal.CampionamentoConferito_TestataGriglia.Id_TestataGriglia = TestataElemProd.id_TestataGriglia).FirstOrDefault()
            End If

            If Listini_Prezzi Is Nothing Then
                ht.Add("Listino_Classe_Des", "Listino non trovato")
                ht.Add("Listino_Cod_Des", "Listino non trovato")
                ht.Add("Listino_Des", "Listino non trovato")
                ht.Add("Validita_Inizio", "")
                ht.Add("Validita_Fine", "")
            Else
                ht.Add("Listino_Classe_Des", Listini_Prezzi.Listino_Classe_Des)
                ht.Add("Listino_Cod_Des", Listini_Prezzi.Listino_Cod_Des)
                ht.Add("Listino_Des", Listini_Prezzi.Listino_Des)
                ht.Add("Validita_Inizio", Listini_Prezzi.Validita_Inizio)
                ht.Add("Validita_Fine", Listini_Prezzi.Validita_Fine)
            End If
            If TestataElemProd Is Nothing Then
                ht.Add("Descr_griglia", "Griglia campionamento non trovata")
                ht.Add("Prodotto", "Prodotto non trovato")
            Else
                ht.Add("Descr_griglia", TestataElemProd.des_TestataGriglia)
                ht.Add("Prodotto", TestataElemProd.mat_des)
            End If
            If Listini_CampConf_ProdottiElem Is Nothing Then
                If TestataElemProd_Calibri Is Nothing Then
                    ht.Add("PrezzoSuConferito", 1)
                Else
                    ht.Add("PrezzoSuConferito", 0)
                End If
            Else
                ht.Add("PrezzoSuConferito", Listini_CampConf_ProdottiElem.prezzo_su_conferito)
            End If

        End Using

        Return ht

    End Function

    '##############################################################################################
    Public Function Leggi_Listino_Prodotto_Elem(ByVal piva As String,
                                                ByVal KeyListinoProdotto As String,
                                                ByVal dataInizio As Date,
                                                ByVal dataFine As Date,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As Listini_CampionamentoConferito_Prodotti

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim keys As String() = KeyListinoProdotto.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Id_TestataGriglia_Prod As Integer = Integer.Parse(keys(2))

        Dim Listini_CampConf_ProdottiElem As Listini_CampionamentoConferito_Prodotti = Nothing

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Intestazione_ListiniPerProdotto()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Listini_CampConf_ProdottiElem =
                (From list_camp_conf In GiasContext.Listini_CampionamentoConferito_Prodotti
                 Where
                        list_camp_conf.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                        list_camp_conf.PIVA.Equals(piva) AndAlso
                        list_camp_conf.Listino_Cod = key_Listino_Cod AndAlso
                        list_camp_conf.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod AndAlso
                        list_camp_conf.Validita_Inizio = dataInizio AndAlso
                        list_camp_conf.Validita_Fine = dataFine
                     ).FirstOrDefault()

        End Using

        Return Listini_CampConf_ProdottiElem

    End Function

    '##############################################################################################
    Public Function Controlla_Date_Listino_Prodotto(
                ByVal listini_campConf_prodotti As Listini_CampionamentoConferito_Prodotti,
                ByRef objParametri As AgronicaCoreParametri
                ) As Integer

        Dim countTrovati As Integer = 0

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Controlla_Date_Listino_Prodotto()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ListProdotto = From l_prod In GiasContext.Listini_CampionamentoConferito_Prodotti
                               Where
                                   l_prod.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                                    l_prod.PIVA.Equals(listini_campConf_prodotti.PIVA) AndAlso
                                    l_prod.Listino_Cod = listini_campConf_prodotti.Listino_Cod AndAlso
                                    l_prod.Id_TestataGriglia_Prod = listini_campConf_prodotti.Id_TestataGriglia_Prod AndAlso
                                    ((listini_campConf_prodotti.Validita_Inizio <= l_prod.Validita_Inizio AndAlso
                                      listini_campConf_prodotti.Validita_Fine >= l_prod.Validita_Inizio) OrElse
                                     (listini_campConf_prodotti.Validita_Inizio <= l_prod.Validita_Fine AndAlso
                                       listini_campConf_prodotti.Validita_Fine >= l_prod.Validita_Fine) OrElse
                                     (listini_campConf_prodotti.Validita_Inizio >= l_prod.Validita_Inizio AndAlso
                                       listini_campConf_prodotti.Validita_Fine <= l_prod.Validita_Fine)) AndAlso
                                    l_prod.prezzo_su_conferito = listini_campConf_prodotti.prezzo_su_conferito
                               Select l_prod

            countTrovati = ListProdotto.Count()

        End Using

        Return countTrovati

    End Function

    '##############################################################################################
    Public Function Leggi_Listini_CampionamentoConferito_Dettagli(
                ByVal piva As String,
                ByVal KeyListinoProdotto As String,
                ByVal validoDal As String,
                ByVal validoAl As String,
                ByRef objParametri As AgronicaCoreParametri
                ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim keys As String() = KeyListinoProdotto.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Id_TestataGriglia_Prod As Integer = Integer.Parse(keys(2))

        Dim id_testata_griglia As Integer = 0

        Dim validoDalDateTime As Date
        If Not String.IsNullOrEmpty(validoDal) Then
            validoDalDateTime = Convert.ToDateTime(validoDal)
        End If

        Dim validoAlDateTime As Date
        If Not String.IsNullOrEmpty(validoAl) Then
            validoAlDateTime = Convert.ToDateTime(validoAl)
        End If

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Listini_CampionamentoConferito_Dettagli()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElemProd =
               (From griglia_testata In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                Where
                  griglia_testata.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                  griglia_testata.PIVA.Equals(piva) AndAlso
                  griglia_testata.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod
            ).FirstOrDefault()

            If TestataElemProd IsNot Nothing Then
                id_testata_griglia = TestataElemProd.Id_TestataGriglia
            End If

            If TestataElemProd IsNot Nothing AndAlso Not String.IsNullOrEmpty(validoDal) AndAlso Not String.IsNullOrEmpty(validoAl) Then

                'Modifica
                Dim Calibri =
                   From campconfer_calibri In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
                   Group Join listini_campconf_dettagli In
                       GiasContext.Listini_CampionamentoConferito_Dettagli.Where(Function(x) x.Listino_Cod = key_Listino_Cod AndAlso x.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod AndAlso x.Validita_Inizio = validoDalDateTime AndAlso x.Validita_Fine = validoAlDateTime)
                       On
                        campconfer_calibri.Piva_SuperUser Equals listini_campconf_dettagli.Piva_SuperUser And
                        campconfer_calibri.PIVA Equals listini_campconf_dettagli.PIVA And
                        campconfer_calibri.Id_Calibro Equals listini_campconf_dettagli.Id_Calibro
                   Into _listini_campconf_dettagli = Group
                   From _lcd In _listini_campconf_dettagli.DefaultIfEmpty()
                   Group Join listini_campconf_prodotti In
                       GiasContext.Listini_CampionamentoConferito_Prodotti.Where(Function(y) y.Listino_Cod = key_Listino_Cod AndAlso y.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod AndAlso y.Validita_Inizio = validoDalDateTime AndAlso y.Validita_Fine = validoAlDateTime)
                       On
                        _lcd.Piva_SuperUser Equals listini_campconf_prodotti.Piva_SuperUser And
                        _lcd.PIVA Equals listini_campconf_prodotti.PIVA And
                        _lcd.Listino_Cod Equals listini_campconf_prodotti.Listino_Cod And
                        _lcd.Id_TestataGriglia_Prod Equals listini_campconf_prodotti.Id_TestataGriglia_Prod
                   Into _listini_campconf_prodotti = Group
                   From _lcp In _listini_campconf_prodotti.DefaultIfEmpty()
                   Where campconfer_calibri.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                            campconfer_calibri.PIVA.Equals(piva) AndAlso
                            campconfer_calibri.Id_TestataGriglia = id_testata_griglia
                   Order By campconfer_calibri.Ordinamento
                   Select New With {
                       .Id_Calibro = campconfer_calibri.Id_Calibro,
                       .Descr_calibro = If(campconfer_calibri.Descr_qualita Is Nothing, campconfer_calibri.Descr_calibro, campconfer_calibri.Descr_qualita & " " & campconfer_calibri.Descr_calibro),
                       .Prezzo = If(Not _lcd Is Nothing, _lcd.Prezzo, 0)}

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(Calibri.ToList(), Formatting.None, serializerSettings)

            Else

                'Inserimento
                Dim Calibri =
                   From campconfer_calibri In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
                   Where campconfer_calibri.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                            campconfer_calibri.PIVA.Equals(piva) AndAlso
                            campconfer_calibri.Id_TestataGriglia = id_testata_griglia
                   Order By campconfer_calibri.Ordinamento
                   Select New With {
                       .Id_Calibro = campconfer_calibri.Id_Calibro,
                       .Descr_calibro = If(campconfer_calibri.Descr_qualita Is Nothing, campconfer_calibri.Descr_calibro, campconfer_calibri.Descr_qualita & " " & campconfer_calibri.Descr_calibro),
                       .Prezzo = 0.0}

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(Calibri.ToList(), Formatting.None, serializerSettings)

            End If


        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Listini_CampionamentoConferito_PerScaglioniData(
                ByVal piva As String,
                ByVal KeyListinoProdotto As String,
                ByVal prezzo_su_conferito As Short,
                ByRef objParametri As AgronicaCoreParametri
                ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim keys As String() = KeyListinoProdotto.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Id_TestataGriglia_Prod As Integer = Integer.Parse(keys(2))

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Listini_CampionamentoConferito_PrezzoSuConferito()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Listini_campconf_prodotti_scaglioni_data =
               From listini_campconf_prodotti In GiasContext.Listini_CampionamentoConferito_Prodotti
               Where listini_campconf_prodotti.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                        listini_campconf_prodotti.PIVA.Equals(piva) AndAlso
                        listini_campconf_prodotti.Listino_Cod = key_Listino_Cod AndAlso
                        listini_campconf_prodotti.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod AndAlso
                        listini_campconf_prodotti.prezzo_su_conferito = prezzo_su_conferito
               Order By listini_campconf_prodotti.Validita_Inizio Descending,
                        listini_campconf_prodotti.Validita_Fine Descending
               Select New With {
                   .Key_List_Prodotti = "",
                   .Validita_Inizio = listini_campconf_prodotti.Validita_Inizio,
                   .Validita_Fine = listini_campconf_prodotti.Validita_Fine,
                   .Prezzo = listini_campconf_prodotti.prezzo}

            ' Compongo la chiave
            Dim myList = Listini_campconf_prodotti_scaglioni_data.ToList()
            For Each obj In myList
                obj.Key_List_Prodotti = obj.Validita_Inizio.ToShortDateString().Replace("/", "") & "-" & obj.Validita_Fine.ToShortDateString().Replace("/", "")
            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Listini_CampionamentoConferito_Prodotti_Equivalenti(
                ByVal piva As String,
                ByVal KeyListinoProdotto As String,
                ByRef objParametri As AgronicaCoreParametri
                ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim keys As String() = KeyListinoProdotto.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Id_TestataGriglia_Prod As Integer = Integer.Parse(keys(2))

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Listini_CampionamentoConferito_Prodotti_Equivalenti()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim campConf_R As New FF_CampionamentoConferimento_R
            Dim campConfTestataGriglia_Prodotti = campConf_R.Leggi_Elem_Testata_Griglie_Prodotti(piva, 0, key_Id_TestataGriglia_Prod, objParametri)

            ' Cerco tutti i prodotti che appartengono alla stessa griglia per i quali non esiste già un prezzo
            Dim ProdottiEquivalenti =
               From materie_prime In GiasContext.Materie_Prime
               Join campconfer_prodotti In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti.Where(Function(x) x.Mat_Cod <> campConfTestataGriglia_Prodotti.Mat_Cod OrElse
                   x.Tabella_Par_Cod_Calibro <> campConfTestataGriglia_Prodotti.Tabella_Par_Cod_Calibro OrElse
                   x.Tabella_Par_Cod_Qualita <> campConfTestataGriglia_Prodotti.Tabella_Par_Cod_Qualita)
               On
                    materie_prime.Mat_Cod Equals campconfer_prodotti.Mat_Cod
               Join specievegetali In GiasContext.SpecieVegetali On materie_prime.Veg_Cod Equals specievegetali.Veg_Cod
               Join varieta In GiasContext.Cultivar On materie_prime.Cul_Cod Equals varieta.Cul_Cod And
                                                        materie_prime.Veg_Cod Equals varieta.Veg_Cod
               Group Join listini_prodotti_equivalenti In
                   GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Where(Function(x) x.Listino_Cod = key_Listino_Cod AndAlso x.Id_TestataGriglia_Prod_Equivalente = key_Id_TestataGriglia_Prod)
                   On
                    campconfer_prodotti.Piva_SuperUser Equals listini_prodotti_equivalenti.Piva_SuperUser And
                    campconfer_prodotti.PIVA Equals listini_prodotti_equivalenti.PIVA And
                    campconfer_prodotti.Id_TestataGriglia_Prod Equals listini_prodotti_equivalenti.Id_TestataGriglia_Prod
               Into _listini_prodotti_equivalenti = Group
               From _lpe In _listini_prodotti_equivalenti.DefaultIfEmpty()
               Group Join otab_param_calibro In GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 1 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On otab_param_calibro.Tabella_Par_Cod Equals campconfer_prodotti.Tabella_Par_Cod_Calibro
                    Into otab_param_calibro_group = Group
               From _otp_calibro In otab_param_calibro_group.DefaultIfEmpty()
               Group Join otab_param_qual In GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 3 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On
                     otab_param_qual.Tabella_Par_Cod Equals campconfer_prodotti.Tabella_Par_Cod_Qualita
                    Into otab_param_qual_group = Group
               From _otp_qual In otab_param_qual_group.DefaultIfEmpty()
               Where campconfer_prodotti.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                        campconfer_prodotti.PIVA.Equals(piva) AndAlso
                        campconfer_prodotti.Id_TestataGriglia = campConfTestataGriglia_Prodotti.Id_TestataGriglia AndAlso
                     Not GiasContext.Listini_CampionamentoConferito_Prodotti.Any(Function(y) _
                    y.Piva_SuperUser = campconfer_prodotti.Piva_SuperUser AndAlso
                    y.PIVA = campconfer_prodotti.PIVA AndAlso
                    y.Listino_Cod = key_Listino_Cod AndAlso
                    y.Id_TestataGriglia_Prod = campconfer_prodotti.Id_TestataGriglia_Prod) AndAlso
                     Not GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Any(Function(z) _
                    z.Piva_SuperUser = campconfer_prodotti.Piva_SuperUser AndAlso
                    z.PIVA = campconfer_prodotti.PIVA AndAlso
                    z.Listino_Cod = key_Listino_Cod AndAlso
                    z.Id_TestataGriglia_Prod_Equivalente <> key_Id_TestataGriglia_Prod AndAlso
                    z.Id_TestataGriglia_Prod = campconfer_prodotti.Id_TestataGriglia_Prod)
               Order By materie_prime.Mat_Des
               Select New With {
                    .Selected = If(Not _lpe Is Nothing, True, False),
                    .Id_TestataGriglia_Prod = campconfer_prodotti.Id_TestataGriglia_Prod,
                    .Mat_Des = materie_prime.Mat_Des,
                    .Cod_Articolo = materie_prime.Cod_Articolo,
                    .Veg_Des = specievegetali.Veg_Des,
                    .Cul_Des = varieta.Cul_Des,
                    .Reg_Des = If(materie_prime.Regolamento = enum_Cod_Regolamento.Regolamento_bio, "Biologico (Reg.CE 834/07 (Ex.Reg.CE 2092/91))", "Convenzionale (Reg. Nessuno)"),
                    .Codice_Esterno = materie_prime.Codice_Esterno,
                    .qualita_des = If(_otp_qual Is Nothing, "", _otp_qual.Descrizione),
                    .calibro_des = If(_otp_calibro Is Nothing, "", _otp_calibro.Descrizione)}


            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(ProdottiEquivalenti.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    ' Stefano - 25/1/2017 - Non più necessario perché da ora ci si lega alle materie prime e non alle linee produzione
    'Private Function Linea_Cod_GrigliaCampionamento() As Integer
    '    Throw New NotImplementedException
    'End Function
    ' FINE Stefano - 25/1/2017 - Non più necessario perché da ora ci si lega alle materie prime e non alle linee produzione


    '##############################################################################################
    Public Function Leggi_Elenco_Listini_Prodotti_Equivalenti(ByVal piva As String,
                                                  ByVal key_Listino_Cod As Integer?,
                                                  ByVal key_Id_TestataGriglia_Prod As Integer?,
                                                              ByVal key_Id_TestataGriglia_Prod_Equivalente As Integer?,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As List(Of Listini_CampionamentoConferito_Prodotti_Equivalenti)

        Dim Righe As List(Of Listini_CampionamentoConferito_Prodotti_Equivalenti) = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Elenco_Listini_Prodotti()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Righe = (From prod_equiv In GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti
                     Where
                               prod_equiv.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                               prod_equiv.PIVA.Equals(piva) AndAlso
                               (key_Listino_Cod Is Nothing OrElse prod_equiv.Listino_Cod = key_Listino_Cod) AndAlso
                              (key_Id_TestataGriglia_Prod Is Nothing OrElse prod_equiv.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod) AndAlso
                                (key_Id_TestataGriglia_Prod_Equivalente Is Nothing OrElse prod_equiv.Id_TestataGriglia_Prod_Equivalente = key_Id_TestataGriglia_Prod_Equivalente)
                            ).ToList()

        End Using

        Return Righe

    End Function


    '##############################################################################################
    Public Function Leggi_Elem_Fattore_Variazione_ParamQualitativi(
                ByVal piva As String,
                ByVal IDFattoreVariazione As Integer,
                ByRef objParametri As AgronicaCoreParametri
                ) As CampionamentoConferito_Fattori_Variazione_ParamQualitativi

        Dim Elem As CampionamentoConferito_Fattori_Variazione_ParamQualitativi = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Elem_Fattore_Variazione_ParamQualitativi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Elem =
           (From fatt_var In GiasContext.CampionamentoConferito_Fattori_Variazione_ParamQualitativi
            Where
              (fatt_var.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
              (fatt_var.PIVA.Equals(piva)) AndAlso
              (fatt_var.Id_fattore_variazione = IDFattoreVariazione)
            Select fatt_var).FirstOrDefault()

        End Using

        Return Elem

    End Function

    '##############################################################################################
    Public Function Leggi_Elem_Listino_Fattore_Variazione(
                ByVal piva As String,
                ByVal Id_listino_fattore_variaz As Integer,
                ByRef objParametri As AgronicaCoreParametri
                ) As Listini_CampionamentoConferito_Fattori_Variazione

        Dim Elem As Listini_CampionamentoConferito_Fattori_Variazione = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Elem_Listino_Fattore_Variazione()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Elem =
           (From list_fatt_var In GiasContext.Listini_CampionamentoConferito_Fattori_Variazione
            Where
              (list_fatt_var.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
              (list_fatt_var.PIVA.Equals(piva)) AndAlso
              (list_fatt_var.Id_listino_fattore_variaz = Id_listino_fattore_variaz)
            Select list_fatt_var).FirstOrDefault()

        End Using

        Return Elem

    End Function

    '##############################################################################################
    Public Function LeggiTabelleConsultazione(ByVal piva As String,
                                              ByVal Tabella_ID As Integer,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As String

        Dim risposta As String = ""

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.LeggiValoriParametriQualitativi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ValoriTabellaConsultazione =
           From tabelleParametri In GiasContext.OTabelle_Parametri
           Where
                (tabelleParametri.Piva.Equals(piva) OrElse tabelleParametri.Piva.Equals("AAAAAAAAAAA")) AndAlso
                (tabelleParametri.Modulo_Generazione = 2) AndAlso
                (tabelleParametri.Tabella_Cod = Tabella_ID) AndAlso
                (tabelleParametri.ChkInvisibile = 0)
           Order By
                tabelleParametri.Descrizione
           Select New With {
               .val_cod = tabelleParametri.Tabella_Par_Cod,
               .val_des = tabelleParametri.Descrizione
           }

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(ValoriTabellaConsultazione.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_FattoriVariazioneParametriQualitativi(ByVal piva As String,
                                                                ByRef objParametri As AgronicaCoreParametri
                                                                ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_FattoriVariazioneParametriQualitativi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim FattVarParQual =
           From fattori_variazione_parqual In GiasContext.CampionamentoConferito_Fattori_Variazione_ParamQualitativi
           Join otab In GiasContext.OTabelle
                 On otab.Tabella_Cod Equals fattori_variazione_parqual.Tabella_Cod
           Join tabelleParametri In GiasContext.OTabelle_Parametri
                 On tabelleParametri.Tabella_Cod Equals fattori_variazione_parqual.Tabella_Cod And
                    tabelleParametri.Tabella_Par_Cod Equals fattori_variazione_parqual.Tabella_Par_Cod
           Where
               (fattori_variazione_parqual.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
               (fattori_variazione_parqual.PIVA.Equals(piva))
           Order By otab.Tabella_Des,
                    tabelleParametri.Descrizione
           Select New With {
                    .Id_fattore_variazione = fattori_variazione_parqual.Id_fattore_variazione,
                    .Tabella_ID = fattori_variazione_parqual.Tabella_Cod,
                    .Tabella_Des = otab.Tabella_Des,
                    .val_cod = fattori_variazione_parqual.Tabella_Par_Cod,
                    .val_des = tabelleParametri.Descrizione
              }

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(FattVarParQual.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function LeggiFattoriVariazione(ByVal piva As String,
                                           ByVal Id_fattore_variazione As Integer,
                                           ByRef countRigheEsistenti As Integer,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As String

        Dim risposta As String = ""
        Dim keys As String() = Nothing

        'TODO Stefano: filtro su prodotti attivi per il parametro qualitativo

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_ValoriFattoriVariazione()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            'Non vengono mostrati i prodotti che sono già fra quelli equivalenti
            Dim FattVariazione =
           From fattvar In GiasContext.CampionamentoConferito_Fattori_Variazione_ParamQualitativi
           Join config_dettagli In GiasContext.OModuli_Referenze_Config_Dettagli On
                        fattvar.PIVA Equals config_dettagli.Piva And
                        CStr(fattvar.Tabella_Cod) Equals config_dettagli.Tabella_ID
           Join otab In GiasContext.OTabelle
                 On CStr(otab.Tabella_Cod) Equals config_dettagli.Tabella_ID
           Join tabelleParametri In GiasContext.OTabelle_Parametri On
               fattvar.Tabella_Par_Cod Equals tabelleParametri.Tabella_Par_Cod And
               fattvar.Tabella_Cod Equals tabelleParametri.Tabella_Cod
           Where fattvar.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                 fattvar.PIVA.Equals(piva) AndAlso
               (Id_fattore_variazione = 0 OrElse fattvar.Id_fattore_variazione = Id_fattore_variazione) AndAlso
                  (config_dettagli.Tipo = 1) AndAlso
               (tabelleParametri.Piva.Equals(piva) OrElse
                (tabelleParametri.Piva.Equals("AAAAAAAAAAA") AndAlso config_dettagli.ChkOmni_Invisibili = 0)) AndAlso
                (tabelleParametri.Modulo_Generazione = 2) AndAlso
                (tabelleParametri.ChkInvisibile = 0)
           Order By otab.Tabella_Des, tabelleParametri.Descrizione
           Select New With {
               .Id_fattore_variazione = fattvar.Id_fattore_variazione,
               .Descr_fattore_variazione = otab.Tabella_Des & " " & tabelleParametri.Descrizione
           }

            countRigheEsistenti = FattVariazione.Distinct().Count()

            ' Compongo la chiave
            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(FattVariazione.Distinct().ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_ValoriFattoriVariazione(ByVal piva As String,
                                                  ByVal KeyListinoProdotto As String,
                                                  ByVal Id_fattore_variazione As Integer,
                                                  ByRef countRigheEsistenti As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As String

        Dim keys As String() = Nothing
        Dim key_piva As String = ""
        Dim key_Listino_Cod As Integer = 0
        Dim key_Id_TestataGriglia_Prod As Integer = 0

        If Not String.IsNullOrEmpty(KeyListinoProdotto) Then
            keys = KeyListinoProdotto.Split("-")
            key_piva = keys(0)
            key_Listino_Cod = Integer.Parse(keys(1))
            key_Id_TestataGriglia_Prod = Integer.Parse(keys(2))
        End If

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_ValoriFattoriVariazione()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            'Non vengono mostrati i prodotti che sono già fra quelli equivalenti
            Dim ValoriFattVar =
           From val_fatt_var In GiasContext.Listini_CampionamentoConferito_Fattori_Variazione
           Join fattvar In GiasContext.CampionamentoConferito_Fattori_Variazione_ParamQualitativi On
                        fattvar.Piva_SuperUser Equals val_fatt_var.Piva_SuperUser And
                        fattvar.PIVA Equals val_fatt_var.PIVA And
                        fattvar.Id_fattore_variazione Equals val_fatt_var.Id_fattore_variazione
           Join config_dettagli In GiasContext.OModuli_Referenze_Config_Dettagli On
                        fattvar.PIVA Equals config_dettagli.Piva And
                        CStr(fattvar.Tabella_Cod) Equals config_dettagli.Tabella_ID
           Join otab In GiasContext.OTabelle
                 On CStr(otab.Tabella_Cod) Equals config_dettagli.Tabella_ID
           Join tabelleParametri In GiasContext.OTabelle_Parametri On
               fattvar.Tabella_Par_Cod Equals tabelleParametri.Tabella_Par_Cod And
               fattvar.Tabella_Cod Equals tabelleParametri.Tabella_Cod
           Where val_fatt_var.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                 val_fatt_var.PIVA.Equals(piva) AndAlso
               (KeyListinoProdotto = "" OrElse
               (val_fatt_var.Listino_Cod = key_Listino_Cod AndAlso
                val_fatt_var.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod)) AndAlso
               (Id_fattore_variazione = 0 OrElse val_fatt_var.Id_fattore_variazione = Id_fattore_variazione) AndAlso
                  (config_dettagli.Tipo = 1) AndAlso
               (tabelleParametri.Piva.Equals(piva) OrElse
                (tabelleParametri.Piva.Equals("AAAAAAAAAAA") AndAlso config_dettagli.ChkOmni_Invisibili = 0)) AndAlso
                (tabelleParametri.Modulo_Generazione = 2) AndAlso
                (tabelleParametri.ChkInvisibile = 0)
           Order By val_fatt_var.Validita_Inizio, val_fatt_var.Validita_Fine
           Select New With {
               .Id_listino_fattore_variaz = val_fatt_var.Id_listino_fattore_variaz,
               .Id_fattore_variazione = val_fatt_var.Id_fattore_variazione,
               .Descr_fattore_variazione = otab.Tabella_Des & " " & tabelleParametri.Descrizione,
               .Validita_Inizio = val_fatt_var.Validita_Inizio,
               .Validita_Fine = val_fatt_var.Validita_Fine,
               .variazione_a_valore = val_fatt_var.variazione_a_valore,
               .valore_al_kg = val_fatt_var.valore_al_kg
           }

            countRigheEsistenti = ValoriFattVar.Distinct().Count()

            ' Compongo la chiave
            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(ValoriFattVar.Distinct().ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_EsclusioneFattoreVariazione(ByVal piva As String,
                                                      ByVal KeyListinoProdotto As String,
                                                      ByVal Id_listino_fattore_variaz As Integer,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As String

        Dim risposta As String = ""

        Dim keys As String() = KeyListinoProdotto.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Id_TestataGriglia_Prod As Integer = Integer.Parse(keys(2))

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_EsclusioneFattoreVariazione()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim prezzoSuConferito As Boolean = False

            ' Cerco se il prezzo è sul conferito o sul campionato
            Dim Listini_CampConf_ProdottiElem =
                (From list_camp_conf In GiasContext.Listini_CampionamentoConferito_Prodotti
                 Where
                        list_camp_conf.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                        list_camp_conf.PIVA.Equals(piva) AndAlso
                        list_camp_conf.Listino_Cod = key_Listino_Cod AndAlso
                        list_camp_conf.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod
                 Select New With
                         {
                            .prezzo_su_conferito = list_camp_conf.prezzo_su_conferito,
                            .Id_TestataGriglia = list_camp_conf.CampionamentoConferito_TestataGriglia_Prodotti.Id_TestataGriglia
                        }).FirstOrDefault()

            If Listini_CampConf_ProdottiElem Is Nothing Then
                Throw New Exception("Listino con Listino_Cod " & key_Listino_Cod.ToString() &
                                    " e  Id_TestataGriglia_Prod " & key_Id_TestataGriglia_Prod.ToString() &
                                    " non trovato")
            Else
                prezzoSuConferito = Listini_CampConf_ProdottiElem.prezzo_su_conferito
            End If

            Dim EsclusioneFattoriVariazione As New List(Of Object)

            If prezzoSuConferito Then

                ' NON DEVE MAI ARRIVARE QUI
                '  CMQ QUELLA SOTTO E' SBAGLIATA PERCHE' DEVE CASO MAI FARE UNA GROUP BY SUI PRODOTTI
                '  LA LASCIO PERCHE' SE IN UN DOMANI CHIEDONO DI NON ABILITARE IL FATTORE DI VARIAZIONE SU ALCUNI PRODOTTI EQUIVALENTI
                '  SARA' DA IMPLEMENTARE IN MODO SIMILE (E ANCHE SOTTO PER PREZZO SU CALIBRO)
                'Dim EsclusioneFattoriVar =
                '    From listini_prodotti In GiasContext.Listini_CampionamentoConferito_Prodotti
                '    Join matprime In GiasContext.Materie_Prime On
                '        matprime.Mat_Cod Equals listini_prodotti.Mat_Cod And
                '        matprime.Piva Equals listini_prodotti.PIVA
                '    Group Join esclusione_fattvar In GiasContext.Listini_CampionamentoConferito_Esclusione_Fattore_Variazione.Where(Function(x) x.Id_listino_fattore_variaz = Id_listino_fattore_variaz And x.Id_Calibro = 0)
                '        On
                '            esclusione_fattvar.Piva_SuperUser Equals listini_prodotti.Piva_SuperUser And
                '            esclusione_fattvar.PIVA Equals listini_prodotti.PIVA
                '    Into _esclusione_fattvar = Group
                '    From _ef In _esclusione_fattvar.DefaultIfEmpty()
                '    Where listini_prodotti.Piva_SuperUser.Equals(Piva_SuperUser) And
                '        listini_prodotti.PIVA.Equals(piva) And
                '        listini_prodotti.Id_TestataGriglia = key_Id_TestataGriglia And
                '        listini_prodotti.Mat_Cod = key_Mat_Cod
                '    Order By matprime.Mat_Des
                '    Select New With {
                '        .Selected = If(Not _ef Is Nothing, True, False),
                '        .Id_Calibro = -1,
                '        .Descrizione = matprime.Mat_Des}

                'Dim myList = EsclusioneFattoriVar.ToList()
                'For Each obj In myList
                '    EsclusioneFattoriVariazione.Add(obj)
                'Next

            Else

                Dim key_Id_TestataGriglia As Integer = Listini_CampConf_ProdottiElem.Id_TestataGriglia
                Dim EsclusioneFattoriVar =
                    From calibri In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
                    Group Join esclusione_fattvar In
                        GiasContext.Listini_CampionamentoConferito_Esclusione_Fattore_Variazione.Where(Function(x) x.Id_listino_fattore_variaz = Id_listino_fattore_variaz)
                        On
                            esclusione_fattvar.Piva_SuperUser Equals calibri.Piva_SuperUser And
                            esclusione_fattvar.PIVA Equals calibri.PIVA And
                            esclusione_fattvar.Id_Calibro Equals calibri.Id_Calibro
                    Into _esclusione_fattvar = Group
                    From _ef In _esclusione_fattvar.DefaultIfEmpty()
                    Where calibri.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                        calibri.PIVA.Equals(piva) AndAlso
                        calibri.Id_TestataGriglia = key_Id_TestataGriglia
                    Order By calibri.Ordinamento
                    Select New With {
                        .Selected = If(Not _ef Is Nothing, True, False),
                        .Id_Calibro = calibri.Id_Calibro,
                        .Descrizione = If(calibri.Descr_qualita Is Nothing, calibri.Descr_calibro, calibri.Descr_qualita & " " & calibri.Descr_calibro)}

                Dim myList = EsclusioneFattoriVar.ToList()
                EsclusioneFattoriVariazione.AddRange(myList)

            End If

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(EsclusioneFattoriVariazione, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Controlla_Date_Listini_FattoriVariazione(
                ByVal listFattVarElem As Listini_CampionamentoConferito_Fattori_Variazione,
                ByRef objParametri As AgronicaCoreParametri
                ) As Integer

        Dim countTrovati As Integer = 0

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Listini_FattoriVariazione()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ListiniFattVar =
                From val_fatt_var In GiasContext.Listini_CampionamentoConferito_Fattori_Variazione
                Where
                    val_fatt_var.Id_listino_fattore_variaz <> listFattVarElem.Id_listino_fattore_variaz AndAlso
                    val_fatt_var.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                    val_fatt_var.PIVA.Equals(listFattVarElem.PIVA) AndAlso
                    val_fatt_var.Id_fattore_variazione = listFattVarElem.Id_fattore_variazione AndAlso
                    val_fatt_var.Listino_Cod = listFattVarElem.Listino_Cod AndAlso
                    val_fatt_var.Id_TestataGriglia_Prod = listFattVarElem.Id_TestataGriglia_Prod AndAlso
                    ((listFattVarElem.Validita_Inizio <= val_fatt_var.Validita_Inizio AndAlso
                    listFattVarElem.Validita_Fine >= val_fatt_var.Validita_Inizio) OrElse
                    (listFattVarElem.Validita_Inizio <= val_fatt_var.Validita_Fine AndAlso
                    listFattVarElem.Validita_Fine >= val_fatt_var.Validita_Fine) OrElse
                    (listFattVarElem.Validita_Inizio >= val_fatt_var.Validita_Inizio AndAlso
                     listFattVarElem.Validita_Fine <= val_fatt_var.Validita_Fine))
                Select val_fatt_var

            countTrovati = ListiniFattVar.Count()
        End Using

        Return countTrovati

    End Function

    '##############################################################################################
    '  Questa contiene tutto il calcolo di acconti / liquidazioni
    '##############################################################################################
    Public Function Estrai_Righe_Conferimento_Valorizzate(
                ByVal piva As String, ByVal centri_aziendali As Integer(),
                ByVal _docNumeroSin As String, ByVal _docNumero As Integer,
                ByVal _docNumeroDes As String, ByVal _nrRiga As Integer,
                ByVal _dataMovDal As String, ByVal _dataMovAl As String,
                ByVal _specie As Integer(), ByVal _varieta As Integer(),
                ByVal _operazioni As Integer(),
                ByVal _fornitori As String(),
                ByVal gruppoFatturazione As Integer(),
                ByVal listinoBase As Integer,
                ByVal IsAccontoLiquidazione As Boolean,
                ByVal StatoCampionamentoIniziale As String,
                ByVal TipoCampionamentoIniziale As Short,
                ByVal _filtroSuProdotti As Integer,
                ByVal _prodotti As Integer(),
                ByRef objParametri As AgronicaCoreParametri,
                ByRef DtValorizzazione As DataTable,
                ByRef trovatoErrore As Boolean
                ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim _filtroSuSpecie As Boolean = False
        If _specie IsNot Nothing AndAlso _specie.Length > 0 Then
            _filtroSuSpecie = True
        End If

        Dim _filtroSuVarieta As Boolean = False
        If _varieta IsNot Nothing AndAlso _varieta.Length > 0 Then
            _filtroSuVarieta = True
        End If

        Dim _filtroSuFornitori As Boolean = False
        If _fornitori IsNot Nothing AndAlso _fornitori.Length > 0 Then
            _filtroSuFornitori = True
        End If

        Dim _filtroSuOperazioni As Boolean = False
        If _operazioni IsNot Nothing AndAlso _operazioni.Length > 0 Then
            _filtroSuOperazioni = True
        End If

        Dim _filtroSuGruppoFatturazione As Boolean = False
        If gruppoFatturazione IsNot Nothing AndAlso gruppoFatturazione.Length > 0 Then
            _filtroSuGruppoFatturazione = True
        End If

        Dim _filtroSuCentriAziendali As Boolean = False
        If centri_aziendali IsNot Nothing AndAlso centri_aziendali.Length > 0 Then
            _filtroSuCentriAziendali = True
        End If

        Dim dataMovDalDateTime As Nullable(Of DateTime)
        dataMovDalDateTime = Nothing
        If Not String.IsNullOrEmpty(_dataMovDal) Then
            dataMovDalDateTime = Convert.ToDateTime(_dataMovDal)
        End If

        Dim dataMovAlDateTime As Nullable(Of DateTime)
        dataMovAlDateTime = Nothing
        If Not String.IsNullOrEmpty(_dataMovAl) Then
            dataMovAlDateTime = Convert.ToDateTime(_dataMovAl)
        End If

        '_filtroSuProdotti = 0 non è applicato nessun filtro
        '_filtroSuProdotti = 1 si filtrano solo i Prodotti inclusi in _prodotti
        '_filtroSuProdotti = 2 si filtrano solo i Prodotti non inclusi in _prodotti

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Estrai_Righe_Conferimento_Valorizzate()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)



        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim movimenti_dettagli As DbSet(Of Movimenti_dettagli) = GiasContext.Movimenti_dettagli
            Dim movimenti_7300 As DbSet(Of Movimenti) = GiasContext.Movimenti
            Dim movimenti_4000 As DbSet(Of Movimenti) = GiasContext.Movimenti
            Dim movimenti_4050 As DbSet(Of Movimenti) = GiasContext.Movimenti
            Dim agenda As DbSet(Of Agenda) = GiasContext.Agenda
            Dim contatti As DbSet(Of Contatti) = GiasContext.Contatti
            Dim risorse_umane As DbSet(Of Risorse_Umane) = GiasContext.Risorse_Umane
            Dim risorse_umane_produttore As DbSet(Of Risorse_Umane) = GiasContext.Risorse_Umane
            Dim matPrima As DbSet(Of Materie_Prime) = GiasContext.Materie_Prime
            Dim fattoreVariazione_paramqual As DbSet(Of CampionamentoConferito_Fattori_Variazione_ParamQualitativi) = GiasContext.CampionamentoConferito_Fattori_Variazione_ParamQualitativi
            Dim matprimadett As DbSet(Of Materie_Prime_Dettagli) = GiasContext.Materie_Prime_Dettagli
            Dim lavCodConf As Integer?() = {LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE}
            Dim tabelleParametri As DbSet(Of OTabelle_Parametri) = GiasContext.OTabelle_Parametri
            Dim matPrimeCampionature As DbSet(Of Materie_Prime_Campionature) = GiasContext.Materie_Prime_Campionature

            ' Cerco tutti i fattori di variazione attivi
            ' TODO Stefano - togliere se non serve
            'Dim FattoreVar_ParamQual =
            '    From fattori_variazione_parqual In fattoreVariazione_paramqual
            '    Join otab In GiasContext.OTabelles
            '     On otab.Tabella_Cod Equals fattori_variazione_parqual.Tabella_Cod
            '    Join tabelleParametri In GiasContext.OTabelle_Parametri
            '     On tabelleParametri.Tabella_Cod Equals fattori_variazione_parqual.Tabella_Cod And
            '        tabelleParametri.Tabella_Par_Cod Equals fattori_variazione_parqual.Tabella_Par_Cod
            '    Where
            '   (fattori_variazione_parqual.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
            '   (fattori_variazione_parqual.PIVA.Equals(piva))
            '    Select New With {
            '        .Id_fattore_variazione = fattori_variazione_parqual.Id_fattore_variazione,
            '        .Tabella_ID = fattori_variazione_parqual.Tabella_Cod,
            '        .Tabella_Des = otab.Tabella_Des,
            '        .val_cod = fattori_variazione_parqual.Tabella_Par_Cod,
            '        .val_des = tabelleParametri.Descrizione
            '  }

            ' Cerco tutti i fattori di variazione attivi
            Dim FattoreVar_ParamQual_Grouped = (
                From fv In fattoreVariazione_paramqual
                Join otab In GiasContext.OTabelle On
                    otab.Tabella_Cod Equals fv.Tabella_Cod
                Where fv.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                      fv.PIVA.Equals(piva)
                Group By tabcod = fv.Tabella_Cod, tabdes = otab.Tabella_Des, tabcoddes = otab.Tabella_Cod_Des
                Into g = Group
                Select New With {
                    .Tabella_Cod = tabcod,
                    .Tabella_Des = tabdes,
                    .Tabella_Cod_Des = tabcoddes
                    }).ToList()
            Dim listFattoreVar_ParamQual_Grouped As New List(Of Object)
            For Each o In FattoreVar_ParamQual_Grouped
                listFattoreVar_ParamQual_Grouped.Add(o)
            Next

            'Cerco tutti i contatti che hanno listini personalizzati direttamente su anagrafica contatti
            Dim Listini_Principali_Per_Fornitore =
            (From lst In GiasContext.Contatti_Codici
             Where lst.Id_cod = 4001 AndAlso lst.Val_cod <> "" AndAlso
                 lst.Val_cod <> "0" AndAlso
                 lst.PIVA = piva
             Select New With {
                    .Cod_Contatto = lst.Cod_Contatto,
                    .Listino = lst.Val_cod
                 }
             ).ToList()

            'Cerco eventuali contatti che hanno listini aggiuntivi per solo fornitore
            Dim Listini_Aggiuntivi_Per_Fornitore =
            (From lst In GiasContext.Listini_PrezzixContatti
             Where lst.Piva_SuperUser = Piva_SuperUser AndAlso lst.Piva = piva AndAlso
                 lst.Cod_Contatto <> "" AndAlso
                 lst.Sa_Cod = 0 AndAlso
                 lst.Cod_Rapporto = 0 AndAlso
                 lst.Cod_Contatto_Produttore = ""
             Select New With {
                    .Cod_Contatto = lst.Cod_Contatto,
                    .Listino = lst.Listino_Cod
                 }
             ).ToList()

            'Cerco eventuali contatti che hanno listini aggiuntivi per fornitore / produttore / centro
            Dim Listini_Aggiuntivi_Per_Fornitore_Produttore_Centro =
            (From lst In GiasContext.Listini_PrezzixContatti
             Where lst.Piva_SuperUser = Piva_SuperUser AndAlso lst.Piva = piva AndAlso
                 lst.Cod_Contatto <> "" AndAlso
                 lst.Sa_Cod <> 0 AndAlso
                 lst.Cod_Rapporto = 0 AndAlso
                 lst.Cod_Contatto_Produttore <> ""
             Select New With {
                    .Cod_Contatto = lst.Cod_Contatto,
                    .Sa_Cod_Listino = lst.Sa_Cod,
                    .Cod_Contatto_Produttore = lst.Cod_Contatto_Produttore,
                    .Listino = lst.Listino_Cod
                 }
             ).ToList()

            'Cerco eventuali contatti che hanno listini aggiuntivi per fornitore / produttore
            Dim Listini_Aggiuntivi_Per_Fornitore_Produttore =
            (From lst In GiasContext.Listini_PrezzixContatti
             Where lst.Piva_SuperUser = Piva_SuperUser AndAlso lst.Piva = piva AndAlso
                 lst.Cod_Contatto <> "" AndAlso
                 lst.Sa_Cod = 0 AndAlso
                 lst.Cod_Rapporto = 0 AndAlso
                 lst.Cod_Contatto_Produttore <> ""
             Select New With {
                    .Cod_Contatto = lst.Cod_Contatto,
                    .Cod_Contatto_Produttore = lst.Cod_Contatto_Produttore,
                    .Listino = lst.Listino_Cod
                 }
             ).ToList()

            'Cerco eventuali contatti che hanno listini aggiuntivi per fornitore / centro aziendale
            Dim Listini_Aggiuntivi_Per_Fornitore_Centro =
            (From lst In GiasContext.Listini_PrezzixContatti
             Where lst.Piva_SuperUser = Piva_SuperUser AndAlso lst.Piva = piva AndAlso
                 lst.Cod_Contatto <> "" AndAlso
                 lst.Sa_Cod <> 0 AndAlso
                 lst.Cod_Rapporto = 0 AndAlso
                 lst.Cod_Contatto_Produttore = ""
             Select New With {
                    .Cod_Contatto = lst.Cod_Contatto,
                    .Sa_Cod_Listino = lst.Sa_Cod,
                    .Listino = lst.Listino_Cod
                 }
             ).ToList()

            'Cerco eventuali contatti che hanno listini per rapporto contabile / centro aziendale
            Dim Listini_Aggiuntivi_Per_RapportoContabile_Centro =
            (From lst In GiasContext.Listini_PrezzixContatti
             Where lst.Piva_SuperUser = Piva_SuperUser AndAlso lst.Piva = piva AndAlso
                 lst.Cod_Contatto = "" AndAlso
                 lst.Sa_Cod <> 0 AndAlso
                 lst.Cod_Rapporto <> 0 AndAlso
                 lst.Cod_Contatto_Produttore = ""
             Select New With {
                    .Cod_Rapporto = lst.Cod_Rapporto,
                    .Sa_Cod_Listino = lst.Sa_Cod,
                    .Listino = lst.Listino_Cod
                 }
             ).ToList()

            'Cerco eventuali contatti che hanno listini aggiuntivi per solo centro aziendale
            Dim Listini_Aggiuntivi_Per_Centro =
            (From lst In GiasContext.Listini_PrezzixContatti
             Where lst.Piva_SuperUser = Piva_SuperUser AndAlso lst.Piva = piva AndAlso
                 lst.Cod_Contatto = "" AndAlso
                 lst.Sa_Cod <> 0 AndAlso
                 lst.Cod_Rapporto = 0 AndAlso
                 lst.Cod_Contatto_Produttore = ""
             Select New With {
                    .Sa_Cod_Listino = lst.Sa_Cod,
                    .Listino = lst.Listino_Cod
                 }
             ).ToList()

            'Cerco eventuali contatti che hanno listini per solo rapporto contabile
            Dim Listini_Aggiuntivi_Per_Rapporto_Contabile =
            (From lst In GiasContext.Listini_PrezzixContatti
             Where lst.Piva_SuperUser = Piva_SuperUser AndAlso lst.Piva = piva AndAlso
                 lst.Cod_Contatto = "" AndAlso
                 lst.Sa_Cod = 0 AndAlso
                 lst.Cod_Rapporto <> 0 AndAlso
                 lst.Cod_Contatto_Produttore = ""
             Select New With {
                    .Cod_Rapporto = lst.Cod_Rapporto,
                    .Listino = lst.Listino_Cod
                 }
             ).ToList()

            Dim ut As New Gias_EF_Utility
            Dim dtElencoListiniPrincipaliFornitore = ut.ObjectQueryToDataTable(Listini_Principali_Per_Fornitore)
            Dim dtElencoListiniAggiuntiviFornitore = ut.ObjectQueryToDataTable(Listini_Aggiuntivi_Per_Fornitore)

            'Listini per fornitore / produttore / centro
            Dim dtElencoListiniAggiuntiviFornitoreProduttoreCentro = ut.ObjectQueryToDataTable(Listini_Aggiuntivi_Per_Fornitore_Produttore_Centro)

            Dim _elenco_listini_FornitoreProduttoreCentro As Integer()
            _elenco_listini_FornitoreProduttoreCentro = (From l In dtElencoListiniAggiuntiviFornitoreProduttoreCentro.AsEnumerable()
                                                         Select CInt(l("Listino"))).ToArray()
            Dim _esistonoListiniFornitoreProduttoreCentro = _elenco_listini_FornitoreProduttoreCentro.Length > 0

            'Listini per fornitore / produttore
            Dim dtElencoListiniAggiuntiviFornitoreProduttore = ut.ObjectQueryToDataTable(Listini_Aggiuntivi_Per_Fornitore_Produttore)

            Dim _elenco_listini_FornitoreProduttore As Integer()
            _elenco_listini_FornitoreProduttore = (From l In dtElencoListiniAggiuntiviFornitoreProduttore.AsEnumerable()
                                                   Select CInt(l("Listino"))).ToArray()
            Dim _esistonoListiniFornitoreProduttore = _elenco_listini_FornitoreProduttore.Length > 0

            'Listini per fornitore / centro
            Dim dtElencoListiniAggiuntiviFornitoreCentro = ut.ObjectQueryToDataTable(Listini_Aggiuntivi_Per_Fornitore_Centro)

            Dim _elenco_listini_FornitoreCentro As Integer()
            _elenco_listini_FornitoreCentro = (From l In dtElencoListiniAggiuntiviFornitoreCentro.AsEnumerable()
                                               Select CInt(l("Listino"))).ToArray()
            Dim _esistonoListiniFornitoreCentro = _elenco_listini_FornitoreCentro.Length > 0

            'Merge di quanto trovato su anag contatti e Listini_PrezzixContatti per solo fornitore perchè potrebbero esistere gli stessi records
            Dim _listiniTrovati As Integer()
            For Each r In dtElencoListiniAggiuntiviFornitore.Rows
                _listiniTrovati = (From l In dtElencoListiniPrincipaliFornitore.AsEnumerable()
                                   Where l("Cod_Contatto") = r("Cod_Contatto") And l("Listino") = r("Listino")
                                   Select CInt(l("Listino"))).ToArray()
                If _listiniTrovati.Length = 0 Then
                    Dim newrow = dtElencoListiniPrincipaliFornitore.NewRow()
                    newrow("Cod_Contatto") = r("Cod_Contatto")
                    newrow("Listino") = r("Listino")
                    dtElencoListiniPrincipaliFornitore.Rows.Add(newrow)
                End If
            Next

            Dim _elenco_listini_Fornitore As Integer()
            _elenco_listini_Fornitore = (From l In dtElencoListiniPrincipaliFornitore.AsEnumerable()
                                         Select CInt(l("Listino"))).ToArray()
            Dim _esistonoListiniFornitore = _elenco_listini_Fornitore.Length > 0

            'Listini per cod rapporto contabile / centro aziendale
            Dim dtElencoListiniRapportoContabileCentro = ut.ObjectQueryToDataTable(Listini_Aggiuntivi_Per_RapportoContabile_Centro)

            Dim _elenco_listini_RapportoContabileCentro As Integer()
            _elenco_listini_RapportoContabileCentro = (From l In dtElencoListiniRapportoContabileCentro.AsEnumerable()
                                                       Select CInt(l("Listino"))).ToArray()
            Dim _esistonoListiniRapportoContabileCentro = _elenco_listini_RapportoContabileCentro.Length > 0

            'Listini per centro
            Dim dtElencoListiniAggiuntiviCentro = ut.ObjectQueryToDataTable(Listini_Aggiuntivi_Per_Centro)

            Dim _elenco_listini_Centro As Integer()
            _elenco_listini_Centro = (From l In dtElencoListiniAggiuntiviCentro.AsEnumerable()
                                      Select CInt(l("Listino"))).ToArray()
            Dim _esistonoListiniCentro = _elenco_listini_Centro.Length > 0

            'Listini per cod rapporto contabile
            Dim dtElencoListiniRapportoContabile = ut.ObjectQueryToDataTable(Listini_Aggiuntivi_Per_Rapporto_Contabile)

            Dim _elenco_listini_RapportoContabile As Integer()
            _elenco_listini_RapportoContabile = (From l In dtElencoListiniRapportoContabile.AsEnumerable()
                                                 Select CInt(l("Listino"))).ToArray()
            Dim _esistonoListiniRapportoContabile = _elenco_listini_RapportoContabile.Length > 0


            'Carico la config che dice quale data usare per cercare i prezzi
            ' Al momento le carico tutte in entrambi i casi, ne lascio due distinte perchè in futuro metteremo una tipologia setup (degrado/rif.pezzi/formula) e potremo filtrare solo per quella
            Dim objParamEntrataXSpecieVarieta As New AgronicaCoreAnagrafeDAL.ParamEntrataXSpecieVarieta_R
            Dim dt_ParamEntrataXSpecieVarietaXDataRif As DataTable = objParamEntrataXSpecieVarieta.Leggi(piva,
                                            0,
                                            Nothing, Nothing,
                                            Nothing,
                                            "", AGRODATAINIZIO,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                            "", "", objParametri)

            Dim dt_ParamEntrataXSpecieVarietaXFormulaFissa As DataTable = objParamEntrataXSpecieVarieta.Leggi(piva,
                                            0,
                                            Nothing, Nothing,
                                            Nothing,
                                            "", AGRODATAINIZIO,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                            "", "", objParametri)

            DtValorizzazione = New DataTable
            DtValorizzazione.Columns.Add(New DataColumn("KeyRigaConferimentoECalibro", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Cal_Cod", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Data_Movimento", GetType(Date)))
            DtValorizzazione.Columns.Add(New DataColumn("Data_Riferimento_Prezzi", GetType(Date)))
            DtValorizzazione.Columns.Add(New DataColumn("Tipo_Data_Riferimento_Prezzi", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Cod_RisUm", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("ConferimentoOAcquisto", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("TipoPrezzo", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Doc_Numero_Completo", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Doc_Numero_Sin", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Doc_Numero", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Doc_Numero_Des", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("DDT_Completo", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Doc_Numero_Visualizzato", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("NrRiga", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Cul_Cod", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Mat_Cod", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Mat_Des", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Grp_Fatt_Cod", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Grp_Fatt_Descr", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Grp_Fatt_Sigla", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Qual_Cod", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Qual_Descr", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Qual_Sigla", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Certif_Cod", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Certif_Descr", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Certif_Sigla", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Calibro_Entrata_Cod", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Calibro_Entrata_Descr", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Calibro_Entrata_Sigla", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Rugginosita_Cod", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Rugginosita_Descr", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Rugginosita_Sigla", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("DegradoPerc", GetType(Decimal)))
            DtValorizzazione.Columns.Add(New DataColumn("Degrado", GetType(Decimal)))
            ' Quantità totale
            DtValorizzazione.Columns.Add(New DataColumn("Qta_Extra_Totale", GetType(Decimal)))
            ' Tara totale 
            DtValorizzazione.Columns.Add(New DataColumn("Tara", GetType(Decimal)))
            DtValorizzazione.Columns.Add(New DataColumn("Id_Calibro", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Descr_Qualita_Camp", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Descr_Calibro_Camp", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Descr_QualCalibro_Camp", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Descr_OrdinQualCalibro_Camp", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Ordinamento_Calibro", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Id_TestataGriglia", GetType(Integer)))
            DtValorizzazione.Columns.Add(New DataColumn("Udm_QtaCampionata", GetType(Integer)))
            ' Quantità campionata; in caso di riga calibro viene ulteriormente rapportata alla % del calibro
            DtValorizzazione.Columns.Add(New DataColumn("QtaCampionata", GetType(Decimal)))
            ' Tara totale rapportata alla quantità campionata; in caso di riga calibro viene ulteriormente rapportata alla % del calibro
            DtValorizzazione.Columns.Add(New DataColumn("TaraCampionata", GetType(Decimal)))
            DtValorizzazione.Columns.Add(New DataColumn("Note", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Automatico", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("StatoCampionamento", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Percentuale_Campionato", GetType(Decimal)))
            ' Quantità totale divisa per calibro; in caso di riga con prezzo su conferito (non viene fatta campionatura) è uguale alla quantità totale 
            DtValorizzazione.Columns.Add(New DataColumn("KgPerCalibro", GetType(Decimal)))
            ' Tara totale divisa per calibro; in caso di riga con prezzo su conferito (non viene fatta campionatura) è uguale alla tara totale 
            DtValorizzazione.Columns.Add(New DataColumn("TaraKgPerCalibro", GetType(Decimal)))
            DtValorizzazione.Columns.Add(New DataColumn("Prezzo", GetType(Decimal)))
            DtValorizzazione.Columns.Add(New DataColumn("PrezzoFattoriVariazione", GetType(Decimal)))
            For Each param In FattoreVar_ParamQual_Grouped
                DtValorizzazione.Columns.Add(New DataColumn(param.Tabella_Cod_Des & "_Codice", GetType(Integer)))
                DtValorizzazione.Columns.Add(New DataColumn(param.Tabella_Cod_Des & "_Descrizione", GetType(String)))
                DtValorizzazione.Columns.Add(New DataColumn(param.Tabella_Cod_Des & "_Sigla", GetType(String)))
                DtValorizzazione.Columns.Add(New DataColumn(param.Tabella_Cod_Des & "_VariazionePrezzo", GetType(Decimal)))
            Next
            DtValorizzazione.Columns.Add(New DataColumn("TotalePrezzo", GetType(Decimal)))
            DtValorizzazione.Columns.Add(New DataColumn("Importo", GetType(Decimal)))
            DtValorizzazione.Columns.Add(New DataColumn("TrasportoACura", GetType(Decimal)))
            DtValorizzazione.Columns.Add(New DataColumn("Messaggi", GetType(String)))
            DtValorizzazione.Columns.Add(New DataColumn("Prezzo_da_riga_conferimento", GetType(Integer)))

            Dim dr As DataRow

            Dim RigheConferimento =
                From ag In agenda
                Join mov In movimenti_7300
                    On
                     ag.PIVA Equals mov.PIVA And
                     ag.Id_Agenda Equals mov.Id_Agenda
                Join mov_4000 In movimenti_4000
                    On
                     ag.PIVA Equals mov_4000.PIVA And
                     ag.Id_Agenda Equals mov_4000.Id_Agenda
                Join mov_4050 In movimenti_4050
                    On
                     ag.PIVA Equals mov_4050.PIVA And
                     ag.Id_Agenda Equals mov_4050.Id_Agenda
                Join r_u In risorse_umane
                    On
                     mov.Cod_RisUm Equals r_u.Cod_RisUm
                Group Join r_u_produttore In risorse_umane_produttore
                    On
                     mov_4000.Cod_Destinazione Equals r_u_produttore.Cod_RisUm
                    Into r_u_produttore_group = Group
                From _r_u_produttore In r_u_produttore_group.DefaultIfEmpty()
                Join cont In contatti
                    On
                     r_u.Piva Equals cont.Piva And
                     r_u.Cod_Contatto Equals cont.Cod_Contatto
                Join mov_det In movimenti_dettagli
                    On
                     ag.PIVA Equals mov_det.PIVA And
                     mov.Id_Agenda Equals mov_det.Id_Agenda And
                     mov.Id_Mov Equals mov_det.Id_Mov
                Join mat_prima In matPrima.Where(Function(x) x.Piva = piva OrElse x.Sa_Cod = -1)
                    On
                     mov_det.Elem_Cod Equals mat_prima.Elem_Cod And
                     mov_det.Mat_Cod Equals mat_prima.Mat_Cod
                Group Join mat_prima_dett In matprimadett
                    On mat_prima_dett.Mat_Cod Equals mat_prima.Mat_Cod
                    Into mat_prima_dett_group = Group
                From _mat_prima_dett In mat_prima_dett_group.DefaultIfEmpty()
                Group Join otab_param_grpfatt In tabelleParametri.Where(Function(x) x.Tabella_Cod = 20 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On otab_param_grpfatt.Tabella_Par_Cod Equals _mat_prima_dett.Extra_Int1
                    Into otab_param_grpfatt_group = Group
                From _otab_param_grpfatt In otab_param_grpfatt_group.DefaultIfEmpty()
                Group Join mat_prime_camp_certificazioni In matPrimeCampionature.Where(Function(x) x.Tipo = "ocertificazioni")
                    On mat_prime_camp_certificazioni.Progressivo Equals mov_det.Cal_Cod
                    Into mat_prime_camp_certificazioni_group = Group
                From _mpc_certificazioni In mat_prime_camp_certificazioni_group.DefaultIfEmpty()
                Group Join otab_param_cert In tabelleParametri.Where(Function(x) x.Tabella_Cod = 12 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On otab_param_cert.Tabella_Par_Cod Equals _mpc_certificazioni.Tipo_Cod
                    Into otab_param_cert_group = Group
                From _otp_cert In otab_param_cert_group.DefaultIfEmpty()
                Group Join mat_prime_camp_qualita In matPrimeCampionature.Where(Function(x) x.Tipo = "oqualità")
                    On mat_prime_camp_qualita.Progressivo Equals mov_det.Cal_Cod
                    Into mat_prime_camp_qualita_group = Group
                From _mpc_qualita In mat_prime_camp_qualita_group.DefaultIfEmpty()
                Group Join otab_param_qual In tabelleParametri.Where(Function(x) x.Tabella_Cod = 3 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On otab_param_qual.Tabella_Par_Cod Equals _mpc_qualita.Tipo_Cod
                    Into otab_param_qual_group = Group
                From _otp_qual In otab_param_qual_group.DefaultIfEmpty()
                Group Join mat_prime_camp_calibri In matPrimeCampionature.Where(Function(x) x.Tipo = "ocalibro")
                    On mat_prime_camp_calibri.Progressivo Equals mov_det.Cal_Cod
                    Into mat_prime_camp_calibri_group = Group
                From _mpc_calibri In mat_prime_camp_calibri_group.DefaultIfEmpty()
                Group Join otab_param_calibro In tabelleParametri.Where(Function(x) x.Tabella_Cod = 1 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On otab_param_calibro.Tabella_Par_Cod Equals _mpc_calibri.Tipo_Cod
                    Into otab_param_cal_group = Group
                From _otp_cal In otab_param_cal_group.DefaultIfEmpty()
                Group Join mat_prime_camp_rugginosita In matPrimeCampionature.Where(Function(x) x.Tipo = "orugginosita")
                    On mat_prime_camp_rugginosita.Progressivo Equals mov_det.Cal_Cod
                Into mat_prime_camp_rugginosita_group = Group
                From _mpc_rugg In mat_prime_camp_rugginosita_group.DefaultIfEmpty()
                Group Join otab_param_rugg In tabelleParametri.Where(Function(x) x.Tabella_Cod = 22 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On otab_param_rugg.Tabella_Par_Cod Equals _mpc_rugg.Tipo_Cod
                    Into otab_param_rugg_group = Group
                From _otp_rugg In otab_param_rugg_group.DefaultIfEmpty()
                Group Join campconf_mov In GiasContext.CampionamentoConferito_Movimenti.Where(Function(x) x.Piva_SuperUser = Piva_SuperUser)
                                On mov_det.PIVA Equals campconf_mov.PIVA And
                                    mov_det.Id_Mov_Det Equals campconf_mov.Id_Mov_Det Into _campconf_mov = Group
                From _cm In _campconf_mov.DefaultIfEmpty()
                Group Join mat_prime_camp_residuoSecco In matPrimeCampionature.Where(Function(x) x.Tipo = "oresiduosecco")
                    On mat_prime_camp_residuoSecco.Progressivo Equals mov_det.Cal_Cod
                Into mat_prime_camp_residuoSecco_group = Group
                From _mpc_resSecco In mat_prime_camp_residuoSecco_group.DefaultIfEmpty()
                Group Join lmcc In GiasContext.Liquid_Mov_CampionamentoConferito
                                On mov_det.PIVA Equals lmcc.PIVA And
                                    mov_det.Id_Mov_Det Equals lmcc.Id_Mov_Det Into _lmcc = Group
                From ___lmcc In _lmcc.DefaultIfEmpty()
                Group Join anagLiq In GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito.Where(Function(a) a.definitivo = 1)
                                On ___lmcc.Piva_SuperUser Equals anagLiq.Piva_SuperUser And
                                    ___lmcc.PIVA Equals anagLiq.PIVA And
                                    ___lmcc.Id_acconto_liquidazione Equals anagLiq.id_anagrafica Into _anagLiq = Group
                From ___anagLiq In _anagLiq.DefaultIfEmpty()
                Where
                    String.IsNullOrEmpty(___anagLiq.descrizione) AndAlso
                    ag.Split = 0 AndAlso
                    ag.PIVA.Equals(piva) AndAlso
                    lavCodConf.Contains(ag.Lav_Cod) AndAlso
                    mov.PIVA.Equals(piva) AndAlso
                    mov_det.PIVA.Equals(piva) AndAlso
                    mov_det.Elem_Cod = TRASFORMATI_VEGETALI AndAlso
                    (String.IsNullOrEmpty(_dataMovDal) OrElse mov.Data_Movimento >= dataMovDalDateTime) AndAlso
                    (String.IsNullOrEmpty(_dataMovAl) OrElse mov.Data_Movimento <= dataMovAlDateTime) AndAlso
                    (String.IsNullOrEmpty(_docNumeroDes) OrElse mov.Doc_Numero_Des.Contains(_docNumeroDes)) AndAlso
                    (_docNumero = 0 Or mov.Doc_Numero = _docNumero) AndAlso
                    (String.IsNullOrEmpty(_docNumeroSin) OrElse mov.Doc_Numero_Sin.Contains(_docNumeroSin)) AndAlso
                    (mov.Cau_Mov = CAU_CARICO OrElse mov.Cau_Mov = CAU_REGISTRAZIONI_TERZIARIA) AndAlso
                    (mov_4000.Cau_Mov = CAU_REGISTRAZIONI) AndAlso
                    (mov_4050.Cau_Mov = CAU_REGISTRAZIONI_ALLEGATE) AndAlso
                    (_nrRiga = 0 OrElse mov_det.Ordine_Det = _nrRiga OrElse mov_det.Extra_Str.Equals(CStr(_nrRiga))) AndAlso
                    ((_filtroSuCentriAziendali = False) OrElse centri_aziendali.Contains(mov_det.Sa_Cod)) AndAlso
                    ((_filtroSuSpecie = False) OrElse _specie.Contains(mat_prima.Veg_Cod)) AndAlso
                    ((_filtroSuVarieta = False) OrElse _varieta.Contains(mat_prima.Cul_Cod)) AndAlso
                    ((_filtroSuOperazioni = False) OrElse _operazioni.Contains(ag.Lav_Cod)) AndAlso
                    ((_filtroSuFornitori = False) OrElse _fornitori.Contains(cont.Cod_Contatto)) AndAlso
                    ((_filtroSuGruppoFatturazione = False) OrElse gruppoFatturazione.Contains(_mat_prima_dett.Extra_Int1)) AndAlso
                    ((_filtroSuProdotti = 0) OrElse
                    (_filtroSuProdotti = 1 AndAlso _prodotti.Contains(mat_prima.Mat_Cod)) OrElse
                    (_filtroSuProdotti = 2 AndAlso Not _prodotti.Contains(mat_prima.Mat_Cod)))
                Order By mov_det.Mat_Cod, cont.Cod_Contatto, mov.Doc_Numero_Des, mov.Doc_Numero, mov.Doc_Numero_Sin, mov_det.Ordine_Det, mov_det.Extra_Str, mov_det.Id_Mov_Det
                Group By x = New With {
                    Key .Sa_Cod = mov_det.Sa_Cod,
                    Key .Id_Mov_Det = mov_det.Id_Mov_Det,
                    Key .Id_Mov = mov_det.Id_Mov,
                    Key .Id_Agenda = mov_det.Id_Agenda,
                    Key .lav_cod = ag.Lav_Cod,
                    Key .Cal_Cod = mov_det.Cal_Cod,
                    Key .Data_Movimento = mov.Data_Movimento,
                   Key .Cod_Contatto = cont.Cod_Contatto,
                   Key .Cod_RisUm = mov.Cod_RisUm,
                   Key .Cod_Contatto_Produttore = _r_u_produttore.Cod_Contatto,
                   Key .Rag_Soc = cont.Rag_Soc,
                   Key .ConferimentoOAcquisto = If(r_u.Cod_Rapporto = -18, "Conferimento", "Acquisto"),
                    Key .Cod_Rapporto = r_u.Cod_Rapporto,
                   Key .Doc_Numero_Completo = mov.Doc_Numero_Sin & CStr(mov.Doc_Numero) & mov.Doc_Numero_Des,
                   Key .Doc_Numero_Sin = mov.Doc_Numero_Sin,
                   Key .Doc_Numero = mov.Doc_Numero,
                   Key .Doc_Numero_Des = mov.Doc_Numero_Des,
                  Key .NrRiga = If(mov_det.Ordine_Det = 0, mov_det.Extra_Str, CStr(mov_det.Ordine_Det)),
                  Key .Veg_Cod = mat_prima.Veg_Cod,
                   Key .Cul_Cod = mat_prima.Cul_Cod,
                     Key .Reg_Cod = mat_prima.Regolamento,
                  Key .Elem_Cod = mat_prima.Elem_Cod,
                  Key .Mat_Cod = mat_prima.Mat_Cod,
                  Key .Mat_Des = mat_prima.Mat_Des,
                   Key .Grp_Fatt_Cod = If(_otab_param_grpfatt Is Nothing, 0, _otab_param_grpfatt.Tabella_Par_Cod),
                  Key .Grp_Fatt_Descr = If(_otab_param_grpfatt Is Nothing, "", If(_otab_param_grpfatt.Descrizione, "")),
                  Key .Grp_Fatt_Sigla = If(_otab_param_grpfatt Is Nothing, "", If(_otab_param_grpfatt.Sigla, "")),
                   Key .Calibro_Entrata_Cod = If(_otp_cal Is Nothing, 0, _otp_cal.Tabella_Par_Cod),
                  Key .Calibro_Entrata_Descr = If(_otp_cal Is Nothing, "", If(_otp_cal.Descrizione, "")),
                  Key .Calibro_Entrata_Sigla = If(_otp_cal Is Nothing, "", If(_otp_cal.Sigla, "")),
                  Key .Qual_Cod = If(_otp_qual Is Nothing, 0, _otp_qual.Tabella_Par_Cod),
                  Key .Qual_Descr = If(_otp_qual Is Nothing, "", If(_otp_qual.Descrizione, "")),
                    Key .Qual_Sigla = CStr(If(_otp_qual Is Nothing, "", _otp_qual.Sigla)),
                  Key .Certif_Cod = If(_otp_cert Is Nothing, 0, _otp_cert.Tabella_Par_Cod),
                  Key .Certif_Descr = If(_otp_cert Is Nothing, "", If(_otp_cert.Descrizione, "")),
                  Key .Certif_Sigla = If(_otp_cert Is Nothing, "", If(_otp_cert.Sigla, "")),
                  Key .Rugginosita_Cod = If(_otp_rugg Is Nothing, 0, _otp_rugg.Tabella_Par_Cod),
                  Key .Rugginosita_Descr = If(_otp_rugg Is Nothing, "", If(_otp_rugg.Descrizione, "")),
                  Key .Rugginosita_Sigla = CStr(If(_otp_rugg Is Nothing, "", _otp_rugg.Sigla)),
                  Key .DegradoPerc = mov_det.Variazione,
                  Key .Degrado = CInt(Math.Round(mov_det.Qta_Extra_Totale / 100 * mov_det.Variazione)),
                  Key .Qta_Extra_Totale = CInt(mov_det.Qta_Extra_Totale) - CInt(Math.Round(mov_det.Qta_Extra_Totale / 100 * mov_det.Variazione)),
                  Key .Tara = mov_det.Tara,
                  Key .Udm_QtaCampionata = If(_cm Is Nothing, 0, _cm.Udm_QtaCampionata),
                  Key .QtaCampionata = If(_cm Is Nothing, 0.0, _cm.QtaCampionata),
                  Key .Note = If(_cm Is Nothing, "", If(_cm.Note, "")),
                  Key .Automatico = If(_cm Is Nothing, TipoCampionamentoIniziale, _cm.Automatico),
                  Key .StatoCampionamento = If(_cm Is Nothing, StatoCampionamentoIniziale, _cm.StatoCampionamento),
                  Key .TrovataTestataCampionamento = If(_cm Is Nothing, False, True),
                  Key .TrasportoACura = mov.Mezzo,
                  Key .Prezzo_Unitario_Netto_Mov_Det = mov_det.Prezzo_Unitario_Netto,
                  Key .ResiduoSecco = _mpc_resSecco.Val_Cod
                    } Into g = Group
                Select New With
                {
                    .Sa_Cod = x.Sa_Cod,
                    .Id_Mov_Det = x.Id_Mov_Det,
                    .Id_Mov = x.Id_Mov,
                    .Id_Agenda = x.Id_Agenda,
                    .lav_cod = x.lav_cod,
                    .Cal_Cod = x.Cal_Cod,
                    .Data_Movimento = x.Data_Movimento,
                    .Cod_Contatto = x.Cod_Contatto,
                    .Cod_RisUm = x.Cod_RisUm,
                    .Cod_Contatto_Produttore = x.Cod_Contatto_Produttore,
                    .Rag_Soc = x.Rag_Soc,
                    .ConferimentoOAcquisto = x.ConferimentoOAcquisto,
                    .Cod_Rapporto = x.Cod_Rapporto,
                    .Doc_Numero_Completo = x.Doc_Numero_Completo,
                    .Doc_Numero_Sin = x.Doc_Numero_Sin,
                    .Doc_Numero = x.Doc_Numero,
                    .Doc_Numero_Des = x.Doc_Numero_Des,
                    .DDT_Completo = g.Max(Function(r) r.mov_4050.Mov_Desc),
                    .Doc_Numero_Visualizzato = g.Max(Function(r) r.mov_4000.Doc_Numero_Visualizzato),
                    .NrRiga = x.NrRiga,
                    .Veg_Cod = x.Veg_Cod,
                    .Cul_Cod = x.Cul_Cod,
                    .Reg_Cod = x.Reg_Cod,
                    .Elem_Cod = x.Elem_Cod,
                    .Mat_Cod = x.Mat_Cod,
                    .Mat_Des = x.Mat_Des,
                    .Grp_Fatt_Cod = x.Grp_Fatt_Cod,
                    .Grp_Fatt_Descr = x.Grp_Fatt_Descr,
                    .Grp_Fatt_Sigla = x.Grp_Fatt_Sigla,
                    .Calibro_Entrata_Cod = x.Calibro_Entrata_Cod,
                    .Calibro_Entrata_Descr = x.Calibro_Entrata_Descr,
                    .Calibro_Entrata_Sigla = x.Calibro_Entrata_Sigla,
                    .Qual_Cod = x.Qual_Cod,
                    .Qual_Descr = x.Qual_Descr,
                    .Qual_Sigla = x.Qual_Sigla,
                    .Certif_Cod = x.Certif_Cod,
                    .Certif_Descr = x.Certif_Descr,
                    .Certif_Sigla = x.Certif_Sigla,
                    .Rugginosita_Cod = x.Rugginosita_Cod,
                    .Rugginosita_Descr = x.Rugginosita_Descr,
                    .Rugginosita_Sigla = x.Rugginosita_Sigla,
                    .Degrado = x.Degrado,
                    .DegradoPerc = x.DegradoPerc,
                    .Qta_Extra_Totale = x.Qta_Extra_Totale,
                    .Tara = x.Tara,
                    .Udm_QtaCampionata = x.Udm_QtaCampionata,
                    .QtaCampionata = x.QtaCampionata,
                    .Note = x.Note,
                    .Automatico = x.Automatico,
                    .StatoCampionamento = x.StatoCampionamento,
                    .TrovataTestataCampionamento = x.TrovataTestataCampionamento,
                    .TrasportoACura = x.TrasportoACura,
                    .Prezzo_Unitario_Netto_Mov_Det = x.Prezzo_Unitario_Netto_Mov_Det,
                    .ResiduoSecco = x.ResiduoSecco
                }

            Dim listRigheConferimento = RigheConferimento.Distinct().ToList()

            Dim LstDaUtilizzare As Integer = 0
            Dim Lst_Associato_Al_Contatto = 0


            '**********************************************
            '***  Inizio ciclo su righe conferimento    ***
            '**********************************************
            For Each objRigheConfer In listRigheConferimento

                'Imposto la data riferimento prezzi = alla data semina se è gestita
                Dim dataRiferimentoPrezzi = objRigheConfer.Data_Movimento
                Dim tipoDataRiferimentoPrezzi = "E"
                Dim dataRifPrezzi = (From dRif In dt_ParamEntrataXSpecieVarietaXDataRif
                                     Where dRif("Veg_Cod") = objRigheConfer.Veg_Cod And
                           dRif("Cul_Cod") = objRigheConfer.Cul_Cod And
                           dRif("Reg_Cod") = objRigheConfer.Reg_Cod And
                           dRif("Validita_Inizio") <= objRigheConfer.Data_Movimento And
                           dRif("Validita_Fine") >= objRigheConfer.Data_Movimento
                                     Select dRif("Riferimento_Prezzi")).FirstOrDefault()
                If dataRifPrezzi Is Nothing Then
                    dataRifPrezzi = (From dRif In dt_ParamEntrataXSpecieVarietaXDataRif
                                     Where dRif("Veg_Cod") = objRigheConfer.Veg_Cod And
                           dRif("Cul_Cod") = objRigheConfer.Cul_Cod And
                           dRif("Reg_Cod") = 0 And
                           dRif("Validita_Inizio") <= objRigheConfer.Data_Movimento And
                           dRif("Validita_Fine") >= objRigheConfer.Data_Movimento
                                     Select dRif("Riferimento_Prezzi")).FirstOrDefault()
                End If
                If dataRifPrezzi Is Nothing Then
                    dataRifPrezzi = (From dRif In dt_ParamEntrataXSpecieVarietaXDataRif
                                     Where dRif("Veg_Cod") = objRigheConfer.Veg_Cod And
                           dRif("Cul_Cod") = 0 And
                           dRif("Reg_Cod") = objRigheConfer.Reg_Cod And
                           dRif("Validita_Inizio") <= objRigheConfer.Data_Movimento And
                           dRif("Validita_Fine") >= objRigheConfer.Data_Movimento
                                     Select dRif("Riferimento_Prezzi")).FirstOrDefault()
                End If
                If dataRifPrezzi Is Nothing Then
                    dataRifPrezzi = (From dRif In dt_ParamEntrataXSpecieVarietaXDataRif
                                     Where dRif("Veg_Cod") = objRigheConfer.Veg_Cod And
                           dRif("Cul_Cod") = 0 And
                           dRif("Reg_Cod") = 0 And
                           dRif("Validita_Inizio") <= objRigheConfer.Data_Movimento And
                           dRif("Validita_Fine") >= objRigheConfer.Data_Movimento
                                     Select dRif("Riferimento_Prezzi")).FirstOrDefault()
                End If
                If dataRifPrezzi IsNot Nothing Then
                    'Data Semina
                    If dataRifPrezzi.Trim() = "S" Then
                        'Cerco la data semina
                        Dim dataSemina = Leggi_DataSeminaDaConferimento(piva, objRigheConfer.Id_Mov_Det, objParametri)
                        If dataSemina <> AGRODATAINIZIO Then
                            dataRiferimentoPrezzi = dataSemina
                            tipoDataRiferimentoPrezzi = "S"
                        End If
                    End If
                End If


                'Cerco se esiste formula fissa da applicare
                Dim FormulaFissaLiquidazione = (From dRif In dt_ParamEntrataXSpecieVarietaXFormulaFissa
                                                Where dRif("Veg_Cod") = objRigheConfer.Veg_Cod And
                           dRif("Cul_Cod") = objRigheConfer.Cul_Cod And
                           dRif("Reg_Cod") = objRigheConfer.Reg_Cod And
                           dRif("Validita_Inizio") <= objRigheConfer.Data_Movimento And
                           dRif("Validita_Fine") >= objRigheConfer.Data_Movimento And
                           dRif("FormulaFissaLiquidazione") <> 0
                                                Select dRif("FormulaFissaLiquidazione")).FirstOrDefault()
                If FormulaFissaLiquidazione Is Nothing Then
                    FormulaFissaLiquidazione = (From dRif In dt_ParamEntrataXSpecieVarietaXFormulaFissa
                                                Where dRif("Veg_Cod") = objRigheConfer.Veg_Cod And
                           dRif("Cul_Cod") = objRigheConfer.Cul_Cod And
                           dRif("Reg_Cod") = 0 And
                           dRif("Validita_Inizio") <= objRigheConfer.Data_Movimento And
                           dRif("Validita_Fine") >= objRigheConfer.Data_Movimento And
                           dRif("FormulaFissaLiquidazione") <> 0
                                                Select dRif("FormulaFissaLiquidazione")).FirstOrDefault()
                End If
                If FormulaFissaLiquidazione Is Nothing Then
                    FormulaFissaLiquidazione = (From dRif In dt_ParamEntrataXSpecieVarietaXFormulaFissa
                                                Where dRif("Veg_Cod") = objRigheConfer.Veg_Cod And
                           dRif("Cul_Cod") = 0 And
                           dRif("Reg_Cod") = objRigheConfer.Reg_Cod And
                           dRif("Validita_Inizio") <= objRigheConfer.Data_Movimento And
                           dRif("Validita_Fine") >= objRigheConfer.Data_Movimento And
                           dRif("FormulaFissaLiquidazione") <> 0
                                                Select dRif("FormulaFissaLiquidazione")).FirstOrDefault()
                End If
                If FormulaFissaLiquidazione Is Nothing Then
                    FormulaFissaLiquidazione = (From dRif In dt_ParamEntrataXSpecieVarietaXFormulaFissa
                                                Where dRif("Veg_Cod") = objRigheConfer.Veg_Cod And
                           dRif("Cul_Cod") = 0 And
                           dRif("Reg_Cod") = 0 And
                           dRif("Validita_Inizio") <= objRigheConfer.Data_Movimento And
                           dRif("Validita_Fine") >= objRigheConfer.Data_Movimento And
                           dRif("FormulaFissaLiquidazione") <> 0
                                                Select dRif("FormulaFissaLiquidazione")).FirstOrDefault()
                End If

                'Cerco eventuale variazione fissa
                Dim FattVarFormulaFissaPerc = 0
                If FormulaFissaLiquidazione IsNot Nothing AndAlso FormulaFissaLiquidazione = enum_FormuleFisseLiquidazioneFF.FFL_ResiduoSeccoBorlotto_45_50 Then
                    If objRigheConfer.ResiduoSecco > 50 Then
                        FattVarFormulaFissaPerc = objRigheConfer.ResiduoSecco - 50
                    ElseIf objRigheConfer.ResiduoSecco < 45 Then
                        FattVarFormulaFissaPerc = objRigheConfer.ResiduoSecco - 45
                    End If
                End If


                Dim Id_Mov_Det_Str As String = objRigheConfer.Id_Mov_Det.ToString()

                ' Cerco la testata griglia interessata
                Dim Id_Testata_Griglia_Trovata As Integer = 0
                Dim Id_Testata_Griglia_Prod_Trovata As Integer = 0

                Dim s As String = Leggi_Id_Testata_Griglia_Da_Movim_Conferimento(piva, objRigheConfer.Id_Mov_Det, Id_Testata_Griglia_Trovata, Id_Testata_Griglia_Prod_Trovata, False, False, objParametri,
                       objRigheConfer.Id_Mov, objRigheConfer.Elem_Cod, objRigheConfer.Mat_Cod, objRigheConfer.Cal_Cod)

                If Id_Testata_Griglia_Trovata = 0 And objRigheConfer.Prezzo_Unitario_Netto_Mov_Det = 0 Then
                    'Creo una nuova riga - Griglia non trovata
                    dr = DtValorizzazione.NewRow
                    ImpostaDataRowValorizzazioneMovimenti(dr, objRigheConfer, 0, listFattoreVar_ParamQual_Grouped, objRigheConfer.Id_Mov_Det, "prezzo_su_conferito", dataRiferimentoPrezzi, tipoDataRiferimentoPrezzi, "Non trovata griglia di campionamento per il prodotto: prezzo non determinabile")
                    DtValorizzazione.Rows.Add(dr)
                Else
                    Dim lstTotTrovati = 0
                    Dim lstCount As Integer = 0
                    Dim lstStessoPrezzoCount As Integer = 0
                    Dim listinoCod As Integer = 0
                    Dim id_testata_griglia_listino As Integer = 0
                    Dim prezzo_su_conferito As Boolean = False
                    Dim prezzo As Decimal = 0.0

                    'L'eventuale prezzo già presente sulla riga prevale su quello calcolato da liquidazione
                    If objRigheConfer.Prezzo_Unitario_Netto_Mov_Det <> 0 Then
                        listinoCod = 0
                        prezzo_su_conferito = 1
                        id_testata_griglia_listino = 0
                        prezzo = objRigheConfer.Prezzo_Unitario_Netto_Mov_Det
                    Else

                        '-------------------------------------------------------------------------------
                        '  Cerco i listini validi per il prodotto corrente (ne deve esistere solo uno)
                        '-------------------------------------------------------------------------------

                        ' ############ 1. LISTINI PER FORNITORE / PRODUTTORE / CENTRO ############
                        Dim _listini_contatto_produttore_centro = (From l In dtElencoListiniAggiuntiviFornitoreProduttoreCentro.AsEnumerable()
                                                                   Where l("Cod_Contatto") = objRigheConfer.Cod_Contatto And
                                                                            l("Sa_Cod_Listino") = objRigheConfer.Sa_Cod And
                                                                            l("Cod_Contatto_Produttore") = objRigheConfer.Cod_Contatto_Produttore
                                                                   Select CInt(l("Listino"))).ToArray()

                        Dim Listini_Prezzi =
                            From list_prezzi In GiasContext.Listini_Prezzi
                            Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                            On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                            list_prezzi.Piva Equals listiniXgriglie.PIVA And
                            list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                            Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                            On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                            Join list_camp_conferito_prodotti In
                                GiasContext.Listini_CampionamentoConferito_Prodotti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                            On
                            list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                            list_prezzi.Piva Equals list_camp_conferito_prodotti.PIVA And
                            list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod
                            Where
                                _listini_contatto_produttore_centro.Contains(list_prezzi.Listino_Cod) AndAlso
                                list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                (list_prezzi.Piva.Equals(piva)) AndAlso
                                (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi And list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                            Select New With
                            {
                                .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                .Listino_Cod = list_prezzi.Listino_Cod,
                                .Listino_Cod_Des = list_prezzi.Listino_Cod_Des,
                                .Listino_Des = list_prezzi.Listino_Des,
                                .Validita_Inizio = list_prezzi.Validita_Inizio,
                                .Validita_Fine = list_prezzi.Validita_Fine,
                                .Id_TestataGriglia_Prod = list_camp_conferito_prodotti.Id_TestataGriglia_Prod,
                                .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                .prezzo = list_camp_conferito_prodotti.prezzo
                            }

                        ' Cerco su prodotti con prezzo equivalente
                        Dim Listini_Prezzi_Equivalenti = From list_prezzi In GiasContext.Listini_Prezzi
                                                         Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                                On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                                list_prezzi.Piva Equals listiniXgriglie.PIVA And
                                list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                                         Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                            On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                                         Join list_camp_conferito_prodotti_equivalenti In
                                                 GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                            On list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti_equivalenti.Piva_SuperUser And
                                list_prezzi.Piva Equals list_camp_conferito_prodotti_equivalenti.PIVA And
                                list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti_equivalenti.Listino_Cod
                                                         Join list_camp_conferito_prodotti In GiasContext.Listini_CampionamentoConferito_Prodotti
                            On list_camp_conferito_prodotti_equivalenti.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                list_camp_conferito_prodotti_equivalenti.PIVA Equals list_camp_conferito_prodotti.PIVA And
                                list_camp_conferito_prodotti_equivalenti.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod And
                                list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente Equals list_camp_conferito_prodotti.Id_TestataGriglia_Prod
                                                         Where
                                _listini_contatto_produttore_centro.Contains(list_prezzi.Listino_Cod) AndAlso
                                list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                (list_prezzi.Piva.Equals(piva)) AndAlso
                                (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                                                         Select New With
                            {
                                .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                .Listino_Cod = list_prezzi.Listino_Cod,
                                .Listino_Cod_Des = list_prezzi.Listino_Des,
                                .Listino_Des = list_prezzi.Listino_Des,
                                .Validita_Inizio = list_prezzi.Validita_Inizio,
                                .Validita_Fine = list_prezzi.Validita_Fine,
                                .Id_TestataGriglia_Prod = list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente,
                                .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                .prezzo = list_camp_conferito_prodotti.prezzo
                            }

                        ' ############ 2. LISTINI PER FORNITORE / PRODUTTORE ############
                        ' Se non ho trovato prezzi associati al fornitore / produttore / centro o a un listino equivalente cerco sui listini per fornitore / produttore
                        If Listini_Prezzi.Count() = 0 AndAlso Listini_Prezzi_Equivalenti.Count() = 0 Then

                            Dim _listini_contatto_produttore = (From l In dtElencoListiniAggiuntiviFornitoreProduttore.AsEnumerable()
                                                                Where l("Cod_Contatto") = objRigheConfer.Cod_Contatto And
                                                                    l("Cod_Contatto_Produttore") = objRigheConfer.Cod_Contatto_Produttore
                                                                Select CInt(l("Listino"))).ToArray()

                            If _listini_contatto_produttore.Length > 0 Then

                                Listini_Prezzi =
                                From list_prezzi In GiasContext.Listini_Prezzi
                                Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                                On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                                list_prezzi.Piva Equals listiniXgriglie.PIVA And
                                list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                                On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                    list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                    list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                Join list_camp_conferito_prodotti In
                                    GiasContext.Listini_CampionamentoConferito_Prodotti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                                On
                                list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                list_prezzi.Piva Equals list_camp_conferito_prodotti.PIVA And
                                list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod
                                Where
                                    _listini_contatto_produttore.Contains(list_prezzi.Listino_Cod) AndAlso
                                    list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                    (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                    (list_prezzi.Piva.Equals(piva)) AndAlso
                                    (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi And list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                    (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                                Select New With
                                {
                                    .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                    .Listino_Cod = list_prezzi.Listino_Cod,
                                    .Listino_Cod_Des = list_prezzi.Listino_Cod_Des,
                                    .Listino_Des = list_prezzi.Listino_Des,
                                    .Validita_Inizio = list_prezzi.Validita_Inizio,
                                    .Validita_Fine = list_prezzi.Validita_Fine,
                                    .Id_TestataGriglia_Prod = list_camp_conferito_prodotti.Id_TestataGriglia_Prod,
                                    .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                    .prezzo = list_camp_conferito_prodotti.prezzo
                                }

                                ' Cerco su prodotti con prezzo equivalente
                                Listini_Prezzi_Equivalenti = From list_prezzi In GiasContext.Listini_Prezzi
                                                             Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                                        On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                                        list_prezzi.Piva Equals listiniXgriglie.PIVA And
                                        list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                                             Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                                    On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                        list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                        list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                                             Join list_camp_conferito_prodotti_equivalenti In
                                                         GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                                    On list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti_equivalenti.Piva_SuperUser And
                                        list_prezzi.Piva Equals list_camp_conferito_prodotti_equivalenti.PIVA And
                                        list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti_equivalenti.Listino_Cod
                                                             Join list_camp_conferito_prodotti In GiasContext.Listini_CampionamentoConferito_Prodotti
                                    On list_camp_conferito_prodotti_equivalenti.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                        list_camp_conferito_prodotti_equivalenti.PIVA Equals list_camp_conferito_prodotti.PIVA And
                                        list_camp_conferito_prodotti_equivalenti.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod And
                                        list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente Equals list_camp_conferito_prodotti.Id_TestataGriglia_Prod
                                                             Where
                                        _listini_contatto_produttore.Contains(list_prezzi.Listino_Cod) AndAlso
                                        list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                        (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                        (list_prezzi.Piva.Equals(piva)) AndAlso
                                        (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                        (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                                                             Select New With
                                    {
                                        .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                        .Listino_Cod = list_prezzi.Listino_Cod,
                                        .Listino_Cod_Des = list_prezzi.Listino_Des,
                                        .Listino_Des = list_prezzi.Listino_Des,
                                        .Validita_Inizio = list_prezzi.Validita_Inizio,
                                        .Validita_Fine = list_prezzi.Validita_Fine,
                                        .Id_TestataGriglia_Prod = list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente,
                                        .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                        .prezzo = list_camp_conferito_prodotti.prezzo
                                    }

                            End If
                        End If

                        ' ############ 3. LISTINI PER FORNITORE / CENTRO ############
                        ' Se non ho trovato prezzi associati al fornitore / produttore o a un listino equivalente cerco sui listini per fornitore / centro
                        If Listini_Prezzi.Count() = 0 AndAlso Listini_Prezzi_Equivalenti.Count() = 0 Then

                            'Cerco gli eventuali listini associati al fornitore / centro
                            Dim _listini_contatto_centro = (From l In dtElencoListiniAggiuntiviFornitoreCentro.AsEnumerable()
                                                            Where l("Cod_Contatto") = objRigheConfer.Cod_Contatto And
                                                                l("Sa_Cod_Listino") = objRigheConfer.Sa_Cod
                                                            Select CInt(l("Listino"))).ToArray()

                            If _listini_contatto_centro.Length > 0 Then


                                Listini_Prezzi =
                                From list_prezzi In GiasContext.Listini_Prezzi
                                Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                                On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                                list_prezzi.Piva Equals listiniXgriglie.PIVA And
                                list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                                On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                    list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                    list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                Join list_camp_conferito_prodotti In
                                    GiasContext.Listini_CampionamentoConferito_Prodotti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                                On
                                list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                list_prezzi.Piva Equals list_camp_conferito_prodotti.PIVA And
                                list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod
                                Where
                                    _listini_contatto_centro.Contains(list_prezzi.Listino_Cod) AndAlso
                                    list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                    (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                    (list_prezzi.Piva.Equals(piva)) AndAlso
                                    (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi And list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                    (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                                Select New With
                                {
                                    .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                    .Listino_Cod = list_prezzi.Listino_Cod,
                                    .Listino_Cod_Des = list_prezzi.Listino_Cod_Des,
                                    .Listino_Des = list_prezzi.Listino_Des,
                                    .Validita_Inizio = list_prezzi.Validita_Inizio,
                                    .Validita_Fine = list_prezzi.Validita_Fine,
                                    .Id_TestataGriglia_Prod = list_camp_conferito_prodotti.Id_TestataGriglia_Prod,
                                    .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                    .prezzo = list_camp_conferito_prodotti.prezzo
                                }

                                ' Cerco su prodotti con prezzo equivalente
                                Listini_Prezzi_Equivalenti = From list_prezzi In GiasContext.Listini_Prezzi
                                                             Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                                        On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                                        list_prezzi.Piva Equals listiniXgriglie.PIVA And
                                        list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                                             Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                                    On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                        list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                        list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                                             Join list_camp_conferito_prodotti_equivalenti In
                                                         GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                                    On list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti_equivalenti.Piva_SuperUser And
                                        list_prezzi.Piva Equals list_camp_conferito_prodotti_equivalenti.PIVA And
                                        list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti_equivalenti.Listino_Cod
                                                             Join list_camp_conferito_prodotti In GiasContext.Listini_CampionamentoConferito_Prodotti
                                    On list_camp_conferito_prodotti_equivalenti.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                        list_camp_conferito_prodotti_equivalenti.PIVA Equals list_camp_conferito_prodotti.PIVA And
                                        list_camp_conferito_prodotti_equivalenti.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod And
                                        list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente Equals list_camp_conferito_prodotti.Id_TestataGriglia_Prod
                                                             Where
                                        _listini_contatto_centro.Contains(list_prezzi.Listino_Cod) AndAlso
                                        list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                        (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                        (list_prezzi.Piva.Equals(piva)) AndAlso
                                        (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                        (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                                                             Select New With
                                    {
                                        .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                        .Listino_Cod = list_prezzi.Listino_Cod,
                                        .Listino_Cod_Des = list_prezzi.Listino_Des,
                                        .Listino_Des = list_prezzi.Listino_Des,
                                        .Validita_Inizio = list_prezzi.Validita_Inizio,
                                        .Validita_Fine = list_prezzi.Validita_Fine,
                                        .Id_TestataGriglia_Prod = list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente,
                                        .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                        .prezzo = list_camp_conferito_prodotti.prezzo
                                    }
                            End If
                        End If

                        ' ############ 4. LISTINI PER FORNITORE ############
                        ' Se non ho trovato prezzi associati al fornitore / centro o a un listino equivalente cerco sui listini per solo fornitore
                        If Listini_Prezzi.Count() = 0 AndAlso Listini_Prezzi_Equivalenti.Count() = 0 Then

                            'Cerco gli eventuali listini associati al fornitore
                            Dim _listini_contatto = (From l In dtElencoListiniPrincipaliFornitore.AsEnumerable()
                                                     Where l("Cod_Contatto") = objRigheConfer.Cod_Contatto
                                                     Select CInt(l("Listino"))).ToArray()


                            If _listini_contatto.Length > 0 Then

                                Listini_Prezzi =
                                From list_prezzi In GiasContext.Listini_Prezzi
                                Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                                On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                                list_prezzi.Piva Equals listiniXgriglie.PIVA And
                                list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                                On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                    list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                    list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                Join list_camp_conferito_prodotti In
                                    GiasContext.Listini_CampionamentoConferito_Prodotti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                                On
                                list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                list_prezzi.Piva Equals list_camp_conferito_prodotti.PIVA And
                                list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod
                                Where
                                    _listini_contatto.Contains(list_prezzi.Listino_Cod) AndAlso
                                    list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                    (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                    (list_prezzi.Piva.Equals(piva)) AndAlso
                                    (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi And list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                    (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                                Select New With
                                {
                                    .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                    .Listino_Cod = list_prezzi.Listino_Cod,
                                    .Listino_Cod_Des = list_prezzi.Listino_Cod_Des,
                                    .Listino_Des = list_prezzi.Listino_Des,
                                    .Validita_Inizio = list_prezzi.Validita_Inizio,
                                    .Validita_Fine = list_prezzi.Validita_Fine,
                                    .Id_TestataGriglia_Prod = list_camp_conferito_prodotti.Id_TestataGriglia_Prod,
                                    .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                    .prezzo = list_camp_conferito_prodotti.prezzo
                                }

                                ' Cerco su prodotti con prezzo equivalente
                                Listini_Prezzi_Equivalenti = From list_prezzi In GiasContext.Listini_Prezzi
                                                             Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                                On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                                list_prezzi.Piva Equals listiniXgriglie.PIVA And
                                list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                                             Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                                On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                                             Join list_camp_conferito_prodotti_equivalenti In
                                                 GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                                On list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti_equivalenti.Piva_SuperUser And
                                list_prezzi.Piva Equals list_camp_conferito_prodotti_equivalenti.PIVA And
                                list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti_equivalenti.Listino_Cod
                                                             Join list_camp_conferito_prodotti In GiasContext.Listini_CampionamentoConferito_Prodotti
                                On list_camp_conferito_prodotti_equivalenti.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                list_camp_conferito_prodotti_equivalenti.PIVA Equals list_camp_conferito_prodotti.PIVA And
                                list_camp_conferito_prodotti_equivalenti.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod And
                                list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente Equals list_camp_conferito_prodotti.Id_TestataGriglia_Prod
                                                             Where
                                _listini_contatto.Contains(list_prezzi.Listino_Cod) AndAlso
                                list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                (list_prezzi.Piva.Equals(piva)) AndAlso
                                (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                                                             Select New With
                                {
                                    .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                    .Listino_Cod = list_prezzi.Listino_Cod,
                                    .Listino_Cod_Des = list_prezzi.Listino_Des,
                                    .Listino_Des = list_prezzi.Listino_Des,
                                    .Validita_Inizio = list_prezzi.Validita_Inizio,
                                    .Validita_Fine = list_prezzi.Validita_Fine,
                                    .Id_TestataGriglia_Prod = list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente,
                                    .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                    .prezzo = list_camp_conferito_prodotti.prezzo
                                }

                            End If
                        End If

                        ' ############ 5. LISTINI PER RAPPORTO CONTABILE / CENTRO ############
                        ' Se non ho trovato prezzi associati al solo fornitore o a un listino equivalente cerco sui listini per solo rapporto contabile
                        If Listini_Prezzi.Count() = 0 AndAlso Listini_Prezzi_Equivalenti.Count() = 0 Then

                            'Cerco gli eventuali listini associati al solo rapporto contabile
                            Dim _listini_rapporto_cont_centro = (From l In dtElencoListiniRapportoContabileCentro.AsEnumerable()
                                                                 Where l("Cod_Rapporto") = objRigheConfer.Cod_Rapporto And
                                                                     l("Sa_Cod_Listino") = objRigheConfer.Sa_Cod
                                                                 Select CInt(l("Listino"))).ToArray()

                            If _listini_rapporto_cont_centro.Length > 0 Then

                                Listini_Prezzi =
                                From list_prezzi In GiasContext.Listini_Prezzi
                                Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                                On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                                list_prezzi.Piva Equals listiniXgriglie.PIVA And
                                list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                                On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                    list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                    list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                Join list_camp_conferito_prodotti In
                                    GiasContext.Listini_CampionamentoConferito_Prodotti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                                On
                                list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                list_prezzi.Piva Equals list_camp_conferito_prodotti.PIVA And
                                list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod
                                Where
                                    _listini_rapporto_cont_centro.Contains(list_prezzi.Listino_Cod) AndAlso
                                    list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                    (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                    (list_prezzi.Piva.Equals(piva)) AndAlso
                                    (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi And list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                    (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                                Select New With
                                {
                                    .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                    .Listino_Cod = list_prezzi.Listino_Cod,
                                    .Listino_Cod_Des = list_prezzi.Listino_Cod_Des,
                                    .Listino_Des = list_prezzi.Listino_Des,
                                    .Validita_Inizio = list_prezzi.Validita_Inizio,
                                    .Validita_Fine = list_prezzi.Validita_Fine,
                                    .Id_TestataGriglia_Prod = list_camp_conferito_prodotti.Id_TestataGriglia_Prod,
                                    .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                    .prezzo = list_camp_conferito_prodotti.prezzo
                                }

                                ' Cerco su prodotti con prezzo equivalente
                                Listini_Prezzi_Equivalenti = From list_prezzi In GiasContext.Listini_Prezzi
                                                             Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                                    On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                                    list_prezzi.Piva Equals listiniXgriglie.PIVA And
                                    list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                                             Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                                On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                    list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                    list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                                             Join list_camp_conferito_prodotti_equivalenti In
                                                     GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                                On list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti_equivalenti.Piva_SuperUser And
                                    list_prezzi.Piva Equals list_camp_conferito_prodotti_equivalenti.PIVA And
                                    list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti_equivalenti.Listino_Cod
                                                             Join list_camp_conferito_prodotti In GiasContext.Listini_CampionamentoConferito_Prodotti
                                On list_camp_conferito_prodotti_equivalenti.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                    list_camp_conferito_prodotti_equivalenti.PIVA Equals list_camp_conferito_prodotti.PIVA And
                                    list_camp_conferito_prodotti_equivalenti.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod And
                                    list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente Equals list_camp_conferito_prodotti.Id_TestataGriglia_Prod
                                                             Where
                                    _listini_rapporto_cont_centro.Contains(list_prezzi.Listino_Cod) AndAlso
                                    list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                    (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                    (list_prezzi.Piva.Equals(piva)) AndAlso
                                    (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                    (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                                                             Select New With
                                {
                                    .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                    .Listino_Cod = list_prezzi.Listino_Cod,
                                    .Listino_Cod_Des = list_prezzi.Listino_Des,
                                    .Listino_Des = list_prezzi.Listino_Des,
                                    .Validita_Inizio = list_prezzi.Validita_Inizio,
                                    .Validita_Fine = list_prezzi.Validita_Fine,
                                    .Id_TestataGriglia_Prod = list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente,
                                    .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                    .prezzo = list_camp_conferito_prodotti.prezzo
                                }

                            End If
                        End If

                        ' ############ 6. LISTINI PER CENTRO ############
                        ' Se non ho trovato prezzi associati al rapporto contabile / centro o a un listino equivalente cerco sui listini per solo centro
                        If Listini_Prezzi.Count() = 0 AndAlso Listini_Prezzi_Equivalenti.Count() = 0 Then

                            'Cerco gli eventuali listini associati al solo centro
                            Dim _listini_centro = (From l In dtElencoListiniAggiuntiviCentro.AsEnumerable()
                                                   Where l("Sa_Cod_Listino") = objRigheConfer.Sa_Cod
                                                   Select CInt(l("Listino"))).ToArray()

                            If _listini_centro.Length > 0 Then
                                Listini_Prezzi =
                                From list_prezzi In GiasContext.Listini_Prezzi
                                Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                                On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                                list_prezzi.Piva Equals listiniXgriglie.PIVA And
                                list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                                On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                    list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                    list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                Join list_camp_conferito_prodotti In
                                    GiasContext.Listini_CampionamentoConferito_Prodotti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                                On
                                list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                list_prezzi.Piva Equals list_camp_conferito_prodotti.PIVA And
                                list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod
                                Where
                                    _listini_centro.Contains(list_prezzi.Listino_Cod) AndAlso
                                    list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                    (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                    (list_prezzi.Piva.Equals(piva)) AndAlso
                                    (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi And list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                    (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                                Select New With
                                {
                                    .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                    .Listino_Cod = list_prezzi.Listino_Cod,
                                    .Listino_Cod_Des = list_prezzi.Listino_Cod_Des,
                                    .Listino_Des = list_prezzi.Listino_Des,
                                    .Validita_Inizio = list_prezzi.Validita_Inizio,
                                    .Validita_Fine = list_prezzi.Validita_Fine,
                                    .Id_TestataGriglia_Prod = list_camp_conferito_prodotti.Id_TestataGriglia_Prod,
                                    .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                    .prezzo = list_camp_conferito_prodotti.prezzo
                                }

                                ' Cerco su prodotti con prezzo equivalente
                                Listini_Prezzi_Equivalenti = From list_prezzi In GiasContext.Listini_Prezzi
                                                             Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                                    On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                                    list_prezzi.Piva Equals listiniXgriglie.PIVA And
                                    list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                                             Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                                On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                    list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                    list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                                             Join list_camp_conferito_prodotti_equivalenti In
                                                     GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                                On list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti_equivalenti.Piva_SuperUser And
                                    list_prezzi.Piva Equals list_camp_conferito_prodotti_equivalenti.PIVA And
                                    list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti_equivalenti.Listino_Cod
                                                             Join list_camp_conferito_prodotti In GiasContext.Listini_CampionamentoConferito_Prodotti
                                On list_camp_conferito_prodotti_equivalenti.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                    list_camp_conferito_prodotti_equivalenti.PIVA Equals list_camp_conferito_prodotti.PIVA And
                                    list_camp_conferito_prodotti_equivalenti.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod And
                                    list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente Equals list_camp_conferito_prodotti.Id_TestataGriglia_Prod
                                                             Where
                                    _listini_centro.Contains(list_prezzi.Listino_Cod) AndAlso
                                    list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                    (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                    (list_prezzi.Piva.Equals(piva)) AndAlso
                                    (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                    (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                                                             Select New With
                                {
                                    .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                    .Listino_Cod = list_prezzi.Listino_Cod,
                                    .Listino_Cod_Des = list_prezzi.Listino_Des,
                                    .Listino_Des = list_prezzi.Listino_Des,
                                    .Validita_Inizio = list_prezzi.Validita_Inizio,
                                    .Validita_Fine = list_prezzi.Validita_Fine,
                                    .Id_TestataGriglia_Prod = list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente,
                                    .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                    .prezzo = list_camp_conferito_prodotti.prezzo
                                }

                            End If
                        End If

                        ' ############ 7. LISTINI PER RAPPORTO CONTABILE ############
                        ' Se non ho trovato prezzi associati al solo centro o a un listino equivalente cerco sui listini per solo rapporto contabile
                        If Listini_Prezzi.Count() = 0 AndAlso Listini_Prezzi_Equivalenti.Count() = 0 Then

                            'Cerco gli eventuali listini associati al solo rapporto contabile
                            Dim _listini_rapporto_cont = (From l In dtElencoListiniRapportoContabile.AsEnumerable()
                                                          Where l("Cod_Rapporto") = objRigheConfer.Cod_Rapporto
                                                          Select CInt(l("Listino"))).ToArray()

                            If _listini_rapporto_cont.Length > 0 Then
                                Listini_Prezzi =
                                From list_prezzi In GiasContext.Listini_Prezzi
                                Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                                On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                                list_prezzi.Piva Equals listiniXgriglie.PIVA And
                                list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                                On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                    list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                    list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                Join list_camp_conferito_prodotti In
                                    GiasContext.Listini_CampionamentoConferito_Prodotti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                                On
                                list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                list_prezzi.Piva Equals list_camp_conferito_prodotti.PIVA And
                                list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod
                                Where
                                    _listini_rapporto_cont.Contains(list_prezzi.Listino_Cod) AndAlso
                                    list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                    (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                    (list_prezzi.Piva.Equals(piva)) AndAlso
                                    (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi And list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                    (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                                Select New With
                                {
                                    .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                    .Listino_Cod = list_prezzi.Listino_Cod,
                                    .Listino_Cod_Des = list_prezzi.Listino_Cod_Des,
                                    .Listino_Des = list_prezzi.Listino_Des,
                                    .Validita_Inizio = list_prezzi.Validita_Inizio,
                                    .Validita_Fine = list_prezzi.Validita_Fine,
                                    .Id_TestataGriglia_Prod = list_camp_conferito_prodotti.Id_TestataGriglia_Prod,
                                    .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                    .prezzo = list_camp_conferito_prodotti.prezzo
                                }

                                ' Cerco su prodotti con prezzo equivalente
                                Listini_Prezzi_Equivalenti = From list_prezzi In GiasContext.Listini_Prezzi
                                                             Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                                    On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                                    list_prezzi.Piva Equals listiniXgriglie.PIVA And
                                    list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                                             Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                                On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                    list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                    list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                                             Join list_camp_conferito_prodotti_equivalenti In
                                                     GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                                On list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti_equivalenti.Piva_SuperUser And
                                    list_prezzi.Piva Equals list_camp_conferito_prodotti_equivalenti.PIVA And
                                    list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti_equivalenti.Listino_Cod
                                                             Join list_camp_conferito_prodotti In GiasContext.Listini_CampionamentoConferito_Prodotti
                                On list_camp_conferito_prodotti_equivalenti.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                    list_camp_conferito_prodotti_equivalenti.PIVA Equals list_camp_conferito_prodotti.PIVA And
                                    list_camp_conferito_prodotti_equivalenti.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod And
                                    list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente Equals list_camp_conferito_prodotti.Id_TestataGriglia_Prod
                                                             Where
                                    _listini_rapporto_cont.Contains(list_prezzi.Listino_Cod) AndAlso
                                    list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                    (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                    (list_prezzi.Piva.Equals(piva)) AndAlso
                                    (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                    (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi)
                                                             Select New With
                                {
                                    .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                    .Listino_Cod = list_prezzi.Listino_Cod,
                                    .Listino_Cod_Des = list_prezzi.Listino_Des,
                                    .Listino_Des = list_prezzi.Listino_Des,
                                    .Validita_Inizio = list_prezzi.Validita_Inizio,
                                    .Validita_Fine = list_prezzi.Validita_Fine,
                                    .Id_TestataGriglia_Prod = list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente,
                                    .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                    .prezzo = list_camp_conferito_prodotti.prezzo
                                }

                            End If
                        End If

                        ' ############ 8. LISTINI BASE ############
                        ' Se non ho trovato prezzi associati al solo rapporto contabile o a un listino equivalente cerco un listino base
                        ' 10/2/2020 che non sia associato ad altro fornitore
                        If Listini_Prezzi.Count() = 0 AndAlso Listini_Prezzi_Equivalenti.Count() = 0 Then
                            Listini_Prezzi =
                               From list_prezzi In GiasContext.Listini_Prezzi
                               Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                            On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                            list_prezzi.Piva Equals listiniXgriglie.PIVA And
                            list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                               Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                               On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                   list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                   list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                               Join list_camp_conferito_prodotti In
                                   GiasContext.Listini_CampionamentoConferito_Prodotti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                               On
                               list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                               list_prezzi.Piva Equals list_camp_conferito_prodotti.PIVA And
                               list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod
                               Where
                                  list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                   list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                                   list_prezzi.Piva.Equals(piva) AndAlso
                                   (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                   (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                    ((_esistonoListiniFornitoreProduttoreCentro = False) OrElse Not _elenco_listini_FornitoreProduttoreCentro.Contains(list_prezzi.Listino_Cod)) AndAlso
                                    ((_esistonoListiniFornitoreProduttore = False) OrElse Not _elenco_listini_FornitoreProduttore.Contains(list_prezzi.Listino_Cod)) AndAlso
                                    ((_esistonoListiniFornitoreCentro = False) OrElse Not _elenco_listini_FornitoreCentro.Contains(list_prezzi.Listino_Cod)) AndAlso
                                    ((_esistonoListiniFornitore = False) OrElse Not _elenco_listini_Fornitore.Contains(list_prezzi.Listino_Cod)) AndAlso
                                    ((_esistonoListiniRapportoContabileCentro = False) OrElse Not _elenco_listini_RapportoContabileCentro.Contains(list_prezzi.Listino_Cod)) AndAlso
                                    ((_esistonoListiniCentro = False) OrElse Not _elenco_listini_Centro.Contains(list_prezzi.Listino_Cod)) AndAlso
                                    ((_esistonoListiniRapportoContabile = False) OrElse Not _elenco_listini_RapportoContabile.Contains(list_prezzi.Listino_Cod))
                               Select New With
                               {
                                   .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                   .Listino_Cod = list_prezzi.Listino_Cod,
                                   .Listino_Cod_Des = list_prezzi.Listino_Cod_Des,
                                   .Listino_Des = list_prezzi.Listino_Des,
                                   .Validita_Inizio = list_prezzi.Validita_Inizio,
                                   .Validita_Fine = list_prezzi.Validita_Fine,
                                   .Id_TestataGriglia_Prod = list_camp_conferito_prodotti.Id_TestataGriglia_Prod,
                                   .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                   .prezzo = list_camp_conferito_prodotti.prezzo
                               }

                            ' Non ho trovato prezzo: cerco su prodotti con prezzo equivalente
                            ' 10/2/2020 che non sia associato ad altro fornitore
                            Listini_Prezzi_Equivalenti = From list_prezzi In GiasContext.Listini_Prezzi
                                                         Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                            On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                            list_prezzi.Piva Equals listiniXgriglie.PIVA And
                            list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                                                         Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                                On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                    list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                    list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                                                         Join list_camp_conferito_prodotti_equivalenti In
                                                     GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                                On list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti_equivalenti.Piva_SuperUser And
                                    list_prezzi.Piva Equals list_camp_conferito_prodotti_equivalenti.PIVA And
                                    list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti_equivalenti.Listino_Cod
                                                         Join list_camp_conferito_prodotti In GiasContext.Listini_CampionamentoConferito_Prodotti
                                On list_camp_conferito_prodotti_equivalenti.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                                    list_camp_conferito_prodotti_equivalenti.PIVA Equals list_camp_conferito_prodotti.PIVA And
                                    list_camp_conferito_prodotti_equivalenti.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod And
                                    list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente Equals list_camp_conferito_prodotti.Id_TestataGriglia_Prod
                                                         Where
                                    list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                    (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                    (list_prezzi.Piva.Equals(piva)) AndAlso
                                    (list_prezzi.Validita_Inizio <= dataRiferimentoPrezzi And list_prezzi.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                    (list_camp_conferito_prodotti.Validita_Inizio <= dataRiferimentoPrezzi AndAlso list_camp_conferito_prodotti.Validita_Fine >= dataRiferimentoPrezzi) AndAlso
                                    ((_esistonoListiniFornitoreProduttoreCentro = False) OrElse Not _elenco_listini_FornitoreProduttoreCentro.Contains(list_prezzi.Listino_Cod)) AndAlso
                                    ((_esistonoListiniFornitoreProduttore = False) OrElse Not _elenco_listini_FornitoreProduttore.Contains(list_prezzi.Listino_Cod)) AndAlso
                                    ((_esistonoListiniFornitoreCentro = False) OrElse Not _elenco_listini_FornitoreCentro.Contains(list_prezzi.Listino_Cod)) AndAlso
                                    ((_esistonoListiniFornitore = False) OrElse Not _elenco_listini_Fornitore.Contains(list_prezzi.Listino_Cod)) AndAlso
                                    ((_esistonoListiniRapportoContabileCentro = False) OrElse Not _elenco_listini_RapportoContabileCentro.Contains(list_prezzi.Listino_Cod)) AndAlso
                                    ((_esistonoListiniCentro = False) OrElse Not _elenco_listini_Centro.Contains(list_prezzi.Listino_Cod)) AndAlso
                                    ((_esistonoListiniRapportoContabile = False) OrElse Not _elenco_listini_RapportoContabile.Contains(list_prezzi.Listino_Cod))
                                                         Select New With
                                    {
                                    .Listino_Classe_Des = list_classi_prezzi.Listino_Classe_Des,
                                    .Listino_Cod = list_prezzi.Listino_Cod,
                                    .Listino_Cod_Des = list_prezzi.Listino_Des,
                                    .Listino_Des = list_prezzi.Listino_Des,
                                    .Validita_Inizio = list_prezzi.Validita_Inizio,
                                    .Validita_Fine = list_prezzi.Validita_Fine,
                                    .Id_TestataGriglia_Prod = list_camp_conferito_prodotti_equivalenti.Id_TestataGriglia_Prod_Equivalente,
                                    .prezzo_su_conferito = list_camp_conferito_prodotti.prezzo_su_conferito,
                                    .prezzo = list_camp_conferito_prodotti.prezzo
                                }
                        End If

                        lstCount = Listini_Prezzi.Count()
                        lstStessoPrezzoCount = Listini_Prezzi_Equivalenti.Count()
                        lstTotTrovati = lstCount + lstStessoPrezzoCount
                        If lstTotTrovati = 0 OrElse lstTotTrovati > 1 Then

                            Dim mess As String = ""
                            If lstTotTrovati = 0 Then
                                mess = "Prezzo non trovato"
                            End If
                            If lstTotTrovati > 1 Then
                                mess = "Non è possibile stabilire il prezzo; sono state trovate " & lstTotTrovati.ToString & " righe attive sui listini: "
                                Dim primoGiro = True
                                For Each lst In Listini_Prezzi.ToList
                                    If Not primoGiro Then
                                        mess += ", "
                                    End If
                                    mess += lst.Listino_Des
                                    primoGiro = False
                                Next
                                For Each lst In Listini_Prezzi_Equivalenti.ToList
                                    If Not primoGiro Then
                                        mess += ", "
                                    End If
                                    mess += lst.Listino_Des
                                    primoGiro = False
                                Next
                            End If

                            'Preparo il messaggio di errore in modo diverso a seconda del fatto che esistano campionamenti
                            If Not objRigheConfer.TrovataTestataCampionamento Then
                                'Creo una nuova riga di errore
                                dr = DtValorizzazione.NewRow
                                ImpostaDataRowValorizzazioneMovimenti(dr, objRigheConfer, 0, listFattoreVar_ParamQual_Grouped, objRigheConfer.Id_Mov_Det, "prezzo_su_conferito", dataRiferimentoPrezzi, tipoDataRiferimentoPrezzi, mess)
                                DtValorizzazione.Rows.Add(dr)
                            Else

                                Dim Qta_Extra_TotaleDecimal = Decimal.Parse(objRigheConfer.Qta_Extra_Totale)
                                Dim QtaCampionataDecimal = Decimal.Parse(objRigheConfer.QtaCampionata)
                                Dim TaraTotaleDecimal = Decimal.Parse(objRigheConfer.Tara)

                                ' Cerco le righe di campionamento
                                Dim Campionamenti_Conferimento_Righe = Leggi_Righe_Campionamento_Per_Valorizzazione(piva, objRigheConfer.Id_Mov_Det,
                                    Id_Testata_Griglia_Trovata, Id_Testata_Griglia_Prod_Trovata, Qta_Extra_TotaleDecimal, QtaCampionataDecimal, TaraTotaleDecimal, mess, objParametri)

                                'Non ho trovato righe di campionamento ma c'era la testata per prodotto con prezzo su campionato 
                                ' Succede se cambiano prodotto o caratteristiche sulla riga successivamente al campionamento
                                If Campionamenti_Conferimento_Righe.Count() = 0 AndAlso objRigheConfer.TrovataTestataCampionamento Then
                                    'Creo una nuova riga di errore
                                    dr = DtValorizzazione.NewRow
                                    ImpostaDataRowValorizzazioneMovimenti(dr, objRigheConfer, Id_Testata_Griglia_Trovata, listFattoreVar_ParamQual_Grouped, objRigheConfer.Id_Mov_Det, "prezzo_su_campionato", dataRiferimentoPrezzi, tipoDataRiferimentoPrezzi, "Prodotto o caratteristiche modificate rispetto a quelle con cui è avvenuto il campionamento. I dati attuali riportano ad una griglia di camp. diversa: cancellare il campionamento e reinserirlo")
                                    DtValorizzazione.Rows.Add(dr)

                                Else

                                    For Each objRigheCampionamento In Campionamenti_Conferimento_Righe
                                        Dim KeyRigaConferimentoECalibro As String = Id_Mov_Det_Str & "-" & objRigheCampionamento.Id_Calibro.ToString()
                                        'Creo una nuova riga
                                        dr = DtValorizzazione.NewRow
                                        ImpostaDataRowValorizzazioneMovimenti(dr, objRigheConfer, Id_Testata_Griglia_Trovata, listFattoreVar_ParamQual_Grouped, KeyRigaConferimentoECalibro, "prezzo_su_campionato", dataRiferimentoPrezzi, tipoDataRiferimentoPrezzi, mess)

                                        dr.Item("Id_Calibro") = objRigheCampionamento.Id_Calibro
                                        dr.Item("Descr_Qualita_Camp") = objRigheCampionamento.Descr_Qualita_Camp
                                        dr.Item("Descr_Calibro_Camp") = objRigheCampionamento.Descr_Calibro_Camp
                                        dr.Item("Descr_QualCalibro_Camp") = objRigheCampionamento.Descr_QualCalibro_Camp
                                        dr.Item("Descr_OrdinQualCalibro_Camp") = objRigheCampionamento.Descr_OrdinQualCalibro_Camp
                                        dr.Item("Ordinamento_Calibro") = objRigheCampionamento.Ordinamento
                                        dr.Item("QtaCampionata") = Decimal.Round(Convert.ToDecimal(objRigheCampionamento.QtaCampionata), 5)
                                        dr.Item("TaraCampionata") = Decimal.Round(Convert.ToDecimal(objRigheCampionamento.TaraCampionata), 5)
                                        dr.Item("Percentuale_Campionato") = Decimal.Round(Convert.ToDecimal(objRigheCampionamento.Percentuale_Campionato), 2)
                                        dr.Item("Degrado") = Decimal.Round(Convert.ToDecimal(objRigheConfer.Degrado) / 100 * Convert.ToDecimal(objRigheCampionamento.Percentuale_Campionato), 3)
                                        dr.Item("KgPerCalibro") = Decimal.Round(Convert.ToDecimal(objRigheCampionamento.KgPerCalibro), 5)
                                        dr.Item("TaraKgPerCalibro") = Decimal.Round(Convert.ToDecimal(objRigheCampionamento.TaraKgPerCalibro), 5)
                                        DtValorizzazione.Rows.Add(dr)
                                    Next
                                End If
                            End If
                        End If

                        ' Listino normale
                        If lstTotTrovati = 1 Then
                            If lstCount = 1 Then
                                Dim lst = Listini_Prezzi.First()
                                listinoCod = lst.Listino_Cod
                                prezzo_su_conferito = lst.prezzo_su_conferito
                                id_testata_griglia_listino = lst.Id_TestataGriglia_Prod
                                prezzo = lst.prezzo
                            Else
                                Dim lst = Listini_Prezzi_Equivalenti.First()
                                listinoCod = lst.Listino_Cod
                                prezzo_su_conferito = lst.prezzo_su_conferito
                                id_testata_griglia_listino = lst.Id_TestataGriglia_Prod
                                prezzo = lst.prezzo
                            End If
                        End If
                        '''End If
                    End If

                    '*********************************************************************************************************************************************************
                    '***  N.B.   l'oggetto creato sopra in caso di prezzo per conferimento deve essere identico a quello creato sotto in caso di prezzo per campionamento  ***
                    '*********************************************************************************************************************************************************
                    If prezzo <> 0 AndAlso prezzo_su_conferito Then

                        dr = DtValorizzazione.NewRow
                        ImpostaDataRowValorizzazioneMovimenti(dr, objRigheConfer, Id_Testata_Griglia_Trovata, listFattoreVar_ParamQual_Grouped, objRigheConfer.Id_Mov_Det, "prezzo_su_conferito", dataRiferimentoPrezzi, tipoDataRiferimentoPrezzi, "")

                        'dr.Item("QtaCampionata") = Decimal.Parse(objRigheConfer.QtaCampionata)
                        'dr.Item("TaraCampionata") = Decimal.Parse(objRigheConfer.Tara) * Decimal.Parse(objRigheConfer.QtaCampionata) / Decimal.Parse(objRigheConfer.Qta_Extra_Totale)
                        dr.Item("QtaCampionata") = 0
                        dr.Item("TaraCampionata") = dr.Item("Tara")
                        dr.Item("KgPerCalibro") = dr.Item("Qta_Extra_Totale")
                        dr.Item("TaraKgPerCalibro") = dr.Item("Tara")
                        dr.Item("Prezzo") = prezzo
                        dr.Item("TotalePrezzo") = prezzo

                        'L'eventuale prezzo già presente sulla riga prevale su quello calcolato da liquidazione
                        If objRigheConfer.Prezzo_Unitario_Netto_Mov_Det <> 0 Then
                            dr.Item("Prezzo_da_riga_conferimento") = 1
                        Else
                            'Altrimenti calcolo gli eventuali prezzi per fattore variazione
                            dr.Item("PrezzoFattoriVariazione") = 0.0

                            'Considero eventuale variazione da formula fissa
                            If FattVarFormulaFissaPerc <> 0 Then
                                Dim FattVarFormulaFissa = prezzo / 100 * FattVarFormulaFissaPerc

                                ''''''''''dr.Item(objParamQual.Tabella_Cod_Des & "_VariazionePrezzo") = FattVarFormulaFissa

                                ' Aggiungo la variazione al totale prezzo
                                dr.Item("PrezzoFattoriVariazione") = dr.Item("PrezzoFattoriVariazione") + FattVarFormulaFissa
                                dr.Item("TotalePrezzo") = dr.Item("TotalePrezzo") + FattVarFormulaFissa
                            End If

                            Dim listOggettiParametriQualitativi = ValorizzaFattoriDiVariazione(piva, objRigheConfer.Cal_Cod, listinoCod,
                                                                         id_testata_griglia_listino,
                                                                         dataRiferimentoPrezzi, objParametri)

                            For Each objParamQual In listOggettiParametriQualitativi


                                Dim Id_listino_fattore_variazione As Integer = objParamQual.Id_listino_fattore_variaz

                                Dim Esclusione = (From esc In GiasContext.Listini_CampionamentoConferito_Esclusione_Fattore_Variazione
                                                  Where esc.Piva_SuperUser = Piva_SuperUser AndAlso
                                                        esc.PIVA = piva AndAlso
                                                        esc.Id_listino_fattore_variaz = Id_listino_fattore_variazione AndAlso
                                                        esc.Id_Calibro = 0).FirstOrDefault()

                                If Esclusione Is Nothing Then

                                    'Cerco descrizione del fattore di variazione
                                    dr.Item(objParamQual.Tabella_Cod_Des & "_Codice") = objParamQual.val_cod
                                    dr.Item(objParamQual.Tabella_Cod_Des & "_Sigla") = objParamQual.val_sigla
                                    dr.Item(objParamQual.Tabella_Cod_Des & "_Descrizione") = objParamQual.val_des

                                    Dim FattVar As Decimal = 0.0
                                    If objParamQual.variazione_a_valore Then
                                        ' a valore
                                        FattVar = Decimal.Parse(objParamQual.valore_al_kg)
                                    Else
                                        ' a %
                                        FattVar = prezzo / 100 * Decimal.Parse(objParamQual.valore_al_kg)
                                    End If

                                    dr.Item(objParamQual.Tabella_Cod_Des & "_VariazionePrezzo") = FattVar

                                    ' Aggiungo la variazione al totale prezzo
                                    dr.Item("PrezzoFattoriVariazione") = dr.Item("PrezzoFattoriVariazione") + FattVar
                                    dr.Item("TotalePrezzo") = dr.Item("TotalePrezzo") + FattVar
                                End If
                            Next
                        End If

                        dr.Item("Importo") = Decimal.Round(dr.Item("TotalePrezzo") * dr.Item("Qta_Extra_Totale"), 2)
                        DtValorizzazione.Rows.Add(dr)

                    End If

                    '################################
                    '###   Prezzo su campionato   ###
                    '################################

                    If lstTotTrovati = 1 AndAlso Not prezzo_su_conferito Then

                        '---------------------------------------------------------------------------------------------------------
                        ' Non ho trovato testata di campionamento, quindi non ci sono righe e non ci sono fattori di variazione
                        ' Imposto il prezzo ed i totali a zero
                        '---------------------------------------------------------------------------------------------------------
                        If Not objRigheConfer.TrovataTestataCampionamento Then
                            'Creo una nuova riga - Campionamento non trovato
                            dr = DtValorizzazione.NewRow
                            ImpostaDataRowValorizzazioneMovimenti(dr, objRigheConfer, Id_Testata_Griglia_Trovata, listFattoreVar_ParamQual_Grouped, objRigheConfer.Id_Mov_Det, "prezzo_su_campionato", dataRiferimentoPrezzi, tipoDataRiferimentoPrezzi, "Campionamento non trovato per questa riga: prezzo non determinabile")
                            DtValorizzazione.Rows.Add(dr)
                        End If


                        '-------------------------------------
                        ' Ho trovato testata di campionamento
                        '-------------------------------------
                        If objRigheConfer.TrovataTestataCampionamento Then

                            Dim FattVar As Decimal = 0.0
                            Dim Qta_Extra_TotaleDecimal = Decimal.Parse(objRigheConfer.Qta_Extra_Totale)
                            Dim QtaCampionataDecimal = Decimal.Parse(objRigheConfer.QtaCampionata)
                            Dim TaraTotaleDecimal = Decimal.Parse(objRigheConfer.Tara)

                            Dim mess As String = ""
                            Dim Campionamenti_Conferimento_Righe = Leggi_Righe_Campionamento_Per_Valorizzazione(piva, objRigheConfer.Id_Mov_Det,
                                    Id_Testata_Griglia_Trovata, Id_Testata_Griglia_Prod_Trovata, Qta_Extra_TotaleDecimal, QtaCampionataDecimal, TaraTotaleDecimal, mess, objParametri)

                            'Non ho trovato righe di campionamento ma c'era la testata per prodotto con prezzo su campionato 
                            ' Succede se cambiano prodotto o caratteristiche sulla riga successivamente al campionamento
                            If Campionamenti_Conferimento_Righe.Count() = 0 AndAlso objRigheConfer.TrovataTestataCampionamento Then

                                'Creo una nuova riga
                                dr = DtValorizzazione.NewRow
                                ImpostaDataRowValorizzazioneMovimenti(dr, objRigheConfer, Id_Testata_Griglia_Trovata, listFattoreVar_ParamQual_Grouped, objRigheConfer.Id_Mov_Det, "prezzo_su_campionato", dataRiferimentoPrezzi, tipoDataRiferimentoPrezzi, "Prodotto o caratteristiche modificate rispetto a quelle con cui è avvenuto il campionamento. I dati attuali riportano ad una griglia di camp. diversa: cancellare il campionamento e reinserirlo")
                                DtValorizzazione.Rows.Add(dr)

                            Else

                                Dim listOggettiParametriQualitativi = ValorizzaFattoriDiVariazione(piva, objRigheConfer.Cal_Cod, listinoCod,
                                                                     id_testata_griglia_listino,
                                                                                           dataRiferimentoPrezzi, objParametri)

                                ' Ho trovato tutte le righe di campionamento ... cerco il prezzo per ognuna se ho trovato la testata listino
                                '''''For Each objRigheCampionamento In Campionamenti_Conferimento_Righe.ToList()
                                For Each objRigheCampionamento In Campionamenti_Conferimento_Righe
                                    Dim calibroLetto As Integer = objRigheCampionamento.Id_Calibro

                                    Dim KeyRigaConferimentoECalibro As String = Id_Mov_Det_Str & "-" & objRigheCampionamento.Id_Calibro.ToString()
                                    'Creo una nuova riga
                                    dr = DtValorizzazione.NewRow
                                    ImpostaDataRowValorizzazioneMovimenti(dr, objRigheConfer, Id_Testata_Griglia_Trovata, listFattoreVar_ParamQual_Grouped, KeyRigaConferimentoECalibro, "prezzo_su_campionato", dataRiferimentoPrezzi, tipoDataRiferimentoPrezzi, mess)
                                    dr.Item("Id_Calibro") = objRigheCampionamento.Id_Calibro
                                    dr.Item("Descr_Qualita_Camp") = objRigheCampionamento.Descr_Qualita_Camp
                                    dr.Item("Descr_Calibro_Camp") = objRigheCampionamento.Descr_Calibro_Camp
                                    dr.Item("Descr_QualCalibro_Camp") = objRigheCampionamento.Descr_QualCalibro_Camp
                                    dr.Item("Descr_OrdinQualCalibro_Camp") = objRigheCampionamento.Descr_OrdinQualCalibro_Camp
                                    dr.Item("Ordinamento_Calibro") = objRigheCampionamento.Ordinamento
                                    dr.Item("QtaCampionata") = Decimal.Round(Convert.ToDecimal(objRigheCampionamento.QtaCampionata), 5)
                                    dr.Item("TaraCampionata") = Decimal.Round(Convert.ToDecimal(objRigheCampionamento.TaraCampionata), 5)
                                    dr.Item("Percentuale_Campionato") = Decimal.Round(Convert.ToDecimal(objRigheCampionamento.Percentuale_Campionato), 2)
                                    dr.Item("Degrado") = Decimal.Round(Convert.ToDecimal(objRigheConfer.Degrado) / 100 * Convert.ToDecimal(objRigheCampionamento.Percentuale_Campionato), 3)
                                    dr.Item("KgPerCalibro") = Decimal.Round(Convert.ToDecimal(objRigheCampionamento.KgPerCalibro), 5)
                                    dr.Item("TaraKgPerCalibro") = Decimal.Round(Convert.ToDecimal(objRigheCampionamento.TaraKgPerCalibro), 5)

                                    Dim IdCal As Integer = objRigheCampionamento.Id_Calibro

                                    If listinoCod <> 0 Then
                                        If objRigheCampionamento.Id_Calibro > 0 And objRigheCampionamento.Percentuale_Campionato > 0 Then
                                            Dim Prezzi_Riga =
                                    From listini_dettaglio In GiasContext.Listini_CampionamentoConferito_Dettagli
                                    Where (listini_dettaglio.Piva_SuperUser = Piva_SuperUser AndAlso
                                            listini_dettaglio.PIVA = piva AndAlso
                                            listini_dettaglio.Listino_Cod = listinoCod AndAlso
                                            listini_dettaglio.Id_TestataGriglia_Prod = id_testata_griglia_listino AndAlso
                                            listini_dettaglio.Id_Calibro = IdCal AndAlso
                                                (listini_dettaglio.Validita_Inizio <= dataRiferimentoPrezzi AndAlso
                                                 listini_dettaglio.Validita_Fine >= dataRiferimentoPrezzi))
                                            Dim przCount As Integer = Prezzi_Riga.Count()
                                            If przCount = 0 Then
                                                dr.Item("Messaggi") = "Prezzo non trovato"
                                                dr.Item("Prezzo") = 0.0
                                                dr.Item("PrezzoFattoriVariazione") = 0.0
                                                dr.Item("TotalePrezzo") = 0.0
                                            End If
                                            If przCount > 1 Then
                                                dr.Item("Messaggi") = "Trovate " & przCount.ToString & " righe di listino attive contemporaneamente: segnalare all'assistenza"
                                                dr.Item("Prezzo") = 0.0
                                                dr.Item("PrezzoFattoriVariazione") = 0.0
                                                dr.Item("TotalePrezzo") = 0.0
                                            End If
                                            If przCount = 1 Then
                                                prezzo = Prezzi_Riga.First().Prezzo
                                                dr.Item("Prezzo") = prezzo
                                                dr.Item("PrezzoFattoriVariazione") = 0.0
                                                dr.Item("TotalePrezzo") = prezzo
                                            End If
                                        End If

                                        'Considero eventuale variazione da formula fissa
                                        If FattVarFormulaFissaPerc <> 0 Then
                                            Dim FattVarFormulaFissa = prezzo / 100 * FattVarFormulaFissaPerc

                                            ''''''''''dr.Item(objParamQual.Tabella_Cod_Des & "_VariazionePrezzo") = FattVarFormulaFissa

                                            ' Aggiungo la variazione al totale prezzo
                                            dr.Item("PrezzoFattoriVariazione") = dr.Item("PrezzoFattoriVariazione") + FattVarFormulaFissa
                                            dr.Item("TotalePrezzo") = dr.Item("TotalePrezzo") + FattVarFormulaFissa
                                        End If

                                        For Each objParamQual In listOggettiParametriQualitativi

                                            Dim Id_listino_fattore_variazione As Integer = objParamQual.Id_listino_fattore_variaz
                                            Dim Esclusione = (From esc In GiasContext.Listini_CampionamentoConferito_Esclusione_Fattore_Variazione
                                                              Where esc.Piva_SuperUser = Piva_SuperUser AndAlso
                                                                esc.PIVA = piva AndAlso
                                                                esc.Id_listino_fattore_variaz = Id_listino_fattore_variazione AndAlso
                                                                esc.Id_Calibro = calibroLetto).FirstOrDefault()

                                            If Esclusione Is Nothing Then

                                                'Cerco descrizione del fattore di variazione
                                                dr.Item(objParamQual.Tabella_Cod_Des & "_Codice") = objParamQual.val_cod
                                                dr.Item(objParamQual.Tabella_Cod_Des & "_Sigla") = objParamQual.val_sigla
                                                dr.Item(objParamQual.Tabella_Cod_Des & "_Descrizione") = objParamQual.val_des

                                                If objParamQual.variazione_a_valore Then
                                                    ' a valore
                                                    FattVar = Decimal.Parse(objParamQual.valore_al_kg)
                                                Else
                                                    ' a %
                                                    FattVar = prezzo / 100 * Decimal.Parse(objParamQual.valore_al_kg)
                                                End If

                                                dr.Item(objParamQual.Tabella_Cod_Des & "_VariazionePrezzo") = FattVar

                                                ' Aggiungo la variazione al totale prezzo
                                                dr.Item("PrezzoFattoriVariazione") = dr.Item("PrezzoFattoriVariazione") + FattVar
                                                dr.Item("TotalePrezzo") = dr.Item("TotalePrezzo") + FattVar

                                            End If
                                        Next
                                    End If

                                    dr.Item("Importo") = Decimal.Round(dr.Item("TotalePrezzo") * dr.Item("KgPerCalibro"), 2)
                                    DtValorizzazione.Rows.Add(dr)

                                Next
                            End If
                        End If
                    End If

                End If
            Next


            If (Not IsAccontoLiquidazione) Then

                'creo la lista delle colonne da visualizzare
                Dim l As New List(Of ColonneNome)
                Dim c As ColonneNome

                c = New ColonneNome("KeyRigaConferimentoECalibro", "KeyRigaConferimentoECalibro", "string") With {
                    ._hidden = True
                }
                l.Add(c)

                c = New ColonneNome("Id_Mov_Det", "Id_Mov_Det", "number") With {
                    ._hidden = True
                }
                l.Add(c)

                c = New ColonneNome("Id_Agenda", "Id_Agenda", "number") With {
                    ._hidden = True
                }
                l.Add(c)

                c = New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {
                    ._hidden = True
                }
                l.Add(c)

                c = New ColonneNome("Cal_Cod", "Cal_Cod", "number") With {
                    ._hidden = True
                }
                l.Add(c)

                c = New ColonneNome("Mat_Cod", "Mat_Cod", "number") With {
                    ._hidden = True
                }
                l.Add(c)

                c = New ColonneNome("Id_TestataGriglia", "Id_TestataGriglia", "number") With {
                    ._hidden = True
                }
                l.Add(c)

                c = New ColonneNome("Qta_Extra_Totale", "Qta_Extra_Totale", "number") With {
                    ._hidden = True
                }
                l.Add(c)

                c = New ColonneNome("Tara", "Tara", "number") With {
                    ._hidden = True
                }
                l.Add(c)

                c = New ColonneNome("Udm_QtaCampionata", "Udm_QtaCampionata", "number") With {
                    ._hidden = True
                }
                l.Add(c)

                c = New ColonneNome("Id_Calibro", "Id_Calibro", "number") With {
                    ._hidden = True
                }
                l.Add(c)

                c = New ColonneNome("Cod_RisUm", "Cod_RisUm", "number") With {
                    ._hidden = True
                }
                l.Add(c)

                c = New ColonneNome("Rag_Soc", "Ragione sociale", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("ConferimentoOAcquisto", "Acq. / Confer.", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                'c = New ColonneNome("Doc_Numero_Completo", "Nr. Doc.", "string")
                c = New ColonneNome("Doc_Numero_Visualizzato", "Nr. Doc.", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("DDT_Completo", "Nr. DDT", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("NrRiga", "Riga", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("Data_Movimento", "Data movimento", "date") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._formatNr = "{0:dd/MM/yyyy}"
                }
                l.Add(c)

                c = New ColonneNome("Data_Riferimento_Prezzi", "Data rifer. prezzi", "date") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._formatNr = "{0:dd/MM/yyyy}"
                }
                l.Add(c)

                c = New ColonneNome("Grp_Fatt_Descr", "Gruppo Fatt.", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("Mat_Des", "Prodotto", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("Calibro_Entrata_Sigla", "Calibro", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("Qual_Sigla", "Qualità", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("Certif_Sigla", "Certific.", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("Rugginosita_Descr", "Rugginosità", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("Descr_OrdinQualCalibro_Camp", "Campionatura", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("Prezzo_da_riga_conferimento", "Prezzo forzato in riga", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._FormatoParticolare = "#=(Prezzo_da_riga_conferimento === 1) ? 'Si' : 'No'#",
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("Prezzo", "Prezzo", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._formatNr = "n6"
                }
                l.Add(c)

                c = New ColonneNome("PrezzoFattoriVariazione", "Prezzo fattori variazione", "number") With {
                    ._hidden = True
                }
                l.Add(c)

                For Each param In FattoreVar_ParamQual_Grouped
                    c = New ColonneNome(param.Tabella_Cod_Des & "_Codice", param.Tabella_Cod_Des & "_Codice", "number") With {
                        ._hidden = True
                    }
                    l.Add(c)

                    c = New ColonneNome(param.Tabella_Cod_Des & "_VariazionePrezzo", "Variazione " & param.Tabella_Des, "number") With {
                        ._Editabile = False,
                        ._Filtrabile = True,
                        ._Display = True,
                        ._formatNr = "n8"
                    }
                    l.Add(c)
                Next

                c = New ColonneNome("TotalePrezzo", "Prezzo totale", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._formatNr = "n8"
                }
                l.Add(c)

                c = New ColonneNome("DegradoPerc", "% Degrado", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._sum = False,
                    ._formatNr = "n2"
                }
                l.Add(c)

                c = New ColonneNome("Degrado", "Degrado", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._sum = True,
                    ._formatNr = "n0"
                }
                l.Add(c)

                c = New ColonneNome("KgPerCalibro", "Kg totali", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._sum = True,
                    ._formatNr = "n0"
                }
                l.Add(c)

                c = New ColonneNome("TaraKgPerCalibro", "Tara totale", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._sum = True,
                    ._formatNr = "n5"
                }
                l.Add(c)

                c = New ColonneNome("Importo", "Importo", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._sum = True,
                    ._formatNr = "n2"
                }
                l.Add(c)

                c = New ColonneNome("Percentuale_Campionato", "% campione", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._formatNr = "n5"
                }
                l.Add(c)

                c = New ColonneNome("QtaCampionata", "Kg campione", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._sum = True,
                    ._formatNr = "n5"
                }
                l.Add(c)

                c = New ColonneNome("TaraCampionata", "Tara campione", "number") With {
                    ._hidden = True
                }
                l.Add(c)

                c = New ColonneNome("Note", "Note", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("StatoCampionamento", "Stato Campion.", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("Automatico", "Tipo Campionamento", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("Messaggi", "Messaggi", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
                }
                l.Add(c)

                c = New ColonneNome("TrasportoACura", "TrasportoACura", "string") With {
                    ._hidden = True
                }
                l.Add(c)

                Dim js As New JSON_DataTable With {.Editabile_Deafault = False}
                risposta = js.JSON_DataTable_Kendo(DtValorizzazione, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto)

            End If

        End Using

        Return risposta

    End Function

    Public Function Leggi_Righe_Campionamento_Per_Valorizzazione(ByVal piva As String,
                                                                 ByVal id_mov_det As Integer,
                                                                 ByVal Id_Testata_Griglia_Trovata As Integer,
                                                                 ByVal id_Testata_Griglia_Prod_Trovata As Integer,
                                                                 ByVal qta_Extra_Totale As Decimal,
                                                                 ByVal qtaCampionata As Decimal,
                                                                 ByVal tara As Decimal,
                                                                 ByRef mess As String,
                                                                 ByRef objParametri As AgronicaCoreParametri
                                                                 ) As IEnumerable(Of Object)

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Righe_Campionamento_Per_Valorizzazione()"
        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TaraCampionataDecimal = tara * qtaCampionata / qta_Extra_Totale

            ' Cerco le righe relative alla testata di campionamento
            Dim Campionamenti_Conferimento_Righe = (
            From camp_conf_righe In GiasContext.CampionamentoConferito_Movimenti_Righe
            Join prodotti In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                                On
                                    camp_conf_righe.Piva_SuperUser Equals prodotti.Piva_SuperUser And
                                    camp_conf_righe.PIVA Equals prodotti.PIVA And
                                    camp_conf_righe.Id_TestataGriglia_Prod Equals prodotti.Id_TestataGriglia_Prod
            Join calibri In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
                                On
                                    camp_conf_righe.Piva_SuperUser Equals calibri.Piva_SuperUser And
                                    camp_conf_righe.PIVA Equals calibri.PIVA And
                                    camp_conf_righe.Id_Calibro Equals calibri.Id_Calibro And
                                    prodotti.Id_TestataGriglia Equals calibri.Id_TestataGriglia
            Where
                                camp_conf_righe.Piva_SuperUser = Piva_SuperUser AndAlso
                                    camp_conf_righe.PIVA = piva AndAlso
                                    camp_conf_righe.Id_Mov_Det = id_mov_det AndAlso
                                    camp_conf_righe.Id_TestataGriglia_Prod = id_Testata_Griglia_Prod_Trovata
            Order By calibri.Ordinamento
            Select New With
                                {
                                    .Id_Calibro = calibri.Id_Calibro,
                                    .Ordinamento = calibri.Ordinamento,
                                    .Id_TestataGriglia_Prod = camp_conf_righe.Id_TestataGriglia_Prod,
                                    .Descr_Qualita_Camp = If(calibri.Descr_qualita, ""),
                                    .Descr_Calibro_Camp = calibri.Descr_calibro,
                                    .Descr_QualCalibro_Camp = If(calibri.Descr_qualita Is Nothing, calibri.Descr_calibro, calibri.Descr_qualita & " " & calibri.Descr_calibro),
                                    .Descr_OrdinQualCalibro_Camp = If(calibri.Descr_qualita Is Nothing, calibri.Descr_calibro, calibri.Descr_qualita & " " & calibri.Descr_calibro),
                                    .QtaCampionata = qtaCampionata / 100 * camp_conf_righe.PercentualeCampionato,
                                    .TaraCampionata = TaraCampionataDecimal / 100 * camp_conf_righe.PercentualeCampionato,
                                    .Percentuale_Campionato = camp_conf_righe.PercentualeCampionato,
                                    .KgPerCalibro = qta_Extra_Totale / 100 * camp_conf_righe.PercentualeCampionato,
                                    .TaraKgPerCalibro = tara / 100 * camp_conf_righe.PercentualeCampionato
                                 })


            Dim myList = Campionamenti_Conferimento_Righe.ToList()

            ' Gestione necessità segnalata il 9/4/2018 da Cofruta ma successivamente rientrata almeno per la stagione corrente: è stata fatta
            ' un'unica griglia per un prodotto / qualità ma successivamente al primo acconto vogliono gestire prezzi diversi per un calibro di entrata
            ' specifico, fermo che il campionamento e la relativa griglia rimangono quelle.
            ' Non era possibile gestire fattore variazione perché la differenza rispetto al prezzo base non è uguale da campione a campione.
            ' Allo stato attuale si poteva già inserire prodotti / qualità / calibro diversi sulla stessa griglia e differenziare i prezzi specifici, però 
            ' i campionamenti e quindi i record della CampionamentoConferito_Movimenti avevano come id_Testata_Griglia_Prod_Trovata quella originale 
            ' del campionamento e quindi questa funzione restituiva 0 righe.
            ' Volendo evitare di fare cancellare / reinserire ogni campionamento, tanto più che se sono già stati fatti
            ' acconti non è il massimo, faccio un secondo tentativo con tutte le altre combinazioni a parità di testata griglia di campionamento.  
            ' Deve funzionare perché tanto tutti questi hanno comunque la stessa griglia calibri

            If myList.Count = 0 Then

                'Cerco tutti le righe di CampionamentoConferito_TestataGriglia_Prodotti con stessa testata
                Dim test_gr_prod =
                   (From testata_griglia_prodotti In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                    Where
                           testata_griglia_prodotti.Id_TestataGriglia_Prod <> id_Testata_Griglia_Prod_Trovata AndAlso
                           testata_griglia_prodotti.Id_TestataGriglia = Id_Testata_Griglia_Trovata AndAlso
                           testata_griglia_prodotti.PIVA.Equals(piva))

                Dim idS As New List(Of Integer)
                For Each p In test_gr_prod.ToList()
                    idS.Add(p.Id_TestataGriglia_Prod)
                Next

                ' Cerco le righe relative alla testata di campionamento
                Campionamenti_Conferimento_Righe = (
                    From camp_conf_righe In GiasContext.CampionamentoConferito_Movimenti_Righe
                    Join prodotti In GiasContext.CampionamentoConferito_TestataGriglia_Prodotti
                                On
                                    camp_conf_righe.Piva_SuperUser Equals prodotti.Piva_SuperUser And
                                    camp_conf_righe.PIVA Equals prodotti.PIVA And
                                    camp_conf_righe.Id_TestataGriglia_Prod Equals prodotti.Id_TestataGriglia_Prod
                    Join calibri In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
                                On
                                    camp_conf_righe.Piva_SuperUser Equals calibri.Piva_SuperUser And
                                    camp_conf_righe.PIVA Equals calibri.PIVA And
                                    camp_conf_righe.Id_Calibro Equals calibri.Id_Calibro And
                                    prodotti.Id_TestataGriglia Equals calibri.Id_TestataGriglia
                    Where
                                camp_conf_righe.Piva_SuperUser = Piva_SuperUser AndAlso
                                    camp_conf_righe.PIVA = piva AndAlso
                                    camp_conf_righe.Id_Mov_Det = id_mov_det AndAlso
                                    idS.Contains(camp_conf_righe.Id_TestataGriglia_Prod)
                    Order By calibri.Ordinamento
                    Select New With
                                {
                                    .Id_Calibro = calibri.Id_Calibro,
                                    .Ordinamento = calibri.Ordinamento,
                                    .Id_TestataGriglia_Prod = camp_conf_righe.Id_TestataGriglia_Prod,
                                    .Descr_Qualita_Camp = If(calibri.Descr_qualita, ""),
                                    .Descr_Calibro_Camp = calibri.Descr_calibro,
                                    .Descr_QualCalibro_Camp = If(calibri.Descr_qualita Is Nothing, calibri.Descr_calibro, calibri.Descr_qualita & " " & calibri.Descr_calibro),
                                    .Descr_OrdinQualCalibro_Camp = If(calibri.Descr_qualita Is Nothing, calibri.Descr_calibro, calibri.Descr_qualita & " " & calibri.Descr_calibro),
                                    .QtaCampionata = qtaCampionata / 100 * camp_conf_righe.PercentualeCampionato,
                                    .TaraCampionata = TaraCampionataDecimal / 100 * camp_conf_righe.PercentualeCampionato,
                                    .Percentuale_Campionato = camp_conf_righe.PercentualeCampionato,
                                    .KgPerCalibro = qta_Extra_Totale / 100 * camp_conf_righe.PercentualeCampionato,
                                    .TaraKgPerCalibro = tara / 100 * camp_conf_righe.PercentualeCampionato
                                 })

                myList = Campionamenti_Conferimento_Righe.ToList()

                If myList.Count > 0 Then

                    Dim objList As Object = myList(0)

                    'Segnalo comunque la situazione
                    Dim ar1 As JArray = Nothing
                    Dim ar2 As JArray = Nothing
                    Dim leggi As New FF_CampionamentoConferimento_R

                    ar1 = JArray.Parse(leggi.Leggi_TestataGriglia_Prodotti(piva, Id_Testata_Griglia_Trovata, Integer.Parse(objList.Id_testataGriglia_Prod), "", objParametri))
                    ar2 = JArray.Parse(leggi.Leggi_TestataGriglia_Prodotti(piva, Id_Testata_Griglia_Trovata, id_Testata_Griglia_Prod_Trovata, "", objParametri))
                    Dim o1 As JObject
                    Dim s1 As String = ""
                    If ar1.Count = 1 Then
                        o1 = ar1(0)
                        s1 += o1("Mat_Des").ToString
                        If Not String.IsNullOrEmpty(o1("qualita_des")) Then
                            s1 += " - "
                            s1 += o1("qualita_des").ToString
                        End If
                        If Not String.IsNullOrEmpty(o1("calibro_des")) Then
                            s1 += " - "
                            s1 += o1("calibro_des").ToString
                        End If
                    End If
                    Dim o2 As JObject
                    Dim s2 As String = ""
                    If ar2.Count = 1 Then
                        o2 = ar2(0)
                        s2 += o2("Mat_Des").ToString
                        If Not String.IsNullOrEmpty(o2("qualita_des")) Then
                            s2 += " - "
                            s2 += o2("qualita_des").ToString
                        End If
                        If Not String.IsNullOrEmpty(o2("calibro_des")) Then
                            s2 += " - "
                            s2 += o2("calibro_des").ToString
                        End If
                    End If

                    Dim s As String = ""
                    s += "Il campionamento era stato inserito collegato a "
                    s += s1
                    s += " mentre il prezzo è riferito a "
                    s += s2
                    s += ". La griglia di campionamento è comunque la stessa."

                    mess = s
                End If
            End If

            ' Necessario perché più di 4 stringhe non riesce a concatenarle e qui ce ne sono 7 (spazi intermedi compresi)
            ' 10/3/2020 Aggiunta sfrido decimali all'elemento con più peso
            Dim residuoQtaCampionata = qtaCampionata
            Dim residuoTaraCampionata = TaraCampionataDecimal
            Dim residuoKgPerCalibro = qta_Extra_Totale
            Dim residuoTaraKgPerCalibro = tara
            Dim pesoMaggiore As Decimal = 0
            For Each obj In myList
                obj.QtaCampionata = Decimal.Round(CDec(obj.QtaCampionata), 3)
                obj.TaraCampionata = Decimal.Round(CDec(obj.TaraCampionata), 3)
                obj.KgPerCalibro = Decimal.Round(CDec(obj.KgPerCalibro), 3)
                obj.TaraKgPerCalibro = Decimal.Round(CDec(obj.TaraKgPerCalibro), 3)

                If CDec(obj.KgPerCalibro) > pesoMaggiore Then
                    pesoMaggiore = CDec(obj.KgPerCalibro)
                End If
                residuoQtaCampionata -= CDec(obj.QtaCampionata)
                residuoTaraCampionata -= CDec(obj.TaraCampionata)
                residuoKgPerCalibro -= CDec(obj.KgPerCalibro)
                residuoTaraKgPerCalibro -= CDec(obj.TaraKgPerCalibro)
                If obj.Descr_QualCalibro_Camp <> "" Then
                    'TODO STEFANO TEMPORANEO
                    obj.Descr_QualCalibro_Camp = obj.Descr_QualCalibro_Camp.Replace("UALITA", "")
                End If
                If obj.Descr_OrdinQualCalibro_Camp <> "" Then
                    obj.Descr_OrdinQualCalibro_Camp = CStr(obj.Ordinamento).PadLeft(3, "0") & " " & obj.Descr_OrdinQualCalibro_Camp
                End If
            Next

            For Each obj In myList
                If CDec(obj.KgPerCalibro) = pesoMaggiore Then
                    If Decimal.Round(residuoQtaCampionata, 3) <> 0 Then
                        obj.QtaCampionata = CDec(obj.QtaCampionata) + Decimal.Round(residuoQtaCampionata, 3)
                    End If
                    If Decimal.Round(residuoTaraCampionata, 3) <> 0 Then
                        obj.TaraCampionata = CDec(obj.TaraCampionata) + Decimal.Round(residuoTaraCampionata, 3)
                    End If
                    If Decimal.Round(residuoKgPerCalibro, 3) <> 0 Then
                        obj.KgPerCalibro = CDec(obj.KgPerCalibro) + Decimal.Round(residuoKgPerCalibro, 3)
                    End If
                    If Decimal.Round(residuoTaraKgPerCalibro, 3) <> 0 Then
                        obj.TaraKgPerCalibro = CDec(obj.TaraKgPerCalibro) + Decimal.Round(residuoTaraKgPerCalibro, 3)
                    End If
                End If
            Next

            Return myList

        End Using

        Return Nothing

    End Function

    Public Function Leggi_Fattori_Variazione_Raggruppati(ByVal piva As String,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         ) As IEnumerable(Of Object)

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Fattori_Variazione_Raggruppati()"
        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            ' Cerco tutti i fattori di variazione attivi
            Dim fattoreVariazione_paramqual As DbSet(Of CampionamentoConferito_Fattori_Variazione_ParamQualitativi) = GiasContext.CampionamentoConferito_Fattori_Variazione_ParamQualitativi
            Dim FattoreVar_ParamQual_Grouped = (
                                    From fv In fattoreVariazione_paramqual
                                    Join otab In GiasContext.OTabelle On
                                        otab.Tabella_Cod Equals fv.Tabella_Cod
                                    Where
                                        fv.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                                        fv.PIVA.Equals(piva)
                                    Group By id_fatt_var = fv.Id_fattore_variazione, tabcod = fv.Tabella_Cod, tabdes = otab.Tabella_Des, tabcoddes = otab.Tabella_Cod_Des
                                    Into g = Group
                                    Select New With {
                                        .Id_fatt_var = id_fatt_var,
                                        .Tabella_Cod = tabcod,
                                        .Tabella_Des = tabdes,
                                        .Tabella_Cod_Des = tabcoddes
                                        }).ToList()

            Return FattoreVar_ParamQual_Grouped

        End Using

        Return Nothing

    End Function

    Public Function ImpostaDataRowValorizzazioneMovimenti(ByRef dr As DataRow,
                                                          ByVal objRigheConfer As Object,
                                                          ByVal Id_Testata_Griglia As Integer,
                                                          ByVal FattoreVar_ParamQual_Grouped As List(Of Object),
                                                          ByVal KeyRigaConferimentoECalibro As String,
                                                          ByVal TipoPrezzo As String,
                                                          ByVal DataRiferimentoPrezzi As Date,
                                                          ByVal TipoDataRiferimentoPrezzi As String,
                                                          ByVal Messaggio As String
                                                          ) As String

        dr.Item("KeyRigaConferimentoECalibro") = KeyRigaConferimentoECalibro
        dr.Item("Id_Mov_Det") = objRigheConfer.Id_Mov_Det
        dr.Item("Id_Agenda") = objRigheConfer.Id_Agenda
        dr.Item("Lav_Cod") = objRigheConfer.Lav_Cod
        dr.Item("Cal_Cod") = objRigheConfer.Cal_Cod
        dr.Item("Data_Movimento") = objRigheConfer.Data_Movimento
        dr.Item("Data_Riferimento_Prezzi") = DataRiferimentoPrezzi
        dr.Item("Tipo_Data_Riferimento_Prezzi") = TipoDataRiferimentoPrezzi
        dr.Item("Cod_RisUm") = objRigheConfer.Cod_RisUm
        dr.Item("Rag_Soc") = objRigheConfer.Rag_Soc
        dr.Item("ConferimentoOAcquisto") = objRigheConfer.ConferimentoOAcquisto
        dr.Item("TipoPrezzo") = TipoPrezzo
        dr.Item("Doc_Numero_Completo") = objRigheConfer.Doc_Numero_Completo
        dr.Item("Doc_Numero_Sin") = objRigheConfer.Doc_Numero_Sin
        dr.Item("Doc_Numero") = objRigheConfer.Doc_Numero
        dr.Item("Doc_Numero_Des") = objRigheConfer.Doc_Numero_Des
        dr.Item("DDT_Completo") = objRigheConfer.DDT_Completo
        dr.Item("Doc_Numero_Visualizzato") = objRigheConfer.Doc_Numero_Visualizzato
        dr.Item("NrRiga") = objRigheConfer.NrRiga
        dr.Item("Veg_Cod") = objRigheConfer.Veg_Cod
        dr.Item("Cul_Cod") = objRigheConfer.Cul_Cod
        dr.Item("Mat_Cod") = objRigheConfer.Mat_Cod
        dr.Item("Mat_Des") = objRigheConfer.Mat_Des
        dr.Item("Grp_Fatt_Cod") = objRigheConfer.Grp_Fatt_Cod
        dr.Item("Grp_Fatt_Descr") = objRigheConfer.Grp_Fatt_Descr
        dr.Item("Grp_Fatt_Sigla") = objRigheConfer.Grp_Fatt_Sigla
        dr.Item("Qual_Cod") = objRigheConfer.Qual_Cod
        dr.Item("Qual_Descr") = objRigheConfer.Qual_Descr
        dr.Item("Qual_Sigla") = objRigheConfer.Qual_Sigla
        dr.Item("Calibro_Entrata_Cod") = objRigheConfer.Calibro_Entrata_Cod
        dr.Item("Calibro_Entrata_Descr") = objRigheConfer.Calibro_Entrata_Descr
        dr.Item("Calibro_Entrata_Sigla") = objRigheConfer.Calibro_Entrata_Sigla
        dr.Item("Certif_Cod") = objRigheConfer.Certif_Cod
        dr.Item("Certif_Descr") = objRigheConfer.Certif_Descr
        dr.Item("Certif_Sigla") = objRigheConfer.Certif_Sigla
        dr.Item("Rugginosita_Cod") = objRigheConfer.Rugginosita_Cod
        dr.Item("Rugginosita_Descr") = objRigheConfer.Rugginosita_Descr
        dr.Item("Rugginosita_Sigla") = objRigheConfer.Rugginosita_Sigla
        dr.Item("Qta_Extra_Totale") = Decimal.Parse(objRigheConfer.Qta_Extra_Totale)
        dr.Item("Tara") = Decimal.Parse(objRigheConfer.Tara)
        dr.Item("Id_TestataGriglia") = Id_Testata_Griglia
        dr.Item("Udm_QtaCampionata") = objRigheConfer.Udm_QtaCampionata
        dr.Item("QtaCampionata") = 0.0
        dr.Item("TaraCampionata") = 0.0
        dr.Item("Note") = objRigheConfer.Note
        dr.Item("Automatico") = objRigheConfer.Automatico
        dr.Item("StatoCampionamento") = objRigheConfer.StatoCampionamento
        dr.Item("Id_Calibro") = 0
        dr.Item("Descr_Qualita_Camp") = ""
        dr.Item("Descr_Calibro_Camp") = ""
        dr.Item("Descr_QualCalibro_Camp") = ""
        dr.Item("Descr_OrdinQualCalibro_Camp") = ""
        dr.Item("Ordinamento_Calibro") = 0
        dr.Item("Percentuale_Campionato") = 0.0
        dr.Item("DegradoPerc") = Decimal.Parse(objRigheConfer.DegradoPerc)
        dr.Item("Degrado") = Decimal.Parse(objRigheConfer.Degrado)
        If String.IsNullOrEmpty(Messaggio) Then
            dr.Item("KgPerCalibro") = 0.0
            dr.Item("TaraKgPerCalibro") = 0.0
        Else
            dr.Item("KgPerCalibro") = Decimal.Parse(objRigheConfer.Qta_Extra_Totale)
            dr.Item("TaraKgPerCalibro") = Decimal.Parse(objRigheConfer.Tara)
        End If
        dr.Item("Prezzo") = 0.0
        dr.Item("PrezzoFattoriVariazione") = 0.0
        For Each o In FattoreVar_ParamQual_Grouped
            dr.Item(o.Tabella_Cod_Des & "_Codice") = 0
            dr.Item(o.Tabella_Cod_Des & "_Sigla") = ""
            dr.Item(o.Tabella_Cod_Des & "_Descrizione") = ""
            dr.Item(o.Tabella_Cod_Des & "_VariazionePrezzo") = 0.0
        Next
        dr.Item("TotalePrezzo") = 0.0
        dr.Item("Importo") = 0.0
        dr.Item("Messaggi") = Messaggio
        dr.Item("TrasportoACura") = objRigheConfer.TrasportoACura
        dr.Item("Prezzo_da_riga_conferimento") = 0

        Return ""
    End Function

    Public Function ValorizzaFattoriDiVariazione(ByVal piva As String, ByVal CalCod As Integer,
                                                 ByVal listinoCod As Integer,
                                                 ByVal id_testata_griglia_listino As Integer,
                                                 ByVal Data_Riferimento_Prezzi As Date,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As List(Of Object)

        Dim listOggettiParametriQualitativi As New List(Of Object)

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim fattoreVariazione_paramqual As DbSet(Of CampionamentoConferito_Fattori_Variazione_ParamQualitativi) = GiasContext.CampionamentoConferito_Fattori_Variazione_ParamQualitativi

            ' Cerco tutti i fattori di variazione attivi
            Dim FattoreVar_ParamQual =
                From fattori_variazione_parqual In fattoreVariazione_paramqual
                Join otab In GiasContext.OTabelle
                 On otab.Tabella_Cod Equals fattori_variazione_parqual.Tabella_Cod
                Join tabelleParametri In GiasContext.OTabelle_Parametri
                 On tabelleParametri.Tabella_Cod Equals fattori_variazione_parqual.Tabella_Cod And
                    tabelleParametri.Tabella_Par_Cod Equals fattori_variazione_parqual.Tabella_Par_Cod
                Where fattori_variazione_parqual.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                      fattori_variazione_parqual.PIVA.Equals(piva)
                Select New With {
                    .Id_fattore_variazione = fattori_variazione_parqual.Id_fattore_variazione,
                    .Tabella_ID = fattori_variazione_parqual.Tabella_Cod,
                    .Tabella_Des = otab.Tabella_Des,
                    .Tabella_Cod_Des = otab.Tabella_Cod_Des,
                    .val_cod = fattori_variazione_parqual.Tabella_Par_Cod,
                    .val_des = tabelleParametri.Descrizione,
                    .val_sigla = tabelleParametri.Sigla
              }

            ' Per ogni fattore di variazione attivo cerco se utilizzato nella riga di conferimento
            For Each param In FattoreVar_ParamQual.ToList()

                Dim tabellaNomeKey = "o" & param.Tabella_Cod_Des.ToLower()

                Dim Elem_Fattori_Variazione =
                     (From mpc In GiasContext.Materie_Prime_Campionature
                      Where mpc.Progressivo = CalCod AndAlso
                            mpc.Tipo = tabellaNomeKey AndAlso
                            mpc.Tipo_Cod = param.val_cod).FirstOrDefault()

                ' Se trovato cerco il valore relativo
                If Elem_Fattori_Variazione IsNot Nothing Then
                    Dim Valore_Fattori_Variazione =
                     (From pfv In GiasContext.Listini_CampionamentoConferito_Fattori_Variazione
                      Where pfv.Piva_SuperUser = Piva_SuperUser AndAlso
                            pfv.PIVA = piva AndAlso
                            pfv.Listino_Cod = listinoCod AndAlso
                            pfv.Id_fattore_variazione = param.Id_fattore_variazione AndAlso
                            pfv.Id_TestataGriglia_Prod = id_testata_griglia_listino AndAlso
                            pfv.Validita_Inizio <= Data_Riferimento_Prezzi AndAlso
                            pfv.Validita_Fine >= Data_Riferimento_Prezzi
                      Select New With {
                                         .Tabella_Des = param.Tabella_Des,
                                         .Tabella_Cod_Des = param.Tabella_Cod_Des,
                                         .Val_Cod = param.val_cod,
                                         .Val_Des = param.val_des,
                                         .Val_Sigla = param.val_sigla,
                                         .Variazione_a_valore = pfv.variazione_a_valore,
                                         .Valore_al_kg = pfv.valore_al_kg,
                                         .Id_listino_fattore_variaz = pfv.Id_listino_fattore_variaz
                          }).FirstOrDefault()

                    ' Se trovato un valore verifico se il prodotto della riga corrente è escluso
                    If Valore_Fattori_Variazione IsNot Nothing Then
                        listOggettiParametriQualitativi.Add(Valore_Fattori_Variazione)
                    End If
                End If
            Next
        End Using

        Return listOggettiParametriQualitativi

    End Function

    Public Function Leggi_Anagrafica_Liquidazioni(ByVal piva As String,
                                                  ByVal dataRiferimento As Date?,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Anagrafica_Liquidazioni()"
        Dim messaggioErrore As String = ""
        Dim risposta As String = ""

        Try

            Dim pivaSuperUser = objParametri.PivaSuperUser

            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim elencoTestateElem = (From testataAccLiq In GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito
                                         Group Join testataLiqRif In GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito
                                        On testataAccLiq.Piva_SuperUser Equals testataLiqRif.Piva_SuperUser And
                                           testataAccLiq.PIVA Equals testataLiqRif.PIVA And
                                           testataAccLiq.id_liquidazione_riferimento Equals testataLiqRif.id_anagrafica
                                        Into _testataLiqRif = Group
                                         From _t2 In _testataLiqRif.DefaultIfEmpty()
                                         Where testataAccLiq.Piva_SuperUser.Equals(pivaSuperUser) AndAlso
                                        testataAccLiq.PIVA.Equals(piva)
                                         Order By testataAccLiq.Validita_Inizio Descending,
                                           testataAccLiq.Validita_Fine Descending,
                                           testataAccLiq.tipo_anagrafica Descending,
                                           testataAccLiq.descrizione
                                         Select New With {
                                         testataAccLiq.id_anagrafica,
                                         testataAccLiq.descrizione,
                                         testataAccLiq.tipo_anagrafica,
                                         .tipo_anagrafica_des = "",
                                         testataAccLiq.centri_aziendali,
                                         testataAccLiq.definitivo,
                                         testataAccLiq.tipo_acconto,
                                         .tipo_acconto_des = "",
                                         testataAccLiq.perc_valore_acconto,
                                         testataAccLiq.Validita_Inizio,
                                         testataAccLiq.Validita_Fine,
                                         testataAccLiq.id_liquidazione_riferimento,
                                         .des_liquidazione_riferimento = If(_t2.descrizione, ""),
                                         .Num_Dati_Generali = (
                                            Aggregate dati_generali In GiasContext.Liquid_Mov_Dati_Generali_CampionamentoConferito
                                            Where testataAccLiq.Piva_SuperUser = dati_generali.Piva_SuperUser AndAlso
                                                  testataAccLiq.PIVA = dati_generali.PIVA AndAlso
                                                  testataAccLiq.id_anagrafica = dati_generali.Id_acconto_liquidazione
                                            Into Count()),
                                            .Righe_Con_Errori = (
                                            Aggregate dati_mov_liquid In GiasContext.Liquid_Mov_CampionamentoConferito.Where(Function(x) x.Messaggio_Errore <> "")
                                            Where testataAccLiq.Piva_SuperUser = dati_mov_liquid.Piva_SuperUser AndAlso
                                                  testataAccLiq.PIVA = dati_mov_liquid.PIVA AndAlso
                                                  testataAccLiq.id_anagrafica = dati_mov_liquid.Id_acconto_liquidazione
                                            Into Count())
                                  })

                If dataRiferimento IsNot Nothing Then
                    elencoTestateElem = elencoTestateElem.Where(Function(x) x.Validita_Inizio <= dataRiferimento AndAlso
                                            x.Validita_Fine >= dataRiferimento)
                End If

                Dim testateElemList = elencoTestateElem.ToList()

                For Each testataElem In testateElemList
                    testataElem.tipo_anagrafica_des = (From x In ElencoTipoAnagraficaLiquidazione Where x.Sigla = testataElem.tipo_anagrafica Select x.Descrizione).FirstOrDefault()
                    testataElem.tipo_acconto_des = (From x In ElencoTipoAccontoLiquidazione Where x.Sigla = testataElem.tipo_acconto Select x.Descrizione).FirstOrDefault()
                Next

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(testateElemList, Formatting.None, serializerSettings)

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message & If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]")
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta

    End Function

    Public Function ElencoLiquidazioni(ByVal piva As String,
                                       ByVal filtroTipo As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Liquidazioni()"
        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim bFiltro = False
        If Not String.IsNullOrEmpty(filtroTipo) Then
            bFiltro = True
        End If

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Elem =
               From liquidazioni In GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito
               Where
                   (liquidazioni.Piva_SuperUser.Equals(Piva_SuperUser)) _
                   AndAlso (liquidazioni.PIVA.Equals(piva)) _
                   AndAlso ((Not bFiltro) OrElse (liquidazioni.tipo_anagrafica.Equals(filtroTipo)))
               Order By liquidazioni.descrizione
               Select New With {
                   .id_liquidazione = liquidazioni.id_anagrafica,
                   .des_liquidazione = liquidazioni.descrizione
                   }

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Elem.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function Leggi_Liquidazioni(ByVal piva As String, ByRef objParametri As AgronicaCoreParametri) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Liquidazioni()"
        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Elem =
               From griglia_testata In GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito
               Where griglia_testata.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                     griglia_testata.PIVA.Equals(piva) AndAlso
                     griglia_testata.tipo_anagrafica.Equals("L")
               Order By griglia_testata.descrizione
               Select griglia_testata.id_anagrafica,
                      griglia_testata.descrizione

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Elem.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function Leggi_ContiEconomici(ByVal piva As String,
                                         ByVal anno As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_ContiEconomici()"
        Dim risposta As String = ""

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ContiEconomici =
                   From RicXConti In GiasContext.RicXConti
                   Join Conti In GiasContext.Conti
                   On
                       Conti.Piva Equals RicXConti.Piva And
                       Conti.Cod_Conto Equals RicXConti.Cod_Conto
                   Where
                        (RicXConti.Imputabile = 1) And
                        (Conti.Piva = piva) And
                        (RicXConti.Ric_Cod = 2) And
                        (RicXConti.Anno = anno)
                   Order By RicXConti.Id_Riclassificazione, Conti.Conto_Descr
                   Select New With {
                        .Descr_Conto = "",
                        .Cod_Conto = RicXConti.Cod_Conto,
                        .Id_Riclassificazione = RicXConti.Id_Riclassificazione,
                        .Conto_Descr = Conti.Conto_Descr}


            ' Compongo la descrizione
            Dim myList = ContiEconomici.ToList()
            For Each obj In myList
                obj.Descr_Conto = obj.Id_Riclassificazione & "-" & obj.Conto_Descr
            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function Leggi_Elem_AnagAccontiLiquidazioni(ByVal piva As String,
                                                       ByVal ID_Anagrafica As Integer,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Elem_AnagAccontiLiquidazioni()"
        Dim risposta As String

        Dim AnagElem As AnagAccontiLiquidazioni_CampionamentoConferito = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            AnagElem = (From acconti_liquidazioni In GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito
                        Where acconti_liquidazioni.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                              acconti_liquidazioni.PIVA.Equals(piva) AndAlso
                              acconti_liquidazioni.id_anagrafica = ID_Anagrafica
                        Select acconti_liquidazioni).FirstOrDefault()

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(AnagElem, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function LeggiElem_AnagAccontiLiquidazioni(ByVal piva As String,
                                                      ByVal IDAnag As Integer,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As AnagAccontiLiquidazioni_CampionamentoConferito

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.LeggiElem_AnagAccontiLiquidazioni()"
        Dim messaggioErrore As String = ""
        Dim Elem As AnagAccontiLiquidazioni_CampionamentoConferito = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Elem =
                (From acconti_liquidazioni In GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito
                 Where acconti_liquidazioni.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                       acconti_liquidazioni.PIVA.Equals(piva) AndAlso
                       acconti_liquidazioni.id_anagrafica = IDAnag
                 Select acconti_liquidazioni).FirstOrDefault()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Elem

    End Function

    Public Function ContaAccontiCollegatiLiquidazione(ByVal piva As String,
                                                      ByVal Id_Liquidazione As Integer,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.ContaAccontiCollegatiLiquidazione()"
        Dim messaggioErrore As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim nCount As Integer = 0

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                nCount = Aggregate acconti_liquidazioni In GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito
                        Where acconti_liquidazioni.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                              acconti_liquidazioni.PIVA.Equals(piva) AndAlso
                              acconti_liquidazioni.id_liquidazione_riferimento = Id_Liquidazione
                        Into Count()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return nCount

    End Function

    Public Function Leggi_ValoriParametriQualitativiGruppoFatturazione(ByVal piva As String,
                                                                       ByRef objParametri As AgronicaCoreParametri
                                                                       ) As String
        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_ValoriParametriQualitativiGruppoFatturazione()"
        Dim risposta As String

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Elem =
            From OTabelle_P In GiasContext.OTabelle_Parametri
            Where OTabelle_P.Modulo_Generazione = 2 AndAlso
                  OTabelle_P.Tabella_Cod = 20
            Order By OTabelle_P.Descrizione
            Select OTabelle_P.Tabella_Par_Cod, OTabelle_P.Descrizione

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Elem, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function Leggi_GruppoFatturazioneParametriQualitativi(ByVal piva As String,
                                                                 ByRef objParametri As AgronicaCoreParametri
                                                                 ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_GruppoFatturazioneParametriQualitativi()"
        Dim risposta As String

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            ' Mostro solo i prodotti utilizzati

            Dim Elem =
            From materie_prime In GiasContext.Materie_Prime.Where(Function(a) _
                      GiasContext.Movimenti_dettagli.Any(Function(y) y.Mat_Cod = a.Mat_Cod))
            Group Join materie_prime_dettagli In GiasContext.Materie_Prime_Dettagli On materie_prime.Mat_Cod Equals materie_prime_dettagli.Mat_Cod
            Into _materie_prime_dettagli = Group
            From _mpd In _materie_prime_dettagli.DefaultIfEmpty()
            Group Join otabelle_parametri In GiasContext.OTabelle_Parametri.Where(Function(x) x.Modulo_Generazione = 2 AndAlso x.Tabella_Cod = 20)
            On _mpd.Extra_Int1 Equals otabelle_parametri.Tabella_Par_Cod
            Into _otabelle_parametri = Group
            From _opt In _otabelle_parametri.DefaultIfEmpty()
            Where
              (materie_prime.Piva.Equals(piva)) AndAlso
              (materie_prime.Elem_Cod = TRASFORMATI_VEGETALI)
            Order By materie_prime.Mat_Des
            Select New With {
                            .Mat_Cod = materie_prime.Mat_Cod,
                            .Mat_Des = materie_prime.Mat_Des,
                            .Tabella_Par_Cod = If(Not _opt Is Nothing, _opt.Tabella_Par_Cod, 0),
                            .Descrizione = If(Not _opt Is Nothing, _opt.Descrizione, "")
                            }


            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Elem.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function Leggi_GruppoFatturazione(ByVal piva As String,
                                             ByVal Tabella_Par_Cod As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As OTabelle_Parametri

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_GruppoFatturazione()"
        Dim messaggioErrore As String = ""
        Dim Elem As OTabelle_Parametri = Nothing

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Elem =
                (From otp In GiasContext.OTabelle_Parametri
                 Where otp.Tabella_Par_Cod = Tabella_Par_Cod AndAlso
                       otp.Modulo_Generazione = 2 AndAlso
                       otp.Tabella_Cod = 20
                 Select otp).FirstOrDefault()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Elem

    End Function

    Public Function Leggi_MateriePrimeDettagli(ByVal piva As String,
                                               ByVal Mat_Cod As Integer,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Materie_Prime_Dettagli

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_MateriePrimeDettagli()"
        Dim messaggioErrore As String = ""
        Dim Elem As Materie_Prime_Dettagli = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Elem =
                (From mpd In GiasContext.Materie_Prime_Dettagli
                 Where mpd.Mat_Cod = Mat_Cod AndAlso
                       mpd.Piva.Equals(piva) AndAlso
                       mpd.Piva_SuperUser.Equals(Piva_SuperUser)
                 Select mpd).FirstOrDefault()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Elem

    End Function

    Public Function Leggi_Listini_Esclusione_Fattore_Variazione(ByVal listFattVar As Listini_CampionamentoConferito_Fattori_Variazione,
                                                                ByRef objParametri As AgronicaCoreParametri
                                                                ) As List(Of Listini_CampionamentoConferito_Esclusione_Fattore_Variazione)

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Listini_Esclusione_Fattore_Variazione()"

        Dim listEsclFattVar As List(Of Listini_CampionamentoConferito_Esclusione_Fattore_Variazione)

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            GiasContext.Configuration.LazyLoadingEnabled = False

            listEsclFattVar = (From lefv In GiasContext.Listini_CampionamentoConferito_Esclusione_Fattore_Variazione
                               Where lefv.Piva_SuperUser = listFattVar.Piva_SuperUser AndAlso
                                    lefv.PIVA = listFattVar.PIVA AndAlso
                                    lefv.Id_listino_fattore_variaz = listFattVar.Id_listino_fattore_variaz).ToList()
        End Using

        Return listEsclFattVar

    End Function

    Public Function Leggi_LiquidazioniPerLancio(ByVal piva As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_LiquidazioniPerLancio()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Elem =
               From griglia_testata In GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito
               Where
                   (griglia_testata.Piva_SuperUser.Equals(Piva_SuperUser)) _
                   AndAlso
                   (griglia_testata.PIVA.Equals(piva))
               Order By griglia_testata.descrizione
               Select
                        griglia_testata.id_anagrafica,
                        griglia_testata.descrizione

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Elem.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function implo_LeggiLotto(ByVal ID As Integer, ByRef objParametri As AgronicaCoreParametri) As cbl_Calibrature

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.implo_LeggiLotto()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim lotto As cbl_Calibrature = Nothing

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            lotto = (From cal In GiasContext.cbl_Calibrature
                     Where (cal.ID = ID)
                     Select cal).FirstOrDefault()

        End Using

        Return lotto

    End Function

    Public Class JoinedCal
        Public ID_Calibro As Integer
        Public Flag_Importato As Integer
        Public Qualita As String
        Public Calibro As String
        Public Perc As Decimal
    End Class

    Public Sub TrovaErroriInLottoDaImportare2(ByVal piva As String,
                                              ByRef lotto As cbl_Calibrature,
                                              ByRef Id_TestataGriglia_ As Integer,
                                              ByRef Id_TestataGriglia_Prod_ As Integer,
                                              ByRef Id_Mov_Det_ As Integer,
                                              ByRef errors As List(Of ErroriImportazione),
                                              ByRef objParametri As AgronicaCoreParametri,
                                              Optional ByRef assocCalibri As List(Of JoinedCal) = Nothing)

        Id_TestataGriglia_ = 0
        Id_TestataGriglia_Prod_ = 0
        Id_Mov_Det_ = 0

        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Id_Lotto = lotto.ID

            '***************************************************************************************************
            ' DEBUGGING Seleziono subito il movimento con RifBolla esatta
            ' nel vecchio procedimento prendo la lista di tutti i movimenti e cerco RifBolla che mi interessa
            '***************************************************************************************************
            'Dim lotto_RifBolla = Left(lotto.RifBolla, lotto.RifBolla.Length - 1)
            'Dim lotto_RigaBolla = Right(lotto.RifBolla, 1).PadLeft(2, "0"c) 'ultimo carattere left padded con '0' lungo 2???

            'Dim rifBolle = From rb In (From mov In GiasContext.Movimenti
            '                           Where (mov.Cau_Mov = CAU_CARICO Or mov.Cau_Mov = CAU_REGISTRAZIONI_TERZIARIA)
            '                           Select New With {
            '                                   .Id_Mov = mov.Id_Mov,
            '                                   .RifBolla = Right(CStr("00000" & mov.Doc_Numero_Sin & CStr(mov.Doc_Numero) & mov.Doc_Numero_Des.Replace("/", "")), 5),
            '                                   .Cod_RisUm = mov.Cod_RisUm
            '                           })
            '               Where rb.RifBolla = lotto_RifBolla
            '               Select rb

            'If (Not rifBolle.Any()) Then

            '    'errors.Add(ErroriImportazione.BollaNonTrovata)

            'Else
            '    Dim rifBolla = rifBolle.First()

            '    Dim rapporto_ As Integer?() = {COD_FORNITORE_ORTOFRUTTA, COD_CONFERENTE}

            '    Dim conf__ = From r_u In GiasContext.Risorse_Umane
            '                 Join c In GiasContext.Contatti
            '                On r_u.Cod_Contatto Equals c.Cod_Contatto
            '                 Where rapporto_.Contains(r_u.Cod_Rapporto) And r_u.Cod_RisUm = rifBolla.Cod_RisUm
            '                 Select r_u.Settore_Des, c.Rag_Soc

            '    If (Not conf__.Any()) Then

            '        'errors.Add(ErroriImportazione.ProduttoreNonTrovato)

            '    Else

            '        Dim first_conf__ = conf__.First()
            '        Dim conf_Settore_Des_ = first_conf__.Settore_Des.Replace("C", "")

            '        If String.Compare(conf_Settore_Des_, lotto.Conferitore_Codice) = 0 Then

            '            'lotto.Conferitore_Nome = first_conf__.Rag_Soc

            '        Else

            '            'errors.Add(ErroriImportazione.ProduttoreDiverso)

            '        End If

            '    End If

            '    Dim riga_ = From movimenti_dettagli In GiasContext.Movimenti_dettagli
            '                Where movimenti_dettagli.Id_Mov = rifBolla.Id_Mov And movimenti_dettagli.Ordine_Det = lotto_RigaBolla
            '                Select movimenti_dettagli

            '    If (Not riga_.Any()) Then

            '        'errors.Add(ErroriImportazione.RigaBollaNonTrovata)

            '    Else

            '    End If

            'End If
            '*****************************************************************************************

            Dim bolle_ = (From movimenti In GiasContext.Movimenti
                          Where movimenti.Cau_Mov = CAU_CARICO OrElse
                                movimenti.Cau_Mov = CAU_REGISTRAZIONI_TERZIARIA
                          Select movimenti.Id_Mov,
                              movimenti.Doc_Numero_Sin,
                              movimenti.Doc_Numero,
                              movimenti.Doc_Numero_Des,
                              movimenti.Data_Movimento,
                              movimenti.Cod_RisUm).ToList()


            Dim b_Id_Mov As Integer = 0
            Dim b_Cod_RisUm As Integer = 0
            Dim nr_bolla As String
            Dim riga_bolla As String = ""
            Dim data_bolla As DateTime

            If Not String.IsNullOrEmpty(lotto.RifBolla) AndAlso Not String.IsNullOrEmpty(lotto.Note) Then

                nr_bolla = Left(lotto.RifBolla, lotto.RifBolla.Length - 1) 'tutto meno l'ultimo carattere
                riga_bolla = Right(lotto.RifBolla, 1).PadLeft(2, "0"c) 'ultimo carattere left padded con '0' lungo 2???
                data_bolla = CDate(CStr(2000 + CInt(Right(lotto.Note, 2))) & "-" & Mid(lotto.Note, 3, 2) & " - " & Left(lotto.Note, 2))

                For Each b_ In bolle_
                    Dim n_sin = b_.Doc_Numero_Sin.Replace("/", "")
                    Dim n_des = b_.Doc_Numero_Des.Replace("/", "")
                    Dim b_nr_bolla = (n_sin & CStr(b_.Doc_Numero) & n_des).PadLeft(5, "0")
                    Dim b_data_bolla = CDate(b_.Data_Movimento)

                    If String.Compare(nr_bolla, b_nr_bolla) = 0 _
                        AndAlso data_bolla.Day = b_data_bolla.Day _
                        AndAlso data_bolla.Month = b_data_bolla.Month _
                        AndAlso data_bolla.Year = b_data_bolla.Year Then

                        b_Id_Mov = b_.Id_Mov
                        b_Cod_RisUm = b_.Cod_RisUm
                        Exit For

                    End If

                Next

            End If

            If (b_Id_Mov = 0) Then

                errors.Add(ErroriImportazione.BollaNonTrovata)

            Else

                Dim rapporto As Integer?() = {COD_FORNITORE_ORTOFRUTTA, COD_CONFERENTE}

                Dim conf_ = From r_u In GiasContext.Risorse_Umane
                            Join c In GiasContext.Contatti
                            On r_u.Cod_Contatto Equals c.Cod_Contatto
                            Where rapporto.Contains(r_u.Cod_Rapporto) AndAlso r_u.Cod_RisUm = b_Cod_RisUm
                            Select r_u.Settore_Des, c.Rag_Soc

                If (Not conf_.Any()) Then

                    errors.Add(ErroriImportazione.ProduttoreNonTrovato)

                Else

                    Dim first_conf_ = conf_.First()
                    Dim conf_Settore_Des = first_conf_.Settore_Des.Replace("C", "")

                    If String.Compare(conf_Settore_Des, lotto.Conferitore_Codice) = 0 Then

                        lotto.Conferitore_Nome = first_conf_.Rag_Soc

                    Else

                        errors.Add(ErroriImportazione.ProduttoreDiverso)

                    End If

                End If

                Dim riga = From movimenti_dettagli In GiasContext.Movimenti_dettagli
                           Where movimenti_dettagli.Id_Mov = b_Id_Mov AndAlso
                               (CStr(movimenti_dettagli.Ordine_Det) = riga_bolla OrElse movimenti_dettagli.Extra_Str = riga_bolla)
                           Select movimenti_dettagli

                If Not riga.Any() Then

                    errors.Add(ErroriImportazione.RigaBollaNonTrovata)

                Else

                    Dim mov_dett As Movimenti_dettagli = riga.First()
                    Id_Mov_Det_ = mov_dett.Id_Mov_Det

                    Try
                        Leggi_Id_Testata_Griglia_Da_Movim_Conferimento(piva,
                                                                       Id_Mov_Det_,
                                                                       Id_TestataGriglia_,
                                                                       Id_TestataGriglia_Prod_,
                                                                       True,
                                                                       True,
                                                                       objParametri)
                    Catch ex As Exception

                    End Try

                    If (Id_TestataGriglia_Prod_ = 0) Then

                        errors.Add(ErroriImportazione.TestataGrigliaNonTrovata)

                    Else

                        Dim Id_TG = Id_TestataGriglia_
                        Dim Id_TG_Prod = Id_TestataGriglia_Prod_

                        'Controllo che la testata griglia trovata non sia già presente in campionamento
                        Dim griglia = From mov In GiasContext.CampionamentoConferito_Movimenti
                                      Where mov.Piva_SuperUser = Piva_SuperUser AndAlso
                                          mov.PIVA = piva AndAlso
                                          mov.Id_TestataGriglia_Prod = Id_TG_Prod AndAlso
                                          mov.Id_Mov_Det = mov_dett.Id_Mov_Det
                                      Select mov

                        If (griglia.Any()) Then
                            'Avviso -> in importazione sommerà le quantità e ricalcola le percentuali
                            errors.Add(ErroriImportazione.LottoCampionato)

                        End If

                        'Controllo che tutti i calibri campionati corrispondano ai calibri presenti in 
                        'CampionamentoConferito_TestataGriglia_Calibri con Id_TestataGriglia = Id_Testata_Griglia

                        Dim calibri_count = From cal In GiasContext.cbl_CalibratureXCalibri
                                            Where cal.IDCalibro = Id_Lotto
                                            Group cal By cal.Nome, cal.Qualita Into Group Where Group.Count() > 1
                                            Select New With {.Nome = Nome}

                        If (calibri_count.Any()) Then
                            errors.Add(ErroriImportazione.CalibroDuplicato)
                        End If

                        Dim calibri_camp = From cal In GiasContext.cbl_CalibratureXCalibri
                                           Where cal.IDCalibro = Id_Lotto
                                           Order By cal.ID
                                           Select New With {
                                                .Flag = 1,
                                                .Qualita = cal.Qualita,
                                                .Calibro = cal.Nome,
                                                .Perc = CDec(cal.Perc)
                                            }

                        Dim calibri_griglia = From cal In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
                                              Where cal.Id_TestataGriglia = Id_TG AndAlso
                                                    cal.Piva_SuperUser = Piva_SuperUser AndAlso
                                                    cal.PIVA = piva
                                              Order By cal.Ordinamento
                                              Select New With {
                                                  .Id_Calibro = cal.Id_Calibro,
                                                  .Qualita = cal.Descr_qualita,
                                                  .Calibro = cal.Descr_calibro}


                        Dim leftjoin = From cal1 In calibri_camp
                                       Group Join cal2 In calibri_griglia
                                           On cal1.Qualita Equals cal2.Qualita And
                                           cal1.Calibro Equals cal2.Calibro Into gr = Group
                                       From g In gr.DefaultIfEmpty(New With {.Id_Calibro = 0, .Qualita = "", .Calibro = ""})
                                       Select New JoinedCal With {
                                            .ID_Calibro = g.Id_Calibro,
                                            .Flag_Importato = cal1.Flag,
                                            .Qualita = cal1.Qualita,
                                            .Calibro = cal1.Calibro,
                                            .Perc = cal1.Perc
                                        }

                        Dim rightjoin = From cal2 In calibri_griglia
                                        Group Join cal1 In calibri_camp
                                           On cal1.Qualita Equals cal2.Qualita And
                                           cal1.Calibro Equals cal2.Calibro Into gr = Group
                                        From g In gr.DefaultIfEmpty(New With {.Flag = 0, .Qualita = "", .Calibro = "", .Perc = CDec(0)})
                                        Select New JoinedCal With {
                                            .ID_Calibro = cal2.Id_Calibro,
                                            .Flag_Importato = g.Flag,
                                            .Qualita = cal2.Qualita,
                                            .Calibro = cal2.Calibro,
                                            .Perc = g.Perc
                                        }

                        Dim fulljoin = leftjoin.Union(rightjoin)

                        Dim joined = fulljoin.ToList()

                        If assocCalibri IsNot Nothing Then
                            assocCalibri = joined
                        End If

                        Dim err0 = False
                        Dim err1 = False

                        For Each elem In joined

                            If (elem.ID_Calibro = 0) Then
                                If (Not err0) Then
                                    errors.Add(ErroriImportazione.CalibroNonTrovato)
                                    err0 = True
                                End If
                            End If

                            If (elem.Flag_Importato = 0) Then
                                If (Not err1) Then
                                    errors.Add(ErroriImportazione.CalibroNonImportato)
                                    err1 = True
                                End If
                            End If

                            If err0 AndAlso err1 Then
                                Exit For
                            End If

                        Next

                        If (Not err0 AndAlso Not err1) Then

                            'controllo anche se l'ordine dei calibri campionati è uguale ai calibri in griglia
                            Dim errOrd = False
                            Dim cnt As Integer = calibri_camp.Count
                            If (cnt = calibri_griglia.Count) Then

                                Dim list_camp = calibri_camp.ToList()
                                Dim list_griglia = calibri_griglia.ToList()

                                For i = 0 To cnt - 1
                                    If (Not String.Compare(list_camp(i).Qualita.Trim(), list_griglia(i).Qualita.Trim()) = 0) OrElse
                                        (Not String.Compare(list_camp(i).Calibro.Trim(), list_griglia(i).Calibro.Trim()) = 0) Then

                                        errOrd = True
                                        Exit For

                                    End If
                                Next
                            End If

                            If errOrd Then
                                errors.Add(ErroriImportazione.OrdinamentoNonRispettato)
                            End If

                        End If

                    End If
                End If
            End If

            '-------------------------------------------------------------------------------------
            'Altri controlli...
            '-------------------------------------------------------------------------------------

            'Controllo per la somma
            Dim dett = (From d In GiasContext.cbl_CalibratureXCalibri
                        Where d.IDCalibro = Id_Lotto
                        Group d By d.IDCalibro Into grouping = Group
                        Select IDCalibro,
                        SumPerc = grouping.Sum(Function(d) d.Perc),
                        SumPeso = grouping.Sum(Function(d) d.Peso)).FirstOrDefault()

            If (dett Is Nothing) Then

                errors.Add(ErroriImportazione.DettaglioNonEsiste)

            Else
                '------------------------------------------------------
                'TODO: controllare peso e/o percentuali totali
                '------------------------------------------------------
                Dim sumPeso As Double = dett.SumPeso
                Dim totPeso As Double = lotto.PesoTot
                Dim sumPerc As Decimal = dett.SumPerc
                If (Math.Abs(sumPerc - 100D) > 0.001D) Then

                    errors.Add(ErroriImportazione.PercentualeNon100)

                End If
            End If

            Dim NrBolla_Lotto = lotto.NrBolla

            'Controllo che il lotto non sia già stato importato
            Dim lotti_ = From cal_ In GiasContext.cbl_Calibrature
                         Where cal_.NrBolla = NrBolla_Lotto AndAlso
                               cal_.ID <> Id_Lotto
                         Select cal_

            If (lotti_.Any()) Then

                errors.Add(ErroriImportazione.LottoImportato)

            End If

            'Controllo esistenza lotto ???
            'Controllo corrispondenza righe ???
            'Controllo codice prodotto ???

        End Using

    End Sub

    Public Sub TrovaErroriInLottoDaImportare1(ByVal piva As String,
                                              ByRef lotto As cbl_Calibrature,
                                              ByRef errors As List(Of ErroriImportazione),
                                              ByRef objParametri As AgronicaCoreParametri)

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.TrovaErroriInLottoDaImportare2()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim rapporto As Integer?() = {COD_FORNITORE_ORTOFRUTTA, COD_CONFERENTE}

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            If (lotto.Lotto IsNot Nothing) AndAlso (Not String.IsNullOrEmpty(lotto.Lotto)) Then

                Dim lotto2Search = If(lotto.Lotto.IndexOf(" ") < 0, lotto.Lotto, lotto.Lotto.Substring(0, lotto.Lotto.IndexOf(" ")))

                Dim elem = (From mov_det In GiasContext.Movimenti_dettagli
                            Join mat_prime In GiasContext.Materie_Prime
                                On mat_prime.Elem_Cod Equals mov_det.Elem_Cod _
                                And mat_prime.Mat_Cod Equals mov_det.Mat_Cod
                            Join mov In GiasContext.Movimenti
                                On mov.Id_Mov Equals mov_det.Id_Mov
                            Join ris_um In GiasContext.Risorse_Umane
                                On ris_um.Cod_RisUm Equals mov.Cod_RisUm
                            Join cont In GiasContext.Contatti
                                On cont.Cod_Contatto Equals ris_um.Cod_Contatto
                            Where mov_det.Lotto = lotto2Search AndAlso
                                  rapporto.Contains(ris_um.Cod_Rapporto) AndAlso
                                  (mov.Cau_Mov = CAU_CARICO OrElse mov.Cau_Mov = CAU_REGISTRAZIONI_TERZIARIA) AndAlso
                                  mov_det.Elem_Cod = TRASFORMATI_VEGETALI
                            Select New With {
                                .Conferitore = cont.Rag_Soc,
                                .Varieta = mat_prime.Mat_Des
                            }).FirstOrDefault()


                If (elem Is Nothing) Then

                    errors.Add(ErroriImportazione.RigaBollaNonTrovata)

                End If

            Else

                errors.Add(ErroriImportazione.RigaBollaNonTrovata)

            End If

        End Using

    End Sub


    Private Class ImpLotto
        Public ID_Lotto As String
        Public Lotto As String
        Public Conferitore_Codice As String
        Public Conferitore_Nome As String
        Public Varieta As String
        Public Qualita As String
        Public Calibro As String
        Public Programma As String
        Public PesoTot As Decimal
        Public NrBolla As String
        Public RifBolla As String
        Public Errore As String
        Public Importatore As Integer
    End Class

    Private Function Leggi_LottiDaImportare_1(ByVal piva As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As List(Of ImpLotto)
        Dim list As New List(Of ImpLotto)

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim impo_list = From cal In GiasContext.cbl_Calibrature
                            Join macchine In GiasContext.Linee_Macchine_Lavorazione
                                On macchine.codice Equals cal.Cod_Macchina_Lav
                            Where (cal.Stato = statoImportazione.fileImportato) _
                            AndAlso (macchine.TipoImportatore = TipoImportatore.Importatore_Calibratrice)
                            Select New With {
                                .ID_Lotto = cal.ID,
                                .Lotto = cal.Lotto,
                                .primo_lotto = If(cal.Lotto.IndexOf(" ") < 0, cal.Lotto, cal.Lotto.Substring(0, cal.Lotto.IndexOf(" "))),
                                .Conferitore_Codice = cal.Conferitore_Codice,
                                .Conferitore_Nome = cal.Conferitore_Nome,
                                .Varieta = cal.Varieta,
                                .Programma = cal.Programma,
                                .PesoTot = cal.PesoTot,
                                .NrBolla = cal.NrBolla,
                                .RifBolla = cal.RifBolla,
                                .Errore = cal.Errore,
                                .Importatore = macchine.TipoImportatore
                            }

            If (impo_list.Any()) Then

                Dim matPrimeCampionature As DbSet(Of Materie_Prime_Campionature) = GiasContext.Materie_Prime_Campionature
                Dim tabelleParametri As DbSet(Of OTabelle_Parametri) = GiasContext.OTabelle_Parametri

                Dim mov_list = (From mov_det In GiasContext.Movimenti_dettagli
                                Join mat_prime In GiasContext.Materie_Prime
                                    On mat_prime.Elem_Cod Equals mov_det.Elem_Cod _
                                    And mat_prime.Mat_Cod Equals mov_det.Mat_Cod
                                Join mov In GiasContext.Movimenti
                                    On mov.Id_Mov Equals mov_det.Id_Mov
                                Join mat_prime_camp_qualita In matPrimeCampionature.Where(Function(x) x.Tipo = "oqualità")
                                    On mat_prime_camp_qualita.Progressivo Equals mov_det.Cal_Cod
                                Group Join otab_param_qual In tabelleParametri.Where(Function(x) x.Tabella_Cod = 3 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                                    On otab_param_qual.Tabella_Par_Cod Equals mat_prime_camp_qualita.Tipo_Cod
                                    Into otab_param_qual_group = Group
                                From _otp_qual In otab_param_qual_group.DefaultIfEmpty()
                                Join mat_prime_camp_calibri In matPrimeCampionature.Where(Function(x) x.Tipo = "ocalibro")
                                    On mat_prime_camp_calibri.Progressivo Equals mov_det.Cal_Cod
                                Group Join otab_param_calibro In tabelleParametri.Where(Function(x) x.Tabella_Cod = 1 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                                    On otab_param_calibro.Tabella_Par_Cod Equals mat_prime_camp_calibri.Tipo_Cod
                                    Into otab_param_cal_group = Group
                                From _otp_cal In otab_param_cal_group.DefaultIfEmpty()
                                Where (mov.Cau_Mov = CAU_CARICO OrElse mov.Cau_Mov = CAU_REGISTRAZIONI_TERZIARIA) AndAlso
                                mov_det.Elem_Cod = TRASFORMATI_VEGETALI
                                Select New With {
                                    .Lotto = mov_det.Lotto,
                                    .Mat_Des = mat_prime.Mat_Des,
                                    .Qualita = If(_otp_qual.Descrizione, ""),
                                    .Calibro = If(_otp_cal.Descrizione, "")
                                } Distinct)

                Dim joined = From il In impo_list
                             Group Join mlist In mov_list
                                On mlist.Lotto Equals il.primo_lotto Into _mlist = Group
                             From _ml In _mlist.DefaultIfEmpty()
                             Select New ImpLotto With {
                                .ID_Lotto = il.ID_Lotto,
                                .Lotto = il.Lotto,
                                .Conferitore_Codice = il.Conferitore_Codice,
                                .Conferitore_Nome = il.Conferitore_Nome,
                                .Varieta = If(_ml.Mat_Des Is Nothing, il.Varieta, _ml.Mat_Des),
                                .Qualita = If(_ml.Qualita, ""),
                                .Calibro = If(_ml.Calibro, ""),
                                .Programma = il.Programma,
                                .PesoTot = il.PesoTot,
                                .NrBolla = il.NrBolla,
                                .RifBolla = il.RifBolla,
                                .Errore = il.Errore,
                                .Importatore = il.Importatore
                            }

                list = joined.ToList()

            End If

        End Using

        Return list

    End Function

    Private Function Leggi_LottiDaImportare_2(ByVal piva As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As List(Of ImpLotto)
        Dim list As New List(Of ImpLotto)

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim impo_list = From cal In GiasContext.cbl_Calibrature
                            Join macchine In GiasContext.Linee_Macchine_Lavorazione
                                On macchine.codice Equals cal.Cod_Macchina_Lav
                            Where (cal.Stato = statoImportazione.fileImportato) _
                            AndAlso (macchine.TipoImportatore = TipoImportatore.Importatore_Campionatrice)
                            Select New With {
                                .ID_Lotto = cal.ID,
                                .Lotto = cal.Lotto,
                                .Conferitore_Codice = cal.Conferitore_Codice,
                                .Conferitore_Nome = cal.Conferitore_Nome,
                                .Varieta = cal.Varieta,
                                .Programma = cal.Programma,
                                .PesoTot = CDec(cal.PesoTot),
                                .NrBolla = cal.NrBolla,
                                .RifBolla = cal.RifBolla,
                                .RifBollaData_dd = CInt(Left(cal.Note, 2)),
                                .RifBollaData_mm = CInt(Mid(cal.Note, 3, 2)),
                                .RifBollaData_yyyy = 2000 + CInt(Right(cal.Note, 2)),
                                .Errore = cal.Errore,
                                .Importatore = macchine.TipoImportatore
                            }

            If impo_list.Any() Then

                Dim matPrimeCampionature As DbSet(Of Materie_Prime_Campionature) = GiasContext.Materie_Prime_Campionature
                Dim tabelleParametri As DbSet(Of OTabelle_Parametri) = GiasContext.OTabelle_Parametri

                Dim mov_list = From mov In GiasContext.Movimenti
                               Join mov_det In GiasContext.Movimenti_dettagli
                                    On mov_det.Id_Mov Equals mov.Id_Mov
                               Join mat_prime In GiasContext.Materie_Prime
                                    On mat_prime.Elem_Cod Equals mov_det.Elem_Cod _
                                    And mat_prime.Mat_Cod Equals mov_det.Mat_Cod
                               Join mat_prime_camp_qualita In matPrimeCampionature.Where(Function(x) x.Tipo = "oqualità")
                                    On mat_prime_camp_qualita.Progressivo Equals mov_det.Cal_Cod
                               Group Join otab_param_qual In tabelleParametri.Where(Function(x) x.Tabella_Cod = 3 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                                    On otab_param_qual.Tabella_Par_Cod Equals mat_prime_camp_qualita.Tipo_Cod
                                    Into otab_param_qual_group = Group
                               From _otp_qual In otab_param_qual_group.DefaultIfEmpty()
                               Join mat_prime_camp_calibri In matPrimeCampionature.Where(Function(x) x.Tipo = "ocalibro")
                                    On mat_prime_camp_calibri.Progressivo Equals mov_det.Cal_Cod
                               Group Join otab_param_calibro In tabelleParametri.Where(Function(x) x.Tabella_Cod = 1 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                                    On otab_param_calibro.Tabella_Par_Cod Equals mat_prime_camp_calibri.Tipo_Cod
                                    Into otab_param_cal_group = Group
                               From _otp_cal In otab_param_cal_group.DefaultIfEmpty()
                               Where (mov.Cau_Mov = CAU_CARICO OrElse mov.Cau_Mov = CAU_REGISTRAZIONI_TERZIARIA) AndAlso
                                     mov_det.Elem_Cod = TRASFORMATI_VEGETALI
                               Select New With {
                                   .RifBolla = Right(CStr("00000" & mov.Doc_Numero_Sin & CStr(mov.Doc_Numero) & mov.Doc_Numero_Des.Replace("/", "")), 5) & Right(mov_det.Extra_Str, 1),
                                   .RifBollaData_dd = CDate(mov.Data_Movimento).Day,
                                   .RifBollaData_mm = CDate(mov.Data_Movimento).Month,
                                   .RifBollaData_yyyy = CDate(mov.Data_Movimento).Year,
                                   .Mat_Des = mat_prime.Mat_Des,
                                   .Qualita = If(_otp_qual.Descrizione, ""),
                                   .Calibro = If(_otp_cal.Descrizione, "")
                                }

                Dim joined = From il In impo_list
                             Group Join mlist In mov_list
                                On mlist.RifBolla Equals il.RifBolla _
                                 And mlist.RifBollaData_dd Equals il.RifBollaData_dd _
                                 And mlist.RifBollaData_mm Equals il.RifBollaData_mm _
                                 And mlist.RifBollaData_yyyy Equals il.RifBollaData_yyyy Into _mlist = Group
                             From _ml In _mlist.DefaultIfEmpty()
                             Select New ImpLotto With {
                                .ID_Lotto = il.ID_Lotto,
                                .Lotto = il.Lotto,
                                .Conferitore_Codice = il.Conferitore_Codice,
                                .Conferitore_Nome = il.Conferitore_Nome,
                                .Varieta = If(_ml.Mat_Des Is Nothing, il.Varieta, _ml.Mat_Des),
                                .Qualita = If(_ml.Qualita, ""),
                                .Calibro = If(_ml.Calibro, ""),
                                .Programma = il.Programma,
                                .PesoTot = il.PesoTot,
                                .NrBolla = il.NrBolla,
                                .RifBolla = il.RifBolla,
                                .Errore = il.Errore,
                                .Importatore = il.Importatore
                            }

                list = joined.ToList()

            End If

        End Using

        Return list

    End Function

    Public Function Leggi_LottiDaImportare(ByVal piva As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_LottiDaImportare()"
        Dim risposta As String = ""

        Dim list_1 = Leggi_LottiDaImportare_1(piva, objParametri)

        Dim list_2 = Leggi_LottiDaImportare_2(piva, objParametri)

        Dim list = list_1.Union(list_2).OrderBy(Function(e) e.Lotto)

        Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
        risposta = JsonConvert.SerializeObject(list.ToList(), Formatting.None, serializerSettings)

        Return risposta

    End Function

    Public Function Leggi_DettaglioLottoDaImportare(ByVal piva As String, ByVal lid As Integer,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_DettaglioLottoDaImportare()"
        Dim risposta As String = ""

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim elem =
                From calxcal In GiasContext.cbl_CalibratureXCalibri
                Where (calxcal.IDCalibro = lid)
                Order By calxcal.ID
                Select New With {
                    .OrdineLogico = 0,
                    .ID = calxcal.ID,
                    .Qualita = calxcal.Qualita,
                    .Nome = calxcal.Nome,
                    .Peso = calxcal.Peso,
                    .Perc = calxcal.Perc
                }
            Dim elemList = elem.ToList()
            Dim ol = 0
            For Each e In elemList
                e.OrdineLogico = ol
                ol += 1
            Next
            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(elemList, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function



    Private Class MasterRiepilogo
        Public ID As Integer
        Public DataCalibratura As DateTime
        Public Conferitore As String
        Public Varieta As String
        Public Lotto As String
        Public Bolla As String
        Public RifBolla As String
        Public PesoTot As Decimal
        Public Scarti As Decimal
    End Class

    Private Class ElemRiepilogo
        Public ID_Calibro As Integer
        Public DataCalibratura As DateTime
        Public Conferitore As String
        Public Varieta As String
        Public Lotto As String
        Public Bolla As String
        Public RifBolla As String
        Public ID_Sequenza As Integer
        Public Qualita As String
        Public Calibro As String
        Public Peso As Decimal
        Public Perc As Decimal
        Public Num As Integer
        Public PesoTot As Decimal
        Public PesoMedio As Decimal
    End Class

    Private Function Leggi_RiepilogoImporazione_1(ByVal dataCalibDal As Nullable(Of DateTime),
                                                  ByVal dataCalibAl As Nullable(Of DateTime),
                                                  ByVal fornitori As String(),
                                                  ByVal specie As Integer,
                                                  ByVal varieta As Integer(),
                                                  ByVal stato As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As List(Of ElemRiepilogo)

        '*************************************************************
        'Lettura per dati con TipoImportatore.Importatore_Calibratrice
        '*************************************************************
        Dim list As New List(Of ElemRiepilogo)

        Dim rapporto As Integer?() = {COD_FORNITORE_ORTOFRUTTA, COD_CONFERENTE}

        Dim bFiltroFornitori As Boolean = False
        If fornitori IsNot Nothing AndAlso fornitori.Length > 0 Then
            bFiltroFornitori = True
        End If

        Dim bFiltroVarieta As Boolean = False
        If varieta IsNot Nothing AndAlso varieta.Length > 0 Then
            bFiltroVarieta = True
        End If

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            GiasContext.Database.CommandTimeout = 3600


            Dim riep_list = From cbl_cal In GiasContext.cbl_Calibrature
                            Join macchine In GiasContext.Linee_Macchine_Lavorazione
                                On macchine.codice Equals cbl_cal.Cod_Macchina_Lav
                            Where (macchine.TipoImportatore = TipoImportatore.Importatore_Calibratrice) _
                                AndAlso (cbl_cal.PesoTot > 0) _
                                AndAlso ((dataCalibDal Is Nothing) OrElse cbl_cal.Data_Inizio >= dataCalibDal) _
                                AndAlso ((dataCalibAl Is Nothing) OrElse cbl_cal.Data_Inizio <= dataCalibAl) _
                                AndAlso ((stato = 0) OrElse (cbl_cal.Stato = stato))
                            Select New With {
                                .ID = cbl_cal.ID,
                                .DataCalibratura = cbl_cal.Data_Inizio,
                                .primo_lotto = If(cbl_cal.Lotto.IndexOf(" ") < 0, cbl_cal.Lotto, cbl_cal.Lotto.Substring(0, cbl_cal.Lotto.IndexOf(" "))),
                                .Lotto = cbl_cal.Lotto,
                                .Scarti = If(cbl_cal.Scarti Is Nothing, 0, CDec(cbl_cal.Scarti)),
                                .PesoTot = If(cbl_cal.PesoTot Is Nothing, 0, CDec(cbl_cal.PesoTot))
                            }

            If (riep_list.Any()) Then

                Dim mov_list = (From mov_det In GiasContext.Movimenti_dettagli
                                Join mat_prime In GiasContext.Materie_Prime
                                    On mat_prime.Elem_Cod Equals mov_det.Elem_Cod _
                                    And mat_prime.Mat_Cod Equals mov_det.Mat_Cod
                                Join mov In GiasContext.Movimenti
                                    On mov.Id_Mov Equals mov_det.Id_Mov
                                Join ris_um In GiasContext.Risorse_Umane
                                    On ris_um.Cod_RisUm Equals mov.Cod_RisUm
                                Join cont In GiasContext.Contatti
                                    On cont.Cod_Contatto Equals ris_um.Cod_Contatto
                                Where rapporto.Contains(ris_um.Cod_Rapporto)
                                Where (mov.Cau_Mov = CAU_CARICO OrElse mov.Cau_Mov = CAU_REGISTRAZIONI_TERZIARIA) _
                                AndAlso mov_det.Elem_Cod = TRASFORMATI_VEGETALI _
                                AndAlso ((bFiltroFornitori = False) OrElse fornitori.Contains(cont.Cod_Contatto)) _
                                AndAlso (specie = -1 OrElse mat_prime.Veg_Cod = specie) _
                                AndAlso ((bFiltroVarieta = False) OrElse varieta.Contains(mat_prime.Cul_Cod))
                                Select New With {
                                    .Lotto = mov_det.Lotto,
                                    .Rag_Soc = cont.Rag_Soc,
                                    .Mat_Des = mat_prime.Mat_Des
                                } Distinct)

                Dim joined As IQueryable(Of MasterRiepilogo)

                If (Not bFiltroFornitori) AndAlso specie = -1 Then

                    joined = From rl In riep_list
                             Group Join mlist In mov_list
                                    On mlist.Lotto Equals rl.primo_lotto Into _mlist = Group
                             From _ml In _mlist.DefaultIfEmpty()
                             Select New MasterRiepilogo With {
                                    .ID = rl.ID,
                                    .DataCalibratura = rl.DataCalibratura,
                                    .Conferitore = If(_ml.Rag_Soc, ""),
                                    .Varieta = If(_ml.Mat_Des, ""),
                                    .Lotto = rl.Lotto,
                                    .Bolla = "",
                                    .RifBolla = "",
                                    .PesoTot = CDec(rl.PesoTot),
                                    .Scarti = CDec(rl.Scarti)
                                }

                Else

                    joined = From rl In riep_list
                             Join ml In mov_list
                                    On ml.Lotto Equals rl.primo_lotto
                             Select New MasterRiepilogo With {
                                    .ID = rl.ID,
                                    .DataCalibratura = rl.DataCalibratura,
                                    .Conferitore = ml.Rag_Soc,
                                    .Varieta = ml.Mat_Des,
                                    .Lotto = rl.Lotto,
                                    .Bolla = "",
                                    .RifBolla = "",
                                    .PesoTot = CDec(rl.PesoTot),
                                    .Scarti = CDec(rl.Scarti)
                                }

                End If

                Dim dettaglio = From cbl_calxcal In GiasContext.cbl_CalibratureXCalibri
                                Where cbl_calxcal.Peso > 0
                                Select New With {
                                    .IDCalibro = cbl_calxcal.IDCalibro,
                                    .ID = cbl_calxcal.ID,
                                    .Qualita = cbl_calxcal.Qualita,
                                    .Nome = cbl_calxcal.Nome,
                                    .Peso = CDec(cbl_calxcal.Peso),
                                    .Perc = CDec(cbl_calxcal.Perc),
                                    .Num = CInt(If(cbl_calxcal.Num Is Nothing, 0, cbl_calxcal.Num))
                                }
                Dim dettaglio_scarti = From j In joined
                                       Where j.Scarti > 0
                                       Select New With {
                                            .IDCalibro = j.ID,
                                            .ID = 0,
                                            .Qualita = "",
                                            .Nome = "Scarto calibratrice",
                                            .Peso = CDec(j.Scarti),
                                            .Perc = Math.Round((CDec(j.Scarti) * 100D) / (CDec(j.PesoTot) + CDec(j.Scarti)), 2),
                                            .Num = 0
                                        }

                dettaglio = dettaglio.Concat(dettaglio_scarti)

                list = (From j In joined
                        Join dett In dettaglio
                            On dett.IDCalibro Equals j.ID
                        Order By j.Lotto, dett.ID
                        Select New ElemRiepilogo With {
                            .ID_Calibro = dett.ID,
                            .DataCalibratura = j.DataCalibratura,
                            .Conferitore = j.Conferitore,
                            .Varieta = j.Varieta,
                            .Lotto = j.Lotto,
                            .Bolla = "",
                            .RifBolla = "",
                            .ID_Sequenza = dett.ID,
                            .Qualita = dett.Qualita,
                            .Calibro = dett.Nome,
                            .Peso = CDec(dett.Peso),
                            .Perc = CDec(dett.Perc),
                            .Num = CInt(dett.Num),
                            .PesoTot = CDec(j.PesoTot) + CDec(j.Scarti),
                            .PesoMedio = If(CInt(dett.Num) = 0, 0, CDec(dett.Peso) / CDec(dett.Num))
                        }).ToList()

            End If

        End Using

        Return list

    End Function

    Private Function Leggi_RiepilogoImporazione_2(ByVal dataCalibDal As Nullable(Of DateTime),
                                                  ByVal dataCalibAl As Nullable(Of DateTime),
                                                  ByVal fornitori As String(),
                                                  ByVal specie As Integer,
                                                  ByVal varieta As Integer(),
                                                  ByVal stato As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As List(Of ElemRiepilogo)

        '**************************************************************
        'Lettura per dati con TipoImportatore.Importatore_Campionatrice
        '**************************************************************

        Dim list As New List(Of ElemRiepilogo)

        Dim rapporto As Integer?() = {COD_FORNITORE_ORTOFRUTTA, COD_CONFERENTE}

        Dim bFiltroFornitori As Boolean = False
        If fornitori IsNot Nothing AndAlso fornitori.Length > 0 Then
            bFiltroFornitori = True
        End If

        Dim bFiltroVarieta As Boolean = False
        If varieta IsNot Nothing AndAlso varieta.Length > 0 Then
            bFiltroVarieta = True
        End If

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim riep_list = From cbl_cal In GiasContext.cbl_Calibrature
                            Join macchine In GiasContext.Linee_Macchine_Lavorazione
                                    On macchine.codice Equals cbl_cal.Cod_Macchina_Lav
                            Where (macchine.TipoImportatore = TipoImportatore.Importatore_Campionatrice) _
                                    AndAlso ((dataCalibDal Is Nothing) OrElse cbl_cal.Data_Inizio >= dataCalibDal) _
                                    AndAlso ((dataCalibAl Is Nothing) OrElse cbl_cal.Data_Inizio <= dataCalibAl) _
                                    AndAlso ((stato = 0) OrElse (cbl_cal.Stato = stato))
                            Select New With {
                                    .ID = cbl_cal.ID,
                                    .DataCalibratura = cbl_cal.Data_Inizio,
                                    .Bolla = cbl_cal.NrBolla,
                                    .RifBolla = cbl_cal.RifBolla,
                                    .Lotto = cbl_cal.Lotto,
                                    .PesoTot = cbl_cal.PesoTot
                            }

            If (riep_list.Any()) Then

                'Attenzione a come viene costruito il campo RifBolla
                Dim mov_list = From mov In GiasContext.Movimenti
                               Join mov_det In GiasContext.Movimenti_dettagli
                                    On mov_det.Id_Mov Equals mov.Id_Mov
                               Join ris_um In GiasContext.Risorse_Umane
                                    On ris_um.Cod_RisUm Equals mov.Cod_RisUm
                               Join cont In GiasContext.Contatti
                                    On cont.Cod_Contatto Equals ris_um.Cod_Contatto
                               Join mat_prime In GiasContext.Materie_Prime
                                    On mat_prime.Elem_Cod Equals mov_det.Elem_Cod _
                                    And mat_prime.Mat_Cod Equals mov_det.Mat_Cod
                               Where (mov.Cau_Mov = CAU_CARICO OrElse mov.Cau_Mov = CAU_REGISTRAZIONI_TERZIARIA) _
                                AndAlso mov_det.Elem_Cod = TRASFORMATI_VEGETALI _
                                   AndAlso rapporto.Contains(ris_um.Cod_Rapporto) _
                                    AndAlso ((bFiltroFornitori = False) OrElse fornitori.Contains(cont.Cod_Contatto)) _
                                    AndAlso (specie = -1 Or mat_prime.Veg_Cod = specie) _
                                    AndAlso ((bFiltroVarieta = False) OrElse varieta.Contains(mat_prime.Cul_Cod))
                               Select New With {
                                    .RifBolla = Right(CStr("00000" & mov.Doc_Numero_Sin & CStr(mov.Doc_Numero) & mov.Doc_Numero_Des.Replace("/", "")), 5) &
                                                Right(mov_det.Extra_Str, 1),
                                    .Rag_Soc = cont.Rag_Soc,
                                    .Mat_Des = mat_prime.Mat_Des
                                }

                Dim joined As IQueryable(Of MasterRiepilogo)

                If (Not bFiltroFornitori) AndAlso (specie = -1) Then

                    joined = From rl In riep_list
                             Group Join mlist In mov_list
                                    On mlist.RifBolla Equals rl.RifBolla Into _mlist = Group
                             From _ml In _mlist.DefaultIfEmpty()
                             Select New MasterRiepilogo With {
                                    .ID = rl.ID,
                                    .DataCalibratura = rl.DataCalibratura,
                                    .Conferitore = If(_ml.Rag_Soc, ""),
                                    .Varieta = If(_ml.Mat_Des, ""),
                                    .Lotto = rl.Lotto,
                                    .Bolla = rl.Bolla,
                                    .RifBolla = rl.RifBolla,
                                    .PesoTot = CDec(rl.PesoTot)
                                }

                Else

                    joined = From rl In riep_list
                             Join ml In mov_list
                                    On ml.RifBolla Equals rl.RifBolla
                             Select New MasterRiepilogo With {
                                    .ID = rl.ID,
                                    .DataCalibratura = rl.DataCalibratura,
                                    .Conferitore = ml.Rag_Soc,
                                    .Varieta = ml.Mat_Des,
                                    .Lotto = rl.Lotto,
                                    .Bolla = rl.Bolla,
                                    .RifBolla = rl.RifBolla,
                                    .PesoTot = CDec(rl.PesoTot)
                                 }

                End If

                list = (From j In joined
                        Join cbl_calxcal In GiasContext.cbl_CalibratureXCalibri
                                On cbl_calxcal.IDCalibro Equals j.ID
                        Order By j.Bolla, cbl_calxcal.ID
                        Select New ElemRiepilogo With {
                                .ID_Calibro = cbl_calxcal.ID,
                                .DataCalibratura = j.DataCalibratura,
                                .Conferitore = j.Conferitore,
                                .Varieta = j.Varieta,
                                .Lotto = j.Lotto,
                                .Bolla = j.Bolla,
                                .RifBolla = j.RifBolla,
                                .ID_Sequenza = cbl_calxcal.ID,
                                .Qualita = cbl_calxcal.Qualita,
                                .Calibro = cbl_calxcal.Nome,
                                .Peso = CDec(cbl_calxcal.Peso),
                                .Perc = CDec(cbl_calxcal.Perc),
                                .Num = CInt(If(cbl_calxcal.Num Is Nothing, 0, cbl_calxcal.Num)),
                                .PesoTot = CDec(j.PesoTot),
                                .PesoMedio = If(cbl_calxcal.Num Is Nothing OrElse cbl_calxcal.Num = 0, 0, CDec(cbl_calxcal.Peso) / CDec(cbl_calxcal.Num))
                            }).ToList()
            End If

        End Using

        Return list

    End Function

    Public Function Leggi_RiepilogoImportazione(ByVal piva As String,
                                                ByVal _dataCalibDal As String,
                                                ByVal _dataCalibAl As String,
                                                ByVal _fornitori As String(),
                                                ByVal _specie As Integer,
                                                ByVal _varieta As Integer(),
                                                ByVal _stato As Integer,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As String

        Dim risposta As String = ""

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_RiepilogoImportazione()"

        Dim dataCalibDalDT As Nullable(Of DateTime)
        dataCalibDalDT = Nothing
        If Not String.IsNullOrEmpty(_dataCalibDal) Then
            dataCalibDalDT = Convert.ToDateTime(_dataCalibDal)
        End If
        Dim dataCalibAlDT As Nullable(Of DateTime)
        dataCalibAlDT = Nothing
        If Not String.IsNullOrEmpty(_dataCalibAl) Then
            Dim tmpDT As DateTime
            tmpDT = Convert.ToDateTime(_dataCalibAl)
            tmpDT = tmpDT.AddHours(23).AddMinutes(59).AddSeconds(59)
            dataCalibAlDT = tmpDT
        End If

        If _stato <> 0 Then
            If (_stato = -1) Then 'Non importati in GIAS
                _stato = statoImportazione.fileImportato
            ElseIf (_stato = 1) Then 'Importati in GIAS
                _stato = statoImportazione.Importato_In_GIAS
            Else
                _stato = 0
            End If
        End If

        Dim list_1 = Leggi_RiepilogoImporazione_1(dataCalibDalDT,
                                                  dataCalibAlDT,
                                                  _fornitori,
                                                  _specie,
                                                  _varieta,
                                                  _stato,
                                                  objParametri)

        Dim list_2 = Leggi_RiepilogoImporazione_2(dataCalibDalDT,
                                                  dataCalibAlDT,
                                                  _fornitori,
                                                  _specie,
                                                  _varieta,
                                                  _stato,
                                                  objParametri)

        Dim list = list_1.Union(list_2)

        Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
        risposta = JsonConvert.SerializeObject(list.ToList(), Formatting.None, serializerSettings)

        Return risposta

    End Function

    Public Function LeggiLavoratoDaProdotto(ByVal piva As String,
                                            ByVal mat_cod As Integer,
                                            ByVal gen_cod As Integer,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.LeggiLavoratoDaProdotto()"
        Dim risposta As String = ""

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim mat = From mp1 In GiasContext.Materie_Prime
                      Join mp2 In GiasContext.Materie_Prime
                          On mp2.Linea_Cod Equals mp1.Linea_Cod
                      Join ogal In GiasContext.OGenerazioni_Anagrafe_Log.Where(Function(x) x.ChkScollegamento = 0)
                          On ogal.Mat_Cod Equals mp2.Mat_Cod
                      Where mp1.Mat_Cod = mat_cod AndAlso ogal.Codice_Generazione = gen_cod
                      Select New With {.mat_cod = mp2.Mat_Cod, .mat_des = mp2.Mat_Des}

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(mat.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function


    '##############################################################################################
    Private Function Leggi_DataSeminaDaConferimento(ByVal piva As String,
                                                   ByVal id_mov_det As Integer,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As Date


        Const nomeRoutine = "AgronicaCoreContabDAL.F_MagazzinoBIZ.Leggi_DataSeminaDaConferimento()"
        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim Data_Semina As Date = AGRODATAINIZIO 'Nota; in caso di nessuna corrispondenza ritorna questo valore

        Try

            strSql.Length = 0
            strSql.Append("  Select Min(MOS.Data_Movimento) as Data_Semina From Movimenti, Mov_Dettagli_Riferimenti, Mov_Destinazioni, Agenda, Agenda As AGS, Movimenti As MOS, Mov_Destinazioni As MDS ")
            strSql.Append(" Where Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And Mov_Dettagli_Riferimenti.ID_Mov_Det = " & Agro_SQL_SaveNum(id_mov_det) & " ")
            strSql.Append(" And Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda.Id_Agenda ")
            strSql.Append(" And Mov_Dettagli_Riferimenti.Lav_Cod_Rif = 125 ")
            strSql.Append(" And Agenda.Piva = Movimenti.Piva ")
            strSql.Append(" And Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov ")
            strSql.Append(" And Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.Append(" And Mov_Destinazioni.Tipo_Destinazione = 0")
            strSql.Append(" And AGS.Piva = MDS.Piva ")
            strSql.Append(" And AGS.Id_Agenda = MOS.Id_Agenda ")
            strSql.Append(" And MOS.Id_Mov = MDS.Id_Mov ")
            strSql.Append(" And AGS.Lav_Cod In (2, 71) ")
            strSql.Append(" And Mov_Destinazioni.Piva = MDS.Piva ")
            strSql.Append(" And Mov_Destinazioni.Sa_Cod = MDS.Sa_Cod ")
            strSql.Append(" And Mov_Destinazioni.Appezza = MDS.Appezza ")
            strSql.Append(" And Mov_Destinazioni.Id_Destinazione = MDS.Id_Destinazione ")
            strSql.Append(" And MDS.Tipo_Destinazione = 0 ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

            If dt.Rows.Count > 0 Then

                If IsDate(dt(0).Item("Data_Semina")) Then
                    Data_Semina = Format(dt(0).Item("Data_Semina"), "dd/MM/yyyy")
                End If

            End If

            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return Data_Semina

    End Function

    Public Function Leggi_Prezzi_Conferimento_Pomodoro(
                ByVal piva As String,
                ByVal listino_cod_da_contratto As Integer,
                ByVal elem_cod As Integer,
                ByVal mat_cod As Integer,
                ByVal tipo_cod_qualita As Integer,
                ByVal tipo_cod_calibro As Integer,
                ByVal data_movimento As Date,
                ByRef objParametri As AgronicaCoreParametri,
                ByRef prezzo As Decimal
                ) As String

        Dim risposta As String = ""
        Dim ErrMess = ""
        prezzo = 0

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Prezzi_Da_Matcod()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            ' Cerco la testata griglia interessata
            Dim Id_Testata_Griglia_Trovata As Integer = 0
            Dim Id_Testata_Griglia_Prod_Trovata As Integer = 0

            Dim s As String = Leggi_Id_Testata_Griglia_Da_MatCod(piva, False, False, elem_cod, mat_cod, tipo_cod_qualita,
                                                                 tipo_cod_calibro, data_movimento,
                                                                 Id_Testata_Griglia_Trovata, Id_Testata_Griglia_Prod_Trovata, objParametri)

            If Id_Testata_Griglia_Trovata = 0 Then
                ErrMess = "Non trovata griglia di campionamento per il prodotto: prezzo non determinabile"
            Else

                Dim id_testata_griglia_listino As Integer = 0

                ' Cerco il listino prezzi associato al fornitore
                Dim Listini_Prezzi =
                            From list_prezzi In GiasContext.Listini_Prezzi
                            Join listiniXgriglie In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Where(Function(x) x.Id_TestataGriglia = Id_Testata_Griglia_Trovata)
                            On list_prezzi.Piva_SuperUser Equals listiniXgriglie.Piva_SuperUser And
                            list_prezzi.Piva Equals listiniXgriglie.PIVA And
                            list_prezzi.Listino_Cod Equals listiniXgriglie.Listino_Cod
                            Join list_classi_prezzi In GiasContext.Listini_Classi_Prezzi
                            On list_classi_prezzi.Piva_SuperUser Equals list_prezzi.Piva_SuperUser And
                                list_classi_prezzi.Piva Equals list_prezzi.Piva And
                                list_classi_prezzi.Listino_Classe_Cod Equals list_prezzi.Listino_Classe_Cod
                            Join list_camp_conferito_prodotti In
                                GiasContext.Listini_CampionamentoConferito_Prodotti.Where(Function(x) x.Id_TestataGriglia_Prod = Id_Testata_Griglia_Prod_Trovata)
                            On
                            list_prezzi.Piva_SuperUser Equals list_camp_conferito_prodotti.Piva_SuperUser And
                            list_prezzi.Piva Equals list_camp_conferito_prodotti.PIVA And
                            list_prezzi.Listino_Cod Equals list_camp_conferito_prodotti.Listino_Cod
                            Where
                                list_prezzi.Listino_Cod = listino_cod_da_contratto AndAlso
                                list_classi_prezzi.Tipo_Classe = 1 AndAlso
                                (list_prezzi.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                                (list_prezzi.Piva.Equals(piva)) AndAlso
                                (list_prezzi.Validita_Inizio <= data_movimento AndAlso list_prezzi.Validita_Fine >= data_movimento) AndAlso
                                (list_camp_conferito_prodotti.Validita_Inizio <= data_movimento AndAlso list_camp_conferito_prodotti.Validita_Fine >= data_movimento)
                            Select New With
                            {
                                .Listino_Des = list_prezzi.Listino_Des,
                                .prezzo = list_camp_conferito_prodotti.prezzo
                            }

                Dim lstCount = Listini_Prezzi.Count()


                If lstCount = 0 Then
                    ErrMess = "Prezzo non trovato"
                End If

                If lstCount > 1 Then
                    ErrMess = "Non è possibile stabilire il prezzo; sono state trovate " & Listini_Prezzi.Count().ToString & " righe attive sui listini: "
                    Dim primoGiro = True
                    For Each lst In Listini_Prezzi.ToList
                        If Not primoGiro Then
                            ErrMess += ", "
                        End If
                        ErrMess += lst.Listino_Des
                        primoGiro = False
                    Next
                End If

                ' Trovato prezzo
                If lstCount = 1 Then
                    Dim lst = Listini_Prezzi.First()
                    prezzo = lst.prezzo
                End If

            End If

        End Using

        Return ErrMess

    End Function

End Class


'#################################################################
'#################################################################
'#################################################################

Public Class FF_CampionamentoConferimento_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################

    Public Function Aggiorna_TestataGriglia_Campionamento(ByVal piva As String,
                                                          ByVal EFArrayToInsert As ArrayList,
                                                          ByVal EFArrayToUpdate As ArrayList,
                                                          ByVal EFArrayToDelete As ArrayList,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Aggiorna_TestataGriglia_Campionamento()"
        Dim messaggioErrore As String = ""
        Dim ObjSequenze = New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each campConfTestataGriglia As CampionamentoConferito_TestataGriglia In EFArrayToInsert
                        success = False
                        For i As Integer = 0 To retries - 1
                            Try
                                'Richiedo un nuovo id sequenza
                                idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "campionamentoconferito_testatagriglia", 0, 2000000000, objParametri)
                                campConfTestataGriglia.Id_TestataGriglia = idSeq
                                GiasContext.CampionamentoConferito_TestataGriglia.Add(campConfTestataGriglia)

                                GiasContext.SaveChanges()
                                success = True
                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            messaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next

                    If success Then
                        For Each campConfTestataGriglia As CampionamentoConferito_TestataGriglia In EFArrayToUpdate
                            GiasContext.CampionamentoConferito_TestataGriglia.Attach(campConfTestataGriglia)
                            GiasContext.Entry(campConfTestataGriglia).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each campConfTestataGriglia As CampionamentoConferito_TestataGriglia In EFArrayToDelete
                            '   Dim campConf As CampionamentoConferito_TestataGriglia =
                            '        (From griglia_testata In GiasContext.CampionamentoConferito_TestataGriglia
                            '         Where
                            '           (griglia_testata.Piva_SuperUser.Equals(campConfTestataGriglia.Piva_SuperUser)) _
                            '           AndAlso
                            '           (griglia_testata.PIVA.Equals(campConfTestataGriglia.PIVA)) _
                            '           AndAlso
                            '           (griglia_testata.Id_TestataGriglia = campConfTestataGriglia.Id_TestataGriglia)
                            '         Select griglia_testata).FirstOrDefault()
                            '
                            '   GiasContext.CampionamentoConferito_TestataGriglia.Remove(campConf)
                            '
                            ' Stefano  Sostituito da quanto sotto per non rileggere

                            GiasContext.CampionamentoConferito_TestataGriglia.Attach(campConfTestataGriglia)
                            GiasContext.CampionamentoConferito_TestataGriglia.Remove(campConfTestataGriglia)
                            
                            GiasContext.SaveChanges()
                        Next

                        ' COMMIT Effettivo
                        scope.Complete()
                    End If

                End Using
            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function


    Public Function Scrivi_TestataGriglia_Campionamento(ByVal piva As String,
                                                        ByVal campConfTestataGriglia As CampionamentoConferito_TestataGriglia,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As CampionamentoConferito_TestataGriglia

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Scrivi_TestataGriglia_Campionamento()"
        Dim messaggioErrore As String = ""
        Dim ObjSequenze = New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    success = False
                    For i As Integer = 0 To retries - 1
                        Try
                            'Richiedo un nuovo id sequenza
                            idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "campionamentoconferito_testatagriglia", 0, 2000000000, objParametri)
                            campConfTestataGriglia.Id_TestataGriglia = idSeq

                            GiasContext.CampionamentoConferito_TestataGriglia.Add(campConfTestataGriglia)

                            GiasContext.SaveChanges()
                            success = True
                            Exit For
                        Catch ex As Exception
                            Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                        End Try
                    Next

                End Using
            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return campConfTestataGriglia

    End Function

    'Public Function Inserisci_Elem_Testata_GriglieCampionamento(ByVal TestataElem As CampionamentoConferito_TestataGriglia,
    '                                                            ByRef objParametri As AgronicaCoreParametri
    '                                                            ) As String

    '    Dim Piva_SuperUser = objParametri.PivaSuperUser

    '    Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Insert_Elem_Testata_GriglieCampionamento()"
    '    Dim messaggioErrore As String = ""
    '    Dim ObjSequenze = New Agro_Sequenze

    '    Dim retries As Integer = 3
    '    Dim success As Boolean = False

    '    Dim gefutils As New Gias_EF_Utility
    '    Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


    '    Try
    '        Dim idSeq As Integer = 0

    '        Using scope As New TransactionScope()

    '            For i As Integer = 0 To retries - 1
    '                Try

    '                    Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

    '                    'Richiedo un nuovo id sequenza
    '                    idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
    '                                   "campionamentoconferito_testatagriglia", 0, 2000000000, objParametri)

    '                    TestataElem.Id_TestataGriglia = idSeq

    '                    TestataElem.Data_Creazione = Date.Now
    '                    TestataElem.Username_Creazione = objParametri.UsernameOperazione
    '                    TestataElem.Data_Modifica = Date.Now
    '                    TestataElem.Username_Modifica = objParametri.UsernameOperazione

    '                    GiasContext.CampionamentoConferito_TestataGriglia.Add(TestataElem)

    '                    GiasContext.SaveChanges()

    '                    scope.Complete()

    '                    success = True

    '                    Exit For

    '                Catch ex As Exception

    '                    Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds

    '                End Try
    '            Next
    '        End Using
    '    Catch ex As Exception

    '        messaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
    '        'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

    '    End Try

    '    If Not success Then
    '        messaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
    '    End If

    '    Return messaggioErrore

    'End Function

    Public Function Aggiorna_TestataGriglia_Prodotti(ByVal piva As String,
                                                     ByVal EFArrayToInsert As ArrayList,
                                                     ByVal EFArrayToUpdate As ArrayList,
                                                     ByVal EFArrayToDelete As ArrayList,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As String

        Dim ObjSequenze = New Agro_Sequenze

        Dim messaggioErrore As String = ""

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim campConf_R As New FF_CampionamentoConferimento_R

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Aggiorna_TestataGriglia_Prodotti()"

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Dim transactionOptions As New TransactionOptions With {
                .IsolationLevel = IsolationLevel.ReadCommitted,
                .Timeout = TransactionManager.MaximumTimeout
            }

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each campConfTestataGrigliaProd As CampionamentoConferito_TestataGriglia_Prodotti In EFArrayToInsert
                        success = False

                        'Controllo che non ci siano doppioni
                        messaggioErrore = campConf_R.Controlla_Sovrapposizione_Riga_Prodotti(campConfTestataGrigliaProd, objParametri, GiasContext)
                        If messaggioErrore <> "" Then
                            success = False
                            Exit For
                        End If
                        For i As Integer = 0 To retries - 1
                            Try
                                'Richiedo un nuovo id sequenza
                                idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "campionamentoconferito_testatagriglia_prodotti", 0, 2000000000, objParametri)
                                campConfTestataGrigliaProd.Id_TestataGriglia_Prod = idSeq
                                GiasContext.CampionamentoConferito_TestataGriglia_Prodotti.Add(campConfTestataGrigliaProd)

                                GiasContext.SaveChanges()
                                success = True
                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            messaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next

                    If success Then
                        For Each campConfTestataGrigliaProd As CampionamentoConferito_TestataGriglia_Prodotti In EFArrayToUpdate

                            'Controllo che non ci siano doppioni
                            messaggioErrore = campConf_R.Controlla_Sovrapposizione_Riga_Prodotti(campConfTestataGrigliaProd, objParametri, GiasContext)
                            If messaggioErrore <> "" Then
                                success = False
                                Exit For
                            End If
                            GiasContext.CampionamentoConferito_TestataGriglia_Prodotti.Attach(campConfTestataGrigliaProd)
                            GiasContext.Entry(campConfTestataGrigliaProd).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        If success Then
                            For Each campConfTestataGrigliaProd As CampionamentoConferito_TestataGriglia_Prodotti In EFArrayToDelete
                                GiasContext.CampionamentoConferito_TestataGriglia_Prodotti.Attach(campConfTestataGrigliaProd)
                                GiasContext.CampionamentoConferito_TestataGriglia_Prodotti.Remove(campConfTestataGrigliaProd)
                                GiasContext.SaveChanges()
                            Next

                            ' COMMIT Effettivo
                            scope.Complete()

                        End If

                    End If
                End Using
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    Public Function Scrivi_TestataGriglia_Prodotti(ByVal piva As String,
                                                   ByVal Id_Testata_Cod As Integer,
                                                   ByVal campConfTestataGrigliaProd As CampionamentoConferito_TestataGriglia_Prodotti,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                     ) As CampionamentoConferito_TestataGriglia_Prodotti

        Dim ObjSequenze = New Agro_Sequenze

        Dim messaggioErrore As String = ""

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim campConf_R As New FF_CampionamentoConferimento_R

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Scrivi_TestataGriglia_Prodotti()"

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    success = False

                    'Controllo che non ci siano doppioni
                    messaggioErrore = campConf_R.Controlla_Sovrapposizione_Riga_Prodotti(campConfTestataGrigliaProd, objParametri, GiasContext)
                    If messaggioErrore <> "" Then
                        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
                    End If
                    Try
                        'Richiedo un nuovo id sequenza
                        idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                           "campionamentoconferito_testatagriglia_prodotti", 0, 2000000000, objParametri)
                        campConfTestataGrigliaProd.Id_TestataGriglia_Prod = idSeq

                        If piva <> "" Then
                            campConfTestataGrigliaProd.PIVA = piva
                        End If

                        If Id_Testata_Cod <> 0 Then
                            campConfTestataGrigliaProd.Id_TestataGriglia = Id_Testata_Cod
                        End If

                        GiasContext.CampionamentoConferito_TestataGriglia_Prodotti.Add(campConfTestataGrigliaProd)

                        GiasContext.SaveChanges()
                        success = True
                    Catch ex As Exception
                        Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                    End Try

                End Using
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return campConfTestataGrigliaProd

    End Function

    Public Function Aggiorna_TestataGriglia_Calibri(ByVal piva As String,
                                                    ByVal Id_TestataGriglia As Integer,
                                                    ByVal EFArrayToInsert As ArrayList,
                                                    ByVal EFArrayToUpdate As ArrayList,
                                                    ByVal EFArrayToDelete As ArrayList,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As String

        Dim messaggioErrore As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Aggiorna_TestataGriglia_Calibri()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each campConfTestataGriglia_Calibri As CampionamentoConferito_TestataGriglia_Calibri In EFArrayToInsert
                        success = False
                        For i As Integer = 0 To retries - 1
                            Try
                                'Richiedo un nuovo id sequenza
                                idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "campionamentoconferito_testatagriglia_calibri", 0, 2000000000, objParametri)
                                campConfTestataGriglia_Calibri.Id_Calibro = idSeq

                                GiasContext.CampionamentoConferito_TestataGriglia_Calibri.Add(campConfTestataGriglia_Calibri)
                                GiasContext.SaveChanges()
                                success = True
                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            messaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next

                    If success Then
                        For Each campConfTestataGriglia_Calibri As CampionamentoConferito_TestataGriglia_Calibri In EFArrayToUpdate
                            GiasContext.CampionamentoConferito_TestataGriglia_Calibri.Attach(campConfTestataGriglia_Calibri)
                            GiasContext.Entry(campConfTestataGriglia_Calibri).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each campConfTestataGriglia_Calibri As CampionamentoConferito_TestataGriglia_Calibri In EFArrayToDelete
                            GiasContext.CampionamentoConferito_TestataGriglia_Calibri.Attach(campConfTestataGriglia_Calibri)
                            GiasContext.CampionamentoConferito_TestataGriglia_Calibri.Remove(campConfTestataGriglia_Calibri)
                            GiasContext.SaveChanges()
                        Next


                        ' Dopo il salvataggio rileggo tutte le righe e le risalvo rinumerando il campo sequenza
                        Dim CalibriCampionamento =
                        From calibri_campionam In GiasContext.CampionamentoConferito_TestataGriglia_Calibri
                        Where calibri_campionam.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                              calibri_campionam.PIVA.Equals(piva) AndAlso
                              calibri_campionam.Id_TestataGriglia = Id_TestataGriglia
                        Order By calibri_campionam.Ordinamento, calibri_campionam.Descr_qualita, calibri_campionam.Descr_calibro
                        Select
                            calibri_campionam

                        Dim indice As Integer = 0

                        For Each campConf As CampionamentoConferito_TestataGriglia_Calibri In CalibriCampionamento
                            indice += 10
                            campConf.Ordinamento = indice
                        Next

                        GiasContext.SaveChanges()
                        ' COMMIT Effettivo
                        scope.Complete()

                    End If
                End Using
            End Using
        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function


    Public Function Scrivi_TestataGriglia_Calibri(ByVal piva As String,
                                                    ByVal Id_TestataGriglia As Integer,
                                                    ByVal campConfTestataGriglia_Calibri As CampionamentoConferito_TestataGriglia_Calibri,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As CampionamentoConferito_TestataGriglia_Calibri

        Dim messaggioErrore As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Scrivi_TestataGriglia_Calibri()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    success = False
                    For i As Integer = 0 To retries - 1
                        Try
                            'Richiedo un nuovo id sequenza
                            idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "campionamentoconferito_testatagriglia_calibri", 0, 2000000000, objParametri)
                            campConfTestataGriglia_Calibri.Id_Calibro = idSeq

                            If piva <> "" Then
                                campConfTestataGriglia_Calibri.PIVA = piva
                            End If

                            If Id_TestataGriglia <> 0 Then
                                campConfTestataGriglia_Calibri.Id_TestataGriglia = Id_TestataGriglia
                            End If

                            GiasContext.CampionamentoConferito_TestataGriglia_Calibri.Add(campConfTestataGriglia_Calibri)
                            GiasContext.SaveChanges()
                            success = True
                            Exit For
                        Catch ex As Exception
                            Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                        End Try
                    Next
                End Using
            End Using
        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return campConfTestataGriglia_Calibri

    End Function

    Public Function Aggiorna_Campioni_RigaConferimento(
                ByVal campConf_Mov As CampionamentoConferito_Movimenti,
                ByVal EFArrayToInsertUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreParametri
                ) As String

        Dim messaggioErrore As String = ""

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Aggiorna_Campioni_RigaConferimento()"

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                'Aggiorno testata movimento di campionamento
                Dim campConf_Movim_TestataElem =
                (From mov_righe In GiasContext.CampionamentoConferito_Movimenti
                 Where
                    (mov_righe.Piva_SuperUser.Equals(campConf_Mov.Piva_SuperUser)) AndAlso
                    (mov_righe.PIVA.Equals(campConf_Mov.PIVA)) AndAlso
                    (mov_righe.Id_Mov_Det.Equals(campConf_Mov.Id_Mov_Det)) AndAlso
                    (mov_righe.Id_TestataGriglia_Prod = campConf_Mov.Id_TestataGriglia_Prod)
                 Select mov_righe).FirstOrDefault()

                If campConf_Movim_TestataElem Is Nothing Then
                    GiasContext.CampionamentoConferito_Movimenti.Add(campConf_Mov)
                Else
                    campConf_Movim_TestataElem.Udm_QtaCampionata = campConf_Mov.Udm_QtaCampionata
                    campConf_Movim_TestataElem.QtaCampionata = campConf_Mov.QtaCampionata
                    campConf_Movim_TestataElem.StatoCampionamento = campConf_Mov.StatoCampionamento
                    campConf_Movim_TestataElem.Note = campConf_Mov.Note
                    campConf_Movim_TestataElem.Data_Modifica = campConf_Mov.Data_Modifica
                    campConf_Movim_TestataElem.Username_Modifica = campConf_Mov.Username_Modifica
                    GiasContext.CampionamentoConferito_Movimenti.Attach(campConf_Movim_TestataElem)
                    GiasContext.Entry(campConf_Movim_TestataElem).State = EntityState.Modified
                End If

                'Aggiorno righe movimento di campionamento
                For Each campConf_Movim_Righe As CampionamentoConferito_Movimenti_Righe In EFArrayToInsertUpdate

                    Dim campConf_Movim_Righe_ToUpdateElem =
                   (From mov_righe In GiasContext.CampionamentoConferito_Movimenti_Righe
                    Where
                       (mov_righe.Piva_SuperUser.Equals(campConf_Movim_Righe.Piva_SuperUser)) _
                      AndAlso
                      (mov_righe.PIVA.Equals(campConf_Movim_Righe.PIVA)) _
                      AndAlso
                       (mov_righe.Id_Mov_Det.Equals(campConf_Movim_Righe.Id_Mov_Det)) _
                      AndAlso
                       (mov_righe.Id_TestataGriglia_Prod = campConf_Movim_Righe.Id_TestataGriglia_Prod) _
                        AndAlso
                       (mov_righe.Id_Calibro = campConf_Movim_Righe.Id_Calibro)
                    Select mov_righe).FirstOrDefault()

                    If campConf_Movim_Righe_ToUpdateElem Is Nothing Then
                        GiasContext.CampionamentoConferito_Movimenti_Righe.Add(campConf_Movim_Righe)
                    Else
                        campConf_Movim_Righe_ToUpdateElem.PercentualeCampionato = campConf_Movim_Righe.PercentualeCampionato
                        campConf_Movim_Righe_ToUpdateElem.Data_Modifica = campConf_Movim_Righe.Data_Modifica
                        campConf_Movim_Righe_ToUpdateElem.Username_Modifica = campConf_Movim_Righe.Username_Modifica
                        GiasContext.CampionamentoConferito_Movimenti_Righe.Attach(campConf_Movim_Righe_ToUpdateElem)
                        GiasContext.Entry(campConf_Movim_Righe_ToUpdateElem).State = EntityState.Modified
                    End If
                Next

                For Each campConf_Movim_Righe As CampionamentoConferito_Movimenti_Righe In EFArrayToDelete
                    ' In questo caso per la Delete rileggo anziché fare l'attach perché anche le righe dove
                    ' c'è 0 come % campionato potrebbero arrivare come ricalcolate causa ricalcoli di riga

                    Dim campConf_Movim_Righe_ToDeleteElem =
                    (From mov_righe In GiasContext.CampionamentoConferito_Movimenti_Righe
                     Where
                        (mov_righe.Piva_SuperUser.Equals(campConf_Movim_Righe.Piva_SuperUser)) AndAlso
                        (mov_righe.PIVA.Equals(campConf_Movim_Righe.PIVA)) AndAlso
                        (mov_righe.Id_Mov_Det.Equals(campConf_Movim_Righe.Id_Mov_Det)) AndAlso
                        (mov_righe.Id_TestataGriglia_Prod = campConf_Movim_Righe.Id_TestataGriglia_Prod) AndAlso
                        (mov_righe.Id_Calibro = campConf_Movim_Righe.Id_Calibro)
                     Select mov_righe).FirstOrDefault()

                    If campConf_Movim_Righe_ToDeleteElem IsNot Nothing Then
                        GiasContext.CampionamentoConferito_Movimenti_Righe.Remove(campConf_Movim_Righe_ToDeleteElem)
                    End If
                Next

                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    Public Function Aggiorna_ID_TestataGriglia_Prod_Su_Campioni_RigaConferimento(
                ByVal campConf_Mov As CampionamentoConferito_Movimenti,
                ByVal ID_TestataGriglia_Prod_New As Integer,
                ByRef objParametri As AgronicaCoreParametri
                ) As String

        Dim messaggioErrore As String = ""

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Aggiorna_ID_TestataGriglia_Prod_Su_Campioni_RigaConferimento()"

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    'Aggiorno testata movimento di campionamento
                    Dim campConf_Movim_TestataElem =
                    (From mov_testata In GiasContext.CampionamentoConferito_Movimenti
                     Where
                        (mov_testata.Piva_SuperUser.Equals(campConf_Mov.Piva_SuperUser)) AndAlso
                        (mov_testata.PIVA.Equals(campConf_Mov.PIVA)) AndAlso
                        (mov_testata.Id_Mov_Det.Equals(campConf_Mov.Id_Mov_Det)) AndAlso
                        (mov_testata.Id_TestataGriglia_Prod = campConf_Mov.Id_TestataGriglia_Prod)
                     Select mov_testata).FirstOrDefault()

                    Dim campConf_Movim_Righe =
                    (From mov_righe In GiasContext.CampionamentoConferito_Movimenti_Righe
                     Where
                        (mov_righe.Piva_SuperUser.Equals(campConf_Movim_TestataElem.Piva_SuperUser)) AndAlso
                        (mov_righe.PIVA.Equals(campConf_Movim_TestataElem.PIVA)) AndAlso
                        (mov_righe.Id_Mov_Det.Equals(campConf_Movim_TestataElem.Id_Mov_Det)) AndAlso
                        (mov_righe.Id_TestataGriglia_Prod = campConf_Movim_TestataElem.Id_TestataGriglia_Prod)
                     Select mov_righe).ToList

                    If campConf_Movim_TestataElem Is Nothing Then
                        Throw New Exception("Testata griglia di campionamento non trovata durante aggiornamento id griglia prodotto. <br/> Contattare l'assistenza")
                    Else

                        ' Essendo un campo chiave devo cancellare e reinserire testata e righe
                        GiasContext.CampionamentoConferito_Movimenti.Remove(campConf_Movim_TestataElem)

                        For Each campConf_Riga In campConf_Movim_Righe
                            GiasContext.CampionamentoConferito_Movimenti_Righe.Remove(campConf_Riga)
                        Next
                        GiasContext.SaveChanges()

                        campConf_Movim_TestataElem.Id_TestataGriglia_Prod = ID_TestataGriglia_Prod_New
                        GiasContext.CampionamentoConferito_Movimenti.Add(campConf_Movim_TestataElem)

                        For Each campConf_Riga In campConf_Movim_Righe
                            campConf_Riga.Id_TestataGriglia_Prod = ID_TestataGriglia_Prod_New
                            GiasContext.CampionamentoConferito_Movimenti_Righe.Add(campConf_Riga)
                        Next

                        GiasContext.SaveChanges()

                        ' COMMIT Effettivo
                        scope.Complete()

                    End If

                End Using
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    Public Function Cancella_CampionamentoRigaConferito(ByVal TestataElem As CampionamentoConferito_Movimenti,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Cancella_CampionamentoRigaConferito()"
        Dim messaggioErrore As String = ""

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                GiasContext.CampionamentoConferito_Movimenti.Attach(TestataElem)
                GiasContext.CampionamentoConferito_Movimenti.Remove(TestataElem)

                Dim RigheElem =
                (From gconf_mov_righe In GiasContext.CampionamentoConferito_Movimenti_Righe
                 Where
                   gconf_mov_righe.Piva_SuperUser.Equals(TestataElem.Piva_SuperUser) AndAlso
                   gconf_mov_righe.PIVA.Equals(TestataElem.PIVA) AndAlso
                   gconf_mov_righe.Id_Mov_Det = TestataElem.Id_Mov_Det
                 Select gconf_mov_righe).ToList()

                For Each riga In RigheElem
                    GiasContext.CampionamentoConferito_Movimenti_Righe.Remove(riga)
                Next

                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    Public Function Aggiorna_Listini_CampionamentoConferito_Dettagli(
                ByVal listini_campConf_prodotti As Listini_CampionamentoConferito_Prodotti,
                ByVal validoDalDateTimeOrigine As DateTime,
                ByVal validoAlDateTimeOrigine As DateTime,
                ByVal EFArrayToInsertUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByVal EFArrayRigheProdottiToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreParametri
                ) As String

        '  N.B.
        '  Le letture vengono sempre fatte con le date originarie (validoDalDateTimeOrigine e validoAlDateTimeOrigine) perché sono campi chiave

        Dim messaggioErrore As String = ""

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Aggiorna_Listini_CampionamentoConferito_Dettagli()"

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each list_prodotti As Listini_CampionamentoConferito_Prodotti In EFArrayRigheProdottiToDelete
                    GiasContext.Listini_CampionamentoConferito_Prodotti.Attach(list_prodotti)
                    GiasContext.Listini_CampionamentoConferito_Prodotti.Remove(list_prodotti)
                Next

                Dim esisteUnaRiga As Boolean = False

                For Each listini_campConf_dettagli As Listini_CampionamentoConferito_Dettagli In EFArrayToInsertUpdate

                    esisteUnaRiga = True

                    Dim listini_dettagli_ToUpdateElem =
               (From list_dettagli In GiasContext.Listini_CampionamentoConferito_Dettagli
                Where
                    list_dettagli.Piva_SuperUser.Equals(listini_campConf_dettagli.Piva_SuperUser) AndAlso
                    list_dettagli.PIVA.Equals(listini_campConf_dettagli.PIVA) AndAlso
                    list_dettagli.Listino_Cod = listini_campConf_dettagli.Listino_Cod AndAlso
                    list_dettagli.Id_TestataGriglia_Prod = listini_campConf_dettagli.Id_TestataGriglia_Prod AndAlso
                    list_dettagli.Id_Calibro = listini_campConf_dettagli.Id_Calibro AndAlso
                    list_dettagli.Validita_Inizio = validoDalDateTimeOrigine AndAlso
                    list_dettagli.Validita_Fine = validoAlDateTimeOrigine
                Select list_dettagli).FirstOrDefault()

                    If listini_dettagli_ToUpdateElem Is Nothing Then
                        GiasContext.Listini_CampionamentoConferito_Dettagli.Add(listini_campConf_dettagli)
                    Else
                        GiasContext.Listini_CampionamentoConferito_Dettagli.Remove(listini_dettagli_ToUpdateElem)
                        GiasContext.Listini_CampionamentoConferito_Dettagli.Add(listini_campConf_dettagli)
                    End If
                Next

                For Each listini_campConf_dettagli_del As Listini_CampionamentoConferito_Dettagli In EFArrayToDelete
                    Dim listini_dettagli_ToDelete_Elem =
                (From list_dettagli In GiasContext.Listini_CampionamentoConferito_Dettagli
                 Where
                    list_dettagli.Piva_SuperUser.Equals(listini_campConf_dettagli_del.Piva_SuperUser) AndAlso
                    list_dettagli.PIVA.Equals(listini_campConf_dettagli_del.PIVA) AndAlso
                    list_dettagli.Listino_Cod = listini_campConf_dettagli_del.Listino_Cod AndAlso
                    list_dettagli.Id_TestataGriglia_Prod = listini_campConf_dettagli_del.Id_TestataGriglia_Prod AndAlso
                    list_dettagli.Id_Calibro = listini_campConf_dettagli_del.Id_Calibro AndAlso
                    list_dettagli.Validita_Inizio = validoDalDateTimeOrigine AndAlso
                    list_dettagli.Validita_Fine = validoAlDateTimeOrigine
                 Select list_dettagli).FirstOrDefault()

                    If listini_dettagli_ToDelete_Elem IsNot Nothing Then
                        GiasContext.Listini_CampionamentoConferito_Dettagli.Remove(listini_dettagli_ToDelete_Elem)
                    End If
                Next

                'Aggiorno testata 
                Dim lst_campConf_prodottiElem =
                (From listini_campConf_prod In GiasContext.Listini_CampionamentoConferito_Prodotti
                 Where
                        listini_campConf_prod.Piva_SuperUser = listini_campConf_prodotti.Piva_SuperUser AndAlso
                        listini_campConf_prod.PIVA = listini_campConf_prodotti.PIVA AndAlso
                        listini_campConf_prod.Listino_Cod = listini_campConf_prodotti.Listino_Cod AndAlso
                        listini_campConf_prod.Id_TestataGriglia_Prod = listini_campConf_prodotti.Id_TestataGriglia_Prod AndAlso
                        listini_campConf_prod.Validita_Inizio = validoDalDateTimeOrigine AndAlso
                        listini_campConf_prod.Validita_Fine = validoAlDateTimeOrigine
                 Select listini_campConf_prod).FirstOrDefault()

                If lst_campConf_prodottiElem IsNot Nothing Then
                    If esisteUnaRiga Then
                        If lst_campConf_prodottiElem.Piva_SuperUser <> listini_campConf_prodotti.Piva_SuperUser OrElse
                           lst_campConf_prodottiElem.PIVA <> listini_campConf_prodotti.PIVA OrElse
                           lst_campConf_prodottiElem.Listino_Cod <> listini_campConf_prodotti.Listino_Cod OrElse
                           lst_campConf_prodottiElem.Id_TestataGriglia_Prod <> listini_campConf_prodotti.Id_TestataGriglia_Prod OrElse
                           lst_campConf_prodottiElem.Validita_Inizio <> listini_campConf_prodotti.Validita_Inizio OrElse
                           lst_campConf_prodottiElem.Validita_Fine <> listini_campConf_prodotti.Validita_Fine Then
                            GiasContext.Listini_CampionamentoConferito_Prodotti.Remove(lst_campConf_prodottiElem)
                            GiasContext.Listini_CampionamentoConferito_Prodotti.Add(listini_campConf_prodotti)
                        End If
                    Else
                        'Non sono state aggiunte / modificate righe: verifico se il nr di righe esistenti è = a quelle cancellate e 
                        ' se sì cancello la testata
                        Dim lst_campConf_prodotti_righe =
                            (From listini_campConf_prodotti_righe In GiasContext.Listini_CampionamentoConferito_Dettagli
                             Where
                                listini_campConf_prodotti_righe.Piva_SuperUser = listini_campConf_prodotti.Piva_SuperUser AndAlso
                                listini_campConf_prodotti_righe.PIVA = listini_campConf_prodotti.PIVA AndAlso
                                listini_campConf_prodotti_righe.Listino_Cod = listini_campConf_prodotti.Listino_Cod AndAlso
                                listini_campConf_prodotti_righe.Id_TestataGriglia_Prod = listini_campConf_prodotti.Id_TestataGriglia_Prod AndAlso
                                listini_campConf_prodotti_righe.Validita_Inizio = validoDalDateTimeOrigine AndAlso
                                listini_campConf_prodotti_righe.Validita_Fine = validoAlDateTimeOrigine
                             Select listini_campConf_prodotti_righe)

                        If lst_campConf_prodotti_righe.Count() = EFArrayToDelete.Count Then
                            GiasContext.Listini_CampionamentoConferito_Prodotti.Remove(lst_campConf_prodottiElem)
                        End If
                    End If
                Else
                    ' Nuovo inserimento: creo la testata
                    If esisteUnaRiga Then
                        'Esistono righe, creo la testata
                        GiasContext.Listini_CampionamentoConferito_Prodotti.Add(listini_campConf_prodotti)
                    Else
                        ' Non esistono righe e non esisteva la testata ... non deve mai capitare
                    End If
                End If

                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    Public Function Aggiorna_Listini_PrezzoSuConferito(ByVal piva As String,
                                                       ByVal EFArrayToInsert As ArrayList,
                                                       ByVal EFArrayToUpdate As ArrayList,
                                                       ByVal EFArrayToDelete As ArrayList,
                                                       ByVal EFArrayRigheProdottiToDelete As ArrayList,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As String

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Aggiorna_TestataGriglia_Campionamento()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' Contiene anche i dettagli
                For Each list_prodotti As Listini_CampionamentoConferito_Prodotti In EFArrayRigheProdottiToDelete
                    GiasContext.Listini_CampionamentoConferito_Prodotti.Attach(list_prodotti)
                    GiasContext.Listini_CampionamentoConferito_Prodotti.Remove(list_prodotti)
                Next

                For Each list_prodotti As Listini_CampionamentoConferito_Prodotti In EFArrayToInsert
                    GiasContext.Listini_CampionamentoConferito_Prodotti.Add(list_prodotti)
                Next

                For Each list_prodotti As Listini_CampionamentoConferito_Prodotti In EFArrayToUpdate
                    GiasContext.Listini_CampionamentoConferito_Prodotti.Attach(list_prodotti)
                    GiasContext.Entry(list_prodotti).State = EntityState.Modified
                Next

                For Each list_prodotti As Listini_CampionamentoConferito_Prodotti In EFArrayToDelete
                    GiasContext.Listini_CampionamentoConferito_Prodotti.Attach(list_prodotti)
                    GiasContext.Listini_CampionamentoConferito_Prodotti.Remove(list_prodotti)
                Next

                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    Public Function Aggiorna_Listini_Prodotti_Equivalenti(ByVal EFArrayToInsert As ArrayList,
                                                          ByVal EFArrayToDelete As ArrayList,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Aggiorna_Listini_Prodotti_Equivalenti()"
        Dim messaggioErrore As String = ""

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' In entrambe i casi rileggo perché sulla griglia potrei aver cliccato su un check e poi tolto il check o viceversa
                ' e in questo caso l'elemento mi arriva come modificato anche se in effetti non è successo nulla
                For Each campConfTestataGriglia_Prodotti_Equivalenti_Insert As Listini_CampionamentoConferito_Prodotti_Equivalenti In EFArrayToInsert
                    Dim campConf_TestataGriglia_Prod_Equiv =
                (From griglia_testata_prod_equiv In GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti
                 Where
                   (griglia_testata_prod_equiv.Piva_SuperUser.Equals(campConfTestataGriglia_Prodotti_Equivalenti_Insert.Piva_SuperUser)) _
                   AndAlso
                   (griglia_testata_prod_equiv.PIVA.Equals(campConfTestataGriglia_Prodotti_Equivalenti_Insert.PIVA)) _
                   AndAlso
                   (griglia_testata_prod_equiv.Listino_Cod = campConfTestataGriglia_Prodotti_Equivalenti_Insert.Listino_Cod) _
                   AndAlso
                   (griglia_testata_prod_equiv.Id_TestataGriglia_Prod = campConfTestataGriglia_Prodotti_Equivalenti_Insert.Id_TestataGriglia_Prod) _
                      AndAlso
                   (griglia_testata_prod_equiv.Id_TestataGriglia_Prod_Equivalente = campConfTestataGriglia_Prodotti_Equivalenti_Insert.Id_TestataGriglia_Prod_Equivalente))

                    If campConf_TestataGriglia_Prod_Equiv.Count = 0 Then
                        GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Add(campConfTestataGriglia_Prodotti_Equivalenti_Insert)
                    End If

                Next

                For Each campConfTestataGriglia_Prodotti_Equivalente As Listini_CampionamentoConferito_Prodotti_Equivalenti In EFArrayToDelete
                    Dim campConf_TestataGriglia_Prod_Equiv =
                (From griglia_testata_prod_equiv In GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti
                 Where
                   (griglia_testata_prod_equiv.Piva_SuperUser.Equals(campConfTestataGriglia_Prodotti_Equivalente.Piva_SuperUser)) _
                   AndAlso
                   (griglia_testata_prod_equiv.PIVA.Equals(campConfTestataGriglia_Prodotti_Equivalente.PIVA)) _
                   AndAlso
                     (griglia_testata_prod_equiv.Listino_Cod = campConfTestataGriglia_Prodotti_Equivalente.Listino_Cod) _
                   AndALso
                   (griglia_testata_prod_equiv.Id_TestataGriglia_Prod = campConfTestataGriglia_Prodotti_Equivalente.Id_TestataGriglia_Prod) _
                     AndAlso
                   (griglia_testata_prod_equiv.Id_TestataGriglia_Prod_Equivalente = campConfTestataGriglia_Prodotti_Equivalente.Id_TestataGriglia_Prod_Equivalente)).FirstOrDefault()

                    If campConf_TestataGriglia_Prod_Equiv IsNot Nothing Then
                        GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Remove(campConf_TestataGriglia_Prod_Equiv)
                    End If

                Next

                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function


    Public Function CancellaPrezziListinoConferimento(ByVal piva As String,
                                                      ByVal key_Listino_Cod As Integer,
                                                      ByVal key_Id_TestataGriglia_Prod As Integer,
                                                      ByVal validoDal As DateTime,
                                                      ByVal validoAl As DateTime,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As String

        Dim messaggioErrore As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.CancellaPrezziListinoConferimento()"

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                'Testata
                Dim TestataElem =
                (From test In GiasContext.Listini_CampionamentoConferito_Prodotti.Include("Listini_CampionamentoConferito_Dettagli")
                 Where test.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                       test.PIVA.Equals(piva) AndAlso
                       test.Listino_Cod = key_Listino_Cod AndAlso
                       test.Id_TestataGriglia_Prod = key_Id_TestataGriglia_Prod AndAlso
                       test.Validita_Inizio = validoDal AndAlso
                       test.Validita_Fine = validoAl).FirstOrDefault()
                If TestataElem IsNot Nothing Then
                    GiasContext.Listini_CampionamentoConferito_Prodotti.Remove(TestataElem)
                End If

                'Prodotti con stesso prezzo: li cancello se non ci sono rimaste righe di listino

                ' SOSPESO IL 7/6/2017 LA PARTE SOTTO E' CMQ FUNZIONANTE QUINDI SE SI VOGLIONO CANCELLARE
                ' BASTA DISASTERISCARE
                'Dim TestataElemRimasto =
                '(From test In GiasContext.Listini_CampionamentoConferito_Prodotti
                ' Where
                '   test.Piva_SuperUser.Equals(Piva_SuperUser) And
                '   test.PIVA.Equals(piva) And
                '   test.Listino_Cod = key_Listino_Cod And
                '   test.Id_TestataGriglia = key_Id_TestataGriglia And
                '   test.Mat_Cod = key_Mat_Cod And
                '   (test.Validita_Inizio <> validoDal Or
                '   test.Validita_Fine <> validoAl)).FirstOrDefault()
                'If TestataElemRimasto Is Nothing Then
                '    Dim ProdEquiv =
                '    (From prod_equiv In GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti
                '     Where
                '        prod_equiv.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                '        prod_equiv.PIVA.Equals(piva) AndAlso
                '        prod_equiv.Listino_Cod = key_Listino_Cod AndAlso
                '        prod_equiv.Id_TestataGriglia = key_Id_TestataGriglia AndAlso
                '        prod_equiv.Mat_Cod = key_Mat_Cod).ToList()

                '    For Each pr_equiv In ProdEquiv
                '        GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Remove(pr_equiv)
                '    Next
                'End If
                ' FINE SOSPESO IL 7/6/2017


                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    Public Function Aggiorna_Fattori_Variazione_ParametriQualitativi(
                ByVal piva As String,
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreParametri
                ) As String

        Dim messaggioErrore As String = ""

        Dim ObjSequenze = New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Aggiorna_Fattori_Variazione_ParametriQualitativi()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each campConfFattVarParamQual As CampionamentoConferito_Fattori_Variazione_ParamQualitativi In EFArrayToInsert
                        success = False
                        For i As Integer = 0 To retries - 1
                            Try
                                'Richiedo un nuovo id sequenza
                                idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "campionamentoconferito_fattori_variazione_paramqualitativi", 0, 2000000000, objParametri)
                                campConfFattVarParamQual.Id_fattore_variazione = idSeq
                                GiasContext.CampionamentoConferito_Fattori_Variazione_ParamQualitativi.Add(campConfFattVarParamQual)

                                GiasContext.SaveChanges()
                                success = True
                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            messaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next

                    If success Then
                        For Each campConfFattVarParamQual As CampionamentoConferito_Fattori_Variazione_ParamQualitativi In EFArrayToUpdate
                            GiasContext.CampionamentoConferito_Fattori_Variazione_ParamQualitativi.Attach(campConfFattVarParamQual)
                            GiasContext.Entry(campConfFattVarParamQual).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each campConfFattVarParamQual As CampionamentoConferito_Fattori_Variazione_ParamQualitativi In EFArrayToDelete
                            GiasContext.CampionamentoConferito_Fattori_Variazione_ParamQualitativi.Attach(campConfFattVarParamQual)
                            GiasContext.CampionamentoConferito_Fattori_Variazione_ParamQualitativi.Remove(campConfFattVarParamQual)
                            GiasContext.SaveChanges()
                        Next

                        ' COMMIT Effettivo
                        scope.Complete()
                    End If
                End Using
            End Using
        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    '##############################################################################################

    Public Function Aggiorna_Valori_Fattori_Variazione(ByVal piva As String,
                                                       ByVal EFArrayToInsert As ArrayList,
                                                       ByVal EFArrayToUpdate As ArrayList,
                                                       ByVal EFArrayToDelete As ArrayList,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As String

        Dim ObjSequenze = New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Aggiorna_Valori_Fattori_Variazione()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each listFattVar As Listini_CampionamentoConferito_Fattori_Variazione In EFArrayToInsert
                        success = False
                        For i As Integer = 0 To retries - 1
                            Try
                                'Richiedo un nuovo id sequenza
                                idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "Listini_CampionamentoConferito_Fattori_Variazione", 0, 2000000000, objParametri)
                                listFattVar.Id_listino_fattore_variaz = idSeq
                                GiasContext.Listini_CampionamentoConferito_Fattori_Variazione.Add(listFattVar)

                                GiasContext.SaveChanges()
                                success = True
                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            messaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next

                    If success Then
                        For Each listFattVar As Listini_CampionamentoConferito_Fattori_Variazione In EFArrayToUpdate
                            GiasContext.Listini_CampionamentoConferito_Fattori_Variazione.Attach(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar As Listini_CampionamentoConferito_Fattori_Variazione In EFArrayToDelete
                            GiasContext.Listini_CampionamentoConferito_Fattori_Variazione.Attach(listFattVar)
                            GiasContext.Listini_CampionamentoConferito_Fattori_Variazione.Remove(listFattVar)

                            ' Rileggo l'elemento per poter cancellare anche i calibri esclusi collegati
                            Dim campConf_R As New FF_CampionamentoConferimento_R
                            Dim listEsclFattVar = campConf_R.Leggi_Listini_Esclusione_Fattore_Variazione(listFattVar, objParametri)
                            If listEsclFattVar IsNot Nothing Then
                                For Each listEsclFattVarElem In listEsclFattVar
                                    GiasContext.Listini_CampionamentoConferito_Esclusione_Fattore_Variazione.Attach(listEsclFattVarElem)
                                    GiasContext.Listini_CampionamentoConferito_Esclusione_Fattore_Variazione.Remove(listEsclFattVarElem)
                                Next
                            End If

                            GiasContext.SaveChanges()
                        Next

                        ' COMMIT Effettivo
                        scope.Complete()
                    End If

                End Using
            End Using
        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    '##############################################################################################

    Public Function AggiornaFattoriVariazioneSuRigheConferimento(ByVal piva As String,
                                                                 ByVal EFArrayToInsert As ArrayList,
                                                                 ByVal EFArrayToUpdate As ArrayList,
                                                                 ByVal EFArrayToDelete As ArrayList,
                                                                 ByRef objParametri As AgronicaCoreParametri
                                                                 ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.AggiornaFattoriVariazioneSuRigheConferimento()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each fattVarConf As CampionamentoConferito_Movimenti In EFArrayToInsert
                    GiasContext.CampionamentoConferito_Movimenti.Add(fattVarConf)
                Next

                For Each fattVarConf As CampionamentoConferito_Movimenti In EFArrayToUpdate
                    GiasContext.CampionamentoConferito_Movimenti.Attach(fattVarConf)
                    GiasContext.Entry(fattVarConf).State = EntityState.Modified
                Next

                For Each fattVarConf As CampionamentoConferito_Movimenti In EFArrayToDelete
                    GiasContext.CampionamentoConferito_Movimenti.Attach(fattVarConf)
                    GiasContext.CampionamentoConferito_Movimenti.Remove(fattVarConf)
                Next

                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    Public Function Aggiorna_Esclusione_Fattori_Variazione(ByVal EFArrayToInsert As ArrayList,
                                                           ByVal EFArrayToDelete As ArrayList,
                                                           ByRef objParametri As AgronicaCoreParametri
                                                           ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Aggiorna_Esclusione_Fattori_Variazione()"
        Dim messaggioErrore As String = ""

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' In entrambe i casi rileggo perché sulla griglia potrei aver cliccato su un check e poi tolto il check o viceversa
                ' e in questo caso l'elemento mi arriva come modificato anche se in effetti non è successo nulla
                For Each esclusione_Insert As Listini_CampionamentoConferito_Esclusione_Fattore_Variazione In EFArrayToInsert
                    Dim esclusione =
                (From escl In GiasContext.Listini_CampionamentoConferito_Esclusione_Fattore_Variazione
                 Where
                   (escl.Piva_SuperUser.Equals(esclusione_Insert.Piva_SuperUser)) AndAlso
                   (escl.PIVA.Equals(esclusione_Insert.PIVA)) AndAlso
                   (escl.Id_listino_fattore_variaz = esclusione_Insert.Id_listino_fattore_variaz) AndAlso
                   (escl.Id_Calibro = esclusione_Insert.Id_Calibro))

                    If esclusione.Count = 0 Then
                        GiasContext.Listini_CampionamentoConferito_Esclusione_Fattore_Variazione.Add(esclusione_Insert)
                    End If

                Next

                For Each esclusione_Delete As Listini_CampionamentoConferito_Esclusione_Fattore_Variazione In EFArrayToDelete
                    Dim esclusione =
                (From escl In GiasContext.Listini_CampionamentoConferito_Esclusione_Fattore_Variazione
                 Where
                   (escl.Piva_SuperUser.Equals(esclusione_Delete.Piva_SuperUser)) AndAlso
                   (escl.PIVA.Equals(esclusione_Delete.PIVA)) AndAlso
                   (escl.Id_listino_fattore_variaz = esclusione_Delete.Id_listino_fattore_variaz) AndAlso
                   (escl.Id_Calibro = esclusione_Delete.Id_Calibro)).FirstOrDefault()

                    If esclusione IsNot Nothing Then
                        GiasContext.Listini_CampionamentoConferito_Esclusione_Fattore_Variazione.Remove(esclusione)
                    End If

                Next

                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Public Function CancellaElemAnagAccontiLiquidazioni(ByVal Elem As AnagAccontiLiquidazioni_CampionamentoConferito,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.CancellaElemAnagAccontiLiquidazioni()"
        Dim messaggioErrore As String = ""

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito.Attach(Elem)
                GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito.Remove(Elem)
                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message & If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]")
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Public Function CancellaInteraLiquidazione(ByVal piva As String,
                                               ByVal idAnagrafica As Integer?,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.CancellaInteraLiquidazione()"
        Dim messaggioErrore As String = ""
        Dim pivaSuperUser As String = objParametri.PivaSuperUser

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                GiasContext.Database.CommandTimeout = 3600

                'Se idAnagrafica = Nothing, non applica il filtro sull'idAnagrafica
                Dim objsToDelete = (From anag In GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito _
                                       .Include("AnagPercentAccontiPerGrpFatt_CampionamentoConferito") _
                                       .Include("Liquid_Mov_Dati_Generali_CampionamentoConferito") _
                                       .Include("Liquid_Mov_Dati_Generali_CampionamentoConferito.Liquid_Mov_CampionamentoConferito") _
                                       .Include("Liquid_Mov_Dati_Generali_CampionamentoConferito.Liquid_Mov_CampionamentoConferito.Liquid_Mov_PerCalibro_CampionamentoConferito") _
                                       .Include("Liquid_Mov_Dati_Generali_CampionamentoConferito.Liquid_Mov_CampionamentoConferito.Liquid_Mov_FattVariaz_CampionamentoConferito") _
                                       .Include("Liquid_Mov_Trattenute_CampionamentoConferito")
                                    Where anag.Piva_SuperUser = pivaSuperUser AndAlso
                                         anag.PIVA = piva AndAlso
                                         anag.id_anagrafica = If(idAnagrafica Is Nothing, anag.id_anagrafica, CInt(idAnagrafica))
                                    Select anag).ToList()

                For Each objToDelete In objsToDelete
                    GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito.Remove(objToDelete)
                Next

                Dim k = GiasContext.SaveChanges()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message & If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]")
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Public Function InserisciAnagAccontiLiquidazioni(ByVal piva As String,
                                                     ByVal Elem As AnagAccontiLiquidazioni_CampionamentoConferito,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.InserisciAnagAccontiLiquidazioni()"
        Dim messaggioErrore As String = ""
        Dim ObjSequenze = New Agro_Sequenze

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try
            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "AnagAccontiLiquidazioni_CampionamentoConferito", 0, 2000000000, objParametri)
                    
                    Elem.Piva_SuperUser = objParametri.PivaSuperUser
                    Elem.PIVA = piva
                    Elem.id_anagrafica = idSeq
                    Elem.Data_Creazione = Date.Now
                    Elem.Username_Creazione = objParametri.UsernameOperazione
                    Elem.Data_Modifica = Date.Now
                    Elem.Username_Modifica = objParametri.UsernameOperazione

                    'Necessaria alla copia dell'oggetto:
                    'l'oggetto Elem corrisponde all'oggetto EF di partenza privato della sua chiave primaria, di conseguenza, impostandone lo stato interno
                    'ad Added notifico ad EF di voler inserire un nuovo elemento.
                    'Questa istruzione in caso di inserimento di elemento da zero è sostanzialmente superflua perché in quel caso lo stato dell'oggetto è già Added
                    GiasContext.Entry(Elem).State = EntityState.Added

                    GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito.Add(Elem)

                    GiasContext.SaveChanges()

                    ' COMMIT Effettivo
                    scope.Complete()

                End Using

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Public Function ModificaAnagAccontiLiquidazioni(ByVal Elem As AnagAccontiLiquidazioni_CampionamentoConferito,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.ModificaAnagAccontiLiquidazioni()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Elem.Data_Modifica = Date.Now
                Elem.Username_Modifica = objParametri.UsernameOperazione

                GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito.Attach(Elem)
                GiasContext.Entry(Elem).State = EntityState.Modified
                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Public Function Aggiorna_Gruppi_Fatturazione_ParametriQualitativi(ByVal piva As String,
                                                                      ByVal EFArrayToInsert As ArrayList,
                                                                      ByVal EFArrayToUpdate As ArrayList,
                                                                      ByRef objParametri As AgronicaCoreParametri
                                                                      ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Aggiorna_Gruppi_Fatturazione_ParametriQualitativi()"
        Dim messaggioErrore As String = ""

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each mpd As Materie_Prime_Dettagli In EFArrayToInsert
                    success = False
                    For i As Integer = 0 To retries - 1
                        Try

                            GiasContext.Materie_Prime_Dettagli.Add(mpd)
                            GiasContext.SaveChanges()
                            success = True
                            Exit For

                        Catch ex As Exception
                            Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                        End Try
                    Next
                    ' Al primo errore evito di continuare le modifiche
                    If Not success Then
                        messaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                        Exit For
                    End If
                Next

                If success Then

                    For Each mpd As Materie_Prime_Dettagli In EFArrayToUpdate

                        GiasContext.Materie_Prime_Dettagli.Attach(mpd)
                        GiasContext.Entry(mpd).State = EntityState.Modified
                        GiasContext.SaveChanges()

                    Next

                End If

            End Using
        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    Public Function implo_ScriviLotto(ByVal lotto As cbl_Calibrature, ByRef objParametri As AgronicaCoreParametri) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.implo_ScriviLotto()"
        Dim messaggioErrore As String = ""

        Try
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                GiasContext.cbl_Calibrature.Attach(lotto)
                GiasContext.Entry(lotto).State = EntityState.Modified
                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Public Function AggiornaLottiDaImportare(ByVal EFArrayToUpdate As ArrayList,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.AggiornaLottiDaImportare()"
        Dim messaggioErrore As String = ""
        Dim campConf_R As New FF_CampionamentoConferimento_R

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each elem As cbl_Calibrature In EFArrayToUpdate

                    Dim lotto = campConf_R.implo_LeggiLotto(elem.ID, objParametri)
                    If lotto Is Nothing Then
                        Throw New Exception("Lotto campionamento con ID " & elem.ID.ToString() & " non trovato")
                    End If

                    lotto.Lotto = elem.Lotto
                    lotto.Varieta = elem.Varieta
                    lotto.Conferitore_Codice = elem.Conferitore_Codice
                    lotto.Conferitore_Nome = elem.Conferitore_Nome
                    lotto.Programma = elem.Programma
                    lotto.NrBolla = elem.NrBolla
                    lotto.RifBolla = elem.RifBolla
                    lotto.Data_Modifica = elem.Data_Modifica
                    lotto.Username_Modifica = elem.Username_Modifica

                    GiasContext.cbl_Calibrature.Attach(lotto)
                    GiasContext.Entry(lotto).State = EntityState.Modified

                Next

                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Public Function AggiornaDettaglioLottoDaImportare(ByVal lid As Integer,
                                                      ByVal EFArrayToUpdate As ArrayList,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.AggiornaDettaglioLottoDaImportare()"
        Dim messaggioErrore As String = ""

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim srcList = (From cal In GiasContext.cbl_CalibratureXCalibri
                                   Where cal.IDCalibro = lid
                                   Select cal).ToList()
                    'In EFArrayToUpdate sono valorizzate solo i campi:
                    ' .ID
                    ' .IDCalibro
                    ' .Nome
                    ' .Qualita
                    ' .Perc
                    ' .Peso

                    Dim srcCnt As Integer = srcList.Count
                    Dim updCnt As Integer = EFArrayToUpdate.Count
                    Dim minCnt As Integer = srcCnt
                    If minCnt > updCnt Then
                        minCnt = updCnt
                    End If

                    Dim srcCal As cbl_CalibratureXCalibri
                    Dim updCal As cbl_CalibratureXCalibri

                    Dim idx As Integer = 0
                    While idx < minCnt

                        srcCal = srcList.Item(idx)
                        updCal = EFArrayToUpdate.Item(idx)

                        If (Not String.Compare(srcCal.Qualita, updCal.Qualita) = 0 OrElse
                            Not String.Compare(srcCal.Nome, updCal.Nome) = 0 OrElse
                            srcCal.Perc <> updCal.Perc) Then

                            srcCal.Qualita = updCal.Qualita
                            srcCal.Nome = updCal.Nome
                            srcCal.Perc = updCal.Perc
                            srcCal.Peso = updCal.Peso

                            srcCal.Data_Modifica = Date.Now
                            srcCal.Username_Modifica = objParametri.UsernameOperazione

                            GiasContext.cbl_CalibratureXCalibri.Attach(srcCal)
                            GiasContext.Entry(srcCal).State = EntityState.Modified

                        End If

                        idx += 1

                    End While

                    'Se ci sono ancora degli elementi originali li devo cancellare
                    While idx < srcCnt

                        srcCal = srcList.Item(idx)

                        GiasContext.cbl_CalibratureXCalibri.Attach(srcCal)
                        GiasContext.cbl_CalibratureXCalibri.Remove(srcCal)

                        idx += 1

                    End While

                    idx = minCnt
                    If idx < updCnt Then

                        Dim lastID = (From cal In GiasContext.cbl_CalibratureXCalibri
                                      Select cal.ID).Max()
                        'Se ci sono ancora degli elementi da aggiornare li devo inserire
                        While idx < updCnt

                            lastID += 1

                            updCal = EFArrayToUpdate.Item(idx)

                            updCal.ID = lastID

                            updCal.inviato = 0
                            updCal.Data_Creazione = Date.Now
                            updCal.Username_Creazione = objParametri.UsernameOperazione
                            updCal.Data_Modifica = Date.Now
                            updCal.Username_Modifica = objParametri.UsernameOperazione
                            updCal.Validita_Inizio = AGRODATAINIZIO
                            updCal.Validita_Fine = AGRODATAFINE

                            idx += 1

                            GiasContext.cbl_CalibratureXCalibri.Add(updCal)

                        End While

                    End If

                    GiasContext.SaveChanges()

                    'COMMIT Effettivo
                    scope.Complete()

                End Using

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Public Function Elimina_Griglia_LottiDaImportare(ByVal piva As String,
                                                     ByVal EFArrayToDelete As ArrayList,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Elimina_Griglia_LottiDaImportare()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    For Each master As cbl_Calibrature In EFArrayToDelete

                        Dim Cal_ID = master.ID

                        Dim detail = (From calxcal In GiasContext.cbl_CalibratureXCalibri
                                      Where calxcal.IDCalibro = Cal_ID
                                      Select calxcal)

                        For Each elem As cbl_CalibratureXCalibri In detail
                            GiasContext.cbl_CalibratureXCalibri.Attach(elem)
                            GiasContext.cbl_CalibratureXCalibri.Remove(elem)
                        Next

                        Dim log_ = (From log_cal In GiasContext.cbl_LogImportazioni
                                    Where log_cal.idCalibratura = Cal_ID
                                    Select log_cal)

                        If log_.Any() Then
                            Dim log = log_.First()
                            GiasContext.cbl_LogImportazioni.Attach(log)
                            GiasContext.cbl_LogImportazioni.Remove(log)
                        End If

                        GiasContext.cbl_Calibrature.Attach(master)
                        GiasContext.cbl_Calibrature.Remove(master)

                        GiasContext.SaveChanges()

                    Next

                    ' COMIT Effettivo
                    scope.Complete()

                End Using
            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function


    Public Class MovimentoDaImportare
        Public Lotto As cbl_Calibrature
        Public Testata As CampionamentoConferito_Movimenti
        Public Righe As List(Of CampionamentoConferito_Movimenti_Righe)
    End Class


    Public Function Importa_LottiDaImportare(ByVal piva As String,
                                             ByVal EFArrayToImport As ArrayList,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Importa_LottiDaImportare()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Dim transactionOptions As New TransactionOptions With {
                .IsolationLevel = IsolationLevel.ReadCommitted,
                .Timeout = TransactionManager.MaximumTimeout
            }

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    For Each movimento As MovimentoDaImportare In EFArrayToImport

                        Dim testata = movimento.Testata
                        Dim testata2upd = (From mov In GiasContext.CampionamentoConferito_Movimenti
                                           Where mov.Piva_SuperUser.Equals(testata.Piva_SuperUser) AndAlso
                                                 mov.PIVA.Equals(testata.PIVA) AndAlso
                                                 mov.Id_Mov_Det = testata.Id_Mov_Det AndAlso
                                                 mov.Id_TestataGriglia_Prod = testata.Id_TestataGriglia_Prod
                                           Select mov).FirstOrDefault()

                        If testata2upd Is Nothing Then

                            'Inserisco la Testata
                            GiasContext.CampionamentoConferito_Movimenti.Add(testata)

                            'e tutte le Righe
                            For Each riga As CampionamentoConferito_Movimenti_Righe In movimento.Righe
                                GiasContext.CampionamentoConferito_Movimenti_Righe.Add(riga)
                            Next

                        Else

                            'devo fare la media pesata delle percentuali

                            Dim Qta0 As Decimal = testata2upd.QtaCampionata
                            Dim Qta1 As Decimal = movimento.Testata.QtaCampionata

                            testata2upd.Automatico = enum_TipoCampionamento.TC_MediaBolleImportate
                            testata2upd.QtaCampionata = Qta0 + Qta1
                            testata2upd.Data_Modifica = movimento.Testata.Data_Modifica
                            testata2upd.Username_Modifica = movimento.Testata.Username_Modifica

                            GiasContext.CampionamentoConferito_Movimenti.Attach(testata2upd)
                            GiasContext.Entry(testata2upd).State = EntityState.Modified

                            For Each mov_riga As CampionamentoConferito_Movimenti_Righe In movimento.Righe

                                Dim riga = mov_riga
                                Dim riga2upd = (From mov_righe In GiasContext.CampionamentoConferito_Movimenti_Righe
                                                Where mov_righe.Piva_SuperUser.Equals(riga.Piva_SuperUser) AndAlso
                                                      mov_righe.PIVA.Equals(riga.PIVA) AndAlso
                                                      mov_righe.Id_Mov_Det = riga.Id_Mov_Det AndAlso
                                                      mov_righe.Id_TestataGriglia_Prod = riga.Id_TestataGriglia_Prod AndAlso
                                                      mov_righe.Id_Calibro = riga.Id_Calibro
                                                Select mov_righe).FirstOrDefault()

                                If riga2upd Is Nothing Then
                                    'devo ricalcolare la percentuale
                                    riga.PercentualeCampionato = (Qta1 * riga.PercentualeCampionato) / (Qta0 + Qta1)

                                    GiasContext.CampionamentoConferito_Movimenti_Righe.Add(riga)
                                Else
                                    riga2upd.PercentualeCampionato = ((Qta0 * riga2upd.PercentualeCampionato) + (Qta1 * riga.PercentualeCampionato)) / (Qta0 + Qta1)
                                    riga2upd.Data_Modifica = riga.Data_Modifica
                                    riga2upd.Username_Modifica = riga.Username_Modifica

                                    GiasContext.CampionamentoConferito_Movimenti_Righe.Attach(riga2upd)
                                    GiasContext.Entry(riga2upd).State = EntityState.Modified
                                End If

                            Next

                        End If

                        'Devo anche modificare il lotto importato cambiandogli stato...
                        movimento.Lotto.Stato = statoImportazione.Importato_In_GIAS

                        GiasContext.cbl_Calibrature.Attach(movimento.Lotto)
                        GiasContext.Entry(movimento.Lotto).State = EntityState.Modified

                        GiasContext.SaveChanges()

                    Next

                    ' COMIT Effettivo
                    scope.Complete()

                End Using

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function


    Public Function Cancella_CampionamentoConferito_TestataGriglia_e_Listini_Prodotti(ByVal piva As String,
                                                                                        ByVal mat_cod As Integer,
                                                                                        ByRef objParametri As AgronicaCoreParametri
                                                                                        ) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Cancella_CampionamentoConferito_TestataGriglia_e_Listini_Prodotti()"
        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim MessaggioErrore As String = String.Empty

        Try

            Dim objCampionamentoConferito As New AgronicaCoreContabDAL.FF_CampionamentoConferimento_R
            Dim List_CampionamentoConferito = objCampionamentoConferito.Leggi_TestataGriglia_Prodotti_FiltroMat_Cod(piva, mat_cod, "",
                                                                                                                    objParametri)

            If List_CampionamentoConferito.Count > 0 Then

                Dim gefutils As New Gias_EF_Utility
                Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    For Each prodotto As CampionamentoConferito_TestataGriglia_Prodotti In List_CampionamentoConferito

                        Dim listini_prodotti =
                            From p In GiasContext.Listini_CampionamentoConferito_Prodotti Where
                                  p.Id_TestataGriglia_Prod = prodotto.Id_TestataGriglia_Prod AndAlso
                                  p.Piva_SuperUser.Equals(prodotto.Piva_SuperUser) AndAlso
                                  p.PIVA.Equals(prodotto.PIVA)

                        ' Cancella Listini_CampionamentoConferito_Prodotti
                        For Each l_p In listini_prodotti
                            GiasContext.Listini_CampionamentoConferito_Prodotti.Attach(l_p)
                            GiasContext.Listini_CampionamentoConferito_Prodotti.Remove(l_p)
                        Next

                        Dim listini_prodotti_equivalenti =
                            From p In GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti
                            Where (p.Id_TestataGriglia_Prod = prodotto.Id_TestataGriglia_Prod OrElse
                                   p.Id_TestataGriglia_Prod_Equivalente = prodotto.Id_TestataGriglia_Prod) AndAlso
                                  p.Piva_SuperUser.Equals(prodotto.Piva_SuperUser) AndAlso
                                  p.PIVA.Equals(prodotto.PIVA)

                        ' Cancella Listini_CampionamentoConferito_Prodotti_Equivalenti
                        For Each l_p_e In listini_prodotti_equivalenti
                            GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Attach(l_p_e)
                            GiasContext.Listini_CampionamentoConferito_Prodotti_Equivalenti.Remove(l_p_e)
                        Next

                        Dim listini_dettagli =
                            From p In GiasContext.Listini_CampionamentoConferito_Dettagli Where
                                  p.Id_TestataGriglia_Prod = prodotto.Id_TestataGriglia_Prod AndAlso
                                  p.Id_TestataGriglia = prodotto.Id_TestataGriglia AndAlso
                                  p.Piva_SuperUser.Equals(prodotto.Piva_SuperUser) AndAlso
                                  p.PIVA.Equals(prodotto.PIVA)

                        ' Cancella Listini_CampionamentoConferito_Dettagli
                        For Each l_d In listini_dettagli
                            GiasContext.Listini_CampionamentoConferito_Dettagli.Attach(l_d)
                            GiasContext.Listini_CampionamentoConferito_Dettagli.Remove(l_d)
                        Next

                        ' Cancella CampionamentoConferito_TestataGriglia_Prodotti
                        GiasContext.CampionamentoConferito_TestataGriglia_Prodotti.Attach(prodotto)
                        GiasContext.CampionamentoConferito_TestataGriglia_Prodotti.Remove(prodotto)

                    Next

                    GiasContext.SaveChanges()

                End Using

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function

End Class
