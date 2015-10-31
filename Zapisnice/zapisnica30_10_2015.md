# Zápisnica zo stretnutia TIS-BoardSmashers 30.10.2015
## Zoznam prítomných:
* Timotej Jurášek
* Martin Miklis
* Jakub Motýľ

## Zhodnotenie úloh z minulého obdobia:
* Zhotoviť analýzu technológií:  
Výber daných technológii bol úspešne analyzovaný.
* Pripraviť triedny diagram dátového modelu:  
Diagram je hotový.
* Presunúť Ganttov diagram s časovým plánom do novej aplikácie:  
Momentálne sa hľadá vhodná náhrada za SmartSheet aplikáciu.
* Preskúmať súborovú štruktúru **omap** vstupov:  
Po dlhom čítaní sme samostatne na nič použiteľné ohľadom **omap** súborov neprišli, vyriešime tento problém spoločne.
* Prediskutovať grafické rozhranie so zadávateľom:  
Po doplnení pár drobností je grafické rozhranie hotové.

## Body stretnutia:
* Nájsť podstatné informácie v **omap**:  
Podarilo sa nám spoločne zistiť, akým spôsobom sa vrstevnice v tomto formáte mapujú, ako sú označené keď sú cyklické, označenie 2D splinov a nájsť objekty s trasami.
* Rozhodnúť, či v cyklických objektoch ponecháme totožné začiatočné prvky s koncovými, alebo budeme reprezentovať cykly dodatočným atribútom:  
Rozhodli sme sa pre dodatočný atribút.
* Rozdeliť triedy aplikácie objemovo rovnomerne pre budúcu prácu na špecifikácii a implementácii:  
Jeden člen týmu spracuje triedy Parser a Track, druhý ViewController, Camera a Terrain, a tretí triedu Builder.

## Budúce úlohy:
* Presunúť Ganttov diagram s časovým plánom do novej aplikácie - Martin Miklis
* Podrobne špecifikovať komponenty (triedy) v programe.
* Do analýzy technológií doplniť rozbor relevantných objektov, symbolov a značiek (tagov).