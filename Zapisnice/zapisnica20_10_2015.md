# Zápisnica zo stretnutia TIS-BoardSmashers 20.10.2015
## Zoznam prítomných:
* Martin Miklis
* Jakub Motýľ

## Zhodnotenie úloh z minulého obdobia:
* Zhotoviť konceptuálnu analýzu:  
Jednotlivé časti konceptuálnej analýzy boli podľa plánu dokončené a budú vložené a spojené do výsledného dokumentu.
* Prediskutovať grafické rozhranie so zadávateľom:  
Čakáme na schválenie, prípadný feedback zadávateľa.
* Doladiť detaily katalógu požiadaviek so zadávateľom:  
Zadávateľ vyjadril svoje postrehy a katalóg požiadaviek bol adekvátne upravený a označený ako schválený, viac info v zložke s komunikáciou vo verejnom repozitári s projektom.
* Potvrdiť so zadávateľom katalóg požiadaviek a funkcie základnej a rozšírenej verzie podľa katalógu požiadaviek:  
Zadávateľ súhlasil s našim rozdelením funkcionality. Viac info v zložke s komunikáciou vo verejnom repozitári s projektom.

## Body stretnutia:
* Vymyslieť intuitívne ovládanie s použitím myši:  
Pravým tlačidlom sa rotuje kamera okolo relatívnych x a y osí zobrazenej oblasti okolo bodu, kam užívateľ klikol. Kolieskom sa približuje a odďaľuje kamera s centrovaním do stredu zobrazenej oblasti. Ľavým tlačidlom sa posúva kamera po relatívnych x a y osiach zobrazenej oblasti. Hodnoty rotácie a posunutia sú relatívne (podľa miery priblíženia), a hodnoty priblíženia sú konštantné.
* Vybrať technológie, ktoré použijeme:  
Zhodli sme sa, že využijeme Unity3D z dôvodov bližšie vysvetlených v čoskoro dostupnej technologickej analýze. Z jazykov dostupných v prostredí Unity sme si vybrali C# a z tohoto dôvodu pre parsovanie dát z **omap** súborov využijeme vlastný parser pracujúci so štandardnou triedou jazyka C# zvanou XMLReader.

## Budúce úlohy:
* Zhotoviť analýzu technológií - Timotej Jurášek
* Pripraviť triedny diagram dátového modelu - Jakub Motýľ
* Presunúť Ganttov diagram s časovým plánom do novej aplikácie - Martin Miklis
* Preskúmať súborovú štruktúru **omap** vstupov - všetci