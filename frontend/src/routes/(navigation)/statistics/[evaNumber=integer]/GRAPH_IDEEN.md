# Stations Statistik Dashboard - Graph Ideen

## Leitfrage und Zielbild

**Leitfrage:** Wie verlaesslich und brauchbar puenktlich funktioniert der Verkehr an einer konkret ausgewaehlten Station - insgesamt, im Vergleich zum Netzwerk, nach Ankunft/Abfahrt, Linie, Richtung, Verkehrstyp und Tageszeit?

**Zielbild:** Die erste Sicht der Stationsseite soll fuer eine Station eine klare Aussage liefern:

> "Von allen geplanten Stopps an dieser Station findet welcher Anteil wirklich statt, welcher Anteil ist aus Kundensicht brauchbar puenktlich, und welche Linien, Richtungen oder Tageszeiten erklaeren die groessten Probleme?"

Die Seite soll eine Station nicht wie ein isoliertes Datenblatt zeigen, sondern als nutzbaren Diagnosepunkt:

- Kundenperspektive zuerst: Ein geplanter Stop ist nur gut, wenn er stattfindet und innerhalb der vereinbarten Schwelle liegt.
- Operative Perspektive ergaenzend: Wie puenktlich sind die Stopps, die tatsaechlich gefahren sind?
- Ursachennahe Perspektiven danach: Linie, Richtung, Tageszeit, Ankunft/Abfahrt, Verkehrstyp und Einzelevents.
- Jede Sektion soll eine eigene Aussage transportieren und Definitionen nur dort wiederholen, wo sie fuer die Interpretation notwendig sind.

## Regeln und Begriffe

### Schwellen

Aktueller API-Stand:

- `customerReliability5Rate` und `operativePunctuality5Rate` meinen laut API-Beschreibung "less than six minutes delay", also technisch `delay < 6:00`, in der UI am klarsten als `<= 5:59`.
- `customerReliability15Rate` und `operativePunctuality15Rate` meinen laut aktueller Station-UI `<= 14:59` bzw. laut API "less than fifteen minutes delay".

Fachliche Klaerung fuer die Ziel-UI:

- Wenn die Produktregel wirklich `< 5 min` meint, braucht die API eine neue 5:00-Schwelle. Die vorhandene "5"-Metrik darf dann nicht als `< 5 min` beschriftet werden.
- Wenn die Produktregel wirklich `< 16 min` meint, braucht die API eine neue 16:00-Schwelle. Die vorhandene "15"-Metrik darf dann nicht als `< 16 min` beschriftet werden.
- Empfohlene kurzfristige Beschriftung ohne API-Aenderung: `<= 5:59` und `<= 14:59`.
- Empfohlene Produktentscheidung: Station und Netzwerk sollten dieselben Schwellenlabels verwenden, damit Nutzer nicht zwischen "5", "6", "15" und "16" interpretieren muessen.

### Perspektive

- **Customer:** Nenner sind alle geplanten Stop-Events. Ausfaelle senken die Kennzahl. Das ist die Standardperspektive fuer die Leitfrage.
- **Operativ:** Nenner sind nur bediente Stop-Events. Ausfaelle werden nicht in der Puenktlichkeitsquote mitgezaehlt und muessen separat sichtbar bleiben.
- Panels duerfen beide Perspektiven gemeinsam zeigen, wenn der Unterschied die Aussage ist. Andernfalls sollte pro Panel eine sichtbare Perspektivenwahl oder eine eindeutige Beschreibung stehen.

### Weitere Regeln

- **Stop-Event:** Ein geplanter Ankunfts- oder Abfahrtszeitpunkt an der Station.
- **Arrival/Departure:** Ankunft und Abfahrt duerfen nicht ungekennzeichnet gemischt werden, weil sie unterschiedliche Fragen beantworten.
- **Local time:** Tageszeit- und Wochenprofile beziehen sich auf lokale Zeit im Request-Zeitraum.
- **Volume first:** Prozentwerte muessen zusammen mit `plannedEvents` oder einer Volumeninformation lesbar sein.
- **Replacement services:** Ersatzverkehre sind standardmaessig enthalten, koennen aber ueber Filter ausgeschlossen werden.
- **Coverage:** Die Station zeigt Navigator-Messdaten fuer die ausgewaehlte Station und den Zeitraum, keine amtliche Vollerhebung.

## Navigator.Api Check

Die Stationsseite nutzt `POST /api/v1/statistics/stations` mit `StationStatisticsMetricRequest`.

Bereits verfuegbare Station-Metriken:

- `EVENT_SUMMARY`: Event-KPIs fuer die Station.
- `BENCHMARK`: Station gegen gefiltertes Netzwerk; `similarStations` ist im Backend-Modell vorhanden, wird aktuell aber mit `null` geliefert.
- `TIME_SERIES`: Event-Zeitreihe nach Bucket.
- `ARRIVAL_DEPARTURE_COMPARISON`: Event-Metriken getrennt nach Ankunft und Abfahrt.
- `WEEKDAY_HOUR_HEATMAP`: Event-Metriken nach Wochentag und Stunde.
- `LINE_RANKING`: Linien an der Station nach Event-Metriken, mit Paging und optionalen Richtungsfiltern.
- `DIRECTIONS`: verbundene Ursprungs-/Zielstationen nach Event-Metriken.
- `TRANSPORT_TYPE_MIX`: Verkehrstypen mit Anteil und Event-Metriken.
- `LINE_HOUR_MATRIX`: Linie x Stunde mit Event-Metriken.
- `EVENT_DETAILS`: einzelne Stop-Events mit Linie, Journey, Start/Ziel, Delay, Cancellation und Ersatzverkehr.

Wichtige Luecken fuer staerkere Visualisierungen:

- Keine eigene Station-Delay-Distribution/Quantile als API-Metrik.
- Keine Journey-outcome-Perspektive fuer Stationen, nur Stop-Event-Perspektive.
- Keine echten Schwellen fuer `< 5:00` oder `< 16:00`, falls diese fachlich gefordert sind.
- Kein befuelltes `similarStations`-Benchmark.

## Ideen

### 1. Station Outcome Summary

**Ideenspezifische Leitfrage:** Was passiert mit 100 geplanten Stopps an dieser Station?

**Aussage:** Dieses Panel soll die zentrale Customer-Botschaft direkt am Anfang liefern: Anteil der geplanten Stopps, die `<= 5:59` puenktlich sind, spaeter sind oder ausfallen.

**Darstellung / Technische Zuordnung:** Horizontaler gestapelter `BarChart` mit genau einer Zeile, analog zur Netzwerk-Balance. Segmente: `served <= 5:59`, `served <= 14:59`, `late >= 15:00`, `cancelled`. Wenn die Zielschwellen auf `< 5:00` und `< 16:00` geaendert werden, muss die Segmentlogik angepasst werden.

**Datenaufwand:** Niedrig mit vorhandenen `EVENT_SUMMARY`-Metriken fuer aktuelle Schwellen; mittel bis hoch bei neuen Schwellen.

**Hinweise:** Das Panel sollte nicht alle KPI-Karten ersetzen, sondern die zentrale Aussage buendeln. Nenner ist immer `plannedEvents`. Ausfaelle muessen sichtbar bleiben.

### 2. KPI Grid mit klarer Customer/Operative Trennung

**Ideenspezifische Leitfrage:** Wie gut ist die Station auf einen Blick?

**Aussage:** Die wichtigsten Kennzahlen der Station: geplante Stopps, Customer-Zuverlaessigkeit, operative Puenktlichkeit, Ausfallrate, mittlere Verspaetung und schwere Verspaetung.

**Darstellung / Technische Zuordnung:** KPI-Karten, kein LayerChart erforderlich. Optional kleine Inline-Trends nur wenn spaeter Vorperiodenwerte verfuegbar werden.

**Datenaufwand:** Niedrig. `EVENT_SUMMARY` und `BENCHMARK` sind vorhanden.

**Hinweise:** Bestehendes `StationKpiGrid` ist teilweise umgesetzt. Es zeigt Customer und operative Werte, aber die Perspektivenlogik ist noch nicht als Regel erklaert. Die 15-Minuten-Schwelle fehlt in den KPIs.

### 3. Station vs Netzwerk Benchmark

**Ideenspezifische Leitfrage:** Ist diese Station besser oder schlechter als das gefilterte Netzwerk?

**Aussage:** Die Station soll nicht nur absolute Werte zeigen, sondern relativ eingeordnet werden.

**Darstellung / Technische Zuordnung:** Dumbbell-/Bullet-Vergleich mit `<Chart>` plus `Rule`/`Points` oder gruppierter horizontaler `BarChart`. Reihen: Customer `<= 5:59`, Customer `<= 14:59`, Cancellation, operative `<= 5:59`, operative `<= 14:59`.

**Datenaufwand:** Niedrig fuer Station vs Netzwerk, da `BENCHMARK` bereits Station und Network liefert. Mittel fuer "similar stations", da `similarStations` aktuell nicht befuellt wird.

**Hinweise:** Bestehender KPI-Trend zeigt nur einen Delta fuer `customerReliability5Rate`. Ein eigenes Benchmark-Panel wuerde die Einordnung deutlich staerker machen.

### 4. Stop Reliability Timeline

**Ideenspezifische Leitfrage:** Wird die Station im Zeitverlauf besser oder schlechter?

**Aussage:** Die Station-Qualitaet ueber Zeit, inklusive Customer- und Operativ-Perspektive.

**Darstellung / Technische Zuordnung:** `LineChart` mit `series`. Perspektivenwahl: Customer oder Operativ. Schwellenwahl: `<= 5:59` oder `<= 14:59`. Cancellation bleibt als separate Linie oder separater Unterbereich sichtbar.

**Datenaufwand:** Niedrig mit `TIME_SERIES`.

**Hinweise:** Aktuell voll/teilweise umgesetzt als `StationTrendChart`, aber Customer-only und ohne Perspektiven-/Schwellenkontrolle. Eine Beschreibung sollte nicht redundant alle Definitionen wiederholen, sondern nur die aktive Perspektive nennen.

### 5. Delay Severity Timeline

**Ideenspezifische Leitfrage:** Sind Probleme eher leichte Verspaetungen, schwere Verspaetungen oder Ausfaelle?

**Aussage:** Nicht nur "puenktlich oder nicht", sondern Schweregrad ueber Zeit.

**Darstellung / Technische Zuordnung:** Gestapelter `AreaChart` mit geplanten Stop-Event-Anteilen: `<= 5:59`, `6:00-14:59`, `>= 15:00`, `>= 30:00`, `>= 60:00`, `cancelled`. Alternativ zwei Modi: "delayed only" und "all planned stops".

**Datenaufwand:** Niedrig bis mittel. `late30Rate`, `late60Rate`, Customer/Operative-Schwellen und Cancellation sind in `EventMetrics` vorhanden; Zwischenklassen muessen wie im Netzwerk-Dashboard aus Aggregaten abgeschaetzt werden.

**Hinweise:** Fuer exakte Klassen waeren zusaetzliche Backend-Buckets sauberer. Wenn nur abgeleitet wird, muss die Ableitung dokumentiert werden.

### 6. Arrival vs Departure Quality

**Ideenspezifische Leitfrage:** Ist die Station als Ankunftspunkt oder als Abfahrtspunkt problematischer?

**Aussage:** Ankunfts- und Abfahrtsqualitaet getrennt, weil Fahrgastfragen verschieden sind.

**Darstellung / Technische Zuordnung:** Gruppierter horizontaler `BarChart` oder kleine Dumbbell-Ansicht. Werte: Customer `<= 5:59`, operative `<= 5:59`, Cancellation, geplante Stopps.

**Datenaufwand:** Niedrig mit `ARRIVAL_DEPARTURE_COMPARISON`.

**Hinweise:** Aktuell teilweise umgesetzt als `StationComparisonChart`, aber nur Customer `5` als Balken plus Nebeninfo. Fuer die Zielaussage sollten Customer/Operativ und Ausfallrate klarer nebeneinander stehen.

### 7. Weekday and Hour Heatmap

**Ideenspezifische Leitfrage:** Zu welchen Tageszeiten und Wochentagen ist diese Station besonders zuverlaessig oder riskant?

**Aussage:** Hotspots nach lokaler Zeit erkennen.

**Darstellung / Technische Zuordnung:** Besser als echtes Heatmap-Raster mit `<Chart>`, `Axis` und `Cell`, nicht als Scatter-only. Metric-Control fuer Customer `<= 5:59`, Customer `<= 14:59`, operative `<= 5:59`, operative `<= 14:59`, Cancellation, planned events.

**Datenaufwand:** Niedrig mit `WEEKDAY_HOUR_HEATMAP`.

**Hinweise:** Aktuell teilweise umgesetzt als `ScatterChart` mit Radius fuer Volumen. Das ist informativ, aber auf Mobile wegen Mindestbreite schwerer. Ein Raster wie im Netzwerk-Dashboard waere konsistenter.

### 8. Hourly Profile

**Ideenspezifische Leitfrage:** Welche Stunden im Tagesverlauf treiben die Stationsqualitaet?

**Aussage:** Aggregiertes Stundenprofil ueber alle Wochentage oder getrennt nach Werktag/Wochenende.

**Darstellung / Technische Zuordnung:** `Chart` mit `Bars` fuer Volumen und `Spline` fuer Customer/Operative/Cancellation. Controls: Day group, Perspective, Threshold.

**Datenaufwand:** Niedrig bis mittel. Kann aus `WEEKDAY_HOUR_HEATMAP` aggregiert werden.

**Hinweise:** Im Netzwerk-Dashboard existiert dieses Muster bereits. Fuer Stationen waere es weniger detailreich als die Heatmap und sollte daher als zusammenfassende Tagesganglinie verstanden werden.

### 9. Transport Type Mix and Quality

**Ideenspezifische Leitfrage:** Welche Verkehrstypen praegen die Station, und wie gut funktionieren sie?

**Aussage:** Zeigt, ob die Stationsqualitaet von einzelnen Verkehrstypen dominiert oder verzerrt wird.

**Darstellung / Technische Zuordnung:** Horizontaler `BarChart` oder gestapelter Balken: Anteil am Volumen plus Qualitaetsmarker. Optional zwei Spalten: Anteil und Customer-Zuverlaessigkeit.

**Datenaufwand:** Niedrig mit `TRANSPORT_TYPE_MIX`.

**Hinweise:** Aktuell teilweise umgesetzt. Die vorhandene Darstellung nutzt den Anteil als Hauptbalken und schreibt Reliability in den Nebentext. Eine Marker- oder Doppelachsenloesung waere lesbarer, wenn sie nicht ueberfrachtet wird.

### 10. Line Delay Pressure Ranking

**Ideenspezifische Leitfrage:** Welche Linien verursachen an dieser Station den groessten Druck?

**Aussage:** Zeigt die Linien, bei denen sich Verspaetungsminuten, Ausfaelle oder Unzuverlaessigkeit konzentrieren.

**Darstellung / Technische Zuordnung:** Horizontaler `BarChart` plus Rankingliste. Umschaltbare Ranking-Metrik: Delay debt, Customer `<= 5:59`, Cancellation, planned events.

**Datenaufwand:** Niedrig fuer Delay debt mit `LINE_RANKING`; mittel fuer flexible Sortierung, weil Backend aktuell nach positiver Delay-Summe sortiert.

**Hinweise:** Aktuell teilweise umgesetzt. Es ist wertvoll, aber noch stark auf Delay debt festgelegt. Fuer Kundenfragen sollte Customer Reliability mindestens als Option sichtbar sein.

### 11. Directions / Connected Stations

**Ideenspezifische Leitfrage:** In welchen Richtungen ist die Station besonders problematisch?

**Aussage:** Verbindet Stationsqualitaet mit Ursprung/Ziel-Kontext.

**Darstellung / Technische Zuordnung:** Horizontaler `BarChart` fuer Richtungen. Spaeter optional `Sankey` aus `layerchart/graph`, wenn Origin/Station/Destination als Fluss visualisiert werden soll.

**Datenaufwand:** Niedrig fuer Ranking mit `DIRECTIONS`; mittel bis hoch fuer echte Sankey-Fluesse, weil die aktuelle Directions-API nur Richtungsknoten aggregiert und keine vollstaendige Flow-Struktur liefert.

**Hinweise:** Aktuell teilweise umgesetzt als Delay-pressure Ranking. Wichtig ist die Unterscheidung `ORIGIN` und `DESTINATION`, damit Nutzer verstehen, ob die Station Start-, Ziel- oder Zwischenkontext ist.

### 12. Line by Hour Matrix

**Ideenspezifische Leitfrage:** Welche Linie ist zu welcher Stunde an dieser Station kritisch?

**Aussage:** Kombiniert Linien- und Tageszeitperspektive.

**Darstellung / Technische Zuordnung:** Matrix mit `<Chart>` und `Cell` oder `ScatterChart` mit Radius fuer Volumen und Farbe fuer Qualitaet. Controls fuer Metric und Top-N-Linien.

**Datenaufwand:** Niedrig mit `LINE_HOUR_MATRIX`.

**Hinweise:** Aktuell teilweise umgesetzt als `ScatterChart`. Fuer mobile Lesbarkeit sollte Top-N begrenzt bleiben und die Legende kompakt sein. Farbe und Radius muessen klar getrennte Bedeutungen haben.

### 13. Station Delay Distribution

**Ideenspezifische Leitfrage:** Wie verteilen sich konkrete Verspaetungen an dieser Station?

**Aussage:** Zeigt, ob die Station wenige extreme Ausreisser oder viele kleine Verspaetungen hat.

**Darstellung / Technische Zuordnung:** `BarChart` fuer Histogramm und optional `LineChart` fuer cumulative view. Bins analog Netzwerk: frueh, 0-5, 5-10, 10-15, 15-30, 30-60, 60+.

**Datenaufwand:** Mittel bis hoch. Aktuell gibt es keine Station-Delay-Distribution-Metrik. Moeglich waere eine neue `STATION_EVENT_DELAY_DISTRIBUTION` analog Netzwerk oder eine Ableitung aus `EVENT_DETAILS`, wobei Paging keine vollstaendige Verteilung erlaubt.

**Hinweise:** Nicht aus paginierten Detaildaten berechnen. Entweder Backend-Aggregat ergaenzen oder weglassen.

### 14. Problem Event Explorer

**Ideenspezifische Leitfrage:** Welche konkreten Stopps erklaeren die auffaelligen Werte?

**Aussage:** Bruecke von aggregierten Panels zu nachvollziehbaren Einzelfaellen.

**Darstellung / Technische Zuordnung:** Tabelle/Explorer, kein LayerChart erforderlich. Filterchips fuer cancelled, late >= 15, late >= 30, replacement, arrival/departure, line.

**Datenaufwand:** Niedrig bis mittel mit `EVENT_DETAILS`; mittel fuer zusaetzliche Filter im UI.

**Hinweise:** Aktuell teilweise umgesetzt als Tabelle. Fuer ein Statistik-Dashboard sollte sie nicht die Hauptaussage tragen, sondern als Audit-/Drilldown-Panel dienen.

### 15. Station Reliability Driver Summary

**Ideenspezifische Leitfrage:** Was sind die wichtigsten Treiber fuer gute oder schlechte Stationsqualitaet?

**Aussage:** Eine kompakte Diagnose: groesste Problem-Linie, schlechteste Tageszeit, schlechteste Richtung, Vergleich zum Netzwerk.

**Darstellung / Technische Zuordnung:** Text-/KPI-Panel mit kleinen Badges, kein zwingender Chart. Optional kleine `BarChart`-Sparklines fuer Top-Treiber.

**Datenaufwand:** Mittel. Kann aus vorhandenen Aggregaten abgeleitet werden, braucht aber robuste Regeln fuer "Treiber" und Mindestvolumen.

**Hinweise:** Sehr gut fuer das Zielbild, aber vorsichtig formulieren: Kein kausaler Claim, sondern "auffaelligster Beitrag im ausgewaehlten Zeitraum".

## Priorisierte Umsetzung

| Prioritaet der Umsetzung | Idee                                            | Umgesetzt | Passt thematisch rein | Begruendung                                                                                                    | Aufwand            |
| ------------------------ | ----------------------------------------------- | --------- | --------------------- | -------------------------------------------------------------------------------------------------------------- | ------------------ |
| P0                       | Station Outcome Summary                         | Nein      | Ja                    | Traegt die zentrale Leitfrage direkt und reduziert Interpretationsaufwand.                                     | Niedrig bis mittel |
| P0                       | KPI Grid mit klarer Customer/Operative Trennung | Nein      | Ja                    | Vorhanden, aber Schwellen- und Perspektivenregeln sind noch nicht durchgaengig klar.                           | Niedrig            |
| P0                       | Stop Reliability Timeline                       | Nein      | Ja                    | Kernverlauf ist vorhanden, braucht Perspektive/Schwellenwahl und konsistente Labels.                           | Niedrig            |
| P1                       | Station vs Netzwerk Benchmark                   | Nein      | Ja                    | API liefert Benchmark, UI nutzt ihn nur in einem KPI-Trend. Ein eigenes Panel staerkt Einordnung.              | Niedrig bis mittel |
| P1                       | Arrival vs Departure Quality                    | Nein      | Ja                    | Bereits vorhanden, aber Customer/Operativ/Ausfall sollten besser getrennt werden.                              | Niedrig            |
| P1                       | Weekday and Hour Heatmap                        | Nein      | Ja                    | Vorhanden als Scatter; ein echtes Raster waere konsistenter und mobiler lesbar.                                | Mittel             |
| P1                       | Line Delay Pressure Ranking                     | Nein      | Ja                    | Wichtig fuer Stationsdiagnose; aktuell auf Delay debt festgelegt.                                              | Niedrig bis mittel |
| P1                       | Line by Hour Matrix                             | Nein      | Ja                    | Starke Verbindung aus Linie und Tageszeit; braucht Metric-Control und mobile Feinschliff.                      | Mittel             |
| P2                       | Delay Severity Timeline                         | Nein      | Ja                    | Erklaert, ob Probleme mild, schwer oder Ausfaelle sind; kann teils aus vorhandenen Metriken abgeleitet werden. | Mittel             |
| P2                       | Hourly Profile                                  | Nein      | Ja                    | Gute Zusammenfassung der Heatmap, aber nachgelagert zur detaillierteren Matrix.                                | Mittel             |
| P2                       | Transport Type Mix and Quality                  | Nein      | Ja                    | Bereits vorhanden, aber Anteil und Qualitaet sollten klarer getrennt werden.                                   | Niedrig            |
| P2                       | Directions / Connected Stations                 | Nein      | Ja                    | Richtungsdiagnose ist relevant, aber weniger zentral als Linie/Zeit.                                           | Niedrig bis mittel |
| P3                       | Station Delay Distribution                      | Nein      | Ja                    | Sehr hilfreich, benoetigt aber neue API-Metrik oder nicht-paginierte Aggregation.                              | Mittel bis hoch    |
| P3                       | Problem Event Explorer                          | Nein      | Ja                    | Als Drilldown wichtig, aber nicht erste Dashboard-Botschaft.                                                   | Niedrig bis mittel |
| P3                       | Station Reliability Driver Summary              | Nein      | Ja                    | Hoher Nutzwert fuer Erklaerung, braucht aber Produktregeln fuer robuste Treiber.                               | Mittel             |
