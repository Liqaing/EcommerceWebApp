
$(document).ready(function (e) {
    e.preventDefault();

    $("#add-to-cart").click(function () {

        let form = $("form").serialize();

        $.ajax({
            url: `/customer/api/cart/add`,
            type: "POST",
            data: data,
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        title: "Add to cart successful",
                        text: response.message,
                        icon: "success"
                    }).then(() => {
                        window.location.href = "https://localhost:7137/";
                    })
                }
                else {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: response.message
                    });
                }
            }

        })
    });

});
