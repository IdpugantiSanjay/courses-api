migrate migration-name migration-assembly dbcontext-name:
    dotnet ef migrations add {{migration-name}} -s API -p {{migration-assembly}} --context {{dbcontext-name}}

update-database migration-name migration-assembly dbcontext-name:
    dotnet ef database update {{migration-name}} -s API -p {{migration-assembly}} --context {{dbcontext-name}}
