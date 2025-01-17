begin tran
declare @error int=1
update [balance] set amount=amount-@amount,@error=0 where account_id=@account_from and amount>=@amount-maxcredit
if(@error=0)
begin 
  update [balance] set amount=amount+@amount,@error=0 where account_id=@account_to
  insert into [dbo].[transact](id,account_from,account_to,amount,extern_account,detail)
  values (@id,@account_from,@account_to, @amount, @extern_account, @detail)
end;
commit tran
select @error;