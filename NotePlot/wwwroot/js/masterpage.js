// DataTables значения по-умолчанию
// Русификация DataTable
var dtLanguage = {
    "processing": "Подождите...",
    "search": "Поиск:",
    "lengthMenu": "Показать _MENU_ записей",
    "info": "Записи с _START_ до _END_ из _TOTAL_ записей",
    "infoEmpty": "Записи с 0 до 0 из 0 записей",
    "infoFiltered": "(отфильтровано из _MAX_ записей)",
    "infoPostFix": "",
    "loadingRecords": "Загрузка записей...",
    "zeroRecords": "Записи отсутствуют.",
    "emptyTable": "В таблице отсутствуют данные",
    "paginate": {
        "first": "Первая",
        "previous": "Предыдущая",
        "next": "Следующая",
        "last": "Последняя"
    },
    "aria": {
        "sortAscending": ": активировать для сортировки столбца по возрастанию",
        "sortDescending": ": активировать для сортировки столбца по убыванию"
    }
}
$.extend(true, $.fn.dataTable.defaults, { language: dtLanguage });

$(document).ready(function () {
    $('button.dropdown-main-menu').click(function () {
        if ($('.np_sidebar').css('display') == 'block') {
            $('.np_sidebar').css('display', 'none');
            $('.np_body').css('margin-left', 0);
        }
        else {
            $('.np_sidebar').css('display', 'block');
            $('.np_body').css('margin-left', '220px');
        }
    })

    $(window).resize(function (event) {
        $('.np_sidebar').attr('style', '');
        $('.np_body').attr('style', '');
    });

    // Гашения значка загрузки
    //$(document).ready(function () {
    $('#loader').hide();
    $('.np_content').show();
    //});
});
