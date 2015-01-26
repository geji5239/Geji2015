function getPageName() {
    var pageName = window.location.pathname;
    if (pageName.substring(1, pageName.length - 1).search('/') == -1) { pageName = pageName.substring(1, pageName.length) }
    else {
        pageName = pageName.substring(1, pageName.substring(1, pageName.length - 1).search('/') + 1);
    }
    return pageName;
}
//创建datagrid
function CreateGrid(url, columns) {
    $("#datagrid").datagrid({
        url: url,
        method: 'get',
        checkOnSelect: true,
        selectOnCheck: true,
        columns: [columns],
        onLoadSuccess: function () {
            // $("input:checkbox").hide();
            $(".datagridid-header-check input:checkbox").hide();
            toolHide();
            //$(".linkbutton-detail").linkbutton({ text: '查看', plain: true, iconCls: 'icon-detail', width: 80 });
            //$(".linkbutton-edit").linkbutton({ text: '修改', plain: true, iconCls: 'icon-edit', width: 80 });
            //$(".linkbutton-delete").linkbutton({ text: '删除', plain: true, iconCls: 'icon-cancel', width: 80 })
        }
    });
}
//function formatOperDetail(val, row, index) {
//    return '<a href="javascript:void(0)" id="toolDetail" class="linkbutton-detail" onclick="detailsAction(' + index + ')"></a>';
//}
//function formatOperEdit(val, row, index)
//{
//    return '<a href="javascript:void(0)" id="toolEdit" class="linkbutton-edit" onclick="editAction(' + index + ')">修改</a>';
//}
//function formatOperDelete(val, row, index) {
//    return '<a href="javascript:void(0)" id="toolDelete" class="linkbutton-delete" onclick="detail(' + index + ')"></a>';
//}
//控制toolBar的显示条件
function toolIsShow() {
    if ($("#datagrid").datagrid('getSelected') == null) {
        toolHide();
    } else {
        if ($("#datagrid").datagrid('getChecked').length == 1) {
            toolShow();
        }
        else {
            toolShow();
            $("#toolEdit").hide();
            $("#toolDetail").hide();
        }
    }
}
function toolShow() {
    $("#toolDetail").show();
    $("#toolEdit").show();
    $("#toolDelete").show();
}
function toolHide() {
    $("#toolDetail").hide();
    $("#toolEdit").hide();
    $("#toolDelete").hide();
}
function addAction() {
    dialogShow("dialog", '/' + getPageName() + '/Add', "添加");
}
function searchAction() {
    dialogShow("dialog", '/' + getPageName() + '/Searching', "检索")
}
function importAction() {
    dialogShow("dialog", '/' + getPageName() + '/Import', "导入")
}
function detailsAction() {
   // $('#datagrid').datagrid('selectRow',index);
    var row = $("#datagrid").datagrid('getSelected');
    if(row){
        dialogShow("dialog", '/' + getPageName() + '/Details?id=' + row.Company_ID, "查看");
    }
}
function editAction() {
    //$('#datagrid').datagrid('selectRow', index);
    var row = $("#datagrid").datagrid('getSelected');
    dialogShow("dialog", '/' + getPageName() + '/Editing?id=' + row.Company_ID, "编辑")
}
function dialogShow(name, url, title) {
    $("#" + name).dialog({
        title: title,
        shadow: false,
        closed: false,
        cache: false,
        href: url,
        modal: true,
        onLoadSuccess:function(data){  
            $('.easyui-linkbutton').linkbutton({ text: '编辑', plain: true, iconCls: 'icon-edit' });
        }  
    });
}
//动态生成添加界面的控件
function addToControl(obj) {
    $("#addForm").empty();
    $.each(obj, function (index, item) {
        var control = null;
        var title = $("<label class='label-title'>");
        switch (item.type) {
            case "textbox": control = createTextbox(); break;
            case "select": control = createSelect(); break;
            case "textarea": control = createTextarea(); break;
        }
        if (item.title != null) {
            title.text(item.title);
        }
        if (control != null) {
            control = setStyle(control, item);
            $("#addForm").append(title);
            $("#addForm").append(control);
            $("#addForm").append("<br/>")
        }
    })
}
function createTextbox() {
   // return $("<input class='easyui-validatebox' data-options=\"required:true,validType:'email'\"/>");   
    return $("<input class='input-dialog'/>");
}
function createTextarea() {
    return $("<textarea class='input-dialog' ></textarea>");
}
function setStyle(control, item) {
    if (item.width != 0) {
        control.size = item.width;
    }
    if (item.name != null) {
        control.attr("name", item.name);
    }
    if (item.validtype != null)
    {
        control.attr("class", "easyui-validatebox");
        control.attr("data-options", "required:true")
        //control.attr("required", item.validtype.Required);
    }
    return control;
}
//关闭dialog
function cancel() {
    $("#dialog").dialog('close');
}
/*  temp*/
function frmAddAjax(form) {
    formAjax(form,"Add",formAddFunc)
}
function frmEditAjax(form)
{
    formAjax(form,"Editing",formEditFunc)
}
function formAjax(form, action, func)
{
    $.ajax({
        url: "/" + getPageName() + "/"+action,
        type: "post",
        dataType: "json",
        data: $(form).serialize(),
        success: function (data)
        {
            func(data);
        },
        error: function (xmlhttprequest, text, error) {
            alert(error)
        }
    });
}
/*  temp*/

/*form action*/ 
function formEditFunc(data)
{
    var result = $.parseJSON($.parseJSON(data));
    if (!result.Error.HasError) {
        $("#dialog").dialog('close');
        //$("#datagrid").datagrid('reload');
        $
        var data = $("#datagrid").datagrid("getData");
        //alert(data.rows[3].Company_ID)
    }
    else {
        showError(result.Error.ErrorMessage);
    }
}
function formAddFunc(data)
{
    var result = $.parseJSON($.parseJSON(data));
    if (!result.Error.HasError) {
        $("#datagrid").datagrid('appendRow', result.Company);
    }
    else {
        showError(result.Error.ErrorMessage);
    }
}
/*end form action*/
function showError(error)
{
    $("#showMsg").empty();
    $("#showMsg").append(error);
}
function findCompany()
{
   var name= $("input[name='Company_Name']").val();
   var state= $("#company-state").combobox('getValue');
    $.ajax({
        url: "/" + getPageName() + "/Find",
        type: "post",
        data: {Company_Name:name,Is_Active:state},
        success: function (findresult)
        {
            var result = $.parseJSON(findresult);
    
            if (!result.Error.HasError)
            {
                if (result.Company.length==0) {
                    showError("不存在相应条件的信息");
                }
                else {
                    $("#datagrid").datagrid('loadData', result.Company);
                    $("#dialogSearch").window('close');
                }
            }
            else
            {
                showError(error.ErrorMessage)
            }
        },
        error: function (xmlhttprequest, text, error)
        {

        }
    });
}
function addInfo() {
    $("form.frmAdd").form('submit', {
        url:"/" + getPageName() + "/Add",
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            formAddFunc(result)
            $("form.frmAdd").form('clear');
        }
    });
}
function editCompany(id)
{
    $("#editForm").form('submit', {
        url: "/" + getPageName() + "/Edited?id=" + id,
        onSubmit: function () {
            return $(this).form('validate');
        },
        success: function (result) {
            formEditFunc(result)
            $("form.frmAdd").form('clear');
        }
    })
}