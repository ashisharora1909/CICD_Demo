USE [WideWorldImportersDW]
GO
/****** Object:  StoredProcedure [Sequences].[ReseedAllSequences]    Script Date: 3/21/2019 10:51:58 AM ******/
DROP PROCEDURE [Sequences].[ReseedAllSequences]
GO
/****** Object:  StoredProcedure [Sequences].[ReseedAllSequences]    Script Date: 3/21/2019 10:51:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [Sequences].[ReseedAllSequences]
AS BEGIN
    -- Ensures that the next sequence values are above the maximum value of the related table columns
    SET NOCOUNT ON;
 
    EXEC Sequences.ReseedSequenceBeyondTableValues @SequenceName = 'CityKey', @SchemaName = 'Dimension', @TableName = 'City', @ColumnName = 'City Key';
    EXEC Sequences.ReseedSequenceBeyondTableValues @SequenceName = 'CustomerKey', @SchemaName = 'Dimension', @TableName = 'Customer', @ColumnName = 'Customer Key';
    EXEC Sequences.ReseedSequenceBeyondTableValues @SequenceName = 'EmployeeKey', @SchemaName = 'Dimension', @TableName = 'Employee', @ColumnName = 'Employee Key';
    EXEC Sequences.ReseedSequenceBeyondTableValues @SequenceName = 'LineageKey', @SchemaName = 'Integration', @TableName = 'Lineage', @ColumnName = 'Lineage Key';
    EXEC Sequences.ReseedSequenceBeyondTableValues @SequenceName = 'PaymentMethodKey', @SchemaName = 'Dimension', @TableName = 'Payment Method', @ColumnName = 'Payment Method Key';
    EXEC Sequences.ReseedSequenceBeyondTableValues @SequenceName = 'StockItemKey', @SchemaName = 'Dimension', @TableName = 'Stock Item', @ColumnName = 'Stock Item Key';
    EXEC Sequences.ReseedSequenceBeyondTableValues @SequenceName = 'SupplierKey', @SchemaName = 'Dimension', @TableName = 'Supplier', @ColumnName = 'Supplier Key';
    EXEC Sequences.ReseedSequenceBeyondTableValues @SequenceName = 'TransactionTypeKey', @SchemaName = 'Dimension', @TableName = 'Transaction Type', @ColumnName = 'Transaction Type Key';
END;
GO
