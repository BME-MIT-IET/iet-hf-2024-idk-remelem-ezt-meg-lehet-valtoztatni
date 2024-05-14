# UI Tesztek Selenium-mal

## Technológia
A felhasználói felületet Selenium könyvtárral Python 3.9-es környezetben készítettem. A tesztek célja, hogy vizuálisan szemléltesse a felhasználói felület használatát hasonlóan egy emberi felhasználó kattintásaihoz. Ennél többet is tudnak azonban a tesztek, mivel segédfüggvények és "assertion"-ök segítségével kódból tesztelhetőek az elvárt kimenetek, pl. szövegek, elvárt listaelemek stb. Ha ezek nem felelnek meg az elvárásnak, a script megszakítja a tesztet és logolja a felismert hibát, így a program későbbi módosításai esetén felismeri az UI-n történt nem várt változásokat.

## A program futtatása
A programot a requirements.txt-ben olvasható függőségek telepítése után a main.py elindításával lehet használni egy **argumentum megadásával**, mely egy **1**-től **7**-ig terjedő egész szám. A számok a futtatandó tesztet azonosítják, **1**-től **6**-ig a számok a _Tesztek_ részben lévő sorrendben lévő teszteknek felelnek meg, a **7**-es pedig az összes tesztet futtatja egymás után. A script elindítja a backend-et és a frontendet is, ezért ezeket külön nem kell indítanunk (nem is szabad, mert ekkor többen próbálnák ugyanazt a portot használni!).
Ekkor megnyílik egy Chrome böngésző, amelyen a script gombnyomásokat, szövegek beírását hajtja végig, így láthatjuk a tesztelt folyamatok vizuális lefutását, illetve a program a fontosabb vizsgálatoknál logolja az eredményeket.
A futás végén a konzolon feltett _Terminate batch job?_ kérdésre "y"-vel válaszoljunk, ez állítja le a frontendet. Ezen kívül a **4.** és **6.** tesztben a PDF letöltéséhez szükséges a letöltést elfogadni.

## Tesztek
Az ellenőrzéshez 6 teszt készült

1. Regisztráció (Belépés megpróbálása nem létező felhasználóval, a kapott hiba után regisztráció, majd újbóli belépési kísérlet a már létező felhasználóval)

2. User Login

3. Termékek kosárba helyezése (a Termékek menüpont egyéb elemeinek tesztelésével, pl. csúszka állítása, látható elemek tesztelése)

4. Megrendelés

5. Admin Login

6. Megrendelés státusz (státusz megváltoztatása adminként, user-be visszalépve is megváltozik a státusz)

A tesztek úgy készültek, hogy külön fájlokból egyenként is futtathatóak legyenek, ehhez azonban manuálisan kell indítani a backendet és frontendet, illetve a teszt után drop-olni és update-elni az adatbázist (ezek a main-ben meg vannak valósítva, így csak futtatni kell)

A tesztek gyakran felhasználnak másik teszteket azon helyeken, ahol ugyanannak a folyamatsorozatnak futtatása szükséges. 