.nuget\NuGet Update -self
.nuget\NuGet pack -sym FluentProjections\FluentProjections.csproj -Prop Configuration=Release
.nuget\NuGet pack -sym FluentProjections.Dapper\FluentProjections.Dapper.csproj -Prop Configuration=Release
.nuget\NuGet pack -sym FluentProjections.AutoMapper\FluentProjections.AutoMapper.csproj -Prop Configuration=Release
.nuget\NuGet pack -sym FluentProjections.ValueInjecter\FluentProjections.ValueInjecter.csproj -Prop Configuration=Release
