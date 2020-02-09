$(document).ready(function () {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-left",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
    $('[data-toggle="tooltip"]').tooltip();
    $('#frmAddWorkOrder').bind('invalid-form.validate', function (form, validator) {
        showErrorMessagesOnValidate(validator.errorList);
    });    
});

showErrorMessagesOnValidate = function (errors) {
    if (!errors)
        return;
    var toastrObj = {
        ToastMessages: []
    };
    errors.forEach(function (item) {
        var label = $(item.element.labels[0]).text();
        var msg = {
            Title: label,
            Message: item.message,
            Type: "Error"
        };
        toastrObj.ToastMessages.push(msg);
    });
    displayMessages(toastrObj);
};

function displayMessages(toastrObj) {
    $.each(toastrObj.ToastMessages, function (idx, msg) {
        toastr[msg.Type.toLowerCase()](msg.Message, msg.Title, {});
    });
}