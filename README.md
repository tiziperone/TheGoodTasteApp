# 🍽️ The Good Taste App

Sistema de escritorio desarrollado en **C# (.NET)** diseñado para la administración operativa, gestión de catálogo de productos, registro de pedidos y control de inventario/ventas para emprendimientos gastronómicos.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0%20%2F%20Windows%20Forms%20%2F%20WPF-purple)
![C#](https://img.shields.io/badge/C%23-12.0-blue)
![Database](https://img.shields.io/badge/Database-PostgreSQL%20%2F%20SQL%20Server-blue)

---

## 📌 Descripción del Proyecto

**The Good Taste App** nace de la necesidad de centralizar y automatizar los flujos de trabajo operativos de un negocio gastronómico artesanal. La aplicación permite a los administradores registrar productos, controlar el stock disponible, gestionar órdenes de compra y mantener la trazabilidad de clientes y ventas mediante una interfaz ágil y persistencia relacional.

---

## ✨ Características Principales

- **Gestión de Catálogo e Inventario:** Altas, bajas, modificaciones y consultas (CRUD) de productos, categorías y stock en tiempo real.
- **Toma y Seguimiento de Pedidos:** Registro ágil de comandas/pedidos, asignación de estados (pendiente, en preparación, entregado) y cálculo automático de importes.
- **Administración de Clientes:** Agenda y registro de historial de compras.
- **Arquitectura en Capas:** Desacoplamiento entre la interfaz visual, la lógica de negocio y el acceso a datos.
- **Persistencia Transaccional:** Control de integridad referencial y manejo seguro de transacciones con base de datos relacional.

---

## 🏗️ Arquitectura del Sistema

El proyecto sigue una arquitectura modular en **N Capas** para garantizar mantenibilidad, testabilidad y separación de responsabilidades:

```text
TheGoodTasteApp/
├── src/
│   ├── Presentation/     # Capa de Interfaz de Usuario (Windows Forms / WPF)
│   ├── BusinessLogic/    # Reglas de negocio, validaciones y servicios
│   ├── DataAccess/       # Repositorios, conexiones DB, mapeo de entidades y consultas SQL
│   └── Entities/         # Modelos de dominio (Producto, Pedido, Cliente, etc.)
└── README.md
