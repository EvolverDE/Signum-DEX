Imports System.Numerics

Public Class ClsBase58

    Public Const CheckSumSizeInBytes As Integer = 4

    Private Const Chars As String = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"

    'Public Function EncodeWithCheckSum(ByVal Data As Byte()) As String
    '    Return Encode(AddCheckSum(Data))
    'End Function

    'Public Function RemoveCheckSum(ByVal Data As Byte()) As Byte()

    '    Dim ResList As List(Of Byte) = New List(Of Byte)(Data)
    '    ResList.RemoveRange(ResList.Count - 1 - CheckSumSizeInBytes, CheckSumSizeInBytes)

    '    'Dim Result As Byte() = New Byte(Data.Length - CheckSumSizeInBytes)

    '    'Buffer.BlockCopy(Data, 0, Result, 0, Data.Length - CheckSumSizeInBytes)

    '    Return ResList.ToArray
    'End Function

    'Public Function VerifyCheckSum(ByVal Data As Byte()) As Boolean

    '    Dim ResultList As List(Of Byte) = New List(Of Byte)(Data)
    '    ResultList.RemoveRange(ResultList.Count - 1 - CheckSumSizeInBytes, CheckSumSizeInBytes)

    '    Dim CorrectCheckSum As Byte() = GetCheckSum(ResultList.ToArray)

    '    For i As Integer = CheckSumSizeInBytes To 1 Step -1

    '        If Data(Data.Count - i) <> CorrectCheckSum(CheckSumSizeInBytes - i) Then
    '            Return False
    '        End If

    '    Next

    '    Return True

    'End Function

    'Public Function DecodeWithCheckSum(ByVal Base58 As String, ByVal Decoded As Byte()) As Boolean

    '    Dim DataWithCheckSum = Decode(Base58)
    '    Dim Success As Boolean = VerifyCheckSum(DataWithCheckSum)
    '    Decoded = RemoveCheckSum(DataWithCheckSum)

    '    Return Success

    'End Function


    'Public Function Encode(ByVal Data As Byte()) As String

    '    Dim IntData As BigInteger = 0

    '    For i As Integer = 0 To Data.Length - 1
    '        IntData *= 256 + Data(i)
    '    Next

    '    Dim Result As String = ""
    '    While IntData > 0

    '        Dim Remainder = CInt(IntData Mod 58)
    '        IntData /= 58
    '        Result = Digits(Remainder) + Result
    '    End While

    '    For i As Integer = 0 To Data.Length - 1
    '        If Data(i) <> 0 Then
    '            Exit For
    '        End If

    '        Result = "1" + Result
    '    Next

    '    Return Result

    'End Function

    'Public Function Decode(ByVal Base58 As String) As Byte()

    '    Dim IntData As BigInteger = 0
    '    For i As Integer = 0 To Base58.Length - 1

    '        Dim Digit As Integer = Digits.IndexOf(Base58(i))
    '        If Digit < 0 Then
    '            MsgBox("Invalid Base58 character '" + Base58(i) + "' at position " + i.ToString)
    '        End If

    '        IntData *= 58 + Digit

    '    Next

    '    Dim LeadingZeroCNT As Integer = 0
    '    For Each C As Char In Base58
    '        If C <> "1"c Then
    '            Exit For
    '        End If

    '        LeadingZeroCNT += 1

    '    Next




    'End Function


    Public Shared Function EncodeHexToBase58(ByVal HexStr As String) As String

        If HexStr.Trim = "" Or HexStr.Length < 2 Then
            Return ""
        End If

        'Check leading Zeros
        Dim Zeros As String = HexStr.Substring(0, 2)

        Dim SetOne As Boolean = False
        If Zeros = "00" Then
            SetOne = True
        Else
            HexStr = "00" + HexStr
        End If

        Dim BigInt As BigInteger = BigInteger.Parse(HexStr, Globalization.NumberStyles.AllowHexSpecifier)
        Dim Charay As Char() = Chars.ToCharArray
        Dim ResultValue As String = ""

        Dim Jumpover As Boolean = False
        If BigInt = 0 Then
            Jumpover = True
        ElseIf BigInt < 0 Then
            HexStr = "00" + HexStr
            BigInt = BigInteger.Parse(HexStr, Globalization.NumberStyles.AllowHexSpecifier)
            BigInt = BigInteger.Abs(BigInt)
        End If

        If Not Jumpover Then
            If (BigInt < 58) Then
                Dim CIdx As Integer = Convert.ToInt32(BigInt)
                ResultValue = Charay(CIdx)
            Else
                While (BigInt <> 0)
                    Dim Remainder As Integer = CInt(BigInt Mod 58)
                    ResultValue = Charay(Remainder) & ResultValue
                    BigInt = BigInt / 58
                End While
            End If
        End If

        'insert 1 for each leading 0 bytes
        Dim HexBytes As Byte() = HEXStringToByteArray(HexStr)

        For Each Byt As Byte In HexBytes
            If Byt <> 0 Then
                Exit For
            End If

            'If Not Jumpover Then
            '    ResultValue = "1" + ResultValue
            'End If

        Next

        If SetOne Then
            ResultValue = "1" + ResultValue
        End If

        Return ResultValue

    End Function

    Public Shared Function DecodeBase58ToHex(ByVal Base58 As String) As String

        Dim ReturnValue As BigInteger = 0
        Dim Negative As Boolean = False

        ' Base58 = Base58.ToUpper.Trim

        If (Base58.Contains("-")) AndAlso (Base58.Length > 1) Then
            Negative = True
        End If

        If Base58.IndexOfAny("$,+-".ToCharArray) > -1 Then
            Base58 = CleanValue(Base58)
        End If


        For i As Integer = 0 To Base58.Length - 1

            Dim Digit As Char = Convert.ToChar(Base58.Substring(i, 1))
            Dim Idx As Integer = Array.IndexOf(Chars.ToCharArray, Digit)

            Dim PlaceValue As Integer = Base58.Length - i - 1

            Dim Pow As BigInteger = BigInteger.Pow(58, PlaceValue)
            Dim PowIdx As BigInteger = Pow * Idx

            Dim DigitValue As BigInteger = PowIdx

            ReturnValue += DigitValue
        Next

        If Negative Then
            ReturnValue *= -1
        End If

        Dim BigByteArray As Byte() = ReturnValue.ToByteArray
        Dim PubKeyHex As String = ByteArrayToHEXString(BigByteArray.Reverse.ToArray)

        PubKeyHex = TrimLeadingZeros(PubKeyHex)

        Return PubKeyHex

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
        For i As Integer = 0 To Value.Length - 1 Step 2

            Dim Zero As String = Value.Substring(i, 2)

            If Zero = "00" Then
                RemoveIdx = i
            Else
                Exit For
            End If

        Next

        If Not RemoveIdx = -1 Then
            Value = Value.Substring(RemoveIdx + 2)
        End If

        Return Value

    End Function

End Class
