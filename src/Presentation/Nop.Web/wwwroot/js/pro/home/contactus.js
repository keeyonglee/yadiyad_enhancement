var PageContactUs = function (opts) {

    var $form = $('#form-contactus');
    var subjectSelect = $form.find('#SubjectId');
    var subjectOther = $form.find('#SubjectOther');

    subjectSelect.on('change', function () {
        let subject = $("#SubjectId option:selected").text()
        console.log(subject);
        subjectOther.val("");
        if (subject === "Others") {
            subjectOther.prop('required', true);
            subjectOther.removeClass("hidden");
        } else {
            subjectOther.prop('required', false);
            subjectOther.addClass("hidden");
        }
    });

};

var pageContactUs = new PageContactUs();