$(document).keydown(function (event) {
    var keyCode = event.keyCode;
    if (keyCode == "27") {
        cancel();
    }
});
$(document).ready(function () {
    toolHide();

    var gridID = "#" + $(".easyui-datagrid").attr("id");

    $(gridID).datagrid({
        onClickRow: function (rowindex, rowdata) {
            toolIsShow()
        },
        onSelect: function () {
            toolIsShow()
        },
        onCheck: function () {
            toolIsShow()
        },
        onCheckAll: function () {
            toolIsShow()
        },
        onUncheckAll: function () {
            toolIsShow()
        },
        onUncheck: function () {
            toolIsShow()
        },
        onUnselect: function () {
            toolIsShow()
        },
        onLoadSuccess: function () {
            toolHide();
        }
    });
});
/*start toolbar is enabled*/
function toolIsShow() {
    var gridID = "#" + $(".easyui-datagrid").attr("id");

    if ($(gridID).datagrid('getSelections').length == 0) {
        toolHide();
    } else {
        if ($(gridID).datagrid('getChecked').length == 1) {
            toolShow();
        }
        else {
            $(".easyui-linkbutton.toolradio").linkbutton({ disabled: true });
            $(".easyui-linkbutton.toolmultiple").linkbutton({ disabled: false })
        }
    }
}
function toolShow() {
    $(".easyui-linkbutton.toolradio").linkbutton({ disabled: false })
    $(".easyui-linkbutton.toolmultiple").linkbutton({ disabled: false })
}
function toolHide() {
    $(".easyui-linkbutton.toolradio").linkbutton({ disabled: true })
    $(".easyui-linkbutton.toolmultiple").linkbutton({ disabled: true })
}


function cancel() {
    $("#dialog").dialog('close')
}
function getControName(url)
{
    return url.substring(1, url.substring(1, url.length - 1).search('/') + 1);
}
function showError(error) {
    //$('.showMsg').show("slow", function () { $(this).hide(5000) });
    $("#showMsg").empty();
    $("#showMsg").append(error);
    setTimeout('$("#showMsg").empty()', 3000);
}

function showMessage(info) {
    $("#showMsg").empty();
    $("#showMsg").append(info);
    setTimeout('$("#showMsg").empty()',3000);
    //$('.showMsg').show("slow", function () { $(this).hide(5000) });
}

function showDialog(url, title, width, height) {
    $("#dialog").dialog({
        title: title,
        shadow: false,
        closed: false,
        cache: false,
        href: url,
        modal: true,
        width: width,
        height: height,
        onLoadSuccess: function (data) {

        }
    });
}

function autoCol()
{
    var headerTds = $(".datagrid-header-inner table tr:first-child").children();
    var bodyTds = $(".datagrid-body table tr:first-child").children();
    var totalWidth = 0;
    bodyTds.each(function (i, obj) {
        var headerTd = $(headerTds.get(i));
        var bodyTd = $(bodyTds.get(i));
        $("div:first-child", headerTds.get(i)).css("text-align", "center");
        var headerTdWidth = headerTd.width(); //获取第i个头部td的宽度

        var bodyTdWidth = bodyTd.width() + 5;
        var width = 0;
        if (headerTdWidth > bodyTdWidth) {
            width = headerTdWidth;
            bodyTd.width(width);
            headerTd.width(width);
            totalWidth += width;
        } else {
            width = bodyTdWidth;
            headerTd.width(width);
            bodyTd.width(width);
            totalWidth += width;
        }
    });
    var headerTable = $(".datagrid-header-inner table:first-child");
    var bodyTable = $(".datagrid-body table:first-child");
}

function funcForAppend(datagrid, result,form) {
    //$("#showMsg").empty();
    showMessage("新增成功");
    if (typeof (result.Data) == 'object' && isNaN(result.Data.length)) {
        $("#" + datagrid).datagrid('appendRow', result.Data);
    }
    else {
        $("#" + datagrid).datagrid('appendRow', result.Data[0]);
    }
    autoCol();
    $("#" + form).form('clear');
}
function funcForUpdate(datagrid, result) {
    showMessage("更新成功");
    //cancel();
    var index = $("#" + datagrid).datagrid('getRowIndex', $("#" + datagrid).datagrid('getSelected'));
    if (typeof (result.Data) == 'object' && isNaN(result.Data.length)) {
        $("#" + datagrid).datagrid('updateRow', { index: index, row: result.Data });
    }
    else {
        $("#" + datagrid).datagrid('updateRow', { index: index, row: result.Data[0] });
    }
   // autoCol()
    setTimeout("cancel()",3000);
}


function funcForAjax(url, method,form,data, datagrid,func)
{
    if ($("#"+form).form('validate')) {
        $.ajax({
            url: url,
            type: method,
            data: data,
            dataType:"json",
            success: function (result)
            {
                if (!result.HasError) {
                    func(datagrid, result,form);
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
function funcForQueryAjax(url, method,data, datagrid) {
    $.ajax({
        url: url,
        type: method,
        data:data,
        beforeSend: function () {
            cancel();
            var image = $("#progresImage");
            var mask = $("#maskOfProgressImage");
            image.show().css({
                "position": "fixed",
                "top": "50%",
                "left": "50%",
                "margin-top": function () { return -1 * image.height() / 2 },
                "margin-left": function () { return -1 * image.width() / 2 }
            });
            mask.show().css("opacity", "0.5");
        },
        complete: function () {
            $("#maskOfProgressImage").css('display', 'none')
            $('#progresImage').css('display', 'none')
        },
        success: function (data) {
            if (data.HasError == null) {
                $.messager.alert("提示", "没有检索到相应的信息");
            } else {
                if (!data.HasError) {
                    if (data.Data.length == 0) {
                        $("#" + datagrid).datagrid('loadData', data.Data);
                        $.messager.alert("提示", "没有检索到相应的信息");
                    } else {
                        $("#" + datagrid).datagrid('loadData', data.Data);
                        cancel();
                    }
                }
                else {
                    showError(data.ErrorMessage);
                }
            }
            autoCol();
        },
        error: function (d, error) {
            showError(error);
        }
    });
}
function getDeleteIDs(ids,rows,url,id)
{
    var selectCol = $("#"+id).datagrid('options').columns[0][0].field;
    for (var i = 0; i < rows.length; i++)
    {ids.push(rows[i][selectCol]) }

    return ids;
}
/*显示方式*/
function showActDlg_DGItemActDlg(id, title, url, width, height) {
    var selectRow = $("#" + id).datagrid('getSelected');
    var selectCol = $("#" + id).datagrid('options').columns[0][0].field;

    url = url + "?ID=" + selectRow[selectCol];
    showDialog(url, title, width, height);
}

function showActDlg(title, url, width, height) {
    showDialog(url, title, width, height)
}


/*按钮操作*/
function submitAct_DGAppend(form, url, datagrid) {
    funcForAjax(url, "post", form, $("#" + form).serialize(), datagrid, funcForAppend);
}

function submitAct_DGReplace(form, url, datagrid) {
 
    funcForQueryAjax(url, "post",$("#"+form).serialize(), datagrid);
}

function submitAct_DGUpdate(form, url, datagrid) {
    funcForAjax(url,"post",form,$("#"+form).serialize(),datagrid,funcForUpdate)
}
function showActDlg_DGItemsConfirm(datagrid, title, msg, url, width, height) {
    $.extend($.messager.defaults, { ok: "确定", cancel: "取消" })
    $.messager.confirm(title, msg, function (r) {
        if (r) {
            var ids = [];
            var rows = $("#" + datagrid).datagrid('getSelections');
            getDeleteIDs(ids,rows,url,datagrid)
            $.ajax({
                url: url + "?keys=" + ids,
                type: "post",
                success: function (result) {
                    if (!result.HasError) {
                        for (var i = 0; i < rows.length; i++)
                        {
                            var index = $("#" + datagrid).datagrid('getRowIndex', rows[i]);
                            $("#" + datagrid).datagrid('deleteRow', index);
                        }
                        $.messager.alert("提示", "已删除" + ids.length + "条记录");
                        
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

function downLoad(url) {
    $.download(url, 'find=commoncode', 'post');
}

