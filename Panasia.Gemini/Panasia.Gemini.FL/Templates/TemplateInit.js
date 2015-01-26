$(document).ready(function () {
    LoadData();
    $("[datatype='date']").each(function () {
        $(this).html(new Date($(this).html().replace(/-/g, "/")).Format("yyyy-MM-dd"))
        $(this).removeAttr("datatype");
    });
    $("[datatype='datetime']").each(function () {
        $(this).html(new Date($(this).html().replace(/-/g, "/")).Format("yyyy-MM-dd hh:mm:ss"))
        $(this).removeAttr("datatype");
    });
    $("[convert]").each(function () {
        var converter = $(this).attr("convert").split("|");
        var value = $(this).html();
        $(this).html("");
        for (var i = 0; i < converter.length; i++) {
            var item = converter[i].split(":");
            if (value == $.trim(item[0])) {
                $(this).html($.trim(item[1]));
                break;
            }
        }
        $(this).removeAttr("convert");
    });

    $("[valueFrom]").each(function () {
        var value = $("#" + $(this).attr("valueFrom")).html();
        $(this).html(value);
    });

    $("[source]").each(function () {
        var htmlTable = this;
        var table = $(htmlTable).attr("source");
        var keys = $(htmlTable).attr("keys").split(",");
        var keySet = new Array;
        for (var i = 0; i < keys.length; i++) {
            if ($("#" + keys[i]).get(0) != undefined) {
                var value = $("#" + keys[i]).html()
                if (isNaN(value))
                {
                    value = "'" + value + "'";
                }
                keySet.push(keys[i] + "=" + value);
            }
        }
        $(htmlTable).removeAttr("keys");
        //keySet.join(",");
        var fieldSet = new Array();

        $(htmlTable).find("tr:first").children("[field]").each(function () {
            fieldSet.push($(this).attr("field"));
        });

        //暂时只能用于绑定单表数据，多表数据要联合查询，最好使用视图，但是现在不能随便建立视图
        $.ajax({
            url: "/Approve/GetListByKey",
            async: false,
            data: {
                table: table,
                keys: keySet.join(","),
                fields: fieldSet.join(",")
            },
            success: function (result) {
                if (!result.HasError) {
                    var data = result.Data;
                    for (var i = 0; i < data.length; i++) {
                        var row = "<tr>";
                        for (var j = 0; j < fieldSet.length; j++) {
                            var header = $(htmlTable).find("[field='" + fieldSet[j] + "']");
                            var converter = header.attr("valueConvert");
                            var db = "";
                            if (converter != undefined && converter != "") {
                                db = " db=" + converter;
                            }
                            row += "<td" + db + ">" + data[i][fieldSet[j]] + "</td>";
                        }
                        $(htmlTable).append(row);
                    }
                    $(htmlTable).find("[field]").removeAttr("field");
                    $(htmlTable).find("[valueConvert]").removeAttr("valueConvert");
                }
            }
        });
        $(this).removeAttr("source");
    });

    $("[db]").each(function () {
        var obj = $(this);
        var db = obj.attr("db").split(",");
        var value = obj.html();
        $.ajax({
            url: "/Approve/GetValueByID",
            async:false,
            data: {
                table: db[0],
                nameField: db[1],
                valueField: db[2],
                value: value
            },
            success: function (data) {
                obj.html(data);
                obj.removeAttr("db");
            }
        })
    });

    DynIframeSize($("#template", window.parent.document).get(0));
});

//function timespan(container, startDate, endDate, timeFormat) {
//    var start = $("#" + startDate).val();
//    var end = $("#" + endDate).val();
//    if (start != "" && end != "") {
//        $("#" + container).html(getOffDateTime(Date.parse(start), Date.parse(end), timeFormat));
//    }
//}

//function getOffDateTime(startDate, endDate, timeFormat) {
//    var mmSec = (endDate - startDate); //得到时间戳相减 得到以毫秒为单位的差
//    var offset = 0;
//    switch (timeFormat) {
//        case "d": offset = 86400000; break;//3600000 * 24
//        case "h": offset = 3600000; break;
//    }
//    return (mmSec / offset).toFixed(1);
//}

function LoadData() {
    var json = eval("(" + $("#values", window.parent.document).val() + ")");
    $("input").each(function () {
        var key = $(this).attr("name");
        if (json.hasOwnProperty(key)) {
            $(this).val(json[key]);
        }
    });
    $("span").each(function () {
        var key = $(this).attr("id");
        if (json.hasOwnProperty(key)) {
            $(this).html(json[key]);
        }
    });
}

function DynIframeSize(pTar) {
    //var pTar = null;
    //if (document.getElementById) {
    //    pTar = document.getElementById(iframe_id);
    //}
    //else {
    //    eval('pTar = ' + iframe_id + ';');
    //}
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