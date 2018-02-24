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

    $('#loader').hide();
    $('.np_content').show();
});
