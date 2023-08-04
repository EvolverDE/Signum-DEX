/**
 * @author evolver
 * Translated to SmartC by Deleterium
 */

#include APIFunctions
#pragma maxConstVars 2
#pragma version 1.0

long CreateOrderTX = 0, AcceptOrderTX = 0;
long Initiator = 0, Responder = 0;
long InitiatorsCollateral = 0, RespondersCollateral = 0, MediatorsDeposit = 0, BuySellAmount = 0, ConciliationAmount = 0, ChainSwapHashLong1 = 0, ChainSwapHashLong2 = 0, ChainSwapHashLong3 = 0, ChainSwapHashLong4 = 0, TimeOut = 0;
long SellOrder = false, FreeForAll = false, isFiatOrder = false, Deniability = 0, Dispute = false, Objection = false, Decimals = 8;

#define GAS_FEE 50000000
#define ONETHOUSAND 1000
#define TENTHOUSAND 10000
#define TWENTYFOURHOURS 1440
#define ONEHOUR 60

#define FIAT_AUD 0x0000000000415544
#define FIAT_BRL 0x000000000042524c
#define FIAT_CAD 0x0000000000434144
#define FIAT_CHF 0x0000000000434846
#define FIAT_CNY 0x0000000000434e59
#define FIAT_CZK 0x0000000000435a4b
#define FIAT_DKK 0x0000000000444b4b
#define FIAT_EUR 0x0000000000455552
#define FIAT_GBP 0x0000000000474250
#define FIAT_HKD 0x0000000000484b44
#define FIAT_HUF 0x0000000000485546
#define FIAT_ILS 0x0000000000494c53
#define FIAT_INR 0x0000000000494e52
#define FIAT_JPY 0x00000000004a5059
#define FIAT_MXN 0x00000000004d584e
#define FIAT_MYR 0x00000000004d5952
#define FIAT_NOK 0x00000000004e4f4b
#define FIAT_NZD 0x00000000004e5a44
#define FIAT_PHP 0x0000000000504850
#define FIAT_PLN 0x0000000000504c4e
#define FIAT_RUB 0x0000000000525542
#define FIAT_SEK 0x000000000053454b
#define FIAT_SGD 0x0000000000534744
#define FIAT_THB 0x0000000000544842
#define FIAT_TWD 0x0000000000545744
#define FIAT_USD 0x0000000000555344

//#define REFUEL_GAS 0xaaaa1111bbbb2222
#define ACTIVATE_DEACTIVATE_DISPUTE 0x805352d2a4817dd3
#define CREATE_ORDER 0x09f2535fcf54cc3b
#define CREATE_ORDER_WITH_RESPONDER 0xb5f321287b0a94fc
#define ACCEPT_ORDER 0x416d0b4b4963b686
#define INJECT_RESPONDER 0x7fdd5d44092b6afc
#define OPEN_DISPUTE 0x683bad5d504e7c61
#define MEDIATE_DISPUTE 0x0f79d4af6ccb95bf
#define APPEAL 0x65e17003908c81e1
#define CHECK_CLOSE_DISPUTE 0xb8a95dcf971ca1a1
#define FINISH_ORDER 0x2b6059b8fdd0d9eb
#define INJECT_CHAIN_SWAP_HASH 0x267440230a0c2f32
#define FINISH_ORDER_WITH_CHAIN_SWAP_KEY 0xc896b494b13b04a1

struct TXINFO {
    long txId,
        timestamp,
        sender,
        amount,
        message[8];
} currentTX;

B_To_Address_Of_Creator();
long CREATOR = Get_B1();

void checkFiat(void) {
	
	switch (currentTX.message[3]) {
	case FIAT_AUD: case FIAT_BRL: case FIAT_CAD: case FIAT_CNY: case FIAT_CZK: case FIAT_DKK: case FIAT_EUR: case FIAT_HKD: case FIAT_INR: case FIAT_ILS: case FIAT_MYR: case FIAT_MXN: case FIAT_NZD: case FIAT_NOK: case FIAT_PHP: case FIAT_PLN: case FIAT_GBP: case FIAT_RUB: case FIAT_SGD: case FIAT_SEK: case FIAT_CHF: case FIAT_THB: case FIAT_USD:
		isFiatOrder = true;
		Decimals = 2;
		break;
	case FIAT_HUF: case FIAT_JPY: case FIAT_TWD:
		isFiatOrder = true;
		Decimals = 0;
		break;
	default:
		isFiatOrder = false;
		Decimals = 8;
	}
}
long checkOneCent(long XAmount, long BuySellAmount) {
	
	// Exploittest: 92_233_720.00 EUR for 9_223_372_000.0000_0000 Signa = 0.01€/Signa
	if (XAmount < BuySellAmount && XAmount <= 9223372000000000) { // 9_0000_0000 < 100_0000_0000
		long T_Sum = XAmount * ONETHOUSAND / BuySellAmount; //9_0000_0000 * 1000 / 100_0000_0000 = 9 = 0.009 XItem
		//								       92_233_720_0000_0000 * 1000
		//								   92_233_720_368_5477_5807 MAX long
		if ((T_Sum >= 10 && Decimals == 2) || (T_Sum >= ONETHOUSAND && Decimals == 0)) {
			return true;
		}
		else if (XAmount >= BuySellAmount) {
			return false;
		}
		
	}
	else {
		return true;
	}

}

void main(void) {
    do {
        A_To_Tx_After_Timestamp(currentTX.timestamp);
        if (Get_A1() == 0) {
            break;
        }
        getTxDetails();
        switch (currentTX.message[0]) {
        case ACTIVATE_DEACTIVATE_DISPUTE:
            ActivateDeactivateDispute();
            break;
        case CREATE_ORDER:
            CreateOrder();
            break;
		case CREATE_ORDER_WITH_RESPONDER:
            CreateOrderWithResponder();
            break;
        case ACCEPT_ORDER:
            AcceptOrder();
            break;
        case INJECT_RESPONDER:
            InjectResponder();
            break;
        case OPEN_DISPUTE:
            OpenDispute();
            break;
        case MEDIATE_DISPUTE:
            MediateDispute();
            break;
        case APPEAL:
            Appeal();
            break;
        case CHECK_CLOSE_DISPUTE:
            CheckCloseDispute();
            break;
        case FINISH_ORDER:
            FinishOrder();
            break;
        case INJECT_CHAIN_SWAP_HASH:
            InjectChainSwapHash();
            break;
        case FINISH_ORDER_WITH_CHAIN_SWAP_KEY:
            FinishOrderWithChainSwapKey();
            break;
		//case REFUEL_GAS:
		//	break;
        }
    } while (true);
}
void getTxDetails(void) {
    currentTX.txId = Get_A1();
	currentTX.amount = getAmount(currentTX.txId);
	currentTX.timestamp = Get_Timestamp_For_Tx_In_A();
	currentTX.sender = getSender(currentTX.txId);
	readMessage(currentTX.txId, 0, currentTX.message);
    readMessage(currentTX.txId, 1, currentTX.message + 4);
	
}
void sendMessageSC(long recipient, long messageToSend1, long messageToSend2, long messageToSend3, long messageToSend4) {
    Set_B1(recipient);
    Set_A1_A2(messageToSend1, messageToSend2);
    Set_A3_A4(messageToSend3, messageToSend4);
    Send_A_To_Address_In_B();
}

void ActivateDeactivateDispute(void) {
    if (Initiator == 0 && currentTX.sender == CREATOR && FreeForAll) {
        Deniability = !Deniability;
    }
	else {
        sendBack();
    }
}

void CreateOrder(void) {
    long T_BuyCollateralAmount = currentTX.message[1];
	checkFiat();
	
    if (((currentTX.amount > (GAS_FEE - getGasFee()) && (T_BuyCollateralAmount > 0 || !isFiatOrder)) || (currentTX.amount <= (ONETHOUSAND * ONETHOUSAND * TENTHOUSAND) + GAS_FEE && T_BuyCollateralAmount == 0)) && Initiator == 0 && (FreeForAll || currentTX.sender == CREATOR)) {
        
        CreateOrderTX = currentTX.txId;
        if (currentTX.amount > T_BuyCollateralAmount) {
            SellOrder = true;
            BuySellAmount = currentTX.amount - T_BuyCollateralAmount; //Sell: 100 = 130 - 30;
            InitiatorsCollateral = T_BuyCollateralAmount; //Sell: 30
        }
		else {
            SellOrder = false;
            InitiatorsCollateral = currentTX.amount - getGasFee(); //Buy: 30;
            BuySellAmount = T_BuyCollateralAmount; //Buy: 100
        }
		
		FreeForAll = true;
		
		if ((isFiatOrder && checkOneCent(currentTX.message[2], BuySellAmount)) || !isFiatOrder) {
			
			if (InitiatorsCollateral >= 0) {
				Initiator = currentTX.sender;

				if (Initiator == CREATOR && Deniability == 1 || !isFiatOrder && Deniability == 1) {
					Deniability = 3;
				}
				sendMessageSC(Initiator, CreateOrderTX, 0, 0, 0);

			}
			else {
				sendBack();
			}
		}
		else {
			sendBack();
		}
    }
	else {
        sendBack();
    }
}

void CreateOrderWithResponder(void) {
	checkFiat();
    if ((currentTX.amount > GAS_FEE && Initiator == 0 && (FreeForAll || currentTX.sender == CREATOR) && currentTX.message[1] != 0) && ((isFiatOrder && checkOneCent(currentTX.message[2], currentTX.amount)) || !isFiatOrder)) {
		
		Initiator = currentTX.sender;
		Responder = currentTX.message[1];
		BuySellAmount = currentTX.amount;
		CreateOrderTX = currentTX.txId;
		AcceptOrderTX = currentTX.txId;
		SellOrder = true;
		FreeForAll = true;
		
		if (Deniability == 1) {
			Deniability = 3;
		}
		
		sendMessageSC(Initiator, CreateOrderTX, 0, 0, 0);
		
    }
	else {
        sendBack();
	}
}
void AcceptOrder(void) {
	
    if (Initiator != 0 && Responder == 0) {

        Responder = currentTX.sender;

        if (Initiator == Responder){

            sendAmount(Get_Current_Balance() - GAS_FEE, Initiator);
            sendMessageSC(Initiator, CreateOrderTX, currentTX.txId, currentTX.txId, 0);
            reset();

        } else {

			if(!isFiatOrder){
				setTimeOut(ONEHOUR);
			}

            if (SellOrder){
                RespondersCollateral = currentTX.amount ;//30
            } else {
                RespondersCollateral = currentTX.amount - BuySellAmount ;//30 = 130 - 100
            }

            if ((RespondersCollateral < InitiatorsCollateral) || (RespondersCollateral > InitiatorsCollateral * 2)){ //Sell=30 0000 0000 < 30 0000 0000 = false; Buy=43 5000 0000 < 43 5000 0000 = false
                Responder = 0;
				RespondersCollateral = 0;
                sendBack();
            } else {
                AcceptOrderTX = currentTX.txId;

				if (Responder == CREATOR && Deniability == 1) {
					Deniability = 3;
				}
            }
        }
		
	} else if(checkCandidates() && getTimeIsUp() && !isFiatOrder) {
		
		if (SellOrder){
			sendOut(BuySellAmount, 0);
		} else {
			sendOut(0, BuySellAmount);
		}
		
		sendMessageSC(Initiator, CreateOrderTX, currentTX.txId, currentTX.txId, Responder);//AcceptOrderTX = Responder
		reset();
		
	} else {
        sendBack();
    }
}

void InjectResponder(void) {
    if (Initiator == currentTX.sender && SellOrder && Responder == 0){
		
		if (Deniability == 1) {
			Deniability = 3;
		}
        
		Responder = currentTX.message[1];
		RespondersCollateral = 0;
        AcceptOrderTX = currentTX.txId;
    } else {
        sendBack();
    }
}

void OpenDispute(void) {
    if (checkCandidates() && checkIfSenderIsOneCandidate() && Deniability == 1 && isFiatOrder) {
        Dispute = true;
        //messageToSend[] = 0;
        //messageToSend[1] = currentTX.txId;
        sendMessageSC(CREATOR, 0, currentTX.txId, 0, 0);
    } else {
        sendBack();
    }
}

void MediateDispute(void) {
    long Percentage = currentTX.message[1];

    if ((currentTX.amount >= (getSumCollateral() / 2) + MediatorsDeposit) && checkCandidates() && currentTX.sender == CREATOR && Percentage >= 0 && Dispute) {
		MediatorsDeposit += currentTX.amount;
		
        if (Percentage >= TENTHOUSAND){
            Percentage = TENTHOUSAND;
        }

        ConciliationAmount = (BuySellAmount / TENTHOUSAND) * Percentage;
		setTimeOut(TWENTYFOURHOURS);

    } else {
        sendBack();
    }
}
void Appeal(void) {
    if (checkCandidates() && checkIfSenderIsOneCandidate() && !getTimeIsUp() && Dispute){
        Objection = true;
        CheckCloseDispute();
    } else {
        sendBack();
    }
}
void CheckCloseDispute(void) {

    if ((getTimeIsUp() && Dispute) || Objection){

        if (Objection){
            Objection = false;
            ConciliationAmount = 0;
            TimeOut = 0;
        } else {

            sendOut(BuySellAmount - ConciliationAmount, ConciliationAmount);
            sendMessageSC(CREATOR, CreateOrderTX, AcceptOrderTX, currentTX.txId, Responder);

            reset();

        }

    } else {
        sendBack();
    }

}

void FinishOrder(void) {

    if(currentTX.amount >= GAS_FEE && checkCandidates() && checkIfSenderIsOneCandidate()) {

        if (Initiator == currentTX.sender){
            sendOut(0, BuySellAmount);
        }
		else {
            sendOut(BuySellAmount, 0);
        }
        sendMessageSC(Initiator, CreateOrderTX, AcceptOrderTX, currentTX.txId, Responder);
        reset();

    }
	else {
        sendBack();
    }
}

void InjectChainSwapHash(void) {

    if (checkCandidates() && !isFiatOrder && checkEmptyChainSwapHashLongs() && ((SellOrder && Initiator == currentTX.sender) || (!SellOrder && Responder == currentTX.sender))) {
        ChainSwapHashLong1 = currentTX.message[1];
		ChainSwapHashLong2 = currentTX.message[2];
		ChainSwapHashLong3 = currentTX.message[3];
		ChainSwapHashLong4 = currentTX.message[4];
    } else {
        sendBack();
    }
}

void FinishOrderWithChainSwapKey(void) {
	
	if (checkCandidates() && checkNotEmptyChainSwapHashLongs() && checkChainSwapHashLongs(currentTX.message[1], currentTX.message[2], currentTX.message[3], currentTX.message[4]) && !getTimeIsUp()) {

		if(SellOrder){
			sendOut(0, BuySellAmount);
		} else {
			sendOut(BuySellAmount, 0);
		}
		
		sendMessageSC(Initiator, CreateOrderTX, AcceptOrderTX, currentTX.txId, Responder);
		reset();

	} else {
		sendBack();
	}
	
}

long getGasFee(){
	
	if(FreeForAll){
		return TENTHOUSAND * 3930;
	}
	else {
		return TENTHOUSAND * 4380;
	}
}

long getSumCollateral(void) {
    long SumCollateral = InitiatorsCollateral + RespondersCollateral;

    if (SumCollateral <= 0){
        SumCollateral = TENTHOUSAND * TENTHOUSAND;
    }
    return SumCollateral;
}
long checkCandidates(void) {return Initiator != 0 && Responder != 0;}
long checkIfSenderIsOneCandidate(void) {return currentTX.sender == Initiator || currentTX.sender == Responder;}

void setTimeOut(long Time){TimeOut = Get_Block_Timestamp() + ((Time / 4) << 32);} //+(360 << 32); 15 * ~4min/block = 60min = 1 hour locktime
//void setTimeOut(long Time) { TimeOut = Add_Minutes_To_Timestamp(Get_Block_Timestamp(), Time);}
long getTimeIsUp(void){return TimeOut != 0 && Get_Block_Timestamp() > TimeOut;}

long checkChainSwapHashLongs(long A1, long A2, long A3, long A4){
	Set_A1_A2(A1, A2);
    Set_A3_A4(A3, A4);
	Set_B1_B2(ChainSwapHashLong1, ChainSwapHashLong2);
	Set_B3_B4(ChainSwapHashLong3, ChainSwapHashLong4);
	return Check_SHA256_A_With_B();
    //SHA256_A_To_B();
	//return ChainSwapHashLong1 == Get_B1() && ChainSwapHashLong2 == Get_B2() && ChainSwapHashLong3 == Get_B3() && ChainSwapHashLong4 == Get_B4();
}

long checkEmptyChainSwapHashLongs(void){
	return ChainSwapHashLong1 == 0 && ChainSwapHashLong2 == 0 && ChainSwapHashLong3 == 0 && ChainSwapHashLong4 == 0;
}

long checkNotEmptyChainSwapHashLongs(void){
	return ChainSwapHashLong1 != 0 && ChainSwapHashLong2 != 0 && ChainSwapHashLong3 != 0 && ChainSwapHashLong4 != 0;
}

void sendOut(long InitiatorsAmount, long RespondersAmount) {
    long SumCollateral = getSumCollateral() - GAS_FEE;
    long HalfCollateral = SumCollateral / 2;
    
	if (Dispute) {
        InitiatorsAmount += (HalfCollateral / 2);
        RespondersAmount += (HalfCollateral / 2);
        sendAmount(HalfCollateral + MediatorsDeposit, CREATOR);
    }
	else {
        InitiatorsAmount += InitiatorsCollateral - (GAS_FEE / 2);
        RespondersAmount += RespondersCollateral - (GAS_FEE / 2);
    }
	
    if (InitiatorsAmount > 0) { sendAmount(InitiatorsAmount, Initiator); }
    if (RespondersAmount > 0) { sendAmount(RespondersAmount, Responder); }

	//long Temp = (Get_Current_Balance() - GAS_FEE) / 2;
	//if (Temp > 0) {
	//	sendAmount(Temp, Initiator);
	//	sendAmount(Temp, Responder);
	//}
	
}
void sendBack(void) {sendAmount(currentTX.amount, currentTX.sender);}
void reset(void) {
    CreateOrderTX = 0;
    AcceptOrderTX = 0;

    Initiator = 0;
    Responder = 0;

	Dispute = false;
	ChainSwapHashLong1 = 0;
	ChainSwapHashLong2 = 0;
	ChainSwapHashLong3 = 0;
	ChainSwapHashLong4 = 0;
    TimeOut = 0;

	MediatorsDeposit = 0;

	if (Deniability == 3) {
		Deniability = 1;
	}

    InitiatorsCollateral =  0;
    RespondersCollateral = 0;
    
    BuySellAmount = 0;
    ConciliationAmount = 0;
	
	if (Get_Current_Balance() > GAS_FEE) {
		sendAmount(Get_Current_Balance() - GAS_FEE, CREATOR);
	}
}