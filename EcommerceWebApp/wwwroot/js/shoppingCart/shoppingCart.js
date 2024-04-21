
$(document).ready(function() {

    $(".minus").click(function () {

        let cartId = $(this).data("cart-id");

        $.ajax({
            url: `/Customer/Cart/Minus?cartId=${cartId}`,
            type: "POST",
            complete: function () {
                window.location.reload();
            }
        })
    });

    $(".add").click(function () {

        let cartId = $(this).data("cart-id");

        $.ajax({
            url: `/Customer/Cart/Add?cartId=${cartId}`,
            type: "POST",
            complete: function () {
                window.location.reload();
            }
        })    
    });

    $(".remove").click(function () {

        let cartId = $(this).data("cart-id");

        $.ajax({
            url: `/Customer/Cart/Remove?cartId=${cartId}`,
            type: "POST",
            complete: function () {
                window.location.reload();
            }
        })
    });

});
