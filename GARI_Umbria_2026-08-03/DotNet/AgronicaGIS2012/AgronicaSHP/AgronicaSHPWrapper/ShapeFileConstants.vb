' Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
'
' This file is part of SharpMap.
' SharpMap is free software; you can redistribute it and/or modify
' it under the terms of the GNU Lesser General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
' 
' SharpMap is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU Lesser General Public License for more details.

' You should have received a copy of the GNU Lesser General Public License
' along with SharpMap; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 


Namespace TestShapeFile
    ''' <summary>
    ''' Represents invariant shapefile values, offsets and lengths
    ''' derived from the shapefile specification.
    ''' </summary>
    Public Class ShapeFileConstants

        Public Const Int32SizeBytes As Int32 = 4
        Public Const DoubleSizeBytes As Int32 = 8

        ''' <summary>
        ''' Size, in bytes, of the shapefile header region.
        ''' </summary>
        Public Const HeaderSizeBytes As Int32 = 100

        ''' <summary>
        ''' The first value in any shapefile.
        ''' </summary>
        Public Const HeaderStartCode As Int32 = 9994

        ''' <summary>
        ''' The version of any valid shapefile.
        ''' </summary>
        Public Const VersionCode As Int32 = 1000

        ''' <summary>
        ''' The number of bytes in a shapefile record header 
        ''' (per-record preamble).
        ''' </summary>
        Public Const ShapeRecordHeaderByteLength As Int32 = 8


        ''' <summary>
        ''' The number of bytes in a record header (per-record preamble)
        ''' in a shapefile's index file.
        ''' </summary>
        Public Const IndexRecordByteLength As Int32 = 8

        ''' <summary>
        ''' The number of bytes used to store the BoundingBox, or 
        ''' extents for the shapefile.
        ''' </summary>
        Public Const BoundingBoxFieldByteLength As Int32 = 32

        ''' <summary>
        ''' The name given to the row identifier in a ShapeFileProvider.
        ''' </summary>
        Public Shared ReadOnly IdColumnName As [String] = "OID"
    End Class
End Namespace
