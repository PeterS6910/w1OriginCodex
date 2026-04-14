create procedure dbo.sp_dropindices(@tableName nvarchar(max))
as
	declare @indexName nvarchar(max);

	declare CurKeys cursor local fast_forward for
		select name
		from sys.indexes
		where object_id = object_id(@tableName)
		and type in (1, 2);

	open CurKeys;

	fetch next from CurKeys into @indexName;

	declare @sqlCommand nvarchar(max);

	while @@FETCH_STATUS = 0
	begin
		set @sqlCommand = 'drop index ' + @tableName + '.' + @indexName;
		exec sp_executesql @sqlCommand;

		fetch next from CurKeys into @indexName;
	end;

	close CurKeys;
	deallocate CurKeys;