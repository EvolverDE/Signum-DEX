'Public Class Curve25519

'End Class

' Ported parts from Java to C# and refactored by Hans Wolff, 17/09/2013 

' Ported from C to Java by Dmitry Skiba [sahn0], 23/02/08.
'  Original: http://code.google.com/p/curve25519-java/
' 

' Generic 64-bit integer implementation of Curve25519 ECDH
'  Written by Matthijs van Duin, 200608242056
'  Public domain.
' 
'  Based on work by Daniel J Bernstein, http://cr.yp.to/ecdh.html
' 

Imports System
Imports System.Security.Cryptography

Namespace Elliptic
    Public Class Curve25519
        ' key size 
        Public Const KeySize As Integer = 32

        ' group order (a prime near 2^252+2^124) 
        Private Shared ReadOnly Order As Byte() = {237, 211, 245, 92, 26, 99, 18, 88, 214, 156, 247, 162, 222, 249, 222, 20, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16}


        ' ******* KEY AGREEMENT ********

        ''' <summary>
        ''' Private key clamping (inline, for performance)
        ''' </summary>
        ''' <param name="key">[out] 32 random bytes</param>
        Public Shared Sub ClampPrivateKeyInline(ByVal key As Byte())
            If key Is Nothing Then Throw New ArgumentNullException("key")
            If key.Length <> 32 Then Throw New ArgumentException(String.Format("key must be 32 bytes long (but was {0} bytes long)", key.Length))
            key(31) = key(31) And &H7F
            key(31) = key(31) Or &H40
            key(0) = key(0) And &HF8
        End Sub


        ''' <summary>
        ''' Private key clamping
        ''' </summary>
        ''' <param name="rawKey">[out] 32 random bytes</param>
        Public Shared Function ClampPrivateKey(ByVal rawKey As Byte()) As Byte()
            If rawKey Is Nothing Then Throw New ArgumentNullException("rawKey")
            If rawKey.Length <> 32 Then Throw New ArgumentException(String.Format("rawKey must be 32 bytes long (but was {0} bytes long)", rawKey.Length), "rawKey")
            Dim res = New Byte(31) {}
            Array.Copy(rawKey, res, 32)
            res(31) = res(31) And &H7F
            res(31) = res(31) Or &H40
            res(0) = res(0) And &HF8
            Return res
        End Function


        ''' <summary>
        ''' Creates a random private key
        ''' </summary>
        ''' <returns>32 random bytes that are clamped to a suitable private key</returns>
        Public Shared Function CreateRandomPrivateKey() As Byte()
            Dim privateKey = New Byte(31) {}
            RandomNumberGenerator.Create().GetBytes(privateKey)
            ClampPrivateKeyInline(privateKey)
            Return privateKey
        End Function


        ''' <summary>
        ''' Key-pair generation (inline, for performance)
        ''' </summary>
        ''' <param name="publicKey">[out] public key</param>
        ''' <param name="signingKey">[out] signing key (ignored if NULL)</param>
        ''' <param name="privateKey">[out] private key</param>
        ''' <remarks>WARNING: if signingKey is not NULL, this function has data-dependent timing</remarks>
        Public Shared Sub KeyGenInline(ByVal publicKey As Byte(), ByVal signingKey As Byte(), ByVal privateKey As Byte())
            If publicKey Is Nothing Then Throw New ArgumentNullException("publicKey")
            If publicKey.Length <> 32 Then Throw New ArgumentException(String.Format("publicKey must be 32 bytes long (but was {0} bytes long)", publicKey.Length), "publicKey")
            If signingKey Is Nothing Then Throw New ArgumentNullException("signingKey")
            If signingKey.Length <> 32 Then Throw New ArgumentException(String.Format("signingKey must be 32 bytes long (but was {0} bytes long)", signingKey.Length), "signingKey")
            If privateKey Is Nothing Then Throw New ArgumentNullException("privateKey")
            If privateKey.Length <> 32 Then Throw New ArgumentException(String.Format("privateKey must be 32 bytes long (but was {0} bytes long)", privateKey.Length), "privateKey")
            RandomNumberGenerator.Create().GetBytes(privateKey)
            ClampPrivateKeyInline(privateKey)
            Core(publicKey, signingKey, privateKey, Nothing)
        End Sub


        ''' <summary>
        ''' Generates the public key out of the clamped private key
        ''' </summary>
        ''' <param name="privateKey">private key (must use ClampPrivateKey first!)</param>
        Public Shared Function GetPublicKey(ByVal privateKey As Byte()) As Byte()
            Dim publicKey = New Byte(31) {}
            Core(publicKey, Nothing, privateKey, Nothing)
            Return publicKey
        End Function


        ''' <summary>
        ''' Generates signing key out of the clamped private key
        ''' </summary>
        ''' <param name="privateKey">private key (must use ClampPrivateKey first!)</param>
        Public Shared Function GetSigningKey(ByVal privateKey As Byte()) As Byte()
            Dim signingKey = New Byte(31) {}
            Dim publicKey = New Byte(31) {}
            Core(publicKey, signingKey, privateKey, Nothing)
            Return signingKey
        End Function


        ''' <summary>
        ''' Key agreement
        ''' </summary>
        ''' <param name="privateKey">[in] your private key for key agreement</param>
        ''' <param name="peerPublicKey">[in] peer's public key</param>
        ''' <returns>shared secret (needs hashing before use)</returns>
        Public Shared Function GetSharedSecret(ByVal privateKey As Byte(), ByVal peerPublicKey As Byte()) As Byte()
            Dim sharedSecret = New Byte(31) {}
            Core(sharedSecret, Nothing, privateKey, peerPublicKey)
            Return sharedSecret
        End Function


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''' 

        ' sahn0:
        '  Using this class instead of long[10] to avoid bounds checks. 

        Private NotInheritable Class Long10
            Public Sub New()
            End Sub

            Public Sub New(ByVal n0 As Long, ByVal n1 As Long, ByVal n2 As Long, ByVal n3 As Long, ByVal n4 As Long, ByVal n5 As Long, ByVal n6 As Long, ByVal n7 As Long, ByVal n8 As Long, ByVal n9 As Long)
                Me.N0 = n0
                Me.N1 = n1
                Me.N2 = n2
                Me.N3 = n3
                Me.N4 = n4
                Me.N5 = n5
                Me.N6 = n6
                Me.N7 = n7
                Me.N8 = n8
                Me.N9 = n9
            End Sub

            Public N0, N1, N2, N3, N4, N5, N6, N7, N8, N9 As Long
        End Class


        ' ******************* radix 2^8 math ********************

        Private Shared Sub Copy32(ByVal source As Byte(), ByVal destination As Byte())
            Array.Copy(source, 0, destination, 0, 32)
        End Sub


        ' p[m..n+m-1] = q[m..n+m-1] + z * x 
        ' n is the size of x 
        ' n+m is the size of p and q 

        Private Shared Function MultiplyArraySmall(ByVal p As Byte(), ByVal q As Byte(), ByVal m As Integer, ByVal x As Byte(), ByVal n As Integer, ByVal z As Integer) As Integer
            Dim v As Integer = 0
            Dim i As Integer = 0

            While i < n
                v += (q(i + m) And &HFF) + z * (x(i) And &HFF)
                p(i + m) = CByte(v)
                v >>= 8
                Threading.Interlocked.Increment(i)
            End While

            Return v
        End Function


        ' p += x * y * z  where z is a small integer
        '  x is size 32, y is size t, p is size 32+t
        '  y is allowed to overlap with p+32 if you don't care about the upper half  

        Private Shared Sub MultiplyArray32(ByVal p As Byte(), ByVal x As Byte(), ByVal y As Byte(), ByVal t As Integer, ByVal z As Integer)
            Const n As Integer = 31
            Dim w As Integer = 0
            Dim i As Integer = 0

            While i < t
                Dim zy As Integer = z * (y(i) And &HFF)
                w += MultiplyArraySmall(p, p, i, x, n, zy) + (p(i + n) And &HFF) + zy * (x(n) And &HFF)
                p(i + n) = CByte(w)
                w >>= 8
                i += 1
            End While

            p(i + n) = CByte(w + (p(i + n) And &HFF))
        End Sub


        ' divide r (size n) by d (size t), returning quotient q and remainder r
        '  quotient is size n-t+1, remainder is size t
        '  requires t > 0 && d[t-1] != 0
        '  requires that r[-1] and d[-1] are valid memory locations
        '  q may overlap with r+t 
        Private Shared Sub DivMod(ByVal q As Byte(), ByVal r As Byte(), ByVal n As Integer, ByVal d As Byte(), ByVal t As Integer)
            Dim rn As Integer = 0
            Dim dt As Integer = (d(t - 1) And &HFF) << 8

            If t > 1 Then
                dt = dt Or d(t - 2) And &HFF
            End If

            While Math.Max(Threading.Interlocked.Decrement(n), n + 1) >= t
                Dim z As Integer = rn << 16 Or (r(n) And &HFF) << 8

                If n > 0 Then
                    z = z Or r(n - 1) And &HFF
                End If

                z /= dt
                rn += MultiplyArraySmall(r, r, n - t + 1, d, t, -z)
                q(n - t + 1) = CByte(z + rn And &HFF) ' rn is 0 or -1 (underflow) 
                MultiplyArraySmall(r, r, n - t + 1, d, t, -rn)
                rn = r(n) And &HFF
                r(n) = 0
            End While

            r(t - 1) = CByte(rn)
        End Sub

        Private Shared Function GetNumSize(ByVal num As Byte(), ByVal maxSize As Integer) As Integer
            Dim i As Integer = maxSize

            While i >= 0
                If num(i) = 0 Then Return i + 1
                i += 1
            End While

            Return 0
        End Function


        ''' <summary>
        ''' Returns x if a contains the gcd, y if b.
        ''' </summary>
        ''' <param name="x">x and y must have 64 bytes space for temporary use.</param>
        ''' <param name="y">x and y must have 64 bytes space for temporary use.</param>
        ''' <param name="a">requires that a[-1] and b[-1] are valid memory locations</param>
        ''' <param name="b">requires that a[-1] and b[-1] are valid memory locations</param>
        ''' <returns>Also, the returned buffer contains the inverse of a mod b as 32-byte signed.</returns>
        Private Shared Function Egcd32(ByVal x As Byte(), ByVal y As Byte(), ByVal a As Byte(), ByVal b As Byte()) As Byte()
            Dim bn As Integer = 32
            Dim i As Integer

            For i = 0 To 32 - 1
                x(i) = y(i)
                y(i) = 0
            Next

            x(0) = 1
            Dim an As Integer = GetNumSize(a, 32)
            If an = 0 Then Return y ' division by zero 
            Dim temp = New Byte(31) {}

            While True
                Dim qn As Integer = bn - an + 1
                DivMod(temp, b, bn, a, an)
                bn = GetNumSize(b, bn)
                If bn = 0 Then Return x
                MultiplyArray32(y, x, temp, qn, -1)
                qn = an - bn + 1
                DivMod(temp, a, an, b, bn)
                an = GetNumSize(a, an)
                If an = 0 Then Return y
                MultiplyArray32(x, y, temp, qn, -1)
            End While
        End Function


        ' ******************* radix 2^25.5 GF(2^255-19) math ********************

        Private Const P25 As Integer = 33554431 ' (1 << 25) - 1 
        Private Const P26 As Integer = 67108863 ' (1 << 26) - 1 

        ' Convert to internal format from little-endian byte format 

        Private Shared Sub Unpack(ByVal x As Long10, ByVal m As Byte())
            x.N0 = m(0) And &HFF Or (m(1) And &HFF) << 8 Or (m(2) And &HFF) << 16 Or (m(3) And &HFF And 3) << 24
            x.N1 = (m(3) And &HFF And Not 3) >> 2 Or (m(4) And &HFF) << 6 Or (m(5) And &HFF) << 14 Or (m(6) And &HFF And 7) << 22
            x.N2 = (m(6) And &HFF And Not 7) >> 3 Or (m(7) And &HFF) << 5 Or (m(8) And &HFF) << 13 Or (m(9) And &HFF And 31) << 21
            x.N3 = (m(9) And &HFF And Not 31) >> 5 Or (m(10) And &HFF) << 3 Or (m(11) And &HFF) << 11 Or (m(12) And &HFF And 63) << 19
            x.N4 = (m(12) And &HFF And Not 63) >> 6 Or (m(13) And &HFF) << 2 Or (m(14) And &HFF) << 10 Or (m(15) And &HFF) << 18
            x.N5 = m(16) And &HFF Or (m(17) And &HFF) << 8 Or (m(18) And &HFF) << 16 Or (m(19) And &HFF And 1) << 24
            x.N6 = (m(19) And &HFF And Not 1) >> 1 Or (m(20) And &HFF) << 7 Or (m(21) And &HFF) << 15 Or (m(22) And &HFF And 7) << 23
            x.N7 = (m(22) And &HFF And Not 7) >> 3 Or (m(23) And &HFF) << 5 Or (m(24) And &HFF) << 13 Or (m(25) And &HFF And 15) << 21
            x.N8 = (m(25) And &HFF And Not 15) >> 4 Or (m(26) And &HFF) << 4 Or (m(27) And &HFF) << 12 Or (m(28) And &HFF And 63) << 20
            x.N9 = (m(28) And &HFF And Not 63) >> 6 Or (m(29) And &HFF) << 2 Or (m(30) And &HFF) << 10 Or (m(31) And &HFF) << 18
        End Sub


        ''' <summary>
        ''' Check if reduced-form input >= 2^255-19
        ''' </summary>
        Private Shared Function IsOverflow(ByVal x As Long10) As Boolean
            Return x.N0 > P26 - 19 And (x.N1 And x.N3 And x.N5 And x.N7 And x.N9) = P25 And (x.N2 And x.N4 And x.N6 And x.N8) = P26 OrElse x.N9 > P25
        End Function


        ' Convert from internal format to little-endian byte format.  The 
        '  number must be in a reduced form which is output by the following ops:
        '      unpack, mul, sqr
        '      set --  if input in range 0 .. P25
        '  If you're unsure if the number is reduced, first multiply it by 1.  

        Private Shared Sub Pack(ByVal x As Long10, ByVal m As Byte())
            Dim ld As Integer = If(IsOverflow(x), 1, 0) - If(x.N9 < 0, 1, 0)
            Dim ud As Integer = ld * -(P25 + 1)
            ld *= 19
            Dim t As Long = ld + x.N0 + (x.N1 << 26)

            Dim test = BitConverter.GetBytes(t)

            m(0) = test(0) ' CByte(t)
            m(1) = test(1) ' CByte(t >> 8)
            m(2) = test(2) ' CByte(t >> 16)
            m(3) = test(3) ' CByte(t >> 24)
            t = (t >> 32) + (x.N2 << 19)
            test = BitConverter.GetBytes(t)

            m(4) = test(0) ' CByte(t)
            m(5) = test(1) ' CByte(t >> 8)
            m(6) = test(2) ' CByte(t >> 16)
            m(7) = test(3) ' CByte(t >> 24)
            t = (t >> 32) + (x.N3 << 13)
            test = BitConverter.GetBytes(t)

            m(8) = test(0) ' CByte(t)
            m(9) = test(1) ' CByte(t >> 8)
            m(10) = test(2) ' CByte(t >> 16)
            m(11) = test(3) ' CByte(t >> 24)
            t = (t >> 32) + (x.N4 << 6)
            test = BitConverter.GetBytes(t)

            m(12) = test(0) ' CByte(t)
            m(13) = test(1) ' CByte(t >> 8)
            m(14) = test(2) ' CByte(t >> 16)
            m(15) = test(3) ' CByte(t >> 24)
            t = (t >> 32) + x.N5 + (x.N6 << 25)
            test = BitConverter.GetBytes(t)

            m(16) = test(0) ' CByte(t)
            m(17) = test(1) ' CByte(t >> 8)
            m(18) = test(2) ' CByte(t >> 16)
            m(19) = test(3) ' CByte(t >> 24)
            t = (t >> 32) + (x.N7 << 19)
            test = BitConverter.GetBytes(t)

            m(20) = test(0) ' CByte(t)
            m(21) = test(1) ' CByte(t >> 8)
            m(22) = test(2) ' CByte(t >> 16)
            m(23) = test(3) ' CByte(t >> 24)
            t = (t >> 32) + (x.N8 << 12)
            test = BitConverter.GetBytes(t)

            m(24) = test(0) ' CByte(t)
            m(25) = test(1) ' CByte(t >> 8)
            m(26) = test(2) ' CByte(t >> 16)
            m(27) = test(3) ' CByte(t >> 24)
            t = (t >> 32) + (x.N9 + ud << 6)
            test = BitConverter.GetBytes(t)

            m(28) = test(0) ' CByte(t)
            m(29) = test(1) ' CByte(t >> 8)
            m(30) = test(2) ' CByte(t >> 16)
            m(31) = test(3) ' CByte(t >> 24)
        End Sub


        ''' <summary>
        ''' Copy a number
        ''' </summary>
        Private Shared Sub Copy(ByVal numOut As Long10, ByVal numIn As Long10)
            numOut.N0 = numIn.N0
            numOut.N1 = numIn.N1
            numOut.N2 = numIn.N2
            numOut.N3 = numIn.N3
            numOut.N4 = numIn.N4
            numOut.N5 = numIn.N5
            numOut.N6 = numIn.N6
            numOut.N7 = numIn.N7
            numOut.N8 = numIn.N8
            numOut.N9 = numIn.N9
        End Sub


        ''' <summary>
        ''' Set a number to value, which must be in range -185861411 .. 185861411
        ''' </summary>
        Private Shared Sub [Set](ByVal numOut As Long10, ByVal numIn As Integer)
            numOut.N0 = numIn
            numOut.N1 = 0
            numOut.N2 = 0
            numOut.N3 = 0
            numOut.N4 = 0
            numOut.N5 = 0
            numOut.N6 = 0
            numOut.N7 = 0
            numOut.N8 = 0
            numOut.N9 = 0
        End Sub


        ' Add/subtract two numbers.  The inputs must be in reduced form, and the 
        '  output isn't, so to do another addition or subtraction on the output, 
        '  first multiply it by one to reduce it. 
        Private Shared Sub Add(ByVal xy As Long10, ByVal x As Long10, ByVal y As Long10)
            xy.N0 = x.N0 + y.N0
            xy.N1 = x.N1 + y.N1
            xy.N2 = x.N2 + y.N2
            xy.N3 = x.N3 + y.N3
            xy.N4 = x.N4 + y.N4
            xy.N5 = x.N5 + y.N5
            xy.N6 = x.N6 + y.N6
            xy.N7 = x.N7 + y.N7
            xy.N8 = x.N8 + y.N8
            xy.N9 = x.N9 + y.N9
        End Sub

        Private Shared Sub [Sub](ByVal xy As Long10, ByVal x As Long10, ByVal y As Long10)
            xy.N0 = x.N0 - y.N0
            xy.N1 = x.N1 - y.N1
            xy.N2 = x.N2 - y.N2
            xy.N3 = x.N3 - y.N3
            xy.N4 = x.N4 - y.N4
            xy.N5 = x.N5 - y.N5
            xy.N6 = x.N6 - y.N6
            xy.N7 = x.N7 - y.N7
            xy.N8 = x.N8 - y.N8
            xy.N9 = x.N9 - y.N9
        End Sub


        ''' <summary>
        ''' Multiply a number by a small integer in range -185861411 .. 185861411.
        ''' The output is in reduced form, the input x need not be.  x and xy may point
        ''' to the same buffer.
        ''' </summary>
        Private Shared Sub MulSmall(ByVal xy As Long10, ByVal x As Long10, ByVal y As Long)
            Dim temp As Long = x.N8 * y
            xy.N8 = temp And (1 << 26) - 1
            temp = (temp >> 26) + x.N9 * y
            xy.N9 = temp And (1 << 25) - 1
            temp = 19 * (temp >> 25) + x.N0 * y
            xy.N0 = temp And (1 << 26) - 1
            temp = (temp >> 26) + x.N1 * y
            xy.N1 = temp And (1 << 25) - 1
            temp = (temp >> 25) + x.N2 * y
            xy.N2 = temp And (1 << 26) - 1
            temp = (temp >> 26) + x.N3 * y
            xy.N3 = temp And (1 << 25) - 1
            temp = (temp >> 25) + x.N4 * y
            xy.N4 = temp And (1 << 26) - 1
            temp = (temp >> 26) + x.N5 * y
            xy.N5 = temp And (1 << 25) - 1
            temp = (temp >> 25) + x.N6 * y
            xy.N6 = temp And (1 << 26) - 1
            temp = (temp >> 26) + x.N7 * y
            xy.N7 = temp And (1 << 25) - 1
            temp = (temp >> 25) + xy.N8
            xy.N8 = temp And (1 << 26) - 1
            xy.N9 += temp >> 26
        End Sub


        ''' <summary>
        ''' Multiply two numbers. The output is in reduced form, the inputs need not be.
        ''' </summary>
        Private Shared Sub Multiply(ByVal xy As Long10, ByVal x As Long10, ByVal y As Long10)
            ' sahn0:
            '  Using local variables to avoid class access.
            '  This seem to improve performance a bit...
            ' 
            Dim x0 As Long = x.N0, x1 As Long = x.N1, x2 As Long = x.N2, x3 As Long = x.N3, x4 As Long = x.N4, x5 As Long = x.N5, x6 As Long = x.N6, x7 As Long = x.N7, x8 As Long = x.N8, x9 As Long = x.N9
            Dim y0 As Long = y.N0, y1 As Long = y.N1, y2 As Long = y.N2, y3 As Long = y.N3, y4 As Long = y.N4, y5 As Long = y.N5, y6 As Long = y.N6, y7 As Long = y.N7, y8 As Long = y.N8, y9 As Long = y.N9
            Dim t As Long = x0 * y8 + x2 * y6 + x4 * y4 + x6 * y2 + x8 * y0 + 2 * (x1 * y7 + x3 * y5 + x5 * y3 + x7 * y1) + 38 * (x9 * y9)
            xy.N8 = t And (1 << 26) - 1
            t = (t >> 26) + x0 * y9 + x1 * y8 + x2 * y7 + x3 * y6 + x4 * y5 + x5 * y4 + x6 * y3 + x7 * y2 + x8 * y1 + x9 * y0
            xy.N9 = t And (1 << 25) - 1
            t = x0 * y0 + 19 * ((t >> 25) + x2 * y8 + x4 * y6 + x6 * y4 + x8 * y2) + 38 * (x1 * y9 + x3 * y7 + x5 * y5 + x7 * y3 + x9 * y1)
            xy.N0 = t And (1 << 26) - 1
            t = (t >> 26) + x0 * y1 + x1 * y0 + 19 * (x2 * y9 + x3 * y8 + x4 * y7 + x5 * y6 + x6 * y5 + x7 * y4 + x8 * y3 + x9 * y2)
            xy.N1 = t And (1 << 25) - 1
            t = (t >> 25) + x0 * y2 + x2 * y0 + 19 * (x4 * y8 + x6 * y6 + x8 * y4) + 2 * (x1 * y1) + 38 * (x3 * y9 + x5 * y7 + x7 * y5 + x9 * y3)
            xy.N2 = t And (1 << 26) - 1
            t = (t >> 26) + x0 * y3 + x1 * y2 + x2 * y1 + x3 * y0 + 19 * (x4 * y9 + x5 * y8 + x6 * y7 + x7 * y6 + x8 * y5 + x9 * y4)
            xy.N3 = t And (1 << 25) - 1
            t = (t >> 25) + x0 * y4 + x2 * y2 + x4 * y0 + 19 * (x6 * y8 + x8 * y6) + 2 * (x1 * y3 + x3 * y1) + 38 * (x5 * y9 + x7 * y7 + x9 * y5)
            xy.N4 = t And (1 << 26) - 1
            t = (t >> 26) + x0 * y5 + x1 * y4 + x2 * y3 + x3 * y2 + x4 * y1 + x5 * y0 + 19 * (x6 * y9 + x7 * y8 + x8 * y7 + x9 * y6)
            xy.N5 = t And (1 << 25) - 1
            t = (t >> 25) + x0 * y6 + x2 * y4 + x4 * y2 + x6 * y0 + 19 * (x8 * y8) + 2 * (x1 * y5 + x3 * y3 + x5 * y1) + 38 * (x7 * y9 + x9 * y7)
            xy.N6 = t And (1 << 26) - 1
            t = (t >> 26) + x0 * y7 + x1 * y6 + x2 * y5 + x3 * y4 + x4 * y3 + x5 * y2 + x6 * y1 + x7 * y0 + 19 * (x8 * y9 + x9 * y8)
            xy.N7 = t And (1 << 25) - 1
            t = (t >> 25) + xy.N8
            xy.N8 = t And (1 << 26) - 1
            xy.N9 += t >> 26
        End Sub


        ''' <summary>
        ''' Square a number.  Optimization of  Multiply(x2, x, x)
        ''' </summary>
        Private Shared Sub Square(ByVal xsqr As Long10, ByVal x As Long10)
            Dim x0 As Long = x.N0, x1 As Long = x.N1, x2 As Long = x.N2, x3 As Long = x.N3, x4 As Long = x.N4, x5 As Long = x.N5, x6 As Long = x.N6, x7 As Long = x.N7, x8 As Long = x.N8, x9 As Long = x.N9
            Dim t As Long = x4 * x4 + 2 * (x0 * x8 + x2 * x6) + 38 * (x9 * x9) + 4 * (x1 * x7 + x3 * x5)
            xsqr.N8 = t And (1 << 26) - 1
            t = (t >> 26) + 2 * (x0 * x9 + x1 * x8 + x2 * x7 + x3 * x6 + x4 * x5)
            xsqr.N9 = t And (1 << 25) - 1
            t = 19 * (t >> 25) + x0 * x0 + 38 * (x2 * x8 + x4 * x6 + x5 * x5) + 76 * (x1 * x9 + x3 * x7)
            xsqr.N0 = t And (1 << 26) - 1
            t = (t >> 26) + 2 * (x0 * x1) + 38 * (x2 * x9 + x3 * x8 + x4 * x7 + x5 * x6)
            xsqr.N1 = t And (1 << 25) - 1
            t = (t >> 25) + 19 * (x6 * x6) + 2 * (x0 * x2 + x1 * x1) + 38 * (x4 * x8) + 76 * (x3 * x9 + x5 * x7)
            xsqr.N2 = t And (1 << 26) - 1
            t = (t >> 26) + 2 * (x0 * x3 + x1 * x2) + 38 * (x4 * x9 + x5 * x8 + x6 * x7)
            xsqr.N3 = t And (1 << 25) - 1
            t = (t >> 25) + x2 * x2 + 2 * (x0 * x4) + 38 * (x6 * x8 + x7 * x7) + 4 * (x1 * x3) + 76 * (x5 * x9)
            xsqr.N4 = t And (1 << 26) - 1
            t = (t >> 26) + 2 * (x0 * x5 + x1 * x4 + x2 * x3) + 38 * (x6 * x9 + x7 * x8)
            xsqr.N5 = t And (1 << 25) - 1
            t = (t >> 25) + 19 * (x8 * x8) + 2 * (x0 * x6 + x2 * x4 + x3 * x3) + 4 * (x1 * x5) + 76 * (x7 * x9)
            xsqr.N6 = t And (1 << 26) - 1
            t = (t >> 26) + 2 * (x0 * x7 + x1 * x6 + x2 * x5 + x3 * x4) + 38 * (x8 * x9)
            xsqr.N7 = t And (1 << 25) - 1
            t = (t >> 25) + xsqr.N8
            xsqr.N8 = t And (1 << 26) - 1
            xsqr.N9 += t >> 26
        End Sub


        ''' <summary>
        ''' Calculates a reciprocal.  The output is in reduced form, the inputs need not 
        ''' be.  Simply calculates  y = x^(p-2)  so it's not too fast. */
        ''' When sqrtassist is true, it instead calculates y = x^((p-5)/8)
        ''' </summary>
        Private Shared Sub Reciprocal(ByVal y As Long10, ByVal x As Long10, ByVal sqrtAssist As Boolean)
            Dim t0 As Long10 = New Long10(), t1 As Long10 = New Long10(), t2 As Long10 = New Long10(), t3 As Long10 = New Long10(), t4 As Long10 = New Long10()
            Dim i As Integer
            ' the chain for x^(2^255-21) is straight from djb's implementation 
            Square(t1, x) ' 2 == 2 * 1	
            Square(t2, t1) ' 4 == 2 * 2	
            Square(t0, t2) ' 8 == 2 * 4	
            Multiply(t2, t0, x) ' 9 == 8 + 1	
            Multiply(t0, t2, t1) ' 11 == 9 + 2	
            Square(t1, t0) ' 22 == 2 * 11	
            Multiply(t3, t1, t2) ' 31 == 22 + 9
            ' 					== 2^5   - 2^0	
            Square(t1, t3) ' 2^6   - 2^1	
            Square(t2, t1) ' 2^7   - 2^2	
            Square(t1, t2) ' 2^8   - 2^3	
            Square(t2, t1) ' 2^9   - 2^4	
            Square(t1, t2) ' 2^10  - 2^5	
            Multiply(t2, t1, t3) ' 2^10  - 2^0	
            Square(t1, t2) ' 2^11  - 2^1	
            Square(t3, t1) ' 2^12  - 2^2	
            For i = 1 To 5 - 1
                Square(t1, t3)
                Square(t3, t1)
            Next ' t3 
            ' 2^20  - 2^10	
            Multiply(t1, t3, t2) ' 2^20  - 2^0	
            Square(t3, t1) ' 2^21  - 2^1	
            Square(t4, t3) ' 2^22  - 2^2	
            For i = 1 To 10 - 1
                Square(t3, t4)
                Square(t4, t3)
            Next ' t4 
            ' 2^40  - 2^20	
            Multiply(t3, t4, t1) ' 2^40  - 2^0	
            For i = 0 To 5 - 1
                Square(t1, t3)
                Square(t3, t1)
            Next ' t3 
            ' 2^50  - 2^10	
            Multiply(t1, t3, t2) ' 2^50  - 2^0	
            Square(t2, t1) ' 2^51  - 2^1	
            Square(t3, t2) ' 2^52  - 2^2	
            For i = 1 To 25 - 1
                Square(t2, t3)
                Square(t3, t2)
            Next ' t3 
            ' 2^100 - 2^50 
            Multiply(t2, t3, t1) ' 2^100 - 2^0	
            Square(t3, t2) ' 2^101 - 2^1	
            Square(t4, t3) ' 2^102 - 2^2	
            For i = 1 To 50 - 1
                Square(t3, t4)
                Square(t4, t3)
            Next ' t4 
            ' 2^200 - 2^100 
            Multiply(t3, t4, t2) ' 2^200 - 2^0	
            For i = 0 To 25 - 1
                Square(t4, t3)
                Square(t3, t4)
            Next ' t3 
            ' 2^250 - 2^50	
            Multiply(t2, t3, t1) ' 2^250 - 2^0	
            Square(t1, t2) ' 2^251 - 2^1	
            Square(t2, t1) ' 2^252 - 2^2	
            If sqrtAssist Then
                Multiply(y, x, t2) ' 2^252 - 3 
            Else
                Square(t1, t2) ' 2^253 - 2^3	
                Square(t2, t1) ' 2^254 - 2^4	
                Square(t1, t2) ' 2^255 - 2^5	
                Multiply(y, t1, t0) ' 2^255 - 21	
            End If
        End Sub


        ''' <summary>
        ''' Checks if x is "negative", requires reduced input
        ''' </summary>
        ''' <param name="x">must be reduced input</param>
        Private Shared Function IsNegative(ByVal x As Long10) As Integer
            Return If(IsOverflow(x) Or x.N9 < 0, 1, 0) Xor x.N0 And 1
        End Function


        ' ******************* Elliptic curve ********************

        ' y^2 = x^3 + 486662 x^2 + x  over GF(2^255-19) 

        ' t1 = ax + az
        '  t2 = ax - az  

        Private Shared Sub MontyPrepare(ByVal t1 As Long10, ByVal t2 As Long10, ByVal ax As Long10, ByVal az As Long10)
            Add(t1, ax, az)
            [Sub](t2, ax, az)
        End Sub


        ' A = P + Q   where
        '   X(A) = ax/az
        '   X(P) = (t1+t2)/(t1-t2)
        '   X(Q) = (t3+t4)/(t3-t4)
        '   X(P-Q) = dx
        '  clobbers t1 and t2, preserves t3 and t4  

        Private Shared Sub MontyAdd(ByVal t1 As Long10, ByVal t2 As Long10, ByVal t3 As Long10, ByVal t4 As Long10, ByVal ax As Long10, ByVal az As Long10, ByVal dx As Long10)
            Multiply(ax, t2, t3)
            Multiply(az, t1, t4)
            Add(t1, ax, az)
            [Sub](t2, ax, az)
            Square(ax, t1)
            Square(t1, t2)
            Multiply(az, t1, dx)
        End Sub


        ' B = 2 * Q   where
        '   X(B) = bx/bz
        '   X(Q) = (t3+t4)/(t3-t4)
        '  clobbers t1 and t2, preserves t3 and t4  

        Private Shared Sub MontyDouble(ByVal t1 As Long10, ByVal t2 As Long10, ByVal t3 As Long10, ByVal t4 As Long10, ByVal bx As Long10, ByVal bz As Long10)
            Square(t1, t3)
            Square(t2, t4)
            Multiply(bx, t1, t2)
            [Sub](t2, t1, t2)
            MulSmall(bz, t2, 121665)
            Add(t1, t1, bz)
            Multiply(bz, t1, t2)
        End Sub


        ''' <summary>
        ''' Y^2 = X^3 + 486662 X^2 + X
        ''' </summary>
        ''' <param name="y2">output</param>
        ''' <param name="x">X</param>
        ''' <param name="temp">temporary</param>
        Private Shared Sub CurveEquationInline(ByVal y2 As Long10, ByVal x As Long10, ByVal temp As Long10)
            Square(temp, x)
            MulSmall(y2, x, 486662)
            Add(temp, temp, y2)
            temp.N0 += 1
            Multiply(y2, temp, x)
        End Sub


        ''' <summary>
        ''' P = kG   and  s = sign(P)/k
        ''' </summary>
        Private Shared Sub Core(ByVal publicKey As Byte(), ByVal signingKey As Byte(), ByVal privateKey As Byte(), ByVal peerPublicKey As Byte())
            If publicKey Is Nothing Then Throw New ArgumentNullException("publicKey")
            If publicKey.Length <> 32 Then Throw New ArgumentException(String.Format("publicKey must be 32 bytes long (but was {0} bytes long)", publicKey.Length), "publicKey")
            If signingKey IsNot Nothing AndAlso signingKey.Length <> 32 Then Throw New ArgumentException(String.Format("signingKey must be null or 32 bytes long (but was {0} bytes long)", signingKey.Length), "signingKey")
            If privateKey Is Nothing Then Throw New ArgumentNullException("privateKey")
            If privateKey.Length <> 32 Then Throw New ArgumentException(String.Format("privateKey must be 32 bytes long (but was {0} bytes long)", privateKey.Length), "privateKey")
            If peerPublicKey IsNot Nothing AndAlso peerPublicKey.Length <> 32 Then Throw New ArgumentException(String.Format("peerPublicKey must be null or 32 bytes long (but was {0} bytes long)", peerPublicKey.Length), "peerPublicKey")
            Dim dx As Long10 = New Long10(), t1 As Long10 = New Long10(), t2 As Long10 = New Long10(), t3 As Long10 = New Long10(), t4 As Long10 = New Long10()
            Dim x As Long10() = {New Long10(), New Long10()}, z As Long10() = {New Long10(), New Long10()}


            ' unpack the base 
            If peerPublicKey IsNot Nothing Then
                Unpack(dx, peerPublicKey)
            Else
                [Set](dx, 9)
            End If


            ' 0G = point-at-infinity 
            [Set](x(0), 1)
            [Set](z(0), 0)

            ' 1G = G 
            Copy(x(1), dx)
            [Set](z(1), 1)
            Dim i As Integer = 32

            While Math.Max(Threading.Interlocked.Decrement(i), i + 1) <> 0
                Dim j As Integer = 8

                While Math.Max(Threading.Interlocked.Decrement(j), j + 1) <> 0
                    ' swap arguments depending on bit 
                    Dim bit1 As Integer = (privateKey(i) And &HFF) >> j And 1
                    Dim bit0 As Integer = Not (privateKey(i) And &HFF) >> j And 1
                    Dim ax As Long10 = x(bit0)
                    Dim az As Long10 = z(bit0)
                    Dim bx As Long10 = x(bit1)
                    Dim bz As Long10 = z(bit1)

                    ' a' = a + b	
                    ' b' = 2 b	
                    MontyPrepare(t1, t2, ax, az)
                    MontyPrepare(t3, t4, bx, bz)
                    MontyAdd(t1, t2, t3, t4, ax, az, dx)
                    MontyDouble(t1, t2, t3, t4, bx, bz)
                End While
            End While

            Reciprocal(t1, z(0), False)
            Multiply(dx, x(0), t1)
            Pack(dx, publicKey)


            ' calculate s such that s abs(P) = G  .. assumes G is std base point 
            If signingKey IsNot Nothing Then
                CurveEquationInline(t1, dx, t2) ' t1 = Py^2  
                Reciprocal(t3, z(1), False) ' where Q=P+G ... 
                Multiply(t2, x(1), t3) ' t2 = Qx  
                Add(t2, t2, dx) ' t2 = Qx + Px  
                t2.N0 += 9 + 486662 ' t2 = Qx + Px + Gx + 486662  
                dx.N0 -= 9 ' dx = Px - Gx  
                Square(t3, dx) ' t3 = (Px - Gx)^2  
                Multiply(dx, t2, t3) ' dx = t2 (Px - Gx)^2  
                [Sub](dx, dx, t1) ' dx = t2 (Px - Gx)^2 - Py^2  
                dx.N0 -= 39420360 ' dx = t2 (Px - Gx)^2 - Py^2 - Gy^2  
                Multiply(t1, dx, BaseR2Y) ' t1 = -Py  
                If IsNegative(t1) <> 0 Then ' sign is 1, so just copy  
                    Copy32(privateKey, signingKey) ' sign is -1, so negate  
                Else
                    MultiplyArraySmall(signingKey, OrderTimes8, 0, privateKey, 32, -1)
                End If


                ' reduce s mod q
                '  (is this needed?  do it just in case, it's fast anyway) 
                'divmod((dstptr) t1, s, 32, order25519, 32);

                ' take reciprocal of s mod q 
                Dim temp1 = New Byte(31) {}
                Dim temp2 = New Byte(63) {}
                Dim temp3 = New Byte(63) {}
                Copy32(Order, temp1)
                Copy32(Egcd32(temp2, temp3, signingKey, temp1), signingKey)
                If (signingKey(31) And &H80) <> 0 Then MultiplyArraySmall(signingKey, signingKey, 0, Order, 32, 1)
            End If
        End Sub


        ''' <summary>
        ''' Smallest multiple of the order that's >= 2^255
        ''' </summary>
        Private Shared ReadOnly OrderTimes8 As Byte() = {104, 159, 174, 231, 210, 24, 147, 192, 178, 230, 188, 23, 245, 206, 247, 166, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 128}

        ''' <summary>
        ''' Constant 1/(2Gy)
        ''' </summary>
        Private Shared ReadOnly BaseR2Y As Long10 = New Long10(5744, 8160848, 4790893, 13779497, 35730846, 12541209, 49101323, 30047407, 40071253, 6226132)

    End Class
End Namespace

