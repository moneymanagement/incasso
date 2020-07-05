$.fn.extend({
    animateCss: function (animationName) {
        var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
        this.addClass('animated ' + animationName).one(animationEnd, function() {
            $(this).removeClass('animated ' + animationName);
        });
        return this;
    }
});

$(function() {
	$('select').not("browser-default").material_select();
	Materialize.updateTextFields();
	//$('ul.tabs').tabs({swipeable:true,responsiveThreshold:1920});
	
		$("#form_login").on("click","#btn_submit",function(e){
			var go=1;
			$("input,select,textarea").removeClass("invalid");
			$(".validate").each(function(i,v){
				if ($(this).val()==""){
					$(this).addClass("invalid");
					go=0;
				}
			});
			if (go==1){
				$(".preloader-wrapper").show();
				$(".success-container").hide();
				$(".btn").addClass("disabled").prop("disabled",true);
				$("#form_login").submit();
			}
		}).on("keyup",function(e){
			e.preventDefault();
			if (e.keyCode==13) $("#btn_submit").trigger("click");
		});
	
	
	if (typeof Dropzone !== 'undefined'){
		Dropzone.autoDiscover = false;
		var dz=$("#dropzone").dropzone({
			url: "?upload/upload",clickable:true,maxFiles:1,acceptedFiles:".xls,.xlsx,.xlsm",addRemoveLinks:true,autoProcessQueue:false,
			init: function() {
				var dropzone = this;
				$("#btn_submit").click(function() {
					progress(1);
					dropzone.processQueue();
				});
				dropzone.on("success", function(file,data) {
					progress(1);
					$(".warning-content,.warning-content2,.warning-content3").html("");
					$(".warning-container,.error-container,.warning-content-title,.warning-content2-title,.warning-content3-title").hide();
					if (parseInt(data)!==0){
						$.post('?upload/add',{params:JSON.stringify({filename:data,override:($("#override").is(":checked")?"1":"0"),type:$("input[id^='type']:checked").val(),admins:$("input[id^='admin']:checked").map(function(){return $(this).val();}).get()})}).done(function(data2){
							if (parseInt(data2)>0){
								$(".success-container").show();
								$(":input:not(:checkbox,:radio)").val("");
								$.post("?upload/list").done(function(data3){
									$("#datalist").html(data3);
								});
								progress(0);
								$(".success-container").show();
								dropzone.removeFile(file);
							} else if (parseInt(data2)==0){
								progress(0);
								$(".error-container").show();
							} else {
								progress(0);
								try { var json = JSON.parse(data2); } 
								catch(e){ return false; }
								var goreopen=0,gonew=0,goinactive=0;
								$.each(json,function(i,v){
									switch(parseInt(v["type"])){
										case 1:
											$(".warning-content").append("<div class='card-panel amber lighten-2 z-depth-0 no-padding'> Dossier: "+v["dossier_no"]+"<br/>Invoice: "+v["invoice_no"]+"<br/>"+v["debtor_number"]+" - "+v["debtor_name"]+"</div>");
											goreopen=1;
											break;
										case 2:
											$(".warning-content2").append("<div class='card-panel amber lighten-2 z-depth-0 no-padding'> Dossier: "+v["dossier_no"]+"<br/>Invoice: "+v["invoice_no"]+"<br/>"+v["debtor_number"]+" - "+v["debtor_name"]+"</div>");
											gonew=1;
											break;
										case 3:
											$(".warning-content3").append("<div class='card-panel amber lighten-2 z-depth-0 no-padding'> Dossier: "+v["dossier_no"]+"<br/>Invoice: "+v["invoice_no"]+"<br/>"+v["debtor_number"]+" - "+v["debtor_name"]+"</div>");
											goinactive=1;
											break;
									}
								});
								if (goreopen==1) $(".warning-content-title").show();
								if (gonew==1) $(".warning-content2-title").show();
								if (goinactive==1) $(".warning-content3-title").show();
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
	
		$('#form_upload').on("click","#btn_submit",function(e){
			var go=1;
			$("input,select,textarea").removeClass("invalid");
			$("input.validate,select.validate,textarea.validate").each(function(i,v){
				if ($(this).val()=="" || $(this).val()==null){
					$(this).addClass("invalid");
					go=0;
				}
			});
		});
		
		$("#form_users").on("change","#role",function(data){
			if ($(this).val()=="2"){
				$(".viewer").fadeIn("slow");
			} else $(".viewer").hide();
		}).on("click","#btn_submit",function(e){
			var go=1;
			$("input,select,textarea").removeClass("invalid");
			$("input.validate,select.validate,textarea.validate").each(function(i,v){
				if ($(this).val()=="" || $(this).val()==null){
					$(this).addClass("invalid");
					go=0;
				}
			});
			if (go==1){
				progress(1);
				$.post("?users/add",{params:JSON.stringify({id:$("#edit").val(),name:$("#name").val(),password:$("#password").val(),role:$("#role").val(),incasso:($("#incasso").is(":checked")?1:0),outsourcing:($("#outsourcing").is(":checked")?1:0),admins:$("input[id^='admin']:checked").map(function(){return $(this).val();}).get()})}).done(function(data){
					if (parseInt(data)>0){
						$(".success-container").show();
						$(":input:not(:checkbox,:radio)").val("");
						$.post("?users/list").done(function(data2){
							$("#datalist").html(data2);
						});						
					}
					Materialize.updateTextFields();
					progress(0);
				});
			}
			delete go;
		});
		
		$("#form_debtors").on("change","#admin_id",function(e){
			var that=$(this);
			progress(1);
			$.post("?debtors/list",{admin_id:that.val()}).done(function(data){
				$("#datalist").html(data);
				progress(0);
			});
		}).on("click","#btn_submit",function(e){
			var go=1;
			$("input,select,textarea").removeClass("invalid");
			$("input.validate,select.validate,textarea.validate").each(function(i,v){
				if ($(this).val()=="" || $(this).val()==null){
					$(this).addClass("invalid");
					go=0;
				}
			});
			if (go==1){
				progress(1);
				$.post("?debtors/add",{params:JSON.stringify({id:$("#edit").val(),admin_id:$("#admin_id").val(),number:$("#number").val(),name:$("#name").val(),contact:$("#contact").val(),email:$("#email").val(),phone:$("#phone").val(),phone2:$("#phone2").val(),address:$("#address").val(),postal:$("#postal").val(),city:$("#city").val(),country:$("#country").val(),notes:$("#notes").val(),notes_mm:$("#notes_mm").val()})}).done(function(data){
					if (parseInt(data)>0){
						$(".success-container").show();
						$(":input:not(:checkbox,:radio)").val("");
						$.post("?debtors/list").done(function(data2){
							$("#datalist").html(data2);
						});
					}
					Materialize.updateTextFields();
					progress(0);
				});
			}
			delete go;
		});
		
		$("#datalist").on("click","input[id^='status_']",function(e){
			progress(1);
			var aval=$(this).val().split(":");
			var xid=aval[0],xstatus=3;
			if ($(this).is(":checked")) xstatus=0;
			$.post("?debtors/add",{params:JSON.stringify({id:xid,status:xstatus})}).done(function(){
				progress(0);
			});
			delete aval,xid,xstatus;
		});
	
		$("#form_admins").on("click","#btn_submit",function(e){
			var go=1;
			$("input,select,textarea").removeClass("invalid");
			$("input.validate,select.validate,textarea.validate").each(function(i,v){
				if ($(this).val()=="" || $(this).val()==null){
					$(this).addClass("invalid");
					go=0;
				}
			});
			if (go==1){
				progress(1);
				$.post("?administrations/add",{params:JSON.stringify({id:$("#edit").val(),name:$("#name").val(),number:$("#number").val(),contact:$("#contact").val(),phone:$("#phone").val(),phone2:$("#phone2").val(),email:$("#email").val(),bank:$("#bank").val(),account:$("#account").val(),iban:$("#iban").val(),bic:$("#bic").val(),users:$("input[id^='user']:checked").map(function(){return $(this).val();}).get()})}).done(function(data){
					if (parseInt(data)>0){
						$(".success-container").show();
						$(":input:not(:checkbox,:radio)").val("");
						$.post("?administrations/list").done(function(data2){
							$("#datalist").html(data2);
						});
					}
					Materialize.updateTextFields();
					progress(0);
				});
			}
			delete go;
		});
		
	$(".maincontent").on("keyup",".search",function(e){
		e.preventDefault();e.stopPropagation();
		go=0;
		if (e.keyCode==13) go=1;
		
		if (go==1){
			progress(1);
			//if (e.target.id=="search1"){
				if (e.target.id=="search0")
					$('.button-collapse').sideNav('show');
				$(".no-data-placeholder").show();
				$(".main-info,#graph_info").hide();
			
				var xsearch=$(this).val(),href="";
				if (e.target.id=="search1"){
					href=$('ul.tabs').find(".active").attr('href');
				}
				$('.debtlist').collapsible('destroy');
				$.post("?home/debtor-list",{s:xsearch}).done(function(data){
					$("#debtorlist").html(data);
					if ($('#sidenav-overlay').length <=0) $('.button-collapse').sideNav('show');
					$('ul.tabs').tabs();
					$('.debtlist').collapsible();
					$('.debtlist').collapsible('open',0);
					if (href) $('ul.tabs').tabs('select_tab',href.substr(1));
					progress(0);
				});
				
			//}
		}
		if (e.target.id=="search0"){
			if ($(".main-info").is(":visible")){
				$("#invoice-table tbody tr").show().unmark();
				if (e.target.value){
					$("#invoice-table tbody tr").mark($(".search").val(),{
						done: function() {
							$("#invoice-table tbody tr").not(":has(mark)").hide();
							progress(0);
						}
					});
				}
			}
		}
		if (go==1){
			$("#"+e.target.id).focus();
			$('.collapsible').collapsible();
			return false;
		}
	});
	
	
	$("#invoice_info,#closed_info").on("click",".invoice-row",function(e){
		var $el=$(this).next(".invoice-notes");
		if ($el.is(":hidden")) $el.slideDown();
		else $el.slideUp("fast");	
	});
	
});

function showModal(callback){
	$('#confirm-box').modal("open");
	$('#confirm-box_NoBtn').on("click",function(){
		$('#confirm-box').modal("close");
		return false;
	})
	
	$('#confirm-box_YesBtn').on("click",function(){
		$('#confirm-box').modal("close"); 
		if ($.isFunction(callback)) callback();
	}); 
} 

function progress(t){
	if (t===1){
		$(".progress").removeClass("main-green").addClass("green lighten-3");
		$(".indeterminate").show();
		$(".success-container,.error-container").hide();
		$(".btn").addClass("disabled").prop("disabled",true);
	} else {
		$(".indeterminate").fadeOut();
		$(".progress").addClass("main-green").removeClass("green lighten-3");
		$(".btn").removeClass("disabled").prop("disabled",false);
	}
}

var map_id,map_name,map_address,map_location;
function init_map(){
		var mapOptions = {zoom:14,center:new google.maps.LatLng(52.3546274,4.8285839),mapTypeId:google.maps.MapTypeId.ROADMAP};
		var geocoder = new google.maps.Geocoder();
		if (map_address){
			geocoder.geocode({'address': map_address},function(results,status){
				if (status == google.maps.GeocoderStatus.OK){
					map = new google.maps.Map(document.getElementById('map_canvas'+map_id), mapOptions);
					marker = new google.maps.Marker({map: map,position: new google.maps.LatLng(results[0].geometry.location.lat(),results[0].geometry.location.lng())});
					infowindow = new google.maps.InfoWindow({content:'<strong>'+map_name+'</strong>'});
					google.maps.event.addListener(marker, 'click', function(){infowindow.open(map,marker);});
					infowindow.open(map,marker);
				} else{
					map_address=map_location;
					init_map();
				}
			});	
		}
}

function setlist(target){
		$("#datalist").on("click","ul.pagination > li:not('.disabled')",function(e){
			var xpage=$(this).attr("data-page");
			progress(1);
			$.post("?"+target+"/list",{page:xpage}).done(function(data){
				$("#datalist").html(data);
				progress(0);
			});
			delete xpage;
		}).on("keyup","#tablesearch",function(e){
			go=0;
			if (e.keyCode==13) go=1;
			if (go==1){
				progress(1);
				$.post("?"+target+"/list",{s:$("#tablesearch").val()}).done(function(data){
					$("#datalist").html(data);
					progress(0);
				});
			}
		}).on("click","#table-list .btn_delete",function(e){
			var xid=$(this).attr("data-id");
			showModal(function(){
				progress(1);
				$.post("?"+target+"/delete",{id:xid}).done(function(data){
					if (parseInt(data)>0){
						$.post("?"+target+"/list",{page:$("ul.pagination").find("li.active").attr("data-page")}).done(function(data2){
							$("#datalist").html(data2);
							progress(0);
						});
					} else progress(0);
				});
			});return false;
		}).on("click","#table-list .btn_edit",function(e){
			progress(1);
			var xid=$(this).attr("data-id");
			$.post("?"+target+"/edit",{id:xid}).done(function(data){
				try { var json = JSON.parse(data); } 
				catch(e){ return false; }
				$.each(json,function(i,v){
					if ($("#"+i).is("*")) $("#"+i).val(v);
				});
				if ($("#edit").is("*") && json.hasOwnProperty('id'))
					$("#edit").val(json.id);
				
				if ($("input[id^='user'").is("*") && json.hasOwnProperty('users')){
					$("input[id^='user'").prop("checked",false);
					$.each(JSON.parse(json.users),function(i,v){
						$("#user"+v).prop("checked",true);
					});
				}
				if ($("input[id^='admin'").is("*") && json.hasOwnProperty('admins')){
					$("input[id^='admin'").prop("checked",false);
					$.each(JSON.parse(json.admins),function(i,v){
						$("#admin"+v).prop("checked",true);
					});
				}
				if ($("#outsourcing").is("*") && $("#incasso").is("*") && json.hasOwnProperty('outsourcing') && json.hasOwnProperty('incasso')){
					$("#outsourcing,#incasso").prop("checked",false);
					if (parseInt(json.outsourcing)==1) $("#outsourcing").prop("checked",true);
					if (parseInt(json.incasso)==1) $("#incasso").prop("checked",true);
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
