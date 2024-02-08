var requestId = 1;
var CID = "";
var CID = ''; var SessID = ''; var UserName = '';
var logindetails = {};
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
var svno = [];
var ServerIP = "localhost";
//before save

function SplitRoll(logDetails, rowIndex, b) {

    debugger
    compid = logDetails.CompanyId
    row = rowIndex;
    //b_ArrRequest = 1;
    Focus8WAPI.getBodyFieldValue("getbodyDatacallBack", ["", "*"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, rowIndex);
}

function getbodyDatacallBack(response) {
    debugger;
    try {
        // ArrResponse = ArrResponse + 1;

        console.log('response:' + response);
        if (row < 0) {
            validRows = response.data[0].RowsInfo.iValidRows
        }
        else {
            validRows = response.RowsInfo.iValidRows;
        }

        vtype = response.data[0].iVoucherType;
        //alert(validRows)
        if (validRows > 0) {
            b_ArrRequest = validRows;
            ArrResponse = 0;
            table1 = [];
            for (var i = 1; i <= validRows; i++) {

                Focus8WAPI.getBodyFieldValue("BodyRowdataSplitroll", ["Cost Center", "Location", "Tax Code", "Department", "Item", "Description", "Unit", "No Of Batches", "Quantity", "L-Purchases Orders", "Rate", "Gross", "Discount", "Discount Amount", "Tax %", "Tax Amount", "sCarton", "sRemarks", "Priority", "ETD_Date", "PO_No", "Batch_RollNo"], 2, false, i, i);
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

function getbodyDatacallBack(response) {
    debugger;
    try {
        // ArrResponse = ArrResponse + 1;

        console.log('response:' + response);
        if (row < 0) {
            validRows = response.data[0].RowsInfo.iValidRows
        }
        else {
            validRows = response.RowsInfo.iValidRows;
        }

        vtype = response.data[0].iVoucherType;
        //alert(validRows)
        if (validRows > 0) {
            b_ArrRequest = validRows;
            ArrResponse = 0;
            table1 = [];
            for (var i = 1; i <= validRows; i++) {

                Focus8WAPI.getBodyFieldValue("BodyRowdataSplitroll", ["Cost Center", "Location", "Tax Code", "Department", "Item", "Description", "Unit", "No Of Batches", "Quantity", "L-Purchases Orders", "Rate", "Gross", "Discount", "Discount Amount", "Tax %", "Tax Amount", "sCarton", "sRemarks", "Priority", "ETD_Date", "PO_No", "Batch_RollNo"], 2, false, i, i);
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
function fnGetValueCallBack4(objWrapperResult) {
    debugger
    console.log(objWrapperResult);
    if (objWrapperResult.message != "") {
    }
    else {
        var responseData = objWrapperResult.data;
        if (objWrapperResult.requestType == 1) {
        }
        else {
        }
    }
}

function BodyRowdataSplitroll(response) {
    debugger
    ArrResponse++
    console.log('getdat:' + response)
    table1.push({
        RowNo: parseInt(ArrResponse),
        costcenter: parseInt(response.data[0].FieldValue),
        Location: parseInt(response.data[1].FieldValue),
        Taxcode: parseInt(response.data[2].FieldValue),
        Department: parseInt(response.data[3].FieldValue),
        Item: parseInt(response.data[4].FieldValue),
        Description: (response.data[5].FieldValue),
        Unit: (response.data[6].FieldValue),
        NoOfRolls: parseInt(response.data[7].FieldValue),
        Quantity: parseFloat(response.data[8].FieldValue),
        Purchases: (response.data[9].FieldValue),       
        Rate: parseFloat(response.data[10].FieldValue),
        Gross: parseFloat(response.data[11].FieldValue),
        Discount: parseFloat(response.data[12].FieldValue),
        DiscountAmt: parseFloat(response.data[13].FieldValue),
        Tax: response.data[14].FieldValue,
        Taxamount: response.data[15].FieldValue,
        sCarton: response.data[16].FieldValue,
        sRemarks: response.data[17].FieldValue,
        Priority: response.data[18].FieldValue,      
        ETD_Date: response.data[19].FieldValue,
        PO_No: response.data[20].FieldValue,
        Batch_RollNo: response.data[21].FieldValue
    });

    if (b_ArrRequest == ArrResponse) {

        var ids = [536870917, 536870918, 536870925, 536870915, 23, 16777321, 24, 33554630, 26, 35, 27, 28, 33554460, 33554601, 33554461, 33554602, 16777274, 16777322, 16777323, 16777324, 16777335, 16777609].map((iFieldId) => `id_body_${iFieldId}`);
        let emptyRow = {};
        for(let id of ids) {
            emptyRow[`${id}`] = '';
        }


        var j = 1;
        var Qty = 0;
        var QtyBase = 0;
        for (var i = 0; i < table1.length; i++) {

            var rollno = table1[i].NoOfRolls;
            var rollqty = 1;
            if (rollno > 0) {
                Qty = parseFloat(table1[i].Quantity) / parseFloat(rollno);
                //QtyBase = parseFloat(table1[i].QtyInBase) / parseFloat(rollno);
            }
            else {
                Qty = parseFloat(table1[i].Quantity);
                //QtyBase = parseFloat(table1[i].QtyInBase);
                rollno = 1
                rollqty = 0;
            }
            for (var k = 0; k < rollno ; k++) {
                var gross = Qty * table1[i].Rate

                if (j <= validRows) {


                    Focus8WAPI.setBodyFieldValue("fnGetValueCallBack4", ["Cost Center", "Location", "Tax Code", "Department", "Item", "Description", "Unit", "No Of Batches", "Quantity", "L-Purchases Orders", "Rate", "Gross", "Discount", "Discount Amount", "Tax %", "Tax Amount", "sCarton", "sRemarks", "Priority", "ETD_Date", "PO_No", "Batch_RollNo"], [table1[i].costcenter, table1[i].Location, table1[i].Taxcode, table1[i].Department, table1[i].Item, table1[i].Description, table1[i].Unit, rollqty, Qty, table1[i].Purchases, table1[i].Rate, gross, table1[i].Discount, table1[i].DiscountAmt, table1[i].Tax, table1[i].Taxamount, table1[i].sCarton, table1[i].sRemarks, table1[i].Priority, table1[i].ETD_Date, table1[i].PO_No, table1[i].Batch_RollNo], 2, false, j, 0)
                }
                else {
                    Focus8WAPI.setBodyFieldValue("fnGetValueCallBack4", "*", emptyRow, Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, -1, j);
                    Focus8WAPI.setBodyFieldValue("fnGetValueCallBack4", ["Cost Center", "Location", "Tax Code", "Department", "Item", "Description", "Unit", "No Of Batches", "Quantity", "L-Purchases Orders", "Rate", "Gross", "Discount", "Discount Amount", "Tax %", "Tax Amount", "sCarton", "sRemarks", "Priority", "ETD_Date", "PO_No", "Batch_RollNo"], [table1[i].costcenter, table1[i].Location, table1[i].Taxcode, table1[i].Department, table1[i].Item, table1[i].Description, table1[i].Unit, rollqty, Qty, table1[i].Purchases, table1[i].Rate, gross, table1[i].Discount, table1[i].DiscountAmt, table1[i].Tax, table1[i].Taxamount, table1[i].sCarton, table1[i].sRemarks, table1[i].Priority, table1[i].ETD_Date, table1[i].PO_No, table1[i].Batch_RollNo], 2, false, j, 0)
                }
                j = j + 1;
            }

        }
        //var inc = parseInt(response.data.message) + 1;
        //for (var i = 0; i < response.data.result.length; i++) {

        //    if (response.data.result[i].Batch_RollNO == "" && response.data.result[i].batchcheck == 1) {

        //        var Batchno = "T" + inc
        //        Focus8WAPI.setBodyFieldValue("fnGetValueCallBack3", "Batch_Roll_No", Batchno, 2, false, i + 1, 0)
        //        inc = inc + 1;
        //    }

        //    //j = table.length

        //}





    }

}

