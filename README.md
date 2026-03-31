# E-Commerce Project (Microservices Architecture For Distributed System)

# Inroduction

This project implements a distributed, containerized backend for a simplified e-commerce platform using a microservices architecture. Each service is independently deployable, owns its own data, and communicates with peers through well-defined contracts with synchronous HTTP calls.

The system is composed of four microservices: Product Service, Customer Service, Order Service, and Payment Service. All services are containerized with Docker and orchestrated via Docker Compose, allowing the entire system to start with a single command: docker compose up --build.

Key design goals of this project:
• Loose coupling — services never share a database or internal models
• Data ownership — each service has its own SQLite database and EF Core DbContext
• Async-first — all EF Core queries use async/await patterns
• Observable — Swagger UI is enabled on every service for easy testing
