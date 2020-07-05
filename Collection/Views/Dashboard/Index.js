
$(function () {
    $(".button-collapse").sideNav();
    $('.debtlist').collapsible('open', 0);
    $('.main-info').collapsible({
        onOpen: function (el) {
            init_map();
        }
    });
    $('.button-collapse').sideNav('show');
    $(".btn-stepbystep").on("click", function (e) {
        var url = $(e.target).prop("href");
        var hash = url.substring(url.indexOf("#") + 1);
        $(".button-collapse").trigger("click");
        if (hash == "!") $('#debtorlist').find('ul.tabs').tabs('select_tab', 'pending_debtors');
        else $('#debtorlist').find('ul.tabs').tabs('select_tab', 'paid_debtors');
    });
    $("#debtorlist").on("click", "#debtlist-table tbody tr", function (e) {
        progress(1);
        $("#debtlist-table > tr").removeClass("active");
        var isCloseInvoice = $(e.target).parent().hasClass('closeInvoice');
        $(".main-info")
            .find(".active").removeClass("active").end()
            .find(".collapsible-body").hide().end();

        $("#graph_info").hide();
        $('.button-collapse').sideNav('hide');

        var xid = $(this).addClass("active").prop("id");
        $.post("Dashboard/InvoiceDetails", { debtorId: xid }).done(function (data) {
            window.currentSelectedDebtorId = xid;
            $("#invoice_info").html(data)
                .off("click", "#btn_dwl_invoice")
                .on("click", "#btn_dwl_invoice", function (e) {
                    progress(1);
                    window.open('Dashboard/InvoiceDownload?debtorId=' + xid + "&isClosed=false", '_blank');
                    progress(0);
                });

            setPickDate();

            $.post("Dashboard/InvoiceClosedDetails", { debtorId: xid }).done(function (data2) {
                $("#closed_info").html(data2)
                    .off("click", "#btn_dwl_invoice_closed")
                    .on("click", "#btn_dwl_invoice_closed", function (e) {
                        progress(1);
                        window.open('Dashboard/InvoiceDownload?debtorId=' + xid + "&isClosed=true", '_blank');
                        progress(0);
                    });

                $.post("Dashboard/AdminDetails", { debtorId: xid }).done(function (data3) {
                    $("#admin_info").html(data3);
                    $.post("Dashboard/DebtorDetails", { debtorId: xid }).done(function (data4) {
                        $("#debtor_info").html(data4)
                            .off("click", "#btn_notes_save")
                            .on("click", "#btn_notes_save", function (e) {
                                progress(1);
                                abp.services.app.Debtors.saveNotes({ Id: xid, Notes: $("#notes").val() })
                                    .done(function (data) {
                                        if (data.Success)
                                            Materialize.toast('Notes saved!', 4000);
                                        else
                                            Materialize.toast('Something went wrong!', 4000);
                                    progress(0);
                                });
                            });
                        $.post("Dashboard/Graph", { debtorId: xid }).done(function (data5) {
                            $("#graph_info").html(data5).show();
                            if ($("#search0").val().length > 0) {
                                $("table tbody tr").unmark().mark($("#search0").val());
                            }
                            $(".no-data-placeholder").hide();
                            init_map();
                            Materialize.updateTextFields();
                            $('ul.tabs').tabs();
                            progress(0);
                            refeshInvoiceInfoPage(isCloseInvoice);
                        });
                    });
                });
            });
        });
    });
});



//$(function () {
//    //Widgets count
//    $('.count-to').countTo();

//    //Sales count to
//    $('.sales-count-to').countTo({
//        formatter: function (value, options) {
//            return '$' + value.toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, ' ').replace('.', ',');
//        }
//    });

//    initRealTimeChart();
//    initDonutChart();
//    initSparkline();
//});

//var realtime = 'on';
//function initRealTimeChart() {
//    //Real time ==========================================================================================
//    var plot = $.plot('#real_time_chart', [getRandomData()], {
//        series: {
//            shadowSize: 0,
//            color: 'rgb(0, 188, 212)'
//        },
//        grid: {
//            borderColor: '#f3f3f3',
//            borderWidth: 1,
//            tickColor: '#f3f3f3'
//        },
//        lines: {
//            fill: true
//        },
//        yaxis: {
//            min: 0,
//            max: 100
//        },
//        xaxis: {
//            min: 0,
//            max: 100
//        }
//    });

//    function updateRealTime() {
//        plot.setData([getRandomData()]);
//        plot.draw();

//        var timeout;
//        if (realtime === 'on') {
//            timeout = setTimeout(updateRealTime, 320);
//        } else {
//            clearTimeout(timeout);
//        }
//    }

//    updateRealTime();

//    $('#realtime').on('change', function () {
//        realtime = this.checked ? 'on' : 'off';
//        updateRealTime();
//    });
//    //====================================================================================================
//}

//function initSparkline() {
//    $(".sparkline").each(function () {
//        var $this = $(this);
//        $this.sparkline('html', $this.data());
//    });
//}

//function initDonutChart() {
//    Morris.Donut({
//        element: 'donut_chart',
//        data: [{
//                label: 'Chrome',
//                value: 37
//            }, {
//                label: 'Firefox',
//                value: 30
//            }, {
//                label: 'Safari',
//                value: 18
//            }, {
//                label: 'Opera',
//                value: 12
//            },
//            {
//                label: 'Other',
//                value: 3
//            }],
//        colors: ['rgb(233, 30, 99)', 'rgb(0, 188, 212)', 'rgb(255, 152, 0)', 'rgb(0, 150, 136)', 'rgb(96, 125, 139)'],
//        formatter: function (y) {
//            return y + '%'
//        }
//    });
//}

//var data = [], totalPoints = 110;
//function getRandomData() {
//    if (data.length > 0) data = data.slice(1);

//    while (data.length < totalPoints) {
//        var prev = data.length > 0 ? data[data.length - 1] : 50, y = prev + Math.random() * 10 - 5;
//        if (y < 0) { y = 0; } else if (y > 100) { y = 100; }

//        data.push(y);
//    }

//    var res = [];
//    for (var i = 0; i < data.length; ++i) {
//        res.push([i, data[i]]);
//    }

//    return res;
//}