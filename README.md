The `SuggestedFileName` will be `"download"` instead of `"testäöü.txt"` when umlaute are specified in the `filename*=UTF-8''test%EF%BF%BD%EF%BF%BD%EF%BF%BD.txt` of the `Content-Disposition` header even though they are UTF-8 escaped.
On Windows the same code works.

Reproduce the issue by running these PowerShell commands:

```ps
docker run -it --rm -w /app --mount "type=bind,src=$(pwd),target=/app" mcr.microsoft.com/playwright/dotnet:v1.52.0
```

```ps
dotnet run --project WebApplication5.csproj
```
