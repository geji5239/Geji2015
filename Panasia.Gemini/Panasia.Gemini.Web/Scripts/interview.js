//用途：1.用于人员招聘模块 面试流程     2.客户管理
//2014-10-25 12:43 李晓彤

//----------------------------------------------------------面试流程-------------------------------------------------------------------

//创建预约
function showActDlg_DGItems(datagrid, title, msg, url, width, height) {
    checkOnline()
    var ids = [];
    var rows = $("#" + datagrid).datagrid('getSelections');
    getDeleteIDs(ids, rows, url, datagrid);
    var log = 0;
    for (var i = 0; i < rows.length; i++) {
        if (rows[i]["Done"].trim() != '') {
            log = rows[i]["Done"].trim();
            break;
        }
    }
    if (log == "已创建预约") {
        $.messager.alert("提示", "已存在面试信息,无需再次预约");
        return;
    }
    else if (log == "已转员工") {
        $.messager.alert("提示", "已转员工,预约失败");
        return;
    }
    else {
        $.extend($.messager.defaults, {
            ok: "确定", cancel: "取消"
        })
        $.messager.confirm(title, msg, function (r) {
            if (r) {
                $.ajax({
                    url: url + "?keys=" + ids,
                    type: "post",
                    success: function (result) {
                        if (!result.HasError) {
                            $.messager.alert("提示", "预约成功");
                            var rows = $("#" + datagrid).datagrid('getSelections');
                            for (var i = 0; i < result.Data.length; i++) {
                                var indexs = $("#" + datagrid).datagrid('getRowIndex', rows[i]);
                                $("#" + datagrid).datagrid('updateRow', {
                                    index: indexs, row: result.Data[i]
                                });
                                toolIsShow();//刷新tool按钮状态
                            }
                        }
                        else {
                            $.messager.alert("提示", result.ErrorMessage);
                        }
                    },
                    error: function (xmlhttprequest, text, error) {
                        showError(error);
                    }
                });
            }

            else {
                cancel();
            }
        });
    }
}

//预约面试邮件发送界面
function showActDlg_DG_YuYueEmails(datagrid, title, url, width, height) {
    checkOnline()
    var ids = [];
    var flag = 0;
    var msg = [];
    var rows = $("#" + datagrid).datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {
        if (rows[i]['AppointmentRresult'].trim() == '预约成功') {
            if (rows[i]["AuditionRresult"].trim() == '通过' || rows[i]["AuditionRresult"].trim() == '未通过')
            { msg = "面试已结束，不能发送预约邮件！"; flag = 1; break; }
            else ids.push(rows[i]['RID']);
        }
        else { msg = "面试预约未成功，不能发送预约邮件！"; flag = 1; break; }
    }
    if (flag == 0) {
        url = url + "?keys=" + ids;
        showDialog(url, title, width, height);
    }
    else {
        $.messager.alert("提示", msg);
    }
}

//通知报到邮件发送界面
function showActDlg_DG_BaoDaoEmails(datagrid, title, url, width, height) {
    checkOnline()
    var ids = [];
    var flag = 0;
    var msg = [];
    var rows = $("#" + datagrid).datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {
        if (rows[i]['Noticed'].trim() == '同意报到') {
            if (rows[i]["IsToEmployee"].trim() == '已转员工')
            { msg = "已转员工，不能发送通知报到邮件！"; flag = 1; break; }
            else ids.push(rows[i]['RID']);
        }
        else { msg = "通知报到未成功，不能发送通知报到邮件！"; flag = 1; break; }
    }
    if (flag == 0) {
        url = url + "?keys=" + ids;
        showDialog(url, title, width, height);
    }
    else {
        $.messager.alert("提示", msg);
    }
}
//邮件发送
function funcForSendEmail(data, url, datagrid) {
    checkOnline()
    $.ajax({
        url: url,
        type: "post",
        data: data,
        dataType: "json",
        success: function (result) {
            if (!result.HasError) {
                $.messager.alert('提示', '邮件发送成功！')
                var rows = $("#" + datagrid).datagrid('getSelections');
                for (var i = 0; i < result.Data.length; i++) {
                    var indexs = $("#" + datagrid).datagrid('getRowIndex', rows[i]);
                    $("#" + datagrid).datagrid('updateRow', { index: indexs, row: result.Data[i] });
                }
                toolIsShow();//刷新tool按钮状态
                setTimeout("cancel()", 3000);
            }
            else {
                showError(result.ErrorMessage);
            }
        },
        error: function (d, error) {
            showError(error);
        }
    });
}
//添加预约结果 李晓彤 2014-10-23
function showActDlg_DGItemActDlgYuYue(datagrid, title, url, width, height) {
    checkOnline()
    var rows = $("#" + datagrid).datagrid('getSelected');
    if (rows["Done"] == 0) {
        $.messager.alert("提示", "请先创建预约信息");
    }
    else if (rows["AuditionRresult"].trim() == '未通过' || rows["AuditionRresult"].trim() == '通过') {
        $.messager.alert("提示", "面试已结束，预约信息无法修改");
    }
    else {

        url = url + "?ID=" + rows["RID"];
        showDialog(url, title, width, height);
        toolIsShow();//刷新tool按钮状态
    }
}

//面试结果录入 李晓彤 2014-10-23
function showActDlg_DGItemActDlgMianShi(datagrid, title, url, width, height) {
    checkOnline()
    var rows = $("#" + datagrid).datagrid('getSelected');
    if (rows["AuditRresult"].trim() == '未通过' || rows["AuditRresult"].trim() == '通过') {
        $.messager.alert("提示", "审核已结束，面试信息无法修改");
    }
    else {

        url = url + "?ID=" + rows["RID"];
        showDialog(url, title, width, height)
    }
}

//入职审核 李晓彤 2014-10-23
function showActDlg_DGItemActDlgShenHe(datagrid, title, url, width, height) {
    checkOnline()
    var rows = $("#" + datagrid).datagrid('getSelected');
    if (rows["Noticed"].trim() == '同意报到' || rows["Noticed"].trim() == '不同意报到') {
        $.messager.alert("提示", "通知报到已结束，审核信息无法修改");
    }
    else {
        url = url + "?ID=" + rows["RID"];
        showDialog(url, title, width, height)
    }
}

//通知报到 李晓彤 2014-10-23
function showActDlg_DGItemActDlgBaoDao(datagrid, title, url, width, height) {
    checkOnline()
    var rows = $("#" + datagrid).datagrid('getSelected');
    if (rows["IsToEmployee"].trim() == '已转员工') {
        $.messager.alert("提示", "已转员工，信息无法修改");
    }
    else {
        url = url + "?ID=" + rows["RID"];
        showDialog(url, title, width, height)
        toolIsShow();//刷新tool按钮状态
    }
}

//转员工 李晓彤 2014-10-23
function showActDlg_DGItemActDlgToEmployee(datagrid, title, url, width, height) {
    var rows = $("#" + datagrid).datagrid('getSelected');
    if (rows["IsToEmployee"].trim() == '已转员工') {
        $.messager.alert("提示", "已转员工，无需再次操作");
    }
    else {
        url = url + "?ID=" + rows["RID"];
        showDialog(url, title, width, height)
    }
}
//清理表中数据（无参数）
function showActDlg_DGItemsClearData(datagrid, title, msg, url) {
    checkOnline()
    $.messager.confirm(title, msg, function (r) {
        if (r) {
            $.ajax({
                url: url,
                type: 'post',
                success: function (result) {
                    if (!result.HasError) {
                        var rows = result.Data;
                        $.messager.alert('提示', '已清理' + rows.length + '条数据');
                        var gridrows = $("#" + datagrid).datagrid('getRows')
                        for (var i = 0; i < gridrows.length; i++) {
                            for (var j = 0; j < rows.length; j++) {
                                if (gridrows[i]['RID'] == rows[j].RID) {
                                    $('#' + datagrid).datagrid('updateRow', {
                                        index: i,
                                        row: {
                                            IsToEmployee: '已无效'
                                        }
                                    });
                                    break;
                                }
                            }
                        }
                    }
                }
            })
        }
    })
}
//结束招聘
function showActDlg_DGItemsOver(datagrid, title, msg, url, width, height) {
    checkOnline()
    $.extend($.messager.defaults, { ok: "确定", cancel: "取消" })
    $.messager.confirm(title, msg, function (r) {
        if (r) {
            var ids = [];
            var rows = $("#" + datagrid).datagrid('getSelections');
            getDeleteIDs(ids, rows, url, datagrid)
            $.ajax({
                url: url + "?keys=" + ids,
                type: "post",
                success: function (result) {
                    if (!result.HasError) {
                        $.messager.alert("提示", "操作成功！");
                        var rows = $("#" +datagrid).datagrid('getSelections') ;
                        for (var i = 0; i < result.Data.length; i++) {
                            var indexs = $("#" +datagrid).datagrid('getRowIndex', rows[i]);
                            $("#" +datagrid).datagrid('updateRow', {index: indexs, row: result.Data[i]});
                    }
                        toolHide();
                    }
                    else {
                        $.messager.alert("提示", result.ErrorMessage);
                    }
                },
                error: function (xmlhttprequest, text, error) {
                showError(error);
                }
            });
        }
        else {
            cancel();
        }
        });
}
//员工建档-创建账户  李晓彤 2014-11-21
function showActDlg_DGItemCreateUser(datagrid, title, msg, url) {
    checkOnline()
    $.messager.confirm(title, msg, function (r) {
        if (r) {
            var rows = $("#" + datagrid).datagrid("getSelections");
            var selectCol = $("#" +datagrid).datagrid('options').columns[0][0].field;
            var error = 0;
            var errorMsg = '';
            for (var i = 0; i < rows.length; i++) {
                $.ajax({
                    url: url + '?ID=' + rows[i]['EmployeeID'],
                    type: 'post',
                    success: function (result) {
                        if (!result.HasError) {
                            $("#" + datagrid).datagrid('reload');
                            $('#' + datagrid).datagrid('updateRow', {
                                index: $("#" + datagrid).datagrid("getRowIndex", rows[i]),
                                row: result.Data[0]
                            });
                            toolIsShow();//刷新tool按钮状态
                        }
                        else {
                            error = 1;
                            errorMsg = result.ErrorMessage;
                        }
                    }
                })
                if (error == 1)
                    break;
            }
            if (error == 0) {
                $.messager.alert('提示', '创建账户成功!');
            }
            else {
                $.messager.alert('提示', errorMsg);
            }
        }
    });
}




//----------------------------------------------------------客户管理-------------------------------------------------------------------

//批量编辑（多字段） 李晓彤2014-11-11(没用上)
function showActDlg_DGItems_ActDlg(datagrid, colunms, title, url, width, height) {
    url = url + "?"
    var items = colunms.split(",");
    var rows = $("#" + datagrid).datagrid('getSelections');
    for (var i = 0; i < items.length; i++) {
        var colunmName = [];
        for (var j = 0; j < rows.length; j++) {
            colunmName.push(rows[j][items[i]]);
        }
        url = url + items[i] + "=" + colunmName;
        if (i + 1 < items.length)
            url = url + "&";
    }
    alert(url);
    showDialog(url, title, width, height)
}


//打开对话框操作数据，保存后表中数据隐藏（title为操作方法）李晓彤2014-11-12（管理员公海领取）
function submitAct_DGOperate(form, title, url, datagrid) {
    checkOnline()
    if ($("#" + form).form('validate')) {
        var rows = $("#" + datagrid).datagrid('getSelections');
        $.ajax({
            url: url,
            type: "post",
            data: $("#" + form).serialize(),
            dataType: "json",
            success: function (result) {
                if (!result.HasError) {
                    cancel();
                    for (var i = 0; i < rows.length; i++) {
                        var index = $("#" + datagrid).datagrid('getRowIndex', rows[i]);
                        $("#" + datagrid).datagrid('deleteRow', index);
                    }
                    toolHide();
                    //if (title.charAt(0) == ' ') {
                    //    $.messager.alert("提示", title);
                    //}
                    //else {
                    //    $.messager.alert("提示", "已" + title + rows.length + "条记录");//李晓彤 2014光棍节，打开对话框对数据进行操作（操作完不显示）也可以引用
                    //}
                    $.messager.alert("提示", "已" + title + rows.length + "条记录");//李晓彤 2014光棍节，打开对话框对数据进行操作（操作完不显示）也可以引用
                }
                else {
                    showError(result.ErrorMessage);
                }
            },
            error: function (d, error) {
                showError(error);
            }
        });
    }
}
    //带有上限个数的新增 李晓彤 2014-11-13
function showActDlg_LimitAdd(title, msg, url, limitUrl, limitColumnName) {
    checkOnline()
    $.ajax({
        url: limitUrl,
        type: 'post',
        success: function (result) {
            if (!result.HasError) {
                var num = result.Data[0][limitColumnName];
                if (num <= 0)
                    $.messager.alert('提示', msg, 'info');
                else
                    showDialog(url, title, 400, 300)
            }
            else {
                $.messager.alert("提示", result.ErrorMessage);
            }
        }
    })
}
    //带有上限个数的公海领取 李晓彤 2014-11-13
function showActDlg_DGItemsCheckResidue(datagrid, title, msg, url) {
    checkOnline()
    var rows = $("#" + datagrid).datagrid('getSelections');
    $.ajax({
        url: '/Page/P01072/GetResidue',
        type: 'post',
        success: function (result) {
            if (!result.HasError) {
                var num = result.Data[0]['Residue'];
                if (rows.length > num) {
                    $.messager.alert('提示', '新增及领取客户已达上限，无法选取！', 'info');
                }
                else {
                    showActDlg_DGItemsConfirm(datagrid, title, msg, url, 400, 300)
                }
            }
            else {
                $.messager.alert("提示", result.ErrorMessage);
            }
        }
    });
}

function changeDG(datagrid) {//测试用
    alert("45")
    var rows = $("#" + datagrid).datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {
        alert(rows[i].RID)
        var index = $("#" + datagrid).datagrid('getRowIndex', rows[i].RID);
        alert(index)
        $('#' + datagrid).datagrid('updateRow', {
            index: index,
            row: {
                AuditRresult: '新消息'
            }
        });
    }
}

    //----------------------------------------------------------固定资产-------------------------------------------------------------------
var reload_dataPanDian = {};
function submitAct_DGReplacePanDian(form, url, datagrid) {
    reload_dataPanDian = $("#" + form).serialize();
    if ($("#" + form).form('validate')) {
        var grid = $("#" + datagrid);
        if (grid.datagrid('options').url != null) {
            grid.datagrid('load', serializeObject($("#" + form).form()));
            grid.datagrid({
                onLoadSuccess: function (data) {
                    if (data.total == 0) {
                        grid.datagrid('options').queryParams = '';
                        $('#btn-export').linkbutton('disable');
                    }
                    else {
                        $('#btn-export').linkbutton('enable');
                    }
                }
            });
        }
        else {
            grid.datagrid({
                url: url,
                queryParams: serializeObject($("#" + form).form()),
            });
        }
        cancel();
        toolHide();
    }
}
    //导出盘点文件
function funcExportPanDian(url) {
    console.log(reload_dataPanDian)
    $.download(url, reload_dataPanDian, 'post');
    //$.ajax({
    //    url: url,
    //    type: 'post',
    //    data: reload_dataPanDian,
    //    success: function (result) {
    //        console.log('4545')
    //    //    var data = $.parseJSON(result);
    //    //    if (data.HasError) {
    //    //        if (data.ErrorMessage == '') {


    //    //        }
    //    //    }
    //    },
    //    error: function (d, error) {
    //        showError(error);
    //    }
    //});
}