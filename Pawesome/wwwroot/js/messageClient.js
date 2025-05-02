/**
 * @fileoverview Real-time messaging client module using SignalR for the Pawesome application
 * @requires signalR
 */

"use strict";

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
        .then(() => console.log("SignalR connection established"))
        .catch(err => console.error("SignalR connection error:", err));

    /**
     * Updates the unread messages counter in the UI
     * Fetches the count from the server and updates the badge display
     */
    function updateUnreadCountBadge() {
        fetch('/messages/api/unread-count')
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
            .catch(err => console.error("Error retrieving unread messages count:", err));
    }

    /**
     * Handler for receiving new messages
     * @param {Object} message - The message object received from the server
     * @param {number} message.senderId - ID of the user who sent the message
     * @param {string} message.content - Content of the message
     * @param {string} message.createdAt - Timestamp when the message was created
     * @param {string} message.senderFullName - Full name of the sender
     */
    connection.on("ReceiveMessage", function (message) {
        const conversationId = document.querySelector('[data-conversation-id]')?.dataset.conversationId;

        if (conversationId && parseInt(conversationId) === message.senderId) {
            const messagesContainer = document.getElementById('messages-container');
            if (messagesContainer) {
                const messageDiv = document.createElement('div');
                messageDiv.innerHTML = `
                    <div>
                        <p>${message.content}</p>
                        <small>${new Date(message.createdAt).toLocaleString()}</small>
                    </div>
                `;
                messagesContainer.appendChild(messageDiv);
                messagesContainer.scrollTop = messagesContainer.scrollHeight;

                fetch(`/messages/api/mark-read/${message.senderId}`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' }
                });
            }
        } else {
            updateUnreadCountBadge();

            if (Notification.permission === "granted") {
                new Notification("New message from " + message.senderFullName, { body: message.content });
            } else if (Notification.permission !== "denied") {
                Notification.requestPermission().then(permission => {
                    if (permission === "granted") {
                        new Notification("New message from " + message.senderFullName, { body: message.content });
                    }
                });
            }
        }
    });

    /**
     * Handler for sent message confirmation
     * Updates the UI to show the sent message and resets the input field
     * @param {Object} message - The sent message object from the server
     * @param {string} message.content - Content of the sent message
     * @param {string} message.createdAt - Timestamp when the message was created
     */
    connection.on("MessageSent", function (message) {
        const messagesContainer = document.getElementById('messages-container');
        if (messagesContainer) {
            const messageDiv = document.createElement('div');
            messageDiv.innerHTML = `
                <div>
                    <p>${message.content}</p>
                    <small>${new Date(message.createdAt).toLocaleString()}</small>
                </div>
            `;
            messagesContainer.appendChild(messageDiv);
            messagesContainer.scrollTop = messagesContainer.scrollHeight;

            document.getElementById('message-input').value = '';
            document.getElementById('sending-indicator').style.display = 'none';
        }
    });

    /**
     * Handler for read message notifications
     * Updates message status indicators in the UI
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
         * Handle message form submission
         * Sends the message to the API endpoint
         * @param {Event} e - Form submission event
         */
        messageForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const messageInput = document.getElementById('message-input');
            const receiverId = document.getElementById('receiver-id').value;

            if (!messageInput.value.trim()) {
                return;
            }

            const sendingIndicator = document.getElementById('sending-indicator');
            if (sendingIndicator) {
                sendingIndicator.style.display = 'inline';
            }

            fetch('/messages/api/send', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    content: messageInput.value,
                    receiverId: parseInt(receiverId)
                })
            });
        });
    }

    updateUnreadCountBadge();

    const conversationId = document.querySelector('[data-conversation-id]')?.dataset.conversationId;
    if (conversationId) {
        fetch(`/messages/api/mark-read/${conversationId}`, {
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