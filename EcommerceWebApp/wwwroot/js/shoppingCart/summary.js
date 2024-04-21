$(document).ready(function () {

    $("#place-order-btn").click(function (e) {
        e.preventDefault();
        const formData = $("form").serialize();

        $.ajax({
            url: "/Customer/api/cart/order",
            type: "POST",
            data: formData,
            success: function (response) {
                console.log(response)
                // Display success message using SweetAlert
                Swal.fire({
                    title: response.title,
                    text: response.message,
                    icon: "success"
                });                
            }
        });
    });
});