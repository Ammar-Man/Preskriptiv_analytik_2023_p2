Projektbeskrivning:
Projektet involverar en bil som navigerar genom en 3D-miljö med målet att undvika kollision med vägen och nå slutet av den. Unity ML-agent implementeras för att lära bilen undvika kollisioner och belönas när den når målet eller träffar en cylinder. Målet är att träningen leder till att bilen navigerar säkert och effektivt.

Konfigurationsanalys:
Jämförelse mellan walker.yml och Basic.yml visar skillnader i parametrar som påverkar träningsresultaten. walker.yml presterar snabbare och stabilare med högre batch-storlek, mindre buffer, normalisering, fler dolda enheter, högre gamma, längre tidshorisont och längre träningssteg.

Kodanalys:
Unity ML-Agents Agent Script används för att styra bilens beteende. Initialisering och parametrar sätts upp, handlingar och observationer hanteras, och olika metoder styr rörelse och logik. Kodstrukturen möjliggör träning av en intelligent agent genom maskininlärning.
Sammanfattningsvis:
Projektet går ut på spelmekanik och maskininlärning för att skapa en autonom bil, och konfigurationsanalysen belyser hur olika inställningar påverkar träningsresultaten. Kodstrukturen ger en översikt över agentens beteende och styrning.Problem lost med två olika metoder:
