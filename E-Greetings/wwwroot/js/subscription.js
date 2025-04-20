document.getElementById("subscribe_button").addEventListener("click", function () {

    console.log("Subscribed");

    $.ajax({
        url: "/auth/check",
        type: 'POST',
        success: function (response) {
            if (response.status) {
                console.log("Its In If");

                $.ajax({
                    url: "/Subscription/save",
                    type: 'POST',
                    success: function (response) {
                        if (response.status) {
                            Swal.fire({
                                title: "Thanks For The Subscription",
                                text: `${response.message}`,
                                icon: "success"
                            });
                            $("#subscribeModal").modal("hide");

                        } else {
                            Swal.fire({
                                title: "Error Occured While Making Subscription",
                                text: `${response.message}`,
                                icon: "error"
                            });

                        }
                    },
                    error: function (xhr, status, error) {
                        Swal.fire({
                            title: "Error Occured While Making Subscription",
                            text: `${error}`,
                            icon: "error"
                        });
                    }
                });
               
            } else {
                console.log("its in Else");
                $("#subscribeModal").modal("hide");
                $("#unauthenticated_user_model").modal("show");

                $("#subscribe_button_unauthenticated").on("click", () => {

                    var sub_email = $("#subscribe_email").val();

                    if (sub_email == null || sub_email == "") {
                        Swal.fire({
                            title: "Error",
                            text: "Email Cannot Be Null Please Provide Valid Email",
                            icon: "error"
                        });
                        return;
                    }

                    $.ajax({
                        url: "/Subscription/save",
                        type: 'POST',
                        data: { email: sub_email },
                        success: function (response) {
                            if (response.status) {
                                Swal.fire({
                                    title: "Thanks For The Subscription",
                                    text: `${response.message}`,
                                    icon: "success"
                                });
                                $("#unauthenticated_user_model").modal("hide");

                            } else {
                                Swal.fire({
                                    title: "Error Occured While Making Subscription",
                                    text: `${response.message}`,
                                    icon: "error"
                                });

                            }
                           
                        },
                        error: function (xhr, status, error) {
                            Swal.fire({
                                title: "Error Occured While Making Subscription",
                                text: `${error}`,
                                icon: "error"
                            });
                        }
                    });

                });


            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: "Error Occured While Making Request",
                text: `${error}`,
                icon: "error"
            });
        }
    });



  

});