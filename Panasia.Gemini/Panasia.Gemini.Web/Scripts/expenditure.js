function EditField(title, control, right) {
    var editField =
        '<div class="edit-field ' + (right ? "edit-field-right" : "") + '">' +
            '<div class="edit-title ' + (right ? "edit-title-right" : "") + '">' +
                '<label class="edit-label ' + (right ? "edit-label-right" : "") + '">' +
                    title +
                '</label>' +
            '</div>' +
            '<div class="edit-content">' +
                control +
            '</div>' +
        '</div>';

    return editField;
}
function DataGrid(id, title, url, columns, customOptions) {
    var dataGrid =
        '<div class="edit-field-r2" style="height: auto; width: 490px; margin-left: 70px;">' +
            '<table id="' + id + '" class="easyui-datagrid" data-options="url:\'' + url + '\'' + customOptions + '" style="width: 463px; height: 100px;" title="' + title + '">' +
                '<thead>' +
                    '<tr>';
    for (var key in columns) {
        dataGrid += '<th data-options="field:\'' + columns[key].field + '\''+
            (columns[key]["width"] ? ',width:' + columns[key].width : '') +
            (columns[key]["checkbox"] ? ',checkbox:true' : '') +
            '">' + key + '</th>';
    }
    dataGrid +=
                    '</tr>' +
                '</thead>' +
            '</table>' +
        '</div>';
    return dataGrid;
}

var paymentDefine = {};
//业务付款
paymentDefine["C07201"] =
    EditField("供应商名称：",
    '<input class="edit-input easyui-combobox" id="Company" name="Company" panelheight="auto" ' +
    'data-options="url:\'/Page/P01041/GetSupplierList\',editable: false,valueField: \'SupplierID\',textField: \'SupplierNAME\'' +
    ',required:true,missingMessage:\'请选择供应商\'' +
    (IsDetail ? ",readonly:true,hasDownArrow:false" : "") +
    ',onSelect:function(data){$(\'#Bank\').combobox({url:\'/Page/P01041/GetSupplierBankList?SupplierID=\'+data.SupplierID});' +
    '$(\'#Account\').combobox(\'clear\');$(\'#Account\').combobox(\'loadData\',[]);}' +
    ((IsModify == 1 && PaymentID == "C07201") ? ',onLoadSuccess:function(){$(this).combobox(\'select\', jsonData[0][$(this).attr(\'id\')]);}' : '') +
    '" />') +
    EditField("供应商银行：",
    '<input class="edit-input easyui-combobox" id="Bank" name="Bank" panelheight="auto" ' +
    'data-options="editable: false,valueField: \'Bank\',textField: \'Bank\'' +
    ',required:true,missingMessage:\'请选择银行\'' +
    (IsDetail ? ",readonly:true,hasDownArrow:false" : "") +
    ',onSelect:function(data){$(\'#Account\').combobox({url:\'/Page/P01041/GetSupplierBankAccountList?SupplierID=\'+data.SupplierID + \'&Bank=\'+data.Bank});}' +
    ((IsModify == 1 && PaymentID == "C07201") ? ',onLoadSuccess:function(){$(this).combobox(\'select\', jsonData[0][$(this).attr(\'id\')]);}' : '') +
    '" />', true) +
    EditField("银行账号：",
    '<input class="edit-input easyui-combobox" id="Account" name="Account" panelheight="auto" ' +
    ((IsModify == 1 && PaymentID == "C07201") ? ' value="' + jsonData[0]["Account"] + '" ' : '') +
    'data-options="editable: false,valueField: \'Account\',textField: \'Account\'' +
    ',required:true,missingMessage:\'请选择账号\'' +
    (IsDetail ? ",readonly:true,hasDownArrow:false" : "") +
    '" />');
//费用支出
paymentDefine["C07202"] =
    EditField("支出项目：",
    '<input class="edit-input easyui-combobox" style="width:85px;" id="CostCategories" panelheight="auto" ' +
    'data-options="url:\'/Combobox/GetOutPayment?ParentID=\',editable: false,valueField: \'id\',textField: \'text\'' +
    ',required:true,missingMessage:\'请选择费用大类\'' +
    (IsDetail ? ",readonly:true,hasDownArrow:false" : "") +
    ',onSelect:function(data){$(\'#ZPaymentID\').combobox({url:\'/Combobox/GetOutPayment?ParentID=\'+data.id});}' +
    ((IsModify == 1 && PaymentID == "C07202") ? ',onLoadSuccess:function(){$(this).combobox(\'select\', jsonData[0][$(this).attr(\'id\')]);}' : '') +
    '" />' + 
    '&nbsp;<input class="edit-input easyui-combobox" style="width:95px;" id="ZPaymentID" name="ZPaymentID" panelheight="auto" panelMaxHeight="235" ' +
    'data-options="editable: false,valueField: \'id\',textField: \'text\'' +
    ',required:true,missingMessage:\'请选择费用小类\'' +
    (IsDetail ? ",readonly:true,hasDownArrow:false" : "") +
    ((IsModify == 1 && PaymentID == "C07202") ? ',onLoadSuccess:function(){$(this).combobox(\'select\', jsonData[0][$(this).attr(\'id\')]);}' : '') +
    '" />') +
    EditField("使用基金：",
    '<input type="checkbox" name="IsReiFund" value="1"' +
    (IsDetail ? 'disabled="disabled"' : "") +
    ((IsModify == 1 && PaymentID == "C07202") ? (jsonData[0]["IsReiFund"] ? 'checked="checked"' : '') : '') +
    '/>', true);
//退款
paymentDefine["C07203"] =
    EditField("客户名称：",
    '<input class="edit-input easyui-combobox" id="Customer" name="Customer" panelheight="auto" ' +
    'data-options="url:\'/Page/P01041/GetCustomerList\',editable: false,valueField: \'CustomerID\',textField: \'CustomerName\'' +
    ',required:true,missingMessage:\'请选择客户\'' +
    (IsDetail ? ",readonly:true,hasDownArrow:false" : "") +
    ',onSelect:function(data){$(\'#dgReOrderProduct\').datagrid({url:' +
    (IsDetail ? "'/Page/P01041/GetReOrderProductDetail?ExpenditureID=" + ExpenditureID + "'" :
    "'/Page/P01041/GetReOrderProducts?CustomerID='+data.CustomerID+'&IsModify=" + IsModify + "&ExpenditureID=" + ExpenditureID + "'") +
    "});}" +
    ((IsModify == 1 && PaymentID == "C07203") ? ',onLoadSuccess:function(){$(this).combobox(\'select\', jsonData[0][$(this).attr(\'id\')]);}' : '') +
    '" />') +
    (IsDetail ? DataGrid("dgReOrderProduct", "", "",
    {
        "订单编号": {"field":"OrderID", "width":110},
        "产品分类": {"field":"ProductType", "width":80},
        "产品名称": {"field":"Product", "width":100},
        "退单金额": {"field":"Amount", "width":60},
        "备注": { "field": "Remark", "width":111 }
    },"") :
    DataGrid("dgReOrderProduct", "", "",
    {
        "": { "field": "AutoKey", "checkbox": true },
        "订单编号": {"field":"OrderID", "width":110},
        "产品分类": {"field":"ProductType", "width":80},
        "产品名称": {"field":"Product", "width":80},
        "退单金额": {"field":"Amount", "width":60},
        "备注": { "field": "Remark", "width":103 }
    }, ",onSelect:TotalAmount,onUnselect:TotalAmount,onSelectAll:TotalAmount,onUnselectAll:TotalAmount"+
    ((IsModify == 1 && PaymentID == "C07203") ? ",onLoadSuccess:function(){BindSelectedOrder('dgReOrderProduct');}" : "")));
//内部往来
paymentDefine["C07204"] =
    EditField("往来单位：",
    '<input class="edit-input easyui-combobox" id="Company" name="Company" panelheight="auto" panelMaxHeight="235" ' +
    'data-options="url:\'/Page/P01041/GetAllCompany\',editable: false,valueField: \'CompanyID\',textField: \'Name\'' +
    ',required:true,missingMessage:\'请选择往来单位\'' +
    (IsDetail ? ",readonly:true,hasDownArrow:false" : "") +
    ',onSelect:function(data){$(\'#Bank\').combobox({url:\'/Page/P01041/GetCompanyBankList?CompanyID=\'+data.CompanyID});' +
    '$(\'#Account\').combobox(\'clear\');$(\'#Account\').combobox(\'loadData\',[]);}' +
    ((IsModify == 1 && PaymentID == "C07204") ? ',onLoadSuccess:function(){$(this).combobox(\'select\', jsonData[0][$(this).attr(\'id\')]);}' : '') +
    '" />') +
    EditField("银行：",
    '<input class="edit-input easyui-combobox" id="Bank" name="Bank" panelheight="auto" ' +
    'data-options="editable: false,valueField: \'BankName\',textField: \'BankName\'' +
    ',required:true,missingMessage:\'请选择银行\'' +
    (IsDetail ? ",readonly:true,hasDownArrow:false" : "") +
    ',onSelect:function(data){$(\'#Account\').combobox({url:\'/Page/P01041/GetCompanyBankAccountList?CompanyID=\'+data.CompanyID + \'&BankName=\'+data.BankName});}' +
    ((IsModify == 1 && PaymentID == "C07204") ? ',onLoadSuccess:function(){$(this).combobox(\'select\', jsonData[0][$(this).attr(\'id\')]);}' : '') +
    '" />', true) +
    EditField("银行账号：",
    '<input class="edit-input easyui-combobox" id="Account" name="Account" panelheight="auto" ' +
    ((IsModify == 1 && PaymentID == "C07204") ? ' value="' + jsonData[0]["Account"] + '" ' : '') +
    'data-options="editable: false,valueField: \'Account\',textField: \'Account\'' +
    ',required:true,missingMessage:\'请选择账号\'' +
    (IsDetail ? ",readonly:true,hasDownArrow:false" : "") +
    '" />');
//返佣业务代付款
paymentDefine["C07205"] =
    EditField("供应商名称：",
    '<input class="edit-input easyui-combobox" id="Company" name="Company" panelheight="auto" ' +
    'data-options="url:\'/Page/P01041/GetSupplierList\',editable: false,valueField: \'SupplierID\',textField: \'SupplierNAME\'' +
    ',required:true,missingMessage:\'请选择供应商\'' +
    (IsDetail ? ",readonly:true,hasDownArrow:false" : "") +
    ',onSelect:function(data){$(\'#dgCommissionOrder\').datagrid({url:' +
    (IsDetail ? "'/Page/P01041/GetCommissionOrderProductDetail?ExpenditureID=" + ExpenditureID + "'" :
    "'/Page/P01041/GetCommissionOrder?SupplierID='+data.SupplierID+'&IsModify=" + IsModify + "&ExpenditureID=" + ExpenditureID + "'") +
    "});}" +
    ((IsModify == 1 && PaymentID == "C07205") ? ',onLoadSuccess:function(){$(this).combobox(\'select\', jsonData[0][$(this).attr(\'id\')]);}' : '') +
    '" />') +
    (IsDetail ? DataGrid("dgCommissionOrder", "", "",
    {
        "订单编号": {"field":"OrderID", "width":110},
        "产品分类": {"field":"ProductType", "width":80},
        "产品名称": {"field":"Product", "width":100},
        "退单金额": {"field":"Amount", "width":60},
        "备注": { "field": "Remark", "width":111 }
    },"") :
    DataGrid("dgCommissionOrder", "", "",
    {
        "": { "field": "AutoKey", "checkbox": true },
        "下单日期": { "field": "OrderDate", "width": 80 },
        "订单编号": { "field": "OrderID", "width": 110 },
        "客户名称": { "field": "CustomerName", "width": 80 },
        "产品名称": { "field": "Product", "width": 80 },
        "合同金额": { "field": "Amount", "width": 60 }
    }, ",onSelect:TotalAmount,onUnselect:TotalAmount,onSelectAll:TotalAmount,onUnselectAll:TotalAmount"+
    ((IsModify == 1 && PaymentID == "C07205") ? ",onLoadSuccess:function(){BindSelectedOrder('dgCommissionOrder');}" : "")));
//扣缴税费
paymentDefine["C07206"] =
    EditField("税种：",
    '<input class="edit-input easyui-combobox" id="Tax" name="Tax" panelheight="auto" panelMaxHeight="235" ' +
    ((IsModify == 1 && PaymentID == "C07206") ? ' value="' + jsonData[0]["Tax"] + '" ' : '') +
    'data-options="url:\'/Share/GetSystemCodes?code=C073\',editable: false,valueField: \'Name\',textField: \'Name\'' +
    ',required:true,missingMessage:\'请选择税种\'' +
    (IsDetail ? ",readonly:true,hasDownArrow:false" : "") +
    '" />');
//还贷款
paymentDefine["C07207"] =
    EditField("银行：",
    '<input class="edit-input easyui-textbox" id="Bank" name="Bank" ' +
    (IsDetail ? 'readonly="readonly "' : "") +
    ((IsModify == 1 && PaymentID == "C07207") ? ' value="' + jsonData[0]["Bank"] + '" ' : '') +
    'data-options="required:true,missingMessage:\'请填写银行\'' +
    '" />');
//其他
paymentDefine["C07208"] =
    EditField("收款方：",
    '<input class="edit-input easyui-textbox" id="Company" name="Company" ' +
    (IsDetail ? 'readonly="readonly "' : "") +
    ((IsModify == 1 && PaymentID == "C07208") ? ' value="' + jsonData[0]["Company"] + '" ' : '') +
    'data-options="required:true,missingMessage:\'请填写收款方\'' +
    '" />');