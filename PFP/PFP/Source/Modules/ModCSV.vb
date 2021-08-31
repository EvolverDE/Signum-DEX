
Module ModCSV

#Region "AT CSV Specials"
    Function ConvertCSVATs2StrucATs(ByVal CSVATs As List(Of String())) As List(Of PFPForm.S_AT)

        Try

            Dim New_CSV_ATList As List(Of PFPForm.S_AT) = New List(Of PFPForm.S_AT)

            For Each ItemAry As String() In CSVATs

                If ItemAry.Length >= 2 Then
                    Dim T_AT As PFPForm.S_AT = New PFPForm.S_AT
                    T_AT.AT = ItemAry(0)
                    T_AT.ATRS = ItemAry(1)
                    T_AT.IsBLS_AT = ItemAry(2)
                    New_CSV_ATList.Add(T_AT)
                End If
            Next

            Return New_CSV_ATList

        Catch ex As Exception
            Return New List(Of PFPForm.S_AT)
        End Try

    End Function
    Function GetATsFromCSV() As List(Of String())

        Try

            Dim AT_CSV_FilePath As String = Application.StartupPath + "\cache.dat"

            If IO.File.Exists(AT_CSV_FilePath) Then
                Dim CSV_ATList As CSVTool.CSVReader = New CSVTool.CSVReader(AT_CSV_FilePath,, True, Application.ProductName)
                Return CSV_ATList.Lists
            End If

            Return New List(Of String())

        Catch ex As Exception
            Return New List(Of String())
        End Try

    End Function
    Function GetDEXATsFromCSV() As List(Of String())

        Dim ATList As List(Of String()) = GetATsFromCSV()

        Dim DEXATList As List(Of String()) = New List(Of String())

        For Each AT In ATList

            If AT(2) = "True" Then
                DEXATList.Add(AT)
            End If

        Next

        Return DEXATList


    End Function
    Function SaveATsToCSV(T_ATList As List(Of PFPForm.S_AT)) As Boolean

        Dim AT_CSV_FilePath As String = Application.StartupPath + "\cache.dat"

        If T_ATList.Count = 0 Then
            Dim x As CSVTool.CSVWriter = New CSVTool.CSVWriter(AT_CSV_FilePath, New List(Of String()),, "create", True, Application.ProductName)
            PFPForm.CSVATList = New List(Of PFPForm.S_AT)

            Return True

        End If

        Dim CSV_ATList As List(Of PFPForm.S_AT) = ConvertCSVATs2StrucATs(GetATsFromCSV())
        Dim New_CSV_ATList As List(Of PFPForm.S_AT) = New List(Of PFPForm.S_AT)

        For Each NEW_AT As PFPForm.S_AT In T_ATList

            Dim NewAT As Boolean = True
            For Each AT As PFPForm.S_AT In CSV_ATList

                If NEW_AT.AT = AT.AT Then
                    NewAT = False
                    Exit For
                End If

            Next

            If NewAT Then
                New_CSV_ATList.Add(NEW_AT)
            End If

        Next


        For Each CSV_AT As PFPForm.S_AT In CSV_ATList
            Dim T_AT As PFPForm.S_AT = CSV_AT

            For Each NEW_AT As PFPForm.S_AT In T_ATList
                If T_AT.AT = NEW_AT.AT Then

                    If T_AT.IsBLS_AT = NEW_AT.IsBLS_AT Then

                    Else
                        T_AT.IsBLS_AT = NEW_AT.IsBLS_AT
                    End If

                End If
            Next

            New_CSV_ATList.Add(T_AT)

        Next


        Dim CSVList As List(Of String()) = New List(Of String())
        CSVList.Add({"ATID", "ATRS", "PFPAT"})
        For Each SAT As PFPForm.S_AT In New_CSV_ATList
            Dim LineArray As String() = {SAT.AT, SAT.ATRS, SAT.IsBLS_AT.ToString}
            CSVList.Add(LineArray)
        Next

        If CSVList.Count > 0 Then
            Dim x As CSVTool.CSVWriter = New CSVTool.CSVWriter(AT_CSV_FilePath, CSVList,, "create", True, Application.ProductName)
        End If

        Return True

    End Function

#End Region

#Region "MyOrders CSV Specials"

    Function GetOrderSettingsFromCSV() As List(Of String())

        Try

            Dim T_OS_CSV_FilePath As String = Application.StartupPath + "\cache2.dat"

            If IO.File.Exists(T_OS_CSV_FilePath) Then
                Dim CSV_ATList As CSVTool.CSVReader = New CSVTool.CSVReader(T_OS_CSV_FilePath,, False, Application.ProductName) 'TODO: debug encrypt=false
                Return CSV_ATList.Lists
            End If

            Return New List(Of String())

        Catch ex As Exception
            Return New List(Of String())
        End Try

    End Function

    Property OrderSettingsBuffer As List(Of ClsOrderSettings) = New List(Of ClsOrderSettings)

    Function GetOrderSettingsFromBuffer(ByVal TXID As ULong) As List(Of ClsOrderSettings)

        Dim T_OSList As List(Of ClsOrderSettings) = New List(Of ClsOrderSettings) ' GetOrderSettings(Order.FirstTransaction)

        For i As Integer = 0 To OrderSettingsBuffer.Count - 1
            Dim T_T_OS As ClsOrderSettings = OrderSettingsBuffer(i)

            If T_T_OS.TXID = TXID Then
                T_OSList.Add(T_T_OS)
                Exit For
            End If

        Next

        Return T_OSList

    End Function

    Function GetOrderSettings(Optional ByVal TX As String = "") As List(Of ClsOrderSettings)

        Try
            Dim T_OrderSettings As List(Of String()) = GetOrderSettingsFromCSV()

            Dim New_CSV_OrderSettings As List(Of ClsOrderSettings) = New List(Of ClsOrderSettings)

            For Each ItemAry As String() In T_OrderSettings

                If Not TX.Trim = "" Then

                    If ItemAry(0).Trim = TX.Trim Then

                        Dim T_OS As ClsOrderSettings = New ClsOrderSettings(ItemAry(0), ItemAry(1), ItemAry(2), ItemAry(7))
                        'T_OS.AT = ItemAry(0)
                        'T_OS.TX = ItemAry(1)
                        'T_OS.Type = ItemAry(2)
                        T_OS.PaytypeString = ItemAry(3)
                        T_OS.Infotext = ItemAry(4)
                        T_OS.AutoSendInfotext = CBool(ItemAry(5))
                        T_OS.AutoCompleteAT = CBool(ItemAry(6))
                        'T_OS.Status = ItemAry(7)

                        New_CSV_OrderSettings.Add(T_OS)

                        Exit For
                    End If

                Else

                    If ItemAry.Length >= 7 Then
                        Dim T_OS As ClsOrderSettings = New ClsOrderSettings(ItemAry(0), ItemAry(1), ItemAry(2), ItemAry(7))
                        'T_OS.AT = ItemAry(0)
                        'T_OS.ATX = ItemAry(1)
                        'T_OS.Type = ItemAry(2)
                        T_OS.PaytypeString = ItemAry(3)
                        T_OS.Infotext = ItemAry(4)
                        T_OS.AutoSendInfotext = ItemAry(5)
                        T_OS.AutoCompleteAT = ItemAry(6)
                        'T_OS.Status = ItemAry(7)

                        New_CSV_OrderSettings.Add(T_OS)
                    End If

                End If

            Next

            Return New_CSV_OrderSettings

        Catch ex As Exception
            Return New List(Of ClsOrderSettings)
        End Try

    End Function



    Function SaveOrderSettingsToCSV(T_OrderSettings As List(Of ClsOrderSettings)) As Boolean

        Dim OrderSettings_CSV_FilePath As String = Application.StartupPath + "\cache2.dat"

        If T_OrderSettings.Count = 0 Then
            Dim x As CSVTool.CSVWriter = New CSVTool.CSVWriter(OrderSettings_CSV_FilePath, New List(Of String()),, "create", False, Application.ProductName) 'TODO: debug encrypt=false

            Return True

        End If

        Dim CSV_OrderSettings As List(Of ClsOrderSettings) = GetOrderSettings()
        Dim New_CSV_OrderSettingList As List(Of ClsOrderSettings) = New List(Of ClsOrderSettings)

#Region "Set new OrderSetting"
        For Each NEW_OrderSetting As ClsOrderSettings In T_OrderSettings

            Dim NewOS As Boolean = True
            For Each CSV_OrderSetting As ClsOrderSettings In CSV_OrderSettings

                If NEW_OrderSetting.TXID = CSV_OrderSetting.TXID Then
                    NewOS = False
                    Exit For
                End If

            Next

            If NewOS Then
                New_CSV_OrderSettingList.Add(NEW_OrderSetting)
            End If

        Next
#End Region

#Region "Refresh Old OrderSetting"

        For Each CSV_OrderSetting As ClsOrderSettings In CSV_OrderSettings
            Dim T_OrderSetting As ClsOrderSettings = CSV_OrderSetting

            For Each NEW_OrderSetting As ClsOrderSettings In T_OrderSettings
                If T_OrderSetting.TXID = NEW_OrderSetting.TXID Then

                    If Not T_OrderSetting.Type.Trim = NEW_OrderSetting.Type.Trim Then
                        T_OrderSetting.Type = NEW_OrderSetting.Type
                    End If

                    If Not T_OrderSetting.PaytypeString.Trim = NEW_OrderSetting.PaytypeString.Trim Then
                        T_OrderSetting.PaytypeString = NEW_OrderSetting.PaytypeString
                    End If

                    If Not T_OrderSetting.Infotext.Trim = NEW_OrderSetting.Infotext.Trim Then
                        T_OrderSetting.Infotext = NEW_OrderSetting.Infotext
                    End If

                    If Not T_OrderSetting.AutoSendInfotext = NEW_OrderSetting.AutoSendInfotext Then
                        T_OrderSetting.AutoSendInfotext = NEW_OrderSetting.AutoSendInfotext
                    End If

                    If Not T_OrderSetting.AutoCompleteAT = NEW_OrderSetting.AutoCompleteAT Then
                        T_OrderSetting.AutoCompleteAT = NEW_OrderSetting.AutoCompleteAT
                    End If

                    If Not T_OrderSetting.Status.Trim = NEW_OrderSetting.Status.Trim Then
                        T_OrderSetting.Status = NEW_OrderSetting.Status
                    End If

                    Exit For

                End If
            Next

            New_CSV_OrderSettingList.Add(T_OrderSetting)

        Next

#End Region

        Dim CSVList As List(Of String()) = New List(Of String())
        CSVList.Add({"AT", "TX", "Type", "Paytype", "Infotext", "AutoSendInfotext", "AutoCompleteAT", "Status"})
        For Each TOS As ClsOrderSettings In New_CSV_OrderSettingList
            If Not TOS.Status = "DELETED" Then
                Dim LineArray As String() = {TOS.ATID, TOS.TXID, TOS.Type, TOS.PaytypeString, TOS.Infotext, TOS.AutoSendInfotext.ToString, TOS.AutoCompleteAT.ToString, TOS.Status}
                CSVList.Add(LineArray)
            End If
        Next

        If CSVList.Count > 0 Then
            Dim x As CSVTool.CSVWriter = New CSVTool.CSVWriter(OrderSettings_CSV_FilePath, CSVList,, "create", False, Application.ProductName) 'TODO: debug encrypt=false
        End If

        Return True

    End Function



    'Function DelOrderSettings(ByVal TX As String) As Boolean

    '    Dim OrderSettings_CSV_FilePath As String = Application.StartupPath + "\cache2.dat"
    '    Dim T_CSV_OrderSettings As List(Of ClsOrderSettings) = GetOrderSettings(TX)

    '    If T_CSV_OrderSettings.Count > 0 Then

    '        Dim CSV_OrderSettings As List(Of ClsOrderSettings) = GetOrderSettings()

    '        Dim RefreshFile As Boolean = False

    '        Dim CSVList As List(Of String()) = New List(Of String())
    '        CSVList.Add({"TX", "Type", "Paytype", "Infotext", "AutoSendInfotext", "AutoCompleteAT", "Status"})
    '        For Each TOS As ClsOrderSettings In CSV_OrderSettings
    '            If TOS.TX = TX Then
    '                RefreshFile = True
    '                Continue For
    '            End If
    '            Dim LineArray As String() = {TOS.TX, TOS.Type, TOS.PaytypeString, TOS.Infotext, TOS.AutoSendInfotext.ToString, TOS.AutoCompleteAT.ToString, TOS.Status}
    '            CSVList.Add(LineArray)
    '        Next

    '        If RefreshFile Then
    '            If CSVList.Count > 0 Then
    '                Dim x As CSVTool.CSVWriter = New CSVTool.CSVWriter(OrderSettings_CSV_FilePath, CSVList,, "create", False, Application.ProductName) 'TODO: debug encrypt=false
    '            End If
    '        End If

    '    End If

    '    Return True

    'End Function

#End Region

End Module