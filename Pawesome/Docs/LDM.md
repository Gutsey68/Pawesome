### LDM

Users (id, lastName, firstName, email, bio, password, photo, rating, role, status, isVerified, balanceAccount, onboardingStep, isOnboardingCompleted, completedProfile, createdAt, updatedAt, #roleId, #addressId)

Pets (<u>id</u>, name, breed, age, photo, info, createdAt, updatedAt, #userId, #animalTypeId)

AnimalTypes (<u>id</u>, name, createdAt, updatedAt)

Adverts (<u>id</u>, startDate, endDate, status, amount, additionalInformation, createdAt, updatedAt, #userId, #addressId)

Notifications (<u>id</u>, comment, createdAt, updatedAt, #userId)

Reports (<u>id</u>, comment, isResolved, targetId, reportType, status, createdAt, updatedAt, #userId)

PasswordResets (<u>id</u>, token, isValid, expiresAt, createdAt, updatedAt, #userId)

Roles (<u>id</u>, name, createdAt, updatedAt)

Messages (<u>id</u>, content, status, createdAt, updatedAt, #senderId, #receiverId)

Addresses (<u>id</u>, streetAddress, additionalInfo, createdAt, updatedAt, #cityId)

Cities (<u>id</u>, name, postalCode, createdAt, updatedAt, #countryId)

Countries (<u>id</u>, name, createdAt, updatedAt)

PetAdverts (<u>#petId, #advertId</u>, createdAt, updatedAt)

Reviews (<u>#userId, #advertId</u>, rate, comment, createdAt, updatedAt)

Payments (<u>id</u>, amount, status, sessionId, paymentIntentId, createdAt, updatedAt, #userId, #advertId)

AnimalTypeAdverts (<u>#animalTypeId, #advertId</u>, createdAt, updatedAt)

## Data Dictionary

### **Users**

| Field name            | Data type       | Length   | Constraint                 | Description                         |
|-----------------------|----------------|----------|----------------------------|-------------------------------------|
| id                    | Int            |          | PK Auto increment          | Unique user identifier              |
| lastName              | Varchar        | 255      | Not null                   | User's last name                    |
| firstName             | Varchar        | 255      | Not null                   | User's first name                   |
| email                 | Varchar        | 255      | Not null, Unique           | User's email address                |
| bio                   | Text           |          |                            | User biography                      |
| password              | Varchar        | 255      | Not null                   | Hashed password                     |
| photo                 | Varchar        | 255      |                            | Profile photo URL                   |
| rating                | Float          |          |                            | User's average rating               |
| role                  | Varchar        | 255      |                            | User's role                         |
| status                | Varchar        | 255      |                            | User's status                       |
| isVerified            | Boolean        |          | Default false              | Account verification                |
| balanceAccount        | Decimal(10,2)  |          | Default 0                  | User account balance                |
| onboardingStep        | Int            |          | Default 1                  | Current onboarding step             |
| isOnboardingCompleted | Boolean        |          | Default false              | Indicates if onboarding is complete |
| completedProfile      | Int            | 100      | Default 0                  | Profile completion percentage       |
| createdAt             | Timestamp      |          | Default CURRENT_TIMESTAMP  | Creation date                       |
| updatedAt             | Timestamp      |          | Default CURRENT_TIMESTAMP  | Update date                         |
| roleId                | Int            |          | FK -> Roles(id)            | Reference to the role               |
| addressId             | Int            |          | FK -> Addresses(id)        | Reference to the address            |

### **Pets**

| Field name | Data type | Length | Constraint | Description |
|---|---|---|---|---|
| id | Int | | PK Auto increment | Unique pet identifier |
| name | Varchar | 255 | Not null | Pet's name |
| breed | Varchar | 255 | | Pet's breed |
| age | Int | | | Pet's age |
| photo | Varchar | 255 | | Pet's photo URL |
| info | Text | | | Additional information |
| createdAt | Timestamp | | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp | | Default CURRENT_TIMESTAMP | Update date |
| userId | Int | | FK -> Users(id) | Pet owner |
| animalTypeId | Int | | FK -> AnimalTypes(id) | Animal type |

### **AnimalTypes**

| Field name | Data type | Length | Constraint | Description |
|---|---|---|---|---|
| id | Int | | PK Auto increment | Unique identifier |
| name | Varchar | 255 | Not null | Animal type (e.g., dog, cat) |
| createdAt | Timestamp | | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp | | Default CURRENT_TIMESTAMP | Update date |

### **Adverts**

| Field name   | Data type | Length | Constraint | Description                 |
|--------------|-----------|--------|------------|-----------------------------|
| id           | Int       |        | PK Auto increment | Unique identifier           |
| startDate    | DateTime  |        | Not null   | Start date                  |
| endDate      | DateTime  |        | Not null   | End date                    |
| status       | Varchar   | 255    | Default 'pending' | Advert status               |
| amount       | Decimal(10,2) |    | Not null   | Advert amount               |
| additionalInformation | Varchar | 255 | | Additional information      |
| createdAt    | Timestamp |        | Default CURRENT_TIMESTAMP | Creation date              |
| updatedAt    | Timestamp |        | Default CURRENT_TIMESTAMP | Update date                |
| userId       | Int       |        | FK -> Users(id) | Reference to the user        |
| addressId    | Int       |        | FK -> Addresses(id) | Reference to the address    |

### **Notifications**

| Field name | Data type | Length | Constraint | Description |
|---|---|---|---|---|
| id | Int | | PK Auto increment | Unique identifier |
| comment | Text | | | Comment |
| createdAt | Timestamp | | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp | | Default CURRENT_TIMESTAMP | Update date |
| userId | Int | | FK -> Users(id) | Reference to the user |

### **Reports**

| Field name | Data type | Length | Constraint | Description |
|---|---|---|---|---|
| id | Int | | PK Auto increment | Unique identifier |
| comment | Text | | | Report comment |
| isResolved | Boolean | | Default false | Indicates if the report is resolved |
| targetId | Int | | Not null | Reported item ID |
| reportType | Varchar | 50 | | Type of reported item |
| status | Varchar | 20 | Default 'Pending' | Report status |
| createdAt | Timestamp | | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp | | Default CURRENT_TIMESTAMP | Update date |
| userId | Int | | FK -> Users(id) | Reference to the user |

### **PasswordResets**

| Field name | Data type | Length | Constraint | Description |
|---|---|---|---|---|
| id | Int | | PK Auto increment | Unique identifier |
| token | Varchar | 255 | Not null, Unique | Reset token |
| isValid | Boolean | | Default true | Indicates if the token is valid |
| expiresAt | DateTime | | Not null | Expiration date |
| createdAt | Timestamp | | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp | | Default CURRENT_TIMESTAMP | Update date |
| userId | Int | | FK -> Users(id) | Reference to the user |

### **Roles**

| Field name | Data type | Length | Constraint | Description |
|---|---|---|---|---|
| id | Int | | PK Auto increment | Unique identifier |
| name | Varchar | 255 | Not null, Unique | Role name |
| createdAt | Timestamp | | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp | | Default CURRENT_TIMESTAMP | Update date |

### **Messages**

| Field name | Data type | Length | Constraint | Description |
|---|---|---|---|---|
| id | Int | | PK Auto increment | Unique identifier |
| content | Text | | Not null | Message content |
| status | Varchar | 255 | Default 'unread' | Message status (read, unread) |
| createdAt | Timestamp | | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp | | Default CURRENT_TIMESTAMP | Update date |
| senderId | Int | | FK -> Users(id) | Sender |
| receiverId | Int | | FK -> Users(id) | Receiver |

### **Addresses**

| Field name | Data type | Length | Constraint | Description |
|---|---|---|---|---|
| id | Int | | PK Auto increment | Unique identifier |
| streetAddress | Varchar | 255 | Not null | Postal address |
| additionalInfo | Varchar | 255 | | Address additional info |
| createdAt | Timestamp | | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp | | Default CURRENT_TIMESTAMP | Update date |
| cityId | Int | | FK -> Cities(id) | Associated city |

### **Cities**

| Field name | Data type | Length | Constraint | Description |
|---|---|---|---|---|
| id | Int | | PK Auto increment | Unique identifier |
| name | Varchar | 255 | Not null | City name |
| postalCode | Varchar | 20 | Not null | Postal code |
| createdAt | Timestamp | | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp | | Default CURRENT_TIMESTAMP | Update date |
| countryId | Int | | FK -> Countries(id) | Reference to the country |

### **Countries**

| Field name | Data type | Length | Constraint | Description |
|---|---|---|---|---|
| id | Int | | PK Auto increment | Unique identifier |
| name | Varchar | 255 | Not null, Unique | Country name |
| createdAt | Timestamp | | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp | | Default CURRENT_TIMESTAMP | Update date |

### **PetAdverts**

| Field name | Data type | Length | Constraint | Description |
|---|---|---|---|---|
| petId | Int | | PK, FK -> Pets(id) | Reference to the pet |
| advertId | Int | | PK, FK -> Adverts(id) | Reference to the advert |
| createdAt | Timestamp | | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp | | Default CURRENT_TIMESTAMP | Update date |

### **Reviews**

| Field name | Data type | Length | Constraint | Description |
|---|---|---|---|---|
| userId | Int | | PK, FK -> Users(id) | Reference to the user |
| advertId | Int | | PK, FK -> Adverts(id) | Reference to the advert |
| rate | Float | | Not null | Given rating |
| comment | Text | | | Comment |
| createdAt | Timestamp | | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp | | Default CURRENT_TIMESTAMP | Update date |

### **Payments**

| Field name | Data type | Length | Constraint | Description |
|---|---|---|---|---|
| id | Int | | PK Auto increment | Unique identifier |
| amount | Decimal(10,2) | | Not null | Paid amount |
| status | Varchar | 255 | Default 'pending' | Payment status |
| sessionId | Varchar | 255 | | Payment session ID |
| paymentIntentId | Varchar | 255 | | Payment intent ID |
| createdAt | Timestamp | | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp | | Default CURRENT_TIMESTAMP | Update date |
| userId | Int | | FK -> Users(id) | Reference to the user |
| advertId | Int | | FK -> Adverts(id) | Reference to the advert |

### **AnimalTypeAdverts**

| Field name | Data type | Length | Constraint | Description |
|------------|-----------|--------|------------|-------------|
| animalTypeId | Int |  | PK, FK -> AnimalTypes(id) | Reference to the animal type |
| advertId | Int |  | PK, FK -> Adverts(id) | Reference to the advert |
| createdAt | Timestamp |  | Default CURRENT_TIMESTAMP | Creation date |
| updatedAt | Timestamp |  | Default CURRENT_TIMESTAMP | Update date |
