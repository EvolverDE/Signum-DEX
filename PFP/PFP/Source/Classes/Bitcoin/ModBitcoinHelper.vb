
Option Strict On
Option Explicit On

''' <summary>
''' this module is to simplify global using
''' </summary>
Module ModBitcoinHelper

    Function GetBitcoinMiningInfo() As String
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Return BitNET.GetMiningInfo()
    End Function

    Function GetBitcoinRawTransaction(ByVal RawTransaction As String) As String
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Return BitNET.GetRawTransaction(RawTransaction)
    End Function

    Function GetBitcoinTransaction(ByVal Transaction As String) As String
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Return BitNET.GetTransaction(Transaction)
    End Function

    Function GetBitcoinTransactionOutput(ByVal Transaction As String, Optional ByVal OutputIndex As Integer = 0) As String
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Return BitNET.GetTXOut(Transaction, OutputIndex)
    End Function

    Function DecodeRawBitcoinTransaction(ByVal RawTransaction As String) As String
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Return BitNET.DecodeRawTX(RawTransaction)
    End Function

    Function DecodeBitcoinScript(ByVal RawBitcoinScript As String) As String
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Return BitNET.DecodeScript(RawBitcoinScript)
    End Function

    Function SendRawBitcoinTransaction(ByVal SignedRawBitcoinTransaction As String) As String
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

    Function CreateNewBitcoinWallet(ByVal WalletName As String) As Boolean
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As Boolean = BitNET.CreateNewWallet(WalletName)
        Return Result
    End Function

    Function LoadBitcoinWallet(ByVal WalletName As String) As Boolean
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As Boolean = BitNET.LoadWallet(WalletName)
        Return Result
    End Function

    Function UnloadBitcoinWallet(ByVal WalletName As String) As Boolean
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Dim Result As Boolean = BitNET.UnloadWallet(WalletName)
        Return Result
    End Function

    Function CreateNewBitcoinAddress(ByVal Address As String) As Boolean
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Return BitNET.CreateNewAddress(Address)
    End Function

    Function ImportNewBitcoinAddress(ByVal Address As String) As Boolean
        Dim BitNET As ClsBitcoinNET = New ClsBitcoinNET()
        Return BitNET.ImportAddress(Address)
    End Function


    Function CreateBitcoinTransaction(ByVal Amount As Double, ByVal RecipientAddress As String, Optional ByVal FilterSenderAddress As String = "") As ClsTransaction
        Dim T_BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim T_UTXOs As List(Of ClsBitcoinNET.S_UnspentTransactionOutput) = T_BitNet.GetFilteredTransactions(FilterSenderAddress, Amount)

        Dim T_Trx As ClsTransaction = New ClsTransaction(T_UTXOs)
        T_Trx.CreateOutput(RecipientAddress, Amount)
        T_Trx.FinalizingOutputs(FilterSenderAddress)
        Return T_Trx

    End Function

    Function CreateBitcoinTransaction(ByVal Amount As Double, ByVal RecipientAddress As String, ByVal FilterSenderAddress As String, ByVal ChainSwapHash As String, Optional ByVal LockTime As Integer = 12) As ClsTransaction
        Dim T_BitNet As ClsBitcoinNET = New ClsBitcoinNET()
        Dim T_UTXOs As List(Of ClsBitcoinNET.S_UnspentTransactionOutput) = T_BitNet.GetFilteredTransactions(FilterSenderAddress, Amount)

        Dim T_Trx As ClsTransaction = New ClsTransaction(T_UTXOs)

        T_Trx.CreateOutput(RecipientAddress, ChainSwapHash, FilterSenderAddress, Amount, LockTime)
        T_Trx.FinalizingOutputs(FilterSenderAddress)
        Return T_Trx

    End Function


    Function RedeemBitcoinTransaction(ByVal BitcoinTransactionID As String, ByVal SenderRecipientAddress As String, ByVal RedeemScript As String) As ClsTransaction

        Dim T_Trx As ClsTransaction = New ClsTransaction(BitcoinTransactionID, New List(Of String)({SenderRecipientAddress}), RedeemScript)

        For i As Integer = 0 To T_Trx.Inputs.Count - 1

            Dim T_UnspentOutput As ClsUnspentOutput = T_Trx.Inputs(i)

            If T_UnspentOutput.OutputType = AbsClsOutputs.E_Type.ChainSwapHash Or T_UnspentOutput.OutputType = AbsClsOutputs.E_Type.ChainSwapHashWithLockTime Then
                If T_UnspentOutput.Addresses.Contains(SenderRecipientAddress) Then
                    T_Trx.CreateOutput(SenderRecipientAddress, Satoshi2Dbl(T_UnspentOutput.AmountNQT))
                End If
            End If

            'T_Trx.Inputs(i) = T_UnspentOutput

        Next

        T_Trx.FinalizingOutputs(SenderRecipientAddress)

        Return T_Trx

    End Function



    Function SignBitcoinTransaction(ByVal Transaction As ClsTransaction, ByVal PrivateKey As String) As String
        Return Transaction.SignTransaction(PrivateKey)
    End Function

    Function SignBitcoinTransactionWithChainSwapKey(ByVal Transaction As ClsTransaction, ByVal PrivateKey As String, ByVal ChainSwapKey As String) As String
        Return Transaction.SignTransaction(PrivateKey, ChainSwapKey)
    End Function

    Function SignBitcoinTransactionWithChainSwapKeyAndRedeemScript(ByVal Transaction As ClsTransaction, ByVal PrivateKey As String, ByVal ChainSwapKey As String) As String
        Return Transaction.SignTransaction(PrivateKey, ChainSwapKey)
    End Function

    Function SignBitcoinTransactionWithRedeemScript(ByVal Transaction As ClsTransaction, ByVal PrivateKey As String) As String
        Return Transaction.SignTransaction(PrivateKey, "")
    End Function

End Module
