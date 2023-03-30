$(document).ready(function () {
    $(document).on('click', '.updateBtn', function (e) {
        e.preventDefault();

        let parent = $(this).parent();
        let prev = parent.prev();
        parent.addClass('d-none');
        prev.addClass('d-none');

        let next = parent.next();
        next.removeClass('d-none');
    })


    $(document).on('click', '.deleteBtn', function (e) {
        e.preventDefault();

        let url = $(this).attr('href');

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url)
                    .then(res => res.text())
                    .then(data => {
                        $('.indexContainer').html(data)
                    })
                Swal.fire(
                    'Deleted!',
                    'Your file has been deleted.',
                    'success'
                )
            }
        })
    })

    $(document).on('click', '.deleteImage', function (e) {
        e.preventDefault();

        let url = $('.deleteImage').attr('href');
        let imageId = $(this).attr('data-imageId');

        fetch(url + "?imageId=" + imageId)
            .then(res => {
                if (res.ok) {
                    return res.text()
                } else {
                    alert("Yanlis Emeliyyat");
                    return;
                }
            })
            .then(data => {
                $('.productImages').html(data)
            })

        //console.log(url + "?imageId=" + imageId);
    })

    let isMain = $('#IsMain').is(':checked');

    if (isMain) {
        $('#fileInput').removeClass('d-none');
        $('#parentList').addClass('d-none');
    } else {
        $('#fileInput').addClass('d-none');
        $('#parentList').removeClass('d-none');
    }

    $('#IsMain').click(function () {
        let isMain = $(this).is(':checked');

        if (isMain) {
            $('#fileInput').removeClass('d-none');
            $('#parentList').addClass('d-none');
        } else {
            $('#fileInput').addClass('d-none');
            $('#parentList').removeClass('d-none');
        }
    })
})