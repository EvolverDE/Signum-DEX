
Imports System.Resources.ResXFileRef

Public Class ClsSignumSmartContract

    Private Property C_SignumAPI As ClsSignumAPI = Nothing

    Private Property C_Creator As ULong = 0UL
    Public ReadOnly Property Creator As ULong
        Get
            Return C_Creator
        End Get
    End Property

    Private Property C_CreatorAddress As String = ""
    Public ReadOnly Property CreatorAddress As String
        Get
            Return C_CreatorAddress
        End Get
    End Property

    Private Property C_AutomatedTransaction As ULong = 0UL
    Public ReadOnly Property AutomatedTransaction As ULong
        Get
            Return C_AutomatedTransaction
        End Get
    End Property

    Private Property C_AutomatedTransactionAddress As String = ""
    Public ReadOnly Property AutomatedTransactionAddress As String
        Get
            Return C_AutomatedTransactionAddress
        End Get
    End Property

    Private Property C_Name As String = ""
    Public ReadOnly Property Name As String
        Get
            Return C_Name
        End Get
    End Property

    Private Property C_Description As String = ""
    Public ReadOnly Property Description As String
        Get
            Return C_Description
        End Get
    End Property

    Private Property C_MachineCode As String = ""
    Public ReadOnly Property MachineCode As String
        Get
            Return C_MachineCode
        End Get
    End Property

    Private Property C_IsReferenceMachineCode As Boolean = False
    Public ReadOnly Property IsReferenceMachineCode As Boolean
        Get
            Return C_IsReferenceMachineCode
        End Get
    End Property

    Private Property C_MachineCodeHashId As ULong = 0UL
    Public ReadOnly Property MachineCodeHashId As ULong
        Get
            Return C_MachineCodeHashId
        End Get
    End Property

    Private Property C_MachineData As String = ""
    Public ReadOnly Property MachineData As String
        Get
            Return C_MachineData
        End Get
    End Property

    Private Property C_MachineDataULongs As List(Of ULong) = New List(Of ULong)
    Public ReadOnly Property MachineDataULongs As List(Of ULong)
        Get
            Return C_MachineDataULongs
        End Get
    End Property

    'Private Property C_CreationMachineData As String = "0000000000000000000000000000000000000000000000000100000000000000020000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001f00000000000000"
    Public Shared ReadOnly Property CreationMachineData As String = "0000000000000000000000000000000000000000000000000100000000000000020000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001f00000000000000"

    Public Shared ReadOnly Property ReferenceMachineData As String = CreationMachineData + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"

    Private Property C_ReferenceMachineData As String = ""

    Private Property C_IsReferenceMachineData As Boolean = False
    Public ReadOnly Property IsReferenceMachineData As Boolean
        Get
            Return C_IsReferenceMachineData
        End Get
    End Property


    Private Property C_BalanceNQT As ULong = 0UL
    Public ReadOnly Property BalanceNQT As ULong
        Get
            Return C_BalanceNQT
        End Get
    End Property

    Private Property C_Frozen As Boolean = False
    Public ReadOnly Property Frozen As Boolean
        Get
            Return C_Frozen
        End Get
    End Property

    Private Property C_Running As Boolean = False
    Public ReadOnly Property Running As Boolean
        Get
            Return C_Running
        End Get
    End Property

    Private Property C_Stopped As Boolean = False
    Public ReadOnly Property Stopped As Boolean
        Get
            Return C_Stopped
        End Get
    End Property

    Private Property C_Finished As Boolean = False
    Public ReadOnly Property Finished As Boolean
        Get
            Return C_Finished
        End Get
    End Property

    Private Property C_Dead As Boolean = True
    Public ReadOnly Property Dead As Boolean
        Get
            Return C_Dead
        End Get
    End Property

    Private Property C_BaseSignumTransaction As ClsSignumTransaction = Nothing
    Public ReadOnly Property BaseSignumTransaction As ClsSignumTransaction
        Get
            Return C_BaseSignumTransaction
        End Get
    End Property

    Private Property C_IsReferenceSmartContract As Boolean = False

    Public ReadOnly Property IsReferenceSmartContract As Boolean
        Get
            Return C_IsReferenceSmartContract
        End Get
    End Property

    Public Sub New(Optional ByVal Node As String = "")

        C_SignumAPI = New ClsSignumAPI(Node)
        C_BaseSignumTransaction = New ClsSignumTransaction(ClsDEXContract._ReferenceTX)

        If C_BaseSignumTransaction.Type = ClsSignumTransaction.E_Type.AUTOMATED_TRANSACTIONS_CREATION Then
            If C_BaseSignumTransaction.Confirmations >= 0 Then
                If IsNothing(GlobalReferenceSignumSmartContract) Then
                    GetSmartContractDetails(ClsDEXContract._ReferenceTX)
                    C_IsReferenceMachineCode = True
                    C_IsReferenceMachineData = True
                Else
                    Exit Sub
                End If
            Else
                Exit Sub
            End If
        Else
            Exit Sub
        End If

    End Sub

    Public Sub New(ByVal ID As ULong, Optional ByVal Node As String = "")

        C_SignumAPI = New ClsSignumAPI(Node)
        C_BaseSignumTransaction = New ClsSignumTransaction(ID)

        If C_BaseSignumTransaction.Type = ClsSignumTransaction.E_Type.AUTOMATED_TRANSACTIONS_CREATION Then
            If C_BaseSignumTransaction.Confirmations >= 0 Then
                If ID = ClsDEXContract._ReferenceTX Then
                    C_IsReferenceSmartContract = True
                    Exit Sub
                Else
                    C_IsReferenceSmartContract = False
                End If

                GetSmartContractDetails(ID)

                If GlobalReferenceSignumSmartContract.MachineCode = C_MachineCode Then
                    C_IsReferenceMachineCode = True
                Else
                    C_IsReferenceMachineCode = False
                End If

                If C_ReferenceMachineData = ReferenceMachineData Then
                    C_IsReferenceMachineData = True
                Else
                    C_IsReferenceMachineData = False
                End If
            Else
                Exit Sub
            End If
        Else
            Exit Sub
        End If

    End Sub

    Private Sub GetSmartContractDetails(ByVal ID As ULong)

        C_AutomatedTransaction = ID

        Dim SmartContractDetailList As List(Of String) = C_SignumAPI.GetSmartContractDetails(C_AutomatedTransaction)

        C_Creator = GetULongBetweenFromList(SmartContractDetailList, "<creator>", "</creator>")
        C_CreatorAddress = GetStringBetweenFromList(SmartContractDetailList, "<creatorRS>", "</creatorRS>")

        C_AutomatedTransactionAddress = GetStringBetweenFromList(SmartContractDetailList, "<atRS>", "</atRS>")

        C_Name = GetStringBetweenFromList(SmartContractDetailList, "<name>", "</name>")
        C_Description = GetStringBetweenFromList(SmartContractDetailList, "<description>", "</description>")

        C_MachineCode = GetStringBetweenFromList(SmartContractDetailList, "<machineCode>", "</machineCode>")

        C_MachineCodeHashId = GetULongBetweenFromList(SmartContractDetailList, "<machineCodeHashId>", "</machineCodeHashId>")

        C_ReferenceMachineData = GetStringBetweenFromList(SmartContractDetailList, "<creationMachineData>", "</creationMachineData>")

        C_MachineData = GetStringBetweenFromList(SmartContractDetailList, "<machineData>", "</machineData>")
        C_MachineDataULongs = ClsSignumAPI.DataStr2ULngList(C_MachineData)

        C_BalanceNQT = GetULongBetweenFromList(SmartContractDetailList, "<balanceNQT>", "</balanceNQT>")
        C_Frozen = GetBooleanBetweenFromList(SmartContractDetailList, "<frozen>", "</frozen>")
        C_Running = GetBooleanBetweenFromList(SmartContractDetailList, "<running>", "</running>")
        C_Stopped = GetBooleanBetweenFromList(SmartContractDetailList, "<stopped>", "</stopped>")
        C_Finished = GetBooleanBetweenFromList(SmartContractDetailList, "<finished>", "</finished>")
        C_Dead = GetBooleanBetweenFromList(SmartContractDetailList, "<dead>", "</dead>")

    End Sub

    Public Sub Refresh()
        GetSmartContractDetails(C_AutomatedTransaction)
    End Sub

End Class
