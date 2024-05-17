
    $(document).ready(function () {

        $("#form").submit(function (e) {
            e.preventDefault();
           
            const proId = parseInt($("#productId").val());
            const quantity = parseInt($("#quantity").val());
            const maxQuantity = parseInt($("#MaxQuantity").val());

            if (quantity > maxQuantity) {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: `Sorry, we could provide quatity: ${quantity} at the moment`
                });
                return false
            }

            $.ajax({
                url: "/customer/api/cart/add",
                type: "POST",
                data: { proId: proId, quantity: quantity },
                success: (res) => {

                    if (res.success) {
                        Swal.fire({
                            title: "Add to cart successful",
                            text: res.message,
                            icon: "success"
                        }).then(() => {
                            window.location.href = "https://localhost:7137/";
                        })
                    }
                    else {
                        Swal.fire({
                            icon: "error",
                            title: "Oops...",
                            text: res.message
                        });
                    }
                }
            })             
        });

    });
