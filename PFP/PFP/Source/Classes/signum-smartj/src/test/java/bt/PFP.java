package bt;
import bt.ui.EmulatorWindow;

/**
 * @author evolver
 */

public class PFP extends Contract {

    Address Initiator = null, Responder = null;
    long InitiatorsCollateral, RespondersCollateral, BuySellAmount, ForwardKey = 0L;
    boolean SellOrder = false, FirstTurn = true, ForwardKeyOK = false;

    //public static final long AUD = 4478273, BRL = 5001794, CAD = 4473155, CNY = 5852739, CZK = 4938307, DKK = 4934468, EUR = 5395781, HKD = 4475720, HUF = 4609352, INR = 5393993, ILS = 5459017, JPY = 5853258, MYR = 5396813, MXN = 5134413, TWD = 4478804, NZD = 4479566, NOK = 4935502, PHP = 5261392, PLN = 5131344, GBP = 5259847, RUB = 4347218, SGD = 4474707, SEK = 4932947, CHF = 4606019, THB = 4343892, USD = 4477781;
    //public static final long EUR = 5395781, USD = 4477781;

    public void CreateOrder(long XBuySellAmount) {

        if ((this.Initiator == null && !this.FirstTurn) || (getCurrentTxSender().equals(getCreator())) ) {

            FirstTurn = false;

            if (getCurrentTxAmount() > XBuySellAmount) {
                SellOrder = true;
                this.BuySellAmount = getCurrentTxAmount() - XBuySellAmount;//Sell: 100 = 130 - 30;
                this.InitiatorsCollateral = XBuySellAmount;//Sell: 30
            } else {
                SellOrder = false;
                this.InitiatorsCollateral = getCurrentTxAmount();//Buy: 30;
                this.BuySellAmount = XBuySellAmount;//Buy: 100
            }


            if (this.InitiatorsCollateral >= 0L) {
                this.Initiator = getCurrentTxSender();
                //this.XAmount = PXAmount;
                sendMessage(getCurrentTx().getMessage(), this.Initiator);
            } else {
				SendBack();
            }

        } else {
			SendBack();
        }

    }
    public void AcceptOrder() {

        if (this.Initiator != null && this.Responder == null) {

            this.Responder = getCurrentTxSender();

            if (this.Initiator.equals(this.Responder)){

                //FinishOrder();

                sendAmount(this.InitiatorsCollateral + this.BuySellAmount, this.Initiator);
                sendMessage("finished", getCurrentTxSender());
                this.Initiator = null;
                this.Responder = null;
                this.InitiatorsCollateral =  0L;
                this.BuySellAmount = 0L;

            } else {

                if(SellOrder){
                    this.RespondersCollateral = getCurrentTxAmount() ;//30
                } else {
                    this.RespondersCollateral = getCurrentTxAmount() - this.BuySellAmount ;//30 = 130 - 100
                }


                if((this.RespondersCollateral < this.InitiatorsCollateral) || (this.RespondersCollateral > this.InitiatorsCollateral * 2L)){ //Sell=30 0000 0000 < 30 0000 0000 = false; Buy=40 0000 0000 < 40 0000 0000 = false
                    this.Responder = null;
                    sendAmount(getCurrentTxAmount(), getCurrentTxSender());
                } else {
                    sendMessage("accepted", this.Responder);
                }
            }
        } else {
			SendBack();
        }

    }
    public void FinishOrder() {

        if (this.Initiator != null && this.Responder != null) {

            if(this.Initiator.equals(getCurrentTxSender()) || this.Responder.equals(getCurrentTxSender())) {

                long Temp =  (getCurrentBalance() - this.RespondersCollateral - this.InitiatorsCollateral - this.BuySellAmount - 50000000L) / 2L;

                if (this.Initiator.equals(getCurrentTxSender())){
                    sendAmount(this.InitiatorsCollateral, this.Initiator);
                    sendAmount(this.RespondersCollateral + this.BuySellAmount, this.Responder);
                } else {
                    sendAmount(this.RespondersCollateral, this.Responder);
                    sendAmount(this.InitiatorsCollateral + this.BuySellAmount, this.Initiator);
                }

                sendMessage("finished", getCurrentTxSender());
                sendAmount(Temp, this.Responder);
                sendAmount(Temp, this.Initiator);

                this.Initiator = null;
                this.Responder = null;
                this.InitiatorsCollateral =  0L;
                this.RespondersCollateral = 0L;
                this.BuySellAmount = 0L;
                this.ForwardKeyOK = false;
                this.ForwardKey = 0L;

            } else {
				SendBack();
            }

        } else {
			SendBack();
        }
    }
    public void FinishOrder2(long firstLong, long secondLong){

        if (this.Initiator != null && this.Responder != null) {

            long T_FWDKey = performSHA256_64(firstLong, secondLong);
            if(T_FWDKey == this.ForwardKey && this.ForwardKeyOK){

                long Temp =  (getCurrentBalance() - this.RespondersCollateral - this.InitiatorsCollateral - this.BuySellAmount - 50000000L) / 2L;
                sendAmount(this.InitiatorsCollateral, this.Initiator);
                sendAmount(this.RespondersCollateral + this.BuySellAmount, this.Responder);

                sendMessage("finished", this.Responder);
                sendAmount(Temp, this.Responder);
                sendAmount(Temp, this.Initiator);

                this.Initiator = null;
                this.Responder = null;
                this.InitiatorsCollateral =  0L;
                this.RespondersCollateral = 0L;
                this.BuySellAmount = 0L;
                this.ForwardKeyOK = false;
                this.ForwardKey = 0L;

            }

        } else {
			SendBack();
        }
    }
    public void InjectResponder(long Recipient){

        Address Resp = getAddress(Recipient);

        if(this.Initiator.equals(getCurrentTxSender()) && this.SellOrder && this.Responder == null){
            this.Responder = Resp;
            sendMessage("accepted", this.Responder);
        }
    }
    public void InjectForwardKey(long FwdKey){
		if (this.Initiator != null && this.Responder != null) {
			if(this.ForwardKey == 0L && (this.Initiator.equals(getCurrentTxSender()) || this.Responder.equals(getCurrentTxSender()))){
				this.ForwardKey = FwdKey;
			} else {
				SendBack();
			}
		} else {
			SendBack();
		}
    }
    public void SetForwardKeyOK(long OK){
        if(this.Initiator.equals(getCurrentTxSender()) && this.ForwardKey != 0L && !this.ForwardKeyOK && this.Responder != null) {
			if (OK == 1L) {
				this.ForwardKeyOK = true;
				sendMessage("accepted", this.Responder);
			} else {
				this.ForwardKeyOK = false;
				this.ForwardKey = 0L;
			}
        } else {
			SendBack();
        }
    }

    private void SendBack(){
		sendAmount(getCurrentTxAmount(), getCurrentTxSender());
	}

    @Override
    public void txReceived() {

    }

    /**
     * The main function is for debugging purposes only it will not be
     * compiled to AT bytecode.
     */
    public static void main(String[] args) throws Exception {

        BT.activateCIP20(true);

        // some initialization code to make things easier to debug
        Emulator emu = Emulator.getInstance();

        Address seller = Emulator.getInstance().getAddress("SELLER");
        emu.airDrop(seller, 2000 * Contract.ONE_BURST);
        Address third_one = Emulator.getInstance().getAddress("THIRD");
        emu.airDrop(third_one, 1000 * Contract.ONE_BURST);
        Address buyer = Emulator.getInstance().getAddress("BUYER");
        emu.airDrop(buyer, 2000 * Contract.ONE_BURST);

        Address auction = Emulator.getInstance().getAddress("BLP_ACC");
        emu.createConctract(seller, auction, PFP.class, 0);

        emu.forgeBlock();

        new EmulatorWindow(PFP.class);
    }
}
