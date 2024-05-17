let dataTable;

$.ajax({
    url: "/admin/api/account/all",
    type: "GET",
    dataType: "json",
    success: function (data) {
        let users = [];

        data.data.forEach(function (item) {
            let user = {
                userId: item.id,
                userName: item.name,
                phoneNumber: item.phoneNumber,
                email: item.email,
                role: item.role,
                roleId: item.roleId,
                lockoutEnd: item.lockoutEnd
            };
            users.push(user);
        }); 

        $('#tb').DataTable({
            data: users,
            columns: [
                { data: "userName", "width": "20%" },
                { data: "role", "width": "15%" },
                { data: "phoneNumber", "width": "20%" },
                { data: "email", "width": "25%" },
                {
                    data: "lockoutEnd",
                    render: (data) => {
                        let today = new Date();
                        let lockout = new Date(data);
                        if (lockout > today) {
                            return "Locked";
                        }
                        else {
                            return "Unlocked";
                        }
                    }
                },
                {
                    data: "userId",
                    render: function (data) {
                        return `
                        <div class="btn-group text-center d-flex" role="group">
                            <a href="/admin/account/detail?id=${data}" class="btn btn-primary mx-1 w-50 rounded">View</a>
                        </div>
                    `
                    },
                    "Width": "15%",
                    "orderable": false
                }
            ]
        });
    },
    error: function (xhr, textStatus, errorThrown) {
        console.log("Error:", errorThrown);
    }
});