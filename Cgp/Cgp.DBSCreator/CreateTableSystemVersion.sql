create table SystemVersion (
	IdVersion uniqueidentifier primary key,
	DbsName nvarchar(255) not null,
	Version nvarchar(255) not null);