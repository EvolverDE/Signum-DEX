
' Ported from C to Java by Dmitry Skiba [sahn0], 23/02/08.
'  Original: http://cds.xs4all.nl:8081/ecdh/
' 
' Generic 64-bit  Integer  implementation of Curve25519 ECDH
'  Written by Matthijs van Duin, 200608242056
'  Public domain.
' 
'  Based on work by Daniel J Bernstein, http://cr.yp.to/ecdh.html
' 

Option Strict On
Option Explicit On

Imports System
Imports System.Threading

Public Class ClsCurve25519

    'public constants
    Public Const C1 As Integer = 1
    Public Const C9 As Integer = 9
    Public Const C486662 As Integer = 486662
    Public Const C39420360 As Integer = 39420360

    ' smallest multiple of the order that's >= 2^255 
    ReadOnly Property ORDER_TIMES_8 As Byte() = {104, 159, 174, 231, 210, 24, 147, 192, 178, 230, 188, 23, 245, 206, 247, 166, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 128}
    ' constants 2Gy and 1/(2Gy) 
    ReadOnly Property BASE_2Y As Long10 = New Long10(39999547, 18689728, 59995525, 1648697, 57546132, 24010086, 19059592, 5425144, 63499247, 16420658)
    ReadOnly Property BASE_R2Y As Long10 = New Long10(5744, 8160848, 4790893, 13779497, 35730846, 12541209, 49101323, 30047407, 40071253, 6226132)


    ' key size 
    ReadOnly Property KEY_SIZE As Integer = 32
    ' 0 
    ReadOnly Property ZERO As Byte() = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    ' the prime 2^255-19 
    ReadOnly Property PRIME As Byte() = {237, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 127}
    ' group order (a prime near 2^252+2^124) 
    ReadOnly Property ORDER As Byte() = {237, 211, 245, 92, 26, 99, 18, 88, 214, 156, 247, 162, 222, 249, 222, 20, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16}


#Region " ******* KEY AGREEMENT ********"

    ''' <summary>
    ''' Private key clamping
    ''' </summary>
    ''' <param name="AgreementKey">[in] 32 random bytes, [out] your private key for key agreement</param>
    Sub Clamp(ByRef AgreementKey As Byte())
        AgreementKey(31) = Convert.ToByte(AgreementKey(31) And &H7F)
        AgreementKey(31) = Convert.ToByte(AgreementKey(31) Or &H40)
        AgreementKey(0) = Convert.ToByte(AgreementKey(0) And &HF8)
    End Sub

    ''' <summary>
    ''' Key-Pair generation * WARNING: if s is not NULL, this function has data-dependent timing 
    ''' </summary>
    ''' <param name="PublicKey">[out] your public key</param>
    ''' <param name="SignKey">[out] your private key for signing</param>
    ''' <param name="AgreementKey">[in] 32 random bytes, [out] your private key for key agreement</param>
    Sub KeyGen(ByRef PublicKey As Byte(), ByRef SignKey As Byte(), ByRef AgreementKey As Byte())
        Clamp(AgreementKey)
        Core(PublicKey, SignKey, AgreementKey, Nothing)
    End Sub

    ''' <summary>
    ''' Key agreement
    ''' </summary>
    ''' <param name="SharedSecret">[out] shared secret (needs hashing before use)</param>
    ''' <param name="AgreementKey">[in]  your private key for key agreement</param>
    ''' <param name="PublicKey">[in] peer's public key</param>
    Sub GetSharedSecret(ByRef SharedSecret As Byte(), ByVal AgreementKey As Byte(), ByVal PublicKey As Byte())
        Core(SharedSecret, Nothing, AgreementKey, PublicKey)
    End Sub

#End Region

    ' sahn0:
    ''' <summary>
    ''' Using this class instead of long[10] to avoid bounds checks. 
    ''' </summary>
    Public Class Long10
        Public Sub New()
        End Sub
        Public Sub New(_0 As Long, _1 As Long, _2 As Long, _3 As Long, _4 As Long, _5 As Long, _6 As Long, _7 As Long, _8 As Long, _9 As Long)
            Me._0 = _0 : Me._1 = _1 : Me._2 = _2 : Me._3 = _3 : Me._4 = _4
            Me._5 = _5 : Me._6 = _6 : Me._7 = _7 : Me._8 = _8 : Me._9 = _9
        End Sub
        Public _0, _1, _2, _3, _4, _5, _6, _7, _8, _9 As Long
    End Class

    ' ******************* radix 2^8 math ********************
    ''' <summary>
    ''' Copies 32 Bytes from Input to Output
    ''' </summary>
    ''' <param name="Output">[out] Copy of Input</param>
    ''' <param name="Input">[in] Input to Copy to Output</param>
    Sub Copy32(ByRef Output As Byte(), ByVal Input As Byte())
        Dim i As Integer
        For i = 0 To 32 - 1
            Output(i) = Input(i)
        Next
    End Sub


    ' p[m..n+m-1] = q[m..n+m-1] + z * x 
    ' n is the size of x 
    ' n+m is the size of p and q 
    Function Multiply_Array_Small(ByVal p As Byte(), ByVal q As Byte(), ByVal m As Integer, ByVal x As Byte(), ByVal n As Integer, ByVal z As Integer) As Integer
        Dim v As Integer = 0, i As Integer = 0
        While i < n
            v += (q(i + m) And &HFF) + z * (x(i) And &HFF)
            p(i + m) = Convert.ToByte(v And &HFF)
            v >>= 8
            i += 1
        End While
        Return v
    End Function

    ' p += x * y * z  where z is a small  Integer 
    ' 	 * x is size 32, y is size t, p is size 32+t
    ' 	 * y is allowed to overlap with p+32 if you don't care about the upper half  
    Function Multiply_Array32(ByVal p As Byte(), ByVal x As Byte(), ByVal y As Byte(), ByVal t As Integer, ByVal z As Integer) As Integer
        Dim n As Integer = 31
        Dim w As Integer = 0
        Dim i As Integer = 0
        While i < t
            Dim zy As Integer = z * (y(i) And &HFF)
            w += Multiply_Array_Small(p, p, i, x, n, zy) + (p(i + n) And &HFF) + zy * (x(n) And &HFF)
            p(i + n) = Convert.ToByte(w And &HFF)
            w >>= 8
            i += 1
        End While
        p(i + n) = Convert.ToByte((w + (p(i + n) And &HFF)) And &HFF)
        Return w >> 8
    End Function
    ' divide r (size n) by d (size t), returning quotient q and remainder r
    ' 	 * quotient is size n-t+1, remainder is size t
    ' 	 * requires t > 0 && d[t-1] != 0
    ' 	 * requires that r[-1] and d[-1] are valid memory locations
    ' 	 * q may overlap with r+t 
    Sub DivideMod(ByRef q As Byte(), ByRef r As Byte(), ByVal n As Integer, ByVal d As Byte(), ByVal t As Integer)
        Dim rn As Integer = 0
        Dim dt As Integer = (d(t - 1) And &HFF) << 8
        If t > 1 Then
            dt = dt Or d(t - 2) And &HFF
        End If
        While Math.Max(Interlocked.Decrement(n), n + 1) >= t
            Dim z As Integer = rn << 16 Or (r(n) And &HFF) << 8
            If n > 0 Then
                z = z Or r(n - 1) And &HFF
            End If
            z \= dt
            rn += Multiply_Array_Small(r, r, n - t + 1, d, t, -z)
            q(n - t + 1) = Convert.ToByte(z + rn And &HFF) ' rn is 0 or -1 (underflow) 
            Multiply_Array_Small(r, r, n - t + 1, d, t, -rn)
            rn = r(n) And &HFF
            r(n) = 0
        End While
        r(t - 1) = Convert.ToByte(rn)
    End Sub
    Function NumberSize(ByVal x As Byte(), ByVal n As Integer) As Integer
        While Math.Max(Interlocked.Decrement(n), n + 1) <> 0 AndAlso x(n) = 0
        End While
        Return n + 1
    End Function
    ' Returns x if a contains the gcd, y if b.
    ' 	 * Also, the returned buffer contains the inverse of a mod b,
    ' 	 * as 32-byte signed.
    ' 	 * x and y must have 64 bytes space for temporary use.
    ' 	 * requires that a[-1] and b[-1] are valid memory locations  
    Function Egcd32(ByRef x As Byte(), ByVal y As Byte(), ByVal a As Byte(), ByVal b As Byte()) As Byte()
        Dim an, qn, i As Integer, bn As Integer = 32
        For i = 0 To 32 - 1
            y(i) = 0 : x(i) = 0
        Next
        x(0) = 1
        an = NumberSize(a, 32)
        If an = 0 Then Return y ' division by zero 
        Dim temp As Byte() = New Byte(31) {}
        While True
            qn = bn - an + 1
            DivideMod(temp, b, bn, a, an)
            bn = NumberSize(b, bn)
            If bn = 0 Then Return x
            Multiply_Array32(y, x, temp, qn, -1)
            qn = an - bn + 1
            DivideMod(temp, a, an, b, bn)
            an = NumberSize(a, an)
            If an = 0 Then Return y
            Multiply_Array32(x, y, temp, qn, -1)
        End While
        Throw New Exception()
    End Function


    ' ******************* radix 2^25.5 GF(2^255-19) math ********************
    ReadOnly Property P25 As Integer = 33554431  ' (1 << 25) - 1 
    ReadOnly Property P26 As Integer = 67108863  ' (1 << 26) - 1 
    ' Convert to internal format from little-endian byte format 
    Sub Unpack(ByRef x As Long10, ByVal m As Byte())
        x._0 = m(0) And &HFF Or (m(1) And &HFF) << 8 Or (m(2) And &HFF) << 16 Or (m(3) And &HFF And 3) << 24
        x._1 = (m(3) And &HFF And Not 3) >> 2 Or (m(4) And &HFF) << 6 Or (m(5) And &HFF) << 14 Or (m(6) And &HFF And 7) << 22
        x._2 = (m(6) And &HFF And Not 7) >> 3 Or (m(7) And &HFF) << 5 Or (m(8) And &HFF) << 13 Or (m(9) And &HFF And 31) << 21
        x._3 = (m(9) And &HFF And Not 31) >> 5 Or (m(10) And &HFF) << 3 Or (m(11) And &HFF) << 11 Or (m(12) And &HFF And 63) << 19
        x._4 = (m(12) And &HFF And Not 63) >> 6 Or (m(13) And &HFF) << 2 Or (m(14) And &HFF) << 10 Or (m(15) And &HFF) << 18
        x._5 = m(16) And &HFF Or (m(17) And &HFF) << 8 Or (m(18) And &HFF) << 16 Or (m(19) And &HFF And 1) << 24
        x._6 = (m(19) And &HFF And Not 1) >> 1 Or (m(20) And &HFF) << 7 Or (m(21) And &HFF) << 15 Or (m(22) And &HFF And 7) << 23
        x._7 = (m(22) And &HFF And Not 7) >> 3 Or (m(23) And &HFF) << 5 Or (m(24) And &HFF) << 13 Or (m(25) And &HFF And 15) << 21
        x._8 = (m(25) And &HFF And Not 15) >> 4 Or (m(26) And &HFF) << 4 Or (m(27) And &HFF) << 12 Or (m(28) And &HFF And 63) << 20
        x._9 = (m(28) And &HFF And Not 63) >> 6 Or (m(29) And &HFF) << 2 Or (m(30) And &HFF) << 10 Or (m(31) And &HFF) << 18
    End Sub
    ' Check if reduced-form input >= 2^255-19 
    Function Is_Overflow(ByVal x As Long10) As Boolean
        Return x._0 > P26 - 19 AndAlso (x._1 And x._3 And x._5 And x._7 And x._9) = P25 AndAlso (x._2 And x._4 And x._6 And x._8) = P26 OrElse x._9 > P25
    End Function
    ' Convert from internal format to little-endian byte format.  The 
    ' 	 * number must be in a reduced form which is output by the following ops:
    ' 	 *     unpack, mul, sqr
    ' 	 *     set --  if input in range 0 .. P25
    ' 	 * If you're unsure if the number is reduced, first multiply it by 1.  
    Sub Pack(ByVal x As Long10, ByRef m As Byte())
        Dim ld As Integer = 0, ud As Integer = 0
        Dim t As Long
        ld = If(Is_Overflow(x), 1, 0) - If(x._9 < 0, 1, 0)
        ud = ld * -(P25 + 1)
        ld *= 19
        t = ld + x._0 + (x._1 << 26)
        m(0) = Convert.ToByte(t And &HFF)
        m(1) = Convert.ToByte((t >> 8) And &HFF)
        m(2) = Convert.ToByte((t >> 16) And &HFF)
        m(3) = Convert.ToByte((t >> 24) And &HFF)
        t = (t >> 32) + (x._2 << 19)
        m(4) = Convert.ToByte(t And &HFF)
        m(5) = Convert.ToByte((t >> 8) And &HFF)
        m(6) = Convert.ToByte((t >> 16) And &HFF)
        m(7) = Convert.ToByte((t >> 24) And &HFF)
        t = (t >> 32) + (x._3 << 13)
        m(8) = Convert.ToByte(t And &HFF)
        m(9) = Convert.ToByte((t >> 8) And &HFF)
        m(10) = Convert.ToByte((t >> 16) And &HFF)
        m(11) = Convert.ToByte((t >> 24) And &HFF)
        t = (t >> 32) + (x._4 << 6)
        m(12) = Convert.ToByte(t And &HFF)
        m(13) = Convert.ToByte((t >> 8) And &HFF)
        m(14) = Convert.ToByte((t >> 16) And &HFF)
        m(15) = Convert.ToByte((t >> 24) And &HFF)
        t = (t >> 32) + x._5 + (x._6 << 25)
        m(16) = Convert.ToByte(t And &HFF)
        m(17) = Convert.ToByte((t >> 8) And &HFF)
        m(18) = Convert.ToByte((t >> 16) And &HFF)
        m(19) = Convert.ToByte((t >> 24) And &HFF)
        t = (t >> 32) + (x._7 << 19)
        m(20) = Convert.ToByte(t And &HFF)
        m(21) = Convert.ToByte((t >> 8) And &HFF)
        m(22) = Convert.ToByte((t >> 16) And &HFF)
        m(23) = Convert.ToByte((t >> 24) And &HFF)
        t = (t >> 32) + (x._8 << 12)
        m(24) = Convert.ToByte(t And &HFF)
        m(25) = Convert.ToByte((t >> 8) And &HFF)
        m(26) = Convert.ToByte((t >> 16) And &HFF)
        m(27) = Convert.ToByte((t >> 24) And &HFF)
        t = (t >> 32) + (x._9 + ud << 6)
        m(28) = Convert.ToByte(t And &HFF)
        m(29) = Convert.ToByte((t >> 8) And &HFF)
        m(30) = Convert.ToByte((t >> 16) And &HFF)
        m(31) = Convert.ToByte((t >> 24) And &HFF)
    End Sub
    ' Copy a number 
    Sub Copy(ByRef _out As Long10, ByVal _in As Long10)
        _out._0 = _in._0
        _out._1 = _in._1
        _out._2 = _in._2
        _out._3 = _in._3
        _out._4 = _in._4
        _out._5 = _in._5
        _out._6 = _in._6
        _out._7 = _in._7
        _out._8 = _in._8
        _out._9 = _in._9
    End Sub
    ' Set a number to value, which must be in range -185861411 .. 185861411 
    Sub Set_In_To_Out(ByRef _out As Long10, ByVal _in As Integer)
        _out._0 = _in
        _out._1 = 0
        _out._2 = 0
        _out._3 = 0
        _out._4 = 0
        _out._5 = 0
        _out._6 = 0
        _out._7 = 0
        _out._8 = 0
        _out._9 = 0
    End Sub
    ' Add/subtract two numbers.  The inputs must be in reduced form, and the 
    ' 	 * output isn't, so to do another addition or subtraction on the output, 
    ' 	 * first multiply it by one to reduce it. 
    Sub AddUp(ByRef xy As Long10, ByVal x As Long10, ByVal y As Long10)
        xy._0 = x._0 + y._0
        xy._1 = x._1 + y._1
        xy._2 = x._2 + y._2
        xy._3 = x._3 + y._3
        xy._4 = x._4 + y._4
        xy._5 = x._5 + y._5
        xy._6 = x._6 + y._6
        xy._7 = x._7 + y._7
        xy._8 = x._8 + y._8
        xy._9 = x._9 + y._9
    End Sub
    Sub Subtract(ByRef xy As Long10, ByVal x As Long10, ByVal y As Long10)
        xy._0 = x._0 - y._0
        xy._1 = x._1 - y._1
        xy._2 = x._2 - y._2
        xy._3 = x._3 - y._3
        xy._4 = x._4 - y._4
        xy._5 = x._5 - y._5
        xy._6 = x._6 - y._6
        xy._7 = x._7 - y._7
        xy._8 = x._8 - y._8
        xy._9 = x._9 - y._9
    End Sub
    ' Multiply a number by a small  Integer  in range -185861411 .. 185861411.
    ' 	 * The output is in reduced form, the input x need not be.  x and xy may point
    ' 	 * to the same buffer. 
    Function Multiply_Small(ByVal xy As Long10, ByVal x As Long10, ByVal y As Long) As Long10
        Dim t As Long
        t = x._8 * y
        xy._8 = t And (1 << 26) - 1
        t = (t >> 26) + x._9 * y
        xy._9 = t And (1 << 25) - 1
        t = 19 * (t >> 25) + x._0 * y
        xy._0 = t And (1 << 26) - 1
        t = (t >> 26) + x._1 * y
        xy._1 = t And (1 << 25) - 1
        t = (t >> 25) + x._2 * y
        xy._2 = t And (1 << 26) - 1
        t = (t >> 26) + x._3 * y
        xy._3 = t And (1 << 25) - 1
        t = (t >> 25) + x._4 * y
        xy._4 = t And (1 << 26) - 1
        t = (t >> 26) + x._5 * y
        xy._5 = t And (1 << 25) - 1
        t = (t >> 25) + x._6 * y
        xy._6 = t And (1 << 26) - 1
        t = (t >> 26) + x._7 * y
        xy._7 = t And (1 << 25) - 1
        t = (t >> 25) + xy._8
        xy._8 = t And (1 << 26) - 1
        xy._9 += t >> 26
        Return xy
    End Function
    ' Multiply two numbers.  The output is in reduced form, the inputs need not 
    ' 	 * be. 
    Function Multiply(ByVal xy As Long10, ByVal x As Long10, ByVal y As Long10) As Long10
        ' sahn0:
        ' 		 * Using local variables to avoid class access.
        ' 		 * This seem to improve performance a bit...
        ' 		 
        Dim x_0 As Long = x._0, x_1 As Long = x._1, x_2 As Long = x._2, x_3 As Long = x._3, x_4 As Long = x._4, x_5 As Long = x._5, x_6 As Long = x._6, x_7 As Long = x._7, x_8 As Long = x._8, x_9 As Long = x._9
        Dim y_0 As Long = y._0, y_1 As Long = y._1, y_2 As Long = y._2, y_3 As Long = y._3, y_4 As Long = y._4, y_5 As Long = y._5, y_6 As Long = y._6, y_7 As Long = y._7, y_8 As Long = y._8, y_9 As Long = y._9
        Dim t As Long
        t = x_0 * y_8 + x_2 * y_6 + x_4 * y_4 + x_6 * y_2 + x_8 * y_0 + 2 * (x_1 * y_7 + x_3 * y_5 + x_5 * y_3 + x_7 * y_1) + 38 * (x_9 * y_9)
        xy._8 = t And (1 << 26) - 1
        t = (t >> 26) + x_0 * y_9 + x_1 * y_8 + x_2 * y_7 + x_3 * y_6 + x_4 * y_5 + x_5 * y_4 + x_6 * y_3 + x_7 * y_2 + x_8 * y_1 + x_9 * y_0
        xy._9 = t And (1 << 25) - 1
        t = x_0 * y_0 + 19 * ((t >> 25) + x_2 * y_8 + x_4 * y_6 + x_6 * y_4 + x_8 * y_2) + 38 * (x_1 * y_9 + x_3 * y_7 + x_5 * y_5 + x_7 * y_3 + x_9 * y_1)
        xy._0 = t And (1 << 26) - 1
        t = (t >> 26) + x_0 * y_1 + x_1 * y_0 + 19 * (x_2 * y_9 + x_3 * y_8 + x_4 * y_7 + x_5 * y_6 + x_6 * y_5 + x_7 * y_4 + x_8 * y_3 + x_9 * y_2)
        xy._1 = t And (1 << 25) - 1
        t = (t >> 25) + x_0 * y_2 + x_2 * y_0 + 19 * (x_4 * y_8 + x_6 * y_6 + x_8 * y_4) + 2 * (x_1 * y_1) + 38 * (x_3 * y_9 + x_5 * y_7 + x_7 * y_5 + x_9 * y_3)
        xy._2 = t And (1 << 26) - 1
        t = (t >> 26) + x_0 * y_3 + x_1 * y_2 + x_2 * y_1 + x_3 * y_0 + 19 * (x_4 * y_9 + x_5 * y_8 + x_6 * y_7 + x_7 * y_6 + x_8 * y_5 + x_9 * y_4)
        xy._3 = t And (1 << 25) - 1
        t = (t >> 25) + x_0 * y_4 + x_2 * y_2 + x_4 * y_0 + 19 * (x_6 * y_8 + x_8 * y_6) + 2 * (x_1 * y_3 + x_3 * y_1) + 38 * (x_5 * y_9 + x_7 * y_7 + x_9 * y_5)
        xy._4 = t And (1 << 26) - 1
        t = (t >> 26) + x_0 * y_5 + x_1 * y_4 + x_2 * y_3 + x_3 * y_2 + x_4 * y_1 + x_5 * y_0 + 19 * (x_6 * y_9 + x_7 * y_8 + x_8 * y_7 + x_9 * y_6)
        xy._5 = t And (1 << 25) - 1
        t = (t >> 25) + x_0 * y_6 + x_2 * y_4 + x_4 * y_2 + x_6 * y_0 + 19 * (x_8 * y_8) + 2 * (x_1 * y_5 + x_3 * y_3 + x_5 * y_1) + 38 * (x_7 * y_9 + x_9 * y_7)
        xy._6 = t And (1 << 26) - 1
        t = (t >> 26) + x_0 * y_7 + x_1 * y_6 + x_2 * y_5 + x_3 * y_4 + x_4 * y_3 + x_5 * y_2 + x_6 * y_1 + x_7 * y_0 + 19 * (x_8 * y_9 + x_9 * y_8)
        xy._7 = t And (1 << 25) - 1
        t = (t >> 25) + xy._8
        xy._8 = t And (1 << 26) - 1
        xy._9 += t >> 26
        Return xy
    End Function
    ' Square a number.  Optimization of  mul25519(x2, x, x)  
    Function Square(ByVal x2 As Long10, ByVal x As Long10) As Long10
        Dim x_0 As Long = x._0, x_1 As Long = x._1, x_2 As Long = x._2, x_3 As Long = x._3, x_4 As Long = x._4, x_5 As Long = x._5, x_6 As Long = x._6, x_7 As Long = x._7, x_8 As Long = x._8, x_9 As Long = x._9
        Dim t As Long
        t = x_4 * x_4 + 2 * (x_0 * x_8 + x_2 * x_6) + 38 * (x_9 * x_9) + 4 * (x_1 * x_7 + x_3 * x_5)
        x2._8 = t And (1 << 26) - 1
        t = (t >> 26) + 2 * (x_0 * x_9 + x_1 * x_8 + x_2 * x_7 + x_3 * x_6 + x_4 * x_5)
        x2._9 = t And (1 << 25) - 1
        t = 19 * (t >> 25) + x_0 * x_0 + 38 * (x_2 * x_8 + x_4 * x_6 + x_5 * x_5) + 76 * (x_1 * x_9 + x_3 * x_7)
        x2._0 = t And (1 << 26) - 1
        t = (t >> 26) + 2 * (x_0 * x_1) + 38 * (x_2 * x_9 + x_3 * x_8 + x_4 * x_7 + x_5 * x_6)
        x2._1 = t And (1 << 25) - 1
        t = (t >> 25) + 19 * (x_6 * x_6) + 2 * (x_0 * x_2 + x_1 * x_1) + 38 * (x_4 * x_8) + 76 * (x_3 * x_9 + x_5 * x_7)
        x2._2 = t And (1 << 26) - 1
        t = (t >> 26) + 2 * (x_0 * x_3 + x_1 * x_2) + 38 * (x_4 * x_9 + x_5 * x_8 + x_6 * x_7)
        x2._3 = t And (1 << 25) - 1
        t = (t >> 25) + x_2 * x_2 + 2 * (x_0 * x_4) + 38 * (x_6 * x_8 + x_7 * x_7) + 4 * (x_1 * x_3) + 76 * (x_5 * x_9)
        x2._4 = t And (1 << 26) - 1
        t = (t >> 26) + 2 * (x_0 * x_5 + x_1 * x_4 + x_2 * x_3) + 38 * (x_6 * x_9 + x_7 * x_8)
        x2._5 = t And (1 << 25) - 1
        t = (t >> 25) + 19 * (x_8 * x_8) + 2 * (x_0 * x_6 + x_2 * x_4 + x_3 * x_3) + 4 * (x_1 * x_5) + 76 * (x_7 * x_9)
        x2._6 = t And (1 << 26) - 1
        t = (t >> 26) + 2 * (x_0 * x_7 + x_1 * x_6 + x_2 * x_5 + x_3 * x_4) + 38 * (x_8 * x_9)
        x2._7 = t And (1 << 25) - 1
        t = (t >> 25) + x2._8
        x2._8 = t And (1 << 26) - 1
        x2._9 += t >> 26
        Return x2
    End Function
    ' Calculates a reciprocal.  The output is in reduced form, the inputs need not 
    ' 	 * be.  Simply calculates  y = x^(p-2)  so it's not too fast. 
    ' When sqrtassist is true, it instead calculates y = x^((p-5)/8) 
    Sub Reciprocal(ByRef y As Long10, ByVal x As Long10, ByVal sqrtassist As Integer)
        Dim t0 As Long10 = New Long10(), t1 As Long10 = New Long10(), t2 As Long10 = New Long10(), t3 As Long10 = New Long10(), t4 As Long10 = New Long10()
        Dim i As Integer
        ' the chain for x^(2^255-21) is straight from djb's implementation 
        Square(t1, x) ' 2 == 2 * 1	
        Square(t2, t1)  ' 4 == 2 * 2	
        Square(t0, t2)  ' 8 == 2 * 4	
        Multiply(t2, t0, x) ' 9 == 8 + 1	
        Multiply(t0, t2, t1)  ' 11 == 9 + 2	
        Square(t1, t0)  ' 22 == 2 * 11	
        Multiply(t3, t1, t2)  ' 31 == 22 + 9 == 2^5   - 2^0	
        Square(t1, t3)  ' 2^6   - 2^1	
        Square(t2, t1)  ' 2^7   - 2^2	
        Square(t1, t2)  ' 2^8   - 2^3	
        Square(t2, t1)  ' 2^9   - 2^4	
        Square(t1, t2)  ' 2^10  - 2^5	
        Multiply(t2, t1, t3)  ' 2^10  - 2^0	
        Square(t1, t2)  ' 2^11  - 2^1	
        Square(t3, t1)  ' 2^12  - 2^2	
        For i = 1 To 5 - 1
            Square(t1, t3)
            Square(t3, t1)
        Next ' t3 
        ' 2^20  - 2^10	
        Multiply(t1, t3, t2)  ' 2^20  - 2^0	
        Square(t3, t1)  ' 2^21  - 2^1	
        Square(t4, t3)  ' 2^22  - 2^2	
        For i = 1 To 10 - 1
            Square(t3, t4)
            Square(t4, t3)
        Next ' t4 
        ' 2^40  - 2^20	
        Multiply(t3, t4, t1)  ' 2^40  - 2^0	
        For i = 0 To 5 - 1
            Square(t1, t3)
            Square(t3, t1)
        Next ' t3 
        ' 2^50  - 2^10	
        Multiply(t1, t3, t2)  ' 2^50  - 2^0	
        Square(t2, t1)  ' 2^51  - 2^1	
        Square(t3, t2)  ' 2^52  - 2^2	
        For i = 1 To 25 - 1
            Square(t2, t3)
            Square(t3, t2)
        Next ' t3 
        ' 2^100 - 2^50 
        Multiply(t2, t3, t1)  ' 2^100 - 2^0	
        Square(t3, t2)  ' 2^101 - 2^1	
        Square(t4, t3)  ' 2^102 - 2^2	
        For i = 1 To 50 - 1
            Square(t3, t4)
            Square(t4, t3)
        Next ' t4 
        ' 2^200 - 2^100 
        Multiply(t3, t4, t2)  ' 2^200 - 2^0	
        For i = 0 To 25 - 1
            Square(t4, t3)
            Square(t3, t4)
        Next ' t3 
        ' 2^250 - 2^50	
        Multiply(t2, t3, t1)  ' 2^250 - 2^0	
        Square(t1, t2)  ' 2^251 - 2^1	
        Square(t2, t1)  ' 2^252 - 2^2	
        If sqrtassist <> 0 Then
            Multiply(y, x, t2)  ' 2^252 - 3 
        Else
            Square(t1, t2)  ' 2^253 - 2^3	
            Square(t2, t1)  ' 2^254 - 2^4	
            Square(t1, t2)  ' 2^255 - 2^5	
            Multiply(y, t1, t0) ' 2^255 - 21	
        End If
    End Sub
    ' checks if x is "negative", requires reduced input 
    Function Is_Negative(ByVal x As Long10) As Integer
        Return Convert.ToInt32(If(Is_Overflow(x) OrElse x._9 < 0, 1, 0) Xor (x._0 And 1))
    End Function
    ' a square root 
    Sub Square_Root(ByRef x As Long10, ByVal u As Long10)
        Dim v As Long10 = New Long10(), t1 As Long10 = New Long10(), t2 As Long10 = New Long10()
        AddUp(t1, u, u)  ' t1 = 2u		
        Reciprocal(v, t1, 1)  ' v = (2u)^((p-5)/8)	
        Square(x, v)    ' x = v^2		
        Multiply(t2, t1, x) ' t2 = 2uv^2		
        t2._0 -= 1    ' t2 = 2uv^2-1		
        Multiply(t1, v, t2) ' t1 = v(2uv^2-1)	
        Multiply(x, u, t1)  ' x = uv(2uv^2-1)	
    End Sub
    ' ******************* Elliptic curve ********************
    ' y^2 = x^3 + 486662 x^2 + x  over GF(2^255-19) 
    ' t1 = ax + az
    ' 	 * t2 = ax - az  
    Sub Monty_Prepare(ByRef t1 As Long10, ByRef t2 As Long10, ByVal ax As Long10, ByVal az As Long10)
        AddUp(t1, ax, az)
        Subtract(t2, ax, az)
    End Sub
    ' A = P + Q   where
    ' 	 *  X(A) = ax/az
    ' 	 *  X(P) = (t1+t2)/(t1-t2)
    ' 	 *  X(Q) = (t3+t4)/(t3-t4)
    ' 	 *  X(P-Q) = dx
    ' 	 * clobbers t1 and t2, preserves t3 and t4  
    Sub Monty_AddUp(ByRef t1 As Long10, ByRef t2 As Long10, ByVal t3 As Long10, ByVal t4 As Long10, ByVal ax As Long10, ByVal az As Long10, ByVal dx As Long10)
        Multiply(ax, t2, t3)
        Multiply(az, t1, t4)
        AddUp(t1, ax, az)
        Subtract(t2, ax, az)
        Square(ax, t1)
        Square(t1, t2)
        Multiply(az, t1, dx)
    End Sub
    ' B = 2 * Q   where
    ' 	 *  X(B) = bx/bz
    ' 	 *  X(Q) = (t3+t4)/(t3-t4)
    ' 	 * clobbers t1 and t2, preserves t3 and t4  
    Sub Monty_Double(ByRef t1 As Long10, ByRef t2 As Long10, ByVal t3 As Long10, ByVal t4 As Long10, ByRef bx As Long10, ByRef bz As Long10)
        Square(t1, t3)
        Square(t2, t4)
        Multiply(bx, t1, t2)
        Subtract(t2, t1, t2)
        Multiply_Small(bz, t2, 121665)
        AddUp(t1, t1, bz)
        Multiply(bz, t1, t2)
    End Sub
    ' Y^2 = X^3 + 486662 X^2 + X
    ' 	 * t is a temporary  
    Sub X_To_Y2(ByRef t As Long10, ByRef y2 As Long10, ByVal x As Long10)
        Square(t, x)
        Multiply_Small(y2, x, C486662)
        AddUp(t, t, y2)
        t._0 += 1
        Multiply(y2, t, x)
    End Sub
    ' P = kG   and  s = sign(P)/k  
    Sub Core(ByRef Px As Byte(), ByRef s As Byte(), ByRef k As Byte(), ByRef Gx As Byte())
        Dim dx As Long10 = New Long10(), t1 As Long10 = New Long10(), t2 As Long10 = New Long10(), t3 As Long10 = New Long10(), t4 As Long10 = New Long10()
        Dim x As Long10() = New Long10() {New Long10(), New Long10()}, z As Long10() = New Long10() {New Long10(), New Long10()}
        Dim i, j As Integer
        ' unpack the base 
        If Gx IsNot Nothing Then
            Unpack(dx, Gx)
        Else
            Set_In_To_Out(dx, 9)
        End If
        ' 0G = point-at-infinity 
        Set_In_To_Out(x(0), 1)
        Set_In_To_Out(z(0), 0)
        ' 1G = G 
        Copy(x(1), dx)
        Set_In_To_Out(z(1), 1)
        i = 32
        While Math.Max(Interlocked.Decrement(i), i + 1) <> 0
            If i = 0 Then
                i = 0
            End If
            j = 8
            While Math.Max(Interlocked.Decrement(j), j + 1) <> 0
                ' swap arguments depending on bit 
                Dim bit1 As Integer = (k(i) And &HFF) >> j And 1
                Dim bit0 As Integer = Not (k(i) And &HFF) >> j And 1
                Dim ax As Long10 = x(bit0)
                Dim az As Long10 = z(bit0)
                Dim bx As Long10 = x(bit1)
                Dim bz As Long10 = z(bit1)
                ' a' = a + b	
                ' b' = 2 b	
                Monty_Prepare(t1, t2, ax, az)
                Monty_Prepare(t3, t4, bx, bz)
                Monty_AddUp(t1, t2, t3, t4, ax, az, dx)
                Monty_Double(t1, t2, t3, t4, bx, bz)
            End While
        End While
        Reciprocal(t1, z(0), 0)
        Multiply(dx, x(0), t1)
        Pack(dx, Px)
        ' calculate s such that s abs(P) = G  .. assumes G is std base point 
        If s IsNot Nothing Then
            X_To_Y2(t2, t1, dx)  ' t1 = Py^2  
            Reciprocal(t3, z(1), 0) ' where Q=P+G ... 
            Multiply(t2, x(1), t3)  ' t2 = Qx  
            AddUp(t2, t2, dx)  ' t2 = Qx + Px  
            t2._0 += C9 + C486662  ' t2 = Qx + Px + Gx + 486662  
            dx._0 -= C9  ' dx = Px - Gx  
            Square(t3, dx)  ' t3 = (Px - Gx)^2  
            Multiply(dx, t2, t3)  ' dx = t2 (Px - Gx)^2  
            Subtract(dx, dx, t1)  ' dx = t2 (Px - Gx)^2 - Py^2  
            dx._0 -= C39420360   ' dx = t2 (Px - Gx)^2 - Py^2 - Gy^2  
            Multiply(t1, dx, BASE_R2Y)  ' t1 = -Py  
            If Is_Negative(t1) <> 0 Then ' sign is 1, so just copy  
                Copy32(s, k)      ' sign is -1, so negate  
            Else
                Multiply_Array_Small(s, ORDER_TIMES_8, 0, k, 32, -1)
            End If
            ' reduce s mod q
            ' 			 * (is this needed?  do it just in case, it's fast anyway) 
            'divmod((dstptr) t1, s, 32, order25519, 32);
            ' take reciprocal of s mod q 
            Dim temp1 = New Byte(31) {}
            Dim temp2 = New Byte(63) {}
            Dim temp3 = New Byte(63) {}
            Copy32(temp1, ORDER)
            Copy32(s, Egcd32(temp2, temp3, s, temp1))
            If (s(31) And &H80) <> 0 Then Multiply_Array_Small(s, s, 0, ORDER, 32, 1)
        End If
    End Sub

End Class