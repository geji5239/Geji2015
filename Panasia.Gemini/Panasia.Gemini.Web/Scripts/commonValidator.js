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
    CharUpper: {/*只能输入多长大写字符*/
        validator: function (value, param) {
            var strp = new RegExp("[A-Z]{" + param[0] + "}");
            var b1 = strp.test(value);
            var b2 = value.length == param[0];
            return b1 & b2;
        },
        message: '请输入{0}位大写字母'
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
    CharAndMaxNum: {/*至多输入多长的数组或者大小写字符*/
        validator: function (value, param) {
            var strp = new RegExp("[0-9A-Za-z]{" + param[0] + "," + param[1] + "}");
            var b1 = strp.test(value);
            var b2 = value.length >= param[0];
            var b3 = value.length <= param[1];
            return b1 & b2 & b3;
        },
        message: '请输入至少{0}位至多{1}位数字或英文字母组成的字符串'
    },
    onlyInt: {
        validator: function (value) {
            return /^-?\d+$/i.test(value);
        },
        message: '请输入数字，并确保格式正确'
    },
    mobile: {
        validator: function (value) {
            return /^(13|15|17|18)\d{9}$/i.test(value) || /^(((0\d{3}[\-])?\d{7}|(0\d{2}[\-])?\d{8}|(0\d{3}[\-])?\d{8}))([\-]\d{2,4})?$/i.test(value);
            //return /(^[0-9]{3,4}\-[0-9]{3,8}$)|(^[0-9]{3,8}$)|(^\([0-9]{3,4}\)[0-9]{3,8}$)|(^0{0,1}13[0-9]{9}$)/i.test(value);
        },
        message: '请输入有效的电话号码格式'
    },
    contactTax: {
            validator: function (value) {
                return  /^(((0\d{3}[\-])?\d{7}|(0\d{2}[\-])?\d{8}|(0\d{3}[\-])?\d{8}))([\-]\d{2,4})?$/i.test(value);
    //return /(^[0-9]{3,4}\-[0-9]{3,8}$)|(^[0-9]{3,8}$)|(^\([0-9]{3,4}\)[0-9]{3,8}$)|(^0{0,1}13[0-9]{9}$)/i.test(value);
    },
        message: '请输入有效的传真号码格式'
        },
    dateCompare: {//传入“datebox”控件标示字符串
        validator: function (value, param) {
            var d1 = $.fn.datebox.defaults.parser($(param[0]).datebox('getValue'));
            var d2 = $.fn.datebox.defaults.parser(value);
            return d2 >= d1;
        },
        message: '日期必须大于等于某日期'
    },
    dateCompareII: {//传入“datebox”控件标示字符串
        validator: function (value, param) {
            var end = $(param[0]).datebox('getValue');
            if (end == '')
                end = '2100-01-01';
            var d1 = $.fn.datebox.defaults.parser(end)
            var d2 = $.fn.datebox.defaults.parser(value);
            return d2 <= d1;
        },
        message: '日期必须小于等于某日期'
    },
    dateCompareIII: {//传入“datebox”控件标示字符串
        validator: function (value, param) {
            var beg = $(param[0]).datebox('getValue');
            if (beg == '')
                beg = '1900-01-01';
            var end = $(param[1]).datebox('getValue');
            if (end == '')
                end = '2100-01-01';
            var d1 = $.fn.datebox.defaults.parser(beg);
            var d3 = $.fn.datebox.defaults.parser(end);
            var d2 = $.fn.datebox.defaults.parser(value);
            return d1 <= d2 && d2 <= d3;
        },
        message: '日期必须在两日期之间'
    },
    dateCompareNow: {//使用方法,传入">"、"<"或“textbox”控件标示字符串
        validator: function (value, param) {
            var nowDate = new Date();
            var d1 = $.fn.datebox.defaults.parser(nowDate.toLocaleDateString());//当前日期
            var d2 = $.fn.datebox.defaults.parser(value);//控件日期
            if (param[0] == '<') {        //验证控件日期小于当前日期
                return d2 <= d1;
            }
            else if (param[0] == '>') {     //验证控件日期大于当前日期
                alert(d2 + ">=" + d1);
                return d2 >= d1;
            }
            else {
                var d3 = $.fn.datebox.defaults.parser($(param[0]).val());      //传入“textbox”控件标示字符串
                return (d1 >= d2 && d2 >= d3) || (d1 <= d2 && d2 <= d3);        //验证控件日期在传入的textbox控件显示的日期和当前日期之间
            }
        },
        message: '预计到期日期必须小于\大于当前日期'
    },
    timeCompareNow: {//使用方法,传入">"、"<"或“textbox”控件标示字符串
        validator: function (value, param) {
            var timeNow = new Date();//当前日期 
            var d1 = Date.parse(timeNow)
            var d2 = Date.parse(value.replace(/-/g, "/"));//控件日期(IE，火狐不识别‘-’，识别‘/’)
            if (param[0] == '<') {        //验证控件日期小于当前日期
                return d2 <= d1;
            }
            else if (param[0] == '>') {     //验证控件日期大于当前日期
                return d2 >= d1;
            }
            else {
                var value2 = $(param[0]).datetimebox('getValue'); //传入“textbox”控件标示字符串
                var d3 = Date.parse(value2.replace(/-/g, "/"));
                return (d1 >= d2 && d2 >= d3) || (d1 <= d2 && d2 <= d3);        //验证控件日期在传入的textbox控件显示的日期和当前日期之间
            }
        },
        message: '预计到期日期必须小于\大于当前日期'
    },
    timeCompare: {
        validator: function (value, param) {
            var d2 = Date.parse(value.replace(/-/g, "/"));//控件日期(IE，火狐不识别‘-’，识别‘/’)

                var value2 = $(param[0]).datetimebox('getValue'); //传入“textbox”控件标示字符串
                var d3 = Date.parse(value2.replace(/-/g, "/"));
                return  d2 > d3      
            
        },
        message: '预计到期日期必须大于*日期'
    },
    onlyTxt: {//Excel格式验证
        validator: function (value, param) {
            return /^.*\.(?:txt)$/i.test(value) ;
        },
        message: '只能选择txt格式文件'
    },
    onlyExcel: {//Excel格式验证
        validator: function (value, param) {
            return /^.*\.(?:xls)$/i.test(value) || /^.*\.(?:xlsx)$/i.test(value) || /^.*\.(?:csv)$/i.test(value);
        },
        message: '只能选择excel格式文件'
    },
    onlyPicture: {//图片格式验证
        validator: function (value, param) {
            return /^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.JPEG|.jpeg|.JPG|.jpg|.GIF|.gif|.BMP|.bmp|.PNG|.png)$/i.test(value)
        },
        message: '只能选择图片格式文件'
    },
    bankInt: {
        validator: function (value, param) {
            var strp = new RegExp("[0-9]{" + param[0] + "}");
            var strp1 = new RegExp("[0-9]{" + param[1] + "}");
            var b1 = strp.test(value);
            var b2 = value.length == param[0];
            var b3 = strp1.test(value);
            var b4 = value.length == param[1];
            return b1 & b2 ||b3 &b4;
        },
        message: '只能输入16或19个数字'
    },
    identityCard: {
        validator: function (value, param) {
            //身份证正则表达式(15位)    
            //isIDCard1 = /^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$/;

            //身份证正则表达式(18位)    
          //  isIDCard2 = /^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{4}$/;
            //身份证正则表达式(18位,X结尾)    
           // isIDCard3 = /^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}X$/;

            isIDCard = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
            //return isIDCard1.test(value) || isIDCard2.test(value) || isIDCard3.test(value);
            return isIDCard.test(value);
        },
        message:'身份证号输入有误请重新输入'
    },
    iswebsite: {
        validator: function (value, param) {
            iswebsite =/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?/;
            var re = new RegExp(iswebsite);
            return iswebsite.test(value)
        },
        message: '请输入有效的网站地址格式'
    },
    postcode: {
        validator: function (value, param) {
            ispostcode = /[1-9]\d{5}(?!\d)/;
            return ispostcode.test(value)
        },
        message: '请输入有效的邮编格式'
    },
    dateCompareArray: {//传入“datebox”控件标示字符串
        validator: function (value, param) {
            if (value.length > param[2]) {
                return false;
            } else {
                var d1 = $.fn.datebox.defaults.parser($(param[0]).datebox('getValue'));
                var d2 = $.fn.datebox.defaults.parser($(param[1]).datebox('getValue'));
                var array = new Array();
                if (value.indexOf(",") != -1) {
                    array = value.split(",");
                    for (i = 0; i < array.length; i++) {
                        var d3 = $.fn.datebox.defaults.parser(array[i]);
                        if (d3 > d1 && d3 < d2) {
                            return false
                        }
                    }
                } else {
                    var d3 = $.fn.datebox.defaults.parser(value);
                    return d3 > d2 || d3 < d1
                }
                return true
            }
        },
        message: '日期长度不得超过{2},日期不得在开始日期和结束日期范围之间'
    },
    PLCompare: {//最大调休时间验证
        validator: function (value, param) {
            var comparedDeimal = $(param[0]).textbox("getValue").replace("小时", "");
            var applyValue = value;
            var unit = $(param[1]).combobox("getValue");
            if (unit == "D")
            {
                applyValue = applyValue * 8;
            }
            return eval(applyValue + param[2] + comparedDeimal);
        },
        message:''
    },
    AnnualLeaveCompare: {//fgh 20141121 年假管理 判断已休年假数不可以大于年假天数 验证提示 
        validator: function (value, param) {
            var d1 = $(param[0]).textbox('getValue');
            var d2 = value;
            console.info(d1)
            //console.info(d2)
            //console.info(parseInt(d2) >= parseInt(d1))
            return parseInt(d2) <= parseInt(d1);
        },
        message: '已休年假数不可以大于年假天数'
    },
    orderReciveRecord: {//订单到账金额验证
        validator: function (value, param) {
            var MoneyUsable = $(param[0]).val();
            if (MoneyUsable == "" || isNaN(MoneyUsable))
            {
                return true;
            }
            else
            {
                return eval(value + "<=" + MoneyUsable);
            }
        },
        message: '到账金额不能超过该流水的可用余额'
    },

    oldpassword: {//验证原密码是否输入正确
            validator: function (value, param) {
                return value == $(param[0]).val();
            },
            message: '密码不一致'
    },
    oldeqnew:{//验证新密码和原密码不同
        validator: function (value, param) {
            return value != $(param[0]).val();
        },
        message: '新密码不能和原密码相同'
    },
    password: {
        validator: function (value) {//验证密码长度和复杂度
            return /^[\@A-Za-z0-9\!\#\$\%\^\&\*\.\~]{6,22}$/i.test(value);
        },
        message: '密码长度为6-22位，可包含数字和字母。'
    },


    yearmonth: {//gj YYYY-mm 年月格式验证
    validator: function (value) {
        return /((20[0-9][0-9])|(19[0-9][0-9]))-((0[1-9])|(1[0-2])){0,7}$/i.test(value);
    },
    message:'请按正确格式输入年月'
},
    decimalRange: {// 验证数字范围(条件限制只能验证最大或最小)
        validator: function (value, param) {
            return eval(value + param[0]);
        },
        message: ''
    }
});


