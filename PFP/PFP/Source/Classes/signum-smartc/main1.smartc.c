/**
 * @author evolver
 * Translated to SmartC by Deleterium
 */

#include APIFunctions
#pragma maxConstVars 2
#pragma version 1.0

long CreateOrderTX = 0, AcceptOrderTX = 0;
long Initiator = NULL, Responder = NULL;
long InitiatorsCollateral = 0, RespondersCollateral = 0, MediatorsDeposit = 0, BuySellAmount = 0, ConciliationAmount = 0, ChainSwapHash = 0, TimeOut = 0;
long SellOrder = false, FreeForAll = false, isFiatOrder = false, Deniability = 0, Dispute = false, Objection = false, Decimals = 8;

#define FIAT_AUD 0x0000000000415544
#define FIAT_BRL 0x000000000042524c
#define FIAT_CAD 0x0000000000434144
#define FIAT_CNY 0x0000000000434e59
#define FIAT_CZK 0x0000000000435a4b
#define FIAT_DKK 0x0000000000444b4b
#define FIAT_EUR 0x0000000000455552
#define FIAT_HKD 0x0000000000484b44
#define FIAT_HUF 0x0000000000485546
#define FIAT_INR 0x0000000000494e52
#define FIAT_ILS 0x0000000000494c53
#define FIAT_JPY 0x00000000004a5059
#define FIAT_MYR 0x00000000004d5952
#define FIAT_MXN 0x00000000004d584e
#define FIAT_TWD 0x0000000000545744
#define FIAT_NZD 0x00000000004e5a44
#define FIAT_NOK 0x00000000004e4f4b
#define FIAT_PHP 0x0000000000504850
#define FIAT_PLN 0x0000000000504c4e
#define FIAT_GBP 0x0000000000474250
#define FIAT_RUB 0x0000000000525542
#define FIAT_SGD 0x0000000000534744
#define FIAT_SEK 0x000000000053454b
#define FIAT_CHF 0x0000000000434846
#define FIAT_THB 0x0000000000544842
#define FIAT_USD 0x0000000000555344

#define ACTIVATE_DEACTIVATE_DISPUTE 0x805352d2a4817dd3
#define CREATE_ORDER 0x09f2535fcf54cc3b
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
        message[4];
} currentTX;

long messageToSend[4];

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

	if (XAmount < BuySellAmount && XAmount <= 9223372000000000) { // 9_0000_0000 < 100_0000_0000
		long T_Sum = XAmount * 1000 / BuySellAmount; //9_0000_0000 * 1000 / 100_0000_0000 = 9 = 0.009 XItem
		//								       92_233_720_0000_0000 * 1000
		//								   92_233_720_368_5477_5807 MAX long
		if ((T_Sum >= 10 && Decimals == 2) || (T_Sum >= 1000 && Decimals == 0)) {
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
        default:
            // Maybe add an error message?
        }
    } while (true);
    // No more TX to process;
}
void getTxDetails(void) {
    currentTX.txId = Get_A1();
    currentTX.amount = Get_Amount_For_Tx_In_A();
    currentTX.timestamp = Get_Timestamp_For_Tx_In_A();
    Message_From_Tx_In_A_To_B();
    currentTX.message[0] = Get_B1();
    currentTX.message[1] = Get_B2();
    currentTX.message[2] = Get_B3();
    currentTX.message[3] = Get_B4();
    B_To_Address_Of_Tx_In_A();
    currentTX.sender = Get_B1();
}
void sendMessageSC(long recipient) {
    Set_B1(recipient);
    Set_A1_A2(messageToSend[0], messageToSend[1]);
    Set_A3_A4(messageToSend[2], messageToSend[3]);
    Send_A_To_Address_In_B();
}
void sendAmount(long amount, long recipient) {
    Set_B1(recipient);
    Send_To_Address_In_B(amount);
}
long performSHA256_64(long A1, long A2) {
    Clear_A();
    Set_A1_A2(A1, A2);
    SHA256_A_To_B();
	return Get_B1();
}

void ActivateDeactivateDispute(void) {
    if (Initiator == NULL && currentTX.sender == CREATOR && FreeForAll) {
        Deniability = !Deniability;
    } else {
        sendBack();
    }
}

// Suposed to receive args on message second long
void CreateOrder(void) {
    long T_BuyCollateralAmount = currentTX.message[1];
	checkFiat();
	
    if (Initiator == NULL && (FreeForAll || currentTX.sender == CREATOR) && T_BuyCollateralAmount > 0 && currentTX.amount > 29400000) {

        FreeForAll = true;
        CreateOrderTX = currentTX.txId ;

        if (currentTX.amount > T_BuyCollateralAmount) {
            SellOrder = true;
            BuySellAmount = currentTX.amount - T_BuyCollateralAmount;//Sell: 100 = 130 - 30;
            InitiatorsCollateral = T_BuyCollateralAmount;//Sell: 30
        } else {
            SellOrder = false;
            InitiatorsCollateral = currentTX.amount;//Buy: 30;
            BuySellAmount = T_BuyCollateralAmount;//Buy: 100
        }

		if ((isFiatOrder && checkOneCent(currentTX.message[2], BuySellAmount)) || !isFiatOrder) {
			
			if (InitiatorsCollateral >= 0) {
				Initiator = currentTX.sender;

				if (Initiator == CREATOR && Deniability == 1 || !isFiatOrder && Deniability == 1) {
					Deniability = 3;
				}

			} else {
				sendBack();
			}

		} else {
			sendBack();
		}

    } else {
        sendBack();
    }

}
void AcceptOrder(void) {
    if (Initiator != NULL && Responder == NULL) {

        Responder = currentTX.sender;

        if (Initiator == Responder){

            sendAmount(Get_Current_Balance() - 29400000, Initiator);
            messageToSend[0] = CreateOrderTX;
            messageToSend[1] = currentTX.txId;
            messageToSend[2] = currentTX.txId;
            messageToSend[3] = 0;
            sendMessageSC(Initiator);
            reset();

        } else {

            if (SellOrder){
                RespondersCollateral = currentTX.amount ;//30
            } else {
                RespondersCollateral = currentTX.amount - BuySellAmount ;//30 = 130 - 100
            }

            if ((RespondersCollateral < InitiatorsCollateral) || (RespondersCollateral > InitiatorsCollateral * 2)){ //Sell=30 0000 0000 < 30 0000 0000 = false; Buy=40 0000 0000 < 40 0000 0000 = false
                Responder = NULL;
                sendBack();
            } else {
                AcceptOrderTX = currentTX.txId;

				if (Responder == CREATOR && Deniability == 1) {
					Deniability = 3;
				}
            }
        }
    } else {
        sendBack();
    }
}
// Suposed to have the recipient as second long in incoming message!
void InjectResponder(void) {
    if (Initiator == currentTX.sender && SellOrder && Responder == NULL){
		
		if (Deniability == 1) {
			Deniability = 3;
		}
        
		Responder = currentTX.message[1];
        AcceptOrderTX = currentTX.txId;
    } else {
        sendBack();
    }
}

void OpenDispute(void) {
    if (checkCandidates() && checkIfSenderIsOneCandidate() && Deniability == 1 && isFiatOrder) {
        Dispute = true;
        messageToSend[] = 0;
        messageToSend[1] = currentTX.txId;
        sendMessageSC(CREATOR);
    } else {
        sendBack();
    }
}
// Suposed to receive Percentage as second long
void MediateDispute(void) {
    long Percentage = currentTX.message[1];

    if (checkCandidates() && currentTX.sender == CREATOR && (currentTX.amount >= (getSumCollateral() / 2) + MediatorsDeposit) && Percentage >= 0 && Dispute) {
		MediatorsDeposit += currentTX.amount;
		
        if (Percentage >= 10000){
            Percentage = 10000;
        }

        ConciliationAmount = (BuySellAmount / 10000) * Percentage;
		TimeOut = Get_Block_Timestamp() + (3 << 32);// +(360 << 32);

    } else {
        sendBack();
    }
}
void Appeal(void) {
    if (checkCandidates() && checkIfSenderIsOneCandidate() && (Get_Block_Timestamp() <= TimeOut) && Dispute){
        Objection = true;
        CheckCloseDispute();
    } else {
        sendBack();
    }
}
void CheckCloseDispute(void) {

    if ((Get_Block_Timestamp() > TimeOut && TimeOut != 0 && Dispute) || Objection){

        if (Objection){
            Objection = false;
            ConciliationAmount = 0;
            TimeOut = 0;
        } else {

            sendOut(BuySellAmount - ConciliationAmount, ConciliationAmount);
            messageToSend[0] = CreateOrderTX;
            messageToSend[1] = AcceptOrderTX;
            messageToSend[2] = currentTX.txId;
            messageToSend[3] = Responder;
            sendMessageSC(CREATOR);

            reset();

        }

    } else {
        sendBack();
    }

}

void FinishOrder(void) {

    if(checkCandidates() && checkIfSenderIsOneCandidate() && currentTX.amount >= 29400000 * 2) {

        if (Initiator == currentTX.sender){
            sendOut(0, BuySellAmount);
        } else {
            sendOut(BuySellAmount,0);
        }

        messageToSend[0] = CreateOrderTX;
        messageToSend[1] = AcceptOrderTX;
        messageToSend[2] = currentTX.txId;
        messageToSend[3] = Responder;
        sendMessageSC(Initiator);

        reset();

    } else {
        sendBack();
    }

}
// Suposed to receive the chainswaphash as second long
void InjectChainSwapHash(void) {
    long T_ChainSwapHash = currentTX.message[1];

    if (checkCandidates() && ChainSwapHash == 0 && ((SellOrder && Initiator == currentTX.sender) || (!SellOrder && Responder == currentTX.sender))) {
        ChainSwapHash = T_ChainSwapHash;
    } else {
        sendBack();
    }
}
// Suposed to receive arguments thru message second and third longs
void FinishOrderWithChainSwapKey(void) {
    long firstChainSwapLong = currentTX.message[1];
    long secondChainSwapLong = currentTX.message[2];
    long T_ChainSwapHash = performSHA256_64(firstChainSwapLong, secondChainSwapLong);

    if (checkCandidates() && ChainSwapHash != 0 && T_ChainSwapHash == ChainSwapHash) {

        if(SellOrder){
            sendOut(0, BuySellAmount);
        } else {
            sendOut(BuySellAmount, 0);
        }

        messageToSend[0] = CreateOrderTX;
        messageToSend[1] = AcceptOrderTX;
        messageToSend[2] = currentTX.txId;
        messageToSend[3] = Responder;
        sendMessageSC(Initiator);

        reset();

    } else {
        sendBack();
    }
}

long getSumCollateral(void) {
    long SumCollateral = InitiatorsCollateral + RespondersCollateral;

    if (SumCollateral <= 0){
        SumCollateral = 100000000;
    }
    return SumCollateral;
}
long checkCandidates(void) {return Initiator != NULL && Responder != NULL;}
long checkIfSenderIsOneCandidate(void) {return currentTX.sender == Initiator || currentTX.sender == Responder;}

void sendOut(long InitiatorsAmount, long RespondersAmount) {
    long SumCollateral = getSumCollateral();
    long HalfCollateral = SumCollateral / 2;
    
	if (Dispute) {
        InitiatorsAmount += (HalfCollateral / 2);
        RespondersAmount += (HalfCollateral / 2);
        sendAmount(HalfCollateral + MediatorsDeposit, CREATOR);
    } else {
        InitiatorsAmount += InitiatorsCollateral;
        RespondersAmount += RespondersCollateral;
    }
	    
    sendAmount(InitiatorsAmount, Initiator);
    sendAmount(RespondersAmount, Responder);

	long Temp = (Get_Current_Balance() - 29400000) / 2;
	if (Temp > 0) {
		sendAmount(Temp, Initiator);
		sendAmount(Temp, Responder);
	}
	
}
void sendBack(void) {sendAmount(currentTX.amount, currentTX.sender);}
void reset(void) {
    CreateOrderTX = 0;
    AcceptOrderTX = 0;

    Initiator = NULL;
    Responder = NULL;

	Dispute = false;
	ChainSwapHash = 0;
    TimeOut = 0;

	MediatorsDeposit = 0;

	if (Deniability == 3) {
		Deniability = 1;
	}

    InitiatorsCollateral =  0;
    RespondersCollateral = 0;
    
    BuySellAmount = 0;
    ConciliationAmount = 0;
	
}