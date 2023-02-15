Public Class ClsOutput
    Inherits AbsClsOutputs

    ReadOnly Property ChangeOutput As Boolean = False

    Sub New()

    End Sub

    Sub New(ByVal Script As List(Of ClsScriptEntry))
        Me.Script = Script
        ConvertScriptToHex()
    End Sub

    'Property C_Addresses As List(Of String) = New List(Of String)

    Sub New(ByVal RecipientAddress As String, ByVal Amount As Double)
        Me.New(RecipientAddress, Amount, False)
    End Sub

    ''' <summary>
    ''' Create a Standard Script
    ''' </summary>
    ''' <param name="RecipientAddress">the Address of the recipient address</param>
    ''' <param name="Amount">the amount in BTC</param>
    Sub New(ByVal RecipientAddress As String, ByVal Amount As Double, ByVal Change As Boolean)
        Me.Addresses.Add(RecipientAddress)
        Me.OutputType = E_Type.Standard
        Me.ChangeOutput = Change
        Me.AmountNQT = Dbl2Satoshi(Amount)
        CreateStandardScript(AddressToRipe160(RecipientAddress))
        ConvertScriptToHex()
    End Sub
    ''' <summary>
    ''' Create a ChainSwapHash Script
    ''' </summary>
    ''' <param name="ChainSwapHash">the chain swap hash to redeem the transaction output</param>
    ''' <param name="RecipientAddress">the Address of the recipient address</param>
    ''' <param name="Amount">the amount in BTC</param>
    Sub New(ByVal ChainSwapHash As String, ByVal RecipientAddress As String, ByVal Amount As Double)
        Me.Addresses.Add(RecipientAddress)
        Me.OutputType = E_Type.ChainSwapHash
        Me.AmountNQT = Dbl2Satoshi(Amount)
        CreateChainSwapScript(ChainSwapHash, AddressToRipe160(RecipientAddress))
        ConvertScriptToHex()
    End Sub
    ''' <summary>
    ''' Create a LockTime Script
    ''' </summary>
    ''' <param name="RecipientAddress">the Address of the recipient address</param>
    ''' <param name="Amount">the amount in BTC</param>
    ''' <param name="ScriptLockTime">the optional LockTime in Blocks as integer</param>
    Sub New(ByVal RecipientAddress As String, ByVal Amount As Double, Optional ByVal ScriptLockTime As Integer = 3)
        Me.Addresses.Add(RecipientAddress)
        Me.OutputType = E_Type.LockTime
        Me.AmountNQT = Dbl2Satoshi(Amount)
        CreateLockTimeScript(AddressToRipe160(RecipientAddress), ScriptLockTime)
        ConvertScriptToHex()
    End Sub
    ''' <summary>
    ''' Create a ChainSwapHash script with LockTime for payback option when the time is up
    ''' </summary>
    ''' <param name="RecipientAddress">the Address of the recipient address</param>
    ''' <param name="ChainSwapHash">the chain swap hash to redeem the transaction output</param>
    ''' <param name="SenderAddress">the Address of the sender address</param>
    ''' <param name="Amount">the amount in BTC</param>
    ''' <param name="ScriptLockTime">the optional LockTime in Blocks as integer</param>
    Sub New(ByVal RecipientAddress As String, ByVal ChainSwapHash As String, ByVal SenderAddress As String, ByVal Amount As Double, Optional ByVal ScriptLockTime As Integer = 3)
        Me.Addresses.AddRange({RecipientAddress, SenderAddress})
        Me.OutputType = E_Type.Pay2ScriptHash
        Me.AmountNQT = Dbl2Satoshi(Amount)
        CreateChainSwapWithLockTimeScript(AddressToRipe160(RecipientAddress), ChainSwapHash, AddressToRipe160(SenderAddress), ScriptLockTime)
        ConvertScriptToHex()

        Dim P2SH As String = PubKeyToRipe160(Me.ScriptHex)

        Dim HASH160 As ClsScriptEntry = New ClsScriptEntry(ClsScriptEntry.E_OP_Code.OP_HASH160)
        Dim EQUAL As ClsScriptEntry = New ClsScriptEntry(ClsScriptEntry.E_OP_Code.OP_EQUAL)

        P2SH = HASH160.ValueHex + IntToHex(Convert.ToInt32(P2SH.Length / 2)) + P2SH + EQUAL.ValueHex

        Me.ScriptHash = P2SH

    End Sub

    Private Sub CreateStandardScript(ByVal RIPE160Recipient As String)

        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_DUP)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_HASH160)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.RIPE160Recipient, RIPE160Recipient)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_EQUALVERIFY)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_CHECKSIG)

    End Sub
    Private Sub CreateChainSwapScript(ByVal ChainSwapHash As String, ByVal RIPE160Recipient As String)

        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_SHA256)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.ChainSwapHash, ChainSwapHash)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_EQUALVERIFY)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_DUP)

        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_HASH160)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.RIPE160Recipient, RIPE160Recipient)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_EQUALVERIFY)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_CHECKSIG)

    End Sub
    Private Sub CreateLockTimeScript(ByVal RIPE160Recipient As String, Optional ByVal ScriptLockTime As Integer = 3)

        If ScriptLockTime >= 127 Then
            ScriptLockTime = 127
        End If

        AddScriptEntry(ClsScriptEntry.E_OP_Code.LockTime, IntToHex(ScriptLockTime, 8, True))
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_CHECKSEQUENCEVERIFY)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_DROP)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_DUP)

        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_HASH160)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.RIPE160Recipient, RIPE160Recipient)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_EQUALVERIFY)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_CHECKSIG)

    End Sub
    Private Sub CreateChainSwapWithLockTimeScript(ByVal RIPE160Recipient As String, ByVal ChainSwapHash As String, ByVal RIPE160Sender As String, Optional ByVal ScriptLockTime As Integer = 3)

        If ScriptLockTime >= 127 Then
            ScriptLockTime = 127
        End If

        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_IF)

        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_SHA256)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.ChainSwapHash, ChainSwapHash)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_EQUALVERIFY)

        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_DUP)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_HASH160)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.RIPE160Recipient, RIPE160Recipient)

        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_ELSE)
        If ScriptLockTime >= 2 And ScriptLockTime <= 16 Then
            AddScriptEntry(ClsScriptEntry.E_OP_Code.LockTime, IntToHex(ScriptLockTime - 2 + 82, 1, False))
        ElseIf ScriptLockTime > 16 And ScriptLockTime <= 255 Then
            Dim PushData As String = "01" + IntToHex(ScriptLockTime, 1, False)
            AddScriptEntry(ClsScriptEntry.E_OP_Code.LockTime, PushData)
        ElseIf ScriptLockTime > 255 And ScriptLockTime <= 65535 Then
            Dim PushData As String = "02" + IntToHex(ScriptLockTime, 2, False)
            AddScriptEntry(ClsScriptEntry.E_OP_Code.LockTime, PushData)
        End If

        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_CHECKSEQUENCEVERIFY)

        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_DROP)

        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_DUP)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_HASH160)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.RIPE160Sender, RIPE160Sender)

        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_ENDIF)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_EQUALVERIFY)
        AddScriptEntry(ClsScriptEntry.E_OP_Code.OP_CHECKSIG)

    End Sub

    Private Sub AddScriptEntry(ByVal OP_Key As ClsScriptEntry.E_OP_Code)
        Me.Script.Add(New ClsScriptEntry(OP_Key))
    End Sub

    Private Sub AddScriptEntry(ByVal OP_Key As ClsScriptEntry.E_OP_Code, ByVal ValueHex As String)
        Me.Script.Add(New ClsScriptEntry(OP_Key, ValueHex))
    End Sub

    Private Sub ConvertScriptToHex()
        Dim T_Length As Integer = 0
        Dim T_Script As String = ""
        For Each ScriptEntry As ClsScriptEntry In Me.Script
            Dim T_Len As Integer = ScriptEntry.ValueHex.Length / 2
            T_Length += T_Len
            If T_Len > 1 Then
                T_Length += 1
                T_Script += IntToHex(ScriptEntry.ValueHex.Length / 2)
            End If
            T_Script += ScriptEntry.ValueHex
        Next
        Me.LengthOfScript = T_Length
        Me.ScriptHex += T_Script
    End Sub

End Class
