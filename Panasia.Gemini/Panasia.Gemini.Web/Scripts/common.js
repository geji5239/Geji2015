$(document).keydown(function (event) {
    var keyCode = event.keyCode;
    switch (keyCode) {
        case 27: { cancel(); } break;
        //case 13: { console.log($(".panel.window>.edit-button")) } break;
    }
});
$(window).resize(function () {
    //$(".edit-dialog1").css("max-height", ($("#tabs").height() - 27) + 'px')
    //var obj ,max= '';
   
    //if ($(".edit-dialog") != [])
    //{
    //    obj = $(".edit-dialog");
        
    //    if ($("#tabs").height() - 27 > 0)
    //    {
    //        max = $("#tabs").height() - 27;
    //    }
    //    console.log(max)
    //    console.log(obj)
    //    if (obj == []) obj = $(".edit-dialog");
    //    console.log(obj)
    //    obj.css("max-height", max+'px');
    //}
   //console.log($("#dialog").height())
})
var dialogid = "dialog";

//-----------扩展dialog提交时的遮罩效果----------
$.extend($.fn.dialog.methods, {
    lock: function (jq,msg) {
        return jq.each(function () {
            var panel = $(this).dialog('dialog');
            if (msg == undefined)
            {
                msg = "正在提交数据请稍后......";
            }
            $("<div class=\"datagrid-mask\"></div>").css({ display: "block", width: panel.width(), height: panel.height() }).appendTo(panel);
            $("<div class=\"datagrid-mask-msg\"></div>").html(msg).appendTo(panel).css({ display: "block", left: (panel.width() - $("div.datagrid-mask-msg", panel).outerWidth()) / 2, top: (panel.height() - $("div.datagrid-mask-msg", panel).outerHeight()) / 2 });
        })
    },
    unlock: function (jq)
    {
        return jq.each(function () {
            var panel = $(this).dialog('dialog');
            panel.find("div.datagrid-mask-msg").remove();
            panel.find("div.datagrid-mask").remove();
        })
    }

})

//-----------新增员工界面上传图片----
var EmployeeUtil = {
    ratio: 1,
    view_H: 130,
    view_W: 130,
    initialize: function (path) {
        var img = new Image();

        if (img.width == 0) {
            var obj = this;

            img.onload = function () {
                obj.imgOperate(img);
            };
        } else {
            this.imgOperate(img);
        }
        img.src = path;
        $("#EmployeeImg").attr("src", path);
    },
    imgOperate: function (img) {
        if (img) {
            this.resize('EmployeeImg', img.width, img.height, 130, 130);
            var x = 0, y = 0, size = 0;
            if (this.view_W > this.view_H) {
                x = (this.view_W - this.view_H) / 2;
                size = this.view_H;
            } else if (this.view_W < this.view_H) {
                y = (this.view_H - this.view_W) / 2;
                size = this.view_W;
            } else {
                size = this.view_W;
            }
            var obj = this;
            $('img#EmployeeImg').imgAreaSelect({
                aspectRatio: "1:1",
                handles: "corners",
                persistent: true,
                show: true,
                imageWidth: obj.view_W,
                imageHeight: obj.view_H,
            });
        }
    }
};
function upImg() {
    $("#file_form").form('submit', {
        onSubmit: function () {
            if ($('#file').val() == "") {
                $.messager.alert("提示", "没有数据！", 'info');
                return false;
            }
        },
        success: function (data) {
            var path = "/EmployeeImg/Temp/" + eval(data);
            $("input[name='PhotoPath']").val(path);
            EmployeeUtil.initialize(path);
        }
    });
}   
function oncheckstate() {
        toolIsShow();
    }
//-----------新增员工界面上传图片----end
$(document).ready(function () {
    toolHide();
    if ($(".datebox :text").length != 0) {
        $(".datebox :text").attr("readonly", "readonly");//日期选择框只读
    }
    $(".edit-dialog").css("max-height", ($("#tabs").height() - 27) + 'px')
    //if ($(".easyui-datagrid").attr("id") != undefined) {
    //    var gridID = "#" + $(".easyui-datagrid").attr("id");
    //    $(gridID).datagrid({
    //        onClickRow: function (rowindex, rowdata) {
    //            toolIsShow()
    //        },
    //        onSelect: function () {
    //            toolIsShow()
    //        },
    //        onCheck: function () {
    //            toolIsShow()
    //        },
    //        onCheckAll: function () {
    //            toolIsShow()
    //        },
    //        onUncheckAll: function () {
    //            toolIsShow()
    //        },
    //        onUncheck: function () {
    //            toolIsShow()
    //        },
    //        onUnselect: function () {
    //            toolIsShow()
    //        },
    //        onLoadSuccess: function () { }
    //    });
    //}
    
});
/*start toolbar is enabled*/
function toolIsShow() {
    var gridID = "#" + $(".easyui-datagrid").attr("id");
    if ($(gridID).datagrid('getSelections').length == 0) {
        toolHide();
    } else {
        if ($(gridID).datagrid('getSelections').length == 1) {
            toolShow();
        }
        else {
            toolHide()
            // $(".easyui-linkbutton.toolradio").linkbutton({ disabled: true });
            $(".easyui-linkbutton.toolmultiple").linkbutton({ disabled: false })
        }
    }

    HideConfig();
}
function toolShow() {
    $(".easyui-linkbutton.toolradio").linkbutton({ disabled: false })
    $(".easyui-linkbutton.toolmultiple").linkbutton({ disabled: false })
}

/*datagrid数据行选中和不选中编辑，删除按钮的隐藏*/
function toolHide() {
    $(".easyui-linkbutton.toolradio").linkbutton({ disabled: true })
    $(".easyui-linkbutton.toolmultiple").linkbutton({ disabled: true })
}

//使用css类名配置简单的识别状态控制按钮显示与否
//类名格式hide_字段名_值1_值2_值3，值可以有任意个
//如果字段在SQL查询时做了文字处理，可以在列表上加一列隐藏列存放原值
//杨磊 2014-9-29
function HideConfig() {
    $(".easyui-linkbutton.normal-button[class*='hide_']").each(function () {
        if (!$.data(this, "linkbutton").options.disabled) {
            var className = $(this).attr("class");
            //var hide = className.match(/\bhide_.+?\s/);
            var hide = className.split(" ");
            for (var k = 0; k < hide.length; k++) {
                if (hide[k].length > 5 && hide[k].trim().substring(0, 5) == 'hide_') {
                    var items = hide[k].split("_");
                    var field = items[1];
                    var gridID = "#" + $(".easyui-datagrid").attr("id");
                    var rows = $(gridID).datagrid('getSelections');
                    for (var i = 0; i < rows.length; i++) {
                        for (var j = 2; j < items.length; j++) {
                            if (rows[i][field] == items[j].trim()) {
                                $(this).linkbutton({ disabled: true });
                                break;
                            }
                        }
                    }
                }
            }
        }
    });
}

/*取消对话框*/
function cancel() {
    $("#dialog").dialog('close');
    $("#dialog").children().remove();
    $(".combo-panel").hide();
}
function getControName(url) {
    return url.substring(1, url.substring(1, url.length - 1).search('/') + 1);
}

/*错误提示信息*/
function showError(error) {
    $("#showmsg").empty();
    $("#showmsg").append(error);
   // console.log($('#'+dialogid).height())
    setTimeout('$("#showmsg").empty()', 3000);
}

/*成功提示信息*/
function showMessage(info) {
    $("#showmsg").empty();
    $("#showmsg").append(info);
    setTimeout('$("#showmsg").empty()', 3000);
}

/*弹出对话框*/
function showDialog(url, title, width, height, dialogID) {
   
    if (dialogID != undefined)
    {
        dialogid = dialogID;
    }
    $("#" + dialogid).dialog({
        title: title,
        shadow: false,
        closed: false,
        cache: false,
        href: url,
        modal: true,
        width: width,
        height: $("#tabs").height() - 27,
        onLoad: function () {
            $(".datebox :text").attr("readonly", "readonly");
            $("#" + dialogid).dialog('center');//居中
            //$('.easyui-textbox:first').combobox().next('span').find('input').focus()//通用设置第一个input焦点
          //  $(".easyui-textbox[readonly!='readonly']:first").next('span').find('input').focus()//.combobox()会把第一个变成combobox，是失误还是有特别的用意？加入只读过滤  --杨磊 2014-9-18
        },
        onLoadError: function () {
            $(this).dialog('close');
            $.messager.alert("提示", "访问页面错误，请稍后重试！");
        }
      
    });
}

function autoColumn(datagrid) {
    $("#" + datagrid).datagrid('fixColumnSize');//自适应宽度 2014-10-13 ysh
    var ss = $("#" + datagrid).datagrid('getColumnFields');
    for (i = 0; i < ss.length; i++) {
        $("#" + datagrid).datagrid('autoSizeColumn', this.field);
    }
}


function funcForAppend(datagrid, result, form) {
    showMessage("新增成功");
    if (canAdd == 1) { $('#btn-create').linkbutton('disable'); canAdd = 0; }//某些申请单只能申请一次，有数据新增按钮就不可用
    //if (typeof (result.Data) == 'object' && isNaN(result.Data.length)) {
    //    $("#" + datagrid).datagrid('appendRow', result.Data);
    //}
    //else {
    //    $("#" + datagrid).datagrid('appendRow', result.Data[0]);
    //}
    // autoColumn(datagrid);

    //$("#" + form).form('clear');
    $("#" + form).form('reset');//reset不会清除属性只读的值 2014-10-11 ysh
   // $("input[type='hidden']").val('');
   // $("input:hidden").val('');
}

function funcForUpdate(datagrid, result, form) {
    showMessage("更新成功");
    //20141210 fgh 注释采用新的数据加载方式
    //var index = $("#" + datagrid).datagrid('getRowIndex', $("#" + datagrid).datagrid('getSelected'));
    //if (typeof (result.Data) == 'object' && isNaN(result.Data.length)) {
    //    $("#" + datagrid).datagrid('updateRow', { index: index, row: result.Data });
    //}
    //else {
    //    var rows = $("#" + datagrid).datagrid('getSelections');
    //    for (var i = 0; i < result.Data.length; i++) {
    //        var indexs = $("#" + datagrid).datagrid('getRowIndex', rows[i]);
    //        $("#" + datagrid).datagrid('updateRow', { index: indexs, row: result.Data[i] });
    //    }
    toolHide();//刷新tool按钮状态
    //    //autoColumn(datagrid);
        setTimeout("cancel()", 3000);//编辑成功3秒自动关闭对话框 
    //}
}
function funcForAddOrUpdate(ids, url, method, form, data, datagrid) {
    if ($("#" + form).form('validate')) {
        $.ajax({
            url: url,
            type: method,
            data: data,
            dataType: "json",
            success: function (result) {
                if (!result.HasError) {
                    var rows = $('#' + datagrid).datagrid('getRows');

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
function funcForAjax(url, method, form, data, datagrid, func) {
    checkOnline();
    if ($("#" + form).form('validate')) {
        $.ajax({
            url: url,
            type: method,
            data: data,
            dataType: "json",
            beforeSend: function () {
                $(".easyui-linkbutton.edit-button").linkbutton({ disabled: true });
                $("#" + dialogid).dialog('lock')
            },
            complete: function () {
                $(".easyui-linkbutton.edit-button").linkbutton({ disabled: false });
                $("#" + dialogid).dialog('unlock')
            },
            success: function (result) {
                if (!result.HasError) {
                    func(datagrid, result, form);
                    // $("#" + datagrid).datagrid('reload',{queryParams: '' }); //fgh 2014-12-10 add      
                    $("#" +datagrid).datagrid('reload');
                }
                else {
                    $.messager.alert('提示', result.ErrorMessage); 
                }
            },
            error: function (d, error) {
                showError(error);
            }
        });
    }
}
function serializeObject(form) {/*将form表单元素的值序列化成对象*/
    var o = {};
    $.each(form.serializeArray(), function (index) {
        if (o[this['name']]) {
            o[this['name']] = o[this['name']] + "," + this['value'];
        }
        else {
            o[this['name']] = this['value'];
        }
    });
    return o;
}
function funcForQueryAjax(url, method, form, datagrid) {
    checkOnline();
    var flag = 0;
    var grid = $("#" + datagrid);
    if (grid.datagrid('options').url != null) {
        grid.datagrid('load', serializeObject($("#" + form).form()));
        grid.datagrid({
            onLoadSuccess: function (data) {
                if (data.total == 0 && flag == 0) {
                    grid.datagrid('options').queryParams = '';
                    $.messager.alert("提示", "没有查询到符合条件的记录！");
                }
                flag++;
            }
            });
            }
            else {
                grid.datagrid({
                url: url,
            queryParams: serializeObject($("#" +form).form()),
        });
    }
    cancel();
    toolHide();
   // var opts = $("#" + datagrid).datagrid('options');

    // data = data + "&page=" + opts.pageNumber + "&pageSize=" + opts.pageSize + "&sortName=" + opts.sortName + "&sortOrder=" + opts.sortOrder;//分页参数传入todo 2014-10-15 ysh

    //$.ajax({
    //    url: url,
    //    type: method,
    //    data: data,
    //    beforeSend: function () {
    //        cancel();
    //        $("#" + datagrid).datagrid('loading');//更改检索后的加载效果
    //    },
    //    complete: function () {
    //        $("#" + datagrid).datagrid('loaded');//更改检索后的加载效果
    //    },
    //    success: function (data) {
    //        if (data.HasError == null) {
    //            $.messager.alert("提示", "没有查询到符合条件的记录！");
    //        } else {
    //            if (!data.HasError) {
    //                if (data.Data.length == 0) {
    //                    $("#" + datagrid).datagrid({ loadFilter: pagerFilter }).datagrid('loadData', { total: 0, rows: [] });
    //                    //$("#" + datagrid).datagrid('clear');
    //                    $.messager.alert("提示", "没有检索到相应的信息");
    //                } else {
    //                    $("#" + datagrid).datagrid({ loadFilter: pagerFilter }).datagrid('loadData', data.Data);//实现分页的数据加载 王 2014-9-30
    //                    $("#" + datagrid).datagrid('reload');
    //                    //$("#" + datagrid).datagrid('loadData', data.Data);
    //                    cancel();
    //                }
    //            }
    //            else {
    //                // showError(data.ErrorMessage);
    //                $.messager.alert('提示', data.ErrorMessage);//检索后出错的弹出 王 2014-10-15 
    //            }
    //        }
    //    },
    //    error: function (d, error) {
    //        showError(error);
    //    }
    //});
}

function getDeleteIDs(ids, rows, url, id) {
    var selectCol = $("#" + id).datagrid('options').columns[0][0].field;
    for (var i = 0; i < rows.length; i++)
    { ids.push(rows[i][selectCol]) }

    return ids;
}
/*显示方式*/
function showActDlg_DGItemActDlg(id, title, url, width, height) {
    var selectRow = $("#" + id).datagrid('getSelected');
    var selectCol = $("#" + id).datagrid('options').columns[0][0].field;

    url = url + "?ID=" + selectRow[selectCol];

    showDialog(url, title, width, height);
}


function showActDlg(title, url, width, height, dialogID) {
    showDialog(url, title, width, height, dialogID);
}


function showtree() {
    //var boarddiv = "<tr><td colspan=6><div class=\"edit-panel3\" style=\"height:110px;width:434px;margin-left:70px\"><table id=\"datagrid1\" style=\"height:110px;width:434px;\" class=\"easyui-datagrid\" data-options=\"url:'/Page/P01076/GetTiaoli?JCID=J015',fitColumns:true,fit:true,rownumbers:true,nowrap: true,singleSelect:true,striped: true,collapsible: false,resizable:true, onSelect: onClickRow,onUnselect: cancleClickRow\">";
    //var boarddiv = boarddiv + "<thead>";
    //var boarddiv = boarddiv + " <tr>";

    ////var boarddiv = boarddiv + " <th data-options=\"field:'AutoKey',checkbox:true,hidden:true\"></th>";
    ////var boarddiv = boarddiv + " <th data-options=\"field:'JCName',fixed:true,\">奖惩条例</th>";
    ////var boarddiv = boarddiv + " <th data-options=\"field:'Remark',fixed:true,\">备注</th>";
    //var boarddiv = boarddiv + " </tr>";
    //var boarddiv = boarddiv + "</thead>";
    //var boarddiv = boarddiv + "</table></div></tr></td>";
    //$(".datagrid-view2 .datagrid-body tr:eq(0)").after(boarddiv);
    //$('#datagrid1').datagrid('reload');
    alert(1);
    $('.datagrid-body .datagrid-btable').datagrid({
        view: detailview,
        detailFormatter: function (index, row) {
            return '<div class="ddv" style="padding:5px 0"></div>';
        },
        onExpandRow: function (index, row) {
            var ddv = $(this).datagrid('getRowDetail', index).find('div.ddv');
            ddv.panel({
                border: false,
                cache: false,
                href: '/Page/P01076/GetTiaoli?JCID=' + row.itemid,
                onLoad: function () {
                    $('#dg').datagrid('fixDetailRowHeight', index);
                }
            });
            $('#dg').datagrid('fixDetailRowHeight', index);
        }
    });
}

/*按钮操作*/
function submitAct_DGAppend(form, url, datagrid) {
    funcForAjax(url, "post", form, $("#" + form).serialize(), datagrid, funcForAppend);

    //$("#" + datagrid).datagrid('reload'); //fgh 2014-12-10 add
}

function submitAct_DGReplace(form, url, datagrid) {
    if ($("#" + form).form('validate')) {
       //  funcForQueryAjax(url, "post", $("#" + form).serialize(), datagrid);
        funcForQueryAjax(url, "post", form, datagrid);
    }
}

function submitAct_DGUpdate(form, url, datagrid) {
    funcForAjax(url, "post", form, $("#" + form).serialize(), datagrid, funcForUpdate);
    //$("#" + datagrid).datagrid('reload'); //fgh 2014-12-10 add
}
//更新行，不刷新表    李晓彤 2014-12-18
function submitAct_DGRowUpdate(form, url, datagrid,msg){
    checkOnline();
    if ($("#" + form).form('validate')) {
        $.ajax({
            url: url,
            type: "post",
            data: $("#" + form).serialize(),
            dataType: "json",
            success: function (result) {
                if (!result.HasError) {
                    funcForRowUpdate(datagrid, result, form,msg);             
                }
                else {
                    $.messager.alert('提示', result.ErrorMessage); 
                }
            },
            error: function (d, error) {
                showError(error);
            }
        });
    }
}
function funcForRowUpdate(datagrid, result, form, msg) {
    showMessage(msg);
    var index = $("#" + datagrid).datagrid('getRowIndex', $("#" + datagrid).datagrid('getSelected'));
    if (typeof (result.Data) == 'object' && isNaN(result.Data.length)) {
        $("#" + datagrid).datagrid('updateRow', { index: index, row: result.Data });
    }
    else {
        var rows = $("#" + datagrid).datagrid('getSelections');
        for (var i = 0; i < result.Data.length; i++) {
            var indexs = $("#" + datagrid).datagrid('getRowIndex', rows[i]);
            $("#" + datagrid).datagrid('updateRow', { index: indexs, row: result.Data[i] });
        }
        toolHide();//刷新tool按钮状态
        setTimeout("cancel()", 3000);//编辑成功3秒自动关闭对话框 
    }
}
function showActDlg_DGItemsConfirm(datagrid, title, msg, url, width, height) {
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
                        for (var i = 0; i < rows.length; i++) {
                            var index = $("#" + datagrid).datagrid('getRowIndex', rows[i]);
                            $("#" + datagrid).datagrid('deleteRow', index);
                        }
                        $.messager.alert("提示", "已" + title + ids.length + "条记录");//李晓彤 2014光棍节，对数据进行其他操作（操作完不显示）也可以引用


                        $("#" + datagrid).datagrid('reload'); //20141210 add

                        toolHide();
                        if (canAdd == 1) { btnAddIsEnable(datagrid) } //判断申请单据页面新增按钮是否恢复可用
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
function showActDlg_DGItemsConfirmWithApproved(datagrid, title, msg, url, width, height) {
    canAdd = 1;
    showActDlg_DGItemsConfirm(datagrid, title, msg, url, width, height)
}
function btnAddIsEnable(datagrid) {
    var rows = $("#" + datagrid).datagrid("getRows");
    $('#btn-create').linkbutton('enable');
    for (var i = 0; i < rows.length; i++) {
        if (rows[i]['Approved'] != '已批准') {//存在非已批准的申请新增不可用
            $('#btn-create').linkbutton('disable');
        }
    }
}
//工资基本项设定——新增、修改一体 李晓彤2014-0-10-13
var AddUpdIndex = 0;//工资基本项设定——修改行索引
function submitAct_DGAddUpd(form, url, datagrid) {
    //checkOnline()
    //var id = $("#" + form + " input[name = EmployeeID]").val();
    //var flag = 0;
    //var rows = $("#" + datagrid).datagrid('getRows');
    //for (var i = 0; i < rows.length; i++) {
    //    if (rows[i]['EmployeeID'] == id) {
    //        flag = 1;
    //        AddUpdIndex = i;
    //        break;
    //    }
    //}
    //if (flag == 0)
        funcForAjax(url, "post", form, $("#" + form).serialize(), datagrid, funcForAppend);
    //else {
    //    funcForAjax(url, "post", form, $("#" + form).serialize(), datagrid, funcForAddUpd);
    //}
}
function funcForAddUpd(datagrid, result) {
    showMessage("更新成功");
    if (typeof (result.Data) == 'object' && isNaN(result.Data.length)) {
        $("#" + datagrid).datagrid('updateRow', { index: AddUpdIndex, row: result.Data });
    }
    else {
        $("#" + datagrid).datagrid('updateRow', { index: AddUpdIndex, row: result.Data[0] });
    }
    setTimeout("cancel()", 3000);
}
//批量修改 李晓彤2014-10-13
function showActDlg_DGItemActDlgPL(datagrid, title, url, width, height) {
    checkOnline()
    var ids = [];
    var rows = $("#" + datagrid).datagrid('getSelections');
    getDeleteIDs(ids, rows, url, datagrid)
    url = url + "?keys=" + ids,
    showDialog(url, title, width, height)
}
//带有送签功能的表单修改 李晓彤 2014-9-29
function showActDlg_DGUpd(datagrid, title, url, width, height) {
    checkOnline()
    var rows = $("#" + datagrid).datagrid('getSelected');
    if (rows["Approved"] == '申请中' || rows["Approved"] == '已核准') {
        $.messager.alert("提示", "当前表单已送签，无法修改");
    }
    else {

        var selectRow = $("#" + datagrid).datagrid('getSelected');
        var selectCol = $("#" + datagrid).datagrid('options').columns[0][0].field;
        url = url + "?ID=" + selectRow[selectCol];
        showDialog(url, title, width, height)
    }
}
//带有送签功能的表单新增（只能存在一条申请） 李晓彤 2014-9-30
var canAdd=0
function showActDlg_DGAdd(datagrid, title, url, width, height) {
    checkOnline()
    var options = $("#" + datagrid).datagrid('getPager').data("pagination").options;
    var totalRowNum = options.total;
    if (totalRowNum > 0) {
        $.messager.alert("提示", "已存在相关信息，无法新增！");
    }
    else {
        var canAdd = 1
        showDialog(url, title, width, height)
    }
}
function showActDlg_DGAddII(datagrid, title, url, width, height) {
    checkOnline()
    var rows = $("#" + datagrid).datagrid("getRows");
    var flag = 0;
    for (var i = 0; i < rows.length; i++) {
        if (rows[i]['Approved'] != '2') {
            flag = 1; break;
        }
    }
    if (flag == 1) {//存在未批准的申请不可新增
        $.messager.alert("提示", "存在未批准申请信息，无法新增！");
    }
    else {
        var canAdd = 1
        showDialog(url, title, width, height)
    }
}
//送签追踪 李晓彤 2014-9-29
function showActDlg_DGItemActDlgLog(id, title, url, width, height) {
    checkOnline()
    var selectRow = $("#" + id).datagrid('getSelected');
    var selectCol = $("#" + id).datagrid('options').columns[0][0].field;
    if (selectRow["Approved"].trim() == '草稿') {
        $.messager.alert("提示", "当前表单未送签");
    }
    else {
        url = url + "?ID=" + selectRow[selectCol];
        showDialog(url, title, width, height);
    }
}

function downLoad(url, data) {
    if ($("#" + data).form('validate')) {
    $.download(url, $('#' + data).serialize(), 'post');
}
}

function doSearch(value) {
    alert('You input: ' + value);
}

//通用送签 杨磊 2014-9-29
function showActDlg_DGItemsMultiApprove(datagrid, title, msg, table_name, url) {
    checkOnline()
    $.extend($.messager.defaults, { ok: "确定", cancel: "取消" })
    $.messager.confirm(title, msg, function (r) {
        if (r) {
            var grid = $("#" + datagrid);
            var ids = [];
            var rows = grid.datagrid('getSelections');
            getDeleteIDs(ids, rows, url, datagrid)
            $.ajax({
                url: url + "?keys=" + ids + "&table_name=" + table_name,
                type: "post",
                success: function (result) {
                    if (result.HasError == undefined)
                    {
                        window.top.location = result.Url;
                    } else {
                        if (!result.HasError) {
                            $.messager.alert("提示", "成功送签" + result.ExecuteCount + "条记录", "", function () {
                                for (var i = 0; i < result.Data.length; i++) {
                                    var IDCol = grid.datagrid('options').columns[0][0].field;

                                    for (var j = 0; j < rows.length; j++) {
                                        if (rows[j][IDCol] == result.Data[i].Key) {
                                            var status = result.Data[i].ApproveStatus;
                                            var statusName = "";
                                            switch (status) {
                                                case "1": statusName = "申请中"; break;
                                                case "2": statusName = "已核准"; break;
                                            }
                                            var index = grid.datagrid('getRowIndex', rows[j]);
                                            grid.datagrid('updateRow', {
                                                index: index,
                                                row: { Approved: statusName, ApproveStatus: status }
                                            });
                                            toolIsShow();
                                        }
                                    }
                                }
                            });
                        }
                        else { $.messager.alert("提示", result.ErrorMessage); }
                    }
                },
                error: function (xmlhttprequest, text, error) { showError(error); }
            });
        }
        else { cancel(); }
    });
}

/*弹出送签提示*/
function AlertShowMessager(id, table, url) {
    $.messager.confirm('送签', '确认要送签？', function (r) {
        if (r) {
            var selectRow = $("#" + id).datagrid('getSelected');
            var selectCol = $("#" + id).datagrid('options').columns[0][0].field;
            var newUrl = url + "?ID=" + selectRow[selectCol] + "&TableName=" + table;
            var approved = selectRow.Approved;
            if (approved == -1 || approved == 0) {
                $.ajax({
                    url: newUrl,
                    dataType: 'json',
                    cache: false,
                    success: function (result) {
                        if (!result.HasError) {
                            $.messager.alert('提示', '送签成功');
                            funcForUpdate(id, result);//更新当前表单状态
                        } else {
                            $.messager.alert('提示', '送签失败');
                        }
                    }
                });
            } else if (approved == 1) {
                $.messager.alert('提示', '当前表单正在审核中，不允许再次送签');
            } else {
                $.messager.alert('提示', '当前表单已审核，不允许再次送签');
            }
        }
    });
}

/*表单可以多次新增的情况 2014-10-29 ysh*/
function beforeShowActDlg(title, id, url, width, height) {
    var rows = $("#" + id).datagrid('getRows');
    if (rows != null) {
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].Approved == -1 || rows[i].Approved == 1 || rows[i].Approved == 0) {
                $('#btn-create').linkbutton('disable');
                $.messager.alert('提示', '当前存在非草稿表单或被驳回表单，不允许新增');
                return;
            } else {
                $('#btn-create').linkbutton('enable');
            }
        }
    }
    showActDlg(title, url, width, height);
}

/*编辑前，已经结单的单子提示*/
function beforeEdit(id, title, url, width, height) {
    var selectRow = $("#" + id).datagrid('getSelected');
    var val = selectRow.Approved;
    switch (val) {
        case 1:
            $.messager.alert('提示', '当前表单正在申请中，不允许编辑');
            return;
            break;
        case 2:
            $.messager.alert('提示', '当前表单已结单，不允许编辑');
            return;
            break;
        default:
            showActDlg_DGItemActDlg(id, title, url, width, height);
            break;
    }
}

/*查看送签记录前判断*/
function beforeAlertCheckLog(id, title, url, width, height) {
    var selectRow = $("#" + id).datagrid('getSelected');
    if (selectRow.Approved == 1 || selectRow.Approved == 2) {
        showActDlg_DGItemActDlg(id, title, url, width, height);
    } else {
        $.messager.alert('提示', '当前表单为草稿，没有送签记录');
    }
}

/*重置用户密码 王倩雯 2014-10-9*/
function showActDlg_PwdActDlg(datagrid, title, msg, url, width, height) {
    $.extend($.messager.defaults, { ok: "确定", cancel: "取消" })
    $.messager.confirm(title, msg, function (r) {
        if (r) {
            var row = $("#" + datagrid).datagrid('getSelected');
            $.ajax({
                url: url + "?ID=" + row.UserID,
                type: "post",
                success: function (result) {
                    if (!result.HasError) {
                        $.messager.alert("提示", row.UserName + "密码重置成功");
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

var timer;

function setProgress() {
    $.ajax({
        url: "/GenerateRecordResult/GetCurrentProgress",
        type: "get",
        dataType: 'json',
        cache: false,
        success: function (result) {
            if (parseInt((result.Data)[1]) == 100) {
                clearInterval(timer);
            }
            $('#progressbar').progressbar('setValue', parseInt((result.Data)[0]));
            $('#progressbarAttendance').progressbar('setValue', parseInt((result.Data)[1]));
        },
        error: function (xmlhttprequest, text, error) {
            clearInterval(timer);
        }
    });
}

/*文件上传和导入2014-10-10 杨曙华 2014-12李晓彤*/
function ImportFile(url) {
    $('#message').empty();
    $('#file_form').form('submit', {
        url: url,
        onSubmit: function () {
            if ($('#file').filebox('getValue') == "") {
                $.messager.alert("提示", "没有数据！", 'info');
                return false;
            }
            var isValid = $(this).form('validate');
            if (!isValid) {
                return isValid;
            }
            $.messager.progress({
                title: '提示',
                msg: '正在导入...',
                interval: 600
            });
            //$('#progressbar').progressbar('setValue', 0).show();
            //$('#progressbarAttendance').progressbar('setValue', 0).show();
            //timer = setInterval("setProgress()", 3000);
        },
        success: function (data) {
            $.messager.progress('close');
            var result = $.parseJSON(data);
            if (!result.HasError) {
                clearInterval(timer);
                $('#progressbar').hide();
                $('#progressbarAttendance').hide();
                $('#tabs').tabs('select', 2);
                $('#importRecord').html("导入记录：" + "导入成功" + result.Data.InsertCount + "条;导入更新" + result.Data.UpdateCount + "条;导入取消" + result.Data.CancelCount + "条");
            } else {
                $.messager.alert('错误提示', result.ErrorMessage);
            }
        }
    });
}

/*生成考勤月报表 2014-10-17 ysh2014-12李晓彤*/
function attendanceToCheckResult(form, title) {
    if ($("#" + form).form('validate')) {
        $.messager.confirm(title, '确认生成上个月考勤报表？', function (r) {
            if (r) {
                //$.messager.progress({
                //    title: title,
                //    msg: '月报表生成中...',
                //});
                $.ajax({
                    url: '/GenerateRecordResult/MakeMonthResult',
                    type: "post",
                    data: $("#" + form).serialize(),
                    dataType: 'json',
                    success: function (result) {
                        //$.messager.progress('close');
                        $.messager.alert(title, result.ErrorMessage);
                    },
                    error: function (xmlhttprequest, text, error) {
                        //$.messager.progress('close');
                        $.messager.alert(title, result.ErrorMessage);
                    }
                });
            }
                cancel();
            //     });
            // } else {
            //     $.messager.alert(title, result.ErrorMessage);
            //}
            //    },
            //    error: function (xmlhttprequest, text, error) {
            //        $.messager.alert(title, error);
            //    }
        });
    }
}

/*  easyui datebox 相关  */
/* 以下是从jquery.easyui.min里借用来的 */
/* datetimebox */
function _9d9(_9da) {
    var c = $(_9da).datetimebox("calendar");
    var t = $(_9da).datetimebox("spinner");
    var date = c.calendar("options").current;
    return new Date(date.getFullYear(), date.getMonth(), date.getDate(), t.timespinner("getHours"), t.timespinner("getMinutes"), t.timespinner("getSeconds"));
}
function _9de(_9df) {
    var opts = $.data(_9df, "datetimebox").options;
    var date = _9d9(_9df);
    _9dd(_9df, opts.formatter.call(_9df, date));
    $(_9df).combo("hidePanel");
}
function _9dd(_9e0, _9e1, _9e2) {
    var opts = $.data(_9e0, "datetimebox").options;
    $(_9e0).combo("setValue", _9e1);
    if (!_9e2) {
        if (_9e1) {
            var date = opts.parser.call(_9e0, _9e1);
            $(_9e0).combo("setValue", opts.formatter.call(_9e0, date));
            $(_9e0).combo("setText", opts.formatter.call(_9e0, date));
        } else {
            $(_9e0).combo("setText", _9e1);
        }
    }
}
/* datetimebox end */
/* datebox */
function _9bd(_9c2, _9c3, _9c4) {
    var _9c5 = $.data(_9c2, "datebox");
    var opts = _9c5.options;
    var _9c6 = _9c5.calendar;
    $(_9c2).combo("setValue", _9c3);
    _9c6.calendar("moveTo", opts.parser.call(_9c2, _9c3));
    if (!_9c4) {
        if (_9c3) {
            _9c3 = opts.formatter.call(_9c2, _9c6.calendar("options").current);
            $(_9c2).combo("setValue", _9c3).combo("setText", _9c3);
        } else {
            $(_9c2).combo("setText", _9c3);
        }
    }
}
function _9be(_9bf) {
    var _9c0 = $.data(_9bf, "datebox");
    var opts = _9c0.options;
    var _9c1 = _9c0.calendar.calendar("options").current;
    if (_9c1) {
        _9bd(_9bf, opts.formatter.call(_9bf, _9c1));
        $(_9bf).combo("hidePanel");
    }
}
/* datebox end */
/* 借用完毕 */

/* datetimebox */
function SetStartDateTimeBox(id, date_e) {
    var value = new Date($('#' + id).datetimebox("getValue").replace(/-/g, "/"))
    //$("#" + id).datetimebox({ editable: false, showSeconds: false }).datebox('calendar').calendar({
    //    validator: function (date) {
    //        if (date_e != undefined) {
    //            return date <= new Date(date_e.getFullYear(), date_e.getMonth(), date_e.getDate());
    //        }
    //        else {
    //            return date;
    //        }
    //    }
    //});
    //if (date_e == undefined || value <= date_e) {
    if (value > date_e) {
        $("#" + id).datetimebox("setValue", "");
            }
            }

function SetEndDateTimeBox(id, date_s) {
    var value = new Date($('#' + id).datetimebox("getValue").replace(/-/g, "/"))
    $("#" + id).datetimebox({ editable: false, showSeconds: false }).datebox('calendar').calendar({
        validator: function (date) {
            if (date_s != undefined) {
                return date >= new Date(date_s.getFullYear(), date_s.getMonth(), date_s.getDate());
            }
            else {
                return date;
            }
        }
    });
    if (date_s == undefined || value >= date_s) {
        $("#" + id).datetimebox("setValue", value.getFullYear() + "-" + (value.getMonth() + 1) + "-" + value.getDate() + " " + value.getHours() + ":" + value.getMinutes());
    }
}

function SetStartDateTimeBoxButtons(startDateID, endDateID) {
    var buttons_s = $.extend([], $.fn.datetimebox.defaults.buttons);
    buttons_s.splice(0, 1, {
        text: '现在',
        handler: function (target) {
            var date = new Date();
            date.setSeconds(0, 0);
            var opts = $(target).datetimebox("options");
            _9dd(target, opts.formatter.call(target, date));
            $(target).datetimebox("hidePanel");

            SetEndDateTimeBox(endDateID, date);
        }
    });
    buttons_s.splice(1, 1, {
        text: '确定',
        handler: function (target) {
            _9de(target);
            var date_s = _9d9(target);
            SetEndDateTimeBox(endDateID, date_s);
        }
    });
    buttons_s.splice(2, 1, {
        text: '清空',
        handler: function (target) {
            $(target).datetimebox("setValue", "");
            $(target).datetimebox("hidePanel");
            SetEndDateTimeBox(endDateID);
        }
    });
    $('#' + startDateID).datetimebox({
        buttons: buttons_s
    });
}

function SetEndDateTimeBoxButtons(startDateID, endDateID) {
    var buttons_e = $.extend([], $.fn.datetimebox.defaults.buttons);
    buttons_e.splice(0, 1, {
        text: '现在',
        handler: function (target) {
            var date = new Date();
            date.setSeconds(0, 0);
            var opts = $(target).datetimebox("options");
            _9dd(target, opts.formatter.call(target, date));
            $(target).datetimebox("hidePanel");
            SetStartDateTimeBox(startDateID, date);
        }
    });
    buttons_e.splice(1, 1, {
        text: '确定',
        handler: function (target) {
            _9de(target);
            var date_e = _9d9(target);
            SetStartDateTimeBox(startDateID, date_e);
        }
    });
    buttons_e.splice(2, 1, {
        text: '清空',
        handler: function (target) {
            $(target).datetimebox("setValue", "");
            $(target).datetimebox("hidePanel");
            SetStartDateTimeBox(startDateID);
        }
    });
    $('#' + endDateID).datetimebox({
        buttons: buttons_e
    });
}

//主调用方法，对datetimebox的日期选择进行客户端限制
function SetDateTimeBoxValidator(startDateID, endDateID) {
    SetStartDateTimeBoxButtons(startDateID, endDateID);
    SetEndDateTimeBoxButtons(startDateID, endDateID);

    if ($('#' + startDateID).datetimebox("getValue") != "") {
        SetEndDateTimeBox(endDateID, new Date($('#' + startDateID).datetimebox("getValue").replace(/-/g, "/")));
    }
    if ($('#' + endDateID).datetimebox("getValue") != "") {
        SetStartDateTimeBox(startDateID, new Date($('#' + endDateID).datetimebox("getValue").replace(/-/g, "/")));
    }
}
function ResetDateTimeBoxValidator(startDateID, endDateID) {
    SetStartDateTimeBox(startDateID);
    SetEndDateTimeBox(endDateID);
}
/* datetimebox end */
/* datebox */
function SetStartDateBox(id, date_e) {
    var current = $("#" + id).datebox("getValue").split("-");
    var value = new Date(current[0], parseInt(current[1]) - 1, current[2]);
    //$("#" + id).datebox({ editable: false }).datebox('calendar').calendar({
    //    validator: function (date) {
    //        if (date_e != undefined) {
    //            return date <= new Date(date_e.getFullYear(), date_e.getMonth(), date_e.getDate());
    //        }
    //        else {
    //            return date;
    //        }
    //    }
    //});
    //if (date_e == undefined || value <= date_e) {
    if (value > date_e) {
        $("#" + id).datebox("setValue", "");
            }
        }

function SetEndDateBox(id, date_s) {
    var current = $("#" + id).datebox("getValue").split("-");
    var value = new Date(current[0], parseInt(current[1]) - 1, current[2]);
    $("#" + id).datebox({ editable: false }).datebox('calendar').calendar({
        validator: function (date) {
            if (date_s != undefined) {
                return date >= new Date(date_s.getFullYear(), date_s.getMonth(), date_s.getDate());
            }
            else {
                return date;
            }
        }
    });
    if (date_s == undefined || value >= date_s) {
        $("#" + id).datebox("setValue", value.getFullYear() + "-" + (value.getMonth() + 1) + "-" + value.getDate());
    }
}
function SetStartDateBoxButtons(startDateID, endDateID) {
    $("#" + startDateID).datebox({
        onSelect: function (date) {
            SetEndDateBox(endDateID, date);
        }
    });

    var buttons_s = $.extend([], $.fn.datebox.defaults.buttons);
    buttons_s.splice(0, 1, {
        text: '今天',
        handler: function (target) {
            var now = new Date();
            var date = new Date(now.getFullYear(), now.getMonth(), now.getDate());
            $(target).datebox("calendar").calendar({ year: now.getFullYear(), month: now.getMonth() + 1, current: now });
            _9be(target);

            SetEndDateBox(endDateID, date);
        }
    });
    buttons_s.splice(1, 1, {
        text: '清空',
        handler: function (target) {
            $(target).datebox("setValue", "");
            $(target).datebox("hidePanel");
            SetEndDateBox(endDateID);
        }
    });
    $('#' + startDateID).datebox({
        buttons: buttons_s
    });
}
function SetEndDateBoxButtons(startDateID, endDateID) {
    $("#" + endDateID).datebox({
        onSelect: function (date) {
            SetStartDateBox(startDateID, date);
        }
    });

    var buttons_e = $.extend([], $.fn.datebox.defaults.buttons);
    buttons_e.splice(0, 1, {
        text: '今天',
        handler: function (target) {
            var now = new Date();
            var date = new Date(now.getFullYear(), now.getMonth(), now.getDate());
            $(target).datebox("calendar").calendar({ year: now.getFullYear(), month: now.getMonth() + 1, current: now });
            _9be(target);

            SetStartDateBox(startDateID, date);
        }
    });
    buttons_e.splice(1, 1, {
        text: '清空',
        handler: function (target) {
            $(target).datebox("setValue", "");
            $(target).datebox("hidePanel");
            SetStartDateBox(startDateID);
        }
    });
    $('#' + endDateID).datebox({
        buttons: buttons_e
    });
}
//主调用方法，对datebox的日期选择进行客户端限制
function SetDateBoxValidator(startDateID, endDateID) {
    SetStartDateBoxButtons(startDateID, endDateID);
    SetEndDateBoxButtons(startDateID, endDateID);

    if ($('#' + startDateID).datetimebox("getValue") != "") {
        SetEndDateBox(endDateID, new Date($('#' + startDateID).datebox("getValue").replace(/-/g, "/")));
    }
    if ($('#' + endDateID).datetimebox("getValue") != "") {
        SetStartDateBox(startDateID, new Date($('#' + endDateID).datebox("getValue").replace(/-/g, "/")));
    }
}
function ResetDateBoxValidator(startDateID, endDateID) {
    SetStartDateBox(startDateID);
    SetEndDateBox(endDateID);
}
/* datebox end */
/*  easyui datebox 相关结束  */

//签核历程的iframe相关
function InitIFrame(iframeID, contentClass, datagridID) {
    if (iframeID == undefined) {
        iframeID = "af";
    }
    if (contentClass == undefined) {
        contentClass = "edit-panel";
    }
    if (datagridID == undefined) {
        datagridID = "datagrid";
    }
    //移动签核历程的iframe
    $(".showmsg").parent().before($("#" + iframeID));
    //$(".showmsg").parent().before('<div id="frameContent" class="edit-field-w2"></div>')
    //$("#" + iframeID).appendTo($("#frameContent"));
    //$("#" + iframeID).prependTo($(".showmsg").parent());
    var selectRow = $("#" + datagridID).datagrid('getSelected');
    var selectCol = $("#" + datagridID).datagrid('options').columns[0][0].field;
    $("#" + iframeID).attr("src", "/Approve/ApproveFlow?ID=" + selectRow[selectCol] + "&TableName=" + $("#" + iframeID).attr("tb"));
    $("#" + iframeID).attr("frameborder", 0).attr("scrolling", "no").attr("width", "100%");
}

//自动调整iframe高度
function DynIframeSize(iframe_id) {
    var pTar = null;
    if (document.getElementById) {
        pTar = document.getElementById(iframe_id);
    }
    else {
        eval('pTar = ' + iframe_id + ';');
    }
    if (pTar && !window.opera) {
        //begin resizing iframe 
        pTar.style.display = "block"
        if (pTar.contentDocument && pTar.contentDocument.body.offsetHeight) {
            //ns6 syntax 
            pTar.height = pTar.contentDocument.body.offsetHeight + 10;
            //pTar.width = pTar.contentDocument.body.scrollWidth + 20;
        }
        else if (pTar.Document && pTar.Document.body.scrollHeight) {
            //ie5+ syntax 
            pTar.height = pTar.Document.body.scrollHeight;
            //pTar.width = pTar.Document.body.scrollWidth;
        }
            }
}

function funcForAjaxNoValid(url, method, data, datagrid, func) {
    checkOnline()
    $.ajax({
        url: url,
        type: method,
        data: data,
        dataType: "json",
        success: function (result) {
            if (!result.HasError) {
                func(datagrid, result);
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

//检查在线状态：如果不在线，则转到登录页面 项谢进（20141121）
function checkOnline() {
   // console.log('logo')
    $.ajax({
        url: "/Home/IsOnline",
        type: "POST",
        success: function (result) {
            if (!result) {
                showLogin();
            }
        },
        error: function () {
            showLogin();
        }
    });
}
function showLogin() {
    if (this.window.parent != null) {
        this.window.parent.window.navigate("/Account/Login");
    }
    else {
        this.window.navigate("/Account/Login");
    }
}
//检查在线状态结束

function UnitWithText(text, unit) {
    $("#" + unit).combobox({
        required: false,
        disabled: true
    });
    $("#" + text).numberbox({
        onChange: function (newValue, oldValue) {
            if ($("#" + text).numberbox('getValue') != "") {
                $("#" + unit).combobox({
                    required: true,
                    disabled: false
                });
            }
            else {
                $("#" + unit).combobox({
                    required: false,
                    disabled: true
                });
                $("#" + unit).combobox('clear')
            }
        }
    });
}



/* fgh 20141209 add **/
function submitActToList_DGAppend(form, url, datagrid) {
    funcForAjax(url, "post", form, $("#" + form).serialize(), datagrid, funcForAppend);
    $("#" + datagrid).datagrid('reload');
    //submitAct_DGReplace(form, urlList, datagrid);
}


//fgh add 更新后显示第一笔
function submitActToList_DGUpdate(form, url, datagrid) {
    funcForAjax(url, "post", form, $("#" + form).serialize(), datagrid, funcForUpdate)
    $("#" + datagrid).datagrid('reload');
    //submitAct_DGReplace(form, urlList, datagrid)
}

function showActDlgToList_DGItemsConfirm(datagrid, title, msg, url, width, height) {
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
                        for (var i = 0; i < rows.length; i++) {
                            var index = $("#" + datagrid).datagrid('getRowIndex', rows[i]);
                            $("#" + datagrid).datagrid('deleteRow', index);
                        }
                        $.messager.alert("提示", "已" + title + ids.length + "条记录");//李晓彤 2014光棍节，对数据进行其他操作（操作完不显示）也可以引用

                        $("#" + datagrid).datagrid('reload');
                        //submitAct_DGReplace('dataform', urlList, datagrid); //重新刷新

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

//iframe显示报表页面 gj
function showReport(url,iframeid,formid)
{
    if ($("#" + formid).form('validate')) {
        $("#" + iframeid).attr("src", url + "?" + $("#" + formid).serialize());
        cancel();
    }
}