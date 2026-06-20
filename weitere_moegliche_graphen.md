# Weitere mögliche Graphen zur Zuverlässigkeit und Pünktlichkeit

> **Leitfrage:** Wie zuverlässig und pünktlich funktioniert der öffentliche Personen- und Fernverkehr in Deutschland – insgesamt, nach Verkehrstyp, nach Station, nach Linie und nach Tageszeit?

## Zielbild

Die zentrale Botschaft des Dashboards sollte nicht nur lauten, wie viele Sekunden Verspätung im Mittel entstanden sind. Aus Fahrgastsicht ist entscheidender:

> Wie wahrscheinlich ist es, dass ein geplanter Stopp tatsächlich stattfindet und brauchbar pünktlich ist?

Deshalb sollte die **Kundensicht-Verlässlichkeit** die primäre Kennzahl in Übersichten sein. Die **operative Pünktlichkeit** bleibt wichtig, muss aber als Kennzahl mit einem anderen Nenner sichtbar getrennt werden.

## Begriffe und Nenner

| Kennzahl | Zähler | Nenner | Aussage |
| - | - | - | - |
| Operative Pünktlichkeit | Pünktliche, tatsächlich gefahrene Stopps | Gefahrene Stopps | Wie pünktlich war der durchgeführte Betrieb? |
| Kundensicht-Verlässlichkeit | Nicht ausgefallene und pünktliche Stopps | Alle geplanten Stopps | Wie oft erhielten Fahrgäste die geplante Leistung pünktlich? |

Für die 5-Minuten-Schwelle gilt im aktuellen Datenmodell:

```text
operative_punctuality_5_rate
= gefahrene Stopps mit Verspätung < 360 s / gefahrene Stopps

customer_reliability_5_rate
= gefahrene Stopps mit Verspätung < 360 s / alle geplanten Stopps
```

Damit gilt auch:

```text
customer_reliability_5_rate
= operative_punctuality_5_rate × (1 - cancellation_rate)
```

Die Differenz zwischen beiden Werten ist kein Messfehler, sondern der sichtbare **Ausfall-Malus**. Ein ausgefallener Stopp wird in der Kundensicht nicht neutral behandelt.

Die vorhandenen Schwellen sind technisch eindeutig als `< 360 s` und `< 900 s` definiert. In der Oberfläche sollten sie konsistent als **„unter 6 Minuten (maximal 5:59)“** und **„unter 15 Minuten (maximal 14:59)“** bezeichnet werden.

## Darstellungsregeln für alle Graphen

1. **Kundensicht als Standard:** `customerReliability5Rate` sollte vorausgewählt sein. Operative Pünktlichkeit wird ergänzend oder als expliziter Umschalter angeboten.
2. **Nenner nennen:** Tooltip, Legende oder Untertitel müssen „von geplanten Stopps“ beziehungsweise „von gefahrenen Stopps“ nennen.
3. **Volumen zeigen:** Jeder Vergleich braucht mindestens `plannedEvents` beziehungsweise `plannedJourneys`, damit kleine Stichproben erkennbar bleiben.
4. **Raten gewichtet aggregieren:** Prozentwerte dürfen nicht arithmetisch über Stationen oder Stunden gemittelt werden. Zähler und Nenner müssen summiert und die Rate anschließend neu berechnet werden.
5. **Event- und Journey-Grain trennen:** Stopps beantworten andere Fragen als vollständige Fahrten. Beide Ebenen dürfen nicht in einer Serie vermischt werden.
6. **Ausfälle als eigenes Ergebnis zeigen:** Ausfälle gehören nicht in eine gewöhnliche Verspätungsverteilung, weil für sie kein sinnvoller Verspätungswert existiert.
7. **Unsicherheit sichtbar machen:** Rankings sollten eine Mindestmenge erlauben und im Tooltip immer `n` zeigen. Bei kleinen Mengen sind Konfidenzintervalle oder eine optische Abschwächung sinnvoll.
8. **Datenabdeckung offenlegen:** Navigator beschreibt den erhobenen Datenbestand und nicht automatisch den vollständigen deutschen Verkehr. Filter, Erhebungszeitraum und beobachtetes Volumen müssen sichtbar bleiben.

## Bereits vorhandene Visualisierungen

Bei der Priorisierung sollten bestehende Ansichten nicht unnötig dupliziert werden.

### Netzwerk

- Zeitreihen für Stop-Verlässlichkeit und Journey-Ergebnisse
- Wochentag-Stunden-Heatmap
- Verspätungshistogramm und kumulative Verteilung
- Stations-Hotspotkarte
- Vergleich der Verkehrstypen

### Station

- Zeitreihe der Stop-Verlässlichkeit
- Vergleich von Ankunft und Abfahrt
- Wochentag-Stunden-Heatmap
- Verkehrstyp-Mix
- Linien- und Richtungsrankings
- Linie-Stunde-Matrix
- Event-Detailtabelle

Die Backend-Verträge enthalten darüber hinaus bereits Daten für Linienprofile, Linien-Zeitreihen, Stationsleistung entlang einer Linie, Journey-Kalender, tägliche Journey-Outcomes sowie Median und 95. Perzentil entlang eines Fahrtverlaufs.

## Einordnung der bereits genannten Ideen

| Idee | Nutzen | Datenlage | Empfehlung |
| - | - | - | - |
| Tages-/Kalender-Heatmap für auffällige Betriebstage | Zeigt Regelmäßigkeit und einzelne Störungstage sehr gut | Journey-Kalender und tägliche Outcomes sind bereits vorhanden | **Hoch**, besonders auf Fahrtnummernebene |
| Gestapelte Journey-Outcomes über die Zeit | Macht Nichtankunft und Ausfälle intuitiver als eine mittlere Verspätung | Zeitreihe vorhanden, Outcomes sind in den Aggregaten aber teilweise überlappend | **Hoch**, nach Einführung disjunkter Outcome-Klassen |
| Verspätungsschwere über die Zeit | Trennt kleinere Abweichungen von schweren Störungen | Aus Schwellenwerten teilweise ableitbar; für 6–15 und 15–30 sind zusätzliche Zähler sinnvoll | **Hoch** |
| Betreibervergleich | Beantwortet eine zentrale Verantwortungsfrage | Neue Gruppierung aus `journey_administration_quality_hourly` nötig | **Hoch**, mittlerer API-Aufwand |
| Stations-Scatterplot „Volumen vs. Zuverlässigkeit“ | Zeigt Ausreißer und Stichprobengröße gemeinsam | Technisch einfach | **Mittel**, als Analyseansicht; gegenüber der Karte teilweise redundant |

Für das Journey-Outcome-Stack sollte eine feste, disjunkte Priorität gelten, beispielsweise:

```text
vollständig ausgefallen
> Ziel nicht erreicht
> teilweise ausgefallen, Ziel aber erreicht
> vollständig durchgeführt
```

Die vorhandenen Detaildaten verwenden bereits sinngemäß diese Reihenfolge. Die aggregierten Kennzahlen `partiallyCancelledJourneys` und `destinationNotReachedJourneys` dürfen dagegen nicht direkt addiert werden, weil sich beide Gruppen überschneiden können.

## Neue Graphideen

### 1. Verlässlichkeitsbilanz je 100 geplanter Stopps

**Frage:** Was geschieht mit 100 geplanten Stopps?

**Darstellung:** Ein horizontaler, zu 100 % gestapelter Balken mit vier disjunkten Segmenten:

1. pünktlich unter 6 Minuten,
2. gefahren mit 6 bis unter 15 Minuten Verspätung,
3. gefahren mit mindestens 15 Minuten Verspätung,
4. ausgefallen.

**Ableitung:**

```text
punctual_below_6     = customerReliability5Rate
late_6_to_below_15  = customerReliability15Rate - customerReliability5Rate
late_15_or_more     = 1 - cancellationRate - customerReliability15Rate
cancelled           = cancellationRate
```

**Einsatz:** Gesamtübersicht, Verkehrstyp, Station, Linie und Tageszeit.

**Datenaufwand:** Niedrig. Alle benötigten Raten sind in `EventMetrics` vorhanden.

**Warum priorisieren:** Dieser Graph beantwortet die Leitfrage unmittelbar und macht gleichzeitig klar, dass Ausfall und Verspätung verschiedene Arten nicht erbrachter Leistung sind.

### 2. Pünktlichkeitslücke als Dumbbell-Chart

**Frage:** Wie stark beschönigt die reine Betriebsbetrachtung das Erlebnis der Fahrgäste?

**Darstellung:** Pro Kategorie zwei Punkte auf derselben horizontalen Skala:

- operative Pünktlichkeit unter 6 Minuten,
- Kundensicht-Verlässlichkeit unter 6 Minuten.

Eine Verbindungslinie zeigt die Lücke. Je größer sie ist, desto stärker drücken Ausfälle die Verlässlichkeit.

**Einsatz:** Besonders geeignet für Verkehrstypen, Betreiber, Stationen und Linien. Für Tageszeiten kann dieselbe Logik als zwei Linien über 24 Stunden dargestellt werden.

**Datenaufwand:**

- Verkehrstypen: niedrig, vorhandener API-Result.
- Stationen und Linien: niedrig bis mittel, vorhandene Rankings liefern die Metriken.
- Betreiber: mittel, neuer gruppierter API-Result nötig.

**Hinweis:** Nicht nach der operativen Rate sortieren, sondern nach Kundensicht-Verlässlichkeit oder Größe der Lücke.

### 3. Schwellenkurve aus Kundensicht

**Frage:** Mit welcher Wahrscheinlichkeit bleibt ein geplanter Stopp unter einer frei ablesbaren Verspätungsschwelle?

**Darstellung:** Zwei kumulative Kurven über der Verspätung in Minuten:

- operativ: Anteil der gefahrenen Stopps bis zur jeweiligen Schwelle,
- Kundensicht: Anteil aller geplanten Stopps bis zur jeweiligen Schwelle.

Vertikale Markierungen bei 5:59 und 14:59 Minuten verbinden die Kurve mit den KPI-Werten. Die Kundensicht-Kurve endet wegen der Ausfälle unter 100 %.

**Ableitung:** Die vorhandene kumulative Verspätungsverteilung betrachtet gefahrene Stopps. Für die Kundensicht wird jeder kumulative Anteil mit `servedEvents / plannedEvents` gewichtet.

**Datenaufwand:** Niedrig. Event-Summary und Delay-Distribution müssen lediglich mit identischen Filtern kombiniert werden.

**Mehrwert gegenüber dem vorhandenen CDF:** Der heutige CDF erklärt die Verteilung der gefahrenen Stopps. Die zweite Kurve erklärt die tatsächliche Chance auf eine erbrachte und rechtzeitige Leistung.

### 4. Tageszeitprofil mit Volumenband

**Frage:** Zu welchen Uhrzeiten sinkt die Verlässlichkeit, und betrifft das viele oder wenige Fahrgäste beziehungsweise Stopps?

**Darstellung:**

- Linie: Kundensicht-Verlässlichkeit unter 6 Minuten je Stunde,
- optionale zweite Linie: Ausfallquote,
- zurückhaltendes Balken- oder Flächenband: geplante Stopps je Stunde.

Optional werden Montag bis Freitag und Wochenende als Small Multiples getrennt.

**Datenaufwand:** Niedrig bis mittel. Die vorhandenen Wochentag-Stunden-Zellen können nach Stunde über ihre Zähler gewichtet zusammengefasst werden. Ein eigener Stunden-Result wäre langfristig sauberer und kleiner.

**Mehrwert gegenüber der Heatmap:** Die Heatmap findet Muster; das Profil macht Spitzenzeiten und die Größe des betroffenen Betriebs leichter vergleichbar.

### 5. Delay-Debt-Pareto nach Station oder Linie

**Frage:** Welche wenigen Stationen oder Linien verursachen den größten Anteil der aufsummierten Verspätungsminuten?

**Darstellung:** Absteigende Balken für `delayDebtMinutes`, darüber eine kumulative Prozentlinie. Eine 80-%-Marke macht sichtbar, ob sich die Belastung auf wenige Hotspots konzentriert.

**Einsatz:**

- Stationen: Stop-Delay-Debt,
- Linien: je nach Ansicht Stop-Delay-Debt oder Ziel-Delay-Debt; die Art muss im Titel stehen,
- Betreiber: nach API-Erweiterung.

**Datenaufwand:** Niedrig für vorhandene Rankings, mittel für Betreiber.

**Hinweis:** Delay Debt ist eine Belastungskennzahl und keine individuelle Wahrscheinlichkeit. Deshalb immer zusammen mit Volumen und Verlässlichkeitsrate anzeigen.

### 6. Zuverlässigkeits- und Verspätungsprofil entlang einer Linie

**Frage:** An welchen Stationen wird eine Linie unzuverlässig und wo baut sich Verspätung auf oder ab?

**Darstellung:** Gemeinsame Stationsachse entlang des Linienverlaufs, darüber zwei abgestimmte Panels:

1. Kundensicht-Verlässlichkeit und Ausfallquote je Stopp,
2. Median und 95. Perzentil der Verspätung als Linie beziehungsweise Band.

**Datenaufwand:** Niedrig bis mittel. `LINE_STATION_PERFORMANCE`, `JOURNEY_STOP_PROFILE` und `JOURNEY_DELAY_BUILD_UP` liefern wesentliche Teile bereits. Bei mehreren Routenvarianten muss zuerst eine konkrete Variante gewählt werden.

**Warum priorisieren:** Der Graph beantwortet „nach Linie“ und „nach Station“ gleichzeitig und zeigt, ob Probleme punktuell entstehen oder sich über den Fahrtverlauf fortpflanzen.

### 7. Verspätungsaufbau zwischen Ankunft und Abfahrt

**Frage:** An welchen Stationen gewinnt eine Fahrt Zeit zurück, und wo kommt zusätzliche Verspätung hinzu?

**Darstellung:** Pro Stationshalt ein divergierender Balken:

```text
delay_change = departure_delay - arrival_delay
```

Negative Werte bedeuten Erholung, positive Werte zusätzlichen Verspätungsaufbau. Median und 95. Perzentil sollten getrennt oder als Small Multiples gezeigt werden.

**Datenaufwand:** Mittel bis hoch. Ankunft und Abfahrt müssen auf Journey-, Stations- und Servicetagsebene gepaart und anschließend robust aggregiert werden.

**Wichtiger Vorbehalt:** Endhaltestellen, ausgefallene Events und Halte ohne beide Zeitpunkte dürfen nicht als Nulländerung interpretiert werden.

### 8. Stabilitätsband statt nur Durchschnittslinie

**Frage:** Ist ein Verkehrstyp oder eine Linie zuverlässig stabil, oder schwankt die Qualität stark von Tag zu Tag?

**Darstellung:**

- Mittellinie: Median der täglichen Kundensicht-Verlässlichkeit,
- Band: 10. bis 90. Perzentil oder 25. bis 75. Perzentil,
- Punkte: besonders schlechte Betriebstage.

**Datenaufwand:** Mittel. Zunächst pro Kalendertag korrekt aggregieren, dann Quantile über die Tageswerte bilden. Die vorhandene normale Zeitreihe allein liefert noch keine Quantilbänder.

**Mehrwert:** Zwei Linien mit gleichem Mittelwert können für Fahrgäste völlig unterschiedlich vorhersehbar sein.

### 9. Rangveränderung zwischen zwei Zeiträumen

**Frage:** Welche Stationen, Linien oder Verkehrstypen haben sich gegenüber dem Vergleichszeitraum verbessert oder verschlechtert?

**Darstellung:** Slopegraph oder Dumbbell-Chart mit aktuellem und vorherigem Zeitraum. Sinnvolle Sortierungen sind:

- Veränderung der Kundensicht-Verlässlichkeit,
- Veränderung der Ausfallquote,
- Veränderung des Delay Debt.

**Datenaufwand:** Niedrig bis mittel. Zwei bestehende Ranking-Abfragen mit identischen Filtern und verschobenem Zeitraum können clientseitig verbunden werden. Für stabile Vergleiche sollte in beiden Perioden dieselbe Mindestmenge gelten.

### 10. Betreibervergleich als Zuverlässigkeitsprofil

**Frage:** Welche Betreiber erbringen die geplante Leistung am verlässlichsten?

**Darstellung:** Keine einzelne Rangliste, sondern ein kompaktes Profil je Betreiber:

- Kundensicht-Verlässlichkeit unter 6 Minuten,
- operative Pünktlichkeit unter 6 Minuten,
- Ausfallquote,
- Journey Completion Rate,
- geplantes Volumen.

Geeignet sind Dumbbell-Chart plus Volumenpunkt oder Small-Multiple-Balken. Ein Radar-Chart sollte vermieden werden, weil Unterschiede und Rangfolgen dort schwer exakt lesbar sind.

**Datenaufwand:** Mittel. Gruppierter API-Result aus `journey_administration_quality_hourly`; für Stop-Kennzahlen gegebenenfalls die entsprechende Event-Aggregation ergänzen.

### 11. Origin-Destination-Matrix für Korridore

**Frage:** Zwischen welchen Start-Ziel-Kombinationen ist eine Linie oder ein Verkehrstyp besonders unzuverlässig?

**Darstellung:** Matrix mit Ursprung in den Zeilen und Ziel in den Spalten. Farbe zeigt Kundensicht-Verlässlichkeit oder Journey Completion, Tooltip zeigt Volumen, Ausfälle und Zielverspätung.

**Datenaufwand:**

- Innerhalb einer Linie: mittel, Routenvarianten sind bereits modelliert.
- Netzwerkweit: hoch, weil eine stark kardinale Origin-Destination-Aggregation und Begrenzung auf relevante Korridore nötig ist.

**Hinweis:** Nur Kombinationen oberhalb einer Mindestmenge darstellen; ansonsten wird die Matrix visuell groß und statistisch dünn.

### 12. Datenabdeckungs- und Stichproben-Chart

**Frage:** Wie belastbar ist die gezeigte Qualität in diesem Zeitraum und Filter?

**Darstellung:** Schmale Begleitvisualisierung unter Zeitreihen:

- geplante beziehungsweise beobachtete Events pro Zeitbucket,
- Anteil Events mit nutzbarer Verspätungsinformation,
- Markierung von Erhebungslücken oder pausierter Datensammlung.

**Datenaufwand:** Mittel. Das Volumen ist vorhanden; eine explizite Kennzahl für Delay-Samples und Erhebungsstatus sollte ergänzt werden.

**Warum wichtig:** Ein scheinbar sehr guter Tag mit wenigen beobachteten Events darf nicht dieselbe visuelle Autorität wie ein vollständig beobachteter Betriebstag erhalten.

## Priorisierte Umsetzung

| Priorität | Graph | Umgesetzt | Passt thematisch rein | Begründung | API-Aufwand |
| - | - | - | - | - | - |
| P0 | Verlässlichkeitsbilanz je 100 Stopps | **Vollständig** – Gesamtbilanz und Verkehrstypvergleich vorhanden | **Ja** | Stärkste direkte Antwort auf die Leitfrage | Niedrig |
| P0 | Pünktlichkeitslücke operativ vs. Kundensicht | **Teilweise** – Kennzahlen in KPIs, Zeitreihe, Karte und Verkehrstyp-Tooltip, aber kein Dumbbell-Chart | **Ja** | Erklärt die zwei Pünktlichkeitsbegriffe unmittelbar | Niedrig für vorhandene Gruppierungen |
| P0 | Schwellenkurve aus Kundensicht | **Teilweise** – operative CDF vorhanden, Kundensicht-Kurve fehlt | **Ja** | Nutzt vorhandenen CDF weiter und behandelt Ausfälle korrekt | Niedrig |
| P0 | Tageszeitprofil mit Volumenband | **Teilweise** – Wochentag-Stunden-Heatmap und Volumenansicht vorhanden, aber kein Stundenprofil | **Ja** | Ergänzt die Heatmap um eine leicht lesbare Spitzenzeitenansicht | Niedrig bis mittel |
| P1 | Journey-Kalender | **Nein** – API-Vertrag vorhanden, keine Visualisierung | **Bedingt** – eher Fahrtnummernansicht | Vorhandene Daten, hoher Nutzen auf Fahrtnummernebene | Niedrig |
| P1 | Disjunkte Journey-Outcomes über die Zeit | **Teilweise** – überlappende Journey-Signale als Zeitreihe, aber kein disjunkter Stack | **Ja** | Macht vollständige und teilweise Nichterbringung sichtbar | Mittel |
| P1 | Verspätungsschwere über die Zeit | **Teilweise** – Schwellenzeitreihe und Gesamtverteilung vorhanden, aber keine Schwereklassen über die Zeit | **Ja** | Unterscheidet normale Abweichung und schwere Störung | Mittel |
| P1 | Linienverlaufsprofil | **Nein** – API-Verträge vorhanden, keine Visualisierung | **Bedingt** – benötigt Linie und Routenvariante | Sehr konkrete Diagnose nach Linie und Station | Niedrig bis mittel |
| P1 | Delay-Debt-Pareto | **Teilweise** – Delay Debt als Kennzahl vorhanden, aber kein Pareto | **Ja** | Priorisiert betriebliche Hotspots nach Gesamtwirkung | Niedrig bis mittel |
| P1 | Betreibervergleich | **Nein** | **Bedingt** – fachlich relevant, aber nicht explizit Teil der Leitfrage | Hoher fachlicher Wert und gute Vergleichbarkeit | Mittel |
| P2 | Stabilitätsband | **Nein** | **Ja** | Ergänzt Niveau um Vorhersagbarkeit | Mittel |
| P2 | Rangveränderung | **Teilweise** – Vorperiodenvergleich nur in den Gesamt-KPIs | **Ja** | Zeigt Entwicklung statt nur Zustand | Niedrig bis mittel |
| P2 | Verspätungsaufbau Ankunft vs. Abfahrt | **Nein** – Journey-Profildaten vorhanden, aber keine gepaarte Visualisierung | **Bedingt** – eher Linien- oder Journey-Detail | Erklärt Mechanismen entlang einer Fahrt | Mittel bis hoch |
| P2 | Origin-Destination-Matrix | **Nein** | **Bedingt** – benötigt Linie oder begrenzten Korridor | Starke Korridoranalyse, aber hohe Kardinalität | Mittel bis hoch |
| Querschnitt | Datenabdeckung und Stichprobe | **Teilweise** – Volumen, Delay-Samples und Abdeckungshinweise vorhanden, aber kein eigener Chart | **Ja** | Verhindert überzogene Aussagen aus dünnen Daten | Mittel |

## Empfohlene erste Kombination für das Netzwerk-Dashboard

Eine besonders aussagekräftige erste Erweiterung wäre:

1. **Verlässlichkeitsbilanz je 100 geplante Stopps** als zentrale Zusammenfassung,
2. **Pünktlichkeitslücke nach Verkehrstyp** als Vergleich,
3. **Tageszeitprofil mit Volumen** für zeitliche Muster,
4. **Schwellenkurve aus Kundensicht** als Vertiefung der Verspätungsverteilung.

Diese vier Graphen verwenden überwiegend vorhandene Daten, decken Gesamtbild, Verkehrstyp und Tageszeit ab und verankern die Trennung von operativer Pünktlichkeit und Kundensicht-Verlässlichkeit im gesamten Dashboard.

## Technische Zuordnung zu LayerChart

| Graph | Geeignete LayerChart-Abstraktion |
| - | - |
| Verlässlichkeitsbilanz | `BarChart` mit gestapelten Serien und horizontaler Orientierung |
| Pünktlichkeitslücke | `Chart` mit Punkten und verbindenden Linien oder kompakter `ScatterChart`-Komposition |
| Schwellenkurve | `LineChart` mit zwei Serien und Schwellenmarkierungen |
| Tageszeitprofil | `Chart` mit Linie und zurückhaltenden Balken auf gemeinsamer X-Achse |
| Delay-Debt-Pareto | `Chart` mit Balken plus kumulativer Linie |
| Linienverlaufsprofil | `Chart` beziehungsweise zwei synchronisierte Charts mit gemeinsamer Stationsreihenfolge |
| Stabilitätsband | `AreaChart` für das Quantilband plus Linie für den Median |
| Rangveränderung | `Chart` mit Linien und Endpunkten oder horizontale Dumbbell-Komposition |
| Origin-Destination-Matrix | `Chart` mit rechteckigen Zellen und bandbasierten Achsen |

Die konventionellen Diagramme sollten mit den vereinfachten LayerChart-Komponenten umgesetzt werden. Für kombinierte Marks, Schwellenlinien und synchronisierte Panels ist `<Chart>` mit eigenen Layern die passendere Abstraktion.
