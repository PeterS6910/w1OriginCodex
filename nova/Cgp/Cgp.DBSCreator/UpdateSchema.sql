update EventSource set EventSource.IdEventLogInt = (
	select IdEventLogInt 
	from EventLogTemp 
	where [EventLogTemp].IdEventLog = EventSource.IdEventlog);

update EventlogParameter set EventlogParameter.IdEventLogInt = (
	select IdEventLogInt 
	from EventLogTemp 
	where [EventLogTemp].IdEventLog = EventlogParameter.IdEventlog);

-- truncate EventLog

truncate table EventLog;

-- populate EventLog

alter table EventLog drop column IdEventLog;
alter table EventLog add IdEventLog bigint identity not null;

insert into EventLog (Type, CGPSource, Description, EventlogDateTime) 
	select Type, CGPSource, Description, EventlogDateTime 
	from EventLogTemp 
	order by IdEventLogInt;

drop table EventLogTemp;

-- prepare for primary keys EventLog

delete from EventLog where EventlogDateTime is null;
alter table EventLog alter column EventlogDateTime datetime2(3) not null;

-- EventSource & EventlogParameter

alter table EventlogParameter drop column IdEventlogParameter;
alter table EventlogParameter drop column IdEventLog;

alter table EventlogParameter add IdEventlogParameter bigint identity not null;

exec sp_rename 'EventlogParameter.IdEventLogInt', 'IdEventLog', 'COLUMN';

create table EventSourceTmp (
	IdEventLog bigint not null,
	EventSourceObjectGuid uniqueidentifier not null);

insert into EventSourceTmp select distinct IdEventLogInt, EventSourceObjectGuid from EventSource;

drop table EventSource;

exec sp_rename 'EventSourceTmp', 'EventSource';

-- shrink database

dbcc shrinkdatabase (0);