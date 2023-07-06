
Option Explicit On
Option Strict On
Imports System.Security.Cryptography

Public Class ClsTransaction

    Private Version As Integer = 2
    'Public ReadOnly Property NumberOfInputs As Integer
    '    Get
    '        Return C_Inputs.Count
    '    End Get
    'End Property

    Public ReadOnly Property TotalInputAmountNQT As ULong
        Get
            Dim T_Amount As ULong = 0UL
            For Each T_Input As ClsUnspentOutput In C_Inputs
                T_Amount += T_Input.AmountNQT
            Next
            Return T_Amount
        End Get
    End Property

    Public ReadOnly Property TotalOutputAmountNQT As ULong
        Get
            Dim T_Amount As ULong = 0UL
            For Each T_Output As ClsOutput In C_Outputs
                T_Amount += T_Output.AmountNQT
            Next
            Return T_Amount
        End Get
    End Property

    Public ReadOnly Property TotalAmountDifferenceNQT As ULong
        Get
            Dim T_InputAmountNQT As ULong = TotalInputAmountNQT
            Dim T_OutputAmountNQT As ULong = TotalOutputAmountNQT

            Dim T_DiffAmountNQT As ULong = 0UL

            If TotalInputAmountNQT > TotalOutputAmountNQT Or TotalInputAmountNQT > 0UL Then
                T_DiffAmountNQT = TotalInputAmountNQT - TotalOutputAmountNQT
            End If

            Return T_DiffAmountNQT
        End Get
    End Property

    'Property C_ChangeAmountNQT As ULong
    Property C_FeesNQTPerByte As ULong = 1UL

    Private Property C_ScriptAddresses As List(Of String) = New List(Of String)

    Private C_Inputs As List(Of ClsUnspentOutput) = New List(Of ClsUnspentOutput)
    Public ReadOnly Property Inputs As List(Of ClsUnspentOutput)
        Get
            Return C_Inputs
        End Get
    End Property

    'Public ReadOnly Property NumberOfOutputs As Integer
    '    Get
    '        Return C_Outputs.Count
    '    End Get
    'End Property

    Private Property C_Outputs As List(Of ClsOutput) = New List(Of ClsOutput)
    Public ReadOnly Property Outputs As List(Of ClsOutput)
        Get
            Return C_Outputs
        End Get
    End Property

    Private Property C_TransactionType As E_TransactionType = E_TransactionType.None

    'Public ReadOnly Property TransactionType As E_TransactionType
    '    Get
    '        Return C_TransactionType
    '    End Get
    'End Property

    Private HashCodeType As Integer = 1
    Private Property OutputLockTime As Integer = 0

    Public Property SignedTransactionHEX As String = ""

    Public Structure S_PrivateKey
        Dim PrivateKey As String
        Dim ChainSwapKey As String
    End Structure

    Public Enum E_TransactionType
        None = 0
        Create = 1
        Redeem = 2
        GetBack = 3
    End Enum

    Public Structure S_TXEntry
        Dim Key As E_TXEntry
        Dim OP_CODE As ClsScriptEntry.E_OP_Code
        Dim Value As String
    End Structure

    Public Enum E_TXEntry
        version = 0
        numberOfInputs = 1
        transactionID = 2
        unspentOutputIndex = 3
        inputScriptLength = 4
        inputScript = 5
        sequence = 6
        numberOfOutputs = 7
        amountNQT = 8
        outputScriptLength = 9
        outputScript = 10
        locktime = 11
        hashCodeType = 12
        signatureLength = -1
        signature = -2
        recipientPublicKey = -3
        OP_CODE = -4
    End Enum

    Private Property C_signedTransaction As List(Of S_TXEntry) = New List(Of S_TXEntry)
    'Public ReadOnly Property SignedTransaction As List(Of S_TXEntry)
    '    Get
    '        Return C_signedTransaction
    '    End Get
    'End Property

    Private Property AddressesFiltered As Boolean = False

    Sub New(ByVal TransactionID As String, ByVal FilterAddress As String, Optional ByVal RedeemScript As String = "")
        Me.New(TransactionID, New List(Of String)({FilterAddress}), RedeemScript)
    End Sub

    Sub New(ByVal TransactionID As String, ByVal FilterAddresses As List(Of String), Optional ByVal RedeemScript As String = "")
        Me.New(New List(Of String)({TransactionID}), FilterAddresses, RedeemScript)
    End Sub
    Sub New(ByVal TransactionIDs As List(Of String), ByVal Addresses As List(Of String), Optional ByVal RedeemScript As String = "")

        For Each T_TransactionID As String In TransactionIDs
            GetTransactionDetails(T_TransactionID, RedeemScript)
        Next

        If Addresses.Count > 0 Then
            FilterAddresses(Addresses)
            If C_Inputs.Count > 0 Then
                C_ScriptAddresses.AddRange(Addresses)
            End If

        Else
            C_TransactionType = E_TransactionType.Create

            For Each T_Input As ClsUnspentOutput In C_Inputs

                'prevent double addresses
                For Each Address As String In T_Input.Addresses

                    Dim AddrFound As Boolean = False
                    If C_ScriptAddresses.Contains(Address) Then
                        AddrFound = True
                    End If

                    If Not AddrFound Then
                        C_ScriptAddresses.Add(Address)
                    End If

                Next

                'C_ScriptAddresses.AddRange(T_Input.Addresses)
            Next

        End If

    End Sub

    Sub New(ByVal UTXOs As List(Of ClsBitcoinNET.S_UnspentTransactionOutput))
        'AtomicSwap: use Redeemscript instead of scriptPubKey

        For i As Integer = 0 To UTXOs.Count - 1
            Dim UTXO As ClsBitcoinNET.S_UnspentTransactionOutput = UTXOs(i)
            AddUnspentTransactionOutput(UTXO)
        Next

    End Sub

    'Sub New(ByVal UTXO As ClsBitcoinNET.S_UnspentTransactionOutput)
    '    AddUnspentTransactionOutput(UTXO)
    'End Sub

    Private Sub AddUnspentTransactionOutput(ByVal UTXO As ClsBitcoinNET.S_UnspentTransactionOutput)

        Dim Output As ClsOutput = New ClsOutput(UTXO.LockingScript)

        'prevent double addresses
        For Each Address As String In UTXO.Addresses

            Dim AddrFound As Boolean = False
            If C_ScriptAddresses.Contains(Address) Then
                AddrFound = True
            End If

            If Not AddrFound Then
                C_ScriptAddresses.Add(Address)
            End If

        Next

        'C_ScriptAddresses.AddRange(UTXO.Addresses)

        Dim T_PrevTX As New ClsUnspentOutput With {
            .TransactionID = UTXO.TransactionID,
            .InputIndex = UTXO.VoutIDX,
            .OutputType = UTXO.Typ,
            .Confirmations = UTXO.Confirmations,
            .Script = UTXO.LockingScript,
            .ScriptHex = Output.ScriptHex,
            .Addresses = UTXO.Addresses,
            .AmountNQT = UTXO.AmountNQT,
            .LengthOfScript = Output.LengthOfScript
        }

        If UTXO.Typ = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime Or UTXO.Typ = AbsClsOutputs.E_Type.LockTime Then
            Dim SequenceStr As String = ClsBitcoinNET.GetXFromScript(UTXO.LockingScript, ClsScriptEntry.E_OP_Code.LockTime)
            Dim Sequence As ULong = 0UL

            If Not SequenceStr.Trim = "" Then
                Sequence = Convert.ToUInt64(BitConverter.ToUInt32(HEXStringToByteArray(ChangeHEXStrEndian(SequenceStr)), 0))
            End If
            T_PrevTX.Sequence = Sequence
        End If

        '.SignatureScript = New List(Of ClsScriptEntry),
        '.SignerAddress = "",
        '.UnsignedTransactionHex = ""

        C_Inputs.Add(T_PrevTX)
    End Sub

    Private Sub GetTransactionDetails(ByVal TransactionID As String, Optional ByVal RedeemScript As String = "")

        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()

        Dim UTXOList As List(Of String) = BitNET.GetTXDetails(TransactionID)

        If UTXOList.Count = 0 Then
            Exit Sub
        End If

        For Each XMLEntry As String In UTXOList

            Dim T_PrevTX As ClsUnspentOutput = New ClsUnspentOutput
            T_PrevTX.TransactionID = TransactionID
            T_PrevTX.OutputType = AbsClsOutputs.E_Type.Unknown
            T_PrevTX.Confirmations = -1

            T_PrevTX.AmountNQT = Convert.ToUInt64(GetStringBetween(XMLEntry, "<value>", "</value>").Replace(",", "").Replace(".", ""))
            T_PrevTX.InputIndex = GetIntegerBetween(XMLEntry, "<n>", "</n>")

            Dim T_LockingScript As String = GetStringBetween(XMLEntry, "<hex>", "</hex>")
            T_PrevTX.ScriptHex = T_LockingScript
            T_PrevTX.LengthOfScript = Convert.ToInt32(T_LockingScript.Length / 2)
            T_PrevTX.Script = ConvertLockingScriptStrToList(T_LockingScript)
            T_PrevTX.OutputType = GetScriptType(T_LockingScript)

            If Not RedeemScript.Trim = "" Then

                Dim T_RedeemScriptHash As String = PubKeyToRipe160(RedeemScript)

                If T_LockingScript.Contains(T_RedeemScriptHash) Then
                    If T_PrevTX.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then
                        T_PrevTX.LengthOfScript = Convert.ToInt32(RedeemScript.Length / 2)
                        T_PrevTX.Script = ConvertLockingScriptStrToList(RedeemScript)
                        Dim T_SenderAddress As String = ClsBitcoinNET.GetXFromScript(T_PrevTX.Script, ClsScriptEntry.E_OP_Code.RIPE160Sender)
                        T_SenderAddress = RIPE160ToAddress(T_SenderAddress, BitcoinAddressPrefix)
                        Dim T_RecipientAddress As String = ClsBitcoinNET.GetXFromScript(T_PrevTX.Script, ClsScriptEntry.E_OP_Code.RIPE160Recipient)
                        T_RecipientAddress = RIPE160ToAddress(T_RecipientAddress, BitcoinAddressPrefix)

                        T_PrevTX.Addresses = New List(Of String)({T_RecipientAddress, T_SenderAddress})

                        T_PrevTX.ScriptHex = RedeemScript
                        T_PrevTX.ScriptHash = PubKeyToRipe160(RedeemScript)
                    End If
                End If

            End If

            If T_PrevTX.Addresses.Count = 0 Then
                T_PrevTX.Addresses.Add(GetStringBetween(XMLEntry, "<address>", "</address>"))
            End If

            If T_PrevTX.OutputType = AbsClsOutputs.E_Type.Unknown Then

                Dim T_Type As String = GetStringBetween(XMLEntry, "<type>", "</type>")

                If T_Type = "pubkeyhash" Then
                    T_PrevTX.OutputType = AbsClsOutputs.E_Type.Standard
                End If

            End If

            Dim TempUTXO As String = BitNET.GetTXOut(TransactionID, T_PrevTX.InputIndex)

            If Not TempUTXO.Trim = "" Then
                T_PrevTX.Spendable = True
                T_PrevTX.Confirmations = GetIntegerBetween(TempUTXO, "<confirmations>", "</confirmations>")
            Else
                T_PrevTX.Spendable = False
                T_PrevTX.Confirmations = -1
            End If

            C_Inputs.Add(T_PrevTX)

        Next

    End Sub

    ''' <summary>
    ''' Create a Standard Output
    ''' </summary>
    ''' <param name="RecipientAddress">the Address of the recipient address</param>
    ''' <param name="Amount">the amount in BTC</param>
    Public Sub CreateOutput(ByVal RecipientAddress As String, ByVal Amount As Double)
        Dim T_Output As ClsOutput = New ClsOutput(RecipientAddress, Amount)
        CreateOutput(T_Output)
    End Sub

    ''' <summary>
    ''' Create a ChainSwapHash Output
    ''' </summary>
    ''' <param name="ChainSwapHash">the chain swap hash to redeem the transaction output</param>
    ''' <param name="RecipientAddress">the Address of the recipient address</param>
    ''' <param name="Amount">the amount in BTC</param>
    Public Sub CreateOutput(ByVal ChainSwapHash As String, ByVal RecipientAddress As String, ByVal Amount As Double)
        Dim T_Output As ClsOutput = New ClsOutput(ChainSwapHash, RecipientAddress, Amount)
        CreateOutput(T_Output)
    End Sub

    ''' <summary>
    ''' Create a LockTime Output
    ''' </summary>
    ''' <param name="RecipientAddress">the Address of the recipient address</param>
    ''' <param name="Amount">the amount in BTC</param>
    ''' <param name="ScriptLockTime">the optional LockTime in Blocks as integer</param>
    Public Sub CreateOutput(ByVal RecipientAddress As String, ByVal Amount As Double, Optional ByVal ScriptLockTime As Integer = 12)
        Dim T_Output As ClsOutput = New ClsOutput(RecipientAddress, Amount, ScriptLockTime)
        'AtomicSwap: P2SH
        CreateOutput(T_Output)
    End Sub

    ''' <summary>
    ''' Create a ChainSwapHash Output with LockTime for payback option when the time is up
    ''' </summary>
    ''' <param name="RecipientAddress">the Address of the recipient address</param>
    ''' <param name="ChainSwapHash">the chain swap hash to redeem the transaction output</param>
    ''' <param name="SenderAddress">the Address of the sender address</param>
    ''' <param name="Amount">the amount in BTC</param>
    ''' <param name="ScriptLockTime">the optional LockTime in Blocks as integer</param>
    Public Sub CreateOutput(ByVal RecipientAddress As String, ByVal ChainSwapHash As String, ByVal SenderAddress As String, ByVal Amount As Double, Optional ByVal ScriptLockTime As Integer = 12)
        Dim T_Output As ClsOutput = New ClsOutput(RecipientAddress, ChainSwapHash, SenderAddress, Amount, ScriptLockTime)
        'AtomicSwap: P2SH

        CreateOutput(T_Output)

    End Sub

    Private Function CreateOutput(ByVal Output As ClsOutput) As Boolean

        If TotalAmountDifferenceNQT = 0UL Or TotalAmountDifferenceNQT < Output.AmountNQT Then
            Return False
        End If

        C_Outputs.Add(Output)

        Return True

    End Function

    Public Sub FinalizingOutputs(Optional ByVal ChangeAddress As String = "")

        If ChangeAddress.Trim = "" Then
            ChangeAddress = GetBitcoinMainAddress()
        End If

        For i As Integer = 0 To C_Outputs.Count - 1

            Dim op As ClsOutput = C_Outputs(i)

            If op.OutputType = AbsClsOutputs.E_Type.Standard Then
                If op.Addresses.Count = 1 Then
                    If op.Addresses.Contains(ChangeAddress) Then
                        op.AmountNQT += TotalAmountDifferenceNQT
                        C_Outputs(i) = op
                        Exit For
                    End If
                End If
            End If

        Next

        If TotalAmountDifferenceNQT > 0UL Then
            Dim T_OutputChange As ClsOutput = New ClsOutput(ChangeAddress, Satoshi2Dbl(TotalAmountDifferenceNQT), True)
            C_Outputs.Add(T_OutputChange)
        End If

        If C_ScriptAddresses.Count > 0 Then
            CreateUnsignedTransaction(C_ScriptAddresses)
        End If

    End Sub

    Private Sub FilterAddresses(ByVal Addresses As List(Of String))

        'Dim UnsignedInputs As List(Of ClsUnspentOutput) = New List(Of ClsUnspentOutput)

        Dim T_UnsignedInputs As List(Of ClsUnspentOutput) = New List(Of ClsUnspentOutput)

        For i As Integer = 0 To C_Inputs.Count - 1
            Dim T_Input As ClsUnspentOutput = C_Inputs(i)

            If T_Input.OutputType = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime Then

                Dim T_SenderAddress As String = RIPE160ToAddress(ClsBitcoinNET.GetXFromScript(T_Input.Script, ClsScriptEntry.E_OP_Code.RIPE160Sender), BitcoinAddressPrefix)
                Dim T_RecipientAddress As String = RIPE160ToAddress(ClsBitcoinNET.GetXFromScript(T_Input.Script, ClsScriptEntry.E_OP_Code.RIPE160Recipient), BitcoinAddressPrefix)

                Dim IsSender As Boolean = Addresses.Contains(T_SenderAddress)
                Dim IsRecipient As Boolean = Addresses.Contains(T_RecipientAddress)

                If IsSender And Not IsRecipient Then
                    C_TransactionType = E_TransactionType.GetBack
                ElseIf Not IsSender And IsRecipient Then
                    C_TransactionType = E_TransactionType.Redeem
                Else
                    'vollerror
                End If

            ElseIf T_Input.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then

                T_UnsignedInputs.Add(T_Input)
                Continue For
            End If

            For Each FA As String In Addresses

                If T_Input.Addresses.Contains(FA) Then
                    T_UnsignedInputs.Add(T_Input)
                    Exit For
                End If
            Next

        Next

        C_Inputs = T_UnsignedInputs

        AddressesFiltered = True

    End Sub

    Private Sub CreateUnsignedTransaction(ByVal Addresses As List(Of String))

        If C_Inputs.Count = 0 Or C_Outputs.Count = 0 Then
            Exit Sub
        End If


        For i As Integer = 0 To C_Inputs.Count - 1
            Dim T_Input As ClsUnspentOutput = C_Inputs(i)

            Dim T_UTX As List(Of ClsUnspentOutput.S_UTXEntry) = New List(Of ClsUnspentOutput.S_UTXEntry)

            Dim T_version As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
            T_version.Key = ClsUnspentOutput.E_UTXEntry.version
            T_version.Value = IntToHex(Version, 8, False)
            T_UTX.Add(T_version)

            Dim T_UnsignedTXHEX As String = T_version.Value ' IntToHex(Version, 8, False)

            Dim T_numberOfInputs As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
            T_numberOfInputs.Key = ClsUnspentOutput.E_UTXEntry.numberOfInputs
            T_numberOfInputs.Value = IntToHex(C_Inputs.Count)
            T_UTX.Add(T_numberOfInputs)

            T_UnsignedTXHEX += T_numberOfInputs.Value ' IntToHex(C_Inputs.Count)


            'get Locktime

            Dim T_Sequence As String = "ffffffff"

            'For j As Integer = 0 To C_Outputs.Count - 1

            '    Dim T_output As ClsOutput = C_Outputs(j)

            '    If T_output.OutputType = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime Or T_output.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then

            '        T_Sequence = ClsBitcoinNET.GetXFromScript(T_output.Script, ClsScriptEntry.E_OP_Code.LockTime)
            '        'T_Sequence = ChangeHEXStrEndian(T_Sequence)
            '        Exit For

            '    End If

            'Next


            For j As Integer = 0 To C_Inputs.Count - 1
                Dim T_Unsignedinput As ClsUnspentOutput = C_Inputs(j)

                Dim T_transactionID As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
                T_transactionID.Key = ClsUnspentOutput.E_UTXEntry.transactionID
                T_transactionID.Value = ChangeHEXStrEndian(T_Unsignedinput.TransactionID)
                T_UTX.Add(T_transactionID)

                T_UnsignedTXHEX += T_transactionID.Value ' ChangeHEXStrEndian(T_Unsignedinput.TransactionID)


                Dim T_inputIndex As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
                T_inputIndex.Key = ClsUnspentOutput.E_UTXEntry.unspentOutputIndex
                T_inputIndex.Value = IntToHex(T_Unsignedinput.InputIndex, 8, False)
                T_UTX.Add(T_inputIndex)

                T_UnsignedTXHEX += T_inputIndex.Value ' IntToHex(T_Unsignedinput.InputIndex, 8, False)

                If i = j Then

                    'If T_Input.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash And RedeemScript.Trim <> "" And T_Unsignedinput.ScriptHash = PubKeyToRipe160(RedeemScript) Then
                    '    T_UnsignedTXHEX += IntToHex(Convert.ToInt32(RedeemScript.Length / 2))
                    '    T_UnsignedTXHEX += RedeemScript
                    'Else

                    Dim T_inputScriptLength As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
                    T_inputScriptLength.Key = ClsUnspentOutput.E_UTXEntry.inputScriptLength
                    T_inputScriptLength.Value = IntToHex(T_Unsignedinput.LengthOfScript)
                    T_UTX.Add(T_inputScriptLength)

                    T_UnsignedTXHEX += T_inputScriptLength.Value ' IntToHex(T_Unsignedinput.LengthOfScript)

                    Dim T_inputScript As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
                    T_inputScript.Key = ClsUnspentOutput.E_UTXEntry.inputScript
                    T_inputScript.Value = T_Unsignedinput.ScriptHex
                    T_UTX.Add(T_inputScript)

                    T_UnsignedTXHEX += T_inputScript.Value ' T_Unsignedinput.ScriptHex
                    'End If

                Else

                    Dim T_inputScriptLength As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
                    T_inputScriptLength.Key = ClsUnspentOutput.E_UTXEntry.inputScriptLength
                    T_inputScriptLength.Value = "00"
                    T_UTX.Add(T_inputScriptLength)

                    T_UnsignedTXHEX += "00"
                End If


                'AtomicSwap: bitcoin locktime
                Dim T_seq As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
                T_seq.Key = ClsUnspentOutput.E_UTXEntry.sequence

                If T_Unsignedinput.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then

                    Dim T_timeout As String = ClsBitcoinNET.GetXFromScript(T_Unsignedinput.Script, ClsScriptEntry.E_OP_Code.LockTime)

                    Dim LTBytes As List(Of Byte) = New List(Of Byte)(HEXStringToByteArray(T_timeout))

                    If LTBytes.Count < 4 Then
                        For ii As Integer = LTBytes.Count To 3
                            LTBytes.Add(0)
                        Next
                    End If

                    T_timeout = ByteArrayToHEXString(LTBytes.ToArray)

                    T_seq.Value = T_timeout
                    T_UnsignedTXHEX += T_timeout
                Else

                    T_seq.Value = T_Sequence
                    T_UnsignedTXHEX += T_Sequence
                End If

                T_UTX.Add(T_seq)

            Next

            Dim T_numberOfOutputs As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
            T_numberOfOutputs.Key = ClsUnspentOutput.E_UTXEntry.numberOfOutputs
            T_numberOfOutputs.Value = IntToHex(C_Outputs.Count)
            T_UTX.Add(T_numberOfOutputs)

            T_UnsignedTXHEX += T_numberOfOutputs.Value ' IntToHex(C_Outputs.Count)

            For j As Integer = 0 To C_Outputs.Count - 1
                Dim T_Output As ClsOutput = C_Outputs(j)

                Dim T_AmountNQT As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
                T_AmountNQT.Key = ClsUnspentOutput.E_UTXEntry.amountNQT
                T_AmountNQT.Value = ULongToHex(T_Output.AmountNQT, 16, False)
                T_UTX.Add(T_AmountNQT)

                T_UnsignedTXHEX += T_AmountNQT.Value ' ULongToHex(T_Output.AmountNQT, 16, False)

                If T_Output.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then

                    Dim T_outputScriptLength As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
                    T_outputScriptLength.Key = ClsUnspentOutput.E_UTXEntry.outputScriptLength
                    T_outputScriptLength.Value = IntToHex(Convert.ToInt32(T_Output.ScriptHash.Length / 2))
                    T_UTX.Add(T_outputScriptLength)

                    T_UnsignedTXHEX += T_outputScriptLength.Value ' IntToHex(Convert.ToInt32(T_Output.ScriptHash.Length / 2))

                    Dim T_outputScript As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
                    T_outputScript.Key = ClsUnspentOutput.E_UTXEntry.outputScript
                    T_outputScript.Value = T_Output.ScriptHash
                    T_UTX.Add(T_outputScript)

                    T_UnsignedTXHEX += T_outputScript.Value ' T_Output.ScriptHash

                Else

                    Dim T_outputScriptLength As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
                    T_outputScriptLength.Key = ClsUnspentOutput.E_UTXEntry.outputScriptLength
                    T_outputScriptLength.Value = IntToHex(T_Output.LengthOfScript)
                    T_UTX.Add(T_outputScriptLength)

                    T_UnsignedTXHEX += T_outputScriptLength.Value ' IntToHex(T_Output.LengthOfScript)

                    Dim T_outputScript As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
                    T_outputScript.Key = ClsUnspentOutput.E_UTXEntry.outputScript
                    T_outputScript.Value = T_Output.ScriptHex
                    T_UTX.Add(T_outputScript)

                    T_UnsignedTXHEX += T_outputScript.Value ' T_Output.ScriptHex
                End If

            Next

            Dim T_outputLocktime As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
            T_outputLocktime.Key = ClsUnspentOutput.E_UTXEntry.locktime
            T_outputLocktime.Value = IntToHex(OutputLockTime, 8, False)
            T_UTX.Add(T_outputLocktime)

            T_UnsignedTXHEX += T_outputLocktime.Value ' IntToHex(OutputLockTime, 8, False) 'lock time

            Dim T_hashCodeType As ClsUnspentOutput.S_UTXEntry = New ClsUnspentOutput.S_UTXEntry
            T_hashCodeType.Key = ClsUnspentOutput.E_UTXEntry.hashCodeType
            T_hashCodeType.Value = IntToHex(HashCodeType, 8, False)
            T_UTX.Add(T_hashCodeType)

            T_UnsignedTXHEX += T_hashCodeType.Value ' IntToHex(HashCodeType, 8, False) 'hash code type

            T_Input.UnsignedTransaction = T_UTX
            T_Input.UnsignedTransactionHex = T_UnsignedTXHEX

            Dim T_Address As String = ""
            For Each FA As String In Addresses
                Dim Founded As Boolean = False
                For Each T_Addr As String In C_Inputs(i).Addresses

                    If FA = T_Addr Then
                        T_Address = FA
                        Founded = True
                        Exit For
                    End If

                Next
                If Founded Then
                    Exit For
                End If
            Next

            T_Input.SignerAddress = T_Address

            C_Inputs(i) = T_Input

        Next

    End Sub

    Public Function SignTransaction(ByVal PrivateKey As String) As String
        Return SignTransaction(New List(Of String)({PrivateKey}))
    End Function

    Public Function SignTransaction(ByVal PrivateKey As String, ByVal ChainSwapKey As String) As String

        Dim T_PrivKey As S_PrivateKey = New S_PrivateKey
        T_PrivKey.ChainSwapKey = ChainSwapKey
        T_PrivKey.PrivateKey = PrivateKey

        Return SignTransaction(New List(Of S_PrivateKey)({T_PrivKey}))
    End Function

    'Public Function SignTransaction(ByVal PrivateKey As String, ByVal ChainSwapKey As String) As String

    '    Dim T_PrivKey As S_PrivateKey = New S_PrivateKey
    '    T_PrivKey.ChainSwapKey = ChainSwapKey
    '    T_PrivKey.PrivateKey = PrivateKey

    '    If Not RedeemScript.Trim = "" Then

    '        Dim RedeemScriptHash As String = PubKeyToRipe160(RedeemScript)
    '        Dim RedeemScriptList As List(Of ClsScriptEntry) = ConvertLockingScriptStrToList(RedeemScript)

    '        Dim RIPE160Sender As String = ClsBitcoinNET.GetXFromScript(RedeemScriptList, ClsScriptEntry.E_OP_Code.RIPE160Sender)
    '        Dim AddressSender As String = RIPE160ToAddress(RIPE160Sender, BitcoinAddressPrefix)

    '        Dim RIPE160Recipient As String = ClsBitcoinNET.GetXFromScript(RedeemScriptList, ClsScriptEntry.E_OP_Code.RIPE160Recipient)
    '        Dim AddressRecipient As String = RIPE160ToAddress(RIPE160Recipient, BitcoinAddressPrefix)

    '        For i As Integer = 0 To C_Inputs.Count - 1

    '            Dim T_BTCTXInput As ClsUnspentOutput = C_Inputs(i)

    '            If T_BTCTXInput.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then
    '                If T_BTCTXInput.ScriptHex.Contains(RedeemScriptHash) Or T_BTCTXInput.ScriptHash.Contains(RedeemScriptHash) Then

    '                    T_BTCTXInput.Addresses.Clear()
    '                    T_BTCTXInput.Addresses.AddRange({AddressRecipient, AddressSender})

    '                    C_Inputs(i) = T_BTCTXInput

    '                End If
    '            End If

    '        Next

    '    End If

    '    Return SignTransaction(New List(Of S_PrivateKey)({T_PrivKey}), RedeemScript)

    'End Function

    Public Function SignTransaction(ByVal PrivateKeys As List(Of String)) As String

        Dim T_PrivateKeys As List(Of S_PrivateKey) = New List(Of S_PrivateKey)

        For Each TPK As String In PrivateKeys
            Dim T_PrivKey As S_PrivateKey = New S_PrivateKey
            T_PrivKey.ChainSwapKey = ""
            T_PrivKey.PrivateKey = TPK
            T_PrivateKeys.Add(T_PrivKey)
        Next

        Return SignTransaction(T_PrivateKeys)
    End Function

    Public Function SignTransaction(ByVal PrivateKeys As List(Of S_PrivateKey)) As String

        Dim T_Addresses As List(Of String) = New List(Of String)

        For Each TPK As S_PrivateKey In PrivateKeys
            T_Addresses.Add(PubKeyToAddress(PrivKeyToPubKey(TPK.PrivateKey), BitcoinAddressPrefix))
        Next

        FilterAddresses(T_Addresses)
        CreateUnsignedTransaction(T_Addresses)

        For i As Integer = 0 To C_Inputs.Count - 1
            Dim T_Input As ClsUnspentOutput = C_Inputs(i)
            For j As Integer = 0 To T_Addresses.Count - 1
                Dim T_Addr As String = T_Addresses(j)

                If T_Input.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then

                    Select Case GetScriptType(T_Input.Script)

                        Case AbsClsOutputs.E_Type.ChainSwapHashWithLockTime

                            If PrivateKeys(j).ChainSwapKey.Trim = "" Then
                                Dim T_SenderAddress As String = RIPE160ToAddress(ClsBitcoinNET.GetXFromScript(T_Input.Script, ClsScriptEntry.E_OP_Code.RIPE160Sender), BitcoinAddressPrefix)

                                If T_SenderAddress = T_Addr Then
                                    SignPart(PrivateKeys(j).PrivateKey, i, "")
                                End If

                            Else
                                Dim T_RecipientAddress As String = RIPE160ToAddress(ClsBitcoinNET.GetXFromScript(T_Input.Script, ClsScriptEntry.E_OP_Code.RIPE160Recipient), BitcoinAddressPrefix)

                                If T_RecipientAddress = T_Addr Then
                                    SignPart(PrivateKeys(j).PrivateKey, i, PrivateKeys(j).ChainSwapKey)
                                End If

                            End If

                        Case AbsClsOutputs.E_Type.LockTime

                            Dim T_RecipientAddress As String = RIPE160ToAddress(ClsBitcoinNET.GetXFromScript(T_Input.Script, ClsScriptEntry.E_OP_Code.RIPE160Recipient), BitcoinAddressPrefix)

                            If T_RecipientAddress = T_Addr Then
                                SignPart(PrivateKeys(j).PrivateKey, i, "")
                            End If

                        Case Else

                    End Select

                Else
                    If T_Input.SignerAddress = T_Addr Then
                        SignPart(PrivateKeys(j).PrivateKey, i, PrivateKeys(j).ChainSwapKey)
                    End If
                End If

            Next
        Next

        Dim TX As String = CreateSignedTransaction()
        Dim BytesForFees As Integer = Convert.ToInt32(TX.Length / 2)

        Dim T_TotalFee As ULong = Convert.ToUInt64(BytesForFees) * C_FeesNQTPerByte

        Dim UseChange As Boolean = False
        For i As Integer = 0 To Outputs.Count - 1
            Dim T_Output As ClsOutput = Outputs(i)
            If T_Output.ChangeOutput Then
                UseChange = True
                Exit For
            End If
        Next

        DistributeFees(T_TotalFee, False, UseChange)
        CreateUnsignedTransaction(T_Addresses)

        For i As Integer = 0 To C_Inputs.Count - 1
            Dim T_Input As ClsUnspentOutput = C_Inputs(i)
            For j As Integer = 0 To T_Addresses.Count - 1
                Dim T_Addr As String = T_Addresses(j)

                If T_Input.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then

                    Select Case GetScriptType(T_Input.Script)

                        Case AbsClsOutputs.E_Type.ChainSwapHashWithLockTime

                            If PrivateKeys(j).ChainSwapKey.Trim = "" Then
                                Dim T_SenderAddress As String = RIPE160ToAddress(ClsBitcoinNET.GetXFromScript(T_Input.Script, ClsScriptEntry.E_OP_Code.RIPE160Sender), BitcoinAddressPrefix)

                                If T_SenderAddress = T_Addr Then
                                    SignPart(PrivateKeys(j).PrivateKey, i, "")
                                End If

                            Else
                                Dim T_RecipientAddress As String = RIPE160ToAddress(ClsBitcoinNET.GetXFromScript(T_Input.Script, ClsScriptEntry.E_OP_Code.RIPE160Recipient), BitcoinAddressPrefix)

                                If T_RecipientAddress = T_Addr Then
                                    SignPart(PrivateKeys(j).PrivateKey, i, PrivateKeys(j).ChainSwapKey)
                                End If

                            End If

                        Case AbsClsOutputs.E_Type.LockTime

                            Dim T_RecipientAddress As String = RIPE160ToAddress(ClsBitcoinNET.GetXFromScript(T_Input.Script, ClsScriptEntry.E_OP_Code.RIPE160Recipient), BitcoinAddressPrefix)

                            If T_RecipientAddress = T_Addr Then
                                SignPart(PrivateKeys(j).PrivateKey, i, "")
                            End If

                        Case Else

                    End Select



                Else
                    If T_Input.SignerAddress = T_Addr Then
                        SignPart(PrivateKeys(j).PrivateKey, i, PrivateKeys(j).ChainSwapKey)
                    End If
                End If

            Next
        Next

        TX = CreateSignedTransaction()
        SignedTransactionHEX = TX
        Return TX

    End Function

    Private Sub SignPart(ByVal PrivateKey As String, ByVal PartIndex As Integer, Optional ByVal ChainSwapKey As String = "")

        If PrivateKey.Trim = "" And Not MessageIsHEXString(PrivateKey) Then
            Exit Sub
        End If

        Dim T_PublicKey As String = PrivKeyToPubKey(PrivateKey)

        'Dim T_ScriptType As AbsClsOutputs.E_Type = Inputs(PartIndex).OutputType

        'If (T_ScriptType = AbsClsOutputs.E_Type.ChainSwapHash Or T_ScriptType = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime) And (ChainSwapKey.Trim = "" Or Not MessageIsHEXString(ChainSwapKey)) Then
        '    Exit Sub
        'End If
        Dim Ripe160 As String = PubKeyToRipe160(T_PublicKey)
        Dim T_Adress As String = PubKeyToAddress(T_PublicKey, BitcoinAddressPrefix)

        If Not Inputs(PartIndex).Addresses.Contains(T_Adress) Then
            Exit Sub
        End If

        Dim T_Part As String = Inputs(PartIndex).UnsignedTransactionHex

        Dim Sha256 As SHA256Managed = New SHA256Managed()
        Dim T_Part_Hash As Byte() = Sha256.ComputeHash(Sha256.ComputeHash(HEXStringToByteArray(T_Part)))
        Secp256k1Vb.Secp256k1.Start()
        Dim Bitcoin_Signature As Byte()() = Secp256k1Vb.Secp256k1.Sign(T_Part_Hash, HEXStringToByteArray(PrivateKey))

        Dim R_Str As String = ByteArrayToHEXString(Bitcoin_Signature(0))
        Dim S_Str As String = ByteArrayToHEXString(Bitcoin_Signature(1))

        Dim Threshold As Byte = HEXStringToByteArray("7f")(0)
        Dim First_R_Byte As Byte = HEXStringToByteArray(R_Str.Substring(0, 2))(0)
        Dim First_S_Byte As Byte = HEXStringToByteArray(S_Str.Substring(0, 2))(0)

        While (First_R_Byte >= Threshold Or First_S_Byte >= Threshold) Or (Bitcoin_Signature(0).Length <> 32 Or Bitcoin_Signature(1).Length <> 32)

            Bitcoin_Signature = Secp256k1Vb.Secp256k1.Sign(T_Part_Hash, HEXStringToByteArray(PrivateKey))
            R_Str = ByteArrayToHEXString(Bitcoin_Signature(0))
            S_Str = ByteArrayToHEXString(Bitcoin_Signature(1))

            First_R_Byte = HEXStringToByteArray(R_Str.Substring(0, 2))(0)
            First_S_Byte = HEXStringToByteArray(S_Str.Substring(0, 2))(0)

        End While

        Dim T_Signature As List(Of ClsScriptEntry) = New List(Of ClsScriptEntry)

        T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.DER_Prefix))
        T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.R_Value, ByteArrayToHEXString(Bitcoin_Signature(0))))
        T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.DER_Split))
        T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.S_Value, ByteArrayToHEXString(Bitcoin_Signature(1))))
        T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.DER_End))
        T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.PublicKey, T_PublicKey))

        If Inputs(PartIndex).OutputType = AbsClsOutputs.E_Type.ChainSwapHash Then
            T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.ChainSwapKey, ChainSwapKey))
            'ElseIf Inputs(PartIndex).OutputType = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime Then

            '    If Inputs(PartIndex).Addresses(0) = PubKeyToAddress(T_PublicKey, BitcoinAddressPrefix) Then
            '        T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.ChainSwapKey, ChainSwapKey))
            '        T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.OP_TRUE))
            '    ElseIf Inputs(PartIndex).Addresses(1) = PubKeyToAddress(T_PublicKey, BitcoinAddressPrefix) Then
            '        T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.OP_FALSE))
            '    End If

        ElseIf Inputs(PartIndex).OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash And GetScriptType(Inputs(PartIndex).Script) = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime Then
            'AtomicSwap: sign with P2SH

            If Inputs(PartIndex).Addresses(0) = PubKeyToAddress(T_PublicKey, BitcoinAddressPrefix) Then
                T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.ChainSwapKey, ChainSwapKey))
                T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.OP_TRUE))
            ElseIf Inputs(PartIndex).Addresses(1) = PubKeyToAddress(T_PublicKey, BitcoinAddressPrefix) Then
                T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.OP_FALSE))
            End If

            Dim T_RedeemScriptList As List(Of ClsScriptEntry) = Inputs(PartIndex).Script
            T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.OP_PUSHDATA1))
            T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.Unknown, IntToHex(Inputs(PartIndex).LengthOfScript,,)))
            T_Signature.AddRange(T_RedeemScriptList.ToArray())
            'T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.OP_DUP))

        ElseIf Inputs(PartIndex).OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash And GetScriptType(Inputs(PartIndex).Script) = AbsClsOutputs.E_Type.LockTime Then

            Dim T_RedeemScriptList As List(Of ClsScriptEntry) = Inputs(PartIndex).Script
            'T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.OP_PUSHDATA1))
            'T_Signature.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.Unknown, IntToHex(Inputs(PartIndex).LengthOfScript,,)))
            T_Signature.AddRange(GetPushDataSequenceFromLength(Inputs(PartIndex).LengthOfScript))
            T_Signature.AddRange(T_RedeemScriptList.ToArray())

        End If

        'Dim ScriptHex As String = ""
        'For Each SigEntry As ClsScriptEntry In T_Signature

        '    If SigEntry.Key = ClsScriptEntry.E_OP_Code.ChainSwapKey Then
        '        ScriptHex += IntToHex(Convert.ToInt32(SigEntry.ValueHex.Length / 2))
        '    ElseIf SigEntry.Key = ClsScriptEntry.E_OP_Code.PublicKey Then
        '        ScriptHex = IntToHex(Convert.ToInt32(ScriptHex.Length / 2)) + ScriptHex
        '        ScriptHex += IntToHex(Convert.ToInt32(SigEntry.ValueHex.Length / 2))
        '    End If

        '    ScriptHex += SigEntry.ValueHex
        'Next

        C_Inputs(PartIndex).SignatureScript = T_Signature

    End Sub

    Private Function CreateSignedTransaction() As String

        Dim T_TX As List(Of S_TXEntry) = New List(Of S_TXEntry)

        Dim T_version As S_TXEntry = New S_TXEntry()
        T_version.Key = E_TXEntry.version
        T_version.Value = IntToHex(Version, 8, False)
        T_TX.Add(T_version)

        Dim T_SignedTransactionHex As String = T_version.Value ' IntToHex(Version, 8, False)

        Dim T_numberOfInputs As S_TXEntry = New S_TXEntry
        T_numberOfInputs.Key = E_TXEntry.numberOfInputs
        T_numberOfInputs.Value = IntToHex(C_Inputs.Count)
        T_TX.Add(T_numberOfInputs)

        T_SignedTransactionHex += T_numberOfInputs.Value ' IntToHex(C_Inputs.Count)


        'get Locktime

        Dim T_Sequence As String = "ffffffff"

        'For j As Integer = 0 To C_Outputs.Count - 1

        '    Dim T_output As ClsOutput = C_Outputs(j)

        '    If T_output.OutputType = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime Or T_output.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then

        '        T_Sequence = ClsBitcoinNET.GetXFromScript(T_output.Script, ClsScriptEntry.E_OP_Code.LockTime)
        '        'T_Sequence = ChangeHEXStrEndian(T_Sequence)
        '        Exit For

        '    End If

        'Next


        For j As Integer = 0 To C_Inputs.Count - 1
            Dim T_Input As ClsUnspentOutput = C_Inputs(j)

            Dim T_transactionID As S_TXEntry = New S_TXEntry
            T_transactionID.Key = E_TXEntry.transactionID
            T_transactionID.Value = ChangeHEXStrEndian(T_Input.TransactionID)
            T_TX.Add(T_transactionID)

            T_SignedTransactionHex += T_transactionID.Value ' ChangeHEXStrEndian(T_Input.TransactionID)


            Dim T_inputIndex As S_TXEntry = New S_TXEntry
            T_inputIndex.Key = E_TXEntry.unspentOutputIndex
            T_inputIndex.Value = IntToHex(T_Input.InputIndex, 8, False)
            T_TX.Add(T_inputIndex)

            T_SignedTransactionHex += T_inputIndex.Value ' IntToHex(T_Input.InputIndex, 8, False)


            Dim T_Signature As String = ""

            For Each x As ClsScriptEntry In T_Input.SignatureScript

                If x.Key = ClsScriptEntry.E_OP_Code.PublicKey Or x.Key = ClsScriptEntry.E_OP_Code.ChainSwapKey Or x.Key = ClsScriptEntry.E_OP_Code.ChainSwapHash Or x.Key = ClsScriptEntry.E_OP_Code.RIPE160Sender Or x.Key = ClsScriptEntry.E_OP_Code.RIPE160Recipient Then
                    T_Signature += IntToHex(Convert.ToInt32(x.ValueHex.Length / 2), 1, False)
                ElseIf x.Key = ClsScriptEntry.E_OP_Code.LockTime Then

                    x.ValueHex = x.ValueHex

                    'If x.ValueHex.Length / 2 > 1 Then
                    T_Signature += IntToHex(Convert.ToInt32(x.ValueHex.Length / 2), 1, False)
                    'End If
                End If

                T_Signature += x.ValueHex

            Next

            Dim T_signatureLength As S_TXEntry = New S_TXEntry
            T_signatureLength.Key = E_TXEntry.signatureLength
            T_signatureLength.Value = IntToHex(Convert.ToInt32(T_Signature.Length / 2))
            T_TX.Add(T_signatureLength)

            T_SignedTransactionHex += T_signatureLength.Value ' IntToHex(Convert.ToInt32(T_Signature.Length / 2))

            Dim T_sig As S_TXEntry = New S_TXEntry
            T_sig.Key = E_TXEntry.signature
            T_sig.Value = T_Signature
            T_TX.Add(T_sig)

            T_SignedTransactionHex += T_sig.Value ' T_Signature

            'AtomicSwap: bitcoin locktime
            Dim T_seq As S_TXEntry = New S_TXEntry
            T_seq.Key = E_TXEntry.sequence

            Dim result1 As Boolean = T_Input.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash And (GetScriptType(T_Input.Script) = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime Or GetScriptType(T_Input.Script) = AbsClsOutputs.E_Type.LockTime)

            If result1 Then

                Dim T_timeout As String = ClsBitcoinNET.GetXFromScript(T_Input.Script, ClsScriptEntry.E_OP_Code.LockTime)
                'T_timeout = ChangeHEXStrEndian(T_timeout)
                Dim LTBytes As List(Of Byte) = New List(Of Byte)(HEXStringToByteArray(T_timeout))

                If LTBytes.Count < 4 Then
                    For i As Integer = LTBytes.Count To 3
                        LTBytes.Add(0)
                    Next
                End If

                T_timeout = ByteArrayToHEXString(LTBytes.ToArray)

                T_seq.Value = T_timeout
                T_SignedTransactionHex += T_timeout
            Else
                T_seq.Value = T_Sequence
                T_SignedTransactionHex += T_Sequence
            End If

            T_TX.Add(T_seq)

        Next

        Dim T_outputs As S_TXEntry = New S_TXEntry
        T_outputs.Key = E_TXEntry.numberOfOutputs
        T_outputs.Value = IntToHex(C_Outputs.Count)
        T_TX.Add(T_outputs)

        T_SignedTransactionHex += T_outputs.Value ' IntToHex(C_Outputs.Count)

        For j As Integer = 0 To C_Outputs.Count - 1
            Dim T_Output As ClsOutput = C_Outputs(j)

            Dim T_amountNQT As S_TXEntry = New S_TXEntry
            T_amountNQT.Key = E_TXEntry.amountNQT
            T_amountNQT.Value = ULongToHex(T_Output.AmountNQT, 16, False)
            T_TX.Add(T_amountNQT)

            T_SignedTransactionHex += T_amountNQT.Value ' ULongToHex(T_Output.AmountNQT, 16, False)

            If T_Output.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then

                Dim T_outputScriptLength As New S_TXEntry
                T_outputScriptLength.Key = E_TXEntry.outputScriptLength
                T_outputScriptLength.Value = IntToHex(Convert.ToInt32(T_Output.ScriptHash.Length / 2))
                T_TX.Add(T_outputScriptLength)

                T_SignedTransactionHex += T_outputScriptLength.Value ' IntToHex(Convert.ToInt32(T_Output.ScriptHash.Length / 2))

                Dim T_outputScript As S_TXEntry = New S_TXEntry
                T_outputScript.Key = E_TXEntry.outputScript
                T_outputScript.Value = T_Output.ScriptHash
                T_TX.Add(T_outputScript)

                T_SignedTransactionHex += T_outputScript.Value ' T_Output.ScriptHash
            Else

                Dim T_outputScriptLength As New S_TXEntry
                T_outputScriptLength.Key = E_TXEntry.outputScriptLength
                T_outputScriptLength.Value = IntToHex(T_Output.LengthOfScript)
                T_TX.Add(T_outputScriptLength)

                T_SignedTransactionHex += T_outputScriptLength.Value ' IntToHex(T_Output.LengthOfScript)

                Dim T_outputScript As S_TXEntry = New S_TXEntry
                T_outputScript.Key = E_TXEntry.outputScript
                T_outputScript.Value = T_Output.ScriptHex
                T_TX.Add(T_outputScript)

                T_SignedTransactionHex += T_outputScript.Value ' T_Output.ScriptHex
            End If

        Next

        Dim T_locktime As S_TXEntry = New S_TXEntry
        T_locktime.Key = E_TXEntry.locktime
        T_locktime.Value = IntToHex(OutputLockTime, 8, False)
        T_TX.Add(T_locktime)

        T_SignedTransactionHex += T_locktime.Value ' IntToHex(OutputLockTime, 8, False) 'lock time
        'T_SignedTransactionHex += IntToHex(HashCodeType, 8, False) 'hash code type

        C_signedTransaction = T_TX
        SignedTransactionHEX = T_SignedTransactionHex

        Return T_SignedTransactionHex

    End Function

    Private Sub DistributeFees(ByVal TotalFeeNQT As ULong, ByVal Percentage As Boolean, ByVal UseChange As Boolean)

        If TotalInputAmountNQT = 0UL Then
            Exit Sub
        End If

        If TotalInputAmountNQT < TotalFeeNQT Then
            TotalFeeNQT = Convert.ToUInt64(TotalInputAmountNQT / 100 * 5)
        End If

        'Dim T_diff As Long = Convert.ToInt64(TotalAmountNQT - TotalFeeNQT)

        'If T_diff < 0 Then
        '    Exit Sub
        'End If

        If Not TotalInputAmountNQT - TotalFeeNQT >= TotalFeeNQT Then
            Exit Sub
        End If

        Dim TotalOutputAmountNQT As ULong = 0UL

        For Each T_Output As ClsOutput In Outputs
            If T_Output.ChangeOutput Then
                Continue For
            End If
            TotalOutputAmountNQT += T_Output.AmountNQT
        Next

        If Not UseChange Then

            If Percentage Then

                For i As Integer = 0 To Outputs.Count - 1
                    Dim T_Output As ClsOutput = Outputs(i)

                    If T_Output.ChangeOutput Then
                        Continue For
                    End If

                    Dim T_Percent As Double = T_Output.AmountNQT / TotalOutputAmountNQT
                    Dim NewAmountNQT As ULong = Convert.ToUInt64(T_Output.AmountNQT - (T_Output.AmountNQT * T_Percent))

                    T_Output.AmountNQT = NewAmountNQT

                    Outputs(i) = T_Output

                Next

            Else

                Dim T_Fee As ULong = Convert.ToUInt64(TotalFeeNQT / Outputs.Count)

                For i As Integer = 0 To Outputs.Count - 1
                    Dim T_Output As ClsOutput = Outputs(i)

                    If T_Output.ChangeOutput Then
                        Continue For
                    End If

                    T_Output.AmountNQT -= T_Fee

                    Outputs(i) = T_Output

                Next

            End If

        Else

            Dim T_ChangeOutput As ClsOutput = Nothing

            For i As Integer = 0 To Outputs.Count - 1
                Dim T_Output As ClsOutput = Outputs(i)

                If T_Output.ChangeOutput Then
                    T_ChangeOutput = T_Output
                    Exit For
                End If

            Next

            If Not T_ChangeOutput Is Nothing Then
                If T_ChangeOutput.AmountNQT > TotalFeeNQT Then

                    If T_ChangeOutput.AmountNQT - TotalFeeNQT >= TotalFeeNQT Then

                        For i As Integer = 0 To Outputs.Count - 1
                            Dim T_Output As ClsOutput = Outputs(i)

                            If T_Output.ChangeOutput Then
                                T_Output.AmountNQT -= TotalFeeNQT
                                Outputs(i) = T_Output
                                Exit For
                            End If

                        Next

                    Else
                        'ChangeAmount not enough, delete changeoutput

                        For i As Integer = 0 To Outputs.Count - 1
                            Dim T_Output As ClsOutput = Outputs(i)

                            If T_Output.ChangeOutput Then
                                Outputs.RemoveAt(i)
                                Exit For
                            End If
                        Next

                    End If
                Else
                    'ChangeAmount not enough, delete changeoutput

                    For i As Integer = 0 To Outputs.Count - 1
                        Dim T_Output As ClsOutput = Outputs(i)

                        If T_Output.ChangeOutput Then
                            Outputs.RemoveAt(i)
                            Exit For
                        End If
                    Next
                End If

            End If


        End If


    End Sub

    'Private Sub ConvertSignatureToHex()

    '    For i As Integer = 0 To Signatures.Count - 1
    '        Dim T_Sig As S_Signature = Signatures(i)

    '        For Each SigEntry As ClsScriptEntry In T_Sig.Script

    '            If SigEntry.ValueHex.Length / 2 > 1 Then
    '                T_Sig.ScriptHex += IntToHex(SigEntry.ValueHex.Length / 2)
    '            End If

    '            T_Sig.ScriptHex += SigEntry.ValueHex

    '        Next

    '        Signatures(i) = T_Sig

    '    Next

    'End Sub

    'Private Sub ConvertSignaturetoHex(ByVal Signature As List(Of ClsScriptEntry))

    '    Dim T_SignatureHex As String = ""

    '    For Each SigEntry As ClsScriptEntry In Signature

    '        If SigEntry.ValueHex.Length / 2 > 1 Then
    '            T_SignatureHex += IntToHex(SigEntry.ValueHex.Length / 2)
    '        End If

    '        T_SignatureHex += SigEntry.ValueHex

    '    Next

    '    SignaturesHex.Add(T_SignatureHex)

    'End Sub

    Public Function IsTimeOut() As Boolean

        For i As Integer = 0 To C_Inputs.Count - 1

            Dim T_BTCTXInput As ClsUnspentOutput = C_Inputs(i)

            If T_BTCTXInput.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then

                Dim LockTime As String = ClsBitcoinNET.GetXFromScript(T_BTCTXInput.Script, ClsScriptEntry.E_OP_Code.LockTime)
                LockTime = ChangeHEXStrEndian(LockTime) 'to big endian

                Dim RIPE160Sender As String = ClsBitcoinNET.GetXFromScript(T_BTCTXInput.Script, ClsScriptEntry.E_OP_Code.RIPE160Sender)
                Dim AddressSender As String = RIPE160ToAddress(RIPE160Sender, BitcoinAddressPrefix)

                Dim RIPE160Recipient As String = ClsBitcoinNET.GetXFromScript(T_BTCTXInput.Script, ClsScriptEntry.E_OP_Code.RIPE160Recipient)
                Dim AddressRecipient As String = RIPE160ToAddress(RIPE160Recipient, BitcoinAddressPrefix)

                If Not LockTime.Trim = "" Then

                    Dim ULLockTime As ULong = HEXStringToULongList(LockTime)(0)

                    If T_BTCTXInput.Confirmations >= Convert.ToInt32(ULLockTime) Then
                        'GetBack

                        CreateOutput(AddressSender, Satoshi2Dbl(T_BTCTXInput.AmountNQT))
                        FinalizingOutputs(AddressSender)
                        CreateUnsignedTransaction(New List(Of String)({AddressSender}))

                        T_BTCTXInput.Addresses.Clear()
                        T_BTCTXInput.Addresses.AddRange({AddressRecipient, AddressSender})

                        C_Inputs(i) = T_BTCTXInput

                        Return True

                    End If

                End If

            End If

        Next

        Return False

    End Function

#Region "Tools"

    Public Shared Function ConvertLockingScriptStrToList(ByVal Script As String) As List(Of ClsScriptEntry)

        If Not MessageIsHEXString(Script) Then
            Return New List(Of ClsScriptEntry)
        End If

        Try

            Dim ConvertedList As List(Of ClsScriptEntry) = New List(Of ClsScriptEntry)

            Dim OP_Code As Boolean = True
            Dim TempLength As Integer = 0
            Dim HEXVal As String = ""

            'Dim RIPE160Counter As Integer = 0

            For i As Integer = 0 To Script.Length - 1 Step 2

                Dim T_HexByte As String = Script.Substring(i, 2)
                Dim T_Byt As Byte = HEXStringToByteArray(T_HexByte)(0)
                Dim T_Int As Integer = Convert.ToInt32(T_Byt)

                If Not OP_Code Then

                    If TempLength > 0 Then
                        HEXVal += T_HexByte
                        TempLength -= 1
                    Else
                        OP_Code = True

                        'If RIPE160Counter = 0 Then
                        ConvertedList.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.Unknown, HEXVal))
                        'Else
                        '    ConvertedList.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.RIPE160Sender, HEXVal))
                        'End If
                        'RIPE160Counter += 1
                        HEXVal = ""

                    End If

                End If


                If ClsScriptEntry.Is_E_OP_Code(T_Int) And OP_Code Then
                    ConvertedList.Add(New ClsScriptEntry(T_Int))
                ElseIf OP_Code Then
                    OP_Code = False
                    TempLength = T_Int
                End If

            Next

            Dim T_ScriptType As AbsClsOutputs.E_Type = GetScriptType(ConvertedList)

            Dim CNTer As Integer = 0
            For SE As Integer = 0 To ConvertedList.Count - 1
                Dim T_ScriptEntry As ClsScriptEntry = ConvertedList(SE)

                Dim KeyCode As Integer = T_ScriptEntry.Key

                Select Case KeyCode
                    Case 82 To 96 'ClsScriptEntry.E_OP_Code.OP_2 to OP_16
                        CNTer += 1
                End Select

                If T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.Unknown And (T_ScriptType = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime Or T_ScriptType = AbsClsOutputs.E_Type.ChainSwapHash) Then

                    If CNTer = 0 Then
                        T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.ChainSwapHash
                        CNTer += 1
                    ElseIf CNTer = 1 Then
                        T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.RIPE160Recipient
                        CNTer += 1
                    ElseIf CNTer = 2 Then
                        T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.LockTime
                        CNTer += 1
                    ElseIf CNTer = 3 Then
                        T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.RIPE160Sender
                        CNTer = 0
                    End If

                ElseIf T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.Unknown And T_ScriptType = AbsClsOutputs.E_Type.LockTime Then

                    If CNTer = 0 Then
                        T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.LockTime
                        CNTer += 1
                    Else
                        T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.RIPE160Recipient
                        CNTer += 1
                    End If
                ElseIf T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.Unknown And T_ScriptType = AbsClsOutputs.E_Type.Pay2ScriptHash Then
                    T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.ScriptHash
                ElseIf T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.Unknown And T_ScriptType = AbsClsOutputs.E_Type.Standard Then
                    T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.RIPE160Recipient
                End If

                ConvertedList(SE) = T_ScriptEntry

            Next

            Return ConvertedList

        Catch ex As Exception
            Return New List(Of ClsScriptEntry)
        End Try
    End Function

    Public Shared Function GetScriptType(ByVal Script As String) As AbsClsOutputs.E_Type

        If Not MessageIsHEXString(Script) Then
            Return AbsClsOutputs.E_Type.Unknown
        End If

        Return GetScriptType(ConvertLockingScriptStrToList(Script))
    End Function
    Public Shared Function GetScriptType(ByVal Script As List(Of ClsScriptEntry)) As AbsClsOutputs.E_Type

        Dim CreateStandardLockingScriptBoolList As List(Of Boolean) = New List(Of Boolean)
        'Dim CreateChainSwapLockingScriptBoolList As List(Of Boolean) = New List(Of Boolean)
        Dim CreateLockTimedLockingScriptBoolList As List(Of Boolean) = New List(Of Boolean)
        Dim CreateLockTimedChainSwapLockingScriptBoolList As List(Of Boolean) = New List(Of Boolean)
        Dim CreatePay2ScriptHashList As List(Of Boolean) = New List(Of Boolean)

        For i As Integer = 0 To Script.Count - 1

            Dim T_ScriptEntry As ClsScriptEntry = Script(i)

            Select Case i

                Case 0

                    CreateLockTimedLockingScriptBoolList.Add(True) 'LTHEX

                    Select Case T_ScriptEntry.Key
                        Case ClsScriptEntry.E_OP_Code.OP_DUP
                            CreateStandardLockingScriptBoolList.Add(True) 'OP_DUP
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_SHA256
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) ' OP_IF
                            CreatePay2ScriptHashList.Add(False)'OP_HASH160
                        Case ClsScriptEntry.E_OP_Code.OP_SHA256
                            CreateStandardLockingScriptBoolList.Add(False) 'OP_DUP
                            'CreateChainSwapLockingScriptBoolList.Add(True) 'OP_SHA256
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) ' OP_IF
                            CreatePay2ScriptHashList.Add(False)'OP_HASH160
                        Case ClsScriptEntry.E_OP_Code.OP_CHECKSEQUENCEVERIFY
                            CreateStandardLockingScriptBoolList.Add(False) 'OP_DUP
                            'CreateChainSwapLockingScriptBoolList.Add(True) 'OP_SHA256
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) ' OP_IF
                            CreatePay2ScriptHashList.Add(False)'OP_HASH160
                        Case ClsScriptEntry.E_OP_Code.OP_IF
                            CreateStandardLockingScriptBoolList.Add(False) 'OP_DUP
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_SHA256
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(True) ' OP_IF
                            CreatePay2ScriptHashList.Add(False)'OP_HASH160
                        Case ClsScriptEntry.E_OP_Code.OP_HASH160
                            CreateStandardLockingScriptBoolList.Add(False) 'OP_DUP
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_SHA256
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) ' OP_IF
                            CreatePay2ScriptHashList.Add(True) 'OP_HASH160
                        Case Else

                    End Select
                Case 1

                    'CreateChainSwapLockingScriptBoolList.Add(True) 'ChainSwapHash
                    CreatePay2ScriptHashList.Add(True) 'ScriptHash

                    Select Case T_ScriptEntry.Key
                        Case ClsScriptEntry.E_OP_Code.OP_HASH160
                            CreateStandardLockingScriptBoolList.Add(True)
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_CHECKSEQUENCEVERIFY
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_SHA256
                        Case ClsScriptEntry.E_OP_Code.OP_CHECKSEQUENCEVERIFY
                            CreateStandardLockingScriptBoolList.Add(False)
                            CreateLockTimedLockingScriptBoolList.Add(True) 'OP_CHECKSEQUENCEVERIFY
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False)'OP_SHA256
                        Case ClsScriptEntry.E_OP_Code.OP_SHA256
                            CreateStandardLockingScriptBoolList.Add(False)
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_CHECKSEQUENCEVERIFY
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'OP_SHA256
                        Case Else
                            CreateStandardLockingScriptBoolList.Add(False)
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_CHECKSEQUENCEVERIFY
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_SHA256
                    End Select
                Case 2

                    CreateStandardLockingScriptBoolList.Add(True)
                    CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'ChainSwapHash

                    Select Case T_ScriptEntry.Key
                        Case ClsScriptEntry.E_OP_Code.OP_HASH160
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_EQUALVERIFY
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_DROP
                            CreatePay2ScriptHashList.Add(False) 'OP_EQUAL
                        Case ClsScriptEntry.E_OP_Code.OP_EQUALVERIFY
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_EQUALVERIFY
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_DROP
                            CreatePay2ScriptHashList.Add(False) 'OP_EQUAL
                        Case ClsScriptEntry.E_OP_Code.OP_DROP
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_EQUALVERIFY
                            CreateLockTimedLockingScriptBoolList.Add(True) 'OP_DROP
                            CreatePay2ScriptHashList.Add(False) 'OP_EQUAL
                        Case ClsScriptEntry.E_OP_Code.OP_EQUAL
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_EQUALVERIFY
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_DROP
                            CreatePay2ScriptHashList.Add(True) 'OP_EQUAL
                        Case Else
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_EQUALVERIFY
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_DROP
                            CreatePay2ScriptHashList.Add(False) 'OP_EQUAL
                    End Select

                Case 3

                    CreatePay2ScriptHashList.Add(False) 'Nothing

                    Select Case T_ScriptEntry.Key
                        Case ClsScriptEntry.E_OP_Code.OP_EQUALVERIFY
                            CreateStandardLockingScriptBoolList.Add(True)
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_DUP
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_DUP
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'OP_EQUALVERIFY
                        Case ClsScriptEntry.E_OP_Code.OP_HASH160
                            CreateStandardLockingScriptBoolList.Add(True)
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_DUP
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_DUP
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_EQUALVERIFY
                        Case ClsScriptEntry.E_OP_Code.OP_DUP
                            CreateStandardLockingScriptBoolList.Add(False)
                            'CreateChainSwapLockingScriptBoolList.Add(True) 'OP_DUP
                            CreateLockTimedLockingScriptBoolList.Add(True) 'OP_DUP
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_EQUALVERIFY
                        Case Else
                            CreateStandardLockingScriptBoolList.Add(False)
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_DUP
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_DUP
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_EQUALVERIFY
                    End Select

                Case 4

                    'CreateLockTimedLockingScriptBoolList.Add(True) 'OP_HASH160
                    CreatePay2ScriptHashList.Add(False) 'Nothing

                    Select Case T_ScriptEntry.Key
                        Case ClsScriptEntry.E_OP_Code.OP_CHECKSIG
                            CreateStandardLockingScriptBoolList.Add(True) 'OP_CHECKSIG
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_HASH160
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_HASH160
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_DUP
                        Case ClsScriptEntry.E_OP_Code.OP_HASH160
                            CreateStandardLockingScriptBoolList.Add(False) 'OP_CHECKSIG
                            'CreateChainSwapLockingScriptBoolList.Add(True) 'OP_HASH160
                            CreateLockTimedLockingScriptBoolList.Add(True) 'OP_HASH160
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_DUP
                        Case ClsScriptEntry.E_OP_Code.OP_DUP
                            CreateStandardLockingScriptBoolList.Add(False) 'OP_CHECKSIG
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_HASH160
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_HASH160
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'OP_DUP
                        Case ClsScriptEntry.E_OP_Code.OP_EQUALVERIFY
                            CreateStandardLockingScriptBoolList.Add(False) 'OP_CHECKSIG
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_HASH160
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_HASH160
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_DUP
                        Case Else
                            CreateStandardLockingScriptBoolList.Add(False) 'OP_CHECKSIG
                            'CreateChainSwapLockingScriptBoolList.Add(False) 'OP_HASH160
                            'CreateLockTimedLockingScriptBoolList.Add(False) 'OP_HASH160
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_DUP
                    End Select

                Case 5

                    CreateStandardLockingScriptBoolList.Add(False) 'Nothing
                    'CreateChainSwapLockingScriptBoolList.Add(True) 'RIPE160
                    CreateLockTimedLockingScriptBoolList.Add(True) 'RIPE160
                    CreatePay2ScriptHashList.Add(False) 'Nothing

                    Select Case T_ScriptEntry.Key
                        Case ClsScriptEntry.E_OP_Code.OP_HASH160
                            'CreateLockTimedLockingScriptBoolList.Add(False) 'RIPE160
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'OP_HASH160
                        Case ClsScriptEntry.E_OP_Code.OP_EQUALVERIFY
                            'CreateLockTimedLockingScriptBoolList.Add(True) 'RIPE160
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_HASH160
                        Case Else
                            'CreateLockTimedLockingScriptBoolList.Add(False) 'RIPE160
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_HASH160
                    End Select

                Case 6

                    CreateStandardLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'RIPE160A
                    CreatePay2ScriptHashList.Add(False) 'Nothing

                    Select Case T_ScriptEntry.Key
                        Case ClsScriptEntry.E_OP_Code.OP_EQUALVERIFY
                            'CreateChainSwapLockingScriptBoolList.Add(True)
                            CreateLockTimedLockingScriptBoolList.Add(True)'OP_EQUALVERIFY
                        Case ClsScriptEntry.E_OP_Code.OP_CHECKSIG
                            'CreateChainSwapLockingScriptBoolList.Add(False)
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_EQUALVERIFY
                        Case Else
                            'CreateChainSwapLockingScriptBoolList.Add(False)
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_EQUALVERIFY
                    End Select

                Case 7

                    CreateStandardLockingScriptBoolList.Add(False) 'Nothing
                    'CreateLockTimedLockingScriptBoolList.Add(False) 'OP_CHECKSIG
                    CreatePay2ScriptHashList.Add(False) 'Nothing

                    Select Case T_ScriptEntry.Key
                        Case ClsScriptEntry.E_OP_Code.OP_CHECKSIG
                            'CreateChainSwapLockingScriptBoolList.Add(True)
                            CreateLockTimedLockingScriptBoolList.Add(True) 'OP_CHECKSIG
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_ELSE
                        Case ClsScriptEntry.E_OP_Code.OP_EQUALVERIFY
                            'CreateChainSwapLockingScriptBoolList.Add(False)
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_CHECKSIG
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_ELSE
                        Case ClsScriptEntry.E_OP_Code.OP_ELSE
                            'CreateChainSwapLockingScriptBoolList.Add(False)
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_CHECKSIG
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'OP_ELSE
                        Case Else
                            'CreateChainSwapLockingScriptBoolList.Add(False)
                            CreateLockTimedLockingScriptBoolList.Add(False) 'OP_CHECKSIG
                            CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_ELSE
                    End Select

                Case 8

                    CreateStandardLockingScriptBoolList.Add(False) 'Nothing
                    'CreateChainSwapLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'LockTime
                    CreatePay2ScriptHashList.Add(False) 'Nothing

                Case 9

                    CreateStandardLockingScriptBoolList.Add(False) 'Nothing
                    'CreateChainSwapLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedLockingScriptBoolList.Add(False) 'Nothing
                    CreatePay2ScriptHashList.Add(False) 'Nothing

                    If T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.OP_CHECKSEQUENCEVERIFY Then
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'OP_CHECKSEQUENCEVERIFY
                    Else
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_CHECKSEQUENCEVERIFY
                    End If

                Case 10

                    CreateStandardLockingScriptBoolList.Add(False) 'Nothing
                    'CreateChainSwapLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedLockingScriptBoolList.Add(False) 'Nothing
                    CreatePay2ScriptHashList.Add(False) 'Nothing

                    If T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.OP_DROP Then
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'OP_DROP
                    Else
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_DROP
                    End If

                Case 11

                    CreateStandardLockingScriptBoolList.Add(False) 'Nothing
                    'CreateChainSwapLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedLockingScriptBoolList.Add(False) 'Nothing
                    CreatePay2ScriptHashList.Add(False) 'Nothing

                    If T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.OP_DUP Then
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'OP_DUP
                    Else
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_DUP
                    End If

                Case 12

                    CreateStandardLockingScriptBoolList.Add(False) 'Nothing
                    'CreateChainSwapLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedLockingScriptBoolList.Add(False) 'Nothing
                    CreatePay2ScriptHashList.Add(False) 'Nothing

                    If T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.OP_HASH160 Then
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'OP_HASH160
                    Else
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_HASH160
                    End If

                Case 13

                    CreateStandardLockingScriptBoolList.Add(False) 'Nothing
                    'CreateChainSwapLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'RIPE160B
                    CreatePay2ScriptHashList.Add(False) 'Nothing

                Case 14

                    CreateStandardLockingScriptBoolList.Add(False) 'Nothing
                    'CreateChainSwapLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedLockingScriptBoolList.Add(False) 'Nothing
                    CreatePay2ScriptHashList.Add(False) 'Nothing

                    If T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.OP_ENDIF Then
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'OP_ENDIF
                    Else
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_ENDIF
                    End If

                Case 15

                    CreateStandardLockingScriptBoolList.Add(False) 'Nothing
                    'CreateChainSwapLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedLockingScriptBoolList.Add(False) 'Nothing
                    CreatePay2ScriptHashList.Add(False) 'Nothing

                    If T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.OP_EQUALVERIFY Then
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'OP_EQUALVERIFY
                    Else
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_EQUALVERIFY
                    End If

                Case 16

                    CreateStandardLockingScriptBoolList.Add(False) 'Nothing
                    'CreateChainSwapLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedLockingScriptBoolList.Add(False) 'Nothing
                    CreatePay2ScriptHashList.Add(False) 'Nothing

                    If T_ScriptEntry.Key = ClsScriptEntry.E_OP_Code.OP_CHECKSIG Then
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(True) 'OP_CHECKSIG
                    Else
                        CreateLockTimedChainSwapLockingScriptBoolList.Add(False) 'OP_CHECKSIG
                    End If

                Case Else
                    CreateStandardLockingScriptBoolList.Add(False) 'Nothing
                    'CreateChainSwapLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedLockingScriptBoolList.Add(False) 'Nothing
                    CreateLockTimedChainSwapLockingScriptBoolList.Add(False)
                    CreatePay2ScriptHashList.Add(False)
            End Select

        Next

        Dim CSt As Boolean = GetFalseOfList(CreateStandardLockingScriptBoolList)
        Dim CCS As Boolean = False ' GetFalseOfList(CreateChainSwapLockingScriptBoolList)
        Dim CLT As Boolean = GetFalseOfList(CreateLockTimedLockingScriptBoolList)
        Dim CLTCS As Boolean = GetFalseOfList(CreateLockTimedChainSwapLockingScriptBoolList)
        Dim P2SH As Boolean = GetFalseOfList(CreatePay2ScriptHashList)


        If Not P2SH And Not CLTCS And Not CLT And Not CCS And CSt Then
            'standard
            Return AbsClsOutputs.E_Type.Standard
        ElseIf Not P2SH And Not CLTCS And Not CLT And CCS And Not CSt Then
            'ChainSwap
            Return AbsClsOutputs.E_Type.ChainSwapHash
        ElseIf Not P2SH And Not CLTCS And CLT And Not CCS And Not CSt Then
            'LockTime
            Return AbsClsOutputs.E_Type.LockTime
        ElseIf Not P2SH And CLTCS And Not CLT And Not CCS And Not CSt Then
            'LockTimeChainSwap
            Return AbsClsOutputs.E_Type.ChainSwapHashWithLockTime
        ElseIf P2SH And Not CLTCS And Not CLT And Not CCS And Not CSt Then
            'Pay2ScriptHash
            Return AbsClsOutputs.E_Type.Pay2ScriptHash
        End If

        Return AbsClsOutputs.E_Type.Unknown

    End Function
    Private Shared Function GetFalseOfList(ByVal BoolList As List(Of Boolean)) As Boolean

        For Each b As Boolean In BoolList
            If b = False Then
                Return False
            End If
        Next

        Return True

    End Function

    Private Function GetPushDataSequenceFromLength(ByVal Length As Integer) As List(Of ClsScriptEntry)

        Dim LengthSequence As List(Of ClsScriptEntry) = New List(Of ClsScriptEntry)

        Select Case Length
            Case 1 To 75
                LengthSequence.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.PUSHDATALENGTH, IntToHex(Length, 1, False)))
            Case 76 To 255
                LengthSequence.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.OP_PUSHDATA1))
                LengthSequence.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.PUSHDATALENGTH, IntToHex(Length, 1, False)))
            Case 256 To 65535
                LengthSequence.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.OP_PUSHDATA2))
                LengthSequence.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.PUSHDATALENGTH, IntToHex(Length, 2, False)))
            Case Is > 65535
                LengthSequence.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.OP_PUSHDATA4))
                LengthSequence.Add(New ClsScriptEntry(ClsScriptEntry.E_OP_Code.PUSHDATALENGTH, IntToHex(Length, 4, False)))
            Case Else
                'negative
        End Select

        Return LengthSequence

    End Function

    'Private Function GetPushDataSequenceStringFromLength(ByVal Length As Integer) As String
    '    Dim LengthSequence As List(Of ClsScriptEntry) = GetPushDataSequenceFromLength(Length)

    '    Dim SequenceString As String = ""
    '    For Each Len As ClsScriptEntry In LengthSequence
    '        SequenceString += Len.ValueHex
    '    Next

    '    Return SequenceString

    'End Function

#End Region

End Class
