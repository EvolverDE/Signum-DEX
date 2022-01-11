
Option Strict On
Option Explicit On

Module ModCSV

    Dim CSVTool As ClsCSV = New ClsCSV(Application.StartupPath + "/cache.dat",, True, Application.ProductName) 'TODO: debug encrypt=false

#Region "AT CSV Specials"
    Function ConvertCSVATs2StrucATs(ByVal CSVATs As List(Of List(Of String))) As List(Of PFPForm.S_AT)

        Try

            Dim New_CSV_ATList As List(Of PFPForm.S_AT) = New List(Of PFPForm.S_AT)

            For Each ItemAry As List(Of String) In CSVATs

                If ItemAry.Count >= 2 Then
                    Dim T_AT As PFPForm.S_AT = New PFPForm.S_AT
                    T_AT.ID = Convert.ToUInt64(ItemAry(0))
                    'T_AT.ATRS = ItemAry(1)
                    T_AT.IsDEX_AT = Convert.ToBoolean(ItemAry(1))
                    New_CSV_ATList.Add(T_AT)
                End If
            Next

            Return New_CSV_ATList

        Catch ex As Exception
            Return New List(Of PFPForm.S_AT)
        End Try

    End Function
    Function GetATsFromCSV() As List(Of List(Of String))

        Try

            Dim AT_CSV_FilePath As String = Application.StartupPath + "/" + "cache.dat"

            If IO.File.Exists(AT_CSV_FilePath) Then
                'Dim CSV_ATList As CSVTool.CSVReader = New CSVTool.CSVReader(AT_CSV_FilePath,, False, Application.ProductName) 'TODO: debug encrypt=false
                'Return CSV_ATList.Lists

                Return CSVTool.RowList

            End If

            Return New List(Of List(Of String))

        Catch ex As Exception
            Return New List(Of List(Of String))
        End Try

    End Function
    Function GetDEXATsFromCSV() As List(Of List(Of String))

        Dim ATList As List(Of List(Of String)) = GetATsFromCSV()

        Dim DEXATList As List(Of List(Of String)) = New List(Of List(Of String))

        For Each AT As List(Of String) In ATList

            If AT(1) = "True" Then
                DEXATList.Add(AT)
            End If

        Next

        Return DEXATList

    End Function
    Function SaveATsToCSV(Optional ByVal T_ATList As List(Of PFPForm.S_AT) = Nothing, Optional ByVal T_MyOrdersList As List(Of ClsOrderSettings) = Nothing) As Boolean

        If IsNothing(T_MyOrdersList) Then
            T_MyOrdersList = OrderSettingsBuffer
        End If

        Dim AT_CSV_FilePath As String = Application.StartupPath + "/" + "cache.dat"

#Region "deprecated"

        'If T_ATList.Count = 0 Then
        '    Dim x As CSVTool.CSVWriter = New CSVTool.CSVWriter(AT_CSV_FilePath, New List(Of String()),, "create", False, Application.ProductName) 'TODO: debug encrypt=false
        '    PFPForm.CSVATList = New List(Of PFPForm.S_AT)
        '    Return True
        'End If

        'Dim CSV_ATList As List(Of PFPForm.S_AT) = ConvertCSVATs2StrucATs(GetATsFromCSV())
        'Dim New_CSV_ATList As List(Of PFPForm.S_AT) = New List(Of PFPForm.S_AT)

        'For Each NEW_AT As PFPForm.S_AT In T_ATList

        '    Dim NewAT As Boolean = True
        '    For Each AT As PFPForm.S_AT In CSV_ATList

        '        If NEW_AT.AT = AT.AT Then
        '            NewAT = False
        '            Exit For
        '        End If

        '    Next

        '    If NewAT Then
        '        New_CSV_ATList.Add(NEW_AT)
        '    End If

        'Next

        'For Each CSV_AT As PFPForm.S_AT In CSV_ATList
        '    Dim T_AT As PFPForm.S_AT = CSV_AT

        '    For Each NEW_AT As PFPForm.S_AT In T_ATList
        '        If T_AT.AT = NEW_AT.AT Then

        '            If T_AT.IsBLS_AT = NEW_AT.IsBLS_AT Then

        '            Else
        '                T_AT.IsBLS_AT = NEW_AT.IsBLS_AT
        '            End If

        '        End If
        '    Next

        '    New_CSV_ATList.Add(T_AT)

        'Next

#End Region

        Dim CSVATList As List(Of List(Of String))

        If IsNothing(T_ATList) Then
            CSVATList = GetATsFromCSV()
        Else
            CSVATList = ConvertATsToListList(T_ATList)
        End If

        'CSVATList.Insert(0, New List(Of String)({"ATID", "ATRS", "PFPAT",######### "OrderTX", "Type", "Paytype", "Infotext", "AutoSendInfotext", "AutoCompleteAT", "Status"}))

        Dim CSVMyOrders As List(Of List(Of String)) = ConvertOrderSettingsToListList(T_MyOrdersList)
        'CSVMyOrders.Insert(0, New List(Of String)({"AT",######### "TX", "Type", "Paytype", "Infotext", "AutoSendInfotext", "AutoCompleteAT", "Status"}))

        Dim NuATCSV As List(Of List(Of String)) = New List(Of List(Of String))

        'NuATCSV.Add(New List(Of String)({"ATID", "ATRS", "PFPAT", "OrderTX", "Type", "Paytype", "Infotext", "AutoSendInfotext", "AutoCompleteAT", "Status"}))

        For Each SAT As List(Of String) In CSVATList

            Dim LineEntrys As List(Of String) = New List(Of String)({SAT(0), SAT(1), SAT(2)}) ', SAT(3)
            For Each MyOrder As List(Of String) In CSVMyOrders

                If SAT(0) = MyOrder(0) Then
                    MyOrder.RemoveAt(0)
                    LineEntrys.AddRange(MyOrder.ToArray)
                    Exit For
                End If

            Next

            NuATCSV.Add(LineEntrys)
        Next

        If NuATCSV.Count > 0 Then
            'Dim x As CSVTool.CSVWriter = New CSVTool.CSVWriter(AT_CSV_FilePath, NuATCSV,, "create", False, Application.ProductName) 'TODO: debug encrypt=false
            CSVTool.RowList = NuATCSV
            CSVTool.WriteCSV(ClsCSV.E_WriteMode.Create, True, Application.ProductName)

        End If

        Return True

    End Function


    Function ConvertATsToListList(ByVal T_ATList As List(Of PFPForm.S_AT)) As List(Of List(Of String))

        If T_ATList.Count = 0 Then
            Return New List(Of List(Of String))
        End If

        Dim CSV_ATList As List(Of PFPForm.S_AT) = ConvertCSVATs2StrucATs(GetATsFromCSV())
        Dim New_CSV_ATList As List(Of PFPForm.S_AT) = New List(Of PFPForm.S_AT)

        For Each NEW_AT As PFPForm.S_AT In T_ATList

            Dim NewAT As Boolean = True
            For Each AT As PFPForm.S_AT In CSV_ATList

                If NEW_AT.ID = AT.ID Then
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
                If T_AT.ID = NEW_AT.ID Then

                    If T_AT.IsDEX_AT = NEW_AT.IsDEX_AT Then

                    Else
                        T_AT.IsDEX_AT = NEW_AT.IsDEX_AT
                    End If

                    If T_AT.HistoryOrders = NEW_AT.HistoryOrders Then

                    Else
                        T_AT.HistoryOrders = NEW_AT.HistoryOrders
                    End If

                End If
            Next

            New_CSV_ATList.Add(T_AT)

        Next

        Dim CSVList As List(Of List(Of String)) = New List(Of List(Of String))
        'CSVList.Add({"ATID", "ATRS", "PFPAT"})

        For Each SAT As PFPForm.S_AT In New_CSV_ATList

            Dim HisOrd As String = ""

            If Not IsNothing(SAT.HistoryOrders) Then
                HisOrd = SAT.HistoryOrders
            End If

            Dim LineArray As List(Of String) = New List(Of String)({SAT.ID.ToString, SAT.IsDEX_AT.ToString, HisOrd}) ', SAT.ATRS
            CSVList.Add(LineArray)
        Next

        Return CSVList

    End Function

#End Region

#Region "MyOrders CSV Specials"

    'Function GetOrderSettingsFromCSV() As List(Of List(Of String))

    '    Dim DEXAts As List(Of List(Of String)) = GetDEXATsFromCSV()


    '    Try

    '        Dim T_OS_CSV_FilePath As String = Application.StartupPath + "/" + "cache2.dat"

    '        If IO.File.Exists(T_OS_CSV_FilePath) Then
    '            Dim CSV_ATList As CSVTool.CSVReader = New CSVTool.CSVReader(T_OS_CSV_FilePath,, False, Application.ProductName) 'TODO: debug encrypt=false
    '            Return CSV_ATList.Lists
    '        End If

    '        Return New List(Of List(Of String))

    '    Catch ex As Exception
    '        Return New List(Of List(Of String))
    '    End Try

    'End Function

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
            Dim T_OrderSettings As List(Of List(Of String)) = GetDEXATsFromCSV()

            Dim New_CSV_OrderSettings As List(Of ClsOrderSettings) = New List(Of ClsOrderSettings)

            For Each ItemAry As List(Of String) In T_OrderSettings

                If Not TX.Trim = "" Then

                    If ItemAry(3).Trim = TX.Trim Then

                        '0=ATID, 1=ATRS, 2=PFPAT, 3=OrderTX, 4=Type, 5=Paytype, 6=Infotext, 7=AutoSendInfotext, 8=AutoCompleteAT, 9=Status
                        'Dim ATID As ULong = ULong.Parse(ItemAry(0))
                        'Dim T_TX As ULong = ULong.Parse(ItemAry(3))

                        'Dim T_Type As Boolean = True
                        'If Not ItemAry(4).Trim = "SellOrder" Then
                        '    T_Type = False
                        'End If

                        'Dim T_Status As ClsDEXContract.E_Status = ClsDEXContract.E_Status.ERROR_

                        'Dim T_Stadi As List(Of ClsDEXContract.E_Status) = New List(Of ClsDEXContract.E_Status)(System.Enum.GetValues(GetType(ClsDEXContract.E_Status)))
                        'For Each Status As ClsDEXContract.E_Status In T_Stadi
                        '    If ItemAry(9) = Status.ToString Then
                        '        T_Status = Status
                        '        Exit For
                        '    End If
                        'Next

                        Dim T_OS As ClsOrderSettings = New ClsOrderSettings(ItemAry(0), ItemAry(3), ItemAry(4), ItemAry(9))
                        'T_OS.AT = ItemAry(0)
                        'T_OS.TX = ItemAry(1)
                        'T_OS.Type = ItemAry(2)
                        T_OS.PaytypeString = ItemAry(5)
                        T_OS.Infotext = ItemAry(6)
                        T_OS.AutoSendInfotext = Boolean.Parse(ItemAry(7))
                        T_OS.AutoCompleteAT = Boolean.Parse(ItemAry(8))
                        'T_OS.Status = ItemAry(7)

                        New_CSV_OrderSettings.Add(T_OS)

                        Exit For
                    End If

                Else

                    If ItemAry.Count >= 10 Then

                        'Dim ATID As ULong = ULong.Parse(ItemAry(0))
                        'Dim T_TX As ULong = ULong.Parse(ItemAry(3))

                        'Dim T_Type As Boolean = True
                        'If Not ItemAry(4).Trim = "SellOrder" Then
                        '    T_Type = False
                        'End If

                        'Dim T_Status As ClsDEXContract.E_Status = ClsDEXContract.E_Status.ERROR_

                        'Dim T_Stadi = New List(Of ClsDEXContract.E_Status)(System.Enum.GetValues(GetType(ClsDEXContract.E_Status)))
                        'For Each Status As ClsDEXContract.E_Status In T_Stadi
                        '    If ItemAry(9) = Status.ToString Then
                        '        T_Status = Status
                        '        Exit For
                        '    End If
                        'Next

                        Dim T_OS As ClsOrderSettings = New ClsOrderSettings(ItemAry(0), ItemAry(3), ItemAry(4), ItemAry(9))
                        'T_OS.AT = ItemAry(0)
                        'T_OS.ATX = ItemAry(1)
                        'T_OS.Type = ItemAry(2)
                        T_OS.PaytypeString = ItemAry(5)
                        T_OS.Infotext = ItemAry(6)
                        T_OS.AutoSendInfotext = Boolean.Parse(ItemAry(7))
                        T_OS.AutoCompleteAT = Boolean.Parse(ItemAry(8))
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
    Function SaveOrderSettingsToCSV(ByVal T_OrderSettings As List(Of ClsOrderSettings)) As Boolean

        'Dim T_ATList As List(Of PFPForm.S_AT) = ConvertCSVATs2StrucATs(GetATsFromCSV())

        SaveATsToCSV(, T_OrderSettings)

#Region "deprecated"
        'Dim OrderSettings_CSV_FilePath As String = Application.StartupPath + "/" + "cache2.dat"


        '        If T_OrderSettings.Count = 0 Then
        '            'Dim x As CSVTool.CSVWriter = New CSVTool.CSVWriter(OrderSettings_CSV_FilePath, New List(Of String()),, "create", False, Application.ProductName) 'TODO: debug encrypt=false

        '            Return True

        '        End If

        '        Dim CSV_OrderSettings As List(Of ClsOrderSettings) = GetOrderSettings()
        '        Dim New_CSV_OrderSettingList As List(Of ClsOrderSettings) = New List(Of ClsOrderSettings)

        '#Region "Set new OrderSetting"
        '        For Each NEW_OrderSetting As ClsOrderSettings In T_OrderSettings

        '            Dim NewOS As Boolean = True
        '            For Each CSV_OrderSetting As ClsOrderSettings In CSV_OrderSettings

        '                If NEW_OrderSetting.TXID = CSV_OrderSetting.TXID Then
        '                    NewOS = False
        '                    Exit For
        '                End If

        '            Next

        '            If NewOS Then
        '                New_CSV_OrderSettingList.Add(NEW_OrderSetting)
        '            End If

        '        Next
        '#End Region

        '#Region "Refresh Old OrderSetting"

        '        For Each CSV_OrderSetting As ClsOrderSettings In CSV_OrderSettings
        '            Dim T_OrderSetting As ClsOrderSettings = CSV_OrderSetting

        '            For Each NEW_OrderSetting As ClsOrderSettings In T_OrderSettings
        '                If T_OrderSetting.TXID = NEW_OrderSetting.TXID Then

        '                    If Not T_OrderSetting.Type.Trim = NEW_OrderSetting.Type.Trim Then
        '                        T_OrderSetting.Type = NEW_OrderSetting.Type
        '                    End If

        '                    If Not T_OrderSetting.PaytypeString.Trim = NEW_OrderSetting.PaytypeString.Trim Then
        '                        T_OrderSetting.PaytypeString = NEW_OrderSetting.PaytypeString
        '                    End If

        '                    If Not T_OrderSetting.Infotext.Trim = NEW_OrderSetting.Infotext.Trim Then
        '                        T_OrderSetting.Infotext = NEW_OrderSetting.Infotext
        '                    End If

        '                    If Not T_OrderSetting.AutoSendInfotext = NEW_OrderSetting.AutoSendInfotext Then
        '                        T_OrderSetting.AutoSendInfotext = NEW_OrderSetting.AutoSendInfotext
        '                    End If

        '                    If Not T_OrderSetting.AutoCompleteAT = NEW_OrderSetting.AutoCompleteAT Then
        '                        T_OrderSetting.AutoCompleteAT = NEW_OrderSetting.AutoCompleteAT
        '                    End If

        '                    If Not T_OrderSetting.Status.Trim = NEW_OrderSetting.Status.Trim Then
        '                        T_OrderSetting.Status = NEW_OrderSetting.Status
        '                    End If

        '                    Exit For

        '                End If
        '            Next

        '            New_CSV_OrderSettingList.Add(T_OrderSetting)

        '        Next

        '#End Region


        'Dim CSVList As List(Of List(Of String)) = ConvertOrderSettingsToListList(T_OrderSettings) ' New List(Of String())
        ' CSVList.Insert(0, New List(Of String)({"AT", "TX", "Type", "Paytype", "Infotext", "AutoSendInfotext", "AutoCompleteAT", "Status"}))
        'For Each TOS As ClsOrderSettings In New_CSV_OrderSettingList
        '    If Not TOS.Status = "DELETED" Then
        '        Dim LineArray As String() = {TOS.ATID, TOS.TXID, TOS.Type, TOS.PaytypeString, TOS.Infotext, TOS.AutoSendInfotext.ToString, TOS.AutoCompleteAT.ToString, TOS.Status}
        '        CSVList.Add(LineArray)
        '    End If
        'Next

        'If CSVList.Count > 0 Then
        '    Dim x As CSVTool.CSVWriter = New CSVTool.CSVWriter(OrderSettings_CSV_FilePath, CSVList,, "create", False, Application.ProductName) 'TODO: debug encrypt=false
        'End If

#End Region

        Return True

    End Function


    Function ConvertOrderSettingsToListList(ByVal T_OrderSettings As List(Of ClsOrderSettings)) As List(Of List(Of String))

        If T_OrderSettings.Count = 0 Then
            Return New List(Of List(Of String))
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

        Dim CSVList As List(Of List(Of String)) = New List(Of List(Of String))
        'CSVList.Add({"AT", "TX", "Type", "Paytype", "Infotext", "AutoSendInfotext", "AutoCompleteAT", "Status"})
        For Each TOS As ClsOrderSettings In New_CSV_OrderSettingList
            If Not TOS.Status = "DELETED" Then
                Dim LineArray As List(Of String) = New List(Of String)({TOS.ATID.ToString, TOS.TXID.ToString, TOS.Type, TOS.PaytypeString, TOS.Infotext, TOS.AutoSendInfotext.ToString, TOS.AutoCompleteAT.ToString, TOS.Status})
                CSVList.Add(LineArray)
            End If
        Next

        Return CSVList

    End Function


    Function DelOrderSettings(ByVal ATID As ULong) As Boolean

        For Each OrderSetting As ClsOrderSettings In OrderSettingsBuffer

            If OrderSetting.ATID = ATID Then
                OrderSetting.Status = "DELETED"
                Exit For
            End If

        Next

        SaveOrderSettingsToCSV(OrderSettingsBuffer)

        Return True

    End Function

#End Region

End Module