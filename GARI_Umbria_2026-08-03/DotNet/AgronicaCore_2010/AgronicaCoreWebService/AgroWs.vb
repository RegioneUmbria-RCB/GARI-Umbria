Imports System
Imports System.Security.Policy
Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste

Public Class AgroWs

    Public AWS_AgroKey_EncoderDecoder As String = "cJy97ei45vrNIsvbe86Wn42rffCHvc8e3it63koLa12"


    '###############################################################################################
    Public Enum enum_AWS_Schede

        AWS_Utility_Versione = 9000

        AWS_Fitofarmaci_Formulati = 1000
        AWS_Fitofarmaci_Formulati_PrincipiAttivi_SpecieVegetali = 1010
        AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi = 1020
        AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi = 1030
        AWS_Fitofarmaci_Formulati_SpecieVegetali = 1040
        AWS_Fitofarmaci_Formulato_Completo = 1050
        AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita = 1060
        AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti = 1070
        AWS_Fitofarmaci_Formulati_PrincipiAttivi = 1080
        AWS_Fitofarmaci_Formulati_Classificazioni = 1090

        AWS_Disciplinari_Formulato_Completo = 2050

    End Enum


    '###############################################################################################
    Public Enum enum_AWS_Xml
        Xml_CODIFICA = 1
        Xml_DECODIFICA = 2
    End Enum



    '###############################################################################################
    Public Function AgroWS_DoorKey(
                                ByVal Scheda As enum_AWS_Schede) _
                                As String

        Select Case Scheda

            Case enum_AWS_Schede.AWS_Utility_Versione
                Return "gommolo"

            Case enum_AWS_Schede.AWS_Fitofarmaci_Formulati
                Return "cipolla_T472GyUas67PoBn6meZv"

            Case enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi_SpecieVegetali
                Return "ostrica_Yh76nDF6syd78DnMTT72"

            Case enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi
                Return "vittoria_Gs5DmUIj6GhfsWlsi88w"

            Case enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi
                Return "zattera_Uh7sR95kJi83hG82PpO9ws"

            Case enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali
                Return "iperbole_ktiud32ArkfoiCYue40pdl4G"

            Case enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo
                Return "ciaomare_jH7fuYtnbiHHjdhfYuaI"

            Case enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti
                Return "padella_np5k0ijFEjspkj42gGpsH"

            Case enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita
                Return "canotto_pjE4Uaiwbn37mdoayhaDP"

            Case enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi
                Return "portone_grth45ksh4ghajnl32149"

            Case enum_AWS_Schede.AWS_Fitofarmaci_Formulati_Classificazioni
                Return "bottone_lkjgropaghpe98t0jkpsh"


            Case enum_AWS_Schede.AWS_Disciplinari_Formulato_Completo
                Return "ciaonee_oighvnvaoiJGARGJWRsjk"


            Case Else
                Return "non definita"

        End Select

    End Function






    '####################################################################################
    Public Function AWS_Codifica_P(ByVal Testo As String) As String

        Dim strEncrypted As String
        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte

        Dim Chiave As String = AWS_AgroKey_EncoderDecoder
        Dim Cript As Boolean = True

        Testo = Testo.Replace("<", "{")
        Testo = Testo.Replace(">", "}")
        KeyPos = 1

        For i = 1 To Len(Testo)
            A1 = Asc(Mid(Testo, i, 1))
            A2 = Asc(Mid(Chiave, KeyPos, 1))
            If Cript Then
                strEncrypted = strEncrypted & Chr(A2 + A1)
            Else
                strEncrypted = strEncrypted & Chr(A1 - A2)
            End If
            KeyPos = KeyPos + 1
            If KeyPos > Len(Chiave) Then KeyPos = 1
        Next

        Return strEncrypted

    End Function


    '####################################################################################
    Public Function AWS_Decodifica_P(ByVal Testo As String) As String

        Dim strEncrypted As String
        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte

        Dim Chiave As String = AWS_AgroKey_EncoderDecoder
        Dim Cript As Boolean = False

        KeyPos = 1

        For i = 1 To Len(Testo)
            A1 = Asc(Mid(Testo, i, 1))
            A2 = Asc(Mid(Chiave, KeyPos, 1))
            If Cript Then
                strEncrypted = strEncrypted & Chr(A2 + A1)
            Else
                strEncrypted = strEncrypted & Chr(A1 - A2)
            End If
            KeyPos = KeyPos + 1
            If KeyPos > Len(Chiave) Then KeyPos = 1
        Next

        strEncrypted = strEncrypted.Replace("{", "<")
        strEncrypted = strEncrypted.Replace("}", ">")

        Return strEncrypted

    End Function





    '####################################################################################
    Public Function AWS_Codifica_R(ByVal Testo As String) As String

        Return Testo

    End Function




    '####################################################################################
    Public Function AWS_Decodifica_R(ByVal Testo As String) As String

        Return Testo

    End Function



    '###############################################################################################
    Public Sub AgroWS_XML__Credenziali(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Scheda As enum_AWS_Schede,
                                    ByRef DoorKey As String,
                                    ByRef CodiceGias As Integer,
                                    ByRef UserName As String,
                                    ByRef Password As String)

        Dim xmlDoc As New Xml.XmlDocument
        Dim xmlCredenziali As Xml.XmlElement
        Dim xmlNodo As Xml.XmlElement

        '----------------------------------------------
        '   <CREDENZIALI scheda="..." doorkey="..." codicegias="..." username="...." password="..." />
        '----------------------------------------------

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo CREDENZIALI
                xmlCredenziali = xmlDoc.CreateElement("CREDENZIALI")

                'Imposto gli attributi
                xmlCredenziali.SetAttribute("scheda", CStr(CInt(Scheda)))
                xmlCredenziali.SetAttribute("doorkey", CStr(DoorKey))
                xmlCredenziali.SetAttribute("codicegias", CStr(CodiceGias))
                xmlCredenziali.SetAttribute("username", CStr(UserName))
                xmlCredenziali.SetAttribute("password", CStr(Password))

                'Imposto XmlParametri come figlio del documento principale
                xmlDoc.AppendChild(xmlCredenziali)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlNodo = Nothing
                xmlCredenziali = Nothing
                xmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                xmlCredenziali = xmlDoc.SelectSingleNode("//CREDENZIALI")

                'Prelevo gli attributi
                Scheda = CInt(xmlCredenziali.GetAttribute("scheda"))
                DoorKey = CStr(xmlCredenziali.GetAttribute("doorkey"))
                CodiceGias = CInt(xmlCredenziali.GetAttribute("codicegias"))
                UserName = CStr(xmlCredenziali.GetAttribute("username"))
                Password = CStr(xmlCredenziali.GetAttribute("password"))

                'Distruggo gli oggetti
                xmlNodo = Nothing
                xmlCredenziali = Nothing
                xmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    'in input/output necessita di piu parametri della precedente
    Public Sub AgroWS_XML__Credenziali(
                                 ByVal Operazione As enum_AWS_Xml,
                                 ByRef StringaXML As String,
                                 ByRef Scheda As enum_AWS_Schede,
                                 ByRef DoorKey As String,
                                 ByRef CodiceGias As Integer,
                                 ByRef SuperUser_Piva As String,
                                 ByRef SuperUser_UserName As String,
                                 ByRef SuperUser_Password As String,
                                 ByRef Utente_UserName As String,
                                 ByRef Utente_Password As String)

        Dim xmlDoc As New Xml.XmlDocument
        Dim xmlCredenziali As Xml.XmlElement
        Dim xmlNodo As Xml.XmlElement

        '----------------------------------------------
        '   <CREDENZIALI scheda="..." doorkey="..." codicegias="..." username="...." password="..." />
        '----------------------------------------------

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo CREDENZIALI
                xmlCredenziali = xmlDoc.CreateElement("CREDENZIALI")

                'Imposto gli attributi
                xmlCredenziali.SetAttribute("scheda", CStr(CInt(Scheda)))
                xmlCredenziali.SetAttribute("doorkey", CStr(DoorKey))
                xmlCredenziali.SetAttribute("codicegias", CStr(CodiceGias))
                xmlCredenziali.SetAttribute("superuser_piva", CStr(SuperUser_Piva))
                xmlCredenziali.SetAttribute("superuser_username", CStr(SuperUser_UserName))
                xmlCredenziali.SetAttribute("superuser_password", CStr(SuperUser_Password))
                xmlCredenziali.SetAttribute("utente_username", CStr(Utente_UserName))
                xmlCredenziali.SetAttribute("utente_password", CStr(Utente_Password))

                'Imposto XmlParametri come figlio del documento principale
                xmlDoc.AppendChild(xmlCredenziali)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlNodo = Nothing
                xmlCredenziali = Nothing
                xmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                xmlCredenziali = xmlDoc.SelectSingleNode("//CREDENZIALI")

                'Prelevo gli attributi
                Scheda = CInt(xmlCredenziali.GetAttribute("scheda"))
                DoorKey = CStr(xmlCredenziali.GetAttribute("doorkey"))
                CodiceGias = CInt(xmlCredenziali.GetAttribute("codicegias"))
                SuperUser_Piva = CStr(xmlCredenziali.GetAttribute("superuser_piva"))
                SuperUser_UserName = CStr(xmlCredenziali.GetAttribute("superuser_username"))
                SuperUser_Password = CStr(xmlCredenziali.GetAttribute("superuser_password"))
                Utente_UserName = CStr(xmlCredenziali.GetAttribute("utente_username"))
                Utente_Password = CStr(xmlCredenziali.GetAttribute("utente_password"))


                'Distruggo gli oggetti
                xmlNodo = Nothing
                xmlCredenziali = Nothing
                xmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    '###############################################################################################
    Public Function AgroWS_XML__Credenziali_Scheda(ByRef StringaXML As String) As enum_AWS_Schede

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlCredenziali As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement
        Dim Scheda As enum_AWS_Schede

        '----------------------------------------------
        '   <CREDENZIALI scheda="..." doorkey="..." codicegias="..." username="...." password="..." />
        '----------------------------------------------

        'Carico la stringa XML nel documento
        XmlDoc.LoadXml(StringaXML)

        'Prelevo il nodo XmlParametri
        XmlCredenziali = XmlDoc.SelectSingleNode("//CREDENZIALI")

        'Prelevo gli attributi
        Scheda = CInt(XmlCredenziali.GetAttribute("scheda"))

        'Distruggo gli oggetti
        XmlNodo = Nothing
        XmlCredenziali = Nothing
        XmlDoc = Nothing

        'Restituisco il risultato
        Return Scheda

    End Function


    '###############################################################################################
    Public Sub AgroWS_XML__Risultati(ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '----------------------------------------------
        '   <RISULTATI errore="...">
        '----------------------------------------------

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo CREDENZIALI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", CStr(Errore))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlRisultati
                XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

                'Prelevo gli attributi
                Errore = CStr(XmlRisultati.GetAttribute("errore"))

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    '###############################################################################################
    Public Sub AgroWS_XML__Versione(ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Versione As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '----------------------------------------------
        '   <RISULTATI versione="...">
        '----------------------------------------------

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo CREDENZIALI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("versione", CStr(Versione))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlRisultati
                XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

                'Prelevo gli attributi
                Versione = CStr(XmlRisultati.GetAttribute("versione"))

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub


    '###############################################################################################
    Public Sub AgroWS_XML_Parametri_Formulati(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Fr_Cod As String,
                                    ByRef Fr_Des As String,
                                    ByRef Class_Cod As String,
                                    ByRef DataRilievo As String,
                                    ByRef Flag_SoloAttivi As Boolean,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="...." password="..." >
        '       <PARAMETRI fr_cod="" 
        '                  fr_des=""                        oppure  "diserb"
        '                  class_cod=""                     oppure  "NULL,100,101,102,103"
        '                  datarilievo=""                   oppure  "18/08/2005"
        '                  flag_soloattivi="true" />
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("fr_des", CStr(Fr_Des))
                XmlParametri.SetAttribute("class_cod", CStr(Class_Cod))
                XmlParametri.SetAttribute("datarilievo", CStr(DataRilievo))
                XmlParametri.SetAttribute("flag_soloattivi", CStr(Flag_SoloAttivi))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                Fr_Des = CStr(XmlParametri.GetAttribute("fr_des"))
                Class_Cod = CStr(XmlParametri.GetAttribute("class_cod"))
                DataRilievo = CStr(XmlParametri.GetAttribute("datarilievo"))
                Flag_SoloAttivi = CBool(XmlParametri.GetAttribute("flag_soloattivi"))

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub







    '###############################################################################################
    Public Sub AgroWS_XML_Parametri_Formulato_Completo(ByVal Operazione As enum_AWS_Xml,
                                                        ByRef StringaXML As String,
                                                        ByRef Fr_Cod As String,
                                                        ByRef Errore As String,
                                                        Optional ByRef Data As String = "",
                                                        Optional ByRef Tipo As String = "",
                                                        Optional ByRef Fr_Des As String = "",
                                                        Optional ByRef Stato_Cod As String = "",
                                                        Optional ByRef Veg_Cod As String = "0")

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="...." password="..." >
        '       <PARAMETRI fr_cod="" 
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))


                If Data <> "" Then
                    If IsDate(Data) AndAlso CDate(CDate(Data).ToShortDateString) = AGRODATAINIZIO Then
                        Data = ""
                    End If
                    XmlParametri.SetAttribute("data", CStr(Data))
                End If
                If Tipo <> "" Then
                    XmlParametri.SetAttribute("tipo", CStr(Tipo))
                End If
                If Fr_Des <> "" Then
                    XmlParametri.SetAttribute("fr_des", CStr(Fr_Des))
                End If

                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))

                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))

                If XmlParametri.HasAttribute("data") Then
                    Data = XmlParametri.GetAttribute("data")
                End If
                If XmlParametri.HasAttribute("tipo") Then
                    Tipo = XmlParametri.GetAttribute("tipo")
                End If
                If XmlParametri.HasAttribute("fr_des") Then
                    Fr_Des = XmlParametri.GetAttribute("fr_des")
                End If

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If

                If XmlParametri.HasAttribute("veg_cod") Then
                    Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    '###############################################################################################
    Public Sub AgroWS_XML_Parametri_Formulati_ConDosi(
                            ByVal Operazione As enum_AWS_Xml,
                            ByRef StringaXML As String,
                            ByRef Veg_Cod As String,
                            ByRef Opt_Avversita_Infestanti As String,
                            ByRef Opt_Singola_Gruppo As String,
                            ByRef Av_Gru() As Integer,
                            ByRef Av_Cod() As Integer,
                            ByRef strPA As String,
                            ByRef TipoRichiesto As String,
                            ByRef TestoRicerca As String,
                            ByRef Data As String,
                            ByRef strFiltro As String,
                            ByRef strSort As String,
                            ByRef Errore As String,
                            ByRef Grfi_cod As Integer,
                            ByRef FlagVisualizza_Commercio_Tutti_Revocati As Integer,
                            ByRef Stato_Cod As String)


        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        Dim i, n As Integer
        Dim strAvCod As String = ""
        Dim strAvGru As String = ""

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="...." password="..." >
        '       <PARAMETRI veg_cod="" Opt_Avversita_Infestanti="" Opt_Singola_Gruppo=""
        '                  Av_Gru="" Av_Cod="" strPA="" TipoRichiesto="" TestoRicerca=""
        '                  Data="" strFiltro="" strSort=""
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("opt_avversita_infestanti", CStr(Opt_Avversita_Infestanti))
                XmlParametri.SetAttribute("opt_singola_gruppo", CStr(Opt_Singola_Gruppo))

                If Av_Gru IsNot Nothing Then
                    For i = 0 To UBound(Av_Gru)
                        strAvGru &= Av_Gru(i).ToString & ","
                    Next
                    If strAvGru <> "" Then
                        strAvGru = Left(strAvGru, strAvGru.Length - 1)
                    End If
                End If
                If Av_Cod IsNot Nothing Then
                    For i = 0 To UBound(Av_Cod)
                        strAvCod &= Av_Cod(i).ToString & ","
                    Next
                    If strAvCod <> "" Then
                        strAvCod = Left(strAvCod, strAvCod.Length - 1)
                    End If
                End If

                XmlParametri.SetAttribute("av_gru", CStr(strAvGru))
                XmlParametri.SetAttribute("av_cod", CStr(strAvCod))

                XmlParametri.SetAttribute("strpa", CStr(strPA))
                XmlParametri.SetAttribute("tiporichiesto", CStr(TipoRichiesto))
                XmlParametri.SetAttribute("testoricerca", CStr(TestoRicerca))
                XmlParametri.SetAttribute("data", CStr(Data))
                XmlParametri.SetAttribute("strfiltro", CStr(strFiltro))
                XmlParametri.SetAttribute("strsort", CStr(strSort))
                XmlParametri.SetAttribute("grfi_cod", CStr(Grfi_cod))

                XmlParametri.SetAttribute("flagvisualizza_commercio_tutti_revocati", CStr(FlagVisualizza_Commercio_Tutti_Revocati))

                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                Dim ArrayTmp As String()

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                Opt_Avversita_Infestanti = CStr(XmlParametri.GetAttribute("opt_avversita_infestanti"))
                Opt_Singola_Gruppo = CStr(XmlParametri.GetAttribute("opt_singola_gruppo"))
                n = 0
                ArrayTmp = Split(XmlParametri.GetAttribute("av_gru"), ",")
                If ArrayTmp IsNot Nothing AndAlso ArrayTmp.Length > 0 Then
                    For i = 0 To ArrayTmp.Length - 1
                        ReDim Preserve Av_Gru(n)
                        If ArrayTmp(i) <> "" Then
                            Av_Gru(n) = CInt(ArrayTmp(i))
                        Else
                            Av_Gru(n) = 0
                        End If
                        n += 1
                    Next
                End If
                n = 0
                ArrayTmp = Split(XmlParametri.GetAttribute("av_cod"), ",")
                If ArrayTmp IsNot Nothing AndAlso ArrayTmp.Length > 0 Then
                    For i = 0 To ArrayTmp.Length - 1
                        ReDim Preserve Av_Cod(n)
                        If ArrayTmp(i) <> "" Then
                            Av_Cod(n) = CInt(ArrayTmp(i))
                        Else
                            Av_Cod(n) = 0
                        End If
                        n += 1
                    Next
                End If
                'Av_Gru = Split(XmlParametri.GetAttribute("av_gru"), ",")
                'Av_Cod = Split(XmlParametri.GetAttribute("av_cod"), ",")
                strPA = CStr(XmlParametri.GetAttribute("strpa"))
                TipoRichiesto = CStr(XmlParametri.GetAttribute("tiporichiesto"))
                TestoRicerca = CStr(XmlParametri.GetAttribute("testoricerca"))
                Data = CStr(XmlParametri.GetAttribute("data"))
                strFiltro = CStr(XmlParametri.GetAttribute("strfiltro"))
                strSort = CStr(XmlParametri.GetAttribute("strsort"))

                Dim sGrfi_Cod As String
                If XmlParametri.HasAttribute("grfi_cod") Then
                    sGrfi_Cod = CStr(XmlParametri.GetAttribute("grfi_cod"))
                    If sGrfi_Cod <> "" Then
                        Grfi_cod = sGrfi_Cod
                    Else
                        Grfi_cod = 0
                    End If
                End If

                FlagVisualizza_Commercio_Tutti_Revocati = 0
                If XmlParametri.HasAttribute("flagvisualizza_commercio_tutti_revocati") AndAlso
                   IsNumeric(XmlParametri.GetAttribute("flagvisualizza_commercio_tutti_revocati")) Then
                    FlagVisualizza_Commercio_Tutti_Revocati = CInt(XmlParametri.GetAttribute("flagvisualizza_commercio_tutti_revocati"))
                End If

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    Public Function Leggi_Formulato_Info_con_UdM_DS(listaProdottiStr As String,
                                                    Stato_Cod As String,
                                                    ByVal ASG_ProgressivoGIAS As Integer,
                                                    ASG_SuperUser_Username As String,
                                                    ASG_SuperUser_Password As String,
                                                    objparametri_Server As AgronicaCoreParametri
                                                    ) As DataTable

        Dim dt_prodotti As DataTable

        Try

            '''''''''''''''''''''''''''''modifica webservice
            Dim XML_Credenziali As Xml.XmlElement
            Dim StrCredenziali As String = ""
            Dim StrParametri As String = ""
            Dim Parametri As String = ""
            Dim strErr As String = ""

            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            Dim xmlDoc As New Xml.XmlDocument

            AgroWS_XML__Credenziali(enum_AWS_Xml.Xml_CODIFICA,
                                    StrCredenziali,
                                    enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo,
                                    AgroWS_DoorKey(enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo),
                                    ASG_ProgressivoGIAS,
                                    ASG_SuperUser_Username,
                                    ASG_SuperUser_Password)

            xmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = xmlDoc.SelectSingleNode("CREDENZIALI")

            Dim leggiConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim urlWS As String =
                leggiConfigSiti.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", objparametri_Server)

            ObjDownloadWs.Url = urlWS
            AgroWS_XML_Parametri_Formulato_Completo(enum_AWS_Xml.Xml_CODIFICA,
                                                    StrParametri,
                                                    listaProdottiStr,
                                                    strErr,
                                                    Stato_Cod:=Stato_Cod)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = xmlDoc.OuterXml

            Parametri = AWS_Codifica_P(Parametri)

            Dim ds As DataSet

            ds = ObjDownloadWs.Leggi_Formulato_Info_con_UdM_DS(Parametri, strErr)

            dt_prodotti = ds.Tables("formulati")

            ObjDownloadWs.Dispose()

        Catch ex As Exception
            dt_prodotti = Nothing
        End Try

        Return dt_prodotti
    End Function

    Public Sub AgroWS_XML_Parametri_Formulati_ConDosi_Profitosan(
                        ByVal Operazione As enum_AWS_Xml,
                        ByRef StringaXML As String,
                        ByRef Applicazione_Richiedente As Integer,
                        ByRef Veg_Cod As String,
                        ByRef Dpi_Cod As Integer,
                        ByRef DPI_Privato_Pubblico As Integer,
                        ByRef Id_Rcdpi As Integer,
                        ByRef Tipo_Testata As Integer,
                        ByRef Opt_Avversita_Infestanti As String,
                        ByRef Opt_Singola_Gruppo As String,
                        ByRef Av_Gru() As Integer,
                        ByRef Av_Cod() As Integer,
                        ByRef strAvversita As String,
                        ByRef Modulo As Integer,
                        ByRef Ep_Cod As Integer,
                        ByRef strPA As String,
                        ByRef TipoRichiesto As String,
                        ByRef TestoRicerca As String,
                        ByRef Data As String,
                        ByRef strFiltro As String,
                        ByRef strSort As String,
                        ByRef Errore As String,
                        ByRef Grfi_cod As Integer,
                        ByRef FlagVisualizza_Commercio_Tutti_Revocati As Integer,
                        ByRef ListaComuni As String,
                        ByRef FormulatiXAllegatiNormative_IDRiga As Integer,
                        ByRef Copertura As String,
                        ByRef Stato_Cod As String,
                        ByRef Lingua_Cod As String,
                        Optional ByRef Stato_Impianto As Integer = 0
                    )

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        Dim i, n As Integer
        Dim strAvCod As String = ""
        Dim strAvGru As String = ""

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="...." password="..." >
        '       <PARAMETRI veg_cod="" Opt_Avversita_Infestanti="" Opt_Singola_Gruppo=""
        '                  Av_Gru="" Av_Cod="" strPA="" TipoRichiesto="" TestoRicerca=""
        '                  Data="" strFiltro="" strSort=""
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("applicazione_richiedente", CStr(Applicazione_Richiedente))
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("dpi_cod", CStr(Dpi_Cod))
                XmlParametri.SetAttribute("dpi_privato_pubblico", CStr(DPI_Privato_Pubblico))
                XmlParametri.SetAttribute("id_rcdpi", CStr(Id_Rcdpi))
                XmlParametri.SetAttribute("tipo_testata", CStr(Tipo_Testata))
                XmlParametri.SetAttribute("opt_avversita_infestanti", CStr(Opt_Avversita_Infestanti))
                XmlParametri.SetAttribute("opt_singola_gruppo", CStr(Opt_Singola_Gruppo))

                If Av_Gru IsNot Nothing Then
                    For i = 0 To UBound(Av_Gru)
                        strAvGru &= Av_Gru(i).ToString & ","
                    Next
                    If strAvGru <> "" Then
                        strAvGru = Left(strAvGru, strAvGru.Length - 1)
                    End If
                End If
                If Av_Cod IsNot Nothing Then
                    For i = 0 To UBound(Av_Cod)
                        strAvCod &= Av_Cod(i).ToString & ","
                    Next
                    If strAvCod <> "" Then
                        strAvCod = Left(strAvCod, strAvCod.Length - 1)
                    End If
                End If

                XmlParametri.SetAttribute("av_gru", CStr(strAvGru))
                XmlParametri.SetAttribute("av_cod", CStr(strAvCod))

                XmlParametri.SetAttribute("strpa", CStr(strPA))
                XmlParametri.SetAttribute("tiporichiesto", CStr(TipoRichiesto))
                XmlParametri.SetAttribute("testoricerca", CStr(TestoRicerca))
                XmlParametri.SetAttribute("data", CStr(Data))
                XmlParametri.SetAttribute("strfiltro", CStr(strFiltro))
                XmlParametri.SetAttribute("strsort", CStr(strSort))
                XmlParametri.SetAttribute("grfi_cod", CStr(Grfi_cod))
                XmlParametri.SetAttribute("stato_impianto", CStr(Stato_Impianto))

                XmlParametri.SetAttribute("stravversita", CStr(strAvversita))
                XmlParametri.SetAttribute("modulo", CStr(Modulo))
                XmlParametri.SetAttribute("ep_cod", CStr(Ep_Cod))

                XmlParametri.SetAttribute("flagvisualizza_commercio_tutti_revocati", CStr(FlagVisualizza_Commercio_Tutti_Revocati))

                XmlParametri.SetAttribute("listacomuni", CStr(ListaComuni))

                XmlParametri.SetAttribute("formulatixallegatinormative_idriga", CStr(FormulatiXAllegatiNormative_IDRiga))

                XmlParametri.SetAttribute("copertura", CStr(Copertura))

                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))
                XmlParametri.SetAttribute("lingua_cod", CStr(Lingua_Cod))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                Dim arrayTmp As String()

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Applicazione_Richiedente = CStr(XmlParametri.GetAttribute("applicazione_richiedente"))
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                Dpi_Cod = CStr(XmlParametri.GetAttribute("dpi_cod"))
                DPI_Privato_Pubblico = 0
                If XmlParametri.HasAttribute("dpi_privato_pubblico") Then
                    DPI_Privato_Pubblico = CStr(XmlParametri.GetAttribute("dpi_privato_pubblico"))
                End If
                Id_Rcdpi = CStr(XmlParametri.GetAttribute("id_rcdpi"))
                Tipo_Testata = CStr(XmlParametri.GetAttribute("tipo_testata"))
                Opt_Avversita_Infestanti = CStr(XmlParametri.GetAttribute("opt_avversita_infestanti"))
                Opt_Singola_Gruppo = CStr(XmlParametri.GetAttribute("opt_singola_gruppo"))
                n = 0
                arrayTmp = Split(XmlParametri.GetAttribute("av_gru"), ",")
                If arrayTmp IsNot Nothing AndAlso arrayTmp.Length > 0 Then
                    For i = 0 To arrayTmp.Length - 1
                        ReDim Preserve Av_Gru(n)
                        If arrayTmp(i) <> "" Then
                            Av_Gru(n) = CInt(arrayTmp(i))
                        Else
                            Av_Gru(n) = 0
                        End If
                        n += 1
                    Next
                End If
                n = 0
                arrayTmp = Split(XmlParametri.GetAttribute("av_cod"), ",")
                If arrayTmp IsNot Nothing AndAlso arrayTmp.Length > 0 Then
                    For i = 0 To arrayTmp.Length - 1
                        ReDim Preserve Av_Cod(n)
                        If arrayTmp(i) <> "" Then
                            Av_Cod(n) = CInt(arrayTmp(i))
                        Else
                            Av_Cod(n) = 0
                        End If
                        n += 1
                    Next
                End If
                'Av_Gru = Split(XmlParametri.GetAttribute("av_gru"), ",")
                'Av_Cod = Split(XmlParametri.GetAttribute("av_cod"), ",")
                strPA = CStr(XmlParametri.GetAttribute("strpa"))
                TipoRichiesto = CStr(XmlParametri.GetAttribute("tiporichiesto"))
                TestoRicerca = CStr(XmlParametri.GetAttribute("testoricerca"))
                Data = CStr(XmlParametri.GetAttribute("data"))
                strFiltro = CStr(XmlParametri.GetAttribute("strfiltro"))
                strSort = CStr(XmlParametri.GetAttribute("strsort"))

                strAvversita = CStr(XmlParametri.GetAttribute("stravversita"))
                Modulo = CStr(XmlParametri.GetAttribute("modulo"))
                Ep_Cod = CStr(XmlParametri.GetAttribute("ep_cod"))

                Dim sGrfi_Cod As String
                If XmlParametri.HasAttribute("grfi_cod") Then
                    sGrfi_Cod = CStr(XmlParametri.GetAttribute("grfi_cod"))
                    If sGrfi_Cod <> "" Then
                        Grfi_cod = sGrfi_Cod
                    Else
                        Grfi_cod = 0
                    End If
                End If

                Dim sStato_Impianto As String
                If XmlParametri.HasAttribute("stato_impianto") Then
                    sStato_Impianto = CStr(XmlParametri.GetAttribute("stato_impianto"))
                    If sStato_Impianto <> "" Then
                        Stato_Impianto = sStato_Impianto
                    Else
                        Stato_Impianto = 0
                    End If
                End If


                Copertura = ""
                If XmlParametri.HasAttribute("copertura") Then
                    Copertura = CStr(XmlParametri.GetAttribute("copertura"))
                End If

                FlagVisualizza_Commercio_Tutti_Revocati = 0
                If XmlParametri.HasAttribute("flagvisualizza_commercio_tutti_revocati") AndAlso
                   IsNumeric(XmlParametri.GetAttribute("flagvisualizza_commercio_tutti_revocati")) Then
                    FlagVisualizza_Commercio_Tutti_Revocati = CInt(XmlParametri.GetAttribute("flagvisualizza_commercio_tutti_revocati"))
                End If

                If XmlParametri.HasAttribute("listacomuni") Then
                    ListaComuni = XmlParametri.GetAttribute("listacomuni")
                End If

                If XmlParametri.HasAttribute("formulatixallegatinormative_idriga") AndAlso
                   IsNumeric(XmlParametri.GetAttribute("formulatixallegatinormative_idriga")) Then
                    FormulatiXAllegatiNormative_IDRiga = CInt(XmlParametri.GetAttribute("formulatixallegatinormative_idriga"))
                Else
                    FormulatiXAllegatiNormative_IDRiga = 0
                End If

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If

                Lingua_Cod = "1"
                If XmlParametri.HasAttribute("lingua_cod") AndAlso IsNumeric(XmlParametri.GetAttribute("lingua_cod")) Then
                    Lingua_Cod = CStr(XmlParametri.GetAttribute("lingua_cod"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub


    '###############################################################################################
    Public Sub AgroWS_XML_Parametri_Formulati_PrincipiAttivi_SpecieVegetali(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Pa_Cod As String,
                                    ByRef Veg_Cod As String,
                                    ByRef Class_Cod As String,
                                    ByRef DataRilievo As String,
                                    ByRef Flag_SoloAttivi As Boolean,
                                    ByRef Stato_Cod As String,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI pa_cod=""                        oppure  "239,150"
        '                  veg_cod=""                       oppure  "33,34,40"
        '                  class_cod=""                     oppure  "NULL,100,101,102,103"
        '                  datarilievo=""                   oppure  "18/08/2005"      per ora non usato !!!!
        '                  flag_soloattivi="true" />
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("pa_cod", CStr(Pa_Cod))
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("class_cod", CStr(Class_Cod))
                XmlParametri.SetAttribute("datarilievo", CStr(DataRilievo))
                XmlParametri.SetAttribute("flag_soloattivi", CStr(Flag_SoloAttivi))

                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Pa_Cod = CStr(XmlParametri.GetAttribute("pa_cod"))
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                Class_Cod = CStr(XmlParametri.GetAttribute("class_cod"))
                DataRilievo = CStr(XmlParametri.GetAttribute("datarilievo"))
                Flag_SoloAttivi = CBool(XmlParametri.GetAttribute("flag_soloattivi"))

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    Public Sub AgroWS_XML_Parametri_Formulati_PrincipiAttivi_Contesto(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Fr_Cod As String,
                                    ByRef pa_cod As String,
                                    ByRef Contesto As String,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150" />
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("pa_cod", CStr(pa_cod))
                XmlParametri.SetAttribute("contesto", CStr(Contesto))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))

                If XmlParametri.HasAttribute("contesto") Then
                    Contesto = XmlParametri.GetAttribute("contesto")
                Else
                    Contesto = "ContestoNonDefinito"
                End If

                If XmlParametri.HasAttribute("pa_cod") Then
                    pa_cod = XmlParametri.GetAttribute("pa_cod")
                End If
                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub


    '###############################################################################################
    Public Sub AgroWS_XML_Parametri_Formulati_PrincipiAttivi(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Fr_Cod As String,
                                    ByRef Stato_Cod As String,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150" />
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))

                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If


                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    '###############################################################################################
    Public Sub AgroWS_XML_Parametri_Formulati_Classificazioni(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Applicazione_Richiedente As Integer,
                                    ByRef Fr_Cod As String,
                                    ByRef FlagVisualizza_Commercio_Tutti_Revocati As String,
                                    ByRef Stato_Cod As String,
                                    ByRef Data As String,
                                    ByRef strFiltro As String,
                                    ByRef strSort As String,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150" />
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))
                XmlParametri.SetAttribute("data", CStr(Data))
                XmlParametri.SetAttribute("flagvisualizza_commercio_tutti_revocati", CStr(FlagVisualizza_Commercio_Tutti_Revocati))
                XmlParametri.SetAttribute("strfiltro", CStr(strFiltro))
                XmlParametri.SetAttribute("strsort", CStr(strSort))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                Data = CStr(XmlParametri.GetAttribute("data"))
                FlagVisualizza_Commercio_Tutti_Revocati = CStr(XmlParametri.GetAttribute("flagvisualizza_commercio_tutti_revocati"))
                strFiltro = CStr(XmlParametri.GetAttribute("strfiltro"))
                strSort = CStr(XmlParametri.GetAttribute("strsort"))

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    '###############################################################################################
    Public Sub AgroWS_XML_Parametri_Formulati_SpecieVegetali(
                                                            ByVal Operazione As enum_AWS_Xml,
                                                            ByRef StringaXML As String,
                                                            ByRef Fr_Cod As String,
                                                                ByRef Stato_Cod As String,
                                                            ByRef Veg_Cod As String,
                                                            ByRef Grfi_Cod As String,
                                                            ByRef Copertura As String,
                                                            ByRef For_Veg_Cod As String,
                                                            ByRef Data As String,
                                                            ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI for_veg_cod=""                   oppure  "100,101,102,103"
        '                  fr_cod=""                        oppure  "239,150"
        '                  veg_cod=""                       oppure  "33,34,40"
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("for_veg_cod", CStr(For_Veg_Cod))
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("data", CStr(Data))

                XmlParametri.SetAttribute("grfi_cod", CStr(Grfi_Cod))
                XmlParametri.SetAttribute("copertura", CStr(Copertura))

                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                For_Veg_Cod = CStr(XmlParametri.GetAttribute("for_veg_cod"))
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                If XmlParametri.HasAttribute("data") Then
                    Data = CStr(XmlParametri.GetAttribute("data"))
                Else
                    Data = ""
                End If

                If XmlParametri.HasAttribute("grfi_cod") Then
                    Grfi_Cod = CStr(XmlParametri.GetAttribute("grfi_cod"))
                Else
                    Grfi_Cod = "0"
                End If
                If XmlParametri.HasAttribute("copertura") Then
                    Copertura = CStr(XmlParametri.GetAttribute("copertura"))
                Else
                    Copertura = ""
                End If

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    Public Sub AgroWS_XML_Parametri_Formulati_SpecieVegetali_Intervallo(
                                                            ByVal Operazione As enum_AWS_Xml,
                                                            ByRef StringaXML As String,
                                                            ByRef Fr_Cod As String,
                                                            ByRef Veg_Cod As String,
                                                            ByRef Grfi_Cod As String,
                                                            ByRef Copertura As String,
                                                            ByRef For_Veg_Cod As String,
                                                            ByRef Data_Da As String,
                                                            ByRef Data_A As String,
                                                            ByRef Stato_Cod As String,
                                                            ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI for_veg_cod=""                   oppure  "100,101,102,103"
        '                  fr_cod=""                        oppure  "239,150"
        '                  veg_cod=""                       oppure  "33,34,40"
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("for_veg_cod", CStr(For_Veg_Cod))
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("data_da", CStr(Data_Da))
                XmlParametri.SetAttribute("data_a", CStr(Data_A))

                XmlParametri.SetAttribute("grfi_cod", CStr(Grfi_Cod))
                XmlParametri.SetAttribute("copertura", CStr(Copertura))

                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                For_Veg_Cod = CStr(XmlParametri.GetAttribute("for_veg_cod"))
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                If XmlParametri.HasAttribute("data_da") Then
                    Data_Da = CStr(XmlParametri.GetAttribute("data_da"))
                Else
                    Data_Da = ""
                End If
                If XmlParametri.HasAttribute("data_a") Then
                    Data_A = CStr(XmlParametri.GetAttribute("data_a"))
                Else
                    Data_A = ""
                End If

                If XmlParametri.HasAttribute("grfi_cod") Then
                    Grfi_Cod = CStr(XmlParametri.GetAttribute("grfi_cod"))
                Else
                    Grfi_Cod = "0"
                End If
                If XmlParametri.HasAttribute("copertura") Then
                    Copertura = CStr(XmlParametri.GetAttribute("copertura"))
                Else
                    Copertura = ""
                End If

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    '###############################################################################################
    Public Sub AgroWS_XML_Parametri_Formulati_SpecieVegetali_Avversita(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Fr_Cod As String,
                                    ByRef Veg_Cod As String,
                                    ByRef Grsp_Cod As String,
                                    ByRef Av_Cod As String,
                                    ByRef Av_Gru As String,
                                    ByRef For_Veg_Av_Cod As String,
                                    ByRef Data As String,
                                    ByRef Stato_Cod As String,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150"
        '                  veg_cod=""                       oppure  "33,34,40"
        '                  grsp_cod=""                      oppure  "33,34,40"
        '                  av_cod=""                        oppure  "230"
        '                  av_gru=""                        oppure  "9"
        '                  For_Veg_Av_Cod="" />             oppure  ""     
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("grsp_cod", CStr(Grsp_Cod))
                XmlParametri.SetAttribute("av_cod", CStr(Av_Cod))
                XmlParametri.SetAttribute("av_gru", CStr(Av_Gru))
                XmlParametri.SetAttribute("for_veg_av_cod", CStr(For_Veg_Av_Cod))
                XmlParametri.SetAttribute("data", CStr(Data))

                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                Grsp_Cod = CStr(XmlParametri.GetAttribute("grsp_cod"))
                Av_Cod = CStr(XmlParametri.GetAttribute("av_cod"))
                Av_Gru = CStr(XmlParametri.GetAttribute("av_gru"))
                For_Veg_Av_Cod = CStr(XmlParametri.GetAttribute("for_veg_av_cod"))
                If XmlParametri.HasAttribute("data") Then
                    Data = CStr(XmlParametri.GetAttribute("data"))
                Else
                    Data = ""
                End If

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    '###############################################################################################
    'Aggiunto il parametro Solo_Registrati
    Public Sub AgroWS_XML_Parametri_Formulati_SpecieVegetali_Avversita_2(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Fr_Cod As String,
                                    ByRef Veg_Cod As String,
                                    ByRef Grsp_Cod As String,
                                    ByRef Av_Cod As String,
                                    ByRef Av_Gru As String,
                                    ByRef For_Veg_Av_Cod As String,
                                    ByRef Solo_Registrati As String,
                                    ByRef Data As String,
                                     ByRef Stato_Cod As String,
                                   ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150"
        '                  veg_cod=""                       oppure  "33,34,40"
        '                  grsp_cod=""                      oppure  "33,34,40"
        '                  av_cod=""                        oppure  "230"
        '                  av_gru=""                        oppure  "9"
        '                  For_Veg_Av_Cod=""                oppure  ""     
        '                  Solo_Registrati="1" />           oppure  "0"     
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("grsp_cod", CStr(Grsp_Cod))
                XmlParametri.SetAttribute("av_cod", CStr(Av_Cod))
                XmlParametri.SetAttribute("av_gru", CStr(Av_Gru))
                XmlParametri.SetAttribute("for_veg_av_cod", CStr(For_Veg_Av_Cod))
                XmlParametri.SetAttribute("solo_registrati", CStr(Solo_Registrati))
                XmlParametri.SetAttribute("data", CStr(Data))

                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                Grsp_Cod = CStr(XmlParametri.GetAttribute("grsp_cod"))
                Av_Cod = CStr(XmlParametri.GetAttribute("av_cod"))
                Av_Gru = CStr(XmlParametri.GetAttribute("av_gru"))
                For_Veg_Av_Cod = CStr(XmlParametri.GetAttribute("for_veg_av_cod"))
                Solo_Registrati = CStr(XmlParametri.GetAttribute("solo_registrati"))
                If XmlParametri.HasAttribute("data") Then
                    Data = CStr(XmlParametri.GetAttribute("data"))
                Else
                    Data = ""
                End If

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    '###############################################################################################
    'Aggiunto il parametro Test_Ricerca e data
    Public Sub AgroWS_XML_Parametri_Formulati_SpecieVegetali_Avversita_3(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Fr_Cod As String,
                                    ByRef Veg_Cod As String,
                                    ByRef Grsp_Cod As String,
                                    ByRef Av_Cod As String,
                                    ByRef Av_Gru As String,
                                    ByRef For_Veg_Av_Cod As String,
                                    ByRef Solo_Registrati As String,
                                    ByRef Testo_Ricerca As String,
                                    ByRef Data As String,
                                     ByRef Stato_Cod As String,
                                   ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150"
        '                  veg_cod=""                       oppure  "33,34,40"
        '                  grsp_cod=""                      oppure  "33,34,40"
        '                  av_cod=""                        oppure  "230"
        '                  av_gru=""                        oppure  "9"
        '                  For_Veg_Av_Cod=""                oppure  ""     
        '                  Solo_Registrati="1"              oppure  "0"    
        '                  Testo_Ricerca=""                 oppure  ""     
        '                  data=""                          oppure  ""     
        '       />
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("grsp_cod", CStr(Grsp_Cod))
                XmlParametri.SetAttribute("av_cod", CStr(Av_Cod))
                XmlParametri.SetAttribute("av_gru", CStr(Av_Gru))
                XmlParametri.SetAttribute("for_veg_av_cod", CStr(For_Veg_Av_Cod))
                XmlParametri.SetAttribute("solo_registrati", CStr(Solo_Registrati))
                XmlParametri.SetAttribute("testo_ricerca", CStr(Testo_Ricerca))
                XmlParametri.SetAttribute("data", CStr(Data))

                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                Grsp_Cod = CStr(XmlParametri.GetAttribute("grsp_cod"))
                Av_Cod = CStr(XmlParametri.GetAttribute("av_cod"))
                Av_Gru = CStr(XmlParametri.GetAttribute("av_gru"))
                For_Veg_Av_Cod = CStr(XmlParametri.GetAttribute("for_veg_av_cod"))
                Solo_Registrati = CStr(XmlParametri.GetAttribute("solo_registrati"))
                Testo_Ricerca = CStr(XmlParametri.GetAttribute("testo_ricerca"))
                If XmlParametri.HasAttribute("data") Then
                    Data = CStr(XmlParametri.GetAttribute("data"))
                Else
                    Data = ""
                End If

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    Public Sub AgroWS_XML_Parametri_Avversita(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Fr_Cod As String,
                                    ByRef Veg_Cod As String,
                                    ByRef Id_RCDPI As String,
                                    ByRef Disciplinare_Cod As String,
                                    ByRef DPI_Privato_Pubblico As String,
                                    ByRef Tipo_Testata As String,
                                    ByRef Id_GaDPI As String,
                                    ByRef Av_Cod As String,
                                    ByRef Av_Gru As String,
                                    ByRef Pa_Cod As String,
                                    ByRef Gru_Pa_Cod As String,
                                    ByRef Id_Paa As String,
                                    ByRef str_Avversita As String,
                                    ByRef Modulo As String,
                                    ByRef Ep_Cod As String,
                                    ByRef FormulatiXAllegatiNormative_IDRiga As String,
                                    ByRef Storico As String,
                                    ByRef Lingua_Cod As String,
                                    ByRef str_Filtro As String,
                                    ByRef str_Sort As String,
                                    ByRef Data As String,
                                    ByRef ListaComuni As String,
                                    ByRef TipiFormulato As String,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("id_rcdpi", CStr(Id_RCDPI))
                XmlParametri.SetAttribute("disciplinare_cod", CStr(Disciplinare_Cod))
                XmlParametri.SetAttribute("dpi_privato_pubblico", CStr(DPI_Privato_Pubblico))
                XmlParametri.SetAttribute("tipo_testata", CStr(Tipo_Testata))
                XmlParametri.SetAttribute("id_gadpi", CStr(Id_GaDPI))
                XmlParametri.SetAttribute("av_cod", CStr(Av_Cod))
                XmlParametri.SetAttribute("av_gru", CStr(Av_Gru))
                XmlParametri.SetAttribute("pa_cod", CStr(Pa_Cod))
                XmlParametri.SetAttribute("gru_pa_cod", CStr(Gru_Pa_Cod))
                XmlParametri.SetAttribute("id_paa", CStr(Id_Paa))
                XmlParametri.SetAttribute("str_avversita", CStr(str_Avversita))
                XmlParametri.SetAttribute("modulo", CStr(Modulo))
                XmlParametri.SetAttribute("ep_cod", CStr(Ep_Cod))
                XmlParametri.SetAttribute("formulatixallegatinormative_idriga", CStr(FormulatiXAllegatiNormative_IDRiga))
                XmlParametri.SetAttribute("storico", CStr(Storico))
                XmlParametri.SetAttribute("str_filtro", CStr(str_Filtro))
                XmlParametri.SetAttribute("str_sort", CStr(str_Sort))
                XmlParametri.SetAttribute("listacomuni", CStr(ListaComuni))
                XmlParametri.SetAttribute("data", CStr(Data))

                XmlParametri.SetAttribute("lingua_cod", CStr(Lingua_Cod))

                XmlParametri.SetAttribute("tipi_formulato", CStr(TipiFormulato))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                Id_RCDPI = CStr(XmlParametri.GetAttribute("id_rcdpi"))
                Disciplinare_Cod = CStr(XmlParametri.GetAttribute("disciplinare_cod"))
                DPI_Privato_Pubblico = CStr(XmlParametri.GetAttribute("dpi_privato_pubblico"))
                Tipo_Testata = CStr(XmlParametri.GetAttribute("tipo_testata"))
                Id_GaDPI = CStr(XmlParametri.GetAttribute("id_gadpi"))
                Av_Cod = CStr(XmlParametri.GetAttribute("av_cod"))
                Av_Gru = CStr(XmlParametri.GetAttribute("av_gru"))
                Pa_Cod = CStr(XmlParametri.GetAttribute("pa_cod"))
                Gru_Pa_Cod = CStr(XmlParametri.GetAttribute("gru_pa_cod"))
                Id_Paa = CStr(XmlParametri.GetAttribute("id_paa"))
                str_Avversita = CStr(XmlParametri.GetAttribute("str_avversita"))
                Modulo = CStr(XmlParametri.GetAttribute("modulo"))
                Ep_Cod = CStr(XmlParametri.GetAttribute("ep_cod"))
                FormulatiXAllegatiNormative_IDRiga = CStr(XmlParametri.GetAttribute("formulatixallegatinormative_idriga"))
                Storico = CStr(XmlParametri.GetAttribute("storico"))
                str_Filtro = CStr(XmlParametri.GetAttribute("str_filtro"))
                str_Sort = CStr(XmlParametri.GetAttribute("str_sort"))
                Data = CStr(XmlParametri.GetAttribute("data"))

                Lingua_Cod = "1"
                If XmlParametri.HasAttribute("lingua_cod") Then
                    Lingua_Cod = CStr(XmlParametri.GetAttribute("lingua_cod"))
                End If

                If XmlParametri.HasAttribute("listacomuni") Then
                    ListaComuni = XmlParametri.GetAttribute("listacomuni")
                End If

                TipiFormulato = ""
                If XmlParametri.HasAttribute("tipi_formulato") Then
                    TipiFormulato = XmlParametri.GetAttribute("tipi_formulato")
                End If


                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    '###############################################################################################
    Public Sub AgroWS_XML_Parametri_Formulati_SpecieVegetali_Infestanti(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Fr_Cod As String,
                                    ByRef Veg_Cod As String,
                                    ByRef Grsp_Cod As String,
                                    ByRef Av_Cod As String,
                                    ByRef Av_Gru As String,
                                    ByRef For_Veg_Av_Cod As String,
                                    ByRef Data As String,
                                    ByRef Stato_Cod As String,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150"
        '                  veg_cod=""                       oppure  "33,34,40"
        '                  grsp_cod=""                      oppure  "33,34,40"
        '                  av_cod=""                        oppure  "230"
        '                  av_gru=""                        oppure  "9"
        '                  For_Veg_Av_Cod="" />             oppure  ""     
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("grsp_cod", CStr(Grsp_Cod))
                XmlParametri.SetAttribute("av_cod", CStr(Av_Cod))
                XmlParametri.SetAttribute("av_gru", CStr(Av_Gru))
                XmlParametri.SetAttribute("for_veg_av_cod", CStr(For_Veg_Av_Cod))
                XmlParametri.SetAttribute("data", CStr(Data))

                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                Grsp_Cod = CStr(XmlParametri.GetAttribute("grsp_cod"))
                Av_Cod = CStr(XmlParametri.GetAttribute("av_cod"))
                Av_Gru = CStr(XmlParametri.GetAttribute("av_gru"))
                For_Veg_Av_Cod = CStr(XmlParametri.GetAttribute("for_veg_av_cod"))
                If XmlParametri.HasAttribute("data") Then
                    Data = CStr(XmlParametri.GetAttribute("data"))
                Else
                    Data = ""
                End If

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub


    '###############################################################################################
    'Aggiunto il parametro Solo_Registrati
    Public Sub AgroWS_XML_Parametri_Formulati_SpecieVegetali_Infestanti_2(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Fr_Cod As String,
                                    ByRef Veg_Cod As String,
                                    ByRef Grsp_Cod As String,
                                    ByRef Av_Cod As String,
                                    ByRef Av_Gru As String,
                                    ByRef For_Veg_Av_Cod As String,
                                    ByRef Solo_Registrati As String,
                                    ByRef Data As String,
                                     ByRef Stato_Cod As String,
                                   ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150"
        '                  veg_cod=""                       oppure  "33,34,40"
        '                  grsp_cod=""                      oppure  "33,34,40"
        '                  av_cod=""                        oppure  "230"
        '                  av_gru=""                        oppure  "9"
        '                  For_Veg_Av_Cod="" />             oppure  ""     
        '                  Solo_Registrati="1" />           oppure  "0"     
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("grsp_cod", CStr(Grsp_Cod))
                XmlParametri.SetAttribute("av_cod", CStr(Av_Cod))
                XmlParametri.SetAttribute("av_gru", CStr(Av_Gru))
                XmlParametri.SetAttribute("for_veg_av_cod", CStr(For_Veg_Av_Cod))
                XmlParametri.SetAttribute("solo_registrati", CStr(Solo_Registrati))
                XmlParametri.SetAttribute("data", CStr(Data))

                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                Grsp_Cod = CStr(XmlParametri.GetAttribute("grsp_cod"))
                Av_Cod = CStr(XmlParametri.GetAttribute("av_cod"))
                Av_Gru = CStr(XmlParametri.GetAttribute("av_gru"))
                For_Veg_Av_Cod = CStr(XmlParametri.GetAttribute("for_veg_av_cod"))
                Solo_Registrati = CStr(XmlParametri.GetAttribute("solo_registrati"))
                If XmlParametri.HasAttribute("data") Then
                    Data = CStr(XmlParametri.GetAttribute("data"))
                Else
                    Data = ""
                End If

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    '###############################################################################################
    'Aggiunto il parametro Testo_Ricerca
    Public Sub AgroWS_XML_Parametri_Formulati_SpecieVegetali_Infestanti_3(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Fr_Cod As String,
                                    ByRef Veg_Cod As String,
                                    ByRef Grsp_Cod As String,
                                    ByRef Av_Cod As String,
                                    ByRef Av_Gru As String,
                                    ByRef For_Veg_Av_Cod As String,
                                    ByRef Solo_Registrati As String,
                                    ByRef Testo_Ricerca As String,
                                    ByRef Data As String,
                                    ByRef Stato_Cod As String,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150"
        '                  veg_cod=""                       oppure  "33,34,40"
        '                  grsp_cod=""                      oppure  "33,34,40"
        '                  av_cod=""                        oppure  "230"
        '                  av_gru=""                        oppure  "9"
        '                  For_Veg_Av_Cod=""                oppure  ""     
        '                  Solo_Registrati="1"              oppure  "0"    
        '                  Testo_Ricerca=""                 oppure  ""     
        '                  data=""                          oppure  ""     
        '       />
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("grsp_cod", CStr(Grsp_Cod))
                XmlParametri.SetAttribute("av_cod", CStr(Av_Cod))
                XmlParametri.SetAttribute("av_gru", CStr(Av_Gru))
                XmlParametri.SetAttribute("for_veg_av_cod", CStr(For_Veg_Av_Cod))
                XmlParametri.SetAttribute("solo_registrati", CStr(Solo_Registrati))
                XmlParametri.SetAttribute("testo_ricerca", CStr(Testo_Ricerca))
                XmlParametri.SetAttribute("data", CStr(Data))

                XmlParametri.SetAttribute("stato_cod", CStr(Stato_Cod))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                Grsp_Cod = CStr(XmlParametri.GetAttribute("grsp_cod"))
                Av_Cod = CStr(XmlParametri.GetAttribute("av_cod"))
                Av_Gru = CStr(XmlParametri.GetAttribute("av_gru"))
                For_Veg_Av_Cod = CStr(XmlParametri.GetAttribute("for_veg_av_cod"))
                Solo_Registrati = CStr(XmlParametri.GetAttribute("solo_registrati"))
                Testo_Ricerca = CStr(XmlParametri.GetAttribute("testo_ricerca"))
                If XmlParametri.HasAttribute("data") Then
                    Data = CStr(XmlParametri.GetAttribute("data"))
                Else
                    Data = ""
                End If

                Stato_Cod = "IT"
                If XmlParametri.HasAttribute("stato_cod") Then
                    Stato_Cod = CStr(XmlParametri.GetAttribute("stato_cod"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    '###############################################################################################
    Public Sub AgroWS_XML_Parametri_Formulati_SpecieVegetali_Infestanti_Dosi(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Fr_Cod As String,
                                    ByRef Veg_Cod As String,
                                    ByRef Av_Cod As String,
                                    ByRef Av_Gru As String,
                                    ByRef grfi_cod As String,
                                    ByRef For_Veg_Av_Dos_Cod As String,
                                    ByRef FormulatiXAllegatiNormative_IDRiga As String,
                                    ByRef Copertura As String,
                                    ByRef Data As String,
                                    ByRef TipoRichiesto As String,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150"
        '                  veg_cod=""                       oppure  "33,34,40"
        '                  av_cod=""                        oppure  "230"
        '                  av_gru=""                        oppure  "9"
        '                  grfi_cod=""                      oppure  "2"
        '                  For_Veg_Av_Dos_Cod="" />         oppure  ""      
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("av_cod", CStr(Av_Cod))
                XmlParametri.SetAttribute("av_gru", CStr(Av_Gru))
                XmlParametri.SetAttribute("grfi_cod", CStr(grfi_cod))
                XmlParametri.SetAttribute("for_veg_av_dos_cod", CStr(For_Veg_Av_Dos_Cod))
                XmlParametri.SetAttribute("formulatixallegatinormative_idriga", CStr(FormulatiXAllegatiNormative_IDRiga))
                XmlParametri.SetAttribute("copertura", CStr(Copertura))
                XmlParametri.SetAttribute("data", CStr(Data))

                XmlParametri.SetAttribute("tipo_richiesto", CStr(TipoRichiesto))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                Av_Cod = CStr(XmlParametri.GetAttribute("av_cod"))
                Av_Gru = CStr(XmlParametri.GetAttribute("av_gru"))

                If XmlParametri.HasAttribute("grfi_cod") Then
                    grfi_cod = CStr(XmlParametri.GetAttribute("grfi_cod"))
                    If grfi_cod = "" Then
                        grfi_cod = 0
                    End If
                End If
                If XmlParametri.HasAttribute("for_veg_av_dos_cod") Then
                    For_Veg_Av_Dos_Cod = CStr(XmlParametri.GetAttribute("for_veg_av_dos_cod"))
                    If For_Veg_Av_Dos_Cod = "" Then
                        For_Veg_Av_Dos_Cod = "0"
                    End If
                Else
                    For_Veg_Av_Dos_Cod = "0"
                End If
                If XmlParametri.HasAttribute("data") Then
                    Data = CStr(XmlParametri.GetAttribute("data"))
                Else
                    Data = ""
                End If

                FormulatiXAllegatiNormative_IDRiga = "-1"
                If XmlParametri.HasAttribute("formulatixallegatinormative_idriga") Then
                    FormulatiXAllegatiNormative_IDRiga = CStr(XmlParametri.GetAttribute("formulatixallegatinormative_idriga"))
                    If FormulatiXAllegatiNormative_IDRiga = "" Then
                        FormulatiXAllegatiNormative_IDRiga = "0"
                    End If
                End If

                Copertura = ""
                If XmlParametri.HasAttribute("copertura") Then
                    Copertura = CStr(XmlParametri.GetAttribute("copertura"))
                End If

                TipoRichiesto = "0"
                If XmlParametri.HasAttribute("tipo_richiesto") Then
                    TipoRichiesto = CStr(XmlParametri.GetAttribute("tipo_richiesto"))
                End If

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub






    '###############################################################################################
    Public Sub AgroWS_XML_Parametri_Formulati_SpecieVegetali_Avversita_Dosi(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Fr_Cod As String,
                                    ByRef Veg_Cod As String,
                                    ByRef Av_Cod As String,
                                    ByRef Av_Gru As String,
                                    ByRef grfi_cod As String,
                                    ByRef For_Veg_Av_Dos_Cod As String,
                                    ByRef FormulatiXAllegatiNormative_IDRiga As String,
                                    ByRef Copertura As String,
                                    ByRef Data As String,
                                    ByRef TipoRichiesto As String,
                                    ByRef Errore As String
                                    )

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150"
        '                  veg_cod=""                       oppure  "33,34,40"
        '                  av_cod=""                        oppure  "230"
        '                  av_gru=""                        oppure  "9"
        '                   grfi_cod = ""                   oppure  "2"
        '                  For_Veg_Av_Dos_Cod="" />         oppure  ""      
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlParametri.SetAttribute("av_cod", CStr(Av_Cod))
                XmlParametri.SetAttribute("av_gru", CStr(Av_Gru))
                XmlParametri.SetAttribute("grfi_cod", CStr(grfi_cod))
                XmlParametri.SetAttribute("for_veg_av_dos_cod", CStr(For_Veg_Av_Dos_Cod))
                XmlParametri.SetAttribute("formulatixallegatinormative_idriga", CStr(FormulatiXAllegatiNormative_IDRiga))
                XmlParametri.SetAttribute("copertura", CStr(Copertura))
                XmlParametri.SetAttribute("data", CStr(Data))

                XmlParametri.SetAttribute("tipo_richiesto", CStr(TipoRichiesto))


                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                Av_Cod = CStr(XmlParametri.GetAttribute("av_cod"))
                Av_Gru = CStr(XmlParametri.GetAttribute("av_gru"))
                If XmlParametri.HasAttribute("grfi_cod") Then
                    grfi_cod = CStr(XmlParametri.GetAttribute("grfi_cod"))
                    If grfi_cod = "" Then
                        grfi_cod = "0"
                    End If
                End If
                If XmlParametri.HasAttribute("for_veg_av_dos_cod") Then
                    For_Veg_Av_Dos_Cod = CStr(XmlParametri.GetAttribute("for_veg_av_dos_cod"))
                    If For_Veg_Av_Dos_Cod = "" Then
                        For_Veg_Av_Dos_Cod = "0"
                    End If
                Else
                    For_Veg_Av_Dos_Cod = "0"
                End If
                If XmlParametri.HasAttribute("data") Then
                    Data = CStr(XmlParametri.GetAttribute("data"))
                Else
                    Data = ""
                End If

                FormulatiXAllegatiNormative_IDRiga = "-1"
                If XmlParametri.HasAttribute("formulatixallegatinormative_idriga") Then
                    FormulatiXAllegatiNormative_IDRiga = CStr(XmlParametri.GetAttribute("formulatixallegatinormative_idriga"))
                    If FormulatiXAllegatiNormative_IDRiga = "" Then
                        FormulatiXAllegatiNormative_IDRiga = "0"
                    End If
                End If

                Copertura = ""
                If XmlParametri.HasAttribute("copertura") Then
                    Copertura = CStr(XmlParametri.GetAttribute("copertura"))
                End If

                TipoRichiesto = "0"
                If XmlParametri.HasAttribute("tipo_richiesto") Then
                    TipoRichiesto = CStr(XmlParametri.GetAttribute("tipo_richiesto"))
                End If


                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub





    '###############################################################################################
    '###############################################################################################
    '###############################################################################################
    '###############################################################################################
    '###############################################################################################
    '###############################################################################################


    '###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulati(ByVal Operazione As enum_AWS_Xml,
                                              ByRef StringaXML As String,
                                              ByRef DtRisultati As DataTable,
                                              ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlDati As Xml.XmlElement
        Dim XmlNodi As Xml.XmlNodeList
        Dim i As Integer

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <DATI cod_disciplinare="..." fr_des="..." cltoss="... " pa_des="..." 
        '             revocato="..." data_revo="..." sospeso="..." data_sosp="..." termine="..." data_term="..." 
        '             data_fine_comm="..." data_fine_usoscorte="..."/>
        '       <DATI cod_disciplinare="..." fr_des="..." cltoss="... " pa_des="..."
        '             revocato="..." data_revo="..." sospeso="..." data_sosp="..." termine="..." data_term="..." 
        '             data_fine_comm="..." data_fine_usoscorte="..."/>
        '       <DATI cod_disciplinare="..." fr_des="..." cltoss="... " pa_des="..."
        '             revocato="..." data_revo="..." sospeso="..." data_sosp="..." termine="..." data_term="..." 
        '             data_fine_comm="..." data_fine_usoscorte="..."/>
        '   </RISULTATI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                'Se il recordset e' pieno ...
                If (Not IsNothing(DtRisultati)) Then

                    'Estraggo le informazioni dal recordset
                    For i = 0 To DtRisultati.Rows.Count - 1

                        '---------------------------------
                        'Creo il nodo DATI
                        XmlDati = XmlDoc.CreateElement("DATI")

                        'Imposto gli attributi
                        XmlDati.SetAttribute("fr_cod", DtRisultati.Rows(i).Item("Fr_Cod").ToString)
                        XmlDati.SetAttribute("fr_des", DtRisultati.Rows(i).Item("Fr_Des").ToString)
                        XmlDati.SetAttribute("revocato", DtRisultati.Rows(i).Item("revocato").ToString)
                        XmlDati.SetAttribute("data_revo", DtRisultati.Rows(i).Item("data_revo").ToString)
                        XmlDati.SetAttribute("sospeso", DtRisultati.Rows(i).Item("sospeso").ToString)
                        XmlDati.SetAttribute("data_sosp", DtRisultati.Rows(i).Item("data_sosp").ToString)
                        XmlDati.SetAttribute("termine", DtRisultati.Rows(i).Item("termine").ToString)
                        XmlDati.SetAttribute("data_term", DtRisultati.Rows(i).Item("data_term").ToString)
                        XmlDati.SetAttribute("data_fine_comm", DtRisultati.Rows(i).Item("Data_Fine_Comm").ToString)
                        XmlDati.SetAttribute("data_fine_usoscorte", DtRisultati.Rows(i).Item("Data_Fine_UsoScorte").ToString)
                        XmlDati.SetAttribute("cltoss", DtRisultati.Rows(i).Item("NewCLTOSS_COD").ToString)
                        XmlDati.SetAttribute("pa_des", DtRisultati.Rows(i).Item("PrincipioAttivo").ToString)

                        'Imposto XmlDati come figlio del nodo XmlRisultati
                        XmlRisultati.AppendChild(XmlDati)

                        '---------------------------------
                    Next

                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlNodi = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                ''Carico la stringa XML nel documento
                'XmlDoc.LoadXml(StringaXML)

                ''Prelevo il nodo XmlParametri
                'XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                ''Prelevo gli attributi
                'Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                'Fr_Des = CStr(XmlParametri.GetAttribute("fr_des"))
                'Class_Cod = CStr(XmlParametri.GetAttribute("class_cod"))
                'DataRilievo = CStr(XmlParametri.GetAttribute("datarilievo"))
                'Flag_SoloAttivi = CBool(XmlParametri.GetAttribute("flag_soloattivi"))

                ''Distruggo gli oggetti
                'XmlNodo = Nothing
                'XmlParametri = Nothing
                'XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub




    '###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulati_conDosi(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Dt As DataTable,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlFormulato As Xml.XmlElement
        Dim XmlNodi As Xml.XmlNodeList
        Dim i As Integer

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <FORMULATO fr_cod="..." fr_des="..." data_reg="..."  class_cod="..." class_des="..."
        '                  revocato="..." data_revo="..." sospeso="..." data_sosp="..." termine="..." data_term="..." 
        '                  data_fine_comm="..." data_fine_usoscorte="..." normeprecauzionali="..."  sempl_comp="..." 
        '                  pa_cod="..." pa_des="..." titolo="..."  tempocarenza="..." 
        '                  dose_min="..." dose_max="..." udm_cod="..."  udm_sim="..." 
        '                  ff_cod="..." da_ff_cod="..." a_ff_cod="..." da_epoca_1="..." a_epoca_1="..." da_epoca_2="..." a_epoca_2="..." 
        '                  acqua_min="..." acqua_max="..." acqua_udm_cod="..." acqua_udm_sim="..." />
        '   </RISULTATI>
        '===========================================================================================
        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                'Se il recordset e' pieno ...
                If (Not IsNothing(Dt)) Then

                    'Estraggo le informazioni dal recordset
                    For i = 0 To Dt.Rows.Count - 1

                        'Creo il nodo FORMULATO
                        XmlFormulato = XmlDoc.CreateElement("FORMULATO")

                        XmlRisultati.AppendChild(XmlFormulato)

                        'Imposto gli attributi
                        XmlFormulato.SetAttribute(LCase("Fr_Cod"), Dt.Rows(i).Item("Fr_Cod").ToString)
                        XmlFormulato.SetAttribute(LCase("Fr_Des"), Dt.Rows(i).Item("Fr_Des").ToString)
                        XmlFormulato.SetAttribute(LCase("Data_Reg"), Dt.Rows(i).Item("data_reg").ToString)
                        XmlFormulato.SetAttribute(LCase("Class_Cod"), Dt.Rows(i).Item("Class_Cod").ToString)
                        XmlFormulato.SetAttribute(LCase("Class_Des"), Dt.Rows(i).Item("Class_Des").ToString)
                        XmlFormulato.SetAttribute(LCase("Revocato"), Dt.Rows(i).Item("revocato").ToString)
                        XmlFormulato.SetAttribute(LCase("Data_Revo"), Dt.Rows(i).Item("data_revo").ToString)
                        XmlFormulato.SetAttribute(LCase("Sospeso"), Dt.Rows(i).Item("sospeso").ToString)
                        XmlFormulato.SetAttribute(LCase("Data_Sosp"), Dt.Rows(i).Item("data_sosp").ToString)
                        XmlFormulato.SetAttribute(LCase("Termine"), Dt.Rows(i).Item("termine").ToString)
                        XmlFormulato.SetAttribute(LCase("Data_Term"), Dt.Rows(i).Item("data_term").ToString)
                        XmlFormulato.SetAttribute(LCase("Data_Fine_Comm"), Dt.Rows(i).Item("Data_Fine_Comm").ToString)
                        XmlFormulato.SetAttribute(LCase("Data_Fine_UsoScorte"), Dt.Rows(i).Item("Data_Fine_UsoScorte").ToString)
                        XmlFormulato.SetAttribute(LCase("Pa_Cod"), Dt.Rows(i).Item("Pa_Cod").ToString)
                        XmlFormulato.SetAttribute(LCase("Pa_Des"), Dt.Rows(i).Item("Pa_Des").ToString)
                        XmlFormulato.SetAttribute(LCase("Titolo"), Dt.Rows(i).Item("Titolo").ToString)
                        'XmlFormulato.SetAttribute(LCase("Peso"), Dt.Rows(i).Item("Peso").ToString)
                        XmlFormulato.SetAttribute(LCase("TempoCarenza"), Dt.Rows(i).Item("TempoCarenza").ToString)
                        XmlFormulato.SetAttribute(LCase("Dose_Min"), Math.Round(Dt.Rows(i).Item("Dose_Min"), 3).ToString)
                        XmlFormulato.SetAttribute(LCase("Dose_Max"), Math.Round(Dt.Rows(i).Item("Dose_Max"), 3).ToString)
                        XmlFormulato.SetAttribute(LCase("Udm_Cod"), Dt.Rows(i).Item("Udm_Cod").ToString)
                        XmlFormulato.SetAttribute(LCase("Udm_Sim"), Dt.Rows(i).Item("Udm_Sim").ToString)

                        XmlFormulato.SetAttribute(LCase("ff_cod"), Dt.Rows(i).Item("ff_cod").ToString)
                        XmlFormulato.SetAttribute(LCase("da_ff_cod"), Dt.Rows(i).Item("da_ff_cod").ToString)
                        XmlFormulato.SetAttribute(LCase("a_ff_cod"), Dt.Rows(i).Item("a_ff_cod").ToString)
                        XmlFormulato.SetAttribute(LCase("da_epoca_1"), Dt.Rows(i).Item("da_epoca_1").ToString)
                        XmlFormulato.SetAttribute(LCase("a_epoca_1"), Dt.Rows(i).Item("a_epoca_1").ToString)
                        XmlFormulato.SetAttribute(LCase("da_epoca_2"), Dt.Rows(i).Item("da_epoca_2").ToString)
                        XmlFormulato.SetAttribute(LCase("a_epoca_2"), Dt.Rows(i).Item("a_epoca_2").ToString)
                        XmlFormulato.SetAttribute(LCase("acqua_min"), Dt.Rows(i).Item("acqua_min").ToString)
                        XmlFormulato.SetAttribute(LCase("acqua_max"), Dt.Rows(i).Item("acqua_max").ToString)
                        XmlFormulato.SetAttribute(LCase("acqua_udm_cod"), Dt.Rows(i).Item("acqua_udm_cod").ToString)
                        XmlFormulato.SetAttribute(LCase("acqua_udm_sim"), Dt.Rows(i).Item("acqua_udm_sim").ToString)


                    Next

                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlNodi = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                ''Carico la stringa XML nel documento
                'XmlDoc.LoadXml(StringaXML)

                ''Prelevo il nodo XmlParametri
                'XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                ''Prelevo gli attributi
                'Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                'Fr_Des = CStr(XmlParametri.GetAttribute("fr_des"))
                'Class_Cod = CStr(XmlParametri.GetAttribute("class_cod"))
                'DataRilievo = CStr(XmlParametri.GetAttribute("datarilievo"))
                'Flag_SoloAttivi = CBool(XmlParametri.GetAttribute("flag_soloattivi"))

                ''Distruggo gli oggetti
                'XmlNodo = Nothing
                'XmlParametri = Nothing
                'XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub



    '###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulati_conDosi_Multiple(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef Dt As DataTable,
                                    ByVal EsportaDosi As Boolean,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlFormulato As Xml.XmlElement
        Dim XmlDose As Xml.XmlElement
        Dim XmlNodi As Xml.XmlNodeList
        Dim i As Integer

        '===========================================================================================
        '   <RISULTATI errore="...">
        '        <FORMULATO fr_cod="..." fr_des="..." class_cod="..." class_des="..." cls_toss_cod="..." data_reg="..." revocato="..." data_sosp="..." termine="..." data_term="..." 
        '                        data_fine_comm="..." data_fine_usoscorte="..." tempocarenza="..." pa_cod="..." pa_des="..." titolo="..." />
        '           <DOSE min="..." max="..." udm_cod="..." udm_sim="..." ff_cod="..." da_ff_cod="..." a_ff_cod="..." da_epoca_1="..." a_epoca_1="..."
        '                               da_epoca_2="..." a_epoca_2="..." acqua_min="..." acqua_max="..." acqua_udm_cod="..." acqua_udm_sim="..." />
        '           <DOSE min="..." max="..." udm_cod="..." udm_sim="..." da_epoca_1="..." a_epoca_1="..."
        '                                acqua_min="..." acqua_max="..." acqua_udm_cod="..." acqua_udm_sim="..." />
        '       </FORMULATO>
        '   </RISULTATI>

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                'Se il recordset e' pieno ...
                If (Not IsNothing(Dt)) Then

                    'Estraggo le informazioni dal recordset
                    'per ogni pa_cod diverso
                    Dim fr_cod_distinct As String() = SelectDistinct(Dt, "Fr_Cod")

                    For i = 0 To fr_cod_distinct.Length - 1

                        'filtro su fr_cod
                        Dim dr_app As DataRow()
                        dr_app = Dt.Select("Fr_cod=" & fr_cod_distinct(i))


                        XmlFormulato = XmlDoc.CreateElement("FORMULATO")
                        XmlFormulato.SetAttribute(LCase("Fr_Cod"), dr_app(0).Item("Fr_Cod").ToString)
                        XmlFormulato.SetAttribute(LCase("fr_des"), dr_app(0).Item("fr_des").ToString)

                        XmlFormulato.SetAttribute(LCase("Class_Cod"), dr_app(0).Item("Class_Cod").ToString)
                        XmlFormulato.SetAttribute(LCase("Class_Des"), dr_app(0).Item("Class_Des").ToString)
                        XmlFormulato.SetAttribute(LCase("Data_Reg"), dr_app(0).Item("data_reg").ToString)
                        XmlFormulato.SetAttribute(LCase("Revocato"), dr_app(0).Item("revocato").ToString)
                        XmlFormulato.SetAttribute(LCase("Data_Revo"), dr_app(0).Item("data_revo").ToString)
                        XmlFormulato.SetAttribute(LCase("Sospeso"), dr_app(0).Item("sospeso").ToString)
                        XmlFormulato.SetAttribute(LCase("Data_Sosp"), dr_app(0).Item("data_sosp").ToString)
                        XmlFormulato.SetAttribute(LCase("Termine"), dr_app(0).Item("termine").ToString)
                        XmlFormulato.SetAttribute(LCase("Data_Term"), dr_app(0).Item("data_term").ToString)
                        XmlFormulato.SetAttribute(LCase("Data_Fine_Comm"), dr_app(0).Item("Data_Fine_Comm").ToString)
                        XmlFormulato.SetAttribute(LCase("Data_Fine_UsoScorte"), dr_app(0).Item("Data_Fine_UsoScorte").ToString)
                        XmlFormulato.SetAttribute(LCase("Pa_Cod"), dr_app(0).Item("Pa_Cod").ToString)
                        XmlFormulato.SetAttribute(LCase("Pa_Des"), dr_app(0).Item("Pa_Des").ToString)
                        XmlFormulato.SetAttribute(LCase("Titolo"), dr_app(0).Item("Titolo").ToString)
                        XmlFormulato.SetAttribute(LCase("Peso"), dr_app(0).Item("Peso").ToString)
                        XmlFormulato.SetAttribute(LCase("TempoCarenza"), dr_app(0).Item("TempoCarenza").ToString)

                        XmlFormulato.SetAttribute(LCase("cltoss_cod"), dr_app(0).Item("cltoss_cod").ToString)
                        XmlFormulato.SetAttribute(LCase("cltoss_des"), dr_app(0).Item("cltoss_des").ToString)
                        XmlFormulato.SetAttribute(LCase("gradotossicita"), dr_app(0).Item("gradotossicita").ToString)

                        If EsportaDosi Then
                            Dim j As Integer
                            For j = 0 To dr_app.Length - 1
                                XmlDose = XmlDoc.CreateElement("DOSE")

                                XmlDose.SetAttribute(LCase("Dose_Min"), Math.Round(dr_app(j).Item("Dose_Min"), 3).ToString)
                                XmlDose.SetAttribute(LCase("Dose_Max"), Math.Round(dr_app(j).Item("Dose_Max"), 3).ToString)
                                XmlDose.SetAttribute(LCase("Udm_Cod"), dr_app(j).Item("Udm_Cod").ToString)
                                XmlDose.SetAttribute(LCase("Udm_Sim"), dr_app(j).Item("Udm_Sim").ToString)
                                XmlDose.SetAttribute(LCase("da_epoca_1"), dr_app(j).Item("da_epoca_1").ToString)
                                XmlDose.SetAttribute(LCase("a_epoca_1"), dr_app(j).Item("a_epoca_1").ToString)
                                XmlDose.SetAttribute(LCase("acqua_min"), dr_app(j).Item("acqua_min").ToString)
                                XmlDose.SetAttribute(LCase("acqua_max"), dr_app(j).Item("acqua_max").ToString)
                                XmlDose.SetAttribute(LCase("acqua_udm_cod"), dr_app(j).Item("acqua_udm_cod").ToString)
                                XmlDose.SetAttribute(LCase("acqua_udm_sim"), dr_app(j).Item("acqua_udm_sim").ToString)
                                'parte che aggiunge l'acqua min max da epoca a epoca
                                XmlFormulato.AppendChild(XmlDose)
                            Next
                        End If

                        XmlRisultati.AppendChild(XmlFormulato)

                    Next

                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlNodi = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                ''Carico la stringa XML nel documento
                'XmlDoc.LoadXml(StringaXML)

                ''Prelevo il nodo XmlParametri
                'XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                ''Prelevo gli attributi
                'Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                'Fr_Des = CStr(XmlParametri.GetAttribute("fr_des"))
                'Class_Cod = CStr(XmlParametri.GetAttribute("class_cod"))
                'DataRilievo = CStr(XmlParametri.GetAttribute("datarilievo"))
                'Flag_SoloAttivi = CBool(XmlParametri.GetAttribute("flag_soloattivi"))

                ''Distruggo gli oggetti
                'XmlNodo = Nothing
                'XmlParametri = Nothing
                'XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub



    '###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulati_PrincipiAttivi_SpecieVegetali(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef DtRisultati As DataTable,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlDati As Xml.XmlElement
        Dim XmlNodi As Xml.XmlNodeList
        Dim i As Integer

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <DATI fr_cod="..." fr_des="..." cltoss="..." 
        '             pa_cod="..." pa_des="..." titolo="..."
        '             revocato="..." data_revo="..." sospeso="..." data_sosp="..." termine="..." data_term="..." 
        '             data_fine_comm="..." data_fine_usoscorte="..."
        '             veg_cod="..." veg_des="..." tempocarenza="..." />
        '       <DATI fr_cod="..." fr_des="..." cltoss="..." 
        '             pa_cod="..." pa_des="..." titolo="..."
        '             revocato="..." data_revo="..." sospeso="..." data_sosp="..." termine="..." data_term="..." 
        '             data_fine_comm="..." data_fine_usoscorte="..."
        '             veg_cod="..." veg_des="..." tempocarenza="..." />
        '   </RISULTATI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                'Se il recordset e' pieno ...
                If (Not IsNothing(DtRisultati)) Then

                    For i = 0 To DtRisultati.Rows.Count - 1

                        '---------------------------------

                        'Creo il nodo DATI
                        XmlDati = XmlDoc.CreateElement("DATI")

                        'Imposto gli attributi
                        XmlDati.SetAttribute("fr_cod", DtRisultati.Rows(i).Item("Fr_Cod").ToString)
                        XmlDati.SetAttribute("fr_des", DtRisultati.Rows(i).Item("Fr_Des").ToString)
                        XmlDati.SetAttribute("cltoss", DtRisultati.Rows(i).Item("NewCLTOSS_COD").ToString)

                        XmlDati.SetAttribute("revocato", DtRisultati.Rows(i).Item("revocato").ToString)
                        XmlDati.SetAttribute("data_revo", DtRisultati.Rows(i).Item("data_revo").ToString)
                        XmlDati.SetAttribute("sospeso", DtRisultati.Rows(i).Item("sospeso").ToString)
                        XmlDati.SetAttribute("data_sosp", DtRisultati.Rows(i).Item("data_sosp").ToString)
                        XmlDati.SetAttribute("termine", DtRisultati.Rows(i).Item("termine").ToString)
                        XmlDati.SetAttribute("data_term", DtRisultati.Rows(i).Item("data_term").ToString)
                        XmlDati.SetAttribute("data_fine_comm", DtRisultati.Rows(i).Item("Data_Fine_Comm").ToString)
                        XmlDati.SetAttribute("data_fine_usoscorte", DtRisultati.Rows(i).Item("Data_Fine_UsoScorte").ToString)

                        XmlDati.SetAttribute("pa_cod", DtRisultati.Rows(i).Item("Pa_Cod").ToString)
                        XmlDati.SetAttribute("pa_des", DtRisultati.Rows(i).Item("Pa_Des").ToString)
                        XmlDati.SetAttribute("titolo", DtRisultati.Rows(i).Item("Titolo").ToString)
                        XmlDati.SetAttribute("peso", DtRisultati.Rows(i).Item("peso").ToString)

                        XmlDati.SetAttribute("veg_cod", DtRisultati.Rows(i).Item("Veg_Cod").ToString)
                        XmlDati.SetAttribute("veg_des", DtRisultati.Rows(i).Item("Veg_Des").ToString)
                        XmlDati.SetAttribute("tempocarenza", DtRisultati.Rows(i).Item("TempoCarenza").ToString)

                        'Imposto XmlDati come figlio del nodo XmlRisultati
                        XmlRisultati.AppendChild(XmlDati)

                        '---------------------------------

                    Next

                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlNodi = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                ''Carico la stringa XML nel documento
                'XmlDoc.LoadXml(StringaXML)

                ''Prelevo il nodo XmlParametri
                'XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                ''Prelevo gli attributi
                'Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                'Fr_Des = CStr(XmlParametri.GetAttribute("fr_des"))
                'Class_Cod = CStr(XmlParametri.GetAttribute("class_cod"))
                'DataRilievo = CStr(XmlParametri.GetAttribute("datarilievo"))
                'Flag_SoloAttivi = CBool(XmlParametri.GetAttribute("flag_soloattivi"))

                ''Distruggo gli oggetti
                'XmlNodo = Nothing
                'XmlParametri = Nothing
                'XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub



    '###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulati_PrincipiAttivi(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef DtRisultati As DataTable,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlFormulato As Xml.XmlElement
        Dim XmlPrincipioAttivo As Xml.XmlElement
        Dim i, j As Integer

        Dim XMLs_Formulati As Xml.XmlNodeList
        Dim XMLs_Principi As Xml.XmlNodeList
        Dim Dr As DataRow

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <FORMULATO fr_cod="..." fr_des="..." data_reg="..."  
        '                  revocato="..." data_revo="..." sospeso="..." data_sosp="..." termine="..." data_term="..." 
        '                  data_fine_comm="..." data_fine_usoscorte="..." normeprecauzionali="..."  sempl_comp="..." />
        '             <PRINCIPIO_ATTIVO pa_cod="..." pa_des="..." titolo="..."  />
        '             <PRINCIPIO_ATTIVO pa_cod="..." pa_des="..." titolo="..."  />
        '       <FORMULATO fr_cod="..." fr_des="..." data_reg="..." 
        '                  revocato="..." data_revo="..." sospeso="..." data_sosp="..." termine="..." data_term="..." 
        '                  data_fine_comm="..." data_fine_usoscorte="..." normeprecauzionali="..."  sempl_comp="..." />
        '             <PRINCIPIO_ATTIVO pa_cod="..." pa_des="..." titolo="..."  />
        '             <PRINCIPIO_ATTIVO pa_cod="..." pa_des="..." titolo="..."  />
        '   </RISULTATI>

        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                'Se il recordset è pieno ...
                If (Not IsNothing(DtRisultati)) Then

                    'Dim ObjUtility As New AgronicaCoreUtility.DatatableUtility
                    Dim strFrCod As String() = SelectDistinct(DtRisultati, "Fr_Cod")


                    If strFrCod IsNot Nothing Then

                        Dim Fr_Cod As String
                        Dim DrFr_Cod As DataRow()

                        For i = 0 To strFrCod.Length - 1

                            Fr_Cod = strFrCod(i)

                            DrFr_Cod = DtRisultati.Select("fr_cod=" & Fr_Cod)

                            If DrFr_Cod IsNot Nothing AndAlso DrFr_Cod.Length > 0 Then

                                XmlFormulato = XmlDoc.CreateElement("FORMULATO")
                                XmlRisultati.AppendChild(XmlFormulato)

                                XmlFormulato.SetAttribute("fr_cod", DrFr_Cod(0).Item("Fr_Cod").ToString)
                                XmlFormulato.SetAttribute("fr_des", DrFr_Cod(0).Item("Fr_Des").ToString)

                                For j = 0 To DrFr_Cod.Length - 1

                                    If j = 0 Then

                                        XmlFormulato.SetAttribute("revocato", DrFr_Cod(j).Item("revocato").ToString)
                                        XmlFormulato.SetAttribute("data_revo", DrFr_Cod(j).Item("data_revo").ToString)
                                        XmlFormulato.SetAttribute("sospeso", DrFr_Cod(j).Item("sospeso").ToString)
                                        XmlFormulato.SetAttribute("data_sosp", DrFr_Cod(j).Item("data_sosp").ToString)
                                        XmlFormulato.SetAttribute("termine", DrFr_Cod(j).Item("termine").ToString)
                                        XmlFormulato.SetAttribute("data_term", DrFr_Cod(j).Item("data_term").ToString)
                                        XmlFormulato.SetAttribute("data_fine_comm", DrFr_Cod(j).Item("Data_Fine_Comm").ToString)
                                        XmlFormulato.SetAttribute("data_fine_usoscorte", DrFr_Cod(j).Item("Data_Fine_UsoScorte").ToString)

                                    End If

                                    XmlPrincipioAttivo = XmlDoc.CreateElement("PRINCIPIO_ATTIVO")
                                    XmlFormulato.AppendChild(XmlPrincipioAttivo)

                                    XmlPrincipioAttivo.SetAttribute("pa_cod", DrFr_Cod(j).Item("Pa_Cod").ToString)
                                    XmlPrincipioAttivo.SetAttribute("pa_des", DrFr_Cod(j).Item("Pa_Des").ToString)
                                    XmlPrincipioAttivo.SetAttribute("titolo", DrFr_Cod(j).Item("Titolo").ToString)

                                Next


                            End If

                        Next

                    End If

                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////


            Case enum_AWS_Xml.Xml_DECODIFICA

                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

                Errore = CStr(XmlRisultati.GetAttribute("errore"))

                If Errore = "" Then

                    '----- Definisco la struttura del DataTable

                    DtRisultati.Columns.Add(New DataColumn("fr_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("fr_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("pa_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("pa_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("titolo", GetType(String)))

                    XMLs_Formulati = XmlRisultati.GetElementsByTagName("FORMULATO")

                    For i = 0 To XMLs_Formulati.Count - 1

                        XmlFormulato = XMLs_Formulati.Item(i)

                        XMLs_Principi = XmlFormulato.GetElementsByTagName("PRINCIPIO_ATTIVO")

                        If XMLs_Principi IsNot Nothing Then

                            For j = 0 To XMLs_Principi.Count - 1

                                XmlPrincipioAttivo = XMLs_Principi.Item(j)

                                Dr = DtRisultati.NewRow

                                'Definisco i valori
                                Dr.Item("fr_cod") = CStr(XmlFormulato.GetAttribute("fr_cod"))
                                Dr.Item("fr_des") = CStr(XmlFormulato.GetAttribute("fr_des"))
                                Dr.Item("pa_cod") = CStr(XmlPrincipioAttivo.GetAttribute("pa_cod"))
                                Dr.Item("pa_des") = CStr(XmlPrincipioAttivo.GetAttribute("pa_des"))
                                Dr.Item("titolo") = CStr(XmlPrincipioAttivo.GetAttribute("titolo"))

                                DtRisultati.Rows.Add(Dr)

                            Next

                        End If

                    Next


                Else

                    DtRisultati = Nothing

                End If

                'Distruggo gli oggetti
                XMLs_Formulati = Nothing
                XMLs_Principi = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing

                '///////////////////////////////////////////////


        End Select

    End Sub




    '###############################################################################################
    'L'informazione sulla Classe Tossicologica non e' piu' dentro la tabella Formulati
    'Inoltre ora i formulati possono avere piu' classi tossicologiche ==> devo aggiungere un nuovo nodo nel XML

    Public Sub AgroWS_XML_Risultati_Formulati_PrincipiAttivi_2(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef DtRisultati As DataTable,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlFormulato As Xml.XmlElement
        Dim XmlPrincipioAttivo As Xml.XmlElement
        Dim XmlClasseTossicologica As Xml.XmlElement

        Dim i, j As Integer

        Dim XMLs_Formulati As Xml.XmlNodeList
        Dim XMLs_Principi As Xml.XmlNodeList
        Dim Dr As DataRow

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <FORMULATO fr_cod="..." fr_des="..." data_reg="..."  
        '                  revocato="..." data_revo="..." sospeso="..." data_sosp="..." termine="..." data_term="..." 
        '                  data_fine_comm="..." data_fine_usoscorte="..." />
        '             <CLASSE_TOSSICOLOGICA cltoss_cod="..." cltoss_des="..." />
        '             <CLASSE_TOSSICOLOGICA cltoss_cod="..." cltoss_des="..." />
        '             <PRINCIPIO_ATTIVO pa_cod="..." pa_des="..." titolo="..."  />
        '             <PRINCIPIO_ATTIVO pa_cod="..." pa_des="..." titolo="..."  />
        '       <FORMULATO fr_cod="..." fr_des="..." data_reg="..." 
        '                  revocato="..." data_revo="..." sospeso="..." data_sosp="..." termine="..." data_term="..." 
        '                  data_fine_comm="..." data_fine_usoscorte="..." />
        '             <CLASSE_TOSSICOLOGICA cltoss_cod="..." cltoss_des="..." />
        '             <PRINCIPIO_ATTIVO pa_cod="..." pa_des="..." titolo="..."  />
        '             <PRINCIPIO_ATTIVO pa_cod="..." pa_des="..." titolo="..."  />
        '   </RISULTATI>

        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")
                XmlDoc.AppendChild(XmlRisultati)

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                'Se il recordset e' pieno ...
                If (Not IsNothing(DtRisultati)) Then


                    'Creo un vettore di stringhe con il DISTINCT dei valori "Fr_Cod"
                    'Dim ObjUtility As New AgronicaCoreUtility.DatatableUtility
                    Dim strFrCod As String() = SelectDistinct(DtRisultati, "Fr_Cod")



                    'Se il vettore degli Fr_Cod contiene qualcosa ...
                    If strFrCod IsNot Nothing Then

                        Dim Fr_Cod As String
                        Dim DrFr_Cod As DataRow()

                        'Dim Pa_Cod_Corrente As String = ""
                        'Dim Pa_Cod_Precedente As String = ""
                        'Dim ClToss_Cod_Corrente As String = ""
                        'Dim ClToss_Cod_Elenco As String = ""


                        'Ciclo sui diversi Fr_Cod ...
                        For i = 0 To strFrCod.Length - 1

                            'i-esimo Fr_Cod
                            Fr_Cod = strFrCod(i)

                            'Seleziono le righe legate all' i-esimo Fr_Cod ...
                            DrFr_Cod = DtRisultati.Select("fr_cod=" & Fr_Cod)

                            Dim HashClToss As New Hashtable
                            Dim HashPA As New Hashtable

                            If (DrFr_Cod IsNot Nothing) AndAlso (DrFr_Cod.Length > 0) Then

                                XmlFormulato = XmlDoc.CreateElement("FORMULATO")
                                XmlRisultati.AppendChild(XmlFormulato)

                                XmlFormulato.SetAttribute("fr_cod", DrFr_Cod(0).Item("Fr_Cod").ToString)
                                XmlFormulato.SetAttribute("fr_des", DrFr_Cod(0).Item("Fr_Des").ToString)
                                XmlFormulato.SetAttribute("revocato", DrFr_Cod(0).Item("revocato").ToString)
                                XmlFormulato.SetAttribute("data_revo", DrFr_Cod(0).Item("data_revo").ToString)
                                XmlFormulato.SetAttribute("sospeso", DrFr_Cod(0).Item("sospeso").ToString)
                                XmlFormulato.SetAttribute("data_sosp", DrFr_Cod(0).Item("data_sosp").ToString)
                                XmlFormulato.SetAttribute("termine", DrFr_Cod(0).Item("termine").ToString)
                                XmlFormulato.SetAttribute("data_term", DrFr_Cod(0).Item("data_term").ToString)
                                XmlFormulato.SetAttribute("data_fine_comm", DrFr_Cod(0).Item("Data_Fine_Comm").ToString)
                                XmlFormulato.SetAttribute("data_fine_usoscorte", DrFr_Cod(0).Item("Data_Fine_UsoScorte").ToString)


                                '===== CLASSE TOSSICOLOGICA

                                'ClToss_Cod_Elenco = "§"

                                For j = 0 To DrFr_Cod.Length - 1

                                    If Not HashClToss.ContainsKey(DrFr_Cod(j).Item("CLTOSS_COD")) Then

                                        HashClToss.Add(DrFr_Cod(j).Item("CLTOSS_COD"), "")

                                        XmlClasseTossicologica = XmlDoc.CreateElement("CLASSE_TOSSICOLOGICA")
                                        XmlClasseTossicologica.SetAttribute("cltoss_cod", DrFr_Cod(j).Item("CLTOSS_COD").ToString)
                                        XmlClasseTossicologica.SetAttribute("cltoss_des", DrFr_Cod(j).Item("CLTOSS_DES").ToString)
                                        XmlFormulato.AppendChild(XmlClasseTossicologica)


                                    End If


                                    'ClToss_Cod_Corrente = DrFr_Cod(j).Item("CLTOSS_COD").ToString

                                    ''Se ho un valore valido che non sia gia' stato preso in considerazione ...
                                    'If (ClToss_Cod_Corrente <> "") And (InStr(ClToss_Cod_Elenco, "§" & ClToss_Cod_Corrente & "§") = 0) Then

                                    '    XmlClasseTossicologica = XmlDoc.CreateElement("CLASSE_TOSSICOLOGICA")
                                    '    XmlFormulato.AppendChild(XmlClasseTossicologica)

                                    '    XmlClasseTossicologica.SetAttribute("cltoss_cod", DrFr_Cod(j).Item("CLTOSS_COD").ToString)
                                    '    XmlClasseTossicologica.SetAttribute("cltoss_des", DrFr_Cod(j).Item("CLTOSS_DES").ToString)

                                    '    'Memorizzo i valori gia' presi in considerazione
                                    '    ClToss_Cod_Elenco &= ClToss_Cod_Corrente & "§"

                                    'End If

                                Next


                                '===== PRINCIPIO ATTIVO

                                'Pa_Cod_Precedente = "-9999"

                                For j = 0 To DrFr_Cod.Length - 1

                                    If Not HashPA.ContainsKey(DrFr_Cod(j).Item("Pa_Cod")) Then

                                        HashPA.Add(DrFr_Cod(j).Item("Pa_Cod"), "")

                                        XmlPrincipioAttivo = XmlDoc.CreateElement("PRINCIPIO_ATTIVO")
                                        XmlFormulato.AppendChild(XmlPrincipioAttivo)

                                        XmlPrincipioAttivo.SetAttribute("pa_cod", DrFr_Cod(j).Item("Pa_Cod").ToString)
                                        XmlPrincipioAttivo.SetAttribute("pa_des", DrFr_Cod(j).Item("Pa_Des").ToString)
                                        XmlPrincipioAttivo.SetAttribute("titolo", DrFr_Cod(j).Item("Titolo").ToString)
                                        XmlPrincipioAttivo.SetAttribute("peso", DrFr_Cod(j).Item("peso").ToString)

                                    End If


                                    'Pa_Cod_Corrente = DrFr_Cod(j).Item("Pa_Cod").ToString

                                    'If (Pa_Cod_Corrente <> Pa_Cod_Precedente) And (Pa_Cod_Corrente <> "0") Then

                                    '    XmlPrincipioAttivo = XmlDoc.CreateElement("PRINCIPIO_ATTIVO")
                                    '    XmlFormulato.AppendChild(XmlPrincipioAttivo)

                                    '    XmlPrincipioAttivo.SetAttribute("pa_cod", DrFr_Cod(j).Item("Pa_Cod").ToString)
                                    '    XmlPrincipioAttivo.SetAttribute("pa_des", DrFr_Cod(j).Item("Pa_Des").ToString)
                                    '    XmlPrincipioAttivo.SetAttribute("titolo", DrFr_Cod(j).Item("Titolo").ToString)
                                    '    XmlPrincipioAttivo.SetAttribute("peso", DrFr_Cod(j).Item("peso").ToString)

                                    'End If

                                    'Pa_Cod_Precedente = Pa_Cod_Corrente

                                Next

                            End If

                        Next

                    End If

                End If

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////


            Case enum_AWS_Xml.Xml_DECODIFICA

                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

                Errore = CStr(XmlRisultati.GetAttribute("errore"))

                If Errore = "" Then

                    '----- Definisco la struttura del DataTable

                    DtRisultati.Columns.Add(New DataColumn("fr_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("fr_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("pa_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("pa_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("titolo", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("peso", GetType(String)))

                    DtRisultati.Columns.Add(New DataColumn("cltoss_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("cltoss_des", GetType(String)))



                    XMLs_Formulati = XmlRisultati.GetElementsByTagName("FORMULATO")

                    For i = 0 To XMLs_Formulati.Count - 1

                        XmlFormulato = XMLs_Formulati.Item(i)

                        XMLs_Principi = XmlFormulato.GetElementsByTagName("PRINCIPIO_ATTIVO")

                        If XMLs_Principi IsNot Nothing Then

                            For j = 0 To XMLs_Principi.Count - 1

                                XmlPrincipioAttivo = XMLs_Principi.Item(j)

                                Dr = DtRisultati.NewRow

                                'Definisco i valori
                                Dr.Item("fr_cod") = CStr(XmlFormulato.GetAttribute("fr_cod"))
                                Dr.Item("fr_des") = CStr(XmlFormulato.GetAttribute("fr_des"))
                                Dr.Item("pa_cod") = CStr(XmlPrincipioAttivo.GetAttribute("pa_cod"))
                                Dr.Item("pa_des") = CStr(XmlPrincipioAttivo.GetAttribute("pa_des"))
                                Dr.Item("titolo") = CStr(XmlPrincipioAttivo.GetAttribute("titolo"))
                                Dr.Item("peso") = CStr(XmlPrincipioAttivo.GetAttribute("peso"))

                                Dr.Item("cltoss_cod") = CStr(XmlPrincipioAttivo.GetAttribute("cltoss_cod"))
                                Dr.Item("cltoss_des") = CStr(XmlPrincipioAttivo.GetAttribute("cltoss_des"))

                                DtRisultati.Rows.Add(Dr)

                            Next

                        End If

                    Next


                Else

                    DtRisultati = Nothing

                End If

                'Distruggo gli oggetti
                XMLs_Formulati = Nothing
                XMLs_Principi = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing

                '///////////////////////////////////////////////


        End Select

    End Sub




    '###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulati_Bio(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef DtRisultati As DataTable,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlFormulato As Xml.XmlElement
        Dim i As Integer

        Dim XMLs_Formulati As Xml.XmlNodeList
        Dim Dr As DataRow

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <FORMULATO fr_cod="..." fr_des="..." bio="..."  />
        '       <FORMULATO fr_cod="..." fr_des="..." bio="..."  />
        '   </RISULTATI>

        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                'Se il recordset è pieno ...
                If (Not IsNothing(DtRisultati)) Then

                    'Dim ObjUtility As New AgronicaCoreUtility.DatatableUtility
                    Dim strFrCod As String() = SelectDistinct(DtRisultati, "Fr_Cod")


                    If strFrCod IsNot Nothing Then

                        Dim Fr_Cod As String
                        Dim DrFr_Cod As DataRow()

                        For i = 0 To strFrCod.Length - 1

                            Fr_Cod = strFrCod(i)

                            DrFr_Cod = DtRisultati.Select("fr_cod=" & Fr_Cod)

                            If DrFr_Cod IsNot Nothing AndAlso DrFr_Cod.Length > 0 Then

                                XmlFormulato = XmlDoc.CreateElement("FORMULATO")
                                XmlRisultati.AppendChild(XmlFormulato)

                                XmlFormulato.SetAttribute("fr_cod", DrFr_Cod(0).Item("Fr_Cod").ToString)
                                XmlFormulato.SetAttribute("fr_des", DrFr_Cod(0).Item("Fr_Des").ToString)
                                XmlFormulato.SetAttribute("bio", DrFr_Cod(0).Item("bio").ToString)

                            End If

                        Next

                    End If

                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////


            Case enum_AWS_Xml.Xml_DECODIFICA

                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

                Errore = CStr(XmlRisultati.GetAttribute("errore"))

                If Errore = "" Then

                    '----- Definisco la struttura del DataTable

                    DtRisultati.Columns.Add(New DataColumn("fr_cod", GetType(Integer)))
                    DtRisultati.Columns.Add(New DataColumn("fr_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("bio", GetType(Integer)))

                    XMLs_Formulati = XmlRisultati.GetElementsByTagName("FORMULATO")

                    For i = 0 To XMLs_Formulati.Count - 1

                        XmlFormulato = XMLs_Formulati.Item(i)

                        Dr = DtRisultati.NewRow
                        Dr.Item("fr_cod") = CInt(XmlFormulato.GetAttribute("fr_cod"))
                        Dr.Item("fr_des") = CStr(XmlFormulato.GetAttribute("fr_des"))
                        Dr.Item("bio") = CInt(XmlFormulato.GetAttribute("bio"))
                        DtRisultati.Rows.Add(Dr)

                    Next

                Else

                    DtRisultati = Nothing

                End If

                'Distruggo gli oggetti
                XMLs_Formulati = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing

                '///////////////////////////////////////////////


        End Select

    End Sub

    '###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulati_PrincipiAttiviGruppiPrincipiAttivi(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef DtRisultati As DataTable,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlFormulato As Xml.XmlElement
        Dim XmlPrincipioAttivo As Xml.XmlElement
        Dim i, j As Integer

        Dim XMLs_Formulati As Xml.XmlNodeList
        Dim XMLs_Principi As Xml.XmlNodeList
        Dim Dr As DataRow

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <FORMULATO fr_cod="..." fr_des="..." data_reg="..."  
        '                  revocato="..." data_revo="..." sospeso="..." data_sosp="..." termine="..." data_term="..." 
        '                  data_fine_comm="..." data_fine_usoscorte="..." normeprecauzionali="..."  sempl_comp="..." />
        '             <PRINCIPIO_ATTIVO pa_cod="..." pa_des="..." titolo="..." gpa_cod="..." gpa_des="..." />
        '             <PRINCIPIO_ATTIVO pa_cod="..." pa_des="..." titolo="..." gpa_cod="..." gpa_des="..." />
        '       <FORMULATO fr_cod="..." fr_des="..." data_reg="..." 
        '                  revocato="..." data_revo="..." sospeso="..." data_sosp="..." termine="..." data_term="..." 
        '                  data_fine_comm="..." data_fine_usoscorte="..." normeprecauzionali="..."  sempl_comp="..." />
        '             <PRINCIPIO_ATTIVO pa_cod="..." pa_des="..." titolo="..." gpa_cod="..." gpa_des="..." />
        '             <PRINCIPIO_ATTIVO pa_cod="..." pa_des="..." titolo="..." gpa_cod="..." gpa_des="..." />
        '   </RISULTATI>

        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                'Se il recordset è pieno ...
                If (Not IsNothing(DtRisultati)) Then

                    Dim strFrCod As String() = SelectDistinct(DtRisultati, "Fr_Cod")

                    If strFrCod IsNot Nothing Then

                        Dim Fr_Cod As String
                        Dim DrFr_Cod As DataRow()

                        For i = 0 To strFrCod.Length - 1

                            Fr_Cod = strFrCod(i)

                            DrFr_Cod = DtRisultati.Select("fr_cod=" & Fr_Cod)

                            If DrFr_Cod IsNot Nothing AndAlso DrFr_Cod.Length > 0 Then

                                XmlFormulato = XmlDoc.CreateElement("FORMULATO")
                                XmlRisultati.AppendChild(XmlFormulato)

                                XmlFormulato.SetAttribute("fr_cod", DrFr_Cod(0).Item("Fr_Cod").ToString)
                                XmlFormulato.SetAttribute("fr_des", DrFr_Cod(0).Item("Fr_Des").ToString)

                                For j = 0 To DrFr_Cod.Length - 1

                                    If j = 0 Then

                                        XmlFormulato.SetAttribute("revocato", DrFr_Cod(j).Item("revocato").ToString)
                                        XmlFormulato.SetAttribute("data_revo", DrFr_Cod(j).Item("data_revo").ToString)
                                        XmlFormulato.SetAttribute("sospeso", DrFr_Cod(j).Item("sospeso").ToString)
                                        XmlFormulato.SetAttribute("data_sosp", DrFr_Cod(j).Item("data_sosp").ToString)
                                        XmlFormulato.SetAttribute("termine", DrFr_Cod(j).Item("termine").ToString)
                                        XmlFormulato.SetAttribute("data_term", DrFr_Cod(j).Item("data_term").ToString)
                                        XmlFormulato.SetAttribute("data_fine_comm", DrFr_Cod(j).Item("Data_Fine_Comm").ToString)
                                        XmlFormulato.SetAttribute("data_fine_usoscorte", DrFr_Cod(j).Item("Data_Fine_UsoScorte").ToString)

                                    End If

                                    XmlPrincipioAttivo = XmlDoc.CreateElement("PRINCIPIO_ATTIVO")
                                    XmlFormulato.AppendChild(XmlPrincipioAttivo)

                                    XmlPrincipioAttivo.SetAttribute("pa_cod", DrFr_Cod(j).Item("Pa_Cod").ToString)
                                    XmlPrincipioAttivo.SetAttribute("pa_des", DrFr_Cod(j).Item("Pa_Des").ToString)
                                    XmlPrincipioAttivo.SetAttribute("titolo", DrFr_Cod(j).Item("Titolo").ToString)

                                    XmlPrincipioAttivo.SetAttribute("gpa_cod", DrFr_Cod(j).Item("gpa_cod").ToString)
                                    XmlPrincipioAttivo.SetAttribute("gpa_des", DrFr_Cod(j).Item("gpa_des").ToString)

                                Next


                            End If

                        Next

                    End If

                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////


            Case enum_AWS_Xml.Xml_DECODIFICA

                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

                Errore = CStr(XmlRisultati.GetAttribute("errore"))

                If Errore = "" Then

                    '----- Definisco la struttura del DataTable

                    DtRisultati.Columns.Add(New DataColumn("fr_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("fr_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("pa_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("pa_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("titolo", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("GPA_Cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("GPA_des", GetType(String)))

                    XMLs_Formulati = XmlRisultati.GetElementsByTagName("FORMULATO")

                    For i = 0 To XMLs_Formulati.Count - 1

                        XmlFormulato = XMLs_Formulati.Item(i)

                        XMLs_Principi = XmlFormulato.GetElementsByTagName("PRINCIPIO_ATTIVO")

                        If XMLs_Principi IsNot Nothing Then

                            For j = 0 To XMLs_Principi.Count - 1

                                XmlPrincipioAttivo = XMLs_Principi.Item(j)

                                Dr = DtRisultati.NewRow

                                'Definisco i valori
                                Dr.Item("fr_cod") = CStr(XmlFormulato.GetAttribute("fr_cod"))
                                Dr.Item("fr_des") = CStr(XmlFormulato.GetAttribute("fr_des"))
                                Dr.Item("pa_cod") = CStr(XmlPrincipioAttivo.GetAttribute("pa_cod"))
                                Dr.Item("pa_des") = CStr(XmlPrincipioAttivo.GetAttribute("pa_des"))
                                Dr.Item("titolo") = CStr(XmlPrincipioAttivo.GetAttribute("titolo"))
                                Dr.Item("gpa_Cod") = CStr(XmlPrincipioAttivo.GetAttribute("gpa_cod"))
                                Dr.Item("gpa_des") = CStr(XmlPrincipioAttivo.GetAttribute("gpa_des"))

                                DtRisultati.Rows.Add(Dr)

                            Next

                        End If

                    Next


                Else

                    DtRisultati = Nothing

                End If

                'Distruggo gli oggetti
                XMLs_Formulati = Nothing
                XMLs_Principi = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing



        End Select

    End Sub

    '###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulati_SpecieVegetali(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef DtRisultati As DataTable,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlDati As Xml.XmlElement
        Dim XMLs_Dati As Xml.XmlNodeList
        Dim XmlNodi As Xml.XmlNodeList

        Dim i As Integer

        Dim Dr As DataRow

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <DATI for_veg_cod="..." fr_cod="..." veg_cod="..." 
        '             note="..." tempocarenza="..." />
        '       <DATI for_veg_cod="..." fr_cod="..." veg_cod="..." 
        '             note="..." tempocarenza="..." />
        '   </RISULTATI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                If (Not IsNothing(DtRisultati)) Then

                    For i = 0 To DtRisultati.Rows.Count - 1

                        '---------------------------------

                        'Creo il nodo DATI
                        XmlDati = XmlDoc.CreateElement("DATI")

                        'Imposto gli attributi

                        XmlDati.SetAttribute("for_veg_cod", DtRisultati.Rows(i).Item("For_Veg_Cod").ToString)
                        XmlDati.SetAttribute("fr_cod", DtRisultati.Rows(i).Item("Fr_Cod").ToString)
                        XmlDati.SetAttribute("veg_cod", DtRisultati.Rows(i).Item("Veg_Cod").ToString)
                        XmlDati.SetAttribute("grsp_cod", DtRisultati.Rows(i).Item("Grsp_Cod").ToString)

                        XmlDati.SetAttribute("note", DtRisultati.Rows(i).Item("Note").ToString)
                        XmlDati.SetAttribute("tempocarenza", DtRisultati.Rows(i).Item("TempoCarenza").ToString)

                        'Imposto XmlDati come figlio del nodo XmlRisultati
                        XmlRisultati.AppendChild(XmlDati)

                        '---------------------------------

                    Next

                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlNodi = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////


            Case enum_AWS_Xml.Xml_DECODIFICA


                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

                Errore = CStr(XmlRisultati.GetAttribute("errore"))

                If Errore = "" Then

                    '----- Definisco la struttura del DataTable

                    DtRisultati.Columns.Add(New DataColumn("fr_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("veg_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("grsp_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("for_veg_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("note", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("tempocarenza", GetType(String)))

                    XMLs_Dati = XmlRisultati.GetElementsByTagName("DATI")

                    For i = 0 To XMLs_Dati.Count - 1

                        XmlDati = XMLs_Dati.Item(i)

                        Dr = DtRisultati.NewRow

                        'Definisco i valori
                        Dr.Item("fr_cod") = CStr(XmlDati.GetAttribute("fr_cod"))
                        Dr.Item("veg_cod") = CStr(XmlDati.GetAttribute("veg_cod"))
                        Dr.Item("grsp_cod") = CStr(XmlDati.GetAttribute("grsp_cod"))
                        Dr.Item("for_veg_cod") = CStr(XmlDati.GetAttribute("for_veg_cod"))
                        Dr.Item("note") = CStr(XmlDati.GetAttribute("note"))
                        Dr.Item("tempocarenza") = CStr(XmlDati.GetAttribute("tempocarenza"))

                        DtRisultati.Rows.Add(Dr)

                    Next


                Else

                    DtRisultati = Nothing

                End If

                'Distruggo gli oggetti
                XMLs_Dati = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing

                '///////////////////////////////////////////////

        End Select

    End Sub

    Public Sub AgroWS_XML_Risultati_Formulati_SpecieVegetali_Intervallo(
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByRef StringaXML As String,
                                    ByRef DtRisultati As DataTable,
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlDati As Xml.XmlElement
        Dim XMLs_Dati As Xml.XmlNodeList
        Dim XmlNodi As Xml.XmlNodeList

        Dim i As Integer

        Dim Dr As DataRow

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <DATI for_veg_cod="..." fr_cod="..." veg_cod="..." 
        '             note="..." tempocarenza="..." />
        '       <DATI for_veg_cod="..." fr_cod="..." veg_cod="..." 
        '             note="..." tempocarenza="..." />
        '   </RISULTATI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                If (Not IsNothing(DtRisultati)) Then

                    For i = 0 To DtRisultati.Rows.Count - 1

                        '---------------------------------

                        'Creo il nodo DATI
                        XmlDati = XmlDoc.CreateElement("DATI")

                        'Imposto gli attributi

                        XmlDati.SetAttribute("for_veg_cod", DtRisultati.Rows(i).Item("For_Veg_Cod").ToString)
                        XmlDati.SetAttribute("fr_cod", DtRisultati.Rows(i).Item("Fr_Cod").ToString)
                        XmlDati.SetAttribute("veg_cod", DtRisultati.Rows(i).Item("Veg_Cod").ToString)
                        XmlDati.SetAttribute("grsp_cod", DtRisultati.Rows(i).Item("Grsp_Cod").ToString)

                        XmlDati.SetAttribute("grfi_cod", DtRisultati.Rows(i).Item("grfi_cod").ToString)
                        XmlDati.SetAttribute("flag_protetto", DtRisultati.Rows(i).Item("Flag_Protetto").ToString)

                        XmlDati.SetAttribute("validita_inizio", DtRisultati.Rows(i).Item("validita_inizio").ToString)
                        XmlDati.SetAttribute("validita_fine", DtRisultati.Rows(i).Item("validita_fine").ToString)

                        XmlDati.SetAttribute("tempocarenza", DtRisultati.Rows(i).Item("TempoCarenza").ToString)

                        'Imposto XmlDati come figlio del nodo XmlRisultati
                        XmlRisultati.AppendChild(XmlDati)

                        '---------------------------------

                    Next

                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlNodi = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////


            Case enum_AWS_Xml.Xml_DECODIFICA


                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

                Errore = CStr(XmlRisultati.GetAttribute("errore"))

                If Errore = "" Then

                    '----- Definisco la struttura del DataTable

                    DtRisultati.Columns.Add(New DataColumn("fr_cod", GetType(Integer)))
                    DtRisultati.Columns.Add(New DataColumn("veg_cod", GetType(Integer)))
                    DtRisultati.Columns.Add(New DataColumn("grsp_cod", GetType(Integer)))
                    DtRisultati.Columns.Add(New DataColumn("for_veg_cod", GetType(Integer)))
                    DtRisultati.Columns.Add(New DataColumn("grfi_cod", GetType(Integer)))
                    DtRisultati.Columns.Add(New DataColumn("flag_protetto", GetType(Integer)))
                    DtRisultati.Columns.Add(New DataColumn("validita_inizio", GetType(Date)))
                    DtRisultati.Columns.Add(New DataColumn("validita_fine", GetType(Date)))
                    DtRisultati.Columns.Add(New DataColumn("tempocarenza", GetType(Integer)))

                    XMLs_Dati = XmlRisultati.GetElementsByTagName("DATI")

                    For i = 0 To XMLs_Dati.Count - 1

                        XmlDati = XMLs_Dati.Item(i)

                        Dr = DtRisultati.NewRow

                        'Definisco i valori
                        If IsNumeric(XmlDati.GetAttribute("fr_cod")) Then
                            Dr.Item("fr_cod") = CInt(XmlDati.GetAttribute("fr_cod"))
                        Else
                            Dr.Item("fr_cod") = 0
                        End If
                        If IsNumeric(XmlDati.GetAttribute("veg_cod")) Then
                            Dr.Item("veg_cod") = CInt(XmlDati.GetAttribute("veg_cod"))
                        Else
                            Dr.Item("veg_cod") = 0
                        End If
                        If IsNumeric(XmlDati.GetAttribute("grsp_cod")) Then
                            Dr.Item("grsp_cod") = CInt(XmlDati.GetAttribute("grsp_cod"))
                        Else
                            Dr.Item("grsp_cod") = 0
                        End If
                        If IsNumeric(XmlDati.GetAttribute("for_veg_cod")) Then
                            Dr.Item("for_veg_cod") = CInt(XmlDati.GetAttribute("for_veg_cod"))
                        Else
                            Dr.Item("for_veg_cod") = 0
                        End If
                        If IsNumeric(XmlDati.GetAttribute("grfi_cod")) Then
                            Dr.Item("grfi_cod") = CInt(XmlDati.GetAttribute("grfi_cod"))
                        Else
                            Dr.Item("grfi_cod") = 0
                        End If
                        If IsNumeric(XmlDati.GetAttribute("flag_protetto")) Then
                            Dr.Item("flag_protetto") = CInt(XmlDati.GetAttribute("flag_protetto"))
                        Else
                            Dr.Item("flag_protetto") = 0
                        End If
                        If IsDate(XmlDati.GetAttribute("validita_inizio")) Then
                            Dr.Item("validita_inizio") = CDate(XmlDati.GetAttribute("validita_inizio"))
                        Else
                            Dr.Item("validita_inizio") = AGRODATAINIZIO
                        End If
                        If IsDate(XmlDati.GetAttribute("validita_fine")) Then
                            Dr.Item("validita_fine") = CDate(XmlDati.GetAttribute("validita_fine"))
                        Else
                            Dr.Item("validita_fine") = AGRODATAFINE
                        End If
                        If IsNumeric(XmlDati.GetAttribute("tempocarenza")) Then
                            Dr.Item("tempocarenza") = CInt(XmlDati.GetAttribute("tempocarenza"))
                        Else
                            Dr.Item("tempocarenza") = 0
                        End If

                        'Dr.Item("fr_cod") = CStr(XmlDati.GetAttribute("fr_cod"))
                        'Dr.Item("veg_cod") = CStr(XmlDati.GetAttribute("veg_cod"))
                        'Dr.Item("grsp_cod") = CStr(XmlDati.GetAttribute("grsp_cod"))
                        'Dr.Item("for_veg_cod") = CStr(XmlDati.GetAttribute("for_veg_cod"))
                        'Dr.Item("grfi_cod") = CStr(XmlDati.GetAttribute("grfi_cod"))
                        'Dr.Item("flag_protetto") = CStr(XmlDati.GetAttribute("flag_protetto"))
                        'Dr.Item("validita_inizio") = CStr(XmlDati.GetAttribute("validita_inizio"))
                        'Dr.Item("validita_fine") = CStr(XmlDati.GetAttribute("validita_fine"))
                        'Dr.Item("tempocarenza") = CStr(XmlDati.GetAttribute("tempocarenza"))

                        DtRisultati.Rows.Add(Dr)

                    Next


                Else

                    DtRisultati = Nothing

                End If

                'Distruggo gli oggetti
                XMLs_Dati = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing

                '///////////////////////////////////////////////

        End Select

    End Sub

    '###############################################################################################
    'Public Sub AgroWS_XML_Risultati_Formulati_SpecieVegetali_Avversita( _
    '                                ByVal Operazione As enum_AWS_Xml, _
    '                                ByRef StringaXML As String, _
    '                                ByRef RsRisultati As ADODB.Recordset, _
    '                                ByRef DtRisultati As DataTable, _
    '                                ByRef Errore As String)

    '    Dim XmlDoc As New Xml.XmlDocument
    '    Dim XmlRisultati As Xml.XmlElement
    '    Dim XmlDati As Xml.XmlElement
    '    Dim XMLs_Dati As Xml.XmlNodeList
    '    Dim XmlNodi As Xml.XmlNodeList
    '    Dim XmlNodo As Xml.XmlElement
    '    Dim Dr As DataRow

    '    Dim i As Integer

    '    '===========================================================================================
    '    '   <RISULTATI errore="...">
    '    '       <DATI fr_cod="..." fr_des="..."  
    '    '             veg_cod="..." veg_des="..." grsp_cod="..." grsp_des="..." tempocarenza="..."
    '    '             for_veg_av_cod="..." for_veg_cod="..."
    '    '             av_cod="..." av_des_vol="..." av_gru="..." av_gru_des="..."
    '    '             note="..." limiteinterventi ="..." udm_cod="..." udm_sim="..."/>
    '    '       <DATI fr_cod="..." fr_des="..."  
    '    '             veg_cod="..." veg_des="..." grsp_cod="..." grsp_des="..." tempocarenza="..."
    '    '             for_veg_av_cod="..." for_veg_cod="..."
    '    '             av_cod="..." av_des_vol="..." av_gru="..." av_gru_des="..."
    '    '             note="..." limiteinterventi ="..." udm_cod="..." udm_sim="..."/>
    '    '   </RISULTATI>
    '    '===========================================================================================

    '    Select Case Operazione

    '        Case enum_AWS_Xml.Xml_CODIFICA
    '            '///////////////////////////////////////////////
    '            '/// CODIFICA //////////////////////////////////
    '            '///////////////////////////////////////////////

    '            'Creo il nodo RISULTATI
    '            XmlRisultati = XmlDoc.CreateElement("RISULTATI")

    '            'Imposto gli attributi
    '            XmlRisultati.SetAttribute("errore", "")

    '            'Se il recordset e' pieno ...
    '            If (Not IsNothing(RsRisultati)) AndAlso _
    '               (RsRisultati.State <> 0) AndAlso _
    '               (Not RsRisultati.EOF) Then

    '                'Estraggo le informazioni dal recordset
    '                Do While Not RsRisultati.EOF

    '                    '---------------------------------

    '                    'Creo il nodo DATI
    '                    XmlDati = XmlDoc.CreateElement("DATI")

    '                    'Imposto gli attributi
    '                    XmlDati.SetAttribute("fr_cod", RsRisultati("fr_cod").Value.ToString)
    '                    XmlDati.SetAttribute("fr_des", RsRisultati("fr_des").Value.ToString)
    '                    XmlDati.SetAttribute("veg_cod", RsRisultati("veg_cod").Value.ToString)
    '                    XmlDati.SetAttribute("veg_des", RsRisultati("veg_des").Value.ToString)
    '                    XmlDati.SetAttribute("grsp_cod", RsRisultati("grsp_cod").Value.ToString)
    '                    XmlDati.SetAttribute("grsp_des", RsRisultati("grsp_des").Value.ToString)
    '                    XmlDati.SetAttribute("tempocarenza", RsRisultati("tempocarenza").Value.ToString)
    '                    XmlDati.SetAttribute("for_veg_cod", RsRisultati("for_veg_cod").Value.ToString)
    '                    XmlDati.SetAttribute("for_veg_av_cod", RsRisultati("for_veg_av_cod").Value.ToString)
    '                    XmlDati.SetAttribute("av_cod", RsRisultati("av_cod").Value.ToString)
    '                    XmlDati.SetAttribute("av_des_vol", RsRisultati("av_des_vol").Value.ToString)
    '                    XmlDati.SetAttribute("av_des_lat", RsRisultati("av_des_lat").Value.ToString)
    '                    XmlDati.SetAttribute("av_gru", RsRisultati("av_gru").Value.ToString)
    '                    XmlDati.SetAttribute("av_gru_des", RsRisultati("av_gru_des").Value.ToString)
    '                    XmlDati.SetAttribute("av_gru_des_lat", RsRisultati("av_gru_des_lat").Value.ToString)
    '                    XmlDati.SetAttribute("note", RsRisultati("note").Value.ToString)
    '                    XmlDati.SetAttribute("limiteinterventi", RsRisultati("limiteinterventi").Value.ToString)
    '                    XmlDati.SetAttribute("udm_cod", RsRisultati("udm_cod").Value.ToString)
    '                    XmlDati.SetAttribute("udm_sim", RsRisultati("udm_sim").Value.ToString)

    '                    'Imposto XmlDati come figlio del nodo XmlRisultati
    '                    XmlRisultati.AppendChild(XmlDati)

    '                    '---------------------------------

    '                    'Prossimo nodo
    '                    RsRisultati.MoveNext()
    '                Loop

    '            End If

    '            'Imposto XmlRisultati come figlio del documento principale
    '            XmlDoc.AppendChild(XmlRisultati)

    '            'Restituisco in uscita la stringa creata
    '            StringaXML = XmlDoc.InnerXml

    '            'Distruggo gli oggetti
    '            XmlRisultati = Nothing
    '            XmlNodi = Nothing
    '            XmlDoc = Nothing
    '            '///////////////////////////////////////////////



    '        Case enum_AWS_Xml.Xml_DECODIFICA
    '            '///////////////////////////////////////////////
    '            '/// DECODIFICA ////////////////////////////////
    '            '///////////////////////////////////////////////

    '            'Carico la stringa XML nel documento
    '            XmlDoc.LoadXml(StringaXML)

    '            'Prelevo il nodo XmlParametri
    '            XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

    '            Errore = CStr(XmlRisultati.GetAttribute("errore"))

    '            If Errore = "" Then

    '                '----- Definisco la struttura del DataTable

    '                DtRisultati.Columns.Add(New DataColumn("fr_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("fr_des", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("veg_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("veg_des", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("grsp_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("grsp_des", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("tempocarenza", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("for_veg_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("for_veg_av_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("av_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("av_des_vol", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("av_des_lat", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("av_gru", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("av_gru_des", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("av_gru_des_lat", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("note", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("limiteinterventi", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("udm_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("udm_sim", GetType(String)))

    '                XMLs_Dati = XmlRisultati.GetElementsByTagName("DATI")

    '                For i = 0 To XMLs_Dati.Count - 1

    '                    XmlDati = XMLs_Dati.Item(i)

    '                    Dr = DtRisultati.NewRow

    '                    'Definisco i valori
    '                    Dr.Item("fr_cod") = CStr(XmlDati.GetAttribute("fr_cod"))
    '                    Dr.Item("fr_des") = CStr(XmlDati.GetAttribute("fr_des"))
    '                    Dr.Item("veg_cod") = CStr(XmlDati.GetAttribute("veg_cod"))
    '                    Dr.Item("veg_des") = CStr(XmlDati.GetAttribute("veg_des"))
    '                    Dr.Item("grsp_cod") = CStr(XmlDati.GetAttribute("grsp_cod"))
    '                    Dr.Item("grsp_des") = CStr(XmlDati.GetAttribute("grsp_des"))
    '                    Dr.Item("tempocarenza") = CStr(XmlDati.GetAttribute("tempocarenza"))
    '                    Dr.Item("for_veg_cod") = CStr(XmlDati.GetAttribute("for_veg_cod"))
    '                    Dr.Item("for_veg_av_cod") = CStr(XmlDati.GetAttribute("for_veg_av_cod"))
    '                    Dr.Item("av_cod") = CStr(XmlDati.GetAttribute("av_cod"))
    '                    Dr.Item("av_des_vol") = CStr(XmlDati.GetAttribute("av_des_vol"))
    '                    Dr.Item("av_des_lat") = CStr(XmlDati.GetAttribute("av_des_lat"))
    '                    Dr.Item("av_gru") = CStr(XmlDati.GetAttribute("av_gru"))
    '                    Dr.Item("av_gru_des") = CStr(XmlDati.GetAttribute("av_gru_des"))
    '                    Dr.Item("av_gru_des_lat") = CStr(XmlDati.GetAttribute("av_gru_des_lat"))
    '                    Dr.Item("note") = CStr(XmlDati.GetAttribute("note"))
    '                    Dr.Item("limiteinterventi") = CStr(XmlDati.GetAttribute("limiteinterventi"))
    '                    Dr.Item("udm_cod") = CStr(XmlDati.GetAttribute("udm_cod"))
    '                    Dr.Item("udm_sim") = CStr(XmlDati.GetAttribute("udm_sim"))

    '                    DtRisultati.Rows.Add(Dr)

    '                Next


    '            Else

    '                DtRisultati = Nothing

    '            End If

    '            'Distruggo gli oggetti
    '            XMLs_Dati = Nothing
    '            XmlRisultati = Nothing
    '            XmlDoc = Nothing


    '    End Select

    'End Sub

    '###############################################################################################
    Public Sub AgroWS_XML_Risultati_Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita( _
                                            ByVal Operazione As enum_AWS_Xml, _
                                            ByRef StringaXML As String, _
                                            ByRef DtRisultati As DataTable, _
                                            ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlDati As Xml.XmlElement
        Dim XMLs_Dati As Xml.XmlNodeList
        Dim XmlNodi As Xml.XmlNodeList
        Dim Dr As DataRow

        Dim i As Integer

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <DATI av_cod="..." av_des_vol="..." av_gru="..." av_gru_des="..."/>
        '       <DATI av_cod="..." av_des_vol="..." av_gru="..." av_gru_des="..."/>
        '   </RISULTATI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                'Se il recordset e' pieno ...
                If Not IsNothing(DtRisultati) Then

                    'Estraggo le informazioni dal recordset
                    For i = 0 To DtRisultati.Rows.Count - 1

                        'Creo il nodo DATI
                        XmlDati = XmlDoc.CreateElement("DATI")

                        'Imposto gli attributi
                        XmlDati.SetAttribute("av_cod", DtRisultati.Rows(i).Item("av_cod").ToString)
                        XmlDati.SetAttribute("av_des_vol", DtRisultati.Rows(i).Item("av_des_vol").ToString)
                        XmlDati.SetAttribute("av_des_lat", DtRisultati.Rows(i).Item("av_des_lat").ToString)
                        XmlDati.SetAttribute("av_gru", DtRisultati.Rows(i).Item("av_gru").ToString)
                        XmlDati.SetAttribute("av_gru_des", DtRisultati.Rows(i).Item("av_gru_des").ToString)
                        XmlDati.SetAttribute("av_gru_des_lat", DtRisultati.Rows(i).Item("av_gru_des_lat").ToString)


                        'Imposto XmlDati come figlio del nodo XmlRisultati
                        XmlRisultati.AppendChild(XmlDati)

                        '---------------------------------
                    Next

                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlNodi = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

                Errore = CStr(XmlRisultati.GetAttribute("errore"))

                If Errore = "" Then

                    '----- Definisco la struttura del DataTable

                    DtRisultati.Columns.Add(New DataColumn("av_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_des_vol", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_des_lat", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_gru", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_gru_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_gru_des_lat", GetType(String)))

                    XMLs_Dati = XmlRisultati.GetElementsByTagName("DATI")

                    For i = 0 To XMLs_Dati.Count - 1

                        XmlDati = XMLs_Dati.Item(i)

                        Dr = DtRisultati.NewRow

                        'Definisco i valori
                        Dr.Item("av_cod") = CStr(XmlDati.GetAttribute("av_cod"))
                        Dr.Item("av_des_vol") = CStr(XmlDati.GetAttribute("av_des_vol"))
                        Dr.Item("av_des_lat") = CStr(XmlDati.GetAttribute("av_des_lat"))
                        Dr.Item("av_gru") = CStr(XmlDati.GetAttribute("av_gru"))
                        Dr.Item("av_gru_des") = CStr(XmlDati.GetAttribute("av_gru_des"))
                        Dr.Item("av_gru_des_lat") = CStr(XmlDati.GetAttribute("av_gru_des_lat"))

                        DtRisultati.Rows.Add(Dr)

                    Next


                Else

                    DtRisultati = Nothing

                End If

                'Distruggo gli oggetti
                XMLs_Dati = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing


        End Select

    End Sub

    '###############################################################################################
    'Public Sub AgroWS_XML_Risultati_Formulati_SpecieVegetali_Infestanti( _
    '                                ByVal Operazione As enum_AWS_Xml, _
    '                                ByRef StringaXML As String, _
    '                                ByRef RsRisultati As ADODB.Recordset, _
    '                                ByRef DtRisultati As DataTable, _
    '                                ByRef Errore As String)

    '    Dim XmlDoc As New Xml.XmlDocument
    '    Dim XmlRisultati As Xml.XmlElement
    '    Dim XmlDati As Xml.XmlElement
    '    Dim XMLs_Dati As Xml.XmlNodeList
    '    Dim XmlNodi As Xml.XmlNodeList
    '    Dim XmlNodo As Xml.XmlElement
    '    Dim Dr As DataRow
    '    Dim i As Integer

    '    '===========================================================================================
    '    '   <RISULTATI errore="...">
    '    '       <DATI fr_cod="..." fr_des="..."  
    '    '             veg_cod="..." veg_des="..." grsp_cod="..." grsp_des="..." tempocarenza="..."
    '    '             for_veg_av_cod="..." for_veg_cod="..."
    '    '             av_cod="..." av_des_vol="..." av_gru="..." av_gru_des="..."
    '    '             note="..." limiteinterventi ="..." udm_cod="..." udm_sim="..."/>
    '    '       <DATI fr_cod="..." fr_des="..."  
    '    '             veg_cod="..." veg_des="..." grsp_cod="..." grsp_des="..." tempocarenza="..."
    '    '             for_veg_av_cod="..." for_veg_cod="..."
    '    '             av_cod="..." av_des_vol="..." av_gru="..." av_gru_des="..."
    '    '             note="..." limiteinterventi ="..." udm_cod="..." udm_sim="..."/>
    '    '   </RISULTATI>
    '    '===========================================================================================

    '    Select Case Operazione

    '        Case enum_AWS_Xml.Xml_CODIFICA
    '            '///////////////////////////////////////////////
    '            '/// CODIFICA //////////////////////////////////
    '            '///////////////////////////////////////////////

    '            'Creo il nodo RISULTATI
    '            XmlRisultati = XmlDoc.CreateElement("RISULTATI")

    '            'Imposto gli attributi
    '            XmlRisultati.SetAttribute("errore", "")

    '            'Se il recordset e' pieno ...
    '            If (Not IsNothing(RsRisultati)) AndAlso _
    '               (RsRisultati.State <> 0) AndAlso _
    '               (Not RsRisultati.EOF) Then

    '                'Estraggo le informazioni dal recordset
    '                Do While Not RsRisultati.EOF

    '                    '---------------------------------

    '                    'Creo il nodo DATI
    '                    XmlDati = XmlDoc.CreateElement("DATI")

    '                    'Imposto gli attributi
    '                    XmlDati.SetAttribute("fr_cod", RsRisultati("fr_cod").Value.ToString)
    '                    XmlDati.SetAttribute("fr_des", RsRisultati("fr_des").Value.ToString)
    '                    XmlDati.SetAttribute("veg_cod", RsRisultati("veg_cod").Value.ToString)
    '                    XmlDati.SetAttribute("veg_des", RsRisultati("veg_des").Value.ToString)
    '                    XmlDati.SetAttribute("grsp_cod", RsRisultati("grsp_cod").Value.ToString)
    '                    XmlDati.SetAttribute("grsp_des", RsRisultati("grsp_des").Value.ToString)
    '                    XmlDati.SetAttribute("tempocarenza", RsRisultati("tempocarenza").Value.ToString)
    '                    XmlDati.SetAttribute("for_veg_cod", RsRisultati("for_veg_cod").Value.ToString)
    '                    XmlDati.SetAttribute("for_veg_av_cod", RsRisultati("for_veg_av_cod").Value.ToString)
    '                    XmlDati.SetAttribute("av_cod", RsRisultati("av_cod").Value.ToString)
    '                    XmlDati.SetAttribute("av_des_vol", RsRisultati("av_des_vol").Value.ToString)
    '                    XmlDati.SetAttribute("av_des_lat", RsRisultati("av_des_lat").Value.ToString)
    '                    XmlDati.SetAttribute("av_gru", RsRisultati("av_gru").Value.ToString)
    '                    XmlDati.SetAttribute("av_gru_des", RsRisultati("av_gru_des").Value.ToString)
    '                    XmlDati.SetAttribute("av_gru_des_lat", RsRisultati("av_gru_des_lat").Value.ToString)
    '                    XmlDati.SetAttribute("note", RsRisultati("note").Value.ToString)
    '                    XmlDati.SetAttribute("limiteinterventi", RsRisultati("limiteinterventi").Value.ToString)
    '                    XmlDati.SetAttribute("udm_cod", RsRisultati("udm_cod").Value.ToString)
    '                    XmlDati.SetAttribute("udm_sim", RsRisultati("udm_sim").Value.ToString)

    '                    'Imposto XmlDati come figlio del nodo XmlRisultati
    '                    XmlRisultati.AppendChild(XmlDati)

    '                    '---------------------------------

    '                    'Prossimo nodo
    '                    RsRisultati.MoveNext()
    '                Loop

    '            End If

    '            'Imposto XmlRisultati come figlio del documento principale
    '            XmlDoc.AppendChild(XmlRisultati)

    '            'Restituisco in uscita la stringa creata
    '            StringaXML = XmlDoc.InnerXml

    '            'Distruggo gli oggetti
    '            XmlRisultati = Nothing
    '            XmlNodi = Nothing
    '            XmlDoc = Nothing
    '            '///////////////////////////////////////////////



    '        Case enum_AWS_Xml.Xml_DECODIFICA
    '            '///////////////////////////////////////////////
    '            '/// DECODIFICA ////////////////////////////////
    '            '///////////////////////////////////////////////

    '            'Carico la stringa XML nel documento
    '            XmlDoc.LoadXml(StringaXML)

    '            'Prelevo il nodo XmlParametri
    '            XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

    '            Errore = CStr(XmlRisultati.GetAttribute("errore"))

    '            If Errore = "" Then

    '                '----- Definisco la struttura del DataTable

    '                DtRisultati.Columns.Add(New DataColumn("fr_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("fr_des", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("veg_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("veg_des", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("grsp_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("grsp_des", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("tempocarenza", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("for_veg_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("for_veg_av_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("av_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("av_des_vol", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("av_gru", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("av_gru_des", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("note", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("limiteinterventi", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("udm_cod", GetType(String)))
    '                DtRisultati.Columns.Add(New DataColumn("udm_sim", GetType(String)))

    '                XMLs_Dati = XmlRisultati.GetElementsByTagName("DATI")

    '                For i = 0 To XMLs_Dati.Count - 1

    '                    XmlDati = XMLs_Dati.Item(i)

    '                    Dr = DtRisultati.NewRow

    '                    'Definisco i valori
    '                    Dr.Item("fr_cod") = CStr(XmlDati.GetAttribute("fr_cod"))
    '                    Dr.Item("fr_des") = CStr(XmlDati.GetAttribute("fr_des"))
    '                    Dr.Item("veg_cod") = CStr(XmlDati.GetAttribute("veg_cod"))
    '                    Dr.Item("veg_des") = CStr(XmlDati.GetAttribute("veg_des"))
    '                    Dr.Item("grsp_cod") = CStr(XmlDati.GetAttribute("grsp_cod"))
    '                    Dr.Item("grsp_des") = CStr(XmlDati.GetAttribute("grsp_des"))
    '                    Dr.Item("tempocarenza") = CStr(XmlDati.GetAttribute("tempocarenza"))
    '                    Dr.Item("for_veg_cod") = CStr(XmlDati.GetAttribute("for_veg_cod"))
    '                    Dr.Item("for_veg_av_cod") = CStr(XmlDati.GetAttribute("for_veg_av_cod"))
    '                    Dr.Item("av_cod") = CStr(XmlDati.GetAttribute("av_cod"))
    '                    Dr.Item("av_des_vol") = CStr(XmlDati.GetAttribute("av_des_vol"))
    '                    Dr.Item("av_gru") = CStr(XmlDati.GetAttribute("av_gru"))
    '                    Dr.Item("av_gru_des") = CStr(XmlDati.GetAttribute("av_gru_des"))
    '                    Dr.Item("note") = CStr(XmlDati.GetAttribute("note"))
    '                    Dr.Item("limiteinterventi") = CStr(XmlDati.GetAttribute("limiteinterventi"))
    '                    Dr.Item("udm_cod") = CStr(XmlDati.GetAttribute("udm_cod"))
    '                    Dr.Item("udm_sim") = CStr(XmlDati.GetAttribute("udm_sim"))

    '                    DtRisultati.Rows.Add(Dr)

    '                Next


    '            Else

    '                DtRisultati = Nothing

    '            End If

    '            'Distruggo gli oggetti
    '            XMLs_Dati = Nothing
    '            XmlRisultati = Nothing
    '            XmlDoc = Nothing

    '    End Select

    'End Sub


    '###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulati_SpecieVegetali_Infestanti_Dosi( _
                                    ByVal Operazione As enum_AWS_Xml, _
                                    ByRef StringaXML As String, _
                                    ByRef DtRisultati As DataTable, _
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlDati As Xml.XmlElement
        Dim XmlNodi As Xml.XmlNodeList
        Dim XMLs_Dati As Xml.XmlNodeList

        Dim i As Integer

        Dim Dr As DataRow

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <DATI fr_cod="..." veg_cod="..." 
        '             for_veg_av_dos_cod="..." for_veg_av_cod="..." 
        '             dose_min="..."  dose_max="..." udm_cod="..." udm_sim="..." 
        '             da_ff_cod="..." a_ff_cod="..." 
        '             da_epoca_1="..." a_epoca_1="..." da_epoca_2="..." a_epoca_2="..." 
        '             mdi_cod="..." note="..." limiteinterventi ="..." udm_cod_limite"..." ep_cod_dpi="..."/>
        '       <DATI fr_cod="..." veg_cod="..." 
        '             for_veg_av_dos_cod="..." for_veg_av_cod="..." 
        '             dose_min="..."  dose_max="..." udm_cod="..." udm_sim="..." 
        '             da_ff_cod="..." a_ff_cod="..." 
        '             da_epoca_1="..." a_epoca_1="..." da_epoca_2="..." a_epoca_2="..." 
        '             mdi_cod="..." note="..." limiteinterventi ="..." udm_cod_limite"..." ep_cod_dpi="..."/>
        '   </RISULTATI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                If (Not IsNothing(DtRisultati)) Then

                    For i = 0 To DtRisultati.Rows.Count - 1

                        '---------------------------------

                        'Creo il nodo DATI
                        XmlDati = XmlDoc.CreateElement("DATI")

                        'Imposto gli attributi
                        XmlDati.SetAttribute("fr_cod", DtRisultati.Rows(i).Item("Fr_Cod").ToString)
                        XmlDati.SetAttribute("veg_cod", DtRisultati.Rows(i).Item("Veg_Cod").ToString)
                        XmlDati.SetAttribute("for_veg_av_dos_cod", DtRisultati.Rows(i).Item("for_veg_av_dos_cod").ToString)
                        XmlDati.SetAttribute("for_veg_av_cod", DtRisultati.Rows(i).Item("for_veg_av_cod").ToString)
                        'XmlDati.SetAttribute("dose_min", DtRisultati.Rows(i).Item("dose_min").ToString)
                        'XmlDati.SetAttribute("dose_max", DtRisultati.Rows(i).Item("dose_max").ToString)
                        XmlDati.SetAttribute("dose_min", Math.Round(DtRisultati.Rows(i).Item("Dose_Min"), 3).ToString)
                        XmlDati.SetAttribute("dose_max", Math.Round(DtRisultati.Rows(i).Item("Dose_Max"), 3).ToString)
                        XmlDati.SetAttribute("udm_cod", DtRisultati.Rows(i).Item("UDM_Cod").ToString)
                        XmlDati.SetAttribute("udm_sim", DtRisultati.Rows(i).Item("UDM_Sim").ToString)
                        XmlDati.SetAttribute("da_ff_cod", DtRisultati.Rows(i).Item("da_ff_cod").ToString)
                        XmlDati.SetAttribute("a_ff_cod", DtRisultati.Rows(i).Item("a_ff_cod").ToString)
                        XmlDati.SetAttribute("da_epoca_1", DtRisultati.Rows(i).Item("da_epoca_1").ToString)
                        XmlDati.SetAttribute("a_epoca_1", DtRisultati.Rows(i).Item("a_epoca_1").ToString)
                        XmlDati.SetAttribute("da_epoca_2", DtRisultati.Rows(i).Item("da_epoca_2").ToString)
                        XmlDati.SetAttribute("a_epoca_2", DtRisultati.Rows(i).Item("a_epoca_2").ToString)
                        XmlDati.SetAttribute("mdi_cod", DtRisultati.Rows(i).Item("mdi_cod").ToString)
                        XmlDati.SetAttribute("note", DtRisultati.Rows(i).Item("note").ToString)
                        XmlDati.SetAttribute("limiteinterventi", DtRisultati.Rows(i).Item("limiteinterventi").ToString)
                        XmlDati.SetAttribute("udm_cod_limite", DtRisultati.Rows(i).Item("udm_cod_limite").ToString)

                        'Imposto XmlDati come figlio del nodo XmlRisultati
                        XmlRisultati.AppendChild(XmlDati)

                        '---------------------------------
                    Next

                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlNodi = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

                Errore = CStr(XmlRisultati.GetAttribute("errore"))

                If Errore = "" Then

                    '----- Definisco la struttura del DataTable

                    DtRisultati.Columns.Add(New DataColumn("fr_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("veg_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("for_veg_av_dos_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("for_veg_av_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("dose_min", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("dose_max", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("udm_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("udm_sim", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("da_ff_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("a_ff_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("da_epoca_1", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("a_epoca_1", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("da_epoca_2", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("a_epoca_2", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("mdi_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("note", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("limiteinterventi", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("udm_cod_limite", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("ep_cod_dpi", GetType(String)))

                    XMLs_Dati = XmlRisultati.GetElementsByTagName("DATI")

                    For i = 0 To XMLs_Dati.Count - 1

                        XmlDati = XMLs_Dati.Item(i)

                        Dr = DtRisultati.NewRow

                        'Definisco i valori
                        Dr.Item("fr_cod") = CStr(XmlDati.GetAttribute("fr_cod"))
                        Dr.Item("veg_cod") = CStr(XmlDati.GetAttribute("veg_cod"))
                        Dr.Item("for_veg_av_dos_cod") = CStr(XmlDati.GetAttribute("for_veg_av_dos_cod"))
                        Dr.Item("for_veg_av_cod") = CStr(XmlDati.GetAttribute("for_veg_av_cod"))
                        Dr.Item("dose_min") = CStr(XmlDati.GetAttribute("dose_min"))
                        Dr.Item("dose_max") = CStr(XmlDati.GetAttribute("dose_max"))
                        Dr.Item("udm_cod") = CStr(XmlDati.GetAttribute("udm_cod"))
                        Dr.Item("udm_sim") = CStr(XmlDati.GetAttribute("udm_sim"))
                        Dr.Item("da_ff_cod") = CStr(XmlDati.GetAttribute("da_ff_cod"))
                        Dr.Item("a_ff_cod") = CStr(XmlDati.GetAttribute("a_ff_cod"))
                        Dr.Item("da_epoca_1") = CStr(XmlDati.GetAttribute("da_epoca_1"))
                        Dr.Item("a_epoca_1") = CStr(XmlDati.GetAttribute("a_epoca_1"))
                        Dr.Item("da_epoca_2") = CStr(XmlDati.GetAttribute("da_epoca_2"))
                        Dr.Item("a_epoca_2") = CStr(XmlDati.GetAttribute("a_epoca_2"))
                        Dr.Item("mdi_cod") = CStr(XmlDati.GetAttribute("mdi_cod"))
                        Dr.Item("note") = CStr(XmlDati.GetAttribute("note"))
                        Dr.Item("limiteinterventi") = CStr(XmlDati.GetAttribute("limiteinterventi"))
                        Dr.Item("udm_cod_limite") = CStr(XmlDati.GetAttribute("udm_cod_limite"))
                        Dr.Item("ep_cod_dpi") = CStr(XmlDati.GetAttribute("ep_cod_dpi"))

                        DtRisultati.Rows.Add(Dr)

                    Next


                Else

                    DtRisultati = Nothing

                End If

                'Distruggo gli oggetti
                XMLs_Dati = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing

                '///////////////////////////////////////////////

        End Select

    End Sub






    '###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulati_SpecieVegetali_Avversita_Dosi( _
                                    ByVal Operazione As enum_AWS_Xml, _
                                    ByRef StringaXML As String, _
                                    ByRef DtRisultati As DataTable, _
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlDati As Xml.XmlElement
        Dim XmlNodi As Xml.XmlNodeList
        Dim XMLs_Dati As Xml.XmlNodeList

        Dim i As Integer

        Dim Dr As DataRow

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <DATI fr_cod="..." veg_cod="..." 
        '             for_veg_av_dos_cod="..." for_veg_av_cod="..." 
        '             dose_min="..."  dose_max="..." udm_cod="..." udm_sim="..." 
        '             da_ff_cod="..." a_ff_cod="..." 
        '             da_epoca_1="..." a_epoca_1="..." da_epoca_2="..." a_epoca_2="..." 
        '             mdi_cod="..." note="..." limiteinterventi ="..." udm_cod_limite/>
        '       <DATI fr_cod="..." veg_cod="..." 
        '             for_veg_av_dos_cod="..." for_veg_av_cod="..." 
        '             dose_min="..."  dose_max="..." udm_cod="..." udm_sim="..." 
        '             da_ff_cod="..." a_ff_cod="..." 
        '             da_epoca_1="..." a_epoca_1="..." da_epoca_2="..." a_epoca_2="..." 
        '             mdi_cod="..." note="..." limiteinterventi ="..." udm_cod_limite/>
        '   </RISULTATI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                'Se il recordset e' pieno ...
                If (Not IsNothing(DtRisultati)) Then

                    For i = 0 To DtRisultati.Rows.Count - 1

                        '---------------------------------

                        'Creo il nodo DATI
                        XmlDati = XmlDoc.CreateElement("DATI")

                        'Imposto gli attributi
                        XmlDati.SetAttribute("fr_cod", DtRisultati.Rows(i).Item("Fr_Cod").ToString)
                        XmlDati.SetAttribute("veg_cod", DtRisultati.Rows(i).Item("Veg_Cod").ToString)
                        XmlDati.SetAttribute("for_veg_av_dos_cod", DtRisultati.Rows(i).Item("for_veg_av_dos_cod").ToString)
                        XmlDati.SetAttribute("for_veg_av_cod", DtRisultati.Rows(i).Item("for_veg_av_cod").ToString)
                        'XmlDati.SetAttribute("dose_min", DtRisultati.Rows(i).Item("dose_min").ToString)
                        'XmlDati.SetAttribute("dose_max", DtRisultati.Rows(i).Item("dose_max").ToString)
                        XmlDati.SetAttribute("dose_min", Math.Round(DtRisultati.Rows(i).Item("Dose_Min"), 3).ToString)
                        XmlDati.SetAttribute("dose_max", Math.Round(DtRisultati.Rows(i).Item("Dose_Max"), 3).ToString)
                        XmlDati.SetAttribute("udm_cod", DtRisultati.Rows(i).Item("UDM_Cod").ToString)
                        XmlDati.SetAttribute("udm_sim", DtRisultati.Rows(i).Item("UDM_Sim").ToString)
                        XmlDati.SetAttribute("da_ff_cod", DtRisultati.Rows(i).Item("da_ff_cod").ToString)
                        XmlDati.SetAttribute("a_ff_cod", DtRisultati.Rows(i).Item("a_ff_cod").ToString)
                        XmlDati.SetAttribute("da_epoca_1", DtRisultati.Rows(i).Item("da_epoca_1").ToString)
                        XmlDati.SetAttribute("a_epoca_1", DtRisultati.Rows(i).Item("a_epoca_1").ToString)
                        XmlDati.SetAttribute("da_epoca_2", DtRisultati.Rows(i).Item("da_epoca_2").ToString)
                        XmlDati.SetAttribute("a_epoca_2", DtRisultati.Rows(i).Item("a_epoca_2").ToString)
                        XmlDati.SetAttribute("mdi_cod", DtRisultati.Rows(i).Item("mdi_cod").ToString)
                        XmlDati.SetAttribute("note", DtRisultati.Rows(i).Item("note").ToString)
                        XmlDati.SetAttribute("limiteinterventi", DtRisultati.Rows(i).Item("limiteinterventi").ToString)
                        XmlDati.SetAttribute("udm_cod_limite", DtRisultati.Rows(i).Item("udm_cod_limite").ToString)

                        'Imposto XmlDati come figlio del nodo XmlRisultati
                        XmlRisultati.AppendChild(XmlDati)

                        '---------------------------------
                    Next


                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlNodi = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

                Errore = CStr(XmlRisultati.GetAttribute("errore"))

                If Errore = "" Then

                    '----- Definisco la struttura del DataTable

                    DtRisultati.Columns.Add(New DataColumn("fr_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("veg_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("for_veg_av_dos_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("for_veg_av_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("dose_min", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("dose_max", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("udm_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("udm_sim", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("da_ff_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("a_ff_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("da_epoca_1", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("a_epoca_1", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("da_epoca_2", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("a_epoca_2", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("mdi_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("note", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("limiteinterventi", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("udm_cod_limite", GetType(String)))

                    XMLs_Dati = XmlRisultati.GetElementsByTagName("DATI")

                    For i = 0 To XMLs_Dati.Count - 1

                        XmlDati = XMLs_Dati.Item(i)

                        Dr = DtRisultati.NewRow

                        'Definisco i valori
                        Dr.Item("fr_cod") = CStr(XmlDati.GetAttribute("fr_cod"))
                        Dr.Item("veg_cod") = CStr(XmlDati.GetAttribute("veg_cod"))
                        Dr.Item("for_veg_av_dos_cod") = CStr(XmlDati.GetAttribute("for_veg_av_dos_cod"))
                        Dr.Item("for_veg_av_cod") = CStr(XmlDati.GetAttribute("for_veg_av_cod"))
                        Dr.Item("dose_min") = CStr(XmlDati.GetAttribute("dose_min"))
                        Dr.Item("dose_max") = CStr(XmlDati.GetAttribute("dose_max"))
                        Dr.Item("udm_cod") = CStr(XmlDati.GetAttribute("udm_cod"))
                        Dr.Item("udm_sim") = CStr(XmlDati.GetAttribute("udm_sim"))
                        Dr.Item("da_ff_cod") = CStr(XmlDati.GetAttribute("da_ff_cod"))
                        Dr.Item("a_ff_cod") = CStr(XmlDati.GetAttribute("a_ff_cod"))
                        Dr.Item("da_epoca_1") = CStr(XmlDati.GetAttribute("da_epoca_1"))
                        Dr.Item("a_epoca_1") = CStr(XmlDati.GetAttribute("a_epoca_1"))
                        Dr.Item("da_epoca_2") = CStr(XmlDati.GetAttribute("da_epoca_2"))
                        Dr.Item("a_epoca_2") = CStr(XmlDati.GetAttribute("a_epoca_2"))
                        Dr.Item("mdi_cod") = CStr(XmlDati.GetAttribute("mdi_cod"))
                        Dr.Item("note") = CStr(XmlDati.GetAttribute("note"))
                        Dr.Item("limiteinterventi") = CStr(XmlDati.GetAttribute("limiteinterventi"))
                        Dr.Item("udm_cod_limite") = CStr(XmlDati.GetAttribute("udm_cod_limite"))

                        DtRisultati.Rows.Add(Dr)

                    Next


                Else

                    DtRisultati = Nothing

                End If

                'Distruggo gli oggetti
                XMLs_Dati = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing

                '///////////////////////////////////////////////

        End Select

    End Sub





    ''###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulati_SpecieVegetali_Avversita( _
                                    ByVal Operazione As enum_AWS_Xml, _
                                    ByRef StringaXML As String, _
                                    ByRef DtRisultati As DataTable, _
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlDati As Xml.XmlElement
        Dim XMLs_Dati As Xml.XmlNodeList
        Dim XmlNodi As Xml.XmlNodeList
        Dim Dr As DataRow

        Dim i As Integer

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <DATI fr_cod="..." fr_des="..."  
        '             veg_cod="..." veg_des="..." grsp_cod="..." grsp_des="..." tempocarenza="..."
        '             for_veg_av_cod="..." for_veg_cod="..."
        '             av_cod="..." av_des_vol="..." av_gru="..." av_gru_des="..."
        '             note="..." limiteinterventi ="..." udm_cod="..." udm_sim="..."/>
        '       <DATI fr_cod="..." fr_des="..."  
        '             veg_cod="..." veg_des="..." grsp_cod="..." grsp_des="..." tempocarenza="..."
        '             for_veg_av_cod="..." for_veg_cod="..."
        '             av_cod="..." av_des_vol="..." av_gru="..." av_gru_des="..."
        '             note="..." limiteinterventi ="..." udm_cod="..." udm_sim="..."/>
        '   </RISULTATI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                'Se il recordset e' pieno ...
                If (Not IsNothing(DtRisultati)) Then

                    For i = 0 To DtRisultati.Rows.Count - 1

                        '---------------------------------

                        'Creo il nodo DATI
                        XmlDati = XmlDoc.CreateElement("DATI")

                        'Imposto gli attributi
                        XmlDati.SetAttribute("fr_cod", DtRisultati.Rows(i).Item("fr_cod").ToString)
                        XmlDati.SetAttribute("fr_des", DtRisultati.Rows(i).Item("fr_des").ToString)
                        XmlDati.SetAttribute("veg_cod", DtRisultati.Rows(i).Item("veg_cod").ToString)
                        XmlDati.SetAttribute("veg_des", DtRisultati.Rows(i).Item("veg_des").ToString)
                        XmlDati.SetAttribute("grsp_cod", DtRisultati.Rows(i).Item("grsp_cod").ToString)
                        XmlDati.SetAttribute("grsp_des", DtRisultati.Rows(i).Item("grsp_des").ToString)
                        XmlDati.SetAttribute("tempocarenza", DtRisultati.Rows(i).Item("tempocarenza").ToString)
                        XmlDati.SetAttribute("for_veg_cod", DtRisultati.Rows(i).Item("for_veg_cod").ToString)
                        XmlDati.SetAttribute("for_veg_av_cod", DtRisultati.Rows(i).Item("for_veg_av_cod").ToString)
                        XmlDati.SetAttribute("av_cod", DtRisultati.Rows(i).Item("av_cod").ToString)
                        XmlDati.SetAttribute("av_des_vol", DtRisultati.Rows(i).Item("av_des_vol").ToString)
                        XmlDati.SetAttribute("av_des_lat", DtRisultati.Rows(i).Item("av_des_lat").ToString)
                        XmlDati.SetAttribute("av_gru", DtRisultati.Rows(i).Item("av_gru").ToString)
                        XmlDati.SetAttribute("av_gru_des", DtRisultati.Rows(i).Item("av_gru_des").ToString)
                        XmlDati.SetAttribute("av_gru_des_lat", DtRisultati.Rows(i).Item("av_gru_des_lat").ToString)
                        XmlDati.SetAttribute("note", DtRisultati.Rows(i).Item("note").ToString)
                        XmlDati.SetAttribute("limiteinterventi", DtRisultati.Rows(i).Item("limiteinterventi").ToString)
                        XmlDati.SetAttribute("udm_cod", DtRisultati.Rows(i).Item("udm_cod").ToString)
                        XmlDati.SetAttribute("udm_sim", DtRisultati.Rows(i).Item("udm_sim").ToString)

                        'Imposto XmlDati come figlio del nodo XmlRisultati
                        XmlRisultati.AppendChild(XmlDati)

                        '---------------------------------

                    Next

                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlNodi = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

                Errore = CStr(XmlRisultati.GetAttribute("errore"))

                If Errore = "" Then

                    '----- Definisco la struttura del DataTable

                    DtRisultati.Columns.Add(New DataColumn("fr_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("fr_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("veg_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("veg_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("grsp_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("grsp_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("tempocarenza", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("for_veg_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("for_veg_av_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_des_vol", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_des_lat", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_gru", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_gru_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_gru_des_lat", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("note", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("limiteinterventi", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("udm_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("udm_sim", GetType(String)))

                    XMLs_Dati = XmlRisultati.GetElementsByTagName("DATI")

                    For i = 0 To XMLs_Dati.Count - 1

                        XmlDati = XMLs_Dati.Item(i)

                        Dr = DtRisultati.NewRow

                        'Definisco i valori
                        Dr.Item("fr_cod") = CStr(XmlDati.GetAttribute("fr_cod"))
                        Dr.Item("fr_des") = CStr(XmlDati.GetAttribute("fr_des"))
                        Dr.Item("veg_cod") = CStr(XmlDati.GetAttribute("veg_cod"))
                        Dr.Item("veg_des") = CStr(XmlDati.GetAttribute("veg_des"))
                        Dr.Item("grsp_cod") = CStr(XmlDati.GetAttribute("grsp_cod"))
                        Dr.Item("grsp_des") = CStr(XmlDati.GetAttribute("grsp_des"))
                        Dr.Item("tempocarenza") = CStr(XmlDati.GetAttribute("tempocarenza"))
                        Dr.Item("for_veg_cod") = CStr(XmlDati.GetAttribute("for_veg_cod"))
                        Dr.Item("for_veg_av_cod") = CStr(XmlDati.GetAttribute("for_veg_av_cod"))
                        Dr.Item("av_cod") = CStr(XmlDati.GetAttribute("av_cod"))
                        Dr.Item("av_des_vol") = CStr(XmlDati.GetAttribute("av_des_vol"))
                        Dr.Item("av_des_lat") = CStr(XmlDati.GetAttribute("av_des_lat"))
                        Dr.Item("av_gru") = CStr(XmlDati.GetAttribute("av_gru"))
                        Dr.Item("av_gru_des") = CStr(XmlDati.GetAttribute("av_gru_des"))
                        Dr.Item("av_gru_des_lat") = CStr(XmlDati.GetAttribute("av_gru_des_lat"))
                        Dr.Item("note") = CStr(XmlDati.GetAttribute("note"))
                        Dr.Item("limiteinterventi") = CStr(XmlDati.GetAttribute("limiteinterventi"))
                        Dr.Item("udm_cod") = CStr(XmlDati.GetAttribute("udm_cod"))
                        Dr.Item("udm_sim") = CStr(XmlDati.GetAttribute("udm_sim"))

                        DtRisultati.Rows.Add(Dr)

                    Next


                Else

                    DtRisultati = Nothing

                End If

                'Distruggo gli oggetti
                XMLs_Dati = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing


        End Select

    End Sub


    ''###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulati_SpecieVegetali_Infestanti( _
                                    ByVal Operazione As enum_AWS_Xml, _
                                    ByRef StringaXML As String, _
                                    ByRef DtRisultati As DataTable, _
                                    ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlRisultati As Xml.XmlElement
        Dim XmlDati As Xml.XmlElement
        Dim XMLs_Dati As Xml.XmlNodeList
        Dim XmlNodi As Xml.XmlNodeList
        Dim Dr As DataRow

        Dim i As Integer

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <DATI fr_cod="..." fr_des="..."  
        '             veg_cod="..." veg_des="..." grsp_cod="..." grsp_des="..." tempocarenza="..."
        '             for_veg_av_cod="..." for_veg_cod="..."
        '             av_cod="..." av_des_vol="..." av_gru="..." av_gru_des="..."
        '             note="..." limiteinterventi ="..." udm_cod="..." udm_sim="..."/>
        '       <DATI fr_cod="..." fr_des="..."  
        '             veg_cod="..." veg_des="..." grsp_cod="..." grsp_des="..." tempocarenza="..."
        '             for_veg_av_cod="..." for_veg_cod="..."
        '             av_cod="..." av_des_vol="..." av_gru="..." av_gru_des="..."
        '             note="..." limiteinterventi ="..." udm_cod="..." udm_sim="..."/>
        '   </RISULTATI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                'Se il recordset e' pieno ...
                If (Not IsNothing(DtRisultati)) Then

                    For i = 0 To DtRisultati.Rows.Count - 1

                        '---------------------------------

                        'Creo il nodo DATI
                        XmlDati = XmlDoc.CreateElement("DATI")

                        'Imposto gli attributi
                        XmlDati.SetAttribute("fr_cod", DtRisultati.Rows(i).Item("fr_cod").ToString)
                        XmlDati.SetAttribute("fr_des", DtRisultati.Rows(i).Item("fr_des").ToString)
                        XmlDati.SetAttribute("veg_cod", DtRisultati.Rows(i).Item("veg_cod").ToString)
                        XmlDati.SetAttribute("veg_des", DtRisultati.Rows(i).Item("veg_des").ToString)
                        XmlDati.SetAttribute("grsp_cod", DtRisultati.Rows(i).Item("grsp_cod").ToString)
                        XmlDati.SetAttribute("grsp_des", DtRisultati.Rows(i).Item("grsp_des").ToString)
                        XmlDati.SetAttribute("tempocarenza", DtRisultati.Rows(i).Item("tempocarenza").ToString)
                        XmlDati.SetAttribute("for_veg_cod", DtRisultati.Rows(i).Item("for_veg_cod").ToString)
                        XmlDati.SetAttribute("for_veg_av_cod", DtRisultati.Rows(i).Item("for_veg_av_cod").ToString)
                        XmlDati.SetAttribute("av_cod", DtRisultati.Rows(i).Item("av_cod").ToString)
                        XmlDati.SetAttribute("av_des_vol", DtRisultati.Rows(i).Item("av_des_vol").ToString)
                        XmlDati.SetAttribute("av_des_lat", DtRisultati.Rows(i).Item("av_des_lat").ToString)
                        XmlDati.SetAttribute("av_gru", DtRisultati.Rows(i).Item("av_gru").ToString)
                        XmlDati.SetAttribute("av_gru_des", DtRisultati.Rows(i).Item("av_gru_des").ToString)
                        XmlDati.SetAttribute("av_gru_des_lat", DtRisultati.Rows(i).Item("av_gru_des_lat").ToString)
                        XmlDati.SetAttribute("note", DtRisultati.Rows(i).Item("note").ToString)
                        XmlDati.SetAttribute("limiteinterventi", DtRisultati.Rows(i).Item("limiteinterventi").ToString)
                        XmlDati.SetAttribute("udm_cod", DtRisultati.Rows(i).Item("udm_cod").ToString)
                        XmlDati.SetAttribute("udm_sim", DtRisultati.Rows(i).Item("udm_sim").ToString)

                        'Imposto XmlDati come figlio del nodo XmlRisultati
                        XmlRisultati.AppendChild(XmlDati)

                        '---------------------------------

                    Next

                End If

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlNodi = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlRisultati = XmlDoc.SelectSingleNode("//RISULTATI")

                Errore = CStr(XmlRisultati.GetAttribute("errore"))

                If Errore = "" Then

                    '----- Definisco la struttura del DataTable

                    DtRisultati.Columns.Add(New DataColumn("fr_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("fr_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("veg_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("veg_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("grsp_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("grsp_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("tempocarenza", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("for_veg_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("for_veg_av_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_des_vol", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_des_lat", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_gru", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_gru_des", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("av_gru_des_lat", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("note", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("limiteinterventi", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("udm_cod", GetType(String)))
                    DtRisultati.Columns.Add(New DataColumn("udm_sim", GetType(String)))

                    XMLs_Dati = XmlRisultati.GetElementsByTagName("DATI")

                    For i = 0 To XMLs_Dati.Count - 1

                        XmlDati = XMLs_Dati.Item(i)

                        Dr = DtRisultati.NewRow

                        'Definisco i valori
                        Dr.Item("fr_cod") = CStr(XmlDati.GetAttribute("fr_cod"))
                        Dr.Item("fr_des") = CStr(XmlDati.GetAttribute("fr_des"))
                        Dr.Item("veg_cod") = CStr(XmlDati.GetAttribute("veg_cod"))
                        Dr.Item("veg_des") = CStr(XmlDati.GetAttribute("veg_des"))
                        Dr.Item("grsp_cod") = CStr(XmlDati.GetAttribute("grsp_cod"))
                        Dr.Item("grsp_des") = CStr(XmlDati.GetAttribute("grsp_des"))
                        Dr.Item("tempocarenza") = CStr(XmlDati.GetAttribute("tempocarenza"))
                        Dr.Item("for_veg_cod") = CStr(XmlDati.GetAttribute("for_veg_cod"))
                        Dr.Item("for_veg_av_cod") = CStr(XmlDati.GetAttribute("for_veg_av_cod"))
                        Dr.Item("av_cod") = CStr(XmlDati.GetAttribute("av_cod"))
                        Dr.Item("av_des_vol") = CStr(XmlDati.GetAttribute("av_des_vol"))
                        Dr.Item("av_des_lat") = CStr(XmlDati.GetAttribute("av_des_lat"))
                        Dr.Item("av_gru") = CStr(XmlDati.GetAttribute("av_gru"))
                        Dr.Item("av_gru_des") = CStr(XmlDati.GetAttribute("av_gru_des"))
                        Dr.Item("av_gru_des_lat") = CStr(XmlDati.GetAttribute("av_gru_des_lat"))
                        Dr.Item("note") = CStr(XmlDati.GetAttribute("note"))
                        Dr.Item("limiteinterventi") = CStr(XmlDati.GetAttribute("limiteinterventi"))
                        Dr.Item("udm_cod") = CStr(XmlDati.GetAttribute("udm_cod"))
                        Dr.Item("udm_sim") = CStr(XmlDati.GetAttribute("udm_sim"))

                        DtRisultati.Rows.Add(Dr)

                    Next


                Else

                    DtRisultati = Nothing

                End If

                'Distruggo gli oggetti
                XMLs_Dati = Nothing
                XmlRisultati = Nothing
                XmlDoc = Nothing


        End Select

    End Sub

    Public Sub AgroWS_XML_Parametri_Formulati_To_Principi_contesto(
                                                        ByVal Operazione As enum_AWS_Xml,
                                                        ByRef StringaXML As String,
                                                        ByRef Fr_Cod As String,
                                                        ByRef pa_cod As String,
                                                        ByRef contesto As String,
                                                        ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150"   />
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))
                XmlParametri.SetAttribute("pa_cod", CStr(pa_cod))
                XmlParametri.SetAttribute("contesto", contesto)


                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                'Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                'Opt_Avversita_Infestanti = CStr(XmlParametri.GetAttribute("opt_avversita_infestanti"))
                'Opt_Singola_Gruppo = CStr(XmlParametri.GetAttribute("opt_singola_gruppo"))
                'Av_Gru = Split(XmlParametri.GetAttribute("av_gru"), ",")
                'Av_Cod = Split(XmlParametri.GetAttribute("av_cod"), ",")
                'strPA = CStr(XmlParametri.GetAttribute("strpa"))
                'TipoRichiesto = CStr(XmlParametri.GetAttribute("tiporichiesto"))
                'TestoRicerca = CStr(XmlParametri.GetAttribute("testoricerca"))
                'Data = CStr(XmlParametri.GetAttribute("data"))
                'strFiltro = CStr(XmlParametri.GetAttribute("strfiltro"))
                'strSort = CStr(XmlParametri.GetAttribute("strsort"))

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub


    '###############################################################################################
    Public Sub AgroWS_XML_Parametri_Formulati_To_Principi( _
                                                        ByVal Operazione As enum_AWS_Xml, _
                                                        ByRef StringaXML As String, _
                                                        ByRef Fr_Cod As String, _
                                                        ByRef Errore As String)


        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement
        Dim XmlNodo As Xml.XmlElement

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150"   />
        '   </CREDENZIALI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", CStr(Fr_Cod))


                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo XmlParametri
                XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                'Prelevo gli attributi
                'Veg_Cod = CStr(XmlParametri.GetAttribute("veg_cod"))
                'Opt_Avversita_Infestanti = CStr(XmlParametri.GetAttribute("opt_avversita_infestanti"))
                'Opt_Singola_Gruppo = CStr(XmlParametri.GetAttribute("opt_singola_gruppo"))
                'Av_Gru = Split(XmlParametri.GetAttribute("av_gru"), ",")
                'Av_Cod = Split(XmlParametri.GetAttribute("av_cod"), ",")
                'strPA = CStr(XmlParametri.GetAttribute("strpa"))
                'TipoRichiesto = CStr(XmlParametri.GetAttribute("tiporichiesto"))
                'TestoRicerca = CStr(XmlParametri.GetAttribute("testoricerca"))
                'Data = CStr(XmlParametri.GetAttribute("data"))
                'strFiltro = CStr(XmlParametri.GetAttribute("strfiltro"))
                'strSort = CStr(XmlParametri.GetAttribute("strsort"))

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    '################################################################################
    Public Function TempoCarenza_from_FrCod_VegCod(ByVal FrCod As Integer,
                                                   ByVal VegCod As Integer,
                                                   ByVal FrVegCod As Integer,
                                                   ByVal GrfiCod As Integer,
                                                   ByVal CopCod As Integer,
                                                   ByVal Data As Date,
                                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                                   ByRef objParametri_Utenti As AgronicaCoreParametri
                                                   ) As Integer
        Dim Parametri As String
        Dim Risultati As String

        Dim strErr As String

        Dim XmlDoc As Xml.XmlDocument
        Dim XML_Credenziali As Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String

        Dim DtRisultati As DataTable

        Try

            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci

            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            Dim objAgroWebConfig As New AgroWebConfig

            If IsNothing(objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci) Then
                Dim objConfig As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci = objConfig.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", objParametri_Server)
            End If

            objWs.NewWS(ObjDownloadWs,
                        objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci,
                        objParametri_Utenti)


            Dim ProgressivoGIAS As Integer
            Dim SuperUser_Password As String = ""

            If IsNothing(HttpContext.Current.Session) Then
                Dim dtUtente As DataTable
                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
                dtUtente = objUtenti.Leggi_Superuser_e_ProgressivoGIAS(objParametri_Utenti.UtenteUsername, "",
                                                                       Date.Now, CType(Now.Hour, Short),
                                                                       0, objParametri_Utenti)

                If dtUtente IsNot Nothing AndAlso dtUtente.Rows.Count > 0 Then
                    ProgressivoGIAS = dtUtente.Rows(0).Item("ProgressivoGIAS")
                    SuperUser_Password = dtUtente.Rows(0).Item("Password_SuperUser")
                End If
            Else
                ProgressivoGIAS = CInt(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
                SuperUser_Password = CStr(HttpContext.Current.Session("ASG_SuperUser_Password"))
            End If

            XmlDoc = New Xml.XmlDocument
            AgroWS_XML__Credenziali(enum_AWS_Xml.Xml_CODIFICA,
                                    StrCredenziali,
                                    enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali,
                                    AgroWS_DoorKey(enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali),
                                    ProgressivoGIAS,
                                    objParametri_Server.SuperUserUsername,
                                    SuperUser_Password)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            Dim Copertura As Integer = 0
            Select Case CopCod
                Case 0, 1, 3, 4, 5, 6 'antigrandine + nessuna varia
                    Copertura = 0
                Case Else
                    Copertura = 1
            End Select

            AgroWS_XML_Parametri_Formulati_SpecieVegetali(enum_AWS_Xml.Xml_CODIFICA,
                                                          StrParametri,
                                                          FrCod, "",
                                                          VegCod,
                                                          GrfiCod,
                                                          Copertura,
                                                          FrVegCod,
                                                          Data,
                                                          strErr)


            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = AWS_Codifica_P(Parametri)

            Risultati = ObjDownloadWs.Formulati_SpecieVegetali(Parametri)

            Risultati = AWS_Decodifica_R(Risultati)
            Risultati = Risultati.Replace(">", ">" & vbCrLf)

            DtRisultati = New DataTable

            AgroWS_XML_Risultati_Formulati_SpecieVegetali(enum_AWS_Xml.Xml_DECODIFICA,
                                                          Risultati,
                                                          DtRisultati,
                                                          strErr)


            If strErr = "" Then
                If DtRisultati.Rows.Count > 0 AndAlso IsNumeric(DtRisultati.Rows(0).Item("tempocarenza")) Then
                    Return DtRisultati.Rows(0).Item("tempocarenza")
                Else
                    Return -1
                End If
            Else
                Return -1
            End If

        Catch ex As Exception
            Return 0
        End Try


    End Function


    Public Function TempoCarenza_from_Multiple_FrCod_VegCod(ByVal FrCod As String,
                                                            ByVal VegCod As String,
                                                            ByVal Data As Date,
                                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                                            ByRef objParametri_Utenti As AgronicaCoreParametri
                                                            ) As DataTable
        Dim Parametri As String
        Dim Risultati As String

        Dim strErr As String

        Dim XmlDoc As Xml.XmlDocument
        Dim XML_Credenziali As Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String

        Dim DtRisultati As DataTable

        Try

            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci

            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs,
                        Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString(),
                        objParametri_Utenti)

            XmlDoc = New Xml.XmlDocument

            AgroWS_XML__Credenziali(enum_AWS_Xml.Xml_CODIFICA, _
                                    StrCredenziali, _
                                    enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali, _
                                    AgroWS_DoorKey(enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali), _
                                    CInt(HttpContext.Current.Session("ASG_ProgressivoGIAS")), _
                                    objParametri_Server.SuperUserUsername, _
                                    CStr(HttpContext.Current.Session("ASG_SuperUser_Password")))

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            AgroWS_XML_Parametri_Formulati_SpecieVegetali(enum_AWS_Xml.Xml_CODIFICA,
                                                          StrParametri,
                                                          FrCod, "",
                                                          VegCod,
                                                          0,
                                                          0, 0,
                                                          Data,
                                                          strErr)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = AWS_Codifica_P(Parametri)

            Risultati = ObjDownloadWs.Formulati_SpecieVegetali(Parametri)

            Risultati = AWS_Decodifica_R(Risultati)
            Risultati = Risultati.Replace(">", ">" & vbCrLf)

            DtRisultati = New DataTable

            AgroWS_XML_Risultati_Formulati_SpecieVegetali(enum_AWS_Xml.Xml_DECODIFICA, _
                                                          Risultati, _
                                                          DtRisultati, _
                                                          strErr)

            If strErr = "" Then
                Return DtRisultati
            Else
                Return Nothing
            End If

        Catch ex As Exception
            Return Nothing
        End Try


    End Function

    Public Function TempoCarenza_from_Multiple_FrCod_VegCod_Intervallo(ByVal FrCod As String,
                                                   ByVal VegCod As String,
                                                    ByVal Data_Da As Date, ByVal Data_A As Date,
                                                       ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                       ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                       Optional ByVal objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing,
                                                          Optional ByVal leggiUrlDaConfigurazioniSiti As Boolean = False
                                                           ) As DataTable
        Dim Parametri As String
        Dim Risultati As String

        Dim strErr As String

        Dim XmlDoc As Xml.XmlDocument
        Dim XML_Credenziali As Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String

        Dim DtRisultati As DataTable

        Try

            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci

            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            Dim urlWs As String = String.Empty

            If Not leggiUrlDaConfigurazioniSiti Then
                Dim objAgroWebConfig As New AgroWebConfig
                urlWs = objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            End If

            If leggiUrlDaConfigurazioniSiti OrElse String.IsNullOrEmpty(urlWs) Then
                Dim objConfig As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                urlWs = objConfig.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", objParametri_Server)

                If urlWs = "" AndAlso objParametri_SuperServer IsNot Nothing Then
                    urlWs = objConfig.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", objParametri_SuperServer)
                End If
            End If

            objWs.NewWS(ObjDownloadWs,
                        urlWs,
                        objParametri_Utenti)

            Dim ProgressivoGIAS As Integer
            Dim SuperUser_Password As String = ""

            If IsNothing(HttpContext.Current) OrElse IsNothing(HttpContext.Current.Session) Then
                Dim dtUtente As DataTable
                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
                dtUtente = objUtenti.Leggi_Superuser_e_ProgressivoGIAS(objParametri_Utenti.UtenteUsername, "",
                                                                       Date.Now, CType(Now.Hour, Short),
                                                                       0, objParametri_Utenti)

                If dtUtente IsNot Nothing AndAlso dtUtente.Rows.Count > 0 Then
                    ProgressivoGIAS = dtUtente.Rows(0).Item("ProgressivoGIAS")
                    SuperUser_Password = dtUtente.Rows(0).Item("Password_SuperUser")
                End If
            Else
                ProgressivoGIAS = CInt(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
                SuperUser_Password = CStr(HttpContext.Current.Session("ASG_SuperUser_Password"))
            End If

            XmlDoc = New Xml.XmlDocument

            AgroWS_XML__Credenziali(enum_AWS_Xml.Xml_CODIFICA,
                                    StrCredenziali,
                                    enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali,
                                    AgroWS_DoorKey(enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali),
                                    ProgressivoGIAS,
                                    objParametri_Server.SuperUserUsername,
                                    SuperUser_Password)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            AgroWS_XML_Parametri_Formulati_SpecieVegetali_Intervallo(enum_AWS_Xml.Xml_CODIFICA,
                                                          StrParametri,
                                                          FrCod,
                                                          VegCod,
                                                          0,
                                                          "", 0,
                                                          Data_Da, Data_A, "",
                                                          strErr)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = AWS_Codifica_P(Parametri)

            Risultati = ObjDownloadWs.Formulati_SpecieVegetali_Intervallo(Parametri)

            Risultati = AWS_Decodifica_R(Risultati)
            Risultati = Risultati.Replace(">", ">" & vbCrLf)

            DtRisultati = New DataTable

            AgroWS_XML_Risultati_Formulati_SpecieVegetali_Intervallo(enum_AWS_Xml.Xml_DECODIFICA,
                                                          Risultati,
                                                          DtRisultati,
                                                          strErr)

            If strErr = "" Then
                Return DtRisultati
            Else
                Return Nothing
            End If

        Catch ex As Exception
            Return Nothing
        End Try


    End Function

    '################################################################################
    Public Function ComposizioneFormulatiRecupera(ByVal FrCod As String,
                                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                                  ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                  Optional ByVal IncludiDescrizioni As Boolean = True
                                                  ) As DataTable

        Const NomeRoutine As String = "AgroWs.ComposizioneFormulatiRecupera()"

        Dim Parametri As String
        Dim Risultati As String

        Dim strErr As String = ""

        Dim XmlDoc As Xml.XmlDocument
        Dim XML_Credenziali As Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String

        Dim DtRisultati As New DataTable
        Dim LinkFito As String = ""
        Dim ProgressivoGIAS As Integer
        Dim SuperUser_Password As String = ""

        Try

            XmlDoc = New Xml.XmlDocument

            'potrei non avere la sessione ...
            'in tal caso recupero i dati necessari dal DB
            If HttpContext.Current IsNot Nothing Then
                If HttpContext.Current.Session IsNot Nothing Then
                    Dim objAgroWebConfig As New AgroWebConfig
                    LinkFito = objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
                    ProgressivoGIAS = HttpContext.Current.Session("ASG_ProgressivoGIAS")
                    SuperUser_Password = HttpContext.Current.Session("ASG_SuperUser_Password")
                Else
                    RecuperaDatiDalDatabase(LinkFito, ProgressivoGIAS, SuperUser_Password, objParametri_Server, objParametri_Utenti)
                End If
            Else
                RecuperaDatiDalDatabase(LinkFito, ProgressivoGIAS, SuperUser_Password, objParametri_Server, objParametri_Utenti)
            End If

            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            objWs.NewWS(ObjDownloadWs,
                   LinkFito,
                   objParametri_Utenti)

            'Dim objAgroWebConfig As New AgroWebConfig
            'objWs.NewWS(ObjDownloadWs, _
            '                objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci, _
            '                objParametri_Utenti)

            'AgroWS_XML__Credenziali(enum_AWS_Xml.Xml_CODIFICA, _
            '                        StrCredenziali, _
            '                        enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi, _
            '                        AgroWS_DoorKey(enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi), _
            '                        CInt(HttpContext.Current.Session("ASG_ProgressivoGIAS")), _
            '                        objParametri_Server.SuperUserUsername, _
            '                        CStr(HttpContext.Current.Session("ASG_SuperUser_Password")))

            AgroWS_XML__Credenziali(enum_AWS_Xml.Xml_CODIFICA,
                                    StrCredenziali,
                                    enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi,
                                    AgroWS_DoorKey(enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi),
                                    ProgressivoGIAS,
                                    objParametri_Server.SuperUserUsername,
                                    SuperUser_Password)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            AgroWS_XML_Parametri_Formulati_PrincipiAttivi(enum_AWS_Xml.Xml_CODIFICA,
                                                          StrParametri,
                                                          FrCod, "",
                                                          strErr)


            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = AWS_Codifica_P(Parametri)

            Risultati = ObjDownloadWs.Formulati_PrincipiAttivi_2(Parametri)

            DtRisultati = New DataTable

            DtRisultati.Columns.Add(New DataColumn("Fr_Cod", GetType(Integer)))
            DtRisultati.Columns.Add(New DataColumn("Fr_Des", GetType(String)))
            DtRisultati.Columns.Add(New DataColumn("Elenco_ClassiTossicologiche", GetType(String)))
            DtRisultati.Columns.Add(New DataColumn("Elenco_PrincipiAttivi", GetType(String)))
            DtRisultati.Columns.Add(New DataColumn("Elenco_PrincipiAttiviPesi", GetType(String)))

            Dim XMLs_Formulati As Xml.XmlNodeList
            Dim XMLs_PrincipiAttivi As Xml.XmlNodeList
            Dim XMLs_ClassiTossicologiche As Xml.XmlNodeList

            Dim XmlFormulato As Xml.XmlElement
            Dim XmlPrincipioAttivo As Xml.XmlElement
            Dim XmlClasseTossicologica As Xml.XmlElement

            Dim DR As DataRow
            Dim ict As Integer = 0
            Dim ipa As Integer = 0
            Dim Testo As String = ""
            Dim i As Integer

            XmlDoc.LoadXml(Risultati)

            'Recupero l'elenco dei Formulati
            XMLs_Formulati = XmlDoc.GetElementsByTagName("FORMULATO")

            If Not IsNothing(XMLs_Formulati) Then

                For i = 0 To XMLs_Formulati.Count - 1

                    'Formulato i-esimo
                    XmlFormulato = XMLs_Formulati.Item(i)

                    'Creo la riga per il datatable
                    DR = DtRisultati.NewRow

                    DR.Item("Fr_Cod") = CInt(XmlFormulato.GetAttribute("fr_cod"))
                    DR.Item("Fr_Des") = CStr(XmlFormulato.GetAttribute("fr_des"))

                    '---------------------------------------------------
                    'CLASSE TOSSICOLOGICA ==>  cod1§des1|cod2§des2
                    'Recupero l'elenco delle Classi Tossicologiche

                    XMLs_ClassiTossicologiche = XmlFormulato.GetElementsByTagName("CLASSE_TOSSICOLOGICA")
                    Testo = ""
                    If XMLs_ClassiTossicologiche.Count > 0 Then
                        For ict = 0 To XMLs_ClassiTossicologiche.Count - 1
                            XmlClasseTossicologica = XMLs_ClassiTossicologiche.Item(ict)
                            Testo &= CStr(XmlClasseTossicologica.GetAttribute("cltoss_cod")) & "§" &
                                     CStr(XmlClasseTossicologica.GetAttribute("cltoss_des")) & "|"
                        Next
                    End If
                    If Testo <> "" Then
                        Testo = Left(Testo, Testo.Length - 1)
                    End If
                    DR.Item("Elenco_ClassiTossicologiche") = Testo

                    '---------------------------------------------------
                    'PRINCIPI ATTIVI ==> cod1§des1§titolo1|cod2§des2§titolo2
                    'Recupero l'elenco dei Principi Attivi

                    XMLs_PrincipiAttivi = XmlFormulato.GetElementsByTagName("PRINCIPIO_ATTIVO")

                    Testo = ""
                    Dim testo1 As String = ""
                    If XMLs_PrincipiAttivi.Count > 0 Then
                        For ipa = 0 To XMLs_PrincipiAttivi.Count - 1
                            XmlPrincipioAttivo = XMLs_PrincipiAttivi.Item(ipa)
                            If IncludiDescrizioni Then
                                Testo &= CStr(XmlPrincipioAttivo.GetAttribute("pa_cod")) & "§" &
                                         CStr(XmlPrincipioAttivo.GetAttribute("pa_des")) & "§" &
                                         CStr(XmlPrincipioAttivo.GetAttribute("titolo")) & "|"
                                testo1 &= CStr(XmlPrincipioAttivo.GetAttribute("pa_cod")) & "§" &
                                         CStr(XmlPrincipioAttivo.GetAttribute("pa_des")) & "§" &
                                         CStr(XmlPrincipioAttivo.GetAttribute("peso")) & "|"

                            Else
                                Testo &= CStr(XmlPrincipioAttivo.GetAttribute("pa_cod")) & "§" &
                                         CStr(XmlPrincipioAttivo.GetAttribute("titolo")) & "|"
                                testo1 &= CStr(XmlPrincipioAttivo.GetAttribute("pa_cod")) & "§" &
                                         CStr(XmlPrincipioAttivo.GetAttribute("peso")) & "|"
                            End If
                        Next
                    End If
                    If Testo <> "" Then
                        Testo = Left(Testo, Testo.Length - 1)
                    End If
                    DR.Item("Elenco_PrincipiAttivi") = Testo
                    If testo1 <> "" Then
                        testo1 = Left(testo1, testo1.Length - 1)
                    End If
                    DR.Item("Elenco_PrincipiAttiviPesi") = testo1

                    '---------------------------------------------------
                    DtRisultati.Rows.Add(DR)

                Next

            End If 'XMLs_Formulati

        Catch ex As Exception
            strErr = "[" & NomeRoutine & "] : " & ex.Message
            Dim Log As New AgronicaCoreDataProvider.LogProvider
            Log.Scrivi_LOG(objParametri_Server, NomeRoutine, strErr)
            Return Nothing
        End Try

        Return DtRisultati

    End Function

    Public Sub RecuperaDatiDalDatabase(ByRef LinkFito As String,
                                       ByRef ProgressivoGIAS As String,
                                       ByRef SuperUser_Password As String,
                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim objConf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        LinkFito = objConf.Leggi_Valore(Enum_SiteRedirector.Sito_GiasOnline,
                                        "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci",
                                        "", "", objParametri_Server)
        Dim dt As DataTable
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
        dt = objUtenti.Leggi_Superuser_e_ProgressivoGIAS(objParametri_Utenti.UtenteUsername, "",
                                                    Date.Now,
                                                    CType(Now.Hour, Short),
                                                    0, objParametri_Utenti)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            ProgressivoGIAS = dt.Rows(0).Item("ProgressivoGIAS")
            SuperUser_Password = dt.Rows(0).Item("Password_SuperUser")
        End If
    End Sub

    '################################################################################
    Public Function Formulati_Classificazioni_DT_Profitosan(ByVal FrCod As String, ByVal Stato_Cod As String,
                                                            ByVal dataRiferimento As DateTime,
                                                            ByRef strErr As String,
                                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                                            ByRef objParametri_Utenti As AgronicaCoreParametri
                                                            ) As DataTable

        Const nomeRoutine = "AgroWs.Formulati_Classificazioni_DT_Profitosan()"

        Dim Parametri As String

        Dim XmlDoc As Xml.XmlDocument
        Dim XML_Credenziali As Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String

        Dim DtRisultati As New DataTable
        Dim LinkFito As String
        Dim ProgressivoGIAS As Integer
        Dim SuperUser_Password As String

        Try

            XmlDoc = New Xml.XmlDocument

            'potrei non avere la sessione ...
            'in tal caso recupero i dati necessari dal DB
            If HttpContext.Current IsNot Nothing Then
                Dim objAgroWebConfig As New AgroWebConfig
                LinkFito = objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
                ProgressivoGIAS = HttpContext.Current.Session("ASG_ProgressivoGIAS")
                SuperUser_Password = HttpContext.Current.Session("ASG_SuperUser_Password")
            Else
                Dim objConf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                LinkFito = objConf.Leggi_Valore(Enum_SiteRedirector.Sito_GiasOnline,
                                                "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci",
                                                "", "", objParametri_Server)
                Dim dt As DataTable
                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
                dt = objUtenti.Leggi_Superuser_e_ProgressivoGIAS(objParametri_Utenti.UtenteUsername, "",
                                                            Date.Now,
                                                            CType(Now.Hour, Short),
                                                            0, objParametri_Utenti)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    ProgressivoGIAS = dt.Rows(0).Item("ProgressivoGIAS")
                    SuperUser_Password = dt.Rows(0).Item("Password_SuperUser")
                End If
            End If

            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            objWs.NewWS(ObjDownloadWs,
                   LinkFito,
                   objParametri_Utenti)

            'Dim objAgroWebConfig As New AgroWebConfig
            'objWs.NewWS(ObjDownloadWs, _
            '                objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci, _
            '                objParametri_Utenti)

            'AgroWS_XML__Credenziali(enum_AWS_Xml.Xml_CODIFICA, _
            '                        StrCredenziali, _
            '                        enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi, _
            '                        AgroWS_DoorKey(enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi), _
            '                        CInt(HttpContext.Current.Session("ASG_ProgressivoGIAS")), _
            '                        objParametri_Server.SuperUserUsername, _
            '                        CStr(HttpContext.Current.Session("ASG_SuperUser_Password")))

            AgroWS_XML__Credenziali(
                Operazione:=enum_AWS_Xml.Xml_CODIFICA,
                StringaXML:=StrCredenziali,
                Scheda:=enum_AWS_Schede.AWS_Fitofarmaci_Formulati_Classificazioni,
                DoorKey:=AgroWS_DoorKey(enum_AWS_Schede.AWS_Fitofarmaci_Formulati_Classificazioni),
                CodiceGias:=ProgressivoGIAS,
                SuperUser_Piva:=objParametri_Server.PivaSuperUser,
                SuperUser_UserName:=objParametri_Server.SuperUserUsername,
                SuperUser_Password:=SuperUser_Password,
                Utente_UserName:=objParametri_Server.UtenteUsername,
                Utente_Password:=""
            )

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            AgroWS_XML_Parametri_Formulati_Classificazioni(enum_AWS_Xml.Xml_CODIFICA,
                                                          StrParametri,
                                                          6,
                                                          FrCod,
                                                          1, Stato_Cod,
                                                          dataRiferimento,
                                                          "",
                                                          "",
                                                          strErr)


            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = AWS_Codifica_P(Parametri)

            DtRisultati = ObjDownloadWs.Formulati_Classificazioni_DT_Profitosan(Parametri, strErr)

        Catch ex As Exception
            strErr = "[" & nomeRoutine & "] : " & ex.Message
            Dim Log As New AgronicaCoreDataProvider.LogProvider
            Log.Scrivi_LOG(objParametri_Server, nomeRoutine, strErr)
            Return Nothing
        End Try

        Return DtRisultati

    End Function

    '#############################################################################################################
    Public Function ComposizioneFormulatiRecupera(ByRef DT As DataTable, _
                                                  ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                  ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                  ) As DataTable

        Dim NomeRoutine As String = "AgroWs.ComposizioneFormulatiRecupera()"
        Dim MessaggioErrore As String = ""
        Dim DTfor As DataTable

        Dim Parametri As String
        Dim Risultati As String

        Dim strErr As String

        Dim XmlDoc As Xml.XmlDocument
        Dim XML_Credenziali As Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String

        Try

            Dim ElencoFormulati As String = ","
            Dim i As Integer = 0
            Dim FrCod As Integer

            'Recupero l'elenco dei formulati 
            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                For i = 0 To DT.Rows.Count - 1
                    FrCod = DT.Rows(i).Item("Pro_Cod")

                    'Se l'elemento e' un formulato
                    If DT.Rows(i).Item("Elem_Cod") = FORMULATI Then

                        'Se l'elemento non è gia' presente dentro la stringa ...
                        If InStr(1, ElencoFormulati, "," & FrCod.ToString & ",", CompareMethod.Text) = 0 Then
                            ElencoFormulati += FrCod.ToString & ","
                        End If
                    End If
                Next


                'Tolgo il primo e ultimo carattere (,) ... 
                If ElencoFormulati.Length > 0 Then
                    ElencoFormulati = Mid(ElencoFormulati, 2)
                    ElencoFormulati = Mid(ElencoFormulati, 1, ElencoFormulati.Length - 1)
                End If

                '' ''Creo la stringa XML di richiesta

                ' ''Dim XmlDoc As New Xml.XmlDocument
                ' ''Dim XmlCredenziali As Xml.XmlElement
                ' ''Dim XmlParametri As Xml.XmlElement
                ' ''Dim XmlNodo As Xml.XmlElement
                ' ''Dim StringaXML As String

                ' ''Dim WsScheda As String
                ' ''Dim WsDoorKey As String
                ' ''Dim WsCodiceGias As String
                ' ''Dim WsUsernameSuperuser As String
                ' ''Dim WsPasswordSuperuser As String

                '' ''===========================================================================================
                '' ''   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
                '' ''       <PARAMETRI fr_cod=""                        oppure  "239,150" />
                '' ''   </CREDENZIALI>
                '' ''===========================================================================================

                ' ''WsScheda = "1080"                                               'AWS_Fitofarmaci_Formulati_PrincipiAttivi
                ' ''WsDoorKey = "portone_grth45ksh4ghajnl32149"
                ' ''WsCodiceGias = objSession("ASG_ProgressivoGIAS")
                ' ''WsUsernameSuperuser = objSession("ASG_SuperUser_Username")         'Session("ASG_SuperUser_Username_Crypt")
                ' ''WsPasswordSuperuser = objSession("ASG_SuperUser_Password")         'Session("ASG_SuperUser_Password_Crypt")

                '' ''Creo il nodo CREDENZIALI
                ' ''XmlCredenziali = XmlDoc.CreateElement("CREDENZIALI")

                '' ''Imposto gli attributi
                ' ''XmlCredenziali.SetAttribute("scheda", CStr(WsScheda))
                ' ''XmlCredenziali.SetAttribute("doorkey", CStr(WsDoorKey))
                ' ''XmlCredenziali.SetAttribute("codicegias", CStr(WsCodiceGias))
                ' ''XmlCredenziali.SetAttribute("username", CStr(WsUsernameSuperuser))
                ' ''XmlCredenziali.SetAttribute("password", CStr(WsPasswordSuperuser))

                '' ''Imposto XmlParametri come figlio del documento principale
                ' ''XmlDoc.AppendChild(XmlCredenziali)

                '' ''Creo il nodo PARAMETRI
                ' ''XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                '' ''Imposto gli attributi
                ' ''XmlParametri.SetAttribute("fr_cod", ElencoFormulati)

                '' ''Imposto XmlParametri come figlio del documento principale
                ' ''XmlCredenziali.AppendChild(XmlParametri)

                '' ''Restituisco in uscita la stringa creata
                ' ''StringaXML = XmlDoc.InnerXml

                '' ''Distruggo gli oggetti
                ' ''XmlNodo = Nothing
                ' ''XmlParametri = Nothing
                '' ''XmlDoc = Nothing

                '' ''//////////////////////////////////////////////////////////
                '' ''/////   Chiamo la funzione del webservice per recuperare la composizione
                '' ''//////////////////////////////////////////////////////////

                ' ''Dim LinkWsFitofarmaci As String
                ' ''Dim WsRisposta As String = ""
                ' ''Dim objCoreWebService As New AgronicaCoreWebService.AgroWs
                ' ''StringaXML = objCoreWebService.AWS_Codifica_P(StringaXML)

                Try
                    ' ''Dim WsFito As New WS_Fitofarmaci.AgroWS_Fitofarmaci

                    '' ''l'indirizzo dei web service deve stare solo nella tabella 'configurazione_siti'
                    '' ''eliminata chiave nel web.config
                    ' ''Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)
                    ' ''If objWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci <> "" Then
                    ' ''    LinkWsFitofarmaci = objWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
                    ' ''End If

                    '' ''If ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString = "" Then
                    '' ''    LinkWsFitofarmaci = objServer.MapPath("https://ws.netagronica.it/AgronicaWebService/AgroWS_Fitofarmaci.asmx")
                    '' ''Else
                    '' ''    LinkWsFitofarmaci = ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString
                    '' ''End If

                    ' ''WsFito.Url = LinkWsFitofarmaci
                    ' ''WsFito.Timeout = 60000
                    ' ''WsRisposta = WsFito.Formulati_PrincipiAttivi_2(StringaXML)

                    Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci

                    Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                    Dim objAgroWebConfig As New AgroWebConfig

                    objWs.NewWS(ObjDownloadWs, _
                                    objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci, _
                                    objParametri_Utenti)

                    XmlDoc = New Xml.XmlDocument

                    AgroWS_XML__Credenziali(enum_AWS_Xml.Xml_CODIFICA, _
                                            StrCredenziali, _
                                            enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi, _
                                            AgroWS_DoorKey(enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi), _
                                            CInt(HttpContext.Current.Session("ASG_ProgressivoGIAS")), _
                                            objParametri_Server.SuperUserUsername, _
                                            CStr(HttpContext.Current.Session("ASG_SuperUser_Password")))

                    XmlDoc.LoadXml(StrCredenziali)

                    XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

                    AgroWS_XML_Parametri_Formulati_PrincipiAttivi(enum_AWS_Xml.Xml_CODIFICA,
                                                                  StrParametri,
                                                                  ElencoFormulati, "",
                                                                  strErr)


                    XML_Credenziali.InnerXml = StrParametri

                    Parametri = XmlDoc.OuterXml

                    Parametri = AWS_Codifica_P(Parametri)

                    Risultati = ObjDownloadWs.Formulati_PrincipiAttivi_2(Parametri)

                Catch ex As Exception
                    Throw New Exception("Webservice fitofarmaci : " & ex.Message)
                End Try


                '//////////////////////////////////////////////////////////
                '/////   Decodifico il risultato ==> DataTable
                '//////////////////////////////////////////////////////////

                'Dim DTfor As New DataTable
                DTfor = New DataTable
                DTfor.Columns.Add(New DataColumn("Fr_Cod", GetType(Integer)))
                DTfor.Columns.Add(New DataColumn("Fr_Des", GetType(String)))
                DTfor.Columns.Add(New DataColumn("Elenco_ClassiTossicologiche", GetType(String)))
                DTfor.Columns.Add(New DataColumn("Elenco_PrincipiAttivi", GetType(String)))


                'Dim XmlDoc As New Xml.XmlDocument
                Dim XMLs_Formulati As Xml.XmlNodeList
                Dim XMLs_PrincipiAttivi As Xml.XmlNodeList
                Dim XMLs_ClassiTossicologiche As Xml.XmlNodeList

                Dim XmlFormulato As Xml.XmlElement
                Dim XmlPrincipioAttivo As Xml.XmlElement
                Dim XmlClasseTossicologica As Xml.XmlElement

                Dim DR As DataRow
                Dim ict As Integer = 0
                Dim ipa As Integer = 0
                Dim Testo As String = ""

                XmlDoc.LoadXml(Risultati)

                'Recupero l'elenco dei Formulati
                XMLs_Formulati = XmlDoc.GetElementsByTagName("FORMULATO")

                If Not IsNothing(XMLs_Formulati) Then

                    For i = 0 To XMLs_Formulati.Count - 1

                        'Formulato i-esimo
                        XmlFormulato = XMLs_Formulati.Item(i)

                        'Creo la riga per il datatable
                        DR = DTfor.NewRow

                        DR.Item("Fr_Cod") = CInt(XmlFormulato.GetAttribute("fr_cod"))
                        DR.Item("Fr_Des") = CStr(XmlFormulato.GetAttribute("fr_des"))

                        '---------------------------------------------------
                        'CLASSE TOSSICOLOGICA ==>  cod1§des1|cod2§des2
                        'Recupero l'elenco delle Classi Tossicologiche

                        XMLs_ClassiTossicologiche = XmlFormulato.GetElementsByTagName("CLASSE_TOSSICOLOGICA")
                        Testo = ""
                        If XMLs_ClassiTossicologiche.Count > 0 Then
                            For ict = 0 To XMLs_ClassiTossicologiche.Count - 1
                                XmlClasseTossicologica = XMLs_ClassiTossicologiche.Item(ict)
                                Testo &= CStr(XmlClasseTossicologica.GetAttribute("cltoss_cod")) & "§" & _
                                         CStr(XmlClasseTossicologica.GetAttribute("cltoss_des")) & "|"
                            Next
                        End If
                        If Testo <> "" Then
                            Testo = Left(Testo, Testo.Length - 1)
                        End If
                        DR.Item("Elenco_ClassiTossicologiche") = Testo

                        '---------------------------------------------------
                        'PRINCIPI ATTIVI ==> cod1§des1§titolo1|cod2§des2§titolo2
                        'Recupero l'elenco dei Principi Attivi

                        XMLs_PrincipiAttivi = XmlFormulato.GetElementsByTagName("PRINCIPIO_ATTIVO")

                        Testo = ""
                        If XMLs_PrincipiAttivi.Count > 0 Then
                            For ipa = 0 To XMLs_PrincipiAttivi.Count - 1
                                XmlPrincipioAttivo = XMLs_PrincipiAttivi.Item(ipa)
                                Testo &= CStr(XmlPrincipioAttivo.GetAttribute("pa_cod")) & "§" & _
                                         CStr(XmlPrincipioAttivo.GetAttribute("pa_des")) & "§" & _
                                         CStr(XmlPrincipioAttivo.GetAttribute("titolo")) & "|"
                            Next
                        End If
                        If Testo <> "" Then
                            Testo = Left(Testo, Testo.Length - 1)
                        End If
                        DR.Item("Elenco_PrincipiAttivi") = Testo

                        '---------------------------------------------------
                        DTfor.Rows.Add(DR)

                    Next

                End If 'XMLs_Formulati

            End If 'IsNothing(DT)

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Dim Log As New AgronicaCoreDataProvider.LogProvider
            Log.Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DTfor = Nothing
        End Try

        Return DTfor

    End Function


    '###########################################################################
    Public Shared Function ComposizioneFormulatiClasseTossicologica(ByRef objParametri_Server As AgronicaCoreParametri,
                                                                    ByVal Fr_Cod As Integer,
                                                                    ByVal DtFor As DataTable
                                                                    ) As String

        Const nomeRoutine = "AgroWs.ComposizioneFormulatiClasseTossicologica()"
        Dim messaggioErrore As String = ""

        Dim DR As DataRow()
        Dim ElencoClassiTossicologiche As String = ""
        Dim ClassiTossicologiche As String()

        Dim ElencoPrincipiAttivi As String = ""

        Dim ClToss_Cod As String = ""
        Dim ClToss_Des As String = ""
        Dim Pa_Cod As String = ""
        Dim Pa_Des As String = ""
        Dim Titolo As String = ""

        Dim i As Integer
        Dim Risultato As String = ""

        Try

            If Not IsNothing(DtFor) AndAlso DtFor.Rows.Count > 0 Then

                'Cerco il formulato corrente ...
                DR = DtFor.Select("Fr_Cod = " & Fr_Cod)

                If Not IsNothing(DR) AndAlso DR.Length > 0 Then

                    'CLASSI TOSSICOLOGICHE
                    ElencoClassiTossicologiche = DR(0).Item("Elenco_ClassiTossicologiche")
                    If ElencoClassiTossicologiche <> "" Then

                        Risultato = ""
                        ClassiTossicologiche = ElencoClassiTossicologiche.Split("|")
                        For i = 0 To ClassiTossicologiche.Length - 1
                            Risultato &= ClassiTossicologiche(i).Split("§")(0) & "  "
                        Next
                        Risultato &= ""

                    End If

                End If

            End If

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Dim Log As New AgronicaCoreDataProvider.LogProvider
            Log.Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            DtFor = Nothing
        End Try

        Return Risultato

    End Function

    '################################################################################################################################################
    Public Shared Function ComposizioneFormulatiDescrizioneAggiuntiva(ByRef objParametri_Server As AgronicaCoreParametri, _
                                                                        ByVal Fr_Cod As Integer, _
                                                                        ByVal DtFor As DataTable) As String

        Dim NomeRoutine As String = "SchedeMagazzinoHelper.ComposizioneFormulatiDescrizioneAggiuntiva()"
        Dim MessaggioErrore As String = ""

        Dim DR As DataRow()
        Dim ElencoClassiTossicologiche As String = ""
        Dim ClassiTossicologiche As String()

        Dim ElencoPrincipiAttivi As String = ""
        Dim PrincipiAttivi As String()


        Dim ClToss_Cod As String = ""
        Dim ClToss_Des As String = ""
        Dim Pa_Cod As String = ""
        Dim Pa_Des As String = ""
        Dim Titolo As String = ""

        Dim i As Integer
        Dim Risultato As String = ""

        'Inizializzo
        Risultato = ""

        Try

            If Not IsNothing(DtFor) AndAlso DtFor.Rows.Count > 0 Then

                'Cerco il formulato corrente ...
                DR = DtFor.Select("Fr_Cod = " & Fr_Cod)

                If Not IsNothing(DR) AndAlso DR.Length > 0 Then

                    'CLASSI TOSSICOLOGICHE
                    ElencoClassiTossicologiche = DR(0).Item("Elenco_ClassiTossicologiche")
                    If ElencoClassiTossicologiche <> "" Then

                        Risultato = "   (Classe Toss. =  "
                        ClassiTossicologiche = ElencoClassiTossicologiche.Split("|")
                        For i = 0 To ClassiTossicologiche.Length - 1
                            Risultato &= ClassiTossicologiche(i).Split("§")(0) & "  "
                        Next
                        Risultato &= ")"

                    End If

                    'PRINCIPI ATTIVI
                    ElencoPrincipiAttivi = DR(0).Item("Elenco_PrincipiAttivi")
                    If ElencoPrincipiAttivi <> "" Then
                        Risultato &= vbCrLf
                        PrincipiAttivi = ElencoPrincipiAttivi.Split("|")
                        For i = 0 To PrincipiAttivi.Length - 1

                            Pa_Cod = PrincipiAttivi(i).Split("§")(0)
                            Pa_Des = PrincipiAttivi(i).Split("§")(1)
                            Titolo = PrincipiAttivi(i).Split("§")(2)

                            'Nota Bene : se il titolo e' "0" allora visualizzo solo la descrizione
                            If CDbl(Titolo) <> 0 Then
                                Risultato &= "     " & Format(CDbl(Titolo), "0.####") & " % - " & Pa_Des & vbCrLf
                            Else
                                Risultato &= "     " & "(" & Pa_Des & ")" & vbCrLf
                            End If
                        Next
                    End If
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Dim Log As New AgronicaCoreDataProvider.LogProvider
            Log.Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DtFor = Nothing
        End Try

        Return Risultato

    End Function



    '####################################################################################################################################################
    Public Shared Function ComposizioneFormulatiDescrizionePrincipiAttivi(ByRef objParametri_Server As AgronicaCoreParametri,
                                                                          ByVal Fr_Cod As Integer,
                                                                          ByVal DtFor As DataTable
                                                                          ) As String

        Const nomeRoutine = "AgroWs.ComposizioneFormulatiDescrizionePrincipiAttivi()"
        Dim messaggioErrore As String = ""

        Dim dr As DataRow()
        Dim ElencoClassiTossicologiche As String = ""

        Dim ElencoPrincipiAttivi As String = ""
        Dim PrincipiAttivi As String()

        Dim ClToss_Cod As String = ""
        Dim ClToss_Des As String = ""
        Dim Pa_Cod As String = ""
        Dim Pa_Des As String = ""
        Dim Titolo As String = ""

        Dim i As Integer
        Dim risultato As String = ""

        Try

            If Not IsNothing(DtFor) AndAlso DtFor.Rows.Count > 0 Then

                'Cerco il formulato corrente ...
                dr = DtFor.Select("Fr_Cod = " & Fr_Cod)

                If Not IsNothing(dr) AndAlso dr.Length > 0 Then

                    'PRINCIPI ATTIVI
                    ElencoPrincipiAttivi = dr(0).Item("Elenco_PrincipiAttivi")
                    If ElencoPrincipiAttivi <> "" Then

                        risultato = vbCrLf
                        PrincipiAttivi = ElencoPrincipiAttivi.Split("|")
                        For i = 0 To PrincipiAttivi.Length - 1

                            Pa_Cod = PrincipiAttivi(i).Split("§")(0)
                            Pa_Des = PrincipiAttivi(i).Split("§")(1)
                            Titolo = PrincipiAttivi(i).Split("§")(2)

                            'Nota Bene : se il titolo e' "0" allora visualizzo solo la descrizione
                            If CDbl(Titolo) <> 0 Then
                                risultato &= "     " & Format(CDbl(Titolo), "0.####") & " % - " & Pa_Des & vbCrLf
                            Else
                                risultato &= "     " & "(" & Pa_Des & ")" & vbCrLf
                            End If
                        Next
                    End If

                End If

            End If

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Dim Log As New AgronicaCoreDataProvider.LogProvider
            Log.Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            DtFor = Nothing
        End Try

        Return risultato

    End Function





    '###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulato_UdM(ByVal Operazione As enum_AWS_Xml,
                                                  ByRef StringaXML As String,
                                                  ByRef Fr_Cod As Integer,
                                                  ByRef Udm_Cod As Integer,
                                                  ByRef Udm_Des As String,
                                                  ByRef Udm_Sim As String,
                                                  ByRef Errore As String)

        Dim xmlDoc As New Xml.XmlDocument
        Dim xmlRisultati As Xml.XmlElement
        Dim xmlFormulato As Xml.XmlElement
        Dim xmlNodi As Xml.XmlNodeList

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <FORMULATO fr_cod="..." udm_cod="..."  udm_des="..." udm_sim="..." />
        '   </RISULTATI>

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                xmlRisultati = xmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                xmlRisultati.SetAttribute("errore", "")


                'Creo il nodo FORMULATO
                xmlFormulato = xmlDoc.CreateElement("FORMULATO")

                xmlRisultati.AppendChild(xmlFormulato)

                'Imposto gli attributi
                xmlFormulato.SetAttribute(LCase("Fr_Cod"), Fr_Cod.ToString)
                xmlFormulato.SetAttribute(LCase("Udm_Cod"), Udm_Cod.ToString)
                xmlFormulato.SetAttribute(LCase("Udm_Des"), Udm_Des)
                xmlFormulato.SetAttribute(LCase("Udm_Sim"), Udm_Sim)

                'Imposto XmlRisultati come figlio del documento principale
                xmlDoc.AppendChild(xmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlRisultati = Nothing
                xmlNodi = Nothing
                xmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                ''Carico la stringa XML nel documento
                'XmlDoc.LoadXml(StringaXML)

                ''Prelevo il nodo XmlParametri
                'XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                ''Prelevo gli attributi
                'Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                'Fr_Des = CStr(XmlParametri.GetAttribute("fr_des"))
                'Class_Cod = CStr(XmlParametri.GetAttribute("class_cod"))
                'DataRilievo = CStr(XmlParametri.GetAttribute("datarilievo"))
                'Flag_SoloAttivi = CBool(XmlParametri.GetAttribute("flag_soloattivi"))

                ''Distruggo gli oggetti
                'XmlNodo = Nothing
                'XmlParametri = Nothing
                'XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub

    Public Function SelectDistinct(ByVal SourceTable As DataTable, ByVal FieldName As String) As String()

        'array da ritornare...
        Dim strRet As String()
        Dim lastValue As Object
        Dim dr As DataRow
        'indice x l'array...
        Dim i As Integer = 0

        'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
        For Each dr In SourceTable.Select("", FieldName)

            If Not IsDBNull(lastValue) Then

                'se l'ultimo valori è nothing prendo il valore....(succede solo la prima volta)
                If lastValue Is Nothing Then
                    'assegno l'ultimo valore...
                    lastValue = dr(FieldName)
                    'aggiungo la nuova riga....
                    ReDim Preserve strRet(i)
                    strRet(i) = lastValue.ToString 'assegno il valore del campo....
                    'incremento l'indice...
                    i += 1
                Else

                    If Not ColonneUguali(lastValue, dr(FieldName)) Then 'le volte successive controllo se sono uguali
                        'assegno l'ultimo valore...
                        lastValue = dr(FieldName)
                        'aggiungo la nuova riga....
                        ReDim Preserve strRet(i)
                        strRet(i) = lastValue.ToString 'assegno il valore del campo....
                        'incremento l'indice...
                        i += 1
                    End If

                End If 'fine controllo 

            End If 'fine controllo valore null

        Next

        Return strRet 'ritorno altrimenti solo la tabella

    End Function


    Private Function ColonneUguali(ByVal A As Object, ByVal B As Object) As Boolean

        'confronta 2 valori x vedere se sono uguali, confronta anche il dbnull 
        If IsDBNull(A) AndAlso IsDBNull(B) Then
            Return True 'entrambi db null
        End If

        If IsDBNull(A) OrElse IsDBNull(B) Then
            Return False 'solo uno è db null
        End If

        Return A.Equals(B) 'confronta i 2 oggetti e ritorna un booleano x indicare se sono uguali o meno..

    End Function

#Region "Esterni"

    Public Sub AgroWSEsterni_XML_Parametri_Formulati_Profitosan_Tunnel( _
                         ByRef StringaXML As String, _
                         ByRef Fr_Cod As String, _
                         ByRef Errore As String)

        Dim xmlDoc As New Xml.XmlDocument
        Dim xmlParametri As Xml.XmlElement

        xmlDoc.LoadXml(StringaXML)

        xmlParametri = xmlDoc.SelectSingleNode("//Parametri")

        Fr_Cod = CStr(xmlParametri.SelectSingleNode("FormulatoCodice").InnerXml)

        xmlParametri = Nothing
        xmlDoc = Nothing

    End Sub

    Public Sub AgroWSEsterni_XML__Credenziali( _
                                ByRef StringaXML As String, _
                                ByRef UserName As String, _
                                ByRef Password As String, _
                                ByRef ApplicazioneCodice As String, _
                                ByRef UsernameSessione As String, _
                                ByRef DetentoreFascicoloCodice As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlCredenziali As Xml.XmlElement

        XmlDoc.LoadXml(StringaXML)

        XmlCredenziali = XmlDoc.SelectSingleNode("//Credenziali")

        UserName = CStr(XmlCredenziali.SelectSingleNode("Username").InnerXml)
        Password = CStr(XmlCredenziali.SelectSingleNode("Password").InnerXml)
        ApplicazioneCodice = CStr(XmlCredenziali.SelectSingleNode("ApplicazioneCodice").InnerXml)
        UsernameSessione = CStr(XmlCredenziali.SelectSingleNode("UsernameSessione").InnerXml)
        DetentoreFascicoloCodice = CStr(XmlCredenziali.SelectSingleNode("DetentoreFascicoloCodice").InnerXml)

        XmlCredenziali = Nothing
        XmlDoc = Nothing

    End Sub

    Public Sub AgroWSEsterni_XML_Parametri_Formulati_Carico( _
                             ByRef StringaXML As String, _
                             ByRef FormulatoTestoRicerca As String, _
                             ByRef Data As String, _
                             ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement

        XmlDoc.LoadXml(StringaXML)

        XmlParametri = XmlDoc.SelectSingleNode("//Parametri")

        FormulatoTestoRicerca = CStr(XmlParametri.SelectSingleNode("FormulatoTestoRicerca").InnerXml)
        Data = CStr(XmlParametri.SelectSingleNode("Data").InnerXml)

        XmlParametri = Nothing
        XmlDoc = Nothing

    End Sub

    Public Sub AgroWSEsterni_XML_Parametri_Formulati( _
                         ByRef StringaXML As String, _
                         ByRef SpecieCodice As String, _
                         ByRef DisciplinareCodice As String, _
                         ByRef FormulatoCodice As String, _
                         ByRef FormulatoTestoRicerca As String, _
                         ByRef Data As String, _
                         ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement

        XmlDoc.LoadXml(StringaXML)

        XmlParametri = XmlDoc.SelectSingleNode("//Parametri")

        SpecieCodice = CStr(XmlParametri.SelectSingleNode("SpecieCodice").InnerXml)
        DisciplinareCodice = CStr(XmlParametri.SelectSingleNode("DisciplinareCodice").InnerXml)
        FormulatoCodice = CStr(XmlParametri.SelectSingleNode("FormulatoCodice").InnerXml)
        FormulatoTestoRicerca = CStr(XmlParametri.SelectSingleNode("FormulatoTestoRicerca").InnerXml)
        Data = CStr(XmlParametri.SelectSingleNode("Data").InnerXml)

        XmlParametri = Nothing
        XmlDoc = Nothing

    End Sub


    Public Sub AgroWSEsterni_XML_Parametri_Fertilizzanti( _
                         ByRef StringaXML As String, _
                         ByRef FertilizzanteCodice As String, _
                         ByRef FertilizzanteTestoRicerca As String, _
                         ByRef Data As String, _
                         ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement

        XmlDoc.LoadXml(StringaXML)

        XmlParametri = XmlDoc.SelectSingleNode("//Parametri")

        FertilizzanteCodice = CStr(XmlParametri.SelectSingleNode("FertilizzanteCodice").InnerXml)
        FertilizzanteTestoRicerca = CStr(XmlParametri.SelectSingleNode("FertilizzanteTestoRicerca").InnerXml)
        Data = CStr(XmlParametri.SelectSingleNode("Data").InnerXml)

        XmlParametri = Nothing
        XmlDoc = Nothing

    End Sub

    Public Sub AgroWSEsterni_XML_Parametri_Formulati_Avversita( _
                     ByRef StringaXML As String, _
                     ByRef SpecieCodice As String, _
                     ByRef DisciplinareCodice As String, _
                     ByRef FormulatoCodice As String, _
                     ByRef Data As String, _
                     ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement

        XmlDoc.LoadXml(StringaXML)

        XmlParametri = XmlDoc.SelectSingleNode("//Parametri")

        SpecieCodice = CStr(XmlParametri.SelectSingleNode("SpecieCodice").InnerXml)
        DisciplinareCodice = CStr(XmlParametri.SelectSingleNode("DisciplinareCodice").InnerXml)
        FormulatoCodice = CStr(XmlParametri.SelectSingleNode("FormulatoCodice").InnerXml)
        Data = CStr(XmlParametri.SelectSingleNode("Data").InnerXml)

        XmlParametri = Nothing
        XmlDoc = Nothing

    End Sub

    Public Sub AgroWSEsterni_XML_Parametri_Formulati_Avversita_Dosi( _
                 ByRef StringaXML As String, _
                 ByRef SpecieCodice As String, _
                 ByRef DisciplinareCodice As String, _
                 ByRef FormulatoCodice As String, _
                 ByRef AvversitaCodice As String, _
                 ByRef Data As String, _
                 ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement

        XmlDoc.LoadXml(StringaXML)

        XmlParametri = XmlDoc.SelectSingleNode("//Parametri")

        SpecieCodice = CStr(XmlParametri.SelectSingleNode("SpecieCodice").InnerXml)
        DisciplinareCodice = CStr(XmlParametri.SelectSingleNode("DisciplinareCodice").InnerXml)
        FormulatoCodice = CStr(XmlParametri.SelectSingleNode("FormulatoCodice").InnerXml)
        AvversitaCodice = CStr(XmlParametri.SelectSingleNode("AvversitaCodice").InnerXml)
        Data = CStr(XmlParametri.SelectSingleNode("Data").InnerXml)

        XmlParametri = Nothing
        XmlDoc = Nothing

    End Sub

    Public Sub AgroWSEsterni_XML_Parametri_Disciplinari(ByRef StringaXML As String, _
                                                         ByRef SpecieCodice As String, _
                                                         ByRef RegioneCodice As String, _
                                                         ByRef Data As String, _
                                                         ByRef Errore As String)

        Dim XmlDoc As New Xml.XmlDocument
        Dim XmlParametri As Xml.XmlElement

        XmlDoc.LoadXml(StringaXML)

        XmlParametri = XmlDoc.SelectSingleNode("//Parametri")

        SpecieCodice = CStr(XmlParametri.SelectSingleNode("SpecieCodice").InnerXml)
        RegioneCodice = CStr(XmlParametri.SelectSingleNode("RegioneCodice").InnerXml)
        Data = CStr(XmlParametri.SelectSingleNode("Data").InnerXml)

        XmlParametri = Nothing
        XmlDoc = Nothing

    End Sub

    'utilizzato SOLO nel 2010
#If VBC_VER >= 9 Then

    Public Sub AgroWSEsterni_XML_Risultati_Formulati_Carico(ByRef xDoc As XDocument, _
                                                            ByRef DtRisultati As DataTable)

        '---------------------------------------------------------------------
        '--- Costruisco la stringa dei risultati
        '---------------------------------------------------------------------

        Dim NodoRisultati As Xml.Linq.XElement

        Dim HashFormulati As New Hashtable
        Dim Fr_Cod As String
        Dim Fr_Des As String
        Dim DataRegistrazione As String
        Dim DataTermine As String
        Dim DataRevoca As String
        Dim DataFineCommercializzazione As String
        Dim DataFineUsoScorte As String

        Dim DataSospensioneDA As String
        Dim DataSospensioneA As String

        Dim HashSostanze As New Hashtable
        Dim HashClassificazioni As New Hashtable
        Dim HashSospensioni As New Hashtable

        Dim Pa_Cod As String
        Dim Pa_Des As String
        Dim Class_Cod As String
        Dim Class_Des As String

        For Each row As DataRow In DtRisultati.Rows

            Fr_Cod = row("Fr_Cod")
            Fr_Des = row("Fr_Des")

            DataRegistrazione = ""
            DataTermine = ""
            DataRevoca = ""
            DataFineCommercializzazione = ""
            DataFineUsoScorte = ""

            If Not IsDBNull(row("data_reg")) Then
                DataRegistrazione = CDate(row("data_reg")).ToShortDateString
            End If
            If Not IsDBNull(row("Data_Term")) Then
                DataTermine = CDate(row("Data_Term")).ToShortDateString
            End If
            If Not IsDBNull(row("Data_Revo")) Then
                DataRevoca = CDate(row("Data_Revo")).ToShortDateString
            End If
            If Not IsDBNull(row("Data_Fine_Comm")) Then
                DataFineCommercializzazione = CDate(row("Data_Fine_Comm")).ToShortDateString
            End If
            If Not IsDBNull(row("Data_Fine_UsoScorte")) Then
                DataFineUsoScorte = CDate(row("Data_Fine_UsoScorte")).ToShortDateString
            End If

            'controllo duplicati
            If Not HashFormulati.ContainsKey(Fr_Cod) Then

                HashFormulati.Add(Fr_Cod, "")

                NodoRisultati = _
                <Formulato>
                    <Codice><%= Fr_Cod %></Codice>
                    <Descrizione><%= Fr_Des %></Descrizione>
                    <DataRegistrazione><%= DataRegistrazione %></DataRegistrazione>
                    <DataTermine><%= DataTermine %></DataTermine>
                    <DataRevoca><%= DataRevoca %></DataRevoca>
                    <DataFineCommercializzazione><%= DataFineCommercializzazione %></DataFineCommercializzazione>
                    <DataFineUsoScorte><%= DataFineUsoScorte %></DataFineUsoScorte>
                    <PeriodiSospensione>
                    </PeriodiSospensione>
                    <Classificazioni>
                    </Classificazioni>
                    <SostanzeAttive>
                    </SostanzeAttive>
                </Formulato>

                'estraggo singole sostanze e classificazioni
                Dim DrFrCod As DataRow() = DtRisultati.Select("Fr_Cod=" & Fr_Cod)

                HashSostanze.Clear()
                HashClassificazioni.Clear()
                HashSospensioni.Clear()

                For Each rowF As DataRow In DrFrCod

                    Pa_Cod = rowF("pa_cod")
                    Pa_Des = rowF("pa_des")
                    Class_Cod = rowF("Class_Cod")
                    Class_Des = rowF("Class_des")

                    If Not HashSostanze.ContainsKey(Pa_Cod) Then
                        HashSostanze.Add(Pa_Cod, Pa_Des)
                    End If
                    If Not HashClassificazioni.ContainsKey(Class_Cod) Then
                        HashClassificazioni.Add(Class_Cod, Class_Des)
                    End If

                    DataSospensioneDA = ""
                    DataSospensioneA = ""

                    If Not IsDBNull(row("DataSospensioneDA")) Then
                        DataSospensioneDA = CDate(row("DataSospensioneDA")).ToShortDateString
                    End If
                    If Not IsDBNull(row("DataSospensioneA")) Then
                        DataSospensioneA = CDate(row("DataSospensioneA")).ToShortDateString
                    End If

                    If DataSospensioneDA <> "" OrElse DataSospensioneA <> "" Then
                        If Not HashSospensioni.ContainsKey(DataSospensioneDA & "_" & DataSospensioneA) Then
                            HashSospensioni.Add(DataSospensioneDA & "_" & DataSospensioneA, "")
                        End If
                    End If

                Next


                For Each eSa As DictionaryEntry In HashSostanze
                    Dim Sa = <SostanzeAttiva><%= eSa.Value %></SostanzeAttiva>
                    NodoRisultati.Element("SostanzeAttive").Add(Sa)
                Next

                For Each eCl As DictionaryEntry In HashClassificazioni
                    Dim Cl = <Classificazione><%= eCl.Value %></Classificazione>
                    NodoRisultati.Element("Classificazioni").Add(Cl)
                Next

                For Each eSo As DictionaryEntry In HashSospensioni
                    Dim So = <Sospensione><SospenzioneDa><%= Split(eSo.Key, "_")(0) %></SospenzioneDa><SospenzioneA><%= Split(eSo.Key, "_")(1) %></SospenzioneA></Sospensione>
                    NodoRisultati.Element("PeriodiSospensione").Add(So)
                Next

                xDoc.Element("Output").Element("Risultati").Add(NodoRisultati)

            End If

        Next row

    End Sub

#End If

    'utilizzzato SOLO nel 2010
#If VBC_VER >= 9 Then

    Public Sub AgroWSEsterni_XML_Risultati_Fertilizzanti(ByRef xDoc As XDocument, _
                                                     ByRef DtRisultati As DataTable)

        '---------------------------------------------------------------------
        '--- Costruisco la stringa dei risultati
        '---------------------------------------------------------------------

        Dim NodoRisultati As Xml.Linq.XElement


        Dim Fer_Cod As String
        Dim Fer_Des As String
        Dim N As String
        Dim P As String
        Dim K As String

        Dim Class_FER_Cod As Integer
        Dim Class_FER_Des As String


        Dim HashFertilizzanti As New Hashtable
        Dim HashClassificazioni As New Hashtable


        For Each row As DataRow In DtRisultati.Rows

            Fer_Cod = row("Fer_Cod")
            Fer_Des = row("Fer_Des")

            If Not IsDBNull(row("N")) Then
                N = CStr(row("N"))
            End If

            If Not IsDBNull(row("P2O5")) Then
                P = CStr(row("P2O5"))
            End If

            If Not IsDBNull(row("K2O")) Then
                K = CStr(row("K2O"))
            End If



            'controllo duplicati
            If Not HashFertilizzanti.ContainsKey(Fer_Cod) Then

                HashFertilizzanti.Add(Fer_Cod, "")

                NodoRisultati = _
                <Fertilizzante>
                    <Codice><%= Fer_Cod %></Codice>
                    <Descrizione><%= Fer_Des %></Descrizione>
                    <N><%= N %></N>
                    <P><%= P %></P>
                    <K><%= K %></K>
                    <Classificazioni>
                    </Classificazioni>
                </Fertilizzante>

                'estraggo singole sostanze e classificazioni
                Dim DrFrCod As DataRow() = DtRisultati.Select("Fer_Cod=" & Fer_Cod)

                HashClassificazioni.Clear()

                For Each rowF As DataRow In DrFrCod

                    Class_FER_Cod = rowF("Class_FER_Cod")
                    Class_FER_Des = rowF("Class_FER_Des")

                    If Not HashClassificazioni.ContainsKey(Class_FER_Cod) Then
                        HashClassificazioni.Add(Class_FER_Cod, Class_FER_Des)
                    End If


                Next


                For Each eCl As DictionaryEntry In HashClassificazioni
                    Dim Cl = <Classificazione><%= eCl.Value %></Classificazione>
                    NodoRisultati.Element("Classificazioni").Add(Cl)
                Next


                xDoc.Element("Output").Element("Risultati").Add(NodoRisultati)

            End If

        Next row

    End Sub

#End If


    'utilizzato SOLO nel 2010
#If VBC_VER >= 9 Then

    Public Sub AgroWSEsterni_XML_Risultati_Formulati(ByRef xDoc As XDocument, _
                                                     ByRef DtRisultati As DataTable)

        '---------------------------------------------------------------------
        '--- Costruisco la stringa dei risultati
        '---------------------------------------------------------------------

        Dim NodoRisultati As Xml.Linq.XElement

        Dim HashFormulati As New Hashtable
        Dim Fr_Cod As String
        Dim Fr_Des As String
        Dim DataRegistrazione As String
        Dim DataTermine As String
        Dim DataRevoca As String
        Dim DataFineCommercializzazione As String
        Dim DataFineUsoScorte As String

        Dim DataSospensioneDA As String
        Dim DataSospensioneA As String

        Dim Carenza As String

        Dim HashSostanze As New Hashtable
        Dim HashClassificazioni As New Hashtable
        Dim HashSospensioni As New Hashtable

        Dim Pa_Cod As String
        Dim Pa_Des As String
        Dim Class_Cod As String
        Dim Class_Des As String

        For Each row As DataRow In DtRisultati.Rows

            Fr_Cod = row("Fr_Cod")
            Fr_Des = row("Fr_Des")

            DataRegistrazione = ""
            DataTermine = ""
            DataRevoca = ""
            DataFineCommercializzazione = ""
            DataFineUsoScorte = ""
            Carenza = ""

            If Not IsDBNull(row("data_reg")) Then
                DataRegistrazione = CDate(row("data_reg")).ToShortDateString
            End If
            If Not IsDBNull(row("Data_Term")) Then
                DataTermine = CDate(row("Data_Term")).ToShortDateString
            End If
            If Not IsDBNull(row("Data_Revo")) Then
                DataRevoca = CDate(row("Data_Revo")).ToShortDateString
            End If
            If Not IsDBNull(row("Data_Fine_Comm")) Then
                DataFineCommercializzazione = CDate(row("Data_Fine_Comm")).ToShortDateString
            End If
            If Not IsDBNull(row("Data_Fine_UsoScorte")) Then
                DataFineUsoScorte = CDate(row("Data_Fine_UsoScorte")).ToShortDateString
            End If
            If Not IsDBNull(row("TempoCarenza")) Then
                Carenza = CInt(row("TempoCarenza"))
            End If

            'controllo duplicati
            If Not HashFormulati.ContainsKey(Fr_Cod) Then

                HashFormulati.Add(Fr_Cod, "")

                NodoRisultati = _
                <Formulato>
                    <Codice><%= Fr_Cod %></Codice>
                    <Descrizione><%= Fr_Des %></Descrizione>
                    <DataRegistrazione><%= DataRegistrazione %></DataRegistrazione>
                    <DataTermine><%= DataTermine %></DataTermine>
                    <DataRevoca><%= DataRevoca %></DataRevoca>
                    <DataFineCommercializzazione><%= DataFineCommercializzazione %></DataFineCommercializzazione>
                    <DataFineUsoScorte><%= DataFineUsoScorte %></DataFineUsoScorte>
                    <PeriodiSospensione>
                    </PeriodiSospensione>
                    <Carenza><%= Carenza %></Carenza>
                    <Classificazioni>
                    </Classificazioni>
                    <SostanzeAttive>
                    </SostanzeAttive>
                </Formulato>

                'estraggo singole sostanze e classificazioni
                Dim DrFrCod As DataRow() = DtRisultati.Select("Fr_Cod=" & Fr_Cod)

                HashSostanze.Clear()
                HashClassificazioni.Clear()
                HashSospensioni.Clear()

                For Each rowF As DataRow In DrFrCod

                    Pa_Cod = rowF("pa_cod")
                    Pa_Des = rowF("pa_des")
                    Class_Cod = rowF("Class_Cod")
                    Class_Des = rowF("Class_des")

                    If Not HashSostanze.ContainsKey(Pa_Cod) Then
                        HashSostanze.Add(Pa_Cod, Pa_Des)
                    End If
                    If Not HashClassificazioni.ContainsKey(Class_Cod) Then
                        HashClassificazioni.Add(Class_Cod, Class_Des)
                    End If

                    DataSospensioneDA = ""
                    DataSospensioneA = ""

                    If Not IsDBNull(row("DataSospensioneDA")) Then
                        DataSospensioneDA = CDate(row("DataSospensioneDA")).ToShortDateString
                    End If
                    If Not IsDBNull(row("DataSospensioneA")) Then
                        DataSospensioneA = CDate(row("DataSospensioneA")).ToShortDateString
                    End If

                    If DataSospensioneDA <> "" OrElse DataSospensioneA <> "" Then
                        If Not HashSospensioni.ContainsKey(DataSospensioneDA & "_" & DataSospensioneA) Then
                            HashSospensioni.Add(DataSospensioneDA & "_" & DataSospensioneA, "")
                        End If
                    End If

                Next


                For Each eSa As DictionaryEntry In HashSostanze
                    Dim Sa = <SostanzeAttiva><%= eSa.Value %></SostanzeAttiva>
                    NodoRisultati.Element("SostanzeAttive").Add(Sa)
                Next

                For Each eCl As DictionaryEntry In HashClassificazioni
                    Dim Cl = <Classificazione><%= eCl.Value %></Classificazione>
                    NodoRisultati.Element("Classificazioni").Add(Cl)
                Next

                For Each eSo As DictionaryEntry In HashSospensioni
                    Dim So = <Sospensione><SospenzioneDa><%= Split(eSo.Key, "_")(0) %></SospenzioneDa><SospenzioneA><%= Split(eSo.Key, "_")(1) %></SospenzioneA></Sospensione>
                    NodoRisultati.Element("PeriodiSospensione").Add(So)
                Next

                xDoc.Element("Output").Element("Risultati").Add(NodoRisultati)

            End If

        Next row

    End Sub

#End If

    'utilizzzato SOLO nel 2010
#If VBC_VER >= 9 Then
    Public Sub AgroWSEsterni_XML_Risultati_Formulati_Avversita(ByRef xDoc As XDocument, _
                                                 ByRef DtRisultati As DataTable)

        '---------------------------------------------------------------------
        '--- Costruisco la stringa dei risultati
        '---------------------------------------------------------------------

        Dim NodoRisultati As Xml.Linq.XElement

        Dim HashCod As New Hashtable

        Dim For_Veg_Av_Cod As Integer
        Dim str_For_Veg_Av_Cod As String
        Dim AvCod As Integer
        Dim AvGru As Integer
        Dim AvDes As String
        Dim AvGruDes As String

        For Each row As DataRow In DtRisultati.Rows

            For_Veg_Av_Cod = row("For_Veg_Av_Cod")
            AvCod = row("Av_Cod")
            AvGru = row("Av_Gru")

            AvDes = ""
            AvGruDes = ""

            If Not IsDBNull(row("Av_Des_Vol")) Then
                AvDes = CStr(row("Av_Des_Vol"))
            End If
            If Not IsDBNull(row("Av_Gru_Des")) Then
                AvGruDes = CStr(row("Av_Gru_Des"))
            End If

            VBMath.Randomize()

            Dim value As Integer = CInt(Int((98 * Rnd()) + 1))


            If Not HashCod.ContainsKey(For_Veg_Av_Cod) Then

                HashCod.Add(For_Veg_Av_Cod, "")

                'primi 2 numeri + 2 numeri casuali + resto dei numeri
                str_For_Veg_Av_Cod = Left(For_Veg_Av_Cod.ToString, 2) + Right(("00" + value.ToString), 2) + Mid(For_Veg_Av_Cod.ToString, 3)

                Select Case AvCod

                    Case 0
                        NodoRisultati = _
                        <Avversita>
                            <Codice><%= str_For_Veg_Av_Cod %></Codice>
                            <Descrizione><%= AvGruDes %></Descrizione>
                        </Avversita>

                    Case Else
                        NodoRisultati = _
                            <Avversita>
                                <Codice><%= str_For_Veg_Av_Cod %></Codice>
                                <Descrizione><%= AvDes %></Descrizione>
                            </Avversita>
                End Select

                xDoc.Element("Output").Element("Risultati").Add(NodoRisultati)

            End If

        Next row

    End Sub

#End If

    'utilizzato SOLO nel 2010
#If VBC_VER >= 9 Then
    Public Sub AgroWSEsterni_XML_Risultati_Formulati_Avversita_Dosi(ByRef xDoc As XDocument, _
                                             ByRef DtRisultati As DataTable)

        '---------------------------------------------------------------------
        '--- Costruisco la stringa dei risultati
        '---------------------------------------------------------------------

        Dim NodoRisultati As Xml.Linq.XElement

        Dim HashCod As New Hashtable

        Dim For_Veg_Av_Dos_Cod As Integer

        Dim DoseMin As String
        Dim DoseMax As String
        Dim DoseUdmDes As String

        Dim DoseAcquaMin As String
        Dim DoseAcquaMax As String
        Dim DoseAcquaUdmDes As String

        Dim EpocaDaDes As String
        Dim EpocaADes As String

        Dim r_dose As Integer = 1

        For Each row As DataRow In DtRisultati.Rows

            For_Veg_Av_Dos_Cod = row("For_Veg_Av_Dos_Cod")

            DoseMin = ""
            DoseMax = ""
            DoseUdmDes = ""
            DoseAcquaMin = ""
            DoseAcquaMax = ""
            DoseAcquaUdmDes = ""
            EpocaDaDes = ""
            EpocaADes = ""

            If Not IsDBNull(row("Dose_Min")) Then
                DoseMin = CStr(row("Dose_Min"))
            End If
            If Not IsDBNull(row("Dose_Max")) Then
                DoseMax = CStr(row("Dose_Max"))
            End If
            If Not IsDBNull(row("UDM_SIM")) Then
                DoseUdmDes = CStr(row("UDM_SIM"))
            End If
            If Not IsDBNull(row("Acqua_min")) Then
                DoseAcquaMin = CStr(row("Acqua_min"))
            End If
            If Not IsDBNull(row("Acqua_max")) Then
                DoseAcquaMax = CStr(row("Acqua_max"))
            End If
            If Not IsDBNull(row("Acqua_UDM_SIM")) Then
                DoseAcquaUdmDes = CStr(row("Acqua_UDM_SIM"))
            End If
            If Not IsDBNull(row("DA_Epoca_Des")) Then
                EpocaDaDes = CStr(row("DA_Epoca_Des"))
            End If
            If Not IsDBNull(row("A_Epoca_Des")) Then
                EpocaADes = CStr(row("A_Epoca_Des"))
            End If

            If Not HashCod.ContainsKey(For_Veg_Av_Dos_Cod) Then

                HashCod.Add(For_Veg_Av_Dos_Cod, "")

                NodoRisultati = _
                        <Dose>
                            <Codice><%= r_dose %></Codice>
                            <DoseMinima><%= DoseMin %></DoseMinima>
                            <DoseMassima><%= DoseMax %></DoseMassima>
                            <DoseUnitaMisura><%= DoseUdmDes %></DoseUnitaMisura>
                            <DoseAcquaMinima><%= DoseAcquaMin %></DoseAcquaMinima>
                            <DoseAcquaMassima><%= DoseAcquaMax %></DoseAcquaMassima>
                            <DoseAcquaUnitaMisura><%= DoseAcquaUdmDes %></DoseAcquaUnitaMisura>
                            <EpocaDa><%= EpocaDaDes %></EpocaDa>
                            <EpocaA><%= EpocaADes %></EpocaA>
                        </Dose>

                r_dose += 1

                xDoc.Element("Output").Element("Risultati").Add(NodoRisultati)

            End If

        Next row

    End Sub

#End If

    'utilizzzato SOLO nel 2010
#If VBC_VER >= 9 Then
    Public Sub AgroWSEsterni_XML_Risultati_Disciplinari(ByRef xDoc As XDocument, _
                                                        ByRef DtRisultati As DataTable)

        '---------------------------------------------------------------------
        '--- Costruisco la stringa dei risultati
        '---------------------------------------------------------------------

        Dim NodoRisultati As Xml.Linq.XElement

        Dim HashDpi As New Hashtable
        Dim Dpi_Cod As String
        Dim Dpi_Des As String
        Dim DataInizio As String
        Dim DataFine As String

        For Each row As DataRow In DtRisultati.Rows

            Dpi_Cod = row("COD_REGOLAMENTO")
            Dpi_Des = row("NomeEsteso")

            DataInizio = ""
            DataFine = ""

            If Not IsDBNull(row("ValidoDal")) Then
                DataInizio = CDate(row("ValidoDal")).ToShortDateString
            End If
            If Not IsDBNull(row("ValidoAl")) Then
                DataFine = CDate(row("ValidoAl")).ToShortDateString
            End If

            'controllo duplicati
            If Not HashDpi.ContainsKey(Dpi_Cod) Then

                HashDpi.Add(Dpi_Cod, "")

                NodoRisultati = _
                <Disciplinare>
                    <Codice><%= Dpi_Cod %></Codice>
                    <Descrizione><%= Dpi_Des %></Descrizione>
                    <DataInizio><%= DataInizio %></DataInizio>
                    <DataFine><%= DataFine %></DataFine>
                </Disciplinare>

                xDoc.Element("Output").Element("Risultati").Add(NodoRisultati)

            End If

        Next row

    End Sub

#End If

    Private Shared Function GetAccessoValido(ByVal n_Accessi_super_User As Integer, ByVal n_Accessi_utente As Integer, ByVal n_max_Accessi_utente As Integer, ByVal ultimo_Creato As DateTime, ByVal n_max_Accessi_super_User As Integer, ByVal AWS_log_DistanzaChiamate As Integer, ByVal AWS_log_DistanzaChiamate_UDM As Integer, ByRef MessaggioErrore As String) As Boolean
        Dim AccessoValido As Boolean
        AccessoValido = True

        'numero accessi giornalieri superato per super user
        If n_Accessi_super_User > n_max_Accessi_super_User Then
            MessaggioErrore &= "numero accessi giornalieri superato per super user."
            AccessoValido = False
        End If

        'numero accessi giornalieri superato per user
        If n_Accessi_utente > n_max_Accessi_utente Then
            MessaggioErrore &= "numero accessi giornalieri superato per user."
            AccessoValido = False
        End If

        'troppe chiamate in un determinato intervallo di tempo
        Dim lAdesso As Date = Now

#If VBC_VER >= 9 Then
        If DateDiff(DirectCast(AWS_log_DistanzaChiamate_UDM, DateInterval), ultimo_Creato, lAdesso) < AWS_log_DistanzaChiamate Then
#Else
        If datediff( DateInterval.Second, ultimo_Creato, lAdesso) < AWS_log_DistanzaChiamate Then
#End If


            MessaggioErrore &= "Troppe chiamate in un determinato intervallo di tempo."
            AccessoValido = False
        End If
        Return AccessoValido
    End Function

    Public Function AgroWSEsterni_Verifica_Abusi( _
                        ByVal Super_user_Username As String, _
                        ByVal Username As String, _
                        ByVal Id_Servizio As Integer, _
                        ByVal Id_Attivita As Integer, _
                        ByRef MessaggioErrore As String, _
                        ByRef objParametri_Utenti As AgronicaCoreParametri) _
                        As Boolean

        Dim accessoValido As Boolean = False

        Dim objAWS_log As New AgronicaCoreUtentiDAL.AWS_log_R


        Dim objCFG As New AgronicaCoreUtentiDAL.Utenti_Validita_Token_R

        'lettura CFG

        Dim n_Accessi_super_User As Integer
        Dim n_Accessi_utente As Integer
        Dim ultimo_Creato As DateTime

        Dim n_max_Accessi_super_User As Integer
        Dim n_max_Accessi_utente As Integer
        Dim n_max_Accessi_udm As Integer

        Dim AWS_log_DistanzaChiamate As Integer
        Dim AWS_log_DistanzaChiamate_UDM As Integer


        Dim dtCFG As DataTable = _
            objCFG.Leggi_Impostazioni_Abusi_AWS( _
                Super_user_Username, _
                "", _
                objParametri_Utenti _
            )

        If dtCFG.Rows.Count > 0 Then
            n_Accessi_super_User = objAWS_log.Num_Accessi_SuperUser(PredisponiFiltroDataOggi("Data_Creazione"), "", objParametri_Utenti)
            n_Accessi_utente = objAWS_log.Num_Accessi_Utente(PredisponiFiltroDataOggi("Data_Creazione"), "", objParametri_Utenti)

            ultimo_Creato = objAWS_log.UltimoLoggato("", "", objParametri_Utenti)

            n_max_Accessi_super_User = dtCFG.Rows(0)("n_max_Accessi_super_User")
            n_max_Accessi_utente = dtCFG.Rows(0)("n_max_Accessi_utente")
            n_max_Accessi_udm = dtCFG.Rows(0)("n_max_Accessi_udm")
            AWS_log_DistanzaChiamate = dtCFG.Rows(0)("AWS_log_DistanzaChiamate")
            AWS_log_DistanzaChiamate_UDM = dtCFG.Rows(0)("AWS_log_DistanzaChiamate_UDM")

            accessoValido = GetAccessoValido(n_Accessi_super_User, n_Accessi_utente, n_max_Accessi_utente, ultimo_Creato, n_max_Accessi_super_User, AWS_log_DistanzaChiamate, AWS_log_DistanzaChiamate_UDM, MessaggioErrore)
        Else
            MessaggioErrore = "Configurazione non Trovata per il super user: " & Super_user_Username
        End If

        Return accessoValido


    End Function

    Private Function PredisponiFiltroDataOggi(ByVal campo As String) As String
        Dim dataOggi As Date = Now.Date

        Return campo & _
            " between " & _
            UtilityProvider.Agro_SQL_SaveDateTime(dataOggi) & " and " & _
            UtilityProvider.Agro_SQL_SaveDateTime( _
                DateAdd( _
                    DateInterval.Second, 1, _
                    DateAdd(DateInterval.Day, 1, dataOggi) _
                ) _
        )

    End Function

    Public Function AgroWSEsterni_Verifica_Credenziali_BancheDati( _
                                ByVal Username As String, _
                                ByVal Password As String, _
                                ByVal Id_Servizio As Integer, _
                                ByVal Id_Attivita As Integer, _
                                ByRef MessaggioErrore As String, _
                                ByRef objParametri_Utenti As AgronicaCoreParametri) _
                                As Boolean

        Dim accessoValido As Boolean

        '----------------------------------------------------------------------------------
        '--- Inizializzo
        '----------------------------------------------------------------------------------

        accessoValido = False

        '----------------------------------------------------------------------------------
        '--- Verifico l'esistenza della coppia Username-Password nel database
        '----------------------------------------------------------------------------------

        If (Username = "") OrElse (Password = "") Then
            accessoValido = False
            MessaggioErrore = "Credenziali insufficienti"
            Return accessoValido
        End If

        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        accessoValido = objUtenti.Controlla_Permessi_Utente(Username, Id_Servizio, Id_Attivita, 0, Now, "", objParametri_Utenti)

        If Not accessoValido Then
            MessaggioErrore = "Credenziali non accettate"
        End If

        '----------------------------------------------------------------------------------
        '--- Restituisco l'esito in uscita
        '----------------------------------------------------------------------------------

        Return accessoValido

    End Function

#End Region


End Class

