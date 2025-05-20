
## ✅ User Stories

1. En tant qu'utilisateur, je souhaite pouvoir répondre / réserver à une annonce de pet sitting pour pouvoir garder un animal.
2. En tant qu'utilisateur, je souhaite payer le petsitter en laissant l'argent en suspens tant que ce n'est pas validé pour une question de sécurité.
3. En tant qu'utilisateur, je souhaite pouvoir valider la prestation pour confirmer qu'elle a bien eu lieu.
4. En tant qu'utilisateur, je souhaite signaler une annonce parce qu'elle a un contenu inapproprié.
5. En tant qu'utilisateur, je souhaite signaler un utilisateur parce qu'il(elle) a un contenu inapproprié.
6. En tant qu'utilisateur, je souhaite recevoir des notifications pour pouvoir être au courant du statut de l'annonce.
7. En tant qu'utilisateur, je souhaite avoir accès à mes réservations passées ou en cours pour savoir avec qui j'ai déjà travaillé et quand est-ce que je l'ai fait.
8. En tant qu'utilisateur, je souhaite pouvoir demander à un pet sitter récent de refaire un pet sitting parce que ça s'était bien passé la première fois.
9. En tant qu'utilisateur, je souhaite voir la liste des factures que j'ai reçues ou envoyées pour me permettre d'avoir un suivi des dépenses.
10. En tant qu'utilisateur, je souhaite générer un PDF de ma facture pour le présenter à ma banque, pour me permettre d'avoir un relevé bancaire de la transaction.

---

## 🔧 Tâches techniques (TODO)

- [ ] Ajouter une erreur `CreateRequest` si aucun animal n'est sélectionné → afficher un message d'erreur.
- [ ] Ajouter dans la vue **Profile** un affichage vers **l'annonce** + lien vers le **détail de son annonce**.
- [ ] Modifier le **MLD** : ajouter/modifier les tables `report`, `payment`.
- [ ] Modifier le **MCD** : établir une relation entre `advert` et `animal_type`.
- [ ] Ajouter les **webhooks Stripe** pour la gestion des paiements.
- [ ] Mapper les informations de **Google** lors du `register`.
- [ ] Enlever le **`<h1>` du footer** (→ vérifier la justification).
- [ ] Lors d’un changement d’informations utilisateur → **mettre à jour les claims JWT**.


---

## 🛠️ Nouvelles User Stories détaillées et tâches associées

### 11. En tant qu'utilisateur, je souhaite que la barre de recherche fonctionne correctement
- Objectif : finaliser cette fonctionnalité dans un délai de 3 semaines.
- Si un filtre ne fonctionne pas correctement, il sera retiré temporairement du système.
- Implémenter une user story spécifique à ce besoin pour suivre l'évolution dans le backlog.

### 12. En tant qu'utilisateur, je souhaite pouvoir modifier mon profil facilement
- Vérification du numéro de téléphone : ✅ déjà implémentée.
- Ajouter un contrôle de format (ex : regex) avec message d’erreur clair.
- Gérer l’upload et la mise à jour de la photo de profil dans le backend.
- Affichage de la petite photo dans le coin supérieur droit de l’interface.
- Développement de la vue front pour modifier le profil utilisateur + effet visuel dans les autres composants concernés.

### 13. En tant qu'utilisateur, je souhaite pouvoir envoyer et recevoir des messages en temps réel
- Implémentation de SignalR dans .NET pour le temps réel (bug léger à corriger).
- Ajouter un bouton dans chaque annonce pour démarrer une conversation.
- Vérifier les performances sous charge : optimisation déjà en place.
- Le système ouvre un hub + communication AJAX avec l'API.

### 14. En tant qu'utilisateur, je souhaite que l’actualisation de mes données fonctionne correctement
- Un bug empêche actuellement le bon fonctionnement du rafraîchissement.
- Créer une tâche pour corriger ce bug dans le sprint suivant.

### 15. En tant qu'utilisateur, je souhaite pouvoir créer une annonce simplement
- Ajouter un champ “date du jour” automatiquement à la création d’annonce.
- Vérifier la faisabilité d’un calendrier pour sélectionner les dates.
- Ajouter un lien vers un espace personnel regroupant toutes mes annonces passées/actives.

### 16. En tant qu’utilisateur, je souhaite pouvoir annuler une annonce sans la supprimer
- Si j’annule une annonce, elle reste dans le système avec un statut "annulée".
- Permet à l’admin de garder un historique de toutes les actions.

### 17. En tant que pet sitter, je souhaite pouvoir postuler à une annonce
- En tant que propriétaire, je souhaite pouvoir choisir un pet sitter parmi ceux ayant postulé.

### 18. En tant qu’utilisateur, je souhaite que l’argent reste en suspens jusqu’à validation
- L’argent payé par le propriétaire est bloqué tant que la prestation n’est pas validée.
- À implémenter pour le sprint 3 (via Stripe connect, escrow-like system).

--changer le mcd et mld -> notification