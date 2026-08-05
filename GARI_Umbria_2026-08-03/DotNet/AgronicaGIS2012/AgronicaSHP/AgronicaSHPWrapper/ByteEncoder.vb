' Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
'
' This file is part of GeoAPI.Net.
' GeoAPI.Net is free software; you can redistribute it and/or modify
' it under the terms of the GNU Lesser General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
' 
' GeoAPI.Net is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU Lesser General Public License for more details.

' You should have received a copy of the GNU Lesser General Public License
' along with GeoAPI.Net; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 


Namespace GeoAPI.DataStructures
    Public NotInheritable Class ByteEncoder
        Private Sub New()
        End Sub
#Region "Endian conversion helper routines"
        ''' <summary>
        ''' Returns the value encoded in Big Endian (PPC, XDR) format.
        ''' </summary>
        ''' <param name="value">Value to encode.</param>
        ''' <returns>Big-endian encoded value.</returns>
        Public Shared Function GetBigEndian(value As Int32) As Int32
            If BitConverter.IsLittleEndian Then
                Return swapByteOrder(value)
            Else
                Return value
            End If
        End Function

        ''' <summary>
        ''' Returns the value encoded in Big Endian (PPC, XDR) format.
        ''' </summary>
        ''' <param name="value">Value to encode.</param>
        ''' <returns>Big-endian encoded value.</returns>
        <CLSCompliant(False)> _
        Public Shared Function GetBigEndian(value As UInt16) As UInt16
            Return If(BitConverter.IsLittleEndian, swapByteOrder(value), value)
        End Function

        ''' <summary>
        ''' Returns the value encoded in Big Endian (PPC, XDR) format.
        ''' </summary>
        ''' <param name="value">Value to encode.</param>
        ''' <returns>Big-endian encoded value.</returns>
        <CLSCompliant(False)> _
        Public Shared Function GetBigEndian(value As UInt32) As UInt32
            Return If(BitConverter.IsLittleEndian, swapByteOrder(value), value)
        End Function

        ''' <summary>
        ''' Returns the value encoded in Big Endian (PPC, XDR) format.
        ''' </summary>
        ''' <param name="value">Value to encode.</param>
        ''' <returns>Big-endian encoded value.</returns>
        Public Shared Function GetBigEndian(value As [Double]) As [Double]
            Return If(BitConverter.IsLittleEndian, swapByteOrder(value), value)
        End Function

        ''' <summary>
        ''' Returns the value encoded in Little Endian (x86, NDR) format.
        ''' </summary>
        ''' <param name="value">Value to encode.</param>
        ''' <returns>Little-endian encoded value.</returns>
        Public Shared Function GetLittleEndian(value As Int32) As Int32
            Return If(BitConverter.IsLittleEndian, value, swapByteOrder(value))
        End Function

        ''' <summary>
        ''' Returns the value encoded in Little Endian (x86, NDR) format.
        ''' </summary>
        ''' <param name="value">Value to encode.</param>
        ''' <returns>Little-endian encoded value.</returns>
        <CLSCompliant(False)> _
        Public Shared Function GetLittleEndian(value As UInt32) As UInt32
            Return If(BitConverter.IsLittleEndian, value, swapByteOrder(value))
        End Function

        ''' <summary>
        ''' Returns the value encoded in Little Endian (x86, NDR) format.
        ''' </summary>
        ''' <param name="value">Value to encode.</param>
        ''' <returns>Little-endian encoded value.</returns>
        <CLSCompliant(False)> _
        Public Shared Function GetLittleEndian(value As UInt16) As UInt16
            Return If(BitConverter.IsLittleEndian, value, swapByteOrder(value))
        End Function

        ''' <summary>
        ''' Returns the value encoded in Little Endian (x86, NDR) format.
        ''' </summary>
        ''' <param name="value">Value to encode.</param>
        ''' <returns>Little-endian encoded value.</returns>
        Public Shared Function GetLittleEndian(value As [Double]) As [Double]
            Return If(BitConverter.IsLittleEndian, value, swapByteOrder(value))
        End Function

        ''' <summary>
        ''' Swaps the Byte order of an <see cref="Int32"/>.
        ''' </summary>
        ''' <param name="value"><see cref="Int32"/> to swap the bytes of.</param>
        ''' <returns>Byte order swapped <see cref="Int32"/>.</returns>
        Private Shared Function swapByteOrder(value As Int32) As Int32
            Dim swapped As Int32 = _
                CType(( _
                    (&HFF) And (value >> 24) Or _
                    (&HFF00) And (value >> 8) Or _
                    (&HFF0000) And (value << 8) Or _
                    (&HFF000000) And (value << 24) _
                ), Int32)
            Return swapped
        End Function

        ''' <summary>
        ''' Swaps the byte order of a <see cref="UInt16"/>.
        ''' </summary>
        ''' <param name="value"><see cref="UInt16"/> to swap the bytes of.</param>
        ''' <returns>Byte order swapped <see cref="UInt16"/>.</returns>
        Private Shared Function swapByteOrder(value As UInt16) As UInt16
            Return CType((&HFF And (value >> 8)) Or (&HFF00 And (value << 8)), UInt16)
        End Function

        ''' <summary>
        ''' Swaps the byte order of a <see cref="UInt32"/>.
        ''' </summary>
        ''' <param name="value"><see cref="UInt32"/> to swap the bytes of.</param>
        ''' <returns>Byte order swapped <see cref="UInt32"/>.</returns>
        Private Shared Function swapByteOrder(value As UInt32) As UInt32
            Dim swapped As UInt32 = ((&HFF) And (value >> 24) Or (&HFF00) And (value >> 8) Or (&HFF0000) And (value << 8) Or (&HFF000000UI) And (value << 24))
            Return swapped
        End Function

        ''' <summary>
        ''' Swaps the byte order of a <see cref="Int64"/>.
        ''' </summary>
        ''' <param name="value"><see cref="Int64"/> to swap the bytes of.</param>
        ''' <returns>Byte order swapped <see cref="Int64"/>.</returns>
        Private Shared Function swapByteOrder(value As Int64) As Int64
            Dim uvalue As UInt64 = CType(value, UInt64)
            Dim swapped As UInt64 = ((&HFF) And (uvalue >> 56) Or (&HFF00) And (uvalue >> 40) Or (&HFF0000) And (uvalue >> 24) Or (&HFF000000UI) And (uvalue >> 8) Or (&HFF00000000L) And (uvalue << 8) Or (&HFF0000000000L) And (uvalue << 24) Or (&HFF000000000000L) And (uvalue << 40) Or (&HFF00000000000000UL) And (uvalue << 56))
            Return CType(swapped, Int64)
        End Function

        ''' <summary>
        ''' Swaps the byte order of a <see cref="Double"/> (double precision IEEE 754)
        ''' </summary>
        ''' <param name="value"><see cref="Double"/> to swap.</param>
        ''' <returns>Byte order swapped <see cref="Double"/> value.</returns>
        Private Shared Function swapByteOrder(value As [Double]) As [Double]
            Dim bits As Int64 = BitConverter.DoubleToInt64Bits(value)
            bits = swapByteOrder(bits)
            Return BitConverter.Int64BitsToDouble(bits)
        End Function
#End Region
    End Class
End Namespace
