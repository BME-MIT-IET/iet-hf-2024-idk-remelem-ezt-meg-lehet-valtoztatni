# IET HF leírás

## Project scope

Mohácsi Ákos webshop applikációját vizsgáljuk. Az applikáció back-endje .NET alapú és REST API-t publikál, a front-endje React alapú. Az applikációnak már vannak alapvető unit és integrációs tesztjei, ezeket is tervezzük bővíteni. A deployment alapjai is adottak, de ezen is kell még dolgozni. 

## Workflow

- az első megbeszélésen megegyezünk az alkalmazáson amin dolgozunk, azt közösen tanulmányozzuk, megírjuk az IET-HF.md fájlt
- a megbeszélés után mindenki megvizsgálja az alkalmazást a saját idejében is
- az első megbeszélésen mindenki választ egyet a kiírt feladatok közül (közös megállapodás alapján)
	- ahhoz nyit egy issuet, abban dokumentálja mit is kell csinálnia pontosan
	- csinál egy branch-et, abban dolgozik - határidő május 16.
	- a doc folderben csinál egy új .md dokumentumot
	- mikor elkészült, nyit egy PR-t, ezt összekjöti az issue-val, majd a Teams chatben ír a csoport többi tagjának hogy review-t kér
	- a review alapján javításokat végez, ha azt jóváhagyta a reviewer merge-eli a main branchbe
- mindenkinek feladata legalább egy PR-t reviewelni
    - mikor elkezdi a review-t, reviewer-ként hozzáadja magát a PR-hez
    - a review során tanulmányozza a kódot, ellenőrzi hibákért
    - a review része ellenőrizni a doc-ban a dokumentációt is, annak létezését
    - a review része ellenőrizni az issue-t is, abban benne van -e minden
    - a reviewben commentként kell visszajelzést adni
    - ha javításra volt szükség, annak elkészültekor azt ellenőrizni
    - a branch merge már nem a reviewer feladata
- május 18. szombat előtt a munkamegosztás dokumentációhoz mindenki megírja a saját részét
- május 18. szombat összeülünk megbeszélni a bemutatást

## Részfeladatok szétosztása

- M Ákos: Unit teszt
- N Ábel: nem-funkcionális jellemzők
- P András: statikus analízis, manuális kód átvizsgálás
- Sz Ádám: API teszt
- V Ábel: UI teszt
