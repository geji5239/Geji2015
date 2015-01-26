$(document).ready(function () {
    $('.styleswitch').click(function () {
        switchStylestyle(this.getAttribute("rel"));
        return false;
    });
    var c = readCookie('style');
    if (c) switchStylestyle(c);
});

function switchStylestyle(styleName) {
    $('link[rel=stylesheet][title]').each(function (i) {
        this.disabled = true;
        if (this.getAttribute('title') == styleName) this.disabled = false;
    });

    $("iframe").contents().find('link[rel=stylesheet][title]').each(function (i) {
        this.disabled = true;
        if (this.getAttribute('title') == styleName) this.disabled = false;
    });

    createCookie('style', styleName, 365);
}
//cookie
function createCookie(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
}
function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
function eraseCookie(name) {
    createCookie(name, "", -1);
}
// /cookie functions

/*扩展easyui里的验证规则*/
$.extend($.fn.validatebox.defaults.rules, {
    minLength: {/*最小长度*/
        validator: function (value, param) {
            return value.length >= param[0];
        },
        message: '请输入至少 {0} 个字符.'
    },
    maxLength: {/*最大长度*/
        validator: function () {
            return value.length <= param[0];
        }
    },
    justLength: {/*只能输入多长*/
        validator: function (value, param) {
            return value.length == param[0];
        },
        message: '请输入{0}个字符'
    },
    CharAndNum: {/*只能输入多长的数组或者大写字符*/
        validator: function (value, param) {
            var strp = new RegExp("[0-9A-Z]{" + param[0] + "}");
            var b1 = strp.test(value);
            var b2 = value.length == param[0];
            return b1 & b2;
        },
        message: '请输入{0}位数字或者大写字母'
    },
    onlyNum: {/*只能输入多少位的数字*/  /*向sim卡号只能是11位*/
        validator: function (value, param) {
            var strp = new RegExp("[0-9]{" + param[0] + "}");
            var b1 = strp.test(value);
            var b2 = value.length == param[0];
            return b1 & b2;
        },
        message: '请输入{0}个数字'
    },
    intOrFloat: {// 验证整数或小数
        validator: function (value) {
            return /^\d+(\.\d+)?$/i.test(value);
        },
        message: '请输入数字，并确保格式正确'
    },
    equals: {
        validator: function (value, param) {
            return value == $(param[0]).val();
        },
        message: '密码不一致'
    },
    CharAndMinNum: {/*至少输入多长的数组或者大小写字符*/
        validator: function (value, param) {
            var strp = new RegExp("[0-9A-Za-z_\.\]{" + param[0] + "}");
            var b1 = strp.test(value);
            var b2 = value.length >= param[0];
            var b3 = value.length < 16;
            return b1 & b2 & b3;
        },
        message: '请输入至少{0}位数字或者大小写字母，不超过16位'
    },
    onlyInt: {
        validator: function (value) {
            return /^-?\d+$/i.test(value);
        },
        message: '请输入数字，并确保格式正确'
    },
    mobile: {
        validator: function (value) {
            return /^(13|15|18)\d{9}$/i.test(value) || /^(((0\d{3}[\-])?\d{7}|(0\d{2}[\-])?\d{8}|(0\d{3}[\-])?\d{8}))([\-]\d{2,4})?$/i.test(value);
            //return /(^[0-9]{3,4}\-[0-9]{3,8}$)|(^[0-9]{3,8}$)|(^\([0-9]{3,4}\)[0-9]{3,8}$)|(^0{0,1}13[0-9]{9}$)/i.test(value);
        },
        message: '请输入有效的电话号码格式'
    }
});