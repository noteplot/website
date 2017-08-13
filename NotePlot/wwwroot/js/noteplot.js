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