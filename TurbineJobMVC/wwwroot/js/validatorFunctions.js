$.validator.addMethod("isduplicatenotratear", function (value, element, parameters) {
    var isSuccess;
    var wono;
    $.ajax({
        type: "GET",
        async: false,
        url: "api/WorkOrder/IsDublicateNotRateAR/" + value,
        data: {},
        dataType: "json",
        cache: false,
        success: function (response) {
            isSuccess = response.woNo;
            wono = response.woNo;

        }
    });
    $.validator.messages.isduplicatenotratear = `برای اموال یک درخواست با کد رهگیری ${wono} ثبت شده که تاییدیه اتمام کار آن توسط شما اعلان نشده است`;
    return isSuccess === undefined;
}, '');
$.validator.unobtrusive.adapters.add("isduplicatenotratear", [], function (options) {
    options.rules.isduplicatenotratear = {};
});

$.validator.addMethod("isdublicateactivear", function (value, element, parameters) {
    var isSuccess;
    var wono;
    $.ajax({
        type: "GET",
        async: false,
        url: "api/WorkOrder/IsDublicateActiveAR/" + value,
        data: {},
        dataType: "json",
        cache: false,
        success: function (response) {
            isSuccess = response.woNo;
            wono = response.woNo;

        }
    });
    $.validator.messages.isdublicateactivear = `برای اموال یک درخواست با کد رهگیری ${wono} ثبت شده که همچنان در دست اقدام است`;
    return isSuccess === undefined;
}, '');
$.validator.unobtrusive.adapters.add("isdublicateactivear", [], function (options) {
    options.rules.isdublicateactivear = {};
});
