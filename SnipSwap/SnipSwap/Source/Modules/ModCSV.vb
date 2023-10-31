
Option Strict On
Option Explicit On

Module ModCSV

    Dim CSVTool As ClsCSV = New ClsCSV(Application.StartupPath + "/cache.dat",, True, Application.ProductName)

#Region "SmartContract CSV Specials"
    Function ConvertCSVSmartContracts2StrucSmartContracts(ByVal CSVSmartContracts As List(Of List(Of String))) As List(Of SnipSwapForm.S_SmartContract)

        Try

            Dim New_CSV_SmartContractList As List(Of SnipSwapForm.S_SmartContract) = New List(Of SnipSwapForm.S_SmartContract)

            For Each ItemAry As List(Of String) In CSVSmartContracts

                If ItemAry.Count >= 2 Then
                    Dim T_SC As SnipSwapForm.S_SmartContract = New SnipSwapForm.S_SmartContract
                    T_SC.ID = Convert.ToUInt64(ItemAry(0))

                    T_SC.IsDEX_SC = Convert.ToBoolean(ItemAry(1))
                    New_CSV_SmartContractList.Add(T_SC)
                End If
            Next

            Return New_CSV_SmartContractList

        Catch ex As Exception
            Return New List(Of SnipSwapForm.S_SmartContract)
        End Try

    End Function
    Function GetSmartContractsFromCSV() As List(Of List(Of String))

        Try

            Dim SmartContract_CSV_FilePath As String = Application.StartupPath + "/" + "cache.dat"

            If IO.File.Exists(SmartContract_CSV_FilePath) Then
                Return CSVTool.RowList
            End If

            Return New List(Of List(Of String))

        Catch ex As Exception
            Return New List(Of List(Of String))
        End Try

    End Function
    Function GetDEXContractsFromCSV(Optional ByVal IsDEXContract As Boolean = True) As List(Of List(Of String))

        Dim SmartContractList As List(Of List(Of String)) = GetSmartContractsFromCSV()

        Dim DEXContractList As List(Of List(Of String)) = New List(Of List(Of String))

        For Each SmartContract As List(Of String) In SmartContractList

            If IsDEXContract Then
                If SmartContract(1) = IsDEXContract.ToString() Then
                    DEXContractList.Add(SmartContract)
                End If
            Else
                DEXContractList.Add(SmartContract)
            End If

        Next

        Return DEXContractList

    End Function
    Function SaveSmartContractsToCSV(Optional ByVal T_SmartContractList As List(Of SnipSwapForm.S_SmartContract) = Nothing, Optional ByVal T_MyOrdersList As List(Of ClsOrderSettings) = Nothing) As Boolean

        If T_MyOrdersList Is Nothing Then
            T_MyOrdersList = OrderSettingsBuffer
        End If

        Dim SmartContract_CSV_FilePath As String = Application.StartupPath + "/" + "cache.dat"

        Dim CSVSmartContractList As List(Of List(Of String))

        If T_SmartContractList Is Nothing Then
            CSVSmartContractList = GetSmartContractsFromCSV()
        Else
            CSVSmartContractList = ConvertSmartContractsToListList(T_SmartContractList)
        End If

        Dim CSVMyOrders As List(Of List(Of String)) = ConvertOrderSettingsToListList(T_MyOrdersList)
        Dim NuSmartContractCSV As List(Of List(Of String)) = New List(Of List(Of String))

        For Each T_SmartContract As List(Of String) In CSVSmartContractList

            Dim LineEntrys As List(Of String) = New List(Of String)({T_SmartContract(0), T_SmartContract(1), T_SmartContract(2)})
            For Each MyOrder As List(Of String) In CSVMyOrders

                If T_SmartContract(0) = MyOrder(0) Then
                    MyOrder.RemoveAt(0)
                    If Not MyOrder.Contains("DELETED") Then
                        LineEntrys.AddRange(MyOrder.ToArray)
                    End If
                    Exit For
                End If

            Next

            NuSmartContractCSV.Add(LineEntrys)
        Next

        If NuSmartContractCSV.Count > 0 Then

            If CSVMyOrders.Count = 0 Then

                For i As Integer = 0 To NuSmartContractCSV.Count - 1
                    Dim Nu_row As List(Of String) = NuSmartContractCSV(i)

                    For ii As Integer = 0 To CSVTool.RowList.Count - 1
                        Dim Old_row As List(Of String) = CSVTool.RowList(ii)

                        If Old_row(0) = Nu_row(0) Then

                            NuSmartContractCSV(i) = Old_row

                        End If
                    Next
                Next

            End If

            CSVTool.RowList = NuSmartContractCSV
            CSVTool.WriteCSV(ClsCSV.E_WriteMode.Create, True, Application.ProductName)
        End If

        Return True

    End Function

    Function ConvertSmartContractsToListList(ByVal T_SmartContractList As List(Of SnipSwapForm.S_SmartContract)) As List(Of List(Of String))

        If T_SmartContractList.Count = 0 Then
            Return New List(Of List(Of String))
        End If

        Dim CSV_SmartContractList As List(Of SnipSwapForm.S_SmartContract) = ConvertCSVSmartContracts2StrucSmartContracts(GetSmartContractsFromCSV())
        Dim New_CSV_SmartContractList As List(Of SnipSwapForm.S_SmartContract) = New List(Of SnipSwapForm.S_SmartContract)

        For Each NEW_SmartContract As SnipSwapForm.S_SmartContract In T_SmartContractList

            Dim NewSmartContract As Boolean = True
            For Each SmartContract As SnipSwapForm.S_SmartContract In CSV_SmartContractList

                If NEW_SmartContract.ID = SmartContract.ID Then
                    NewSmartContract = False
                    Exit For
                End If

            Next

            If NewSmartContract Then
                New_CSV_SmartContractList.Add(NEW_SmartContract)
            End If

        Next

        For Each CSV_SmartContract As SnipSwapForm.S_SmartContract In CSV_SmartContractList
            Dim T_SmartContract As SnipSwapForm.S_SmartContract = CSV_SmartContract

            For Each NEW_SmartContract As SnipSwapForm.S_SmartContract In T_SmartContractList
                If T_SmartContract.ID = NEW_SmartContract.ID Then

                    If T_SmartContract.IsDEX_SC = NEW_SmartContract.IsDEX_SC Then

                    Else
                        T_SmartContract.IsDEX_SC = NEW_SmartContract.IsDEX_SC
                    End If

                    If T_SmartContract.HistoryOrders = NEW_SmartContract.HistoryOrders Then

                    Else
                        T_SmartContract.HistoryOrders = NEW_SmartContract.HistoryOrders
                    End If

                End If
            Next

            New_CSV_SmartContractList.Add(T_SmartContract)

        Next

        Dim CSVList As List(Of List(Of String)) = New List(Of List(Of String))

        For Each T_SmartContract As SnipSwapForm.S_SmartContract In New_CSV_SmartContractList

            Dim HisOrd As String = ""

            If Not T_SmartContract.HistoryOrders Is Nothing Then
                HisOrd = T_SmartContract.HistoryOrders
            End If

            Dim LineArray As List(Of String) = New List(Of String)({T_SmartContract.ID.ToString, T_SmartContract.IsDEX_SC.ToString, HisOrd})
            CSVList.Add(LineArray)
        Next

        Return CSVList

    End Function

#End Region

#Region "MyOrders CSV Specials"

    Property OrderSettingsBuffer As List(Of ClsOrderSettings) = New List(Of ClsOrderSettings)
    Function GetOrderSettingsFromBuffer(ByVal TXID As ULong) As List(Of ClsOrderSettings)

        Dim T_OSList As List(Of ClsOrderSettings) = New List(Of ClsOrderSettings) ' GetOrderSettings(Order.FirstTransaction)

        For i As Integer = 0 To OrderSettingsBuffer.Count - 1
            Dim T_T_OS As ClsOrderSettings = OrderSettingsBuffer(i)
            If T_T_OS.TransactionID = TXID Then
                T_OSList.Add(T_T_OS)
                Exit For
            End If

        Next

        Return T_OSList

    End Function
    Function GetOrderSettings(Optional ByVal TX As String = "") As List(Of ClsOrderSettings)

        Try
            Dim T_OrderSettings As List(Of List(Of String)) = GetDEXContractsFromCSV()

            Dim New_CSV_OrderSettings As List(Of ClsOrderSettings) = New List(Of ClsOrderSettings)

            For Each ItemAry As List(Of String) In T_OrderSettings

                If Not TX.Trim = "" Then

                    If ItemAry(3).Trim = TX.Trim Then

                        '0=ATID, 1=ATRS, 2=SnipSwapAT, 3=OrderTX, 4=Type, 5=Paytype, 6=Infotext, 7=AutoSendInfotext, 8=AutoCompleteAT, 9=Status
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
                        T_OS.AutoCompleteSmartContract = Boolean.Parse(ItemAry(8))
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
                        T_OS.AutoCompleteSmartContract = Boolean.Parse(ItemAry(8))
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
        SaveSmartContractsToCSV(, T_OrderSettings)
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

                If NEW_OrderSetting.TransactionID = CSV_OrderSetting.TransactionID Then
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
                If T_OrderSetting.TransactionID = NEW_OrderSetting.TransactionID Then

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

                    If Not T_OrderSetting.AutoCompleteSmartContract = NEW_OrderSetting.AutoCompleteSmartContract Then
                        T_OrderSetting.AutoCompleteSmartContract = NEW_OrderSetting.AutoCompleteSmartContract
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

        For Each TOS As ClsOrderSettings In New_CSV_OrderSettingList
            Dim LineArray As List(Of String) = New List(Of String)({TOS.SmartContractID.ToString, TOS.TransactionID.ToString, TOS.Type, TOS.PaytypeString, TOS.Infotext, TOS.AutoSendInfotext.ToString, TOS.AutoCompleteSmartContract.ToString, TOS.Status})
            CSVList.Add(LineArray)
        Next

        Return CSVList

    End Function

    Function DelOrderSettings(ByVal SmartContractID As ULong) As Boolean

        For i As Integer = 0 To OrderSettingsBuffer.Count - 1

            Dim OrderSetting As ClsOrderSettings = OrderSettingsBuffer(i)

            If OrderSetting.SmartContractID = SmartContractID Then
                OrderSetting.Status = "DELETED"
                OrderSettingsBuffer(i) = OrderSetting
                Exit For
            End If

        Next

        SaveOrderSettingsToCSV(OrderSettingsBuffer)

        Return True

    End Function

#End Region

End Module