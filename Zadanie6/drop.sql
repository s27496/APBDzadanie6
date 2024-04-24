IF (OBJECT_ID('dbo.Product_Warehouse_Order', 'F') IS NOT NULL)
ALTER TABLE Product_Warehouse DROP CONSTRAINT Product_Warehouse_Order;

IF (OBJECT_ID('dbo.Receipt_Product', 'F') IS NOT NULL)
ALTER TABLE "Order" DROP CONSTRAINT Receipt_Product;

IF (OBJECT_ID('dbo._Product', 'F') IS NOT NULL)
ALTER TABLE Product_Warehouse DROP CONSTRAINT _Product;

IF (OBJECT_ID('dbo._Warehouse', 'F') IS NOT NULL)
ALTER TABLE Product_Warehouse DROP CONSTRAINT _Warehouse;


DROP TABLE IF EXISTS dbo."Order";
DROP TABLE IF EXISTS dbo.Product;
DROP TABLE IF EXISTS dbo.Warehouse;
DROP TABLE IF EXISTS dbo.Product_Warehouse;
