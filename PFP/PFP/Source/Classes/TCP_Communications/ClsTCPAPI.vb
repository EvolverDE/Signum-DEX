
Option Strict On
Option Explicit On

'http://127.0.0.1:8130/API/v1.0/GetInfo

Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.Remoting.Channels
Imports System.Text
Imports System.Threading
Imports PFP.ClsBitcoinTransaction

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

                Dim byterange As Integer = CInt(Integer.MaxValue * 0.25)

                Dim recvBytes(byterange) As Byte
                Dim htmlReq As String = Nothing
                Dim bytes As Integer = 0

                Try
                    bytes = TCPClient.Client.Receive(recvBytes, 0, TCPClient.Client.Available, SocketFlags.None)
                    htmlReq = Encoding.ASCII.GetString(recvBytes, 0, bytes)
                    htmlReq = htmlReq.Replace(vbLf, "")

                Catch ex As Exception
                    TCPClient.Close()
                    Continue For
                End Try

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

                                    If APIRequest.Parameters.Count > 0 Then

                                        Dim Inputs As List(Of String) = New List(Of String)
                                        Dim PrivateKeys As List(Of ClsBitcoinTransaction.S_PrivateKey) = New List(Of ClsBitcoinTransaction.S_PrivateKey)
                                        Dim SenderAddresses As List(Of ClsBitcoinTransaction.S_Address) = New List(Of ClsBitcoinTransaction.S_Address)
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
                                                            SenderAddresses.Add(New ClsBitcoinTransaction.S_Address(T_Address))
                                                        Next

                                                    Else
                                                        SenderAddresses.Add(New ClsBitcoinTransaction.S_Address(Parameter.Value))
                                                    End If

                                                Case ClsAPIRequest.E_Parameter.PrivateKey

                                                    Dim T_PKCSK As ClsBitcoinTransaction.S_PrivateKey = New ClsBitcoinTransaction.S_PrivateKey(,)

                                                    If Parameter.Value.Contains(",") Then

                                                        Dim T_Privatekeys As List(Of String) = New List(Of String)(Parameter.Value.Split(","c))

                                                        For Each T_PrivateKey As String In T_Privatekeys
                                                            PrivateKeys.Add(New ClsBitcoinTransaction.S_PrivateKey(T_PrivateKey))
                                                        Next
                                                    Else
                                                        PrivateKeys.Add(New ClsBitcoinTransaction.S_PrivateKey(Parameter.Value))
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

                                        If SenderAddresses.Count = 0 And PrivateKeys.Count > 0 Then
                                            For Each T_PrivateKey As ClsBitcoinTransaction.S_PrivateKey In PrivateKeys
                                                Dim T_Address As ClsBitcoinTransaction.S_Address = New ClsBitcoinTransaction.S_Address()
                                                T_Address.Address = (PrivKeyToPubKey(T_PrivateKey.PrivateKey))
                                                T_Address.ChainSwapKey = If(T_PrivateKey.ChainSwapKey.Trim() = "", False, True)
                                                SenderAddresses.Add(T_Address)
                                            Next
                                        End If

                                        If SenderAddresses.Count > 0 And PrivateKeys.Count = 0 Then

                                            Dim BitcoinTransaction As ClsBitcoinTransaction = New ClsBitcoinTransaction(Inputs, SenderAddresses, "")
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

                                            If BitcoinTransaction.Inputs.Count > 0 Then
                                                JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                            End If

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

                                            If BitcoinTransaction.Outputs.Count > 0 Then
                                                JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                            End If

                                            JSONExport += "]}"

                                            ResponseHTML = JSONExport

                                        End If

                                        If SenderAddresses.Count > 0 And PrivateKeys.Count > 0 Then

                                            Dim BitcoinTransaction As ClsBitcoinTransaction = New ClsBitcoinTransaction(Inputs, SenderAddresses, "")
                                            BitcoinTransaction.CreateOutput(RecipientAddress, ChainSwapHash, SenderAddresses(0).Address, Amount)
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

                                            If BitcoinTransaction.Inputs.Count > 0 Then
                                                JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                            End If

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

                                            If BitcoinTransaction.Outputs.Count > 0 Then
                                                JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                            End If

                                            JSONExport += "], ""signedTransaction"": """ + BitcoinTransaction.SignedTransactionHEX + """}"

                                            ResponseHTML = JSONExport

                                        End If

                                    End If

                                Case ClsAPIRequest.E_Endpoint.Orders

                                    If APIRequest.Parameters.Count > 0 Then
                                        'use Parameters

                                        Dim OrderID As String = APIRequest.Parameters.FirstOrDefault(Function(OID) OID.Parameter = ClsAPIRequest.E_Parameter.ID).Value

                                        If Not IsNothing(OrderID) Then
                                            If Not IsNumeric(OrderID) Then
                                                OrderID = ClsReedSolomon.Decode(OrderID).ToString()
                                            End If
                                        Else
                                            OrderID = ""
                                        End If

                                        Dim PassPhrase As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.PassPhrase).Value
                                        Dim PublicKey As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.PublicKey).Value
                                        'If IsNothing(PublicKey) Then PublicKey = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.SenderPublicKey).Value

                                        Dim Type As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.Type).Value
                                        Dim AmountNQT As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.AmountNQT).Value
                                        Dim XAmountNQT As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.XAmountNQT).Value
                                        Dim CollateralNQT As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.CollateralNQT).Value
                                        Dim XItem As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.XItem).Value

                                        If IsNothing(Type) Then Type = ""
                                        If IsNothing(AmountNQT) Then AmountNQT = ""
                                        If IsNothing(XAmountNQT) Then XAmountNQT = ""
                                        If IsNothing(CollateralNQT) Then CollateralNQT = ""

                                        If IsNothing(XItem) Then
                                            XItem = ""
                                        ElseIf Not SupportedCurrencies.Contains(XItem) Then
                                            XItem = ""
                                        End If

                                        Dim Masterkeys As List(Of String) = New List(Of String)
                                        If Not IsNothing(PassPhrase) Then
                                            Masterkeys = GetMasterKeys(PassPhrase)
                                            PublicKey = Masterkeys(0)
                                        Else
                                            If IsNothing(PublicKey) Then PublicKey = ""
                                            PassPhrase = ""
                                        End If

                                        If Not OrderID = "" And PassPhrase = "" And Not PublicKey = "" And Type = "" And AmountNQT = "" And CollateralNQT = "" And XAmountNQT = "" And XItem = "" Then
                                            'Accept
                                            If MessageIsHEXString(PublicKey) And PublicKey.Length = 64 Then
                                                For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList
                                                    If T_DEXContract.ID.ToString() = OrderID Then
                                                        'address = TS-4FCL-YHVW-R94Z-F4D7J ; id = 15570460086676567378
                                                        'publickey = 6FBE5B0C2A6BA726 12702795B2E25061 6C367BD8B28F965A 36CD59DD13D09A51

                                                        If T_DEXContract.Status = ClsDEXContract.E_Status.OPEN Then 'T_DEXContract.IsSellOrder And
                                                            Dim T_UnsignedTransactionBytes As String = T_DEXContract.AcceptSellOrder(PublicKey)

                                                            If T_UnsignedTransactionBytes.Contains("errorCode") Then
                                                                ResponseHTML = T_UnsignedTransactionBytes.Substring(T_UnsignedTransactionBytes.IndexOf("->") + 2).Trim
                                                            ElseIf T_UnsignedTransactionBytes.Contains(Application.ProductName + "-error") Then

                                                            Else
                                                                '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                                ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""acceptOrder"",""data"":{""unsignedTransactionBytes"":""" + T_UnsignedTransactionBytes + """}}"

                                                            End If

                                                        End If

                                                    End If
                                                Next

                                            End If

                                        ElseIf Not OrderID = "" And Not PassPhrase = "" And Not PublicKey = "" And Type = "" And AmountNQT = "" And CollateralNQT = "" And XAmountNQT = "" And XItem = "" Then
                                            'Accept + Sign
                                            If MessageIsHEXString(PublicKey) And PublicKey.Length = 64 Then
                                                For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList
                                                    If T_DEXContract.ID.ToString() = OrderID Then

                                                        If T_DEXContract.Status = ClsDEXContract.E_Status.OPEN Then 'T_DEXContract.IsSellOrder And
                                                            Dim T_TransactionID As String = T_DEXContract.AcceptSellOrder(PublicKey,,, Masterkeys(1))

                                                            If T_TransactionID.Contains("errorCode") Then
                                                                ResponseHTML = T_TransactionID.Substring(T_TransactionID.IndexOf("->") + 2).Trim
                                                            ElseIf T_TransactionID.Contains(Application.ProductName + "-error") Then

                                                            Else
                                                                '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                                ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""acceptOrder"",""data"":{""transaction"":""" + T_TransactionID + """}}"
                                                            End If

                                                        End If

                                                    End If
                                                Next

                                            End If

                                        ElseIf Not OrderID = "" And PassPhrase = "" And Not PublicKey = "" And Not Type = "" And Not AmountNQT = "" And Not CollateralNQT = "" And Not XAmountNQT = "" And Not XItem = "" Then
                                            'Create
                                            If MessageIsHEXString(PublicKey) And PublicKey.Length = 64 And SupportedCurrencies.Contains(XItem) Then

                                                Dim Amount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(AmountNQT))
                                                Dim Collateral As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))
                                                Dim XAmount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))

                                                For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList
                                                    If T_DEXContract.ID.ToString() = OrderID And (T_DEXContract.Status = ClsDEXContract.E_Status.FREE Or (T_DEXContract.Status = ClsDEXContract.E_Status.NEW_ And T_DEXContract.CreatorID = GetAccountID(PublicKey))) Then

                                                        Dim T_UnsignedTransactionBytes As String = T_DEXContract.CreateSellOrder(PublicKey, Amount, Collateral, XItem, XAmount, 0.0)

                                                        If T_UnsignedTransactionBytes.Contains("errorCode") Then
                                                            ResponseHTML = T_UnsignedTransactionBytes.Substring(T_UnsignedTransactionBytes.IndexOf("->") + 2).Trim
                                                        ElseIf T_UnsignedTransactionBytes.Contains(Application.ProductName + "-error") Then

                                                        Else
                                                            '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                            ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createOrder"",""data"":{""contract"":""" + T_DEXContract.ID.ToString() + """,""unsignedTransactionBytes"":""" + T_UnsignedTransactionBytes + """}}"
                                                        End If

                                                    End If
                                                Next

                                            End If

                                        ElseIf OrderID = "" And PassPhrase = "" And Not PublicKey = "" And Not Type = "" And Not AmountNQT = "" And Not CollateralNQT = "" And Not XAmountNQT = "" And Not XItem = "" Then
                                            'Create
                                            If MessageIsHEXString(PublicKey) And PublicKey.Length = 64 And SupportedCurrencies.Contains(XItem) Then

                                                Dim Amount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(AmountNQT))
                                                Dim Collateral As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))
                                                Dim XAmount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(XAmountNQT))

                                                For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList
                                                    If T_DEXContract.Status = ClsDEXContract.E_Status.FREE Or (T_DEXContract.Status = ClsDEXContract.E_Status.NEW_ And T_DEXContract.CreatorID = GetAccountID(PublicKey)) Then

                                                        Dim T_UnsignedTransactionBytes As String = T_DEXContract.CreateSellOrder(PublicKey, Amount, Collateral, XItem, XAmount, 0.0)

                                                        If T_UnsignedTransactionBytes.Contains("errorCode") Then
                                                            ResponseHTML = T_UnsignedTransactionBytes.Substring(T_UnsignedTransactionBytes.IndexOf("->") + 2).Trim
                                                        ElseIf T_UnsignedTransactionBytes.Contains(Application.ProductName + "-error") Then

                                                        Else
                                                            '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                            ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createOrder"",""data"":{""contract"":""" + T_DEXContract.ID.ToString() + """,""unsignedTransactionBytes"":""" + T_UnsignedTransactionBytes + """}}"
                                                            Exit For
                                                        End If

                                                    End If
                                                Next

                                            End If

                                        ElseIf Not OrderID = "" And Not PassPhrase = "" And Not PublicKey = "" And Not Type = "" And Not AmountNQT = "" And Not CollateralNQT = "" And Not XAmountNQT = "" And Not XItem = "" Then
                                            'Create + Sign
                                            If MessageIsHEXString(PublicKey) And PublicKey.Length = 64 And SupportedCurrencies.Contains(XItem) Then

                                                Dim Amount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(AmountNQT))
                                                Dim Collateral As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))
                                                Dim XAmount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))

                                                For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList
                                                    If T_DEXContract.ID.ToString() = OrderID And (T_DEXContract.Status = ClsDEXContract.E_Status.FREE Or (T_DEXContract.Status = ClsDEXContract.E_Status.NEW_ And T_DEXContract.CreatorID = GetAccountID(PublicKey))) Then

                                                        Dim T_TransactionID As String = T_DEXContract.CreateSellOrder(Masterkeys(0), Amount, Collateral, XItem, XAmount, 0.0, Masterkeys(1))

                                                        If T_TransactionID.Contains("errorCode") Then
                                                            ResponseHTML = T_TransactionID.Substring(T_TransactionID.IndexOf("->") + 2).Trim
                                                        ElseIf T_TransactionID.Contains(Application.ProductName + "-error") Then

                                                        Else
                                                            '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                            ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createOrder"",""data"":{""transaction"":""" + T_TransactionID + """}}"
                                                        End If

                                                    End If
                                                Next

                                            End If

                                        ElseIf OrderID = "" And Not PassPhrase = "" And Not PublicKey = "" And Not Type = "" And Not AmountNQT = "" And Not CollateralNQT = "" And Not XAmountNQT = "" And Not XItem = "" Then
                                            'Create + Sign
                                            If MessageIsHEXString(PublicKey) And PublicKey.Length = 64 And SupportedCurrencies.Contains(XItem) Then

                                                Dim Amount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(AmountNQT))
                                                Dim Collateral As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))
                                                Dim XAmount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))

                                                For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList
                                                    If T_DEXContract.Status = ClsDEXContract.E_Status.FREE Or (T_DEXContract.Status = ClsDEXContract.E_Status.NEW_ And T_DEXContract.CreatorID = GetAccountID(PublicKey)) Then

                                                        Dim T_TransactionID As String = T_DEXContract.CreateSellOrder(Masterkeys(0), Amount, Collateral, XItem, XAmount, 0.0, Masterkeys(1))

                                                        If T_TransactionID.Contains("errorCode") Then
                                                            ResponseHTML = T_TransactionID.Substring(T_TransactionID.IndexOf("->") + 2).Trim
                                                        ElseIf T_TransactionID.Contains(Application.ProductName + "-error") Then

                                                        Else
                                                            '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                            ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createOrder"",""data"":{""transaction"":""" + T_TransactionID + """}}"
                                                            Exit For
                                                        End If

                                                    End If
                                                Next

                                            End If

                                        End If

                                    End If

                                Case ClsAPIRequest.E_Endpoint.Broadcast

                                    Dim Token As String = ""
                                    Dim SignedTransactionBytes As String = ""

                                    If APIRequest.Parameters.Count > 0 Then
                                        'use Parameters
                                        Token = APIRequest.Parameters.FirstOrDefault(Function(OID) OID.Parameter = ClsAPIRequest.E_Parameter.Token).Value
                                        SignedTransactionBytes = APIRequest.Parameters.FirstOrDefault(Function(OID) OID.Parameter = ClsAPIRequest.E_Parameter.SignedTransactionBytes).Value

                                        If IsNothing(Token) Then Token = ""
                                        If IsNothing(SignedTransactionBytes) Then SignedTransactionBytes = ""
                                    Else
                                        'check Body

                                        For Each BodyEntry As KeyValuePair(Of String, Object) In APIRequest.Body

                                            Select Case BodyEntry.Key.ToLower()
                                                Case "token"
                                                    Token = BodyEntry.Value.ToString()
                                                Case "signedtransactionbytes"
                                                    SignedTransactionBytes = BodyEntry.Value.ToString()
                                            End Select
                                        Next

                                    End If

                                    If Not Token = "" And Not SignedTransactionBytes = "" And MessageIsHEXString(SignedTransactionBytes) Then

                                        If SupportedCurrencies.Contains(Token) Then

                                            If ClsDEXContract.CurrencyIsCrypto(Token) Then
                                                Dim XItem As AbsClsXItem = ClsXItemAdapter.NewXItem(Token)
                                                Dim T_Result As String = XItem.BroadcastTransaction(SignedTransactionBytes)
                                                If Not IsErrorOrWarning(T_Result,,, False) Then
                                                    ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""broadcast"",""data"":{""message"":""" + T_Result + """}}"
                                                End If
                                            End If

                                        ElseIf Token = "Signum" Then

                                            Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI("")
                                            Dim T_Result As String = SignumAPI.BroadcastTransaction(SignedTransactionBytes)
                                            If Not IsErrorOrWarning(T_Result,,, False) Then
                                                ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""broadcast"",""data"":{""message"":""" + T_Result + """}}"
                                            End If

                                        End If

                                    End If

                                Case ClsAPIRequest.E_Endpoint.SmartContract

                                    Dim ID As String = ""

                                    If APIRequest.Parameters.Count > 0 Then
                                        ID = APIRequest.Parameters.FirstOrDefault(Function(OID) OID.Parameter = ClsAPIRequest.E_Parameter.ID).Value

                                        If IsNothing(ID) Then ID = ""
                                    Else
                                        'check Body

                                        For Each BodyEntry As KeyValuePair(Of String, Object) In APIRequest.Body

                                            Select Case BodyEntry.Key.ToLower()
                                                Case "id"
                                                    ID = BodyEntry.Value.ToString()
                                            End Select
                                        Next

                                    End If

                                    If Not ID.Trim() = "" Then

                                        Dim IDs As List(Of String) = New List(Of String)(ID.Split(","c))

                                        Dim T_Response As String = "{""application"":""PFPDEX"",""Interface"":""API"",""version"":""1"",""contentType"":""application/json"",""response"":""smartContract"",""data"":["

                                        For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList

                                            If IDs.Contains(T_DEXContract.ID.ToString()) Then

                                                T_Response += "{"
                                                T_Response += """at"":""" + T_DEXContract.ID.ToString() + ""","
                                                T_Response += """atRS"":""" + T_DEXContract.Address + ""","
                                                T_Response += """creatorID"":""" + T_DEXContract.CreatorID.ToString() + ""","
                                                T_Response += """creatorRS"":""" + T_DEXContract.CreatorAddress + ""","
                                                T_Response += """status"":""" + T_DEXContract.Status.ToString() + ""","
                                                T_Response += """deniability"":""" + T_DEXContract.Deniability.ToString().ToLower() + ""","
                                                T_Response += "},"

                                            End If

                                        Next

                                        T_Response = T_Response.Remove(T_Response.Length - 1)
                                        T_Response += "]}"

                                        If PFPForm.C_DEXContractList.Count = 0 Then
                                            ResponseHTML = T_Response
                                        End If

                                    End If

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

                            Case ClsAPIRequest.E_Endpoint.Bitcoin

                                If APIRequest.Parameters.Count > 0 Then
                                    'use Parameters

                                    Dim Inputs As List(Of String) = New List(Of String)
                                    Dim PrivateKeys As List(Of ClsBitcoinTransaction.S_PrivateKey) = New List(Of ClsBitcoinTransaction.S_PrivateKey)
                                    Dim SenderAddresses As List(Of ClsBitcoinTransaction.S_Address) = New List(Of ClsBitcoinTransaction.S_Address)
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
                                                        SenderAddresses.Add(New ClsBitcoinTransaction.S_Address(T_Address))
                                                    Next

                                                Else
                                                    SenderAddresses.Add(New ClsBitcoinTransaction.S_Address(Parameter.Value))
                                                End If

                                            Case ClsAPIRequest.E_Parameter.PrivateKey

                                                Dim T_PKCSK As ClsBitcoinTransaction.S_PrivateKey = New ClsBitcoinTransaction.S_PrivateKey(,)

                                                If Parameter.Value.Contains(",") Then

                                                    Dim T_Privatekeys As List(Of String) = New List(Of String)(Parameter.Value.Split(","c))

                                                    For Each T_PrivateKey As String In T_Privatekeys
                                                        PrivateKeys.Add(New ClsBitcoinTransaction.S_PrivateKey(T_PrivateKey))
                                                    Next
                                                Else
                                                    PrivateKeys.Add(New ClsBitcoinTransaction.S_PrivateKey(Parameter.Value))
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

                                    If SenderAddresses.Count = 0 And PrivateKeys.Count > 0 Then
                                        For Each T_PrivateKey As ClsBitcoinTransaction.S_PrivateKey In PrivateKeys
                                            Dim T_Address As ClsBitcoinTransaction.S_Address = New ClsBitcoinTransaction.S_Address()
                                            T_Address.Address = (PrivKeyToPubKey(T_PrivateKey.PrivateKey))
                                            T_Address.ChainSwapKey = If(T_PrivateKey.ChainSwapKey.Trim() = "", False, True)
                                            SenderAddresses.Add(T_Address)
                                        Next
                                    End If

                                    If SenderAddresses.Count > 0 And PrivateKeys.Count = 0 Then

                                        Dim BitcoinTransaction As ClsBitcoinTransaction = New ClsBitcoinTransaction(Inputs, SenderAddresses, "")
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

                                        If BitcoinTransaction.Inputs.Count > 0 Then
                                            JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                        End If

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

                                        If BitcoinTransaction.Outputs.Count > 0 Then
                                            JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                        End If

                                        JSONExport += "]}"

                                        ResponseHTML = JSONExport

                                    End If

                                    If SenderAddresses.Count > 0 And PrivateKeys.Count > 0 Then

                                        Dim BitcoinTransaction As ClsBitcoinTransaction = New ClsBitcoinTransaction(Inputs, SenderAddresses, "")
                                        BitcoinTransaction.CreateOutput(RecipientAddress, ChainSwapHash, SenderAddresses(0).Address, Amount)
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

                                        If BitcoinTransaction.Inputs.Count > 0 Then
                                            JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                        End If

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

                                        If BitcoinTransaction.Outputs.Count > 0 Then
                                            JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                        End If

                                        JSONExport += "], ""signedTransaction"": """ + BitcoinTransaction.SignedTransactionHEX + """}"

                                        ResponseHTML = JSONExport

                                    End If

                                Else
                                    'check Body

                                    If APIRequest.Body.Count > 0 Then

                                        Dim InputIDs As List(Of String) = New List(Of String)
                                        Dim PrivateKeys As List(Of ClsBitcoinTransaction.S_PrivateKey) = New List(Of ClsBitcoinTransaction.S_PrivateKey)
                                        Dim SenderAddresses As List(Of ClsBitcoinTransaction.S_Address) = New List(Of ClsBitcoinTransaction.S_Address)
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
                                                        Dim T_PKCSK As ClsBitcoinTransaction.S_PrivateKey = New ClsBitcoinTransaction.S_PrivateKey(,)
                                                        Dim T_Address As ClsBitcoinTransaction.S_Address = New ClsBitcoinTransaction.S_Address(,)
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
                                            For Each T_PrivateKey As ClsBitcoinTransaction.S_PrivateKey In PrivateKeys
                                                Dim T_Address As ClsBitcoinTransaction.S_Address = New ClsBitcoinTransaction.S_Address()
                                                T_Address.Address = (PrivKeyToPubKey(T_PrivateKey.PrivateKey))
                                                T_Address.ChainSwapKey = If(T_PrivateKey.ChainSwapKey.Trim() = "", False, True)
                                                SenderAddresses.Add(T_Address)
                                            Next
                                        End If

                                        If SenderAddresses.Count > 0 And PrivateKeys.Count = 0 Then

                                            Dim BitcoinTransaction As ClsBitcoinTransaction = New ClsBitcoinTransaction(InputIDs, SenderAddresses, Scripts)

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

                                            If BitcoinTransaction.Inputs.Count > 0 Then
                                                JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                            End If

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

                                            If BitcoinTransaction.Outputs.Count > 0 Then
                                                JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                            End If

                                            JSONExport += "]}"

                                            ResponseHTML = JSONExport

                                        End If

                                        If SenderAddresses.Count > 0 And PrivateKeys.Count > 0 Then

                                            Dim BitcoinTransaction As ClsBitcoinTransaction = New ClsBitcoinTransaction(InputIDs, SenderAddresses, Scripts)

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

                                            If BitcoinTransaction.Inputs.Count > 0 Then
                                                JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                            End If

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

                                            If BitcoinTransaction.Outputs.Count > 0 Then
                                                JSONExport = JSONExport.Remove(JSONExport.Length - 1)
                                            End If

                                            JSONExport += "], ""signedTransaction"": """ + BitcoinTransaction.SignedTransactionHEX + """}"

                                            ResponseHTML = JSONExport

                                        End If


                                        If SenderAddresses.Count = 0 And PrivateKeys.Count = 0 Then
                                            'no addresses

                                        End If


                                    End If

                                End If

                            Case ClsAPIRequest.E_Endpoint.Orders

                                If APIRequest.Parameters.Count > 0 Then
                                    'use Parameters

                                    Dim OrderID As String = APIRequest.Parameters.FirstOrDefault(Function(OID) OID.Parameter = ClsAPIRequest.E_Parameter.ID).Value

                                    If Not IsNothing(OrderID) Then
                                        If Not IsNumeric(OrderID) Then
                                            OrderID = ClsReedSolomon.Decode(OrderID).ToString()
                                        End If
                                    Else
                                        OrderID = ""
                                    End If

                                    Dim PassPhrase As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.PassPhrase).Value
                                    Dim PublicKey As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.PublicKey).Value
                                    'If IsNothing(PublicKey) Then PublicKey = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.SenderPublicKey).Value

                                    Dim Type As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.Type).Value
                                    Dim AmountNQT As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.AmountNQT).Value
                                    Dim XAmountNQT As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.XAmountNQT).Value
                                    Dim CollateralNQT As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.CollateralNQT).Value
                                    Dim XItem As String = APIRequest.Parameters.FirstOrDefault(Function(PK) PK.Parameter = ClsAPIRequest.E_Parameter.XItem).Value

                                    If IsNothing(Type) Then Type = ""
                                    If IsNothing(AmountNQT) Then AmountNQT = ""
                                    If IsNothing(XAmountNQT) Then XAmountNQT = ""
                                    If IsNothing(CollateralNQT) Then CollateralNQT = ""
                                    If IsNothing(XItem) Then
                                        XItem = ""
                                    ElseIf Not SupportedCurrencies.Contains(XItem) Then
                                        XItem = ""
                                    End If

                                    Dim Masterkeys As List(Of String) = New List(Of String)
                                    If Not IsNothing(PassPhrase) Then
                                        Masterkeys = GetMasterKeys(PassPhrase)
                                        PublicKey = Masterkeys(0)
                                    Else
                                        If IsNothing(PublicKey) Then PublicKey = ""
                                        PassPhrase = ""
                                    End If

                                    If Not OrderID = "" And PassPhrase = "" And Not PublicKey = "" And Type = "" And AmountNQT = "" And CollateralNQT = "" And XAmountNQT = "" And XItem = "" Then
                                        'Accept
                                        If MessageIsHEXString(PublicKey) And PublicKey.Length = 64 Then
                                            For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList
                                                If T_DEXContract.ID.ToString() = OrderID Then
                                                    'address = TS-4FCL-YHVW-R94Z-F4D7J ; id = 15570460086676567378
                                                    'publickey = 6FBE5B0C2A6BA726 12702795B2E25061 6C367BD8B28F965A 36CD59DD13D09A51

                                                    If T_DEXContract.Status = ClsDEXContract.E_Status.OPEN Then 'T_DEXContract.IsSellOrder And
                                                        Dim T_UnsignedTransactionBytes As String = T_DEXContract.AcceptSellOrder(PublicKey)

                                                        If T_UnsignedTransactionBytes.Contains("errorCode") Then
                                                            ResponseHTML = T_UnsignedTransactionBytes.Substring(T_UnsignedTransactionBytes.IndexOf("->") + 2).Trim
                                                        ElseIf T_UnsignedTransactionBytes.Contains(Application.ProductName + "-error") Then

                                                        Else
                                                            '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                            ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""acceptOrder"",""data"":{""contract"":""" + T_DEXContract.ID.ToString() + """,""unsignedTransactionBytes"":""" + T_UnsignedTransactionBytes + """}}"

                                                        End If

                                                    End If

                                                End If
                                            Next

                                        End If

                                    ElseIf Not OrderID = "" And Not PassPhrase = "" And Not PublicKey = "" And Type = "" And AmountNQT = "" And CollateralNQT = "" And XAmountNQT = "" And XItem = "" Then
                                        'Accept + Sign
                                        If MessageIsHEXString(PublicKey) And PublicKey.Length = 64 Then
                                            For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList
                                                If T_DEXContract.ID.ToString() = OrderID Then

                                                    If T_DEXContract.Status = ClsDEXContract.E_Status.OPEN Then 'T_DEXContract.IsSellOrder And
                                                        Dim T_TransactionID As String = T_DEXContract.AcceptSellOrder(PublicKey,,, Masterkeys(1))

                                                        If T_TransactionID.Contains("errorCode") Then
                                                            ResponseHTML = T_TransactionID.Substring(T_TransactionID.IndexOf("->") + 2).Trim
                                                        ElseIf T_TransactionID.Contains(Application.ProductName + "-error") Then

                                                        Else
                                                            '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                            ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""acceptOrder"",""data"":{""transaction"":""" + T_TransactionID + """}}"
                                                        End If

                                                    End If

                                                End If
                                            Next

                                        End If

                                    ElseIf Not OrderID = "" And PassPhrase = "" And Not PublicKey = "" And Not Type = "" And Not AmountNQT = "" And Not CollateralNQT = "" And Not XAmountNQT = "" And Not XItem = "" Then
                                        'Create
                                        If MessageIsHEXString(PublicKey) And PublicKey.Length = 64 And SupportedCurrencies.Contains(XItem) Then

                                            Dim Amount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(AmountNQT))
                                            Dim Collateral As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))
                                            Dim XAmount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))

                                            For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList
                                                If T_DEXContract.ID.ToString() = OrderID And (T_DEXContract.Status = ClsDEXContract.E_Status.FREE Or (T_DEXContract.Status = ClsDEXContract.E_Status.NEW_ And T_DEXContract.CreatorID = GetAccountID(PublicKey))) Then

                                                    Dim T_UnsignedTransactionBytes As String = T_DEXContract.CreateSellOrder(Masterkeys(0), Amount, Collateral, XItem, XAmount, 0.0)

                                                    If T_UnsignedTransactionBytes.Contains("errorCode") Then
                                                        ResponseHTML = T_UnsignedTransactionBytes.Substring(T_UnsignedTransactionBytes.IndexOf("->") + 2).Trim
                                                    ElseIf T_UnsignedTransactionBytes.Contains(Application.ProductName + "-error") Then

                                                    Else
                                                        '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createOrder"",""data"":{""contract"":""" + T_DEXContract.ID.ToString() + """,""unsignedTransactionBytes"":""" + T_UnsignedTransactionBytes + """}}"
                                                    End If

                                                End If
                                            Next

                                        End If

                                    ElseIf OrderID = "" And PassPhrase = "" And Not PublicKey = "" And Not Type = "" And Not AmountNQT = "" And Not CollateralNQT = "" And Not XAmountNQT = "" And Not XItem = "" Then
                                        'Create
                                        If MessageIsHEXString(PublicKey) And PublicKey.Length = 64 And SupportedCurrencies.Contains(XItem) Then

                                            Dim Amount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(AmountNQT))
                                            Dim Collateral As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))
                                            Dim XAmount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(XAmountNQT))

                                            For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList
                                                If T_DEXContract.Status = ClsDEXContract.E_Status.FREE Or (T_DEXContract.Status = ClsDEXContract.E_Status.NEW_ And T_DEXContract.CreatorID = GetAccountID(PublicKey)) Then

                                                    Dim T_UnsignedTransactionBytes As String = T_DEXContract.CreateSellOrder(PublicKey, Amount, Collateral, XItem, XAmount, 0.0)

                                                    If T_UnsignedTransactionBytes.Contains("errorCode") Then
                                                        ResponseHTML = T_UnsignedTransactionBytes.Substring(T_UnsignedTransactionBytes.IndexOf("->") + 2).Trim
                                                    ElseIf T_UnsignedTransactionBytes.Contains(Application.ProductName + "-error") Then

                                                    Else
                                                        '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createOrder"",""data"":{""contract"":""" + T_DEXContract.ID.ToString() + """,""unsignedTransactionBytes"":""" + T_UnsignedTransactionBytes + """}}"
                                                        Exit For
                                                    End If

                                                End If
                                            Next

                                        End If

                                    ElseIf Not OrderID = "" And Not PassPhrase = "" And Not PublicKey = "" And Not Type = "" And Not AmountNQT = "" And Not CollateralNQT = "" And Not XAmountNQT = "" And Not XItem = "" Then
                                        'Create + Sign
                                        If MessageIsHEXString(PublicKey) And PublicKey.Length = 64 And SupportedCurrencies.Contains(XItem) Then

                                            Dim Amount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(AmountNQT))
                                            Dim Collateral As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))
                                            Dim XAmount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))

                                            For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList
                                                If T_DEXContract.ID.ToString() = OrderID And (T_DEXContract.Status = ClsDEXContract.E_Status.FREE Or (T_DEXContract.Status = ClsDEXContract.E_Status.NEW_ And T_DEXContract.CreatorID = GetAccountID(PublicKey))) Then

                                                    Dim T_TransactionID As String = T_DEXContract.CreateSellOrder(Masterkeys(0), Amount, Collateral, XItem, XAmount, 0.0, Masterkeys(1))

                                                    If T_TransactionID.Contains("errorCode") Then
                                                        ResponseHTML = T_TransactionID.Substring(T_TransactionID.IndexOf("->") + 2).Trim
                                                    ElseIf T_TransactionID.Contains(Application.ProductName + "-error") Then

                                                    Else
                                                        '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createOrder"",""data"":{""transaction"":""" + T_TransactionID + """}}"
                                                    End If

                                                End If
                                            Next

                                        End If

                                    ElseIf OrderID = "" And Not PassPhrase = "" And Not PublicKey = "" And Not Type = "" And Not AmountNQT = "" And Not CollateralNQT = "" And Not XAmountNQT = "" And Not XItem = "" Then
                                        'Create + Sign
                                        If MessageIsHEXString(PublicKey) And PublicKey.Length = 64 And SupportedCurrencies.Contains(XItem) Then

                                            Dim Amount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(AmountNQT))
                                            Dim Collateral As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))
                                            Dim XAmount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(CollateralNQT))

                                            For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList
                                                If T_DEXContract.Status = ClsDEXContract.E_Status.FREE Or (T_DEXContract.Status = ClsDEXContract.E_Status.NEW_ And T_DEXContract.CreatorID = GetAccountID(PublicKey)) Then

                                                    Dim T_TransactionID As String = T_DEXContract.CreateSellOrder(Masterkeys(0), Amount, Collateral, XItem, XAmount, 0.0, Masterkeys(1))

                                                    If T_TransactionID.Contains("errorCode") Then
                                                        ResponseHTML = T_TransactionID.Substring(T_TransactionID.IndexOf("->") + 2).Trim
                                                    ElseIf T_TransactionID.Contains(Application.ProductName + "-error") Then

                                                    Else
                                                        '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createOrder"",""data"":{""transaction"":""" + T_TransactionID + """}}"
                                                        Exit For
                                                    End If

                                                End If
                                            Next

                                        End If

                                    End If

                                Else
                                    'check Body
                                    Dim OrderID As String = ""
                                    Dim SellOrder As Boolean = True
                                    Dim PublicKey As String = ""
                                    Dim PassPhrase As String = ""
                                    Dim InjectPublicKey As String = ""
                                    Dim InjectChainSwapHash As String = ""
                                    Dim ChainSwapKey As String = ""
                                    Dim SignHexKey As String = ""
                                    Dim AmountNQT As String = ""
                                    Dim CollateralNQT As String = ""
                                    Dim XAmountNQT As String = ""
                                    Dim XItem As String = ""

                                    For Each BodyEntry As KeyValuePair(Of String, Object) In APIRequest.Body

                                        Select Case BodyEntry.Key.ToLower()
                                            Case "orderid", "contract"
                                                OrderID = BodyEntry.Value.ToString()
                                                If IsReedSolomon(OrderID) Then
                                                    OrderID = ClsReedSolomon.Decode(OrderID).ToString()
                                                End If
                                            Case "type"
                                                If Not BodyEntry.Value.ToString() = "SellOrder" Then
                                                    SellOrder = False
                                                End If
                                            Case "publickey"
                                                PublicKey = BodyEntry.Value.ToString()
                                            Case "passphrase"
                                                PassPhrase = BodyEntry.Value.ToString()
                                            Case "injectpublickey"
                                                InjectPublicKey = BodyEntry.Value.ToString()
                                            Case "injectchainswaphash"
                                                InjectChainSwapHash = BodyEntry.Value.ToString()
                                            Case "chainswapkey"
                                                ChainSwapKey = BodyEntry.Value.ToString()
                                            Case "amountnqt"
                                                AmountNQT = BodyEntry.Value.ToString()
                                            Case "collateralnqt"
                                                CollateralNQT = BodyEntry.Value.ToString()
                                            Case "xamountnqt"
                                                XAmountNQT = BodyEntry.Value.ToString()
                                            Case "xitem"
                                                XItem = BodyEntry.Value.ToString()
                                        End Select

                                    Next

                                    Dim Masterkeys As List(Of String) = New List(Of String)
                                    If Not PassPhrase = "" Then
                                        Masterkeys = GetMasterKeys(PassPhrase)
                                        PublicKey = Masterkeys(0)
                                        SignHexKey = Masterkeys(1)
                                    End If

                                    Dim Amount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(If(Not IsNumeric(AmountNQT), "0", AmountNQT)))
                                    Dim Collateral As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(If(Not IsNumeric(CollateralNQT), "0", CollateralNQT)))
                                    Dim XAmount As Double = ClsSignumAPI.Planck2Dbl(Convert.ToUInt64(If(Not IsNumeric(XAmountNQT), "0", XAmountNQT)))

                                    Dim ReservedOrder As ClsDEXContract

                                    If PFPForm.C_DEXContractList.Count > 0 Then
                                        ReservedOrder = PFPForm.C_DEXContractList.FirstOrDefault(Function(reserved) reserved.Status = ClsDEXContract.E_Status.NEW_ And reserved.CreatorID = GetAccountID(PublicKey))
                                        'Nothing
                                    End If

                                    For Each T_DEXContract As ClsDEXContract In PFPForm.C_DEXContractList

                                        If OrderID.Trim() = "" Then

                                            If T_DEXContract.Status = ClsDEXContract.E_Status.FREE Or (T_DEXContract.Status = ClsDEXContract.E_Status.NEW_ And T_DEXContract.CreatorID = GetAccountID(PublicKey)) Then

                                                Dim T_Result As String = T_DEXContract.CreateSellOrder(PublicKey, Amount, Collateral, XItem, XAmount, 0.0, SignHexKey)

                                                If T_Result.Contains("errorCode") Then
                                                    ResponseHTML = T_Result.Substring(T_Result.IndexOf("->") + 2).Trim
                                                ElseIf T_Result.Contains(Application.ProductName + "-error") Then

                                                Else

                                                    If SignHexKey.Trim() = "" Then
                                                        '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createOrder"",""data"":{""contract"":""" + T_DEXContract.ID.ToString() + """,""unsignedTransactionBytes"":""" + T_Result + """}}"
                                                    Else
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createOrder"",""data"":{""transaction"":""" + T_Result + """}}"
                                                    End If

                                                    Exit For

                                                End If

                                            End If

                                        Else

                                            If T_DEXContract.ID.ToString() = OrderID And (T_DEXContract.Status = ClsDEXContract.E_Status.FREE Or (T_DEXContract.Status = ClsDEXContract.E_Status.NEW_ And T_DEXContract.CreatorID = GetAccountID(PublicKey))) Then
                                                'create new order
                                                Dim T_Result As String = T_DEXContract.CreateSellOrder(PublicKey, Amount, Collateral, XItem, XAmount, 0.0, SignHexKey)
                                                If T_Result.Contains("errorCode") Then
                                                    ResponseHTML = T_Result.Substring(T_Result.IndexOf("->") + 2).Trim
                                                ElseIf T_Result.Contains(Application.ProductName + "-error") Then

                                                Else

                                                    If SignHexKey.Trim() = "" Then
                                                        '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createOrder"",""data"":{""contract"":""" + T_DEXContract.ID.ToString() + """,""unsignedTransactionBytes"":""" + T_Result + """}}"
                                                    Else
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createOrder"",""data"":{""transaction"":""" + T_Result + """}}"
                                                    End If

                                                    Exit For

                                                End If
                                            ElseIf T_DEXContract.ID.ToString() = OrderID And T_DEXContract.Status = ClsDEXContract.E_Status.OPEN And InjectChainSwapHash.Trim() = "" And InjectPublicKey.Trim() = "" Then
                                                'accept order
                                                Dim T_Result As String = ""
                                                If T_DEXContract.IsSellOrder Then
                                                    T_Result = T_DEXContract.AcceptSellOrder(PublicKey, Collateral, 0.0, SignHexKey)
                                                Else
                                                    T_Result = T_DEXContract.AcceptBuyOrder(PublicKey, -1, Collateral, 0.0, SignHexKey)
                                                End If
                                                If T_Result.Contains("errorCode") Then
                                                    ResponseHTML = T_Result.Substring(T_Result.IndexOf("->") + 2).Trim
                                                ElseIf T_Result.Contains(Application.ProductName + "-error") Then

                                                Else

                                                    If SignHexKey.Trim() = "" Then
                                                        '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""acceptOrder"",""data"":{""contract"":""" + T_DEXContract.ID.ToString() + """,""unsignedTransactionBytes"":""" + T_Result + """}}"
                                                    Else
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""acceptOrder"",""data"":{""transaction"":""" + T_Result + """}}"
                                                    End If

                                                    Exit For

                                                End If
                                            ElseIf T_DEXContract.ID.ToString() = OrderID And T_DEXContract.Status = ClsDEXContract.E_Status.OPEN And T_DEXContract.IsSellOrder And T_DEXContract.CurrentInitiatorID = GetAccountID(PublicKey) And InjectChainSwapHash.Trim() = "" And MessageIsHEXString(InjectPublicKey) And InjectPublicKey.Length() = 64 Then
                                                'inject responder
                                                Dim T_Result As String = T_DEXContract.InjectResponder(PublicKey, GetAccountID(InjectPublicKey), 0.0, SignHexKey)
                                                If T_Result.Contains("errorCode") Then
                                                    ResponseHTML = T_Result.Substring(T_Result.IndexOf("->") + 2).Trim
                                                ElseIf T_Result.Contains(Application.ProductName + "-error") Then

                                                Else
                                                    If SignHexKey.Trim() = "" Then
                                                        '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""injectResponder"",""data"":{""contract"":""" + T_DEXContract.ID.ToString() + """,""unsignedTransactionBytes"":""" + T_Result + """}}"
                                                    Else
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""injectResponder"",""data"":{""transaction"":""" + T_Result + """}}"
                                                    End If

                                                    Exit For
                                                End If
                                            ElseIf T_DEXContract.ID.ToString() = OrderID And T_DEXContract.Status = ClsDEXContract.E_Status.OPEN And T_DEXContract.IsSellOrder And T_DEXContract.CurrentInitiatorID = GetAccountID(PublicKey) And InjectPublicKey.Trim() = "" And MessageIsHEXString(InjectChainSwapHash) And InjectChainSwapHash.Length() = 64 Then
                                                'inject chainswaphash
                                                Dim T_Result As String = T_DEXContract.InjectChainSwapHash(PublicKey, InjectChainSwapHash, 0.0, SignHexKey)
                                                If T_Result.Contains("errorCode") Then
                                                    ResponseHTML = T_Result.Substring(T_Result.IndexOf("->") + 2).Trim
                                                ElseIf T_Result.Contains(Application.ProductName + "-error") Then

                                                Else
                                                    If SignHexKey.Trim() = "" Then
                                                        '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""injectChainSwapHash"",""data"":{""contract"":""" + T_DEXContract.ID.ToString() + """,""unsignedTransactionBytes"":""" + T_Result + """}}"
                                                    Else
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""injectChainSwapHash"",""data"":{""transaction"":""" + T_Result + """}}"
                                                    End If

                                                    Exit For
                                                End If

                                            ElseIf T_DEXContract.ID.ToString() = OrderID And T_DEXContract.Status = ClsDEXContract.E_Status.RESERVED And (T_DEXContract.CurrentInitiatorID = GetAccountID(PublicKey) Or T_DEXContract.CurrentResponderID = GetAccountID(PublicKey)) Then
                                                'finish order
                                                Dim T_Result As String = ""
                                                Dim ResponseText As String = ""
                                                If ChainSwapKey.Trim() = "" Then
                                                    ResponseText = "finishOrder"
                                                    T_Result = T_DEXContract.FinishOrder(PublicKey, 0.0, SignHexKey)
                                                Else
                                                    ResponseText = "finishOrderWithChainSwapKey"
                                                    If MessageIsHEXString(ChainSwapKey) Then
                                                        T_Result = T_DEXContract.FinishOrderWithChainSwapKey(PublicKey, ChainSwapKey, 0.0, SignHexKey)
                                                    End If
                                                End If

                                                If T_Result.Contains("errorCode") Then
                                                    ResponseHTML = T_Result.Substring(T_Result.IndexOf("->") + 2).Trim
                                                ElseIf T_Result.Contains(Application.ProductName + "-error") Then

                                                Else
                                                    If SignHexKey.Trim() = "" Then
                                                        '"{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""" + ResponseText + """,""data"":{""contract"":""" + T_DEXContract.ID.ToString() + """,""unsignedTransactionBytes"":""" + T_Result + """}}"
                                                    Else
                                                        ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""" + ResponseText + """,""data"":{""transaction"":""" + T_Result + """}}"
                                                    End If

                                                    Exit For
                                                End If

                                            End If

                                        End If

                                    Next

                                End If

                            Case ClsAPIRequest.E_Endpoint.Broadcast

                                Dim Token As String = ""
                                Dim SignedTransactionBytes As String = ""

                                If APIRequest.Parameters.Count > 0 Then
                                    'use Parameters
                                    Token = APIRequest.Parameters.FirstOrDefault(Function(OID) OID.Parameter = ClsAPIRequest.E_Parameter.Token).Value
                                    SignedTransactionBytes = APIRequest.Parameters.FirstOrDefault(Function(OID) OID.Parameter = ClsAPIRequest.E_Parameter.SignedTransactionBytes).Value

                                    If IsNothing(Token) Then Token = ""
                                    If IsNothing(SignedTransactionBytes) Then SignedTransactionBytes = ""
                                Else
                                    'check Body

                                    For Each BodyEntry As KeyValuePair(Of String, Object) In APIRequest.Body

                                        Select Case BodyEntry.Key.ToLower()
                                            Case "token"
                                                Token = BodyEntry.Value.ToString()
                                            Case "signedtransactionbytes"
                                                SignedTransactionBytes = BodyEntry.Value.ToString()
                                        End Select
                                    Next

                                End If

                                If Not Token = "" And Not SignedTransactionBytes = "" And MessageIsHEXString(SignedTransactionBytes) Then

                                    If SupportedCurrencies.Contains(Token) Then

                                        If ClsDEXContract.CurrencyIsCrypto(Token) Then
                                            Dim XItem As AbsClsXItem = ClsXItemAdapter.NewXItem(Token)
                                            Dim T_Result As String = XItem.BroadcastTransaction(SignedTransactionBytes)
                                            If Not IsErrorOrWarning(T_Result,,, False) Then
                                                ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""broadcast"",""data"":{""message"":""" + T_Result + """}}"
                                            End If
                                        End If

                                    ElseIf Token = "Signum" Then

                                        Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI("")
                                        Dim T_Result As String = SignumAPI.BroadcastTransaction(SignedTransactionBytes)
                                        If Not IsErrorOrWarning(T_Result,,, False) Then
                                            ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""broadcast"",""data"":{""message"":""" + T_Result + """}}"
                                        End If

                                    End If

                                End If

                            Case ClsAPIRequest.E_Endpoint.SmartContract

                                Dim PrivateKey As String = ""
                                Dim PublicKey As String = ""

                                If APIRequest.Parameters.Count > 0 Then

                                    PrivateKey = APIRequest.Parameters.FirstOrDefault(Function(OID) OID.Parameter = ClsAPIRequest.E_Parameter.PrivateKey).Value
                                    PublicKey = APIRequest.Parameters.FirstOrDefault(Function(OID) OID.Parameter = ClsAPIRequest.E_Parameter.PublicKey).Value

                                    If IsNothing(PrivateKey) Then PrivateKey = ""
                                    If IsNothing(PublicKey) Then PublicKey = ""

                                Else
                                    'check Body

                                    For Each BodyEntry As KeyValuePair(Of String, Object) In APIRequest.Body

                                        Select Case BodyEntry.Key.ToLower()
                                            Case "privatekey"
                                                PrivateKey = BodyEntry.Value.ToString()
                                            Case "publickey"
                                                PublicKey = BodyEntry.Value.ToString()
                                        End Select
                                    Next

                                End If

                                If Not PrivateKey.Trim() = "" Then
                                    PublicKey = PrivKeyToPubKey(PrivateKey)
                                End If

                                If Not PublicKey.Trim() = "" Then

                                    Dim SignumAPI As ClsSignumAPI = New ClsSignumAPI("")

                                    Dim T_Result As String = SignumAPI.CreateSmartContract(PublicKey, ClsSignumSmartContract.CreationMachineData)
                                    If Not IsErrorOrWarning(T_Result,,, False) Then

                                        Dim UnsignedTransactionBytes As String = ""

                                        If T_Result.Contains("{") Then
                                            Dim Converter As ClsJSONAndXMLConverter = New ClsJSONAndXMLConverter(T_Result, ClsJSONAndXMLConverter.E_ParseType.JSON)
                                            UnsignedTransactionBytes = Converter.FirstValue("unsignedTransactionBytes").ToString()
                                        End If

                                        If Not PrivateKey.Trim() = "" Then
                                            T_Result = SignumAPI.BroadcastTransaction(T_Result)
                                            If Not IsErrorOrWarning(T_Result,,, False) Then
                                                ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createSmartContract"",""data"":{""transaction"":""" + T_Result + """}}"
                                            End If
                                        Else
                                            ResponseHTML = "{""application"":""PFPDEX"",""interface"":""API"",""version"":""1.0"",""contentType"":""application/json"",""response"":""createSmartContract"",""data"":{""unsignedTransactionBytes"":""" + UnsignedTransactionBytes + """}}"
                                        End If

                                    End If

                                End If

                        End Select

                    Else
                        ResponseHTML = "{""response"":""error wrong request""}"
                    End If

                    SendHTMLResponse(TCPClient, ResponseHTML)

                End If

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