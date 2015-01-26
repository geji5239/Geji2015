//对预先设定好的各种单据在提交时进行检查
function DocumentsValidate(iframe_id) {
    var iframe_document = $("#" + iframe_id).contents();
    $("#validateSummary").empty();
    //必填项检查
    /* error elements */
    var empty_element = new Array();
    var integer_element = new Array();
    var decimal_element = new Array();
    var ranger_element = new Array();
    /* ************** */
    //空元素判断
    iframe_document.find("[required='required']").each(function () {
        if ($.trim($(this).val()) == "") {
            var t = $.trim($(this).parent().prev("td").text());
            if (empty_element.indexOf(t) == -1) {
                empty_element.push(t);
                $("#validateSummary").append("<li class=\"error\" style=\"color:red;\">" + t + " 必填</span>");
            }
        }
    });
    //整数判断
    iframe_document.find("[datatype='integer']").each(function () {
        var val = $.trim($(this).val());
        if (val != "") {
            //不是整数
            if (Math.floor(val) != val) {
                var t = $.trim($(this).parent().prev("td").text());
                if (empty_element.indexOf(t) == -1 &&
                    integer_element.indexOf(t) == -1) {
                    integer_element.push(t);
                    $("#validateSummary").append("<li class=\"error\" style=\"color:red;\">" + t + " 必须是整数</span>");
                }
            }
        }
    });
    //实数判断
    iframe_document.find("[datatype='decimal']").each(function () {
        var val = $.trim($(this).val());
        if (val != "") {
            //不是数字
            if (isNaN(val)) {
                var t = $.trim($(this).parent().prev("td").text());
                if (empty_element.indexOf(t) == -1 &&
                    decimal_element.indexOf(t) == -1) {
                    decimal_element.push(t);
                    $("#validateSummary").append("<li class=\"error\" style=\"color:red;\">" + t + " 必须是数字</span>");
                }
            }
        }
    });
    //数字范围判断
    iframe_document.find("[max]").each(function () {
        var val = $.trim($(this).val());
        if (val != "") {
            if (!isNaN(val)) {
                var max = $(this).attr("max");
                if (eval(val + ">" + max)) {
                    var t = $.trim($(this).parent().prev("td").text());
                    if (empty_element.indexOf(t) == -1 &&
                        integer_element.indexOf(t) == -1 &&
                        decimal_element.indexOf(t) == -1 &&
                        ranger_element.indexOf(t) == -1) {
                        ranger_element.push(t);
                        $("#validateSummary").append("<li class=\"error\" style=\"color:red;\">" + t + " 只能小于等于 " + max + "</span>");
                    }
                }
            }
        }
    });
    iframe_document.find("[min]").each(function () {
        var val = $.trim($(this).val());
        if (val != "") {
            if (!isNaN(val)) {
                var min = $(this).attr("min");
                if (eval(val + "<" + min)) {
                    var t = $.trim($(this).parent().prev("td").text());
                    if (empty_element.indexOf(t) == -1 &&
                        integer_element.indexOf(t) == -1 &&
                        decimal_element.indexOf(t) == -1 &&
                        ranger_element.indexOf(t) == -1) {
                        ranger_element.push(t);
                        $("#validateSummary").append("<li class=\"error\" style=\"color:red;\">" + t + " 只能大于等于 " + min + "</span>");
                    }
                }
            }
        }
    });
    if (empty_element.length == 0 &&
        integer_element.length == 0 &&
        decimal_element.length == 0 &&
        ranger_element.length == 0) {
        return true;
    }
    else {
        return false;
    }
}