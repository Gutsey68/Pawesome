- change the mcd and mld -> notification

Perfect, you want to implement a **booking workflow with secure payment and manual validation**, inspired by platforms like **Airbnb**, **Vinted** or **Blablacar**, in your pet sitting app. Here's a **clear, complete and structured functional workflow** to manage both **requests** and **offers** for pet sitting:

---

## ⚙️ BOOKING WORKFLOW FOR AN ADVERTISEMENT (REQUEST or OFFER)

### 1. 📬 Initial Contact (already existing via messaging)

* A user finds an advertisement or receives one via a search.
* They can ask questions via messaging (optional but recommended).

---

### 2. 📝 Initial Booking

* **Action**: The user clicks on "Book" (whether it's a `request` or an `offer`).
* **Screen**: Summary (dates, animals, price, conditions).
* **Action**: They enter their payment information (via Stripe).
* **Stripe Processing**: Creation of a **pending payment (PaymentIntent with `capture_method=manual`)**.

    * The money is **authorized but not captured**.
    * Stripe keeps the funds **in temporary escrow**.
* **Booking status**: `pending_confirmation`.

---

### 3. ✅ Acceptance by the pet sitter

* The pet sitter is notified (email + in-app notification).
* They can **accept or decline the booking**.

    * 🔁 If they **decline**, the payment is canceled, status changes to `declined`.
    * ✅ If they **accept**, the status becomes `accepted`.

---

### 4. 📆 Sitting Period

* Before the start: automatic reminder + possibility to cancel (according to conditions).
* During: both users can exchange messages via the messaging system.

---

### 5. 🧾 End of the sitting and validation

* At the end of the sitting period:

    * The **pet owner** is invited to **confirm that everything went well**.
    * The user clicks on **"Validate the service"**.
    * 💰 **The payment is captured in Stripe at this point**.
    * Booking status: `completed`.

> ⏳ If the user doesn't confirm after X days (e.g., 2 days), the service is considered **tacitly validated** and the payment is automatically captured.

---

### 6. ⭐ Ratings

* Both parties can **leave a review**.

    * ⚠️ Once one of the two people evaluates, the other has X days to respond.

---

### 7. 💸 Payment to the pet sitter

* Stripe automatically transfers the funds to the pet sitter's connected account (Stripe Connect).
* It's possible to add a **hold of X days** for security reasons.

---

## 🧩 Possible statuses for a booking

| Status    | Description                            |
| --------- | -------------------------------------- |
| `pending` | Waiting for pet sitter validation      |
| `accepted` | Accepted, upcoming                     |
| `declined` | Declined                               |
| `cancelled` | Canceled before the date               |
| `in_progress` | The sitting is ongoing                 |
| `completed` | Sitting completed and validated        |
| `disputed` | Dispute opened                         |
| `expired` | Booking not confirmed in time          |

---

## 📌 Special cases to anticipate

### ❌ Cancellation

* If the cancellation comes:

    * **Before confirmation**: no charges.
    * **After confirmation**: cancellation conditions according to defined policy.
    * Stripe allows to **release the funds** or **refund them partially/totally**.

### ⚠️ Dispute

* The pet owner can report an issue.
* Status = `disputed`.
* You can create an **administration module** to handle cases manually.

---

## 🔐 Security & guarantees

* Stripe Connect with pending payment = fund security.
* No payment until the service is validated.
* The system protects both parties (like Airbnb).

---
