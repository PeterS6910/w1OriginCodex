delete from EventlogParameter where IdEventlog in (
	select IdEventlog from Eventlog where EventlogDateTime <= @DateTime);
	
delete from Eventsource where IdEventlog in (
	select IdEventlog from Eventlog where EventlogDateTime <= @DateTime);
	
delete from Eventlog where IdEventlog in (
	select IdEventlog from Eventlog where EventlogDateTime <= @DateTime);
