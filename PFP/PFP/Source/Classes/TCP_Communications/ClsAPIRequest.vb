
Imports System.Windows.Input

Public Class ClsAPIRequest

    Public ReadOnly Property C_Interface As String = "API"
    Public ReadOnly Property C_Version As String = "v1.0"

    Private Property C_RAWRequest As String = ""
    Private Property C_RAWRequestList As List(Of String) = New List(Of String)

    Private Property C_Method As E_Method = E_Method.NONE
    Public ReadOnly Property Method As E_Method
        Get
            Return C_Method
        End Get
    End Property

    Private Property C_Command As E_Command = E_Command.NONE
    Public ReadOnly Property Command As E_Command
        Get
            Return C_Command
        End Get
    End Property

    Private Property C_Parameters As List(Of S_Parameter) = New List(Of S_Parameter)
    Public ReadOnly Property Parameters As List(Of S_Parameter)
        Get
            Return C_Parameters
        End Get
    End Property


    Public Enum E_Method
        NONE = 0
        HTTP_GET = 1
        HTTP_POST = 2
    End Enum
    Public Enum E_Path
        Intface = 0
        Version = 1
        Command = 2
    End Enum
    Private Structure S_PathValues
        Dim Path As E_Path
        Dim Value As String
    End Structure
    Private Property PathValues As List(Of S_PathValues) = New List(Of S_PathValues)

    Public Enum E_Command
        NONE = 0
        GetInfo = 1
        GetCandles = 2
        GetOpenOrders = 3
        AcceptOrder = 4

        CreateBitcoinTransaction = 5

    End Enum

    Public Enum E_Parameter

        NONE = 0

        pair = 1
        days = 2
        tickmin = 3

        DEXContractAddress = 4
        DEXContractID = 5
        PublicKey = 6

        BitcoinTransaction = 7
        BitcoinOutputType = 8
        BitcoinAmountNQT = 9
        BitcoinSenderAddress = 10
        BitcoinRecipientAddress = 11
        BitcoinChainSwapHash = 12
        BitcoinTimeOut = 13

    End Enum

    Public Enum E_BitcoinType
        NONE = 0
        Standard = 1
        TimeLock = 2
        ChainSwapHash = 3
        TimeLockChainSwapHash = 4
    End Enum

    Public Structure S_Parameter
        Dim Parameter As E_Parameter
        Dim Value As String
    End Structure

    Sub New(ByVal RAWRequest As String)
        C_RAWRequest = RAWRequest
        GetRAWRequestList()
        GetMethodAndPath()
        GetCommand()
    End Sub

    Private Sub GetRAWRequestList()
        C_RAWRequestList = C_RAWRequest.Split(Convert.ToChar(vbCr)).ToList()
    End Sub

    Private Sub GetMethodAndPath()

        If C_RAWRequestList.Count > 0 Then

            Dim Method_Path_HTTPv As List(Of String) = New List(Of String)(C_RAWRequestList(0).Split(" "c))
            If Method_Path_HTTPv.Count > 0 Then
                If Method_Path_HTTPv(0).Trim().ToUpper() = "GET" Then
                    C_Method = E_Method.HTTP_GET
                ElseIf Method_Path_HTTPv(0).Trim().ToUpper() = "POST" Then
                    C_Method = E_Method.HTTP_POST
                End If
            End If


            If Method_Path_HTTPv.Count > 1 Then

                Dim Request As String = Method_Path_HTTPv(1).Trim()
                Dim SubRequest As List(Of String) = New List(Of String)

                If Request.Contains("/") Then
                    SubRequest.AddRange(Request.Split("/"c))
                End If

                If SubRequest.Count > 0 Then
                    If SubRequest(0).Trim = "" Then
                        SubRequest.RemoveAt(0)
                    End If
                End If

                If SubRequest.Count >= 3 Then

                    If SubRequest(0) = C_Interface Then
                        Dim IFace As S_PathValues = New S_PathValues
                        IFace.Path = E_Path.Intface
                        IFace.Value = SubRequest(0)
                        PathValues.Add(IFace)
                    Else
                        Exit Sub
                    End If


                    If SubRequest(1) = C_Version Then
                        Dim Version As S_PathValues = New S_PathValues
                        Version.Path = E_Path.Version
                        Version.Value = SubRequest(1)
                        PathValues.Add(Version)
                    Else
                        Exit Sub
                    End If


                    Dim Command As S_PathValues = New S_PathValues
                    Command.Path = E_Path.Command
                    Command.Value = SubRequest(2)
                    PathValues.Add(Command)

                End If

            End If

        End If

    End Sub

    Private Sub GetCommand()

        If PathValues.Count > 0 Then

            Dim Command As S_PathValues = PathValues.FirstOrDefault(Function(c) c.Path = E_Path.Command)

            If Command.Value.Contains("?") Then

                Dim T_Command As String = Command.Value.Remove(Command.Value.IndexOf("?"))
                If [Enum].GetNames(GetType(E_Command)).ToList().Contains(T_Command) Then
                    C_Command = [Enum].Parse(GetType(E_Command), T_Command)
                End If

                Dim T_Parameters As String = Command.Value.Substring(Command.Value.IndexOf("?") + 1)

                Dim QueryParameters As List(Of String) = New List(Of String)
                QueryParameters.AddRange(T_Parameters.Split("&"c).ToArray())

                Dim ParameterNames As List(Of E_Parameter) = New List(Of E_Parameter)([Enum].GetValues(GetType(E_Parameter)))

                For Each T_QueryParameter As String In QueryParameters
                    For Each T_Parameter As E_Parameter In ParameterNames
                        If T_QueryParameter.Contains(T_Parameter.ToString()) Then
                            Dim Parameter As S_Parameter = New S_Parameter
                            Parameter.Parameter = T_Parameter

                            Select Case Parameter.Parameter
                                Case E_Parameter.BitcoinOutputType
                                    Dim T_BTCTypes As List(Of E_BitcoinType) = New List(Of E_BitcoinType)([Enum].GetValues(GetType(E_BitcoinType)))
                                    Dim T_Value As String = T_QueryParameter.Substring(T_QueryParameter.IndexOf("=") + 1)

                                    Parameter.Value = T_BTCTypes.FirstOrDefault(Function(btc) btc.ToString() = T_Value).ToString()

                                Case Else
                                    Parameter.Value = T_QueryParameter.Substring(T_QueryParameter.IndexOf("=") + 1)
                            End Select

                            C_Parameters.Add(Parameter)
                        End If
                    Next
                Next
            Else

                If [Enum].GetNames(GetType(E_Command)).ToList().Contains(Command.Value) Then
                    C_Command = [Enum].Parse(GetType(E_Command), Command.Value)
                End If

            End If

        End If

    End Sub

End Class
