# Transcript Analyzer

## Table of Contents

- [Transcript Analyzer](#transcript-analyzer)
  - [Table of Contents](#table-of-contents)
  - [Project Overview](#project-overview)
  - [Definitions](#definitions)
  - [Overview of the transcript process](#overview-of-the-transcript-process)
  - [Architecture](#architecture)
    - [N-Tier Application Structure](#n-tier-application-structure)


## Project Overview
The purpose of this project is to create a system of front-end web apps, APIs, cloud jobs, and other tools that enables an application that analyzes IRS transcripts for individuals or businesses using the required tax payer id value. The application will retrieve the required authorization from the user, file that authorization with the IRS, and then download the transcript information from the IRS APIs. Once transcripts are downloaded and stored, the transcript can then be analyzed by one or more analyzer jobs that will use specific parts of the overall transcript to provide information to users. That information can be used for various business proceses such as loan qualification, income analysis, scanning for past or potential future tax issues, etc. Different users will have access to different analysis data pending need, contract setup, etc.

## Definitions

* IRS - The United States Internal Revenue service. The federal agency responsibile for tax code enforcement within the jurisdiction of the United States.
* DOR - Department of Revenue. The state agencies that provide tax code enforcement for individual states.
* TDS - Transcript Delivery System. An online system provided by the IRS for retrieving transcripts.
* CAF - Centralized Authorization File - IRS case management system that links an authorization- 8821- with a taxpayer account that allows use of TDS and other personal info, eg. copies of IRS notices.
* Tax Payer - An individual or business that has an account with the IRS.
* Service Provider - A professional providing a tax services to a tax payer.

## Overview of the transcript process

Process Modules:

1. Authorization acquisition
   1. Get a tax payer to file IRS Form 8821 (https://www.irs.gov/pub/irs-pdf/f8821.pdf) to grant access to Tax Information to the service provider.
      1. Gather an e-signature for the tax payer using an online form
      2. Currently, an 8821 grants access to tax payer information only to an individual service provider, not a tax services firm.
2. Submit the individual 8821 forms to the IRS CAF
3. Confirm form 8821 CAF received
   1. Manual process at IRS
   2. Confirmation by record in tax pro account and CAF check pass
4. Access TDS through IRS e-Services/transcript API
   1. Download a batch of transcripts from IRS e-services
   2. Resolve any transcript download errors
   3. Store transcripts for later analysis
5. Extract transcript data into datasets
   1. Individual analyzer jobs will analyze sections of the transcripts for specific business cases.
   2. Each analysis must be version controlled to provide full tracking history.
   3. Different transcripts for different years may require differnt mapping and analysis rules.
6. Transcript analysis - possible use cases
   1. Tax history and return filing
   2. Marketing
   3. Lending and underwriting
   4. Income analysis
   5. Tax issue resolution
7. Transcript outputs/notifications
   1. Different pending user access level, contractual requirements, etc.
   

## Local Development

For local development setup using Docker, see [docker/README.md](./docker/README.md).

### Quick Start

```bash
cd docker
cp .env.template .env
./scripts/up.sh        # Start infrastructure
./scripts/up.sh full   # Or start full stack
```

## Architecture

### N-Tier Application Structure

**Frontend: Next.js Applications**
- **Framework**: Next.js 14 with TypeScript and React 18
- **Routing**: App Router with route groups for feature organization
- **State Management**: Redux Toolkit with RTK Query for API communication
- **UI Components**: Material-UI (MUI)
- **Aurhorization**: Auth0/OCTA credentials management and login integration

**Backend: .NET Core API**
- **Framework**: .NET 10.0 Web API using minimal API pattern
- **Architecture**: Clean architecture (https://github.com/jasontaylordev/CleanArchitecture/blob/main/README.md)
- **Database**: Azure SQL Server via Entity Framework Core 10.0 and Dapper
- **Authentication**:
  - JWT Bearer tokens for user authentication
- **Cloud Storage**: Azure Blob Storage integration

**Cloud Services: Azure
