# UI Tesztek Selenium-mal

## Technológia
A felhasználói felületet Selenium könyvtárral Python 3.9-es környezetben készítettem. A tesztek célja, hogy vizuálisan szemléltesse a felhasználói felület használatát hasonlóan egy emberi felhasználó kattintásaihoz. Ennél többet is tudnak azonban a tesztek, mivel segédfüggvények és "assertion"-ök segítségével kódból tesztelhetőek az elvárt kimenetek, pl. szövegek, elvárt listaelemek stb. Ha ezek nem felelnek meg az elvárásnak, a script megszakítja a tesztet és logolja a felismert hibát, így a program későbbi módosításai esetén felismeri az UI-n történt nem várt változásokat.

## A program futtatása és szükséges kiegészítők
A programot a main.py elindításával lehet használni. A script elindítja a backend-et és a frontendet is, ezért ezeket külön nem kell indítanunk (nem is szabad, mert ekkor többen próbálnák ugyanazt a portot használni!).
Ekkor megnyílik egy Chrome böngésző, amelyen a script gombnyomásokat, szövegek beírását hajtja végig, így láthatjuk a tesztelt folyamatok vizuális lefutását, illetve a program a fontosabb vizsgálatoknál logolja az eredményeket.
A futás végén a konzolon feltett _Terminate batch job?_ kérdésre "y"-vel válaszoljunk, ez állítja le a frontendet.

_Szükséges függőségek_: 

A **dotnet ef**-nek megtalálhatónak kell lennie az alapértelmezett mappájában: _%USERPROFILE%\.dotnet\tools\dotnet-ef_. Ez a tesztelt alkalmazáshoz is kell elvileg, de ha mégsem itt található, ez a scriptben a jelzett helyen cserélendő a fájl elérési útjára.

Az **npm**-nek is hasonlóan megtalálhatónak kell lennie a _C:\Program Files\nodejs\npm.cmd_ alapértelmezett útvonalon. Amennyiben mégsem itt van, szintén cserélendő a scriptben a jelzett helyen.

## Tesztek
Az ellenőrzéshez 6 teszt készült

-Regisztráció (Belépés megpróbálása nem létező felhasználóval, a kapott hiba után regisztráció, majd újbóli belépési kísérlet a már létező felhasználóval)

-User Login

-Admin Login

-Termékek kosárba helyezése (a Termékek menüpont egyéb elemeinek tesztelésével, pl. csúszka állítása, látható elemek tesztelése)

-Megrendelés

-Megrendelés státusz (státusz megváltoztatása adminként, user-be visszalépve is megváltozik a státusz)

A tesztek úgy készültek, hogy külön fájlokból egyenként is futtathatóak legyenek, ehhez azonban manuálisan kell indítani a backendet és frontendet, illetve a teszt után drop-olni és update-elni az adatbázist (ezek a main-ben meg vannak valósítva, így csak futtatni kell)

A 6 teszt közül csak a regisztrációs és a megrendelés státusz teszt van külön meghívva a main-ben egymás után, mivel a megrendelés státusz futtatja a maradék 4 tesztet is, azokat felesleges lenne mégegyszer külön is futtatni. Amennyiben mégis szeretnénk valamelyik tesztet önállóan futtatni úgy, hogy ne kelljen manuálisan elindítanunk a tesztelt alkalmazást és visszaállítani az adatbázist, a main run_tests függvényének try blokkjában kommenteljük ki a többit, és csak a kívánt teszthívást és utána egy reset_database(driver) hívást hagyjunk benne.