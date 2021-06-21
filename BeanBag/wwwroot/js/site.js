// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(window).on("load", function () {
    /*Preloader*/
    $(".preloader").fadeOut("slow");
});
/*Navbar Collapse*/
$(".nav-link").on("click", function (){ /* check the behaviour of this function*/
    $(".navbar-collapse").collapse("hide");
});
/*  ==========================================
    SHOW UPLOADED IMAGE
* ========================================== */
function readURL(input) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            $('#imageResult')
                .attr('src', e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
    }
   /* document.getElementById("item-details").innerHTML += "<p>"+"Item details"+"</p>"+"Name: Chair" + "<br>" +
        "Inventory: Furniture inventory" + "<br>" +
        "Type: Furniture" + "<br>" +
        "scanDate : 2021/06/04";*/
    
}

$(function () {
    $('#upload').on('change', function () {
        readURL(input);
    });
});
