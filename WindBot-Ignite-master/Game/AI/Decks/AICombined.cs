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
    [Deck("Combined", "AI_Combined")]
    public class AICombined : AIHardCodedBase
    {
        int[] SNAKE_EYE_SEND_SPELL_COST = {
                CardId.SkillDrain,
                CardId.AntiSpellFragrance,
                CardId.SnakeEyeDiabellstar,
                CardId.DiabellstarBlackWitch,
                CardId.SnakeEyePoplar,
                CardId.SnakeEyeOak,
                CardId.SnakeEyeAsh,
                CardId.DivineTempleSnakeEyes,
                CardId.WANTEDSinfulSpoils,
         };

        int[] SNAKE_EYES_STARTER =
{
                CardId.SnakeEyeAsh,
                CardId.DiabellstarBlackWitch,
                CardId.WANTEDSinfulSpoils,
                CardId.OriginalSinfulSpoilsSnakeEyes,
                CardId.SnakeEyePoplar
        };

        int[] dont_use_as_link_mat =
{
            CardId.UnchainedSoulRage,
            CardId.SPLittleKnight,
            CardId.FiendsmithDiesIrae,
            CardId.PhantomOfYubel,
            CardId.VarudrasBringerofEndTimes,
            CardId.DDDHighKingCaesar
        };

        int[] RunickSpells =
{
            CardId.RunickTip,
            CardId.RunickFreezingCurse,
            CardId.RunickGoldenDroplet,
            CardId.RunickSlumber,
            CardId.RunickSmitingStorm,
            CardId.RunickFlashingFire,
            CardId.RunickDestruction,
            CardId.RunickDispelling
        };

        public AICombined(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            /**************************************** Snake Eyes *********************************************/
            // Basically First Actions
            AddExecutor(ExecutorType.Activate, CardId.WANTEDSinfulSpoils);
            AddExecutor(ExecutorType.Activate, CardId.LightningStorm);
            AddExecutor(ExecutorType.Activate, CardId.SkillDrain);
            AddExecutor(ExecutorType.Activate, CardId.AntiSpellFragrance);
            AddExecutor(ExecutorType.Activate, CardId.DarkRulerNoMore);
            AddExecutor(ExecutorType.Activate, CardId.FeatherDuster);
            AddExecutor(ExecutorType.GoToBattlePhase, GoToBattlePhase);
            AddExecutor(ExecutorType.Activate, CardId.EvenlyMatched);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithTractus, FiendsmithTractusActivate);
            AddExecutor(ExecutorType.Activate, CardId.Bonfire);

            // Normal Priority
            AddExecutor(ExecutorType.Activate, CardId.TripleTacticsTalent);
            AddExecutor(ExecutorType.SpSummon, CardId.MoonOfTheClosedHeaven, ClosedMoonSpecial);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithSequentia, FiendsmithSequentiaActivate);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithRequiem, FiendsmithRequiemActivate);
            AddExecutor(ExecutorType.Activate, CardId.TheFiendsmith, TheFiendsmithActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.NecroqiopPrincess);
            AddExecutor(ExecutorType.Activate, CardId.BeatriceLadyOfEnternal);

            AddExecutor(ExecutorType.SpSummon, CardId.BeatriceLadyOfEnternal);
            AddExecutor(ExecutorType.SpSummon, CardId.FiendsmithRequiem, FiendSmithRequiemSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.FiendsmithSequentia, FiendsmithSequentiaSummon);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithLacrimosa, FiendsmithLacrimosaActivate);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithDiesIrae, FiendsmithDiesIraeActivate);

            AddExecutor(ExecutorType.Activate, CardId.DivineTempleSnakeEyes);
            AddExecutor(ExecutorType.Summon, CardId.SnakeEyeAsh, SnakeEyeAshSummon);
            AddExecutor(ExecutorType.Activate, CardId.SnakeEyeAsh, SnakeEyeAshActivate);
            AddExecutor(ExecutorType.Activate, CardId.SnakeEyeFlamberge, SnakeEyeFlambergeActivate);
            AddExecutor(ExecutorType.Activate, CardId.SnakeEyePoplar);
            AddExecutor(ExecutorType.SpSummon, CardId.DiabellstarBlackWitch, DiabellstarBlackWitchSPSummon);
            AddExecutor(ExecutorType.Summon, CardId.SnakeEyeOak, SnakeEyeOakSummon);
            AddExecutor(ExecutorType.Activate, CardId.SnakeEyeOak, SnakeEyeOakActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.IPMasquerena, IPSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PromethianPrincess, PromethianPrincessSummon);
            AddExecutor(ExecutorType.Activate, CardId.DiabellstarBlackWitch);
            AddExecutor(ExecutorType.Activate, CardId.PromethianPrincess, PromethianPrincessActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.SalamangreatRagingPhoenix, RagingPhoenixSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.HiitaCharmerAblaze, HiitaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DharcCharmerGloomy, DharcSummon);
            AddExecutor(ExecutorType.Activate, CardId.HiitaCharmerAblaze);
            AddExecutor(ExecutorType.Activate, CardId.DharcCharmerGloomy);
            AddExecutor(ExecutorType.Activate, CardId.SnakeEyeDiabellstar, SnakeEyeDiabellstarActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.SeleneQueenofMasterMagicians, SeleneMagicianSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.AccesscodeTalker, AccessCodeSummon);
            AddExecutor(ExecutorType.Activate, CardId.AccesscodeTalker, AccessCodeActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.Apollusa, ApollusaSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.RelinquishdAnima, AnimaSummon);
            AddExecutor(ExecutorType.Activate, CardId.OriginalSinfulSpoilsSnakeEyes, OSSSEActivate);
            AddExecutor(ExecutorType.Activate, CardId.OneForOne, OneforOneActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider, LinkSpiderSummon);

            AddExecutor(ExecutorType.Activate, CardId.SalamangreatRagingPhoenix);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmarePhoenix, KnightmarePhoenixSummon);
            AddExecutor(ExecutorType.Activate, CardId.KnightmarePhoenix);
  


            AddExecutor(ExecutorType.SpSummon, CardId.SPLittleKnight, SPSummon);
            AddExecutor(ExecutorType.Activate, CardId.IPMasquerena, IPActivate);
            AddExecutor(ExecutorType.Activate, CardId.SPLittleKnight, SPActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.WorldseadragonZealantis, ZealantisSummon);
            AddExecutor(ExecutorType.Activate, CardId.WorldseadragonZealantis);
            AddExecutor(ExecutorType.Activate, CardId.SeleneQueenofMasterMagicians);
            AddExecutor(ExecutorType.Activate, CardId.Apollusa, ApollusaActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.UnderworldGoddess, UnderworldGoddessSummon);
            AddExecutor(ExecutorType.Activate, CardId.UnderworldGoddess);

            // Low Priority
            AddExecutor(ExecutorType.Summon, CardId.SnakeEyePoplar);
            AddExecutor(ExecutorType.SpSummon, CardId.TyphonSkyCrisis, TyphonSummon);
            AddExecutor(ExecutorType.Activate, CardId.TyphonSkyCrisis, TyphonActivate);

            /**************************************** Yubel *********************************************/

            // Basically First Actions

            // Normal Priority
            AddExecutor(ExecutorType.Activate, CardId.Yubel);
            AddExecutor(ExecutorType.Activate, CardId.Yubel11);
            AddExecutor(ExecutorType.Activate, CardId.Yubel12);
            AddExecutor(ExecutorType.Activate, CardId.SpiritOfYubel);
            AddExecutor(ExecutorType.Activate, CardId.BystialDruiswurm, BystialActivate);
            AddExecutor(ExecutorType.Activate, CardId.BystialMagnamhut, BystialActivate);

            AddExecutor(ExecutorType.Activate, CardId.TheFiendsmith, TheFiendsmithActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.FiendsmithRequiem, FiendSmithRequiemSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.FiendsmithSequentia, FiendsmithSequentiaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.VarudrasBringerofEndTimes);
            AddExecutor(ExecutorType.SpSummon, CardId.BeatriceLadyOfEnternal, BeatriceSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DDDHighKingCaesar);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithTractus);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithDiesIrae, FiendsmithDiesIraeActivate);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithLacrimosa);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithSequentia, FiendsmithSequentiaActivate);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithRequiem, FiendsmithRequiemActivate);
            AddExecutor(ExecutorType.Activate, CardId.BeatriceLadyOfEnternal, BeatriceActivate);
            //AddExecutor(ExecutorType.SpSummon, CardId.MoonOfTheClosedHeaven, ClosedHeavenSummon);

            AddExecutor(ExecutorType.Activate, CardId.NightmarePain, NightmarePainActivate);
            AddExecutor(ExecutorType.Activate, CardId.OpeningOfTheSpritGates, SpiritGatesActivate);
            AddExecutor(ExecutorType.Activate, CardId.NightmareThrone);

            AddExecutor(ExecutorType.Summon, CardId.DarkBeckoningBeast);
            AddExecutor(ExecutorType.Summon, CardId.SamsaraDLotus);
            AddExecutor(ExecutorType.Summon, CardId.ChaosSummoningBeast);

            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulSharvara, ShavaraActivate);
            AddExecutor(ExecutorType.Activate, CardId.DarkBeckoningBeast);
            AddExecutor(ExecutorType.Activate, CardId.GruesumGraveSquirmer, GraveSquirmerActivate);
            AddExecutor(ExecutorType.Activate, CardId.ChaosSummoningBeast);
            AddExecutor(ExecutorType.Activate, CardId.ChaosSummoningBeast);
            AddExecutor(ExecutorType.Activate, CardId.EscapeOfUnchained, EscapeActivate);
            AddExecutor(ExecutorType.Activate, CardId.PhantomOfYubel, PhantomActivate);
            AddExecutor(ExecutorType.Activate, CardId.VarudrasBringerofEndTimes, VarudrasActivate);
            AddExecutor(ExecutorType.Activate, CardId.DDDHighKingCaesar, CaesarActivate);
            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulYama);
            AddExecutor(ExecutorType.Activate, CardId.KnightmarePhoenix);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulRage, RageActivate);
            AddExecutor(ExecutorType.Activate, CardId.SPLittleKnight, SPActivate);

            AddExecutor(ExecutorType.SpSummon, CardId.PhantomOfYubel, PhantomSummon);

            AddExecutor(ExecutorType.Activate, CardId.Muckracker, MuckrackerActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulYama, YamaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmarePhoenix, PhoenixSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulRage);
            AddExecutor(ExecutorType.SpSummon, CardId.SPLittleKnight, SPSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Muckracker, MuckrackerSummon);


            /*********************** Tenpai *****************************/
            // Basically First Actions
            AddExecutor(ExecutorType.Activate, CardId.HeatWave);
            AddExecutor(ExecutorType.SpSummon, CardId.KashtiraFenrir);
            AddExecutor(ExecutorType.Activate, CardId.DimensionShifter);

            // Normal Priority
            AddExecutor(ExecutorType.Activate, CardId.PlanetWraithsoth);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraFenrir);
            AddExecutor(ExecutorType.Activate, CardId.PotofProsperity);
            AddExecutor(ExecutorType.Activate, CardId.SangenSummoning, SummoningActivate);
            AddExecutor(ExecutorType.Activate, CardId.SangenKaimen, KaimenActivate);
            AddExecutor(ExecutorType.Activate, CardId.TenpaiFadra, FadraActivate);
            AddExecutor(ExecutorType.Activate, CardId.TenpaiPaidra, PaidraActivate);
            AddExecutor(ExecutorType.Activate, CardId.TenpaiChundra, ChundraActivate);
            AddExecutor(ExecutorType.Summon, CardId.PokiDraco);
            AddExecutor(ExecutorType.Activate, CardId.PokiDraco, PokiDracoActivate);
            AddExecutor(ExecutorType.Summon, CardId.TenpaiGenroku);
            AddExecutor(ExecutorType.Activate, CardId.TenpaiGenroku);
            AddExecutor(ExecutorType.Summon, CardId.TenpaiPaidra);
            AddExecutor(ExecutorType.Summon, CardId.TenpaiFadra);
            AddExecutor(ExecutorType.Summon, CardId.TenpaiChundra);

            AddExecutor(ExecutorType.SpSummon, CardId.AncientFairyDragon, AncientFairySummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BlackroseDragon, BlackroseSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SangenpaiTranscendentDragion, TranscendentSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SangenpaiBidentDragion, BidentSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.TridentDragion, TridentSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BlackRoseMoonlightDragon, MoonlightSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.UltimayaTzolkin, TzolkinSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KuibeltTheBladeDragon, KuibeltSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.HieraticSealsOfSpheres, SpheresSummon);
            AddExecutor(ExecutorType.Activate, CardId.HieraticSealsOfSpheres, SpheresActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.WorldseadragonZealantis, ZealantisSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SalamangreatRagingPhoenix, RagingPhoenixSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PromethianPrincess, PrincessSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.HiitaCharmerAblaze, HiidaSummon);
            AddExecutor(ExecutorType.Activate, CardId.SangenpaiTranscendentDragion);
            AddExecutor(ExecutorType.Activate, CardId.SangenpaiBidentDragion);
            AddExecutor(ExecutorType.Activate, CardId.TridentDragion);
            AddExecutor(ExecutorType.Activate, CardId.BlackRoseMoonlightDragon);
            AddExecutor(ExecutorType.Activate, CardId.BlackroseDragon);
            AddExecutor(ExecutorType.Activate, CardId.UltimayaTzolkin);
            AddExecutor(ExecutorType.Activate, CardId.KuibeltTheBladeDragon);
            AddExecutor(ExecutorType.Activate, CardId.AncientFairyDragon);
            AddExecutor(ExecutorType.Activate, CardId.WorldseadragonZealantis);
            AddExecutor(ExecutorType.Activate, CardId.SalamangreatRagingPhoenix);
            AddExecutor(ExecutorType.Activate, CardId.PromethianPrincess);
            AddExecutor(ExecutorType.Activate, CardId.HiitaCharmerAblaze);


            AddExecutor(ExecutorType.Activate, CardId.BystialMagnamhut, BystialActivate);
            AddExecutor(ExecutorType.Activate, CardId.BystialDruiswurm, BystialActivate);

            /********************** Runick *******************************/

            // Basically First Actions
            AddExecutor(ExecutorType.Activate, CardId.HeatWave);
            // Normal Priority
            AddExecutor(ExecutorType.Activate, CardId.PotOfDuality);
            AddExecutor(ExecutorType.Activate, CardId.DimensonalFissure);
            AddExecutor(ExecutorType.Activate, CardId.UpstartGoblin);
            AddExecutor(ExecutorType.Activate, CardId.TimeTearingMorganite);
            AddExecutor(ExecutorType.Activate, CardId.OneDayOfPeace);
            AddExecutor(ExecutorType.GoToBattlePhase, GoToBattlePhase);
            AddExecutor(ExecutorType.Activate, CardId.EvenlyMatched);
            AddExecutor(ExecutorType.SpSummon, CardId.LavaGolemn);
            AddExecutor(ExecutorType.Summon, CardId.MajestyFiend);
            AddExecutor(ExecutorType.Summon, CardId.AmanoIwato);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming);
            AddExecutor(ExecutorType.Activate, CardId.MessengerOfPeace);
            AddExecutor(ExecutorType.Activate, CardId.InstantFusion, InstantFusionActivate);

            AddExecutor(ExecutorType.Activate, CardId.RunickGoldenDroplet, GoldenDropletActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickFreezingCurse, FreezingCurseActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickTip, TipActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickDispelling, DispellingActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickSlumber, SlumberActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickFlashingFire, FlashingFireActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickSmitingStorm, StormActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickDestruction, DestructionActivate);
            AddExecutor(ExecutorType.Activate, CardId.DrawMuscle);
            AddExecutor(ExecutorType.Activate, CardId.ChickenGame);
            AddExecutor(ExecutorType.Activate, CardId.CardScanner);
            AddExecutor(ExecutorType.Activate, CardId.RunickFountain, FountainActivate);
            AddExecutor(ExecutorType.Activate, CardId.ThereCanBeOnlyOne);
            AddExecutor(ExecutorType.Activate, CardId.GozenMatch);
            AddExecutor(ExecutorType.Activate, CardId.SynchroZone);
            AddExecutor(ExecutorType.Activate, CardId.SkillDrain);
            AddExecutor(ExecutorType.Activate, CardId.RivalyOfWarlords);
            AddExecutor(ExecutorType.Activate, CardId.TripleTacticsTalent);
            AddExecutor(ExecutorType.Activate, CardId.GraveOfTheSuperAncient);
            AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, SolemnJudgmentActivate);
            AddExecutor(ExecutorType.Summon, CardId.Bagooska);
            AddExecutor(ExecutorType.Activate, CardId.SleipnirRunick, SleipnirActivate);
            AddExecutor(ExecutorType.Activate, CardId.FrekiRunick);
            AddExecutor(ExecutorType.Activate, CardId.GeriRunick);
            AddExecutor(ExecutorType.Activate, CardId.MuninRunick, MuninActivate);
            AddExecutor(ExecutorType.Activate, CardId.HuginRunick, HuginActivate);
            AddExecutor(ExecutorType.Activate, CardId.InterdimensionalMatterTransolcator);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDesires);
            AddExecutor(ExecutorType.Activate, CardId.CardOfDemise);

            /*************************** Labrynth *****************************/
            // Basically First Actions
            AddExecutor(ExecutorType.Activate, CardId.EradicatorVirus);
            AddExecutor(ExecutorType.Activate, CardId.AntiSpellFragrance);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalBarrier);
            AddExecutor(ExecutorType.Activate, CardId.DifferentDimensionGround);
            AddExecutor(ExecutorType.Activate, CardId.SimultaneousCannon, SimultaneousCannonActivate);

            // Normal Priority
            AddExecutor(ExecutorType.Summon, CardId.ArianePinkLabrynth);
            AddExecutor(ExecutorType.Summon, CardId.AriannaGreenLabrynth);
            AddExecutor(ExecutorType.Activate, CardId.AriannaGreenLabrynth, GreenActivate);
            AddExecutor(ExecutorType.Activate, CardId.ArianePinkLabrynth);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthChandraglier, FurnatureActivate);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthStovie, FurnatureActivate);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthCooClock, ClockActivate);
            AddExecutor(ExecutorType.Activate, CardId.LovelyLabrynth, LovelyActivate);
            AddExecutor(ExecutorType.Activate, CardId.LadyLabrnyth, LadyActivate);
            AddExecutor(ExecutorType.Activate, CardId.BigWelcomeLabrnyth, BigWelcomeActivate);
            AddExecutor(ExecutorType.Activate, CardId.WelcomeLabrynth, WelcomeActivate);
            AddExecutor(ExecutorType.SpellSet, CardId.BigWelcomeLabrnyth);
            AddExecutor(ExecutorType.SpellSet, CardId.WelcomeLabrynth);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthLabyrinth);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthSetup, SetupActivate);
            AddExecutor(ExecutorType.Summon, CardId.BackJack);
            AddExecutor(ExecutorType.Activate, CardId.BackJack, BackJackActivate);
            AddExecutor(ExecutorType.Activate, CardId.DarumaCannon, DarumaActivate);
            AddExecutor(ExecutorType.Activate, CardId.DogmatikaPunishment, PunishmentActivate);
            AddExecutor(ExecutorType.Activate, CardId.TrapTrick, TraptrickActivate);

            AddExecutor(ExecutorType.SpSummon, CardId.ChaosAngel, ChaosAngelSpecial);
            AddExecutor(ExecutorType.Activate, CardId.ChaosAngel);

            AddExecutor(ExecutorType.SpSummon, CardId.Muckracker, MuckrackerSpecial);
            AddExecutor(ExecutorType.Activate, CardId.Muckracker);

            AddExecutor(ExecutorType.SpSummon, CardId.TyphonSkyCrisis, TyphonSummon);
            AddExecutor(ExecutorType.Activate, CardId.TyphonSkyCrisis, TyphonActivate);

            AddExecutor(ExecutorType.SpSummon, CardId.RelinquishdAnima, AnimaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulYama, YamaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulAnguish, AnguishSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulAbomination, AbominationSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SPLittleKnight, SPSummon);

            AddExecutor(ExecutorType.Activate, CardId.EnigmasterPackbit, PackbitActivate);
            AddExecutor(ExecutorType.Activate, CardId.BerfometKingPhantomBeast, BerfometActivate);
            AddExecutor(ExecutorType.Activate, CardId.Garura);
            AddExecutor(ExecutorType.Activate, CardId.ElderEntityNtss);
            AddExecutor(ExecutorType.Activate, CardId.RelinquishdAnima);
            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulYama);
            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulAnguish);
            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulAbomination, AbominationActivate);
            AddExecutor(ExecutorType.Activate, CardId.TriBrigadeBucephalus);
            AddExecutor(ExecutorType.Activate, CardId.SPLittleKnight, SpActivate);
            AddExecutor(ExecutorType.Activate, CardId.LordOfHeavelyPrison, LordPrisonActivate);
            // Low Priority

            // Reactive
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.TorrentialTribute, DarumaActivate);
            AddExecutor(ExecutorType.Activate, CardId.CrossoutDesignator, CrossoutActivate);
            AddExecutor(ExecutorType.Activate, CardId.ForbiddenDroplet, DropletActivate);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalBarrier, DefaultDimensionalBarrier);
            AddExecutor(ExecutorType.Activate, CardId.CrossoutDesignator, CosmicActivate);
            AddExecutor(ExecutorType.Activate, CardId.FantasticalPhantazmay);

            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.GoToBattlePhase);


            /********************** Branded *********************************/
            // Basically First Actions
            AddExecutor(ExecutorType.GoToBattlePhase, GoToBattlePhase);
            AddExecutor(ExecutorType.Activate, CardId.EvenlyMatched);
            // Normal Priority
            AddExecutor(ExecutorType.Summon, CardId.AluberDespia);
            AddExecutor(ExecutorType.Summon, CardId.GuidingQuem);
            AddExecutor(ExecutorType.Summon, CardId.BlazingCartesia);
            AddExecutor(ExecutorType.Summon, CardId.FallenOfAlbaz);
            AddExecutor(ExecutorType.Summon, CardId.SpringansKitt);

            AddExecutor(ExecutorType.SpSummon, CardId.BystialLubellion);

            AddExecutor(ExecutorType.Activate, CardId.BystialLubellion);
            AddExecutor(ExecutorType.Activate, CardId.AlbionTheShroudedDragon);
            AddExecutor(ExecutorType.Activate, CardId.BystialSaronir, BystialActivate);
            AddExecutor(ExecutorType.Activate, CardId.BystialMagnamhut, BystialActivate);
            AddExecutor(ExecutorType.Activate, CardId.AluberDespia);
            AddExecutor(ExecutorType.Activate, CardId.FallenOfAlbaz);
            AddExecutor(ExecutorType.Activate, CardId.SpringansKitt);
            AddExecutor(ExecutorType.Activate, CardId.BlazingCartesia, CartesiaActivate);
            AddExecutor(ExecutorType.Activate, CardId.GuidingQuem);
            AddExecutor(ExecutorType.Activate, CardId.TriBrigadeMercourier, MercourierActivate);
            AddExecutor(ExecutorType.Activate, CardId.DespianTragedy);

            AddExecutor(ExecutorType.Activate, CardId.AllureOfDarkness, DefaultAllureofDarkness);
            AddExecutor(ExecutorType.Activate, CardId.BrandedFusion);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial);
            AddExecutor(ExecutorType.Activate, CardId.FusionDeployment);
            AddExecutor(ExecutorType.Activate, CardId.GoldSarc);
            AddExecutor(ExecutorType.Activate, CardId.TripleTacticsTalent);
            AddExecutor(ExecutorType.Activate, CardId.TripleTacticsThrust);
            AddExecutor(ExecutorType.Activate, CardId.BrandedInHighSpirits, HighSpiritsActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrandedInRed, BrandedInRedActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrandedOpening);
            AddExecutor(ExecutorType.Activate, CardId.CalledByTheGrave, CalledByActivate);
            AddExecutor(ExecutorType.Activate, CardId.ForbiddenDroplet, DropletActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrandedRetribution, RetributionActivate);
            AddExecutor(ExecutorType.Activate, CardId.FusionDuplication, DuplicationActivate);

            AddExecutor(ExecutorType.Activate, CardId.GuardianChimera);
            AddExecutor(ExecutorType.Activate, CardId.AlbionTheSanctifireDragon);
            AddExecutor(ExecutorType.Activate, CardId.BorreloadFuriousDragon, FuriousActivate);
            AddExecutor(ExecutorType.Activate, CardId.MirrorJadeTheIcebladeDragon, MirrorjadeActivate);
            AddExecutor(ExecutorType.Activate, CardId.PredaplantDRagostapelia, FaceUpEffectNegate);
            AddExecutor(ExecutorType.Activate, CardId.LubellionSearingDragon);
            AddExecutor(ExecutorType.Activate, CardId.AlbaLenatusAbyssDragon);
            AddExecutor(ExecutorType.Activate, CardId.DespianQuaeritis, QuartusActivate);
            AddExecutor(ExecutorType.Activate, CardId.GranguignolDuskDragon);
            AddExecutor(ExecutorType.Activate, CardId.TitanikladAshDragon);
            AddExecutor(ExecutorType.Activate, CardId.AlbionTheBrandedDragon);
            AddExecutor(ExecutorType.Activate, CardId.RindbrummStrikingDragon, RindbrummActivate);

            AddExecutor(ExecutorType.Activate, CardId.ShadollDragon);
            AddExecutor(ExecutorType.Activate, CardId.SnatchSteal);
            AddExecutor(ExecutorType.Activate, CardId.ChangeOfHeart);
            AddExecutor(ExecutorType.Activate, CardId.BookOfEclipse, EclipseActivate);

            AddExecutor(ExecutorType.Activate, CardId.BrightestBlazingBranded, BlazingBrandedActivate);





            // Others
            AddExecutor(ExecutorType.Activate, CardId.RunickFlashingFire, FlashingFireActivate);

            // Reactive
            AddExecutor(ExecutorType.Activate, CardId.CrossoutDesignator, CrossoutActivate);
            AddExecutor(ExecutorType.Activate, CardId.CalledByTheGrave, CalledByActivate);
            AddExecutor(ExecutorType.Activate, CardId.ForbiddenDroplet, DropletActivate);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalBarrier, DefaultDimensionalBarrier);

            AddExecutor(ExecutorType.SpellSet, SpellSet);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.GoToBattlePhase);
        }


        // Choose Go first or second
        public override bool OnSelectHand()
        {
            if (GetPlayerDeckType() == Archetypes.Tenpai)
                return false;
            bool choice = true;
            return choice;
        }


        // Assume no extra deck side
        public override void OnChangeSide(IList<int> _main, IList<int> _extra, IList<int> _side)
        {
            IList<int> pool = new List<int>(_main);
            pool = pool.Concat(_side).ToList();
            int mainCount = _main.Count();
            int sideCount = _side.Count();

            _main.Clear();
            _side.Clear();

            Archetypes enemyDeck = GetEnemyDeckType();

            if (false){
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

            switch(enemyDeck)
            {
                case Archetypes.SnakeEyes:
                    if (winResult == 0) // win
                    {
                        int[] toAdd =
                        {
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.GhostMourner,
                            CardId.GhostMourner,
                            CardId.GhostMourner,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.Nibiru,
                            CardId.Nibiru,
                            CardId.Nibiru,
                            CardId.CrossoutDesignator,
                            CardId.CrossoutDesignator,
                            CardId.CrossoutDesignator,
                            CardId.OneForOne,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                            CardId.RivalyOfWarlords,
                            CardId.FantasticalPhantazmay,
                            CardId.FantasticalPhantazmay,
                            CardId.FantasticalPhantazmay,
                            CardId.MultchummyPurulia,
                            CardId.MultchummyPurulia,
                            CardId.MultchummyPurulia,
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    else // lose
                    {
                        int[] toAdd =
{
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.GhostMourner,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.Nibiru,
                            CardId.Nibiru,
                            CardId.Nibiru,
                            CardId.CrossoutDesignator,
                            CardId.CrossoutDesignator,
                            CardId.CrossoutDesignator,
                            CardId.AntiSpellFragrance,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.TripleTacticsTalent,
                            CardId.RivalyOfWarlords,
                            CardId.DifferentDimensionGround,
                            CardId.DifferentDimensionGround,
                            CardId.TorrentialTribute,
                            CardId.TorrentialTribute,
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    break;
                case Archetypes.Tenpai:
                    if (winResult == 0) // win
                    {
                        int[] toAdd =
                        {
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.DrollnLockBird,
                            CardId.DrollnLockBird,
                            CardId.DrollnLockBird,
                            CardId.CalledByTheGrave,
                            CardId.AntiSpellFragrance,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                            CardId.GhostMourner,
                            CardId.GhostMourner,
                            CardId.GhostMourner,
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    else // lose
                    {
                        int[] toAdd =
                        {
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.DrollnLockBird,
                            CardId.DrollnLockBird,
                            CardId.DrollnLockBird,
                            CardId.CalledByTheGrave,
                            CardId.AntiSpellFragrance,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                            CardId.GhostMourner,
                            CardId.GhostMourner,
                            CardId.GhostMourner,
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    break;
                case Archetypes.Labrynth:
                    {
                        int[] toAdd =
                        {
                            CardId.BystialBaldrake,
                            CardId.BystialDruiswurm,
                            CardId.BystialMagnamhut,
                            CardId.BystialSaronir,
                            CardId.CalledByTheGrave,
                            CardId.FeatherDuster,
                            CardId.CosmicCyclone,
                            CardId.CosmicCyclone,
                            CardId.CosmicCyclone,
                            CardId.EvenlyMatched,
                            CardId.EvenlyMatched,
                            CardId.EvenlyMatched,
                            CardId.LightningStorm,
                            CardId.LightningStorm,
                            CardId.LightningStorm,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    break;
                case Archetypes.Runick:
                    {
                        int[] toAdd =
                        {
                            CardId.FeatherDuster,
                            CardId.CosmicCyclone,
                            CardId.CosmicCyclone,
                            CardId.CosmicCyclone,
                            CardId.EvenlyMatched,
                            CardId.EvenlyMatched,
                            CardId.EvenlyMatched,
                            CardId.LightningStorm,
                            CardId.LightningStorm,
                            CardId.LightningStorm,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.CrossoutDesignator,
                            CardId.CrossoutDesignator,
                            CardId.CrossoutDesignator,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.CalledByTheGrave,
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    break;
            }



            // Fill out the remaining main deck and side deck slots.
            AddCardsToList(_main, pool, mainCount);
            AddCardsToList(_side, pool, sideCount);

            postSide = true;
        }

        /// <summary>
        /// Decide which card should the attacker attack.
        /// </summary>
        /// <param name="attacker">Card that attack.</param>
        /// <param name="defenders">Cards that defend.</param>
        /// <returns>BattlePhaseAction including the target, or null (in this situation, GameAI will check the next attacker)</returns>
        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            // Attack with yubels
            int[] yubels =
            {
                CardId.Yubel,
                CardId.Yubel11,
                CardId.Yubel12,
                CardId.SpiritOfYubel,
                CardId.PhantomOfYubel
            };
            if (yubels.Any(x => x == attacker.Id) && !attacker.IsDisabled())
            {
                ClientCard toAttack = null;
                if (defenders.Count > 0)
                    toAttack = defenders.OrderByDescending(x => x.GetDefensePower()).FirstOrDefault();

                return AI.Attack(attacker, toAttack);
            }

            BattlePhaseAction willAttack = base.OnSelectAttackTarget(attacker, defenders);

            if (willAttack != null)
                return willAttack;

            if (GetPlayerDeckType() != Archetypes.Tenpai)
                return null;

            // Always attack for now
            ClientCard target = null;
            if (defenders.Count > 0)
                target = defenders[0];
            return AI.Attack(attacker, target);//toAttack == null if it is a direct attack
        }


        public override bool OnSelectYesNo(long desc)
        {
            var option = Util.GetOptionFromDesc(desc);
            var cardId = Util.GetCardIdFromDesc(desc);
            if (cardId == CardId.SangenKaimen)
                return true;
            if ((cardId == CardId.ArianePinkLabrynth || cardId == CardId.AriannaGreenLabrynth)) //Should special/set
            {
                return true;
            }
            return base.OnSelectYesNo(desc);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> _cards, int min, int max, long hint, bool cancelable)
        {
            //if (Duel.Phase == DuelPhase.BattleStart)
            //    return null;
            if (AI.HaveSelectedCards())
                return null;

            ClientCard currentCard = GetCurrentCardResolveInChain();
            IList<ClientCard> selected = new List<ClientCard>();

            #region AI Selected
            if (currentCard != null)
            {
                if (CardId.DivineTempleSnakeEyes == currentCard.Id)
                {
                    if (hint == HintMsg.SpSummon)
                    {
                        selected.Add(_cards.Where(x => x.Id == CardId.IPMasquerena).FirstOrDefault());
                        selected.Add(_cards.Where(x => x.Id == CardId.PromethianPrincess).FirstOrDefault());
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeFlamberge).FirstOrDefault());
                    }
                }
                if (CardId.BeatriceLadyOfEnternal == currentCard.Id)
                {
                    selected.Add(_cards.Where(x => x.Id == CardId.SamsaraDLotus).FirstOrDefault());

                    if (GetEnemyDeckType() == Archetypes.Tenpai)
                        selected.Add(_cards.Where(x => x.Id == CardId.RiseToFullHeight).FirstOrDefault());
                    if (!HasCombo())
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());
                    selected.Add(_cards.Where(x => x.Id == CardId.BlackGoat).FirstOrDefault());
                }
                if (CardId.PromethianPrincess == currentCard.Id)
                {
                    selected.Add(_cards.Where(x => x.Controller == 0 && x.Id == CardId.SnakeEyeFlamberge).FirstOrDefault());
                    selected.Add(_cards.Where(x => x.Controller == 1 && !DONT_DESTROY.Any(y => y == x.Id)).FirstOrDefault());
                }
                if (CardId.IPMasquerena == currentCard.Id)
                {
                    selected.Add(_cards.Where(x => x.Id == CardId.SPLittleKnight).FirstOrDefault());
                }
                if (CardId.SangenSummoning == currentCard.Id)
                {
                    if (hint == HintMsg.AddToHand)
                    {
                        if (!HasPerformedPreviously(ExecutorType.Summon))
                        {
                            if (!Bot.HasInHand(CardId.TenpaiPaidra))
                                selected.Add(_cards.Where(x => x.Id == CardId.TenpaiPaidra).FirstOrDefault());
                        }
                        if (!Bot.HasInHand(CardId.TenpaiChundra))
                            selected.Add(_cards.Where(x => x.Id == CardId.TenpaiChundra).FirstOrDefault());
                        if (!Bot.HasInHand(CardId.TenpaiGenroku))
                            selected.Add(_cards.Where(x => x.Id == CardId.TenpaiGenroku).FirstOrDefault());
                    }
                    else
                    {
                        int[] dontDiscard =
                        {
                            CardId.TenpaiChundra,
                            CardId.TenpaiPaidra,
                            CardId.TenpaiGenroku
                        };
                        _cards = _cards.OrderBy(x => x.Id == CardId.TenpaiFadra ? 0 : 1).ThenBy(x => dontDiscard.Any(y => x.Id == y) ? 1 : 0).ToArray();
                    }



                    // Field spell gy target
                    selected.Add(_cards.Where(x => x.Id == CardId.TridentDragion).FirstOrDefault());
                    selected.Add(_cards.Where(x => x.Id == CardId.SangenpaiTranscendentDragion).FirstOrDefault());
                    selected.Add(_cards.Where(x => x.Id == CardId.SangenpaiBidentDragion).FirstOrDefault());
                }
                if (CardId.SangenKaimen == currentCard.Id)
                {
                    if (!Bot.HasInHand(CardId.TenpaiChundra))
                        selected.Add(_cards.Where(x => x.Id == CardId.TenpaiChundra).FirstOrDefault());
                    if (!Bot.HasInHand(CardId.TenpaiGenroku))
                        selected.Add(_cards.Where(x => x.Id == CardId.TenpaiGenroku).FirstOrDefault());
                    if (!Bot.HasInHand(CardId.TenpaiPaidra))
                        selected.Add(_cards.Where(x => x.Id == CardId.TenpaiPaidra).FirstOrDefault());

                    selected.Add(_cards.Where(x => x.Id == CardId.TenpaiChundra).FirstOrDefault());
                    selected.Add(_cards.Where(x => x.Id == CardId.TenpaiFadra).FirstOrDefault());
                    selected.Add(_cards.Where(x => x.Id == CardId.TenpaiGenroku).FirstOrDefault());
                    selected.Add(_cards.Where(x => x.Id == CardId.TenpaiPaidra).FirstOrDefault());

                }
                if (CardId.TenpaiPaidra == currentCard.Id)
                {
                    if (hint == HintMsg.SpSummon)
                        selected = selected.Concat(TenpaiBattlePhaseSynchro(_cards)).ToList();
                    // Add to Hand
                    if (!Bot.HasInHand(CardId.SangenKaimen))
                        selected.Add(_cards.Where(x => x.Id == CardId.SangenKaimen).FirstOrDefault());
                    if (!Bot.HasInHand(CardId.SangenSummoning))
                        selected.Add(_cards.Where(x => x.Id == CardId.SangenSummoning).FirstOrDefault());
                }
                if (CardId.TenpaiChundra == currentCard.Id)
                {
                    if (hint == HintMsg.SpSummon)
                        selected = selected.Concat(TenpaiBattlePhaseSynchro(_cards)).ToList();
                    // Special from deck
                    if (!Bot.HasInHand(CardId.SangenKaimen) && !HasPerformedPreviously(CardId.SangenKaimen))
                        selected.Add(_cards.Where(x => x.Id == CardId.TenpaiPaidra).FirstOrDefault());
                    if (!Bot.HasInMonstersZone(CardId.TenpaiFadra))
                        selected.Add(_cards.Where(x => x.Id == CardId.TenpaiFadra).FirstOrDefault());
                    if (!Bot.HasInMonstersZone(CardId.TenpaiPaidra))
                        selected.Add(_cards.Where(x => x.Id == CardId.TenpaiPaidra).FirstOrDefault());
                }
                if (CardId.TenpaiFadra == currentCard.Id)
                {
                    if (hint == HintMsg.SpSummon)
                        selected = selected.Concat(TenpaiBattlePhaseSynchro(_cards)).ToList();
                    // Special from gy
                    if (!Bot.HasInMonstersZone(CardId.TenpaiChundra))
                        selected.Add(_cards.Where(x => x.Id == CardId.TenpaiChundra).FirstOrDefault());
                }
                if (CardId.TenpaiGenroku == currentCard.Id)
                {
                    if (!Bot.HasInHand(CardId.SangenKaimen) && !HasPerformedPreviously(CardId.SangenKaimen))
                        selected.Add(_cards.Where(x => x.Id == CardId.TenpaiPaidra).FirstOrDefault());
                    if (!Bot.HasInMonstersZone(CardId.TenpaiChundra) && !HasPerformedPreviously(CardId.TenpaiChundra))
                        selected.Add(_cards.Where(x => x.Id == CardId.TenpaiChundra).FirstOrDefault());
                    if (!Bot.HasInMonstersZone(CardId.TenpaiFadra) && !HasPerformedPreviously(CardId.TenpaiFadra))
                        selected.Add(_cards.Where(x => x.Id == CardId.TenpaiFadra).FirstOrDefault());
                    if (!Bot.HasInMonstersZone(CardId.TenpaiPaidra) && !HasPerformedPreviously(CardId.TenpaiPaidra))
                        selected.Add(_cards.Where(x => x.Id == CardId.TenpaiPaidra).FirstOrDefault());
                }
                if (CardId.SangenpaiBidentDragion == currentCard.Id)
                {
                    if (hint == HintMsg.SpSummon)
                    {
                        selected.Add(_cards.Where(x => x.Id == CardId.TenpaiFadra).FirstOrDefault());
                        selected.Add(_cards.Where(x => x.Id == CardId.TenpaiChundra).FirstOrDefault());
                    }
                    else
                    {
                        _cards.OrderBy(x => x.Owner == 0 ? 1 : 0);
                    }
                }
                if (CardId.SangenpaiTranscendentDragion == currentCard.Id)
                {
                    if (hint == HintMsg.Destroy)
                    {
                        selected.Add(_cards.Where(x => x.Id == CardId.SangenSummoning).FirstOrDefault());
                        _cards.OrderBy(x => x.Owner == 0 ? 1 : 0).ThenBy(x => x.Location == CardLocation.MonsterZone ? 0 : 1);
                    }
                }
                if (CardId.TridentDragion == currentCard.Id)
                {
                    if (hint == HintMsg.Target)
                    {
                        selected.Add(_cards.Where(x => x.Id == CardId.SangenSummoning).FirstOrDefault());
                        selected.Add(_cards.Where(x => x.Id == CardId.SangenpaiTranscendentDragion).FirstOrDefault());
                    }
                }
                if (CardId.KuibeltTheBladeDragon == currentCard.Id)
                {

                }
                if (CardId.PotofProsperity == currentCard.Id)
                {
                    if (hint == HintMsg.Remove)
                    {
                        if (Duel.Turn == 1)
                        {
                            selected.Add(_cards.Where(x => x.Id == CardId.HiitaCharmerAblaze).FirstOrDefault());
                            selected.Add(_cards.Where(x => x.Id == CardId.WorldseadragonZealantis).FirstOrDefault());
                            selected.Add(_cards.Where(x => x.Id == CardId.SalamangreatRagingPhoenix).FirstOrDefault());
                        }
                        else
                        {
                            selected.Add(_cards.Where(x => x.Id == CardId.CrystalWingSynchroDragon).FirstOrDefault());
                            selected.Add(_cards.Where(x => x.Id == CardId.UltimayaTzolkin).FirstOrDefault());
                            selected.Add(_cards.Where(x => x.Id == CardId.AncientFairyDragon).FirstOrDefault());
                        }

                        _cards = _cards.OrderBy(x => (
                            x.Id == CardId.HieraticSealsOfSpheres ||
                            x.Id == CardId.SangenpaiBidentDragion ||
                            x.Id == CardId.SangenpaiTranscendentDragion ||
                            x.Id == CardId.TridentDragion) ?
                            1 : 0
                            ).ToList();
                    }
                    if (hint == HintMsg.AddToHand)
                    {
                        if (!Bot.HasInHand(CardId.TenpaiPaidra))
                            selected.Add(_cards.Where(x => x.Id == CardId.TenpaiPaidra).FirstOrDefault());
                        if (!Bot.HasInHand(CardId.TenpaiChundra))
                            selected.Add(_cards.Where(x => x.Id == CardId.TenpaiPaidra).FirstOrDefault());
                        if (!Bot.HasInHand(CardId.SangenKaimen))
                            selected.Add(_cards.Where(x => x.Id == CardId.SangenKaimen).FirstOrDefault());
                        if (!Bot.HasInHand(CardId.SangenSummoning))
                            selected.Add(_cards.Where(x => x.Id == CardId.SangenSummoning).FirstOrDefault());
                    }
                }
                if (hint == HintMsg.SpSummon)
                {
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFountain))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.HuginRunick));
                    if (Duel.Phase.HasFlag(DuelPhase.BattleStep))
                    {
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.GeriRunick));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.FrekiRunick));
                    }
                    selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.SleipnirRunick));
                    selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.MuninRunick));
                }
                if (CardId.RunickFreezingCurse == currentCard.Id)
                {
                    if (hint == HintMsg.Target)
                    {
                        if (Duel.CurrentChain.Count() >= 2)
                        {
                            if (CardId.InfiniteImpermanence == Card.Id ||
                                CardId.EffectVeiler == Card.Id ||
                                CardId.GhostMourner == Card.Id)
                                selected.Add(_cards.FirstOrDefault(x => Duel.CurrentChain.Any(y => y.Equals(x))));
                        }
                    }
                }
                if (CardId.RunickTip == currentCard.Id)
                {
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFountain))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFountain));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFreezingCurse))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFreezingCurse));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFlashingFire))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFlashingFire));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFountain))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFountain));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickSlumber))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickSlumber));
                }
                if (CardId.RunickDestruction == currentCard.Id)
                {
                    foreach (var target in SPELL_FIELD_TARGETS)
                        selected.Add(_cards.FirstOrDefault(x => x.Id == target));
                }
                if (CardId.RunickFlashingFire == currentCard.Id)
                {
                    foreach (var target in MONSTER_FIELD_TARGETS)
                        selected.Add(_cards.FirstOrDefault(x => x.Id == target));
                }
                if (CardId.InterdimensionalMatterTransolcator == currentCard.Id)
                {
                    selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.MajestyFiend));
                    selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AmanoIwato));
                    foreach (var card in _cards)
                        if (card.Controller == 0)
                            selected.Add(card);
                }
                if (CardId.PotOfDuality == currentCard.Id)
                {
                    if (!Bot.HasInHandOrInSpellZone(CardId.SolemnJudgment))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.SolemnJudgment));
                    if (!Bot.HasInHandOrInSpellZone(CardId.SkillDrain))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.SkillDrain));
                    if (!Bot.HasInHandOrInSpellZone(CardId.GozenMatch))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.GozenMatch));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RivalyOfWarlords))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RivalyOfWarlords));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickTip))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickTip));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFountain))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFountain));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFreezingCurse))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFreezingCurse));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFlashingFire))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFlashingFire));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFountain))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFountain));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickSlumber))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickSlumber));
                }
                if (CardId.BigWelcomeLabrnyth == currentCard.Id)
                {
                    if (hint == HintMsg.SpSummon)
                    {
                        if (Bot.GetMonsterCount() == 0)
                        {
                            if (Duel.Player == 0)
                            {
                                if (!Bot.HasInHand(CardId.AriannaGreenLabrynth))
                                    selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AriannaGreenLabrynth));
                                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LadyLabrnyth));
                            }
                            else
                            {
                                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LadyLabrnyth));
                                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthStovie));
                                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthChandraglier));
                                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthCooClock));
                            }
                        }
                        else
                        {
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LovelyLabrynth));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AriannaGreenLabrynth));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.ArianePinkLabrynth));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LadyLabrnyth));
                        }
                    }
                    else if (hint == HintMsg.ReturnToHand)
                    {
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthCooClock));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthStovie));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthChandraglier));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AriasLabrnyth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LadyLabrnyth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AriannaGreenLabrynth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.ArianePinkLabrynth));
                    }
                }
                if (CardId.LabrynthStovie == currentCard.Id || CardId.LabrynthChandraglier == currentCard.Id)
                {
                    if (hint == HintMsg.Discard)
                    {
                        if (Duel.CurrentChain.Any(x => x.Id == CardId.Nibiru && x.Owner == 0))
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.Nibiru));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.BackJack));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LovelyLabrynth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthChandraglier));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthCooClock));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.WelcomeLabrynth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.BigWelcomeLabrnyth));
                    }
                    else if (hint == HintMsg.Set)
                    {
                        if (!Bot.HasInSpellZoneOrInGraveyard(CardId.BigWelcomeLabrnyth))
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.BigWelcomeLabrnyth));
                        if (!Bot.HasInSpellZone(CardId.WelcomeLabrynth))
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.WelcomeLabrynth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthLabyrinth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.BigWelcomeLabrnyth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.WelcomeLabrynth));

                    }
                }
                if (CardId.AriannaGreenLabrynth == currentCard.Id)
                {
                    if (hint == HintMsg.AddToHand)
                    {
                        if (Duel.Player == 0)
                        {
                            int[] trapsToActivate =
                            {
                                CardId.WelcomeLabrynth,
                                CardId.BigWelcomeLabrnyth,
                                CardId.TrapTrick
                            };
                            if (Bot.HasInHand(trapsToActivate) && !Bot.HasInHandOrHasInMonstersZone(CardId.AriasLabrnyth) && !HasPerformedPreviously(CardId.AriasLabrnyth))
                            {
                                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AriasLabrnyth));
                            }
                        }
                        else
                        {
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthCooClock));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AriasLabrnyth));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LadyLabrnyth));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthStovie));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthChandraglier));
                        }
                    }
                }
                if (CardId.SimultaneousCannon == currentCard.Id)
                {
                    int[] levels = SimultaneousCannonLevels();
                    if (levels != null)
                    {
                        selected.Add(_cards.FirstOrDefault(x => x.HasType(CardType.Fusion) && x.Level == levels[0]));
                        selected.Add(_cards.FirstOrDefault(x => x.HasType(CardType.Xyz) && x.Level == levels[1]));
                        //selected.Add(_cards.FirstOrDefault(x => x.HasType(CardType.Xyz) && x.Level == levels[1])); maybe don't need due to auto selector?
                    }
                }
                if (CardId.RunickFlashingFire == currentCard.Id)
                {
                    if (hint == HintMsg.SpSummon)
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.HuginRunick));
                    else
                    {
                        foreach (var target in MONSTER_FIELD_TARGETS)
                            selected.Add(_cards.FirstOrDefault(x => x.Id == target));
                    }
                }
            }
            if (hint == HintMsg.AddToHand)
            {
                if (_cards.ContainsCardWithId(CardId.SnakeEyeAsh) && !HasPerformedPreviously(CardId.SnakeEyeAsh, ExecutorType.Summon))
                    selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());

                if (_cards.ContainsCardWithId(CardId.SnakeEyePoplar) && !HasPerformedPreviously(CardId.SnakeEyePoplar, 0))
                    selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyePoplar).FirstOrDefault());

                selected.Add(_cards.Where(x => x.Id == CardId.DivineTempleSnakeEyes).FirstOrDefault());
                selected.Add(_cards.Where(x => x.Id == CardId.OriginalSinfulSpoilsSnakeEyes).FirstOrDefault());
                selected.Add(_cards.Where(x => x.Id == CardId.DiabellstarBlackWitch).FirstOrDefault());

            }
            if (hint == HintMsg.ToField) // Place in spell and trap zone?
            {
                selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeDiabellstar).FirstOrDefault());
            }
            if (hint == HintMsg.Set)
            {
                selected.Add(_cards.Where(x => x.Id == CardId.OriginalSinfulSpoilsSnakeEyes).FirstOrDefault());
            }
            if (hint == HintMsg.ToGrave) // Send for cost?
            {
                foreach (int id in SNAKE_EYE_SEND_SPELL_COST)
                    if (_cards.ContainsCardWithId(id))
                        selected.Add(_cards.Where(x => x.Id == id && x.Location == CardLocation.SpellZone).FirstOrDefault());

                // Send from spell zone then hand first
                _cards = _cards
                    .OrderBy(x => x.Location == CardLocation.SpellZone ? 0 : 1)
                    .OrderBy(x => x.Location == CardLocation.Hand ? 0 : 1)
                    .ThenBy(x => x.Id == CardId.DiabellstarBlackWitch ? 0 : 1)
                    .ThenBy(x => x.Id == CardId.SnakeEyeAsh ? 0 : 1)
                    .ThenBy(x => x.Id == CardId.SnakeEyePoplar ? 0 : 1)
                    .ToList();
            }
            if (hint == HintMsg.SpSummon) // From anywhere
            {
                // Summon flamberge from gy first
                selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeFlamberge && x.Location == CardLocation.Grave).FirstOrDefault());

                if (_cards.Where(x => x.Id == CardId.SnakeEyeOak).Any() && !Bot.HasInSpellZone(CardId.OriginalSinfulSpoilsSnakeEyes))
                    selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeOak).FirstOrDefault());

                if (_cards.ContainsCardWithId(CardId.SnakeEyeFlamberge))
                {
                    // Get the ones from deck first
                    foreach(ClientCard card in _cards.Where(x => x.Id == CardId.SnakeEyeFlamberge).OrderBy(x => x.Location == CardLocation.Deck ? 0 : 1).ToList())
                        selected.Add(card);
                }

                selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());
                selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeOak).FirstOrDefault());

                selected = selected.Concat(TenpaiBattlePhaseSynchro(_cards)).ToList();

            }
            if (hint == HintMsg.Target)
            {
                if (CardId.SnakeEyeOak == Duel.CurrentChain.LastOrDefault().Id)
                {
                    if (_cards.ContainsCardWithId(CardId.SnakeEyeAsh) && !HasPerformedPreviously(CardId.SnakeEyeAsh, ExecutorType.Activate))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());

                    if (_cards.ContainsCardWithId(CardId.SnakeEyePoplar) && !HasPerformedPreviously(CardId.SnakeEyePoplar, 0))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyePoplar).FirstOrDefault());

                    selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());
                }
                if (CardId.SnakeEyePoplar == Duel.CurrentChain.LastOrDefault().Id)
                {
                    if (_cards.ContainsCardWithId(CardId.SnakeEyeDiabellstar) && !HasPerformedPreviously(CardId.SnakeEyeDiabellstar, ExecutorType.Activate))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeDiabellstar).FirstOrDefault());

                    if (Bot.GetGraveyardMonsters().Where(x => x.Level == 1 && x.HasAttribute(CardAttribute.Fire)).Count() > 2)
                    {
                        if (_cards.ContainsCardWithId(CardId.SnakeEyeFlamberge))
                            selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeFlamberge).FirstOrDefault());

                        if (_cards.ContainsCardWithId(CardId.SnakeEyePoplar))
                            selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyePoplar).FirstOrDefault());

                        if (_cards.ContainsCardWithId(CardId.SnakeEyeOak))
                            selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeOak).FirstOrDefault());

                        if (_cards.ContainsCardWithId(CardId.SnakeEyeAsh))
                            selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());
                    }
                }
                if (CardId.SnakeEyeDiabellstar == Duel.CurrentChain.LastOrDefault().Id)
                {
                    if (_cards.ContainsCardWithId(CardId.SnakeEyeOak))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeOak).FirstOrDefault());

                    if (_cards.ContainsCardWithId(CardId.SnakeEyeAsh))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());

                    if (_cards.ContainsCardWithId(CardId.SnakeEyePoplar))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyePoplar).FirstOrDefault());

                    if (_cards.ContainsCardWithId(CardId.SnakeEyeFlamberge))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeFlamberge).FirstOrDefault());
                }
                if (CardId.SnakeEyeFlamberge == Duel.CurrentChain.LastOrDefault().Id)
                {
                    if (Duel.Player == 0)
                    {
                        if (Duel.Turn != 1)// Non first turn actions
                        {
                            foreach (int id in MONSTER_FIELD_TARGETS)
                                selected.Add(_cards.Where(x => x.Id == id && x.Owner == 1).FirstOrDefault());
                        }

                        // Other targets to place
                        selected.Add(_cards.Where(x => x.Id == CardId.IPMasquerena && x.Owner == 0 && x.Location == CardLocation.Grave).FirstOrDefault());
                    }
                    else
                    {
                        selected.Add(_cards.Where(x => x.Id == CardId.IPMasquerena).FirstOrDefault());
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeOak).FirstOrDefault());
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());
                    }
                }
                if (CardId.KnightmarePhoenix == currentCard.Id)
                {
                    foreach (var target in SPELL_FIELD_TARGETS)
                        selected.Add(_cards.FirstOrDefault(x => x.Id == target));
                }
                
            }
            if (hint == HintMsg.LinkMaterial)
            {
                IList<ClientCard> highPriority = new List<ClientCard>();
                IList<ClientCard> lowPriority = new List<ClientCard>();

                int[] highList =
                {
                    CardId.HiitaCharmerAblaze,
                    CardId.SnakeEyePoplar,
                    CardId.DarkBeckoningBeast,
                    CardId.SamsaraDLotus,
                    CardId.FabledLurrie,
                    CardId.ChaosSummoningBeast,
                };
                int[] lowList =
                {
                    CardId.SnakeEyeFlamberge,
                };

                lowList = lowList.Concat(dont_use_as_link_mat).ToArray();

                _cards = _cards
                    .OrderBy(x => highList.Contains(x.Id) ? 0 : 1)
                    .ThenBy(x => x.HasType(CardType.Link) ? -x.LinkCount: 0) // Use the highest link monster first
                    .ThenBy(x => lowList.Contains(x.Id) ? 1 : 0)
                    .ToList();

                selected.Add(_cards[0]);

                // Stop selecting if using low priorty cards
                if (materialSelected > 0 && cancelable && _cards.Where(x => lowList.Contains(x.Id)).Count() == _cards.Count())
                    return null;

                materialSelected += 1;
            }
            if (hint == HintMsg.Remove)
            {
                if (CardId.AccesscodeTalker == Card.Id)
                {
                    // Grave yard first
                    _cards = _cards.OrderBy(x => x.Location == CardLocation.Grave ? 0 : 1).ToArray();
                }
            }
            if (hint == HintMsg.Destroy)
            {
                _cards = _cards
                            .OrderBy(x => x.Controller == 1 ? 0 : 1) // Enemies first
                            .ThenBy(x => x.Location == CardLocation.MonsterZone ? 0 : 1)
                            .ToArray();
            }
            #endregion

            return SelectMinimum(selected, _cards, min, max, hint);
        }

        public override IList<ClientCard> OnCardSorting(IList<ClientCard> cards)
        {
            return base.OnCardSorting(cards);
        }


        public override int OnSelectOption(IList<long> options)
        {
            foreach(long desc in options)
            {
                var option = Util.GetOptionFromDesc(desc);
                var cardId = Util.GetCardIdFromDesc(desc);
            }

            if (options.Count == 2)
            {
                var cardId = Util.GetCardIdFromDesc(options[0]);
                if (RunickSpells.Any(x => x == cardId))
                {
                    if (SpecialRunick() && cardId != CardId.RunickTip)
                        return 1;
                    else
                        return 0;
                }

                var hint = options[0];
                cardId = Util.GetCardIdFromDesc(options[1]);

                if (hint == HintMsg.GYToHand)
                {
                    if (cardId == CardId.SnakeEyeOak)
                        return 1; // 0 = to hand, 1 = ss to field
                }
                if (cardId == CardId.TenpaiPaidra)
                {
                    return 0; // 0 = to hand, 1 = set to field
                }
            }

            return base.OnSelectOption(options);
        }

        public override IList<ClientCard> OnSelectSum(IList<ClientCard> cards, int sum, int min, int max, long hint, bool mode)
        {
            return base.OnSelectSum(cards, sum, min, max, hint, mode);
        }

        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            return base.OnSelectAttacker(attackers, defenders);
        }

        public override int OnAnnounceCard(IList<int> avail)
        {
            return base.OnAnnounceCard(avail);
        }


        #region Generic Actions
        public bool SpellSet()
        {
            if (RunickSpells.Any(x => x == Card.Id) && Bot.GetFieldSpellCard()?.Id == CardId.RunickFountain)
                return false;

            return (Card.IsTrap() || Card.HasType(CardType.QuickPlay) || DefaultSpellMustSetFirst()) && Bot.GetSpellCountWithoutField() < 4;
        }
        #endregion


        #region Generic Monsters

        public bool IPSummon()
        {
            if (Duel.Turn == 1)
            {
                if (Bot.HasInMonstersZone(CardId.SnakeEyeFlamberge) && !HasPerformedPreviously(CardId.SnakeEyeFlamberge, 2))
                    return true;
            }
            return false;
        }

        public bool IPActivate()
        {
            return SPActivate();
        }

        public bool PromethianPrincessSummon()
        {
            if (!Bot.GetMonsters().Any(x => x.HasAttribute(CardAttribute.Fire)) && !Bot.GetGraveyardMonsters().Any(x => x.HasAttribute(CardAttribute.Fire)))
                return false;
            if (Duel.Turn == 1)
            {
                if (Bot.HasInGraveyard(CardId.DiabellstarBlackWitch) && Bot.GetLinkMaterialWorth() >= 5)
                    return true;
                if (Bot.GetLinkMaterialWorth() >= 4)
                    return true;
                if (Bot.GetLinkMaterialWorth() == 3 && Bot.GetMonsterCount() == 2)
                    return true;
                if (Bot.HasInGraveyard(CardId.SnakeEyeAsh) && !HasPerformedPreviously(CardId.SnakeEyeAsh))
                    return true;

                return false;
            }
            return true;
        }

        public bool PromethianPrincessActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (Enemy.GetMonsters().Where(x => !DONT_DESTROY.Contains(x.Id)).Any())
                    return true;

                return false;
            }

            return true; // Always activate on field
        }

        public bool HiitaSummon()
        {
            if (Bot.GetLinkMaterialWorth(dontUseAsMaterial) < 2)
                return false;
            if (Duel.Turn == 1)
            {
                if (Bot.HasInMonsterZoneSpellZoneOrGraveyard(CardId.SnakeEyeDiabellstar) && Bot.GetLinkMaterialWorth() >= 5)
                    return true;
                return false;
            }
            if (Enemy.GetGraveyardMonsters().Where(x => x.HasAttribute(CardAttribute.Fire)).Any())
                return true;

            return false;
        }

        public bool DharcSummon()
        {
            if (Bot.GetLinkMaterialWorth(dontUseAsMaterial) < 2)
                return false;
            if (Enemy.GetGraveyardMonsters().Where(x => x.HasAttribute(CardAttribute.Dark)).Any())
                return true;

            return false;
        }

        public bool SeleneMagicianSummon()
        {
            int spellCount = 0;
            spellCount += Bot.GetGraveyardSpells().Count;
            spellCount += Bot.GetSpellCount(); // might be off due to facedown cards
            spellCount += Enemy.GetGraveyardSpells().Count;
            spellCount += Enemy.GetSpellCount(); // might be off due to facedown cards

            if (spellCount < 3)
                return false;

            if (Bot.GetMonsters().Any(x => x?.HasRace(CardRace.SpellCaster) ?? false))
                return true;
            return false;
        }

        public bool AnimaSummon()
        {
            if (Bot.GetMonsters().ContainsCardWithId(CardId.SnakeEyePoplar))
                return true;
            if (Bot.GetMonsterCount() == 2)
                return true;
            return false;
        }

        public bool RagingPhoenixSummon()
        {
            int[] dontuse = (int[])dontUseAsMaterial.Clone();
            int[] alsodontuse = { CardId.SnakeEyeFlamberge, CardId.NecroqiopPrincess };
            dontuse = dontUseAsMaterial.Union(alsodontuse).ToArray();

            if (Bot.GetLinkMaterialWorth(dontuse) >= 4)
                return true;
            if (Duel.Turn > 1 && Bot.GetLinkMaterialWorth() >= 5)
                return true;
            if (Bot.HasInMonstersZone(CardId.PromethianPrincess) && Bot.GetMonsterCount() >= 2 && Enemy.GetMonsterCount() >= 1)
                return true;
            return false;
        }

        public bool SPSummon()
        {
            if (Bot.GetLinkMaterialWorth(dont_use_as_link_mat) < 2)
                return false;
            if (Duel.Turn == 1)
            {
                if (Enemy.HasInMonstersZone(CardId.Nibiru) && Bot.GetMonsters().Where(x => x.IsExtraCard()).Any())
                    return true;
            }
            else if (Bot.GetFieldCount() == 2 && Bot.GetMonsters().Where(x => x.IsExtraCard()).Any())
                return true;
            return false;
        }

        public bool SPActivate()
        {
            return true;
        }

        public bool AccessCodeSummon()
        {
            if (Duel.Turn == 1)
                return false;
            if (Duel.Player == 1)
                return false;
            if (Enemy.GetMonsterCount() == 0)
                return false;


            if (Bot.GetMonsters().Where(x => x.HasType(CardType.Link) && x.LinkCount >= 2).Any())
                return true;

            return false;
        }

        public bool AccessCodeActivate()
        {
            if (Util.GetOptionFromDesc(ActivateDescription) == 0)
                return true;

            if (Bot.GetGraveyardMonsters().Where(x => x.HasType(CardType.Link)).Any())
                return true;

            return false;
        }

        public bool KnightmarePhoenixSummon()
        {
            if (Enemy.GetSpells().Any(x => SPELL_FIELD_TARGETS.Any(y => y == x.Id)))
                return true;
            return false;
        }

        public bool ZealantisSummon()
        {
            if (Bot.GetMonsters().ContainsCardWithId(CardId.SalamangreatRagingPhoenix))
            {
                // Set up zelantis otk
                if (Enemy.GetMonsterCount() > 0)
                {
                    if (Bot.GetMonsters().Count() >= 2)
                        return true;
                }
                else if (Bot.GetMonsters().Count() >= 3)
                {
                    if (Duel.Turn > 1)
                    {
                        // OTK with phoenix
                        return true;
                    }
                }
            }
            else if (Bot.GetMonsters().Where(x => x.Id == CardId.Apollusa && x.Attack < 1600).Any())
                return true;

            return false;
        }

        public bool UnderworldGoddessSummon()
        {
            return false;
        }

        public bool LinkSpiderSummon()
        {
            if (Bot.GetFieldCount() > 1)
                return true;
            return false;
        }

        public bool ClosedMoonSpecial()
        {
            if (!Bot.HasInHandOrInSpellZone(CardId.OriginalSinfulSpoilsSnakeEyes) || !Bot.HasInSpellZone(CardId.SnakeEyeDiabellstar) || Bot.GetLinkMaterialWorth(dontUseAsMaterial) < 2)
                return false;
            if (HasPerformedPreviously(CardId.FiendsmithRequiem))
                return false;
            return true;
        }

        public bool ShavaraActivate()
        {
            return true;
        }

        public bool BeatriceActivate()
        {
            return true;
        }

        public bool BeatriceSummon()
        {
            return true;
        }

        public bool VarudrasActivate()
        {
            return true;
        }

        public bool CaesarActivate()
        {
            return true;
        }

        public bool RageActivate()
        {
            return true;
        }

        public bool MuckrackerActivate()
        {
            return true;
        }

        public bool MuckrackerSummon()
        {
            if (Bot.GetLinkMaterialWorth(dont_use_as_link_mat) < 2)
                return false;
            if (Enemy.HasInSpellZone(CardId.AntiSpellFragrance))
                return true;
            return false;
        }

        public bool YamaSummon()
        {
            if (Bot.GetLinkMaterialWorth(dont_use_as_link_mat) < 2)
                return false;
            return true;
        }

        public bool PhoenixSummon()
        {
            if (Enemy.GetSpells().Any(x => SPELL_FIELD_TARGETS.Any(y => y.Equals(x))))
                return true;
            return false;
        }

        public bool ClosedHeavenSummon()
        {
            return true;
        }

        public bool BlackroseSummon()
        {
            if (Duel.Turn == 1)
                return false;

            if (Duel.Phase == DuelPhase.Main1)
            {
                if (!AccessToKaimen())
                    return false;
            }

            if (Enemy.GetFieldCount() >= 5)
                return true;

            return false;
        }

        public bool MoonlightSummon()
        {
            if (Duel.Phase == DuelPhase.Main1)
            {
                return false;
            }

            return false;
        }

        public bool TzolkinSummon()
        {
            if (Duel.Turn != 1)
                return false;

            return true;
        }

        public bool KuibeltSummon()
        {
            if (Duel.Turn == 1)
                return false;
            if (!AccessToKaimen())
                return false;
            if (Enemy.GetMonsters().Where(x => x.Attack >= 2700).Count() >= 2)
                return true;
            return false;
        }

        public bool AncientFairySummon()
        {
            if (Duel.Turn != 1)
                return false;
            if (Bot.HasInHand(CardId.TenpaiFadra))
                return true;
            return false;
        }

        public bool SpheresSummon()
        {
            if (Duel.Turn == 1 || Duel.Phase == DuelPhase.Main2)
                return true;
            return false;
        }

        public bool SpheresActivate()
        {
            if (Enemy.HasInHandOrInSpellZone(MONSTER_FIELD_TARGETS))
                return true;
            if (Duel.ChainTargets.Any(x => x.IsCode(CardId.HieraticSealsOfSpheres)))
                return true;

            return false;
        }

        public bool PrincessSummon()
        {
            if (Bot.GetLinkMaterialWorth() >= 4 && Enemy.GetMonsterCount() > 1)
                return true;
            if (!Bot.HasInMonstersZone(CardId.TenpaiChundra) && !Bot.HasInMonstersZone(CardId.TenpaiFadra))
                return true;

            return false;
        }

        public bool HiidaSummon()
        {
            if (!Enemy.GetGraveyardMonsters().Any(x => x.HasAttribute(CardAttribute.Fire)))
                return false;
            if (!AccessToKaimen() || previousActionsEnemy.Any(x => x.cardId == CardId.DimensionalBarrier))
                return true;

            return false;
        }

        public bool BackJackActivate()
        {
            return true;
        }

        public bool ChaosAngelSpecial()
        {
            return true;
        }

        public bool MuckrackerSpecial()
        {
            return true;
        }

        public bool AnguishSummon()
        {
            return true;
        }

        public bool AbominationSummon()
        {
            return true;
        }

        public bool PackbitActivate()
        {
            return true;
        }

        public bool BerfometActivate()
        {
            return true;
        }

        public bool AbominationActivate()
        {
            return true;
        }

        public bool SpActivate()
        {
            return true;
        }

        public bool LordPrisonActivate()
        {
            return true;
        }

        #endregion


        #region Generic Spells
        public bool OneforOneActivate()
        {
            if (!HasPerformedPreviously(CardId.SnakeEyeAsh, ExecutorType.Activate) || !HasPerformedPreviously(CardId.SnakeEyeOak, ExecutorType.Activate))
                return true;
            return false;
        }

        public bool InstantFusionActivate()
        {
            return true;
        }

        public bool EclipseActivate()
        {
            return true;
        }
        #endregion


        #region Generic Traps
        public bool EscapeActivate()
        {
            if (Enemy.GetMonsters().Any(x => MONSTER_FIELD_TARGETS.Any(y => y == x.Id)))
                return true;
            if (Enemy.GetSpells().Any(x => SPELL_FIELD_TARGETS.Any(y => y == x.Id)))
                return true;
            return false;
        }

        public bool SolemnJudgmentActivate()
        {
            return DefaultSolemnJudgment();
        }

        public bool DarumaActivate()
        {
            if (Duel.Player == 0)
                return false;
            if (Enemy.GetMonsterCount() == 0)
                return false;
            if (Duel.Phase == DuelPhase.Battle || Duel.Phase == DuelPhase.BattleStart)
                return true;
            if (Duel.ChainTargets.Any(x => x.Equals(Card)))
                return true;
            if (Enemy.GetMonsterCount() >= Bot.GetMonsterCount() + 2)
                return true;
            return false;
        }

        public bool PunishmentActivate()
        {
            if (Enemy.GetMonsters().Any(x => MONSTER_FIELD_TARGETS.Any(y => y == x.Id)))
            {
                return true;
            }
            if (Enemy.GetSpells().Any(x => SPELL_FIELD_TARGETS.Any(y => y == x.Id)) &&
                Enemy.GetMonsters().Any(x => x.Attack <= 2500) && Bot.ExtraDeck.ContainsCardWithId(CardId.ElderEntityNtss)

                )
            {
                return true;
            }
            return false;
        }

        public bool TraptrickActivate()
        {
            if (Duel.Player == 0)
                return true;
            else if (Bot.GetSpells().Count(x => x.IsFacedown()) <= 2) // todo count non activatable spell traps
                return true;
            return false;
        }
        #endregion


        #region SnakeEyes

        public bool SnakeEyeAshSummon()
        {
            if (Bot.HasInHand(CardId.SnakeEyeOak) && SnakeEyeOakSummon())
                return false;

            return true;
        }

        public bool SnakeEyeAshActivate()
        {
            if (Card.IsDisabled())
                return false;

            if (ActivateDescription == Util.GetStringId(CardId.SnakeEyeAsh, 0)) // Currently broken, value is 62205969853120512
            {
                return true;
            }
            if (ActivateDescription == Util.GetStringId(CardId.SnakeEyeAsh, 1))
            {
                return SnakeEyeSummonFromDeck();
            }

            return true;
        }

        public bool SnakeEyeOakSummon()
        {
            if (Bot.GetGraveyardMonsters().Any(x => x.Level == 1 && x.HasAttribute(CardAttribute.Fire)) || Bot.Banished.Any(x => x.Level == 1 && x.HasAttribute(CardAttribute.Fire)))
                return true;

            return false;
        }

        public bool SnakeEyeOakActivate()
        {
            if (Card.IsDisabled())
                return false;

            if (ActivateDescription == Util.GetStringId(CardId.SnakeEyeOak, 0))
            {
                return true;
            }
            if (ActivateDescription == Util.GetStringId(CardId.SnakeEyeOak, 1))
            {
                return SnakeEyeSummonFromDeck();
            }

            return true;
        }

        public bool SnakeEyeSummonFromDeck()
        {
            if (Bot.HasInSpellZone(SNAKE_EYE_SEND_SPELL_COST))
                return true;

            if (Bot.GetMonsterCount() >= 3 && Bot.GetMonsters().Where(x => x.Level == 1 && x.HasAttribute(CardAttribute.Fire)).Count() >= 2)
                return true;

            if (Bot.GetMonsterCount() == 2 && Bot.HasInMonstersZone(CardId.RelinquishdAnima))
                return true;

            return false;
        }

        public bool SnakeEyeFlambergeActivate()
        {
            if (Card.IsDisabled())
                return false;

            var a = Util.GetStringId(CardId.SnakeEyeFlamberge, 0);
            var b = ActivateDescription;

            if (Duel.Player == 0) // Own turn effect to place
            {
                if (Duel.Turn != 1)// Non first turn actions
                {
                    foreach (int id in MONSTER_FIELD_TARGETS)
                        if (Enemy.HasInMonstersZone(id))
                            return true;
                }

                // Other targets to place
                if (Bot.HasInGraveyard(CardId.IPMasquerena))
                    return true;
            }
            else // Enemy Turn
            {
                return true; // Shot gun whatever
            }

            // If in GY activate float effect
            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }

            return false;
        }

        public bool DiabellstarBlackWitchSPSummon()
        {
            if (Bot.HasInSpellZone(SNAKE_EYE_SEND_SPELL_COST))
                return true;
            if (Bot.Hand.Count() > 2)
                return true;

            return true;
        }

        public bool SnakeEyeDiabellstarActivate()
        {
            // Battle phase effect
            if (Card.Location == CardLocation.MonsterZone)
                return true;

            return true;
        }

        public bool OSSSEActivate()
        {
            if (HasPerformedPreviously(CardId.SnakeEyeAsh, ExecutorType.Activate) && HasPerformedPreviously(CardId.SnakeEyeOak, ExecutorType.Activate))
                return false;

            if (Bot.HasInMonsterZoneSpellZone(SNAKE_EYE_SEND_SPELL_COST))
                return true;

            return false;
        }

        #endregion

        #region Yubel
        public bool GraveSquirmerActivate()
        {
            return true;
        }

        public bool NightmarePainActivate()
        {
            return true;
        }

        public bool SpiritGatesActivate()
        {
            return true;
        }

        public bool PhantomActivate()
        {
            return true;
        }

        public bool PhantomSummon()
        {
            return true;
        }

        #endregion

        #region Tenpai
        public bool SummoningActivate()
        {
            if (Bot.GetFieldSpellCard() != null && Card.Location == CardLocation.Hand)
                return false;
            return true;
        }

        public bool KaimenActivate()
        {
            if (Duel.Phase == DuelPhase.Battle || Duel.Phase == DuelPhase.BattleStart)
                return true;
            if (Duel.CurrentChain.Count >= 1 && Util.GetLastChainCard().IsCode(CardId.DrollnLockBird))
                return true;
            return false;
        }

        public bool ChundraActivate()
        {
            var option = Util.GetOptionFromDesc(ActivateDescription);
            if (option == 0)
                return TenpaiSynchroBattle();

            return true;
        }

        public bool PaidraActivate()
        {
            var option = Util.GetOptionFromDesc(ActivateDescription);
            if (option == 1)
                return TenpaiSynchroBattle();

            return true;
        }

        public bool FadraActivate()
        {
            var option = Util.GetOptionFromDesc(ActivateDescription);
            if (option == 1)
                return TenpaiSynchroBattle();

            return true;
        }

        bool TenpaiSynchroBattle()
        {
            if (Duel.LastChainPlayer == 0)
                return false;

            // Check if all monsters have attacked
            if (CanAttack())
                return false;
            return true;
        }

        public bool TranscendentSummon()
        {
            if (Duel.Turn == 1)
                return false;

            if (Duel.Phase == DuelPhase.Main1)
            {
                if (!AccessToKaimen())
                    return false;
            }

            return true;
        }

        public bool BidentSummon()
        {
            if (Duel.Turn == 1)
                return false;

            if (Duel.Phase == DuelPhase.Main1)
            {
                if (!AccessToKaimen())
                    return false;
            }

            return true;
        }

        public bool TridentSummon()
        {
            if (Duel.Turn == 1)
                return false;

            if (Duel.Phase == DuelPhase.Main1)
            {
                return false;
            }

            return true;
        }

        public bool AccessToKaimen()
        {
            if (HasPerformedPreviously(CardId.SangenKaimen))
                return false;

            if (Bot.HasInHand(CardId.SangenKaimen))
                return true;
            return false;
        }

        public bool PokiDracoActivate()
        {
            if (Util.GetOptionFromDesc(ActivateDescription) == 1)
                return false;
            return true;
        }

        private IList<ClientCard> TenpaiBattlePhaseSynchro(IList<ClientCard> _cards)
        {
            IList<ClientCard> selected = new List<ClientCard>();
            if (!HasPerformedPreviously(CardId.SangenpaiBidentDragion))
                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.SangenpaiBidentDragion));
            if (!HasPerformedPreviously(CardId.SangenpaiTranscendentDragion))
                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.SangenpaiTranscendentDragion));
            if (!HasPerformedPreviously(CardId.TridentDragion))
                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.TridentDragion));
            return selected;
        }

        public bool CanAttack()
        {
            if (Bot.Deck.Any(x => x.Id == CardId.TenpaiFadra))
            {
                var a = 1;
            }
            if (!Bot.GetMonsters().Any(x => !x.Attacked))
                return false;
            if (Bot.GetMonsters().Any(x => x.Id == CardId.TenpaiFadra && !x.IsDisabled()))
                return true;
            if (Bot.GetMonsters().Any(x => x.Id == CardId.TenpaiChundra && !x.IsDisabled()) &&
               !HasPerformedPreviously(CardId.TenpaiChundra, 2))
                return true;

            if (Bot.GetMonsters().Any(x => !x.Attacked && Enemy.GetMonsters().Any(y => y.GetDefensePower() <= x.Attack)))
                return true;
            return false;
        }

        #endregion

        #region Runick
        public bool GoldenDropletActivate()
        {
            if (SpecialRunick())
                return true;
            if (Enemy.Deck.Count < 15)
                return true;
            return false;
        }

        public bool FreezingCurseActivate()
        {
            if (SpecialRunick())
                return true;
            if (FaceUpEffectNegate())
                return true;
            if (Bot.GetMonstersInExtraZone() != null && Duel.Phase == DuelPhase.End)
                return true;
            return false;
        }

        public bool TipActivate()
        {
            if (SpecialRunick())
                return true;
            if (Duel.LastChainPlayer == 0)
                return false;
            return true;
        }

        public bool DispellingActivate()
        {
            if (SpecialRunick())
                return true;
            if (Bot.GetMonstersInExtraZone() != null)
                return true;
            return false;
        }

        public bool SlumberActivate()
        {
            if (SpecialRunick())
                return true;
            if (Duel.Player == 1 && Duel.Phase.HasFlag(DuelPhase.BattleStep))
                return true;
            if (Bot.GetMonstersInExtraZone() != null && Duel.Phase == DuelPhase.End)
                return true;
            return false;
        }

        public bool FlashingFireActivate()
        {
            if (SpecialRunick())
                return true;
            if (Enemy.GetMonsters().Any(x => MONSTER_FIELD_TARGETS.Any(y => y == x.Id)))
                return true;
            if (Bot.GetMonstersInExtraZone() != null && Duel.Phase == DuelPhase.End)
                return true;
            return false;
        }

        public bool StormActivate()
        {
            if (SpecialRunick())
                return true;
            if (Enemy.GetFieldCount() >= 4)
                return true;
            return false;
        }

        public bool DestructionActivate()
        {
            if (SpecialRunick())
                return true;
            if (Enemy.GetSpells().Any(x => SPELL_FIELD_TARGETS.Any(y => y == x.Id)))
                return true;
            if (CosmicActivate())
                return true;
            if (Bot.GetMonstersInExtraZone() != null && Duel.Phase == DuelPhase.End)
                return true;
            return false;
        }

        public bool FountainActivate()
        {
            if (!Bot.HasInHand(RunickSpells))
                return true;
            if (Bot.GetGraveyardSpells().Count(x => RunickSpells.Any(y => y == x.Id)) >= 2)
                return true;
            return false;
        }

        public bool SleipnirActivate()
        {
            return true;
        }

        public bool MuninActivate()
        {
            return true;
        }

        public bool HuginActivate()
        {
            return true;
        }

        public bool SpecialRunick()
        {
            if (Duel.LastChainPlayer == 0)
                return false;

            int[] protectAgainst =
            {
                CardId.KnightmarePhoenix,
                CardId.FeatherDuster,
                CardId.LightningStorm,
            };

            if (Duel.CurrentChain.Count >= 1 && Duel.CurrentChain.Any(x => x.Controller == 1 && protectAgainst.Any(y => y == x.Id)))
                return true;
            if (!Bot.HasInHandOrInSpellZone(CardId.RunickFountain))
                return true;
            if (Duel.Phase.HasFlag(DuelPhase.BattleStep) && Bot.GetMonstersInExtraZone() == null)
                return true;

            return false;
        }
        #endregion

        #region Labrynth
        public bool GreenActivate()
        {
            return true;
        }

        public bool FurnatureActivate()
        {
            if (Duel.Phase == DuelPhase.End)
                return true;
            if (HasPerformedPreviously(CardId.LabrynthCooClock))
                return true;
            if (Bot.HasInHand(CardId.BackJack))
                return true;
            return false;
        }

        public bool ClockActivate()
        {
            return true;
        }

        public bool LovelyActivate()
        {
            return true;
        }

        public bool LadyActivate()
        {
            return true;
        }


        public bool BigWelcomeActivate()
        {
            if (Enemy.GetMonsters().Any(x => MONSTER_FIELD_TARGETS.Any(y => y == x.Id)))
                return true;
            if (Duel.Player == 0)
                return true;
            if (Duel.Phase == DuelPhase.End)
                return true;
            if (Duel.Phase == DuelPhase.BattleStart)
                return true;
            return false;
        }

        public bool WelcomeActivate()
        {
            return true;
        }

        public bool SetupActivate()
        {
            if (Bot.GetMonsters().Any(x => x.HasRace(CardRace.Fiend) && x.IsFaceup()))
                return true;
            return false;
        }

        public bool SimultaneousCannonActivate()
        {
            if (SimultaneousCannonLevels() != null)
                return true;

            return false;
        }

        // First is fusion, second value is xyz
        public int[] SimultaneousCannonLevels()
        {
            List<int> fusionLevels = new List<int>();
            List<int> xyzPairs = new List<int>();

            List<int> targetLevels = new List<int>();
            int totalCards = Bot.GetFieldHandCount() + Enemy.GetFieldHandCount();

            foreach (var card in Enemy.GetMonsters())
            {
                int level = card.Level;
                if (!targetLevels.Contains(level))
                    targetLevels.Add(level);
            }

            // Find a fusion first
            foreach (var fusionCard in Bot.ExtraDeck)
            {
                if (fusionCard == null)
                    continue;
                if (!fusionCard.HasType(CardType.Fusion))
                    continue;

                int fusionLevel = fusionCard.Level;
                List<int> xyzLevels = new List<int>(); // This holds single xyz leves
                foreach (var xyzCard in Bot.ExtraDeck) // Find 2 xyz now
                {
                    if (xyzCard == null)
                        continue;
                    if (!xyzCard.HasType(CardType.Xyz))
                        continue;
                    if (xyzLevels.Contains(xyzCard.Level))
                    {
                        if (fusionLevel + 2 * xyzCard.Level == totalCards && targetLevels.Contains(fusionLevel + xyzCard.Level))
                            return new int[] { fusionLevel, xyzCard.Level };
                    }
                    else
                        xyzLevels.Add(xyzCard.Level);
                }
            }


            return null;
        }
        #endregion


        #region Branded
        public bool CartesiaActivate()
        {
            return true;
        }

        public bool MercourierActivate()
        {
            return true;
        }

        public bool HighSpiritsActivate()
        {
            return true;
        }

        public bool BrandedInRedActivate()
        {
            return true;
        }

        public bool RetributionActivate()
        {
            return true;
        }

        public bool DuplicationActivate()
        {
            return true;
        }


        public bool FuriousActivate()
        {
            return true;
        }

        public bool MirrorjadeActivate()
        {
            return true;
        }

        public bool QuartusActivate()
        {
            return true;
        }

        public bool RindbrummActivate()
        {
            return true;
        }

        public bool BlazingBrandedActivate()
        {
            return true;
        }

        #endregion

        public bool HasCombo()
        {
            return (SNAKE_EYES_STARTER.Any(x => Bot.HasInHandOrInSpellZone(x) && !HasPerformedPreviously(x)));
        }
    }
}
