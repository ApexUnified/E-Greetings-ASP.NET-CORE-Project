

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

                    *{
                        margin:0px;
                        padding:0px;
                    }


                  body {
                background: linear-gradient(135deg, #6c5ce7, #a367fc);
                min-height: 100vh;
                font-family: 'Poppins', sans-serif;
                padding: 20px;
            }

           .birthday-card {
                max-width: 900px;
                margin: 2rem auto;
                background: #fff;
                border-radius: 30px;
                overflow: hidden;
                box-shadow: 0 20px 60px rgba(0, 0, 0, 0.2);
            }

            .preview-section {
                background: linear-gradient(45deg, #6c5ce7, #a367fc);
                padding: 3rem 2rem;
                position: relative;
                color: white;
                overflow: hidden;
            }

            .balloon {
                position: absolute;
                animation: float 6s ease-in-out infinite;
            }

             @keyframes float {
                0%, 100% { transform: translateY(0); }
                50% { transform: translateY(-20px); }
            }

            .celebration-text {
                font-size: 3.5rem;
                font-weight: 800;
                color: #fff;
                text-shadow: 3px 3px 6px rgba(0, 0, 0, 0.2);
                margin: 1rem 0;
                position: relative;
                z-index: 2;
            }

            .birthday-details {
                background: rgba(255, 255, 255, 0.1);
                backdrop-filter: blur(10px);
                border-radius: 20px;
                padding: 2rem;
                margin: 2rem 0;
                border: 1px solid rgba(255, 255, 255, 0.2);
                position: relative;
                z-index: 2;
            }




            .message-text {
                font-size: 1.2rem;
                line-height: 1.6;
                margin: 1.5rem 0;
                color: rgba(255, 255, 255, 0.9);
            }

            .emoji {
                font-size: 2rem;
                margin: 0 5px;
                animation: bounce 2s infinite;
            }

            @keyframes bounce {
                        0%, 100% { transform: translateY(0); }
                        50% { transform: translateY(-10px); }
                    }

                    .theme-select {
                        background: rgba(255, 255, 255, 0.1);
                        border: 1px solid rgba(255, 255, 255, 0.2);
                        border-radius: 15px;
                        color: white;
                        padding: 0.5rem 1rem;
                    }

                    .theme-select option {
                        background: #6c5ce7;
                        color: white;
                    }

                    @media (max-width: 768px) {
                        .birthday-card {
                            margin: 1rem;
                        }

                        .celebration-text {
                            font-size: 2.5rem;
                        }

                        .preview-section {
                            padding: 2rem 1rem;
                        }
                    }

                    </style>
                </head>
                <body>
                     ${document.getElementById("birthday_card").outerHTML}
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
        url: "/Birthday/SendEmail",
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