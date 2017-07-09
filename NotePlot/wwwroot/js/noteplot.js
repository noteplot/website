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

// входной параметр - элемент с атрибутом гиперссылки href
function OpenDialog(hr) {
    if ($('#masterDialog').length > 0)
        $('#masterDialog').dialog("close");
    //alert($(hr).attr("href"));                
    //console.log($(hr).attr("href")); 
    $("<div id = 'masterDialog'></div>")        
        .addClass("dialog")
        .appendTo("body")
        .dialog({
            autoOpen: false,
            modal: true,
            draggable: false,
            resizable: false,
            title: $(hr).attr("np_dialog_title"),   // читаем заголовок для диалогового окна
            close: function () {
                $(this).remove();
            }
        }).load($(hr).attr("href"), function () {   // читаем атрибут href для вызова формы
            $(this).dialog("open"); // Открываем диалог только после загрузки формы, в пртивном случае не будет центрирования
     });
}
