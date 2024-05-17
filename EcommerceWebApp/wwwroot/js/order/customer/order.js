$(document).ready(() => {
    $("#update-shipping").click((e) => {
        e.preventDefault();

        const data = $("form").serialize();
       
        $.ajax({
            url: "/customer/api/order/updateshipping", 
            type: "POST",
            data: data,
            success: (response) => {
                Swal.fire({
                    title: "Update Successful",
                    text: "You have successfully update shipping detail",
                    icon: "success"
                }).then(() => {
                    $("input[name='orderHeader.Name']").value = response.name;
                    $("input[name='orderHeader.PhoneNumber']").value = response.phnomeNumber;
                    $("input[name='orderHeader.HomeNumber']").value = response.homenumber;
                    $("input[name='orderHeader.StreetName']").value = response.streetNumber;
                    $("input[name='orderHeader.Village']").value = response.village;
                    $("input[name='orderHeader.Commune']").value = response.commune;
                    $("input[name='orderHeader.City']").value = response.city;
                    $("input[name='orderHeader.PostalNumber']").value = response.postalNumber;
                })
            }
        });

    });
});