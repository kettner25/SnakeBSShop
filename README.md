# Implementace hada v C Sharpu

Tento had vyžaduje pro svůj běh [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

Samotného hada lze spustit souborem `run.cmd` nebo z Visual Studia 2022.

Do konfiguračního soubor `Snake\appsettings.json` vyplňte údaje (např. pro hada týmu #8)
```json
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://127.0.0.1:6008"
      }
    }
  },
  "Snake": {
    "Name":  "Had týmu #8"
  }
```

Samotného hada najdete v souboru `Snake\Snake.cs`, kde změníte tělo funkce `Move`
