$(function () {
    var service = abp.services.app.Administrations;
    $("#form_admins").on("click", "#btn_submit", function (e) {
        var go = 1;
        $("input,select,textarea").removeClass("invalid");
        $("input.validate,select.validate,textarea.validate").each(function (i, v) {
            if ($(this).val() == "" || $(this).val() == null) {
                $(this).addClass("invalid");
                go = 0;
            }
        });

        var data = ({
            Id: $("#edit").val(),
            Name: $("#Name").val(),
            Number: $("#Number").val(),
            Contact: $("#Contact").val(),
            Phone: $("#Phone").val(),
            Phone2: $("#Phone2").val(),
            Email: $("#Email").val(),
            Bank: $("#Bank").val(), Account: $("#Account").val(),
            Iban: $("#Iban").val(), Bic: $("#Bic").val(),
            UsersList: $("input[id^='User']:checked").map(function () { return $(this).val(); }).get()
        });
        var type = (data.Id ? "update" : "create");
        var method = data.Id ? "PUT" : "POST";

        if (go == 1) {
            progress(1);
            if (type == "create") {
                service.create(data).done(function (data) {
                    callBack(data);
                });
            }
            else {
                service.update(data).done(function (data) {
                    callBack(data)
                });
            }
        }
        delete go;
    });
    function callBack(data) {
        if (data.Success) {
            $(".success-container").show();
            $(":input:not(:checkbox,:radio)").val("");
            var xpage = $('ul').find('li.active.main-green').data('page') || 0;
            $.get("/administrations/GetGrid?" + $.param({ requestedPage: xpage })).done(function (data) {
                $("#datalist").html('').append($(data));
                progress(0);
            });
        } else {
            $(".error-container").show();
        }
        M.updateTextFields();
        progress(0);
    }
})