﻿ $(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $(".openDialog").on("click", function (e) {
        e.preventDefault();
        OpenDialog(this);
    });

    $(window).resize(function (event) {
        if ($(".ui-dialog").length > 0) {
            if ($('#masterDialog').length > 0)
                $('#masterDialog').dialog("widget").dialogCenter();
            if ($('#np_MessageDialog').length > 0)                
                $('#np_MessageDialog').dialog("widget").dialogCenter();
        }
    });
/*
    // добавление токена для запросов POST
    $.ajaxPrefilter(
        function (options, localOptions, jqXHR) {
            if (options.type !== "GET") {
                var token = GetAntiForgeryToken();
                if (token !== null) {
                    if (options.data.indexOf("X-Requested-With") === -1) {
                        options.data = "X-Requested-With=XMLHttpRequest" + ((options.data === "") ? "" : "&" + options.data);
                    }
                    options.data = options.data + "&" + token.name + '=' + token.value;
                }
            }
        }
    );
*/
});

// ф-ция для центрирования диалога при изменении window в гаджетах
jQuery.fn.dialogCenter = function () {
    //this.css("position", "fixed");
    var y = ($(window).height() - this.outerHeight()) / 2;
    var x = ($(window).width() - this.outerWidth()) / 2;
    this.css("top", y + "px").css("left", x + "px");
    return this;
}

// создание диалоговой формы
// входной параметр - элемент с атрибутом гиперссылки href
function OpenDialog(hr,OnClose) {
    if ($('#masterDialog').length > 0)
        $('#masterDialog').dialog("close");

    $("<div id = 'masterDialog'></div>")        
        .addClass("dialog")
        .appendTo("body")
        .dialog({
            autoOpen: false,
            modal: true,
            draggable: false,
            resizable: false,
            title: $(hr).attr("np_dialog_title"),   // читаем заголовок для диалогового окна
            width: $(hr).attr("np_dialog_width"),   // устанавливаем ширину диалога, если она задана
            close: function () {
                if (OnClose) {
                    OnClose();
                }                    
                $(this).remove();
            }
        }).load($(hr).attr("href"), function () {   // читаем атрибут href для вызова формы
            $(this).dialog("open"); // Открываем диалог только после загрузки формы, в пртивном случае не будет центрирования
     });
}

// для AjaxForm - если все OK перегружается текущая страница
function np_AjaxFormSubmit(event) {
    event.preventDefault();
    var form_id = $(this).attr('id');
    if (form_id) { form_id = '#' + form_id };
    // если не использовать хелпер Html.CheckBox, то установка checkbox обязательна
    $("input:checkbox").each(function () { // this - это не объект jQuery , а html-element
        this.value = (this.checked == true);
    });
    if ($(form_id + ' input').valid()) {
        np_AjaxBeforeSend();
        $.ajax({
            url: $(form_id).attr('action'),
            data: $(form_id).serialize(),
            type: 'POST',
            cache: false,
            async: false,
            success: function (data) {
                //alert(document.location.href); 
                //event.data.onSuccess(data); // передача ф-ции через класс
                np_AjaxComplete();             
                if (event.data && event.data.onSuccess) {
                    event.data.onSuccess();
                }
                if ($(form_id).attr('np_reload') == "true") {
                    location.reload();
                }                
            },
            error: function (jqXHR, textStatus, errorThrown) { // панель ошибок формы
                np_AjaxComplete();
                $(form_id).after("<div class='errorForm'><span></span></div>");
                $('.errorForm span').html(jqXHR.responseText);
            }
        });        
    };
};

//валидация - сброс ошибок по клику 
function np_AjaxFormInputClick(event) {  // очистка сообщений об ошибках
    $("label.error").remove();           // это элемент валидатора
    $(".errorForm").remove();            // это панель ошибок формы    
};

function np_AjaxBeforeSend() {
    $('#loader').show();
};

function np_AjaxComplete() {
    $('#loader').hide();
};

// простое сообщение, например об ошибке TODO: сделать полноправный диалог
function np_ShowMessage(mes, tit) {
    if ($('#np_MessageDialog').length > 0)
        $('#np_MessageDialog').dialog("close");
    if (tit == undefined)
        tit = '';
    $("<div id = 'np_MessageDialog'></div>")
        .addClass("dialog")
        .appendTo("body")
        .dialog({
            title: tit,
            autoOpen: open,
            modal: true,
            draggable: false,
            resizable: false,
            close: function () {
                $(this).remove();
            },
            /*
            buttons: {
                Ok: function () {
                    $(this).dialog("close");
                }
            },
            */
            buttons: [
                {
                    text: "Ok",
                    //icon: "ui-icon-heart",
                    click: function () {
                        $(this).dialog("close");
                    }
                }
            ]
            
        }).append(mes);
};

function np_MessageDialogPost(event) {
    event.preventDefault();
    event.data.action = $(this).attr("href");
    if ($('#np_MessageDialog').length > 0)
        $('#np_MessageDialog').dialog("close");
    var tit = event.data.dialogTitle;
    var mes = event.data.dialogMessage;
    if (tit == undefined)
        tit = 'Внимание!';
    if (mes == undefined)
        mes = 'Вы уверены?';

    $("<div id = 'np_MessageDialog'></div>")
        .addClass("dialog")
        .appendTo("body")
        .dialog({
            title: tit,
            autoOpen: open,
            modal: true,
            draggable: false,
            resizable: false,
            close: function () {
                $(this).remove();
            },
            /*
            buttons: {
                Ok: function () {
                    $(this).dialog("close");
                }
            },
            */
            buttons: [
                {
                    text: "Ok",
                    //icon: "ui-icon-heart",
                    click: function () {
                        $(this).dialog("close");                        
                        np_AjaxPost(event);
                    }
                },
                {
                    text: "Cancel",
                    //icon: "ui-icon-heart",
                    click: function () {
                        $(this).dialog("close");
                    }
                },
            ]

        }).append(mes);
};

// AjaxPost - действие в href
function np_AjaxPost(event) {
    event.preventDefault();
    var _action = (event.data.action) ? event.data.action : $(this).attr("href");
    /*
    var _action = $(this).attr("href");
    if (_action == undefined) // вызов из диалога
        _action = event.data.action;
    */
    var _token = null;
    if (event.data.token)
        _token = { __RequestVerificationToken: event.data.token };
    np_AjaxBeforeSend();

    $.ajax({
        url: _action,
        type: 'POST',
        cache: false,
        async: false,
        data: _token,//{ __RequestVerificationToken: event.data.token },
        success: function (data) {
            //location.reload();
            np_AjaxComplete();
            if (event.data && event.data.onSuccess) {
                event.data.onSuccess();
            }
            else
                location.reload();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            np_AjaxComplete();
            np_ShowMessage(jqXHR.responseText);
        }
    });
};

// ф-ция для сериализации формы: поля ввода и одна таблица
// lName - название списка в объекте JSON
jQuery.fn.np_serializeForm = function (lName) {
    var data = $(this).find(":not(td)>input,:not(td)>select,:not(td)>textarea").serializeArray();
    //console.log(data);
    //alert(data);
    var _json = ''
    var _s = '';
    $.each(data, function () {
        if (this.name !== undefined) {
            s = '"' + this.name + '"' + ' : ' + '"' + this.value + '"'
            if (s.length > 0 & _json.length > 0) { s = ',' + s }
            _json = _json + s;
        };
    });
    console.log(_json);
    //2 cериализация таблицы
    var data = $(this).find("td>input,td>select,td>textarea").serializeArray();
    //console.log(data);
    //alert(data);
    var _jsonT = ''
    s = '';
    var _i = 0; var fs = 2;
    var j = 0
    $.each(data, function () {
        if (this.name !== undefined) {
            s = '"' + this.name + '"' + ' : ' + '"' + this.value + '"'
            if (s.length > 0) {
                if (j == 0)
                    if (_jsonT.length > 0) {
                        s = ',{' + s;
                    }
                    else {
                        s = '{' + s;
                    }
                else {
                    s = ',' + s
                }
            }
            j = j + 1;
            if (j == fs) {
                s = s + '}';
                j = 0;
            }
            if (_jsonT.length == 0) { s = '[' + s; }

            _jsonT = _jsonT + s;
        };
    });
    //console.log(_jsonT);
    if (_jsonT == '{}' || _jsonT == '') {
        _jsonT = null
    }
    else { _jsonT = _jsonT + ']' }
    //alert(_jsonT);
    if (_jsonT) {
        _json = '{' + _json + ',' + lName + ' : ' + _jsonT + '}'
    }
    else {
        _json = '{' + _json + '}'
    }
    //console.log(_json);
    return _json;
};

// ф-ция для сериализации таблицы формы
jQuery.fn.np_serializeTable = function (fs) {
    //cериализация таблицы
    var data = $(this).find("td>input,td>select,td>textarea").serializeArray();
    var _jsonT = ''
    s = '';
    var _i = 0; //var fs = 2;
    var j = 0
    $.each(data, function () {
        if (this.name !== undefined) {
            s = '"' + this.name + '"' + ' : ' + '"' + this.value + '"'
            if (s.length > 0) {
                if (j == 0)
                    if (_jsonT.length > 0) {
                        s = ',{' + s;
                    }
                    else {
                        s = '{' + s;
                    }
                else {
                    s = ',' + s
                }
            }
            j = j + 1;
            if (j == fs) {
                s = s + '}';
                j = 0;
            }
            _jsonT = _jsonT + s;
        };
    });
    if (_jsonT == '') {
        _jsonT = null
    }
    return _jsonT;
};


// для AjaxForm - если все OK перегружается текущая страница
// для Form требующих сложную сериализацию в JSON
// для AjaxForm - если все OK перегружается текущая страница
function np_AjaxFormSubmitEx(event) {
    event.preventDefault();
    var lName = 'LIST';
    if (event.data.lName) {
        lName = event.data.lName;
    }
    var form_id = $(this).attr('id');
    if (form_id) { form_id = '#' + form_id };
    // если не использовать хелпер Html.CheckBox, то установка checkbox обязательна
    $("input:checkbox").each(function () { // this - это не объект jQuery , а html-element
        this.value = (this.checked == true);
    });
    //_sJson = "HELLO";
    var fs = 2; // кол-во полей по-умолчанию - для парсинга, передается через event.data.fs
    if (event.data.fs) {
        fs = event.data.fs;
    }

    var _sJson = $(form_id).np_serializeTable(fs); // JSON - в сиде строки
    //var _data = 'JSON=' + '"' + _sJson + '"';
    /*
    var token = GetAntiForgeryToken();
    if (token !== null) {
        _data = _data + "&" + token.name + '=' + token.value;
    }
    */
    _data = $(form_id).find(":not(td)>input,:not(td)>select,:not(td)>textarea").serialize();
    var token = GetAntiForgeryToken();
    if (token !== null) {
        _data = _data + "&" + token.name + '=' + token.value;
    }
    if (_sJson !== null) {
        _data = _data + "&" +  'JSON=' + '"' + _sJson + '"';
    }

    if ($(form_id + ' input').valid()) {
        np_AjaxBeforeSend();
        $.ajax({
            url: $(form_id).attr('action'),
            data: _data,//'JSON='+ '"' + _sJson + '"',// Чтобы передать строку  data: 'id="Hello"'; - только так работает
            type: 'POST',
            //contentType: "application/json; charset=utf-8", 
            cache: false,
            async: false,
            success: function (data) {
                //alert(document.location.href); 
                //event.data.onSuccess(data); // передача ф-ции через класс
                np_AjaxComplete();
                if (event.data && event.data.onSuccess) {
                    event.data.onSuccess();
                }
                if ($(form_id).attr('np_reload') == "true") {
                    location.reload();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) { // панель ошибок формы
                np_AjaxComplete();
                $(form_id).after("<div class='errorForm'><span></span></div>");
                $('.errorForm span').html(jqXHR.responseText);
            }
        });
    };
};

/*
function np_AjaxFormSubmitEx(event) {
    event.preventDefault();
    var lName = 'LIST';
    if (event.data.lName) {
        lName = event.data.lName;
    }
    var form_id = $(this).attr('id');
    if (form_id) { form_id = '#' + form_id };
    // если не использовать хелпер Html.CheckBox, то установка checkbox обязательна
    $("input:checkbox").each(function () { // this - это не объект jQuery , а html-element
        this.value = (this.checked == true);
    });
    if ($(form_id + ' input').valid()) {
        //var _sJson = $(form_id).np_serializeForm(lName); // JSON
        //var _sJson = '{ "name" : "Hans Sarpei" }';
        //var dataJson = JSON.stringify(_sJson);
        var dataJson = 'hello'; 
        //np_ShowMessage(_sJson);
        //return;
        np_AjaxBeforeSend();        
        $.ajax({
            url: $(form_id).attr('action'),
            type: 'POST',
            data: $(form_id).serialize(),//"{'param1': 'Fp1'}",//$(form_id).serialize(),//          
            contentType: "application/json; charset=utf-8",
            cache: false,
            async: false,
            success: function (data) {
                //alert(document.location.href); 
                //event.data.onSuccess(data); // передача ф-ции через класс
                np_AjaxComplete();
                if (event.data && event.data.onSuccess) {
                    event.data.onSuccess();
                }
                if ($(form_id).attr('np_reload') == "true") {
                    location.reload();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) { // панель ошибок формы
                np_AjaxComplete();
                $(form_id).after("<div class='errorForm'><span></span></div>");
                $('.errorForm span').html(jqXHR.responseText);
            }
        });
    };    
};
*/


function GetAntiForgeryToken() {
    var tokenField = $("input[type='hidden'][name$='RequestVerificationToken']");
    if (tokenField.length == 0) {
        return null;
    } else {
        return {
            name: tokenField[0].name,
            value: tokenField[0].value
        };
    }
};

function fix_block_items_width(block, what, to) {
    $(block).each(function () {

        var iWidth = $(this).width();

        if (what.length > 0) {

            for (var i = 0; i < what.length; i++) {
                $(this).find(what[i]).each(function () {
                    iWidth -= $(this).width() + (parseInt($(this).css('padding-left')) * 3 - 8);
                });
            }
            $(this).find(to).width(iWidth - 14);

        }
    });

}
function fix() {
    fix_block_items_width('.input-prepend', ['button.add-on'], 'input');
}

//Класс  для вставки строки в таблицу
//В таблице в ячейке указывается название класса == названию поля 
function np_InsertRow(tid, tr, fds) {
    this.tableId = tid;      // Id таблицы, куда добавляется строка
    this.rowTemplate = tr;   // шаблон строки вставки
    this.fields = fds;       // список добавляемых полей
};

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.split(search).join(replacement);
};

// возращает дату как строку в соответсвии с форматом
function np_getStringDate(dt, rType, fDate, fTime, dSep, tSep) {
    var dt_str = '';
    var tm_str = '';
    if (!dSep) { dSep = '.' }
    if (!tSep) { tSep = ':' }
    if ((rType == 'time') || (rType == 'date_time')) {
        // часы
        var hh = String(dt.getHours());
        if (hh.length == 1) {
            hh = '0' + hh;
        }
        //минуты
        var nn = String(dt.getMinutes());
        if (nn.length == 1) {
            nn = '0' + nn;
        }
        //секунды
        var ss = String(dt.getSeconds());
        if (ss.length == 1) {
            ss = '0' + ss;
        }
        //форматируем
        if ((fTime.toLowerCase() == 'hh' + tSep + 'nn') || (fTime.toLowerCase() == 'hh' + tSep + 'mm')) {
            tm_str = hh + tSep + nn
        }
        else
            if ((fTime.toLowerCase() == 'hh' + tSep + 'nn' + tSep + 'ss') || (fTime.toLowerCase() == 'hh' + tSep + 'mm' + tSep + 'ss')) {
                tm_str = hh + tSep + nn + tSep + ss
            }
    }
    if ((rType == 'date') || (rType == 'date_time')) {
        //день
        var dd = String(dt.getDate());
        if (dd.length == 1) {
            dd = '0' + dd;
        }
        //месяц
        var MM = String(dt.getMonth() + 1);
        if (MM.length == 1) {
            MM = '0' + MM;
        }
        //Год
        var yy = String(dt.getFullYear());

        //форматируем
        // ru
        if (fDate.toLowerCase() == 'dd' + tSep + 'mm' + tSep + 'yy') {
            dt_str = dd + dSep + MM + dSep + yy
        }
        //en
        else
            if (fDate.toLowerCase() == 'mm' + tSep + 'dd' + tSep + 'yy') {
                dt_str = MM + dSep + dd + dSep + yy
            }
    }
    if (rType == 'time') { return tm_str }
    else if (rType == 'date') { return dt_str }
    else if (rType == 'date_time') { return dt_str + ' ' + tm_str }
    else return '';
};
