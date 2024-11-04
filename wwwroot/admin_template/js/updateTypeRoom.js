let imageIndex = 0;
const uploadedImages = {};

let cropper;
let currentImageIndex;
const urlParams = new URLSearchParams(window.location.search);

const typeId = urlParams.get('id');

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

                const lastCarouselItem = carouselInner.lastElementChild;
                const lastThumbnailItem = thumbnailContainer.lastElementChild;

                const imageId = lastCarouselItem.getAttribute('data-image-id');
                const dataIndex = lastCarouselItem.getAttribute('data-index');

                // Send the imageId to the server if it's not zero
                if (imageId && parseInt(imageId) !== 0) {
                    $.ajax({
                        url: '/admin/delete-last-image',
                        type: 'POST',
                        data: { imageId: imageId },
                        beforeSend: function () {
                            lsdRing.removeClass('d-none');
                        },
                        success: function (response) {
                            // Remove the last image from carousel
                            carouselInner.removeChild(lastCarouselItem);
                            // Remove the last thumbnail
                            thumbnailContainer.removeChild(lastThumbnailItem);

                            // Update imageIndex
                            imageIndex--;

                            // If the deleted image was the active one, make the last image active
                            if (carouselInner.querySelector('.carousel-item.active') === null && imageIndex > 0) {
                                carouselInner.lastElementChild.classList.add('active');
                            }

                            Swal.fire(
                                'Deleted!',
                                'The last image has been deleted.',
                                'success'
                            );
                        },
                        error: function (xhr, status, error) {
                            console.error('Error occurred during Ajax request:', error);
                            Swal.fire({
                                title: "Error",
                                icon: "error",
                                text: "Failed to delete the image. Please try again later.",
                                confirmButtonText: "Close",
                            });
                        },
                        complete: function (xhr, status) {
                            lsdRing.addClass('d-none');
                        }
                    });
                } else {
                    // Remove the last image from carousel
                    carouselInner.removeChild(lastCarouselItem);
                    // Remove the last thumbnail
                    thumbnailContainer.removeChild(lastThumbnailItem);

                    // Update imageIndex
                    imageIndex--;

                    // If the deleted image was the active one, make the last image active
                    if (carouselInner.querySelector('.carousel-item.active') === null && imageIndex > 0) {
                        carouselInner.lastElementChild.classList.add('active');
                    }

                    Swal.fire(
                        'Deleted!',
                        'The last image has been deleted.',
                        'success'
                    );
                }
            }
        });
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
}
async function submitUpdateFormAjax() {
    const form = $('#updateTypeRoomForm');

    // Kiểm tra tính hợp lệ của form
    if (!validateTypeRoomForm()) {
        return; // Dừng lại nếu form không hợp lệ
    }


    const formData = new FormData();

    // Thu thập dữ liệu từ form
    formData.append("TypeId", typeId);
    formData.append("TypeName", document.getElementById("TypeName").value);
    formData.append("NumberOfBeds", document.getElementById("NumberOfBeds").value);

    formData.append("NumberOfAdults", document.getElementById("NumberOfAdults").value);
    formData.append("MaximumExtraAdult", document.getElementById("MaximumExtraAdult").value);
    formData.append("ExtraAdultFee", document.getElementById("ExtraAdultFee").value);

    formData.append("NumberOfChildren", document.getElementById("NumberOfChildren").value);
    formData.append("MaximumExtraChild", document.getElementById("MaximumExtraChild").value);
    formData.append("ExtraChildFee", document.getElementById("ExtraChildFee").value);

    formData.append("Price", document.getElementById("Price").value);
    formData.append("Description", document.getElementById("Description").value);

    // Thu thập danh sách dịch vụ từ Select2
    const selectedServices = $('#Services').val();
    if (selectedServices) {
        selectedServices.forEach(serviceId => {
            formData.append("ServiceIds", serviceId);
        });
        console.log("Selected Services: ", selectedServices);
    }

    // Thu thập các ảnh từ collectImageDTOs
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
        url: '/Manager/TypeRoom/Update?handler=Post', // URL tới phương thức xử lý
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
                text: 'Saving type room details...',
                allowOutsideClick: false,
                showConfirmButton: false,
                willOpen: () => {
                    Swal.showLoading();
                }
            });
        },
        success: function (response) {
            if (response.success) {
                Swal.fire("Success", "Type room information saved successfully!", "success")
                    .then(() => window.location.href = '/Manager/TypeRoom/List'); // Redirect to rooms list
            }
        },
        error: function (xhr, status, error) {
            Swal.fire("Error", "There was an error saving the type room information.", "error");
            console.log(xhr.responseText);
        }

    });

}

function validateTypeRoomForm() {
    // Thiết lập validate cho form
    $('#updateTypeRoomForm').validate({
        rules: {
            TypeName: {
                required: true,
            },
            NumberOfBeds: {
                required: true,
                min: 1,
            },
            NumberOfAdults: {
                required: true,
                min: 1,
            },
            MaximumExtraAdult: {
                required: true,
                min: 0,
            },
            ExtraAdultFee: {
                required: true,
                min: 0,
            },
            NumberOfChildren: {
                required: true,
                min: 0,
            },
            MaximumExtraChild: {
                required: true,
                min: 0,
            },
            ExtraChildFee: {
                required: true,
                min: 0,
            },
            Price: {
                required: true,
                min: 0
            },
/*            ServiceIds: {
                required: true
            },*/

        },
        messages: {
            TypeName: {
                required: "Hãy nhập tên loại phòng.",
            },
            NumberOfBeds: {
                required: "Hãy nhập số giường.",
                min: "Số giường ít nhất phải phải là 1.",
            },
            NumberOfAdults: {
                required: "Hãy nhập số người lớn.",
                min: "Số người lớn ít nhất là 1.",
            },
            MaximumExtraAdult: {
                required: "Hãy nhập số người lớn được phép thêm.",
                min: "Số người lớn được phép thêm nhỏ nhất là 0.",
            },
            ExtraAdultFee: {
                required: "Hãy nhập phụ phí thêm người lớn.",
                min: "Phụ phí nhỏ nhất là 0.",
            },
            NumberOfChildren: {
                required: "Hãy nhập số trẻ em.",
                min: "Số trẻ ít nhất là 0.",
            },
            MaximumExtraChild: {
                required: "Hãy nhập số trẻ em được phép thêm.",
                min: "Số trẻ em được phép thêm nhỏ nhất là 0.",
            },
            ExtraChildFee: {
                required: "Hãy nhập phụ phí thêm trẻ em.",
                min: "Phụ phí nhỏ nhất là 0.",
            },
            Price: {
                required: "Hãy nhập giá phòng.",
                min: "Giá phòng nhỏ nhất là 0."
            },
/*            ServiceIds: {
                required: "Hãy chọn ít nhất 1 dịch vụ."
            },*/
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
    return $('#updateTypeRoomForm').valid();
}
