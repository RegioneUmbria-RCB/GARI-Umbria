Public Class AI

    Private _AI As String
    Private _Description As String
    Private _Fnc1 As String
    Private _format As String
    Private _DataLenghtNoAI As String
    Public Property DataLenghtNoAI As String
        Get
            Return _DataLenghtNoAI
        End Get
        Set(ByVal value As String)
            _DataLenghtNoAI = value
        End Set
    End Property
    Public Property Format As String
        Get
            Return _format
        End Get
        Set(ByVal value As String)
            _format = value
        End Set
    End Property
    Public Property Description As String
        Get
            Return _Description
        End Get
        Set(ByVal value As String)
            _Description = value
        End Set
    End Property
    Public Property Fnc1 As String
        Get
            Return _Fnc1
        End Get
        Set(ByVal value As String)
            _Fnc1 = value
        End Set
    End Property
    Public Property AI As String
        Get
            Return _AI
        End Get
        Set(ByVal value As String)
            _AI = value
        End Set
    End Property



    Public Sub New(ByVal withList As Boolean)

        If withList Then
            _ListaAI = New List(Of AI)

            _ListaAI.Add(New AI(False) With {._AI = "00", .Description = "Serial Shipping Container Code (SSCC)", .Format = "N2+N18", .DataLenghtNoAI = "18", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "01", .Description = "Global Trade Item Number (GTIN)", .Format = "N2+N14", .DataLenghtNoAI = "14", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "02", .Description = "GTIN of Contained Trade Items", .Format = "N2+N14", .DataLenghtNoAI = "14", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "10", .Description = "Batch/Lot Number", .Format = "N2+X..20", .DataLenghtNoAI = "variable, up to 20", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "11", .Description = "Production Date", .Format = "N2+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "12", .Description = "Due Date", .Format = "N2+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "13", .Description = "Packaging Date", .Format = "N2+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "15", .Description = "Sell by Date (Quality Control)", .Format = "N2+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "17", .Description = "Expiration Date", .Format = "N2+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "20", .Description = "Product Variant", .Format = "N2+N2", .DataLenghtNoAI = "2", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "21", .Description = "Serial Number", .Format = "N2+X..20", .DataLenghtNoAI = "variable, up to 20", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "22", .Description = "Secondary Data Fields", .Format = "N2+X..29", .DataLenghtNoAI = "variable, up to 29", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "240", .Description = "Additional Product Identification", .Format = "N3+X..30", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "241", .Description = "Customer Part Number", .Format = "N3+X..30", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "242", .Description = "Made-to-Order Variation Number", .Format = "N3+N..6", .DataLenghtNoAI = "variable, up to 6", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "250", .Description = "Secondary Serial Number", .Format = "N3+X..30", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "251", .Description = "Reference to Source Entity", .Format = "N3+X..30", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "253", .Description = "Global Document Type Identifier", .Format = "N3+N13+X..17", .DataLenghtNoAI = "variable, 13-17", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "254", .Description = "GLN Extension Component", .Format = "N3+X..20", .DataLenghtNoAI = "variable, up to 20", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "30", .Description = "Count of items", .Format = "N2+N..8", .DataLenghtNoAI = "variable, up to 8", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "310y", .Description = "Product Net Weight in kg", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "311y", .Description = "Product Length/1st Dimension, in meters", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "312y", .Description = "Product Width/Diameter/2nd Dimension, in meters", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "313y", .Description = "Product Depth/Thickness/Height/3rd Dimension, in meters", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "314y", .Description = "Product Area, in square meters", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "315y", .Description = "Product Net Volume, in liters", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "316y", .Description = "Product Net Volume, in cubic meters", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "320y", .Description = "Product Net Weight, in pounds", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "321y", .Description = "Product Length/1st Dimension, in inches", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "322y", .Description = "Product Length/1st Dimension, in feet", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "323y", .Description = "Product Length/1st Dimension, in yards", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "324y", .Description = "Product Width/Diameter/2nd Dimension, in inches", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "325y", .Description = "Product Width/Diameter/2nd Dimension, in feet", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "326y", .Description = "Product Width/Diameter/2nd Dimension, in yards", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "327y", .Description = "Product Depth/Thickness/Height/3rd Dimension, in inches", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "328y", .Description = "Product Depth/Thickness/Height/3rd Dimension, in feet", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "329y", .Description = "Product Depth/Thickness/3rd Dimension, in yards", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "330y", .Description = "Container Gross Weight (kg)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "331y", .Description = "Container Length/1st Dimension (Meters)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "332y", .Description = "Container Width/Diameter/2nd Dimension (Meters)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "333y", .Description = "Container Depth/Thickness/3rd Dimension (Meters)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "334y", .Description = "Container Area (Square Meters)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "335y", .Description = "Container Gross Volume (Liters)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "336y", .Description = "Container Gross Volume (Cubic Meters)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "340y", .Description = "Container Gross Weight (Pounds)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "341y", .Description = "Container Length/1st Dimension, in inches", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "342y", .Description = "Container Length/1st Dimension, in feet", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "343y", .Description = "Container Length/1st Dimension in, in yards", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "344y", .Description = "Container Width/Diameter/2nd Dimension, in inches", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "345y", .Description = "Container Width/Diameter/2nd Dimension, in feet", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "346y", .Description = "Container Width/Diameter/2nd Dimension, in yards", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "347y", .Description = "Container Depth/Thickness/Height/3rd Dimension, in inches", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "348y", .Description = "Container Depth/Thickness/Height/3rd Dimension, in feet", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "349y", .Description = "Container Depth/Thickness/Height/3rd Dimension, in yards", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "350y", .Description = "Product Area (Square Inches)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "351y", .Description = "Product Area (Square Feet)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "352y", .Description = "Product Area (Square Yards)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "353y", .Description = "Container Area (Square Inches)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "354y", .Description = "Container Area (Square Feet)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "355y", .Description = "Container Area (Square Yards)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "356y", .Description = "Net Weight (Troy Ounces)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "357y", .Description = "Net Weight/Volume (Ounces)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "360y", .Description = "Product Volume (Quarts)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "361y", .Description = "Product Volume (Gallons)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "362y", .Description = "Container Gross Volume (Quarts)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "363y", .Description = "Container Gross Volume (U.S. Gallons)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "364y", .Description = "Product Volume (Cubic Inches)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "365y", .Description = "Product Volume (Cubic Feet)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "366y", .Description = "Product Volume (Cubic Yards)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "367y", .Description = "Container Gross Volume (Cubic Inches)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "368y", .Description = "Container Gross Volume (Cubic Feet)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "369y", .Description = "Container Gross Volume (Cubic Yards)", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "37", .Description = "Number of Units Contained", .Format = "N2+N..8", .DataLenghtNoAI = "variable, up to 8", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "390y", .Description = "Amount payable (local currency)", .Format = "N4+N..15", .DataLenghtNoAI = "variable, up to 15", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "391y", .Description = "Amount payable (with ISO currency code)", .Format = "N4+N3+N..15", .DataLenghtNoAI = "variable, 3-18", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "392y", .Description = "Amount payable per single item (local currency)", .Format = "N4+N..15", .DataLenghtNoAI = "variable, up to 15", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "393y", .Description = "Amount payable per single item (with ISO currency code)", .Format = "N4+N3+N..15", .DataLenghtNoAI = "variable, 3-18", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "400", .Description = "Customer Purchase Order Number", .Format = "N3+X..30", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "401", .Description = "Consignment Number", .Format = "N3+X..30", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "402", .Description = "Bill of Lading number", .Format = "N3+N17", .DataLenghtNoAI = "17", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "403", .Description = "Routing code", .Format = "N3+X..30", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "410", .Description = "Ship To/Deliver To Location Code (Global Location Number)", .Format = "N3+N13", .DataLenghtNoAI = "13", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "411", .Description = "Bill To/Invoice Location Code (Global Location Number)", .Format = "N3+N13", .DataLenghtNoAI = "13", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "412", .Description = "Purchase From Location Code (Global Location Number)", .Format = "N3+N13", .DataLenghtNoAI = "13", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "413", .Description = "Ship for, Deliver for, or Forward to Location Code (Global Location Number)", .Format = "N3+N13", .DataLenghtNoAI = "13", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "414", .Description = "Identification of a physical location (Global Location Number)", .Format = "N3+N13", .DataLenghtNoAI = "13", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "415", .Description = "Global Location Number of the Invoicing Party", .Format = "N3+N13", .DataLenghtNoAI = "", ._Fnc1 = ""})
            _ListaAI.Add(New AI(False) With {._AI = "420", .Description = "Ship To/Deliver To Postal Code (Single Postal Authority)", .Format = "N3+X..20", .DataLenghtNoAI = "variable, up to 20", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "421", .Description = "Ship To/Deliver To Postal Code (with ISO country code)", .Format = "N3+N3+X..9", .DataLenghtNoAI = "variable, 3-15", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "422", .Description = "Country of Origin (ISO country code)", .Format = "N3+N3", .DataLenghtNoAI = "3", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "423", .Description = "Country or countries of initial processing", .Format = "N3+N3+N..12", .DataLenghtNoAI = "variable, 3-15", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "424", .Description = "Country of processing", .Format = "N3+N3", .DataLenghtNoAI = "3", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "425", .Description = "Country of disassembly", .Format = "N3+N3", .DataLenghtNoAI = "3", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "426", .Description = "Country of full process chain", .Format = "N3+N3", .DataLenghtNoAI = "3", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "7001", .Description = "NATO Stock Number (NSN)", .Format = "N4+N13", .DataLenghtNoAI = "13", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "7002", .Description = "UN/ECE Meat Carcasses and cuts classification", .Format = "N4+X..30", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "7003", .Description = "expiration date and time", .Format = "N4+N10", .DataLenghtNoAI = "10", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "7004", .Description = "Active Potency", .Format = "N4+N..4", .DataLenghtNoAI = "variable, up to 4", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "703n", .Description = "Processor approval (with ISO country code) -- n indicates sequence number of several processors", .Format = "N4+N3+X..27", .DataLenghtNoAI = "variable, 3-30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8001", .Description = "Roll Products - Width/Length/Core Diameter/Direction/Splices", .Format = "N4+N14", .DataLenghtNoAI = "14", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8002", .Description = "Mobile phone identifier", .Format = "N4+X..20", .DataLenghtNoAI = "variable, up to 20", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8003", .Description = "Global Returnable Asset Identifier", .Format = "N4+N14+X..16", .DataLenghtNoAI = "variable, 14-30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8004", .Description = "Global Individual Asset Identifier", .Format = "N4+X..30", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8005", .Description = "Price per Unit of Measure", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8006", .Description = "identification of the components of an item", .Format = "N4+N14+N2+N2", .DataLenghtNoAI = "18", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8007", .Description = "International Bank Account Number", .Format = "N4+X..30", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8008", .Description = "Date/time of production", .Format = "N4+N8+N..4", .DataLenghtNoAI = "variable, 8-12", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8018", .Description = "Global Service Relation Number", .Format = "N4+N18", .DataLenghtNoAI = "18", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8020", .Description = "Payment slip reference number", .Format = "N4+X..25", .DataLenghtNoAI = "variable, up to 25", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8100", .Description = "Coupon Extended Code: Number System and Offer", .Format = "N4+N6", .DataLenghtNoAI = "6", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8101", .Description = "Coupon Extended Code: Number System, Offer, End of Offer", .Format = "N4+N1+N5+N4", .DataLenghtNoAI = "10", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8102", .Description = "Coupon Extended Code: Number System preceded by 0", .Format = "N4+N1+N1", .DataLenghtNoAI = "2", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "8110", .Description = "Coupon code ID (North America)", .Format = "N4+X..70", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "90", .Description = "Mutually Agreed Between Trading Partners", .Format = "N4+X..70", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "91-99", .Description = "Internal Company Codes", .Format = "N2+X..30", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
            _ListaAI.Add(New AI(False) With {._AI = "91-99", .Description = "Internal Company Codes", .Format = "N2+X..30", .DataLenghtNoAI = "variable, up to 30", ._Fnc1 = "Y"})
        End If
    End Sub

    Public Function getAI(ByVal AI As String) As AI
        Return _ListaAI.Find(Function(p) p.AI = AI)
    End Function


    Private _ListaAI As List(Of AI)


End Class
