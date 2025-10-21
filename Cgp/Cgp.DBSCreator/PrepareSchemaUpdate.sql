-- temporary table EventLogTemp

create table EventLogTemp(
	IdEventLog uniqueidentifier primary key,
	Type nvarchar(255),
	CGPSource nvarchar(255),
	Description nvarchar(MAX),
	EventlogDateTime datetime2(3),
	IdEventLogInt bigint identity);

insert into EventLogTemp select IdEventlog, Type, CGPSource, Description, EventlogDateTime from EventLog order by EventlogDateTime;

-- drop keys & indices

exec dbo.sp_dropkeys @keyType = 'FOREIGN KEY', @tableName = 'EventSource';
exec dbo.sp_dropkeys @keyType = 'FOREIGN KEY', @tableName = 'EventLog';
exec dbo.sp_dropkeys @keyType = 'FOREIGN KEY', @tableName = 'EventlogParameter';

exec dbo.sp_dropkeys @keyType = 'PRIMARY KEY', @tableName = 'EventSource';
exec dbo.sp_dropkeys @keyType = 'PRIMARY KEY', @tableName = 'EventLog';
exec dbo.sp_dropkeys @keyType = 'PRIMARY KEY', @tableName = 'EventlogParameter';

exec dbo.sp_dropindices @tableName = 'EventSource';
exec dbo.sp_dropindices @tableName = 'EventLog';
exec dbo.sp_dropindices @tableName = 'EventlogParameter';

-- add new referencing columns (soon-to-be foreign keys) EventSource.IdEventLogInt & EventlogPatameter.IdEventLogInt

alter table EventSource add IdEventLogInt bigint;
alter table EventlogParameter add IdEventLogInt bigint;
