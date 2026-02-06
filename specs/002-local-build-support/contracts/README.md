# API Contracts: Local Build Support

**Feature**: 002-local-build-support
**Date**: 2026-02-05

## Not Applicable

This feature is infrastructure tooling for the local development environment. It does not introduce any new API endpoints.

The Docker development environment will host the existing API defined in the main project. No additional API contracts are required for this feature.

## Related Contracts

The following existing API contracts will be accessible through the Docker environment:

- **Health Check**: `GET /health` - Standard health endpoint
- **API v1**: `/api/v1/*` - All existing API endpoints

These contracts are defined in the main project specification (001-mvp-foundation).
