# Portfol.io

![.NET Core](https://img.shields.io/badge/.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/Entity_Framework-339933?style=for-the-badge&logo=entityframework&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white)

Portfol.io is a dynamic, centralized Content Management System (CMS) designed specifically for software engineers. It bypasses the limitations of static portfolio templates by providing a secure admin dashboard to manage and present a professional brand, including technical projects, skills, coding profiles, and career milestones.

This repository was developed as part of the Advanced Internet Computing course, supervised and evaluated by Dr. Salah Safi.

## Authors

* **[Abdallah Tahboub](https://github.com/abdullatahboub)**
* **[Yaman Alrifai](https://github.com/yamanalrfai)**
* **Mohammed Alhamed**
* **[Yousef Al-Shishani](https://github.com/YousefKurchaloy)**
  

## Features

* **Admin CMS Dashboard:** Secure, authenticated backend for full CRUD operations on all portfolio content.
* **Dynamic Project Showcase:** Filter and display technical builds (e.g., APIs, mobile apps) dynamically linked to the technologies used to build them.
* **Skills Matrix:** Categorized representation of technical proficiencies (Backend, Frontend, AI, Systems, etc.).
* **Professional Timeline:** A chronological tracker for industry events, university milestones, and career experiences.
* **Coding Profiles Integration:** Dedicated section to highlight algorithmic problem-solving handles (e.g., Codeforces, AtCoder).
* **Visitor Contact System:** Front-end messaging form that feeds directly into the admin dashboard for easy recruiter communication.

## Database Architecture

The data model is managed via **Entity Framework Core** and utilizes comprehensive data annotations and schema configurations. Core entities include:

```mermaid
erDiagram
    ApplicationUser {
        int Id PK
        string Username
        string Email
        string PasswordHash
    }

    Project {
        int Id PK
        string Title
        string Description
        string RepositoryUrl
        datetime CompletionDate
        int ApplicationUserId FK
    }

    Skill {
        int Id PK
        string Name
        int Category "Enum: SkillCategory"
        int ProficiencyLevel
        int ApplicationUserId FK
    }

    ProjectSkill {
        int ProjectId PK, FK
        int SkillId PK, FK
    }

    TimelineEvent {
        int Id PK
        string Title
        string Organization
        string Location
        datetime StartDate
        datetime EndDate
        int ApplicationUserId FK
    }

    Credential {
        int Id PK
        string Name
        string IssuingAuthority
        datetime IssueDate
        string VerificationUrl
        int ApplicationUserId FK
    }

    CodingProfile {
        int Id PK
        string PlatformName
        string UserHandle
        int MaxRating
        int ApplicationUserId FK
    }

    ContactMessage {
        int Id PK
        string SenderName
        string SenderEmail
        string Subject
        string Body
        datetime SentDate
        boolean IsRead
        int ApplicationUserId FK
    }

    %% Core 1-to-Many Relationships tying the system to the Admin User
    ApplicationUser ||--o{ Project : "manages"
    ApplicationUser ||--o{ Skill : "possesses"
    ApplicationUser ||--o{ TimelineEvent : "experiences"
    ApplicationUser ||--o{ Credential : "earns"
    ApplicationUser ||--o{ CodingProfile : "owns"
    ApplicationUser ||--o{ ContactMessage : "receives"

    %% The Many-to-Many Relationship via the Junction Table
    Project ||--o{ ProjectSkill : "contains"
    Skill ||--o{ ProjectSkill : "used in"
```

The data model is managed via **Entity Framework Core** and utilizes comprehensive data annotations and schema configurations. Core entities include:

* **ApplicationUser:** Handles CMS authentication.
* **Project & Skill:** Connected via a **Many-to-Many** relationship, reflecting how real-world projects utilize multiple technologies, and specific skills apply to multiple projects.
* **TimelineEvent:** Tracks dates, locations, and details for professional milestones.
* **Credential:** Verifiable achievements and certificates.
* **CodingProfile:** Competitive programming statistics.
* **ContactMessage:** securely stores visitor inquiries.

## Getting Started

### Prerequisites
* [.NET 10.0 SDK](https://dotnet.microsoft.com/download) (or matching version)
* Visual Studio 2022 / JetBrains Rider / VS Code
* SQL Server

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/YousefKurchaloy/Portfol.io.git
   cd Portfol.io
   ```
2. **Update Database Connection**
   Configure your connection string in ```appsettings.json```.

4. **Apply EF Core Migrations**
   Open your Package Manager Console or terminal and run:
   ```bash
   dotnet ef database update
   ```
5. **Run the Application**
   ```bash
   dotnet run
   ```
