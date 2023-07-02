Option Strict On
Option Explicit On

'https://github.com/MrMaxweII/Secp256k1-Calculator/blob/c9374a8dab79a0b609c235b9c6e8c3ba290e410a/Math_Modulo.java
' **************************************************************************************  
'                                            						*
'    Math_Modulo Class V1.1                           					*
'    Hier werden mathematische Berechnungen definiert  über den Zahlenraum Modulo "p"  	*
'                                          						*
' ************************************************************************************** 

Imports System.Globalization
Imports System.Numerics

Namespace Secp256k1Vb

    Friend NotInheritable Class Math_Modulo
        Private Shared ReadOnly ZERO As New BigInteger(0)
        Private Shared ReadOnly ONE As New BigInteger(1)
        Private Shared ReadOnly TWO As New BigInteger(2)
        Private Shared ReadOnly THREE As New BigInteger(3)
        Private Shared ReadOnly FOUR As New BigInteger(4)
        Private Shared ReadOnly FIVE As New BigInteger(5)
        Private Shared ReadOnly SIX As New BigInteger(6)
        Private Shared ReadOnly SEVEN As New BigInteger(7)

        Public Shared ReadOnly P As BigInteger = BigInteger.Parse("0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F", NumberStyles.AllowHexSpecifier)   ' Bitcoin Modulo: FFFFFFFF FFFFFFFF FFFFFFFF FFFFFFFF FFFFFFFF FFFFFFFF FFFFFFFE FFFFFC2F	  	
        Private Shared ReadOnly GENERATOR As BigInteger = BigInteger.Parse("079BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798", NumberStyles.AllowHexSpecifier)   ' Bitcoin Generrator Punkt  G =  02 79BE667E F9DCBBAC 55A06295 CE870B07 029BFCDB 2DCE28D9 59F2815B 16F81798
        Private Shared ReadOnly GENERATORY As BigInteger = BigInteger.Parse("0483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8", NumberStyles.AllowHexSpecifier)  ' Generator Y-Koordinate
        Private Shared ReadOnly ORDNUNG As BigInteger = BigInteger.Parse("0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141", NumberStyles.AllowHexSpecifier)   ' Ordnung n von G: (Modulo)


        ' * Addiert a + b    2,3µs   
        Public Shared Function Add(a As BigInteger, b As BigInteger) As BigInteger
            Return BigInteger.Add(a, b) Mod P
        End Function
        Public Shared Function Add(a As BigInteger, b As BigInteger, c As BigInteger) As BigInteger
            Return BigInteger.Add(BigInteger.Add(a, b), c) Mod P
        End Function
        Public Shared Function Add(a As BigInteger, b As BigInteger, c As BigInteger, d As BigInteger) As BigInteger
            Return Add(BigInteger.Add(a, b), BigInteger.Add(c, d))
        End Function

        ' * negiert -a   0,9µs  
        Public Shared Function Neg(a As BigInteger) As BigInteger
            Return BigInteger.Subtract(P, a)
        End Function

        ' * subrahiert a-b
        Public Shared Function [Sub](a As BigInteger, b As BigInteger) As BigInteger
            Return Add(a, Neg(b))
        End Function

        ' * Multipliziert a * b    7µs  
        Public Shared Function Mul(a As BigInteger, b As BigInteger) As BigInteger
            Return BigInteger.Multiply(a, b) Mod P
        End Function
        Public Shared Function Mul(a As BigInteger, b As BigInteger, c As BigInteger) As BigInteger
            Return BigInteger.Multiply(BigInteger.Multiply(a, b), c) Mod P
        End Function
        Public Shared Function Mul(a As BigInteger, b As BigInteger, c As BigInteger, d As BigInteger) As BigInteger
            Return Mul(Mul(a, b), Mul(c, d))
        End Function

        ' * dividiert a/b   
        Public Shared Function Div(a As BigInteger, b As BigInteger) As BigInteger
            Return Mul(a, Inv(b))
        End Function

        ' * Liefert  1/a   45µs  
        Private Shared Function Inv(a As BigInteger) As BigInteger
            'Sofern p eine Primzahl ist, geht das.
            Return BigInteger.ModPow(a, P - 2, P)
        End Function

        ' * Diese Funktion berechnet die Zahl 1/2 mit der auf der elliptischen 
        ' 	 * Kurve durch zwei geteilt werden kann.  
        Public Shared Function CalcHalb(a As BigInteger) As BigInteger
            Return ModInverse(a, ORDNUNG)
        End Function

        ' * Potenz x^n    (sehr langsam!)   
        Public Shared Function Pow(x As BigInteger, n As BigInteger) As BigInteger
            Return BigInteger.ModPow(x, n, P)
        End Function

        ' * Wurzel sqrt(a)     Tonelli–Shanks algorithmus 
        Public Shared Function Sqrt(n As BigInteger) As BigInteger
            Dim s As BigInteger = ZERO
            Dim q As BigInteger = P - ONE
            While (q And ONE) = ZERO
                q /= TWO : s += ONE
            End While
            If s = ONE Then
                Dim rr As BigInteger = Pow(n, (P + ONE) / FOUR)
                If rr * rr Mod P = n Then Return Abs(rr)
                If True Then
                    'Console.WriteLine(vbCrLf & "Fehler!  sqrt(" & n.ToString() & ") existiert nicht! (Math_Modulo.sqrt) " & vbCrLf)
                    Return ZERO
                End If
            End If

            ' Find the first quadratic non-residue z by brute-force search
            Dim z As BigInteger = ZERO

            While True
                z += ONE
                If Pow(z, (P - ONE) / TWO) = P - ONE Then Exit While
            End While
            Dim c As BigInteger = Pow(z, q)
            Dim r As BigInteger = Pow(n, (q + ONE) / TWO)
            Dim t As BigInteger = Pow(n, q)
            Dim m As BigInteger = s
            While Not t = ONE
                Dim tt As BigInteger = t
                Dim i As BigInteger = ZERO
                While Not tt = ONE
                    tt = tt * tt Mod P
                    i += ONE
                    If i = m Then
                        'If Not n = ZERO Then
                        '    Console.WriteLine(vbCrLf & "Fehler!  sqrt(" & n.ToString & ") existiert nicht! (Math_Modulo.sqrt) " & vbCrLf)
                        'End If
                        Return ZERO
                    End If
                End While
                Dim b As BigInteger = Pow(c, Pow(TWO, m - i - ONE))
                Dim b2 As BigInteger = b * b Mod P
                r = r * b Mod P : t = t * b2 Mod P
                c = b2 : m = i
            End While
            If r * r Mod P = n Then Return Abs(r)
            Return ZERO
        End Function

        ' liefer den Betrag |a|
        Private Shared Function Abs(a As BigInteger) As BigInteger
            If a.CompareTo((P - ONE) / TWO) = 1 Then Return Neg(a)
            Return a
        End Function

        Public Shared Function ModInverse(a As BigInteger, n As BigInteger) As BigInteger
            Dim i As BigInteger = n, v As BigInteger = 0, d As BigInteger = 1
            While a > 0
                Dim t As BigInteger = i / a, x As BigInteger = a
                a = i Mod x : i = x : x = d : d = v - t * x : v = x
            End While
            v = v Mod n
            If v < 0 Then v = (v + n) Mod n
            Return v
        End Function

        Public Shared Function TestBit(number As BigInteger, ibit As Int32) As Boolean
            Return (number And BigInteger.One << ibit) <> 0
        End Function

        Public Shared Function ClearBit(number As BigInteger, n As Int32) As BigInteger
            Return number And Not BigInteger.One << n
        End Function

        Public Shared Function BitLength(number As BigInteger) As Int32
            Dim result = 0
            Do
                result += 1
                number /= 2
            Loop While number <> 0
            Return result
        End Function
    End Class
End Namespace