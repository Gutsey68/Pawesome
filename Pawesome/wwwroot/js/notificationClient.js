/**
 * @fileoverview Client-side notification handler using SignalR
 * This module manages real-time notifications, displays them to users,
 * and maintains the notification counter in the UI.
 */

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .build();

/**
 * Event handler for receiving notifications from the server
 * @param {Object} notification - The notification object from the server
 */
connection.on("ReceiveNotification", (notification) => {
    showNotification(notification);
    updateNotificationCount();
});

/**
 * Starts the SignalR connection and implements reconnection logic
 * @async
 * @returns {Promise<void>}
 */
async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR Connected");
    } catch (err) {
        console.log(err);
        setTimeout(startConnection, 5000);
    }
}

/**
 * Creates and displays a notification toast
 * @param {Object} notification - The notification to display
 * @param {string} notification.title - Notification title
 * @param {string} notification.message - Notification message
 * @param {string} notification.type - Notification type
 * @param {string} [notification.linkUrl] - Optional URL for details
 */
function showNotification(notification) {
    const toast = document.createElement('div');
    toast.classList.add('notification-toast');
    toast.classList.add(`notification-type-${notification.type.toLowerCase()}`);

    toast.innerHTML = `
        <div class="notification-header">
            <h3>${notification.title}</h3>
            <span class="notification-close">&times;</span>
        </div>
        <div class="notification-body">
            <p>${notification.message}</p>
        </div>
        ${notification.linkUrl ? `<a href="${notification.linkUrl}" class="notification-link">View details</a>` : ''}
    `;

    document.getElementById('notification-container').appendChild(toast);

    setTimeout(() => {
        toast.classList.add('fade-out');
        setTimeout(() => toast.remove(), 500);
    }, 5000);
}

/**
 * Updates the notification counter badge with the current unread count
 * @returns {void}
 */
function updateNotificationCount() {
    fetch('/Notification/GetUnreadCount')
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP Error: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            const badge = document.getElementById('notification-badge');
            if (badge) {
                if (data.count > 0) {
                    badge.innerText = data.count;
                    badge.classList.remove('hidden');
                } else {
                    badge.classList.add('hidden');
                }
            }
        })
        .catch(err => console.error("Error retrieving notification count:", err));
}

startConnection();
