### **Users**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|id|Int||PK Auto increment|Identifiant unique de l'utilisateur|
|lastName|Varchar|255|Not null|Nom de l'utilisateur|
|firstName|Varchar|255|Not null|Prénom de l'utilisateur|
|email|Varchar|255|Not null, Unique|Adresse email de l'utilisateur|
|bio|Text|||Biographie de l'utilisateur|
|password|Varchar|255|Not null|Mot de passe hashé|
|photo|Varchar|255||URL de la photo de profil|
|rating|Float|||Note moyenne de l'utilisateur|
|role|Varchar|255||Rôle de l'utilisateur|
|status|Varchar|255||Statut de l'utilisateur|
|isVerified|Boolean||Default false|Vérification du compte|
|balanceAccount|Decimal(10,2)||Default 0|Solde du compte utilisateur|
|onboardingStep|Int||Default 1|Étape d'onboarding en cours|
|isOnboardingCompleted|Boolean||Default false|Indique si l'onboarding est terminé|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|
|roleId|Int||FK -> Roles(id)|Référence au rôle|
|adressId|Int||FK -> Adresses(id)|Référence à l'adresse|

### **Pets**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|id|Int||PK Auto increment|Identifiant unique de l'animal|
|name|Varchar|255|Not null|Nom de l'animal|
|breed|Varchar|255||Race de l'animal|
|age|Int|||Âge de l'animal|
|photo|Varchar|255||URL de la photo de l'animal|
|info|Text|||Informations complémentaires|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|
|userId|Int||FK -> Users(id)|Propriétaire de l'animal|
|animalTypesId|Int||FK -> AnimalTypes(id)|Type d'animal|

### **AnimalTypes**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|id|Int||PK Auto increment|Identifiant unique|
|name|Varchar|255|Not null|Type d'animal (ex: chien, chat)|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|

### **Adverts**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|id|Int||PK Auto increment|Identifiant unique|
|startDate|DateTime||Not null|Date de début|
|endDate|DateTime||Not null|Date de fin|
|status|Varchar|255|Default 'pending'|Statut de l'annonce|
|amount|Decimal(10,2)||Not null|Montant de l'annonce|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|

### **Notifications**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|id|Int||PK Auto increment|Identifiant unique|
|comment|Text|||Commentaire|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|
|userId|Int||FK -> Users(id)|Référence à l'utilisateur|

### **Reports**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|id|Int||PK Auto increment|Identifiant unique|
|comment|Text|||Commentaire du signalement|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|
|userId|Int||FK -> Users(id)|Référence à l'utilisateur|

### **PasswordResets**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|id|Int||PK Auto increment|Identifiant unique|
|token|Varchar|255|Not null, Unique|Jeton de réinitialisation|
|isValid|Boolean||Default true|Indique si le jeton est valide|
|expiresAt|DateTime||Not null|Date d'expiration|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|
|userId|Int||FK -> Users(id)|Référence à l'utilisateur|

### **Roles**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|id|Int||PK Auto increment|Identifiant unique|
|name|Varchar|255|Not null, Unique|Nom du rôle|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|

### **Messages**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|id|Int||PK Auto increment|Identifiant unique|
|content|Text||Not null|Contenu du message|
|status|Varchar|255|Default 'unread'|Statut du message (lu, non lu)|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|
|senderId|Int||FK -> Users(id)|Expéditeur|
|receiverId|Int||FK -> Users(id)|Destinataire|

### **Addresses**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|id|Int||PK Auto increment|Identifiant unique|
|streetAddress|Varchar|255|Not null|Adresse postale|
|additionalInfo|Varchar|255||Complément d'adresse|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|
|cityId|Int||FK -> Cities(id)|Ville associée|

### **Cities**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|id|Int||PK Auto increment|Identifiant unique|
|name|Varchar|255|Not null|Nom de la ville|
|postalCode|Varchar|20|Not null|Code postal|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|
|countryId|Int||FK -> Countries(id)|Référence au pays|

### **Countries**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|id|Int||PK Auto increment|Identifiant unique|
|name|Varchar|255|Not null, Unique|Nom du pays|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|

### **PetsAdverts**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|petId|Int||PK, FK -> Pets(id)|Référence à l'animal|
|advertId|Int||PK, FK -> Adverts(id)|Référence à l'annonce|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|

### **Reviews**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|userId|Int||PK, FK -> Users(id)|Référence à l'utilisateur|
|advertId|Int||PK, FK -> Adverts(id)|Référence à l'annonce|
|rate|Float||Not null|Note attribuée|
|comment|Text|||Commentaire|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|

### **Payments**

|Nom du champ|Type de données|Longueur|Contrainte|Description|
|---|---|---|---|---|
|userId|Int||PK, FK -> Users(id)|Référence à l'utilisateur|
|advertId|Int||PK, FK -> Adverts(id)|Référence à l'annonce|
|amount|Decimal(10,2)||Not null|Montant payé|
|status|Varchar|255|Default 'pending'|Statut du paiement|
|createdAt|Timestamp||Default CURRENT_TIMESTAMP|Date de création|
|updatedAt|Timestamp||Default CURRENT_TIMESTAMP|Date de mise à jour|

---
# MLD 

Users (id, lastName, firstName, email, bio, password, photo, rating, role, status, isVerified, balanceAccount, onboardingStep, isOnboardingCompleted, createdAt, updatedAt, #roleId, #adressId)

Pets (<u>id</u>, name, breed, age, photo, info, createdAt, updatedAt, #userId, #animalTypesId)

AnimalTypes (<u>id</u>, name, createdAt, updatedAt)

Adverts (<u>id</u>, startDate, endDate, status, amount, createdAt, updatedAt)

Notifications (<u>id</u>, comment, createdAt, updatedAt, #userId)

Reports (<u>id</u>, comment, createdAt, updatedAt, #userId )

PasswordResets (<u>id</u>, token, isValid, expiresAt, createdAt, updatedAt, #userId)

Roles (<u>id</u>, name, createdAt, updatedAt)

Messages (<u>id</u>, content, status, createdAt, updatedAt, #senderId, #receiverId)

Addresses (<u>id</u>, streetAddress, additionalInfo, createdAt, updatedAt, #cityId)

Cities (<u>id</u>, name, postalCode, createdAt, updatedAt, #countryId)

Countries (<u>id</u>, name, createdAt, updatedAt)

PetsAdverts ( <u> #petId, #advertId</u>, createdAt, updatedAt)

Reviews ( <u>#userId, #advertId</u>, rate, comment, createdAt, updatedAt)

Payments ( <u>#userId, #advertId</u>, amount, status, createdAt, updatedAt)