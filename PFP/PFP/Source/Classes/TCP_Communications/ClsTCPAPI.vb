
Option Strict On
Option Explicit On

Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Text

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

            T_TCPServer.Stop()

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

                    If IsNothing(SCon2.TCPClient.Client) Then

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

            TCPClient.SendTimeout = 1000
            TCPClient.ReceiveTimeout = 1000

            Dim recvBytes(1024) As Byte
            Dim htmlReq As String = Nothing
            Dim bytes As Integer = 0

            Try

                bytes = TCPClient.Client.Receive(recvBytes, 0, TCPClient.Client.Available, SocketFlags.None)
                htmlReq = Encoding.ASCII.GetString(recvBytes, 0, bytes)

                htmlReq = htmlReq.Replace(vbLf, "")

                'MultiInvoker(ListBox1, "Items", {"Insert", 0, htmlReq})
                'MultiInvoker(ListBox1, "Items", {"Insert", 0, "HTTP Request: "})

                Dim RequestList As List(Of String) = New List(Of String)
                Dim CarrierReturn As Char = Convert.ToChar(vbCr)
                RequestList.AddRange(htmlReq.Split(CarrierReturn))


                '		(0)	    "GET /API/v1.0/GetCandles?pair=USD_SIGNA&days=3&tickmin=15 HTTP/1.1"	String
                '		(1)	    "Host: localhost:8130"	String
                '		(2)	    "Connection: keep-alive"	String
                '		(3) 	"Cache-Control: max-age=0"	String
                '		(4)	    "sec-ch-ua: ""Google Chrome"";v=""89"", ""Chromium"";v=""89"", "";Not A Brand"";v=""99"""	String
                '		(5)	    "sec-ch-ua-mobile: ?0"	String
                '		(6)	    "Upgrade-Insecure-Requests: 1"	String
                '		(7)	    "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.128 Safari/537.36"	String
                '		(8)	    "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"	String
                '		(9)	    "Sec-Fetch-Site: none"	String
                '		(10)	"Sec-Fetch-Mode: navigate"	String
                '		(11)	"Sec-Fetch-User: ?1"	String
                '		(12)	"Sec-Fetch-Dest: document"	String
                '		(13)	"Accept-Encoding: gzip, deflate, br"	String
                '		(14)	"Accept-Language: de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7"	String
                '		(15)	""	String
                '		(16)	""	String


                If RequestList.Count > 0 Then

                    Dim Method_Path_HTTPv As List(Of String) = New List(Of String)(RequestList(0).Split(" "c))

                    If Method_Path_HTTPv(0).Trim.ToUpper = "GET" Then


                        Dim Request As String = Method_Path_HTTPv(1).Trim
                        Dim SubRequest As List(Of String) = New List(Of String)

                        If Request.Contains("/") Then
                            SubRequest.AddRange(Request.Split("/"c))
                        End If

                        If SubRequest.Count > 0 Then
                            If SubRequest(0).Trim = "" Then
                                SubRequest.RemoveAt(0)
                            End If
                        End If

                        Dim APIInterface As String = ""
                        Dim APIVersion As String = ""
                        Dim APICommand As String = ""

                        If SubRequest.Count >= 3 Then
                            APIInterface = SubRequest(0)
                            APIVersion = SubRequest(1)
                            APICommand = SubRequest(2)
                        End If


                        Dim ResponseHTML As String = "{""response"":""no data.""}"

                        Dim QueryParameters As List(Of String) = New List(Of String)
                        If APICommand.Contains("?") Then
                            Dim T_Parameters As String = APICommand.Substring(APICommand.IndexOf("?") + 1)
                            APICommand = APICommand.Remove(APICommand.IndexOf("?"))
                            QueryParameters.AddRange(T_Parameters.Split("&"c).ToArray)
                        End If


                        For Each Response As API_Response In ResponseMSGList
                            If Response.API_Interface = APIInterface And Response.API_Version = APIVersion And Response.API_Command = APICommand Then

                                If QueryParameters.Count > 0 Then

                                    If CheckLists(QueryParameters, Response.API_Parameters) Then
                                        ResponseHTML = Response.API_Response
                                    End If
                                Else
                                    ResponseHTML = Response.API_Response
                                End If

                            End If
                        Next


                        'ResponseHTML += "<form action = ""GetCandles"" id=""person"" method=""POST"">"
                        'ResponseHTML += "<Label Class=""h2"" form=""person"">API-login</label>"
                        'ResponseHTML += "<Label for=""vorname"">APIUser</label>"
                        'ResponseHTML += "<input type = ""text"" name=""APIUser"" id=""APIUser"" maxlength=""30"">"
                        'ResponseHTML += "<Label for=""zuname"">APIKey</label>"
                        'ResponseHTML += "<input type = ""text"" name=""APIKey"" id=""APIKey"" maxlength=""40"">"
                        'ResponseHTML += "<Button type = ""reset"" > Eingaben zur&uuml;cksetzen</button>"
                        'ResponseHTML += "<Button type = ""submit"" > Eingaben absenden</button>"
                        'ResponseHTML += "</form>"

                        'ResponseHTML += "{""response"":""GetCandles"",""Data"":{""PAIR"":""USD/SIGNA"",""TICMIN"":""5"",{""Date"":""01.01.2021 12:30:00"",""OPEN"":""12.34567890"",""HIGH"":""15.12345678"",""LOW"":""10.87654321"",""CLOSE"":""13.24681357""},{""Date"":""01.01.2021 12:35:00"",""OPEN"":""13.24681357"",""HIGH"":""16.12345678"",""LOW"":""9.87654321"",""CLOSE"":""12.09876543""}}}"

                        SendHTMLResponse(TCPClient, ResponseHTML)

                    ElseIf Method_Path_HTTPv(0).Trim.ToUpper = "POST" Then

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


                        'Dim ResponseHTML As String = ""
                        'ResponseHTML += "{""response"":""GetCandles"",""Data"":{""PAIR"":""USD/SIGNA"",""TICMIN"":""5"",{""Date"":""01.01.2021 12:30:00"",""OPEN"":""12.34567890"",""HIGH"":""15.12345678"",""LOW"":""10.87654321"",""CLOSE"":""13.24681357""},{""Date"":""01.01.2021 12:35:00"",""OPEN"":""13.24681357"",""HIGH"":""16.12345678"",""LOW"":""9.87654321"",""CLOSE"":""12.09876543""}}}"

                        SendHTMLResponse(TCPClient, "{""response"":""no data.""}")

                    Else
                        ' Not HTTP GET method

                        SendHTMLResponse(TCPClient, "{""response"":""error wrong request""}")

                    End If

                End If

            Catch ex As Exception
                If API_ShowStatusMSG Then
                    StatusMSG.Add("ProcessRequest(): " + ex.Message)
                End If

                TCPClient.Close()

            End Try

        Next



    End Sub


    Private Function CheckLists(ByVal List1 As List(Of String), List2 As List(Of String)) As Boolean

        Dim Returner As Boolean = True

        If List1.Count <> List2.Count Then
            Return False
        End If

        If List1.Count = 0 Then
            Return True
        End If

        Dim Result As List(Of Boolean) = New List(Of Boolean)

        For i As Integer = 0 To List1.Count - 1

            Dim List1Entry As String = List1(i)

            Dim Founded As Boolean = False
            For j As Integer = 0 To List2.Count - 1

                Dim List2Entry As String = List2(j)

                If List1Entry = List2Entry Then
                    Result.Add(True)
                    Founded = True
                    Exit For
                End If

            Next

            If Not Founded Then
                Result.Add(False)
            End If

        Next

        For Each Res As Boolean In Result
            If Res = False Then
                Return False
            End If
        Next

        Return True

    End Function


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