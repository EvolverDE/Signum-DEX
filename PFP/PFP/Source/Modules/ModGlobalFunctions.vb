
Module ModGlobalFunctions

    Function GetID() As String

        For i As Integer = 0 To Now.Millisecond
            Rnd()
        Next

        Dim IDMix As String = System.Environment.MachineName + System.Environment.UserDomainName + System.Environment.UserName + Now.ToLongDateString + Now.ToLongTimeString + Rnd().ToString

        Return GetSHA256HashString(IDMix)

    End Function

    Function GetAccountID(ByVal PublicKeyHEX As String) As ULong
        Dim PubKeyAry() As Byte = HEXStringToByteArray(PublicKeyHEX)
        Dim SHA256 As System.Security.Cryptography.SHA256Managed = New System.Security.Cryptography.SHA256Managed()
        PubKeyAry = SHA256.ComputeHash(PubKeyAry)
        Return BitConverter.ToUInt64(PubKeyAry, 0) '{PubKeyAry(0), PubKeyAry(1), PubKeyAry(2), PubKeyAry(3), PubKeyAry(4), PubKeyAry(5), PubKeyAry(6), PubKeyAry(7)}, 0)
    End Function
    Function GetAccountRS(ByVal PublicKeyHEX As String) As String
        Return ClsReedSolomon.Encode(GetAccountID(PublicKeyHEX))
    End Function
    Function GetAccountRSFromID(ByVal AccountID As ULong) As String
        Return ClsReedSolomon.Encode(AccountID)
    End Function
    Function GetAccountIDFromRS(ByVal AccountRS As String) As ULong
        Return ClsReedSolomon.Decode(AccountRS)
    End Function

    Function RandomBytes(ByVal Length As Integer) As Byte()

        Dim rnd As Random = New Random
        Dim b(Length) As Byte
        rnd.NextBytes(b)

        Return b

    End Function

    Function MessageIsHEXString(ByVal Message As String) As Boolean

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

    Function GetUnixTimestamp() As String

        Dim UnixTime As Double
        UnixTime = (DateTime.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds

        Dim UnixTimeString As String = UnixTime.ToString
        If UnixTimeString.Contains(",") Then
            UnixTimeString = UnixTimeString.Remove(UnixTimeString.IndexOf(","))
        End If

        Return UnixTimeString

    End Function

    Enum E_ConnectionStatus
        Offline = 0
        InSync = 1
        NoDEXNETPeers = 2
        NoSignumAPI = 3
        Online = 4
    End Enum

    Function GetConnectionStatus(ByVal PrimaryNode As String, ByVal DEXNET As ClsDEXNET) As E_ConnectionStatus

        Dim SLS As ClsSignumAPI = New ClsSignumAPI(PrimaryNode)
        Dim Block As Integer = SLS.GetCurrentBlock

        Dim PeerCNT As Integer = 0

        If Not IsNothing(DEXNET) Then
            PeerCNT = DEXNET.Peers.Count
        End If

        If Block = 0 And PeerCNT = 0 Then
            Return E_ConnectionStatus.Offline
        ElseIf Block = 0 And PeerCNT >= 1 Then
            Return E_ConnectionStatus.NoSignumAPI
        ElseIf Block >= 1 And PeerCNT = 0 Then
            Return E_ConnectionStatus.NoDEXNETPeers
        ElseIf Block >= 1 And PeerCNT >= 1 Then
            Return E_ConnectionStatus.Online
        Else
            Return E_ConnectionStatus.Offline
        End If

    End Function


#Region "PayPal Interactions"
    Function CheckPayPalAPI() As String

        Dim PPAPI As ClsPayPal = New ClsPayPal
        PPAPI.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
        PPAPI.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")


        Dim PPOrderID As List(Of String) = PPAPI.GetAuthToken()

        If PPOrderID.Count > 0 Then
            If PPOrderID(0).Contains("<error>") Then
                Return Between(PPOrderID(0), "<error>", "</error>", GetType(String))
            End If
        End If

        Return "True"

    End Function
#End Region

#Region "ListView Specials"

    ''' <summary>
    ''' Von einer ListView das subitem aus dem item lesen
    ''' </summary>
    ''' <param name="LV">Die ListView, aus der gelesen werden soll</param>
    ''' <param name="ColName">Der Spaltenname, aus dem gelesen werden soll</param>
    ''' <param name="LVItem">Die Zeile bzw. das item aus dem gelesen werden soll</param>
    ''' <param name="index">Alternativ das item an index stelle in der ListView</param>
    ''' <returns>Vorzugsweise einen String, andernfalls den index der Spalte</returns>
    ''' <remarks></remarks>
    Function GetLVColNameFromSubItem(ByRef LV As ListView, ByVal ColName As String, Optional ByVal LVItem As ListViewItem = Nothing, Optional ByVal index As Integer = -1) As Object

        If index > -1 Then
            LVItem = LV.Items.Item(index)
        End If

        If LVItem Is Nothing Then

            For i As Integer = 0 To LV.Columns.Count - 1
                Dim col As String = LV.Columns.Item(i).Text
                If col = ColName Then
                    Return i
                End If
            Next

        Else

            For i As Integer = 0 To LV.Columns.Count - 1
                Dim col As String = LV.Columns.Item(i).Text
                If col = ColName Then
                    Return LVItem.SubItems.Item(i).Text
                End If
            Next

        End If

        Return -1

    End Function
    Sub SetLVColName2SubItem(ByRef LV As ListView, ByRef LVItem As ListViewItem, ByVal ColName As String, ByVal SetStr As String, Optional ByVal ForeColor As Color = Nothing, Optional ByVal BackColor As Color = Nothing)

        Dim IDX As Integer = GetLVColNameFromSubItem(LV, ColName)

        If Not IDX < 0 Then
            If IsNothing(ForeColor) Or IsNothing(BackColor) Then
                LVItem.SubItems(IDX).Text = SetStr
            Else
                LVItem.SubItems(IDX).Text = SetStr

                If Not IsNothing(ForeColor) Then
                    LVItem.SubItems(IDX).ForeColor = ForeColor
                End If

                If Not IsNothing(BackColor) Then
                    LVItem.SubItems(IDX).BackColor = BackColor
                End If

            End If

        End If

    End Sub

#End Region


End Module