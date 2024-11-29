var SimpleForm = function (opts) {
    var base = this;
    var selector = opts.selector;
    var emptyChoiceModel =
    {
        value: ""
    };
    var newChoiceModel = $.extend(true, {}, emptyChoiceModel);
    var emptyFieldModel =
    {
        type: "comment",
        title: "",
        choices: []
    };

    var newFieldModel = $.extend(true, {}, emptyFieldModel);
    var model = opts.model || {
        fields: [newFieldModel]
    };

    Object.defineProperty(base, 'model', {
        get() {
            $.each(model.fields, function (i, item) {
                item.name = "question" + (i + 1);
            });

            return model;
        }
    });

    $.templates({
        tmplSimpleFormBuilder: "#template-simple-form-builder"
    })

    base.setFields = function (fields) {
        $.observable(model.fields).refresh(fields);
    }

    var setUserEvent = function () {
        model.changeMode = function (field, type) {
            if (type === "comment") {
                $.observable(field.choices).remove(0, field.choices.length);
            } else if (field.choices.length <= 0) {
                var newChoiceModel = $.extend(true, {}, emptyChoiceModel);
                $.observable(field.choices).insert(newChoiceModel);
            }
        };
        model.addField = function () {
            var newFieldModel = $.extend(true, {}, emptyFieldModel);
            $.observable(model.fields).insert(newFieldModel);
        };
        model.removeField = function (fields, i) {
            $.observable(fields).remove(i);
            return false;
        };
        model.addChoice = function (choices) {
            var newChoiceModel = $.extend(true, {}, emptyChoiceModel);
            $.observable(choices).insert(newChoiceModel);
            return false;
        };
        model.removeChoice = function (choices, i) {
            $.observable(choices).remove(i);
            return false;
        };
    };

    var init = function () {
        setUserEvent();
        $.templates.tmplSimpleFormBuilder.link(selector, model);
    };

    init();
};