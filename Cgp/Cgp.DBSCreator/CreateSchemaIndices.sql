-- primary keys

alter table EventSource add primary key clustered (IdEventLog, EventSourceObjectGuid);
alter table EventLog add primary key nonclustered (IdEventLog);
alter table EventlogParameter add primary key clustered (IdEventlogParameter);

-- foreign keys

alter table EventSource add foreign key (IdEventLog) references EventLog(IdEventLog);
alter table EventlogParameter add foreign key (IdEventLog) references EventLog(IdEventLog);

-- add non-primary key indices

create unique clustered index IndexEventLog_EventlogDateTime_IdEventLog on EventLog(EventlogDateTime, IdEventLog);
create index IndexEventSource_IdEventLog on EventSource(IdEventLog);
create index IndexEventlogParameter_IdEventLog on EventlogParameter(IdEventLog);
