// Write your JavaScript code.
$('.item-4').mouseenter(function () {
    $(this).addClass('active');
    $('.base-img').removeClass('active');
    $('.second-img').removeClass('active');
    $('.third-img').addClass('active');
});
$('.item-4').mouseleave(function () {
    $(this).removeClass('active');
    $('.base-img').addClass('active');
    $('.second-img').removeClass('active');
    $('.third-img').removeClass('active');
});
$('.item-1, .item-2, .item-3, .item-5').mouseenter(function () {
    $(this).addClass('active');
    console.log("enter", $(this).css());
    $('.base-img').addClass('active');
    $('.second-img').removeClass('active');
    $('.third-img').removeClass('active');
});
$('.item-1, .item-2, .item-3, .item-5').mouseleave(function () {
    $(this).removeClass('active');
    console.log("leave", $(this).css());
});
$('.item-6').mouseenter(function () {
    $(this).addClass('active');
    $('.base-img').removeClass('active');
    $('.second-img').addClass('active');
    $('.third-img').removeClass('active');
});
$('.item-6').mouseleave(function () {
    $(this).removeClass('active');
    $('.base-img').addClass('active');
    $('.third-img').removeClass('active');
    $('.second-img').removeClass('active');
});
