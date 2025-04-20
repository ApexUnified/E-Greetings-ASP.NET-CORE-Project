

    document.getElementById("addEmailButton").addEventListener("click", function () {
        const emailInput = document.getElementById("emailInput");
        const emailSelect = document.getElementById("emailSelect");
        const emailError = document.getElementById("emailError");

        const emailValue = emailInput.value.trim();

        if (emailValue === "") {
            emailError.textContent = "Email cannot be empty.";
            emailError.style.display = "block";
            return;
        }

        // Server-side compatible email validation using JavaScript
        const emailPattern = new RegExp(
            "^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$"
        );

        if (!emailPattern.test(emailValue)) {
            emailError.textContent = "Invalid email format.";
            emailError.style.display = "block";
            return;
        }

        // Check if email already exists in the list
        const existingOptions = Array.from(emailSelect.options).map(option => option.value);
        if (existingOptions.includes(emailValue)) {
            emailError.textContent = "This email is already added.";
            emailError.style.display = "block";
            return;
        }

        // Hide error and add email to the select list
        emailError.style.display = "none";

        const newOption = document.createElement("option");
        newOption.value = emailValue;
        newOption.textContent = emailValue;
        emailSelect.appendChild(newOption);

        // Clear input and close the modal
        emailInput.value = "";
        const modal = bootstrap.Modal.getInstance(document.getElementById("emailModal"));
        modal.hide();
    });





document.getElementById("sendmail").addEventListener("click", function () {
    document.querySelectorAll("script[src*='aspnetcore-browser-refresh.js']").forEach((script) => script.remove());
    console.log("Mail Send Trigger");

    const card_id = document.getElementById("card_id").value;
    const selectedEmails = $("#emailSelect").val();


    let cardHtml = `
            <html>
                <head>
                         <style>
                          body {
                        background: linear-gradient(135deg, #f8f9fa, #e9ecef);

                    }

                    .wedding-card {
                        margin: 2rem auto;
                        background: white;
                        border-radius: 30px;
                        overflow: hidden;
                        box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
                    }

                    .preview-section {
                        background: linear-gradient(45deg, #fff1f1, #fff8f8);
                        padding: 3rem 2rem;
                        position: relative;
                    }

                    .floral-border {
                        position: absolute;
                        opacity: 0.1;
                        background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'%3E%3Cpath fill='%23ff9999' d='M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z'/%3E%3C/svg%3E");
                        background-size: contain;
                    }

                    .top-left {
                        top: 0;
                        left: 0;
                        transform: rotate(45deg);
                    }

                    .top-right {
                        top: 0;
                        right: 0;
                        transform: rotate(135deg);
                    }

                    .bottom-left {
                        bottom: 0;
                        left: 0;
                        transform: rotate(-45deg);
                    }

                    .bottom-right {
                        bottom: 0;
                        right: 0;
                        transform: rotate(-135deg);
                    }

                    .couple-name {
                        font-family: 'Great Vibes', cursive;
                        font-size: 4rem;
                        color: #ff6b6b;
                        margin: 1rem 0;
                        text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.1);
                    }

                    .invitation-text {
                        font-family: 'Montserrat', sans-serif;
                        font-weight: 300;
                        letter-spacing: 2px;
                        color: #666;
                    }

                    .date-venue {
                        background: white;
                        border-radius: 20px;
                        padding: 2rem;
                        margin: 2rem 0;
                        box-shadow: 0 10px 30px rgba(0, 0, 0, 0.05);
                    }

                        .date-venue h4 {
                            font-family: 'Montserrat', sans-serif;
                            font-weight: 600;
                            color: #444;
                            margin-bottom: 1rem;
                        }



                    @media (max-width: 768px) {
                        .wedding-card

                    {
                        margin: 1rem;
                    }

                    .couple-name {
                        font-size: 3rem;
                    }

                    .preview-section {
                        padding: 2rem 1rem;
                    }

                    }


                         </style>
                </head>
                <body>
                     ${document.getElementById("card_section").outerHTML}
                </body>
            </html>`;


    cardHtml = cardHtml.replace(/<script src="\/_framework\/aspnetcore-browser-refresh\.js"><\/script>/g, "");
    console.log(cardHtml);
    if (selectedEmails == "") {
        alert("Email Should Be Not Empty");
        return;
    }

    Swal.fire({
        position: "mid-center",
        icon: "success",
        title: "Sending Emails Please Wait ",
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading(); // Show loading spinner
        }
    });

    $.ajax({
        url: "/Wedding/SendEmail",
        type: "POST",
        data: { cardHtml: cardHtml, card_id: card_id, emails: selectedEmails },
        success: function (response) {
            if (response.status) {
                Swal.close();

                Swal.fire({
                    title: "Emails Has Been Sent Succesfully",
                    text: `${response.message}`,
                    icon: "success"
                });
            } else {
                Swal.close();

                Swal.fire({
                    title: "Error Occured While Sending Emails",
                    text: `${response.message}`,
                    icon: "error"
                });
            }
            

        },
        error: function (xhr, status, error) {
            Swal.close();

            Swal.fire({
                title: "Error Occured While Sending Emails",
                text: `${error}`,
                icon: "error"
            });
        }
    });
});