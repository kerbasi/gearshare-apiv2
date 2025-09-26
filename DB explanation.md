# **AutoPartsDB Schema Documentation**

## **1\. Overview**

**AutoPartsDB** is a relational database designed to manage an inventory of automotive parts for an e-commerce application. It tracks parts, their manufacturers, the vehicles they are compatible with, and manages stock levels and pricing.

The database is built on a normalized structure to ensure data integrity and minimize redundancy. All relationships are enforced using foreign key constraints.

## **2\. Table Definitions**

Below is a detailed description of each table in the database.

### **2.1 Manufacturers**

This table stores information about the companies that produce the auto parts.

| Column Name | Data Type | Constraints | Description |
| :---- | :---- | :---- | :---- |
| manufacturer\_id | INT | **Primary Key**, Identity | A unique identifier for each manufacturer. |
| name | NVARCHAR(100) | Not Null, Unique | The official name of the manufacturing company. |
| country | NVARCHAR(50) | Not Null | The country where the manufacturer is based. |

### **2.2 Cars**

This table contains a list of car models that parts can be compatible with. It defines a model's production range.

| Column Name | Data Type | Constraints | Description |
| :---- | :---- | :---- | :---- |
| car\_id | INT | **Primary Key**, Identity | A unique identifier for each car model entry. |
| make | NVARCHAR(50) | Not Null | The make of the car (e.g., 'Toyota', 'Ford'). |
| model | NVARCHAR(50) | Not Null | The model of the car (e.g., 'Camry', 'F-150'). |
| year\_start | INT | Not Null | The starting year of this model's production generation. |
| year\_end | INT | Not Null | The ending year of this model's production generation. |

### **2.3 Parts**

This is the central table for the inventory, storing details about each individual auto part.

| Column Name | Data Type | Constraints | Description |
| :---- | :---- | :---- | :---- |
| part\_id | INT | **Primary Key**, Identity | A unique identifier for each part. |
| sku | NVARCHAR(50) | Not Null, Unique | The Stock Keeping Unit (SKU) used for inventory tracking. |
| name | NVARCHAR(100) | Not Null | The common name of the part (e.g., 'Premium Oil Filter'). |
| description | NVARCHAR(255) | Not Null | A brief description of the part's features and specifications. |
| price | DECIMAL(10, 2\) | Not Null, CHECK (price \>= 0\) | The retail price of the part. |
| stock\_quantity | INT | Not Null, DEFAULT 0, CHECK (stock\_quantity \>= 0\) | The current number of units in stock. |
| manufacturer\_id | INT | **Foreign Key** (references Manufacturers) | Links the part to its manufacturer. |

### **2.4 PartCompatibility**

This is a junction (or link) table that creates a many-to-many relationship between Parts and Cars. It defines which parts fit which cars.

| Column Name | Data Type | Constraints | Description |
| :---- | :---- | :---- | :---- |
| part\_id | INT | **Composite Primary Key**, **Foreign Key** (references Parts) | The identifier of the part. |
| car\_id | INT | **Composite Primary Key**, **Foreign Key** (references Cars) | The identifier of the car. |

**Note:** The primary key for this table is a combination of part\_id and car\_id. This ensures that a part can only be linked to a specific car once, preventing duplicate compatibility entries.