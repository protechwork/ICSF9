//#region varaiables 
var loginDetails = {};
var logDetails = {};
var VariantID = 0;
var docNo = "0";
var docDate = "0";
var fgQty = 0;
var fgCode = 0;
var warehouse = "0";
var Narration =""; 


var item = 0;
var quantity =0;
var rate = 0;
var gross =0;
var partybatchno =0;
var ItemRemarks = "";
var OperatorName = "";
var MachineName = "";

var iDocDate = "0";
var logDetails = {};
var selectedRow = 1;
var requestIds = [];
var requestsProcessed = [];
var bodyRequestsProcessed = [];
var requestId = 0;
var validRows = 0;
var totalRows = 0;
var validRowsArray = [];
var noofRows = 0;
var rowData = {};
var tableData = [];
var sAbbr = "";
var bodyData = [];
var bodyRequests = [];
var BodyDataArr = [];
var vsBodyData = {};
var requestId = 1;
var requestsProcessed = [];
var AUTO_JVNO = 0;
var Exchange = 0;
var Location = 0;
var Cheque = 0;
var Card = 0;
var Pass = 0;
var Online = 0;
var Finance = 0;
var Wallet = 0;
var Warehouse = 0;
var Finance_Option = 0;
var dFinance = 0;
var ApprovedFinance = 0;
var hData = {};
var Cash = 0;
var BalanceRemarks = 0;
var baseUrl = 'http://localhost/F9ICSKaleGrp_As';
//#endregion

function isRequestCompleted(iRequestId, processedRequestsArray) {

    return processedRequestsArray.indexOf(iRequestId) === -1 ? false : true;

}

function isRequestProcessed(iRequestId) {
    for (let i = 0; i < requestsProcessed.length; i++) {
        if (requestsProcessed[i] == iRequestId) {
            return true;
        }
    } return false;
}



//#region Start of P E  
function PEAfterSaves(loginDetails, rowIndex) {
    //alert(loginDetails);	
    debugger;
    requestId = 0;
    requestsProcessed = [];
    Focus8WAPI.getFieldValue("fieldValueCallbackIssueAfterSave", ["", "DocNo", "Date","Warehouse", "sNarration","FGCode","FGQty","MachineName","OperatorName"],Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
    
}

function fieldValueCallbackIssueAfterSave(response) {
    debugger;
    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
	logDetails = response.data[0];
    docNo = response.data[1].FieldValue;
	docDate = response.data[2].FieldValue;
	warehouse = response.data[3].FieldValue;
        
	var data = {
	    CompanyId: logDetails.CompanyId,
	    SessionId: logDetails.SessionId,
	    LoginId: logDetails.LoginId,
	    vtype: logDetails.iVoucherType,
	    docNo: docNo,
	    BodyData: bodyData

	};

	console.log({ data })
	$.ajax({
	    type: "POST",
	    url: "/F9ICSKaleGrp_As/UpdateStockProduce/UpdateSP",
	    data: JSON.stringify(data),
	    contentType: "application/json; charset=utf-8",
	    async: false,
	    success: function (r) {
	        debugger
	        alert("Data Posted Succesfully")
	        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
	    },
	    error: function (e) {
	        console.log(e.message)
	        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
	    }

	});

        
    //Focus8WAPI.getBodyFieldValue("PEbodyCallbackAfterSave", ["Item", "Quantity", "Rate", "Gross", "PartyBatchNo", "ItemRemarks"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, requestId++)
	//Focus8WAPI.getBodyFieldValue("PEbodyCallbackAfterSave", ["Item", "Quantity", "Rate", "Gross", "PartyBatchNo", "ItemRemarks"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, requestId++)
	

    } catch (e) {
        console.log("fieldValueCallbacksupplierValidate", e.message)
    }
}


function PEbodyCallbackAfterSave(response) {
    debugger;
    try {
        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
        
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        console.log(response)
        debugger;
        validRows = response.RowsInfo.iValidRows;
        totalRows = response.RowsInfo.iTotalRows;
        bodyData = [];
        vsBodyData = {};
        BodyDataArr = [];

        //Focus8WAPI.getBodyFieldValue('PErowCallBack', ["", "*"], 2, false, 1, ++requestId);

        var data = {
            CompanyId: logDetails.CompanyId,
            SessionId: logDetails.SessionId,
            LoginId: logDetails.LoginId,
            vtype: logDetails.iVoucherType,
            docNo: docNo,
            BodyData: bodyData

        };

        console.log({ data })
        $.ajax({
            type: "POST",
            url: "/F9ICSKaleGrp_As/UpdateStockProduce/UpdateSP",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (r) {
                debugger
                alert("Data Posted Succesfully")
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            },
            error: function (e) {
                console.log(e.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            }

        });
    } catch (e) {
        console.log("bodyCallbackAfterSave", e.message)
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
    }

}
function openExtraTab() {
        var myWindow = window.open("", "myWindow", "width=200,height=100");   // Opens a new window
        myWindow.document.write("<p>I replaced the current window.</p>");
    
}
function PErowCallBack(response) {
    console.log('PErowCallBack :: ', response);
    debugger;
    if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);
    validRows = response.RowsInfo.iValidRows;
    totalRows = response.RowsInfo.iTotalRows;
    bodyData = [];

    vsBodyData = {};
    vsBodyDataArr = [];

    bodyRequestsProcessed = [];
    for (let i = 1 ; i <= validRows; i++) {
        Focus8WAPI.getBodyFieldValue('PEinitializeRow', ["*"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, i, i);
    }
}

function PEinitializeRow(response) {

    try {
        if (isRequestCompleted(response.iRequestId, bodyRequestsProcessed)) {
            return;
        }
        bodyRequestsProcessed.push(response.iRequestId);

        const row = initializeRowDataFields(response.data.slice(0));
        bodyData.push(response.data.slice(0));
        vsBodyData[response.iRequestId] = row;

        //if (validRowsRMASales === Object.values(vsBodyData).length) {
        //    Focus8WAPI.getFieldValue("headerSalesCallbackalter", ["", "DocNo"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, ++requestId);
        //}
    } catch (e) {
        alert(e.message)
    }
}

//#endregion



function bodyCallbackAfterSave(response) {
    debugger;
    try {
        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        console.log(response)
        item = response.data[0].FieldValue;
        partybatchno = response.data[1].FieldValue;
        ItemRemarks = response.data[2].FieldValue;


        var data = {
            CompanyId: logDetails.CompanyId,
            SessionId: logDetails.SessionId,
            LoginId: logDetails.LoginId,
            vtype: logDetails.iVoucherType,
            docNo: docNo
        };

        console.log({ data })
        $.ajax({
            type: "POST",
            url: "/F9ICSKaleGrp_As/RaiseFinanceJV/RaiseJV",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (r) {
                debugger
                alert("Data Posted Succesfully")
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            },
            error: function (e) {
                console.log(e.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            }

        });
    } catch (e) {
        console.log("bodyCallbackAfterSave", e.message)
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
    }

}


function loadFinance() {

    if (sessionStorage.getItem("Finance_Option") == 1 && parseFloat(sessionStorage.getItem("Finance")) > 0) {

        var url = baseUrl + `/FinanceReceipt/GetFinanceRecieptDetails?CompanyId=${sessionStorage.getItem("CompanyId")}&SessionId=${sessionStorage.getItem("SessionId")}&LoginId=${sessionStorage.getItem("LoginId")}&vtype=${sessionStorage.getItem("iVoucherType")}&docNo=${sessionStorage.getItem("docNo")}&Finance=${sessionStorage.getItem("Finance")}&idocDate=${sessionStorage.getItem("idocDate")}`;
        console.log({ logDetails, docNo, url })
        Focus8WAPI.openPopup(url, openPopupCallBackBeforeSave);
    }
}
function loadCheque() {

    if (parseFloat(sessionStorage.getItem("Cheque")) > 0) {

        var url = baseUrl + `/BuyBack/GetCheckRecieptDetails?CompanyId=${sessionStorage.getItem("CompanyId")}&SessionId=${sessionStorage.getItem("SessionId")}&LoginId=${sessionStorage.getItem("LoginId")}&vtype=${sessionStorage.getItem("iVoucherType")}&docNo=${sessionStorage.getItem("docNo")}&Cheque=${sessionStorage.getItem("Cheque")}`;
        console.log({ logDetails, docNo, url })
        Focus8WAPI.openPopup(url, openPopupCallBackBeforeSave);
    }
    else {
        loadPass();
    }
}
function loadCard() {

    if (parseFloat(sessionStorage.getItem("Card")) > 0) {

        var url = baseUrl + `/CardReceipt/GetCardRecieptDetails?CompanyId=${sessionStorage.getItem("CompanyId")}&SessionId=${sessionStorage.getItem("SessionId")}&LoginId=${sessionStorage.getItem("LoginId")}&vtype=${sessionStorage.getItem("iVoucherType")}&docNo=${sessionStorage.getItem("docNo")}&Card=${sessionStorage.getItem("Card")}&Location=${sessionStorage.getItem("Location")}&idocDate=${sessionStorage.getItem("idocDate")}`;
        console.log({ logDetails, docNo, url })
        Focus8WAPI.openPopup(url, openPopupCallBackBeforeSave);
    }
 
    else
    {
        loadPass();
    }

}
function loadPass() {

    if (parseFloat(sessionStorage.getItem("Pass")) > 0) {

        var url = baseUrl + `/PassReceipt/GetPassRecieptDetails?CompanyId=${sessionStorage.getItem("CompanyId")}&SessionId=${sessionStorage.getItem("SessionId")}&LoginId=${sessionStorage.getItem("LoginId")}&vtype=${sessionStorage.getItem("iVoucherType")}&docNo=${sessionStorage.getItem("docNo")}&Pass=${sessionStorage.getItem("Pass")}&idocDate=${sessionStorage.getItem("idocDate")}`;
        console.log({ logDetails, docNo, url })
        Focus8WAPI.openPopup(url, openPopupCallBackBeforeSave);
    }
    else {
        loadOnline();
    }
}
function loadOnline() {
    debugger
    if (parseFloat(sessionStorage.getItem("Online")) > 0) {

        var url = baseUrl + `/OnlineReciept/GetOnlineRecieptDetails?CompanyId=${sessionStorage.getItem("CompanyId")}&SessionId=${sessionStorage.getItem("SessionId")}&LoginId=${sessionStorage.getItem("LoginId")}&vtype=${sessionStorage.getItem("iVoucherType")}&docNo=${sessionStorage.getItem("docNo")}&Online=${sessionStorage.getItem("Online")}&idocDate=${sessionStorage.getItem("idocDate")}`;
        console.log({ logDetails, docNo, url })
        Focus8WAPI.openPopup(url, openPopupCallBackBeforeSave);
    }
    else {
        loadWallet();
    }
}

function loadWallet() {

    if (parseFloat(sessionStorage.getItem("Wallet")) > 0) {

        var url = baseUrl + `/WalletReceipt/GetWalletRecieptDetails?CompanyId=${sessionStorage.getItem("CompanyId")}&SessionId=${sessionStorage.getItem("SessionId")}&LoginId=${sessionStorage.getItem("LoginId")}&vtype=${sessionStorage.getItem("iVoucherType")}&docNo=${sessionStorage.getItem("docNo")}&Wallet=${sessionStorage.getItem("Wallet")}&idocDate=${sessionStorage.getItem("idocDate")}`;
        console.log({ logDetails, docNo, url })
        Focus8WAPI.openPopup(url, openPopupCallBackBeforeSave);
    }
}

function openPopupCallBackBeforeSave(objWrapperResult) {
    try {
        debugger;
        console.log(objWrapperResult);
    }
    catch (err) {
        alert("Exception :: openPopupCallBackBeforeSave :" + err.message);
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
    }
}
//function RaiseJVAfterSave(...args) {
function RaiseJVAfterSave(logDetails, rowIndex) {

    Focus8WAPI.getFieldValue("HeaderCallbackData", ["", "*"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId++);

}

var HBuyBack = [];
var sIncrement = 0;

var sIncrementII = 0;

var sIncrementIII = 0;

var sIncrementIV = 0;

var sIncrementV = 0;

var remarks = null;
var receipt = [];
var Gross = 0;
var dNetAmt = 0;
function HeaderCallbackData(response) {
    debugger;
    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        logDetails = response.data[0];
        //console.log(logDetails)
        docNo = response.data[1].FieldValue;
        
        hData = initializeFields(response.data.slice(1));

        //remarks = hData["Balance_Remarks"]["FieldValue"];
  
        //BalanceRemarks = prompt('Enter MOP Remarks', remarks);

       
       //  Focus8WAPI.setFieldValue("setFieldCallback", ["Balance_Remarks"], [remarks], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId++);
      // Focus8WAPI.getBodyFieldValue("bodyCallbackAfterSaves", ["Exchange", "Cheque", "Card", "Pass", "Online", "Finance", "Wallet E-Pay", "Approved Finance", "Cash"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, -1, requestId++)

        Focus8WAPI.getBodyFieldValue("newbodyCallbackAfterSaves", ["Gross"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, requestId++)



    } catch (e) {
        console.log("HeaderCallbackData", e.message)
    }
}


function setFieldCallback(res) {
    console.log(JSON.stringify(res));
}

function newbodyCallbackAfterSaves(response) {

    try {
        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        //hData = initializeFields(response.data.slice(1));
     //   alert(2)
        console.log(response)
        Gross = response.data[0].FieldValue;
    Focus8WAPI.getBodyFieldValue("bodyCallbackAfterSaves", ["Exchange", "Cheque", "Card", "Pass", "Online", "Finance", "Wallet E-Pay", "Approved Finance", "Cash"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, -1, requestId++)


    
    
    } catch (e) {
        console.log("bodyCallbackAfterSaves", e.message)
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
    }

}

function bodyCallbackAfterSaves(response) {

    try {
        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        //hData = initializeFields(response.data.slice(1));

        sIncrement = "I";
        sIncrementII = "II";
        sIncrementIII = "III";
        sIncrementIV = "IV";
        sIncrementV = "V";

        Exchange = response.data[0].FieldValue;
        Cheque = response.data[1].FieldValue;
        Card = response.data[2].FieldValue;
        Pass = response.data[3].FieldValue;
        Online = response.data[4].FieldValue;
        Finance = response.data[5].FieldValue;
        Wallet = response.data[6].FieldValue;
        ApprovedFinance = response.data[7].FieldValue;
        Cash = response.data[8].FieldValue;

        if (Exchange == 0 && Cheque == 0 && Card == 0 && Pass == 0 && Online == 0 && Finance == 0 && Wallet == 0 && ApprovedFinance == 0 && Cash == 0) {
            console.log("MOP Amount is Zero")


            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            return;
        }
        var FooterAmt = ( parseFloat(Exchange) + parseFloat(Cheque) + parseFloat(Card) +  parseFloat(Pass) +  parseFloat(Online)+  parseFloat(Finance) +  parseFloat(Wallet) +  parseFloat(ApprovedFinance) +  parseFloat(Cash));
        console.log(FooterAmt);
        dNetAmt = (parseFloat(Gross)) -(parseFloat(FooterAmt));
       // alert(dNetAmt)
        console.log(dNetAmt)
        if (dNetAmt > 0) {
            remarks = hData["Balance_Remarks"]["FieldValue"];
            //   remarks = "Good";
            BalanceRemarks = prompt('Enter MOP Remarks', remarks);

        }
        HBuyBack = [{
            Exchange: toNumber(response.data[0].FieldValue), Cheque: toNumber(response.data[1].FieldValue),

                Card: toNumber(response.data[2].FieldValue), Pass: toNumber(response.data[3].FieldValue),

                    Online: toNumber(response.data[4].FieldValue), Finance: toNumber(response.data[5].FieldValue),

                        Wallet: toNumber(response.data[6].FieldValue), ApprovedFinance: toNumber(response.data[7].FieldValue),
                Cash:toNumber(response.data[8].FieldValue)

        }
        ];
        console.log(HBuyBack)

        console.log(hData)
        receipt = [

            {

                Prod: hData["Prod" + sIncrement]["FieldValue"], ProdQty: hData["ProdQty" + sIncrement]["FieldValue"], Prod_Rate: hData["Prod_Rate" + sIncrement]["FieldValue"], ProdGross: hData["ProdGross" + sIncrement]["FieldValue"],
                 Cheque_Account: hData["Cheque_Account" + sIncrement]["FieldValue"], Cheque_Amt: hData["Cheque_Amt" + sIncrement]["FieldValue"],
                 Pass_Type: hData["Pass_Type" + sIncrement]["FieldValue"],PService_Charge: hData["PService_Charge" + sIncrement]["FieldValue"],
                 PGST: hData["PGST" + sIncrement]["FieldValue"],PDisc: hData["PDisc" + sIncrement]["FieldValue"],Pass_Amount: hData["Pass_Amount" + sIncrement]["FieldValue"],
                 Online_Type: hData["Online_Type" + sIncrement]["FieldValue"], Service_Charge: hData["Service_Charge" + sIncrement]["FieldValue"],
                 OnlineGST: hData["GST" + sIncrement]["FieldValue"], Online_Amt: hData["Online_Amt" + sIncrement]["FieldValue"],
                 EDC_Batch_ID: hData["EDC_Batch_ID" + sIncrement]["FieldValue"], MSF: hData["MSF" + sIncrement]["FieldValue"],
                 CardGST: hData["CRDGST" + sIncrement]["FieldValue"], Amt_Processed: hData["Amt_Processed" + sIncrement]["FieldValue"],
                 WType: hData["WType" + sIncrement]["FieldValue"], WSer_Charge: hData["WSer_Charge" + sIncrement]["FieldValue"],
                 WGST: hData["WGST" + sIncrement]["FieldValue"], WAmt: hData["WAmt" + sIncrement]["FieldValue"],
                 MSFAmount: hData["MSF" + sIncrement + "_Amount"]["FieldValue"], GSTAmount: hData["GST" + sIncrement + "_Amount"]["FieldValue"],
            },

        {
            Prod: hData["Prod" + sIncrementII]["FieldValue"], ProdQty: hData["ProdQty" + sIncrementII]["FieldValue"], Prod_Rate: hData["Prod_Rate" + sIncrementII]["FieldValue"], ProdGross: hData["ProdGross" + sIncrementII]["FieldValue"],
            Cheque_Account: hData["Cheque_Account" + sIncrementII]["FieldValue"], Cheque_Amt: hData["Cheque_Amt" + sIncrementII]["FieldValue"],
            Pass_Type: hData["Pass_Type" + sIncrementII]["FieldValue"], PService_Charge: hData["PService_Charge" + sIncrementII]["FieldValue"],
            PGST: hData["PGST" + sIncrementII]["FieldValue"], PDisc: hData["PDisc" + sIncrementII]["FieldValue"], Pass_Amount: hData["Pass_Amount" + sIncrementII]["FieldValue"],
            Online_Type: hData["Online_Type" + sIncrementII]["FieldValue"], Service_Charge: hData["Service_Charge" + sIncrementII]["FieldValue"],
            OnlineGST: hData["GST" + sIncrementII]["FieldValue"], Online_Amt: hData["Online_Amt" + sIncrementII]["FieldValue"],


            EDC_Batch_ID: hData["EDC_Batch_ID" + sIncrementII]["FieldValue"], MSF: hData["MSF" + sIncrementII]["FieldValue"],
            CardGST: hData["CRDGST" + sIncrementII]["FieldValue"], Amt_Processed: hData["Amt_Processed" + sIncrementII]["FieldValue"],
            WType: hData["WType" + sIncrementII]["FieldValue"], WSer_Charge: hData["WSer_Charge" + sIncrementII]["FieldValue"],
            WGST: hData["WGST" + sIncrementII]["FieldValue"], WAmt: hData["WAmt" + sIncrementII]["FieldValue"],
            MSFAmount: hData["MSF" + sIncrementII + "_Amount"]["FieldValue"], GSTAmount: hData["GST" + sIncrementII + "_Amount"]["FieldValue"],

        },

           {
               Prod: hData["Prod" + sIncrementIII]["FieldValue"], ProdQty: hData["ProdQty" + sIncrement]["FieldValue"], Prod_Rate: hData["Prod_Rate" + sIncrementIII]["FieldValue"], ProdGross: hData["ProdGross" + sIncrementIII]["FieldValue"],
               Cheque_Account: hData["Cheque_Account" + sIncrementIII]["FieldValue"], Cheque_Amt: hData["Cheque_Amt" + sIncrementIII]["FieldValue"],
               Pass_Type: hData["Pass_Type" + sIncrementIII]["FieldValue"], PService_Charge: hData["PService_Charge" + sIncrementIII]["FieldValue"],
               PGST: hData["PGST" + sIncrementIII]["FieldValue"], PDisc: hData["PDisc" + sIncrementIII]["FieldValue"], Pass_Amount: hData["Pass_Amount" + sIncrementIII]["FieldValue"],
               Online_Type: hData["Online_Type" + sIncrementIII]["FieldValue"], Service_Charge: hData["Service_Charge" + sIncrementIII]["FieldValue"],
               OnlineGST: hData["GST" + sIncrementIII]["FieldValue"], Online_Amt: hData["Online_Amt" + sIncrementIII]["FieldValue"],

               EDC_Batch_ID: hData["EDC_Batch_ID" + sIncrementIII]["FieldValue"], MSF: hData["MSF" + sIncrementIII]["FieldValue"],
               CardGST: hData["CRDGST" + sIncrementIII]["FieldValue"], Amt_Processed: hData["Amt_Processed" + sIncrementIII]["FieldValue"],
               WType: hData["WType" + sIncrementIII]["FieldValue"], WSer_Charge: hData["WSer_Charge" + sIncrementIII]["FieldValue"],
               WGST: hData["WGST" + sIncrementIII]["FieldValue"], WAmt: hData["WAmt" + sIncrementIII]["FieldValue"],
               MSFAmount: hData["MSF" + sIncrementIII + "_Amount"]["FieldValue"], GSTAmount: hData["GST" + sIncrementIII + "_Amount"]["FieldValue"],

           },
        {

        Prod: hData["Prod" + sIncrementIV]["FieldValue"], ProdQty: hData["ProdQty" + sIncrementIV]["FieldValue"], Prod_Rate: hData["Prod_Rate" + sIncrementIV]["FieldValue"], ProdGross: hData["ProdGross" + sIncrementIV]["FieldValue"],
        Cheque_Account: hData["Cheque_Account" + sIncrementIV]["FieldValue"], Cheque_Amt: hData["Cheque_Amt" + sIncrementIV]["FieldValue"],
        Pass_Type: hData["Pass_Type" + sIncrementIV]["FieldValue"],PService_Charge: hData["PService_Charge" + sIncrementIV]["FieldValue"],
        PGST: hData["PGST" + sIncrementIV]["FieldValue"],PDisc: hData["PDisc" + sIncrementIV]["FieldValue"],Pass_Amount: hData["Pass_Amount" + sIncrementIV]["FieldValue"],
        Online_Type: hData["Online_Type" + sIncrementIV]["FieldValue"], Service_Charge: hData["Service_Charge" + sIncrementIV]["FieldValue"],
        OnlineGST: hData["GST" + sIncrementIV]["FieldValue"], Online_Amt: hData["Online_Amt" + sIncrementIV]["FieldValue"],

        EDC_Batch_ID: hData["EDC_Batch_ID" + sIncrementIV]["FieldValue"], MSF: hData["MSF" + sIncrementIV]["FieldValue"],
        CardGST: hData["CRDGST" + sIncrementIV]["FieldValue"], Amt_Processed: hData["Amt_Processed" + sIncrementIV]["FieldValue"],
        WType: hData["WType" + sIncrementIV]["FieldValue"], WSer_Charge: hData["WSer_Charge" + sIncrementIV]["FieldValue"],
        WGST: hData["WGST" + sIncrementIV]["FieldValue"], WAmt: hData["WAmt" + sIncrementIV]["FieldValue"],
        MSFAmount: hData["MSF" + sIncrementIV + "_Amount"]["FieldValue"], GSTAmount: hData["GST" + sIncrementIV + "_Amount"]["FieldValue"],

        },
        {

            Prod: hData["Prod" + sIncrementV]["FieldValue"], ProdQty: hData["ProdQty" + sIncrementV]["FieldValue"], Prod_Rate: hData["Prod_Rate" + sIncrementV]["FieldValue"], ProdGross: hData["ProdGross" + sIncrementV]["FieldValue"],
            Cheque_Account: hData["Cheque_Account" + sIncrementV]["FieldValue"], Cheque_Amt: hData["Cheque_Amt" + sIncrementV]["FieldValue"],
            Pass_Type: hData["Pass_Type" + sIncrementV]["FieldValue"], PService_Charge: hData["PService_Charge" + sIncrementV]["FieldValue"],
            PGST: hData["PGST" + sIncrementV]["FieldValue"], PDisc: hData["PDisc" + sIncrementV]["FieldValue"], Pass_Amount: hData["Pass_Amount" + sIncrementV]["FieldValue"],
            Online_Type: hData["Online_Type" + sIncrementV]["FieldValue"], Service_Charge: hData["Service_Charge" + sIncrementV]["FieldValue"],
            OnlineGST: hData["GST" + sIncrementV]["FieldValue"], Online_Amt: hData["Online_Amt" + sIncrementV]["FieldValue"],

            EDC_Batch_ID: hData["EDC_Batch_ID" + sIncrementV]["FieldValue"], MSF: hData["MSF" + sIncrementV]["FieldValue"],
            CardGST: hData["CRDGST" + sIncrementV]["FieldValue"], Amt_Processed: hData["Amt_Processed" + sIncrementV]["FieldValue"],
            WType: hData["WType" + sIncrementV]["FieldValue"], WSer_Charge: hData["WSer_Charge" + sIncrementV]["FieldValue"],
            WGST: hData["WGST" + sIncrementV]["FieldValue"], WAmt: hData["WAmt" + sIncrementV]["FieldValue"],
            MSFAmount: hData["MSF" + sIncrementV + "_Amount"]["FieldValue"], GSTAmount: hData["GST" + sIncrementV + "_Amount"]["FieldValue"],

        }
        ];
        
        docNo = hData["DocNo"]["FieldValue"];
        Finance_Option = hData["Finance_Option"]["FieldValue"];
        CheckbuyBack = hData["Buy_Back"]["FieldValue"];
       // AUTO_JVNO = hData["AUTO_JVNO"]["FieldValue"];
        console.log(docNo)
        var data = {
            CompanyId: logDetails.CompanyId,
            SessionId: logDetails.SessionId,
            LoginId: logDetails.LoginId,
            vtype: logDetails.iVoucherType,
            iFATag: logDetails.iFATag,
            iInvTag:logDetails.iInvTag,
            docNo: docNo,
            Finance_Option: Finance_Option,
            receipt: receipt,
            HBuyBack: HBuyBack,
            Buy_Back: CheckbuyBack,
     BalanceRemarks,BalanceRemarks
        };
        $.ajax({
            type: "POST",
            url: baseUrl + "/RaiseFinanceJV/RaiseJV",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: false,
            //dataType: "json",
            success: function (data1) {
                debugger
                //alert("Data Posted Succesfully")
                if (data1.status) {
                    alert(data1.data.succes);             
                }

                else {
                    alert(data1.data.succes);

                }
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            },
            error: function (e) {
                console.log(e.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            }

        });
    } catch (e) {
        console.log("bodyCallbackAfterSaves", e.message)
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
    }

}



function toNumber(value) {
    if (!value) {
        return 0;
    }
    if (!isNaN(value)) {
        return value ? parseFloat(value) : 0;
    }
    return parseFloat(value.split(',').join(''));
}


function initializeFields(fields) {
    let row = {};
    Object.values(fields).forEach((v, i, a) => {
        if (v) {

            row[`${v['sFieldName']}`] = {};
            row[`${v['sFieldName']}`]['sFieldName'] = v['sFieldName'];
            row[`${v['sFieldName']}`]['FieldText'] = v['FieldText'];
            row[`${v['sFieldName']}`]['FieldValue'] = v['FieldValue'];
            row[`${v['sFieldName']}`]['iFieldId'] = v['iFieldId'];
        }
    });
    return row;
}
function initializeRowCallback(response) {
    try {
        debugger
        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        initializeFields(response.data);
        alert(10)

        console.log(response.data.Prod_RateI == 0)
        var prod1 = response.data.prod1;
        var prod2 = response.data.ProdQtyI;
        alert(prod1)
        //if (rows.length === validRows) {

        //    const isValidRows = rows.every(_=> _["MIN_No"].FieldValue && _["MIIR_No"].FieldValue)
        //    if (!isValidRows) {
        //        alert("MIIR No  is empty, Please check");
        //        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
        //    } else {
        //        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
        //    }
        //}
    } catch (e) {
        console.log("initializeRowCallback", e.message)
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
    }
}
function initializeRowDataFields(fields) {
    debugger;
    try {
        var rowData = {};
        Object.values(fields).forEach((v, i, a) => {
            if (v) {
                rowData[`${v['sFieldName']}`] = {};
                rowData[`${v['sFieldName']}`]['sFieldName'] = v['sFieldName'];
                rowData[`${v['sFieldName']}`]['FieldText'] = v['FieldText'];
                rowData[`${v['sFieldName']}`]['FieldValue'] = v['FieldValue'];
                rowData[`${v['sFieldName']}`]['iFieldId'] = v['iFieldId'];
            }
        });
        rows.push(rowData);
    } catch (e) {

    }

    //console.log('rowData  string:: ', JSON.stringify(rowData));
    //console.log('rowData :: ', rowData)
}

function OldbodyCallbackAfterSaves(response) {

    try {
        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        //hData = initializeFields(response.data.slice(1));

        BalanceRemarks = hData["Balance_Remarks"]["FieldValue"];

        Exchange = response.data[0].FieldValue;
        Cheque = response.data[1].FieldValue;
        Card = response.data[2].FieldValue;
        Pass = response.data[3].FieldValue;
        Online = response.data[4].FieldValue;
        Finance = response.data[5].FieldValue;
        Wallet = response.data[6].FieldValue;
        ApprovedFinance = response.data[7].FieldValue;
        Cash = response.data[8].FieldValue;

        console.log(Exchange);
        HBuyBack = [{
            Exchange: response.data[0].FieldValue, Cheque: response.data[1].FieldValue,

            Card: response.data[2].FieldValue, Pass: response.data[3].FieldValue,

            Online: response.data[4].FieldValue, Finance: response.data[5].FieldValue,

            Wallet: response.data[6].FieldValue, ApprovedFinance: response.data[7].FieldValue,
            Cash: response.data[8].FieldValue

        }
        ];
        var url = baseUrl + `/RaiseFinanceJV/ModeofRemarks?CompanyId=${logDetails.CompanyId}&SessionId=${logDetails.SessionId}&LoginId=${logDetails.LoginId}&vtype=${logDetails.iVoucherType}&docNo=${docNo}&BalanceRemarks=${BalanceRemarks}
        &iFATag=${logDetails.iFATag}&iInvTag=${logDetails.iInvTag}&Exchange=${Exchange}&Cheque=${Cheque}&Card=${Card}&Pass=${Pass}&Online=${Online}&Wallet=${Wallet}&Finance=${Finance}`;
        console.log({ logDetails, docNo, url })
        Focus8WAPI.openPopup(url, openPopupCallBackBeforeSave);

    } catch (e) {
        console.log("bodyCallbackAfterSaves", e.message)
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
    }

}


//Generalized AfterSave UpdateGRNJWMlt_AnexJWPur_GRNJWSTK Add By Shaikh Azhar 26-03-2023 
function UpdGRNJWMlt_AnexJWPur_GRNJWSTK(logDetails, rowIndex) {
    //alert(loginDetails);	
    debugger;
    requestId = 0;
    requestsProcessed = [];
    CID = logDetails.CompanyId;
    SessID = logDetails.SessionId;
    UserName = logDetails.UserName;
    LoginId = logDetails.LoginId;
    
    Focus8WAPI.getFieldValue("fieldValueCallbackIssueGRNJWMlt_AnexJWPur_GRNJWSTK", ["DocNo", "Date"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId++);

}
function fieldValueCallbackIssueGRNJWMlt_AnexJWPur_GRNJWSTK(response) {
    debugger;
    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        //loginDetails = response.data[0];
        //VariantID = response.data[0].FieldValue;
        docNo = response.data[0].FieldValue;
        docDate = response.data[1].FieldValue;
        

        var data = {
            CompanyId: CID,
            SessionId: SessID,
            User: UserName,
            LoginId: LoginId,
            vtype: 1299,
            docNo: docNo,
            //iVariantID: VariantID,
            BodyData: bodyData

        };

        console.log({ data })
        $.ajax({
            type: "POST",
            url: "/ICSF9/Generalized_GRNJWMulti_AnnexJWPur_GRNJWSTK/UpdateGRNJWMulti_AnnexJWPur_GRNJWSTK",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (r) {
                debugger
                res = JSON.stringify(r);
                if (Number(r.data.message.localeCompare("Stock Not Sufficent"))== 0)
                {                   
                    //Convert Byte Array to BLOB.
                    var blob = new Blob([r.data.text_data], { type: "application/octetstream" });


                    //Check the Browser type and download the File.
                    var isIE = false || !!document.documentMode;
                    if (isIE) {
                        window.navigator.msSaveBlob(blob, docNo + "-" + new Date().getHours()+ "-" +  new Date().getMinutes() + ".csv");
                    } else {
                        var url = window.URL || window.webkitURL;
                        link = url.createObjectURL(blob);
                        var a = $("<a />");
                        a.attr("download", docNo + "-" + new Date().getHours() + "-" + new Date().getMinutes() + ".csv");
                        a.attr("href", link);
                        $("body").append(a);
                        a[0].click();
                        $("body").remove(a);
                    }
                }
                alert(r.data.message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            },
            error: function (e) {
                console.log(e.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            }

        });


    } catch (e) {
        console.log("fieldValueCallbacksupplierValidate", e.message)
    }
}

function Base64ToBytes(base64) {
    var s = window.atob(base64);
    var bytes = new Uint8Array(s.length);
    for (var i = 0; i < s.length; i++) {
        bytes[i] = s.charCodeAt(i);
    }
    return bytes;
};

//CreateDocToProduct Add By Rakesh 18-08-2022 Function
function CreateDocToProduct(loginDetails, rowIndex) {
    //alert(loginDetails);	
    debugger;
    requestId = 0;
    requestsProcessed = [];
    Focus8WAPI.getFieldValue("fieldValueCallbackItemGeneralize", ["", "*"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId++);

}
function fieldValueCallbackItemGeneralize(response) {
    debugger;
    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        logDetails = response.data[0];
        docNo = response.data[1].FieldValue;
        docDate = response.data[2].FieldValue;


        var data = {
            CompanyId: logDetails.CompanyId,
            SessionId: logDetails.SessionId,
            User: logDetails.UserName,
            LoginId: logDetails.LoginId,
            vtype: logDetails.iVoucherType,
            docNo: docNo,
            BodyData: bodyData

        };

        console.log({ data })
        $.ajax({
            type: "POST",
            url: "/ICSF9/Generalized/GenerilizedAfterSave",

            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (r) {
                debugger
                res = JSON.stringify(r);
                alert(r.data.message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            },
            error: function (e) {
                console.log(e.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            }

        });


        //Focus8WAPI.getBodyFieldValue("PEbodyCallbackAfterSave", ["Item", "Quantity", "Rate", "Gross", "PartyBatchNo", "ItemRemarks"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, requestId++)
        //Focus8WAPI.getBodyFieldValue("PEbodyCallbackAfterSave", ["Item", "Quantity", "Rate", "Gross", "PartyBatchNo", "ItemRemarks"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, requestId++)


    } catch (e) {
        console.log("fieldValueCallbacksupplierValidate", e.message)
    }
}

//CreateDocToAccount Add By Rakesh 21-08-2022 Function
function CreateDocToAccount(loginDetails, rowIndex) {
    //alert(loginDetails);	
    debugger;
    requestId = 0;
    requestsProcessed = [];
    Focus8WAPI.getFieldValue("fieldValueCallbackAccountGeneralize", ["", "*"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId++);

}
function fieldValueCallbackAccountGeneralize(response) {
    debugger;
    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        logDetails = response.data[0];
        docNo = response.data[1].FieldValue;
        docDate = response.data[2].FieldValue;


        var data = {
            CompanyId: logDetails.CompanyId,
            SessionId: logDetails.SessionId,
            User: logDetails.UserName,
            LoginId: logDetails.LoginId,
            vtype: logDetails.iVoucherType,
            docNo: docNo,
            BodyData: bodyData

        };

        console.log({ data })
        $.ajax({
            type: "POST",
            url: "/ICSF9/Generalized/Generilized_Account_AfterSave",

            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (r) {
                debugger
                res = JSON.stringify(r);
                alert(r.data.message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            },
            error: function (e) {
                console.log(e.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            }

        });


        //Focus8WAPI.getBodyFieldValue("PEbodyCallbackAfterSave", ["Item", "Quantity", "Rate", "Gross", "PartyBatchNo", "ItemRemarks"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, requestId++)
        //Focus8WAPI.getBodyFieldValue("PEbodyCallbackAfterSave", ["Item", "Quantity", "Rate", "Gross", "PartyBatchNo", "ItemRemarks"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, requestId++)


    } catch (e) {
        console.log("fieldValueCallbacksupplierValidate", e.message)
    }
}


//Create Dynamic Master Add By Rakesh 14-09-2022 Function
function CreateDynamicMaster(loginDetails, rowIndex) {
    //alert(loginDetails);	
    debugger;
    requestId = 0;
    requestsProcessed = [];
    Focus8WAPI.getFieldValue("fieldValueCallbackDynamicMaster", ["", "*"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId++);

}
function fieldValueCallbackDynamicMaster(response) {
    debugger;
    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        logDetails = response.data[0];
        docNo = response.data[1].FieldValue;
        docDate = response.data[2].FieldValue;


        var data = {
            CompanyId: logDetails.CompanyId,
            SessionId: logDetails.SessionId,
            User: logDetails.UserName,
            LoginId: logDetails.LoginId,
            vtype: logDetails.iVoucherType,
            docNo: docNo,
            BodyData: bodyData

        };

        console.log({ data })
        $.ajax({
            type: "POST",
            url: "/ICSF9/GAS/GenerilizedMasterAfterSave",

            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (r) {
                debugger
                res = JSON.stringify(r);
                alert(r.data.message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            },
            error: function (e) {
                console.log(e.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            }

        });


        //Focus8WAPI.getBodyFieldValue("PEbodyCallbackAfterSave", ["Item", "Quantity", "Rate", "Gross", "PartyBatchNo", "ItemRemarks"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, requestId++)
        //Focus8WAPI.getBodyFieldValue("PEbodyCallbackAfterSave", ["Item", "Quantity", "Rate", "Gross", "PartyBatchNo", "ItemRemarks"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, requestId++)


    } catch (e) {
        console.log("fieldValueCallbacksupplierValidate", e.message)
    }
}




//DocHead2Document Add By Rizwan 18-08-2022 Function
function DocHead2Document(loginDetails, rowIndex) {
    //alert(loginDetails);	
    debugger;
    requestId = 0;
    requestsProcessed = [];
    Focus8WAPI.getFieldValue("fieldValueCallbackIssueAfterSaveGRNImp", ["", "DocNo", "Date", "sNarration", "Vendor Account"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId++);

}
function fieldValueCallbackIssueAfterSaveGRNImp(response) {
    debugger;
    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        logDetails = response.data[0];
        docNo = response.data[1].FieldValue;
        docDate = response.data[2].FieldValue;


        var data = {
            CompanyId: logDetails.CompanyId,
            SessionId: logDetails.SessionId,
            User: logDetails.UserName,
            LoginId: logDetails.LoginId,
            vtype: logDetails.iVoucherType,
            docNo: docNo,
            BodyData: bodyData

        };

        console.log({ data })
        $.ajax({
            type: "POST",
            url: "/ICSF9/GeneralAfterSave/UpdateDocument",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (r) {
                debugger
                res = JSON.stringify(r);
                alert(r.data.message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            },
            error: function (e) {
                console.log(e.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            }

        });


    } catch (e) {
        console.log("fieldValueCallbacksupplierValidate", e.message)
    }
}

//Generalized AfterSave Master Add By Shaikh Azhar 09-08-2022 
function GASUpdateMaster(loginDetails, rowIndex) {
    //alert(loginDetails);	
    debugger;
    requestId = 0;
    requestsProcessed = [];
    Focus8WAPI.getFieldValue("fieldValueCallbackIssueGASUpdateMaster", ["DocNo", "Date"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId++);

}
function fieldValueCallbackIssueGASUpdateMaster(response) {
    debugger;
    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        logDetails = response.data[0];
        docNo = response.data[1].FieldValue;
        docDate = response.data[2].FieldValue;


        var data = {
            CompanyId: logDetails.CompanyId,
            SessionId: logDetails.SessionId,
            User: logDetails.UserName,
            LoginId: logDetails.LoginId,
            vtype: logDetails.iVoucherType,
            docNo: docNo,
            BodyData: bodyData

        };

        console.log({ data })
        $.ajax({
            type: "POST",
            url: "/ICSF9/GASMaster/UpdateMaster",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (r) {
                debugger
                res = JSON.stringify(r);
                alert(r.data.message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            },
            error: function (e) {
                console.log(e.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            }

        });


    } catch (e) {
        console.log("fieldValueCallbacksupplierValidate", e.message)
    }
}


//Generalized AfterSave I2P_R2P Add By Shaikh Azhar 27-02-2023 
function UpdateI2P_R2P(loginDetails, rowIndex) {
    //alert(loginDetails);	
    debugger;
    requestId = 0;
    requestsProcessed = [];
    CID = loginDetails.CompanyId;
    SessID = loginDetails.SessionId;
    UserName = loginDetails.UserName;
    LoginId = loginDetails.LoginId;
    //vtyp=logDetails.iVoucherType;
    Focus8WAPI.getFieldValue("fieldValueCallbackIssueI2P_R2P", ["VariantID", "DocNo", "Date"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId++);

}
function fieldValueCallbackIssueI2P_R2P(response) {
    debugger;
    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        logDetails = response.data[0];
        VariantID = response.data[0].FieldValue;
        docNo = response.data[1].FieldValue;
        
        ////docDate = response.data[2].FieldValue;


        var data = {
            CompanyId: CID,
            SessionId: SessID,
            User: UserName,
            LoginId: LoginId,
            vtype: 6656,
            docNo: docNo,
            iVariantID: VariantID,
            BodyData: bodyData

        };

        console.log({ data })
        $.ajax({
            type: "POST",
            url: "/ICSF9/Generalized_I2P_R2P/UpdateI2P_R2P",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (r) {
                debugger
                res = JSON.stringify(r);
                alert(r.data.message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            },
            error: function (e) {
                console.log(e.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            }

        });


    } catch (e) {
        console.log("fieldValueCallbacksupplierValidate", e.message)
    }
}
//Generalized AfterSave PPCPlan_PPSfgReqd Add By Shaikh Azhar 09-03-2023 
function PPCPlan_PPSfgReqd(loginDetails, rowIndex) {
    debugger;
    requestId = 0;
    requestsProcessed = [];
    CID = loginDetails.CompanyId;
    SessID = loginDetails.SessionId;
    UserName = loginDetails.UserName;
    LoginId = loginDetails.LoginId;
    
    Focus8WAPI.getFieldValue("fieldValueCallbackIssuePPCPlan_PPSfgReqd", ["DocNo", "Date"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId++);

}
function fieldValueCallbackIssuePPCPlan_PPSfgReqd(response) {
    debugger;
    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        logDetails = response.data[0];

        docNo = response.data[0].FieldValue;

        ////docDate = response.data[2].FieldValue;


        var data = {
            CompanyId: CID,
            SessionId: SessID,
            User: UserName,
            LoginId: LoginId,
            vtype: 7946,
            docNo: docNo,
            BodyData: bodyData

        };

        console.log({ data })
        $.ajax({
            type: "POST",
            url: "/ICSF9/Generalized_PPCPlan_PPSfgReqd/UpdatePPCPlan_PPSfgReqd",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (r) {
                debugger
                res = JSON.stringify(r);
                alert(r.data.message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            },
            error: function (e) {
                console.log(e.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            }

        });


    } catch (e) {
        console.log("fieldValueCallbacksupplierValidate", e.message)
    }
}
function RelPO_POS(loginDetails, rowIndex) {
    debugger;
    requestId = 0;
    requestsProcessed = [];
    CID = loginDetails.CompanyId;
    SessID = loginDetails.SessionId;
    UserName = loginDetails.UserName;
    LoginId = loginDetails.LoginId;

    Focus8WAPI.getFieldValue("fieldValueCallbackIssueRelPO_POS", ["DocNo", "Date"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId++);

}
function fieldValueCallbackIssueRelPO_POS(response) {
    debugger;
    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        logDetails = response.data[0];

        docNo = response.data[0].FieldValue;

        ////docDate = response.data[2].FieldValue;


        var data = {
            CompanyId: CID,
            SessionId: SessID,
            User: UserName,
            LoginId: LoginId,
            vtype: 7953,
            docNo: docNo,
            BodyData: bodyData

        };

        console.log({ data })
        $.ajax({
            type: "POST",
            url: "/ICSF9/Generalized_RelPO_POS/UpdateRelPO_POS",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (r) {
                debugger
                res = JSON.stringify(r);
                alert(r.data.message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            },
            error: function (e) {
                console.log(e.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            }

        });


    } catch (e) {
        console.log("fieldValueCallbacksupplierValidate", e.message)
    }
}
