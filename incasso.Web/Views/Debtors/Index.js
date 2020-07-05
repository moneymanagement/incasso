$(function () {
    var service = abp.services.app.Debtors;

    $("#datalist").on("click", "input[id^='Status_']", function (e) {
        progress(1);
        var aval = $(this).val().split(":");
        var xid = aval[0], xstatus = 3;
        if ($(this).is(":checked"))
            xstatus = 0;
        service.changeStatus({ Id: xid, Status: xstatus }).done(function () {
            progress(0);
            //$.get(hostName + "/Debtors/GetGrid?" + $.param({ requestedPage: xpage })).done(function (data) {
            //    $("#datalist").html('').append($(data));
            //    progress(0);
            //});
        });
        delete aval, xid, xstatus;
    });



    $("#form_debtors").on("change", "#Admin_Id", function (e) {
        var that = $(this);
        progress(1);
        $.get("/Debtors/GetGrid?" + $.param({ requestedPage: 0, adminId: $(this).val() })).done(function (data) {
            $("#datalist").html('').append($(data));
            progress(0);
        });
    }).on("click", "#btn_submit", function (e) {
        var go = 1;
        $("input,select,textarea").removeClass("invalid");
        $("input.validate,select.validate,textarea.validate").each(function (i, v) {
            if ($(this).val() == "" || $(this).val() == null) {
                $(this).addClass("invalid");
                go = 0;
            }
        });
        var form = $(this).closest('form');

        var data = {
            Id: $("#edit").val(), 
            Status: $("#hiddenstatus").val(), 
            Number: $('#Number').val(),
            Name: $('#Name ').val(),
            Contact: $('#Contact').val(),
            Email: $('#Email').val(),
            Phone: $('#Phone').val(),
            Mobile: $('#Mobile').val(),
            Address: $('#Address').val(),
            Postal: $('#Postal').val(),
            City: $('#City').val(),
            Country: $('#Country').val(),
            Notes: $('#Notes').val(),
            AdministratorId: $('#AdministratorId').val(),
            Notes_mm: $('#Notes_mm').val(),
        };
        var type = (data.Id ? "update" : "create");
        var method = data.Id ? "PUT" : "POST";

        if (go == 1) {
            progress(1);
            if (type == "create") {

                service.create((data)).done(function (data) {
                    callBack(data);
                });
            }
            else {
                service.update((data)).done(function (data) {
                    callBack(data)
                });

            }

        }
        delete go;
    });


    function callBack(data) {
        if (data) {
            $(".success-container").show();
            $(":input:not(:checkbox,:radio)").val("");
            var xpage = $('ul').find('li.active.main-green').data('page') || 0;
            $.get(hostName + "/Debtors/GetGrid?" + $.param({ requestedPage: xpage })).done(function (data) {
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



