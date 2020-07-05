(function () {
    $(function () {
      
            setTimeout(function () {
                $('#Name').val('');
                $('#Password').val('');
            }, 600);
 
        var service = abp.services.app.users;
        $("#form_users").on("change", "#Role", function (data) {
            if ($(this).val() == "Viewer" || $(this).val() == "Editor") {
                $(".viewer").fadeIn("slow");
            } else $(".viewer").hide();
        }).on("click", "#btn_submit", function (e) {
            var go = 1;
            $("input,select,textarea").removeClass("invalid");
            $("input.validate,select.validate,textarea.validate").each(function (i, v) {

                if ($(this).val() == "" || $(this).val() == null) {
                    $(this).addClass("invalid");
                    go = 0;
                }
            });
            var data = {
                Id: $("#edit").val(),
                UserName: $("#Name").val(),
                Name: $("#Name").val(),
                Surname: $("#Name").val(),
                EmailAddress: $("#Name").val() + "@yopmail.com",
                IsActive: true,
                Password: $("#Password").val(),
                RoleNames: [$("#Role option:selected").text()],
                Incasso: ($("#Incasso").is(":checked")),
                OutSourcing: ($("#OutSourcing").is(":checked") ),
                Admins: $("input[id^='Admin']:checked").map(function () { return $(this).val(); }).get()
            }; 
            var method = (+data.Id > 0 ? "Update" : "Create" );
            if (go == 1) {
                progress(1);
                if (method == "Create") {
                    if (!data.Password) {
                        $("#Password").addClass(" validate invalid");
                        progress(0);
                        return;
                    }
                    service.create(data).done(function (data) {
                        if (data.Success) {
                            $(".success-container").show();
                            $(":input:not(:checkbox,:radio)").val("");
                            progress(1);
                            var xpage = $('ul').find('li.active.main-green').data('page') || 0;
                            $.get("/Users/GetGrid?" + $.param({ requestedPage: xpage })).done(function (data) {
                                $("#datalist").html('').append($(data));
                                progress(0);
                            });
                        } else {
                            $(".error-container").show();
                        }
                        M.updateTextFields();
                        progress(0);
                    });
                } else {

                    service.update(data).done(function (data) {
                        if (data.Success) {
                            $(".success-container").show();
                            $(":input:not(:checkbox,:radio)").val("");
                            $("input[id^='Admin'").prop("checked", false);
                            $("#Outsourcing,#Incasso").prop("checked", false);
                            progress(1);
                            var xpage = $('ul').find('li.active.main-green').data('page') || 0;
                            $.get("/Users/GetGrid?" + $.param({ requestedPage: xpage })).done(function (data) {
                                $("#datalist").html('').append($(data));
                                progress(0);
                            });
                        } else {
                            $(".error-container").show();
                        }
                        M.updateTextFields();
                        progress(0);
                    });
                }
            }
            delete go;
        });





        var _userService = abp.services.app.user;
        var _$modal = $('#UserCreateModal');
        var _$form = _$modal.find('form');

        _$form.validate({
            rules: {
                Password: "required",
                ConfirmPassword: {
                    equalTo: "#Password"
                }
            }
        });

        $('#RefreshButton').click(function () {
            refreshUserList();
        });

        $('.delete-user').click(function () {
            var userId = $(this).attr("data-user-id");
            var userName = $(this).attr('data-user-name');

            deleteUser(userId, userName);
        });

        $('.edit-user').click(function (e) {
            var userId = $(this).attr("data-user-id");

            e.preventDefault();
            $.ajax({
                url: abp.appPath + 'Users/EditUserModal?userId=' + userId,
                type: 'POST',
                contentType: 'application/html',
                success: function (content) {
                    $('#UserEditModal div.modal-content').html(content);
                },
                error: function (e) { }
            });
        });

        _$form.find('button[type="submit"]').click(function (e) {
            e.preventDefault();

            if (!_$form.valid()) {
                return;
            }

            var user = _$form.serializeFormToObject(); //serializeFormToObject is defined in main.js
            user.roleNames = [];
            var _$roleCheckboxes = $("input[name='role']:checked");
            if (_$roleCheckboxes) {
                for (var roleIndex = 0; roleIndex < _$roleCheckboxes.length; roleIndex++) {
                    var _$roleCheckbox = $(_$roleCheckboxes[roleIndex]);
                    user.roleNames.push(_$roleCheckbox.attr('data-role-name'));
                }
            }

            abp.ui.setBusy(_$modal);
            _userService.create(user).done(function () {
                _$modal.modal('hide');
                location.reload(true); //reload page to see new user!
            }).always(function () {
                abp.ui.clearBusy(_$modal);
            });
        });

        _$modal.on('shown.bs.modal', function () {
            _$modal.find('input:not([type=hidden]):first').focus();
        });

        function refreshUserList() {
            location.reload(true); //reload page to see new user!
        }

        function deleteUser(userId, userName) {
            abp.message.confirm(
                "Delete user '" + userName + "'?",
                function (isConfirmed) {
                    if (isConfirmed) {
                        _userService.delete({
                            id: userId
                        }).done(function () {
                            refreshUserList();
                        });
                    }
                }
            );
        }
    });
})();