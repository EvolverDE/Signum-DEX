﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmDevelope
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmDevelope))
        Me.TestTabControl = New System.Windows.Forms.TabControl()
        Me.TPgConvertings = New System.Windows.Forms.TabPage()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.TBTestHashHEX = New System.Windows.Forms.TextBox()
        Me.Label58 = New System.Windows.Forms.Label()
        Me.TBTestHashUL4 = New System.Windows.Forms.TextBox()
        Me.Label54 = New System.Windows.Forms.Label()
        Me.TBTestHashUL2 = New System.Windows.Forms.TextBox()
        Me.Label55 = New System.Windows.Forms.Label()
        Me.TBTestHashUL1 = New System.Windows.Forms.TextBox()
        Me.Label56 = New System.Windows.Forms.Label()
        Me.TBTestHashUL3 = New System.Windows.Forms.TextBox()
        Me.Label57 = New System.Windows.Forms.Label()
        Me.TBTestKeyUL4 = New System.Windows.Forms.TextBox()
        Me.Label53 = New System.Windows.Forms.Label()
        Me.TBTestKeyUL2 = New System.Windows.Forms.TextBox()
        Me.Label52 = New System.Windows.Forms.Label()
        Me.TBTestKeyUL1 = New System.Windows.Forms.TextBox()
        Me.Label51 = New System.Windows.Forms.Label()
        Me.TBTestKeyUL3 = New System.Windows.Forms.TextBox()
        Me.Label50 = New System.Windows.Forms.Label()
        Me.BtTestHash = New System.Windows.Forms.Button()
        Me.TBTestHashULHEX = New System.Windows.Forms.TextBox()
        Me.Label49 = New System.Windows.Forms.Label()
        Me.TBTestKeyHEX = New System.Windows.Forms.TextBox()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.TBTestKeyString = New System.Windows.Forms.TextBox()
        Me.Label47 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.BtTestRecursiveXMLSearch = New System.Windows.Forms.Button()
        Me.TBTestRecursiveXMLSearch = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.TBTestXMLOutput = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.BtTestJSONToXML = New System.Windows.Forms.Button()
        Me.TBTestJSONInput = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.BtTestCreateCollWord = New System.Windows.Forms.Button()
        Me.TBTestCollWordOutput = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TBTestCollWordInput = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.BtTestTimeConvert = New System.Windows.Forms.Button()
        Me.TBTestTime = New System.Windows.Forms.TextBox()
        Me.TestGrpBoxConvertings = New System.Windows.Forms.GroupBox()
        Me.BtTestHex2ULng = New System.Windows.Forms.Button()
        Me.TBTestConvert = New System.Windows.Forms.TextBox()
        Me.BtTestConvert = New System.Windows.Forms.Button()
        Me.BtTestConvert2 = New System.Windows.Forms.Button()
        Me.BtTestDatStr2ULngList = New System.Windows.Forms.Button()
        Me.TPgINITools = New System.Windows.Forms.TabPage()
        Me.TestGrpBoxINITests = New System.Windows.Forms.GroupBox()
        Me.TBTestSetTXINI = New System.Windows.Forms.TextBox()
        Me.BtTestSetTXINI = New System.Windows.Forms.Button()
        Me.BtTestGetTXINI = New System.Windows.Forms.Button()
        Me.BtTestDelTXINI = New System.Windows.Forms.Button()
        Me.TBTestGetTXINI = New System.Windows.Forms.TextBox()
        Me.TBTestDelTXINI = New System.Windows.Forms.TextBox()
        Me.TPgPayPal = New System.Windows.Forms.TabPage()
        Me.SplitContainer10 = New System.Windows.Forms.SplitContainer()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.BtTestPPPayout = New System.Windows.Forms.Button()
        Me.TBTestPPPONote = New System.Windows.Forms.TextBox()
        Me.TBTestPPPOCurrency = New System.Windows.Forms.TextBox()
        Me.TBTestPPPOAmount = New System.Windows.Forms.TextBox()
        Me.TBTestPPPORecipient = New System.Windows.Forms.TextBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.CoBxTestPPCurrency = New System.Windows.Forms.ComboBox()
        Me.TBTestPPPrice = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.TBTestPPXAmount = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.TBTestPPXItem = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.BtTestCreatePPOrder = New System.Windows.Forms.Button()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.BtTestPPGetAllTX = New System.Windows.Forms.Button()
        Me.BtTestPPGetTXWithNote = New System.Windows.Forms.Button()
        Me.TBTestPPTXNote = New System.Windows.Forms.TextBox()
        Me.BtTestPPCheckAPI = New System.Windows.Forms.Button()
        Me.LiBoPayPalComs = New System.Windows.Forms.ListBox()
        Me.TPgAT_SC_Comm = New System.Windows.Forms.TabPage()
        Me.RTBTestDebug = New System.Windows.Forms.RichTextBox()
        Me.TestGrpBxATCom = New System.Windows.Forms.GroupBox()
        Me.NUDTestAmount = New System.Windows.Forms.NumericUpDown()
        Me.NUDTestXAmount = New System.Windows.Forms.NumericUpDown()
        Me.CoBxTestATComATID = New System.Windows.Forms.ComboBox()
        Me.TBTestXItem = New System.Windows.Forms.TextBox()
        Me.TBTestPP = New System.Windows.Forms.TextBox()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.TB_TestCreateWithResponder = New System.Windows.Forms.TextBox()
        Me.BtTestCreateWithResponder = New System.Windows.Forms.Button()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.BtTestCheckCloseDispute = New System.Windows.Forms.Button()
        Me.BtTestAppeal = New System.Windows.Forms.Button()
        Me.BtTestOpenDispute = New System.Windows.Forms.Button()
        Me.BtTestDeActivateDeniability = New System.Windows.Forms.Button()
        Me.NUDTestCollateral = New System.Windows.Forms.NumericUpDown()
        Me.BtTestFinish = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BtTestCreate = New System.Windows.Forms.Button()
        Me.LabAmount = New System.Windows.Forms.Label()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.ChBxSellOrder = New System.Windows.Forms.CheckBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.BtTestAccept = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.TBTestResponder = New System.Windows.Forms.TextBox()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.BtTestInjectResponder = New System.Windows.Forms.Button()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.TBTestChainSwapHashLong3 = New System.Windows.Forms.TextBox()
        Me.Label46 = New System.Windows.Forms.Label()
        Me.TBTestChainSwapHashLong2 = New System.Windows.Forms.TextBox()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.TBTestChainSwapHashLong1 = New System.Windows.Forms.TextBox()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.BtTestChainSwapKeyToHash = New System.Windows.Forms.Button()
        Me.TBTestChainSwapKey = New System.Windows.Forms.TextBox()
        Me.BtTestInjectChainSwapHash = New System.Windows.Forms.Button()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.BtCSKConvertback = New System.Windows.Forms.Button()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.TBTestChainSwapKeyULong3 = New System.Windows.Forms.TextBox()
        Me.TBTestChainSwapKeyULong2 = New System.Windows.Forms.TextBox()
        Me.TBTestChainSwapKeyULong1 = New System.Windows.Forms.TextBox()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.BtTestFinishWithChainSwapKey = New System.Windows.Forms.Button()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.NUDTestMediateAmount = New System.Windows.Forms.NumericUpDown()
        Me.BtTestMediateDispute = New System.Windows.Forms.Button()
        Me.BtExport = New System.Windows.Forms.Button()
        Me.BtTestLoadDEXContract = New System.Windows.Forms.Button()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.Label40 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.LVTestDEXContractBasic = New System.Windows.Forms.ListView()
        Me.ColumnHeader13 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader14 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.LVTestCurrentOrder = New System.Windows.Forms.ListView()
        Me.ColumnHeader9 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader12 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TPgMultithreadings = New System.Windows.Forms.TabPage()
        Me.TestGrpBoxMultithreadings = New System.Windows.Forms.GroupBox()
        Me.SplitContainer9 = New System.Windows.Forms.SplitContainer()
        Me.LabActiveNodes = New System.Windows.Forms.Label()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.ChBxAutoRefreshMultiThreads = New System.Windows.Forms.CheckBox()
        Me.BtTestMultiRefresh = New System.Windows.Forms.Button()
        Me.BtTestExit = New System.Windows.Forms.Button()
        Me.SplitContainer12 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer13 = New System.Windows.Forms.SplitContainer()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.LVTestMulti = New System.Windows.Forms.ListView()
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SplitContainer14 = New System.Windows.Forms.SplitContainer()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.LVActiveNodes = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader15 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TPgTCPAPI = New System.Windows.Forms.TabPage()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.ChBxTestTCPAPIEnable = New System.Windows.Forms.CheckBox()
        Me.ChBxTestTCPAPIShowStatus = New System.Windows.Forms.CheckBox()
        Me.LiBoTCPAPIStatus = New System.Windows.Forms.ListBox()
        Me.TPgMultiinvokes = New System.Windows.Forms.TabPage()
        Me.LVTestMultiInvoke = New System.Windows.Forms.ListView()
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader8 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader10 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader11 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.BtTestMultiInvokeSetInLV = New System.Windows.Forms.Button()
        Me.TPgDEXNET = New System.Windows.Forms.TabPage()
        Me.SplitContainer6 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.LiBoDEXNETPublicKeys = New System.Windows.Forms.ListBox()
        Me.Label63 = New System.Windows.Forms.Label()
        Me.ChBxDEXNETEncryptMsg = New System.Windows.Forms.CheckBox()
        Me.BtTestBroadcastMsg = New System.Windows.Forms.Button()
        Me.TBTestRecipientPubKey = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TBTestBroadcastMessage = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.BtTestConnect = New System.Windows.Forms.Button()
        Me.TBTestPeerPort = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TBTestNewPeer = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ChBxTestDEXNETShowStatus = New System.Windows.Forms.CheckBox()
        Me.ChBxTestDEXNETEnable = New System.Windows.Forms.CheckBox()
        Me.LiBoDEXNETStatus = New System.Windows.Forms.ListBox()
        Me.CMSTestLiBoDEXNETStatus = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ClearEntryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SplitContainer7 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer4 = New System.Windows.Forms.SplitContainer()
        Me.BtTestAddRelKey = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TBTestAddRelKey = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.LiBoTestRelKeys = New System.Windows.Forms.ListBox()
        Me.SplitContainer5 = New System.Windows.Forms.SplitContainer()
        Me.BtTestRefreshLiBoRelMsgs = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.LiBoTestRelMsgs = New System.Windows.Forms.ListBox()
        Me.SplitContainer8 = New System.Windows.Forms.SplitContainer()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.LiBoTestPeers = New System.Windows.Forms.ListBox()
        Me.TPgBitcoin = New System.Windows.Forms.TabPage()
        Me.SplitContainer15 = New System.Windows.Forms.SplitContainer()
        Me.TBTestBitcoinAddress = New System.Windows.Forms.TextBox()
        Me.GroupBox12 = New System.Windows.Forms.GroupBox()
        Me.BtTestBitcoinScanBlocks = New System.Windows.Forms.Button()
        Me.Label62 = New System.Windows.Forms.Label()
        Me.GroupBox11 = New System.Windows.Forms.GroupBox()
        Me.BtTestBitcoinGetTransaction = New System.Windows.Forms.Button()
        Me.BtTestBitcoinGetRawTransaction = New System.Windows.Forms.Button()
        Me.Label61 = New System.Windows.Forms.Label()
        Me.TBTestBitcoinTransactionID = New System.Windows.Forms.TextBox()
        Me.GroupBox10 = New System.Windows.Forms.GroupBox()
        Me.BtTestBitcoinGetReceivedByAddress = New System.Windows.Forms.Button()
        Me.BtTestBitcoinListUnspent = New System.Windows.Forms.Button()
        Me.BtTestBitcoinGetDescriptorInfo = New System.Windows.Forms.Button()
        Me.BtTestBitcoinImportDescriptor = New System.Windows.Forms.Button()
        Me.Label59 = New System.Windows.Forms.Label()
        Me.BtTestBitcoinGetUTXOSet = New System.Windows.Forms.Button()
        Me.GroupBox9 = New System.Windows.Forms.GroupBox()
        Me.BtTestBitcoionLoadWallet = New System.Windows.Forms.Button()
        Me.BtTestBitcoinWalletName = New System.Windows.Forms.Button()
        Me.Label60 = New System.Windows.Forms.Label()
        Me.TBTestBitcoinWalletName = New System.Windows.Forms.TextBox()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.BtTestBitcoinGetBalance = New System.Windows.Forms.Button()
        Me.BtTestBitcoinGetBalances = New System.Windows.Forms.Button()
        Me.BtTestBitcoinGetInfo = New System.Windows.Forms.Button()
        Me.BtTestBitcoinGetWalletInfo = New System.Windows.Forms.Button()
        Me.RTBTestBitcoin = New System.Windows.Forms.RichTextBox()
        Me.DevTimer = New System.Windows.Forms.Timer(Me.components)
        Me.SplitContainer11 = New System.Windows.Forms.SplitContainer()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.TBTestMyPassPhrase = New System.Windows.Forms.TextBox()
        Me.TBTestMyAgreeKey = New System.Windows.Forms.TextBox()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.TBTestMySignKey = New System.Windows.Forms.TextBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.TBTestMyPublicKey = New System.Windows.Forms.TextBox()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.TPgSignumTX = New System.Windows.Forms.TabPage()
        Me.Label64 = New System.Windows.Forms.Label()
        Me.TBTestSigTX_TXID = New System.Windows.Forms.TextBox()
        Me.TBTestSigTX_PassPhrase = New System.Windows.Forms.TextBox()
        Me.Label65 = New System.Windows.Forms.Label()
        Me.BtTestSigTX_ReadTX = New System.Windows.Forms.Button()
        Me.LVTestSigTX = New System.Windows.Forms.ListView()
        Me.ColumnHeader16 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader17 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.BtTestSigTX_Decrypt = New System.Windows.Forms.Button()
        Me.Label66 = New System.Windows.Forms.Label()
        Me.RTBTestSigTX = New System.Windows.Forms.RichTextBox()
        Me.TestTabControl.SuspendLayout()
        Me.TPgConvertings.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.TestGrpBoxConvertings.SuspendLayout()
        Me.TPgINITools.SuspendLayout()
        Me.TestGrpBoxINITests.SuspendLayout()
        Me.TPgPayPal.SuspendLayout()
        CType(Me.SplitContainer10, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer10.Panel1.SuspendLayout()
        Me.SplitContainer10.Panel2.SuspendLayout()
        Me.SplitContainer10.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.TPgAT_SC_Comm.SuspendLayout()
        Me.TestGrpBxATCom.SuspendLayout()
        CType(Me.NUDTestAmount, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUDTestXAmount, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel4.SuspendLayout()
        CType(Me.NUDTestCollateral, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.Panel6.SuspendLayout()
        Me.Panel7.SuspendLayout()
        CType(Me.NUDTestMediateAmount, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TPgMultithreadings.SuspendLayout()
        Me.TestGrpBoxMultithreadings.SuspendLayout()
        CType(Me.SplitContainer9, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer9.Panel1.SuspendLayout()
        Me.SplitContainer9.Panel2.SuspendLayout()
        Me.SplitContainer9.SuspendLayout()
        CType(Me.SplitContainer12, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer12.Panel1.SuspendLayout()
        Me.SplitContainer12.Panel2.SuspendLayout()
        Me.SplitContainer12.SuspendLayout()
        CType(Me.SplitContainer13, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer13.Panel1.SuspendLayout()
        Me.SplitContainer13.Panel2.SuspendLayout()
        Me.SplitContainer13.SuspendLayout()
        CType(Me.SplitContainer14, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer14.Panel1.SuspendLayout()
        Me.SplitContainer14.Panel2.SuspendLayout()
        Me.SplitContainer14.SuspendLayout()
        Me.TPgTCPAPI.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TPgMultiinvokes.SuspendLayout()
        Me.TPgDEXNET.SuspendLayout()
        CType(Me.SplitContainer6, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer6.Panel1.SuspendLayout()
        Me.SplitContainer6.Panel2.SuspendLayout()
        Me.SplitContainer6.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.CMSTestLiBoDEXNETStatus.SuspendLayout()
        CType(Me.SplitContainer7, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer7.Panel1.SuspendLayout()
        Me.SplitContainer7.Panel2.SuspendLayout()
        Me.SplitContainer7.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer4.Panel1.SuspendLayout()
        Me.SplitContainer4.Panel2.SuspendLayout()
        Me.SplitContainer4.SuspendLayout()
        CType(Me.SplitContainer5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer5.Panel1.SuspendLayout()
        Me.SplitContainer5.Panel2.SuspendLayout()
        Me.SplitContainer5.SuspendLayout()
        CType(Me.SplitContainer8, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer8.Panel1.SuspendLayout()
        Me.SplitContainer8.Panel2.SuspendLayout()
        Me.SplitContainer8.SuspendLayout()
        Me.TPgBitcoin.SuspendLayout()
        CType(Me.SplitContainer15, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer15.Panel1.SuspendLayout()
        Me.SplitContainer15.Panel2.SuspendLayout()
        Me.SplitContainer15.SuspendLayout()
        Me.GroupBox12.SuspendLayout()
        Me.GroupBox11.SuspendLayout()
        Me.GroupBox10.SuspendLayout()
        Me.GroupBox9.SuspendLayout()
        Me.GroupBox8.SuspendLayout()
        CType(Me.SplitContainer11, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer11.Panel1.SuspendLayout()
        Me.SplitContainer11.Panel2.SuspendLayout()
        Me.SplitContainer11.SuspendLayout()
        Me.TPgSignumTX.SuspendLayout()
        Me.SuspendLayout()
        '
        'TestTabControl
        '
        Me.TestTabControl.Controls.Add(Me.TPgAT_SC_Comm)
        Me.TestTabControl.Controls.Add(Me.TPgSignumTX)
        Me.TestTabControl.Controls.Add(Me.TPgMultithreadings)
        Me.TestTabControl.Controls.Add(Me.TPgDEXNET)
        Me.TestTabControl.Controls.Add(Me.TPgTCPAPI)
        Me.TestTabControl.Controls.Add(Me.TPgBitcoin)
        Me.TestTabControl.Controls.Add(Me.TPgPayPal)
        Me.TestTabControl.Controls.Add(Me.TPgMultiinvokes)
        Me.TestTabControl.Controls.Add(Me.TPgINITools)
        Me.TestTabControl.Controls.Add(Me.TPgConvertings)
        Me.TestTabControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestTabControl.Location = New System.Drawing.Point(0, 0)
        Me.TestTabControl.Name = "TestTabControl"
        Me.TestTabControl.SelectedIndex = 0
        Me.TestTabControl.Size = New System.Drawing.Size(1240, 712)
        Me.TestTabControl.TabIndex = 39
        '
        'TPgConvertings
        '
        Me.TPgConvertings.Controls.Add(Me.GroupBox4)
        Me.TPgConvertings.Controls.Add(Me.GroupBox2)
        Me.TPgConvertings.Controls.Add(Me.GroupBox1)
        Me.TPgConvertings.Controls.Add(Me.GroupBox5)
        Me.TPgConvertings.Controls.Add(Me.TestGrpBoxConvertings)
        Me.TPgConvertings.Location = New System.Drawing.Point(4, 22)
        Me.TPgConvertings.Name = "TPgConvertings"
        Me.TPgConvertings.Padding = New System.Windows.Forms.Padding(3)
        Me.TPgConvertings.Size = New System.Drawing.Size(1232, 686)
        Me.TPgConvertings.TabIndex = 0
        Me.TPgConvertings.Text = "Convertings"
        Me.TPgConvertings.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.TBTestHashHEX)
        Me.GroupBox4.Controls.Add(Me.Label58)
        Me.GroupBox4.Controls.Add(Me.TBTestHashUL4)
        Me.GroupBox4.Controls.Add(Me.Label54)
        Me.GroupBox4.Controls.Add(Me.TBTestHashUL2)
        Me.GroupBox4.Controls.Add(Me.Label55)
        Me.GroupBox4.Controls.Add(Me.TBTestHashUL1)
        Me.GroupBox4.Controls.Add(Me.Label56)
        Me.GroupBox4.Controls.Add(Me.TBTestHashUL3)
        Me.GroupBox4.Controls.Add(Me.Label57)
        Me.GroupBox4.Controls.Add(Me.TBTestKeyUL4)
        Me.GroupBox4.Controls.Add(Me.Label53)
        Me.GroupBox4.Controls.Add(Me.TBTestKeyUL2)
        Me.GroupBox4.Controls.Add(Me.Label52)
        Me.GroupBox4.Controls.Add(Me.TBTestKeyUL1)
        Me.GroupBox4.Controls.Add(Me.Label51)
        Me.GroupBox4.Controls.Add(Me.TBTestKeyUL3)
        Me.GroupBox4.Controls.Add(Me.Label50)
        Me.GroupBox4.Controls.Add(Me.BtTestHash)
        Me.GroupBox4.Controls.Add(Me.TBTestHashULHEX)
        Me.GroupBox4.Controls.Add(Me.Label49)
        Me.GroupBox4.Controls.Add(Me.TBTestKeyHEX)
        Me.GroupBox4.Controls.Add(Me.Label48)
        Me.GroupBox4.Controls.Add(Me.TBTestKeyString)
        Me.GroupBox4.Controls.Add(Me.Label47)
        Me.GroupBox4.Location = New System.Drawing.Point(8, 197)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(1056, 216)
        Me.GroupBox4.TabIndex = 39
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "SHA256"
        '
        'TBTestHashHEX
        '
        Me.TBTestHashHEX.Location = New System.Drawing.Point(69, 117)
        Me.TBTestHashHEX.Name = "TBTestHashHEX"
        Me.TBTestHashHEX.Size = New System.Drawing.Size(406, 20)
        Me.TBTestHashHEX.TabIndex = 24
        '
        'Label58
        '
        Me.Label58.AutoSize = True
        Me.Label58.ForeColor = System.Drawing.Color.Black
        Me.Label58.Location = New System.Drawing.Point(6, 120)
        Me.Label58.Name = "Label58"
        Me.Label58.Size = New System.Drawing.Size(57, 13)
        Me.Label58.TabIndex = 23
        Me.Label58.Text = "HashHEX:"
        '
        'TBTestHashUL4
        '
        Me.TBTestHashUL4.Location = New System.Drawing.Point(872, 91)
        Me.TBTestHashUL4.Name = "TBTestHashUL4"
        Me.TBTestHashUL4.Size = New System.Drawing.Size(173, 20)
        Me.TBTestHashUL4.TabIndex = 22
        '
        'Label54
        '
        Me.Label54.AutoSize = True
        Me.Label54.ForeColor = System.Drawing.Color.Black
        Me.Label54.Location = New System.Drawing.Point(814, 94)
        Me.Label54.Name = "Label54"
        Me.Label54.Size = New System.Drawing.Size(55, 13)
        Me.Label54.TabIndex = 21
        Me.Label54.Text = "HashUL4:"
        '
        'TBTestHashUL2
        '
        Me.TBTestHashUL2.Location = New System.Drawing.Point(872, 65)
        Me.TBTestHashUL2.Name = "TBTestHashUL2"
        Me.TBTestHashUL2.Size = New System.Drawing.Size(173, 20)
        Me.TBTestHashUL2.TabIndex = 20
        '
        'Label55
        '
        Me.Label55.AutoSize = True
        Me.Label55.ForeColor = System.Drawing.Color.Black
        Me.Label55.Location = New System.Drawing.Point(814, 68)
        Me.Label55.Name = "Label55"
        Me.Label55.Size = New System.Drawing.Size(55, 13)
        Me.Label55.TabIndex = 19
        Me.Label55.Text = "HashUL2:"
        '
        'TBTestHashUL1
        '
        Me.TBTestHashUL1.Location = New System.Drawing.Point(639, 65)
        Me.TBTestHashUL1.Name = "TBTestHashUL1"
        Me.TBTestHashUL1.Size = New System.Drawing.Size(173, 20)
        Me.TBTestHashUL1.TabIndex = 18
        '
        'Label56
        '
        Me.Label56.AutoSize = True
        Me.Label56.ForeColor = System.Drawing.Color.Black
        Me.Label56.Location = New System.Drawing.Point(578, 68)
        Me.Label56.Name = "Label56"
        Me.Label56.Size = New System.Drawing.Size(55, 13)
        Me.Label56.TabIndex = 17
        Me.Label56.Text = "HashUL1:"
        '
        'TBTestHashUL3
        '
        Me.TBTestHashUL3.Location = New System.Drawing.Point(639, 91)
        Me.TBTestHashUL3.Name = "TBTestHashUL3"
        Me.TBTestHashUL3.Size = New System.Drawing.Size(173, 20)
        Me.TBTestHashUL3.TabIndex = 16
        '
        'Label57
        '
        Me.Label57.AutoSize = True
        Me.Label57.ForeColor = System.Drawing.Color.Black
        Me.Label57.Location = New System.Drawing.Point(578, 94)
        Me.Label57.Name = "Label57"
        Me.Label57.Size = New System.Drawing.Size(55, 13)
        Me.Label57.TabIndex = 15
        Me.Label57.Text = "HashUL3:"
        '
        'TBTestKeyUL4
        '
        Me.TBTestKeyUL4.Location = New System.Drawing.Point(872, 39)
        Me.TBTestKeyUL4.Name = "TBTestKeyUL4"
        Me.TBTestKeyUL4.Size = New System.Drawing.Size(173, 20)
        Me.TBTestKeyUL4.TabIndex = 14
        '
        'Label53
        '
        Me.Label53.AutoSize = True
        Me.Label53.ForeColor = System.Drawing.Color.Black
        Me.Label53.Location = New System.Drawing.Point(818, 42)
        Me.Label53.Name = "Label53"
        Me.Label53.Size = New System.Drawing.Size(48, 13)
        Me.Label53.TabIndex = 13
        Me.Label53.Text = "KeyUL4:"
        '
        'TBTestKeyUL2
        '
        Me.TBTestKeyUL2.Location = New System.Drawing.Point(872, 13)
        Me.TBTestKeyUL2.Name = "TBTestKeyUL2"
        Me.TBTestKeyUL2.Size = New System.Drawing.Size(173, 20)
        Me.TBTestKeyUL2.TabIndex = 12
        '
        'Label52
        '
        Me.Label52.AutoSize = True
        Me.Label52.ForeColor = System.Drawing.Color.Black
        Me.Label52.Location = New System.Drawing.Point(818, 16)
        Me.Label52.Name = "Label52"
        Me.Label52.Size = New System.Drawing.Size(48, 13)
        Me.Label52.TabIndex = 11
        Me.Label52.Text = "KeyUL2:"
        '
        'TBTestKeyUL1
        '
        Me.TBTestKeyUL1.Location = New System.Drawing.Point(639, 13)
        Me.TBTestKeyUL1.Name = "TBTestKeyUL1"
        Me.TBTestKeyUL1.Size = New System.Drawing.Size(173, 20)
        Me.TBTestKeyUL1.TabIndex = 10
        '
        'Label51
        '
        Me.Label51.AutoSize = True
        Me.Label51.ForeColor = System.Drawing.Color.Black
        Me.Label51.Location = New System.Drawing.Point(585, 16)
        Me.Label51.Name = "Label51"
        Me.Label51.Size = New System.Drawing.Size(48, 13)
        Me.Label51.TabIndex = 9
        Me.Label51.Text = "KeyUL1:"
        '
        'TBTestKeyUL3
        '
        Me.TBTestKeyUL3.Location = New System.Drawing.Point(639, 39)
        Me.TBTestKeyUL3.Name = "TBTestKeyUL3"
        Me.TBTestKeyUL3.Size = New System.Drawing.Size(173, 20)
        Me.TBTestKeyUL3.TabIndex = 8
        '
        'Label50
        '
        Me.Label50.AutoSize = True
        Me.Label50.ForeColor = System.Drawing.Color.Black
        Me.Label50.Location = New System.Drawing.Point(585, 42)
        Me.Label50.Name = "Label50"
        Me.Label50.Size = New System.Drawing.Size(48, 13)
        Me.Label50.TabIndex = 7
        Me.Label50.Text = "KeyUL3:"
        '
        'BtTestHash
        '
        Me.BtTestHash.ForeColor = System.Drawing.Color.Black
        Me.BtTestHash.Location = New System.Drawing.Point(491, 13)
        Me.BtTestHash.Name = "BtTestHash"
        Me.BtTestHash.Size = New System.Drawing.Size(75, 47)
        Me.BtTestHash.TabIndex = 6
        Me.BtTestHash.Text = "hash"
        Me.BtTestHash.UseVisualStyleBackColor = True
        '
        'TBTestHashULHEX
        '
        Me.TBTestHashULHEX.Location = New System.Drawing.Point(639, 117)
        Me.TBTestHashULHEX.Name = "TBTestHashULHEX"
        Me.TBTestHashULHEX.Size = New System.Drawing.Size(406, 20)
        Me.TBTestHashULHEX.TabIndex = 5
        '
        'Label49
        '
        Me.Label49.AutoSize = True
        Me.Label49.ForeColor = System.Drawing.Color.Black
        Me.Label49.Location = New System.Drawing.Point(576, 120)
        Me.Label49.Name = "Label49"
        Me.Label49.Size = New System.Drawing.Size(57, 13)
        Me.Label49.TabIndex = 4
        Me.Label49.Text = "HashHEX:"
        '
        'TBTestKeyHEX
        '
        Me.TBTestKeyHEX.Location = New System.Drawing.Point(69, 39)
        Me.TBTestKeyHEX.Name = "TBTestKeyHEX"
        Me.TBTestKeyHEX.Size = New System.Drawing.Size(406, 20)
        Me.TBTestKeyHEX.TabIndex = 3
        '
        'Label48
        '
        Me.Label48.AutoSize = True
        Me.Label48.ForeColor = System.Drawing.Color.Black
        Me.Label48.Location = New System.Drawing.Point(6, 42)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(53, 13)
        Me.Label48.TabIndex = 2
        Me.Label48.Text = "Key HEX:"
        '
        'TBTestKeyString
        '
        Me.TBTestKeyString.Location = New System.Drawing.Point(69, 13)
        Me.TBTestKeyString.Name = "TBTestKeyString"
        Me.TBTestKeyString.Size = New System.Drawing.Size(406, 20)
        Me.TBTestKeyString.TabIndex = 1
        '
        'Label47
        '
        Me.Label47.AutoSize = True
        Me.Label47.ForeColor = System.Drawing.Color.Black
        Me.Label47.Location = New System.Drawing.Point(6, 16)
        Me.Label47.Name = "Label47"
        Me.Label47.Size = New System.Drawing.Size(58, 13)
        Me.Label47.TabIndex = 0
        Me.Label47.Text = "Key String:"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.BtTestRecursiveXMLSearch)
        Me.GroupBox2.Controls.Add(Me.TBTestRecursiveXMLSearch)
        Me.GroupBox2.Controls.Add(Me.Label14)
        Me.GroupBox2.Controls.Add(Me.TBTestXMLOutput)
        Me.GroupBox2.Controls.Add(Me.Label13)
        Me.GroupBox2.Controls.Add(Me.BtTestJSONToXML)
        Me.GroupBox2.Controls.Add(Me.TBTestJSONInput)
        Me.GroupBox2.Controls.Add(Me.Label12)
        Me.GroupBox2.Location = New System.Drawing.Point(320, 92)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(744, 99)
        Me.GroupBox2.TabIndex = 38
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "JSON To XML"
        '
        'BtTestRecursiveXMLSearch
        '
        Me.BtTestRecursiveXMLSearch.ForeColor = System.Drawing.Color.Black
        Me.BtTestRecursiveXMLSearch.Location = New System.Drawing.Point(629, 39)
        Me.BtTestRecursiveXMLSearch.Name = "BtTestRecursiveXMLSearch"
        Me.BtTestRecursiveXMLSearch.Size = New System.Drawing.Size(109, 23)
        Me.BtTestRecursiveXMLSearch.TabIndex = 7
        Me.BtTestRecursiveXMLSearch.Text = "Recursive Search"
        Me.BtTestRecursiveXMLSearch.UseVisualStyleBackColor = True
        '
        'TBTestRecursiveXMLSearch
        '
        Me.TBTestRecursiveXMLSearch.Location = New System.Drawing.Point(312, 41)
        Me.TBTestRecursiveXMLSearch.Name = "TBTestRecursiveXMLSearch"
        Me.TBTestRecursiveXMLSearch.Size = New System.Drawing.Size(311, 20)
        Me.TBTestRecursiveXMLSearch.TabIndex = 6
        Me.TBTestRecursiveXMLSearch.Text = "key2/key3/key4/key5"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.ForeColor = System.Drawing.Color.Black
        Me.Label14.Location = New System.Drawing.Point(192, 44)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(114, 13)
        Me.Label14.TabIndex = 5
        Me.Label14.Text = "RecursiveSearchPath:"
        '
        'TBTestXMLOutput
        '
        Me.TBTestXMLOutput.Location = New System.Drawing.Point(54, 68)
        Me.TBTestXMLOutput.Name = "TBTestXMLOutput"
        Me.TBTestXMLOutput.Size = New System.Drawing.Size(684, 20)
        Me.TBTestXMLOutput.TabIndex = 4
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.ForeColor = System.Drawing.Color.Black
        Me.Label13.Location = New System.Drawing.Point(6, 71)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(42, 13)
        Me.Label13.TabIndex = 3
        Me.Label13.Text = "Output:"
        '
        'BtTestJSONToXML
        '
        Me.BtTestJSONToXML.ForeColor = System.Drawing.Color.Black
        Me.BtTestJSONToXML.Location = New System.Drawing.Point(54, 39)
        Me.BtTestJSONToXML.Name = "BtTestJSONToXML"
        Me.BtTestJSONToXML.Size = New System.Drawing.Size(132, 23)
        Me.BtTestJSONToXML.TabIndex = 2
        Me.BtTestJSONToXML.Text = "Convert JSON To XML"
        Me.BtTestJSONToXML.UseVisualStyleBackColor = True
        '
        'TBTestJSONInput
        '
        Me.TBTestJSONInput.Location = New System.Drawing.Point(54, 13)
        Me.TBTestJSONInput.Name = "TBTestJSONInput"
        Me.TBTestJSONInput.Size = New System.Drawing.Size(684, 20)
        Me.TBTestJSONInput.TabIndex = 1
        Me.TBTestJSONInput.Text = resources.GetString("TBTestJSONInput.Text")
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.ForeColor = System.Drawing.Color.Black
        Me.Label12.Location = New System.Drawing.Point(6, 16)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(34, 13)
        Me.Label12.TabIndex = 0
        Me.Label12.Text = "Input:"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.BtTestCreateCollWord)
        Me.GroupBox1.Controls.Add(Me.TBTestCollWordOutput)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.TBTestCollWordInput)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 92)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(302, 99)
        Me.GroupBox1.TabIndex = 37
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Colloquial Words Test"
        '
        'BtTestCreateCollWord
        '
        Me.BtTestCreateCollWord.ForeColor = System.Drawing.Color.Black
        Me.BtTestCreateCollWord.Location = New System.Drawing.Point(60, 39)
        Me.BtTestCreateCollWord.Name = "BtTestCreateCollWord"
        Me.BtTestCreateCollWord.Size = New System.Drawing.Size(234, 23)
        Me.BtTestCreateCollWord.TabIndex = 4
        Me.BtTestCreateCollWord.Text = "Convert / Create Colloquial Word"
        Me.BtTestCreateCollWord.UseVisualStyleBackColor = True
        '
        'TBTestCollWordOutput
        '
        Me.TBTestCollWordOutput.Location = New System.Drawing.Point(60, 68)
        Me.TBTestCollWordOutput.Name = "TBTestCollWordOutput"
        Me.TBTestCollWordOutput.Size = New System.Drawing.Size(234, 20)
        Me.TBTestCollWordOutput.TabIndex = 3
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(6, 71)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(42, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Output:"
        '
        'TBTestCollWordInput
        '
        Me.TBTestCollWordInput.Location = New System.Drawing.Point(60, 13)
        Me.TBTestCollWordInput.Name = "TBTestCollWordInput"
        Me.TBTestCollWordInput.Size = New System.Drawing.Size(234, 20)
        Me.TBTestCollWordInput.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(6, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(48, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Input ID:"
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.BtTestTimeConvert)
        Me.GroupBox5.Controls.Add(Me.TBTestTime)
        Me.GroupBox5.Location = New System.Drawing.Point(944, 19)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(114, 80)
        Me.GroupBox5.TabIndex = 36
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Time to Unix"
        '
        'BtTestTimeConvert
        '
        Me.BtTestTimeConvert.ForeColor = System.Drawing.Color.Black
        Me.BtTestTimeConvert.Location = New System.Drawing.Point(6, 45)
        Me.BtTestTimeConvert.Name = "BtTestTimeConvert"
        Me.BtTestTimeConvert.Size = New System.Drawing.Size(100, 23)
        Me.BtTestTimeConvert.TabIndex = 1
        Me.BtTestTimeConvert.Text = "convert"
        Me.BtTestTimeConvert.UseVisualStyleBackColor = True
        '
        'TBTestTime
        '
        Me.TBTestTime.Location = New System.Drawing.Point(6, 19)
        Me.TBTestTime.Name = "TBTestTime"
        Me.TBTestTime.Size = New System.Drawing.Size(100, 20)
        Me.TBTestTime.TabIndex = 0
        '
        'TestGrpBoxConvertings
        '
        Me.TestGrpBoxConvertings.Controls.Add(Me.BtTestHex2ULng)
        Me.TestGrpBoxConvertings.Controls.Add(Me.TBTestConvert)
        Me.TestGrpBoxConvertings.Controls.Add(Me.BtTestConvert)
        Me.TestGrpBoxConvertings.Controls.Add(Me.BtTestConvert2)
        Me.TestGrpBoxConvertings.Controls.Add(Me.BtTestDatStr2ULngList)
        Me.TestGrpBoxConvertings.Location = New System.Drawing.Point(6, 6)
        Me.TestGrpBoxConvertings.Name = "TestGrpBoxConvertings"
        Me.TestGrpBoxConvertings.Size = New System.Drawing.Size(536, 80)
        Me.TestGrpBoxConvertings.TabIndex = 35
        Me.TestGrpBoxConvertings.TabStop = False
        Me.TestGrpBoxConvertings.Text = "Convertings"
        '
        'BtTestHex2ULng
        '
        Me.BtTestHex2ULng.ForeColor = System.Drawing.Color.Black
        Me.BtTestHex2ULng.Location = New System.Drawing.Point(300, 45)
        Me.BtTestHex2ULng.Name = "BtTestHex2ULng"
        Me.BtTestHex2ULng.Size = New System.Drawing.Size(91, 23)
        Me.BtTestHex2ULng.TabIndex = 7
        Me.BtTestHex2ULng.Text = "HEX2ULong"
        Me.BtTestHex2ULng.UseVisualStyleBackColor = True
        '
        'TBTestConvert
        '
        Me.TBTestConvert.Location = New System.Drawing.Point(6, 19)
        Me.TBTestConvert.Name = "TBTestConvert"
        Me.TBTestConvert.Size = New System.Drawing.Size(288, 20)
        Me.TBTestConvert.TabIndex = 3
        '
        'BtTestConvert
        '
        Me.BtTestConvert.ForeColor = System.Drawing.Color.Black
        Me.BtTestConvert.Location = New System.Drawing.Point(116, 45)
        Me.BtTestConvert.Name = "BtTestConvert"
        Me.BtTestConvert.Size = New System.Drawing.Size(86, 23)
        Me.BtTestConvert.TabIndex = 4
        Me.BtTestConvert.Text = "String2ULong"
        Me.BtTestConvert.UseVisualStyleBackColor = True
        '
        'BtTestConvert2
        '
        Me.BtTestConvert2.ForeColor = System.Drawing.Color.Black
        Me.BtTestConvert2.Location = New System.Drawing.Point(208, 45)
        Me.BtTestConvert2.Name = "BtTestConvert2"
        Me.BtTestConvert2.Size = New System.Drawing.Size(86, 23)
        Me.BtTestConvert2.TabIndex = 5
        Me.BtTestConvert2.Text = "ULong2String"
        Me.BtTestConvert2.UseVisualStyleBackColor = True
        '
        'BtTestDatStr2ULngList
        '
        Me.BtTestDatStr2ULngList.ForeColor = System.Drawing.Color.Black
        Me.BtTestDatStr2ULngList.Location = New System.Drawing.Point(6, 45)
        Me.BtTestDatStr2ULngList.Name = "BtTestDatStr2ULngList"
        Me.BtTestDatStr2ULngList.Size = New System.Drawing.Size(104, 23)
        Me.BtTestDatStr2ULngList.TabIndex = 6
        Me.BtTestDatStr2ULngList.Text = "DataStr2ULngList"
        Me.BtTestDatStr2ULngList.UseVisualStyleBackColor = True
        '
        'TPgINITools
        '
        Me.TPgINITools.Controls.Add(Me.TestGrpBoxINITests)
        Me.TPgINITools.Location = New System.Drawing.Point(4, 22)
        Me.TPgINITools.Name = "TPgINITools"
        Me.TPgINITools.Padding = New System.Windows.Forms.Padding(3)
        Me.TPgINITools.Size = New System.Drawing.Size(1232, 686)
        Me.TPgINITools.TabIndex = 1
        Me.TPgINITools.Text = "INI-Tools-Tests"
        Me.TPgINITools.UseVisualStyleBackColor = True
        '
        'TestGrpBoxINITests
        '
        Me.TestGrpBoxINITests.Controls.Add(Me.TBTestSetTXINI)
        Me.TestGrpBoxINITests.Controls.Add(Me.BtTestSetTXINI)
        Me.TestGrpBoxINITests.Controls.Add(Me.BtTestGetTXINI)
        Me.TestGrpBoxINITests.Controls.Add(Me.BtTestDelTXINI)
        Me.TestGrpBoxINITests.Controls.Add(Me.TBTestGetTXINI)
        Me.TestGrpBoxINITests.Controls.Add(Me.TBTestDelTXINI)
        Me.TestGrpBoxINITests.Location = New System.Drawing.Point(6, 6)
        Me.TestGrpBoxINITests.Name = "TestGrpBoxINITests"
        Me.TestGrpBoxINITests.Size = New System.Drawing.Size(316, 85)
        Me.TestGrpBoxINITests.TabIndex = 37
        Me.TestGrpBoxINITests.TabStop = False
        Me.TestGrpBoxINITests.Text = "INI-Tools-Tests"
        '
        'TBTestSetTXINI
        '
        Me.TBTestSetTXINI.Location = New System.Drawing.Point(6, 19)
        Me.TBTestSetTXINI.Name = "TBTestSetTXINI"
        Me.TBTestSetTXINI.Size = New System.Drawing.Size(97, 20)
        Me.TBTestSetTXINI.TabIndex = 14
        '
        'BtTestSetTXINI
        '
        Me.BtTestSetTXINI.ForeColor = System.Drawing.Color.Black
        Me.BtTestSetTXINI.Location = New System.Drawing.Point(6, 46)
        Me.BtTestSetTXINI.Name = "BtTestSetTXINI"
        Me.BtTestSetTXINI.Size = New System.Drawing.Size(97, 23)
        Me.BtTestSetTXINI.TabIndex = 11
        Me.BtTestSetTXINI.Text = "Set TX in INI"
        Me.BtTestSetTXINI.UseVisualStyleBackColor = True
        '
        'BtTestGetTXINI
        '
        Me.BtTestGetTXINI.ForeColor = System.Drawing.Color.Black
        Me.BtTestGetTXINI.Location = New System.Drawing.Point(109, 46)
        Me.BtTestGetTXINI.Name = "BtTestGetTXINI"
        Me.BtTestGetTXINI.Size = New System.Drawing.Size(97, 23)
        Me.BtTestGetTXINI.TabIndex = 12
        Me.BtTestGetTXINI.Text = "Get TX from INI"
        Me.BtTestGetTXINI.UseVisualStyleBackColor = True
        '
        'BtTestDelTXINI
        '
        Me.BtTestDelTXINI.ForeColor = System.Drawing.Color.Black
        Me.BtTestDelTXINI.Location = New System.Drawing.Point(212, 46)
        Me.BtTestDelTXINI.Name = "BtTestDelTXINI"
        Me.BtTestDelTXINI.Size = New System.Drawing.Size(97, 23)
        Me.BtTestDelTXINI.TabIndex = 13
        Me.BtTestDelTXINI.Text = "Del TX from INI"
        Me.BtTestDelTXINI.UseVisualStyleBackColor = True
        '
        'TBTestGetTXINI
        '
        Me.TBTestGetTXINI.Location = New System.Drawing.Point(109, 19)
        Me.TBTestGetTXINI.Name = "TBTestGetTXINI"
        Me.TBTestGetTXINI.Size = New System.Drawing.Size(97, 20)
        Me.TBTestGetTXINI.TabIndex = 15
        '
        'TBTestDelTXINI
        '
        Me.TBTestDelTXINI.Location = New System.Drawing.Point(212, 19)
        Me.TBTestDelTXINI.Name = "TBTestDelTXINI"
        Me.TBTestDelTXINI.Size = New System.Drawing.Size(97, 20)
        Me.TBTestDelTXINI.TabIndex = 16
        '
        'TPgPayPal
        '
        Me.TPgPayPal.Controls.Add(Me.SplitContainer10)
        Me.TPgPayPal.Location = New System.Drawing.Point(4, 22)
        Me.TPgPayPal.Name = "TPgPayPal"
        Me.TPgPayPal.Size = New System.Drawing.Size(1232, 686)
        Me.TPgPayPal.TabIndex = 2
        Me.TPgPayPal.Text = "PayPal-Tests"
        Me.TPgPayPal.UseVisualStyleBackColor = True
        '
        'SplitContainer10
        '
        Me.SplitContainer10.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer10.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer10.Name = "SplitContainer10"
        Me.SplitContainer10.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer10.Panel1
        '
        Me.SplitContainer10.Panel1.Controls.Add(Me.GroupBox7)
        Me.SplitContainer10.Panel1.Controls.Add(Me.GroupBox6)
        Me.SplitContainer10.Panel1.Controls.Add(Me.GroupBox3)
        Me.SplitContainer10.Panel1.Controls.Add(Me.BtTestPPCheckAPI)
        '
        'SplitContainer10.Panel2
        '
        Me.SplitContainer10.Panel2.Controls.Add(Me.LiBoPayPalComs)
        Me.SplitContainer10.Size = New System.Drawing.Size(1232, 686)
        Me.SplitContainer10.SplitterDistance = 180
        Me.SplitContainer10.TabIndex = 11
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.BtTestPPPayout)
        Me.GroupBox7.Controls.Add(Me.TBTestPPPONote)
        Me.GroupBox7.Controls.Add(Me.TBTestPPPOCurrency)
        Me.GroupBox7.Controls.Add(Me.TBTestPPPOAmount)
        Me.GroupBox7.Controls.Add(Me.TBTestPPPORecipient)
        Me.GroupBox7.Controls.Add(Me.Label27)
        Me.GroupBox7.Controls.Add(Me.Label26)
        Me.GroupBox7.Controls.Add(Me.Label20)
        Me.GroupBox7.Controls.Add(Me.Label16)
        Me.GroupBox7.Location = New System.Drawing.Point(447, 3)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(239, 121)
        Me.GroupBox7.TabIndex = 17
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "PayPal Payouts"
        '
        'BtTestPPPayout
        '
        Me.BtTestPPPayout.ForeColor = System.Drawing.Color.Black
        Me.BtTestPPPayout.Location = New System.Drawing.Point(173, 64)
        Me.BtTestPPPayout.Name = "BtTestPPPayout"
        Me.BtTestPPPayout.Size = New System.Drawing.Size(56, 47)
        Me.BtTestPPPayout.TabIndex = 8
        Me.BtTestPPPayout.Text = "PayPal Batch Payout"
        Me.BtTestPPPayout.UseVisualStyleBackColor = True
        '
        'TBTestPPPONote
        '
        Me.TBTestPPPONote.Location = New System.Drawing.Point(67, 91)
        Me.TBTestPPPONote.Name = "TBTestPPPONote"
        Me.TBTestPPPONote.Size = New System.Drawing.Size(100, 20)
        Me.TBTestPPPONote.TabIndex = 7
        '
        'TBTestPPPOCurrency
        '
        Me.TBTestPPPOCurrency.Location = New System.Drawing.Point(67, 65)
        Me.TBTestPPPOCurrency.Name = "TBTestPPPOCurrency"
        Me.TBTestPPPOCurrency.Size = New System.Drawing.Size(100, 20)
        Me.TBTestPPPOCurrency.TabIndex = 6
        '
        'TBTestPPPOAmount
        '
        Me.TBTestPPPOAmount.Location = New System.Drawing.Point(67, 39)
        Me.TBTestPPPOAmount.Name = "TBTestPPPOAmount"
        Me.TBTestPPPOAmount.Size = New System.Drawing.Size(100, 20)
        Me.TBTestPPPOAmount.TabIndex = 5
        '
        'TBTestPPPORecipient
        '
        Me.TBTestPPPORecipient.Location = New System.Drawing.Point(67, 13)
        Me.TBTestPPPORecipient.Name = "TBTestPPPORecipient"
        Me.TBTestPPPORecipient.Size = New System.Drawing.Size(100, 20)
        Me.TBTestPPPORecipient.TabIndex = 4
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.ForeColor = System.Drawing.Color.Black
        Me.Label27.Location = New System.Drawing.Point(6, 94)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(33, 13)
        Me.Label27.TabIndex = 3
        Me.Label27.Text = "Note:"
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.ForeColor = System.Drawing.Color.Black
        Me.Label26.Location = New System.Drawing.Point(6, 68)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(52, 13)
        Me.Label26.TabIndex = 2
        Me.Label26.Text = "Currency:"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.ForeColor = System.Drawing.Color.Black
        Me.Label20.Location = New System.Drawing.Point(6, 42)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(46, 13)
        Me.Label20.TabIndex = 1
        Me.Label20.Text = "Amount:"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.ForeColor = System.Drawing.Color.Black
        Me.Label16.Location = New System.Drawing.Point(6, 16)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(55, 13)
        Me.Label16.TabIndex = 0
        Me.Label16.Text = "Recipient:"
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.CoBxTestPPCurrency)
        Me.GroupBox6.Controls.Add(Me.TBTestPPPrice)
        Me.GroupBox6.Controls.Add(Me.Label18)
        Me.GroupBox6.Controls.Add(Me.TBTestPPXAmount)
        Me.GroupBox6.Controls.Add(Me.Label19)
        Me.GroupBox6.Controls.Add(Me.TBTestPPXItem)
        Me.GroupBox6.Controls.Add(Me.Label17)
        Me.GroupBox6.Controls.Add(Me.BtTestCreatePPOrder)
        Me.GroupBox6.Location = New System.Drawing.Point(259, 3)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(182, 121)
        Me.GroupBox6.TabIndex = 16
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "PayPal Orders"
        '
        'CoBxTestPPCurrency
        '
        Me.CoBxTestPPCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CoBxTestPPCurrency.FormattingEnabled = True
        Me.CoBxTestPPCurrency.Location = New System.Drawing.Point(115, 64)
        Me.CoBxTestPPCurrency.Name = "CoBxTestPPCurrency"
        Me.CoBxTestPPCurrency.Size = New System.Drawing.Size(61, 21)
        Me.CoBxTestPPCurrency.TabIndex = 20
        '
        'TBTestPPPrice
        '
        Me.TBTestPPPrice.Location = New System.Drawing.Point(65, 65)
        Me.TBTestPPPrice.Name = "TBTestPPPrice"
        Me.TBTestPPPrice.Size = New System.Drawing.Size(44, 20)
        Me.TBTestPPPrice.TabIndex = 19
        Me.TBTestPPPrice.Text = "12,50"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.ForeColor = System.Drawing.Color.Black
        Me.Label18.Location = New System.Drawing.Point(6, 68)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(34, 13)
        Me.Label18.TabIndex = 17
        Me.Label18.Text = "Price:"
        '
        'TBTestPPXAmount
        '
        Me.TBTestPPXAmount.Location = New System.Drawing.Point(65, 39)
        Me.TBTestPPXAmount.Name = "TBTestPPXAmount"
        Me.TBTestPPXAmount.Size = New System.Drawing.Size(111, 20)
        Me.TBTestPPXAmount.TabIndex = 16
        Me.TBTestPPXAmount.Text = "123"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.ForeColor = System.Drawing.Color.Black
        Me.Label19.Location = New System.Drawing.Point(6, 42)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(53, 13)
        Me.Label19.TabIndex = 15
        Me.Label19.Text = "XAmount:"
        '
        'TBTestPPXItem
        '
        Me.TBTestPPXItem.Location = New System.Drawing.Point(65, 13)
        Me.TBTestPPXItem.Name = "TBTestPPXItem"
        Me.TBTestPPXItem.Size = New System.Drawing.Size(111, 20)
        Me.TBTestPPXItem.TabIndex = 14
        Me.TBTestPPXItem.Text = "SIGNA"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.ForeColor = System.Drawing.Color.Black
        Me.Label17.Location = New System.Drawing.Point(6, 16)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(37, 13)
        Me.Label17.TabIndex = 13
        Me.Label17.Text = "XItem:"
        '
        'BtTestCreatePPOrder
        '
        Me.BtTestCreatePPOrder.ForeColor = System.Drawing.Color.Black
        Me.BtTestCreatePPOrder.Location = New System.Drawing.Point(65, 91)
        Me.BtTestCreatePPOrder.Name = "BtTestCreatePPOrder"
        Me.BtTestCreatePPOrder.Size = New System.Drawing.Size(111, 23)
        Me.BtTestCreatePPOrder.TabIndex = 10
        Me.BtTestCreatePPOrder.Text = "Create PayPalOrder"
        Me.BtTestCreatePPOrder.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Label15)
        Me.GroupBox3.Controls.Add(Me.BtTestPPGetAllTX)
        Me.GroupBox3.Controls.Add(Me.BtTestPPGetTXWithNote)
        Me.GroupBox3.Controls.Add(Me.TBTestPPTXNote)
        Me.GroupBox3.Location = New System.Drawing.Point(133, 3)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(120, 121)
        Me.GroupBox3.TabIndex = 15
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "PayPal Transactions"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.ForeColor = System.Drawing.Color.Black
        Me.Label15.Location = New System.Drawing.Point(6, 16)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(86, 13)
        Me.Label15.TabIndex = 11
        Me.Label15.Text = "PayPal TX Note:"
        '
        'BtTestPPGetAllTX
        '
        Me.BtTestPPGetAllTX.ForeColor = System.Drawing.Color.Black
        Me.BtTestPPGetAllTX.Location = New System.Drawing.Point(6, 91)
        Me.BtTestPPGetAllTX.Name = "BtTestPPGetAllTX"
        Me.BtTestPPGetAllTX.Size = New System.Drawing.Size(108, 23)
        Me.BtTestPPGetAllTX.TabIndex = 9
        Me.BtTestPPGetAllTX.Text = "Get All TX"
        Me.BtTestPPGetAllTX.UseVisualStyleBackColor = True
        '
        'BtTestPPGetTXWithNote
        '
        Me.BtTestPPGetTXWithNote.ForeColor = System.Drawing.Color.Black
        Me.BtTestPPGetTXWithNote.Location = New System.Drawing.Point(9, 59)
        Me.BtTestPPGetTXWithNote.Name = "BtTestPPGetTXWithNote"
        Me.BtTestPPGetTXWithNote.Size = New System.Drawing.Size(100, 23)
        Me.BtTestPPGetTXWithNote.TabIndex = 13
        Me.BtTestPPGetTXWithNote.Text = "Get TX with Note"
        Me.BtTestPPGetTXWithNote.UseVisualStyleBackColor = True
        '
        'TBTestPPTXNote
        '
        Me.TBTestPPTXNote.Location = New System.Drawing.Point(9, 32)
        Me.TBTestPPTXNote.Name = "TBTestPPTXNote"
        Me.TBTestPPTXNote.Size = New System.Drawing.Size(100, 20)
        Me.TBTestPPTXNote.TabIndex = 12
        Me.TBTestPPTXNote.Text = "PayPal Testnote"
        '
        'BtTestPPCheckAPI
        '
        Me.BtTestPPCheckAPI.ForeColor = System.Drawing.Color.Black
        Me.BtTestPPCheckAPI.Location = New System.Drawing.Point(7, 3)
        Me.BtTestPPCheckAPI.Name = "BtTestPPCheckAPI"
        Me.BtTestPPCheckAPI.Size = New System.Drawing.Size(120, 23)
        Me.BtTestPPCheckAPI.TabIndex = 14
        Me.BtTestPPCheckAPI.Text = "Check PayPal API"
        Me.BtTestPPCheckAPI.UseVisualStyleBackColor = True
        '
        'LiBoPayPalComs
        '
        Me.LiBoPayPalComs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LiBoPayPalComs.FormattingEnabled = True
        Me.LiBoPayPalComs.Location = New System.Drawing.Point(0, 0)
        Me.LiBoPayPalComs.Name = "LiBoPayPalComs"
        Me.LiBoPayPalComs.Size = New System.Drawing.Size(1232, 502)
        Me.LiBoPayPalComs.TabIndex = 0
        '
        'TPgAT_SC_Comm
        '
        Me.TPgAT_SC_Comm.AutoScroll = True
        Me.TPgAT_SC_Comm.Controls.Add(Me.RTBTestDebug)
        Me.TPgAT_SC_Comm.Controls.Add(Me.TestGrpBxATCom)
        Me.TPgAT_SC_Comm.Controls.Add(Me.Label1)
        Me.TPgAT_SC_Comm.Controls.Add(Me.Label21)
        Me.TPgAT_SC_Comm.Controls.Add(Me.LVTestDEXContractBasic)
        Me.TPgAT_SC_Comm.Controls.Add(Me.LVTestCurrentOrder)
        Me.TPgAT_SC_Comm.Location = New System.Drawing.Point(4, 22)
        Me.TPgAT_SC_Comm.Name = "TPgAT_SC_Comm"
        Me.TPgAT_SC_Comm.Size = New System.Drawing.Size(1232, 686)
        Me.TPgAT_SC_Comm.TabIndex = 3
        Me.TPgAT_SC_Comm.Text = "AutomatedTransaction / SmartContract Communication"
        Me.TPgAT_SC_Comm.UseVisualStyleBackColor = True
        '
        'RTBTestDebug
        '
        Me.RTBTestDebug.Location = New System.Drawing.Point(798, 336)
        Me.RTBTestDebug.Name = "RTBTestDebug"
        Me.RTBTestDebug.Size = New System.Drawing.Size(416, 342)
        Me.RTBTestDebug.TabIndex = 71
        Me.RTBTestDebug.Text = ""
        '
        'TestGrpBxATCom
        '
        Me.TestGrpBxATCom.Controls.Add(Me.NUDTestAmount)
        Me.TestGrpBxATCom.Controls.Add(Me.NUDTestXAmount)
        Me.TestGrpBxATCom.Controls.Add(Me.CoBxTestATComATID)
        Me.TestGrpBxATCom.Controls.Add(Me.TBTestXItem)
        Me.TestGrpBxATCom.Controls.Add(Me.TBTestPP)
        Me.TestGrpBxATCom.Controls.Add(Me.Panel4)
        Me.TestGrpBxATCom.Controls.Add(Me.BtTestCheckCloseDispute)
        Me.TestGrpBxATCom.Controls.Add(Me.BtTestAppeal)
        Me.TestGrpBxATCom.Controls.Add(Me.BtTestOpenDispute)
        Me.TestGrpBxATCom.Controls.Add(Me.BtTestDeActivateDeniability)
        Me.TestGrpBxATCom.Controls.Add(Me.NUDTestCollateral)
        Me.TestGrpBxATCom.Controls.Add(Me.BtTestFinish)
        Me.TestGrpBxATCom.Controls.Add(Me.Panel1)
        Me.TestGrpBxATCom.Controls.Add(Me.Panel2)
        Me.TestGrpBxATCom.Controls.Add(Me.Panel3)
        Me.TestGrpBxATCom.Controls.Add(Me.Panel5)
        Me.TestGrpBxATCom.Controls.Add(Me.Panel6)
        Me.TestGrpBxATCom.Controls.Add(Me.Panel7)
        Me.TestGrpBxATCom.Controls.Add(Me.BtExport)
        Me.TestGrpBxATCom.Controls.Add(Me.BtTestLoadDEXContract)
        Me.TestGrpBxATCom.Controls.Add(Me.Label31)
        Me.TestGrpBxATCom.Controls.Add(Me.Label40)
        Me.TestGrpBxATCom.Location = New System.Drawing.Point(8, 3)
        Me.TestGrpBxATCom.Name = "TestGrpBxATCom"
        Me.TestGrpBxATCom.Size = New System.Drawing.Size(1206, 314)
        Me.TestGrpBxATCom.TabIndex = 36
        Me.TestGrpBxATCom.TabStop = False
        Me.TestGrpBxATCom.Text = "AT/SC Communication"
        '
        'NUDTestAmount
        '
        Me.NUDTestAmount.DecimalPlaces = 8
        Me.NUDTestAmount.Location = New System.Drawing.Point(137, 95)
        Me.NUDTestAmount.Maximum = New Decimal(New Integer() {1410065408, 2, 0, 0})
        Me.NUDTestAmount.Name = "NUDTestAmount"
        Me.NUDTestAmount.Size = New System.Drawing.Size(254, 20)
        Me.NUDTestAmount.TabIndex = 80
        Me.NUDTestAmount.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'NUDTestXAmount
        '
        Me.NUDTestXAmount.DecimalPlaces = 2
        Me.NUDTestXAmount.Location = New System.Drawing.Point(137, 212)
        Me.NUDTestXAmount.Maximum = New Decimal(New Integer() {1000000, 0, 0, 0})
        Me.NUDTestXAmount.Name = "NUDTestXAmount"
        Me.NUDTestXAmount.Size = New System.Drawing.Size(173, 20)
        Me.NUDTestXAmount.TabIndex = 88
        Me.NUDTestXAmount.Value = New Decimal(New Integer() {20, 0, 0, 0})
        '
        'CoBxTestATComATID
        '
        Me.CoBxTestATComATID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CoBxTestATComATID.FormattingEnabled = True
        Me.CoBxTestATComATID.Location = New System.Drawing.Point(10, 32)
        Me.CoBxTestATComATID.Name = "CoBxTestATComATID"
        Me.CoBxTestATComATID.Size = New System.Drawing.Size(300, 21)
        Me.CoBxTestATComATID.TabIndex = 79
        '
        'TBTestXItem
        '
        Me.TBTestXItem.Location = New System.Drawing.Point(137, 173)
        Me.TBTestXItem.Name = "TBTestXItem"
        Me.TBTestXItem.Size = New System.Drawing.Size(173, 20)
        Me.TBTestXItem.TabIndex = 89
        Me.TBTestXItem.Text = "USD"
        '
        'TBTestPP
        '
        Me.TBTestPP.Location = New System.Drawing.Point(10, 254)
        Me.TBTestPP.Name = "TBTestPP"
        Me.TBTestPP.Size = New System.Drawing.Size(1189, 20)
        Me.TBTestPP.TabIndex = 82
        '
        'Panel4
        '
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel4.Controls.Add(Me.TB_TestCreateWithResponder)
        Me.Panel4.Controls.Add(Me.BtTestCreateWithResponder)
        Me.Panel4.Controls.Add(Me.Label22)
        Me.Panel4.Controls.Add(Me.Label23)
        Me.Panel4.Controls.Add(Me.Label24)
        Me.Panel4.Controls.Add(Me.Label25)
        Me.Panel4.Location = New System.Drawing.Point(135, 10)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(98, 295)
        Me.Panel4.TabIndex = 91
        '
        'TB_TestCreateWithResponder
        '
        Me.TB_TestCreateWithResponder.Location = New System.Drawing.Point(1, 124)
        Me.TB_TestCreateWithResponder.Name = "TB_TestCreateWithResponder"
        Me.TB_TestCreateWithResponder.Size = New System.Drawing.Size(94, 20)
        Me.TB_TestCreateWithResponder.TabIndex = 68
        '
        'BtTestCreateWithResponder
        '
        Me.BtTestCreateWithResponder.ForeColor = System.Drawing.Color.Black
        Me.BtTestCreateWithResponder.Location = New System.Drawing.Point(1, 269)
        Me.BtTestCreateWithResponder.Name = "BtTestCreateWithResponder"
        Me.BtTestCreateWithResponder.Size = New System.Drawing.Size(94, 23)
        Me.BtTestCreateWithResponder.TabIndex = 4
        Me.BtTestCreateWithResponder.Text = "create w. Resp."
        Me.BtTestCreateWithResponder.UseVisualStyleBackColor = True
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.ForeColor = System.Drawing.Color.Black
        Me.Label22.Location = New System.Drawing.Point(0, 67)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(63, 13)
        Me.Label22.TabIndex = 32
        Me.Label22.Text = "SellAmount:"
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.ForeColor = System.Drawing.Color.Black
        Me.Label23.Location = New System.Drawing.Point(0, 106)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(73, 13)
        Me.Label23.TabIndex = 50
        Me.Label23.Text = "ResponderID:"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(0, 146)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(37, 13)
        Me.Label24.TabIndex = 66
        Me.Label24.Text = "XItem:"
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Location = New System.Drawing.Point(0, 185)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(53, 13)
        Me.Label25.TabIndex = 64
        Me.Label25.Text = "XAmount:"
        '
        'BtTestCheckCloseDispute
        '
        Me.BtTestCheckCloseDispute.ForeColor = System.Drawing.Color.Black
        Me.BtTestCheckCloseDispute.Location = New System.Drawing.Point(780, 280)
        Me.BtTestCheckCloseDispute.Name = "BtTestCheckCloseDispute"
        Me.BtTestCheckCloseDispute.Size = New System.Drawing.Size(110, 23)
        Me.BtTestCheckCloseDispute.TabIndex = 87
        Me.BtTestCheckCloseDispute.Text = "checkCloseDispute"
        Me.BtTestCheckCloseDispute.UseVisualStyleBackColor = True
        '
        'BtTestAppeal
        '
        Me.BtTestAppeal.ForeColor = System.Drawing.Color.Black
        Me.BtTestAppeal.Location = New System.Drawing.Point(699, 280)
        Me.BtTestAppeal.Name = "BtTestAppeal"
        Me.BtTestAppeal.Size = New System.Drawing.Size(75, 23)
        Me.BtTestAppeal.TabIndex = 86
        Me.BtTestAppeal.Text = "appeal"
        Me.BtTestAppeal.UseVisualStyleBackColor = True
        '
        'BtTestOpenDispute
        '
        Me.BtTestOpenDispute.ForeColor = System.Drawing.Color.Black
        Me.BtTestOpenDispute.Location = New System.Drawing.Point(508, 280)
        Me.BtTestOpenDispute.Name = "BtTestOpenDispute"
        Me.BtTestOpenDispute.Size = New System.Drawing.Size(83, 23)
        Me.BtTestOpenDispute.TabIndex = 85
        Me.BtTestOpenDispute.Text = "openDispute"
        Me.BtTestOpenDispute.UseVisualStyleBackColor = True
        '
        'BtTestDeActivateDeniability
        '
        Me.BtTestDeActivateDeniability.ForeColor = System.Drawing.Color.Black
        Me.BtTestDeActivateDeniability.Location = New System.Drawing.Point(10, 280)
        Me.BtTestDeActivateDeniability.Name = "BtTestDeActivateDeniability"
        Me.BtTestDeActivateDeniability.Size = New System.Drawing.Size(121, 23)
        Me.BtTestDeActivateDeniability.TabIndex = 84
        Me.BtTestDeActivateDeniability.Text = "deActivateDeniability"
        Me.BtTestDeActivateDeniability.UseVisualStyleBackColor = True
        '
        'NUDTestCollateral
        '
        Me.NUDTestCollateral.DecimalPlaces = 8
        Me.NUDTestCollateral.Location = New System.Drawing.Point(237, 135)
        Me.NUDTestCollateral.Maximum = New Decimal(New Integer() {1410065408, 2, 0, 0})
        Me.NUDTestCollateral.Name = "NUDTestCollateral"
        Me.NUDTestCollateral.Size = New System.Drawing.Size(154, 20)
        Me.NUDTestCollateral.TabIndex = 81
        Me.NUDTestCollateral.Value = New Decimal(New Integer() {30, 0, 0, 0})
        '
        'BtTestFinish
        '
        Me.BtTestFinish.ForeColor = System.Drawing.Color.Black
        Me.BtTestFinish.Location = New System.Drawing.Point(896, 280)
        Me.BtTestFinish.Name = "BtTestFinish"
        Me.BtTestFinish.Size = New System.Drawing.Size(75, 23)
        Me.BtTestFinish.TabIndex = 83
        Me.BtTestFinish.Text = "finish"
        Me.BtTestFinish.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.BtTestCreate)
        Me.Panel1.Controls.Add(Me.LabAmount)
        Me.Panel1.Controls.Add(Me.Label35)
        Me.Panel1.Controls.Add(Me.ChBxSellOrder)
        Me.Panel1.Location = New System.Drawing.Point(235, 10)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(77, 295)
        Me.Panel1.TabIndex = 90
        '
        'BtTestCreate
        '
        Me.BtTestCreate.ForeColor = System.Drawing.Color.Black
        Me.BtTestCreate.Location = New System.Drawing.Point(1, 269)
        Me.BtTestCreate.Name = "BtTestCreate"
        Me.BtTestCreate.Size = New System.Drawing.Size(73, 23)
        Me.BtTestCreate.TabIndex = 4
        Me.BtTestCreate.Text = "create"
        Me.BtTestCreate.UseVisualStyleBackColor = True
        '
        'LabAmount
        '
        Me.LabAmount.AutoSize = True
        Me.LabAmount.ForeColor = System.Drawing.Color.Black
        Me.LabAmount.Location = New System.Drawing.Point(0, 67)
        Me.LabAmount.Name = "LabAmount"
        Me.LabAmount.Size = New System.Drawing.Size(66, 13)
        Me.LabAmount.TabIndex = 32
        Me.LabAmount.Text = "WantToSell:"
        '
        'Label35
        '
        Me.Label35.AutoSize = True
        Me.Label35.ForeColor = System.Drawing.Color.Black
        Me.Label35.Location = New System.Drawing.Point(0, 106)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(53, 13)
        Me.Label35.TabIndex = 50
        Me.Label35.Text = "Collateral:"
        '
        'ChBxSellOrder
        '
        Me.ChBxSellOrder.AutoSize = True
        Me.ChBxSellOrder.Checked = True
        Me.ChBxSellOrder.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChBxSellOrder.Location = New System.Drawing.Point(1, 47)
        Me.ChBxSellOrder.Name = "ChBxSellOrder"
        Me.ChBxSellOrder.Size = New System.Drawing.Size(69, 17)
        Me.ChBxSellOrder.TabIndex = 51
        Me.ChBxSellOrder.Text = "SellOrder"
        Me.ChBxSellOrder.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.BtTestAccept)
        Me.Panel2.Location = New System.Drawing.Point(314, 10)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(79, 295)
        Me.Panel2.TabIndex = 92
        '
        'BtTestAccept
        '
        Me.BtTestAccept.ForeColor = System.Drawing.Color.Black
        Me.BtTestAccept.Location = New System.Drawing.Point(1, 269)
        Me.BtTestAccept.Name = "BtTestAccept"
        Me.BtTestAccept.Size = New System.Drawing.Size(75, 23)
        Me.BtTestAccept.TabIndex = 5
        Me.BtTestAccept.Text = "accept"
        Me.BtTestAccept.UseVisualStyleBackColor = True
        '
        'Panel3
        '
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel3.Controls.Add(Me.TBTestResponder)
        Me.Panel3.Controls.Add(Me.Label33)
        Me.Panel3.Controls.Add(Me.BtTestInjectResponder)
        Me.Panel3.Location = New System.Drawing.Point(395, 10)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(108, 295)
        Me.Panel3.TabIndex = 93
        '
        'TBTestResponder
        '
        Me.TBTestResponder.Location = New System.Drawing.Point(1, 23)
        Me.TBTestResponder.Name = "TBTestResponder"
        Me.TBTestResponder.Size = New System.Drawing.Size(104, 20)
        Me.TBTestResponder.TabIndex = 7
        '
        'Label33
        '
        Me.Label33.AutoSize = True
        Me.Label33.ForeColor = System.Drawing.Color.Black
        Me.Label33.Location = New System.Drawing.Point(0, 7)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(99, 13)
        Me.Label33.TabIndex = 27
        Me.Label33.Text = "InjectResponderID:"
        '
        'BtTestInjectResponder
        '
        Me.BtTestInjectResponder.ForeColor = System.Drawing.Color.Black
        Me.BtTestInjectResponder.Location = New System.Drawing.Point(1, 269)
        Me.BtTestInjectResponder.Name = "BtTestInjectResponder"
        Me.BtTestInjectResponder.Size = New System.Drawing.Size(104, 23)
        Me.BtTestInjectResponder.TabIndex = 8
        Me.BtTestInjectResponder.Text = "injectResponder"
        Me.BtTestInjectResponder.UseVisualStyleBackColor = True
        '
        'Panel5
        '
        Me.Panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel5.Controls.Add(Me.TBTestChainSwapHashLong3)
        Me.Panel5.Controls.Add(Me.Label46)
        Me.Panel5.Controls.Add(Me.TBTestChainSwapHashLong2)
        Me.Panel5.Controls.Add(Me.Label41)
        Me.Panel5.Controls.Add(Me.Label34)
        Me.Panel5.Controls.Add(Me.TBTestChainSwapHashLong1)
        Me.Panel5.Controls.Add(Me.Label39)
        Me.Panel5.Controls.Add(Me.BtTestChainSwapKeyToHash)
        Me.Panel5.Controls.Add(Me.TBTestChainSwapKey)
        Me.Panel5.Controls.Add(Me.BtTestInjectChainSwapHash)
        Me.Panel5.Location = New System.Drawing.Point(974, 10)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(109, 295)
        Me.Panel5.TabIndex = 95
        '
        'TBTestChainSwapHashLong3
        '
        Me.TBTestChainSwapHashLong3.Location = New System.Drawing.Point(1, 179)
        Me.TBTestChainSwapHashLong3.Name = "TBTestChainSwapHashLong3"
        Me.TBTestChainSwapHashLong3.ReadOnly = True
        Me.TBTestChainSwapHashLong3.Size = New System.Drawing.Size(105, 20)
        Me.TBTestChainSwapHashLong3.TabIndex = 51
        '
        'Label46
        '
        Me.Label46.AutoSize = True
        Me.Label46.ForeColor = System.Drawing.Color.Black
        Me.Label46.Location = New System.Drawing.Point(0, 163)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(73, 13)
        Me.Label46.TabIndex = 52
        Me.Label46.Text = "HashULong3:"
        '
        'TBTestChainSwapHashLong2
        '
        Me.TBTestChainSwapHashLong2.Location = New System.Drawing.Point(1, 140)
        Me.TBTestChainSwapHashLong2.Name = "TBTestChainSwapHashLong2"
        Me.TBTestChainSwapHashLong2.ReadOnly = True
        Me.TBTestChainSwapHashLong2.Size = New System.Drawing.Size(105, 20)
        Me.TBTestChainSwapHashLong2.TabIndex = 49
        '
        'Label41
        '
        Me.Label41.AutoSize = True
        Me.Label41.ForeColor = System.Drawing.Color.Black
        Me.Label41.Location = New System.Drawing.Point(0, 124)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(73, 13)
        Me.Label41.TabIndex = 50
        Me.Label41.Text = "HashULong2:"
        '
        'Label34
        '
        Me.Label34.AutoSize = True
        Me.Label34.ForeColor = System.Drawing.Color.Black
        Me.Label34.Location = New System.Drawing.Point(0, 7)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(82, 13)
        Me.Label34.TabIndex = 48
        Me.Label34.Text = "ChainSwapKey:"
        '
        'TBTestChainSwapHashLong1
        '
        Me.TBTestChainSwapHashLong1.Location = New System.Drawing.Point(1, 101)
        Me.TBTestChainSwapHashLong1.Name = "TBTestChainSwapHashLong1"
        Me.TBTestChainSwapHashLong1.ReadOnly = True
        Me.TBTestChainSwapHashLong1.Size = New System.Drawing.Size(105, 20)
        Me.TBTestChainSwapHashLong1.TabIndex = 11
        '
        'Label39
        '
        Me.Label39.AutoSize = True
        Me.Label39.ForeColor = System.Drawing.Color.Black
        Me.Label39.Location = New System.Drawing.Point(0, 85)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(73, 13)
        Me.Label39.TabIndex = 37
        Me.Label39.Text = "HashULong1:"
        '
        'BtTestChainSwapKeyToHash
        '
        Me.BtTestChainSwapKeyToHash.ForeColor = System.Drawing.Color.Black
        Me.BtTestChainSwapKeyToHash.Location = New System.Drawing.Point(1, 61)
        Me.BtTestChainSwapKeyToHash.Name = "BtTestChainSwapKeyToHash"
        Me.BtTestChainSwapKeyToHash.Size = New System.Drawing.Size(105, 22)
        Me.BtTestChainSwapKeyToHash.TabIndex = 10
        Me.BtTestChainSwapKeyToHash.Text = "compute Hash"
        Me.BtTestChainSwapKeyToHash.UseVisualStyleBackColor = True
        '
        'TBTestChainSwapKey
        '
        Me.TBTestChainSwapKey.Location = New System.Drawing.Point(1, 23)
        Me.TBTestChainSwapKey.Name = "TBTestChainSwapKey"
        Me.TBTestChainSwapKey.Size = New System.Drawing.Size(105, 20)
        Me.TBTestChainSwapKey.TabIndex = 9
        '
        'BtTestInjectChainSwapHash
        '
        Me.BtTestInjectChainSwapHash.ForeColor = System.Drawing.Color.Black
        Me.BtTestInjectChainSwapHash.Location = New System.Drawing.Point(1, 269)
        Me.BtTestInjectChainSwapHash.Name = "BtTestInjectChainSwapHash"
        Me.BtTestInjectChainSwapHash.Size = New System.Drawing.Size(105, 23)
        Me.BtTestInjectChainSwapHash.TabIndex = 12
        Me.BtTestInjectChainSwapHash.Text = "injectSwapHash"
        Me.BtTestInjectChainSwapHash.UseVisualStyleBackColor = True
        '
        'Panel6
        '
        Me.Panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel6.Controls.Add(Me.BtCSKConvertback)
        Me.Panel6.Controls.Add(Me.Label42)
        Me.Panel6.Controls.Add(Me.TBTestChainSwapKeyULong3)
        Me.Panel6.Controls.Add(Me.TBTestChainSwapKeyULong2)
        Me.Panel6.Controls.Add(Me.TBTestChainSwapKeyULong1)
        Me.Panel6.Controls.Add(Me.Label43)
        Me.Panel6.Controls.Add(Me.Label44)
        Me.Panel6.Controls.Add(Me.BtTestFinishWithChainSwapKey)
        Me.Panel6.Location = New System.Drawing.Point(1085, 10)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(116, 295)
        Me.Panel6.TabIndex = 96
        '
        'BtCSKConvertback
        '
        Me.BtCSKConvertback.ForeColor = System.Drawing.Color.Black
        Me.BtCSKConvertback.Location = New System.Drawing.Point(1, 177)
        Me.BtCSKConvertback.Name = "BtCSKConvertback"
        Me.BtCSKConvertback.Size = New System.Drawing.Size(110, 23)
        Me.BtCSKConvertback.TabIndex = 81
        Me.BtCSKConvertback.Text = "convert back"
        Me.BtCSKConvertback.UseVisualStyleBackColor = True
        '
        'Label42
        '
        Me.Label42.AutoSize = True
        Me.Label42.ForeColor = System.Drawing.Color.Black
        Me.Label42.Location = New System.Drawing.Point(1, 84)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(66, 13)
        Me.Label42.TabIndex = 79
        Me.Label42.Text = "KeyULong3:"
        '
        'TBTestChainSwapKeyULong3
        '
        Me.TBTestChainSwapKeyULong3.Location = New System.Drawing.Point(1, 101)
        Me.TBTestChainSwapKeyULong3.Name = "TBTestChainSwapKeyULong3"
        Me.TBTestChainSwapKeyULong3.Size = New System.Drawing.Size(112, 20)
        Me.TBTestChainSwapKeyULong3.TabIndex = 77
        '
        'TBTestChainSwapKeyULong2
        '
        Me.TBTestChainSwapKeyULong2.Location = New System.Drawing.Point(1, 62)
        Me.TBTestChainSwapKeyULong2.Name = "TBTestChainSwapKeyULong2"
        Me.TBTestChainSwapKeyULong2.Size = New System.Drawing.Size(112, 20)
        Me.TBTestChainSwapKeyULong2.TabIndex = 14
        '
        'TBTestChainSwapKeyULong1
        '
        Me.TBTestChainSwapKeyULong1.Location = New System.Drawing.Point(1, 23)
        Me.TBTestChainSwapKeyULong1.Name = "TBTestChainSwapKeyULong1"
        Me.TBTestChainSwapKeyULong1.Size = New System.Drawing.Size(112, 20)
        Me.TBTestChainSwapKeyULong1.TabIndex = 13
        '
        'Label43
        '
        Me.Label43.AutoSize = True
        Me.Label43.ForeColor = System.Drawing.Color.Black
        Me.Label43.Location = New System.Drawing.Point(0, 6)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(66, 13)
        Me.Label43.TabIndex = 43
        Me.Label43.Text = "KeyULong1:"
        '
        'Label44
        '
        Me.Label44.AutoSize = True
        Me.Label44.ForeColor = System.Drawing.Color.Black
        Me.Label44.Location = New System.Drawing.Point(0, 46)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(66, 13)
        Me.Label44.TabIndex = 45
        Me.Label44.Text = "KeyULong2:"
        '
        'BtTestFinishWithChainSwapKey
        '
        Me.BtTestFinishWithChainSwapKey.ForeColor = System.Drawing.Color.Black
        Me.BtTestFinishWithChainSwapKey.Location = New System.Drawing.Point(1, 269)
        Me.BtTestFinishWithChainSwapKey.Name = "BtTestFinishWithChainSwapKey"
        Me.BtTestFinishWithChainSwapKey.Size = New System.Drawing.Size(112, 23)
        Me.BtTestFinishWithChainSwapKey.TabIndex = 15
        Me.BtTestFinishWithChainSwapKey.Text = "finishWithSwapULs"
        Me.BtTestFinishWithChainSwapKey.UseVisualStyleBackColor = True
        '
        'Panel7
        '
        Me.Panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel7.Controls.Add(Me.Label45)
        Me.Panel7.Controls.Add(Me.NUDTestMediateAmount)
        Me.Panel7.Controls.Add(Me.BtTestMediateDispute)
        Me.Panel7.Location = New System.Drawing.Point(595, 8)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(100, 297)
        Me.Panel7.TabIndex = 94
        '
        'Label45
        '
        Me.Label45.AutoSize = True
        Me.Label45.Location = New System.Drawing.Point(0, 7)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(47, 13)
        Me.Label45.TabIndex = 57
        Me.Label45.Text = "Percent:"
        '
        'NUDTestMediateAmount
        '
        Me.NUDTestMediateAmount.DecimalPlaces = 2
        Me.NUDTestMediateAmount.Location = New System.Drawing.Point(1, 23)
        Me.NUDTestMediateAmount.Name = "NUDTestMediateAmount"
        Me.NUDTestMediateAmount.Size = New System.Drawing.Size(96, 20)
        Me.NUDTestMediateAmount.TabIndex = 59
        Me.NUDTestMediateAmount.Value = New Decimal(New Integer() {50, 0, 0, 0})
        '
        'BtTestMediateDispute
        '
        Me.BtTestMediateDispute.ForeColor = System.Drawing.Color.Black
        Me.BtTestMediateDispute.Location = New System.Drawing.Point(1, 271)
        Me.BtTestMediateDispute.Name = "BtTestMediateDispute"
        Me.BtTestMediateDispute.Size = New System.Drawing.Size(96, 23)
        Me.BtTestMediateDispute.TabIndex = 54
        Me.BtTestMediateDispute.Text = "mediateDispute"
        Me.BtTestMediateDispute.UseVisualStyleBackColor = True
        '
        'BtExport
        '
        Me.BtExport.ForeColor = System.Drawing.Color.Black
        Me.BtExport.Location = New System.Drawing.Point(9, 87)
        Me.BtExport.Name = "BtExport"
        Me.BtExport.Size = New System.Drawing.Size(75, 23)
        Me.BtExport.TabIndex = 78
        Me.BtExport.Text = "export "
        Me.BtExport.UseVisualStyleBackColor = True
        '
        'BtTestLoadDEXContract
        '
        Me.BtTestLoadDEXContract.ForeColor = System.Drawing.Color.Black
        Me.BtTestLoadDEXContract.Location = New System.Drawing.Point(9, 58)
        Me.BtTestLoadDEXContract.Name = "BtTestLoadDEXContract"
        Me.BtTestLoadDEXContract.Size = New System.Drawing.Size(75, 23)
        Me.BtTestLoadDEXContract.TabIndex = 77
        Me.BtTestLoadDEXContract.Text = "load"
        Me.BtTestLoadDEXContract.UseVisualStyleBackColor = True
        '
        'Label31
        '
        Me.Label31.AutoSize = True
        Me.Label31.ForeColor = System.Drawing.Color.Black
        Me.Label31.Location = New System.Drawing.Point(6, 16)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(38, 13)
        Me.Label31.TabIndex = 28
        Me.Label31.Text = "AT ID:"
        '
        'Label40
        '
        Me.Label40.AutoSize = True
        Me.Label40.ForeColor = System.Drawing.Color.Black
        Me.Label40.Location = New System.Drawing.Point(6, 237)
        Me.Label40.Name = "Label40"
        Me.Label40.Size = New System.Drawing.Size(66, 13)
        Me.Label40.TabIndex = 23
        Me.Label40.Text = "PassPhrase:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(412, 320)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(73, 13)
        Me.Label1.TabIndex = 70
        Me.Label1.Text = "Current Order:"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(15, 320)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(106, 13)
        Me.Label21.TabIndex = 69
        Me.Label21.Text = "DEXContract Basics:"
        '
        'LVTestDEXContractBasic
        '
        Me.LVTestDEXContractBasic.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader13, Me.ColumnHeader14})
        Me.LVTestDEXContractBasic.FullRowSelect = True
        Me.LVTestDEXContractBasic.GridLines = True
        Me.LVTestDEXContractBasic.HideSelection = False
        Me.LVTestDEXContractBasic.Location = New System.Drawing.Point(15, 336)
        Me.LVTestDEXContractBasic.Name = "LVTestDEXContractBasic"
        Me.LVTestDEXContractBasic.Size = New System.Drawing.Size(394, 343)
        Me.LVTestDEXContractBasic.TabIndex = 63
        Me.LVTestDEXContractBasic.UseCompatibleStateImageBehavior = False
        Me.LVTestDEXContractBasic.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader13
        '
        Me.ColumnHeader13.Text = "Property"
        '
        'ColumnHeader14
        '
        Me.ColumnHeader14.Text = "Value"
        '
        'LVTestCurrentOrder
        '
        Me.LVTestCurrentOrder.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader9, Me.ColumnHeader12})
        Me.LVTestCurrentOrder.FullRowSelect = True
        Me.LVTestCurrentOrder.GridLines = True
        Me.LVTestCurrentOrder.HideSelection = False
        Me.LVTestCurrentOrder.Location = New System.Drawing.Point(415, 336)
        Me.LVTestCurrentOrder.Name = "LVTestCurrentOrder"
        Me.LVTestCurrentOrder.Size = New System.Drawing.Size(377, 343)
        Me.LVTestCurrentOrder.TabIndex = 68
        Me.LVTestCurrentOrder.UseCompatibleStateImageBehavior = False
        Me.LVTestCurrentOrder.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader9
        '
        Me.ColumnHeader9.Text = "Property"
        '
        'ColumnHeader12
        '
        Me.ColumnHeader12.Text = "Value"
        '
        'TPgMultithreadings
        '
        Me.TPgMultithreadings.Controls.Add(Me.TestGrpBoxMultithreadings)
        Me.TPgMultithreadings.Location = New System.Drawing.Point(4, 22)
        Me.TPgMultithreadings.Name = "TPgMultithreadings"
        Me.TPgMultithreadings.Size = New System.Drawing.Size(1232, 686)
        Me.TPgMultithreadings.TabIndex = 4
        Me.TPgMultithreadings.Text = "Multithreading-Tests"
        Me.TPgMultithreadings.UseVisualStyleBackColor = True
        '
        'TestGrpBoxMultithreadings
        '
        Me.TestGrpBoxMultithreadings.Controls.Add(Me.SplitContainer9)
        Me.TestGrpBoxMultithreadings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestGrpBoxMultithreadings.Location = New System.Drawing.Point(0, 0)
        Me.TestGrpBoxMultithreadings.Name = "TestGrpBoxMultithreadings"
        Me.TestGrpBoxMultithreadings.Size = New System.Drawing.Size(1232, 686)
        Me.TestGrpBoxMultithreadings.TabIndex = 38
        Me.TestGrpBoxMultithreadings.TabStop = False
        Me.TestGrpBoxMultithreadings.Text = "Multithreading-Tests"
        '
        'SplitContainer9
        '
        Me.SplitContainer9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer9.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer9.Location = New System.Drawing.Point(3, 16)
        Me.SplitContainer9.Name = "SplitContainer9"
        Me.SplitContainer9.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer9.Panel1
        '
        Me.SplitContainer9.Panel1.Controls.Add(Me.LabActiveNodes)
        Me.SplitContainer9.Panel1.Controls.Add(Me.Label36)
        Me.SplitContainer9.Panel1.Controls.Add(Me.ChBxAutoRefreshMultiThreads)
        Me.SplitContainer9.Panel1.Controls.Add(Me.BtTestMultiRefresh)
        Me.SplitContainer9.Panel1.Controls.Add(Me.BtTestExit)
        '
        'SplitContainer9.Panel2
        '
        Me.SplitContainer9.Panel2.Controls.Add(Me.SplitContainer12)
        Me.SplitContainer9.Size = New System.Drawing.Size(1226, 667)
        Me.SplitContainer9.SplitterDistance = 55
        Me.SplitContainer9.TabIndex = 32
        '
        'LabActiveNodes
        '
        Me.LabActiveNodes.AutoSize = True
        Me.LabActiveNodes.ForeColor = System.Drawing.Color.Black
        Me.LabActiveNodes.Location = New System.Drawing.Point(245, 8)
        Me.LabActiveNodes.Name = "LabActiveNodes"
        Me.LabActiveNodes.Size = New System.Drawing.Size(13, 13)
        Me.LabActiveNodes.TabIndex = 34
        Me.LabActiveNodes.Text = "0"
        '
        'Label36
        '
        Me.Label36.AutoSize = True
        Me.Label36.ForeColor = System.Drawing.Color.Black
        Me.Label36.Location = New System.Drawing.Point(165, 8)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(74, 13)
        Me.Label36.TabIndex = 33
        Me.Label36.Text = "Active Nodes:"
        '
        'ChBxAutoRefreshMultiThreads
        '
        Me.ChBxAutoRefreshMultiThreads.AutoSize = True
        Me.ChBxAutoRefreshMultiThreads.ForeColor = System.Drawing.Color.Black
        Me.ChBxAutoRefreshMultiThreads.Location = New System.Drawing.Point(3, 32)
        Me.ChBxAutoRefreshMultiThreads.Name = "ChBxAutoRefreshMultiThreads"
        Me.ChBxAutoRefreshMultiThreads.Size = New System.Drawing.Size(114, 17)
        Me.ChBxAutoRefreshMultiThreads.TabIndex = 32
        Me.ChBxAutoRefreshMultiThreads.Text = "autorefresh (1 sec)"
        Me.ChBxAutoRefreshMultiThreads.UseVisualStyleBackColor = True
        '
        'BtTestMultiRefresh
        '
        Me.BtTestMultiRefresh.ForeColor = System.Drawing.Color.Black
        Me.BtTestMultiRefresh.Location = New System.Drawing.Point(3, 3)
        Me.BtTestMultiRefresh.Name = "BtTestMultiRefresh"
        Me.BtTestMultiRefresh.Size = New System.Drawing.Size(75, 23)
        Me.BtTestMultiRefresh.TabIndex = 31
        Me.BtTestMultiRefresh.Text = "Refresh"
        Me.BtTestMultiRefresh.UseVisualStyleBackColor = True
        '
        'BtTestExit
        '
        Me.BtTestExit.ForeColor = System.Drawing.Color.Black
        Me.BtTestExit.Location = New System.Drawing.Point(84, 3)
        Me.BtTestExit.Name = "BtTestExit"
        Me.BtTestExit.Size = New System.Drawing.Size(75, 23)
        Me.BtTestExit.TabIndex = 30
        Me.BtTestExit.Text = "Exit"
        Me.BtTestExit.UseVisualStyleBackColor = True
        '
        'SplitContainer12
        '
        Me.SplitContainer12.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer12.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer12.Name = "SplitContainer12"
        '
        'SplitContainer12.Panel1
        '
        Me.SplitContainer12.Panel1.Controls.Add(Me.SplitContainer13)
        '
        'SplitContainer12.Panel2
        '
        Me.SplitContainer12.Panel2.Controls.Add(Me.SplitContainer14)
        Me.SplitContainer12.Size = New System.Drawing.Size(1226, 608)
        Me.SplitContainer12.SplitterDistance = 608
        Me.SplitContainer12.TabIndex = 29
        '
        'SplitContainer13
        '
        Me.SplitContainer13.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer13.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer13.IsSplitterFixed = True
        Me.SplitContainer13.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer13.Name = "SplitContainer13"
        Me.SplitContainer13.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer13.Panel1
        '
        Me.SplitContainer13.Panel1.Controls.Add(Me.Label37)
        '
        'SplitContainer13.Panel2
        '
        Me.SplitContainer13.Panel2.Controls.Add(Me.LVTestMulti)
        Me.SplitContainer13.Size = New System.Drawing.Size(608, 608)
        Me.SplitContainer13.SplitterDistance = 25
        Me.SplitContainer13.TabIndex = 0
        '
        'Label37
        '
        Me.Label37.AutoSize = True
        Me.Label37.Location = New System.Drawing.Point(5, 6)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(89, 13)
        Me.Label37.TabIndex = 0
        Me.Label37.Text = "API Request List:"
        '
        'LVTestMulti
        '
        Me.LVTestMulti.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader5, Me.ColumnHeader6})
        Me.LVTestMulti.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LVTestMulti.FullRowSelect = True
        Me.LVTestMulti.GridLines = True
        Me.LVTestMulti.HideSelection = False
        Me.LVTestMulti.Location = New System.Drawing.Point(0, 0)
        Me.LVTestMulti.MultiSelect = False
        Me.LVTestMulti.Name = "LVTestMulti"
        Me.LVTestMulti.Size = New System.Drawing.Size(608, 579)
        Me.LVTestMulti.TabIndex = 28
        Me.LVTestMulti.UseCompatibleStateImageBehavior = False
        Me.LVTestMulti.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Node"
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Command"
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "ThreadID"
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Status"
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Result"
        '
        'SplitContainer14
        '
        Me.SplitContainer14.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer14.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer14.IsSplitterFixed = True
        Me.SplitContainer14.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer14.Name = "SplitContainer14"
        Me.SplitContainer14.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer14.Panel1
        '
        Me.SplitContainer14.Panel1.Controls.Add(Me.Label38)
        '
        'SplitContainer14.Panel2
        '
        Me.SplitContainer14.Panel2.Controls.Add(Me.LVActiveNodes)
        Me.SplitContainer14.Size = New System.Drawing.Size(614, 608)
        Me.SplitContainer14.SplitterDistance = 25
        Me.SplitContainer14.TabIndex = 0
        '
        'Label38
        '
        Me.Label38.AutoSize = True
        Me.Label38.Location = New System.Drawing.Point(3, 6)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(88, 13)
        Me.Label38.TabIndex = 1
        Me.Label38.Text = "Active Node List:"
        '
        'LVActiveNodes
        '
        Me.LVActiveNodes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader15})
        Me.LVActiveNodes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LVActiveNodes.FullRowSelect = True
        Me.LVActiveNodes.GridLines = True
        Me.LVActiveNodes.HideSelection = False
        Me.LVActiveNodes.Location = New System.Drawing.Point(0, 0)
        Me.LVActiveNodes.MultiSelect = False
        Me.LVActiveNodes.Name = "LVActiveNodes"
        Me.LVActiveNodes.Size = New System.Drawing.Size(614, 579)
        Me.LVActiveNodes.TabIndex = 29
        Me.LVActiveNodes.UseCompatibleStateImageBehavior = False
        Me.LVActiveNodes.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Node"
        '
        'ColumnHeader15
        '
        Me.ColumnHeader15.Text = "Status"
        '
        'TPgTCPAPI
        '
        Me.TPgTCPAPI.Controls.Add(Me.SplitContainer1)
        Me.TPgTCPAPI.Location = New System.Drawing.Point(4, 22)
        Me.TPgTCPAPI.Name = "TPgTCPAPI"
        Me.TPgTCPAPI.Size = New System.Drawing.Size(1232, 686)
        Me.TPgTCPAPI.TabIndex = 5
        Me.TPgTCPAPI.Text = "TCP API"
        Me.TPgTCPAPI.UseVisualStyleBackColor = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.ChBxTestTCPAPIEnable)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ChBxTestTCPAPIShowStatus)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.LiBoTCPAPIStatus)
        Me.SplitContainer1.Size = New System.Drawing.Size(1232, 686)
        Me.SplitContainer1.SplitterDistance = 25
        Me.SplitContainer1.TabIndex = 1
        '
        'ChBxTestTCPAPIEnable
        '
        Me.ChBxTestTCPAPIEnable.AutoSize = True
        Me.ChBxTestTCPAPIEnable.ForeColor = System.Drawing.Color.Black
        Me.ChBxTestTCPAPIEnable.Location = New System.Drawing.Point(3, 3)
        Me.ChBxTestTCPAPIEnable.Name = "ChBxTestTCPAPIEnable"
        Me.ChBxTestTCPAPIEnable.Size = New System.Drawing.Size(103, 17)
        Me.ChBxTestTCPAPIEnable.TabIndex = 1
        Me.ChBxTestTCPAPIEnable.Text = "TCP API Enable"
        Me.ChBxTestTCPAPIEnable.UseVisualStyleBackColor = True
        '
        'ChBxTestTCPAPIShowStatus
        '
        Me.ChBxTestTCPAPIShowStatus.AutoSize = True
        Me.ChBxTestTCPAPIShowStatus.ForeColor = System.Drawing.Color.Black
        Me.ChBxTestTCPAPIShowStatus.Location = New System.Drawing.Point(112, 3)
        Me.ChBxTestTCPAPIShowStatus.Name = "ChBxTestTCPAPIShowStatus"
        Me.ChBxTestTCPAPIShowStatus.Size = New System.Drawing.Size(86, 17)
        Me.ChBxTestTCPAPIShowStatus.TabIndex = 0
        Me.ChBxTestTCPAPIShowStatus.Text = "Show Status"
        Me.ChBxTestTCPAPIShowStatus.UseVisualStyleBackColor = True
        '
        'LiBoTCPAPIStatus
        '
        Me.LiBoTCPAPIStatus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LiBoTCPAPIStatus.FormattingEnabled = True
        Me.LiBoTCPAPIStatus.Location = New System.Drawing.Point(0, 0)
        Me.LiBoTCPAPIStatus.Name = "LiBoTCPAPIStatus"
        Me.LiBoTCPAPIStatus.Size = New System.Drawing.Size(1232, 657)
        Me.LiBoTCPAPIStatus.TabIndex = 0
        '
        'TPgMultiinvokes
        '
        Me.TPgMultiinvokes.Controls.Add(Me.LVTestMultiInvoke)
        Me.TPgMultiinvokes.Controls.Add(Me.BtTestMultiInvokeSetInLV)
        Me.TPgMultiinvokes.Location = New System.Drawing.Point(4, 22)
        Me.TPgMultiinvokes.Name = "TPgMultiinvokes"
        Me.TPgMultiinvokes.Size = New System.Drawing.Size(1232, 686)
        Me.TPgMultiinvokes.TabIndex = 6
        Me.TPgMultiinvokes.Text = "Multiinvoke-Tests"
        Me.TPgMultiinvokes.UseVisualStyleBackColor = True
        '
        'LVTestMultiInvoke
        '
        Me.LVTestMultiInvoke.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader7, Me.ColumnHeader8, Me.ColumnHeader10, Me.ColumnHeader11})
        Me.LVTestMultiInvoke.FullRowSelect = True
        Me.LVTestMultiInvoke.GridLines = True
        Me.LVTestMultiInvoke.HideSelection = False
        Me.LVTestMultiInvoke.Location = New System.Drawing.Point(15, 67)
        Me.LVTestMultiInvoke.Name = "LVTestMultiInvoke"
        Me.LVTestMultiInvoke.Size = New System.Drawing.Size(747, 97)
        Me.LVTestMultiInvoke.TabIndex = 1
        Me.LVTestMultiInvoke.UseCompatibleStateImageBehavior = False
        Me.LVTestMultiInvoke.View = System.Windows.Forms.View.Details
        '
        'BtTestMultiInvokeSetInLV
        '
        Me.BtTestMultiInvokeSetInLV.ForeColor = System.Drawing.Color.Black
        Me.BtTestMultiInvokeSetInLV.Location = New System.Drawing.Point(27, 22)
        Me.BtTestMultiInvokeSetInLV.Name = "BtTestMultiInvokeSetInLV"
        Me.BtTestMultiInvokeSetInLV.Size = New System.Drawing.Size(75, 23)
        Me.BtTestMultiInvokeSetInLV.TabIndex = 0
        Me.BtTestMultiInvokeSetInLV.Text = "Set in LV"
        Me.BtTestMultiInvokeSetInLV.UseVisualStyleBackColor = True
        '
        'TPgDEXNET
        '
        Me.TPgDEXNET.Controls.Add(Me.SplitContainer6)
        Me.TPgDEXNET.Location = New System.Drawing.Point(4, 22)
        Me.TPgDEXNET.Name = "TPgDEXNET"
        Me.TPgDEXNET.Size = New System.Drawing.Size(1232, 686)
        Me.TPgDEXNET.TabIndex = 7
        Me.TPgDEXNET.Text = "DEXNET"
        Me.TPgDEXNET.UseVisualStyleBackColor = True
        '
        'SplitContainer6
        '
        Me.SplitContainer6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer6.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer6.Name = "SplitContainer6"
        Me.SplitContainer6.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer6.Panel1
        '
        Me.SplitContainer6.Panel1.Controls.Add(Me.SplitContainer2)
        '
        'SplitContainer6.Panel2
        '
        Me.SplitContainer6.Panel2.Controls.Add(Me.SplitContainer7)
        Me.SplitContainer6.Size = New System.Drawing.Size(1232, 686)
        Me.SplitContainer6.SplitterDistance = 334
        Me.SplitContainer6.TabIndex = 2
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.LiBoDEXNETPublicKeys)
        Me.SplitContainer2.Panel1.Controls.Add(Me.Label63)
        Me.SplitContainer2.Panel1.Controls.Add(Me.ChBxDEXNETEncryptMsg)
        Me.SplitContainer2.Panel1.Controls.Add(Me.BtTestBroadcastMsg)
        Me.SplitContainer2.Panel1.Controls.Add(Me.TBTestRecipientPubKey)
        Me.SplitContainer2.Panel1.Controls.Add(Me.Label11)
        Me.SplitContainer2.Panel1.Controls.Add(Me.TBTestBroadcastMessage)
        Me.SplitContainer2.Panel1.Controls.Add(Me.Label10)
        Me.SplitContainer2.Panel1.Controls.Add(Me.BtTestConnect)
        Me.SplitContainer2.Panel1.Controls.Add(Me.TBTestPeerPort)
        Me.SplitContainer2.Panel1.Controls.Add(Me.Label5)
        Me.SplitContainer2.Panel1.Controls.Add(Me.TBTestNewPeer)
        Me.SplitContainer2.Panel1.Controls.Add(Me.Label4)
        Me.SplitContainer2.Panel1.Controls.Add(Me.ChBxTestDEXNETShowStatus)
        Me.SplitContainer2.Panel1.Controls.Add(Me.ChBxTestDEXNETEnable)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.LiBoDEXNETStatus)
        Me.SplitContainer2.Size = New System.Drawing.Size(1230, 332)
        Me.SplitContainer2.SplitterDistance = 129
        Me.SplitContainer2.TabIndex = 0
        '
        'LiBoDEXNETPublicKeys
        '
        Me.LiBoDEXNETPublicKeys.FormattingEnabled = True
        Me.LiBoDEXNETPublicKeys.Location = New System.Drawing.Point(577, 22)
        Me.LiBoDEXNETPublicKeys.Name = "LiBoDEXNETPublicKeys"
        Me.LiBoDEXNETPublicKeys.Size = New System.Drawing.Size(646, 95)
        Me.LiBoDEXNETPublicKeys.TabIndex = 14
        '
        'Label63
        '
        Me.Label63.AutoSize = True
        Me.Label63.Location = New System.Drawing.Point(574, 6)
        Me.Label63.Name = "Label63"
        Me.Label63.Size = New System.Drawing.Size(62, 13)
        Me.Label63.TabIndex = 13
        Me.Label63.Text = "PublicKeys:"
        '
        'ChBxDEXNETEncryptMsg
        '
        Me.ChBxDEXNETEncryptMsg.AutoSize = True
        Me.ChBxDEXNETEncryptMsg.Checked = True
        Me.ChBxDEXNETEncryptMsg.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChBxDEXNETEncryptMsg.ForeColor = System.Drawing.Color.Black
        Me.ChBxDEXNETEncryptMsg.Location = New System.Drawing.Point(319, 105)
        Me.ChBxDEXNETEncryptMsg.Name = "ChBxDEXNETEncryptMsg"
        Me.ChBxDEXNETEncryptMsg.Size = New System.Drawing.Size(108, 17)
        Me.ChBxDEXNETEncryptMsg.TabIndex = 12
        Me.ChBxDEXNETEncryptMsg.Text = "Encrypt Message"
        Me.ChBxDEXNETEncryptMsg.UseVisualStyleBackColor = True
        '
        'BtTestBroadcastMsg
        '
        Me.BtTestBroadcastMsg.ForeColor = System.Drawing.Color.Black
        Me.BtTestBroadcastMsg.Location = New System.Drawing.Point(433, 101)
        Me.BtTestBroadcastMsg.Name = "BtTestBroadcastMsg"
        Me.BtTestBroadcastMsg.Size = New System.Drawing.Size(135, 23)
        Me.BtTestBroadcastMsg.TabIndex = 11
        Me.BtTestBroadcastMsg.Text = "broadcast message"
        Me.BtTestBroadcastMsg.UseVisualStyleBackColor = True
        '
        'TBTestRecipientPubKey
        '
        Me.TBTestRecipientPubKey.Location = New System.Drawing.Point(285, 29)
        Me.TBTestRecipientPubKey.Name = "TBTestRecipientPubKey"
        Me.TBTestRecipientPubKey.Size = New System.Drawing.Size(283, 20)
        Me.TBTestRecipientPubKey.TabIndex = 10
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.ForeColor = System.Drawing.Color.Black
        Me.Label11.Location = New System.Drawing.Point(175, 32)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(95, 13)
        Me.Label11.TabIndex = 9
        Me.Label11.Text = "Recipient PubKey:"
        '
        'TBTestBroadcastMessage
        '
        Me.TBTestBroadcastMessage.Location = New System.Drawing.Point(285, 3)
        Me.TBTestBroadcastMessage.Name = "TBTestBroadcastMessage"
        Me.TBTestBroadcastMessage.Size = New System.Drawing.Size(283, 20)
        Me.TBTestBroadcastMessage.TabIndex = 8
        Me.TBTestBroadcastMessage.Text = "<SCID>0</SCID><Answer>request rejected</Answer>"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.ForeColor = System.Drawing.Color.Black
        Me.Label10.Location = New System.Drawing.Point(175, 6)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(104, 13)
        Me.Label10.TabIndex = 7
        Me.Label10.Text = "Broadcast Message:"
        '
        'BtTestConnect
        '
        Me.BtTestConnect.ForeColor = System.Drawing.Color.Black
        Me.BtTestConnect.Location = New System.Drawing.Point(69, 101)
        Me.BtTestConnect.Name = "BtTestConnect"
        Me.BtTestConnect.Size = New System.Drawing.Size(100, 23)
        Me.BtTestConnect.TabIndex = 6
        Me.BtTestConnect.Text = "connect"
        Me.BtTestConnect.UseVisualStyleBackColor = True
        '
        'TBTestPeerPort
        '
        Me.TBTestPeerPort.Location = New System.Drawing.Point(69, 75)
        Me.TBTestPeerPort.Name = "TBTestPeerPort"
        Me.TBTestPeerPort.Size = New System.Drawing.Size(100, 20)
        Me.TBTestPeerPort.TabIndex = 5
        Me.TBTestPeerPort.Text = "8131"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(6, 78)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(54, 13)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Peer Port:"
        '
        'TBTestNewPeer
        '
        Me.TBTestNewPeer.Location = New System.Drawing.Point(69, 49)
        Me.TBTestNewPeer.Name = "TBTestNewPeer"
        Me.TBTestNewPeer.Size = New System.Drawing.Size(100, 20)
        Me.TBTestNewPeer.TabIndex = 3
        Me.TBTestNewPeer.Text = "127.0.0.1"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(6, 52)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(57, 13)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "New Peer:"
        '
        'ChBxTestDEXNETShowStatus
        '
        Me.ChBxTestDEXNETShowStatus.AutoSize = True
        Me.ChBxTestDEXNETShowStatus.ForeColor = System.Drawing.Color.Black
        Me.ChBxTestDEXNETShowStatus.Location = New System.Drawing.Point(3, 26)
        Me.ChBxTestDEXNETShowStatus.Name = "ChBxTestDEXNETShowStatus"
        Me.ChBxTestDEXNETShowStatus.Size = New System.Drawing.Size(130, 17)
        Me.ChBxTestDEXNETShowStatus.TabIndex = 1
        Me.ChBxTestDEXNETShowStatus.Text = "DEXNET ShowStatus"
        Me.ChBxTestDEXNETShowStatus.UseVisualStyleBackColor = True
        '
        'ChBxTestDEXNETEnable
        '
        Me.ChBxTestDEXNETEnable.AutoSize = True
        Me.ChBxTestDEXNETEnable.ForeColor = System.Drawing.Color.Black
        Me.ChBxTestDEXNETEnable.Location = New System.Drawing.Point(3, 3)
        Me.ChBxTestDEXNETEnable.Name = "ChBxTestDEXNETEnable"
        Me.ChBxTestDEXNETEnable.Size = New System.Drawing.Size(106, 17)
        Me.ChBxTestDEXNETEnable.TabIndex = 0
        Me.ChBxTestDEXNETEnable.Text = "DEXNET Enable"
        Me.ChBxTestDEXNETEnable.UseVisualStyleBackColor = True
        '
        'LiBoDEXNETStatus
        '
        Me.LiBoDEXNETStatus.ContextMenuStrip = Me.CMSTestLiBoDEXNETStatus
        Me.LiBoDEXNETStatus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LiBoDEXNETStatus.FormattingEnabled = True
        Me.LiBoDEXNETStatus.Location = New System.Drawing.Point(0, 0)
        Me.LiBoDEXNETStatus.Name = "LiBoDEXNETStatus"
        Me.LiBoDEXNETStatus.Size = New System.Drawing.Size(1230, 199)
        Me.LiBoDEXNETStatus.TabIndex = 0
        '
        'CMSTestLiBoDEXNETStatus
        '
        Me.CMSTestLiBoDEXNETStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ClearEntryToolStripMenuItem, Me.ClearAllToolStripMenuItem})
        Me.CMSTestLiBoDEXNETStatus.Name = "CMSTestLiBoDEXNETStatus"
        Me.CMSTestLiBoDEXNETStatus.Size = New System.Drawing.Size(130, 48)
        '
        'ClearEntryToolStripMenuItem
        '
        Me.ClearEntryToolStripMenuItem.Name = "ClearEntryToolStripMenuItem"
        Me.ClearEntryToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
        Me.ClearEntryToolStripMenuItem.Text = "clear entry"
        '
        'ClearAllToolStripMenuItem
        '
        Me.ClearAllToolStripMenuItem.Name = "ClearAllToolStripMenuItem"
        Me.ClearAllToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
        Me.ClearAllToolStripMenuItem.Text = "clear all"
        '
        'SplitContainer7
        '
        Me.SplitContainer7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer7.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer7.Name = "SplitContainer7"
        '
        'SplitContainer7.Panel1
        '
        Me.SplitContainer7.Panel1.Controls.Add(Me.SplitContainer3)
        '
        'SplitContainer7.Panel2
        '
        Me.SplitContainer7.Panel2.Controls.Add(Me.SplitContainer8)
        Me.SplitContainer7.Size = New System.Drawing.Size(1230, 346)
        Me.SplitContainer7.SplitterDistance = 942
        Me.SplitContainer7.TabIndex = 2
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.SplitContainer4)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.SplitContainer5)
        Me.SplitContainer3.Size = New System.Drawing.Size(942, 346)
        Me.SplitContainer3.SplitterDistance = 150
        Me.SplitContainer3.TabIndex = 1
        '
        'SplitContainer4
        '
        Me.SplitContainer4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer4.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer4.Name = "SplitContainer4"
        Me.SplitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer4.Panel1
        '
        Me.SplitContainer4.Panel1.Controls.Add(Me.BtTestAddRelKey)
        Me.SplitContainer4.Panel1.Controls.Add(Me.Label6)
        Me.SplitContainer4.Panel1.Controls.Add(Me.TBTestAddRelKey)
        Me.SplitContainer4.Panel1.Controls.Add(Me.Label9)
        '
        'SplitContainer4.Panel2
        '
        Me.SplitContainer4.Panel2.Controls.Add(Me.LiBoTestRelKeys)
        Me.SplitContainer4.Size = New System.Drawing.Size(150, 346)
        Me.SplitContainer4.SplitterDistance = 91
        Me.SplitContainer4.TabIndex = 0
        '
        'BtTestAddRelKey
        '
        Me.BtTestAddRelKey.ForeColor = System.Drawing.Color.Black
        Me.BtTestAddRelKey.Location = New System.Drawing.Point(3, 49)
        Me.BtTestAddRelKey.Name = "BtTestAddRelKey"
        Me.BtTestAddRelKey.Size = New System.Drawing.Size(96, 23)
        Me.BtTestAddRelKey.TabIndex = 9
        Me.BtTestAddRelKey.Text = "Add RelKey"
        Me.BtTestAddRelKey.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(3, 75)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(79, 13)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "Relevant Keys:"
        '
        'TBTestAddRelKey
        '
        Me.TBTestAddRelKey.Location = New System.Drawing.Point(3, 23)
        Me.TBTestAddRelKey.Name = "TBTestAddRelKey"
        Me.TBTestAddRelKey.Size = New System.Drawing.Size(96, 20)
        Me.TBTestAddRelKey.TabIndex = 7
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.ForeColor = System.Drawing.Color.Black
        Me.Label9.Location = New System.Drawing.Point(3, 7)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(96, 13)
        Me.Label9.TabIndex = 8
        Me.Label9.Text = "Add Relevant Key:"
        '
        'LiBoTestRelKeys
        '
        Me.LiBoTestRelKeys.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LiBoTestRelKeys.FormattingEnabled = True
        Me.LiBoTestRelKeys.Location = New System.Drawing.Point(0, 0)
        Me.LiBoTestRelKeys.Name = "LiBoTestRelKeys"
        Me.LiBoTestRelKeys.Size = New System.Drawing.Size(150, 251)
        Me.LiBoTestRelKeys.TabIndex = 0
        '
        'SplitContainer5
        '
        Me.SplitContainer5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer5.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer5.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer5.Name = "SplitContainer5"
        Me.SplitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer5.Panel1
        '
        Me.SplitContainer5.Panel1.Controls.Add(Me.BtTestRefreshLiBoRelMsgs)
        Me.SplitContainer5.Panel1.Controls.Add(Me.Label7)
        '
        'SplitContainer5.Panel2
        '
        Me.SplitContainer5.Panel2.Controls.Add(Me.LiBoTestRelMsgs)
        Me.SplitContainer5.Size = New System.Drawing.Size(788, 346)
        Me.SplitContainer5.SplitterDistance = 25
        Me.SplitContainer5.TabIndex = 0
        '
        'BtTestRefreshLiBoRelMsgs
        '
        Me.BtTestRefreshLiBoRelMsgs.Location = New System.Drawing.Point(114, 4)
        Me.BtTestRefreshLiBoRelMsgs.Name = "BtTestRefreshLiBoRelMsgs"
        Me.BtTestRefreshLiBoRelMsgs.Size = New System.Drawing.Size(50, 18)
        Me.BtTestRefreshLiBoRelMsgs.TabIndex = 1
        Me.BtTestRefreshLiBoRelMsgs.Text = "refresh"
        Me.BtTestRefreshLiBoRelMsgs.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(3, 7)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(104, 13)
        Me.Label7.TabIndex = 0
        Me.Label7.Text = "Relevant Messages:"
        '
        'LiBoTestRelMsgs
        '
        Me.LiBoTestRelMsgs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LiBoTestRelMsgs.FormattingEnabled = True
        Me.LiBoTestRelMsgs.Location = New System.Drawing.Point(0, 0)
        Me.LiBoTestRelMsgs.Name = "LiBoTestRelMsgs"
        Me.LiBoTestRelMsgs.Size = New System.Drawing.Size(788, 317)
        Me.LiBoTestRelMsgs.TabIndex = 0
        '
        'SplitContainer8
        '
        Me.SplitContainer8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer8.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer8.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer8.Name = "SplitContainer8"
        Me.SplitContainer8.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer8.Panel1
        '
        Me.SplitContainer8.Panel1.Controls.Add(Me.Label8)
        '
        'SplitContainer8.Panel2
        '
        Me.SplitContainer8.Panel2.Controls.Add(Me.LiBoTestPeers)
        Me.SplitContainer8.Size = New System.Drawing.Size(284, 346)
        Me.SplitContainer8.SplitterDistance = 25
        Me.SplitContainer8.TabIndex = 0
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(3, 7)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(37, 13)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "Peers:"
        '
        'LiBoTestPeers
        '
        Me.LiBoTestPeers.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LiBoTestPeers.FormattingEnabled = True
        Me.LiBoTestPeers.Location = New System.Drawing.Point(0, 0)
        Me.LiBoTestPeers.Name = "LiBoTestPeers"
        Me.LiBoTestPeers.Size = New System.Drawing.Size(284, 317)
        Me.LiBoTestPeers.TabIndex = 0
        '
        'TPgBitcoin
        '
        Me.TPgBitcoin.Controls.Add(Me.SplitContainer15)
        Me.TPgBitcoin.Location = New System.Drawing.Point(4, 22)
        Me.TPgBitcoin.Name = "TPgBitcoin"
        Me.TPgBitcoin.Size = New System.Drawing.Size(1232, 686)
        Me.TPgBitcoin.TabIndex = 8
        Me.TPgBitcoin.Text = "Bitcoin"
        Me.TPgBitcoin.UseVisualStyleBackColor = True
        '
        'SplitContainer15
        '
        Me.SplitContainer15.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer15.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer15.Name = "SplitContainer15"
        Me.SplitContainer15.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer15.Panel1
        '
        Me.SplitContainer15.Panel1.Controls.Add(Me.TBTestBitcoinAddress)
        Me.SplitContainer15.Panel1.Controls.Add(Me.GroupBox12)
        Me.SplitContainer15.Panel1.Controls.Add(Me.GroupBox11)
        Me.SplitContainer15.Panel1.Controls.Add(Me.GroupBox10)
        Me.SplitContainer15.Panel1.Controls.Add(Me.GroupBox9)
        Me.SplitContainer15.Panel1.Controls.Add(Me.GroupBox8)
        '
        'SplitContainer15.Panel2
        '
        Me.SplitContainer15.Panel2.Controls.Add(Me.RTBTestBitcoin)
        Me.SplitContainer15.Size = New System.Drawing.Size(1232, 686)
        Me.SplitContainer15.SplitterDistance = 270
        Me.SplitContainer15.TabIndex = 2
        '
        'TBTestBitcoinAddress
        '
        Me.TBTestBitcoinAddress.Location = New System.Drawing.Point(260, 38)
        Me.TBTestBitcoinAddress.Name = "TBTestBitcoinAddress"
        Me.TBTestBitcoinAddress.Size = New System.Drawing.Size(306, 20)
        Me.TBTestBitcoinAddress.TabIndex = 2
        Me.TBTestBitcoinAddress.Text = "n3E33ZG4wLwh4hRuFWNgxofhx52ANniQbY"
        '
        'GroupBox12
        '
        Me.GroupBox12.Controls.Add(Me.BtTestBitcoinScanBlocks)
        Me.GroupBox12.Controls.Add(Me.Label62)
        Me.GroupBox12.Location = New System.Drawing.Point(371, 6)
        Me.GroupBox12.Name = "GroupBox12"
        Me.GroupBox12.Size = New System.Drawing.Size(200, 156)
        Me.GroupBox12.TabIndex = 9
        Me.GroupBox12.TabStop = False
        Me.GroupBox12.Text = "Block"
        '
        'BtTestBitcoinScanBlocks
        '
        Me.BtTestBitcoinScanBlocks.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoinScanBlocks.Location = New System.Drawing.Point(9, 58)
        Me.BtTestBitcoinScanBlocks.Name = "BtTestBitcoinScanBlocks"
        Me.BtTestBitcoinScanBlocks.Size = New System.Drawing.Size(75, 23)
        Me.BtTestBitcoinScanBlocks.TabIndex = 2
        Me.BtTestBitcoinScanBlocks.Text = "scan blocks"
        Me.BtTestBitcoinScanBlocks.UseVisualStyleBackColor = True
        '
        'Label62
        '
        Me.Label62.AutoSize = True
        Me.Label62.Location = New System.Drawing.Point(6, 16)
        Me.Label62.Name = "Label62"
        Me.Label62.Size = New System.Drawing.Size(48, 13)
        Me.Label62.TabIndex = 0
        Me.Label62.Text = "Address:"
        '
        'GroupBox11
        '
        Me.GroupBox11.Controls.Add(Me.BtTestBitcoinGetTransaction)
        Me.GroupBox11.Controls.Add(Me.BtTestBitcoinGetRawTransaction)
        Me.GroupBox11.Controls.Add(Me.Label61)
        Me.GroupBox11.Controls.Add(Me.TBTestBitcoinTransactionID)
        Me.GroupBox11.Location = New System.Drawing.Point(578, 6)
        Me.GroupBox11.Name = "GroupBox11"
        Me.GroupBox11.Size = New System.Drawing.Size(119, 156)
        Me.GroupBox11.TabIndex = 8
        Me.GroupBox11.TabStop = False
        Me.GroupBox11.Text = "Transaction"
        '
        'BtTestBitcoinGetTransaction
        '
        Me.BtTestBitcoinGetTransaction.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoinGetTransaction.Location = New System.Drawing.Point(6, 87)
        Me.BtTestBitcoinGetTransaction.Name = "BtTestBitcoinGetTransaction"
        Me.BtTestBitcoinGetTransaction.Size = New System.Drawing.Size(106, 23)
        Me.BtTestBitcoinGetTransaction.TabIndex = 3
        Me.BtTestBitcoinGetTransaction.Text = "get transaction"
        Me.BtTestBitcoinGetTransaction.UseVisualStyleBackColor = True
        '
        'BtTestBitcoinGetRawTransaction
        '
        Me.BtTestBitcoinGetRawTransaction.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoinGetRawTransaction.Location = New System.Drawing.Point(6, 58)
        Me.BtTestBitcoinGetRawTransaction.Name = "BtTestBitcoinGetRawTransaction"
        Me.BtTestBitcoinGetRawTransaction.Size = New System.Drawing.Size(106, 23)
        Me.BtTestBitcoinGetRawTransaction.TabIndex = 2
        Me.BtTestBitcoinGetRawTransaction.Text = "get raw transaction"
        Me.BtTestBitcoinGetRawTransaction.UseVisualStyleBackColor = True
        '
        'Label61
        '
        Me.Label61.AutoSize = True
        Me.Label61.Location = New System.Drawing.Point(6, 16)
        Me.Label61.Name = "Label61"
        Me.Label61.Size = New System.Drawing.Size(80, 13)
        Me.Label61.TabIndex = 1
        Me.Label61.Text = "Transaction ID:"
        '
        'TBTestBitcoinTransactionID
        '
        Me.TBTestBitcoinTransactionID.Location = New System.Drawing.Point(6, 32)
        Me.TBTestBitcoinTransactionID.Name = "TBTestBitcoinTransactionID"
        Me.TBTestBitcoinTransactionID.Size = New System.Drawing.Size(106, 20)
        Me.TBTestBitcoinTransactionID.TabIndex = 0
        Me.TBTestBitcoinTransactionID.Text = "00fafdf52acd69839f60fe20ad0adc2352bb1f737720997c19b4f9e24c22e832"
        '
        'GroupBox10
        '
        Me.GroupBox10.Controls.Add(Me.BtTestBitcoinGetReceivedByAddress)
        Me.GroupBox10.Controls.Add(Me.BtTestBitcoinListUnspent)
        Me.GroupBox10.Controls.Add(Me.BtTestBitcoinGetDescriptorInfo)
        Me.GroupBox10.Controls.Add(Me.BtTestBitcoinImportDescriptor)
        Me.GroupBox10.Controls.Add(Me.Label59)
        Me.GroupBox10.Controls.Add(Me.BtTestBitcoinGetUTXOSet)
        Me.GroupBox10.Location = New System.Drawing.Point(253, 6)
        Me.GroupBox10.Name = "GroupBox10"
        Me.GroupBox10.Size = New System.Drawing.Size(112, 207)
        Me.GroupBox10.TabIndex = 7
        Me.GroupBox10.TabStop = False
        Me.GroupBox10.Text = "Address"
        '
        'BtTestBitcoinGetReceivedByAddress
        '
        Me.BtTestBitcoinGetReceivedByAddress.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoinGetReceivedByAddress.Location = New System.Drawing.Point(6, 174)
        Me.BtTestBitcoinGetReceivedByAddress.Name = "BtTestBitcoinGetReceivedByAddress"
        Me.BtTestBitcoinGetReceivedByAddress.Size = New System.Drawing.Size(100, 23)
        Me.BtTestBitcoinGetReceivedByAddress.TabIndex = 8
        Me.BtTestBitcoinGetReceivedByAddress.Text = "get by address"
        Me.BtTestBitcoinGetReceivedByAddress.UseVisualStyleBackColor = True
        '
        'BtTestBitcoinListUnspent
        '
        Me.BtTestBitcoinListUnspent.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoinListUnspent.Location = New System.Drawing.Point(6, 145)
        Me.BtTestBitcoinListUnspent.Name = "BtTestBitcoinListUnspent"
        Me.BtTestBitcoinListUnspent.Size = New System.Drawing.Size(100, 23)
        Me.BtTestBitcoinListUnspent.TabIndex = 7
        Me.BtTestBitcoinListUnspent.Text = "list unspent"
        Me.BtTestBitcoinListUnspent.UseVisualStyleBackColor = True
        '
        'BtTestBitcoinGetDescriptorInfo
        '
        Me.BtTestBitcoinGetDescriptorInfo.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoinGetDescriptorInfo.Location = New System.Drawing.Point(6, 116)
        Me.BtTestBitcoinGetDescriptorInfo.Name = "BtTestBitcoinGetDescriptorInfo"
        Me.BtTestBitcoinGetDescriptorInfo.Size = New System.Drawing.Size(100, 23)
        Me.BtTestBitcoinGetDescriptorInfo.TabIndex = 6
        Me.BtTestBitcoinGetDescriptorInfo.Text = "get descriptor info"
        Me.BtTestBitcoinGetDescriptorInfo.UseVisualStyleBackColor = True
        '
        'BtTestBitcoinImportDescriptor
        '
        Me.BtTestBitcoinImportDescriptor.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoinImportDescriptor.Location = New System.Drawing.Point(6, 87)
        Me.BtTestBitcoinImportDescriptor.Name = "BtTestBitcoinImportDescriptor"
        Me.BtTestBitcoinImportDescriptor.Size = New System.Drawing.Size(100, 23)
        Me.BtTestBitcoinImportDescriptor.TabIndex = 4
        Me.BtTestBitcoinImportDescriptor.Text = "import descriptor"
        Me.BtTestBitcoinImportDescriptor.UseVisualStyleBackColor = True
        '
        'Label59
        '
        Me.Label59.AutoSize = True
        Me.Label59.ForeColor = System.Drawing.Color.Black
        Me.Label59.Location = New System.Drawing.Point(6, 16)
        Me.Label59.Name = "Label59"
        Me.Label59.Size = New System.Drawing.Size(48, 13)
        Me.Label59.TabIndex = 3
        Me.Label59.Text = "Address:"
        '
        'BtTestBitcoinGetUTXOSet
        '
        Me.BtTestBitcoinGetUTXOSet.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoinGetUTXOSet.Location = New System.Drawing.Point(6, 58)
        Me.BtTestBitcoinGetUTXOSet.Name = "BtTestBitcoinGetUTXOSet"
        Me.BtTestBitcoinGetUTXOSet.Size = New System.Drawing.Size(100, 23)
        Me.BtTestBitcoinGetUTXOSet.TabIndex = 1
        Me.BtTestBitcoinGetUTXOSet.Text = "getutxoset"
        Me.BtTestBitcoinGetUTXOSet.UseVisualStyleBackColor = True
        '
        'GroupBox9
        '
        Me.GroupBox9.Controls.Add(Me.BtTestBitcoionLoadWallet)
        Me.GroupBox9.Controls.Add(Me.BtTestBitcoinWalletName)
        Me.GroupBox9.Controls.Add(Me.Label60)
        Me.GroupBox9.Controls.Add(Me.TBTestBitcoinWalletName)
        Me.GroupBox9.Location = New System.Drawing.Point(107, 6)
        Me.GroupBox9.Name = "GroupBox9"
        Me.GroupBox9.Size = New System.Drawing.Size(140, 156)
        Me.GroupBox9.TabIndex = 6
        Me.GroupBox9.TabStop = False
        Me.GroupBox9.Text = "Wallet"
        '
        'BtTestBitcoionLoadWallet
        '
        Me.BtTestBitcoionLoadWallet.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoionLoadWallet.Location = New System.Drawing.Point(6, 87)
        Me.BtTestBitcoionLoadWallet.Name = "BtTestBitcoionLoadWallet"
        Me.BtTestBitcoionLoadWallet.Size = New System.Drawing.Size(126, 23)
        Me.BtTestBitcoionLoadWallet.TabIndex = 3
        Me.BtTestBitcoionLoadWallet.Text = "load wallet"
        Me.BtTestBitcoionLoadWallet.UseVisualStyleBackColor = True
        '
        'BtTestBitcoinWalletName
        '
        Me.BtTestBitcoinWalletName.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoinWalletName.Location = New System.Drawing.Point(6, 58)
        Me.BtTestBitcoinWalletName.Name = "BtTestBitcoinWalletName"
        Me.BtTestBitcoinWalletName.Size = New System.Drawing.Size(126, 23)
        Me.BtTestBitcoinWalletName.TabIndex = 2
        Me.BtTestBitcoinWalletName.Text = "create descriptor wallet"
        Me.BtTestBitcoinWalletName.UseVisualStyleBackColor = True
        '
        'Label60
        '
        Me.Label60.AutoSize = True
        Me.Label60.Location = New System.Drawing.Point(6, 16)
        Me.Label60.Name = "Label60"
        Me.Label60.Size = New System.Drawing.Size(38, 13)
        Me.Label60.TabIndex = 1
        Me.Label60.Text = "Name:"
        '
        'TBTestBitcoinWalletName
        '
        Me.TBTestBitcoinWalletName.Location = New System.Drawing.Point(6, 32)
        Me.TBTestBitcoinWalletName.Name = "TBTestBitcoinWalletName"
        Me.TBTestBitcoinWalletName.Size = New System.Drawing.Size(126, 20)
        Me.TBTestBitcoinWalletName.TabIndex = 0
        Me.TBTestBitcoinWalletName.Text = "TESTWALLET"
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.BtTestBitcoinGetBalance)
        Me.GroupBox8.Controls.Add(Me.BtTestBitcoinGetBalances)
        Me.GroupBox8.Controls.Add(Me.BtTestBitcoinGetInfo)
        Me.GroupBox8.Controls.Add(Me.BtTestBitcoinGetWalletInfo)
        Me.GroupBox8.Location = New System.Drawing.Point(8, 3)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Size = New System.Drawing.Size(93, 159)
        Me.GroupBox8.TabIndex = 5
        Me.GroupBox8.TabStop = False
        Me.GroupBox8.Text = "Info"
        '
        'BtTestBitcoinGetBalance
        '
        Me.BtTestBitcoinGetBalance.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoinGetBalance.Location = New System.Drawing.Point(6, 77)
        Me.BtTestBitcoinGetBalance.Name = "BtTestBitcoinGetBalance"
        Me.BtTestBitcoinGetBalance.Size = New System.Drawing.Size(80, 23)
        Me.BtTestBitcoinGetBalance.TabIndex = 6
        Me.BtTestBitcoinGetBalance.Text = "getBalance"
        Me.BtTestBitcoinGetBalance.UseVisualStyleBackColor = True
        '
        'BtTestBitcoinGetBalances
        '
        Me.BtTestBitcoinGetBalances.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoinGetBalances.Location = New System.Drawing.Point(6, 106)
        Me.BtTestBitcoinGetBalances.Name = "BtTestBitcoinGetBalances"
        Me.BtTestBitcoinGetBalances.Size = New System.Drawing.Size(80, 23)
        Me.BtTestBitcoinGetBalances.TabIndex = 5
        Me.BtTestBitcoinGetBalances.Text = "getBalances"
        Me.BtTestBitcoinGetBalances.UseVisualStyleBackColor = True
        '
        'BtTestBitcoinGetInfo
        '
        Me.BtTestBitcoinGetInfo.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoinGetInfo.Location = New System.Drawing.Point(6, 19)
        Me.BtTestBitcoinGetInfo.Name = "BtTestBitcoinGetInfo"
        Me.BtTestBitcoinGetInfo.Size = New System.Drawing.Size(80, 23)
        Me.BtTestBitcoinGetInfo.TabIndex = 0
        Me.BtTestBitcoinGetInfo.Text = "getInfo"
        Me.BtTestBitcoinGetInfo.UseVisualStyleBackColor = True
        '
        'BtTestBitcoinGetWalletInfo
        '
        Me.BtTestBitcoinGetWalletInfo.ForeColor = System.Drawing.Color.Black
        Me.BtTestBitcoinGetWalletInfo.Location = New System.Drawing.Point(5, 48)
        Me.BtTestBitcoinGetWalletInfo.Name = "BtTestBitcoinGetWalletInfo"
        Me.BtTestBitcoinGetWalletInfo.Size = New System.Drawing.Size(81, 23)
        Me.BtTestBitcoinGetWalletInfo.TabIndex = 4
        Me.BtTestBitcoinGetWalletInfo.Text = "getWalletInfo"
        Me.BtTestBitcoinGetWalletInfo.UseVisualStyleBackColor = True
        '
        'RTBTestBitcoin
        '
        Me.RTBTestBitcoin.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RTBTestBitcoin.ForeColor = System.Drawing.Color.Black
        Me.RTBTestBitcoin.Location = New System.Drawing.Point(0, 0)
        Me.RTBTestBitcoin.Name = "RTBTestBitcoin"
        Me.RTBTestBitcoin.Size = New System.Drawing.Size(1232, 412)
        Me.RTBTestBitcoin.TabIndex = 1
        Me.RTBTestBitcoin.Text = ""
        '
        'DevTimer
        '
        Me.DevTimer.Enabled = True
        '
        'SplitContainer11
        '
        Me.SplitContainer11.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(153, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.SplitContainer11.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer11.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer11.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer11.Name = "SplitContainer11"
        Me.SplitContainer11.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer11.Panel1
        '
        Me.SplitContainer11.Panel1.Controls.Add(Me.Label32)
        Me.SplitContainer11.Panel1.Controls.Add(Me.TBTestMyPassPhrase)
        Me.SplitContainer11.Panel1.Controls.Add(Me.TBTestMyAgreeKey)
        Me.SplitContainer11.Panel1.Controls.Add(Me.Label30)
        Me.SplitContainer11.Panel1.Controls.Add(Me.TBTestMySignKey)
        Me.SplitContainer11.Panel1.Controls.Add(Me.Label29)
        Me.SplitContainer11.Panel1.Controls.Add(Me.TBTestMyPublicKey)
        Me.SplitContainer11.Panel1.Controls.Add(Me.Label28)
        '
        'SplitContainer11.Panel2
        '
        Me.SplitContainer11.Panel2.Controls.Add(Me.TestTabControl)
        Me.SplitContainer11.Size = New System.Drawing.Size(1240, 799)
        Me.SplitContainer11.SplitterDistance = 83
        Me.SplitContainer11.TabIndex = 11
        '
        'Label32
        '
        Me.Label32.AutoSize = True
        Me.Label32.ForeColor = System.Drawing.Color.White
        Me.Label32.Location = New System.Drawing.Point(515, 9)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(83, 13)
        Me.Label32.TabIndex = 8
        Me.Label32.Text = "My PassPhrase:"
        '
        'TBTestMyPassPhrase
        '
        Me.TBTestMyPassPhrase.Location = New System.Drawing.Point(604, 6)
        Me.TBTestMyPassPhrase.Name = "TBTestMyPassPhrase"
        Me.TBTestMyPassPhrase.Size = New System.Drawing.Size(401, 20)
        Me.TBTestMyPassPhrase.TabIndex = 7
        '
        'TBTestMyAgreeKey
        '
        Me.TBTestMyAgreeKey.Location = New System.Drawing.Point(88, 58)
        Me.TBTestMyAgreeKey.Name = "TBTestMyAgreeKey"
        Me.TBTestMyAgreeKey.Size = New System.Drawing.Size(422, 20)
        Me.TBTestMyAgreeKey.TabIndex = 6
        '
        'Label30
        '
        Me.Label30.AutoSize = True
        Me.Label30.ForeColor = System.Drawing.Color.White
        Me.Label30.Location = New System.Drawing.Point(5, 61)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(76, 13)
        Me.Label30.TabIndex = 5
        Me.Label30.Text = "My Agree Key:"
        '
        'TBTestMySignKey
        '
        Me.TBTestMySignKey.Location = New System.Drawing.Point(88, 32)
        Me.TBTestMySignKey.Name = "TBTestMySignKey"
        Me.TBTestMySignKey.Size = New System.Drawing.Size(422, 20)
        Me.TBTestMySignKey.TabIndex = 4
        '
        'Label29
        '
        Me.Label29.AutoSize = True
        Me.Label29.ForeColor = System.Drawing.Color.White
        Me.Label29.Location = New System.Drawing.Point(5, 35)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(69, 13)
        Me.Label29.TabIndex = 3
        Me.Label29.Text = "My Sign Key:"
        '
        'TBTestMyPublicKey
        '
        Me.TBTestMyPublicKey.Location = New System.Drawing.Point(88, 6)
        Me.TBTestMyPublicKey.Name = "TBTestMyPublicKey"
        Me.TBTestMyPublicKey.Size = New System.Drawing.Size(422, 20)
        Me.TBTestMyPublicKey.TabIndex = 2
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.ForeColor = System.Drawing.Color.White
        Me.Label28.Location = New System.Drawing.Point(5, 9)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(77, 13)
        Me.Label28.TabIndex = 0
        Me.Label28.Text = "My Public Key:"
        '
        'TPgSignumTX
        '
        Me.TPgSignumTX.Controls.Add(Me.RTBTestSigTX)
        Me.TPgSignumTX.Controls.Add(Me.Label66)
        Me.TPgSignumTX.Controls.Add(Me.BtTestSigTX_Decrypt)
        Me.TPgSignumTX.Controls.Add(Me.LVTestSigTX)
        Me.TPgSignumTX.Controls.Add(Me.BtTestSigTX_ReadTX)
        Me.TPgSignumTX.Controls.Add(Me.TBTestSigTX_PassPhrase)
        Me.TPgSignumTX.Controls.Add(Me.Label65)
        Me.TPgSignumTX.Controls.Add(Me.TBTestSigTX_TXID)
        Me.TPgSignumTX.Controls.Add(Me.Label64)
        Me.TPgSignumTX.Location = New System.Drawing.Point(4, 22)
        Me.TPgSignumTX.Name = "TPgSignumTX"
        Me.TPgSignumTX.Size = New System.Drawing.Size(1232, 686)
        Me.TPgSignumTX.TabIndex = 9
        Me.TPgSignumTX.Text = "Signum Transactions"
        Me.TPgSignumTX.UseVisualStyleBackColor = True
        '
        'Label64
        '
        Me.Label64.AutoSize = True
        Me.Label64.ForeColor = System.Drawing.Color.Black
        Me.Label64.Location = New System.Drawing.Point(8, 6)
        Me.Label64.Name = "Label64"
        Me.Label64.Size = New System.Drawing.Size(77, 13)
        Me.Label64.TabIndex = 0
        Me.Label64.Text = "TransactionID:"
        '
        'TBTestSigTX_TXID
        '
        Me.TBTestSigTX_TXID.Location = New System.Drawing.Point(11, 22)
        Me.TBTestSigTX_TXID.Name = "TBTestSigTX_TXID"
        Me.TBTestSigTX_TXID.Size = New System.Drawing.Size(200, 20)
        Me.TBTestSigTX_TXID.TabIndex = 1
        Me.TBTestSigTX_TXID.Text = "8637898249465223694"
        '
        'TBTestSigTX_PassPhrase
        '
        Me.TBTestSigTX_PassPhrase.Location = New System.Drawing.Point(11, 61)
        Me.TBTestSigTX_PassPhrase.Name = "TBTestSigTX_PassPhrase"
        Me.TBTestSigTX_PassPhrase.Size = New System.Drawing.Size(376, 20)
        Me.TBTestSigTX_PassPhrase.TabIndex = 3
        Me.TBTestSigTX_PassPhrase.UseSystemPasswordChar = True
        '
        'Label65
        '
        Me.Label65.AutoSize = True
        Me.Label65.ForeColor = System.Drawing.Color.Black
        Me.Label65.Location = New System.Drawing.Point(8, 45)
        Me.Label65.Name = "Label65"
        Me.Label65.Size = New System.Drawing.Size(66, 13)
        Me.Label65.TabIndex = 2
        Me.Label65.Text = "PassPhrase:"
        '
        'BtTestSigTX_ReadTX
        '
        Me.BtTestSigTX_ReadTX.ForeColor = System.Drawing.Color.Black
        Me.BtTestSigTX_ReadTX.Location = New System.Drawing.Point(11, 87)
        Me.BtTestSigTX_ReadTX.Name = "BtTestSigTX_ReadTX"
        Me.BtTestSigTX_ReadTX.Size = New System.Drawing.Size(200, 23)
        Me.BtTestSigTX_ReadTX.TabIndex = 4
        Me.BtTestSigTX_ReadTX.Text = "read Transaction"
        Me.BtTestSigTX_ReadTX.UseVisualStyleBackColor = True
        '
        'LVTestSigTX
        '
        Me.LVTestSigTX.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader16, Me.ColumnHeader17})
        Me.LVTestSigTX.FullRowSelect = True
        Me.LVTestSigTX.GridLines = True
        Me.LVTestSigTX.HideSelection = False
        Me.LVTestSigTX.Location = New System.Drawing.Point(11, 116)
        Me.LVTestSigTX.MultiSelect = False
        Me.LVTestSigTX.Name = "LVTestSigTX"
        Me.LVTestSigTX.Size = New System.Drawing.Size(1213, 329)
        Me.LVTestSigTX.TabIndex = 5
        Me.LVTestSigTX.UseCompatibleStateImageBehavior = False
        Me.LVTestSigTX.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader16
        '
        Me.ColumnHeader16.Text = "Property"
        Me.ColumnHeader16.Width = 184
        '
        'ColumnHeader17
        '
        Me.ColumnHeader17.Text = "Value"
        Me.ColumnHeader17.Width = 1020
        '
        'BtTestSigTX_Decrypt
        '
        Me.BtTestSigTX_Decrypt.ForeColor = System.Drawing.Color.Black
        Me.BtTestSigTX_Decrypt.Location = New System.Drawing.Point(217, 87)
        Me.BtTestSigTX_Decrypt.Name = "BtTestSigTX_Decrypt"
        Me.BtTestSigTX_Decrypt.Size = New System.Drawing.Size(170, 23)
        Me.BtTestSigTX_Decrypt.TabIndex = 6
        Me.BtTestSigTX_Decrypt.Text = "decrypt attachment"
        Me.BtTestSigTX_Decrypt.UseVisualStyleBackColor = True
        '
        'Label66
        '
        Me.Label66.AutoSize = True
        Me.Label66.Location = New System.Drawing.Point(8, 448)
        Me.Label66.Name = "Label66"
        Me.Label66.Size = New System.Drawing.Size(53, 13)
        Me.Label66.TabIndex = 7
        Me.Label66.Text = "Message:"
        '
        'RTBTestSigTX
        '
        Me.RTBTestSigTX.Location = New System.Drawing.Point(11, 464)
        Me.RTBTestSigTX.Name = "RTBTestSigTX"
        Me.RTBTestSigTX.Size = New System.Drawing.Size(1213, 81)
        Me.RTBTestSigTX.TabIndex = 8
        Me.RTBTestSigTX.Text = ""
        '
        'FrmDevelope
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(153, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1240, 799)
        Me.Controls.Add(Me.SplitContainer11)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FrmDevelope"
        Me.Text = "FrmDevelope"
        Me.TestTabControl.ResumeLayout(False)
        Me.TPgConvertings.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.TestGrpBoxConvertings.ResumeLayout(False)
        Me.TestGrpBoxConvertings.PerformLayout()
        Me.TPgINITools.ResumeLayout(False)
        Me.TestGrpBoxINITests.ResumeLayout(False)
        Me.TestGrpBoxINITests.PerformLayout()
        Me.TPgPayPal.ResumeLayout(False)
        Me.SplitContainer10.Panel1.ResumeLayout(False)
        Me.SplitContainer10.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer10, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer10.ResumeLayout(False)
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.TPgAT_SC_Comm.ResumeLayout(False)
        Me.TPgAT_SC_Comm.PerformLayout()
        Me.TestGrpBxATCom.ResumeLayout(False)
        Me.TestGrpBxATCom.PerformLayout()
        CType(Me.NUDTestAmount, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUDTestXAmount, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        CType(Me.NUDTestCollateral, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.Panel6.ResumeLayout(False)
        Me.Panel6.PerformLayout()
        Me.Panel7.ResumeLayout(False)
        Me.Panel7.PerformLayout()
        CType(Me.NUDTestMediateAmount, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TPgMultithreadings.ResumeLayout(False)
        Me.TestGrpBoxMultithreadings.ResumeLayout(False)
        Me.SplitContainer9.Panel1.ResumeLayout(False)
        Me.SplitContainer9.Panel1.PerformLayout()
        Me.SplitContainer9.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer9, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer9.ResumeLayout(False)
        Me.SplitContainer12.Panel1.ResumeLayout(False)
        Me.SplitContainer12.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer12, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer12.ResumeLayout(False)
        Me.SplitContainer13.Panel1.ResumeLayout(False)
        Me.SplitContainer13.Panel1.PerformLayout()
        Me.SplitContainer13.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer13, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer13.ResumeLayout(False)
        Me.SplitContainer14.Panel1.ResumeLayout(False)
        Me.SplitContainer14.Panel1.PerformLayout()
        Me.SplitContainer14.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer14, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer14.ResumeLayout(False)
        Me.TPgTCPAPI.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.TPgMultiinvokes.ResumeLayout(False)
        Me.TPgDEXNET.ResumeLayout(False)
        Me.SplitContainer6.Panel1.ResumeLayout(False)
        Me.SplitContainer6.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer6, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer6.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.PerformLayout()
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.CMSTestLiBoDEXNETStatus.ResumeLayout(False)
        Me.SplitContainer7.Panel1.ResumeLayout(False)
        Me.SplitContainer7.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer7, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer7.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.SplitContainer4.Panel1.ResumeLayout(False)
        Me.SplitContainer4.Panel1.PerformLayout()
        Me.SplitContainer4.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer4.ResumeLayout(False)
        Me.SplitContainer5.Panel1.ResumeLayout(False)
        Me.SplitContainer5.Panel1.PerformLayout()
        Me.SplitContainer5.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer5.ResumeLayout(False)
        Me.SplitContainer8.Panel1.ResumeLayout(False)
        Me.SplitContainer8.Panel1.PerformLayout()
        Me.SplitContainer8.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer8, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer8.ResumeLayout(False)
        Me.TPgBitcoin.ResumeLayout(False)
        Me.SplitContainer15.Panel1.ResumeLayout(False)
        Me.SplitContainer15.Panel1.PerformLayout()
        Me.SplitContainer15.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer15, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer15.ResumeLayout(False)
        Me.GroupBox12.ResumeLayout(False)
        Me.GroupBox12.PerformLayout()
        Me.GroupBox11.ResumeLayout(False)
        Me.GroupBox11.PerformLayout()
        Me.GroupBox10.ResumeLayout(False)
        Me.GroupBox10.PerformLayout()
        Me.GroupBox9.ResumeLayout(False)
        Me.GroupBox9.PerformLayout()
        Me.GroupBox8.ResumeLayout(False)
        Me.SplitContainer11.Panel1.ResumeLayout(False)
        Me.SplitContainer11.Panel1.PerformLayout()
        Me.SplitContainer11.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer11, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer11.ResumeLayout(False)
        Me.TPgSignumTX.ResumeLayout(False)
        Me.TPgSignumTX.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TestTabControl As TabControl
    Friend WithEvents TPgConvertings As TabPage
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents BtTestTimeConvert As Button
    Friend WithEvents TBTestTime As TextBox
    Friend WithEvents TestGrpBoxConvertings As GroupBox
    Friend WithEvents TBTestConvert As TextBox
    Friend WithEvents BtTestConvert As Button
    Friend WithEvents BtTestConvert2 As Button
    Friend WithEvents BtTestDatStr2ULngList As Button
    Friend WithEvents TPgINITools As TabPage
    Friend WithEvents TestGrpBoxINITests As GroupBox
    Friend WithEvents TBTestSetTXINI As TextBox
    Friend WithEvents BtTestSetTXINI As Button
    Friend WithEvents BtTestGetTXINI As Button
    Friend WithEvents BtTestDelTXINI As Button
    Friend WithEvents TBTestGetTXINI As TextBox
    Friend WithEvents TBTestDelTXINI As TextBox
    Friend WithEvents TPgPayPal As TabPage
    Friend WithEvents BtTestPPGetAllTX As Button
    Friend WithEvents BtTestCreatePPOrder As Button
    Friend WithEvents TPgAT_SC_Comm As TabPage
    Friend WithEvents TPgMultithreadings As TabPage
    Friend WithEvents TestGrpBoxMultithreadings As GroupBox
    Friend WithEvents LVTestMulti As ListView
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents ColumnHeader3 As ColumnHeader
    Friend WithEvents ColumnHeader4 As ColumnHeader
    Friend WithEvents ColumnHeader5 As ColumnHeader
    Friend WithEvents ColumnHeader6 As ColumnHeader
    Friend WithEvents BtTestExit As Button
    Friend WithEvents BtTestMultiRefresh As Button
    Friend WithEvents TPgTCPAPI As TabPage
    Friend WithEvents LiBoTCPAPIStatus As ListBox
    Friend WithEvents TPgMultiinvokes As TabPage
    Friend WithEvents LVTestMultiInvoke As ListView
    Friend WithEvents ColumnHeader7 As ColumnHeader
    Friend WithEvents ColumnHeader8 As ColumnHeader
    Friend WithEvents ColumnHeader10 As ColumnHeader
    Friend WithEvents ColumnHeader11 As ColumnHeader
    Friend WithEvents BtTestMultiInvokeSetInLV As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents BtTestCreateCollWord As Button
    Friend WithEvents TBTestCollWordOutput As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents TBTestCollWordInput As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents ChBxTestTCPAPIEnable As CheckBox
    Friend WithEvents ChBxTestTCPAPIShowStatus As CheckBox
    Friend WithEvents TPgDEXNET As TabPage
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents LiBoDEXNETStatus As ListBox
    Friend WithEvents ChBxTestDEXNETEnable As CheckBox
    Friend WithEvents ChBxTestDEXNETShowStatus As CheckBox
    Friend WithEvents BtTestConnect As Button
    Friend WithEvents TBTestPeerPort As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents TBTestNewPeer As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents SplitContainer6 As SplitContainer
    Friend WithEvents SplitContainer3 As SplitContainer
    Friend WithEvents SplitContainer4 As SplitContainer
    Friend WithEvents SplitContainer5 As SplitContainer
    Friend WithEvents Label6 As Label
    Friend WithEvents LiBoTestRelKeys As ListBox
    Friend WithEvents Label7 As Label
    Friend WithEvents LiBoTestRelMsgs As ListBox
    Friend WithEvents DevTimer As Timer
    Friend WithEvents SplitContainer7 As SplitContainer
    Friend WithEvents SplitContainer8 As SplitContainer
    Friend WithEvents Label8 As Label
    Friend WithEvents LiBoTestPeers As ListBox
    Friend WithEvents BtTestAddRelKey As Button
    Friend WithEvents Label9 As Label
    Friend WithEvents TBTestAddRelKey As TextBox
    Friend WithEvents BtTestBroadcastMsg As Button
    Friend WithEvents TBTestRecipientPubKey As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents TBTestBroadcastMessage As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents SplitContainer9 As SplitContainer
    Friend WithEvents ChBxAutoRefreshMultiThreads As CheckBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents TBTestXMLOutput As TextBox
    Friend WithEvents Label13 As Label
    Friend WithEvents BtTestJSONToXML As Button
    Friend WithEvents TBTestJSONInput As TextBox
    Friend WithEvents Label12 As Label
    Friend WithEvents BtTestRecursiveXMLSearch As Button
    Friend WithEvents TBTestRecursiveXMLSearch As TextBox
    Friend WithEvents Label14 As Label
    Friend WithEvents SplitContainer10 As SplitContainer
    Friend WithEvents LiBoPayPalComs As ListBox
    Friend WithEvents BtTestPPGetTXWithNote As Button
    Friend WithEvents TBTestPPTXNote As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents BtTestPPCheckAPI As Button
    Friend WithEvents GroupBox6 As GroupBox
    Friend WithEvents Label17 As Label
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents Label18 As Label
    Friend WithEvents TBTestPPXAmount As TextBox
    Friend WithEvents Label19 As Label
    Friend WithEvents TBTestPPXItem As TextBox
    Friend WithEvents TBTestPPPrice As TextBox
    Friend WithEvents CoBxTestPPCurrency As ComboBox
    Friend WithEvents GroupBox7 As GroupBox
    Friend WithEvents Label16 As Label
    Friend WithEvents Label26 As Label
    Friend WithEvents Label20 As Label
    Friend WithEvents BtTestPPPayout As Button
    Friend WithEvents TBTestPPPONote As TextBox
    Friend WithEvents TBTestPPPOCurrency As TextBox
    Friend WithEvents TBTestPPPOAmount As TextBox
    Friend WithEvents TBTestPPPORecipient As TextBox
    Friend WithEvents Label27 As Label
    Friend WithEvents ChBxDEXNETEncryptMsg As CheckBox
    Friend WithEvents SplitContainer11 As SplitContainer
    Friend WithEvents TBTestMyAgreeKey As TextBox
    Friend WithEvents Label30 As Label
    Friend WithEvents TBTestMySignKey As TextBox
    Friend WithEvents Label29 As Label
    Friend WithEvents TBTestMyPublicKey As TextBox
    Friend WithEvents Label28 As Label
    Friend WithEvents TBTestMyPassPhrase As TextBox
    Friend WithEvents Label32 As Label
    Friend WithEvents CMSTestLiBoDEXNETStatus As ContextMenuStrip
    Friend WithEvents ClearEntryToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Label36 As Label
    Friend WithEvents LabActiveNodes As Label
    Friend WithEvents SplitContainer12 As SplitContainer
    Friend WithEvents SplitContainer13 As SplitContainer
    Friend WithEvents SplitContainer14 As SplitContainer
    Friend WithEvents Label37 As Label
    Friend WithEvents Label38 As Label
    Friend WithEvents LVActiveNodes As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader15 As ColumnHeader
    Friend WithEvents BtTestHex2ULng As Button
    Friend WithEvents TestGrpBxATCom As GroupBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label21 As Label
    Friend WithEvents LVTestCurrentOrder As ListView
    Friend WithEvents ColumnHeader9 As ColumnHeader
    Friend WithEvents ColumnHeader12 As ColumnHeader
    Friend WithEvents LVTestDEXContractBasic As ListView
    Friend WithEvents ColumnHeader13 As ColumnHeader
    Friend WithEvents ColumnHeader14 As ColumnHeader
    Friend WithEvents Label31 As Label
    Friend WithEvents Label40 As Label
    Friend WithEvents BtTestLoadDEXContract As Button
    Friend WithEvents BtExport As Button
    Friend WithEvents NUDTestAmount As NumericUpDown
    Friend WithEvents NUDTestXAmount As NumericUpDown
    Friend WithEvents CoBxTestATComATID As ComboBox
    Friend WithEvents TBTestXItem As TextBox
    Friend WithEvents TBTestPP As TextBox
    Friend WithEvents Panel4 As Panel
    Friend WithEvents TB_TestCreateWithResponder As TextBox
    Friend WithEvents BtTestCreateWithResponder As Button
    Friend WithEvents Label22 As Label
    Friend WithEvents Label23 As Label
    Friend WithEvents Label24 As Label
    Friend WithEvents Label25 As Label
    Friend WithEvents BtTestCheckCloseDispute As Button
    Friend WithEvents BtTestAppeal As Button
    Friend WithEvents BtTestOpenDispute As Button
    Friend WithEvents BtTestDeActivateDeniability As Button
    Friend WithEvents NUDTestCollateral As NumericUpDown
    Friend WithEvents BtTestFinish As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents BtTestCreate As Button
    Friend WithEvents LabAmount As Label
    Friend WithEvents Label35 As Label
    Friend WithEvents ChBxSellOrder As CheckBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents BtTestAccept As Button
    Friend WithEvents Panel3 As Panel
    Friend WithEvents TBTestResponder As TextBox
    Friend WithEvents Label33 As Label
    Friend WithEvents BtTestInjectResponder As Button
    Friend WithEvents Panel5 As Panel
    Friend WithEvents Label34 As Label
    Friend WithEvents TBTestChainSwapHashLong1 As TextBox
    Friend WithEvents Label39 As Label
    Friend WithEvents BtTestChainSwapKeyToHash As Button
    Friend WithEvents TBTestChainSwapKey As TextBox
    Friend WithEvents BtTestInjectChainSwapHash As Button
    Friend WithEvents Panel6 As Panel
    Friend WithEvents Label42 As Label
    Friend WithEvents TBTestChainSwapKeyULong3 As TextBox
    Friend WithEvents TBTestChainSwapKeyULong2 As TextBox
    Friend WithEvents TBTestChainSwapKeyULong1 As TextBox
    Friend WithEvents Label43 As Label
    Friend WithEvents Label44 As Label
    Friend WithEvents BtTestFinishWithChainSwapKey As Button
    Friend WithEvents Panel7 As Panel
    Friend WithEvents Label45 As Label
    Friend WithEvents NUDTestMediateAmount As NumericUpDown
    Friend WithEvents BtTestMediateDispute As Button
    Friend WithEvents BtTestRefreshLiBoRelMsgs As Button
    Friend WithEvents BtCSKConvertback As Button
    Friend WithEvents TBTestChainSwapHashLong3 As TextBox
    Friend WithEvents Label46 As Label
    Friend WithEvents TBTestChainSwapHashLong2 As TextBox
    Friend WithEvents Label41 As Label
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents TBTestHashUL4 As TextBox
    Friend WithEvents Label54 As Label
    Friend WithEvents TBTestHashUL2 As TextBox
    Friend WithEvents Label55 As Label
    Friend WithEvents TBTestHashUL1 As TextBox
    Friend WithEvents Label56 As Label
    Friend WithEvents TBTestHashUL3 As TextBox
    Friend WithEvents Label57 As Label
    Friend WithEvents TBTestKeyUL4 As TextBox
    Friend WithEvents Label53 As Label
    Friend WithEvents TBTestKeyUL2 As TextBox
    Friend WithEvents Label52 As Label
    Friend WithEvents TBTestKeyUL1 As TextBox
    Friend WithEvents Label51 As Label
    Friend WithEvents TBTestKeyUL3 As TextBox
    Friend WithEvents Label50 As Label
    Friend WithEvents BtTestHash As Button
    Friend WithEvents TBTestHashULHEX As TextBox
    Friend WithEvents Label49 As Label
    Friend WithEvents TBTestKeyHEX As TextBox
    Friend WithEvents Label48 As Label
    Friend WithEvents TBTestKeyString As TextBox
    Friend WithEvents Label47 As Label
    Friend WithEvents TBTestHashHEX As TextBox
    Friend WithEvents Label58 As Label
    Friend WithEvents RTBTestDebug As RichTextBox
    Friend WithEvents TPgBitcoin As TabPage
    Friend WithEvents RTBTestBitcoin As RichTextBox
    Friend WithEvents BtTestBitcoinGetInfo As Button
    Friend WithEvents SplitContainer15 As SplitContainer
    Friend WithEvents BtTestBitcoinGetUTXOSet As Button
    Friend WithEvents Label59 As Label
    Friend WithEvents TBTestBitcoinAddress As TextBox
    Friend WithEvents BtTestBitcoinGetWalletInfo As Button
    Friend WithEvents GroupBox10 As GroupBox
    Friend WithEvents BtTestBitcoinImportDescriptor As Button
    Friend WithEvents GroupBox9 As GroupBox
    Friend WithEvents BtTestBitcoinWalletName As Button
    Friend WithEvents Label60 As Label
    Friend WithEvents TBTestBitcoinWalletName As TextBox
    Friend WithEvents GroupBox8 As GroupBox
    Friend WithEvents BtTestBitcoionLoadWallet As Button
    Friend WithEvents BtTestBitcoinGetBalances As Button
    Friend WithEvents BtTestBitcoinGetBalance As Button
    Friend WithEvents BtTestBitcoinGetDescriptorInfo As Button
    Friend WithEvents GroupBox11 As GroupBox
    Friend WithEvents BtTestBitcoinGetRawTransaction As Button
    Friend WithEvents Label61 As Label
    Friend WithEvents TBTestBitcoinTransactionID As TextBox
    Friend WithEvents BtTestBitcoinGetTransaction As Button
    Friend WithEvents BtTestBitcoinListUnspent As Button
    Friend WithEvents BtTestBitcoinGetReceivedByAddress As Button
    Friend WithEvents GroupBox12 As GroupBox
    Friend WithEvents BtTestBitcoinScanBlocks As Button
    Friend WithEvents Label62 As Label
    Friend WithEvents LiBoDEXNETPublicKeys As ListBox
    Friend WithEvents Label63 As Label
    Friend WithEvents TPgSignumTX As TabPage
    Friend WithEvents LVTestSigTX As ListView
    Friend WithEvents ColumnHeader16 As ColumnHeader
    Friend WithEvents ColumnHeader17 As ColumnHeader
    Friend WithEvents BtTestSigTX_ReadTX As Button
    Friend WithEvents TBTestSigTX_PassPhrase As TextBox
    Friend WithEvents Label65 As Label
    Friend WithEvents TBTestSigTX_TXID As TextBox
    Friend WithEvents Label64 As Label
    Friend WithEvents BtTestSigTX_Decrypt As Button
    Friend WithEvents RTBTestSigTX As RichTextBox
    Friend WithEvents Label66 As Label
End Class
