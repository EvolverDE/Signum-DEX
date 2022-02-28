
Module ModGlobalFunctions
    Property GlobalPublicKey() As String = ""
    Property GlobalAccountID() As ULong = 0UL
    Property GlobalAddress() As String = ""

    Property GlobalPIN As String = ""

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
        Dim Block As Integer = SLS.GetCurrentBlock()

        Dim PeerCNT As Integer = 0

        If Not DEXNET Is Nothing Then
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


    Function CheckPIN() As Boolean

        Dim PINFingerprint As String = GetINISetting(E_Setting.PINFingerPrint, "")

        If PINFingerprint = "" Then

            Dim PlainPassPhrase As String = GetINISetting(E_Setting.PassPhrase, "")

            If PlainPassPhrase.Trim = "" Then
                Return False
            Else

                Dim PubKey As String = GetPubKeyHEX(PlainPassPhrase)
                Dim AccID As ULong = GetAccountID(PubKey)
                Dim RS As String = ClsReedSolomon.Encode(AccID)

                If GlobalAddress.Contains(RS) Then
                    Return True
                Else
                    Return False
                End If

            End If

        Else

            Dim SHA512 As System.Security.Cryptography.SHA512 = System.Security.Cryptography.SHA512Managed.Create()
            Dim HashPIN() As Byte = SHA512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(GlobalPIN).ToArray)
            Dim HashPINHEX As String = ByteArrayToHEXString(HashPIN)

            If HashPINHEX = PINFingerprint Then

                Dim AESPassPhrase As String = GetINISetting(E_Setting.PassPhrase, "")

                If AESPassPhrase.Trim = "" Then
                    Return False
                Else

                    Dim DecryptedPassPhrase As String = AESDecrypt(AESPassPhrase, GlobalPIN)

                    If DecryptedPassPhrase = AESPassPhrase Or DecryptedPassPhrase.Trim = "" Then
                        Return False
                    End If

                    Dim PubKey As String = GetPubKeyHEX(DecryptedPassPhrase)
                    Dim AccID As ULong = GetAccountID(PubKey)
                    Dim RS As String = ClsReedSolomon.Encode(AccID)

                    If GlobalAddress.Contains(RS) Then
                        Return True
                    Else
                        Return False
                    End If

                End If

            Else
                Return False
            End If

        End If

    End Function

    ''' <summary>
    ''' 0=PubKeyHEX; 1=SignKeyHEX; 2=AgreeKeyHEX; 3=PassPhrase; 
    ''' </summary>
    ''' <returns></returns>
    Function GetPassPhrase() As List(Of String)

        If Not CheckPIN() Then
            Return New List(Of String)
        End If

        Dim PINFingerprint As String = GetINISetting(E_Setting.PINFingerPrint, "")

        If PINFingerprint = "" Then
            Dim PlainPassPhrase As String = GetINISetting(E_Setting.PassPhrase, "")

            Dim MasterKeys As List(Of String) = GetMasterKeys(PlainPassPhrase)
            MasterKeys.Add(PlainPassPhrase)

            Return MasterKeys
        Else
            Dim AESPassPhrase As String = GetINISetting(E_Setting.PassPhrase, "")
            Dim DecryptedPassPhrase As String = AESDecrypt(AESPassPhrase, GlobalPIN)

            If DecryptedPassPhrase = AESPassPhrase Or DecryptedPassPhrase.Trim = "" Then
                Return New List(Of String)
            End If

            Dim MasterKeys As List(Of String) = GetMasterKeys(DecryptedPassPhrase)
            MasterKeys.Add(DecryptedPassPhrase)

            Return MasterKeys
        End If

    End Function

    ''' <summary>
    ''' converts accountID (and publicKey) from given address (0=AccountID; 1=PublicKey)
    ''' </summary>
    ''' <param name="Address">the address (e.g. (T)S-2222-2222-2222-22222(-0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ))</param>
    ''' <returns>List(Of String)("12345678901234567890","a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2")</returns>
    Function ConvertAddress(ByVal Address As String) As List(Of String)

        Dim ReturnList As List(Of String) = New List(Of String)

        Try

            If Address.Trim = "" Then
                Return ReturnList
            End If

            Dim PreFix As String = ""

            If Address.Contains("-") Then
                PreFix = Address.Remove(Address.IndexOf("-") + 1)
            Else
                Return ReturnList
            End If


            If PreFix.Contains(ClsSignumAPI._AddressPreFix) Then
                Address = Address.Substring(Address.IndexOf(PreFix) + PreFix.Length)
            End If

            Select Case CharCnt(Address, "-")
                Case 3

                    If IsReedSolomon(Address) Then
                        Dim AccID As ULong = GetAccountIDFromRS(Address)
                        ReturnList.Add(AccID.ToString)
                    End If

                Case 4

                    Dim PubKeyBase36 As String = Address.Substring(Address.LastIndexOf("-") + 1)
                    Address = Address.Remove(Address.IndexOf(PubKeyBase36) - 1)

                    If IsReedSolomon(Address) Then
                        Dim AccID As ULong = GetAccountIDFromRS(Address)
                        ReturnList.Add(AccID.ToString)

                        Dim PubKeyHex As String = ClsBase36.DecodeBase36ToHex(PubKeyBase36)
                        ReturnList.Add(PubKeyHex)
                    End If

            End Select

        Catch ex As Exception

            'ClsMsgs.MBox(ex.Message, "Error",,, ClsMsgs.Status.Erro)

            If GetINISetting(E_Setting.InfoOut, False) Then
                Dim Out As ClsOut = New ClsOut(Application.StartupPath)
                Out.ErrorLog2File(Application.ProductName + "-error in ModGlobalFunctions.vb -> ConvertAddress(): -> " + ex.Message)
            End If

        End Try

        Return ReturnList

    End Function


    Function IsReedSolomon(ByVal RSString As String) As Boolean

        If Not RSString.Length = 20 Then
            Return False
        End If

        Dim CharAry() As Char = RSString.ToUpper.ToCharArray

        For Each Chr As Char In CharAry
            Select Case Chr
                Case "-"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "A"c, "B"c, "C"c, "D"c, "E"c, "F"c, "G"c, "H"c, "J"c, "K"c, "L"c, "M"c, "N"c, "P"c, "Q"c, "R"c, "S"c, "T"c, "U"c, "V"c, "W"c, "X"c, "Y"c, "Z"c
                Case Else
                    Return False
            End Select

        Next

        Return True
    End Function
    Function CharCnt(ByVal Input As String, ByVal Search As String) As Integer

        Dim Cnter As Integer = 0
        For i As Integer = 0 To Input.Length - 1

            Dim Chr As String = Input.Substring(i, 1)

            If Chr = Search Then
                Cnter += 1
            End If
        Next

        Return Cnter

    End Function
    Function IsNumber(ByVal Input As String) As Boolean

        Dim CharAry() As Char = Input.ToUpper.ToCharArray

        For Each Chr As Char In CharAry
            Select Case Chr
                Case "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c
                Case Else
                    Return False
            End Select

        Next

        Return True

    End Function


#Region "PayPal Interactions"
    Function CheckPayPalAPI() As String

        Dim PPAPI As ClsPayPal = New ClsPayPal
        PPAPI.Client_ID = GetINISetting(E_Setting.PayPalAPIUser, "")
        PPAPI.Secret = GetINISetting(E_Setting.PayPalAPISecret, "")


        Dim PPOrderID As List(Of String) = PPAPI.GetAuthToken()

        If PPOrderID.Count > 0 Then
            If PPOrderID(0).Contains("<error>") Then
                Return GetStringBetween(PPOrderID(0), "<error>", "</error>")
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