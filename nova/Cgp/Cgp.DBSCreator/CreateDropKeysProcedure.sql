create procedure dbo.sp_dropkeys(@keyType nvarchar(max), @tableName nvarchar(max))
as
	declare @ConstraintName nvarchar(128);

	declare CurReferentialConstraints cursor local fast_forward for 
		select CONSTRAINT_NAME
		from INFORMATION_SCHEMA.TABLE_CONSTRAINTS
		where CONSTRAINT_TYPE = @keyType
		and TABLE_NAME = @tableName;

	open CurReferentialConstraints;

	fetch next from CurReferentialConstraints into @ConstraintName;

	declare @sqlCommand nvarchar(max);

	while @@FETCH_STATUS = 0
	begin
		set @sqlCommand = 'alter table ' + @tableName + ' drop ' + @ConstraintName;
		exec sp_executesql @sqlCommand;
		fetch next from CurReferentialConstraints into @ConstraintName;
	end

	close CurReferentialConstraints;
	deallocate CurReferentialConstraints;
