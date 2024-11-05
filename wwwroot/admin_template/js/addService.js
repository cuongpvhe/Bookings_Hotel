// Khởi tạo các biến global
let imageCounter = 0;
const uploadedImages = new Map();
let cropper;

// Xử lý khi chọn file ảnh
function handleImageUpload(event) {
    const files = event.target.files;
    if (files && files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            showCropModal(e.target.result);
        };
        reader.readAsDataURL(files[0]);
    }
}

// Hiển thị modal crop ảnh
function showCropModal(imageSrc) {
    const modal = $('#cropImageModal');
    const image = $('#imageToCrop');

    image.attr('src', imageSrc);
    modal.modal('show');

    // Khởi tạo cropper khi modal hiển thị
    modal.on('shown.bs.modal', function () {
        if (cropper) {
            cropper.destroy();
        }

        cropper = new Cropper(image[0], {
            aspectRatio: 16 / 9,
            viewMode: 2,
            autoCropArea: 1,
            responsive: true,
            restore: false,
            guides: true,
            center: true,
            highlight: false,
            cropBoxMovable: true,
            cropBoxResizable: true,
            toggleDragModeOnDblclick: false
        });
    });
}

// Xử lý khi nhấn nút lưu ảnh đã crop
function handleCropSave() {
    if (!cropper) return;

    const canvas = cropper.getCroppedCanvas({
        width: 800,
        height: 450
    });

    canvas.toBlob(function (blob) {
        // Tạo URL cho ảnh đã crop
        const croppedImageUrl = URL.createObjectURL(blob);

        // Thêm ảnh vào carousel
        addImageToCarousel(croppedImageUrl);

        // Lưu blob để upload
        const imageFile = new File([blob], `image-${imageCounter}.jpg`, { type: 'image/jpeg' });
        uploadedImages.set(imageCounter - 1, imageFile);

        // Đóng modal
        $('#cropImageModal').modal('hide');
    }, 'image/jpeg');
}

// Thêm ảnh vào carousel
function addImageToCarousel(imageSrc) {
    const carouselInner = document.getElementById('carousel-inner');
    const newDiv = document.createElement('div');
    newDiv.className = `carousel-item ${imageCounter === 0 ? 'active' : ''}`;
    newDiv.innerHTML = `
        <img src="${imageSrc}" class="d-block w-100" alt="Service Image ${imageCounter + 1}">
    `;
    carouselInner.appendChild(newDiv);
    imageCounter++;
}

// Xóa ảnh cuối cùng
function deleteLastImage() {
    if (imageCounter > 0) {
        const carouselInner = document.getElementById('carousel-inner');
        carouselInner.removeChild(carouselInner.lastChild);
        uploadedImages.delete(imageCounter - 1);
        imageCounter--;

        // Nếu xóa hết ảnh, reset counter
        if (imageCounter === 0) {
            const input = document.getElementById('imageInput');
            if (input) input.value = '';
        }
    }
}

// Xử lý các nút điều chỉnh ảnh
document.getElementById('rotateLeft').addEventListener('click', () => cropper?.rotate(-90));
document.getElementById('rotateRight').addEventListener('click', () => cropper?.rotate(90));
document.getElementById('scaleX').addEventListener('click', () => {
    const scaleX = cropper.getData().scaleX;
    cropper.scaleX(scaleX === 1 ? -1 : 1);
});
document.getElementById('scaleY').addEventListener('click', () => {
    const scaleY = cropper.getData().scaleY;
    cropper.scaleY(scaleY === 1 ? -1 : 1);
});

// Xử lý form submit
document.getElementById('addServiceForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    if (!validateServiceForm()) {
        return; 
    }

    if (uploadedImages.size === 0) {
        Swal.fire({
            title: 'Warning',
            text: 'Vui lòng thêm ít nhất một ảnh!',
            icon: 'warning'
        });
        return;
    }

    const formData = new FormData(this);
    uploadedImages.forEach((file, index) => {
        formData.append(`ImageFiles`, file);
    });

    Swal.fire({
        title: 'Processing',
        text: 'Chờ xử lý...',
        allowOutsideClick: false,
        showConfirmButton: false,
        willOpen: () => {
            Swal.showLoading();
        }
    });

    try {
        const response = await fetch(this.action, {
            method: 'POST',
            body: formData
        });
        if (response.ok) {
            Swal.fire({
                title: 'Success!',
                text: 'Thêm mới dịch vụ thành công',
                icon: 'success',
                showConfirmButton: true
            }).then(() => {
                window.location.href = './List';
            });
        } else {
            Swal.fire({
                title: 'Error!',
                text: 'Thêm mới dịch vụ thất bại!',
                icon: 'error'
            });
        }
    } catch (error) {
        Swal.fire({
            title: 'Error!',
            text: error.message,
            icon: 'error'
        });
    }
});

function validateServiceForm() {
    // Thiết lập validate cho form
    $('#addServiceForm').validate({
        rules: {
            'service.ServiceName': {
                required: true,
                minlength: 2,
                maxlength: 50
            },
            'service.Price': {
                required: true,
                min: 0.01,
                number: true
            },
            'service.Description': {
                maxlength: 500
            },
/*            'service.Status': {
                required: true
            }*/
        },
        messages: {
            'service.ServiceName': {
                required: "Vui lòng nhập tên dịch vụ.",
                minlength: "Tên dịch vụ phải dài ít nhất 2 ký tự.",
                maxlength: "Tên dịch vụ không quá 50 ký tự."
            },
            'service.Price': {
                required: "Vui lòng nhập giá dịch vụ.",
                min: "Giá phải lơn hơn 0.",
                number: "Vui lòng nhập giá hợp lệ."
            },
            'service.Description': {
                maxlength: "Mô tả không quá 500 ký tự."
            },
/*            'service.Status': {
                required: "Vui lòng chọn trạng thái."
            }*/
        },
        errorElement: 'span',
        errorPlacement: function (error, element) {
            error.addClass('text-danger');
            if (element.parent('.input-group').length) {
                error.insertAfter(element.parent()); // For price input with $ symbol
            } else {
                error.insertAfter(element);
            }
        },
        highlight: function (element, errorClass, validClass) {
            $(element).addClass('is-invalid').removeClass('is-valid');
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).addClass('is-valid').removeClass('is-invalid');
        },
        submitHandler: function (form) {
            // Kiểm tra xem có ít nhất một ảnh được tải lên
            if (uploadedImages.size === 0) {
                Swal.fire({
                    title: 'Error',
                    text: 'Vui lòng thêm ít nhất 1 ảnh',
                    icon: 'error'
                });
                return false;
            }


            // If all validations pass, show loading state
            Swal.fire({
                title: 'Processing',
                text: 'Chờ xử lý...',
                allowOutsideClick: false,
                showConfirmButton: false,
                willOpen: () => {
                    Swal.showLoading();
                }
            });

            form.submit();
        }
    });

    // Thêm custom method để validate giá tiền
    $.validator.addMethod("money", function (value, element) {
        return this.optional(element) || /^\d{0,10}(\.\d{0,2})?$/.test(value);
    }, "Vui lòng nhập giá hợp lệ.");


    // Real-time price formatting
    $('input[name="service.Price"]').on('input', function () {
        let value = $(this).val();
        // Remove non-numeric characters except decimal point
        value = value.replace(/[^\d.]/g, '');
        // Ensure only one decimal point
        value = value.replace(/(\..*)\./g, '$1');
        // Limit to 2 decimal places
        const parts = value.split('.');
        if (parts.length > 1) {
            parts[1] = parts[1].slice(0, 2);
            value = parts.join('.');
        }
        $(this).val(value);
    });

    // Add keypress validation for price input
    $('input[name="service.Price"]').keypress(function (e) {
        if (e.which != 8 && e.which != 0 && e.which != 46 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });

    // Initialize tooltips for validation messages
    $('[data-toggle="tooltip"]').tooltip();

    return $('#addServiceForm').valid();
}


$(document).ready(function () {
    $('#imageInput').on('change', handleImageUpload);

    $('#cropImageButton').on('click', handleCropSave);

    $('#cropImageModal').on('hidden.bs.modal', function () {
        if (cropper) {
            cropper.destroy();
            cropper = null;
        }
    });
});