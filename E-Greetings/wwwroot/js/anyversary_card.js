

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
                            background: linear-gradient(135deg, #2c3e50, #3498db);
                        }

                        .anniversary-card {
                            margin: 2rem auto;
                            background: #fff;
                            border-radius: 30px;
                            overflow: hidden;
                            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.2);
                        }



                        .preview-section {
                            background: linear-gradient(45deg, #2c3e50, #3498db);
                            padding: 3rem 2rem;
                            position: relative;
                            color: white;
                            overflow: hidden;
                        }

                        .heart {
                            position: absolute;
                            opacity: 0.1;
                            font-size: 10px;
                            color: white;
                            animation: float 6s ease-in-out infinite;
                        }

                        .anniversary-heading {
                            font-family: 'Playfair Display', serif;
                            font-size: 3.5rem;
                            font-weight: 700;
                            color: #fff;
                            text-shadow: 3px 3px 6px rgba(0, 0, 0, 0.2);
                            margin: 1rem 0;
                            position: relative;
                            z-index: 2;
                        }

                        .details-box {
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
                            font-size: 1.4rem;
                            line-height: 1.8;
                            margin: 1.5rem 0;
                            color: rgba(255, 255, 255, 0.9);
                            font-style: italic;
                        }

                        .years-together {
                            font-size: 2.5rem;
                            font-weight: 700;
                            color: #ffd700;
                            text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
                            margin: 1rem 0;
                        }
s
                        .signature {
                            font-family: 'Playfair Display', serif;
                            font-style: italic;
                            font-size: 1.3rem;
                            margin-top: 1rem;
                            color: rgba(255, 255, 255, 0.9);
                        }

                       @media (max-width: 768px) {
                            .anniversary-card

                        {
                            margin: 1rem;
                        }

                        .anniversary-heading {
                            font-size: 2.5rem;
                        }

                        .preview-section {
                            padding: 2rem 1rem;
                        }

                        }
                   </style>
                </head>
                <body>
                     ${document.getElementById("cardPreview").outerHTML}
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
        url: "/Anyversary/SendEmail",
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