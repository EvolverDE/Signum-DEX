Option Explicit On
Option Strict On
Imports PFP.ClsBitcoin

Public MustInherit Class AbsClsXItem

#Region "INISettings"

    Public MustOverride Function SetXItemTransactionToINI(ByVal DEXContract As ClsDEXContract, ByVal XItemTransactionID As String, ByVal ChainSwapKey As String, ByVal ChainSwapHash As String) As Boolean

    Public MustOverride Function SetXItemTransactionToINI(ByVal DEXContract As ClsDEXContract, ByVal XItemTransactionID As String, ByVal ChainSwapHash As String) As Boolean

    Public MustOverride Function SetXItemTransactionToINI(ByVal DEXContractID As ULong, ByVal OrderID As ULong, ByVal XItemTransactionID As String, ByVal ChainSwapKey As String, ByVal ChainSwapHash As String) As Boolean

    Public MustOverride Function GetXItemChainSwapKeyFromINI(ByVal DEXContractID As ULong, ByVal OrderID As ULong, ByVal XItemTransactionID As String) As String

    Public MustOverride Function GetXItemChainSwapHashFromINI(ByVal DEXContractID As ULong, ByVal OrderID As ULong, ByVal XItemTransactionID As String) As String

    'Public MustOverride Function GetXItemRedeemScriptFromINI(ByVal DEXContractID As ULong, ByVal OrderID As ULong, ByVal BTCTXID As String) As String

    Public MustOverride Function GetXItemTransactionFromINI(ByVal DEXContractID As ULong, ByVal OrderID As ULong) As String

    Public MustOverride Function GetXItemTransactionFromINI(ByVal DEXContractID As ULong, ByVal OrderID As ULong, ByVal ChainSwapHash As String) As String

    Public MustOverride Function DelXItemTransactionFromINI(ByVal DEXContractID As ULong, ByVal OrderID As ULong, ByVal XItemTransactionID As String, ByVal ChainSwapHash As String) As Boolean

#End Region

    Public MustOverride Function CheckChainSwapHash(ByVal ChainSwapHash As String) As String

    Public MustOverride Function GetXItemInfo() As String

    Public MustOverride Function CreateXItemTransactionWithChainSwapHash(ByVal RecipientAddress As String, ByVal Amount As Double, ByVal ChainSwapHash As String) As S_Transaction
    Public MustOverride Function ClaimXItemTransactionWithChainSwapKey(ByVal XItemTransactionID As String, ByVal ChainSwapHash As String, ByVal ChainSwapKey As String) As String

    Public MustOverride Function CheckXItemTransactionConditions(ByVal XItemTransactionID As String, ByVal RedeemScript As String) As Boolean

    Public MustOverride Function GetBackXItemTransaction(ByVal XItemTransactionID As String, ByVal ChainSwapHash As String) As String

End Class
