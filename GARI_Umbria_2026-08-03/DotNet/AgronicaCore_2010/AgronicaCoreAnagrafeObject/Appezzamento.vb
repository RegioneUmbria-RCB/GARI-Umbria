Imports System.Runtime.Serialization

'Public Class Impresa
'    <DataMember()> _
'    Public Property StringVale() As String
'End Class



<DataContract()>
Public Class Lotto

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value>1 scrittura - 2 Modifica</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Tipo_Operazione() As Integer

    <DataMember()>
    Public Property Partita_Iva() As String

    <DataMember()>
    Public Property Lotto() As String

    <DataMember()>
    Public Property Chiave_Gias() As String

    <DataMember()>
    Public Property Descrizione() As String

    <DataMember()>
    Public Property Progetto_Des() As String

    <DataMember()>
    Public Property Superficie() As Decimal

    <DataMember()>
    Public Property Superficie_Appezzamento() As Decimal

    <DataMember()>
Public Property Codice_Contratto() As String

    '<DataMember()>
    'Public Property Capitolato_Cliente() As String

    <DataMember()>
    Public Property Disciplinare() As String

    <DataMember()>
    Public Property Regolanmento() As String

    <DataMember()>
    Public Property Stringa_Particelle() As List(Of Particelle)



    ''in realtà è la data inizio impianto
    <DataMember()>
    Public Property Data_Semina_Trapianto() As Date

    <DataMember()>
    Public Property Validita_Inizio() As Date

    <DataMember()>
    Public Property Validita_Fine() As Date







    Public Shared Function LeggiListaAppezzamenti(ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                            ByVal Veg_Cod As String, _
                                            ByVal Cul_cod As String, _
                                            ByVal Data_Inizio As Date, _
                                            ByVal Data_Fine As Date, _
                                            ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of Lotto)
        Dim Lista_Appezzamento As New List(Of Lotto)

        Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read


        objParametri.ImpostaFinestre_con_SalvataggioTemporale(Data_Inizio, Data_Fine)
        Dim Dt As DataTable = objAppezza.Leggi_x_Sincronizzatore_WCF(Piva, Sa_Cod, Veg_Cod, Cul_cod, "", "", objParametri)

        Dim separatore_codice As String = "/"
        Dim separatore_descrizione As String = " "

        Dim i As Integer

        For i = 0 To Dt.Rows.Count - 1
            'info appezzamento
            Dim app As New AgronicaCoreAnagrafeObject.Lotto
            app.Partita_Iva = Dt.Rows(i).Item("Piva")

            app.Lotto = Dt.Rows(i).Item("progetto_nome")

            app.Chiave_Gias = Dt.Rows(i).Item("PIVA") & separatore_codice _
                                    & Dt.Rows(i).Item("Sa_Cod") & separatore_codice _
                                    & Dt.Rows(i).Item("Appezza") & separatore_codice _
                                    & Dt.Rows(i).Item("ID_reg") & separatore_codice _
                                    & Dt.Rows(i).Item("Progetto_Cod")

            app.Descrizione = Dt.Rows(i).Item("App_Nome") & separatore_descrizione _
                             & Dt.Rows(i).Item("Sup_Imp") & "ha." & separatore_descrizione _
                             & Dt.Rows(i).Item("Veg_Des") & separatore_descrizione _
                             & Dt.Rows(i).Item("Cul_des") & separatore_descrizione _
                             & Dt.Rows(i).Item("Validita_Inizio_Distinta") & separatore_descrizione _
                             & Dt.Rows(i).Item("Validita_Fine_Distinta") & separatore_descrizione

            If IsDBNull(Dt.Rows(i).Item("Codice_Contratto")) Then
                app.Codice_Contratto = ""
            Else
                app.Codice_Contratto = Dt.Rows(i).Item("Codice_Contratto")
            End If

            app.Validita_Inizio = Dt.Rows(i).Item("Validita_Inizio_Distinta")
            app.Validita_Fine = Dt.Rows(i).Item("Validita_Fine_Distinta")



            'app.Capitolato_Cliente = "cap_cli"
            app.Regolanmento = True
            app.Disciplinare = "disc"

            app.Data_Semina_Trapianto = Dt.Rows(i).Item("Data_Inizio_Impianto")

            'particelle
            Dim objP As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
            Dim DTP As DataTable = objP.LeggiParticelle_Da_Appezzamento(Dt.Rows(i).Item("Piva"), _
                                                                        Dt.Rows(i).Item("sa_cod"), _
                                                                        Dt.Rows(i).Item("Appezza"), AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                                        "", "", _
                                                                        objParametri)

            app.Stringa_Particelle = New List(Of Particelle)

            Dim j As Integer
            For j = 0 To DTP.Rows.Count - 1
                Dim objappParticella As New Particelle
                objappParticella.Provincia = DTP.Rows(j).Item("PROV")
                objappParticella.Comune = DTP.Rows(j).Item("COM")
                objappParticella.Sezione = DTP.Rows(j).Item("Sezione")
                objappParticella.Foglio = DTP.Rows(j).Item("Foglio")
                objappParticella.Numero = DTP.Rows(j).Item("Numero")
                objappParticella.SubAlterno = DTP.Rows(j).Item("Subalterno")

                app.Stringa_Particelle.Add(objappParticella)
            Next

            Lista_Appezzamento.Add(app)
        Next

        Return Lista_Appezzamento
    End Function


    Public Shared Function LeggiAppezzamento(ByVal Piva As String, _
                                            ByVal sa_cod As Integer, _
                                            ByVal Appezza As Integer, _
                                            ByVal Id_Reg As Integer, _
                                            ByVal Data_Inizio As Date, _
                                            ByVal Data_Fine As Date, _
                                            ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Lotto

        Dim elemento_Appezzamento As New Lotto

        Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        objParametri.ImpostaFinestre_con_SalvataggioTemporale(Data_Inizio, Data_Fine)
        Dim Dt As DataTable = objAppezza.Leggi_x_Sincronizzatore_WCF(Piva, sa_cod, Appezza, Id_Reg, "", "", objParametri)

        Dim separatore_codice As String = "/"
        Dim separatore_descrizione As String = " - "

        Dim i As Integer

        For i = 0 To Dt.Rows.Count - 1
            'info appezzamento
            Dim app As New AgronicaCoreAnagrafeObject.Lotto
            app.Partita_Iva = Dt.Rows(i).Item("Piva")

            app.Superficie = Dt.Rows(i).Item("Sup_Imp")

            app.Superficie_Appezzamento = Dt.Rows(i).Item("Sup_App")
             
            app.Progetto_Des = Dt.Rows(i).Item("Progetto_Des")

            app.Lotto = Dt.Rows(i).Item("Progetto_nome")
            app.Chiave_Gias = Dt.Rows(i).Item("PIVA") & separatore_codice _
                                    & Dt.Rows(i).Item("Sa_Cod") & separatore_codice _
                                    & Dt.Rows(i).Item("Appezza") & separatore_codice _
                                    & Dt.Rows(i).Item("ID_reg") & separatore_codice _
                                    & Dt.Rows(i).Item("Progetto_Cod")

            app.Descrizione = Dt.Rows(i).Item("App_Nome") & separatore_descrizione _
                             & Dt.Rows(i).Item("Sup_Imp") & "ha." & separatore_descrizione _
                             & Dt.Rows(i).Item("Veg_Des") & separatore_descrizione _
                             & Dt.Rows(i).Item("Cul_des") & separatore_descrizione _
                             & Dt.Rows(i).Item("Validita_Inizio_Distinta") & separatore_descrizione _
                             & Dt.Rows(i).Item("Validita_Fine_Distinta") & separatore_descrizione

            If IsDBNull(Dt.Rows(i).Item("Codice_Contratto")) Then
                app.Codice_Contratto = ""
            Else
                app.Codice_Contratto = Dt.Rows(i).Item("Codice_Contratto")
            End If


            app.Regolanmento = Dt.Rows(i).Item("reg_des")
            app.Disciplinare = Dt.Rows(i).Item("disciplinare")

            app.Data_Semina_Trapianto = Dt.Rows(i).Item("Data_Inizio_Impianto")

            'particelle
            Dim objP As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
            Dim DTP As DataTable = objP.LeggiParticelle_Da_Appezzamento(Dt.Rows(i).Item("Piva"), _
                                                                        Dt.Rows(i).Item("sa_cod"), _
                                                                        Dt.Rows(i).Item("Appezza"), AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                                        "", "", _
                                                                        objParametri)

            app.Stringa_Particelle = New List(Of Particelle)

            Dim j As Integer
            For j = 0 To DTP.Rows.Count - 1
                Dim objappParticella As New Particelle
                objappParticella.Provincia = DTP.Rows(j).Item("PROV")
                objappParticella.Comune = DTP.Rows(j).Item("COM")
                objappParticella.Sezione = DTP.Rows(j).Item("Sezione")
                objappParticella.Foglio = DTP.Rows(j).Item("Foglio")
                objappParticella.Numero = DTP.Rows(j).Item("Numero")
                objappParticella.SubAlterno = DTP.Rows(j).Item("Subalterno")

                objappParticella.Area_Condotta = DTP.Rows(j).Item("Area")

                app.Stringa_Particelle.Add(objappParticella)
            Next

            elemento_Appezzamento = app
        Next

        Return elemento_Appezzamento
    End Function

End Class
