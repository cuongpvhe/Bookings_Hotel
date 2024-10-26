﻿$(document).ready(function () {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))
})

// image
let imageIndex = 0;
const uploadedImages = {};

let cropper;
let currentImageIndex;

function handleImageUpload(input) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            $('#cropImageModal').modal('show');
            $('#imageToCrop').attr('src', e.target.result);
            $('#cropImageModal').on('shown.bs.modal', function () {
                if (cropper) {
                    cropper.destroy();
                }
                cropper = new Cropper(document.getElementById('imageToCrop'), {
                    aspectRatio: 5 / 3,
                    viewMode: 1,
                    autoCropArea: 1,
                    scalable: true,
                    zoomable: true,
                    ready() {
                        this.cropper.setCropBoxData({
                            width: this.cropper.getContainerData().width,
                            height: this.cropper.getContainerData().height,
                        });
                    },
                });
            });
            currentImageIndex = imageIndex;
        };
        reader.readAsDataURL(input.files[0]);
        input.value = ''; // Reset input value to allow re-upload of the same file
    }
}

$('#cropImageModal').on('hidden.bs.modal', function () {
    if (cropper) {
        cropper.destroy();
    }
});

$('#cropImageButton').on('click', function () {
    const canvas = cropper.getCroppedCanvas({
        width: 1000,
        height: 600,
    });

    canvas.toBlob(function (blob) {
        const url = URL.createObjectURL(blob);
        if (currentImageIndex === imageIndex) {
            addImageToCarousel(url);
            addThumbnail(url);
        } else {
            updateImageInCarousel(url, currentImageIndex);
            updateThumbnail(url, currentImageIndex);
        }
        uploadedImages[currentImageIndex] = new File([blob], 'croppedImage.jpg', { type: 'image/jpeg' });
        $('#cropImageModal').modal('hide');
    }, 'image/jpeg');
});

function handleThumbnailChange(input, index) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            $('#cropImageModal').modal('show');
            $('#imageToCrop').attr('src', e.target.result);
            $('#cropImageModal').on('shown.bs.modal', function () {
                if (cropper) {
                    cropper.destroy();
                }
                cropper = new Cropper(document.getElementById('imageToCrop'), {
                    aspectRatio: 5 / 3,
                    viewMode: 1,
                    autoCropArea: 1,
                    scalable: true,
                    zoomable: true,
                    ready() {
                        this.cropper.setCropBoxData({
                            width: this.cropper.getContainerData().width,
                            height: this.cropper.getContainerData().height,
                        });
                    },
                });
            });
            currentImageIndex = index;
        };
        reader.readAsDataURL(input.files[0]);
    }
}

function addImageToCarousel(imageSrc) {
    const carouselInner = document.getElementById('carousel-inner');
    const div = document.createElement('div');
    div.className = 'carousel-item' + (imageIndex === 0 ? ' active' : '');
    div.setAttribute('data-index', imageIndex);
    div.innerHTML = `<img src="${imageSrc}" class="d-block w-100" alt="Image ${imageIndex}">`;
    carouselInner.appendChild(div);
    imageIndex++;
}

function addThumbnail(imageSrc) {
    const thumbnailContainer = document.querySelector('.thumbnail-container');
    const thumbnailItem = document.createElement('div');
    thumbnailItem.className = 'thumbnail-item';
    const img = document.createElement('img');
    img.src = imageSrc;
    img.alt = `Thumbnail ${imageIndex - 1}`;
    img.setAttribute('data-index', imageIndex - 1);
    img.onclick = function () {
        const carousel = new bootstrap.Carousel(document.getElementById('carouselExample'));
        carousel.to(parseInt(img.getAttribute('data-index')));
    };
    const input = document.createElement('input');
    input.type = 'file';
    input.setAttribute('data-index', imageIndex - 1);
    input.onchange = function () {
        handleThumbnailChange(this, parseInt(input.getAttribute('data-index')));
    };
    const button = document.createElement('button');
    button.className = 'btn btn-sm btn-primary';
    button.innerText = 'Change';
    button.onclick = function () {
        input.click();
    };

    thumbnailItem.appendChild(img);
    thumbnailItem.appendChild(button);
    thumbnailItem.appendChild(input);
    thumbnailContainer.appendChild(thumbnailItem);
}

function updateImageInCarousel(imageSrc, index) {
    const carouselInner = document.getElementById('carousel-inner');
    const items = carouselInner.getElementsByClassName('carousel-item');
    for (let item of items) {
        if (parseInt(item.getAttribute('data-index')) === index) {
            item.querySelector('img').src = imageSrc;
        }
    }
}

function updateThumbnail(imageSrc, index) {
    const thumbnailContainer = document.querySelector('.thumbnail-container');
    const imgs = thumbnailContainer.getElementsByTagName('img');
    for (let img of imgs) {
        if (parseInt(img.getAttribute('data-index')) === index) {
            img.src = imageSrc;
        }
    }
}

function deleteLastImage() {
    if (imageIndex > 0) {
        const carouselInner = document.getElementById('carousel-inner');
        const thumbnailContainer = document.querySelector('.thumbnail-container');

        carouselInner.removeChild(carouselInner.lastElementChild);
        thumbnailContainer.removeChild(thumbnailContainer.lastElementChild);

        imageIndex--;

        if (carouselInner.querySelector('.carousel-item.active') === null && imageIndex > 0) {
            carouselInner.lastElementChild.classList.add('active');
        }
    }
}

document.getElementById('rotateLeft').addEventListener('click', function () {
    cropper.rotate(-45);
});

document.getElementById('rotateRight').addEventListener('click', function () {
    cropper.rotate(45);
});

document.getElementById('scaleX').addEventListener('click', function () {
    const currentScaleX = cropper.getData().scaleX || 1;
    cropper.scaleX(-currentScaleX);
});

document.getElementById('scaleY').addEventListener('click', function () {
    const currentScaleY = cropper.getData().scaleY || 1;
    cropper.scaleY(-currentScaleY);
});



function collectImageDTOs() {
    const imageDTOs = [];
    for (let index in uploadedImages) {
        if (uploadedImages.hasOwnProperty(index)) {
            const formData = new FormData();
            formData.append('index', index);
            formData.append('imageFile', uploadedImages[index]);
            imageDTOs.push({ index: index, imageFile: uploadedImages[index] });
        }
    }
    return imageDTOs;
}
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

            // Thu thập dữ liệu từ form
            formData.append("RoomNumber", document.getElementById("RoomNumber").value);
/*            formData.append("NumberOfBeds", document.getElementById("NumberOfBeds").value);
            formData.append("NumberOfAdults", document.getElementById("NumberOfAdults").value);
            formData.append("NumberOfChildren", document.getElementById("NumberOfChildren").value);
            formData.append("Price", document.getElementById("Price").value);*/
            formData.append("RoomTypeId", document.getElementById("RoomTypeId").value);
            formData.append("Description", document.getElementById("Description").value);

            // Thu thập danh sách dịch vụ từ Select2
/*            const selectedServices = $('#Services').val(); 
            if (selectedServices) {
                selectedServices.forEach(serviceId => {
                    formData.append("ServiceIds", serviceId); 
                });
                console.log("Selected Services: ", selectedServices);
            }*/

            // Thu thập các ảnh từ collectImageDTOs
/*            const imageDTOs = collectImageDTOs();
            imageDTOs.forEach((imageDTO, index) => {
                formData.append(`Images[${index}][index]`, imageDTO.index);
                formData.append(`Images[${index}][imageFile]`, imageDTO.imageFile);
            });
*/
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
                maxlength: 8,
                digits: true
            },
            NumberOfBeds: {
                required: true,
                min: 1,
            },
            NumberOfAdults: {
                required: true,
                min: 1,
            },
            NumberOfChildren: {
                required: true,

            },
            Price: {
                required: true,
                min: 1
            },
            RoomTypeId: {
                required: true
            },
            ServiceIds: {
                required: true
            },
            Description: {
                maxlength: 500
            }
        },
        messages: {
            RoomNumber: {
                required: "Please enter the room number.",
                maxlength: "Room number cannot exceed 8 digits.",
                digits: "Room number must be a valid number."
            },
            NumberOfBeds: {
                required: "Please enter the number of beds.",
                min: "Must have at least 1 bed.",
            },
            NumberOfAdults: {
                required: "Please enter the number of adults.",
                min: "Must allow at least 1 adult.",
            },
            NumberOfChildren: {
                required: "Please enter the number of children.",
            },
            Price: {
                required: "Please enter the room price.",
                min: "Price must be at least 1."
            },
            RoomTypeId: {
                required: "Please select a room type."
            },
            ServiceIds: {
                required: "Please select at least one service."
            },
            Description: {
                maxlength: "Description cannot exceed 500 characters."
            }
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
