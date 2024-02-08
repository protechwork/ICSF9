//#region varaiables 
var requestId = 1;
var CID = "";
var CID = ''; var SessID = ''; var UserName = '';
var logindetails = {};
var logDetails = {};
var ivouchertype = "0";
var headerarr = [];
var list = [];
var flag = 0;
var table = [];
var table1 = [];
var b_ArrRequest = [];
var ArrResponse = [];
var compid = [];
var vtype = [];
var row = 0;
var LoginId = 0;
var DocNo = 0;
var docDate = 0;
var ibodyID = 0;
var VendorAC = 0;
var Branch = 0;
var Vendor = 0;
var Dept = 0;
var WorksCenter = 0;
var POAbbrList="";
var Warehouse = 0;
var lstRef =0;
var requestsProcessed = [];
//var requestsProcessed = [];
var sName = "";
var sCode = "";
var Item_d = 0;

var Focus8WAPI = {
    ENUMS: {
        MODULE_TYPE: {
            MASTER: 1,
            TRANSACTION: 2,
            UI: 3,
            GLOBAL: 4,
            MRP: 5,
            FixedAsset: 6,
            TransHome: 7
        },

        REQUEST_TYPE: {
            GET: 1,
            SET: 2,
            CONTINUE: 3,
            RESET_CACHE: 4
        },

        REQUEST_TYPE_UI: {
            SET_POPUP_COORDINATE: 1,
            OPEN_POPUP: 2,
            CLOSE_POPUP: 3,
            GOTOHOMEPAGE: 4,
            OPEN_INVOICE_DESIGNER: 5,
            AWAKE_SESSION: 6,
            LOGOUT: 7,
            MANDATORY_FIELDS_ENTRYSCREEN: 8
        }
    },

    getFieldValue: function (sCallbackFn, Field, iModuleType, isFieldId, iRequestId, bStruct) {
        var obj = null;

        try {
            obj = {
                moduleType: iModuleType,
                rowIndex: 0,
                isFieldId: isFieldId,
                requestType: Focus8WAPI.ENUMS.REQUEST_TYPE.GET,
                objData: { fieldid: Field },
                iRequestId: iRequestId,
                sCallbackFn: sCallbackFn,
                bStruct: bStruct
            };

            if (Focus8WAPI.PRIVATE.isValidInput(obj, false) == true) {
                Focus8WAPI.PRIVATE.postMessage(obj);
            }
        }
        catch (err) {
            alert("Exception: Focus8WAPI.getFieldValue " + err.message);
        }
    },

    setFieldValue: function (sCallbackFn, Field, Value, iModuleType, isFieldId, iRequestId, bStruct) {
        var obj = null;

        try {
            obj = {
                moduleType: iModuleType,
                rowIndex: 0,
                isFieldId: isFieldId,
                requestType: Focus8WAPI.ENUMS.REQUEST_TYPE.SET,
                objData: { fieldid: Field, value: Value },
                iRequestId: iRequestId,
                sCallbackFn: sCallbackFn,
                bStruct: bStruct
            };

            if (Focus8WAPI.PRIVATE.isValidInput(obj, false) == true) {
                Focus8WAPI.PRIVATE.postMessage(obj);
            }
        }
        catch (err) {
            alert("Exception: Focus8WAPI.setFieldValue " + err.message);
        }
    },

    getBodyFieldValue: function (sCallbackFn, Field, iModuleType, isFieldId, iRowIndex, iRequestId, bStruct) {
        var obj = null;

        try {
            obj = {
                moduleType: iModuleType,
                rowIndex: iRowIndex,
                isFieldId: isFieldId,
                requestType: Focus8WAPI.ENUMS.REQUEST_TYPE.GET,
                objData: { fieldid: Field },
                iRequestId: iRequestId,
                sCallbackFn: sCallbackFn,
                bStruct: bStruct
            };

            if (Focus8WAPI.PRIVATE.isValidInput(obj, true) == true) {
                Focus8WAPI.PRIVATE.postMessage(obj);
            }
        }
        catch (err) {
            alert("Exception: Focus8WAPI.getBodyFieldValue " + err.message);
        }
    },

    setBodyFieldValue: function (sCallbackFn, Field, Value, iModuleType, isFieldId, iRowIndex, iRequestId, bStruct) {
        var obj = null;

        try {
            obj = {
                moduleType: iModuleType,
                rowIndex: iRowIndex,
                isFieldId: isFieldId,
                requestType: Focus8WAPI.ENUMS.REQUEST_TYPE.SET,
                objData: { fieldid: Field, value: Value },
                iRequestId: iRequestId,
                sCallbackFn: sCallbackFn,
                bStruct: bStruct
            };

            if (Focus8WAPI.PRIVATE.isValidInput(obj, true) == true) {
                Focus8WAPI.PRIVATE.postMessage(obj);
            }
        }
        catch (err) {
            alert("Exception: Focus8WAPI.setBodyFieldValue " + err.message);
        }
    },

    continueModule: function (iModuleType, result) {
        var obj = null;

        try {
            obj = {};
            obj.moduleType = iModuleType;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE.CONTINUE;
            obj.result = result;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.continueModule " + err.message);
        }
    },

    openPopup: function (url, sCallback) {
        var obj = null;

        try {
            if (Focus8WAPI.PRIVATE.isNullOrEmpty(url, true) == true) {
                return (false);
            }

            obj = {};
            obj.URL = url;
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.OPEN_POPUP;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.openPopup " + err.message);
        }

        return (true);
    },

    closePopup: function () {
        var obj = null;

        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.CLOSE_POPUP;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.closePopup " + err.message);
        }
    },

    gotoHomePage: function () {
        var obj = null;

        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.GOTOHOMEPAGE;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.gotoHomePage " + err.message);
        }
    },

    logout: function () {
        var obj = null;

        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.LOGOUT;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.logout " + err.message);
        }
    },

    awakeSession: function () {
        var obj = null;

        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.AWAKE_SESSION;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.awakeSession " + err.message);
        }
    },
    getMandatoryFields: function (sCallback, iMasterTypeId) {
        var obj = null;
        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.MASTER;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.MANDATORY_FIELDS_ENTRYSCREEN;
            obj.sCallbackFn = sCallback;
            obj.objData = iMasterTypeId;
            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.getMandatoryFields " + err.message);
        }
    },

    resetTransactionCache: function (iVoucherType) {
        var obj = null;

        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE.RESET_CACHE;
            obj.iVoucherType = iVoucherType;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.awakeSession " + err.message);
        }
    },

    setPopupCoordinates: function (sLeft, sTop, sWidth, sHeight) {
        var obj = null;
        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.SET_POPUP_COORDINATE;
            obj.Left = sLeft;
            obj.Top = sTop;
            obj.Width = sWidth;
            obj.Height = sHeight;
            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.openPopup " + err.message);
        }

        return (true);
    },

    getGlobalValue: function (sCallbackFn, sVariable, iRequestId) {
        var obj = null;

        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.GLOBAL;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE.GET;
            obj.Variable = sVariable;
            obj.iRequestId = iRequestId;
            obj.sCallbackFn = sCallbackFn;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.getGlobalValue " + err.message);
        }
    },

    openInvoiceDesigner: function (sCallbackFn, LayoutId, iVouchertype, iHeaderId, eModuleType, HeaderGroup, iSubReportId, bSaveHTMLSource, iRequestId) {
        var obj = null;
        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.OPEN_INVOICE_DESIGNER;
            obj.LayoutId = LayoutId;
            obj.iVouchertype = iVouchertype;
            obj.iHeaderId = iHeaderId;
            obj.ModuleType = eModuleType;
            obj.HeaderGroup = HeaderGroup;
            obj.iSubReportId = iSubReportId;
            obj.bSaveHTMLSource = bSaveHTMLSource;
            obj.sCallbackFn = sCallbackFn;
            obj.iRequestId = iRequestId;
            Focus8WAPI.PRIVATE.postMessage(obj);
            return obj;
        }
        catch (err) {
            alert("Exception: Focus8WAPI.openPopup " + err.message);
        }
    },

    PRIVATE: {
        isValidInput: function (obj, bBodyField) {
            try {
                if (Focus8WAPI.PRIVATE.isValidObject(obj.moduleType) == false || obj.moduleType.toString() == "") {
                    alert("Validation Exception: Please pass Module Type parameter");

                    return (false);
                }

                if (Focus8WAPI.PRIVATE.isValidObject(obj.isFieldId) == false || obj.isFieldId.toString() == "") {
                    alert("Validation Exception: Please pass isFieldId parameter");

                    return (false);
                }

                if (Focus8WAPI.PRIVATE.isValidObject(obj.objData.fieldid) == false) {
                    alert("Validation Exception: Please pass Field parameter");

                    return (false);
                }
                else {
                    if (Array.isArray(obj.objData.fieldid) == true) {
                        if (obj.objData.fieldid.length == 0) {
                            alert("Validation Exception: Please pass Field parameter");

                            return (false);
                        }
                    }
                }


                if (bBodyField == true) {
                    if (Focus8WAPI.PRIVATE.isValidObject(obj.rowIndex) == false) {
                        alert("Validation Exception: Row Index should be number type");

                        return (false);
                    }

                    if (Array.isArray(obj.rowIndex) == false) {
                        if (isNaN(obj.rowIndex)) {
                            alert("Validation Exception: Row Index should be number type");

                            return (false);
                        }

                        if (obj.rowIndex == 0) {
                            // alert("Validation Exception: Row Index should be greater than 0 for Body Fields");

                            return (false);
                        }
                    }
                }
            }
            catch (err) {
                alert("Exception: {Focus8WAPI.PRIVATE.isValidInput} " + err.message);
            }

            return (true);
        },

        postMessage: function (obj) {
            try {
                obj.FromClient = true;
                window.parent.postMessage(obj, "*");
            }
            catch (err) {
                // alert("Exception: Focus8WAPI.PRIVATE.postMessage " + err.message);
            }
        },

        onReceiveMessage: function (evt) {
            var objReturnData = null;
            var obj = null;

            try {
                Focus8WAPI.PRIVATE.stopKeyProcess(evt);
                objReturnData = evt.data;

                // Client                
                if (Focus8WAPI.PRIVATE.isValidObject(objReturnData.FromClient) == true) {
                    return;
                }

                console.log('Focus8WAPI::Received Response: ', JSON.stringify(objReturnData));

                if (Focus8WAPI.PRIVATE.isNullOrEmpty(objReturnData.sCallbackFn, true) == false) {
                    obj = {};
                    obj.returnCode = objReturnData.response.lValue;
                    obj.message = objReturnData.response.sValue;
                    obj.data = objReturnData.response.data;
                    obj.fieldId = objReturnData.fieldId;
                    obj.requestType = objReturnData.requestType;
                    obj.moduleType = objReturnData.moduleType;
                    obj.iRequestId = objReturnData.iRequestId;

                    if (Focus8WAPI.PRIVATE.isValidObject(objReturnData.RowsInfo) == true) {
                        obj.RowsInfo = objReturnData.RowsInfo;
                    }

                    eval(objReturnData.sCallbackFn)(obj);
                }
            }
            catch (err) {
                alert("Exception: Focus8WAPI.PRIVATE.onReceiveMessage " + err.message);
            }
        },

        isValidObject: function (obj) {
            try {
                if (typeof obj == "undefined" || obj == null) {
                    return (false);
                }

                return (true);
            }
            catch (err) {
                alert("Exception: {Focus8WAPI.PRIVATE.isValidObject} " + err.message);
            }

            return (false);
        },

        isNullOrEmpty: function (sValue, bTrim) {
            var bResult = false;

            try {
                if (Focus8WAPI.PRIVATE.isValidObject(sValue) == false || (typeof sValue).toLowerCase() != "string" || sValue.length <= 0) {
                    return (true);
                }

                if (Focus8WAPI.PRIVATE.isValidObject(bTrim) == true && bTrim == true) {
                    if (sValue.trim().length == 0) {
                        return (true);
                    }
                }
            }
            catch (err) {
                alert("Exception: {Focus8WAPI.PRIVATE.isNullOrEmpty} " + err.message);
                bResult = true;
            }

            return (bResult);
        },

        stopKeyProcess: function (evt) {
            try {
                if (Focus8WAPI.PRIVATE.isValidObject(evt) == false) {
                    return;
                }

                if (evt.preventDefault) {
                    evt.preventDefault();
                }
                else {
                    evt.returnValue = false;
                }

                if (evt.bubbles == true) {
                    evt.stopPropagation();
                }
            }
            catch (err) {
                alert("Exception: {Focus8WAPI.PRIVATE.stopKeyProcess} " + err.message);
            }
        }
    }

}
window.addEventListener('message', Focus8WAPI.PRIVATE.onReceiveMessage);


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

// Before Save Added Shaikh Azhar Dated 21-02-2022 Batch ROW
function BodyRowNoBS(logDetails, rowIndex) {
    debugger;
    
    if (rowIndex > 0) {
        CID = logDetails.CompanyId;
        row = rowIndex;
        UserName = logDetails.UserName;
        reflist =  "DocNo, Date, HBodyRowID";
        SessID = logDetails.SessionId;  //response.data[0].SessionId
        LoginId = logDetails.LoginId;   //response.data[0].LoginId
        requestsProcessed = [];
        requestId = 0;
    

        Focus8WAPI.getFieldValue("BodyRowNoBSfieldValueCallback", ["DocNo", "Date", "HBodyRowID"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, ++requestId)
    }
}
function BodyRowNoBSfieldValueCallback(response) {
    var strval="";  
    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        //logDetails = response.data[0];
        //console.log(logDetails)
        debugger;
       
        docNo = response.data[0].FieldValue;
        ibodyID = "#" +  response.data[2].FieldValue;
        //docDate = response.data[2].FieldValue;
        $(ibodyID).val(row);
        debugger;
        console.log(docNo)
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
        debugger;
        //debugger;
        
        //if (docNo != "") {
        //    var url = `/ICSF9/BatchCrtnBS/UpdateMaster?CompanyId=${CID}&SessionId=${SessID}&User=${UserName}&LoginId=${LoginId}&DocNo=${docNo}&irow=${row}`;
        //    console.log({ logDetails, docNo, url })
        //    $.ajax({
        //        type: "POST",
        //        url: url,
        //        contentType: "application/json; charset=utf-8",
        //        //   async: false,
        //        success: function (r) {
        //            debugger
        //            res = JSON.stringify(r);
        //            //alert(r.data.succes);
        //            strval=r.data.succes.toString();
        //            //document.getElementById("id_body_16777398").value = res;
        //            $("#id_body_16777398").val(strval);
        //            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
        //        },
        //        error: function (e) {
        //            console.log(e.message)
        //            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
        //        }

        //    });

        //}

    } catch (e) {
        console.log("headerCallback", e.message)
    }
}



// Before Save Added Shaikh Azhar Dated 09-06-2022 PORM
function PORM(logDetails, rowIndex) {
    debugger
    if (rowIndex > 0) {
        CID = logDetails.CompanyId;
        row = rowIndex;
        UserName = logDetails.UserName;

        SessID = logDetails.SessionId;  //response.data[0].SessionId
        LoginId = logDetails.LoginId;   //response.data[0].LoginId
        requestsProcessed = [];
        requestId = 0;
    
        Focus8WAPI.getFieldValue("fieldValueCallbackIssueBS_PORM", ["", "DocNo", "Date", "VendorAC", "Branch", "Dept", "Works Center","POAbbrList"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, ++requestId);
    }
    

}
function fieldValueCallbackIssueBS_PORM(response) {

    docNo = response.data[1].FieldValue;
    docDate = response.data[2].FieldValue;
    VendorAC = response.data[3].FieldValue;
    Branch = response.data[4].FieldValue;
    Dept = response.data[5].FieldValue
    /*WorksCenter = response.data[6].FieldValue;
    POAbbrList = response.data[7].FieldValue;*/
    ivouchertype = response.data[0].iVoucherType;
    Warehouse = 0;
    //Warehouse = response.data[7].FieldValue;
    debugger;
    if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);

    try {

        Focus8WAPI.getBodyFieldValue("getbodyDatacallBackBS_PORM", ["", "*"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, ++requestId);

    } catch (e) {
        console.log("fieldValueCallbacksupplierValidate", e.message)
    }
}
function getbodyDatacallBackBS_PORM(response) {
    debugger;
    if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);
    try {


        // ArrResponse = ArrResponse + 1;

        //docNo = response.data[0].MetaDataHeader[0].iFieldId
        console.log('response:' + response);
        if (row < 0) {
            validRows = response.data[0].RowsInfo.iValidRows
        }
        else {
            validRows = response.RowsInfo.iValidRows;
        }
        table1 = [];

        //alert(validRows)
        //if (validRows > 0) {

            b_ArrRequest = validRows;
            ArrResponse = 0;
            table1 = [];
           // for (var i = 1; i <= validRows; i++) {

                Focus8WAPI.getBodyFieldValue("BodyRowdataSplitrollBS_PORM", ["Tax Code" ,"Item", "Quantity", "Rate", "Gross", "sRemarks"], 2, false, row, ++requestId);
           // }

       // }
       // else {
            //alert('Product Not Available')
         //   Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
       // }
    }
    catch (ex) {
        alert(ex.message);
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
    }

}
function BodyRowdataSplitrollBS_PORM(response) {
    debugger
    //ArrResponse++
    //console.log('getdat:' + response)
    if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);
    Item_d = parseInt(response.data[1].FieldValue);
    table1.push({
        RowNo: row,
        TaxCode: parseInt(response.data[0].FieldValue),
        Item: parseInt(response.data[1].FieldValue),
        Quantity: parseFloat(response.data[2].FieldValue),
        Rate: parseFloat(response.data[3].FieldValue),
        Gross: parseFloat(response.data[4].FieldValue)


    });
    ArrResponse++
    if (row > 0)  {

        var postingData = { CompanyId: CID, SessionId: SessID, User: UserName, LoginId: LoginId, irowNo: row, vtype: ivouchertype, docNo: docNo, docDate: docDate, Branch: Branch, iBookNo: VendorAC, Dept: Dept, WorksCenter: WorksCenter, Warehouse: Warehouse,BodyData: table1 };
        
        requestId = 0;
        console.log(postingData);
        if (Item_d  > 0 )
        {
            
            var url = `/F9ICSKaleGrp_As/PORM/CheckPORM?CompanyId=${CID}&SessionId=${SessID}&User=${UserName}&LoginId=${LoginId}&irowNo=${row}&vtype=${ivouchertype}&docNo=${docNo}&docDate=${docDate}&Branch=${Branch}&iBookNo=${VendorAC}&Dept=${Dept}&WorksCenter=${WorksCenter}&Warehouse=${Warehouse}&Item_d=${Item_d}&POAbbrList=${POAbbrList}&BodyData=${table1}`;
            table1 = [];
            console.log(url);

            Focus8WAPI.openPopup(url, openPopupCallBackBeforeSave);
        }
        else {
            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            
        }
        


        //$.ajax({
        //    type: "POST",
        //    url: "/F9ICSKaleGrp_As/PORM/CheckPORM",
        //    data: JSON.stringify(postingData),
        //    contentType: "application/json; charset=utf-8",
        //    success: function (r) {
        //        debugger;

        //        //res = JSON.stringify(r);
        //        //alert(r.data.message);
        //        //console.log('Resu', Result);
        //        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);

        //    },
        //    error: function (serror) {
        //        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
        //    }
        //})
    }

}





// Before Save Product 
function BSProduct(logDetails, rowIndex) {
    debugger;
    Focus8WAPI.getFieldValue("BSProductPassfieldValueCallback", ["Name", "Code"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, ++requestId)
}
function BSProductPassfieldValueCallback(response) {

    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        logDetails = response.data[0];
        //console.log(logDetails)
        debugger;
        sName = response.data[1].FieldValue;
        sCode = response.data[2].FieldValue;


        console.log(sCode)

        debugger;

        if (sName != "") {
            var url = `/F9ICSKaleGrp_As/Product/Productsave?CompanyId=${logDetails.CompanyId}&User=${logDetails.UserName}&SessionId=${logDetails.SessionId}&LoginId=${logDetails.LoginId}&sName=${sName}`;
            console.log({ logDetails, sName, url })
            $.ajax({
                type: "POST",
                url: url,
                contentType: "application/json; charset=utf-8",
                //   async: false,
                success: function (r) {
                    debugger
                    //res = JSON.stringify(r);
                    //alert("dasda");
                    Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
                },
                error: function (e) {
                    console.log(e.message)
                    Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
                }

            });

        }

    } catch (e) {
        console.log("headerCallback", e.message)
    }
}

// Before Save IssPro
function ProdOrdAutoClose(logDetails, rowIndex) {
    debugger;

    // logDetails = [...args][0];  
    //Focus8WAPI.getFieldValue("PassSfieldValueCallback", ["", "DocNo", "RefList", "Date", "Collection_Account"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, ++requestId)
    Focus8WAPI.getFieldValue("ProdOrdAutoClosePassfieldValueCallback", ["", "Date"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, ++requestId)
}
function ProdOrdAutoClosePassfieldValueCallback(response) {

    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        logDetails = response.data[0];
        //console.log(logDetails)
        debugger;
        idocDate = response.data[1].FieldValue;



        console.log(idocDate)

        debugger;

        //Focus8WAPI.getBodyFieldValue("PassSbodyCallbackBeforeSave", [""], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, ++requestId)
        if (idocDate != "") {
            var url = `/F9ICSKaleGrp_As/IssPro/ProdOrdAutoClose?CompanyId=${logDetails.CompanyId}&User=${logDetails.UserName}&SessionId=${logDetails.SessionId}&LoginId=${logDetails.LoginId}&vtype=${logDetails.iVoucherType}&idocDate=${idocDate}`;
            console.log({ logDetails, idocDate, url })
            //Focus8WAPI.openPopup(url, openPopupCallBackBeforeSave);
            $.ajax({
                type: "POST",
                url: url,
                contentType: "application/json; charset=utf-8",
                //   async: false,
                success: function (r) {
                    debugger
                    //res = JSON.stringify(r);
                    //alert("dasda");
                    Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
                },
                error: function (e) {
                    console.log(e.message)
                    Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
                }

            });

        }

    } catch (e) {
        console.log("headerCallback", e.message)
    }
}

// Before Save Gin
function GtEntRfPLst(logDetails, rowIndex) {
    debugger;

    // logDetails = [...args][0];  
    //Focus8WAPI.getFieldValue("PassSfieldValueCallback", ["", "DocNo", "RefList", "Date", "Collection_Account"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, ++requestId)
    Focus8WAPI.getFieldValue("GtEntRfPLstPassfieldValueCallback", ["", "DocNo", "Date", "VendorAC", "Branch", "RefList"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, ++requestId)
}
function GtEntRfPLstPassfieldValueCallback(response) {

    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        logDetails = response.data[0];
        //console.log(logDetails)
        debugger;
        docNo = response.data[1].FieldValue;
        idocDate = response.data[2].FieldValue;
        Vendor = response.data[3].FieldValue;
        Branch = response.data[4].FieldValue;
        reflist = response.data[5].FieldValue;


        console.log(docNo)

        debugger;

        //Focus8WAPI.getBodyFieldValue("PassSbodyCallbackBeforeSave", [""], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, ++requestId)
        if (reflist != "") {
            var url = `/F9ICSKaleGrp_As/GtEntRfPndngLst/GtEntRfPLst?CompanyId=${logDetails.CompanyId}&Branch=${Branch}&Vendor=${Vendor}&User=${logDetails.UserName}&SessionId=${logDetails.SessionId}&LoginId=${logDetails.LoginId}&vtype=${logDetails.iVoucherType}&docNo=${docNo}&idocDate=${idocDate}&refLst=${reflist}`;
            console.log({ logDetails, docNo, url })
            //Focus8WAPI.openPopup(url, openPopupCallBackBeforeSave);
            $.ajax({
                type: "POST",
                url: url,
                contentType: "application/json; charset=utf-8",
                //   async: false,
                success: function (r) {
                    debugger
                    //res = JSON.stringify(r);
                    //alert("dasda");
                    Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
                },
                error: function (e) {
                    console.log(e.message)
                    Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
                }

            });

        }

    } catch (e) {
        console.log("headerCallback", e.message)
    }
}

//before save GRNJWPur
function BSGRNJWPur(logDetails, rowIndex) {
    debugger

    CID = logDetails.CompanyId;
    row = rowIndex;
    UserName = logDetails.UserName;
    
    SessID = logDetails.SessionId;  //response.data[0].SessionId
    LoginId = logDetails.LoginId;   //response.data[0].LoginId
    requestsProcessed = [];
    requestId = 0;
    Focus8WAPI.getFieldValue("fieldValueCallbackIssueBS_GRNJWPur", ["", "DocNo", "Date", "VendorAC", "Branch", "Dept", "Works Center"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, ++requestId);

}
function fieldValueCallbackIssueBS_GRNJWPur(response) {

    docNo = response.data[1].FieldValue;
    docDate = response.data[2].FieldValue;
    VendorAC = response.data[3].FieldValue;
    Branch = response.data[4].FieldValue;
    Dept = response.data[5].FieldValue
    WorksCenter = response.data[6].FieldValue;
    ivouchertype = response.data[0].iVoucherType;
    Warehouse = 0;
    //Warehouse = response.data[7].FieldValue;
    debugger;
    if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);

    try {

        Focus8WAPI.getBodyFieldValue("getbodyDatacallBackBS_GRNJWPur", ["", "*"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, ++requestId);

    } catch (e) {
        console.log("fieldValueCallbacksupplierValidate", e.message)
    }
}
function getbodyDatacallBackBS_GRNJWPur(response) {
    debugger;
    if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);
    try {


        // ArrResponse = ArrResponse + 1;

        //docNo = response.data[0].MetaDataHeader[0].iFieldId
        console.log('response:' + response);
        if (row < 0) {
            validRows = response.data[0].RowsInfo.iValidRows
        }
        else {
            validRows = response.RowsInfo.iValidRows;
        }
        table1 = [];

        //alert(validRows)
        if (validRows > 0) {

            b_ArrRequest = validRows;
            ArrResponse = 0;
            table1 = [];
            for (var i = 1; i <= validRows; i++) {

                Focus8WAPI.getBodyFieldValue("BodyRowdataSplitrollBS_GRNJWPur", ["Process", "Item", "Quantity", "Rate", "Gross", "Discount", "sRemarks"], 2, false, i, ++requestId);
            }

        }
        else {
            //alert('Product Not Available')
            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
        }
    }
    catch (ex) {
        alert(ex.message);
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
    }

}
function BodyRowdataSplitrollBS_GRNJWPur(response) {
    debugger
    //ArrResponse++
    //console.log('getdat:' + response)
    if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);

    table1.push({
        RowNo: parseInt(ArrResponse),
        Item: parseInt(response.data[1].FieldValue),
        Quantity: parseFloat(response.data[2].FieldValue),
        Rate: parseFloat(response.data[3].FieldValue),
        Gross: parseFloat(response.data[4].FieldValue)


    });
    ArrResponse++
    if (ArrResponse == validRows) {
        var postingData = { CompanyId: CID, SessionId: SessID, User: UserName, LoginId: LoginId, irowNo: row, vtype: ivouchertype, docNo: docNo, docDate: docDate, Branch: Branch, iBookNo: VendorAC, Dept: Dept, WorksCenter: WorksCenter, Warehouse: Warehouse, BodyData: table1 };
        
        requestId = 0;
        console.log(postingData);
        $.ajax({
            type: "POST",
            url: "/F9ICSKaleGrp_As/GRNJW/CheckFGQty",
            data: JSON.stringify(postingData),
            contentType: "application/json; charset=utf-8",
            success: function (r) {
                debugger;

                //res = JSON.stringify(r);
                //alert(r.data.message);
                //console.log('Resu', Result);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);

            },
            error: function (serror) {
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            }
        })

        //if (row > 0 ) {
        //    var url = `/F9ICSKaleGrp_As/PORM/CheckPORM?CompanyId: ${CID}, SessionId: ${SessID}, User: ${UserName}, LoginId: ${LoginId}, irowNo: ${row}, vtype: ${ivouchertype}, docNo: ${docNo}, docDate: ${docDate}, Branch: ${Branch}, iBookNo: ${VendorAC}, Dept: ${Dept}, WorksCenter: ${WorksCenter}, Warehouse: ${Warehouse}, BodyData: ${table1}`;
        //    table1 = [];
        //    console.log({row, url })
        //    Focus8WAPI.openPopup(url, openPopupCallBackBeforeSave);
        //}
        //else {
        //    Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);

        //}

    }

}

// 19-01-2022 added by Shaikh Azhar
//before save DCJWSal
function BSDCJWSal(logDetails, rowIndex) {
    debugger
    CID = logDetails.CompanyId;
    row = rowIndex;
    UserName = logDetails.UserName;

    SessID = logDetails.SessionId;  //response.data[0].SessionId
    LoginId = logDetails.LoginId;   //response.data[0].LoginId
    requestsProcessed = [];
    requestId = 0;
    Focus8WAPI.getFieldValue("fieldValueCallbackIssueBS_DCJWSal", ["", "DocNo", "Date", "CustomerAC", "Branch", "Dept", "Works Center", "Warehouse"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, ++requestId);
}

function fieldValueCallbackIssueBS_DCJWSal(response) {
    debugger;
    docNo = response.data[1].FieldValue;
    docDate = response.data[2].FieldValue;
    VendorAC = response.data[3].FieldValue;
    Branch = response.data[4].FieldValue;
    Dept = response.data[5].FieldValue
    WorksCenter = response.data[6].FieldValue;
    ivouchertype = response.data[0].iVoucherType;
    Warehouse = response.data[7].FieldValue;
    debugger;
    if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);

    try {

        Focus8WAPI.getBodyFieldValue("getbodyDatacallBackBS_DCJWSal", ["", "*"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, ++requestId);

    } catch (e) {
        console.log("fieldValueCallbacksupplierValidate", e.message)
    }
}
function getbodyDatacallBackBS_DCJWSal(response) {
    debugger;
    if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);
    try {


        // ArrResponse = ArrResponse + 1;

        //docNo = response.data[0].MetaDataHeader[0].iFieldId
        console.log('response:' + response);
        if (row < 0) {
            validRows = response.data[0].RowsInfo.iValidRows
        }
        else {
            validRows = response.RowsInfo.iValidRows;
        }
        table1 = [];

        //alert(validRows)
        if (validRows > 0) {

            b_ArrRequest = validRows;
            ArrResponse = 0;
            table1 = [];
            for (var i = 1; i <= validRows; i++) {

                Focus8WAPI.getBodyFieldValue("BodyRowdataSplitrollBS_DCJWSal", ["Process", "Item", "Quantity", "Rate", "Gross", "Discount", "sRemarks"], 2, false, i, ++requestId);
            }

        }
        else {
            //alert('Product Not Available')
            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
        }
    }
    catch (ex) {
        alert(ex.message);
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
    }

}

function BodyRowdataSplitrollBS_DCJWSal(response) {
    debugger
    //ArrResponse++
    //console.log('getdat:' + response)
    if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);

    table1.push({
        RowNo: parseInt(ArrResponse),
        Item: parseInt(response.data[1].FieldValue),
        Quantity: parseFloat(response.data[2].FieldValue),
        Rate: parseFloat(response.data[3].FieldValue),
        Gross: parseFloat(response.data[4].FieldValue)


    });
    ArrResponse++
    if (ArrResponse == validRows) {

        var postingData = { CompanyId: CID, SessionId: SessID, User: UserName, LoginId: LoginId, irowNo: row, vtype: ivouchertype, docNo: docNo, docDate: docDate, Branch: Branch, iBookNo: VendorAC, Dept: Dept, WorksCenter: WorksCenter, Warehouse: Warehouse, BodyData: table1 };
        table1 = [];
        requestId = 0;
        console.log(postingData);
        $.ajax({
            type: "POST",
            url: "/F9ICSKaleGrp_As/DCJWSal/CheckFGQty",
            data: JSON.stringify(postingData),
            contentType: "application/json; charset=utf-8",
            success: function (r) {
                debugger;

                //res = JSON.stringify(r);
                //alert(r.data.message);
                //console.log('Resu', Result);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);

            },
            error: function (serror) {
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            }
        })
    }

}
function toMoney(value, format = "en-IN", options = { minimumFractionDigits: 2, maximumFractionDigits: 2 }) {
    value = parseFloat(value);
    if (isNaN(value))
        return '';
    return Number(value).toLocaleString(format, options);
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

    function openPopupCallBackBeforeSave(objWrapperResult) {
        try {

            console.log(objWrapperResult);
        }
        catch (err) {
            alert("Exception :: openPopupCallBackBeforeSave :" + err.message);
            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
        }
    }