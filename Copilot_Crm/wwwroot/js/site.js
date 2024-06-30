$(document).ready(function () {
    $('#sendButton').click(sendMessage);
    $('#userMessage').keypress(function (event) {
        if (event.which === 13) {
            sendMessage();
        }
    });

    function sendMessage() {
        var userMessage = $('#userMessage').val().trim();
        displayMessage(userMessage, 'user');
        $('#userMessage').val('');

        if (isCustomerDetailsRequest(userMessage)) {
            sendToChatbotFindCustomerDetails(userMessage);
        } else {
            sendToChatbotSendMessage(userMessage);
        }
    }

    function sendToChatbotFindCustomerDetails(userMessage) {
        var customerName = extractCustomerName(userMessage);
        if (!customerName) {
            displayMessage("Le nom du client est manquant dans la requête.", 'bot');
            return;
        }

        var requestData = { SearchFilters: { FirstName: customerName } };

        console.log("Sending request to chatbot for customer details:", requestData);

        $.ajax({
            type: 'POST',
            url: 'api/Customer/FindCustomerDetails',
            contentType: 'application/json',
            data: JSON.stringify(requestData),
            dataType: 'json',
            headers: {
                'Authorization': 'cKVXw7FDXP1pS_Hq8CPkOtru3WPyk1FXEznYM9IhuzNUI8w0zXnVA8kJhOIe7U-a7iQ1N-t_KvzB_du491S1KiC_w6tbwyTVwELZWgfVRRuvYxYNJVKRk23rAkduspYhfJ2k8CEs5wyBOrs2sFKQ2VtQXhuJuNDGRfZZL3vr4h5W44qHiCqnEhsutJZULCPiEXLxtm5tt4ebFFaFhNCIVmGZF0TFXBVd30q5lOH2qi7ZNX0pjE6SMgjyxdtNIZQN-5v8xBhF_kIv0jgXmu0xWGKNfBfCRPieEC984tW6Kna-Fy81EqZd9LtDWXonP69NfjBE33vrbzTCeEbi8Pr1WKgtAyyup5JcYxbiQn9xn73sfMqy'
            },
            success: function (response) {
                console.log("Response received from chatbot:", response);
                if (response.ResponseData && response.ResponseData.length > 0) {
                    var customer = response.ResponseData[0];
                    var botMessage = `Nom: ${customer.FirstName} ${customer.LastName}\nEmail: ${customer.Email}\nTéléphone: ${customer.MobilePhone}`;
                    displayMessage(botMessage, 'bot');
                } else {
                    console.log("No customer details found:", response);
                    displayMessage("Aucun détail de client trouvé.", 'bot');
                }
            },
            error: function (xhr, status, error) {
                console.log("Error retrieving customer details:", xhr.responseText);
                displayMessage("Error retrieving customer details: " + xhr.responseText, 'bot');
            }
        });
    }

    function sendToChatbotSendMessage(userMessage) {
        var requestData = {
            model: "gpt-3.5-turbo-0613",
            messages: [
                { role: "system", content: "CRM solution expert" },
                { role: "user", content: userMessage }
            ]
        };

        console.log("Sending general message to chatbot:", requestData);

        $.ajax({
            type: 'POST',
            url: 'api/Chat/SendMessageToChat',
            contentType: 'application/json',
            data: JSON.stringify(requestData),
            dataType: 'json',
            success: function (response) {
                console.log("Response received from chatbot:", response);
                displayMessage(response.choices[0].message.content, 'bot');
            },
            error: function (xhr, status, error) {
                console.log("Error sending message to chatbot:", xhr.responseText);
                displayMessage("Erreur lors de l'envoi du message au chatbot: " + xhr.responseText, 'bot');
            }
        });
    }

    function isCustomerDetailsRequest(message) {
        var keywords = ['détails du client', 'coordonnées de', 'find customer details', 'customer info', 'customer details', '  customer details of '];
        var lowerCaseMessage = message.toLowerCase();
        return keywords.some(keyword => lowerCaseMessage.includes(keyword.toLowerCase()));
    }

    function extractCustomerName(message) {
        var matches = message.match(/\b(client|customer)\b\s*(\w+)/i);
        return matches ? matches[2] : null;
    }

    function displayMessage(message, sender) {
        var messageElement = $('<li></li>').addClass(sender === 'user' ? 'user-message' : 'bot-message');
        var avatarElement = $('<div></div>').addClass('avatar');
        var messageTextElement = $('<span></span>').text(message);

        messageElement.append(avatarElement).append(messageTextElement);
        $('#messages').append(messageElement);
        $('#messages').scrollTop($('#messages')[0].scrollHeight);
    }
});