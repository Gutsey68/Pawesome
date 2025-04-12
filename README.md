# Guide de démarrage du projet Pawesome

Ce guide vous aidera à configurer le projet MVC Pawesome pour le développement, utilisant Docker uniquement pour la base de données et le service mail.

## Prérequis

- [Docker](https://www.docker.com/products/docker-desktop/) installé et en cours d'exécution
- [.NET 9 SDK](https://dotnet.microsoft.com/download) installé localement
- [Git](https://git-scm.com/downloads) pour cloner le dépôt
- Connaissances de base de .NET Core et Docker

## Instructions d'installation

1. Clonez le dépôt :
   ```bash
   git clone https://CCICampus@dev.azure.com/CCICampus/CDA-ALT2425-G3/_git/CDA-ALT2425-G3
   cd Pawesome
   ```

2. Vérifiez que Docker fonctionne sur votre machine

3. Assurez-vous que le fichier `compose.dev.yaml` est présent dans le répertoire racine du projet

4. Restaurez les dépendances du projet :
   ```bash
   dotnet restore
   ```

## Démarrage du projet

1. Lancez d'abord les conteneurs Docker :

```bash
docker compose -f compose.dev.yaml up -d
```

2. Naviguez vers le répertoire du projet et exécutez l'application :

```bash
cd Pawesome
dotnet run
```

L'application sera disponible à l'adresse http://localhost:5159/

## Services Docker

### Base de données PostgreSQL

- Hôte : localhost
- Port : 5434 (mappé depuis 5432 dans le conteneur)
- Nom d'utilisateur : pawesome
- Mot de passe : pawesome
- Nom de la base : pawesome

### MailHog (Service de test d'emails)

Un service MailHog est inclus pour intercepter et visualiser les emails envoyés par l'application pendant le développement.

- Interface web : http://localhost:8025/
- Serveur SMTP : localhost:1025

Tous les emails envoyés par l'application seront capturés par MailHog et visibles dans l'interface web.

## Architecture du projet

Le projet Pawesome est structuré selon les principes MVC (Model-View-Controller) avec une organisation modulaire :

- **Extensions** - Classes d'extension pour simplifier la configuration :
   - `ServiceCollectionExtensions.cs` : Configure les services (base de données, identity, validations, etc.)
   - `ApplicationBuilderExtensions.cs` : Configure le pipeline HTTP
   - `WebApplicationExtensions.cs` : Gère l'initialisation de la base de données

- **Architecture en couches** :
   - **Controllers** : Gèrent les requêtes HTTP
   - **Models** : Définissent les entités et DTOs
   - **Views** : Interface utilisateur
   - **Services** : Logique métier
   - **Repositories** : Accès aux données
   - **Validators** : Validation des données avec FluentValidation

## Arrêt de l'application

Pour arrêter l'application, appuyez sur `Ctrl+C` dans le terminal où elle s'exécute.

Pour arrêter les conteneurs Docker :

```bash
docker compose -f compose.dev.yaml down
```

Pour également supprimer les volumes (données de la base) :

```bash
docker compose -f compose.dev.yaml down -v
```