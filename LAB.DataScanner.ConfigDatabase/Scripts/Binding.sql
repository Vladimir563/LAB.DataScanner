CREATE TABLE [binding].[Binding]
(
	PublisherInstanceID INT,
	ConsumerInstanceID INT,
	CONSTRAINT FK_AppInstance_Bindings FOREIGN KEY ([PublisherInstanceID]) REFERENCES [component].[ApplicationInstance]([InstanceID]),
	CONSTRAINT FK_CustomerInstance_Bindings FOREIGN KEY ([ConsumerInstanceID]) REFERENCES [component].[ApplicationInstance]([InstanceID])
)
