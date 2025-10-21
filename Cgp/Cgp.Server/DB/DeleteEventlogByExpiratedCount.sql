declare @toDelete as integer;

set @toDelete = (select count(*) from eventlog as count);
set @toDelete = @toDelete - @maxRecords;

if (@toDelete > 0)
begin
	delete from EventlogParameter where IdEventlog in (
		select top (@toDelete)  IdEventlog from Eventlog order by EventlogDateTime);
		
	delete from Eventsource where IdEventlog in (
		select top (@toDelete)  IdEventlog from Eventlog order by EventlogDateTime);
		
	delete from Eventlog where IdEventlog in (
		select top (@toDelete)  IdEventlog from Eventlog order by EventlogDateTime);
end