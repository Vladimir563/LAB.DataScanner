CREATE TABLE [component].[ApplicationInstance]
(
	InstanceID INT NOT NULL IDENTITY PRIMARY KEY CLUSTERED(InstanceID),
	TypeID INT,
	CONSTRAINT FK_AppType_AppInstance FOREIGN KEY ([TypeID]) REFERENCES [meta].[ApplicationType]([TypeID]),
	InstanceName NVARCHAR(50),
	ConfigJson NVARCHAR(MAX)
)
