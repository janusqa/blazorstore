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
