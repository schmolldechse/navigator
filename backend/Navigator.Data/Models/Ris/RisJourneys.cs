using System.Text.Json;

namespace Navigator.Data.Models.Ris;

public class RisJourneys
{
    /// <summary>
    /// Information on the operator [Betreiber] and the administration [Verwaltung].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Administration
    {

        /// <summary>
        /// Unique id of the administration [Verwaltung].
        /// <br/>- 8 (S - S-Bahn Berlin)
        /// <br/>- 19 (EST - EUROSTAR)
        /// <br/>- 51 (PKP - PKP Intercity)
        /// <br/>- 53 (DPN - Nahreisezug)
        /// <br/>- 54 (CD - Ceske Drahy)
        /// <br/>- 55 (MAV - MAV)
        /// <br/>- 56 (ZSS - ZSSK)
        /// <br/>- 71 (REN - RENFE)
        /// <br/>- 74 (SJ - SJ)
        /// <br/>- 78 (HZ - HZPP)
        /// <br/>- 79 (SZ - Slovenske zeleznice)
        /// <br/>- 80 (DB - DB Fernverkehr AG)
        /// <br/>- 81 (ÖBB - Österreichische Bundesbahnen)
        /// <br/>- 82 (CFL - CFL)
        /// <br/>- 83 (TI - Trenitalia)
        /// <br/>- 84 (NS - Nederlandse Spoorwegen)
        /// <br/>- 85 (SBB - SBB)
        /// <br/>- 86 (DSB - Dänische Staatsbahnen)
        /// <br/>- 87 (SCF - SNCF)
        /// <br/>- 88 (SCB - SNCB)
        /// <br/>- 3018 (THA - THALYS)
        /// <br/>- 3189 (ARV - ARRIVA vlaky)
        /// <br/>- 3230 (VBG - vogtlandbahn - Die Länderbahn GmbH DLB)
        /// <br/>- 3236 (WB - WESTbahn)
        /// <br/>- 3246 (IC - RegioJet)
        /// <br/>- 3270 (TN - TRENORD)
        /// <br/>- 3288 (GW - GW Train Regio)
        /// <br/>- 3332 (KZC - KZC Doprava s.r.o.)
        /// <br/>- 3393 (TGV - SNCF Voyages Deutschland)
        /// <br/>- 3613 (StB - Steiermarkbahn und Bus GmbH)
        /// <br/>- 80001 (S - S-Bahn Berlin)
        /// <br/>- 550043 (GyS - GySEV)
        /// <br/>- 743051 (ST - Snälltåget)
        /// <br/>- 800151 (DB - DB Regio AG Nordost)
        /// <br/>- 800153 (DB - DB Regio AG Nordost)
        /// <br/>- 800154 (DB - DB Regio AG Nordost)
        /// <br/>- 800155 (DB - DB Regio AG Nordost)
        /// <br/>- 800156 (DB - DB Regio AG Nordost)
        /// <br/>- 800157 (DB - DB Regio AG Nordost)
        /// <br/>- 800158 (DB - DB Regio AG Nordost)
        /// <br/>- 800159 (DB - DB Regio AG Nordost)
        /// <br/>- 800160 (DB - DB Regio AG Nordost)
        /// <br/>- 800161 (DB - DB Regio AG Nordost)
        /// <br/>- 800163 (DB - DB Regio AG Nordost)
        /// <br/>- 800165 (DB - DB Regio AG Nordost)
        /// <br/>- 800166 (DB - DB Regio AG Nordost)
        /// <br/>- 800201 (DB - DB Regio AG Nord)
        /// <br/>- 800271 (DB - DB Regio AG Nord)
        /// <br/>- 800279 (DB - DB Regio AG Nord)
        /// <br/>- 800292 (DB - DB Regio AG Nord)
        /// <br/>- 800293 (DB - DB Regio AG Nord)
        /// <br/>- 800295 (DB - DB Regio AG Nord)
        /// <br/>- 800310 (DB - DB Regio AG NRW)
        /// <br/>- 800318 (DB - DB Arriva)
        /// <br/>- 800333 (DB - DB Regio AG NRW)
        /// <br/>- 800337 (DB - DB Regio AG NRW)
        /// <br/>- 800338 (DB - DB Regio AG NRW)
        /// <br/>- 800348 (DB - DB Regio AG NRW)
        /// <br/>- 800349 (DB - DB Regio AG NRW)
        /// <br/>- 800351 (DB - DB Regio AG NRW)
        /// <br/>- 800352 (DB - DB Regio AG NRW)
        /// <br/>- 800354 (DB - DB Regio AG NRW)
        /// <br/>- 800363 (DB - DB Regio AG NRW)
        /// <br/>- 800413 (DB - DB Regio AG Südost)
        /// <br/>- 800417 (DB - DB Regio AG Südost)
        /// <br/>- 800430 (EGB - DB RegioNetz Verkehrs GmbH Erzgebirgsbahn)
        /// <br/>- 800445 (DB - DB Regio AG Südost)
        /// <br/>- 800456 (DB - DB Regio AG Südost)
        /// <br/>- 800469 (DB - DB Regio AG Südost)
        /// <br/>- 800478 (DB - DB Regio AG Südost)
        /// <br/>- 800486 (DB - DB Regio AG Südost)
        /// <br/>- 800487 (DB - DB Regio AG Südost)
        /// <br/>- 800489 (DB - DB Regio AG Südost)
        /// <br/>- 800523 (KHB - DB RegioNetz Verkehrs GmbH Kurhessenbahn)
        /// <br/>- 800528 (S - DB Regio AG S-Bahn Rhein-Main)
        /// <br/>- 800535 (DB - DB Regio AG Mitte)
        /// <br/>- 800553 (DB - DB Regio AG Mitte)
        /// <br/>- 800571 (DB - DB Regio AG Mitte)
        /// <br/>- 800572 (DB - DB Regio AG Mitte)
        /// <br/>- 800574 (DB - DB Regio AG Mitte)
        /// <br/>- 800603 (WFB - DB RegioNetz Verkehrs GmbH Westfrankenbahn)
        /// <br/>- 800622 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 800631 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 800632 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 800640 (SWX - DB Regio AG Mitte SÜWEX)
        /// <br/>- 800643 (S - DB Regio AG S-Bahn Stuttgart)
        /// <br/>- 800647 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 800659 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 800693 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 800694 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 800714 (DB - DB Regio AG Bayern)
        /// <br/>- 800720 (DB - DB Regio AG Bayern)
        /// <br/>- 800721 (DB - DB Regio AG Bayern)
        /// <br/>- 800725 (S - DB Regio AG S-Bahn München)
        /// <br/>- 800734 (DB - DB Regio AG Bayern)
        /// <br/>- 800742 (DB - DB Regio AG Bayern)
        /// <br/>- 800746 (DB - DB Regio AG Bayern)
        /// <br/>- 800755 (DB - DB Regio AG Bayern)
        /// <br/>- 800759 (DB - DB Regio AG Bayern)
        /// <br/>- 800765 (DB - DB Regio AG Bayern)
        /// <br/>- 800767 (DB - DB Regio AG Bayern)
        /// <br/>- 800772 (DB - DB Regio AG Bayern)
        /// <br/>- 800785 (DB - DB Regio AG Bayern)
        /// <br/>- 800790 (DB - DB Regio AG Bayern)
        /// <br/>- 801512 (DB - DB Regio AG Mitte)
        /// <br/>- 801513 (DB - DB Regio AG Mitte)
        /// <br/>- 801518 (DB - DB Regio AG Mitte)
        /// <br/>- 801526 (DB - DB Regio AG Mitte)
        /// <br/>- 801539 (DB - DB Regio AG Mitte)
        /// <br/>- 801566 (DB - DB Regio AG Mitte)
        /// <br/>- 801591 (DB - DB Regio AG Mitte)
        /// <br/>- 801599 (DB - DB Regio AG Mitte)
        /// <br/>- 810003 (mbs - Montafoner Bahn)
        /// <br/>- 810005 (ZB - Zillertalbahn)
        /// <br/>- 810007 (SLB - Salzburger Lokalbahnen)
        /// <br/>- 810008 (STH - Stern &amp; Hafferl Verkehrs-GmbH)
        /// <br/>- 810009 (WiL - Wiener Linien)
        /// <br/>- 810011 (R - Schneebergbahn)
        /// <br/>- 810017 (NÖV - NÖ Verkehrsorganisations-ges.m.b.H.)
        /// <br/>- 810021 (NÖV - NÖ Verkehrsorganisations-ges.m.b.H.)
        /// <br/>- 810023 (NÖV - NÖ Verkehrsorganisations-ges.m.b.H.)
        /// <br/>- 810024 (P - Waldviertler Schmalspurbahn)
        /// <br/>- 810025 (SLB - Salzburger Lokalbahnen)
        /// <br/>- 810028 (GyS - GySEV)
        /// <br/>- 810031 (STR - Linz Linien AG (Straßenbahn Stadt Linz))
        /// <br/>- 810043 (ÖPO - ÖBB-Postbus)
        /// <br/>- 840037 (Rnt - R-net)
        /// <br/>- 840052 (Brg - Breng)
        /// <br/>- 840054 (Vll - Valleilijn)
        /// <br/>- 840055 (BN - Blauwnet)
        /// <br/>- 840100 (NS - Nederlandse Spoorwegen)
        /// <br/>- 840500 (ARR - Arriva Nederland)
        /// <br/>- 850022 (AB - Appenzeller Bahnen)
        /// <br/>- 850023 (TPC - Transports Publics du Chablais)
        /// <br/>- 850029 (MBC - Transports de la région Morges-Bière-Cossonay)
        /// <br/>- 850031 (BDW - BDWM Transport)
        /// <br/>- 850032 (BLM - Lauterbrunnen-Mürren)
        /// <br/>- 850033 (BLS - BLS AG)
        /// <br/>- 850035 (BOB - Berner Oberland-Bahnen)
        /// <br/>- 850038 (ASM - Aare Seeland mobil)
        /// <br/>- 850042 (MVR - Montreux-Vevey-Riviera)
        /// <br/>- 850043 (CJ - Chemins de fer du Jura)
        /// <br/>- 850044 (TRN - Transports Publics Neuchâtelois SA)
        /// <br/>- 850046 (FB - Forchbahn)
        /// <br/>- 850047 (FLP - Lugano-Ponte Tresa)
        /// <br/>- 850048 (MGB - Matterhorn Gotthard Bahn (fo))
        /// <br/>- 850049 (FAR - Ferrovie Autolinee Regionali Ticinesi)
        /// <br/>- 850051 (FW - Frauenfeld-Wil)
        /// <br/>- 850053 (TPF - Transports publics fribourgeois)
        /// <br/>- 850055 (LEB - Lausanne-Echallens-Bercher)
        /// <br/>- 850056 (ASM - Aare Seeland mobil)
        /// <br/>- 850061 (TMR - Transports de Martigny et Régions (mc))
        /// <br/>- 850064 (MOB - Montreux-Oberland Bernois)
        /// <br/>- 850065 (THU - THURBO)
        /// <br/>- 850066 (NSt - Nyon-St-Cergue-Morez)
        /// <br/>- 850072 (RhB - Rhätische Bahn)
        /// <br/>- 850073 (TRN - Transports Publics Neuchâtelois SA)
        /// <br/>- 850074 (RA - Regionalps)
        /// <br/>- 850078 (SZU - Sihltal-Zürich-Uetliberg-Bahn)
        /// <br/>- 850081 (ASM - Aare Seeland mobil)
        /// <br/>- 850082 (SOB - Schweizerische Südostbahn (sob))
        /// <br/>- 850086 (ZB - Zentralbahn)
        /// <br/>- 850088 (RBS - Regionalverkehr Bern-Solothurn)
        /// <br/>- 850093 (MGB - Matterhorn Gotthard Bahn (bvz))
        /// <br/>- 850096 (WSB - Wynental-und Suhrental-Bahn)
        /// <br/>- 850097 (TRA - Transports Vallée de Joux-Yverdon-Ste-Croix)
        /// <br/>- 850193 (URh - Untersee und Rhein)
        /// <br/>- 850195 (SBS - Schweizerische Bodensee-Schiffahrtsgesellschaft)
        /// <br/>- 850360 (BSB - Bodensee-Schiffsbetriebe)
        /// <br/>- 850371 (BC - Société coopérative du Chemin de fer - Musée Blonay-Chamby)
        /// <br/>- 850801 (PAG - PostAuto Schweiz)
        /// <br/>- 850846 (RVS - Regionale Verkehrsbetriebe Schaffhausen)
        /// <br/>- 853186 (SZR - Schiff Eglisau-Tössegg)
        /// <br/>- 853271 (ANA - Association neuchâteloise des Amis du Tramway ANAT)
        /// <br/>- 857200 (SBB - SBB)
        /// <br/>- 857206 (SBB - SBB)
        /// <br/>- 857210 (SBB - SBB)
        /// <br/>- 857221 (THU - THURBO)
        /// <br/>- 857231 (SBB - SBB)
        /// <br/>- 859014 (VDB - Verein Dampfbahn Bern)
        /// <br/>- 859999 (SBB - SBB)
        /// <br/>- 860087 (ARR - Arriva Danmark)
        /// <br/>- 861002 (DSB - Dänische Staatsbahnen)
        /// <br/>- 8006000 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 0S (S - S-Bahn Hamburg)
        /// <br/>- 51KD (KD - Koleje Dolnoslaskie)
        /// <br/>- 51PR (PR - Polregio)
        /// <br/>- 51PR-Q (PR - Polregio)
        /// <br/>- 51PR-R (PR - Polregio)
        /// <br/>- 51PR-S (PR - Polregio)
        /// <br/>- 8002A3 (DB - DB Regio AG Nord)
        /// <br/>- 8002B5 (DB - DB Regio AG Nord)
        /// <br/>- 8003A5 (DB - DB Regio AG NRW)
        /// <br/>- 8003G1 (DB - DB Regio AG NRW)
        /// <br/>- 8003G2 (DB - DB Regio AG NRW)
        /// <br/>- 8003H5 (DB - DB Regio AG NRW)
        /// <br/>- 8003L1 (DB - DB Regio AG NRW)
        /// <br/>- 8003L2 (DB - DB Regio AG NRW)
        /// <br/>- 8003RL (DB - DB Regio AG NRW)
        /// <br/>- 8003S (DB - DB Regio AG NRW)
        /// <br/>- 8004A9 (DB - DB Regio AG Südost)
        /// <br/>- 8004L1 (DB - DB Regio AG Südost)
        /// <br/>- 8004NT (DB - DB Regio AG Südost)
        /// <br/>- 8004OB (OBS - DB RegioNetz Verkehrs GmbH Oberweißbacher Berg+Schwarzatalbahn)
        /// <br/>- 8005A4 (DB - DB Regio AG Mitte)
        /// <br/>- 8005KG (DB - DB Regio AG Mitte)
        /// <br/>- 8005MW (DB - DB Regio AG Mitte)
        /// <br/>- 8005ND (DB - DB Regio AG Mitte)
        /// <br/>- 8005SV (DB - Rhein-Mosel-Bus Ahrweiler)
        /// <br/>- 8006A7 (WFB - DB RegioNetz Verkehrs GmbH Westfrankenbahn)
        /// <br/>- 8006C4 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 8006C5 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 8006C6 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 8006D1 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 8006D2 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 8006D6 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 8006D8 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 8006SH (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- 8007D4 (DB - DB Regio AG Bayern)
        /// <br/>- 8007D5 (DB - DB Regio AG Bayern)
        /// <br/>- 8007DU (DB - DB Regio AG Bayern)
        /// <br/>- 8007H1 (DB - DB Regio AG Bayern)
        /// <br/>- 8007H2 (DB - DB Regio AG Bayern)
        /// <br/>- 8013D (SOB - DB RegioNetz Verkehrs GmbH Südostbayernbahn)
        /// <br/>- 8013E (SOB - DB RegioNetz Verkehrs GmbH Südostbayernbahn)
        /// <br/>- 8015A1 (DB - DB Regio AG Mitte)
        /// <br/>- 8015A6 (DB - DB Regio AG Mitte)
        /// <br/>- 8015FR (DB - DB Regio AG Mitte)
        /// <br/>- 8015H9 (DB - DB Regio AG Mitte)
        /// <br/>- 80SEV (DB - DB Fernverkehr AG)
        /// <br/>- 80SSP (D - Sylt Shuttle Plus)
        /// <br/>- 80TRI (EC - DB/SBB/TI)
        /// <br/>- 81GE43 (DPN - Nahreisezug)
        /// <br/>- 857LEX (LEX - LEX)
        /// <br/>- 85DBSH (SBB - SBB)
        /// <br/>- A0 (AKN - AKN Eisenbahn GmbH)
        /// <br/>- A5 (VEN - Rhenus Veniro)
        /// <br/>- A6 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6FEX (DPN - Nahreisezug)
        /// <br/>- A6S1 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S11 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S12 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S31 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S32 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S34 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S4 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S41 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S42 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S5 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S51 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S52 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S6 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S7 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S71 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S8 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A6S81 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- A8N (ALX - alex - Die Länderbahn GmbH DLB)
        /// <br/>- A9 (ag - agilis)
        /// <br/>- aav006 (DPN - Nahreisezug)
        /// <br/>- aavARN (DPN - Nahreisezug)
        /// <br/>- aavARR (DPN - Nahreisezug)
        /// <br/>- aavASE (DPN - Nahreisezug)
        /// <br/>- aavDKB (DPN - Nahreisezug)
        /// <br/>- aavNET (DPN - Nahreisezug)
        /// <br/>- aavTEC (DPN - Nahreisezug)
        /// <br/>- AB (ABR - SWEG Bahn Stuttgart GmbH)
        /// <br/>- ABIRE (IRE - SWEG Bahn Stuttgart GmbH)
        /// <br/>- ABRB (RB - SWEG Bahn Stuttgart GmbH)
        /// <br/>- ABRE (RE - SWEG Bahn Stuttgart GmbH)
        /// <br/>- ak_AK (AK - Autokraft)
        /// <br/>- ak_DRN (DIB - Dithmarschenbus (DB Regio Bus Nord GmbH))
        /// <br/>- ak_SVG (SVG - Sylter Verkehrsgesellschaft)
        /// <br/>- ak_SVL (DPN - Nahreisezug)
        /// <br/>- ak_SWN (DPN - Nahreisezug)
        /// <br/>- ak_VIN (DPN - Nahreisezug)
        /// <br/>- ak_VKP (DPN - Nahreisezug)
        /// <br/>- ak_VSF (DPN - Nahreisezug)
        /// <br/>- akAK_A (DPN - Nahreisezug)
        /// <br/>- akAKTI (DPN - Nahreisezug)
        /// <br/>- akDRNA (DPN - Nahreisezug)
        /// <br/>- akECKE (DPN - Nahreisezug)
        /// <br/>- akKVGK (DPN - Nahreisezug)
        /// <br/>- akROH (DPN - Nahreisezug)
        /// <br/>- akSFKK (DPN - Nahreisezug)
        /// <br/>- akTRAN (DPN - Nahreisezug)
        /// <br/>- akVINA (DPN - Nahreisezug)
        /// <br/>- akWDR (DPN - Nahreisezug)
        /// <br/>- AL (DWE - Dessau-Wörlitzer Eisenbahn)
        /// <br/>- ald012 (DPN - Nahreisezug)
        /// <br/>- ald020 (DPN - Nahreisezug)
        /// <br/>- ald021 (DPN - Nahreisezug)
        /// <br/>- ald022 (DPN - Nahreisezug)
        /// <br/>- ald023 (DPN - Nahreisezug)
        /// <br/>- ald024 (DPN - Nahreisezug)
        /// <br/>- ald025 (DPN - Nahreisezug)
        /// <br/>- ald030 (DPN - Nahreisezug)
        /// <br/>- ald044 (DPN - Nahreisezug)
        /// <br/>- ald046 (DPN - Nahreisezug)
        /// <br/>- ald065 (DPN - Nahreisezug)
        /// <br/>- ald067 (DPN - Nahreisezug)
        /// <br/>- ald069 (DPN - Nahreisezug)
        /// <br/>- ald072 (DPN - Nahreisezug)
        /// <br/>- ald073 (DPN - Nahreisezug)
        /// <br/>- ald075 (DPN - Nahreisezug)
        /// <br/>- ald077 (DPN - Nahreisezug)
        /// <br/>- ald083 (DPN - Nahreisezug)
        /// <br/>- ald084 (DPN - Nahreisezug)
        /// <br/>- ald087 (DPN - Nahreisezug)
        /// <br/>- ald091 (DPN - Nahreisezug)
        /// <br/>- ald093 (DPN - Nahreisezug)
        /// <br/>- ald094 (DPN - Nahreisezug)
        /// <br/>- ald095 (DPN - Nahreisezug)
        /// <br/>- ald096 (DPN - Nahreisezug)
        /// <br/>- ald099 (DPN - Nahreisezug)
        /// <br/>- AM (ABR - Abellio Rail Mitteldeutschland GmbH)
        /// <br/>- AMHBX (HBX - Abellio Rail Mitteldeutschland GmbH)
        /// <br/>- AMRB (RB - Abellio Rail Mitteldeutschland GmbH)
        /// <br/>- AMRE (RE - Abellio Rail Mitteldeutschland GmbH)
        /// <br/>- AMS (S - Abellio Rail Mitteldeutschland GmbH)
        /// <br/>- aoeBus (DPN - Nahreisezug)
        /// <br/>- apg__1 (DPN - Nahreisezug)
        /// <br/>- avv009 (DPN - Nahreisezug)
        /// <br/>- avvBus (DPN - Nahreisezug)
        /// <br/>- B1 (DB - DB Regio AG Nord)
        /// <br/>- B1EDZ (DPN - Nahreisezug)
        /// <br/>- B2 (DB - DB Regio AG NRW)
        /// <br/>- B3 (P - Brohltalbahn)
        /// <br/>- B4 (S - DB Regio AG S-Bahn Rhein-Main)
        /// <br/>- B5 (DB - DB Regio AG Mitte)
        /// <br/>- B6 (DB - DB Regio AG Baden-Württemberg)
        /// <br/>- B7 (DPN - Nahreisezug)
        /// <br/>- bacBus (DPN - Nahreisezug)
        /// <br/>- bambus (DPN - Nahreisezug)
        /// <br/>- bayaut (DPN - Nahreisezug)
        /// <br/>- BB (DB - DB Regio AG Nordost)
        /// <br/>- bcl001 (DPN - Nahreisezug)
        /// <br/>- BD (SDG - SDG Sächsische Dampfeisenbahngesellschaft mbH)
        /// <br/>- BE (BE - Bentheimer Eisenbahn)
        /// <br/>- bod000 (DPN - Nahreisezug)
        /// <br/>- bod001 (DPN - Nahreisezug)
        /// <br/>- bod002 (DPN - Nahreisezug)
        /// <br/>- bod003 (DPN - Nahreisezug)
        /// <br/>- bod004 (DPN - Nahreisezug)
        /// <br/>- bod005 (DPN - Nahreisezug)
        /// <br/>- bod006 (DPN - Nahreisezug)
        /// <br/>- bod008 (DPN - Nahreisezug)
        /// <br/>- bod012 (DPN - Nahreisezug)
        /// <br/>- bod013 (DPN - Nahreisezug)
        /// <br/>- bod014 (DPN - Nahreisezug)
        /// <br/>- bod015 (DPN - Nahreisezug)
        /// <br/>- bod018 (DPN - Nahreisezug)
        /// <br/>- bod019 (DPN - Nahreisezug)
        /// <br/>- bod020 (DPN - Nahreisezug)
        /// <br/>- bod031 (DPN - Nahreisezug)
        /// <br/>- BurBus (DPN - Nahreisezug)
        /// <br/>- BW (DB - DB Regio AG Nordost)
        /// <br/>- byr001 (DPN - Nahreisezug)
        /// <br/>- C6 (KTB - Kandertalbahn)
        /// <br/>- C8 (LEO - Chiemgauer Lokalbahn)
        /// <br/>- CD (CB - City-Bahn Chemnitz)
        /// <br/>- cha016 (DPN - Nahreisezug)
        /// <br/>- cobBus (DPN - Nahreisezug)
        /// <br/>- css002 (P - Chiemseebahn)
        /// <br/>- csscss (DPN - Nahreisezug)
        /// <br/>- cw001 (DPN - Nahreisezug)
        /// <br/>- cw010 (DPN - Nahreisezug)
        /// <br/>- CX (MRB - Mitteldeutsche Regiobahn)
        /// <br/>- CXRB (RB - Mitteldeutsche Regiobahn)
        /// <br/>- CXRE (RE - Mitteldeutsche Regiobahn)
        /// <br/>- D3 (RTB - Rurtalbahn)
        /// <br/>- daf005 (DPN - Nahreisezug)
        /// <br/>- dgfBus (DPN - Nahreisezug)
        /// <br/>- drbBUS (DPN - Nahreisezug)
        /// <br/>- E0 (EVB - EVB ELBE-WESER GmbH)
        /// <br/>- E3 (P - Kasbachtalbahn)
        /// <br/>- EB (RB - Erfurter Bahn GmbH)
        /// <br/>- ED (FEG - Freiberger Eisenbahngesellschaft)
        /// <br/>- estbus (DPN - Nahreisezug)
        /// <br/>- etgBus (DPN - Nahreisezug)
        /// <br/>- EX (RE - Erfurter Bahn GmbH)
        /// <br/>- F1 (DPN - Nahreisezug)
        /// <br/>- F7 (RB - Bodensee-Oberschwaben-Bahn)
        /// <br/>- fds002 (DPN - Nahreisezug)
        /// <br/>- fdsBus (DPN - Nahreisezug)
        /// <br/>- FisBus (DPN - Nahreisezug)
        /// <br/>- FLX10 (FLX - FlixTrain)
        /// <br/>- FLX11 (FLX - FlixTrain)
        /// <br/>- FLX15 (FLX - FlixTrain)
        /// <br/>- FLX20 (FLX - FlixTrain)
        /// <br/>- FLX30 (FLX - FlixTrain)
        /// <br/>- FLX35 (FLX - FlixTrain)
        /// <br/>- frg001 (DPN - Nahreisezug)
        /// <br/>- fuebus (DPN - Nahreisezug)
        /// <br/>- fwzBus (DPN - Nahreisezug)
        /// <br/>- GA (GA - Go-Ahead Baden-Württemberg GmbH)
        /// <br/>- GAIRE (IRE - Go-Ahead Baden-Württemberg GmbH)
        /// <br/>- GAMEX (MEX - Go-Ahead Baden-Württemberg GmbH)
        /// <br/>- GARB (RB - Go-Ahead Baden-Württemberg GmbH)
        /// <br/>- GARE (RE - Go-Ahead Baden-Württemberg GmbH)
        /// <br/>- ge2GEV (DPN - Nahreisezug)
        /// <br/>- ge3GEV (DPN - Nahreisezug)
        /// <br/>- geiBus (DPN - Nahreisezug)
        /// <br/>- gf2001 (DPN - Nahreisezug)
        /// <br/>- gfn011 (DPN - Nahreisezug)
        /// <br/>- gfn012 (DPN - Nahreisezug)
        /// <br/>- gfn015 (DPN - Nahreisezug)
        /// <br/>- gfn020 (DPN - Nahreisezug)
        /// <br/>- ghuBus (DPN - Nahreisezug)
        /// <br/>- grhBus (DPN - Nahreisezug)
        /// <br/>- GY (GA - Go-Ahead Bayern GmbH)
        /// <br/>- GYRB (RB - Go-Ahead Bayern GmbH)
        /// <br/>- GYRE (RE - Go-Ahead Bayern GmbH)
        /// <br/>- H4 (RT - RegioTram)
        /// <br/>- H6 (HzL - Hohenzollerische Landesbahn (SWEG))
        /// <br/>- H7 (HzL - Hohenzollerische Landesbahn (SWEG))
        /// <br/>- hggBus (DPN - Nahreisezug)
        /// <br/>- HL (HSB - Harzer Schmalspurbahn)
        /// <br/>- hnv030 (DPN - Nahreisezug)
        /// <br/>- hnv031 (DPN - Nahreisezug)
        /// <br/>- hnv034 (DPN - Nahreisezug)
        /// <br/>- hnv050 (DPN - Nahreisezug)
        /// <br/>- hof004 (DPN - Nahreisezug)
        /// <br/>- hvv001 (DPN - Nahreisezug)
        /// <br/>- hvvDAH (DPN - Nahreisezug)
        /// <br/>- hvvHAD (DPN - Nahreisezug)
        /// <br/>- hvvHHA (DPN - Nahreisezug)
        /// <br/>- hvvHOX (DPN - Nahreisezug)
        /// <br/>- hvvKVI (DPN - Nahreisezug)
        /// <br/>- hvvLIZ (DPN - Nahreisezug)
        /// <br/>- hvvRAO (DPN - Nahreisezug)
        /// <br/>- hvvRMV (DPN - Nahreisezug)
        /// <br/>- hvvVHH (DPN - Nahreisezug)
        /// <br/>- hvvVLP (DPN - Nahreisezug)
        /// <br/>- invBus (DPN - Nahreisezug)
        /// <br/>- K4 (HLB - HLB Hessenbahn GmbH)
        /// <br/>- K4RB (HLB - HLB Hessenbahn GmbH)
        /// <br/>- K4RE (HLB - HLB Hessenbahn GmbH)
        /// <br/>- K6 (AVG - Albtal-Verkehrs-Gesellschaft mbH)
        /// <br/>- KD (KD - Köln-Düsseldorfer Deutsche Rheinschifffahrt GmbH)
        /// <br/>- kis001 (DPN - Nahreisezug)
        /// <br/>- kolbus (DPN - Nahreisezug)
        /// <br/>- kraBus (DPN - Nahreisezug)
        /// <br/>- krgBus (DPN - Nahreisezug)
        /// <br/>- kulBus (DPN - Nahreisezug)
        /// <br/>- kvg001 (DPN - Nahreisezug)
        /// <br/>- kvv002 (DPN - Nahreisezug)
        /// <br/>- kvv003 (DPN - Nahreisezug)
        /// <br/>- kvv004 (DPN - Nahreisezug)
        /// <br/>- kvv006 (DPN - Nahreisezug)
        /// <br/>- kvv010 (DPN - Nahreisezug)
        /// <br/>- kvv011 (DPN - Nahreisezug)
        /// <br/>- kvv012 (DPN - Nahreisezug)
        /// <br/>- kvv015 (DPN - Nahreisezug)
        /// <br/>- kvv017 (DPN - Nahreisezug)
        /// <br/>- kvv021 (DPN - Nahreisezug)
        /// <br/>- kvv023 (DPN - Nahreisezug)
        /// <br/>- kvv024 (DPN - Nahreisezug)
        /// <br/>- kvv025 (DPN - Nahreisezug)
        /// <br/>- kvv027 (DPN - Nahreisezug)
        /// <br/>- kvv028 (DPN - Nahreisezug)
        /// <br/>- kvv030 (DPN - Nahreisezug)
        /// <br/>- kvv041 (DPN - Nahreisezug)
        /// <br/>- kvv22E (DPN - Nahreisezug)
        /// <br/>- kvvFEX (DPN - Nahreisezug)
        /// <br/>- L7 (SBB - SBB GmbH)
        /// <br/>- L8 (BRB - Bayerische Regiobahn)
        /// <br/>- lamBus (DPN - Nahreisezug)
        /// <br/>- lanGEV (DPN - Nahreisezug)
        /// <br/>- LD (TL - trilex  - Die Länderbahn GmbH DLB)
        /// <br/>- LDTLX (TLX - trilex-express - Die Länderbahn GmbH DLB)
        /// <br/>- lklGEV (DPN - Nahreisezug)
        /// <br/>- M1 (P - Museumsbahn)
        /// <br/>- M2 (S - REGIOBAHN)
        /// <br/>- M2RE (R - REGIOBAHN)
        /// <br/>- M4 (VSE - Verein Sächsischer Eisenbahnfreunde)
        /// <br/>- M8 (BRB - Bayerische Regiobahn)
        /// <br/>- M9 (MSB - Mainschleifenbahn)
        /// <br/>- marmar (DPN - Nahreisezug)
        /// <br/>- marovf (DPN - Nahreisezug)
        /// <br/>- marrbk (DPN - Nahreisezug)
        /// <br/>- mvgb10 (DPN - Nahreisezug)
        /// <br/>- mvgb14 (DPN - Nahreisezug)
        /// <br/>- mvgb15 (DPN - Nahreisezug)
        /// <br/>- mvgb16 (DPN - Nahreisezug)
        /// <br/>- mvv099 (DPN - Nahreisezug)
        /// <br/>- mvvEBU (DPN - Nahreisezug)
        /// <br/>- mvvRBU (DPN - Nahreisezug)
        /// <br/>- mvvRFB (DPN - Nahreisezug)
        /// <br/>- MW (MBB - Mecklenburgische Bäderbahn Molli)
        /// <br/>- mzbBUS (DPN - Nahreisezug)
        /// <br/>- N0 (neg - Norddeutsche Eisenbahn Gesellschaft)
        /// <br/>- N1 (NWB - NordWestBahn)
        /// <br/>- N2 (NWB - NordWestBahn)
        /// <br/>- N4 (RB - cantus Verkehrsgesellschaft)
        /// <br/>- N4RE (RE - cantus Verkehrsgesellschaft)
        /// <br/>- N6 (SWE - Südwestdeutsche Landesverkehrs-GmbH)
        /// <br/>- N8 (P - BayernBahn GmbH)
        /// <br/>- nas001 (DPN - Nahreisezug)
        /// <br/>- nas003 (DPN - Nahreisezug)
        /// <br/>- nasBLK (DPN - Nahreisezug)
        /// <br/>- nasBOE (DPN - Nahreisezug)
        /// <br/>- nasDVG (DPN - Nahreisezug)
        /// <br/>- nasFWL (DPN - Nahreisezug)
        /// <br/>- nasHAB (DPN - Nahreisezug)
        /// <br/>- nasHAT (DPN - Nahreisezug)
        /// <br/>- nasHVB (DPN - Nahreisezug)
        /// <br/>- nasHVG (DPN - Nahreisezug)
        /// <br/>- nasKSB (DPN - Nahreisezug)
        /// <br/>- nasLVB (DPN - Nahreisezug)
        /// <br/>- nasLVT (DPN - Nahreisezug)
        /// <br/>- nasMBB (DPN - Nahreisezug)
        /// <br/>- nasMBT (DPN - Nahreisezug)
        /// <br/>- nasMQ (DPN - Nahreisezug)
        /// <br/>- nasNJL (DPN - Nahreisezug)
        /// <br/>- nasNTB (DPN - Nahreisezug)
        /// <br/>- nasOBS (DPN - Nahreisezug)
        /// <br/>- nasOVH (DPN - Nahreisezug)
        /// <br/>- nasRBM (DPN - Nahreisezug)
        /// <br/>- nasRL (DPN - Nahreisezug)
        /// <br/>- nasSAW (DPN - Nahreisezug)
        /// <br/>- nasSDL (DPN - Nahreisezug)
        /// <br/>- nasTHU (DPN - Nahreisezug)
        /// <br/>- nasVET (DPN - Nahreisezug)
        /// <br/>- nasVGS (DPN - Nahreisezug)
        /// <br/>- nasZel (DPN - Nahreisezug)
        /// <br/>- NB (RB - NEB Niederbarnimer Eisenbahn)
        /// <br/>- NBRB12 (RB - NEB Niederbarnimer Eisenbahn)
        /// <br/>- NBRB25 (RB - NEB Niederbarnimer Eisenbahn)
        /// <br/>- NBRB26 (RB - NEB Niederbarnimer Eisenbahn)
        /// <br/>- NBRB27 (RB - NEB Niederbarnimer Eisenbahn)
        /// <br/>- NBRB35 (RB - NEB Niederbarnimer Eisenbahn)
        /// <br/>- NBRB36 (RB - NEB Niederbarnimer Eisenbahn)
        /// <br/>- NBRB54 (RB - NEB Niederbarnimer Eisenbahn)
        /// <br/>- NBRB60 (RB - NEB Niederbarnimer Eisenbahn)
        /// <br/>- NBRB61 (RB - NEB Niederbarnimer Eisenbahn)
        /// <br/>- NBRB62 (RB - NEB Niederbarnimer Eisenbahn)
        /// <br/>- NBRB63 (RB - NEB Niederbarnimer Eisenbahn)
        /// <br/>- nvpBUS (DPN - Nahreisezug)
        /// <br/>- NWBus (NWB - NordWestBahn)
        /// <br/>- NX (NX - National Express)
        /// <br/>- NXRB (RB - National Express)
        /// <br/>- NXRE (RE - National Express)
        /// <br/>- NY (MSM - MSM Partyzug)
        /// <br/>- NYUEX (UEX - Urlaubs-Express)
        /// <br/>- NZ (RE - DB Fernverkehr AG)
        /// <br/>- O0 (NBE - Nordbahn Eisenbahngesellschaft)
        /// <br/>- O7 (ÖBA - Öchsle-Bahn-Betriebsgesellschaft mbH)
        /// <br/>- O9 (OPB - oberpfalzbahn - Die Länderbahn GmbH DLB)
        /// <br/>- O9X (OPX - oberpfalz-express - Die Länderbahn GmbH DLB)
        /// <br/>- OD (SOE - Sächsisch-Oberlausitzer Eisenbahngesellschaft)
        /// <br/>- omp001 (DPN - Nahreisezug)
        /// <br/>- omp003 (DPN - Nahreisezug)
        /// <br/>- omp007 (DPN - Nahreisezug)
        /// <br/>- omp014 (DPN - Nahreisezug)
        /// <br/>- omp022 (DPN - Nahreisezug)
        /// <br/>- omp024 (DPN - Nahreisezug)
        /// <br/>- omp033 (DPN - Nahreisezug)
        /// <br/>- omp035 (DPN - Nahreisezug)
        /// <br/>- omp043 (DPN - Nahreisezug)
        /// <br/>- omp047 (DPN - Nahreisezug)
        /// <br/>- omp048 (DPN - Nahreisezug)
        /// <br/>- omp050 (DPN - Nahreisezug)
        /// <br/>- omp051 (DPN - Nahreisezug)
        /// <br/>- omp052 (DPN - Nahreisezug)
        /// <br/>- omp053 (DPN - Nahreisezug)
        /// <br/>- omp054 (DPN - Nahreisezug)
        /// <br/>- omp055 (DPN - Nahreisezug)
        /// <br/>- omp056 (DPN - Nahreisezug)
        /// <br/>- omp057 (DPN - Nahreisezug)
        /// <br/>- omp058 (DPN - Nahreisezug)
        /// <br/>- omp059 (DPN - Nahreisezug)
        /// <br/>- omp062 (DPN - Nahreisezug)
        /// <br/>- omp063 (DPN - Nahreisezug)
        /// <br/>- omp065 (DPN - Nahreisezug)
        /// <br/>- omp066 (DPN - Nahreisezug)
        /// <br/>- omp067 (DPN - Nahreisezug)
        /// <br/>- omp069 (DPN - Nahreisezug)
        /// <br/>- omp070 (DPN - Nahreisezug)
        /// <br/>- omp071 (DPN - Nahreisezug)
        /// <br/>- omp072 (DPN - Nahreisezug)
        /// <br/>- omp073 (DPN - Nahreisezug)
        /// <br/>- omp074 (DPN - Nahreisezug)
        /// <br/>- omp075 (DPN - Nahreisezug)
        /// <br/>- omp077 (DPN - Nahreisezug)
        /// <br/>- omp079 (DPN - Nahreisezug)
        /// <br/>- omp085 (DPN - Nahreisezug)
        /// <br/>- omp086 (DPN - Nahreisezug)
        /// <br/>- omp088 (DPN - Nahreisezug)
        /// <br/>- omp092 (DPN - Nahreisezug)
        /// <br/>- omp093 (DPN - Nahreisezug)
        /// <br/>- omp094 (DPN - Nahreisezug)
        /// <br/>- omp095 (DPN - Nahreisezug)
        /// <br/>- omp096 (DPN - Nahreisezug)
        /// <br/>- omp098 (DPN - Nahreisezug)
        /// <br/>- omp099 (DPN - Nahreisezug)
        /// <br/>- omsBus (DPN - Nahreisezug)
        /// <br/>- ova002 (DPN - Nahreisezug)
        /// <br/>- ova035 (DPN - Nahreisezug)
        /// <br/>- ovaOVA (DPN - Nahreisezug)
        /// <br/>- ovfOVF (DPN - Nahreisezug)
        /// <br/>- OWBus (OE - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- owl020 (DPN - Nahreisezug)
        /// <br/>- owl021 (DPN - Nahreisezug)
        /// <br/>- owl022 (DPN - Nahreisezug)
        /// <br/>- owl023 (DPN - Nahreisezug)
        /// <br/>- owl024 (DPN - Nahreisezug)
        /// <br/>- owl025 (DPN - Nahreisezug)
        /// <br/>- owl026 (DPN - Nahreisezug)
        /// <br/>- owl027 (DPN - Nahreisezug)
        /// <br/>- owl028 (DPN - Nahreisezug)
        /// <br/>- owl029 (DPN - Nahreisezug)
        /// <br/>- owl031 (STB - moBiel GmbH)
        /// <br/>- owl032 (DPN - Nahreisezug)
        /// <br/>- owl038 (DPN - Nahreisezug)
        /// <br/>- owl039 (DPN - Nahreisezug)
        /// <br/>- owl040 (DPN - Nahreisezug)
        /// <br/>- owl041 (DPN - Nahreisezug)
        /// <br/>- owl042 (DPN - Nahreisezug)
        /// <br/>- owl043 (DPN - Nahreisezug)
        /// <br/>- owl044 (DPN - Nahreisezug)
        /// <br/>- owl045 (DPN - Nahreisezug)
        /// <br/>- owl049 (DPN - Nahreisezug)
        /// <br/>- owl050 (DPN - Nahreisezug)
        /// <br/>- owl051 (DPN - Nahreisezug)
        /// <br/>- owl052 (DPN - Nahreisezug)
        /// <br/>- owl053 (DPN - Nahreisezug)
        /// <br/>- owl054 (DPN - Nahreisezug)
        /// <br/>- owl059 (DPN - Nahreisezug)
        /// <br/>- OWRB (RB - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- OWRB13 (RB - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- OWRB14 (RB - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- OWRB15 (RB - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- OWRB19 (RB - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- OWRB33 (RB - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- OWRB46 (RB - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- OWRB51 (RB - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- OWRB64 (RB - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- OWRB65 (RB - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- OWRE (RE - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- OWRE2 (RE - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- OWRE4 (RE - Ostdeutsche Eisenbahn GmbH)
        /// <br/>- pafBus (DPN - Nahreisezug)
        /// <br/>- PB (RB - Hanseatische Eisenbahn GmbH)
        /// <br/>- pbaATT (DPN - Nahreisezug)
        /// <br/>- pbaitr (DPN - Nahreisezug)
        /// <br/>- pbaKB (ÖPO - ÖBB-Postbus)
        /// <br/>- pbaVVT (DPN - Nahreisezug)
        /// <br/>- pbaW3 (DPN - Nahreisezug)
        /// <br/>- pbaWR (ÖPO - ÖBB-Postbus)
        /// <br/>- prg091 (DPN - Nahreisezug)
        /// <br/>- R0 (ENO - enno)
        /// <br/>- R1 (ME - metronom)
        /// <br/>- R2 (ERB - eurobahn)
        /// <br/>- R2RB (RB - eurobahn)
        /// <br/>- R2RE (RE - eurobahn)
        /// <br/>- R4 (VIA - VIAS Rail GmbH)
        /// <br/>- R4NRN (VIA - VIAS Rail GmbH)
        /// <br/>- R4RB35 (VIA - VIAS Rail GmbH)
        /// <br/>- R4RH (VIA - VIAS GmbH)
        /// <br/>- R4S7 (VIA - VIAS Rail GmbH)
        /// <br/>- R4WEST (VIA - VIAS Rail GmbH)
        /// <br/>- R7 (HzL - Hohenzollerische Landesbahn (SWEG))
        /// <br/>- rabRAB (RAB - Regionalverkehr Alb-Bodensee)
        /// <br/>- rbgAST (SBG - Südbadenbus)
        /// <br/>- rbgBBO (DPN - Nahreisezug)
        /// <br/>- rbgBER (DPN - Nahreisezug)
        /// <br/>- rbgBRN (BRN - Busverkehr Rhein-Neckar)
        /// <br/>- rbgBVH (DPN - Nahreisezug)
        /// <br/>- rbgFAB (DPN - Nahreisezug)
        /// <br/>- rbgFMO (FMO - Friedrich Müller Omnibusunternehmen GmbH)
        /// <br/>- rbgHMO (DPN - Nahreisezug)
        /// <br/>- rbgKNU (DPN - Nahreisezug)
        /// <br/>- rbgOVZ (DPN - Nahreisezug)
        /// <br/>- rbgRBG (DPN - Nahreisezug)
        /// <br/>- rbgRBS (RBS - Regiobus Stuttgart)
        /// <br/>- rbgRVS (RVS - Südwestbus)
        /// <br/>- rbgRVs (RVS - Südwestbus)
        /// <br/>- rbgSBG (SBG - Südbadenbus)
        /// <br/>- rbgWMR (DPN - Nahreisezug)
        /// <br/>- rboMB (DPN - Nahreisezug)
        /// <br/>- rbpORN (ORN - ORN Omnibusverkehr Rhein-Nahe GmbH (Rhein-Nahe-Bus))
        /// <br/>- rbpRMA (DB - Rhein-Mosel-Bus Ahrweiler)
        /// <br/>- rbpRMB (DB - RMB Rhein-Mosel-Bus)
        /// <br/>- rbpRPB (RPB - Rheinpfalzbus)
        /// <br/>- rbpRU1 (DPN - Nahreisezug)
        /// <br/>- rbpRU2 (DPN - Nahreisezug)
        /// <br/>- rbpSAA (DPN - Nahreisezug)
        /// <br/>- rbpSWM (DB - DB Regio Bus Mitte)
        /// <br/>- rbr002 (DPN - Nahreisezug)
        /// <br/>- rbr003 (DPN - Nahreisezug)
        /// <br/>- rbr004 (DPN - Nahreisezug)
        /// <br/>- rbrBOS (DPN - Nahreisezug)
        /// <br/>- rbrOST (DPN - Nahreisezug)
        /// <br/>- rbrSBE (DPN - Nahreisezug)
        /// <br/>- rbrSEV (DPN - Nahreisezug)
        /// <br/>- rbrSNB (DPN - Nahreisezug)
        /// <br/>- RC (AZS - AUTOZUG Sylt)
        /// <br/>- RD (VBG - vogtlandbahn - Die Länderbahn GmbH DLB)
        /// <br/>- rmbwug (DPN - Nahreisezug)
        /// <br/>- rmpREB (DPN - Nahreisezug)
        /// <br/>- rmpRSG (DPN - Nahreisezug)
        /// <br/>- rmpUBB (DPN - Nahreisezug)
        /// <br/>- rmtEVA (DPN - Nahreisezug)
        /// <br/>- rmtEWB (DPN - Nahreisezug)
        /// <br/>- rmtFiS (DPN - Nahreisezug)
        /// <br/>- rmtGVB (DPN - Nahreisezug)
        /// <br/>- rmtIOV (DPN - Nahreisezug)
        /// <br/>- rmtJES (DPN - Nahreisezug)
        /// <br/>- rmtJNV (DPN - Nahreisezug)
        /// <br/>- rmtKOM (DPN - Nahreisezug)
        /// <br/>- rmtLWW (DPN - Nahreisezug)
        /// <br/>- rmtMBB (DPN - Nahreisezug)
        /// <br/>- rmtMKI (DPN - Nahreisezug)
        /// <br/>- rmtNDH (DPN - Nahreisezug)
        /// <br/>- rmtOVG (DPN - Nahreisezug)
        /// <br/>- rmtPVG (DPN - Nahreisezug)
        /// <br/>- rmtRBM (DPN - Nahreisezug)
        /// <br/>- rmtSal (DPN - Nahreisezug)
        /// <br/>- rmtSNG (DPN - Nahreisezug)
        /// <br/>- rmtSWG (DPN - Nahreisezug)
        /// <br/>- rmtTWS (DPN - Nahreisezug)
        /// <br/>- rmtVHO (DPN - Nahreisezug)
        /// <br/>- rmtVLG (DPN - Nahreisezug)
        /// <br/>- rmtVLO (DPN - Nahreisezug)
        /// <br/>- rmtVUS (DPN - Nahreisezug)
        /// <br/>- rmtVUW (DPN - Nahreisezug)
        /// <br/>- rmtVWG (DPN - Nahreisezug)
        /// <br/>- rmtVWO (DPN - Nahreisezug)
        /// <br/>- rmtWER (DPN - Nahreisezug)
        /// <br/>- rmtWGT (DPN - Nahreisezug)
        /// <br/>- rmv001 (DPN - Nahreisezug)
        /// <br/>- rmv007 (DPN - Nahreisezug)
        /// <br/>- rmv019 (DPN - Nahreisezug)
        /// <br/>- rmv020 (DPN - Nahreisezug)
        /// <br/>- rmv031 (DPN - Nahreisezug)
        /// <br/>- rmv045 (DPN - Nahreisezug)
        /// <br/>- rmv053 (DPN - Nahreisezug)
        /// <br/>- rmv061 (DPN - Nahreisezug)
        /// <br/>- rmv087 (DPN - Nahreisezug)
        /// <br/>- rmv099 (DPN - Nahreisezug)
        /// <br/>- rmv106 (DPN - Nahreisezug)
        /// <br/>- rmv117 (DPN - Nahreisezug)
        /// <br/>- rmv156 (DPN - Nahreisezug)
        /// <br/>- rmv158 (DPN - Nahreisezug)
        /// <br/>- rmv162 (DPN - Nahreisezug)
        /// <br/>- rmv163 (DPN - Nahreisezug)
        /// <br/>- rmv165 (DPN - Nahreisezug)
        /// <br/>- rmv168 (DPN - Nahreisezug)
        /// <br/>- rmv196 (DPN - Nahreisezug)
        /// <br/>- rmv211 (DPN - Nahreisezug)
        /// <br/>- rmv218 (DPN - Nahreisezug)
        /// <br/>- rmv222 (DPN - Nahreisezug)
        /// <br/>- rmv223 (DPN - Nahreisezug)
        /// <br/>- rmv224 (DPN - Nahreisezug)
        /// <br/>- rmv234 (DPN - Nahreisezug)
        /// <br/>- rmv238 (DPN - Nahreisezug)
        /// <br/>- rmv242 (DPN - Nahreisezug)
        /// <br/>- rmv243 (DPN - Nahreisezug)
        /// <br/>- rmv251 (DPN - Nahreisezug)
        /// <br/>- rmv254 (DPN - Nahreisezug)
        /// <br/>- rmv255 (DPN - Nahreisezug)
        /// <br/>- rmv257 (DPN - Nahreisezug)
        /// <br/>- rmv258 (DPN - Nahreisezug)
        /// <br/>- rmv260 (DPN - Nahreisezug)
        /// <br/>- rmv264 (DPN - Nahreisezug)
        /// <br/>- rmv265 (DPN - Nahreisezug)
        /// <br/>- rmv269 (DPN - Nahreisezug)
        /// <br/>- rmv272 (DPN - Nahreisezug)
        /// <br/>- rmv275 (DPN - Nahreisezug)
        /// <br/>- rmv277 (DPN - Nahreisezug)
        /// <br/>- rmv278 (DPN - Nahreisezug)
        /// <br/>- rmv282 (DPN - Nahreisezug)
        /// <br/>- rmv283 (DPN - Nahreisezug)
        /// <br/>- rmv284 (DPN - Nahreisezug)
        /// <br/>- rmv289 (DPN - Nahreisezug)
        /// <br/>- rmv290 (DPN - Nahreisezug)
        /// <br/>- rmv293 (DPN - Nahreisezug)
        /// <br/>- rmv294 (DPN - Nahreisezug)
        /// <br/>- rmv295 (DPN - Nahreisezug)
        /// <br/>- rmv297 (DPN - Nahreisezug)
        /// <br/>- rmv301 (DPN - Nahreisezug)
        /// <br/>- rmv304 (DPN - Nahreisezug)
        /// <br/>- rmv305 (DPN - Nahreisezug)
        /// <br/>- rmv306 (DPN - Nahreisezug)
        /// <br/>- rmv307 (DPN - Nahreisezug)
        /// <br/>- rmv308 (DPN - Nahreisezug)
        /// <br/>- rmv309 (DPN - Nahreisezug)
        /// <br/>- rmv316 (DPN - Nahreisezug)
        /// <br/>- rmv317 (DPN - Nahreisezug)
        /// <br/>- rmv319 (DPN - Nahreisezug)
        /// <br/>- rmv322 (DPN - Nahreisezug)
        /// <br/>- rmv323 (DPN - Nahreisezug)
        /// <br/>- rmv333 (DPN - Nahreisezug)
        /// <br/>- rmv392 (DPN - Nahreisezug)
        /// <br/>- rmv394 (DPN - Nahreisezug)
        /// <br/>- rmv395 (DPN - Nahreisezug)
        /// <br/>- rmv396 (DPN - Nahreisezug)
        /// <br/>- rmv397 (DPN - Nahreisezug)
        /// <br/>- rmv398 (DPN - Nahreisezug)
        /// <br/>- rmv399 (DPN - Nahreisezug)
        /// <br/>- rmv400 (DPN - Nahreisezug)
        /// <br/>- rmv401 (DPN - Nahreisezug)
        /// <br/>- rmv403 (DPN - Nahreisezug)
        /// <br/>- rmv404 (DPN - Nahreisezug)
        /// <br/>- rmv405 (DPN - Nahreisezug)
        /// <br/>- rmv406 (DPN - Nahreisezug)
        /// <br/>- rmv408 (DPN - Nahreisezug)
        /// <br/>- rmv412 (DPN - Nahreisezug)
        /// <br/>- rmv413 (DPN - Nahreisezug)
        /// <br/>- rmvALV (DPN - Nahreisezug)
        /// <br/>- rmvARG (DPN - Nahreisezug)
        /// <br/>- rmvBBW (DPN - Nahreisezug)
        /// <br/>- rmvBEC (DPN - Nahreisezug)
        /// <br/>- rmvCBU (DPN - Nahreisezug)
        /// <br/>- rmvDBR (DPN - Nahreisezug)
        /// <br/>- rmvERL (DPN - Nahreisezug)
        /// <br/>- rmvESE (DPN - Nahreisezug)
        /// <br/>- rmvFBB (DPN - Nahreisezug)
        /// <br/>- rmvFGR (DPN - Nahreisezug)
        /// <br/>- rmvFRI (DPN - Nahreisezug)
        /// <br/>- rmvFro (DPN - Nahreisezug)
        /// <br/>- rmvFSB (DPN - Nahreisezug)
        /// <br/>- rmvFUL (DPN - Nahreisezug)
        /// <br/>- rmvGBG (DPN - Nahreisezug)
        /// <br/>- rmvGIB (DPN - Nahreisezug)
        /// <br/>- rmvHEB (DPN - Nahreisezug)
        /// <br/>- rmvHGS (DPN - Nahreisezug)
        /// <br/>- rmvHLB (DPN - Nahreisezug)
        /// <br/>- rmvHTR (DPN - Nahreisezug)
        /// <br/>- rmvJUN (DPN - Nahreisezug)
        /// <br/>- rmvKAE (DPN - Nahreisezug)
        /// <br/>- rmvKOF (DPN - Nahreisezug)
        /// <br/>- rmvKRT (DPN - Nahreisezug)
        /// <br/>- rmvLGG (DPN - Nahreisezug)
        /// <br/>- rmvMUL (DPN - Nahreisezug)
        /// <br/>- rmvNVG (DPN - Nahreisezug)
        /// <br/>- rmvPoh (DPN - Nahreisezug)
        /// <br/>- rmvRAC (DPN - Nahreisezug)
        /// <br/>- rmvREV (DPN - Nahreisezug)
        /// <br/>- rmvRIN (DPN - Nahreisezug)
        /// <br/>- rmvRTO (DPN - Nahreisezug)
        /// <br/>- rmvRTV (DPN - Nahreisezug)
        /// <br/>- rmvSBB (DPN - Nahreisezug)
        /// <br/>- rmvSBO (DPN - Nahreisezug)
        /// <br/>- rmvSFB (DPN - Nahreisezug)
        /// <br/>- rmvSIL (DPN - Nahreisezug)
        /// <br/>- rmvSLM (DPN - Nahreisezug)
        /// <br/>- rmvSWF (DPN - Nahreisezug)
        /// <br/>- rmvSWK (DPN - Nahreisezug)
        /// <br/>- rmvSWN (DPN - Nahreisezug)
        /// <br/>- rmvSWR (DPN - Nahreisezug)
        /// <br/>- rmvTRD (DPN - Nahreisezug)
        /// <br/>- rmvVBB (DPN - Nahreisezug)
        /// <br/>- rmvVES (DPN - Nahreisezug)
        /// <br/>- rmvVIL (DPN - Nahreisezug)
        /// <br/>- rmvVLD (DPN - Nahreisezug)
        /// <br/>- rmvVUW (DPN - Nahreisezug)
        /// <br/>- rmvWB (DPN - Nahreisezug)
        /// <br/>- rmvWIN (DPN - Nahreisezug)
        /// <br/>- rmvWIR (DPN - Nahreisezug)
        /// <br/>- rmvWIS (DPN - Nahreisezug)
        /// <br/>- rmvWZV (DPN - Nahreisezug)
        /// <br/>- rohBus (DPN - Nahreisezug)
        /// <br/>- RS (RE - Regionalverkehre Start Deutschland GmbH)
        /// <br/>- RSNM (RB - Regionalverkehre Start Deutschland GmbH (Start Niedersachsen-Mitte))
        /// <br/>- RSTN (STN - Regionalverkehre Start Deutschland GmbH (Start Taunus))
        /// <br/>- RSUE (RE - Regionalverkehre Start Deutschland GmbH (Start Unterelbe))
        /// <br/>- rvgRVG (DPN - Nahreisezug)
        /// <br/>- rvoRVO (RVO - Regionalverkehr Oberbayern)
        /// <br/>- rvvRVV (DPN - Nahreisezug)
        /// <br/>- RW (PRE - Pressnitztalbahn)
        /// <br/>- S0 (DPN - Nahreisezug)
        /// <br/>- S1 (DPN - Nahreisezug)
        /// <br/>- S3 (SWB - Stadtwerke Bonn)
        /// <br/>- S6 (SWE - SWEG Südwestdeutsche Landesverkehrs-GmbH)
        /// <br/>- S7 (DPN - Nahreisezug)
        /// <br/>- S9 (ag - agilis)
        /// <br/>- SAB (SAB - Schwäbische Alb-Bahn)
        /// <br/>- SAD003 (SAD - Vinschgaubahn)
        /// <br/>- sadBUS (DPN - Nahreisezug)
        /// <br/>- SB (RB - Süd-Thüringen-Bahn GmbH)
        /// <br/>- sbpMU (DPN - Nahreisezug)
        /// <br/>- sbpRW (DPN - Nahreisezug)
        /// <br/>- sbpSK (DPN - Nahreisezug)
        /// <br/>- SBSBUS (DPN - Nahreisezug)
        /// <br/>- sbsbus (DPN - Nahreisezug)
        /// <br/>- SBSIRE (IRE - SWEG Bahn Stuttgart GmbH)
        /// <br/>- SBSMEX (MEX - SWEG Bahn Stuttgart GmbH)
        /// <br/>- SBSRB (RB - SWEG Bahn Stuttgart GmbH)
        /// <br/>- SBSRE (RE - SWEG Bahn Stuttgart GmbH)
        /// <br/>- SBX (RE - Süd-Thüringen-Bahn GmbH)
        /// <br/>- SD (DB - DB Regio AG Südost)
        /// <br/>- smrBus (DPN - Nahreisezug)
        /// <br/>- smrRfb (DPN - Nahreisezug)
        /// <br/>- snp001 (DPN - Nahreisezug)
        /// <br/>- snp002 (DPN - Nahreisezug)
        /// <br/>- spaBus (DPN - Nahreisezug)
        /// <br/>- srlBus (DPN - Nahreisezug)
        /// <br/>- sswssw (DPN - Nahreisezug)
        /// <br/>- sva006 (DPN - Nahreisezug)
        /// <br/>- svaBUS (DPN - Nahreisezug)
        /// <br/>- svaSTR (DPN - Nahreisezug)
        /// <br/>- svr001 (DPN - Nahreisezug)
        /// <br/>- SW (DPN - Nahreisezug)
        /// <br/>- swg099 (SWE - SWEG Südwestdeutsche Landesverkehrs-GmbH)
        /// <br/>- swgSWB (DPN - Nahreisezug)
        /// <br/>- swlbus (DPN - Nahreisezug)
        /// <br/>- swm001 (DPN - Nahreisezug)
        /// <br/>- swm002 (DPN - Nahreisezug)
        /// <br/>- swm003 (DPN - Nahreisezug)
        /// <br/>- swpVBP (DPN - Nahreisezug)
        /// <br/>- swt_31 (DPN - Nahreisezug)
        /// <br/>- swt_32 (DPN - Nahreisezug)
        /// <br/>- swt_33 (DPN - Nahreisezug)
        /// <br/>- swt_35 (DPN - Nahreisezug)
        /// <br/>- swt_36 (DPN - Nahreisezug)
        /// <br/>- swt_37 (DPN - Nahreisezug)
        /// <br/>- swt_38 (DPN - Nahreisezug)
        /// <br/>- swt_39 (DPN - Nahreisezug)
        /// <br/>- swt001 (DPN - Nahreisezug)
        /// <br/>- swt007 (DPN - Nahreisezug)
        /// <br/>- swt009 (DPN - Nahreisezug)
        /// <br/>- swt014 (DPN - Nahreisezug)
        /// <br/>- swt021 (DPN - Nahreisezug)
        /// <br/>- swt022 (DPN - Nahreisezug)
        /// <br/>- swt023 (DPN - Nahreisezug)
        /// <br/>- swt024 (DPN - Nahreisezug)
        /// <br/>- swt025 (DPN - Nahreisezug)
        /// <br/>- swt026 (DPN - Nahreisezug)
        /// <br/>- swt028 (DPN - Nahreisezug)
        /// <br/>- swt030 (DPN - Nahreisezug)
        /// <br/>- swt031 (DPN - Nahreisezug)
        /// <br/>- swt032 (DPN - Nahreisezug)
        /// <br/>- swt033 (DPN - Nahreisezug)
        /// <br/>- swtb00 (DPN - Nahreisezug)
        /// <br/>- swtb01 (DPN - Nahreisezug)
        /// <br/>- swtb03 (DPN - Nahreisezug)
        /// <br/>- swtb12 (DPN - Nahreisezug)
        /// <br/>- swtb16 (DPN - Nahreisezug)
        /// <br/>- swtb19 (DPN - Nahreisezug)
        /// <br/>- swtb20 (DPN - Nahreisezug)
        /// <br/>- swtb21 (DPN - Nahreisezug)
        /// <br/>- swtb22 (DPN - Nahreisezug)
        /// <br/>- swtb24 (DPN - Nahreisezug)
        /// <br/>- swtb25 (DPN - Nahreisezug)
        /// <br/>- swtb27 (DPN - Nahreisezug)
        /// <br/>- swtb29 (DPN - Nahreisezug)
        /// <br/>- T8 (BRB - Bayerische Regiobahn)
        /// <br/>- TDHS (S - S-Bahn Hannover (Transdev))
        /// <br/>- TDRR (RRB - RheinRuhrBahn (Transdev))
        /// <br/>- tgo099 (SWE - SWEG Südwestdeutsche Landesverkehrs-GmbH)
        /// <br/>- TR (RB - MittelrheinBahn (Trans Regio))
        /// <br/>- TRI (TRI - TRI Train Rental GmbH)
        /// <br/>- tub001 (DPN - Nahreisezug)
        /// <br/>- tub003 (DPN - Nahreisezug)
        /// <br/>- tub004 (DPN - Nahreisezug)
        /// <br/>- tub007 (DPN - Nahreisezug)
        /// <br/>- tub008 (DPN - Nahreisezug)
        /// <br/>- tub009 (DPN - Nahreisezug)
        /// <br/>- tub010 (DPN - Nahreisezug)
        /// <br/>- tub015 (DPN - Nahreisezug)
        /// <br/>- tub022 (DPN - Nahreisezug)
        /// <br/>- tub030 (DPN - Nahreisezug)
        /// <br/>- tub031 (DPN - Nahreisezug)
        /// <br/>- tub033 (DPN - Nahreisezug)
        /// <br/>- tub034 (DPN - Nahreisezug)
        /// <br/>- tub035 (DPN - Nahreisezug)
        /// <br/>- tub036 (DPN - Nahreisezug)
        /// <br/>- tub038 (DPN - Nahreisezug)
        /// <br/>- tub041 (DPN - Nahreisezug)
        /// <br/>- tub042 (DPN - Nahreisezug)
        /// <br/>- tub044 (DPN - Nahreisezug)
        /// <br/>- tub048 (DPN - Nahreisezug)
        /// <br/>- tub049 (DPN - Nahreisezug)
        /// <br/>- tub051 (DPN - Nahreisezug)
        /// <br/>- tub053 (DPN - Nahreisezug)
        /// <br/>- tub054 (DPN - Nahreisezug)
        /// <br/>- tub061 (DPN - Nahreisezug)
        /// <br/>- tub068 (DPN - Nahreisezug)
        /// <br/>- tub069 (DPN - Nahreisezug)
        /// <br/>- tub071 (DPN - Nahreisezug)
        /// <br/>- tub072 (DPN - Nahreisezug)
        /// <br/>- tub079 (DPN - Nahreisezug)
        /// <br/>- tub090 (DPN - Nahreisezug)
        /// <br/>- tub093 (DPN - Nahreisezug)
        /// <br/>- tub094 (DPN - Nahreisezug)
        /// <br/>- tub095 (DPN - Nahreisezug)
        /// <br/>- tut001 (DPN - Nahreisezug)
        /// <br/>- UW (UBB - Usedomer Bäderbahn)
        /// <br/>- V6 (vlx - vlexx)
        /// <br/>- V6RB (RB - vlexx)
        /// <br/>- V6RE (RE - vlexx)
        /// <br/>- V7 (SVG - SVG Schienenverkehrsgesellschaft Stuttgart)
        /// <br/>- V9 (P - Wanderbahn im Regental)
        /// <br/>- vabsta (DPN - Nahreisezug)
        /// <br/>- vag010 (DPN - Nahreisezug)
        /// <br/>- vag011 (DPN - Nahreisezug)
        /// <br/>- vag013 (DPN - Nahreisezug)
        /// <br/>- vag014 (DPN - Nahreisezug)
        /// <br/>- vag060 (DPN - Nahreisezug)
        /// <br/>- vanbus (DPN - Nahreisezug)
        /// <br/>- vanstr (DPN - Nahreisezug)
        /// <br/>- vanuba (DPN - Nahreisezug)
        /// <br/>- vbb070 (DPN - Nahreisezug)
        /// <br/>- vbb071 (DPN - Nahreisezug)
        /// <br/>- vbb072 (DPN - Nahreisezug)
        /// <br/>- vbbBBG (DPN - Nahreisezug)
        /// <br/>- vbbBRB (DPN - Nahreisezug)
        /// <br/>- vbbBRT (DPN - Nahreisezug)
        /// <br/>- vbbBVB (DPN - Nahreisezug)
        /// <br/>- vbbBVF (DPN - Nahreisezug)
        /// <br/>- vbbBVT (DPN - Nahreisezug)
        /// <br/>- vbbBVU (DPN - Nahreisezug)
        /// <br/>- vbbCNB (DPN - Nahreisezug)
        /// <br/>- vbbCNT (DPN - Nahreisezug)
        /// <br/>- vbbFFB (DPN - Nahreisezug)
        /// <br/>- vbbFFT (DPN - Nahreisezug)
        /// <br/>- vbbGLA (DPN - Nahreisezug)
        /// <br/>- vbbHVG (DPN - Nahreisezug)
        /// <br/>- vbbMOB (DPN - Nahreisezug)
        /// <br/>- vbbORP (DPN - Nahreisezug)
        /// <br/>- vbbOSL (DPN - Nahreisezug)
        /// <br/>- vbbOVG (DPN - Nahreisezug)
        /// <br/>- vbbREI (DPN - Nahreisezug)
        /// <br/>- vbbRPM (DPN - Nahreisezug)
        /// <br/>- vbbRVS (DPN - Nahreisezug)
        /// <br/>- vbbSCH (DPN - Nahreisezug)
        /// <br/>- vbbSRS (DPN - Nahreisezug)
        /// <br/>- vbbSTE (DPN - Nahreisezug)
        /// <br/>- vbbSTF (DPN - Nahreisezug)
        /// <br/>- vbbSTG (DPN - Nahreisezug)
        /// <br/>- vbbUVG (DPN - Nahreisezug)
        /// <br/>- vbbVEE (DPN - Nahreisezug)
        /// <br/>- vbbVIB (DPN - Nahreisezug)
        /// <br/>- vbbVIF (DPN - Nahreisezug)
        /// <br/>- vbbVIT (DPN - Nahreisezug)
        /// <br/>- vbbVTF (DPN - Nahreisezug)
        /// <br/>- vgb00 (DPN - Nahreisezug)
        /// <br/>- vgm013 (DPN - Nahreisezug)
        /// <br/>- vgm020 (DPN - Nahreisezug)
        /// <br/>- vgm022 (DPN - Nahreisezug)
        /// <br/>- vgm023 (DPN - Nahreisezug)
        /// <br/>- vgm024 (DPN - Nahreisezug)
        /// <br/>- vgm025 (DPN - Nahreisezug)
        /// <br/>- vgm026 (DPN - Nahreisezug)
        /// <br/>- vgm028 (DPN - Nahreisezug)
        /// <br/>- vgm029 (DPN - Nahreisezug)
        /// <br/>- vgm030 (DPN - Nahreisezug)
        /// <br/>- vgm031 (DPN - Nahreisezug)
        /// <br/>- vgm032 (DPN - Nahreisezug)
        /// <br/>- vgm033 (DPN - Nahreisezug)
        /// <br/>- vgm034 (DPN - Nahreisezug)
        /// <br/>- vgm035 (DPN - Nahreisezug)
        /// <br/>- vgm036 (DPN - Nahreisezug)
        /// <br/>- vgm037 (DPN - Nahreisezug)
        /// <br/>- vgm038 (DPN - Nahreisezug)
        /// <br/>- vgm039 (DPN - Nahreisezug)
        /// <br/>- vgm040 (DPN - Nahreisezug)
        /// <br/>- vgm041 (DPN - Nahreisezug)
        /// <br/>- vgm042 (DPN - Nahreisezug)
        /// <br/>- vgm044 (DPN - Nahreisezug)
        /// <br/>- vgm045 (DPN - Nahreisezug)
        /// <br/>- vgm046 (DPN - Nahreisezug)
        /// <br/>- vgm047 (DPN - Nahreisezug)
        /// <br/>- vgm050 (DPN - Nahreisezug)
        /// <br/>- vgm051 (DPN - Nahreisezug)
        /// <br/>- vgm053 (DPN - Nahreisezug)
        /// <br/>- vgm060 (DPN - Nahreisezug)
        /// <br/>- vgm079 (DPN - Nahreisezug)
        /// <br/>- vgm092 (DPN - Nahreisezug)
        /// <br/>- vgm093 (WB - Westfalenbus)
        /// <br/>- vgm094 (DPN - Nahreisezug)
        /// <br/>- vgmb93 (WB - Westfalenbus)
        /// <br/>- vgn_16 (DPN - Nahreisezug)
        /// <br/>- vgn043 (DPN - Nahreisezug)
        /// <br/>- vgn061 (DPN - Nahreisezug)
        /// <br/>- vgn063 (DPN - Nahreisezug)
        /// <br/>- vgn065 (DPN - Nahreisezug)
        /// <br/>- vgn068 (DPN - Nahreisezug)
        /// <br/>- vgn083 (DPN - Nahreisezug)
        /// <br/>- vgsARG (DPN - Nahreisezug)
        /// <br/>- vgsBar (DPN - Nahreisezug)
        /// <br/>- vgsBTV (DPN - Nahreisezug)
        /// <br/>- vgsKIR (DPN - Nahreisezug)
        /// <br/>- vgsKVS (DPN - Nahreisezug)
        /// <br/>- vgsLay (DPN - Nahreisezug)
        /// <br/>- vgsMLB (DPN - Nahreisezug)
        /// <br/>- vgsNVG (DPN - Nahreisezug)
        /// <br/>- vgsSAM (DPN - Nahreisezug)
        /// <br/>- vgsSBB (DPN - Nahreisezug)
        /// <br/>- vgsSBS (S - Saarbahn)
        /// <br/>- vgsVVB (DPN - Nahreisezug)
        /// <br/>- vgsZar (DPN - Nahreisezug)
        /// <br/>- vhb000 (DPN - Nahreisezug)
        /// <br/>- vhb002 (DPN - Nahreisezug)
        /// <br/>- vhb003 (DPN - Nahreisezug)
        /// <br/>- vmo004 (DPN - Nahreisezug)
        /// <br/>- vmo008 (DPN - Nahreisezug)
        /// <br/>- vmo010 (DPN - Nahreisezug)
        /// <br/>- vmo012 (DPN - Nahreisezug)
        /// <br/>- vmo050 (DPN - Nahreisezug)
        /// <br/>- vmo099 (DPN - Nahreisezug)
        /// <br/>- vms001 (DPN - Nahreisezug)
        /// <br/>- vms002 (DPN - Nahreisezug)
        /// <br/>- vms010 (DPN - Nahreisezug)
        /// <br/>- vms011 (DPN - Nahreisezug)
        /// <br/>- vms012 (DPN - Nahreisezug)
        /// <br/>- vms014 (DPN - Nahreisezug)
        /// <br/>- vms020 (DPN - Nahreisezug)
        /// <br/>- vms022 (DPN - Nahreisezug)
        /// <br/>- vms023 (DPN - Nahreisezug)
        /// <br/>- vms024 (DPN - Nahreisezug)
        /// <br/>- vms025 (DPN - Nahreisezug)
        /// <br/>- vms031 (DPN - Nahreisezug)
        /// <br/>- vms032 (DPN - Nahreisezug)
        /// <br/>- vms040 (DPN - Nahreisezug)
        /// <br/>- vms041 (DPN - Nahreisezug)
        /// <br/>- vms042 (DPN - Nahreisezug)
        /// <br/>- vms043 (DPN - Nahreisezug)
        /// <br/>- vms044 (DPN - Nahreisezug)
        /// <br/>- vms046 (DPN - Nahreisezug)
        /// <br/>- vms047 (DPN - Nahreisezug)
        /// <br/>- vms051 (DPN - Nahreisezug)
        /// <br/>- vms056 (DPN - Nahreisezug)
        /// <br/>- vms060 (DPN - Nahreisezug)
        /// <br/>- vms061 (DPN - Nahreisezug)
        /// <br/>- vms062 (DPN - Nahreisezug)
        /// <br/>- vms063 (DPN - Nahreisezug)
        /// <br/>- vms064 (DPN - Nahreisezug)
        /// <br/>- vms070 (DPN - Nahreisezug)
        /// <br/>- vms076 (DPN - Nahreisezug)
        /// <br/>- vms077 (DPN - Nahreisezug)
        /// <br/>- vms080 (DPN - Nahreisezug)
        /// <br/>- vms081 (DPN - Nahreisezug)
        /// <br/>- vms099 (DPN - Nahreisezug)
        /// <br/>- voe_12 (DPN - Nahreisezug)
        /// <br/>- voe_SB (DPN - Nahreisezug)
        /// <br/>- voe002 (DPN - Nahreisezug)
        /// <br/>- voe011 (DPN - Nahreisezug)
        /// <br/>- voe012 (DPN - Nahreisezug)
        /// <br/>- voe013 (DPN - Nahreisezug)
        /// <br/>- voe015 (DPN - Nahreisezug)
        /// <br/>- voe021 (DPN - Nahreisezug)
        /// <br/>- voe022 (DPN - Nahreisezug)
        /// <br/>- voe023 (DPN - Nahreisezug)
        /// <br/>- voe024 (DPN - Nahreisezug)
        /// <br/>- voe027 (DPN - Nahreisezug)
        /// <br/>- voe028 (DPN - Nahreisezug)
        /// <br/>- voe029 (DPN - Nahreisezug)
        /// <br/>- voe081 (DPN - Nahreisezug)
        /// <br/>- voe091 (DPN - Nahreisezug)
        /// <br/>- voeALT (DPN - Nahreisezug)
        /// <br/>- voeBU3 (DPN - Nahreisezug)
        /// <br/>- voeBU4 (DPN - Nahreisezug)
        /// <br/>- voeFAE (DPN - Nahreisezug)
        /// <br/>- voeSTR (DPN - Nahreisezug)
        /// <br/>- voeSWB (DPN - Nahreisezug)
        /// <br/>- vogBus (DPN - Nahreisezug)
        /// <br/>- von026 (DPN - Nahreisezug)
        /// <br/>- von027 (DPN - Nahreisezug)
        /// <br/>- von030 (DPN - Nahreisezug)
        /// <br/>- von031 (DPN - Nahreisezug)
        /// <br/>- von032 (DPN - Nahreisezug)
        /// <br/>- von042 (DPN - Nahreisezug)
        /// <br/>- von044 (DPN - Nahreisezug)
        /// <br/>- von052 (DPN - Nahreisezug)
        /// <br/>- von062 (DPN - Nahreisezug)
        /// <br/>- von064 (DPN - Nahreisezug)
        /// <br/>- von065 (DPN - Nahreisezug)
        /// <br/>- von069 (DPN - Nahreisezug)
        /// <br/>- vpeAST (DPN - Nahreisezug)
        /// <br/>- vpeBus (DPN - Nahreisezug)
        /// <br/>- vph063 (DPN - Nahreisezug)
        /// <br/>- vph071 (DPN - Nahreisezug)
        /// <br/>- vph072 (DPN - Nahreisezug)
        /// <br/>- vph073 (DPN - Nahreisezug)
        /// <br/>- vph074 (DPN - Nahreisezug)
        /// <br/>- vph075 (DPN - Nahreisezug)
        /// <br/>- vph076 (DPN - Nahreisezug)
        /// <br/>- vph077 (DPN - Nahreisezug)
        /// <br/>- vph078 (DPN - Nahreisezug)
        /// <br/>- vph079 (DPN - Nahreisezug)
        /// <br/>- vph080 (DPN - Nahreisezug)
        /// <br/>- vph081 (DPN - Nahreisezug)
        /// <br/>- vph082 (DPN - Nahreisezug)
        /// <br/>- vpo099 (DPN - Nahreisezug)
        /// <br/>- vrm002 (DPN - Nahreisezug)
        /// <br/>- vrm005 (DPN - Nahreisezug)
        /// <br/>- vrm006 (DPN - Nahreisezug)
        /// <br/>- vrm007 (DPN - Nahreisezug)
        /// <br/>- vrm008 (DPN - Nahreisezug)
        /// <br/>- vrm009 (DPN - Nahreisezug)
        /// <br/>- vrm011 (DPN - Nahreisezug)
        /// <br/>- vrm014 (DPN - Nahreisezug)
        /// <br/>- vrm015 (DPN - Nahreisezug)
        /// <br/>- vrm016 (DPN - Nahreisezug)
        /// <br/>- vrm017 (DPN - Nahreisezug)
        /// <br/>- vrm019 (DPN - Nahreisezug)
        /// <br/>- vrm020 (DPN - Nahreisezug)
        /// <br/>- vrm025 (DPN - Nahreisezug)
        /// <br/>- vrm031 (DPN - Nahreisezug)
        /// <br/>- vrm032 (DPN - Nahreisezug)
        /// <br/>- vrm036 (DPN - Nahreisezug)
        /// <br/>- vrm061 (DPN - Nahreisezug)
        /// <br/>- vrm066 (DPN - Nahreisezug)
        /// <br/>- vrm067 (DPN - Nahreisezug)
        /// <br/>- vrm068 (DPN - Nahreisezug)
        /// <br/>- vrm069 (DPN - Nahreisezug)
        /// <br/>- vrm070 (DPN - Nahreisezug)
        /// <br/>- vrm072 (DPN - Nahreisezug)
        /// <br/>- vrm073 (DPN - Nahreisezug)
        /// <br/>- vrm074 (DPN - Nahreisezug)
        /// <br/>- vrm077 (DPN - Nahreisezug)
        /// <br/>- vrm078 (DPN - Nahreisezug)
        /// <br/>- vrm083 (DPN - Nahreisezug)
        /// <br/>- vrm084 (DPN - Nahreisezug)
        /// <br/>- vrm085 (DPN - Nahreisezug)
        /// <br/>- vrn008 (STR - Rhein-Neckar-Verkehr GmbH)
        /// <br/>- vrn011 (STR - Rhein-Neckar-Verkehr GmbH)
        /// <br/>- vrn016 (DPN - Nahreisezug)
        /// <br/>- vrn017 (DPN - Nahreisezug)
        /// <br/>- vrn018 (DPN - Nahreisezug)
        /// <br/>- vrn019 (DPN - Nahreisezug)
        /// <br/>- vrn020 (DPN - Nahreisezug)
        /// <br/>- vrn022 (MNV - MNV Mittelhaardt Nahverkehrsgesellschaft)
        /// <br/>- vrn023 (DPN - Nahreisezug)
        /// <br/>- vrn025 (DPN - Nahreisezug)
        /// <br/>- vrn026 (DPN - Nahreisezug)
        /// <br/>- vrn027 (DPN - Nahreisezug)
        /// <br/>- vrn028 (H&amp;P - Hetzler &amp; Pfadt)
        /// <br/>- vrn029 (DPN - Nahreisezug)
        /// <br/>- vrn030 (DPN - Nahreisezug)
        /// <br/>- vrn032 (PAL - PalatinaBus)
        /// <br/>- vrn033 (DPN - Nahreisezug)
        /// <br/>- vrn040 (DPN - Nahreisezug)
        /// <br/>- vrn041 (DPN - Nahreisezug)
        /// <br/>- vrn043 (DPN - Nahreisezug)
        /// <br/>- vrn047 (DPN - Nahreisezug)
        /// <br/>- vrn049 (DPN - Nahreisezug)
        /// <br/>- vrn050 (QNV - QNV Queichtal Nahverkehr)
        /// <br/>- vrn051 (DPN - Nahreisezug)
        /// <br/>- vrn053 (DPN - Nahreisezug)
        /// <br/>- vrn057 (DPN - Nahreisezug)
        /// <br/>- vrn058 (DPN - Nahreisezug)
        /// <br/>- vrn059 (DPN - Nahreisezug)
        /// <br/>- vrn062 (DPN - Nahreisezug)
        /// <br/>- vrn068 (DPN - Nahreisezug)
        /// <br/>- vrn073 (DPN - Nahreisezug)
        /// <br/>- vrn074 (DPN - Nahreisezug)
        /// <br/>- vrn076 (DPN - Nahreisezug)
        /// <br/>- vrn078 (DPN - Nahreisezug)
        /// <br/>- vrn079 (DPN - Nahreisezug)
        /// <br/>- vrn080 (DPN - Nahreisezug)
        /// <br/>- vrn081 (DPN - Nahreisezug)
        /// <br/>- vrn082 (DPN - Nahreisezug)
        /// <br/>- vrn083 (DPN - Nahreisezug)
        /// <br/>- vrn095 (DPN - Nahreisezug)
        /// <br/>- vrnOEG (RNV - Rhein-Neckar-Verkehr GmbH (Oberrheinische Eisenbahn))
        /// <br/>- vrnRHB (RNV - Rhein-Neckar-Verkehr GmbH (Rhein-Haardtbahn))
        /// <br/>- vrr001 (DPN - Nahreisezug)
        /// <br/>- vrr002 (DPN - Nahreisezug)
        /// <br/>- vrr010 (DPN - Nahreisezug)
        /// <br/>- vrr011 (DPN - Nahreisezug)
        /// <br/>- vrr012 (DPN - Nahreisezug)
        /// <br/>- vrr013 (DPN - Nahreisezug)
        /// <br/>- vrr015 (DPN - Nahreisezug)
        /// <br/>- vrr016 (DPN - Nahreisezug)
        /// <br/>- vrr018 (DPN - Nahreisezug)
        /// <br/>- vrr020 (DPN - Nahreisezug)
        /// <br/>- vrr021 (DPN - Nahreisezug)
        /// <br/>- vrr023 (DPN - Nahreisezug)
        /// <br/>- vrr025 (DPN - Nahreisezug)
        /// <br/>- vrr029 (DPN - Nahreisezug)
        /// <br/>- vrr030 (DPN - Nahreisezug)
        /// <br/>- vrr031 (DPN - Nahreisezug)
        /// <br/>- vrr032 (DPN - Nahreisezug)
        /// <br/>- vrr033 (DPN - Nahreisezug)
        /// <br/>- vrr034 (DPN - Nahreisezug)
        /// <br/>- vrr035 (DPN - Nahreisezug)
        /// <br/>- vrr036 (DPN - Nahreisezug)
        /// <br/>- vrr037 (DPN - Nahreisezug)
        /// <br/>- vrr038 (DPN - Nahreisezug)
        /// <br/>- vrr039 (DPN - Nahreisezug)
        /// <br/>- vrr040 (DPN - Nahreisezug)
        /// <br/>- vrr045 (DPN - Nahreisezug)
        /// <br/>- vrr050 (DPN - Nahreisezug)
        /// <br/>- vrr060 (DPN - Nahreisezug)
        /// <br/>- vrr064 (DPN - Nahreisezug)
        /// <br/>- vrr065 (DPN - Nahreisezug)
        /// <br/>- vrr066 (DPN - Nahreisezug)
        /// <br/>- vrr070 (DPN - Nahreisezug)
        /// <br/>- vrr071 (DPN - Nahreisezug)
        /// <br/>- vrr072 (DPN - Nahreisezug)
        /// <br/>- vrr073 (DPN - Nahreisezug)
        /// <br/>- vrr075 (DPN - Nahreisezug)
        /// <br/>- vrr076 (DPN - Nahreisezug)
        /// <br/>- vrr077 (DPN - Nahreisezug)
        /// <br/>- vrr080 (DPN - Nahreisezug)
        /// <br/>- vrr088 (BVR - Busverkehr Rheinland)
        /// <br/>- vrs001 (DPN - Nahreisezug)
        /// <br/>- vrs003 (DPN - Nahreisezug)
        /// <br/>- vrs006 (DPN - Nahreisezug)
        /// <br/>- vrs008 (DPN - Nahreisezug)
        /// <br/>- vrs011 (DPN - Nahreisezug)
        /// <br/>- vrs012 (DPN - Nahreisezug)
        /// <br/>- vrs013 (DPN - Nahreisezug)
        /// <br/>- vrs014 (DPN - Nahreisezug)
        /// <br/>- vrs016 (DPN - Nahreisezug)
        /// <br/>- vrs017 (DPN - Nahreisezug)
        /// <br/>- vrs021 (DPN - Nahreisezug)
        /// <br/>- vrs022 (DPN - Nahreisezug)
        /// <br/>- vrs023 (DPN - Nahreisezug)
        /// <br/>- vrs024 (DPN - Nahreisezug)
        /// <br/>- vrs025 (DPN - Nahreisezug)
        /// <br/>- vrs028 (DPN - Nahreisezug)
        /// <br/>- vrs029 (DPN - Nahreisezug)
        /// <br/>- vrs060 (DPN - Nahreisezug)
        /// <br/>- vrs063 (DPN - Nahreisezug)
        /// <br/>- vsh001 (DPN - Nahreisezug)
        /// <br/>- vsh010 (DPN - Nahreisezug)
        /// <br/>- vsh020 (DPN - Nahreisezug)
        /// <br/>- vuvab (VU - Verkehrsgesellschaft mbH Untermain)
        /// <br/>- vvs012 (WEG - Württembergische Eisenbahn-Gesellschaft mbH)
        /// <br/>- vvs020 (DPN - Nahreisezug)
        /// <br/>- vvs021 (DPN - Nahreisezug)
        /// <br/>- vvs030 (DPN - Nahreisezug)
        /// <br/>- vvs031 (DPN - Nahreisezug)
        /// <br/>- vvs033 (DPN - Nahreisezug)
        /// <br/>- vvs034 (DPN - Nahreisezug)
        /// <br/>- vvs035 (DPN - Nahreisezug)
        /// <br/>- vvs041 (DPN - Nahreisezug)
        /// <br/>- vvs050 (DPN - Nahreisezug)
        /// <br/>- vvs051 (DPN - Nahreisezug)
        /// <br/>- vvs052 (DPN - Nahreisezug)
        /// <br/>- vvs055 (DPN - Nahreisezug)
        /// <br/>- vvs077 (DPN - Nahreisezug)
        /// <br/>- vvs078 (DPN - Nahreisezug)
        /// <br/>- vwmBuS (DPN - Nahreisezug)
        /// <br/>- vwmNAH (DPN - Nahreisezug)
        /// <br/>- vwmStr (DPN - Nahreisezug)
        /// <br/>- vwmVLP (DPN - Nahreisezug)
        /// <br/>- vws003 (DPN - Nahreisezug)
        /// <br/>- vws005 (DPN - Nahreisezug)
        /// <br/>- vws007 (DPN - Nahreisezug)
        /// <br/>- W0 (WDR - Wyker Dampfschiffs-Reederei Föhr-Amrum GmbH)
        /// <br/>- W2 (Dab - Daadetalbahn)
        /// <br/>- W3 (WFB - WestfalenBahn)
        /// <br/>- W6 (WTB - Wutachtalbahn)
        /// <br/>- W9 (WBA - waldbahn - Die Länderbahn GmbH DLB)
        /// <br/>- wabBus (DPN - Nahreisezug)
        /// <br/>- web_AN (DPN - Nahreisezug)
        /// <br/>- web_HB (DPN - Nahreisezug)
        /// <br/>- web_OF (DPN - Nahreisezug)
        /// <br/>- web_OS (DPN - Nahreisezug)
        /// <br/>- web002 (DPN - Nahreisezug)
        /// <br/>- web005 (DPN - Nahreisezug)
        /// <br/>- web006 (DPN - Nahreisezug)
        /// <br/>- web018 (DPN - Nahreisezug)
        /// <br/>- web019 (DPN - Nahreisezug)
        /// <br/>- webALL (DPN - Nahreisezug)
        /// <br/>- webAND (DPN - Nahreisezug)
        /// <br/>- webARE (DPN - Nahreisezug)
        /// <br/>- webAST (DPN - Nahreisezug)
        /// <br/>- webBBU (DPN - Nahreisezug)
        /// <br/>- webBEC (DPN - Nahreisezug)
        /// <br/>- webBPU (DPN - Nahreisezug)
        /// <br/>- webBRU (DPN - Nahreisezug)
        /// <br/>- webBTR (STR - Bremer Straßenbahn AG)
        /// <br/>- webBVB (DPN - Nahreisezug)
        /// <br/>- webBVS (DPN - Nahreisezug)
        /// <br/>- webDEL (DPN - Nahreisezug)
        /// <br/>- webDHE (DPN - Nahreisezug)
        /// <br/>- webEDZ (DPN - Nahreisezug)
        /// <br/>- webEMS (DPN - Nahreisezug)
        /// <br/>- webEVB (DPN - Nahreisezug)
        /// <br/>- webFAS (DPN - Nahreisezug)
        /// <br/>- webFIS (DPN - Nahreisezug)
        /// <br/>- webFLX (DPN - Nahreisezug)
        /// <br/>- webGEB (DPN - Nahreisezug)
        /// <br/>- webGER (DPN - Nahreisezug)
        /// <br/>- webGIE (DPN - Nahreisezug)
        /// <br/>- webGOE (DPN - Nahreisezug)
        /// <br/>- webGOS (DPN - Nahreisezug)
        /// <br/>- webHAR (DPN - Nahreisezug)
        /// <br/>- webHDK (DPN - Nahreisezug)
        /// <br/>- webHKR (DPN - Nahreisezug)
        /// <br/>- webHM (DPN - Nahreisezug)
        /// <br/>- webHUT (DPN - Nahreisezug)
        /// <br/>- webHVG (DPN - Nahreisezug)
        /// <br/>- webJAC (DPN - Nahreisezug)
        /// <br/>- webjan (DPN - Nahreisezug)
        /// <br/>- webKAR (DPN - Nahreisezug)
        /// <br/>- webKBA (DPN - Nahreisezug)
        /// <br/>- webKRR (DPN - Nahreisezug)
        /// <br/>- webKVG (DPN - Nahreisezug)
        /// <br/>- webLSE (DPN - Nahreisezug)
        /// <br/>- webMEY (DPN - Nahreisezug)
        /// <br/>- webNIE (DPN - Nahreisezug)
        /// <br/>- webNOH (DPN - Nahreisezug)
        /// <br/>- webNOR (DPN - Nahreisezug)
        /// <br/>- webONS (DPN - Nahreisezug)
        /// <br/>- webOS1 (DPN - Nahreisezug)
        /// <br/>- webOSV (DPN - Nahreisezug)
        /// <br/>- webRA1 (DPN - Nahreisezug)
        /// <br/>- webRBG (DPN - Nahreisezug)
        /// <br/>- webRVH (DPN - Nahreisezug)
        /// <br/>- webSAL (DPN - Nahreisezug)
        /// <br/>- webSTO (DPN - Nahreisezug)
        /// <br/>- webSVG (DPN - Nahreisezug)
        /// <br/>- webSWE (DPN - Nahreisezug)
        /// <br/>- webSWH (DPN - Nahreisezug)
        /// <br/>- webSZG (DPN - Nahreisezug)
        /// <br/>- webTMW (DPN - Nahreisezug)
        /// <br/>- webUEB (DPN - Nahreisezug)
        /// <br/>- webUET (STB - üstra Hannoversche Verkehrsbetriebe AG)
        /// <br/>- webUFF (DPN - Nahreisezug)
        /// <br/>- webVBB (DPN - Nahreisezug)
        /// <br/>- webVBN (DPN - Nahreisezug)
        /// <br/>- webVBW (DPN - Nahreisezug)
        /// <br/>- webVGE (DPN - Nahreisezug)
        /// <br/>- webVGH (DPN - Nahreisezug)
        /// <br/>- webVGP (DPN - Nahreisezug)
        /// <br/>- webVGV (DPN - Nahreisezug)
        /// <br/>- webVGW (DPN - Nahreisezug)
        /// <br/>- webVL1 (DPN - Nahreisezug)
        /// <br/>- webVLG (DPN - Nahreisezug)
        /// <br/>- webVO8 (DPN - Nahreisezug)
        /// <br/>- webVOH (DPN - Nahreisezug)
        /// <br/>- webVOL (DPN - Nahreisezug)
        /// <br/>- webVOS (DPN - Nahreisezug)
        /// <br/>- webVSN (DPN - Nahreisezug)
        /// <br/>- webVWG (DPN - Nahreisezug)
        /// <br/>- webWHV (DPN - Nahreisezug)
        /// <br/>- webWIN (DPN - Nahreisezug)
        /// <br/>- webWIS (DPN - Nahreisezug)
        /// <br/>- webWOB (DPN - Nahreisezug)
        /// <br/>- webWSF (DPN - Nahreisezug)
        /// <br/>- webXOS (WEB - Weser-Ems-Bus)
        /// <br/>- webYGO (RBB - Regionalbus Braunschweig GmbH)
        /// <br/>- webYUE (RBB - Regionalbus Braunschweig GmbH)
        /// <br/>- webYUZ (DPN - Nahreisezug)
        /// <br/>- wenBUS (DPN - Nahreisezug)
        /// <br/>- WL (RB - Kreisbahn Mansfelder Land)
        /// <br/>- wstwst (DPN - Nahreisezug)
        /// <br/>- wvvBUS (DPN - Nahreisezug)
        /// <br/>- wvvSTR (DPN - Nahreisezug)
        /// <br/>- wzlBus (DPN - Nahreisezug)
        /// <br/>- X1 (erx - erixx)
        /// <br/>- X2 (erx - erixx)
        /// <br/>- Y0 (SCH - Adler-Schiffe)
        /// <br/>- Y8 (BRB - Bayerische Regiobahn)
        /// <br/>- Z8 (BZB - Bayerische Zugspitzbahn)
        /// <br/>- Z9 (P - Rhön-Zügle)
        /// <br/>- zsbteg (DPN - Nahreisezug)
        /// <br/>- zvv008 (DPN - Nahreisezug)
        /// <br/>- zvv018 (DPN - Nahreisezug)
        /// <br/>- zvv019 (DPN - Nahreisezug)
        /// <br/>- zvv041 (DPN - Nahreisezug)
        /// <br/>note: list is not exhausting and more undocumented values may be returned
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("administrationID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string AdministrationID { get; set; }

        /// <summary>
        /// Unique code of the operator [Betreiber].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("operatorCode")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string OperatorCode { get; set; }

        /// <summary>
        /// Name of the operator [Betreiber].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("operatorName")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string OperatorName { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Codeshare [Code-Teilungen mit Flügen verschiedener Fluggesellschaften] information for a particular journey event.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class CodeShare
    {

        /// <summary>
        /// Code of cooperating airline [IATA-Code der Fluggesellschaft].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("airlineCode")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string AirlineCode { get; set; }

        /// <summary>
        /// Flightnumber of cooperating airline journey [Flugnummer des Fluges der kooperierenden Fluggesellschaft].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("flightnumber")]
        public int Flightnumber { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Differing continuation information in case the continuation doesn't take place at the start / beginning of the journey [Durchbindung an Unterwegshalten].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ContinuationInfo
    {

        /// <summary>
        /// ID of arrival or departure the continuation takes place.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("arrivalOrDepartureID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(10)]
        public string ArrivalOrDepartureID { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("stopPlace")]
        [System.ComponentModel.DataAnnotations.Required]
        public StopPlaceEmbedded StopPlace { get; set; } = new StopPlaceEmbedded();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Optional direction information [Richtungstext] for a particular transport that may differ from the destination [Zielhalt] on some parts of the journey. Take care that a text and / or a particular stop-place [Haltestelle] may be provided.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class DirectionInfo
    {

        /// <summary>
        /// Optional list of stop-places [Haltestellen] the direction text refers to. May be more than one stop-place for cases like for instance 'Richtung Messe &amp; Hauptbahnhof'.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("stopPlaces")]
        public System.Collections.Generic.ICollection<StopPlaceEmbedded> StopPlaces { get; set; }

        /// <summary>
        /// Direction text [Richtungstext]. Must not necessarly be the name of a real stop-place [Haltestelle] (for instance 'Richtung Automobilmesse'). For instance before the stop 'fare' the transport has direction 'fare', after 'fare' has been reached maybe 'central station' or 'airport'.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("text")]
        public string Text { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Disruption communication [Störungskommunikation] attachment.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class DisruptionCommunicationAttachment
    {

        /// <summary>
        /// Label for the attachment.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("label")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Label { get; set; }

        /// <summary>
        /// URL of attachment.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("url")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Url { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Disruption communication information [Stoerungskommunikation] descriptions.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class DisruptionCommunicationDescription
    {

        /// <summary>
        /// Attachments for additional information that may be communicated to the traveller (download use-cases).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("attachments")]
        public System.Collections.Generic.ICollection<DisruptionCommunicationAttachment> Attachments { get; set; }

        /// <summary>
        /// Images for additional information that may be communicated to the traveller.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("images")]
        public System.Collections.Generic.ICollection<DisruptionCommunicationImage> Images { get; set; }

        /// <summary>
        /// Links for additional information that may be communicated to the traveller.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("links")]
        public System.Collections.Generic.ICollection<DisruptionCommunicationLink> Links { get; set; }

        /// <summary>
        /// Long text of disruption communication.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("text")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Text { get; set; }

        /// <summary>
        /// Optional short text of disruption communication.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("textShort")]
        public string TextShort { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Disruption communication [Störungskommunikation] image.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class DisruptionCommunicationImage
    {

        /// <summary>
        /// Label for the image.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("label")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Label { get; set; }

        /// <summary>
        /// URL of image.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("url")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Url { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Disruption communication [Störungskommunikation] link.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class DisruptionCommunicationLink
    {

        /// <summary>
        /// Label for the link.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("label")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Label { get; set; }

        /// <summary>
        /// URL of link.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("url")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Url { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Detailed error information on field level.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ErrorDetail
    {

        /// <summary>
        /// Detailed information for error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("detail")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Detail { get; set; }

        /// <summary>
        /// Unique code that identifies error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Name of field / element that raised the error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("field")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Field { get; set; }

        /// <summary>
        /// Common description of error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("title")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Title { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// API error object according to RFC7807.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ErrorResponse
    {

        /// <summary>
        /// Detailed information for error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("detail")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Detail { get; set; }

        /// <summary>
        /// Unique code that identifies error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// List of detailed errors in case multiple errors have lead to the surrounding error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("errors")]
        public System.Collections.Generic.ICollection<ErrorDetail> Errors { get; set; }

        /// <summary>
        /// Unique identifier for instance that raised the error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("instanceId")]
        public string InstanceId { get; set; }

        /// <summary>
        /// Http status for error origin.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Common description of error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("title")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Title { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Type of event.
    /// <br/>- ARRIVAL (Ankunft)
    /// <br/>- DEPARTURE (Abfahrt)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    [System.Text.Json.Serialization.JsonConverter(typeof(TolerantEventTypeConverter))]
    public enum EventType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"ARRIVAL")]
        ARRIVAL = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"DEPARTURE")]
        DEPARTURE = 1,

    }

    internal class TolerantEventTypeConverter : System.Text.Json.Serialization.JsonConverter<EventType>
    {
        public override EventType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string eventType = reader.GetString();
            if (Enum.TryParse<EventType>(eventType, true, out var result)) return result;
            return EventType.ARRIVAL;
        }

        public override void Write(Utf8JsonWriter writer, EventType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    /// <summary>
    /// Batch response error for a particular journey.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyBatchError
    {

        /// <summary>
        /// Unique code that identifies error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Text that describes the error.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("errorText")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string ErrorText { get; set; }

        /// <summary>
        /// ID of erroneous journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Batch request to match journeys [Fahrten] / departures [Abfahrten] / arrivals [Ankünfte] by one ore more journey events [Fahrtereignisse].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyBatchMatchByEventsRequest
    {

        /// <summary>
        /// List of match requests. A maximum of 50 match requests is allowed.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("requests")]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MinLength(1)]
        [System.ComponentModel.DataAnnotations.MaxLength(50)]
        public System.Collections.Generic.ICollection<JourneyMatchByEventsRequest> Requests { get; set; } = new System.Collections.ObjectModel.Collection<JourneyMatchByEventsRequest>();

        /// <summary>
        /// Indicates whether to return all journey events [Fahrtereignisse] as well. Useful when the caller wants to match journeys and their journey events as well. Defaults to false.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("returnJourneyEvents")]
        public bool ReturnJourneyEvents { get; set; }

        /// <summary>
        /// Tolerance in minutes (+x / -x minutes) for matching against time information. A maximum of 10 minutes is allowed.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("timeTolerance")]
        [System.ComponentModel.DataAnnotations.Range(int.MinValue, 10)]
        public int TimeTolerance { get; set; } = 0;

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Batch request to match journeys [Fahrten] / departures [Abfahrten] / arrivals [Ankünfte] by scheduled journey start / end [geplanter Fahrtbeginn / Fahrtende] information.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyBatchMatchByStartEndRequest
    {

        /// <summary>
        /// List of match requests. A maximum of 50 match requests is allowed.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("requests")]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MinLength(1)]
        [System.ComponentModel.DataAnnotations.MaxLength(50)]
        public System.Collections.Generic.ICollection<JourneyMatchByStartEndRequest> Requests { get; set; } = new System.Collections.ObjectModel.Collection<JourneyMatchByStartEndRequest>();

        /// <summary>
        /// Indicates whether to return all journey events [Fahrtereignisse] as well. Useful when the caller wants to match journeys and their journey events as well. Defaults to false.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("returnJourneyEvents")]
        public bool ReturnJourneyEvents { get; set; }

        /// <summary>
        /// Tolerance in minutes (+x / -x minutes) for matching against time information. A maximum of 10 minutes is allowed.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("timeTolerance")]
        [System.ComponentModel.DataAnnotations.Range(int.MinValue, 10)]
        public int TimeTolerance { get; set; } = 0;

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Batch response to match journeys [Fahrten] / departures [Abfahrten] / arrivals [Ankünfte].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyBatchMatchResponse
    {

        /// <summary>
        /// List of match response.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("responses")]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MinLength(1)]
        public System.Collections.Generic.ICollection<JourneyMatchResponse> Responses { get; set; } = new System.Collections.ObjectModel.Collection<JourneyMatchResponse>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Batch request to return mutiple journeys [Fahrten] at once.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyBatchRequest
    {

        /// <summary>
        /// Include journey references like relief [Entlastung], replace [Ersatz], continuation [Durchbindung] and travels-with [Vereinigung / Fluegelung].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("includeReferences")]
        public bool IncludeReferences { get; set; }

        /// <summary>
        /// IDs of journeys [Fahrt-IDs]. A maximum of 500 journey-ids is allowed.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyIDs")]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MinLength(1)]
        [System.ComponentModel.DataAnnotations.MaxLength(500)]
        public System.Collections.Generic.ICollection<string> JourneyIDs { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>
        /// Languages to return, defaults to 'DE' and 'EN' (if available)
        /// <br/>- DE (German)
        /// <br/>- EN (English)
        /// <br/>- FR (French)
        /// <br/>- IT (Italian)
        /// <br/>- CS (Czech)
        /// <br/>- DA (Danish)
        /// <br/>- ES (Spanish)
        /// <br/>- NL (Dutch)
        /// <br/>- PL (Polish)
        /// <br/>
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("languages")]
        public System.Collections.Generic.ICollection<string> Languages { get; set; }

        /// <summary>
        /// Separate cancelled events [ausgefallene Fahrtereignisse] in dedicated collection 'eventsCancelled'.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("separateCancelled")]
        public bool SeparateCancelled { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Batch response to return mutiple journeys [Fahrten] at once.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyBatchResponse
    {

        /// <summary>
        /// List of erroneous journeys for this particular batch request.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("erroneousJourneys")]
        public System.Collections.Generic.ICollection<JourneyBatchError> ErroneousJourneys { get; set; }

        /// <summary>
        /// List of journeys.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeys")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<JourneyEventBased> Journeys { get; set; } = new System.Collections.ObjectModel.Collection<JourneyEventBased>();

        /// <summary>
        /// List of meta last changed timestamps the returned journeys have been changed as fully-qualified-date (ISO-8601 with time-zone or offset).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("metaLastChangedTimestamps")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<MetaLastChangedTimestamp> MetaLastChangedTimestamps { get; set; } = new System.Collections.ObjectModel.Collection<MetaLastChangedTimestamp>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Journey relation results.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyByRelationResults
    {

        /// <summary>
        /// List of found journeys.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeys")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<JourneyFindResult> Journeys { get; set; } = new System.Collections.ObjectModel.Collection<JourneyFindResult>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Journey event [Fahrtereignis].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyEvent
    {

        /// <summary>
        /// Indicates whether event is additional, meaning not be part of the regular schedule [Zusatzhalt]. Note: In case an event is cancelled and additional, it's considered as an cancelled additional stop [zurückgenommener Zusatzhalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("additional")]
        public bool Additional { get; set; }

        /// <summary>
        /// ID of arrival or departure, depends on event type.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("arrivalOrDepartureID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(12)]
        public string ArrivalOrDepartureID { get; set; }

        /// <summary>
        /// Indicates whether event is cancelled [Haltausfall]. Note: In case an event is cancelled and additional, it's considered as an cancelled additional stop [zurückgenommener Zusatzhalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("cancelled")]
        public bool Cancelled { get; set; }

        /// <summary>
        /// List of codeshares [Code-Teilungen mit Flügen verschiedener Fluggesellschaften] for this particular journey at this arrival / departure.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("codeshares")]
        public System.Collections.Generic.ICollection<CodeShare> Codeshares { get; set; }

        /// <summary>
        /// List of message-ids [Nachrichten IDs] for this particular event. IDs are pointing to the global 'messages' structure, containing disruption-communications [Störungskommunikationen], attributes [Fahrtmerkmale], notes [Hinweise], ris cause codes [RIS Kundengründe] and ris quality deviations [RIS Qualitätsabweichungen].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("messages")]
        public System.Collections.Generic.ICollection<int> Messages { get; set; }

        /// <summary>
        /// Indicates whether passengers are not allowed to enter / leave [kein Fahrgastwechsel].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("noPassengerChange")]
        public bool NoPassengerChange { get; set; }

        /// <summary>
        /// Indicates whether arrival / departure is an on demand stop [Bedarfshalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("onDemand")]
        public bool OnDemand { get; set; }

        /// <summary>
        /// Actual platform [Gleis, Bahnsteig, Plattform] the transport arrives / departs at, may be empty.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("platform")]
        public string Platform { get; set; }

        /// <summary>
        /// Ifopt [DHID] of actual platform [Gleis, Bahnsteig, Plattform] the transport arrives / departs at, may be empty.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("platformIfopt")]
        public string PlatformIfopt { get; set; }

        /// <summary>
        /// Scheduled platform [Gleis, Bahnsteig, Plattform] the transport arrives / departs at.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("platformSchedule")]
        public string PlatformSchedule { get; set; }

        /// <summary>
        /// Ifopt [DHID] of scheduled platform [Gleis, Bahnsteig, Plattform] the transport arrives / departs at.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("platformScheduleIfopt")]
        public string PlatformScheduleIfopt { get; set; }

        /// <summary>
        /// List of transports this journey at this particular event is reliefed by [Entlastungszug].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("reliefBy")]
        public System.Collections.Generic.ICollection<TransportDestinationRef> ReliefBy { get; set; }

        /// <summary>
        /// List of transports this journey at this particular event reliefs for [Entlastungszug].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("reliefFor")]
        public System.Collections.Generic.ICollection<TransportDestinationRef> ReliefFor { get; set; }

        /// <summary>
        /// List of transports this journey at this particular event is replaced by [Ersatzzug].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("replacedBy")]
        public System.Collections.Generic.ICollection<TransportDestinationRef> ReplacedBy { get; set; }

        /// <summary>
        /// List of transports this journey at this particular event replaces [Ersatzzug].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("replacementFor")]
        public System.Collections.Generic.ICollection<TransportDestinationRef> ReplacementFor { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("stopPlace")]
        [System.ComponentModel.DataAnnotations.Required]
        public StopPlaceInJourney StopPlace { get; set; } = new StopPlaceInJourney();

        /// <summary>
        /// Best known time information of stop as fully-qualified-date (ISO-8601 with time-zone or offset).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("time")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset Time { get; set; }

        /// <summary>
        /// Scheduled time [Soll] of stop as fully-qualified-date (ISO-8601 with time-zone or offset).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("timeSchedule")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset TimeSchedule { get; set; }

        /// <summary>
        /// Specifies on which information 'time' is based.
        /// <br/>- SCHEDULE (Time source is schedule [Plan / Soll])
        /// <br/>- PREVIEW (Time source is preview / forecast [Vorschau / Disposition / Prognose])
        /// <br/>- REAL (Time source is real [Echt = passiert (kann nur in die Vergangenheit gesetzt werden)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("timeType")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string TimeType { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("transport")]
        [System.ComponentModel.DataAnnotations.Required]
        public TransportWithDirection Transport { get; set; } = new TransportWithDirection();

        /// <summary>
        /// List of journeys this journey at this particular event travels with [Vereinigung].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("travelsWith")]
        public System.Collections.Generic.ICollection<TransportDestinationPortionWorkingRef> TravelsWith { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<EventType>))]
        public EventType Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Event based [Fahrtereignisbasiert] information for a particular journey [Fahrtverlauf].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyEventBased
    {

        /// <summary>
        /// Continuation by successor journey, if available [Durchbindung am Ziel].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("continuationBy")]
        public System.Collections.Generic.ICollection<TransportDestinationContinuationRef> ContinuationBy { get; set; }

        /// <summary>
        /// Continuation by predecessor journey, if available [Durchbindung am Start].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("continuationFor")]
        public System.Collections.Generic.ICollection<TransportOriginContinuationRef> ContinuationFor { get; set; }

        /// <summary>
        /// List of events [Fahrtereignisse (Abfahrten und Ankünfte)].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("events")]
        public System.Collections.Generic.ICollection<JourneyEvent> Events { get; set; }

        /// <summary>
        /// List of cancelled events [ausgefallene Fahrtereignisse (Abfahrten und Ankünfte)]. Only filled in case separation of cancelled events has been requested.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("eventsCancelled")]
        public System.Collections.Generic.ICollection<JourneyEvent> EventsCancelled { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("info")]
        [System.ComponentModel.DataAnnotations.Required]
        public JourneyInfo Info { get; set; } = new JourneyInfo();

        /// <summary>
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("messages")]
        public Messages Messages { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Journey find result.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyFindResult
    {

        [System.Text.Json.Serialization.JsonPropertyName("info")]
        [System.ComponentModel.DataAnnotations.Required]
        public JourneyInfo Info { get; set; } = new JourneyInfo();

        /// <summary>
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("journeyRelation")]
        [System.ComponentModel.DataAnnotations.Required]
        public JourneyRelation JourneyRelation { get; set; } = new JourneyRelation();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Journey find results.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyFindResults
    {

        /// <summary>
        /// List of found journeys.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeys")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<JourneyFindResult> Journeys { get; set; } = new System.Collections.ObjectModel.Collection<JourneyFindResult>();

        /// <summary>
        /// Maximum number of results the caller has requested to return from provided offset.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// Pagination offset the caller has requested in order to navigate through results.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("offset")]
        public int Offset { get; set; }

        /// <summary>
        /// Total number of available results.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("total")]
        public int Total { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Comprehensive journey information.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyInfo
    {

        [System.Text.Json.Serialization.JsonPropertyName("destination")]
        [System.ComponentModel.DataAnnotations.Required]
        public StopPlaceEmbeddedWithCancel Destination { get; set; } = new StopPlaceEmbeddedWithCancel();

        [System.Text.Json.Serialization.JsonPropertyName("differingDestination")]
        public StopPlaceEmbedded DifferingDestination { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("differingOrigin")]
        public StopPlaceEmbedded DifferingOrigin { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("headerAdministration")]
        [System.ComponentModel.DataAnnotations.Required]
        public Administration HeaderAdministration { get; set; } = new Administration();

        /// <summary>
        /// Header number of journey [Kopf-Fahrtnummer], may differ from number of journey.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("headerJourneyNumber")]
        public int HeaderJourneyNumber { get; set; }

        /// <summary>
        /// Indicates whether whole journey has been cancelled.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyCancelled")]
        public bool JourneyCancelled { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("origin")]
        [System.ComponentModel.DataAnnotations.Required]
        public StopPlaceEmbeddedWithCancel Origin { get; set; } = new StopPlaceEmbeddedWithCancel();

        [System.Text.Json.Serialization.JsonPropertyName("transportAtStart")]
        [System.ComponentModel.DataAnnotations.Required]
        public Transport TransportAtStart { get; set; } = new Transport();

        /// <summary>
        /// Defines whether journey [Fahrt] is regular or some kind of special.
        /// <br/>- REGULAR (Regular scheduled journey)
        /// <br/>- REPLACEMENT (Journey that replaces another journey)
        /// <br/>- RELIEF (Journey that reliefs another journey)
        /// <br/>- EXTRA (Journey that is somehow extra)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Single request to match journeys [Fahrten] / departures [Abfahrten] / arrivals [Ankünfte] by one ore more journey events [Fahrtereignisse] of a particular journey [Fahrt]. This enables the caller to match journeys also without knowing their start information
    /// <br/>
    /// <br/>Attention: One particular request may lead to multiple results in case matching wasn't unique, all referenced by the same request id.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyMatchByEventsRequest
    {

        /// <summary>
        /// Matching criterias for journey event [Fahrtereignisse] information. Should be used in case no information on scheduled journey start is available. At least one event has to be provided. Multiple events are combined with AND operator.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("matchEvents")]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MinLength(1)]
        public System.Collections.Generic.ICollection<JourneyMatchJourneyEvent> MatchEvents { get; set; } = new System.Collections.ObjectModel.Collection<JourneyMatchJourneyEvent>();

        /// <summary>
        /// Request id in order to find results for particular request within response list. Must be unique accross all requests. A maximum length of 50 is allowed.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("requestID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string RequestID { get; set; }

        /// <summary>
        /// List of transport-types [Produktklassen] to return match results for, possible values are:
        /// <br/>- HIGH_SPEED_TRAIN (High speed train [Hochgeschwindigkeitszug])
        /// <br/>- INTERCITY_TRAIN (Inter city train [Intercityzug])
        /// <br/>- INTER_REGIONAL_TRAIN (Inter regional train [Interregiozug])
        /// <br/>- REGIONAL_TRAIN (Regional train [Regionalzug])
        /// <br/>- CITY_TRAIN (City train [S-Bahn])
        /// <br/>- SUBWAY (Subway [U-Bahn])
        /// <br/>- TRAM (Tram [Strassenbahn])
        /// <br/>- BUS (Bus [Bus])
        /// <br/>- FERRY (Ferry [Faehre])
        /// <br/>- SHUTTLE (Shuttle [Ruftaxi])
        /// <br/>- FLIGHT (Flight [Flug])
        /// <br/>- CHARTER_TRAIN (Charter train [Charterzug])
        /// <br/>- UNKNOWN (Unknown)
        /// <br/>
        /// <br/>If empty or omitted, CHARTER_TRAIN won't be returned by default.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("transportTypes")]
        public System.Collections.Generic.ICollection<string> TransportTypes { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Single request to match journeys [Fahrten] / departures [Abfahrten] / arrivals [Ankünfte] by scheduled journey start [Geplanter Fahrtbeginn] and optional journey end [Geplantes Fahrtende] information.
    /// <br/>
    /// <br/>Attention: One particular request may lead to multiple results in case matching wasn't unique, all referenced by the same request id.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyMatchByStartEndRequest
    {

        [System.Text.Json.Serialization.JsonPropertyName("matchStartEnd")]
        [System.ComponentModel.DataAnnotations.Required]
        public JourneyMatchJourneyStartEnd MatchStartEnd { get; set; } = new JourneyMatchJourneyStartEnd();

        /// <summary>
        /// Request id in order to find results for particular request within response list. Must be unique accross all requests. A maximum length of 50 is allowed.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("requestID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string RequestID { get; set; }

        /// <summary>
        /// List of transport-types [Produktklassen] to return match results for, possible values are:
        /// <br/>- HIGH_SPEED_TRAIN (High speed train [Hochgeschwindigkeitszug])
        /// <br/>- INTERCITY_TRAIN (Inter city train [Intercityzug])
        /// <br/>- INTER_REGIONAL_TRAIN (Inter regional train [Interregiozug])
        /// <br/>- REGIONAL_TRAIN (Regional train [Regionalzug])
        /// <br/>- CITY_TRAIN (City train [S-Bahn])
        /// <br/>- SUBWAY (Subway [U-Bahn])
        /// <br/>- TRAM (Tram [Strassenbahn])
        /// <br/>- BUS (Bus [Bus])
        /// <br/>- FERRY (Ferry [Faehre])
        /// <br/>- SHUTTLE (Shuttle [Ruftaxi])
        /// <br/>- FLIGHT (Flight [Flug])
        /// <br/>- CHARTER_TRAIN (Charter train [Charterzug])
        /// <br/>- UNKNOWN (Unknown)
        /// <br/>
        /// <br/>If empty or omitted, CHARTER_TRAIN won't be returned by default.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("transportTypes")]
        public System.Collections.Generic.ICollection<string> TransportTypes { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Matching criterias for journey event [Fahrtereignisse] information. At least one event has to be provided. Multiple events are combined with AND operator.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyMatchJourneyEvent
    {

        /// <summary>
        /// Optional administration-id [Verwaltungs-ID] of journey event.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("administrationID")]
        public string AdministrationID { get; set; }

        /// <summary>
        /// Optional category of journey event [externe Fahrtgattung nach Ausgabensteuerung].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("category")]
        public string Category { get; set; }

        /// <summary>
        /// Date (yyyy-MM-dd) of journey-event [Fahrtereignis] at timezone 'Europe/Berlin'.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("date")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(DateFormatConverter))]
        public System.DateTimeOffset Date { get; set; }

        /// <summary>
        /// Journey-number [Fahrtnummer] of journey event. At least 'number' or 'line' + 'administrationID' has to be provided.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyNumber")]
        public int JourneyNumber { get; set; }

        /// <summary>
        /// Line of journey [Linie] of journey event. At least 'number' or 'line' + 'administrationID' has to be provided.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("line")]
        public string Line { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("stopPlace")]
        public JourneyMatchStopPlace StopPlace { get; set; }

        /// <summary>
        /// Optional time of journey-event [Abfahrts- oder Ankunftszeit ohne Datumsanteil] in format 'HH:mm:ss'.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("time")]
        public System.TimeSpan Time { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Response for matched journey events [Fahrtereignisse].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyMatchJourneyEventResult
    {

        /// <summary>
        /// ID of arrival or departure, depends on event type.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("arrivalOrDepartureID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(12)]
        public string ArrivalOrDepartureID { get; set; }

        /// <summary>
        /// Index of journey event [Fahrtereigniss] within list of journey events.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("index")]
        public int Index { get; set; }

        /// <summary>
        /// Scheduled time [Geplante Zeit] of journey event at stop-place as fully-qualified-date (ISO-8601 with time-zone or offset).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("scheduledTime")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset ScheduledTime { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("stopPlace")]
        [System.ComponentModel.DataAnnotations.Required]
        public JourneyMatchStopPlace StopPlace { get; set; } = new JourneyMatchStopPlace();

        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<EventType>))]
        public EventType Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Matching criterias for scheduled journey origin [Geplanter Fahrtbeginn] and optional journey destination [Geplantes Fahrtende] information. Should be used in case information on scheduled journey start is available. At least one of 'matchStartEnd' or 'matchEvents' must be used.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyMatchJourneyStartEnd
    {

        [System.Text.Json.Serialization.JsonPropertyName("endStopPlace")]
        public JourneyMatchStopPlace EndStopPlace { get; set; }

        /// <summary>
        /// Optional scheduled end time [Geplante Zielzeit] at scheduled end stop-place [Geplanter Zielhalt] as fully-qualified-date (ISO-8601 with time-zone or offset).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("endTime")]
        public System.DateTimeOffset EndTime { get; set; }

        /// <summary>
        /// ID of the header-administration [Kopf-Verwaltung] of journey.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("headerAdministrationID")]
        public string HeaderAdministrationID { get; set; }

        /// <summary>
        /// ID of the start-administration [Start-Verwaltung] at scheduled start stop-place [Geplanter Starthalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("startAdministrationID")]
        public string StartAdministrationID { get; set; }

        /// <summary>
        /// Optional category [externe Fahrtgattung] at scheduled start stop-place [Geplanter Starthalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("startCategory")]
        public string StartCategory { get; set; }

        /// <summary>
        /// Number of journey [Fahrtnummer] at scheduled start stop-place [Geplanter Starthalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("startJourneyNumber")]
        public int StartJourneyNumber { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("startStopPlace")]
        [System.ComponentModel.DataAnnotations.Required]
        public JourneyMatchStopPlace StartStopPlace { get; set; } = new JourneyMatchStopPlace();

        /// <summary>
        /// Scheduled start time [Geplante Startzeit] at scheduled start stop-place [Geplanter Starthalt] as fully-qualified-date (ISO-8601 with time-zone or offset).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("startTime")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset StartTime { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Single response to match journeys [Fahrten] / departures [Abfahrten] / arrivals [Ankünfte].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyMatchResponse
    {

        /// <summary>
        /// List of matched journey events in case matching of journey events has been requested. Empty in case no journey could be matched.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("events")]
        public System.Collections.Generic.ICollection<JourneyMatchJourneyEventResult> Events { get; set; }

        /// <summary>
        /// ID of matched journey [Fahrt]. Empty in case no journey could be matched.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        public string JourneyID { get; set; }

        /// <summary>
        /// Request id in order to find particular request within request list. Take care that multiple responses may be linked to one request id in case matching wasn't unique.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("requestID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string RequestID { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Matching stop-place [Haltestelle] information.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyMatchStopPlace
    {

        /// <summary>
        /// Eva number of stop-place [Haltestelle]. Either 'evaNumber' or 'rl100' must be provided.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("evaNumber")]
        public string EvaNumber { get; set; }

        /// <summary>
        /// RL-100 code of stop-place [Haltestelle]. Either 'evaNumber' or 'rl100' must be provided.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("rl100")]
        public string Rl100 { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Description of a journey [Fahrt] by key attributes, that are summarized as journey relation [Fahrtrelation].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class JourneyRelation
    {

        /// <summary>
        /// Eva number of scheduled end stop-place [Geplanter Zielhalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("endEvaNumber")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string EndEvaNumber { get; set; }

        /// <summary>
        /// Scheduled end time [Geplante Zielzeit] at scheduled end stop-place [Geplanter Zielhalt] as fully-qualified-date (ISO-8601 with time-zone or offset).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("endTime")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset EndTime { get; set; }

        /// <summary>
        /// Unique id of the header administration [Kopf-Verwaltung], may differ from startAdministrationID (for instance journeys has been created by DB Fernverkehr 80 but its operated by SNCF 81).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("headerAdministrationID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string HeaderAdministrationID { get; set; }

        /// <summary>
        /// Header number of journey [Kopf-Fahrtnummer] at scheduled start stop-place [Geplanter Starthalt], may differ from startNumber.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("headerJourneyNumber")]
        public int HeaderJourneyNumber { get; set; }

        /// <summary>
        /// Unique id of the administration [Verwaltung] at scheduled start stop-place [Geplanter Starthalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("startAdministrationID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string StartAdministrationID { get; set; }

        /// <summary>
        /// Category [externe Fahrtgattung nach Ausgabensteurerung] at scheduled start stop-place [Geplanter Starthalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("startCategory")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string StartCategory { get; set; }

        /// <summary>
        /// Eva number of scheduled start stop-place [Geplanter Starthalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("startEvaNumber")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string StartEvaNumber { get; set; }

        /// <summary>
        /// Number of journey [Fahrtnummer] at scheduled start stop-place [Geplanter Starthalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("startJourneyNumber")]
        public int StartJourneyNumber { get; set; }

        /// <summary>
        /// Scheduled start time [Geplante Startzeit] at scheduled start stop-place [Geplanter Starthalt] as fully-qualified-date (ISO-8601 with time-zone or offset).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("startTime")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset StartTime { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Journey-attribute [Fahrtmerkmale / Sollmerkmale] message.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class MessageAttribute
    {

        /// <summary>
        /// - FR (Fahrradmitnahme reservierungspflichtig)
        /// <br/>- RP (Reservierungspflicht)
        /// <br/>- RR (1. Kl. Reservierungspflicht)
        /// <br/>- RT (Teilreservierungspflicht)
        /// <br/>- OC (rollstuhltaugliches WC)
        /// <br/>- 3D (Schlafwagen 2. Klasse T3 mit Dusche und WC)
        /// <br/>- S1 (Schlafwagen 1. Klasse Single mit Dusche und WC)
        /// <br/>- S2 (Schlafwagen 1. Klasse Double mit Dusche und WC)
        /// <br/>- NG (Tarifliche Vereinigung (DB-Nahverkehrsfahrkarten gelten mit Ausnahme von Sonderangeboten)
        /// <br/>- NJ (Tarifliche Vereinigung (Alle Nahverkehrsfahrkarten werden anerkannt)
        /// <br/>- N+ (Tarifliche Vereinigung (Nahverkehrskooperation DB FV: fiktiver Zug)
        /// <br/>- CK (Komfort Check-in möglich)
        /// <br/>- HS (Zugang fuer Rollstuhlfahrer)
        /// <br/>- OA (Rollstuhlstellplatz - Voranmeldung unter +43 5 1717)
        /// <br/>- OG (bedingt rollstuhltaugliches WC)
        /// <br/>- RO (Rollstuhlstellplatz)
        /// <br/>- FB (Fahrradmitnahme begrenzt moeglich)
        /// <br/>- FF (Fahrradmitnahme reservierungpflichtig -nur grenzueberschreitend moeglich)
        /// <br/>- FK (Fahrradmitnahme begrenzt moeglich)
        /// <br/>- FO (Fahrradreservierung unter 030 2970 oder in Reisezentren + DB Agenturen)
        /// <br/>- FR (Fahrradmitnahme reservierungspflichtig)
        /// <br/>- G  (Fahrradmitnahme begrenzt moeglich)
        /// <br/>- NF (keine Fahrradbefoerderung moeglich)
        /// <br/>- DC (keine behindertengerechte Toilette)
        /// <br/>- EH (Fahrzeuggebundene Einstiegshilfe: Anmeldung 01806-512512 *)
        /// <br/>- EF (Fahrzeuggebundene Einstiegshilfe)
        /// <br/>- RG (Behindertengerechtes Fahrzeug)
        /// <br/>- SM (Info www.bahn.de/sh-barrierefrei)
        /// <br/>- SI (Barrierefreier Zustieg an geeigneten Stationen moeglich)
        /// <br/>- AB (Bus mit Fahrradanhaenger)
        /// <br/>- FJ (Keine Mitnahme von Fahrradgruppen moeglich)
        /// <br/>- FS (Bei Fahrradmitnahme Sperrzeiten beachten)
        /// <br/>- FT (Radexpress und Ausflugszug)
        /// <br/>- KF (Kostenlose Fahrradbefoerderung)
        /// <br/>- RF (Fahrradbus: Fuer Reisende mit Fahrrad)
        /// <br/>- TF (Bus mit Fahrradtraeger)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("code")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Code { get; set; }

        /// <summary>
        /// Display priority [Anzeigereihenfolge aka 'Priorität*] for message. Order is ascending.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("displayPriority")]
        public int DisplayPriority { get; set; }

        /// <summary>
        /// Detailed display priority [detaillierte Anzeigereihenfolge aka 'Feinsortierung'] for message. Order is ascending.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("displayPriorityDetail")]
        public int DisplayPriorityDetail { get; set; }

        /// <summary>
        /// ID of message that is unique within all message collections.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("messageID")]
        public int MessageID { get; set; }

        /// <summary>
        /// Text for attribute.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("text")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Text { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Disruption-communication [Störungskommunikationen] message.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class MessageDisruptionCommunication
    {

        /// <summary>
        /// Cause for disruption [Stoerungsursache].
        /// <br/>- UNKNOWN_CAUSE (cause is unknown [unbekannte Ursache])
        /// <br/>- OTHER_CAUSE (cause is not unknown but different from all enumerated causes [nicht definierte Ursache])
        /// <br/>- TECHNICAL_PROBLEM_RAILWAY_SECTION (due to a problem referring a railway section [Streckenstörung])
        /// <br/>- TECHNICAL_PROBLEM_VEHICLE (due to a technical problem [technische Störung])
        /// <br/>- TECHNICAL_PROBLEM_OTHER (due to a technical problem [technische Störung])
        /// <br/>- STRIKE (due to strike [Streik / Arbeitskamp])
        /// <br/>- DEMONSTRATION (due to demonstration [Demonstration])
        /// <br/>- ACCIDENT (due to an accident [Unfall])
        /// <br/>- HOLIDAY (due to holidays [Ferien])
        /// <br/>- WEATHER_STORM (due to bad weather [Unwetter])
        /// <br/>- WEATHER_HEAT (due to bad weather [Hitze])
        /// <br/>- WEATHER_WINTER (due to bad weather [Winterwitterung])
        /// <br/>- MAINTENANCE (due to maintenance [Wartungsarbeiten an der Infrastruktur / den Fahrzeugen])
        /// <br/>- CONSTRUCTION (due to construction [Bauarbeiten])
        /// <br/>- POLICE_ACTIVITY (due to police activity [Polizeieinsatz])
        /// <br/>- MEDICAL_EMERGENCY (due to a medical emergency [Notarzteinsatz])
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("cause")]
        public string Cause { get; set; }

        /// <summary>
        /// Display priority [Anzeigereihenfolge] for disruption-communication. Order is ascending.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("displayPriority")]
        public int DisplayPriority { get; set; }

        /// <summary>
        /// ID of disruption communication [StoerungskommunikationsID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("disruptionCommunicationID")]
        public string DisruptionCommunicationID { get; set; }

        /// <summary>
        /// ID of disruption [StoerungsID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("disruptionID")]
        public string DisruptionID { get; set; }

        /// <summary>
        /// Effect of distruption communication [Stoerungsauswirkung].
        /// <br/>- NO_SERVICE (no service possible [Betriebseinstellung])
        /// <br/>- MASSIVE_IMPAIRMENT (possible effects apply to all kinds of services  [Massive Beeinträchtigungen])
        /// <br/>- IMPAIRMENT (consequences apply to different services [Beeinträchtigungen])
        /// <br/>- REDUCED_SERVICE (cancellations of journey  [Fahrtausfälle])
        /// <br/>- MODIFIED_SERVICE ([Fahrtänderungen])
        /// <br/>- DETOUR ([Umleitungen])
        /// <br/>- STOP_MOVED ([Haltausfälle])
        /// <br/>- SIGNIFICANT_DELAYS ([Verspätungen])
        /// <br/>- ADDITIONAL_SERVICE ([Ersatzverkehr])
        /// <br/>- OTHER_EFFECT ([Andere Beeinträchtigungen])
        /// <br/>- UNKNOWN_EFFECT ([Unbekannte Auswirkung]
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("effect")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Effect { get; set; }

        /// <summary>
        /// Incidates whether alternatives [alternative Verbindungen] are available for this particular disruption-communication.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("hasAlternatives")]
        public bool HasAlternatives { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langCs")]
        public DisruptionCommunicationDescription LangCs { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langDa")]
        public DisruptionCommunicationDescription LangDa { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langDe")]
        [System.ComponentModel.DataAnnotations.Required]
        public DisruptionCommunicationDescription LangDe { get; set; } = new DisruptionCommunicationDescription();

        [System.Text.Json.Serialization.JsonPropertyName("langEn")]
        public DisruptionCommunicationDescription LangEn { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langEs")]
        public DisruptionCommunicationDescription LangEs { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langFr")]
        public DisruptionCommunicationDescription LangFr { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langIt")]
        public DisruptionCommunicationDescription LangIt { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langNl")]
        public DisruptionCommunicationDescription LangNl { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langPl")]
        public DisruptionCommunicationDescription LangPl { get; set; }

        /// <summary>
        /// ID of message that is unique within all message collections.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("messageID")]
        public int MessageID { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Note [Hinweistext] message.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class MessageNote
    {

        /// <summary>
        /// Optional category of message, like for instance 'Bauarbeiten' or 'Informationen'.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("category")]
        public string Category { get; set; }

        /// <summary>
        /// Optional code of note.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// ID of disruption communication [StoerungskommunikationsID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("disruptionCommunicationID")]
        public string DisruptionCommunicationID { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langCs")]
        public MessageNoteDescription LangCs { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langDa")]
        public MessageNoteDescription LangDa { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langDe")]
        [System.ComponentModel.DataAnnotations.Required]
        public MessageNoteDescription LangDe { get; set; } = new MessageNoteDescription();

        [System.Text.Json.Serialization.JsonPropertyName("langEn")]
        public MessageNoteDescription LangEn { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langEs")]
        public MessageNoteDescription LangEs { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langFr")]
        public MessageNoteDescription LangFr { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langIt")]
        public MessageNoteDescription LangIt { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langNl")]
        public MessageNoteDescription LangNl { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("langPl")]
        public MessageNoteDescription LangPl { get; set; }

        /// <summary>
        /// ID of message that is unique within all message collections.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("messageID")]
        public int MessageID { get; set; }

        /// <summary>
        /// Freetext of note.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("text")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Obsolete]
        public string Text { get; set; }

        /// <summary>
        /// Short freetext of note, may be empty.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("textShort")]
        [System.Obsolete]
        public string TextShort { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Message note [Hinweistext] descriptions.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class MessageNoteDescription
    {

        /// <summary>
        /// Attachments for additional information that may be communicated to the traveller (download use-cases).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("attachments")]
        public System.Collections.Generic.ICollection<DisruptionCommunicationAttachment> Attachments { get; set; }

        /// <summary>
        /// Images for additional information that may be communicated to the traveller.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("images")]
        public System.Collections.Generic.ICollection<DisruptionCommunicationImage> Images { get; set; }

        /// <summary>
        /// Links for additional information that may be communicated to the traveller.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("links")]
        public System.Collections.Generic.ICollection<DisruptionCommunicationLink> Links { get; set; }

        /// <summary>
        /// Long text of message note.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("text")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Text { get; set; }

        /// <summary>
        /// Optional short text of message note.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("textShort")]
        public string TextShort { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Ris cause code [RIS Kundengrund] message.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class MessageRisCauseCode
    {

        /// <summary>
        /// RIS cause code [Code für RIS Kundengrund].
        /// <br/>- 00 (entspricht 'keine Verspätungsbegründung')
        /// <br/>- 01 (entspricht 'nähere Informationen in Kürze')
        /// <br/>- 02 (entspricht 'Polizeieinsatz')
        /// <br/>- 03 (entspricht 'Feuerwehreinsatz auf der Strecke')
        /// <br/>- 04 (aktuell nicht belegt)
        /// <br/>- 05 (entspricht 'ärztliche Versorgung eines Fahrgastes')
        /// <br/>- 06 (entspricht 'unbefugtes Ziehen der Notbremse')
        /// <br/>- 07 (entspricht 'unbefugte Personen auf der Strecke')
        /// <br/>- 08 (entspricht 'Notarzteinsatz auf der Strecke')
        /// <br/>- 09 (entspricht 'Streikauswirkungen')
        /// <br/>- 10 (entspricht 'Tiere auf der Strecke')
        /// <br/>- 11 (entspricht 'Unwetter')
        /// <br/>- 12 (entspricht 'Warten auf ein verspätetes Schiff')
        /// <br/>- 13 (entspricht 'Pass- und Zollkontrolle')
        /// <br/>- 14 (aktuell nicht belegt)
        /// <br/>- 15 (entspricht 'Beeinträchtigung durch Vandalismus')
        /// <br/>- 16 (entspricht 'Entschärfung einer Fliegerbombe')
        /// <br/>- 17 (entspricht 'Beschädigung einer Brücke')
        /// <br/>- 18 (entspricht 'umgestürzter Baum auf der Strecke')
        /// <br/>- 19 (entspricht 'Unfall an einem Bahnübergang')
        /// <br/>- 20 (aktuell nicht belegt)
        /// <br/>- 21 (entspricht 'Warten auf Anschlussreisende')
        /// <br/>- 22 (entspricht 'Witterungsbedingte Beeinträchtigungen')
        /// <br/>- 23 (aktuell nicht belegt)
        /// <br/>- 24 (entspricht 'Verspätung im Ausland')
        /// <br/>- 25 (entspricht 'Bereitstellung weiterer Wagen')
        /// <br/>- 26 (aktuell nicht belegt)
        /// <br/>- 27 (aktuell nicht belegt)
        /// <br/>- 28 (entspricht 'Gegenstände auf der Strecke')
        /// <br/>- 29 (entspricht 'Ersatzverkehr mit Bus ist eingerichtet')
        /// <br/>- 30 (aktuell nicht belegt)
        /// <br/>- 31 (entspricht 'Bauarbeiten')
        /// <br/>- 32 (entspricht 'Unterstützung beim Ein- und Ausstieg')
        /// <br/>- 33 (entspricht 'Reparatur an der Oberleitung')
        /// <br/>- 34 (entspricht 'Reparatur an einem Signal')
        /// <br/>- 35 (entspricht 'Streckensperrung ')
        /// <br/>- 36 (entspricht 'Reparatur am Zug')
        /// <br/>- 37 (aktuell nicht belegt)
        /// <br/>- 38 (entspricht 'Reparatur an der Strecke')
        /// <br/>- 39 (entspricht '')
        /// <br/>- 40 (entspricht 'defektes Stellwerk')
        /// <br/>- 41 (aktuell nicht belegt)
        /// <br/>- 42 (entspricht 'vorübergehend verminderte Geschwindigkeit auf der Strecke')
        /// <br/>- 43 (entspricht 'Verspätung eines vorausfahrenden Zuges')
        /// <br/>- 44 (entspricht 'Warten auf einen entgegenkommenden Zug')
        /// <br/>- 45 (entspricht 'Vorfahrt eines anderen Zuges')
        /// <br/>- 46 (entspricht 'Vorfahrt eines anderen Zuges')
        /// <br/>- 47 (entspricht 'verspätete Bereitstellung des Zuges')
        /// <br/>- 48 (entspricht 'Verspätung aus vorheriger Fahrt')
        /// <br/>- 49 (entspricht 'kurzfristiger Personalausfall')
        /// <br/>- 50 (entspricht 'kurzfristige Erkrankung von Personal')
        /// <br/>- 51 (entspricht 'verspätetes Personal aus vorheriger Fahrt')
        /// <br/>- 52 (entspricht 'Streik')
        /// <br/>- 53 (entspricht 'Unwetterauswirkungen')
        /// <br/>- 54 (entspricht 'Verfügbarkeit der Gleise derzeit eingeschränkt')
        /// <br/>- 55 (aktuell nicht belegt)
        /// <br/>- 56 (entspricht 'Warten auf Anschlussreisende')
        /// <br/>- 57 (entspricht 'zusätzlicher Halt zum Ein- und Ausstieg')
        /// <br/>- 58 (entspricht 'Umleitung des Zuges')
        /// <br/>- 59 (entspricht 'Schnee und Eis')
        /// <br/>- 60 (entspricht 'witterungsbedingt verminderte Geschwindigkeit')
        /// <br/>- 61 (entspricht 'defekte Tür')
        /// <br/>- 62 (aktuell nicht belegt)
        /// <br/>- 63 (aktuell nicht belegt)
        /// <br/>- 64 (entspricht 'Reparatur an der Weiche')
        /// <br/>- 65 (entspricht 'Erdrutsch')
        /// <br/>- 66 (entspricht 'Hochwasser')
        /// <br/>- 67 (entspricht 'behördliche Maßnahme')
        /// <br/>- 68 (entspricht 'hohes Fahrgastaufkommen verlängert Ein- und Ausstieg')
        /// <br/>- 69 (entspricht 'Zug verkehrt mit verminderter Geschwindigkeit')
        /// <br/>- 99 (entspricht 'sonstige Gründe')
        /// <br/>
        /// <br/>note: list is not exhausting and more undocumented values may be returned
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("code")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Code { get; set; }

        /// <summary>
        /// ID of message that is unique within all message collections.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("messageID")]
        public int MessageID { get; set; }

        /// <summary>
        /// Text for code.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("text")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Text { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Ris quality deviation [RIS Qualitätsabweichungen] message.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class MessageRisQualityDeviation
    {

        /// <summary>
        /// RIS quality deviation [Code für RIS Qualitätsabweichung].
        /// <br/>- 70 (WLAN nicht verfügbar)
        /// <br/>- 71 (WLAN in einem/mehreren Wagen nicht verfügbar)
        /// <br/>- 72 (Info-/Entertainment nicht verfügbar)
        /// <br/>- 73 (Heute: Mehrzweckabteil vorne)
        /// <br/>- 74 (Heute: Mehrzweckabteil hinten)
        /// <br/>- 75 (Heute: 1. Klasse vorne)
        /// <br/>- 76 (Heute: 1. Klasse hinten)
        /// <br/>- 77 (1. Klasse fehlt)
        /// <br/>- 78 (aktuell nicht belegt)
        /// <br/>- 79 (Mehrzweckabteil fehlt)
        /// <br/>- 80 (andere Reihenfolge der Wagen)
        /// <br/>- 81 (aktuell nicht belegt)
        /// <br/>- 82 (mehrere Wagen fehlen)
        /// <br/>- 83 (defekte fahrzeuggebundene Einstiegshilfe)
        /// <br/>- 84 (Zug verkehrt richtig gereiht)
        /// <br/>- 85 (ein Wagen fehlt)
        /// <br/>- 86 (gesamter Zug ohne Reservierung)
        /// <br/>- 87 (einzelne Wagen ohne Reservierung)
        /// <br/>- 88 (keine Qualitätsmängel)
        /// <br/>- 89 (Reservierungen sind wieder vorhanden)
        /// <br/>- 90 (kein gastronomisches Angebot)
        /// <br/>- 91 (Fahrradmitnahme nicht möglich)
        /// <br/>- 92 (Eingeschränkte Fahrradbeförderung )
        /// <br/>- 93 (behindertengerechte Einrichtung fehlt)
        /// <br/>- 94 (Ersatzbewirtschaftung)
        /// <br/>- 95 (Universal-WC fehlt)
        /// <br/>- 96 (Überbesetzung mit Kulanzleistungen)
        /// <br/>- 97 (Überbesetzung ohne Kulanzleistungen)
        /// <br/>- 98 (sonstige Qualitätsmängel)
        /// <br/>
        /// <br/>note: list is not exhausting and more undocumented values may be returned
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("code")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Code { get; set; }

        /// <summary>
        /// ID of message that is unique within all message collections.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("messageID")]
        public int MessageID { get; set; }

        /// <summary>
        /// Text for code.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("text")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Text { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Container to deduplicate messages [Störungskommunikationen, Fahrtmerkmale, Hinweise, RIS Kundengründe oder RIS Qualitätsabweichungen] in order to assign them to journeys [Fahrten], journey-events [Fahrtereignisse wie Abfahrt und Ankunft] and stop-places [Haltestellen].
    /// <br/>- messages are provided once and may be referenced multiple times [z.B. ein Fahrtmerkmal für alle Fahrtereignisse oder eine Störungskommunikation für alle Abfahrten]
    /// <br/>- ids are not stable and only valid within the provided context (ie first journey request may have different message ids than second request for the same journey)
    /// <br/>- ids are unique over all buckets
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Messages
    {

        /// <summary>
        /// List of journey-attributes [Fahrtmerkmale / Sollmerkmale].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("attributes")]
        public System.Collections.Generic.ICollection<MessageAttribute> Attributes { get; set; }

        /// <summary>
        /// List of disruption-communications [Störungskommunikationen].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("disruptions")]
        public System.Collections.Generic.ICollection<MessageDisruptionCommunication> Disruptions { get; set; }

        /// <summary>
        /// List of notes [Freitexte / Hinweistexte].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("notes")]
        public System.Collections.Generic.ICollection<MessageNote> Notes { get; set; }

        /// <summary>
        /// List of ris cause codes [RIS Kundengründe].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("risCauseCodes")]
        public System.Collections.Generic.ICollection<MessageRisCauseCode> RisCauseCodes { get; set; }

        /// <summary>
        /// List of ris quality deviations [RIS Qualitätsabweichungen].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("risQualityDeviations")]
        public System.Collections.Generic.ICollection<MessageRisQualityDeviation> RisQualityDeviations { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Meta information a journey has been changed the last time for batch results.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class MetaLastChangedTimestamp
    {

        /// <summary>
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

        /// <summary>
        /// Timestamp the journey has been changed the last time as fully-qualified-date (ISO-8601 with time-zone or offset).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("metaLastChangedTimestamp")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset MetaLastChangedTimestamp1 { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Replacement transport [Ersatzverkehr] information, in case transport is a rail replacement transport [Schienenersatzverkehr (SEV)] or emergency bus service [Busnotverkehr]. Indicates that this transport is a replacement transport.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class ReplacementTransport
    {

        /// <summary>
        /// Real type of replacement transport that may differ from sales perspective (for instance a 'REGIONAL_TRAIN' is usuallay replaced by a 'BUS'). Possible values are:
        /// <br/>- HIGH_SPEED_TRAIN (High speed train [Hochgeschwindigkeitszug])
        /// <br/>- INTERCITY_TRAIN (Inter city train [Intercityzug])
        /// <br/>- INTER_REGIONAL_TRAIN (Inter regional train [Interregiozug])
        /// <br/>- REGIONAL_TRAIN (Regional train [Regionalzug])
        /// <br/>- CITY_TRAIN (City train [S-Bahn])
        /// <br/>- SUBWAY (Subway [U-Bahn])
        /// <br/>- TRAM (Tram [Strassenbahn])
        /// <br/>- BUS (Bus [Bus])
        /// <br/>- FERRY (Ferry [Faehre])
        /// <br/>- SHUTTLE (Shuttle [Ruftaxi])
        /// <br/>- FLIGHT (Flight [Flug])
        /// <br/>- CHARTER_TRAIN (Charter train [Charterzug])
        /// <br/>- UNKNOWN (Unknown)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("realType")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string RealType { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Differing stop-place [Haltestelle] in case a platform change [Gleiswechsel] within a station [Bahnhof], the stop-place belongs to, occurs and stop-place ids are changed (for instance a platform change in 'Frankfurt Main Hbf' from platform '12' to '103' will result in a change of the stop-place id from '8000105' to '8098105' hence stopPlace.evaNumber = '8000105' and stopPlace.differingStopPlace.evaNumber = '8098105').
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaceDifferingInJourney
    {

        /// <summary>
        /// Eva number of stop-place [Haltestelle].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("evaNumber")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string EvaNumber { get; set; }

        /// <summary>
        /// Transmodel identifier for fixed objects, in germany DHID = Deutschlandweite Halt ID also known as global id, of stop-place [Haltestelle].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("ifopt")]
        public string Ifopt { get; set; }

        /// <summary>
        /// Name for stop-place [Haltestelle] in fixed language 'DE'.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Comprehensive stop-place [Haltestelle] information.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaceEmbedded
    {

        /// <summary>
        /// Eva number of stop-place [Haltestelle].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("evaNumber")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string EvaNumber { get; set; }

        /// <summary>
        /// Transmodel identifier for fixed objects, in germany DHID = Deutschlandweite Halt ID also known as global id, of stop-place [Haltestelle].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("ifopt")]
        public string Ifopt { get; set; }

        /// <summary>
        /// Name for stop-place [Haltestelle] in fixed language 'DE'.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Comprehensive stop-place [Haltestelle] information.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaceEmbeddedWithCancel
    {

        /// <summary>
        /// Indicates whether the stop ie departure / arrival has been cancelled [Haltausfall].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("cancelled")]
        public bool Cancelled { get; set; }

        /// <summary>
        /// Eva number of stop-place [Haltestelle].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("evaNumber")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string EvaNumber { get; set; }

        /// <summary>
        /// Transmodel identifier for fixed objects, in germany DHID = Deutschlandweite Halt ID also known as global id, of stop-place [Haltestelle].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("ifopt")]
        public string Ifopt { get; set; }

        /// <summary>
        /// Name for stop-place [Haltestelle] in fixed language 'DE'.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Stop-place [Haltestelle] information for a stop-place a transport [Verkehrsart / Verkehrsmittel] departs / arrives at.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopPlaceInJourney
    {

        [System.Text.Json.Serialization.JsonPropertyName("differingStopPlace")]
        public StopPlaceDifferingInJourney DifferingStopPlace { get; set; }

        /// <summary>
        /// Eva number of stop-place [Haltestelle].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("evaNumber")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string EvaNumber { get; set; }

        /// <summary>
        /// Transmodel identifier for fixed objects, in germany DHID = Deutschlandweite Halt ID also known as global id, of stop-place [Haltestelle].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("ifopt")]
        public string Ifopt { get; set; }

        /// <summary>
        /// Name for stop-place [Haltestelle] in fixed language 'DE'.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Transport [Verkehrsart / Verkehrsmittel] information.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Transport
    {

        [System.Text.Json.Serialization.JsonPropertyName("administration")]
        [System.ComponentModel.DataAnnotations.Required]
        public Administration Administration { get; set; } = new Administration();

        /// <summary>
        /// Category of the transport [externe Fahrtgattung].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("category")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Category { get; set; }

        /// <summary>
        /// Internal category of the transport [interne Fahrtgattung vor Ausgabensteuerung). Must not be shown to customers.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("categoryInternal")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string CategoryInternal { get; set; }

        /// <summary>
        /// Deutschlandweite Fahrt ID (DFID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dfid")]
        public string Dfid { get; set; }

        /// <summary>
        /// Deutschlandweite Teil-Linien ID (DTID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dtid")]
        public string Dtid { get; set; }

        /// <summary>
        /// Description of the journey [Fahrtbezeichnung] that must be used to inform the traveller.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyDescription")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string JourneyDescription { get; set; }

        /// <summary>
        /// Number of the transport [Fahrtnummer].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyNumber")]
        public int JourneyNumber { get; set; }

        /// <summary>
        /// Marketing or product name of the transport, for instance 'Sprinter' or 'Schwarzwaldexpress' etc. [Marketing Name / Produkt Name].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// Line of the transport [Linie].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("line")]
        public string Line { get; set; }

        /// <summary>
        /// Type of transport. Possible values are:
        /// <br/>- HIGH_SPEED_TRAIN (High speed train [Hochgeschwindigkeitszug])
        /// <br/>- INTERCITY_TRAIN (Inter city train [Intercityzug])
        /// <br/>- INTER_REGIONAL_TRAIN (Inter regional train [Interregiozug])
        /// <br/>- REGIONAL_TRAIN (Regional train [Regionalzug])
        /// <br/>- CITY_TRAIN (City train [S-Bahn])
        /// <br/>- SUBWAY (Subway [U-Bahn])
        /// <br/>- TRAM (Tram [Strassenbahn])
        /// <br/>- BUS (Bus [Bus])
        /// <br/>- FERRY (Ferry [Faehre])
        /// <br/>- SHUTTLE (Shuttle [Ruftaxi])
        /// <br/>- FLIGHT (Flight [Flug])
        /// <br/>- CHARTER_TRAIN (Charter train [Charterzug])
        /// <br/>- UNKNOWN (Unknown)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Public transport [Oeffentlicher Transport] with (scheduled) destination [Ziel] and optional information for continuations successor [Durchbindungen am Zielhalt].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class TransportDestinationContinuationRef
    {

        /// <summary>
        /// Category of the transport [externe Fahrtgattung].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("category")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Category { get; set; }

        /// <summary>
        /// Internal category of the transport [interne Fahrtgattung vor Ausgabensteuerung). Must not be shown to customers.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("categoryInternal")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string CategoryInternal { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("destination")]
        [System.ComponentModel.DataAnnotations.Required]
        public StopPlaceEmbeddedWithCancel Destination { get; set; } = new StopPlaceEmbeddedWithCancel();

        /// <summary>
        /// Deutschlandweite Fahrt ID (DFID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dfid")]
        public string Dfid { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("differingContinuation")]
        public ContinuationInfo DifferingContinuation { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("differingDestination")]
        public StopPlaceEmbedded DifferingDestination { get; set; }

        /// <summary>
        /// Deutschlandweite Teil-Linien ID (DTID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dtid")]
        public string Dtid { get; set; }

        /// <summary>
        /// Description of the journey [Fahrtbezeichnung] that must be used to inform the traveller.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyDescription")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string JourneyDescription { get; set; }

        /// <summary>
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

        /// <summary>
        /// Number of the transport [Fahrtnummer].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyNumber")]
        public int JourneyNumber { get; set; }

        /// <summary>
        /// Marketing or product name of the transport, for instance 'Sprinter' or 'Schwarzwaldexpress' etc. [Marketing Name / Produkt Name].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// Line of the transport [Linie].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("line")]
        public string Line { get; set; }

        /// <summary>
        /// Type of transport. Possible values are:
        /// <br/>- HIGH_SPEED_TRAIN (High speed train [Hochgeschwindigkeitszug])
        /// <br/>- INTERCITY_TRAIN (Inter city train [Intercityzug])
        /// <br/>- INTER_REGIONAL_TRAIN (Inter regional train [Interregiozug])
        /// <br/>- REGIONAL_TRAIN (Regional train [Regionalzug])
        /// <br/>- CITY_TRAIN (City train [S-Bahn])
        /// <br/>- SUBWAY (Subway [U-Bahn])
        /// <br/>- TRAM (Tram [Strassenbahn])
        /// <br/>- BUS (Bus [Bus])
        /// <br/>- FERRY (Ferry [Faehre])
        /// <br/>- SHUTTLE (Shuttle [Ruftaxi])
        /// <br/>- FLIGHT (Flight [Flug])
        /// <br/>- CHARTER_TRAIN (Charter train [Charterzug])
        /// <br/>- UNKNOWN (Unknown)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Public transport [Oeffentlicher Transport] with (scheduled) destination [Ziel] and differing destination [abweichender Zielhalt] for coupled transports [vereinigte Züge].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class TransportDestinationPortionWorkingRef
    {

        /// <summary>
        /// Category of the transport [externe Fahrtgattung].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("category")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Category { get; set; }

        /// <summary>
        /// Internal category of the transport [interne Fahrtgattung vor Ausgabensteuerung). Must not be shown to customers.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("categoryInternal")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string CategoryInternal { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("destination")]
        [System.ComponentModel.DataAnnotations.Required]
        public StopPlaceEmbeddedWithCancel Destination { get; set; } = new StopPlaceEmbeddedWithCancel();

        /// <summary>
        /// Deutschlandweite Fahrt ID (DFID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dfid")]
        public string Dfid { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("differingDestination")]
        public StopPlaceEmbedded DifferingDestination { get; set; }

        /// <summary>
        /// Deutschlandweite Teil-Linien ID (DTID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dtid")]
        public string Dtid { get; set; }

        /// <summary>
        /// Description of the journey [Fahrtbezeichnung] that must be used to inform the traveller.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyDescription")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string JourneyDescription { get; set; }

        /// <summary>
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

        /// <summary>
        /// Number of the transport [Fahrtnummer].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyNumber")]
        public int JourneyNumber { get; set; }

        /// <summary>
        /// Marketing or product name of the transport, for instance 'Sprinter' or 'Schwarzwaldexpress' etc. [Marketing Name / Produkt Name].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// Line of the transport [Linie].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("line")]
        public string Line { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("separationAt")]
        public StopPlaceEmbedded SeparationAt { get; set; }

        /// <summary>
        /// Type of transport. Possible values are:
        /// <br/>- HIGH_SPEED_TRAIN (High speed train [Hochgeschwindigkeitszug])
        /// <br/>- INTERCITY_TRAIN (Inter city train [Intercityzug])
        /// <br/>- INTER_REGIONAL_TRAIN (Inter regional train [Interregiozug])
        /// <br/>- REGIONAL_TRAIN (Regional train [Regionalzug])
        /// <br/>- CITY_TRAIN (City train [S-Bahn])
        /// <br/>- SUBWAY (Subway [U-Bahn])
        /// <br/>- TRAM (Tram [Strassenbahn])
        /// <br/>- BUS (Bus [Bus])
        /// <br/>- FERRY (Ferry [Faehre])
        /// <br/>- SHUTTLE (Shuttle [Ruftaxi])
        /// <br/>- FLIGHT (Flight [Flug])
        /// <br/>- CHARTER_TRAIN (Charter train [Charterzug])
        /// <br/>- UNKNOWN (Unknown)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Transport [Verkehrsart / Verkehrsmittel] information for journey references with destination.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class TransportDestinationRef
    {

        /// <summary>
        /// Category of the transport [externe Fahrtgattung].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("category")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Category { get; set; }

        /// <summary>
        /// Internal category of the transport [interne Fahrtgattung vor Ausgabensteuerung). Must not be shown to customers.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("categoryInternal")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string CategoryInternal { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("destination")]
        [System.ComponentModel.DataAnnotations.Required]
        public StopPlaceEmbeddedWithCancel Destination { get; set; } = new StopPlaceEmbeddedWithCancel();

        /// <summary>
        /// Deutschlandweite Fahrt ID (DFID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dfid")]
        public string Dfid { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("differingDestination")]
        public StopPlaceEmbedded DifferingDestination { get; set; }

        /// <summary>
        /// Deutschlandweite Teil-Linien ID (DTID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dtid")]
        public string Dtid { get; set; }

        /// <summary>
        /// Description of the journey [Fahrtbezeichnung] that must be used to inform the traveller.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyDescription")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string JourneyDescription { get; set; }

        /// <summary>
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

        /// <summary>
        /// Number of the transport [Fahrtnummer].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyNumber")]
        public int JourneyNumber { get; set; }

        /// <summary>
        /// Marketing or product name of the transport, for instance 'Sprinter' or 'Schwarzwaldexpress' etc. [Marketing Name / Produkt Name].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// Line of the transport [Linie].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("line")]
        public string Line { get; set; }

        /// <summary>
        /// Type of transport. Possible values are:
        /// <br/>- HIGH_SPEED_TRAIN (High speed train [Hochgeschwindigkeitszug])
        /// <br/>- INTERCITY_TRAIN (Inter city train [Intercityzug])
        /// <br/>- INTER_REGIONAL_TRAIN (Inter regional train [Interregiozug])
        /// <br/>- REGIONAL_TRAIN (Regional train [Regionalzug])
        /// <br/>- CITY_TRAIN (City train [S-Bahn])
        /// <br/>- SUBWAY (Subway [U-Bahn])
        /// <br/>- TRAM (Tram [Strassenbahn])
        /// <br/>- BUS (Bus [Bus])
        /// <br/>- FERRY (Ferry [Faehre])
        /// <br/>- SHUTTLE (Shuttle [Ruftaxi])
        /// <br/>- FLIGHT (Flight [Flug])
        /// <br/>- CHARTER_TRAIN (Charter train [Charterzug])
        /// <br/>- UNKNOWN (Unknown)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Public transport [Oeffentlicher Transport] with (scheduled) origin [Start] and optional information for continuations predecessors [Durchbindungen am Starthalt].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class TransportOriginContinuationRef
    {

        /// <summary>
        /// Category of the transport [externe Fahrtgattung].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("category")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Category { get; set; }

        /// <summary>
        /// Internal category of the transport [interne Fahrtgattung vor Ausgabensteuerung). Must not be shown to customers.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("categoryInternal")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string CategoryInternal { get; set; }

        /// <summary>
        /// Deutschlandweite Fahrt ID (DFID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dfid")]
        public string Dfid { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("differingContinuation")]
        public ContinuationInfo DifferingContinuation { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("differingOrigin")]
        public StopPlaceEmbedded DifferingOrigin { get; set; }

        /// <summary>
        /// Deutschlandweite Teil-Linien ID (DTID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dtid")]
        public string Dtid { get; set; }

        /// <summary>
        /// Description of the journey [Fahrtbezeichnung] that must be used to inform the traveller.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyDescription")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string JourneyDescription { get; set; }

        /// <summary>
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

        /// <summary>
        /// Number of the transport [Fahrtnummer].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyNumber")]
        public int JourneyNumber { get; set; }

        /// <summary>
        /// Marketing or product name of the transport, for instance 'Sprinter' or 'Schwarzwaldexpress' etc. [Marketing Name / Produkt Name].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// Line of the transport [Linie].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("line")]
        public string Line { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("origin")]
        [System.ComponentModel.DataAnnotations.Required]
        public StopPlaceEmbeddedWithCancel Origin { get; set; } = new StopPlaceEmbeddedWithCancel();

        /// <summary>
        /// Type of transport. Possible values are:
        /// <br/>- HIGH_SPEED_TRAIN (High speed train [Hochgeschwindigkeitszug])
        /// <br/>- INTERCITY_TRAIN (Inter city train [Intercityzug])
        /// <br/>- INTER_REGIONAL_TRAIN (Inter regional train [Interregiozug])
        /// <br/>- REGIONAL_TRAIN (Regional train [Regionalzug])
        /// <br/>- CITY_TRAIN (City train [S-Bahn])
        /// <br/>- SUBWAY (Subway [U-Bahn])
        /// <br/>- TRAM (Tram [Strassenbahn])
        /// <br/>- BUS (Bus [Bus])
        /// <br/>- FERRY (Ferry [Faehre])
        /// <br/>- SHUTTLE (Shuttle [Ruftaxi])
        /// <br/>- FLIGHT (Flight [Flug])
        /// <br/>- CHARTER_TRAIN (Charter train [Charterzug])
        /// <br/>- UNKNOWN (Unknown)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Transport [Verkehrsart / Verkehrsmittel] information.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class TransportWithDirection
    {

        [System.Text.Json.Serialization.JsonPropertyName("administration")]
        [System.ComponentModel.DataAnnotations.Required]
        public Administration Administration { get; set; } = new Administration();

        /// <summary>
        /// Category of the transport [externe Fahrtgattung].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("category")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Category { get; set; }

        /// <summary>
        /// Internal category of the transport [interne Fahrtgattung vor Ausgabensteuerung). Must not be shown to customers.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("categoryInternal")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string CategoryInternal { get; set; }

        /// <summary>
        /// Deutschlandweite Fahrt ID (DFID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dfid")]
        public string Dfid { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("direction")]
        public DirectionInfo Direction { get; set; }

        /// <summary>
        /// Deutschlandweite Teil-Linien ID (DTID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dtid")]
        public string Dtid { get; set; }

        /// <summary>
        /// Description of the journey [Fahrtbezeichnung] that must be used to inform the traveller.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyDescription")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string JourneyDescription { get; set; }

        /// <summary>
        /// Number of the transport [Fahrtnummer].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyNumber")]
        public int JourneyNumber { get; set; }

        /// <summary>
        /// Marketing or product name of the transport, for instance 'Sprinter' or 'Schwarzwaldexpress' etc. [Marketing Name / Produkt Name].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// Line of the transport [Linie].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("line")]
        public string Line { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("replacementTransport")]
        public ReplacementTransport ReplacementTransport { get; set; }

        /// <summary>
        /// Type of transport. Possible values are:
        /// <br/>- HIGH_SPEED_TRAIN (High speed train [Hochgeschwindigkeitszug])
        /// <br/>- INTERCITY_TRAIN (Inter city train [Intercityzug])
        /// <br/>- INTER_REGIONAL_TRAIN (Inter regional train [Interregiozug])
        /// <br/>- REGIONAL_TRAIN (Regional train [Regionalzug])
        /// <br/>- CITY_TRAIN (City train [S-Bahn])
        /// <br/>- SUBWAY (Subway [U-Bahn])
        /// <br/>- TRAM (Tram [Strassenbahn])
        /// <br/>- BUS (Bus [Bus])
        /// <br/>- FERRY (Ferry [Faehre])
        /// <br/>- SHUTTLE (Shuttle [Ruftaxi])
        /// <br/>- FLIGHT (Flight [Flug])
        /// <br/>- CHARTER_TRAIN (Charter train [Charterzug])
        /// <br/>- UNKNOWN (Unknown)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    internal class DateFormatConverter : System.Text.Json.Serialization.JsonConverter<System.DateTimeOffset>
    {
        public override System.DateTimeOffset Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            var dateTime = reader.GetString();
            if (dateTime == null)
            {
                throw new System.Text.Json.JsonException("Unexpected JsonTokenType.Null");
            }

            return System.DateTimeOffset.Parse(dateTime);
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, System.DateTimeOffset value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
        }
    }
}
