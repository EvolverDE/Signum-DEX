
Option Strict On
Option Explicit On

' author evolver
' DEXNET Class
' Version 1.0

Public Class ClsDEXNET

    Property DEXNET_PublicKeyHEX As String = ""
    Property DEXNET_PrivateKeyHEX As String = ""
    Property DEXNET_AgreeKeyHEX As String = ""


    Property StatusList As List(Of String) = New List(Of String)
    Property Peers As List(Of S_Peer) = New List(Of S_Peer)
    Property MaxPeers As Integer = 500

    Property BroadcastBuffer As List(Of String) = New List(Of String)
    Property LastBroadcastedMsgList As List(Of String) = New List(Of String)
    Property RelevantKeys As List(Of String) = New List(Of String)
    Property RelevantMsgs As List(Of S_RelevantMessage) = New List(Of S_RelevantMessage)

    Property DEXNET_ServerPort As Integer = 8131
    Property TCPListener As Net.Sockets.TcpListener
    Property ListenerThread As Threading.Thread

    Property StreamWriter As IO.StreamWriter
    Property StreamReader As IO.StreamReader
    Property MessageThread As Threading.Thread
    Property ShowStatus As Boolean = False
    Property DEXNETClose As Boolean = False


    Structure S_Peer
        Dim TCPClient As Net.Sockets.TcpClient
        Dim Port As Integer
        Dim PossibleHost As String
        Dim PossibleRemoteServerPort As Integer
        Dim StreamWriter As IO.StreamWriter
        Dim StreamReader As IO.StreamReader
        Dim PublicKey As String
        Dim MessageThread As Threading.Thread
        Dim LastGetMsg As String
        Dim LastSetMsg As String
        Dim Timeout As Integer
        'Dim GetMsg As String

        Dim GetMsgs As List(Of String)
        Dim SetMsgs As List(Of String)

        Sub New(ByVal Clear As Boolean)
            Port = -1
            PossibleHost = ""
            PossibleRemoteServerPort = -1
            PublicKey = ""
            LastGetMsg = ""
            LastSetMsg = ""
            Timeout = -1
            'GetMsg = ""

            GetMsgs = New List(Of String)
            SetMsgs = New List(Of String)
        End Sub

    End Structure

    Private Structure S_Process
        Dim GetPossibleHost As String
        Dim GetIP As String
        Dim GetPort As Integer
        Dim PossibleRemoteServerPort As Integer
        Dim GetMsg As String
        Dim SetMsg As String
        Dim PublicKey As String

        Dim GetMsgs As List(Of String)

        Sub New(ByVal Clear As Boolean)
            GetPossibleHost = ""
            GetIP = ""
            GetPort = -1
            PossibleRemoteServerPort = -1
            GetMsg = ""
            SetMsg = ""
            PublicKey = ""

            GetMsgs = New List(Of String)
        End Sub

    End Structure

    Structure S_RelevantMessage
        Dim RelevantKey As String
        Dim RelevantMessage As String
        Dim Timestamp As Double
        Dim Lifetime As Double
    End Structure

    Enum E_XMLTags
        Request = 0
        Response = 1
        Timestamp = 2
        Message = 3
        PublicKey = 4
        SignatureHash = 5
    End Enum

    Public Sub New(Optional ByVal PPort As Integer = 8131, Optional ByVal PShowStatus As Boolean = False)

        Dim MasterKeys()() As Byte = Keygen(HEXStringToByteArray(GetID())) '{PublicKey, SignKey, KeyHash} 'KeyHash = Agreementkey

        DEXNET_PublicKeyHEX = ByteArrayToHEXString(MasterKeys(0))
        DEXNET_PrivateKeyHEX = ByteArrayToHEXString(MasterKeys(1))
        DEXNET_AgreeKeyHEX = ByteArrayToHEXString(MasterKeys(2))

        DEXNET_ServerPort = PPort
        ShowStatus = PShowStatus

        StartServer()

    End Sub

#Region "Basics"
    Public Sub StartServer(Optional ByVal PPort As Integer = 8131)

        If GetINISetting(E_Setting.InfoOut, False) Then
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.Info2File(Application.ProductName + "-info from DEXNET->StartServer()")
        End If

        DEXNET_ServerPort = PPort
        DEXNETClose = False

        If IsNothing(TCPListener) Then
            TCPListener = New Net.Sockets.TcpListener(Net.IPAddress.Any, DEXNET_ServerPort)

            ListenerThread = New Threading.Thread(AddressOf Listen)
            ListenerThread.Start()

            If ShowStatus Then
                StatusList.Add("server started at Port:" + DEXNET_ServerPort.ToString)
            End If
        Else
            If ShowStatus Then
                StatusList.Add("server already started at Port:" + DEXNET_ServerPort.ToString)
            End If
        End If

        StartRefresher()

    End Sub

    Dim TempConnect As String = ""
    Public Sub Connect(ByVal HostIP As String, ByVal RemotePort As Integer, Optional ByVal MyHost As String = "")

        Dim PeerOK As Boolean = True

        If TempConnect.Contains(HostIP + ":" + RemotePort.ToString + ";") Then
            Exit Sub
        End If

        Dim TempConnList As List(Of String) = New List(Of String)

        If TempConnect.Contains(";") Then
            TempConnList.AddRange(TempConnect.Split(";"c))
        End If

        For i As Integer = 0 To TempConnList.Count - 1

            Dim A_Peer As String = TempConnList(i)

            If A_Peer.Contains(":") Then
                A_Peer = A_Peer.Remove(A_Peer.IndexOf(":"))
            End If

            If Not CheckHostIsNotIP(HostIP, A_Peer) Then
                PeerOK = False
                Exit Sub
            End If
        Next

        If PeerOK Then
            For i As Integer = 0 To Peers.Count - 1
                Dim A_Peer As S_Peer = Peers(i)

                If Not CheckHostIsNotIP(HostIP, A_Peer.PossibleHost) Then
                    PeerOK = False
                    Exit Sub
                End If
            Next
        End If


        If PeerOK Then
            If Peers.Count >= MaxPeers Then
                If ShowStatus Then
                    StatusList.Add("Connect(): MaxPeers reached: " + MaxPeers.ToString)
                End If
            Else
                TempConnect += HostIP + ":" + RemotePort.ToString + ";"
                Dim ConThread As Threading.Thread = New Threading.Thread(AddressOf ConnectThread)
                ConThread.Start(New List(Of Object)({HostIP, RemotePort, MyHost}))
            End If
        End If

    End Sub

    Private Sub ConnectThread(ByVal Input As Object)

        Dim T_List As List(Of Object) = DirectCast(Input, List(Of Object))


        Dim RemoteHostIP As String = TryCast(T_List(0), String)
        Dim RemotePort As Integer = DirectCast(T_List(1), Integer)
        Dim MyHost As String = TryCast(T_List(2), String)

        If RemotePort < 0 Then
            Exit Sub
        End If

        Try

            Dim T_Peer As S_Peer = New S_Peer(True)
            Dim T_TCPClient As Net.Sockets.TcpClient = New Net.Sockets.TcpClient(RemoteHostIP, RemotePort)

            If T_TCPClient.Connected Then

                T_Peer.TCPClient = T_TCPClient

                Dim IPEP As Net.IPEndPoint = DirectCast(T_TCPClient.Client.RemoteEndPoint, Net.IPEndPoint)
                T_Peer.PossibleHost = RemoteHostIP
                T_Peer.PossibleRemoteServerPort = RemotePort
                T_Peer.Port = RemotePort
                Dim NStream As Net.Sockets.NetworkStream = T_TCPClient.GetStream

                T_Peer.StreamWriter = New IO.StreamWriter(NStream)
                T_Peer.StreamReader = New IO.StreamReader(NStream)

                T_Peer.PublicKey = ""

                Dim NewPeerRequest As String = CreateXMLMessage("Broadcast", E_XMLTags.Request)
                NewPeerRequest += CreateXMLMessage(GetUnixTimestamp(), E_XMLTags.Timestamp)
                NewPeerRequest += "<" + E_XMLTags.Message.ToString + ">"
                NewPeerRequest += CreateXMLMessage(MyHost, "NewPeer")
                NewPeerRequest += CreateXMLMessage(DEXNET_ServerPort.ToString, "PSPort")
                NewPeerRequest += CreateXMLMessage(DEXNET_PublicKeyHEX, "PPublicKey")
                NewPeerRequest += "</" + E_XMLTags.Message.ToString + ">"

                NewPeerRequest += CreateXMLMessage(DEXNET_PublicKeyHEX, E_XMLTags.PublicKey)
                NewPeerRequest = CreateSignatureHash(NewPeerRequest)

                T_Peer.SetMsgs.Add(NewPeerRequest)
                T_Peer.MessageThread = New Threading.Thread(AddressOf ReceiveMessage)

                Peers.Add(T_Peer)
                Peers(Peers.Count - 1).MessageThread.Start(Convert.ToInt32(Peers.Count - 1))

                TempConnect = TempConnect.Replace(RemoteHostIP + ":" + RemotePort.ToString + ";", "")

                Dim DEXNETNodesString As String = GetINISetting(E_Setting.DEXNETNodes, "signum.zone:8131")
                If Not DEXNETNodesString.Contains(RemoteHostIP + ":" + RemotePort.ToString) Then
                    DEXNETNodesString += ";" + RemoteHostIP + ":" + RemotePort.ToString
                    SetINISetting(E_Setting.DEXNETNodes, DEXNETNodesString)
                End If

            End If

        Catch ex As Exception
            If ShowStatus Then
                StatusList.Add("ConnectThread(HostIP:" + RemoteHostIP + " RemotePort:" + RemotePort.ToString + " MyHost:" + MyHost + "): " + ex.Message)
            End If
        End Try

    End Sub

    Public Sub BroadcastMessage(ByVal Message As String, Optional ByVal SenderPrivateKeyHEX As String = "", Optional ByVal SenderAgreementKeyHex As String = "", Optional ByVal SenderPublicKeyHEX As String = "", Optional ByVal RecipientPublicKeyHex As String = "")

        If SenderPublicKeyHEX.Trim = "" Or SenderPrivateKeyHEX.Trim = "" Then
            SenderPublicKeyHEX = DEXNET_PublicKeyHEX
            SenderPrivateKeyHEX = ""
        End If

        If Not SenderAgreementKeyHex.Trim = "" And Not RecipientPublicKeyHex.Trim = "" Then
            Dim Curve As ClsCurve25519 = New ClsCurve25519
            Dim SharedKeyBytes As Byte() = New Byte(31) {}
            Curve.GetSharedSecret(SharedKeyBytes, HEXStringToByteArray(SenderAgreementKeyHex), HEXStringToByteArray(RecipientPublicKeyHex))
            Dim SharedKeyHEXString As String = ByteArrayToHEXString(SharedKeyBytes)
            Message = AESEncrypt2HEXStr(Message, SharedKeyHEXString)
        End If

        Dim NewBroadcastRequest As String = CreateXMLMessage("Broadcast", E_XMLTags.Request)
        NewBroadcastRequest += CreateXMLMessage(GetUnixTimestamp(), E_XMLTags.Timestamp)
        NewBroadcastRequest += CreateXMLMessage(Message, E_XMLTags.Message)
        NewBroadcastRequest += CreateXMLMessage(SenderPublicKeyHEX, E_XMLTags.PublicKey)

        NewBroadcastRequest = CreateSignatureHash(NewBroadcastRequest, SenderPrivateKeyHEX)

        SendRequest(NewBroadcastRequest)

    End Sub

    Private Sub SendRequest(ByVal Request As String)

        Dim PeerCNT As Integer = Peers.Count
        For i As Integer = 0 To PeerCNT - 1
            Dim T_Peer As S_Peer = Peers(i)
            T_Peer.SetMsgs.Add(Request)
            Peers(i) = T_Peer
        Next

    End Sub

    Public Sub StopServer()

        If GetINISetting(E_Setting.InfoOut, False) Then
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.Info2File(Application.ProductName + "-info from DEXNET->StopServer()")
        End If

        DEXNETClose = True

        If Not IsNothing(TCPListener) Then
            TCPListener.Server.Close()
            TCPListener = Nothing
        End If

        For Each x As S_Peer In Peers
            x.TCPClient.Close()
        Next

        Peers.Clear()

    End Sub
#End Region

#Region "Advanced"
    Public Function GetStatusMessages() As List(Of String)

        Dim T_MsgsList As List(Of String) = New List(Of String)

        Try
            Dim IDXList As List(Of Integer) = New List(Of Integer)
            Dim SLCNT As Integer = StatusList.Count

            For IDX As Integer = 0 To SLCNT - 1
                Dim Status As String = StatusList(IDX)
                IDXList.Add(IDX)

                T_MsgsList.Add(Status)
            Next

            IDXList.Reverse()

            For Each i In IDXList
                StatusList.RemoveAt(i)
            Next

        Catch ex As Exception

        End Try

        Return T_MsgsList

    End Function
    Public Function GetBroadcastedMsgs() As List(Of String)
        Dim T_MsgsList As List(Of String) = New List(Of String)

        ClearBroadcastBuffer()

        Dim BCCNT As Integer = LastBroadcastedMsgList.Count
        For i As Integer = 0 To BCCNT - 1
            Dim LastBroadcastMsg As String = LastBroadcastedMsgList(i)

            Dim Founded As Boolean = False
            For Each BroadcastBufferMsg As String In BroadcastBuffer

                If BroadcastBufferMsg = LastBroadcastMsg Then
                    Founded = True
                    Exit For
                End If

            Next

            If Not Founded Then
                T_MsgsList.Add(LastBroadcastMsg)
                BroadcastBuffer.Add(LastBroadcastMsg)
            End If

        Next

        Return T_MsgsList

    End Function
    Function ClearBroadcastBuffer() As Boolean

        Dim DeadFounded As Boolean = True
        While DeadFounded

            DeadFounded = False

            Dim DelIDX As Integer = -1
            For ii As Integer = 0 To BroadcastBuffer.Count - 1
                Dim BroadcastBufferMsg As String = BroadcastBuffer(ii)

                Dim Exists As Boolean = False
                Dim BCCNT As Integer = LastBroadcastedMsgList.Count
                For i As Integer = 0 To BCCNT - 1
                    Dim LastBroadcastMsg As String = LastBroadcastedMsgList(i)

                    If BroadcastBufferMsg = LastBroadcastMsg Then
                        Exists = True
                        Exit For
                    End If

                Next

                If Not Exists Then
                    DeadFounded = True
                    DelIDX = ii
                    Exit For
                End If

            Next

            If DeadFounded And DelIDX <> -1 Then
                BroadcastBuffer.RemoveAt(DelIDX)
            End If

        End While

        Return True

    End Function

    Public Function GetRelevantMsgs() As List(Of S_RelevantMessage)
        Return New List(Of S_RelevantMessage)(RelevantMsgs.ToArray)
    End Function
    Public Function GetPeers() As List(Of S_Peer)
        Return New List(Of S_Peer)(Peers.ToArray)
    End Function
    Public Sub GetClients()
        SendRequest("<" + E_XMLTags.Request.ToString + ">GetClients</" + E_XMLTags.Request.ToString + "><" + E_XMLTags.Timestamp.ToString + ">" + GetUnixTimestamp() + "</" + E_XMLTags.Timestamp.ToString + "><" + E_XMLTags.PublicKey.ToString + ">" + DEXNET_PublicKeyHEX + "</" + E_XMLTags.PublicKey.ToString + ">")
    End Sub

    Public Sub GetPing()

        For i As Integer = 0 To Peers.Count - 1
            Dim Peer As S_Peer = Peers(i)
            Peer.Timeout = 1
            Peers(i) = Peer
        Next

        SendRequest("<" + E_XMLTags.Request.ToString + ">Ping</" + E_XMLTags.Request.ToString + "><" + E_XMLTags.Timestamp.ToString + ">" + GetUnixTimestamp() + "</" + E_XMLTags.Timestamp.ToString + "><" + E_XMLTags.PublicKey.ToString + ">" + DEXNET_PublicKeyHEX + "</" + E_XMLTags.PublicKey.ToString + ">")
    End Sub

#End Region

#Region "Internals"
    Private Sub Listen()

        While Not DEXNETClose

            Try

                TCPListener.Start()

                Dim T_TCPClient As Net.Sockets.TcpClient = TCPListener.AcceptTcpClient()

                If T_TCPClient.Connected Then

                    Dim T_Peer As S_Peer = New S_Peer(True)

                    T_Peer.TCPClient = T_TCPClient

                    Dim IPEP As Net.IPEndPoint = DirectCast(T_TCPClient.Client.RemoteEndPoint, Net.IPEndPoint)

                    T_Peer.PossibleHost = IPEP.Address.ToString
                    T_Peer.Port = IPEP.Port
                    T_Peer.PossibleRemoteServerPort = -1

                    Dim NStream As Net.Sockets.NetworkStream = T_TCPClient.GetStream

                    T_Peer.StreamWriter = New IO.StreamWriter(NStream)
                    T_Peer.StreamReader = New IO.StreamReader(NStream)

                    T_Peer.PublicKey = ""

                    T_Peer.MessageThread = New Threading.Thread(AddressOf ReceiveMessage)
                    Peers.Add(T_Peer)
                    Peers(Peers.Count - 1).MessageThread.Start(Peers.Count - 1)

                    If ShowStatus Then
                        StatusList.Add("(" + Now.ToShortDateString + " " + Now.ToLongTimeString + ") " + T_Peer.PossibleHost + ":" + T_Peer.Port.ToString + " connected.")
                    End If

                End If

            Catch ex As Exception

                If GetINISetting(E_Setting.InfoOut, False) Then
                    Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                    Out.ErrorLog2File(Application.ProductName + "-error in DEXNET->Listen(): -> " + ex.Message)
                End If

                If ShowStatus Then
                    StatusList.Add("(" + Now.ToShortDateString + " " + Now.ToLongTimeString + ") Listen(Port:" + DEXNET_ServerPort.ToString + "): " + ex.Message)
                End If

                If Not DEXNETClose Then
                    DEXNET_ServerPort += 1

                    TCPListener = New Net.Sockets.TcpListener(Net.IPAddress.Any, DEXNET_ServerPort)

                    If ShowStatus Then
                        StatusList.Add("server restarted at Port:" + DEXNET_ServerPort.ToString)
                    End If

                End If

            End Try

        End While

        If GetINISetting(E_Setting.InfoOut, False) Then
            Dim Out As ClsOut = New ClsOut(Application.StartupPath)
            Out.Info2File(Application.ProductName + "-info from DEXNET->Listen(): -> End ")
        End If

        'end: disconnect all clients

        For i As Integer = 0 To Peers.Count - 1
            Dim T_Peer As S_Peer = Peers(i)

            Try
                T_Peer.TCPClient.Close()
            Catch ex As Exception
                If ShowStatus Then
                    StatusList.Add("TCPServ(Exit): " + ex.Message)
                End If
            End Try

        Next

    End Sub
    Private Sub ReceiveMessage(ByVal Input As Object)

        Dim IDX As Integer = DirectCast(Input, Integer)

        While Not DEXNETClose And Peers.Count - 1 >= IDX

            Try
                Dim T_Peer As S_Peer = Peers(IDX)
                If Not IsNothing(T_Peer.TCPClient) Then

                    Dim NetStream As Net.Sockets.NetworkStream = T_Peer.TCPClient.GetStream
                    If NetStream.CanRead Then

                        Dim TempMsg As String = T_Peer.StreamReader.ReadLine()

                        If Not IsNothing(TempMsg) Then
                            T_Peer = Peers(IDX)
                            T_Peer.GetMsgs.Add(TempMsg)
                            Peers(IDX) = T_Peer
                        Else
                            T_Peer.TCPClient.Close()
                            Exit While
                        End If

                        If ShowStatus Then
                            Dim LogMsg As String = "(" + Now.ToShortDateString + " " + Now.ToLongTimeString + ") " + T_Peer.PossibleHost + ":" + T_Peer.Port.ToString + " received: " + TempMsg
                            StatusList.Add(LogMsg)
                        End If

                    End If
                Else
                    Exit While
                End If

            Catch ex As Exception

                If ShowStatus Then
                    StatusList.Add("ReceiveMessage(): " + ex.Message)
                End If

            End Try

        End While

    End Sub

    Private Sub StartRefresher()
        ListenerThread = New Threading.Thread(AddressOf Refresher)
        ListenerThread.Start()
    End Sub
    Private Sub Refresher()

        While Not DEXNETClose

            Threading.Thread.Sleep(1)

            Application.DoEvents()
            Dim Wait As Boolean = ClearList()
            Wait = ProcessMessages()
            Wait = SendMessages()

            Wait = ClearRelevantMsgs()
            Wait = FilterRelevantMsgs()

            If StatusList.Count > 100 Then
                StatusList.RemoveAt(0)
            End If
        End While

    End Sub
    Private Function ClearList() As Boolean
        Dim DeadFound As Boolean = True

        While DeadFound
            DeadFound = False

            Try

                Dim PeerIDX As Integer = Peers.Count

                If PeerIDX > MaxPeers Then
                    Dim T_Peer As S_Peer = Peers(Peers.Count - 1)
                    T_Peer.TCPClient.Close()
                    T_Peer.TCPClient = Nothing
                End If

                PeerIDX = Peers.Count

                For i As Integer = 0 To PeerIDX - 1
                    Dim T_Peer As S_Peer = Peers(i)
                    If Not T_Peer.TCPClient.Connected Then
                        DeadFound = True
                        T_Peer.TCPClient.Close()
                        Peers.Remove(T_Peer)

                        If ShowStatus Then
                            StatusList.Add("(" + Now.ToShortDateString + " " + Now.ToLongTimeString + ") " + T_Peer.PossibleHost + ":" + T_Peer.Port.ToString + " disconnected.")
                        End If

                        Exit For
                    End If

                Next

            Catch ex As Exception
                If ShowStatus Then
                    StatusList.Add("ClearList(Peers): " + ex.Message)
                End If
            End Try

        End While


        'Dim PubKeyList As List(Of String) = New List(Of String)

        'Dim WExit As Boolean = False
        'While Not WExit
        '    WExit = True

        '    For i As Integer = 0 To Peers.Count - 1
        '        Dim T_Peer As S_Peer = Peers(i)

        '        Dim FoundPubKey As Boolean = False
        '        For Each PubKey As String In PubKeyList

        '            If T_Peer.PublicKey = PubKey And T_Peer.PublicKey <> DEXNET_PublicKeyHEX Then
        '                FoundPubKey = True
        '                Exit For
        '            End If

        '        Next

        '        If FoundPubKey Then
        '            WExit = False
        '            T_Peer.TCPClient.Close()
        '            Peers.Remove(T_Peer)
        '            Exit For
        '        Else
        '            If DEXNET_PublicKeyHEX <> T_Peer.PublicKey And T_Peer.PublicKey.Trim <> "" Then
        '                PubKeyList.Add(T_Peer.PublicKey)
        '            End If
        '        End If

        '    Next

        'End While



        Dim DelIdx As Integer = -1
        If LastBroadcastedMsgList.Count > 1000 Then

            For i As Integer = 0 To LastBroadcastedMsgList.Count - 1

                Dim BroadcastedMsg As String = LastBroadcastedMsgList(i)
                Dim UnixTimestamp As String = GetXMLMessage(BroadcastedMsg, E_XMLTags.Timestamp)
                Dim DiffTimestamp As Double = Double.Parse(GetUnixTimestamp()) - Double.Parse(UnixTimestamp)
                If DiffTimestamp > 3600 Then
                    DelIdx = i
                    Exit For
                End If

            Next

        End If

        If DelIdx <> -1 Then
            LastBroadcastedMsgList.RemoveRange(DelIdx, LastBroadcastedMsgList.Count - DelIdx)
        End If

        Return True

    End Function

    Private Function ProcessMessages() As Boolean

        Try

            Dim ProcList As List(Of S_Process) = New List(Of S_Process)

            For i As Integer = 0 To Peers.Count - 1
                Dim T_Peer As S_Peer = Peers(i)

                If T_Peer.GetMsgs.Count > 0 Then
                    Dim TempMsg As String = T_Peer.GetMsgs(T_Peer.GetMsgs.Count - 1)
                    T_Peer.LastGetMsg = TempMsg
                    Dim TempMsgs As List(Of String) = New List(Of String)(T_Peer.GetMsgs.ToArray)
                    T_Peer.GetMsgs.Clear()
                    Peers(i) = T_Peer


                    Dim T_Process As S_Process = New S_Process(True)

                    T_Process.GetPossibleHost = T_Peer.PossibleHost
                    T_Process.PossibleRemoteServerPort = T_Peer.PossibleRemoteServerPort
                    T_Process.GetIP = T_Peer.TCPClient.Client.RemoteEndPoint.ToString

                    Dim IPEP As Net.IPEndPoint = DirectCast(T_Peer.TCPClient.Client.RemoteEndPoint, Net.IPEndPoint)
                    T_Process.GetPort = IPEP.Port

                    T_Process.GetMsgs = TempMsgs
                    T_Process.PublicKey = T_Peer.PublicKey

                    ProcList.Add(T_Process)

                End If

                'If T_Peer.GetMsg.Trim <> "" Then
                '    Dim TempMsg As String = T_Peer.GetMsg
                '    T_Peer.LastGetMsg = TempMsg
                '    T_Peer.GetMsg = ""
                '    Peers(i) = T_Peer


                '    Dim T_Process As S_Process = New S_Process(True)

                '    T_Process.GetPossibleHost = T_Peer.PossibleHost
                '    T_Process.PossibleRemoteServerPort = T_Peer.PossibleRemoteServerPort
                '    T_Process.GetIP = T_Peer.TCPClient.Client.RemoteEndPoint.ToString


                '    Dim IPEP As Net.IPEndPoint = T_Peer.TCPClient.Client.RemoteEndPoint
                '    T_Process.GetPort = IPEP.Port

                '    T_Process.GetMsg = TempMsg
                '    T_Process.PublicKey = T_Peer.PublicKey

                '    ProcList.Add(T_Process)

                'End If

            Next


            For Each T_Process As S_Process In ProcList

                For T_GetMsgsIDX As Integer = 0 To T_Process.GetMsgs.Count - 1

                    Dim T_GetMsg As String = T_Process.GetMsgs(T_GetMsgsIDX)

                    Dim Request As String = GetXMLMessage(T_GetMsg, E_XMLTags.Request)
                    Dim Response As String = GetXMLMessage(T_GetMsg, E_XMLTags.Response)
                    Dim T_PubKey As String = GetXMLMessage(T_GetMsg, E_XMLTags.PublicKey)
                    Dim T_SigHash As String = GetXMLMessage(T_GetMsg, E_XMLTags.SignatureHash)

                    If Not T_PubKey.Trim = "" Then

                        If Not Request.Trim = "" Then

                            Select Case Request
                                Case "GetClients"

                                    Dim Answer As String = "<" + E_XMLTags.Response.ToString + ">GetClients</" + E_XMLTags.Response.ToString + "><" + E_XMLTags.Message.ToString + "><Entrys>" '<EntryCount>" + Peers.Count.ToString + "</EntryCount>

                                    Dim CNT As Integer = 0
                                    For i As Integer = 0 To Peers.Count - 1
                                        Dim T_Peer As S_Peer = Peers(i)

                                        If T_Peer.PossibleHost.Contains("127.") Or T_Peer.PossibleHost.Contains("192.168.") Or T_Peer.PossibleRemoteServerPort = -1 Or (Answer.Contains("<HostIP>" + T_Peer.PossibleHost + "</HostIP>") And Answer.Contains("<PeersPort>" + T_Peer.PossibleRemoteServerPort.ToString + "</PeersPort>")) Then
                                        Else
                                            Answer += "<Entry" + CNT.ToString + "><HostIP>" + T_Peer.PossibleHost + "</HostIP><PeersPort>" + T_Peer.PossibleRemoteServerPort.ToString + "</PeersPort><PeersPublicKey>" + T_Peer.PublicKey + "</PeersPublicKey></Entry" + CNT.ToString + ">"
                                            CNT += 1
                                        End If

                                    Next

                                    Answer += "</Entrys></" + E_XMLTags.Message.ToString + "><" + E_XMLTags.PublicKey.ToString + ">" + DEXNET_PublicKeyHEX + "</" + E_XMLTags.PublicKey.ToString + ">"

                                    T_Process.SetMsg = CreateSignatureHash(Answer)

                                    For i As Integer = 0 To Peers.Count - 1
                                        Dim T_Peer As S_Peer = Peers(i)

                                        If T_Peer.PossibleHost = T_Process.GetPossibleHost And T_Peer.Port = T_Process.GetPort Then
                                            T_Peer.SetMsgs.Add(T_Process.SetMsg)
                                            T_Peer.PublicKey = T_Process.PublicKey
                                            Peers(i) = T_Peer
                                            Exit For
                                        End If

                                    Next

                                Case "Broadcast"

                                    If DEXNET_PublicKeyHEX <> T_PubKey And Not T_PubKey.Trim = "" Then

                                        If CheckSignatureHash(T_GetMsg, T_PubKey, T_SigHash) = True Then

                                            T_Process.PublicKey = T_PubKey

                                            Dim BroadcastOK As Boolean = True
                                            Dim T_Message As String = GetXMLMessage(T_GetMsg, E_XMLTags.Message)
                                            If T_Message.Contains("<NewPeer></NewPeer>") Then

                                                Dim T_NuMessage = T_Message.Replace("<NewPeer></NewPeer>", "<NewPeer>" + T_Process.GetIP + "</NewPeer>")
                                                If CheckXMLTag(T_Message, "PSPort") Then
                                                    T_Process.PossibleRemoteServerPort = GetIntegerBetween(T_Message, "<PSPort>", "</PSPort>")
                                                End If

                                                T_GetMsg = T_GetMsg.Replace(T_Message, T_NuMessage)
                                                T_GetMsg = T_GetMsg.Replace(T_PubKey, DEXNET_PublicKeyHEX)

                                                T_GetMsg = CreateSignatureHash(T_GetMsg)

                                            ElseIf CheckXMLTag(T_Message, "NewPeer") Then

                                                Dim T_HostIP As String = GetStringBetween(T_Message, "<NewPeer>", "</NewPeer>")
                                                If T_HostIP.Contains(":") Then
                                                    T_HostIP = T_HostIP.Remove(T_HostIP.IndexOf(":"))
                                                End If

                                                Dim T_Port As Integer = GetIntegerBetween(T_Message, "<PSPort>", "</PSPort>")
                                                Dim T_PPublicKey As String = GetStringBetween(T_Message, "<PPublicKey>", "</PPublicKey>")

                                                Dim T_GetIP As String = T_Process.GetIP
                                                If T_GetIP.Contains(":") Then
                                                    T_GetIP = T_GetIP.Remove(T_GetIP.IndexOf(":"))
                                                End If

                                                If CheckHostIsNotIP(T_HostIP, T_GetIP) Then

                                                    Dim Founded As Boolean = False
                                                    For i As Integer = 0 To Peers.Count - 1
                                                        Dim T_Peer As S_Peer = Peers(i)

                                                        If (T_Peer.PossibleHost = T_HostIP And T_Peer.PossibleRemoteServerPort = T_Port) Or T_Peer.PublicKey = T_PPublicKey Then
                                                            Founded = True
                                                            Exit For
                                                        End If

                                                    Next

                                                    If Not Founded Then
                                                        Connect(T_HostIP, T_Port)
                                                    End If

                                                End If

                                            ElseIf CheckXMLTag(T_Message, "RefreshOrder") Then

                                                Dim T_AT As String = ""

                                                If CheckXMLTag(T_Message, "AT") Then
                                                    T_AT = GetXMLMessage(T_Message, "AT")
                                                End If

                                                Dim T_Type As String = ""
                                                If CheckXMLTag(T_Message, "Type") Then
                                                    T_Type = GetXMLMessage(T_Message, "Type")
                                                End If

                                                Dim T_Seller As String = ""
                                                If CheckXMLTag(T_Message, "SellerID") Then
                                                    T_Seller = GetXMLMessage(T_Message, "SellerID")
                                                End If

                                                Dim T_Buyer As String = ""
                                                If CheckXMLTag(T_Message, "BuyerID") Then
                                                    T_Buyer = GetXMLMessage(T_Message, "BuyerID")
                                                End If

                                                Dim T_AccountID As ULong = GetAccountID(T_PubKey)

                                                If T_Type.Trim = "SellOrder" Then
                                                    If Not T_AccountID.ToString = T_Seller Then
                                                        BroadcastOK = False
                                                    End If
                                                ElseIf T_Type.Trim = "BuyOrder" Then
                                                    If Not T_AccountID.ToString = T_Buyer Then
                                                        BroadcastOK = False
                                                    End If
                                                End If

                                            End If

                                            If BroadcastOK Then

                                                Dim Found As Boolean = False
                                                For Each BroadcastedMsg As String In LastBroadcastedMsgList

                                                    If T_GetMsg = BroadcastedMsg Then
                                                        Found = True
                                                        Exit For
                                                    End If

                                                Next

                                                If Not Found Then
                                                    LastBroadcastedMsgList.Insert(0, T_GetMsg)

                                                    Dim SendMessage As String = CreateXMLMessage("Confirmation", E_XMLTags.Response) + CreateXMLMessage(GetUnixTimestamp(), E_XMLTags.Timestamp) + CreateXMLMessage(DEXNET_PublicKeyHEX, E_XMLTags.PublicKey)
                                                    SendMessage = CreateSignatureHash(SendMessage)

                                                    For PeerInt As Integer = 0 To Peers.Count - 1
                                                        Dim Peer As S_Peer = Peers(PeerInt)

                                                        If Peer.TCPClient.Client.RemoteEndPoint.ToString = T_Process.GetIP And Peer.Port = T_Process.GetPort Then
                                                            Peer.PublicKey = T_Process.PublicKey
                                                            Peer.SetMsgs.Add(SendMessage)
                                                            Peer.PossibleHost = T_Process.GetPossibleHost
                                                            Peer.PossibleRemoteServerPort = T_Process.PossibleRemoteServerPort
                                                        Else
                                                            Peer.SetMsgs.Add(T_GetMsg)
                                                        End If

                                                        Peers(PeerInt) = Peer

                                                    Next

                                                End If

                                            End If

                                        End If

                                    End If

                                Case "Ping"

                                    Dim Answer As String = CreateXMLMessage("Ping", E_XMLTags.Response) + CreateXMLMessage(GetUnixTimestamp(), E_XMLTags.Timestamp) + CreateXMLMessage(DEXNET_PublicKeyHEX, E_XMLTags.PublicKey)
                                    Answer = CreateSignatureHash(Answer)

                                    T_Process.SetMsg = CreateSignatureHash(Answer)

                                    For i As Integer = 0 To Peers.Count - 1
                                        Dim T_Peer As S_Peer = Peers(i)

                                        If T_Peer.PossibleHost = T_Process.GetPossibleHost And T_Peer.Port = T_Process.GetPort Then
                                            T_Peer.SetMsgs.Add(T_Process.SetMsg)
                                            T_Peer.PublicKey = T_Process.PublicKey
                                            Peers(i) = T_Peer
                                            Exit For
                                        End If

                                    Next

                            End Select

                        ElseIf Not Response.Trim = "" Then

                            Select Case Response

                                Case "GetClients"

                                    If DEXNET_PublicKeyHEX <> T_PubKey And Not T_PubKey.Trim = "" Then
                                        If CheckSignatureHash(T_GetMsg, T_PubKey, T_SigHash) = True Then

                                            Dim Message As String = GetXMLMessage(T_GetMsg, E_XMLTags.Message)

                                            If Not Message = "" Then
                                                Dim Entrys As String = GetXMLMessage(Message, "Entrys")

                                                Dim CNT As Integer = 0

                                                While True
                                                    Dim Entry As String = GetXMLMessage(Message, "Entry" + CNT.ToString)

                                                    If Entry.Trim = "" Then
                                                        Exit While
                                                    End If

                                                    Dim HostIP As String = GetXMLMessage(Entry, "HostIP")
                                                    Dim PeersPort As String = GetXMLMessage(Entry, "PeersPort")
                                                    Dim PeersPublicKey As String = GetXMLMessage(Entry, "PeersPublicKey")

                                                    If Not PeersPublicKey.Trim = "" And Not PeersPublicKey = DEXNET_PublicKeyHEX Then
                                                        If Not PeersPort.Trim = "" And Not PeersPort.Trim = "-1" Then
                                                            Dim PeersPortInt As Integer = Integer.Parse(PeersPort)

                                                            Dim Founded As Boolean = False
                                                            For c As Integer = 0 To Peers.Count - 1
                                                                Dim T_Peer As S_Peer = Peers(c)
                                                                If T_Peer.PublicKey = PeersPublicKey Or (Not CheckHostIsNotIP(T_Peer.PossibleHost, HostIP) And T_Peer.PossibleRemoteServerPort = PeersPortInt) Then
                                                                    Founded = True
                                                                End If

                                                            Next

                                                            If Not Founded Then
                                                                Connect(HostIP, PeersPortInt)
                                                            End If

                                                        End If
                                                    End If

                                                    CNT += 1

                                                End While

                                            End If

                                        End If
                                    End If

                                Case "Broadcast"

                                    If DEXNET_PublicKeyHEX <> T_PubKey And Not T_PubKey.Trim = "" Then
                                        If CheckSignatureHash(T_GetMsg) = True Then

                                            Dim Found As Boolean = False
                                            For Each BroadcastedMsg As String In LastBroadcastedMsgList

                                                If T_GetMsg = BroadcastedMsg Then
                                                    Found = True
                                                    Exit For
                                                End If

                                            Next

                                            If Not Found Then
                                                LastBroadcastedMsgList.Insert(0, T_GetMsg)

                                                For PeerInt As Integer = 0 To Peers.Count - 1
                                                    Dim Peer As S_Peer = Peers(PeerInt)

                                                    If Peer.TCPClient.Client.RemoteEndPoint.ToString = T_Process.GetIP And Peer.Port = T_Process.GetPort Then
                                                        'send nothing back
                                                    Else
                                                        Peer.SetMsgs.Add(T_GetMsg)
                                                    End If

                                                    Peers(PeerInt) = Peer

                                                Next

                                            End If

                                        End If

                                    End If

                                Case "Confirmation"

                                    If DEXNET_PublicKeyHEX <> T_PubKey And Not T_PubKey.Trim = "" Then

                                        If CheckSignatureHash(T_GetMsg) = True Then

                                            Dim SendMessage As String = CreateXMLMessage("GetClients", E_XMLTags.Request) + CreateXMLMessage(GetUnixTimestamp(), E_XMLTags.Timestamp) + CreateXMLMessage(DEXNET_PublicKeyHEX, E_XMLTags.PublicKey)

                                            For PeerInt As Integer = 0 To Peers.Count - 1
                                                Dim Peer As S_Peer = Peers(PeerInt)

                                                If Peer.TCPClient.Client.RemoteEndPoint.ToString = T_Process.GetIP And Peer.Port = T_Process.GetPort And Peer.PublicKey = "" Then
                                                    Peer.PublicKey = T_PubKey
                                                    Peer.PossibleHost = T_Process.GetPossibleHost
                                                    Peer.SetMsgs.Add(SendMessage)
                                                    Peer.PossibleRemoteServerPort = T_Process.PossibleRemoteServerPort
                                                    Peers(PeerInt) = Peer
                                                    Exit For
                                                    'Else
                                                    'TODO: tell other Peers about the new peer
                                                    'Peer.SetMsg = ""
                                                End If

                                            Next

                                        End If

                                    End If

                                Case "Ping"

                                    If DEXNET_PublicKeyHEX <> T_PubKey And Not T_PubKey.Trim = "" Then
                                        If CheckSignatureHash(T_GetMsg, T_PubKey, T_SigHash) = True Then

                                            For PeerInt As Integer = 0 To Peers.Count - 1
                                                Dim Peer As S_Peer = Peers(PeerInt)

                                                If Peer.TCPClient.Client.RemoteEndPoint.ToString = T_Process.GetIP And Peer.Port = T_Process.GetPort Then
                                                    Peer.Timeout = -1
                                                    Peers(PeerInt) = Peer
                                                    Exit For
                                                End If

                                            Next

                                        End If
                                    End If


                            End Select

                        End If

                    End If

                Next

            Next

        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in ProcessMessages(): -> " + ex.Message)
            End If

        End Try

        Return True

    End Function

    Private Function CreateSignatureHash(ByVal Message As String, Optional ByVal PrivateKeyHEX As String = "") As String

        If Message.Contains("<" + E_XMLTags.SignatureHash.ToString + ">") And Message.Contains("</" + E_XMLTags.SignatureHash.ToString + ">") Then
            'Refreshing Hash
            Dim T_TMHash As String = GetXMLMessage(Message, E_XMLTags.SignatureHash)
            Message = Message.Replace(CreateXMLMessage(T_TMHash, E_XMLTags.SignatureHash), CreateXMLMessage("", E_XMLTags.SignatureHash))
        Else
            'Adding Hash
            Message += CreateXMLMessage("", E_XMLTags.SignatureHash)
        End If

        If PrivateKeyHEX.Trim = "" Then
            PrivateKeyHEX = DEXNET_PrivateKeyHEX
        End If

        Message = Message.Replace(CreateXMLMessage("", E_XMLTags.SignatureHash), CreateXMLMessage(GenerateSignature(StringToHEXString(Message), PrivateKeyHEX), E_XMLTags.SignatureHash))

        Return Message

    End Function
    Private Function CheckSignatureHash(ByVal GetMsg As String, Optional ByVal PublicKey As String = "", Optional ByVal SignatureHash As String = "") As Boolean

        If Not GetMsg.Trim = "" Then

            Dim T_PublicKey As String = PublicKey
            If T_PublicKey.Trim = "" Then
                T_PublicKey = GetXMLMessage(GetMsg, E_XMLTags.PublicKey)
            End If

            Dim T_SignatureHash As String = SignatureHash
            If T_SignatureHash.Trim = "" Then
                T_SignatureHash = GetXMLMessage(GetMsg, E_XMLTags.SignatureHash)
            End If

            Dim T_UnsignedMessage As String = GetMsg.Replace(CreateXMLMessage(T_SignatureHash, E_XMLTags.SignatureHash), CreateXMLMessage("", E_XMLTags.SignatureHash))

            Return VerifySignature(T_SignatureHash, StringToHEXString(T_UnsignedMessage), T_PublicKey)

        End If

        Return False

    End Function
    Private Function SendMessages() As Boolean

        For i As Integer = 0 To Peers.Count - 1
            Dim T_Peer As S_Peer = Peers(i)

            If T_Peer.SetMsgs.Count > 0 Then

                Try

                    Dim TempMsg As String = T_Peer.SetMsgs(0)
                    T_Peer.LastSetMsg = TempMsg
                    T_Peer.SetMsgs.RemoveAt(0)
                    T_Peer.StreamWriter.WriteLine(TempMsg)
                    T_Peer.StreamWriter.Flush()
                    Peers(i) = T_Peer


                    If ShowStatus Then
                        Dim LogMsg As String = "(" + Now.ToShortDateString + " " + Now.ToLongTimeString + ") " + T_Peer.PossibleHost + ":" + T_Peer.Port.ToString + " sended: " + TempMsg
                        StatusList.Add(LogMsg)
                    End If

                Catch ex As Exception

                    If GetINISetting(E_Setting.InfoOut, False) Then
                        Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                        Out.ErrorLog2File(Application.ProductName + "-error in SendMessages(): -> " + ex.Message)
                    End If

                End Try

            End If

        Next

        Return True

    End Function

    Function FilterRelevantMsgs() As Boolean

        Dim BCMessages As List(Of String) = GetBroadcastedMsgs()

        For i As Integer = 0 To BCMessages.Count - 1
            Dim BCMessage As String = BCMessages(i)

            Dim T_Message As String = GetXMLMessage(BCMessage, E_XMLTags.Message)
            Dim T_PubKey As String = GetXMLMessage(BCMessage, E_XMLTags.PublicKey)

            Dim AddressedMessage As Boolean = False
            If MsgIsHEXStr(T_Message) Then

                Dim DecryptedMSG As String = DecryptMsg(T_Message, DEXNET_AgreeKeyHEX, T_PubKey)

                If Not DecryptedMSG = T_Message Then
                    AddressedMessage = True
                    BCMessage = BCMessage.Replace(T_Message, DecryptedMSG)
                End If

            End If


            Dim ExpectedMessage As Boolean = False
            For ii As Integer = 0 To RelevantMsgs.Count - 1
                Dim RevMsg As S_RelevantMessage = RelevantMsgs(ii)

                If BCMessage.Contains(RevMsg.RelevantKey) Then
                    ExpectedMessage = True
                    RevMsg.RelevantMessage = BCMessage
                    RevMsg.Timestamp = Convert.ToDouble(GetUnixTimestamp())
                    RelevantMsgs(ii) = RevMsg
                    Exit For
                End If

            Next

            If Not ExpectedMessage And AddressedMessage Then

                Dim ShortID As String = GetID()
                ShortID = ShortID.Remove(8)
                RelevantKeys.Add("Unexpected" + ShortID)

                Dim T_RelevantMsg As S_RelevantMessage = New S_RelevantMessage

                T_RelevantMsg.Lifetime = 240
                T_RelevantMsg.RelevantKey = "Unexpected" + ShortID
                T_RelevantMsg.RelevantMessage = BCMessage
                T_RelevantMsg.Timestamp = Convert.ToDouble(GetUnixTimestamp())

                RelevantMsgs.Add(T_RelevantMsg)

            End If


        Next

        Return True

    End Function

    Function DecryptMsg(ByVal Message As String, ByVal AgreementKeyHex As String, ByVal PublicKey As String) As String

        Dim MsgCopy As String = Message

        'Dim PubKey As String = ""
        'If MsgCopy.Length > 64 Then

        'PubKey = MsgCopy.Remove(64)
        'MsgCopy = MsgCopy.Substring(64)

        Dim Curve As ClsCurve25519 = New ClsCurve25519
        Dim SharedKeyBytes As Byte() = New Byte(31) {}
        Curve.GetSharedSecret(SharedKeyBytes, HEXStringToByteArray(AgreementKeyHex), HEXStringToByteArray(PublicKey))
        Dim SharedKeyHEXString As String = ByteArrayToHEXString(SharedKeyBytes)
        MsgCopy = AESDecrypt(MsgCopy, SharedKeyHEXString)

        'End If

        If MsgCopy.Trim = "" Then
            Return Message
        Else
            Return MsgCopy
        End If

    End Function

    Function ClearRelevantMsgs() As Boolean
        Try
#Region "Clear DeadKeys from RelevantMessages"
            Dim DeadFounded As Boolean = True
            While DeadFounded
                DeadFounded = False

                Dim DelIDX As Integer = -1
                For ii As Integer = 0 To RelevantMsgs.Count - 1
                    Dim RelevantMsg As S_RelevantMessage = RelevantMsgs(ii)

                    Dim Exists As Boolean = False

                    Dim Cnt As Integer = RelevantKeys.Count
                    For i As Integer = 0 To Cnt - 1
                        If Cnt <> RelevantKeys.Count Then
                            Continue While
                        End If

                        Dim RelevantKey As String = RelevantKeys(i)

                        If RelevantMsg.RelevantKey = RelevantKey Then
                            Exists = True
                            Exit For
                        End If

                    Next

                    If Not Exists Then
                        DeadFounded = True
                        DelIDX = ii
                        Exit For
                    End If

                Next

                If DeadFounded And DelIDX <> -1 Then
                    RelevantMsgs.RemoveAt(DelIDX)
                End If

            End While
#End Region
#Region "Add RelevantKey to RelevantMessages"
            For i As Integer = 0 To RelevantKeys.Count - 1
                Dim RelevantKey As String = RelevantKeys(i)

                Dim Notfound As Boolean = True
                For ii As Integer = 0 To RelevantMsgs.Count - 1
                    Dim RelevantMsg As S_RelevantMessage = RelevantMsgs(ii)

                    If RelevantMsg.RelevantKey = RelevantKey Then
                        Notfound = False
                        Exit For
                    End If

                Next

                If Notfound Then
                    Dim T_RelevantMsg As S_RelevantMessage = New S_RelevantMessage
                    T_RelevantMsg.RelevantKey = RelevantKey
                    T_RelevantMsg.Timestamp = Convert.ToDouble(GetUnixTimestamp())
                    T_RelevantMsg.Lifetime = 240.0
                    T_RelevantMsg.RelevantMessage = ""

                    RelevantMsgs.Add(T_RelevantMsg)

                End If

            Next
#End Region
#Region "Timeout Relevant Messages"
            Dim DelTempKey As String = ""
            For i As Integer = 0 To RelevantMsgs.Count - 1
                Dim RevMsg As S_RelevantMessage = RelevantMsgs(i)
                Dim DiffTimestamp As Double = Double.Parse(GetUnixTimestamp()) - RevMsg.Timestamp
                If DiffTimestamp > RevMsg.Lifetime Then
                    If RevMsg.RelevantMessage.Trim = "" Then ' RevMsg.RelevantKey.Contains("Unexpected") Or
                        DelTempKey = RevMsg.RelevantKey
                    End If
                    RevMsg.RelevantMessage = ""
                    RevMsg.Timestamp = Convert.ToDouble(GetUnixTimestamp())
                    RelevantMsgs(i) = RevMsg
                End If
            Next

            If Not DelTempKey.Trim = "" Then
                RelevantKeys.Remove(DelTempKey)
            End If
#End Region
        Catch ex As Exception
            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in ProcessMessages(): -> " + ex.Message)
            End If

        End Try

        Return True

    End Function
    Function AddRelevantKey(ByVal RelevantKey As String) As Boolean

        Dim Founded As Boolean = False
        For i As Integer = 0 To RelevantKeys.Count - 1
            Dim RevKey As String = RelevantKeys(i)

            If RevKey = RelevantKey Then
                Founded = True
                Exit For
            End If

        Next

        If Not Founded Then
            RelevantKeys.Add(RelevantKey)
        End If

        Return True

    End Function
    Function DelRelevantKey(ByVal RelevantKey As String) As Boolean

        Dim FoundIDX As Integer = -1

        For i As Integer = 0 To RelevantKeys.Count - 1
            Dim RevKey As String = RelevantKeys(i)

            If RevKey = RelevantKey Then
                FoundIDX = i
                Exit For
            End If

        Next

        If FoundIDX <> -1 Then
            RelevantKeys.RemoveAt(FoundIDX)
        End If

        Return True

    End Function

#End Region

#Region "Specials"

    Function CreateXMLMessage(ByVal Message As String, ByVal Key As E_XMLTags) As String
        Return "<" + Key.ToString + ">" + Message + "</" + Key.ToString + ">"
    End Function
    Public Shared Function CreateXMLMessage(ByVal Message As String, ByVal Key As String) As String
        Return "<" + Key + ">" + Message + "</" + Key + ">"
    End Function

    Function GetXMLMessage(ByVal Message As String, ByVal Key As E_XMLTags) As String
        Return GetStringBetween(Message, "<" + Key.ToString + ">", "</" + Key.ToString + ">")
    End Function
    Public Shared Function GetXMLMessage(ByVal Message As String, ByVal Key As String) As String
        Return GetStringBetween(Message, "<" + Key.Trim + ">", "</" + Key.Trim + ">")
    End Function
    Function CheckXMLTag(ByVal Message As String, ByVal Tag As String) As Boolean

        If Message.Contains("<" + Tag.Trim + ">") And Message.Contains("</" + Tag.Trim + ">") Then
            Return True
        Else
            Return False
        End If

    End Function

    Function CheckHost(ByVal IPorHost As String) As Boolean

        Try

            Dim T_IPHost As Net.IPHostEntry = New Net.IPHostEntry()
            T_IPHost = System.Net.Dns.GetHostEntry(IPorHost)

            Return True

        Catch ex As Exception
            Return False
        End Try

    End Function
    Function CheckHostIsNotIP(ByVal IPorHost As String, ByVal IP As String) As Boolean

        If IPorHost.Trim = "" Then
            Return False
        End If

        If Not CheckHost(IPorHost) Then
            Return False
        End If

        Try

            Dim T_Host As Net.IPHostEntry = New Net.IPHostEntry()
            T_Host = System.Net.Dns.GetHostEntry(IPorHost)

            Dim T_HostAddresses As List(Of Net.IPAddress) = New List(Of Net.IPAddress)(T_Host.AddressList)
            Dim T_HostIP As String = IPorHost
            For Each IPAddress As Net.IPAddress In T_HostAddresses
                If IPAddress.AddressFamily = Net.Sockets.AddressFamily.InterNetwork Then
                    T_HostIP = IPAddress.ToString
                End If
            Next


            Dim T_IP As Net.IPHostEntry = New Net.IPHostEntry()
            T_IP = System.Net.Dns.GetHostEntry(IP)

            Dim T_IPAddresses As List(Of Net.IPAddress) = New List(Of Net.IPAddress)(T_IP.AddressList)
            Dim T_IPIP As String = IP
            For Each IPAddress As Net.IPAddress In T_IPAddresses
                If IPAddress.AddressFamily = Net.Sockets.AddressFamily.InterNetwork Then
                    T_IPIP = IPAddress.ToString
                End If
            Next

            If T_IPIP = T_HostIP Or T_IPIP = "" Or T_HostIP = "" Then
                Return False
            Else
                Return True
            End If

        Catch ex As Exception
            Return False
        End Try


    End Function


    Function MsgIsHEXStr(ByVal Message As String) As Boolean

        If Message.Length Mod 2 <> 0 Then
            Return False
        End If

        Dim CharAry() As Char = Message.ToUpper.ToCharArray

        For Each Chr As Char In CharAry
            Select Case Chr
                Case "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "A"c, "B"c, "C"c, "D"c, "E"c, "F"c
                Case Else
                    Return False
            End Select

        Next

        Return True

    End Function

#End Region

End Class