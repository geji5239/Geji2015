//参数说明：
//nameContainer ： 存放所选名称的容器的name
//valueContainer ： 存放所选值的容器的name
//defaultState ： 节点的默认开闭状态 1为开，其他值为闭合

function ShowPaymentTree(nameContainer, valueContainer, defaultState) {
    if ($("#dvMain_org").length == 0) {
        $("body").append('<div id="dvMain_org" style="padding:5px 5px 5px 5px;overflow-x:hidden;">' +
            '<div class="easyui-panel" style="height:26px;">' +
            '<a href="javascript:void(0)" class="easyui-linkbutton l-btn l-btn-small" data-options="iconCls:\'icon-ok\'" id="btnSel_org"><span class="l-btn-left l-btn-icon-left"><span class="l-btn-text">选择</span><span class="l-btn-icon icon-ok">&nbsp;</span></span></a>' +
        '</div>' +
        '<img id="loadingImage" class="progress" src="/Content/Images/loading.gif" />' +
        '<ul id="tt_org" style="width:300px; height:300px;display:none;"></ul>' +
        '</div>');

        $("#dvMain_org").css("display", "none");

        $("#btnSel_org").click(function () {
            var selectedNode = $('#tt_org').tree("getSelected");
            if (selectedNode != null) {
                //鉴于配置中name的使用率远远大于id，因此这里用name进行筛选
                $("input[name='" + nameContainer + "']").val(selectedNode.text);
                $("input[name='" + valueContainer + "']").val(selectedNode.id);
                $("#dvMain_org").dialog("close");
            }
            else {
                $.messager.alert("提示", "请选择一个类型");
            }
        });

        $('#tt_org').tree({
            url: '/bi_Payment/GetTree',
            onClick: function (node) {//单击事件  
                $(this).tree('toggle', node.target);
            },
            onLoadSuccess: function () {
                $("#loadingImage").css("display", "none");
                $(this).css("display", "");
                if ($("input[name='" + valueContainer + "']").val() != "") {
                    var leafnode = $('#tt_org').tree('find', $("input[name='" + valueContainer + "']").val());
                    var node = leafnode;
                    while ($('#tt_org').tree('getParent', node.target) != null) {
                        node = $('#tt_org').tree('getParent', node.target);
                        $('#tt_org').tree('expand', node.target);
                    }
                    $('#tt_org').tree('select', leafnode.target);
                }
            }
        });
    }
    else
    {
        $('#tt_org').tree('reload');
    }
    
    $("#dvMain_org").css("display", "block");
    $('#dvMain_org').dialog({
        shadow: false,
        width: 400,
        height: 400,
        modal: true,
        title: "选择所属类别"
    });
}

//function PaymentDelete(datagrid, title, msg, url, width, height) {

//    $.extend($.messager.defaults, { ok: "确定", cancel: "取消" })
//    $.messager.confirm(title, msg, function (r) {
//        if (r) {
//            var ids = [];
//            var rows = $("#" + datagrid).datagrid('getSelections');
//            getDeleteIDssss(ids, rows, url, datagrid)
//            $.ajax({
//                url: url + "?keys=" + ids,
//                type: "post",
//                success: function (result) {
//                    if (!result.HasError) {
//                        for (var i = 0; i < rows.length; i++) {
//                            var index = $("#" + datagrid).datagrid('getRowIndex', rows[i]);
//                            $("#" + datagrid).datagrid('deleteRow', index);
//                        }
//                        $.messager.alert("提示", "已删除" + ids.length + "条记录");

//                    }
//                    else {
//                        $.messager.alert("提示", result.Error);
//                    }
//                },
//                error: function (xmlhttprequest, text, error) {
//                    showError(error);
//                }
//            });
//        }
//        else {
//            cancel();
//        }
//    });
//}
//function getDeleteIDssss(ids, rows, url, id) {
//    var selectCol = $("#" + id).datagrid('options').columns[0][0].field;
//    for (var i = 0; i < rows.length; i++)
//    { ids.push(rows[i][selectCol]) }

//    return ids;
//}