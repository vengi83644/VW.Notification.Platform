# Smart Notification Delivery Platform

## 1. Introduction

This document outlines the architecture, design, and implementation details for the Smart Notification Delivery Platform. The platform is designed to receive events, process them through a rule-based routing engine, and deliver templated notifications via multiple channels. The system emphasizes a clean, modular, and extensible design using .NET 8, adhering to SOLID principles and Clean Architecture.

## 2. Architecture Overview

The platform follows a Clean Architecture approach, separating concerns into distinct layers. This promotes maintainability, testability, and scalability.
![image](https://github.com/user-attachments/assets/962ed6cd-fa30-48b1-ad7f-8713e213a9a0)


### 2.1. Layers

* **Domain Layer:**
    * Contains core business logic, entities, and interfaces.
    * Entities: `NotificationEvent`, `NotificationRequest`, `Rule`, `Customer`, `CustomerRule` `TemplateMessage`.
    * This layer has no dependencies on other layers.

* **Application Layer:**
    * Contains application-specific business rules and use cases.
    * Orchestrates data flow between the Domain and Infrastructure layers.
    * Defines interfaces implemented by the Infrastructure layer (e.g., `ICustomerRepository`, `IRuleRepository`, `ITemplateRepository`).
    * Example: `ProcessEventUseCase` which takes an event, finds matching rules, prepares notifications, and queues them for sending.

* **Infrastructure Layer:**
    * Implements interfaces defined in the Application layer.
    * Handles all external concerns:
        * **Notification Channels:** Simulated Email (`ConsoleEmailService`) and SMS (`ConsoleSmsService`). Concrete implementations for actual email/SMS providers would reside here.
        * **Templating Engine:** Integration with Razor. Templates could be stored as files or in a database.
        * **Persistence:** (Optional, but recommended for rules/templates) Entity Framework Core for database interactions if rules, templates, or logs need to be stored.
        * **Logging:** Integration with `Microsoft.Extensions.Logging`.
        * **Retry Mechanism:** Implementation of retry logic (e.g., using Polly).

* **Presentation Layer (API):**
    * ASP.NET Core Web API.
    * Exposes REST endpoints for event ingestion (e.g., `POST /api/events`).
    * Handles request/response mapping and validation.

## 3. Core Components and Flow

1.  **Event Ingestion (API):**
    * An external system sends an event (e.g., JSON payload) to `POST /api/events`.
    * The API controller validates the incoming event data.
    * The controller dispatches the event to an Application service/handler (e.g., `EventService.ProcessEventAsync(NotificationEvent)`).

2.  **Rule-Based Routing Engine (Application & Domain):**
    * The `EventService` retrieves applicable routing rules. Rules can be defined in a configuration file (e.g., `appsettings.json`), a database, or a dedicated rule store.
    * A `Rule` entity would typically contain:
        * `RuleId`
        * `Name`
        * `EventType`
        * `RuleTemplates`
    * The engine evaluates the event against the conditions of each rule.
    * User preferences (e.g., preferred channel, opt-out status) can be fetched and factored into the routing decision.

3.  **Templating (Infrastructure & Application):**
    * Once a rule matches and an action (channel + template) is determined, the `EventService` requests template rendering.
    * It calls an `ITemplateService` (implemented in Infrastructure) with the `templateId` and `NotificationTemplateData`.
    * The `ITemplateService` uses Razor or Liquid to render the message content, injecting event data into the template placeholders.

4.  **Notification Dispatch (Application & Infrastructure):**
    * The `EventService` creates a `NotificationRequest` (containing channel, recipient, subject, body).
    * It then uses a strategy factory to get the appropriate `INotificationService` implementation (e.g., `IEmailService`, `ISmsService`) based on the channel.
    * The selected `INotificationService` attempts to send the notification.

5.  **Channel Simulation (Infrastructure):**
    * `EmailService`: Writes email details (To, Subject, Body) to the console.
    * `SmsService`: Writes SMS details (To, Body) to the console.

6.  **Retry Mechanism (Infrastructure & Application):**
    * If a notification channel service fails to send a message (e.g., transient network issue), the sending attempt should be retried.
    * Polly library can be used to configure retry policies (e.g., exponential backoff, number of retries).
    * The `INotificationService` implementations would incorporate this retry logic.
    * For persistent failures, the system should log the error and potentially move the notification to a dead-letter queue or alert an administrator.

## 4. Technical Expectations Implementation

* **.NET 8:** The solution will be built using the latest stable .NET 8 SDK and runtime.
* **SOLID Principles:**
    * **SRP (Single Responsibility Principle):** Each class (e.g., `RazorViewEngine`, `EmailService`) has a single, well-defined responsibility.
    * **OCP (Open/Closed Principle):** The system is open for extension (e.g., adding new notification channels like `WhatsAppService` by implementing `INotificationService`) but closed for modification of existing, tested code.
    * **LSP (Liskov Substitution Principle):** Subtypes (e.g., `EmailService`, `ActualSmtpEmailService`) are substitutable for their base type (`IEmailService` or `INotificationService`).
    * **ISP (Interface Segregation Principle):** Interfaces are granular. For example, instead of one large `INotificationManager`, have smaller interfaces like `IEmailService`, `ISmsService`.
    * **DIP (Dependency Inversion Principle):** High-level modules (Application layer) depend on abstractions (interfaces), not on low-level modules (Infrastructure layer). Dependencies are injected.
* **Unit Tests:**
    * xUnit or MSTest will be used for unit testing.
    * Focus on testing:
        * Domain logic (e.g., rule condition parsing, entity validations).
        * Application service logic (e.g., correct orchestration of rule evaluation, templating, and dispatch).
        * Rule engine evaluation.
        * Template rendering logic (with mock data).
    * Mocking frameworks like Moq or NSubstitute will be used to isolate dependencies.
* **Logging:**
    * `Microsoft.Extensions.Logging` framework will be used throughout the application.
    * Key events, errors, and diagnostic information will be logged.
    * Log output can be configured (e.g., console, file, centralized logging system).

## 5. Setup and Run Instructions

1.  **Prerequisites:**
    * .NET 8 SDK installed.
    * Git.
    * An IDE like Visual Studio 2022 or VS Code.

2.  **Clone the Repository:**
    ```bash
    git clone <repository-url>
    cd SmartNotificationPlatform
    ```

3.  **Build the Solution:**
    ```bash
    dotnet build
    ```

4.  **Configure the Application:**
    * All entities can be defined in `appsettings.json`.
    * Message templates are available in the `Templates` folder.

5.  **Run the API:**
    ```bash
    cd src/SmartNotificationPlatform.Api
    dotnet run
    ```
    The API will typically start on `https://localhost:7xxx` or `http://localhost:5xxx`.

6.  **Test Event Ingestion:**
    * Use the Swagger UI to send a `POST` request to the `/api/events/send` endpoint.
    * Sample JSON payload can be found in `appsettings.json` under `notificationEventDTO`
    * Check the console output for simulated email/SMS messages.

## 6. Design Trade-offs and Extensibility

### 6.1. Design Trade-offs

* **Rule Engine Complexity:**
    * **Current:** Simple property matching (e.g., in `appsettings.json`).
    * **Alternative:** A more sophisticated rule engine (e.g., using a DSL, integrating a business rules engine like NRules) could offer more flexibility but increase complexity. The current approach prioritizes simplicity for this assignment.
* **Template Storage:**
    * **Current:** Likely file-based for simplicity.
    * **Alternative:** Database storage for templates would allow dynamic updates without redeployment but adds a database dependency.
* **Synchronous vs. Asynchronous Event Processing:**
    * **Current (Implied):** The API might process events synchronously upon receipt for simplicity.
    * **Alternative:** For high throughput, events could be queued (e.g., RabbitMQ, Azure Service Bus) after initial validation, and processed by background workers. This adds resilience and scalability but also complexity.
* **Templating Engine Choice (Razor):**
    * **Liquid:** Generally simpler syntax, sandboxed, good for user-defined templates.

### 6.2. Extensibility Notes

* **Adding New Notification Channels (e.g., Push, WhatsApp):**
    1.  Define a new interface if specific methods are needed (e.g., `IPushNotificationService`) or use the generic `INotificationService`.
    2.  Create a new service class in the `Infrastructure` layer (e.g., `FirebasePushNotificationService`, `TwilioWhatsAppService`) that implements the interface.
    3.  Implement the `SendNotificationAsync` method, integrating with the specific channel's API or SDK.
    4.  Update `NotificationStrategyFactory` to include the new service.
    5.  Update rule definitions to allow actions targeting the new channel.
    6.  The core `EventService` should not need modification due to OCP.

* **Adding New Rule Condition Types:**
    * Modify the `Rule` entity and the rule evaluation logic in the `Application` layer to support new condition structures or operators.

* **Changing Templating Engine:**
    * Create a new implementation of `ITemplateService`.
    * Update DI registration to use the new service. Templates would need to be in the new format.

* **Introducing a Database:**
    * Add Entity Framework Core to the `Infrastructure` project.
    * Define `DbContext` and entity configurations.
    * Implement repositories (e.g., `DbRuleRepository`, `DbTemplateRepository`) to store and retrieve rules, templates, and notification logs.

* **Scaling Out:**
    * If using a message queue for event processing, multiple instances of background workers can consume events from the queue.
    * The API layer can be scaled horizontally behind a load balancer.

## 7. Conclusion

This Smart Notification Delivery Platform is designed to be robust, maintainable, and extensible, meeting the core requirements of the assignment. By adhering to Clean Architecture and SOLID principles, the system provides a solid foundation for future enhancements and integration with various notification channels and business rules.

