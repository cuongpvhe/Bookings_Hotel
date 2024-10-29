

async function submitRoomFormAjax() {
    const form = $('#addRoomForm');

    // Kiểm tra tính hợp lệ của form
    if (!validateRoomForm()) {
        return; // Dừng lại nếu form không hợp lệ
    }

    const roomNumber = document.getElementById("RoomNumber").value;

    try {
        const exists = await checkRoomNumberExists(roomNumber); // Kiểm tra số phòng

        if (exists) {
            showRoomNumberError();
        } else {
            const formData = new FormData();

 
            formData.append("RoomNumber", document.getElementById("RoomNumber").value);
            formData.append("RoomTypeId", document.getElementById("RoomTypeId").value);
            formData.append("Description", document.getElementById("Description").value);

            // AJAX request
            $.ajax({
                url: '/Manager/Room/Create?handler=Post', // URL tới phương thức xử lý
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                beforeSend: function () {
                    Swal.fire({
                        title: 'Processing',
                        text: 'Saving room details...',
                        allowOutsideClick: false,
                        showConfirmButton: false,
                        willOpen: () => {
                            Swal.showLoading();
                        }
                    });
                },
                success: function (response) {
                    if (response.success) {
                        Swal.fire("Success", "Room information saved successfully!", "success")
                            .then(() => window.location.href = '/Manager/Room/List'); // Redirect to rooms list
                    }
                },
                error: function (xhr, status, error) {
                    Swal.fire("Error", "There was an error saving the room information.", "error");
                    console.log(xhr.responseText);
                }

            });
        }
    } catch (error) {
        console.log("Error ", error);
    }
}

function validateRoomForm() {
    // Thiết lập validate cho form
    $('#addRoomForm').validate({
        rules: {
            RoomNumber: {
                required: true,
                digits: true
            },
            RoomTypeId: {
                required: true
            },
            RoomStatus: {
                required: true
            },
        },
        messages: {
            RoomNumber: {
                required: "Hãy nhập số phòng.",
                digits: "Số phòng phải là chữ số."
            },
            RoomTypeId: {
                required: "Hãy chọn loại phòng."
            },
            RoomStatus: {
                required: "Hãy chọn trạng thái phòng"
            },
        },
        errorPlacement: function (error, element) {
            error.addClass("text-danger");
            if (element.prop("tagName") === "SELECT") {
                error.insertAfter(element.next('.select2-container'));
            } else {
                error.insertAfter(element);
            }
        }
    });

    // Trả về kết quả hợp lệ của form
    return $('#addRoomForm').valid();
}


// Hàm kiểm tra số phòng đã tồn tại qua AJAX
function checkRoomNumberExists(roomNumber) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: '/Manager/Room/Create?handler=CheckRoomNumber', // URL tới phương thức kiểm tra
            type: 'POST',
            data: { roomNumber: roomNumber },
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                resolve(response.exists); // Trả về kết quả tồn tại của số phòng qua Promise
            },
            error: function () {
                Swal.fire("Error", "The room number is existed.", "error");
                reject();
            }
        });
    });
}

// Hàm hiển thị lỗi nếu số phòng đã tồn tại
function showRoomNumberError() {
    const roomNumberField = $("#RoomNumber");
    roomNumberField.addClass("is-invalid"); // Đánh dấu trường là không hợp lệ

    // Kiểm tra xem đã có thông báo lỗi chưa, nếu chưa thì thêm vào
    if (!$("#RoomNumber-error").length) {
        $("<span id='RoomNumber-error' class='text-danger'>This room number already exists.</span>")
            .insertAfter(roomNumberField);
    }
}
