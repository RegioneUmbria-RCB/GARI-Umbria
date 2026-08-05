Imports System.Data.Entity
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

'Public Class ObjDaInserire

'    Public Property Id_Agenda As Integer
'    Public Property Raggruppamento_Cod As Integer
'    Public Property Id_CDG As Integer
'    Public Property Id_CDG_Dettagli As Integer
'    Public Property Lav_Cod As Integer
'    Public Property Des_Lib As String
'    Public Property Data_Operazione As Date



'End Class

'
'#################################################################
'#################################################################
'#################################################################




'
'#################################################################
'#################################################################
'#################################################################



Public Class CDG_DAL_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Property CDG_Testata_Group As Object











    '##############################################################################################
    Public Function Verifica_CDG_Animali(ByVal piva As String,
                                            ByVal sa_cod As Integer,
                                            ByVal cod_animale As Integer,
                                            ByVal cod_progetto As Integer,
                                            ByRef objParametri As AgronicaCoreParametri) As Integer


        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Verifica_CDG_Zoo()"
        Dim risultato As Boolean = False

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim DettagliElem_Zoo =
                      From CI In GiasContext.CDG_Dettagli
                      Where (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                      AndAlso (CI.Piva.Equals(piva)) _
                      AndAlso (sa_cod = 0 OrElse CI.Sa_Cod = sa_cod) _
                      AndAlso (CI.Cod_Animale = cod_animale) _
                      AndAlso (cod_progetto = 0 OrElse CI.Cod_Animale_Distinta = cod_progetto)

            risultato = DettagliElem_Zoo.Count > 0

        End Using

        Return risultato

    End Function

    '##############################################################################################
    Public Function Verifica_CDG_Impianti(ByVal piva As String,
                                            ByVal sa_cod As Integer,
                                            ByVal appezza As Integer,
                                            ByVal id_reg As Integer,
                                            ByVal lista_progetto_cod As List(Of Integer),
                                            ByRef objParametri As AgronicaCoreParametri) As Integer


        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Verifica_CDG_Impianti()"
        Dim verificaImpianti As Boolean = lista_progetto_cod Is Nothing
        Dim risultato As Boolean = False

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim DistinteImpianti As New List(Of Integer)

            If Not verificaImpianti Then

                DistinteImpianti =
                    (From ic In GiasContext.Imprese_Progetti
                     Where ic.Piva = piva And
                        ic.Sa_Cod = sa_cod And
                        ic.Appezza = appezza And
                        ic.Id_Reg = id_reg And
                        Not lista_progetto_cod.Contains(ic.Progetto_Cod)
                     Select ic.Progetto_Cod).ToList

            End If

            If verificaImpianti OrElse DistinteImpianti.Count > 0 Then

                Dim DettagliElem_Impianti =
                      From CI In GiasContext.CDG_Dettagli
                      Where (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                      AndAlso (CI.Piva.Equals(piva)) _
                      AndAlso (CI.Sa_Cod = sa_cod) _
                      AndAlso (CI.Appezza = appezza) _
                      AndAlso (CI.Id_Reg = id_reg) _
                      AndAlso (verificaImpianti OrElse DistinteImpianti.Contains(CI.Progetto_Cod))

                risultato = DettagliElem_Impianti.Count > 0

            End If

        End Using

        Return risultato

    End Function

    '##############################################################################################
    Public Function Ricerca_GestioneCompleta(ByVal piva As String,
                                             ByVal bAgenda As Boolean,
                                             ByVal budget As Integer,
                                             ByVal FiltroDescrizione As String,
                                             ByVal FiltroSplit As Integer,
                                             ByVal FiltroTipo() As String,
                                             ByVal FiltroStato() As String,
                                             ByVal FiltroCosti_Ricavi() As String,
                                             ByVal Key As String,
                                             ByVal Validita_Inizio As Date,
                                             ByVal Validita_Fine As Date,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim Ora As Integer
        Dim Minuti As Integer
        Dim parteIntera As Decimal
        Dim parteDecimali As Decimal

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Ricerca_GestioneCompleta()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim strFiltro As String = ""
        Dim bStato_Manuale As Boolean
        Dim bStato_Automatico As Boolean
        Dim bStato_Da_Verificare As Boolean
        Dim Indice As Integer


        Dim bFiltroTipo As Boolean = False
        If FiltroTipo IsNot Nothing AndAlso FiltroTipo.Length > 0 Then
            bFiltroTipo = True
        Else
            bFiltroTipo = False
        End If


        Dim bFiltroStato As Boolean = False
        If FiltroStato IsNot Nothing AndAlso FiltroStato.Length > 0 Then

            bFiltroStato = True

            For Indice = 0 To FiltroStato.Length - 1

                Select Case CInt(FiltroStato(Indice))

                    Case 0 'Manuale

                        bStato_Manuale = True

                    Case 1 'Automatico

                        bStato_Automatico = True

                    Case 2 'Da Verificare

                        bStato_Da_Verificare = True

                End Select

            Next

        Else

            bFiltroStato = False

        End If


        Dim bFiltroCosti_Ricavi As Boolean = False

        If FiltroCosti_Ricavi IsNot Nothing AndAlso FiltroCosti_Ricavi.Length > 0 Then
            bFiltroCosti_Ricavi = True
        Else
            bFiltroCosti_Ricavi = False
        End If

        Dim Sa_Cod_Padre As Integer = 0
        Dim Appezza_Padre As Integer = 0
        Dim Id_Reg_Padre As Integer = 0


        'Key
        Dim bFiltroKey As Boolean = False
        If Trim(Key) <> "" Then
            Dim ArrayA As String() = Split(Key, "-")
            Sa_Cod_Padre = ArrayA(1)
            Appezza_Padre = ArrayA(3)
            Id_Reg_Padre = ArrayA(4)
            bFiltroKey = True
        End If

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From CI In GiasContext.CDG_Testata
               Join Agenda In GiasContext.Agenda
                     On Agenda.PIVA Equals CI.Piva And
                        Agenda.Id_Agenda Equals CI.Id_Agenda
               Join CDG_Dettagli In GiasContext.CDG_Dettagli
                     On CDG_Dettagli.Piva Equals CI.Piva And
                        CDG_Dettagli.Id_CDG Equals CI.Id_CDG
               Group Join Mov_Dettagli_Riferimenti In GiasContext.Mov_Dettagli_Riferimenti
                 On Mov_Dettagli_Riferimenti.Piva_Rif Equals CI.Piva _
                   And Mov_Dettagli_Riferimenti.Id_Agenda_Rif Equals CI.Id_Agenda Into Mov_Dettagli_Riferimenti_Group = Group
               From _Mov_Dettagli_Riferimenti_Group In Mov_Dettagli_Riferimenti_Group.DefaultIfEmpty()
               Group Join Agenda2 In GiasContext.Agenda
                 On _Mov_Dettagli_Riferimenti_Group.Piva Equals Agenda2.PIVA _
                   And _Mov_Dettagli_Riferimenti_Group.Id_Agenda Equals Agenda2.Id_Agenda Into Agenda2_Group = Group
               From _Agenda2_Group In Agenda2_Group.DefaultIfEmpty()
               Group Join Unita_Misura In GiasContext.UnitaMisura
                     On Unita_Misura.UDM_COD Equals CI.Udm_Cod Into UnitaMisura_Group = Group
               From _Unita_Misura In UnitaMisura_Group.DefaultIfEmpty()
               Group Join Attivita In GiasContext.Attivita
                     On Attivita.ID_Attivita Equals CI.Id_Attivita Into Attivita_Group = Group
               From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
               Where CI.Piva.Equals(piva) AndAlso
                     CI.Piva_Superuser.Equals(Piva_SuperUser) AndAlso
                     CI.Budget = budget AndAlso
                     CI.Vecchio_Tipo_Inser_Dati = 0 AndAlso
                     ((FiltroDescrizione = "") OrElse Agenda.des_lib.Contains(FiltroDescrizione)) AndAlso
                     ((FiltroSplit = -1) OrElse Agenda.Split = FiltroSplit) AndAlso
                     ((Not bFiltroTipo) OrElse FiltroTipo.Contains(CI.Modalita_Imputazione)) AndAlso
                     ((Not bFiltroStato) OrElse
                      ((bStato_Manuale AndAlso ((_Agenda2_Group.Data_Modifica <= _Mov_Dettagli_Riferimenti_Group.data_creazione OrElse CI.Modalita_Imputazione <> 0) AndAlso CI.ModifInAutom = 0)) OrElse
                                  (bStato_Automatico AndAlso CI.ModifInAutom = 1) OrElse
                                  (bStato_Da_Verificare AndAlso CI.Modalita_Imputazione = 0 AndAlso (_Agenda2_Group.Data_Modifica > _Mov_Dettagli_Riferimenti_Group.data_creazione OrElse _Mov_Dettagli_Riferimenti_Group Is Nothing)))) AndAlso
               ((Not bFiltroCosti_Ricavi) OrElse FiltroCosti_Ricavi.Contains(CI.Costi_Ricavi)) AndAlso
               ((Not bFiltroKey) OrElse (CDG_Dettagli.Sa_Cod = Sa_Cod_Padre AndAlso CDG_Dettagli.Appezza = Appezza_Padre AndAlso CDG_Dettagli.Id_Reg = Id_Reg_Padre)) AndAlso
               CI.Data_Inserimento >= Validita_Inizio And
               CI.Data_Inserimento <= Validita_Fine And
               (_Mov_Dettagli_Riferimenti_Group.Lav_Cod_Rif = LAVCOD_COSTI_CDG Or _Mov_Dettagli_Riferimenti_Group Is Nothing)
               Order By CI.Data_Inserimento Ascending, CI.Descrizione Ascending
               Select New With {
              .Piva = CI.Piva,
              .Id_Agenda = If(_Mov_Dettagli_Riferimenti_Group Is Nothing, CI.Id_Agenda, _Mov_Dettagli_Riferimenti_Group.Id_Agenda),
              .Id_Mov = If(_Mov_Dettagli_Riferimenti_Group Is Nothing, 0, _Mov_Dettagli_Riferimenti_Group.Id_Mov),
              .Id_Mov_Det = If(_Mov_Dettagli_Riferimenti_Group Is Nothing, 0, _Mov_Dettagli_Riferimenti_Group.Id_Mov_Det),
              .Id_Agenda_Rif = If(_Mov_Dettagli_Riferimenti_Group Is Nothing, 0, _Mov_Dettagli_Riferimenti_Group.Id_Agenda_Rif),
              .Des_Lib = Agenda.des_lib,
              .Id_CDG = If(bAgenda, 0, CI.Id_CDG),
              .Descrizione = If(bAgenda, "", CI.Descrizione),
              .Data = CI.Data_Inserimento,
              .Costi_Ricavi = If(CI.Costi_Ricavi = 0, "costi", "ricavi"),
              .Costi_Ricavi_Des = If(CI.Costi_Ricavi = 0, "Costi", "Ricavi"),
              .Modalita_Imputazione = CI.Modalita_Imputazione,
              .Modalita_Imputazione_Des = "",
              .Stato = If((_Agenda2_Group.Data_Modifica <= _Mov_Dettagli_Riferimenti_Group.data_creazione Or CI.Modalita_Imputazione <> 0) And CI.ModifInAutom = 0, 0, If(CI.ModifInAutom = 1, 1, 2)),
              .Desc = If(bAgenda, "", If(_Attivita_Group Is Nothing, "", _Attivita_Group.Sigla & " - " & _Attivita_Group.Desc)),
              .Qta = If(bAgenda, "0", CStr(CI.Qta)),
              .Udm_Des = If(bAgenda, "", If(_Unita_Misura Is Nothing, "", _Unita_Misura.UDM_SIM)),
              .OrigineApp = If(bAgenda, 0, CI.OrigineApp),
              .Cod_Risum = If(bAgenda, 0, CI.Cod_RisUm),
              .Mac_Cod = If(bAgenda, 0, CI.Mac_Cod),
              .Split = Agenda.Split,
              .Lav_Cod = If(_Agenda2_Group Is Nothing, Agenda.Lav_Cod, _Agenda2_Group.Lav_Cod),
              .isAttivitaInterna = _Attivita_Group.Attivita_Interna
              }

            Dim myList = TestataElem.Distinct().ToList()

            For Each obj In myList

                If obj.Modalita_Imputazione = 0 Then
                    obj.Modalita_Imputazione_Des = "Op. QdC o Zoo"
                Else
                    If obj.Modalita_Imputazione = 1 AndAlso obj.OrigineApp = 0 Then
                        obj.Modalita_Imputazione_Des = "Diretta"
                    Else
                        If obj.Modalita_Imputazione = 1 AndAlso obj.OrigineApp <> 0 Then
                            obj.Modalita_Imputazione_Des = "App"
                        Else
                            If obj.Modalita_Imputazione = 2 Then
                                obj.Modalita_Imputazione_Des = "TimeSheet"
                            Else
                                If obj.Modalita_Imputazione = 4 Then
                                    obj.Modalita_Imputazione_Des = "Contabilità"
                                End If
                            End If
                        End If
                    End If
                End If

                If obj.Cod_Risum <> 0 OrElse obj.Mac_Cod <> 0 Then
                    'Conversione intero minuti a data
                    parteIntera = Math.Truncate(CDec(obj.Qta.Replace(".", ",")))
                    parteDecimali = CDec(obj.Qta.Replace(".", ",")) - parteIntera
                    Ora = parteIntera
                    Minuti = 60 * parteDecimali
                    obj.Qta = Ora.ToString("D2") & ":" & Minuti.ToString("D2")
                End If

                If obj.Split Is Nothing Then
                    obj.Split = 0
                End If


            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

        End Using


        Return risposta

    End Function


    '##############################################################################################
    Public Function Ricerca_GestioneCompleta_Contab(ByVal piva As String,
                                                    ByVal FiltroCosti_Ricavi() As String,
                                                    ByVal Validita_Inizio As Date,
                                                    ByVal Validita_Fine As Date,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Ricerca_GestioneCompleta_Contab()"

        Dim risposta As String = ""

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim strFiltro As String = ""

        Dim bFiltroCosti_Ricavi As Boolean = False

        If FiltroCosti_Ricavi IsNot Nothing AndAlso FiltroCosti_Ricavi.Length > 0 Then
            bFiltroCosti_Ricavi = True
        Else
            bFiltroCosti_Ricavi = False
        End If


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
            From Agenda In GiasContext.Agenda
            Join Movimenti In GiasContext.Movimenti
                 On Movimenti.PIVA Equals Agenda.PIVA And
                    Movimenti.Id_Agenda Equals Agenda.Id_Agenda
            Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli
                 On Movimenti.PIVA Equals Movimenti_Dettagli.PIVA And
                    Movimenti.Id_Agenda Equals Movimenti_Dettagli.Id_Agenda And
                    Movimenti.Id_Mov Equals Movimenti_Dettagli.Id_Mov
            Group Join Mov_Dettagli_Riferimenti In GiasContext.Mov_Dettagli_Riferimenti.Where(Function(x) x.Lav_Cod_Rif = LAVCOD_COSTI_CDG)
                 On Mov_Dettagli_Riferimenti.Piva Equals Movimenti_Dettagli.PIVA And
                    Mov_Dettagli_Riferimenti.Id_Agenda Equals Movimenti_Dettagli.Id_Agenda And
                    Mov_Dettagli_Riferimenti.Id_Mov Equals Movimenti_Dettagli.Id_Mov And
                    Mov_Dettagli_Riferimenti.Id_Mov_Det Equals Movimenti_Dettagli.Id_Mov_Det
                    Into Mov_Dettagli_Riferimenti_Group = Group
            From _Mov_Dettagli_Riferimenti_Group In Mov_Dettagli_Riferimenti_Group.DefaultIfEmpty()
            Group Join Materie_Prime In GiasContext.Materie_Prime
                         On Materie_Prime.Elem_Cod Equals Movimenti_Dettagli.Elem_Cod And
                            Materie_Prime.Mat_Cod Equals Movimenti_Dettagli.Mat_Cod Into Materie_Prime_Group = Group
            From _Materie_Prime_Group In Materie_Prime_Group.DefaultIfEmpty()
            Group Join CategorieMagazzino In GiasContext.CategorieMagazzino
                         On Movimenti_Dettagli.Elem_Cod Equals CategorieMagazzino.Elem_Cod Into CategorieMagazzino_Group = Group
            From _CategorieMagazzino_Group In CategorieMagazzino_Group.DefaultIfEmpty()
            Group Join Fertilizzanti In GiasContext.Fertilizzanti
                         On Movimenti_Dettagli.Pro_Cod Equals Fertilizzanti.Fer_Cod Into Fertilizzanti_Group = Group
            From _Fertilizzanti_Group In Fertilizzanti_Group.DefaultIfEmpty()
            Group Join Formulati In GiasContext.Formulati
                         On Movimenti_Dettagli.Pro_Cod Equals Formulati.Fr_Cod Into Formulati_Group = Group
            From _Formulati_Group In Formulati_Group.DefaultIfEmpty()
            Group Join Coadiuvante In GiasContext.Coadiuvante
                         On Movimenti_Dettagli.Pro_Cod Equals Coadiuvante.Coad_Cod Into Coadiuvante_Group = Group
            From _Coadiuvante_Group In Coadiuvante_Group.DefaultIfEmpty()
            Group Join InsettiUtili In GiasContext.InsettiUtili
                         On Movimenti_Dettagli.Pro_Cod Equals InsettiUtili.Ins_Cod Into InsettiUtili_Group = Group
            From _InsettiUtili_Group In InsettiUtili_Group.DefaultIfEmpty()
            Group Join Trappole In GiasContext.Trappole
                         On Movimenti_Dettagli.Pro_Cod Equals Trappole.TRAP_COD Into Trappole_Group = Group
            From _Trappole_Group In Trappole_Group.DefaultIfEmpty()
            Group Join Avversita In GiasContext.Avversita
                         On Movimenti_Dettagli.Pro_Cod Equals Avversita.Av_Cod Into Avversita_Group = Group
            From _Avversita_Group In Avversita_Group.DefaultIfEmpty()
            Group Join Categorie In GiasContext.Categorie.Where(Function(x) x.PADRE = "S000029")
                         On Right(Categorie.COD, 3) Equals Movimenti_Dettagli.Pro_Cod Into Categorie_Group = Group
            From _Categorie_Group In Categorie_Group.DefaultIfEmpty()
            Group Join Unita_Misura In GiasContext.UnitaMisura
                     On Unita_Misura.UDM_COD Equals Movimenti_Dettagli.Udm_Cod Into UnitaMisura_Group = Group
            From _Unita_Misura In UnitaMisura_Group.DefaultIfEmpty()
            Where Agenda.PIVA = piva AndAlso
                  (_Mov_Dettagli_Riferimenti_Group Is Nothing) AndAlso
                  Agenda.Validita_Inizio >= Validita_Inizio AndAlso
                  Agenda.Validita_Inizio <= Validita_Fine AndAlso
                  Movimenti_Dettagli.Elem_Cod <> 0 AndAlso
                  ((Movimenti_Dettagli.Elem_Cod = CostantiPersonalizzate.FERTILIZZANTI AndAlso _Fertilizzanti_Group.Fer_Cod <> 0) OrElse
                   (Movimenti_Dettagli.Elem_Cod = CostantiPersonalizzate.FORMULATI AndAlso _Formulati_Group.Fr_Cod <> 0) OrElse
                   (Movimenti_Dettagli.Elem_Cod = CostantiPersonalizzate.COADIUVANTI AndAlso _Coadiuvante_Group.Coad_Cod <> 0) OrElse
                   (Movimenti_Dettagli.Elem_Cod = CostantiPersonalizzate.INSETTI AndAlso _InsettiUtili_Group.Ins_Cod <> 0) OrElse
                   (Movimenti_Dettagli.Elem_Cod = CostantiPersonalizzate.TRAPPOLE AndAlso _Trappole_Group.TRAP_COD <> 0) OrElse
                   (Movimenti_Dettagli.Elem_Cod = CostantiPersonalizzate.INNESCHI AndAlso _Avversita_Group.Av_Cod <> 0) OrElse
                   (Movimenti_Dettagli.Elem_Cod = CostantiPersonalizzate.SERVIZI AndAlso Left(_Categorie_Group.COD, 1) = "S") OrElse
                   (Movimenti_Dettagli.Mat_Cod <> 0 AndAlso _Materie_Prime_Group.Mat_Cod <> 0) OrElse
                   (Movimenti_Dettagli.Elem_Cod = CostantiPersonalizzate.ALTRI_BENI AndAlso Movimenti_Dettagli.Pro_Cod = 0 AndAlso Movimenti_Dettagli.Mat_Cod = 0)) AndAlso
                  ({LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_EMESSA, LAVCOD_ORDINE_ACQUISTO, LAVCOD_BOLLA_EMESSA}.Contains(Agenda.Lav_Cod)) AndAlso
                  Movimenti_Dettagli.Elem_Cod <> CostantiPersonalizzate.RIGA_DESCRIZIONE_LIBERA AndAlso
                  ((Not bFiltroCosti_Ricavi) OrElse FiltroCosti_Ricavi.Contains(Movimenti.Cau_Mov)) AndAlso
                  ((Agenda.Data_Modifica > _Mov_Dettagli_Riferimenti_Group.data_creazione) OrElse
                   _Mov_Dettagli_Riferimenti_Group Is Nothing)
            Order By Agenda.Validita_Inizio Ascending, Agenda.Id_Agenda Ascending
            Select New With {
                .Piva = Agenda.PIVA,
                .Id_Agenda = Agenda.Id_Agenda,
                .Id_Mov = Movimenti_Dettagli.Id_Mov,
                .Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det,
                .Id_Agenda_Rif = If(_Mov_Dettagli_Riferimenti_Group Is Nothing, 0, _Mov_Dettagli_Riferimenti_Group.Id_Agenda),
                .Des_Lib = Agenda.des_lib,
                .Id_CDG = 0,
                .Descrizione = Agenda.des_lib,
                .Data = Agenda.Validita_Inizio,
                .Costi_Ricavi = If(Movimenti.Cau_Mov = CAU_CARICO, "costi", "ricavi"),
                .Costi_Ricavi_Des = If(Movimenti.Cau_Mov = CAU_CARICO, "Costi", "Ricavi"),
                .Stato = If(_Mov_Dettagli_Riferimenti_Group Is Nothing, 1, 2),
                .Desc = Movimenti_Dettagli.Mov_Det_Des,
                .Lotto = Movimenti_Dettagli.Lotto,
                .Qta = CStr(Movimenti_Dettagli.Qta),
                .Elem_Cod = CStr(Movimenti_Dettagli.Elem_Cod),
                .NomeComune = If(_CategorieMagazzino_Group Is Nothing, "", _CategorieMagazzino_Group.NomeComune),
                .Prodotto_Cod = If(Movimenti_Dettagli.Pro_Cod <> 0, Movimenti_Dettagli.Pro_Cod, Movimenti_Dettagli.Mat_Cod),
                .Prodotto_Des = Movimenti_Dettagli.Mov_Det_Des,
                .Mat_Des = If(_Materie_Prime_Group Is Nothing, "", _Materie_Prime_Group.Mat_Des),
                .Fer_Des = If(_Fertilizzanti_Group Is Nothing, "", _Fertilizzanti_Group.Fer_Des),
                .Fr_Des = If(_Formulati_Group Is Nothing, "", _Formulati_Group.Fr_Des),
                .Coad_Des = If(_Coadiuvante_Group Is Nothing, "", _Coadiuvante_Group.Coad_Des),
                .Ins_Des = If(_InsettiUtili_Group Is Nothing, "", _InsettiUtili_Group.Ins_Des),
                .Trap_Des = If(_Trappole_Group Is Nothing, "", _Trappole_Group.TRAP_DES),
                .Av_Des_Vol = If(_Avversita_Group Is Nothing, "", _Avversita_Group.Av_Des_Vol),
                .Servizio_Des = If(_Categorie_Group Is Nothing, "", _Categorie_Group.DESCR),
                .Udm_Des = If(_Unita_Misura Is Nothing, "", _Unita_Misura.UDM_SIM),
                .Cod_Risum = Movimenti.Cod_RisUm,
                .Lav_Cod = Agenda.Lav_Cod,
                .Modalita_Imputazione_Des = "Contabilità"
            }

            Dim myList_Eredita = TestataElem.ToList()
            For Each obj In myList_Eredita


                Select Case obj.Elem_Cod
                    Case CostantiPersonalizzate.FERTILIZZANTI
                        obj.Desc = obj.Fer_Des
                    Case CostantiPersonalizzate.FORMULATI
                        obj.Desc = obj.Fr_Des
                    Case CostantiPersonalizzate.COADIUVANTI
                        obj.Desc = obj.Coad_Des
                    Case CostantiPersonalizzate.INSETTI
                        obj.Desc = obj.Ins_Des
                    Case CostantiPersonalizzate.TRAPPOLE
                        obj.Desc = obj.Trap_Des
                    Case CostantiPersonalizzate.INNESCHI
                        obj.Desc = obj.Av_Des_Vol
                    Case CostantiPersonalizzate.ALTRI_BENI
                        obj.NomeComune = "Altri Beni Strumentali"
                    Case CostantiPersonalizzate.SERVIZI
                        obj.Desc = obj.Servizio_Des & " " & obj.Desc
                        obj.NomeComune = "Servizi"
                    Case Else
                        obj.Desc = obj.Mat_Des
                End Select

            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(myList_Eredita, Formatting.None, serializerSettings)

        End Using


        Return risposta

    End Function





    Public Function Leggi_Agenda_VegCod(ByVal piva As String,
                                          ByVal Id_Agenda As Integer,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser



        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Agenda_VegCod()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT distinct Cultivar.Veg_Cod ")
            strSql.AppendLine(" From Mov_Destinazioni, Cultivar, Reg_Impianti   ")

            strSql.AppendLine(" Where Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" And Mov_Destinazioni.Id_Agenda = " & Id_Agenda & " ")
            strSql.AppendLine(" And Mov_Destinazioni.Tipo_Destinazione = 0 ")
            strSql.AppendLine(" And Mov_Destinazioni.Piva = Reg_Impianti.Piva ")
            strSql.AppendLine(" And Mov_Destinazioni.Sa_Cod = Reg_Impianti.Sa_Cod ")
            strSql.AppendLine(" And Mov_Destinazioni.Appezza = Reg_Impianti.Appezza ")
            strSql.AppendLine(" And Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg ")
            strSql.AppendLine(" And Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod ")

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)



        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :   " & messaggioErrore)
        End Try

        Return dt


    End Function




    Public Function Leggi_SquadrexAttvita(ByVal piva As String,
                                          ByVal Id_Squadra As Integer,
                                          ByVal Id_Attivita As Integer,
                                          ByVal Lav_Cod As Integer,
                                          ByVal Veg_Cod As Integer,
                                          ByVal strfiltro As String,
                                          ByVal data_movimento As Date,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser



        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_SquadrexAttvita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try


            strSql.Length = 0

            'strSql.AppendLine(" SELECT Distinct SquadrexAttivita.*, 0 AS ID_Attivita, '' AS [Desc] ")
            strSql.AppendLine(" SELECT distinct SquadrexAttivita.* ")
            strSql.AppendLine(" From SquadrexAttivita   ")

            If Lav_Cod <> 0 Then
                strSql.AppendLine(" ,AttivitaxOperazioni ")
            End If

            strSql.AppendLine(" Where SquadrexAttivita.Piva_SuperUser = '" & Piva_SuperUser & "' ")
            strSql.AppendLine(" And SquadrexAttivita.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & " ")
            strSql.AppendLine(" And SquadrexAttivita.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ")

            If piva <> "" Then
                strSql.AppendLine(" And SquadrexAttivita.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If Id_Squadra <> 0 Then
                strSql.AppendLine(" And SquadrexAttivita.Id_Squadra = " & Id_Squadra & " ")
            End If

            If Lav_Cod <> 0 Then
                strSql.AppendLine(" And ( ")
                strSql.AppendLine("    (SquadrexAttivita.Lav_Cod = " & Lav_Cod & " And SquadrexAttivita.Lav_Cod = AttivitaxOperazioni.Lav_Cod And SquadrexAttivita.Piva_SuperUser = AttivitaxOperazioni.Piva_SuperUser) ")
                strSql.AppendLine(" Or ")
                strSql.AppendLine("    (  AttivitaxOperazioni.Lav_Cod = " & Lav_Cod & " And SquadrexAttivita.Lav_Cod = 0 And '|'+ SquadrexAttivita.attivita_list  + '|' like '%|'+ convert(varchar, AttivitaxOperazioni.Id_Attivita) + '|%' ) ")
                strSql.AppendLine(" ) ")
            End If

            If Veg_Cod <> 0 Then
                strSql.AppendLine(" And ( SquadrexAttivita.veg_cod_list IS NULL Or SquadrexAttivita.veg_cod_list = '' Or '|'+ SquadrexAttivita.veg_cod_list + '|' like '%|'+ convert(varchar, " & Veg_Cod & ") + '|%') ")
            End If

            If strfiltro <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(strfiltro, , objParametri))
            End If

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)



        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :   " & messaggioErrore)
        End Try

        Return dt


    End Function





    Public Function Leggi_AttivitaxSquadra(ByVal piva As String,
                                          ByVal Id_Squadra As Integer,
                                          ByVal lav_cod As Integer,
                                          ByVal data_movimento As Date,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser



        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_AttivitaxSquadra()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try


            strSql.Length = 0

            strSql.AppendLine(" SELECT Distinct Attivita.Id_Attivita, ISNULL(Attivita.[Desc], '') AS [Desc]  ")
            strSql.AppendLine(" From SquadrexAttivita, Attivita ")
            If lav_cod <> 0 Then
                strSql.AppendLine(" ,AttivitaXOperazioni  ")
            End If
            strSql.AppendLine(" Where SquadrexAttivita.Piva_SuperUser = '" & Piva_SuperUser & "' ")
            strSql.AppendLine(" And SquadrexAttivita.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" And SquadrexAttivita.Id_Squadra = " & Id_Squadra & " ")
            strSql.AppendLine(" And '|'+ SquadrexAttivita.attivita_list +'|'   like '%|'+ convert(varchar, Attivita.Id_Attivita) +'|%' ")
            If lav_cod <> 0 Then
                strSql.AppendLine(" And ( SquadrexAttivita.Piva_SuperUser = AttivitaXOperazioni.Piva_SuperUser ")
                strSql.AppendLine(" And Attivita.Id_Attivita =  AttivitaXOperazioni.Id_Attivita ")
                strSql.AppendLine(" And AttivitaXOperazioni.Lav_Cod = " & lav_cod & ") ")
            End If
            strSql.AppendLine(" And Attivita.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & " ")
            strSql.AppendLine(" And Attivita.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ")


            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :     " & messaggioErrore)
        End Try

        Return dt


    End Function


    Public Function Leggi_SquadrexAttvita_Macchine(ByVal piva As String,
                                                   ByVal Id_Squadra As String,
                                                   ByVal Id_Attivita As Integer,
                                                   ByVal data_movimento As Date,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser



        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_SquadrexAttvita_Macchine()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try


            strSql.Length = 0

            strSql.AppendLine(" SELECT Distinct Parco_Macchine.Sa_Cod, Mac_Cod, Mac_Des, ISNULL(Attivita.[Desc], '') AS [Desc]  ")
            strSql.AppendLine(" From SquadrexAttivita, Parco_Macchine ")

            strSql.AppendLine(" Left Outer Join Attivita On (Attivita.Id_Attivita = " & Id_Attivita & " And Attivita.Piva_SuperUser = '" & Piva_SuperUser & "') ")
            strSql.AppendLine(" Where SquadrexAttivita.Piva_SuperUser = '" & Piva_SuperUser & "' ")
            strSql.AppendLine(" And SquadrexAttivita.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" And SquadrexAttivita.Id_Squadra = " & Id_Squadra & " ")
            strSql.AppendLine(" And '|'+ SquadrexAttivita.mac_cod_list +'|'   like '%|'+ convert(varchar, Parco_Macchine.Mac_Cod) +'|%' ")

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :   " & messaggioErrore)
        End Try

        Return dt


    End Function



    Public Function Leggi_SquadrexAttvita_Contatti(ByVal piva As String,
                                                   ByVal Id_Squadra As String,
                                                   ByVal Id_Attivita As Integer,
                                                   ByVal data_movimento As Date,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser



        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_SquadrexAttvita_Contatti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try


            strSql.Length = 0

            strSql.AppendLine(" Select Distinct Risorse_Umane.Cod_Risum, Risorse_Umane.Cod_Rapporto, Risorse_Umane.Qualifica_Cod,  ISNULL(qualifiche.Qualifica_Des, '') AS Qualifica_Des, Contatti.Nome, Contatti.Cognome, Contatti.Rag_Soc, Rapporti_Contabili.Rapporto_Des, ISNULL(Attivita.[Desc], '') AS [Desc] ")
            strSql.AppendLine(" From SquadrexAttivita, Contatti, Rapporti_Contabili, Risorse_Umane ")

            strSql.AppendLine(" Left Outer Join Qualifiche On (Risorse_Umane.Qualifica_Cod = Qualifiche.Qualifica_Cod And Qualifiche.Piva_SuperUser = '" & Piva_SuperUser & "') ")
            strSql.AppendLine(" Left Outer Join Attivita On (Attivita.Id_Attivita = " & Id_Attivita & " And Attivita.Piva_SuperUser = '" & Piva_SuperUser & "') ")

            strSql.AppendLine(" Where SquadrexAttivita.Piva_SuperUser = '" & Piva_SuperUser & "' ")
            strSql.AppendLine(" And SquadrexAttivita.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" And SquadrexAttivita.Id_Squadra = " & Id_Squadra & " ")
            strSql.AppendLine(" And '|'+ SquadrexAttivita.cod_risum_list +'|'   like '%|'+ convert(varchar, Risorse_Umane.cod_risum) +'|%' ")
            strSql.AppendLine(" And Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ")
            strSql.AppendLine(" And Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto ")


            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :   " & messaggioErrore)
        End Try

        Return dt


    End Function


    Public Function Ricerca_Agenda_Split(ByVal piva As String,
                                   ByVal Key As String,
                                   ByVal data_movimento As Date,
                                   ByVal bEsclusi As Boolean,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim chiave As String() = Key.Split("-")
        Dim kPiva As String = CStr(chiave(0))
        Dim kSa_Cod As Integer = Integer.Parse(chiave(1))
        Dim kAppezza As Integer = Integer.Parse(chiave(3))
        Dim kId_Reg As Integer = Integer.Parse(chiave(4))
        Dim kProgetto_Cod As Integer = Integer.Parse(chiave(5))

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Ricerca_Agenda_Split()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim dts As DataTable

        Dim Elenco_Esercizi As String
        Dim Elenco_Risorse As String

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT AgendaCdG.Id_Agenda, AgendaCdG.Lav_Cod, AgendaCdG.Des_Lib,  Cdg_Testata.Data_Inserimento AS DataRegistrazione,  ")
            strSql.AppendLine(" Case WHEN MIN(attivita.[desc]) <>  MAX(attivita.[desc]) THEN MIN(attivita.[desc]) + ' - ' + MAX(attivita.[desc]) Else MAX(attivita.[desc]) End  AS attivita, ")

            strSql.AppendLine(" Agenda.Validita_Inizio AS DataIntervento, '' AS Elenco_Esercizi, '' AS Elenco_Risorse ")
            strSql.AppendLine(" From Agenda AS AgendaCdG ")
            strSql.AppendLine(" join CDG_Testata on AgendaCdG.PIVA = Cdg_Testata.PIVA And AgendaCdG.Id_Agenda = Cdg_Testata.Id_Agenda ")
            strSql.AppendLine(" join CDG_Dettagli on Cdg_Testata.PIVA = CDG_Dettagli.PIVA And Cdg_Testata.Id_Cdg = CDG_Dettagli.Id_Cdg  ")
            strSql.AppendLine(" join Attivita ")
            strSql.AppendLine(" On Cdg_Testata.id_attivita = attivita.id_attivita ")
            strSql.AppendLine(" And ( Cdg_Testata.Piva = attivita.Piva ")
            strSql.AppendLine(" Or attivita.Sa_Cod = -1) ")
            strSql.AppendLine(" join Imprese_Progetti ")
            strSql.AppendLine(" On CDG_Dettagli.Piva = Imprese_Progetti.Piva ")
            strSql.AppendLine(" And CDG_Dettagli.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            strSql.AppendLine(" And CDG_Dettagli.Appezza = Imprese_Progetti.Appezza ")
            strSql.AppendLine(" And CDG_Dettagli.Id_Reg = Imprese_Progetti.Id_Reg ")
            strSql.AppendLine(" And CDG_Dettagli.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
            strSql.AppendLine(" left join mov_dettagli_riferimenti ")
            strSql.AppendLine(" On AgendaCdG.Piva = mov_dettagli_riferimenti.Piva_Rif ")
            strSql.AppendLine(" And AgendaCdG.Id_agenda = mov_dettagli_riferimenti.Id_agenda_Rif ")
            strSql.AppendLine(" And mov_dettagli_riferimenti.Lav_Cod_Rif = 4500 ")
            strSql.AppendLine(" left join Agenda ")
            strSql.AppendLine(" On Agenda.Piva = mov_dettagli_riferimenti.Piva ")
            strSql.AppendLine(" And Agenda.Id_agenda = mov_dettagli_riferimenti.Id_agenda ")
            'strSql.AppendLine(" Where CDG_Dettagli.Progetto_Cod Not In ( ")
            'strSql.AppendLine(" Select Progetto_Cod From Reg_Impianti_Codici  ")
            'strSql.AppendLine(" Where Piva = CDG_Dettagli.Piva And id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa) & ")")

            strSql.AppendLine(" Where Cdg_Testata.budget = 0 ")

            If data_movimento <> AGRODATAINIZIO Then
                strSql.AppendLine(" And Agenda.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & " ")
                strSql.AppendLine(" And Agenda.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ")
            End If

            'strSql.AppendLine(" And Agenda.Lav_Cod > 0 And Agenda.Lav_Cod < 1000 " )

            strSql.AppendLine(" And CDG_Dettagli.Piva = '" & Agro_SQL_SaveText(kPiva) & "'")
            strSql.AppendLine(" And CDG_Dettagli.Sa_Cod = " & kSa_Cod)
            strSql.AppendLine(" And CDG_Dettagli.Appezza = " & kAppezza)
            strSql.AppendLine(" And CDG_Dettagli.Id_Reg = " & kId_Reg)
            strSql.AppendLine(" And CDG_Dettagli.Progetto_Cod = " & kProgetto_Cod)



            '===================================================================================================================================================
            'Esclusione id_agenda appartenenti a distinte chiuse (a sap non possono arrivare operazioni agenda in modifica su esercizi chiusi..
            'quindi rimanderemmo la stessa percentuale dell'esercizio chiuso e sap scarterebbe il movimento)
            '---------------------------------------------------------------------------------------------------------------------------------------------------
            strSql.AppendLine(" And CDG_Testata.Id_CDG ")

            If Not bEsclusi Then
                strSql.AppendLine(" Not ")

            End If

            strSql.AppendLine(" In (Select Id_CDG From CDG_Dettagli AS CDG_Dettagli2 Where Progetto_Cod In ")
            strSql.AppendLine("            (Select Progetto_Cod From Reg_Impianti_Codici Where Piva = CDG_Dettagli2.Piva And id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa) & " ")
            strSql.AppendLine("                                                          And isnull(Reg_Impianti_Codici.val_cod, '0') = '1') )")

            '===================================================================================================================================================

            strSql.AppendLine(" GROUP BY AgendaCdG.Id_Agenda, AgendaCdG.Lav_Cod, AgendaCdG.Des_Lib, Cdg_Testata.Data_Inserimento,  ")
            strSql.AppendLine(" Agenda.Validita_Inizio ")
            strSql.AppendLine(" ORDER BY DataRegistrazione, Des_Lib")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)


            If bEsclusi Then

                For Each dr As DataRow In dt.Rows

                    'Ricerca Descrizioni               
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Distinct Imprese_Progetti.Progetto_Nome, CDG_Testata.Descrizione ")
                    strSql.AppendLine(" From ")
                    strSql.AppendLine(" CDG_Testata, CDG_Dettagli, Imprese_Progetti ")
                    'strSql.AppendLine(" , Mov_Dettagli_Riferimenti ")
                    strSql.AppendLine(" Where CDG_Dettagli.Progetto_Cod In ")
                    strSql.AppendLine("   ( Select Progetto_Cod From Reg_Impianti_Codici  ")
                    strSql.AppendLine("            Where Piva = CDG_Dettagli.Piva And id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa) & " ")
                    strSql.AppendLine("            And isnull(Reg_Impianti_Codici.val_cod, '0') = '1') ")

                    strSql.AppendLine(" And CDG_Dettagli.Piva = Imprese_Progetti.Piva ")
                    strSql.AppendLine(" And CDG_Dettagli.Sa_Cod = Imprese_Progetti.Sa_Cod ")
                    strSql.AppendLine(" And CDG_Dettagli.Appezza = Imprese_Progetti.Appezza ")
                    strSql.AppendLine(" And CDG_Dettagli.Id_Reg = Imprese_Progetti.Id_Reg ")
                    strSql.AppendLine(" And CDG_Dettagli.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
                    strSql.AppendLine(" And CDG_Testata.Piva = CDG_Dettagli.Piva ")
                    strSql.AppendLine(" And CDG_Testata.ID_Cdg = CDG_Dettagli.ID_Cdg ")
                    strSql.AppendLine(" And CDG_Testata.Budget = 0 ")

                    'strSql.AppendLine(" And Mov_Dettagli_Riferimenti.Piva = CDG_Testata.Piva ")
                    'strSql.AppendLine(" And Mov_Dettagli_Riferimenti.Id_Agenda = " & dr.Item("Id_Agenda") & " ")
                    'strSql.AppendLine(" And Mov_Dettagli_Riferimenti.Piva_Rif = CDG_Testata.Piva ")
                    'strSql.AppendLine(" And Mov_Dettagli_Riferimenti.Id_Agenda_Rif = CDG_Testata.Id_Agenda ")

                    dts = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

                    Elenco_Esercizi = ""
                    Elenco_Risorse = ""

                    For Each drs As DataRow In dts.Rows

                        If InStr(Elenco_Esercizi, drs.Item("Progetto_Nome")) = 0 Then

                            Elenco_Esercizi = Elenco_Esercizi & IIf(Elenco_Esercizi = "", "", ",") & drs.Item("Progetto_Nome")

                        End If

                        If InStr(Elenco_Risorse, drs.Item("Descrizione")) = 0 Then

                            Elenco_Risorse = Elenco_Risorse & IIf(Elenco_Risorse = "", "", ",") & drs.Item("Descrizione")

                        End If




                    Next

                    dr.Item("Elenco_Esercizi") = Elenco_Esercizi
                    dr.Item("Elenco_Risorse") = Elenco_Risorse


                Next

            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :   " & messaggioErrore)
        End Try

        Return dt


    End Function



    Public Function Controlla_Raccoglitore_Agenda(ByVal ElencoAgenda As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                   ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Controlla_Raccoglitore_Agenda()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable


        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * from agenda where raccoglitore_cod <> 0 ")
            strSql.AppendLine(" and raccoglitore_cod in (select raccoglitore_cod from agenda AG1 where agenda.Raccoglitore_Cod = AG1.Raccoglitore_Cod and '" & Agro_SQL_SaveText(ElencoAgenda) & "' Like Convert(varchar,'%|' + convert(varchar,AG1.id_agenda) + '|%')) ")
            strSql.AppendLine(" and raccoglitore_cod in (select raccoglitore_cod from agenda AG2 where agenda.Raccoglitore_Cod = AG2.Raccoglitore_Cod and '" & Agro_SQL_SaveText(ElencoAgenda) & "' like convert(varchar,'%|' + convert(varchar,AG2.id_agenda) + '|%')) ")

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :   " & messaggioErrore)
        End Try

        Return dt


    End Function




    'Nota: se cambia cambiare anche la funzione corrispondente per multicentro (Leggi_Impianti_Dettagli_Multicentro)
    Public Function Leggi_Impianti_Dettagli_Old(ByVal piva As String,
                                            ByVal id_agenda As Long,
                                            ByVal data_movimento As Date,
                                            ByVal bDT As Boolean,
                                            ByVal id_agenda_cdg As Long,
                                            ByVal leggiDettDistinta As Boolean,
                                            ByVal attPoliennale As Boolean,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing
                                           ) As Object

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Impianti_Dettagli()"

        Dim gefutils As New Gias_EF_Utility

        Dim strCau_Mov As String() = {CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO, CAU_RILIEVO_RACCOLTA, CAU_LAVORAZIONE}
        Dim strCau_Progetto As String() = {CAU_PROGETTO_PRODUZIONE}

        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim DT As DataTable

        Dim residuoQuestoImpianto As Decimal = 0
        Dim Residuo As Decimal = 100
        Dim Counter As Integer = 0

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If


        Dim Dettagli =
                From CDG_Dettagli In GiasContext.CDG_Dettagli
                Join CDG_Testata In GiasContext.CDG_Testata
                     On CDG_Testata.Piva Equals CDG_Dettagli.Piva _
                     And CDG_Testata.Id_CDG Equals CDG_Dettagli.Id_CDG
                Where
                  (CDG_Dettagli.Piva_Superuser.Equals(Piva_SuperUser)) _
                  AndAlso (CDG_Dettagli.Piva.Equals(piva)) _
                  AndAlso (CDG_Testata.Id_Agenda = id_agenda_cdg) _
                  AndAlso (CDG_Testata.Budget = 0) _
                  AndAlso (CDG_Dettagli.Id_Reg <> 0)
                Select New With {
                      .Id_CDG = CDG_Dettagli.Id_CDG,
                      .Id_CDG_Dettagli = CDG_Dettagli.Id_CDG_Dettagli,
                      .piva = CDG_Dettagli.Piva,
                      .sa_cod = CDG_Dettagli.Sa_Cod,
                      .appezza = CDG_Dettagli.Appezza,
                      .id_reg = CDG_Dettagli.Id_Reg,
                      .Progetto_Cod = CDG_Dettagli.Progetto_Cod,
                      .Valore = CDG_Dettagli.Valore
                    }

        Dim w_id_cdg = 0
        Dim myListDettagli = Dettagli.ToList()


        For Each obj In myListDettagli
            If w_id_cdg = 0 Then
                w_id_cdg = obj.Id_CDG
            End If
        Next

        Dim myListPrimiDettagli = myListDettagli.Where(Function(x) x.Id_CDG = w_id_cdg)

        Dim contaDistinteAttive As Integer = 0
        Dim DistinteDT As DataTable = Nothing

        ' Cerca il nr di distinte attive ... solo se non devo leggere il dettaglio per distinta (entrata sulla pagina dove poi le distinte
        '  del periodo non corrente le vado a cercare quando si espande il sottolivello) 
        If Not leggiDettDistinta Then
            Dim Distinte =
           From CI In GiasContext.Mov_Destinazioni
           Join Movimenti In GiasContext.Movimenti
             On Movimenti.PIVA Equals CI.Piva And
                Movimenti.Id_Agenda Equals CI.Id_Agenda And
                Movimenti.Id_Mov Equals CI.Id_Mov
           Join Reg_Impianti In GiasContext.Reg_Impianti
                On Reg_Impianti.PIVA Equals CI.Piva And
                   Reg_Impianti.SA_COD Equals CI.Sa_Cod And
                   Reg_Impianti.APPEZZA Equals CI.Appezza And
                   Reg_Impianti.ID_REG Equals CI.Id_Destinazione
           Join Imprese_Progetti In GiasContext.Imprese_Progetti
             On Reg_Impianti.PIVA Equals Imprese_Progetti.Piva And
                 Reg_Impianti.SA_COD Equals Imprese_Progetti.Sa_Cod And
                 Reg_Impianti.APPEZZA Equals Imprese_Progetti.Appezza And
                 Reg_Impianti.ID_REG Equals Imprese_Progetti.Id_Reg
           Group Join Reg_Impianti_Distinta In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Distinta_Chiusa))
             On Reg_Impianti_Distinta.PIVA Equals Imprese_Progetti.Piva And
                Reg_Impianti_Distinta.sa_cod Equals Imprese_Progetti.Sa_Cod And
               Reg_Impianti_Distinta.appezza Equals Imprese_Progetti.Appezza And
               Reg_Impianti_Distinta.Id_Reg Equals Imprese_Progetti.Id_Reg And
               Reg_Impianti_Distinta.Progetto_Cod Equals Imprese_Progetti.Progetto_Cod
                Into Reg_Impianti_Distinta_Group = Group
           From _Reg_Impianti_Distinta_Group In Reg_Impianti_Distinta_Group.DefaultIfEmpty()
           Where
          (CI.Piva.Equals(piva)) _
          AndAlso (CI.Id_Agenda = id_agenda) _
           AndAlso (CI.Tipo_Destinazione = 0) _
           AndAlso strCau_Progetto.Contains(Imprese_Progetti.Cau_Progetto) _
           AndAlso (Imprese_Progetti.Validita_Fine >= data_movimento)
           Select New With {
          .Piva = CI.Piva,
          .Sa_Cod = CI.Sa_Cod,
          .Appezza = CI.Appezza,
          .Id_Destinazione = CI.Id_Destinazione,
          .Progetto_Cod = Imprese_Progetti.Progetto_Cod,
          .Validita_Inizio_Distinta = Imprese_Progetti.Validita_Inizio,
          .Validita_Fine_Distinta = Imprese_Progetti.Validita_Fine,
          .Flag_Distinta_Chiusa = If(_Reg_Impianti_Distinta_Group Is Nothing OrElse _Reg_Impianti_Distinta_Group.val_cod = 0, False, True)
          }

            DistinteDT = gefutils.ObjectQueryToDataTable(Distinte.Distinct().Where(Function(x) Not x.Flag_Distinta_Chiusa).ToList())

        End If

        Dim TestataElem =
           From CI In GiasContext.Mov_Destinazioni
           Join Movimenti In GiasContext.Movimenti
             On Movimenti.PIVA Equals CI.Piva And
                Movimenti.Id_Agenda Equals CI.Id_Agenda And
                Movimenti.Id_Mov Equals CI.Id_Mov
           Join Reg_Impianti In GiasContext.Reg_Impianti
                On Reg_Impianti.PIVA Equals CI.Piva And
                   Reg_Impianti.SA_COD Equals CI.Sa_Cod And
                   Reg_Impianti.APPEZZA Equals CI.Appezza And
                   Reg_Impianti.ID_REG Equals CI.Id_Destinazione
           Join Appezzamento In GiasContext.Appezzamento
                On Appezzamento.PIVA Equals Reg_Impianti.PIVA And
                   Appezzamento.SA_COD Equals Reg_Impianti.SA_COD And
                   Appezzamento.APPEZZA Equals Reg_Impianti.APPEZZA
           Group Join Campi In GiasContext.Campi
               On Appezzamento.PIVA Equals Campi.Piva And
                   Appezzamento.SA_COD Equals Campi.Sa_Cod And
                   Appezzamento.Campo_Cod Equals Campi.Campo_Cod Into Campi_Group = Group
           From _Campi_Group In Campi_Group.DefaultIfEmpty()
           Join Centri_Aziendali In GiasContext.Centri_Aziendali
                On Centri_Aziendali.PIVA Equals Appezzamento.PIVA And
                   Centri_Aziendali.sa_cod Equals Appezzamento.SA_COD
           Join Imprese_Progetti In GiasContext.Imprese_Progetti
             On Reg_Impianti.PIVA Equals Imprese_Progetti.Piva And
                 Reg_Impianti.SA_COD Equals Imprese_Progetti.Sa_Cod And
                 Reg_Impianti.APPEZZA Equals Imprese_Progetti.Appezza And
                 Reg_Impianti.ID_REG Equals Imprese_Progetti.Id_Reg
           Group Join Cultivar In GiasContext.Cultivar
             On Cultivar.Cul_Cod Equals Reg_Impianti.CUL_COD Into Cultivar_Group = Group
           From _Cultivar_Group In Cultivar_Group.DefaultIfEmpty()
           Group Join Specie In GiasContext.SpecieVegetali
             On Specie.Veg_Cod Equals _Cultivar_Group.Veg_Cod Into Specie_Group = Group
           From _Specie_Group In Specie_Group.DefaultIfEmpty()
           Group Join Reg_Impianti_Distinta In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Distinta_Chiusa))
             On Reg_Impianti_Distinta.PIVA Equals Imprese_Progetti.Piva And
                Reg_Impianti_Distinta.sa_cod Equals Imprese_Progetti.Sa_Cod And
               Reg_Impianti_Distinta.appezza Equals Imprese_Progetti.Appezza And
               Reg_Impianti_Distinta.Id_Reg Equals Imprese_Progetti.Id_Reg And
               Reg_Impianti_Distinta.Progetto_Cod Equals Imprese_Progetti.Progetto_Cod
                Into Reg_Impianti_Distinta_Group = Group
           From _Reg_Impianti_Distinta_Group In Reg_Impianti_Distinta_Group.DefaultIfEmpty()
           Group Join Reg_Impianti_CodiceImp In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Codice_Impianto))
             On Reg_Impianti_CodiceImp.PIVA Equals Reg_Impianti.PIVA And
                Reg_Impianti_CodiceImp.sa_cod Equals Reg_Impianti.SA_COD And
               Reg_Impianti_CodiceImp.appezza Equals Reg_Impianti.APPEZZA And
               Reg_Impianti_CodiceImp.Id_Reg Equals Reg_Impianti.ID_REG
                Into Reg_Impianti_CodiceImp_Group = Group
           From _Reg_Impianti_CodiceImp_Group In Reg_Impianti_CodiceImp_Group.DefaultIfEmpty()
           Where
          (CI.Piva.Equals(piva)) _
          AndAlso (CI.Id_Agenda = id_agenda) _
           AndAlso (CI.Tipo_Destinazione = 0) _
           AndAlso strCau_Progetto.Contains(Imprese_Progetti.Cau_Progetto) _
           AndAlso (leggiDettDistinta OrElse Imprese_Progetti.Validita_Inizio <= data_movimento) _
           AndAlso (leggiDettDistinta OrElse Imprese_Progetti.Validita_Fine >= data_movimento)
           Order By CI.QuotaDistribuzione Descending,
               CI.Piva,
               CI.Sa_Cod,
               CI.Appezza,
               CI.Id_Destinazione
           Select New With {
          .Piva = CI.Piva,
          .Sa_Cod = Centri_Aziendali.sa_cod,
          .Sa_Nome = Centri_Aziendali.sa_nome,
          .Campo_Cod = If(_Campi_Group Is Nothing, 0, _Campi_Group.Campo_Cod),
          .Appezza = CI.Appezza,
          .Id_Agenda = CI.Id_Agenda,
          .Id_Destinazione = CI.Id_Destinazione,
          .Key = "",
          .Veg_Cod = If(_Specie_Group Is Nothing, 0, _Specie_Group.Veg_Cod),
          .Veg_Des = If(_Specie_Group Is Nothing, "", _Specie_Group.Veg_Des),
          .Cul_Cod = If(_Cultivar_Group Is Nothing, 0, _Cultivar_Group.Cul_Cod),
          .Cul_Des = If(_Cultivar_Group Is Nothing, Str_TerrenoNudo, _Cultivar_Group.Cul_Des),
          .Descrizione = Appezzamento.APP_NOME,
          .Progetto_Cod = Imprese_Progetti.Progetto_Cod,
          .Progetto_Nome = Imprese_Progetti.Progetto_Nome,
          .Progetto_Des = Imprese_Progetti.Progetto_Des,
          .Validita_Inizio_Distinta = Imprese_Progetti.Validita_Inizio,
          .Validita_Fine_Distinta = Imprese_Progetti.Validita_Fine,
          .Flag_Distinta_Chiusa = If(_Reg_Impianti_Distinta_Group Is Nothing OrElse _Reg_Impianti_Distinta_Group.val_cod = 0, False, True),
          .Codice_Impianto = If(_Reg_Impianti_CodiceImp_Group Is Nothing, "", _Reg_Impianti_CodiceImp_Group.val_cod),
          .Superficie = CI.Qta2,
          .Valore = If(CI.QuotaDistribuzione Is Nothing, 0, CI.QuotaDistribuzione * 100),
          .ValoreMassimo = If(CI.QuotaDistribuzione Is Nothing, 0, CI.QuotaDistribuzione * 100)
          }

        Dim myList = TestataElem.Distinct().ToList()




        Dim objValoreMax As Decimal = 0

        Dim w_Piva = ""
        Dim w_SaCod = 0
        Dim w_Appezza = 0
        Dim w_IdReg = 0
        Dim htResidui As New Hashtable()
        For Each obj In myList

            If leggiDettDistinta Then
                If w_Piva <> "" Then
                    'Al cambio di impianto memorizzo i residui a parità di chiave
                    If w_Piva <> obj.Piva OrElse w_SaCod <> obj.Sa_Cod OrElse w_Appezza <> obj.Appezza OrElse w_IdReg <> obj.Id_Destinazione Then
                        If residuoQuestoImpianto <> 0 Then
                            htResidui.Add(w_Piva & "|" & CStr(w_SaCod) & "|" & CStr(w_Appezza) & "|" & CStr(w_IdReg), residuoQuestoImpianto)
                        End If
                        residuoQuestoImpianto = Decimal.Round(Convert.ToDecimal(obj.ValoreMassimo), 2)
                        w_Piva = obj.Piva
                        w_SaCod = obj.Sa_Cod
                        w_Appezza = obj.Appezza
                        w_IdReg = obj.Id_Destinazione


                    Else


                    End If
                Else
                    'Primo Giro
                    residuoQuestoImpianto = Decimal.Round(Convert.ToDecimal(obj.ValoreMassimo), 2)
                    w_Piva = obj.Piva
                    w_SaCod = obj.Sa_Cod
                    w_Appezza = obj.Appezza
                    w_IdReg = obj.Id_Destinazione
                End If
            End If

            ' Cerco quante distinte sono attive dalla data movimento in poi testando sia la data che il flag distinta chiusa
            ' Solo se non l'ho già letto prima
            If leggiDettDistinta Then
                contaDistinteAttive = myList.Where(Function(x) x.Piva = obj.Piva AndAlso x.Sa_Cod = obj.Sa_Cod AndAlso x.Appezza = obj.Appezza AndAlso x.Id_Destinazione = obj.Id_Destinazione AndAlso Not x.Flag_Distinta_Chiusa AndAlso x.Validita_Fine_Distinta >= data_movimento).Count
            Else

                If DistinteDT IsNot Nothing Then
                    contaDistinteAttive = (From elenco_distinte In DistinteDT
                                            Where elenco_distinte("Piva") = obj.Piva AndAlso
                                                  elenco_distinte("Sa_Cod") = obj.Sa_Cod AndAlso
                                                  elenco_distinte("Appezza") = obj.Appezza AndAlso
                                                  elenco_distinte("Id_Destinazione") = obj.Id_Destinazione
                                            Select elenco_distinte).Count()
                End If

            End If

            obj.Key = obj.Piva & "-" & obj.Sa_Cod.ToString() & "-" & obj.Campo_Cod.ToString() & "-" & obj.Appezza.ToString() & "-" & obj.Id_Destinazione.ToString() & "-" & obj.Progetto_Cod.ToString()

            Counter += 1

            Dim myListPrimiDettagli_UnRecord = myListPrimiDettagli.Where(Function(x) x.piva = obj.Piva And x.sa_cod = obj.Sa_Cod And x.appezza = obj.Appezza And x.id_reg = obj.Id_Destinazione And x.Progetto_Cod = obj.Progetto_Cod).FirstOrDefault

            If myListPrimiDettagli_UnRecord IsNot Nothing Then
                'Sono nel caso in cui sono in modifica ed era già presente una % su un impianto o una distinta
                obj.ValoreMassimo = (From s In myListPrimiDettagli.Where(Function(x) x.piva = obj.Piva And x.sa_cod = obj.Sa_Cod And x.appezza = obj.Appezza And x.id_reg = obj.Id_Destinazione) Select s.Valore).Sum()
                obj.Valore = Decimal.Round(Convert.ToDecimal(myListPrimiDettagli_UnRecord.Valore), 2)
                If obj.Flag_Distinta_Chiusa Then
                    ' Necessario perchè se rientro in modifica e la distinta corrente è chiusa non la conterebbe fra quelle
                    ' su cui suddividere e il risultato finale sarebbe più alto di 100
                    contaDistinteAttive += 1

                Else

                    '05/07/2021: Correzione Bug
                    residuoQuestoImpianto = residuoQuestoImpianto - Decimal.Round(Convert.ToDecimal(obj.Valore), 2)

                End If
            Else
                'Sono nel caso di nuove righe
                obj.ValoreMassimo = Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                'obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                If Not attPoliennale Then
                    ' Se non è poliennale assegno la % solo se al periodo corrente se la relativa distinta è aperta
                    If obj.Validita_Inizio_Distinta <= data_movimento AndAlso
                            obj.Validita_Fine_Distinta >= data_movimento AndAlso
                            Not obj.Flag_Distinta_Chiusa Then
                        obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                        residuoQuestoImpianto = residuoQuestoImpianto - Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                    Else
                        obj.Valore = 0
                    End If
                Else
                    ' Se è poliennale assegno la quota parte di % solo se al periodo corrente e a quelli futuri la cui distinta è aperta
                    If contaDistinteAttive > 0 AndAlso Not obj.Flag_Distinta_Chiusa AndAlso obj.Validita_Fine_Distinta >= data_movimento Then
                        obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore) / contaDistinteAttive, 2)
                        residuoQuestoImpianto = residuoQuestoImpianto - Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                    Else
                        obj.Valore = 0
                    End If
                End If

            End If

            'Residuo = Residuo - obj.ValoreMassimo
            Residuo = Residuo - Decimal.Round(Convert.ToDecimal(obj.Valore), 2)

            If obj.ValoreMassimo > objValoreMax Then
                objValoreMax = obj.ValoreMassimo
            End If

        Next


        If leggiDettDistinta Then

            'Ultimo giro
            If w_Piva <> "" AndAlso residuoQuestoImpianto <> 0 Then
                htResidui.Add(w_Piva & "|" & CStr(w_SaCod) & "|" & CStr(w_Appezza) & "|" & CStr(w_IdReg), residuoQuestoImpianto)
            End If

            ' SISTEMAZIONE RESIDUI
            'PRIMA PARTE: Sistemo i residui sullo specifico impianto con più distinte e sottraggo dallo sfrido generale

            For Each k In htResidui.Keys
                For Each obj In myList

                    If CStr(k) = obj.Piva & "|" & CStr(obj.Sa_Cod) & "|" & CStr(obj.Appezza) & "|" & CStr(obj.Id_Destinazione) Then
                        obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore) + htResidui(k), 2)
                        Residuo = Residuo - htResidui(k)
                        Exit For
                    End If
                Next
            Next
        End If

        'SECONDA PARTE: Metto l'eventuale sfrido finale sulla riga con valore maggiore
        If leggiDettDistinta AndAlso objValoreMax > 0 AndAlso Residuo > 0 Then
            w_Piva = ""
            w_SaCod = 0
            w_Appezza = 0
            w_IdReg = 0
            If objValoreMax > 0 AndAlso Residuo <> 0 Then
                For Each obj In myList
                    If obj.ValoreMassimo = objValoreMax Then
                        '''If leggiDettDistinta OrElse (Not leggiDettDistinta AndAlso contaDistinteAttive = 1) THEN
                        obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore) + Residuo, 2)
                        w_Piva = obj.Piva
                        w_SaCod = obj.Sa_Cod
                        w_Appezza = obj.Appezza
                        w_IdReg = obj.Id_Destinazione
                        Exit For
                    End If
                Next
                'Devo sistemare anche il valore massimo in tutte le righe con stessa chiave
                If w_Piva <> "" Then
                    For Each obj In myList
                        If w_Piva = obj.Piva AndAlso
                            w_SaCod = obj.Sa_Cod AndAlso
                            w_Appezza = obj.Appezza AndAlso
                            w_IdReg = obj.Id_Destinazione Then
                            obj.ValoreMassimo = Decimal.Round(Convert.ToDecimal(obj.ValoreMassimo) + Residuo, 2)
                        End If
                    Next
                End If
            End If
        End If

        ' Sistemazione sfrido decimali quando non leggo il dettaglio delle distinte
        If Not leggiDettDistinta Then
            Dim max As Decimal = 100
            Dim righe = myList.Count
            Dim c = 0
            For Each obj In myList
                c += 1
                If c = righe Then
                    'Se ValoreMassimo e Valore non sono uguali significa che siamo nel caso di poliannuale
                    If obj.ValoreMassimo = obj.Valore Then
                        obj.ValoreMassimo = max
                        obj.Valore = max
                    Else
                        obj.ValoreMassimo = max
                    End If
                Else
                    max -= obj.ValoreMassimo
                End If
            Next
        End If

        Select Case bDT

            Case True

                Dim ut As New Gias_EF_Utility
                DT = ut.ObjectQueryToDataTable(myList)

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return DT

            Case False

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return risposta

        End Select

        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

        Return Nothing

    End Function





    'Nota: se cambia cambiare anche la funzione corrispondente per multicentro (Leggi_Impianti_Dettagli_Multicentro)
    Public Function Leggi_Impianti_Dettagli(ByVal piva As String,
                                            ByVal id_agenda As Long,
                                            ByVal data_movimento As Date,
                                            ByVal bDT As Boolean,
                                            ByVal id_agenda_cdg As Long,
                                            ByVal leggiDettDistinta As Boolean,
                                            ByVal attPoliennale As Boolean,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing
                                           ) As Object

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Impianti_Dettagli()"

        Dim gefutils As New Gias_EF_Utility

        Dim strCau_Mov As String() = {CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO, CAU_RILIEVO_RACCOLTA, CAU_LAVORAZIONE}
        Dim strCau_Progetto As String() = {CAU_PROGETTO_PRODUZIONE}

        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim DT As DataTable

        Dim residuoQuestoImpianto As Decimal = 0
        Dim Residuo As Decimal = 100
        Dim Counter As Integer = 0

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If


        Dim Dettagli =
                From CDG_Dettagli In GiasContext.CDG_Dettagli
                Join CDG_Testata In GiasContext.CDG_Testata
                     On CDG_Testata.Piva Equals CDG_Dettagli.Piva _
                     And CDG_Testata.Id_CDG Equals CDG_Dettagli.Id_CDG
                Where
                  (CDG_Dettagli.Piva_Superuser.Equals(Piva_SuperUser)) _
                  AndAlso (CDG_Dettagli.Piva.Equals(piva)) _
                  AndAlso (CDG_Testata.Id_Agenda = id_agenda_cdg) _
                  AndAlso (CDG_Testata.Budget = 0) _
                  AndAlso (CDG_Dettagli.Id_Reg <> 0)
                Select New With {
                      .Id_CDG = CDG_Dettagli.Id_CDG,
                      .Id_CDG_Dettagli = CDG_Dettagli.Id_CDG_Dettagli,
                      .piva = CDG_Dettagli.Piva,
                      .sa_cod = CDG_Dettagli.Sa_Cod,
                      .appezza = CDG_Dettagli.Appezza,
                      .id_reg = CDG_Dettagli.Id_Reg,
                      .Progetto_Cod = CDG_Dettagli.Progetto_Cod,
                      .Valore = CDG_Dettagli.Valore
                    }

        Dim w_id_cdg = 0
        Dim myListDettagli = Dettagli.ToList()


        For Each obj In myListDettagli
            If w_id_cdg = 0 Then
                w_id_cdg = obj.Id_CDG
            End If
        Next

        Dim myListPrimiDettagli = myListDettagli.Where(Function(x) x.Id_CDG = w_id_cdg)

        Dim contaDistinteAttive As Integer = 0
        Dim DistinteDT As DataTable = Nothing

        ' Cerca il nr di distinte attive ... solo se non devo leggere il dettaglio per distinta (entrata sulla pagina dove poi le distinte
        '  del periodo non corrente le vado a cercare quando si espande il sottolivello) 
        If Not leggiDettDistinta Then
            Dim Distinte =
           From CI In GiasContext.Mov_Destinazioni
           Join Movimenti In GiasContext.Movimenti
             On Movimenti.PIVA Equals CI.Piva And
                Movimenti.Id_Agenda Equals CI.Id_Agenda And
                Movimenti.Id_Mov Equals CI.Id_Mov
           Join Reg_Impianti In GiasContext.Reg_Impianti
                On Reg_Impianti.PIVA Equals CI.Piva And
                   Reg_Impianti.SA_COD Equals CI.Sa_Cod And
                   Reg_Impianti.APPEZZA Equals CI.Appezza And
                   Reg_Impianti.ID_REG Equals CI.Id_Destinazione
           Join Imprese_Progetti In GiasContext.Imprese_Progetti
             On Reg_Impianti.PIVA Equals Imprese_Progetti.Piva And
                 Reg_Impianti.SA_COD Equals Imprese_Progetti.Sa_Cod And
                 Reg_Impianti.APPEZZA Equals Imprese_Progetti.Appezza And
                 Reg_Impianti.ID_REG Equals Imprese_Progetti.Id_Reg
           Group Join Reg_Impianti_Distinta In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Distinta_Chiusa))
             On Reg_Impianti_Distinta.PIVA Equals Imprese_Progetti.Piva And
                Reg_Impianti_Distinta.sa_cod Equals Imprese_Progetti.Sa_Cod And
               Reg_Impianti_Distinta.appezza Equals Imprese_Progetti.Appezza And
               Reg_Impianti_Distinta.Id_Reg Equals Imprese_Progetti.Id_Reg And
               Reg_Impianti_Distinta.Progetto_Cod Equals Imprese_Progetti.Progetto_Cod
                Into Reg_Impianti_Distinta_Group = Group
           From _Reg_Impianti_Distinta_Group In Reg_Impianti_Distinta_Group.DefaultIfEmpty()
           Where
          (CI.Piva.Equals(piva)) _
          AndAlso (CI.Id_Agenda = id_agenda) _
           AndAlso (CI.Tipo_Destinazione = 0) _
           AndAlso strCau_Progetto.Contains(Imprese_Progetti.Cau_Progetto) _
           AndAlso (Imprese_Progetti.Validita_Fine >= data_movimento)
           Select New With {
          .Piva = CI.Piva,
          .Sa_Cod = CI.Sa_Cod,
          .Appezza = CI.Appezza,
          .Id_Destinazione = CI.Id_Destinazione,
          .Progetto_Cod = Imprese_Progetti.Progetto_Cod,
          .Validita_Inizio_Distinta = Imprese_Progetti.Validita_Inizio,
          .Validita_Fine_Distinta = Imprese_Progetti.Validita_Fine,
          .Flag_Distinta_Chiusa = If(_Reg_Impianti_Distinta_Group Is Nothing OrElse _Reg_Impianti_Distinta_Group.val_cod = 0, False, True)
          }

            DistinteDT = gefutils.ObjectQueryToDataTable(Distinte.Distinct().Where(Function(x) Not x.Flag_Distinta_Chiusa).ToList())

        End If

        Dim TestataElem =
           From CI In GiasContext.Mov_Destinazioni
           Join Movimenti In GiasContext.Movimenti
             On Movimenti.PIVA Equals CI.Piva And
                Movimenti.Id_Agenda Equals CI.Id_Agenda And
                Movimenti.Id_Mov Equals CI.Id_Mov
           Join Reg_Impianti In GiasContext.Reg_Impianti
                On Reg_Impianti.PIVA Equals CI.Piva And
                   Reg_Impianti.SA_COD Equals CI.Sa_Cod And
                   Reg_Impianti.APPEZZA Equals CI.Appezza And
                   Reg_Impianti.ID_REG Equals CI.Id_Destinazione
           Join Appezzamento In GiasContext.Appezzamento
                On Appezzamento.PIVA Equals Reg_Impianti.PIVA And
                   Appezzamento.SA_COD Equals Reg_Impianti.SA_COD And
                   Appezzamento.APPEZZA Equals Reg_Impianti.APPEZZA
           Group Join Campi In GiasContext.Campi
               On Appezzamento.PIVA Equals Campi.Piva And
                   Appezzamento.SA_COD Equals Campi.Sa_Cod And
                   Appezzamento.Campo_Cod Equals Campi.Campo_Cod Into Campi_Group = Group
           From _Campi_Group In Campi_Group.DefaultIfEmpty()
           Join Centri_Aziendali In GiasContext.Centri_Aziendali
                On Centri_Aziendali.PIVA Equals Appezzamento.PIVA And
                   Centri_Aziendali.sa_cod Equals Appezzamento.SA_COD
           Join Imprese_Progetti In GiasContext.Imprese_Progetti
             On Reg_Impianti.PIVA Equals Imprese_Progetti.Piva And
                 Reg_Impianti.SA_COD Equals Imprese_Progetti.Sa_Cod And
                 Reg_Impianti.APPEZZA Equals Imprese_Progetti.Appezza And
                 Reg_Impianti.ID_REG Equals Imprese_Progetti.Id_Reg
           Group Join Cultivar In GiasContext.Cultivar
             On Cultivar.Cul_Cod Equals Reg_Impianti.CUL_COD Into Cultivar_Group = Group
           From _Cultivar_Group In Cultivar_Group.DefaultIfEmpty()
           Group Join Specie In GiasContext.SpecieVegetali
             On Specie.Veg_Cod Equals _Cultivar_Group.Veg_Cod Into Specie_Group = Group
           From _Specie_Group In Specie_Group.DefaultIfEmpty()
           Group Join Reg_Impianti_Distinta In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Distinta_Chiusa))
             On Reg_Impianti_Distinta.PIVA Equals Imprese_Progetti.Piva And
                Reg_Impianti_Distinta.sa_cod Equals Imprese_Progetti.Sa_Cod And
               Reg_Impianti_Distinta.appezza Equals Imprese_Progetti.Appezza And
               Reg_Impianti_Distinta.Id_Reg Equals Imprese_Progetti.Id_Reg And
               Reg_Impianti_Distinta.Progetto_Cod Equals Imprese_Progetti.Progetto_Cod
                Into Reg_Impianti_Distinta_Group = Group
           From _Reg_Impianti_Distinta_Group In Reg_Impianti_Distinta_Group.DefaultIfEmpty()
           Group Join Reg_Impianti_CodiceImp In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Codice_Impianto))
             On Reg_Impianti_CodiceImp.PIVA Equals Reg_Impianti.PIVA And
                Reg_Impianti_CodiceImp.sa_cod Equals Reg_Impianti.SA_COD And
               Reg_Impianti_CodiceImp.appezza Equals Reg_Impianti.APPEZZA And
               Reg_Impianti_CodiceImp.Id_Reg Equals Reg_Impianti.ID_REG
                Into Reg_Impianti_CodiceImp_Group = Group
           From _Reg_Impianti_CodiceImp_Group In Reg_Impianti_CodiceImp_Group.DefaultIfEmpty()
           Where
          (CI.Piva.Equals(piva)) _
          AndAlso (CI.Id_Agenda = id_agenda) _
           AndAlso (CI.Tipo_Destinazione = 0) _
           AndAlso strCau_Progetto.Contains(Imprese_Progetti.Cau_Progetto) _
           AndAlso (leggiDettDistinta OrElse Imprese_Progetti.Validita_Inizio <= data_movimento) _
           AndAlso (leggiDettDistinta OrElse Imprese_Progetti.Validita_Fine >= data_movimento)
           Order By CI.QuotaDistribuzione Descending,
               CI.Piva,
               CI.Sa_Cod,
               CI.Appezza,
               CI.Id_Destinazione
           Select New With {
          .Piva = CI.Piva,
          .Sa_Cod = Centri_Aziendali.sa_cod,
          .Sa_Nome = Centri_Aziendali.sa_nome,
          .Campo_Cod = If(_Campi_Group Is Nothing, 0, _Campi_Group.Campo_Cod),
          .Appezza = CI.Appezza,
          .Id_Agenda = CI.Id_Agenda,
          .Id_Destinazione = CI.Id_Destinazione,
          .Key = "",
          .Veg_Cod = If(_Specie_Group Is Nothing, 0, _Specie_Group.Veg_Cod),
          .Veg_Des = If(_Specie_Group Is Nothing, "", _Specie_Group.Veg_Des),
          .Cul_Cod = If(_Cultivar_Group Is Nothing, 0, _Cultivar_Group.Cul_Cod),
          .Cul_Des = If(_Cultivar_Group Is Nothing, Str_TerrenoNudo, _Cultivar_Group.Cul_Des),
          .Descrizione = Appezzamento.APP_NOME,
          .Progetto_Cod = Imprese_Progetti.Progetto_Cod,
          .Progetto_Nome = Imprese_Progetti.Progetto_Nome,
          .Progetto_Des = Imprese_Progetti.Progetto_Des,
          .Validita_Inizio_Distinta = Imprese_Progetti.Validita_Inizio,
          .Validita_Fine_Distinta = Imprese_Progetti.Validita_Fine,
          .Flag_Distinta_Chiusa = If(_Reg_Impianti_Distinta_Group Is Nothing OrElse _Reg_Impianti_Distinta_Group.val_cod = 0, False, True),
          .Codice_Impianto = If(_Reg_Impianti_CodiceImp_Group Is Nothing, "", _Reg_Impianti_CodiceImp_Group.val_cod),
          .Superficie = CI.Qta2,
          .Valore = If(CI.QuotaDistribuzione Is Nothing, 0, CI.QuotaDistribuzione * 100),
          .ValoreMassimo = If(CI.QuotaDistribuzione Is Nothing, 0, CI.QuotaDistribuzione * 100)
          }

        Dim myList = TestataElem.Distinct().ToList()

        myList = (From myImp In myList Order By myImp.Valore Descending, myImp.Piva, myImp.Sa_Cod, myImp.Appezza, myImp.Id_Destinazione).ToList()


        Dim objValoreMax As Decimal = 0

        Dim w_Piva = ""
        Dim w_SaCod = 0
        Dim w_Appezza = 0
        Dim w_IdReg = 0
        Dim htResidui As New Hashtable()
        For Each obj In myList

            If leggiDettDistinta Then
                If w_Piva <> "" Then
                    'Al cambio di impianto memorizzo i residui a parità di chiave (aggiungo anche controllo duplicato perché l'ordinamento in EF a volte è errato sui decimali)
                    If (w_Piva <> obj.Piva OrElse w_SaCod <> obj.Sa_Cod OrElse w_Appezza <> obj.Appezza OrElse w_IdReg <> obj.Id_Destinazione) AndAlso
                        Not htResidui.ContainsKey(w_Piva & "|" & CStr(w_SaCod) & "|" & CStr(w_Appezza) & "|" & CStr(w_IdReg)) Then
                        If residuoQuestoImpianto <> 0 Then
                            htResidui.Add(w_Piva & "|" & CStr(w_SaCod) & "|" & CStr(w_Appezza) & "|" & CStr(w_IdReg), residuoQuestoImpianto)
                        End If
                        residuoQuestoImpianto = Decimal.Round(Convert.ToDecimal(obj.ValoreMassimo), 2)
                            w_Piva = obj.Piva
                            w_SaCod = obj.Sa_Cod
                            w_Appezza = obj.Appezza
                        w_IdReg = obj.Id_Destinazione
                    End If
                    Else
                        'Primo Giro
                        residuoQuestoImpianto = Decimal.Round(Convert.ToDecimal(obj.ValoreMassimo), 2)
                    w_Piva = obj.Piva
                    w_SaCod = obj.Sa_Cod
                    w_Appezza = obj.Appezza
                    w_IdReg = obj.Id_Destinazione
                End If
            End If

            ' Cerco quante distinte sono attive dalla data movimento in poi testando sia la data che il flag distinta chiusa
            ' Solo se non l'ho già letto prima
            If leggiDettDistinta Then
                contaDistinteAttive = myList.Where(Function(x) x.Piva = obj.Piva AndAlso x.Sa_Cod = obj.Sa_Cod AndAlso x.Appezza = obj.Appezza AndAlso x.Id_Destinazione = obj.Id_Destinazione AndAlso Not x.Flag_Distinta_Chiusa AndAlso x.Validita_Fine_Distinta >= data_movimento).Count
            Else

                If DistinteDT IsNot Nothing Then
                    contaDistinteAttive = (From elenco_distinte In DistinteDT
                                            Where elenco_distinte("Piva") = obj.Piva AndAlso
                                                  elenco_distinte("Sa_Cod") = obj.Sa_Cod AndAlso
                                                  elenco_distinte("Appezza") = obj.Appezza AndAlso
                                                  elenco_distinte("Id_Destinazione") = obj.Id_Destinazione
                                            Select elenco_distinte).Count()
                End If

            End If

            obj.Key = obj.Piva & "-" & obj.Sa_Cod.ToString() & "-" & obj.Campo_Cod.ToString() & "-" & obj.Appezza.ToString() & "-" & obj.Id_Destinazione.ToString() & "-" & obj.Progetto_Cod.ToString()

            Counter += 1

            Dim myListPrimiDettagli_UnRecord = myListPrimiDettagli.Where(Function(x) x.piva = obj.Piva And x.sa_cod = obj.Sa_Cod And x.appezza = obj.Appezza And x.id_reg = obj.Id_Destinazione And x.Progetto_Cod = obj.Progetto_Cod).FirstOrDefault

            If myListPrimiDettagli_UnRecord IsNot Nothing Then
                'Sono nel caso in cui sono in modifica ed era già presente una % su un impianto o una distinta
                obj.ValoreMassimo = (From s In myListPrimiDettagli.Where(Function(x) x.piva = obj.Piva And x.sa_cod = obj.Sa_Cod And x.appezza = obj.Appezza And x.id_reg = obj.Id_Destinazione) Select s.Valore).Sum()
                obj.Valore = Decimal.Round(Convert.ToDecimal(myListPrimiDettagli_UnRecord.Valore), 2)
                If obj.Flag_Distinta_Chiusa Then
                    ' Necessario perchè se rientro in modifica e la distinta corrente è chiusa non la conterebbe fra quelle
                    ' su cui suddividere e il risultato finale sarebbe più alto di 100
                    contaDistinteAttive += 1

                Else

                    '05/07/2021: Correzione Bug
                    residuoQuestoImpianto = residuoQuestoImpianto - Decimal.Round(Convert.ToDecimal(obj.Valore), 2)

                End If
            Else
                'Sono nel caso di nuove righe
                obj.ValoreMassimo = Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                'obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                If Not attPoliennale Then
                    ' Se non è poliennale assegno la % solo se al periodo corrente se la relativa distinta è aperta
                    If obj.Validita_Inizio_Distinta <= data_movimento AndAlso
                            obj.Validita_Fine_Distinta >= data_movimento AndAlso
                            Not obj.Flag_Distinta_Chiusa Then
                        obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                        residuoQuestoImpianto = residuoQuestoImpianto - Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                    Else
                        obj.Valore = 0
                    End If
                Else
                    ' Se è poliennale assegno la quota parte di % solo se al periodo corrente e a quelli futuri la cui distinta è aperta
                    If contaDistinteAttive > 0 AndAlso Not obj.Flag_Distinta_Chiusa AndAlso obj.Validita_Fine_Distinta >= data_movimento Then
                        obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore) / contaDistinteAttive, 2)
                        residuoQuestoImpianto = residuoQuestoImpianto - Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                    Else
                        obj.Valore = 0
                    End If
                End If

            End If

            'Residuo = Residuo - obj.ValoreMassimo
            Residuo = Residuo - Decimal.Round(Convert.ToDecimal(obj.Valore), 2)

            If obj.ValoreMassimo > objValoreMax Then
                objValoreMax = obj.ValoreMassimo
            End If

        Next


        If leggiDettDistinta Then

            'Ultimo giro
            If w_Piva <> "" AndAlso residuoQuestoImpianto <> 0 AndAlso
                        Not htResidui.ContainsKey(w_Piva & "|" & CStr(w_SaCod) & "|" & CStr(w_Appezza) & "|" & CStr(w_IdReg)) Then
                htResidui.Add(w_Piva & "|" & CStr(w_SaCod) & "|" & CStr(w_Appezza) & "|" & CStr(w_IdReg), residuoQuestoImpianto)
            End If

            ' SISTEMAZIONE RESIDUI
            'PRIMA PARTE: Sistemo i residui sullo specifico impianto con più distinte e sottraggo dallo sfrido generale

            For Each k In htResidui.Keys
                For Each obj In myList

                    If CStr(k) = obj.Piva & "|" & CStr(obj.Sa_Cod) & "|" & CStr(obj.Appezza) & "|" & CStr(obj.Id_Destinazione) And obj.Validita_Fine_Distinta >= data_movimento And Not obj.Flag_Distinta_Chiusa Then
                        obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore) + htResidui(k), 2)
                        Residuo = Residuo - htResidui(k)
                        Exit For
                    End If
                Next
            Next
        End If

        'SECONDA PARTE: Metto l'eventuale sfrido finale sulla riga con valore maggiore
        If leggiDettDistinta AndAlso objValoreMax > 0 AndAlso Residuo <> 0 Then
            w_Piva = ""
            w_SaCod = 0
            w_Appezza = 0
            w_IdReg = 0
            If objValoreMax > 0 Then
                For Each obj In myList
                    If obj.ValoreMassimo = objValoreMax And obj.Validita_Fine_Distinta >= data_movimento Then
                        '''If leggiDettDistinta OrElse (Not leggiDettDistinta AndAlso contaDistinteAttive = 1) THEN
                        obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore) + Residuo, 2)
                        w_Piva = obj.Piva
                        w_SaCod = obj.Sa_Cod
                        w_Appezza = obj.Appezza
                        w_IdReg = obj.Id_Destinazione
                        Exit For
                    End If
                Next
                'Devo sistemare anche il valore massimo in tutte le righe con stessa chiave
                If w_Piva <> "" Then
                    For Each obj In myList
                        If w_Piva = obj.Piva AndAlso
                            w_SaCod = obj.Sa_Cod AndAlso
                            w_Appezza = obj.Appezza AndAlso
                            w_IdReg = obj.Id_Destinazione Then
                            obj.ValoreMassimo = Decimal.Round(Convert.ToDecimal(obj.ValoreMassimo) + Residuo, 2)
                        End If
                    Next
                End If
            End If
        End If

        ' Sistemazione sfrido decimali quando non leggo il dettaglio delle distinte
        If Not leggiDettDistinta Then
            Dim max As Decimal = 100
            Dim righe = myList.Count
            Dim c = 0
            For Each obj In myList
                c += 1
                If c = righe Then
                    'Se ValoreMassimo e Valore non sono uguali significa che siamo nel caso di poliannuale
                    If obj.ValoreMassimo = obj.Valore Then
                        obj.ValoreMassimo = max
                        obj.Valore = max
                    Else
                        obj.ValoreMassimo = max
                    End If
                Else
                    max -= Convert.ToDecimal(obj.ValoreMassimo)
                End If
            Next
        End If

        Select Case bDT

            Case True

                Dim ut As New Gias_EF_Utility
                DT = ut.ObjectQueryToDataTable(myList)

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return DT

            Case False

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return risposta

        End Select

        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

        Return Nothing

    End Function




    Public Function Leggi_Impianti_Dettagli_Multicentro(ByVal piva As String,
                                                        ByVal Raccoglitore_Cod As Long,
                                                        ByVal data_movimento As Date,
                                                        ByVal DtRipartizione As DataTable,
                                                        ByVal TotaleRipartizione As Decimal,
                                                        ByVal bDT As Boolean,
                                                        ByVal id_agenda_cdg As Long,
                                                        ByVal leggiDettDistinta As Boolean,
                                                        ByVal attPoliennale As Boolean,
                                                        ByRef objParametri As AgronicaCoreParametri,
                                                        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing
                                                        ) As Object

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Impianti_Dettagli_Multicentro()"

        Dim gefutils As New Gias_EF_Utility

        Dim strCau_Mov As String() = {CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO, CAU_RILIEVO_RACCOLTA, CAU_LAVORAZIONE}
        Dim strCau_Progetto As String() = {CAU_PROGETTO_PRODUZIONE}

        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim DT As DataTable

        Dim residuoQuestoImpianto As Decimal = 0
        Dim Residuo As Decimal = 100
        Dim Counter As Integer = 0
        Dim Counter2 As Integer = 0
        Dim DTDettagli As DataTable = Nothing

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If


        Dim Dettagli =
                From CDG_Dettagli In GiasContext.CDG_Dettagli
                Join CDG_Testata In GiasContext.CDG_Testata
                     On CDG_Testata.Piva Equals CDG_Dettagli.Piva _
                     And CDG_Testata.Id_CDG Equals CDG_Dettagli.Id_CDG
                Where
                  (CDG_Dettagli.Piva_Superuser.Equals(Piva_SuperUser)) _
                  AndAlso (CDG_Dettagli.Piva.Equals(piva)) _
                  AndAlso (CDG_Testata.Id_Agenda = id_agenda_cdg AndAlso id_agenda_cdg <> 0) _
                  AndAlso (CDG_Testata.Budget = 0) _
                  AndAlso (CDG_Dettagli.Id_Reg <> 0)
                Select New With {
                      .Id_CDG = CDG_Dettagli.Id_CDG,
                      .Id_CDG_Dettagli = CDG_Dettagli.Id_CDG_Dettagli,
                      .piva = CDG_Dettagli.Piva,
                      .sa_cod = CDG_Dettagli.Sa_Cod,
                      .appezza = CDG_Dettagli.Appezza,
                      .id_reg = CDG_Dettagli.Id_Reg,
                       .id_destinazione = CDG_Dettagli.Id_Reg,
                      .Progetto_Cod = CDG_Dettagli.Progetto_Cod,
                      .Valore = CDG_Dettagli.Valore,
                      .ValoreMassimo = CDG_Dettagli.Valore
                    }

        If Dettagli.Count > 0 Then
            Dim ut2 As New Gias_EF_Utility
            DTDettagli = ut2.ObjectQueryToDataTable(Dettagli)
        End If


        Dim w_id_cdg = 0

        Dim myListDettagli = Dettagli.ToList()
        For Each obj In myListDettagli
            If w_id_cdg = 0 Then
                w_id_cdg = obj.Id_CDG
            End If
        Next


        Dim myListPrimiDettagli = myListDettagli.Where(Function(x) x.Id_CDG = w_id_cdg)

        Dim contaDistinteAttive As Integer = 0
        Dim DistinteDT As DataTable = Nothing

        ' Cerca il nr di distinte attive ... solo se non devo leggere il dettaglio per distinta (entrata sulla pagina dove poi le distinte
        '  del periodo non corrente le vado a cercare quando si espande il sottolivello) 
        If Not leggiDettDistinta Then
            Dim Distinte =
        From CI In GiasContext.Mov_Destinazioni
        Join Agenda In GiasContext.Agenda
            On Agenda.PIVA Equals CI.Piva And
            Agenda.Id_Agenda Equals CI.Id_Agenda
        Join Movimenti In GiasContext.Movimenti
            On Movimenti.PIVA Equals CI.Piva And
            Movimenti.Id_Agenda Equals CI.Id_Agenda And
            Movimenti.Id_Mov Equals CI.Id_Mov
        Join Reg_Impianti In GiasContext.Reg_Impianti
            On Reg_Impianti.PIVA Equals CI.Piva And
                Reg_Impianti.SA_COD Equals CI.Sa_Cod And
                Reg_Impianti.APPEZZA Equals CI.Appezza And
                Reg_Impianti.ID_REG Equals CI.Id_Destinazione
        Join Imprese_Progetti In GiasContext.Imprese_Progetti
            On Reg_Impianti.PIVA Equals Imprese_Progetti.Piva And
                Reg_Impianti.SA_COD Equals Imprese_Progetti.Sa_Cod And
                Reg_Impianti.APPEZZA Equals Imprese_Progetti.Appezza And
                Reg_Impianti.ID_REG Equals Imprese_Progetti.Id_Reg
        Group Join Reg_Impianti_Distinta In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Distinta_Chiusa))
            On Reg_Impianti_Distinta.PIVA Equals Imprese_Progetti.Piva And
            Reg_Impianti_Distinta.sa_cod Equals Imprese_Progetti.Sa_Cod And
            Reg_Impianti_Distinta.appezza Equals Imprese_Progetti.Appezza And
            Reg_Impianti_Distinta.Id_Reg Equals Imprese_Progetti.Id_Reg And
            Reg_Impianti_Distinta.Progetto_Cod Equals Imprese_Progetti.Progetto_Cod
            Into Reg_Impianti_Distinta_Group = Group
        From _Reg_Impianti_Distinta_Group In Reg_Impianti_Distinta_Group.DefaultIfEmpty()
        Where
        (CI.Piva.Equals(piva)) _
        AndAlso (Agenda.Raccoglitore_Cod = Raccoglitore_Cod) _
        AndAlso (CI.Tipo_Destinazione = 0) _
        AndAlso strCau_Progetto.Contains(Imprese_Progetti.Cau_Progetto) _
        AndAlso (Imprese_Progetti.Validita_Fine >= data_movimento)
        Select New With {
        .Piva = CI.Piva,
        .Sa_Cod = CI.Sa_Cod,
        .Appezza = CI.Appezza,
        .Id_Destinazione = CI.Id_Destinazione,
        .Progetto_Cod = Imprese_Progetti.Progetto_Cod,
        .Validita_Inizio_Distinta = Imprese_Progetti.Validita_Inizio,
        .Validita_Fine_Distinta = Imprese_Progetti.Validita_Fine,
        .Flag_Distinta_Chiusa = If(_Reg_Impianti_Distinta_Group Is Nothing OrElse _Reg_Impianti_Distinta_Group.val_cod = 0, False, True)
        }

            DistinteDT = gefutils.ObjectQueryToDataTable(Distinte.Distinct().Where(Function(x) Not x.Flag_Distinta_Chiusa).ToList())

        End If

        Dim TestataElem =
        From CI In GiasContext.Mov_Destinazioni
        Join Agenda In GiasContext.Agenda
            On Agenda.PIVA Equals CI.Piva And
            Agenda.Id_Agenda Equals CI.Id_Agenda
        Join Movimenti In GiasContext.Movimenti
            On Movimenti.PIVA Equals CI.Piva And
            Movimenti.Id_Agenda Equals CI.Id_Agenda And
            Movimenti.Id_Mov Equals CI.Id_Mov
        Join Reg_Impianti In GiasContext.Reg_Impianti
            On Reg_Impianti.PIVA Equals CI.Piva And
                Reg_Impianti.SA_COD Equals CI.Sa_Cod And
                Reg_Impianti.APPEZZA Equals CI.Appezza And
                Reg_Impianti.ID_REG Equals CI.Id_Destinazione
        Join Appezzamento In GiasContext.Appezzamento
            On Appezzamento.PIVA Equals Reg_Impianti.PIVA And
                Appezzamento.SA_COD Equals Reg_Impianti.SA_COD And
                Appezzamento.APPEZZA Equals Reg_Impianti.APPEZZA
        Group Join Campi In GiasContext.Campi
            On Appezzamento.PIVA Equals Campi.Piva And
                Appezzamento.SA_COD Equals Campi.Sa_Cod And
                Appezzamento.Campo_Cod Equals Campi.Campo_Cod Into Campi_Group = Group
        From _Campi_Group In Campi_Group.DefaultIfEmpty()
        Join Centri_Aziendali In GiasContext.Centri_Aziendali
            On Centri_Aziendali.PIVA Equals Appezzamento.PIVA And
                Centri_Aziendali.sa_cod Equals Appezzamento.SA_COD
        Join Imprese_Progetti In GiasContext.Imprese_Progetti
            On Reg_Impianti.PIVA Equals Imprese_Progetti.Piva And
                Reg_Impianti.SA_COD Equals Imprese_Progetti.Sa_Cod And
                Reg_Impianti.APPEZZA Equals Imprese_Progetti.Appezza And
                Reg_Impianti.ID_REG Equals Imprese_Progetti.Id_Reg
        Group Join Cultivar In GiasContext.Cultivar
            On Cultivar.Cul_Cod Equals Reg_Impianti.CUL_COD Into Cultivar_Group = Group
        From _Cultivar_Group In Cultivar_Group.DefaultIfEmpty()
        Group Join Specie In GiasContext.SpecieVegetali
            On Specie.Veg_Cod Equals _Cultivar_Group.Veg_Cod Into Specie_Group = Group
        From _Specie_Group In Specie_Group.DefaultIfEmpty()
        Group Join Reg_Impianti_Distinta In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Distinta_Chiusa))
            On Reg_Impianti_Distinta.PIVA Equals Imprese_Progetti.Piva And
            Reg_Impianti_Distinta.sa_cod Equals Imprese_Progetti.Sa_Cod And
            Reg_Impianti_Distinta.appezza Equals Imprese_Progetti.Appezza And
            Reg_Impianti_Distinta.Id_Reg Equals Imprese_Progetti.Id_Reg And
            Reg_Impianti_Distinta.Progetto_Cod Equals Imprese_Progetti.Progetto_Cod
            Into Reg_Impianti_Distinta_Group = Group
        From _Reg_Impianti_Distinta_Group In Reg_Impianti_Distinta_Group.DefaultIfEmpty()
        Group Join Reg_Impianti_CodiceImp In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Codice_Impianto))
            On Reg_Impianti_CodiceImp.PIVA Equals Reg_Impianti.PIVA And
            Reg_Impianti_CodiceImp.sa_cod Equals Reg_Impianti.SA_COD And
            Reg_Impianti_CodiceImp.appezza Equals Reg_Impianti.APPEZZA And
            Reg_Impianti_CodiceImp.Id_Reg Equals Reg_Impianti.ID_REG
            Into Reg_Impianti_CodiceImp_Group = Group
        From _Reg_Impianti_CodiceImp_Group In Reg_Impianti_CodiceImp_Group.DefaultIfEmpty()
        Where
        (CI.Piva.Equals(piva)) _
        AndAlso (Agenda.Raccoglitore_Cod = Raccoglitore_Cod) _
        AndAlso (CI.Tipo_Destinazione = 0) _
        AndAlso strCau_Progetto.Contains(Imprese_Progetti.Cau_Progetto) _
        AndAlso (leggiDettDistinta OrElse Imprese_Progetti.Validita_Inizio <= data_movimento) _
        AndAlso (leggiDettDistinta OrElse Imprese_Progetti.Validita_Fine >= data_movimento)
        Order By CI.QuotaDistribuzione Descending,
            CI.Piva,
            CI.Sa_Cod,
            CI.Appezza,
            CI.Id_Destinazione
        Select New With {
        .Piva = CI.Piva,
        .Sa_Cod = Centri_Aziendali.sa_cod,
        .Sa_Nome = Centri_Aziendali.sa_nome,
        .Campo_Cod = If(_Campi_Group Is Nothing, 0, _Campi_Group.Campo_Cod),
        .Appezza = CI.Appezza,
        .Id_Destinazione = CI.Id_Destinazione,
        .Key = "",
        .Key2 = CI.Piva & CI.Sa_Cod & CI.Appezza & CI.Id_Destinazione,
        .Veg_Cod = If(_Specie_Group Is Nothing, 0, _Specie_Group.Veg_Cod),
        .Veg_Des = If(_Specie_Group Is Nothing, "", _Specie_Group.Veg_Des),
        .Cul_Cod = If(_Cultivar_Group Is Nothing, 0, _Cultivar_Group.Cul_Cod),
        .Cul_Des = If(_Cultivar_Group Is Nothing, Str_TerrenoNudo, _Cultivar_Group.Cul_Des),
        .Descrizione = Appezzamento.APP_NOME,
        .Progetto_Cod = Imprese_Progetti.Progetto_Cod,
        .Progetto_Nome = Imprese_Progetti.Progetto_Nome,
        .Progetto_Des = Imprese_Progetti.Progetto_Des,
        .Validita_Inizio_Distinta = Imprese_Progetti.Validita_Inizio,
        .Validita_Fine_Distinta = Imprese_Progetti.Validita_Fine,
        .Flag_Distinta_Chiusa = If(_Reg_Impianti_Distinta_Group Is Nothing OrElse _Reg_Impianti_Distinta_Group.val_cod = 0, False, True),
        .Codice_Impianto = If(_Reg_Impianti_CodiceImp_Group Is Nothing, "", _Reg_Impianti_CodiceImp_Group.val_cod),
        .Superficie = CI.Qta2,
        .Valore = If(CI.QuotaDistribuzione Is Nothing, 0, CI.QuotaDistribuzione * 100),
        .ValoreMassimo = If(CI.QuotaDistribuzione Is Nothing, 0, CI.QuotaDistribuzione * 100)
        }

        Dim myList = TestataElem.Distinct().ToList()

        '===============================================================================================================================================================================
        'Ripartizione (Quota_Distribuzione non è corretto poiché in una situazione multicentro il totale 100 è riferito ad ogni operazione di agenda e gli impianti sono differenti
        '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        If Not IsNothing(DtRipartizione) Then

            'Scrittura
            If DtRipartizione.Rows.Count > 0 Then

                Dim dr_search As DataRow()
                For Each obj In myList

                    dr_search = DtRipartizione.Select("Piva = '" & obj.Piva & "' And Sa_Cod = " & obj.Sa_Cod & " And Appezza = " & obj.Appezza & " And Id_Destinazione = " & obj.Id_Destinazione)

                    If dr_search.Length <> 0 Then

                        obj.Valore = dr_search(0).Item("Qta") * 100 / TotaleRipartizione
                        obj.ValoreMassimo = obj.Valore

                    End If

                Next

            End If

        ElseIf Not IsNothing(DTDettagli) Then

            'Lettura
            Dim dr_search As DataRow()
            For Each obj In myList

                dr_search = DTDettagli.Select("Piva = '" & obj.Piva & "' And Sa_Cod = " & obj.Sa_Cod & " And Appezza = " & obj.Appezza & " And Id_Destinazione = " & obj.Id_Destinazione)

                If dr_search.Length <> 0 Then

                    obj.Valore = CDec(dr_search(0).Item("valore"))
                    obj.ValoreMassimo = obj.Valore

                End If

            Next

        End If
        '===============================================================================================================================================================================

        'Nuovo ordinamento in base al settaggio del nuovo valore
        myList = myList.OrderByDescending(Function(x) x.Valore).ThenByDescending(Function(x) x.Key2).ToList()



        Dim objValoreMax As Decimal = 0

        Dim w_Piva = ""
        Dim w_SaCod = 0
        Dim w_Appezza = 0
        Dim w_IdReg = 0
        Dim htResidui As New Hashtable()
        For Each obj In myList

            If leggiDettDistinta Then
                If w_Piva <> "" Then
                    'Al cambio di impianto memorizzo i residui a parità di chiave
                    If w_Piva <> obj.Piva OrElse w_SaCod <> obj.Sa_Cod OrElse w_Appezza <> obj.Appezza OrElse w_IdReg <> obj.Id_Destinazione Then
                        If residuoQuestoImpianto <> 0 Then
                            htResidui.Add(w_Piva & "|" & CStr(w_SaCod) & "|" & CStr(w_Appezza) & "|" & CStr(w_IdReg), residuoQuestoImpianto)
                        End If
                        residuoQuestoImpianto = Decimal.Round(Convert.ToDecimal(obj.ValoreMassimo), 2)
                        w_Piva = obj.Piva
                        w_SaCod = obj.Sa_Cod
                        w_Appezza = obj.Appezza
                        w_IdReg = obj.Id_Destinazione


                    Else


                    End If
                Else
                    'Primo Giro
                    residuoQuestoImpianto = Decimal.Round(Convert.ToDecimal(obj.ValoreMassimo), 2)
                    w_Piva = obj.Piva
                    w_SaCod = obj.Sa_Cod
                    w_Appezza = obj.Appezza
                    w_IdReg = obj.Id_Destinazione
                End If
            End If

            ' Cerco quante distinte sono attive dalla data movimento in poi testando sia la data che il flag distinta chiusa
            ' Solo se non l'ho già letto prima
            If leggiDettDistinta Then
                contaDistinteAttive = myList.Where(Function(x) x.Piva = obj.Piva AndAlso x.Sa_Cod = obj.Sa_Cod AndAlso x.Appezza = obj.Appezza AndAlso x.Id_Destinazione = obj.Id_Destinazione AndAlso Not x.Flag_Distinta_Chiusa AndAlso x.Validita_Fine_Distinta >= data_movimento).Count
            Else

                If DistinteDT IsNot Nothing Then
                    contaDistinteAttive = (From elenco_distinte In DistinteDT
                                            Where elenco_distinte("Piva") = obj.Piva AndAlso
                                                  elenco_distinte("Sa_Cod") = obj.Sa_Cod AndAlso
                                                  elenco_distinte("Appezza") = obj.Appezza AndAlso
                                                  elenco_distinte("Id_Destinazione") = obj.Id_Destinazione
                                            Select elenco_distinte).Count()
                End If

            End If

            obj.Key = obj.Piva & "-" & obj.Sa_Cod.ToString() & "-" & obj.Campo_Cod.ToString() & "-" & obj.Appezza.ToString() & "-" & obj.Id_Destinazione.ToString() & "-" & obj.Progetto_Cod.ToString()

            Counter += 1

            Dim myListPrimiDettagli_UnRecord = myListPrimiDettagli.Where(Function(x) x.piva = obj.Piva And x.sa_cod = obj.Sa_Cod And x.appezza = obj.Appezza And x.id_reg = obj.Id_Destinazione And x.Progetto_Cod = obj.Progetto_Cod).FirstOrDefault

            If myListPrimiDettagli_UnRecord IsNot Nothing Then
                'Sono nel caso in cui sono in modifica ed era già presente una % su un impianto o una distinta
                obj.ValoreMassimo = (From s In myListPrimiDettagli.Where(Function(x) x.piva = obj.Piva And x.sa_cod = obj.Sa_Cod And x.appezza = obj.Appezza And x.id_reg = obj.Id_Destinazione) Select s.Valore).Sum()
                obj.Valore = Decimal.Round(Convert.ToDecimal(myListPrimiDettagli_UnRecord.Valore), 2)
                If obj.Flag_Distinta_Chiusa Then
                    ' Necessario perchè se rientro in modifica e la distinta corrente è chiusa non la conterebbe fra quelle
                    ' su cui suddividere e il risultato finale sarebbe più alto di 100
                    contaDistinteAttive += 1

                Else

                    '05/07/2021: Correzione Bug
                    residuoQuestoImpianto = residuoQuestoImpianto - Decimal.Round(Convert.ToDecimal(obj.Valore), 2)

                End If
            Else
                'Sono nel caso di nuove righe
                obj.ValoreMassimo = Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                'obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                If Not attPoliennale Then
                    ' Se non è poliennale assegno la % solo se al periodo corrente se la relativa distinta è aperta
                    If obj.Validita_Inizio_Distinta <= data_movimento AndAlso
                                    obj.Validita_Fine_Distinta >= data_movimento AndAlso
                                    Not obj.Flag_Distinta_Chiusa Then
                        obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                        residuoQuestoImpianto = residuoQuestoImpianto - Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                    Else
                        obj.Valore = 0
                    End If
                Else
                    ' Se è poliennale assegno la quota parte di % solo se al periodo corrente e a quelli futuri la cui distinta è aperta
                    If contaDistinteAttive > 0 AndAlso Not obj.Flag_Distinta_Chiusa AndAlso obj.Validita_Fine_Distinta >= data_movimento Then
                        obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore) / contaDistinteAttive, 2)
                        residuoQuestoImpianto = residuoQuestoImpianto - Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                    Else
                        obj.Valore = 0
                    End If
                End If

            End If

            'Residuo = Residuo - obj.ValoreMassimo
            Residuo = Residuo - Decimal.Round(Convert.ToDecimal(obj.Valore), 2)

            If obj.ValoreMassimo > objValoreMax Then
                objValoreMax = obj.ValoreMassimo
            End If

        Next


        If leggiDettDistinta Then

            'Ultimo giro
            If w_Piva <> "" AndAlso residuoQuestoImpianto <> 0 Then
                htResidui.Add(w_Piva & "|" & CStr(w_SaCod) & "|" & CStr(w_Appezza) & "|" & CStr(w_IdReg), residuoQuestoImpianto)
            End If

            ' SISTEMAZIONE RESIDUI
            'PRIMA PARTE: Sistemo i residui sullo specifico impianto con più distinte e sottraggo dallo sfrido generale

            For Each k In htResidui.Keys
                For Each obj In myList
                    If CStr(k) = obj.Piva & "|" & CStr(obj.Sa_Cod) & "|" & CStr(obj.Appezza) & "|" & CStr(obj.Id_Destinazione) Then
                        obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore) + htResidui(k), 2)
                        Residuo = Residuo - htResidui(k)
                        Exit For
                    End If
                Next
            Next
        End If

        'SECONDA PARTE: Metto l'eventuale sfrido finale sulla riga con valore maggiore
        If leggiDettDistinta AndAlso objValoreMax > 0 AndAlso Residuo > 0 Then
            w_Piva = ""
            w_SaCod = 0
            w_Appezza = 0
            w_IdReg = 0
            If objValoreMax > 0 AndAlso Residuo <> 0 Then
                For Each obj In myList
                    If obj.ValoreMassimo = objValoreMax Then
                        '''If leggiDettDistinta OrElse (Not leggiDettDistinta AndAlso contaDistinteAttive = 1) THEN
                        obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore) + Residuo, 2)
                        w_Piva = obj.Piva
                        w_SaCod = obj.Sa_Cod
                        w_Appezza = obj.Appezza
                        w_IdReg = obj.Id_Destinazione
                        Exit For
                    End If
                Next
                'Devo sistemare anche il valore massimo in tutte le righe con stessa chiave
                If w_Piva <> "" Then
                    For Each obj In myList
                        If w_Piva = obj.Piva AndAlso
                                    w_SaCod = obj.Sa_Cod AndAlso
                                    w_Appezza = obj.Appezza AndAlso
                                    w_IdReg = obj.Id_Destinazione Then
                            obj.ValoreMassimo = Decimal.Round(Convert.ToDecimal(obj.ValoreMassimo) + Residuo, 2)
                        End If
                    Next
                End If
            End If
        End If

        ' Sistemazione sfrido decimali quando non leggo il dettaglio delle distinte
        If Not leggiDettDistinta Then
            Dim max As Decimal = 100
            Dim righe = myList.Count
            Dim c = 0
            For Each obj In myList
                c += 1
                If c = righe Then
                    'Se ValoreMassimo e Valore non sono uguali significa che siamo nel caso di poliannuale
                    If obj.ValoreMassimo = obj.Valore Then
                        obj.ValoreMassimo = max
                        obj.Valore = max
                    Else
                        obj.ValoreMassimo = max
                    End If
                Else
                    max -= Convert.ToDecimal(obj.ValoreMassimo)
                End If

            Next

        End If



        Select Case bDT

            Case True

                Dim ut As New Gias_EF_Utility
                DT = ut.ObjectQueryToDataTable(myList)

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return DT

            Case False

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return risposta

        End Select

        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

        Return Nothing

    End Function


    Public Function Trova_Impianti(ByVal piva As String,
                                   ByVal filtro_centro As String,
                                   ByVal filtro_specie As String,
                                   ByVal filtro_terrenonudo As String,
                                   ByVal filtro_varieta As String,
                                   ByVal id_agenda_cdg As Long,
                                   ByVal data_movimento As Date,
                                   ByVal split As Integer,
                                   ByVal filtro_codice_Appezzamento As String,
                                   ByVal filtro_codice_impianto As String,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal Id_Budget As Integer = 0
                                   ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Trova_Impianti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim dtGrouped As New DataTable

        Dim TabellaPrefisso As String = ""


        Try
            strSql.Length = 0

            If Id_Budget <> 0 Then
                TabellaPrefisso = "Budget_"
            End If

            'strSql.AppendLine(" SELECT Piva, Sa_Cod, Sa_Nome, Appezza, Id_Destinazione, [Key], Veg_Cod, Veg_Des, Cul_Cod, Cul_Des, Descrizione, Progetto_Cod,  Progetto_Nome, ")
            'strSql.AppendLine(" Progetto_Des, Validita_Inizio_Distinta, Validita_Fine_Distinta, Superficie, max(Valore) AS Valore, Campo_Cod, Campo_Des, Flag_Distinta_Chiusa, Codice_Impianto from ( ")


            'Unione con Impianti Presenti in Dettagli
            If id_agenda_cdg <> 0 Then

                'INIZIO introdotto il 24/7/2019 perchè in caso di presenza della % solo negli esercizi futuri questi non venivano più mostrati quando si entrava in modifica di costi non collegati a QdC se si cercava di aggiungere altri impianti
                strSql.AppendLine(" WITH RIGHE_ESISTENTI AS ")
                'FINE introdotto il 24/7/2019 perchè in caso di presenza della % solo negli esercizi futuri questi non venivano più mostrati quando si entrava in modifica di costi non collegati a QdC se si cercava di aggiungere altri impianti

                strSql.AppendLine(" ( ")
                strSql.AppendLine("     Select Distinct " & TabellaPrefisso & "Reg_Impianti.Piva, Centri_Aziendali.Sa_Cod, Centri_Aziendali.Sa_Nome,  " & TabellaPrefisso & "Reg_Impianti.Appezza, " & TabellaPrefisso & "Reg_Impianti.Id_Reg AS Id_Destinazione, ")
                strSql.AppendLine("     CAST(" & TabellaPrefisso & "Reg_Impianti.Piva AS varchar(25))   + '-' + CAST(ISNULL(Centri_Aziendali.Sa_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Campi.Campo_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Appezza, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Id_Reg, 0) AS varchar(50)) AS [Key],  ")
                strSql.AppendLine("     ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des, " & TabellaPrefisso & "Reg_Impianti.Cul_Cod, ISNULL(Cultivar.Cul_Des, 'Terreno Nudo') AS Cul_Des, " & TabellaPrefisso & "Appezzamento.App_Nome AS Descrizione, " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod, ")
                strSql.AppendLine("     " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta, ")
                strSql.AppendLine("     " & TabellaPrefisso & "Imprese_Progetti.Progetto_Nome AS Progetto_Nome, " & TabellaPrefisso & "Imprese_Progetti.Progetto_Des AS Progetto_Des, " & TabellaPrefisso & "Reg_Impianti.Sup_Imp AS Superficie, CDG_Dettagli.Valore AS Valore, ISNULL(" & TabellaPrefisso & "Campi.Campo_Cod, 0) AS Campo_Cod, ISNULL(" & TabellaPrefisso & "Campi.Campo_Des, '') AS Campo_Des, ")
                strSql.AppendLine("     CASE WHEN isnull(" & TabellaPrefisso & "Reg_Impianti_distinta.val_cod, 0) = 0 THEN 0 ELSE 1 END AS Flag_Distinta_Chiusa, ISNULL(" & TabellaPrefisso & "Reg_Impianti_CodiceImp.val_cod, 0) AS Codice_Impianto ")

                'INIZIO introdotto il 24/7/2019 perchè in caso di presenza della % solo negli esercizi futuri questi non venivano più mostrati quando si entrava in modifica di costi non collegati a QdC se si cercava di aggiungere altri impianti
                strSql.AppendLine("     ,ROW_NUMBER()  ")
                strSql.AppendLine(" OVER ( PARTITION BY  ")
                strSql.AppendLine("     " & TabellaPrefisso & "Reg_Impianti.PIVA, Centri_Aziendali.Sa_Cod, Centri_Aziendali.Sa_Nome,  " & TabellaPrefisso & "Reg_Impianti.Appezza, " & TabellaPrefisso & "Reg_Impianti.Id_Reg, ")
                strSql.AppendLine("     CAST(" & TabellaPrefisso & "Reg_Impianti.PIVA AS varchar(25))   + '-' + CAST(ISNULL(Centri_Aziendali.Sa_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Campi.Campo_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Appezza, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Id_Reg, 0) AS varchar(50)), ")
                strSql.AppendLine("     ISNULL(SpecieVegetali.Veg_Cod, 0), ISNULL(SpecieVegetali.Veg_Des, ''), " & TabellaPrefisso & "Reg_Impianti.Cul_Cod, ISNULL(Cultivar.Cul_Des, 'Terreno Nudo'), " & TabellaPrefisso & "Appezzamento.App_Nome , ")
                strSql.AppendLine("     " & TabellaPrefisso & "Reg_Impianti.Sup_Imp, ISNULL(" & TabellaPrefisso & "Campi.Campo_Cod, 0), ISNULL(" & TabellaPrefisso & "Campi.Campo_Des, ''), ")
                strSql.AppendLine("     ISNULL(" & TabellaPrefisso & "Reg_Impianti_CodiceImp.val_cod, 0) ")
                strSql.AppendLine(" ORDER by ")
                strSql.AppendLine("     " & TabellaPrefisso & "Reg_Impianti.PIVA, Centri_Aziendali.sa_cod, ISNULL(" & TabellaPrefisso & "Campi.Campo_Cod, 0), ISNULL(" & TabellaPrefisso & "Reg_Impianti.APPEZZA, 0), ISNULL(" & TabellaPrefisso & "Reg_Impianti.ID_REG, 0) , ")
                strSql.AppendLine("     " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio, " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine ")
                strSql.AppendLine(" ) AS rk ")
                'FINE introdotto il 24/7/2019 perchè in caso di presenza della % solo negli esercizi futuri questi non venivano più mostrati quando si entrava in modifica di costi non collegati a QdC se si cercava di aggiungere altri impianti

                strSql.AppendLine(" From " & TabellaPrefisso & "Reg_Impianti  ")

                strSql.AppendLine(" Join " & TabellaPrefisso & "Imprese_Progetti On  ")
                strSql.AppendLine("  " & TabellaPrefisso & "Reg_Impianti.Piva = " & TabellaPrefisso & "Imprese_Progetti.Piva ")
                strSql.AppendLine(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod ")
                strSql.AppendLine(" And " & TabellaPrefisso & "Reg_Impianti.Appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza ")
                strSql.AppendLine(" And " & TabellaPrefisso & "Reg_Impianti.Id_Reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg ")

                strSql.AppendLine(" Join CDG_Dettagli On ")
                strSql.AppendLine("  CDG_Dettagli.Sa_Cod = " & TabellaPrefisso & "Reg_Impianti.Sa_Cod ")
                strSql.AppendLine(" And CDG_Dettagli.Appezza = " & TabellaPrefisso & "Reg_Impianti.Appezza ")
                strSql.AppendLine(" And CDG_Dettagli.Id_Reg = " & TabellaPrefisso & "Reg_Impianti.Id_Reg ")
                strSql.AppendLine(" And CDG_Dettagli.Progetto_Cod  = " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod ")

                strSql.AppendLine(" Join CDG_Testata On ")
                strSql.AppendLine("  CDG_Testata.Piva = CDG_Dettagli.Piva ")
                strSql.AppendLine(" And CDG_Testata.Id_CDG = CDG_Dettagli.Id_CDG ")

                strSql.AppendLine(" Join Centri_Aziendali On ")
                strSql.AppendLine("  " & TabellaPrefisso & "Reg_Impianti.Piva = Centri_Aziendali.Piva ")
                strSql.AppendLine(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = Centri_Aziendali.Sa_Cod ")

                strSql.AppendLine(" Join " & TabellaPrefisso & "Appezzamento On  ")
                strSql.AppendLine("  " & TabellaPrefisso & "Reg_Impianti.Piva = " & TabellaPrefisso & "Appezzamento.Piva ")
                strSql.AppendLine(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = " & TabellaPrefisso & "Appezzamento.Sa_Cod ")
                strSql.AppendLine(" And " & TabellaPrefisso & "Reg_Impianti.Appezza = " & TabellaPrefisso & "Appezzamento.Appezza ")

                strSql.AppendLine(" Left Outer Join " & TabellaPrefisso & "Campi On")
                strSql.AppendLine(" " & TabellaPrefisso & "Campi.piva = " & TabellaPrefisso & "Appezzamento.Piva And ")
                strSql.AppendLine(" " & TabellaPrefisso & "Campi.sa_cod = " & TabellaPrefisso & "Appezzamento.Sa_Cod And ")
                strSql.AppendLine(" " & TabellaPrefisso & "Campi.Campo_Cod = " & TabellaPrefisso & "Appezzamento.Campo_Cod ")

                strSql.AppendLine(" Left Join Cultivar On ")
                strSql.AppendLine("  Cultivar.Cul_Cod = " & TabellaPrefisso & "Reg_Impianti.Cul_Cod ")

                strSql.AppendLine(" Left Join   SpecieVegetali On ")
                strSql.AppendLine("  Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")

                strSql.AppendLine(" Left Join  " & TabellaPrefisso & "Reg_Impianti_codici AS " & TabellaPrefisso & "Reg_Impianti_CodiceImp on ")
                strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.piva = " & TabellaPrefisso & "Imprese_Progetti.Piva And ")
                strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And ")
                strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza And ")
                strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.id_reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg And ")
                strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.id_cod = " & CInt(enum_CodiciAnagrafe.Codice_Impianto))

                strSql.AppendLine(" Left Join  " & TabellaPrefisso & "Reg_Impianti_codici AS " & TabellaPrefisso & "Reg_Impianti_distinta on ")
                strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_distinta.piva = " & TabellaPrefisso & "Imprese_Progetti.Piva And ")
                strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_distinta.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And ")
                strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_distinta.appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza And ")
                strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_distinta.id_reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg And ")
                strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_distinta.progetto_cod = " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod And ")
                strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_distinta.id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa))

                strSql.AppendLine(" Where CDG_Testata.Budget = 0 And " & TabellaPrefisso & "Reg_Impianti.Piva = '" & Agro_SQL_SaveText(piva) & "'")
                strSql.AppendLine(" And CDG_Testata.Id_Agenda =  " & id_agenda_cdg & " ")

                If Id_Budget <> 0 Then
                    strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
                End If

                'INIZIO asteriscato il 24/7/2019 perchè in caso di presenza della % solo negli esercizi futuri questi non venivano più mostrati quando si entrava in modifica di costi non collegati a QdC se si cercava di aggiungere altri impianti
                'strSql.AppendLine(" And " & Tabellaprefisso & "Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & " ")
                'strSql.AppendLine(" And " & Tabellaprefisso & "Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ")
                'FINE asteriscato il 24/7/2019 

                strSql.AppendLine(" )")

                'INIZIO introdotto il 24/7/2019 perchè in caso di presenza della % solo negli esercizi futuri questi non venivano più mostrati quando si entrava in modifica di costi non collegati a QdC se si cercava di aggiungere altri impianti
                strSql.AppendLine(" Select RIGHE_ESISTENTI.* From RIGHE_ESISTENTI ")
                strSql.AppendLine(" Where RIGHE_ESISTENTI.rk = 1 ")
                'FINE introdotto il 24/7/2019 perchè in caso di presenza della % solo negli esercizi futuri questi non venivano più mostrati quando si entrava in modifica di costi non collegati a QdC se si cercava di aggiungere altri impianti

                strSql.AppendLine(" Union All ")

            End If

            strSql.AppendLine("(SELECT Distinct " & TabellaPrefisso & "Reg_Impianti.Piva, Centri_Aziendali.Sa_Cod, Centri_Aziendali.Sa_Nome, " & TabellaPrefisso & "Reg_Impianti.Appezza, " & TabellaPrefisso & "Reg_Impianti.Id_Reg AS Id_Destinazione, ")
            strSql.AppendLine(" CAST(" & TabellaPrefisso & "Reg_Impianti.Piva AS varchar(25))   + '-' + CAST(ISNULL(Centri_Aziendali.Sa_Cod, 0) AS varchar(50))  + '-' + CAST(ISNULL(" & TabellaPrefisso & "Campi.Campo_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Appezza, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Id_Reg, 0) AS varchar(50)) AS [Key],  ")

            strSql.AppendLine(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ISNULL(SpecieVegetali.Veg_Des, ISNULL(Codici_Anagrafe.descrizione, '')) AS Veg_Des, " & TabellaPrefisso & "Reg_Impianti.Cul_Cod, ISNULL(Cultivar.Cul_Des, 'Terreno Nudo') AS Cul_Des, " & TabellaPrefisso & "Appezzamento.App_Nome AS Descrizione, " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod, ")
            strSql.AppendLine(" " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta, ")
            strSql.AppendLine(" " & TabellaPrefisso & "Imprese_Progetti.Progetto_Nome AS Progetto_Nome, " & TabellaPrefisso & "Imprese_Progetti.Progetto_Des AS Progetto_Des, " & TabellaPrefisso & "Reg_Impianti.Sup_Imp AS Superficie, 0 AS Valore, ISNULL(" & TabellaPrefisso & "Campi.Campo_Cod, 0) AS Campo_Cod, ISNULL(" & TabellaPrefisso & "Campi.Campo_Des, '') AS Campo_Des, ")
            strSql.AppendLine(" 0 AS Flag_Distinta_Chiusa, ISNULL(" & TabellaPrefisso & "Reg_Impianti_CodiceImp.val_cod, 0) AS Codice_Impianto ")
            strSql.AppendLine("  ,1 AS rk  ")

            strSql.AppendLine(" From " & TabellaPrefisso & "Reg_Impianti  ")

            strSql.AppendLine(" Join Centri_Aziendali On")
            strSql.AppendLine("  " & TabellaPrefisso & "Reg_Impianti.Piva = Centri_Aziendali.Piva ")
            strSql.AppendLine(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = Centri_Aziendali.Sa_Cod ")

            strSql.AppendLine(" Join " & TabellaPrefisso & "Appezzamento On")
            strSql.AppendLine("  " & TabellaPrefisso & "Reg_Impianti.Piva = " & TabellaPrefisso & "Appezzamento.Piva ")
            strSql.AppendLine(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = " & TabellaPrefisso & "Appezzamento.Sa_Cod ")
            strSql.AppendLine(" And " & TabellaPrefisso & "Reg_Impianti.Appezza = " & TabellaPrefisso & "Appezzamento.Appezza ")

            strSql.AppendLine(" Left Outer Join " & TabellaPrefisso & "Campi On ")
            strSql.AppendLine(" " & TabellaPrefisso & "Campi.piva = " & TabellaPrefisso & "Appezzamento.Piva And ")
            strSql.AppendLine(" " & TabellaPrefisso & "Campi.sa_cod = " & TabellaPrefisso & "Appezzamento.Sa_Cod And ")
            strSql.AppendLine(" " & TabellaPrefisso & "Campi.Campo_Cod = " & TabellaPrefisso & "Appezzamento.Campo_Cod ")

            strSql.AppendLine(" Join " & TabellaPrefisso & "Imprese_Progetti On")
            strSql.AppendLine("  " & TabellaPrefisso & "Reg_Impianti.Piva = " & TabellaPrefisso & "Imprese_Progetti.Piva ")
            strSql.AppendLine(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod ")
            strSql.AppendLine(" And " & TabellaPrefisso & "Reg_Impianti.Appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza ")
            strSql.AppendLine(" And " & TabellaPrefisso & "Reg_Impianti.Id_Reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg ")

            strSql.AppendLine(" Left Join Cultivar On")
            strSql.AppendLine("  Cultivar.Cul_Cod = " & TabellaPrefisso & "Reg_Impianti.Cul_Cod ")

            strSql.AppendLine(" Left Join SpecieVegetali On")
            strSql.AppendLine("  Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")

            strSql.AppendLine("     LEFT OUTER JOIN  " & TabellaPrefisso & "Reg_Impianti_Codici AS " & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo on " & TabellaPrefisso & "Reg_Impianti.PIVA=" & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.PIVA and " & TabellaPrefisso & "Reg_Impianti.SA_COD=" & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.SA_COD and " & TabellaPrefisso & "Reg_Impianti.APPEZZA=" & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.APPEZZA and " & TabellaPrefisso & "Reg_Impianti.id_reg=" & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.id_reg and " & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.id_cod >2999 and " & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.id_cod < 4000 ")

            strSql.AppendLine("     LEFT OUTER JOIN  Codici_Anagrafe  ")
            strSql.AppendLine("              on " & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.id_cod = Codici_Anagrafe.codice ")

            strSql.AppendLine(" Left Join  " & TabellaPrefisso & "Reg_Impianti_codici AS " & TabellaPrefisso & "Reg_Impianti_CodiceImp on ")
            strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.piva = " & TabellaPrefisso & "Imprese_Progetti.Piva And ")
            strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And ")
            strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza And ")
            strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.id_reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg And ")
            strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.id_cod = " & CInt(enum_CodiciAnagrafe.Codice_Impianto))

            strSql.AppendLine(" Left Join  " & TabellaPrefisso & "Reg_Impianti_codici AS " & TabellaPrefisso & "Reg_Impianti_distinta on ")
            strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_distinta.piva = " & TabellaPrefisso & "Imprese_Progetti.Piva And ")
            strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_distinta.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And ")
            strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_distinta.appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza And ")
            strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_distinta.id_reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg And ")
            strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_distinta.progetto_cod = " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod And ")
            strSql.AppendLine(" " & TabellaPrefisso & "Reg_Impianti_distinta.id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa))

            strSql.AppendLine(" Where " & TabellaPrefisso & "Reg_Impianti.Piva = '" & Agro_SQL_SaveText(piva) & "'")

            If data_movimento <> AGRODATAINIZIO Then
                strSql.AppendLine(" And " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & " ")
                strSql.AppendLine(" And " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ")
            End If

            'Esludo Esercizi Chiusi
            strSql.AppendLine(" And isnull(" & TabellaPrefisso & "Reg_Impianti_distinta.val_cod, '0') != '1' ")


            'Esclusione Esercizi con Raccolte nel Periodo di Competenza
            If split = -1 Then
                strSql.AppendLine(" And Not exists (Select 1 From Agenda, movimenti, mov_destinazioni ")
                strSql.AppendLine("                          Where Lav_Cod = 125 And Cau_Mov = '2200' ")
                strSql.AppendLine("                          And   Agenda.id_agenda = Movimenti.id_agenda ")
                strSql.AppendLine("                          And   Movimenti.id_Mov = mov_destinazioni.id_Mov ")
                strSql.AppendLine("                          And mov_destinazioni.tipo_destinazione = 0 ")
                strSql.AppendLine("                          And mov_destinazioni.piva = " & TabellaPrefisso & "Imprese_Progetti.piva ")
                strSql.AppendLine("                          And mov_destinazioni.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.sa_cod ")
                strSql.AppendLine("                          And mov_destinazioni.appezza = " & TabellaPrefisso & "Imprese_Progetti.appezza ")
                strSql.AppendLine("                          And mov_destinazioni.id_destinazione = " & TabellaPrefisso & "Imprese_Progetti.id_reg ")
                strSql.AppendLine("                          And ( movimenti.Data_Movimento >= " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio ")
                strSql.AppendLine("                          And movimenti.Data_Movimento <= " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine) )")
            End If


            If Id_Budget <> 0 Then
                strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If filtro_centro <> "0" Then
                strSql.AppendLine(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_centro) & ") ")
            End If

            If filtro_specie <> "0" AndAlso filtro_terrenonudo = "0" Then
                strSql.AppendLine(" And Cultivar.Veg_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_specie) & ") ")
            End If

            If filtro_specie = "0" AndAlso filtro_terrenonudo <> "0" Then
                strSql.AppendLine(" And Codici_Anagrafe.codice In (" & Agro_SQL_Save_Clausola_IN(filtro_terrenonudo) & ") ")
            End If

            If filtro_specie <> "0" AndAlso filtro_terrenonudo <> "0" Then
                strSql.AppendLine(" And ( Cultivar.Veg_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_specie) & ")  Or Codici_Anagrafe.codice In (" & Agro_SQL_Save_Clausola_IN(filtro_terrenonudo) & ") ) ")
            End If

            If filtro_varieta <> "0" Then
                strSql.AppendLine(" And Cultivar.Cul_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_varieta) & ") ")
            End If

            If filtro_codice_Appezzamento <> "0" Then
                'Costruzione stringa con apici
                strSql.AppendLine(" And Upper(" & TabellaPrefisso & "Appezzamento.App_Nome) In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(filtro_codice_Appezzamento, ",", "','") & "'", True) & ") ")
            End If

            If filtro_codice_impianto <> "0" Then
                'Costruzione stringa con apici
                strSql.AppendLine(" And Upper(" & TabellaPrefisso & "Reg_Impianti_CodiceImp.val_cod) In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(filtro_codice_impianto, ",", "','") & "'", True) & ") ")
            End If

            strSql.AppendLine(" )")

            'strSql.AppendLine(" ) T ")
            'strSql.AppendLine(" group by Piva, Sa_Cod, Sa_Nome, Campo_Cod, Campo_Des, Appezza, Id_Destinazione, [Key], Veg_Cod, Veg_Des, Cul_Cod, Cul_Des, Descrizione, Progetto_Cod,  Progetto_Nome , Progetto_Des, Validita_Inizio_Distinta, Validita_Fine_Distinta, Flag_Distinta_Chiusa, Codice_Impianto, Superficie")

            'strSql.AppendLine(" ORDER BY Valore Desc, Veg_Des, Cul_Des")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


            Dim dv As DataView = dt.DefaultView
            dv.Sort = "valore desc, veg_des , cul_des"
            dt = dv.ToTable()

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim listGrouped = (
                    From row In dt.AsEnumerable()
                    Group row By datiGroup = New With {
                                Key .piva = row("piva"),
                                Key .sa_cod = row("sa_cod"),
                                Key .sa_nome = row("sa_nome"),
                                Key .appezza = row("appezza"),
                                Key .Id_destinazione = row("Id_destinazione"),
                                Key .key = row("key"),
                                Key .veg_cod = row("veg_cod"),
                                Key .veg_des = row("veg_des"),
                                Key .cul_cod = row("cul_cod"),
                                Key .cul_des = row("cul_des"),
                                Key .descrizione = row("descrizione"),
                                Key .progetto_Cod = row("progetto_Cod"),
                                Key .validita_inizio_distinta = row("validita_inizio_distinta"),
                                Key .validita_fine_distinta = row("validita_fine_distinta"),
                                Key .progetto_nome = row("progetto_nome"),
                                Key .progetto_des = row("progetto_des"),
                                Key .superficie = row("superficie"),
                                Key .campo_cod = row("campo_cod"),
                                Key .campo_des = row("campo_des"),
                                Key .flag_distinta_chiusa = row("flag_distinta_chiusa"),
                                Key .codice_impianto = row("codice_impianto")
                                               } Into Group
                    Select New With {
                                datiGroup.piva,
                                datiGroup.sa_cod,
                                datiGroup.sa_nome,
                                datiGroup.appezza,
                                datiGroup.Id_destinazione,
                                datiGroup.key,
                                datiGroup.veg_cod,
                                datiGroup.veg_des,
                                datiGroup.cul_cod,
                                datiGroup.cul_des,
                                datiGroup.descrizione,
                                datiGroup.progetto_Cod,
                                datiGroup.validita_inizio_distinta,
                                datiGroup.validita_fine_distinta,
                                datiGroup.progetto_nome,
                                datiGroup.progetto_des,
                                datiGroup.superficie,
                                datiGroup.campo_cod,
                                datiGroup.campo_des,
                                datiGroup.flag_distinta_chiusa,
                                datiGroup.codice_impianto,
                                .valore = Group.Max(Function(x) x("valore"))
                                }
                                ).ToList

                dtGrouped.Columns.Add(New DataColumn("Piva", GetType(String)))
                dtGrouped.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
                dtGrouped.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
                dtGrouped.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
                dtGrouped.Columns.Add(New DataColumn("Id_Destinazione", GetType(Integer)))
                dtGrouped.Columns.Add(New DataColumn("Key", GetType(String)))
                dtGrouped.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
                dtGrouped.Columns.Add(New DataColumn("Veg_Des", GetType(String)))
                dtGrouped.Columns.Add(New DataColumn("Cul_Cod", GetType(Integer)))
                dtGrouped.Columns.Add(New DataColumn("Cul_Des", GetType(String)))
                dtGrouped.Columns.Add(New DataColumn("Descrizione", GetType(String)))
                dtGrouped.Columns.Add(New DataColumn("Progetto_Cod", GetType(Integer)))
                dtGrouped.Columns.Add(New DataColumn("Validita_Inizio_Distinta", GetType(Date)))
                dtGrouped.Columns.Add(New DataColumn("Validita_Fine_Distinta", GetType(Date)))
                dtGrouped.Columns.Add(New DataColumn("Progetto_Nome", GetType(String)))
                dtGrouped.Columns.Add(New DataColumn("Progetto_Des", GetType(String)))
                dtGrouped.Columns.Add(New DataColumn("Superficie", GetType(Decimal)))
                dtGrouped.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
                dtGrouped.Columns.Add(New DataColumn("Campo_Des", GetType(String)))
                dtGrouped.Columns.Add(New DataColumn("Flag_Distinta_Chiusa", GetType(Boolean)))
                dtGrouped.Columns.Add(New DataColumn("Codice_Impianto", GetType(String)))
                dtGrouped.Columns.Add(New DataColumn("Valore", GetType(Decimal)))

                Dim d0 As DataRow
                For Each r In listGrouped

                    ' Cerco se ci sono righe con stessa chiave e valore = 0 
                    ' Può succedere se c'è stato uno split dove le % sono solo sulle righe di esercizi futuri rispetto alla data_inserimento di CDG_Testata
                    Dim nrTrovati = 0
                    If CDec(r.valore) = 0 Then
                        nrTrovati = (From x In dtGrouped.AsEnumerable()
                                     Where r.key = x("key") AndAlso
                                             r.Id_destinazione = x("Id_Destinazione")
                                     Select x).ToList.Count
                    End If
                    If nrTrovati = 0 Then
                        d0 = dtGrouped.NewRow
                        d0("Piva") = r.piva
                        d0("Sa_Cod") = r.sa_cod
                        d0("Sa_Nome") = r.sa_nome
                        d0("Appezza") = r.appezza
                        d0("Id_Destinazione") = r.Id_destinazione
                        d0("Key") = r.key
                        d0("Veg_Cod") = r.veg_cod
                        d0("Veg_Des") = r.veg_des
                        d0("Cul_Cod") = r.cul_cod
                        d0("Cul_Des") = r.cul_des
                        d0("Descrizione") = r.descrizione
                        d0("Progetto_Cod") = r.progetto_Cod
                        d0("Validita_Inizio_Distinta") = r.validita_inizio_distinta
                        d0("Validita_Fine_Distinta") = r.validita_fine_distinta
                        d0("Progetto_Nome") = r.progetto_nome
                        d0("Progetto_Des") = r.progetto_des
                        d0("Superficie") = r.superficie
                        d0("Campo_Cod") = r.campo_cod
                        d0("Campo_Des") = r.campo_des
                        d0("Flag_Distinta_Chiusa") = r.flag_distinta_chiusa
                        d0("Codice_Impianto") = r.codice_impianto
                        d0("Valore") = r.valore
                        dtGrouped.Rows.Add(d0)
                    End If
                Next


            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dtGrouped = Nothing
            Throw New Exception("" & nomeRoutine & " :                         " & messaggioErrore)
        End Try


        Return dtGrouped


    End Function



    Public Function Trova_Distinte(ByVal piva As String,
                                   ByVal filtro_centro As String,
                                   ByVal filtro_specie As String,
                                   ByVal filtro_terrenonudo As String,
                                   ByVal filtro_varieta As String,
                                   ByVal data_movimento As Date,
                                   ByVal split As Integer,
                                   ByVal filtro_codice_appezzamento As String,
                                   ByVal filtro_codice_impianto As String,
                                   ByVal filtro_codice_distinta As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Trova_Distinte()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0

            strSql.AppendLine(" SELECT Piva, Sa_Cod, Sa_Nome, Appezza, Id_Destinazione, [Key], Veg_Cod, Veg_Des, Cul_Cod, Cul_Des, Descrizione, Progetto_Cod,  Progetto_Nome, ")
            strSql.AppendLine(" Progetto_Des, Validita_Inizio_Distinta, Validita_Fine_Distinta, Superficie, max(Valore) AS Valore, Campo_Cod, Campo_Des, Flag_Distinta_Chiusa, Codice_Impianto from ( ")

            strSql.AppendLine("(SELECT Distinct Reg_Impianti.Piva, Centri_Aziendali.Sa_Cod, Centri_Aziendali.Sa_Nome, Reg_Impianti.Appezza, Reg_Impianti.Id_Reg AS Id_Destinazione, ")
            strSql.AppendLine(" CAST(reg_Impianti.Piva AS varchar(25))   + '-' + CAST(ISNULL(Centri_Aziendali.Sa_Cod, 0) AS varchar(50))  + '-' + CAST(ISNULL(Campi.Campo_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(Reg_Impianti.Appezza, 0) AS varchar(50)) + '-' + CAST(ISNULL(Reg_Impianti.Id_Reg, 0) AS varchar(50)) + '-' + CAST(ISNULL(Imprese_Progetti.Progetto_Cod, 0) AS varchar(50)) AS [Key],  ")
            strSql.AppendLine(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ISNULL(SpecieVegetali.Veg_Des, ISNULL(Codici_Anagrafe.descrizione, '')) AS Veg_Des, Reg_impianti.Cul_Cod, ISNULL(Cultivar.Cul_Des, 'Terreno Nudo') AS Cul_Des, Appezzamento.App_Nome AS Descrizione, Imprese_Progetti.Progetto_Cod, ")
            strSql.AppendLine(" Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta, ")
            strSql.AppendLine(" Imprese_Progetti.Progetto_Nome AS Progetto_Nome, Imprese_Progetti.Progetto_Des AS Progetto_Des, Reg_impianti.Sup_Imp AS Superficie, 0 AS Valore, ISNULL(Campi.Campo_Cod, 0) AS Campo_Cod, ISNULL(Campi.Campo_Des, '') AS Campo_Des, ")
            strSql.AppendLine(" 0 AS Flag_Distinta_Chiusa, ISNULL(Reg_Impianti_CodiceImp.val_cod, 0) AS Codice_Impianto ")
            strSql.AppendLine(" From Reg_Impianti  ")

            strSql.AppendLine(" Join Centri_Aziendali On")
            strSql.AppendLine("  Reg_impianti.Piva = Centri_Aziendali.Piva ")
            strSql.AppendLine(" And Reg_impianti.Sa_Cod = Centri_Aziendali.Sa_Cod ")

            strSql.AppendLine(" Join Appezzamento On")
            strSql.AppendLine("  Reg_impianti.Piva = Appezzamento.Piva ")
            strSql.AppendLine(" And Reg_impianti.Sa_Cod = Appezzamento.Sa_Cod ")
            strSql.AppendLine(" And Reg_impianti.Appezza = Appezzamento.Appezza ")

            strSql.AppendLine(" Left Outer Join Campi On ")
            strSql.AppendLine(" Campi.piva = Appezzamento.Piva And ")
            strSql.AppendLine(" Campi.sa_cod = Appezzamento.Sa_Cod And ")
            strSql.AppendLine(" Campi.Campo_Cod = Appezzamento.Campo_Cod ")

            strSql.AppendLine(" Join Imprese_Progetti On")
            strSql.AppendLine("  Reg_impianti.Piva = Imprese_Progetti.Piva ")
            strSql.AppendLine(" And Reg_impianti.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            strSql.AppendLine(" And Reg_impianti.Appezza = Imprese_Progetti.Appezza ")
            strSql.AppendLine(" And Reg_impianti.Id_Reg = Imprese_Progetti.Id_Reg ")

            strSql.AppendLine(" Left Join Cultivar On")
            strSql.AppendLine("  Cultivar.Cul_Cod = Reg_impianti.Cul_Cod ")

            strSql.AppendLine(" Left Join SpecieVegetali On")
            strSql.AppendLine("  Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")

            strSql.AppendLine("     LEFT OUTER JOIN  Reg_Impianti_Codici AS Reg_Impianti_Codici_Terr_Nudo on Reg_Impianti.PIVA=Reg_Impianti_Codici_Terr_Nudo.PIVA and Reg_Impianti.SA_COD=Reg_Impianti_Codici_Terr_Nudo.SA_COD and Reg_Impianti.APPEZZA=Reg_Impianti_Codici_Terr_Nudo.APPEZZA and Reg_Impianti.id_reg=Reg_Impianti_Codici_Terr_Nudo.id_reg and Reg_Impianti_Codici_Terr_Nudo.id_cod >2999 and Reg_Impianti_Codici_Terr_Nudo.id_cod < 4000 ")

            strSql.AppendLine("     LEFT OUTER JOIN  Codici_Anagrafe  ")
            strSql.AppendLine("              on Reg_Impianti_Codici_Terr_Nudo.id_cod = Codici_Anagrafe.codice ")

            strSql.AppendLine(" Left Join  reg_impianti_codici AS Reg_Impianti_CodiceImp on ")
            strSql.AppendLine(" Reg_Impianti_CodiceImp.piva = Imprese_Progetti.Piva And ")
            strSql.AppendLine(" Reg_Impianti_CodiceImp.sa_cod = Imprese_Progetti.Sa_Cod And ")
            strSql.AppendLine(" Reg_Impianti_CodiceImp.appezza = Imprese_Progetti.Appezza And ")
            strSql.AppendLine(" Reg_Impianti_CodiceImp.id_reg = Imprese_Progetti.Id_Reg And ")
            strSql.AppendLine(" Reg_Impianti_CodiceImp.id_cod = " & CInt(enum_CodiciAnagrafe.Codice_Impianto))

            strSql.AppendLine(" Left Join  reg_impianti_codici AS reg_impianti_distinta on ")
            strSql.AppendLine(" reg_impianti_distinta.piva = Imprese_Progetti.Piva And ")
            strSql.AppendLine(" reg_impianti_distinta.sa_cod = Imprese_Progetti.Sa_Cod And ")
            strSql.AppendLine(" reg_impianti_distinta.appezza = Imprese_Progetti.Appezza And ")
            strSql.AppendLine(" reg_impianti_distinta.id_reg = Imprese_Progetti.Id_Reg And ")
            strSql.AppendLine(" reg_impianti_distinta.progetto_cod = Imprese_Progetti.Progetto_Cod And ")
            strSql.AppendLine(" reg_impianti_distinta.id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa))

            strSql.AppendLine(" Where Reg_impianti.Piva = '" & Agro_SQL_SaveText(piva) & "'")

            If data_movimento <> AGRODATAINIZIO Then
                strSql.AppendLine(" And Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & " ")
                strSql.AppendLine(" And Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ")
            End If

            'Esclusione Esercizi Chiusi
            strSql.AppendLine(" And isnull(reg_impianti_distinta.val_cod, '0') != '1' ")


            'Esclusione Esercizi con Raccolte nel Periodo di Competenza
            strSql.AppendLine(" And Not exists (Select 1 From Agenda, movimenti, mov_destinazioni ")
            strSql.AppendLine("                          Where Lav_Cod = 125 And Cau_Mov = '2200' ")
            strSql.AppendLine("                          And Agenda.id_agenda = Movimenti.id_agenda ")
            strSql.AppendLine("                          And Movimenti.id_Mov = mov_destinazioni.id_Mov ")
            strSql.AppendLine("                          And mov_destinazioni.tipo_destinazione = 0 ")
            strSql.AppendLine("                          And mov_destinazioni.piva = imprese_progetti.piva ")
            strSql.AppendLine("                          And mov_destinazioni.sa_cod = imprese_progetti.sa_cod ")
            strSql.AppendLine("                          And mov_destinazioni.appezza = imprese_progetti.appezza ")
            strSql.AppendLine("                          And mov_destinazioni.id_destinazione = imprese_progetti.id_reg ")
            strSql.AppendLine("                          And ( movimenti.Data_Movimento >= Imprese_progetti.Validita_Inizio ")
            strSql.AppendLine("                          And movimenti.Data_Movimento <= Imprese_progetti.Validita_Fine) )")

            If filtro_centro <> "0" Then
                strSql.AppendLine(" And Reg_impianti.Sa_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_centro) & ") ")
            End If

            If filtro_specie <> "0" AndAlso filtro_terrenonudo = "0" Then
                strSql.AppendLine(" And Cultivar.Veg_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_specie) & ") ")
            End If

            If filtro_specie = "0" AndAlso filtro_terrenonudo <> "0" Then
                strSql.AppendLine(" And Codici_Anagrafe.codice In (" & Agro_SQL_Save_Clausola_IN(filtro_terrenonudo) & ") ")
            End If

            If filtro_specie <> "0" AndAlso filtro_terrenonudo <> "0" Then
                strSql.AppendLine(" And ( Cultivar.Veg_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_specie) & ")  Or Codici_Anagrafe.codice In (" & Agro_SQL_Save_Clausola_IN(filtro_terrenonudo) & ") ) ")
            End If

            If filtro_varieta <> "0" Then
                strSql.AppendLine(" And Cultivar.Cul_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_varieta) & ") ")
            End If

            If filtro_codice_appezzamento <> "0" Then
                'Costruzione stringa con apici
                strSql.AppendLine(" And Upper(Appezzamento.App_Nome) In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(filtro_codice_appezzamento, ",", "','") & "'", True) & ") ")
            End If

            If filtro_codice_impianto <> "0" Then
                'Costruzione stringa con apici
                strSql.AppendLine(" And Upper(Reg_Impianti_CodiceImp.val_cod) In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(filtro_codice_impianto, ",", "','") & "'", True) & ") ")
            End If

            If filtro_codice_distinta <> "0" Then
                'Costruzione stringa con apici
                strSql.AppendLine(" And Upper(Imprese_Progetti.Progetto_Nome) In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(filtro_codice_distinta, ",", "','") & "'", True) & ") ")
            End If

            strSql.AppendLine(" )")



            strSql.AppendLine(" ) T ")
            strSql.AppendLine(" group by Piva, Sa_Cod, Sa_Nome, Campo_Cod, Campo_Des, Appezza, Id_Destinazione, [Key], Veg_Cod, Veg_Des, Cul_Cod, Cul_Des, Descrizione, Progetto_Cod,  Progetto_Nome , Progetto_Des, Validita_Inizio_Distinta, Validita_Fine_Distinta, Flag_Distinta_Chiusa, Codice_Impianto, Superficie")

            strSql.AppendLine(" ORDER BY Valore Desc, Veg_Des, Cul_Des")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try


        Return dt


    End Function



    Public Function Trova_Codici_Appezzamenti(ByVal filtro_aziende As String,
                                              ByVal filtro_centro As String,
                                              ByVal filtro_specie As String,
                                              ByVal filtro_terrenonudo As String,
                                              ByVal filtro_varieta As String,
                                              ByVal data_movimento As Date,
                                              ByRef objParametri As AgronicaCoreParametri,
                                              Optional ByVal Id_Budget As Integer = 0,
                                              Optional ByVal bIncludiFiltroVisibilitaImprese As Boolean = False
                                              ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Trova_Codici_Appezzamenti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim TabellaPrefisso As String = ""


        Try
            strSql.Length = 0

            If Id_Budget <> 0 Then
                TabellaPrefisso = "Budget_"
            End If

            strSql.Append(" SELECT distinct " & TabellaPrefisso & "Appezzamento.App_Nome AS Descrizione, " & TabellaPrefisso & "Appezzamento.App_Nome AS Codice ")
            'strSql.Append(" SELECT " & Tabellaprefisso & "Appezzamento.App_Nome AS Descrizione, (Cast(" & Tabellaprefisso & "Appezzamento.Sa_Cod  AS varchar(50)) + '-' + Cast(" & Tabellaprefisso & "Appezzamento.Appezza  AS varchar(50))) AS Codice ")
            strSql.Append(" From " & TabellaPrefisso & "Reg_impianti  ")

            strSql.Append(" Join Centri_Aziendali On")
            strSql.Append("  " & TabellaPrefisso & "Reg_impianti.Piva = Centri_Aziendali.Piva ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Sa_Cod = Centri_Aziendali.Sa_Cod ")

            strSql.Append(" Join " & TabellaPrefisso & "Appezzamento On")
            strSql.Append("  " & TabellaPrefisso & "Reg_impianti.Piva = " & TabellaPrefisso & "Appezzamento.Piva ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Sa_Cod = " & TabellaPrefisso & "Appezzamento.Sa_Cod ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Appezza = " & TabellaPrefisso & "Appezzamento.Appezza ")

            strSql.Append(" Left Outer Join " & TabellaPrefisso & "Campi On ")
            strSql.Append(" " & TabellaPrefisso & "Campi.piva = " & TabellaPrefisso & "Appezzamento.Piva And ")
            strSql.Append(" " & TabellaPrefisso & "Campi.sa_cod = " & TabellaPrefisso & "Appezzamento.Sa_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Campi.Campo_Cod = " & TabellaPrefisso & "Appezzamento.Campo_Cod ")

            strSql.Append(" Join " & TabellaPrefisso & "Imprese_Progetti On")
            strSql.Append("  " & TabellaPrefisso & "Reg_impianti.Piva = " & TabellaPrefisso & "Imprese_Progetti.Piva ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Sa_Cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Id_Reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg ")

            strSql.Append(" Left Join Cultivar On")
            strSql.Append("  Cultivar.Cul_Cod = " & TabellaPrefisso & "Reg_impianti.Cul_Cod ")

            strSql.Append(" Left Join SpecieVegetali On")
            strSql.Append("  Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")

            strSql.Append("     LEFT OUTER JOIN  " & TabellaPrefisso & "Reg_impianti_Codici AS " & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo on " & TabellaPrefisso & "Reg_impianti.PIVA=" & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.PIVA and " & TabellaPrefisso & "Reg_impianti.SA_COD=" & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.SA_COD and " & TabellaPrefisso & "Reg_impianti.APPEZZA=" & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.APPEZZA and " & TabellaPrefisso & "Reg_impianti.id_reg=" & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.id_reg and " & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.id_cod >2999 and " & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.id_cod < 4000 ")

            strSql.Append("     LEFT OUTER JOIN  Codici_Anagrafe  ")
            strSql.Append("              on " & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.id_cod = Codici_Anagrafe.codice ")

            strSql.Append(" Left Join  " & TabellaPrefisso & "Reg_impianti_codici AS " & TabellaPrefisso & "Reg_impianti_CodiceImp on ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_CodiceImp.piva = " & TabellaPrefisso & "Imprese_Progetti.Piva And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_CodiceImp.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_CodiceImp.appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_CodiceImp.id_reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_CodiceImp.id_cod = " & CInt(enum_CodiciAnagrafe.Codice_Impianto))

            strSql.Append(" Left Join  " & TabellaPrefisso & "Reg_impianti_codici AS " & TabellaPrefisso & "Reg_impianti_distinta on ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_distinta.piva = " & TabellaPrefisso & "Imprese_Progetti.Piva And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_distinta.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_distinta.appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_distinta.id_reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_distinta.progetto_cod = " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_distinta.id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa))

            strSql.Append(" Where 1=1 ")
            If filtro_aziende <> "" Then
                strSql.Append(" AND " & TabellaPrefisso & "Reg_impianti.Piva In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(filtro_aziende, "|", "','") & "'", True) & ") ")
            End If

            If data_movimento <> AGRODATAINIZIO Then
                strSql.Append(" And " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & " ")
                strSql.Append(" And " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ")
            End If

            If filtro_centro <> "0" Then
                strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Sa_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_centro) & ") ")
            End If

            If filtro_specie <> "0" AndAlso filtro_terrenonudo = "0" Then
                strSql.Append(" And Cultivar.Veg_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_specie) & ") ")
            End If

            If filtro_specie = "0" AndAlso filtro_terrenonudo <> "0" Then
                strSql.Append(" And Codici_Anagrafe.codice In (" & Agro_SQL_Save_Clausola_IN(filtro_terrenonudo) & ") ")
            End If

            If filtro_specie <> "0" AndAlso filtro_terrenonudo <> "0" Then
                strSql.Append(" And ( Cultivar.Veg_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_specie) & ")  Or Codici_Anagrafe.codice In (" & Agro_SQL_Save_Clausola_IN(filtro_terrenonudo) & ") ) ")
            End If

            If filtro_varieta <> "0" Then
                strSql.Append(" And Cultivar.Cul_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_varieta) & ") ")
            End If

            If Id_Budget <> 0 Then
                strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If bIncludiFiltroVisibilitaImprese Then

                'Filtro Visibilità Imprese
                strSql.AppendLine(" and " & TabellaPrefisso & "Appezzamento.Piva in ( ")

                strSql.AppendLine(" Select Distinct dbo.Imprese.PIVA ")
                strSql.AppendLine(" FROM  (( ")
                strSql.AppendLine(" Imprese INNER JOIN UtentiXImprese On Imprese.Piva = UtentixImprese.Piva) ")
                strSql.AppendLine(" INNER Join ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA)  ")
                strSql.AppendLine(" Where ImpresexIndirizzi.Tipo_Indirizzo = 1 ")
                strSql.AppendLine(" And   UtentixImprese.[User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
                strSql.AppendLine(" And   Imprese.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                strSql.AppendLine(" And   Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")



                '----------------------------------------------------------------
                '--- Filtro associato all'utente 
                '----------------------------------------------------------------
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim UtenteProfiloImpreseSql As String = ""
                Dim UtenteProfiloCentriSql As String = ""
                Dim DtImpreseVisibili As DataTable
                Dim i As Integer

                DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)
                If DtImpreseVisibili IsNot Nothing Then
                    For i = 0 To DtImpreseVisibili.Rows.Count - 1
                        UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                    Next
                    If UtenteProfiloImpreseSql <> "" Then
                        UtenteProfiloImpreseSql = " AND " & TabellaPrefisso & "Appezzamento.piva IN (" & Agro_SQL_Save_Clausola_IN(Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1), True) & ") "
                    End If

                    strSql.AppendLine(UtenteProfiloImpreseSql)

                End If

                '-------------------------------------------------------------------------


            End If


            strSql.Append(" ORDER BY Descrizione Asc")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try


        Return dt


    End Function


    Public Function Trova_Codici_Impianti(ByVal filtro_aziende As String,
                                          ByVal filtro_centro As String,
                                          ByVal filtro_specie As String,
                                          ByVal filtro_terrenonudo As String,
                                          ByVal filtro_varieta As String,
                                          ByVal data_movimento As Date,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByVal Id_Budget As Integer = 0,
                                          Optional ByVal bIncludiFiltroVisibilitaImprese As Boolean = False
                                          ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Trova_Codici_Impianti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim TabellaPrefisso As String = ""


        Try
            strSql.Length = 0

            If Id_Budget <> 0 Then
                TabellaPrefisso = "Budget_"
            End If

            strSql.Append(" SELECT distinct ISNULL(" & TabellaPrefisso & "Reg_impianti_CodiceImp.val_cod, '') + ' - ' + Progetto_Des AS Codice_Impianto, ISNULL(" & TabellaPrefisso & "Reg_impianti_CodiceImp.val_cod, '') AS Codice ")
            'strSql.Append(" SELECT ISNULL(" & Tabellaprefisso & "Reg_impianti_CodiceImp.val_cod, '') AS Codice_Impianto, (Cast(" & Tabellaprefisso & "Reg_impianti.Sa_Cod  AS varchar(50)) + '-' + Cast(" & Tabellaprefisso & "Reg_impianti.Appezza  AS varchar(50)) + '-' + Cast(" & Tabellaprefisso & "Reg_impianti.Id_Reg  AS varchar(50))) AS Codice ")
            strSql.Append(" From " & TabellaPrefisso & "Reg_impianti  ")

            strSql.Append(" Join Centri_Aziendali On")
            strSql.Append("  " & TabellaPrefisso & "Reg_impianti.Piva = Centri_Aziendali.Piva ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Sa_Cod = Centri_Aziendali.Sa_Cod ")

            strSql.Append(" Join " & TabellaPrefisso & "Appezzamento On")
            strSql.Append("  " & TabellaPrefisso & "Reg_impianti.Piva = " & TabellaPrefisso & "Appezzamento.Piva ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Sa_Cod = " & TabellaPrefisso & "Appezzamento.Sa_Cod ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Appezza = " & TabellaPrefisso & "Appezzamento.Appezza ")

            strSql.Append(" Left Outer Join " & TabellaPrefisso & "Campi On ")
            strSql.Append(" " & TabellaPrefisso & "Campi.piva = " & TabellaPrefisso & "Appezzamento.Piva And ")
            strSql.Append(" " & TabellaPrefisso & "Campi.sa_cod = " & TabellaPrefisso & "Appezzamento.Sa_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Campi.Campo_Cod = " & TabellaPrefisso & "Appezzamento.Campo_Cod ")

            strSql.Append(" Join " & TabellaPrefisso & "Imprese_Progetti On")
            strSql.Append("  " & TabellaPrefisso & "Reg_impianti.Piva = " & TabellaPrefisso & "Imprese_Progetti.Piva ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Sa_Cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Id_Reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg ")

            strSql.Append(" Left Join Cultivar On")
            strSql.Append("  Cultivar.Cul_Cod = " & TabellaPrefisso & "Reg_impianti.Cul_Cod ")

            strSql.Append(" Left Join SpecieVegetali On")
            strSql.Append("  Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")

            strSql.Append("     LEFT OUTER JOIN  " & TabellaPrefisso & "Reg_impianti_Codici AS " & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo on " & TabellaPrefisso & "Reg_impianti.PIVA=" & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.PIVA and " & TabellaPrefisso & "Reg_impianti.SA_COD=" & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.SA_COD and " & TabellaPrefisso & "Reg_impianti.APPEZZA=" & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.APPEZZA and " & TabellaPrefisso & "Reg_impianti.id_reg=" & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.id_reg and " & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.id_cod >2999 and " & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.id_cod < 4000 ")

            strSql.Append("     LEFT OUTER JOIN  Codici_Anagrafe  ")
            strSql.Append("              on " & TabellaPrefisso & "Reg_impianti_Codici_Terr_Nudo.id_cod = Codici_Anagrafe.codice ")

            strSql.Append(" Left Join  " & TabellaPrefisso & "Reg_impianti_codici AS " & TabellaPrefisso & "Reg_impianti_CodiceImp on ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_CodiceImp.piva = " & TabellaPrefisso & "Imprese_Progetti.Piva And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_CodiceImp.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_CodiceImp.appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_CodiceImp.id_reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_CodiceImp.id_cod = " & CInt(enum_CodiciAnagrafe.Codice_Impianto))

            strSql.Append(" Left Join  " & TabellaPrefisso & "Reg_impianti_codici AS " & TabellaPrefisso & "Reg_impianti_distinta on ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_distinta.piva = " & TabellaPrefisso & "Imprese_Progetti.Piva And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_distinta.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_distinta.appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_distinta.id_reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_distinta.progetto_cod = " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_impianti_distinta.id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa))


            strSql.Append(" Where " & TabellaPrefisso & "Reg_impianti_CodiceImp.val_cod <> '' ")

            If filtro_aziende <> "" Then
                strSql.Append(" AND " & TabellaPrefisso & "Reg_impianti.Piva In (" & Agro_SQL_Save_Clausola_IN("'" & Replace(filtro_aziende, "|", "','") & "'", True) & ") ")
            End If

            If data_movimento <> AGRODATAINIZIO Then
                strSql.Append(" And " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & " ")
                strSql.Append(" And " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ")
            End If

            'If split <> -1 THEN
            '    strSql.Append(" And isnull(" & Tabellaprefisso & "Reg_impianti_distinta.val_cod, '0') != '1' ")
            'End If


            If filtro_centro <> "0" Then
                strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Sa_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_centro) & ") ")
            End If

            If filtro_specie <> "0" AndAlso filtro_terrenonudo = "0" Then
                strSql.Append(" And Cultivar.Veg_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_specie) & ") ")
            End If

            If filtro_specie = "0" AndAlso filtro_terrenonudo <> "0" Then
                strSql.Append(" And Codici_Anagrafe.codice In (" & Agro_SQL_Save_Clausola_IN(filtro_terrenonudo) & ") ")
            End If

            If filtro_specie <> "0" AndAlso filtro_terrenonudo <> "0" Then
                strSql.Append(" And ( Cultivar.Veg_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_specie) & ")  Or Codici_Anagrafe.codice In (" & Agro_SQL_Save_Clausola_IN(filtro_terrenonudo) & ") ) ")
            End If

            If filtro_varieta <> "0" Then
                strSql.Append(" And Cultivar.Cul_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_varieta) & ") ")
            End If

            If Id_Budget <> 0 Then
                strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If


            If bIncludiFiltroVisibilitaImprese Then

                'Filtro Visibilità Imprese
                strSql.AppendLine(" and " & TabellaPrefisso & "Reg_impianti.Piva in ( ")

                strSql.AppendLine(" Select Distinct dbo.Imprese.PIVA ")
                strSql.AppendLine(" FROM  (( ")
                strSql.AppendLine(" Imprese INNER JOIN UtentiXImprese On Imprese.Piva = UtentixImprese.Piva) ")
                strSql.AppendLine(" INNER Join ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA)  ")
                strSql.AppendLine(" Where ImpresexIndirizzi.Tipo_Indirizzo = 1 ")
                strSql.AppendLine(" And   UtentixImprese.[User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
                strSql.AppendLine(" And   Imprese.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                strSql.AppendLine(" And   Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


                '----------------------------------------------------------------
                '--- Filtro associato all'utente 
                '----------------------------------------------------------------
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim UtenteProfiloImpreseSql As String = ""
                Dim UtenteProfiloCentriSql As String = ""
                Dim DtImpreseVisibili As DataTable
                Dim i As Integer

                DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)
                If DtImpreseVisibili IsNot Nothing Then
                    For i = 0 To DtImpreseVisibili.Rows.Count - 1
                        UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                    Next
                    If UtenteProfiloImpreseSql <> "" Then
                        UtenteProfiloImpreseSql = " AND " & TabellaPrefisso & "Reg_impianti.piva IN (" & Agro_SQL_Save_Clausola_IN(Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1), True) & ") "
                    End If

                    strSql.AppendLine(UtenteProfiloImpreseSql)

                End If

                '-------------------------------------------------------------------------



            End If

            strSql.Append(" ORDER BY Codice_Impianto Asc")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try


        Return dt


    End Function


    Public Function Trova_Codici_Distinte(ByVal piva As String,
                                          ByVal filtro_centro As String,
                                          ByVal filtro_specie As String,
                                          ByVal filtro_terrenonudo As String,
                                          ByVal filtro_varieta As String,
                                          ByVal data_movimento As Date,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByVal Id_Budget As Integer = 0
                                          ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Trova_Codici_Distinte()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim TabellaPrefisso As String = ""


        Try
            strSql.Length = 0

            If Id_Budget <> 0 Then
                TabellaPrefisso = "Budget_"
            End If

            strSql.Append(" SELECT " & TabellaPrefisso & "Imprese_Progetti.Progetto_Nome AS Codice_Distinta, " & TabellaPrefisso & "Imprese_Progetti.Progetto_Nome AS Codice ")
            strSql.Append(" From " & TabellaPrefisso & "Reg_Impianti  ")

            strSql.Append(" Join Centri_Aziendali On")
            strSql.Append("  " & TabellaPrefisso & "Reg_Impianti.Piva = Centri_Aziendali.Piva ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = Centri_Aziendali.Sa_Cod ")

            strSql.Append(" Join " & TabellaPrefisso & "Appezzamento On")
            strSql.Append("  " & TabellaPrefisso & "Reg_Impianti.Piva = " & TabellaPrefisso & "Appezzamento.Piva ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = " & TabellaPrefisso & "Appezzamento.Sa_Cod ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Appezza = " & TabellaPrefisso & "Appezzamento.Appezza ")

            strSql.Append(" Left Outer Join Campi On ")
            strSql.Append(" Campi.piva = " & TabellaPrefisso & "Appezzamento.Piva And ")
            strSql.Append(" Campi.sa_cod = " & TabellaPrefisso & "Appezzamento.Sa_Cod And ")
            strSql.Append(" Campi.Campo_Cod = " & TabellaPrefisso & "Appezzamento.Campo_Cod ")

            strSql.Append(" Join " & TabellaPrefisso & "Imprese_Progetti On")
            strSql.Append("  " & TabellaPrefisso & "Reg_Impianti.Piva = " & TabellaPrefisso & "Imprese_Progetti.Piva ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza ")
            strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Id_Reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg ")

            strSql.Append(" Left Join Cultivar On")
            strSql.Append("  Cultivar.Cul_Cod = " & TabellaPrefisso & "Reg_Impianti.Cul_Cod ")

            strSql.Append(" Left Join SpecieVegetali On")
            strSql.Append("  Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")

            strSql.Append("     LEFT OUTER JOIN  " & TabellaPrefisso & "Reg_Impianti_Codici AS " & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo on " & TabellaPrefisso & "Reg_Impianti.PIVA=" & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.PIVA and " & TabellaPrefisso & "Reg_Impianti.SA_COD=" & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.SA_COD and " & TabellaPrefisso & "Reg_Impianti.APPEZZA=" & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.APPEZZA and " & TabellaPrefisso & "Reg_Impianti.id_reg=" & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.id_reg and " & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.id_cod >2999 and " & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.id_cod < 4000 ")

            strSql.Append("     LEFT OUTER JOIN  Codici_Anagrafe  ")
            strSql.Append("              on " & TabellaPrefisso & "Reg_Impianti_Codici_Terr_Nudo.id_cod = Codici_Anagrafe.codice ")

            strSql.Append(" Inner Join  " & TabellaPrefisso & "Reg_Impianti_codici AS " & TabellaPrefisso & "Reg_Impianti_CodiceImp on ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.piva = " & TabellaPrefisso & "Imprese_Progetti.Piva And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.id_reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_CodiceImp.id_cod = " & CInt(enum_CodiciAnagrafe.Codice_Impianto))

            strSql.Append(" Left Join  " & TabellaPrefisso & "Reg_Impianti_codici AS " & TabellaPrefisso & "Reg_Impianti_distinta on ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_distinta.piva = " & TabellaPrefisso & "Imprese_Progetti.Piva And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_distinta.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_distinta.appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_distinta.id_reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_distinta.progetto_cod = " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_distinta.id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa))

            strSql.Append(" Where " & TabellaPrefisso & "Reg_Impianti.Piva = '" & Agro_SQL_SaveText(piva) & "' And " & TabellaPrefisso & "Reg_Impianti_CodiceImp.val_cod <> ''")

            If data_movimento <> AGRODATAINIZIO Then
                strSql.Append(" And " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & " ")
                strSql.Append(" And " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ")
            End If

            'If split <> -1 THEN
            '    strSql.Append(" And isnull(" & Tabellaprefisso & "Reg_Impianti_distinta.val_cod, '0') != '1' ")
            'End If


            If filtro_centro <> "0" Then
                strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_centro) & ") ")
            End If

            If filtro_specie <> "0" AndAlso filtro_terrenonudo = "0" Then
                strSql.Append(" And Cultivar.Veg_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_specie) & ") ")
            End If

            If filtro_specie = "0" AndAlso filtro_terrenonudo <> "0" Then
                strSql.Append(" And Codici_Anagrafe.codice In (" & Agro_SQL_Save_Clausola_IN(filtro_terrenonudo) & ") ")
            End If

            If filtro_specie <> "0" AndAlso filtro_terrenonudo <> "0" Then
                strSql.Append(" And ( Cultivar.Veg_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_specie) & ")  Or Codici_Anagrafe.codice In (" & Agro_SQL_Save_Clausola_IN(filtro_terrenonudo) & ") ) ")
            End If

            If filtro_varieta <> "0" Then
                strSql.Append(" And Cultivar.Cul_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_varieta) & ") ")
            End If

            If Id_Budget <> 0 Then
                strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If


            strSql.Append(" ORDER BY Codice_Distinta Asc")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try


        Return dt


    End Function

    Public Function Trova_Distinte_CDG_Dettagli_Per_Id_Imputazione(ByVal piva As String,
                                             ByVal Tipo_Imputazione As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Trova_Distinte_CDG_Dettagli_Per_Id_Imputazione()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.Append(" SELECT DISTINCT dbo.CDG_Dettagli.Id_Imputazione,  ")
            strSql.Append(" dbo.Imputazioni.Imputazione_Cod_Des, dbo.Imputazioni.Analisi, ")
            strSql.Append(" dbo.Imputazioni_Classi.Imputazione_Classe_Cod, ")
            strSql.Append(" dbo.Imputazioni_Classi.Imputazione_Classe_Des ")
            strSql.Append(" FROM dbo.CDG_Dettagli INNER JOIN ")
            strSql.Append(" dbo.Imputazioni ON dbo.CDG_Dettagli.Piva_Superuser = dbo.Imputazioni.Piva_SuperUser AND dbo.CDG_Dettagli.Piva = dbo.Imputazioni.Piva AND dbo.CDG_Dettagli.Id_Imputazione = dbo.Imputazioni.Imputazione_Cod ")
            strSql.Append(" INNER JOIN dbo.Imputazioni_Classi ON dbo.Imputazioni_Classi.Piva_SuperUser = dbo.Imputazioni.Piva_SuperUser AND dbo.Imputazioni_Classi.Piva = dbo.Imputazioni.Piva")
            strSql.Append(" where dbo.CDG_Dettagli.Piva = '" & Agro_SQL_SaveText(piva) & "'   ")
            strSql.Append(" And dbo.Imputazioni.Tipo_Imputazione = " & Agro_SQL_SaveNum(Tipo_Imputazione))



            ' NOn parametrizzato in attesa fix json
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function


    Public Function Trova_Distinte_Per_Impianto(ByVal piva As String,
                                   ByVal filtro_impianto As String,
                                   ByVal valoreSpalmabile As Decimal,
                                   ByVal filtro_progetto_cod As Integer,
                                   ByVal id_agenda_cdg As Long,
                                   ByVal data_movimento As Date,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal Modalita As Integer = 0,
                                   Optional ByVal Id_Budget As Integer = 0
                                   ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim chiave As String() = filtro_impianto.Split("-")
        Dim kPiva As String = CStr(chiave(0))
        Dim kSa_Cod As Integer = Integer.Parse(chiave(1))
        Dim kAppezza As Integer = Integer.Parse(chiave(3))
        Dim kId_Reg As Integer = Integer.Parse(chiave(4))

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Trova_Distinte_Per_Impianto()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        valoreSpalmabile = Decimal.Round(valoreSpalmabile, 2)
        Dim TabellaPrefisso As String = ""


        Try
            strSql.Length = 0

            If Id_Budget <> 0 Then
                TabellaPrefisso = "Budget_"
            End If


            Dim righeSuCuiSpalmare As Integer = 0

            If id_agenda_cdg = 0 AndAlso valoreSpalmabile <> 0 Then
                ' Cerco il totale delle distinte che mostrerò per poter poi mostrare il valore già suddiviso
                strSql.Append("SELECT Distinct " & TabellaPrefisso & "Reg_Impianti.Piva, " & TabellaPrefisso & "Reg_Impianti.Sa_Cod,  " & TabellaPrefisso & "Reg_Impianti.Appezza, " & TabellaPrefisso & "Reg_Impianti.Id_Reg AS Id_Destinazione, " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod ")
                strSql.Append(" From ")
                strSql.Append(" " & TabellaPrefisso & "Reg_Impianti ")
                strSql.Append(" Join " & TabellaPrefisso & "Imprese_Progetti  on  ")
                strSql.Append(" " & TabellaPrefisso & "Reg_Impianti.PIVA = " & TabellaPrefisso & "Imprese_Progetti.Piva And " & TabellaPrefisso & "Reg_Impianti.SA_COD = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And ")
                strSql.Append(" " & TabellaPrefisso & "Reg_Impianti.APPEZZA = " & TabellaPrefisso & "Imprese_Progetti.Appezza And " & TabellaPrefisso & "Reg_Impianti.ID_REG = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg ")
                strSql.Append(" Left Join " & TabellaPrefisso & "Reg_Impianti_Codici  on ")
                strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_Codici.PIVA = " & TabellaPrefisso & "Imprese_Progetti.Piva And ")
                strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_Codici.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And " & TabellaPrefisso & "Reg_Impianti_Codici.appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza And ")
                strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_Codici.Id_Reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg And " & TabellaPrefisso & "Reg_Impianti_Codici.Progetto_Cod = " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod And ")
                strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_Codici.id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa))

                strSql.Append(" Where " & TabellaPrefisso & "Reg_Impianti.Piva = '" & Agro_SQL_SaveText(kPiva) & "'")
                strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = " & kSa_Cod)
                strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Appezza = " & kAppezza)
                strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Id_Reg = " & kId_Reg)

                If Id_Budget <> 0 Then
                    strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
                End If

                strSql.Append(" And (" & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio > " & Agro_SQL_SaveDate(data_movimento) & " Or ")
                strSql.Append("  " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine < " & Agro_SQL_SaveDate(data_movimento) & " ) ")
                strSql.Append("  And " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento))
                strSql.Append(" And ( isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.id_cod, 0) != " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa) & " ")
                strSql.Append(" OR ( ")
                strSql.Append(" isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.id_cod, 0) = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa) & " And isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.val_cod, 0) = 0 ) ")
                strSql.Append(" ) ")




                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

                righeSuCuiSpalmare = dt.Rows.Count

            End If

            strSql.Length = 0
            strSql.Append(" SELECT Piva, Sa_Cod, Sa_Nome, Appezza, Id_Destinazione, [Key], KeyDistinta, Veg_Cod, Veg_Des, Cul_Cod, Cul_Des, Descrizione, ")
            strSql.Append(" Progetto_Cod, Validita_Inizio_Distinta, Validita_Fine_Distinta, Progetto_Nome, Progetto_Des, Superficie,   ")

            If id_agenda_cdg = 0 Then
                If righeSuCuiSpalmare <> 0 Then

                    strSql.Append(Agro_SQL_SaveNum(Decimal.Round(CDec(valoreSpalmabile / righeSuCuiSpalmare), 2)) & " AS Valore ")
                Else
                    strSql.Append(" 0 AS Valore ")
                End If

            Else
                strSql.Append(" max(Valore) AS Valore ")
            End If
            strSql.Append("  , Flag_Distinta_Chiusa ")
            strSql.Append(" from ( ")

            strSql.Append("(SELECT Distinct " & TabellaPrefisso & "Reg_Impianti.Piva, Centri_Aziendali.Sa_Cod, Centri_Aziendali.Sa_Nome, " & TabellaPrefisso & "Reg_Impianti.Appezza, " & TabellaPrefisso & "Reg_Impianti.Id_Reg AS Id_Destinazione, ")
            strSql.Append(" CAST(" & TabellaPrefisso & "Reg_Impianti.Piva AS varchar(25))   + '-' + CAST(ISNULL(Centri_Aziendali.Sa_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Campi.Campo_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Appezza, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Id_Reg, 0) AS varchar(50)) AS [Key],  ")
            strSql.Append(" CAST(" & TabellaPrefisso & "Reg_Impianti.Piva AS varchar(25))   + '-' + CAST(ISNULL(Centri_Aziendali.Sa_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Campi.Campo_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Appezza, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Id_Reg, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod, 0) AS varchar(50)) AS KeyDistinta,  ")
            strSql.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, Cultivar.Cul_Cod, Cultivar.Cul_Des, Appezzamento.App_Nome AS Descrizione, " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod, ")
            strSql.Append(" " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta, " & TabellaPrefisso & "Imprese_Progetti.Progetto_Nome AS Progetto_Nome, " & TabellaPrefisso & "Imprese_Progetti.Progetto_Des AS Progetto_Des, " & TabellaPrefisso & "Reg_Impianti.Sup_Imp AS Superficie, 0 AS Valore ")
            If id_agenda_cdg = 0 AndAlso CDate(data_movimento) <> AGRODATAINIZIO Then
                strSql.Append(" , 0 AS Flag_Distinta_Chiusa ")
            Else
                strSql.Append(" , CASE WHEN isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.id_cod, 0) = 0 THEN 0 ELSE CASE WHEN isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.val_cod, 0) = 0 THEN 0 ELSE 1 END END AS Flag_Distinta_Chiusa ")
            End If

            strSql.Append(" From ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti ")
            strSql.Append(" Join Centri_Aziendali On ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti.PIVA = Centri_Aziendali.PIVA And  ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti.SA_COD = Centri_Aziendali.sa_cod ")
            strSql.Append(" Join Appezzamento On ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti.PIVA = Appezzamento.PIVA And " & TabellaPrefisso & "Reg_Impianti.SA_COD = Appezzamento.SA_COD And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti.APPEZZA = Appezzamento.APPEZZA ")

            strSql.Append(" Left Outer Join " & TabellaPrefisso & "Campi On ")
            strSql.Append(" " & TabellaPrefisso & "Campi.piva = Appezzamento.Piva And ")
            strSql.Append(" " & TabellaPrefisso & "Campi.sa_cod = Appezzamento.Sa_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Campi.Campo_Cod = Appezzamento.Campo_Cod ")

            strSql.Append(" Left Outer Join Cultivar On ")
            strSql.Append(" Cultivar.Cul_Cod = " & TabellaPrefisso & "Reg_Impianti.CUL_COD ")
            strSql.Append(" Left Outer Join SpecieVegetali On ")
            strSql.Append(" Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            strSql.Append(" Join " & TabellaPrefisso & "Imprese_Progetti  on  ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti.PIVA = " & TabellaPrefisso & "Imprese_Progetti.Piva And " & TabellaPrefisso & "Reg_Impianti.SA_COD = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti.APPEZZA = " & TabellaPrefisso & "Imprese_Progetti.Appezza And " & TabellaPrefisso & "Reg_Impianti.ID_REG = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg ")

            strSql.Append(" Left Join " & TabellaPrefisso & "Reg_Impianti_Codici  on ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_Codici.PIVA = " & TabellaPrefisso & "Imprese_Progetti.Piva And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_Codici.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And " & TabellaPrefisso & "Reg_Impianti_Codici.appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_Codici.Id_Reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg And " & TabellaPrefisso & "Reg_Impianti_Codici.Progetto_Cod = " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod And ")
            strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_Codici.id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa))

            strSql.Append(" Where " & TabellaPrefisso & "Reg_Impianti.Piva = '" & Agro_SQL_SaveText(kPiva) & "'")
            strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = " & kSa_Cod)
            strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Appezza = " & kAppezza)
            strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Id_Reg = " & kId_Reg)


            Select Case Modalita

                Case 0

                    strSql.Append(" And (" & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio > " & Agro_SQL_SaveDate(data_movimento) & " Or ")
                    strSql.Append("  " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine < " & Agro_SQL_SaveDate(data_movimento) & " ) ")

                    'Se non sono in modifica però scarto quelle passate e quelle non attive
                    'If id_agenda_cdg = 0 THEN
                    strSql.Append("  And " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento))

                    strSql.Append(" And ( isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.id_cod, 0) != " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa) & " ")
                    strSql.Append(" OR ( ")
                    strSql.Append(" isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.id_cod, 0) = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa) & " And isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.val_cod, 0) = 0 ) ")
                    strSql.Append(" ) ")
                    'End If


                Case 1 'Filtro da Split Ricerca della Distinta Unica (vedi Split Annuale --> Distinta Valida nella data)

                    strSql.Append(" And " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & "  ")
                    strSql.Append(" And " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ")

                    strSql.Append(" And ( isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.id_cod, 0) != " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa) & " ")
                    strSql.Append(" OR ( ")
                    strSql.Append(" isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.id_cod, 0) = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa) & " And isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.val_cod, 0) = 0 ) ")
                    strSql.Append(" ) ")


                Case 2 'Filtro da Split per Distinte Successive alla Data (vedi Split Poliannuale)

                    strSql.Append(" And ((" & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & " And " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ) Or " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio >= " & Agro_SQL_SaveDate(data_movimento) & " )")
                    strSql.Append(" And (isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.id_cod, 0)!= " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa) & " ")
                    strSql.Append(" Or ( ")
                    strSql.Append(" isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.id_cod, 0) = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa) & " And isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.val_cod, 0) = 0 ) ")
                    strSql.Append(" ) ")


            End Select

            strSql.Append(" And " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod != " & filtro_progetto_cod)

            strSql.Append(" )")

            'Unione con Impianti Presenti in Dettagli
            If id_agenda_cdg <> 0 Then

                strSql.Append(" Union All ")
                strSql.Append(" (Select Distinct " & TabellaPrefisso & "Reg_Impianti.Piva, Centri_Aziendali.Sa_Cod, Centri_Aziendali.Sa_Nome, " & TabellaPrefisso & "Reg_Impianti.Appezza, " & TabellaPrefisso & "Reg_Impianti.Id_Reg AS Id_Destinazione, ")
                strSql.Append(" CAST(" & TabellaPrefisso & "Reg_Impianti.Piva AS varchar(25))   + '-' + CAST(ISNULL(Centri_Aziendali.Sa_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Campi.Campo_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Appezza, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Id_Reg, 0) AS varchar(50)) AS [Key],  ")
                strSql.Append(" CAST(" & TabellaPrefisso & "Reg_Impianti.Piva AS varchar(25))   + '-' + CAST(ISNULL(Centri_Aziendali.Sa_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Campi.Campo_Cod, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Appezza, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Reg_Impianti.Id_Reg, 0) AS varchar(50)) + '-' + CAST(ISNULL(" & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod, 0) AS varchar(50)) AS KeyDistinta,  ")
                strSql.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, Cultivar.Cul_Cod, Cultivar.Cul_Des, Appezzamento.App_Nome AS Descrizione, " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod, ")
                strSql.Append(" " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta, " & TabellaPrefisso & "Imprese_Progetti.Progetto_Nome AS Progetto_Nome, " & TabellaPrefisso & "Imprese_Progetti.Progetto_Des AS Progetto_Des, " & TabellaPrefisso & "Reg_Impianti.Sup_Imp AS Superficie, CDG_Dettagli.Valore AS Valore ")
                strSql.Append(" , CASE WHEN isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.id_cod, 0) = 0 THEN 0 ELSE CASE WHEN isnull(" & TabellaPrefisso & "Reg_Impianti_Codici.val_cod, 0) = 0 THEN 0 ELSE 1 END END AS Flag_Distinta_Chiusa ")
                strSql.Append(" From CDG_Dettagli")

                strSql.Append(" Join CDG_Testata ")
                strSql.Append(" On CDG_Testata.Piva = CDG_Dettagli.Piva ")
                strSql.Append(" And CDG_Testata.Id_CDG = CDG_Dettagli.Id_CDG ")

                strSql.Append(" Join " & TabellaPrefisso & "Reg_Impianti ")
                strSql.Append(" On CDG_Dettagli.Sa_Cod = " & TabellaPrefisso & "Reg_Impianti.Sa_Cod ")
                strSql.Append(" And CDG_Dettagli.Appezza = " & TabellaPrefisso & "Reg_Impianti.Appezza ")
                strSql.Append(" And CDG_Dettagli.Id_Reg = " & TabellaPrefisso & "Reg_Impianti.Id_Reg ")

                strSql.Append(" Join Centri_Aziendali ")
                strSql.Append(" On " & TabellaPrefisso & "Reg_Impianti.Piva = Centri_Aziendali.Piva ")
                strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = Centri_Aziendali.Sa_Cod ")

                strSql.Append(" Join Appezzamento ")
                strSql.Append(" On " & TabellaPrefisso & "Reg_Impianti.Piva = Appezzamento.Piva ")
                strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = Appezzamento.Sa_Cod ")
                strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Appezza = Appezzamento.Appezza ")

                strSql.Append(" Left Outer Join " & TabellaPrefisso & "Campi On ")
                strSql.Append(" " & TabellaPrefisso & "Campi.piva = Appezzamento.Piva And ")
                strSql.Append(" " & TabellaPrefisso & "Campi.sa_cod = Appezzamento.Sa_Cod And ")
                strSql.Append(" " & TabellaPrefisso & "Campi.Campo_Cod = Appezzamento.Campo_Cod ")

                strSql.Append(" Join " & TabellaPrefisso & "Imprese_Progetti ")
                strSql.Append(" On CDG_Dettagli.Piva = " & TabellaPrefisso & "Imprese_Progetti.Piva ")
                strSql.Append(" And CDG_Dettagli.Sa_Cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod ")
                strSql.Append(" And CDG_Dettagli.Appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza ")
                strSql.Append(" And CDG_Dettagli.Id_Reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg ")
                strSql.Append(" And CDG_Dettagli.Progetto_Cod  = " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod ")

                strSql.Append(" Left Outer Join Cultivar ")
                strSql.Append(" On Cultivar.Cul_Cod = " & TabellaPrefisso & "Reg_Impianti.Cul_Cod ")

                strSql.Append(" Left Outer Join SpecieVegetali ")
                strSql.Append(" On Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")

                strSql.Append(" Left Join " & TabellaPrefisso & "Reg_Impianti_Codici  on ")
                strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_Codici.PIVA = " & TabellaPrefisso & "Imprese_Progetti.Piva And ")
                strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_Codici.sa_cod = " & TabellaPrefisso & "Imprese_Progetti.Sa_Cod And " & TabellaPrefisso & "Reg_Impianti_Codici.appezza = " & TabellaPrefisso & "Imprese_Progetti.Appezza And ")
                strSql.Append(" " & TabellaPrefisso & "Reg_Impianti_Codici.Id_Reg = " & TabellaPrefisso & "Imprese_Progetti.Id_Reg And " & TabellaPrefisso & "Reg_Impianti_Codici.Progetto_Cod = " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod ")
                strSql.Append("  And " & TabellaPrefisso & "Reg_Impianti_Codici.id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa))

                strSql.Append(" Where " & TabellaPrefisso & "Reg_Impianti.Piva = '" & Agro_SQL_SaveText(kPiva) & "'")
                strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = " & kSa_Cod)
                strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Appezza = " & kAppezza)
                strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Id_Reg = " & kId_Reg)
                strSql.Append(" And CDG_Testata.Id_Agenda =  " & id_agenda_cdg & " And CDG_Testata.Budget = 0 ")

                strSql.Append(" And (CDG_Dettagli.Valore > 0 OR " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio > " & Agro_SQL_SaveDate(data_movimento) & " Or ")
                strSql.Append("  " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine < " & Agro_SQL_SaveDate(data_movimento) & " ) ")

                strSql.Append(" And " & TabellaPrefisso & "Imprese_Progetti.Progetto_Cod != " & filtro_progetto_cod)
                strSql.Append(" )")


            End If

            strSql.Append(" ) T ")
            strSql.Append(" group by Piva, Sa_Cod, Sa_Nome, Appezza, Id_Destinazione, [Key], KeyDistinta, Veg_Cod, Veg_Des, Cul_Cod, Cul_Des, Descrizione, Progetto_Cod, Validita_Inizio_Distinta, Validita_Fine_Distinta, Progetto_Nome , Progetto_Des, Superficie, Flag_Distinta_Chiusa")

            strSql.Append(" ORDER BY Validita_Inizio_Distinta, Validita_Fine_Distinta, Valore Desc")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try

        'Occorre controllare che causa decimali non si sia passati oltre al valore spalmabile
        Dim totaleSpalmato As Decimal = 0
        For Each dr In dt.Rows
            totaleSpalmato += dr.Item("valore")
        Next
        If dt.Rows.Count > 0 AndAlso valoreSpalmabile <> 0 AndAlso totaleSpalmato <> valoreSpalmabile Then
            dt.Rows(0).Item("valore") -= totaleSpalmato - valoreSpalmabile
        End If

        Return dt


    End Function


    Public Function Leggi_Campi(ByVal piva As String,
                                ByVal validita_inizio As Date,
                                ByVal validita_fine As Date,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Campi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT Campo_Cod, (Sa_Nome + ' - ' + Campo_Des) AS Campo_Des, Campi.Sa_Cod, Sa_Nome ")
            strSql.Append(" From Centri_Aziendali, Campi")
            strSql.Append(" Where Campi.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And Campi.Piva = Centri_Aziendali.Piva ")
            strSql.Append(" And Campi.Sa_Cod = Centri_Aziendali.Sa_Cod ")
            strSql.Append(" And Campi.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_inizio) & " ")
            strSql.Append(" And Campi.Validita_Fine >= " & Agro_SQL_SaveDate(validita_fine) & " ")

            strSql.Append(" ORDER BY Campo_Des")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function



    Public Function Leggi_Dettagli_Agenda(ByVal piva As String,
                                          ByVal id_agenda As Integer,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Dettagli_Agenda()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" Select Movimenti_Dettagli.* ")
            strSql.Append(" From Agenda, Movimenti, Movimenti_Dettagli, Mov_Destinazioni ")
            strSql.Append(" Where Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And Agenda.Piva = Movimenti.Piva ")
            strSql.Append(" And Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.Append(" And Movimenti.Piva = Movimenti_Dettagli.Piva ")
            strSql.Append(" And Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            strSql.Append(" And Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            strSql.Append(" And Movimenti.Cau_Mov Not in ('7300', '7350') ")
            strSql.Append(" And Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva ")
            strSql.Append(" And Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            strSql.Append(" And Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            strSql.Append(" And Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            strSql.Append(" And Tipo_Destinazione = 0 ")
            strSql.Append(" And Agenda.Id_Agenda = " & id_agenda & " ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)


            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function


    Public Function Leggi_Impianti_Da_Prodotto(ByVal piva As String,
                                               ByVal id_agenda As Integer,
                                               ByVal raccoglitore_cod As Integer,
                                               ByVal ObjData As Object,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Impianti_Da_Prodotto()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" Select Mov_Destinazioni.* ")
            strSql.Append(" From Agenda, Movimenti, Movimenti_Dettagli, Mov_Destinazioni ")
            strSql.Append(" Where Elem_Cod = " & Agro_SQL_SaveNum(ObjData("Elem_Cod")))
            strSql.Append(" And Pro_Cod = " & Agro_SQL_SaveNum(ObjData("Pro_Cod")))
            strSql.Append(" And Mat_Cod = " & Agro_SQL_SaveNum(ObjData("Mat_Cod")))
            strSql.Append(" And Cod_Progetto = " & Agro_SQL_SaveNum(ObjData("Cod_Progetto")))
            strSql.Append(" And Lotto = '" & Agro_SQL_SaveText(ObjData("Lotto")) & "'")
            strSql.Append(" And Udm_Cod = " & Agro_SQL_SaveNum(ObjData("Udm_Cod")))
            strSql.Append(" And Cal_Cod = " & Agro_SQL_SaveNum(ObjData("Cal_Cod")))
            strSql.Append(" And Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And Agenda.Piva = Movimenti.Piva ")
            strSql.Append(" And Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.Append(" And Agenda.Raccoglitore_Cod = " & raccoglitore_cod & " ")
            strSql.Append(" And Movimenti.Piva = Movimenti_Dettagli.Piva ")
            strSql.Append(" And Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            strSql.Append(" And Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            strSql.Append(" And Movimenti.Cau_Mov Not in ('7300', '7350') ")
            strSql.Append(" And Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva ")
            strSql.Append(" And Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            strSql.Append(" And Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            strSql.Append(" And Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            strSql.Append(" And Tipo_Destinazione = 0 ")

            If raccoglitore_cod <> 0 Then
                strSql.Append(" And Agenda.Raccoglitore_Cod = " & raccoglitore_cod & " ")
            Else
                strSql.Append(" And Agenda.Id_Agenda = " & id_agenda & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)


            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function


    Public Function Leggi_Impianti(ByVal piva As String,
                                   ByVal veg_cod As Integer,
                                   ByVal campo_cod As Integer,
                                   ByVal validita_inizio As Date,
                                   ByVal validita_fine As Date,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Impianti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            'strSql.Append(" SELECT Distinct (Sa_Nome + ' - ' + App_Nome + ' - ' + Veg_Des + ' - ' + Cul_Des + ' - ' + Imprese_Progetti.Progetto_Nome) AS Impianto_Des, ")
            strSql.Append(" SELECT Distinct (CASE WHEN Reg_Impianti_Codici.val_cod is null OR Reg_Impianti_Codici.val_cod = '' THEN '*' Else Reg_Impianti_Codici.val_cod End + ' - ' + ISNULL(Imprese_Progetti.Progetto_Nome, '') + ' (' + Veg_Des + ')' + ' - ' + ISNULL(Campi.Campo_Des, '')) AS Impianto_Des, ")
            strSql.Append(" (CONVERT(varchar(10), Imprese_Progetti.Sa_Cod) + '|' + CONVERT(varchar(10), Imprese_Progetti.Appezza) + '|' + CONVERT(varchar(10), Imprese_Progetti.Id_Reg) + '|' + CONVERT(varchar(10), Imprese_Progetti.Progetto_Cod)) AS Impianto_Cod, Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta, ")
            strSql.Append(" Reg_Impianti.Sa_Cod, Reg_Impianti.Appezza, Reg_Impianti.Id_Reg, Imprese_Progetti.Progetto_Cod, ")
            strSql.Append(" ISNULL(Reg_Impianti_Codici.val_cod, '') AS Codice_Impianto, ISNULL(Imprese_Progetti.Progetto_Nome, '') AS Codice_Distinta, ")
            strSql.Append(" Veg_Des, Cul_Des, App_Nome ")

            strSql.Append(" From Centri_Aziendali, Appezzamento, Campi, SpecieVegetali, Cultivar, Imprese_Progetti, Reg_Impianti ")

            strSql.Append(" Left Outer Join Reg_Impianti_Codici On ( ")
            strSql.Append(" Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA ")
            strSql.Append(" And Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod ")
            strSql.Append(" And Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza ")
            strSql.Append(" And Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg ")
            strSql.Append(" And Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Codice_Impianto & ") " & vbCrLf)

            strSql.Append(" Where Reg_Impianti.Piva = Centri_Aziendali.Piva ")
            strSql.Append(" And   Reg_Impianti.Sa_Cod = Centri_Aziendali.Sa_Cod ")
            strSql.Append(" And   Reg_Impianti.Piva = Appezzamento.Piva ")
            strSql.Append(" And   Reg_Impianti.Sa_Cod = Appezzamento.Sa_Cod ")
            strSql.Append(" And   Reg_Impianti.Appezza = Appezzamento.Appezza ")
            strSql.Append(" And   Campi.Piva = Appezzamento.Piva ")
            strSql.Append(" And   Campi.Sa_Cod = Appezzamento.Sa_Cod ")
            strSql.Append(" And   Campi.Campo_Cod = Appezzamento.Campo_Cod ")
            strSql.Append(" And   Reg_Impianti.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And   Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod ")
            strSql.Append(" And   Reg_Impianti.Piva = Imprese_Progetti.Piva ")
            strSql.Append(" And   Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            strSql.Append(" And   Reg_Impianti.Appezza = Imprese_Progetti.Appezza ")
            strSql.Append(" And   Reg_Impianti.Id_Reg = Imprese_Progetti.Id_Reg ")
            strSql.Append(" And   Reg_Impianti.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And   Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod ")
            strSql.Append(" And Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            strSql.Append(" And Cultivar.Cul_Cod = Reg_impianti.Cul_Cod ")
            strSql.Append(" And ((Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_inizio) & " ")
            strSql.Append(" And Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio) & " ) ")
            strSql.Append(" Or ")
            strSql.Append("      (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine) & " ")
            strSql.Append(" And Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_fine) & " ))")


            'Filtro le distinte chiuse
            strSql.Append(" And Not Exists (Select 1 From Reg_Impianti_Codici AS Reg_Impianti_Distinta ")
            strSql.Append(" Where Reg_Impianti_Distinta.PIVA = Imprese_Progetti.Piva ")
            strSql.Append(" And Reg_Impianti_Distinta.SA_COD = Imprese_Progetti.Sa_Cod ")
            strSql.Append(" And Reg_Impianti_Distinta.APPEZZA = Imprese_Progetti.Appezza ")
            strSql.Append(" And Reg_Impianti_Distinta.id_reg = Imprese_Progetti.Id_Reg ")
            strSql.Append(" And Reg_Impianti_Distinta.Progetto_Cod = Imprese_Progetti.Progetto_Cod " & vbCrLf)
            strSql.Append(" And Reg_Impianti_Distinta.Id_Cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa))
            strSql.Append(" And Reg_Impianti_Distinta.Val_Cod = 1)")


            If veg_cod <> 0 Then
                strSql.Append(" And  Cultivar.Veg_Cod = " & veg_cod & " ")
            End If

            If campo_cod <> 0 Then
                strSql.Append(" And  Campi.Campo_Cod = " & campo_cod & " ")
            End If

            strSql.Append(" ORDER BY Impianto_Des")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)


            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function


    Public Function Leggi_Impianti_Da_Campo(ByVal piva As String,
                                            ByVal sa_Cod As Integer,
                                            ByVal campo_cod As Integer,
                                            ByVal data_movimento As Date,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Impianti_Da_Campo()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT Distinct Reg_Impianti.*, Imprese_Progetti.Progetto_Cod")

            strSql.Append(" From Appezzamento, Campi, Reg_Impianti ")

            strSql.Append(" Join Imprese_Progetti On")
            strSql.Append("  Reg_impianti.Piva = Imprese_Progetti.Piva ")
            strSql.Append(" And Reg_impianti.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            strSql.Append(" And Reg_impianti.Appezza = Imprese_Progetti.Appezza ")
            strSql.Append(" And Reg_impianti.Id_Reg = Imprese_Progetti.Id_Reg ")

            strSql.Append(" Left Join  reg_impianti_codici AS reg_impianti_distinta on ")
            strSql.Append(" reg_impianti_distinta.piva = Imprese_Progetti.Piva And ")
            strSql.Append(" reg_impianti_distinta.sa_cod = Imprese_Progetti.Sa_Cod And ")
            strSql.Append(" reg_impianti_distinta.appezza = Imprese_Progetti.Appezza And ")
            strSql.Append(" reg_impianti_distinta.id_reg = Imprese_Progetti.Id_Reg And ")
            strSql.Append(" reg_impianti_distinta.progetto_cod = Imprese_Progetti.Progetto_Cod And ")
            strSql.Append(" reg_impianti_distinta.id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa))

            strSql.Append(" Where Reg_impianti.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & " ")
            strSql.Append(" And Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ")
            strSql.Append(" And ( isnull(reg_impianti_distinta.id_cod, 0) != " & enum_CodiciAnagrafe.Distinta_Chiusa & " ")
            strSql.Append(" OR ( ")
            strSql.Append(" isnull(reg_impianti_distinta.id_cod, 0) = " & enum_CodiciAnagrafe.Distinta_Chiusa & " And isnull(reg_impianti_distinta.val_cod, 0) = 0 ) ")
            strSql.Append(" ) ")

            strSql.Append(" And   Reg_Impianti.Piva = Appezzamento.Piva ")
            strSql.Append(" And   Reg_Impianti.Sa_Cod = Appezzamento.Sa_Cod ")
            strSql.Append(" And   Reg_Impianti.Appezza = Appezzamento.Appezza ")
            strSql.Append(" And   Campi.Piva = Appezzamento.Piva ")
            strSql.Append(" And   Campi.Sa_Cod = Appezzamento.Sa_Cod ")
            strSql.Append(" And   Campi.Campo_Cod = Appezzamento.Campo_Cod ")
            strSql.Append(" And   Reg_Impianti.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And ((Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & " ")
            strSql.Append(" And Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ) ")
            strSql.Append(" Or ")
            strSql.Append("      (Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(data_movimento) & " ")
            strSql.Append(" And Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(data_movimento) & " ))")



            If sa_Cod <> 0 Then
                strSql.Append(" And  Campi.Sa_Cod = " & sa_Cod & " ")
            End If

            If campo_cod <> 0 Then
                strSql.Append(" And  Campi.Campo_Cod = " & campo_cod & " ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)


            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function





    Public Function Ricava_Last_Visita(ByVal piva As String,
                                       ByVal Id_Attivita As Integer,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Ricava_Last_Visita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" Select Top 1 Id_Attivita, CDG_Dettagli.Id_Imputazione  ")
            strSql.Append(" From CDG_Testata, CDG_Dettagli, Mov_Dettagli_Riferimenti")
            strSql.Append(" Where CDG_Testata.Piva_Superuser = '" & Piva_SuperUser & "'")
            strSql.Append(" And CDG_Testata.Piva = Mov_Dettagli_Riferimenti.Piva_Rif ")
            strSql.Append(" And CDG_Testata.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda_Rif ")
            strSql.Append(" And Mov_Dettagli_Riferimenti.Lav_Cod = " & LAVCOD_VISITA & " ")
            strSql.Append(" And CDG_Testata.Piva = CDG_Dettagli.Piva ")
            strSql.Append(" And CDG_Testata.Id_Cdg = CDG_Dettagli.Id_Cdg ")
            strSql.Append(" And CDG_Testata.Id_Attivita = " & Id_Attivita & " ")

            strSql.Append(" Order by CDG_Testata.Data_Creazione Desc ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function


    Public Function Ricava_Cod_Risum_Visita(ByVal piva As String,
                                            ByVal Id_Agenda As Integer,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Ricava_Cod_Risum_Visita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT Movimenti_Dettagli.Mat_Cod AS Cod_Risum, (Contatti.Rag_Soc + Contatti.Cognome + ' ' + Contatti.Nome) AS Descrizione  ")
            strSql.Append(" From Agenda, Movimenti, Movimenti_Dettagli, Risorse_Umane, Contatti")
            strSql.Append(" Where Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            strSql.Append(" And Agenda.Piva = Movimenti.Piva ")
            strSql.Append(" And Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.Append(" And Movimenti.Cau_Mov = '" & CAU_ASSEGNATARIO_VISITA & "' ")
            strSql.Append(" And Movimenti.Piva = Movimenti_Dettagli.Piva ")
            strSql.Append(" And Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            strSql.Append(" And Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            strSql.Append(" And Movimenti_Dettagli.Elem_Cod = 0 And Movimenti_Dettagli.Mat_Cod <> 0 ")
            strSql.Append(" And Movimenti_Dettagli.Mat_Cod = Risorse_Umane.Cod_Risum ")
            strSql.Append(" And Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function


    Public Function Ricava_Orario_Visita(ByVal piva As String,
                                         ByVal Id_Agenda As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Ricava_Orario_Visita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT Ora AS Ora_Inizio, OraFine AS Ora_Fine ")
            strSql.Append(" From Agenda, Movimenti")
            strSql.Append(" Where Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            strSql.Append(" And Agenda.Piva = Movimenti.Piva ")
            strSql.Append(" And Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.Append(" And Movimenti.Cau_Mov = '" & CAU_VISITE_ISPETTIVE & "' ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function


    Public Function Ricava_Id_Mov_Det(ByVal piva As String,
                                      ByVal id_agenda As Integer,
                                      ByVal id_mov_det As Integer,
                                      ByVal iavanti As Integer,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Ricava_Id_Mov_Det()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim Id_Mov_Det_New As Integer

        Try

            strSql.Length = 0
            strSql.Append(" SELECT Top 1 Id_Mov_Det")

            strSql.Append(" From Movimenti, Movimenti_Dettagli ")

            strSql.Append(" Where Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(id_agenda) & " ")
            strSql.Append(" And Movimenti.Piva = Movimenti_Dettagli.Piva ")
            strSql.Append(" And Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            strSql.Append(" And Movimenti.Id_Mov= Movimenti_Dettagli.Id_Mov ")
            strSql.Append(" And Movimenti.Cau_Mov In ('7300', '7350') ")
            strSql.Append(" And Movimenti_Dettagli.Elem_Cod Not In (0, 502) ")

            Select Case iavanti

                Case 2 'Prossimo

                    strSql.Append(" And Movimenti_Dettagli.Id_Mov_Det <> " & Agro_SQL_SaveNum(id_mov_det) & " ")

                    strSql.Append(" And Not Exists (Select 1 From Mov_Dettagli_Riferimenti Where Piva = Movimenti_Dettagli.Piva ")
                    strSql.Append("                                                        And   Id_Mov = Movimenti_Dettagli.Id_Mov  ")
                    strSql.Append("                                                        And   Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")
                    strSql.Append("                                                        And   Lav_Cod_Rif = 4500 ) ")

                    strSql.Append(" Order by Id_Mov_Det Asc ")

                Case 1 'Avanti

                    strSql.Append(" And Movimenti_Dettagli.Id_Mov_Det > " & Agro_SQL_SaveNum(id_mov_det) & " ")
                    strSql.Append(" Order by Id_Mov_Det Asc ")

                Case 0 'Indietro

                    strSql.Append(" And Movimenti_Dettagli.Id_Mov_Det < " & Agro_SQL_SaveNum(id_mov_det) & " ")
                    strSql.Append(" Order by Id_Mov_Det Desc ")

            End Select



            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

            If dt.Rows.Count > 0 Then

                For Each dr As DataRow In dt.Rows

                    Id_Mov_Det_New = dr.Item("Id_Mov_Det")

                Next

            Else

                Id_Mov_Det_New = id_mov_det

            End If

            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return Id_Mov_Det_New

    End Function
    Public Function Leggi_Testata_Da_Agenda_QDC(ByVal piva As String,
                                                ByVal Id_Agenda As Integer,
                                                ByVal Filtro_Distinta As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Testata_Da_Agenda_QDC()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim chiave As String() = Filtro_Distinta.Split("-")
        Dim kPiva As String = CStr(chiave(0))
        Dim kSa_Cod As Integer = Integer.Parse(chiave(1))
        Dim kAppezza As Integer = Integer.Parse(chiave(3))
        Dim kId_Reg As Integer = Integer.Parse(chiave(4))
        Dim kProgetto_Cod As Integer = Integer.Parse(chiave(5))

        Try



            strSql.Length = 0
            strSql.Append(" SELECT Distinct CDG_Testata.*, Attivita.Attivita_PoliAnnuale, CDG_Dettagli.Valore, Agenda.Des_Lib, Agenda.Validita_Inizio AS Validita_Inizio_Agenda ")

            strSql.Append(" From Attivita, CDG_Dettagli, Agenda, CDG_Testata ")

            'strSql.Append(" Join Mov_Dettagli_Riferimenti On")
            'strSql.Append("  CDG_Testata.Piva = Mov_Dettagli_Riferimenti.Piva ")
            'strSql.Append(" And Mov_Dettagli_Riferimenti.Lav_Cod_Rif = 4500 ")
            'strSql.Append(" And Mov_Dettagli_Riferimenti.Id_Agenda = " & Id_Agenda & " ")
            'strSql.Append(" And CDG_Testata.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda_Rif ")

            strSql.Append(" Where CDG_Testata.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And CDG_Testata.Id_Attivita = Attivita.Id_Attivita ")

            strSql.Append(" And CDG_Testata.Piva = Agenda.Piva ")
            strSql.Append(" And CDG_Testata.Id_Agenda = Agenda.Id_Agenda ")

            strSql.Append(" And Agenda.Id_Agenda = " & Id_Agenda & " And CDG_Testata.Budget = 0 ")


            strSql.Append(" And CDG_Dettagli.Piva = CDG_Testata.Piva ")
            strSql.Append(" And CDG_Dettagli.Id_CDG = CDG_Testata.Id_CDG ")
            strSql.Append(" And CDG_Dettagli.Sa_Cod = " & kSa_Cod)
            strSql.Append(" And CDG_Dettagli.Appezza = " & kAppezza)
            strSql.Append(" And CDG_Dettagli.Id_Reg = " & kId_Reg)
            strSql.Append(" And CDG_Dettagli.Progetto_Cod = " & kProgetto_Cod)


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)


            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function


    Public Function Leggi_Dettagli(ByVal piva As String,
                                   ByVal sa_cod As Integer,
                                   ByVal appezza As Integer,
                                   ByVal id_reg As Integer,
                                   ByVal progetto_cod As Integer,
                                   ByVal id_cdg As Integer,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Dettagli()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT *  ")
            strSql.Append(" From CDG_Dettagli ")
            strSql.Append(" Where CDG_Dettagli.Piva = '" & Agro_SQL_SaveText(piva) & "'")

            If sa_cod <> 0 Then
                strSql.Append(" And  CDG_Dettagli.Sa_Cod = " & sa_cod & " ")
            End If

            If appezza <> 0 Then
                strSql.Append(" And  CDG_Dettagli.appezza = " & appezza & " ")
            End If

            If id_reg <> 0 Then
                strSql.Append(" And  CDG_Dettagli.id_reg = " & id_reg & " ")
            End If

            If progetto_cod <> 0 Then
                strSql.Append(" And  CDG_Dettagli.Progetto_Cod = " & progetto_cod & " ")
            End If

            If id_cdg <> 0 Then
                strSql.Append(" And  CDG_Dettagli.id_cdg = " & id_cdg & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function



    Public Function Leggi_Distinte(ByVal piva As String,
                                   ByVal sa_cod As Integer,
                                   ByVal appezza As Integer,
                                   ByVal id_reg As Integer,
                                   ByVal validita_inizio As Date,
                                   ByVal validita_fine As Date,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Distinte()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT Distinct (Sa_Nome + ' - ' + App_Nome + ' - ' + Veg_Des + ' - ' + Cul_Des) AS Impianto_Des, ")
            strSql.Append(" Imprese_Progetti.Progetto_Cod, ")
            strSql.Append(" Imprese_Progetti.Progetto_Nome AS Progetto_Des, ")
            strSql.Append(" Reg_Impianti.Sa_Cod, Reg_Impianti.Appezza, Reg_Impianti.Id_Reg ")
            strSql.Append(" From Centri_Aziendali, Appezzamento, Reg_Impianti, Imprese_Progetti, SpecieVegetali, Cultivar ")
            strSql.Append(" Where Reg_Impianti.Piva = Centri_Aziendali.Piva ")
            strSql.Append(" And   Reg_Impianti.Sa_Cod = Centri_Aziendali.Sa_Cod ")
            strSql.Append(" And   Reg_Impianti.Piva = Appezzamento.Piva ")
            strSql.Append(" And   Reg_Impianti.Sa_Cod = Appezzamento.Sa_Cod ")
            strSql.Append(" And   Reg_Impianti.Appezza = Appezzamento.Appezza ")
            strSql.Append(" And   Reg_Impianti.Piva = Imprese_Progetti.Piva ")
            strSql.Append(" And   Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            strSql.Append(" And   Reg_Impianti.Appezza = Imprese_Progetti.Appezza ")
            strSql.Append(" And   Reg_Impianti.Id_Reg = Imprese_Progetti.Id_Reg ")
            strSql.Append(" And   Reg_Impianti.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And   Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod ")
            strSql.Append(" And Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            strSql.Append(" And Cultivar.Cul_Cod = Reg_impianti.Cul_Cod ")
            strSql.Append(" And Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_inizio) & " ")
            strSql.Append(" And Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_fine) & " ")


            If sa_cod <> 0 Then
                strSql.Append(" And  Reg_Impianti.Sa_Cod = " & sa_cod & " ")
            End If

            If appezza <> 0 Then
                strSql.Append(" And  Reg_Impianti.appezza = " & appezza & " ")
            End If

            If id_reg <> 0 Then
                strSql.Append(" And  Reg_Impianti.id_reg = " & id_reg & " ")
            End If

            strSql.Append(" ORDER BY Progetto_Des")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function


    '##############################################################################################
    Public Function Leggi_Varieta_Impianti(ByVal piva As String,
                                           ByVal filtro_centro As String,
                                           ByVal filtro_specie As String,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal Id_Budget As Integer = 0
                                           ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Impianti_Dettagli()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim TabellaPrefisso As String = ""

        Try

            If Id_Budget <> 0 Then
                TabellaPrefisso = "Budget_"
            End If

            strSql.Length = 0
            strSql.Append(" SELECT Distinct Cultivar.Cul_Cod, Veg_Des + ' - ' + Cul_Des AS Cul_Des From Cultivar, " & TabellaPrefisso & "Reg_Impianti, SpecieVegetali ")
            strSql.Append(" Where " & TabellaPrefisso & "Reg_Impianti.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            strSql.Append(" And Cultivar.Cul_Cod = " & TabellaPrefisso & "Reg_Impianti.Cul_Cod ")

            If filtro_centro <> "0" Then
                strSql.Append(" And " & TabellaPrefisso & "Reg_Impianti.Sa_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_centro) & ") ")
            End If

            If filtro_specie <> "0" Then
                strSql.Append(" And Cultivar.Veg_Cod In (" & Agro_SQL_Save_Clausola_IN(filtro_specie) & ") ")
            End If

            If Id_Budget <> 0 Then
                strSql.Append(" And " & TabellaPrefisso & "Reg_impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            strSql.Append(" ORDER BY Cul_Des")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function


    '##############################################################################################
    Public Function Leggi_CDG(ByVal piva As String,
                              ByVal id_agenda As Integer,
                              ByVal id_mov_det As Integer,
                              ByVal id_agenda_cdg As Integer,
                              ByVal data_movimento As Date,
                              ByVal Automatico As Integer,
                              ByVal Id_Attivita_Eredita As Integer,
                              ByVal Attivita_Des_Eredita As String,
                              ByVal split As Integer,
                              ByRef objParametri As AgronicaCoreParametri,
                              Optional ByVal bForzaLeggiDettDistinta As Boolean = False,
                              Optional ByVal Budget As Integer = 0,
                              Optional ByVal Raccoglitore_Cod As Integer = 0
                              ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_CDG()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim strCau_Mov As String() = {CAU_SCARICO}

        'Dim strElem_Cod_Fertilizzanti AS Integer() = {CostantiPersonalizzate.FERTILIZZANTI}


        Dim JsonString As New StringBuilder()

        Dim kendo_Manodopera As String = ""
        Dim kendo_Terzisti As String = ""
        Dim kendo_Macchine As String = ""
        Dim kendo_Magazzino As String = ""
        Dim kendo_Libera As String = ""
        Dim kendo_Eredita As String = ""
        Dim kendo_Impianti_Dettagli As String = ""
        Dim kendo_Macchine_Dettagli As String = ""
        Dim kendo_Progetti_Dettagli As String = ""
        Dim kendo_Linee_Dettagli As String = ""
        Dim kendo_Zoo_Dettagli As String = ""
        Dim bValido As Boolean = False

        Leggi_CDG_Testate_Parte1(piva,
                                id_agenda,
                                id_mov_det,
                                id_agenda_cdg,
                                Automatico,
                                objParametri,
                                kendo_Manodopera,
                                kendo_Terzisti,
                                kendo_Macchine)

        Leggi_CDG_Testate_Parte2(piva,
                                id_agenda,
                                id_mov_det,
                                id_agenda_cdg,
                                Automatico,
                                objParametri,
                                kendo_Magazzino,
                                kendo_Libera)


        Leggi_CDG_Testate_Parte3(piva,
                                id_agenda,
                                id_mov_det,
                                id_agenda_cdg,
                                Automatico,
                                Id_Attivita_Eredita,
                                Attivita_Des_Eredita,
                                objParametri,
                                kendo_Eredita,
                                Raccoglitore_Cod)

        Leggi_CDG_Dettagli(piva,
                           id_agenda,
                           id_agenda_cdg,
                           data_movimento,
                           Automatico,
                           objParametri,
                           kendo_Impianti_Dettagli, kendo_Macchine_Dettagli, kendo_Progetti_Dettagli, kendo_Linee_Dettagli, kendo_Zoo_Dettagli, split, bForzaLeggiDettDistinta, Budget, Raccoglitore_Cod)


        'Concatenazione Json       
        JsonString.Append("{ ")

        If kendo_Manodopera <> "" AndAlso kendo_Manodopera <> "[]" Then
            If bValido Then
                JsonString.Append(" ,""kendo_Manodopera"": " & kendo_Manodopera)
            Else
                JsonString.Append(" ""kendo_Manodopera"": " & kendo_Manodopera)
            End If
            bValido = True
        End If

        If kendo_Terzisti <> "" AndAlso kendo_Terzisti <> "[]" Then
            If bValido Then
                JsonString.Append(" ,""kendo_Terzisti"": " & kendo_Terzisti)
            Else
                JsonString.Append(" ""kendo_Terzisti"": " & kendo_Terzisti)
            End If
            bValido = True
        End If

        If kendo_Macchine <> "" AndAlso kendo_Macchine <> "[]" Then
            If bValido Then
                JsonString.Append(" ,""kendo_Macchine"": " & kendo_Macchine)
            Else
                JsonString.Append(" ""kendo_Macchine"": " & kendo_Macchine)
            End If
            bValido = True
        End If

        If kendo_Magazzino <> "" AndAlso kendo_Magazzino <> "[]" Then
            If bValido Then
                JsonString.Append(" ,""kendo_Magazzino"": " & kendo_Magazzino)
            Else
                JsonString.Append(" ""kendo_Magazzino"": " & kendo_Magazzino)
            End If
            bValido = True
        End If

        If kendo_Libera <> "" AndAlso kendo_Libera <> "[]" Then
            If bValido Then
                JsonString.Append(" ,""kendo_Libera"": " & kendo_Libera)
            Else
                JsonString.Append(" ""kendo_Libera"": " & kendo_Libera)
            End If
            bValido = True
        End If

        If kendo_Eredita <> "" AndAlso kendo_Eredita <> "[]" Then
            If bValido Then
                JsonString.Append(" ,""kendo_Eredita"": " & kendo_Eredita)
            Else
                JsonString.Append(" ""kendo_Eredita"": " & kendo_Eredita)
            End If
            bValido = True
        End If


        If kendo_Impianti_Dettagli <> "" AndAlso kendo_Impianti_Dettagli <> "[]" Then
            If bValido Then
                JsonString.Append(" ,""kendo_Impianti_Dettagli"": " & kendo_Impianti_Dettagli)
            Else
                JsonString.Append(" ""kendo_Impianti_Dettagli"": " & kendo_Impianti_Dettagli)
            End If
            bValido = True
        End If

        If kendo_Progetti_Dettagli <> "" AndAlso kendo_Progetti_Dettagli <> "[]" Then
            If bValido Then
                JsonString.Append(" ,""kendo_Progetti_Dettagli"": " & kendo_Progetti_Dettagli)
            Else
                JsonString.Append(" ""kendo_Progetti_Dettagli"": " & kendo_Progetti_Dettagli)
            End If
            bValido = True
        End If

        If kendo_Linee_Dettagli <> "" AndAlso kendo_Linee_Dettagli <> "[]" Then
            If bValido Then
                JsonString.Append(" ,""kendo_Linee_Dettagli"": " & kendo_Linee_Dettagli)
            Else
                JsonString.Append(" ""kendo_Linee_Dettagli"": " & kendo_Linee_Dettagli)
            End If
            bValido = True
        End If

        If kendo_Macchine_Dettagli <> "" AndAlso kendo_Macchine_Dettagli <> "[]" Then
            If bValido Then
                JsonString.Append(" ,""kendo_Macchine_Dettagli"": " & kendo_Macchine_Dettagli)
            Else
                JsonString.Append(" ""kendo_Macchine_Dettagli"": " & kendo_Macchine_Dettagli)
            End If
            bValido = True
        End If

        If kendo_Zoo_Dettagli <> "" AndAlso kendo_Zoo_Dettagli <> "[]" Then
            If bValido Then
                JsonString.Append(" ,""kendo_Zoo_Dettagli"": " & kendo_Zoo_Dettagli)
            Else
                JsonString.Append(" ""kendo_Zoo_Dettagli"": " & kendo_Zoo_Dettagli)
            End If
            bValido = True
        End If


        JsonString.Append("} ")

        risposta = JsonString.ToString()


        Return risposta

    End Function


    '##############################################################################################
    Public Sub Leggi_CDG_Testate_Parte1(ByVal piva As String,
                                        ByVal id_agenda As Integer,
                                        ByVal id_mov_det As Integer,
                                        ByVal id_agenda_cdg As Integer,
                                        ByVal Automatico As Integer,
                                        objParametri As AgronicaCoreParametri,
                                        ByRef kendo_Manodopera As String,
                                        ByRef kendo_Terzisti As String,
                                        ByRef kendo_Macchine As String,
                                        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing)

        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim Ora As Integer
        Dim Minuti As Integer
        Dim parteIntera As Decimal
        Dim parteDecimali As Decimal

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_CDG_Testate_Parte1()"

        'Dim gefutils AS New Gias_EF_Utility
        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim strCau_Mov As String() = {CAU_SCARICO}

        'Dim strElem_Cod_Fertilizzanti AS Integer() = {CostantiPersonalizzate.FERTILIZZANTI}

        Dim bValido As Boolean = False

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If


        Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}


        '######################################################################################################################
        '############################################ TESTATA #################################################################
        '######################################################################################################################

        If id_agenda_cdg <> 0 Then

            'Manodopera
            Dim TestataElem_Manodopera =
               From CI In GiasContext.CDG_Testata
               Join Risorse_Umane In GiasContext.Risorse_Umane
                 On Risorse_Umane.Cod_RisUm Equals CI.Cod_RisUm
               Join Rapporti_Contabili In GiasContext.Rapporti_Contabili
                 On Risorse_Umane.Cod_Rapporto Equals Rapporti_Contabili.Cod_Rapporto
               Join Contatti In GiasContext.Contatti
                 On Contatti.Piva Equals Risorse_Umane.Piva And
                    Contatti.Cod_Contatto Equals Risorse_Umane.Cod_Contatto
               Group Join Unita_Misura In GiasContext.UnitaMisura
                 On Unita_Misura.UDM_COD Equals CI.Udm_Cod Into UnitaMisura_Group = Group
               From _Unita_Misura In UnitaMisura_Group.DefaultIfEmpty()
               Group Join Qualifiche In GiasContext.Qualifiche
                 On Qualifiche.Qualifica_Cod Equals CI.Qualifica_Cod Into Qualifica_Group = Group
               From _Qualifica_Group In Qualifica_Group.DefaultIfEmpty()
               Group Join Attivita In GiasContext.Attivita
                 On Attivita.ID_Attivita Equals CI.Id_Attivita Into Attivita_Group = Group
               From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
               Group Join Tariffe In GiasContext.Tariffe
                 On Tariffe.Tariffa_Cod Equals CI.Tariffa_Cod Into Tariffa_Group = Group
               From _Tariffa_Group In Tariffa_Group.DefaultIfEmpty()
               Group Join CategorieMagazzino In GiasContext.CategorieMagazzino
                 On CategorieMagazzino.Elem_Cod Equals CI.Elem_Cod Into CategorieMagazzino_Group = Group
               From _CategorieMagazzino In CategorieMagazzino_Group.DefaultIfEmpty()
               Where
              (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
              AndAlso (CI.Piva.Equals(piva)) _
              AndAlso (CI.Id_Agenda = id_agenda_cdg) _
              AndAlso (CI.Vecchio_Tipo_Inser_Dati = 0) _
               AndAlso (CI.Flag_Movimento_Campagna = 0)
               Select New With {
                   .Id_CDG = CI.Id_CDG,
                   .Data_Inserimento = CI.Data_Inserimento,
                   .Modalita_Imputazione = CI.Modalita_Imputazione,
                   .Id_Agenda = CI.Id_Agenda,
                   .Id_Mov = CI.Id_Mov,
                   .Id_Mov_Det = CI.Id_Mov_Det,
                   .Mac_Cod = CI.Mac_Cod,
                   .Cod_Risum = CI.Cod_RisUm,
                   .Elem_Cod = CI.Elem_Cod,
                   .Pro_Cod = CI.Pro_Cod,
                   .Mat_Cod = CI.Mat_Cod,
                   .Lav_Cod = CI.Lav_Cod,
                   .ID_Attivita = CI.Id_Attivita,
                   .Qualifica_Cod = CI.Qualifica_Cod,
                   .Tariffa_Cod = CI.Tariffa_Cod,
                   .Turno_Cod = CI.Turno_Cod,
                   .Conto_Cod = CI.Conto_Cod,
                   .Lotto = CI.Lotto,
                   .Mezzo = CI.Mezzo,
                   .Udm_Cod = CI.Udm_Cod,
                   .Ora_Inizio = If(CI.Data_Ora_Inizio Is Nothing, AGRODATAINIZIO, CI.Data_Ora_Inizio),
                   .Ora_Fine = If(CI.Data_Ora_Fine Is Nothing, AGRODATAFINE, CI.Data_Ora_Fine),
                   .Qta = CI.Qta,
                   .Ora = AGRODATAINIZIO,
                   .Prezzo_Unitario = CI.Prezzo_Unitario,
                   .Prezzo_Totale = CI.Valore_Totale,
                   .Descrizione = CI.Descrizione,
                   .Tipo_Ripartizione = CI.Tipo_Ripartizione,
                   .Budget = CI.Budget,
                   .Costi_Ricavi = CI.Costi_Ricavi,
                   .Modalita_Ripartizione = CI.Modalita_Ripartizione,
                   .Vecchio_Tipo_Inser_Dati = CI.Vecchio_Tipo_Inser_Dati,
                   .Rag_Soc = Contatti.Rag_Soc,
                   .Cognome = Contatti.Cognome,
                   .Nome = Contatti.Nome,
                   .Prodotto_Des = Contatti.Cognome & " " & Contatti.Nome,
                   .Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto,
                   .Rapporto_Des = Rapporti_Contabili.Rapporto_Des,
                   .Qualifica_Des = If(_Qualifica_Group Is Nothing, "", _Qualifica_Group.Qualifica_Des),
                   .Desc = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Sigla & " - " & _Attivita_Group.Desc),
                   .Tariffa_Des = If(_Tariffa_Group Is Nothing, "", _Tariffa_Group.Tariffa_Des),
                   .Udm_Des = If(_Unita_Misura Is Nothing, "", _Unita_Misura.UDM_SIM),
                   .OrigineApp = CI.OrigineApp,
                   .APP_CDG_Generale_ID = CI.APP_CDG_Generale_ID
                   }
            'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
            '.Bozza_Cod = CI.Bozza,
            '.Bozza_Des = If(CI.Bozza = 1, Gias.Si, Gias.No)


            ' Compongo la chiave
            Dim myList_Manodopera = TestataElem_Manodopera.ToList()
            For Each obj In myList_Manodopera
                obj.Rag_Soc = obj.Rag_Soc & "" & obj.Cognome & " " & obj.Nome

                'Se ora inizio e fine sono uguali significa che sto gestendo direttamente le quantità e potenzialmente più di 24 ore
                If obj.Ora_Inizio <> obj.Ora_Fine Then
                    'Conversione intero minuti a data
                    parteIntera = Math.Truncate(obj.Qta)
                    parteDecimali = obj.Qta - parteIntera
                    Ora = parteIntera
                    Minuti = 60 * parteDecimali
                    obj.Ora = "01/01/1900 " & Ora.ToString("D2") & ":" & Minuti.ToString("D2")
                End If

            Next

            kendo_Manodopera = JsonConvert.SerializeObject(myList_Manodopera, Formatting.None, serializerSettings)
            TestataElem_Manodopera = Nothing
            myList_Manodopera = Nothing


            'Terzisti
            Dim TestataElem_Terzisti =
                 From CI In GiasContext.CDG_Testata
                 Group Join Fabbricati In GiasContext.Fabbricati
                 On CI.Piva Equals Fabbricati.PIVA And
                    CI.Sa_Cod Equals Fabbricati.SA_COD And
                    CI.Id_Destinazione Equals Fabbricati.Fabbricato_Cod Into Fabbricati_Group = Group
                 From _Fabbricati_Group In Fabbricati_Group.DefaultIfEmpty()
                 Group Join Centri_Aziendali In GiasContext.Centri_Aziendali
                 On Centri_Aziendali.PIVA Equals CI.Piva And
                    Centri_Aziendali.sa_cod Equals CI.Sa_Cod Into Centri_Aziendali_Group = Group
                 From _Centri_Aziendali_Group In Centri_Aziendali_Group.DefaultIfEmpty()
                 Join Materie_Prime In GiasContext.Materie_Prime
                     On Materie_Prime.Elem_Cod Equals CI.Elem_Cod And
                        Materie_Prime.Mat_Cod Equals CI.Mat_Cod
                 Join Unita_Misura In GiasContext.UnitaMisura
                     On Unita_Misura.UDM_COD Equals CI.Udm_Cod
                 Group Join Qualifiche In GiasContext.Qualifiche
                     On Qualifiche.Qualifica_Cod Equals CI.Qualifica_Cod Into Qualifica_Group = Group
                 From _Qualifica_Group In Qualifica_Group.DefaultIfEmpty()
                 Group Join Attivita In GiasContext.Attivita
                     On Attivita.ID_Attivita Equals CI.Id_Attivita Into Attivita_Group = Group
                 From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
                 Group Join Tariffe In GiasContext.Tariffe
                     On Tariffe.Tariffa_Cod Equals CI.Tariffa_Cod Into Tariffa_Group = Group
                 From _Tariffa_Group In Tariffa_Group.DefaultIfEmpty()
                 Group Join CategorieMagazzino In GiasContext.CategorieMagazzino
                     On CategorieMagazzino.Elem_Cod Equals CI.Elem_Cod Into CategorieMagazzino_Group = Group
                 From _CategorieMagazzino In CategorieMagazzino_Group.DefaultIfEmpty()
                 Where
                  (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                  AndAlso (CI.Piva.Equals(piva)) _
                  AndAlso (CI.Id_Agenda = id_agenda_cdg) _
                  AndAlso (CI.Elem_Cod = 700) _
                  AndAlso (CI.Vecchio_Tipo_Inser_Dati = 0) _
                  AndAlso (CI.Flag_Movimento_Campagna = 0)
                 Select New With {
                     .Id_CDG = CI.Id_CDG,
                     .Data_Inserimento = CI.Data_Inserimento,
                     .Modalita_Imputazione = CI.Modalita_Imputazione,
                     .Id_Agenda = CI.Id_Agenda,
                     .Id_Mov = CI.Id_Mov,
                     .Id_Mov_Det = CI.Id_Mov_Det,
                     .Mac_Cod = CI.Mac_Cod,
                     .Cod_Risum = CI.Cod_RisUm,
                     .Elem_Cod = CI.Elem_Cod,
                     .Pro_Cod = CI.Pro_Cod,
                     .Mat_Cod = CI.Mat_Cod,
                     .Lav_Cod = CI.Lav_Cod,
                     .ID_Attivita = CI.Id_Attivita,
                     .Qualifica_Cod = CI.Qualifica_Cod,
                     .Tariffa_Cod = CI.Tariffa_Cod,
                     .Turno_Cod = CI.Turno_Cod,
                     .Conto_Cod = CI.Conto_Cod,
                     .Lotto = CI.Lotto,
                     .Mezzo = CI.Mezzo,
                     .Udm_Cod = CI.Udm_Cod,
                     .Qta = CI.Qta,
                     .Prezzo_Unitario = CI.Prezzo_Unitario,
                     .Prezzo_Totale = CI.Valore_Totale,
                     .Descrizione = CI.Descrizione,
                     .Tipo_Ripartizione = CI.Tipo_Ripartizione,
                     .Budget = CI.Budget,
                     .Costi_Ricavi = CI.Costi_Ricavi,
                     .Modalita_Ripartizione = CI.Modalita_Ripartizione,
                     .Vecchio_Tipo_Inser_Dati = CI.Vecchio_Tipo_Inser_Dati,
                     .Sa_Cod = CI.Sa_Cod,
                     .Sa_Nome = If(_Centri_Aziendali_Group Is Nothing, "", _Centri_Aziendali_Group.sa_nome),
                     .Tipo_Destinazione = CI.Tipo_Destinazione,
                     .Fabbricato_Cod = CI.Id_Destinazione,
                     .Fabbricato_Des = If(_Fabbricati_Group Is Nothing, "", _Fabbricati_Group.Fabbricato_Des),
                     .Prodotto_Des = Materie_Prime.Mat_Des,
                     .Udm_Des = Unita_Misura.UDM_SIM,
                     .Qualifica_Des = If(_Qualifica_Group Is Nothing, "", _Qualifica_Group.Qualifica_Des),
                     .Desc = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Sigla & " - " & _Attivita_Group.Desc),
                     .Tariffa_Des = If(_Tariffa_Group Is Nothing, "", _Tariffa_Group.Tariffa_Des),
                     .OrigineApp = CI.OrigineApp,
                     .APP_CDG_Generale_ID = CI.APP_CDG_Generale_ID
                     }

            'Imposto la testata se non fatto precedentemente
            Dim myList_Terzisti = TestataElem_Terzisti.ToList()
            'For Each obj In myList_Terzisti
            '    Id_CDG = obj.Id_CDG
            '    Exit For
            'Next

            kendo_Terzisti = JsonConvert.SerializeObject(myList_Terzisti, Formatting.None, serializerSettings)
            TestataElem_Terzisti = Nothing
            myList_Terzisti = Nothing

            'Macchine
            Dim TestataElem_Macchine =
               From CI In GiasContext.CDG_Testata
               Join Parco_Macchine In GiasContext.Parco_Macchine
                 On Parco_Macchine.Mac_Cod Equals CI.Mac_Cod
               Group Join Centri_Aziendali In GiasContext.Centri_Aziendali
                 On Centri_Aziendali.PIVA Equals Parco_Macchine.Piva And
                    Centri_Aziendali.sa_cod Equals Parco_Macchine.Sa_Cod Into Centri_Aziendali_Group = Group
               From _Centri_Aziendali In Centri_Aziendali_Group.DefaultIfEmpty()
               Group Join Unita_Misura In GiasContext.UnitaMisura
                 On Unita_Misura.UDM_COD Equals CI.Udm_Cod Into UnitaMisura_Group = Group
               From _Unita_Misura In UnitaMisura_Group.DefaultIfEmpty()
               Group Join Qualifiche In GiasContext.Qualifiche
                 On Qualifiche.Qualifica_Cod Equals CI.Qualifica_Cod Into Qualifica_Group = Group
               From _Qualifica_Group In Qualifica_Group.DefaultIfEmpty()
               Group Join Attivita In GiasContext.Attivita
                 On Attivita.ID_Attivita Equals CI.Id_Attivita Into Attivita_Group = Group
               From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
               Group Join Tariffe In GiasContext.Tariffe
                 On Tariffe.Tariffa_Cod Equals CI.Tariffa_Cod Into Tariffa_Group = Group
               From _Tariffa_Group In Tariffa_Group.DefaultIfEmpty()
               Where
              (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
              AndAlso (CI.Piva.Equals(piva)) _
              AndAlso (CI.Id_Agenda = id_agenda_cdg) _
              AndAlso (CI.Mac_Cod <> 0) _
              AndAlso (CI.Vecchio_Tipo_Inser_Dati = 0) _
              AndAlso (CI.Flag_Movimento_Campagna = 0)
               Select New With {
                   .Id_CDG = CI.Id_CDG,
                   .Data_Inserimento = CI.Data_Inserimento,
                   .Modalita_Imputazione = CI.Modalita_Imputazione,
                   .Id_Agenda = CI.Id_Agenda,
                   .Id_Mov = CI.Id_Mov,
                   .Id_Mov_Det = CI.Id_Mov_Det,
                   .Mac_Cod = CI.Mac_Cod,
                   .Cod_Risum = CI.Cod_RisUm,
                   .Elem_Cod = CI.Elem_Cod,
                   .Pro_Cod = CI.Pro_Cod,
                   .Mat_Cod = CI.Mat_Cod,
                   .Lav_Cod = CI.Lav_Cod,
                   .ID_Attivita = CI.Id_Attivita,
                   .Qualifica_Cod = CI.Qualifica_Cod,
                   .Tariffa_Cod = CI.Tariffa_Cod,
                   .Turno_Cod = CI.Turno_Cod,
                   .Conto_Cod = CI.Conto_Cod,
                   .Lotto = CI.Lotto,
                   .Mezzo = CI.Mezzo,
                   .Udm_Cod = CI.Udm_Cod,
                   .Ora_Inizio = If(CI.Data_Ora_Inizio Is Nothing, AGRODATAINIZIO, CI.Data_Ora_Inizio),
                   .Ora_Fine = If(CI.Data_Ora_Fine Is Nothing, AGRODATAFINE, CI.Data_Ora_Fine),
                   .Qta = CI.Qta,
                   .Ora = AGRODATAINIZIO,
                   .Prezzo_Unitario = CI.Prezzo_Unitario,
                   .Prezzo_Totale = CI.Valore_Totale,
                   .Descrizione = CI.Descrizione,
                   .Tipo_Ripartizione = CI.Tipo_Ripartizione,
                   .Budget = CI.Budget,
                   .Costi_Ricavi = CI.Costi_Ricavi,
                   .Modalita_Ripartizione = CI.Modalita_Ripartizione,
                   .Vecchio_Tipo_Inser_Dati = CI.Vecchio_Tipo_Inser_Dati,
                   .Sa_Cod = If(_Centri_Aziendali Is Nothing, 0, _Centri_Aziendali.sa_cod),
                   .Sa_Nome = If(_Centri_Aziendali Is Nothing, "", _Centri_Aziendali.sa_nome),
                   .Tipo = Parco_Macchine.Tipo,
                   .Tipo_Des = If(Parco_Macchine.Tipo = 0, "Agricolo Zootecnico", If(Parco_Macchine.Tipo = 1, "Industriale", "Commerciale")),
                   .CLASS_CODE_ROOT = Left(Parco_Macchine.Class_Code, 2),
                   .CLASS_DESC = "",
                   .Mac_Des = Parco_Macchine.Mac_Des,
                   .Prodotto_Des = Parco_Macchine.Mac_Des,
                   .Qualifica_Des = If(_Qualifica_Group Is Nothing, "", _Qualifica_Group.Qualifica_Des),
                   .Desc = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Sigla & " - " & _Attivita_Group.Desc),
                   .Tariffa_Des = If(_Tariffa_Group Is Nothing, "", _Tariffa_Group.Tariffa_Des),
                   .Udm_Des = If(_Unita_Misura Is Nothing, "", _Unita_Misura.UDM_SIM),
                    .OrigineApp = CI.OrigineApp,
                   .APP_CDG_Generale_ID = CI.APP_CDG_Generale_ID
                    }

            'Imposto la testata se non fatto precedentemente
            Dim myList_Macchine = TestataElem_Macchine.ToList()
            For Each obj In myList_Macchine
                'Se ora inizio e fine sono uguali significa che sto gestendo direttamente le quantità e potenzialmente più di 24 ore
                If obj.Ora_Inizio <> obj.Ora_Fine Then
                    'Conversione intero minuti a data
                    parteIntera = Math.Truncate(obj.Qta)
                    parteDecimali = obj.Qta - parteIntera
                    Ora = parteIntera
                    Minuti = 60 * parteDecimali
                    obj.Ora = "01/01/1900 " & Ora.ToString("D2") & ":" & Minuti.ToString("D2")
                End If
            Next

            kendo_Macchine = JsonConvert.SerializeObject(myList_Macchine, Formatting.None, serializerSettings)
            TestataElem_Macchine = Nothing
            myList_Macchine = Nothing

        End If

        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

    End Sub



    '##############################################################################################
    Public Sub Leggi_CDG_Testate_Parte2(ByVal piva As String,
                                        ByVal id_agenda As Integer,
                                        ByVal id_mov_det As Integer,
                                        ByVal id_agenda_cdg As Integer,
                                        ByVal Automatico As Integer,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByRef kendo_Magazzino As String,
                                        ByRef kendo_Libera As String,
                                        Optional ByVal Raccoglitore_Cod As Integer = 0,
                                        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                        Optional bypass_Kendo_Libera As Boolean = False)

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_CDG_Testate_Parte2()"

        'Dim gefutils AS New Gias_EF_Utility
        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim strCau_Mov As String() = {CAU_SCARICO}

        'Dim strElem_Cod_Fertilizzanti AS Integer() = {3}

        Dim bValido As Boolean = False

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}


        '######################################################################################################################
        '############################################ TESTATA #################################################################
        '######################################################################################################################


        If id_agenda_cdg <> 0 Then

            ''Manodopera
            'Dim TestataElem_Manodopera =
            '   From CI In GiasContext.CDG_Testata
            '   Join Risorse_Umane In GiasContext.Risorse_Umane
            '     On Risorse_Umane.Cod_RisUm Equals CI.Cod_RisUm
            '   Join Rapporti_Contabili In GiasContext.Rapporti_Contabili
            '     On Risorse_Umane.Cod_Rapporto Equals Rapporti_Contabili.Cod_Rapporto
            '   Join Contatti In GiasContext.Contatti
            '     On Contatti.Piva Equals Risorse_Umane.Piva And
            '        Contatti.Cod_Contatto Equals Risorse_Umane.Cod_Contatto
            '   Group Join Unita_Misura In GiasContext.UnitaMisura
            '     On Unita_Misura.UDM_COD Equals CI.Udm_Cod Into UnitaMisura_Group = Group
            '   From _Unita_Misura In UnitaMisura_Group.DefaultIfEmpty()
            '   Group Join Qualifiche In GiasContext.Qualifiche
            '     On Qualifiche.Qualifica_Cod Equals CI.Qualifica_Cod Into Qualifica_Group = Group
            '   From _Qualifica_Group In Qualifica_Group.DefaultIfEmpty()
            '   Group Join Attivita In GiasContext.Attivita
            '     On Attivita.ID_Attivita Equals CI.Id_Attivita Into Attivita_Group = Group
            '   From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
            '   Group Join Tariffe In GiasContext.Tariffe
            '     On Tariffe.Tariffa_Cod Equals CI.Tariffa_Cod Into Tariffa_Group = Group
            '   From _Tariffa_Group In Tariffa_Group.DefaultIfEmpty()
            '   Group Join CategorieMagazzino In GiasContext.CategorieMagazzino
            '     On CategorieMagazzino.Elem_Cod Equals CI.Elem_Cod Into CategorieMagazzino_Group = Group
            '   From _CategorieMagazzino In CategorieMagazzino_Group.DefaultIfEmpty()
            '   Where
            '  (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
            '  And (CI.Piva.Equals(piva)) _
            '  And (CI.Id_Agenda = id_agenda_cdg) _
            '  And (CI.Vecchio_Tipo_Inser_Dati = 0) _
            '   And (CI.Flag_Movimento_Campagna = 0)
            '   Select New With {
            '       .Id_CDG = CI.Id_CDG,
            '       .Data_Inserimento = CI.Data_Inserimento,
            '       .Modalita_Imputazione = CI.Modalita_Imputazione,
            '       .Id_Agenda = CI.Id_Agenda,
            '       .Id_Mov = CI.Id_Mov,
            '       .Id_Mov_Det = CI.Id_Mov_Det,
            '       .Mac_Cod = CI.Mac_Cod,
            '       .Cod_Risum = CI.Cod_RisUm,
            '       .Elem_Cod = CI.Elem_Cod,
            '       .Pro_Cod = CI.Pro_Cod,
            '       .Mat_Cod = CI.Mat_Cod,
            '       .ID_Attivita = CI.Id_Attivita,
            '       .Qualifica_Cod = CI.Qualifica_Cod,
            '       .Tariffa_Cod = CI.Tariffa_Cod,
            '       .Turno_Cod = CI.Turno_Cod,
            '       .Conto_Cod = CI.Conto_Cod,
            '       .Lotto = CI.Lotto,
            '       .Mezzo = CI.Mezzo,
            '       .Udm_Cod = CI.Udm_Cod,
            '       .Ora_Inizio = If(CI.Data_Ora_Inizio Is Nothing, AGRODATAINIZIO, CI.Data_Ora_Inizio),
            '       .Ora_Fine = If(CI.Data_Ora_Fine Is Nothing, AGRODATAFINE, CI.Data_Ora_Fine),
            '       .Qta = CI.Qta,
            '       .Ora = AGRODATAINIZIO,
            '       .Prezzo_Unitario = CI.Prezzo_Unitario,
            '       .Prezzo_Totale = CI.Valore_Totale,
            '       .Descrizione = CI.Descrizione,
            '       .Tipo_Ripartizione = CI.Tipo_Ripartizione,
            '       .Budget = CI.Budget,
            '       .Costi_Ricavi = CI.Costi_Ricavi,
            '       .Modalita_Ripartizione = CI.Modalita_Ripartizione,
            '       .Vecchio_Tipo_Inser_Dati = CI.Vecchio_Tipo_Inser_Dati,
            '       .Rag_Soc = "",
            '       .Cognome = Contatti.Cognome,
            '       .Nome = Contatti.Nome,
            '       .Prodotto_Des = Contatti.Cognome & " " & Contatti.Nome,
            '       .Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto,
            '       .Rapporto_Des = Rapporti_Contabili.Rapporto_Des,
            '       .Qualifica_Des = If(_Qualifica_Group Is Nothing, "", _Qualifica_Group.Qualifica_Des),
            '       .Desc = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Desc),
            '       .Tariffa_Des = If(_Tariffa_Group Is Nothing, "", _Tariffa_Group.Tariffa_Des),
            '       .Udm_Des = If(_Unita_Misura Is Nothing, "", _Unita_Misura.UDM_SIM),
            '        .OrigineApp = CI.OrigineApp,
            '       .APP_CDG_Generale_ID = CI.APP_CDG_Generale_ID
            '        }


            '' Compongo la chiave
            'Dim myList_Manodopera = TestataElem_Manodopera.ToList()
            'For Each obj In myList_Manodopera
            '    obj.Rag_Soc = obj.Rag_Soc & "" & obj.Cognome & " " & obj.Nome

            '    'Conversione intero minuti a data
            '    parteIntera = Math.Truncate(obj.Qta)
            '    parteDecimali = obj.Qta - parteIntera
            '    Ora = parteIntera
            '    Minuti = 60 * parteDecimali
            '    obj.Ora = "01/01/1900 " & Ora.ToString("D2") & ":" & Minuti.ToString("D2")

            'Next

            'kendo_Manodopera = JsonConvert.SerializeObject(myList_Manodopera, Formatting.None, serializerSettings)
            'TestataElem_Manodopera = Nothing
            'myList_Manodopera = Nothing


            ''Terzisti
            'Dim TestataElem_Terzisti =
            '     From CI In GiasContext.CDG_Testata
            '     Group Join Fabbricati In GiasContext.Fabbricati
            '     On CI.Piva Equals Fabbricati.PIVA And
            '        CI.Sa_Cod Equals Fabbricati.SA_COD And
            '        CI.Id_Destinazione Equals Fabbricati.Fabbricato_Cod Into Fabbricati_Group = Group
            '     From _Fabbricati_Group In Fabbricati_Group.DefaultIfEmpty()
            '     Group Join Centri_Aziendali In GiasContext.Centri_Aziendali
            '     On Centri_Aziendali.PIVA Equals CI.Piva And
            '        Centri_Aziendali.sa_cod Equals CI.Sa_Cod Into Centri_Aziendali_Group = Group
            '     From _Centri_Aziendali_Group In Centri_Aziendali_Group.DefaultIfEmpty()
            '     Join Materie_Prime In GiasContext.Materie_Prime
            '         On Materie_Prime.Elem_Cod Equals CI.Elem_Cod And
            '            Materie_Prime.Mat_Cod Equals CI.Mat_Cod
            '     Join Unita_Misura In GiasContext.UnitaMisura
            '         On Unita_Misura.UDM_COD Equals CI.Udm_Cod
            '     Group Join Qualifiche In GiasContext.Qualifiche
            '         On Qualifiche.Qualifica_Cod Equals CI.Qualifica_Cod Into Qualifica_Group = Group
            '     From _Qualifica_Group In Qualifica_Group.DefaultIfEmpty()
            '     Group Join Attivita In GiasContext.Attivita
            '         On Attivita.ID_Attivita Equals CI.Id_Attivita Into Attivita_Group = Group
            '     From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
            '     Group Join Tariffe In GiasContext.Tariffe
            '         On Tariffe.Tariffa_Cod Equals CI.Tariffa_Cod Into Tariffa_Group = Group
            '     From _Tariffa_Group In Tariffa_Group.DefaultIfEmpty()
            '     Group Join CategorieMagazzino In GiasContext.CategorieMagazzino
            '         On CategorieMagazzino.Elem_Cod Equals CI.Elem_Cod Into CategorieMagazzino_Group = Group
            '     From _CategorieMagazzino In CategorieMagazzino_Group.DefaultIfEmpty()
            '     Where
            '      (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
            '      And (CI.Piva.Equals(piva)) _
            '      And (CI.Id_Agenda = id_agenda_cdg) _
            '      And (CI.Elem_Cod = CostantiPersonalizzate.CAT_MAG_SERVIZI_PROFESSIONALI) _
            '      And (CI.Vecchio_Tipo_Inser_Dati = 0) _
            '      And (CI.Flag_Movimento_Campagna = 0)
            '     Select New With {
            '           .Id_CDG = CI.Id_CDG,
            '           .Data_Inserimento = CI.Data_Inserimento,
            '           .Modalita_Imputazione = CI.Modalita_Imputazione,
            '           .Id_Agenda = CI.Id_Agenda,
            '           .Id_Mov = CI.Id_Mov,
            '           .Id_Mov_Det = CI.Id_Mov_Det,
            '           .Mac_Cod = CI.Mac_Cod,
            '           .Cod_Risum = CI.Cod_RisUm,
            '           .Elem_Cod = CI.Elem_Cod,
            '           .Pro_Cod = CI.Pro_Cod,
            '           .Mat_Cod = CI.Mat_Cod,
            '           .ID_Attivita = CI.Id_Attivita,
            '           .Qualifica_Cod = CI.Qualifica_Cod,
            '           .Tariffa_Cod = CI.Tariffa_Cod,
            '           .Turno_Cod = CI.Turno_Cod,
            '           .Conto_Cod = CI.Conto_Cod,
            '           .Lotto = CI.Lotto,
            '           .Mezzo = CI.Mezzo,
            '           .Udm_Cod = CI.Udm_Cod,
            '           .Qta = CI.Qta,
            '           .Prezzo_Unitario = CI.Prezzo_Unitario,
            '           .Prezzo_Totale = CI.Valore_Totale,
            '           .Descrizione = CI.Descrizione,
            '           .Tipo_Ripartizione = CI.Tipo_Ripartizione,
            '           .Budget = CI.Budget,
            '           .Costi_Ricavi = CI.Costi_Ricavi,
            '           .Modalita_Ripartizione = CI.Modalita_Ripartizione,
            '           .Vecchio_Tipo_Inser_Dati = CI.Vecchio_Tipo_Inser_Dati,
            '           .Sa_Cod = CI.Sa_Cod,
            '           .Sa_Nome = If(_Centri_Aziendali_Group Is Nothing, "", _Centri_Aziendali_Group.sa_nome),
            '           .Tipo_Destinazione = CI.Tipo_Destinazione,
            '           .Fabbricato_Cod = CI.Id_Destinazione,
            '           .Fabbricato_Des = If(_Fabbricati_Group Is Nothing, "", _Fabbricati_Group.Fabbricato_Des),
            '           .Prodotto_Des = Materie_Prime.Mat_Des,
            '           .Udm_Des = Unita_Misura.UDM_SIM,
            '           .Qualifica_Des = If(_Qualifica_Group Is Nothing, "", _Qualifica_Group.Qualifica_Des),
            '           .Desc = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Desc),
            '           .Tariffa_Des = If(_Tariffa_Group Is Nothing, "", _Tariffa_Group.Tariffa_Des),
            '        .OrigineApp = CI.OrigineApp,
            '       .APP_CDG_Generale_ID = CI.APP_CDG_Generale_ID
            '            }

            ''Imposto la testata se non fatto precedentemente
            'Dim myList_Terzisti = TestataElem_Terzisti.ToList()
            ''For Each obj In myList_Terzisti
            ''    Id_CDG = obj.Id_CDG
            ''    Exit For
            ''Next

            'kendo_Terzisti = JsonConvert.SerializeObject(myList_Terzisti, Formatting.None, serializerSettings)
            'TestataElem_Terzisti = Nothing
            'myList_Terzisti = Nothing

            ''Macchine
            'Dim TestataElem_Macchine =
            '   From CI In GiasContext.CDG_Testata
            '   Join Parco_Macchine In GiasContext.Parco_Macchine
            '     On Parco_Macchine.Mac_Cod Equals CI.Mac_Cod
            '   Group Join Centri_Aziendali In GiasContext.Centri_Aziendali
            '     On Centri_Aziendali.PIVA Equals Parco_Macchine.Piva And
            '        Centri_Aziendali.sa_cod Equals Parco_Macchine.Sa_Cod Into Centri_Aziendali_Group = Group
            '   From _Centri_Aziendali In Centri_Aziendali_Group.DefaultIfEmpty()
            '   Group Join Unita_Misura In GiasContext.UnitaMisura
            '     On Unita_Misura.UDM_COD Equals CI.Udm_Cod Into UnitaMisura_Group = Group
            '   From _Unita_Misura In UnitaMisura_Group.DefaultIfEmpty()
            '   Group Join Qualifiche In GiasContext.Qualifiche
            '     On Qualifiche.Qualifica_Cod Equals CI.Qualifica_Cod Into Qualifica_Group = Group
            '   From _Qualifica_Group In Qualifica_Group.DefaultIfEmpty()
            '   Group Join Attivita In GiasContext.Attivita
            '     On Attivita.ID_Attivita Equals CI.Id_Attivita Into Attivita_Group = Group
            '   From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
            '   Group Join Tariffe In GiasContext.Tariffe
            '     On Tariffe.Tariffa_Cod Equals CI.Tariffa_Cod Into Tariffa_Group = Group
            '   From _Tariffa_Group In Tariffa_Group.DefaultIfEmpty()
            '   Where
            '  (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
            '  And (CI.Piva.Equals(piva)) _
            '  And (CI.Id_Agenda = id_agenda_cdg) _
            '  And (CI.Mac_Cod <> 0) _
            '  And (CI.Vecchio_Tipo_Inser_Dati = 0) _
            '  And (CI.Flag_Movimento_Campagna = 0)
            '   Select New With {
            '       .Id_CDG = CI.Id_CDG,
            '       .Data_Inserimento = CI.Data_Inserimento,
            '       .Modalita_Imputazione = CI.Modalita_Imputazione,
            '       .Id_Agenda = CI.Id_Agenda,
            '       .Id_Mov = CI.Id_Mov,
            '       .Id_Mov_Det = CI.Id_Mov_Det,
            '       .Mac_Cod = CI.Mac_Cod,
            '       .Cod_Risum = CI.Cod_RisUm,
            '       .Elem_Cod = CI.Elem_Cod,
            '       .Pro_Cod = CI.Pro_Cod,
            '       .Mat_Cod = CI.Mat_Cod,
            '       .ID_Attivita = CI.Id_Attivita,
            '       .Qualifica_Cod = CI.Qualifica_Cod,
            '       .Tariffa_Cod = CI.Tariffa_Cod,
            '       .Turno_Cod = CI.Turno_Cod,
            '       .Conto_Cod = CI.Conto_Cod,
            '       .Lotto = CI.Lotto,
            '       .Mezzo = CI.Mezzo,
            '       .Udm_Cod = CI.Udm_Cod,
            '       .Ora_Inizio = If(CI.Data_Ora_Inizio Is Nothing, AGRODATAINIZIO, CI.Data_Ora_Inizio),
            '       .Ora_Fine = If(CI.Data_Ora_Fine Is Nothing, AGRODATAFINE, CI.Data_Ora_Fine),
            '       .Qta = CI.Qta,
            '       .Ora = AGRODATAINIZIO,
            '       .Prezzo_Unitario = CI.Prezzo_Unitario,
            '       .Prezzo_Totale = CI.Valore_Totale,
            '       .Descrizione = CI.Descrizione,
            '       .Tipo_Ripartizione = CI.Tipo_Ripartizione,
            '       .Budget = CI.Budget,
            '       .Costi_Ricavi = CI.Costi_Ricavi,
            '       .Modalita_Ripartizione = CI.Modalita_Ripartizione,
            '       .Vecchio_Tipo_Inser_Dati = CI.Vecchio_Tipo_Inser_Dati,
            '       .Sa_Cod = If(_Centri_Aziendali Is Nothing, 0, _Centri_Aziendali.sa_cod),
            '       .Sa_Nome = If(_Centri_Aziendali Is Nothing, "", _Centri_Aziendali.sa_nome),
            '       .Tipo = Parco_Macchine.Tipo,
            '       .Tipo_Des = If(Parco_Macchine.Tipo = 0, "Agricolo Zootecnico", If(Parco_Macchine.Tipo = 1, "Industriale", "Commerciale")),
            '       .CLASS_CODE_ROOT = Left(Parco_Macchine.Class_Code, 2),
            '       .CLASS_DESC = "",
            '       .Mac_Des = Parco_Macchine.Mac_Des,
            '       .Prodotto_Des = Parco_Macchine.Mac_Des,
            '       .Qualifica_Des = If(_Qualifica_Group Is Nothing, "", _Qualifica_Group.Qualifica_Des),
            '       .Desc = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Desc),
            '       .Tariffa_Des = If(_Tariffa_Group Is Nothing, "", _Tariffa_Group.Tariffa_Des),
            '       .Udm_Des = If(_Unita_Misura Is Nothing, "", _Unita_Misura.UDM_SIM),
            '        .OrigineApp = CI.OrigineApp,
            '       .APP_CDG_Generale_ID = CI.APP_CDG_Generale_ID
            '        }

            ''Imposto la testata se non fatto precedentemente
            'Dim myList_Macchine = TestataElem_Macchine.ToList()
            'For Each obj In myList_Macchine
            '    'Conversione intero minuti a data
            '    parteIntera = Math.Truncate(obj.Qta)
            '    parteDecimali = obj.Qta - parteIntera
            '    Ora = parteIntera
            '    Minuti = 60 * parteDecimali
            '    obj.Ora = "01/01/1900 " & Ora.ToString("D2") & ":" & Minuti.ToString("D2")
            'Next

            'kendo_Macchine = JsonConvert.SerializeObject(myList_Macchine, Formatting.None, serializerSettings)
            'TestataElem_Macchine = Nothing
            'myList_Macchine = Nothing


            'Magazzino
            Dim TestataElem_Magazzino =
                   From CI In GiasContext.CDG_Testata
                   Group Join Fabbricati In GiasContext.Fabbricati
                     On CI.Piva Equals Fabbricati.PIVA And
                        CI.Sa_Cod Equals Fabbricati.SA_COD And
                        CI.Id_Destinazione Equals Fabbricati.Fabbricato_Cod Into Fabbricati_Group = Group
                   From _Fabbricati_Group In Fabbricati_Group.DefaultIfEmpty()
                   Group Join Centri_Aziendali In GiasContext.Centri_Aziendali
                     On Centri_Aziendali.PIVA Equals CI.Piva And
                        Centri_Aziendali.sa_cod Equals CI.Sa_Cod Into Centri_Aziendali_Group = Group
                   From _Centri_Aziendali_Group In Centri_Aziendali_Group.DefaultIfEmpty()
                   Join CategorieMagazzino In GiasContext.CategorieMagazzino
                     On CI.Elem_Cod Equals CategorieMagazzino.Elem_Cod
                   Join Unita_Misura In GiasContext.UnitaMisura
                     On Unita_Misura.UDM_COD Equals CI.Udm_Cod
                   Group Join Materie_Prime In GiasContext.Materie_Prime
                     On Materie_Prime.Elem_Cod Equals CI.Elem_Cod And
                        Materie_Prime.Mat_Cod Equals CI.Mat_Cod Into Materie_Prime_Group = Group
                   From _Materie_Prime_Group In Materie_Prime_Group.DefaultIfEmpty()
                   Group Join Fertilizzanti In GiasContext.Fertilizzanti
                     On CI.Pro_Cod Equals Fertilizzanti.Fer_Cod Into Fertilizzanti_Group = Group
                   From _Fertilizzanti_Group In Fertilizzanti_Group.DefaultIfEmpty()
                   Group Join Formulati In GiasContext.Formulati
                     On CI.Pro_Cod Equals Formulati.Fr_Cod Into Formulati_Group = Group
                   From _Formulati_Group In Formulati_Group.DefaultIfEmpty()
                   Group Join Coadiuvante In GiasContext.Coadiuvante
                     On CI.Pro_Cod Equals Coadiuvante.Coad_Cod Into Coadiuvante_Group = Group
                   From _Coadiuvante_Group In Coadiuvante_Group.DefaultIfEmpty()
                   Group Join InsettiUtili In GiasContext.InsettiUtili
                     On CI.Pro_Cod Equals InsettiUtili.Ins_Cod Into InsettiUtili_Group = Group
                   From _InsettiUtili_Group In InsettiUtili_Group.DefaultIfEmpty()
                   Group Join Trappole In GiasContext.Trappole
                     On CI.Pro_Cod Equals Trappole.TRAP_COD Into Trappole_Group = Group
                   From _Trappole_Group In Trappole_Group.DefaultIfEmpty()
                   Group Join Avversita In GiasContext.Avversita
                     On CI.Pro_Cod Equals Avversita.Av_Cod Into Avversita_Group = Group
                   From _Avversita_Group In Avversita_Group.DefaultIfEmpty()
                   Group Join Farmaci In GiasContext.Farmaci
                     On CI.Pro_Cod Equals Farmaci.Farm_Cod Into Farmaci_Group = Group
                   From _Farmaci_Group In Farmaci_Group.DefaultIfEmpty()
                   Group Join Qualifiche In GiasContext.Qualifiche
                     On Qualifiche.Qualifica_Cod Equals CI.Qualifica_Cod Into Qualifica_Group = Group
                   From _Qualifica_Group In Qualifica_Group.DefaultIfEmpty()
                   Group Join Attivita In GiasContext.Attivita
                     On Attivita.ID_Attivita Equals CI.Id_Attivita Into Attivita_Group = Group
                   From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
                   Group Join Tariffe In GiasContext.Tariffe
                     On Tariffe.Tariffa_Cod Equals CI.Tariffa_Cod Into Tariffa_Group = Group
                   From _Tariffa_Group In Tariffa_Group.DefaultIfEmpty()
                   Where
                  (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                  AndAlso (CI.Piva.Equals(piva)) _
                  AndAlso (CI.Id_Agenda = id_agenda_cdg) _
                  AndAlso (CI.Elem_Cod <> 0) _
                  AndAlso (CI.Elem_Cod <> CostantiPersonalizzate.CAT_MAG_SERVIZI_PROFESSIONALI) _
                  AndAlso (CI.Vecchio_Tipo_Inser_Dati = 0) _
                  AndAlso (CI.Flag_Movimento_Campagna = 0) _
                  AndAlso ((CI.Elem_Cod = CostantiPersonalizzate.FERTILIZZANTI AndAlso _Fertilizzanti_Group.Fer_Cod <> 0) _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.FORMULATI AndAlso _Formulati_Group.Fr_Cod <> 0) _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.COADIUVANTI AndAlso _Coadiuvante_Group.Coad_Cod <> 0) _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.INSETTI AndAlso _InsettiUtili_Group.Ins_Cod <> 0) _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.TRAPPOLE AndAlso _Trappole_Group.TRAP_COD <> 0) _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.INNESCHI AndAlso _Avversita_Group.Av_Cod <> 0) _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.FARMACI AndAlso _Farmaci_Group.Farm_Cod <> 0) _
                    OrElse (CI.Mat_Cod <> 0 AndAlso _Materie_Prime_Group.Mat_Cod <> 0))
                   Select New With {
                       .Id_CDG = CI.Id_CDG,
                       .Data_Inserimento = CI.Data_Inserimento,
                       .Modalita_Imputazione = CI.Modalita_Imputazione,
                       .Id_Agenda = CI.Id_Agenda,
                       .Id_Mov = CI.Id_Mov,
                       .Id_Mov_Det = CI.Id_Mov_Det,
                       .Mac_Cod = CI.Mac_Cod,
                       .Cod_Risum = CI.Cod_RisUm,
                       .Elem_Cod = CI.Elem_Cod,
                       .Pro_Cod = CI.Pro_Cod,
                       .Mat_Cod = CI.Mat_Cod,
                       .Lav_Cod = CI.Lav_Cod,
                       .ID_Attivita = CI.Id_Attivita,
                       .Qualifica_Cod = CI.Qualifica_Cod,
                       .Tariffa_Cod = CI.Tariffa_Cod,
                       .Turno_Cod = CI.Turno_Cod,
                       .Conto_Cod = CI.Conto_Cod,
                       .Lotto = CI.Lotto,
                       .Mezzo = CI.Mezzo,
                       .Udm_Cod = CI.Udm_Cod,
                       .Qta = CI.Qta,
                       .Prezzo_Unitario = CI.Prezzo_Unitario,
                       .Prezzo_Totale = CI.Valore_Totale,
                       .Descrizione = CI.Descrizione,
                       .Tipo_Ripartizione = CI.Tipo_Ripartizione,
                       .Budget = CI.Budget,
                       .Costi_Ricavi = CI.Costi_Ricavi,
                       .Modalita_Ripartizione = CI.Modalita_Ripartizione,
                       .Vecchio_Tipo_Inser_Dati = CI.Vecchio_Tipo_Inser_Dati,
                       .Sa_Cod = CI.Sa_Cod,
                       .Sa_Nome = If(_Centri_Aziendali_Group Is Nothing, "", _Centri_Aziendali_Group.sa_nome),
                       .Tipo_Destinazione = CI.Tipo_Destinazione,
                       .Fabbricato_Cod = CI.Id_Destinazione,
                       .Fabbricato_Des = If(_Fabbricati_Group Is Nothing, "", _Fabbricati_Group.Fabbricato_Des),
                       .NomeComune = CategorieMagazzino.NomeComune,
                       .Prodotto_Cod = If(CI.Pro_Cod <> 0, CI.Pro_Cod, CI.Mat_Cod),
                       .Prodotto_Des = "",
                       .Mat_Des = If(_Materie_Prime_Group Is Nothing, "", _Materie_Prime_Group.Mat_Des),
                       .Fer_Des = If(_Fertilizzanti_Group Is Nothing, "", _Fertilizzanti_Group.Fer_Des),
                       .Fr_Des = If(_Formulati_Group Is Nothing, "", _Formulati_Group.Fr_Des),
                       .Coad_Des = If(_Coadiuvante_Group Is Nothing, "", _Coadiuvante_Group.Coad_Des),
                       .Ins_Des = If(_InsettiUtili_Group Is Nothing, "", _InsettiUtili_Group.Ins_Des),
                       .Trap_Des = If(_Trappole_Group Is Nothing, "", _Trappole_Group.TRAP_DES),
                       .Farm_Des = If(_Farmaci_Group Is Nothing, "", _Farmaci_Group.Denominazione & " " & _Farmaci_Group.Confezione),
                       .Av_Des_Vol = If(_Avversita_Group Is Nothing, "", _Avversita_Group.Av_Des_Vol),
                       .Udm_Des = Unita_Misura.UDM_SIM,
                       .Qualifica_Des = If(_Qualifica_Group Is Nothing, "", _Qualifica_Group.Qualifica_Des),
                       .Desc = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Sigla & " - " & _Attivita_Group.Desc),
                       .Tariffa_Des = If(_Tariffa_Group Is Nothing, "", _Tariffa_Group.Tariffa_Des),
                        .OrigineApp = CI.OrigineApp,
                       .APP_CDG_Generale_ID = CI.APP_CDG_Generale_ID
                        }

            'Imposto la testata se non fatto precedentemente
            Dim myList_Magazzino = TestataElem_Magazzino.ToList()
            For Each obj In myList_Magazzino
                Select Case obj.Elem_Cod
                    Case CostantiPersonalizzate.FERTILIZZANTI
                        obj.Prodotto_Des = obj.Fer_Des
                    Case CostantiPersonalizzate.FORMULATI
                        obj.Prodotto_Des = obj.Fr_Des
                    Case CostantiPersonalizzate.COADIUVANTI
                        obj.Prodotto_Des = obj.Coad_Des
                    Case CostantiPersonalizzate.INSETTI
                        obj.Prodotto_Des = obj.Ins_Des
                    Case CostantiPersonalizzate.TRAPPOLE
                        obj.Prodotto_Des = obj.Trap_Des
                    Case CostantiPersonalizzate.INNESCHI
                        obj.Prodotto_Des = obj.Av_Des_Vol
                    Case CostantiPersonalizzate.FARMACI
                        obj.Prodotto_Des = obj.Farm_Des
                    Case Else
                        obj.Prodotto_Des = obj.Mat_Des
                End Select
            Next

            kendo_Magazzino = JsonConvert.SerializeObject(myList_Magazzino, Formatting.None, serializerSettings)
            TestataElem_Magazzino = Nothing
            myList_Magazzino = Nothing



            If Not bypass_Kendo_Libera Then


                'Libero
                Dim TestataElem_Libera =
                   From CI In GiasContext.CDG_Testata
                   Join Unita_Misura In GiasContext.UnitaMisura
                     On Unita_Misura.UDM_COD Equals CI.Udm_Cod
                   Group Join Attivita In GiasContext.Attivita
                     On Attivita.ID_Attivita Equals CI.Id_Attivita Into Attivita_Group = Group
                   From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
                   Where
                  (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                  AndAlso (CI.Piva.Equals(piva)) _
                  AndAlso (CI.Elem_Cod = 0) _
                  AndAlso (CI.Mac_Cod = 0) _
                  AndAlso (CI.Cod_RisUm = 0) _
                  AndAlso (CI.Id_Agenda = id_agenda_cdg) _
                  AndAlso (CI.Vecchio_Tipo_Inser_Dati = 0) _
                  AndAlso (CI.Flag_Movimento_Campagna = 0)
                   Select New With {
                       .Id_CDG = CI.Id_CDG,
                       .Data_Inserimento = CI.Data_Inserimento,
                       .Modalita_Imputazione = CI.Modalita_Imputazione,
                       .Id_Agenda = CI.Id_Agenda,
                       .Id_Mov = CI.Id_Mov,
                       .Id_Mov_Det = CI.Id_Mov_Det,
                       .Mac_Cod = CI.Mac_Cod,
                       .Cod_Risum = CI.Cod_RisUm,
                       .Elem_Cod = CI.Elem_Cod,
                       .Pro_Cod = CI.Pro_Cod,
                       .Mat_Cod = CI.Mat_Cod,
                       .Lav_Cod = CI.Lav_Cod,
                       .ID_Attivita = CI.Id_Attivita,
                       .Qualifica_Cod = CI.Qualifica_Cod,
                       .Tariffa_Cod = CI.Tariffa_Cod,
                       .Turno_Cod = CI.Turno_Cod,
                       .Conto_Cod = CI.Conto_Cod,
                       .Lotto = CI.Lotto,
                       .Mezzo = CI.Mezzo,
                       .Udm_Cod = CI.Udm_Cod,
                       .Qta = CI.Qta,
                       .Prezzo_Unitario = CI.Prezzo_Unitario,
                       .Prezzo_Totale = CI.Valore_Totale,
                       .Prodotto_Des = CI.Descrizione,
                       .Tipo_Ripartizione = CI.Tipo_Ripartizione,
                       .Budget = CI.Budget,
                       .Costi_Ricavi = CI.Costi_Ricavi,
                       .Modalita_Ripartizione = CI.Modalita_Ripartizione,
                       .Vecchio_Tipo_Inser_Dati = CI.Vecchio_Tipo_Inser_Dati,
                       .Udm_Des = Unita_Misura.UDM_SIM,
                       .Desc = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Sigla & " - " & _Attivita_Group.Desc),
                        .OrigineApp = CI.OrigineApp,
                       .APP_CDG_Generale_ID = CI.APP_CDG_Generale_ID
                        }

                'Imposto la testata se non fatto precedentemente
                Dim myList_Libera = TestataElem_Libera.ToList()
                'For Each obj In myList_Libera
                '    Id_CDG = obj.Id_CDG
                '    Exit For
                'Next

                kendo_Libera = JsonConvert.SerializeObject(myList_Libera, Formatting.None, serializerSettings)
                TestataElem_Libera = Nothing
                myList_Libera = Nothing

            End If

        End If



        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If


    End Sub



    '##############################################################################################
    'Eredita
    Public Sub Leggi_CDG_Testate_Parte3(ByVal piva As String,
                                        ByVal id_agenda As Integer,
                                        ByVal id_mov_det As Integer,
                                        ByVal id_agenda_cdg As Integer,
                                        ByVal Automatico As Integer,
                                        ByVal Id_Attivita_Eredita As Integer,
                                        ByVal Attivita_Des_Eredita As String,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByRef kendo_Eredita As String,
                                        Optional ByVal Raccoglitore_Cod As Integer = 0,
                                        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing)

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_CDG_Testate_Parte3()"

        'Dim gefutils AS New Gias_EF_Utility
        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim strCau_Mov As String() = {CAU_SCARICO}

        'Dim strElem_Cod_Fertilizzanti AS Integer() = {3}

        Dim bValido As Boolean = False

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}


        '######################################################################################################################
        '############################################ TESTATA #################################################################
        '######################################################################################################################


        'Eredita
        Select Case Automatico

            Case 0

                'Impostazione Manuale
                If id_agenda_cdg <> 0 Then

                    'Lettura da Agenda Collegata
                    Dim bFiltroId_Mov_Det As Boolean
                    Select Case id_mov_det
                        Case 0
                            bFiltroId_Mov_Det = False
                        Case Else 'Contab
                            bFiltroId_Mov_Det = True
                    End Select


                    'Eredita come Magazzino
                    Dim TestataElem_Eredita =
                   From CI In GiasContext.CDG_Testata
                   Group Join Fabbricati In GiasContext.Fabbricati
                     On CI.Piva Equals Fabbricati.PIVA And
                        CI.Sa_Cod Equals Fabbricati.SA_COD And
                        CI.Id_Destinazione Equals Fabbricati.Fabbricato_Cod Into Fabbricati_Group = Group
                   From _Fabbricati_Group In Fabbricati_Group.DefaultIfEmpty()
                   Group Join Centri_Aziendali In GiasContext.Centri_Aziendali
                     On Centri_Aziendali.PIVA Equals CI.Piva And
                        Centri_Aziendali.sa_cod Equals CI.Sa_Cod Into Centri_Aziendali_Group = Group
                   From _Centri_Aziendali_Group In Centri_Aziendali_Group.DefaultIfEmpty()
                   Group Join CategorieMagazzino In GiasContext.CategorieMagazzino
                         On CI.Elem_Cod Equals CategorieMagazzino.Elem_Cod Into CategorieMagazzino_Group = Group
                   From _CategorieMagazzino_Group In CategorieMagazzino_Group.DefaultIfEmpty()
                   Group Join Unita_Misura In GiasContext.UnitaMisura
                         On Unita_Misura.UDM_COD Equals CI.Udm_Cod Into Unita_Misura_Group = Group
                   From _Unita_Misura_Group In Unita_Misura_Group.DefaultIfEmpty()
                   Group Join Materie_Prime In GiasContext.Materie_Prime
                     On Materie_Prime.Elem_Cod Equals CI.Elem_Cod And
                        Materie_Prime.Mat_Cod Equals CI.Mat_Cod Into Materie_Prime_Group = Group
                   From _Materie_Prime_Group In Materie_Prime_Group.DefaultIfEmpty()
                   Group Join Fertilizzanti In GiasContext.Fertilizzanti
                     On CI.Pro_Cod Equals Fertilizzanti.Fer_Cod Into Fertilizzanti_Group = Group
                   From _Fertilizzanti_Group In Fertilizzanti_Group.DefaultIfEmpty()
                   Group Join Formulati In GiasContext.Formulati
                     On CI.Pro_Cod Equals Formulati.Fr_Cod Into Formulati_Group = Group
                   From _Formulati_Group In Formulati_Group.DefaultIfEmpty()
                   Group Join Coadiuvante In GiasContext.Coadiuvante
                     On CI.Pro_Cod Equals Coadiuvante.Coad_Cod Into Coadiuvante_Group = Group
                   From _Coadiuvante_Group In Coadiuvante_Group.DefaultIfEmpty()
                   Group Join InsettiUtili In GiasContext.InsettiUtili
                     On CI.Pro_Cod Equals InsettiUtili.Ins_Cod Into InsettiUtili_Group = Group
                   From _InsettiUtili_Group In InsettiUtili_Group.DefaultIfEmpty()
                   Group Join Trappole In GiasContext.Trappole
                     On CI.Pro_Cod Equals Trappole.TRAP_COD Into Trappole_Group = Group
                   From _Trappole_Group In Trappole_Group.DefaultIfEmpty()
                   Group Join Farmaci In GiasContext.Farmaci
                     On CI.Pro_Cod Equals Farmaci.Farm_Cod Into Farmaci_Group = Group
                   From _Farmaci_Group In Farmaci_Group.DefaultIfEmpty()
                   Group Join Avversita In GiasContext.Avversita
                         On CI.Pro_Cod Equals Avversita.Av_Cod Into Avversita_Group = Group
                   From _Avversita_Group In Avversita_Group.DefaultIfEmpty()
                   Group Join Categorie In GiasContext.Categorie.Where(Function(x) x.PADRE = "S000029")
                         On Right(Categorie.COD, 3) Equals CI.Pro_Cod Into Categorie_Group = Group
                   From _Categorie_Group In Categorie_Group.DefaultIfEmpty()
                   Group Join Qualifiche In GiasContext.Qualifiche
                     On Qualifiche.Qualifica_Cod Equals CI.Qualifica_Cod Into Qualifica_Group = Group
                   From _Qualifica_Group In Qualifica_Group.DefaultIfEmpty()
                   Group Join Attivita In GiasContext.Attivita
                     On Attivita.ID_Attivita Equals CI.Id_Attivita Into Attivita_Group = Group
                   From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
                   Group Join Tariffe In GiasContext.Tariffe
                     On Tariffe.Tariffa_Cod Equals CI.Tariffa_Cod Into Tariffa_Group = Group
                   From _Tariffa_Group In Tariffa_Group.DefaultIfEmpty()
                   Where
                  (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                  AndAlso (CI.Piva.Equals(piva)) _
                  AndAlso (CI.Id_Agenda = id_agenda_cdg) _
                  AndAlso (CI.Elem_Cod <> 0) _
                  AndAlso ((bFiltroId_Mov_Det) OrElse CI.Elem_Cod <> CostantiPersonalizzate.CAT_MAG_SERVIZI_PROFESSIONALI) _
                  AndAlso ((bFiltroId_Mov_Det) OrElse CI.Elem_Cod <> CostantiPersonalizzate.SERVIZI) _
                  AndAlso (CI.Vecchio_Tipo_Inser_Dati = 0) _
                  AndAlso ((bFiltroId_Mov_Det AndAlso CI.Modalita_Imputazione = 4) OrElse CI.Flag_Movimento_Campagna = 1) _
                  AndAlso ((CI.Elem_Cod = CostantiPersonalizzate.FERTILIZZANTI AndAlso _Fertilizzanti_Group.Fer_Cod <> 0) _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.FORMULATI AndAlso _Formulati_Group.Fr_Cod <> 0) _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.COADIUVANTI AndAlso _Coadiuvante_Group.Coad_Cod <> 0) _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.INSETTI AndAlso _InsettiUtili_Group.Ins_Cod <> 0) _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.TRAPPOLE AndAlso _Trappole_Group.TRAP_COD <> 0) _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.INNESCHI AndAlso _Avversita_Group.Av_Cod <> 0) _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.SERVIZI AndAlso Left(_Categorie_Group.COD, 1) = "S") _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.FARMACI AndAlso _Farmaci_Group.Farm_Cod <> 0) _
                    OrElse (CI.Mat_Cod <> 0 AndAlso _Materie_Prime_Group.Mat_Cod <> 0) _
                    OrElse (CI.Elem_Cod = CostantiPersonalizzate.ALTRI_BENI AndAlso CI.Pro_Cod = 0 AndAlso CI.Mat_Cod = 0))
                   Select New With {
                       .Id_CDG = CI.Id_CDG,
                       .Data_Inserimento = CI.Data_Inserimento,
                       .Modalita_Imputazione = CI.Modalita_Imputazione,
                       .Id_Agenda = CI.Id_Agenda,
                       .Id_Mov = CI.Id_Mov,
                       .Id_Mov_Det = CI.Id_Mov_Det,
                       .Mac_Cod = CI.Mac_Cod,
                       .Cod_Risum = CI.Cod_RisUm,
                       .Elem_Cod = CI.Elem_Cod,
                       .Pro_Cod = CI.Pro_Cod,
                       .Mat_Cod = CI.Mat_Cod,
                       .Lav_Cod = CI.Lav_Cod,
                       .ID_Attivita = CI.Id_Attivita,
                       .Qualifica_Cod = CI.Qualifica_Cod,
                       .Tariffa_Cod = CI.Tariffa_Cod,
                       .Turno_Cod = CI.Turno_Cod,
                       .Conto_Cod = CI.Conto_Cod,
                       .Lotto = CI.Lotto,
                       .Mezzo = CI.Mezzo,
                       .Udm_Cod = CI.Udm_Cod,
                       .Qta = CI.Qta,
                       .Prezzo_Unitario = CI.Prezzo_Unitario,
                       .Prezzo_Totale = CI.Valore_Totale,
                       .Descrizione = CI.Descrizione,
                       .Tipo_Ripartizione = CI.Tipo_Ripartizione,
                       .Budget = CI.Budget,
                       .Costi_Ricavi = CI.Costi_Ricavi,
                       .Modalita_Ripartizione = CI.Modalita_Ripartizione,
                       .Vecchio_Tipo_Inser_Dati = CI.Vecchio_Tipo_Inser_Dati,
                       .Sa_Cod = CI.Sa_Cod,
                       .Sa_Nome = If(_Centri_Aziendali_Group Is Nothing, "", _Centri_Aziendali_Group.sa_nome),
                       .Tipo_Destinazione = CI.Tipo_Destinazione,
                       .Fabbricato_Cod = CI.Id_Destinazione,
                       .Fabbricato_Des = If(_Fabbricati_Group Is Nothing, "", _Fabbricati_Group.Fabbricato_Des),
                       .NomeComune = If(_CategorieMagazzino_Group Is Nothing, "", _CategorieMagazzino_Group.NomeComune),
                       .Prodotto_Cod = If(CI.Pro_Cod <> 0, CI.Pro_Cod, CI.Mat_Cod),
                       .Prodotto_Des = "",
                       .Mat_Des = If(_Materie_Prime_Group Is Nothing, "", _Materie_Prime_Group.Mat_Des),
                       .Fer_Des = If(_Fertilizzanti_Group Is Nothing, "", _Fertilizzanti_Group.Fer_Des),
                       .Fr_Des = If(_Formulati_Group Is Nothing, "", _Formulati_Group.Fr_Des),
                       .Coad_Des = If(_Coadiuvante_Group Is Nothing, "", _Coadiuvante_Group.Coad_Des),
                       .Ins_Des = If(_InsettiUtili_Group Is Nothing, "", _InsettiUtili_Group.Ins_Des),
                       .Trap_Des = If(_Trappole_Group Is Nothing, "", _Trappole_Group.TRAP_DES),
                       .Farm_Des = If(_Farmaci_Group Is Nothing, "", _Farmaci_Group.Denominazione & " " & _Farmaci_Group.Confezione),
                       .Av_Des_Vol = If(_Avversita_Group Is Nothing, "", _Avversita_Group.Av_Des_Vol),
                       .Udm_Des = If(_Unita_Misura_Group Is Nothing, "", _Unita_Misura_Group.UDM_SIM),
                       .Qualifica_Des = If(_Qualifica_Group Is Nothing, "", _Qualifica_Group.Qualifica_Des),
                       .Desc = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Sigla & " - " & _Attivita_Group.Desc),
                       .Tariffa_Des = If(_Tariffa_Group Is Nothing, "", _Tariffa_Group.Tariffa_Des),
                        .OrigineApp = CI.OrigineApp,
                       .APP_CDG_Generale_ID = CI.APP_CDG_Generale_ID
                        }

                    'Imposto la testata se non fatto precedentemente
                    Dim myList_Eredita = TestataElem_Eredita.Distinct().ToList()
                    For Each obj In myList_Eredita
                        Select Case obj.Elem_Cod
                            Case CostantiPersonalizzate.FERTILIZZANTI
                                obj.Prodotto_Des = obj.Fer_Des
                            Case CostantiPersonalizzate.FORMULATI
                                obj.Prodotto_Des = obj.Fr_Des
                            Case CostantiPersonalizzate.COADIUVANTI
                                obj.Prodotto_Des = obj.Coad_Des
                            Case CostantiPersonalizzate.INSETTI
                                obj.Prodotto_Des = obj.Ins_Des
                            Case CostantiPersonalizzate.TRAPPOLE
                                obj.Prodotto_Des = obj.Trap_Des
                            Case CostantiPersonalizzate.INNESCHI
                                obj.Prodotto_Des = obj.Av_Des_Vol
                            Case CostantiPersonalizzate.FARMACI
                                obj.Prodotto_Des = obj.Farm_Des
                            Case CostantiPersonalizzate.ALTRI_BENI
                                obj.Prodotto_Des = obj.Descrizione
                                obj.NomeComune = "Altri Beni Strumentali"
                            Case CostantiPersonalizzate.SERVIZI
                                obj.Prodotto_Des = obj.Descrizione
                                obj.NomeComune = "Servizi"
                            Case Else
                                obj.Prodotto_Des = obj.Mat_Des
                        End Select
                    Next

                    kendo_Eredita = JsonConvert.SerializeObject(myList_Eredita, Formatting.None, serializerSettings)
                    TestataElem_Eredita = Nothing
                    myList_Eredita = Nothing

                End If

            Case 1



                If id_agenda <> 0 Then

                    'Lettura da Agenda Collegata
                    Dim bFiltroId_Mov_Det As Boolean

                    Select Case Raccoglitore_Cod

                        Case 0

                            Select Case id_mov_det
                                Case 0
                                    bFiltroId_Mov_Det = False
                                Case Else 'Contab
                                    bFiltroId_Mov_Det = True
                                    strCau_Mov = {CAU_SCARICO, CAU_CARICO}
                            End Select

                        Case Else

                            bFiltroId_Mov_Det = False

                    End Select




                    Dim TestataElem_Eredita =
                       From CI In GiasContext.Movimenti_dettagli
                       Join Movimenti In GiasContext.Movimenti
                         On CI.PIVA Equals Movimenti.PIVA And
                            CI.Id_Agenda Equals Movimenti.Id_Agenda And
                            CI.Id_Mov Equals Movimenti.Id_Mov
                       Join Agenda In GiasContext.Agenda
                         On Agenda.PIVA Equals Movimenti.PIVA And
                            Agenda.Id_Agenda Equals Movimenti.Id_Agenda
                       Group Join Mov_Destinazioni In GiasContext.Mov_Destinazioni
                         On CI.PIVA Equals Mov_Destinazioni.Piva And
                            CI.Id_Agenda Equals Mov_Destinazioni.Id_Agenda And
                            CI.Id_Mov Equals Mov_Destinazioni.Id_Mov And
                            CI.Id_Mov_Det Equals Mov_Destinazioni.Id_Mov_Det Into Mov_Destinazioni_Group = Group
                       From _Mov_Destinazioni_Group In Mov_Destinazioni_Group.DefaultIfEmpty()
                       Group Join Fabbricati In GiasContext.Fabbricati
                         On _Mov_Destinazioni_Group.Piva Equals Fabbricati.PIVA And
                            _Mov_Destinazioni_Group.Sa_Cod Equals Fabbricati.SA_COD And
                            _Mov_Destinazioni_Group.Id_Destinazione Equals Fabbricati.Fabbricato_Cod Into Fabbricati_Group = Group
                       From _Fabbricati_Group In Fabbricati_Group.DefaultIfEmpty()
                       Group Join Centri_Aziendali In GiasContext.Centri_Aziendali
                         On Centri_Aziendali.PIVA Equals CI.PIVA And
                            Centri_Aziendali.sa_cod Equals CI.Sa_Cod Into Centri_Aziendali_Group = Group
                       From _Centri_Aziendali_Group In Centri_Aziendali_Group.DefaultIfEmpty()
                       Group Join CategorieMagazzino In GiasContext.CategorieMagazzino
                         On CI.Elem_Cod Equals CategorieMagazzino.Elem_Cod Into CategorieMagazzino_Group = Group
                       From _CategorieMagazzino_Group In CategorieMagazzino_Group.DefaultIfEmpty()
                       Group Join Unita_Misura In GiasContext.UnitaMisura
                         On Unita_Misura.UDM_COD Equals CI.Udm_Cod Into Unita_Misura_Group = Group
                       From _Unita_Misura_Group In Unita_Misura_Group.DefaultIfEmpty()
                       Group Join Materie_Prime In GiasContext.Materie_Prime
                         On Materie_Prime.Elem_Cod Equals CI.Elem_Cod And
                            Materie_Prime.Mat_Cod Equals CI.Mat_Cod Into Materie_Prime_Group = Group
                       From _Materie_Prime_Group In Materie_Prime_Group.DefaultIfEmpty()
                       Group Join Fertilizzanti In GiasContext.Fertilizzanti
                         On CI.Pro_Cod Equals Fertilizzanti.Fer_Cod Into Fertilizzanti_Group = Group
                       From _Fertilizzanti_Group In Fertilizzanti_Group.DefaultIfEmpty()
                       Group Join Formulati In GiasContext.Formulati
                         On CI.Pro_Cod Equals Formulati.Fr_Cod Into Formulati_Group = Group
                       From _Formulati_Group In Formulati_Group.DefaultIfEmpty()
                       Group Join Coadiuvante In GiasContext.Coadiuvante
                         On CI.Pro_Cod Equals Coadiuvante.Coad_Cod Into Coadiuvante_Group = Group
                       From _Coadiuvante_Group In Coadiuvante_Group.DefaultIfEmpty()
                       Group Join InsettiUtili In GiasContext.InsettiUtili
                         On CI.Pro_Cod Equals InsettiUtili.Ins_Cod Into InsettiUtili_Group = Group
                       From _InsettiUtili_Group In InsettiUtili_Group.DefaultIfEmpty()
                       Group Join Trappole In GiasContext.Trappole
                         On CI.Pro_Cod Equals Trappole.TRAP_COD Into Trappole_Group = Group
                       From _Trappole_Group In Trappole_Group.DefaultIfEmpty()
                       Group Join Farmaci In GiasContext.Farmaci
                         On CI.Pro_Cod Equals Farmaci.Farm_Cod Into Farmaci_Group = Group
                       From _Farmaci_Group In Farmaci_Group.DefaultIfEmpty()
                       Group Join Avversita In GiasContext.Avversita
                         On CI.Pro_Cod Equals Avversita.Av_Cod Into Avversita_Group = Group
                       From _Avversita_Group In Avversita_Group.DefaultIfEmpty()
                       Group Join Categorie In GiasContext.Categorie.Where(Function(x) x.PADRE = "S000029")
                         On Right(Categorie.COD, 3) Equals CI.Pro_Cod Into Categorie_Group = Group
                       From _Categorie_Group In Categorie_Group.DefaultIfEmpty()
                       Group Join Qualifiche In GiasContext.Qualifiche
                         On Qualifiche.Qualifica_Cod Equals CI.Qualifica_Cod Into Qualifica_Group = Group
                       From _Qualifica_Group In Qualifica_Group.DefaultIfEmpty()
                       Group Join Attivita In GiasContext.Attivita
                         On Attivita.ID_Attivita Equals CI.ID_Attivita Into Attivita_Group = Group
                       From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
                       Group Join Tariffe In GiasContext.Tariffe
                         On Tariffe.Tariffa_Cod Equals CI.Tariffa_Cod Into Tariffa_Group = Group
                       From _Tariffa_Group In Tariffa_Group.DefaultIfEmpty()
                       Where
                          (Agenda.PIVA.Equals(piva)) _
                      AndAlso ((Not bFiltroId_Mov_Det) OrElse CI.Id_Mov_Det = id_mov_det) _
                      AndAlso (CI.Elem_Cod <> 0) _
                      AndAlso ((bFiltroId_Mov_Det) OrElse CI.Elem_Cod <> CostantiPersonalizzate.CAT_MAG_SERVIZI_PROFESSIONALI) _
                      AndAlso ((bFiltroId_Mov_Det) OrElse CI.Elem_Cod <> CostantiPersonalizzate.SERVIZI) _
                      AndAlso strCau_Mov.Contains(Movimenti.Cau_Mov) _
                      AndAlso ((CI.Elem_Cod = CostantiPersonalizzate.FERTILIZZANTI AndAlso _Fertilizzanti_Group.Fer_Cod <> 0) _
                        OrElse (CI.Elem_Cod = CostantiPersonalizzate.FORMULATI AndAlso _Formulati_Group.Fr_Cod <> 0) _
                        OrElse (CI.Elem_Cod = CostantiPersonalizzate.COADIUVANTI AndAlso _Coadiuvante_Group.Coad_Cod <> 0) _
                        OrElse (CI.Elem_Cod = CostantiPersonalizzate.INSETTI AndAlso _InsettiUtili_Group.Ins_Cod <> 0) _
                        OrElse (CI.Elem_Cod = CostantiPersonalizzate.TRAPPOLE AndAlso _Trappole_Group.TRAP_COD <> 0) _
                        OrElse (CI.Elem_Cod = CostantiPersonalizzate.INNESCHI AndAlso _Avversita_Group.Av_Cod <> 0) _
                        OrElse (CI.Elem_Cod = CostantiPersonalizzate.SERVIZI AndAlso Left(_Categorie_Group.COD, 1) = "S") _
                        OrElse (CI.Elem_Cod = CostantiPersonalizzate.FARMACI AndAlso _Farmaci_Group.Farm_Cod <> 0) _
                        OrElse (CI.Mat_Cod <> 0 AndAlso _Materie_Prime_Group.Mat_Cod <> 0) _
                        OrElse (CI.Elem_Cod = CostantiPersonalizzate.ALTRI_BENI AndAlso CI.Pro_Cod = 0 AndAlso CI.Mat_Cod = 0))
                       Select New With {
                           .Id_CDG = 0,
                           .Flag_Movimento_Campagna = 1,
                           .Data_Inserimento = Movimenti.Data_Movimento,
                           .Modalita_Imputazione = 0,
                           .Id_Agenda = Agenda.Id_Agenda,
                           .Raccoglitore_Cod = Agenda.Raccoglitore_Cod,
                           .Lav_Cod = Agenda.Lav_Cod,
                           .Id_Mov = Movimenti.Id_Mov,
                           .Id_Mov_Det = CI.Id_Mov_Det,
                           .Mac_Cod = 0,
                           .Cod_Risum = 0,
                           .Elem_Cod = CI.Elem_Cod,
                           .Pro_Cod = CI.Pro_Cod,
                           .Mat_Cod = CI.Mat_Cod,
                           .ID_Attivita = CI.ID_Attivita,
                           .Qualifica_Cod = CI.Qualifica_Cod,
                           .Tariffa_Cod = CI.Tariffa_Cod,
                           .Turno_Cod = CI.Turno_Cod,
                           .Conto_Cod = CI.Cod_Conto,
                           .Lotto = CI.Lotto,
                           .Mezzo = CI.Mezzo_Det,
                           .Mov_Det_Des = CI.Mov_Det_Des,
                           .Udm_Cod = CI.Udm_Cod,
                           .Qta = CI.Qta,
                           .Prezzo_Unitario = CI.Prezzo_Unitario,
                           .Prezzo_Totale = If(CI.Imponibile_Netto > 0, CI.Imponibile_Netto, -CI.Imponibile_Netto),
                           .Descrizione = "",
                           .Tipo_Ripartizione = 0,
                           .Budget = 0,
                           .Costi_Ricavi = 0,
                           .Modalita_Ripartizione = 0,
                           .Vecchio_Tipo_Inser_Dati = 0,
                           .Sa_Cod = CI.Sa_Cod,
                           .Sa_Nome = If(_Centri_Aziendali_Group Is Nothing, "", _Centri_Aziendali_Group.sa_nome),
                           .Tipo_Destinazione = If(_Mov_Destinazioni_Group Is Nothing, 0, _Mov_Destinazioni_Group.Tipo_Destinazione),
                           .Fabbricato_Cod = If(_Mov_Destinazioni_Group Is Nothing, 0, _Mov_Destinazioni_Group.Id_Destinazione),
                           .Fabbricato_Des = If(_Fabbricati_Group Is Nothing, "", _Fabbricati_Group.Fabbricato_Des),
                           .NomeComune = If(_CategorieMagazzino_Group Is Nothing, "", _CategorieMagazzino_Group.NomeComune),
                           .Prodotto_Cod = If(CI.Pro_Cod <> 0, CI.Pro_Cod, CI.Mat_Cod),
                           .Prodotto_Des = CI.Mov_Det_Des,
                           .Mat_Des = If(_Materie_Prime_Group Is Nothing, "", _Materie_Prime_Group.Mat_Des),
                           .Fer_Des = If(_Fertilizzanti_Group Is Nothing, "", _Fertilizzanti_Group.Fer_Des),
                           .Fr_Des = If(_Formulati_Group Is Nothing, "", _Formulati_Group.Fr_Des),
                           .Coad_Des = If(_Coadiuvante_Group Is Nothing, "", _Coadiuvante_Group.Coad_Des),
                           .Ins_Des = If(_InsettiUtili_Group Is Nothing, "", _InsettiUtili_Group.Ins_Des),
                           .Trap_Des = If(_Trappole_Group Is Nothing, "", _Trappole_Group.TRAP_DES),
                           .Farm_Des = If(_Farmaci_Group Is Nothing, "", _Farmaci_Group.Denominazione & " " & _Farmaci_Group.Confezione),
                           .Av_Des_Vol = If(_Avversita_Group Is Nothing, "", _Avversita_Group.Av_Des_Vol),
                           .Servizio_Des = If(_Categorie_Group Is Nothing, "", _Categorie_Group.DESCR),
                           .Udm_Des = If(_Unita_Misura_Group Is Nothing, "", _Unita_Misura_Group.UDM_SIM),
                           .Qualifica_Des = If(_Qualifica_Group Is Nothing, "", _Qualifica_Group.Qualifica_Des),
                           .Desc = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Sigla & " - " & _Attivita_Group.Desc),
                           .Tariffa_Des = If(_Tariffa_Group Is Nothing, "", _Tariffa_Group.Tariffa_Des)
                            }


                    'Filtro
                    Select Case Raccoglitore_Cod

                        Case 0

                            TestataElem_Eredita = TestataElem_Eredita.Where(Function(x) x.Id_Agenda = id_agenda)

                        Case Else

                            TestataElem_Eredita = TestataElem_Eredita.Where(Function(x) x.Raccoglitore_Cod = Raccoglitore_Cod)

                    End Select

                    'Imposto la testata se non fatto precedentemente
                    Dim myList_Eredita = TestataElem_Eredita.ToList()
                    For Each obj In myList_Eredita
                        'obj.Prezzo_Totale = obj.Qta * obj.Prezzo_Unitario

                        Select Case obj.Elem_Cod
                            Case CostantiPersonalizzate.FERTILIZZANTI
                                obj.Prodotto_Des = obj.Fer_Des
                            Case CostantiPersonalizzate.FORMULATI
                                obj.Prodotto_Des = obj.Fr_Des
                            Case CostantiPersonalizzate.COADIUVANTI
                                obj.Prodotto_Des = obj.Coad_Des
                            Case CostantiPersonalizzate.INSETTI
                                obj.Prodotto_Des = obj.Ins_Des
                            Case CostantiPersonalizzate.TRAPPOLE
                                obj.Prodotto_Des = obj.Trap_Des
                            Case CostantiPersonalizzate.INNESCHI
                                obj.Prodotto_Des = obj.Av_Des_Vol
                            Case CostantiPersonalizzate.FARMACI
                                obj.Prodotto_Des = obj.Farm_Des
                            Case CostantiPersonalizzate.ALTRI_BENI
                                obj.Prodotto_Des = obj.Mov_Det_Des
                                obj.NomeComune = "Altri Beni Strumentali"
                            Case CostantiPersonalizzate.SERVIZI
                                obj.Prodotto_Des = obj.Servizio_Des & " " & obj.Mov_Det_Des
                                obj.NomeComune = "Servizi"
                            Case Else
                                obj.Prodotto_Des = obj.Mat_Des
                        End Select

                        'Impostazione id_attività eredita
                        If Id_Attivita_Eredita <> 0 Then
                            obj.ID_Attivita = Id_Attivita_Eredita
                            obj.Desc = Attivita_Des_Eredita
                        End If

                    Next

                    kendo_Eredita = JsonConvert.SerializeObject(myList_Eredita, Formatting.None, serializerSettings)
                    TestataElem_Eredita = Nothing
                    myList_Eredita = Nothing

                End If


        End Select

        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

    End Sub


    '##############################################################################################
    'Eredita
    Public Function Leggi_CDG_CaricoScaricoZoo(ByVal piva As String,
                                               ByVal id_agenda As Integer,
                                               ByVal id_mov_det As Integer,
                                               ByVal id_agenda_cdg As Integer,
                                               ByVal Automatico As Integer,
                                               ByVal Id_Attivita_Eredita As Integer,
                                               ByVal Attivita_Des_Eredita As String,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               ByRef kendo_Libera As String,
                                               ByRef Prezzo_Totale As Double,
                                               Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing)

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_CDG_CaricoScaricoZoo()"

        'Dim gefutils AS New Gias_EF_Utility
        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim strCau_Mov As String() = {CAU_CARICO_CONSISTENZE, CAU_SCARICO_CONSISTENZE}

        'Dim strElem_Cod_Fertilizzanti AS Integer() = {3}

        Dim bValido As Boolean = False

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}


        '######################################################################################################################
        '############################################ TESTATA #################################################################
        '######################################################################################################################


        'Eredita
        Select Case Automatico

            Case 0

                'Impostazione Manuale
                If id_agenda_cdg <> 0 Then

                    'Lettura da Agenda Collegata
                    Dim bFiltroId_Mov_Det As Boolean
                    Select Case id_mov_det
                        Case 0
                            bFiltroId_Mov_Det = False
                        Case Else 'Contab
                            bFiltroId_Mov_Det = True
                    End Select


                    '  'Eredita come Magazzino
                    '  Dim TestataElem_Eredita =
                    ' From CI In GiasContext.CDG_Testata
                    ' Group Join Stalla_Raggruppamenti_Group In GiasContext.Stalla_Raggruppamenti
                    '   On CI.Piva Equals Stalla_Raggruppamenti_Group.PIVA And
                    '      CI.Sa_Cod Equals Stalla_Raggruppamenti_Group.sa_cod And
                    '      CI.Id_Destinazione Equals Stalla_Raggruppamenti_Group.Raggruppamento_Cod Into Stalla_Raggruppamenti_Group = Group
                    ' From _Stalla_Raggruppamenti_Group In Stalla_Raggruppamenti_Group.DefaultIfEmpty()
                    ' Group Join Centri_Aziendali In GiasContext.Centri_Aziendali
                    '   On Centri_Aziendali.PIVA Equals CI.Piva And
                    '      Centri_Aziendali.sa_cod Equals CI.Sa_Cod Into Centri_Aziendali_Group = Group
                    ' From _Centri_Aziendali_Group In Centri_Aziendali_Group.DefaultIfEmpty()
                    ' Group Join CategorieMagazzino In GiasContext.CategorieMagazzino
                    '       On CI.Elem_Cod Equals CategorieMagazzino.Elem_Cod Into CategorieMagazzino_Group = Group
                    ' From _CategorieMagazzino_Group In CategorieMagazzino_Group.DefaultIfEmpty()
                    ' Group Join Unita_Misura In GiasContext.UnitaMisura
                    '       On Unita_Misura.UDM_COD Equals CI.Udm_Cod Into Unita_Misura_Group = Group
                    ' From _Unita_Misura_Group In Unita_Misura_Group.DefaultIfEmpty()
                    ' Group Join Zoo_Animali In GiasContext.Zoo_Animali
                    '   On Zoo_Animali.Cod_Progetto Equals CI.pro And
                    '      Materie_Prime.Mat_Cod Equals CI.Mat_Cod Into Materie_Prime_Group = Group
                    ' From _Materie_Prime_Group In Materie_Prime_Group.DefaultIfEmpty()
                    ' Group Join Fertilizzanti In GiasContext.Fertilizzanti
                    '   On CI.Pro_Cod Equals Fertilizzanti.Fer_Cod Into Fertilizzanti_Group = Group
                    ' From _Fertilizzanti_Group In Fertilizzanti_Group.DefaultIfEmpty()
                    ' Group Join Formulati In GiasContext.Formulati
                    '   On CI.Pro_Cod Equals Formulati.Fr_Cod Into Formulati_Group = Group
                    ' From _Formulati_Group In Formulati_Group.DefaultIfEmpty()
                    ' Group Join Coadiuvante In GiasContext.Coadiuvante
                    '   On CI.Pro_Cod Equals Coadiuvante.Coad_Cod Into Coadiuvante_Group = Group
                    ' From _Coadiuvante_Group In Coadiuvante_Group.DefaultIfEmpty()
                    ' Group Join InsettiUtili In GiasContext.InsettiUtili
                    '   On CI.Pro_Cod Equals InsettiUtili.Ins_Cod Into InsettiUtili_Group = Group
                    ' From _InsettiUtili_Group In InsettiUtili_Group.DefaultIfEmpty()
                    ' Group Join Trappole In GiasContext.Trappole
                    '   On CI.Pro_Cod Equals Trappole.TRAP_COD Into Trappole_Group = Group
                    ' From _Trappole_Group In Trappole_Group.DefaultIfEmpty()
                    ' Group Join Avversita In GiasContext.Avversita
                    '       On CI.Pro_Cod Equals Avversita.Av_Cod Into Avversita_Group = Group
                    ' From _Avversita_Group In Avversita_Group.DefaultIfEmpty()
                    ' Group Join Categorie In GiasContext.Categorie.Where(Function(x) x.PADRE = "S000029")
                    '       On Right(Categorie.COD, 3) Equals CI.Pro_Cod Into Categorie_Group = Group
                    ' From _Categorie_Group In Categorie_Group.DefaultIfEmpty()
                    ' Group Join Qualifiche In GiasContext.Qualifiche
                    '   On Qualifiche.Qualifica_Cod Equals CI.Qualifica_Cod Into Qualifica_Group = Group
                    ' From _Qualifica_Group In Qualifica_Group.DefaultIfEmpty()
                    ' Group Join Attivita In GiasContext.Attivita
                    '   On Attivita.ID_Attivita Equals CI.Id_Attivita Into Attivita_Group = Group
                    ' From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
                    ' Group Join Tariffe In GiasContext.Tariffe
                    '   On Tariffe.Tariffa_Cod Equals CI.Tariffa_Cod Into Tariffa_Group = Group
                    ' From _Tariffa_Group In Tariffa_Group.DefaultIfEmpty()
                    ' Where
                    '(CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                    'AndAlso (CI.Piva.Equals(piva)) _
                    'AndAlso (CI.Id_Agenda = id_agenda_cdg) _
                    'AndAlso (CI.Elem_Cod <> 0) _
                    'AndAlso ((bFiltroId_Mov_Det) OrElse CI.Elem_Cod <> CostantiPersonalizzate.CAT_MAG_SERVIZI_PROFESSIONALI) _
                    'AndAlso ((bFiltroId_Mov_Det) OrElse CI.Elem_Cod <> CostantiPersonalizzate.SERVIZI) _
                    'AndAlso (CI.Vecchio_Tipo_Inser_Dati = 0) _
                    'AndAlso ((bFiltroId_Mov_Det AndAlso CI.Modalita_Imputazione = 4) OrElse CI.Flag_Movimento_Campagna = 1) _
                    'AndAlso ((CI.Elem_Cod = CostantiPersonalizzate.FERTILIZZANTI AndAlso _Fertilizzanti_Group.Fer_Cod <> 0) _
                    '  OrElse (CI.Elem_Cod = CostantiPersonalizzate.FORMULATI AndAlso _Formulati_Group.Fr_Cod <> 0) _
                    '  OrElse (CI.Elem_Cod = CostantiPersonalizzate.COADIUVANTI AndAlso _Coadiuvante_Group.Coad_Cod <> 0) _
                    '  OrElse (CI.Elem_Cod = CostantiPersonalizzate.INSETTI AndAlso _InsettiUtili_Group.Ins_Cod <> 0) _
                    '  OrElse (CI.Elem_Cod = CostantiPersonalizzate.TRAPPOLE AndAlso _Trappole_Group.TRAP_COD <> 0) _
                    '  OrElse (CI.Elem_Cod = CostantiPersonalizzate.INNESCHI AndAlso _Avversita_Group.Av_Cod <> 0) _
                    '  OrElse (CI.Elem_Cod = CostantiPersonalizzate.SERVIZI AndAlso Left(_Categorie_Group.COD, 1) = "S") _
                    '  OrElse (CI.Mat_Cod <> 0 AndAlso _Materie_Prime_Group.Mat_Cod <> 0) _
                    '  OrElse (CI.Elem_Cod = CostantiPersonalizzate.ALTRI_BENI AndAlso CI.Pro_Cod = 0 AndAlso CI.Mat_Cod = 0))
                    ' Select New With {
                    '     .Id_CDG = CI.Id_CDG,
                    '     .Data_Inserimento = CI.Data_Inserimento,
                    '     .Modalita_Imputazione = CI.Modalita_Imputazione,
                    '     .Id_Agenda = CI.Id_Agenda,
                    '     .Id_Mov = CI.Id_Mov,
                    '     .Id_Mov_Det = CI.Id_Mov_Det,
                    '     .Mac_Cod = CI.Mac_Cod,
                    '     .Cod_Risum = CI.Cod_RisUm,
                    '     .Elem_Cod = CI.Elem_Cod,
                    '     .Pro_Cod = CI.Pro_Cod,
                    '     .Mat_Cod = CI.Mat_Cod,
                    '     .Lav_Cod = CI.Lav_Cod,
                    '     .ID_Attivita = CI.Id_Attivita,
                    '     .Qualifica_Cod = CI.Qualifica_Cod,
                    '     .Tariffa_Cod = CI.Tariffa_Cod,
                    '     .Turno_Cod = CI.Turno_Cod,
                    '     .Conto_Cod = CI.Conto_Cod,
                    '     .Lotto = CI.Lotto,
                    '     .Mezzo = CI.Mezzo,
                    '     .Udm_Cod = CI.Udm_Cod,
                    '     .Qta = CI.Qta,
                    '     .Prezzo_Unitario = CI.Prezzo_Unitario,
                    '     .Prezzo_Totale = CI.Valore_Totale,
                    '     .Descrizione = CI.Descrizione,
                    '     .Tipo_Ripartizione = CI.Tipo_Ripartizione,
                    '     .Budget = CI.Budget,
                    '     .Costi_Ricavi = CI.Costi_Ricavi,
                    '     .Modalita_Ripartizione = CI.Modalita_Ripartizione,
                    '     .Vecchio_Tipo_Inser_Dati = CI.Vecchio_Tipo_Inser_Dati,
                    '     .Sa_Cod = CI.Sa_Cod,
                    '     .Sa_Nome = If(_Centri_Aziendali_Group Is Nothing, "", _Centri_Aziendali_Group.sa_nome),
                    '     .Tipo_Destinazione = CI.Tipo_Destinazione,
                    '     .Fabbricato_Cod = CI.Id_Destinazione,
                    '     .Fabbricato_Des = If(_Fabbricati_Group Is Nothing, "", _Fabbricati_Group.Fabbricato_Des),
                    '     .NomeComune = If(_CategorieMagazzino_Group Is Nothing, "", _CategorieMagazzino_Group.NomeComune),
                    '     .Prodotto_Cod = If(CI.Pro_Cod <> 0, CI.Pro_Cod, CI.Mat_Cod),
                    '     .Prodotto_Des = "",
                    '     .Mat_Des = If(_Materie_Prime_Group Is Nothing, "", _Materie_Prime_Group.Mat_Des),
                    '     .Fer_Des = If(_Fertilizzanti_Group Is Nothing, "", _Fertilizzanti_Group.Fer_Des),
                    '     .Fr_Des = If(_Formulati_Group Is Nothing, "", _Formulati_Group.Fr_Des),
                    '     .Coad_Des = If(_Coadiuvante_Group Is Nothing, "", _Coadiuvante_Group.Coad_Des),
                    '     .Ins_Des = If(_InsettiUtili_Group Is Nothing, "", _InsettiUtili_Group.Ins_Des),
                    '     .Trap_Des = If(_Trappole_Group Is Nothing, "", _Trappole_Group.TRAP_DES),
                    '      .Av_Des_Vol = If(_Avversita_Group Is Nothing, "", _Avversita_Group.Av_Des_Vol),
                    '     .Udm_Des = If(_Unita_Misura_Group Is Nothing, "", _Unita_Misura_Group.UDM_SIM),
                    '     .Qualifica_Des = If(_Qualifica_Group Is Nothing, "", _Qualifica_Group.Qualifica_Des),
                    '     .Desc = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Sigla & " - " & _Attivita_Group.Desc),
                    '     .Tariffa_Des = If(_Tariffa_Group Is Nothing, "", _Tariffa_Group.Tariffa_Des),
                    '      .OrigineApp = CI.OrigineApp,
                    '     .APP_CDG_Generale_ID = CI.APP_CDG_Generale_ID
                    '      }

                    '  'Imposto la testata se non fatto precedentemente
                    '  Dim myList_Eredita = TestataElem_Eredita.Distinct().ToList()
                    '  For Each obj In myList_Eredita
                    '      Select Case obj.Elem_Cod
                    '          Case CostantiPersonalizzate.FERTILIZZANTI
                    '              obj.Prodotto_Des = obj.Fer_Des
                    '          Case CostantiPersonalizzate.FORMULATI
                    '              obj.Prodotto_Des = obj.Fr_Des
                    '          Case CostantiPersonalizzate.COADIUVANTI
                    '              obj.Prodotto_Des = obj.Coad_Des
                    '          Case CostantiPersonalizzate.INSETTI
                    '              obj.Prodotto_Des = obj.Ins_Des
                    '          Case CostantiPersonalizzate.TRAPPOLE
                    '              obj.Prodotto_Des = obj.Trap_Des
                    '          Case CostantiPersonalizzate.INNESCHI
                    '              obj.Prodotto_Des = obj.Av_Des_Vol
                    '          Case CostantiPersonalizzate.ALTRI_BENI
                    '              obj.Prodotto_Des = obj.Descrizione
                    '              obj.NomeComune = "Altri Beni Strumentali"
                    '          Case CostantiPersonalizzate.SERVIZI
                    '              obj.Prodotto_Des = obj.Descrizione
                    '              obj.NomeComune = "Servizi"
                    '          Case Else
                    '              obj.Prodotto_Des = obj.Mat_Des
                    '      End Select
                    '  Next

                    '  kendo_Eredita = JsonConvert.SerializeObject(myList_Eredita, Formatting.None, serializerSettings)
                    '  TestataElem_Eredita = Nothing
                    '  myList_Eredita = Nothing

                End If

            Case 1



                If id_agenda <> 0 Then

                    'Lettura da Agenda Collegata
                    Dim bFiltroId_Mov_Det As Boolean

                    Select Case id_mov_det
                        Case 0
                            bFiltroId_Mov_Det = False
                        Case Else 'Contab
                            bFiltroId_Mov_Det = True
                            strCau_Mov = {CAU_CARICO_CONSISTENZE, CAU_SCARICO_CONSISTENZE}
                    End Select


                    Dim TestataElem_Eredita =
                       From CI In GiasContext.Movimenti_dettagli
                       Join Movimenti In GiasContext.Movimenti
                         On CI.PIVA Equals Movimenti.PIVA And
                            CI.Id_Agenda Equals Movimenti.Id_Agenda And
                            CI.Id_Mov Equals Movimenti.Id_Mov
                       Join Agenda In GiasContext.Agenda
                         On Agenda.PIVA Equals Movimenti.PIVA And
                            Agenda.Id_Agenda Equals Movimenti.Id_Agenda
                       Group Join Mov_Destinazioni In GiasContext.Mov_Destinazioni
                         On CI.PIVA Equals Mov_Destinazioni.Piva And
                            CI.Id_Agenda Equals Mov_Destinazioni.Id_Agenda And
                            CI.Id_Mov Equals Mov_Destinazioni.Id_Mov And
                            CI.Id_Mov_Det Equals Mov_Destinazioni.Id_Mov_Det Into Mov_Destinazioni_Group = Group
                       From _Mov_Destinazioni_Group In Mov_Destinazioni_Group.DefaultIfEmpty()
                       Group Join Stalla_Raggruppamenti In GiasContext.Stalla_Raggruppamenti
                         On _Mov_Destinazioni_Group.Piva Equals Stalla_Raggruppamenti.PIVA And
                            _Mov_Destinazioni_Group.Sa_Cod Equals Stalla_Raggruppamenti.sa_cod And
                            _Mov_Destinazioni_Group.Id_Destinazione Equals Stalla_Raggruppamenti.Raggruppamento_Cod Into Stalla_Raggruppamenti_Group = Group
                       From _Stalla_Raggruppamenti_Group In Stalla_Raggruppamenti_Group.DefaultIfEmpty()
                       Group Join Stalla In GiasContext.Stalla
                         On _Stalla_Raggruppamenti_Group.PIVA Equals Stalla.PIVA And
                             _Stalla_Raggruppamenti_Group.sa_cod Equals Stalla.sa_cod And
                             _Stalla_Raggruppamenti_Group.STA_NUM Equals Stalla.STA_NUM Into Stalla_Group = Group
                       From _Stalla_Group In Stalla_Group.DefaultIfEmpty()
                       Group Join Centri_Aziendali In GiasContext.Centri_Aziendali
                         On Centri_Aziendali.PIVA Equals CI.PIVA And
                            Centri_Aziendali.sa_cod Equals CI.Sa_Cod Into Centri_Aziendali_Group = Group
                       From _Centri_Aziendali_Group In Centri_Aziendali_Group.DefaultIfEmpty()
                       Group Join Unita_Misura In GiasContext.UnitaMisura
                         On Unita_Misura.UDM_COD Equals CI.Udm_Cod Into Unita_Misura_Group = Group
                       From _Unita_Misura_Group In Unita_Misura_Group.DefaultIfEmpty()
                       Group Join Zoo_Animali In GiasContext.Zoo_Animali
                         On Zoo_Animali.Cod_Progetto Equals CI.Cod_Progetto Into Zoo_Animali_Group = Group
                       From _Zoo_Animali_Group In Zoo_Animali_Group.DefaultIfEmpty()
                       Group Join Qualifiche In GiasContext.Qualifiche
                         On Qualifiche.Qualifica_Cod Equals CI.Qualifica_Cod Into Qualifica_Group = Group
                       From _Qualifica_Group In Qualifica_Group.DefaultIfEmpty()
                       Group Join Attivita In GiasContext.Attivita
                         On Attivita.ID_Attivita Equals CI.ID_Attivita Into Attivita_Group = Group
                       From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
                       Group Join Tariffe In GiasContext.Tariffe
                         On Tariffe.Tariffa_Cod Equals CI.Tariffa_Cod Into Tariffa_Group = Group
                       From _Tariffa_Group In Tariffa_Group.DefaultIfEmpty()
                       Where
                          (Agenda.PIVA.Equals(piva)) _
                      AndAlso ((Not bFiltroId_Mov_Det) OrElse CI.Id_Mov_Det = id_mov_det) _
                      AndAlso strCau_Mov.Contains(Movimenti.Cau_Mov) _
                      AndAlso CI.Elem_Cod = CostantiPersonalizzate.ZOO_CONSISTENZA
                       Group By x = New With {
                            Key .Id_CDG = 0,
                            Key .Flag_Movimento_Campagna = 0,
                            Key .Data_Movimento = Movimenti.Data_Movimento,
                            Key .Modalita_Imputazione = 0,
                            Key .Id_Agenda = Agenda.Id_Agenda,
                            Key .Raccoglitore_Cod = 0,
                           Key .Lav_Cod = Agenda.Lav_Cod,
                           Key .Id_Mov = 0,
                           Key .Id_Mov_Det = 0,
                           Key .Mac_Cod = 0,
                           Key .Cod_Risum = 0,
                           Key .Elem_Cod = 0,
                           Key .Pro_Cod = 0,
                           Key .Mat_Cod = 0,
                           Key .ID_Attivita = CI.ID_Attivita,
                           Key .Qualifica_Cod = 0,
                           Key .Tariffa_Cod = 0,
                           Key .Turno_Cod = 0,
                           Key .Conto_Cod = 0,
                           Key .Lotto = 0,
                           Key .Mezzo = 0,
                           Key .Mov_Det_Des = Agenda.des_lib,
                           Key .Udm_Cod = CI.Udm_Cod,
                           Key .Qta = 1,
                           Key .Prezzo_Totale = 0,
                           Key .Descrizione = Agenda.des_lib,
                           Key .Tipo_Ripartizione = 0,
                           Key .Budget = 0,
                           Key .Costi_Ricavi = 0,
                           Key .Modalita_Ripartizione = 0,
                           Key .Vecchio_Tipo_Inser_Dati = 0,
                           Key .Sa_Cod = 0,
                           Key .Sa_Nome = "",
                           Key .Tipo_Destinazione = 0,
                           Key .Fabbricato_Cod = 0,
                           Key .Fabbricato_Des = "",
                           Key .Prodotto_Cod = 0,
                           Key .Prodotto_Des = Agenda.des_lib,
                           Key .Matricola = "",
                           Key .Udm_Des = "",
                           Key .Qualifica_Des = "",
                          .Desc = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Sigla & " - " & _Attivita_Group.Desc),
                           Key .Tariffa_Des = ""
                  } Into g = Group
                       Order By g.Sum(Function(r) r.CI.Prezzo_Unitario) Descending
                       Select New With {
                             .Id_CDG = x.Id_CDG,
                             .Flag_Movimento_Campagna = x.Flag_Movimento_Campagna,
                             .Data_Movimento = x.Data_Movimento,
                             .Modalita_Imputazione = x.Modalita_Imputazione,
                             .Id_Agenda = x.Id_Agenda,
                             .Raccoglitore_Cod = x.Raccoglitore_Cod,
                             .Lav_Cod = x.Lav_Cod,
                             .Id_Mov = x.Id_Mov,
                             .Id_Mov_Det = x.Id_Mov_Det,
                             .Mac_Cod = 0,
                             .Cod_Risum = 0,
                             .Elem_Cod = x.Elem_Cod,
                             .Pro_Cod = x.Pro_Cod,
                             .Mat_Cod = x.Mat_Cod,
                             .ID_Attivita = x.ID_Attivita,
                             .Qualifica_Cod = x.Qualifica_Cod,
                             .Tariffa_Cod = x.Tariffa_Cod,
                             .Turno_Cod = x.Turno_Cod,
                             .Conto_Cod = x.Conto_Cod,
                             .Lotto = x.Lotto,
                             .Mezzo = x.Mezzo,
                             .Mov_Det_Des = x.Mov_Det_Des,
                             .Udm_Cod = x.Udm_Cod,
                             .Qta = x.Qta,
                             .Prezzo_Totale = x.Prezzo_Totale,
                             .Descrizione = x.Descrizione,
                             .Tipo_Ripartizione = 0,
                             .Budget = 0,
                             .Costi_Ricavi = 0,
                              .Modalita_Ripartizione = 0,
                             .Vecchio_Tipo_Inser_Dati = 0,
                             .Sa_Cod = x.Sa_Cod,
                             .Sa_Nome = x.Sa_Nome,
                            .Tipo_Destinazione = x.Tipo_Destinazione,
                            .Fabbricato_Cod = x.Fabbricato_Cod,
                            .Fabbricato_Des = x.Fabbricato_Des,
                            .Prodotto_Cod = x.Prodotto_Cod,
                            .Prodotto_Des = x.Prodotto_Des,
                            .Matricola = x.Matricola,
                            .Udm_Des = x.Udm_Des,
                            .Qualifica_Des = x.Qualifica_Des,
                            .Desc = x.Desc,
                            .Tariffa_Des = x.Tariffa_Des,
                            .Prezzo_Unitario = g.Sum(Function(r) r.CI.Prezzo_Unitario)
                       }

                    TestataElem_Eredita = TestataElem_Eredita.Where(Function(x) x.Id_Agenda = id_agenda)

                    'Imposto la testata se non fatto precedentemente
                    Dim myList_Eredita = TestataElem_Eredita.Distinct.ToList()
                    For Each obj In myList_Eredita

                        obj.Prezzo_Totale = obj.Prezzo_Unitario

                        Prezzo_Totale = obj.Prezzo_Unitario

                        'Impostazione id_attività eredita
                        If Id_Attivita_Eredita <> 0 Then
                            obj.ID_Attivita = Id_Attivita_Eredita
                            obj.Desc = Attivita_Des_Eredita
                        End If

                    Next

                    kendo_Libera = JsonConvert.SerializeObject(myList_Eredita, Formatting.None, serializerSettings)
                    TestataElem_Eredita = Nothing
                    myList_Eredita = Nothing

                End If


        End Select

        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

    End Function


    '##############################################################################################
    Private Sub Leggi_CDG_Dettagli(ByVal piva As String,
                                   ByVal id_agenda As Integer,
                                   ByVal id_agenda_cdg As Integer,
                                   ByVal data_movimento As Date,
                                   ByVal Automatico As Integer,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   ByRef kendo_Impianti_Dettagli As String,
                                   ByRef kendo_Macchine_Dettagli As String,
                                   ByRef kendo_Progetti_Dettagli As String,
                                   ByRef kendo_Linee_Dettagli As String,
                                   ByRef kendo_Zoo_Dettagli As String,
                                   ByVal split As Integer,
                                   Optional ByVal bForzaLeggiDettDistinta As Boolean = False,
                                   Optional Budget As Integer = 0,
                                   Optional Raccoglitore_Cod As Integer = 0)


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_CDG_Dettagli()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim strCau_Mov As String() = {CAU_SCARICO}
        Dim strCau_Progetto As String() = {CAU_PROGETTO_PRODUZIONE}
        Dim bSkip As Boolean = False

        Dim DtRipartizione As DataTable = Nothing
        Dim DtProdotto As DataTable
        Dim Sa_Cod_Last As Integer = 0
        Dim bMulticentro As Boolean = False
        Dim TotaleRipartizione As Decimal = 0

        'Dim strElem_Cod_Fertilizzanti AS Integer() = {CostantiPersonalizzate.FERTILIZZANTI}


        Dim JsonString As New StringBuilder()

        Dim bValido As Boolean = False

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}

            '######################################################################################################################
            '############################################ DETTAGLI ################################################################
            '######################################################################################################################


            If id_agenda <> 0 Then

                Dim leggi_agenda As New Agenda_R
                Dim objAttivitaXoperazioni As New AgronicaCoreContabDAL.AttivitaXOperazioni_R
                Dim objAttivita As New AgronicaCoreContabDAL.Attivita_R
                Dim id_attivita As Integer = 0
                Dim Lav_Cod As Integer = 0
                Dim DT_Attivita As New DataTable
                Dim DtAttPoliannuali As New DataTable
                Dim DrAttPoliannuali As DataRow()
                Dim DT_Agenda As DataTable
                Dim poliennale As Boolean = False
                Dim annuale As Boolean = False
                Dim bAttivitaPoliannuale As Boolean = False

                DT_Agenda = leggi_agenda.Leggi(piva, 0, id_agenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri, ,, Raccoglitore_Cod)
                'DT_Attivita = objAttivita.Leggi(id_attivita, "", "", objParametri)
                DtAttPoliannuali = objAttivita.Leggi(0, "Attivita_Poliannuale = 1 And Tipo_Utilizzo In (0,2)", "", objParametri)



                For Each dr_ag In DT_Agenda.Rows

                    Lav_Cod = dr_ag("Lav_Cod")

                    If dr_ag("Lav_Cod") = LAVCOD_ALTRE_OPERAZIONI Then
                        id_attivita = dr_ag("ID_Attivita")


                        DrAttPoliannuali = DtAttPoliannuali.Select("Id_Attivita = " & id_attivita)
                        If DrAttPoliannuali Is Nothing OrElse DrAttPoliannuali.Length = 0 Then
                            annuale = True
                        Else
                            poliennale = True
                        End If

                    Else


                        DT_Agenda = Leggi_Agenda_Riferimento(piva, dr_ag("Id_Agenda"), 0, True, objParametri, GiasContext)

                        If DT_Agenda.Rows.Count > 0 Then

                            'Lettura delle testate relative all'id_agenda
                            DT_Attivita = Leggi_CDG_Testata(piva, DT_Agenda.Rows(0).Item("Id_Agenda_Rif"), 0, "", objParametri)
                            'DT_Attivita = objAttivitaXoperazioni.Leggi_Solo_Attivita(id_attivita, ag.Rows(0)("Lav_Cod"), "", "", objParametri, True, piva)
                            For Each dr_Attivita As DataRow In DT_Attivita.Rows

                                DrAttPoliannuali = DtAttPoliannuali.Select("Id_Attivita = " & dr_Attivita("id_attivita"))
                                If DrAttPoliannuali Is Nothing OrElse DrAttPoliannuali.Length = 0 Then
                                    annuale = True
                                Else
                                    poliennale = True
                                End If

                            Next
                        End If

                    End If

                Next

                If poliennale AndAlso Not annuale Then
                    bAttivitaPoliannuale = True
                End If


                'Controllo Lav_Cod
                Select Case Lav_Cod

                    Case 1000 To 1999, LAVCOD_ORDINE_ACQUISTO, LAVCOD_ORDINE_VENDITA 'Contab

                        kendo_Zoo_Dettagli = "[]"
                        strCau_Mov = {CAU_CARICO, CAU_SCARICO}
                        bSkip = False


                    Case 3000 To 3999 'Zoo

                        kendo_Zoo_Dettagli = Leggi_Zoo_Dettagli_Ipno(piva, id_agenda, data_movimento, False, objParametri)
                        strCau_Mov = {CAU_SCARICO}
                        bSkip = True


                    Case Else

                        If Raccoglitore_Cod <> 0 Then

                            'Verifico se siamo in situazione multicentro
                            DtProdotto = Leggi_Dettagli_Agenda(piva, id_agenda, objParametri)

                            If DtProdotto.Rows.Count > 0 Then

                                DtRipartizione = Leggi_Impianti_Da_Prodotto(piva, 0, Raccoglitore_Cod, DtProdotto(0), objParametri)

                                If DtRipartizione.Rows.Count > 0 Then

                                    For Each drRipartizione In DtRipartizione.Rows

                                        If Sa_Cod_Last = 0 Then
                                            Sa_Cod_Last = drRipartizione("Sa_Cod")
                                        End If
                                        If Sa_Cod_Last <> drRipartizione("Sa_Cod") Then
                                            bMulticentro = True
                                        End If

                                    Next

                                    If id_agenda_cdg = 0 AndAlso bMulticentro Then

                                        Select Case id_agenda_cdg

                                            Case 0

                                                'Nota: operazione multicentro non ancora creata --> occorre ripartire sulle superfici (Qta2) poichè la qta distribuzione non è sensata
                                                For Each drRipartizione In DtRipartizione.Rows

                                                    drRipartizione("Qta") = drRipartizione("Qta2")
                                                    TotaleRipartizione = TotaleRipartizione + drRipartizione("Qta")

                                                Next

                                        End Select

                                    Else

                                        DtRipartizione = Nothing

                                    End If


                                End If

                            End If


                        End If

                        Select Case bMulticentro
                            Case False
                                kendo_Impianti_Dettagli = Leggi_Impianti_Dettagli(piva, id_agenda, data_movimento, False, id_agenda_cdg, bForzaLeggiDettDistinta, bAttivitaPoliannuale, objParametri)
                            Case True
                                kendo_Impianti_Dettagli = Leggi_Impianti_Dettagli_Multicentro(piva, Raccoglitore_Cod, data_movimento, DtRipartizione, TotaleRipartizione, False, id_agenda_cdg, bForzaLeggiDettDistinta, bAttivitaPoliannuale, objParametri)
                        End Select


                        strCau_Mov = {CAU_SCARICO}
                        bSkip = True

                End Select


            End If


            If Not bSkip AndAlso id_agenda_cdg <> 0 Then

                'Impianti

                ' Con la gestione a distinta potrei avere una situazione in cui ci sono costi su distinte diversa da quella corrente e nessun 
                ' costo sulla corrente; quindi con la prima query trovo quelle che hanno costi e con la successiva trovo solo il
                ' periodo corrente da mostrare sulla riga principale

                ' Leggo la prima testata per non sommare più volte le %
                Dim primaRigaTestata = (From testata In GiasContext.CDG_Testata
                                        Where testata.Piva_Superuser.Equals(Piva_SuperUser) AndAlso
                                              testata.Piva.Equals(piva) AndAlso
                                              testata.Id_Agenda = id_agenda_cdg).FirstOrDefault()

                If primaRigaTestata Is Nothing Then
                    Throw New Exception(String.Format("L'id agenda {0} non è presente in CDG_Testata ({1}_{0})",
                                                      id_agenda_cdg, piva))
                End If

                If Budget = 2 Then

                    'Impianti da Tabelle Budget

                    Dim PrimaQuery_Impianti =
                       From CI In GiasContext.CDG_Dettagli
                       Join CDG_Testata In GiasContext.CDG_Testata
                         On CDG_Testata.Piva Equals CI.Piva _
                         And CDG_Testata.Id_CDG Equals CI.Id_CDG
                       Join Reg_Impianti In GiasContext.Budget_Reg_Impianti
                    On Reg_Impianti.PIVA Equals CI.Piva _
                         And Reg_Impianti.SA_COD Equals CI.Sa_Cod _
                         And Reg_Impianti.APPEZZA Equals CI.Appezza _
                         And Reg_Impianti.ID_REG Equals CI.Id_Reg _
                       And Reg_Impianti.Id_Budget Equals CDG_Testata.Budget
                       Where
                      (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                      AndAlso (CI.Piva.Equals(piva)) _
                      AndAlso (CDG_Testata.Id_Agenda = id_agenda_cdg) _
                      AndAlso (CI.Id_Reg <> 0) _
                       AndAlso (CI.Id_CDG = primaRigaTestata.Id_CDG)
                       Group By x = New With {
                            Key .Id_CDG = 0,
                            Key .Id_CDG_Dettagli = 0,
                            Key .Piva = CI.Piva,
                            Key .Sa_Cod = CI.Sa_Cod,
                            Key .Campo_Cod = CI.Campo_Cod,
                            Key .Appezza = CI.Appezza,
                            Key .Id_Reg = CI.Id_Reg,
                            Key .Id_Cod_reg_impianti_codici = CI.Id_Cod_reg_impianti_codici,
                            Key .Id_Imputazione = CI.Id_Imputazione,
                            Key .Macchine_Cod = CI.Macchine_Cod,
                            Key .Linea_Cod = CI.Linea_Cod,
                            Key .Lotto_Input_Costi = CI.Lotto_Input_Costi,
                            Key .Validita_Inizio = CI.Validita_Inizio,
                            Key .Validita_Fine = CI.Validita_Fine
                        } Into g = Group
                       Select New With {
                          .Id_CDG = 0,
                          .Id_CDG_Dettagli = 0,
                          .Piva = x.Piva,
                          .Sa_Cod = x.Sa_Cod,
                          .Campo_Cod = x.Campo_Cod,
                          .Appezza = x.Appezza,
                          .Id_Reg = x.Id_Reg,
                          .Id_Cod_reg_impianti_codici = x.Id_Cod_reg_impianti_codici,
                          .Id_Imputazione = x.Id_Imputazione,
                          .Macchine_Cod = x.Macchine_Cod,
                          .Linea_Cod = x.Linea_Cod,
                          .Lotto_Input_Costi = x.Lotto_Input_Costi,
                          .Validita_Inizio = x.Validita_Inizio,
                          .Validita_Fine = x.Validita_Fine,
                          .Valore_Totale = g.Sum(Function(r) r.CI.Valore)
                      }

                    Dim myList_PrimaQuery_Impianti = PrimaQuery_Impianti.ToList()

                    Dim myListImpianti As New List(Of Object)

                    For Each obj In myList_PrimaQuery_Impianti

                        'Impianti
                        Dim DettagliElem_Impianti =
                            From Reg_Impianti In GiasContext.Budget_Reg_Impianti
                            Join Imprese_Progetti In GiasContext.Budget_Imprese_Progetti
                            On Imprese_Progetti.Piva Equals Reg_Impianti.PIVA And
                            Imprese_Progetti.Sa_Cod Equals Reg_Impianti.SA_COD And
                            Imprese_Progetti.Appezza Equals Reg_Impianti.APPEZZA And
                            Imprese_Progetti.Id_Reg Equals Reg_Impianti.ID_REG
                            Group Join CDG_Dettagli In GiasContext.CDG_Dettagli.Where(Function(x) x.Piva_Superuser.Equals(Piva_SuperUser))
                            On Imprese_Progetti.Piva Equals CDG_Dettagli.Piva _
                            And Imprese_Progetti.Sa_Cod Equals CDG_Dettagli.Sa_Cod _
                            And Imprese_Progetti.Appezza Equals CDG_Dettagli.Appezza _
                            And Imprese_Progetti.Id_Reg Equals CDG_Dettagli.Id_Reg _
                            And Imprese_Progetti.Progetto_Cod Equals CDG_Dettagli.Progetto_Cod
                            Into CDG_Dettagli_Group = Group
                            From _CDG_Dettagli_Group In CDG_Dettagli_Group.DefaultIfEmpty()
                            Group Join CDG_Testata In GiasContext.CDG_Testata.Where(Function(x) x.Piva_Superuser.Equals(Piva_SuperUser))
                            On CDG_Testata.Piva Equals _CDG_Dettagli_Group.Piva _
                            And CDG_Testata.Id_CDG Equals _CDG_Dettagli_Group.Id_CDG
                            Into CDG_Testata_Group = Group
                            From _CDG_Testata_Group In CDG_Testata_Group.DefaultIfEmpty()
                            Join Appezzamento In GiasContext.Budget_Appezzamento
                            On Appezzamento.PIVA Equals Reg_Impianti.PIVA And
                               Appezzamento.SA_COD Equals Reg_Impianti.SA_COD And
                               Appezzamento.APPEZZA Equals Reg_Impianti.APPEZZA
                            Join Centri_Aziendali In GiasContext.Centri_Aziendali
                            On Centri_Aziendali.PIVA Equals Appezzamento.PIVA And
                               Centri_Aziendali.sa_cod Equals Appezzamento.SA_COD
                            Group Join Cultivar In GiasContext.Cultivar
                            On Cultivar.Cul_Cod Equals Reg_Impianti.CUL_COD Into Cultivar_Group = Group
                            From _Cultivar_Group In Cultivar_Group.DefaultIfEmpty()
                            Group Join Specie In GiasContext.SpecieVegetali
                            On Specie.Veg_Cod Equals _Cultivar_Group.Veg_Cod Into Specie_Group = Group
                            From _Specie_Group In Specie_Group.DefaultIfEmpty()
                            Group Join Reg_Impianti_Distinta In GiasContext.Budget_Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Distinta_Chiusa))
                            On Reg_Impianti_Distinta.PIVA Equals Imprese_Progetti.Piva And
                            Reg_Impianti_Distinta.sa_cod Equals Imprese_Progetti.Sa_Cod And
                            Reg_Impianti_Distinta.appezza Equals Imprese_Progetti.Appezza And
                            Reg_Impianti_Distinta.Id_Reg Equals Imprese_Progetti.Id_Reg And
                            Reg_Impianti_Distinta.Progetto_Cod Equals Imprese_Progetti.Progetto_Cod
                            Into Reg_Impianti_Distinta_Group = Group
                            From _Reg_Impianti_Distinta_Group In Reg_Impianti_Distinta_Group.DefaultIfEmpty()
                            Group Join Reg_Impianti_CodiceImp In GiasContext.Budget_Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Codice_Impianto))
                            On Reg_Impianti_CodiceImp.PIVA Equals Reg_Impianti.PIVA And
                            Reg_Impianti_CodiceImp.sa_cod Equals Reg_Impianti.SA_COD And
                            Reg_Impianti_CodiceImp.appezza Equals Reg_Impianti.APPEZZA And
                            Reg_Impianti_CodiceImp.Id_Reg Equals Reg_Impianti.ID_REG
                        Into Reg_Impianti_CodiceImp_Group = Group
                            From _Reg_Impianti_CodiceImp_Group In Reg_Impianti_CodiceImp_Group.DefaultIfEmpty()
                            Group Join Campi In GiasContext.Budget_Campi
                            On Campi.Piva Equals Appezzamento.PIVA _
                            And Campi.Sa_Cod Equals Appezzamento.SA_COD _
                            And Campi.Campo_Cod Equals Appezzamento.Campo_Cod
                            Into Campi_Group = Group
                            From _Campi_Group In Campi_Group.DefaultIfEmpty()
                            Where
                            Reg_Impianti.PIVA = CStr(obj.Piva) _
                            AndAlso Reg_Impianti.SA_COD = CStr(obj.Sa_Cod) _
                            AndAlso Reg_Impianti.APPEZZA = CStr(obj.Appezza) _
                            AndAlso Reg_Impianti.ID_REG = CStr(obj.Id_Reg) _
                            AndAlso (split = 1 OrElse Imprese_Progetti.Validita_Inizio <= data_movimento) _
                            AndAlso (split = 1 OrElse Imprese_Progetti.Validita_Fine >= data_movimento) _
                            AndAlso strCau_Progetto.Contains(Imprese_Progetti.Cau_Progetto) _
                            AndAlso (_CDG_Testata_Group.Id_Agenda = id_agenda_cdg)
                            Select New With {
                           .Id_CDG = If(_CDG_Dettagli_Group Is Nothing, 0, _CDG_Dettagli_Group.Id_CDG),
                           .Id_CDG_Dettagli = If(_CDG_Dettagli_Group Is Nothing, 0, _CDG_Dettagli_Group.Id_CDG_Dettagli),
                           .Piva = Reg_Impianti.PIVA,
                           .Sa_Cod = Reg_Impianti.SA_COD,
                           .Campo_Cod = obj.Campo_Cod,
                           .Campo_Des = If(_Campi_Group Is Nothing, "", _Campi_Group.Campo_Des),
                           .Sa_Nome = Centri_Aziendali.sa_nome,
                           .Appezza = Reg_Impianti.APPEZZA,
                           .Id_Destinazione = Reg_Impianti.ID_REG,
                           .Id_Cod_reg_impianti_codici = obj.Id_Cod_reg_impianti_codici,
                            .Id_Imputazione = obj.Id_Imputazione,
                            .Macchine_Cod = obj.Macchine_Cod,
                            .Linea_Cod = obj.Linea_Cod,
                            .Lotto_Input_Costi = obj.Lotto_Input_Costi,
                            .Valore = If(_CDG_Dettagli_Group Is Nothing, 0, _CDG_Dettagli_Group.Valore),
                            .ValoreMassimo = obj.Valore_Totale,
                            .Validita_Inizio = obj.Validita_Inizio,
                            .Validita_Fine = obj.Validita_Fine,
                            .Validita_Inizio_Distinta = Imprese_Progetti.Validita_Inizio,
                            .Validita_Fine_Distinta = Imprese_Progetti.Validita_Fine,
                            .Key = "",
                            .Veg_Cod = If(_Specie_Group Is Nothing, 0, _Specie_Group.Veg_Cod),
                            .Veg_Des = If(_Specie_Group Is Nothing, "", _Specie_Group.Veg_Des),
                            .Cul_Cod = If(_Cultivar_Group Is Nothing, 0, _Cultivar_Group.Cul_Cod),
                            .Cul_Des = If(_Cultivar_Group Is Nothing, "Terreno Nudo", _Cultivar_Group.Cul_Des),
                            .Descrizione = Appezzamento.APP_NOME,
                            .Progetto_Cod = Imprese_Progetti.Progetto_Cod,
                            .Progetto_Nome = Imprese_Progetti.Progetto_Nome,
                            .Progetto_Des = Imprese_Progetti.Progetto_Des,
                            .Superficie = Reg_Impianti.Sup_Imp,
                            .Codice_Impianto = If(_Reg_Impianti_CodiceImp_Group Is Nothing, "", _Reg_Impianti_CodiceImp_Group.val_cod),
                            .Flag_Distinta_Chiusa = If(_Reg_Impianti_Distinta_Group Is Nothing OrElse CInt(_Reg_Impianti_Distinta_Group.val_cod) = 0, False, True)
                             }


                        ' Compongo la chiave
                        Dim myImpianti_Dettagli_Elem = DettagliElem_Impianti.FirstOrDefault()
                        myImpianti_Dettagli_Elem.Valore = Decimal.Round(Convert.ToDecimal(myImpianti_Dettagli_Elem.Valore), 2)
                        myImpianti_Dettagli_Elem.Key = myImpianti_Dettagli_Elem.Piva & "-" & myImpianti_Dettagli_Elem.Sa_Cod.ToString() & "-" & myImpianti_Dettagli_Elem.Campo_Cod.ToString() & "-" & myImpianti_Dettagli_Elem.Appezza.ToString() & "-" & myImpianti_Dettagli_Elem.Id_Destinazione.ToString()
                        myListImpianti.Add(myImpianti_Dettagli_Elem)

                    Next

                    kendo_Impianti_Dettagli = JsonConvert.SerializeObject(myListImpianti, Formatting.None, serializerSettings)
                    myListImpianti = Nothing

                Else




                    'Impianti da Consuntivo

                    Dim PrimaQuery_Impianti =
                       From CI In GiasContext.CDG_Dettagli
                       Join CDG_Testata In GiasContext.CDG_Testata
                         On CDG_Testata.Piva Equals CI.Piva _
                         And CDG_Testata.Id_CDG Equals CI.Id_CDG
                       Join Reg_Impianti In GiasContext.Reg_Impianti
                         On Reg_Impianti.PIVA Equals CI.Piva _
                         And Reg_Impianti.SA_COD Equals CI.Sa_Cod _
                         And Reg_Impianti.APPEZZA Equals CI.Appezza _
                         And Reg_Impianti.ID_REG Equals CI.Id_Reg
                       Where
                      (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                      AndAlso (CI.Piva.Equals(piva)) _
                      AndAlso (CDG_Testata.Id_Agenda = id_agenda_cdg) _
                      AndAlso (CI.Id_Reg <> 0) _
                       AndAlso (CDG_Testata.Budget = 0) _
                       AndAlso (CI.Id_CDG = primaRigaTestata.Id_CDG)
                       Group By x = New With {
                            Key .Id_CDG = 0,
                            Key .Id_CDG_Dettagli = 0,
                            Key .Piva = CI.Piva,
                            Key .Sa_Cod = CI.Sa_Cod,
                            Key .Campo_Cod = CI.Campo_Cod,
                            Key .Appezza = CI.Appezza,
                            Key .Id_Reg = CI.Id_Reg,
                            Key .Id_Cod_reg_impianti_codici = CI.Id_Cod_reg_impianti_codici,
                            Key .Id_Imputazione = CI.Id_Imputazione,
                            Key .Macchine_Cod = CI.Macchine_Cod,
                            Key .Linea_Cod = CI.Linea_Cod,
                            Key .Lotto_Input_Costi = CI.Lotto_Input_Costi,
                            Key .Validita_Inizio = CI.Validita_Inizio,
                            Key .Validita_Fine = CI.Validita_Fine
                        } Into g = Group
                       Select New With {
                          .Id_CDG = 0,
                          .Id_CDG_Dettagli = 0,
                          .Piva = x.Piva,
                          .Sa_Cod = x.Sa_Cod,
                          .Campo_Cod = x.Campo_Cod,
                          .Appezza = x.Appezza,
                          .Id_Reg = x.Id_Reg,
                          .Id_Cod_reg_impianti_codici = x.Id_Cod_reg_impianti_codici,
                          .Id_Imputazione = x.Id_Imputazione,
                          .Macchine_Cod = x.Macchine_Cod,
                          .Linea_Cod = x.Linea_Cod,
                          .Lotto_Input_Costi = x.Lotto_Input_Costi,
                          .Validita_Inizio = x.Validita_Inizio,
                          .Validita_Fine = x.Validita_Fine,
                          .Valore_Totale = g.Sum(Function(r) r.CI.Valore)
                      }

                    Dim myList_PrimaQuery_Impianti = PrimaQuery_Impianti.ToList()

                    Dim myListImpianti As New List(Of Object)

                    For Each obj In myList_PrimaQuery_Impianti

                        'Impianti
                        Dim DettagliElem_Impianti =
                            From Reg_Impianti In GiasContext.Reg_Impianti
                            Join Imprese_Progetti In GiasContext.Imprese_Progetti
                            On Imprese_Progetti.Piva Equals Reg_Impianti.PIVA And
                            Imprese_Progetti.Sa_Cod Equals Reg_Impianti.SA_COD And
                            Imprese_Progetti.Appezza Equals Reg_Impianti.APPEZZA And
                            Imprese_Progetti.Id_Reg Equals Reg_Impianti.ID_REG
                            Group Join CDG_Dettagli In GiasContext.CDG_Dettagli.Where(Function(x) x.Piva_Superuser.Equals(Piva_SuperUser))
                            On Imprese_Progetti.Piva Equals CDG_Dettagli.Piva _
                            And Imprese_Progetti.Sa_Cod Equals CDG_Dettagli.Sa_Cod _
                            And Imprese_Progetti.Appezza Equals CDG_Dettagli.Appezza _
                            And Imprese_Progetti.Id_Reg Equals CDG_Dettagli.Id_Reg _
                            And Imprese_Progetti.Progetto_Cod Equals CDG_Dettagli.Progetto_Cod
                            Into CDG_Dettagli_Group = Group
                            From _CDG_Dettagli_Group In CDG_Dettagli_Group.DefaultIfEmpty()
                            Group Join CDG_Testata In GiasContext.CDG_Testata.Where(Function(x) x.Piva_Superuser.Equals(Piva_SuperUser) AndAlso x.Budget = 0)
                            On CDG_Testata.Piva Equals _CDG_Dettagli_Group.Piva _
                            And CDG_Testata.Id_CDG Equals _CDG_Dettagli_Group.Id_CDG
                            Into CDG_Testata_Group = Group
                            From _CDG_Testata_Group In CDG_Testata_Group.DefaultIfEmpty()
                            Join Appezzamento In GiasContext.Appezzamento
                            On Appezzamento.PIVA Equals Reg_Impianti.PIVA And
                               Appezzamento.SA_COD Equals Reg_Impianti.SA_COD And
                               Appezzamento.APPEZZA Equals Reg_Impianti.APPEZZA
                            Join Centri_Aziendali In GiasContext.Centri_Aziendali
                            On Centri_Aziendali.PIVA Equals Appezzamento.PIVA And
                               Centri_Aziendali.sa_cod Equals Appezzamento.SA_COD
                            Group Join Cultivar In GiasContext.Cultivar
                            On Cultivar.Cul_Cod Equals Reg_Impianti.CUL_COD Into Cultivar_Group = Group
                            From _Cultivar_Group In Cultivar_Group.DefaultIfEmpty()
                            Group Join Specie In GiasContext.SpecieVegetali
                            On Specie.Veg_Cod Equals _Cultivar_Group.Veg_Cod Into Specie_Group = Group
                            From _Specie_Group In Specie_Group.DefaultIfEmpty()
                            Group Join Reg_Impianti_Distinta In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Distinta_Chiusa))
                            On Reg_Impianti_Distinta.PIVA Equals Imprese_Progetti.Piva And
                            Reg_Impianti_Distinta.sa_cod Equals Imprese_Progetti.Sa_Cod And
                            Reg_Impianti_Distinta.appezza Equals Imprese_Progetti.Appezza And
                            Reg_Impianti_Distinta.Id_Reg Equals Imprese_Progetti.Id_Reg And
                            Reg_Impianti_Distinta.Progetto_Cod Equals Imprese_Progetti.Progetto_Cod
                            Into Reg_Impianti_Distinta_Group = Group
                            From _Reg_Impianti_Distinta_Group In Reg_Impianti_Distinta_Group.DefaultIfEmpty()
                            Group Join Reg_Impianti_CodiceImp In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Codice_Impianto))
                            On Reg_Impianti_CodiceImp.PIVA Equals Reg_Impianti.PIVA And
                            Reg_Impianti_CodiceImp.sa_cod Equals Reg_Impianti.SA_COD And
                            Reg_Impianti_CodiceImp.appezza Equals Reg_Impianti.APPEZZA And
                            Reg_Impianti_CodiceImp.Id_Reg Equals Reg_Impianti.ID_REG
                        Into Reg_Impianti_CodiceImp_Group = Group
                            From _Reg_Impianti_CodiceImp_Group In Reg_Impianti_CodiceImp_Group.DefaultIfEmpty()
                            Group Join Campi In GiasContext.Campi
                            On Campi.Piva Equals Appezzamento.PIVA _
                            And Campi.Sa_Cod Equals Appezzamento.SA_COD _
                            And Campi.Campo_Cod Equals Appezzamento.Campo_Cod
                            Into Campi_Group = Group
                            From _Campi_Group In Campi_Group.DefaultIfEmpty()
                            Where
                            Reg_Impianti.PIVA = CStr(obj.Piva) _
                            AndAlso Reg_Impianti.SA_COD = CStr(obj.Sa_Cod) _
                            AndAlso Reg_Impianti.APPEZZA = CStr(obj.Appezza) _
                            AndAlso Reg_Impianti.ID_REG = CStr(obj.Id_Reg) _
                            AndAlso strCau_Progetto.Contains(Imprese_Progetti.Cau_Progetto) _
                            AndAlso (_CDG_Testata_Group.Id_Agenda = id_agenda_cdg)
                            Select New With {
                           .Id_CDG = If(_CDG_Dettagli_Group Is Nothing, 0, _CDG_Dettagli_Group.Id_CDG),
                           .Id_CDG_Dettagli = If(_CDG_Dettagli_Group Is Nothing, 0, _CDG_Dettagli_Group.Id_CDG_Dettagli),
                           .Piva = Reg_Impianti.PIVA,
                           .Sa_Cod = Reg_Impianti.SA_COD,
                           .Campo_Cod = obj.Campo_Cod,
                           .Campo_Des = If(_Campi_Group Is Nothing, "", _Campi_Group.Campo_Des),
                           .Sa_Nome = Centri_Aziendali.sa_nome,
                           .Appezza = Reg_Impianti.APPEZZA,
                           .Id_Destinazione = Reg_Impianti.ID_REG,
                           .Id_Cod_reg_impianti_codici = obj.Id_Cod_reg_impianti_codici,
                           .Id_Imputazione = obj.Id_Imputazione,
                           .Macchine_Cod = obj.Macchine_Cod,
                           .Linea_Cod = obj.Linea_Cod,
                           .Lotto_Input_Costi = obj.Lotto_Input_Costi,
                           .Valore = If(_CDG_Dettagli_Group Is Nothing, 0, _CDG_Dettagli_Group.Valore),
                           .ValoreMassimo = obj.Valore_Totale,
                           .Validita_Inizio = obj.Validita_Inizio,
                           .Validita_Fine = obj.Validita_Fine,
                           .Validita_Inizio_Distinta = Imprese_Progetti.Validita_Inizio,
                           .Validita_Fine_Distinta = Imprese_Progetti.Validita_Fine,
                           .Key = "",
                           .Veg_Cod = If(_Specie_Group Is Nothing, 0, _Specie_Group.Veg_Cod),
                           .Veg_Des = If(_Specie_Group Is Nothing, "", _Specie_Group.Veg_Des),
                           .Cul_Cod = If(_Cultivar_Group Is Nothing, 0, _Cultivar_Group.Cul_Cod),
                           .Cul_Des = If(_Cultivar_Group Is Nothing, Str_TerrenoNudo, _Cultivar_Group.Cul_Des),
                           .Descrizione = Appezzamento.APP_NOME,
                           .Progetto_Cod = Imprese_Progetti.Progetto_Cod,
                           .Progetto_Nome = Imprese_Progetti.Progetto_Nome,
                           .Progetto_Des = Imprese_Progetti.Progetto_Des,
                           .Superficie = Reg_Impianti.Sup_Imp,
                           .Codice_Impianto = If(_Reg_Impianti_CodiceImp_Group Is Nothing, "", _Reg_Impianti_CodiceImp_Group.val_cod),
                           .Flag_Distinta_Chiusa = If(_Reg_Impianti_Distinta_Group Is Nothing OrElse _Reg_Impianti_Distinta_Group.val_cod = 0, False, True)
                            }






                        ''Impianti
                        'Dim DettagliElem_Impianti =
                        '    From Reg_Impianti In GiasContext.Reg_Impianti
                        '    Join Imprese_Progetti In GiasContext.Imprese_Progetti
                        '    On Imprese_Progetti.Piva Equals Reg_Impianti.PIVA And
                        '    Imprese_Progetti.Sa_Cod Equals Reg_Impianti.SA_COD And
                        '    Imprese_Progetti.Appezza Equals Reg_Impianti.APPEZZA And
                        '    Imprese_Progetti.Id_Reg Equals Reg_Impianti.ID_REG
                        '    Group Join CDG_Dettagli In GiasContext.CDG_Dettagli.Where(Function(x) x.Piva_Superuser.Equals(Piva_SuperUser))
                        '    On Imprese_Progetti.Piva Equals CDG_Dettagli.Piva _
                        '    And Imprese_Progetti.Sa_Cod Equals CDG_Dettagli.Sa_Cod _
                        '    And Imprese_Progetti.Appezza Equals CDG_Dettagli.Appezza _
                        '    And Imprese_Progetti.Id_Reg Equals CDG_Dettagli.Id_Reg _
                        '    And Imprese_Progetti.Progetto_Cod Equals CDG_Dettagli.Progetto_Cod
                        '    Into CDG_Dettagli_Group = Group
                        '    From _CDG_Dettagli_Group In CDG_Dettagli_Group.DefaultIfEmpty()
                        '    Group Join CDG_Testata In GiasContext.CDG_Testata.Where(Function(x) x.Piva_Superuser.Equals(Piva_SuperUser) AndAlso x.Budget = 0)
                        '    On CDG_Testata.Piva Equals _CDG_Dettagli_Group.Piva _
                        '    And CDG_Testata.Id_CDG Equals _CDG_Dettagli_Group.Id_CDG
                        '    Into CDG_Testata_Group = Group
                        '    From _CDG_Testata_Group In CDG_Testata_Group.DefaultIfEmpty()
                        '    Join Appezzamento In GiasContext.Appezzamento
                        '    On Appezzamento.PIVA Equals Reg_Impianti.PIVA And
                        '       Appezzamento.SA_COD Equals Reg_Impianti.SA_COD And
                        '       Appezzamento.APPEZZA Equals Reg_Impianti.APPEZZA
                        '    Join Centri_Aziendali In GiasContext.Centri_Aziendali
                        '    On Centri_Aziendali.PIVA Equals Appezzamento.PIVA And
                        '       Centri_Aziendali.sa_cod Equals Appezzamento.SA_COD
                        '    Group Join Cultivar In GiasContext.Cultivar
                        '    On Cultivar.Cul_Cod Equals Reg_Impianti.CUL_COD Into Cultivar_Group = Group
                        '    From _Cultivar_Group In Cultivar_Group.DefaultIfEmpty()
                        '    Group Join Specie In GiasContext.SpecieVegetali
                        '    On Specie.Veg_Cod Equals _Cultivar_Group.Veg_Cod Into Specie_Group = Group
                        '    From _Specie_Group In Specie_Group.DefaultIfEmpty()
                        '    Group Join Reg_Impianti_Distinta In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Distinta_Chiusa))
                        '    On Reg_Impianti_Distinta.PIVA Equals Imprese_Progetti.Piva And
                        '    Reg_Impianti_Distinta.sa_cod Equals Imprese_Progetti.Sa_Cod And
                        '    Reg_Impianti_Distinta.appezza Equals Imprese_Progetti.Appezza And
                        '    Reg_Impianti_Distinta.Id_Reg Equals Imprese_Progetti.Id_Reg And
                        '    Reg_Impianti_Distinta.Progetto_Cod Equals Imprese_Progetti.Progetto_Cod
                        '    Into Reg_Impianti_Distinta_Group = Group
                        '    From _Reg_Impianti_Distinta_Group In Reg_Impianti_Distinta_Group.DefaultIfEmpty()
                        '    Group Join Reg_Impianti_CodiceImp In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Codice_Impianto))
                        '    On Reg_Impianti_CodiceImp.PIVA Equals Reg_Impianti.PIVA And
                        '    Reg_Impianti_CodiceImp.sa_cod Equals Reg_Impianti.SA_COD And
                        '    Reg_Impianti_CodiceImp.appezza Equals Reg_Impianti.APPEZZA And
                        '    Reg_Impianti_CodiceImp.Id_Reg Equals Reg_Impianti.ID_REG
                        'Into Reg_Impianti_CodiceImp_Group = Group
                        '    From _Reg_Impianti_CodiceImp_Group In Reg_Impianti_CodiceImp_Group.DefaultIfEmpty()
                        '    Group Join Campi In GiasContext.Campi
                        '    On Campi.Piva Equals Appezzamento.PIVA _
                        '    And Campi.Sa_Cod Equals Appezzamento.SA_COD _
                        '    And Campi.Campo_Cod Equals Appezzamento.Campo_Cod
                        '    Into Campi_Group = Group
                        '    From _Campi_Group In Campi_Group.DefaultIfEmpty()
                        '    Where
                        '    Reg_Impianti.PIVA = CStr(obj.Piva) _
                        '    AndAlso Reg_Impianti.SA_COD = CStr(obj.Sa_Cod) _
                        '    AndAlso Reg_Impianti.APPEZZA = CStr(obj.Appezza) _
                        '    AndAlso Reg_Impianti.ID_REG = CStr(obj.Id_Reg) _
                        '    AndAlso (split = 1 OrElse Imprese_Progetti.Validita_Inizio <= data_movimento) _
                        '    AndAlso (split = 1 OrElse Imprese_Progetti.Validita_Fine >= data_movimento) _
                        '    AndAlso strCau_Progetto.Contains(Imprese_Progetti.Cau_Progetto) _
                        '    AndAlso (_CDG_Testata_Group.Id_Agenda = id_agenda_cdg)
                        '    Select New With {
                        '   .Id_CDG = If(_CDG_Dettagli_Group Is Nothing, 0, _CDG_Dettagli_Group.Id_CDG),
                        '   .Id_CDG_Dettagli = If(_CDG_Dettagli_Group Is Nothing, 0, _CDG_Dettagli_Group.Id_CDG_Dettagli),
                        '   .Piva = Reg_Impianti.PIVA,
                        '   .Sa_Cod = Reg_Impianti.SA_COD,
                        '   .Campo_Cod = obj.Campo_Cod,
                        '   .Campo_Des = If(_Campi_Group Is Nothing, "", _Campi_Group.Campo_Des),
                        '   .Sa_Nome = Centri_Aziendali.sa_nome,
                        '   .Appezza = Reg_Impianti.APPEZZA,
                        '   .Id_Destinazione = Reg_Impianti.ID_REG,
                        '   .Id_Cod_reg_impianti_codici = obj.Id_Cod_reg_impianti_codici,
                        '   .Id_Imputazione = obj.Id_Imputazione,
                        '   .Macchine_Cod = obj.Macchine_Cod,
                        '   .Linea_Cod = obj.Linea_Cod,
                        '   .Lotto_Input_Costi = obj.Lotto_Input_Costi,
                        '   .Valore = If(_CDG_Dettagli_Group Is Nothing, 0, _CDG_Dettagli_Group.Valore),
                        '   .ValoreMassimo = obj.Valore_Totale,
                        '   .Validita_Inizio = obj.Validita_Inizio,
                        '   .Validita_Fine = obj.Validita_Fine,
                        '   .Validita_Inizio_Distinta = Imprese_Progetti.Validita_Inizio,
                        '   .Validita_Fine_Distinta = Imprese_Progetti.Validita_Fine,
                        '   .Key = "",
                        '   .Veg_Cod = If(_Specie_Group Is Nothing, 0, _Specie_Group.Veg_Cod),
                        '   .Veg_Des = If(_Specie_Group Is Nothing, "", _Specie_Group.Veg_Des),
                        '   .Cul_Cod = If(_Cultivar_Group Is Nothing, 0, _Cultivar_Group.Cul_Cod),
                        '   .Cul_Des = If(_Cultivar_Group Is Nothing, Str_TerrenoNudo, _Cultivar_Group.Cul_Des),
                        '   .Descrizione = Appezzamento.APP_NOME,
                        '   .Progetto_Cod = Imprese_Progetti.Progetto_Cod,
                        '   .Progetto_Nome = Imprese_Progetti.Progetto_Nome,
                        '   .Progetto_Des = Imprese_Progetti.Progetto_Des,
                        '   .Superficie = Reg_Impianti.Sup_Imp,
                        '   .Codice_Impianto = If(_Reg_Impianti_CodiceImp_Group Is Nothing, "", _Reg_Impianti_CodiceImp_Group.val_cod),
                        '   .Flag_Distinta_Chiusa = If(_Reg_Impianti_Distinta_Group Is Nothing OrElse _Reg_Impianti_Distinta_Group.val_cod = 0, False, True)
                        '    }
                        ' EVENTUALMENTE Sostituisce da Order By alla fine 
                        ' Group By x = New With {
                        '     Key .Id_CDG = 0,
                        '     Key .Id_CDG_Dettagli = 0,
                        '     Key .Piva = CI.Piva,
                        '     Key .Sa_Cod = Centri_Aziendali.sa_cod,
                        '     Key .Sa_Nome = Centri_Aziendali.sa_nome,
                        '     Key .Appezza = CI.Appezza,
                        '     Key .Id_Destinazione = CI.Id_Reg,
                        '     Key .Id_Cod_reg_impianti_codici = CI.Id_Cod_reg_impianti_codici,
                        '     Key .Id_Imputazione = CI.Id_Imputazione,
                        '     Key .Macchine_Cod = CI.Macchine_Cod,
                        '     Key .Linea_Cod = CI.Linea_Cod,
                        '     Key .Lotto_Input_Costi = CI.Lotto_Input_Costi,
                        '     Key .Validita_Inizio = CI.Validita_Inizio,
                        '     Key .Validita_Fine = CI.Validita_Fine,
                        '     Key .Key = "",
                        '     Key .Veg_Cod = If(_Specie_Group Is Nothing, 0, _Specie_Group.Veg_Cod),
                        '     Key .Veg_Des = If(_Specie_Group Is Nothing, "", _Specie_Group.Veg_Des),
                        '     Key .Cul_Cod = If(_Cultivar_Group Is Nothing, 0, _Cultivar_Group.Cul_Cod),
                        '     Key .Cul_Des = If(_Cultivar_Group Is Nothing, "Terreno Nudo", _Cultivar_Group.Cul_Des),
                        '     Key .Descrizione = Appezzamento.APP_NOME,
                        '     Key .Progetto_Cod = Imprese_Progetti.Progetto_Cod,
                        '     Key .Progetto_Nome = Imprese_Progetti.Progetto_Nome,
                        '     Key .Progetto_Des = Imprese_Progetti.Progetto_Des,
                        '     Key .Superficie = Reg_Impianti.Sup_Imp
                        '} Into g = Group
                        ' Order By g.Sum(Function(r) r.CI.Valore) Descending
                        ' Select New With {
                        '     .Id_CDG = x.Id_CDG,
                        '     .Id_CDG_Dettagli = x.Id_CDG_Dettagli,
                        '     .Piva = x.Piva,
                        '     .Sa_Cod = x.Sa_Cod,
                        '     .Sa_Nome = x.Sa_Nome,
                        '     .Appezza = x.Appezza,
                        '     .Id_Destinazione = x.Id_Destinazione,
                        '     .Id_Cod_reg_impianti_codici = x.Id_Cod_reg_impianti_codici,
                        '     .Id_Imputazione = x.Id_Imputazione,
                        '     .Macchine_Cod = x.Macchine_Cod,
                        '     .Linea_Cod = x.Linea_Cod,
                        '     .Lotto_Input_Costi = x.Lotto_Input_Costi,
                        '     .Valore = g.Sum(Function(r) r.CI.Valore),
                        '     .Validita_Inizio = x.Validita_Inizio,
                        '     .Validita_Fine = x.Validita_Fine,
                        '     .Key = "",
                        '     .Veg_Cod = x.Veg_Cod,
                        '     .Veg_Des = x.Veg_Des,
                        '     .Cul_Cod = x.Cul_Cod,
                        '     .Cul_Des = x.Cul_Des,
                        '     .Descrizione = x.Descrizione,
                        '     .Progetto_Cod = g.Min(Function(r) r.Imprese_Progetti.Progetto_Cod),
                        '     .Progetto_Nome = g.Min(Function(r) r.Imprese_Progetti.Progetto_Nome),
                        '     .Progetto_Des = g.Min(Function(r) r.Imprese_Progetti.Progetto_Des),
                        '     .Superficie = x.Superficie
                        '}

                        '                        Order By CI.Valore Descending

                        ' Compongo la chiave
                        Dim myImpianti_Dettagli_Elem = DettagliElem_Impianti.FirstOrDefault()
                        myImpianti_Dettagli_Elem.Valore = Decimal.Round(Convert.ToDecimal(myImpianti_Dettagli_Elem.Valore), 2)
                        myImpianti_Dettagli_Elem.Key = myImpianti_Dettagli_Elem.Piva & "-" & myImpianti_Dettagli_Elem.Sa_Cod.ToString() & "-" & myImpianti_Dettagli_Elem.Campo_Cod.ToString() & "-" & myImpianti_Dettagli_Elem.Appezza.ToString() & "-" & myImpianti_Dettagli_Elem.Id_Destinazione.ToString()
                        myListImpianti.Add(myImpianti_Dettagli_Elem)

                    Next

                    kendo_Impianti_Dettagli = JsonConvert.SerializeObject(myListImpianti, Formatting.None, serializerSettings)
                    myListImpianti = Nothing


                End If



                If String.IsNullOrEmpty(kendo_Impianti_Dettagli) OrElse kendo_Impianti_Dettagli = "[]" Then

                    'Progetti
                    Dim DettagliElem_Progetti =
                   From CI In GiasContext.CDG_Dettagli
                   Join CDG_Testata In GiasContext.CDG_Testata
                     On CDG_Testata.Piva Equals CI.Piva _
                     And CDG_Testata.Id_CDG Equals CI.Id_CDG
                   Join Imputazioni In GiasContext.Imputazioni
                     On Imputazioni.Piva Equals CI.Piva _
                     And Imputazioni.Imputazione_Cod Equals CI.Id_Imputazione
                   Join Imputazioni_Tipi In GiasContext.Imputazioni_Tipi
                     On Imputazioni_Tipi.Piva_SuperUser Equals CI.Piva_Superuser And
                        Imputazioni_Tipi.Piva Equals CI.Piva And
                        Imputazioni_Tipi.Tipo_Imputazione Equals Imputazioni.Tipo_Imputazione
                   Join Imputazioni_Classi In GiasContext.Imputazioni_Classi
                     On Imputazioni_Classi.Piva_SuperUser Equals CI.Piva_Superuser And
                        Imputazioni_Classi.Piva Equals CI.Piva And
                        Imputazioni_Classi.Imputazione_Classe_Cod Equals Imputazioni.Imputazione_Classe_Cod
                   Where
                  (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                  AndAlso (CI.Piva.Equals(piva)) _
                  AndAlso (CDG_Testata.Id_Agenda = id_agenda_cdg) _
                  AndAlso (CI.Id_Imputazione <> 0)
                   Select New With {
                       .Id_CDG = 0,
                       .Id_CDG_Dettagli = 0,
                       .Piva = CI.Piva,
                       .Sa_Cod = CI.Sa_Cod,
                       .Appezza = CI.Appezza,
                       .Id_Destinazione = CI.Id_Reg,
                       .Id_Cod_reg_impianti_codici = CI.Id_Cod_reg_impianti_codici,
                       .Imputazione_Cod = CI.Id_Imputazione,
                       .Macchine_Cod = CI.Macchine_Cod,
                       .Linea_Cod = CI.Linea_Cod,
                       .Lotto_Input_Costi = CI.Lotto_Input_Costi,
                       .Valore = CI.Valore,
                       .Validita_Inizio = CI.Validita_Inizio,
                       .Validita_Fine = CI.Validita_Fine,
                       .Tipo_Imputazione = Imputazioni.Tipo_Imputazione,
                       .Tipo_Imputazione_Des = Imputazioni_Tipi.Tipo_Imputazione_Des,
                       .Imputazione_Classe_Cod = Imputazioni.Imputazione_Classe_Cod,
                       .Imputazione_Classe_Des = Imputazioni_Classi.Imputazione_Classe_Des,
                       .Imputazione_Nome = Imputazioni.Imputazione_Nome
                  }


                    ' Compongo la chiave
                    Dim myList_Progetti_Dettagli = DettagliElem_Progetti.Distinct().ToList()

                    kendo_Progetti_Dettagli = JsonConvert.SerializeObject(myList_Progetti_Dettagli, Formatting.None, serializerSettings)
                    DettagliElem_Progetti = Nothing
                    myList_Progetti_Dettagli = Nothing







                    If String.IsNullOrEmpty(kendo_Progetti_Dettagli) OrElse kendo_Progetti_Dettagli = "[]" Then




                        'Macchine
                        Dim DettagliElem_Macchine =
                               From CI In GiasContext.CDG_Dettagli
                               Join CDG_Testata In GiasContext.CDG_Testata
                                 On CDG_Testata.Piva Equals CI.Piva _
                                 And CDG_Testata.Id_CDG Equals CI.Id_CDG
                               Join Parco_Macchine In GiasContext.Parco_Macchine
                                 On Parco_Macchine.Mac_Cod Equals CI.Macchine_Cod
                               Group Join Centri_Aziendali In GiasContext.Centri_Aziendali
                                 On Centri_Aziendali.PIVA Equals Parco_Macchine.Piva And
                                    Centri_Aziendali.sa_cod Equals Parco_Macchine.Sa_Cod Into Centri_Aziendali_Group = Group
                               From _Centri_Aziendali In Centri_Aziendali_Group.DefaultIfEmpty()
                               Where
                              (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                              AndAlso (CI.Piva.Equals(piva)) _
                              AndAlso (CDG_Testata.Id_Agenda = id_agenda_cdg) _
                              AndAlso (CI.Macchine_Cod <> 0)
                               Select New With {
                                   .Id_CDG = 0,
                                   .Id_CDG_Dettagli = 0,
                                   .Piva = CI.Piva,
                                   .Appezza = CI.Appezza,
                                   .Id_Destinazione = CI.Id_Reg,
                                   .Id_Cod_reg_impianti_codici = CI.Id_Cod_reg_impianti_codici,
                                   .Id_Imputazione = CI.Id_Imputazione,
                                   .Macchine_Cod = CI.Macchine_Cod,
                                   .Linea_Cod = CI.Linea_Cod,
                                   .Lotto_Input_Costi = CI.Lotto_Input_Costi,
                                   .Valore = CI.Valore,
                                   .Validita_Inizio = CI.Validita_Inizio,
                                   .Validita_Fine = CI.Validita_Fine,
                                   .Sa_Cod = If(_Centri_Aziendali Is Nothing, 0, _Centri_Aziendali.sa_cod),
                                   .Sa_Nome = If(_Centri_Aziendali Is Nothing, "", _Centri_Aziendali.sa_nome),
                                   .Tipo = Parco_Macchine.Tipo,
                                   .Tipo_Des = If(Parco_Macchine.Tipo = 0, "Agricolo Zootecnico", If(Parco_Macchine.Tipo = 1, "Industriale", "Commerciale")),
                                   .CLASS_CODE_ROOT = Left(Parco_Macchine.Class_Code, 2),
                                   .CLASS_DESC = "",
                                   .Mac_Cod = Parco_Macchine.Mac_Cod,
                                   .Mac_Des = Parco_Macchine.Mac_Des
                              }


                        ' Compongo la chiave
                        Dim myList_Macchine_Dettagli = DettagliElem_Macchine.Distinct().ToList()

                        kendo_Macchine_Dettagli = JsonConvert.SerializeObject(myList_Macchine_Dettagli, Formatting.None, serializerSettings)
                        DettagliElem_Macchine = Nothing
                        myList_Macchine_Dettagli = Nothing




                        If String.IsNullOrEmpty(kendo_Macchine_Dettagli) OrElse kendo_Macchine_Dettagli = "[]" Then


                            'Linee Produzione
                            Dim DettagliElem_Linee =
                               From CI In GiasContext.CDG_Dettagli
                               Join CDG_Testata In GiasContext.CDG_Testata
                                 On CDG_Testata.Piva Equals CI.Piva _
                                 And CDG_Testata.Id_CDG Equals CI.Id_CDG
                               Join Linee_Produzioni In GiasContext.Linee_Produzioni
                                 On Linee_Produzioni.Piva Equals CI.Piva And
                                    Linee_Produzioni.Linea_Cod Equals CI.Linea_Cod
                               Join Linee_Produzioni_Mix In GiasContext.Linee_Produzioni_Mix
                                 On Linee_Produzioni_Mix.Piva Equals CI.Piva And
                                    Linee_Produzioni_Mix.Linea_Cod Equals CI.Linea_Cod
                               Group Join Specie In GiasContext.SpecieVegetali
                                 On Specie.Veg_Cod Equals Linee_Produzioni_Mix.Veg_Cod Into Specie_Group = Group
                               From _Specie_Group In Specie_Group.DefaultIfEmpty()
                               Group Join Cultivar In GiasContext.Cultivar
                                 On Linee_Produzioni_Mix.Cul_Cod Equals Cultivar.Cul_Cod Into Cultivar_Group = Group
                               From _Cultivar_Group In Cultivar_Group.DefaultIfEmpty()
                               Where
                              (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                              AndAlso (CI.Piva.Equals(piva)) _
                              AndAlso (CDG_Testata.Id_Agenda = id_agenda_cdg) _
                              AndAlso (CI.Linea_Cod <> 0)
                               Select New With {
                                   .Id_CDG = 0,
                                   .Id_CDG_Dettagli = 0,
                                   .Piva = CI.Piva,
                                   .Appezza = CI.Appezza,
                                   .Id_Destinazione = CI.Id_Reg,
                                   .Id_Cod_reg_impianti_codici = CI.Id_Cod_reg_impianti_codici,
                                   .Id_Imputazione = CI.Id_Imputazione,
                                   .Macchine_Cod = CI.Macchine_Cod,
                                   .Linea_Cod = CI.Linea_Cod,
                                   .Lotto_Input_Costi = CI.Lotto_Input_Costi,
                                   .Valore = CI.Valore,
                                   .Validita_Inizio = CI.Validita_Inizio,
                                   .Validita_Fine = CI.Validita_Fine,
                                   .Veg_Cod = Linee_Produzioni_Mix.Veg_Cod,
                                   .Veg_Des = If(_Specie_Group Is Nothing, "", _Specie_Group.Veg_Des),
                                   .Cul_Cod = Linee_Produzioni_Mix.Cul_Cod,
                                   .Cul_Des = If(_Cultivar_Group Is Nothing, "", _Cultivar_Group.Cul_Des),
                                   .Linea_Des = Linee_Produzioni.Linea_Des & " (" & Linee_Produzioni.Linea_Cod_Des & ")",
                                   .Regolamento_Cod = Linee_Produzioni.Reg_Cod
                              }


                            ' Compongo la chiave
                            Dim myList_Linee_Dettagli = DettagliElem_Linee.Distinct().ToList()

                            kendo_Linee_Dettagli = JsonConvert.SerializeObject(myList_Linee_Dettagli, Formatting.None, serializerSettings)
                            DettagliElem_Linee = Nothing
                            myList_Linee_Dettagli = Nothing



                            If String.IsNullOrEmpty(kendo_Linee_Dettagli) OrElse kendo_Linee_Dettagli = "[]" Then


                                'Zoo
                                'Lettura delle giacenze zoo
                                Dim ZooBIZ As New AgronicaCoreAnagrafeDAL.Zoo_Animali
                                Dim dt = ZooBIZ.Leggi_Giacenze(piva, 0, 0, 0, 0, data_movimento & " 23:59:59", objParametri, True)


                                dt.Columns.Add(New DataColumn("Valore", GetType(Decimal)))
                                dt.Columns.Add(New DataColumn("Selected", GetType(Boolean)))

                                'Lettura dei dati salvati
                                Dim DettagliElem_Zoo =
                                   From CI In GiasContext.CDG_Dettagli
                                   Join CDG_Testata In GiasContext.CDG_Testata
                                     On CDG_Testata.Piva Equals CI.Piva _
                                     And CDG_Testata.Id_CDG Equals CI.Id_CDG
                                   Where
                                  (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                                  AndAlso (CI.Piva.Equals(piva)) _
                                  AndAlso (CDG_Testata.Id_Agenda = id_agenda_cdg) _
                                  AndAlso (CI.Cod_Animale <> 0)
                                   Select New With {
                                       .Id_Agenda = CDG_Testata.Id_Agenda,
                                       .Id_CDG = CDG_Testata.Id_CDG,
                                       .Id_CDG_Dettagli = CI.Id_CDG_Dettagli,
                                       .Cod_Animale = CI.Cod_Animale,
                                       .Valore = CI.Valore
                                  }


                                Dim myList_Zoo_Dettagli = DettagliElem_Zoo.Distinct().ToList()
                                Dim dr_search As DataRow()

                                For Each obj In myList_Zoo_Dettagli

                                    dr_search = dt.Select("Cod_Animale = " & obj.Cod_Animale)

                                    If dr_search.Length <> 0 Then

                                        dr_search(0).Item("Valore") = obj.Valore
                                        dr_search(0).Item("Selected") = True

                                    End If

                                    'strFiltro = strFiltro & IIf(strFiltro = "", "", " OR ") & " Cod_Animale = " & obj.Cod_Animale


                                    '    
                                Next


                                dt.DefaultView.Sort = "Valore Desc"
                                dt = dt.DefaultView.ToTable()

                                kendo_Zoo_Dettagli = Newtonsoft.Json.JsonConvert.SerializeObject(dt)
                                DettagliElem_Zoo = Nothing
                                myList_Zoo_Dettagli = Nothing

                            End If


                        End If



                    End If


                End If

            End If


        End Using

    End Sub




    '##############################################################################################
    Public Function Leggi_CDG_Dettagli_GroupBy_Impianti(ByVal piva As String,
                                                        ByVal id_agenda_cdg As Integer,
                                                        ByVal data_movimento As Date,
                                                        ByVal bDT As Boolean,
                                                        ByRef objParametri As AgronicaCoreParametri,
                                                        ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities
                                                        ) As Object


        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_CDG_Dettagli_GroupBy_Impianti()"

        'Dim gefutils AS New Gias_EF_Utility
        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim strCau_Mov As String() = {CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO, CAU_RILIEVO_RACCOLTA, CAU_LAVORAZIONE}
        Dim strCau_Progetto As String() = {CAU_PROGETTO_PRODUZIONE}

        Dim DT As DataTable

        Dim Residuo As Double = 100
        Dim Counter As Integer = 0

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        'Impianti
        Dim DettagliElem_Impianti =
                   From CI In GiasContext.CDG_Dettagli
                   Join CDG_Testata In GiasContext.CDG_Testata
                     On CDG_Testata.Piva Equals CI.Piva _
                     And CDG_Testata.Id_CDG Equals CI.Id_CDG
                   Join Reg_Impianti In GiasContext.Reg_Impianti
                     On Reg_Impianti.PIVA Equals CI.Piva _
                     And Reg_Impianti.SA_COD Equals CI.Sa_Cod _
                     And Reg_Impianti.APPEZZA Equals CI.Appezza _
                     And Reg_Impianti.ID_REG Equals CI.Id_Reg
                   Join Imprese_Progetti In GiasContext.Imprese_Progetti
                 On CI.Piva Equals Imprese_Progetti.Piva And
                     CI.Sa_Cod Equals Imprese_Progetti.Sa_Cod And
                     CI.Appezza Equals Imprese_Progetti.Appezza And
                     CI.Id_Reg Equals Imprese_Progetti.Id_Reg And
                     CI.Progetto_Cod Equals Imprese_Progetti.Progetto_Cod
                   Join Appezzamento In GiasContext.Appezzamento
                        On Appezzamento.PIVA Equals Reg_Impianti.PIVA And
                           Appezzamento.SA_COD Equals Reg_Impianti.SA_COD And
                           Appezzamento.APPEZZA Equals Reg_Impianti.APPEZZA
                   Join Centri_Aziendali In GiasContext.Centri_Aziendali
                        On Centri_Aziendali.PIVA Equals Appezzamento.PIVA And
                           Centri_Aziendali.sa_cod Equals Appezzamento.SA_COD
                   Group Join Cultivar In GiasContext.Cultivar
                     On Cultivar.Cul_Cod Equals Reg_Impianti.CUL_COD Into Cultivar_Group = Group
                   From _Cultivar_Group In Cultivar_Group.DefaultIfEmpty()
                   Group Join Specie In GiasContext.SpecieVegetali
                     On Specie.Veg_Cod Equals _Cultivar_Group.Veg_Cod Into Specie_Group =
                       Group
                   From _Specie_Group In Specie_Group.DefaultIfEmpty()
                   Group Join Reg_Impianti_Distinta In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Distinta_Chiusa))
                   On Reg_Impianti_Distinta.PIVA Equals Imprese_Progetti.Piva And
                      Reg_Impianti_Distinta.sa_cod Equals Imprese_Progetti.Sa_Cod And
                      Reg_Impianti_Distinta.appezza Equals Imprese_Progetti.Appezza And
                      Reg_Impianti_Distinta.Id_Reg Equals Imprese_Progetti.Id_Reg And
                      Reg_Impianti_Distinta.Progetto_Cod Equals Imprese_Progetti.Progetto_Cod
                      Into Reg_Impianti_Distinta_Group = Group
                   From _Reg_Impianti_Distinta_Group In Reg_Impianti_Distinta_Group.DefaultIfEmpty()
                   Where
                  (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                  AndAlso (CI.Piva.Equals(piva)) _
                  AndAlso (CDG_Testata.Id_Agenda = id_agenda_cdg) _
                  AndAlso (CI.Id_Reg <> 0) _
                  AndAlso strCau_Progetto.Contains(Imprese_Progetti.Cau_Progetto)
                   Group By x = New With {
                      Key .Id_CDG = CI.Id_CDG,
                      Key .Piva = CI.Piva,
                      Key .Sa_Cod = Centri_Aziendali.sa_cod,
                      Key .Sa_Nome = Centri_Aziendali.sa_nome,
                      Key .Appezza = CI.Appezza,
                      Key .Id_Destinazione = CI.Id_Reg,
                      Key .Id_Cod_reg_impianti_codici = CI.Id_Cod_reg_impianti_codici,
                      Key .Id_Imputazione = CI.Id_Imputazione,
                      Key .Macchine_Cod = CI.Macchine_Cod,
                      Key .Linea_Cod = CI.Linea_Cod,
                      Key .Lotto_Input_Costi = CI.Lotto_Input_Costi,
                      Key .Validita_Inizio = CI.Validita_Inizio,
                      Key .Validita_Fine = CI.Validita_Fine,
                      Key .Key = "",
                      Key .Veg_Cod = If(_Specie_Group Is Nothing, 0, _Specie_Group.Veg_Cod),
                      Key .Veg_Des = If(_Specie_Group Is Nothing, "", _Specie_Group.Veg_Des),
                      Key .Cul_Cod = If(_Cultivar_Group Is Nothing, 0, _Cultivar_Group.Cul_Cod),
                      Key .Cul_Des = If(_Cultivar_Group Is Nothing, Str_TerrenoNudo, _Cultivar_Group.Cul_Des),
                      Key .Descrizione = Appezzamento.APP_NOME,
                      Key .Superficie = Reg_Impianti.Sup_Imp,
                      Key .Progetto_Cod = CI.Progetto_Cod,
                      Key .Campo_Cod = CI.Campo_Cod,
                      Key .Flag_Distinta_Chiusa = If(_Reg_Impianti_Distinta_Group Is Nothing OrElse _Reg_Impianti_Distinta_Group.val_cod = 0, False, True)
                  } Into g = Group
                   Order By g.Sum(Function(r) r.CI.Valore) Descending
                   Select New With {
                             .Id_CDG = x.Id_CDG,
                             .Piva = x.Piva,
                             .Sa_Cod = x.Sa_Cod,
                             .Sa_Nome = x.Sa_Nome,
                             .Appezza = x.Appezza,
                             .Id_Destinazione = x.Id_Destinazione,
                             .Id_Cod_reg_impianti_codici = x.Id_Cod_reg_impianti_codici,
                             .Id_Imputazione = x.Id_Imputazione,
                             .Macchine_Cod = x.Macchine_Cod,
                             .Linea_Cod = x.Linea_Cod,
                             .Lotto_Input_Costi = x.Lotto_Input_Costi,
                             .Validita_Inizio = x.Validita_Inizio,
                             .Validita_Fine = x.Validita_Fine,
                             .Key = "",
                             .Veg_Cod = x.Veg_Cod,
                             .Veg_Des = x.Veg_Des,
                             .Cul_Cod = x.Cul_Cod,
                             .Cul_Des = x.Cul_Des,
                             .Descrizione = x.Descrizione,
                             .Superficie = x.Superficie,
                             .Progetto_Cod = x.Progetto_Cod,
                             .Campo_Cod = x.Campo_Cod,
                             .Flag_Distinta_Chiusa = x.Flag_Distinta_Chiusa,
                             .Valore = g.Sum(Function(r) r.CI.Valore)
                  }


        ' Compongo la chiave
        Dim myList_Impianti_Dettagli = DettagliElem_Impianti.Distinct().ToList()
        ' Devo prendere solo la prima ricorrenza perchè per ogni testata le righe di dettaglio sono uguali
        Dim w_ID_CDG = 0
        For Each obj In myList_Impianti_Dettagli
            If w_ID_CDG = 0 Then
                w_ID_CDG = obj.Id_CDG
            End If
            obj.Key = obj.Piva & "-" & obj.Sa_Cod.ToString() & "-" & obj.Appezza.ToString() & "-" & obj.Id_Destinazione.ToString()
            obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
        Next

        Dim myListPrimaTestata = myList_Impianti_Dettagli.Where(Function(x) x.Id_CDG = w_ID_CDG)
        Select Case bDT

            Case True

                Dim ut As New Gias_EF_Utility
                DT = ut.ObjectQueryToDataTable(myListPrimaTestata.ToList())

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return DT

            Case False

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(myListPrimaTestata.ToList(), Formatting.None, serializerSettings)

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return risposta

        End Select

        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

    End Function



    '##############################################################################################
    Public Function Leggi_Impianti_Dettagli_GroupByImpianti(ByVal Piva As String,
                                                            ByVal Raccoglitore_Cod As Integer,
                                                            ByVal Id_Agenda As Integer,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Leggi_Impianti_Dettagli_GroupByImpianti()"


        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            stb.Length = 0
            stb.AppendLine(" SELECT  Distinct Piva, Sa_Cod,  Appezza, Id_Destinazione ")
            stb.AppendLine(" FROM  Mov_Destinazioni ")
            stb.AppendLine(" Where  Tipo_Destinazione = 0 ")

            If Piva <> "" Then
                stb.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Raccoglitore_Cod <> 0 Then
                stb.AppendLine(" AND Mov_Destinazioni.Id_Agenda In (Select Id_Agenda From Agenda Where Piva = '" & Agro_SQL_SaveText(Piva) & "' And Raccoglitore_Cod = " & Agro_SQL_SaveNum(Raccoglitore_Cod) & ")   ")
            ElseIf Id_Agenda <> 0 Then
                stb.AppendLine(" AND Mov_Destinazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function



    '##############################################################################################
    Public Function Leggi_Testata_Eredita(ByVal piva As String,
                                          ByVal id_agenda As Integer,
                                          ByVal raccoglitore_cod As Integer,
                                          ByVal bCDG As Boolean,
                                          ByRef objParametri As AgronicaCoreParametri,
                                                Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing
                                          ) As DataTable


        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Testata_Eredita()"

        'Dim gefutils AS New Gias_EF_Utility
        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim strTab_Imputazione As String() = {"EREDITA"}
        Dim strCau_Mov As String() = {CAU_SCARICO}
        Dim DT As DataTable

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If


        Select Case bCDG

            Case True

                'Eredita come Magazzino
                Dim TestataElem_Eredita =
                   From CI In GiasContext.CDG_Testata
                   Where
                  (CI.Piva_Superuser.Equals(Piva_SuperUser)) _
                  AndAlso (CI.Piva.Equals(piva)) _
                  AndAlso (CI.Id_Agenda = id_agenda) _
                  AndAlso (CI.Vecchio_Tipo_Inser_Dati = 0) _
                  AndAlso (CI.Flag_Movimento_Campagna = 1) _
                  AndAlso strTab_Imputazione.Contains(CI.Tab_Imputazione) _
                  AndAlso (CI.Budget = 0)
                   Select New With {
                       .Id_CDG = CI.Id_CDG,
                       .Data_Inserimento = CI.Data_Inserimento,
                       .Modalita_Imputazione = CI.Modalita_Imputazione,
                       .Elem_Cod = CI.Elem_Cod,
                       .Pro_Cod = CI.Pro_Cod,
                       .Mat_Cod = CI.Mat_Cod,
                       .Lotto = CI.Lotto,
                       .Udm_Cod = CI.Udm_Cod,
                       .Qta = CI.Qta,
                       .Sa_Cod = CI.Sa_Cod,
                       .Tipo_Destinazione = CI.Tipo_Destinazione,
                       .Fabbricato_Cod = CI.Id_Destinazione,
                       .Prodotto_Cod = If(CI.Pro_Cod <> 0, CI.Pro_Cod, CI.Mat_Cod),
                        .OrigineApp = CI.OrigineApp,
                       .APP_CDG_Generale_ID = CI.APP_CDG_Generale_ID,
                       .ID_Attivita = CI.Id_Attivita,
                       .Lav_Cod = CI.Lav_Cod
                        }

                Dim ut As New Gias_EF_Utility
                DT = ut.ObjectQueryToDataTable(TestataElem_Eredita.Distinct().ToList())

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return DT


            Case False

                'Lettura da Agenda Collegata
                Dim TestataElem_Eredita =
                       From CI In GiasContext.Movimenti_dettagli
                       Join Movimenti In GiasContext.Movimenti
                         On CI.PIVA Equals Movimenti.PIVA And
                            CI.Id_Agenda Equals Movimenti.Id_Agenda And
                            CI.Id_Mov Equals Movimenti.Id_Mov
                       Join Agenda In GiasContext.Agenda
                         On Agenda.PIVA Equals Movimenti.PIVA And
                            Agenda.Id_Agenda Equals Movimenti.Id_Agenda
                       Group Join Mov_Destinazioni In GiasContext.Mov_Destinazioni
                         On CI.PIVA Equals Mov_Destinazioni.Piva And
                            CI.Id_Agenda Equals Mov_Destinazioni.Id_Agenda And
                            CI.Id_Mov Equals Mov_Destinazioni.Id_Mov And
                            CI.Id_Mov_Det Equals Mov_Destinazioni.Id_Mov_Det Into Mov_Destinazioni_Group = Group
                       From _Mov_Destinazioni_Group In Mov_Destinazioni_Group.DefaultIfEmpty()
                       Where
                          (Agenda.PIVA.Equals(piva)) _
                      AndAlso (CI.Elem_Cod <> 0) _
                      AndAlso (CI.Elem_Cod <> CostantiPersonalizzate.CAT_MAG_SERVIZI_PROFESSIONALI) _
                      AndAlso strCau_Mov.Contains(Movimenti.Cau_Mov)
                       Select New With {
                           .Id_Agenda = Agenda.Id_Agenda,
                           .Raccoglitore_Cod = Agenda.Raccoglitore_Cod,
                           .Lav_Cod = Agenda.Lav_Cod,
                           .Id_CDG = 0,
                           .Data_Inserimento = Movimenti.Data_Movimento,
                           .Modalita_Imputazione = 0,
                           .Elem_Cod = CI.Elem_Cod,
                           .Pro_Cod = CI.Pro_Cod,
                           .Mat_Cod = CI.Mat_Cod,
                           .Lotto = CI.Lotto,
                           .Udm_Cod = CI.Udm_Cod,
                           .Qta = CI.Qta,
                           .Sa_Cod = CI.Sa_Cod,
                           .Tipo_Destinazione = If(_Mov_Destinazioni_Group Is Nothing, 0, _Mov_Destinazioni_Group.Tipo_Destinazione),
                           .Fabbricato_Cod = If(_Mov_Destinazioni_Group Is Nothing, 0, _Mov_Destinazioni_Group.Id_Destinazione),
                           .Prodotto_Cod = If(CI.Pro_Cod <> 0, CI.Pro_Cod, CI.Mat_Cod)
                            }


                'Filtro
                Select Case raccoglitore_cod

                    Case 0

                        If id_agenda <> 0 Then
                            TestataElem_Eredita = TestataElem_Eredita.Where(Function(x) x.Id_Agenda = id_agenda)
                        End If

                    Case Else

                        TestataElem_Eredita = TestataElem_Eredita.Where(Function(x) x.Raccoglitore_Cod = raccoglitore_cod)

                End Select

                Dim ut As New Gias_EF_Utility
                DT = ut.ObjectQueryToDataTable(TestataElem_Eredita.Distinct().ToList())

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return DT

        End Select

        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

    End Function



    '##############################################################################################
    Public Function Leggi_Agenda(ByVal piva As String,
                                 ByVal id_agenda As Integer,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 Optional ByVal Id_Budget As Integer = 0,
                                 Optional ByVal Raccoglitore_Cod As Integer = 0
                                 ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Agenda()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Select Case Id_Budget

                Case 0

                    'Nessun Vincolo sul Budget
                    Dim TestataElem =
                          From CI In GiasContext.Agenda
                          Join Movimenti In GiasContext.Movimenti
                            On Movimenti.PIVA Equals CI.PIVA And
                               Movimenti.Id_Agenda Equals CI.Id_Agenda
                          Group Join CDG_Testata In GiasContext.CDG_Testata
                            On CDG_Testata.Piva Equals CI.PIVA _
                            And CDG_Testata.Id_Agenda Equals CI.Id_Agenda Into CDG_Testata_Group = Group
                          From _CDG_Testata_Group In CDG_Testata_Group.DefaultIfEmpty()
                          Group Join Attivita In GiasContext.Attivita
                                On Attivita.ID_Attivita Equals CI.Id_Attivita Into Attivita_Group = Group
                          From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
                          Where
                         (CI.PIVA.Equals(piva))
                          Order By Movimenti.Id_Agenda, Movimenti.Id_Mov
                          Select New With {
                         .Piva = CI.PIVA,
                         .Id_Agenda = CI.Id_Agenda,
                         .Des_Lib = CI.des_lib,
                         .Lav_Cod = CI.Lav_Cod,
                         .Id_CDG = If(_CDG_Testata_Group Is Nothing, 0, _CDG_Testata_Group.Id_CDG),
                         .Id_Budget = If(_CDG_Testata_Group Is Nothing, 0, _CDG_Testata_Group.Budget),
                         .Data_CDG = If(_CDG_Testata_Group Is Nothing, Movimenti.Data_Movimento, _CDG_Testata_Group.Data_Inserimento),
                         .Data_Movimento = Movimenti.Data_Movimento,
                         .Id_Attivita = If(_Attivita_Group Is Nothing, 0, _Attivita_Group.ID_Attivita),
                         .Attivita_Des = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Desc),
                         .Split = CI.Split,
                         .Raccoglitore_Cod = CI.Raccoglitore_Cod,
                         .Mov_Desc = Movimenti.Mov_Desc
                         }


                    'Filtro Dinamico
                    Select Case Raccoglitore_Cod

                        Case 0

                            If id_agenda <> 0 Then
                                TestataElem = TestataElem.Where(Function(x) x.Id_Agenda = id_agenda)
                            End If

                        Case Else

                            TestataElem = TestataElem.Where(Function(x) x.Raccoglitore_Cod = Raccoglitore_Cod)

                    End Select



                    Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}

                    risposta = JsonConvert.SerializeObject(TestataElem.ToList(), Formatting.None, serializerSettings)





                Case Else

                    Dim TestataElem =
                      From CI In GiasContext.Agenda
                      Join Movimenti In GiasContext.Movimenti
                        On Movimenti.PIVA Equals CI.PIVA And
                           Movimenti.Id_Agenda Equals CI.Id_Agenda
                      Group Join CDG_Testata In GiasContext.CDG_Testata
                        On CDG_Testata.Piva Equals CI.PIVA _
                        And CDG_Testata.Id_Agenda Equals CI.Id_Agenda Into CDG_Testata_Group = Group
                      From _CDG_Testata_Group In CDG_Testata_Group.DefaultIfEmpty()
                      Group Join Attivita In GiasContext.Attivita
                            On Attivita.ID_Attivita Equals CI.Id_Attivita Into Attivita_Group = Group
                      From _Attivita_Group In Attivita_Group.DefaultIfEmpty()
                      Where
                     (CI.PIVA.Equals(piva)) _
                      AndAlso (_CDG_Testata_Group Is Nothing OrElse _CDG_Testata_Group.Budget = Id_Budget)
                      Order By Movimenti.Id_Agenda, Movimenti.Id_Mov
                      Select New With {
                     .Piva = CI.PIVA,
                     .Id_Agenda = CI.Id_Agenda,
                     .Des_Lib = CI.des_lib,
                     .Lav_Cod = CI.Lav_Cod,
                     .Id_CDG = If(_CDG_Testata_Group Is Nothing, 0, _CDG_Testata_Group.Id_CDG),
                     .Id_Budget = If(_CDG_Testata_Group Is Nothing, 0, _CDG_Testata_Group.Budget),
                     .Data_CDG = If(_CDG_Testata_Group Is Nothing, Movimenti.Data_Movimento, _CDG_Testata_Group.Data_Inserimento),
                     .Data_Movimento = Movimenti.Data_Movimento,
                     .Id_Attivita = If(_Attivita_Group Is Nothing, 0, _Attivita_Group.ID_Attivita),
                     .Attivita_Des = If(_Attivita_Group Is Nothing, "", _Attivita_Group.Desc),
                     .Split = CI.Split,
                     .Raccoglitore_Cod = CI.Raccoglitore_Cod,
                     .Mov_Desc = Movimenti.Mov_Desc
                     }

                    'Filtro Dinamico
                    Select Case Raccoglitore_Cod

                        Case 0

                            If id_agenda <> 0 Then
                                TestataElem = TestataElem.Where(Function(x) x.Id_Agenda = id_agenda)
                            End If

                        Case Else

                            TestataElem = TestataElem.Where(Function(x) x.Raccoglitore_Cod = Raccoglitore_Cod)

                    End Select

                    Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}

                    risposta = JsonConvert.SerializeObject(TestataElem.ToList(), Formatting.None, serializerSettings)

            End Select





        End Using

        Return risposta

    End Function


    'Verifica se l'id_agenda è il primo in ordine di chiave in base al raccoglitore di appartenenza
    Public Function VerificaCorrettezzaAgendaRaccoglitore(ByVal piva As String,
                                                          ByVal Id_Agenda As Integer,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As Integer

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.VerificaCorrettezzaAgendaRaccoglitore()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim risposta As Integer = -1

        Try

            'Lettura id_agenda per leggere il lav_cod
            strSql.Length = 0

            strSql.Append(" Select * From Agenda ")
            strSql.Append(" Where Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "'   ")
            strSql.Append(" And Agenda.Id_Agenda = " & Id_Agenda & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            If dt.Rows.Count > 0 Then

                If dt.Rows(0).Item("Lav_Cod") >= 3000 And dt.Rows(0).Item("Lav_Cod") < 4000 Then

                    'Operazione Zoo
                    risposta = 0

                Else

                    'Lettura Raccoglitore
                    strSql.Length = 0

                    strSql.Append(" Select * From Agenda ")
                    strSql.Append(" Where Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "'   ")
                    strSql.Append(" And Agenda.Id_Agenda = " & Id_Agenda & " ")
                    strSql.Append(" And (isNull(Raccoglitore_Cod, 0) = 0 Or ")
                    strSql.Append(" Id_Agenda in ")
                    strSql.Append(" (Select Top 1 Id_Agenda From Agenda AG Where ")
                    strSql.Append("          AG.Piva = Agenda.Piva ")
                    strSql.Append("     And  AG.Raccoglitore_Cod = Agenda.Raccoglitore_Cod Order by Id_Agenda Asc)) ")

                    '--------------------------------------------------------------------------
                    dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
                    '--------------------------------------------------------------------------
                    If dt.Rows.Count > 0 Then

                        risposta = If(IsNumeric(dt.Rows(0).Item("Raccoglitore_Cod")), dt.Rows(0).Item("Raccoglitore_Cod"), 0)

                    End If

                End If

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            risposta = -1
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return risposta

    End Function




    '##############################################################################################
    Public Function Leggi_Agenda_Riferimento(ByVal piva As String,
                                             ByVal id_agenda As Integer,
                                             ByVal id_mov_det As Integer,
                                             ByVal bDT As Boolean,
                                             ByRef objParametri As AgronicaCoreParametri,
                                                Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing
                                             ) As Object

        Dim risposta As String = ""
        Dim DT As DataTable

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Agenda_Riferimento()"

        'Dim scope AS TransactionScope = Nothing

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        Dim strCau_Mov As String = "{7300, 7350}"

        Dim TestataElem =
               From CI In GiasContext.Mov_Dettagli_Riferimenti
               Join Agenda In GiasContext.Agenda
                     On Agenda.PIVA Equals CI.Piva_Rif And
                        Agenda.Id_Agenda Equals CI.Id_Agenda_Rif
               Group Join CDG_Testata In GiasContext.CDG_Testata
                     On CDG_Testata.Piva Equals CI.Piva_Rif And
                        CDG_Testata.Id_Agenda Equals CI.Id_Agenda_Rif
                     Into CDG_Testata_Group = Group
               From _CDG_Testata_Group In CDG_Testata_Group.DefaultIfEmpty()
               Where
              (CI.Piva.Equals(piva)) _
              AndAlso (_CDG_Testata_Group Is Nothing OrElse _CDG_Testata_Group.Budget = 0) _
              AndAlso (CI.Id_Agenda = id_agenda) _
              AndAlso ((CI.Id_Mov_Det = id_mov_det) OrElse id_mov_det = 0) _
              AndAlso (CI.Lav_Cod_Rif = LAVCOD_COSTI_CDG) _
              AndAlso strCau_Mov.Contains(CI.Cau_Mov_Rif)
               Order By _CDG_Testata_Group.Id_CDG Descending
               Select New With {
              .Id_Mov = CI.Id_Mov,
              .Id_Mov_Det = CI.Id_Mov_Det,
              .Lav_Cod = CI.Lav_Cod,
              .Id_Agenda_Rif = CI.Id_Agenda_Rif,
              .Id_Mov_Rif = CI.Id_Mov_Rif,
              .Des_Lib = Agenda.des_lib,
              .Data_Movimento = If(_CDG_Testata_Group Is Nothing, Agenda.Validita_Inizio, _CDG_Testata_Group.Data_Inserimento),
              .Id_CDG = If(_CDG_Testata_Group Is Nothing, 0, _CDG_Testata_Group.Id_CDG),
              .Budget = If(_CDG_Testata_Group Is Nothing, 0, _CDG_Testata_Group.Budget),
              .Split = Agenda.Split
            }

        'Nota: L'ordinamento è importante per non considerare eventuali ghost nella tabella mov_dettagli_riferimenti
        Select Case bDT

            Case True

                Dim ut As New Gias_EF_Utility
                DT = ut.ObjectQueryToDataTable(TestataElem.ToList())

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return DT

            Case False

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(TestataElem.ToList(), Formatting.None, serializerSettings)

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return risposta

        End Select

        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

    End Function



    Public Function Leggi_Agenda_Riferimento_SQL(ByVal piva As String,
                                                 ByVal id_agenda As Integer,
                                                 ByVal id_mov_det As Integer,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As Object

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Agenda_Riferimento_SQL()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try


            strSql.Length = 0
            strSql.Append(" Select Mov_Dettagli_Riferimenti.Id_Mov, Mov_Dettagli_Riferimenti.Id_Mov_Det, Id_Agenda_Rif, Id_Mov_Rif, des_lib, ISNULL(CDG_Testata.Data_Inserimento, Agenda.Validita_Inizio) AS Data_Movimento,")
            strSql.Append(" ISNULL(CDG_Testata.Id_CDG, 0) AS Id_CDG, Agenda.Split ")

            strSql.Append(" From Agenda, CDG_Testata, Mov_Dettagli_Riferimenti ")

            strSql.Append(" Where Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And Mov_Dettagli_Riferimenti.Id_Agenda = " & Agro_SQL_SaveNum(id_agenda) & " ")

            strSql.Append(" And Agenda.PIVA = Mov_Dettagli_Riferimenti.Piva_Rif ")
            strSql.Append(" And Agenda.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda_Rif ")
            strSql.Append(" And CDG_Testata.Piva = Mov_Dettagli_Riferimenti.Piva_Rif And CDG_Testata.Budget = 0 ")
            strSql.Append(" And Mov_Dettagli_Riferimenti.Cau_Mov_Rif In ('7300', '7350') ")
            strSql.Append(" And CDG_Testata.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda_Rif ")
            strSql.Append(" And Mov_Dettagli_Riferimenti.Lav_Cod_Rif = 4500 ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function



    '##############################################################################################
    Public Function Leggi_Des_Lib_Dettaglio(ByVal piva As String,
                                             ByVal id_agenda As Integer,
                                             ByVal id_mov_det As Integer,
                                             ByVal bDT As Boolean,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Object

        Dim risposta As String = ""
        Dim DT As DataTable = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Des_Lib_Dettaglio()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From CI In GiasContext.Movimenti_dettagli
               Join Agenda In GiasContext.Agenda
                     On Agenda.PIVA Equals CI.PIVA And
                        Agenda.Id_Agenda Equals CI.Id_Agenda
               Where
              (CI.PIVA.Equals(piva)) _
              AndAlso (CI.Id_Agenda = id_agenda) _
              AndAlso (CI.Id_Mov_Det = id_mov_det)
               Select New With {
              .des_lib_dettaglio = "",
              .des_lib = Agenda.des_lib,
              .mov_det_des = CI.Mov_Det_Des,
              .lotto = CI.Lotto
            }


            Dim myTestataElem = TestataElem.Distinct().ToList()
            For Each obj In myTestataElem
                obj.des_lib_dettaglio = obj.des_lib.ToString() & " (" & obj.mov_det_des.ToString() & " " & obj.lotto.ToString() & ")"
            Next

            Select Case bDT

                Case True

                    Dim ut As New Gias_EF_Utility
                    DT = ut.ObjectQueryToDataTable(myTestataElem.Distinct().ToList())
                    Return DT

                Case False

                    Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                    risposta = JsonConvert.SerializeObject(myTestataElem.ToList(), Formatting.None, serializerSettings)
                    Return risposta

            End Select

        End Using

    End Function


    '##############################################################################################
    Public Function Leggi_Agenda_Consistente_ScaricoTempi(ByVal piva As String,
                                                          ByVal id_agenda As Integer,
                                                          ByRef objParametri As AgronicaCoreParametri,
                                                            Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing
                                                          ) As DataTable

        Dim risposta As String = ""
        Dim DT As DataTable

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Agenda_Consistente_ScaricoTempi()"

        'Dim gefutils AS New Gias_EF_Utility
        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If


        Dim TestataElem =
               From CI In GiasContext.Agenda
               Join CDG_Testata In GiasContext.CDG_Testata
                    On CDG_Testata.Id_Agenda Equals CI.Id_Agenda
               Where CI.PIVA.Equals(piva) AndAlso
                     CI.Id_Agenda = id_agenda AndAlso
                     CDG_Testata.Budget = 0
               Select New With {
              .Piva = CI.PIVA,
              .Id_Agenda = CI.Id_Agenda,
              .Lav_Cod = CI.Lav_Cod
              }


        Dim ut As New Gias_EF_Utility
        DT = ut.ObjectQueryToDataTable(TestataElem.Distinct().ToList())

        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

        Return DT


    End Function




    '##############################################################################################
    Public Function Ricerca_Scarico_Tempi(ByVal piva As String,
                                          ByVal FiltroManodopera() As String,
                                          ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal isTimeSheetPersonale As Boolean = False
                                          ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Ricerca_Scarico_Tempi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim strJoin As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim dt_finale As New DataTable
        Dim dr_search As DataRow()
        Dim dr_finale As DataRow
        Dim select_list As String
        Dim PivaSuperUser = objParametri.PivaSuperUser
        Dim Data_Ora_Inizio As String
        Dim Data_Ora_Fine As String
        Dim Data_Ora_Tot As String
        Dim Qta_Tot As String
        Dim Id_Agenda As String
        Dim Id_CDG As String
        Dim Modalita_Imputazione As String
        Dim Des_Lib As String
        Dim Giorni As Integer
        Dim Data As Date

        Dim Ora As Integer
        Dim Minuti As Integer
        Dim parteIntera As Decimal
        Dim parteDecimali As Decimal

        'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
        'Dim Bozza_Cod As String
        'Dim Bozza_Des As String

        Dim bFiltroManodopera As Boolean = False
        If FiltroManodopera IsNot Nothing AndAlso FiltroManodopera.Length > 0 Then
            bFiltroManodopera = True
        Else
            bFiltroManodopera = False
        End If

        strSql.Length = 0

        select_list = "     CDG_Testata.Id_Agenda, CDG_Testata.Id_CDG, CDG_Testata.Modalita_Imputazione, CDG_Dettagli.Id_CDG_Dettagli, CDG_Testata.Piva, Agenda.Des_Lib, CDG_Dettagli.Sa_Cod, ISNULL(CDG_Dettagli.Campo_Cod, 0) AS Campo_Cod, CDG_Dettagli.Appezza, CDG_Dettagli.Id_Reg, CDG_Dettagli.Id_Cod_reg_impianti_codici, " & vbCrLf &
                      "     CDG_Dettagli.Progetto_Cod, CDG_Dettagli.Macchine_Cod AS Mac_Cod_Det, CDG_Dettagli.Id_Imputazione AS Imputazione_Cod, CDG_Testata.Mac_Cod, CDG_Testata.Cod_Risum, CDG_Dettagli.Linea_Cod, " & vbCrLf &
                      "     CDG_Testata.Qta, CDG_Testata.Data_Inserimento, (ISNULL(Contatti.Cognome, '') + ' ' + ISNULL(Contatti.Nome, '')) AS Rag_Soc, " & vbCrLf &
                      "     CDG_Testata.ID_Attivita, CDG_Testata.Qualifica_Cod, CDG_Testata.Tariffa_Cod, CDG_Testata.Elem_Cod, CDG_Testata.Udm_Cod, " & vbCrLf &
                      "     Rapporti_Contabili.Cod_Rapporto, Rapporti_Contabili.Rapporto_Des, ISNULL(Qualifica_Des, '') AS Qualifica_Des, ISNULL(Campi.Campo_Des, '') AS Campo_Des, " & vbCrLf &
                      "     (ISNULL(Attivita.[Sigla], '') + ' - ' + ISNULL(Attivita.[Desc], '')) AS [Desc], ISNULL(Attivita.attivita_poliannuale, '') AS attivita_poliannuale, ISNULL(Tariffa_Des, '') Tariffa_Des, UnitaMisura.Udm_Sim, " & vbCrLf &
                      "     CDG_Testata.Data_Inserimento, CDG_Testata.Data_Ora_Inizio, CDG_Testata.Data_Ora_Fine, " & vbCrLf &
                      "     (ISNULL(Sa_Nome, '') + ' - ' + ISNULL(App_Nome, '') + ' - ' + ISNULL(Campo_Des, '') + ' - ' + ISNULL(Veg_Des, '') + ' - ' + ISNULL(Cul_Des, '')) AS Impianto_Des, " & vbCrLf &
                      "     ISNULL(Imprese_Progetti.Progetto_Nome, '') AS Progetto_Des, ISNULL(Imputazioni.Imputazione_Nome, '') AS Imputazione_Nome, " & vbCrLf &
                      "     ISNULL(Parco_Macchine.Mac_Des, '') AS Mac_Des, ISNULL(Parco_Macchine_Det.Mac_Des, '') AS Mac_Des_Det, ISNULL(CDG_Dettagli.Raggruppamento_Cod, 0) AS Raggruppamento_Cod, ISNULL(Stalla_Raggruppamenti.Raggruppamento_Des, '') AS Raggruppamento_Des, " & vbCrLf &
                      "     CDG_Testata.Prezzo_Unitario, CDG_Testata.Valore_Totale AS Prezzo_Totale, ISNULL(Zoo_Animali.Progetto, '') AS Progetto, ISNULL(APP_CDG_Generale_ID,'') AS APP_CDG_Generale_ID, " & vbCrLf &
                      "     ISNULL(OrigineApp,0) AS OrigineApp, ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ISNULL(Campo_Des, '') AS Campo_Des, ISNULL(Reg_Impianti_Codici.Val_Cod, '') AS Codice_Anagrafe_Impianto, ISNULL(Stalla.Sta_Des, '') AS Sta_Des, ISNULL(Reg_Impianti_Codici2.Val_Cod, 0) AS Esercizio_Chiuso, " & vbCrLf &
                      "     CASE WHEN ISNULL(OrigineApp,0) = 0 THEN 'No'" & vbCrLf &
                      "          WHEN ISNULL(OrigineApp,0) = 1 THEN 'Sì' " & vbCrLf &
                      "          ELSE 'Sì Ric.' " & vbCrLf &
                      "     END AS OrigineApp_Des, " & vbCrLf &
                      "     ISNULL(Attivita.Attivita_Interna,0) AS Attivita_Interna," & vbCrLf &
                      "     ISNULL((SELECT CASE WHEN Lav_Cod = 5007 THEN 1 ELSE 0 END AS isVisita FROM Mov_Dettagli_Riferimenti mdf WHERE mdf.Piva_Rif = CdG_Testata.Piva AND mdf.Id_Agenda_Rif = CDG_Testata.Id_Agenda), 0) AS isVisita" & vbCrLf
        'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPP
        '"     ISNULL(Bozza, 0) As Bozza_Cod," & vbCrLf &
        '"     CASE WHEN ISNULL(Bozza,0) = 0 Then 'No'" & vbCrLf &
        '"          WHEN ISNULL(Bozza,0) = 1 THEN 'Sì' " & vbCrLf &
        '"     END AS Bozza_Des, " & vbCrLf &

        dt_finale.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Id_CDG", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Id_CDG_Dettagli", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Modalita_Imputazione", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("OrigineApp", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("OrigineApp_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("APP_CDG_Generale_ID", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Piva", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Campo_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Id_Reg", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Id_Cod_reg_impianti_codici", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Progetto_Cod", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Elem_Cod", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Mac_Cod", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Mac_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Raggruppamento_Cod", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Raggruppamento_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Progetto", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Imputazione_Cod", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Imputazione_Nome", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Linea_Cod", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Qta", GetType(Decimal)))
        dt_finale.Columns.Add(New DataColumn("Cod_Risum", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("ID_Attivita", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("attivita_poliannuale", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Qualifica_Cod", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Tariffa_Cod", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Cod_Rapporto", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Rapporto_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Qualifica_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Desc", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Tariffa_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Udm_Sim", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Data_Inserimento", GetType(Date)))
        dt_finale.Columns.Add(New DataColumn("Data_Inserimento_Check", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Impianto_Cod", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Impianto_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Progetto_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Mac_Cod_Det", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Mac_Des_Det", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(Decimal)))
        dt_finale.Columns.Add(New DataColumn("Prezzo_Totale", GetType(Decimal)))
        dt_finale.Columns.Add(New DataColumn("Totale_Riga", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Numero_CDC", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Specie_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Codice_Anagrafe_Impianto", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Sta_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Esercizio_Chiuso", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Attivita_Interna", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("isVisita", GetType(Integer)))

        Giorni = DateDiff("d", Validita_Inizio, Validita_Fine)

        For i = 0 To Giorni

            Data = DateAdd(DateInterval.Day, i, Date.ParseExact(Validita_Inizio, "dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo))

            'Calcolo Campi Data_Ora
            Data_Ora_Inizio = "Data_Ora_Inizio" & Format(Data, "yyyyMMdd")
            Data_Ora_Fine = "Data_Ora_Fine" & Format(Data, "yyyyMMdd")
            Data_Ora_Tot = "Data_Ora_Tot" & Format(Data, "yyyyMMdd")
            Qta_Tot = "Qta_Tot" & Format(Data, "yyyyMMdd")
            Id_Agenda = "Id_Agenda" & Format(Data, "yyyyMMdd")
            Id_CDG = "Id_CDG" & Format(Data, "yyyyMMdd")
            Des_Lib = "Des_Lib" & Format(Data, "yyyyMMdd")
            Modalita_Imputazione = "Modalita_Imputazione" & Format(Data, "yyyyMMdd")
            'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
            'Bozza_Cod = "Bozza_Cod" & Format(Data, "yyyyMMdd")
            'Bozza_Des = "Bozza_Des" & Format(Data, "yyyyMMdd")

            dt_finale.Columns.Add(New DataColumn(Data_Ora_Inizio, GetType(Date)))
            dt_finale.Columns.Add(New DataColumn(Data_Ora_Fine, GetType(Date)))
            dt_finale.Columns.Add(New DataColumn(Data_Ora_Tot, GetType(Date)))
            dt_finale.Columns.Add(New DataColumn(Qta_Tot, GetType(Decimal)))
            dt_finale.Columns.Add(New DataColumn(Id_Agenda, GetType(Integer)))
            dt_finale.Columns.Add(New DataColumn(Id_CDG, GetType(Integer)))
            dt_finale.Columns.Add(New DataColumn(Des_Lib, GetType(String)))
            dt_finale.Columns.Add(New DataColumn(Modalita_Imputazione, GetType(Integer)))
            'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
            'dt_finale.Columns.Add(New DataColumn(Bozza_Cod, GetType(Integer)))
            'dt_finale.Columns.Add(New DataColumn(Bozza_Des, GetType(String)))

        Next


        strSql.AppendLine(" SELECT DISTINCT")
        strSql.AppendLine(select_list)

        strSql.AppendLine(" FROM CDG_Testata ")

        strSql.AppendLine("")

        strSql.AppendLine(" INNER JOIN CDG_Dettagli ON (CDG_Testata.Piva = CDG_Dettagli.Piva AND CDG_Testata.Id_Cdg = CDG_Dettagli.Id_Cdg)")
        strSql.AppendLine(" INNER JOIN Agenda ON (CDG_Testata.Piva = Agenda.Piva AND CDG_Testata.Id_Agenda = Agenda.Id_Agenda)")
        strSql.AppendLine(" INNER JOIN UnitaMisura ON (CDG_Testata.UDM_COD = UnitaMisura.UDM_COD)")

        strSql.AppendLine("")

        strSql.AppendLine(" LEFT OUTER JOIN Qualifiche ON (CDG_Testata.Qualifica_Cod = Qualifiche.Qualifica_Cod)")
        strSql.AppendLine(" LEFT OUTER JOIN Attivita ON (CDG_Testata.ID_Attivita = Attivita.ID_Attivita)")
        strSql.AppendLine(" LEFT OUTER JOIN Tariffe ON (CDG_Testata.Tariffa_Cod = Tariffe.Tariffa_Cod)")
        strSql.AppendLine(" LEFT OUTER JOIN Risorse_Umane ON (CDG_Testata.Cod_RisUm = Risorse_Umane.Cod_RisUm)")

        'Se arriviamo dal pulsante 'Il tuo TimeSheet', carico solo i dati relativi all'utente collegato
        If isTimeSheetPersonale AndAlso objParametri.UtenteUsername <> objParametri.SuperUserUsername Then
            strSql.AppendLine(" INNER JOIN ContattiXUtentiGias ContattiXUtentiGias ON ContattiXUtentiGias.cod_contatto = Risorse_Umane.Cod_Contatto AND ContattiXUtentiGias.piva = Risorse_Umane.piva AND ContattiXUtentiGias.username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'")
        End If

        strSql.AppendLine("")

        strSql.AppendLine(" LEFT OUTER JOIN Rapporti_Contabili ON (Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto)")
        strSql.AppendLine(" LEFT OUTER JOIN Contatti ON (Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto)")
        strSql.AppendLine(" LEFT OUTER JOIN Parco_Macchine ON (CDG_Testata.Mac_Cod = Parco_Macchine.Mac_Cod)")
        strSql.AppendLine(" LEFT OUTER JOIN Parco_Macchine AS Parco_Macchine_Det ON (CDG_Dettagli.Macchine_Cod = Parco_Macchine_Det.Mac_Cod)")
        strSql.AppendLine(" LEFT OUTER JOIN Centri_Aziendali ON (")
        strSql.AppendLine("       CDG_Dettagli.Piva = Centri_Aziendali.Piva ")
        strSql.AppendLine("       AND CDG_Dettagli.Sa_Cod = Centri_Aziendali.Sa_Cod) ")

        strSql.AppendLine("")

        strSql.AppendLine(" LEFT OUTER JOIN Appezzamento ON (")
        strSql.AppendLine("       CDG_Dettagli.Piva = Appezzamento.Piva ")
        strSql.AppendLine("       AND CDG_Dettagli.Sa_Cod = Appezzamento.Sa_Cod ")
        strSql.AppendLine("       AND CDG_Dettagli.Appezza = Appezzamento.Appezza) ")

        strSql.AppendLine("")

        strSql.AppendLine(" Left OUTER JOIN Campi ON Appezzamento.Piva = Campi.Piva")
        strSql.AppendLine("       AND Appezzamento.Sa_Cod = Campi.Sa_Cod")
        strSql.AppendLine("       AND Appezzamento.Campo_Cod = Campi.Campo_Cod")

        strSql.AppendLine("")

        strSql.AppendLine(" LEFT OUTER JOIN Reg_Impianti ON (")
        strSql.AppendLine("       CDG_Dettagli.Piva = Reg_Impianti.Piva ")
        strSql.AppendLine("       AND CDG_Dettagli.Sa_Cod = Reg_Impianti.Sa_Cod ")
        strSql.AppendLine("       AND CDG_Dettagli.Appezza = Reg_Impianti.Appezza ")
        strSql.AppendLine("       AND CDG_Dettagli.Id_Reg = Reg_Impianti.Id_Reg) ")

        strSql.AppendLine("")

        strSql.AppendLine(" LEFT OUTER JOIN Imprese_Progetti ON (")
        strSql.AppendLine("       CDG_Dettagli.Piva = Imprese_Progetti.Piva ")
        strSql.AppendLine("       AND CDG_Dettagli.Sa_Cod = Imprese_Progetti.Sa_Cod ")
        strSql.AppendLine("       AND CDG_Dettagli.Appezza = Imprese_Progetti.Appezza ")
        strSql.AppendLine("       AND CDG_Dettagli.Id_Reg = Imprese_Progetti.Id_Reg ")
        strSql.AppendLine("       AND CDG_Dettagli.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
        strSql.AppendLine("       AND Imprese_Progetti.Validita_Inizio <= CDG_Testata.Data_Inserimento ")
        strSql.AppendLine("       AND Imprese_Progetti.Validita_Fine >= CDG_Testata.Data_Inserimento) ")

        strSql.AppendLine("")

        strSql.AppendLine(" LEFT OUTER JOIN Reg_Impianti_Codici ON ( ")
        strSql.AppendLine("       Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA ")
        strSql.AppendLine("       AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod ")
        strSql.AppendLine("       AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza ")
        strSql.AppendLine("       AND Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg ")
        strSql.AppendLine("       AND Reg_Impianti_Codici.id_cod = " & CInt(enum_CodiciAnagrafe.Codice_Impianto) & ") " & vbCrLf)

        strSql.AppendLine("")

        'Chiusura esercizio
        strSql.AppendLine(" LEFT OUTER JOIN Reg_Impianti_Codici AS Reg_Impianti_Codici2 ON ( ")
        strSql.AppendLine("       Imprese_Progetti.PIVA = Reg_Impianti_Codici2.PIVA ")
        strSql.AppendLine("       AND Imprese_Progetti.SA_COD = Reg_Impianti_Codici2.sa_cod ")
        strSql.AppendLine("       AND Imprese_Progetti.APPEZZA = Reg_Impianti_Codici2.appezza ")
        strSql.AppendLine("       AND Imprese_Progetti.ID_REG = Reg_Impianti_Codici2.Id_Reg ")
        strSql.AppendLine("       AND Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici2.Progetto_Cod ")
        strSql.AppendLine("       AND Reg_Impianti_Codici2.id_cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa) & ") " & vbCrLf)

        strSql.AppendLine("")

        strSql.AppendLine(" LEFT OUTER JOIN Cultivar ON ( ")
        strSql.AppendLine("       Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod) ")

        strSql.AppendLine("")

        strSql.AppendLine(" LEFT OUTER JOIN SpecieVegetali ON ( ")
        strSql.AppendLine("       Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod) ")

        strSql.AppendLine("")

        strSql.AppendLine(" LEFT OUTER JOIN Imputazioni ON ( ")
        strSql.AppendLine("       Imputazioni.Imputazione_Cod = CDG_Dettagli.Id_Imputazione ")
        strSql.AppendLine(" AND Imputazioni.Piva = '" & Agro_SQL_SaveText(piva) & "') ")

        strSql.AppendLine("")

        strSql.AppendLine(" LEFT OUTER JOIN Stalla_Raggruppamenti ON ( ")
        strSql.AppendLine("       Stalla_Raggruppamenti.Raggruppamento_Cod = CDG_Dettagli.Raggruppamento_Cod) ")

        strSql.AppendLine("")

        strSql.AppendLine(" LEFT OUTER JOIN Stalla ON ( ")
        strSql.AppendLine("        Stalla_Raggruppamenti.Piva = Stalla.Piva ")
        strSql.AppendLine("        AND Stalla_Raggruppamenti.Sa_Cod = Stalla.Sa_Cod ")
        strSql.AppendLine("        AND Stalla_Raggruppamenti.Sta_Num = Stalla.Sta_Num) ")

        strSql.AppendLine("")

        strSql.AppendLine(" LEFT OUTER JOIN Zoo_Animali ON (  ")
        strSql.AppendLine("       Zoo_Animali.Cod_Progetto = CDG_Dettagli.Cod_Animale) ")

        strSql.AppendLine("")

        strSql.AppendLine(" WHERE CDG_Testata.Data_Inserimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
        strSql.AppendLine(" AND CDG_Testata.Data_Inserimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
        strSql.AppendLine(" AND CDG_Testata.Piva_SuperUser = '" & PivaSuperUser & "' ")
        strSql.AppendLine(" AND CDG_Testata.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

        strSql.AppendLine("")

        strSql.AppendLine(" AND (CDG_Testata.Cod_Risum != 0 OR  CDG_Testata.Mac_Cod != 0 ) ")
        strSql.AppendLine(" AND CDG_Testata.Budget = 0 ")

        strSql.AppendLine("")


        ''Filtro le distinte chiuse
        'strSql.AppendLine(" AND Not Exists (SELECT 1 FROM Reg_Impianti_Codici AS Reg_Impianti_Distinta ")
        'strSql.AppendLine(" Where Reg_Impianti_Distinta.PIVA = Imprese_Progetti.Piva ")
        'strSql.AppendLine(" AND Reg_Impianti_Distinta.SA_COD = Imprese_Progetti.Sa_Cod ")
        'strSql.AppendLine(" AND Reg_Impianti_Distinta.APPEZZA = Imprese_Progetti.Appezza ")
        'strSql.AppendLine(" AND Reg_Impianti_Distinta.id_reg = Imprese_Progetti.Id_Reg ")
        'strSql.AppendLine(" AND Reg_Impianti_Distinta.Progetto_Cod = Imprese_Progetti.Progetto_Cod " + vbCrLf)
        'strSql.AppendLine(" AND Reg_Impianti_Distinta.Id_Cod = " & CInt(enum_CodiciAnagrafe.Distinta_Chiusa))
        'strSql.AppendLine(" AND Reg_Impianti_Distinta.Val_Cod = 1)")

        'strSql.AppendLine("  ORDER BY CDG_Testata.Data_Inserimento , rag_soc, mac_des, Data_Ora_Inizio, data_ora_fine ")
        strSql.AppendLine(" ORDER BY CDG_Testata.Data_Inserimento, Data_Ora_Inizio, data_ora_fine ")

        '--------------------------------------------------------------------------
        dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)



        For Each dr As DataRow In dt.Rows

            'Calcolo Campi Data_Ora
            Data_Ora_Inizio = "Data_Ora_Inizio" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Data_Ora_Fine = "Data_Ora_Fine" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Data_Ora_Tot = "Data_Ora_Tot" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Qta_Tot = "Qta_Tot" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Id_Agenda = "Id_Agenda" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Id_CDG = "Id_CDG" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Des_Lib = "Des_Lib" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Modalita_Imputazione = "Modalita_Imputazione" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
            'Bozza_Cod = "Bozza_Cod" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            'Bozza_Des = "Bozza_Des" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")

            'SELECT Case CInt(dr.Item("Modalita_Imputazione"))

            'Case 0, 1 'QCD/Standalone --> Rottura = Testata

            dr_search = dt_finale.Select("Piva = '" & dr.Item("Piva") & "' AND " &
                                         "Id_CDG = " & dr.Item("Id_CDG") & " AND " &
                                         "Campo_Cod = " & dr.Item("Campo_Cod"))

            '     Case 2 'Scarico Tempi

            ' dr_search = dt_finale.SELECT("Piva = '" & dr.Item("Piva") & "' AND " &
            ' "Modalita_Imputazione = " & dr.Item("Modalita_Imputazione") & " AND " &
            ' "OrigineApp = " & dr.Item("OrigineApp") & " AND " &
            '"Sa_Cod = " & dr.Item("Sa_Cod") & " AND " &
            '"Appezza = " & dr.Item("Appezza") & " AND " &
            '"Id_Reg = " & dr.Item("Id_Reg") & " AND " &
            '"Id_Cod_reg_impianti_codici = " & dr.Item("Id_Cod_reg_impianti_codici") & " AND " &
            '"Progetto_Cod = " & dr.Item("Progetto_Cod") & " AND " &
            '"Id_Imputazione = " & dr.Item("Id_Imputazione") & " AND " &
            '"Mac_Cod = " & dr.Item("Mac_Cod") & " AND " &
            '"Cod_Risum = " & dr.Item("Cod_Risum") & " AND " &
            '"Linea_Cod = " & dr.Item("Linea_Cod") & " AND " &
            '"ID_Attivita = " & dr.Item("ID_Attivita") & " AND " &
            '"Qualifica_Cod = " & dr.Item("Qualifica_Cod") & " AND " &
            '"Tariffa_Cod = " & dr.Item("Tariffa_Cod") & " AND " &
            '"Data_Inserimento_Check = '" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd") & "'")

            'End SELECT




            If dr_search.Length <> 0 Then

                dr_search(0).Item(Data_Ora_Inizio) = dr.Item("Data_Ora_Inizio")
                dr_search(0).Item(Data_Ora_Fine) = dr.Item("Data_Ora_Fine")
                dr_search(0).Item(Qta_Tot) = dr.Item("Qta")

                'Conversione intero minuti a data
                parteIntera = Math.Truncate(dr.Item("Qta"))
                parteDecimali = dr.Item("Qta") - parteIntera
                Ora = parteIntera
                Minuti = 60 * parteDecimali

                'In caso di raggiungimento di 60 min --> la converto in 1 ora
                If Minuti = 60 Then
                    Ora = Ora + 1
                    Minuti = 0
                End If

                If dr_search(0).Item(Data_Ora_Inizio).ToString = "" OrElse dr_search(0).Item(Data_Ora_Fine).ToString = "" Then
                    dr_search(0).Item(Data_Ora_Tot) = dr_search(0).Item(Data_Ora_Inizio)
                Else
                    dr_search(0).Item(Data_Ora_Tot) = CDate(dr_search(0).Item(Data_Ora_Inizio)).ToShortDateString & " " & Ora.ToString("D2") & ":" & Minuti.ToString("D2") & ":00"
                End If

                Select Case dr_search(0).Item("Numero_CDC")

                    Case 1

                        'Inserimento secondo dettaglio
                        If dr.Item("Campo_Cod") = 0 And dr.Item("Appezza") <> 0 And dr.Item("Id_Reg") <> 0 Then
                            If dr.Item("Codice_Anagrafe_Impianto") <> "" Then
                                dr_search(0).Item("Impianto_Des") = dr_search(0).Item("Impianto_Des") & ", " & dr.Item("Codice_Anagrafe_Impianto")
                            Else
                                dr_search(0).Item("Impianto_Des") = dr_search(0).Item("Impianto_Des") & ",*"
                            End If


                        End If

                        '    dr_search(0).Item("Impianto_Des") = dr_search(0).Item("Impianto_Des") & ", *"
                        'End If

                        If Trim(dr.Item("Imputazione_Nome")) <> "" Then
                            dr_search(0).Item("Imputazione_Nome") = dr_search(0).Item("Imputazione_Nome") & ", " & dr.Item("Imputazione_Nome")
                        End If

                        If Trim(dr.Item("Progetto")) <> "" Then
                            dr_search(0).Item("Raggruppamento_Des") = dr_search(0).Item("Raggruppamento_Des") & ", " & dr.Item("Progetto")
                        End If

                    Case 2

                        If dr.Item("Campo_Cod") = 0 And dr.Item("Appezza") <> 0 And dr.Item("Id_Reg") <> 0 Then
                            dr_search(0).Item("Impianto_Des") = dr_search(0).Item("Impianto_Des") & ", ...."
                        End If

                        If Trim(dr_search(0).Item("Imputazione_Nome")) <> "" Then
                            dr_search(0).Item("Imputazione_Nome") = dr_search(0).Item("Imputazione_Nome") & ", ...."
                        End If

                        If Trim(dr_search(0).Item("Raggruppamento_Des")) <> "" Then
                            dr_search(0).Item("Raggruppamento_Des") = dr_search(0).Item("Raggruppamento_Des") & ", ...."
                        End If

                    Case Else

                        'Do Nothing

                End Select



                dr_search(0).Item("Numero_CDC") = dr_search(0).Item("Numero_CDC") + 1

                'Correzione Chiusura Esercizio
                dr_search(0).Item("Esercizio_Chiuso") = CInt(dr_search(0).Item("Esercizio_Chiuso")) + dr.Item("Esercizio_Chiuso")

                dr_search(0).Item("Attivita_Interna") = CInt(dr_search(0).Item("Attivita_Interna")) + dr.Item("Attivita_Interna")
                dr_search(0).Item("isVisita") = CInt(dr_search(0).Item("isVisita")) + dr.Item("isVisita")

                'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
                'dr_search(0).Item(Bozza_Cod) = dr.Item("Bozza_Cod")
                'dr_search(0).Item(Bozza_Des) = dr.Item("Bozza_Des")


            Else

                'Creo una nuova riga
                dr_finale = dt_finale.NewRow

                dr_finale.Item("Modalita_Imputazione") = dr.Item("Modalita_Imputazione")
                dr_finale.Item("OrigineApp") = dr.Item("OrigineApp")
                dr_finale.Item("OrigineApp_Des") = dr.Item("OrigineApp_Des")
                dr_finale.Item("APP_CDG_Generale_ID") = dr.Item("APP_CDG_Generale_ID")
                dr_finale.Item("Id_Agenda") = dr.Item("Id_Agenda")
                dr_finale.Item("Id_CDG") = dr.Item("Id_CDG")
                dr_finale.Item("Id_CDG_Dettagli") = dr.Item("Id_CDG_Dettagli")
                dr_finale.Item("Piva") = dr.Item("Piva")
                dr_finale.Item("Sa_Cod") = dr.Item("Sa_Cod")

                dr_finale.Item("Imputazione_Cod") = dr.Item("Imputazione_Cod")
                dr_finale.Item("Imputazione_Nome") = dr.Item("Imputazione_Nome")

                dr_finale.Item("Raggruppamento_Cod") = dr.Item("Raggruppamento_Cod")
                dr_finale.Item("Raggruppamento_Des") = dr.Item("Raggruppamento_Des")

                If dr.Item("Sta_Des") <> "" Then
                    dr_finale.Item("Raggruppamento_Des") = dr_finale.Item("Raggruppamento_Des") & " (" & dr.Item("Sta_Des") & ")"
                End If
                If dr.Item("Progetto") <> "" Then
                    dr_finale.Item("Raggruppamento_Des") = dr_finale.Item("Raggruppamento_Des") & " " & dr.Item("Progetto")
                End If

                If dr.Item("Progetto_Cod") <> 0 Then
                    dr_finale.Item("Impianto_Cod") = dr.Item("Sa_Cod") & "|" & dr.Item("Appezza") & "|" & dr.Item("Id_Reg") & "|" & dr.Item("Progetto_Cod")
                End If

                dr_finale.Item("Mac_Cod_Det") = dr.Item("Mac_Cod_Det")
                dr_finale.Item("Mac_Des_Det") = dr.Item("Mac_Des_Det")
                dr_finale.Item("Elem_Cod") = dr.Item("Elem_Cod")
                dr_finale.Item("Udm_Cod") = dr.Item("Udm_Cod")
                dr_finale.Item("Mac_Cod") = dr.Item("Mac_Cod")
                dr_finale.Item("Cod_Risum") = dr.Item("Cod_Risum")
                dr_finale.Item("Linea_Cod") = dr.Item("Linea_Cod")
                dr_finale.Item("ID_Attivita") = dr.Item("ID_Attivita")
                dr_finale.Item("attivita_poliannuale") = dr.Item("attivita_poliannuale")
                dr_finale.Item("Desc") = dr.Item("Desc")
                dr_finale.Item("Qualifica_Cod") = dr.Item("Qualifica_Cod")
                dr_finale.Item("Qualifica_Des") = dr.Item("Qualifica_Des")
                dr_finale.Item("Tariffa_Cod") = dr.Item("Tariffa_Cod")
                dr_finale.Item("Tariffa_Des") = dr.Item("Tariffa_Des")
                dr_finale.Item("Data_Inserimento") = Format(dr.Item("Data_Inserimento"), "dd/MM/yyyy")
                dr_finale.Item("Data_Inserimento_Check") = CStr(Format(dr.Item("Data_Inserimento"), "yyyyMMdd"))
                dr_finale.Item("Cod_Rapporto") = dr.Item("Cod_Rapporto")
                dr_finale.Item("Rapporto_Des") = dr.Item("Rapporto_Des")
                dr_finale.Item("Rag_Soc") = Trim(dr.Item("Rag_Soc"))
                dr_finale.Item("Mac_Des") = dr.Item("Mac_Des")
                dr_finale.Item("Udm_Sim") = dr.Item("Udm_Sim")

                Select Case dr.Item("Campo_Cod")
                    Case 0

                        dr_finale.Item("Campo_Cod") = 0
                        dr_finale.Item("Appezza") = dr.Item("Appezza")
                        dr_finale.Item("Id_Reg") = dr.Item("Id_Reg")
                        dr_finale.Item("Id_Cod_reg_impianti_codici") = dr.Item("Id_Cod_reg_impianti_codici")
                        dr_finale.Item("Progetto_Cod") = dr.Item("Progetto_Cod")


                        If dr.Item("Appezza") <> 0 And dr.Item("Id_Reg") <> 0 Then
                            If dr.Item("Codice_Anagrafe_Impianto") <> "" Then
                                dr_finale.Item("Impianto_Des") = dr.Item("Codice_Anagrafe_Impianto")
                            Else
                                dr_finale.Item("Impianto_Des") = "*"
                            End If

                            If dr.Item("Veg_Des") <> "" Then
                                dr_finale.Item("Impianto_Des") = dr_finale.Item("Impianto_Des") & " - " & dr.Item("Progetto_Des") & " (" & dr.Item("Veg_Des") & ")" & " - " & dr.Item("Campo_Des")
                            End If
                        Else
                            dr_finale.Item("Impianto_Des") = ""
                        End If

                        dr_finale.Item("Progetto_Des") = dr.Item("Progetto_Des")

                    Case Else

                        dr_finale.Item("Campo_Cod") = dr.Item("Campo_Cod")
                        dr_finale.Item("Campo_Des") = dr.Item("Campo_Des")
                        dr_finale.Item("Appezza") = 0
                        dr_finale.Item("Id_Reg") = 0
                        dr_finale.Item("Id_Cod_reg_impianti_codici") = 0
                        dr_finale.Item("Progetto_Cod") = 0

                        dr_finale.Item("Impianto_Des") = ""
                        dr_finale.Item("Progetto_Des") = ""


                End Select


                dr_finale.Item("Prezzo_Unitario") = dr.Item("Prezzo_Unitario")
                dr_finale.Item("Prezzo_Totale") = dr.Item("Prezzo_Totale")

                dr_finale.Item(Data_Ora_Inizio) = dr.Item("Data_Ora_Inizio")
                dr_finale.Item(Data_Ora_Fine) = dr.Item("Data_Ora_Fine")
                dr_finale.Item(Qta_Tot) = dr.Item("Qta")

                'Conversione intero minuti a data
                parteIntera = Math.Truncate(dr.Item("Qta"))
                parteDecimali = dr.Item("Qta") - parteIntera
                Ora = parteIntera
                Minuti = 60 * parteDecimali

                'In caso di raggiungimento di 60 min --> la converto in 1 ora
                If Minuti = 60 Then
                    Ora = Ora + 1
                    Minuti = 0
                End If

                If dr_finale.Item(Data_Ora_Inizio) <> dr_finale.Item(Data_Ora_Fine) Then
                    dr_finale.Item(Data_Ora_Tot) = "01/01/1900 " & Ora.ToString("D2") & ":" & Minuti.ToString("D2")
                Else
                    ' Se sono uguali significa che ho salvato direttamente la quantità di ore che può essere > 24
                    dr_finale.Item(Data_Ora_Tot) = AGRODATAINIZIO
                End If

                dr_finale.Item("Totale_Riga") = Ora.ToString("D2") & ":" & Minuti.ToString("D2")

                dr_finale.Item(Id_Agenda) = dr.Item("Id_Agenda")
                dr_finale.Item(Id_CDG) = dr.Item("Id_CDG")
                dr_finale.Item(Des_Lib) = dr.Item("Des_Lib")
                dr_finale.Item(Modalita_Imputazione) = dr.Item("Modalita_Imputazione")

                'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
                'dr_finale.Item(Bozza_Cod) = dr.Item("Bozza_Cod")
                'dr_finale.Item(Bozza_Des) = dr.Item("Bozza_Des")

                dr_finale.Item("Numero_CDC") = 1

                dr_finale.Item("Esercizio_Chiuso") = dr.Item("Esercizio_Chiuso")

                dr_finale.Item("Attivita_Interna") = dr.Item("Attivita_Interna")
                dr_finale.Item("isVisita") = dr.Item("isVisita")


                dt_finale.Rows.Add(dr_finale)

            End If

        Next

        Return dt_finale

    End Function


    '##############################################################################################
    Public Function Leggi_Movimento_Scarico(ByVal piva As String,
                                            ByVal id_agenda As Integer,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As String

        Dim risposta As String = ""
        Dim strCau_Mov As String = CAU_SCARICO

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Movimento_Scarico()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From CI In GiasContext.Agenda
               Join Movimenti In GiasContext.Movimenti
                 On Movimenti.PIVA Equals CI.PIVA And
                    Movimenti.Id_Agenda Equals CI.Id_Agenda
               Where
              (CI.PIVA.Equals(piva)) _
              AndAlso (CI.Id_Agenda = id_agenda) _
              AndAlso strCau_Mov.Contains(Movimenti.Cau_Mov)
               Order By Movimenti.Id_Mov
               Select New With {
              .Piva = CI.PIVA,
              .Id_Agenda = CI.Id_Agenda,
              .Des_Lib = CI.des_lib,
              .Lav_Cod = CI.Lav_Cod,
              .Id_Mov = Movimenti.Id_Mov,
              .Data_Movimento = Movimenti.Data_Movimento
              }

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(TestataElem.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function



    '##############################################################################################
    Public Function Leggi_Contatti(ByVal piva As String,
                                   ByVal cod_rapporto As Integer,
                                   ByVal data As Date,
                                   ByRef objParametri As AgronicaCoreParametri,
                                    Optional isTimeSheetPersonale As Boolean = False
                                   ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Contatti()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim UtenteUsername As String = objParametri.UtenteUsername
            Dim UtenteConnesso =
                (From CU In GiasContext.ContattiXUtentiGias
                 Where CU.username.Equals(UtenteUsername)
                 Select CU.cod_contatto).FirstOrDefault()

            Dim TestataElem =
                From CI In GiasContext.Risorse_Umane
                Join Contatti In GiasContext.Contatti
                    On Contatti.Cod_Contatto Equals CI.Cod_Contatto And Contatti.Piva Equals CI.Piva
                Join Rapporti_Contabili In GiasContext.Rapporti_Contabili
                    On Rapporti_Contabili.Cod_Rapporto Equals CI.Cod_Rapporto
                Where
                    (CI.Piva.Equals(piva) OrElse (Contatti.Sa_Cod = -1)) AndAlso
                    (Not isTimeSheetPersonale OrElse UtenteUsername Is "" OrElse Contatti.Cod_Contatto.Equals(UtenteConnesso))
                Order By Contatti.Rag_Soc, Contatti.Cognome, Contatti.Nome
                Select New With {
                    .Cod_Risum = CI.Cod_RisUm,
                    .Rag_Soc = Contatti.Rag_Soc,
                    .Cognome = Contatti.Cognome,
                    .Nome = Contatti.Nome,
                    .Cod_Rapporto = CI.Cod_Rapporto,
                    .Dipendente = Rapporti_Contabili.Dipendente,
                    .Terzista = Rapporti_Contabili.Terzista,
                    .Rapporto_Des = Rapporti_Contabili.Rapporto_Des,
                    .Qualifica_Cod = CI.Qualifica_Cod,
                    .Validita_Inizio = CI.Validita_Inizio,
                    .Validita_Fine = CI.Validita_Fine
                    }


            'Filtro Dinamico
            If cod_rapporto <> 0 Then
                TestataElem = TestataElem.Where(Function(x) x.Cod_Rapporto = cod_rapporto)
            Else
                'Dipendenti
                TestataElem = TestataElem.Where(Function(x) x.Dipendente = 1 OrElse x.Cod_Rapporto = COD_CAPO_AREA OrElse x.Terzista = 1)
            End If

            If data <> AGRODATAINIZIO Then
                TestataElem = TestataElem.Where(Function(x) x.Validita_Inizio <= data)
                TestataElem = TestataElem.Where(Function(x) x.Validita_Fine >= data)
            End If

            ' Compongo la chiave
            Dim myList = TestataElem.ToList()
            For Each obj In myList
                If (obj.Rag_Soc = "" AndAlso (obj.Cognome <> "" AndAlso obj.Nome <> "")) Then
                    obj.Rag_Soc = obj.Rag_Soc & "" & obj.Cognome & " " & obj.Nome
                End If
            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(myList.Distinct().ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function Leggi_Rapporti_Contabili_Manodopera(ByVal piva As String,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Rapporti_Contabili_Manodopera()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From CI In GiasContext.Rapporti_Contabili
               Where
              (CI.Piva.Equals(piva) OrElse (CI.Cod_Rapporto < 0)) AndAlso
                 ((CI.Dipendente = 1) OrElse (CI.Terzista = 1) OrElse (CI.Cod_Rapporto = COD_CAPO_AREA))
               Order By CI.Rapporto_Des
               Select New With {
              .Cod_Rapporto = CI.Cod_Rapporto,
              .Rapporto_Des = CI.Rapporto_Des,
              .Dipendente = CI.Dipendente,
              .Terzista = CI.Terzista
              }

            ''Filtro Dinamico
            'If dipendente <> 0 THEN
            '    TestataElem = TestataElem.Where(Function(x) x.Dipendente = 1)
            'End If

            'If terzista <> 0 THEN
            '    TestataElem = TestataElem.Where(Function(x) x.Terzista = 1)
            'End If


            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(TestataElem.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function


    '##############################################################################################
    Public Function Leggi_Tipi_Imputazione(ByVal piva As String,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional isAttivitaInterna As Boolean = False
                                           ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Tipi_Imputazione()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From CI In GiasContext.Imputazioni_Tipi
               Join Imputazioni In GiasContext.Imputazioni
                 On Imputazioni.Piva_SuperUser Equals CI.Piva_SuperUser And
                    Imputazioni.Piva Equals CI.Piva And
                    Imputazioni.Tipo_Imputazione Equals CI.Tipo_Imputazione
               Where
              (CI.Piva.Equals(piva)) AndAlso
               (CI.Piva_SuperUser.Equals(Piva_SuperUser))
               Order By CI.Tipo_Imputazione_Des
               Select New With {
              .Piva = CI.Piva,
              .Tipo_Imputazione = CI.Tipo_Imputazione,
              .Tipo_Imputazione_Des = CI.Tipo_Imputazione_Des,
              .Progetto_Interno = Imputazioni.Imputazione_Interna
              }

            'Se vengo dal pulsante/elemento Attivita Interne, carico solo i progetti interni
            If isAttivitaInterna Then
                TestataElem = TestataElem.Where(Function(x) x.Progetto_Interno = 1)
            Else
                TestataElem = TestataElem.Where(Function(x) x.Progetto_Interno = 0)
            End If

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(TestataElem.Distinct().ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function



    '##############################################################################################
    Public Function Leggi_Raggruppamenti(ByVal piva As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Raggruppamenti()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From CI In GiasContext.Stalla_Raggruppamenti
               Join Stalla In GiasContext.Stalla
                 On Stalla.PIVA Equals CI.PIVA And
                    Stalla.STA_NUM Equals CI.STA_NUM And
                    Stalla.sa_cod Equals CI.sa_cod
               Join Centri_Aziendali In GiasContext.Centri_Aziendali
                   On Stalla.PIVA Equals Centri_Aziendali.PIVA And
                   Stalla.sa_cod Equals Centri_Aziendali.sa_cod
               Where
              (CI.PIVA.Equals(piva)) AndAlso
               (CI.PivaSuperUser.Equals(Piva_SuperUser))
               Order By CI.Raggruppamento_Des
               Select New With {
              .Piva = CI.PIVA,
              .Raggruppamento_Cod = CI.Raggruppamento_Cod,
              .Raggruppamento_Des = CI.Raggruppamento_Des & " (" & Centri_Aziendali.sa_nome & " - " & Stalla.STA_DES & ")"
              }

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(TestataElem.Distinct().ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Classi_Imputazione(ByVal piva As String,
                                             ByVal tipo_imputazione As Integer,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             Optional isAttivitaInterna As Boolean = False
                                             ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Classi_Imputazione()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From CI In GiasContext.Imputazioni_Classi
               Join Imputazioni In GiasContext.Imputazioni
                 On Imputazioni.Piva_SuperUser Equals CI.Piva_SuperUser And
                    Imputazioni.Piva Equals CI.Piva And
                    Imputazioni.Imputazione_Classe_Cod Equals CI.Imputazione_Classe_Cod
               Join Imputazioni_Tipi In GiasContext.Imputazioni_Tipi
                 On Imputazioni_Tipi.Piva_SuperUser Equals Imputazioni.Piva_SuperUser And
                    Imputazioni_Tipi.Piva Equals Imputazioni.Piva And
                    Imputazioni_Tipi.Tipo_Imputazione Equals Imputazioni.Tipo_Imputazione
               Where
              (CI.Piva.Equals(piva)) AndAlso
               (CI.Piva_SuperUser.Equals(Piva_SuperUser))
               Order By CI.Imputazione_Classe_Des
               Select New With {
              .Piva = CI.Piva,
              .Imputazione_Classe_Cod = CI.Imputazione_Classe_Cod,
              .Imputazione_Classe_Des = CI.Imputazione_Classe_Des,
              .Tipo_Imputazione = Imputazioni_Tipi.Tipo_Imputazione,
              .Tipo_Imputazione_Des = Imputazioni_Tipi.Tipo_Imputazione_Des,
              .Progetto_Interno = Imputazioni.Imputazione_Interna
              }

            'Filtro Dinamico
            If tipo_imputazione <> 0 Then
                TestataElem = TestataElem.Where(Function(x) x.Tipo_Imputazione = tipo_imputazione)
            End If

            'Se vengo dal pulsante/elemento Attivita Interne, carico solo i progetti interni
            If isAttivitaInterna Then
                TestataElem = TestataElem.Where(Function(x) x.Progetto_Interno = 1)
            Else
                TestataElem = TestataElem.Where(Function(x) x.Progetto_Interno = 0)
            End If

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(TestataElem.Distinct().ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function



    '##############################################################################################
    Public Function Leggi_Imputazioni(ByVal piva As String,
                                      ByVal tipo_imputazione As Integer,
                                      ByVal imputazione_classe_cod As Integer,
                                      ByVal id_attivita As Integer,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      ByVal tipo_fase As Integer,
                                      Optional ByVal budget As Integer = CInt(enum_Attvita_Consuntivo_Budget.Entrambi),
                                      Optional isAttivitaInterna As Boolean = False,
                                      Optional isFromTimeSheet As Boolean = False
                                      ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Imputazioni()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Select Case id_attivita

                Case 0

                    Select Case budget

                        Case CInt(enum_Attvita_Consuntivo_Budget.Entrambi) 'Tutti (Consuntivo e Budget)

                            Dim TestataElem =
                               From CI In GiasContext.Imputazioni
                               Join Imputazioni_Tipi In GiasContext.Imputazioni_Tipi
                                 On Imputazioni_Tipi.Piva_SuperUser Equals CI.Piva_SuperUser And
                                    Imputazioni_Tipi.Piva Equals CI.Piva And
                                    Imputazioni_Tipi.Tipo_Imputazione Equals CI.Tipo_Imputazione
                               Join Imputazioni_Classi In GiasContext.Imputazioni_Classi
                                 On Imputazioni_Classi.Piva_SuperUser Equals CI.Piva_SuperUser And
                                    Imputazioni_Classi.Piva Equals CI.Piva And
                                    Imputazioni_Classi.Imputazione_Classe_Cod Equals CI.Imputazione_Classe_Cod
                               Join Imputazioni_Fasi In GiasContext.Imputazioni_Fasi
                                 On Imputazioni_Fasi.Piva_SuperUser Equals CI.Piva_SuperUser And
                                    Imputazioni_Fasi.Piva Equals CI.Piva And
                                    Imputazioni_Fasi.Imputazione_Cod Equals CI.Imputazione_Cod
                               Where
                              (CI.Piva.Equals(piva)) AndAlso
                               (CI.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                               (CI.ChkImputazione = 1)
                               Order By CI.Imputazione_Nome
                               Select New With {
                              .Piva = CI.Piva,
                              .Tipo_Imputazione = CI.Tipo_Imputazione,
                              .Tipo_Imputazione_Des = Imputazioni_Tipi.Tipo_Imputazione_Des,
                              .Imputazione_Classe_Cod = CI.Imputazione_Classe_Cod,
                              .Imputazione_Classe_Des = Imputazioni_Classi.Imputazione_Classe_Des,
                              .Imputazione_Cod = CI.Imputazione_Cod,
                              .Imputazione_Nome = CI.Imputazione_Nome,
                              .Tipo_Fase = Imputazioni_Fasi.Tipo_Fase,
                              .Progetto_Interno = CI.Imputazione_Interna
                              }

                            'Filtro Dinamico
                            If tipo_imputazione <> 0 Then
                                TestataElem = TestataElem.Where(Function(x) x.Tipo_Imputazione = tipo_imputazione)
                            End If

                            If imputazione_classe_cod <> 0 Then
                                TestataElem = TestataElem.Where(Function(x) x.Imputazione_Classe_Cod = imputazione_classe_cod)
                            End If

                            If tipo_fase <> 0 Then
                                TestataElem = TestataElem.Where(Function(x) x.Tipo_Fase = tipo_fase)
                            End If

                            'Se arrivo da un quasiasi timesheet, non devo filtrare sulla colonna Progetto_Interno
                            If Not isFromTimeSheet Then
                                'Se vengo dal pulsante/elemento Attivita Interne, carico solo i progetti interni
                                If isAttivitaInterna Then
                                    TestataElem = TestataElem.Where(Function(x) x.Progetto_Interno = 1)
                                Else
                                    TestataElem = TestataElem.Where(Function(x) x.Progetto_Interno = 0)
                                End If
                            End If

                            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                            risposta = JsonConvert.SerializeObject(TestataElem.Distinct().ToList(), Formatting.None, serializerSettings)


                        Case Else

                            'Tipo_Utilizzo Puntuale (Consuntivo o Budget)
                            Dim TestataElem =
                               From CI In GiasContext.Imputazioni
                               Join Imputazioni_Tipi In GiasContext.Imputazioni_Tipi
                                 On Imputazioni_Tipi.Piva_SuperUser Equals CI.Piva_SuperUser And
                                    Imputazioni_Tipi.Piva Equals CI.Piva And
                                    Imputazioni_Tipi.Tipo_Imputazione Equals CI.Tipo_Imputazione
                               Join Imputazioni_Classi In GiasContext.Imputazioni_Classi
                                 On Imputazioni_Classi.Piva_SuperUser Equals CI.Piva_SuperUser And
                                    Imputazioni_Classi.Piva Equals CI.Piva And
                                    Imputazioni_Classi.Imputazione_Classe_Cod Equals CI.Imputazione_Classe_Cod
                               Join Imputazioni_Fasi In GiasContext.Imputazioni_Fasi
                                 On Imputazioni_Fasi.Piva_SuperUser Equals CI.Piva_SuperUser And
                                    Imputazioni_Fasi.Piva Equals CI.Piva And
                                    Imputazioni_Fasi.Imputazione_Cod Equals CI.Imputazione_Cod
                               Join Attivita In GiasContext.Attivita
                                 On Imputazioni_Fasi.Imputazione_Fase_Cod Equals Attivita.ID_Attivita
                               Where
                              (CI.Piva.Equals(piva)) AndAlso
                               (CI.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                               (CI.ChkImputazione = 1) AndAlso
                               (Attivita.Tipo_Utilizzo = budget OrElse Attivita.Tipo_Utilizzo = CInt(enum_Attvita_Consuntivo_Budget.Entrambi))
                               Order By CI.Imputazione_Nome
                               Select New With {
                              .Piva = CI.Piva,
                              .Tipo_Imputazione = CI.Tipo_Imputazione,
                              .Tipo_Imputazione_Des = Imputazioni_Tipi.Tipo_Imputazione_Des,
                              .Imputazione_Classe_Cod = CI.Imputazione_Classe_Cod,
                              .Imputazione_Classe_Des = Imputazioni_Classi.Imputazione_Classe_Des,
                              .Imputazione_Cod = CI.Imputazione_Cod,
                              .Imputazione_Nome = CI.Imputazione_Nome,
                              .Tipo_Fase = Imputazioni_Fasi.Tipo_Fase,
                              .Progetto_Interno = CI.Imputazione_Interna
                              }


                            'Filtro Dinamico
                            If tipo_imputazione <> 0 Then
                                TestataElem = TestataElem.Where(Function(x) x.Tipo_Imputazione = tipo_imputazione)
                            End If

                            If imputazione_classe_cod <> 0 Then
                                TestataElem = TestataElem.Where(Function(x) x.Imputazione_Classe_Cod = imputazione_classe_cod)
                            End If

                            If tipo_fase <> 0 Then
                                TestataElem = TestataElem.Where(Function(x) x.Tipo_Fase = tipo_fase)
                            End If

                            'Se arrivo da un quasiasi timesheet, non devo filtrare sulla colonna Progetto_Interno
                            If Not isFromTimeSheet Then
                                'Se vengo dal pulsante/elemento Attivita Interne, carico solo i progetti interni
                                If isAttivitaInterna Then
                                    TestataElem = TestataElem.Where(Function(x) x.Progetto_Interno = 1)
                                Else
                                    TestataElem = TestataElem.Where(Function(x) x.Progetto_Interno = 0)
                                End If
                            End If

                            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                            risposta = JsonConvert.SerializeObject(TestataElem.Distinct().ToList(), Formatting.None, serializerSettings)

                    End Select


                Case Else

                    Dim TestataElem =
                       From CI In GiasContext.Imputazioni
                       Join Imputazioni_Tipi In GiasContext.Imputazioni_Tipi
                         On Imputazioni_Tipi.Piva_SuperUser Equals CI.Piva_SuperUser And
                            Imputazioni_Tipi.Piva Equals CI.Piva And
                            Imputazioni_Tipi.Tipo_Imputazione Equals CI.Tipo_Imputazione
                       Join Imputazioni_Classi In GiasContext.Imputazioni_Classi
                         On Imputazioni_Classi.Piva_SuperUser Equals CI.Piva_SuperUser And
                            Imputazioni_Classi.Piva Equals CI.Piva And
                            Imputazioni_Classi.Imputazione_Classe_Cod Equals CI.Imputazione_Classe_Cod
                       Join Imputazioni_Fasi In GiasContext.Imputazioni_Fasi
                         On Imputazioni_Fasi.Piva_SuperUser Equals CI.Piva_SuperUser And
                            Imputazioni_Fasi.Piva Equals CI.Piva And
                            Imputazioni_Fasi.Imputazione_Cod Equals CI.Imputazione_Cod
                       Where
                      (CI.Piva.Equals(piva)) AndAlso
                       (CI.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                       (CI.ChkImputazione = 1)
                       Order By CI.Imputazione_Nome
                       Select New With {
                      .Piva = CI.Piva,
                      .Tipo_Imputazione = CI.Tipo_Imputazione,
                      .Tipo_Imputazione_Des = Imputazioni_Tipi.Tipo_Imputazione_Des,
                      .Imputazione_Classe_Cod = CI.Imputazione_Classe_Cod,
                      .Imputazione_Classe_Des = Imputazioni_Classi.Imputazione_Classe_Des,
                      .Imputazione_Cod = CI.Imputazione_Cod,
                      .Imputazione_Nome = CI.Imputazione_Nome,
                      .Id_Attivita = Imputazioni_Fasi.Imputazione_Fase_Cod,
                      .Tipo_Fase = Imputazioni_Fasi.Tipo_Fase,
                      .Progetto_Interno = CI.Imputazione_Interna
                      }

                    'Filtro Dinamico
                    If tipo_imputazione <> 0 Then
                        TestataElem = TestataElem.Where(Function(x) x.Tipo_Imputazione = tipo_imputazione)
                    End If

                    If imputazione_classe_cod <> 0 Then
                        TestataElem = TestataElem.Where(Function(x) x.Imputazione_Classe_Cod = imputazione_classe_cod)
                    End If

                    If id_attivita <> 0 Then
                        TestataElem = TestataElem.Where(Function(x) x.Id_Attivita = id_attivita)
                    End If

                    If tipo_fase <> 0 Then
                        TestataElem = TestataElem.Where(Function(x) x.Tipo_Fase = tipo_fase)
                    End If

                    'Se arrivo da un quasiasi timesheet, non devo filtrare sulla colonna Progetto_Interno
                    If Not isFromTimeSheet Then
                        'Se vengo dal pulsante/elemento Attivita Interne, carico solo i progetti interni
                        If isAttivitaInterna Then
                            TestataElem = TestataElem.Where(Function(x) x.Progetto_Interno = 1)
                        Else
                            TestataElem = TestataElem.Where(Function(x) x.Progetto_Interno = 0)
                        End If
                    End If

                    Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                    risposta = JsonConvert.SerializeObject(TestataElem.Distinct().ToList(), Formatting.None, serializerSettings)

            End Select

        End Using

        Return risposta

    End Function




    '##############################################################################################
    Public Function Leggi_Specie_Linee_Produzioni(ByVal piva As String,
                                                  ByVal regolamento_cod As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Specie_Linee_Produzioni()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From CI In GiasContext.SpecieVegetali
               Join Linee_Produzioni_Mix In GiasContext.Linee_Produzioni_Mix
                 On Linee_Produzioni_Mix.Veg_Cod Equals CI.Veg_Cod
               Join Linee_Produzioni In GiasContext.Linee_Produzioni
                 On Linee_Produzioni_Mix.Piva Equals Linee_Produzioni.Piva And
                    Linee_Produzioni_Mix.Linea_Cod Equals Linee_Produzioni.Linea_Cod
               Where
              (Linee_Produzioni_Mix.Piva.Equals(piva))
               Order By CI.Veg_Des
               Select New With {
              .Veg_Cod = CI.Veg_Cod,
              .Veg_Des = CI.Veg_Des,
              .Regolamento_Cod = Linee_Produzioni.Reg_Cod
              }

            ''Filtro Dinamico
            'If regolamento_cod <> 0 THEN
            '    TestataElem = TestataElem.Where(Function(x) x.Regolamento_Cod = regolamento_cod)
            'End If

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(TestataElem.Distinct().ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function


    '##############################################################################################
    Public Function Leggi_Cultivar_Linee_Produzioni(ByVal piva As String,
                                                    ByVal veg_cod As Integer,
                                                    ByVal regolamento_cod As Integer,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Cultivar_Linee_Produzioni()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From CI In GiasContext.Cultivar
               Join SpecieVegetali In GiasContext.SpecieVegetali
                 On SpecieVegetali.Veg_Cod Equals CI.Veg_Cod
               Join Linee_Produzioni_Mix In GiasContext.Linee_Produzioni_Mix
                 On Linee_Produzioni_Mix.Cul_Cod Equals CI.Cul_Cod
               Join Linee_Produzioni In GiasContext.Linee_Produzioni
                 On Linee_Produzioni_Mix.Piva Equals Linee_Produzioni.Piva And
                    Linee_Produzioni_Mix.Linea_Cod Equals Linee_Produzioni.Linea_Cod
               Where
              (Linee_Produzioni_Mix.Piva.Equals(piva))
               Order By CI.Cul_Des
               Select New With {
              .Veg_Cod = CI.Veg_Cod,
              .Veg_Des = SpecieVegetali.Veg_Des,
              .Cul_Cod = CI.Cul_Cod,
              .Cul_Des = CI.Cul_Des,
              .Regolamento_Cod = Linee_Produzioni.Reg_Cod
              }

            'Filtro Dinamico
            If veg_cod <> 0 Then
                TestataElem = TestataElem.Where(Function(x) x.Veg_Cod = veg_cod)
            End If
            If regolamento_cod <> 0 Then
                TestataElem = TestataElem.Where(Function(x) x.Regolamento_Cod = regolamento_cod)
            End If

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(TestataElem.Distinct().ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function



    '##############################################################################################
    Public Function Leggi_Linee_Produzioni(ByVal piva As String,
                                           ByVal veg_cod As Integer,
                                           ByVal cul_cod As Integer,
                                           ByVal regolamento_cod As Integer,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Linee_Produzioni()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From CI In GiasContext.Linee_Produzioni
               Join Linee_Produzioni_Mix In GiasContext.Linee_Produzioni_Mix
                 On Linee_Produzioni_Mix.Piva Equals CI.Piva And
                    Linee_Produzioni_Mix.Linea_Cod Equals CI.Linea_Cod
               Group Join Specie In GiasContext.SpecieVegetali
                 On Specie.Veg_Cod Equals Linee_Produzioni_Mix.Veg_Cod Into Specie_Group = Group
               From _Specie_Group In Specie_Group.DefaultIfEmpty()
               Group Join Cultivar In GiasContext.Cultivar
                 On Linee_Produzioni_Mix.Cul_Cod Equals Cultivar.Cul_Cod Into Cultivar_Group = Group
               From _Cultivar_Group In Cultivar_Group.DefaultIfEmpty()
               Where
              (Linee_Produzioni_Mix.Piva.Equals(piva))
               Order By _Cultivar_Group.Cul_Des
               Select New With {
              .Veg_Cod = Linee_Produzioni_Mix.Veg_Cod,
              .Veg_Des = If(_Specie_Group Is Nothing, "", _Specie_Group.Veg_Des),
              .Cul_Cod = Linee_Produzioni_Mix.Cul_Cod,
              .Cul_Des = If(_Cultivar_Group Is Nothing, "", _Cultivar_Group.Cul_Des),
              .Linea_Cod = CI.Linea_Cod,
              .Linea_Des = CI.Linea_Des & " (" & CI.Linea_Cod_Des & ")",
              .Regolamento_Cod = CI.Reg_Cod
              }

            'Filtro Dinamico
            If veg_cod <> 0 Then
                TestataElem = TestataElem.Where(Function(x) x.Veg_Cod = veg_cod)
            End If
            If cul_cod > 0 Then
                TestataElem = TestataElem.Where(Function(x) x.Cul_Cod = cul_cod)
            End If
            If regolamento_cod <> 0 Then
                TestataElem = TestataElem.Where(Function(x) x.Regolamento_Cod = regolamento_cod)
            End If


            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(TestataElem.Distinct().ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function



    '##############################################################################################
    Public Function Leggi_Lotti_Linee_Produzioni(ByVal piva As String,
                                                 ByVal veg_cod As Integer,
                                                 ByVal cul_cod As Integer,
                                                 ByVal regolamento_cod As Integer,
                                                 ByVal linea_cod As Integer,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Lotti_Linee_Produzioni()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From CI In GiasContext.Cultivar
               Join SpecieVegetali In GiasContext.SpecieVegetali
                 On SpecieVegetali.Veg_Cod Equals CI.Veg_Cod
               Join Linee_Produzioni_Mix In GiasContext.Linee_Produzioni_Mix
                 On Linee_Produzioni_Mix.Cul_Cod Equals CI.Cul_Cod
               Join Linee_Produzioni In GiasContext.Linee_Produzioni
                 On Linee_Produzioni_Mix.Piva Equals Linee_Produzioni.Piva And
                    Linee_Produzioni_Mix.Linea_Cod Equals Linee_Produzioni.Linea_Cod
               Join Trasformazioni In GiasContext.Trasformazioni
                 On Linee_Produzioni_Mix.Piva Equals Trasformazioni.PIVA And
                    Linee_Produzioni_Mix.Linea_Cod Equals Trasformazioni.Linea_Cod
               Where
              (Linee_Produzioni_Mix.Piva.Equals(piva))
               Order By CI.Cul_Des
               Select New With {
              .Veg_Cod = CI.Veg_Cod,
              .Veg_Des = SpecieVegetali.Veg_Des,
              .Cul_Cod = CI.Cul_Cod,
              .Cul_Des = CI.Cul_Des,
              .Linea_Cod = Linee_Produzioni.Linea_Cod,
              .Linea_Des = Linee_Produzioni.Linea_Des & " (" & Linee_Produzioni.Linea_Cod_Des & ")",
              .Trasformazione_Cod = Trasformazioni.Id_Trasformazione,
              .Trasformazione_Des = Trasformazioni.Trasformazione_Des,
              .Regolamento_Cod = Linee_Produzioni.Reg_Cod
              }

            'Filtro Dinamico
            If veg_cod <> 0 Then
                TestataElem = TestataElem.Where(Function(x) x.Veg_Cod = veg_cod)
            End If
            If cul_cod <> 0 Then
                TestataElem = TestataElem.Where(Function(x) x.Cul_Cod = cul_cod)
            End If
            If regolamento_cod <> 0 Then
                TestataElem = TestataElem.Where(Function(x) x.Regolamento_Cod = regolamento_cod)
            End If
            If linea_cod <> 0 Then
                TestataElem = TestataElem.Where(Function(x) x.Linea_Cod = linea_cod)
            End If

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(TestataElem.Distinct().ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function



    '##############################################################################################
    Public Function Leggi_ID_Attivita_Eredita(ByVal piva As String,
                                              ByVal id_agenda As Integer,
                                              ByRef objParametri As AgronicaCoreParametri,
                                                Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing
                                              ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_ID_Attivita_Eredita()"
        Dim strTab_Imputazione As String = "{EREDITA}"

        'Dim gefutils AS New Gias_EF_Utility
        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        Dim TestataElem =
               From CI In GiasContext.CDG_Testata
               Join Attivita In GiasContext.Attivita
                 On Attivita.ID_Attivita Equals CI.Id_Attivita
               Where
               (CI.Piva.Equals(piva)) _
               AndAlso CI.Id_Agenda = id_agenda _
               AndAlso CI.Budget = 0 _
               AndAlso strTab_Imputazione.Contains(CI.Tab_Imputazione)
               Select New With {
              .Id_Attivita = CI.Id_Attivita,
              .Attivita_Des = Attivita.Desc,
              .Poliennale = Attivita.Attivita_Poliannuale
              }

        Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
        risposta = JsonConvert.SerializeObject(TestataElem.Distinct().ToList(), Formatting.None, serializerSettings)


        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

        Return risposta

    End Function




    '##############################################################################################
    Public Function Leggi_Tipi_Macchine(ByVal piva As String,
                                        ByVal tipo As Integer,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Tipi_Macchine()"

        Dim gefutils As New Gias_EF_Utility


        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From CI In GiasContext.Macchine
               Join Parco_Macchine In GiasContext.Parco_Macchine
                 On Left(Parco_Macchine.Class_Code, 2) Equals CI.CLASS_CODE
               Where
              (Parco_Macchine.Piva.Equals(piva) OrElse Parco_Macchine.Sa_Cod = -1) AndAlso
               Parco_Macchine.Tipo = tipo
               Order By CI.CLASS_DESC
               Select New With {
                  .CLASS_CODE_ROOT = CI.CLASS_CODE,
                  .CLASS_DESC = CI.CLASS_DESC
              }

            Dim elementi = TestataElem.Distinct().OrderBy(Function(x) x.CLASS_DESC).ToList()

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(elementi, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function


    Public Function TrovaRigheCDG(ByVal piva As String,
                                  ByVal Vecchio_Tipo_Inser_Dati As Integer,
                                  ByRef CDG_Dati_Generali As List(Of CDG_Testata),
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As String

        Dim messaggioErrore As String = ""

        Dim risposta As Boolean = False

        Dim pivaSuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL.TrovaRigheCDG()"

        Try

            Dim gefutils As New Gias_EF_Utility

            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                ' GiasContext.ContextOptions.LazyLoadingEnabled = False

                giasContext.Database.CommandTimeout = 3600

                CDG_Dati_Generali = (From Testata In giasContext.CDG_Testata.Include("CDG_Dettagli")
                                     Where Testata.Piva_Superuser.Equals(pivaSuperUser) AndAlso
                                           Testata.Piva.Equals(piva) AndAlso
                                           Testata.Budget = 0 AndAlso
                                           Testata.Vecchio_Tipo_Inser_Dati = Vecchio_Tipo_Inser_Dati).ToList()
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    Public Function Leggi_IdCDG_Da_IdAgenda(ByVal piva As String, ByVal id_agenda As Integer, ByRef objParametri As AgronicaCoreParametri) As List(Of Integer)

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.CDG_DAL_R.Leggi_IdCDG_Da_IdAgenda()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable
        Dim id_CDG As New List(Of Integer)

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT *  ")
            StrSQL.AppendLine(" FROM  CDG_Testata ")
            StrSQL.AppendLine(" WHERE Piva_SuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine(" AND Budget = 0 AND Piva = " & Agro_SQL_SaveText_NULL(piva))
            StrSQL.AppendLine(" AND id_Agenda = " & Agro_SQL_SaveNum(id_agenda))

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) Then
                For Each dr As DataRow In DT.Rows
                    id_CDG.Add(CInt(dr.Item("id_CDG")))
                Next
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return id_CDG

    End Function

    Public Function Ipno_Lettura_Agenda_Senza_CDG(ByVal PIVA As String,
                                                  ByVal Id_Agenda As Integer,
                                                  ByVal Raccoglitore_Cod As Integer,
                                                  ByVal strFiltro As String,
                                                  ByVal bForzato As Boolean,
                                                  ByVal FinestraTemp_Inizio As Date,
                                                  ByVal FinestraTemp_Fine As Date,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Ipno_Lettura_Agenda_Senza_CDG()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim strJoin As New System.Text.StringBuilder
        Dim dt As DataTable


        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.Des_Lib, Movimenti.Data_Movimento, IsNull(Attivita.Id_Attivita, 0) as xId_Attivita, IsNull(Attivita.[Desc], '') as xAttivita_Des, Raccoglitore_Cod, Rag_Soc ")
            strSql.AppendLine(" FROM Agenda ")
            strSql.AppendLine(" INNER JOIN Imprese ON Agenda.Piva = Imprese.Piva ")
            strSql.AppendLine(" INNER JOIN Movimenti ON Agenda.piva = Movimenti.piva AND Agenda.id_agenda = Movimenti.id_agenda ")

            strSql.AppendLine(" LEFT OUTER JOIN Attivita On Attivita.Id_Attivita = Agenda.Id_Attivita ")
            strSql.AppendLine(" WHERE Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            strSql.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
            strSql.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")

            'Filtro
            Select Case Raccoglitore_Cod

                Case 0

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Agenda.id_agenda = " & Id_Agenda)
                    End If

                Case Else

                    strSql.AppendLine(" AND Agenda.Raccoglitore_Cod = " & Raccoglitore_Cod)

            End Select


            If Not bForzato Then

                'Eseguito dal report costi (analizza tutti le operazioni di agenda di cui ho precedentemente eliminato il tab_imputazion 'EREDITA')

                '1 Condizione:
                'Considero le operazioni di agenda che hanno un movimento di scarico/visite/carico zoo e che non hanno ancora alcun cdg)
                strSql.AppendLine(" And (Cau_Mov In ('7350', '6852', '3700', '3750')  And (Not Exists (Select 1 From Mov_Dettagli_Riferimenti Where Piva = Agenda.Piva And Id_Agenda = Agenda.Id_Agenda And Lav_Cod_Rif = 4500 )) ")


                '2 Condizione Alternativa:
                'Operazioni che hanno una vecchia imputazione in cdg
                strSql.AppendLine(" Or Exists(Select 1 From Mov_Dettagli_Riferimenti Where Piva = Agenda.Piva And Id_Agenda = Agenda.Id_Agenda And Lav_Cod_Rif = 4500 ")
                strSql.AppendLine(" And Agenda.Data_Modifica > Mov_Dettagli_Riferimenti.Data_Creazione and Abs(datediff(second, Agenda.Data_Modifica, Mov_Dettagli_Riferimenti.Data_Creazione  )) > 1 )) ")


            End If


            strSql.AppendLine(" And  Agenda.Id_Agenda Not In (Select Id_Agenda From Cdg_Testata Where Vecchio_Tipo_Inser_Dati <> 0 And Budget = 0)  ")

            'Altre operazioni del raccoglitore da byapssare
            strSql.AppendLine(" And isnull(Agenda.Stato_Export_2, 0) <> -1 ")

            If strFiltro <> "" Then
                strSql.AppendLine(" And (" & strFiltro & ")   ")
            End If


            strSql.AppendLine(" ORDER BY  Agenda.Id_Agenda Asc, Movimenti.Data_Movimento ASC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return dt

    End Function




    Public Function Leggi_Zoo_Dettagli(ByVal piva As String,
                                            ByVal id_agenda As Long,
                                            ByVal data_movimento As Date,
                                            ByVal bDT As Boolean,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                            Optional ByVal Prezzo_Totale As Double = 0
                                           ) As Object

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Impianti_Dettagli()"

        'Dim gefutils AS New Gias_EF_Utility
        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim obj_Zoo As New AgronicaCoreAnagrafeDAL.Zoo_Animali

        Dim DT As DataTable

        Dim Residuo As Double = 100
        Dim Residuo_Check As Double = 100
        Dim Counter As Integer = 0

        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If



        Dim dataMovimentoDate_Inizio As Date = New Date(data_movimento.Year, data_movimento.Month, data_movimento.Day, 0, 0, 0)
        Dim dataMovimentoDate_Fine As Date = New Date(data_movimento.Year, data_movimento.Month, data_movimento.Day, 23, 59, 59)

        Dim agendaTmp = (From Agenda In GiasContext.Agenda Where Agenda.Id_Agenda = id_agenda AndAlso Agenda.PIVA = piva).FirstOrDefault
        Dim Lav_Cod = agendaTmp.Lav_Cod
        Dim myList = Nothing
        Select Case Lav_Cod
            Case LAVCOD_NASCITA_ANIMALI, LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_ACQUISTO_ANIMALI, LAVCOD_VENDITA_ANIMALI, LAVCOD_TRASFERIMENTO_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI, LAVCOD_MORTE_ANIMALI, LAVCOD_SPOSTAMENTI_ZOO

                Dim TestataElem =
           From CI In GiasContext.Mov_Destinazioni
           Join Movimenti In GiasContext.Movimenti
             On Movimenti.PIVA Equals CI.Piva And
                Movimenti.Id_Agenda Equals CI.Id_Agenda And
                Movimenti.Id_Mov Equals CI.Id_Mov
           Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli
               On Movimenti_Dettagli.PIVA Equals Movimenti.PIVA And
                  Movimenti_Dettagli.Id_Agenda Equals Movimenti.Id_Agenda And
                  Movimenti_Dettagli.Id_Mov Equals Movimenti.Id_Mov And
                  Movimenti_Dettagli.Id_Mov_Det Equals CI.Id_Mov_Det
           Join Zoo_Animali In GiasContext.Zoo_Animali
               On Zoo_Animali.PIVA Equals Movimenti_Dettagli.PIVA And
                  Zoo_Animali.Cod_Progetto Equals Movimenti_Dettagli.Cod_Progetto
           Join Centri_Aziendali In GiasContext.Centri_Aziendali
                On Centri_Aziendali.PIVA Equals Movimenti_Dettagli.PIVA And
                   Centri_Aziendali.sa_cod Equals Movimenti_Dettagli.Sa_Cod
           Join Zoo_Animali_Distinte In GiasContext.Zoo_Animali_Distinte
             On Zoo_Animali_Distinte.PIVA Equals Zoo_Animali.PIVA And
                Zoo_Animali_Distinte.Cod_Animale Equals Zoo_Animali.Cod_Progetto
           Join Specie In GiasContext.Lista_Specie_Animali
             On Specie.GEN_COD Equals Zoo_Animali.GEN_COD And
                Specie.SPE_COD Equals Zoo_Animali.SPE_COD
           Join Razze In GiasContext.Lista_Razze_Animali
             On Razze.GEN_COD Equals Zoo_Animali.GEN_COD And
                Razze.SPE_COD Equals Zoo_Animali.SPE_COD And
                Razze.RAZ_COD Equals Zoo_Animali.RAZ_COD
           Where
          (CI.Piva.Equals(piva)) _
          AndAlso (CI.Id_Agenda = id_agenda) _
           AndAlso (CI.Tipo_Destinazione = CostantiPersonalizzate.STALLA OrElse CI.Tipo_Destinazione = TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA) _
           AndAlso Zoo_Animali_Distinte.Validita_Inizio <= dataMovimentoDate_Fine _
           AndAlso Zoo_Animali_Distinte.Validita_Fine >= dataMovimentoDate_Inizio _
           AndAlso (Movimenti.Cau_Mov <> CostantiPersonalizzate.CAU_PESATURA_ANIMALI)
           Order By CI.QuotaDistribuzione Descending
           Select New With {
          .Piva = CI.Piva,
          .Sa_Cod = Centri_Aziendali.sa_cod,
          .Sa_Nome = Centri_Aziendali.sa_nome,
          .Appezza = CI.Appezza,
          .Id_Destinazione = CI.Id_Destinazione,
          .Key = "",
          .Gen_Cod = Specie.GEN_COD,
          .Spe_Cod = Specie.SPE_COD,
          .Veg_Des = Specie.SPE_DES,
          .Raz_Cod = Razze.RAZ_COD,
          .Raz_Des = Razze.RAZ_DES,
          .Descrizione = Zoo_Animali.Nome,
          .Progetto_Cod = 0,
          .Prezzo_Unitario = Movimenti_Dettagli.Prezzo_Unitario,
          .Progetto_Nome = Zoo_Animali_Distinte.Progetto_Nome,
          .Progetto_Des = Zoo_Animali_Distinte.Progetto_Des,
          .Superficie = CI.Qta2,
          .Valore = If(CI.QuotaDistribuzione = 0, If(Prezzo_Totale = 0, 0, Movimenti_Dettagli.Prezzo_Unitario * 100 / Prezzo_Totale), CI.QuotaDistribuzione * 100),
          .Cod_Animale = Zoo_Animali.Cod_Progetto,
          .Cod_Animale_Distinte = Zoo_Animali_Distinte.Cod_Progetto,
          .STA_NUM = 0,
          .Raggruppamento_Cod = 0
          }

                'And strCau_Progetto.Contains(Zoo_Animali_Distinte.Cau_Progetto) _
                ' Compongo la chiave
                myList = TestataElem.Distinct().ToList()

            Case LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI

                Dim TestataElem =
                   (From CI In GiasContext.Mov_Destinazioni
                    Join Movimenti In GiasContext.Movimenti
                     On Movimenti.PIVA Equals CI.Piva And
                        Movimenti.Id_Agenda Equals CI.Id_Agenda And
                        Movimenti.Id_Mov Equals CI.Id_Mov
                    Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli
                       On Movimenti_Dettagli.PIVA Equals Movimenti.PIVA And
                          Movimenti_Dettagli.Id_Agenda Equals Movimenti.Id_Agenda And
                          Movimenti_Dettagli.Id_Mov Equals Movimenti.Id_Mov
                    Join Zoo_Animali In GiasContext.Zoo_Animali
                       On Zoo_Animali.PIVA Equals CI.Piva And
                          Zoo_Animali.Cod_Progetto Equals CI.Id_Destinazione
                    Join Centri_Aziendali In GiasContext.Centri_Aziendali
                        On Centri_Aziendali.PIVA Equals CI.Piva And
                           Centri_Aziendali.sa_cod Equals CI.Sa_Cod_Riferimento
                    Join Zoo_Animali_Distinte In GiasContext.Zoo_Animali_Distinte
                     On Zoo_Animali_Distinte.PIVA Equals Zoo_Animali.PIVA And
                        Zoo_Animali_Distinte.Cod_Animale Equals Zoo_Animali.Cod_Progetto
                    Join Specie In GiasContext.Lista_Specie_Animali
                     On Specie.GEN_COD Equals Zoo_Animali.GEN_COD And
                        Specie.SPE_COD Equals Zoo_Animali.SPE_COD
                    Join Razze In GiasContext.Lista_Razze_Animali
                     On Razze.GEN_COD Equals Zoo_Animali.GEN_COD And
                        Razze.SPE_COD Equals Zoo_Animali.SPE_COD And
                        Razze.RAZ_COD Equals Zoo_Animali.RAZ_COD
                    Where
                  (CI.Piva.Equals(piva)) _
                  AndAlso (CI.Id_Agenda = id_agenda) _
                   AndAlso (CI.Tipo_Destinazione = 1) _
                   AndAlso (Movimenti.Cau_Mov = CAU_ALIMENTAZIONE) _
                   AndAlso Zoo_Animali_Distinte.Validita_Inizio <= dataMovimentoDate_Fine _
                   AndAlso Zoo_Animali_Distinte.Validita_Fine >= dataMovimentoDate_Inizio
                    Order By CI.QuotaDistribuzione Descending
                    Select New With {
                  .Piva = CI.Piva,
                  .Sa_Cod = Centri_Aziendali.sa_cod,
                  .Sa_Nome = Centri_Aziendali.sa_nome,
                  .Appezza = CI.Appezza,
                  .Id_Destinazione = CI.Id_Destinazione,
                  .Key = "",
                  .Gen_Cod = Specie.GEN_COD,
                  .Spe_Cod = Specie.SPE_COD,
                  .Veg_Des = Specie.SPE_DES,
                  .Raz_Cod = Razze.RAZ_COD,
                  .Raz_Des = Razze.RAZ_DES,
                  .Descrizione = Zoo_Animali.Nome,
                  .Progetto_Cod = 0,
                  .Progetto_Nome = Zoo_Animali_Distinte.Progetto_Nome,
                  .Progetto_Des = Zoo_Animali_Distinte.Progetto_Des,
                  .Superficie = CI.Qta2,
                  .Valore = If(CI.QuotaDistribuzione Is Nothing, 0, CI.QuotaDistribuzione * 100),
                  .Cod_Animale = Zoo_Animali.Cod_Progetto,
                  .Cod_Animale_Distinte = Zoo_Animali_Distinte.Cod_Progetto,
                  .STA_NUM = 0,
                  .Raggruppamento_Cod = 0,
                  .Selected = True
                  }).Distinct

                'And strCau_Progetto.Contains(Zoo_Animali_Distinte.Cau_Progetto) _
                ' Compongo la chiave
                myList = TestataElem.Distinct().ToList()


            Case LAVCOD_VACCINAZIONI_ANIMALI, LAVCOD_CUREMEDICAMENTI_ANIMALI

                Dim TestataElem =
                   (From CI In GiasContext.Mov_Destinazioni
                    Join Agenda In GiasContext.Agenda
                     On Agenda.PIVA Equals CI.Piva And
                        Agenda.Id_Agenda Equals CI.Id_Agenda
                    Join Movimenti In GiasContext.Movimenti
                     On Movimenti.PIVA Equals CI.Piva And
                        Movimenti.Id_Agenda Equals CI.Id_Agenda And
                        Movimenti.Id_Mov Equals CI.Id_Mov
                    Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli
                       On Movimenti_Dettagli.PIVA Equals Movimenti.PIVA And
                          Movimenti_Dettagli.Id_Agenda Equals Movimenti.Id_Agenda And
                          Movimenti_Dettagli.Id_Mov Equals Movimenti.Id_Mov
                    Join Zoo_Animali In GiasContext.Zoo_Animali
                       On Zoo_Animali.PIVA Equals CI.Piva And
                          Zoo_Animali.Cod_Progetto Equals CI.Id_Destinazione
                    Join Centri_Aziendali In GiasContext.Centri_Aziendali
                        On Centri_Aziendali.PIVA Equals Agenda.PIVA And
                           Centri_Aziendali.sa_cod Equals Agenda.Sa_Cod
                    Join Zoo_Animali_Distinte In GiasContext.Zoo_Animali_Distinte
                     On Zoo_Animali_Distinte.PIVA Equals Zoo_Animali.PIVA And
                        Zoo_Animali_Distinte.Cod_Animale Equals Zoo_Animali.Cod_Progetto
                    Join Specie In GiasContext.Lista_Specie_Animali
                     On Specie.GEN_COD Equals Zoo_Animali.GEN_COD And
                        Specie.SPE_COD Equals Zoo_Animali.SPE_COD
                    Join Razze In GiasContext.Lista_Razze_Animali
                     On Razze.GEN_COD Equals Zoo_Animali.GEN_COD And
                        Razze.SPE_COD Equals Zoo_Animali.SPE_COD And
                        Razze.RAZ_COD Equals Zoo_Animali.RAZ_COD
                    Where
                  (CI.Piva.Equals(piva)) _
                  AndAlso (CI.Id_Agenda = id_agenda) _
                   AndAlso (CI.Tipo_Destinazione = 1) _
                   AndAlso (Movimenti.Cau_Mov = CAU_TRATTAMENTO_ZOO) _
                   AndAlso Zoo_Animali_Distinte.Validita_Inizio <= dataMovimentoDate_Fine _
                   AndAlso Zoo_Animali_Distinte.Validita_Fine >= dataMovimentoDate_Inizio
                    Order By CI.QuotaDistribuzione Descending
                    Select New With {
                  .Piva = CI.Piva,
                  .Sa_Cod = Centri_Aziendali.sa_cod,
                  .Sa_Nome = Centri_Aziendali.sa_nome,
                  .Appezza = CI.Appezza,
                  .Id_Destinazione = CI.Id_Destinazione,
                  .Key = "",
                  .Gen_Cod = Specie.GEN_COD,
                  .Spe_Cod = Specie.SPE_COD,
                  .Veg_Des = Specie.SPE_DES,
                  .Raz_Cod = Razze.RAZ_COD,
                  .Raz_Des = Razze.RAZ_DES,
                  .Descrizione = Zoo_Animali.Nome,
                  .Progetto_Cod = 0,
                  .Progetto_Nome = Zoo_Animali_Distinte.Progetto_Nome,
                  .Progetto_Des = Zoo_Animali_Distinte.Progetto_Des,
                  .Superficie = CI.Qta2,
                  .Valore = If(CI.QuotaDistribuzione Is Nothing, 0, CI.QuotaDistribuzione * 100),
                  .Cod_Animale = Zoo_Animali.Cod_Progetto,
                  .Cod_Animale_Distinte = Zoo_Animali_Distinte.Cod_Progetto,
                  .STA_NUM = 0,
                  .Raggruppamento_Cod = 0,
                  .Selected = True
                  }).Distinct

                'And strCau_Progetto.Contains(Zoo_Animali_Distinte.Cau_Progetto) _
                ' Compongo la chiave
                myList = TestataElem.Distinct().ToList()

        End Select

        If myList IsNot Nothing Then    
                
            Dim test As New List(Of Tuple(Of Integer, String))()
            For Each obj In myList
                Dim codAnimale As Integer = CType(obj, Object).Cod_Animale
                Dim pivaa As String = CType(obj, Object).Piva
                If Not test.Any(Function(x) x.Item1 = codAnimale AndAlso x.Item2 = piva) Then
                    test.Add(Tuple.Create(codAnimale, pivaa))
                End If
            Next
            Dim gruppi = test.GroupBy(Function(x) x.Item2)

            Dim dizionarioZoo As New Dictionary(Of String, Object)

            For Each gruppo In gruppi
                Dim pivaa = gruppo.Key
                Dim listaCodAnimali = gruppo.Select(Function(x) x.Item1).Distinct().ToList()
                Dim dtZoo = obj_Zoo.Leggi_Giacenze(pivaa, 0, 0, 0, 0, data_movimento, objParametri, listCod_Animali:=listaCodAnimali)

                ' Converti DataTable in lista di ExpandoObject

                ' Crea dizionario chiave "piva-cod_animale"
                For Each row As DataRow In dtZoo.Rows
                    Dim key = row("Piva").ToString() & "-" & row("Cod_Animale").ToString()
                    dizionarioZoo(key) = row
                Next
            Next

            ' Aggiorna i valori su myList
            For Each obj In myList
                Dim key = obj.Piva & "-" & obj.Cod_Animale.ToString()
                If dizionarioZoo.ContainsKey(key) Then
                    Dim row = dizionarioZoo(key)
                    obj.STA_NUM = row("STA_NUM")
                    obj.Raggruppamento_Cod = row("Raggruppamento_Cod")
                End If
            Next
'            For Each obj In myList
'
'                Dim dtZoo = obj_Zoo.Leggi_Giacenze(obj.Piva, 0, 0, 0, obj.Cod_Animale, data_movimento, objParametri)
'                If dtZoo.Rows.Count > 0 Then
'                    obj.STA_NUM = dtZoo.Rows(0).Item("Sta_Num")
'                    obj.Raggruppamento_Cod = dtZoo.Rows(0).Item("Raggruppamento_Cod")
'                End If
'
'            Next

            For Each obj In myList

                obj.Key = obj.Piva & "-" & obj.Sa_Cod.ToString() & "-" & obj.Appezza.ToString() & "-" & obj.Id_Destinazione.ToString()

                Counter = Counter + 1

                'Determinazione Valore Ripartito
                obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                Residuo_Check = Math.Round(Residuo_Check - obj.Valore, 2)

                If Residuo_Check > obj.Valore And CInt(myList.Count) > Counter Then

                    'Aggiornamento Residuo
                    Residuo = Math.Round(Residuo - obj.Valore, 2)

                ElseIf CInt(myList.Count) > Counter Then

                    'Ripastisco il residuo ripartita da qui in avanti
                    obj.Valore = Math.Round(Residuo / (CInt(myList.Count) - Counter + 1), 2)
                    Residuo = Math.Round(Residuo - obj.Valore, 2)

                Else

                    'Ultimo--> Impostazione residuo per evitare sfridi
                    If Residuo > 0 Then
                        obj.Valore = Residuo
                    Else
                        obj.Valore = 0
                    End If

                    Residuo = 0

                End If

            Next
        Else
            myList = New List(Of Object)
        End If


        Select Case bDT

            Case True

                Dim ut As New Gias_EF_Utility
                DT = ut.ObjectQueryToDataTable(myList)

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return DT

            Case False

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

                If bCloseContext Then
                    GiasContext.Dispose()
                    GiasContext = Nothing
                End If

                Return risposta

        End Select

        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

    End Function




    Public Function Leggi_Zoo_Dettagli_Ipno(ByVal piva As String,
                                            ByVal id_agenda As Long,
                                            ByVal data_movimento As Date,
                                            ByVal bDT As Boolean,
                                            ByRef objParametri As AgronicaCoreParametri
                                           ) As Object

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Zoo_Dettagli_Ipno()"

        Dim gefutils As New Gias_EF_Utility

        Dim obj_Zoo As New AgronicaCoreAnagrafeDAL.Zoo_Animali

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim DT As DataTable

        Dim Residuo As Double = 100
        Dim Counter As Integer = 0
        Dim Costo_Totale As Double = 0

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim agendaTmp = (From Agenda In GiasContext.Agenda Where Agenda.Id_Agenda = id_agenda AndAlso Agenda.PIVA = piva).FirstOrDefault
            Dim Lav_Cod = agendaTmp.Lav_Cod
            Dim myList


            Select Case Lav_Cod
                Case LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_VENDITA_ANIMALI, LAVCOD_TRASFERIMENTO_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI
                    'D Nothing
                Case Else
                    'Incremento di 1 gg la data movimento
                    data_movimento = DateAdd("d", 1, data_movimento)
            End Select

            Select Case Lav_Cod


                Case LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI

                    Dim TestataElem =
                       (From CI In GiasContext.Mov_Destinazioni
                        Join Movimenti In GiasContext.Movimenti
                         On Movimenti.PIVA Equals CI.Piva And
                            Movimenti.Id_Agenda Equals CI.Id_Agenda And
                            Movimenti.Id_Mov Equals CI.Id_Mov
                        Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli
                           On Movimenti_Dettagli.PIVA Equals Movimenti.PIVA And
                              Movimenti_Dettagli.Id_Agenda Equals Movimenti.Id_Agenda And
                              Movimenti_Dettagli.Id_Mov Equals Movimenti.Id_Mov
                        Join Zoo_Animali In GiasContext.Zoo_Animali
                           On Zoo_Animali.PIVA Equals CI.Piva And
                              Zoo_Animali.Cod_Progetto Equals CI.Id_Destinazione
                        Join Centri_Aziendali In GiasContext.Centri_Aziendali
                            On Centri_Aziendali.PIVA Equals Movimenti_Dettagli.PIVA And
                               Centri_Aziendali.sa_cod Equals Movimenti_Dettagli.Sa_Cod
                        Join Zoo_Animali_Distinte In GiasContext.Zoo_Animali_Distinte
                         On Zoo_Animali_Distinte.PIVA Equals Zoo_Animali.PIVA And
                            Zoo_Animali_Distinte.Cod_Animale Equals Zoo_Animali.Cod_Progetto
                        Join Specie In GiasContext.Lista_Specie_Animali
                         On Specie.GEN_COD Equals Zoo_Animali.GEN_COD And
                            Specie.SPE_COD Equals Zoo_Animali.SPE_COD
                        Join Razze In GiasContext.Lista_Razze_Animali
                         On Razze.GEN_COD Equals Zoo_Animali.GEN_COD And
                            Razze.SPE_COD Equals Zoo_Animali.SPE_COD And
                            Razze.RAZ_COD Equals Zoo_Animali.RAZ_COD
                        Where
                      (CI.Piva.Equals(piva)) _
                      AndAlso (CI.Id_Agenda = id_agenda) _
                       AndAlso (CI.Tipo_Destinazione = 1) _
                       AndAlso (Movimenti.Cau_Mov = CAU_ALIMENTAZIONE) _
                       AndAlso (Zoo_Animali_Distinte.Validita_Inizio <= data_movimento) _
                       AndAlso (Zoo_Animali_Distinte.Validita_Fine >= data_movimento)
                        Order By CI.QuotaDistribuzione Descending
                        Select New With {
                      .Piva = CI.Piva,
                      .Sa_Cod = Centri_Aziendali.sa_cod,
                      .sa_nome = Centri_Aziendali.sa_nome,
                      .Appezza = CI.Appezza,
                      .Id_Destinazione = CI.Id_Destinazione,
                      .Key = "",
                      .Gen_Cod = Specie.GEN_COD,
                      .Spe_Cod = Specie.SPE_COD,
                      .Veg_Des = Specie.SPE_DES,
                      .Raz_Cod = Razze.RAZ_COD,
                      .RAZ_DES = Razze.RAZ_DES,
                      .Descrizione = Zoo_Animali.Nome,
                      .Progetto_Cod = 0,
                      .Progetto_Nome = Zoo_Animali_Distinte.Progetto_Nome,
                      .Progetto_Des = Zoo_Animali_Distinte.Progetto_Des,
                      .Superficie = CI.Qta2,
                      .Valore = If(CI.QuotaDistribuzione Is Nothing, 0, CI.QuotaDistribuzione * 100),
                      .Cod_Animale = Zoo_Animali_Distinte.Cod_Animale,
                      .Cod_Progetto = Zoo_Animali_Distinte.Cod_Progetto,
                      .Codice_Distinta = Zoo_Animali_Distinte.Codice_Distinta,
                      .Distinta_Chiusa = Zoo_Animali_Distinte.Distinta_Chiusa,
                      .STA_NUM = 0,
                      .Raggruppamento_Cod = 0,
                      .Raggruppamento_Des = "",
                      .Selected = True,
                      .Stato_Des = "",
                      .Matricola = "",
                      .STA_DES = "",
                      .SPE_DES = "",
                      .IPRO_DES = "",
                      .Tipo_Des = "",
                      .Progetto = "",
                      .Dat_Nascita = "01/01/1900",
                      .Rag_Soc = ""
                      }).Distinct

                    'And strCau_Progetto.Contains(Zoo_Animali_Distinte.Cau_Progetto) _
                    ' Compongo la chiave
                    myList = TestataElem.Distinct().ToList()


                Case LAVCOD_PESATURA_ANIMALI

                    Dim TestataElem =
                       From CI In GiasContext.Mov_Destinazioni
                       Join Movimenti In GiasContext.Movimenti
                         On Movimenti.PIVA Equals CI.Piva And
                            Movimenti.Id_Agenda Equals CI.Id_Agenda And
                            Movimenti.Id_Mov Equals CI.Id_Mov
                       Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli
                           On Movimenti_Dettagli.PIVA Equals Movimenti.PIVA And
                              Movimenti_Dettagli.Id_Agenda Equals Movimenti.Id_Agenda And
                              Movimenti_Dettagli.Id_Mov Equals Movimenti.Id_Mov
                       Join Zoo_Animali In GiasContext.Zoo_Animali
                           On Zoo_Animali.PIVA Equals Movimenti_Dettagli.PIVA And
                              Zoo_Animali.Cod_Progetto Equals Movimenti_Dettagli.Cod_Progetto
                       Join Centri_Aziendali In GiasContext.Centri_Aziendali
                            On Centri_Aziendali.PIVA Equals Movimenti_Dettagli.PIVA And
                               Centri_Aziendali.sa_cod Equals Movimenti_Dettagli.Sa_Cod
                       Join Zoo_Animali_Distinte In GiasContext.Zoo_Animali_Distinte
                         On Zoo_Animali_Distinte.PIVA Equals Zoo_Animali.PIVA And
                            Zoo_Animali_Distinte.Cod_Animale Equals Zoo_Animali.Cod_Progetto
                       Join Specie In GiasContext.Lista_Specie_Animali
                         On Specie.GEN_COD Equals Zoo_Animali.GEN_COD And
                            Specie.SPE_COD Equals Zoo_Animali.SPE_COD
                       Join Razze In GiasContext.Lista_Razze_Animali
                         On Razze.GEN_COD Equals Zoo_Animali.GEN_COD And
                            Razze.SPE_COD Equals Zoo_Animali.SPE_COD And
                            Razze.RAZ_COD Equals Zoo_Animali.RAZ_COD
                       Where
                      (CI.Piva.Equals(piva)) _
                      AndAlso (CI.Id_Agenda = id_agenda) _
                       AndAlso (CI.Tipo_Destinazione = CostantiPersonalizzate.STALLA OrElse CI.Tipo_Destinazione = CostantiPersonalizzate.TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA) _
                       AndAlso (Zoo_Animali_Distinte.Validita_Inizio <= data_movimento) _
                       AndAlso (Zoo_Animali_Distinte.Validita_Fine >= data_movimento) _
                       AndAlso (Movimenti.Cau_Mov = CAU_PESATURA_ANIMALI)
                       Order By CI.QuotaDistribuzione Descending
                       Select New With {
                      .Piva = CI.Piva,
                      .Sa_Cod = Centri_Aziendali.sa_cod,
                      .sa_nome = Centri_Aziendali.sa_nome,
                      .Appezza = CI.Appezza,
                      .Id_Destinazione = 0,
                      .Key = "",
                      .Gen_Cod = Specie.GEN_COD,
                      .Spe_Cod = Specie.SPE_COD,
                      .Veg_Des = Specie.SPE_DES,
                      .Raz_Cod = Razze.RAZ_COD,
                      .RAZ_DES = Razze.RAZ_DES,
                      .Descrizione = Zoo_Animali.Nome,
                      .Progetto_Cod = 0,
                      .Progetto_Nome = Zoo_Animali_Distinte.Progetto_Nome,
                      .Progetto_Des = Zoo_Animali_Distinte.Progetto_Des,
                      .Superficie = CI.Qta2,
                      .Prezzo_Unitario = Movimenti_Dettagli.Prezzo_Unitario,
                      .Valore = 0,
                      .Cod_Animale = Zoo_Animali_Distinte.Cod_Animale,
                      .Cod_Progetto = Zoo_Animali_Distinte.Cod_Progetto,
                      .Codice_Distinta = Zoo_Animali_Distinte.Codice_Distinta,
                      .Distinta_Chiusa = Zoo_Animali_Distinte.Distinta_Chiusa,
                      .STA_NUM = 0,
                      .Raggruppamento_Cod = 0,
                      .Raggruppamento_Des = "",
                      .Selected = True,
                      .Stato_Des = "",
                      .Matricola = "",
                      .STA_DES = "",
                      .SPE_DES = "",
                      .IPRO_DES = "",
                      .Tipo_Des = "",
                      .Progetto = "",
                      .Dat_Nascita = "01/01/1900",
                      .Rag_Soc = ""
                      }

                    'And strCau_Progetto.Contains(Zoo_Animali_Distinte.Cau_Progetto) _
                    ' Compongo la chiave
                    myList = TestataElem.Distinct().ToList()

                    'Ricavo il costo totale
                    If myList IsNot Nothing Then
                        For Each obj In myList
                            Costo_Totale = Costo_Totale + obj.Prezzo_Unitario
                        Next
                    End If




                Case LAVCOD_VACCINAZIONI_ANIMALI, LAVCOD_CUREMEDICAMENTI_ANIMALI

                    Dim TestataElem =
                    From CI In GiasContext.Mov_Destinazioni
                    Join Agenda In GiasContext.Agenda
                     On Agenda.PIVA Equals CI.Piva And
                        Agenda.Id_Agenda Equals CI.Id_Agenda
                    Join Movimenti In GiasContext.Movimenti
                     On Movimenti.PIVA Equals CI.Piva And
                        Movimenti.Id_Agenda Equals CI.Id_Agenda And
                        Movimenti.Id_Mov Equals CI.Id_Mov
                    Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli
                       On Movimenti_Dettagli.PIVA Equals Movimenti.PIVA And
                          Movimenti_Dettagli.Id_Agenda Equals Movimenti.Id_Agenda And
                          Movimenti_Dettagli.Id_Mov Equals Movimenti.Id_Mov
                    Join Zoo_Animali In GiasContext.Zoo_Animali
                       On Zoo_Animali.PIVA Equals CI.Piva And
                          Zoo_Animali.Cod_Progetto Equals CI.Id_Destinazione
                    Join Centri_Aziendali In GiasContext.Centri_Aziendali
                        On Centri_Aziendali.PIVA Equals Agenda.PIVA And
                           Centri_Aziendali.sa_cod Equals Agenda.Sa_Cod
                    Join Zoo_Animali_Distinte In GiasContext.Zoo_Animali_Distinte
                     On Zoo_Animali_Distinte.PIVA Equals Zoo_Animali.PIVA And
                        Zoo_Animali_Distinte.Cod_Animale Equals Zoo_Animali.Cod_Progetto
                    Join Specie In GiasContext.Lista_Specie_Animali
                     On Specie.GEN_COD Equals Zoo_Animali.GEN_COD And
                        Specie.SPE_COD Equals Zoo_Animali.SPE_COD
                    Join Razze In GiasContext.Lista_Razze_Animali
                     On Razze.GEN_COD Equals Zoo_Animali.GEN_COD And
                        Razze.SPE_COD Equals Zoo_Animali.SPE_COD And
                        Razze.RAZ_COD Equals Zoo_Animali.RAZ_COD
                    Where
                  (CI.Piva.Equals(piva)) _
                  AndAlso (CI.Id_Agenda = id_agenda) _
                   AndAlso (CI.Tipo_Destinazione = 1) _
                   AndAlso (Movimenti.Cau_Mov = CAU_TRATTAMENTO_ZOO)
                    Order By CI.QuotaDistribuzione Descending
                    Select New With {
                     .Piva = CI.Piva,
                     .Sa_Cod = Centri_Aziendali.sa_cod,
                     .sa_nome = Centri_Aziendali.sa_nome,
                     .Appezza = CI.Appezza,
                     .Id_Destinazione = 0,
                     .Key = "",
                     .Gen_Cod = Specie.GEN_COD,
                     .Spe_Cod = Specie.SPE_COD,
                     .Veg_Des = Specie.SPE_DES,
                     .Raz_Cod = Razze.RAZ_COD,
                     .RAZ_DES = Razze.RAZ_DES,
                     .Descrizione = Zoo_Animali.Nome,
                     .Progetto_Cod = 0,
                     .Progetto_Nome = Zoo_Animali_Distinte.Progetto_Nome,
                     .Progetto_Des = Zoo_Animali_Distinte.Progetto_Des,
                     .Superficie = CI.Qta2,
                     .Prezzo_Unitario = Movimenti_Dettagli.Prezzo_Unitario,
                     .Valore = 0,
                     .Cod_Animale = Zoo_Animali_Distinte.Cod_Animale,
                     .Cod_Progetto = Zoo_Animali_Distinte.Cod_Progetto,
                     .Codice_Distinta = Zoo_Animali_Distinte.Codice_Distinta,
                     .Distinta_Chiusa = Zoo_Animali_Distinte.Distinta_Chiusa,
                     .STA_NUM = 0,
                     .Raggruppamento_Cod = 0,
                     .Raggruppamento_Des = "",
                     .Selected = True,
                     .Stato_Des = "",
                     .Matricola = "",
                     .STA_DES = "",
                     .SPE_DES = "",
                     .IPRO_DES = "",
                     .Tipo_Des = "",
                     .Progetto = "",
                     .Dat_Nascita = "01/01/1900",
                     .Rag_Soc = ""
                     }

                    'And strCau_Progetto.Contains(Zoo_Animali_Distinte.Cau_Progetto) _
                    ' Compongo la chiave
                    myList = TestataElem.Distinct().ToList()

                    'Ricavo il costo totale
                    If myList IsNot Nothing Then
                        For Each obj In myList
                            Costo_Totale = Costo_Totale + obj.Prezzo_Unitario
                        Next
                    End If


                Case Else 'Incremento/Decremento/Trasferimento

                    Dim TestataElem =
                       From CI In GiasContext.Mov_Destinazioni
                       Join Movimenti In GiasContext.Movimenti
                         On Movimenti.PIVA Equals CI.Piva And
                            Movimenti.Id_Agenda Equals CI.Id_Agenda And
                            Movimenti.Id_Mov Equals CI.Id_Mov
                       Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli
                           On Movimenti_Dettagli.PIVA Equals Movimenti.PIVA And
                              Movimenti_Dettagli.Id_Agenda Equals Movimenti.Id_Agenda And
                              Movimenti_Dettagli.Id_Mov Equals Movimenti.Id_Mov
                       Join Zoo_Animali In GiasContext.Zoo_Animali
                           On Zoo_Animali.PIVA Equals Movimenti_Dettagli.PIVA And
                              Zoo_Animali.Cod_Progetto Equals Movimenti_Dettagli.Cod_Progetto
                       Join Centri_Aziendali In GiasContext.Centri_Aziendali
                            On Centri_Aziendali.PIVA Equals Movimenti_Dettagli.PIVA And
                               Centri_Aziendali.sa_cod Equals Movimenti_Dettagli.Sa_Cod
                       Join Zoo_Animali_Distinte In GiasContext.Zoo_Animali_Distinte
                         On Zoo_Animali_Distinte.PIVA Equals Zoo_Animali.PIVA And
                            Zoo_Animali_Distinte.Cod_Animale Equals Zoo_Animali.Cod_Progetto
                       Join Specie In GiasContext.Lista_Specie_Animali
                         On Specie.GEN_COD Equals Zoo_Animali.GEN_COD And
                            Specie.SPE_COD Equals Zoo_Animali.SPE_COD
                       Join Razze In GiasContext.Lista_Razze_Animali
                         On Razze.GEN_COD Equals Zoo_Animali.GEN_COD And
                            Razze.SPE_COD Equals Zoo_Animali.SPE_COD And
                            Razze.RAZ_COD Equals Zoo_Animali.RAZ_COD
                       Where
                      (CI.Piva.Equals(piva)) _
                      AndAlso (CI.Id_Agenda = id_agenda) _
                       AndAlso (CI.Tipo_Destinazione = CostantiPersonalizzate.STALLA OrElse CI.Tipo_Destinazione = CostantiPersonalizzate.TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA) _
                       AndAlso (Zoo_Animali_Distinte.Validita_Inizio <= data_movimento) _
                       AndAlso (Zoo_Animali_Distinte.Validita_Fine >= data_movimento) _
                       AndAlso (Movimenti.Cau_Mov <> CAU_PESATURA_ANIMALI)
                       Order By CI.QuotaDistribuzione Descending
                       Select New With {
                      .Piva = CI.Piva,
                      .Sa_Cod = Centri_Aziendali.sa_cod,
                      .sa_nome = Centri_Aziendali.sa_nome,
                      .Appezza = CI.Appezza,
                      .Id_Destinazione = 0,
                      .Key = "",
                      .Gen_Cod = Specie.GEN_COD,
                      .Spe_Cod = Specie.SPE_COD,
                      .Veg_Des = Specie.SPE_DES,
                      .Raz_Cod = Razze.RAZ_COD,
                      .RAZ_DES = Razze.RAZ_DES,
                      .Descrizione = Zoo_Animali.Nome,
                      .Progetto_Cod = 0,
                      .Progetto_Nome = Zoo_Animali_Distinte.Progetto_Nome,
                      .Progetto_Des = Zoo_Animali_Distinte.Progetto_Des,
                      .Superficie = CI.Qta2,
                      .Prezzo_Unitario = Movimenti_Dettagli.Prezzo_Unitario,
                      .Valore = 0,
                      .Cod_Animale = Zoo_Animali_Distinte.Cod_Animale,
                      .Cod_Progetto = Zoo_Animali_Distinte.Cod_Progetto,
                      .Codice_Distinta = Zoo_Animali_Distinte.Codice_Distinta,
                      .Distinta_Chiusa = Zoo_Animali_Distinte.Distinta_Chiusa,
                      .STA_NUM = 0,
                      .Raggruppamento_Cod = 0,
                      .Raggruppamento_Des = "",
                      .Selected = True,
                      .Stato_Des = "",
                      .Matricola = "",
                      .STA_DES = "",
                      .SPE_DES = "",
                      .IPRO_DES = "",
                      .Tipo_Des = "",
                      .Progetto = "",
                      .Dat_Nascita = "01/01/1900",
                      .Rag_Soc = ""
                      }

                    'And strCau_Progetto.Contains(Zoo_Animali_Distinte.Cau_Progetto) _
                    ' Compongo la chiave
                    myList = TestataElem.Distinct().ToList()

                    'Ricavo il costo totale
                    If myList IsNot Nothing Then
                        For Each obj In myList
                            Costo_Totale = Costo_Totale + obj.Prezzo_Unitario
                        Next
                    End If


            End Select

            If myList IsNot Nothing Then

                For Each obj In myList

                    Dim dtZoo = obj_Zoo.Leggi_Giacenze(obj.Piva, 0, 0, 0, obj.Cod_Animale, data_movimento & " 23:59:59", objParametri, True)
                    If dtZoo.Rows.Count > 0 Then

                        obj.STA_NUM = dtZoo.Rows(0).Item("Sta_Num")
                        obj.Id_Destinazione = dtZoo.Rows(0).Item("Raggruppamento_Cod")
                        obj.Raggruppamento_Cod = dtZoo.Rows(0).Item("Raggruppamento_Cod")
                        obj.Raggruppamento_Des = dtZoo.Rows(0).Item("Raggruppamento_Des")
                        obj.Matricola = dtZoo.Rows(0).Item("Matricola")
                        obj.Stato_Des = dtZoo.Rows(0).Item("Stato_Des")
                        obj.STA_DES = dtZoo.Rows(0).Item("STA_DES")
                        obj.SPE_DES = dtZoo.Rows(0).Item("SPE_DES")
                        obj.IPRO_DES = dtZoo.Rows(0).Item("IPRO_DES")
                        obj.Tipo_Des = dtZoo.Rows(0).Item("Tipo_Des")
                        obj.Progetto = dtZoo.Rows(0).Item("Progetto")
                        If CDate(dtZoo.Rows(0).Item("Dat_Nascita")) <> AGRODATAINIZIO Then
                            obj.Dat_Nascita = dtZoo.Rows(0).Item("Dat_Nascita")
                        End If


                        Select Case Lav_Cod

                            Case LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI

                            Case Else
                                'Ricalcolo valore visto che non viene valorizzata la quotadistribuzione
                                If Costo_Totale <> 0 Then
                                    obj.valore = obj.Prezzo_Unitario / Costo_Totale
                                Else
                                    obj.valore = 0
                                End If


                        End Select

                        'obj.Rag_Soc = dtZoo.Rows(0).Item("Rag_Soc")

                    End If

                Next

                For Each obj In myList
                    obj.Key = obj.Piva & "-" & obj.Sa_Cod.ToString() & "-" & obj.Appezza.ToString() & "-" & obj.Id_Destinazione.ToString()

                    Counter = Counter + 1

                    'Imposto l'ultimo valore in modo da non ottenere sfiridi sul 100% che poi bloccano il salvataggio automatico
                    If CInt(myList.Count) = Counter Then
                        obj.Valore = Decimal.Round(Convert.ToDecimal(Residuo), 2)
                    Else
                        obj.Valore = Decimal.Round(Convert.ToDecimal(obj.Valore), 2)
                        Residuo = Residuo - obj.Valore
                    End If


                Next
            Else
                myList = New List(Of Object)
            End If


            Select Case bDT

                Case True

                    Dim ut As New Gias_EF_Utility
                    DT = ut.ObjectQueryToDataTable(myList)
                    Return DT

                Case False

                    Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                    risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)
                    Return risposta

            End Select


        End Using

    End Function








    '##############################################################################################
    Public Function Ricerca_Sintesi_Persone_TimeSheet(ByVal piva As String,
                                           ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Ricerca_Sintesi_Persone_TimeSheet()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim strJoin As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim dt_finale As New DataTable
        Dim dr_finale As DataRow = Nothing
        Dim PivaSuperUser = objParametri.PivaSuperUser
        Dim Data_Ora_Tot As String
        Dim Data_Ora_Tot_Str As String
        Dim Giorni As Integer
        Dim Data As Date

        Dim Ora As Integer
        Dim Minuti As Integer
        Dim parteIntera As Decimal
        Dim parteDecimali As Decimal

        strSql.Length = 0
        strSql.AppendLine(" Select ")

        strSql.AppendLine("Risorse_Umane.Cod_Risum, ")
        strSql.AppendLine("(ISNULL(Contatti.Cognome, '') + ' ' + ISNULL(Contatti.Nome, '')) AS Rag_Soc, ")
        strSql.AppendLine("Risorse_Umane.Qualifica_Cod, ")
        strSql.AppendLine("Rapporti_Contabili.Cod_Rapporto, Rapporti_Contabili.Rapporto_Des, ISNULL(Qualifica_Des, '') AS Qualifica_Des, ")
        strSql.AppendLine("ISNULL(CDG_Testata.Data_Inserimento, " & Agro_SQL_SaveDate(Validita_Inizio) & ") AS Data_Inserimento, ")
        strSql.AppendLine("SUM(ISNULL(CDG_Testata.Qta, 0)) AS Qta")
        strSql.AppendLine(" From Risorse_Umane  ")

        strSql.AppendLine(" Inner Join Rapporti_Contabili On (Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto)  ")
        strSql.AppendLine(" AND Rapporti_Contabili.Cod_Rapporto IN ( ")
        strSql.AppendLine(CInt(enum_Rapporti_Contabili_Standard.Dipendente))
        strSql.AppendLine(", ")
        strSql.AppendLine(CInt(enum_Rapporti_Contabili_Standard.Avventizio))
        strSql.AppendLine(") ")

        strSql.AppendLine(" Left Outer Join Qualifiche On (Risorse_Umane.Qualifica_Cod = Qualifiche.Qualifica_Cod)  ")

        strSql.AppendLine(" Inner Join Contatti On (Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto)  ")
        strSql.AppendLine(" And (Contatti.Sa_Cod = -1 OR Contatti.PIVA = '" & Agro_SQL_SaveText(piva) & "' ) ")

        strSql.AppendLine(" Left Outer Join CDG_Testata On (CDG_Testata.Cod_RisUm = Risorse_Umane.Cod_RisUm)  ")
        strSql.AppendLine(" And (CDG_Testata.Data_Inserimento IS null OR CDG_Testata.Data_Inserimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ) ")
        strSql.AppendLine(" And (CDG_Testata.Data_Inserimento IS null OR CDG_Testata.Data_Inserimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ) ")
        strSql.AppendLine(" And (CDG_Testata.Piva_SuperUser IS null OR CDG_Testata.Piva_SuperUser = '" & PivaSuperUser & "' )")
        strSql.AppendLine(" AND (CDG_Testata.Piva IS null OR CDG_Testata.Piva = '" & Agro_SQL_SaveText(piva) & "' ) ")
        strSql.AppendLine(" AND (CDG_Testata.Cod_Risum IS null OR CDG_Testata.Cod_Risum != 0 ) ")

        '---  INIZIO Scarto movimenti di Split
        strSql.AppendLine(" Left Outer Join Agenda On (CDG_Testata.Id_agenda = Agenda.Id_agenda)  ")
        strSql.AppendLine(" AND (Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "' ) ")

        strSql.AppendLine(" Where ISNULL(Agenda.Split, '0') = '0' ")
        strSql.AppendLine(" AND ISNULL(CDG_Testata.Budget, 0) = 0 ")
        '---  FINE Scarto movimenti di Split
        strSql.AppendLine("And Risorse_Umane.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
        strSql.AppendLine("And Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

        strSql.AppendLine(" group by ")
        strSql.AppendLine(" Risorse_Umane.Cod_Risum, ")
        strSql.AppendLine(" Contatti.Cognome, Contatti.Nome, ")
        strSql.AppendLine(" Risorse_Umane.Qualifica_Cod, ")
        strSql.AppendLine(" Rapporti_Contabili.Cod_Rapporto, Rapporti_Contabili.Rapporto_Des, Qualifica_Des, ")
        strSql.AppendLine(" CDG_Testata.Data_Inserimento ")

        strSql.AppendLine(" ORDER BY Cognome, Nome, CDG_Testata.Data_Inserimento ASC ")

        dt_finale.Columns.Add(New DataColumn("Cod_Risum", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Qualifica_Cod", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Cod_Rapporto", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Rapporto_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Qualifica_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Data_Inserimento", GetType(Date)))
        dt_finale.Columns.Add(New DataColumn("QtaTotale", GetType(Decimal)))
        dt_finale.Columns.Add(New DataColumn("Totale_Riga", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Totale_Riga_Str", GetType(String)))

        Giorni = DateDiff("d", Validita_Inizio, Validita_Fine)

        For i = 0 To Giorni

            Data = DateAdd(DateInterval.Day, i, Date.ParseExact(Validita_Inizio, "dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo))

            'Calcolo Campi Data_Ora
            Data_Ora_Tot = "Data_Ora_Tot" & Format(Data, "yyyyMMdd")
            Data_Ora_Tot_Str = "Data_Ora_Tot_Str" & Format(Data, "yyyyMMdd")
            dt_finale.Columns.Add(New DataColumn(Data_Ora_Tot, GetType(String)))
            dt_finale.Columns.Add(New DataColumn(Data_Ora_Tot_Str, GetType(String)))

        Next


        '--------------------------------------------------------------------------
        dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
        '--------------------------------------------------------------------------

        For Each dr As DataRow In dt.Rows

            Data_Ora_Tot = "Data_Ora_Tot" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Data_Ora_Tot_Str = "Data_Ora_Tot_Str" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Dim giaPresente As Boolean = False
            For Each rFinale As DataRow In dt_finale.Rows
                If rFinale.Item("Cod_Risum") = dr.Item("Cod_Risum") Then
                    dr_finale = rFinale
                    giaPresente = True
                    Exit For
                End If
            Next

            If Not giaPresente Then
                'Creo una nuova riga
                dr_finale = dt_finale.NewRow

                dr_finale.Item("Cod_Risum") = dr.Item("Cod_Risum")
                dr_finale.Item("Qualifica_Cod") = dr.Item("Qualifica_Cod")
                dr_finale.Item("Qualifica_Des") = dr.Item("Qualifica_Des")
                dr_finale.Item("Cod_Rapporto") = dr.Item("Cod_Rapporto")
                dr_finale.Item("Rapporto_Des") = dr.Item("Rapporto_Des")
                dr_finale.Item("Rag_Soc") = Trim(dr.Item("Rag_Soc"))
                dr_finale.Item("QtaTotale") = 0
            End If

            dr_finale.Item("QtaTotale") += dr.Item("Qta")

            'Conversione intero minuti singola giornata
            parteIntera = Math.Truncate(dr.Item("Qta"))
            parteDecimali = dr.Item("Qta") - parteIntera
            Ora = parteIntera
            Minuti = 60 * parteDecimali
            dr_finale.Item(Data_Ora_Tot) = "01/01/1900 " & Ora.ToString("D2") & ":" & Minuti.ToString("D2")
            dr_finale.Item(Data_Ora_Tot_Str) = Ora.ToString("D2") & ":" & Minuti.ToString("D2")

            'Conversione intero minuti totale ore
            parteIntera = Math.Truncate(dr_finale.Item("QtaTotale"))
            parteDecimali = dr_finale.Item("QtaTotale") - parteIntera
            Ora = parteIntera
            Minuti = 60 * parteDecimali
            dr_finale.Item("Totale_Riga") = "01/01/1900 " & Ora.ToString("D2") & ":" & Minuti.ToString("D2")
            dr_finale.Item("Totale_Riga_Str") = Ora.ToString("D2") & ":" & Minuti.ToString("D2")

            If Not giaPresente Then
                dt_finale.Rows.Add(dr_finale)
            End If
        Next

        Return dt_finale

    End Function


    '##############################################################################################
    Public Function Ricerca_Sintesi_Macchine_TimeSheet(ByVal piva As String,
                                           ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Ricerca_Sintesi_Macchine_TimeSheet()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim strJoin As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim dt_finale As New DataTable
        Dim dr_finale As DataRow = Nothing
        Dim PivaSuperUser = objParametri.PivaSuperUser
        Dim Data_Ora_Tot As String
        Dim Data_Ora_Tot_Str As String
        Dim Giorni As Integer
        Dim Data As Date

        Dim Ora As Integer
        Dim Minuti As Integer
        Dim parteIntera As Decimal
        Dim parteDecimali As Decimal

        strSql.Length = 0
        strSql.AppendLine(" Select ")


        strSql.AppendLine(" Parco_Macchine.Mac_Cod, Parco_Macchine.Mac_Des,  Parco_Macchine.Class_Code, Macchine.CLASS_DESC,")
        strSql.AppendLine(" ISNULL(CDG_Testata.Data_Inserimento, " & Agro_SQL_SaveDate(Validita_Inizio) & ") AS Data_Inserimento, ")
        strSql.AppendLine(" SUM(ISNULL(CDG_Testata.Qta, 0)) AS Qta")
        strSql.AppendLine(" From Parco_Macchine  ")
        strSql.AppendLine(" INNER JOIN  UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA ")
        strSql.AppendLine(" AND UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

        strSql.AppendLine(" INNER JOIN  Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE  ")

        strSql.AppendLine(" Left Outer Join CDG_Testata On (CDG_Testata.Mac_Cod = Parco_Macchine.Mac_Cod)  ")
        strSql.AppendLine(" And (CDG_Testata.Data_Inserimento IS null OR CDG_Testata.Data_Inserimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ) ")
        strSql.AppendLine(" And (CDG_Testata.Data_Inserimento IS null OR CDG_Testata.Data_Inserimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ) ")
        strSql.AppendLine(" And (CDG_Testata.Piva_SuperUser IS null OR CDG_Testata.Piva_SuperUser = '" & PivaSuperUser & "' )")
        strSql.AppendLine(" AND (CDG_Testata.Piva IS null OR CDG_Testata.Piva = '" & Agro_SQL_SaveText(piva) & "' ) ")
        strSql.AppendLine(" AND (CDG_Testata.Mac_Cod IS null OR CDG_Testata.Mac_Cod != 0 ) ")

        '---  INIZIO Scarto movimenti di Split
        strSql.AppendLine(" Left Outer Join Agenda On (CDG_Testata.Id_agenda = Agenda.Id_agenda)  ")
        strSql.AppendLine(" AND (Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "' ) ")

        strSql.AppendLine(" Where ISNULL(Agenda.Split, '0') = '0' ")
        '---  FINE Scarto movimenti di Split
        strSql.AppendLine(" AND ISNULL(CDG_Testata.Budget, 0) = 0 ")
        strSql.AppendLine(" And Visibile_ctrl_gestione = 1 ")
        strSql.AppendLine("And Parco_Macchine.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
        strSql.AppendLine("And Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

        strSql.AppendLine(" group by ")
        strSql.AppendLine(" Parco_Macchine.Mac_Cod, Parco_Macchine.Mac_Des, Parco_Macchine.Class_Code, Macchine.CLASS_DESC, ")
        strSql.AppendLine(" CDG_Testata.Data_Inserimento ")

        strSql.AppendLine(" ORDER BY Mac_Des, CDG_Testata.Data_Inserimento ASC ")

        dt_finale.Columns.Add(New DataColumn("Mac_Cod", GetType(Integer)))
        dt_finale.Columns.Add(New DataColumn("Mac_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Class_Code", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("CLASS_DESC", GetType(String)))

        dt_finale.Columns.Add(New DataColumn("Data_Inserimento", GetType(Date)))
        dt_finale.Columns.Add(New DataColumn("QtaTotale", GetType(Decimal)))
        dt_finale.Columns.Add(New DataColumn("Totale_Riga", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Totale_Riga_Str", GetType(String)))

        Giorni = DateDiff("d", Validita_Inizio, Validita_Fine)

        For i = 0 To Giorni

            Data = DateAdd(DateInterval.Day, i, Date.ParseExact(Validita_Inizio, "dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo))

            'Calcolo Campi Data_Ora
            Data_Ora_Tot = "Data_Ora_Tot" & Format(Data, "yyyyMMdd")
            Data_Ora_Tot_Str = "Data_Ora_Tot_Str" & Format(Data, "yyyyMMdd")
            dt_finale.Columns.Add(New DataColumn(Data_Ora_Tot, GetType(String)))
            dt_finale.Columns.Add(New DataColumn(Data_Ora_Tot_Str, GetType(String)))

        Next


        '--------------------------------------------------------------------------
        dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

        For Each dr As DataRow In dt.Rows

            Data_Ora_Tot = "Data_Ora_Tot" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Data_Ora_Tot_Str = "Data_Ora_Tot_Str" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Dim giaPresente As Boolean = False
            For Each rFinale As DataRow In dt_finale.Rows
                If rFinale.Item("Mac_Cod") = dr.Item("Mac_Cod") Then
                    dr_finale = rFinale
                    giaPresente = True
                    Exit For
                End If
            Next

            If Not giaPresente Then
                'Creo una nuova riga
                dr_finale = dt_finale.NewRow
                dr_finale.Item("Mac_Cod") = dr.Item("Mac_Cod")
                dr_finale.Item("Mac_Des") = dr.Item("Mac_Des")
                dr_finale.Item("Class_Code") = dr.Item("Class_Code")
                dr_finale.Item("CLASS_DESC") = dr.Item("CLASS_DESC")
                dr_finale.Item("QtaTotale") = 0
            End If

            dr_finale.Item("QtaTotale") += dr.Item("Qta")

            'Conversione intero minuti singola giornata
            parteIntera = Math.Truncate(dr.Item("Qta"))
            parteDecimali = dr.Item("Qta") - parteIntera
            Ora = parteIntera
            Minuti = 60 * parteDecimali
            dr_finale.Item(Data_Ora_Tot) = "01/01/1900 " & Ora.ToString("D2") & ":" & Minuti.ToString("D2")
            dr_finale.Item(Data_Ora_Tot_Str) = Ora.ToString("D2") & ":" & Minuti.ToString("D2")

            'Conversione intero minuti totale ore
            parteIntera = Math.Truncate(dr_finale.Item("QtaTotale"))
            parteDecimali = dr_finale.Item("QtaTotale") - parteIntera
            Ora = parteIntera
            Minuti = 60 * parteDecimali
            dr_finale.Item("Totale_Riga") = "01/01/1900 " & Ora.ToString("D2") & ":" & Minuti.ToString("D2")
            dr_finale.Item("Totale_Riga_Str") = Ora.ToString("D2") & ":" & Minuti.ToString("D2")

            If Not giaPresente Then
                dt_finale.Rows.Add(dr_finale)
            End If
        Next

        Return dt_finale

    End Function


    Public Function Leggi_OperazioniCampagna_AggiuntiModificati(
                                                              ByVal Piva As String,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByVal xOrderBy As String,
                                                              ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                              ) As DataTable

        Dim nomeRoutine As String = "CDG_DAL.CDG_DAL_R.Leggi_OperazioniCampagna_AggiuntiModificati()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT DISTINCT Mov_Destinazioni.PIVA, Mov_Destinazioni.sa_cod, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione AS id_reg, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.Des_Lib, Movimenti.Data_Movimento, MO2.Cau_Mov, Movimenti.Id_Mov, MD2.Elem_Cod, MD2.Pro_Cod, MD2.Mat_Cod, MD2.Lotto, MD2.Udm_Cod, MD2.Id_Attivita, MD2.Turno_Cod, MD2.Qualifica_Cod, MD2.Tariffa_Cod, MD2.Prezzo_Unitario, Sum(MD2.Qta * ISNULL(Mov_Destinazioni.QuotaDistribuzione,0)) AS Totale")
            strSql.AppendLine("FROM Mov_Destinazioni ")
            strSql.AppendLine("INNER JOIN Agenda ON Agenda.Piva = Mov_Destinazioni.Piva And Agenda.Sa_Cod = Mov_Destinazioni.Sa_Cod And Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda")
            strSql.AppendLine("INNER JOIN Movimenti ON Agenda.Piva = Movimenti.Piva And Agenda.Sa_Cod = Movimenti.Sa_Cod And Agenda.Id_Agenda = Movimenti.Id_Agenda And Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov And Movimenti.Cau_Mov In ('" & enum_Agenda_Causali.TRATTAMENTO & "', '" & enum_Agenda_Causali.RILIEVO_CAMPO & "', '" & enum_Agenda_Causali.RILIEVO_RACCOLTA & "', '" & enum_Agenda_Causali.LAVORAZIONE & "')")
            strSql.AppendLine("INNER JOIN Movimenti_Dettagli ON Movimenti_Dettagli.Piva = Movimenti.Piva And Movimenti_Dettagli.Sa_Cod = Movimenti.Sa_Cod And Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda And Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov And Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det")
            strSql.AppendLine("INNER JOIN Movimenti AS MO2 On Agenda.Piva = MO2.Piva And Agenda.Id_Agenda = MO2.Id_Agenda And MO2.Cau_Mov In ('" & enum_Agenda_Causali.SCARICO & "','" & enum_Agenda_Causali.IMPUTAZIONE_MANODOPERA & "','" & enum_Agenda_Causali.IMPUTAZIONE_TERZISTI & "','" & enum_Agenda_Causali.IMPUTAZIONE_PARCOMACCHINE & "')")
            strSql.AppendLine("INNER JOIN Movimenti_Dettagli AS MD2 On MO2.Piva = MD2.Piva And MO2.Id_Agenda = MD2.Id_Agenda And MO2.Id_Mov = MD2.Id_Mov")

            strSql.AppendLine("WHERE (Agenda.Lav_Cod > 0 AND Agenda.Lav_Cod < 1000)")

            'Prendo i soli record che non sono presenti in CDG_Testata o sono da aggiornare
            strSql.AppendLine("AND NOT EXISTS ( SELECT TOP 1 1 FROM CDG_Testata")
            strSql.AppendLine("WHERE Agenda.piva=CDG_Testata.Piva AND CDG_Testata.Budget = 0 AND Agenda.id_agenda=CDG_Testata.Id_Agenda AND CONVERT(DATE,CDG_Testata.Data_Modifica) > CONVERT(DATE,Agenda.Data_Modifica))")

            If Piva <> "" Then
                strSql.AppendLine(" AND Agenda.Piva = " & Agro_SQL_SaveText_NULL(Piva))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.AppendLine("GROUP BY Mov_Destinazioni.PIVA, Mov_Destinazioni.sa_cod, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.Des_Lib, Movimenti.Data_Movimento, MO2.Cau_Mov, Movimenti.Id_Mov,  MD2.Elem_Cod, MD2.Pro_Cod, MD2.Mat_Cod, MD2.Lotto, MD2.Udm_Cod, MD2.Id_Attivita, MD2.Turno_Cod, MD2.Qualifica_Cod, MD2.Tariffa_Cod, MD2.Prezzo_Unitario")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                strSql.AppendLine(" ORDER BY Movimenti.Data_Movimento, Agenda.id_agenda")
            End If
            '---------------------------------------------


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_OperazioniCampagna_AggiuntiModificati_ContaRicorrenze(
                                                              ByVal Piva As String,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByVal xOrderBy As String,
                                                              ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                              ) As DataTable

        Dim nomeRoutine As String = "CDG_DAL.CDG_DAL_R.Leggi_OperazioniCampagna_AggiuntiModificati()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT COUNT(Movimenti_Dettagli.Id_Mov_Det) AS Numero, Movimenti_Dettagli.Piva, Movimenti_Dettagli.Id_Agenda, Movimenti_Dettagli.Id_Mov")
            strSql.AppendLine("FROM Agenda ")
            strSql.AppendLine("INNER JOIN Movimenti ON Agenda.Piva = Movimenti.Piva AND Agenda.Id_Agenda = Movimenti.ID_Agenda")
            strSql.AppendLine("INNER JOIN Movimenti_Dettagli ON Movimenti.Piva = Movimenti_Dettagli.Piva AND Movimenti.Id_Mov = Movimenti_Dettagli.ID_Mov")
            strSql.AppendLine("WHERE Movimenti.Cau_Mov IN ('" & enum_Agenda_Causali.TRATTAMENTO & "', '" & enum_Agenda_Causali.RILIEVO_CAMPO & "', '" & enum_Agenda_Causali.RILIEVO_RACCOLTA & "', '" & enum_Agenda_Causali.LAVORAZIONE & "')")
            strSql.AppendLine("AND (Agenda.Lav_Cod > 0 AND Agenda.Lav_Cod < 1000)")

            If Piva <> "" Then
                strSql.AppendLine(" AND Agenda.Piva = " & Agro_SQL_SaveText_NULL(Piva))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.AppendLine("GROUP BY Movimenti_Dettagli.Piva, Movimenti_Dettagli.Id_Agenda, Movimenti_Dettagli.Id_Mov")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                strSql.AppendLine(" ORDER BY Movimenti_Dettagli.id_agenda, Movimenti_Dettagli.id_mov")
            End If
            '---------------------------------------------


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_OperazioniCampagna_Cancellati(
                                                        ByVal Piva As String,
                                                        ByVal budget As Integer,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim nomeRoutine As String = "CDG_DAL.CDG_DAL_R.Leggi_OperazioniCampagna_Cancellati()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT DISTINCT CDG_Testata.Piva, CDG_Testata.Id_Agenda ")
            strSql.AppendLine("FROM CDG_Testata ")
            strSql.AppendLine("LEFT JOIN Agenda ")
            strSql.AppendLine("ON Agenda.piva = CDG_Testata.Piva AND Agenda.id_agenda = CDG_Testata.Id_Agenda")
            strSql.AppendLine("WHERE CDG_Testata.Id_Agenda <> 0 AND Agenda.Id_Agenda IS NULL")
            strSql.AppendLine(" AND CDG_Testata.Budget = " & budget)
            If Piva <> "" Then
                strSql.AppendLine(" AND CDG_Testata.Piva = " & Agro_SQL_SaveText_NULL(Piva))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   CDG_Testata.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   CDG_Testata.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            End If
            '---------------------------------------------


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function CercaProgettiChiusi(ByVal Piva As String, ByVal idAgenda_Cdg As Integer,
                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As String

        Dim nomeRoutine As String = "CDG_DAL.CDG_DAL_R.CercaProgettiChiusi()"
        Dim PivaSuperUser = objParametri_Server.PivaSuperUser

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        'Ricerca esercizi chiusi campagna
        strSql.Length = 0

        strSql.Append(" Select distinct app_nome, ISNULL(Reg_Impianti_Codici.val_cod, '') AS Cod_Impianto, progetto_nome, progetto_des From CDG_Dettagli ")

        strSql.Append("   Join Cdg_testata On (  " & vbCrLf)
        strSql.Append("       CDG_Dettagli.Piva = Cdg_testata.Piva " & vbCrLf)
        strSql.Append(" And   CDG_Dettagli.Id_cdg = Cdg_testata.Id_cdg) " & vbCrLf)

        strSql.Append(" Join Appezzamento On (  " & vbCrLf)
        strSql.Append("       CDG_Dettagli.Piva = Appezzamento.Piva " & vbCrLf)
        strSql.Append(" And   CDG_Dettagli.Sa_Cod = Appezzamento.Sa_Cod " & vbCrLf)
        strSql.Append(" And   CDG_Dettagli.Appezza = Appezzamento.Appezza) " & vbCrLf)

        strSql.Append(" Join Reg_Impianti On (  " & vbCrLf)
        strSql.Append("       CDG_Dettagli.Piva = Reg_Impianti.Piva " & vbCrLf)
        strSql.Append(" And   CDG_Dettagli.Sa_Cod = Reg_Impianti.Sa_Cod " & vbCrLf)
        strSql.Append(" And   CDG_Dettagli.Appezza = Reg_Impianti.Appezza " & vbCrLf)
        strSql.Append(" And   CDG_Dettagli.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)

        strSql.Append(" Join Imprese_Progetti On (  " & vbCrLf)
        strSql.Append("       CDG_Dettagli.Piva = Imprese_Progetti.Piva " & vbCrLf)
        strSql.Append(" And   CDG_Dettagli.Sa_Cod = Imprese_Progetti.Sa_Cod " & vbCrLf)
        strSql.Append(" And   CDG_Dettagli.Appezza = Imprese_Progetti.Appezza " & vbCrLf)
        strSql.Append(" And   CDG_Dettagli.Id_Reg = Imprese_Progetti.Id_Reg " & vbCrLf)
        strSql.Append(" And   CDG_Dettagli.Progetto_Cod = Imprese_Progetti.Progetto_Cod ) " & vbCrLf)

        strSql.Append(" Left Outer Join Reg_Impianti_Codici On ( " & vbCrLf)
        strSql.Append(" Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA " & vbCrLf)
        strSql.Append(" And Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod " & vbCrLf)
        strSql.Append(" And Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza " & vbCrLf)
        strSql.Append(" And Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg " & vbCrLf)
        strSql.Append(" And Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Codice_Impianto & ") " & vbCrLf)

        'Chiusura esercizio
        strSql.Append(" Join Reg_Impianti_Codici AS Reg_Impianti_Codici2 On ( " & vbCrLf)
        strSql.Append(" Reg_Impianti.PIVA = Reg_Impianti_Codici2.PIVA " & vbCrLf)
        strSql.Append(" And Reg_Impianti.SA_COD = Reg_Impianti_Codici2.sa_cod " & vbCrLf)
        strSql.Append(" And Reg_Impianti.APPEZZA = Reg_Impianti_Codici2.appezza " & vbCrLf)
        strSql.Append(" And Reg_Impianti.ID_REG = Reg_Impianti_Codici2.Id_Reg " & vbCrLf)
        strSql.Append(" And Reg_Impianti_Codici2.id_cod = " & enum_CodiciAnagrafe.Distinta_Chiusa & vbCrLf)
        strSql.Append(" And CDG_Dettagli.Progetto_Cod = Reg_Impianti_Codici2.Progetto_Cod " & vbCrLf)
        strSql.Append(" And Reg_Impianti_Codici2.val_cod = 1  ) " & vbCrLf)

        strSql.Append("  Join Cultivar On (  ")
        strSql.Append("       Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod) " & vbCrLf)

        strSql.Append("  Join SpecieVegetali On (  ")
        strSql.Append("       Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod) " & vbCrLf)

        strSql.Append(" WHERE CDG_Testata.Budget = 0 AND CDG_Testata.id_agenda  = " & idAgenda_Cdg & " " & vbCrLf)
        strSql.Append(" And CDG_Testata.Piva_SuperUser = '" & PivaSuperUser & "' " & vbCrLf)
        strSql.Append(" AND CDG_Testata.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

        strSql.Append("  ORDER BY app_nome,  Cod_Impianto, progetto_nome, progetto_des " & vbCrLf)


        '--------------------------------------------------------------------------
        dt = EseguiQuery_Lettura(objParametri_Server, strSql.ToString, nomeRoutine)
        '--------------------------------------------------------------------------

        If dt.Rows.Count > 0 Then
            messaggioErrore = "Non è possibile cancellare questa operazione in quanto esistono questi esercizi chiusi: <br/>"
            For Each dr In dt.Rows
                messaggioErrore &= "Appezzamento: " & dr.Item("app_nome") & " - Cod_Impianto: " & dr.Item("Cod_Impianto") & " - Esercizio: " & dr.Item("progetto_nome") & " / " & dr.Item("progetto_des") & "<br/> "
            Next

        Else

            'Ricerca esercizi chiusi zoo
            strSql.Length = 0

            strSql.Append(" Select distinct Matricola, Progetto,  Codice_Distinta ,  progetto_des From CDG_Dettagli ")

            strSql.Append("   Join Cdg_testata On (  ")
            strSql.Append("       CDG_Dettagli.Piva = Cdg_testata.Piva ")
            strSql.Append(" And   CDG_Dettagli.Id_cdg = Cdg_testata.Id_cdg) ")

            strSql.Append(" Join Zoo_Animali On (  ")
            strSql.Append("       Zoo_Animali.Cod_Progetto = CDG_Dettagli.Cod_Animale) ")

            strSql.Append(" Join Zoo_Animali_Distinte On (  ")
            strSql.Append("       Zoo_Animali_Distinte.Cod_Animale = CDG_Dettagli.Cod_Animale  ")
            strSql.Append("   AND    Zoo_Animali_Distinte.Cod_Progetto = CDG_Dettagli.Cod_Animale_Distinta  ")
            strSql.Append("   AND    Zoo_Animali_Distinte.Distinta_Chiusa = 1) ")

            strSql.Append(" WHERE CDG_Testata.Budget = 0 AND CDG_Testata.id_agenda  = " & idAgenda_Cdg & " ")
            strSql.Append(" And CDG_Testata.Piva_SuperUser = '" & PivaSuperUser & "' ")
            strSql.Append(" AND CDG_Testata.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            strSql.Append("  ORDER BY Matricola, Progetto,  Codice_Distinta,  progetto_des ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count > 0 Then
                messaggioErrore = "Non è possibile cancellare questa operazione in quanto esistono questi esercizi chiusi: <br/>"
                For Each dr In dt.Rows
                    messaggioErrore &= "Matricola: " & dr.Item("Matricola") & "Progetto: " & dr.Item("Progetto") & "Esercizio: " & dr.Item("Codice_Distinta") & " - " & dr.Item("progetto_des") & "<br/> "
                Next

            End If
        End If

        Return messaggioErrore

    End Function


    '##############################################################################################
    Public Function Leggi_Squadre_Attivita(ByVal piva As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Squadre_Attivita()"
        Dim separators As String() = {"|"}

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Attivita = (From x In GiasContext.Attivita Select New With {x.ID_Attivita, x.Desc}).ToList()
            Dim Specie = (From x In GiasContext.SpecieVegetali Select New With {x.Veg_Cod, x.Veg_Des}).ToList()
            Dim Persone = (From ru In GiasContext.Risorse_Umane
                           Join c In GiasContext.Contatti On c.Cod_Contatto Equals ru.Cod_Contatto
                           Join rc In GiasContext.Rapporti_Contabili On rc.Cod_Rapporto Equals ru.Cod_Rapporto
                           Where (ru.Piva.Equals(piva) OrElse (c.Sa_Cod = -1)) AndAlso (ru.Piva = c.Piva OrElse (c.Sa_Cod = -1)) AndAlso
                               (rc.Cod_Rapporto = COD_CAPO_AREA OrElse rc.Cod_Rapporto = COD_TECNICO OrElse rc.Dipendente = 1)
                           Select New With {ru.Cod_RisUm, c.Rag_Soc, c.Cognome, c.Nome}).ToList()
            Dim Macchine = (From x In GiasContext.Parco_Macchine Where (x.Piva.Equals(piva) OrElse (x.Sa_Cod = -1)) AndAlso x.Tipo = 0
                            Select New With {x.Mac_Cod, x.Mac_Des}).ToList()

            Dim SquadreAttivita =
               (From s In GiasContext.SquadreXAttivita
                Join i In GiasContext.Imprese On s.Piva Equals i.PIVA
                Group Join o In GiasContext.Operazioni On s.Lav_Cod Equals o.LAV_COD Into og = Group From op In og.DefaultIfEmpty()
                Where String.IsNullOrEmpty(piva) OrElse s.Piva = piva AndAlso
                    s.Lav_Cod > 0
                Order By i.rag_soc, s.des_Squadra
                Select New With {
                   s.Piva_SuperUser,
                   s.Piva,
                   s.ID_Squadra,
                   s.Lav_Cod,
                   s.attivita_list,
                   s.veg_cod_list,
                   s.cod_risum_list,
                   s.mac_cod_list,
                   s.Validita_Inizio,
                   s.Validita_Fine,
                   .Azienda = i.rag_soc,
                   .Squadra = s.des_Squadra,
                   .Operazione = op.LAV_DES,
                   .Specie = "",
                   .Attivita = "",
                   .Persone = "",
                   .Macchine = "",
                   .Pubblico = If(s.pubblico = 1, "SI", "NO")
                }).ToList()

            For Each squadra In SquadreAttivita

                Dim id_attivita = squadra.attivita_list.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                Dim lista_attivita = ""
                For Each id In id_attivita
                    Dim desc_attivita = (From x In Attivita Where x.ID_Attivita.ToString = id Select x.Desc).FirstOrDefault()
                    lista_attivita &= If(lista_attivita <> "", ", ", "") & desc_attivita
                Next
                squadra.Attivita = lista_attivita

                If Not String.IsNullOrEmpty(squadra.veg_cod_list) Then
                    Dim id_specie = squadra.veg_cod_list.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    Dim lista_specie = ""
                    For Each id In id_specie
                        Dim desc_specie = (From x In Specie Where x.Veg_Cod.ToString = id Select x.Veg_Des).FirstOrDefault()
                        lista_specie &= If(lista_specie <> "", ", ", "") & desc_specie
                    Next
                    squadra.Specie = lista_specie
                End If

                If Not String.IsNullOrEmpty(squadra.cod_risum_list) Then
                    Dim id_persone = squadra.cod_risum_list.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    Dim lista_persone = ""
                    For Each id In id_persone
                        Dim persona = (From x In Persone Where x.Cod_RisUm.ToString = id Select x).FirstOrDefault()
                        If persona IsNot Nothing Then
                            lista_persone &= If(lista_persone <> "", ", ", "") & persona.Rag_Soc & "" & persona.Cognome & " " & persona.Nome
                        End If
                    Next
                    squadra.Persone = lista_persone
                End If

                If Not String.IsNullOrEmpty(squadra.mac_cod_list) Then
                    Dim id_macchine = squadra.mac_cod_list.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    Dim lista_macchine = ""
                    For Each id In id_macchine
                        Dim desc_macchina = (From x In Macchine Where x.Mac_Cod.ToString = id Select x.Mac_Des).FirstOrDefault()
                        If desc_macchina IsNot Nothing Then
                            lista_macchine &= If(lista_macchine <> "", ", ", "") & desc_macchina
                        End If
                    Next
                    squadra.Macchine = lista_macchine
                End If

            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(SquadreAttivita, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function Leggi_Squadre_Attivita_Demetra(ByVal piva As String,
                                           ByRef objParametri As AgronicaCoreParametri) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Squadre_Attivita_Demetra()"
        Dim separators As String() = {"|"}

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Attivita = (From x In GiasContext.Attivita Select New With {x.ID_Attivita, x.Desc}).ToList()
            Dim Specie = (From x In GiasContext.SpecieVegetali Select New With {x.Veg_Cod, x.Veg_Des}).ToList()
            Dim Persone = (From ru In GiasContext.Risorse_Umane
                           Join c In GiasContext.Contatti On c.Cod_Contatto Equals ru.Cod_Contatto
                           Join rc In GiasContext.Rapporti_Contabili On rc.Cod_Rapporto Equals ru.Cod_Rapporto
                           Where (ru.Piva.Equals(piva) OrElse (c.Sa_Cod = -1)) AndAlso (ru.Piva = c.Piva OrElse (c.Sa_Cod = -1))
                           Select New With {ru.Cod_RisUm, c.Rag_Soc, c.Cognome, c.Nome}).ToList()
            Dim Macchine = (From x In GiasContext.Parco_Macchine Where (x.Piva.Equals(piva) OrElse (x.Sa_Cod = -1)) AndAlso x.Tipo = 0
                            Select New With {x.Mac_Cod, x.Mac_Des}).ToList()

            Dim SquadreAttivita =
               (From s In GiasContext.SquadreXAttivita
                Join i In GiasContext.Imprese On s.Piva Equals i.PIVA
                Group Join o In GiasContext.Operazioni
                    On s.Lav_Cod Equals o.LAV_COD
                    Into og = Group From op In og.DefaultIfEmpty()
                Where String.IsNullOrEmpty(piva) OrElse s.Piva = piva
                Order By i.rag_soc, s.des_Squadra
                Select New With {
                   s.Piva_SuperUser,
                   s.Piva,
                   s.ID_Squadra,
                   s.Lav_Cod,
                   s.attivita_list,
                   s.veg_cod_list,
                   s.cod_risum_list,
                   s.cod_risum_caposquadra_list,
                   s.mac_cod_list,
                   s.Validita_Inizio,
                   s.Validita_Fine,
                   .Azienda = i.rag_soc,
                   .Squadra = s.des_Squadra,
                   .Operazione = op.LAV_DES,
                   .Specie = "",
                   .Attivita = "",
                   .Persone = "",
                   .Caposquadra = "",
                   .Macchine = "",
                   .Pubblico = If(s.pubblico = 1, "SI", "NO")
                }).ToList()

            For Each squadra In SquadreAttivita

                Dim lista_attivita = ""
                If Not String.IsNullOrEmpty(squadra.attivita_list) Then
                    Dim id_attivita = squadra.attivita_list.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    For Each id In id_attivita
                        Dim desc_attivita = (From x In Attivita Where x.ID_Attivita.ToString = id Select x.Desc).FirstOrDefault()
                        lista_attivita &= If(lista_attivita <> "", ", ", "") & desc_attivita
                    Next
                End If
                squadra.Attivita = lista_attivita

                If Not String.IsNullOrEmpty(squadra.veg_cod_list) Then
                    Dim id_specie = squadra.veg_cod_list.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    Dim lista_specie = ""
                    For Each id In id_specie
                        Dim desc_specie = (From x In Specie Where x.Veg_Cod.ToString = id Select x.Veg_Des).FirstOrDefault()
                        lista_specie &= If(lista_specie <> "", ", ", "") & desc_specie
                    Next
                    squadra.Specie = lista_specie
                End If

                If Not String.IsNullOrEmpty(squadra.cod_risum_list) Then
                    Dim id_persone = squadra.cod_risum_list.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    Dim lista_persone = ""
                    For Each id In id_persone
                        Dim persona = (From x In Persone Where x.Cod_RisUm.ToString = id Select x).FirstOrDefault()
                        If persona IsNot Nothing Then
                            lista_persone &= If(lista_persone <> "", ", ", "") & persona.Rag_Soc & "" & persona.Cognome & " " & persona.Nome
                        End If
                    Next
                    squadra.Persone = lista_persone
                End If

                If Not String.IsNullOrEmpty(squadra.cod_risum_caposquadra_list) Then
                    Dim id_persone = squadra.cod_risum_caposquadra_list.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    Dim lista_caposquadra = ""
                    For Each id In id_persone
                        Dim persona = (From x In Persone Where x.Cod_RisUm.ToString = id Select x).FirstOrDefault()
                        If persona IsNot Nothing Then
                            lista_caposquadra &= If(lista_caposquadra <> "", ", ", "") & persona.Rag_Soc & "" & persona.Cognome & " " & persona.Nome
                        End If
                    Next
                    squadra.Caposquadra = lista_caposquadra
                End If

                If Not String.IsNullOrEmpty(squadra.mac_cod_list) Then
                    Dim id_macchine = squadra.mac_cod_list.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    Dim lista_macchine = ""
                    For Each id In id_macchine
                        Dim desc_macchina = (From x In Macchine Where x.Mac_Cod.ToString = id Select x.Mac_Des).FirstOrDefault()
                        If desc_macchina IsNot Nothing Then
                            lista_macchine &= If(lista_macchine <> "", ", ", "") & desc_macchina
                        End If
                    Next
                    squadra.Macchine = lista_macchine
                End If

            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(SquadreAttivita, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function


    Public Function Nuova_Squadra_Attivita(ByVal piva As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim risposta As String

        Dim Squadra As SquadreXAttivita = New SquadreXAttivita
        Squadra.Piva_SuperUser = objParametri.PivaSuperUser
        Squadra.Piva = piva
        Squadra.pubblico = 0
        Squadra.Validita_Inizio = AGRODATAINIZIO
        Squadra.Validita_Fine = AGRODATAFINE

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        risposta = JsonConvert.SerializeObject(Squadra, Formatting.None, serializerSettings)

        Return risposta

    End Function

    Public Function Leggi_Squadra_Attivita(ByVal piva As String, ByVal ID_Squadra As Integer, ByRef objParametri As AgronicaCoreParametri) As String

        Dim risposta As String
        Dim Squadra As SquadreXAttivita = Nothing
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Squadra = (From s In GiasContext.SquadreXAttivita
                       Where s.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso s.Piva.Equals(piva) AndAlso (s.ID_Squadra = ID_Squadra)
                       Select s).FirstOrDefault()

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Squadra, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function Leggi_Operazioni(ByRef objParametri As AgronicaCoreParametri) As String

        Dim risposta As String
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Operazioni = (From o In GiasContext.Operazioni Order By o.LAV_DES Select o).ToList()
            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Operazioni, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function Leggi_Attivita(ByVal Lav_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As String

        Dim risposta As String
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim listaAttivita As New List(Of Integer)
            If Lav_Cod <> 0 Then
                listaAttivita = (From x In GiasContext.AttivitaXOperazioni Where x.Lav_Cod = Lav_Cod Select x.ID_Attivita).Distinct().ToList()
            End If

            Dim Attivita = (From a In GiasContext.Attivita Where Lav_Cod = 0 OrElse listaAttivita.Contains(a.ID_Attivita) Order By a.Desc).ToList()

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Attivita, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Persone(ByVal piva As String,
                                   ByVal cod_rapporto As Integer,
                                   ByVal pubblico As Boolean,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional leggixSquadreCdG As Boolean = False
                                   ) As String

        Dim risposta As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            If leggixSquadreCdG Then
                'Se provengo dalla lettura persone x Squadre nel CgG, prima leggo i capi area poi i tecnici
                cod_rapporto = COD_CAPO_AREA
            End If

            Dim Persone =
                 (From ru In GiasContext.Risorse_Umane
                  Join c In GiasContext.Contatti On c.Cod_Contatto Equals ru.Cod_Contatto
                  Join rc In GiasContext.Rapporti_Contabili On rc.Cod_Rapporto Equals ru.Cod_Rapporto
                  Where (ru.Piva.Equals(piva) OrElse (c.Sa_Cod = -1)) AndAlso (ru.Piva = c.Piva OrElse (c.Sa_Cod = -1)) AndAlso
                      If(cod_rapporto <> 0, ru.Cod_Rapporto = cod_rapporto, rc.Dipendente = 1) AndAlso
                      If(pubblico, c.Sa_Cod = -1, c.Sa_Cod <> -1)
                  Order By c.Rag_Soc, c.Cognome, c.Nome
                  Select New With {
                     ru.Cod_RisUm,
                     c.Rag_Soc,
                     c.Cognome,
                     c.Nome,
                     ru.Cod_Rapporto,
                     rc.Dipendente,
                     rc.Rapporto_Des,
                     ru.Qualifica_Cod,
                     ru.Validita_Inizio,
                     ru.Validita_Fine,
                     .Pubblico = c.Sa_Cod = -1,
                     ru.Cod_Contatto
                 }).ToList()

            If leggixSquadreCdG Then
                Dim list_CodContatto_CapiArea As New List(Of String)
                list_CodContatto_CapiArea.AddRange(Persone.Select(Function(x) x.Cod_Contatto).ToList())

                Dim Persone_Tecnici =
                 (From ru In GiasContext.Risorse_Umane
                  Join c In GiasContext.Contatti On c.Cod_Contatto Equals ru.Cod_Contatto
                  Join rc In GiasContext.Rapporti_Contabili On rc.Cod_Rapporto Equals ru.Cod_Rapporto
                  Where (ru.Piva.Equals(piva) OrElse (c.Sa_Cod = -1)) AndAlso (ru.Piva = c.Piva OrElse (c.Sa_Cod = -1)) AndAlso
                      (rc.Dipendente = 1 OrElse rc.Terzista = 1) AndAlso
                      If(pubblico, c.Sa_Cod = -1, c.Sa_Cod <> -1) AndAlso
                      Not list_CodContatto_CapiArea.Contains(ru.Cod_Contatto)
                  Order By c.Rag_Soc, c.Cognome, c.Nome
                  Select New With {
                      ru.Cod_RisUm,
                      c.Rag_Soc,
                      c.Cognome,
                      c.Nome,
                      ru.Cod_Rapporto,
                      rc.Dipendente,
                      rc.Rapporto_Des,
                      ru.Qualifica_Cod,
                      ru.Validita_Inizio,
                      ru.Validita_Fine,
                      .Pubblico = c.Sa_Cod = -1,
                      ru.Cod_Contatto
                      }).ToList()

                If Persone_Tecnici.Count > 0 Then
                    Persone.AddRange(Persone_Tecnici)
                End If
            End If

            For Each obj In Persone
                obj.Rag_Soc = obj.Rag_Soc & "" & obj.Cognome & " " & obj.Nome & If(leggixSquadreCdG AndAlso obj.Cod_Rapporto = COD_CAPO_AREA, " (Capo Area)", "")
            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Persone, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function Leggi_Persone_Demetra(ByVal piva As String,
                                   ByVal cod_rapporto As Integer,
                                   ByVal pubblico As Boolean,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional leggixSquadreCdG As Boolean = False
                                   ) As String

        Dim risposta As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim lavoratori As Integer() = CostantiPersonalizzate.SQUADRE_LAVORATORI_LIST.ToArray()

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            If leggixSquadreCdG Then
                'Se provengo dalla lettura persone x Squadre nel CgG, prima leggo i capi area poi i tecnici
                cod_rapporto = COD_CAPO_AREA
            End If

            Dim Persone =
                 (From ru In GiasContext.Risorse_Umane.Where(Function(x) lavoratori.Any(Function(l) l.Equals(x.Cod_Rapporto)))
                  Join c In GiasContext.Contatti
                      On c.Cod_Contatto Equals ru.Cod_Contatto
                  Join rc In GiasContext.Rapporti_Contabili.Where(Function(x) lavoratori.Any(Function(l) l.Equals(x.Cod_Rapporto)))
                      On rc.Cod_Rapporto Equals ru.Cod_Rapporto
                  Where (ru.Piva.Equals(piva) OrElse (c.Sa_Cod = -1)) AndAlso (ru.Piva = c.Piva OrElse (c.Sa_Cod = -1)) AndAlso
                      If(pubblico, c.Sa_Cod = -1, c.Sa_Cod <> -1)
                  Order By c.Rag_Soc, c.Cognome, c.Nome
                  Select New With {
                     ru.Cod_RisUm,
                     c.Rag_Soc,
                     c.Cognome,
                     c.Nome,
                     ru.Cod_Rapporto,
                     rc.Dipendente,
                     rc.Rapporto_Des,
                     ru.Qualifica_Cod,
                     ru.Validita_Inizio,
                     ru.Validita_Fine,
                     .Pubblico = c.Sa_Cod = -1,
                     ru.Cod_Contatto
                 }).ToList()

            If leggixSquadreCdG Then
                Dim list_CodContatto_CapiArea As New List(Of String)
                list_CodContatto_CapiArea.AddRange(Persone.Select(Function(x) x.Cod_Contatto).ToList())

                Dim Persone_Tecnici =
                 (From ru In GiasContext.Risorse_Umane.Where(Function(x) lavoratori.Any(Function(l) l.Equals(x.Cod_Rapporto)))
                  Join c In GiasContext.Contatti
                      On c.Cod_Contatto Equals ru.Cod_Contatto
                  Join rc In GiasContext.Rapporti_Contabili.Where(Function(x) lavoratori.Any(Function(l) l.Equals(x.Cod_Rapporto)))
                      On rc.Cod_Rapporto Equals ru.Cod_Rapporto
                  Where (ru.Piva.Equals(piva) OrElse (c.Sa_Cod = -1)) AndAlso (ru.Piva = c.Piva OrElse (c.Sa_Cod = -1)) AndAlso
                      If(pubblico, c.Sa_Cod = -1, c.Sa_Cod <> -1) AndAlso
                      Not list_CodContatto_CapiArea.Contains(ru.Cod_Contatto)
                  Order By c.Rag_Soc, c.Cognome, c.Nome
                  Select New With {
                      ru.Cod_RisUm,
                      c.Rag_Soc,
                      c.Cognome,
                      c.Nome,
                      ru.Cod_Rapporto,
                      rc.Dipendente,
                      rc.Rapporto_Des,
                      ru.Qualifica_Cod,
                      ru.Validita_Inizio,
                      ru.Validita_Fine,
                      .Pubblico = c.Sa_Cod = -1,
                      ru.Cod_Contatto
                      }).ToList()

                If Persone_Tecnici.Count > 0 Then
                    Persone.AddRange(Persone_Tecnici)
                End If
            End If

            For Each obj In Persone
                obj.Rag_Soc = $"{obj.Rag_Soc} {obj.Cognome} {obj.Nome} ({obj.Rapporto_Des})"
            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Persone, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Macchine(ByVal piva As String,
                                   ByVal ctrl_gestione As Boolean,
                                   ByVal pubblico As Boolean,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As String

        Dim risposta As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Macchine =
                 From pm In GiasContext.Parco_Macchine
                 Where (pm.Piva.Equals(piva) OrElse (pm.Sa_Cod = -1)) AndAlso pm.Tipo = 0
                 Order By pm.Mac_Des
                 Select New With {
                     pm.Mac_Cod,
                     pm.Mac_Des,
                     pm.Visibile_ctrl_gestione,
                     .Pubblico = pm.Sa_Cod = -1
                 }

            If Not IsNothing(pubblico) Then
                Macchine = Macchine.Where(Function(x) x.Pubblico = pubblico)
            End If

            If Not IsNothing(ctrl_gestione) Then
                Macchine = Macchine.Where(Function(x) x.Visibile_ctrl_gestione = 1)
            End If

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Macchine.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Id_Imputazione(ByVal piva As String,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByVal Controlla_Data_CDG_Testata As Boolean,
                                        ByVal Id_Imputazione As Integer,
                                        ByVal Id_Attivita As Integer,
                                        ByVal Validita_Inizio As Date,
                                        ByVal Validita_Fine As Date
                                        ) As String


        Dim risposta As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)


            If Controlla_Data_CDG_Testata = False Then
                Dim DettagliElem =
                   From CDG_Dettagli In GiasContext.CDG_Dettagli
                   Where
                  (CDG_Dettagli.Piva.Equals(piva)) AndAlso
                   (CDG_Dettagli.Piva_Superuser.Equals(Piva_SuperUser)) AndAlso
                   (CDG_Dettagli.Id_Imputazione = Id_Imputazione)
                   Select New With {
                    .Piva = CDG_Dettagli.Piva,
                    .Id_CDG = CDG_Dettagli.Id_CDG,
                    .Id_Imputazione = CDG_Dettagli.Id_Imputazione
                    }


                risposta = JsonConvert.SerializeObject(DettagliElem.FirstOrDefault(), Formatting.None, serializerSettings)

            Else

                Dim DettagliTestata =
                       From CDG_Dettagli In GiasContext.CDG_Dettagli
                       Join CDG_Testata In GiasContext.CDG_Testata
                           On CDG_Dettagli.Piva Equals CDG_Testata.Piva And
                           CDG_Dettagli.Piva_Superuser Equals CDG_Testata.Piva_Superuser And
                           CDG_Dettagli.Id_CDG Equals CDG_Testata.Id_CDG
                       Where
                        (CDG_Testata.Budget = 0) AndAlso
                      (CDG_Dettagli.Piva.Equals(piva)) AndAlso
                       (CDG_Dettagli.Piva_Superuser.Equals(Piva_SuperUser)) AndAlso
                        (CDG_Dettagli.Id_Imputazione = Id_Imputazione) AndAlso
                        (CDG_Testata.Id_Attivita = Id_Attivita)
                       Select New With {
                           .Piva = CDG_Dettagli.Piva,
                          .Id_CDG = CDG_Dettagli.Id_CDG,
                          .Id_CDG_Dettagli = CDG_Dettagli.Id_CDG_Dettagli,
                          .Id_Imputazione = CDG_Dettagli.Id_Imputazione,
                          .Id_Attivita = CDG_Testata.Id_Attivita,
                          .Data_Inserimento = CDG_Testata.Data_Inserimento
                       }

                risposta = JsonConvert.SerializeObject(DettagliTestata.FirstOrDefault(), Formatting.None, serializerSettings)
            End If

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Controlla_Modifica_Validita_ProgettiAttivita(ByVal piva As String,
                                                                ByRef objParametri As AgronicaCoreParametri,
                                                                ByVal Validita_Inizio As Date,
                                                                ByVal Validita_Fine As Date,
                                                                Optional ByVal Id_Imputazione As Integer? = Nothing,
                                                                Optional ByVal Id_Attivita As Integer? = Nothing
                                                                ) As Boolean

        Dim risposta As Boolean = False
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim DettagliTestata =
                   From CDG_Dettagli In GiasContext.CDG_Dettagli
                   Join CDG_Testata In GiasContext.CDG_Testata
                       On CDG_Dettagli.Piva Equals CDG_Testata.Piva And
                       CDG_Dettagli.Piva_Superuser Equals CDG_Testata.Piva_Superuser And
                       CDG_Dettagli.Id_CDG Equals CDG_Testata.Id_CDG
                   Where
                       (CDG_Testata.Budget = 0) AndAlso
                  (CDG_Dettagli.Piva.Equals(piva)) AndAlso
                   (CDG_Dettagli.Piva_Superuser.Equals(Piva_SuperUser))
                   Select New With {
                       .Piva = CDG_Dettagli.Piva,
                      .Id_CDG = CDG_Dettagli.Id_CDG,
                      .Id_CDG_Dettagli = CDG_Dettagli.Id_CDG_Dettagli,
                      .Id_Imputazione = CDG_Dettagli.Id_Imputazione,
                      .Id_Attivita = CDG_Testata.Id_Attivita,
                      .Data_Inserimento = CDG_Testata.Data_Inserimento
                   }

            If Id_Imputazione IsNot Nothing Then
                DettagliTestata = DettagliTestata.Where(Function(x) x.Id_Imputazione = Id_Imputazione)
            End If

            If Id_Attivita IsNot Nothing Then
                DettagliTestata = DettagliTestata.Where(Function(x) x.Id_Attivita = Id_Attivita)
            End If


            Dim risultato = DettagliTestata.Distinct().ToList()


            If risultato.Count > 0 Then

                Dim Numero_Righe_FilltroImputazione = risultato.Count

                Dim dv = risultato.Where(Function(f) f.Data_Inserimento >= Validita_Inizio AndAlso
                                             f.Data_Inserimento <= Validita_Fine)

                'Controllo che rimangano visibili tutti i Progetti in quella validita di tempo
                If dv IsNot Nothing AndAlso dv.Count = Numero_Righe_FilltroImputazione Then
                    risposta = True
                End If

            Else
                risposta = True
            End If

        End Using


        Return risposta

    End Function

    '##############################################################################################
    Public Function Controlla_Id_Attivita_InCDG_Testata(ByVal piva As String,
                                                        ByRef objParametri As AgronicaCoreParametri,
                                                        ByVal Id_Attivita As Integer) As Boolean

        Dim risposta As Boolean = False
        Dim risultato As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)


            Dim TestataElem =
                From CDG_Testata In GiasContext.CDG_Testata
                Where
                    (CDG_Testata.Budget = 0) AndAlso
                (CDG_Testata.Piva.Equals(piva)) AndAlso
                (CDG_Testata.Piva_Superuser.Equals(Piva_SuperUser)) AndAlso
                (CDG_Testata.Id_Attivita = Id_Attivita)
                Select New With {
                .Piva = CDG_Testata.Piva,
                .Id_CDG = CDG_Testata.Id_CDG,
                .Id_Attivita = CDG_Testata.Id_Attivita
                }


            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risultato = JsonConvert.SerializeObject(TestataElem.FirstOrDefault(), Formatting.None, serializerSettings)

            If risultato <> "" AndAlso risultato <> "null" AndAlso risultato <> "[]" Then
                risposta = True
            End If

        End Using

        Return risposta

    End Function





    Public Function CheckCorrettezzaDettagli(ByVal piva As String,
                                             ByVal Id_Agenda As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.CheckCorrettezzaDettagli()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" Select Top 1 CDG_Testata.Id_CDG, Sum(Valore) AS Totale")

            strSql.Append(" From CDG_Testata, CDG_Dettagli ")
            strSql.Append(" Where CDG_Testata.Piva = '" & Agro_SQL_SaveText(piva) & "'   ")
            strSql.Append(" And CDG_Testata.Piva = CDG_Dettagli.Piva And CDG_Testata.Budget = 0 ")
            strSql.Append(" And CDG_Testata.Id_Agenda = " & Id_Agenda & " ")
            strSql.Append(" And CDG_Testata.Id_CDG = CDG_Dettagli.Id_CDG ")
            strSql.Append(" Group by CDG_Testata.Id_CDG ")
            strSql.Append(" Having sum(valore) <> 100 ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try


        Return dt

    End Function

    Public Function Leggi_CDG_Testata_Da_Budget(ByVal Id_Budget As Int32,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_CDG_Testata()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0

            strSql.AppendLine(" SELECT * From CDG_Testata  ")
            strSql.AppendLine(" Where Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine(" ORDER BY Id_CDG, Id_Agenda") 'Importante!!

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try


        Return dt


    End Function


    Public Function Leggi_CDG_Testata(ByVal piva As String,
                                      ByVal id_agenda As String,
                                      ByVal id_cdg As Integer,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal Id_Budget As Integer = 0
                                      ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_CDG_Testata()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0

            strSql.AppendLine(" SELECT * From CDG_Testata  ")
            strSql.AppendLine(" Where piva = " & Agro_SQL_SaveText_NULL(piva))

            If Id_Budget <> -1 Then
                strSql.AppendLine(" AND Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If id_cdg <> 0 Then
                strSql.AppendLine(" AND Id_CDG = " & Agro_SQL_SaveNum(id_cdg))
            End If

            If id_agenda <> 0 Then
                strSql.AppendLine(" AND id_agenda = " & Agro_SQL_SaveNum(id_agenda))
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine(" ORDER BY Id_CDG") 'Importante!!

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try


        Return dt


    End Function

    Public Function LeggiCronologia_CdG(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Appezza As Integer,
                                        ByVal Id_Reg As Integer,
                                        ByVal Progetto_Cod As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional joinAttivita As Boolean = False
                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.LeggiCronologia_CdG()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            '------------------------------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" SELECT a.des_lib, m.data_movimento ")
            strSql.AppendLine(" FROM CDG_Testata Testata ")
            strSql.AppendLine(" JOIN CDG_Dettagli Dettagli ")
            strSql.AppendLine(" ON Testata.Id_CDG = Dettagli.Id_CDG")

            strSql.AppendLine(" JOIN Agenda a ")
            strSql.AppendLine(" ON Testata.Id_Agenda = a.Id_Agenda")

            strSql.AppendLine(" JOIN Movimenti m ")
            strSql.AppendLine(" ON m.Id_Agenda = a.Id_Agenda")

            If joinAttivita Then
                strSql.AppendLine(" LEFT JOIN Attivita")
                strSql.AppendLine(" ON Testata.Id_Attivita = Attivita.ID_Attivita")
            End If

            strSql.AppendLine(" WHERE 1=1")

            If Piva <> "" Then
                strSql.AppendLine(" AND Testata.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If


            If Appezza <> 0 Then
                strSql.AppendLine(" AND Dettagli.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Dettagli.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            If Progetto_Cod <> 0 Then
                strSql.AppendLine(" AND Dettagli.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
            End If

            If joinAttivita Then
                strSql.AppendLine(" AND Attivita_Poliannuale <>  1 ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Data_Inserimento DESC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiCronologia_CdG_Massivo(listChiavi As List(Of (String, Integer, Integer, Integer, Integer)),
                                                profonditaJoin As Enum_EntitaModificaMultiplaPianoColturale,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.LeggiCronologia_CdG_Massivo()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Dim flagConnessione, flagTransazione As Boolean

        Try
            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            TempChiaviMassivo.CreaTabellaTemp_FiltroEsercizi(listChiavi, nomeRoutine, objParametri)

            '------------------------------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" SELECT DISTINCT ")
            strSql.AppendLine("     Dettagli.Piva, Dettagli.Sa_Cod, Dettagli.Appezza, Dettagli.Id_Reg, Dettagli.Progetto_Cod ")
            strSql.AppendLine(" FROM CDG_Testata Testata ")
            strSql.AppendLine(" JOIN CDG_Dettagli Dettagli ")
            strSql.AppendLine(" ON Testata.Id_CDG = Dettagli.Id_CDG")

            strSql.AppendLine(" JOIN Agenda a ")
            strSql.AppendLine(" ON Testata.Id_Agenda = a.Id_Agenda")

            strSql.AppendLine(" JOIN Movimenti m ")
            strSql.AppendLine(" ON m.Id_Agenda = a.Id_Agenda")

            If listChiavi IsNot Nothing AndAlso listChiavi.Count > 0 Then
                strSql.AppendLine("	JOIN #TempEsercizio temp (NOLOCK) ON Dettagli.Piva COLLATE SQL_Latin1_General_CP850_CI_AS = temp.Piva ")
                strSql.AppendLine("	AND Dettagli.Sa_Cod = temp.Sa_Cod ")
                strSql.AppendLine("	AND Dettagli.Appezza = temp.Appezza ")
                strSql.AppendLine("	AND Dettagli.Id_Reg = temp.Id_Reg ")
                strSql.AppendLine("	AND Dettagli.Progetto_Cod = temp.Progetto_Cod ")
            End If

            strSql.AppendLine(" WHERE 1=1")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroEsercizi(nomeRoutine, objParametri)

            'commit transazione
            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing

            'rollback transazione
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return dt

    End Function
End Class


Public Class CDG_DAL_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################

    Public Function Scrivi_Export_Lan(ByVal Piva_SuperUser As String,
                                      ByVal piva As String,
                                      ByVal Vecchio_Tipo_Inser_Dati As Integer,
                                      ByVal ArrayToInsert As JArray,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreParametri
                                      ) As String


        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_W.Scrivi_Export_Lan()"
        Dim messaggioErrore As String = ""

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Try
            'Eseguo comunque sempre la cancellazione con una prima transazione a parte
            '   Se qualcosa andasse male nella fase di inserimento è comunque meglio non avere i dati vecchi

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                'Cancellazione Preventiva
                Dim leggi_CDG As New CDG_DAL_R

                Dim Record_ToDelete As List(Of CDG_Testata) = Nothing

                'Cerco le righe da cancellare di precedenti acconti
                messaggioErrore = leggi_CDG.TrovaRigheCDG(
                                        piva, Vecchio_Tipo_Inser_Dati,
                                        Record_ToDelete,
                                        objParametri_Server)

                If messaggioErrore = "" Then

                    ' Cancellazione lancio precedente
                    For Each lmd As CDG_Testata In Record_ToDelete
                        GiasContext.CDG_Testata.Attach(lmd)
                        GiasContext.CDG_Testata.Remove(lmd)
                    Next

                    GiasContext.SaveChanges()

                End If

            End Using

            ' Stefano 14/2/2018 - con EF 4 andava in Timeout e le prestazioni non erano accettabili
            Utility.VerificaApriTransazione(objParametri_Server, flagConnessione, flagTransazione)

            Dim objSequenze As Agro_Sequenze = Nothing
            Dim progressivoTestata As Integer = 0
            Dim progressivoDettagli As Integer = 0
            Dim dt As DataTable

            For Each obj As JObject In ArrayToInsert

                strSql.Length = 0
                strSql.Append(" Select * From Mov_Dettagli_Riferimenti Where Piva = '" & Agro_SQL_SaveText(piva) & "'")
                strSql.Append(" AND   Lav_Cod_Rif = 4500 ")
                strSql.Append(" AND   Id_Agenda = " & Agro_SQL_SaveNum(obj("Id_Agenda")))
                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri_Server, strSql.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

                If dt.Rows.Count = 0 Then

                    objSequenze = New Agro_Sequenze

                    progressivoTestata = objSequenze.NuovoId_Tabella("CDG_Testata",
                                                                     0, 2000000000,
                                                                      objParametri_Server)

                    progressivoDettagli = objSequenze.NuovoId_Tabella("CDG_Dettagli",
                                                                     0, 2000000000,
                                                                      objParametri_Server)

                    strSql.Length = 0
                    strSql.AppendLine(" Insert INTO CDG_Testata ( ")
                    strSql.AppendLine(" Piva_SuperUser, ")
                    strSql.AppendLine(" piva, ")
                    strSql.AppendLine(" Id_CDG, ")
                    strSql.AppendLine(" Data_Inserimento, ")
                    strSql.AppendLine(" Modalita_Imputazione, ")
                    strSql.AppendLine(" Id_Agenda, ")
                    strSql.AppendLine(" Id_Mov, ")
                    strSql.AppendLine(" Id_Mov_Det, ")
                    strSql.AppendLine(" Mac_Cod, ")
                    strSql.AppendLine(" Cod_RisUm, ")
                    strSql.AppendLine(" Elem_Cod, ")
                    strSql.AppendLine(" Pro_Cod, ")
                    strSql.AppendLine(" Mat_Cod, ")
                    strSql.AppendLine(" Id_Attivita, ")
                    strSql.AppendLine(" Qualifica_Cod, ")
                    strSql.AppendLine(" Tariffa_Cod, ")
                    strSql.AppendLine(" Turno_Cod, ")
                    strSql.AppendLine(" Conto_Cod, ")
                    strSql.AppendLine(" Lotto, ")
                    strSql.AppendLine(" Mezzo, ")
                    strSql.AppendLine(" Udm_Cod, ")
                    strSql.AppendLine(" Prezzo_Unitario, ")
                    strSql.AppendLine(" Qta, ")
                    strSql.AppendLine(" Valore_Totale, ")
                    strSql.AppendLine(" Descrizione, ")
                    strSql.AppendLine(" Tipo_Ripartizione, ")
                    strSql.AppendLine(" Budget, ")
                    strSql.AppendLine(" Costi_Ricavi, ")
                    strSql.AppendLine(" Modalita_Ripartizione, ")
                    strSql.AppendLine(" Vecchio_Tipo_Inser_Dati, ")
                    strSql.AppendLine(" inviato ,")
                    strSql.AppendLine(" datainvio, ")
                    strSql.AppendLine(" Data_Creazione ,")
                    strSql.AppendLine(" Data_Modifica, ")
                    strSql.AppendLine(" Username_Creazione, ")
                    strSql.AppendLine(" Username_Modifica, ")
                    strSql.AppendLine(" Validita_Inizio, ")
                    strSql.AppendLine(" Validita_Fine, ")
                    strSql.AppendLine(" Tipo_Destinazione, ")
                    strSql.AppendLine(" Sa_Cod, ")
                    strSql.AppendLine(" Id_Destinazione ")
                    strSql.AppendLine("          ) ")

                    strSql.AppendLine(" VALUES (")
                    strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva_SuperUser) & "'  ")
                    strSql.AppendLine("         , '" & Agro_SQL_SaveText(piva) & "'  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(progressivoTestata) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(obj("Data_Inserimento")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Modalita_Imputazione")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Id_Agenda")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Id_Mov")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Id_Mov_Det")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Mac_Cod")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Cod_RisUm")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Elem_Cod")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Pro_Cod")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Mat_Cod")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Id_Attivita")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Qualifica_Cod")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Tariffa_Cod")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Turno_Cod")) & "  ")
                    strSql.AppendLine("       , " & Agro_SQL_SaveNum(obj("Conto_Cod")) & "  ")
                    strSql.AppendLine("       , '" & Agro_SQL_SaveText(obj("Lotto")) & " ' ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Mezzo")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Udm_Cod")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(Decimal.Parse(obj("Prezzo_Unitario"), Globalization.CultureInfo.CurrentCulture)) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(Decimal.Parse(obj("Qta"), Globalization.CultureInfo.CurrentCulture)) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(Decimal.Parse(obj("Valore_Totale"), Globalization.CultureInfo.CurrentCulture)) & "  ")
                    strSql.AppendLine("          , '" & Agro_SQL_SaveText(obj("Descrizione")) & "'  ")
                    strSql.AppendLine("      ," & Agro_SQL_SaveNum(obj("Tipo_Ripartizione")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Budget")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("CostiORicavi")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Modalita_Ripartizione")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Tipo_Imputazione")) & "  ")
                    strSql.AppendLine("         , 0 ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
                    strSql.AppendLine("       , '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'  ")
                    strSql.AppendLine("       , '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(AGRODATAFINE) & "  ")
                    strSql.AppendLine("         , 0 ")
                    strSql.AppendLine("         , 0 ")
                    strSql.AppendLine("         , 0 ")
                    strSql.AppendLine(" ) ")

                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
                    '--------------------------------------------------------------------------
                    strSql.Length = 0
                    strSql.AppendLine(" Insert INTO CDG_Dettagli ( ")
                    strSql.AppendLine(" Piva_SuperUser, ")
                    strSql.AppendLine(" piva, ")
                    strSql.AppendLine(" Id_CDG, ")
                    strSql.AppendLine(" Id_CDG_Dettagli, ")
                    strSql.AppendLine(" Sa_Cod, ")
                    strSql.AppendLine(" Appezza, ")
                    strSql.AppendLine(" Id_Reg, ")
                    strSql.AppendLine(" Id_Cod_reg_impianti_codici, ")
                    strSql.AppendLine(" Progetto_Cod, ")
                    strSql.AppendLine(" Id_Imputazione, ")
                    strSql.AppendLine(" Macchine_Cod, ")
                    strSql.AppendLine(" Linea_Cod, ")
                    strSql.AppendLine(" Lotto_Input_Costi, ")
                    strSql.AppendLine(" Valore, ")
                    strSql.AppendLine(" inviato ,")
                    strSql.AppendLine(" datainvio, ")
                    strSql.AppendLine(" Data_Creazione ,")
                    strSql.AppendLine(" Data_Modifica, ")
                    strSql.AppendLine(" Username_Creazione, ")
                    strSql.AppendLine(" Username_Modifica, ")
                    strSql.AppendLine(" Validita_Inizio, ")
                    strSql.AppendLine(" Validita_Fine ")
                    strSql.AppendLine("          ) ")

                    strSql.AppendLine(" VALUES (")

                    strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva_SuperUser) & "'  ")
                    strSql.AppendLine("         ,  '" & Agro_SQL_SaveText(piva) & "'  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(progressivoTestata) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(progressivoDettagli) & "  ")

                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Sa_Cod")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Appezza")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Id_Impianto")) & "  ")


                    strSql.Append(" ,  ISNULL(( SELECT TOP 1 Reg_Impianti_Codici.id_cod  " & vbCrLf)
                    strSql.Append(" From Reg_Impianti_Codici  " & vbCrLf)
                    strSql.Append(" WHERE Reg_Impianti_Codici.PIVA = '" & Agro_SQL_SaveText(piva) & "' " & vbCrLf)
                    strSql.Append(" And   Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(obj("Sa_Cod")) & "  " & vbCrLf)
                    strSql.Append(" And   Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(obj("Appezza")) & "  " & vbCrLf)
                    strSql.Append(" And   Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(obj("Id_Impianto")) & "  " & vbCrLf)
                    strSql.Append(" And   (Reg_Impianti_Codici.Progetto_Cod = 0)  " & vbCrLf)
                    'strSql.Append(" And (Codici_Anagrafe.gruppo = 'TERRENO')  " & vbCrLf)
                    strSql.Append(" And Reg_Impianti_Codici.id_cod > 3000 And Reg_Impianti_Codici.id_cod <= 4000 " & vbCrLf)
                    strSql.Append(" ) , 0)   " & vbCrLf)

                    strSql.AppendLine("         , 0 ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Id_Imputazione")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Macchine_Cod")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Linea_Cod")) & "  ")
                    strSql.AppendLine("         , ''  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(Decimal.Parse(obj("Valore"), Globalization.CultureInfo.CurrentCulture)) & "  ")
                    strSql.AppendLine("         , 0 ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
                    strSql.AppendLine("       , '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'  ")
                    strSql.AppendLine("       , '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(AGRODATAFINE) & "  ")
                    strSql.AppendLine(" ) ")

                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
                    '--------------------------------------------------------------------------

                End If

            Next

            'FASE 2 - Agg-to DW
            Dim agg_DW_CDG_Costi_Ricavi As New DW_CDG_Costi_Ricavi_DAL_W

            messaggioErrore = agg_DW_CDG_Costi_Ricavi.AggiornaDW_Da_Lan(
                                        piva, Vecchio_Tipo_Inser_Dati,
                                        objParametri_Server, objParametri_Utenti)

            Utility.VerificaChiudiTransazione(objParametri_Server, flagTransazione)

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri_Server, flagTransazione)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally
            Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)
        End Try

        Return messaggioErrore

    End Function



    Public Function Delete_CDG(ByVal piva As String,
                               ByVal id_agenda_cdg As Integer,
                               ByVal id_mov_det_cdg As Integer,
                               ByVal des_lib As String,
                               ByVal data_inserimento As Date,
                               ByRef objParametri As AgronicaCoreParametri
                               ) As String

        Dim ObjSequenze = New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.CDG_DAL_W.Delete_CDG()"
        Dim messaggioErrore As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim cdgRead As New CDG_DAL_R

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            'Cerca se si sta cancellando un'operazione con esercizi Campagna o Zoo chiusi
            messaggioErrore = cdgRead.CercaProgettiChiusi(piva, id_agenda_cdg, objParametri)

            If messaggioErrore = "" Then

                Dim transactionOptions = New TransactionOptions()
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                    Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                        '#################################################################################################################
                        '###########################  Cancellazione Dati #################################################################
                        '#################################################################################################################

                        Dim ElencoMov_Destinazioni = (From CI In GiasContext.Mov_Destinazioni
                                                      Join CDGTestata In GiasContext.CDG_Testata
                                                      On CDGTestata.Piva Equals CI.Piva And
                                                     CDGTestata.Id_Agenda Equals CI.Id_Agenda And
                                                     CDGTestata.Id_Mov Equals CI.Id_Mov And
                                                     CDGTestata.Id_Mov_Det Equals CI.Id_Mov_Det
                                                      Where CI.Piva.Equals(piva) AndAlso
                                                            CI.Id_Agenda = id_agenda_cdg
                                                      Select CI).Distinct().ToList()


                        For Each Mov_Destinazioni_Delete As Mov_Destinazioni In ElencoMov_Destinazioni
                            GiasContext.Mov_Destinazioni.Attach(Mov_Destinazioni_Delete)
                            GiasContext.Mov_Destinazioni.Remove(Mov_Destinazioni_Delete)
                        Next

                        GiasContext.SaveChanges()





                        Dim ElencoMovimenti_Dettagli = (From CI In GiasContext.Movimenti_dettagli
                                                        Join CDGTestata In GiasContext.CDG_Testata
                                                  On CDGTestata.Piva Equals CI.PIVA And
                                                     CDGTestata.Id_Agenda Equals CI.Id_Agenda And
                                                     CDGTestata.Id_Mov Equals CI.Id_Mov And
                                                     CDGTestata.Id_Mov_Det Equals CI.Id_Mov_Det
                                                        Where CI.PIVA.Equals(piva) AndAlso
                                                              CI.Id_Agenda = id_agenda_cdg
                                                        Select CI).Distinct().ToList()


                        For Each Movimenti_Dettagli_Delete As Movimenti_dettagli In ElencoMovimenti_Dettagli
                            GiasContext.Movimenti_dettagli.Attach(Movimenti_Dettagli_Delete)
                            GiasContext.Movimenti_dettagli.Remove(Movimenti_Dettagli_Delete)
                        Next

                        GiasContext.SaveChanges()


                        Dim ElencoMovimenti = (From CI In GiasContext.Movimenti
                                               Join CDGTestata In GiasContext.CDG_Testata
                                                  On CDGTestata.Piva Equals CI.PIVA And
                                                     CDGTestata.Id_Agenda Equals CI.Id_Agenda And
                                                     CDGTestata.Id_Mov Equals CI.Id_Mov
                                               Where CI.PIVA.Equals(piva) AndAlso
                                                     CI.Id_Agenda = id_agenda_cdg
                                               Select CI).Distinct().ToList()


                        For Each Movimenti_Delete As Movimenti In ElencoMovimenti
                            GiasContext.Movimenti.Attach(Movimenti_Delete)
                            GiasContext.Movimenti.Remove(Movimenti_Delete)
                        Next

                        GiasContext.SaveChanges()


                        Dim ElencoAgenda_Riferimenti = (From CI In GiasContext.Mov_Dettagli_Riferimenti
                                                        Where CI.Piva.Equals(piva) AndAlso
                                                              CI.Id_Agenda_Rif = id_agenda_cdg
                                                        Select CI).Distinct().ToList()


                        For Each Riferimenti_Delete As Mov_Dettagli_Riferimenti In ElencoAgenda_Riferimenti
                            GiasContext.Mov_Dettagli_Riferimenti.Attach(Riferimenti_Delete)
                            GiasContext.Mov_Dettagli_Riferimenti.Remove(Riferimenti_Delete)
                        Next

                        GiasContext.SaveChanges()


                        Dim ElencoAgenda = (From CI In GiasContext.Agenda
                                            Where CI.PIVA.Equals(piva) AndAlso
                                                  CI.Id_Agenda = id_agenda_cdg
                                            Select CI).Distinct().ToList()


                        For Each Agenda_Delete As Agenda In ElencoAgenda
                            GiasContext.Agenda.Attach(Agenda_Delete)
                            GiasContext.Agenda.Remove(Agenda_Delete)
                        Next

                        GiasContext.SaveChanges()


                        Dim ElencoTestate = (From t In GiasContext.CDG_Testata.Include("CDG_Dettagli")
                                             Where t.Piva_Superuser.Equals(Piva_SuperUser) AndAlso
                                                   t.Piva.Equals(piva) AndAlso
                                                   t.Id_Agenda = id_agenda_cdg).ToList()

                        For Each CDG_Testata_Delete As CDG_Testata In ElencoTestate
                            GiasContext.CDG_Testata.Attach(CDG_Testata_Delete)
                            GiasContext.CDG_Testata.Remove(CDG_Testata_Delete)
                        Next

                        GiasContext.SaveChanges()



                        'Agronica_Log_Agenda
                        Dim Agronica_Log_Agenda As New Agronica_Log_Agenda

                        Agronica_Log_Agenda.SuperUser = objParametri.PivaSuperUser
                        Agronica_Log_Agenda.Piva = piva
                        Agronica_Log_Agenda.Sa_Cod = 0
                        Agronica_Log_Agenda.Id_Agenda = id_agenda_cdg
                        Agronica_Log_Agenda.Utente = objParametri.UsernameOperazione
                        Agronica_Log_Agenda.Lav_Cod = LAVCOD_COSTI_CDG
                        Agronica_Log_Agenda.Des_lib = des_lib
                        Agronica_Log_Agenda.Data_Ora_Lavorazione = data_inserimento
                        Agronica_Log_Agenda.Data_Ora_RegistrazioneLog = Date.Now
                        Agronica_Log_Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Cancellazione
                        Agronica_Log_Agenda.Id_Servizio = 5

                        GiasContext.Agronica_Log_Agenda.Add(Agronica_Log_Agenda)
                        GiasContext.SaveChanges()


                        ' COMMIT Effettivo
                        scope.Complete()


                    End Using
                End Using

            Else
                Return messaggioErrore
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


    End Function



    Public Function Aggiorna_CDG(ByVal bScaricoTempi As Boolean,
                                 ByVal Modalita_Imputazione As Integer,
                                 ByVal Piva As String,
                                 ByVal Id_Agenda_CDG As Integer,
                                 ByVal Id_CDG As Integer,
                                 ByVal Des_Lib As String,
                                 ByVal Data_Inserimento As DateTime,
                                 ByVal EFArrayToInsert As ArrayList,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing
                                 ) As Integer

        Dim ObjSequenze = New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True
        Dim successTestata = True
        Dim successComplessivo As Boolean = True

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.CDG_DAL_W.Aggiorna_CDG()"
        Dim messaggioErrore As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        'Dim gefutils AS New Gias_EF_Utility
        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim Id_Agenda_New As Integer
        Dim Dummy As Integer
        Dim Tipo_Operazione As Integer

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        Try

            If Id_Agenda_CDG <> 0 Then

                '#################################################################################################################
                '###########################  Cancellazione Dati #################################################################
                '#################################################################################################################
                Select Case bScaricoTempi

                    Case True

                        'Cancellazione Testata Puntuale
                        Dim ElencoTestate = (From t In GiasContext.CDG_Testata.Include("CDG_Dettagli")
                                             Where t.Piva_Superuser.Equals(Piva_SuperUser) AndAlso
                                                   t.Piva.Equals(Piva) AndAlso
                                                   t.Id_Agenda = Id_Agenda_CDG AndAlso
                                                   t.Id_CDG = Id_CDG).ToList()


                        For Each CDG_Testata_Delete As CDG_Testata In ElencoTestate
                            GiasContext.CDG_Testata.Attach(CDG_Testata_Delete)
                            GiasContext.CDG_Testata.Remove(CDG_Testata_Delete)
                        Next

                        GiasContext.SaveChanges()


                        'Controllo se l'id_agenda_cdg è ancora consistente (se ha ancora almeno una testata) 
                        Dim DT As DataTable
                        Dim leggi As New CDG_DAL_R

                        DT = leggi.Leggi_Agenda_Consistente_ScaricoTempi(Piva, Id_Agenda_CDG, objParametri, GiasContext:=GiasContext)


                        If DT.Rows.Count = 0 Then

                            'Cancellazione Id_Agenda
                            Dummy = Cancella_Agenda(Modalita_Imputazione, Piva, Id_Agenda_CDG, Des_Lib, Data_Inserimento, objParametri, GiasContext:=GiasContext)

                            Tipo_Operazione = CInt(enum_TipoOperazioneDB.Cancellazione)

                        Else
                            Tipo_Operazione = CInt(enum_TipoOperazioneDB.Modifica)
                        End If

                    Case False


                        'Cancellazione Id_Agenda Totale
                        Dim ElencoTestate = (From t In GiasContext.CDG_Testata.Include("CDG_Dettagli")
                                             Where t.Piva_Superuser.Equals(Piva_SuperUser) AndAlso
                                                   t.Piva.Equals(Piva) AndAlso
                                                   t.Id_Agenda = Id_Agenda_CDG).ToList()


                        'Cancellazione Id_Agenda
                        Dummy = Cancella_Agenda(Modalita_Imputazione, Piva, Id_Agenda_CDG, Des_Lib, Data_Inserimento, objParametri, GiasContext:=GiasContext)


                        For Each CDG_Testata_Delete As CDG_Testata In ElencoTestate
                            GiasContext.CDG_Testata.Attach(CDG_Testata_Delete)
                            GiasContext.CDG_Testata.Remove(CDG_Testata_Delete)
                        Next

                        GiasContext.SaveChanges()

                        Tipo_Operazione = CInt(enum_TipoOperazioneDB.Cancellazione)

                End Select

                'Agronica_Log_Agenda
                Dim Agronica_Log_Agenda As New Agronica_Log_Agenda

                Agronica_Log_Agenda.SuperUser = objParametri.PivaSuperUser
                Agronica_Log_Agenda.Piva = Piva
                Agronica_Log_Agenda.Sa_Cod = 0
                Agronica_Log_Agenda.Id_Agenda = Id_Agenda_CDG
                Agronica_Log_Agenda.Utente = objParametri.UsernameOperazione
                Agronica_Log_Agenda.Lav_Cod = LAVCOD_COSTI_CDG
                Agronica_Log_Agenda.Des_lib = Des_Lib
                Agronica_Log_Agenda.Data_Ora_Lavorazione = Data_Inserimento
                Agronica_Log_Agenda.Data_Ora_RegistrazioneLog = Date.Now
                Agronica_Log_Agenda.Tipo_Operazione = Tipo_Operazione
                Agronica_Log_Agenda.Id_Servizio = 5

                GiasContext.Agronica_Log_Agenda.Add(Agronica_Log_Agenda)
                GiasContext.SaveChanges()

            End If


            Dim Id_CDG_Dettagli As Integer = 0
            Dim Id_Mov As Integer = 0
            Dim Id_Mov_Det As Integer = 0


            '#################################################################################################################
            '######################  Inserimento Dati nelle sole tabelle CDG #################################################
            '#################################################################################################################

            Dim CDG_Testata As CDG_Testata
            Dim Agenda As Agenda
            Dim Movimenti As Movimenti
            Dim Movimenti_Dettagli As Movimenti_dettagli
            Dim Mov_Destinazioni As Mov_Destinazioni
            Dim Mov_Dettagli_Riferimenti As Mov_Dettagli_Riferimenti
            Dim bCheckAgendaDuplicato As Boolean = False

            For Each ArrayP As ArrayList In EFArrayToInsert

                ''Reset Chiavi
                Id_CDG = 0
                Id_CDG_Dettagli = 0
                Id_Mov_Det = 0

                For Each obj In ArrayP

                    If obj.GetType() Is GetType(Agenda) Then

                        Agenda = obj

                        If Id_Agenda_CDG = 0 Then

                            'Richiedo un nuovo id sequenza
                            Id_Agenda_CDG = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                                         "Agenda", 0, 2000000000, objParametri)

                        End If

                        Agenda.Id_Agenda = Id_Agenda_CDG


                        GiasContext.Agenda.Add(Agenda)
                        GiasContext.SaveChanges()


                        'Agronica_Log_Agenda
                        Dim Agronica_Log_Agenda As New Agronica_Log_Agenda
                        Agronica_Log_Agenda.SuperUser = objParametri.PivaSuperUser
                        Agronica_Log_Agenda.Piva = Piva
                        Agronica_Log_Agenda.Sa_Cod = 0
                        Agronica_Log_Agenda.Id_Agenda = Id_Agenda_CDG
                        Agronica_Log_Agenda.Utente = objParametri.UsernameOperazione
                        Agronica_Log_Agenda.Lav_Cod = LAVCOD_COSTI_CDG
                        Agronica_Log_Agenda.Des_lib = Des_Lib
                        Agronica_Log_Agenda.Data_Ora_Lavorazione = Data_Inserimento
                        Agronica_Log_Agenda.Data_Ora_RegistrazioneLog = Date.Now
                        Agronica_Log_Agenda.Tipo_Operazione = 1
                        Agronica_Log_Agenda.Id_Servizio = 5
                        GiasContext.Agronica_Log_Agenda.Add(Agronica_Log_Agenda)
                        GiasContext.SaveChanges()

                    End If


                    If obj.GetType() Is GetType(Movimenti) Then

                        Movimenti = obj

                        'Richiedo un nuovo id sequenza
                        Id_Mov = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                                   "Movimenti", 0, 2000000000, objParametri)

                        Movimenti.Id_Agenda = Id_Agenda_CDG
                        Movimenti.Id_Mov = Id_Mov

                        GiasContext.Movimenti.Add(Movimenti)
                        GiasContext.SaveChanges()

                    End If


                    If obj.GetType() Is GetType(Mov_Dettagli_Riferimenti) Then

                        Mov_Dettagli_Riferimenti = obj

                        Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Id_Agenda_CDG

                        GiasContext.Mov_Dettagli_Riferimenti.Add(Mov_Dettagli_Riferimenti)

                        GiasContext.SaveChanges()

                    End If


                    If obj.GetType() Is GetType(Movimenti_dettagli) Then

                        Movimenti_Dettagli = obj


                        'Richiedo un nuovo id sequenza
                        Id_Mov_Det = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "Movimenti_Dettagli", 0, 2000000000, objParametri)

                        Movimenti_Dettagli.Id_Agenda = Id_Agenda_CDG
                        Movimenti_Dettagli.Id_Mov = Id_Mov
                        Movimenti_Dettagli.Id_Mov_Det = Id_Mov_Det

                        GiasContext.Movimenti_dettagli.Add(Movimenti_Dettagli)
                        GiasContext.SaveChanges()

                    End If


                    If obj.GetType() Is GetType(Mov_Destinazioni) Then

                        Mov_Destinazioni = obj

                        Mov_Destinazioni.Id_Agenda = Id_Agenda_CDG
                        Mov_Destinazioni.Id_Mov = Id_Mov
                        Mov_Destinazioni.Id_Mov_Det = Id_Mov_Det

                        GiasContext.Mov_Destinazioni.Add(Mov_Destinazioni)
                        GiasContext.SaveChanges()

                    End If


                    If obj.GetType() Is GetType(CDG_Testata) Then

                        CDG_Testata = obj


                        'Richiedo un nuovo id sequenza
                        Id_CDG = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                                    "CDG_Testata", 0, 2000000000, objParametri)


                        CDG_Testata.Id_CDG = Id_CDG
                        CDG_Testata.Id_Agenda = Id_Agenda_CDG
                        CDG_Testata.Id_Mov = Id_Mov
                        CDG_Testata.Id_Mov_Det = Id_Mov_Det

                        Id_Mov_Det = 0


                        For Each DettaglioGriglia As CDG_Dettagli In CDG_Testata.CDG_Dettagli


                            'Richiedo un nuovo id sequenza
                            Id_CDG_Dettagli = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                                   "CDG_Dettagli", 0, 2000000000, objParametri)


                            ' Senza questo SaveChanges se non esiste l'ID_Sequenza non riesce ad aggiornare
                            ' il numeratore per i records successivi al primo
                            GiasContext.SaveChanges()

                            DettaglioGriglia.Id_CDG = Id_CDG
                            DettaglioGriglia.Id_CDG_Dettagli = Id_CDG_Dettagli

                        Next

                        GiasContext.CDG_Testata.Add(CDG_Testata)
                        GiasContext.SaveChanges()

                    End If
                Next
            Next

            Id_Agenda_New = Id_Agenda_CDG


        Catch ex As Exception

            Id_Agenda_New = -1
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

        Return Id_Agenda_New

    End Function





    Public Function Aggiorna_CDG_Split(ByVal Piva As String,
                                       ByVal EFArrayToInsert As ArrayList,
                                       ByVal htAgenda As Hashtable,
                                       ByVal htAgenda_CDG As Hashtable,
                                       ByVal htCDG_Testata As Hashtable,
                                       ByVal htCDG_Dettagli_Delete As Hashtable,
                                       ByVal Progetto_Cod_Padre As Integer,
                                       ByVal Data_Split As Date,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As Integer

        Dim ObjSequenze As New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True
        Dim successTestata = True
        Dim successComplessivo As Boolean = True

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.CDG_DAL_W.Aggiorna_CDG_Split()"
        Dim messaggioErrore As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim Dummy As Integer
        Dim Id_Agenda As Integer
        Dim Id_CDG As Integer
        Dim Id_CDG_Dettagli As Integer
        Dim Lav_Cod As Integer

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                '#################################################################################################################
                '###########################  Aggiornamento Agenda.Split #########################################################
                '#################################################################################################################

                For Each obj As DictionaryEntry In htAgenda_CDG

                    Id_Agenda = obj.Value.Id_Agenda

                    Dim ElencoAgenda = (From CI In GiasContext.Agenda
                                        Where CI.PIVA.Equals(Piva) AndAlso
                                              CI.Id_Agenda = Id_Agenda
                                        Select CI).Distinct().ToList()


                    For Each Agenda_Modify As Agenda In ElencoAgenda
                        Agenda_Modify.Split = 1
                        Agenda_Modify.Username_Modifica = objParametri.UsernameOperazione
                        Agenda_Modify.Data_Modifica = Date.Now
                        Agenda_Modify.des_lib = obj.Value.Des_Lib
                        Agenda_Modify.Validita_Inizio = Data_Split
                        Agenda_Modify.Blocco_Data = Data_Split
                    Next

                    GiasContext.SaveChanges()

                    'Nota: non toccare la tabella movimenti: la data_movimento non deve cambiare


                    'Agronica_Log_Agenda
                    Dim Agronica_Log_Agenda As New Agronica_Log_Agenda
                    Agronica_Log_Agenda.SuperUser = objParametri.PivaSuperUser
                    Agronica_Log_Agenda.Piva = Piva
                    Agronica_Log_Agenda.Sa_Cod = 0
                    Agronica_Log_Agenda.Id_Agenda = Id_Agenda
                    Agronica_Log_Agenda.Utente = objParametri.UsernameOperazione
                    Agronica_Log_Agenda.Lav_Cod = LAVCOD_COSTI_CDG
                    Agronica_Log_Agenda.Des_lib = obj.Value.Des_Lib
                    Agronica_Log_Agenda.Data_Ora_Lavorazione = obj.Value.Data_Operazione
                    Agronica_Log_Agenda.Data_Ora_RegistrazioneLog = Date.Now
                    Agronica_Log_Agenda.Tipo_Operazione = 2
                    Agronica_Log_Agenda.Id_Servizio = 5
                    GiasContext.Agronica_Log_Agenda.Add(Agronica_Log_Agenda)
                    GiasContext.SaveChanges()

                Next


                '#################################################################################################################
                '###########################  Aggiornamento Log ##################################################################
                '#################################################################################################################

                For Each obj As DictionaryEntry In htAgenda

                    Id_Agenda = obj.Value.Id_Agenda
                    Lav_Cod = obj.Value.Lav_Cod


                    'Controllo che l'Id_Agenda non sia già presente nel log di Agenda                    
                    If Not htAgenda_CDG.ContainsKey(Id_Agenda) Then

                        'Agronica_Log_Agenda
                        Dim Agronica_Log_Agenda As New Agronica_Log_Agenda
                        Agronica_Log_Agenda.SuperUser = objParametri.PivaSuperUser
                        Agronica_Log_Agenda.Piva = Piva
                        Agronica_Log_Agenda.Sa_Cod = 0
                        Agronica_Log_Agenda.Id_Agenda = Id_Agenda
                        Agronica_Log_Agenda.Utente = objParametri.UsernameOperazione
                        Agronica_Log_Agenda.Lav_Cod = Lav_Cod
                        Agronica_Log_Agenda.Des_lib = obj.Value.Des_Lib
                        Agronica_Log_Agenda.Data_Ora_Lavorazione = obj.Value.Data_Operazione
                        Agronica_Log_Agenda.Data_Ora_RegistrazioneLog = Date.Now
                        Agronica_Log_Agenda.Tipo_Operazione = 2
                        Agronica_Log_Agenda.Id_Servizio = 5
                        GiasContext.Agronica_Log_Agenda.Add(Agronica_Log_Agenda)
                        GiasContext.SaveChanges()

                    End If



                Next

                For Each obj As DictionaryEntry In htCDG_Testata

                    Id_CDG = obj.Value.Id_CDG


                    'Modifica Username e Data Tabella CDG_Testata
                    Dim ElencoTestata = (From CI In GiasContext.CDG_Testata
                                         Where CI.Piva.Equals(Piva) AndAlso
                                               CI.Id_CDG = Id_CDG
                                         Select CI).Distinct().ToList()


                    For Each CDG_Testata_Modify As CDG_Testata In ElencoTestata
                        CDG_Testata_Modify.Data_Inserimento = Data_Split
                        CDG_Testata_Modify.Username_Modifica = objParametri.UsernameOperazione
                        CDG_Testata_Modify.Data_Modifica = Date.Now
                    Next

                    GiasContext.SaveChanges()

                    'Cancellazione Dettagli
                    Dim ElencoCDG_Dettagli = (From CI In GiasContext.CDG_Dettagli
                                              Where CI.Piva.Equals(Piva) AndAlso
                                                    CI.Id_CDG = Id_CDG AndAlso
                                                    CI.Progetto_Cod = Progetto_Cod_Padre
                                              Select CI).Distinct().ToList()

                    For Each CDG_Dettaglio_Delete As CDG_Dettagli In ElencoCDG_Dettagli
                        GiasContext.CDG_Dettagli.Attach(CDG_Dettaglio_Delete)
                        GiasContext.CDG_Dettagli.Remove(CDG_Dettaglio_Delete)
                    Next

                    GiasContext.SaveChanges()

                Next


                'Cancellazione Dettagli (Es Duplicati)
                For Each obj As DictionaryEntry In htCDG_Dettagli_Delete

                    Id_CDG = obj.Value.Id_CDG
                    Id_CDG_Dettagli = obj.Value.Id_CDG_Dettagli

                    'Cancellazione Dettagli Padre
                    Dim ElencoCDG_Dettagli = (From CI In GiasContext.CDG_Dettagli
                                              Where CI.Piva.Equals(Piva) AndAlso
                                                    CI.Id_CDG = Id_CDG AndAlso
                                                    CI.Id_CDG_Dettagli = Id_CDG_Dettagli
                                              Select CI).Distinct().ToList()

                    For Each CDG_Dettaglio_Delete As CDG_Dettagli In ElencoCDG_Dettagli
                        GiasContext.CDG_Dettagli.Attach(CDG_Dettaglio_Delete)
                        GiasContext.CDG_Dettagli.Remove(CDG_Dettaglio_Delete)
                    Next

                    GiasContext.SaveChanges()

                Next







                '#################################################################################################################
                '######################  Inserimento Dati nelle sole tabelle CDG #################################################
                '#################################################################################################################

                Dim CDG_Dettagli As CDG_Dettagli

                For Each ArrayP As ArrayList In EFArrayToInsert

                    'Reset Chiavi                   
                    Id_CDG_Dettagli = 0


                    For Each obj In ArrayP

                        If obj.GetType() Is GetType(CDG_Dettagli) Then

                            CDG_Dettagli = obj

                            'Richiedo un nuovo id sequenza
                            Id_CDG_Dettagli = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                                       "CDG_Dettagli", 0, 2000000000, objParametri)

                            CDG_Dettagli.Id_CDG_Dettagli = Id_CDG_Dettagli

                            GiasContext.CDG_Dettagli.Add(CDG_Dettagli)
                            GiasContext.SaveChanges()

                            ' Senza questo SaveChanges se non esiste l'ID_Sequenza non riesce ad aggiornare
                            ' il numeratore per i records successivi al primo
                            GiasContext.SaveChanges()

                        End If

                    Next

                Next

                Dummy = 1


            End Using
            'End Using
        Catch ex As Exception
            Dummy = -1
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Dummy

    End Function



    Public Function Cancella_Agenda(ByVal Modalita_Imputazione As Integer,
                                    ByVal Piva As String,
                                    ByVal Id_Agenda_CDG As Integer,
                                    ByVal Des_Lib As String,
                                    ByVal Data_Inserimento As DateTime,
                                    ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing
                                    ) As Integer

        Dim ObjSequenze As New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.CDG_DAL_W.Cancella_Agenda()"
        Dim messaggioErrore As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        'Dim gefutils AS New Gias_EF_Utility
        'Dim EFConnString AS String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim Dummy As Integer

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        Try

            Dummy = 0

            Select Case Modalita_Imputazione

                Case 2 'Scarico Tempi

                    'Nn esistono altre tabella ad eccezione di agenda

                Case Else  'QDC, Standalone

                    Dim ElencoMov_Destinazioni = (From CI In GiasContext.Mov_Destinazioni
                                                  Join CDGTestata In GiasContext.CDG_Testata
                                                              On CDGTestata.Piva Equals CI.Piva And
                                                             CDGTestata.Id_Agenda Equals CI.Id_Agenda And
                                                             CDGTestata.Id_Mov Equals CI.Id_Mov And
                                                             CDGTestata.Id_Mov_Det Equals CI.Id_Mov_Det
                                                  Where CI.Piva.Equals(Piva) AndAlso
                                                            CI.Id_Agenda = Id_Agenda_CDG
                                                  Select CI).Distinct().ToList()


                    For Each Mov_Destinazioni_Delete As Mov_Destinazioni In ElencoMov_Destinazioni
                        GiasContext.Mov_Destinazioni.Attach(Mov_Destinazioni_Delete)
                        GiasContext.Mov_Destinazioni.Remove(Mov_Destinazioni_Delete)
                    Next

                    GiasContext.SaveChanges()





                    Dim ElencoMovimenti_Dettagli = (From CI In GiasContext.Movimenti_dettagli
                                                    Join CDGTestata In GiasContext.CDG_Testata
                                                          On CDGTestata.Piva Equals CI.PIVA And
                                                             CDGTestata.Id_Agenda Equals CI.Id_Agenda And
                                                             CDGTestata.Id_Mov Equals CI.Id_Mov And
                                                             CDGTestata.Id_Mov_Det Equals CI.Id_Mov_Det
                                                    Where CI.PIVA.Equals(Piva) AndAlso
                                                              CI.Id_Agenda = Id_Agenda_CDG
                                                    Select CI).Distinct().ToList()


                    For Each Movimenti_Dettagli_Delete As Movimenti_dettagli In ElencoMovimenti_Dettagli
                        GiasContext.Movimenti_dettagli.Attach(Movimenti_Dettagli_Delete)
                        GiasContext.Movimenti_dettagli.Remove(Movimenti_Dettagli_Delete)
                    Next

                    GiasContext.SaveChanges()

                    Dim ElencoAgenda_Riferimenti = (From CI In GiasContext.Mov_Dettagli_Riferimenti
                                                    Where CI.Piva_Rif.Equals(Piva) AndAlso
                                                          CI.Id_Agenda_Rif = Id_Agenda_CDG
                                                    Select CI).Distinct().ToList()

                    For Each Riferimenti_Delete As Mov_Dettagli_Riferimenti In ElencoAgenda_Riferimenti
                        GiasContext.Mov_Dettagli_Riferimenti.Attach(Riferimenti_Delete)
                        GiasContext.Mov_Dettagli_Riferimenti.Remove(Riferimenti_Delete)
                    Next

                    GiasContext.SaveChanges()


            End Select


            'Cancellazione Movimento
            Dim ElencoMovimenti = (From CI In GiasContext.Movimenti
                                   Where CI.PIVA.Equals(Piva) AndAlso
                                             CI.Id_Agenda = Id_Agenda_CDG
                                   Select CI).Distinct().ToList()

            For Each Movimenti_Delete As Movimenti In ElencoMovimenti
                GiasContext.Movimenti.Attach(Movimenti_Delete)
                GiasContext.Movimenti.Remove(Movimenti_Delete)
            Next

            GiasContext.SaveChanges()


            'Cancellazione Agenda
            Dim ElencoAgenda = (From CI In GiasContext.Agenda
                                Where CI.PIVA.Equals(Piva) AndAlso
                                          CI.Id_Agenda = Id_Agenda_CDG
                                Select CI).Distinct().ToList()


            For Each Agenda_Delete As Agenda In ElencoAgenda
                GiasContext.Agenda.Attach(Agenda_Delete)
                GiasContext.Agenda.Remove(Agenda_Delete)
            Next

            GiasContext.SaveChanges()


        Catch ex As Exception
            Dummy = -1
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            If bCloseContext Then
                GiasContext.Dispose()
                GiasContext = Nothing
            End If

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
            GiasContext = Nothing
        End If

        Return Dummy

    End Function


    Public Function ModificaCDG_Testata(ByVal Piva As String,
                                        ByVal Id_Agenda_CDG As Integer,
                                        ByVal Id_CDG As Integer,
                                        ByVal Numero_CDC As Integer,
                                        ByVal bDettagli_Modify As Boolean,
                                        ByVal Des_Lib As String,
                                        ByVal Data_Inserimento As DateTime,
                                        ByVal Qta As Double,
                                        ByVal Prezzo_Unitario As Double,
                                        ByVal Data_Ora_Inizio As DateTime,
                                        ByVal Data_Ora_Fine As DateTime,
                                        ByVal EFArrayToUpdate As ArrayList,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Integer
        'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
        'Optional ByVal isBozza As Integer = 0


        Dim ObjSequenze = New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.CDG_DAL_W.ModificaCDG_Testata()"
        Dim messaggioErrore As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim Dummy As Integer

        Dim StrSQL As New System.Text.StringBuilder
        Dim Err As String = ""
        Dim Id_CDG_Dettagli As Integer

        Try

            'Using scope AS New TransactionScope()

            Dummy = 0

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                'Controllo numero di centri di costo per riga... altrimenti cancellerei tutti i dettagli mentre in inserimento posso inserirne uno alla volta.
                'N.B.  Numero_CDC è >= 2 anche nel caso di impianti poliannuali con spalmatura già effettuata su più distinte
                If Numero_CDC < 2 AndAlso bDettagli_Modify Then

                    'Cancellazione Dettagli
                    Dim ElencoDettagli = (From t In GiasContext.CDG_Dettagli
                                          Where t.Piva_Superuser.Equals(Piva_SuperUser) AndAlso
                                                t.Piva.Equals(Piva) AndAlso
                                                t.Id_CDG = Id_CDG).ToList()


                    For Each CDG_Dettagli_Delete As CDG_Dettagli In ElencoDettagli
                        GiasContext.CDG_Dettagli.Attach(CDG_Dettagli_Delete)
                        GiasContext.CDG_Dettagli.Remove(CDG_Dettagli_Delete)
                    Next


                    GiasContext.SaveChanges()

                End If


                'Inserimento Dettagli
                Dim CDG_Testata As CDG_Testata

                For Each ArrayP As ArrayList In EFArrayToUpdate

                    ''Reset Chiavi
                    Id_CDG_Dettagli = 0

                    For Each obj In ArrayP

                        If obj.GetType() Is GetType(CDG_Testata) Then

                            CDG_Testata = obj

                            'GiasContext.CDG_Testata.Attach(obj)
                            'GiasContext.ObjectStateManager.ChangeObjectState(obj, EntityState.Unchanged)

                            'Vedi commento sopra.
                            If Numero_CDC < 2 AndAlso bDettagli_Modify Then

                                For Each DettaglioGriglia As CDG_Dettagli In CDG_Testata.CDG_Dettagli

                                    'Richiedo un nuovo id sequenza
                                    Id_CDG_Dettagli = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                           "CDG_Dettagli", 0, 2000000000, objParametri)

                                    GiasContext.SaveChanges()

                                    'DettaglioGriglia.Id_CDG = Id_CDG
                                    DettaglioGriglia.Id_CDG_Dettagli = Id_CDG_Dettagli
                                    CDG_Testata.CDG_Dettagli.Add(DettaglioGriglia)
                                    GiasContext.CDG_Dettagli.Attach(DettaglioGriglia)
                                    GiasContext.Entry(DettaglioGriglia).State = EntityState.Added

                                    CDG_Testata.Data_Ora_Inizio = Data_Ora_Inizio
                                    CDG_Testata.Data_Ora_Fine = Data_Ora_Fine
                                    CDG_Testata.Qta = Qta
                                    CDG_Testata.Valore_Totale = Qta * Prezzo_Unitario
                                    'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
                                    'CDG_Testata.Bozza = isBozza

                                    CDG_Testata.Username_Modifica = UtilityProvider.Agro_SQL_SaveText_NULL(objParametri.UsernameOperazione)
                                    CDG_Testata.Data_Modifica = Date.Now
                                    GiasContext.Entry(CDG_Testata).State = EntityState.Modified
                                Next

                            Else

                                Dim testata = (From t In GiasContext.CDG_Testata
                                               Where t.Id_CDG = CDG_Testata.Id_CDG).FirstOrDefault()

                                If testata Is Nothing Then
                                    Throw New Exception("Testata non trovata")
                                End If

                                testata.Data_Ora_Inizio = Data_Ora_Inizio
                                testata.Data_Ora_Fine = Data_Ora_Fine
                                testata.Qta = Qta
                                testata.Valore_Totale = Qta * Prezzo_Unitario
                                'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
                                'testata.Bozza = isBozza

                                testata.Username_Modifica = UtilityProvider.Agro_SQL_SaveText_NULL(objParametri.UsernameOperazione)
                                testata.Data_Modifica = Date.Now

                                GiasContext.CDG_Testata.Attach(testata)
                                GiasContext.Entry(testata).State = EntityState.Modified

                            End If

                            GiasContext.SaveChanges()

                        End If

                    Next
                Next


                'Modifica Puntuale Testata

                'StrSQL.Length = 0
                'StrSQL.AppendLine(" UPDATE CDG_Testata ")
                'StrSQL.AppendLine(" SET ")
                'StrSQL.AppendLine("      Data_Ora_Inizio = " & Agro_SQL_SaveDateTime(Data_Ora_Inizio) & " ")
                'StrSQL.AppendLine("     ,Data_Ora_Fine = " & Agro_SQL_SaveDateTime(Data_Ora_Fine) & " ")
                'StrSQL.AppendLine("     ,Qta = " & Agro_SQL_SaveNum(Qta) & " ")
                'StrSQL.AppendLine("     ,Username_Modifica = " & Agro_SQL_SaveText_NULL(objParametri.UsernameOperazione))

                'StrSQL.AppendLine(" Where Piva_Superuser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                'StrSQL.AppendLine(" AND Piva = " & Agro_SQL_SaveText_NULL(Piva))
                'StrSQL.AppendLine(" AND Id_CDG = " & Agro_SQL_SaveNum(Id_CDG))


                '--------------------------------------------------------------------------
                'Dim esito AS Boolean = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
                '--------------------------------------------------------------------------




                'Agronica_Log_Agenda
                Dim Agronica_Log_Agenda As New Agronica_Log_Agenda


                Agronica_Log_Agenda.SuperUser = objParametri.PivaSuperUser
                Agronica_Log_Agenda.Piva = Piva
                Agronica_Log_Agenda.Sa_Cod = 0
                Agronica_Log_Agenda.Id_Agenda = Id_Agenda_CDG
                Agronica_Log_Agenda.Utente = objParametri.UsernameOperazione
                Agronica_Log_Agenda.Lav_Cod = LAVCOD_COSTI_CDG
                Agronica_Log_Agenda.Des_lib = Des_Lib
                Agronica_Log_Agenda.Data_Ora_Lavorazione = Data_Inserimento
                Agronica_Log_Agenda.Data_Ora_RegistrazioneLog = Date.Now
                Agronica_Log_Agenda.Tipo_Operazione = 2
                Agronica_Log_Agenda.Id_Servizio = 5


                GiasContext.Agronica_Log_Agenda.Add(Agronica_Log_Agenda)
                GiasContext.SaveChanges()



                'End Using
            End Using
        Catch ex As Exception

            Dummy = -1
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return Dummy

    End Function

    Public Function CancellaCDG_Testata(ByVal piva As String, ByVal id_CDG As Integer, ByRef objParametri As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.CDG_DAL_W.CancellaCDG_Testata()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim Err As String = ""

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE CDG_Testata ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = " & Agro_SQL_SaveText_NULL(objParametri.UsernameOperazione))
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM CDG_Testata ")
                StrSQL.AppendLine(" WHERE  1=1 ")

            End If

            StrSQL.AppendLine(" AND Piva_Superuser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine(" AND id_CDG = " & Agro_SQL_SaveNum(id_CDG))
            If piva <> "" Then
                StrSQL.AppendLine(" AND piva = " & Agro_SQL_SaveText_NULL(piva))
            End If

            '--------------------------------------------------------------------------
            Dim esito As Boolean = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Err = MessaggioErrore
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Err

    End Function

    Public Function CancellaCDG_Dettagli(ByVal piva As String, ByVal id_CDG As Integer, ByRef objParametri As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.CDG_DAL_W.CancellaCDG_Dettagli()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim Err As String = ""

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE CDG_Dettagli ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = " & Agro_SQL_SaveText_NULL(objParametri.UsernameOperazione))
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM CDG_Dettagli ")
                StrSQL.AppendLine(" WHERE  1=1 ")

            End If

            StrSQL.AppendLine(" AND Piva_Superuser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine(" AND id_CDG = " & Agro_SQL_SaveNum(id_CDG))
            If piva <> "" Then
                StrSQL.AppendLine(" AND piva = " & Agro_SQL_SaveText_NULL(piva))
            End If

            '--------------------------------------------------------------------------
            Dim esito As Boolean = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Err = MessaggioErrore
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Err
    End Function



    Public Function AllineaDate(ByVal piva As String, ByVal id_Agenda As Integer, ByVal Id_Agenda_CDG As Integer, ByRef objParametri As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.CDG_DAL_W.AllineaDate()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim Err As String = ""

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Mov_Dettagli_Riferimenti ")
            StrSQL.AppendLine(" SET ")
            StrSQL.AppendLine("      Data_Creazione = (Select Data_Modifica from Agenda Where Piva = '" & Agro_SQL_SaveText(piva) & "' And Id_Agenda = " & id_Agenda & ")" & " ")
            StrSQL.AppendLine("      Where Piva = '" & Agro_SQL_SaveText(piva) & "' And Id_Agenda = " & id_Agenda & " And cau_mov = '7400' and Id_Agenda_Rif = " & Id_Agenda_CDG & " ")



            '--------------------------------------------------------------------------
            Dim esito As Boolean = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Err = MessaggioErrore
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Err

    End Function



    Public Function AllineaDateRiferimento(ByVal piva As String, ByVal id_Agenda As Integer, ByVal Id_Agenda_CDG As Integer, ByVal Data As DateTime, ByRef objParametri As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.CDG_DAL_W.AllineaDate()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim Err As String = ""

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Mov_Dettagli_Riferimenti ")
            StrSQL.AppendLine(" SET ")
            StrSQL.AppendLine("      Data_Creazione = " & Agro_SQL_SaveDate(Data) & " ")
            StrSQL.AppendLine("      Where Piva = '" & Agro_SQL_SaveText(piva) & "' And Id_Agenda = " & id_Agenda & " And cau_mov = '7400' ")

            If Id_Agenda_CDG <> 0 Then
                StrSQL.AppendLine(" And Id_Agenda_Rif = " & Id_Agenda_CDG & " ")
            End If




            '--------------------------------------------------------------------------
            Dim esito As Boolean = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Err = MessaggioErrore
            Throw New Exception("[" & NomeRoutine & "] :    " & MessaggioErrore)
        End Try

        Return Err

    End Function


    Public Function AllineaId_Mov(ByVal piva As String, ByVal Id_Agenda_CDG As Integer, ByRef objParametri As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.CDG_DAL_W.AllineaId_Mov()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim Err As String = ""

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE CDG_Testata ")
            StrSQL.AppendLine(" Set Id_Mov = (select Id_Mov From movimenti Where Piva = '" & Agro_SQL_SaveText(piva) & "' And Id_Agenda = " & Id_Agenda_CDG & " And Cau_Mov = '7350') ")
            StrSQL.AppendLine("     Where Piva = '" & Agro_SQL_SaveText(piva) & "' And Id_Agenda = " & Id_Agenda_CDG & " And Id_Mov = 0 ")

            '--------------------------------------------------------------------------
            Dim esito As Boolean = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Err = MessaggioErrore
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Err

    End Function




    Public Function Modifica_CDG_Testata_Split_Semina_Trapianto(ByVal piva As String, ByVal Id_Agenda_CDG_Old As Integer, ByVal Id_CDG_Old As Integer, ByVal Qta_New As Decimal, ByVal Data_Ora_Inizio_New As DateTime, ByVal Data_Ora_Fine_New As DateTime, ByVal Valore_Totale_New As Decimal, ByRef objParametri As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.CDG_DAL_W.Split_Semina_Trapianto_Switch_Keys()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim Err As String = ""

        Try
            ''---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE CDG_Testata ")
            StrSQL.AppendLine(" Set Qta = " & Agro_SQL_SaveNum(Qta_New) & " ")
            StrSQL.AppendLine(" , Data_Ora_Inizio = " & Agro_SQL_SaveDateTime(Data_Ora_Inizio_New) & " ")
            StrSQL.AppendLine(" , Data_Ora_Fine = " & Agro_SQL_SaveDateTime(Data_Ora_Fine_New) & " ")
            StrSQL.AppendLine(" , Valore_Totale = " & Agro_SQL_SaveNum(Valore_Totale_New) & " ")
            StrSQL.AppendLine(" Where Budget = 0 And Id_CDG = " & Id_CDG_Old & " And ID_Agenda = " & Id_Agenda_CDG_Old & " ")

            '--------------------------------------------------------------------------
            Dim esito As Boolean = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Err = MessaggioErrore
            Throw New Exception("[" & NomeRoutine & "] :   " & MessaggioErrore)
        End Try

        Return Err

    End Function



    Public Function Modifica_Scarico_Split_Semina_Trapianto(ByVal piva As String, ByVal Id_Agenda_CDG_Old As Integer, ByVal Id_Mov_Det As Integer, ByVal Qta_New As Decimal, ByRef objParametri As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.CDG_DAL_W.Modifica_Scarico_Split_Semina_Trapianto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim Err As String = ""

        Try
            ''---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Movimenti_Dettagli ")
            StrSQL.AppendLine(" Set Qta = " & Agro_SQL_SaveNum(Qta_New) & " ")
            StrSQL.AppendLine(" Where Id_Mov_Det = " & Id_Mov_Det & " And ID_Agenda = " & Id_Agenda_CDG_Old & " ")

            '--------------------------------------------------------------------------
            Dim esito As Boolean = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ''---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Mov_Destinazioni ")
            StrSQL.AppendLine(" Set Qta = " & CDbl(Qta_New) & " ")
            StrSQL.AppendLine(" Where Id_Mov_Det = " & Id_Mov_Det & " And ID_Agenda = " & Id_Agenda_CDG_Old & " ")

            '--------------------------------------------------------------------------
            esito = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Err = MessaggioErrore
            Throw New Exception("[" & NomeRoutine & "] :   " & MessaggioErrore)
        End Try

        Return Err

    End Function



    Public Function Modifica_Stato_Export_2(ByVal piva As String, ByVal Raccoglitore_Cod As Integer, ByVal Id_Agenda As Integer, ByVal Stato_Export_2 As Integer, ByRef objParametri As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.CDG_DAL_W.Modifica_Stato_Export_2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim Err As String = ""

        Try

            'Controllo coerenza
            If Raccoglitore_Cod <> 0 OrElse Id_Agenda <> 0 Then

                ''---------------------------------------------
                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE Agenda ")
                StrSQL.AppendLine(" Set Stato_Export_2 = " & Agro_SQL_SaveNum(Stato_Export_2) & " ")
                StrSQL.AppendLine(" Where Piva = '" & Agro_SQL_SaveText(piva) & "'")

                If Raccoglitore_Cod <> 0 Then
                    StrSQL.AppendLine(" And Raccoglitore_Cod = " & Raccoglitore_Cod & " ")
                Else
                    StrSQL.AppendLine(" And Id_Agenda = " & Id_Agenda & " ")
                End If

                '--------------------------------------------------------------------------
                Dim esito As Boolean = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Err = MessaggioErrore
            Throw New Exception("[" & NomeRoutine & "] :   " & MessaggioErrore)
        End Try

        Return Err

    End Function


    Public Function Batch_Delete(ByVal piva As String, ByVal strFiltro As String, ByVal bForzato As Boolean, ByVal ModalitaZoo As Integer, ByRef objParametri As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.CDG_DAL_W.Batch_Delete()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim StrFiltroSQL As New System.Text.StringBuilder
        Dim Err As String = ""
        Dim esito As Boolean
        Dim DT As New DataTable
        Dim strElenco As String = ""
        'Dim iCount As Integer = 0
        Try

            StrFiltroSQL.Length = 0
            StrFiltroSQL.AppendLine(" (Select Id_Agenda_Rif, Agenda.Lav_cod from Agenda (NOLOCK) Inner Join mov_dettagli_Riferimenti (NOLOCK) ")
            StrFiltroSQL.AppendLine(" On Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            StrFiltroSQL.AppendLine(" And Lav_Cod_Rif = 4500 ")
            StrFiltroSQL.AppendLine(" And Agenda.Piva = Mov_Dettagli_Riferimenti.Piva ")
            StrFiltroSQL.AppendLine(" And Agenda.id_agenda = Mov_Dettagli_Riferimenti.Id_Agenda ")

            If Not bForzato Then
                StrFiltroSQL.AppendLine(" And Agenda.Data_Modifica > Mov_Dettagli_Riferimenti.Data_Creazione And Abs(DateDiff(Second, Agenda.Data_Modifica, Mov_Dettagli_Riferimenti.Data_Creazione)) > 1 ")
            End If


            If strFiltro <> "" Then
                StrFiltroSQL.Append(" AND (" & Agro_SQL_Save_xFiltroAggiuntivo(strFiltro) & ")) ")
            Else
                StrFiltroSQL.Append(") ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrFiltroSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then

                Dim dr_operazioni() As DataRow
                Dim tabImputazione As String
                Dim filtro As String = ""

                For i = 1 To 2
                    Select Case i
                        Case 1
                            filtro = "(Lav_cod > 0 And Lav_cod < 1000)"
                            If ModalitaZoo = 0 Then
                                'in modalita Aboca deve inserire anche la nascita e l'acquisto
                                filtro += "OR (Lav_cod = 3000 Or Lav_cod = 3034)"
                            End If
                            dr_operazioni = DT.Select(filtro)
                            tabImputazione = "EREDITA"
                        Case 2
                            filtro = "(Lav_cod > 2999 And Lav_cod < 4000)"
                            If ModalitaZoo = 0 Then
                                'in modalita Aboca deve escludere la nascita e l'acquisto perchè già fatte con EREDITA
                                filtro += "AND (Lav_cod <> 3000 AND Lav_Cod <> 3034)"
                            End If
                            dr_operazioni = DT.Select(filtro)
                            tabImputazione = "LIBERA"
                    End Select

                    If dr_operazioni.Length > 0 Then

                        For Each dr In dr_operazioni
                            strElenco = strElenco & IIf(Trim(strElenco) = "", "", ",") & dr("Id_Agenda_Rif")
                            'iCount = iCount + 1

                            ''Max 100
                            'If iCount > 100 Then
                            '    Exit For
                            'End If

                        Next

                        'Formattazione
                        strElenco = "(" & strElenco & ")"

                        'Cancellazione Dettagli
                        StrSQL.Length = 0
                        StrSQL.AppendLine(" Delete From Cdg_Dettagli Where Id_Cdg in (select Id_Cdg From CDG_Testata Where Tab_Imputazione = '" & Agro_SQL_SaveText(tabImputazione) & "' And Budget = 0 And Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                        StrSQL.AppendLine(" And Id_Agenda In ")
                        StrSQL.Append(Agro_SQL_Save_Clausola_IN(strElenco))
                        StrSQL.Append(")")

                        '--------------------------------------------------------------------------
                        esito = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                        '--------------------------------------------------------------------------


                        'Cancellazione Testata
                        StrSQL.Length = 0
                        StrSQL.AppendLine(" Delete From Cdg_Testata Where Tab_Imputazione = '" & Agro_SQL_SaveText(tabImputazione) & "' And Budget = 0 And Piva = '" & Agro_SQL_SaveText(piva) & "' And Id_Agenda In ")
                        StrSQL.Append(Agro_SQL_Save_Clausola_IN(strElenco))


                        '--------------------------------------------------------------------------
                        esito = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                        '--------------------------------------------------------------------------

                    End If

                    strElenco = ""

                Next

            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Err = MessaggioErrore
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Err

    End Function


    Public Function Scrivi_ExportCDGVecchioTipo(ByVal Piva_SuperUser As String,
                                      ByVal piva As String,
                                      ByVal Vecchio_Tipo_Inser_Dati As Integer,
                                      ByVal ArrayToInsMod As JArray,
                                      ByVal DtCdgToDelete As DataTable,
                                      ByVal dataModifica As Date,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreParametri
                                      ) As String


        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_W.Scrivi_ExportCDGVecchioTipo()"
        Dim messaggioErrore As String = ""

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            Utility.VerificaApriTransazione(objParametri_Server, flagConnessione, flagTransazione)

            Dim idsAgendaList = (From a In ArrayToInsMod Select a("Id_Agenda")).ToList()
            Dim idsAgenda As New StringBuilder("(")
            For Each idAgenda In idsAgendaList
                If idsAgenda.ToString <> "(" Then
                    idsAgenda.Append(", ")
                End If
                idsAgenda.Append(CStr(idAgenda))
            Next
            idsAgenda.Append(")")

            If idsAgendaList.Count Then
                Delete_Cdg_ByAgenda(piva, Vecchio_Tipo_Inser_Dati, idsAgenda.ToString(), objParametri_Server)
            End If

            Dim objSequenze As Agro_Sequenze = Nothing
            Dim progressivoTestata As Integer = 0
            Dim progressivoDettagli As Integer = 0
            Dim dt As DataTable

            For Each obj As JObject In ArrayToInsMod

                strSql.Length = 0
                strSql.Append(" Select * From Mov_Dettagli_Riferimenti Where Piva = '" & Agro_SQL_SaveText(piva) & "'")
                strSql.Append(" AND   Lav_Cod_Rif = 4500 ")
                strSql.Append(" AND   Id_Agenda = " & Agro_SQL_SaveNum(obj("Id_Agenda")))
                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri_Server, strSql.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

                If dt.Rows.Count = 0 Then

                    objSequenze = New Agro_Sequenze

                    progressivoTestata = objSequenze.NuovoId_Tabella("CDG_Testata",
                                                                     0, 2000000000,
                                                                      objParametri_Server)

                    progressivoDettagli = objSequenze.NuovoId_Tabella("CDG_Dettagli",
                                                                     0, 2000000000,
                                                                      objParametri_Server)

                    strSql.Length = 0
                    strSql.AppendLine(" Insert INTO CDG_Testata ( ")
                    strSql.AppendLine(" Piva_SuperUser, ")
                    strSql.AppendLine(" piva, ")
                    strSql.AppendLine(" Id_CDG, ")
                    strSql.AppendLine(" Data_Inserimento, ")
                    strSql.AppendLine(" Modalita_Imputazione, ")
                    strSql.AppendLine(" Id_Agenda, ")
                    strSql.AppendLine(" Id_Mov, ")
                    strSql.AppendLine(" Id_Mov_Det, ")
                    strSql.AppendLine(" Mac_Cod, ")
                    strSql.AppendLine(" Cod_RisUm, ")
                    strSql.AppendLine(" Elem_Cod, ")
                    strSql.AppendLine(" Pro_Cod, ")
                    strSql.AppendLine(" Mat_Cod, ")
                    strSql.AppendLine(" Id_Attivita, ")
                    strSql.AppendLine(" Qualifica_Cod, ")
                    strSql.AppendLine(" Tariffa_Cod, ")
                    strSql.AppendLine(" Turno_Cod, ")
                    strSql.AppendLine(" Conto_Cod, ")
                    strSql.AppendLine(" Lotto, ")
                    strSql.AppendLine(" Mezzo, ")
                    strSql.AppendLine(" Udm_Cod, ")
                    strSql.AppendLine(" Prezzo_Unitario, ")
                    strSql.AppendLine(" Qta, ")
                    strSql.AppendLine(" Valore_Totale, ")
                    strSql.AppendLine(" Descrizione, ")
                    strSql.AppendLine(" Tipo_Ripartizione, ")
                    strSql.AppendLine(" Budget, ")
                    strSql.AppendLine(" Costi_Ricavi, ")
                    strSql.AppendLine(" Modalita_Ripartizione, ")
                    strSql.AppendLine(" Vecchio_Tipo_Inser_Dati, ")
                    strSql.AppendLine(" inviato ,")
                    strSql.AppendLine(" datainvio, ")
                    strSql.AppendLine(" Data_Creazione ,")
                    strSql.AppendLine(" Data_Modifica, ")
                    strSql.AppendLine(" Username_Creazione, ")
                    strSql.AppendLine(" Username_Modifica, ")
                    strSql.AppendLine(" Validita_Inizio, ")
                    strSql.AppendLine(" Validita_Fine, ")
                    strSql.AppendLine(" Tipo_Destinazione, ")
                    strSql.AppendLine(" Sa_Cod, ")
                    strSql.AppendLine(" Id_Destinazione, ")
                    strSql.AppendLine(" Flag_Movimento_Campagna, ")
                    strSql.AppendLine(" ModifInAutom, ")
                    strSql.AppendLine(" OrigineApp ")

                    strSql.AppendLine("          ) ")

                    strSql.AppendLine(" VALUES (")
                    strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva_SuperUser) & "'  ")
                    strSql.AppendLine("         , '" & Agro_SQL_SaveText(piva) & "'  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(progressivoTestata) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(obj("Data_Inserimento")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Modalita_Imputazione")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Id_Agenda")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Id_Mov")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Id_Mov_Det")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Mac_Cod")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Cod_RisUm")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Elem_Cod")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Pro_Cod")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Mat_Cod")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Id_Attivita")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Qualifica_Cod")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Tariffa_Cod")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Turno_Cod")) & "  ")
                    strSql.AppendLine("       , " & Agro_SQL_SaveNum(obj("Conto_Cod")) & "  ")
                    strSql.AppendLine("       , '" & Agro_SQL_SaveText(obj("Lotto")) & " ' ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Mezzo")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Udm_Cod")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(Decimal.Parse(obj("Prezzo_Unitario"), Globalization.CultureInfo.CurrentCulture)) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(Decimal.Parse(obj("Qta"), Globalization.CultureInfo.CurrentCulture)) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(Decimal.Parse(obj("Valore_Totale"), Globalization.CultureInfo.CurrentCulture)) & "  ")
                    strSql.AppendLine("          , '" & Agro_SQL_SaveText(obj("Descrizione")) & "'  ")
                    strSql.AppendLine("      ," & Agro_SQL_SaveNum(obj("Tipo_Ripartizione")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Budget")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("CostiORicavi")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Modalita_Ripartizione")) & "  ")
                    strSql.AppendLine("          , " & Agro_SQL_SaveNum(obj("Tipo_Imputazione")) & "  ")
                    strSql.AppendLine("         , 0 ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(dataModifica) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(dataModifica) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(dataModifica) & "  ")
                    strSql.AppendLine("       , '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'  ")
                    strSql.AppendLine("       , '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(AGRODATAFINE) & "  ")
                    strSql.AppendLine("         , 0 ")
                    strSql.AppendLine("         , 0 ")
                    strSql.AppendLine("         , 0 ")
                    strSql.AppendLine("         , 1 ")
                    strSql.AppendLine("         , 0 ")
                    strSql.AppendLine("         , 0 ")
                    strSql.AppendLine(" ) ")

                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
                    '--------------------------------------------------------------------------
                    strSql.Length = 0
                    strSql.AppendLine(" Insert INTO CDG_Dettagli ( ")
                    strSql.AppendLine(" Piva_SuperUser, ")
                    strSql.AppendLine(" piva, ")
                    strSql.AppendLine(" Id_CDG, ")
                    strSql.AppendLine(" Id_CDG_Dettagli, ")
                    strSql.AppendLine(" Sa_Cod, ")
                    strSql.AppendLine(" Appezza, ")
                    strSql.AppendLine(" Id_Reg, ")
                    strSql.AppendLine(" Id_Cod_reg_impianti_codici, ")
                    strSql.AppendLine(" Progetto_Cod, ")
                    strSql.AppendLine(" Id_Imputazione, ")
                    strSql.AppendLine(" Macchine_Cod, ")
                    strSql.AppendLine(" Linea_Cod, ")
                    strSql.AppendLine(" Lotto_Input_Costi, ")
                    strSql.AppendLine(" Valore, ")
                    strSql.AppendLine(" inviato ,")
                    strSql.AppendLine(" datainvio, ")
                    strSql.AppendLine(" Data_Creazione ,")
                    strSql.AppendLine(" Data_Modifica, ")
                    strSql.AppendLine(" Username_Creazione, ")
                    strSql.AppendLine(" Username_Modifica, ")
                    strSql.AppendLine(" Validita_Inizio, ")
                    strSql.AppendLine(" Validita_Fine ")
                    strSql.AppendLine("          ) ")

                    strSql.AppendLine(" VALUES (")

                    strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva_SuperUser) & "'  ")
                    strSql.AppendLine("         ,  '" & Agro_SQL_SaveText(piva) & "'  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(progressivoTestata) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(progressivoDettagli) & "  ")

                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Sa_Cod")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Appezza")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Id_Impianto")) & "  ")


                    strSql.Append(" ,  ISNULL(( SELECT TOP 1 Reg_Impianti_Codici.id_cod  " & vbCrLf)
                    strSql.Append(" From Reg_Impianti_Codici  " & vbCrLf)
                    strSql.Append(" WHERE Reg_Impianti_Codici.PIVA = '" & Agro_SQL_SaveText(piva) & "' " & vbCrLf)
                    strSql.Append(" And   Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(obj("Sa_Cod")) & "  " & vbCrLf)
                    strSql.Append(" And   Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(obj("Appezza")) & "  " & vbCrLf)
                    strSql.Append(" And   Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(obj("Id_Impianto")) & "  " & vbCrLf)
                    strSql.Append(" And   (Reg_Impianti_Codici.Progetto_Cod = 0)  " & vbCrLf)
                    'strSql.Append(" And (Codici_Anagrafe.gruppo = 'TERRENO')  " & vbCrLf)
                    strSql.Append(" And Reg_Impianti_Codici.id_cod > 3000 And Reg_Impianti_Codici.id_cod <= 4000 " & vbCrLf)
                    strSql.Append(" ) , 0)   " & vbCrLf)

                    strSql.Append(" ,  ISNULL(( SELECT TOP 1 Imprese_Progetti.Progetto_Cod  " & vbCrLf)
                    strSql.Append(" From Imprese_Progetti  " & vbCrLf)
                    strSql.Append(" WHERE Imprese_Progetti.PIVA = '" & Agro_SQL_SaveText(piva) & "' " & vbCrLf)
                    strSql.Append(" And   Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(obj("Sa_Cod")) & "  " & vbCrLf)
                    strSql.Append(" And   Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(obj("Appezza")) & "  " & vbCrLf)
                    strSql.Append(" And   Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(obj("Id_Impianto")) & "  " & vbCrLf)
                    strSql.Append(" And   (Imprese_Progetti.validita_inizio >= " & Agro_SQL_SaveDateTime(dataModifica) & ") " & vbCrLf)
                    strSql.Append(" And   (Imprese_Progetti.validita_inizio <= " & Agro_SQL_SaveDateTime(dataModifica) & ") " & vbCrLf)
                    strSql.Append(" ) , 0)   " & vbCrLf)

                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Id_Imputazione")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Macchine_Cod")) & "  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(obj("Linea_Cod")) & "  ")
                    strSql.AppendLine("         , ''  ")
                    strSql.AppendLine("         , " & Agro_SQL_SaveNum(Decimal.Parse(obj("Valore"), Globalization.CultureInfo.CurrentCulture)) & "  ")
                    strSql.AppendLine("         , 0 ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(dataModifica) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(dataModifica) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(dataModifica) & "  ")
                    strSql.AppendLine("       , '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'  ")
                    strSql.AppendLine("       , '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & "  ")
                    strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(AGRODATAFINE) & "  ")
                    strSql.AppendLine(" ) ")

                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
                    '--------------------------------------------------------------------------

                End If

            Next

            'FASE 2 - Agg-to DW
            Dim agg_DW_CDG_Costi_Ricavi As New DW_CDG_Costi_Ricavi_DAL_W

            Dim esito As Boolean = agg_DW_CDG_Costi_Ricavi.InserisciDW_VecchioTipo(
                                        piva, Vecchio_Tipo_Inser_Dati, idsAgenda.ToString,
                                        objParametri_Server, objParametri_Utenti)

            If Not esito Then
                Throw New Exception("Errore durante l'aggiornamento del DW")
            End If

            Utility.VerificaChiudiTransazione(objParametri_Server, flagTransazione)

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri_Server, flagTransazione)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally
            Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)
        End Try

        Return messaggioErrore

    End Function


    Public Function Delete_Cdg_ByAgenda(ByVal piva As String, ByVal Vecchio_Tipo_Inser_Dati As Integer, ByVal idsAgenda As String, ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.CDG_DAL_W.Delete_Cdg()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim esito As Boolean
        Dim DW_CDG_Costi_Ricavi_W As New DW_CDG_Costi_Ricavi_DAL_W
        Try

            'Cancellazione Dettagli
            StrSQL.Length = 0
            StrSQL.AppendLine(" Delete From Cdg_Dettagli Where Id_Cdg in (")
            StrSQL.AppendLine("    Select Id_Cdg From CDG_Testata Where Piva = '" & Agro_SQL_SaveText(piva) & "' And Id_Agenda In " & Agro_SQL_Save_Clausola_IN(idsAgenda) & " ")
            StrSQL.Append("             And Vecchio_Tipo_Inser_Dati  = " & Agro_SQL_SaveNum(Vecchio_Tipo_Inser_Dati) & " ")
            StrSQL.Append(")")

            '--------------------------------------------------------------------------
            esito = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'Cancellazione Testata
            StrSQL.Length = 0
            StrSQL.AppendLine(" Delete From Cdg_Testata Where Piva = '" & Agro_SQL_SaveText(piva) & "' And Id_Agenda In " & Agro_SQL_Save_Clausola_IN(idsAgenda) & "")
            StrSQL.Append(" And Vecchio_Tipo_Inser_Dati  = " & Agro_SQL_SaveNum(Vecchio_Tipo_Inser_Dati) & " ")

            '--------------------------------------------------------------------------
            esito = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'Cancellazione DW
            esito = DW_CDG_Costi_Ricavi_W.Delete_DWCdg_ByAgenda(piva, Vecchio_Tipo_Inser_Dati, idsAgenda, objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            esito = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return esito

    End Function

    Public Function Elimina_Squadra_Attivita(ByVal piva As String, ByVal ID_Squadra As Integer, ByRef objParametri As AgronicaCoreParametri, Optional SistemaOrigine As Integer = -1) As String

        Dim messaggioErrore As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_W.Elimina_Squadra_Attivita()"

        Try

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim Squadra = (From s In GiasContext.SquadreXAttivita
                               Where s.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                                     s.Piva.Equals(piva) AndAlso
                                     s.ID_Squadra = ID_Squadra
                               Select s).FirstOrDefault()

                If Squadra IsNot Nothing Then
                    Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                    Dim riga As String = JsonConvert.SerializeObject(Squadra, a)

                    GiasContext.SquadreXAttivita.Attach(Squadra)
                    GiasContext.SquadreXAttivita.Remove(Squadra)

                    'Scrittura tabella Agronica_Log_Anagrafe
                    Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                    Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Squadre,
                                                                                         Squadra.Piva,
                                                                                         CStr(Squadra.ID_Squadra),
                                                                                         Nothing, Nothing, Nothing, Nothing,
                                                                                         enum_TipoOperazioneDB.Cancellazione,
                                                                                         objParametri,
                                                                                         enum_Id_Servizio.GiasOnline,
                                                                                         "",
                                                                                         riga,
                                                                                         Origine:=SistemaOrigine)
                    GiasContext.Agronica_Log_Anagrafe.Add(log)

                    GiasContext.SaveChanges()
                End If

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function


    Public Function Scrivi_Squadra_Attivita(ByVal piva As String, ByVal riga As String, ByRef objParametri As AgronicaCoreParametri,
                                            Optional NoteLog As String = NOTELOG_ANAGRAFE_NG,
                                            Optional SistemaOrigine As Integer = -1,
                                            Optional ByRef idSquadra As Integer = 0) As String

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_W.Scrivi_Squadra_Attivita()"

        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Dim objSquadra As SquadreXAttivita = JsonConvert.DeserializeObject(Of SquadreXAttivita)(riga)

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                    Dim tipoOperazione As String = "0" ' 0: Non fare niente - 1: Creazione nuova Squadra - 2: Modifica Squadra
                    Dim Squadra As SquadreXAttivita = Nothing

                    If objSquadra.ID_Squadra > 0 Then
                        tipoOperazione = "2"
                        Squadra = (From s In GiasContext.SquadreXAttivita Where s.ID_Squadra = objSquadra.ID_Squadra Select s).FirstOrDefault()
                        Squadra = Gias_EF_Utility.CopyEntity(GiasContext, objSquadra, Squadra)
                        Squadra.Piva_SuperUser = objParametri.PivaSuperUser
                        Squadra.Data_Modifica = Date.Now
                        Squadra.Username_Modifica = objParametri.UsernameOperazione
                        GiasContext.SquadreXAttivita.Attach(Squadra)
                        GiasContext.Entry(Squadra).State = EntityState.Modified
                    Else
                        tipoOperazione = "1"
                        Squadra = Gias_EF_Utility.CopyEntity(GiasContext, objSquadra, Nothing)
                        Squadra.Piva_SuperUser = objParametri.PivaSuperUser
                        Squadra.Piva = piva
                        Squadra.Data_Creazione = Date.Now
                        Squadra.Username_Creazione = objParametri.UsernameOperazione
                        Squadra.Data_Modifica = Date.Now
                        Squadra.Username_Modifica = objParametri.UsernameOperazione
                        Squadra.inviato = 0
                        GiasContext.SquadreXAttivita.Add(Squadra)
                    End If

                    GiasContext.SaveChanges()

                    'Scrittura tabella Agronica_Log_Anagrafe
                    Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                    Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Squadre,
                                                                                         Squadra.Piva,
                                                                                         CStr(Squadra.ID_Squadra),
                                                                                         Nothing, Nothing, Nothing, Nothing,
                                                                                         tipoOperazione,
                                                                                         objParametri,
                                                                                         enum_Id_Servizio.GiasOnline,
                                                                                         NoteLog,
                                                                                         riga,
                                                                                         Origine:=SistemaOrigine)
                    GiasContext.Agronica_Log_Anagrafe.Add(log)
                    GiasContext.SaveChanges()

                    idSquadra = Squadra.ID_Squadra

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

    Public Function Modifica_Dettaglio_Per_Copia_Budget(ByVal IdBudget As Integer,
                                                        ByVal IdAppezza As List(Of Tuple(Of Tuple(Of String, Integer, Integer), Integer)),
                                                        ByVal IdReg As List(Of Tuple(Of Tuple(Of String, Integer, Integer, Integer), Integer)),
                                                        ByVal IdProgetto As List(Of Tuple(Of Integer, Integer)),
                                                        ByVal IdCampo As List(Of Tuple(Of Tuple(Of String, Integer, Integer), Integer)),
                                                        ByRef objParametri As AgronicaCoreParametri,
                                                            Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                            Optional ByVal OpenNewTransaction As Boolean = True
                                                        ) As Integer

        Dim appezzaRec As Tuple(Of Tuple(Of String, Integer, Integer), Integer)
        Dim IdRegRec As Tuple(Of Tuple(Of String, Integer, Integer, Integer), Integer)
        Dim ProgettoRec As Tuple(Of Integer, Integer)
        Dim CampoRec As Tuple(Of Tuple(Of String, Integer, Integer), Integer)

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_W.Modifica_Dettaglio_Per_Copia_Budget()"

        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        If OpenNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadUncommitted
            scope = New TransactionScope(scopeOption, transactionOptions)
        End If

        Try

            Dim dettagli = (From s In GiasContext.CDG_Dettagli Join t In GiasContext.CDG_Testata On s.Id_CDG Equals t.Id_CDG
                            Where t.Budget = IdBudget And s.Appezza > 0 Select s)

            For Each dettaglio As CDG_Dettagli In dettagli

                appezzaRec = IdAppezza.Find(Function(x) x.Item1.Item3 = CInt(dettaglio.Appezza) AndAlso x.Item1.Item1 = CStr(dettaglio.Piva) AndAlso x.Item1.Item2 = CInt(dettaglio.Sa_Cod))
                IdRegRec = IdReg.Find(Function(x) x.Item1.Item4 = CInt(dettaglio.Id_Reg) AndAlso x.Item1.Item3 = CInt(dettaglio.Appezza) AndAlso x.Item1.Item1 = CStr(dettaglio.Piva) AndAlso x.Item1.Item2 = CInt(dettaglio.Sa_Cod))
                ProgettoRec = IdProgetto.Find(Function(x) x.Item1 = CInt(dettaglio.Progetto_Cod))
                CampoRec = IIf(CInt(dettaglio.Campo_Cod) = 0, IdCampo.First, IdCampo.Find(Function(x) x.Item1.Item3 = CInt(dettaglio.Campo_Cod) AndAlso x.Item1.Item1 = CStr(dettaglio.Piva) AndAlso x.Item1.Item2 = CInt(dettaglio.Sa_Cod)))

                If Not ((IsNothing(appezzaRec) OrElse IsNothing(IdRegRec) OrElse IsNothing(ProgettoRec) OrElse IsNothing(CampoRec))) Then

                    dettaglio.Appezza = appezzaRec.Item2
                    dettaglio.Id_Reg = IdRegRec.Item2
                    dettaglio.Progetto_Cod = ProgettoRec.Item2
                    dettaglio.Campo_Cod = CampoRec.Item2
                    dettaglio.Data_Modifica = Date.Now
                    dettaglio.Username_Modifica = objParametri.UsernameOperazione
                    GiasContext.CDG_Dettagli.Attach(dettaglio)
                    GiasContext.Entry(dettaglio).State = EntityState.Modified

                End If
            Next

            GiasContext.SaveChanges()

            If OpenNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As Exception

            If scope IsNot Nothing Then
                scope.Dispose()
            End If

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            If bCloseContext Then
                GiasContext.Dispose()
            End If

        End Try

        Return 0

    End Function

End Class

