
Option Strict On
Option Explicit On

'http://127.0.0.1:8130/API/v1.0/GetInfo

Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Windows

Public Class ClsTCPAPI

    Public Property API_ServerPort() As Integer = 8130
    Property T_TCPServer As TcpListener

    Public Property AlreadyStarted As Boolean = False

    Structure S_Connection
        Dim RequestThread As Thread
        Dim TCPClient As TcpClient
        Dim IP As String
        Dim Port As Integer
    End Structure

    Property ConnectionList As List(Of S_Connection) = New List(Of S_Connection)
    Property StopServer As Boolean = True
    Property TCPTimer As System.Windows.Forms.Timer
    Public Property ResponseMSGList As List(Of API_Response) = New List(Of API_Response)

    Private Property PrivateSessionKey As String = ""
    Private Property PublicSessionKey As String = ""

    Public Structure API_Response
        Dim API_Interface As String
        Dim API_Version As String
        Dim API_Command As String
        Dim API_Parameters As List(Of String)

        Dim API_Response As String

    End Structure

    Public Property StatusMSG As List(Of String) = New List(Of String)
    Public Property API_ShowStatusMSG As Boolean = True

    Sub New(Optional ByVal ServerPort As Integer = 8130, Optional ByVal ShowStatusMsg As Boolean = True)

        API_ServerPort = ServerPort
        API_ShowStatusMSG = ShowStatusMsg

        Dim RandomBytesAsPhrase As Byte() = GetRandomBytesWithTimeEntropy(32)
        Dim SessionMasterKeys = Keygen(RandomBytesAsPhrase)
        PrivateSessionKey = ByteArrayToHEXString(SessionMasterKeys(1))
        PublicSessionKey = ByteArrayToHEXString(SessionMasterKeys(0))

        TCPTimer = New Windows.Forms.Timer
        TCPTimer.Interval = 250
        AddHandler TCPTimer.Tick, AddressOf TCPTimer_Tick
        TCPTimer.Enabled = True

    End Sub

    Public Sub StartAPIServer()
        StopServer = False
        Dim serverThread As New Thread(New ThreadStart(AddressOf TCPServer))
        serverThread.Start()
        AlreadyStarted = True
    End Sub
    Private Sub TCPServer()

        T_TCPServer = New TcpListener(IPAddress.Any, API_ServerPort)
        If API_ShowStatusMSG Then
            StatusMSG.Add("API server started at: " + IPAddress.Any.ToString() + ":" & API_ServerPort.ToString)
        End If


        While Not StopServer

            Try

                T_TCPServer.Start()

                Dim TCPClient As TcpClient = T_TCPServer.AcceptTcpClient()

                Dim T_Connection As S_Connection = New S_Connection

                Dim T_CEP As IPEndPoint = DirectCast(TCPClient.Client.RemoteEndPoint, Net.IPEndPoint)

                Dim IPPort() As String = T_CEP.ToString.Split(":"c)

                T_Connection.IP = IPPort(0)
                T_Connection.Port = Integer.Parse(IPPort(1))

                If API_ShowStatusMSG Then
                    StatusMSG.Add("Client: " + T_Connection.IP + ":" + T_Connection.Port.ToString)
                End If


                T_Connection.TCPClient = TCPClient
                T_Connection.RequestThread = Nothing

                ConnectionList.Add(T_Connection)

            Catch ex As Exception

                If API_ShowStatusMSG Then
                    StatusMSG.Add("TCPServer(): " + ex.Message)
                End If

            End Try

        End While

    End Sub

    Public Function StopAPIServer() As Boolean

        Try

            StopServer = True
            If Not T_TCPServer Is Nothing Then
                T_TCPServer.Stop()
            End If

            AlreadyStarted = False

            Dim ThreadAlive As Boolean = True

            While ThreadAlive
                ThreadAlive = False

                For i As Integer = 0 To ConnectionList.Count - 1

                    Dim TCPClient As TcpClient = ConnectionList(i).TCPClient
                    TCPClient.Close()

                    Dim TCPThread As Thread = ConnectionList(i).RequestThread
                    TCPThread.Abort()

                    If TCPThread.IsAlive Then
                        ThreadAlive = True
                    End If

                Next

            End While


        Catch ex As Exception
            If API_ShowStatusMSG Then
                StatusMSG.Add("StopAPIServer()" + ex.Message)
            End If
        End Try

        Return True

    End Function

    Private Sub TCPTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        ClearList()
        ProcessRequest()
    End Sub

    Private Sub ClearList()

        Try

            Dim DeadFound As Boolean = True

            While DeadFound

                DeadFound = False

                For Each SCon2 As S_Connection In ConnectionList

                    If SCon2.TCPClient.Client Is Nothing Then

                        DeadFound = True
                        SCon2.TCPClient.Close()
                        ConnectionList.Remove(SCon2)

                        If API_ShowStatusMSG Then
                            StatusMSG.Add(Now.ToShortDateString + " " + Now.ToLongTimeString + " " + SCon2.IP + " " + SCon2.Port.ToString + " disconnected.")
                        End If

                        Exit For

                    Else

                        If Not SCon2.TCPClient.Client.Connected Then
                            DeadFound = True
                            SCon2.TCPClient.Close()
                            ConnectionList.Remove(SCon2)

                            If API_ShowStatusMSG Then
                                StatusMSG.Add(Now.ToShortDateString + " " + Now.ToLongTimeString + " " + SCon2.IP + " " + SCon2.Port.ToString + " disconnected.")
                            End If

                            Exit For

                        End If

                    End If

                Next


            End While

        Catch ex As Exception
            If API_ShowStatusMSG Then
                StatusMSG.Add("ClearList(): " + ex.Message)
            End If

        End Try

    End Sub

    Private Sub ProcessRequest()

        For ConnectionIDX As Integer = 0 To ConnectionList.Count - 1

            Dim TCPClient As TcpClient = ConnectionList(ConnectionIDX).TCPClient

            Try

                TCPClient.SendTimeout = 1000
                TCPClient.ReceiveTimeout = 1000

                Dim recvBytes(CInt(Integer.MaxValue * 0.5)) As Byte
                Dim htmlReq As String = Nothing
                Dim bytes As Integer = 0

                bytes = TCPClient.Client.Receive(recvBytes, 0, TCPClient.Client.Available, SocketFlags.None)
                htmlReq = Encoding.ASCII.GetString(recvBytes, 0, bytes)

                htmlReq = htmlReq.Replace(vbLf, "")

                Dim APIRequest As ClsAPIRequest = New ClsAPIRequest(htmlReq)

                If APIRequest.Endpoint <> ClsAPIRequest.E_Endpoint.NONE Then

                    Dim ResponseHTML As String = "{""response"":""no data.""}"

                    If APIRequest.Method = ClsAPIRequest.E_Method.HTTP_GET Then

#Region "GET request template"

                        ''		(0)	    "GET /API/v1.0/GetCandles?pair=USD_SIGNA&days=3&tickmin=15 HTTP/1.1"	String
                        ''		(1)	    "Host: localhost:8130"	String
                        ''		(2)	    "Connection: keep-alive"	String
                        ''		(3) 	"Cache-Control: max-age=0"	String
                        ''		(4)	    "sec-ch-ua: ""Google Chrome"";v=""89"", ""Chromium"";v=""89"", "";Not A Brand"";v=""99"""	String
                        ''		(5)	    "sec-ch-ua-mobile: ?0"	String
                        ''		(6)	    "Upgrade-Insecure-Requests: 1"	String
                        ''		(7)	    "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.128 Safari/537.36"	String
                        ''		(8)	    "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"	String
                        ''		(9)	    "Sec-Fetch-Site: none"	String
                        ''		(10)	"Sec-Fetch-Mode: navigate"	String
                        ''		(11)	"Sec-Fetch-User: ?1"	String
                        ''		(12)	"Sec-Fetch-Dest: document"	String
                        ''		(13)	"Accept-Encoding: gzip, deflate, br"	String
                        ''		(14)	"Accept-Language: de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7"	String
                        ''		(15)	""	String
                        ''		(16)	""	String
#End Region
                        Dim FoundStaticResponse As Boolean = False
                        For Each Response As API_Response In ResponseMSGList
                            If Response.API_Interface = APIRequest.C_Interface And Response.API_Version = APIRequest.C_Version And Response.API_Command = APIRequest.Endpoint.ToString() Then
                                FoundStaticResponse = True

                                For Each para In APIRequest.Parameters

                                    Dim paraStr = para.Parameter.ToString().ToLower() + "=" + para.Value.ToString()

                                    If Not Response.API_Parameters.Contains(paraStr) Then
                                        FoundStaticResponse = False
                                        Exit For
                                    End If
                                Next

                                If FoundStaticResponse Then
                                    ResponseHTML = Response.API_Response
                                    Exit For
                                End If

                            End If
                        Next

                        If Not FoundStaticResponse Then

                            Select Case APIRequest.Endpoint
                                Case ClsAPIRequest.E_Endpoint.Bitcoin

                                    Dim Inputs As List(Of String) = New List(Of String)
                                    Dim SenderAddresses As List(Of ClsTransaction.S_Address) = New List(Of ClsTransaction.S_Address)
                                    Dim RecipientAddress As String = ""
                                    Dim ChainSwapHash As String = ""
                                    Dim Amount As Double = 0.0

                                    For Each Parameter As ClsAPIRequest.S_Parameter In APIRequest.Parameters

                                        Select Case Parameter.Parameter
                                            Case ClsAPIRequest.E_Parameter.Inputs

                                                If Parameter.Value.Contains(",") Then
                                                    Inputs.AddRange(Parameter.Value.Split(","c))
                                                Else
                                                    Inputs.Add(Parameter.Value)
                                                End If

                                            Case ClsAPIRequest.E_Parameter.Sender

                                                If Parameter.Value.Contains(",") Then

                                                    Dim T_Addresses As List(Of String) = New List(Of String)(Parameter.Value.Split(","c))

                                                    For Each T_Address As String In T_Addresses
                                                        SenderAddresses.Add(New ClsTransaction.S_Address(T_Address))
                                                    Next

                                                Else
                                                    SenderAddresses.Add(New ClsTransaction.S_Address(Parameter.Value))
                                                End If

                                            Case ClsAPIRequest.E_Parameter.Recipient
                                                RecipientAddress = Parameter.Value

                                            Case ClsAPIRequest.E_Parameter.ChainSwapHash
                                                ChainSwapHash = Parameter.Value

                                            Case ClsAPIRequest.E_Parameter.AmountNQT

                                                If IsNumeric(Parameter.Value) Then
                                                    Amount = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(Parameter.Value))
                                                End If

                                        End Select

                                    Next

                                    Dim BitcoinTransaction As ClsTransaction = New ClsTransaction(Inputs, SenderAddresses, "")
                                    BitcoinTransaction.CreateOutput(RecipientAddress, ChainSwapHash, SenderAddresses(0).Address, Amount)
                                    BitcoinTransaction.FinalizingOutputs()

                                    Dim JSONExport As String = "{""inputs"":["

                                    For Each Inpu As ClsUnspentOutput In BitcoinTransaction.Inputs
                                        JSONExport += "{"
                                        JSONExport += """transactionId"":""" + Inpu.TransactionID + ""","
                                        JSONExport += """index"":" + Inpu.InputIndex.ToString() + ","
                                        JSONExport += """addresses"":""" + Inpu.GetAddressesString + ""","
                                        JSONExport += """type"":""" + Inpu.OutputType.ToString() + ""","
                                        JSONExport += """script"":""" + Inpu.GetScriptString + ""","
                                        JSONExport += """scriptHex"":""" + Inpu.ScriptHex + ""","
                                        JSONExport += """scriptHash"":""" + Inpu.ScriptHash + ""","
                                        JSONExport += """amount"":" + String.Format("{0:#0.00000000}", ClsSignumAPI.Planck2Dbl(Inpu.AmountNQT)).Replace(",", ".") + ","
                                        JSONExport += """spendable"":" + Inpu.Spendable.ToString().ToLower() + ","
                                        JSONExport += """unsignedInput"":""" + Inpu.UnsignedTransactionHex + """"
                                        JSONExport += "},"
                                    Next

                                    JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                    JSONExport += "],""outputs"":["

                                    For i As Integer = 0 To BitcoinTransaction.Outputs.Count - 1

                                        Dim Outpu As ClsOutput = BitcoinTransaction.Outputs(i)
                                        JSONExport += "{"
                                        JSONExport += """index"":" + i.ToString() + ","
                                        JSONExport += """addresses"":""" + Outpu.GetAddressesString + ""","
                                        JSONExport += """type"":""" + Outpu.OutputType.ToString() + ""","
                                        JSONExport += """script"":""" + Outpu.GetScriptString + ""","
                                        JSONExport += """scriptHex"":""" + Outpu.ScriptHex + ""","
                                        JSONExport += """scriptHash"":""" + Outpu.ScriptHash + ""","
                                        JSONExport += """amount"":" + String.Format("{0:#0.00000000}", ClsSignumAPI.Planck2Dbl(Outpu.AmountNQT)).Replace(",", ".") + ","
                                        JSONExport += "},"
                                    Next

                                    JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                    JSONExport += "]}"

                                    ResponseHTML = JSONExport

                                Case ClsAPIRequest.E_Endpoint.Transaction

                            End Select

                        End If

                    ElseIf APIRequest.Method = ClsAPIRequest.E_Method.HTTP_POST Then

#Region "POST request template"

                        '### Chrome ###
                        '		(0)	"POST /GetCandles HTTP/1.1"	String
                        '		(1)	"Host: localhost:8130"	String
                        '		(2)	"Connection: keep-alive"	String
                        '		(3)	"Content-Length: 30"	String
                        '		(4)	"Cache-Control: max-age=0"	String
                        '		(5)	"sec-ch-ua: "" Not A;Brand"";v=""99"", ""Chromium"";v=""90"", ""Google Chrome"";v=""90"""	String
                        '		(6)	"sec-ch-ua-mobile: ?0"	String
                        '		(7)	"Upgrade-Insecure-Requests: 1"	String
                        '		(8)	"Origin: http://localhost:8130"	String
                        '		(9)	"Content-Type: application/x-www-form-urlencoded"	String
                        '		(10)	"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36"	String
                        '		(11)	"Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"	String
                        '		(12)	"Sec-Fetch-Site: same-origin"	String
                        '		(13)	"Sec-Fetch-Mode: navigate"	String
                        '		(14)	"Sec-Fetch-User: ?1"	String
                        '		(15)	"Sec-Fetch-Dest: document"	String
                        '		(16)	"Referer: http://localhost:8130/"	String
                        '		(17)	"Accept-Encoding: gzip, deflate, br"	String
                        '		(18)	"Accept-Language: de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7"	String
                        '		(19)	""	String
                        '		(20)	"APIUser=apimann&APIKey=apipass"	String

#End Region

                        Select Case APIRequest.Endpoint

                            Case ClsAPIRequest.E_Endpoint.Orders

                                If APIRequest.Parameters.Count > 0 Then
                                    'use Parameters

                                    Dim PublicKey As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.SenderPublicKey).Value

                                    If IsNothing(PublicKey) Then PublicKey = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.PublicKey).Value

                                    Dim OrderID As String = APIRequest.Parameters.FirstOrDefault(Function(OID) OID.Parameter = ClsAPIRequest.E_Parameter.OrderID).Value

                                    If Not IsNumeric(OrderID) Then
                                        OrderID = ClsReedSolomon.Decode(OrderID).ToString()
                                    End If

                                    If Not IsNothing(PublicKey) And Not IsNothing(OrderID) Then

                                        If Not OrderID Is Nothing And MessageIsHEXString(PublicKey) And PublicKey.Length = 64 Then
                                            For Each T_DEXContract As ClsDEXContract In PFPForm.DEXContractList
                                                If T_DEXContract.ID.ToString() = OrderID Then
                                                    'address = TS-4FCL-YHVW-R94Z-F4D7J ; id = 15570460086676567378
                                                    'publickey = 6FBE5B0C2A6BA726 12702795B2E25061 6C367BD8B28F965A 36CD59DD13D09A51
                                                    'http://127.0.0.1:8130/API/v1.0/AcceptOrder?DEXContractAddress=TS-4FCL-YHVW-R94Z-F4D7J&PublicKey=6FBE5B0C2A6BA72612702795B2E250616C367BD8B28F965A36CD59DD13D09A51

                                                    If T_DEXContract.Status = ClsDEXContract.E_Status.OPEN Then 'T_DEXContract.IsSellOrder And
                                                        Dim T_UnsignedTransactionBytes As String = T_DEXContract.AcceptSellOrder(PublicKey)

                                                        If T_UnsignedTransactionBytes.Contains("errorCode") Then
                                                            ResponseHTML = T_UnsignedTransactionBytes.Substring(T_UnsignedTransactionBytes.IndexOf("->") + 2).Trim
                                                        ElseIf T_UnsignedTransactionBytes.Contains(Application.ProductName + "-error") Then

                                                        Else
                                                            '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                            ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""AcceptOrder"",""data"":{""unsignedTransactionBytes"":""" + T_UnsignedTransactionBytes + """}}"
                                                        End If

                                                    End If

                                                End If
                                            Next

                                        End If

                                    End If

                                Else
                                    'check Body



                                End If

                            Case ClsAPIRequest.E_Endpoint.Bitcoin

                                If APIRequest.Parameters.Count > 0 Then
                                    'use Parameters

                                Else
                                    'check Body

                                    If APIRequest.Body.Count > 0 Then

                                        Dim InputIDs As List(Of String) = New List(Of String)
                                        Dim PrivateKeys As List(Of ClsTransaction.S_PrivateKey) = New List(Of ClsTransaction.S_PrivateKey)
                                        Dim SenderAddresses As List(Of ClsTransaction.S_Address) = New List(Of ClsTransaction.S_Address)
                                        Dim Scripts As List(Of String) = New List(Of String)

                                        Dim OutputIDs As List(Of ClsOutput) = New List(Of ClsOutput)

                                        For Each BodyEntry As KeyValuePair(Of String, Object) In APIRequest.Body

                                            Select Case BodyEntry.Key

                                                Case "inputs"

                                                    Dim Inputs As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))

                                                    If BodyEntry.Value.GetType = GetType(List(Of KeyValuePair(Of String, Object))) Then
                                                        Inputs = DirectCast(BodyEntry.Value, List(Of KeyValuePair(Of String, Object)))
                                                    Else
                                                        Dim InputObjects As List(Of Object) = DirectCast(BodyEntry.Value, List(Of Object))
                                                        Dim InputKeyValue As KeyValuePair(Of String, Object) = DirectCast(InputObjects(0), KeyValuePair(Of String, Object))
                                                        Inputs = New List(Of KeyValuePair(Of String, Object))({InputKeyValue})
                                                    End If

                                                    For Each InputEntry As KeyValuePair(Of String, Object) In Inputs

                                                        Dim Parameters As List(Of KeyValuePair(Of String, Object)) = DirectCast(InputEntry.Value, List(Of KeyValuePair(Of String, Object)))
                                                        Dim T_PKCSK As ClsTransaction.S_PrivateKey = New ClsTransaction.S_PrivateKey(,)
                                                        Dim T_Address As ClsTransaction.S_Address = New ClsTransaction.S_Address(,)
                                                        Dim T_InputID As String = ""
                                                        Dim T_Script As String = ""

                                                        For i As Integer = 0 To Parameters.Count - 1

                                                            Dim Parameter As KeyValuePair(Of String, Object) = Parameters(i)

                                                            'Dim T_Parameter As KeyValuePair(Of String, Object) = DirectCast(Parameter, KeyValuePair(Of String, Object))

                                                            Select Case Parameter.Key
                                                                Case "transaction"
                                                                    T_InputID = Parameter.Value.ToString()
                                                                Case "index"
                                                                    i = i
                                                                Case "script"
                                                                    T_Script = Parameter.Value.ToString()
                                                                Case "privateKey"
                                                                    T_PKCSK.PrivateKey = Parameter.Value.ToString()
                                                                Case "address"
                                                                    T_Address.Address = Parameter.Value.ToString()
                                                                Case "chainSwapKey"
                                                                    T_PKCSK.ChainSwapKey = Parameter.Value.ToString()
                                                                    T_Address.ChainSwapKey = True
                                                            End Select

                                                        Next

                                                        InputIDs.Add(T_InputID)
                                                        If T_PKCSK.PrivateKey.Trim() <> "" Then PrivateKeys.Add(T_PKCSK)
                                                        If T_Address.Address.Trim() <> "" Then SenderAddresses.Add(T_Address)
                                                        Scripts.Add(T_Script)

                                                    Next

                                                Case "outputs"

                                                    Dim Outputs As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))

                                                    If BodyEntry.Value.GetType = GetType(List(Of KeyValuePair(Of String, Object))) Then
                                                        Outputs = DirectCast(BodyEntry.Value, List(Of KeyValuePair(Of String, Object)))
                                                    Else
                                                        Dim OutputObjects As List(Of Object) = DirectCast(BodyEntry.Value, List(Of Object))
                                                        Dim OutputKeyValue As KeyValuePair(Of String, Object) = DirectCast(OutputObjects(0), KeyValuePair(Of String, Object))
                                                        Outputs = New List(Of KeyValuePair(Of String, Object))({OutputKeyValue})
                                                    End If

                                                    For Each OutputEntry As KeyValuePair(Of String, Object) In Outputs

                                                        Dim Parameters As List(Of KeyValuePair(Of String, Object)) = DirectCast(OutputEntry.Value, List(Of KeyValuePair(Of String, Object)))

                                                        Dim Type As String = ""
                                                        Dim Recipient As String = ""
                                                        Dim Change As String = ""
                                                        Dim chainSwapHash As String = ""
                                                        Dim Amount As Double = 0.0

                                                        For i As Integer = 0 To Parameters.Count - 1

                                                            Dim Parameter As KeyValuePair(Of String, Object) = Parameters(i)

                                                            Select Case Parameter.Key
                                                                Case "type"
                                                                    Type = Parameter.Value.ToString()
                                                                Case "recipient"
                                                                    Recipient = Parameter.Value.ToString()
                                                                Case "change"
                                                                    Change = Parameter.Value.ToString()
                                                                Case "chainSwapHash"
                                                                    chainSwapHash = Parameter.Value.ToString()
                                                                Case "amount"
                                                                    Amount = Convert.ToDouble(Parameter.Value)
                                                            End Select

                                                        Next

                                                        OutputIDs.Add(New ClsOutput(Recipient, chainSwapHash, Change, Amount))

                                                    Next

                                            End Select
                                        Next

                                        If SenderAddresses.Count = 0 And PrivateKeys.Count > 0 Then
                                            For Each T_PrivateKey As ClsTransaction.S_PrivateKey In PrivateKeys
                                                Dim T_Address As ClsTransaction.S_Address = New ClsTransaction.S_Address()
                                                T_Address.Address = (PrivKeyToPubKey(T_PrivateKey.PrivateKey))
                                                T_Address.ChainSwapKey = If(T_PrivateKey.ChainSwapKey.Trim() = "", False, True)
                                                SenderAddresses.Add(T_Address)
                                            Next
                                        End If

                                        If SenderAddresses.Count > 0 And PrivateKeys.Count = 0 Then

                                            Dim BitcoinTransaction As ClsTransaction = New ClsTransaction(InputIDs, SenderAddresses, Scripts)

                                            For Each OP As ClsOutput In OutputIDs
                                                BitcoinTransaction.CreateOutput(OP)
                                            Next

                                            BitcoinTransaction.FinalizingOutputs()

                                            Dim JSONExport As String = "{""inputs"":["

                                            For Each Inpu As ClsUnspentOutput In BitcoinTransaction.Inputs
                                                JSONExport += "{"
                                                JSONExport += """transactionId"":""" + Inpu.TransactionID + ""","
                                                JSONExport += """index"":" + Inpu.InputIndex.ToString() + ","
                                                JSONExport += """addresses"":""" + Inpu.GetAddressesString + ""","
                                                JSONExport += """type"":""" + Inpu.OutputType.ToString() + ""","
                                                JSONExport += """script"":""" + Inpu.GetScriptString + ""","
                                                JSONExport += """scriptHex"":""" + Inpu.ScriptHex + ""","
                                                JSONExport += """scriptHash"":""" + Inpu.ScriptHash + ""","
                                                JSONExport += """amount"":" + String.Format("{0:#0.00000000}", ClsSignumAPI.Planck2Dbl(Inpu.AmountNQT)).Replace(",", ".") + ","
                                                JSONExport += """spendable"":" + Inpu.Spendable.ToString().ToLower() + ","
                                                JSONExport += """unsignedInput"":""" + Inpu.UnsignedTransactionHex + """"
                                                JSONExport += "},"
                                            Next

                                            JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                            JSONExport += "],""outputs"":["

                                            For i As Integer = 0 To BitcoinTransaction.Outputs.Count - 1

                                                Dim Outpu As ClsOutput = BitcoinTransaction.Outputs(i)
                                                JSONExport += "{"
                                                JSONExport += """index"":" + i.ToString() + ","
                                                JSONExport += """addresses"":""" + Outpu.GetAddressesString + ""","
                                                JSONExport += """type"":""" + Outpu.OutputType.ToString() + ""","
                                                JSONExport += """script"":""" + Outpu.GetScriptString + ""","
                                                JSONExport += """scriptHex"":""" + Outpu.ScriptHex + ""","
                                                JSONExport += """scriptHash"":""" + Outpu.ScriptHash + ""","
                                                JSONExport += """amount"":" + String.Format("{0:#0.00000000}", ClsSignumAPI.Planck2Dbl(Outpu.AmountNQT)).Replace(",", ".") + ","
                                                JSONExport += "},"
                                            Next

                                            JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                            JSONExport += "]}"

                                            ResponseHTML = JSONExport

                                        End If

                                        If SenderAddresses.Count > 0 And PrivateKeys.Count > 0 Then

                                            Dim BitcoinTransaction As ClsTransaction = New ClsTransaction(InputIDs, SenderAddresses, Scripts)

                                            For Each OP As ClsOutput In OutputIDs
                                                BitcoinTransaction.CreateOutput(OP)
                                            Next

                                            BitcoinTransaction.FinalizingOutputs()
                                            BitcoinTransaction.SignTransaction(PrivateKeys)

                                            Dim JSONExport As String = "{""inputs"":["

                                            For Each Inpu As ClsUnspentOutput In BitcoinTransaction.Inputs
                                                JSONExport += "{"
                                                JSONExport += """transactionId"":""" + Inpu.TransactionID + ""","
                                                JSONExport += """index"":" + Inpu.InputIndex.ToString() + ","
                                                JSONExport += """addresses"":""" + Inpu.GetAddressesString + ""","
                                                JSONExport += """type"":""" + Inpu.OutputType.ToString() + ""","
                                                JSONExport += """script"":""" + Inpu.GetScriptString + ""","
                                                JSONExport += """scriptHex"":""" + Inpu.ScriptHex + ""","
                                                JSONExport += """scriptHash"":""" + Inpu.ScriptHash + ""","
                                                JSONExport += """amount"":" + String.Format("{0:#0.00000000}", ClsSignumAPI.Planck2Dbl(Inpu.AmountNQT)).Replace(",", ".") + ","
                                                JSONExport += """spendable"":" + Inpu.Spendable.ToString().ToLower() + ","
                                                JSONExport += """unsignedInput"":""" + Inpu.UnsignedTransactionHex + """"
                                                JSONExport += "},"
                                            Next

                                            JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                            JSONExport += "],""outputs"":["

                                            For i As Integer = 0 To BitcoinTransaction.Outputs.Count - 1

                                                Dim Outpu As ClsOutput = BitcoinTransaction.Outputs(i)
                                                JSONExport += "{"
                                                JSONExport += """index"":" + i.ToString() + ","
                                                JSONExport += """addresses"":""" + Outpu.GetAddressesString + ""","
                                                JSONExport += """type"":""" + Outpu.OutputType.ToString() + ""","
                                                JSONExport += """script"":""" + Outpu.GetScriptString + ""","
                                                JSONExport += """scriptHex"":""" + Outpu.ScriptHex + ""","
                                                JSONExport += """scriptHash"":""" + Outpu.ScriptHash + ""","
                                                JSONExport += """amount"":" + String.Format("{0:#0.00000000}", ClsSignumAPI.Planck2Dbl(Outpu.AmountNQT)).Replace(",", ".") + ","
                                                JSONExport += "},"
                                            Next

                                            JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                            JSONExport += "], ""signedTransaction"": """ + BitcoinTransaction.SignedTransactionHEX + """}"

                                            ResponseHTML = JSONExport

                                        End If


                                        If SenderAddresses.Count = 0 And PrivateKeys.Count = 0 Then
                                            'no addresses

                                        End If


                                    End If

                                End If

                            Case ClsAPIRequest.E_Endpoint.Transaction

                                If APIRequest.Parameters.Count > 0 Then
                                    'use Parameters

                                Else
                                    'check Body



                                End If

#Region "deprecaded"

                                ''http://127.0.0.1:8130/API/v1.0/CreateBitcoinTransaction?BitcoinTransaction=8f6d4029eefc4d3e86ca4759acc5c3a02b754850a371621c053a5cae14c3c957&BitcoinOutputType=TimeLockChainSwapHash&BitcoinSenderAddress=msgEkDrXVpAYCgY5vZFzRRyBddiks2G2ha&BitcoinRecipientAddress=msgEkDrXVpAYCgY5vZFzRRyBddiks2G2ha&BitcoinChainSwapHash=abcdef&BitcoinAmountNQT=2120

                                'Dim BTCInputTX As String = APIRequest.Parameters.FirstOrDefault(Function(c) c.Parameter = ClsAPIRequest.E_Parameter.BitcoinTransaction).Value
                                'Dim BTCSender As String = APIRequest.Parameters.FirstOrDefault(Function(c) c.Parameter = ClsAPIRequest.E_Parameter.BitcoinSenderAddress).Value
                                'Dim BTCRecipient As String = APIRequest.Parameters.FirstOrDefault(Function(c) c.Parameter = ClsAPIRequest.E_Parameter.BitcoinRecipientAddress).Value
                                'Dim BTCAmountNQT As ULong = Convert.ToUInt64(APIRequest.Parameters.FirstOrDefault(Function(c) c.Parameter = ClsAPIRequest.E_Parameter.BitcoinAmountNQT).Value)
                                'Dim BTCTimeOut As Integer = Convert.ToInt32(APIRequest.Parameters.FirstOrDefault(Function(c) c.Parameter = ClsAPIRequest.E_Parameter.BitcoinTimeOut).Value)
                                'Dim BTCChainSwapHash As String = APIRequest.Parameters.FirstOrDefault(Function(c) c.Parameter = ClsAPIRequest.E_Parameter.BitcoinChainSwapHash).Value

                                'Dim BTCTX As ClsTransaction = New ClsTransaction(BTCInputTX, BTCSender)
                                'BTCTX.CreateOutput(BTCRecipient, BTCChainSwapHash, "", ClsSignumAPI.Planck2Dbl(BTCAmountNQT))

                                'BTCTX.FinalizingOutputs()

                                'Dim ResponseData As String = ""

                                'ResponseData += """inputs"":["

                                'For Each x As ClsUnspentOutput In BTCTX.Inputs
                                '    ResponseData += "{"
                                '    ResponseData += """TransactionID"":""" + x.TransactionID + ""","
                                '    ResponseData += """OutputType"":""" + x.OutputType.ToString() + ""","
                                '    ResponseData += """ScriptHex"":""" + x.ScriptHex + ""","
                                '    ResponseData += """ScriptHash"":""" + x.ScriptHash + ""","
                                '    ResponseData += """UnsignedTransactionHex"":""" + x.UnsignedTransactionHex + """"
                                '    ResponseData += "},"
                                'Next

                                'ResponseData = ResponseData.Remove(ResponseData.Count - 1)
                                'ResponseData += "],"

                                'ResponseData += """outputs"":["

                                'For Each x As ClsOutput In BTCTX.Outputs

                                '    ResponseData += "{"
                                '    ResponseData += """AmountNQT"":" + x.AmountNQT.ToString() + ","
                                '    ResponseData += """OutputType"":""" + x.OutputType.ToString() + "(" + AbsClsOutputs.E_Type.ChainSwapHashWithLockTime.ToString() + ")"","
                                '    ResponseData += """ScriptHex"":""" + x.ScriptHex + ""","
                                '    ResponseData += """ScriptHash"":""" + x.ScriptHash + """"
                                '    ResponseData += "},"
                                'Next

                                'ResponseData = ResponseData.Remove(ResponseData.Count - 1)

                                'ResponseData += "]"

                                'ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""CreateBitcoinTransaction"",""data"":{" + ResponseData + "}}"

#End Region

                        End Select

                    Else
                        ResponseHTML = "{""response"":""error wrong request""}"
                    End If

                    SendHTMLResponse(TCPClient, ResponseHTML)

                End If

#Region "deprecaded"

                ''MultiInvoker(ListBox1, "Items", {"Insert", 0, htmlReq})
                ''MultiInvoker(ListBox1, "Items", {"Insert", 0, "HTTP Request: "})

                'Dim RequestList As List(Of String) = New List(Of String)
                'Dim CarrierReturn As Char = Convert.ToChar(vbCr)
                'RequestList.AddRange(htmlReq.Split(CarrierReturn))


                ''		(0)	    "GET /API/v1.0/GetCandles?pair=USD_SIGNA&days=3&tickmin=15 HTTP/1.1"	String
                ''		(1)	    "Host: localhost:8130"	String
                ''		(2)	    "Connection: keep-alive"	String
                ''		(3) 	"Cache-Control: max-age=0"	String
                ''		(4)	    "sec-ch-ua: ""Google Chrome"";v=""89"", ""Chromium"";v=""89"", "";Not A Brand"";v=""99"""	String
                ''		(5)	    "sec-ch-ua-mobile: ?0"	String
                ''		(6)	    "Upgrade-Insecure-Requests: 1"	String
                ''		(7)	    "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.128 Safari/537.36"	String
                ''		(8)	    "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"	String
                ''		(9)	    "Sec-Fetch-Site: none"	String
                ''		(10)	"Sec-Fetch-Mode: navigate"	String
                ''		(11)	"Sec-Fetch-User: ?1"	String
                ''		(12)	"Sec-Fetch-Dest: document"	String
                ''		(13)	"Accept-Encoding: gzip, deflate, br"	String
                ''		(14)	"Accept-Language: de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7"	String
                ''		(15)	""	String
                ''		(16)	""	String


                'If RequestList.Count > 0 Then

                '    Dim Method_Path_HTTPv As List(Of String) = New List(Of String)(RequestList(0).Split(" "c))

                '    If Method_Path_HTTPv(0).Trim.ToUpper = "GET" Then

                '        Dim Request As String = Method_Path_HTTPv(1).Trim
                '        Dim SubRequest As List(Of String) = New List(Of String)

                '        If Request.Contains("/") Then
                '            SubRequest.AddRange(Request.Split("/"c))
                '        End If

                '        If SubRequest.Count > 0 Then
                '            If SubRequest(0).Trim = "" Then
                '                SubRequest.RemoveAt(0)
                '            End If
                '        End If

                '        Dim APIInterface As String = ""
                '        Dim APIVersion As String = ""
                '        Dim APICommand As String = ""

                '        If SubRequest.Count >= 3 Then
                '            APIInterface = SubRequest(0)
                '            APIVersion = SubRequest(1)
                '            APICommand = SubRequest(2)
                '        End If


                '        Dim ResponseHTML As String = "{""response"":""no data.""}"

                '        Dim QueryParameters As List(Of String) = New List(Of String)
                '        If APICommand.Contains("?") Then
                '            Dim T_Parameters As String = APICommand.Substring(APICommand.IndexOf("?") + 1)
                '            APICommand = APICommand.Remove(APICommand.IndexOf("?"))
                '            QueryParameters.AddRange(T_Parameters.Split("&"c).ToArray)
                '        End If

                '        Dim FoundStatic As Boolean = False
                '        For Each Response As API_Response In ResponseMSGList
                '            If Response.API_Interface = APIInterface And Response.API_Version = APIVersion And Response.API_Command = APICommand Then

                '                If QueryParameters.Count > 0 Then
                '                    If CheckLists(QueryParameters, Response.API_Parameters) Then
                '                        ResponseHTML = Response.API_Response
                '                        FoundStatic = True
                '                    End If
                '                Else
                '                    ResponseHTML = Response.API_Response
                '                    FoundStatic = True
                '                End If

                '            End If
                '        Next

                '        If Not FoundStatic Then

                '            If APICommand = "AcceptOrder" Then

                '                If QueryParameters.Count > 0 Then

                '                    Dim ReqID As String = ""
                '                    Dim PublicKey As String = ""

                '                    For Each Query As String In QueryParameters

                '                        Dim T_ReqKey As String = Query.Split("="c)(0)
                '                        Dim T_ReqValue As String = Query.Split("="c)(1)

                '                        If T_ReqKey = "DEXContractAddress" Then
                '                            ReqID = ClsReedSolomon.Decode(T_ReqValue).ToString
                '                        End If


                '                        If T_ReqKey = "DEXContractID" Then
                '                            ReqID = T_ReqValue
                '                        End If

                '                        If T_ReqKey = "PublicKey" Then
                '                            PublicKey = T_ReqValue
                '                        End If

                '                    Next


                '                    If Not ReqID = "" And Not ReqID = "0" And Not PublicKey = "" And MessageIsHEXString(PublicKey) And PublicKey.Length = 64 Then
                '                        For Each T_DEXContract As ClsDEXContract In PFPForm.DEXContractList
                '                            If T_DEXContract.ID.ToString = ReqID Then
                '                                'address = TS-4FCL-YHVW-R94Z-F4D7J ; id = 15570460086676567378
                '                                'publickey = 6FBE5B0C2A6BA72612702795B2E250616C367BD8B28F965A36CD59DD13D09A51
                '                                'http://127.0.0.1:8130/API/v1.0/AcceptOrder?DEXContractAddress=TS-4FCL-YHVW-R94Z-F4D7J&PublicKey=6FBE5B0C2A6BA72612702795B2E250616C367BD8B28F965A36CD59DD13D09A51

                '                                If T_DEXContract.Status = ClsDEXContract.E_Status.OPEN Then 'T_DEXContract.IsSellOrder And
                '                                    Dim T_UnsignedTransactionBytes As String = T_DEXContract.AcceptSellOrder(PublicKey)

                '                                    If T_UnsignedTransactionBytes.Contains("errorCode") Then
                '                                        ResponseHTML = T_UnsignedTransactionBytes.Substring(T_UnsignedTransactionBytes.IndexOf("->") + 2).Trim
                '                                    ElseIf T_UnsignedTransactionBytes.Contains(Application.ProductName + "-error") Then

                '                                    Else
                '                                        '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                '                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""AcceptOrder"",""data"":{""unsignedTransactionBytes"":""" + T_UnsignedTransactionBytes + """}}"
                '                                    End If

                '                                End If

                '                            End If
                '                        Next
                '                    End If

                '                End If

                '            End If

                '        End If

                '        'ResponseHTML += "<form action = ""GetCandles"" id=""person"" method=""POST"">"
                '        'ResponseHTML += "<Label Class=""h2"" form=""person"">API-login</label>"
                '        'ResponseHTML += "<Label for=""vorname"">APIUser</label>"
                '        'ResponseHTML += "<input type = ""text"" name=""APIUser"" id=""APIUser"" maxlength=""30"">"
                '        'ResponseHTML += "<Label for=""zuname"">APIKey</label>"
                '        'ResponseHTML += "<input type = ""text"" name=""APIKey"" id=""APIKey"" maxlength=""40"">"
                '        'ResponseHTML += "<Button type = ""reset"" > Eingaben zur&uuml;cksetzen</button>"
                '        'ResponseHTML += "<Button type = ""submit"" > Eingaben absenden</button>"
                '        'ResponseHTML += "</form>"

                '        'ResponseHTML += "{""response"":""GetCandles"",""Data"":{""PAIR"":""USD/SIGNA"",""TICMIN"":""5"",{""Date"":""01.01.2021 12:30:00"",""OPEN"":""12.34567890"",""HIGH"":""15.12345678"",""LOW"":""10.87654321"",""CLOSE"":""13.24681357""},{""Date"":""01.01.2021 12:35:00"",""OPEN"":""13.24681357"",""HIGH"":""16.12345678"",""LOW"":""9.87654321"",""CLOSE"":""12.09876543""}}}"

                '        SendHTMLResponse(TCPClient, ResponseHTML)

                '    ElseIf Method_Path_HTTPv(0).Trim.ToUpper = "POST" Then

                '        '### Chrome ###
                '        '		(0)	"POST /GetCandles HTTP/1.1"	String
                '        '		(1)	"Host: localhost:8130"	String
                '        '		(2)	"Connection: keep-alive"	String
                '        '		(3)	"Content-Length: 30"	String
                '        '		(4)	"Cache-Control: max-age=0"	String
                '        '		(5)	"sec-ch-ua: "" Not A;Brand"";v=""99"", ""Chromium"";v=""90"", ""Google Chrome"";v=""90"""	String
                '        '		(6)	"sec-ch-ua-mobile: ?0"	String
                '        '		(7)	"Upgrade-Insecure-Requests: 1"	String
                '        '		(8)	"Origin: http://localhost:8130"	String
                '        '		(9)	"Content-Type: application/x-www-form-urlencoded"	String
                '        '		(10)	"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36"	String
                '        '		(11)	"Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"	String
                '        '		(12)	"Sec-Fetch-Site: same-origin"	String
                '        '		(13)	"Sec-Fetch-Mode: navigate"	String
                '        '		(14)	"Sec-Fetch-User: ?1"	String
                '        '		(15)	"Sec-Fetch-Dest: document"	String
                '        '		(16)	"Referer: http://localhost:8130/"	String
                '        '		(17)	"Accept-Encoding: gzip, deflate, br"	String
                '        '		(18)	"Accept-Language: de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7"	String
                '        '		(19)	""	String
                '        '		(20)	"APIUser=apimann&APIKey=apipass"	String	


                '        'Dim ResponseHTML As String = ""
                '        'ResponseHTML += "{""response"":""GetCandles"",""Data"":{""PAIR"":""USD/SIGNA"",""TICMIN"":""5"",{""Date"":""01.01.2021 12:30:00"",""OPEN"":""12.34567890"",""HIGH"":""15.12345678"",""LOW"":""10.87654321"",""CLOSE"":""13.24681357""},{""Date"":""01.01.2021 12:35:00"",""OPEN"":""13.24681357"",""HIGH"":""16.12345678"",""LOW"":""9.87654321"",""CLOSE"":""12.09876543""}}}"

                '        SendHTMLResponse(TCPClient, "{""response"":""no data.""}")

                '    Else
                '        ' Not HTTP GET method

                '        SendHTMLResponse(TCPClient, "{""response"":""error wrong request""}")

                '    End If

                'End If

#End Region

            Catch ex As Exception

                If API_ShowStatusMSG Then
                    StatusMSG.Add("ProcessRequest(): " + ex.Message)
                End If

                TCPClient.Close()

            End Try

        Next

    End Sub

    'Private Function CheckLists(ByVal List1 As List(Of String), List2 As List(Of String)) As Boolean

    '    Dim Returner As Boolean = True

    '    If List1.Count <> List2.Count Then
    '        Return False
    '    End If

    '    If List1.Count = 0 Then
    '        Return True
    '    End If

    '    Dim Result As List(Of Boolean) = New List(Of Boolean)

    '    For i As Integer = 0 To List1.Count - 1

    '        Dim List1Entry As String = List1(i)

    '        Dim Founded As Boolean = False
    '        For j As Integer = 0 To List2.Count - 1

    '            Dim List2Entry As String = List2(j)

    '            If List1Entry = List2Entry Then
    '                Result.Add(True)
    '                Founded = True
    '                Exit For
    '            End If

    '        Next

    '        If Not Founded Then
    '            Result.Add(False)
    '        End If

    '    Next

    '    For Each Res As Boolean In Result
    '        If Res = False Then
    '            Return False
    '        End If
    '    Next

    '    Return True

    'End Function

    Private Sub SendHTMLResponse(ByVal TCPClient As TcpClient, ByVal httpRequest As String)

        Try

            Dim respByte() As Byte = Encoding.ASCII.GetBytes(httpRequest)

            Dim htmlHeader As String =
                "HTTP/1.0 200 OK" & ControlChars.CrLf &
                "Server: PFPAPIServer 1.0" & ControlChars.CrLf &
                "Content-Length: " & respByte.Length & ControlChars.CrLf &
                "Content-Type: " & "application/json" &' getContentType(httpRequest) &
                ControlChars.CrLf & ControlChars.CrLf
            ' The content Length of HTML Header
            Dim headerByte() As Byte = Encoding.ASCII.GetBytes(htmlHeader)

            'MultiInvoker(ListBox1, "Items", {"Insert", 0, "HTML Header: " & ControlChars.CrLf & htmlHeader})

            If API_ShowStatusMSG Then
                StatusMSG.Add("HTML Header: " & ControlChars.CrLf & htmlHeader)
            End If

            ' Send HTML Header back to Web Browser
            TCPClient.Client.Send(headerByte, 0, headerByte.Length, SocketFlags.None)
            ' Send HTML Content back to Web Browser
            TCPClient.Client.Send(respByte, 0, respByte.Length, SocketFlags.None)
            ' Close HTTP Socket connection
            TCPClient.Client.Shutdown(SocketShutdown.Both)
            TCPClient.Close()

        Catch ex As Exception
            If API_ShowStatusMSG Then
                StatusMSG.Add("sendHTMLResponse(): " + ex.Message)
            End If
        End Try

    End Sub

End Class