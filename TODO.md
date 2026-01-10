# Zako IssueTracker Refactoring TODO

> **Goal**: Transform zako-issuetracker into a modern, modular Discord bot using Discord.Net best practices while maintaining Docker support and environment variable configuration. 

---

## Phase 1: Issue #33 - Attribute Refactoring

### Setup
- [ ] Create `src/Attributes/` directory
- [ ] Create `src/Extensions/` directory (if not exists)

### Implementation
- [ ] **Create** `src/Attributes/EnumAttributes.cs`
  - [ ] `DisplayNameAttribute` class
  - [ ] `DescriptionAttribute` class
  - [ ] `DatabaseStorageAttribute` class
  - [ ] `DatabaseStorageType` enum

- [ ] **Create** `src/Extensions/EnumExtensions. cs`
  - [ ] `GetDisplayName<T>()` extension method
  - [ ] `GetDescription<T>()` extension method
  - [ ] `ToStorageValue<T>()` extension method

- [ ] **Update** `Program.cs` - Decorate enums
  - [ ] Add attributes to `IssueTag` enum (Bug, Feature, Enhancement)
  - [ ] Add attributes to `IssueStatus` enum (Proposed, Approved, Rejected, Deleted)
  - [ ] Add emoji display names
  - [ ] Add descriptions

- [ ] **Update** `src/Issue/IssueSystem.cs`
  - [ ] Replace `.ToString()` with `.ToStorageValue()` (8 locations)
  - [ ] Line 39: `tag.ToString()` → `tag.Value. ToStorageValue()`
  - [ ] Line 64: `newStatus.ToString()` → `newStatus.ToStorageValue()`
  - [ ] Line 78-79: Tag/status conversions

- [ ] **Update** `Program.cs` - Display enums
  - [ ] Lines 459-460: Use `.GetDisplayName()` instead of `.ToString()`
  - [ ] Lines 70-77: Update slash command choices with display names

- [ ] **Update** `src/Commands/IssueListEmbed.cs`
  - [ ] Lines 32-33: Use `.GetDisplayName()` for Tag and Status

### Testing
- [ ] Run existing tests - verify they still pass
- [ ] Test Discord embeds show emoji display names
- [ ] Verify database storage unchanged (still using strings)

---

## Phase 2: Architecture Refactoring - Infrastructure

### Dependencies
- [ ] **Update** `zako-issuetracker.csproj`
  - [ ] Add `Discord.Net. Interactions` v3.15.0
  - [ ] Add `Microsoft.Extensions. Hosting` v8.0.0
  - [ ] Add `Microsoft.Extensions.DependencyInjection` v8.0.0
  - [ ] Add `Microsoft.Extensions. Logging` v8.0.0
  - [ ] Add `Microsoft.Extensions. Logging.Console` v8.0.0
  - [ ] Keep existing packages (Discord.Net, Microsoft.Data.Sqlite)

### Project Structure
- [ ] Create `src/Configuration/` directory
- [ ] Create `src/Services/` directory
- [ ] Create `src/Modules/` directory
- [ ] Create `src/Handlers/` directory
- [ ] Create `src/Utilities/` directory (if not exists)

### Configuration
- [ ] **Create** `src/Configuration/EnvironmentConfig.cs`
  - [ ] Read `DISCORD_TOKEN` (required)
  - [ ] Read `SQLITE_FILE` (required)
  - [ ] Read `IMG_LINK` (optional, default value)
  - [ ] Read `ADMIN_IDS` (comma-separated)
  - [ ] Read `EMBED_PAGE_SIZE` (default: 5)
  - [ ] `IsAdmin(userId)` helper method

---

## Phase 3: Services Layer

### Core Services
- [ ] **Create** `src/Services/DiscordStartupService.cs`
  - [ ] Implement `IHostedService`
  - [ ] Constructor:  inject `DiscordSocketClient`, `EnvironmentConfig`, `ILogger`
  - [ ] `StartAsync()`: Login and start bot
  - [ ] `StopAsync()`: Logout and stop bot
  - [ ] `OnLogAsync()`: Map Discord logs to ILogger

- [ ] **Create** `src/Services/InteractionHandlingService.cs`
  - [ ] Implement `IHostedService`
  - [ ] Constructor: inject `DiscordSocketClient`, `InteractionService`, `IServiceProvider`, `ILogger`
  - [ ] `StartAsync()`: Register event handlers, discover modules
  - [ ] `OnReadyAsync()`: Register commands globally
  - [ ] `OnInteractionAsync()`: Route interactions to handlers
  - [ ] `OnLogAsync()`: Interaction logging
  - [ ] **Automatic module discovery via reflection**

- [ ] **Create** `src/Services/DatabaseInitializationService.cs`
  - [ ] Implement `IHostedService`
  - [ ] Constructor: inject `EnvironmentConfig`, `ILogger`
  - [ ] `StartAsync()`: Create DB directory, file, tables
  - [ ] Create `zako` table (if not exists)
  - [ ] Create `zakonim` table (if not exists)
  - [ ] Error handling and logging

- [ ] **Create** `src/Services/IssueDataService.cs`
  - [ ] Refactor from static `IssueData` class
  - [ ] Constructor: inject `EnvironmentConfig`
  - [ ] `StoreIssueAsync()` - instance method
  - [ ] `UpdateIssueStatusAsync()` - instance method
  - [ ] `ListOfIssueAsync()` - instance method
  - [ ] `GetIssueByIdAsync()` - instance method
  - [ ] `DeleteIssueAsync()` - instance method
  - [ ] **Make all methods injectable (no more static! )**

---

## Phase 4: Command Modules

### Module Files
- [ ] **Create** `src/Modules/IssueModule.cs`
  - [ ] Inherit from `InteractionModuleBase<SocketInteractionContext>`
  - [ ] Constructor: inject `IssueDataService`, `EnvironmentConfig`, `ILogger`
  - [ ] `[SlashCommand("issue-new")]` - Show modal
  - [ ] `[SlashCommand("issue-get")]` - Get by ID
  - [ ] `[SlashCommand("issue-list")]` - List with filters
  - [ ] `[SlashCommand("issue-export")]` - Export JSON
  - [ ] Helper:  `BuildIssueEmbed()` - reusable embed builder

- [ ] **Create** `src/Modules/AdminModule.cs`
  - [ ] Inherit from `InteractionModuleBase<SocketInteractionContext>`
  - [ ] Constructor: inject `IssueDataService`, `EnvironmentConfig`, `ILogger`
  - [ ] `[SlashCommand("issue-set-status")]` - Change status (admin check)
  - [ ] `[SlashCommand("issue-delete")]` - Delete issue (admin check)
  - [ ] Helper: `IsAdmin()` check using `EnvironmentConfig`

- [ ] **Create** `src/Modules/FunModule.cs`
  - [ ] Inherit from `InteractionModuleBase<SocketInteractionContext>`
  - [ ] Constructor: inject `EnvironmentConfig`, `ILogger`
  - [ ] `[SlashCommand("zakonim")]` - Fun command
  - [ ] SQLite interaction for zakonim table
  - [ ] Error handling for duplicate entries

---

## Phase 5: Interaction Handlers

### Modal Handlers
- [ ] **Create** `src/Handlers/ModalHandlers.cs`
  - [ ] Inherit from `InteractionModuleBase<SocketInteractionContext>`
  - [ ] Constructor: inject `IssueDataService`, `ILogger`
  - [ ] `[ModalInteraction("ISSUE_MODAL")]` handler
  - [ ] Parse modal components (title, tag, detail)
  - [ ] Validate tag input
  - [ ] Call `IssueDataService.StoreIssueAsync()`
  - [ ] Send success/failure embed

### Component Handlers
- [ ] **Create** `src/Handlers/ComponentHandlers.cs`
  - [ ] Inherit from `InteractionModuleBase<SocketInteractionContext>`
  - [ ] Constructor: inject `IssueDataService`, `EnvironmentConfig`, `ILogger`
  - [ ] `[ComponentInteraction("issue-previous")]` - Previous page
  - [ ] `[ComponentInteraction("issue-next")]` - Next page
  - [ ] Parse current page from embed
  - [ ] Parse tag filter from embed
  - [ ] Update embed with new page
  - [ ] Boundary checks (first/last page)

---

## Phase 6: Update Program.cs

### Refactor Main Entry Point
- [ ] **Replace** `Program.cs` entirely
  - [ ] Keep only `Main()` method (~50 lines total)
  - [ ] Setup `Host.CreateDefaultBuilder()`
  - [ ] **NO** `ConfigureAppConfiguration` (no JSON files!)
  - [ ] `ConfigureServices`:
    - [ ] Register `EnvironmentConfig` (singleton)
    - [ ] Register `DiscordSocketConfig` (singleton)
    - [ ] Register `DiscordSocketClient` (singleton)
    - [ ] Register `InteractionService` (singleton)
    - [ ] Register `IssueDataService` (singleton)
    - [ ] Register hosted services: 
      - [ ] `DatabaseInitializationService`
      - [ ] `InteractionHandlingService`
      - [ ] `DiscordStartupService`
  - [ ] `ConfigureLogging`:
    - [ ] Clear providers
    - [ ] Add console logging
    - [ ] Filter Microsoft/System logs
    - [ ] Set minimum log level
  - [ ] Call `await host.RunAsync()`

### Cleanup Old Code
- [ ] **Remove** from `Program.cs`:
  - [ ] All event handler methods (590+ lines)
  - [ ] `ReadyAsync()` - moved to `InteractionHandlingService`
  - [ ] `InteractionCreatedAsync()` - moved to handlers
  - [ ] `MessageReceivedAsync()` - not needed
  - [ ] `LogAsync()` - moved to services
  - [ ] Manual `SlashCommandBuilder` code
  - [ ] All switch-case statements

---

## Phase 7: Backwards Compatibility

### Mark Old Code as Obsolete
- [ ] **Update** `src/EnvLoader.cs`
  - [ ] Add `[Obsolete]` attributes to all methods
  - [ ] Add message:  "Use EnvironmentConfig service instead"
  - [ ] Keep for backwards compatibility

- [ ] **Update** `src/Issue/IssueSystem.cs`
  - [ ] Add `[Obsolete]` to static methods
  - [ ] Add message: "Use IssueDataService instead"
  - [ ] Keep for external dependencies (if any)

---

## Phase 8: Docker & Deployment

### Docker Configuration
- [ ] **Verify** `Dockerfile` still works (no changes needed!)
- [ ] **Verify** `compose.yaml` still works (no changes needed!)
- [ ] **Update** `.env. example` (if needed)
  - [ ] Document all environment variables
  - [ ] Add comments for new developers

### Docker Testing
- [ ] Build Docker image:  `docker build -t zako-issuetracker .`
- [ ] Test with docker-compose: `docker-compose up`
- [ ] Verify environment variables are read correctly
- [ ] Test all slash commands in Discord
- [ ] Test modals and buttons
- [ ] Test admin commands
- [ ] Verify database persistence

---

## Phase 9: Testing & Validation

### Unit Testing
- [ ] **Update** `zako-issuetracker.Tests/EnvLoaderTests.cs`
  - [ ] Create tests for `EnvironmentConfig`
  - [ ] Test missing required variables
  - [ ] Test default values

- [ ] **Update** `zako-issuetracker.Tests/IssueSystemTests.cs`
  - [ ] Adapt tests for `IssueDataService` (instance-based)
  - [ ] Test dependency injection

- [ ] **Create** `zako-issuetracker.Tests/ServiceTests.cs`
  - [ ] Test `DatabaseInitializationService`
  - [ ] Test `IssueDataService`
  - [ ] Mock environment config

### Integration Testing
- [ ] Test bot startup sequence
- [ ] Test command registration
- [ ] Test all slash commands: 
  - [ ] `/issue-new` - Modal appears
  - [ ] `/issue-get` - Fetches issue
  - [ ] `/issue-list` - Shows paginated list
  - [ ] `/issue-export` - Exports JSON
  - [ ] `/issue-set-status` - Admin only
  - [ ] `/issue-delete` - Admin only
  - [ ] `/zakonim` - Fun command
- [ ] Test modal submission
- [ ] Test pagination buttons
- [ ] Test admin permission checks
- [ ] Test error handling

---

## Phase 10: Documentation

### Documentation
- [ ] **Update** `README.md`
  - [ ] Document new architecture
  - [ ] Update setup instructions
  - [ ] Add environment variable documentation
  - [ ] Add development guide
  - [ ] Add Docker deployment guide

- [ ] **Create** `ARCHITECTURE.md`
  - [ ] Explain service-based architecture
  - [ ] Document dependency injection
  - [ ] Explain module system
  - [ ] Show interaction flow diagram

- [ ] **Create** `MIGRATION.md`
  - [ ] Document breaking changes
  - [ ] Migration guide from old architecture
  - [ ] Compatibility notes

- [ ] **Update** `TODO.md` (this file!)
  - [ ] Mark completed tasks
  - [ ] Add future improvements
     
---

## Phase 11: Deployment & Monitoring

### Deployment
- [ ] Deploy to production environment
- [ ] Verify environment variables set correctly
- [ ] Monitor logs for errors
- [ ] Test all commands in production

### Monitoring
- [ ] Check ILogger output
- [ ] Monitor database file size
- [ ] Monitor bot latency
- [ ] Check command usage stats

---

## References

- [Discord.Net Documentation](https://docs.discordnet.dev/)
- [Discord.Net. Interactions Guide](https://docs.discordnet.dev/guides/int_framework/intro.html)
- [Aux/Discord.Net-Example Repository](https://github.com/Aux/Discord.Net-Example)
- [Microsoft.Extensions. Hosting Documentation](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)
