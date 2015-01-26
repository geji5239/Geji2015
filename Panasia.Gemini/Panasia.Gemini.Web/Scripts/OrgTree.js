//参数说明：
//前提，因为此树一共会返回4种数据即 公司，部门，职位和员工，所以在以下说明中会多次出现各数据之数据表首字母，公司-C、部门-D、职位-J、员工-E
//nameContainer ： 存放返回名称的容器的name
//valueContainers ： 存放返回值的容器的name。鉴于值最多可能需要返回4个，所以这里应当传入一个数组，例如['C:CompanyID','D:DeptID','J:JobID','E:EmployeeID']，根据自己需要调整
//configType ： 树的数据组成【1、全部呈现，包括公司，部门，职位和员工 2、公司、部门、职位 3、公司、部门 4、公司】
//dataTextFormat : 最后输出到页面上显示文本的格式化数据，例如：XX公司-XX部门，那么格式化方法就是C-D。如果这个参数为空，则默认根据configType显示最底层数据。
//defaultDataContainer : 按需加载的数据容器的name，比如在联动时，需要根据所选公司来选择部门，这里就可以填上公司ID所在的隐藏域的name。还有一种情况就是ID取自登录者信息，则这里填[User]。可以为空
//defaultDataType : 按需加载的数据类型: C 表示 CompanyID , D 表示 DeptID。可以为空
//以上2个参数任意一个为空则2个参数均无效
//defaultState ： 节点的默认开闭状态 1为开，其他值为闭合，调用时可以省略
function ShowOrgTree(nameContainer, valueContainers, configType, dataTextFormat, defaultDataContainer, defaultDataType, defaultState) {
    $("#dvMain_org").remove();
    $("body").append('<div id="dvMain_org" style="padding:5px 5px 5px 5px;overflow-x:hidden;">' +
        '<div class="easyui-panel" style="height:26px;">' +
        '<a href="javascript:void(0)" class="easyui-linkbutton l-btn l-btn-small" data-options="iconCls:\'icon-ok\'" id="btnSel_org"><span class="l-btn-left l-btn-icon-left"><span class="l-btn-text">选择</span><span class="l-btn-icon icon-ok">&nbsp;</span></span></a>' +
    '</div>' +
    '<div style="overflow:scroll">'+
    '<img id="loadingImage" class="progress" src="/Content/Images/loading.gif" />' +
    '<ul id="tt_org" style="width:300px; height:300px;display:none;"></ul>' +
    '</div>'+
    '</div>');

    $("#dvMain_org").css("display", "none");

    var validType = "";
    var validName = "";

    //不知为何这里不能用switch case
    if (configType == "1") { validType = "e"; validName = "员工"; }
    else if (configType == "2") { validType = "j"; validName = "职位"; }
    else if (configType == "3") { validType = "d"; validName = "部门"; }
    else if (configType == "4") { validType = "c"; validName = "公司"; }

    $("#btnSel_org").click(function () {
        var selectedNode = $('#tt_org').tree("getSelected");
        if (selectedNode != null && selectedNode.type == validType) {
            var nodeFamily = {};
            var node = selectedNode;
            var datatype = "";

            for (node = selectedNode; node != null; node = $('#tt_org').tree('getParent', node.target)) {
                if (datatype != node.type) {
                    nodeFamily[node.type] = node;
                }
                else {
                    continue;
                }
                datatype = node.type;
            }

            //格式化字符串
            var text = "";
            if (dataTextFormat != undefined &&
                dataTextFormat != "" &&
                dataTextFormat != null) {
                text = dataTextFormat.toLowerCase()
                    .replace("c", nodeFamily["c"] == undefined ? "" : nodeFamily["c"].text)
                    .replace("d", nodeFamily["d"] == undefined ? "" : nodeFamily["d"].text)
                    .replace("j", nodeFamily["j"] == undefined ? "" : nodeFamily["j"].text)
                    .replace("e", nodeFamily["e"] == undefined ? "" : nodeFamily["e"].text);
            }
            else {
                text = selectedNode.text;
            }

            //鉴于配置中name的使用率远远大于id，因此这里用name进行筛选
            //配合easyui
            if ($("input[name='" + nameContainer + "']").hasClass("textbox-value")) {
                $("input[textboxname='" + nameContainer + "']").textbox("setValue", text);
            }
            else {
                $("input[name='" + nameContainer + "']").val(text);
            }

            //给值容器集合分别赋值
            var valueChanged = false;
            for (var i = 0; i < valueContainers.length; i++) {
                var item = valueContainers[i].split(':');
                if (item.length == 2) {
                    var type = item[0].toLowerCase();
                    var name = item[1];

                    var oldValue = $("input[name='" + name + "']").val();

                    $("input[name='" + name + "']").val(nodeFamily[type].id);

                    if (oldValue != nodeFamily[type].id) {
                        valueChanged = true;
                    }
                }
            }

            if (valueChanged)//新旧值不同，需要清空关联项
            {
                //查找是否有其他按钮把valueContainers其中一项作为defaultDataContainer进行了操作，如果有的话需要把那个按钮方法里的nameContainer和valueContainer清空
                $("a[onclick^='ShowOrgTree']").each(function () {
                    var parameters = $(this).attr("onclick").replace(/ShowOrgTree|\(|\)|\'|\"/g, "").split(',');

                    for (var i = 0; i < valueContainers.length; i++) {
                        var item = valueContainers[i].split(':');
                        if (item.length == 2) {
                            var name = item[1];

                            if (parameters.length >= 6 && parameters[4] == name)//只有参数数量在6个以上并且defaultDataContainer=本次操作的valueContainers其中一项的才需要进行清空操作
                            {
                                $("input[name='" + parameters[0] + "']").val("");
                                $("input[name='" + parameters[1] + "']").val("");
                                break;
                            }
                        }
                    }
                });
            }

            $("#dvMain_org").dialog("close");
        }
        else {
            $.messager.alert("提示", "请选择一个" + validName);
        }
    });

    var defaultData = defaultDataContainer;
    if (defaultDataType != undefined &&
        defaultDataContainer != undefined &&
        defaultDataContainer != "" &&
        defaultDataContainer != null &&
        defaultDataContainer.toLowerCase() != "[user]") {
        if ($("input[name='" + defaultDataContainer + "']").length > 0) {
            defaultData = $("input[name='" + defaultDataContainer + "']").val();
            //如果有联动的，要求先选择联动的值
            if (defaultData == "") {
                var tips = "";
                if (defaultDataType.toLowerCase() == "c") {
                    tips = "公司";
                }
                else if (defaultDataType.toLowerCase() == "d") {
                    tips = "部门";
                }
                $.messager.alert("提示", "请先选择" + tips);
                return;
            }
        }
        else {
            defaultData = "";
        }
    }

    $("#dvMain_org").css("display", "block");
    $('#dvMain_org').dialog({
        shadow: false,
        closed: false,
        cache: false,
        width: 200,
        height: 400,
        modal: true,
        title: "选择" + validName
    });
    //初始化树
    $('#tt_org').tree({
        url: '/Common/InitOrgTree?configType=' + configType + '&defaultState=' + defaultState + '&defaultData=' + defaultData + '&defaultDataType=' + defaultDataType,
        onClick: function (node) {//单击事件  
            $(this).tree('toggle', node.target);
        },
        onLoadSuccess: function () {
            $("#loadingImage").css("display", "none");
            $(this).css("display", "");

            for (var i = 0; i < valueContainers.length; i++) {
                var item = valueContainers[i].split(':');
                if (item.length == 2) {
                    var type = item[0].toLowerCase();
                    var name = item[1];
                    if (type == validType) {
                        if ($("input[name='" + name + "']").val() != "") {//检测到有值，则初始化选中该值
                            var leafnode = $('#tt_org').tree('find', $("input[name='" + name + "']").val());
                            var node = leafnode;
                            while ($('#tt_org').tree('getParent', node.target) != null) {
                                node = $('#tt_org').tree('getParent', node.target);
                                $('#tt_org').tree('expand', node.target);
                            }
                            $('#tt_org').tree('select', leafnode.target);
                        }
                        break;
                    }
                }
            }

            $('#dvMain_org').dialog("center")
        }
    });
}

//$(document).ready(function () {
//    $.extend($.fn.validatebox.defaults.rules, {
//        treeValid: {
//            validator: function (value) {
//                return $.trim(value) != "";
//            },
//            message: '请选择'
//        }
//    });
//});