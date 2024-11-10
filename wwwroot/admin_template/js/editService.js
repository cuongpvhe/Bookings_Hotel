let imageIndex = 0;
const uploadedImages = {};

let cropper;
let currentImageIndex;
let hasExistingImages = false;
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
    hasExistingImages = true;
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
        const uniqueFileName = `croppedImage_${Date.now()}_${Math.random().toString(36).substring(2, 15)}.jpg`;
        const newFile = new File([blob], uniqueFileName, { type: 'image/jpeg' });
        if (currentImageIndex === imageIndex) {
            addImageToCarousel(url, imageIndex, 0); // New image, set imageId to 0
            addThumbnail(url, imageIndex, 0); // New image, set imageId to 0
            uploadedImages[imageIndex] = newFile;
            imageIndex++; // Increase imageIndex after adding a new image
        } else {
            updateImageInCarousel(url, currentImageIndex);
            updateThumbnail(url, currentImageIndex);
            uploadedImages[currentImageIndex] = newFile;
        }
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
        input.value = ''; // Reset input value to allow re-upload of the same file
    }
}

function addImageToCarousel(imageSrc, index, imageId) {
    const carouselInner = document.getElementById('carousel-inner');
    const div = document.createElement('div');
    div.className = 'carousel-item' + (index === 0 ? ' active' : '');
    div.setAttribute('data-index', index);
    div.setAttribute('data-image-id', imageId); // Set data-image-id attribute
    div.innerHTML = `<img src="${imageSrc}" class="d-block w-100" alt="Image ${index}">`;
    carouselInner.appendChild(div);
}

function addThumbnail(imageSrc, index, imageId) {
    const thumbnailContainer = document.querySelector('.thumbnail-container');
    const thumbnailItem = document.createElement('div');
    thumbnailItem.className = 'thumbnail-item';
    thumbnailItem.setAttribute('data-index', index);
    thumbnailItem.setAttribute('data-image-id', imageId); // Set data-image-id attribute
    const img = document.createElement('img');
    img.src = imageSrc;
    img.alt = `Thumbnail ${index}`;
    img.setAttribute('data-index', index);
    img.setAttribute('data-image-id', imageId); // Set data-image-id attribute
    img.onclick = function () {
        const carousel = new bootstrap.Carousel(document.getElementById('carouselExample'));
        carousel.to(parseInt(img.getAttribute('data-index')));
    };
    const input = document.createElement('input');
    input.type = 'file';
    input.setAttribute('data-index', index);
    input.setAttribute('data-image-id', imageId); // Set data-image-id attribute
    input.onchange = function () {
        handleThumbnailChange(this, parseInt(input.getAttribute('data-index')));
    };
    const button = document.createElement('button');
    button.className = 'btn btn-sm btn-primary';
    button.type = 'button';
    button.innerText = 'Thay đổi';
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

function collectImageDTOs() {
    const imageDTOs = [];
    const carouselItems = document.getElementById('carousel-inner').getElementsByClassName('carousel-item');
    for (let item of carouselItems) {
        const index = item.getAttribute('data-index');
        const imageId = item.getAttribute('data-image-id') || 0;
        const imageFile = uploadedImages[index] || null;
        imageDTOs.push({ index: parseInt(index), imageId: parseInt(imageId), imageFile: imageFile });
    }
    return imageDTOs;
}

function loadImagesForUpdate(imageDTOs) {
    imageDTOs.forEach((imageDTO, index) => {
        addImageToCarousel(imageDTO.imageUrl, imageDTO.index, imageDTO.imageId);
        addThumbnail(imageDTO.imageUrl, imageDTO.index, imageDTO.imageId);
        uploadedImages[imageDTO.index] = null;
        if (imageDTO.index >= imageIndex) {
            imageIndex = imageDTO.index + 1;
        }
    });

    hasExistingImages = imageDTOs.length > 0;
}
async function submitEditFormAjax() {
    const form = $('#editServiceForm');
    console.log(hasExistingImages);
    if (Object.keys(uploadedImages).length === 0 ) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu ảnh',
            text: 'Vui lòng thêm ít nhất một ảnh trước khi lưu.',
        });
        return; // Stop the submission if no image is present
    } else if (!hasExistingImages) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu ảnh',
            text: 'Vui lòng thêm ít nhất một ảnh trước khi lưu.',
        });
        return;
    }

    // Kiểm tra tính hợp lệ của form
    if (!validateServiceForm()) {
        return; // Dừng lại nếu form không hợp lệ
    }
    const originalServiceName = document.getElementById("OriginalServiceName").value;
    const serviceName = document.getElementById("ServiceName").value;
    try {
        if (serviceName !== originalServiceName) {
            const exists = await checkServiceNameExists(serviceName);
            if (exists) {
                showServiceNameError();
                return; // Dừng lại nếu số phòng đã tồn tại
            }
        }
        const formData = new FormData();

        // Thu thập dữ liệu từ form
        formData.append("ServiceId", document.getElementById("ServiceId").value);
        formData.append("ServiceName", document.getElementById("ServiceName").value);
        formData.append("Price", document.getElementById("Price").value);
        formData.append("Status", document.getElementById("Status").value);

        formData.append("Description", document.getElementById("Description").value);

        const imageDTOs = collectImageDTOs();

        imageDTOs.forEach((imageDTO, index) => {
            formData.append(`imageDTOS[${index}].index`, imageDTO.index);
            formData.append(`imageDTOS[${index}].imageId`, imageDTO.imageId);
            if (imageDTO.imageFile) {
                formData.append(`imageDTOS[${index}].imageFile`, imageDTO.imageFile);
            }
        });

        // AJAX request
        $.ajax({
            url: '?handler=Post', // URL tới phương thức xử lý
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            beforeSend: function () {
                Swal.fire({
                    title: 'Đang xử lý',
                    allowOutsideClick: false,
                    showConfirmButton: false,
                    willOpen: () => {
                        Swal.showLoading();
                    }
                });
            },
            success: function (response) {
                if (response.success) {
                    Swal.fire("Success", "Cập nhật thông tin dịch vụ  thành công!", "success")
                        .then(() => window.location.href = '/Manager/Services/List'); // Redirect to rooms list
                }
            },
            error: function (xhr, status, error) {
                Swal.fire("Error", "Đã có lỗi trong quá trình cập nhật thông tin dịch vụ.", "error");
                console.log(xhr.responseText);
            }

        });
    } catch (error) {
        console.log("Error ", error);
    }
}

function validateServiceForm() {
    // Thiết lập validate cho form
    $('#editServiceForm').validate({
        rules: {
            'ServiceName': {
                required: true,
                minlength: 2,
                maxlength: 50
            },
            'Price': {
                required: true,
                min: 0.01,
                number: true
            },
            'Description': {
                maxlength: 500
            },
            /*            'service.Status': {
                            required: true
                        }*/
        },
        messages: {
            'ServiceName': {
                required: "Vui lòng nhập tên dịch vụ.",
                minlength: "Tên dịch vụ phải dài ít nhất 2 ký tự.",
                maxlength: "Tên dịch vụ không quá 50 ký tự."
            },
            'Price': {
                required: "Vui lòng nhập giá dịch vụ.",
                min: "Giá phải lơn hơn 0.",
                number: "Vui lòng nhập giá hợp lệ."
            },
            'Description': {
                maxlength: "Mô tả không quá 500 ký tự."
            },
            /*            'service.Status': {
                            required: "Vui lòng chọn trạng thái."
                        }*/
        },
        errorElement: 'span',
        errorPlacement: function (error, element) {
            error.addClass("text-danger");
            if (element.prop("tagName") === "SELECT") {
                error.insertAfter(element.next('.select2-container'));
            } else {
                error.insertAfter(element);
            }
        },
    });

    return $('#editServiceForm').valid();
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
function deleteLastImage() {
    if (imageIndex > 0) {
        Swal.fire({
            title: 'Bạn có chắc không?',
            text: "Bạn có thực sự muốn xóa hình ảnh cuối cùng?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Có',
            cancelButtonText: 'Hủy'
        }).then((result) => {
            if (result.isConfirmed) {
                
                    const carouselInner = document.getElementById('carousel-inner');
                    const thumbnailContainer = document.querySelector('.thumbnail-container');

                    carouselInner.removeChild(carouselInner.lastElementChild);
                    thumbnailContainer.removeChild(thumbnailContainer.lastElementChild);

                    imageIndex--;
                    if (carouselInner.children.length === 0) {
                        hasExistingImages = false;
                    }

                    if (carouselInner.querySelector('.carousel-item.active') === null && imageIndex > 0) {
                        carouselInner.lastElementChild.classList.add('active');
                    }
                }
        });
    }
}

function checkServiceNameExists(name) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: '/Manager/Services/Create?handler=CheckServiceName', // URL tới phương thức kiểm tra
            type: 'POST',
            data: { serviceName: name },
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                resolve(response.exists); // Trả về kết quả tồn tại của số phòng qua Promise
            },
            error: function () {
                Swal.fire("Error", "Error", "error");
                reject();
            }
        });
    });
}

function showServiceNameError() {
    const serviceNameField = $("#ServiceName");
    serviceNameField.addClass("is-invalid"); // Use serviceNameField instead of serviceNamerField

    // Check if an error message already exists, and add it if it doesn't
    if (!$("#ServiceName-error").length) {
        $("<span id='ServiceName-error' class='text-danger'>Tên dịch vụ này đã tồn tại</span>")
            .insertAfter(serviceNameField);
    }
}

