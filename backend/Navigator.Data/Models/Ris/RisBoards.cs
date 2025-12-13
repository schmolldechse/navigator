using System.Text.Json;

namespace Navigator.Data.Models.Ris;

public class RisBoards
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
    /// Arrival board [Ankunftstafel] for public transports [Öffentliche Verkehre] ie trains, buses, trams, subways etc.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class BoardPublicArrival
    {

        /// <summary>
        /// List of available arrivals [Ankünfte].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("arrivals")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<StopArrival> Arrivals { get; set; } = new System.Collections.ObjectModel.Collection<StopArrival>();

        /// <summary>
        /// List of disruptions [Stoerungsinformationen] for particular stop-place (or members of the requested stop-place group) the board applies to.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("disruptions")]
        public System.Collections.Generic.ICollection<DisruptionCommunicationEmbeddedLegacy> Disruptions { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Departure board [Abfahrtstafel] for public transports [Öffentliche Verkehre] ie trains, buses, trams, subways etc..
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class BoardPublicDeparture
    {

        /// <summary>
        /// List of available departures [Abfahrten].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("departures")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<StopDeparture> Departures { get; set; } = new System.Collections.ObjectModel.Collection<StopDeparture>();

        /// <summary>
        /// List of disruptions [Störungsinformationen] for particular stop-place (or members of the requested stop-place group) the board applies to.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("disruptions")]
        public System.Collections.Generic.ICollection<DisruptionCommunicationEmbeddedLegacy> Disruptions { get; set; }

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
    /// Disruption communication information [Stoerungskommunikation] descriptions.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class DisruptionCommunicationDescription
    {

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
    /// Embedded disruption communication information [Stoerungskommunikation].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class DisruptionCommunicationEmbeddedLegacy
    {

        /// <summary>
        /// Textual short description of disruption by language identifier.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("descriptions")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.IDictionary<string, DisruptionCommunicationDescription> Descriptions { get; set; } = new System.Collections.Generic.Dictionary<string, DisruptionCommunicationDescription>();

        /// <summary>
        /// Display priority [Anzeigereihenfolge] for disruption. Order is by display priority asc. May be empty.
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
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string DisruptionID { get; set; }

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
    /// Defines whether journey [Fahrt] is regular or some kind of special.
    /// <br/>- REGULAR (Regular scheduled journey)
    /// <br/>- REPLACEMENT (Journey that replaces another journey)
    /// <br/>- RELIEF (Journey that reliefs another journey)
    /// <br/>- EXTRA (Journey that is somehow extra
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    [System.Text.Json.Serialization.JsonConverter(typeof(TolerantJourneyTypeConverter))]
    public enum JourneyType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"REGULAR")]
        REGULAR = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"REPLACEMENT")]
        REPLACEMENT = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"RELIEF")]
        RELIEF = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"EXTRA")]
        EXTRA = 3,

    }

    internal class TolerantJourneyTypeConverter : System.Text.Json.Serialization.JsonConverter<JourneyType>
    {
        public override JourneyType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string journeyType = reader.GetString();
            if (Enum.TryParse<JourneyType>(journeyType, true, out var result)) return result;
            return JourneyType.REGULAR;
        }

        public override void Write(Utf8JsonWriter writer, JourneyType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    /// <summary>
    /// Journey-attribute [Fahrtmerkmale / Sollmerkmale] message.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class MessageAttributeLegacy
    {

        /// <summary>
        /// Journey-attribute [Fahrtmerkmale / Sollmerkmale].
        /// <br/>- FR (Fahrradmitnahme reservierungspflichtig)
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
        public int? DisplayPriority { get; set; }

        /// <summary>
        /// Detailed display priority [detaillierte Anzeigereihenfolge aka 'Feinsortierung'] for message. Order is ascending.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("displayPriorityDetail")]
        public int? DisplayPriorityDetail { get; set; }

        /// <summary>
        /// Text for attribute.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("text")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Text { get; set; }

        /// <summary>
        /// Short freetext of message, may be empty.
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
    /// Message for customers.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class MessageLegacy
    {

        /// <summary>
        /// Optional category of message, like for instance 'Bauarbeiten' or 'Informationen'
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("category")]
        public string Category { get; set; }

        /// <summary>
        /// Unique code of message may be empty in case of HIM based messages.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// Display priority [Anzeigereihenfolge] for message. Order is by display priority asc. May be empty.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("displayPriority")]
        public int? DisplayPriority { get; set; }

        /// <summary>
        /// Freetext of message.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("text")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Text { get; set; }

        /// <summary>
        /// Short freetext of message, may be empty.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("textShort")]
        public string TextShort { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<MessageType>))]
        public MessageType Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Classification of message.
    /// <br/>- CUSTOMER_TEXT (unstructured free text [unstrukturierter Kundenfreitext], may be based on HIM messages (no disruptions!) (for instance 'Bitte beachten Sie die Maskenpflicht')
    /// <br/>- QUALITY_VARIATION (structured quality variations [struckturierte Qualitätsabweichung] (for instance 'Geänderte Wagenreihung' or 'Fahrradmitnahme nicht möglich')
    /// <br/>- CUSTOMER_REASON (structured customer reasons [struckturierte Kundenbegründungen] (for instance 'Umgestürzter Baum auf Strecke' or 'Verspätung aufgrund vorausfahrendem Zug')
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    [System.Text.Json.Serialization.JsonConverter(typeof(TolerantMessageTypeConverter))]
    public enum MessageType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"CUSTOMER_TEXT")]
        CUSTOMER_TEXT = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"QUALITY_VARIATION")]
        QUALITY_VARIATION = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"CUSTOMER_REASON")]
        CUSTOMER_REASON = 2,

    }

    internal class TolerantMessageTypeConverter : System.Text.Json.Serialization.JsonConverter<MessageType>
    {
        public override MessageType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string messageType = reader.GetString();
            if (Enum.TryParse<MessageType>(messageType, true, out var result)) return result;
            return MessageType.CUSTOMER_TEXT;
        }

        public override void Write(Utf8JsonWriter writer, MessageType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
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
    /// Sort keys for time based sorting.
    /// <br/>- TIME (Sorting based on best known time information ie real before preview before schedule)
    /// <br/>- TIME_SCHEDULE (Sorting based on schedule time)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    [System.Text.Json.Serialization.JsonConverter(typeof(TolerantSortKeyTimeConverter))]
    public enum SortKeyTime
    {

        [System.Runtime.Serialization.EnumMember(Value = @"TIME")]
        TIME = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"TIME_SCHEDULE")]
        TIME_SCHEDULE = 1,

    }

    internal class TolerantSortKeyTimeConverter : System.Text.Json.Serialization.JsonConverter<SortKeyTime>
    {
        public override SortKeyTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string sortKeyTime = reader.GetString();
            if (Enum.TryParse<SortKeyTime>(sortKeyTime, true, out var result)) return result;
            return SortKeyTime.TIME_SCHEDULE;
        }

        public override void Write(Utf8JsonWriter writer, SortKeyTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    /// <summary>
    /// Arrival [Ankunft] information within arrival boards.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopArrival
    {

        /// <summary>
        /// Indicates whether this arrival is additional [Zusatzhalt], meaning not be part of the regular schedule.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("additional")]
        public bool Additional { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("administration")]
        [System.ComponentModel.DataAnnotations.Required]
        public Administration Administration { get; set; } = new Administration();

        /// <summary>
        /// ID of arrival [AnkunftID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("arrivalID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(12)]
        public string ArrivalID { get; set; }

        /// <summary>
        /// List of journey-attributes [Fahrtmerkmale / Sollmerkmale] for particular stop..
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("attributes")]
        public System.Collections.Generic.ICollection<MessageAttributeLegacy> Attributes { get; set; }

        /// <summary>
        /// Indicates whether the arrival has been canceled [Haltausfall].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("canceled")]
        public bool Canceled { get; set; }

        /// <summary>
        /// List of codeshares [Code-Teilungen mit Flügen verschiedener Fluggesellschaften] for this particular journey at this arrival / departure.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("codeshares")]
        public System.Collections.Generic.ICollection<CodeShare> Codeshares { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("continuationFor")]
        public TransportPublicOrigin ContinuationFor { get; set; }

        /// <summary>
        /// List of disruptions [Stoerungsinformationen] for particular stop.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("disruptions")]
        public System.Collections.Generic.ICollection<DisruptionCommunicationEmbeddedLegacy> Disruptions { get; set; }

        /// <summary>
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("journeyType")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<JourneyType>))]
        public JourneyType JourneyType { get; set; }

        /// <summary>
        /// List of available messages to display for this departure.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("messages")]
        public System.Collections.Generic.ICollection<MessageLegacy> Messages { get; set; }

        /// <summary>
        /// Indicates whether stop is an on demand stop [Bedarfshalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("onDemand")]
        public bool OnDemand { get; set; }

        /// <summary>
        /// Indicates whether there was at least one disruption in the past of this journey.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("pastDisruptions")]
        public bool PastDisruptions { get; set; }

        /// <summary>
        /// Actual platform [Gleis, Bahnsteig, Plattform] the transport departs at.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("platform")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Platform { get; set; }

        /// <summary>
        /// Scheduled platform [Gleis, Bahnsteig, Plattform] the transport departs at.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("platformSchedule")]
        public string PlatformSchedule { get; set; }

        /// <summary>
        /// List of transports this journey is reliefed by [Entlastungszug].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("reliefBy")]
        public System.Collections.Generic.ICollection<TransportPublicOrigin> ReliefBy { get; set; }

        /// <summary>
        /// List of transports this journey reliefs for [Entlastungszug].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("reliefFor")]
        public System.Collections.Generic.ICollection<TransportPublicOrigin> ReliefFor { get; set; }

        /// <summary>
        /// List of transports this journey is replaced by [Ersatzzug].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("replacedBy")]
        public System.Collections.Generic.ICollection<TransportPublicOrigin> ReplacedBy { get; set; }

        /// <summary>
        /// List of transports this journey replaces [Ersatzzug].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("replacementFor")]
        public System.Collections.Generic.ICollection<TransportPublicOrigin> ReplacementFor { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("station")]
        [System.ComponentModel.DataAnnotations.Required]
        public StopPlaceEmbedded Station { get; set; } = new StopPlaceEmbedded();

        /// <summary>
        /// Best known time information of stop as fully qualified date (for instance '2019-08-19T12:56:14+02:00' or '2019-08-19T10:56:14Z').
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("time")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset Time { get; set; }

        /// <summary>
        /// Scheduled time [Abfahrtszeit Soll] of stop as fully qualified date (for instance '2019-08-19T12:56:14+02:00' or '2019-08-19T10:56:14Z').
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("timeSchedule")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset TimeSchedule { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("timeType")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<TimeType>))]
        public TimeType TimeType { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("transport")]
        [System.ComponentModel.DataAnnotations.Required]
        public TransportPublicOriginVia Transport { get; set; } = new TransportPublicOriginVia();

        /// <summary>
        /// List of journeys this journey travels with [Vereinigung].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("travelsWith")]
        public System.Collections.Generic.ICollection<TransportPublicOrigin> TravelsWith { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Stop at a particular stop-place [Haltestelle] for arrival / departure boards.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopAtStopPlace
    {

        /// <summary>
        /// Indicates whether the stop ie departure / arrival has been canceled [Haltausfall].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("canceled")]
        public bool Canceled { get; set; }

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
    /// Stop at a particular stop-place [Haltestelle] for arrival / departure boards with display priority [Anzeigeprioritaet].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopAtStopPlacePrio
    {

        /// <summary>
        /// Indicates whether this stop is additional [Zusatzhalt], meaning not be part of the regular schedule.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("additional")]
        public bool Additional { get; set; }

        /// <summary>
        /// Indicates whether the stop ie departure / arrival has been canceled [Haltausfall].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("canceled")]
        public bool Canceled { get; set; }

        /// <summary>
        /// Display priority for station within via list. Priority is ascending (1 = hightest priority, ...).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("displayPriority")]
        public int DisplayPriority { get; set; }

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
    /// Departure [Abfahrt] information within departure boards.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class StopDeparture
    {

        /// <summary>
        /// Indicates whether this departure is additional [Zusatzhalt], meaning not be part of the regular schedule.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("additional")]
        public bool Additional { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("administration")]
        [System.ComponentModel.DataAnnotations.Required]
        public Administration Administration { get; set; } = new Administration();

        /// <summary>
        /// List of journey-attributes [Fahrtmerkmale / Sollmerkmale] for particular stop..
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("attributes")]
        public System.Collections.Generic.ICollection<MessageAttributeLegacy> Attributes { get; set; }

        /// <summary>
        /// Indicates whether the departure has been canceled [Haltausfall].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("canceled")]
        public bool Canceled { get; set; }

        /// <summary>
        /// List of codeshares [Code-Teilungen mit Flügen verschiedener Fluggesellschaften] for this particular journey at this arrival / departure.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("codeshares")]
        public System.Collections.Generic.ICollection<CodeShare> Codeshares { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("continuationBy")]
        public TransportPublicDestination ContinuationBy { get; set; }

        /// <summary>
        /// ID of departure [AbfahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("departureID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(12)]
        public string DepartureID { get; set; }

        /// <summary>
        /// List of disruptions [Stoerungsinformationen] for particular stop.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("disruptions")]
        public System.Collections.Generic.ICollection<DisruptionCommunicationEmbeddedLegacy> Disruptions { get; set; }

        /// <summary>
        /// Indicates whether there is at least one disruption in the future of this journey.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("futureDisruptions")]
        public bool FutureDisruptions { get; set; }

        /// <summary>
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("journeyType")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<JourneyType>))]
        public JourneyType JourneyType { get; set; }

        /// <summary>
        /// List of available messages to display for this departure.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("messages")]
        public System.Collections.Generic.ICollection<MessageLegacy> Messages { get; set; }

        /// <summary>
        /// Indicates whether stop is an on demand stop [Bedarfshalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("onDemand")]
        public bool OnDemand { get; set; }

        /// <summary>
        /// Actual platform [Gleis, Bahnsteig, Plattform] the transport departs at.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("platform")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Platform { get; set; }

        /// <summary>
        /// Scheduled platform [Gleis, Bahnsteig, Plattform] the transport departs at.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("platformSchedule")]
        public string PlatformSchedule { get; set; }

        /// <summary>
        /// List of transports this journey is reliefed by [Entlastungszug].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("reliefBy")]
        public System.Collections.Generic.ICollection<TransportPublicDestination> ReliefBy { get; set; }

        /// <summary>
        /// List of transports this journey reliefs for [Entlastungszug].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("reliefFor")]
        public System.Collections.Generic.ICollection<TransportPublicDestination> ReliefFor { get; set; }

        /// <summary>
        /// List of transports this journey is replaced by [Ersatzzug].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("replacedBy")]
        public System.Collections.Generic.ICollection<TransportPublicDestination> ReplacedBy { get; set; }

        /// <summary>
        /// List of transports this journey replaces [Ersatzzug].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("replacementFor")]
        public System.Collections.Generic.ICollection<TransportPublicDestination> ReplacementFor { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("station")]
        [System.ComponentModel.DataAnnotations.Required]
        public StopPlaceEmbedded Station { get; set; } = new StopPlaceEmbedded();

        /// <summary>
        /// Best known time information of stop as fully qualified date (for instance '2019-08-19T12:56:14+02:00' or '2019-08-19T10:56:14Z').
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("time")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset Time { get; set; }

        /// <summary>
        /// Scheduled time [Abfahrtszeit Soll] of stop as fully qualified date (for instance '2019-08-19T12:56:14+02:00' or '2019-08-19T10:56:14Z').
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("timeSchedule")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset TimeSchedule { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("timeType")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<TimeType>))]
        public TimeType TimeType { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("transport")]
        [System.ComponentModel.DataAnnotations.Required]
        public TransportPublicDestinationVia Transport { get; set; } = new TransportPublicDestinationVia();

        /// <summary>
        /// List of journeys this journey travels with [Vereinigung].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("travelsWith")]
        public System.Collections.Generic.ICollection<TransportPublicDestinationPortionWorking> TravelsWith { get; set; }

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
    /// Specifies on which information 'time' is based.
    /// <br/>- SCHEDULE (Time source is schedule [Plan / Soll])
    /// <br/>- PREVIEW (Time source is preview / forecast [Vorschau / Disposition / Prognose])
    /// <br/>- REAL (Time source is real [Echt = passiert (kann nur in die Vergangenheit gesetzt werden)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    [System.Text.Json.Serialization.JsonConverter(typeof(TolerantTimeTypeConverter))]
    public enum TimeType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"SCHEDULE")]
        SCHEDULE = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"PREVIEW")]
        PREVIEW = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"REAL")]
        REAL = 2,

    }

    internal class TolerantTimeTypeConverter : System.Text.Json.Serialization.JsonConverter<TimeType>
    {
        public override TimeType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string timeType = reader.GetString();
            if (Enum.TryParse<TimeType>(timeType, true, out var result)) return result;
            return TimeType.SCHEDULE;
        }

        public override void Write(Utf8JsonWriter writer, TimeType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    /// <summary>
    /// Public transport [Oeffentlicher Transport] with (scheduled) destination [Ziel] and differing destination in case the final stop of the journey changed [Haltausfall, Laufwegverlängerung, ...].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class TransportPublicDestination
    {

        /// <summary>
        /// Code of the transport [Fahrtgattung].
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
        public StopAtStopPlace Destination { get; set; } = new StopAtStopPlace();

        /// <summary>
        /// Deutschlandweite Fahrt ID (DFID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dfid")]
        public string Dfid { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("differingDestination")]
        public StopAtStopPlace DifferingDestination { get; set; }

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
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

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
        /// Number of the transport [Fahrtnummer].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("number")]
        public int Number { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("replacementTransport")]
        public ReplacementTransport ReplacementTransport { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<TransportType>))]
        public TransportType Type { get; set; }

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
    public partial class TransportPublicDestinationPortionWorking
    {

        /// <summary>
        /// Code of the transport [Fahrtgattung].
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
        public StopAtStopPlace Destination { get; set; } = new StopAtStopPlace();

        /// <summary>
        /// Deutschlandweite Fahrt ID (DFID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dfid")]
        public string Dfid { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("differingDestination")]
        public StopAtStopPlace DifferingDestination { get; set; }

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
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

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
        /// Number of the transport [Fahrtnummer].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("number")]
        public int Number { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("replacementTransport")]
        public ReplacementTransport ReplacementTransport { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("separationAt")]
        public StopPlaceEmbedded SeparationAt { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<TransportType>))]
        public TransportType Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Public transport [Oeffentlicher Transport] with (scheduled) destination [Ziel], differing destination in case the final stop of the journey changed [Haltausfall, Laufwegverlängerung, ...], and via [Via-Halte].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class TransportPublicDestinationVia
    {

        /// <summary>
        /// Code of the transport [Fahrtgattung].
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
        public StopAtStopPlace Destination { get; set; } = new StopAtStopPlace();

        /// <summary>
        /// Deutschlandweite Fahrt ID (DFID).
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("dfid")]
        public string Dfid { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("differingDestination")]
        public StopAtStopPlace DifferingDestination { get; set; }

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
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

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
        /// Number of the transport [Fahrtnummer].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("number")]
        public int Number { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("replacementTransport")]
        public ReplacementTransport ReplacementTransport { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<TransportType>))]
        public TransportType Type { get; set; }

        /// <summary>
        /// List of remaining stations the transport stops at [ViaHalt].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("via")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<StopAtStopPlacePrio> Via { get; set; } = new System.Collections.ObjectModel.Collection<StopAtStopPlacePrio>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Public transport [Oeffentlicher Transport] with origin [Herkunft].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class TransportPublicOrigin
    {

        /// <summary>
        /// Code of the transport [Fahrtgattung].
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

        [System.Text.Json.Serialization.JsonPropertyName("differingOrigin")]
        public StopAtStopPlace DifferingOrigin { get; set; }

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
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

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
        /// Number of the transport [Fahrtnummer].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("number")]
        public int Number { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("origin")]
        [System.ComponentModel.DataAnnotations.Required]
        public StopAtStopPlace Origin { get; set; } = new StopAtStopPlace();

        [System.Text.Json.Serialization.JsonPropertyName("replacementTransport")]
        public ReplacementTransport ReplacementTransport { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<TransportType>))]
        public TransportType Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Public transport [Oeffentlicher Transport] with origin [Herkunft] and via [Via-Halte].
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class TransportPublicOriginVia
    {

        /// <summary>
        /// Code of the transport [Fahrtgattung].
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

        [System.Text.Json.Serialization.JsonPropertyName("differingOrigin")]
        public StopAtStopPlace DifferingOrigin { get; set; }

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
        /// ID of journey [FahrtID].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("journeyID")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(82)]
        public string JourneyID { get; set; }

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
        /// Number of the transport [Fahrtnummer].
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("number")]
        public int Number { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("origin")]
        [System.ComponentModel.DataAnnotations.Required]
        public StopAtStopPlace Origin { get; set; } = new StopAtStopPlace();

        [System.Text.Json.Serialization.JsonPropertyName("replacementTransport")]
        public ReplacementTransport ReplacementTransport { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("type")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<TransportType>))]
        public TransportType Type { get; set; }

        /// <summary>
        /// List of past stations the transport stoped at.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("via")]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<StopAtStopPlacePrio> Via { get; set; } = new System.Collections.ObjectModel.Collection<StopAtStopPlacePrio>();

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    /// <summary>
    /// Type of transport.
    /// <br/>- HIGH_SPEED_TRAIN (High speed train [Hochgeschwindigkeitszug] like ICE or TGV etc.)
    /// <br/>- INTERCITY_TRAIN (Inter city train [Intercityzug])
    /// <br/>- INTER_REGIONAL_TRAIN (Inter regional train [Interregiozug])
    /// <br/>- REGIONAL_TRAIN (Regional train [Regionalzug])
    /// <br/>- CITY_TRAIN (City train [S-Bahn])
    /// <br/>- SUBWAY (Subway [U-Bahn])
    /// <br/>- TRAM (Tram [Strassenbahn])
    /// <br/>- BUS (Bus [Bus])
    /// <br/>- FERRY (Ferry [Faehre])
    /// <br/>- FLIGHT (Flight [Flugzeug])
    /// <br/>- CAR (Car [Auto])
    /// <br/>- TAXI (Taxi)
    /// <br/>- SHUTTLE (Shuttle [Ruftaxi])
    /// <br/>- BIKE ((E-)Bike [Fahrrad])
    /// <br/>- SCOOTER ((E-)Scooter [Roller])
    /// <br/>- WALK (Walk ([Laufen])
    /// <br/>- UNKNOWN (Unknown)
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
    [System.Text.Json.Serialization.JsonConverter(typeof(TolerantTransportTypeConverter))]
    public enum TransportType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"HIGH_SPEED_TRAIN")]
        HIGH_SPEED_TRAIN = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"INTERCITY_TRAIN")]
        INTERCITY_TRAIN = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"INTER_REGIONAL_TRAIN")]
        INTER_REGIONAL_TRAIN = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"REGIONAL_TRAIN")]
        REGIONAL_TRAIN = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"CITY_TRAIN")]
        CITY_TRAIN = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"SUBWAY")]
        SUBWAY = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"TRAM")]
        TRAM = 6,

        [System.Runtime.Serialization.EnumMember(Value = @"BUS")]
        BUS = 7,

        [System.Runtime.Serialization.EnumMember(Value = @"FERRY")]
        FERRY = 8,

        [System.Runtime.Serialization.EnumMember(Value = @"FLIGHT")]
        FLIGHT = 9,

        [System.Runtime.Serialization.EnumMember(Value = @"CAR")]
        CAR = 10,

        [System.Runtime.Serialization.EnumMember(Value = @"TAXI")]
        TAXI = 11,

        [System.Runtime.Serialization.EnumMember(Value = @"SHUTTLE")]
        SHUTTLE = 12,

        [System.Runtime.Serialization.EnumMember(Value = @"BIKE")]
        BIKE = 13,

        [System.Runtime.Serialization.EnumMember(Value = @"SCOOTER")]
        SCOOTER = 14,

        [System.Runtime.Serialization.EnumMember(Value = @"WALK")]
        WALK = 15,

        [System.Runtime.Serialization.EnumMember(Value = @"UNKNOWN")]
        UNKNOWN = 16,

    }

    internal class TolerantTransportTypeConverter : System.Text.Json.Serialization.JsonConverter<TransportType>
    {
        public override TransportType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string transportType = reader.GetString();
            if (Enum.TryParse<TransportType>(transportType, true, out var result)) return result;
            return TransportType.UNKNOWN;
        }

        public override void Write(Utf8JsonWriter writer, TransportType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
