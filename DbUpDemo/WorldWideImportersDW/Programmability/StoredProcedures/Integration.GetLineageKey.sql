USE [WideWorldImportersDW]
GO
/****** Object:  StoredProcedure [Integration].[GetLineageKey]    Script Date: 3/21/2019 10:51:58 AM ******/
DROP PROCEDURE [Integration].[GetLineageKey]
GO
/****** Object:  StoredProcedure [Integration].[GetLineageKey]    Script Date: 3/21/2019 10:51:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Integration].[GetLineageKey]
@TableName sysname,
@NewCutoffTime datetime2(7)
WITH EXECUTE AS OWNER
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @DataLoadStartedWhen datetime2(7) = SYSDATETIME();

    INSERT Integration.Lineage
        ([Data Load Started], [Table Name], [Data Load Completed],
         [Was Successful], [Source System Cutoff Time])
    VALUES
        (@DataLoadStartedWhen, @TableName, NULL,
         0, @NewCutoffTime);

    SELECT TOP(1) [Lineage Key] AS LineageKey
    FROM Integration.Lineage
    WHERE [Table Name] = @TableName
    AND [Data Load Started] = @DataLoadStartedWhen
    ORDER BY LineageKey DESC;

    RETURN 0;
END;
GO
