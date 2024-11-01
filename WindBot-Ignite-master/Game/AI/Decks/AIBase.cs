using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using static WindBot.MCST;
using static WindBot.NEAT;
using System.Linq;
using System;
using static WindBot.AbstractAIEngine;
using System.Diagnostics;

namespace WindBot.Game.AI.Decks
{
    [Deck("AIBase", "AI_Base")]
    public class AIBase : DefaultExecutor
    {
        protected AbstractAIEngine aIEngine;

        protected int materialSelected = 0; // Used to count how many matrials used for summoning, resets on call of SetMain();
        protected int chainLinkCount = 0;
        protected Stack<int> playerChainIndex = new Stack<int>();
        protected bool isChainResolving = false;
        protected int winResult = -1;

        protected bool postSide = false;

        protected List<PreviousAction> previousActions = new List<PreviousAction>();
        protected List<PreviousAction> previousActionsEnemy = new List<PreviousAction>();
        protected List<PreviousAction> lingeringTurnActions = new List<PreviousAction>();
        protected List<PreviousAction> lingeringPrevTurnActions = new List<PreviousAction>();
        protected List<PreviousAction> lingeringTurnActionsEnemy = new List<PreviousAction>();
        protected List<PreviousAction> lingeringPrevTurnActionsEnemy = new List<PreviousAction>();
        protected List<string> used = new List<string>();
        protected List<int> usedEnemy = new List<int>();
        protected List<int> seenCards = new List<int>();

        public enum ActivatedEffect
        {
            None = 0x0,
            First = 0x1,
            Second = 0x2,
            Third = 0x4
        }

        public class PreviousAction
        {
            public ExecutorType type;
            public long cardId;
            public long description;

            public override string ToString()
            {
                return cardId.ToString() + ";" + type.ToString() + ";" + description.ToString();
            }
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

            public const int NecroqiopPrincess = 93860227;


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


        public AIBase(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, ShouldPerform);
            AddExecutor(ExecutorType.SpSummon, ShouldPerform);
            AddExecutor(ExecutorType.Summon, ShouldPerform);
            AddExecutor(ExecutorType.MonsterSet, ShouldPerform);
            AddExecutor(ExecutorType.SummonOrSet, ShouldPerform);
            //AddExecutor(ExecutorType.Repos, ShouldPerform);
            AddExecutor(ExecutorType.SpellSet, ShouldPerform);


            AddExecutor(ExecutorType.GoToBattlePhase, ShouldPerform);
            AddExecutor(ExecutorType.GoToMainPhase2, ShouldPerform);
            AddExecutor(ExecutorType.GoToEndPhase, ShouldPerform);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            if (SQLComm.IsMCTS)
                aIEngine = new MCTSEngine(this);
            else
                aIEngine = new NeuralNet(this);
        }


        private List<long> HintMsgForEnemy = new List<long>
        {
            HintMsg.Release, HintMsg.Destroy, HintMsg.Remove, HintMsg.ToGrave, HintMsg.ReturnToHand, HintMsg.ToDeck,
            HintMsg.FusionMaterial, HintMsg.SynchroMaterial, HintMsg.XyzMaterial, HintMsg.LinkMaterial
        };
        private List<long> HintMsgForDeck = new List<long>
        {
            HintMsg.SpSummon, HintMsg.ToGrave, HintMsg.Remove, HintMsg.AddToHand, HintMsg.FusionMaterial
        };
        private List<long> HintMsgForSelf = new List<long>
        {
            HintMsg.Equip
        };
        private List<long> HintMsgForMaterial = new List<long>
        {
            HintMsg.FusionMaterial, HintMsg.SynchroMaterial, HintMsg.XyzMaterial, HintMsg.LinkMaterial, HintMsg.Release
        };
        private List<long> HintMsgForMaxSelect = new List<long>
        {
            HintMsg.SpSummon, HintMsg.ToGrave, HintMsg.AddToHand, HintMsg.FusionMaterial, HintMsg.Destroy
        };

        public bool ShouldPerform()
        {
            Stopwatch watch = Stopwatch.StartNew();

            if (Type == ExecutorType.Activate && Card.Id == CardId.SimultaneousCannon)
                if (!SimultaneousCannonActivate() && !isChainResolving)
                    return false;

            var perform = aIEngine.ShouldPerform(Card, Type.ToString(), ActivateDescription, GetFieldState(), Duel);
            if (perform && Card != null)
            {
                if (!used.Contains(Card.Name))
                    used.Add(Card.Name);
            }

            Logger.DebugWriteLine("ShouldPerform Time:" + watch.Elapsed, ConsoleColor.Yellow);

            return perform;
        }

        // Choose Go first or second
        public override bool OnSelectHand()
        {
            Stopwatch watch = Stopwatch.StartNew();

            aIEngine.ShouldPerform(null, "GoFirst", -1, new List<FieldStateValues>(), Duel);

            Logger.DebugWriteLine("OnSelectHand Time:" + watch.Elapsed, ConsoleColor.Yellow);

            return true;
        }

        public override void OnNewTurn()
        {
            Stopwatch watch = Stopwatch.StartNew();

            base.OnNewTurn();
            previousActions.Clear();
            previousActionsEnemy.Clear();

            lingeringPrevTurnActions = lingeringTurnActions;
            lingeringTurnActions = new List<PreviousAction>();

            lingeringPrevTurnActionsEnemy = lingeringTurnActionsEnemy;
            lingeringTurnActions = new List<PreviousAction>();

            aIEngine.OnNewTurn(Duel);

            Logger.DebugWriteLine("OnNewTurn Time:" + watch.Elapsed, ConsoleColor.Yellow);
        }


        public override void OnNewPhase()
        {
            Stopwatch watch = Stopwatch.StartNew();

            base.OnNewPhase();
            aIEngine.OnNewPhase();

            Logger.DebugWriteLine("OnNewPhase Time:" + watch.Elapsed, ConsoleColor.Yellow);
        }

        public override void SetMain(MainPhase main)
        {
            Stopwatch watch = Stopwatch.StartNew();

            base.SetMain(main);
            var fieldState = GetFieldState();
            fieldState.Add(new FieldStateValues(
                "",
                "IsMainPhasePriority",
                "true"));
            fieldState.Add(new FieldStateValues(
                "",
                "CanBattlePhase",
                main.CanBattlePhase.ToString()));

            materialSelected = 0;
            aIEngine.SetMain(main, fieldState, Duel);

            Logger.DebugWriteLine("SetMain Time:" + watch.Elapsed, ConsoleColor.Yellow);
        }

        public override void SetBattle(BattlePhase battle)
        {
            Stopwatch watch = Stopwatch.StartNew();

            base.SetBattle(battle);
            var fieldState = GetFieldState();

            // Current Attackers
            foreach(var card in battle.AttackableCards)
            {
                fieldState.Add(new FieldStateValues(
                    "CanAttack",
                    (card?.Name ?? ""),
                    ""));
            }
            if (battle.AttackableCards.Count == 0)
                fieldState.Add(new FieldStateValues(
                    "NoAttackers",
                    "",
                    ""));


            aIEngine.SetBattle(battle, fieldState, Duel);

            Logger.DebugWriteLine("SetMain Time:" + watch.Elapsed, ConsoleColor.Yellow);
        }


        public override bool OnSelectYesNo(long desc)
        {
            Stopwatch watch = Stopwatch.StartNew();

            var option = Util.GetOptionFromDesc(desc);
            var cardId = Util.GetCardIdFromDesc(desc);
            var res =  aIEngine.ShouldPerform(null, "YesNo", desc, GetFieldState(), Duel);

            Logger.DebugWriteLine("OnSelectYesNo Time:" + watch.Elapsed, ConsoleColor.Yellow);

            return res;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> _cards, int min, int max, long hint, bool cancelable)
        {
            Stopwatch watch = Stopwatch.StartNew();

            //if (Duel.Phase == DuelPhase.BattleStart)
            //    return null;
            if (AI.HaveSelectedCards())
                return null;

            IList<ClientCard> cards = new List<ClientCard>(_cards);
            ClientCard currentCard = GetCurrentCardResolveInChain();

            IList<ClientCard> selected = new List<ClientCard>();

            // Custom Default selection
            if (currentCard != null)
            {
                if (CardId.ForbiddenDroplet == currentCard.Id)
                {
                    if (hint == HintMsg.ToGrave)
                    {

                    }

                }

                if (CardId.SimultaneousCannon == currentCard.Id)
                {
                    int[] levels = SimultaneousCannonLevels();
                    if (levels != null)
                    {
                        selected.Add(_cards.FirstOrDefault(x => x.HasType(CardType.Fusion) && x.Level == levels[0]));
                        selected.Add(_cards.FirstOrDefault(x => x.HasType(CardType.Xyz) && x.Rank == levels[1]));
                        //selected.Add(_cards.FirstOrDefault(x => x.HasType(CardType.Xyz) && x.Level == levels[1])); maybe don't need due to auto selector?
                    }
                }
            }


            if (selected.Count == 0)
                selected = aIEngine.SelectCards(currentCard, min, max, hint, cancelable, cards, GetFieldState(), Duel);



            #region Random selection
            // Fill in the remaining with defaults
            if (HintMsgForEnemy.Contains(hint))
            {
                IList<ClientCard> enemyCards = cards.Where(card => card.Controller == 1).ToList();

                // select enemy's card first
                while (enemyCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = enemyCards[Program.Rand.Next(enemyCards.Count)];
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
                    ClientCard card = deckCards[Program.Rand.Next(deckCards.Count)];
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
                    ClientCard card = botCards[Program.Rand.Next(botCards.Count)];
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
                    ClientCard card = cards[Program.Rand.Next(cards.Count)];
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
                ClientCard card = cards[0];//cards[Program.Rand.Next(cards.Count)];
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

            Logger.DebugWriteLine("OnSelectCard Time:" + watch.Elapsed, ConsoleColor.Yellow);

            return selected;
        }


        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            return base.OnSelectPosition(cardId, positions);
            Stopwatch watch = Stopwatch.StartNew();

            var res = aIEngine.OnSelectPosition(cardId, positions, GetFieldState(), Duel);
            //return base.OnSelectPosition(cardId, positions);

            Logger.DebugWriteLine("OnSelectPosition Time:" + watch.Elapsed, ConsoleColor.Yellow);

            return res;
        }


        // TODO
        public override IList<ClientCard> OnCardSorting(IList<ClientCard> cards)
        {
            return base.OnCardSorting(cards);
        }

        // Called when a chain is about to happen
        public override void SetChain(IList<ClientCard> cards, IList<long> descs, bool forced)
        {
            Stopwatch watch = Stopwatch.StartNew();

            base.SetChain(cards, descs, forced);

            aIEngine.SetChain(cards, descs, forced, GetFieldState(), Duel, Util);

            Logger.DebugWriteLine("OnSelectPosition Time:" + watch.Elapsed, ConsoleColor.Yellow);
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
                if (shouldAddLingeringAction(card.Id, ExecutorType.Activate))
                {
                    lingeringTurnActionsEnemy.Add(new PreviousAction() { cardId = card.Id, type = ExecutorType.Activate });
                }
            }
            else
            {
                //Console.WriteLine("Add Player Chain " + chainLinkCount.ToString() + " " + card.Name);
                playerChainIndex.Push(chainLinkCount);
            }
            base.OnChaining(player, card);
        }

        public override void OnChainSolving()
        {

            isChainResolving = true;
            aIEngine.OnChainSolving();
        }

        // As a Chain link resolves
        public override void OnChainSolved()
        {
            if (playerChainIndex.Count > 0)
                playerChainIndex.Pop();
            aIEngine.OnChainSolved();
        }

        public override void OnChainEnd()
        {
            isChainResolving = false;
            base.OnChainEnd();
            if (chainLinkCount != 0)
                chainLinkCount = 0;
            // TODO Fix?
            aIEngine.OnChainSolved();
        }

        public override int OnSelectOption(IList<long> options)
        {
            Stopwatch watch = Stopwatch.StartNew();

            var res = aIEngine.SelectOption(options, GetFieldState(), Duel, Util);

            Logger.DebugWriteLine("OnSelectOption Time:" + watch.Elapsed, ConsoleColor.Yellow);

            return res;
        }

        // TODO
        //public override int OnAnnounceNumber()

        public override int OnAnnounceCard(IList<int> avail)
        {
            Stopwatch watch = Stopwatch.StartNew();

            var res = aIEngine.OnAnnounceCard(Util.GetLastChainCard(), avail, GetFieldState(), Duel);

            Logger.DebugWriteLine("OnAnnounceCard Time:" + watch.Elapsed, ConsoleColor.Yellow);

            return res;
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
            if (shouldAddLingeringAction(Card.Id, Type))
            {
                lingeringTurnActionsEnemy.Add(new PreviousAction() { cardId = Card.Id, type = Type });
            }
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

            aIEngine.OnWin(result);

            used.Clear();
            usedEnemy.Clear();


            postSide = false;
        }

        // Assume no extra deck side TODO
        public override void OnChangeSide(IList<int> _main, IList<int> _extra, IList<int> _side)
        {
            IList<int> pool = new List<int>(_main);
            pool = pool.Concat(_side).ToList();
            int mainCount = _main.Count();
            int sideCount = _side.Count();

            _main.Clear();
            _side.Clear();

            if (false)
            {
                int[] minMain =
                {
                CardId.SnakeEyeFlamberge,
                CardId.SnakeEyeDiabellstar,
                CardId.DiabellstarBlackWitch,
                CardId.SnakeEyeOak,
                CardId.SnakeEyeAsh,
                CardId.SnakeEyeAsh,
                CardId.SnakeEyeAsh,
                CardId.SnakeEyePoplar,
                CardId.SnakeEyePoplar,
                CardId.Bonfire,
                CardId.Bonfire,
                CardId.Bonfire,
                CardId.OriginalSinfulSpoilsSnakeEyes,
                CardId.WANTEDSinfulSpoils,
                CardId.WANTEDSinfulSpoils,
                CardId.WANTEDSinfulSpoils,
                CardId.DivineTempleSnakeEyes,
            };

                AddCardsToList(_main, pool, mainCount, minMain);
            }

            // Fill out the remaining main deck and side deck slots.
            AddCardsToList(_main, pool, mainCount);
            AddCardsToList(_side, pool, sideCount);

            postSide = true;
        }


        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            return base.OnSelectAttacker(attackers, defenders);

            Stopwatch watch = Stopwatch.StartNew();
            if (attackers.Count == 0)
                return null;
            var res = aIEngine.OnSelectAttacker(attackers, GetFieldState(), Duel);

            //return base.OnSelectAttacker(attackers, defenders);
            Logger.DebugWriteLine("OnSelectAttacker Time:" + watch.Elapsed, ConsoleColor.Yellow);

            return res;
        }


        /// <summary>
        /// Decide which card should the attacker attack.
        /// </summary>
        /// <param name="attacker">Card that attack.</param>
        /// <param name="defenders">Cards that defend.</param>
        /// <returns>BattlePhaseAction including the target, or null (in this situation, GameAI will check the next attacker)</returns>
        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            return base.OnSelectAttackTarget(attacker, defenders);

            Stopwatch watch = Stopwatch.StartNew();

            var fieldState = GetFieldState();

            // Current Attacker
            fieldState.Add(new FieldStateValues(
                "CurrentAttacker",
                (attacker?.Name ?? ""),
                ""));

            // Always attack for now
            ClientCard toAttack = aIEngine.OnSelectAttackTarget(attacker, defenders, fieldState, Duel);

            Logger.DebugWriteLine("OnSelectAttackTarget Time:" + watch.Elapsed, ConsoleColor.Yellow);

            if (toAttack != null || attacker.CanDirectAttack)
                return AI.Attack(attacker, toAttack);//toAttack == null if it is a direct attack

            return null;
            //return ;
        }


        public override IList<ClientCard> OnSelectSum(IList<ClientCard> cards, int sum, int min, int max, long hint, bool mode)
        {
            return base.OnSelectSum(cards, sum, min, max, hint, mode);
        }

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

        private List<FieldStateValues> GetFieldState()
        {
            Stopwatch watch = Stopwatch.StartNew();

            List<FieldStateValues> c = new List<FieldStateValues>();

            //c.Add(new FieldStateValues("", "Turn", Duel.Turn.ToString()));
            //c.Add(new FieldStateValues("", "ActionNumber", ActionNumber.ToString()));

            c.Add(new FieldStateValues("", "CurrentPhase", Duel.Phase.ToString()));
            c.Add(new FieldStateValues("", "CurrentTurn", Duel.Player  == 0 ? "Player" : "Enemy"));

            c.Add(new FieldStateValues("", "PlayerMonsterCount", Bot.GetMonsterCount().ToString()));
            c.Add(new FieldStateValues("", "PlayerSpellTrapCount", Bot.GetSpellCount().ToString()));
            c.Add(new FieldStateValues("", "PlayerHandCount", Bot.GetHandCount().ToString()));
            c.Add(new FieldStateValues("", "EnemyMonsterCount", Enemy.GetMonsterCount().ToString()));
            c.Add(new FieldStateValues("", "EnemySpellTrapCount", Enemy.GetSpellCount().ToString()));

            //Hand
            for (int i = 0; i < 1; i++)
                c.AddRange(CompileValues(Duel.Fields[i].Hand, (i == 0 ? "Player" : "Enemy") + "Hand", false));

            // Monster Zone
            for (int i = 0; i <= 1; i++)
                c.AddRange(CompileValues(Duel.Fields[i].GetMonsters(), (i == 0 ? "Player" : "Enemy") + "Field", true));

            // Spell Trap Zone
            for (int i = 0; i <= 1; i++)
                c.AddRange(CompileValues(Duel.Fields[i].GetSpells(), (i == 0 ? "Player" : "Enemy") + "SpellTrap", true));

            // GraveYard
            for (int i = 0; i <= 1; i++)
                c.AddRange(CompileValues(Duel.Fields[i].Graveyard, (i == 0 ? "Player" : "Enemy") + "GY", false));

            // Banished
            for (int i = 0; i <= 1; i++)
                c.AddRange(CompileValues(Duel.Fields[i].Banished, (i == 0 ? "Player" : "Enemy") + "Banish", true));

            // Previous Turn Actions
            {
                foreach (var action in previousActions)
                {
                    c.Add(new FieldStateValues(
                        "PreviousAction",
                        (action.ToString()),
                        ""));
                }
            }

            // Previous enemy Actions
            {
                foreach (var action in previousActionsEnemy)
                {
                    c.Add(new FieldStateValues(
                        "PreviousActionEnemy",
                        (action.ToString()),
                        ""));
                }
            }

            // Lingering player Effects
            {
                foreach (var action in lingeringTurnActions.Concat(lingeringPrevTurnActions))
                {
                    c.Add(new FieldStateValues(
                        "LingeringAction",
                        (action.ToString()),
                        ""));
                }
            }

            // Lingering enemy Effects
            {
                foreach (var action in lingeringTurnActionsEnemy.Concat(lingeringPrevTurnActionsEnemy))
                {
                    c.Add(new FieldStateValues(
                        "LingeringActionEnemy",
                        (action.ToString()),
                        ""));
                }
            }

            // Cards Target on Chain
            {
                foreach (var card in Duel.ChainTargets)
                {
                    c.Add(new FieldStateValues(
                        "ChainTargets",
                        (card?.Name ?? "") + ";" + (card?.Controller == 0 ? "Player" : "Enemy"),
                        ""));
                }

                foreach (var card in Duel.CurrentChain)
                {
                    c.Add(new FieldStateValues(
                    "CurrentChain",
                    (card?.Name ?? "") + ";" + (card?.Controller == 0 ? "Player" : "Enemy"),
                    ""));
                }
            }

            // Last card on chain
            {
                var card = GetCurrentCardResolveInChain();

                if (card == null)
                    c.Add(new FieldStateValues(
                            "LastCardOnChain",
                            "NoCard",
                            ""));
                else
                    c.Add(new FieldStateValues(
                                "LastCardOnChain",
                                (card?.Name ?? "") +";" + (card?.Location.ToString()?? "") + ";" + (card?.Controller == 0 ? "Player" : "Enemy"),
                                ""));
            }

            c.Add(new FieldStateValues(
                "IsChainResolving",
                isChainResolving.ToString(),
                ""));


            // Main phase stuff
            if (Duel.MainPhaseEnd)
                c.Add(new FieldStateValues(
                    "EndOfMain",
                    "true",
                    ""));

            // Attacking Stuff
            if (Enemy.UnderAttack)
            {
                c.Add(new FieldStateValues(
                    "PlayerAttacker",
                    (Bot.BattlingMonster?.Name ?? "self"),
                    ""));
                c.Add(new FieldStateValues(
                    "EnemyDefender",
                    (Enemy.BattlingMonster?.Name ?? "self"),
                    ""));
            }

            if (Bot.UnderAttack)
            {
                c.Add(new FieldStateValues(
                    "PlayerDefender",
                    (Bot.BattlingMonster?.Name ?? "self"),
                    ""));
                c.Add(new FieldStateValues(
                    "EnemyAttacker",
                    (Enemy.BattlingMonster?.Name ?? "self"),
                    ""));
            }


            // Is Lethal
            {
                if (Duel.Player == 0)
                    if (Util.GetTotalAttackingMonsterAttack(0) > Util.Enemy.LifePoints)
                        c.Add(new FieldStateValues(
                            "PlayerHasLethal",
                            "",
                            ""));

                if (Duel.Player == 1)
                    if (Util.GetTotalAttackingMonsterAttack(1) > Util.Bot.LifePoints)
                        c.Add(new FieldStateValues(
                            "EnemyHasLethal",
                            "",
                            ""));
            }

            Logger.DebugWriteLine("     FieldStateValues Time:" + watch.Elapsed, ConsoleColor.Yellow);

            return c;
        }

        List<FieldStateValues> CompileValues(IList<ClientCard> cards, string location, bool needPosition)
        {
            List<FieldStateValues> values = new List<FieldStateValues>();
            Dictionary<string, FieldStateValues> seenCards = new Dictionary<string, FieldStateValues>();
            foreach (var card in cards)
            {
                if (card != null)
                {
                    string name = (card?.Name ?? "") + (needPosition ? ";" + (CardPosition)card?.Position : "");

                    //if ((Duel.Phase == DuelPhase.BattleStart || Duel.Phase == DuelPhase.BattleStep)) Add if card has attacked

                    if (seenCards.ContainsKey(name))
                    {
                        int value = int.Parse(seenCards[name].Value);
                        seenCards[name].Value = (value + 1).ToString();
                    }
                    else
                    {
                        var fieldStateValue = new FieldStateValues(location, name, "1");
                        values.Add(fieldStateValue);
                        seenCards.Add(name, fieldStateValue);
                    }
                }
            }

            return values;
        }

        #endregion

        #region Custom Actions
        private bool SimultaneousCannonActivate()
        {
            if (SimultaneousCannonLevels() != null)
                return true;

            return false;
        }

        // First is fusion, second value is xyz
        private int[] SimultaneousCannonLevels()
        {
            List<int> fusionLevels = new List<int>();
            List<int> xyzPairs = new List<int>();

            
            List<int> banishedFusionLevels = new List<int>();
            List<int> banishedXyzRank = new List<int>();

            List<int> targetLevels = new List<int>();
            int totalCards = Bot.GetFieldHandCount() + Enemy.GetFieldHandCount();

            foreach (var card in Enemy.GetMonsters())
            {
                int level = card.Level;
                if (!targetLevels.Contains(level) && level != 0)
                    targetLevels.Add(level);

                level = card.Rank;
                if (!targetLevels.Contains(level) && level != 0)
                    targetLevels.Add(level);
            }

            // Get all fusion levels In extra deck
            foreach (var fusionCard in Bot.ExtraDeck)
            {
                if (fusionCard == null)
                    continue;
                if (!fusionCard.HasType(CardType.Fusion))
                    continue;
                fusionLevels.Add(fusionCard.Level);
            }

            List<int> xyzLevels = new List<int>(); // This holds single xyz levels
            foreach (var xyzCard in Bot.ExtraDeck) // Find 2 xyz now
            {
                if (xyzCard == null)
                    continue;
                if (!xyzCard.HasType(CardType.Xyz))
                    continue;
                if (xyzLevels.Contains(xyzCard.Rank) && !xyzPairs.Contains(xyzCard.Rank))
                {
                    xyzPairs.Add(xyzCard.Rank);
                }
                else
                    xyzLevels.Add(xyzCard.Rank);
            }

            // Get banished levels
            foreach (var card in Bot.Banished)
            {
                if (card == null)
                    continue;
                if (card.HasType(CardType.Fusion))
                    banishedFusionLevels.Add(card.Level);
                if (card.HasType(CardType.Xyz))
                    banishedXyzRank.Add(card.Rank);
            }

            // All possible pairs

            List<SimultaneousCannonPairs> pairs = new List<SimultaneousCannonPairs>();

            foreach(int fusionLevel in fusionLevels)
                foreach(int xyzLevel in xyzPairs)
                    if (fusionLevel + xyzLevel * 2 == totalCards)
                        pairs.Add(new SimultaneousCannonPairs() { FusionLevel = fusionLevel, XYZLevel = xyzLevel });

            // Get the first valid pair
            foreach(var pair in pairs)
            {
                List<int> allFusionLevels = new List<int>(banishedFusionLevels);
                List<int> allXYZRanks = new List<int>(banishedXyzRank);
                allFusionLevels.Add(pair.FusionLevel);
                allXYZRanks.Add(pair.XYZLevel);
                foreach (int target in targetLevels)
                    foreach (int level in allFusionLevels)
                        foreach (int rank in allXYZRanks)
                            if (target == level + rank)
                                return new int[] {pair.FusionLevel, pair.XYZLevel };
            }


            return null;
        }

        private class SimultaneousCannonPairs
        {
            public int FusionLevel;
            public int XYZLevel;
        }
        #endregion

        private bool shouldAddLingeringAction(long cardId, ExecutorType type)
        {
            if (cardId == CardId.DimensionShifter && type == ExecutorType.Activate)
                return true;
            return false;
        }
    }
}
