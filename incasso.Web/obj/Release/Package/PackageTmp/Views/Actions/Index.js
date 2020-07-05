$(function () {
    var service = abp.services.app.Invoices;


    $("#invoice_info,#closed_info").on("click", ".invoice-row", function (e) {
        var $el = $(this).next(".invoice-notes");
        if ($el.is(":hidden")) $el.slideDown();
        else $el.slideUp("fast");
    });

    $("#form_actions").on("click", "#btn_submit", function (e) {
        var id = $("#edit").val();
        if (id <= 0) {
            return;
        }
        var go = 1;
        $("input,select,textarea").removeClass("invalid");
        $("input.validate,select.validate,textarea.validate").each(function (i, v) {
            if ($(this).val() == "" || $(this).val() == null) {
                $(this).addClass("invalid");
                go = 0;
            }
        });
        if (go === 1) {
            progress(1);
            var data = ({
                Id: $("#edit").val(),
                DossierNo: $("#DossierNo").val(),
                InvoiceNo: $("#InvoiceNo").val(),
                Action: $("#Action").val(),
                ActionDate: $("#ActionDate").val(),
                Open: $("#Open").val(),
                InvoiceDate: $("#InvoiceDate").val(),
                ExpiredDate: $("#ExpiredDate").val(),
                PaymentDate: $("#PaymentDate").val(),
                Amount: $("#Amount").val(),
                Paid: $("#Paid").val(),
                Paidmm: $("#Paidmm").val(),
                PaidClient: $("#PaidClient").val(),
                Interest: $("#Interest").val(),
                CollectionFee: $("#CollectionFee").val(),
                AdminCosts: $("#AdminCosts").val(),
                Type: $("#Type").val(),
                Status: $("#Status").val(),
                Closed: $("#Closed").val()
            });
            service.update(data).done(function (data) {
                callBack(data);
            });
        }
    });

    function callBack(data) {
        if (data.Success) {
            $(".success-container").show();
            $(":input:not(:checkbox,:radio)").val("");
            var xpage = $('ul').find('li.active.main-green').data('page') || 0;
            $.get("/Actions/GetGrid?" + $.param({ requestedPage: xpage })).done(function (data) {
                $("#datalist").html('').append($(data));
                progress(0);
            });
           
        } 
         else {
                $(".error-container").show();
        }
        M.updateTextFields();
        progress(0);
    }
})