USE [data_scanner_db]
GO

SET NOCOUNT ON

SET IDENTITY_INSERT [meta].[ApplicationType] ON

DECLARE @mergeOutput TABLE ( [DMLAction] VARCHAR(6) );
MERGE INTO [meta].[ApplicationType] AS [Target]
USING (VALUES
(1, 'UrlsGenerator', '1.0', ''),
(2, 'WebPageDownloader', '1.0', ''),
(3, 'HtmlToJsonConverter ', '1.0', ''),
(4, 'SimpleTableDBPersister ', '1.0', '')) 
AS [Source] ([TypeID],[TypeName],[TypeVersion],[ConfigTemplateJson])
ON ([Target].[TypeID] = [Source].[TypeID])
WHEN MATCHED AND (
	NULLIF([Source].[TypeName], [Target].[TypeName]) IS NOT NULL OR NULLIF([Target].[TypeName], [Source].[TypeName]) IS NOT NULL OR 
	NULLIF([Source].[TypeVersion], [Target].[TypeVersion]) IS NOT NULL OR NULLIF([Target].[TypeVersion], [Source].[TypeVersion]) IS NOT NULL OR 
	NULLIF([Source].[ConfigTemplateJson], [Target].[ConfigTemplateJson]) IS NOT NULL OR NULLIF([Target].[ConfigTemplateJson], [Source].[ConfigTemplateJson]) IS NOT NULL) THEN
 UPDATE SET
  [Target].[TypeName] = [Source].[TypeName], 
  [Target].[TypeVersion] = [Source].[TypeVersion], 
  [Target].[ConfigTemplateJson] = [Source].[ConfigTemplateJson]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([TypeID],[TypeName],[TypeVersion],[ConfigTemplateJson])
 VALUES([Source].[TypeID],[Source].[TypeName],[Source].[TypeVersion],[Source].[ConfigTemplateJson])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE
OUTPUT $action INTO @mergeOutput;

DECLARE @mergeError int
 , @mergeCount int, @mergeCountIns int, @mergeCountUpd int, @mergeCountDel int
SELECT @mergeError = @@ERROR
SELECT @mergeCount = COUNT(1), @mergeCountIns = SUM(IIF([DMLAction] = 'INSERT', 1, 0)), @mergeCountUpd = SUM(IIF([DMLAction] = 'UPDATE', 1, 0)), @mergeCountDel = SUM (IIF([DMLAction] = 'DELETE', 1, 0)) FROM @mergeOutput
IF @mergeError != 0
 BEGIN
 PRINT 'ERROR OCCURRED IN MERGE FOR [meta].[ApplicationType]. Rows affected: ' + CAST(@mergeCount AS VARCHAR(100)); -- SQL should always return zero rows affected
 END
ELSE
 BEGIN
 PRINT '[meta].[ApplicationType] rows affected by MERGE: ' + CAST(COALESCE(@mergeCount,0) AS VARCHAR(100)) + ' (Inserted: ' + CAST(COALESCE(@mergeCountIns,0) AS VARCHAR(100)) + '; Updated: ' + CAST(COALESCE(@mergeCountUpd,0) AS VARCHAR(100)) + '; Deleted: ' + CAST(COALESCE(@mergeCountDel,0) AS VARCHAR(100)) + ')' ;
 END
GO


SET IDENTITY_INSERT [meta].[ApplicationType] OFF
SET NOCOUNT OFF
GO