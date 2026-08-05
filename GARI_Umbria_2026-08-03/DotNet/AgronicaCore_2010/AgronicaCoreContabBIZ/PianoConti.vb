Imports System.Data
Imports System.Xml
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class PianoConti_R
    Inherits AgronicaCoreDataProvider.LogProvider




    '============================================================================
    Public Function Leggi( _
                            ByVal Piva As String, _
                            ByVal Ric_Cod As Integer, _
                            ByVal Anno As Integer, _
                            ByVal Cod_Conto As Integer, _
                            ByVal Cod_Contatto As String, _
                            ByVal FinestraTemp_Inizio As Date, _
                            ByVal FinestraTemp_Fine As Date, _
                            ByVal ForDelete As Boolean, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                    As Boolean




        '============================================================================


        Const nomeRoutine = "ContabBIZ.PianoConti_R.Leggi()"
        Dim Dummy As Boolean

        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim i, j As Int32


        Dim RisultatoFunzione As String = String.Empty


        Dim XmlDoc As XmlDocument


        Dim XmlDatiConti As XmlElement
        Dim XmlConto As XmlElement
        Dim XmlRicxCod As XmlElement

        Dim dtPianoConti As DataTable

        '------------------------------'

        Try


            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                     FlagTransazioneLocale, _
                                                                                     objParametri)

            '------------------------------

            'Mi procuro un elenco dei Movimenti di Agenda associati all'Impresa
            'all'interno della finestra temporale selezionata

            'CONTI _R 
            Dim objConti As New AgronicaCoreContabDAL.Conti_R

            'Mi procuro il recordset richiesto

            dtPianoConti = objConti.Leggi("", _
                                    CLng(Cod_Conto), _
                                    CStr(Cod_Contatto), _
                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                      "", _
                                      "", _
                                    objParametri)

                                 




            'Se ottengo almeno un risultato, creo la struttura XML
            If dtPianoConti.Rows.Count > 0 Then




                '----- < Documento XML > -----
                XmlDoc = New XmlDocument

                XmlDatiConti = XmlDoc.CreateElement("DatiConti")


                'Effettuo un ciclo 
                For i = 0 To dtPianoConti.Rows.Count - 1

                    '----- < CONTO > -----
                    XmlConto = XmlDoc.CreateElement("Conto")

                    With XmlConto
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(dtPianoConti.Rows(i).Item("PIVA")))

                        .SetAttribute("cod_conto", Agro_SQL_Load(dtPianoConti.Rows(i).Item("Cod_Conto")))
                        .SetAttribute("conto_descr", Agro_SQL_Load(dtPianoConti.Rows(i).Item("Conto_Descr")))
                        .SetAttribute("flag_ue", Agro_SQL_Load(dtPianoConti.Rows(i).Item("Flag_UE")))
                        .SetAttribute("cod_contatto", Agro_SQL_Load(dtPianoConti.Rows(i).Item("Cod_Contatto")))
                        .SetAttribute("extra_str", Agro_SQL_Load(dtPianoConti.Rows(i).Item("Extra_Str")))
                        .SetAttribute("extra_int", Agro_SQL_Load(dtPianoConti.Rows(i).Item("Extra_Int")))
                        .SetAttribute("extra_date", Agro_SQL_Load(dtPianoConti.Rows(i).Item("Extra_Date")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(dtPianoConti.Rows(i).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(dtPianoConti.Rows(i).Item("Validita_Fine")))


                    End With


                    '#############################################
                    '###### CONTI X RICLASSIFICAZIONI  ###########
                    '#############################################

                    'Se ottengo almeno un risultato, creo la struttura XML

                    'BASSO LIVELLO DA FARE

                    Dim objRicxCod As New AgronicaCoreContabDAL.RicxConti_R

                    Dim DTRicxCod As DataTable

                    DTRicxCod = objRicxCod.Leggi_New(Piva,
                                                     Ric_Cod,
                                                     Anno,
                                                     dtPianoConti.Rows(i).Item("Cod_Conto"),
                                                     "", "",
                                                     "", "",
                                                     objParametri)

                    For j = 0 To DTRicxCod.Rows.Count - 1
                        XmlRicxCod = XmlDoc.CreateElement("RicXCod")
                        With XmlRicxCod
                            .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                            .SetAttribute("piva", Agro_SQL_Load(DTRicxCod.Rows(j).Item("Piva")))
                            .SetAttribute("ric_cod", Agro_SQL_Load(DTRicxCod.Rows(j).Item("Ric_Cod")))
                            .SetAttribute("cod_conto", Agro_SQL_Load(DTRicxCod.Rows(j).Item("Cod_Conto")))
                            .SetAttribute("anno", Agro_SQL_Load(DTRicxCod.Rows(j).Item("Anno")))
                            .SetAttribute("id_riclassificazione", Agro_SQL_Load(DTRicxCod.Rows(j).Item("Id_Riclassificazione")))
                            .SetAttribute("dare_avere", Agro_SQL_Load(DTRicxCod.Rows(j).Item("Dare_Avere")))
                            .SetAttribute("imputabile", Agro_SQL_Load(DTRicxCod.Rows(j).Item("Imputabile")))
                            .SetAttribute("saldo", Agro_SQL_Load(DTRicxCod.Rows(j).Item("Saldo")))
                            .SetAttribute("validita_inizio", Agro_SQL_Load(DTRicxCod.Rows(j).Item("validita_inizio")))
                            .SetAttribute("validita_fine", Agro_SQL_Load(DTRicxCod.Rows(j).Item("validita_fine")))
                        End With


                        XmlConto.AppendChild(XmlRicxCod)
                    Next

                    XmlDatiConti.AppendChild(XmlConto)


                Next



                XmlDoc.AppendChild(XmlDatiConti)

                RisultatoFunzione = XmlDoc.InnerXml

            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)


        Catch ex As Exception

            RisultatoFunzione = False

            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        'Restituisco il risultato
        Return RisultatoFunzione




    End Function






End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

