/**
 * add by cgh
 * 针对panel window dialog三个组件拖动时会超出父级元素的修正
 * 如果父级元素的overflow属性为hidden，则修复上下左右个方向
 * 如果父级元素的overflow属性为非hidden，则只修复上左两个方向
 * @param left
 * @param top
 * @returns
 */
var easyuiPanelOnMove = function (left, top) {
    var parentObj = $(this).panel('panel').parent();
    if (left < 0) {
        $(this).window('move', {
            left: 1
        });
    }
    if (top < 0) {
        $(this).window('move', {
            top: 1
        });
    }
    var width = $(this).panel('options').width;
    var height = $(this).panel('options').height;
    var right = left + width;
    var buttom = top + height;
    var parentWidth = parentObj.width();
    var parentHeight = parentObj.height();
    if (parentObj.css("overflow") == "hidden") {
        if (left > parentWidth - width) {
            $(this).window('move', {
                "left": parentWidth - width
            });
        }
        if (top > parentHeight - height) {
            $(this).window('move', {
                "top": parentHeight - height
            });
        }
    }
};
$.fn.panel.defaults.onMove = easyuiPanelOnMove;
$.fn.window.defaults.onMove = easyuiPanelOnMove;
$.fn.dialog.defaults.onMove = easyuiPanelOnMove;


//给combobox加上可以直接选择索引的方法  杨磊 2015-1-22
$.extend($.fn.combobox.methods, {
    selectIndex: function (jq, index) {
        var data = jq.combobox("getData");
        if (data && data.length) {
            if (isNaN(index) || (index >= data.length)) {
                index = 0;
            }
            jq.combobox("select", data[index][jq.combobox("options").valueField]);
        }
    }
});