# OrderManagement - Order Management System
<img width="1871" height="1061" alt="OrderManagement" src="https://github.com/user-attachments/assets/52ce7101-554d-4b69-983c-bf6ff4a67b31" />

Microservices system for order management with rigorous stock validation.

## Architecture

```
                              RabbitMQ
                                 |
    +----------------------------+----------------------------+
    |                            |                            |
    v                            v                            v
+-----------+            +--------------+            +-------------+
|  Products |  ------>   |  Inventory   |  <------   |   Orders    |
|  Service  |  Event     |   Service    |   Event    |   Service   |
| (MongoDB) |            | (SQL Server) |            | (SQL Server)|
+-----------+            +--------------+            +-------------+
```

### Services

| Service | Port | Database | Description |
|---------|------|----------|-------------|
| Orders API | 5001 | SQL Server (OrdersDb) | Order management |
| Inventory API | 5002 | SQL Server (InventoryDb) | Stock control |
| Products API | 5003 | MongoDB | Product catalog |

### Communication Flow

1. **Product Creation**: Products Service publishes `ProductCreatedEvent` -> Inventory Service creates stock item
2. **Order Creation**: Orders Service saves order with status `PENDING` and publishes `OrderCreatedEvent`
3. **Validation**: Inventory Service validates stock and publishes `OrderInventoryValidated`
4. **Update**: Orders Service updates status to `APPROVED` or `OUT_OF_STOCK`

## Prerequisites

- Docker and Docker Compose

## Running with Docker

### Start all services

```bash
docker-compose up -d --build
```

This will start:
- SQL Server (port 1433)
- MongoDB (port 27017)
- RabbitMQ (ports 5672/15672)
- Orders API (port 5001)
- Inventory API (port 5002)
- Products API (port 5003)

### Check container status

```bash
docker-compose ps
```

### View service logs

```bash
docker-compose logs -f orders-api inventory-api products-api
```

### Stop all services

```bash
docker-compose down
```

## Local Execution (Development)

### 1. Start infrastructure

```bash
docker-compose up -d sqlserver mongodb rabbitmq
```

### 2. Run .NET services

In separate terminals:

```bash
# Terminal 1 - Products Service
cd src/OrderManagement.Products/OrderManagement.Products.API
dotnet run

# Terminal 2 - Inventory Service
cd src/OrderManagement.Inventory/OrderManagement.Inventory.API
dotnet run

# Terminal 3 - Orders Service
cd src/OrderManagement.Orders/OrderManagement.Orders.API
dotnet run
```

## Configuration

### Default Connections

| Service | Connection |
|---------|------------|
| SQL Server | localhost:1433 (sa/Order@123456) |
| RabbitMQ | localhost:5672 (guest/guest) |
| RabbitMQ UI | http://localhost:15672 |
| MongoDB | localhost:27017 |
| Mongo Express | http://localhost:8081 (admin/admin) |

## API Endpoints

### Products Service (http://localhost:5003)

```bash
# Create product (automatically creates stock item via event)
POST /api/products
{
  "title": "Notebook Dell",
  "description": "Notebook Dell Inspiron 15",
  "price": 3500.00,
  "brand": "Dell",
  "quantity": 10
}

# Get product
GET /api/products/{id}
```

### Inventory Service (http://localhost:5002)

```bash
# Get stock by SKU
GET /api/inventory/{sku}

# Update stock quantity
PUT /api/inventory/{sku}
{
  "sku": "DEL-NOT-12345678",
  "quantity": 50
}
```

### Orders Service (http://localhost:5001)

```bash
# Create order
POST /api/orders
{
  "items": [
    { "sku": "DEL-NOT-12345678", "quantity": 2 },
    { "sku": "APP-IPH-87654321", "quantity": 1 }
  ]
}

# Get order by ID
GET /api/orders/{id}

# List all orders
GET /api/orders
```

## Usage Example

### 1. Create a product

```bash
curl -X POST http://localhost:5003/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Notebook",
    "description": "Notebook Dell Inspiron",
    "price": 3500.00,
    "brand": "Dell",
    "quantity": 10
  }'
```

### 2. Check automatically created stock

```bash
curl http://localhost:5002/api/inventory/DEL-NOT-XXXXXXXX
```

### 3. Create an order

```bash
curl -X POST http://localhost:5001/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "items": [
      { "sku": "DEL-NOT-XXXXXXXX", "quantity": 2 }
    ]
  }'
```

### 4. Check order status

```bash
curl http://localhost:5001/api/orders/1
```

**Response (APPROVED):**
```json
{
  "orderId": 1,
  "status": "APPROVED",
  "validationReason": null,
  "createdAt": "2024-01-15T10:30:00",
  "updatedAt": "2024-01-15T10:30:01",
  "items": [
    { "sku": "DEL-NOT-XXXXXXXX", "quantity": 2 }
  ]
}
```

**Response (OUT_OF_STOCK):**
```json
{
  "orderId": 2,
  "status": "OUT_OF_STOCK",
  "validationReason": "SKU 'ABC-XYZ-12345678' insufficient stock (requested: 100, available: 10)",
  "createdAt": "2024-01-15T10:35:00",
  "updatedAt": "2024-01-15T10:35:01",
  "items": [
    { "sku": "ABC-XYZ-12345678", "quantity": 100 }
  ]
}
```

## Order Status

| Status | Description |
|--------|-------------|
| PENDING | Order created, awaiting stock validation |
| APPROVED | All items available, stock reserved |
| OUT_OF_STOCK | One or more items unavailable |

## Technologies

- .NET 8.0
- Entity Framework Core 9.0
- MongoDB Driver 3.6
- RabbitMQ Client 7.2
- SQL Server
- Docker / Docker Compose

## Project Structure

```
OrderManagement/
├── src/
│   ├── OrderManagement.Orders/
│   │   ├── OrderManagement.Orders.API/
│   │   ├── OrderManagement.Orders.Application/
│   │   ├── OrderManagement.Orders.Core/
│   │   └── OrderManagement.Orders.Infrastructure/
│   ├── OrderManagement.Inventory/
│   │   ├── OrderManagement.Inventory.API/
│   │   ├── OrderManagement.Inventory.Application/
│   │   ├── OrderManagement.Inventory.Core/
│   │   └── OrderManagement.Inventory.Infrastructure/
│   └── OrderManagement.Products/
│       └── OrderManagement.Products.API/
├── docker-compose.yml
└── README.md
```
