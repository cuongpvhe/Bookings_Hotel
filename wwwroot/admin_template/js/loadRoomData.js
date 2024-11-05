$(document).ready(function () {
    loadRoomData(pageNumber, pageSize, keyword, roomStatus);
    loadCategories();
    changePageSize();
    searchByCategoryName();
    searchByFavorite();
})

let pageSize = 10;
let pageNumber = 0;
let keyword = "";
let roomStatusFilter = "";

function loadRoomData(pageNumber, pageSize, keyword, roomStatusFilter) {
    $.ajax({
        url: '/get-all-product',
        type: 'GET',
        data: {
            pageNumber: pageNumber,
            pageSize: pageSize,
            roomStatusFilter: roomStatusFilter,
            keyword: keyword,
        },
        beforeSend: function () {
            lsdRing.removeClass('d-none');
        },
        success: function (response) {
            
                });

                renderPagination(data.pageNumber, data.totalPage);
            }
        },
        error: function (xhr, status, error) {
            console.log('Error:', error);
            Swal.fire({
                title: "Load Data Fail",
                icon: "error",
                text: "Please try later.",
                confirmButtonText: "Close",
            });
        },
        complete: function (xhr, status) {
        }
    });
}

function searchByKeyword() {
    $('#keyword').change(function () {
        keyword = $(this).val();
        loadRoomData(pageNumber, pageSize, keyword, roomStatusFilter);
    });
}

function searchByKeyword() {
    $('#roomStatusFilter').change(function () {
        roomStatusFilter = $(this).val();
        loadRoomData(pageNumber, pageSize, keyword, roomStatusFilter);
    });
}

function renderPagination(pageNumber, totalPage) {
    let paginationRoot = $('.pagination');
    paginationRoot.empty();
    let html = '<li class="page-item" data-page-number="' + (pageNumber === 0 ? pageNumber : pageNumber - 1) + '"><span class="page-link">Previous</span></li>';

    for (var i = 0; i < totalPage; i++) {
        if (i === pageNumber) {
            html += '<li class="page-item" data-page-number="' + i + '"><span class="page-link active">' + (i + 1) + '</span></li>';
        } else {
            html += '<li class="page-item" data-page-number="' + i + '"><span class="page-link">' + (i + 1) + '</span></li>';
        }
    }

    html += '<li class="page-item" data-page-number="' + (pageNumber === totalPage - 1 ? pageNumber : pageNumber + 1) + '"><span class="page-link">Next</span></li>';

    paginationRoot.html(html);
    changePagination();
}

function changePagination() {
    let paginationRoot = $('.pagination');
    paginationRoot.off('click', '.page-item');
    paginationRoot.on('click', '.page-item', function () {
        pageNumber = $(this).data('page-number');
        loadRoomData(pageNumber, pageSize, keyword);
    })
}

//Change Page Size
function changePageSize() {
    let pageSizeButton = $('#pageSize');
    pageSizeButton.off('change');
    pageSizeButton.on('change', function () {
        pageSize = $(this).val();
        pageNumber = 0;
        loadRoomData(pageNumber, pageSize, keyword);
    })
}