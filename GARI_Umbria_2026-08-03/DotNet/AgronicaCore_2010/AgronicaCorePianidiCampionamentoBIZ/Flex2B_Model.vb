
''' <summary>
''' parametri in ingresso
''' </summary>
Public Class InvioFlex2B_IN
    Public Property ID_PDC_Testata As Integer
    Public Property ID_PDC_Dettagli As Integer
End Class


Public Class Flex2B_Model_CFG

    Public Property url As String
    Public Property username As String
    Public Property password As String ' password in chiaro
    Public Property passwordHash As String ' password già in formato SHA-256
    Public Property serializationCultureInfo As String

End Class

Public Class Flex2B_Model_SessionInit_in
    Inherits Flex2B_Model_api_in
    Public Property userid As String
    Public Property password As String

End Class

Public Class Flex2B_Model_api_out
    Public Property errcodereturn As Integer ' 425
    Public Property errordescriptionreturn As String ' Market Access does not have value of BLOCK or PASS
    Public Property passed As Boolean ' false

End Class

Public Class Flex2B_Model_api_in

End Class
Public Class Flex2B_Model_SessionInit_out
    Inherits Flex2B_Model_api_out

    Public Property userid As String
    Public Property password As String
    Public Property sessionuuid As String

End Class

Public Class Flex2B_Model_Agronica_Out
    Inherits Flex2B_Model_api_out

End Class

Public Class Flex2B_Model_Agronica_in
    Inherits Flex2B_Model_api_in

    ''' <summary>
    ''' samplenr - 
    ''' </summary>
    ''' <remarks>Esempio: 12351</remarks>
    Public Property samplenr As String

    ''' <summary>
    ''' facility - 
    ''' </summary>
    ''' <remarks>Esempio:  EDENFRUIT</remarks>
    Public Property facility As String
    
    ''' <summary>
    ''' kpin - Zespri's Kpin
    ''' </summary>
    ''' <remarks>Esempio:  ABC</remarks>
    Public Property kpin As String
    
    ''' <summary>
    ''' block - Zespri's Block Name
    ''' </summary>
    ''' <remarks>Esempio: 1</remarks>
    Public Property block As String
    
    ''' <summary>
    ''' variety - 
    ''' </summary>
    ''' <remarks>Esempio:  HW</remarks>
    Public Property variety As String
    
    ''' <summary>
    ''' grower - 
    ''' </summary>
    ''' <remarks>Esempio:  Agri-company</remarks>
    Public Property grower As String

    ''' <summary>
    ''' grower - 
    ''' </summary>
    ''' <remarks>Esempio:  Orchardroad 1, Parimaribo</remarks>
    Public Property blockdescription As String

    ''' <summary>
    ''' au - Australia
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property au As String

    ''' <summary>
    ''' br - Brasile
    ''' </summary>
    ''' <remarks>Esempio:  BLOCK</remarks>
    Public Property br As String

    ''' <summary>
    ''' ca - Canada
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property ca As String

    ''' <summary>
    ''' cn - Cina
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property cn As String

    ''' <summary>
    ''' gc - Hong Kong
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property gc As String

    ''' <summary>
    ''' hk - Indonesia
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property hk As String

    ''' <summary>
    ''' id - India
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property id As String

    ''' <summary>
    ''' in - Messico
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property [in] As String

    ''' <summary>
    ''' mx - Malesia
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property mx As String

    ''' <summary>
    ''' my - Arabia Saudita
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property my As String

    ''' <summary>
    ''' sa - 
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property sa As String

    ''' <summary>
    ''' sg - Singapore
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property sg As String

    ''' <summary>
    ''' th - Thailandia
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property th As String

    ''' <summary>
    ''' tw - Repubblica di Cina
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property tw As String

    ''' <summary>
    ''' us - Stati Uniti d'America
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property us As String

    ''' <summary>
    ''' vn - Vietnam
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property vn As String

    ''' <summary>
    ''' z3 - 
    ''' </summary>
    ''' <remarks>Esempio:  PASS</remarks>
    Public Property z3 As String

    ''' <summary>
    ''' za - Sudafrica
    ''' </summary>
    ''' <remarks>Esempio:  BLOCK</remarks>
    Public Property za As String

    ''' <summary>
    ''' zr - EU Customer MRL (33%)
    ''' </summary>
    ''' <remarks>Esempio:  BLOCK</remarks>
    Public Property zr As String

    ''' <summary>
    ''' decision - 
    ''' </summary>
    ''' <remarks>Esempio:  OK</remarks>
    Public Property decision As String

    ''' <summary>
    ''' ggn - Global Gap Number
    ''' </summary>
    ''' <remarks>Esempio: 404928642202</remarks>
    Public Property ggn As String

    ''' <summary>
    ''' ton - resa prevista in tonnellate ad ettaro, letta da
    ''' </summary>
    ''' <remarks>Esempio: 250,25</remarks>
    Public Property ton As String

    ''' <summary>
    ''' countryoforigin - ISO 2 code (ie IT, FR, GR, PT,....)
    ''' </summary>
    ''' <remarks>Esempio: GR</remarks>
    Public Property countryoforigin As String

    ''' <summary>
    ''' growingmethod - CK (Conventional), OB (organic)
    ''' </summary>
    ''' <remarks>Esempio: OB</remarks>
    Public Property growingmethod As String

End Class
