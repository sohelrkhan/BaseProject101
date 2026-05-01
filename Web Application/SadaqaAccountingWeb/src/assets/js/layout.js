$(document).on("keypress keyup blur", ".decimal", function () {
    $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});
$(document).on("keypress keyup blur", ".number", function () {
    $(this).val($(this).val().replace(/[^\d].+/, ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});
$(document).on("blur", ".decimal", function () {
    $(this).val(parseFloat($(this).val()).toFixed(2));
    if (isNaN($(this).val())) {
        $(this).val('0');
    }
});
$(document).on("focus", ".decimal,.number", function () {
    var val = $(this).val();
    if (val == 0) {
        $(this).val('');
    }
});
$(document).on("blur", ".decimal,.number", function () {
    var val = $(this).val();
    if (val == '') {
        $(this).val('0');
    }
});
$(document).on('click', '.summerNoteClose', function () {
    $('div.modal.note-modal').modal('hide');
});
$(".select2").select2({
   /* dropdownParent: $(".modal-body"),*/ width: '100%', placeholder: "Select..",
    allowClear: true,

});
$(".select2Body").select2({
    width: '100%', placeholder: "Select..",
    allowClear: true,
});
$('.select2Modal').each(function (i, obj) {
    $(this).select2({
        dropdownParent: $(this).closest("div.modal"), width: '100%', placeholder: "Select..",
        allowClear: true,
    });
});