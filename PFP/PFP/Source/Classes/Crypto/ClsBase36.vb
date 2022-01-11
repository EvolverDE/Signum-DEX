
Option Strict On
Option Explicit On

Public Class ClsBase36

    Private Const Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"

    Public Shared Function EncodeHexToBase36(ByVal HexStr As String) As String

        Dim BigInt As System.Numerics.BigInteger = System.Numerics.BigInteger.Parse(HexStr, Globalization.NumberStyles.AllowHexSpecifier)

        Dim Charay As Char() = Chars.ToCharArray

        Dim ResultValue As String = ""

        If BigInt < 0 Then
            HexStr = "00" + HexStr
            BigInt = System.Numerics.BigInteger.Parse(HexStr, Globalization.NumberStyles.AllowHexSpecifier)
            BigInt = System.Numerics.BigInteger.Abs(BigInt)
        End If

        If (BigInt < 36) Then
            Dim CIdx As Integer = Convert.ToInt32(BigInt)
            ResultValue = Charay(CIdx)
        Else
            While (BigInt <> 0)
                Dim Remainder As Integer = Convert.ToInt32((BigInt Mod 36).ToString)
                ResultValue = Charay(Remainder) & ResultValue
                BigInt = BigInt / 36
            End While
        End If

        Return ResultValue

    End Function

    Public Shared Function DecodeBase36ToHex(ByVal Base36 As String) As String

        Dim ReturnValue As System.Numerics.BigInteger = 0
        Dim Negative As Boolean = False

        Base36 = Base36.ToUpper.Trim

        If (Base36.Contains("-")) AndAlso (Base36.Length > 1) Then
            Negative = True
        End If

        If Base36.IndexOfAny("$,+-".ToCharArray) > -1 Then
            Base36 = CleanValue(Base36)
        End If

        Base36 = TrimLeadingZeros(Base36)
        For i As Integer = 0 To Base36.Length - 1

            Dim Digit As Char = Convert.ToChar(Base36.Substring(i, 1))
            Dim Idx As Integer = Array.IndexOf(Chars.ToCharArray, Digit)

            Dim PlaceValue As Integer = Base36.Length - i - 1

            Dim Pow As System.Numerics.BigInteger = System.Numerics.BigInteger.Pow(36, PlaceValue)
            Dim PowIdx As System.Numerics.BigInteger = Pow * Idx

            Dim DigitValue As System.Numerics.BigInteger = PowIdx

            ReturnValue += DigitValue
        Next

        If Negative Then
            ReturnValue *= -1
        End If

        Dim BigByteArray As Byte() = ReturnValue.ToByteArray
        Dim PubKeyHex = ByteArrayToHEXString(BigByteArray.Reverse.ToArray)

        Return TrimLeadingZeros(PubKeyHex)

    End Function


    Public Shared Function CleanValue(ByVal DecimalValue As String) As String
        Dim ResultValue As String = DecimalValue

        If (ResultValue.IndexOfAny("$,+-".ToCharArray) > -1) Then
            ResultValue = ResultValue.Replace("$", "")
            ResultValue = ResultValue.Replace(",", "")
            ResultValue = ResultValue.Replace("+", "")
            ResultValue = ResultValue.Replace("-", "")
        End If
        Return ResultValue
    End Function

    Private Shared Function TrimLeadingZeros(ByVal Value As String) As String

        Dim RemoveIdx As Integer = -1
        For i As Integer = 0 To Value.Length - 1

            Dim Zero As String = Value.Substring(i, 1)

            If Zero = "0" Then
                RemoveIdx = i
            Else
                Exit For
            End If

        Next

        If Not RemoveIdx = -1 Then
            Value = Value.Substring(RemoveIdx + 1)
        End If

        Return Value

    End Function


End Class
