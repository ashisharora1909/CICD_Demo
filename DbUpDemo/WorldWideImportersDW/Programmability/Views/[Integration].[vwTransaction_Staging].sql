use WideWorldImportersDW
GO

CREATE OR ALTER VIEW [Integration].[vwTransaction_Staging]
AS
SELECT * FROM [Integration].[Transaction_Staging]
GO
