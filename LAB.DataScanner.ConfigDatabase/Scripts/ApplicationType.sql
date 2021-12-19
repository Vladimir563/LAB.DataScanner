CREATE TABLE [meta].[ApplicationType]
(
	TypeID INT NOT NULL IDENTITY PRIMARY KEY CLUSTERED (TypeID),
	TypeName NVARCHAR(50),
	TypeVersion NVARCHAR(12),
	ConfigTemplateJson NVARCHAR(MAX)
)
