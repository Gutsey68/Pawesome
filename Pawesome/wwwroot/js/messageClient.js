/**
 * @fileoverview Real-time messaging client module using SignalR for the Pawesome application
 * @requires signalR
 */

"use strict";

/**
 * Formats a date according to the backend format (dd/MM/yyyy HH:mm)
 * @param {string|Date} dateTime - The date to format
 * @returns {string} - The formatted date
 */
function formatDate(dateTime) {
    const date = new Date(dateTime);

    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');

    return `${day}/${month}/${year} ${hours}:${minutes}`;
}

/**
 * Gets the correct URL for a user image
 * @param {string|null} photoPath - The image path
 * @returns {string} - The formatted image URL
 */
function getPhotoUrl(photoPath) {
    if (!photoPath) {
        return "/images/placeholder-avatar.png";
    }

    if (photoPath.includes("external:")) {
        return photoPath.substring(photoPath.indexOf("external:") + 9);
    }

    if (photoPath.startsWith("http", 0)) {
        return photoPath;
    }

    if (photoPath.startsWith("/images/users/", 0)) {
        return photoPath;
    }

    return `/images/users/${photoPath}`;
}

document.addEventListener('DOMContentLoaded', function () {
    /**
     * SignalR connection instance
     * @type {signalR.HubConnection}
     */
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/messageHub")
        .withAutomaticReconnect()
        .build();

    /**
     * Initializes the SignalR connection
     * @returns {Promise} Promise that resolves when the connection is established
     */
    connection.start()
        .then(() => console.log("Connexion SignalR établie"))
        .catch(err => console.error("Erreur de connexion SignalR :", err));

    /**
     * Updates the unread messages counter in the UI.
     * Fetches the count from the server and updates the badge display.
     */
    function updateUnreadCountBadge() {
        fetch('/api/unread-count')
            .then(response => response.json())
            .then(data => {
                const badge = document.getElementById('unread-messages-badge');
                if (badge) {
                    if (data.count > 0) {
                        badge.textContent = data.count;
                        badge.style.display = 'inline-block';
                    } else {
                        badge.style.display = 'none';
                    }
                }
            })
            .catch(err => console.error("Erreur lors de la récupération du nombre de messages non lus :", err));
    }

    /**
     * Handler for receiving new messages.
     * @param {Object} message - The message object received from the server
     */
    connection.on("ReceiveMessage", function (message) {
        const conversationId = document.querySelector('[data-conversation-id]')?.dataset.conversationId;

        if (conversationId && parseInt(conversationId) === message.senderId) {
            const messagesContainer = document.getElementById('messages-container');
            if (messagesContainer) {
                const isEmpty = messagesContainer.querySelector('.empty-conversation');
                if (isEmpty) {
                    messagesContainer.innerHTML = '';
                }

                const messageDiv = document.createElement('div');
                messageDiv.className = 'message-wrapper message-incoming';
                messageDiv.innerHTML = `
                    <div class="message">
                        <p class="message-content">${message.content}</p>
                        <p class="message-time">${formatDate(message.createdAt)}</p>
                    </div>
                `;
                messagesContainer.appendChild(messageDiv);
                messagesContainer.scrollTop = messagesContainer.scrollHeight;

                fetch(`/api/mark-read/${message.senderId}`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' }
                });
            }
        } else {
            updateUnreadCountBadge();

            if (Notification.permission === "granted") {
                const options = {
                    body: message.content,
                    icon: message.senderPhoto ? getPhotoUrl(message.senderPhoto) : null
                };
                new Notification("Nouveau message de " + message.senderFullName, options);
            } else if (Notification.permission !== "denied") {
                Notification.requestPermission().then(permission => {
                    if (permission === "granted") {
                        const options = {
                            body: message.content,
                            icon: message.senderPhoto ? getPhotoUrl(message.senderPhoto) : null
                        };
                        new Notification("Nouveau message de " + message.senderFullName, options);
                    }
                });
            }
        }
    });

    /**
     * Handler for sent message confirmation.
     * Updates the UI to show the sent message and resets the input field.
     * @param {Object} message - The sent message object from the server
     */
    connection.on("MessageSent", function (message) {
        const messagesContainer = document.getElementById('messages-container');
        const sendButton = document.getElementById('send-message-btn');

        if (sendButton) {
            sendButton.classList.remove('is-loading');
        }

        if (messagesContainer) {
            const isEmpty = messagesContainer.querySelector('.empty-conversation');
            if (isEmpty) {
                messagesContainer.innerHTML = '';
            }

            const messageDiv = document.createElement('div');
            messageDiv.className = 'message-wrapper message-outgoing';
            messageDiv.innerHTML = `
                <div class="message">
                    <p class="message-content">${message.content}</p>
                    <p class="message-time">${formatDate(message.createdAt)}</p>
                </div>
            `;
            messagesContainer.appendChild(messageDiv);
            messagesContainer.scrollTop = messagesContainer.scrollHeight;

            document.getElementById('message-input').value = '';
        }
    });

    /**
     * Handler for read message notifications.
     * Updates message status indicators in the UI.
     * @param {number} userId - ID of the user who read the messages
     */
    connection.on("MessagesRead", function (userId) {
        const messageStatuses = document.querySelectorAll(`.message-status[data-sender="${userId}"]`);
        messageStatuses.forEach(status => {
            status.innerHTML = '✓';
        });
    });

    const messageForm = document.getElementById('message-form');
    if (messageForm) {
        /**
         * Handles message form submission.
         * Sends the message to the API endpoint.
         * @param {Event} e - Form submission event
         */
        messageForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const messageInput = document.getElementById('message-input');
            const receiverId = document.getElementById('receiver-id').value;
            const sendButton = document.getElementById('send-message-btn');

            if (!messageInput.value.trim()) {
                return;
            }

            sendButton.classList.add('is-loading');

            fetch('/api/send', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    content: messageInput.value,
                    receiverId: parseInt(receiverId)
                })
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error("Erreur lors de l'envoi du message");
                    }
                    return response.json();
                })
                .catch(error => {
                    console.error("Erreur :", error);
                    
                    sendButton.classList.remove('is-loading');
                    alert("Le message n'a pas pu être envoyé. Veuillez réessayer.");
                });
        });
    }

    updateUnreadCountBadge();

    const conversationId = document.querySelector('[data-conversation-id]')?.dataset.conversationId;
    if (conversationId) {
        fetch(`/api/mark-read/${conversationId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const messagesContainer = document.getElementById('messages-container');
        if (messagesContainer) {
            messagesContainer.scrollTop = messagesContainer.scrollHeight;
        }
    }
});