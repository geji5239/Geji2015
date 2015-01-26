//参数说明：
//nameContainer ： 存放返回名称的容器的name
//valueContainer ： 存放返回值的容器的name
//configType ： 树的数据组成【1、全部呈现，包括公司，部门，职位和员工 2、公司、部门、职位 3、公司、部门 4、公司】
//defaultDataContainer : 按需加载的数据容器的name，比如在联动时，需要根据所选公司来选择部门，这里就可以填上公司ID所在的隐藏域的name。还有一种情况就是ID取自登录者信息，则这里填[User]。可以为空
//defaultDataType : 按需加载的数据类型: C 表示 CompanyID , D 表示 DeptID。可以为空
//以上2个参数任意一个为空则2个参数均无效
//defaultState ： 节点的默认开闭状态 1为开，其他值为闭合，调用时可以省略
var validName = "";
function ShowOrgTree(nameContainer, valueContainer, configType, defaultDataContainer, defaultDataType, defaultState) {
    $("#dvMain_org").remove();
    $("body").append('<div id="dvMain_org" style="padding:5px 5px 5px 5px;overflow-x:hidden;">' +
        '<div class="easyui-panel" style="height:26px;">' +
        '<a href="javascript:void(0)" class="easyui-linkbutton l-btn l-btn-small" data-options="iconCls:\'icon-ok\'" id="btnSel_org"><span class="l-btn-left l-btn-icon-left"><span class="l-btn-text">选择</span><span class="l-btn-icon icon-ok">&nbsp;</span></span></a>' +
    '</div>' +
    '<img id="loadingImage" class="progress" src="/Content/Images/loading.gif" />' +
    '<ul id="tt_org" style="width:300px; height:300px;display:none;"></ul>' +
    '</div>');

    $("#dvMain_org").css("display", "none");

    var validType = "";

    //不知为何这里不能用switch case
    if (configType == "1") { validType = "employee"; validName = "员工"; }
    else if (configType == "2") { validType = "job"; validName = "职位"; }
    else if (configType == "3") { validType = "department"; validName = "部门"; }
    else if (configType == "4") { validType = "company"; validName = "公司"; }
    
    $("#btnSel_org").click(function () {
        var selectedNode = $('#tt_org').tree("getSelected");
        if (selectedNode != null && selectedNode.type == validType) {
            //鉴于配置中name的使用率远远大于id，因此这里用name进行筛选
            var oldValue = $("input[name='" + valueContainer + "']").val();//留个底，以便下面清空操作可以进行判断
            $("input[name='" + nameContainer + "']").val(selectedNode.text);
            $("input[name='" + valueContainer + "']").val(selectedNode.id);

            if (oldValue != selectedNode.id)//新旧值不同，需要清空
            {
                //查找是否有其他按钮把valueContainer作为defaultDataContainer进行了操作，如果有的话需要把那个按钮方法里的nameContainer和valueContainer清空
                $("a[onclick^='ShowOrgTree']").each(function () {
                    var parameters = $(this).attr("onclick").replace(/ShowOrgTree|\(|\)|\'|\"/g, "").split(',');
                    if (parameters.length >= 5 && parameters[3] == valueContainer)//只有参数数量在5个以上并且defaultDataContainer=本次操作的valueContainer的才需要进行清空操作
                    {
                        $("input[name='" + parameters[0] + "']").val("");
                        $("input[name='" + parameters[1] + "']").val("");
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
    if (defaultDataType!=undefined && 
        defaultDataContainer != undefined &&
        defaultDataContainer != "" &&
        defaultDataContainer != null &&
        defaultDataContainer.toLowerCase() != "[user]") {
        if ($("input[name='" + defaultDataContainer + "']").length > 0) {
            defaultData = $("input[name='" + defaultDataContainer + "']").val();
            if (defaultData == "")
            {
                var tips = "";
                if (defaultDataType.toLowerCase() == "c")
                {
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

    $('#tt_org').tree({
        url: '/Common/InitOrgTree?configType=' + configType + '&defaultState=' + defaultState + '&defaultData=' + defaultData + '&defaultDataType=' + defaultDataType,
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

    $("#dvMain_org").css("display", "block");
    $('#dvMain_org').dialog({
        shadow: false,
        width: 200,
        height: 400,
        modal: true,
        title: "选择" + validName
    });
}