Public Class ClsBitcoin
    Inherits AbsClsXItem

#Region "INISettings"

    Public Overrides Function SetXItemTransactionToINI(DEXContract As ClsDEXContract, XItemTransactionID As String, ChainSwapKey As String, ChainSwapHash As String) As Boolean

        Dim BitcoinTransactions As String = GetINISetting(E_Setting.BitcoinTransactions, "")

        '<DEXContract></DEXContract><OrderID></OrderID><BTCTXID></BTCTXID><ChainSwapKey></ChainSwapKey><RedeemScript></RedeemScript>

        Dim SetStr As String = "<DEXContract>" + DEXContract.ID.ToString() + "</DEXContract>" +
            "<OrderID>" + DEXContract.CurrentCreationTransaction.ToString() + "</OrderID>" +
            "<BitcoinTransactionID>" + XItemTransactionID + "</BitcoinTransactionID>" +
            "<ChainSwapKey>" + ChainSwapKey + "</ChainSwapKey>" +
            "<RedeemScript>" + ChainSwapHash + "</RedeemScript>;"

        If Not BitcoinTransactions.Contains(SetStr) Then
            If BitcoinTransactions.Trim = "" Then
                BitcoinTransactions = SetStr
            ElseIf BitcoinTransactions.Contains(";") Then
                BitcoinTransactions += SetStr
            End If

            SetINISetting(E_Setting.BitcoinTransactions, BitcoinTransactions.Trim)

        End If

        Return True

    End Function

    Public Overrides Function SetXItemTransactionToINI(DEXContract As ClsDEXContract, XItemTransactionID As String, ChainSwapHash As String) As Boolean

        Dim BitcoinTransactions As String = GetINISetting(E_Setting.BitcoinTransactions, "")

        '<DEXContract></DEXContract><OrderID></OrderID><BTCTXID></BTCTXID><ChainSwapKey></ChainSwapKey><RedeemScript></RedeemScript>

        Dim SetStr As String = "<DEXContract>" + DEXContract.ID.ToString() + "</DEXContract>" +
            "<OrderID>" + DEXContract.CurrentCreationTransaction.ToString() + "</OrderID>" +
            "<BitcoinTransactionID>" + XItemTransactionID + "</BitcoinTransactionID>" +
            "<ChainSwapKey></ChainSwapKey>" +
            "<RedeemScript>" + ChainSwapHash + "</RedeemScript>;"

        If Not BitcoinTransactions.Contains(SetStr) Then
            If BitcoinTransactions.Trim = "" Then
                BitcoinTransactions = SetStr
            ElseIf BitcoinTransactions.Contains(";") Then
                BitcoinTransactions += SetStr
            End If

            SetINISetting(E_Setting.BitcoinTransactions, BitcoinTransactions.Trim)

        End If

        Return True

    End Function

    Public Overrides Function SetXItemTransactionToINI(DEXContractID As ULong, OrderID As ULong, XItemTransactionID As String, ChainSwapKey As String, ChainSwapHash As String) As Boolean

        Dim BitcoinTransactions As String = GetINISetting(E_Setting.BitcoinTransactions, "")

        '<DEXContract></DEXContract><OrderID></OrderID><BTCTXID></BTCTXID><ChainSwapKey></ChainSwapKey><RedeemScript></RedeemScript>

        Dim SetStr As String = "<DEXContract>" + DEXContractID.ToString() + "</DEXContract>" +
            "<OrderID>" + OrderID.ToString() + "</OrderID>" +
            "<BitcoinTransactionID>" + XItemTransactionID + "</BitcoinTransactionID>" +
            "<ChainSwapKey>" + ChainSwapKey + "</ChainSwapKey>" +
            "<RedeemScript>" + ChainSwapHash + "</RedeemScript>;"

        If Not BitcoinTransactions.Contains(SetStr) Then
            If BitcoinTransactions.Trim = "" Then
                BitcoinTransactions = SetStr
            ElseIf BitcoinTransactions.Contains(";") Then
                BitcoinTransactions += SetStr
            End If

            SetINISetting(E_Setting.BitcoinTransactions, BitcoinTransactions.Trim)

        End If

        Return True

    End Function

    Public Overrides Function GetXItemChainSwapKeyFromINI(DEXContractID As ULong, OrderID As ULong, XItemTransactionID As String) As String

        Dim BitcoinTransactions As String = GetINISetting(E_Setting.BitcoinTransactions, "")
        Dim BitcoinTransactionsList As List(Of String) = New List(Of String)

        If BitcoinTransactions.Contains(";") Then
            Dim TempList As List(Of String) = New List(Of String)(BitcoinTransactions.Split(";"c))
            For Each TempTX As String In TempList
                If Not TempTX.Trim = "" Then
                    BitcoinTransactionsList.Add(TempTX)
                End If
            Next
        Else
            If Not BitcoinTransactions.Trim = "" Then
                BitcoinTransactionsList.Add(BitcoinTransactions)
            End If
        End If

        Dim FindStr As String = "<DEXContract>" + DEXContractID.ToString() + "</DEXContract>" +
            "<OrderID>" + OrderID.ToString() + "</OrderID>" +
            "<BitcoinTransactionID>" + XItemTransactionID + "</BitcoinTransactionID>"

        For Each BitcoinTX As String In BitcoinTransactionsList
            If BitcoinTX.Contains(FindStr) And BitcoinTX.Contains("<ChainSwapKey>") Then
                Dim ChainSwapKey As String = GetStringBetween(BitcoinTX, "<ChainSwapKey>", "</ChainSwapKey>")
                Return ChainSwapKey
            End If
        Next

        Return ""

    End Function

    Public Overrides Function GetXItemChainSwapHashFromINI(DEXContractID As ULong, OrderID As ULong, XItemTransactionID As String) As String

        Dim BitcoinTransactions As String = GetINISetting(E_Setting.BitcoinTransactions, "")
        Dim BitcoinTransactionsList As List(Of String) = New List(Of String)

        If BitcoinTransactions.Contains(";") Then
            Dim TempList As List(Of String) = New List(Of String)(BitcoinTransactions.Split(";"c))
            For Each TempTX As String In TempList
                If Not TempTX.Trim = "" Then
                    BitcoinTransactionsList.Add(TempTX)
                End If
            Next
        Else
            If Not BitcoinTransactions.Trim = "" Then
                BitcoinTransactionsList.Add(BitcoinTransactions)
            End If
        End If

        Dim FindStr As String = "<DEXContract>" + DEXContractID.ToString() + "</DEXContract>" +
            "<OrderID>" + OrderID.ToString() + "</OrderID>" +
            "<BitcoinTransactionID>" + XItemTransactionID + "</BitcoinTransactionID>"

        For Each BitcoinTX As String In BitcoinTransactionsList
            If BitcoinTX.Contains(FindStr) And BitcoinTX.Contains("<RedeemScript>") Then
                Dim RedeemScript As String = GetStringBetween(BitcoinTX, "<RedeemScript>", "</RedeemScript>")
                If RedeemScript.Trim <> "" Then
                    Return ClsBitcoinNET.GetXFromScript(RedeemScript, ClsScriptEntry.E_OP_Code.ChainSwapHash)
                End If

            End If
        Next

        Return ""

    End Function

    Private Function GetBitcoinRedeemScriptFromINI(DEXContractID As ULong, OrderID As ULong, ByVal BitcoinTransactionID As String, ByVal ChainSwapHash As String) As String

        Dim BitcoinTransactions As String = GetINISetting(E_Setting.BitcoinTransactions, "")
        Dim BitcoinTransactionsList As List(Of String) = New List(Of String)

        If BitcoinTransactions.Contains(";") Then
            Dim TempList As List(Of String) = New List(Of String)(BitcoinTransactions.Split(";"c))
            For Each TempTX As String In TempList
                If Not TempTX.Trim = "" Then
                    BitcoinTransactionsList.Add(TempTX)
                End If
            Next
        Else
            If Not BitcoinTransactions.Trim = "" Then
                BitcoinTransactionsList.Add(BitcoinTransactions)
            End If
        End If

        Dim FindStr As String = "<DEXContract>" + DEXContractID.ToString() + "</DEXContract>" +
            "<OrderID>" + OrderID.ToString() + "</OrderID>" +
            "<BitcoinTransactionID>" + BitcoinTransactionID + "</BitcoinTransactionID>"

        For Each BitcoinTX As String In BitcoinTransactionsList
            If BitcoinTX.Contains(FindStr) And BitcoinTX.Contains("<RedeemScript>") And BitcoinTX.Contains(ChainSwapHash) Then
                Dim RedeemScript As String = GetStringBetween(BitcoinTX, "<RedeemScript>", "</RedeemScript>")
                Return RedeemScript
            End If
        Next

        Return ""

    End Function

    Private Function GetBitcoinRedeemScriptFromINI(ByVal BitcoinTransactionID As String, ByVal ChainSwapHash As String) As String

        Dim BitcoinTransactions As String = GetINISetting(E_Setting.BitcoinTransactions, "")
        Dim BitcoinTransactionsList As List(Of String) = New List(Of String)

        If BitcoinTransactions.Contains(";") Then
            Dim TempList As List(Of String) = New List(Of String)(BitcoinTransactions.Split(";"c))
            For Each TempTX As String In TempList
                If Not TempTX.Trim = "" Then
                    BitcoinTransactionsList.Add(TempTX)
                End If
            Next
        Else
            If Not BitcoinTransactions.Trim = "" Then
                BitcoinTransactionsList.Add(BitcoinTransactions)
            End If
        End If

        For Each BitcoinTX As String In BitcoinTransactionsList
            If BitcoinTX.Contains("<BitcoinTransactionID>") And BitcoinTX.Contains(BitcoinTransactionID) And BitcoinTX.Contains("<RedeemScript>") And BitcoinTX.Contains(ChainSwapHash) Then
                Dim RedeemScript As String = GetStringBetween(BitcoinTX, "<RedeemScript>", "</RedeemScript>")
                Return RedeemScript
            End If
        Next

        Return ""

    End Function

    Public Overrides Function GetXItemTransactionFromINI(DEXContractID As ULong, OrderID As ULong) As String

        Dim BitcoinTransactions As String = GetINISetting(E_Setting.BitcoinTransactions, "")
        Dim BitcoinTransactionsList As List(Of String) = New List(Of String)

        If BitcoinTransactions.Contains(";") Then
            Dim TempList As List(Of String) = New List(Of String)(BitcoinTransactions.Split(";"c))
            For Each TempTX As String In TempList
                If Not TempTX.Trim = "" Then
                    BitcoinTransactionsList.Add(TempTX)
                End If
            Next
        Else
            If Not BitcoinTransactions.Trim = "" Then
                BitcoinTransactionsList.Add(BitcoinTransactions)
            End If
        End If

        Dim FindStr As String = "<DEXContract>" + DEXContractID.ToString() + "</DEXContract>" +
            "<OrderID>" + OrderID.ToString() + "</OrderID>"

        For Each BitcoinTX As String In BitcoinTransactionsList
            If BitcoinTX.Contains(FindStr) And BitcoinTX.Contains("<BitcoinTransactionID>") Then
                Return GetStringBetween(BitcoinTX, "<BitcoinTransactionID>", "</BitcoinTransactionID>")
            End If
        Next

        Return ""

    End Function

    Public Overrides Function GetXItemTransactionFromINI(DEXContractID As ULong, OrderID As ULong, ByVal ChainSwapHash As String) As String

        Dim BitcoinTransactions As String = GetINISetting(E_Setting.BitcoinTransactions, "")
        Dim BitcoinTransactionsList As List(Of String) = New List(Of String)

        If BitcoinTransactions.Contains(";") Then
            Dim TempList As List(Of String) = New List(Of String)(BitcoinTransactions.Split(";"c))
            For Each TempTX As String In TempList
                If Not TempTX.Trim = "" Then
                    BitcoinTransactionsList.Add(TempTX)
                End If
            Next
        Else
            If Not BitcoinTransactions.Trim = "" Then
                BitcoinTransactionsList.Add(BitcoinTransactions)
            End If
        End If

        Dim FindStr As String = "<DEXContract>" + DEXContractID.ToString() + "</DEXContract>" +
            "<OrderID>" + OrderID.ToString() + "</OrderID>"

        For Each BitcoinTX As String In BitcoinTransactionsList
            If BitcoinTX.Contains(FindStr) And BitcoinTX.Contains("<RedeemScript>") And BitcoinTX.Contains("<BitcoinTransactionID>") Then

                Dim T_RedeemScript As String = GetStringBetween(BitcoinTX, "<RedeemScript>", "</RedeemScript>").ToLower

                If T_RedeemScript.Contains(ChainSwapHash.ToLower) Then
                    Return GetStringBetween(BitcoinTX, "<BitcoinTransactionID>", "</BitcoinTransactionID>")
                End If
            End If
        Next

        Return ""

    End Function

    Public Overrides Function DelXItemTransactionFromINI(DEXContractID As ULong, OrderID As ULong, XItemTransactionID As String, ChainSwapHash As String) As Boolean

        Dim BitcoinTransactions As String = GetINISetting(E_Setting.BitcoinTransactions, "")
        Dim BitcoinTransactionsList As List(Of String) = New List(Of String)

        If BitcoinTransactions.Contains(";") Then
            Dim TempList As List(Of String) = New List(Of String)(BitcoinTransactions.Split(";"c))
            For Each TempTX As String In TempList
                If Not TempTX.Trim = "" Then
                    BitcoinTransactionsList.Add(TempTX)
                End If
            Next
        Else
            If Not BitcoinTransactions.Trim = "" Then
                BitcoinTransactionsList.Add(BitcoinTransactions)
            End If
        End If

        Dim Returner As Boolean = False

        Dim FindStr As String = "<DEXContract>" + DEXContractID.ToString() + "</DEXContract>" +
            "<OrderID>" + OrderID.ToString() + "</OrderID>" +
            "<BitcoinTransactionID>" + XItemTransactionID + "</BitcoinTransactionID>"

        BitcoinTransactions = ""
        For Each BitcoinTX As String In BitcoinTransactionsList
            If BitcoinTX.Contains(FindStr) And BitcoinTX.Contains("<RedeemScript>") Then

                Dim T_ChainSwapHash As String = GetStringBetween(BitcoinTX, "<RedeemScript>", "</RedeemScript>").ToLower()

                If T_ChainSwapHash.Contains(ChainSwapHash.ToLower()) Then
                    Returner = True
                Else
                    BitcoinTransactions += BitcoinTX + ";"
                End If
            Else
                BitcoinTransactions += BitcoinTX + ";"
            End If
        Next

        SetINISetting(E_Setting.BitcoinTransactions, BitcoinTransactions.Trim())

        Return Returner

    End Function

#End Region

    Public Overrides Function GetXItemInfo() As String
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Return BitNET.GetMiningInfo()
    End Function

#Region "Bitcoin Account Form"

    Public Function GetWalletInfo() As String
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Return BitNET.GetWalletInfo()
    End Function

    Public Function GetFeeBTCPerKiloByte(Optional ByVal Blocks As Integer = 1) As Double
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        '<feerate>0.00001000</feerate><blocks>2</blocks>
        Dim XMLStr As String = BitNET.GetFee(Blocks)
        Return GetDoubleBetween(XMLStr, "<feerate>", "</feerate>")
    End Function

    Public Function GetFeeNQT(Optional ByVal PerByte As Boolean = False, Optional ByVal Blocks As Integer = 1) As ULong

        Dim FeeNQT As ULong = ClsSignumAPI.Dbl2Planck(GetFeeBTCPerKiloByte(Blocks))

        If PerByte Then

            If FeeNQT / 1024 < 1 Then
                Return 1
            Else
                Return Convert.ToInt64(FeeNQT / 1024)
            End If

        Else
            Return FeeNQT
        End If

    End Function

    Public Function AbortReScan() As String
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Return BitNET.AbortReScan()
    End Function

    'Public Function GetBitcoinRawTransaction(ByVal RawTransaction As String) As String
    '    Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
    '    Return BitNET.GetRawTransaction(RawTransaction)
    'End Function

    'Public Function GetBitcoinTransaction(ByVal Transaction As String) As String
    '    Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
    '    Return BitNET.GetTransaction(Transaction)
    'End Function

    'Public Function GetBitcoinTransactionOutput(ByVal Transaction As String, Optional ByVal OutputIndex As Integer = 0) As String
    '    Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
    '    Return BitNET.GetTXOut(Transaction, OutputIndex)
    'End Function

    'Public Function DecodeRawBitcoinTransaction(ByVal RawTransaction As String) As String
    '    Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
    '    Return BitNET.DecodeRawTX(RawTransaction)
    'End Function

    'Public Function DecodeBitcoinScript(ByVal RawBitcoinScript As String) As String
    '    Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
    '    Return BitNET.DecodeScript(RawBitcoinScript)
    'End Function

    Public Function SendRawBitcoinTransaction(ByVal SignedRawBitcoinTransaction As String) As String
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()

        'TODO: Bitcoin filter errormessage

        '{"result":null,"error":{"code":-26,"message":"mandatory-script-verify-flag-failed (Signature must be zero for failed CHECK(MULTI)SIG operation)"},"id":1}
        '{"result":null,"error":{"code":-26,"message":"mandatory-script-verify-flag-failed (Attempted to use a disabled opcode)"},"id":1}
        '{"result":null,"error":{"code":-26,"message":"mandatory-script-verify-flag-failed (unknown error)"},"id":1}
        '{"result":null,"error":{"code":-26,"message":"mandatory-script-verify-flag-failed (NOPx reserved for soft-fork upgrades)"},"id":1}
        '{"result":null,"error":{"code":-26,"message":"mandatory-script-verify-flag-failed (Operation not valid with the current stack size)"},"id":1}
        '{"result":null,"error":{"code":-26,"message":"mandatory-script-verify-flag-failed (Opcode missing or not understood)"},"id":1}
        '{"result":"dfbb5c90bd4e1bd608f9b6c9dbece97f3802f7b31de87d16d888816077f22ed5","error":null,"id":1}

        Dim Response As String = BitNET.SendRawTX(SignedRawBitcoinTransaction)

        Dim Erro As String = GetStringBetween(Response, "<error>", "</error>")
        If Erro.Trim <> "" Then
            Return Application.ProductName + "-error in SendRawBitcoinTransaction(): -> " + vbCrLf + Erro
        Else
            Return Response
        End If

    End Function

    Public Function CreateNewBitcoinWallet(ByVal WalletName As String) As Boolean
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As Boolean = BitNET.CreateNewWallet(WalletName)
        Return Result
    End Function

    Public Function LoadBitcoinWallet(ByVal WalletName As String) As Boolean
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As Boolean = BitNET.LoadWallet(WalletName)
        Return Result
    End Function

    Public Function UnloadBitcoinWallet(ByVal WalletName As String) As Boolean
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As Boolean = BitNET.UnloadWallet(WalletName)
        Return Result
    End Function

    Public Function CreateNewBitcoinAddress(ByVal Address As String) As Boolean
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Return BitNET.CreateNewAddress(Address)
    End Function

    Public Function ImportNewBitcoinAddress(ByVal Address As String) As Boolean
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Return BitNET.ImportAddress(Address)
    End Function

#End Region

    Public Function CreateBitcoinTransaction(ByVal Amount As Double, ByVal RecipientAddress As String, Optional ByVal FilterSenderAddress As String = "") As ClsTransaction
        Dim T_BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim T_UTXOs As List(Of ClsBitcoinNET.S_UnspentTransactionOutput) = T_BitNet.GetFilteredTransactions(FilterSenderAddress, Amount)

        Dim T_Trx As ClsTransaction = New ClsTransaction(T_UTXOs)
        T_Trx.CreateOutput(RecipientAddress, Amount)
        T_Trx.FinalizingOutputs(FilterSenderAddress)
        Return T_Trx

    End Function

    Public Function CreateBitcoinTransaction(ByVal Amount As Double, ByVal RecipientAddress As String, Optional ByVal FilterSenderAddress As String = "", Optional ByVal LockTime As Integer = 12) As ClsTransaction
        Dim T_BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim T_UTXOs As List(Of ClsBitcoinNET.S_UnspentTransactionOutput) = T_BitNet.GetFilteredTransactions(FilterSenderAddress, Amount)

        Dim T_Trx As ClsTransaction = New ClsTransaction(T_UTXOs)

        T_Trx.CreateOutput(RecipientAddress, Amount, LockTime)

        T_Trx.FinalizingOutputs(FilterSenderAddress)
        Return T_Trx

    End Function

    Public Function CreateBitcoinTransaction(ByVal Amount As Double, ByVal RecipientAddress As String, ByVal FilterSenderAddress As String, ByVal ChainSwapHash As String, Optional ByVal LockTime As Integer = -1) As ClsTransaction
        Dim T_BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim T_UTXOs As List(Of ClsBitcoinNET.S_UnspentTransactionOutput) = T_BitNet.GetFilteredTransactions(FilterSenderAddress, Amount)

        Dim T_Trx As ClsTransaction = New ClsTransaction(T_UTXOs)
        If LockTime <= 0 Then
            T_Trx.CreateOutput(ChainSwapHash, RecipientAddress, Amount)
        Else
            T_Trx.CreateOutput(RecipientAddress, ChainSwapHash, FilterSenderAddress, Amount, LockTime)
        End If

        T_Trx.FinalizingOutputs(FilterSenderAddress)
        Return T_Trx

    End Function

    'Public Function RedeemBitcoinTransaction(ByVal BitcoinTransactionID As String, ByVal SenderRecipientAddress As String, ByVal RedeemScript As String) As ClsTransaction

    '    Dim T_Trx As ClsTransaction = New ClsTransaction(BitcoinTransactionID, New List(Of String)({SenderRecipientAddress}), RedeemScript)

    '    For i As Integer = 0 To T_Trx.Inputs.Count - 1

    '        Dim T_UnspentOutput As ClsUnspentOutput = T_Trx.Inputs(i)

    '        If T_UnspentOutput.OutputType = AbsClsOutputs.E_Type.ChainSwapHash Or T_UnspentOutput.OutputType = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime Then
    '            If T_UnspentOutput.Addresses.Contains(SenderRecipientAddress) Then
    '                T_Trx.CreateOutput(SenderRecipientAddress, Satoshi2Dbl(T_UnspentOutput.AmountNQT))
    '            End If
    '        End If

    '        'T_Trx.Inputs(i) = T_UnspentOutput

    '    Next

    '    T_Trx.FinalizingOutputs(SenderRecipientAddress)

    '    Return T_Trx

    'End Function


    Public Function SignBitcoinTransaction(ByVal Transaction As ClsTransaction, ByVal PrivateKey As String) As String
        Return Transaction.SignTransaction(PrivateKey)
    End Function

    'Public Function SignBitcoinTransactionWithChainSwapKey(ByVal Transaction As ClsTransaction, ByVal PrivateKey As String, ByVal ChainSwapKey As String) As String
    '    Return Transaction.SignTransaction(PrivateKey, ChainSwapKey)
    'End Function

    'Public Function SignBitcoinTransactionWithChainSwapKeyAndRedeemScript(ByVal Transaction As ClsTransaction, ByVal PrivateKey As String, ByVal ChainSwapKey As String) As String
    '    Return Transaction.SignTransaction(PrivateKey, ChainSwapKey)
    'End Function

    'Public Function SignBitcoinTransactionWithRedeemScript(ByVal Transaction As ClsTransaction, ByVal PrivateKey As String) As String
    '    Return Transaction.SignTransaction(PrivateKey, "")
    'End Function


    Public Function SendXItemTransaction(ByVal RecipientAddress As String, ByVal Amount As Double) As String

        Dim T_BTCTransaction As ClsTransaction = CreateBitcoinTransaction(Amount, RecipientAddress, GetBitcoinMainAddress())
        Dim BitcoinPrivateKey As String = GetBitcoinMainPrivateKey(False).ToLower()

        If Not BitcoinPrivateKey = "" Then
            T_BTCTransaction.C_FeesNQTPerByte = GetFeeNQT(True)
            Dim T_BitcoinRAWTX As String = SignBitcoinTransaction(T_BTCTransaction, BitcoinPrivateKey) 'AtomicSwap: Sign BitcoinTX with PrivateKey

            If Not T_BitcoinRAWTX.Trim = "" Then
                Return SendRawBitcoinTransaction(T_BitcoinRAWTX)
            End If

        End If

        Return Application.ProductName + "-error in SendXItemTransaction() -> No Keys"

    End Function

    Public Structure S_Transaction
        Dim TransactionID As String
        Dim ScriptHex As String
        Sub New(Optional ByVal ID As String = "", Optional ByVal Script As String = "")
            TransactionID = ID
            ScriptHex = Script
        End Sub
    End Structure

    Public Overrides Function CreateXItemTransactionWithChainSwapHash(ByVal RecipientAddress As String, ByVal Amount As Double, ByVal ChainSwapHash As String) As S_Transaction

        Dim T_Transaction As S_Transaction = New S_Transaction(Application.ProductName + "-error in CreateXItemTransactionWithChainSwapHash(AtomicSwapError) -> No Keys",)
        Dim T_BTCTransaction As ClsTransaction = CreateBitcoinTransaction(Amount, RecipientAddress, GetBitcoinMainAddress(), ChainSwapHash, 12) '12 * ~10min/block = 120min = 2 hours locktime
        Dim BitcoinPrivateKey As String = GetBitcoinMainPrivateKey(False).ToLower()

        If Not BitcoinPrivateKey = "" Then
            T_BTCTransaction.C_FeesNQTPerByte = GetFeeNQT(True)
            Dim T_BitcoinRAWTX As String = SignBitcoinTransaction(T_BTCTransaction, BitcoinPrivateKey) 'AtomicSwap: Sign BitcoinTX with PrivateKey

            If Not T_BitcoinRAWTX.Trim = "" Then
                T_Transaction.TransactionID = SendRawBitcoinTransaction(T_BitcoinRAWTX)
            End If

            For Each T_RedeemOutput As ClsOutput In T_BTCTransaction.Outputs

                If T_RedeemOutput.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then
                    T_Transaction.ScriptHex = T_RedeemOutput.ScriptHex
                    Exit For
                End If

            Next

            Return T_Transaction

        Else
            Return T_Transaction
        End If

    End Function

    Public Overrides Function CheckXItemTransactionConditions(ByVal XItemTransaction As String, ByVal ChainSwapHash As String) As Boolean

        Dim RedeemScript As String = GetBitcoinRedeemScriptFromINI(XItemTransaction, ChainSwapHash)

        Dim T_BTCTransaction As ClsTransaction = New ClsTransaction(XItemTransaction, New List(Of String))

        Dim InputConfirmations As Integer = 0
        Dim InputScript As List(Of ClsScriptEntry) = New List(Of ClsScriptEntry)

        For Each x In T_BTCTransaction.Inputs

            If x.OutputType = AbsClsOutputs.E_Type.Pay2ScriptHash Then
                InputConfirmations = x.Confirmations
                InputScript = x.Script
                Exit For
            End If

        Next

        If InputScript.Count = 0 Then
            Return False
        End If

        If InputConfirmations <= 10 Then

            Dim InputScriptHash As String = ClsBitcoinNET.GetXFromScript(InputScript, ClsScriptEntry.E_OP_Code.ScriptHash)
            Dim Script As List(Of ClsScriptEntry) = ClsTransaction.ConvertLockingScriptStrToList(RedeemScript)
            Dim T_Output As ClsOutput = New ClsOutput(Script)
            Dim RedeemScriptHash As String = PubKeyToRipe160(T_Output.ScriptHex)

            If RedeemScriptHash = InputScriptHash Then

                Dim SType As AbsClsOutputs.E_Type = ClsTransaction.GetScriptType(Script)

                If SType = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime Then

                    Dim T_LockTime As String = ClsBitcoinNET.GetXFromScript(Script, ClsScriptEntry.E_OP_Code.LockTime)

                    Dim LockTime As Integer = 0

                    Select Case T_LockTime.Length
                        Case 2
                            LockTime = Convert.ToInt32(T_LockTime, 16)
                        Case 4
                            T_LockTime = T_LockTime.Substring(2)
                            LockTime = Convert.ToInt32(T_LockTime, 16)
                        Case 6
                            T_LockTime = T_LockTime.Substring(4) + T_LockTime.Substring(2, 2)
                            LockTime = Convert.ToInt32(T_LockTime, 16)
                    End Select

                    If LockTime >= 12 Then
                        Return True
                    End If

                End If
            End If
        End If

        Return False

    End Function

    Public Overrides Function CheckChainSwapHash(ByVal ChainSwapHash As String) As String

        If MessageIsHEXString(ChainSwapHash) And ChainSwapHash.Length <> 64 Then
            Dim Script As List(Of ClsScriptEntry) = ClsTransaction.ConvertLockingScriptStrToList(ChainSwapHash)
            Return ClsBitcoinNET.GetXFromScript(Script, ClsScriptEntry.E_OP_Code.ChainSwapHash)
        End If

        Return ChainSwapHash

    End Function

    Public Overrides Function ClaimXItemTransactionWithChainSwapKey(ByVal TransactionID As String, ByVal ChainSwapHash As String, ByVal ChainSwapKey As String) As String

        Dim RedeemScript As String = GetBitcoinRedeemScriptFromINI(TransactionID, ChainSwapHash)
        Dim T_BTCTransaction As ClsTransaction = New ClsTransaction(TransactionID, GetBitcoinMainAddress(), RedeemScript)

        For i As Integer = 0 To T_BTCTransaction.Inputs.Count - 1

            Dim T_UnspentOutput As ClsUnspentOutput = T_BTCTransaction.Inputs(i)

            If T_UnspentOutput.OutputType = AbsClsOutputs.E_Type.ChainSwapHash Or T_UnspentOutput.OutputType = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime Then
                If T_UnspentOutput.Addresses.Contains(GetBitcoinMainAddress()) Then
                    T_BTCTransaction.CreateOutput(GetBitcoinMainAddress(), Satoshi2Dbl(T_UnspentOutput.AmountNQT))
                End If
            End If

        Next

        T_BTCTransaction.FinalizingOutputs(GetBitcoinMainAddress())

        Dim T_BitcoinRAWTX As String = T_BTCTransaction.SignTransaction(GetBitcoinMainPrivateKey(False).ToLower(), ChainSwapKey)

        Dim T_BitcoinTXID As String = Application.ProductName + "-error in ClaimXItemTransactionWithChainSwapKey(AtomicSwapError) -> No Keys"
        If Not T_BitcoinRAWTX.Trim = "" Then
            T_BitcoinTXID = SendRawBitcoinTransaction(T_BitcoinRAWTX)
        End If

        Return T_BitcoinTXID

    End Function

    Public Function ClaimBitcoinTransaction(ByVal TransactionID As String, ByVal RedeemScript As String) As ClsTransaction

        Dim T_BTCTransaction As ClsTransaction = New ClsTransaction(TransactionID, GetBitcoinMainAddress(), RedeemScript)

        For i As Integer = 0 To T_BTCTransaction.Inputs.Count - 1

            Dim T_UnspentOutput As ClsUnspentOutput = T_BTCTransaction.Inputs(i)

            If T_UnspentOutput.OutputType = AbsClsOutputs.E_Type.ChainSwapHash Or T_UnspentOutput.OutputType = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime Then
                If T_UnspentOutput.Addresses.Contains(GetBitcoinMainAddress()) Then
                    T_BTCTransaction.CreateOutput(GetBitcoinMainAddress(), Satoshi2Dbl(T_UnspentOutput.AmountNQT))
                End If
            End If

        Next

        T_BTCTransaction.FinalizingOutputs(GetBitcoinMainAddress())

        'Dim T_BitcoinRAWTX As String = T_BTCTransaction.SignTransaction(GetBitcoinMainPrivateKey(True).ToLower())

        'Dim T_BitcoinTXID As String = Application.ProductName + "-error in ClaimBitcoinTransaction() -> No Keys"
        'If Not T_BitcoinRAWTX.Trim = "" Then
        '    T_BitcoinTXID = SendRawBitcoinTransaction(T_BitcoinRAWTX)
        'End If

        Return T_BTCTransaction

    End Function

    Public Overrides Function GetBackXItemTransaction(ByVal TransactionID As String, ByVal ChainSwapHash As String) As String

        Dim RedeemScript As String = GetBitcoinRedeemScriptFromINI(TransactionID, ChainSwapHash)

        Dim T_BitcoinTransaction As ClsTransaction = New ClsTransaction(TransactionID, GetBitcoinMainAddress(), RedeemScript)

        If T_BitcoinTransaction.IsTimeOut() Then

            Dim BitcoinPrivateKey As String = GetBitcoinMainPrivateKey(False).ToLower()

            If Not BitcoinPrivateKey = "" Then

                T_BitcoinTransaction.C_FeesNQTPerByte = GetFeeNQT(True)

                Dim T_BitcoinRAWTX As String = T_BitcoinTransaction.SignTransaction(BitcoinPrivateKey, "")

                Dim T_BitcoinTXID As String = ""
                If Not T_BitcoinRAWTX.Trim = "" Then
                    T_BitcoinTXID = SendRawBitcoinTransaction(T_BitcoinRAWTX)
                End If

                Return T_BitcoinTXID
            Else
                Return Application.ProductName + "-error in GetBackXItemTransaction(AtomicSwapError) -> No Keys"
            End If
        Else
            Return Application.ProductName + "-error in GetBackXItemTransaction(AtomicSwapError) -> Timeout not yet expired"
        End If

    End Function

End Class
