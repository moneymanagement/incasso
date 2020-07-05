(function () {
    $(function () {
        var service = abp.services.app.UploadData;
        if (typeof Dropzone !== 'undefined') {
            Dropzone.autoDiscover = false;
            var dz = $("#dropzone").dropzone({
                url: "UploadData/Upload", clickable: true, maxFiles: 1, acceptedFiles: ".xls,.xlsx,.xlsm", addRemoveLinks: true, autoProcessQueue: false,
                init: function () {
                    var dropzone = this;
                    $("#btn_submit").click(function () {
                        progress(1);
                        dropzone.processQueue();
                    });
                    dropzone.on("success", function (file, data) {
                        progress(1);
                        $(".warning-content,.warning-content2,.warning-content3").html("");
                        $(".warning-container,.error-container,.warning-content-title,.warning-content2-title,.warning-content3-title").hide();
                        if (parseInt(data) !== 0) {
                            var service = abp.services.app.UploadData;
                            service.create({
                                IsOverride: ($("#Override").is(":checked") ? true : false), FileName: data.result.fileName,
                                PhysicalFileName: data.result.physicalFileName, PhysicalFilePath: data.result.physicalFilePath,
                                FileType: $("input[id^='Type']:checked").val(), Admins: $("input[id^='Admin']:checked").map(function () { return $(this).val(); }).get()
                            }
                            ).done(function (data2) {
                                if (data2.Success) {
                                    $(".success-container").show();
                                    $(":input:not(:checkbox,:radio)").val("");
                                    $.get(abp.appPath + "UploadData/GetGrid?" + $.param({ requestedPage: 0 })).done(function (data) {
                                        $("#datalist").html('').append($(data));
                                        progress(0);
                                    });
                                    progress(0);
                                    $(".success-container").show();
                                    dropzone.removeFile(file);
                                } else if (parseInt(data2) == 0) {
                                    progress(0);
                                    $(".error-container").show();
                                } else {
                                    progress(0);
                                    try { var json = JSON.parse(data2); }
                                    catch (e) { return false; }
                                    var goreopen = 0, gonew = 0, goinactive = 0;
                                    $.each(json, function (i, v) {
                                        switch (parseInt(v["type"])) {
                                            case 1:
                                                $(".warning-content").append("<div class='card-panel amber lighten-2 z-depth-0 no-padding'> Dossier: " + v["dossier_no"] + "<br/>Invoice: " + v["invoice_no"] + "<br/>" + v["debtor_number"] + " - " + v["debtor_name"] + "</div>");
                                                goreopen = 1;
                                                break;
                                            case 2:
                                                $(".warning-content2").append("<div class='card-panel amber lighten-2 z-depth-0 no-padding'> Dossier: " + v["dossier_no"] + "<br/>Invoice: " + v["invoice_no"] + "<br/>" + v["debtor_number"] + " - " + v["debtor_name"] + "</div>");
                                                gonew = 1;
                                                break;
                                            case 3:
                                                $(".warning-content3").append("<div class='card-panel amber lighten-2 z-depth-0 no-padding'> Dossier: " + v["dossier_no"] + "<br/>Invoice: " + v["invoice_no"] + "<br/>" + v["debtor_number"] + " - " + v["debtor_name"] + "</div>");
                                                goinactive = 1;
                                                break;
                                        }
                                    });
                                    if (goreopen == 1) $(".warning-content-title").show();
                                    if (gonew == 1) $(".warning-content2-title").show();
                                    if (goinactive == 1) $(".warning-content3-title").show();
                                    $(".warning-container").show();
                                    dropzone.removeFile(file);
                                }
                            }).fail(function (xhr, status, error) {
                                alert(error);
                                progress(0);
                                dropzone.removeFile(file);
                                $(":input:not(:checkbox,:radio)").val("");
                            });


                        } else {
                            progress(0);
                            $(".error-container").show();
                        }
                    });
                }
            }).addClass("dropzone");
        }

        $('#form_upload').on("click", "#btn_submit", function (e) {
            var go = 1;
            $("input,select,textarea").removeClass("invalid");
            $("input.validate,select.validate,textarea.validate").each(function (i, v) {
                if ($(this).val() == "" || $(this).val() == null) {
                    $(this).addClass("invalid");
                    go = 0;
                }
            });
        });

    });
})();