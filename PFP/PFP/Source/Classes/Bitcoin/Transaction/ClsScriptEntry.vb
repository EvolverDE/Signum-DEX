
Option Strict On
Option Explicit On

Imports System.Numerics

Public Class ClsScriptEntry

    Public Key As E_OP_Code
    'Property Key As E_OP_Code
    '    Get
    '        Return C_Key
    '    End Get
    '    Set(value As E_OP_Code)
    '        C_Key = value
    '    End Set
    'End Property

    Public ValueHex As String
    'Property ValueHex As String
    '    Get
    '        Return C_ValueHex
    '    End Get
    '    Set(value As String)
    '        C_ValueHex = value
    '    End Set
    'End Property

    Public Enum E_OP_Code

        'Constants
        OP_0 = 0
        OP_FALSE = 0

        OP_PUSHDATA1 = 76
        OP_PUSHDATA2 = 77
        OP_PUSHDATA4 = 78
        OP_1NEGATE = 79

        OP_1 = 81
        OP_TRUE = 81

        OP_2 = 82
        OP_3 = 83
        OP_4 = 84
        OP_5 = 85
        OP_6 = 86
        OP_7 = 87
        OP_8 = 88
        OP_9 = 89
        OP_10 = 90
        OP_11 = 91
        OP_12 = 92
        OP_13 = 93
        OP_14 = 94
        OP_15 = 95
        OP_16 = 96

        'Flow control
        OP_NOP = 97

        OP_IF = 99
        OP_NOTIF = 100

        OP_ELSE = 103
        OP_ENDIF = 104
        OP_VERIFY = 105
        OP_RETURN = 106

        'Stack
        OP_TOALTSTACK = 107
        OP_FROMALTSTACK = 108

        OP_IFDUP = 115
        OP_DEPTH = 116
        OP_DROP = 117
        OP_DUP = 118
        OP_NIP = 119
        OP_OVER = 120
        OP_PICK = 121
        OP_ROLL = 122
        OP_ROT = 123
        OP_SWAP = 124
        OP_TUCK = 125

        OP_2DROP = 109
        OP_2DUP = 110
        OP_3DUP = 111
        OP_2OVER = 112
        OP_2ROT = 113
        OP_2SWAP = 114

        'Splice
        'OP_CAT = 126
        'OP_SUBSTR = 127
        'OP_LEFT = 128
        'OP_RIGHT = 129
        OP_SIZE = 130

        'Bitwise logic
        'OP_INVERT = 131
        'OP_AND = 132
        'OP_OR = 133
        'OP_XOR = 134
        OP_EQUAL = 135
        OP_EQUALVERIFY = 136

        'Arithmetic
        OP_1ADD = 139
        OP_1SUB = 140
        'OP_2MUL = 141
        'OP_2DIV = 142
        OP_NEGATE = 143
        OP_ABS = 144
        OP_NOT = 145
        OP_0NOTEQUAL = 146
        OP_ADD = 147
        OP_SUB = 148
        'OP_MUL = 149
        'OP_DIV = 150
        'OP_MOD = 151
        'OP_LSHIFT = 152
        'OP_RSHIFT = 153
        OP_BOOLAND = 154
        OP_BOOLOR = 155
        OP_NUMEQUAL = 156
        OP_NUMEQUALVERIFY = 157
        OP_NUMNOTEQUAL = 158
        OP_LESSTHAN = 159
        OP_GREATERTHAN = 160
        OP_LESSTHANOREQUAL = 161
        OP_GREATERTHANOREQUAL = 162
        OP_MIN = 163
        OP_MAX = 164
        OP_WITHIN = 165

        'Crypto
        OP_RIPEMD160 = 166
        OP_SHA1 = 167
        OP_SHA256 = 168
        OP_HASH160 = 169
        OP_HASH256 = 170
        OP_CODESEPARATOR = 171
        OP_CHECKSIG = 172
        OP_CHECKSIGVERIFY = 173
        OP_CHECKMULTISIG = 174
        OP_CHECKMULTISIGVERIFY = 175

        'Locktime
        OP_CHECKLOCKTIMEVERIFY = 177
        'OP_NOP2 = 177
        OP_CHECKSEQUENCEVERIFY = 178
        'OP_NOP3 = 178

        'Pseudo-words
        OP_PUBKEYHASH = 253
        OP_PUBKEY = 254
        OP_INVALIDOPCODE = 255

        'Reserved words
        OP_RESERVED = 80
        OP_VER = 98
        OP_VERIF = 101
        OP_VERNOTIF = 102
        OP_RESERVED1 = 137
        OP_RESERVED2 = 138
        OP_NOP1 = 176

        OP_NOP4 = 179
        OP_NOP5 = 180
        OP_NOP6 = 181
        OP_NOP7 = 182
        OP_NOP8 = 183
        OP_NOP9 = 184
        OP_NOP10 = 185

        ScriptHash = -1
        RIPE160Sender = -2
        AddressSender = -3
        RIPE160Recipient = -4
        AddressRecipient = -5
        ChainSwapKey = -6
        ChainSwapHash = -7
        LockTime = -8
        Signature = -9
        PublicKey = -10
        DER_Prefix = -11
        R_Value = -12
        DER_Split = -13
        S_Value = -14
        DER_End = -15

        PUSHDATALENGTH = -16

        Value = -17

        Unknown = -999

    End Enum

    Sub New(ByVal OPCodeValue As Integer)
        Me.New(ConvertValueToE_OP_Code(OPCodeValue))
    End Sub

    Sub New(ByVal OPCode As E_OP_Code)
        Key = OPCode
        convertOPCodeToHex()
    End Sub

    Sub New(ByVal OPCode As E_OP_Code, ByVal Value As String)

        Key = OPCode
        convertOPCodeToHex()
        If CheckHex(Value) Then
            ValueHex = Value
        Else
            ValueHex = ""
        End If

    End Sub

    Private Function CheckHex(InputString As String) As Boolean
        Return MessageIsHEXString(InputString)
    End Function

    Private Sub convertOPCodeToHex()

        If Key < 0 Then

            Select Case Key
                Case E_OP_Code.DER_Prefix
                    ValueHex = "4730440220"
                Case E_OP_Code.DER_Split
                    ValueHex = "0220"
                Case E_OP_Code.DER_End
                    ValueHex = "01"
                Case Else
                    ValueHex = ""
            End Select

        Else
            ValueHex = IntToHex(Key)
        End If

    End Sub

    Private Function IntToHex(ByVal Int As Integer, Optional ByVal Length As Integer = -1, Optional BigEndian As Boolean = True) As String

        Dim BI As BigInteger = New BigInteger(Int)

        Return BigIntToHEX(BI, Length, BigEndian)

    End Function
    Private Function BigIntToHEX(ByVal BigInt As Numerics.BigInteger, Optional ByVal Length As Integer = -1, Optional BigEndian As Boolean = True) As String

        Dim HEXList As List(Of String) = New List(Of String)({"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f"})

        If BigEndian Then
            Return ChangeBaseSystem(BigInt, HEXList, Length)
        Else
            Return ChangeHEXStrEndian(ChangeBaseSystem(BigInt, HEXList, Length))
        End If

    End Function
    Private Function ChangeBaseSystem(ByVal Input As Numerics.BigInteger, ByVal BaseSystem As List(Of String), Optional Length As Integer = -1) As String

        Dim BS As ClsBaseX = New ClsBaseX(BaseSystem)
        Dim Converted As String = BS.BigIntToWordString(Input, "")

        If Converted.Trim = "" And Input.ToString = "0" Then
            Converted = Input.ToString()
        End If

        If Not Length = -1 Then
            If Converted.Length < Length Then
                Dim T_Len As Integer = Converted.Length

                For i As Integer = T_Len To Length - 1
                    Converted = "0" + Converted
                Next

            End If
        End If

        If Converted.Length Mod 2 <> 0 Then
            Converted = "0" + Converted
        End If

        Return Converted
    End Function
    Private Function ChangeHEXStrEndian(ByVal InputHEX As String) As String

        Dim StartEndian As List(Of Byte) = New List(Of Byte)(HEXStringToByteArray(InputHEX))
        StartEndian.Reverse()
        Dim EndEndian As String = ByteArrayToHEXString(StartEndian.ToArray)

        Return EndEndian

    End Function

    Public Shared Function GetListOfE_OP_Code() As List(Of E_OP_Code)

        Dim T_EnumList As List(Of E_OP_Code) = New List(Of E_OP_Code)

        For Each T_OP_Code As E_OP_Code In System.Enum.GetValues(GetType(E_OP_Code))
            T_EnumList.Add(T_OP_Code)
        Next

        Return T_EnumList

    End Function

    Public Shared Function ConvertValueToE_OP_Code(ByVal Value As Integer) As E_OP_Code

        If Is_E_OP_Code(Value) Then
            Dim T_EnumList As List(Of E_OP_Code) = GetListOfE_OP_Code()

            For Each T_Enum As E_OP_Code In T_EnumList

                If T_Enum = Value Then
                    Return T_Enum
                End If

            Next

        End If

        Return E_OP_Code.Unknown

    End Function

    Public Shared Function Is_E_OP_Code(ByVal Value As Integer) As Boolean
        Return System.Enum.IsDefined(GetType(E_OP_Code), Value)
    End Function

End Class
