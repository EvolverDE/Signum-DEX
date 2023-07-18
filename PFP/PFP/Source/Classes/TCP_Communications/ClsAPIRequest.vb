﻿
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

        Transaction = 5

        Bitcoin = 6

    End Enum

    Public Enum E_Parameter

        NONE = 0

        Status = 1
        OrderID = 2

        Pair = 3
        Days = 4
        Tickmin = 5

        SenderPublicKey = 6

        Token = 7

        Inputs = 8
        Outputs = 9

        Transaction = 10
        Type = 11
        Script = 12

        Sender = 13
        Change = 14
        Recipient = 15

        PrivateKey = 16
        PublicKey = 17

        ChainSwapKey = 18
        ChainSwapHash = 19

        AmountNQT = 20

    End Enum

    'http://localhost:8130/API/v1/Bitcoin/Transaction/8f6d4029eefc4d3e86ca4759acc5c3a02b754850a371621c053a5cae14c3c957?Type=TimeLockChainSwapHash&SenderAddress=msgEkDrXVpAYCgY5vZFzRRyBddiks2G2ha&RecipientAddress=msgEkDrXVpAYCgY5vZFzRRyBddiks2G2ha&ChainSwapHash=abcdef&AmountNQT=2120

    'Public Enum E_BitcoinType
    '    NONE = 0
    '    Standard = 1
    '    TimeLock = 2
    '    ChainSwapHash = 3
    '    TimeLockChainSwapHash = 4
    'End Enum

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
                                            OrderID.Parameter = E_Parameter.OrderID

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
                                                'POST/API/v1/Orders/{id}?senderPublicKey=6FBE5B0C2A6BA72612702795B2E250616C367BD8B28F965A36CD59DD13D09A51

                                                C_Parameters.Add(CreateParameter(E_Parameter.OrderID, Path(4).Remove(Path(4).IndexOf("?"c))))

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
                                                '    "senderPublicKey": "6FBE5B0C2A6BA72612702795B2E250616C367BD8B28F965A36CD59DD13D09A51"
                                                '}

                                                C_Parameters.Add(CreateParameter(E_Parameter.OrderID, Path(4)))

                                                Dim JSONString As String = GetJSONFromRequest()
                                                'Dim JSON As ClsJSON = New ClsJSON()
                                                'Dim XML As String = JSON.JSONToXML(JSONString)
                                                'Dim SenderPubKey As String = GetStringBetween(XML, "<" + E_Parameter.SenderPublicKey.ToString().ToLower() + ">", "</" + E_Parameter.SenderPublicKey.ToString().ToLower() + ">", False, False)

                                                'C_Parameters.Add(CreateParameter(E_Parameter.SenderPublicKey, SenderPubKey))

                                                C_Parameters.AddRange(GetParametersFromJSON(JSONString))

                                            Else
                                                C_Method = E_Method.NONE
                                                C_Endpoint = E_Endpoint.NONE
                                            End If

                                        Else
                                            C_Method = E_Method.NONE
                                            C_Endpoint = E_Endpoint.NONE
                                        End If

                                    Case E_Endpoint.Transaction

                                        'POST/API/v1/Transaction HTTP/1.1
                                        'Content-Type: Application/ json
                                        'User-Agent: PostmanRuntime/ 7.32.3
                                        'Accept: */*
                                        'Postman-Token: bf140a20-8124 - 4354 - 9Dcd-33be63c4f2e1
                                        'Host:                                   localhost :  8130
                                        'Accept-Encoding: gzip, deflate, br
                                        'Connection:                             keep-alive
                                        'Content-Length:   509

                                        '{
                                        '    "token": "BTC",
                                        '    "inputs": [
                                        '        {
                                        '            "type": "standard",
                                        '            "transaction": "8f6d4029eefc4d3e86ca4759acc5c3a02b754850a371621c053a5cae14c3c957",
                                        '            "unlockingScript": "76a9148562fc5e57b965f8a62deaafeee84a7fc457c1d688ac", //optional: only if type is pay2ScriptHash (of timeLockChainSwapHash or chainSwapHash)
                                        '            "chainSwapKey":"a1b2c3d4e5f6", //optional: only if type is timeLockChainSwapHash or chainSwapHash
                                        '            "senderAddress": "msgEkDrXVpAYCgY5vZFzRRyBddiks2G2ha"
                                        '        }
                                        '    ],
                                        '    "outputs": [
                                        '        {
                                        '            "type": "timeLockChainSwapHash",
                                        '            "changeAddress": "msgEkDrXVpAYCgY5vZFzRRyBddiks2G2ha", //optional: default=first input senderAddress
                                        '            "recipientAddress": "n3tv3tqp7QvTiUCX5cRaEqtuAHYoGdkjJG",
                                        '            "chainSwapHash": "6f5e4d3c2b1a", //optional: only if type is pay2ScriptHash (timeLockChainSwapHash or chainSwapHash)
                                        '            "amountNQT": 1234
                                        '        }
                                        '    ]
                                        '}

                                        Dim JSONString As String = GetJSONFromRequest()
                                        C_Body = GetParametersFromJSON(JSONString)

                                    Case E_Endpoint.Bitcoin

                                        Dim JSONString As String = GetJSONFromRequest()
                                        C_Body = GetParametersFromJSON(JSONString)

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

        'Dim JSON As ClsJSON = New ClsJSON()
        'Dim o = JSON.JSONRecursive2(JSONString)
        'Dim p = Between2List(o(0), "[", "]")

        Dim XML As String = Converter.XMLString  ' JSON.JSONToXML(JSONString)

        Dim Inputs As KeyValuePair(Of String, Object) = Converter.GetFromPath("result/inputs") '.Search(Of List(Of KeyValuePair(Of String, Object)))("inputs")
        Dim Outputs As KeyValuePair(Of String, Object) = Converter.GetFromPath("result/outputs") '.Search(Of List(Of KeyValuePair(Of String, Object)))("outputs")

        'Dim FirstInput As KeyValuePair(Of String, Object) = Inputs.FirstOrDefault()
        'If FirstInput.Key <> "inputs" Then
        '    FirstInput = New KeyValuePair(Of String, Object)("inputs", New List(Of KeyValuePair(Of String, Object))({New KeyValuePair(Of String, Object)("0", Outputs)}))
        'End If

        'Dim FirstOutput As KeyValuePair(Of String, Object) = Outputs.FirstOrDefault()
        'If FirstOutput.Key <> "outputs" Then
        '    FirstOutput = New KeyValuePair(Of String, Object)("outputs", New List(Of KeyValuePair(Of String, Object))({New KeyValuePair(Of String, Object)("0", Outputs)}))
        'End If

        Return New List(Of KeyValuePair(Of String, Object))({Inputs, Outputs})

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
                    Parameter.Parameter = Parameters.FirstOrDefault(Function(param) Para.ToLower().Contains(param.ToString().ToLower()))
                    Parameter.Value = Para.Substring(Para.IndexOf("="c) + 1)

                    T_Parameters.Add(Parameter)

                End If

            Next

            Return T_Parameters

        ElseIf ParametersString.Contains("="c) Then

            Dim Parameter As S_Parameter = New S_Parameter
            Parameter.Parameter = Parameters.FirstOrDefault(Function(param) ParametersString.ToLower().Contains(param.ToString().ToLower()))
            Parameter.Value = ParametersString.Substring(ParametersString.IndexOf("="c) + 1)

            Return New List(Of S_Parameter)({Parameter})

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