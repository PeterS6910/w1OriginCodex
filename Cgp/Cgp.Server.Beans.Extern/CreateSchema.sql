create table Eventlog (
	Type nvarchar(255) not null,
	CGPSource nvarchar(255) not null,
	Description nvarchar(max) not null,
	EventlogDateTime datetime2(3) not null,
	IdEventLog bigint identity(1,1) not null primary key nonclustered);

create unique clustered index IndexEventLog_EventlogDateTime_IdEventLog on dbo.Eventlog
(
	EventlogDateTime asc,
	IdEventLog asc
);

create table EventSource (
	IdEventLog bigint not null,
	EventSourceObjectGuid uniqueidentifier not null,
	primary key clustered 
	(
		IdEventLog,
		EventSourceObjectGuid));

create nonclustered index IndexEventSource_IdEventLog on EventSource
(
	IdEventLog
);

alter table dbo.EventSource  with check add foreign key(IdEventLog)
references dbo.Eventlog (IdEventLog);

create table EventlogParameter (
	Type nvarchar(255) not null,
	TypeGuid uniqueidentifier null,
	TypeObjectType tinyint null,
	Value nvarchar(max) not null,
	IdEventLog bigint null,
	IdEventlogParameter bigint identity(1,1) not null primary key);

create nonclustered index IndexEventlogParameter_IdEventLog on dbo.EventlogParameter
(
	IdEventLog
);

alter table dbo.EventlogParameter  with check add foreign key(IdEventLog)
references dbo.Eventlog (IdEventLog);

create table SystemVersion (
	IdVersion uniqueidentifier primary key,
	DbsName nvarchar(255) not null,
	Version nvarchar(255) not null);
	
create table TimetecData (
	id int not null primary key, 
	LastEventId bigint);
	
create table TimetecErrorEvents (
	id int identity(1,1) not null primary key, 
	ErrorEventId bigint);

create table ConsecutiveEvents (
	id int not null IDENTITY   primary key, 
	LastEventlogId bigint null,
    SourceId uniqueidentifier not null,
    ReasonId uniqueidentifier not null ,
    LastEventDateTime datetime2(3) null);
