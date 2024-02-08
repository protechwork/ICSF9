
var baseUrl = 'http://localhost/BuyBackNew';
var requestsProcessed = [];
var bodyRequestsProcessed = [];
var Item = 0;
var quantity = 0;
var logDetails = {};
var loginDetails = {};
var docNo = 0;
var ibodyID = 0;
var type = 0;
var iBodyRowIndex = 0;
var rowIndex = 0;
var requestIds = [];
var requestId = 1;
var validRowsRMASales = 0;
var totalRowsRMASales = 0;
var rowDataRMASales = {};

var selectedRowSales = 1;
var requestIds = [];
var requestsProcessed = [];

var bodyDataSales = [];


var vsBodyData = {};
var vsBodyDataArr = [];

function isRequestCompleted(iRequestId, requestsArray) {
    return requestsArray.indexOf(iRequestId) === -1 ? false : true;
}
function isRequestProcessed(iRequestId) {
    for (let i = 0; i < requestsProcessed.length; i++) {
        if (requestsProcessed[i] == iRequestId) {
            return true;
        }
    } return false;
}
function UpdateSalesRMAData(...args) {

    Focus8WAPI.getBodyFieldValue('rowCallBack', ["", "*"], 2, false, 1, ++requestId);


    //
}
function rowCallBack(response) {
    console.log('rowCallBack :: ', response);
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
    for (let i = 1 ; i <= validRowsRMASales; i++) {
        Focus8WAPI.getBodyFieldValue('initializeRow', ["*"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, i, i);
    }
}


function initializeRow(response) {

    try {
        if (isRequestCompleted(response.iRequestId, bodyRequestsProcessed)) {
           return;
        }
        bodyRequestsProcessed.push(response.iRequestId);

        const row = initializeRowDataFields(response.data.slice(0));
        
        bodyDataSales.push(row);
        vsBodyData[response.iRequestId] = row;
        if (validRowsRMASales === Object.values(vsBodyData).length) {
            Focus8WAPI.getFieldValue("headerSalesCallbackalter", ["", "DocNo"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, ++requestId);
        }
    } catch (e) {
        alert(e.message)
    }
}

function initializeRowDataFields(fields) {

    const row = {};
    Object.values(fields).forEach((v, i, a) => {
        if (v) {
            row[`${v['sFieldName']}`] = {};
            row[`${v['sFieldName']}`]['sFieldName'] = v['sFieldName'];
            row[`${v['sFieldName']}`]['FieldText'] = v['FieldText'];
            row[`${v['sFieldName']}`]['FieldValue'] = v['FieldValue'];
            row[`${v['sFieldName']}`]['iFieldId'] = v['iFieldId'];
        }

    })
    //console.log('rowDataM   string:: ', JSON.stringify(row));
    return row;
}


function headerSalesCallbackalter(response) {
    try {

        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        logDetails = response.data[0];
        docNo = response.data[1].FieldValue;


        Focus8WAPI.getBodyFieldValue("bodySalesCallbackalter", ["Item", "Quantity"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, 1, ++requestId)


    } catch (e) {
        console.log("headerCallback", e.message)
    }
}



function bodySalesCallbackalter(response) {

    try {
        if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
            return;
        }
        requestsProcessed.push(response.iRequestId);
        debugger;
        console.log(response)

        Item = response.data[0].FieldValue;
        quantity = response.data[1].FieldValue;

        var datarequest = {
            companyId: logDetails.CompanyId,
            SessionId: logDetails.SessionId,
            LoginId: logDetails.LoginId,
            vtype: logDetails.iVoucherType,
            docNo: docNo,
            Item: Item,
            quantity: quantity,
            iBodyRowIndex: iBodyRowIndex
        };
       
    //var    SalesData = [];D
    //    for (let i = 0; i < bodyDataSales.length; i++) {

    //        //    alert(1)
    //        var RMADetails = {
    //            ProductName: bodyDataSales[i]["Item"]["FieldText"],                
    //            RMA: bodyDataSales[i]["RMA"]["FieldValue"],
    //        }
    //        var strarray = RMADetails.RMA.split(',');
    //        for (var j = 0; j < strarray.length; i++) {
    //            alert(strarray[j])


    //        }
    //        SalesData.push(RMADetails);
    //    }
        updateSalesQuantity(datarequest);
      //  Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);

    } catch (e) {
        console.log("bodySalesCallbackalter", e.message)
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
    }

}






function updateSalesQuantity(data) {
    debugger;

    var preferencesSales = [];
    preferencesSales = data;
    var ids = Object.values(bodyDataSales).map((data) => `id_body_${data.iFieldId}`);
    //   var check = Object.values(bodyData).map((data) => `id_body_${data.iFieldId}`);
    // var check=
     let emptyRow = {};
     for(let id of ids) {
         emptyRow[`${id}`] = '';
     }
     emptyRow.TransactionId = 0;
    //console.log(JSON.stringify(emptyRow));
    let row = selectedRow;


    debugger;
    var actualRowindex = 0;
    for (let j = 0; j < bodyDataSales.length; j++) {

        //var returnedData = $.grep(sale, function (element, index) {
        //    return element.EmptyRows == (j + 1);
        //});
        var returnedData = bodyDataSales[j]["RMA"]["FieldValue"].toString().split(',');
        

        if (returnedData != null && returnedData.length > 0) {
            for (let i = 0; i < returnedData.length; i++) {

                ++actualRowindex;
                ++requestId;
                requestIds.push(requestId);
                let copiedRowData = jQuery.extend(true, {}, bodyDataSales[j]);
                let fieldNames = Object.keys(copiedRowData);
                copiedRowData['Quantity'].FieldValue = 1;

                copiedRowData['Gross'].FieldValue = 1 * bodyDataSales[j].Rate.FieldValue;
                copiedRowData['RMA'].FieldValue = returnedData[i];
                let fieldValues = Object.values(copiedRowData).map((data) => data.FieldValue);

                if (actualRowindex < totalRowsRMASales) {
                    ++requestId;
                    requestIds.push(requestId);
                    Focus8WAPI.setBodyFieldValue('setBodyFieldValueCallBack', [...fieldNames], [...fieldValues], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, actualRowindex, requestId);

                } else {
                    ++requestId;
                    requestIds.push(requestId);
                    Focus8WAPI.setBodyFieldValue("setBodyFieldValueCallBack", "*", emptyRow, 2, false, -1, requestId);
                    ++requestId;
                    requestIds.push(requestId);
                    Focus8WAPI.setBodyFieldValue('setBodyFieldValueCallBack', [...fieldNames], [...fieldValues], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, actualRowindex, requestId);

                }

            }
        } else {

            ++requestId;
            requestIds.push(requestId);
            ++actualRowindex;
            let copiedRowData = jQuery.extend(true, {}, bodyDataSales[j]);
            let fieldNames = Object.keys(copiedRowData);
            let fieldValues = Object.values(copiedRowData).map((data) => data.FieldValue);
           
            if (actualRowindex < totalRowsRMASales) {
                ++requestId;
                requestIds.push(requestId);
                Focus8WAPI.setBodyFieldValue('setBodyFieldValueCallBack', [...fieldNames], [...fieldValues], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, actualRowindex, requestId);


            } else {
                ++requestId;
                requestIds.push(requestId);
                Focus8WAPI.setBodyFieldValue("setBodyFieldValueCallBack", "*", emptyRow, 2, false, -1, requestId);
                ++requestId;
                requestIds.push(requestId);
                Focus8WAPI.setBodyFieldValue('setBodyFieldValueCallBack', [...fieldNames], [...fieldValues], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, actualRowindex, requestId);

            }



        }        //alert(2)
        //Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
        //for(var x=0; x<=copiedRowData['Quantity'].FieldValue;x++)
        //{

        //}
        //  copiedRowData['No. Of Cyld.'].FieldValue =  1;


    }
}



function setBodyFieldValueCallBack(response) {
    if (isRequestProcessed(response.iRequestId)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);

}





function GetRMAData(...args) {
    //logDetails = [...args][0];
    //alert(5)

    getRowDataSales();


    //
}