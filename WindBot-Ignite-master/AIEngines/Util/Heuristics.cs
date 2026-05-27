using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.AIEngines.Util
{
    class Heuristics
    {
        public static float GetHeuristics(Executor e)
        {
            float total = 0;

            total += e.Util.Bot.GetFieldCount() * 0.15f;
            total += e.Util.Bot.GetHandCount() * 0.1f;

            #region Fusion Monsters

            if (e.Util.Bot.HasInMonstersZone(CardId.AzaminaSilvera))
                total += 1;

            if (e.Util.Bot.HasInMonstersZone(CardId.FiendsmithDiesIrae))
            {
                List<ClientCard> equipped = new List<ClientCard>();
                var result = e.Util.Bot.MonsterZone.FirstOrDefault(x => x != null && x.IsCode(CardId.FiendsmithDiesIrae));
                if (result != null)
                    equipped = result.EquipCards;

                if (equipped.Any(x => x != null && x.HasAttribute(CardAttribute.Light) && x.HasRace(CardRace.Fiend)))
                    total += 2f;
                else if (e.Util.Bot.Graveyard.Any(x => x != null && x.HasAttribute(CardAttribute.Light) && x.HasRace(CardRace.Fiend)))
                    total += 0.8f;
            }

            #endregion

            #region Link Monsters

            if (e.Util.Bot.HasInMonstersZone(CardId.IPMasquerena) &&
                e.Util.Bot.GetMonsterCount() >= 2 &&
                e.Util.Bot.HasInExtra(CardId.SPLittleKnight))
                total += 1.5f;


            if (e.Util.Bot.HasInMonstersZone(CardId.SPLittleKnight))
                total += 0.5f;

            if (e.Util.Bot.HasInGraveyard(CardId.PromethianPrincess))
            {
                if (e.Util.Bot.MonsterZone.Any(x => x != null && x.HasAttribute(CardAttribute.Fire)))
                    total += 1f;

                else if (e.Util.Bot.SpellZone.Any(x => x != null && x.IsCode(CardId.DivineTempleSnakeEyes) && x.HasPosition(CardPosition.FaceUp)) &&
                    e.Util.Bot.SpellZone.Any(x => x != null && x.HasAttribute(CardAttribute.Fire)))
                    total += 0.9f;
            }

            if (e.Util.Bot.HasInGraveyard(CardId.SalamangreatRagingPhoenix) &&
                e.Util.Bot.MonsterZone.Any(x => x != null && x.HasAttribute(CardAttribute.Fire)))
                total += 0.5f;

            if (e.Util.Bot.HasInSpellZone(CardId.AngelStatueAzurune))
            {
                total += 0.5f;
                if (e.Util.Bot.HasInMonstersZone(CardId.SilhouhatteRabbit))
                    total += 0.5f;
            }

            
            #endregion




            return total;
        }


        public class CardId
        {
            // Generic Monsters
            public const int AshBlossom = 14558128;
            public const int EffectVeiler = 97268402;
            public const int GhostMourner = 52038441;
            public const int GhostOgre = 59438930;
            public const int GhostBelle = 73642296;
            public const int DrollnLockBird = 94145021;
            public const int Nibiru = 27204311;
            public const int DimensionShifter = 91800273;
            public const int MultchummyPurulia = 84192580;
            public const int FantasticalPhantazmay = 78661338;
            public const int BackJack = 60990740;
            public const int LordOfHeavelyPrison = 09822220;
            public const int Pankratops = 82385847;
            public const int DDCrow = 24508238;

            // Generic Spells
            public const int CrossoutDesignator = 65681983;
            public const int TripleTacticsTalent = 25311006;
            public const int CalledByTheGrave = 24224830;
            public const int ForbiddenDroplet = 24299458;
            public const int CosmicCyclone = 8267140;
            public const int SuperPoly = 48130397;           
            public const int BookOfEclipse = 35480699;

            // Generic Traps
            public const int TransactionRollback = 06351147;
            public const int BlackGoat = 49299410;
            public const int FusionDuplication = 43331750;

            public const int AngelStatueAzurune = 44822037;

            // Generic Synchro
            public const int BlackRoseMoonlightDragon = 33698022;
            public const int BlackroseDragon = 73580471;
            public const int UltimayaTzolkin = 1686814;
            public const int CrystalWingSynchroDragon = 50954680;
            public const int KuibeltTheBladeDragon = 87837090;
            public const int ChaosAngel = 22850702;
            public const int GoldenBeastMalong = 93125329;
            public const int EnigmasterPackbit = 72444406;
            // Generic Fusions
            public const int Garura = 11765832;
            public const int MudragonSwamp = 54757758;
            public const int ElderEntityNtss = 80532587;
            public const int GuardianChimera = 11321089;

            // Generic xyz
            public const int TyphonSkyCrisis = 93039339;
            public const int BeatriceLadyOfEnternal = 27552504;
            public const int Bagooska = 90590303;
            public const int ExcitonKnight = 46772449;
            public const int VarudrasBringerofEndTimes = 70636044;
            public const int DDDHighKingCaesar = 79559912;

            // Generic Links
            public const int SalamangreatRagingPhoenix = 57134592;
            public const int KnightmarePhoenix = 2857636;
            public const int PromethianPrincess = 2772337;
            public const int IPMasquerena = 65741786;
            public const int SPLittleKnight = 29301450;
            public const int WorldseadragonZealantis = 45112597;
            public const int Apollusa = 4280259;
            public const int UnderworldGoddess = 98127546;
            public const int HieraticSealsOfSpheres = 24361622;
            public const int Muckracker = 71607202;
            public const int SilhouhatteRabbit = 1528054;


            // Chimera
            public const int BerfometKingPhantomBeast = 69601012;
            public const int ChimeraKingPhantomBeast = 01269875;

            // Snake Eyes
            public const int SnakeEyeFlamberge = 48452496;
            public const int DiabellstarBlackWitch = 72270339;

            public const int DivineTempleSnakeEyes = 53639887;

            public const int AzaminaSilvera = 46396218;

            // Tenpai
            public const int TenpaiPaidra = 39931513;
            public const int TenpaiChundra = 91810826;
            public const int TenpaiFadra = 65326118;
            public const int TenpaiGenroku = 23657016;

            public const int SangenKaimen = 66730191;

            // Kashtira
            public const int KashtiraFenrir = 32909498;
            public const int PlanetWraithsoth = 71832012;

            // Fiendsmith
            public const int TheFiendsmith = 60764609;

            public const int FiendsmithTractus = 98567237;
            public const int FiendsmithSanctus = 35552985;

            public const int FiendsmithDiesIrae = 82135803;
            public const int FiendsmithLacrimosa = 46640168;

            public const int FiendsmithRequiem = 02463794;
            public const int FiendsmithSequentia = 49867899;

            public const int NecroqiopPrincess = 93860227;

            // Bystial
            public const int BystialMagnamhut = 33854624;
            public const int BystialDruiswurm = 6637331;
            public const int BystialSaronir = 60242223;
            public const int BystialBaldrake = 72656408;
            public const int BystialLubellion = 32731036;


            // Labrynth
            public const int LadyLabrnyth = 81497285;
            public const int LovelyLabrynth = 02347656;
            public const int AriasLabrnyth = 73602965;
            public const int ArianePinkLabrynth = 75730490;
            public const int AriannaGreenLabrynth = 01225009;
            public const int LabrynthChandraglier = 37629703;
            public const int LabrynthStovie = 74018812;
            public const int LabrynthCooClock = 00002511;
            public const int LabrynthSetup = 69895264;
            public const int LabrynthLabyrinth = 33407125;
            public const int WelcomeLabrynth = 05380979;
            public const int BigWelcomeLabrnyth = 92714517;

            // Dogmatika
            public const int NadirServant = 01984618;

            // Rescue Ace
            public const int RACEImpulse = 38339996;
            public const int RACEFireAttacker = 64612053;


            // Yubel
            public const int Yubel = 78371393;
            public const int Yubel12 = 31764700;
            public const int Yubel11 = 04779091;
            public const int SpiritOfYubel = 90829280;
            public const int SamsaraDLotus = 62318994;
            public const int GruesumGraveSquirmer = 24215921;

            public const int NightmarePain = 65261141;
            public const int MatureChronicle = 92670749;
            public const int NightmareThrone = 93729896;

            public const int EternalFavourite = 87532344;

            public const int YubelLovingDefender = 4717959;
            public const int PhantomOfYubel = 80453041;
         
            // Unchained
            public const int UnchainedSoulSharvara = 41165831;

            public const int EscapeOfUnchained = 53417695;
            public const int ChamberOfUnchained = 80801743;

            public const int UnchainedSoulRage = 67680512;
            public const int UnchainedSoulAnguish = 93084621;
            public const int UnchainedSoulAbomination = 29479256;
            public const int UnchainedSoulYama = 24269961;


            // Branded
            public const int AlbionTheShroudedDragon = 25451383;
            public const int AluberDespia = 62962630;
            public const int FallenOfAlbaz = 68468459;
            public const int BlazingCartesia = 95515789;
            public const int GuidingQuem = 45883110;
            public const int TriBrigadeMercourier = 19096726;

            public const int BrandedLost = 18973184;
            public const int BrandedFusion = 44362883;
            public const int BrandedInHighSpirits = 29948294;
            public const int BrandedInRed = 82738008;
            public const int BrandedOpening = 36637374;

            public const int BrandedRetribution = 17751597;
            public const int BrightestBlazingBranded = 19271881;

            public const int AlbionTheSanctifireDragon = 38811586;
            public const int BorreloadFuriousDragon = 92892239;
            public const int MirrorJadeTheIcebladeDragon = 44146295;
            public const int PredaplantDRagostapelia = 69946549;
            public const int LubellionSearingDragon = 70534340;
            public const int DespianQuaeritis = 72272462;
            public const int GranguignolDuskDragon = 2415933;
            public const int TitanikladAshDragon = 41373230;
            public const int AlbionTheBrandedDragon = 87746184;
            public const int RindbrummStrikingDragon = 51409648;

            //Shaddoll
            public const int ShadollDragon = 77723643;
            // Runick
            public const int RunickGoldenDroplet = 20618850;
            public const int RunickFreezingCurse = 30430448;
            public const int RunickTip = 31562086;
            public const int RunickDispelling = 66712905;
            public const int RunickSlumber = 67835547;
            public const int RunickFlashingFire = 68957034;
            public const int RunickSmitingStorm = 93229151;
            public const int RunickDestruction = 94445733;
            public const int RunickFountain = 92107604;

            public const int SleipnirRunick = 74659582;
            public const int FrekiRunick = 47219274;
            public const int GeriRunick = 28373620;
            public const int MuninRunick = 92385016;
            public const int HuginRunick = 55990317;


            // Stun
            public const int MajestyFiend = 33746252;
            public const int AmanoIwato = 32181268;
            public const int InterdimensionalMatterTransolcator = 60238002;
            public const int MessengerOfPeace = 44656491;
            public const int DimensonalFissure = 81674782;
        }

    }
}
