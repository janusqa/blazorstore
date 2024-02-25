window.ShowToastr = function (type, message) {
    console.log('file error');
    if (type === 'success') {
        toastr.success(message, 'Operation Successful', { timeOut: 5000 });
    } else if (type === 'error') {
        toastr.error(message, 'Operation unsuccessful', { timeOut: 5000 });
    }
};

window.ShowSweetAlert = function (type, message) {
    Swal.fire({
        title: type === 'success' ? 'Success' : 'Error',
        text: message,
        icon: type,
        confirmButtonText: 'Ok',
    });
};

function ConfirmationModal(title, text, buttonText, icon = 'warning') {
    return Swal.fire({
        title: title,
        text: text,
        icon: icon,
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: buttonText,
    }).then((result) => {
        return Promise.resolve(result.isConfirmed);
    });
}

function Spinner(visible = false) {
    var modal = $('#staticBackdrop');

    if (visible) {
        modal.modal('show');
    } else {
        modal.one('shown.bs.modal', function () {
            // This callback will be executed after the modal is fully shown
            // console.log('Modal fully shown');
            // Additional code if needed
            modal.modal('hide'); // Now hide the modal
        });

        // Trigger the modal show
        modal.modal('show');
    }
}

TinyMceInit = function (selector) {
    tinymce.remove(`textarea#${selector}`);
    tinymce.init({
        selector: `textarea#${selector}`,
        plugins:
            'anchor autolink charmap codesample emoticons lists table visualblocks wordcount',
        toolbar:
            'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | align lineheight | numlist bullist indent outdent',
    });
};
