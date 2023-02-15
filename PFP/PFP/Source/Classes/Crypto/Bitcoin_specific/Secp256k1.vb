Option Strict On
Option Explicit On


'https://github.com/MrMaxweII/Secp256k1-Calculator

' *********************************************************************************************
'  	Secp256k1  V2.3                        					05.11.2019	*       
' 	- Multipliziert einen Faktor mit einem Punkt auf der elliptischen Kurve.		*
' 	- Generiert den Pub.Key durch die Multiplikation von "G" mit dem Priv.Key.		*
' 	- Erzeugt ECDSA Signatur								*	
' 	- Verifiziert ECDSA Signatur								*	
' 												* 
' *********************************************************************************************

Imports System.Numerics
Imports System.Globalization
Imports CRand = PFP.Secp256k1Vb.Randoms.CryptoRandom
' TestSecp256k1Vb.Secp256k1Vb.Randoms.CryptoRandom

Namespace Secp256k1Vb
    Public NotInheritable Class Secp256k1

        Public Shared ReadOnly ZERO As New BigInteger(0)
        Public Shared ReadOnly ONE As New BigInteger(1)
        Public Shared ReadOnly TWO As New BigInteger(2)
        Public Shared ReadOnly THREE As New BigInteger(3)
        Public Shared ReadOnly FOUR As New BigInteger(4)
        Public Shared ReadOnly FIVE As New BigInteger(5)
        Public Shared ReadOnly SIX As New BigInteger(6)
        Public Shared ReadOnly SEVEN As New BigInteger(7)

        Public Shared ReadOnly ModuloHalb As BigInteger = BigInteger.Parse("07FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF7FFFFE17", NumberStyles.AllowHexSpecifier)
        Public Shared ReadOnly GENERATOR As BigInteger = BigInteger.Parse("079BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798", NumberStyles.AllowHexSpecifier)
        Public Shared ReadOnly GENERATORY As BigInteger = BigInteger.Parse("0483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8", NumberStyles.AllowHexSpecifier)
        Public Shared ReadOnly ORDNUNG As BigInteger = BigInteger.Parse("0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141", NumberStyles.AllowHexSpecifier)
        Public Shared ReadOnly HALB As BigInteger = BigInteger.Parse("07fffffffffffffffffffffffffffffff5d576e7357a4501ddfe92f46681b20a1", NumberStyles.AllowHexSpecifier)

        ' *	Mit Start wird die EXP-List inizialisiert.
        ' 	Dies ist für die Multiplikation mit "G" notwendig!  
        Public Shared Sub Start()
            EXPList.Set_EXP_List()
        End Sub

        ' *	Es wird eine Signatur erstellt bestehend aus den Teilen "r" und "s".
        ' 	Übergeben wird der 32byte lange Hash, der signiert werden soll,
        ' 	- der Priv.Key 32Byte,
        ' 	- die "rand" Zufallszahl "k" als ByteArray.
        ' 	Rückgabe ist ein BigInteger-Array bestehend aus 2 Elementen: [0] = r   und    [1] = s. 
        ' 	Achtung: Die "rand" Zufallszahl "k" muss aus einer kryptographisch starken Entropie stammen! 
        ' 	Falls "k" vorhersebar ist, kann der Priv.Key leicht aufgedeckt werden!!! 
        Public Shared Function Sign(ByVal hash As Byte(), ByVal privKey As Byte(), ByVal k As Byte()) As BigInteger()
            If privKey.Length <> 32 Or hash.Length <> 32 Then
                Return {New BigInteger(0), New BigInteger(0)}
            End If
            'If hash.Length <> 32 Then Throw New ArgumentOutOfRangeException(Nothing)

            hash = hash.Reverse().ToArray
            privKey = privKey.Reverse().ToArray

            'Die nächsten 5 Zeilen wären nicht nötig.
            Dim ran = To_FixLength(k, 32)
            If ran(0) < 0 Then
                ran = ran.Take(31).ToArray()
                ran = To_FixLength(ran, 32)
            End If
            Dim rand = ToBigInteger(ran)

            Dim result = New BigInteger(1) {}
            Dim r As BigInteger = Multiply_G(rand)(0)
            Dim r_x_priv As BigInteger = r * ToBigInteger(privKey) Mod ORDNUNG
            Dim zähler As BigInteger = (ToBigInteger(hash) + r_x_priv) Mod ORDNUNG
            Dim k_inverse As BigInteger = Math_Modulo.ModInverse(rand, ORDNUNG)

            result(0) = r
            result(1) = k_inverse * zähler Mod ORDNUNG
            Return result
        End Function

        Public Shared Function Sign(ByVal Message As Byte(), ByVal PrivateKey As Byte()) As Byte()()

            Dim SignatureBigIntegerList As List(Of BigInteger) = New List(Of BigInteger)(Sign(Message, PrivateKey, CreatePrivateKey()))

            Dim R_Bytes As Byte() = SignatureBigIntegerList(0).ToByteArray().Reverse().ToArray()
            Dim S_Bytes As Byte() = SignatureBigIntegerList(1).ToByteArray().Reverse().ToArray()

            Return {R_Bytes, S_Bytes}

        End Function

        ' *	Die Signatur "r" und "s" wird geprüft.
        ' 	- Übergeben wird der 32byte lange Hash, dessen Signatur geprüft werden soll,
        ' 	- die Signatur selbst "sig" als BigInteger-Array bestehend aus 2 Elementen: [0] = r   und    [1] = s. 
        ' 	- und der Pub.Key als BigInteger Array mit 2 Elementen.
        Public Shared Function Verify(ByVal hash As Byte(), ByVal sig As BigInteger(), ByVal pub As BigInteger()) As Boolean
            Dim h As BigInteger = ToBigInteger(hash) Mod ORDNUNG

            Dim s_invers As BigInteger = Math_Modulo.ModInverse(sig(1), ORDNUNG)
            Dim arg1 = Multiply_G(h * s_invers Mod ORDNUNG)
            Dim arg2 = Multiply_Point(pub, sig(0) * s_invers Mod ORDNUNG)
            Dim arg3 = Addition(arg1, arg2)
            If arg3(0) = sig(0) Then
                Return True
            Else Return False
            End If
        End Function

        Public Shared Function Verify(ByVal Message As Byte(), ByVal Signature As Byte()(), ByVal PublicKey As Byte()) As Boolean

            Dim PublicKekBI As PublicKey = Secp256k1Vb.PublicKey.Parse(PublicKey)
            Dim SignatureBigIntegerList As List(Of BigInteger) = New List(Of BigInteger)({New BigInteger(Signature(0).Reverse().ToArray()), New BigInteger(Signature(1).Reverse().ToArray())})
            Return Verify(Message, SignatureBigIntegerList.ToArray, PublicKekBI.ToXY)

        End Function

        ' *	Multipliziert den Generator mit dem "factor" auf der elliptischen Kurve.  
        ' 	*	Schnelle Berechnung mit Hilfe der EXP_List.   ca. 3ms  
        Public Shared Function Multiply_G(ByVal factor As BigInteger) As BigInteger()
            Dim voher As BigInteger() = EXPList.nullVektor
            Dim result = New BigInteger(1) {}
            For i = 0 To 255
                If Math_Modulo.TestBit(factor, i) Then
                    result = Addition(voher, EXPList.list(i))
                    voher = result
                End If
            Next
            Return result
        End Function

        ' * Multipliziert einen eigenen Punkt "point" mit "factor" auf der elliptischen Kurve.
        ' 	 *  Rekusieve Funkton, sehr rechenintensiev und daher sehr langsam!	
        Private Shared Function Multiply_Point(ByVal point As BigInteger(), ByVal factor As BigInteger) As BigInteger()
            Dim erg = point
            Dim NULL = New BigInteger(1) {}
            NULL(0) = ZERO
            NULL(1) = ZERO
            If factor = ZERO Then
                Return NULL
            End If
            If factor = ONE Then
                Return erg
            End If
            If factor = TWO Then
                Return Multiply_2(erg)
            End If
            If factor = THREE Then
                Return Addition(Multiply_2(erg), erg)
            End If
            If factor = FOUR Then
                Return Multiply_2(Multiply_2(erg))
            End If


            If factor.CompareTo(FOUR) = 1 Then ' : End If
                'If True Then
                Dim exp As Integer = Math_Modulo.BitLength(factor) - 1
                While exp > 0
                    erg = Multiply_2(erg)
                    exp -= 1
                End While
                factor = Math_Modulo.ClearBit(factor, Math_Modulo.BitLength(factor) - 1)
                erg = Addition(Multiply_Point(point, factor), erg)
            End If
            Return erg
        End Function

        '	Multiplikation auf der elliptischen Kurve mit 2  (Nur zur Vorberechnung, nicht zur laufzeit anwenden!)	m = (3*P[0]²)/(2*sqrt(P[0]²+7))
        '	n = P[1] - m*P[0];
        '	erg[0] = m² - 2*P[0]
        '	erg[1] = -(m*erg[0] + n)     
        Private Shared Function Multiply_2(ByVal P As BigInteger()) As BigInteger()
            Dim erg = New BigInteger(1) {}
            Dim m As BigInteger = Math_Modulo.Div(Math_Modulo.Mul(THREE, Math_Modulo.Pow(P(0), TWO)), Math_Modulo.Mul(TWO, Math_Modulo.Sqrt(Math_Modulo.Add(Math_Modulo.Pow(P(0), THREE), SEVEN))))
            If P(1).CompareTo(ModuloHalb) = 1 Then m = Math_Modulo.Neg(m)
            Dim n As BigInteger = Math_Modulo.Sub(P(1), Math_Modulo.Mul(m, P(0)))
            erg(0) = Math_Modulo.Sub(Math_Modulo.Pow(m, TWO), Math_Modulo.Mul(TWO, P(0)))
            erg(1) = Math_Modulo.Neg(Math_Modulo.Add(Math_Modulo.Mul(m, erg(0)), n))
            Return erg
        End Function

        ' *	Addiert ein Punkt P mit dem Punkt Q auf der elliptischen Kurve.
        ' 	*	m = (Q[1]-P[1])/(Q[0]-P[0])
        ' 	*	n = P[1] - m*P[0];
        ' 	*	x = m² - x1 -x2
        ' 	*	y = -(m*x + n)
        Private Shared Function Addition(ByVal po1 As BigInteger(), ByVal po2 As BigInteger()) As BigInteger()
            Dim nullVektor = New BigInteger(1) {}
            nullVektor(0) = BigInteger.Zero
            nullVektor(1) = BigInteger.Zero
            If po1(0) = ZERO AndAlso po1(1) = ZERO Then Return po2
            If po2(0) = ZERO AndAlso po2(1) = ZERO Then Return po1
            If po2(1) = po1(1) Then
                Return Multiply_2(po1)
            ElseIf po2(0) = po1(0) Then
                Return nullVektor
            End If
            Dim erg = New BigInteger(1) {}
            Dim m As BigInteger = Math_Modulo.Div(Math_Modulo.Sub(po2(1), po1(1)), Math_Modulo.Sub(po2(0), po1(0)))
            Dim n As BigInteger = Math_Modulo.Sub(po1(1), Math_Modulo.Mul(m, po1(0)))
            erg(0) = Math_Modulo.Sub(Math_Modulo.Sub(Math_Modulo.Mul(m, m), po1(0)), po2(0))
            erg(1) = Math_Modulo.Neg(Math_Modulo.Add(Math_Modulo.Mul(m, erg(0)), n))
            Return erg
        End Function

        ' *	Subtrahiert ein Punkt P mit dem Punkt Q auf der elliptischen Kurve.
        ' 	*	Wird nur zu Testzwecken benötigt.  
        Private Shared Function Subtraktion(ByVal p1 As BigInteger(), ByVal p2 As BigInteger()) As BigInteger()
            Dim y = New BigInteger(1) {}
            y(0) = p2(0)
            y(1) = Math_Modulo.Neg(p2(1))
            Return Addition(p1, y)
        End Function

        ' *	Dividiert P/Q auf der elliptischen Kurve
        ' 	 *  Wird nur zu Testzwecken benötigt.
        Private Shared Function Div(ByVal P As BigInteger(), ByVal Q As BigInteger) As BigInteger()
            Dim teiler As BigInteger = Math_Modulo.CalcHalb(Q)
            Return Multiply_Point(P, teiler)
        End Function

        ' *	Beschneidet ein ByteArray belibiger länge auf eine fest definierte Länge "len".
        ' 	*	- Wenn "data" kleiner als "len" ist wird es vorne mit Nullen aufgefüllt.
        ' 	*	- Wenn "data" länger als "len" ist, wird es hinten abgeschnitten.  
        Private Shared Function To_FixLength(ByVal data As Byte(), ByVal len As Integer) As Byte()
            If data.Length < len Then
                Dim result = New Byte(len - 1) {}
                Array.Copy(data, 0, result, len - data.Length, data.Length)
                Return result
            End If
            If data.Length > len Then Return data.Take(len).ToArray()
            Return data
        End Function


        ' -------------------------------------------- Test´s ---------------------------------- //

        ' * Gibt den Punkt Y von X zurück.  
        ' 	*	Da es immer zwei gültige Y-Werte zu einem X-Wert gibt ist die Berechnung nur zu 50% richtig. 
        ' 	*	Die Information um welchen Punkt es sich handelt kann die Methode nicht "erraten". 
        ' 	*	Daher werden beide möglichen Y-Werte in einem Array zurück gegeben.
        ' 	*	Y1 = sqrt(x³+7) ,  Y2 = -sqrt(x³+7) 
        Public Shared Function Y_Von_X(ByVal x As BigInteger) As BigInteger()
            Dim erg = New BigInteger(1) {}
            Try
                erg(0) = Math_Modulo.Sqrt(Math_Modulo.Add(Math_Modulo.Pow(x, THREE), SEVEN))
                erg(1) = Math_Modulo.Neg(Math_Modulo.Sqrt(Math_Modulo.Add(Math_Modulo.Pow(x, THREE), SEVEN)))
                Return erg
            Catch
                'Console.WriteLine("X-Koordinate trifft nicht auf die elliptische Kurve!")
                Return erg
            End Try
        End Function

        Private Shared Function CreatePrivateKey() As Byte()
            Dim bytes = New Byte(31) {}
            Do
                CRand.NextBytes(bytes)
                bytes(bytes.Length - 1) = 0
            Loop While Not IsInPrivKeyRange(bytes)

            Return BigInteger.ModPow(New BigInteger(bytes), 1, ORDNUNG).ToByteArray().Take(32).ToArray()
        End Function

        Public Shared Function CreatePublicKey(ByVal privkey As Byte()) As PublicKey
            If privkey.Length <> 32 Then
                'Throw New ArgumentOutOfRangeException(Nothing)
                Return New PublicKey(New BigInteger(0), New BigInteger(0))
            End If

            privkey = privkey.Reverse().ToArray

            If Not IsInPrivKeyRanges(privkey) Then
                'Throw New ArgumentOutOfRangeException(Nothing)
                Return New PublicKey(New BigInteger(0), New BigInteger(0))
            End If

            Dim k = ToBigInteger(privkey)
            Dim resultpoint = Multiply_G(k)
            Return New PublicKey(resultpoint)
        End Function

        Private Shared Function IsZero(Of T As Structure)(numbers As T()) As Boolean
            Return numbers.SequenceEqual(New T(numbers.Length - 1) {})
        End Function

        Private Shared Function IsLEOrdnung(numbers As Byte()) As Boolean
            Return New BigInteger(numbers) = BigInteger.ModPow(New BigInteger(numbers), 1, ORDNUNG)
        End Function

        Private Shared Function IsInPrivKeyRange(numbers As Byte()) As Boolean
            Return Not IsZero(numbers) AndAlso IsLEOrdnung(numbers)
        End Function

        Private Shared Function IsInPrivKeyRanges(privkey As Byte()) As Boolean
            Return Not IsZero(privkey) AndAlso IsLEOrdnung(ToBigInteger(CType(privkey, Byte())).ToByteArray())
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
