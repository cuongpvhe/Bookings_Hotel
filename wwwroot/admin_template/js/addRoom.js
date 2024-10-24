$(document).ready(function () {
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


async function submitRoomForm() {
    // Tạo FormData để chứa tất cả dữ liệu form
    const formData = new FormData(document.getElementById('addRoomForm'));

    // Lấy các ảnh từ hàm collectImageDTOs
    const imageDTOs = collectImageDTOs();
    imageDTOs.forEach(imageDTO => {
        formData.append('Images', imageDTO.imageFile); // Thêm ảnh vào formData
        formData.append('ImageIndexes', imageDTO.index); // Thêm index của ảnh
    });

    try {
        // Gửi dữ liệu qua Ajax
        const response = await fetch('@Url.Page("/Manager/AddNewRoom")', {
            method: 'POST',
            body: formData
        });

        const result = await response.json();

        if (response.ok) {
            Swal.fire({
                title: 'Success!',
                text: 'Room has been added successfully.',
                icon: 'success',
                confirmButtonText: 'OK'
            }).then(() => {
                window.location.href = '/Manager/Rooms'; // Redirect sau khi thành công
            });
        } else {
            Swal.fire({
                title: 'Error!',
                text: result.errorMessage || 'An error occurred while saving the room.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            title: 'Error!',
            text: 'An unexpected error occurred.',
            icon: 'error',
            confirmButtonText: 'OK'
        });
    }
}
