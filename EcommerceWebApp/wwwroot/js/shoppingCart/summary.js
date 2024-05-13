/*
// Send ajax request to backend
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
                })
                .then((result) => {
                    if (result.isConfirmed) {
                        window.location.href = "/";
                    }
                });
            }
        });
    });
});
*/


$(document).ready(function () {

    let isCancel = false;

    // Alert and handle cancel
    /*
    $("#place-order-btn").click(function (e) {
        const form = $("form");

        Swal.fire({
            title: "Do you want to place an order?",
            text: "You will be taken to payment page!",
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Yes, place the order",
            cancelButtonText: "No, I still need to add more product",
            cancelButtonColor: "#A8A8A8",
            reverseButtons: true
        })
            .then(function (result) {
                if (!result.isConfirmed) {
                    isCancel = true;
                    console.log(isCancel)
                }
            });
    });
    */


    const form = $("#form");
    form.submit((e) => {
        e.preventDefault();
        Swal.fire({
            title: "Do you want to place an order?",
            text: "You will be taken to payment page!",
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Yes, place the order",
            cancelButtonText: "No, I still need to add more product",
            cancelButtonColor: "#A8A8A8",
            reverseButtons: true
        })
            .then(function (result) {
                if (result.isConfirmed) {
                    form.submit();
                }
                else {
                    return false;
                }
            });
    });

});