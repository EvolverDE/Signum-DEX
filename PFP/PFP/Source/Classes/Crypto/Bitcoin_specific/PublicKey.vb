Option Strict On
Option Explicit On

Imports System.Numerics
'Imports System.Runtime.InteropServices.WindowsRuntime

Namespace Secp256k1Vb

    Public NotInheritable Class PublicKey
        Const EvenPublicKey As Byte = 2
        Const OddPublicKey As Byte = 3
        Const FullPublicKey As Byte = 4

        Public ReadOnly X, Y As BigInteger
        Public ReadOnly Property ToXY As BigInteger()
            Get
                Return New BigInteger() {Me.X, Me.Y}
            End Get
        End Property

        Public Sub New(xx As BigInteger, yy As BigInteger)
            Me.X = xx : Me.Y = yy
        End Sub

        Public Sub New(xy As BigInteger())
            Me.X = xy(0) : Me.Y = xy(1)
        End Sub

        Public Function Serialize(ByRef buf As Byte(), Optional compressed As Boolean = False) As Int32
            Dim len = If(compressed, 33, 65)
            If buf.Length < len Then
                'Throw New ArgumentOutOfRangeException(NameOf(buf))
                Return 0
            End If

            Dim XSpan As List(Of Byte) = New List(Of Byte)(Me.X.ToByteArray)
            Dim YSpan As List(Of Byte) = New List(Of Byte)(Me.Y.ToByteArray)
            Dim BufList As List(Of Byte) = New List(Of Byte)

            If XSpan.Count < 32 Then
                For i As Integer = XSpan.Count To 32 - 1
                    XSpan.Add(0)
                Next
            End If

            If YSpan.Count < 32 Then
                For i As Integer = YSpan.Count To 32 - 1
                    YSpan.Add(0)
                Next
            End If

            BufList.AddRange(XSpan.GetRange(0, 32))

            'Me.X.ToByteArray.AsSpan.Slice(0, 32).CopyTo(buf.AsSpan.Slice(1, 32))

            If Not compressed Then
                'Me.Y.ToByteArray.AsSpan.Slice(0, 32).CopyTo(buf.AsSpan.Slice(33, 32))
                Dim T_BufList As List(Of Byte) = New List(Of Byte)(YSpan.GetRange(0, 32))

                BufList.InsertRange(0, T_BufList.ToArray())

                BufList.Add(FullPublicKey)
                BufList.Reverse()

                buf = BufList.ToArray

                Return 65
            Else

                BufList.Add(If(Me.Y.IsEven, EvenPublicKey, OddPublicKey))
                BufList.Reverse()
                buf = BufList.ToArray

                Return 33
            End If

        End Function

        Public Function Serialize(Optional compressed As Boolean = False) As Byte()
            Dim result = New Byte(If(compressed, 33, 65) - 1) {}
            Me.Serialize(result, compressed)
            Return result
        End Function

        Public Shared Function Parse(bytes As Byte(), ByRef readBytes As Int32) As PublicKey
            If bytes.Length < 33 Then
                'Throw New ArgumentOutOfRangeException(Nothing)
                Return New PublicKey(New BigInteger(0), New BigInteger(0))
            End If

            Dim ByteList As List(Of Byte) = New List(Of Byte)(bytes)
            ByteList = ByteList.GetRange(1, 32)

            ByteList.Reverse()

            Dim y As BigInteger, x As BigInteger = ToBigInteger(ByteList.ToArray) ' bytes.AsSpan.Slice(1, 32).ToArray())
            If x >= Math_Modulo.P Then
                'Throw New ArgumentOutOfRangeException(Nothing)
                Return New PublicKey(New BigInteger(0), New BigInteger(0))
            End If
            If bytes(0) = EvenPublicKey OrElse bytes(0) = OddPublicKey Then
                Try
                    y = Secp256k1.Y_Von_X(x)(0)
                    If y.IsEven <> (bytes(0) = EvenPublicKey) Then
                        ' y.v0 % 2 == 0 && bytes[0] == OddPublicKey
                        ' y.v0 % 2 != 0 && bytes[0] == EvenPublicKey
                        y = -y
                    End If
                    readBytes = 33
                    Return New PublicKey(x, y)
                Catch ex As Exception
                    'Throw New ArgumentOutOfRangeException(Nothing)
                    Return New PublicKey(New BigInteger(0), New BigInteger(0))
                End Try
            ElseIf bytes(0) = FullPublicKey Then
                If bytes.Length < 65 Then
                    'Throw New ArgumentOutOfRangeException(Nothing)
                    Return New PublicKey(New BigInteger(0), New BigInteger(0))
                End If

                ByteList = New List(Of Byte)(bytes)
                ByteList.GetRange(33, 32)

                ByteList.Reverse()

                y = ToBigInteger(ByteList.ToArray) ' bytes.AsSpan.Slice(33, 32).ToArray())
                If Not (x Xor 3) + 7 = (y Xor 2) Then
                    'Throw New ArgumentOutOfRangeException(Nothing)
                    Return New PublicKey(New BigInteger(0), New BigInteger(0))
                End If
                readBytes = 65
                Return New PublicKey(x, y)
            Else
                'Throw New ArgumentOutOfRangeException(Nothing)
                Return New PublicKey(New BigInteger(0), New BigInteger(0))
            End If
        End Function

        Public Shared Function Parse(bytes As Byte()) As PublicKey
            Dim blind As Int32 = 0
            Return Parse(bytes, blind)
        End Function

        Private Shared Function ToBigInteger(bytes As Byte()) As BigInteger
            Dim tmp As Byte()
            If Not bytes(bytes.Length - 1) = 0 Then
                tmp = bytes.Concat({0}).ToArray()
            Else : tmp = bytes
            End If
            Return New BigInteger(tmp)
        End Function

    End Class

End Namespace
