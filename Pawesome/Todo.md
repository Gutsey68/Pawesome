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

Voici un prompt clair et structuré à fournir à **GitHub Copilot** (ou à utiliser dans un fichier `README.md` ou `WORKFLOW.md` de ton projet) pour implémenter le workflow de paiement avec Stripe :

---

## 🎯 OBJECTIF : Implémenter un workflow de paiement sécurisé avec Stripe (type Airbnb/Vinted)

Nous utilisons **Stripe avec capture manuelle** (`capture_method=manual`) pour retenir l'argent sans le capturer immédiatement.  
Le paiement est capturé **uniquement après validation du service par l'utilisateur**, ou automatiquement après un délai.

---

### ✅ PRÉREQUIS TECHNIQUES

- ✅ Stripe CLI installé et utilisé pour tester les webhooks localement :
  ```bash
  stripe listen --forward-to localhost:5000/api/stripe/webhook
  stripe trigger payment_intent.succeeded
````

* ✅ Contrôleur `WebhookController` déjà créé pour traiter les événements Stripe :

  * Événements utilisés :

    * `payment_intent.succeeded`
    * `payment_intent.payment_failed`
    * `payment_intent.canceled`
  * Signature du webhook validée avec `whsec_...`.

* ✅ Paiement initié via `PaymentIntent` avec `capture_method: manual` (pas Checkout direct).

---

## ⚙️ WORKFLOW DE BOOKING

### 1. 📬 Premier contact (déjà en place)

* Les utilisateurs échangent via messagerie interne avant de réserver.

---

### 2. 📝 Réservation initiale

* L’utilisateur clique sur “Réserver”.
* Il accède à un écran récapitulatif (dates, animaux, montant...).
* Stripe crée un `PaymentIntent` avec `capture_method = manual`.
* 🏦 L’argent est **autorisé mais pas capturé**.
* Booking enregistré avec statut `pending_confirmation`.

---

### 3. ✅ Acceptation ou refus du pet sitter

* Le pet sitter reçoit une notification.
* Il peut **accepter** ou **refuser** :

  * Si refus : Stripe annule le `PaymentIntent` → `booking.status = declined`.
  * Si accepté : `booking.status = accepted`.

---

### 4. 📆 Période de garde

* Avant : rappel automatique.
* Pendant : communication via messagerie.

---

### 5. 🧾 Fin de la garde et validation

* À la fin, le client clique sur **"Valider le service"** :

  * Appelle `CapturePaymentAsync(paymentIntentId)`
  * Stripe capture le paiement.
  * `booking.status = completed`

* ⏳ Si aucune action dans X jours → capture automatique via job.

---

### 6. ⭐ Avis

* Les deux parties peuvent s’évaluer.
* ⚠️ Une seule réponse possible après un avis initial.

---

### 7. 💸 Paiement du pet sitter

* (À venir) Utilisation de Stripe Connect pour verser les fonds au pet sitter.
* Possibilité d’ajouter un délai de sécurité (3 jours par ex.).

---

## 🔄 STATUTS POSSIBLES POUR UN BOOKING

| Status        | Description                        |
| ------------- | ---------------------------------- |
| `pending`     | En attente de validation du sitter |
| `accepted`    | Accepté, à venir                   |
| `declined`    | Refusé                             |
| `cancelled`   | Annulé avant la date               |
| `in_progress` | En cours                           |
| `completed`   | Terminé et validé                  |
| `disputed`    | Litige en cours                    |
| `expired`     | Réservation non confirmée à temps  |

---

## 🔐 Sécurité & cas particuliers

### ❌ Annulation

* Avant confirmation : aucune charge.
* Après confirmation : politique d’annulation → `CancelPaymentAuthorizationAsync()`.

### ⚠️ Litige

* Signalement possible → `booking.status = disputed`.
* Traitement via back-office admin (à venir).

---

👉 Implémenter les points suivants :

* Créer un `PaymentIntent` avec autorisation uniquement.
* Stocker `payment_intent_id` dans la DB.
* Ajouter un bouton "Valider la garde" qui capture le paiement.
* Créer un job ou cron pour capture automatique.
* Implémenter l'annulation et le litige.

---

TODO: 
- sur advert/details il faut dire que l'autre doit accepter.
- les notifications ne marchent pas sur les annonces 
- celui qui a fait une annonce doit voir les demandes. -> faire un lien sur Booking/Index
- il faut ajouter la searchbar sur modale quand pas sur home ou dashboard
- bug de prix sur Booking/Create 