
if (FWrapper == undefined || FWrapper == null) {
    var FWrapper = {
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
        OpenInvoiceDesigner: function (arrData, evt) {
            try {
                result = NETWORK.executeServerMethod(GLOBAL.getContextPath("InvoiceDesign", "InvoiceDesign", "Transactions"),
                                                           true,
                                                            {
                                                                LayoutId: arrData.LayoutId,
                                                                iVouchertype: arrData.iVouchertype,
                                                                iHeaderId: arrData.iHeaderId,
                                                                ModuleType: arrData.ModuleType,
                                                                HeaderGroup: arrData.HeaderGroup,
                                                                iSubReportId: arrData.iSubReportId,
                                                                bSaveHTMLSource: arrData.bSaveHTMLSource,
                                                            },
                                                           "html",
                                                           false);

                //return result;
                var objResponse = COMMON.prototype.getEmptyResultObject();
                objResponse.lValue = 1;
                objResponse.sValue = "";
                objResponse.data = result.data;
                var objReturn = {};
                objReturn.moduleType = FWrapper.PRIVATE.getModuleType(arrData);
                objReturn.requestType = FWrapper.ENUMS.REQUEST_TYPE_UI.OPEN_INVOICE_DESIGNER;
                objReturn.response = objResponse;
                objReturn.sCallbackFn = FWrapper.PRIVATE.getCallback(arrData);
                objReturn.iRequestId = FWrapper.PRIVATE.getRequestId(arrData);

                evt.source.postMessage(objReturn, evt.origin);

            } catch (Ex) {
                alert("Exception: Fwrapper: OpenInvoiceDesigner " + Ex.message);
                return false;
            }
        },

        receiveMessage: function (evt) {
            let iModuleType = 0;
            let arrData = null;

            try {
                arrData = evt.data;

                // Server
                if (FConvert.toBoolean(arrData.FromClient) == false) {
                    return;
                }

                console.log('Received request:  ', FConvert.toString(arrData));

                iModuleType = FWrapper.PRIVATE.getModuleType(arrData);

                switch (iModuleType) {
                    case FWrapper.ENUMS.MODULE_TYPE.MASTER:
                        FWrapper.processMasterRequest(arrData, iModuleType, evt);
                        break;
                    case FWrapper.ENUMS.MODULE_TYPE.TRANSACTION:
                        FWrapper.processTransactionRequest(arrData, iModuleType, evt);
                        break;
                    case FWrapper.ENUMS.MODULE_TYPE.TransHome:
                        FWrapper.processTransactionHomeRequest(arrData, iModuleType, evt);
                        break;
                    case FWrapper.ENUMS.MODULE_TYPE.UI:
                        FWrapper.processUIRequest(arrData, evt);
                        break;
                    case FWrapper.ENUMS.MODULE_TYPE.GLOBAL:
                        FWrapper.processGlobalRequest(arrData, iModuleType, evt);
                        break;
                    case FWrapper.ENUMS.MODULE_TYPE.MRP:
                        FWrapper.processMRPRequest(arrData, iModuleType, evt);
                        break;
                    case FWrapper.ENUMS.MODULE_TYPE.FixedAsset:
                        FWrapper.processFixedAssetRequest(arrData, iModuleType, evt);
                        break;
                }
            }
            catch (Ex) {
                alert("Exception: Fwrapper: receiveMessage " + Ex.message);
                return false;
            }

            return (true);
        },

        openPopup: function (sURL, sCloseCallback, sLoadCallback, sErrorCallback) {
            let eleContainer = null;

            try {
                alert(URL);
                debugger;
                sURL = FWrapper.prepareURL(sURL);
                if (FCommon.String.isNullOrEmpty(sURL, true) == true) {
                    return;
                }

                eleContainer = GLOBAL.getExternalModuleContainer();
                FCommon.UI.removeChildren(eleContainer);

                if (FCommon.String.isNullOrEmpty(sLoadCallback, true) == false) {
                    eleContainer.onload = function (event) {
                        if (FCommon.String.includes(sLoadCallback, "(") == true) {
                            eval(sLoadCallback);
                        }
                        else {
                            eval(sLoadCallback)(true, event);
                        }
                    };
                }

                if (FCommon.String.isNullOrEmpty(sErrorCallback, true) == false) {
                    eleContainer.onerror = function (event) {
                        if (FCommon.String.includes(sErrorCallback, "(") == true) {
                            eval(sErrorCallback);
                        }
                        else {
                            eval(sErrorCallback)(true, event);
                        }
                    };
                }

                eleContainer.src = sURL;
                eleContainer.style.display = "block";

                if (FCommon.String.isNullOrEmpty(sCloseCallback, true) == false) {
                    eleContainer.setAttribute("data-closecallback", sCloseCallback);
                }
                else {
                    eleContainer.setAttribute("data-closecallback", "");
                }
            }
            catch (err) {
                alert("Exception: Fwrapper: openPopup " + err.message);
            }
        },

        closePopup: function () {
            let eleContainer = null;
            let sCallback = "";

            try {
                eleContainer = GLOBAL.getExternalModuleContainer();
                FCommon.UI.removeChildren(eleContainer);

                eleContainer.src = "";
                eleContainer.style.display = "none";

                sCallback = eleContainer.getAttribute("data-closecallback");
                eleContainer.setAttribute("data-closecallback", "");

                if (FCommon.String.isNullOrEmpty(sCallback, true) == false) {
                    if (FCommon.String.includes(sCallback, "(") == true) {
                        eval(sCallback);
                    }
                    else {
                        eval(sCallback)();
                    }
                }
            }
            catch (err) {
                alert("Exception: Fwrapper: closePopup " + err.message);
            }
        },

        processMasterRequest: function (arrData, iModuleType, evt) {
            var fnCallback = "";
            var iRequestType = 0;
            var iRowIndex = 0;
            var sCallbackFn = "";
            var iRequestId = null;
            var isFieldId = false;
            var objData = null;
            var objResponse = null;
            var objReturn = null;

            try {
                iRequestType = FWrapper.PRIVATE.getRequestType(arrData);
                isFieldId = FWrapper.PRIVATE.isFieldId(arrData);
                iRowIndex = FWrapper.PRIVATE.getRowIndex(arrData);
                objData = FWrapper.PRIVATE.getData(arrData);
                sCallbackFn = FWrapper.PRIVATE.getCallback(arrData);
                iRequestId = FWrapper.PRIVATE.getRequestId(arrData);

                if (iRequestType == FWrapper.ENUMS.REQUEST_TYPE.CONTINUE) {
                    debugger
                    //MASTERENTRYSCREEN.continueEMProcess(arrData, evt);
                    MasterEntryExternalModule.continueProcess(arrData, evt);
                    //continueProcess(arrData.result, evt)
                    return ;
                }

                if (FCommon.UI.isValidObject(objData) == true) {

                    if (iRequestType == FWrapper.ENUMS.REQUEST_TYPE.GET) {
                        fnCallback = "FOCUS.MASTER.UI.getValue";
                    }
                    else if (iRequestType == FWrapper.ENUMS.REQUEST_TYPE_UI.MANDATORY_FIELDS_ENTRYSCREEN)
                    {
                        fnCallback = "FOCUS.MASTER.UI.getMandatoryFields";
                    }
                    else
                    {
                        fnCallback = "FOCUS.MASTER.UI.setValue";
                    }

                    if (fnCallback != null) {
                        objResponse = eval(fnCallback)(objData, isFieldId, iRowIndex);
                    }

                    objReturn = {};
                    objReturn.response = objResponse;
                    objReturn.moduleType = iModuleType;
                    objReturn.fieldId = objData.fieldid;
                    objReturn.requestType = iRequestType;
                    objReturn.sCallbackFn = sCallbackFn;
                    objReturn.iRequestId = iRequestId;

                    evt.source.postMessage(objReturn, evt.origin);
                }
            }
            catch (err) {
                alert("Exception: {Fwrapper.processMasterRequest} " + err.message);
            }
        },

        processTransactionRequest: function (arrData, iModuleType, evt) {
            let iRequestType = 0;
            let iVoucherType = 0;
            let sCallbackFn = "";
            let isFieldId = false;
            let bStruct = false;
            let rowIndex = null;
            let iRequestId = null;
            let objData = null;
            let objResponse = null;
            let objReturn = null;

            try {
                iRequestType = FWrapper.PRIVATE.getRequestType(arrData);
                if (iRequestType == FWrapper.ENUMS.REQUEST_TYPE.CONTINUE) {
                    transEntryExternalModule.continueProcess(arrData.result, evt)
                    return;
                }
                else if (iRequestType == FWrapper.ENUMS.REQUEST_TYPE.RESET_CACHE) {
                    iVoucherType = FWrapper.PRIVATE.getVoucherType(arrData);
                    GLOBAL.clearTransactionCache(iVoucherType);
                    return;
                }

                isFieldId = FWrapper.PRIVATE.isFieldId(arrData);
                rowIndex = FWrapper.PRIVATE.getRowIndex(arrData);
                objData = FWrapper.PRIVATE.getData(arrData);
                sCallbackFn = FWrapper.PRIVATE.getCallback(arrData);
                iRequestId = FWrapper.PRIVATE.getRequestId(arrData);
                bStruct = FWrapper.PRIVATE.getStruct(arrData);

                if (Array.isArray(rowIndex) == true) {
                    console.log('processTransactionRequest::RequestType:' + FConvert.toString(iRequestType) + "::isFieldId:" + FConvert.toString(isFieldId) + "::rowIndex:<Array>::Input:" + FConvert.toString(objData));
                }
                else {
                    console.log('processTransactionRequest::RequestType:' + FConvert.toString(iRequestType) + "::isFieldId:" + FConvert.toString(isFieldId) + "::rowIndex:" + FConvert.toString(iRowIndex) + "::Input:" + FConvert.toString(objData));
                }

                if (FCommon.UI.isValidObject(objData) == true) {

                    if (iRequestType == FWrapper.ENUMS.REQUEST_TYPE.GET) {
                        objResponse = eval("tranEntryAPI.getValue")(objData.fieldid, isFieldId == false, rowIndex, bStruct);
                        objResponse.FieldValue = objResponse.data;
                    }
                    else {
                        objResponse = eval("tranEntryAPI.setValue")(objData.fieldid, isFieldId == false, rowIndex, objData.value, bStruct);
                    }

                    objReturn = {};
                    objReturn.response = objResponse;
                    objReturn.moduleType = iModuleType;
                    objReturn.fieldId = objData.fieldid;
                    objReturn.requestType = iRequestType;
                    objReturn.sCallbackFn = sCallbackFn;
                    objReturn.iRequestId = iRequestId;
                    objReturn.bStruct = bStruct;

                    if (Array.isArray(rowIndex) == false && rowIndex > 0) {
                        objReturn.RowsInfo = eval("tranEntryAPI.getTotalValidRows")();
                    }

                    if (FCommon.UI.isValidObject(evt.source) == true) {
                        evt.source.postMessage(objReturn, evt.origin);
                    }                    
                }
            }
            catch (err) {
                alert("Exception: {Fwrapper.processTransactionRequest} " + err.message);
            }
        },

        processTransactionHomeRequest: function (arrData, iModuleType, evt) {
            var iRequestType = 0;

            try {
                debugger
                iRequestType = FWrapper.PRIVATE.getRequestType(arrData);
                if (iRequestType == FWrapper.ENUMS.REQUEST_TYPE.CONTINUE) {
                    transMainExternalModule.continueProcess(arrData.result, evt)
                    return;
                }

            }
            catch (err) {
                alert("Exception: {Fwrapper.processTransactionHomeRequest} " + err.message);
            }
        },

        processFixedAssetRequest: function (arrData, iModuleType, evt) {
            var iRequestType = 0;
            var iRowIndex = 0;
            var iVoucherType = 0;
            var sCallbackFn = "";
            var iRequestId = null;
            var isFieldId = false;
            var bStruct = false;
            var objData = null;
            var objResponse = null;
            var objReturn = null;

            try {
                iRequestType = FWrapper.PRIVATE.getRequestType(arrData);
                if (iRequestType == FWrapper.ENUMS.REQUEST_TYPE.CONTINUE) {
                    FixAEntryExternalModule.continueProcess(arrData.result, evt)
                    return;
                }                

                isFieldId = FWrapper.PRIVATE.isFieldId(arrData);
                iRowIndex = FWrapper.PRIVATE.getRowIndex(arrData);
                objData = FWrapper.PRIVATE.getData(arrData);
                sCallbackFn = FWrapper.PRIVATE.getCallback(arrData);
                iRequestId = FWrapper.PRIVATE.getRequestId(arrData);
                bStruct = FWrapper.PRIVATE.getStruct(arrData);

                console.log('processFixedAssetRequest::RequestType:' + FConvert.toString(iRequestType) + "::isFieldId:" + FConvert.toString(isFieldId) + "::iRowIndex:" + FConvert.toString(iRowIndex) + "Input:" + FConvert.toString(objData));

                if (FCommon.UI.isValidObject(objData) == true) {

                    if (iRequestType == FWrapper.ENUMS.REQUEST_TYPE.GET) {
                        objResponse = eval("FixedAssetAPI.getValue")(objData.fieldid, isFieldId == false, iRowIndex, bStruct);
                        objResponse.FieldValue = objResponse.data;
                    }
                    else {
                        objResponse = eval("FixedAssetAPI.setValue")(objData.fieldid, isFieldId == false, iRowIndex, objData.value, bStruct);
                    }

                    objReturn = {};
                    objReturn.response = objResponse;
                    objReturn.moduleType = iModuleType;
                    objReturn.fieldId = objData.fieldid;
                    objReturn.requestType = iRequestType;
                    objReturn.sCallbackFn = sCallbackFn;
                    objReturn.iRequestId = iRequestId;
                    objReturn.bStruct = bStruct;

                    if (iRowIndex > 0) {
                        objReturn.RowsInfo = eval("FixedAssetAPI.getTotalValidRows")();
                    }

                    if (FCommon.UI.isValidObject(evt.source) == true) {
                        evt.source.postMessage(objReturn, evt.origin);
                    }
                }
            }
            catch (err) {
                alert("Exception: {Fwrapper.processTransactionRequest} " + err.message);
            }
        },

        processMRPRequest: function (arrData, iModuleType, evt) {
            var iRequestType = 0;
            var iRowIndex = 0;
            var iVoucherType = 0;
            var sCallbackFn = "";
            var iRequestId = null;
            var isFieldId = false;
            var bStruct = false;
            var objData = null;
            var objResponse = null;
            var objReturn = null;

            try {
                iRequestType = FWrapper.PRIVATE.getRequestType(arrData);

                isFieldId = FWrapper.PRIVATE.isFieldId(arrData);
                iRowIndex = FWrapper.PRIVATE.getRowIndex(arrData);
                objData = FWrapper.PRIVATE.getData(arrData);
                sCallbackFn = FWrapper.PRIVATE.getCallback(arrData);
                iRequestId = FWrapper.PRIVATE.getRequestId(arrData);
                bStruct = FWrapper.PRIVATE.getStruct(arrData);

                console.log('processMRPRequest::RequestType:' + FConvert.toString(iRequestType) + "::isFieldId:" + FConvert.toString(isFieldId) + "::iRowIndex:" + FConvert.toString(iRowIndex) + "Input:" + FConvert.toString(objData));

                if (FCommon.UI.isValidObject(objData) == true) {

                    if (iRequestType == FWrapper.ENUMS.REQUEST_TYPE.GET) {
                        objResponse = eval("MRP.getValue")(objData.fieldid, isFieldId == false, iRowIndex, bStruct);
                        objResponse.FieldValue = objResponse.data;
                    }
                    else {
                        objResponse = eval("MRPCommon.MRPExternalMethods.SetValue")(objData.fieldid, isFieldId == false, iRowIndex, objData.value, bStruct);
                    }

                    objReturn = {};
                    objReturn.response = objResponse;
                    objReturn.moduleType = iModuleType;
                    objReturn.fieldId = objData.fieldid;
                    objReturn.requestType = iRequestType;
                    objReturn.sCallbackFn = sCallbackFn;
                    objReturn.iRequestId = iRequestId;
                    objReturn.bStruct = bStruct;


                    if (FCommon.UI.isValidObject(evt.source) == true) {
                        evt.source.postMessage(objReturn, evt.origin);
                    }
                }
            }
            catch (err) {
                alert("Exception: {Fwrapper.processMRPRequest} " + err.message);
            }
        },


        processUIRequest: function (arrData, evt) {
            var iRequestType = 0;
            var eleContainer = null;
            var value = null;

            try {
                iRequestType = FWrapper.PRIVATE.getRequestType(arrData);
                switch (iRequestType) {
                    case FWrapper.ENUMS.REQUEST_TYPE_UI.SET_POPUP_COORDINATE:
                        eleContainer = GLOBAL.getExternalModuleContainer();
                        if (FCommon.UI.isValidObject(eleContainer) == false) {
                            console.log('SET_POPUP_COORDINATE::Container not found;');
                            break;
                        }
                        value = FConvert.toString(arrData.Left);
                        if (FCommon.String.isNullOrEmpty(value, true) == false) {
                            eleContainer.style.left = value;
                        }

                        value = FConvert.toString(arrData.Top);
                        if (FCommon.String.isNullOrEmpty(value, true) == false) {
                            eleContainer.style.top = value;
                        }

                        value = FConvert.toString(arrData.Width);
                        if (FCommon.String.isNullOrEmpty(value, true) == false) {
                            eleContainer.style.width = value;
                        }

                        value = FConvert.toString(arrData.Height);
                        if (FCommon.String.isNullOrEmpty(value, true) == false) {
                            eleContainer.style.height = value;
                        }

                        break;
                    case FWrapper.ENUMS.REQUEST_TYPE_UI.OPEN_POPUP:
                        FWrapper.openPopup(arrData["URL"]);
                        break;
                    case FWrapper.ENUMS.REQUEST_TYPE_UI.CLOSE_POPUP:
                        FWrapper.closePopup();
                        break;
                    case FWrapper.ENUMS.REQUEST_TYPE_UI.GOTOHOMEPAGE:
                        GLOBAL.gotoHomePage();
                        break;
                    case FWrapper.ENUMS.REQUEST_TYPE_UI.LOGOUT:
                        GENERAL.Logout();
                        break;
                    case FWrapper.ENUMS.REQUEST_TYPE_UI.AWAKE_SESSION:
                        GLOBAL.awakeSession();
                        break;
                    case FWrapper.ENUMS.REQUEST_TYPE_UI.OPEN_INVOICE_DESIGNER:
                        FWrapper.OpenInvoiceDesigner(arrData, evt);
                        break;
                }
            }
            catch (err) {
                alert("Exception: {Fwrapper.processUIRequest} " + err.message);
            }
        },

        processGlobalRequest: function (arrData, iModuleType, evt) {
            var iRequestType = 0;
            var sVariable = "";
            var sCallbackFn = "";
            var iRequestId = null;
            var objGlobalValue = null;
            var objResponse = null;
            var objReturn = null;

            try {
                iRequestType = FWrapper.PRIVATE.getRequestType(arrData);
                sVariable = FConvert.toString(arrData["Variable"]);
                sCallbackFn = FWrapper.PRIVATE.getCallback(arrData);
                iRequestId = FWrapper.PRIVATE.getRequestId(arrData);

                console.log('GlobalValue::RequestType:' + FConvert.toString(iRequestType) + "::Variable:" + sVariable);

                switch (iRequestType) {
                    case FWrapper.ENUMS.REQUEST_TYPE.GET:
                        objResponse = COMMON.prototype.getEmptyResultObject();
                        objResponse.lValue = 0;
                        objResponse.sValue = "";
                        objResponse.data = {};

                        objGlobalValue = FWrapper.PRIVATE.getGlobalObject();

                        sVariable = sVariable.toUpperCase();
                        if (sVariable == "SESSION" || sVariable == "SESSIONID") {
                            objResponse.lValue = 1;
                            objResponse.sValue = "";
                            objResponse.data = {};
                            objResponse.data.FieldText = objGlobalValue.SessionId;
                            objResponse.data.FieldValue = objGlobalValue.SessionId;
                        }
                        else if (sVariable == "COMPANY" || sVariable == "COMPANYID") {
                            objResponse.lValue = 1;
                            objResponse.sValue = "";
                            objResponse.data = {};
                            objResponse.data.FieldText = objGlobalValue.CompanyId;
                            objResponse.data.FieldValue = objGlobalValue.CompanyId;
                        }
                        else if (sVariable == "YEARID") {
                            objResponse.lValue = 1;
                            objResponse.sValue = "";
                            objResponse.data = {};
                            objResponse.data.FieldText = objGlobalValue.YearId;
                            objResponse.data.FieldValue = objGlobalValue.YearId;
                        }
                        else if (sVariable == "LOGIN" || sVariable == "LOGINID") {
                            objResponse.lValue = 1;
                            objResponse.sValue = "";
                            objResponse.data = {};
                            objResponse.data.FieldText = objGlobalValue.LoginId;
                            objResponse.data.FieldValue = objGlobalValue.LoginId;
                        }
                        else if (sVariable == "LOGINNAME" || sVariable == "USERNAME") {
                            objResponse.lValue = 1;
                            objResponse.sValue = "";
                            objResponse.data = {};
                            objResponse.data.FieldText = objGlobalValue.UserName;
                            objResponse.data.FieldValue = objGlobalValue.UserName;
                        }
                        else if (sVariable == "LANGUAGE" || sVariable == "LANGUAGEID") {
                            objResponse.lValue = 1;
                            objResponse.sValue = "";
                            objResponse.data = {};
                            objResponse.data.FieldText = objGlobalValue.LanguageId;
                            objResponse.data.FieldValue = objGlobalValue.LanguageId;
                        }
                        else if (sVariable == "ALTLANGUAGE" || sVariable == "ALTLANGUAGEID") {
                            objResponse.lValue = 1;
                            objResponse.sValue = "";
                            objResponse.data = {};
                            objResponse.data.FieldText = objGlobalValue.AltLanguageId;
                            objResponse.data.FieldValue = objGlobalValue.AltLanguageId;
                        }
                        else if (sVariable == "CALENDAR" || sVariable == "CALENDARTYPE") {
                            objResponse.lValue = 1;
                            objResponse.sValue = "";
                            objResponse.data = {};
                            objResponse.data.FieldText = objGlobalValue.CalendarType;
                            objResponse.data.FieldValue = objGlobalValue.CalendarType;
                        }
                        else if (sVariable == "FINANCIALDATE" || sVariable == "ACCOUNTINGDATE") {
                            objResponse.lValue = 1;
                            objResponse.sValue = "";
                            objResponse.data = {};
                            objResponse.data.FieldText = objGlobalValue.AccountingDate;
                            objResponse.data.FieldValue = objGlobalValue.AccountingDate;
                        }
                        else if (sVariable == "LOGID") {
                            objResponse.lValue = 1;
                            objResponse.sValue = "";
                            objResponse.data = {};
                            objResponse.data.FieldText = objGlobalValue.LogId;
                            objResponse.data.FieldValue = objGlobalValue.LogId;
                        }
                        else if (sVariable == "CDID") {
                            objResponse.lValue = 1;
                            objResponse.sValue = "";
                            objResponse.data = {};
                            objResponse.data.FieldText = objGlobalValue.CDID;
                            objResponse.data.FieldValue = objGlobalValue.CDID;
                        }
                        else {
                            objResponse.lValue = 1;
                            objResponse.sValue = "";
                            objResponse.data = objGlobalValue;
                        }

                        if (objResponse.lValue > 0) {
                            objReturn = {};
                            objReturn.moduleType = iModuleType;
                            objReturn.requestType = iRequestType;
                            objReturn.fieldId = sVariable;
                            objReturn.response = objResponse;
                            objReturn.sCallbackFn = sCallbackFn;
                            objReturn.iRequestId = iRequestId;

                            evt.source.postMessage(objReturn, evt.origin);
                        }
                        break;
                }
            }
            catch (err) {
                alert("Exception: {Fwrapper.processGlobalRequest} " + err.message);
            }
        },

        prepareURL: function (sURL) {
            let sProtocol = "";
            let sPathName = "";
            let sHost = "";

            try {
                sProtocol = document.location.protocol;
                sHost = document.location.host;
                sPathName = document.location.pathname;
                if (sPathName.indexOf("/", 1) < 0) {
                    sPathName = sPathName + "/";
                }                   
                sPathName = sPathName.substr(0, sPathName.indexOf("/", 1));
                sPathName = "/";

                sURL = FConvert.toString(sURL).trim();
                if (FCommon.String.isNullOrEmpty(sURL) == true) {
                    return (sURL);
                }

                if (FCommon.String.startsWith(sURL, "/") == true) {
                    return (sProtocol + "//" + sHost + sPathName + sURL);
                }

                if (FCommon.String.startsWith(sURL.toLowerCase(), "www.") == true) {
                    return (sURL);
                }

                if (FCommon.String.startsWith(sURL.toLowerCase(), "http:") == true) {
                    return (sURL);
                }

                if (FCommon.String.startsWith(sURL.toLowerCase(), "https:") == true) {
                    return (sURL);
                }

                return (sProtocol + "//" + sHost + sPathName + "/" + sURL);
            }
            catch (err) {
                WriteConsoleLog("Exception: {FWrapper.prepareURL} " + err.message, "red");
            }

            return (sURL);
        },

        loadExternalJS: function (sURL, sFunctionName, objCallback, data, data1, sLoadedCallback) {
            let scriptTag = null;
            let location = null;
            let obj = null;

            try {
                sURL = FWrapper.prepareURL(sURL);
                if (FWrapper.isJSAlreadyLoaded(sURL) == true) {
                    if (FCommon.String.isNullOrEmpty(sFunctionName, true) == false) {
                        if (FCommon.String.isNullOrEmpty(sLoadedCallback, true) == false) {
                            try {
                                eval(sLoadedCallback)(data, data1);
                            }
                            catch (err) {
                            }
                        }

                        try {

                            if (FCommon.String.isNullOrEmpty(objCallback, true) == false) {
                                var ReturnObj = eval(sFunctionName)(objCallback.Data, data, data1);
                                eval(objCallback.Function)(ReturnObj);
                            }
                            else {
                                obj = FWrapper.PRIVATE.getGlobalObject();

                                eval(sFunctionName)(obj, data, data1);
                            }
                        }
                        catch (err) {
                            alert("Exception: {FWrapper.loadExternalJS.alreadyLoaded} " + err.message);
                        }
                    }

                    return;
                }

                location = UIContainer.getDefaultContainer();
                if (FCommon.UI.isValidObject(location) == false) {
                    location = document.head;
                }

                scriptTag = document.createElement('script');

                scriptTag.onload = function (evt) {
                    if (FCommon.String.isNullOrEmpty(sFunctionName, true) == false) {
                        var obj = null;

                        if (FCommon.String.isNullOrEmpty(sLoadedCallback, true) == false) {
                            try {
                                eval(sLoadedCallback)(data, data1);
                            }
                            catch (err) {
                            }
                        }

                        try {
                            if (FCommon.String.isNullOrEmpty(objCallback, true) == false) {

                                var ReturnObj = eval(sFunctionName)(objCallback.Data, data, data1);

                                eval(objCallback.Function)(ReturnObj);

                            }
                            else {

                                obj = FWrapper.PRIVATE.getGlobalObject();

                                eval(sFunctionName)(obj, data, data1);
                            }

                        }
                        catch (err) {
                            alert("Exception: {FWrapper.loadExternalJS.onload} " + err.message);
                        }
                    }
                }

                scriptTag.src = sURL;
                location.appendChild(scriptTag);
            }
            catch (err) {
                alert("Exception: {Fwrapper.loadExternalJS} " + err.message);
            }
        },

        isJSAlreadyLoaded: function (sURL) {
            var scripts = null;
            var iCounter = 0;

            try {
                scripts = document.getElementsByTagName('script');
                for (iCounter = scripts.length - 1; iCounter >= 0; iCounter--) {
                    if (scripts[iCounter].src.toLowerCase() == sURL.toLowerCase()) {
                        return (true);
                    }
                }
            }
            catch (err) {
                alert("Exception: {Fwrapper.isJSAlreadyLoaded} " + err.message);
            }

            return (false);
        },

        PRIVATE: {
            getModuleType: function (arrData) {
                var value = 0;

                value = FConvert.toInt(arrData['moduleType']);

                return (value);
            },

            getRequestType: function (arrData) {
                var value = 0;

                value = FConvert.toInt(arrData['requestType']);

                return (value);
            },

            isFieldId: function (arrData) {
                var value = false;

                value = FConvert.toBoolean(arrData['isFieldId']);

                return (value);
            },

            getRowIndex: function (arrData) {
                let value = 0;

                if (Array.isArray(arrData["rowIndex"]) == true) {
                    value = arrData["rowIndex"];
                }
                else {
                    value = FConvert.toInt(arrData["rowIndex"]);
                }

                return (value);
            },

            getVoucherType: function (arrData) {
                var value = 0;

                value = FConvert.toInt(arrData["iVoucherType"]);

                return (value);
            },

            getData: function (arrData) {
                var value = null;

                value = arrData['objData'];

                return (value);
            },

            getCallback: function (arrData) {
                var value = "";

                value = arrData["sCallbackFn"];

                return (value);
            },

            getRequestId: function (arrData) {
                var value = null;

                value = arrData["iRequestId"];

                return (value);
            },

            getStruct: function (arrData) {
                var value = null;

                value = FConvert.toBoolean(arrData["bStruct"]);

                return (value);
            },

            getGlobalObject: function () {
                var objGlobalValue = null;
                var obj = null;

                try {
                    objGlobalValue = GLOBAL.getGlobalValue();

                    obj = {};
                    obj.SessionId = objGlobalValue.SessionId;
                    obj.CompanyId = objGlobalValue.CompanyId;
                    obj.LoginId = objGlobalValue.LoginId;
                    obj.UserName = objGlobalValue.UserName;
                    obj.LanguageId = objGlobalValue.LanguageId;
                    obj.AltLanguageId = objGlobalValue.AltLanguageId;
                    obj.CalendarType = objGlobalValue.CalendarType;
                    obj.AccountingDate = objGlobalValue.AccountingDate;
                    obj.LiteVersion = objGlobalValue.LiteVersion;
                    obj.YearId = FConvert.getYearId(obj.CompanyId);
                    obj.CDID = FConvert.toString(objGlobalValue.CDID);
                    obj.CurrentTime = new Date();
                }
                catch (err) {
                    alert("Exception: {Fwrapper.PRIVATE.getGlobalObject} " + err.message);
                }

                return (obj);
            }
        }
    }

    if (window.addEventListener) {
        window.addEventListener("message", FWrapper.receiveMessage, false);
    }
    else if (window.attachEvent) {
        window.attachEvent("message", FWrapper.receiveMessage, false);
    }

    //window.addEventListener('message', FWrapper.receiveMessage, false);
}


