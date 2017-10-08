$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $(".openDialog").on("click", function (e) {
        e.preventDefault();
        OpenDialog(this);
    });

    $(window).resize(function (event) {
        if ($(".ui-dialog").length > 0) {
            $('#masterDialog').dialog("widget").dialogCenter();
        }
    });

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
function OpenDialog(hr) {
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
function np_ShowMessage(mes) {
    if ($('#np_MessageDialog').length > 0)
        $('#np_MessageDialog').dialog("close");
    $("<div id = 'np_MessageDialog'></div>")
        .addClass("dialog")
        .appendTo("body")
        .dialog({
            autoOpen: true,
            modal: true,
            draggable: false,
            resizable: false,
            close: function () {
                $(this).remove();
            }
        }).append(mes);
};

// AjaxPost - действие в href
function np_AjaxPost(event) {
    event.preventDefault();
    var _action = $(this).attr("href");
    np_AjaxBeforeSend();
    $.ajax({
        url: _action,
        type: 'POST',
        cache: false,
        async: false,
        data: { __RequestVerificationToken: event.data.token },
        success: function (data) {
            location.reload();
            np_AjaxComplete();
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
    console.log(_jsonT);
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
    console.log(_json);
    return _json;
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
    var _sJson = $(form_id).np_serializeForm(lName); // JSON - в сиде строки
    if ($(form_id + ' input').valid()) {
        np_AjaxBeforeSend();
        $.ajax({
            url: $(form_id).attr('action'),
            data: 'JSON='+ '"' + _sJson + '"',// Чтобы передать строку  data: 'id="Hello"'; - только так работает
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

