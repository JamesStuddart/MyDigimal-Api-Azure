# MyDigimal Platform - Comprehensive Project Documentation

## Executive Summary

**MyDigimal** is a comprehensive animal care tracking and management platform that enables users to digitally manage their animal collections (referred to as "digimals"). The platform provides customizable health logging, feeding schedules, genetics tracking, and community features for animal enthusiasts, hobbyists, breeders, and professionals.

### Technology Stack
- **Backend**: Azure Functions V4, .NET 9.0, PostgreSQL (Neon), Auth0
- **Frontend (Svelte)**: Svelte 3, Tailwind CSS, Smelte UI - **~60% Complete**
- **Frontend (Flutter)**: Flutter 3.x, Provider pattern - **~40% Complete**
- **Authentication**: Auth0 with JWT tokens
- **Database**: PostgreSQL via Neon serverless
- **Hosting**: Azure Functions (Serverless)

### Current Status
- **API**: Fully functional with 30+ endpoints
- **Svelte Web App**: Beta stage, core features implemented
- **Flutter Mobile App**: Alpha stage, incomplete features
- **Recommended Path**: Build new frontend to replace incomplete Svelte/Flutter apps

---

## Repository Structure

### Primary Repositories
1. **API Backend**: `/Users/jamesstuddart/Code/MyDigimal-Api-Azure`
2. **Svelte Web App**: `/Users/jamesstuddart/Code/MyDigimal`
3. **Flutter Mobile App**: `/Users/jamesstuddart/Code/MyDigimal-Mobile`

---

# PART 1: BACKEND API (Azure Functions)

## Architecture Overview

### Technology Details
- **Framework**: Azure Functions V4 (isolated worker model)
- **Runtime**: .NET 9.0
- **Database**: PostgreSQL on Neon (serverless pooler)
- **ORM**: Dapper (micro-ORM with raw SQL)
- **Authentication**: Auth0 JWT validation
- **Storage**: Azure Blob/Queue/Table Storage
- **Monitoring**: Application Insights

### Project Structure
```
MyDigimal-Api-Azure/
├── MyDigimal.Api.Azure/          # HTTP Triggers (Controllers)
│   ├── Triggers/
│   │   ├── AuthenticationTrigger.cs
│   │   ├── CreatureTrigger.cs
│   │   ├── LogTrigger.cs
│   │   ├── SchemaTrigger.cs
│   │   ├── GroupsTrigger.cs
│   │   ├── NotificationsTrigger.cs
│   │   ├── NewsTrigger.cs
│   │   ├── ReferenceDataTrigger.cs
│   │   ├── TagsTrigger.cs
│   │   ├── ReportingTrigger.cs
│   │   └── UserTrigger.cs
│   ├── Models/                   # View models/DTOs
│   └── Program.cs                # DI & startup config
├── MyDigimal.Core/               # Business logic layer
├── MyDigimal.Data/               # Data access layer
└── MyDigimal.Common/             # Shared utilities
```

---

## API Endpoints Reference

### Authentication (`/auth`)
| Method | Endpoint | Auth Level | Purpose |
|--------|----------|------------|---------|
| GET | `/auth` | User | Check if authenticated |
| POST | `/auth/register` | Public | Register new user (Auth0 webhook) |
| POST | `/auth/details` | User | Get user profile details |

**Register Request:**
```json
{
  "email": "user@example.com",
  "username": "username",
  "platform": "Google"
}
```

**User Details Response:**
```json
{
  "name": "John Doe",
  "avatar": "https://...",
  "accountPlan": "Free",
  "paymentPlan": "Monthly",
  "renewalMonth": 12,
  "renewalYear": 2025,
  "role": "User",
  "accountPlanSettings": {
    "maxCreatures": 10
  }
}
```

---

### Creatures (`/creature`)
| Method | Endpoint | Auth Level | Purpose |
|--------|----------|------------|---------|
| GET | `/creature` | User | List all user's creatures |
| GET | `/creature/sc/{shortCode}` | Public | Get creature by short code |
| GET | `/creature/{id}` | User | Get specific creature |
| POST | `/creatures` | User | Create new creature |
| PUT | `/creature/{id}` | User | Update creature |
| DELETE | `/creature/{id}` | User | Archive creature (soft delete) |

**Create Creature Request:**
```json
{
  "name": "Smaug",
  "commonName": "Ball Python",
  "species": "Python regius",
  "sex": "Male",
  "morph": "Pastel",
  "genes": ["Pastel", "Het Pied"],
  "born": "2023-01-15",
  "bornBy": "Breeder Name",
  "sire": "uuid-of-father",
  "dam": "uuid-of-mother",
  "purchased": "2023-03-01",
  "purchasedFrom": "Reptile Expo",
  "feedingCadence": 7,
  "feedingCadenceType": "Days",
  "tags": "python,pastel,breeder",
  "groupName": "My Snakes",
  "logSchemaId": "uuid-of-schema"
}
```

**Creature Response:**
```json
{
  "id": "uuid",
  "name": "Smaug",
  "commonName": "Ball Python",
  "species": "Python regius",
  "sex": "Male",
  "status": "Alive",
  "shortCode": "ABC123",
  "image": "https://...",
  "owner": "user-uuid",
  "created": "2023-03-01T10:00:00Z"
}
```

---

### Logging (`/creature/{id}/log`)
| Method | Endpoint | Auth Level | Purpose |
|--------|----------|------------|---------|
| POST | `/creature/{id}/log` | User | Create log entry/entries |
| POST | `/creature/{id}/log/duplicate/{entryId}` | User | Duplicate last log entry |
| GET | `/creature/{id}/log/-/{fromYear}/{toYear}` | User | Get logs for date range |
| GET | `/creature/-/log/-/{fromYear}/{toYear}` | User | Get all user logs |
| GET | `/creature/{id}/log/{schemaEntryId}/{fromYear}/{toYear}` | User | Get filtered logs |
| DELETE | `/creature/{id}/log/{entryId}` | User | Delete log entry |

**Create Log Entry Request:**
```json
{
  "entries": [
    {
      "logSchemaEntryId": "uuid",
      "value": "Frozen/Thawed Rat",
      "date": "2025-10-17",
      "notes": "Ate well",
      "correlationId": "batch-uuid",
      "linkEntryIds": ["related-creature-uuid"]
    }
  ]
}
```

---

### Schemas (`/schema`)
| Method | Endpoint | Auth Level | Purpose |
|--------|----------|------------|---------|
| GET | `/schema` | User | List available schemas |
| GET | `/schema/{id}` | User | Get schema with entries |
| GET | `/schema/{id}/creature/{creatureId}` | User | Get schema for creature |
| GET | `/creature/{creatureId}/schema` | User | Get creature's schema |
| GET | `/schema/{id}/genetics` | User | Get genetics data |

**Schema Response:**
```json
{
  "id": "uuid",
  "title": "Reptile Care Log",
  "author": "user-uuid",
  "isPublic": true,
  "entries": [
    {
      "id": "uuid",
      "title": "Feeding",
      "icon": "food",
      "type": "choice",
      "values": ["Frozen/Thawed", "Live", "Refused"],
      "required": true,
      "chartType": "Line"
    }
  ]
}
```

---

### Groups (`/groups`)
| Method | Endpoint | Auth Level | Purpose |
|--------|----------|------------|---------|
| GET | `/groups` | User | List user's groups |
| POST | `/groups/{groupName}` | User | Create new group |
| DELETE | `/groups/{groupName}` | User | Delete group |

---

### Notifications (`/notification`)
| Method | Endpoint | Auth Level | Purpose |
|--------|----------|------------|---------|
| GET | `/notification` | User | Get unread notifications |
| DELETE | `/notification/{id}` | User | Mark as read |
| PUT | `/notification/{id}/{processType}` | User | Accept/decline notification |

**Notification Types:**
- `SubscriptionActivated`
- `SubscriptionExpiring`
- `DigimalTransferred`
- `MessageReceived`
- `WebsitePublished`
- `DomainConnected`

---

### Reporting (`/reporting`)
| Method | Endpoint | Auth Level | Purpose |
|--------|----------|------------|---------|
| GET | `/reporting/feeds/due` | User | Get creatures due for feeding |

**Due Feeds Response:**
```json
[
  {
    "creatureId": "uuid",
    "creatureName": "Smaug",
    "lastFeedDate": "2025-10-10",
    "nextFeedDate": "2025-10-17",
    "feedingCadence": 7,
    "feedingCadenceType": "Days",
    "latestLogEntry": { /* log entry object */ }
  }
]
```

---

### Tags (`/tags`)
| Method | Endpoint | Auth Level | Purpose |
|--------|----------|------------|---------|
| GET | `/tags` | User | Get all user tags |

---

### News (`/newsfeed`)
| Method | Endpoint | Auth Level | Purpose |
|--------|----------|------------|---------|
| GET | `/newsfeed` | Public | Get latest news |
| POST | `/newsfeed` | Admin | Create news article |
| DELETE | `/newsfeed?id={id}` | Admin | Delete news article |

---

### Reference Data (`/ref`)
| Method | Endpoint | Auth Level | Purpose |
|--------|----------|------------|---------|
| GET | `/ref/creature` | User | Get creature reference data |
| GET | `/ref/authentication` | User | Get auth platform options |

---

## Data Models

### Core Entities

#### UserEntity
```csharp
{
    Guid Id
    string Name
    string Email
    string Avatar
    AccountPlanType AccountPlan        // Free, Starter, Hobbiest, Breeder, Ultra, EarlyAdopter
    PaymentPlanType PaymentPlan        // Monthly, Yearly
    AccountStatusType AccountStatus    // Active, Suspended, Deactivated, etc.
    AccountRoleType AccountRole        // Public=-1000, User=0, Admin=1000
    int RenewalMonth
    int RenewalYear
}
```

#### CreatureEntity
```csharp
{
    Guid Id
    string Name
    string CommonName
    string Species
    string Morph
    string Genes                       // JSON string
    CreatureSexType Sex                // Unknown, Male, Female
    CreatureStatusType Status          // Alive, Missing, Sick, Dead, Archived
    Guid Owner                         // FK to User
    Guid? LogSchemaId                  // FK to Schema
    DateTime? Born
    int? BornYear
    string BornBy
    Guid? Sire                         // Parent creature (father)
    Guid? Dam                          // Parent creature (mother)
    DateTime? Purchased
    string PurchasedFrom
    string PurchasedUrl
    string Image
    string ShortCode                   // Public identifier
    int? FeedingCadence
    FeedingCadenceType? FeedingCadenceType  // Hours, Days, Weeks, Months
    string Tags                        // Comma-separated
    string GroupName
}
```

#### LogSchemaEntity
```csharp
{
    Guid Id
    string Title
    bool IsPublic
    Guid? Author                       // FK to User
    DateTime Created
    DateTime? Modified
    string RecommendedSpecies          // Comma-separated
    string RecommendedCommonNames      // Comma-separated
    string Genes                       // JSON string
    string Morphs                      // JSON string
}
```

#### LogSchemaEntryEntity
```csharp
{
    Guid Id
    Guid SchemaId                      // FK to Schema
    Guid? ParentId                     // For hierarchical entries
    string Icon
    string Title
    LogSchemaEntryType Type            // text, numeric, choice, date, etc.
    ChartType ChartType                // None, HeatMap, Line
    string Values                      // JSON array for choices
    string DefaultValue
    bool Required
    bool RepeatLastEntry
    bool QuickAction
}
```

#### LogEntryEntity
```csharp
{
    Guid Id
    Guid CreatureId                    // FK to Creature
    Guid LogSchemaEntryId              // FK to SchemaEntry
    DateTime Date
    string Notes
    string Value
    Guid? CorrelationId                // Groups related entries
    Guid Owner                         // FK to User
}
```

#### NotificationEntity
```csharp
{
    Guid Id
    NotificationType Type
    string Title
    string Description
    string MetaData                    // JSON
    Guid? Author
    string AuthorName
    Guid Recipient                     // FK to User
    DateTime Created
    DateTime? DateRead
}
```

#### CreatureGroupEntity
```csharp
{
    Guid Id
    string Name
    Guid CreatedBy                     // FK to User
}
```

#### NewsEntity
```csharp
{
    Guid Id
    string Title
    string Description
    bool IsPublic
    bool IsArchived
    Guid Author                        // FK to User
    DateTime Created
    DateTime? Modified
}
```

---

## Authentication & Authorization

### Auth Flow
1. User authenticates with Auth0 (social login)
2. Auth0 issues JWT token with custom claims
3. Client sends token in `Authorization: Bearer {token}` header
4. API validates token against Auth0's public keys
5. Extracts `ClaimTypes.NameIdentifier` (Auth0 subject)
6. Looks up user in `UserExternalAuthEntity` by provider key
7. Enriches claims with `custom:user_id` and role

### Authorization Levels
- **Public** (-1000): No authentication required
- **User** (0): Authenticated user
- **Admin** (1000): Administrator only

### Token Claims
```json
{
  "custom:user_id": "user-database-uuid",
  "sub": "auth0|12345",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "0"
}
```

### Configuration
```json
{
  "Auth0Settings": {
    "AuthorityEndpoint": "https://azure-mydigimal.uk.auth0.com/",
    "Issuer": "https://azure-mydigimal.uk.auth0.com/",
    "Audience": "https://dev-api.mydigimal.com",
    "ActionSecret": "[webhook-secret]"
  }
}
```

---

## Business Logic Patterns

### Account Plans
Each plan enforces creature limits:
- **Free**: 10 creatures
- **Starter**: 25 creatures
- **Hobbiest**: 50 creatures
- **Breeder**: 100 creatures
- **Ultra**: 250 creatures
- **EarlyAdopter**: Unlimited

### Creature Lifecycle
1. **Create** → Auto-generates `CreatureCreation` event
2. **Update** → Metadata changes tracked
3. **Archive** → Soft delete (Status = Archived)
4. **Transfer** → Ownership change with notifications

### Log Entry Linking
- Supports creature-to-creature relationships via `LinkEntryIds`
- When creature A logs, automatically creates entry for creature B
- Use case: Breeding events, transfers, shared activities

### Feeding Schedule Calculation
```csharp
NextFeedDate = FeedingCadenceType switch {
    Hours => LastFeedDate.AddHours(FeedingCadence),
    Days => LastFeedDate.AddDays(FeedingCadence),
    Weeks => LastFeedDate.AddDays(FeedingCadence * 7),
    Months => LastFeedDate.AddMonths(FeedingCadence)
};
```

### Correlation IDs
- Groups related log entries across multiple creatures
- Enables batch operations (create/delete)
- Maintains data integrity for multi-creature events

---

## Database Schema

### Connection Details
- **Provider**: Neon PostgreSQL (serverless)
- **Host**: `ep-summer-firefly-a9v4nk2h-pooler.gwc.azure.neon.tech:5432`
- **Database**: MyDigimal
- **SSL**: Required
- **Pooling**: Enabled via Neon pooler

### Key Tables
- `users` - User accounts
- `user_auth_platforms` - Social login tracking
- `user_external_auth` - Auth0 integration
- `creatures` - Animal records
- `creature_groups` - Organization groups
- `creature_events` - Audit trail
- `log_schemas` - Template definitions
- `log_schema_entries` - Field definitions
- `log_entries` - Actual log data
- `notifications` - User notifications
- `news` - News feed items

### Relationships
```
users 1---N creatures
users 1---N creature_groups
creatures N---1 log_schemas
log_schemas 1---N log_schema_entries
creatures 1---N log_entries
log_schema_entries 1---N log_entries
creatures N---1 creatures (sire/dam - self-referential)
users 1---N notifications
```

---

## Configuration & Deployment

### Environment Variables
```json
{
  "AzureWebJobsStorage": "UseDevelopmentStorage=true",
  "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
  "EncryptionConfig:Key": "[encryption-key]",
  "NpgSqlConnectionConfig:Connection": "[postgres-connection-string]",
  "Auth0Settings:AuthorityEndpoint": "https://azure-mydigimal.uk.auth0.com/",
  "Auth0Settings:Issuer": "https://azure-mydigimal.uk.auth0.com/",
  "Auth0Settings:Audience": "https://dev-api.mydigimal.com",
  "Auth0Settings:ActionSecret": "[action-secret]",
  "AppSettings:AvailableLoginTypes": ["Facebook", "Google"]
}
```

### CORS Configuration
**Development**: Allow all origins
**Production**: Restrict to:
- `https://mydigimal.com`
- `https://test.mydigimal.com`
- `https://dev.mydigimal.com`

### Monitoring
- Application Insights enabled
- Telemetry with sampling
- Exception logging middleware

---

# PART 2: SVELTE WEB APP

## Overview
- **Status**: ~60% complete (Beta)
- **Framework**: Svelte 3
- **UI Library**: Smelte (Material Design + Tailwind CSS)
- **Routing**: svelte-spa-router
- **State**: Svelte stores (reactive)
- **Build**: Rollup

## Application Structure
```
src/
├── App.svelte                     # Main router container
├── main.js                        # Entry point
├── routes.js                      # Route definitions
├── _shared/                       # Global components
│   ├── ui/                        # UI components (Alert, Modal, etc.)
│   └── components/                # Shared business components
├── public/                        # Unauthenticated pages
│   ├── home/                      # Landing page
│   ├── privacy/ & terms/
│   └── redirects/                 # Short-code redirects
└── secure/                        # Authenticated pages
    ├── dashboard/                 # Main dashboard
    ├── digimals/                  # Creature CRUD
    ├── profile/                   # User settings
    └── users/                     # Admin panel
```

## Implemented Features

### Authentication
- Auth0 integration (token-based)
- Cookie session management (30-min timeout)
- Profile caching
- Auto-login on page load
- Role-based access (User/Admin)

### Digimal Management
- List/grid view with filtering
- Create new digimal form
- Edit digimal details
- Delete/archive digimal
- Import from CSV (bulk)
- Group management
- Tag management
- Short-code sharing
- QR code generation

### Logging System
- Schema-based custom fields
- Date range filtering
- Log entry creation
- Log history view
- Quick-action logging
- Duplicate last entry

### Dashboard
- Widget-based layout
- Recent activity
- Upcoming feeds
- Notifications panel
- Quick actions

### Profile Management
- User settings
- Avatar (placeholder)
- Account details
- Billing info (display only)

### Admin Features
- User management
- User list/search
- User details view

## Incomplete Features
- git  generator
- Enclosure management
- Configuration system
- Store/marketplace
- Help documentation
- Advanced genetics UI
- Payment integration

## State Management

### authStore.js
```javascript
{
  authStatus: "NotChecked" | "Checking" | "Authorized" | "Unauthorized",
  isSignedIn: boolean,
  userProfile: {
    avatar, email, username, token,
    accountPlan, paymentPlan, accountStatus,
    role, accountPlanSettings
  }
}
```

### dataStore.js
```javascript
{
  digimals: [],           // User's creatures
  schemas: [],            // Available log schemas
  tags: [],               // User's tags
  groups: [],             // User's groups
  newsFeed: []            // News items
}
```

### notificationStore.js
```javascript
{
  notifications: []       // Unread notifications
}
```

## API Integration
All API calls use Axios with Bearer token authentication:
```javascript
const repo = axios.create({
  baseURL: process.env.gatewayUri,
  headers: { 'Authorization': profile.token }
});
```

## Routes

### Public Routes
- `/` - Home/landing page
- `/privacy` - Privacy policy
- `/terms` - Terms of service
- `/sc/:shortCode/` - Short-code redirect
- `/qa/:shortCode/` - Quick-action redirect

### Secure Routes
- `/secure/dashboard/` - Main dashboard
- `/secure/digimals/` - Digimal list
- `/secure/digimals/create/` - Create digimal
- `/secure/digimals/:id/` - Digimal details
- `/secure/digimals/import/` - CSV import
- `/secure/digimals/cards/` - Card generation
- `/secure/profile/` - User profile
- `/secure/profile/settings` - Account settings
- `/secure/users` - Admin user management

## Key Dependencies
- `svelte@^3.0.0`
- `smelte@^1.0.18` - UI components
- `svelte-spa-router@^2.2.0` - Routing
- `axios@^0.21.1` - HTTP client
- `papaparse@^5.3.1` - CSV parsing
- `uuid@^8.3.2` - UUID generation
- `fa-svelte@^3.1.0` - FontAwesome icons

## Build & Development
```bash
npm install
npm run dev          # Dev server with live reload
npm run build        # Production build
npm run start        # Serve production build
```

---

# PART 3: FLUTTER MOBILE APP

## Overview
- **Status**: ~40% complete (Alpha)
- **Framework**: Flutter 3.x
- **State Management**: Provider pattern
- **Architecture**: Repository pattern with services
- **Target**: iOS & Android

## Application Structure
```
lib/
├── main.dart                      # App entry point
├── models/                        # Data models
│   ├── creature.dart
│   ├── log_entry.dart
│   ├── schema.dart
│   └── user.dart
├── services/                      # API services
│   ├── auth_service.dart
│   ├── creature_service.dart
│   ├── log_service.dart
│   └── api_client.dart
├── providers/                     # State providers
│   ├── auth_provider.dart
│   ├── creature_provider.dart
│   └── theme_provider.dart
├── screens/                       # UI screens
│   ├── auth/
│   │   ├── login_screen.dart
│   │   └── register_screen.dart
│   ├── home/
│   │   ├── home_screen.dart
│   │   └── dashboard_screen.dart
│   ├── creatures/
│   │   ├── creature_list_screen.dart
│   │   ├── creature_detail_screen.dart
│   │   └── create_creature_screen.dart
│   └── profile/
│       └── profile_screen.dart
└── widgets/                       # Reusable widgets
    ├── creature_card.dart
    ├── log_entry_form.dart
    └── custom_app_bar.dart
```

## Implemented Features

### Authentication
- Login screen
- Auth0 integration (flutter_auth0 package)
- Token storage (flutter_secure_storage)
- Auto-login on app launch
- Logout functionality

### Creature Management
- Creature list view (grid/list)
- Creature detail view
- Create creature (basic form)
- Image upload (partially implemented)
- Search/filter (basic)

### Logging
- Log entry form
- Schema-based fields (partial)
- Date picker
- Submit log entry

### Dashboard
- Summary widgets
- Recent activity (placeholder)
- Quick actions

### Profile
- User profile view
- Account details
- Settings (partial)

## Incomplete Features
- Advanced creature editing
- Full schema support (complex field types)
- Genetics tracking UI
- Notifications
- Groups management
- Tags management
- Feeding schedule
- QR code scanning
- CSV import
- Admin features
- Offline support
- Push notifications

## State Management

### AuthProvider
```dart
class AuthProvider extends ChangeNotifier {
  User? _currentUser;
  String? _token;
  bool _isAuthenticated = false;

  Future<void> login(String email, String password);
  Future<void> logout();
  Future<void> refreshToken();
}
```

### CreatureProvider
```dart
class CreatureProvider extends ChangeNotifier {
  List<Creature> _creatures = [];
  Creature? _selectedCreature;

  Future<void> fetchCreatures();
  Future<void> createCreature(Creature creature);
  Future<void> updateCreature(String id, Creature creature);
  Future<void> deleteCreature(String id);
}
```

## API Integration
Uses `http` package with Bearer token:
```dart
final response = await http.get(
  Uri.parse('$baseUrl/creature'),
  headers: {
    'Authorization': 'Bearer $token',
    'Content-Type': 'application/json'
  }
);
```

## Key Dependencies
```yaml
dependencies:
  flutter: sdk: flutter
  provider: ^6.0.0              # State management
  http: ^0.13.0                 # HTTP client
  flutter_secure_storage: ^8.0.0  # Secure token storage
  flutter_auth0: ^1.0.0         # Auth0 integration
  cached_network_image: ^3.2.0  # Image caching
  intl: ^0.18.0                 # Date formatting
  image_picker: ^0.8.0          # Image selection
  qr_code_scanner: ^1.0.0       # QR scanning (not fully integrated)
```

## Build & Development
```bash
flutter pub get
flutter run              # Run on connected device
flutter build apk        # Build Android APK
flutter build ios        # Build iOS (requires Mac + Xcode)
```

---

# PART 4: DOMAIN CONCEPTS

## Core Domain Model

### Digimal (Creature)
A digital representation of a physical animal with:
- **Identity**: Name, species, common name, morph, genes
- **Biological**: Sex, birth date, parents (sire/dam)
- **Status**: Alive, missing, sick, dead, archived
- **Ownership**: Owner, purchase details
- **Care**: Feeding cadence, log schema
- **Organization**: Tags, groups
- **Sharing**: Short code, QR code, image

### Log Schema (Template System) - Core Feature

The **Template/Schema System** is the core feature that makes MyDigimal "the most customizable and extensible animal care tracking system online." It allows users to define custom logging templates for different animal types, with support for public/private templates, template recommendations, and a future template marketplace.

#### What is a Log Schema (Template)?

A **Log Schema** (also called a Template) is a customizable data collection framework that defines:
- **What data** to track for an animal (feeding, weight, sheds, vet visits, etc.)
- **How data is captured** (text input, numeric fields, dropdowns, dates, etc.)
- **How data is visualized** (line charts, heat maps, or no chart)
- **Special behaviors** (required fields, quick actions, repeat last value convenience)
- **Species recommendations** (which animals this template is designed for)
- **Genetics definitions** (genes and morphs specific to a species)

Think of it as a **dynamic form builder** where users can create tracking sheets perfectly tailored to their animal's needs.

#### Public vs Private Templates

**Private Templates:**
- **Ownership**: Created by and visible only to the creating user
- **Access**: Only the author can see and use the template
- **Use Case**: Custom tracking for personal collections with specific needs
- **Example**: "My Leopard Geckos - Extended Health" (tailored for user's specific veterinary tracking)

**Public Templates:**
- **Ownership**: Created by a user but marked as `isPublic = true`
- **Access**: Visible to ALL users in the system
- **Use Case**: Community-contributed templates for common animal types
- **Example**: "Ball Python Care Log" created by a breeder and shared with community
- **Future**: Featured/verified public templates by admin

**API Implementation** (MyDigimal.Data/Repositories/CreatureLogs/LogSchemasRepository.cs:15-26):
```csharp
public async Task<IEnumerable<LogSchemaEntity>> GetAsync(Guid ownerId, bool includePublic = false)
{
    var includePublicSql = includePublic ? "OR ispublic = true " : string.Empty;
    var result = await QueryAsync(
        $"SELECT * FROM LogSchemas WHERE author = @ownerId {includePublicSql}",
        new {ownerId}
    );
    return result.ToList();
}
```

When fetching schemas, users get:
- All templates they personally created (`author = userId`)
- **PLUS** all templates marked as public (`isPublic = true`)

This enables a **template library** where experienced users share their proven tracking systems.

#### Template Structure

**Level 1: Schema Metadata**
```csharp
LogSchemaEntity {
    Guid Id                           // Unique identifier
    string Title                      // "Ball Python Health Log"
    bool IsPublic                     // Public or private
    Guid? Author                      // Creator's user ID
    DateTime Created, Modified        // Timestamps

    // Recommendations
    string RecommendedSpecies         // "Python regius,Morelia spilota"
    string RecommendedCommonNames     // "Ball Python,Carpet Python"

    // Genetics (JSON)
    string Genes                      // {"het": ["Pied", "Clown"], "visible": ["Pastel", "Mojave"]}
    string Morphs                     // {"combos": ["Super Pastel", "Mojave Pastel"]}
}
```

**Recommended Species/Common Names**:
- Comma-separated lists suggesting which animals this template fits
- When creating a creature and selecting a template, the UI auto-populates species fields
- Helps users find appropriate templates quickly

**Genetics Data**:
- JSON strings storing gene and morph definitions
- Enables genetics calculators and visualization
- Retrieved via `/schema/{id}/genetics` endpoint

**Level 2: Schema Entries (Fields)**
```csharp
LogSchemaEntryEntity {
    Guid Id, SchemaId                 // Identity
    Guid? ParentId                    // Hierarchical support (nested fields)

    // Field definition
    string Icon                       // Material icon name
    string Title                      // "Feeding"
    string Type                       // "text", "numeric", "choice", "date", "boolean"
    int Index                         // Display order

    // Data capture
    string Values                     // JSON array for choice fields ["Live", "F/T", "Refused"]
    string DefaultValue               // Pre-filled value
    bool Required                     // Mandatory field

    // UX enhancements
    bool RepeatLastEntry              // Auto-fill with previous value
    bool QuickAction                  // Show in quick-log interface

    // Visualization
    int ChartType                     // None=0, HeatMap=1, Line=2
}
```

**Field Types**:
- `text`: Free-form text input
- `numeric`: Number input (weight, length, temperature)
- `choice`: Dropdown select (predefined options in `Values`)
- `date`: Date picker
- `boolean`: True/false checkbox

**Hierarchical Entries (Parent-Child)**:
- `ParentId` enables nested fields
- Example: "Feeding" parent with children "Prey Type", "Prey Size", "Refused"
- Allows grouping related data points
- Factory builds hierarchy: parent entries → child entries

**Quick Actions**:
- `QuickAction = true` marks fields for fast-entry UI
- Shows on dashboard/home screen for one-tap logging
- Example: "Fed today" button that logs feeding without opening full form

**Repeat Last Entry**:
- `RepeatLastEntry = true` auto-fills field with creature's last logged value
- Convenience feature: feeding type rarely changes, so default to last entry
- Reduces data entry time

**Chart Visualization**:
- `ChartType.Line`: Numeric data over time (weight gain, shed intervals)
- `ChartType.HeatMap`: Frequency visualization (feeding patterns, temperature cycles)
- `ChartType.None`: No chart rendering (text notes, yes/no fields)

#### LogSchemaFactory - Smart Defaults

The `LogSchemaFactory` builds a complete schema with smart defaults based on creature context (MyDigimal.Core/Schemas/LogSchemaFactory.cs:16-101):

```csharp
public async Task<LogSchemaModel> BuildSchema(
    Guid logSchemaId,
    Guid userId,
    bool includePublic = true,
    Guid? creatureId = null  // ← Context for smart defaults
)
```

**Key Features**:
1. **Permission Check**: Ensures user can access schema (owned or public)
2. **Hierarchical Loading**: Fetches parent entries, then children
3. **Creature-Specific Defaults**: If `creatureId` provided:
   - For fields with `RepeatLastEntry = true`
   - Queries creature's last log entry
   - Pre-fills `DefaultValue` with last recorded value
   - Makes logging faster: "Fed F/T rat" → defaults to "F/T rat" next time

**Example Flow**:
```
User opens log form for "Smaug" (ball python)
→ GET /schema/abc-123/creature/smaug-uuid
→ Factory fetches schema entries
→ Finds "Feeding" field with RepeatLastEntry=true
→ Queries last feeding entry for Smaug: "Frozen/Thawed Rat"
→ Sets DefaultValue = "Frozen/Thawed Rat"
→ UI shows dropdown pre-selected to "Frozen/Thawed Rat"
→ User just clicks "Submit" if same as last time
```

#### Template Recommendation System

When users create a new creature, they select a template. The system recommends templates based on species.

**Frontend Implementation** (MyDigimal Svelte app - /secure/digimals/new/index.svelte:96-109):
```javascript
let updateSpeciesNames = (event) => {
    let value = event.detail.target.selectedOptions[0].value;

    if (value && value.length > 0) {
        let selectedSchema = availableSchemas.find(x => x.id === value);

        if (selectedSchema) {
            // Auto-populate species fields from template recommendations
            digimal.species = selectedSchema.recommendedSpecies
                ? selectedSchema.recommendedSpecies
                : digimal.species;
            digimal.commonName = selectedSchema.recommendedCommonNames
                ? selectedSchema.recommendedCommonNames
                : digimal.commonName;
        }
    }
}
```

**User Experience**:
1. User creating new creature selects "Ball Python Health Log" template
2. System auto-fills:
   - Species: "Python regius"
   - Common Name: "Ball Python"
3. User can override if needed
4. Saves time and ensures consistency

#### Current Limitations & Future Features

**Current State: READ-ONLY**

The current API has **NO CREATE, UPDATE, or DELETE endpoints** for schemas!

**Existing Endpoints**:
```
✅ GET /schema              - List templates (user's + public)
✅ GET /schema/{id}         - Get single template
✅ GET /schema/{id}/creature/{creatureId}  - With creature context
✅ GET /creature/{creatureId}/schema       - Via creature's template
✅ GET /schema/{id}/genetics               - Extract genetics data
```

**Missing Operations**:
```
❌ POST /schema                 - Create new template
❌ PUT /schema/{id}             - Update template
❌ DELETE /schema/{id}          - Delete template
❌ POST /schema/{id}/clone      - Clone/fork template
❌ POST /schema/{id}/entries    - Add field to template
```

**Future Vision: Template Marketplace**

The platform envisions a template marketplace where:
- **Template Creators** (power users) design comprehensive templates and share with community
- **Template Consumers** (new users) browse marketplace by animal type and one-click install
- **Community Growth**: Users sharing knowledge through templates drives viral growth
- **Monetization**: Premium templates ($0.99-$4.99) with revenue sharing (70% creator, 30% platform)

**Planned Marketplace Features**:
1. **Phase 1**: Template library with categories, filters, and search
2. **Phase 2**: Ratings, reviews, and download tracking
3. **Phase 3**: Creator profiles and featured templates
4. **Phase 4**: Paid premium templates with revenue sharing

**Template Inheritance/Cloning**:
Users will be able to **fork** public templates and customize them:
```
"Ball Python Care Log" (public, by ProBreeder)
    ├─ Fields: Feeding, Weight, Shed, Health Notes
    │
    └─ User clones as "My Ball Python Log"
         ├─ Keeps: Feeding, Weight, Shed, Health Notes
         ├─ Adds: Breeding Notes, Genetics Calculator
         └─ Removes: Health Notes (doesn't track)
```

#### Real-World Use Cases

**Use Case 1: Reptile Breeder**
- Tracks 50+ ball pythons
- Uses public template "Ball Python Care" as base
- Clones and adds breeding-specific fields (pairing notes, egg count, morph outcomes)
- Quick-logs feedings from dashboard
- Reviews weight charts to identify feeding issues

**Use Case 2: Exotic Pet Veterinarian**
- Creates private "Veterinary Exam Log" template
- Fields: Visit date, weight, temperature, diagnosis, treatment plan
- Records exam data during visits
- Reviews weight/temp trends over time
- Exports data for clients

**Use Case 3: Hobbyist with Mixed Collection**
- Owns 3 geckos, 2 snakes, 1 frog
- Installs species-specific public templates from marketplace
- Each animal gets appropriate template
- Logs feeding, sheds, health per species requirements

#### Implementation Recommendations

**Priority 1: Template CRUD API Endpoints**
1. `POST /schema` - Let users create templates
2. `POST /schema/{id}/clone` - Fork existing templates
3. `POST /schema/{id}/entries` - Add fields
4. `PUT /schema/{id}` - Edit metadata
5. `DELETE /schema/{id}` - Remove templates

**Priority 2: Template Builder UI**
- Visual field editor with drag-drop
- Field type selector (text, numeric, choice, date)
- Chart configuration interface
- Preview mode
- Save/publish workflow

**Priority 3: Template Marketplace**
- Browse public templates by category
- Search and filter capabilities
- One-click template installation
- Download count tracking

**Database Enhancements Needed**:
```sql
-- Marketplace fields
ALTER TABLE LogSchemas ADD COLUMN description TEXT;
ALTER TABLE LogSchemas ADD COLUMN previewImage VARCHAR(500);
ALTER TABLE LogSchemas ADD COLUMN downloadCount INT DEFAULT 0;
ALTER TABLE LogSchemas ADD COLUMN isVerified BOOLEAN DEFAULT FALSE;
ALTER TABLE LogSchemas ADD COLUMN basedOnSchemaId UUID REFERENCES LogSchemas(Id);

-- Template ratings
CREATE TABLE TemplateRatings (
    Id UUID PRIMARY KEY,
    TemplateId UUID REFERENCES LogSchemas(Id),
    UserId UUID REFERENCES Users(Id),
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Review TEXT,
    Created TIMESTAMP DEFAULT NOW()
);
```

**Key Takeaway**: The Template/Schema System is the **killer feature** that differentiates MyDigimal from competitors. It transforms a generic animal tracker into a flexible platform adaptable to any species, use case, and user expertise level. Once template creation is live, the marketplace becomes the growth engine driving adoption through network effects.

### Log Entry
A single data point recorded for a creature:
- **Data**: Value, date, notes
- **Context**: Which schema entry, which creature
- **Relationships**: Correlation ID (batch), linked creatures
- **Ownership**: Who recorded it

### Groups
User-defined collections for organizing creatures:
- Example: "My Snakes", "Breeding Stock", "For Sale"

### Notifications
System or user-generated events requiring attention:
- Subscription changes
- Creature transfers
- Messages
- System announcements

### Account Plans
Tiered subscription model:
- **Free**: Basic features, limited creatures
- **Paid Tiers**: More creatures, advanced features
- **Billing**: Monthly or yearly

---

## Business Rules

### Creature Limits
- Each account plan has max creature count
- Enforced at creation time
- Archived creatures don't count toward limit

### Creature Status Lifecycle
```
Created (Alive) → Sick → Alive
                → Missing → Alive/Dead
                → Dead (terminal)
                → Archived (soft-delete)
```

### Feeding Schedule
- User defines cadence (e.g., "every 7 days")
- System calculates next feed date
- Reporting shows overdue feeds

### Log Entry Relationships
- Parent-child entries (nested data)
- Linked entries (multi-creature events)
- Correlation groups (batch operations)

### Pedigree Tracking
- Creatures can have sire (father) and dam (mother)
- Supports multi-generation tracking
- Enables genetics calculations

### Transfer Workflow
1. Owner initiates transfer via notification system
2. Recipient receives `DigimalAwaitingTransfer` notification
3. Recipient accepts/declines via `PUT /notification/{id}/{processType}`
4. On accept:
   - Creature ownership changes (`Owner` field updated)
   - `OwnerChange` event logged in `CreatureEvents` table
   - Both parties receive notifications (DigimalTransferredTo, DigimalTransferredFrom)
5. On decline:
   - Ownership remains unchanged
   - Both parties receive `DigimalTransferFailed` notifications

**Important Notes:**
- All historical log entries remain intact after transfer
- New owner can view all previous logs (logs associated with creature, not original owner)
- Log entries retain original creator in `Owner` field
- Only log creator can delete their own logs (authorization enforced)
- **Logs are NOT made immutable** on transfer - original creator can still delete
- Transfer creates an audit trail via CreatureEvents table

---

## User Personas

### Hobbyist
- Tracks 5-25 animals
- Basic logging (feeding, weight)
- Wants reminders for care tasks

### Breeder
- Tracks 50-200+ animals
- Advanced genetics tracking
- Pedigree management
- Sales tracking

### Rescue/Shelter
- High animal turnover
- Detailed health logs
- Transfer tracking
- Multi-user access (future)

### Enthusiast
- Detailed record keeping
- Chart visualization
- Historical data analysis
- Community sharing (future)

---

# PART 5: TECHNICAL DECISIONS & PATTERNS

## API Design Decisions

### Why Azure Functions?
- Serverless scaling
- Pay-per-execution pricing
- Built-in Azure service integration
- .NET ecosystem familiarity

### Why Dapper over Entity Framework?
- Performance (micro-ORM)
- Full SQL control
- Lightweight dependency
- No lazy-loading issues

### Why JWT Tokens?
- Stateless authentication
- Standard Claims for authorization
- Easy integration with Auth0
- Mobile-friendly

### Why PostgreSQL?
- Rich data types (JSON columns)
- Strong ACID guarantees
- Open source
- Neon provides serverless scaling

## Frontend Design Decisions

### Why Svelte?
- Small bundle size
- No virtual DOM overhead
- Reactive by default
- Easy to learn

### Why Flutter?
- Single codebase for iOS/Android
- Native performance
- Rich widget library
- Strong typing (Dart)

### Why Smelte?
- Material Design + Tailwind
- Pre-built components
- Responsive out-of-box

## Architecture Patterns Used

### Repository Pattern
Abstracts data access, enables testing:
```csharp
interface ICreatureRepository {
    Task<Creature> GetByIdAsync(Guid id);
    Task<IEnumerable<Creature>> GetByOwnerAsync(Guid userId);
    Task CreateAsync(Creature creature);
}
```

### Unit of Work
Manages transactions across multiple repositories:
```csharp
using (var uow = _unitOfWorkFactory.Create()) {
    await uow.Creatures.CreateAsync(creature);
    await uow.CreatureEvents.CreateAsync(event);
    await uow.CommitAsync();
}
```

### Factory Pattern
Creates complex objects (account plans, notifications):
```csharp
var plan = _accountPlanFactory.Create(user.AccountPlan);
var maxCreatures = plan.MaxCreatures;
```

### Strategy Pattern
Different notification processors per type:
```csharp
interface INotificationProcessor {
    Task ProcessAcceptAsync(Notification notification);
    Task ProcessDeclineAsync(Notification notification);
}
```

### Provider Pattern (Flutter)
Reactive state management:
```dart
Consumer<CreatureProvider>(
  builder: (context, provider, child) {
    return ListView.builder(
      itemCount: provider.creatures.length,
      itemBuilder: (context, index) => ...
    );
  }
)
```

---

# PART 6: DEVELOPMENT GUIDE

## Setting Up Backend (API)

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code
- Azure Functions Core Tools V4
- Azurite (Azure Storage Emulator)
- PostgreSQL client (optional, for DB access)

### Steps
1. Clone repository
2. Copy `settings.json` to `local.settings.json`
3. Configure connection strings and Auth0 settings
4. Install Azurite: `npm install -g azurite`
5. Start Azurite: `azurite --silent`
6. Run: `func start` or F5 in Visual Studio
7. API runs at `http://localhost:7071`

### Database Setup
- Neon PostgreSQL database already provisioned
- Run SQL scripts in `/database` folder (if exists)
- Or use existing database (already populated)

### Testing API
```bash
# Check authentication
curl http://localhost:7071/api/auth \
  -H "Authorization: Bearer {token}"

# Get creatures
curl http://localhost:7071/api/creature \
  -H "Authorization: Bearer {token}"
```

---

## Setting Up Svelte Frontend

### Prerequisites
- Node.js 14+
- npm or yarn

### Steps
1. Navigate to `/Users/jamesstuddart/Code/MyDigimal/src/digimal`
2. Install dependencies: `npm install`
3. Copy `.env.example` to `.env`
4. Set `gatewayUri=http://localhost:7071/api`
5. Run dev server: `npm run dev`
6. App runs at `http://localhost:5000`

### Auth0 Configuration
- Create Auth0 application (SPA)
- Configure callback URLs
- Set Auth0Domain and Auth0ClientId in `.env`

---

## Setting Up Flutter App

### Prerequisites
- Flutter SDK 3.x
- Xcode (for iOS)
- Android Studio (for Android)

### Steps
1. Navigate to `/Users/jamesstuddart/Code/MyDigimal-Mobile`
2. Install dependencies: `flutter pub get`
3. Configure API URL in `lib/config/api_config.dart`
4. Set Auth0 credentials in `lib/config/auth_config.dart`
5. Run: `flutter run`

### Platform-Specific Setup
**iOS:**
- Configure bundle ID
- Set up signing certificate
- Update `Info.plist` with Auth0 callback

**Android:**
- Update `applicationId` in `build.gradle`
- Configure Auth0 callback in `AndroidManifest.xml`

---

# PART 7: FRONTEND REBUILD STRATEGY

## Recommended Approach

Given that both frontends are incomplete (~60% Svelte, ~40% Flutter), **building a new frontend from scratch is recommended**. This allows:
- Modern tooling and frameworks
- Clean architecture without technical debt
- Consistent user experience
- Single platform focus initially

## Option 1: Modern React/Next.js SPA

### Why?
- Largest ecosystem and talent pool
- Excellent tooling (TypeScript, Tailwind, etc.)
- Server-side rendering (SEO benefits)
- Static generation for performance

### Tech Stack
- **Framework**: Next.js 14+ (App Router)
- **Language**: TypeScript
- **UI**: Tailwind CSS + shadcn/ui or Chakra UI
- **State**: Zustand or React Context
- **API**: React Query (caching, optimistic updates)
- **Auth**: @auth0/auth0-react
- **Forms**: React Hook Form + Zod validation
- **Charts**: Recharts or Chart.js
- **Icons**: Lucide React or FontAwesome

### Project Structure
```
app/
├── (auth)/
│   ├── login/
│   └── register/
├── (public)/
│   ├── page.tsx              # Landing
│   ├── privacy/
│   └── terms/
├── (dashboard)/
│   ├── layout.tsx            # Protected layout
│   ├── dashboard/
│   ├── creatures/
│   │   ├── page.tsx          # List
│   │   ├── [id]/             # Detail
│   │   └── new/              # Create
│   ├── logs/
│   ├── profile/
│   └── admin/
├── api/                      # API route handlers (if needed)
├── components/
│   ├── ui/                   # shadcn components
│   ├── creatures/
│   ├── logs/
│   └── layout/
├── lib/
│   ├── api/                  # API client
│   ├── stores/               # Zustand stores
│   ├── hooks/                # Custom hooks
│   └── utils/
└── types/                    # TypeScript types
```

### Implementation Phases

**Phase 1: Foundation (Week 1-2)**
- Next.js setup with TypeScript
- Auth0 integration
- API client with React Query
- Layout components (header, sidebar, footer)
- Protected route middleware

**Phase 2: Core Features (Week 3-5)**
- Creature list/detail/create/edit
- Log entry creation
- Schema-based form rendering
- Dashboard with widgets
- Profile management

**Phase 3: Advanced Features (Week 6-8)**
- Groups & tags
- Notifications
- Feeding schedule/reporting
- CSV import
- QR code generation
- Charts/visualization

**Phase 4: Polish (Week 9-10)**
- Responsive design
- Loading states
- Error handling
- Optimistic updates
- SEO optimization

---

## Option 2: Modern Mobile (Flutter or React Native)

### Why?
- Native mobile experience
- Push notifications
- Camera integration (QR scanning, photos)
- Offline capability

### Recommendation: React Native with Expo
- **Framework**: Expo (managed workflow)
- **Language**: TypeScript
- **UI**: NativeBase or React Native Paper
- **Navigation**: React Navigation
- **State**: Zustand
- **API**: React Query
- **Auth**: expo-auth-session + Auth0

### Project Structure
```
src/
├── navigation/
│   ├── AppNavigator.tsx
│   ├── AuthNavigator.tsx
│   └── DashboardNavigator.tsx
├── screens/
│   ├── auth/
│   ├── dashboard/
│   ├── creatures/
│   ├── logs/
│   └── profile/
├── components/
│   ├── creatures/
│   ├── logs/
│   └── ui/
├── services/
│   ├── api/
│   ├── storage/
│   └── notifications/
├── stores/
│   ├── authStore.ts
│   ├── creatureStore.ts
│   └── logStore.ts
├── types/
└── utils/
```

---

## Option 3: Hybrid (Web + Mobile with Shared Backend)

Build both:
1. **Next.js web app** for desktop users
2. **React Native mobile app** for on-the-go users
3. Share TypeScript types and API client logic

### Monorepo Structure
```
packages/
├── web/                      # Next.js app
├── mobile/                   # React Native app
├── shared/                   # Shared code
│   ├── types/                # TypeScript types
│   ├── api-client/           # API client
│   ├── utils/                # Utilities
│   └── validation/           # Zod schemas
└── api/                      # Backend (keep as-is)
```

---

## Recommended: Next.js Web App First

**Rationale:**
- Faster development (single platform)
- Easier testing and iteration
- Better for data-heavy CRUD operations
- Mobile web works well initially
- Can add native mobile later if needed

**Approach:**
1. Build responsive Next.js app
2. Ensure mobile-friendly (PWA capabilities)
3. Launch to users for feedback
4. If mobile usage is high, build dedicated native app later

---

# PART 8: API INTEGRATION GUIDE FOR FRONTEND DEVELOPERS

## Authentication Setup

### Auth0 Configuration
```typescript
// lib/auth0.ts
import { Auth0Client } from '@auth0/auth0-spa-js';

const auth0 = new Auth0Client({
  domain: 'azure-mydigimal.uk.auth0.com',
  clientId: process.env.NEXT_PUBLIC_AUTH0_CLIENT_ID!,
  authorizationParams: {
    redirect_uri: window.location.origin,
    audience: 'https://dev-api.mydigimal.com'
  }
});

export default auth0;
```

### API Client
```typescript
// lib/api/client.ts
import axios from 'axios';
import auth0 from '../auth0';

const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || 'https://dev-api.mydigimal.com'
});

// Add auth token to all requests
apiClient.interceptors.request.use(async (config) => {
  const token = await auth0.getTokenSilently();
  config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// Handle errors
apiClient.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      auth0.loginWithRedirect();
    }
    return Promise.reject(error);
  }
);

export default apiClient;
```

---

## Type Definitions

### Core Types
```typescript
// types/creature.ts
export enum CreatureSex {
  Unknown = 0,
  Male = 1,
  Female = 2
}

export enum CreatureStatus {
  Alive = 0,
  Missing = 1,
  Sick = 2,
  Dead = 3,
  Archived = 4
}

export enum FeedingCadenceType {
  Hours = 0,
  Days = 1,
  Weeks = 2,
  Months = 3
}

export interface Creature {
  id: string;
  name: string;
  commonName?: string;
  species?: string;
  morph?: string;
  genes?: string[];
  sex: CreatureSex;
  status: CreatureStatus;
  owner: string;
  logSchemaId?: string;
  born?: string;
  bornYear?: number;
  bornBy?: string;
  sire?: string;
  dam?: string;
  purchased?: string;
  purchasedFrom?: string;
  purchasedUrl?: string;
  image?: string;
  shortCode?: string;
  feedingCadence?: number;
  feedingCadenceType?: FeedingCadenceType;
  tags?: string;
  groupName?: string;
  created?: string;
  modified?: string;
}

export interface CreateCreatureDto {
  name: string;
  commonName?: string;
  species?: string;
  sex: CreatureSex;
  morph?: string;
  genes?: string[];
  logSchemaId?: string;
  born?: string;
  bornBy?: string;
  feedingCadence?: number;
  feedingCadenceType?: FeedingCadenceType;
  tags?: string;
  groupName?: string;
}
```

```typescript
// types/log.ts
export interface LogEntry {
  id: string;
  creatureId: string;
  logSchemaEntryId: string;
  date: string;
  notes?: string;
  value: string;
  correlationId?: string;
  owner: string;
}

export interface CreateLogEntryDto {
  logSchemaEntryId: string;
  value: string;
  date: string;
  notes?: string;
  correlationId?: string;
  linkEntryIds?: string[];
}

export interface CreateLogEntriesDto {
  entries: CreateLogEntryDto[];
}
```

```typescript
// types/schema.ts
export enum LogSchemaEntryType {
  Text = 0,
  Numeric = 1,
  Choice = 2,
  Date = 3,
  Boolean = 4
}

export enum ChartType {
  None = 0,
  HeatMap = 1,
  Line = 2
}

export interface LogSchemaEntry {
  id: string;
  schemaId: string;
  parentId?: string;
  icon?: string;
  title: string;
  type: LogSchemaEntryType;
  chartType: ChartType;
  values?: string[];
  defaultValue?: string;
  required: boolean;
  repeatLastEntry: boolean;
  quickAction: boolean;
}

export interface LogSchema {
  id: string;
  title: string;
  isPublic: boolean;
  author?: string;
  created: string;
  modified?: string;
  recommendedSpecies?: string[];
  recommendedCommonNames?: string[];
  genes?: Record<string, string[]>;
  morphs?: Record<string, string[]>;
  entries: LogSchemaEntry[];
}
```

```typescript
// types/user.ts
export enum AccountPlanType {
  Free = 1000,
  Starter = 2000,
  Hobbiest = 3000,
  Breeder = 4000,
  Ultra = 10000,
  EarlyAdopter = 20000
}

export enum PaymentPlanType {
  Monthly = 0,
  Yearly = 1
}

export interface UserProfile {
  name: string;
  avatar?: string;
  accountPlan: AccountPlanType;
  paymentPlan: PaymentPlanType;
  renewalMonth: number;
  renewalYear: number;
  role: string;
  accountPlanSettings: {
    maxCreatures: number;
  };
}
```

---

## API Service Functions

### Creature Service
```typescript
// lib/api/creatures.ts
import apiClient from './client';
import { Creature, CreateCreatureDto } from '@/types/creature';

export const creatureService = {
  async getAll(): Promise<Creature[]> {
    const { data } = await apiClient.get('/creature');
    return data;
  },

  async getById(id: string): Promise<Creature> {
    const { data } = await apiClient.get(`/creature/${id}`);
    return data;
  },

  async getByShortCode(shortCode: string): Promise<Creature> {
    const { data } = await apiClient.get(`/creature/sc/${shortCode}`);
    return data;
  },

  async create(creature: CreateCreatureDto): Promise<Creature> {
    const { data } = await apiClient.post('/creatures', creature);
    return data;
  },

  async update(id: string, creature: Partial<Creature>): Promise<Creature> {
    const { data } = await apiClient.put(`/creature/${id}`, creature);
    return data;
  },

  async delete(id: string): Promise<void> {
    await apiClient.delete(`/creature/${id}`);
  }
};
```

### Log Service
```typescript
// lib/api/logs.ts
import apiClient from './client';
import { LogEntry, CreateLogEntriesDto } from '@/types/log';

export const logService = {
  async create(creatureId: string, dto: CreateLogEntriesDto): Promise<void> {
    await apiClient.post(`/creature/${creatureId}/log`, dto);
  },

  async getByDateRange(
    creatureId: string,
    fromYear: number,
    toYear: number
  ): Promise<LogEntry[]> {
    const { data } = await apiClient.get(
      `/creature/${creatureId}/log/-/${fromYear}/${toYear}`
    );
    return data;
  },

  async getBySchemaEntry(
    creatureId: string,
    schemaEntryId: string,
    fromYear: number,
    toYear: number
  ): Promise<LogEntry[]> {
    const { data } = await apiClient.get(
      `/creature/${creatureId}/log/${schemaEntryId}/${fromYear}/${toYear}`
    );
    return data;
  },

  async delete(creatureId: string, entryId: string): Promise<void> {
    await apiClient.delete(`/creature/${creatureId}/log/${entryId}`);
  },

  async duplicate(creatureId: string, entryId: string): Promise<void> {
    await apiClient.post(`/creature/${creatureId}/log/duplicate/${entryId}`);
  }
};
```

### Schema Service
```typescript
// lib/api/schemas.ts
import apiClient from './client';
import { LogSchema } from '@/types/schema';

export const schemaService = {
  async getAll(): Promise<LogSchema[]> {
    const { data } = await apiClient.get('/schema');
    return data;
  },

  async getById(id: string): Promise<LogSchema> {
    const { data } = await apiClient.get(`/schema/${id}`);
    return data;
  },

  async getByCreature(
    schemaId: string,
    creatureId: string
  ): Promise<LogSchema> {
    const { data } = await apiClient.get(
      `/schema/${schemaId}/creature/${creatureId}`
    );
    return data;
  },

  async getCreatureSchema(creatureId: string): Promise<LogSchema> {
    const { data } = await apiClient.get(`/creature/${creatureId}/schema`);
    return data;
  },

  async getGenetics(schemaId: string): Promise<any> {
    const { data } = await apiClient.get(`/schema/${schemaId}/genetics`);
    return data;
  }
};
```

### Auth Service
```typescript
// lib/api/auth.ts
import apiClient from './client';
import { UserProfile } from '@/types/user';

export const authService = {
  async getUserDetails(): Promise<UserProfile> {
    const { data } = await apiClient.post('/auth/details');
    return data;
  },

  async isAuthed(): Promise<boolean> {
    try {
      await apiClient.get('/auth');
      return true;
    } catch {
      return false;
    }
  }
};
```

---

## React Query Hooks

### useCreatures
```typescript
// lib/hooks/useCreatures.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { creatureService } from '../api/creatures';
import { CreateCreatureDto } from '@/types/creature';

export function useCreatures() {
  return useQuery({
    queryKey: ['creatures'],
    queryFn: creatureService.getAll
  });
}

export function useCreature(id: string) {
  return useQuery({
    queryKey: ['creature', id],
    queryFn: () => creatureService.getById(id),
    enabled: !!id
  });
}

export function useCreateCreature() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (creature: CreateCreatureDto) =>
      creatureService.create(creature),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['creatures'] });
    }
  });
}

export function useUpdateCreature() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<Creature> }) =>
      creatureService.update(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['creatures'] });
      queryClient.invalidateQueries({ queryKey: ['creature', variables.id] });
    }
  });
}

export function useDeleteCreature() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => creatureService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['creatures'] });
    }
  });
}
```

---

## Example Components

### Creature List
```typescript
// components/creatures/CreatureList.tsx
'use client';

import { useCreatures } from '@/lib/hooks/useCreatures';
import CreatureCard from './CreatureCard';

export default function CreatureList() {
  const { data: creatures, isLoading, error } = useCreatures();

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error loading creatures</div>;

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      {creatures?.map(creature => (
        <CreatureCard key={creature.id} creature={creature} />
      ))}
    </div>
  );
}
```

### Create Creature Form
```typescript
// components/creatures/CreateCreatureForm.tsx
'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useCreateCreature } from '@/lib/hooks/useCreatures';
import { CreatureSex } from '@/types/creature';

const schema = z.object({
  name: z.string().min(1, 'Name is required'),
  commonName: z.string().optional(),
  species: z.string().optional(),
  sex: z.nativeEnum(CreatureSex),
  born: z.string().optional(),
  feedingCadence: z.number().optional(),
  tags: z.string().optional()
});

type FormData = z.infer<typeof schema>;

export default function CreateCreatureForm() {
  const { register, handleSubmit, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(schema)
  });
  const createCreature = useCreateCreature();

  const onSubmit = (data: FormData) => {
    createCreature.mutate(data, {
      onSuccess: () => {
        // Redirect or show success message
      }
    });
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div>
        <label>Name</label>
        <input {...register('name')} className="w-full border rounded p-2" />
        {errors.name && <p className="text-red-500">{errors.name.message}</p>}
      </div>

      <div>
        <label>Common Name</label>
        <input {...register('commonName')} className="w-full border rounded p-2" />
      </div>

      <div>
        <label>Sex</label>
        <select {...register('sex', { valueAsNumber: true })} className="w-full border rounded p-2">
          <option value={CreatureSex.Unknown}>Unknown</option>
          <option value={CreatureSex.Male}>Male</option>
          <option value={CreatureSex.Female}>Female</option>
        </select>
      </div>

      <button
        type="submit"
        disabled={createCreature.isPending}
        className="bg-blue-500 text-white px-4 py-2 rounded"
      >
        {createCreature.isPending ? 'Creating...' : 'Create Creature'}
      </button>
    </form>
  );
}
```

---

# PART 9: DEPLOYMENT GUIDE

## Backend Deployment (Azure Functions)

### Azure Resources Needed
1. **Function App** (Consumption or Premium plan)
2. **Storage Account** (for function app)
3. **Application Insights** (monitoring)
4. **PostgreSQL Database** (already on Neon)

### Deployment Steps
1. Create Function App in Azure Portal
2. Configure application settings (environment variables)
3. Deploy from Visual Studio or CLI:
   ```bash
   func azure functionapp publish <function-app-name>
   ```

### Environment Variables (Production)
```
FUNCTIONS_WORKER_RUNTIME=dotnet-isolated
NpgSqlConnectionConfig:Connection=[neon-connection-string]
Auth0Settings:AuthorityEndpoint=https://azure-mydigimal.uk.auth0.com/
Auth0Settings:Audience=https://api.mydigimal.com
EncryptionConfig:Key=[production-key]
CORS:Origins=https://mydigimal.com
```

---

## Frontend Deployment (Next.js)

### Option 1: Vercel (Recommended)
1. Connect GitHub repository
2. Configure environment variables
3. Deploy automatically on push

### Option 2: Azure Static Web Apps
1. Create Static Web App resource
2. Connect GitHub
3. Configure build settings

### Option 3: Self-hosted
1. Build: `npm run build`
2. Deploy to server
3. Configure nginx/Apache reverse proxy

### Environment Variables
```
NEXT_PUBLIC_API_URL=https://api.mydigimal.com
NEXT_PUBLIC_AUTH0_DOMAIN=azure-mydigimal.uk.auth0.com
NEXT_PUBLIC_AUTH0_CLIENT_ID=[client-id]
NEXT_PUBLIC_AUTH0_AUDIENCE=https://api.mydigimal.com
```

---

# PART 10: PROMPT FOR NEW LLM AGENT

## Copy-Paste Prompt for Frontend Development

```
# Project Context: MyDigimal Platform

You are tasked with building a modern frontend for the MyDigimal platform, an animal care tracking and management system.

## What is MyDigimal?
MyDigimal allows users to digitally manage their animal collections ("digimals") with features including:
- Creature management (CRUD operations)
- Customizable health/activity logging
- Feeding schedules and reminders
- Genetics/pedigree tracking
- Groups and tags for organization
- Account management with tiered plans

## Backend API
The backend is a fully functional Azure Functions API (.NET 9.0) with PostgreSQL database. It provides 30+ REST endpoints covering:
- Authentication (Auth0 JWT)
- Creature operations
- Logging with customizable schemas
- Groups, tags, notifications
- Reference data
- Admin functions

**API Base URL**: `https://dev-api.mydigimal.com` (or configure for local dev)

## Your Task
Build a **Next.js 14+ web application** (TypeScript, Tailwind CSS, shadcn/ui) that consumes this API and provides a complete user experience.

## Core Features to Implement

### Phase 1: Foundation
1. Auth0 integration (login/logout/session)
2. Protected routes middleware
3. API client with axios + React Query
4. Layout components (header, sidebar, navigation)
5. Responsive design (mobile-first)

### Phase 2: Core Features
1. **Dashboard**: Summary widgets, recent activity, upcoming feeds
2. **Creatures**:
   - List view (grid/table with filters)
   - Detail view (all info, tabs for logs/details)
   - Create form (validation with Zod)
   - Edit form
   - Delete/archive
3. **Logging**:
   - Schema-based form rendering
   - Log entry creation
   - History view with date filtering
   - Charts (line graphs, heat maps)
4. **Profile**: User settings, account details

### Phase 3: Advanced Features
1. Groups management
2. Tags management
3. Notifications panel
4. Feeding schedule/reporting
5. CSV import
6. QR code generation
7. Search/filter across creatures

## Technical Requirements

### Tech Stack
- **Framework**: Next.js 14+ (App Router)
- **Language**: TypeScript
- **UI**: Tailwind CSS + shadcn/ui
- **State**: Zustand or React Context
- **API**: React Query (TanStack Query)
- **Auth**: @auth0/auth0-react
- **Forms**: React Hook Form + Zod
- **Charts**: Recharts
- **Icons**: Lucide React

### Authentication
- Auth0 domain: `azure-mydigimal.uk.auth0.com`
- Audience: `https://dev-api.mydigimal.com`
- Token in Authorization header: `Bearer {token}`

### API Endpoints (Examples)
- `GET /creature` - List creatures
- `POST /creatures` - Create creature
- `GET /creature/{id}` - Get creature
- `PUT /creature/{id}` - Update creature
- `DELETE /creature/{id}` - Delete creature
- `POST /creature/{id}/log` - Create log entry
- `GET /creature/{id}/log/-/{fromYear}/{toYear}` - Get logs
- `GET /schema` - List log schemas
- `POST /auth/details` - Get user profile

### Data Models (Key Types)
Refer to the TypeScript type definitions in the documentation for:
- `Creature`, `CreateCreatureDto`
- `LogEntry`, `CreateLogEntryDto`
- `LogSchema`, `LogSchemaEntry`
- `UserProfile`
- Enums: `CreatureSex`, `CreatureStatus`, `FeedingCadenceType`

## Project Structure
```
app/
├── (auth)/login, register
├── (public)/landing, privacy, terms
├── (dashboard)/
│   ├── layout.tsx (protected)
│   ├── dashboard/
│   ├── creatures/[id], new
│   ├── logs/
│   ├── profile/
│   └── admin/
components/ui/ (shadcn)
components/creatures/, logs/, layout/
lib/api/ (services)
lib/hooks/ (React Query)
lib/stores/ (Zustand)
types/
```

## Development Guidelines
1. **Type Safety**: Strict TypeScript, no `any`
2. **Error Handling**: Toast notifications for errors
3. **Loading States**: Skeletons, spinners
4. **Validation**: Zod schemas for forms
5. **Accessibility**: Semantic HTML, ARIA labels
6. **Performance**: React Query caching, optimistic updates
7. **Mobile**: Responsive design, touch-friendly

## Key Business Rules
- Creatures have max limits based on account plan
- Archived creatures don't count toward limit
- Log entries can link multiple creatures (batch operations)
- Feeding schedules calculate next feed date automatically
- Short codes enable public sharing of creatures

## Available Documentation
You have access to comprehensive documentation covering:
- Complete API endpoint reference
- Data models and relationships
- Authentication flow
- Business logic patterns
- Example API service functions
- React Query hooks
- Example components

Refer to the full documentation file for detailed API contracts, type definitions, and integration examples.

## Getting Started
1. Set up Next.js project with TypeScript
2. Install dependencies (shadcn/ui, React Query, Auth0, etc.)
3. Configure Auth0 and API client
4. Implement authentication flow
5. Build layout components
6. Implement creature list (simplest feature)
7. Iterate through remaining features

Ask questions if you need clarification on:
- Specific API endpoints
- Data model relationships
- Business logic
- UI/UX decisions

Begin with Phase 1 (Foundation) and work incrementally through each phase.
```

---

# APPENDIX: ADDITIONAL RESOURCES

## Database Schema Diagram (Conceptual)
```
users ──┬── creatures ──┬── log_entries
        │               └── creature_events
        ├── creature_groups
        ├── notifications
        └── news (if admin)

log_schemas ──┬── log_schema_entries ──┬── log_entries
              └── creatures (FK)        └── (relationship)

creatures ─(self)→ sire/dam (pedigree)
```

## API Error Responses
All errors follow this format:
```json
{
  "error": "Error message",
  "statusCode": 400,
  "details": {
    "field": "validation error"
  }
}
```

Common status codes:
- `401` - Unauthorized (missing/invalid token)
- `403` - Forbidden (insufficient permissions)
- `404` - Not found
- `400` - Bad request (validation error)
- `500` - Internal server error

## Authentication Token Example
```json
{
  "iss": "https://azure-mydigimal.uk.auth0.com/",
  "sub": "auth0|12345",
  "aud": "https://dev-api.mydigimal.com",
  "iat": 1697500000,
  "exp": 1697586400,
  "custom:user_id": "uuid-of-user",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "0"
}
```

## Feature Comparison: Svelte vs Flutter

| Feature | Svelte (60%) | Flutter (40%) |
|---------|--------------|---------------|
| Authentication | ✅ Complete | ✅ Complete |
| Creature CRUD | ✅ Complete | ⚠️ Partial |
| Logging | ✅ Complete | ⚠️ Basic only |
| Groups | ✅ Complete | ❌ Missing |
| Tags | ✅ Complete | ❌ Missing |
| Notifications | ✅ Complete | ❌ Missing |
| Dashboard | ✅ Complete | ⚠️ Placeholder |
| CSV Import | ✅ Complete | ❌ Missing |
| QR Codes | ✅ Complete | ❌ Missing |
| Admin Panel | ✅ Complete | ❌ Missing |
| Charts | ⚠️ Basic | ❌ Missing |
| Offline Mode | ❌ Missing | ❌ Missing |

## Contact & Support
- **GitHub**: (if applicable)
- **Docs**: This file
- **API**: `https://dev-api.mydigimal.com`
- **Auth0 Tenant**: `azure-mydigimal.uk.auth0.com`

---

**Document Version**: 1.0
**Last Updated**: 2025-10-17
**Prepared For**: Frontend development by new LLM agent or developer