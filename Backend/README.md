# Webshop Backend

.NET végpont alapú szerver oldali alkalmazás.

## Használat:
A használathoz rendelkezni kell **.NET 6.0 SDK**-val vagy **docker**-rel.

A solution fájl megnyitásával **Visual Studio**-ban könnyedén elindítható development módban, ilyenkor a **localhost:5100**-on érhetők el a végpontok.
Lokális futtatás elött a migrációkat le kell futtatni, amit a következő paranccsal lehet megtenni: dotnet ef database update --project WebShop.Api

A **docker compose** parancs kiadásával létrehoz egy docker konténert és production módban elindítja az alkalmazást, ilyekor a **localhost:80**-on érhetők el a végpontok.

A swagger végpont, csak development módban érhető el.
