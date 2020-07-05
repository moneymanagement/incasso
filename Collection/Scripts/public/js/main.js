function formatDate(input) {
    var datePart = input.match(/\d+/g),
        year = datePart[0], // get only two digits
        month = datePart[1], day = datePart[2];

    return day + '-' + month + '-' + year;
}

$.fn.extend({
    animateCss: function (animationName) {
        var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
        this.addClass('animated ' + animationName).one(animationEnd, function () {
            $(this).removeClass('animated ' + animationName);
        });
        return this;
    }
});

$(function () {
    setPickDate();
    $('select').not("browser-default").material_select();
    Materialize.updateTextFields();
    //$('ul.tabs').tabs({swipeable:true,responsiveThreshold:1920});

    $("#form_login").on("click", "#btn_submit", function (e) {
        var go = 1;
        $("input,select,textarea").removeClass("invalid");
        $(".validate").each(function (i, v) {
            if ($(this).val() == "") {
                $(this).addClass("invalid");
                go = 0;
            }
        });
        if (go == 1) {
            $(".preloader-wrapper").show();
            $(".success-container").hide();
            $(".btn").addClass("disabled").prop("disabled", true);
            $("#form_login").submit();
        }
    }).on("keyup", function (e) {
        e.preventDefault();
        if (e.keyCode == 13) $("#btn_submit").trigger("click");
    });


    if (typeof Dropzone !== 'undefined') {
        Dropzone.autoDiscover = false;
        var dz = $("#dropzone").dropzone({
            url: "?upload/upload", clickable: true, maxFiles: 1, acceptedFiles: ".xls,.xlsx,.xlsm", addRemoveLinks: true, autoProcessQueue: false,
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
                        $.post('?upload/add', { params: JSON.stringify({ filename: data, override: ($("#override").is(":checked") ? "1" : "0"), type: $("input[id^='type']:checked").val(), admins: $("input[id^='admin']:checked").map(function () { return $(this).val(); }).get() }) }).done(function (data2) {
                            if (parseInt(data2) > 0) {
                                $(".success-container").show();
                                $(":input:not(:checkbox,:radio)").val("");
                                $.post("?upload/list").done(function (data3) {
                                    $("#datalist").html(data3);
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
                        });
                    } else {
                        progress(0);
                        $(".error-container").show();
                    }
                });
            }
        }).addClass("dropzone");
    }
    $(".maincontent").on("keyup", ".search", function (e) {
        e.preventDefault(); e.stopPropagation();
        go = 0;
        if (e.keyCode == 13) go = 1;

        if (go == 1) {
            progress(1);

            //if (e.target.id=="search1"){
            if (e.target.id == "search0")
                $('.button-collapse').sideNav('show');
            $(".no-data-placeholder").show();
            $(".main-info,#graph_info").hide();

            var xsearch = $(this).val(), href = "";
            if (e.target.id == "search1") {
                href = $('ul.tabs').find(".active").attr('href');
            }
            $('.debtlist').collapsible('destroy');
            $.post("Dashboard/DebtorList", { query: xsearch }).done(function (data) {
                $("#debtorlist").html(data);
                if ($('#sidenav-overlay').length <= 0) $('.button-collapse').sideNav('show');
                $('ul.tabs').tabs();
                $('.debtlist').collapsible();
                $('.debtlist').collapsible('open', 0);
                if (href) $('ul.tabs').tabs('select_tab', href.substr(1));
                progress(0);
            });

            //}
        }
        if (e.target.id == "search0") {
            if ($(".main-info").is(":visible")) {
                $("#invoice-table tbody tr").show().unmark();
                if (e.target.value) {
                    $("#invoice-table tbody tr").mark($(".search").val(), {
                        done: function () {
                            $("#invoice-table tbody tr").not(":has(mark)").hide();
                            progress(0);
                        }
                    });
                }
            }
        }
        if (go == 1) {
            $("#" + e.target.id).focus();
            $('.collapsible').collapsible();
            return false;
        }
    });

    $("#invoice_info,#closed_info").on("click", ".invoice-row", function (e) {
        if ($(e.target).attr('name') == "selectedInvoice[]")
            return;
        var $el = $(this).next(".invoice-notes");
        if ($el.is(":hidden")) $el.slideDown();
        else $el.slideUp("fast");
    });

    $('.maincontent')
        .off("click", ".note_delete")
        .on("click", ".note_delete", function (event) {
            event.preventDefault();
            var noteID = $(this).attr('data-id');
            showModal(function () {
                progress(1);
                abp.services.app.Invoices.deleteNotes({ Id: noteID }).done(function (data) {
                    if (data.Success) {
                        progress(0);
                        $('#note_view_' + noteID).closest('.note-block').remove();
                    } else progress(0);
                });
            });
        });

    $('.maincontent')
        .off("click", ".note_update")
        .on("click", ".note_update", function (event) {
            event.preventDefault();
            var _noteID = $(this).attr('data-id');
            var _note = $('#note_text_' + _noteID).val();
            var _invoiceID = $(this).closest('tr').attr('data-invoice');
            var pureElObject = getPurePickDateObject('note_date_' + _noteID);
            var _date = pureElObject.get('select', 'yyyy-mm-dd');
            //var _date = $('#note_date_' + _noteID).val();

            if (_note.length > 3 && formatDate(_date).length > 6) {
                progress(1);
                abp.services.app.Invoices.updateNotes( { Id: _noteID, Notes: _note, Date: _date, InvoiceId: _invoiceID }).done(function (data) {
                    if (data.Success) {
                        progress(0);
                        // $note_view = $('#note_view_' + _noteID);
                        // $note_edit = $('#note_edit_' + _noteID);
                        // $('#note_view_' + _noteID).show();
                        // $('#note_edit_' + _noteID).hide();
                        // $('.semibold', $note_view).html(formatDate(_date));
                        // $('.note', $note_view).html(_note);
                        $('[name*="selectedInvoice"]:checked').prop("checked", false);
                        if (window.currentSelectedDebtorId) {
                            var xid = window.currentSelectedDebtorId;
                            $.post("Dashboard/InvoiceDetails", { debtorId: xid }).done(function (data) {
                                $("#invoice_info").html(data);
                                setPickDate();
                                progress(0);
                                //$.post("?home/invoice-closed-info", { d: xid }).done(function (data2) {
                                //    $("#closed_info").html(data2);
                                //    progress(0);
                                  refeshInvoiceInfoPage();
                                //});
                            });
                        }

                    } else progress(0);
                });
            } else {
                alert('please enter valid input');
            }
        });

     //$('.maincontent')
     //	.off("click", "#invoice-table input[type=checkbox]")
     //	.on("click", "#invoice-table input[type=checkbox]", function (event) {
     //		event.preventDefault();
     //		if (event.stopPropagation) {    // standard
     //			event.stopPropagation();
     //		} else {    // IE6-8
     //			 event.cancelBubble = true;
     //		}
     //	});

    $('.maincontent')
        .off("click", ".note_add")
        .on("click", ".note_add", function (event) {
            event.preventDefault();
            var pureElObject = getPurePickDateObject("note_date_add");
            var _note = $('#note_text_add').val();
            var _date = pureElObject.get('select', 'yyyy-mm-dd');
            var _selectedInvoice = $('[name*="selectedInvoice"]:checked');

            if (_selectedInvoice.length <= 0) {
                alert('please select invoice in which you want to add notes');
                return false;
            }
            if (_note.length <= 3) {
                alert('please add more text in note'); return false;
            }
             if(formatDate(_date).length!=10){
             	alert("please select date");return false;
             }

            var _selectedInvoiceIds = [];
            $('[name*="selectedInvoice"]:checked').each(function () {
                _selectedInvoiceIds.push($(this).val());
            });

            if (_note.length > 3 && formatDate(_date).length > 6) {
                progress(1);
                abp.services.app.Invoices.addNotes({ Ids: _selectedInvoiceIds, Notes: _note, Date: _date }).done(function (data) {
                    if (data.Success) {
                        var d = new Date();
                        $('#note_text_add').val('');
                        var pureElObject = getPurePickDateObject("note_date_add");
                        pureElObject.set('select', new Date());
                        $('[name*="selectedInvoice"]:checked').prop("checked", false);
                        if (window.currentSelectedDebtorId) {
                            var xid = window.currentSelectedDebtorId;
                            $.post("Dashboard/InvoiceDetails", { debtorId: xid }).done(function (data) {
                                $("#invoice_info").html(data);
                                setPickDate();
                                progress(0);// comment if underlines are uncomment
                                //$.post("?home/invoice-closed-info", { d: xid }).done(function (data2) {
                                //    $("#closed_info").html(data2);
                                //    progress(0);
                                  refeshInvoiceInfoPage();
                                //});
                            });
                        }
                    } else progress(0);
                });
            } else {
                alert('please enter valid input');
            }
        });

    $('.maincontent')
        .off("click", ".note_add_refresh")
        .on("click", ".note_add_refresh", function (event) {
            event.preventDefault();
            $('#note_text_add').val('');
            var pureElObject = getPurePickDateObject("note_date_add");
            pureElObject.set('select', new Date());
            $('[name*="selectedInvoice"]:checked').prop("checked", false);
        });

    $('.maincontent')
        .off("click", ".note_edit")
        .on("click", ".note_edit", function (event) {
            event.preventDefault();
            var noteID = $(this).attr('data-id');
            $note_view = $('#note_view_' + noteID);
            var pureElObject = getPurePickDateObject("note_date_" + noteID);
            pureElObject.set('select', $('.semibold', $note_view).html(), { format: 'dd-mm-yyyy' });
            $("#note_text_" + noteID).val($('.note', $note_view).html());

            $('#note_view_' + noteID).hide();
            $('#note_edit_' + noteID).show();
        });

    $('.maincontent')
        .off("click", ".note_back")
        .on("click", ".note_back", function (event) {
            event.preventDefault();
            debugger
            var noteID = $(this).attr('data-id');
            $note_view = $('#note_view_' + noteID);
            var pureElObject = getPurePickDateObject("note_date_" + noteID);
            pureElObject.set('select', $('.semibold', $note_view).html(), { format: 'dd-mm-yyyy' });
            $("#note_text_" + noteID).val($('.note', $note_view).html());

            $('#note_view_' + noteID).show();
            $('#note_edit_' + noteID).hide();
        });
});

function showModal(callback) {
    $('#confirm-box').modal("open");
    $('#confirm-box_NoBtn').on("click", function () {
        $('#confirm-box').modal("close");
        return false;
    })

    $('#confirm-box_YesBtn').on("click", function () {
        $('#confirm-box').modal("close");
        if ($.isFunction(callback)) callback();
    });
}

function progress(t) {
    if (t === 1) {
        $(".progress").removeClass("main-green").addClass("green lighten-3");
        $(".indeterminate").show();
        $(".success-container,.error-container").hide();
        $(".btn").addClass("disabled").prop("disabled", true);
    } else {
        $(".indeterminate").fadeOut();
        $(".progress").addClass("main-green").removeClass("green lighten-3");
        $(".btn").removeClass("disabled").prop("disabled", false);
    }
}

var map_id, map_name, map_address, map_location;
function init_map() {
    var mapOptions = { zoom: 14, center: new google.maps.LatLng(52.3546274, 4.8285839), mapTypeId: google.maps.MapTypeId.ROADMAP };
    var geocoder = new google.maps.Geocoder();
    if (map_address) {
        geocoder.geocode({ 'address': map_address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                map = new google.maps.Map(document.getElementById('map_canvas' + map_id), mapOptions);
                marker = new google.maps.Marker({ map: map, position: new google.maps.LatLng(results[0].geometry.location.lat(), results[0].geometry.location.lng()) });
                infowindow = new google.maps.InfoWindow({ content: '<strong>' + map_name + '</strong>' });
                google.maps.event.addListener(marker, 'click', function () { infowindow.open(map, marker); });
                infowindow.open(map, marker);
            } else {
                map_address = map_location;
                init_map();
            }
        });
    }
}

function setlist(target) {
    $("#datalist").on("click", "ul.pagination > li:not('.disabled')", function (e) {
        var xpage = $(this).attr("data-page");
        progress(1);
        $.post("?" + target + "/list", { page: xpage }).done(function (data) {
            $("#datalist").html(data);
            progress(0);
        });
        delete xpage;
    }).on("keyup", "#tablesearch", function (e) {
        go = 0;
        if (e.keyCode == 13) go = 1;
        if (go == 1) {
            progress(1);
            $.post("?" + target + "/list", { s: $("#tablesearch").val() }).done(function (data) {
                $("#datalist").html(data);
                progress(0);
            });
        }
    }).on("click", "#table-list .btn_delete", function (e) {
        var xid = $(this).attr("data-id");
        showModal(function () {
            progress(1);
            $.post("?" + target + "/delete", { id: xid }).done(function (data) {
                if (parseInt(data) > 0) {
                    $.post("?" + target + "/list", { page: $("ul.pagination").find("li.active").attr("data-page") }).done(function (data2) {
                        $("#datalist").html(data2);
                        progress(0);
                    });
                } else progress(0);
            });
        }); return false;
    }).on("click", "#table-list .btn_edit", function (e) {
        progress(1);
        var xid = $(this).attr("data-id");
        $.post("?" + target + "/edit", { id: xid }).done(function (data) {
            try { var json = JSON.parse(data); }
            catch (e) { return false; }
            $.each(json, function (i, v) {
                if ($("#" + i).is("*")) $("#" + i).val(v);
            });
            if ($("#edit").is("*") && json.hasOwnProperty('id'))
                $("#edit").val(json.id);

            if ($("input[id^='user'").is("*") && json.hasOwnProperty('users')) {
                $("input[id^='user'").prop("checked", false);
                $.each(JSON.parse(json.users), function (i, v) {
                    $("#user" + v).prop("checked", true);
                });
            }
            if ($("input[id^='admin'").is("*") && json.hasOwnProperty('admins')) {
                $("input[id^='admin'").prop("checked", false);
                $.each(JSON.parse(json.admins), function (i, v) {
                    $("#admin" + v).prop("checked", true);
                });
            }
            if ($("#outsourcing").is("*") && $("#incasso").is("*") && json.hasOwnProperty('outsourcing') && json.hasOwnProperty('incasso')) {
                $("#outsourcing,#incasso").prop("checked", false);
                if (parseInt(json.outsourcing) == 1) $("#outsourcing").prop("checked", true);
                if (parseInt(json.incasso) == 1) $("#incasso").prop("checked", true);
            }
            if ($("#role").is("*"))
                $('#role').trigger("change").material_select();
            $('select').not("browser-default").material_select('destroy');
            $('select').not("browser-default").material_select();
            Materialize.updateTextFields();
            progress(0);
            delete json;
        });
        delete xid;
    });
}

function hideClosedInvoiceAddNoteSection() {
    $('#invoice_note_add_or_update').closest('li').find('.collapsible-header').removeClass('active');
    $('#invoice_note_add_or_update').hide();
}

function showOpenInvoices() {
    $('#invoice_info').closest('li').find('.collapsible-header').addClass('active');
    $('#invoice_info').show();
}

function showClosedInvoices() {
    $('#closed_info').closest('li').find('.collapsible-header').addClass('active');
    $('#closed_info').show();
}
function hideOpenInvoices() {
    $('#invoice_info').closest('li').find('.collapsible-header').removeClass('active')
    $('#invoice_info').hide();
}
function hideClosedInvoices() {
    $('#closed_info').closest('li').find('.collapsible-header').removeClass('active')
    $('#closed_info').hide();
}

function refeshInvoiceInfoPage(isClosed) {
    window.cop = setTimeout(function () {
            hideClosedInvoices();
           hideOpenInvoices();

        if (isClosed) {
            showClosedInvoices();
        } else {
            showOpenInvoices();
        }
        //hideClosedInvoiceAddNoteSection();
        if (window.cop)
            clearTimeout(window.cop);

    }, 1000);
}

function getPurePickDateObject(id) {
    var elObject = $('#' + id).pickadate();
    var pureElObject = elObject.pickadate('picker');
    return pureElObject;
}

function setPickDate() {
    $('[id*="note_date_"].datepicker').each(function () {
        var elObject = $(this).pickadate({
            format: "dd/mm/yyyy",
            formatSubmit: 'yyyy-mm-dd',
        });

        if ($(this).id == "note_date_add") {
            var pureElObject = elObject.pickadate('picker');
            pureElObject.set('select', new Date());
        }
    });
}