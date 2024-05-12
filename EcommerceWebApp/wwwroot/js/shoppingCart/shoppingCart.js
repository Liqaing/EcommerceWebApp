
$(document).ready(function() {

    $(".minus").click(function () {

        let cartId = $(this).data("cart-id");

        $.ajax({
            url: `/Customer/Cart/Minus?cartId=${cartId}`,
            type: "POST",
            complete: function (res) {
                window.location.reload();
                /*
                if (res.complete) {
                    if (res.quantity == 0) {
                        $(`#${cartId}`).remove();
                    }
                    else {
                        $(`#quantity-${cartId}`).val(res.qty);
                    }
                }
                */                
            }
        })
    });

    $(".add").click(function () {

        let cartId = $(this).data("cart-id");

        $.ajax({
            url: `/Customer/Cart/Add?cartId=${cartId}`,
            type: "POST",
            complete: function (res) {
                window.location.reload();
                
                //$(`#quantity-${cartId}`).val(res.qty);               
            }
            
        })
    });

    $(".remove").click(function () {

        let cartId = $(this).data("cart-id");
        console.log(cartId)
        $.ajax({
            url: `/Customer/Cart/Remove?cartId=${cartId}`,
            type: "POST",
            complete: function (res) {
                window.location.reload();
                
                //$(`#${cartId}`).remove();
                }
            })
        })
    });

    $("#proceed-to-pay").click(() => {
        //e.preventDefault();
        console.log("test")

        //const data = $("#form").serialize();
        //console.log(data)
        $.ajax({
            url: "Cart/Checkout",
            type: "POST",
            //data: data,
            success: (res) => {
                if (res.success) {
                    window.location.href = "/Customer/Cart/Summary";
                }
                else {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: res.msg
                    });
                }
                    
            }
        });

    });
