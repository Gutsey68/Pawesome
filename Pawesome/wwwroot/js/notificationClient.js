/**
 * @fileoverview Client-side notification handler using SignalR
 * This module manages real-time notifications, displays them to users,
 * and maintains the notification counter in the interface.
 */

const notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .build();

const messageConnection = new signalR.HubConnectionBuilder()
    .withUrl("/messageHub")
    .withAutomaticReconnect()
    .build();

/**
 * Event for receiving notifications from the server
 * @param {Object} notification - The notification object sent by the server
 */
notificationConnection.on("ReceiveNotification", (notification) => {
    showNotification(notification);
    updateNotificationCount();
});

/**
 * Event for updating the notification counter
 * Listens on the notification hub connection
 */
notificationConnection.on("UpdateNotifications", () => {
    updateNotificationCount();
});

/**
 * Event for updating the notification counter
 * Listens on the message hub connection
 */
messageConnection.on("UpdateNotifications", () => {
    updateNotificationCount();
});

/**
 * Starts the SignalR connection to the notification hub and implements reconnection logic
 * @async
 * @returns {Promise<void>}
 */
async function startNotificationConnection() {
    try {
        await notificationConnection.start();
        updateNotificationCount();
    } catch (err) {
        setTimeout(startNotificationConnection, 5000);
    }
}

/**
 * Starts the SignalR connection to the message hub and implements reconnection logic
 * @async
 * @returns {Promise<void>}
 */
async function startMessageConnection() {
    try {
        await messageConnection.start();
    } catch (err) {
        setTimeout(startMessageConnection, 5000);
    }
}

/**
 * Handles reconnection events for the notification hub
 */
notificationConnection.onreconnected(() => {
    updateNotificationCount();
});

/**
 * Handles reconnection events for the message hub
 */
messageConnection.onreconnected(() => {
    updateNotificationCount();
});

/**
 * Creates and displays a toast notification
 * @param {Object} notification - The notification to display
 * @param {string} notification.title - Notification title
 * @param {string} notification.message - Notification message
 * @param {string} notification.type - Notification type
 * @param {string} [notification.linkUrl] - Optional URL for more details
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

    const container = document.getElementById('notification-container');
    if (container) {
        container.appendChild(toast);

        const closeButton = toast.querySelector('.notification-close');
        if (closeButton) {
            closeButton.addEventListener('click', () => {
                toast.classList.add('fade-out');
                setTimeout(() => toast.remove(), 500);
            });
        }

        setTimeout(() => {
            toast.classList.add('fade-out');
            setTimeout(() => toast.remove(), 500);
        }, 5000);
    }
}

/**
 * Updates the notification badge with the current number of unread notifications
 * @returns {void}
 */
function updateNotificationCount() {
    fetch('/Notification/GetUnreadCount')
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error: ${response.status}`);
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
        .catch(err => {});
}
document.addEventListener('DOMContentLoaded', () => {
    startNotificationConnection();
    startMessageConnection();
});
