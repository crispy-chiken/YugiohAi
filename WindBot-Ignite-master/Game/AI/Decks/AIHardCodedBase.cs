using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

using static WindBot.NEAT;
using System.Linq;
using System;

namespace WindBot.Game.AI.Decks
{
    public class AIHardCodedBase : DefaultExecutor
    {
        protected int materialSelected = 0; // Used to count how many matrials used for summoning, resets on call of SetMain();
        protected int chainLinkCount = 0;
        protected Stack<int> playerChainIndex = new Stack<int>();
        protected bool isChainResolving = false;
        protected int winResult = -1;

        protected bool postSide = false;

        protected List<PreviousAction> previousActions = new List<PreviousAction>();
        protected List<PreviousAction> previousActionsEnemy = new List<PreviousAction>();
        protected List<string> used = new List<string>();
        protected List<int> usedEnemy = new List<int>();
        protected List<int> seenCards = new List<int>();

        // Try and take out dangerous targets
        public int[] MONSTER_FIELD_TARGETS = {
            CardId.SnakeEyeFlamberge,
            CardId.PromethianPrincess,
            CardId.SalamangreatRagingPhoenix,
            CardId.AccesscodeTalker,
            CardId.WorldseadragonZealantis,
            CardId.FiendsmithSequentia,
            CardId.Apollusa,
            CardId.PhantomOfYubel,
            CardId.LovelyLabrynth,
            CardId.MirrorJadeTheIcebladeDragon,
            CardId.MajestyFiend,
            CardId.PredaplantDRagostapelia,
            CardId.BorreloadFuriousDragon
        };

        public int[] DONT_DESTROY =
        {
            CardId.GeriRunick,
            CardId.SnakeEyeFlamberge,
            CardId.SnakeEyePoplar,
        };

        public int[] SPELL_FIELD_TARGETS =
        {
            CardId.SkillDrain,
            CardId.GozenMatch,
            CardId.RivalyOfWarlords,
            CardId.ThereCanBeOnlyOne,
            CardId.RunickFountain,
            CardId.GraveOfTheSuperAncient,
            CardId.SynchroZone,
            CardId.SangenSummoning,
            CardId.BrandedLost,
            CardId.MessengerOfPeace,
            CardId.DimensonalFissure,
            CardId.AntiSpellFragrance,
            CardId.DivineTempleSnakeEyes,
        };

        // Effect negate
        public int[] reactiveEnemyTurn =
        {
                CardId.SnakeEyeAsh,
                CardId.DiabellstarBlackWitch,
                CardId.SnakeEyePoplar,
                CardId.SnakeEyeOak,
                CardId.PromethianPrincess,
                CardId.Apollusa,
        };

        public int[] protactiveEnemyTurn =
        {
                CardId.AccesscodeTalker,
        };

        public int[] proactivePlayerTurn =
        {
            CardId.AmanoIwato,
            CardId.MajestyFiend
        };

        public int[] reactivePlayerTurn =
        {
                CardId.Apollusa,
                CardId.IPMasquerena,
        };

        public int[] faceupSpellTrapNegate =
        {
            CardId.HeatWave,
            CardId.RunickFountain,
            CardId.RunickDestruction,
            CardId.DarumaCannon,
            CardId.SimultaneousCannon,
            CardId.EvenlyMatched,
            CardId.DarkRulerNoMore,
            CardId.ForbiddenDroplet
        };

        public int[] dontUseAsMaterial =
        {
            CardId.AccesscodeTalker,
            CardId.WorldseadragonZealantis,
            CardId.UnderworldGoddess,
            CardId.Apollusa,
            CardId.SalamangreatRagingPhoenix,
            CardId.FiendsmithDiesIrae,
            CardId.PhantomOfYubel,
            CardId.SalamangreatRagingPhoenix,
            CardId.AccesscodeTalker,
            CardId.SnakeEyeFlamberge
        };


        public enum ActivatedEffect
        {
            None = 0x0,
            First = 0x1,
            Second = 0x2,
            Third = 0x4
        }

        public enum Archetypes
        {
            Unknown,
            SnakeEyes,
            Labrynth,
            Branded,
            Tenpai,
            Yubel,
            VoicelessVoice,
            Runick
        }

        public class PreviousAction
        {
            public ExecutorType type;
            public long cardId;
            public long description;
        }

        protected class CardId
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
            public const int Kuriphoton = 35112613;
            public const int LordOfHeavelyPrison = 09822220;
            public const int LavaGolemn = 00102380;
            public const int Pankratops = 82385847;
            public const int SphereMode = 10000080;
            public const int DDCrow = 24508238;

            // Generic Spells
            public const int Bonfire = 85106525;
            public const int CrossoutDesignator = 65681983;
            public const int TripleTacticsTalent = 25311006;
            public const int OneForOne = 2295440;
            public const int CalledByTheGrave = 24224830;
            public const int FeatherDuster = 18144507;
            public const int LightningStorm = 14532163;
            public const int Terraforming = 73628505;
            public const int ForbiddenDroplet = 24299458;
            public const int CosmicCyclone = 8267140;
            public const int HeatWave = 45141013;
            public const int PotOfExtravagance = 49238328;
            public const int PotofProsperity = 84211599;
            public const int PotOfDuality = 98645731;
            public const int PotOfDesires = 35261759;
            public const int DarkRulerNoMore = 54693926;
            public const int SuperPoly = 48130397;
            public const int InstantFusion = 01845204;
            public const int UpstartGoblin = 70368879;
            public const int ChickenGame = 67616300;
            public const int AllureOfDarkness = 01475311;
            public const int FoolishBurial = 81439173;
            public const int GoldSarc = 75500286;
            public const int TripleTacticsThrust = 35269904;
            public const int SnatchSteal = 45986603;
            public const int ChangeOfHeart = 04031928;
            public const int BookOfEclipse = 35480699;
            public const int CardOfDemise = 59750328;

            // Generic Traps
            public const int InfiniteImpermanence = 10045474;
            public const int AntiSpellFragrance = 58921041;
            public const int SkillDrain = 82732705;
            public const int DimensionalBarrier = 83326048;
            public const int IceDragonPrison = 20899496;
            public const int SimultaneousCannon = 25096909;
            public const int DarumaCannon = 30748475;
            public const int GiganticThundercross = 34047456;
            public const int TorrentialTribute = 53582587;
            public const int TerrorsOverroot = 63086455;
            public const int LostWind = 74003290;
            public const int TrapTrick = 80101899;
            public const int TerrorsAfterroot = 85698115;
            public const int CompulsoryEvac = 94192409;
            public const int TransactionRollback = 06351147;
            public const int BlackGoat = 49299410;
            public const int RiseToFullHeight = 19254117;
            public const int RivalyOfWarlords = 90846359;
            public const int DifferentDimensionGround = 3184916;
            public const int EradicatorVirus = 5474237;
            public const int ThereCanBeOnlyOne = 24207889;
            public const int GozenMatch = 53334471;
            public const int SynchroZone = 60306277;
            public const int EvenlyMatched = 1569423;
            public const int SolemnJudgment = 41420027;
            public const int GraveOfTheSuperAncient = 83266092;
            public const int FusionDuplication = 43331750;
            public const int SolemnStrike = 40605147;

            // Generic Synchro
            public const int BlackRoseMoonlightDragon = 33698022;
            public const int BlackroseDragon = 73580471;
            public const int UltimayaTzolkin = 1686814;
            public const int CrystalWingSynchroDragon = 50954680;
            public const int KuibeltTheBladeDragon = 87837090;
            public const int AncientFairyDragon = 25862681;
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
            public const int RelinquishdAnima = 94259633;
            public const int LinkSpider = 98978921;
            public const int AccesscodeTalker = 86066372;
            public const int SalamangreatRagingPhoenix = 57134592;
            public const int KnightmarePhoenix = 2857636;
            public const int PromethianPrincess = 2772337;
            public const int HiitaCharmerAblaze = 48815792;
            public const int DharcCharmerGloomy = 8264361;
            public const int IPMasquerena = 65741786;
            public const int SPLittleKnight = 29301450;
            public const int WorldseadragonZealantis = 45112597;
            public const int SeleneQueenofMasterMagicians = 45819647;
            public const int Apollusa = 4280259;
            public const int UnderworldGoddess = 98127546;
            public const int HieraticSealsOfSpheres = 24361622;
            public const int MoonOfTheClosedHeaven = 71818935;
            public const int Muckracker = 71607202;


            // Chimera
            public const int BerfometKingPhantomBeast = 69601012;
            public const int ChimeraKingPhantomBeast = 01269875;

            // Snake Eyes
            public const int SnakeEyeAsh = 9674034;
            public const int SnakeEyeFlamberge = 48452496;
            public const int SnakeEyePoplar = 90241276;
            public const int SnakeEyeDiabellstar = 27260347;
            public const int SnakeEyeOak = 45663742;
            public const int DiabellstarBlackWitch = 72270339;

            public const int WANTEDSinfulSpoils = 80845034;
            public const int OriginalSinfulSpoilsSnakeEyes = 89023486;
            public const int DivineTempleSnakeEyes = 53639887;

            public const int SinfulSpoilSilvera = 38511382;

            // Tenpai
            public const int TenpaiPaidra = 39931513;
            public const int TenpaiChundra = 91810826;
            public const int TenpaiFadra = 65326118;
            public const int TenpaiGenroku = 23657016;
            public const int PokiDraco = 08175346;

            public const int SangenKaimen = 66730191;
            public const int SangenSummoning = 30336082;

            public const int SangenpaiTranscendentDragion = 18969888;
            public const int SangenpaiBidentDragion = 82570174;
            public const int TridentDragion = 39402797;

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

            public const int NecroqiopPrincess =  93860227;


            // Fabled
            public const int FabledLurrie = 97651498;
            

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
            public const int DogmatikaMaximus = 95679145;
            public const int DogmatikaEcclesia = 60303688;

            public const int NadirServant = 01984618;

            public const int DogmatikaPunishment = 82956214;
            
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
            // Sacred Beast
            public const int DarkBeckoningBeast = 81034083;
            public const int ChaosSummoningBeast = 27439792;
            public const int OpeningOfTheSpritGates = 80312545;

            // Unchained
            public const int UnchainedSoulSharvara = 41165831;

            public const int EscapeOfUnchained = 53417695;
            public const int ChamberOfUnchained = 80801743;

            public const int UnchainedSoulRage = 67680512;
            public const int UnchainedSoulAnguish = 93084621;
            public const int UnchainedSoulAbomination = 29479256;
            public const int UnchainedSoulYama = 24269961;

            // Tribrigade
            public const int TriBrigadeBucephalus = 10019086;

            // Branded
            public const int AlbionTheShroudedDragon = 25451383;
            public const int AluberDespia = 62962630;
            public const int FallenOfAlbaz = 68468459;
            public const int SpringansKitt = 45484331;
            public const int BlazingCartesia = 95515789;
            public const int GuidingQuem = 45883110;
            public const int TriBrigadeMercourier = 19096726;
            public const int DespianTragedy = 36577931;

            public const int BrandedLost = 18973184;
            public const int BrandedFusion = 44362883;
            public const int FusionDeployment = 06498706;
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
            public const int AlbaLenatusAbyssDragon = 03410461;
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

            public const int CardScanner = 77066768;
            public const int DrawMuscle = 41367003;

            // Gimmick Puppet
            public const int GimmickPuppetNightmare = 55204071;

            // Stun
            public const int MajestyFiend = 33746252;
            public const int AmanoIwato = 32181268;
            public const int InterdimensionalMatterTransolcator = 60238002;
            public const int MessengerOfPeace = 44656491;
            public const int OneDayOfPeace = 33782437;
            public const int TimeTearingMorganite = 19403423;
            public const int DimensonalFissure = 81674782;
            public const int BattleFader = 19665973;
        }

        public AIHardCodedBase(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Reactive
            AddExecutor(ExecutorType.Activate, CardId.ChickenGame, DefaultChickenGame);
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, FaceUpEffectNegate);
            AddExecutor(ExecutorType.Activate, CardId.GhostMourner, FaceUpEffectNegate);
            AddExecutor(ExecutorType.Activate, CardId.GhostOgre, GhostOgreActivate);
            AddExecutor(ExecutorType.Activate, CardId.GhostBelle, DefaultGhostBelleAndHauntedMansion);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, FaceUpEffectNegate);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, CardId.DrollnLockBird, DrollActivate);
            AddExecutor(ExecutorType.Activate, CardId.DDCrow, DDCrowActivate);
            AddExecutor(ExecutorType.Activate, CardId.Nibiru);
            AddExecutor(ExecutorType.Activate, CardId.FantasticalPhantazmay);
            AddExecutor(ExecutorType.Activate, CardId.DimensionShifter);
            AddExecutor(ExecutorType.Activate, CardId.CosmicCyclone, CosmicActivate);
            AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, DefaultSolemnJudgment);

            AddExecutor(ExecutorType.Activate, CardId.SkillDrain);
            AddExecutor(ExecutorType.Activate, CardId.GozenMatch);
            AddExecutor(ExecutorType.Activate, CardId.RivalyOfWarlords);
            AddExecutor(ExecutorType.Activate, CardId.ThereCanBeOnlyOne);
            AddExecutor(ExecutorType.Activate, CardId.AntiSpellFragrance);
            AddExecutor(ExecutorType.Activate, CardId.GraveOfTheSuperAncient);
            AddExecutor(ExecutorType.Activate, CardId.SynchroZone);
            AddExecutor(ExecutorType.Activate, CardId.DifferentDimensionGround);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalBarrier);
        }

        protected List<long> HintMsgForEnemy = new List<long>
        {
            HintMsg.Release, HintMsg.Destroy, HintMsg.Remove, HintMsg.ToGrave, HintMsg.ReturnToHand, HintMsg.ToDeck,
            HintMsg.FusionMaterial, HintMsg.SynchroMaterial, HintMsg.XyzMaterial, HintMsg.LinkMaterial
        };

        protected List<long> HintMsgForDeck = new List<long>
        {
            HintMsg.SpSummon, HintMsg.ToGrave, HintMsg.Remove, HintMsg.AddToHand, HintMsg.FusionMaterial
        };

        protected List<long> HintMsgForSelf = new List<long>
        {
            HintMsg.Equip
        };

        protected List<long> HintMsgForMaterial = new List<long>
        {
            HintMsg.FusionMaterial, HintMsg.SynchroMaterial, HintMsg.XyzMaterial, HintMsg.LinkMaterial, HintMsg.Release
        };

        protected List<long> HintMsgForMaxSelect = new List<long>
        {
            HintMsg.SpSummon, HintMsg.ToGrave, HintMsg.AddToHand, HintMsg.FusionMaterial, HintMsg.Destroy
        };

        // Choose Go first or second
        public override bool OnSelectHand()
        {
            bool choice = true;
            return choice;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();
            previousActions.Clear();
            previousActionsEnemy.Clear();
        }

        public override void SetMain(MainPhase main)
        {
            base.SetMain(main);
            materialSelected = 0;
        }


        public override bool OnSelectYesNo(long desc)
        {
            var option = Util.GetOptionFromDesc(desc);
            var cardId = Util.GetCardIdFromDesc(desc);
            return true;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> _cards, int min, int max, long hint, bool cancelable)
        {
            //if (Duel.Phase == DuelPhase.BattleStart)
            //    return null;
            if (AI.HaveSelectedCards())
                return null;

            IList<ClientCard> selected = new List<ClientCard>();
 
            return SelectMinimum(selected, _cards, min, max, hint);
        }

        public override IList<ClientCard> OnCardSorting(IList<ClientCard> cards)
        {
            return base.OnCardSorting(cards);
        }

        // Do not touch _cards list
        public IList<ClientCard> SelectMinimum(IList<ClientCard> selected, IList<ClientCard> _cards, int min, int max, long hint)
        {
 
            IList<ClientCard> cards = new List<ClientCard>(_cards);
            ClientCard currentCard = GetCurrentCardResolveInChain();

            // Default selection
            if (currentCard != null)
            {
                if (HintMsg.Target == hint)
                {
                    if (Duel.CurrentChain.Count() >= 2)
                    {
                        if (CardId.InfiniteImpermanence == Card.Id ||
                            CardId.EffectVeiler == Card.Id ||
                            CardId.GhostMourner == Card.Id)
                            selected.Add(_cards.FirstOrDefault(x => Duel.CurrentChain.Any(y => y.Equals(x))));
                    }
                    if (CardId.CalledByTheGrave == currentCard.Id)
                    {
                        selected.Add(_cards.Where(x => x.Controller == 1 && x.Location == CardLocation.Grave && Duel.CurrentChain.Any(y => y.IsCode(x.Id))).FirstOrDefault());
                    }
                    int[] GYBanish =
                    {
                        CardId.BystialMagnamhut,
                        CardId.BystialDruiswurm,
                        CardId.BystialSaronir
                    };
                    if (GYBanish.Any(x => x == currentCard.Id))
                        selected.Add(_cards.Where(x => x.Location == CardLocation.Grave && x.HasAttribute(CardAttribute.Dark | CardAttribute.Light) && Duel.ChainTargets.Any(y => y.Equals(x)))
                                            .OrderBy(x => x.Owner == 1 ? 0 : 1)
                                            .FirstOrDefault()
                                     );
                }
                if (CardId.ForbiddenDroplet == currentCard.Id)
                {
                    if (hint == HintMsg.ToGrave)
                    {
                        int count = 0;
                        if (Duel.Player == 1)
                        {
                            foreach (int id in protactiveEnemyTurn)
                               count += Enemy.GetMonsters().Where(x => x.Id == id).Count();

                            foreach (int id in reactiveEnemyTurn)
                                count += Enemy.GetMonsters().Where(x => x.Id == id).Count();

                        }
                        else if (Duel.Player == 0)
                        {
                            foreach (int id in proactivePlayerTurn)
                                count += Enemy.GetMonsters().Where(x => x.Id == id).Count();

                            foreach (int id in reactivePlayerTurn)
                                count += Enemy.GetMonsters().Where(x => x.Id == id).Count();
                        }
                        foreach(var card in Duel.CurrentChain)
                        {
                            if (!_cards.Any(x => x.Equals(card)))
                                continue;

                            if (card.Owner == 0 && card.Location == CardLocation.SpellZone)
                                selected.Add(_cards.Where(x => x.Equals(card)).FirstOrDefault());

                            if (selected.Count >= count)
                                break;
                        }
                    }
                    else
                    {
                        if (Duel.Player == 1)
                        {
                            foreach (int id in protactiveEnemyTurn)
                                selected.Add(_cards.Where(x => x.Id == id).FirstOrDefault());

                            foreach (int id in reactiveEnemyTurn)
                                selected.Add(_cards.Where(x => x.Id == id).FirstOrDefault());

                        }
                        else if (Duel.Player == 0)
                        {
                            foreach (int id in proactivePlayerTurn)
                                selected.Add(_cards.Where(x => x.Id == id).FirstOrDefault());

                            foreach (int id in reactivePlayerTurn)
                                selected.Add(_cards.Where(x => x.Id == id).FirstOrDefault());
                        }
                    }
                }
                else if (CardId.SPLittleKnight == currentCard.Id)
                {
                    _cards.OrderBy(x => MONSTER_FIELD_TARGETS.Any(y => y.Equals(x)) ? 0 : 1)
                          .ThenBy(x => SPELL_FIELD_TARGETS.Any(y => y.Equals(x)) ? 0 : 1)
                          .ThenBy(x => x.Location == CardLocation.MonsterZone ? 0 : 1)
                          .ThenBy(x => x.Location == CardLocation.SpellZone ? 0 : 1);
                }

                #region Fiendsmith Selection
                if (CardId.TheFiendsmith == currentCard.Id)
                {
                    if (hint == HintMsg.AddToHand)
                    {
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.FiendsmithTractus));
                    }
                    // Shuffle into deck
                    if (hint == HintMsg.ToDeck)
                        foreach (var id in FiendsmithShuffleToDeck)
                        {
                            selected.Add(_cards.FirstOrDefault(x => x.Id == id));
                        }
                }
                else if (CardId.FiendsmithLacrimosa == currentCard.Id)
                {
                    if (hint == HintMsg.SpSummon)
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.TheFiendsmith));
                    // Shuffle into deck
                    if (hint == HintMsg.ToDeck)
                        foreach (var id in FiendsmithShuffleToDeck)
                        {
                            selected.Add(_cards.FirstOrDefault(x => x.Id == id));
                        }
                }
                else if (CardId.FiendsmithTractus == currentCard.Id)
                {
                    selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.FabledLurrie));
                }
                else if (CardId.FiendsmithSequentia == currentCard.Id)
                {
                    if (hint == HintMsg.SpSummon)
                    {
                        selected.Add(_cards.Where(x => x.Id == CardId.FiendsmithLacrimosa).FirstOrDefault());
                    }
                }
                else if (CardId.FiendsmithDiesIrae == currentCard.Id)
                {
                    if (Card.Location == CardLocation.Grave) // Send to grave effect
                    {
                        _cards.OrderBy(x => MONSTER_FIELD_TARGETS.Any(y => y.Equals(x)) ? 0 : 1)
                                  .ThenBy(x => SPELL_FIELD_TARGETS.Any(y => y.Equals(x)) ? 0 : 1)
                                  .ThenBy(x => x.Location == CardLocation.MonsterZone ? 0 : 1)
                                  .ThenBy(x => x.Location == CardLocation.SpellZone ? 0 : 1);
                    }
                    else
                    {
                        foreach (int id in faceupSpellTrapNegate)
                            selected.Add(_cards.Where(x => x.Id == id).FirstOrDefault());

                        if (Duel.Player == 1)
                        {
                            foreach (int id in protactiveEnemyTurn)
                                selected.Add(_cards.Where(x => x.Id == id).FirstOrDefault());

                            foreach (int id in reactiveEnemyTurn)
                                selected.Add(_cards.Where(x => x.Id == id).FirstOrDefault());

                        }
                        else if (Duel.Player == 0)
                        {
                            foreach (int id in proactivePlayerTurn)
                                selected.Add(_cards.Where(x => x.Id == id).FirstOrDefault());

                            foreach (int id in reactivePlayerTurn)
                                selected.Add(_cards.Where(x => x.Id == id).FirstOrDefault());
                        }
                    }
                }
                else if (CardId.DDCrow == currentCard.Id)
                {
                    selected.Add(_cards.Where(x => x.Id == DDCrowTargets()).FirstOrDefault());
                }
            }
            else if (hint == HintMsg.FusionMaterial)
            {
                IList<ClientCard> highPriority = new List<ClientCard>();
                IList<ClientCard> lowPriority = new List<ClientCard>();

                int[] highList =
                {
                    CardId.FiendsmithSequentia
                };
                int[] lowList =
                {
                    CardId.TheFiendsmith,
                    CardId.FiendsmithDiesIrae
                };

                _cards = _cards
                    .OrderBy(x => highList.Contains(x.Id) ? 0 : 1)
                    .ThenBy(x => lowList.Contains(x.Id) ? 1 : 0)
                    .ToList();

                // Stop selecting if using low priorty cards
                if (materialSelected > 0 && _cards.Where(x => lowList.Contains(x.Id)).Count() == _cards.Count())
                    return null;

                materialSelected += 1;
            }
            #endregion

            #region Random selection
            // Fill in the remaining with defaults
            if (HintMsgForEnemy.Contains(hint))
            {
                IList<ClientCard> enemyCards = cards.Where(card => card.Controller == 1).ToList();

                // select enemy's card first
                while (enemyCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = enemyCards[Rand.Next(enemyCards.Count)];
                    selected.Add(card);
                    enemyCards.Remove(card);
                    cards.Remove(card);
                }
            }

            if (HintMsgForDeck.Contains(hint))
            {
                IList<ClientCard> deckCards = cards.Where(card => card.Location == CardLocation.Deck).ToList();

                // select deck's card first
                while (deckCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = deckCards[Rand.Next(deckCards.Count)];
                    selected.Add(card);
                    deckCards.Remove(card);
                    cards.Remove(card);
                }
            }

            if (HintMsgForSelf.Contains(hint))
            {
                IList<ClientCard> botCards = cards.Where(card => card.Controller == 0).ToList();

                // select bot's card first
                while (botCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = botCards[Rand.Next(botCards.Count)];
                    selected.Add(card);
                    botCards.Remove(card);
                    cards.Remove(card);
                }
            }

            if (HintMsgForMaterial.Contains(hint))
            {
                IList<ClientCard> materials = cards.OrderBy(card => card.Attack).ToList();

                // select low attack first
                while (materials.Count > 0 && selected.Count < min)
                {
                    ClientCard card = materials[0];
                    selected.Add(card);
                    materials.Remove(card);
                    cards.Remove(card);
                }
            }

            if (HintMsgForMaxSelect.Contains(hint))
            {
                // select max cards
                while (selected.Count < max)
                {
                    ClientCard card = cards[Rand.Next(cards.Count)];
                    selected.Add(card);
                    cards.Remove(card);
                }
            }
            #endregion

            // Clear null values
            selected = selected.Where(item => item != null).ToList();
            // Remove duplicates
            selected = selected.Distinct().ToList();

            cards = new List<ClientCard>(_cards);
            // select random cards
            while (selected.Count < min)
            {
                ClientCard card = cards[0];//cards[Rand.Next(cards.Count)];
                if (!selected.Contains(card))
                    selected.Add(card);
                cards.Remove(card);
            }

            // Don't over select
            if (selected.Count > max)
            {
                selected = selected.Take(max).ToList();
            }

            // Add to previousActions
            foreach (var card in selected)
            {
                if (!used.Contains(card.Name))
                    used.Add(card.Name);
                previousActions.Add(new PreviousAction() { cardId = card.Id, type = ExecutorType.Select, description = hint });
            }

            return selected;
        }

        // Called when a chain is about to happen
        public override void SetChain(IList<ClientCard> cards, IList<long> descs, bool forced)
        {
            base.SetChain(cards, descs, forced);
        }

        // As Chain activates
        public override void OnChaining(int player, ClientCard card)
        {
            chainLinkCount += 1;
            if (player == 1)
            {
                if (!usedEnemy.Contains(card.Id))
                    usedEnemy.Add(card.Id);
                if (!seenCards.Contains(card.Id))
                    seenCards.Add(card.Id);
                previousActionsEnemy.Add(new PreviousAction() { cardId = card.Id, type = ExecutorType.Activate });
            }
            else
            {
                playerChainIndex.Push(chainLinkCount);
            }
            base.OnChaining(player, card);
        }

        public override void OnChainSolving()
        {
            isChainResolving = true;
        }

        // As a Chain link resolves
        public override void OnChainSolved()
        {
            if (playerChainIndex.Count > 0)
                playerChainIndex.Pop();
        }

        public override void OnChainEnd()
        {
            isChainResolving = false;
            base.OnChainEnd();
            if (chainLinkCount != 0)
                chainLinkCount = 0;
        }

        public override int OnSelectOption(IList<long> options)
        {
            int index = 0;
            foreach(long desc in options)
            {
                var option = Util.GetOptionFromDesc(desc);
                var cardId = Util.GetCardIdFromDesc(desc);

                if (CardId.TripleTacticsTalent == cardId)
                {
                    if (Duel.Turn > 1)
                    {
                        if (option == 1)
                            return index; // steal
                    }
                    else if (option == 0) // Draw 2
                        return index;
                }
                else if (CardId.FiendsmithLacrimosa == cardId)
                {
                    // special summon = 1, add to hand = 0
                    return 1;
                }
                index++;
            }

            return 0;
        }

        public override int OnAnnounceCard(IList<int> avail)
        {
            if (Util.GetLastChainCard().Id == CardId.CrossoutDesignator)
            {
                if (Duel.CurrentChain.Count >= 2)
                {
                    foreach(var card in Duel.CurrentChain)
                        if (avail.Contains(card.Id) && card.Controller == 1)
                            return (card.Id);
                    foreach (var card in Enemy.GetSpells())
                        if (avail.Contains(card.Id))
                            return card.Id;
                    foreach (var card in Enemy.GetMonsters())
                        if (avail.Contains(card.Id))
                            return card.Id;

                }
            }
            return base.OnAnnounceCard(avail);
        }

        public override void OnPostActivate(bool activate)
        {
            if (!activate)
                return;
            if (!used.Contains(Card.Name))
                used.Add(Card.Name);
            previousActions.Add(new PreviousAction()
            {
                cardId = Card.Id,
                type = Type,
                description = ActivateDescription
            });
        }

        public override void OnWin(int result, List<int> _deck, List<int> _extra, List<int> _side, Dictionary<int, string> _idToName)
        {
            winResult = result;

            List<SQLComm.CardQuant> deckQuant = new List<SQLComm.CardQuant>();
            List<int> deck = new List<int>(_deck);
            List<int> extra = new List<int>(_extra);
            List<int> side = new List<int>(_side);
            while (deck.Count > 0)
            {
                int id = deck[0];
                int quant = deck.Where(x => x == id).Count();
                deck.RemoveAll(x => x == id);
                deckQuant.Add(new SQLComm.CardQuant() { Name = _idToName[id], Id = id.ToString(), Quant = quant, Location = 0 });
            }
            while (extra.Count > 0)
            {
                int id = extra[0];
                int quant = extra.Where(x => x == id).Count();
                extra.RemoveAll(x => x == id);
                deckQuant.Add(new SQLComm.CardQuant() { Name = _idToName[id], Id = id.ToString(), Quant = quant, Location = 1 });
            }
            /*while (side.Count > 0)
            {
                int id = side[0];
                int quant = side.Where(x => x == id).Count();
                side.RemoveAll(x => x == id);
                deckQuant.Add(new SQLComm.CardQuant() { Name = _idToName[id], Id = id.ToString(), Quant = quant, Location = 2 });
            }*/


            SQLComm.SavePlayedCards(Duel.IsFirst, postSide, result, used, deckQuant);

            used.Clear();
            usedEnemy.Clear();


            postSide = false;
        }


        #region Generic Actions

        public bool DefaultNegate()
        {
            if (Duel.LastChainPlayer == 0)
                return false;

            // Tenpai field spell
            ClientCard last = Util.GetLastChainCard();

            if (last == null)
                return false;

            bool isTenpaiType = (last.HasAttribute(CardAttribute.Fire) && last.HasRace(CardRace.Dragon) && last.Location == CardLocation.MonsterZone);
            if (Enemy.HasInSpellZone(CardId.SangenSummoning) && isTenpaiType && Duel.Phase == DuelPhase.Main1 && Duel.Player == 1)
                return false;


            return true;
        }

        public bool FaceUpEffectNegate()
        {
            
            // Apo negate
            if (Duel.Player == 0 && Enemy.GetMonsters().Where(x => x.Id == CardId.Apollusa && x.Attack >= 800 && !Util.IsChainTarget(x)).Any())
                return true;


            foreach (int id in protactiveEnemyTurn)
                if (Duel.Player == 1 && Enemy.GetMonsters().Where(x => x.Id == id && !x.IsDisabled()).Any())
                    return true;

            foreach (int id in proactivePlayerTurn)
                if (Duel.Player == 0 && Enemy.GetMonsters().Where(x => x.Id == id && !x.IsDisabled()).Any())
                    return true;

            if (!DefaultNegate())
                return false;

            if (Duel.Player == 1)
                foreach (int id in reactiveEnemyTurn)
                    if (Duel.CurrentChain.Where(x => x.Id == id && !x.IsDisabled() && !Util.IsChainTarget(x)).Any())
                        return true;

            if (Duel.Player == 0)
                foreach (int id in reactivePlayerTurn)
                    if (Duel.CurrentChain.Where(x => x.Id == id && !x.IsDisabled() && !Util.IsChainTarget(x)).Any())
                        return true;

            return false;
        }

        public bool FaceUpSpellNegate()
        {
            foreach (int id in faceupSpellTrapNegate)
                if (Enemy.GetSpells().Where(x => x.Id == id && !x.IsDisabled()).Any())
                    return true;
            return false;
        }

        public bool GoToBattlePhase()
        {
            if (Bot.HasInHand(CardId.EvenlyMatched) && Bot.IsFieldEmpty())
                return true;
            return false;
        }

        #endregion


        #region Generic Monsters
        public bool AshBlossomActivate()
        {
            if (!DefaultNegate())
                return false;

            int[] ashTargets =
            {
                CardId.OriginalSinfulSpoilsSnakeEyes,
                CardId.SnakeEyeAsh //Effect 2
            };

            if (Duel.CurrentChain.LastOrDefault().IsCode(ashTargets))
                return true;

            return false;
        }

        public bool GhostOgreActivate()
        {
            if (!DefaultNegate())
                return false;

            int[] dont =
            {
                CardId.SnakeEyeFlamberge,
                CardId.SnakeEyePoplar
            };

            return true;
        }

        public bool DrollActivate()
        {
            if (Duel.Player == 0)
                return false;
            return true;
        }

        public bool ApollusaSummon()
        {
            if (Bot.GetLinkMaterialWorth(dontUseAsMaterial) >= 4)
                return true;
            return true;
        }

        public bool ApollusaActivate()
        {
            if (Duel.LastChainPlayer == 1)
                return true;
            return false;
        }

        public bool TyphonSummon()
        {
            return false;
        }

        public bool TyphonActivate()
        {
            return false;
        }

        public bool BystialActivate()
        {
            if (Duel.ChainTargets.Where(x => x.Location == CardLocation.Grave && x.HasAttribute(CardAttribute.Dark | CardAttribute.Light)).Any())
                return true;
            if (Duel.Phase == DuelPhase.End)
                return true;
            if (Card.Location != CardLocation.Hand)
                return true;
            return false;
        }

        public bool DDCrowActivate()
        {
            if (Duel.CurrentChain.ContainsCardWithId(CardId.DDCrow))
                return false;
            return DDCrowTargets() != -1;
        }

        public int DDCrowTargets()
        {
            if (Duel.ChainTargets.Any(x => x.Location == CardLocation.Grave))
                return Duel.ChainTargets.FirstOrDefault(x => x.Location == CardLocation.Grave).Id;

            if (Duel.CurrentChain.Any(x => x.Id == CardId.SnakeEyeFlamberge && x.Location == CardLocation.Grave) && Enemy.Graveyard.Where(x => x.Level == 1 && x.HasAttribute(CardAttribute.Fire)).Count() == 2)
                return Enemy.Graveyard.FirstOrDefault(x => x.Level == 1 && x.HasAttribute(CardAttribute.Fire)).Id;

            if (Duel.Player == 0 && Enemy.Graveyard.ContainsCardWithId(CardId.PromethianPrincess))
                return CardId.PromethianPrincess;

            if (Duel.Player == 0 && Enemy.Graveyard.ContainsCardWithId(CardId.SalamangreatRagingPhoenix))
                return CardId.SalamangreatRagingPhoenix;

            if (Duel.Player == 0 && Enemy.Graveyard.ContainsCardWithId(CardId.UnchainedSoulYama))
                return CardId.UnchainedSoulYama;

            if (Duel.CurrentChain.Any(x => x.Id == CardId.TheFiendsmith && x.Location == CardLocation.Grave))
                return CardId.TheFiendsmith;

            if (Duel.CurrentChain.Any(x => x.Id == CardId.SangenpaiBidentDragion && x.Location == CardLocation.Grave))
                return CardId.SangenpaiBidentDragion;
            if (Duel.CurrentChain.Any(x => x.Id == CardId.SangenpaiTranscendentDragion && x.Location == CardLocation.Grave))
                return CardId.SangenpaiTranscendentDragion;

            return -1;
        }

        #endregion


        #region Generic Spells
        public bool BonfireActivate()
        {
            return true;
        }

        public bool CrossoutActivate()
        {
            if (Duel.LastChainPlayer == 0)
                return false;
            if (Duel.CurrentChain.Count() <= 0)
                return false;
            if (Bot.Deck.Where(x => x.Name == Duel.CurrentChain.LastOrDefault()?.Name && x.Controller == 1).Any())
                return true;
            if (Bot.Deck.Where(x => x.Name == Enemy.GetSpells().LastOrDefault()?.Name).Any())
                return true;
            if (Bot.Deck.Where(x => x.Name == Enemy.GetMonsters().LastOrDefault()?.Name).Any())
                return true;
            return false;
        }

        public bool CalledByActivate()
        {
            if (Duel.CurrentChain.Where(x => x.Controller == 1 && x.Location == CardLocation.Grave && x.HasType(CardType.Monster)).Any())
                return true;
            return false;
        }

        public bool CosmicActivate()
        {
            if (Duel.CurrentChain.Any(x => SPELL_FIELD_TARGETS.Contains(x.Id) && !Duel.ChainTargets.Any()))
                return true;
            if (Duel.Player == 1 && Enemy.GetSpells().Any(x => x.HasPosition(CardPosition.FaceDown)))
                return true;
            if (Duel.Player == 0 && Enemy.GetSpells().Count(x => x.HasPosition(CardPosition.FaceDown)) == 1)
                return true;

            return false;
        }

        public bool DropletActivate()
        {
            return FaceUpEffectNegate();
        }
        #endregion


        #region Generic Traps
        #endregion

        #region Fiendsmith

        public int[] FiendsmithShuffleToDeck =
        {
            CardId.FiendsmithLacrimosa,
            CardId.FiendsmithDiesIrae,
            CardId.FiendsmithRequiem,
            CardId.FiendsmithSequentia,
            CardId.FabledLurrie,
            CardId.MoonOfTheClosedHeaven
        };

        public bool TheFiendsmithActivate()
        {
            if (Card.Location == CardLocation.Hand)
                return true;
            if (Card.Location == CardLocation.Grave)
                return true;
            if (Card.Location == CardLocation.MonsterZone && Enemy.GetMonsterCount() > 0)
                return true;
            return false;
        }

        public bool FiendsmithTractusActivate()
        {
            if (Card.Location != CardLocation.Grave)
                return true;
            else if (Bot.GetMonsters().Count(x => x.HasAttribute(CardAttribute.Light) && x.HasRace(CardRace.Fiend)) >= 3)
                return true;
            return false;
        }

        public bool FiendsmithLacrimosaActivate()
        {
            if (Bot.HasInMonstersZone(CardId.FiendsmithDiesIrae))
                return false;
            return true;
        }

        public bool FiendsmithDiesIraeActivate()
        {
            if (Card.Location == CardLocation.Grave)
                return true;
            return FaceUpEffectNegate() || FaceUpSpellNegate();
        }

        public bool FiendsmithRequiemActivate()
        {
            var option = Util.GetOptionFromDesc(ActivateDescription);
            // Only equip from gy
            if (option == 1 && Card.Location != CardLocation.Grave)
                return false;
            return true;
        }

        public bool FiendsmithSequentiaActivate()
        {
            var option = Util.GetOptionFromDesc(ActivateDescription);
            // Only equip from gy
            if (option == 1 && Card.Location != CardLocation.Grave)
                return false;
            return true;
        }

        public bool FiendsmithSequentiaSummon()
        {
            if (Bot.GetLinkMaterialWorth(dontUseAsMaterial) < 2)
                return false;
            if (HasPerformedPreviously(CardId.FiendsmithSequentia))
                return false;
            return true;
        }

        public bool FiendSmithRequiemSummon()
        {
            int[] dontuse =
            {
                CardId.FiendsmithDiesIrae,
                CardId.FiendsmithLacrimosa
            };
            if (Bot.GetLinkMaterialWorth(dontUseAsMaterial.Concat(dontuse).ToArray()) < 1)
                return false;
            if (HasPerformedPreviously(CardId.FiendsmithRequiem))
                return false;
            return true;
        }

        public bool NecroquipPrincessSummon()
        {
            if (Bot.HasInMonstersZone(CardId.TheFiendsmith))
                return true;
            return false;
        }

        #endregion


        #region Util
        // check basic action
        protected bool HasPerformedPreviously(ExecutorType action)
        {
            return previousActions.Where(x => x.type == action).Any();
        }

        protected bool HasPerformedPreviously(long cardId)
        {
            return previousActions.Any(x => x.cardId == cardId);
        }

        protected bool HasPerformedPreviously(long cardId, ExecutorType action)
        {
            return previousActions.Where(x => x.cardId == cardId && x.type == action).Any();
        }

        // Check activation
        protected bool HasPerformedPreviously(long cardId, int option)
        {
            long hint = Util.GetStringId(cardId, option);


            // Specific fixes
            if (cardId == CardId.SnakeEyeAsh && option == 0)
                hint = 62205969853120512;


            return previousActions.Where(x => x.cardId == cardId && x.description == hint && x.type == ExecutorType.Activate).Any();
        }

        protected bool HasPerformedPreviously(long cardId, ExecutorType action, long option)
        {
            return previousActions.Where(x => x.cardId == cardId && x.type == action && x.description == option).Any();
        }

        // Returns the card that is currently resolving that you need to resolve
        protected ClientCard GetCurrentCardResolveInChain()
        {
            if (isChainResolving)
            {
                if (playerChainIndex.Count() > 0)
                {
                    var index = playerChainIndex.Peek();
                    return Duel.CurrentChain[index - 1];
                }
            }
            else
            {
                return Util.GetLastChainCard();
            }
            return null;
        }

        protected Archetypes GetEnemyDeckType()
        {
            int[] SnakeEyes =
            {
                CardId.SnakeEyeAsh,
                CardId.SnakeEyeOak,
                CardId.SnakeEyePoplar,
                CardId.SnakeEyeDiabellstar,
                CardId.SnakeEyeFlamberge,
                CardId.OriginalSinfulSpoilsSnakeEyes,
                CardId.DiabellstarBlackWitch,
                CardId.WANTEDSinfulSpoils,
                CardId.DivineTempleSnakeEyes
            };

            if (SnakeEyes.Any(x => seenCards.Contains(x)))
                return Archetypes.SnakeEyes;

            int[] Labrynth =
            {

            };

            int[] Branded =
            {

            };

            int[] Tenpai =
            {
                CardId.TenpaiChundra,
                CardId.TenpaiFadra,
                CardId.TenpaiPaidra,
                CardId.SangenKaimen,
                CardId.SangenSummoning,
            };
            if (Tenpai.Any(x => seenCards.Contains(x)))
                return Archetypes.Tenpai;

            return Archetypes.Unknown;
        }

        protected Archetypes GetPlayerDeckType()
        {
            int[] SnakeEyes =
{
                CardId.SnakeEyeAsh,
                CardId.SnakeEyeOak,
                CardId.SnakeEyePoplar,
                CardId.SnakeEyeDiabellstar,
                CardId.SnakeEyeFlamberge,
                CardId.OriginalSinfulSpoilsSnakeEyes,
                CardId.DiabellstarBlackWitch,
                CardId.WANTEDSinfulSpoils,
                CardId.DivineTempleSnakeEyes
            };

            // if (SnakeEyes.Any(x => Util.Bot.Deck.Any(y => y.Alias == x)))
            //     return Archetypes.SnakeEyes;

            int[] Labrynth =
            {

            };

            int[] Branded =
            {

            };

            int[] Tenpai =
            {
                CardId.TenpaiChundra,
                CardId.TenpaiFadra,
                CardId.TenpaiPaidra,
                CardId.SangenKaimen,
                CardId.SangenSummoning,
            };
            // if (Tenpai.Any(x => Util.Bot.Deck.Any(y => y.Alias == x)))
            //     return Archetypes.Tenpai;

            return Archetypes.Unknown;
        }

        /// <summary>
        /// Add as many of the given cards from the main/side list to the cards to add list
        /// </summary>
        /// <param name="toAddTo">The list to add to</param>
        /// <param name="cardsToAdd">Cards you want to add</param>
        /// <param name="pool">the pool of cards to take from</param>
        protected void AddCardsToList(IList<int> toAddTo, IList<int> pool, int limit, int[] cardsToAdd = null)
        {
            if (cardsToAdd != null)
            {
                foreach (int card in cardsToAdd)
                {
                    if (toAddTo.Count() >= limit)
                        break;
                    if (pool.Contains(card))
                    {
                        toAddTo.Add(card);
                        pool.Remove(card);
                    }
                }
            }
            else
            {
                while (toAddTo.Count() < limit && pool.Count() > 0)
                {
                    var card = pool.ElementAt(0);
                    pool.RemoveAt(0);
                    toAddTo.Add(card);
                }
            }
        }

        protected void MoveTo(IList<int> from, IList<int> to, IList<int> cards = null, int lastx = 0)
        {
            if (cards != null)
            {
                foreach (var card in cards)
                {
                    if (from.Any(x => x == card))
                    {
                        from.Remove(card);
                        to.Add(card);
                    }
                }
            }
            else
            {
                for(var i = from.Count - 1; i >= from.Count - 1 - lastx; i --)
                {
                    int card = from[i];
                    from.RemoveAt(i);
                    to.Add(card);
                }
            }
        }
        #endregion
    }
}
