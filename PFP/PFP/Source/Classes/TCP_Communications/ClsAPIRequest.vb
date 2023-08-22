
Public Class ClsAPIRequest

    Public ReadOnly Property C_Interface As String = "API"
    Public ReadOnly Property C_Version As String = "v1"

    Private Property C_RAWRequest As String = ""
    Private Property C_RAWRequestList As List(Of String) = New List(Of String)

    Private Property C_Method As E_Method = E_Method.NONE
    Public ReadOnly Property Method As E_Method
        Get
            Return C_Method
        End Get
    End Property

    Private Property C_Endpoint As E_Endpoint = E_Endpoint.NONE
    Public ReadOnly Property Endpoint As E_Endpoint
        Get
            Return C_Endpoint
        End Get
    End Property

    Private Property C_Parameters As List(Of S_Parameter) = New List(Of S_Parameter)
    Public ReadOnly Property Parameters As List(Of S_Parameter)
        Get
            Return C_Parameters
        End Get
    End Property

    Private Property C_Body As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))

    Public ReadOnly Property Body As List(Of KeyValuePair(Of String, Object))
        Get
            Return C_Body
        End Get
    End Property

    Public Enum E_Method
        NONE = 0
        HTTP_GET = 1
        HTTP_POST = 2
        HTTP_PUT = 3
        HTTP_DELETE = 4
    End Enum
    'Public Enum E_Path
    '    Intface = 0
    '    Version = 1
    '    Command = 2
    'End Enum
    'Private Structure S_PathValues
    '    Dim Path As E_Path
    '    Dim Value As String
    'End Structure
    'Private Property PathValues As List(Of S_PathValues) = New List(Of S_PathValues)

    Public Enum E_Endpoint

        NONE = 0

        Info = 1
        Candles = 2
        Orders = 3
        Bitcoin = 4

        Broadcast = 5

        SmartContract = 6

    End Enum

    Public Enum E_Parameter

        NONE = 0

        Status = 1
        ID = 2

        Pair = 3
        Days = 4
        Tickmin = 5

        'SenderPublicKey = 6

        Token = 7

        Inputs = 8
        Outputs = 9

        Transaction = 10
        Type = 11
        Script = 12

        Sender = 13
        Change = 14
        Recipient = 15

        PassPhrase = 16
        Mnemonic = 17

        PrivateKey = 18
        PublicKey = 19

        ChainSwapKey = 20
        ChainSwapHash = 21

        AmountNQT = 22
        CollateralNQT = 23
        XAmountNQT = 24
        XItem = 25

        SignedTransactionBytes = 26

    End Enum

    Public Structure S_Parameter
        Dim Parameter As E_Parameter
        Dim Value As String
    End Structure

    Sub New(ByVal RAWRequest As String)
        C_RAWRequest = RAWRequest
        GetRAWRequestList()
        GetMethodAndPath()
    End Sub

    Private Sub GetRAWRequestList()
        C_RAWRequestList = C_RAWRequest.Split(Convert.ToChar(vbCr)).ToList()
    End Sub
    Private Sub GetMethodAndPath()

        If C_RAWRequestList.Count > 0 Then

            Dim FirstRequestLine As String = C_RAWRequestList(0).Replace(" ", "")

            If FirstRequestLine.ToLower().Contains("http") Then

                FirstRequestLine = FirstRequestLine.Remove(FirstRequestLine.ToLower().IndexOf("http"))
                Dim Path As List(Of String) = New List(Of String)(FirstRequestLine.Split("/"c))

                If Path.Count > 3 Then
                    Dim Methods As List(Of E_Method) = New List(Of E_Method)([Enum].GetValues(GetType(E_Method)))
                    Dim Endpoints As List(Of E_Endpoint) = New List(Of E_Endpoint)([Enum].GetValues(GetType(E_Endpoint)))

                    C_Method = Methods.FirstOrDefault(Function(method) method.ToString().ToLower().Contains(Path(0).ToLower()))
                    C_Endpoint = Endpoints.FirstOrDefault(Function(endpoint) endpoint.ToString().ToLower().Contains(Path(3).ToLower()) Or Path(3).ToLower().Contains(endpoint.ToString().ToLower()))

                    If Path(1).ToLower().Contains(C_Interface.ToLower()) And Path(2).ToLower().Contains(C_Version.ToLower()) Then
                        Select Case C_Method

                            Case E_Method.HTTP_GET

                                Select Case C_Endpoint

                                    Case E_Endpoint.Info
                                        'no Parameters -> Done
                                    Case E_Endpoint.Orders

                                        If Path.Count > 4 Then

                                            Dim OrderID As S_Parameter = New S_Parameter
                                            OrderID.Parameter = E_Parameter.ID

                                            'GET/API/v1/Orders/{id}?status=open
                                            If Path(4).Contains("?") Then
                                                OrderID.Value = Path(4).Remove(Path(4).IndexOf("?"c))
                                                Dim ParaString As String = Path(4).Substring(Path(4).IndexOf("?"c) + 1)
                                                C_Parameters.AddRange(ParseParameters(ParaString))
                                            Else
                                                'GET/API/v1/Orders/{id}
                                                OrderID.Value = Path(4)
                                            End If

                                            C_Parameters.Add(OrderID)
                                        Else
                                            'GET/API/v1/Orders?status=open
                                            If Path(3).Contains("?") Then
                                                Dim ParaString As String = Path(3).Substring(Path(3).IndexOf("?"c) + 1)
                                                C_Parameters.AddRange(ParseParameters(ParaString))
                                            End If

                                        End If

                                    Case E_Endpoint.Candles

                                        If Path.Count > 4 Then

                                            If Path(4).Contains("?") Then
                                                'GET/API/v1/Candles/{pair}?days=3&tickmin=15
                                                C_Parameters.Add(CreateParameter(E_Parameter.Pair, Path(4).Remove(Path(4).IndexOf("?"c))))

                                                Dim ParaString As String = Path(4).Substring(Path(4).IndexOf("?"c) + 1)
                                                C_Parameters.AddRange(ParseParameters(ParaString))
                                            Else
                                                'default GET/API/v1/Candles/{pair}
                                                C_Parameters.Add(CreateParameter(E_Parameter.Pair, Path(4)))
                                                C_Parameters.Add(CreateParameter(E_Parameter.Days, "3"))
                                                C_Parameters.Add(CreateParameter(E_Parameter.Tickmin, "15"))
                                            End If

                                        Else
                                            'default: GET/API/v1/Candles
                                            C_Parameters.Add(CreateParameter(E_Parameter.Pair, "USD_SIGNA"))
                                            C_Parameters.Add(CreateParameter(E_Parameter.Days, "3"))
                                            C_Parameters.Add(CreateParameter(E_Parameter.Tickmin, "15"))
                                        End If

                                    Case E_Endpoint.Bitcoin
                                        'GET/API/v1/Bitcoin/Transaction/{inputTransactions}?BitcoinOutputType=TimeLockChainSwapHash&BitcoinSenderAddresses={addresses}&BitcoinRecipientAddresses={addresses}&BitcoinChainSwapHash={chainSwapHash}&BitcoinAmountNQT=2120

                                        If Path.Count > 5 Then

                                            If Path(5).Contains("?") Then

                                                C_Parameters.Add(CreateParameter(E_Parameter.Inputs, Path(5).Remove(Path(5).IndexOf("?"c))))

                                                Dim ParaString As String = Path(5).Substring(Path(5).IndexOf("?"c) + 1)
                                                C_Parameters.AddRange(ParseParameters(ParaString))

                                            Else


                                            End If

                                        End If

                                    Case E_Endpoint.Broadcast

                                        If Path.Count > 4 Then

                                            If Path(4).Contains("?") Then
                                                'GET/API/v1/Broadcast/BTC?SignedTransactionBytes={hex string}
                                                C_Parameters.Add(CreateParameter(E_Parameter.Token, Path(4).Remove(Path(4).IndexOf("?"c))))
                                                Dim ParaString As String = Path(4).Substring(Path(4).IndexOf("?"c) + 1)
                                                C_Parameters.AddRange(ParseParameters(ParaString))

                                            End If

                                        ElseIf Path.Count > 3 Then

                                            If Path(3).Contains("?") Then
                                                'GET/API/v1/Broadcast?Token=BTC&SignedTransactionBytes={hex string}
                                                Dim ParaString As String = Path(3).Substring(Path(3).IndexOf("?"c) + 1)
                                                C_Parameters.AddRange(ParseParameters(ParaString))
                                            End If

                                        ElseIf C_RAWRequestList.Contains("{") Then
                                            'POST/API/v1/Broadcast
                                            '{
                                            '   "token": "BTC",
                                            '   "signedTransactionBytes": "{Hex string}"
                                            '}
                                            Dim JSONString As String = GetJSONFromRequest()
                                            C_Body = GetParametersFromJSON(JSONString)

                                        End If

                                    Case E_Endpoint.SmartContract

                                        If Path.Count > 3 Then

                                            'POST/API/v1/SmartContract?ID={long[,long,long]}

                                            If Path(3).Contains("?ID=") Then
                                                Dim ParaString As String = Path(3).Substring(Path(3).IndexOf("?"c) + 1)
                                                C_Parameters.AddRange(ParseParameters(ParaString))
                                            End If

                                        ElseIf C_RAWRequestList.Contains("{") Then

                                            'POST/API/v1/SmartContract
                                            '{ "id": [
                                            '   "{long}",
                                            '   "{long (OPTIONAL)}",
                                            '   "{long (OPTIONAL)}"
                                            ']}

                                            Dim JSONString As String = GetJSONFromRequest()
                                            C_Body = GetParametersFromJSON(JSONString)

                                        End If

                                    Case Else

                                        C_Method = E_Method.NONE
                                        C_Endpoint = E_Endpoint.NONE

                                End Select

                            Case E_Method.HTTP_POST

                                Select Case C_Endpoint

                                    Case E_Endpoint.Orders
                                        'must have json with senderPublicKey

                                        If Path.Count > 4 Then

                                            If Path(4).Contains("?") Then
                                                'POST/API/v1/Orders/{id}?senderPublicKey={32 bytes in Hex}

                                                C_Parameters.Add(CreateParameter(E_Parameter.ID, Path(4).Remove(Path(4).IndexOf("?"c))))
                                                Dim ParaString As String = Path(4).Substring(Path(4).IndexOf("?"c) + 1)
                                                C_Parameters.AddRange(ParseParameters(ParaString))

                                            ElseIf C_RAWRequestList.Contains("{") Then
                                                'POST/API/v1/Orders/{id} HTTP/1.1
                                                'Content-Type: Application/ json
                                                'User-Agent: PostmanRuntime/ 7.32.3
                                                'Accept: */*
                                                'Postman-Token:   6568226a-f9c0-413b-9997-3eb8f20ff595
                                                'Host:   localhost :  8130
                                                'Accept-Encoding: gzip, deflate, br
                                                'Connection: keep-alive
                                                'Content-Length:  87

                                                '{
                                                '    "senderPublicKey": "{32 bytes in Hex}"
                                                '}

                                                C_Parameters.Add(CreateParameter(E_Parameter.ID, Path(4)))
                                                Dim JSONString As String = GetJSONFromRequest()
                                                C_Body = GetParametersFromJSON(JSONString)

                                            End If

                                        ElseIf Path.Count > 3 Then

                                            If Path(3).Contains("?") Then
                                                'POST/API/v1/Orders?Type=SellOrder&AmountNQT=10000000000&CollateralNQT=3000000000&XAmountNQT=1000000000&XItem=USD

                                                Dim Endpoint As String = Path(3).Remove(Path(3).IndexOf("?"c))

                                                If Not Endpoint = E_Endpoint.Orders.ToString() Then
                                                    C_Parameters.Add(CreateParameter(E_Parameter.ID, Endpoint))
                                                End If

                                                Dim ParaString As String = Path(3).Substring(Path(3).IndexOf("?"c) + 1)
                                                C_Parameters.AddRange(ParseParameters(ParaString))

                                            ElseIf C_RAWRequestList.Contains("{") Then
                                                'check body

                                                '{
                                                '    "type": "SellOrder",
                                                '    "amountNQT": 10000000000,
                                                '    "collateralNQT": 3000000000,
                                                '    "xAmountNQT": 1000000000,
                                                '    "xItem": "USD"
                                                '}

                                                Dim JSONString As String = GetJSONFromRequest()
                                                C_Body = GetParametersFromJSON(JSONString)

                                            End If

                                        Else
                                            C_Method = E_Method.NONE
                                            C_Endpoint = E_Endpoint.NONE
                                        End If

                                    Case E_Endpoint.Bitcoin

                                        Dim JSONString As String = GetJSONFromRequest()
                                        C_Body = GetParametersFromJSON(JSONString)

                                    Case E_Endpoint.Broadcast

                                        If Path.Count > 4 Then

                                            If Path(4).Contains("?") Then
                                                'POST/API/v1/Broadcast/BTC?SignedTransactionBytes={some hex string}
                                                C_Parameters.Add(CreateParameter(E_Parameter.Token, Path(4).Remove(Path(4).IndexOf("?"c))))
                                                Dim ParaString As String = Path(4).Substring(Path(4).IndexOf("?"c) + 1)
                                                C_Parameters.AddRange(ParseParameters(ParaString))

                                            End If

                                        ElseIf Path.Count > 3 Then

                                            If Path(3).Contains("?") Then
                                                'POST/API/v1/Broadcast?Token=BTC&SignedTransactionBytes={hex string}
                                                Dim ParaString As String = Path(3).Substring(Path(3).IndexOf("?"c) + 1)
                                                C_Parameters.AddRange(ParseParameters(ParaString))
                                            End If

                                        ElseIf C_RAWRequestList.Contains("{") Then
                                            'POST/API/v1/Broadcast
                                            '{
                                            '   "token": "BTC",
                                            '   "signedTransactionBytes": "{hex string}"
                                            '}
                                            Dim JSONString As String = GetJSONFromRequest()
                                            C_Body = GetParametersFromJSON(JSONString)

                                        End If

                                    Case E_Endpoint.SmartContract

                                        If Path.Count > 3 Then
                                            'POST/API/v1/SmartContract?privateKey={32 bytes in Hex (NOT RECOMMENDED)}
                                            'POST/API/v1/SmartContract?publicKey={32 bytes in Hex}

                                            If Path(3).Contains("?") Then
                                                'create new contract
                                                Dim ParaString As String = Path(3).Substring(Path(3).IndexOf("?"c) + 1)
                                                C_Parameters.AddRange(ParseParameters(ParaString))

                                            ElseIf C_RAWRequestList.Contains("{") Then

                                                'POST/API/v1/SmartContract
                                                '{
                                                '   "privateKey": "{32 bytes in Hex (NOT RECOMMENDED)}"
                                                '}

                                                'POST/API/v1/SmartContract
                                                '{
                                                '   "publicKey": "{32 bytes in Hex}"
                                                '}

                                                Dim JSONString As String = GetJSONFromRequest()
                                                C_Body = GetParametersFromJSON(JSONString)
                                            End If

                                        End If

                                    Case Else

                                        C_Method = E_Method.NONE
                                        C_Endpoint = E_Endpoint.NONE

                                End Select

                                'Case E_Method.HTTP_PUT

                                'Case E_Method.HTTP_DELETE

                            Case Else
                                C_Method = E_Method.NONE
                                C_Endpoint = E_Endpoint.NONE
                        End Select

                    End If

                End If

            End If

#Region "old"

            'Dim Method_Path_HTTPv As List(Of String) = New List(Of String)(C_RAWRequestList(0).Split(" "c))
            'If Method_Path_HTTPv.Count > 0 Then
            '    If Method_Path_HTTPv(0).Trim().ToUpper() = "GET" Then
            '        C_Method = E_Method.HTTP_GET
            '    ElseIf Method_Path_HTTPv(0).Trim().ToUpper() = "POST" Then
            '        C_Method = E_Method.HTTP_POST
            '    End If
            'End If


            'If Method_Path_HTTPv.Count > 1 Then

            '    Dim Request As String = Method_Path_HTTPv(1).Trim()
            '    Dim SubRequest As List(Of String) = New List(Of String)

            '    If Request.Contains("/") Then
            '        SubRequest.AddRange(Request.Split("/"c))
            '    End If

            '    If SubRequest.Count > 0 Then
            '        If SubRequest(0).Trim = "" Then
            '            SubRequest.RemoveAt(0)
            '        End If
            '    End If

            '    If SubRequest.Count >= 3 Then

            '        If SubRequest(0) = C_Interface Then
            '            Dim IFace As S_PathValues = New S_PathValues
            '            IFace.Path = E_Path.Intface
            '            IFace.Value = SubRequest(0)
            '            PathValues.Add(IFace)
            '        Else
            '            Exit Sub
            '        End If


            '        If SubRequest(1) = C_Version Then
            '            Dim Version As S_PathValues = New S_PathValues
            '            Version.Path = E_Path.Version
            '            Version.Value = SubRequest(1)
            '            PathValues.Add(Version)
            '        Else
            '            Exit Sub
            '        End If


            '        Dim Command As S_PathValues = New S_PathValues
            '        Command.Path = E_Path.Command
            '        Command.Value = SubRequest(2)
            '        PathValues.Add(Command)

            '    End If

            'End If

#End Region

        End If

    End Sub

    Private Function GetParametersFromJSON(ByVal JSONString As String) As List(Of KeyValuePair(Of String, Object))

        '{
        '    "token": "BTC",
        '    "inputs": [
        '        {
        '            "type": "standard",
        '            "transaction": "8f6d4029eefc4d3e86ca4759acc5c3a02b754850a371621c053a5cae14c3c957",
        '            "unlockingScript": "76a9148562fc5e57b965f8a62deaafeee84a7fc457c1d688ac",
        '            "chainSwapKey": "a1b2c3d4e5f6",
        '            "senderAddress": "msgEkDrXVpAYCgY5vZFzRRyBddiks2G2ha"
        '        },
        '        {
        '            "type": "standard",
        '            "transaction": "8f6d4029eefc4d3e86ca4759acc5c3a02b754850a371621c053a5cae14c3c957",
        '            "unlockingScript": "76a9148562fc5e57b965f8a62deaafeee84a7fc457c1d688ac",
        '            "chainSwapKey": "a1b2c3d4e5f6",
        '            "senderAddress": "msgEkDrXVpAYCgY5vZFzRRyBddiks2G2ha"
        '        }
        '    ],
        '    "outputs": [
        '        {
        '            "type": "timeLockChainSwapHash",
        '            "changeAddress": "msgEkDrXVpAYCgY5vZFzRRyBddiks2G2ha",
        '            "recipientAddress": "n3tv3tqp7QvTiUCX5cRaEqtuAHYoGdkjJG",
        '            "chainSwapHash": "6f5e4d3c2b1a",
        '            "amountNQT": 1234
        '        }
        '    ]
        '}

        Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(JSONString, ClsJSONAndXMLConverter.E_ParseType.JSON)

        Dim XML As String = Converter.XMLString

        Dim Token As KeyValuePair(Of String, Object) = Converter.GetFromPath("result/token")

        If IsNothing(Token.Key) Then
            Return Converter.ListOfKeyValues
        ElseIf Token.Key = "BTC" Then
            Dim Inputs As KeyValuePair(Of String, Object) = Converter.GetFromPath("result/inputs")
            Dim Outputs As KeyValuePair(Of String, Object) = Converter.GetFromPath("result/outputs")

            Return New List(Of KeyValuePair(Of String, Object))({Inputs, Outputs})
        End If

        Return New List(Of KeyValuePair(Of String, Object))

    End Function

    Private Function CreateParameter(ByVal Parameter As E_Parameter, ByVal Value As String) As S_Parameter
        Return New S_Parameter With {.Parameter = Parameter, .Value = Value}
    End Function

    Private Function ParseParameters(ByVal ParametersString As String) As List(Of S_Parameter)

        Dim Parameters As List(Of E_Parameter) = New List(Of E_Parameter)([Enum].GetValues(GetType(E_Parameter)))

        If ParametersString.Contains("&"c) Then

            Dim ParameterList As List(Of String) = ParametersString.Split("&"c).ToList()

            Dim T_Parameters As List(Of S_Parameter) = New List(Of S_Parameter)
            For Each Para In ParameterList

                If Para.Contains("="c) Then

                    Dim Parameter As S_Parameter = New S_Parameter
                    Dim Exact As E_Parameter = Parameters.FirstOrDefault(Function(param) Para.Remove(Para.IndexOf("=")).ToLower() = param.ToString().ToLower())

                    If Exact = E_Parameter.NONE Then
                        Parameter.Parameter = Parameters.FirstOrDefault(Function(param) Para.ToLower().Contains(param.ToString().ToLower()))
                    Else
                        Parameter.Parameter = Exact
                    End If

                    If Not Parameter.Parameter = E_Parameter.NONE Then
                        Parameter.Value = Para.Substring(Para.IndexOf("="c) + 1)
                        T_Parameters.Add(Parameter)
                    End If

                End If

            Next

            Return T_Parameters

        ElseIf ParametersString.Contains("="c) Then

            Dim Parameter As S_Parameter = New S_Parameter
            Dim Exact As E_Parameter = Parameters.FirstOrDefault(Function(param) ParametersString.Remove(ParametersString.IndexOf("=")).ToLower() = param.ToString().ToLower())

            If Exact = E_Parameter.NONE Then
                Parameter.Parameter = Parameters.FirstOrDefault(Function(param) ParametersString.ToLower().Contains(param.ToString().ToLower()))
            Else
                Parameter.Parameter = Exact
            End If

            If Not Parameter.Parameter = E_Parameter.NONE Then
                Parameter.Value = ParametersString.Substring(ParametersString.IndexOf("="c) + 1)
                Return New List(Of S_Parameter)({Parameter})
            End If

        End If

        Return New List(Of S_Parameter)

    End Function

    'Private Function GetJSONFromRequest(ByVal Request As String) As Object
    '    Return GetJSONFromRequest(Request.Split(Convert.ToChar(vbCr)).ToList())
    'End Function

    Private Function GetJSONFromRequest(Optional ByVal Request As List(Of String) = Nothing) As Object

        If Request Is Nothing Then
            Request = C_RAWRequestList
        End If

        Dim Idx As Integer = Request.FindIndex(Function(c) c = "{")
        Return String.Join("", Request.Skip(Idx)).Replace(" ", "")

    End Function

    'Private Sub GetCommand()

    '    If PathValues.Count > 0 Then

    '        Dim Command As S_PathValues = PathValues.FirstOrDefault(Function(c) c.Path = E_Path.Command)

    '        If Command.Value.Contains("?") Then

    '            Dim T_Command As String = Command.Value.Remove(Command.Value.IndexOf("?"))
    '            If [Enum].GetNames(GetType(E_Endpoint)).ToList().Contains(T_Command) Then
    '                C_Endpoint = [Enum].Parse(GetType(E_Endpoint), T_Command)
    '            End If

    '            Dim T_Parameters As String = Command.Value.Substring(Command.Value.IndexOf("?") + 1)

    '            Dim QueryParameters As List(Of String) = New List(Of String)
    '            QueryParameters.AddRange(T_Parameters.Split("&"c).ToArray())

    '            Dim ParameterNames As List(Of E_Parameter) = New List(Of E_Parameter)([Enum].GetValues(GetType(E_Parameter)))

    '            For Each T_QueryParameter As String In QueryParameters
    '                For Each T_Parameter As E_Parameter In ParameterNames
    '                    If T_QueryParameter.Contains(T_Parameter.ToString()) Then
    '                        Dim Parameter As S_Parameter = New S_Parameter
    '                        Parameter.Parameter = T_Parameter

    '                        Select Case Parameter.Parameter
    '                            Case E_Parameter.BitcoinOutputType
    '                                Dim T_BTCTypes As List(Of E_BitcoinType) = New List(Of E_BitcoinType)([Enum].GetValues(GetType(E_BitcoinType)))
    '                                Dim T_Value As String = T_QueryParameter.Substring(T_QueryParameter.IndexOf("=") + 1)

    '                                Parameter.Value = T_BTCTypes.FirstOrDefault(Function(btc) btc.ToString() = T_Value).ToString()

    '                            Case Else
    '                                Parameter.Value = T_QueryParameter.Substring(T_QueryParameter.IndexOf("=") + 1)
    '                        End Select

    '                        C_Parameters.Add(Parameter)
    '                    End If
    '                Next
    '            Next
    '        Else

    '            If [Enum].GetNames(GetType(E_Endpoint)).ToList().Contains(Command.Value) Then
    '                C_Endpoint = [Enum].Parse(GetType(E_Endpoint), Command.Value)
    '            End If

    '        End If

    '    End If

    'End Sub

End Class
