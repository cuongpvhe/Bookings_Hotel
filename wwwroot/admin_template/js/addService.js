﻿let imageIndex = 0;
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

a
async function submitFormAjax() {
    const form = $('#addServiceForm');

    if (!validateServiceForm()) {
        return; 
    }


    const formData = new FormData(this);
    formData.append("ServiceName", document.getElementById("ServiceName").value);
    formData.append("Price", document.getElementById("Price").value);
    formData.append("Description", document.getElementById("Description").value);
    const imageDTOs = collectImageDTOs();
    imageDTOs.forEach((imageDTO, index) => {
        formData.append(`imageDTOS[${index}][index]`, imageDTO.index);
        formData.append(`imageDTOS[${index}][imageFile]`, imageDTO.imageFile);
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

    // AJAX request
    $.ajax({
        url: '/Manager/Services/Create?handler=Post', // URL tới phương thức xử lý
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
                Swal.fire("Success", "Thêm dịch vụ thành công!", "success")
                    .then(() => window.location.href = '/Manager/Services/List'); // Redirect to rooms list
            }
        },
        error: function (xhr, status, error) {
            Swal.fire("Error", "Đã có lỗi trong quá trình lưu thông tin dịch vụ.", "error");
            console.log(xhr.responseText);
        }

    });

});

function validateServiceForm() {
    // Thiết lập validate cho form
    $('#addServiceForm').validate({
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

    return $('#addServiceForm').valid();
}
