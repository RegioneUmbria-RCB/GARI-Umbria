' Filename:    MainWindow.xaml.cs
' Description: MainWindow is the main application window for the
'              TestShapeFile application.
' 2007-01-22 nschan Initial revision.

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Shapes

Namespace TestShapeFile

    
    ''' <summary>
    ''' Interaction logic for MainWindow.xaml.
    ''' </summary>
    Partial Public NotInheritable Class MainWindow
        Inherits System.Windows.Window
#Region "Private fields"
        ' Open file dialog for choosing a shapefile.
        Private openFileDialog As New Microsoft.Win32.OpenFileDialog()

        ' Helper class for reading shapefile and displaying WPF shapes on canvas.
        Private shapeDisplay As ShapeDisplay
#End Region

#Region "Constructor"

        ''' <summary>
        ''' Constructor for MainWindow class.
        ''' </summary>
        Public Sub New()
            InitializeComponent()

            ' Create the shape display instance.
            Me.shapeDisplay = New ShapeDisplay(Me, Me.canvas1)

            ' Colorize the menu.
            Me.menu1.Background = New LinearGradientBrush(Color.FromArgb(255, 158, 190, 245), Color.FromArgb(255, 196, 218, 250), 45)

            ' Colorize the canvas.
            Me.canvas1.Background = New LinearGradientBrush(Colors.WhiteSmoke, Colors.LightSteelBlue, 45)
        End Sub
#End Region

#Region "Window closing"
        Private Sub mainWindow_Closing(sender As Object, e As CancelEventArgs)
            ' Ask the user to confirm exit.
            Dim msg As String = "esco?" 'TestShapeFile.Properties.Resources.MainWindow_ExitQuestion
            Dim appName As String = "" 'TestShapeFile.Properties.Resources.MainWindow_Title
            Dim result As MessageBoxResult = MessageBox.Show(msg, appName, MessageBoxButton.YesNo, MessageBoxImage.Question)
            If result = MessageBoxResult.Yes Then
                ' Proceed with closing of the window.
                'Me.shapeDisplay.CancelReadShapeFile()
                e.Cancel = False
                Return
            End If

            ' Cancel the closing of the window.
            e.Cancel = True
        End Sub
#End Region

#Region "File Menu"
        ''' <summary>
        ''' Handle when the File menu is opened, so that we can update
        ''' the enabled state of the menu items appropriately.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub fileMI_SubmenuOpened(sender As Object, e As EventArgs)
            ' Disable menu items if we are already reading a shapefile.
            Me.openMI.IsEnabled = Not Me.shapeDisplay.IsReadingShapeFile
            Me.resetMI.IsEnabled = Me.openMI.IsEnabled
        End Sub

        ''' <summary>
        ''' Handle File|Open menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub openMI_Click(sender As Object, e As EventArgs)
            ' Show the Open File dialog.
            Me.openFileDialog.Filter = "Shapefiles (*.shp)|*.shp"
            Me.openFileDialog.Title = "Open Shapefile for Import"
            Dim result As Nullable(Of Boolean) = Me.openFileDialog.ShowDialog()
            If Not result.HasValue OrElse Not result.Value Then
                Return
            End If

            ' Read the shapefile.
            Me.shapeDisplay.ReadShapeFile(Me.openFileDialog.FileName)
        End Sub

        ''' <summary>
        ''' Handle File|Reset menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub resetMI_Click(sender As Object, e As EventArgs)
            ' Ask user to confirm reset of canvas.
            Dim msg As String = "Reset?" 'TestShapeFile.Properties.Resources.MainWindow_ResetQuestion
            Dim appName As String = "" 'TestShapeFile.Properties.Resources.MainWindow_Title
            Dim result As MessageBoxResult = MessageBox.Show(msg, appName, MessageBoxButton.YesNo, MessageBoxImage.Question)
            If result = MessageBoxResult.Yes Then
                ' Reset the canvas.
                Me.shapeDisplay.ResetCanvas()
            End If
        End Sub

        ''' <summary>
        ''' Handle File|Reset menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub ExportGiasMI_Click(sender As Object, e As EventArgs)
            ' Ask user to confirm reset of canvas.
            Dim msg As String = "Esporta su gias?" 'TestShapeFile.Properties.Resources.MainWindow_ResetQuestion
            Dim appName As String = "" 'TestShapeFile.Properties.Resources.MainWindow_Title
            Dim result As MessageBoxResult = MessageBox.Show(msg, appName, MessageBoxButton.YesNo, MessageBoxImage.Question)
            If result = MessageBoxResult.Yes Then

                MessageBox.Show("Esportazione completata")
            End If
        End Sub


        ''' <summary>
        ''' Handle File|Exit menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub exitMI_Click(sender As Object, e As EventArgs)
            ' When we call Close(), this will trigger the Window.Closing event.
            Me.Close()
        End Sub
#End Region

#Region "View Menu"
        ''' <summary>
        ''' Handle when the View Menu is opened.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub viewMI_SubmenuOpened(sender As Object, e As EventArgs)
            ' Update enabled and checked states of menu items.
            Me.displayLonLatMI.IsEnabled = (Me.shapeDisplay.CanZoom AndAlso Not Me.shapeDisplay.IsReadingShapeFile)
            Me.displayLonLatMI.IsChecked = Me.shapeDisplay.IsDisplayLonLatEnabled

            Me.enablePanningMI.IsEnabled = Me.displayLonLatMI.IsEnabled
            Me.enablePanningMI.IsChecked = Me.shapeDisplay.IsPanningEnabled

            Me.zoomMI.IsEnabled = Me.displayLonLatMI.IsEnabled
        End Sub

        ''' <summary>
        ''' Handle View|Display Lon/Lat menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub displayLonLatMI_Click(sender As Object, e As EventArgs)
            ' Toggle the display of the lon/lat coordinates on the canvas.
            Me.shapeDisplay.IsDisplayLonLatEnabled = Not Me.shapeDisplay.IsDisplayLonLatEnabled
        End Sub

        ''' <summary>
        ''' Handle View|Enable Panning menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub enablePanningMI_Click(sender As Object, e As EventArgs)
            ' Toggle the panning feature on or off.
            Me.shapeDisplay.IsPanningEnabled = Not Me.shapeDisplay.IsPanningEnabled
        End Sub
#End Region

#Region "Zoom Menu"
        ''' <summary>
        ''' Handle when the View|Zoom menu is opened, so that we can update
        ''' the enabled state of the menu items appropriately.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub zoomMI_SubmenuOpened(sender As Object, e As EventArgs)
            Me.zoom50MI.IsEnabled = (Me.shapeDisplay.CanZoom AndAlso Not Me.shapeDisplay.IsReadingShapeFile)
            Me.zoom100MI.IsEnabled = Me.zoom50MI.IsEnabled
            Me.zoom200MI.IsEnabled = Me.zoom50MI.IsEnabled
            Me.zoom400MI.IsEnabled = Me.zoom50MI.IsEnabled
            Me.zoom800MI.IsEnabled = Me.zoom50MI.IsEnabled
            Me.zoom1600MI.IsEnabled = Me.zoom50MI.IsEnabled
            Me.zoom3200MI.IsEnabled = Me.zoom50MI.IsEnabled
        End Sub

        ''' <summary>
        ''' Handle View|Zoom 50% menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub zoom50_Click(sender As Object, e As EventArgs)
            Me.shapeDisplay.Zoom(0.5)
        End Sub

        ''' <summary>
        ''' Handle View|Zoom 100% menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub zoom100_Click(sender As Object, e As EventArgs)
            Me.shapeDisplay.Zoom(1)
        End Sub

        ''' <summary>
        ''' Handle View|Zoom 200% menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub zoom200_Click(sender As Object, e As EventArgs)
            Me.shapeDisplay.Zoom(2)
        End Sub

        ''' <summary>
        ''' Handle View|Zoom 400% menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub zoom400_Click(sender As Object, e As EventArgs)
            Me.shapeDisplay.Zoom(4)
        End Sub

        ''' <summary>
        ''' Handle View|Zoom 800% menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub zoom800_Click(sender As Object, e As EventArgs)
            Me.shapeDisplay.Zoom(8)
        End Sub

        ''' <summary>
        ''' Handle View|Zoom 1600% menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub zoom1600_Click(sender As Object, e As EventArgs)
            Me.shapeDisplay.Zoom(16)
        End Sub

        ''' <summary>
        ''' Handle View|Zoom 3200% menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub zoom3200_Click(sender As Object, e As EventArgs)
            Me.shapeDisplay.Zoom(32)
        End Sub
#End Region

#Region "Geometry Type Menu"
        ''' <summary>
        ''' Handle when the Options|Geometry Type menu is opened, so that we can
        ''' update the enabled and checked states of the menu items appropriately.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub geometryTypeMI_SubmenuOpened(sender As Object, e As EventArgs)
            ' Update enabled states.
            Me.pathGeometryMI.IsEnabled = Not Me.shapeDisplay.IsReadingShapeFile
            Me.streamGeometryMI.IsEnabled = Me.pathGeometryMI.IsEnabled
            Me.streamGeometryUnstrokedMI.IsEnabled = Me.pathGeometryMI.IsEnabled

            ' Update checked states.
            Select Case Me.shapeDisplay.GeometryType
                Case GeometryType.UsePathGeometry
                    Me.pathGeometryMI.IsChecked = True
                    Me.streamGeometryMI.IsChecked = False
                    Me.streamGeometryUnstrokedMI.IsChecked = False
                    Exit Select
                Case GeometryType.UseStreamGeometry
                    Me.pathGeometryMI.IsChecked = False
                    Me.streamGeometryMI.IsChecked = True
                    Me.streamGeometryUnstrokedMI.IsChecked = False
                    Exit Select
                Case GeometryType.UseStreamGeometryNotStroked
                    Me.pathGeometryMI.IsChecked = False
                    Me.streamGeometryMI.IsChecked = False
                    Me.streamGeometryUnstrokedMI.IsChecked = True
                    Exit Select
            End Select
        End Sub

        ''' <summary>
        ''' Handle Geometry Type|Path Geometry menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub pathGeometryMI_Click(sender As Object, e As EventArgs)
            Me.shapeDisplay.GeometryType = GeometryType.UsePathGeometry
        End Sub

        ''' <summary>
        ''' Handle Geometry|Stream Geometry menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub streamGeometryMI_Click(sender As Object, e As EventArgs)
            Me.shapeDisplay.GeometryType = GeometryType.UseStreamGeometry
        End Sub

        ''' <summary>
        ''' Handle Geometry Type|Stream Geometry Unstroked menu item click.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub streamGeometryUnstrokedMI_Click(sender As Object, e As EventArgs)
            Me.shapeDisplay.GeometryType = GeometryType.UseStreamGeometryNotStroked
        End Sub
#End Region

#Region "Keyboard handling"
        ''' <summary>
        ''' Handle the KeyDown event for the main window.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub mainWindow_KeyDown(sender As Object, e As KeyboardEventArgs)
            ' Pan 10% of the canvas width or height.
            If e.KeyboardDevice.IsKeyDown(Key.Left) Then
                Me.shapeDisplay.Pan(0.1, 0)
            ElseIf e.KeyboardDevice.IsKeyDown(Key.Right) Then
                Me.shapeDisplay.Pan(-0.1, 0)
            ElseIf e.KeyboardDevice.IsKeyDown(Key.Up) Then
                Me.shapeDisplay.Pan(0, 0.1)
            ElseIf e.KeyboardDevice.IsKeyDown(Key.Down) Then
                Me.shapeDisplay.Pan(0, -0.1)
            End If
        End Sub
#End Region
    End Class
End Namespace

' END
