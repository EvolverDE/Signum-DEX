
Option Strict On
Option Explicit On

Imports System
Imports System.Threading

Imports PFP.Curve25519


Public Class EC_KCDSA

    ' ******* DIGITAL SIGNATURES ********
    ' deterministic EC-KCDSA
    ' 	 *
    ' 	 *    s is the private key for signing
    ' 	 *    P is the corresponding public key
    ' 	 *    Z is the context data (signer public key or certificate, etc)
    ' 	 *
    ' 	 * signing:
    ' 	 *
    ' 	 *    m = hash(Z, message)
    ' 	 *    x = hash(m, s)
    ' 	 *    keygen25519(Y, NULL, x);
    ' 	 *    r = hash(Y);
    ' 	 *    h = m XOR r
    ' 	 *    sign25519(v, h, x, s);
    ' 	 *
    ' 	 *    output (v,r) as the signature
    ' 	 *
    ' 	 * verification:
    ' 	 *
    ' 	 *    m = hash(Z, message);
    ' 	 *    h = m XOR r
    ' 	 *    verify25519(Y, v, h, P)
    ' 	 *
    ' 	 *    confirm  r == hash(Y)
    ' 	 *
    ' 	 * It would seem to me that it would be simpler to have the signer directly do 
    ' 	 * h = hash(m, Y) and send that to the recipient instead of r, who can verify 
    ' 	 * the signature by checking h == hash(m, Y).  If there are any problems with 
    ' 	 * such a scheme, please let me know.
    ' 	 *
    ' 	 * Also, EC-KCDSA (like most DS algorithms) picks x random, which is a waste of 
    ' 	 * perfectly good entropy, but does allow Y to be calculated in advance of (or 
    ' 	 * parallel to) hashing the message.
    ' 	 

    ''' <summary>
    ''' Signature generation primitive, calculates (x-h)s mod q
    ''' </summary>
    ''' <param name="SignHash">signature hash (of message, signature pub key, and context data)</param>
    ''' <param name="SignPrivKey">signature private key</param>
    ''' <param name="PrivKey">private key for signing</param>
    ''' <returns>signature value</returns>
    Function Sign(ByVal SignHash As Byte(), ByVal SignPrivKey As Byte(), ByVal PrivKey As Byte()) As Byte()
        ' v = (x - h) s  Mod q

        Dim Dummy1(31) As Byte
        Dim Dummy2(31) As Byte
        Dim Temp_v(63) As Byte
        Dim Dummy3(63) As Byte

        Dim Curve As Curve25519 = New Curve25519

        ' Reduce modulo group order
        Curve.DivideMod(Dummy1, SignHash, 32, Curve.ORDER, 32)
        Curve.DivideMod(Dummy2, SignPrivKey, 32, Curve.ORDER, 32)

        ' v = x1 - h1
        ' If v Is negative, add the group order to it to become positive.
        ' If v was already positive we don't have to worry about overflow
        ' when adding the order because v < ORDER And 2*ORDER < 2^256
        Dim SignValue As Byte() = New Byte(31) {}
        Curve.Multiply_Array_Small(SignValue, SignPrivKey, 0, SignHash, 32, -1)
        Curve.Multiply_Array_Small(SignValue, SignValue, 0, Curve.ORDER, 32, 1)

        ' Temp_v = (x-h)*s Mod q
        Curve.Multiply_Array32(Temp_v, SignValue, PrivKey, 32, 1)
        Curve.DivideMod(Dummy3, Temp_v, 64, Curve.ORDER, 32)

        Dim w As Boolean = False
        For i As Integer = 0 To 31
            SignValue(i) = Temp_v(i)
            w = w Or CBool(SignValue(i))
        Next

        If w <> False Then
            Return SignValue
        Else
            Return Nothing
        End If

    End Function


    ''' <summary>
    ''' Signature verification primitive, calculates Y = vP + hG
    ''' </summary>
    ''' <param name="SignValue">signature value</param>
    ''' <param name="SignHash">signature hash</param>
    ''' <param name="PublicKey">public key</param>
    ''' <returns>signature public key</returns>
    Function Verify(ByVal SignValue As Byte(), ByVal SignHash As Byte(), ByVal PublicKey As Byte()) As Byte()

        ' SignPublicKey = SignValue abs(PublicKey) + SignHash G  

        Dim Curve As Curve25519 = New Curve25519

        Dim d = New Byte(31) {}
        Dim p As Long10() = New Long10() {New Long10(), New Long10()}, s As Long10() = New Long10() {New Long10(), New Long10()}, yx As Long10() = New Long10() {New Long10(), New Long10(), New Long10()}, yz As Long10() = New Long10() {New Long10(), New Long10(), New Long10()}, t1 As Long10() = New Long10() {New Long10(), New Long10(), New Long10()}, t2 As Long10() = New Long10() {New Long10(), New Long10(), New Long10()}
        Dim k As Int32, vi As Int32 = 0, hi As Int32 = 0, di As Int32 = 0, nvh As Int32 = 0

        ' set p[0] to G and p[1] to P  
        Curve.Set_In_To_Out(p(0), 9)
        Curve.Unpack(p(1), PublicKey)
        ' set s[0] to P+G and s[1] to P-G  
        ' s[0] = (Py^2 + Gy^2 - 2 Py Gy)/(Px - Gx)^2 - Px - Gx - 486662  
        ' s[1] = (Py^2 + Gy^2 + 2 Py Gy)/(Px - Gx)^2 - Px - Gx - 486662  
        Curve.X_To_Y2(t1(0), t2(0), p(1))  ' t2[0] = Py^2  
        Curve.Square_Root(t1(0), t2(0)) ' t1[0] = Py or -Py  

        Dim Negative As Integer = Curve.Is_Negative(t1(0))   ' ... check which  
        t2(0)._0 += Curve25519.C39420360   ' t2[0] = Py^2 + Gy^2  
        Curve.Multiply(t2(1), Curve.BASE_2Y, t1(0)) ' t2[1] = 2 Py Gy or -2 Py Gy  
        Curve.Subtract(t1(Negative), t2(0), t2(1)) ' t1[0] = Py^2 + Gy^2 - 2 Py Gy  
        Curve.AddUp(t1(1 - Negative), t2(0), t2(1)) ' t1[1] = Py^2 + Gy^2 + 2 Py Gy  
        Curve.Copy(t2(0), p(1))   ' t2[0] = Px  
        t2(0)._0 -= 9      ' t2[0] = Px - Gx  
        Curve.Square(t2(1), t2(0))    ' t2[1] = (Px - Gx)^2  
        Curve.Reciprocal(t2(0), t2(1), 0) ' t2[0] = 1/(Px - Gx)^2  
        Curve.Multiply(s(0), t1(0), t2(0))  ' s[0] = t1[0]/(Px - Gx)^2  
        Curve.Subtract(s(0), s(0), p(1))  ' s[0] = t1[0]/(Px - Gx)^2 - Px  
        s(0)._0 -= Curve25519.C9 + Curve25519.C486662    ' s[0] = X(P+G)  
        Curve.Multiply(s(1), t1(1), t2(0))  ' s[1] = t1[1]/(Px - Gx)^2  
        Curve.Subtract(s(1), s(1), p(1))  ' s[1] = t1[1]/(Px - Gx)^2 - Px  
        s(1)._0 -= Curve25519.C9 + Curve25519.C486662    ' s[1] = X(P-G)  
        Curve.Multiply_Small(s(0), s(0), 1) ' reduce s[0] 
        Curve.Multiply_Small(s(1), s(1), 1) ' reduce s[1] 
        ' prepare the chain  
        For i As Integer = 0 To 32 - 1
            vi = vi >> 8 Xor SignValue(i) And &HFF Xor (SignValue(i) And &HFF) << 1
            hi = hi >> 8 Xor SignHash(i) And &HFF Xor (SignHash(i) And &HFF) << 1
            nvh = Not (vi Xor hi)
            di = nvh And (di And &H80) >> 7 Xor vi
            di = di Xor nvh And (di And &H1) << 1
            di = di Xor nvh And (di And &H2) << 1
            di = di Xor nvh And (di And &H4) << 1
            di = di Xor nvh And (di And &H8) << 1
            di = di Xor nvh And (di And &H10) << 1
            di = di Xor nvh And (di And &H20) << 1
            di = di Xor nvh And (di And &H40) << 1
            d(i) = Convert.ToByte(di And &HFF)
        Next
        di = (nvh And (di And &H80) << 1 Xor vi) >> 8
        ' initialize state 
        Curve.Set_In_To_Out(yx(0), 1)
        Curve.Copy(yx(1), p(di))
        Curve.Copy(yx(2), s(0))
        Curve.Set_In_To_Out(yz(0), 0)
        Curve.Set_In_To_Out(yz(1), 1)
        Curve.Set_In_To_Out(yz(2), 1)
        ' y[0] is (even)P + (even)G
        ' 		 * y[1] is (even)P + (odd)G  if current d-bit is 0
        ' 		 * y[1] is (odd)P + (even)G  if current d-bit is 1
        ' 		 * y[2] is (odd)P + (odd)G
        ' 		 
        vi = 0
        hi = 0
        ' and go for it! 

        For i As Integer = 31 To 0 Step -1
            vi = vi << 8 Or SignValue(i) And &HFF
            hi = hi << 8 Or SignHash(i) And &HFF
            di = di << 8 Or d(i) And &HFF

            For j As Integer = 7 To 0 Step -1
                Curve.Monty_Prepare(t1(0), t2(0), yx(0), yz(0))
                Curve.Monty_Prepare(t1(1), t2(1), yx(1), yz(1))
                Curve.Monty_Prepare(t1(2), t2(2), yx(2), yz(2))
                k = ((vi Xor vi >> 1) >> j And 1) + ((hi Xor hi >> 1) >> j And 1)
                Curve.Monty_Double(yx(2), yz(2), t1(k), t2(k), yx(0), yz(0))
                k = di >> j And 2 Xor (di >> j And 1) << 1
                Curve.Monty_AddUp(t1(1), t2(1), t1(k), t2(k), yx(1), yz(1), p(di >> j And 1))
                Curve.Monty_AddUp(t1(2), t2(2), t1(0), t2(0), yx(2), yz(2), s(((vi Xor hi) >> j And 2) >> 1))
            Next
        Next

        k = (vi And 1) + (hi And 1)
        Curve.Reciprocal(t1(0), yz(k), 0)
        Curve.Multiply(t1(1), yx(k), t1(0))

        Dim SignPublicKey As Byte() = New Byte(31) {}

        Curve.Pack(t1(1), SignPublicKey)

        Return SignPublicKey

    End Function

End Class