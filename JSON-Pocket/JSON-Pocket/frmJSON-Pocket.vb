' donation address: BURST-RYF4-2EN3-GD96-4SE92

Imports Newtonsoft.Json
Imports System.IO
Imports System.Net
Imports System.Text

Public Class frmJSONPocket

    Delegate Sub MultiDelegate(ByVal params As Object)

    Dim nodelist As List(Of node) = New List(Of node)
    Dim poollist As List(Of node) = New List(Of node)

    Dim ActiveGlobalNode As node = Nothing
    Dim ActiveGlobalPassPhrase As String = ""

    Dim jsonNodes As String = ""
    Dim jsonPools As String = ""

    Dim passphrase As String = ""
    Dim AvgFee As Double = 0.0

    Dim BGTT As Threading.Thread

    Structure node
        Dim type As String
        Dim url As String
        Dim https As String
        Dim state As String
        Dim version As String
        Dim peers As String
        Dim address As String
        Dim numaddress As String
        Dim name As String
    End Structure

    Sub writeFile(ByVal str As String, Optional ByVal filename As String = "info.log", Optional dir As String = "BaseDir")

        Dim path As String = dir + "\" + filename

        If dir = "BaseDir" Then
            path = startuppath + filename
        End If

        Using sw As StreamWriter = File.CreateText(path)
            sw.Write(str)
        End Using

        Using sr As StreamReader = File.OpenText(path)
            Do While sr.Peek() >= 0
                Console.WriteLine(sr.ReadLine())
            Loop
        End Using

    End Sub

    Function readFile(ByVal filename As String, Optional dir As String = "BaseDir") As String

        If dir = "BaseDir" Then
            Return File.ReadAllText(startuppath + filename)
        End If

        Return File.ReadAllText(dir + "\" + filename)
    End Function

    Sub loadLang()

        'Main
        FileTSMI.Text = getLang(iniPropHomeStrip.File) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.File.ToString)
        refreshpeers.Text = getLang(iniPropHomeStrip.refreshpeers) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.refreshpeers.ToString)
        unlock.Text = getLang(iniPropHomeStrip.unlock) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.unlock.ToString)
        lock.Text = getLang(iniPropHomeStrip.lock) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.lock.ToString)
        exitprog.Text = getLang(iniPropHomeStrip.exitprog) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.exitprog.ToString)

        SettingsTSMI.Text = getLang(iniPropHomeStrip.SettingsTSMI) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.SettingsTSMI.ToString)

        PeerTSMI.Text = getLang(iniPropHomeStrip.PeerTSMI) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.PeerTSMI.ToString)

        refreshpeer.Text = getLang(iniPropHomeStrip.refreshpeer) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.refreshpeer.ToString)
        autoconnect.Text = getLang(iniPropHomeStrip.autoconnect) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.autoconnect.ToString)

        AddressTSMI.Text = getLang(iniPropHomeStrip.addressTSMI) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.addressTSMI.ToString)
        refreshaddressinfo.Text = getLang(iniPropHomeStrip.refreshaddressinfo) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.refreshaddressinfo.ToString)


        TabOverview.Text = getLang(iniPropTabControl.TabOverview) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TabControl.ToString, iniPropTabControl.TabOverview.ToString)
        TabSend.Text = getLang(iniPropTabControl.TabSend) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TabControl.ToString, iniPropTabControl.TabSend.ToString)
        TabTransacts.Text = getLang(iniPropTabControl.TabTransacts) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TabControl.ToString, iniPropTabControl.TabTransacts.ToString)


        GrpBxAccInfo.Text = getLang(iniPropGrpBxAccInfo.Title) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.Title.ToString)
        LabNewAcc.Text = getLang(iniPropGrpBxAccInfo.LabNewAcc) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.LabNewAcc.ToString)
        BtAccVerify.Text = getLang(iniPropGrpBxAccInfo.BtAccVerify) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.BtAccVerify.ToString)
        LabName.Text = getLang(iniPropGrpBxAccInfo.LabName) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.LabName.ToString)
        BtSetName.Text = getLang(iniPropGrpBxAccInfo.BtSetName) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.BtSetName.ToString)
        LabNumAcc.Text = getLang(iniPropGrpBxAccInfo.LabNumAcc) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.LabNumAcc.ToString)
        LabPubKey.Text = getLang(iniPropGrpBxAccInfo.LabPubKey) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.LabPubKey.ToString)


        GrpBxMiningInfo.Text = getLang(iniPropGrpBxMiningInfo.Title) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxMiningInfo.ToString, iniPropGrpBxMiningInfo.Title.ToString)
        LabGenBlocks.Text = getLang(iniPropGrpBxMiningInfo.LabGenBlocks) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxMiningInfo.ToString, iniPropGrpBxMiningInfo.LabGenBlocks.ToString)
        LabRewAss.Text = getLang(iniPropGrpBxMiningInfo.LabRewAss) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxMiningInfo.ToString, iniPropGrpBxMiningInfo.LabRewAss.ToString)
        BtSetRewAss.Text = getLang(iniPropGrpBxMiningInfo.BtSetRewAss) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxMiningInfo.ToString, iniPropGrpBxMiningInfo.BtSetRewAss.ToString)


        GrpBxBalancesInfo.Text = getLang(iniPropGrpBxBalancesInfo.Title) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxBalancesInfo.ToString, iniPropGrpBxBalancesInfo.Title.ToString)
        LConBal.Text = getLang(iniPropGrpBxBalancesInfo.LConBal) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxBalancesInfo.ToString, iniPropGrpBxBalancesInfo.LConBal.ToString)
        LUnBal.Text = getLang(iniPropGrpBxBalancesInfo.LUnBal) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxBalancesInfo.ToString, iniPropGrpBxBalancesInfo.LUnBal.ToString)
        LGenBal.Text = getLang(iniPropGrpBxBalancesInfo.LGenBal) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxBalancesInfo.ToString, iniPropGrpBxBalancesInfo.LGenBal.ToString)
        LSumBal.Text = getLang(iniPropGrpBxBalancesInfo.LSumBal) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxBalancesInfo.ToString, iniPropGrpBxBalancesInfo.LSumBal.ToString)


        WalletVersion.Text = getLang(iniPropStatusStrip.WalletVersion) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.WalletVersion.ToString)
        WalletPeers.Text = getLang(iniPropStatusStrip.WalletPeers) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.WalletPeers.ToString)
        'TSSLabOn.Text = INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, "TSSLabOn")
        TSSLabOn.Text = getLang(iniPropStatusStrip.TSSLabOff) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.TSSLabOff.ToString)
        'TSSLabSec.Text = INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, "TSSLabSec")
        TSSLabSec.Text = getLang(iniPropStatusStrip.TSSLabUnSec) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.TSSLabUnSec.ToString)
        WLock.Text = getLang(iniPropStatusStrip.WLock) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.WLock.ToString)
        'WLock.Text = INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, "WUnLock")


        BlockID.Text = getLang(iniPropTransColumns.BlockID) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.BlockID.ToString)
        TransID.Text = getLang(iniPropTransColumns.TransID) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.TransID.ToString)
        Confirms.Text = getLang(iniPropTransColumns.Confirms) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.Confirms.ToString)
        Sender.Text = getLang(iniPropTransColumns.Sender) ' INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.Sender.ToString)
        Recipient.Text = getLang(iniPropTransColumns.Recipient) ' INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.Recipient.ToString)
        Type.Text = getLang(iniPropTransColumns.Type) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.Type.ToString)
        Amount.Text = getLang(iniPropTransColumns.Amount) ' INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.Amount.ToString)
        Fee.Text = getLang(iniPropTransColumns.Fee) ' INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.Fee.ToString)


        GrpBxSendTo.Text = getLang(iniPropGrpBxSendTo.Title)
        LabRecipient.Text = getLang(iniPropGrpBxSendTo.LabRecipient)
        LabAmount.Text = getLang(iniPropGrpBxSendTo.LabAmount)
        BtSend.Text = getLang(iniPropGrpBxSendTo.BtSend)


        GrpBxUnConfTrans.Text = getLang(iniPropGrpBxUnConfTrans.Title)
        LabFee.Text = getLang(iniPropGrpBxUnConfTrans.LabFee)
        LUnConfTrans.Text = getLang(iniPropGrpBxUnConfTrans.LUnConfTrans)
        LUnConTransBytes.Text = getLang(iniPropGrpBxUnConfTrans.LUnConTransBytes)
        LAvgFee.Text = getLang(iniPropGrpBxUnConfTrans.LAvgFee)
        LBigFee.Text = getLang(iniPropGrpBxUnConfTrans.LBigFee)
        LSumFee.Text = getLang(iniPropGrpBxUnConfTrans.LSumFee)


        BtGetFeeInfo.Text = getLang(iniPropGrpBxUnConfTrans.BtGetFeeInfo)

    End Sub

    Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        BGTT = New Threading.Thread(AddressOf BGTimerThread)
        BGTT.Start()

        TabControl1.Controls.Remove(TabSend)

        If Not File.Exists(startuppath + "config.ini") Then
            INISetValue("", "Connection", "NodeListURL", "http://burstcoin.cc:80/network/json;http://burst.btfg.space:81/network/json")
            INISetValue("", "Connection", "PoolListURL", "http://burstcoin.cc:80/pool/json;http://burst.btfg.space:81/pool/json")
            INISetValue("", "Connection", "Type", "Wallet")
            INISetValue("", "Connection", "Timeout", "10")
            INISetValue("", "Connection", "OKOnly", "True")
            INISetValue("", "GUI", "Language", "DE")
            INISetValue("", "GUI", "FormSizeX", Me.Size.Width.ToString)
            INISetValue("", "GUI", "FormSizeY", Me.Size.Height.ToString)
            INISetValue("", "GUI", "OVHoSplitter", SplitContainer1.SplitterDistance.ToString)
            INISetValue("", "GUI", "OVVeSplitter", SplitContainer2.SplitterDistance.ToString)
            INISetValue("", "GUI", "TSHoSplitter", SplitContainer3.SplitterDistance.ToString)
            INISetValue("", "GUI", "TSVeSplitter", SplitContainer4.SplitterDistance.ToString)
        Else

            Dim frmWidth As Integer = CInt(INIGetValue("", "GUI", "FormSizeX"))
            Dim frmHeight As Integer = CInt(INIGetValue("", "GUI", "FormSizeY"))

            Me.Size = New Size(frmWidth, frmHeight)

            SplitContainer1.SplitterDistance = INIGetValue("", "GUI", "OVHoSplitter")
            SplitContainer2.SplitterDistance = INIGetValue("", "GUI", "OVVeSplitter")
            SplitContainer3.SplitterDistance = INIGetValue("", "GUI", "TSHoSplitter")
            SplitContainer4.SplitterDistance = INIGetValue("", "GUI", "TSVeSplitter")

        End If

        If Not File.Exists(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini") Then
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.File.ToString, "Datei")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.refreshpeers.ToString, "Knotenlisten aktualisieren")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.unlock.ToString, "Brieftasche freischalten")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.lock.ToString, "Brieftasche sperren")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.exitprog.ToString, "Programm beenden")

            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.SettingsTSMI.ToString, "Einstellungen")

            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.PeerTSMI.ToString, "Knoten:")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.refreshpeer.ToString, "Verbindung aktualisieren")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.autoconnect.ToString, "automatisch mit einem Knoten in der Liste verbinden")

            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.addressTSMI.ToString, "Adresse:")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.HomeStrip.ToString, iniPropHomeStrip.refreshaddressinfo.ToString, "Informationen aktualisieren")

            'mainForm tabinfo
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TabControl.ToString, iniPropTabControl.TabOverview.ToString, "Übersicht")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TabControl.ToString, iniPropTabControl.TabSend.ToString, "Überweisen")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TabControl.ToString, iniPropTabControl.TabTransacts.ToString, "Transaktionen")

            'mainForm tabOverview accountinfo
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.Title.ToString, "Kontoinformationen")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.LabNewAcc.ToString, "Dies ist ein neues Konto!")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.BtAccVerify.ToString, "verifizieren")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.LabName.ToString, "Name")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.BtSetName.ToString, "(neu) setzen")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.LabNumAcc.ToString, "numerische Adresse")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxAccInfo.ToString, iniPropGrpBxAccInfo.LabPubKey.ToString, "öffentlicher Schlüssel")

            'mainForm tabOverview mininginfo
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxMiningInfo.ToString, iniPropGrpBxMiningInfo.Title.ToString, "Mininginformationen")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxMiningInfo.ToString, iniPropGrpBxMiningInfo.LabGenBlocks.ToString, "generierte Blöcke:")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxMiningInfo.ToString, iniPropGrpBxMiningInfo.LabRewAss.ToString, "zugewiesene Miningassistenz:")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxMiningInfo.ToString, iniPropGrpBxMiningInfo.BtSetRewAss.ToString, "(neu) setzen")

            'mainForm tabOverview balanceinfo
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxBalancesInfo.ToString, iniPropGrpBxBalancesInfo.Title.ToString, "Guthaben")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxBalancesInfo.ToString, iniPropGrpBxBalancesInfo.LConBal.ToString, "garantiertes Guthaben:")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxBalancesInfo.ToString, iniPropGrpBxBalancesInfo.LUnBal.ToString, "(noch) nicht bestätigtes Guthaben:")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxBalancesInfo.ToString, iniPropGrpBxBalancesInfo.LGenBal.ToString, "generiertes Guthaben:")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxBalancesInfo.ToString, iniPropGrpBxBalancesInfo.LSumBal.ToString, "Gesamtguthaben:")

            'mainForm statusstrip items
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.WalletVersion.ToString, "Knotenversion:")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.WalletPeers.ToString, "Verbindungen:")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.TSSLabOn.ToString, "Online!")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.TSSLabConn.ToString, "Verbinde...")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.TSSLabOff.ToString, "Offline!")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.TSSLabSec.ToString, "sichere Verbindung(https)")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.TSSLabUnSec.ToString, "unsichere Verbindung(http)")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.WLock.ToString, "Brieftasche gesperrt")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.StatusStrip.ToString, iniPropStatusStrip.WUnLock.ToString, "Brieftasche freigeschaltet")

            'mainForm tabTransacts listview columns
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.BlockID.ToString, "BlockID")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.TransID.ToString, "ID")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.Confirms.ToString, "Bestätigungen")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.Sender.ToString, "Sender")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.Recipient.ToString, "Empfänger")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.Type.ToString, "Art")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.Amount.ToString, "Menge")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.TransColumns.ToString, iniPropTransColumns.Fee.ToString, "Gebühren")

            'Listview contextmenu
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.LVContext.ToString, iniLVContext.Sender.ToString, "Sender kopieren")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.LVContext.ToString, iniLVContext.Recipient.ToString, "Empfänger kopieren")

            'main MsgBox
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.MsgBox.ToString, iniMsgBox.GenericError.ToString, "Es ist ein Fehler aufgetreten: ")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.MsgBox.ToString, iniMsgBox.PeerResponse.ToString, "Der Knoten antwortete mit folgender Meldung: ")

            'MsgBox connection errors
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.MainMsgBox.ToString, iniMainMsgBox.TimeoutNodeList.ToString, "Timeout beim Verbindungsaufbau zur Liste der Nodes!")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.MainMsgBox.ToString, iniMainMsgBox.TimeOutPoolList.ToString, "Timeout beim Verbindungsaufbau zur Liste der Pools!")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.MainMsgBox.ToString, iniMainMsgBox.GetTransError.ToString, "Anfrage über Transaktionen kann nicht verarbeitet werden!")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.MainMsgBox.ToString, iniMainMsgBox.ConnLost.ToString, "Verbindung zum Wallet verloren.")


            Dim VerifyInfoMsg As String = "Um Dieses neue Konto zu verifizieren, müssen Sie einfach eine ausgehende Transaktion erstellen;;"
            VerifyInfoMsg += "(z.b. einen Namen oder eine Poolassistenz setzen);;;;"
            VerifyInfoMsg += "Dies kostet in der Regel eine Transaktionsgebühr in Höhe von 1 BURST"

            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.MainMsgBox.ToString, iniMainMsgBox.VerifyInfo.ToString, VerifyInfoMsg)


            Dim URLInfoMsg As String = "Die URLs von der die Liste der Knoten geholt werden soll.;;"
            URLInfoMsg += "Es können mehrere URLs angegeben werden (semikolon(;) getrennt);;"
            URLInfoMsg += "Die Antwort der URL muss im JSON-Format vorliegen welches die folgenden Eigenschaften beinhaltet:;;;;"
            URLInfoMsg += """type"" für den Verbindungstyp(z.b. Wallet);;"
            URLInfoMsg += """https"" für die sichere Verbindung (Yes/No) "
            URLInfoMsg += """state"" für den Status der Verbindung (OK/STUCK/FORKED/null);;"
            URLInfoMsg += """url"" für die Internetadresse des Knotens;;"

            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.SettingsMsgBox.ToString, iniSettingsMsgBox.URLInfo.ToString, URLInfoMsg)


            Dim PoolURLInfoMsg As String = "Die URLs von der die Liste der Pools geholt werden soll.;;"
            PoolURLInfoMsg += "Es können mehrere URLs angegeben werden (semikolon(;) getrennt);;;;"
            PoolURLInfoMsg += "Die Antwort der URL muss im JSON-Format vorliegen welches die folgenden Eigenschaften beinhaltet:;;"
            PoolURLInfoMsg += """address"" für die BURST-Adresse des Pools;;"
            PoolURLInfoMsg += """url"" für die Internetadresse des Pools;;"

            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.SettingsMsgBox.ToString, iniSettingsMsgBox.PoolURLInfo.ToString, PoolURLInfoMsg)


            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.SettingsMsgBox.ToString, iniSettingsMsgBox.TypeInfo.ToString, "Der Typ, zu dem das Wallet verbunden werden soll (Wallet,Pool,Faucet oder ALL)")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.SettingsMsgBox.ToString, iniSettingsMsgBox.TimeoutInfo.ToString, "Die Max. Wartezeit in Millisekunden für eine Anfrage")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.SettingsMsgBox.ToString, iniSettingsMsgBox.SaveQuestion.ToString, "Sollen die aktuellen Einstellungen gespeichert werden?")



            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.Title.ToString, "Einstellungen")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.GrpBxConn.ToString, "Verbindungseinstellungen")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.LabURLPeers.ToString, "URL der KnotenpunktListe:")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.LabURLPools.ToString, "URL der PoolListe:")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.LabType.ToString, "Typ:")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.LabTimeout.ToString, "Timeout:")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.ChBxOKOnly.ToString, "nur zu Status: OK verbinden")



            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxSendTo.ToString, iniPropGrpBxSendTo.Title.ToString, "Überweisungsinformationen")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxSendTo.ToString, iniPropGrpBxSendTo.LabRecipient.ToString, "Empfänger")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxSendTo.ToString, iniPropGrpBxSendTo.LabAmount.ToString, "BURST-Menge")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxSendTo.ToString, iniPropGrpBxSendTo.BtSend.ToString, "überweisen")


            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxUnConfTrans.ToString, iniPropGrpBxUnConfTrans.Title.ToString, "Gebühreninformationen")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxUnConfTrans.ToString, iniPropGrpBxUnConfTrans.LabFee.ToString, "Gebühren")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxUnConfTrans.ToString, iniPropGrpBxUnConfTrans.LUnConfTrans.ToString, "unbestätigte Transaktionen: ")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxUnConfTrans.ToString, iniPropGrpBxUnConfTrans.LUnConTransBytes.ToString, "unbestätigte Gesamtbytes: ")


            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxUnConfTrans.ToString, iniPropGrpBxUnConfTrans.LAvgFee.ToString, "unbestätigte Durchschnittsgebühren: ")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxUnConfTrans.ToString, iniPropGrpBxUnConfTrans.LBigFee.ToString, "unbestätigte größte Gebühr: ")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxUnConfTrans.ToString, iniPropGrpBxUnConfTrans.LSumFee.ToString, "unbestätigte Gesamtgebühren: ")

            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.GrpBxUnConfTrans.ToString, iniPropGrpBxUnConfTrans.BtGetFeeInfo.ToString, "Gebühren übernehmen")



            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Payment.ToString, Payment.Ordinary_payment.ToString, "normale Zahlung")

            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Messaging.ToString, Messaging.Account_info.ToString, "Kontoinformationen")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Messaging.ToString, Messaging.Alias_assignment.ToString, "Alias-Nachricht")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Messaging.ToString, Messaging.Alias_buy.ToString, "Alias kauf")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Messaging.ToString, Messaging.Alias_sell.ToString, "Alias verkauf")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Messaging.ToString, Messaging.Arbitrary_message.ToString, "Beliebige Nachricht")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Messaging.ToString, Messaging.Hub_terminal_announcement.ToString, "HUB Ankündigung")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Messaging.ToString, Messaging.Poll_creation.ToString, "Umfragenerstellung")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Messaging.ToString, Messaging.Vote_casting.ToString, "Stimmenabgabe")

            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Colored_coins.ToString, Colored_coins.Ask_order_cancellation.ToString, "kündigung der Auftragsanfrage")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Colored_coins.ToString, Colored_coins.Ask_order_placement.ToString, "Auftragsanfrage")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Colored_coins.ToString, Colored_coins.Asset_issuance.ToString, "Vermögensausgabe")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Colored_coins.ToString, Colored_coins.Asset_transfer.ToString, "Vermögensübertragung")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Colored_coins.ToString, Colored_coins.Bid_order_cancellation.ToString, "Angebotskündigung")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Colored_coins.ToString, Colored_coins.Bid_order_placement.ToString, "Auftragsangebot")

            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Digital_goods.ToString, Digital_goods.Delisting.ToString, "Abkündigung")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Digital_goods.ToString, Digital_goods.Delivery.ToString, "Lieferung")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Digital_goods.ToString, Digital_goods.Feedback.ToString, "Rückmeldung")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Digital_goods.ToString, Digital_goods.Listing.ToString, "Auflistung")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Digital_goods.ToString, Digital_goods.Price_change.ToString, "Preisänderung")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Digital_goods.ToString, Digital_goods.Purchase.ToString, "Kauf")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Digital_goods.ToString, Digital_goods.Quantity_change.ToString, "Mengenänderung")
            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Digital_goods.ToString, Digital_goods.Refund.ToString, "Rückerstattung")

            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Account_Control.ToString, Account_Control.Effective_balance_leasing.ToString, "Effektive Guthabenverleihung")

            INISetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", Types.Pool_assignment.ToString, Pool_assignment.Pool_assignment.ToString, "Poolassistenzzuweisung")

        End If

        loadLang()

        Me.Show()

        Dim jsonNodes As String = ""
        If File.Exists(startuppath + "network.txt") Then
            jsonNodes = readFile("network.txt")
        End If

        Dim jsonPools As String = ""
        If File.Exists(startuppath + "pools.txt") Then
            jsonPools = readFile("pools.txt")
        End If

        LoadNodes(jsonNodes)
        LoadPools(jsonPools)

    End Sub

    Private Sub frmBCW_FormClosing(sender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        BGTT.Abort()

        INISetValue("", "GUI", "FormSizeX", Me.Size.Width.ToString)
        INISetValue("", "GUI", "FormSizeY", Me.Size.Height.ToString)
        INISetValue("", "GUI", "OVHoSplitter", SplitContainer1.SplitterDistance.ToString)
        INISetValue("", "GUI", "OVVeSplitter", SplitContainer2.SplitterDistance.ToString)
        INISetValue("", "GUI", "TSHoSplitter", SplitContainer3.SplitterDistance.ToString)
        INISetValue("", "GUI", "TSVeSplitter", SplitContainer4.SplitterDistance.ToString)

    End Sub

    Sub MultiInvoker(ByVal obj As Object, ByVal prop As Object, ByVal val As Object)

        Dim paramList As List(Of Object) = New List(Of Object)

        paramList.Add(obj)
        paramList.Add(prop)
        paramList.Add(val)

        Me.Invoke(New MultiDelegate(AddressOf invoker), paramList)

    End Sub

    Sub invoker(ByVal params As List(Of Object))
        SetPropertyValueByName(params.Item(0), params.Item(1), params.Item(2))
    End Sub

    Sub ping()

        'Dim txt = CType(Me.Invoke(Function() NodeURLs.SelectedItem), String)
        'Dim txt2 = CType(Me.Invoke(New GetVal(AddressOf GetVal2), NodeURLs.SelectedItem), String)

        MultiInvoker(TSSLabOn, "ForeColor", Color.Gray) 'TSSLabOn.ForeColor = Color.Gray
        MultiInvoker(TSSLabOn, "Text", getLang(iniPropStatusStrip.TSSLabConn)) 'TSSLabOn.Text = "Verbinde..."

        MultiInvoker(TSProgressBar, "Visible", True)

        Application.DoEvents()

        Dim TOut As Integer = Val(INIGetValue("", "Connection", "Timeout"))
        If TOut <= 0 Then
            TOut = 1
        End If

        Dim ms As Integer = (TOut * 1000)

        If ms = 0 Then
            ms = 1
        End If


        If CType(Me.Invoke(Function() NodeURLs.Text), String) <> "" Then

            Dim tempurl As String = ""
            For Each node In nodelist

                If node.url = CType(Me.Invoke(Function() NodeURLs.Text), String) Then 'NodeURLs.SelectedItem Then
                    ActiveGlobalNode = node
                    tempurl = node.url
                    Exit For
                End If

            Next

            If tempurl = "" Then
                Dim tempnode As node = New node

                tempnode.url = CType(Me.Invoke(Function() NodeURLs.Text), String)
                tempnode.https = "NO"
                tempnode.state = "OK"
                ActiveGlobalNode = tempnode

            End If

            Dim url As String = ActiveGlobalNode.url

            If url.Contains("http") Then
                url = url.Substring(url.IndexOf("://") + 3)
            End If

            If url.Contains(":") Then
                url = url.Remove(url.IndexOf(":"))
            End If


            If My.Computer.Network.Ping(url, ms) Then
                '/test?requestTag=INFO
                Dim erro As String = BurstRequest(ActiveGlobalNode.url, "requestType=getState", "version")
                If erro = "error" Then
                    MultiInvoker(TSSLabOn, "ForeColor", Color.Red)                              'TSSLabOn.ForeColor = Color.Red
                    MultiInvoker(TSSLabOn, "Text", getLang(iniPropStatusStrip.TSSLabOff))                                  'TSSLabOn.Text = "Offline!"
                    MultiInvoker(WalletVersion, "Text", getLang(iniPropStatusStrip.WalletVersion) + " 0")                     'WalletVersion.Text = "Version: 0"
                    MultiInvoker(WalletPeers, "Text", getLang(iniPropStatusStrip.WalletPeers) + " 0")                               'WalletPeers.Text = "Peers: 0"

                    MultiInvoker(TSProgressBar, "Visible", False)

                    MultiInvoker(TSSLabSec, "ForeColor", Color.Red)                             'TSSLabSec.ForeColor = Color.Red
                    MultiInvoker(TSSLabSec, "Text", getLang(iniPropStatusStrip.TSSLabUnSec))              'TSSLabSec.Text = "unsichere Verbindung(http)!"


                    'MultiInvoker(WalletPanel, "Visible", False)                                'WalletPanel.Enabled = False
                    MultiInvoker(AddressTSMI, "Enabled", False)                                 'AdresseTSMI.Enabled = False
                    MultiInvoker(TBaddress, "Enabled", False)                                     'Adresse.Enabled = False
                    MultiInvoker(unlock, "Enabled", False)
                Else

                    ActiveGlobalNode.version = erro
                    ActiveGlobalNode.peers = BurstRequest(ActiveGlobalNode.url, "requestType=getState", "numberOfPeers")

                    MultiInvoker(TSSLabOn, "ForeColor", Color.LimeGreen)                        'TSSLabOn.ForeColor = Color.LimeGreen
                    MultiInvoker(TSSLabOn, "Text", "Online!")                                   'TSSLabOn.Text = "Online!"
                    MultiInvoker(WalletVersion, "Text", getLang(iniPropStatusStrip.WalletVersion) + " " + ActiveGlobalNode.version) 'WalletVersion.Text = "Version: " + ActiveGlobalNode.version
                    MultiInvoker(WalletPeers, "Text", getLang(iniPropStatusStrip.WalletPeers) + " " + ActiveGlobalNode.peers)       'WalletPeers.Text = "Peers: " + ActiveGlobalNode.peers

                    MultiInvoker(TSProgressBar, "Visible", False)


                    'MultiInvoker(WalletPanel, "Visible", True)                                 'WalletPanel.Enabled = True
                    MultiInvoker(AddressTSMI, "Enabled", True)                                  'AdresseTSMI.Enabled = True
                    MultiInvoker(TBaddress, "Enabled", True)                                      'Adresse.Enabled = True
                    MultiInvoker(unlock, "Enabled", True)

                    If ActiveGlobalNode.https.ToLower = "yes" Then
                        MultiInvoker(TSSLabSec, "ForeColor", Color.LimeGreen)                   'TSSLabSec.ForeColor = Color.LimeGreen
                        MultiInvoker(TSSLabSec, "Text", getLang(iniPropStatusStrip.TSSLabSec))           'TSSLabSec.Text = "sichere Verbindung(https)!"
                    Else
                        MultiInvoker(TSSLabSec, "ForeColor", Color.Red)                         'TSSLabSec.ForeColor = Color.Red
                        MultiInvoker(TSSLabSec, "Text", getLang(iniPropStatusStrip.TSSLabUnSec))          'TSSLabSec.Text = "unsichere Verbindung(http)!"
                    End If

                End If

            Else

                Dim erro As String = BurstRequest(ActiveGlobalNode.url, "requestType=getState", "version")

                If erro <> "error" Then
                    ActiveGlobalNode.version = erro
                    ActiveGlobalNode.peers = BurstRequest(ActiveGlobalNode.url, "requestType=getState", "numberOfPeers")

                    MultiInvoker(TSSLabOn, "ForeColor", Color.LimeGreen)                        'TSSLabOn.ForeColor = Color.LimeGreen
                    MultiInvoker(TSSLabOn, "Text", getLang(iniPropStatusStrip.TSSLabOn))                                   'TSSLabOn.Text = "Online!"
                    MultiInvoker(WalletVersion, "Text", getLang(iniPropStatusStrip.WalletVersion) + ActiveGlobalNode.version) 'WalletVersion.Text = "Version: " + ActiveGlobalNode.version
                    MultiInvoker(WalletPeers, "Text", getLang(iniPropStatusStrip.WalletPeers) + ActiveGlobalNode.peers)       'WalletPeers.Text = "Peers: " + ActiveGlobalNode.peers

                    MultiInvoker(TSProgressBar, "Visible", False)

                    'MultiInvoker(WalletPanel, "Visible", True)                                 'WalletPanel.Enabled = True
                    MultiInvoker(AddressTSMI, "Enabled", True)                                  'AdresseTSMI.Enabled = True
                    MultiInvoker(TBaddress, "Enabled", True)                                      'Adresse.Enabled = True
                    MultiInvoker(unlock, "Enabled", True)


                    If ActiveGlobalNode.https.ToLower = "yes" Then
                        MultiInvoker(TSSLabSec, "ForeColor", Color.LimeGreen)                   'TSSLabSec.ForeColor = Color.LimeGreen
                        MultiInvoker(TSSLabSec, "Text", getLang(iniPropStatusStrip.TSSLabSec))           'TSSLabSec.Text = "sichere Verbindung(https)!"
                    Else
                        MultiInvoker(TSSLabSec, "ForeColor", Color.Red)                         'TSSLabSec.ForeColor = Color.Red
                        MultiInvoker(TSSLabSec, "Text", getLang(iniPropStatusStrip.TSSLabUnSec))          'TSSLabSec.Text = "unsichere Verbindung(http)!"
                    End If

                Else

                    MultiInvoker(TSSLabOn, "ForeColor", Color.Red)                                  'TSSLabOn.ForeColor = Color.Red
                    MultiInvoker(TSSLabOn, "Text", getLang(iniPropStatusStrip.TSSLabOff))                                      'TSSLabOn.Text = "Offline!"
                    MultiInvoker(WalletVersion, "Text", getLang(iniPropStatusStrip.WalletVersion) + " 0")                         'WalletVersion.Text = "Version: 0"
                    MultiInvoker(WalletPeers, "Text", getLang(iniPropStatusStrip.WalletPeers) + " 0")                                   'WalletPeers.Text = "Peers: 0"

                    MultiInvoker(TSProgressBar, "Visible", False)

                    'MultiInvoker(WalletPanel, "Visible", False)                                    'WalletPanel.Enabled = False
                    MultiInvoker(AddressTSMI, "Enabled", False)                                     'AdresseTSMI.Enabled = False
                    MultiInvoker(TBaddress, "Enabled", False)                                         'Adresse.Enabled = False
                    MultiInvoker(unlock, "Enabled", False)


                End If

            End If
        End If

    End Sub

    Sub NodeURLs_DropDownClosed(sender As System.Object, e As System.EventArgs) Handles NodeURLs.DropDownClosed

        WalletPanel.Visible = False
        AddressTSMI.Enabled = False
        TBaddress.Enabled = False
        unlock.Enabled = False

        If Not NodeURLs.Text.Trim = "" Then
            Dim PiTh As Threading.Thread = New Threading.Thread(AddressOf ping)
            PiTh.Start()
        End If

    End Sub

    Private Sub NodeURLs_KeyPress(sender As System.Object, e As System.Windows.Forms.KeyPressEventArgs) Handles NodeURLs.KeyPress
        If Asc(e.KeyChar) = 13 And NodeURLs.Text.Trim <> "" Then
            NodeURLs_DropDownClosed(Nothing, Nothing)
        End If
    End Sub

    Sub ShowAddressInfo_Click(ByVal adresse As String)

        If testAddress(adresse) Then

            MultiInvoker(WalletPanel, "Visible", True) 'WalletPanel.Visible = True

            Application.DoEvents()

            If Not IsNothing(ActiveGlobalNode.url) Then
                Try

                    Dim response As String = BurstRequest(ActiveGlobalNode.url, "requestType=getAccount&account=" + adresse)
                    Dim err As String = dejson(response, "errorDescription")

                    If IsNothing(err) Or err = "" Then

                        MultiInvoker(LabNewAcc, "Text", "") ' LabNewAcc.Text = ""
                        MultiInvoker(BtAccVerify, "Visible", False) 'BtAccVerify.Visible = False

                        Dim Address As String = dejson(response, "accountRS") ' BurstRequest(ActiveGlobalNode.url, "requestType=getAccount&account=" + Adresse.Text, "accountRS")

                        'Dim name As String = dejson(response, "name") ' BurstRequest(ActiveGlobalNode.url, "requestType=getAccount&account=" + Adresse.Text, "name")
                        MultiInvoker(TBName, "Text", dejson(response, "name").Trim) 'TBName.Text = dejson(response, "name").Trim

                        'Dim numAddress As String = BurstRequest(ActiveGlobalNode.url, "requestType=getAccount&account=" + Adresse.Text, "account")
                        MultiInvoker(TBNumAdd, "Text", dejson(response, "account").Trim) ' TBNumAdd.Text = dejson(response, "account").Trim

                        'Dim pubkey As String = BurstRequest(ActiveGlobalNode.url, "requestType=getAccount&account=" + Adresse.Text, "publicKey")
                        MultiInvoker(TBPubKey, "Text", dejson(response, "publicKey").Trim) 'TBPubKey.Text = dejson(response, "publicKey").Trim

                        Dim blocks As Linq.JObject = JsonConvert.DeserializeObject(BurstRequest(ActiveGlobalNode.url, "requestType=getAccountBlockIds&account=" + adresse, ""))
                        MultiInvoker(LabGenBlocks, "Text", getLang(iniPropGrpBxMiningInfo.LabGenBlocks)) 'TBMinedBlocks.Text = blocks.Item("blockIds").Count.ToString
                        MultiInvoker(TBMinedBlocks, "Text", blocks.Item("blockIds").Count.ToString) 'TBMinedBlocks.Text = blocks.Item("blockIds").Count.ToString

                        Dim rewardassignment As String = BurstRequest(ActiveGlobalNode.url, "requestType=getRewardRecipient&account=" + adresse, "rewardRecipient")

                        rewardassignment = BurstRequest(ActiveGlobalNode.url, "requestType=rsConvert&account=" + rewardassignment, "accountRS")
                        MultiInvoker(TBAssistedTo, "Text", rewardassignment) ' TBAssistedTo.Text = rewardassignment


                        'getting reward assignded pool/address
                        For Each pool In poollist
                            If pool.address.Trim = rewardassignment.Trim Then
                                MultiInvoker(CoBxRewAss, "SelectedItem", pool.name) 'CoBxRewAss.SelectedItem = pool.name
                                Exit For
                            ElseIf pool.address.Trim = "Solo-Miners" And rewardassignment.Trim = adresse.Trim Then
                                MultiInvoker(CoBxRewAss, "SelectedItem", pool.name)
                                Exit For
                            End If
                        Next


                        If rewardassignment = Address Then
                            'TODO: Solomining kram
                        Else
                            'TODO: Poolmining kram
                        End If


                        Dim Trans As Linq.JObject = JsonConvert.DeserializeObject(BurstRequest(ActiveGlobalNode.url, "requestType=getAccountTransactions&account=" + adresse, ""))

                        If IsNothing(Trans) Then
                            MsgBox(getLang(iniMainMsgBox.VerifyInfo))
                            Exit Sub
                        End If

                        Dim Transacts As Linq.JArray = Trans.Item("transactions")

                        MultiInvoker(LVTrans, "Items", {"Clear"}.ToList)

                        For i As Integer = 0 To Transacts.Count - 1
                            Dim TransO As Linq.JObject = Transacts.Item(i)

                            Dim senderRS As String = TransO.Item("senderRS").ToString
                            Dim recipientRS As String = TransO.Item("recipientRS")

                            Dim type As String = TransO.Item("type").ToString
                            type += "/" + TransO.Item("subtype").ToString

                            Dim gebuhren As String = CStr(CLng(TransO.Item("feeNQT").ToString) / 100000000)
                            Dim transID As String = TransO.Item("transaction").ToString

                            Dim bheight As String = TransO.Item("height").ToString
                            Dim bID As String = TransO.Item("block").ToString

                            Dim confs As String = TransO.Item("confirmations").ToString

                            Dim amountNQT As String = CStr(FormatNumber(CLng(TransO.Item("amountNQT").ToString) / 100000000, 8))

                            Dim lvi As ListViewItem = New ListViewItem

                            With lvi
                                .Text = bID
                                .SubItems.Add(transID)
                                .SubItems.Add(confs)

                                .SubItems.Add(senderRS)
                                .SubItems.Add(recipientRS)
                                .SubItems.Add(gettyp(type))
                                .SubItems.Add(amountNQT)
                                .SubItems.Add(gebuhren)
                                If confs < 3 Then
                                    .BackColor = Color.OrangeRed
                                ElseIf confs > 2 And confs < 5 Then
                                    .BackColor = Color.Yellow
                                ElseIf confs > 10 Then
                                    .BackColor = Color.LightGray
                                Else
                                    .BackColor = Color.LawnGreen
                                End If
                            End With

                            MultiInvoker(LVTrans, "Items", {"Add", lvi}.ToList) '

                            'MultiInvoker(LVTrans, "AutoResizeColumns", ColumnHeaderAutoResizeStyle.ColumnContent)
                            'MultiInvoker(LVTrans, "AutoResizeColumns", ColumnHeaderAutoResizeStyle.HeaderSize)

                            'LVTrans.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
                            'LVTrans.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
                            'LVTrans.Columns.RemoveAt(LVTrans.Columns.Count - 1)


                            'With LVTrans.Items.Add(bID)
                            '    .SubItems.Add(transID)
                            '    .SubItems.Add(confs)

                            '    .SubItems.Add(senderRS)
                            '    .SubItems.Add(recipientRS)
                            '    .SubItems.Add(type)
                            '    .SubItems.Add(amountNQT)
                            '    .SubItems.Add(gebuhren)
                            '    If confs < 3 Then
                            '        .BackColor = Color.OrangeRed
                            '    ElseIf confs > 2 And confs < 5 Then
                            '        .BackColor = Color.Yellow
                            '    ElseIf confs > 10 Then
                            '        .BackColor = Color.LightGray
                            '    Else
                            '        .BackColor = Color.LawnGreen
                            '    End If

                            'End With


                            '"senderPublicKey": "b260654fd3af59bbfc91413f768be7484efb9039a7bdd4f40d58bb3a08785c50",
                            '"signature": "b955ab748d9078c40ec1bc805b556f0677c5e128239a7af53c7f08520fe89309b9eb1f12f448a11ee12608673ade29011e8c2c9b94b75cbb931b810fd8e05f38",
                            '"feeNQT": "100000000",
                            '"type": 0,
                            '"confirmations": 62,
                            '"fullHash": "7282a6821efa1c16c2999d05e270dc2180dace1685d9f9b2473cc874a0494dfe",
                            '"version": 1,
                            '"ecBlockId": "17761180296053101036",
                            '"signatureHash": "352b4648a6e1ec88edacc2f971031284bc91c959004457e728270ad7e5b954d6",
                            '"senderRS": "BURST-3NV9-DV4C-G9YD-283AG",
                            '"subtype": 0,
                            '"amountNQT": "410301535",
                            '"sender": "513452545987892071",
                            '"recipientRS": "BURST-KFP2-89T6-DYED-36Y7X",
                            '"recipient": "2204446261418440352",
                            '"ecBlockHeight": 383505,
                            '"block": "7261143214842594453",
                            '"blockTimestamp": 92753117,
                            '"deadline": 60,
                            '"transaction": "1593423377130226290",
                            '"timestamp": 92752997,
                            '"height": 383520


                        Next

                        response = BurstRequest(ActiveGlobalNode.url, "requestType=getAccount&account=" + adresse)
                        Dim GarGut As String = dejson(response, "guaranteedBalanceNQT")
                        Dim UnGut As String = dejson(response, "unconfirmedBalanceNQT")
                        'Dim EffGut As String = dejson(response, "effectiveBalanceNXT") 
                        'Dim Gut As String = dejson(response, "balanceNQT") 
                        Dim GenGut As String = dejson(response, "forgedBalanceNQT")


                        If Not GarGut = "error" And Not UnGut = "error" And Not GenGut = "error" Then

                            GarGut = CStr(CDbl(GarGut) / 100000000)
                            UnGut = CStr((CDbl(UnGut) / 100000000) - CDbl(GarGut))
                            'EffGut = CStr(CDbl(EffGut) / 100000000)
                            'Gut = CStr(CDbl(Gut) / 100000000)
                            GenGut = CStr(CDbl(GenGut) / 100000000)

                            MultiInvoker(LConBal, "Text", getLang(iniPropGrpBxBalancesInfo.LConBal) + " " + GarGut + " BURST") ' LGarGut.Text = "garantiertes Guthaben: " + GarGut + " BURST"

                            MultiInvoker(LUnBal, "Text", getLang(iniPropGrpBxBalancesInfo.LUnBal) + " " + UnGut + " BURST") ' LUnGut.Text = "(noch) nicht bestätigtes Guthaben: " + UnGut + " BURST"
                            'LEffGut.Text = "effektives Guthaben: " + EffGut + " BURST"
                            'LGut.Text = "Guthaben: " + Gut + " BURST"
                            MultiInvoker(LGenBal, "Text", getLang(iniPropGrpBxBalancesInfo.LGenBal) + " " + GenGut + " BURST") ' LGenGut.Text = "generiertes Guthaben: " + GenGut + " BURST"
                            MultiInvoker(LSumBal, "Text", getLang(iniPropGrpBxBalancesInfo.LSumBal) + " " + CStr(CDbl(GarGut) + CDbl(UnGut)) + " BURST") ' LGesGut.Text = "Gesamtguthaben: " + CStr(CDbl(GarGut) + CDbl(UnGut)) + " BURST"
                        Else
                            MsgBox(getLang(iniMainMsgBox.ConnLost))
                        End If


                    Else

                        MultiInvoker(LabNewAcc, "Text", getLang(iniPropGrpBxAccInfo.LabNewAcc)) ' LabNewAcc.Text = "Dies ist ein neues Konto!"
                        MultiInvoker(BtAccVerify, "Visible", True) ' BtAccVerify.Visible = True


                        response = BurstRequest(ActiveGlobalNode.url, "requestType=getAccountId&secretPhrase=" + passphrase)
                        Dim Address As String = dejson(response, "accountRS")
                        Dim numAddress As String = dejson(response, "account")
                        Dim pubKey As String = dejson(response, "publicKey")


                        MultiInvoker(TBName, "Text", "") ' TBName.Text = ""
                        MultiInvoker(TBNumAdd, "Text", numAddress.Trim) ' TBNumAdd.Text = numAddress.Trim
                        MultiInvoker(TBPubKey, "Text", pubKey.Trim) ' TBPubKey.Text = pubKey.Trim
                        MultiInvoker(LabGenBlocks, "Text", getLang(iniPropGrpBxMiningInfo.LabGenBlocks)) ' TBMinedBlocks.Text = "0"
                        MultiInvoker(TBMinedBlocks, "Text", "0") ' TBMinedBlocks.Text = "0"

                        MultiInvoker(TBAssistedTo, "Text", Address.Trim) ' TBAssistedTo.Text = Address.Trim

                        MultiInvoker(LConBal, "Text", getLang(iniPropGrpBxBalancesInfo.LConBal)) ' LGarGut.Text = "garantiertes Guthaben: "

                        MultiInvoker(LUnBal, "Text", getLang(iniPropGrpBxBalancesInfo.LUnBal)) ' LUnGut.Text = "(noch) nicht bestätigtes Guthaben: "
                        MultiInvoker(LGenBal, "Text", getLang(iniPropGrpBxBalancesInfo.LGenBal)) ' LGenGut.Text = "generiertes Guthaben: "
                        MultiInvoker(LSumBal, "Text", getLang(iniPropGrpBxBalancesInfo.LSumBal)) ' LGesGut.Text = "Gesamtguthaben: "

                        MultiInvoker(LVTrans, "Items", {"Clear"}.ToList) ' LVTrans.Items.Clear()

                    End If

                Catch ex As Exception
                    MsgBox(getLang(iniMsgBox.GenericError) + vbCrLf + vbCrLf + ex.Message)
                End Try

            End If

        Else
            MultiInvoker(WalletPanel, "Visible", False) ' WalletPanel.Visible = False
        End If

    End Sub

    Sub clearALLControls(Optional ByVal txtOnly As String = "")

        If txtOnly = "" Then
            TBaddress.ReadOnly = False
            LabNewAcc.Text = ""
            BtAccVerify.Visible = False

            unlock.Visible = True
            lock.Visible = False
        End If


        TBName.Text = ""
        TBNumAdd.Text = ""
        TBPubKey.Text = ""
        LabGenBlocks.Text = getLang(iniPropGrpBxMiningInfo.LabGenBlocks)
        TBMinedBlocks.Text = ""
        TBAssistedTo.Text = ""

        LConBal.Text = getLang(iniPropGrpBxBalancesInfo.LConBal)

        LUnBal.Text = getLang(iniPropGrpBxBalancesInfo.LUnBal)
        LGenBal.Text = getLang(iniPropGrpBxBalancesInfo.LGenBal)
        LSumBal.Text = getLang(iniPropGrpBxBalancesInfo.LSumBal)

        TBFee.Text = ""
        LUnConfTrans.Text = getLang(iniPropGrpBxUnConfTrans.LUnConfTrans)
        LUnConTransBytes.Text = getLang(iniPropGrpBxUnConfTrans.LUnConTransBytes)
        LAvgFee.Text = getLang(iniPropGrpBxUnConfTrans.LAvgFee)
        LBigFee.Text = getLang(iniPropGrpBxUnConfTrans.LBigFee)
        LSumFee.Text = getLang(iniPropGrpBxUnConfTrans.LSumFee)

        LVTrans.Items.Clear()

        Application.DoEvents()

    End Sub

    Private Sub refreshpeers_Click(sender As System.Object, e As System.EventArgs) Handles refreshpeers.Click

        TSSLabOn.ForeColor = Color.Red
        TSSLabOn.Text = getLang(iniPropStatusStrip.TSSLabOn, , 1)
        WalletVersion.Text = getLang(iniPropStatusStrip.WalletVersion) + " 0"
        WalletPeers.Text = getLang(iniPropStatusStrip.WalletPeers) + " 0"
        Application.DoEvents()

        Dim listofnodes As List(Of String) = INIGetValue("", "Connection", "NodeListURL")
        Dim listofpools As List(Of String) = INIGetValue("", "Connection", "PoolListURL")

        TSProgressBar.Style = ProgressBarStyle.Marquee

        For i As Integer = 0 To listofnodes.Count - 1

            Dim GetNodesThread As Threading.Thread = New Threading.Thread(AddressOf GetNodes)
            GetNodesThread.Start({"nodes", listofnodes.Item(i)}.ToList)

            While jsonNodes = "" Or GetNodesThread.IsAlive
                TSProgressBar.Style = ProgressBarStyle.Marquee
                TSProgressBar.Visible = True
                Application.DoEvents()
            End While

            If jsonNodes <> "" And jsonNodes <> "timeout" Then
                Exit For
            End If

        Next

        For i As Integer = 0 To listofpools.Count - 1

            Dim GetPoolsThread As Threading.Thread = New Threading.Thread(AddressOf GetNodes)
            GetPoolsThread.Start({"pools", listofpools.Item(i)}.ToList)

            While jsonNodes = "" Or jsonPools = "" Or GetPoolsThread.IsAlive
                TSProgressBar.Style = ProgressBarStyle.Marquee
                TSProgressBar.Visible = True
                Application.DoEvents()
            End While

            If jsonPools <> "" And jsonPools <> "timeout" Then
                Exit For
            End If

        Next

        TSProgressBar.Visible = False

        Dim ji As String = jsonNodes
        Dim pi As String = jsonPools


        If ji = "timeout" Then
            MsgBox(getLang(iniMainMsgBox.TimeoutNodeList))
            Me.Enabled = True
            'NodeURLs.SelectedIndex = 0
            WalletPanel.Visible = False

            If File.Exists(startuppath + "network.txt") Then
                ji = readFile("network.txt")
            Else
                Exit Sub
            End If

        End If

        If pi = "timeout" Then
            MsgBox(getLang(iniMainMsgBox.TimeOutPoolList))

            Me.Enabled = True
            'NodeURLs.SelectedIndex = 0
            WalletPanel.Visible = False

            If File.Exists(startuppath + "pools.txt") Then
                pi = readFile("pools.txt")
            Else
                Exit Sub
            End If

        End If


        If ji.Contains("callbackName") Then
            ji = ji.Substring(ji.IndexOf("(") + 1)
            ji = ji.Remove(ji.IndexOf(")"))
        End If

        If Not ji = "timeout" Then
            writeFile(ji, "network.txt")
        End If



        If pi.Contains("callbackName") Then
            pi = pi.Substring(pi.IndexOf("(") + 1)
            pi = pi.Remove(pi.IndexOf(")"))
        End If

        If Not pi = "timeout" Then
            writeFile(pi, "pools.txt")
        End If



        LoadNodes(ji)
        LoadPools(pi)


        jsonNodes = ""
        jsonPools = ""
    End Sub

    Sub GetNodes(ByVal paramlist As Object)
        Dim paralist As List(Of String) = New List(Of String)
        paralist = paramlist

        Dim target As String = paralist.Item(0)
        Dim fromURL As Object = paralist.Item(1)

        'MultiInvoker(TSProgressBar, "Minimum", 1)
        'MultiInvoker(TSProgressBar, "Maximum", 100)
        'MultiInvoker(TSProgressBar, "Value", 50)
        'MultiInvoker(TSProgressBar, "Style", ProgressBarStyle.Marquee)
        'MultiInvoker(TSProgressBar, "Visible", True)

        If fromURL.GetType = GetType(String) Then

            Try

                Dim ret As String = ""

                Dim inStrem As StreamReader
                Dim webReq As WebRequest
                Dim webRes As WebResponse

                webReq = WebRequest.Create(fromURL)

                Dim TOut As Integer = Val(INIGetValue("", "Connection", "Timeout"))
                If TOut <= 0 Then
                    TOut = 1
                End If

                webReq.Timeout = TOut * 1000

                webRes = webReq.GetResponse()

                inStrem = New StreamReader(webRes.GetResponseStream())

                ret = inStrem.ReadToEnd
                If target = "nodes" And ret.Trim <> "" Then
                    jsonNodes = ret
                    Exit Sub
                ElseIf target = "pools" And ret.Trim <> "" Then
                    jsonPools = ret
                    Exit Sub
                End If

            Catch ex As Exception
                If target = "nodes" Then
                    jsonNodes = "timeout"
                ElseIf target = "pools" Then
                    jsonPools = "timeout"
                End If
            End Try

        Else

            For Each url In fromURL

                Try

                    Dim ret As String = ""

                    Dim inStrem As StreamReader
                    Dim webReq As WebRequest
                    Dim webRes As WebResponse

                    webReq = WebRequest.Create(url.trim)
                    Dim TOut As Integer = Val(INIGetValue("", "Connection", "Timeout"))
                    If TOut <= 0 Then
                        TOut = 1
                    End If
                    webReq.Timeout = TOut * 1000

                    webRes = webReq.GetResponse()

                    inStrem = New StreamReader(webRes.GetResponseStream())

                    ret = inStrem.ReadToEnd

                    If target = "nodes" And ret.Trim <> "" Then
                        jsonNodes = ret
                        Exit Sub
                    ElseIf target = "pools" And ret.Trim <> "" Then
                        jsonPools = ret
                        Exit Sub
                    End If

                Catch ex As Exception
                    Continue For
                End Try
            Next

        End If

        If target = "nodes" Then
            jsonNodes = "timeout"
        ElseIf target = "pools" Then
            jsonPools = "timeout"
        End If

        'MultiInvoker(TSProgressBar, "Minimum", 1)
        'MultiInvoker(TSProgressBar, "Maximum", 100)
        'MultiInvoker(TSProgressBar, "Value", 50)
        'MultiInvoker(TSProgressBar, "Visible", False)

    End Sub

    Sub LoadNodes(ByVal jinput As String)

        nodelist.Clear()
        NodeURLs.Items.Clear()
        'NodeURLs.Items.Add("http://wallet.burstcoin.zone:8125")

        nodelist.AddRange({New node With {.https = "NO", .state = "OK", .type = "Wallet", .url = "http://localhost:8125"}}) ', New node With {.https = "NO", .state = "OK", .type = "Wallet", .url = "http://213.202.233.127:8125"}})

        Me.Enabled = False
        Dim jsoninput As String = jinput
        Dim dejson As Linq.JArray = JsonConvert.DeserializeObject(jsoninput)

        If IsNothing(dejson) Then

            Me.Enabled = True
            WalletPanel.Visible = False
            AddressTSMI.Enabled = False
            TBaddress.Enabled = False
            unlock.Enabled = False

            Exit Sub
        End If

        For i As Integer = 0 To dejson.Count - 1
            Dim typ As Linq.JObject = JsonConvert.DeserializeObject(dejson.Item(i).ToString)

            Dim nodeitem As node = New node


            nodeitem.type = typ.GetValue("type")
            If IsNothing(nodeitem.type) Then
                nodeitem.type = "N/A"
            End If

            nodeitem.https = typ.GetValue("https")
            If IsNothing(nodeitem.https) Then
                nodeitem.https = "N/A"
            End If

            nodeitem.state = typ.GetValue("state")
            If IsNothing(nodeitem.state) Then
                nodeitem.state = "N/A"
            End If

            nodeitem.url = typ.GetValue("url")
            If IsNothing(nodeitem.url) Then
                nodeitem.url = "N/A"
            End If


            If INIGetValue("", "Connection", "Type").ToLower = "all" Then
                If nodeitem.state.ToLower = "ok" Then
                    nodelist.Add(nodeitem)
                End If
            Else
                If nodeitem.type.ToLower = INIGetValue("", "Connection", "Type").ToLower And (nodeitem.state.ToLower = "ok" Or CBool(INIGetValue("", "Connection", "OKOnly")) = False) Then
                    nodelist.Add(nodeitem)
                End If
            End If

            'If nodeitem.type.ToLower = "pool" And nodeitem.state.ToLower = "ok" Then
            '    CoBxRewAss.Items.Add(nodeitem.url)
            'End If

            Application.DoEvents()

        Next


        If nodelist.Count > 0 Then
            NodeURLs.Visible = True
            For Each nod In nodelist
                NodeURLs.Items.Add(nod.url)
            Next

            NodeURLs.Width = 300
            NodeURLs.DropDownWidth = 300

            If NodeURLs.Text.Trim = "" Then
                NodeURLs.SelectedIndex = 0
            End If


        Else
            NodeURLs.Visible = False
        End If

        Me.Enabled = True
        WalletPanel.Visible = False
        AddressTSMI.Enabled = False
        TBaddress.Enabled = False
        unlock.Enabled = False
        'ping()

    End Sub

    Sub LoadPools(ByVal jinput As String)

        poollist.Clear()
        CoBxRewAss.Items.Clear()

        Dim dejson As Linq.JArray = JsonConvert.DeserializeObject(jinput)

        If IsNothing(dejson) Then

            Me.Enabled = True
            WalletPanel.Visible = False
            AddressTSMI.Enabled = False
            TBaddress.Enabled = False
            unlock.Enabled = False

            Exit Sub
        End If


        For i As Integer = 0 To dejson.Count - 1
            Dim typ As Linq.JObject = JsonConvert.DeserializeObject(dejson.Item(i).ToString)

            Dim poolitem As node = New node
            'TODO: poolitems einlesen

            '{
            '"accountId":"15674744673246368361",
            '"accountRS":"BURST-RNMB-9FJW-3BJW-F3Z3M",
            '"name":"Burst Mining Club Pool Wallet",
            '"description":null,
            '"balance":"39966",
            '"assignedMiners":1493,
            '"successfulMiners":335,
            '"foundBlocks":754,
            '"earnedAmount":"1269029"
            '}


            '{
            '"accountId":"9225891750247351890",
            '"accountRS":"BURST-8KLL-PBYV-6DBC-AM942",
            '"name":"CryptoGuru",
            '"description":null,
            '"balance":"41500",
            '"assignedMiners":35,
            '"successfulMiners":14,
            '"foundBlocks":42,
            '"earnedAmount":"70449"
            '}

            'Dim dejsonpools As Linq.JArray = JsonConvert.DeserializeObject(jsoninput)


            poolitem.address = typ.GetValue("accountRS")
            If IsNothing(poolitem.address) Then
                poolitem.address = "N/A"
            End If

            poolitem.numaddress = typ.GetValue("accountId")
            If IsNothing(poolitem.numaddress) Then
                poolitem.numaddress = "N/A"
            End If

            poolitem.name = typ.GetValue("name")
            If IsNothing(poolitem.name) Then
                poolitem.name = "N/A"
            End If

            poollist.Add(poolitem)
            CoBxRewAss.Items.Add(poolitem.name)

            'poolitem.url = typ.GetValue("url")
            'If IsNothing(poolitem.url) Then
            '    poolitem.url = "N/A"
            'End If


            'If INIGetValue("", "Connection", "Type").ToLower = "all" Then
            '    If poolitem.state.ToLower = "ok" Then
            '        poollist.Add(poolitem)
            '    End If
            'Else
            '    If poolitem.type.ToLower = INIGetValue("", "Connection", "Type").ToLower And (poolitem.state.ToLower = "ok" Or frmSettings.ChBxOKOnly.Checked = False) Then
            '        poollist.Add(poolitem)
            '    End If
            'End If

            'If nodeitem.type.ToLower = "pool" And nodeitem.state.ToLower = "ok" Then
            '    CoBxRewAss.Items.Add(nodeitem.url)
            'End If

            Application.DoEvents()


        Next

        CoBxRewAss.SelectedItem = CoBxRewAss.Items.Item(0)

    End Sub

    Private Sub AktualisierenToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles refreshaddressinfo.Click

        clearALLControls("txt")

        Dim getAddressInfoThread As Threading.Thread = New Threading.Thread(AddressOf ShowAddressInfo_Click)
        getAddressInfoThread.Start(TBaddress.Text.Trim)

        While Not getAddressInfoThread.ThreadState = Threading.ThreadState.Stopped
            TSProgressBar.Style = ProgressBarStyle.Marquee
            TSProgressBar.Visible = True
            Application.DoEvents()
        End While

        LVTrans.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
        LVTrans.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

        TSProgressBar.Visible = False

    End Sub

    Private Sub AktualisierenToolStripMenuItem1_Click(sender As System.Object, e As System.EventArgs) Handles refreshpeer.Click
        NodeURLs_DropDownClosed(Nothing, Nothing)
    End Sub

    Private Sub EinstellungenToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SettingsTSMI.Click
        frmSettings.ShowDialog()
    End Sub

    Private Sub AutomatischVerbindenToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles autoconnect.Click

        TSSLabOn.ForeColor = Color.Gray

        For Each x In NodeURLs.Items

            If Not TSSLabOn.ForeColor = Color.LimeGreen Then
                NodeURLs.SelectedItem = x
            Else
                Exit For
            End If

            Dim PiTh As Threading.Thread = New Threading.Thread(AddressOf ping)
            PiTh.Start()

            While Not PiTh.ThreadState = Threading.ThreadState.Stopped
                Application.DoEvents()
            End While

            While TSSLabOn.ForeColor = Color.Gray
                Application.DoEvents()
                Threading.Thread.Sleep(100)
            End While

        Next

    End Sub

    Private Sub WalletToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PeerTSMI.Click
        refreshpeer.Text = getLang(iniPropHomeStrip.refreshpeer) + " (" + NodeURLs.Text.Trim + ")"
    End Sub

    Private Sub TBPassPhrase_Enter(sender As System.Object, e As System.EventArgs) Handles TBPassPhrase.Enter, TBPassPhrase.Click

        TBPassPhrase.Text = ""
        passphrase = ""
        WLock.ForeColor = Color.Black
        WLock.Text = getLang(iniPropStatusStrip.WLock)
        TBaddress.Text = ""

    End Sub

    Private Sub TBPassPhrase_KeyPress(sender As System.Object, e As System.Windows.Forms.KeyPressEventArgs) Handles TBPassPhrase.KeyPress

        If Asc(e.KeyChar) = 8 Then
            Try
                passphrase = passphrase.Remove(passphrase.Length - 1)
            Catch ex As Exception

            End Try

        Else

            If Asc(e.KeyChar) = 13 Then

                If passphrase = "" Then
                    WLock.ForeColor = Color.Black
                    WLock.Text = getLang(iniPropStatusStrip.WLock)

                    TabControl1.Controls.Remove(TabSend)
                    'TabControl1.Controls.Remove(TabPage3)

                Else
                    WLock.ForeColor = Color.LimeGreen
                    WLock.Text = getLang(iniPropStatusStrip.WUnLock)

                    TabControl1.Controls.Add(TabSend)
                    'TabControl1.Controls.Add(TabPage3)

                End If

                Dim Address As String = BurstRequest(ActiveGlobalNode.url, "requestType=getAccountId&secretPhrase=" + passphrase, "accountRS")

                TBaddress.Text = Address

                FileTSMI.HideDropDown()

                clearALLControls()

                TabOverview.Focus()
                TabOverview.Refresh()
                unlock.Visible = False
                lock.Visible = True
                TBaddress.ReadOnly = True



                Dim getAddressInfoThread As Threading.Thread = New Threading.Thread(AddressOf ShowAddressInfo_Click)
                getAddressInfoThread.Start(TBaddress.Text.Trim)

                While Not getAddressInfoThread.ThreadState = Threading.ThreadState.Stopped
                    TSProgressBar.Style = ProgressBarStyle.Marquee
                    TSProgressBar.Visible = True
                    Application.DoEvents()
                End While

                LVTrans.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
                LVTrans.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)


                TSProgressBar.Visible = False

            Else
                passphrase += e.KeyChar
            End If

        End If

        TBPassPhrase.Text = Microsoft.VisualBasic.StrDup(passphrase.Length, "*")
        TBPassPhrase.SelectionStart = TBPassPhrase.Text.Length

        'AktualisierenToolStripMenuItem_Click(Nothing, Nothing)

        e.Handled = True

    End Sub

    Private Sub TBPassPhrase_TextChanged(sender As System.Object, e As System.EventArgs) Handles TBPassPhrase.TextChanged

        If passphrase = "" Then
            TBPassPhrase.ForeColor = Color.Gray
            TBPassPhrase.Text = "PassPhrase"
        Else
            TBPassPhrase.ForeColor = Color.Black
        End If

    End Sub

    Private Sub sperren_Click(sender As System.Object, e As System.EventArgs) Handles lock.Click
        passphrase = ""
        TBPassPhrase_KeyPress(Nothing, New KeyPressEventArgs(ChrW(Keys.Enter)))

        clearALLControls()
    End Sub

    Private Sub BeendenToolStripMenuItem1_Click(sender As System.Object, e As System.EventArgs) Handles exitprog.Click
        Me.Close()
    End Sub

    Private Sub Adresse_KeyPress(sender As System.Object, e As System.Windows.Forms.KeyPressEventArgs) Handles TBaddress.KeyPress

        If Asc(e.KeyChar) = 13 Then

            If TBaddress.ReadOnly = False Then

                clearALLControls()

                Dim getAddressInfoThread As Threading.Thread = New Threading.Thread(AddressOf ShowAddressInfo_Click)
                getAddressInfoThread.Start(TBaddress.Text.Trim)

                While Not getAddressInfoThread.ThreadState = Threading.ThreadState.Stopped
                    TSProgressBar.Style = ProgressBarStyle.Marquee
                    TSProgressBar.Visible = True
                    Application.DoEvents()
                End While
                TSProgressBar.Visible = False

            End If
        End If

    End Sub

    Private Sub BtAccVerify_Click(sender As System.Object, e As System.EventArgs) Handles BtAccVerify.Click

        Dim mboxres As MsgBoxResult = MsgBox(getLang(iniMainMsgBox.VerifyInfo), MsgBoxStyle.YesNo)

        If mboxres = MsgBoxResult.No Then
            Exit Sub
        End If

        Dim response = BurstRequest("http://Burstcoin.zone", "Get1Burst4=" + TBaddress.Text.Trim, , "/faucet.php")
        Dim answer As String = dejson(response, "answer")
        Dim status As String = dejson(response, "status")

        MsgBox(answer + " " + status)

    End Sub

    Private Sub CoBxRewAss_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles CoBxRewAss.SelectedIndexChanged

        Dim poolname As String = CoBxRewAss.SelectedItem

        For Each pool In poollist

            If pool.name.ToLower.Trim = poolname.ToLower.Trim Then

                If pool.name.ToLower.Trim = "solo-miners" Then
                    TBAssistedTo.Text = TBaddress.Text
                Else
                    TBAssistedTo.Text = pool.address.Trim
                End If

                Exit For
            End If

        Next

    End Sub

    Private Sub CoBxRewAss_KeyPress(sender As System.Object, e As System.Windows.Forms.KeyPressEventArgs) Handles CoBxRewAss.KeyPress

        ' If Asc(e.KeyChar) = 13 Then
        CoBxRewAss_SelectedIndexChanged(Nothing, Nothing)
        ' End If

    End Sub

    Private Sub BtGetFeeInfo_Click(sender As Object, e As EventArgs) Handles BtGetFeeInfo.Click

        If AvgFee > 0 And Not IsNothing(AvgFee) Then
            TBFee.Text = roundup(AvgFee).ToString
        Else
            TBFee.Text = "1"
        End If

    End Sub

    Private Sub LVTrans_MouseClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles LVTrans.MouseClick

        LVTrans.ContextMenu = Nothing

        If e.Button = Windows.Forms.MouseButtons.Right Then

            If LVTrans.SelectedItems.Count > 0 Then

                LVTrans.ContextMenu = Nothing

                Dim cntxt As ContextMenu = New ContextMenu

                Dim ctxitemsend As MenuItem = New MenuItem

                ctxitemsend.Text = getLang(iniLVContext.Sender)
                ctxitemsend.Name = "copysender"
                AddHandler ctxitemsend.Click, AddressOf ctx_clicks

                Dim ctxitemrec As MenuItem = New MenuItem
                ctxitemrec.Text = getLang(iniLVContext.Recipient)
                ctxitemrec.Name = "copyrec"
                AddHandler ctxitemrec.Click, AddressOf ctx_clicks

                cntxt.MenuItems.Add(ctxitemsend)
                cntxt.MenuItems.Add(ctxitemrec)

                LVTrans.ContextMenu = cntxt

            End If

        End If

    End Sub


    Private Sub ctx_clicks(sender As Object, e As EventArgs)

        Dim addres As String = LVColName2SubItem(LVTrans, getLang(iniPropTransColumns.Sender), LVTrans.SelectedItems.Item(0))

        Try

            My.Computer.Clipboard.Clear()
            If LVTrans.SelectedItems.Count > 0 Then

                If sender.name = "copysender" Then
                    My.Computer.Clipboard.SetText(addres)
                    MsgBox(addres)
                ElseIf sender.name = "copyrec" Then
                    My.Computer.Clipboard.SetText(addres)
                    MsgBox(addres)
                End If
            Else
                LVTrans.ContextMenu = Nothing
            End If

        Catch ex As Exception
            MsgBox(ex.Message + " (" + addres + ")")
        End Try

    End Sub

    Sub BGTimerThread()

        While BGTT.ThreadState = Threading.ThreadState.Running

            Dim bol As Boolean = CType(Me.Invoke(Function() SplitContainer3.Visible), Boolean)

            If bol And CType(Me.Invoke(Function() WalletPanel.Visible), Boolean) Then

                Dim jsoninput As String = BurstRequest(ActiveGlobalNode.url, "requestType=getUnconfirmedTransactions")

                If jsoninput = "error" Then
                    Continue While
                End If

                Dim dejsonn = JsonConvert.DeserializeObject(jsoninput)
                Dim dejsonarr = dejsonn.first
                Dim dejsonarr2 = dejsonarr.first

                MultiInvoker(LUnConfTrans, "Text", getLang(iniPropGrpBxUnConfTrans.LUnConfTrans) + " " + dejsonarr2.Count.ToString + " / 255")

                Dim UnConfBytes As Integer = 0

                Dim BigFee As Double = 0.0
                Dim SumFee As Double = 0.0

                Dim transcnt As Integer = 0

                'MultiInvoker(TSProgressBar, "Visible", True)
                'MultiInvoker(TSProgressBar, "Style", ProgressBarStyle.Continuous)
                'MultiInvoker(TSProgressBar, "Maximum", dejsonarr2.Count)


                MultiInvoker(LAvgFee, "Text", getLang(iniPropGrpBxUnConfTrans.LAvgFee) + " Loading...")
                MultiInvoker(LBigFee, "Text", getLang(iniPropGrpBxUnConfTrans.LBigFee) + " Loading...")
                MultiInvoker(LSumFee, "Text", getLang(iniPropGrpBxUnConfTrans.LSumFee) + " Loading...")

                MultiInvoker(LUnConTransBytes, "Text", getLang(iniPropGrpBxUnConfTrans.LUnConTransBytes) + " Loading...")


                For i As Integer = 0 To dejsonarr2.Count - 1

                    Dim x As Object = dejsonarr2.Item(i)

                    Dim transID As String = dejson(x.ToString, "transaction")

                    If dejsonarr2.count <= 20 Then
                        MultiInvoker(LUnConTransBytes, "Visible", True)
                        Dim resp As String = BurstRequest(ActiveGlobalNode.url, "requestType=getTransactionBytes&transaction=" + transID) '<<<<<<< slow as HELL!
                        Dim transBytes As String = dejson(resp, "transactionBytes")

                        UnConfBytes += transBytes.Length
                    Else
                        MultiInvoker(LUnConTransBytes, "Visible", False)
                    End If

                    Dim tempFee As Double = CDbl(dejson(x.ToString, "feeNQT"))

                    SumFee += tempFee

                    If tempFee > BigFee Then
                        BigFee = tempFee
                    End If

                    transcnt += 1

                    ' MultiInvoker(TSProgressBar, "Value", i)

                Next

                If SumFee > 0 And Not IsNothing(SumFee) Then

                    AvgFee = SumFee / transcnt
                    AvgFee /= 100000000
                    SumFee /= 100000000
                    BigFee /= 100000000

                    MultiInvoker(LAvgFee, "Text", getLang(iniPropGrpBxUnConfTrans.LAvgFee) + " " + Math.Round(CDbl(AvgFee), 8).ToString + " BURST")
                    MultiInvoker(LBigFee, "Text", getLang(iniPropGrpBxUnConfTrans.LBigFee) + " " + Math.Round(CDbl(BigFee), 8).ToString + " BURST")
                    MultiInvoker(LSumFee, "Text", getLang(iniPropGrpBxUnConfTrans.LSumFee) + " " + Math.Round(CDbl(SumFee), 8).ToString + " BURST")

                    MultiInvoker(LUnConTransBytes, "Text", getLang(iniPropGrpBxUnConfTrans.LUnConTransBytes) + " " + Math.Round(CDbl(UnConfBytes / 1024), 2).ToString + " kB / 44 kB")

                    'MultiInvoker(TBFee, "Text", roundup(AvgFee).ToString) ' TBFee.Text = roundup(AvgFee).ToString

                Else

                    MultiInvoker(LAvgFee, "Text", getLang(iniPropGrpBxUnConfTrans.LAvgFee) + " 0 BURST")
                    MultiInvoker(LBigFee, "Text", getLang(iniPropGrpBxUnConfTrans.LBigFee) + " 0 BURST")
                    MultiInvoker(LSumFee, "Text", getLang(iniPropGrpBxUnConfTrans.LSumFee) + " 0 BURST")

                    MultiInvoker(LUnConTransBytes, "Text", getLang(iniPropGrpBxUnConfTrans.LUnConTransBytes) + " 0 kB / 44 kB")

                    'MultiInvoker(TBFee, "Text", "1") ' TBFee.Text = "1"

                End If

                'MultiInvoker(TSProgressBar, "Style", ProgressBarStyle.Marquee)
                'MultiInvoker(TSProgressBar, "Value", 0)
                'MultiInvoker(TSProgressBar, "Maximum", 100)

                'MultiInvoker(TSProgressBar, "Visible", False)

                Dim TOut As Integer = Val(INIGetValue("", "Connection", "Timeout"))
                If TOut <= 0 Then
                    TOut = 1
                End If

                Threading.Thread.Sleep(TOut * 1000)

            Else

                Threading.Thread.Sleep(500)

            End If

        End While

    End Sub


    Private Sub BtSetName_Click(sender As System.Object, e As System.EventArgs) Handles BtSetName.Click

        Dim response = BurstRequest(ActiveGlobalNode.url, "requestType=getAccount&account=" + TBaddress.Text.Trim)
        Dim Address As String = dejson(response, "accountRS")

        If Not Address.Trim = "" And Not Address = "error" Then
            Dim numAddress As String = dejson(response, "account")
            'response = BurstRequest(ActiveGlobalNode.url, "requestType=getAccountId&secretPhrase=" + passphrase, "publicKey")
            Dim pubKey As String = BurstRequest(ActiveGlobalNode.url, "requestType=getAccountId&secretPhrase=" + passphrase, "publicKey") 'dejson(response, "publicKey")
            Dim Balancestr As String = dejson(response, "guaranteedBalanceNQT")

            Dim Balancedbl As Double = CDbl(Balancestr) / 100000000

            response = BurstRequest(ActiveGlobalNode.url, "requestType=setAccountInfo&name=" + TBName.Text.Trim + "&secretPhrase=" + passphrase + "&publicKey=" + pubKey + "&feeNQT=100000000&deadline=60")

            Dim err As String = dejson(response, "errorDescription")

            Dim broadcasted As String = ""

            If IsNothing(err) Or err = "" Then
                broadcasted = dejson(response, "broadcasted")
                If broadcasted.Trim.ToLower = "true" Then
                    MsgBox(getLang(iniMsgBox.PeerResponse) + vbCrLf + vbCrLf + "OK! Transaction: " + dejson(response, "transaction"))
                Else
                    MsgBox(getLang(iniMsgBox.PeerResponse) + vbCrLf + vbCrLf + "error!")
                End If

            Else
                MsgBox(getLang(iniMsgBox.PeerResponse) + vbCrLf + vbCrLf + err)
            End If



            '{
            '   "unsignedTransactionBytes":"0115777fa1053c00da6933d1cc45c6af02aef7e8218787f5868bf502c6d2f6a59ec15d659ccec5240000000000000000000000000000000000e1f50500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003ff505002620d17e587e0a890104746573740000",
            '   "transactionJSON":
            '       {
            '           "senderPublicKey":"da6933d1cc45c6af02aef7e8218787f5868bf502c6d2f6a59ec15d659ccec524",
            '           "feeNQT":"100000000",
            '           "type":1,
            '           "version":1,
            '           "ecBlockId":"9874844051513090086",
            '           "attachment":
            '               {
            '                   "name":"test",
            '                   "description":"",
            '                   "version.AccountInfo":1
            '               },
            '           "senderRS":"BURST-SFG8-96PE-7EBN-2NUK4",
            '           "subtype":5,
            '           "amountNQT":"0",
            '           "sender":"92134921892279750",
            '           "ecBlockHeight":390463,
            '           "deadline":60,
            '           "timestamp":94470007,
            '           "height":2147483647
            '       },
            '   "broadcasted":false,
            '   "requestProcessingTime":31
            '}



            '{
            '   "signatureHash":"f9394f8a99d35262c7f7b3b76687331b3dd698401ca15edb36090e15e156b435",
            '   "unsignedTransactionBytes":"01150dada2053c00da6933d1cc45c6af02aef7e8218787f5868bf502c6d2f6a59ec15d659ccec5240000000000000000000000000000000000e1f50500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007bf60500bdae3559c857688701067465737465720000",
            '   "transactionJSON":
            '       {
            '           "senderPublicKey":"da6933d1cc45c6af02aef7e8218787f5868bf502c6d2f6a59ec15d659ccec524",
            '           "signature":"c3f9c75c9bba3e0f91f3dd20bf5917287b83e45c0cf338588bb0cbab45c139043a5b9d285938a53857d41f9adc6683c114a3fa51fa8c3b0c7a8ebc3a24fb5133",
            '           "feeNQT":"100000000",
            '           "type":1,
            '           "fullHash":"576125bfedeb91eeee011c12e9b63d6f4a4f6bfb57a1b3de14866a9f0d777b8a",
            '           "version":1,
            '           "ecBlockId":"9757145110699945661",
            '           "signatureHash":"f9394f8a99d35262c7f7b3b76687331b3dd698401ca15edb36090e15e156b435",
            '           "attachment":
            '               {
            '                   "name":"tester",
            '                   "description":"",
            '                   "version.AccountInfo":1
            '               },
            '           "senderRS":"BURST-SFG8-96PE-7EBN-2NUK4",
            '           "subtype":5,
            '           "amountNQT":"0",
            '           "sender":"92134921892279750",
            '           "ecBlockHeight":390779,
            '           "deadline":60,
            '           "transaction":"17190780658996568407",
            '           "timestamp":94547213,
            '           "height":2147483647
            '       },
            '   "broadcasted":true,
            '   "requestProcessingTime":4044,
            '   "transactionBytes":"01150dada2053c00da6933d1cc45c6af02aef7e8218787f5868bf502c6d2f6a59ec15d659ccec5240000000000000000000000000000000000e1f505000000000000000000000000000000000000000000000000000000000000000000000000c3f9c75c9bba3e0f91f3dd20bf5917287b83e45c0cf338588bb0cbab45c139043a5b9d285938a53857d41f9adc6683c114a3fa51fa8c3b0c7a8ebc3a24fb5133000000007bf60500bdae3559c857688701067465737465720000",
            '   "fullHash":"576125bfedeb91eeee011c12e9b63d6f4a4f6bfb57a1b3de14866a9f0d777b8a",
            '   "transaction":"17190780658996568407"
            '}


        Else

            MsgBox("Dieses Konto existiert noch nicht im Burstcoin-Netzwerk, es wird fürs Mining empfohlen zunächst eine ""Poolassistenz"" durchzuführen!")

        End If

    End Sub

    Private Sub BtSetRewAss_Click(sender As System.Object, e As System.EventArgs) Handles BtSetRewAss.Click

        Dim response = BurstRequest(ActiveGlobalNode.url, "requestType=getAccount&account=" + TBaddress.Text.Trim)
        Dim Address As String = dejson(response, "accountRS")

        If Not Address.Trim = "" And Not Address = "error" Then

            Dim numAddress As String = dejson(response, "account")
            'response = BurstRequest(ActiveGlobalNode.url, "requestType=getAccountId&secretPhrase=" + passphrase, "publicKey")
            Dim pubKey As String = BurstRequest(ActiveGlobalNode.url, "requestType=getAccountId&secretPhrase=" + passphrase, "publicKey") 'dejson(response, "publicKey")
            Dim Balancestr As String = dejson(response, "guaranteedBalanceNQT")

            Dim Balancedbl As Double = CDbl(Balancestr) / 100000000

            response = BurstRequest(ActiveGlobalNode.url, "requestType=setRewardRecipient&recipient=" + TBAssistedTo.Text.Trim + "&secretPhrase=" + passphrase + "&publicKey=" + pubKey + "&feeNQT=100000000&deadline=60")

            Dim err As String = dejson(response, "errorDescription")

            Dim broadcasted As String = ""

            If IsNothing(err) Or err = "" Then
                broadcasted = dejson(response, "broadcasted")
                If broadcasted.Trim.ToLower = "true" Then
                    MsgBox(getLang(iniMsgBox.PeerResponse) + vbCrLf + vbCrLf + "OK! Transaction: " + dejson(response, "transaction"))
                Else
                    MsgBox(getLang(iniMsgBox.PeerResponse) + vbCrLf + vbCrLf + "error!")
                End If

            Else
                MsgBox(getLang(iniMsgBox.PeerResponse) + vbCrLf + vbCrLf + err)
            End If

        End If

    End Sub

    Private Sub BtSend_Click(sender As System.Object, e As System.EventArgs) Handles BtSend.Click

        Dim response = BurstRequest(ActiveGlobalNode.url, "requestType=getAccount&account=" + TBaddress.Text.Trim)
        Dim Address As String = dejson(response, "accountRS")

        If Not Address.Trim = "" And Not Address = "error" Then

            Dim numAddress As String = dejson(response, "account")
            'response = BurstRequest(ActiveGlobalNode.url, "requestType=getAccountId&secretPhrase=" + passphrase, "publicKey")
            Dim pubKey As String = BurstRequest(ActiveGlobalNode.url, "requestType=getAccountId&secretPhrase=" + passphrase, "publicKey") 'dejson(response, "publicKey")
            Dim Balancestr As String = dejson(response, "guaranteedBalanceNQT")

            Dim Balancedbl As Double = CDbl(Balancestr) / 100000000

            If TBAmount.Text.Trim = "" Then
                TBAmount.Text = "0"
            End If

            If TBFee.Text.Trim = "" Then
                TBFee.Text = "0"
            End If

            Dim amdbl As Double = CDbl(TBAmount.Text.Trim.Replace(".", ","))
            Dim feedbl As Double = CDbl(TBFee.Text.Trim.Replace(".", ","))

            Dim amount As ULong = Math.Round(amdbl, 8) * 100000000
            Dim fee As ULong = Math.Round(feedbl, 8) * 100000000


            response = BurstRequest(ActiveGlobalNode.url, "requestType=sendMoney&recipient=" + TBRecipient.Text.Trim + "&secretPhrase=" + passphrase + "&amountNQT=" + amount.ToString + "&feeNQT=" + fee.ToString + "&deadline=60")

            Dim err As String = dejson(response, "errorDescription")

            Dim broadcasted As String = ""

            If IsNothing(err) Or err = "" Then
                broadcasted = dejson(response, "broadcasted")
                If broadcasted = "true" Then
                    MsgBox(getLang(iniMsgBox.PeerResponse) + vbCrLf + vbCrLf + "OK! Transaction: " + dejson(response, "transaction"))
                Else
                    MsgBox(getLang(iniMsgBox.PeerResponse) + vbCrLf + vbCrLf + "error!")
                End If

            Else
                MsgBox(getLang(iniMsgBox.PeerResponse) + vbCrLf + vbCrLf + err)
            End If


        End If

    End Sub


    Private Sub TBAmount_KeyPress(sender As System.Object, e As System.Windows.Forms.KeyPressEventArgs) Handles TBFee.KeyPress, TBAmount.KeyPress

        Select Case Asc(e.KeyChar)
            Case 44, 46, 48 To 57, 8
                ' Komma, Punkt, Zahlen und Backspace zulassen
            Case Else
                ' alle anderen Eingaben unterdrücken
                e.Handled = True
        End Select

    End Sub
End Class
