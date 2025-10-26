# 🏷️ Discount System

A modular .NET solution for managing and consuming discount codes.

---

## 🚀 Overview

This project provides a **high-performance discount code management system** designed for scalability and concurrency.  
It supports **bulk code generation**, **safe concurrent usage**, and **SignalR-based real-time communication** between services.

The solution is composed of multiple layers for separation of concerns and testability.

---

## 🧩 Architecture

| Layer | Description |
|-------|--------------|
| **Core** | Contains domain entities, value objects, enums, and application interfaces/commands. It defines the business logic contracts. |
| **Services** | Implements the application logic (`DiscountAppService`), including validation, caching, and persistence orchestration. |
| **Persistence.SqlServer** | Entity Framework Core–based repository implementation for SQL Server. Uses `DiscountDbContext` and mappings between domain and database models. |
| **RedisCache** | Implements distributed caching (`ICacheService`) using Redis for improved performance under high concurrency. |
| **SignalRServer** | A lightweight server that exposes the core services via **SignalR Hubs** for low-latency real-time communication. |
| **ApiServer** | REST API server for basic testing and integration, also used for validator demonstration. |
| **UnitTests** | Contains unit tests for the `DiscountAppService` and related components using `Moq` and `xUnit`. |

---

## ⚙️ Features

- ✅ Bulk discount code generation with uniqueness guarantee  
- ✅ Concurrency-safe code consumption (single-use enforcement)  
- ✅ Redis caching for faster repeated access  
- ✅ Layered architecture for clear separation of concerns  
- ✅ FluentValidation for input validation  
- ✅ SignalR real-time server for fast communication  
- ✅ Unit tests with Moq and xUnit

---

## 🧱 Technologies

- **.NET 8**
- **Entity Framework Core**
- **SignalR**
- **Redis (StackExchange.Redis)**
- **FluentValidation**
- **Mapster**
- **xUnit + Moq**

---

