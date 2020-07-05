$.fn.extend({
	animateCss: function (animationName) {
		var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
		this.addClass('animated ' + animationName).one(animationEnd, function () {
			$(this).removeClass('animated ' + animationName);
		});
		return this;
	}
});

function defaultSelectSetup(){
	$('select').formSelect();
	M.updateTextFields();
}
$(function () {
	$('select').formSelect();
    M.updateTextFields();
    if (abp.localization.currentCulture.name == "en")
    {
        $('#language').val('en-US')
    } else {
        $('#language').val('nl-NL')
    }
    $('#language').trigger("change").formSelect()

    //$('ul.tabs').tabs({swipeable:true,responsiveThreshold:1920});

    $('#language').change(function (elment, ele) {
        if ($(elment.target).val()=="nl-NL")
        $('#languageNederlands >i').click()
        else
        $('#languageEnglish >i').click()
    });
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
			$.post("?home/debtor-list", { s: xsearch }).done(function (data) {
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


});

function showModal(callback) {
	$('#confirm-box').modal("");
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

function setlist(option) {
    $("#datalist")
        .on("click", "ul.pagination > li:not('.disabled')", function (e) {

            var xpage = $(this).attr("data-page");
            if ($(this).hasClass('chevron')) {
                xpage = $(this).parent('ul').find('li.active.main-green').data('page') + $(this).data('page');
                if ($(this).parent('ul').find('li[data-page="' + xpage + '"]').length === 0 || xpage === -1)
                    return;
            }
            progress(1);
            $.get( option.target + "/GetGrid?" + $.param({ requestedPage: xpage })).done(function (data) {
                $("#datalist").html('').append($(data));
                progress(0);
            });
            delete xpage;
        })

        .on("keyup", "#tablesearch", function (e) {
            go = 0;
            if (e.keyCode == 13) go = 1;
            if (go == 1) {
                progress(1);
                var xpage = $('ul').find('li.active.main-green').data('page') || 0;
                $.get( option.target + "/GetGrid?" + $.param({ search: $(e.target).val(), requestedPage: xpage })).done(function (data) {
                    $("#datalist").html('').append($(data));
                    progress(0);
                });
            }
        }).on("change", "#filtertype", function (e) {
            progress(1);
            $.get(option.target + "/GetGrid?" + $.param({ search: $('#tablesearch').val(), InvoiceType: $('#filtertype').val()})).done(function (data) {
                $("#datalist").html('').append($(data));
                progress(0);
            });
        }).on("click", "#table-list .btn_delete", function (e) {
             var xid = $(this).attr("data-id");
        showModal(function () {
            progress(1);
            appservice = option.target == "Actions" ? "Invoices" : option.target;
            $.post(apiURL + appservice+ "/delete", { id: xid }).done(function (data) {
                if (data.Success) {
                    var xpage = $('ul').find('li.active.main-green').data('page') || 0;
                    $.get( option.target + "/GetGrid?" + $.param({ requestedPage: xpage })).done(function (data) {
                        $("#datalist").html('').append($(data));
                        progress(0);
                    });
                } else progress(0);
            });
        }); return false;
        }).on("click", "#table-list .btn_edit", function (e) {
        progress(1);
            var xid = $(this).attr("data-id");
            appservice = option.target == "Actions" ? "Invoices" : option.target;
            $.post(apiURL + appservice + "/Get?", { Id: xid }).done(function (data) {
            try { var json = (data.Result); }
                catch (e) { return false; }
              
			$.each(json, function (i, v) {
                if (i.indexOf('$') < 0 && $("#" + i).is("*"))
                    $("#" + i).val(v);
            });
                if ($("#edit").is("*") && json.hasOwnProperty('Id')) {
                    $("#edit").val(json.Id);
                }

                if ($("#hiddenstatus").is("*") && json.hasOwnProperty('Status')) {
                    $("#hiddenstatus").val(json.Status);
                }

                if ($("input[id^='Password'").is("*")) {
                    $("#Password").removeClass("validate invalid");
                }

			if ($("input[id^='User'").is("*") && json.hasOwnProperty('Users')) {
				$("input[id^='User'").prop("checked", false);
                $.each((json.Users), function (i, v) {
                        $("#User" + v.Id).prop("checked", true);
				});
            }
            if ($("input[id^='Admin'").is("*") && json.hasOwnProperty('Administrators')) {
				$("input[id^='Admin'").prop("checked", false);
                $.each((json.Administrators), function (i, v) {
                    $("#Admin" + v.Id).prop("checked", true);
				});
			}
			if ($("#Outsourcing").is("*") && $("#Incasso").is("*") && json.hasOwnProperty('Outsourcing') && json.hasOwnProperty('Incasso')) {
				$("#Outsourcing,#Incasso").prop("checked", false);
				if (parseInt(json.outsourcing) == 1) $("#Outsourcing").prop("checked", true);
				if (parseInt(json.incasso) == 1) $("#Incasso").prop("checked", true);
            }

        	if ($("#OutSourcing").is("*") && $("#Incasso").is("*") && json.hasOwnProperty('OutSourcing') && json.hasOwnProperty('Incasso')) {
				$("#OutSourcing,#Incasso").prop("checked", false);
                if ((json.OutSourcing)) $("#OutSourcing").prop("checked", true);
                if ((json.Incasso)) $("#Incasso").prop("checked", true);
            }


            if (json.hasOwnProperty("FileType") && json.FileType == "Outsourcing") {
                $("#Type0").prop("checked", true);

            } else {
                $("#Type0").prop("checked", false);
            }

            if (json.hasOwnProperty("FileType") && json.FileType == "Collection") {
                $("#Type1").prop("checked", true);
            } else {
                $("#Type1").prop("checked", false);
            }
            if (json.hasOwnProperty("Override") && json.Override) {
                $('#Override').prop("checked", true)
            } else {
                $('#Override').prop("checked", false)

            }
            if (json.hasOwnProperty("Closed") && json.Closed) {
                $('#Closed').val('true')
            } else {
                $('#Closed').val('false')
            }

            if (json.hasOwnProperty("Debtor") && json.Debtor) {
                $('#DebtorNumber').val(json.Debtor.Number)
            } else {
                $('#DebtorNumber').val('')
            }

           
            if ($("#Closed").is("*"))
                 $('#Closed').trigger("change").formSelect();

            if ($("#Role").is("*"))
				$('#Role').trigger("change").formSelect();

            try {
                $('select').formSelect('destroy');
            } catch (e) {

            }
            $('select').formSelect();
			M.updateTextFields();
			progress(0);
			delete json;
		});
		delete xid;
	});
}
