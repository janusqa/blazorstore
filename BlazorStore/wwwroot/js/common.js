window.ShowToastr = function (type, message) {
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

TinyMceInit = function (selector) {
    tinymce.init({
        selector: `textarea#${selector}`,
        //  plugins: 'ai tinycomments mentions anchor autolink charmap codesample emoticons image link lists media searchreplace table visualblocks wordcount checklist mediaembed casechange export formatpainter pageembed permanentpen footnotes advtemplate advtable advcode editimage tableofcontents mergetags powerpaste tinymcespellchecker autocorrect a11ychecker typography inlinecss',
        //  toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link image media table mergetags | align lineheight | tinycomments | checklist numlist bullist indent outdent | emoticons charmap | removeformat',
        plugins:
            'anchor autolink charmap codesample emoticons lists table visualblocks wordcount',
        toolbar:
            'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | align lineheight | numlist bullist indent outdent',
        tinycomments_mode: 'embedded',
        tinycomments_author: 'Author name',
        mergetags_list: [
            { value: 'First.Name', title: 'First Name' },
            { value: 'Email', title: 'Email' },
        ],
        ai_request: (request, respondWith) =>
            respondWith.string(() =>
                Promise.reject('See docs to implement AI Assistant')
            ),
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
